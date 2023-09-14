using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.IO;
using Common;
using ImageAcquisition;
using SharedMemory;
using VisionProcessing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using IOMode;
using Lighting;
using Microsoft.Win32;
using System.Linq;

namespace VisionModule
{
    public class Vision3Lead3DProcess
    {
        [DllImport("kernel32.dll")]
        static extern void Sleep(uint dwMilliseconds);

        #region constant variables

        //private const int BUFFERSIZE = 20;

        #endregion

        #region enum

        public enum ResulType { Pass, PassPH, FailLead, FailPin1, FailPosition, FailPackage, Timesout, FailPH, NotReady,
                                FailLeadOffset, FailLeadSkew, FailLeadWidth, FailLeadLength, FailLeadLengthVariance,
                                FailLeadPitchGap, FailLeadPitchVariance, FailLeadStandOff, FailLeadStandOffVariance, FailLeadCoplan, FailLeadAGV,
                                FailLeadSpan, FailLeadSweeps, FailLeadUnCutTiebar, FailLeadMissing, FailLeadContamination,
                                FailLeadPkgDefect, FailLeadPkgDimension , FailEdgeNotFound};

        public enum TCPIPResulID
        {
            Fail = 0, /*FailMark = 1, FailPackage = 2,*/ FailPosition = 3//, FailNotSeatProper = 4, FailCrack = 11, FailForeignMaterial = 12,
            //FailPackageDimension = 13, FailVoid = 14, FailChippedOffOrScractches = 15, FailCopper = 16, FailDiscolouration = 17, FailEmpty = 18,
            //FailNoMark = 19, Fail2DCodeNoFound = 20, Fail2DCodeVerification = 21
        };
        #endregion

        #region Member Variables
        private int m_intCounter = 0;
        private bool m_blnAuto;
        private bool m_blnPreviouslyIsLead = true;
        // Thread handle
        private readonly object m_objStopLock = new object();
        private bool m_blnStopping = false;
        private bool m_blnPause = false;
        private bool m_blnCustomWantPositioning = false;
        private bool m_blnCustomWantPackage = false;
        private bool m_blnCustomWant5S = false;
        private bool m_blnCustomWantPad = false;
        private bool m_blnCustomWantLead3D = false;
        private bool m_blnRotateImageUpdated = false;
        private bool m_blnForceStopProduction = false;
        private bool m_blnLoadRejectImageListPath = false;
        private bool[] m_arr5SFoundUnit = new bool[5];
        private bool[] m_arr5SFoundUnitPkg = new bool[5];
        private bool[] m_arr5SPackagedSizeChecked = new bool[5];
        private List<bool>[] m_arr5SImageRotated2 = new List<bool>[5];  // mean m_arr5SImageRotated2[5][] where 1st array index is center and side ROI index, 2nd array index is image index
        private int m_intPassStartNode = 0;
        private int m_intFailStartNode = 0;
        private int m_intPassEndNode = 0;
        private int m_intFailEndNode = 0;
        private int m_intGrabIndex = 0;
        private int m_intGrabRequire = 0;
        private int m_intFileIndex = 0;
        private uint m_intCameraGainPrev = 1;
        private int[] m_arrPassNoBuffer = null;
        private int[] m_arrFailNoBuffer = null;
        private string[] m_arrRejectNameBuffer = null;
        private string[] m_arrRejectMessageBuffer = null;
        private List<string> m_arrRejectImageListPath = new List<string>();
        private List<string> m_arrRejectImageErrorMessageListPath = new List<string>();
        private float m_fCameraShuttlePrev = 1f;
        private int m_intCameraOutState = 1;
        private int[] m_arrCameraIntensityPrev = { -1, -1, -1, -1 }; // index == light source number. Maximum for each camera link to light source is only four
        private string[] m_arrErrorMessage = new string[5];

        private int[] m_arrPHCameraIntensityPrev = { -1, -1, -1 };//PH
        private float m_fPHCameraShuttlePrev = 1f;//PH
        private uint m_uintPHCameraGainPrev = 1;//PH

        private bool m_blnStopped = false, m_blnStopped_GrabImage = false, m_blnStopped_AfterInspect, m_blnStopped_SaveImage;
        private bool m_blnStopped_CenterThread = false, m_blnStopped_SideTLThread = false, m_blnStopped_SideBRThread = false;
        private Thread m_thThread, m_thSubThread_GrabImage, m_thSubThread_AfterInspect, m_thSubThread_SaveImage;
        private Thread m_thSubThread_Center, m_thSubThread_SideTL, m_thSubThread_SideBR;
        private VisionIO m_objVisionIO;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;
        private AVTVimba m_objAVTFireGrab;
        private IDSuEyeCamera m_objIDSCamera;
        private TeliCamera m_objTeliCamera;
        private VisionComThread m_smComThread;
        private TCPIPIO m_smTCPIPIO;
        private RS232 m_thCOMMPort;

        private ImageDrawing m_objTempImage;
        private ImageDrawing m_objGainImage;
        private ImageDrawing m_objDestImage;
        private ImageDrawing m_objTemporaryRotateSource;
        private ImageDrawing[] m_arrTempImage;              // 0=Center, 1=Top, 2=Right, 3=Bottom, 4=Left
        private ImageDrawing[] m_arrGainImage;              // 0=Center, 1=Top, 2=Right, 3=Bottom, 4=Left
        private ImageDrawing[] m_arrTempGaugeImage;         // 0=Center, 1=Top, 2=Right, 3=Bottom, 4=Left   // Used to keep pre process image "Gradient, Prewitt, LowPass, HighPass, etc" before gauge measuremnt.
        private ImageDrawing[] m_arrTemporaryRotateSource;
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
        private ROI[] m_arrRotatedROI;

        TrackLog m_objPosTL;
        private int m_intDebugHour = 0;

        private bool m_bSubTh1_GrabImage = false;
        private bool m_bSubTh2_SideTest = false;
        private bool m_bSubTh_CenterTest = false;
        private bool m_bSubTh_SideTLTest = false;
        private bool m_bSubTh_SideBRTest = false;
        private bool m_bSubTh_SideTLTest_LeadEdgeDone = false;
        private bool m_bSubTh_SideBRTest_LeadEdgeDone = false;
        private bool m_bSubTh_SideTLTest_LeadEdgeResult = false;
        private bool m_bSubTh_SideBRTest_LeadEdgeResult = false;
        private bool m_bSubTh_SideTest_BuildSideLeadFail = false;
        private bool m_bSubTh_StartAfterInspect = false;
        private bool m_bSubTh_CenterTest_Result = false;
        private bool m_bSubTh_SideTLTest_Result = false;
        private bool m_bSubTh_SideBRTest_Result = false;
        private bool m_bSubTh_PHTest = false;
        private bool m_bSubTh_PHTest_Result = false;
        private bool m_bWantPHTest = false;
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
        private bool m_bGrabImageFinalResult = false;
        private ResulType m_eInspectionResult_Center = ResulType.Pass;
        private ResulType m_eInspectionResult_SideTL = ResulType.Pass;
        private ResulType m_eInspectionResult_SideBR = ResulType.Pass;


        //TCPIPIO
        private int m_intTCPIPResultID = -1;
        private float m_fOffsetX = 0;
        private float m_fOffsetY = 0;
        private float m_fOffsetAngle = 0;
        private bool m_blnStartVision_In = false;
        private bool m_blnEndVision_Out = true;
        private bool m_blnGrabbing_Out = false;
        private bool m_blnCheckOffset_In = false;
        private bool m_blnPass1_Out = true;
        private bool m_blnOrientResult2_Out = false;
        private bool m_blnOrientResult1_Out = false;
        private bool m_blnPackageFail_Out = false;        // Fail criteria: Package
        private bool m_blnPositionReject_Out = false;     // Fail criteria: Position Reject
        private bool m_blnCheckPH_In = false;

        #endregion

        HiPerfTimer m_T1 = new HiPerfTimer();
        HiPerfTimer m_T2 = new HiPerfTimer();
        string m_strTrack = "", m_strTrack_Center = "", m_strTrack_TL = "", m_strTrack_BR = "";
        float m_fTimingPrev = 0, m_fTimingPrev2 = 0;
        float m_fTiming = 0, m_fTiming2 = 0;
        private int m_intLightControlModel = 1; // 0=LEDi, 1=VTControl

        private int m_int_PadInspection_ViewImageIndex = 0;
        private int m_int_PackageInspection_ViewImageIndex = 1;

        private int intReadIndex = 0;
        public Vision3Lead3DProcess(CustomOption objCustomOption, ProductionInfo smProductionInfo, VisionInfo objVisionInfo,
            AVTVimba objAVTFireGrab, VisionComThread smComThread, RS232 thCOMMPort, TCPIPIO smTCPIPIO)
        {
            m_smCustomizeInfo = objCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = objVisionInfo;
            m_objAVTFireGrab = objAVTFireGrab;
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
            //m_arrRejectNameBuffer = new string[BUFFERSIZE];


            GetCustomTest();

            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                m_smTCPIPIO.ReceiveCommandEvent += new TCPIPIO.ReceiveCommandHandle(TakeAction_TCPIPIO);
            else
                m_smComThread.ReceiveCommandEvent += new VisionComThread.ReceiveCommandHandle(TakeAction);


            int intEnableGrabIOMask;
            if (m_smVisionInfo.g_strCameraModel == "IDS")
                intEnableGrabIOMask = 0x03;
            else
                intEnableGrabIOMask = 0x00;

            //create vision io object
            m_objVisionIO = new VisionIO(m_smVisionInfo.g_strVisionName, m_smVisionInfo.g_strVisionDisplayName,
                                         m_smVisionInfo.g_intVisionIndex, m_smVisionInfo.g_intVisionSameCount,
                                         m_smVisionInfo.g_strVisionNameRemark, intEnableGrabIOMask);

            m_objTempImage = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
            m_objGainImage = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
            m_objTemporaryRotateSource = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);

            m_arrTempImage = new ImageDrawing[5];
            m_arrGainImage = new ImageDrawing[5];
            m_arrTempGaugeImage = new ImageDrawing[5];
            m_arrRotatedROI = new ROI[5];
            m_arrTemporaryRotateSource = new ImageDrawing[5];
            for (int i = 0; i < 5; i++)
            {
                m_arrTempImage[i] = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_arrGainImage[i] = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_arrTemporaryRotateSource[i] = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_arrTempGaugeImage[i] = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_arrRotatedROI[i] = new ROI();

            }

            //List<string> arrThreadNameBF = new List<string>();
            //List<string> arrThreadNameAF = new List<string>();
            //arrThreadNameBF = ProcessTh.GetThreadsName("SRMVision");

            m_thThread = new Thread(new ThreadStart(UpdateProgress));
            m_thThread.IsBackground = true;
            m_thThread.Priority = ThreadPriority.Highest;
            m_thThread.Start();

            m_thSubThread_GrabImage = new Thread(new ThreadStart(UpdateSubProgress_GrabImage));
            m_thSubThread_GrabImage.IsBackground = true;
            m_thSubThread_GrabImage.Priority = ThreadPriority.Highest;
            m_thSubThread_GrabImage.Start();

            m_thSubThread_Center = new Thread(new ThreadStart(UpdateSubProgress_Center));
            m_thSubThread_Center.IsBackground = true;
            m_thSubThread_Center.Priority = ThreadPriority.Highest;
            m_thSubThread_Center.Start();

            m_thSubThread_SideTL = new Thread(new ThreadStart(UpdateSubProgress_SideTL));
            m_thSubThread_SideTL.IsBackground = true;
            m_thSubThread_SideTL.Priority = ThreadPriority.Highest;
            m_thSubThread_SideTL.Start();

            m_thSubThread_SideBR = new Thread(new ThreadStart(UpdateSubProgress_SideBR));
            m_thSubThread_SideBR.IsBackground = true;
            m_thSubThread_SideBR.Priority = ThreadPriority.Highest;
            m_thSubThread_SideBR.Start();

            m_thSubThread_AfterInspect = new Thread(new ThreadStart(UpdateSubProgress_AfterInspect));
            m_thSubThread_AfterInspect.IsBackground = true;
            m_thSubThread_AfterInspect.Priority = ThreadPriority.Normal;
            m_thSubThread_AfterInspect.Start();

            m_thSubThread_SaveImage = new Thread(new ThreadStart(UpdateSubProgress_SaveImage));
            m_thSubThread_SaveImage.IsBackground = true;
            m_thSubThread_SaveImage.Priority = ThreadPriority.Lowest;
            m_thSubThread_SaveImage.Start();

            //Thread.Sleep(500);
            //arrThreadNameAF = ProcessTh.GetThreadsName("SRMVision");
            //ProcessTh.GetDifferentThreadsName(arrThreadNameAF, arrThreadNameBF, "V3a", 0x02);
        }

        public Vision3Lead3DProcess(CustomOption objCustomOption, ProductionInfo smProductionInfo, VisionInfo objVisionInfo,
            IDSuEyeCamera objIDSCamera, VisionComThread smComThread, RS232 thCOMMPort, TCPIPIO smTCPIPIO)
        {
            m_smCustomizeInfo = objCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = objVisionInfo;
            m_objIDSCamera = objIDSCamera;
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
            //m_arrRejectNameBuffer = new string[BUFFERSIZE];

            GetCustomTest();

            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                m_smTCPIPIO.ReceiveCommandEvent += new TCPIPIO.ReceiveCommandHandle(TakeAction_TCPIPIO);
            else
                m_smComThread.ReceiveCommandEvent += new VisionComThread.ReceiveCommandHandle(TakeAction);
           
            int intEnableGrabIOMask;
            if (m_smVisionInfo.g_strCameraModel == "IDS")
                intEnableGrabIOMask = 0x03;
            else
                intEnableGrabIOMask = 0x00;

            //create vision io object
            m_objVisionIO = new VisionIO(m_smVisionInfo.g_strVisionName, m_smVisionInfo.g_strVisionDisplayName,
                                         m_smVisionInfo.g_intVisionIndex, m_smVisionInfo.g_intVisionSameCount,
                                         m_smVisionInfo.g_strVisionNameRemark, intEnableGrabIOMask);

            m_objTempImage = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
            m_objGainImage = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
            m_objTemporaryRotateSource = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);

            m_arrTempImage = new ImageDrawing[5];
            m_arrGainImage = new ImageDrawing[5];
            m_arrTempGaugeImage = new ImageDrawing[5];
            m_arrRotatedROI = new ROI[5];
            m_arrTemporaryRotateSource = new ImageDrawing[5];
            for (int i = 0; i < 5; i++)
            {
                m_arrTempImage[i] = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_arrGainImage[i] = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_arrTempGaugeImage[i] = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_arrTemporaryRotateSource[i] = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_arrRotatedROI[i] = new ROI();

            }

            //List<string> arrThreadNameBF = new List<string>();
            //List<string> arrThreadNameAF = new List<string>();
            //arrThreadNameBF = ProcessTh.GetThreadsName("SRMVision");

            m_thThread = new Thread(new ThreadStart(UpdateProgress));
            m_thThread.IsBackground = true;
            m_thThread.Priority = ThreadPriority.Highest;
            m_thThread.Start();

            m_thSubThread_GrabImage = new Thread(new ThreadStart(UpdateSubProgress_GrabImage));
            m_thSubThread_GrabImage.IsBackground = true;
            m_thSubThread_GrabImage.Priority = ThreadPriority.Highest;
            m_thSubThread_GrabImage.Start();

            m_thSubThread_Center = new Thread(new ThreadStart(UpdateSubProgress_Center));
            m_thSubThread_Center.IsBackground = true;
            m_thSubThread_Center.Priority = ThreadPriority.Highest;
            m_thSubThread_Center.Start();

            m_thSubThread_SideTL = new Thread(new ThreadStart(UpdateSubProgress_SideTL));
            m_thSubThread_SideTL.IsBackground = true;
            m_thSubThread_SideTL.Priority = ThreadPriority.Highest;
            m_thSubThread_SideTL.Start();

            m_thSubThread_SideBR = new Thread(new ThreadStart(UpdateSubProgress_SideBR));
            m_thSubThread_SideBR.IsBackground = true;
            m_thSubThread_SideBR.Priority = ThreadPriority.Highest;
            m_thSubThread_SideBR.Start();

            m_thSubThread_AfterInspect = new Thread(new ThreadStart(UpdateSubProgress_AfterInspect));
            m_thSubThread_AfterInspect.IsBackground = true;
            m_thSubThread_AfterInspect.Priority = ThreadPriority.Normal;
            m_thSubThread_AfterInspect.Start();

            m_thSubThread_SaveImage = new Thread(new ThreadStart(UpdateSubProgress_SaveImage));
            m_thSubThread_SaveImage.IsBackground = true;
            m_thSubThread_SaveImage.Priority = ThreadPriority.Lowest;
            m_thSubThread_SaveImage.Start();

            //Thread.Sleep(500);
            //arrThreadNameAF = ProcessTh.GetThreadsName("SRMVision");
            //ProcessTh.GetDifferentThreadsName(arrThreadNameAF, arrThreadNameBF, "V3b", 0x02);
        }

        public Vision3Lead3DProcess(CustomOption objCustomOption, ProductionInfo smProductionInfo, VisionInfo objVisionInfo,
            TeliCamera objTeliCamera, VisionComThread smComThread, RS232 thCOMMPort, TCPIPIO smTCPIPIO)
        {
            m_smCustomizeInfo = objCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = objVisionInfo;
            m_objTeliCamera = objTeliCamera;
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
            //m_arrRejectNameBuffer = new string[BUFFERSIZE];

            GetCustomTest();

            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                m_smTCPIPIO.ReceiveCommandEvent += new TCPIPIO.ReceiveCommandHandle(TakeAction_TCPIPIO);
            else
                m_smComThread.ReceiveCommandEvent += new VisionComThread.ReceiveCommandHandle(TakeAction);
            
            int intEnableGrabIOMask;
            if (m_smVisionInfo.g_strCameraModel == "IDS")
                intEnableGrabIOMask = 0x03;
            else
                intEnableGrabIOMask = 0x00;

            //create vision io object
            m_objVisionIO = new VisionIO(m_smVisionInfo.g_strVisionName, m_smVisionInfo.g_strVisionDisplayName,
                                         m_smVisionInfo.g_intVisionIndex, m_smVisionInfo.g_intVisionSameCount,
                                         m_smVisionInfo.g_strVisionNameRemark, intEnableGrabIOMask);

            m_objTempImage = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
            m_objGainImage = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
            m_objTemporaryRotateSource = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);

            m_arrTempImage = new ImageDrawing[5];
            m_arrGainImage = new ImageDrawing[5];
            m_arrTempGaugeImage = new ImageDrawing[5];
            m_arrRotatedROI = new ROI[5];
            m_arrTemporaryRotateSource = new ImageDrawing[5];
            for (int i = 0; i < 5; i++)
            {
                m_arrTempImage[i] = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_arrGainImage[i] = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_arrTempGaugeImage[i] = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_arrTemporaryRotateSource[i] = new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                m_arrRotatedROI[i] = new ROI();

            }

            //List<string> arrThreadNameBF = new List<string>();
            //List<string> arrThreadNameAF = new List<string>();
            //arrThreadNameBF = ProcessTh.GetThreadsName("SRMVision");

            m_thThread = new Thread(new ThreadStart(UpdateProgress));
            m_thThread.IsBackground = true;
            m_thThread.Priority = ThreadPriority.Highest;
            m_thThread.Start();

            m_thSubThread_GrabImage = new Thread(new ThreadStart(UpdateSubProgress_GrabImage));
            m_thSubThread_GrabImage.IsBackground = true;
            m_thSubThread_GrabImage.Priority = ThreadPriority.Highest;
            m_thSubThread_GrabImage.Start();

            m_thSubThread_Center = new Thread(new ThreadStart(UpdateSubProgress_Center));
            m_thSubThread_Center.IsBackground = true;
            m_thSubThread_Center.Priority = ThreadPriority.Highest;
            m_thSubThread_Center.Start();

            m_thSubThread_SideTL = new Thread(new ThreadStart(UpdateSubProgress_SideTL));
            m_thSubThread_SideTL.IsBackground = true;
            m_thSubThread_SideTL.Priority = ThreadPriority.Highest;
            m_thSubThread_SideTL.Start();

            m_thSubThread_SideBR = new Thread(new ThreadStart(UpdateSubProgress_SideBR));
            m_thSubThread_SideBR.IsBackground = true;
            m_thSubThread_SideBR.Priority = ThreadPriority.Highest;
            m_thSubThread_SideBR.Start();

            m_thSubThread_AfterInspect = new Thread(new ThreadStart(UpdateSubProgress_AfterInspect));
            m_thSubThread_AfterInspect.IsBackground = true;
            m_thSubThread_AfterInspect.Priority = ThreadPriority.Normal;
            m_thSubThread_AfterInspect.Start();

            m_thSubThread_SaveImage = new Thread(new ThreadStart(UpdateSubProgress_SaveImage));
            m_thSubThread_SaveImage.IsBackground = true;
            m_thSubThread_SaveImage.Priority = ThreadPriority.Lowest;
            m_thSubThread_SaveImage.Start();

            //Thread.Sleep(500);
            //arrThreadNameAF = ProcessTh.GetThreadsName("SRMVision");
            //ProcessTh.GetDifferentThreadsName(arrThreadNameAF, arrThreadNameBF, "V3c", 0x02);
        }

        /// <summary>
        /// Grab image
        /// </summary>      
        /// <returns>true = successfully grab image, false = fail to grab image</returns>
        public bool GrabImageWithSetIntensity()
        {
            m_smVisionInfo.g_objGrabTime.Start();
            bool blnSuccess = true;
            HiPerfTimer timer_GrabTime = new HiPerfTimer();

            int intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_fCameraShuttle * 0.02f);

            m_objAVTFireGrab.DiscardFrame();
            for (int i = 0; i < m_intGrabRequire; i++)
            {
                #region if more than 1 image need to be captured
                // Set light source channel ON/OFF
                if (m_intGrabRequire > 0)
                {
                    // change light source intensity for different image's effects
                    for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                    {
                        int intValueNo = 0;

                        // Due to some light source only ON for second image so its intensity value is at array no. 0.
                        // So we need to loop to find which array no. is for that image
                        for (int k = 1; k < m_smVisionInfo.g_arrLightSource[j].ref_arrValue.Count; k++)
                        {
                            // if this image no is in array k
                            if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo != null)
                            {
                                if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo[k] == i)
                                {
                                    intValueNo = k;
                                    break;
                                }
                            }
                        }

                        // Set camera gain
                        if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
                        {
                            m_objAVTFireGrab.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
                            m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                        }

                        //switch (m_smCustomizeInfo.g_blnLEDiControl)
                        switch (m_intLightControlModel) // 2018 07 13 - CCENG: Temporary force to use VTControl
                        {
                            case 0:
                                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                {
                                    if (m_arrCameraIntensityPrev[j] != m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo])
                                    {
                                        LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                           m_smVisionInfo.g_arrLightSource[j].ref_intChannel,
                                           Convert.ToByte(m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo]));

                                        m_arrCameraIntensityPrev[j] = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                        Thread.Sleep(5);
                                    }
                                }
                                else
                                {
                                    if (m_arrCameraIntensityPrev[j] != 0)
                                    {
                                        LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                            m_smVisionInfo.g_arrLightSource[j].ref_intChannel, Convert.ToByte(0));

                                        m_arrCameraIntensityPrev[j] = 0;
                                        Thread.Sleep(5);
                                    }
                                }
                                break;
                            case 1:
                                if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                {
                                    if (m_arrCameraIntensityPrev[j] != m_smVisionInfo.g_arrLightSource[j].ref_arrValue[i])
                                    {
                                        TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                            m_smVisionInfo.g_arrLightSource[j].ref_intChannel,
                                            m_smVisionInfo.g_arrLightSource[j].ref_arrValue[i]);

                                        m_arrCameraIntensityPrev[j] = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[i];
                                        Thread.Sleep(2);
                                    }
                                }
                                else
                                {
                                    if (m_arrCameraIntensityPrev[j] != 0)
                                    {
                                        TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                            m_smVisionInfo.g_arrLightSource[j].ref_intChannel, 0);

                                        m_arrCameraIntensityPrev[j] = 0;
                                        Thread.Sleep(2);
                                    }
                                }
                                break;
                        }
                    }
                }
                #endregion

                if (i > 0) // for second image and third image
                {
                    timer_GrabTime.Stop();
                    if ((m_objAVTFireGrab.ref_intNextGrabDelay - timer_GrabTime.Duration) > 0)
                        Thread.Sleep((int)(m_objAVTFireGrab.ref_intNextGrabDelay - timer_GrabTime.Duration));
                }

                if (!m_objAVTFireGrab.Grab())
                {
                    blnSuccess = false;
                    m_blnForceStopProduction = true;
                }

                if (i < m_intGrabRequire - 1)
                {
                    timer_GrabTime.Start();
                    Thread.Sleep(intExposureTime);
                }
                else
                {
                    Thread.Sleep(Math.Max(intExposureTime, 4));
                }
            }

            if (blnSuccess)
            {
                SetGrabDone(false);
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
                        }
                    }
                }
            }
            else
                SetGrabDone(false);

            for (int i = 0; i < m_intGrabRequire; i++)
            {
                m_objAVTFireGrab.ReleaseImage(i);
            }

            if (m_objAVTFireGrab.ref_strErrorText != "")
            {
                m_smVisionInfo.g_strErrorMessage = m_objAVTFireGrab.ref_strErrorText;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();
                return false;
            }
            else
            {
                //AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();
                return true;
            }
            //m_smVisionInfo.g_objGrabTime.Start();
            //bool blnSuccess = true;


            //for (int i = 0; i < m_intGrabRequire; i++)
            //{
            //    #region if more than 1 image need to be captured
            //    // Set light source channel ON/OFF
            //    if (m_intGrabRequire > 1)
            //    { // change light source intensity for different image's effects
            //        for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
            //        {
            //            int intValueNo = 0;

            //            // Due to some light source only ON for second image so its intensity value is at array no. 0.
            //            // So we need to loop to find which array no. is for that image
            //            for (int k = 1; k < m_smVisionInfo.g_arrLightSource[j].ref_arrValue.Count; k++)
            //            {
            //                // if this image no is in array k
            //                if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo[k] == i)
            //                {
            //                    intValueNo = k;
            //                    break;
            //                }
            //            }
            //            // Set camera gain
            //            if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
            //            {
            //                m_objAVTFireGrab.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
            //                m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
            //            }
            //            switch (m_smCustomizeInfo.g_blnLEDiControl)
            //            {
            //                case true:
            //                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
            //                        LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
            //                            m_smVisionInfo.g_arrLightSource[j].ref_intChannel,
            //                            Convert.ToByte(m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo]));
            //                    else
            //                        LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
            //                            m_smVisionInfo.g_arrLightSource[j].ref_intChannel, Convert.ToByte(0));
            //                    Thread.Sleep(3);
            //                    break;
            //                case false:
            //                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
            //                        TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
            //                            m_smVisionInfo.g_arrLightSource[j].ref_intChannel, m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo]);
            //                    else
            //                        TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
            //                            m_smVisionInfo.g_arrLightSource[j].ref_intChannel, 0);
            //                    Thread.Sleep(3);
            //                    break;
            //            }
            //        }
            //    }
            //    #endregion

            //    if (!m_objAVTFireGrab.Grab())
            //        blnSuccess = false;

            //    Thread.Sleep(3);
            //}

            //if (blnSuccess)
            //{
            //    SetGrabDone();

            //    for (int i = 0; i < m_intGrabRequire; i++)
            //    {
            //        if (m_objAVTFireGrab.GetFrame(i))
            //        {
            //            if (m_objAVTFireGrab.ConvertFrame(i))
            //            {
            //                if (m_smVisionInfo.g_blnViewColorImage)
            //                {
            //                    m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objAVTFireGrab.ref_ptrImagePointer);
            //                    CImageDrawing objImage = m_smVisionInfo.g_arrColorImages[i];
            //                    m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref objImage);
            //                }
            //                else
            //                {
            //                    m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objAVTFireGrab.ref_ptrImagePointer);
            //                    ImageDrawing objImage = m_smVisionInfo.g_arrImages[i];
            //                    m_smVisionInfo.g_objMemoryImage.CopyTo(ref objImage);
            //                }

            //                m_objAVTFireGrab.ReleaseImage(i);
            //            }
            //        }
            //    }
            //}
            //else
            //    SetGrabDone();

            //if (m_objAVTFireGrab.ref_strErrorText != "")
            //{
            //    m_smVisionInfo.g_strErrorMessage = m_objAVTFireGrab.ref_strErrorText;
            //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //    return false;
            //}
            //else
            //{
            //    AttachImageToROI();
            //    m_smVisionInfo.g_blnLoadFile = false;
            //    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            //    return true;
            //}
        }

        // JB
        ////Found bug intGrabIndex should not put as the index for m_smVisionInfo.g_arrLightSource
        //private void UpdateAVTCameraOutport(int intGrabIndex)
        //{
        //    //TrackLog objTL = new TrackLog();
        //    // Outport 1
        //    if ((m_smVisionInfo.g_arrLightSource[intGrabIndex].ref_intSeqNo & 0x01) == 0)
        //    {
        //        if ((m_intCameraOutState & (0x01)) > 0)
        //        {
        //            //objTL.WriteLine("GrabIndex=" + intGrabIndex.ToString() + ", Output 1 set 00");
        //            if (m_smVisionInfo.g_intTriggerMode == 1)
        //                m_objAVTFireGrab.OutPort(0, 3); // Busy Outpot 1
        //            else
        //                m_objAVTFireGrab.OutPort(0, 0); // Off Outpot 1
        //            m_intCameraOutState &= ~0x01;
        //        }
        //    }
        //    else
        //    {
        //        if ((m_intCameraOutState & (0x01)) == 0)
        //        {
        //            //objTL.WriteLine("GrabIndex=" + intGrabIndex.ToString() + ", Output 1 set 0" + m_smVisionInfo.g_intTriggerMode.ToString());
        //            m_objAVTFireGrab.OutPort(0, m_smVisionInfo.g_intTriggerMode); // On Outport 1
        //            m_intCameraOutState |= 0x01;
        //        }
        //    }

        //    // Outport 2
        //    if ((m_smVisionInfo.g_arrLightSource[intGrabIndex].ref_intSeqNo & 0x02) == 0)
        //    {
        //        if ((m_intCameraOutState & (0x02)) > 0)
        //        {
        //            //objTL.WriteLine("GrabIndex=" + intGrabIndex.ToString() + ", Output 2 set 10");
        //            if (m_smVisionInfo.g_intTriggerMode == 1)
        //                m_objAVTFireGrab.OutPort(1, 3); // Busy Outpot 2
        //            else
        //                m_objAVTFireGrab.OutPort(1, 0); // Off Outpot 2
        //            m_intCameraOutState &= ~0x02;       
        //        }
        //    }
        //    else
        //    {
        //        if ((m_intCameraOutState & (0x02)) == 0)
        //        {
        //            //objTL.WriteLine("GrabIndex=" + intGrabIndex.ToString() + ", Output 2 set 1" + m_smVisionInfo.g_intTriggerMode.ToString());
        //            m_objAVTFireGrab.OutPort(1, m_smVisionInfo.g_intTriggerMode); // On Outport 2
        //            m_intCameraOutState |= 0x02;
        //        }
        //    }

        //    // Outport 3
        //    if ((m_smVisionInfo.g_arrLightSource[intGrabIndex].ref_intSeqNo & 0x04) == 0)
        //    {
        //        if ((m_intCameraOutState & (0x04)) > 0)
        //        {
        //            //objTL.WriteLine("GrabIndex=" + intGrabIndex.ToString() + ", Output 3 set 20");
        //            if (m_smVisionInfo.g_intTriggerMode == 1)
        //                m_objAVTFireGrab.OutPort(2, 3); // Busy Outpot 2
        //            else
        //                m_objAVTFireGrab.OutPort(2, 0); // Off Outpot 3
        //            m_intCameraOutState &= ~0x04;
        //        }
        //    }
        //    else
        //    {
        //        if ((m_intCameraOutState & (0x04)) == 0)
        //        {
        //            //objTL.WriteLine("GrabIndex=" + intGrabIndex.ToString() + ", Output 3 set 2" + m_smVisionInfo.g_intTriggerMode.ToString());
        //            m_objAVTFireGrab.OutPort(2, m_smVisionInfo.g_intTriggerMode); // On Outport 3
        //            m_intCameraOutState |= 0x04;
        //        }
        //    }
        //}

        private void UpdateAVTCameraOutport(int intGrabIndex)
        {
            //TrackLog objTL = new TrackLog();
            int intGrabSequence;
            switch (intGrabIndex)
            {
                default:
                case 0:
                    intGrabSequence = 0x01;
                    break;
                case 1:
                    intGrabSequence = 0x02;
                    break;
                case 2:
                    intGrabSequence = 0x04;
                    break;
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {
                if ((m_smVisionInfo.g_arrLightSource[i].ref_intSeqNo & intGrabSequence) == 0)
                {
                    if ((m_intCameraOutState & (intGrabSequence)) > 0)
                    {
                        //objTL.WriteLine("GrabIndex=" + i.ToString() + ", Output 1 set 00");
                        if (m_smVisionInfo.g_intTriggerMode == 1)
                            m_objAVTFireGrab.OutPort(i, 3); // Busy Outpot 1
                        else
                            m_objAVTFireGrab.OutPort(i, 0); // Off Outpot 1
                        m_intCameraOutState &= ~intGrabSequence;
                    }
                }
                else
                {
                    if ((m_intCameraOutState & (intGrabSequence)) == 0)
                    {
                        //objTL.WriteLine("GrabIndex=" + i.ToString() + ", Output 1 set 0" + m_smVisionInfo.g_intTriggerMode.ToString());
                        m_objAVTFireGrab.OutPort(i, m_smVisionInfo.g_intTriggerMode); // On Outport 1
                        m_intCameraOutState |= intGrabSequence;
                    }
                }
            }
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
        public bool GrabImage(int intGrabImageMask, bool blnForInspection)
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

            if (!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.CheckPH != null)
            {
                if (((m_blnAuto && m_objVisionIO.CheckPH.IsOn(m_smVisionInfo.g_blnCheckPH)) || (!m_blnAuto && m_smVisionInfo.g_blnViewPHImage)) && m_smVisionInfo.g_blnWantCheckPH) //|| m_smVisionInfo.g_blnSelectedPH
                {
                    if (m_blnPreviouslyIsLead)// || m_smVisionInfo.g_blnViewPHImage)
                    {
                        LoadPHLighting();
                    }
                    m_blnPreviouslyIsLead = false;
                    return GrabImage_Sequence_SetIntensityForPH_Teli(intGrabImageMask, blnForInspection);

                }
                else
                {
                    if (!m_blnPreviouslyIsLead)//|| !m_smVisionInfo.g_blnViewPHImage)
                    {
                        LoadLeadLighting();
                    }
                    m_blnPreviouslyIsLead = true;
                    if (m_smVisionInfo.g_intLightControllerType == 2)
                    {
                        if (m_smVisionInfo.g_strCameraModel == "AVT")
                            return GrabImage_Sequence_NoSetIntensity_AVT(intGrabImageMask, blnForInspection);
                        else if (m_smVisionInfo.g_strCameraModel == "Teli")
                            return GrabImage_Sequence_NoSetIntensity_Teli(intGrabImageMask, blnForInspection);
                    }

                    if (m_smVisionInfo.g_strCameraModel == "IDS")
                    {
                        return GrabMultiImage_IDS_IOCard_NoSetIntensity(blnForInspection);  // XDW16-001
                    }
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                    {
                        return GrabImage_Teli(intGrabImageMask, blnForInspection);
                    }
                    else
                    {
                        return GrabImage_Normal_NoSetIntensity_AVTVimba(intGrabImageMask, blnForInspection);
                    }
                }
            }
            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
            {
                if (((m_blnAuto && m_blnCheckPH_In) || (!m_blnAuto && m_smVisionInfo.g_blnViewPHImage)) && m_smVisionInfo.g_blnWantCheckPH) //|| m_smVisionInfo.g_blnSelectedPH
                {
                    if (m_blnPreviouslyIsLead)// || m_smVisionInfo.g_blnViewPHImage)
                    {
                        LoadPHLighting();
                    }
                    m_blnPreviouslyIsLead = false;
                    return GrabImage_Sequence_SetIntensityForPH_Teli(intGrabImageMask, blnForInspection);

                }
                else
                {
                    if (!m_blnPreviouslyIsLead)//|| !m_smVisionInfo.g_blnViewPHImage)
                    {
                        LoadLeadLighting();
                    }
                    m_blnPreviouslyIsLead = true;
                    if (m_smVisionInfo.g_intLightControllerType == 2)
                    {
                        if (m_smVisionInfo.g_strCameraModel == "AVT")
                            return GrabImage_Sequence_NoSetIntensity_AVT(intGrabImageMask, blnForInspection);
                        else if (m_smVisionInfo.g_strCameraModel == "Teli")
                            return GrabImage_Sequence_NoSetIntensity_Teli(intGrabImageMask, blnForInspection);
                    }

                    if (m_smVisionInfo.g_strCameraModel == "IDS")
                    {
                        return GrabMultiImage_IDS_IOCard_NoSetIntensity(blnForInspection);  // XDW16-001
                    }
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                    {
                        return GrabImage_Teli(intGrabImageMask, blnForInspection);
                    }
                    else
                    {
                        return GrabImage_Normal_NoSetIntensity_AVTVimba(intGrabImageMask, blnForInspection);
                    }
                }
            }
            else
            {
                if (!m_blnPreviouslyIsLead)//|| !m_smVisionInfo.g_blnViewPHImage)
                {
                    LoadLeadLighting();
                }

                m_blnPreviouslyIsLead = true;
                if (m_smVisionInfo.g_intLightControllerType == 2)
                {
                    if (m_smVisionInfo.g_strCameraModel == "AVT")
                        return GrabImage_Sequence_NoSetIntensity_AVT(intGrabImageMask, blnForInspection);
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                        return GrabImage_Sequence_NoSetIntensity_Teli(intGrabImageMask, blnForInspection);
                }

                if (m_smVisionInfo.g_strCameraModel == "IDS")
                {
                    return GrabMultiImage_IDS_IOCard_NoSetIntensity(blnForInspection);  // XDW16-001
                }
                else if (m_smVisionInfo.g_strCameraModel == "Teli")
                {
                    return GrabImage_Teli(intGrabImageMask, blnForInspection);
                }
                else
                {
                    return GrabImage_Normal_NoSetIntensity_AVTVimba(intGrabImageMask, blnForInspection);
                }
            }
        }

        public bool GrabImage_Teli(int intGrabImageMask, bool blnForInspection)
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
            //HiPerfTimer timer_GrabTime = new HiPerfTimer();

            TrackLog objTL = new TrackLog();
            float fTotalGrabTime = 0f;
            HiPerfTimer timer_TotalTime = new HiPerfTimer();
            HiPerfTimer timer_TotalGrabTime = new HiPerfTimer();
            timer_TotalTime.Start();

            int intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[0] * 0.001f);  // For Teli, Shuttle 1 == 1 microsecond
            m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = false;
            m_objTeliCamera.DiscardFrame();

            for (int i = 0; i < m_intGrabRequire; i++)
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                if (intGrabImageMask > 0)   // Grab all image if intGrabImageMask is 0.
                {
                    if ((intGrabImageMask & (0x01 << i)) == 0)
                    {
                        continue;
                    }
                }

                if (i > 0) // for second image and third image
                {
                    //timer_GrabTime.Stop();
                    //if ((m_objAVTFireGrab.ref_intNextGrabDelay - timer_GrabTime.Duration) > 0)
                    //    Thread.Sleep((int)(m_objAVTFireGrab.ref_intNextGrabDelay - timer_GrabTime.Duration));
                    //timer_TotalGrabTime.Start();

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

                //timer_TotalGrabTime.Stop();
                //fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
                if (i < m_intGrabRequire - 1)
                {
                    Thread.Sleep(intExposureTime);
                }
                else
                {
                    Thread.Sleep(Math.Max(intExposureTime, 10));
                }
            }

            //timer_TotalGrabTime.Start();

            if (!m_objTeliCamera.WaitFrameReady())
            {
                blnSuccess = false;
                m_blnForceStopProduction = true;
            }

            if (blnSuccess)
            {
                SetGrabDone(blnForInspection);
                m_smVisionInfo.g_objTransferTime.Start();

                //for (int i = 0; i < m_intGrabRequire; i++)
                //{
                //if (blnSeparateGrab)
                //{
                //    if (i != intSelectedImage)
                //        continue;
                //}

                //if (intGrabImageMask > 0)   // Grab all image if intGrabImageMask is 0.
                //{
                //    if ((intGrabImageMask & (0x01 << i)) == 0)
                //    {
                //        continue;
                //    }
                //}
                //timer_TotalGrabTime.Start();

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
                //}

                timer_TotalGrabTime.Stop();
                fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
            }
            else
                SetGrabDone(blnForInspection);

            //for (int i = 0; i < m_intGrabRequire; i++)
            //{
            //    if (blnSeparateGrab)
            //    {
            //        if (i != intSelectedImage)
            //            continue;
            //    }

            //    if (intGrabImageMask > 0)   // Grab all image if intGrabImageMask is 0.
            //    {
            //        if ((intGrabImageMask & (0x01 << i)) == 0)
            //        {
            //            continue;
            //        }
            //    }
            //    timer_TotalGrabTime.Start();
            //    m_objTeliCamera.ReleaseImage(i);
            //    timer_TotalGrabTime.Stop();
            //    fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
            //}
            m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = true; // 2020 07 17 - CCENG: Set Result before Set Done.
            m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;
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

        public bool GrabImage_Sequence_NoSetIntensity_AVT(int intGrabImageMask, bool blnForInspection)
        {
            if (!m_objAVTFireGrab.ref_blnCameraInitDone)
            {
                m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = true;
                m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;
                m_smVisionInfo.g_strErrorMessage = "Camera No Connected";
                return true;
            }

            // Default Using AVT Camera
            m_smVisionInfo.g_objGrabTime.Start();

            Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay);

            bool blnSuccess = true;
            bool blnSeparateGrab = m_smVisionInfo.g_blnSeparateGrab;
            int intSelectedImage = m_smVisionInfo.g_intSelectedImage;
            HiPerfTimer timer_GrabTime = new HiPerfTimer();

            TrackLog objTL = new TrackLog();
            float fTotalGrabTime = 0f;
            HiPerfTimer timer_TotalTime = new HiPerfTimer();
            HiPerfTimer timer_TotalGrabTime = new HiPerfTimer();
            timer_TotalTime.Start();

            int intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_fCameraShuttle * 0.02f);
            m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = false;
            timer_TotalGrabTime.Start();
            m_objAVTFireGrab.DiscardFrame();
            timer_TotalGrabTime.Stop();
            fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;

            for (int i = 0; i < m_intGrabRequire; i++)
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                if (intGrabImageMask > 0)   // Grab all image if intGrabImageMask is 0.
                {
                    if ((intGrabImageMask & (0x01 << i)) == 0)
                    {
                        continue;
                    }
                }

                if (i > 0) // for second image and third image
                {
                    timer_GrabTime.Stop();
                    if ((m_objAVTFireGrab.ref_intNextGrabDelay - timer_GrabTime.Duration) > 0)
                        Thread.Sleep((int)(m_objAVTFireGrab.ref_intNextGrabDelay - timer_GrabTime.Duration));
                }

                #region if more than 1 image need to be captured
                // Set light source channel ON/OFF
                if (m_intGrabRequire > 1)
                {
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

                        intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[i] * 0.02f);
                    }
                    //UpdateAVTCameraOutport(i);
                }
                #endregion
                if (i == 0)
                {
                    m_objAVTFireGrab.OutPort(1, 4);
                }
                //if (i == 1)
                //{
                //    m_objAVTFireGrab.OutPort(0, m_smVisionInfo.g_intTriggerMode);
                //}

                timer_TotalGrabTime.Start();
                if (!m_objAVTFireGrab.Grab())
                {
                    blnSuccess = false;
                    m_blnForceStopProduction = true;
                }
                timer_TotalGrabTime.Stop();
                fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
                if (i < m_intGrabRequire - 1)
                {
                    timer_GrabTime.Start();

                    Thread.Sleep(intExposureTime);
                }
                else
                {
                    Thread.Sleep(Math.Max(intExposureTime, 10));
                }

            }

            if (blnSuccess)
            {
                SetGrabDone(blnForInspection);
                m_smVisionInfo.g_objTransferTime.Start();

                for (int i = 0; i < m_intGrabRequire; i++)
                {
                    if (blnSeparateGrab)
                    {
                        if (i != intSelectedImage)
                            continue;
                    }

                    if (intGrabImageMask > 0)   // Grab all image if intGrabImageMask is 0.
                    {
                        if ((intGrabImageMask & (0x01 << i)) == 0)
                        {
                            continue;
                        }
                    }
                    timer_TotalGrabTime.Start();
                    if (m_objAVTFireGrab.GetFrame(i))
                    {
                        if (m_objAVTFireGrab.ConvertFrame(i))
                        {
                            if (m_smVisionInfo.g_blnViewColorImage)
                            {
                                m_smVisionInfo.g_objMemoryColorImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                                m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objAVTFireGrab.ref_ptrImagePointer);
                                m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, i);
                            }
                            else
                            {
                                m_smVisionInfo.g_objMemoryImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                                m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objAVTFireGrab.ref_ptrImagePointer);
                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, i);
                                m_smVisionInfo.g_arrImages[i].AddGain(m_smVisionInfo.g_arrImageGain[i]);
                            }
                        }
                    }
                    timer_TotalGrabTime.Stop();
                    fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
                }
            }
            else
                SetGrabDone(blnForInspection);

            for (int i = 0; i < m_intGrabRequire; i++)
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                if (intGrabImageMask > 0)   // Grab all image if intGrabImageMask is 0.
                {
                    if ((intGrabImageMask & (0x01 << i)) == 0)
                    {
                        continue;
                    }
                }
                timer_TotalGrabTime.Start();
                m_objAVTFireGrab.ReleaseImage(i);
                timer_TotalGrabTime.Stop();
                fTotalGrabTime = fTotalGrabTime + timer_TotalGrabTime.Duration;
            }

            //Reset outport
            m_objAVTFireGrab.OutPort(1, 5);
            //m_objAVTFireGrab.OutPort(0, 0);

            if (m_objAVTFireGrab.ref_strErrorText != "")
            {
                m_smVisionInfo.g_strErrorMessage = m_objAVTFireGrab.ref_strErrorText;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();
                return false;
            }
            else
            {
                // Set camera gain
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[0])
                {
                    m_objAVTFireGrab.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[0]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[0];
                }

                // Set camera shuttle
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[0])
                {
                    m_objAVTFireGrab.SetCameraParameter(1, (uint)m_smVisionInfo.g_arrCameraShuttle[0]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[0];
                }

                //AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();

                timer_TotalTime.Stop();
                //objTL.WriteLine("Total grab time = " + ((timer_TotalTime.Duration - fTotalGrabTime).ToString()));
                //objTL.WriteLine("Total grab time = " + timer_TotalTime.Duration.ToString());
                return true;
            }
        }

        //Teli Camera Test
        public bool GrabImage_Sequence_NoSetIntensity_Teli(int intGrabImageMask, bool blnForInspection)
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
            m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = false;
            for (int i = 0; i < m_intGrabRequire; i++)
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                if (i > 0) // for when grabbing second image and third image and forth image
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
                                SetExtraGain(i - 1, m_smVisionInfo.g_arrDebugImages);
                                ImageMerge(i - 1, m_smVisionInfo.g_arrDebugImages);
                                ImageUniformize(i - 1, m_smVisionInfo.g_arrDebugImages);
                            }
                            else
                            {
                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, i - 1);
                                m_smVisionInfo.g_arrImages[i - 1].AddGain(m_smVisionInfo.g_arrImageGain[i - 1]);
                                SetExtraGain(i - 1);
                                ImageMerge(i - 1);
                                ImageUniformize(i - 1);
                            }
                            if (i == 1)
                            {
                                if (blnSuccess) m_bGrabImage1Result = true;
                                if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[i - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + ".bmp");
                                m_bGrabImage1Done = true;
                            }
                            else if (i == 2)
                            {
                                if (blnSuccess) m_bGrabImage2Result = true;
                                if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[i - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (i - 1) + ".bmp");
                                m_bGrabImage2Done = true;
                            }
                            else if (i == 3)
                            {
                                if (blnSuccess) m_bGrabImage3Result = true;
                                if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[i - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (i - 1) + ".bmp");
                                m_bGrabImage3Done = true;
                            }
                            else if (i == 4)
                            {
                                if (blnSuccess) m_bGrabImage4Result = true;
                                if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[i - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (i - 1) + ".bmp");
                                m_bGrabImage4Done = true;
                            }
                            else if (i == 5)
                            {
                                if (blnSuccess) m_bGrabImage5Result = true;
                                if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[i - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (i - 1) + ".bmp");
                                m_bGrabImage5Done = true;
                            }
                            else if (i == 6)
                            {
                                if (blnSuccess) m_bGrabImage6Result = true;
                                if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[i - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (i - 1) + ".bmp");
                                m_bGrabImage6Done = true;
                            }
                            else if (i == 7)
                            {
                                if (blnSuccess) m_bGrabImage7Result = true;
                                if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[i - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (i - 1) + ".bmp");
                                m_bGrabImage7Done = true;
                            }
                        }
                    }
                    else
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }

                    //if (i == 1)
                    //{
                    //    m_bGrabImage1Done = true;
                    //}
                    //else if (i == 2)
                    //{
                    //    m_bGrabImage2Done = true;
                    //}
                    //else if (i == 3)
                    //{
                    //    m_bGrabImage3Done = true;
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
                if (i == 0)
                {
                    //10-07-2019 ZJYEOH : Check the shuttle and gain again because PH got other setting
                    m_uintPHCameraGainPrev = 1;
                    m_fPHCameraShuttlePrev = 1f;
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

                    if (!m_smCustomizeInfo.g_blnMixController)
                    {
                        if (m_smCustomizeInfo.g_blnVTControl)
                        {
                            if (i == 0)
                            {
                                m_objTeliCamera.OutPort(1, 3);
                                m_objTeliCamera.OutPort(0, m_smVisionInfo.g_intTriggerMode);
                            }
                            else if (i == (m_intGrabRequire - 1))
                            {
                                m_objTeliCamera.OutPort(0, 0);
                            }
                        }
                    }
                    else
                    {
                        if (m_intLightControlModel == 1)    // 2018 07 13 - CCENG: Temporary force to use VTControl
                        {
                            if (i == 0)
                            {
                                m_objTeliCamera.OutPort(1, 3);
                                m_objTeliCamera.OutPort(0, m_smVisionInfo.g_intTriggerMode);
                            }
                            else if (i == (m_intGrabRequire - 1))
                            {
                                m_objTeliCamera.OutPort(0, 0);
                            }
                        }
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
            }

            if (!m_objTeliCamera.WaitFrameReady())
            {
                blnSuccess = false;
                m_blnForceStopProduction = true;
            }

            if (blnSuccess)
            {
                SetGrabDone(blnForInspection);
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
                                SetExtraGain(m_intGrabRequire - 1, m_smVisionInfo.g_arrDebugImages);
                                ImageMerge(m_intGrabRequire - 1, m_smVisionInfo.g_arrDebugImages);
                                ImageUniformize(m_intGrabRequire - 1, m_smVisionInfo.g_arrDebugImages);
                            }
                            else
                            {
                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, m_intGrabRequire - 1);
                                m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].AddGain(m_smVisionInfo.g_arrImageGain[m_intGrabRequire - 1]);
                                SetExtraGain(m_intGrabRequire - 1);
                                ImageMerge(m_intGrabRequire - 1);
                                ImageUniformize(m_intGrabRequire - 1);
                            }
                        }
                        if (m_intGrabRequire == 1)
                        {
                            if (blnSuccess) m_bGrabImage1Result = true;
                            if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (m_intGrabRequire - 1) + ".bmp");
                            m_bGrabImage1Done = true;
                        }
                        else if (m_intGrabRequire == 2)
                        {
                            if (blnSuccess) m_bGrabImage2Result = true;
                            if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (m_intGrabRequire - 1) + ".bmp");
                            m_bGrabImage2Done = true;
                        }
                        else if (m_intGrabRequire == 3)
                        {
                            if (blnSuccess) m_bGrabImage3Result = true;
                            if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (m_intGrabRequire - 1) + ".bmp");
                            m_bGrabImage3Done = true;
                        }
                        else if (m_intGrabRequire == 4)
                        {
                            if (blnSuccess) m_bGrabImage4Result = true;
                            if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (m_intGrabRequire - 1) + ".bmp");
                            m_bGrabImage4Done = true;
                        }
                        else if (m_intGrabRequire == 5)
                        {
                            if (blnSuccess) m_bGrabImage5Result = true;
                            if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (m_intGrabRequire - 1) + ".bmp");
                            m_bGrabImage5Done = true;
                        }
                        else if (m_intGrabRequire == 6)
                        {
                            if (blnSuccess) m_bGrabImage6Result = true;
                            if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (m_intGrabRequire - 1) + ".bmp");
                            m_bGrabImage6Done = true;
                        }
                        else if (m_intGrabRequire == 7)
                        {
                            if (blnSuccess) m_bGrabImage7Result = true;
                            if (m_smVisionInfo.g_blnSaveImageAfterGrab) m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].SaveImage("D:\\TSLi3D\\" + m_intCounter + "_Image" + (m_intGrabRequire - 1) + ".bmp");
                            m_bGrabImage7Done = true;
                        }
                    }
                }
            }
            else
                SetGrabDone(blnForInspection);

            m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;

            //Reset outport
            if (!m_smCustomizeInfo.g_blnMixController)
            {
                if (m_smCustomizeInfo.g_blnLEDiControl)
                {
                    m_objTeliCamera.OutPort(1, 3);
                    m_objTeliCamera.OutPort(1, 0);
                }
                else if (m_smCustomizeInfo.g_blnVTControl)
                {
                    m_objTeliCamera.OutPort(1, 0);
                }
            }
            else
            {
                if(m_intLightControlModel == 0) // 2018 07 13 - CCENG: Temporary force to use VTControl
                {
                    m_objTeliCamera.OutPort(1, 3);
                    m_objTeliCamera.OutPort(1, 0);
                }
                else if (m_intLightControlModel == 1)   // 2018 07 13 - CCENG: Temporary force to use VTControl
                {
                    m_objTeliCamera.OutPort(1, 0);
                }

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

                    HiPerfTimer timesout = new HiPerfTimer();
                    timesout.Start();

                    while (true)
                    {
                        if (!m_smVisionInfo.g_bImageStatisticAnalysisUpdateInfo)
                            break;

                        if (timesout.Timing > 3000)
                        {
                            STTrackLog.WriteLine(">>>>>>>>>>>>> time out 1");
                            break;
                        }

                        Thread.Sleep(1);
                    }
                }

                //AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();

                return true;
            }
        }

        public bool GrabImage_Normal_NoSetIntensity_AVTVimba(int intGrabImageMask, bool blnForInspection)
        {
            if (!m_objAVTFireGrab.ref_blnCameraInitDone)
            {
                m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = true;
                m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;
                m_smVisionInfo.g_strErrorMessage = "Camera No Connected";
                return true;
            }

            // Default Using AVT Camera
            m_smVisionInfo.g_objGrabTime.Start();

            Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay);

            bool blnSuccess = true;
            bool blnSeparateGrab = m_smVisionInfo.g_blnSeparateGrab;
            int intSelectedImage = m_smVisionInfo.g_intSelectedImage;
            HiPerfTimer timer_GrabTime = new HiPerfTimer();

            int intExposureTime = (int)Math.Ceiling(m_smVisionInfo.g_fCameraShuttle * 0.02f);
            m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = false;
            m_objAVTFireGrab.DiscardFrame();
            for (int i = 0; i < m_intGrabRequire; i++)
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                if (intGrabImageMask > 0)   // Grab all image if intGrabImageMask is 0.
                {
                    if ((intGrabImageMask & (0x01 << i)) == 0)
                    {
                        continue;
                    }
                }

                if (i > 0) // for second image and third image
                {
                    timer_GrabTime.Stop();
                    if ((m_objAVTFireGrab.ref_intNextGrabDelay - timer_GrabTime.Duration) > 0)
                        Thread.Sleep((int)(m_objAVTFireGrab.ref_intNextGrabDelay - timer_GrabTime.Duration));
                }

                #region if more than 1 image need to be captured
                // Set light source channel ON/OFF
                if (m_intGrabRequire > 1)
                {
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

                    UpdateAVTCameraOutport(i);
                }
                #endregion

                if (!m_objAVTFireGrab.Grab())
                {
                    blnSuccess = false;
                    m_blnForceStopProduction = true;
                }

                if (i < m_intGrabRequire - 1)
                {
                    timer_GrabTime.Start();

                    Thread.Sleep(intExposureTime);
                }
                else
                {
                    Thread.Sleep(Math.Max(intExposureTime, 10));
                }

            }

            if (blnSuccess)
            {
                SetGrabDone(blnForInspection);
                m_smVisionInfo.g_objTransferTime.Start();

                for (int i = 0; i < m_intGrabRequire; i++)
                {
                    if (blnSeparateGrab)
                    {
                        if (i != intSelectedImage)
                            continue;
                    }

                    if (intGrabImageMask > 0)   // Grab all image if intGrabImageMask is 0.
                    {
                        if ((intGrabImageMask & (0x01 << i)) == 0)
                        {
                            continue;
                        }
                    }

                    if (m_objAVTFireGrab.GetFrame(i))
                    {
                        if (m_objAVTFireGrab.ConvertFrame(i))
                        {
                            if (m_smVisionInfo.g_blnViewColorImage)
                            {
                                m_smVisionInfo.g_objMemoryColorImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                                m_smVisionInfo.g_objMemoryColorImage.LoadImageFromMemory(m_objAVTFireGrab.ref_ptrImagePointer);
                                m_smVisionInfo.g_objMemoryColorImage.CopyTo(ref m_smVisionInfo.g_arrColorImages, i);
                            }
                            else
                            {
                                m_smVisionInfo.g_objMemoryImage.SetImageSize(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight);
                                m_smVisionInfo.g_objMemoryImage.LoadImageFromMemory(m_objAVTFireGrab.ref_ptrImagePointer);
                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, i);
                                m_smVisionInfo.g_arrImages[i].AddGain(m_smVisionInfo.g_arrImageGain[i]);

                            }

                            if (i == 0)
                            {
                                if (blnSuccess) m_bGrabImage1Result = true;
                                m_bGrabImage1Done = true;
                            }
                            else if (i == 1)
                            {
                                if (blnSuccess) m_bGrabImage2Result = true;
                                m_bGrabImage2Done = true;
                            }
                            else if (i == 2)
                            {
                                if (blnSuccess) m_bGrabImage3Result = true;
                                m_bGrabImage3Done = true;
                            }
                            else if (i == 3)
                            {
                                if (blnSuccess) m_bGrabImage4Result = true;
                                m_bGrabImage4Done = true;
                            }
                            else if (i == 4)
                            {
                                if (blnSuccess) m_bGrabImage5Result = true;
                                m_bGrabImage5Done = true;
                            }
                            else if (i == 5)
                            {
                                if (blnSuccess) m_bGrabImage6Result = true;
                                m_bGrabImage6Done = true;
                            }
                            else if (i == 6)
                            {
                                if (blnSuccess) m_bGrabImage7Result = true;
                                m_bGrabImage7Done = true;
                            }
                        }
                    }
                }
            }
            else
                SetGrabDone(blnForInspection);
            m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;
            for (int i = 0; i < m_intGrabRequire; i++)
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                if (intGrabImageMask > 0)   // Grab all image if intGrabImageMask is 0.
                {
                    if ((intGrabImageMask & (0x01 << i)) == 0)
                    {
                        continue;
                    }
                }

                m_objAVTFireGrab.ReleaseImage(i);
            }

            if (m_objAVTFireGrab.ref_strErrorText != "")
            {
                m_smVisionInfo.g_strErrorMessage = m_objAVTFireGrab.ref_strErrorText;
                m_smVisionInfo.g_objTransferTime.Stop();
                return false;
            }
            else
            {
                // Set camera gain
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[0])
                {
                    m_objAVTFireGrab.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[0]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[0];
                }

                // Set camera shuttle
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[0])
                {
                    m_objAVTFireGrab.SetCameraParameter(1, (uint)m_smVisionInfo.g_arrCameraShuttle[0]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[0];
                }

                //AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                m_smVisionInfo.g_objTransferTime.Stop();
                return true;
            }

        }

        public bool GrabSingleImage_IDS()
        {
            m_smVisionInfo.g_objGrabTime.Start();

            m_objVisionIO.IOGrabbing.SetOn("V3Lead ");

            // On Lighting
            if (m_intGrabIndex == 0 && m_objVisionIO.Grab1 != null)
                m_objVisionIO.Grab1.SetOn("V3Lead ");
            if (m_intGrabIndex == 1 && m_objVisionIO.Grab2 != null)
                m_objVisionIO.Grab2.SetOn("V3Lead ");

            if (m_intGrabRequire > 1)
            {
                // Set camera gain
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[m_intGrabIndex])
                {
                    m_objIDSCamera.SetGain((int)m_smVisionInfo.g_arrCameraGain[m_intGrabIndex]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[m_intGrabIndex];
                }
            }

            if (m_objIDSCamera.Grab())
            {
                //m_objIDSCamera.WaitExposureDone();
                Thread.Sleep(2);

                SetGrabDone(false);

                if (m_intGrabIndex == 0 && m_objVisionIO.Grab1 != null)
                    m_objVisionIO.Grab1.SetOff("V3Lead ");
                if (m_intGrabIndex == 1 && m_objVisionIO.Grab2 != null)
                    m_objVisionIO.Grab2.SetOff("V3Lead ");

                m_smVisionInfo.g_objTransferTime.Start();

                if (m_objIDSCamera.GetFrame(m_smVisionInfo.g_objMemoryImage))
                {
                    m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, m_intGrabIndex);
                    m_smVisionInfo.g_arrImages[m_intGrabIndex].AddGain(m_smVisionInfo.g_arrImageGain[m_intGrabIndex]);

                    m_smVisionInfo.g_objTransferTime.Stop();
                }
            }
            else
                SetGrabDone(false);

            if (m_intGrabRequire > 1)
            {
                int intNextGrabCount = m_intGrabIndex + 1;
                if (intNextGrabCount >= m_intGrabRequire)
                    intNextGrabCount = 0;
                // Set camera gain
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[intNextGrabCount])
                {
                    m_objIDSCamera.SetGain((int)m_smVisionInfo.g_arrCameraGain[intNextGrabCount]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[intNextGrabCount];
                }
            }

            if (m_intGrabIndex == 0 && m_objVisionIO.Grab1 != null)
                m_objVisionIO.Grab1.SetOff("V3Lead ");
            if (m_intGrabIndex == 1 && m_objVisionIO.Grab2 != null)
                m_objVisionIO.Grab2.SetOff("V3Lead ");

            if (m_objIDSCamera.ref_strErrorText != "")
            {
                m_smVisionInfo.g_strErrorMessage = m_objIDSCamera.ref_strErrorText;
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                m_smVisionInfo.g_objTransferTime.Stop();
                return false;
            }
            else
            {
                //AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                m_smVisionInfo.g_objTransferTime.Stop();
                return true;
            }

        }

        public bool GrabMultiImage_IDS_IOCard_NoSetIntensity(bool blnForInspection)
        {
            /*
             *  -----------------------2018 07 14  - CCENG -----------------------------------------------------
             *  IDS camera is not going to use in this Open Vision 64Bit Project. Will replace with Teli or vimba or other camera instead.
             *  This function is not supporting StartTest_MultiThreading function
             *  Futher enhancement of IDS coding need to do if want to support this project. 
             *  ------------------------------------------------------------------------------------------------
             */

            // No transfer time here
            m_smVisionInfo.g_objGrabTime.Start();

            Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay);

            m_objVisionIO.IOGrabbing.SetOn("V3Lead ");

            bool blnSeparateGrab = m_smVisionInfo.g_blnSeparateGrab;
            int intSelectedImage = m_smVisionInfo.g_intSelectedImage;

            bool blnRetry = false;
            for (int i = 0; i < m_intGrabRequire; i++)     // Maximum 2 grabs for pad vision
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                // Make sure light is OFF
                if ((m_objVisionIO.Grab1 != null && m_objVisionIO.Grab1.IsOn()) ||
                    (m_objVisionIO.Grab2 != null && m_objVisionIO.Grab2.IsOn()))
                {
                    m_objVisionIO.Grab1.SetOff("V3Lead ");
                    m_objVisionIO.Grab2.SetOff("V3Lead ");

                    Thread.Sleep(1);    // Sleep 1ms before ON back.
                }

                //if (i > 0)
                //    Thread.Sleep(5);

                // On Lighting
                if (i == 0 && m_objVisionIO.Grab1 != null)
                //if (i == 0 && m_objVisionIO.Grab1 != null && m_objVisionIO.Grab2 != null)
                {
                    m_objVisionIO.Grab1.SetOn("V3Lead ");
                    //    m_objVisionIO.Grab2.SetOn("V3Lead ");
                }

                if (i == 1 && m_objVisionIO.Grab2 != null)
                    m_objVisionIO.Grab2.SetOn("V3Lead ");

                // Set camera gain
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
                {
                    m_objIDSCamera.SetGain((int)m_smVisionInfo.g_arrCameraGain[i]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                }


                // Set camera shuttle
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[i])
                {
                    m_objIDSCamera.SetShuttle(m_smVisionInfo.g_arrCameraShuttle[i]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
                }

                // Start Grab image
                if (m_objIDSCamera.Grab())
                {
                    //m_objIDSCamera.WaitExposureDone();
                    Thread.Sleep((int)Math.Max(2, Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[i])));

                    // Off Lighting (Light Controller will automatically off the light if IO ON time longer than intensity setting)
                    if (i == 0 && m_objVisionIO.Grab1 != null)
                        m_objVisionIO.Grab1.SetOff("V3Lead ");
                    if (i == 1 && m_objVisionIO.Grab2 != null)
                        m_objVisionIO.Grab2.SetOff("V3Lead ");

                    // Get image frame
                    if (m_objIDSCamera.GetFrame(m_smVisionInfo.g_objMemoryImage))
                    {
                        m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, i);
                        m_smVisionInfo.g_arrImages[i].AddGain(m_smVisionInfo.g_arrImageGain[i]);
                    }
                    else
                    {
                        if (!blnRetry)
                        {
                            blnRetry = true;
                            i--;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                }
                else
                {
                    break;
                }
            }

            SetGrabDone(blnForInspection);

            if (m_intGrabRequire > 1)
            {
                // Set first image gain to camera before next cycle camera grab is coming.
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[0])
                {
                    m_objIDSCamera.SetGain((int)m_smVisionInfo.g_arrCameraGain[0]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[0];
                }

                // Set first image shuttle to camera before next cycle camera grab is coming.
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[0])
                {
                    m_objIDSCamera.SetShuttle(m_smVisionInfo.g_arrCameraShuttle[0]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[0];
                }
            }

            if (m_objIDSCamera.ref_strErrorText != "")
            {
                m_smVisionInfo.g_strErrorMessage = m_objIDSCamera.ref_strErrorText;
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                return false;
            }
            else
            {
                //AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                return true;
            }
        }

        public bool GrabMultiImage_IDS_GPIO_NoSetIntensity()
        {
            // No transfer time here
            m_smVisionInfo.g_objGrabTime.Start();

            m_objVisionIO.IOGrabbing.SetOn("V3Lead ");

            bool blnSeparateGrab = m_smVisionInfo.g_blnSeparateGrab;
            int intSelectedImage = m_smVisionInfo.g_intSelectedImage;

            bool blnRetry = false;
            for (int i = 0; i < m_intGrabRequire; i++)     // Maximum 2 grabs for pad vision
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                // Make sure light is OFF
                if ((m_objVisionIO.Grab1 != null && m_objVisionIO.Grab1.IsOn()) ||
                    (m_objVisionIO.Grab2 != null && m_objVisionIO.Grab2.IsOn()))
                {
                    m_objVisionIO.Grab1.SetOff("V3Lead ");
                    m_objVisionIO.Grab2.SetOff("V3Lead ");

                    Thread.Sleep(1);    // Sleep 1ms before ON back.
                }

                //if (i > 0)
                //    Thread.Sleep(5);

                // On Lighting
                if (i == 0 && m_objVisionIO.Grab1 != null)
                //if (i == 0 && m_objVisionIO.Grab1 != null && m_objVisionIO.Grab2 != null)
                {
                    m_objVisionIO.Grab1.SetOn("V3Lead ");
                    //    m_objVisionIO.Grab2.SetOn("V3Lead ");
                }

                if (i == 1 && m_objVisionIO.Grab2 != null)
                    m_objVisionIO.Grab2.SetOn("V3Lead ");

                // Set camera gain
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
                {
                    m_objIDSCamera.SetGain((int)m_smVisionInfo.g_arrCameraGain[i]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                }


                // Set camera shuttle
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[i])
                {
                    m_objIDSCamera.SetShuttle(m_smVisionInfo.g_arrCameraShuttle[i]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
                }

                // Start Grab image
                if (m_objIDSCamera.Grab())
                {
                    //m_objIDSCamera.WaitExposureDone();
                    Thread.Sleep((int)Math.Max(2, Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[i])));

                    // Off Lighting (Light Controller will automatically off the light if IO ON time longer than intensity setting)
                    if (i == 0 && m_objVisionIO.Grab1 != null)
                        m_objVisionIO.Grab1.SetOff("V3Lead ");
                    if (i == 1 && m_objVisionIO.Grab2 != null)
                        m_objVisionIO.Grab2.SetOff("V3Lead ");

                    // Get image frame
                    if (m_objIDSCamera.GetFrame(m_smVisionInfo.g_objMemoryImage))
                    {
                        m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, i);
                        m_smVisionInfo.g_arrImages[i].AddGain(m_smVisionInfo.g_arrImageGain[i]);
                    }
                    else
                    {
                        if (!blnRetry)
                        {
                            blnRetry = true;
                            i--;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                }
                else
                {
                    break;
                }
            }

            SetGrabDone(false);

            if (m_intGrabRequire > 1)
            {
                // Set first image gain to camera before next cycle camera grab is coming.
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[0])
                {
                    m_objIDSCamera.SetGain((int)m_smVisionInfo.g_arrCameraGain[0]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[0];
                }

                // Set first image shuttle to camera before next cycle camera grab is coming.
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[0])
                {
                    m_objIDSCamera.SetShuttle(m_smVisionInfo.g_arrCameraShuttle[0]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[0];
                }
            }

            if (m_objIDSCamera.ref_strErrorText != "")
            {
                m_smVisionInfo.g_strErrorMessage = m_objIDSCamera.ref_strErrorText;
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                return false;
            }
            else
            {
                //AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                return true;
            }
        }

        private void SetLightControllerIntensity_ForIOCardTriggerController(int intGrabIndex)
        {
            // change light source intensity for different image's effects
            for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
            {
                int intValueIndex = 0;

                // Due to some light source only ON for second image so its intensity value is at array no. 0.
                // So we need to loop to find which array no. is for that image
                for (int k = 1; k < m_smVisionInfo.g_arrLightSource[j].ref_arrValue.Count; k++)
                {
                    // if this image no is in array k
                    if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo != null)
                    {
                        if (m_smVisionInfo.g_arrLightSource[j].ref_arrImageNo[k] == intGrabIndex)
                        {
                            intValueIndex = k;
                            break;
                        }
                    }
                }

                switch (m_smCustomizeInfo.g_blnLEDiControl)
                {
                    case true:
                        if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << intGrabIndex)) > 0)
                        {
                            if (m_arrCameraIntensityPrev[j] != m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueIndex])
                            {
                                LEDi_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                   m_smVisionInfo.g_arrLightSource[j].ref_intChannel,
                                   Convert.ToByte(m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueIndex]));

                                //TrackLog objTL = new TrackLog();
                                //objTL.WriteLine("Prev=" + m_arrCameraIntensityPrev[j].ToString() +
                                //                ", Port=" + m_smVisionInfo.g_arrLightSource[j].ref_intPortNo.ToString() +
                                //                ", Chn=" + m_smVisionInfo.g_arrLightSource[j].ref_intChannel.ToString() +
                                //                ", Value=" + Convert.ToByte(m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueIndex]).ToString());

                                m_arrCameraIntensityPrev[j] = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueIndex];
                                Thread.Sleep(5);
                            }
                        }
                        break;
                    case false:
                        if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << intGrabIndex)) > 0)
                        {
                            if (m_arrCameraIntensityPrev[j] != m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueIndex])
                            {
                                TCOSIO_Control.SetIntensity(m_smVisionInfo.g_arrLightSource[j].ref_intPortNo,
                                    m_smVisionInfo.g_arrLightSource[j].ref_intChannel,
                                    m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueIndex]);

                                m_arrCameraIntensityPrev[j] = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueIndex];
                                Thread.Sleep(2);
                            }
                        }
                        break;
                }
            }

        }

        public bool GrabMultiImage_IDS_IOCard_SetIntensity()
        {
            // No transfer time here
            m_smVisionInfo.g_objGrabTime.Start();

            m_objVisionIO.IOGrabbing.SetOn("V3Lead ");

            bool blnSeparateGrab = m_smVisionInfo.g_blnSeparateGrab;
            int intSelectedImage = m_smVisionInfo.g_intSelectedImage;

            bool blnRetry = false;
            for (int i = 0; i < m_intGrabRequire; i++)     // Maximum 2 grabs for pad vision
            {
                if (blnSeparateGrab)
                {
                    if (i != intSelectedImage)
                        continue;
                }

                // Make sure light is OFF
                if ((m_objVisionIO.Grab1 != null && m_objVisionIO.Grab1.IsOn()) ||
                    (m_objVisionIO.Grab2 != null && m_objVisionIO.Grab2.IsOn()) ||
                    (m_objVisionIO.Grab3 != null && m_objVisionIO.Grab3.IsOn()))
                {
                    m_objVisionIO.Grab1.SetOff("V3Lead ");
                    m_objVisionIO.Grab2.SetOff("V3Lead ");
                    m_objVisionIO.Grab3.SetOff("V3Lead ");

                    Thread.Sleep(1);    // Sleep 1ms before ON back.
                }

                SetLightControllerIntensity_ForIOCardTriggerController(i);

                // On Lighting
                for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                {
                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                    {
                        if (j == 0 && m_objVisionIO.Grab1 != null)
                            m_objVisionIO.Grab1.SetOn("V3Lead ");
                        if (j == 1 && m_objVisionIO.Grab2 != null)
                            m_objVisionIO.Grab2.SetOn("V3Lead ");
                        if (j == 2 && m_objVisionIO.Grab3 != null)
                            m_objVisionIO.Grab3.SetOn("V3Lead ");
                    }
                }

                // Set camera gain
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[i])
                {
                    m_objIDSCamera.SetGain((int)m_smVisionInfo.g_arrCameraGain[i]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                }

                // Set camera shuttle
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[i])
                {
                    m_objIDSCamera.SetShuttle(m_smVisionInfo.g_arrCameraShuttle[i]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
                }

                // Start Grab image
                if (m_objIDSCamera.Grab())
                {
                    //m_objIDSCamera.WaitExposureDone();
                    Thread.Sleep((int)Math.Max(2, Math.Ceiling(m_smVisionInfo.g_arrCameraShuttle[i])));

                    // Off Lighting (Light Controller will automatically off the light if IO ON time longer than intensity setting)
                    if (i == 0 && m_objVisionIO.Grab1 != null)
                        m_objVisionIO.Grab1.SetOff("V3Lead ");
                    if (i == 1 && m_objVisionIO.Grab2 != null)
                        m_objVisionIO.Grab2.SetOff("V3Lead ");


                    // Get image frame
                    //if (m_objIDSCamera.GetFrame(m_smVisionInfo.g_objMemoryImage))
                    //{
                    //    m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, i);
                    //}
                    //else
                    //{
                    //    if (!blnRetry)
                    //    {
                    //        blnRetry = true;
                    //        i--;
                    //        continue;
                    //    }
                    //    else
                    //    {
                    //        break;
                    //    }
                    //}

                }
                else
                {
                    break;
                }
            }

            SetGrabDone(false);

            if (m_intGrabRequire > 1)
            {
                // Set first image gain to camera before next cycle camera grab is coming.
                if (m_intCameraGainPrev != m_smVisionInfo.g_arrCameraGain[0])
                {
                    m_objIDSCamera.SetGain((int)m_smVisionInfo.g_arrCameraGain[0]);
                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[0];
                }

                // Set first image shuttle to camera before next cycle camera grab is coming.
                if (m_fCameraShuttlePrev != m_smVisionInfo.g_arrCameraShuttle[0])
                {
                    m_objIDSCamera.SetShuttle(m_smVisionInfo.g_arrCameraShuttle[0]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[0];
                }
            }

            if (m_objIDSCamera.ref_strErrorText != "")
            {
                m_smVisionInfo.g_strErrorMessage = m_objIDSCamera.ref_strErrorText;
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                return false;
            }
            else
            {
                //AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
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

        public void WaitAllThreadStopped()
        {
            HiPerfTimer timesout = new HiPerfTimer();
            timesout.Start();

            while(true)
            {
                if (m_blnStopped && 
                    m_blnStopped_AfterInspect && 
                    m_blnStopped_CenterThread && 
                    m_blnStopped_GrabImage &&
                    m_blnStopped_SideBRThread && 
                    m_blnStopped_SideTLThread &&
                    m_blnStopped_SaveImage
                    )
                {
                    STTrackLog.WriteLine("Vision3Lead3DProcess All threads have stopped.");
                    break;
                }

                if (timesout.Timing > 3000)
                {
                    STTrackLog.WriteLine("Vision3Lead3DProcess : m_blnStopped = " + m_blnStopped.ToString());
                    STTrackLog.WriteLine("Vision3Lead3DProcess : m_blnStopped_AfterInspect = " + m_blnStopped_AfterInspect.ToString());
                    STTrackLog.WriteLine("Vision3Lead3DProcess : m_blnStopped_CenterThread = " + m_blnStopped_CenterThread.ToString());
                    STTrackLog.WriteLine("Vision3Lead3DProcess : m_blnStopped_GrabImage = " + m_blnStopped_GrabImage.ToString());
                    STTrackLog.WriteLine("Vision3Lead3DProcess : m_blnStopped_SideBRThread = " + m_blnStopped_SideBRThread.ToString());
                    STTrackLog.WriteLine("Vision3Lead3DProcess : m_blnStopped_SideTLThread = " + m_blnStopped_SideTLThread.ToString());
                    STTrackLog.WriteLine("Vision3Lead3DProcess : m_blnStopped_SaveImage = " + m_blnStopped_SaveImage.ToString());
                    STTrackLog.WriteLine("Vision3Lead3DProcess : >>>>>>>>>>>>> time out 3");
                    break;
                }

                Thread.Sleep(1);
            }
        }

        public bool InitCamera(int intPort, String SerialNo, int intResolutionX, int intResolutionY, bool blnFirstTime)
        {
            bool blnInitSuccess = true;

            if (blnFirstTime)
            {
                if (m_smVisionInfo.g_strCameraModel == "IDS")
                {
                    //if (!m_objIDSCamera.InitializeCamera_AutoSetFrameRate(1))
                    if (!m_objIDSCamera.InitializeCamera(1))    // XDW16-001
                        blnInitSuccess = false;
                }
                else if (m_smVisionInfo.g_strCameraModel == "AVT")
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
            else if (m_smVisionInfo.g_strCameraModel == "IDS")
            {
                //m_objIDSCamera.SetShuttle(m_smVisionInfo.g_fCameraShuttle);
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
                    if (objLightSource.ref_intValue > 255)
                        objLightSource.ref_intValue = 255;
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
                    // Maximum grab 3 times
                    for (int j = 0; j < 3; j++)
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
                                    //m_smVisionInfo.g_arrCenterROIRotatedImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    //m_smVisionInfo.g_arrSideROIRotatedImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    if (m_smVisionInfo.g_arr5SRotatedImages[0] == null)
                                    {
                                        m_smVisionInfo.g_arr5SRotatedImages[0] = new List<ImageDrawing>();
                                        if (m_smVisionInfo.g_arrLead3D.Length > 1)
                                        {
                                            m_smVisionInfo.g_arr5SRotatedImages[1] = new List<ImageDrawing>();
                                            m_smVisionInfo.g_arr5SRotatedImages[2] = new List<ImageDrawing>();
                                            m_smVisionInfo.g_arr5SRotatedImages[3] = new List<ImageDrawing>();
                                            m_smVisionInfo.g_arr5SRotatedImages[4] = new List<ImageDrawing>();
                                        }
                                    }
                                    m_smVisionInfo.g_arr5SRotatedImages[0].Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    if (m_smVisionInfo.g_arrLead3D.Length > 1)
                                    {
                                        m_smVisionInfo.g_arr5SRotatedImages[1].Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                        m_smVisionInfo.g_arr5SRotatedImages[2].Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                        m_smVisionInfo.g_arr5SRotatedImages[3].Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                        m_smVisionInfo.g_arr5SRotatedImages[4].Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    }
                                    m_smVisionInfo.g_arrCameraShuttle.Add(new float());
                                    m_smVisionInfo.g_arrCameraGain.Add(new int());
                                    m_smVisionInfo.g_arrImageGain.Add(new float());

                                    if (m_arr5SImageRotated2[0] == null)
                                    {
                                        m_arr5SImageRotated2[0] = new List<bool>();
                                        if (m_smVisionInfo.g_arrLead3D.Length > 1)
                                        {
                                            m_arr5SImageRotated2[1] = new List<bool>();
                                            m_arr5SImageRotated2[2] = new List<bool>();
                                            m_arr5SImageRotated2[3] = new List<bool>();
                                            m_arr5SImageRotated2[4] = new List<bool>();
                                        }
                                    }
                                    m_arr5SImageRotated2[0].Add(false);
                                    if (m_smVisionInfo.g_arrLead3D.Length > 1)
                                    {
                                        m_arr5SImageRotated2[1].Add(false);
                                        m_arr5SImageRotated2[2].Add(false);
                                        m_arr5SImageRotated2[3].Add(false);
                                        m_arr5SImageRotated2[4].Add(false);
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

                    if (!m_smCustomizeInfo.g_blnMixController)
                    {
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
                    }
                    else
                    {
                        if (m_intLightControlModel == 0)    // 2018 07 13 - CCENG: Temporary force to use VTControl
                        {
                            LEDi_Control.SetIntensity(x, objLightSource.ref_intChannel, Convert.ToByte(objLightSource.ref_arrValue[0]));
                        }
                        else if (m_intLightControlModel == 1)   // 2018 07 13 - CCENG: Temporary force to use VTControl
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

                    }


                    Thread.Sleep(5); // Delay after set intensity to light source controller

                    // Keep light source intensity previous setting
                    m_arrCameraIntensityPrev[i] = objLightSource.ref_intValue;
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
                    else if (m_smVisionInfo.g_strCameraModel == "IDS")
                        m_objIDSCamera.SetShuttle(m_smVisionInfo.g_arrCameraShuttle[i]);
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                    {
                        m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[i]);
                    }

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
                    else if (m_smVisionInfo.g_strCameraModel == "IDS")
                        m_objIDSCamera.SetGain((int)m_smVisionInfo.g_arrCameraGain[i]);
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                        m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);

                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
            {
                m_smVisionInfo.g_arrImageGain[i] = objFileHandle.GetValueAsFloat("ImageGain" + i.ToString(), 1);
            }

            // Grab for first time after init to active the grab function (bug from IDS)
            if (m_smVisionInfo.g_strCameraModel == "IDS")
                m_objIDSCamera.Grab();

            return blnInitSuccess;
        }

        public bool InitCamera_old(int intPort)
        {
            bool blnInitSuccess = true;

            if (m_smVisionInfo.g_strCameraModel == "IDS")
            {
                //if (!m_objIDSCamera.InitializeCamera_AutoSetFrameRate(1))
                if (!m_objIDSCamera.InitializeCamera(1))    // XDW16-001
                    blnInitSuccess = false;
            }
            else
            {
                if (!m_objAVTFireGrab.InitializeCamera(intPort, false))
                    blnInitSuccess = false;
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
            else if (m_smVisionInfo.g_strCameraModel == "IDS")
            {
                //m_objIDSCamera.SetShuttle(m_smVisionInfo.g_fCameraShuttle);
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
                    if (objLightSource.ref_intValue > 255)
                        objLightSource.ref_intValue = 255;
                    objLightSource.ref_intPortNo = x;

                    if (m_smVisionInfo.g_strCameraModel == "AVT")
                    {
                        int intCameraOutNo = Convert.ToInt32(objLightSource.ref_strType.Substring(objLightSource.ref_strType.Length - 1));

                        if (intCameraOutNo == 0)
                            m_objAVTFireGrab.OutPort(intCameraOutNo, m_smVisionInfo.g_intTriggerMode);
                        else
                            m_objAVTFireGrab.OutPort(intCameraOutNo, 0);

                    }

                    objLightSource.ref_arrValue = new List<int>();
                    objLightSource.ref_arrImageNo = new List<int>();
                    // Maximum grab 3 times
                    for (int j = 0; j < 3; j++)
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
                                    //m_smVisionInfo.g_arrCenterROIRotatedImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    //m_smVisionInfo.g_arrSideROIRotatedImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    if (m_smVisionInfo.g_arr5SRotatedImages[0] == null)
                                    {
                                        m_smVisionInfo.g_arr5SRotatedImages[0] = new List<ImageDrawing>();
                                        if (m_smVisionInfo.g_arrLead3D.Length > 1)
                                        {
                                            m_smVisionInfo.g_arr5SRotatedImages[1] = new List<ImageDrawing>();
                                            m_smVisionInfo.g_arr5SRotatedImages[2] = new List<ImageDrawing>();
                                            m_smVisionInfo.g_arr5SRotatedImages[3] = new List<ImageDrawing>();
                                            m_smVisionInfo.g_arr5SRotatedImages[4] = new List<ImageDrawing>();
                                        }
                                    }
                                    m_smVisionInfo.g_arr5SRotatedImages[0].Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    if (m_smVisionInfo.g_arrLead3D.Length > 1)
                                    {
                                        m_smVisionInfo.g_arr5SRotatedImages[1].Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                        m_smVisionInfo.g_arr5SRotatedImages[2].Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                        m_smVisionInfo.g_arr5SRotatedImages[3].Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                        m_smVisionInfo.g_arr5SRotatedImages[4].Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                    }
                                    m_smVisionInfo.g_arrCameraShuttle.Add(new float());
                                    m_smVisionInfo.g_arrCameraGain.Add(new int());
                                    m_smVisionInfo.g_arrImageGain.Add(new float());

                                    if (m_arr5SImageRotated2[0] == null)
                                    {
                                        m_arr5SImageRotated2[0] = new List<bool>();
                                        if (m_smVisionInfo.g_arrLead3D.Length > 1)
                                        {
                                            m_arr5SImageRotated2[1] = new List<bool>();
                                            m_arr5SImageRotated2[2] = new List<bool>();
                                            m_arr5SImageRotated2[3] = new List<bool>();
                                            m_arr5SImageRotated2[4] = new List<bool>();
                                        }
                                    }
                                    m_arr5SImageRotated2[0].Add(false);
                                    if (m_smVisionInfo.g_arrLead3D.Length > 1)
                                    {
                                        m_arr5SImageRotated2[1].Add(false);
                                        m_arr5SImageRotated2[2].Add(false);
                                        m_arr5SImageRotated2[3].Add(false);
                                        m_arr5SImageRotated2[4].Add(false);
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
                    switch (m_smCustomizeInfo.g_blnLEDiControl)
                    {
                        case true:
                            LEDi_Control.SetIntensity(x, objLightSource.ref_intChannel, Convert.ToByte(objLightSource.ref_intValue));
                            break;
                        case false:
                            TCOSIO_Control.SetIntensity(x, objLightSource.ref_intChannel, objLightSource.ref_intValue);
                            TCOSIO_Control.SendMessage(x, "@ST" + objLightSource.ref_intChannel + "1*");    // Set Strobe ON
                            TCOSIO_Control.SendMessage(x, "@SI" + objLightSource.ref_intChannel + "00*");   // Set Constant Intensity to 0
                            break;
                    }

                    Thread.Sleep(5); // Delay after set intensity to light source controller

                    // Keep light source intensity previous setting
                    m_arrCameraIntensityPrev[i] = objLightSource.ref_intValue;
                }
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
                    else if (m_smVisionInfo.g_strCameraModel == "IDS")
                        m_objIDSCamera.SetShuttle(m_smVisionInfo.g_arrCameraShuttle[i]);

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
                    else if (m_smVisionInfo.g_strCameraModel == "IDS")
                        m_objIDSCamera.SetGain((int)m_smVisionInfo.g_arrCameraGain[i]);

                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
            {
                m_smVisionInfo.g_arrImageGain[i] = objFileHandle.GetValueAsFloat("ImageGain" + i.ToString(), 1);
            }

            // Grab for first time after init to active the grab function (bug from IDS)
            if (m_smVisionInfo.g_strCameraModel == "IDS")
                m_objIDSCamera.Grab();

            return blnInitSuccess;
        }

        public bool InitCameraSequence(int intPort, String SerialNo, int intResolutionX, int intResolutionY, bool blnFirstTime)
        {
            bool blnInitSuccess = true;

            if (blnFirstTime)
            {
                if (m_smVisionInfo.g_strCameraModel == "IDS")
                {
                    //if (!m_objIDSCamera.InitializeCamera_AutoSetFrameRate(1))
                    if (!m_objIDSCamera.InitializeCamera(1))    // XDW16-001
                        blnInitSuccess = false;
                }
                else if (m_smVisionInfo.g_strCameraModel == "AVT")
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
            else if (m_smVisionInfo.g_strCameraModel == "IDS")
            {
                //m_objIDSCamera.SetShuttle(m_smVisionInfo.g_fCameraShuttle);
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
                    if (objLightSource.ref_intValue > 255)
                        objLightSource.ref_intValue = 255;
                    objLightSource.ref_intPortNo = x;
                    objLightSource.ref_PHValue = objFileHandle.GetValueAsInt("PH" + strSearch, 31, 1);

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
                                        //m_smVisionInfo.g_arrCenterROIRotatedImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                        //m_smVisionInfo.g_arrSideROIRotatedImages.Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                        if (m_smVisionInfo.g_arr5SRotatedImages[0] == null)
                                        {
                                            m_smVisionInfo.g_arr5SRotatedImages[0] = new List<ImageDrawing>();
                                            if (m_smVisionInfo.g_arrLead3D.Length > 1)
                                            {
                                                m_smVisionInfo.g_arr5SRotatedImages[1] = new List<ImageDrawing>();
                                                m_smVisionInfo.g_arr5SRotatedImages[2] = new List<ImageDrawing>();
                                                m_smVisionInfo.g_arr5SRotatedImages[3] = new List<ImageDrawing>();
                                                m_smVisionInfo.g_arr5SRotatedImages[4] = new List<ImageDrawing>();
                                            }
                                        }
                                        m_smVisionInfo.g_arr5SRotatedImages[0].Add(new ImageDrawing(m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));

                                        if (m_smVisionInfo.g_arrLead3D.Length > 1)
                                        {
                                            m_smVisionInfo.g_arr5SRotatedImages[1].Add(new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                            m_smVisionInfo.g_arr5SRotatedImages[2].Add(new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                            m_smVisionInfo.g_arr5SRotatedImages[3].Add(new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                            m_smVisionInfo.g_arr5SRotatedImages[4].Add(new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                                        }
                                        m_smVisionInfo.g_arrCameraShuttle.Add(new float());
                                        m_smVisionInfo.g_arrCameraGain.Add(new int());
                                        m_smVisionInfo.g_arrImageGain.Add(new float());
                                        m_smVisionInfo.g_arrImageExtraGain.Add(new List<float>(5) { 0, 0, 0, 0, 0 });

                                        if (m_arr5SImageRotated2[0] == null)
                                        {
                                            m_arr5SImageRotated2[0] = new List<bool>();
                                            if (m_smVisionInfo.g_arrLead3D.Length > 1)
                                            {
                                                m_arr5SImageRotated2[1] = new List<bool>();
                                                m_arr5SImageRotated2[2] = new List<bool>();
                                                m_arr5SImageRotated2[3] = new List<bool>();
                                                m_arr5SImageRotated2[4] = new List<bool>();
                                            }
                                        }

                                        m_arr5SImageRotated2[0].Add(false);
                                        if (m_smVisionInfo.g_arrLead3D.Length > 1)
                                        {
                                            m_arr5SImageRotated2[1].Add(false);
                                            m_arr5SImageRotated2[2].Add(false);
                                            m_arr5SImageRotated2[3].Add(false);
                                            m_arr5SImageRotated2[4].Add(false);
                                        }
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

                    // Keep light source intensity previous setting
                    m_arrCameraIntensityPrev[i] = objLightSource.ref_intValue;
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

            //2020-10-12 ZJYEOH : Reassign intensity value based on Image Display Mode
            if (m_smVisionInfo.g_arrLightSource.Count > 4 && m_smVisionInfo.g_intImageDisplayMode == 1 && m_smVisionInfo.g_intImageMergeType == 2)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
                {

                    for (int j = 0; j < m_smVisionInfo.g_arrLightSource[i].ref_arrValue.Count; j++)
                    {
                        switch (m_smVisionInfo.g_arrLightSource[i].ref_arrImageNo[j])
                        {
                            case 1: //Top Left
                                if ((j == 0 || j > 2) && (i < 4 && ((i % 2) == 0)))
                                    m_smVisionInfo.g_arrLightSource[i].ref_arrValue[j] = 0;
                                else if (j == 1 && ((i % 2) != 0))
                                    m_smVisionInfo.g_arrLightSource[i].ref_arrValue[j] = 0;
                                break;
                            case 2: //Bottom Right
                                if ((j == 0 || j > 2) && (i < 4 && ((i % 2) != 0)))
                                    m_smVisionInfo.g_arrLightSource[i].ref_arrValue[j] = 0;
                                else if (j == 2 && ((i % 2) == 0))
                                    m_smVisionInfo.g_arrLightSource[i].ref_arrValue[j] = 0;
                                break;
                            case 0: // Center
                            default: // Other
                                if ((j == 0 || j > 2) && (i < 4))
                                    m_smVisionInfo.g_arrLightSource[i].ref_arrValue[j] = m_smVisionInfo.g_arrLightSource[0].ref_arrValue[j];
                                else if ((j == 0 || j > 2) && (i > 3 && ((i % 2) != 0)))
                                    m_smVisionInfo.g_arrLightSource[i].ref_arrValue[j] = m_smVisionInfo.g_arrLightSource[i - 1].ref_arrValue[j];
                                break;
                        }
                    }
                }

                //Save Updated Value
                string strFilePath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] +
                 "\\Camera.xml";
                if (m_smVisionInfo.g_blnGlobalSharingCameraData)
                    strFilePath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml";

                XmlParser objFile = new XmlParser(strFilePath);
                objFile.WriteSectionElement(m_smVisionInfo.g_strVisionFolderName);


                STDeviceEdit.CopySettingFile(strFilePath, "");

                for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
                {
                    objFile.WriteElement1Value(m_smVisionInfo.g_arrLightSource[i].ref_strType, "");
                    for (int j = 0; j < m_smVisionInfo.g_arrLightSource[i].ref_arrValue.Count; j++)
                    {
                        objFile.WriteElement2Value("Seq" + j.ToString(), m_smVisionInfo.g_arrLightSource[i].ref_arrValue[j]);
                    }
                }
                objFile.WriteEndElement();
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">", m_smProductionInfo.g_strLotID);

            }
            if (!m_smCustomizeInfo.g_blnMixController)
            {
                if (m_smCustomizeInfo.g_blnLEDiControl)
                {
                    List<int> arrCOMList = new List<int>();
                    for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
                    {
                        bool blnFound = false;
                        for (int c = 0; c < arrCOMList.Count; c++)
                        {
                            if (arrCOMList[c] == m_smVisionInfo.g_arrLightSource[i].ref_intPortNo)
                            {
                                blnFound = true;
                                break;
                            }
                        }

                        if (!blnFound)
                            arrCOMList.Add(m_smVisionInfo.g_arrLightSource[i].ref_intPortNo);
                    }

                    //Set to stop mode
                    for (int c = 0; c < arrCOMList.Count; c++)
                        LEDi_Control.RunStop(arrCOMList[c], 0, false);  //LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);

                    Thread.Sleep(10);
                    for (int i = 0; i < m_intGrabRequire; i++)
                    {
                        int intValue1 = 0;
                        int intValue2 = 0;
                        int intValue3 = 0;
                        int intValue4 = 0;
                        int intValue5 = 0;
                        int intValue6 = 0;
                        int intValue7 = 0;
                        int intValue8 = 0;

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
                                                case 3:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue4 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                                case 4:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue5 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                                case 5:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue6 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                                case 6:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue7 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                                case 7:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue8 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                            }

                                            break;
                                        }
                                    }
                                }
                            }

                            // 2021 04 20 - CCENG: Reupdate intensity according to image display mode
                            LEDi_Control.UpdateIntensityValueAccordingToImageDisplayMode(m_smVisionInfo.g_intImageDisplayMode, i,
                                                                                        ref intValue1, ref intValue2, ref intValue3, ref intValue4,
                                                                                        ref intValue5, ref intValue6, ref intValue7, ref intValue8);

                            //Set all light source for sequence light controller for each grab
                            //LEDi_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, i, intValue1, intValue2, intValue3, intValue4);

                            if (arrCOMList.Count > 0)
                                LEDi_Control.SetSeqIntensity(arrCOMList[0], 0, i, intValue1, intValue2, intValue3, intValue4);
                            if (arrCOMList.Count > 1)
                                LEDi_Control.SetSeqIntensity(arrCOMList[1], 0, i, intValue5, intValue6, intValue7, intValue8);

                            Thread.Sleep(10);
                        }
                    }
                    for (int c = 0; c < arrCOMList.Count; c++)
                        LEDi_Control.SaveIntensity(arrCOMList[c], 0); //LEDi_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0);
                    Thread.Sleep(100);
                    //Set to run mode
                    for (int i = 0; i < arrCOMList.Count; i++)
                        LEDi_Control.RunStop(arrCOMList[i], 0, true);   // LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
                    Thread.Sleep(10);
                }
                #region VTControl
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
                #endregion
            }
            #region m_intLightControlModel!=2
            else
            {
                if (m_intLightControlModel == 0)    // 2018 07 13 - CCENG: Temporary force to use VTControl
                {
                    //Set to stop mode
                    LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);
                    Thread.Sleep(10);
                    for (int i = 0; i < m_intGrabRequire; i++)
                    {
                        int intValue1 = 0;
                        int intValue2 = 0;
                        int intValue3 = 0;
                        int intValue4 = 0;

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
                                                case 3:
                                                    if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                                    {
                                                        intValue4 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                                    }
                                                    break;
                                            }

                                            break;
                                        }
                                    }
                                }
                            }
                            //Set all light source for sequence light controller for each grab
                            LEDi_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, i, intValue1, intValue2, intValue3, intValue4);
                            Thread.Sleep(10);
                            TrackLog objTL = new TrackLog();
                            objTL.WriteLine("Vision 3");
                            objTL.WriteLine("Sequence number: " + i.ToString());
                            objTL.WriteLine("Com: " + m_smVisionInfo.g_arrLightSource[0].ref_intPortNo.ToString());
                            objTL.WriteLine("Intensity 1: " + intValue1.ToString());
                            objTL.WriteLine("Intensity 2: " + intValue2.ToString());
                            objTL.WriteLine("Intensity 3: " + intValue3.ToString());
                            objTL.WriteLine("Intensity 4: " + intValue4.ToString());
                        }
                    }
                    LEDi_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0);
                    Thread.Sleep(100);
                    //Set to run mode
                    LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
                    Thread.Sleep(10);
                }
                else if (m_intLightControlModel == 1)   // 2018 07 13 - CCENG: Temporary force to use VTControl
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
            }
            #endregion
            m_smVisionInfo.g_fPHCameraShuttle = objFileHandle.GetValueAsFloat("PHShutter", 200f);
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
                    else if (m_smVisionInfo.g_strCameraModel == "IDS")
                        m_objIDSCamera.SetShuttle(m_smVisionInfo.g_arrCameraShuttle[i]);
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                        m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_arrCameraShuttle[i]);
                    m_fCameraShuttlePrev = m_smVisionInfo.g_arrCameraShuttle[i];
                }
            }
            m_smVisionInfo.g_uintPHCameraGain = (uint)objFileHandle.GetValueAsInt("PHGain", 20);
            for (int i = 0; i < m_smVisionInfo.g_arrCameraGain.Count; i++)
            {
                m_smVisionInfo.g_arrCameraGain[i] = (uint)objFileHandle.GetValueAsInt("Gain" + i.ToString(), 1);

                if (i == 0)
                {
                    if (m_smVisionInfo.g_strCameraModel == "AVT")
                        m_objAVTFireGrab.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);
                    else if (m_smVisionInfo.g_strCameraModel == "IDS")
                        m_objIDSCamera.SetGain((int)m_smVisionInfo.g_arrCameraGain[i]);
                    else if (m_smVisionInfo.g_strCameraModel == "Teli")
                        m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_arrCameraGain[i]);

                    m_intCameraGainPrev = m_smVisionInfo.g_arrCameraGain[i];
                }
            }
            m_smVisionInfo.g_fPHImageGain = objFileHandle.GetValueAsFloat("PHImageGain", 1);
            for (int i = 0; i < m_smVisionInfo.g_arrImageGain.Count; i++)
            {
                m_smVisionInfo.g_arrImageGain[i] = objFileHandle.GetValueAsFloat("ImageGain" + i.ToString(), 1);
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImageExtraGain.Count; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrImageExtraGain[i].Count; j++)
                {
                    objFileHandle.GetSecondSection("ImageExtraGain" + i.ToString());
                    m_smVisionInfo.g_arrImageExtraGain[i][j] = objFileHandle.GetValueAsFloat("ImageExtraGain" + i.ToString() + "_" + j.ToString(), 1, 2);
                }
            }

            // Grab for first time after init to active the grab function (bug from IDS)
            if (m_smVisionInfo.g_strCameraModel == "IDS")
                m_objIDSCamera.Grab();

            //Load system ROI
            if (m_smVisionInfo.g_intImageMergeType != 0)
            {
                LoadSystemROISetting();
            }

            // Define Image View Count
            switch (m_smVisionInfo.g_intImageMergeType)
            {
                default:
                case 0:
                    {
                        m_smVisionInfo.g_intImageViewCount = m_intGrabRequire;
                    }
                    break;
                case 1:
                    {
                        // Merge 1 and 2 to Image View 1
                        m_smVisionInfo.g_intImageViewCount = m_intGrabRequire - 1;
                    }
                    break;
                case 2:
                    {
                        // Merge 1, 2, and 3 to Image View 1
                        m_smVisionInfo.g_intImageViewCount = m_intGrabRequire - 2;
                    }
                    break;
                case 3:
                    {
                        if (m_intGrabRequire < 4)
                        {
                            // Merge 1 and 2 to Image View 1 since have 3 images only.
                            m_smVisionInfo.g_intImageViewCount = m_intGrabRequire - 1;
                        }
                        else
                        {
                            // Merge 1 and 2 to Image View 1; Merge 3 and 4 to Image View 2
                            m_smVisionInfo.g_intImageViewCount = m_intGrabRequire - 2;
                        }
                    }
                    break;
                case 4:
                    {
                        // Merge 1, 2, and 3 to Image View 1, and Merge 4, 5 to Image View 2
                        m_smVisionInfo.g_intImageViewCount = m_intGrabRequire - 3;
                    }
                    break;
            }

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

        /// <summary>
        /// Attach corresponding image to its parent or ROI
        /// </summary>
        private void AttachImageToROI()
        {
            if (m_smVisionInfo.g_blnPixelGrayMaxRangeON)
            {
                if (m_smVisionInfo.g_fPixelGrayMaxRange == -1)
                {
                    m_smVisionInfo.g_arrImages[0].CopyTo(ref m_objTempImage);
                    m_smVisionInfo.g_fPixelGrayMaxRange = 0;
                }
                else
                {
                    if (m_objDestImage == null)
                    {
                        m_objDestImage = new ImageDrawing();
                    }
                    ImageDrawing.SubtractImage(m_objTempImage, m_smVisionInfo.g_arrImages[0], m_objDestImage);

                    int intHighestGrayValue = ImageDrawing.GetHightPixelGrayValue(m_objDestImage);

                    if (m_smVisionInfo.g_fPixelGrayMaxRange < intHighestGrayValue)
                    {
                        m_smVisionInfo.g_fPixelGrayMaxRange = intHighestGrayValue;
                    }
                }
            }

            //m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
            //if (m_smVisionInfo.g_arrImages.Count > 1)
            //    m_smVisionInfo.g_arrImages[1].CopyTo(m_smVisionInfo.g_arrRotatedImages[1]);
            //if (m_smVisionInfo.g_arrImages.Count > 2)
            //    m_smVisionInfo.g_arrImages[2].CopyTo(m_smVisionInfo.g_arrRotatedImages[2]);

            m_smVisionInfo.g_objCameraROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
            m_smVisionInfo.g_objCalibrateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

            //if (m_blnCustomWantPad || m_blnCustomWant5S)
            //{
            //    AttachToROI(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrImages[0]);

            //    if (m_blnCustomWantPositioning)
            //    {
            //        for (int i = 0; i < m_smVisionInfo.g_arrPositioningROIs.Count; i++)
            //            m_smVisionInfo.g_arrPositioningROIs[i].AttachImage(m_smVisionInfo.g_arrImages[0]);
            //    }
            //}

            if (m_blnCustomWantLead3D)
            {
                AttachToROI(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrImages[0]);

                if (m_blnCustomWantPositioning)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrPositioningROIs.Count; i++)
                        m_smVisionInfo.g_arrPositioningROIs[i].AttachImage(m_smVisionInfo.g_arrImages[0]);
                }
            }

            if (m_blnCustomWantPositioning)
                m_smVisionInfo.g_objPositioning.ref_objCrosshair.AttachImage(m_smVisionInfo.g_arrImages[0]);

            m_smVisionInfo.VS_VM_UpdateSmallPictureBox = true;

        }

        /// <summary>
        /// Attach ROI to its parent ROI or parent image
        /// </summary>
        /// <param name="arrROI">ROI</param>
        /// <param name="objImage">parent image</param>
        private void AttachToROI(List<List<ROI>> arrROIs, ImageDrawing objImage)
        {
            ROI objROI;

            for (int i = 0; i < arrROIs.Count; i++)
            {
                for (int j = 0; j < arrROIs[i].Count; j++)
                {
                    objROI = arrROIs[i][j];

                    switch (objROI.ref_intType)
                    {
                        case 1:
                            objROI.AttachImage(objImage);   // Attach Search ROI to image
                            break;
                        case 2:
                            objROI.AttachImage(arrROIs[i][0]);  // Attach Gauge ROI and Package ROI to Search ROI
                            break;
                        case 3:
                            if (objROI.ref_strROIName == "Don't Care ROI")
                                objROI.AttachImage(arrROIs[i][2]);          // Attach Dont care ROI to Package ROI
                            else
                                objROI.AttachImage(arrROIs[i][1]);          // Attach other ROI to Gauge ROI (if have)
                            break;
                    }
                    arrROIs[i][j] = objROI;
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
        private void GetCustomTest()
        {
            if ((m_smCustomizeInfo.g_intWantPad & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnCustomWantPad = true;
            }

            if ((m_smCustomizeInfo.g_intWantPad5S & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnCustomWant5S = true;
            }

            if ((m_smCustomizeInfo.g_intWantLead3D & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnCustomWantLead3D = true;
            }

            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnCustomWantPackage = true;
            }

            if ((m_smCustomizeInfo.g_intWantPositioning & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                m_blnCustomWantPositioning = true;
            }
        }

        private void RecordGRR()
        {
            // Check GRR turn ON or not
            if (!m_smVisionInfo.g_blnGRRON)
                return;

            try
            {
                string strSampleBlobsFeatures;
                string[] strSampleFeature = new string[100];
                int intLeadNumber = m_smVisionInfo.g_arrLead3D[0].GetBlobsFeaturesNumber();

                for (int i = 0; i < intLeadNumber; i++)
                {
                    //// Get current sample value
                    strSampleBlobsFeatures = m_smVisionInfo.g_arrLead3D[0].GetBlobFeaturesResult(i);
                    strSampleFeature = strSampleBlobsFeatures.Split('#');

                    for (int j = 0; j < strSampleFeature.Length; j++)
                    {
                        if (strSampleFeature[j] == "---")
                            strSampleFeature[j] = "-999";
                    }

                    if (!m_smVisionInfo.g_objGRR.Record(0, i, Convert.ToSingle(strSampleFeature[1])))
                        return;
                    if (!m_smVisionInfo.g_objGRR.Record(1, i, Convert.ToSingle(strSampleFeature[2])))
                        return;
                    if (!m_smVisionInfo.g_objGRR.Record(2, i, Convert.ToSingle(strSampleFeature[3])))
                        return;
                    if (!m_smVisionInfo.g_objGRR.Record(3, i, Convert.ToSingle(strSampleFeature[4])))
                        return;
                    if (!m_smVisionInfo.g_objGRR.Record(4, i, Convert.ToSingle(strSampleFeature[5])))
                        return;
                    if (!m_smVisionInfo.g_objGRR.Record(5, i, Convert.ToSingle(strSampleFeature[6])))
                        return;
                }

                m_smVisionInfo.g_objGRR.NextIndex();
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Vision 3 Lead3D GRR Error: " + ex.ToString());
            }
        }

        private void RecordGRR2()
        {
            // Check GRR turn ON or not
            if (!m_smVisionInfo.g_blnGRRON)
                return;

            try
            {
                string strSampleBlobsFeatures;
                string[] strSampleFeature = new string[100];

                int intRecordIndex = 0;
                string strLeadName = "";
                for (int p = 0; p < m_smVisionInfo.g_arrLead3D.Length; p++)
                {
                    if (p != 0)
                        break;

                    switch (p)
                    {
                        case 0:
                            strLeadName = "Middle Lead ";
                            break;
                        case 1:
                            strLeadName = "Top Lead ";
                            break;
                        case 2:
                            strLeadName = "Right Lead ";
                            break;
                        case 3:
                            strLeadName = "Bottom Lead ";
                            break;
                        case 4:
                            strLeadName = "Left Lead ";
                            break;
                    }

                    int intLeadNumber = m_smVisionInfo.g_arrLead3D[p].GetBlobsFeaturesNumber();

                    //float[] arrData = { 0.44f, 0.45f, 0.46f, 0.47f};
                    //float[] arrData = { 0.440f,   0.440f,   0.440f,   0.440f,   0.441f,   0.440f,   0.440f,   0.439f,   0.441f,
                    //                    0.426f,   0.428f,   0.428f,   0.427f,   0.428f,   0.428f,   0.428f,   0.428f,   0.428f,
                    //                    0.440f,   0.441f,   0.441f,   0.440f,   0.441f,   0.441f,   0.441f,   0.441f,   0.441f,
                    //                    0.443f,   0.443f,   0.442f,   0.443f,   0.443f,   0.443f,   0.443f,   0.443f,   0.442f,
                    //                    0.429f,   0.429f,   0.429f,   0.430f,   0.430f,   0.430f,   0.429f,   0.429f,   0.430f,
                    //                    0.426f,   0.424f,   0.425f,   0.424f,   0.424f,   0.425f,   0.424f,   0.424f,   0.423f,
                    //                    0.437f,   0.438f,   0.439f,   0.439f,   0.438f,   0.438f,   0.439f,   0.438f,   0.438f,
                    //                    0.444f,   0.444f,   0.445f,   0.445f,   0.444f,   0.445f,   0.444f,   0.445f,   0.444f,
                    //                    0.448f,   0.448f,   0.448f,   0.447f,   0.447f,   0.448f,   0.448f,   0.448f,   0.448f,
                    //                    0.430f,   0.431f,   0.431f,   0.431f,   0.431f,   0.430f,   0.431f,   0.431f,   0.431f};

                    //float[] arrData = { 0.439f,   0.437f,   0.437f,   0.437f,   0.438f,   0.438f,   0.438f,   0.437f,   0.437f,
                    //                    0.449f,   0.437f,   0.436f,   0.437f,   0.437f,   0.438f,   0.436f,   0.437f,   0.438f,
                    //                    0.437f,   0.437f,   0.436f,   0.436f,   0.436f,   0.437f,   0.436f,   0.437f,   0.437f,
                    //                    0.444f,   0.436f,   0.436f,   0.436f,   0.436f,   0.435f,   0.435f,   0.436f,   0.437f,
                    //                    0.435f,   0.425f,   0.427f,   0.426f,   0.426f,   0.426f,   0.428f,   0.428f,   0.425f,
                    //                    0.431f,   0.421f,   0.419f,   0.421f,   0.421f,   0.421f,   0.421f,   0.422f,   0.422f,
                    //                    0.437f,   0.429f,   0.428f,   0.427f,   0.430f,   0.430f,   0.426f,   0.427f,   0.429f,
                    //                    0.450f,   0.450f,   0.450f,   0.449f,   0.449f,   0.451f,   0.450f,   0.451f,   0.450f,
                    //                    0.428f,   0.422f,   0.422f,   0.421f,   0.422f,   0.422f,   0.421f,   0.422f,   0.422f,
                    //                    0.428f,   0.423f,   0.422f,   0.422f,   0.422f,   0.422f,   0.421f,   0.420f,   0.422f};

                    //if (m_smVisionInfo.g_objGRR.ref_intOperatorCount == 0 && m_smVisionInfo.g_objGRR.ref_intPartCount == 0 && m_smVisionInfo.g_objGRR.ref_intTrialCount == 0)
                    //{
                    //    intReadIndex = 0;
                    //}

                    for (int i = 0; i < intLeadNumber; i++)
                    {
                        //// Get current sample value
                        strSampleBlobsFeatures = m_smVisionInfo.g_arrLead3D[p].GetBlobFeaturesResult(i);
                        strSampleFeature = strSampleBlobsFeatures.Split('#');

                        for (int j = 0; j < strSampleFeature.Length; j++)
                        {
                            if (strSampleFeature[j] == "---")
                                strSampleFeature[j] = "-999";
                        }

                        //Offset
                        if (!m_smVisionInfo.g_objGRR.Record(0, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(strSampleFeature[1])))
                            return;
                        //Width
                        //if (i == 0)
                        //{
                        //    if (intReadIndex < arrData.Length)
                        //    {
                        //        strSampleFeature[2] = arrData[intReadIndex].ToString();
                        //        intReadIndex++;
                        //    }

                        //}
                            if (!m_smVisionInfo.g_objGRR.Record(1, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(strSampleFeature[2])))
                            return;
                        //Length
                        if (!m_smVisionInfo.g_objGRR.Record(2, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(strSampleFeature[3])))
                            return;
                        //Pitch
                        if (!m_smVisionInfo.g_objGRR.Record(3, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(strSampleFeature[4])))
                            return;
                        //Gap
                        if (!m_smVisionInfo.g_objGRR.Record(4, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(strSampleFeature[5])))
                            return;
                        //Skew
                        if (!m_smVisionInfo.g_objGRR.Record(5, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(strSampleFeature[6])))
                            return;
                        //StandOff
                        if (!m_smVisionInfo.g_objGRR.Record(6, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(strSampleFeature[7])))
                            return;
                        //Coplan
                        if (!m_smVisionInfo.g_objGRR.Record(7, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(strSampleFeature[8])))
                            return;
                        //if (!m_smVisionInfo.g_objGRR.Record(5, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(strSampleFeature[6])))
                        //    return;

                        intRecordIndex++;
                    }
                }

                m_smVisionInfo.g_objGRR.NextIndex();
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Vision 3 Lead3D GRR Error: " + ex.ToString());
            }
        }

        private void SaveGRR()
        {
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "GRRReport\\" +
                m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime + "\\" +
                m_smVisionInfo.g_strGRRName + ".xml");

            objFile.WriteSectionElement("Lot");
            objFile.WriteElement1Value("LotID", m_smProductionInfo.g_strLotID);
            objFile.WriteElement1Value("OperatorID", m_smProductionInfo.g_strOperatorID);
            objFile.WriteElement1Value("RecipeID", m_smProductionInfo.g_strRecipeID);
            objFile.WriteElement1Value("MachineID", m_smCustomizeInfo.g_strMachineID);
            string strDateTime = m_smVisionInfo.g_strGRRName.Substring(m_smVisionInfo.g_strGRRName.LastIndexOf('_') + 1);
            objFile.WriteElement1Value("GRRStartTime", strDateTime.Substring(0, 4) + "/" + strDateTime.Substring(4, 2) + "/" + strDateTime.Substring(6, 2) + "  " + strDateTime.Substring(8, 2) + ":" + strDateTime.Substring(10, 2) + ":" + strDateTime.Substring(12, 2));
            objFile.WriteElement1Value("PartNo", m_smVisionInfo.g_intPartNo);
            objFile.WriteElement1Value("OperatorNo", m_smVisionInfo.g_intOperatorNo);
            objFile.WriteElement1Value("TrialNo", m_smVisionInfo.g_intTrialNo);

            objFile.WriteEndElement();
        }

        /// <summary>
        /// Stop the grab timer and set grab IO off to indicate grab action has been done
        /// </summary>
        private void SetGrabDone(bool blnForInspection)
        {
            float fGrabDelay = 5 - m_smVisionInfo.g_objGrabTime.Timing;
            if (fGrabDelay >= 1)
                Thread.Sleep((int)fGrabDelay);

            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
            {
                if (blnForInspection) //2021-11-16 ZJYEOH : Change m_smVisionInfo.g_intMachineStatus == 2 to blnForInspection
                    m_smTCPIPIO.Send(m_smVisionInfo.g_intVisionIndex, "GRBRP", true, -1);
                //m_blnGrabbing_Out = false;
            }
            else
            {
                if (m_objVisionIO.IOGrabbing.IsOn())
                {
                    m_objVisionIO.IOGrabbing.SetOff("V3Lead ");
                }
            }
            m_blnGrabbing_Out = false;
            m_smVisionInfo.g_objGrabTime.Stop();

            m_smVisionInfo.g_objGrabDoneTime.Start();
        }

        private void UpdateDebugMode()
        {
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

                        CheckLowYield();
                        CheckContinuousPass();
                        CheckContinuousFail();
                        //Grab image
                        if (m_smVisionInfo.MN_PR_GrabImage || m_smVisionInfo.AT_PR_GrabImage)
                        {
                            m_smVisionInfo.g_blnGrabbing = true;
                            if (m_smVisionInfo.AT_PR_GrabImage) // 01-08-2019 ZJYEOH : Only clear drawing result when user pressed grab button, solved "grab before test" no drawings 
                                m_smVisionInfo.g_blnClearResult = true;

                            GrabImage(0, false);

                            // 2019-12-26 ZJYEOH : Copy Image to Rotated Image so that will not look weird when draw rotated Image
                            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                                m_smVisionInfo.g_arrImages[i].CopyTo(m_smVisionInfo.g_arrRotatedImages[i]);

                            m_smVisionInfo.g_blnViewRotatedImage = false;   // 2018 10 12 - CCENG: Reset g_blnViewRotatedImage to display original image when user press grab button
                            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                            m_smVisionInfo.VS_AT_UpdateQuantity = true;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            m_smVisionInfo.g_blnGrabbing = false;
                            m_smVisionInfo.MN_PR_GrabImage = false;
                            m_smVisionInfo.AT_PR_GrabImage = false;
                            
                        }

                        //Start live image
                        if (m_smVisionInfo.AT_PR_StartLiveImage && !m_smVisionInfo.AT_PR_PauseLiveImage)
                        {
                            m_smVisionInfo.g_blnGrabbing = true;
                            m_smVisionInfo.g_blnClearResult = true;

                            GrabImage(0, false);

                            // 2019-12-26 ZJYEOH : Copy Image to Rotated Image so that will not look weird when draw rotated Image
                            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                                m_smVisionInfo.g_arrImages[i].CopyTo(m_smVisionInfo.g_arrRotatedImages[i]);

                            m_smVisionInfo.g_blnViewRotatedImage = false;   // 2018 10 12 - CCENG: Reset g_blnViewRotatedImage to display original image when user press live button
                            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                            m_smVisionInfo.VS_AT_UpdateQuantity = true;
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            m_smVisionInfo.g_blnGrabbing = false;
                            Thread.Sleep(50);
                        }

                        //Attach each ROI to its parent
                        if (m_smVisionInfo.AT_PR_AttachImagetoROI)
                        {
                            AttachImageToROI();
                            m_smVisionInfo.AT_PR_AttachImagetoROI = false;
                            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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
                            m_smVisionInfo.PR_CO_DeleteProcessSuccess = true; // 2020-07-22 ZJYEOH : Simply Send delete success because Lead3D dont have new lot clear template function

                            m_smVisionInfo.PR_CO_DeleteTemplateDone = true;
                        }

                        //Perform manual pad/lead inspection
                        if (m_smVisionInfo.MN_PR_StartTest && !m_smVisionInfo.AT_PR_AttachImagetoROI)
                        {
                            m_smVisionInfo.g_objTotalTime.Start();
                            m_smVisionInfo.MN_PR_StartTest = false;
                          
                            StartTest_MultiThreading_Lead(false);

                            m_smVisionInfo.g_objTotalTime.Stop();
                            m_smVisionInfo.VM_AT_BlockImageUpdate = false;
                            m_smVisionInfo.PR_MN_UpdateInfo = true;
                            m_smVisionInfo.PR_TL_UpdateInfo = true;
                            m_smVisionInfo.PR_TL_UpdateInfo2 = true;
                            m_smVisionInfo.PR_MN_TestDone = true;
                            //2020-08-12 ZJYEOH : After tolerance orm close just set to false so that when manual test will check all leads
                            //m_smVisionInfo.AT_VM_OfflineTestAllLead3D = false;
                        }

                        //Perform production test
                        if ((!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.IOStartVision.IsOn() && m_smVisionInfo.g_intMachineStatus == 2) ||
                            (m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_blnStartVision_In && m_smVisionInfo.g_intMachineStatus == 2) || 
                            (m_smVisionInfo.g_blnDebugRUN && m_smVisionInfo.g_intMachineStatus == 2))
                        {
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
                                //m_blnGrabbing_Out = true;
                                m_intTCPIPResultID = -1;
                                m_fOffsetX = 0;
                                m_fOffsetY = 0;
                                m_fOffsetAngle = 0;

                            }
                            else
                            {
                                m_objVisionIO.IOEndVision.SetOff("1 ");
                                m_objVisionIO.IOPass1.SetOff("V3Lead ");
                                m_objVisionIO.IOGrabbing.SetOn("V3Lead ");   //STTrackLog.WriteLine("Set IO Grabbing ON");
                            }
                            m_blnGrabbing_Out = true;
                            StartTest_MultiThreading_Lead(true);

                            m_smVisionInfo.g_objTotalTime.Stop();

                            if (!m_blnForceStopProduction)
                            {
                                if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                                {
                                    m_blnEndVision_Out = true;
                                }
                                else
                                    m_objVisionIO.IOEndVision.SetOn("2 ");
                            }
                            else
                            {
                                STTrackLog.WriteLine("Vision3Lead3DProcess > Force Stop Production");
                                m_blnForceStopProduction = false;
                                m_smVisionInfo.g_intMachineStatus = 1;
                            }

                            m_smProductionInfo.VM_TH_UpdateCount = true;

                            m_smVisionInfo.VS_AT_ProductionTestDone = true;

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
                                m_objVisionIO.IOEndVision.SetOff("3 ");
                            else if (m_objVisionIO.IOEndVision.IsOff() && m_smVisionInfo.g_intMachineStatus == 2)
                                m_objVisionIO.IOEndVision.SetOn("4 ");
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

                        UpdateDebugMode();
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Vision3Lead3DProcess->UpdateProgress() :" + ex.ToString());
                SRMMessageBox.Show("Vision3Lead3DProcess has been terminated. Please Exit SRMVision software and Run again!", "Vision3Process", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (m_smVisionInfo.g_strCameraModel == "IDS")
                {
                }
                else if (m_smVisionInfo.g_strCameraModel == "AVT")
                    m_objAVTFireGrab.OFFCamera();
                else if (m_smVisionInfo.g_strCameraModel == "Teli")
                    m_objTeliCamera.OFFCamera();

                m_blnStopped = true;
                //SetStopped();
                m_thThread = null;
            }
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
                        GrabImage(0, m_blnAuto);
                    }

                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Vision1Process->UpdateSubProgress_GrabImage() :" + ex.ToString());
                }
                Thread.Sleep(1);
            }

            m_thSubThread_GrabImage = null;
            m_blnStopped_GrabImage = true;

        }

        
        private void UpdateSubProgress_Center()
        {
            /*
             * ------------ Lead Inspection Sequence # ------------------------:
             *              
             * Sequence 1:  - Center Edge Dimension > Image 2
             *              - Center Leads Dimension > Image 2
             *              - Center Package Defect > Image 2
             *              - Center Package Defect 2 > Image 3
             */

            while (!m_blnStopping)
            {
                try
                {
                    if (m_bSubTh_CenterTest)
                    {
                        //m_T2.Start();
                        //m_fTiming2 = 0;
                        //m_fTimingPrev2 = 0;
                        m_strTrack_Center = "";

                        //m_fTiming2 = m_T2.Timing;
                        //m_strTrack_Center += ", C1=" + (m_fTiming2 - m_fTimingPrev2).ToString();
                        //m_fTimingPrev2 = m_fTiming2;

                        m_bSubTh_CenterTest_Result = false;

                        WaitEventDone(ref m_bSubTh_SideTLTest_LeadEdgeDone, true, 10000, "C1");
                        WaitEventDone(ref m_bSubTh_SideBRTest_LeadEdgeDone, true, 10000, "C2");

                        bool blnPassSideBaseLine = m_bSubTh_SideTLTest_Result && m_bSubTh_SideBRTest_Result;

                        m_bGrabImageFinalResult = WaitEventDone(ref m_bGrabImage1Done, true, ref m_bGrabImage1Result, 10000, "C3");
                        //WaitEventDone(ref m_bGrabImage1Done, true);     // Image 2 (in arrImage[0]) is used for center edge, lead and package inspection
                        if ((m_bGrabImageFinalResult && blnPassSideBaseLine) || m_smVisionInfo.AT_VM_OfflineTestAllLead3D)
                        {
                            m_bSubTh_CenterTest_Result = !Lead3DInspection_CenterLead_MultiThreading(ref m_eInspectionResult_Center, 0);

                            m_smVisionInfo.g_arrLead3D[0].ref_blnFindLeadCenterPointsDone = true;
                            if (!m_bSubTh_CenterTest_Result && !m_smVisionInfo.AT_VM_OfflineTestAllLead3D)
                                m_smVisionInfo.g_arrLead3D[0].ref_blnFindLeadCenterPointsDone = true; // Set to true so that side thread can continue
                            else
                            {
                                WaitEventDone(ref m_bSubTh_SideTLTest, false, 10000, "C4");
                                WaitEventDone(ref m_bSubTh_SideBRTest, false, 10000, "C5");

                                if (!m_bSubTh_SideTest_BuildSideLeadFail || m_smVisionInfo.AT_VM_OfflineTestAllLead3D)
                                    m_bSubTh_CenterTest_Result = !Lead3DInspection_CompileSideLeadToCenterLead_MultiThreading(ref m_eInspectionResult_Center, 0) && m_bSubTh_CenterTest_Result;
                            }

                            if (!m_bSubTh_CenterTest_Result)// && m_eInspectionResult_Center == ResulType.FailLead)
                                m_arrErrorMessage[0] += m_smVisionInfo.g_arrLead3D[0].GetLeadFailTestDisplayResult();

                            if (!m_smVisionInfo.AT_VM_OfflineTestAllLead3D && m_blnCustomWantPackage && m_bSubTh_CenterTest_Result && (((m_smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask & 0x01) != 0) || m_smVisionInfo.g_arrLead3D[0].GetWantInspectPackage()))
                            {
                                m_bSubTh_CenterTest_Result = !Package5SInspection_MultiThreading(0x01, ref m_eInspectionResult_Center, 0, 0);

                                if (m_bSubTh_CenterTest_Result || m_smVisionInfo.AT_VM_OfflineTestAllLead3D)
                                {
                                    int intImageIndex = GetArrayImageIndex(1);  // Get Package Dark Field Defect Image Index. In this case, the image index is 1 for PadPackage and 2 for Pad5SPackage
                                    if (m_smVisionInfo.g_intImageMergeType == 1)   // "Merge Grab 1 and Grab 2" type. Total grab 3 images.
                                    {
                                        if (intImageIndex == 1)
                                        {
                                            m_bGrabImageFinalResult = WaitEventDone(ref m_bGrabImage2Done, true, ref m_bGrabImage2Result, 10000, "C6");
                                            TrackTiming(false, "CW2", false, m_smVisionInfo.g_blnTrackBasic);
                                        }
                                        // WaitEventDone(ref m_bGrabImage2Done, true);
                                        else
                                        {
                                            m_bGrabImageFinalResult = WaitEventDone(ref m_bGrabImage3Done, true, ref m_bGrabImage3Result, 10000, "C7");
                                            TrackTiming(false, "CW3", false, m_smVisionInfo.g_blnTrackBasic);
                                        }//WaitEventDone(ref m_bGrabImage3Done, true);
                                    }
                                    else if (m_smVisionInfo.g_intImageMergeType == 3)   // "Merge Grab 3 and Grab 4" type. Total grab 5 images.
                                    {
                                        intImageIndex = GetArrayImageIndex(2);  // Get Package Dark Field Defect Image Index. In this case, the image index is 2 for PadPackage and 4 for Pad5SPackage
                                        if (intImageIndex == 2)
                                        {
                                            m_bGrabImageFinalResult = WaitEventDone(ref m_bGrabImage3Done, true, ref m_bGrabImage3Result, 10000, "C8");
                                            TrackTiming(false, "CW2", false, m_smVisionInfo.g_blnTrackBasic);
                                        }
                                        // WaitEventDone(ref m_bGrabImage2Done, true);
                                        else
                                        {
                                            m_bGrabImageFinalResult = WaitEventDone(ref m_bGrabImage5Done, true, ref m_bGrabImage5Result, 10000, "C9");
                                            TrackTiming(false, "CW3", false, m_smVisionInfo.g_blnTrackBasic);
                                        }//WaitEventDone(ref m_bGrabImage3Done, true);
                                    }
                                    if (m_bGrabImageFinalResult)
                                    {
                                        m_bSubTh_CenterTest_Result = !Package5SInspection_MultiThreading(0x01, ref m_eInspectionResult_Center, 0, intImageIndex);
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (!blnPassSideBaseLine)
                            {
                                m_smVisionInfo.g_arrLead3D[0].ref_blnFindLeadCenterPointsDone = true;
                                m_bSubTh_CenterTest_Result = true;
                            }
                        }
                        m_bSubTh_CenterTest = false;
                    }
                    else if (m_bSubTh_PHTest)
                    {
                        m_bSubTh_PHTest_Result = false;
                        m_bGrabImageFinalResult = WaitEventDone(ref m_bGrabImage1Done, true, ref m_bGrabImage2Result, 10000, "C10");

                        m_smVisionInfo.g_arrPHROIs[0].AttachImage(m_smVisionInfo.g_arrImages[0]);

                        if (m_smVisionInfo.g_objPositioning.CheckPH(m_smVisionInfo.g_arrPHROIs[0]))
                        {
                            m_bSubTh_PHTest_Result = true;
                            //m_arrErrorMessage[0] = m_smVisionInfo.g_objPositioning.ref_strErrorMessage;


                        }
                        else
                        {
                            m_eInspectionResult_Center = ResulType.FailPH;
                            m_bSubTh_PHTest_Result = false;
                            m_arrErrorMessage[0] = m_smVisionInfo.g_objPositioning.ref_strErrorMessage;

                        }

                        m_bSubTh_PHTest = false;
                    }

                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Vision1Process->UpdateSubProgress_Center() :" + ex.ToString());

                    //m_smVisionInfo.g_arrImages[0].SaveImage("D:\\AGVFail.bmp");

                    m_bSubTh_CenterTest_Result = false;
                    m_bSubTh_PHTest = false;
                    m_bSubTh_CenterTest = false;

                }
                Thread.Sleep(1);
            }

            m_thSubThread_Center = null;
            m_blnStopped_CenterThread = true;

        }

        private void UpdateSubProgress_SideTL()
        {
            /*
             * Lead Inspection Sequence #
             * Sequence 1:  - Side Edge Dimension > Image 1
             *              - Side Lead Dimension > Image 1
             */

            while (!m_blnStopping)
            {

                try
                {
                    if (m_bSubTh_SideTLTest)
                    {
                        //T1.Start();
                        //fTiming = 0;
                        //fTimingPrev = 0;
                        //m_strTrack_TL = "";

                        //fTiming = T1.Timing;
                        //m_strTrack_TL += ", TL1=" + (fTiming - fTimingPrev).ToString();
                        //fTimingPrev = fTiming;

                        m_bSubTh_SideTLTest_Result = false;
                        m_eInspectionResult_SideTL = ResulType.Pass;

                        if (m_smVisionInfo.g_intImageMergeType == 0)        // "No Merge" type. Total grab 2 images.
                        {
                            WaitEventDone(ref m_bGrabImage1Done, true, 10000, "TL1");     // Image 1 (in arrImage[0]) is used for side package
                        }
                        else if (m_smVisionInfo.g_intImageMergeType == 1)   // "Merge Grab 1 and Grab 2" type. Total grab 2 images.
                        {
                            WaitEventDone(ref m_bGrabImage2Done, true, 10000, "TL2");     // Image 2 (in arrImage[0]) is used for side package 
                        }
                        else if (m_smVisionInfo.g_intImageMergeType == 2)   // "Merge All. Grab 1 and Grab 2 and Grab 3" type. Total grab 3 images.
                        {
                            WaitEventDone(ref m_bGrabImage3Done, true, 10000, "TL3");    
                        }

                        m_bSubTh_SideTLTest_Result = !Lead3DInspection_SideEdge_MultiThreading(0x12, ref m_eInspectionResult_SideTL, 0);

                        m_bSubTh_SideTLTest_LeadEdgeResult = m_bSubTh_SideTLTest_Result;
                        m_bSubTh_SideTLTest_LeadEdgeDone = true;    // Set this event to let center thread know that can start center unit edge checking now.

                        if (m_bSubTh_SideTLTest_Result || m_smVisionInfo.AT_VM_OfflineTestAllLead3D)
                        {
                            HiPerfTimer timeout = new HiPerfTimer();
                            timeout.Start();

                            // For for center ROI find the all leads position (blob center point only) done.
                            while (true)
                            {
                                if (m_smVisionInfo.g_arrLead3D[0].ref_blnFindLeadCenterPointsDone == true)
                                {
                                    break;
                                }
                                if (timeout.Timing > 10000)
                                {
                                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 9 - " + "TL4");
                                    STTrackLog.WriteLine("m_bSubTh_CenterTest_Result = " + m_bSubTh_CenterTest_Result.ToString());
                                    STTrackLog.WriteLine("AT_VM_OfflineTestAllLead3D = " + m_smVisionInfo.AT_VM_OfflineTestAllLead3D.ToString());
                                    STTrackLog.WriteLine("Waiting event timeout");
                                    break;
                                }
                                Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
                            }
                            
                            timeout.Stop();
                        }

                        if (!m_smVisionInfo.AT_VM_OfflineTestAllLead3D && (m_eInspectionResult_Center != ResulType.Pass))//(m_eInspectionResult_Center == ResulType.FailLead || m_eInspectionResult_Center == ResulType.FailPosition))
                            m_bSubTh_SideTLTest_Result = true;
                        else if((m_bSubTh_SideTLTest_Result && m_bSubTh_SideBRTest_LeadEdgeResult) || m_smVisionInfo.AT_VM_OfflineTestAllLead3D)
                            m_bSubTh_SideTLTest_Result = !Lead3DInspection_SideLead_MultiThreading(0x12, ref m_eInspectionResult_SideTL, 0);


                        m_bSubTh_SideTLTest = false;
                    }
                }
                catch (Exception ex)
                {
                    m_bSubTh_SideTLTest = false;
                    SRMMessageBox.Show("Vision1Process->UpdateSubProgress_SideTL() :" + ex.ToString());
                }
                Thread.Sleep(1);
            }

            m_thSubThread_SideTL = null;
            m_blnStopped_SideTLThread = true;
        }

        private void UpdateSubProgress_SideBR()
        {
            /*
             * ---------Lead Inspection Sequence # ----------------:
             * Sequence 1:  - Side Edge Dimension > Image 1
             *              - Side Lead Dimension > Image 1
             * 
             */

            while (!m_blnStopping)
            {
                try
                {
                    if (m_bSubTh_SideBRTest)
                    {
                        //T1.Start();
                        //fTiming = 0;
                        //fTimingPrev = 0;
                        //m_strTrack_BR = "";

                        //fTiming = T1.Timing;
                        //m_strTrack_BR += ", BR1=" + (fTiming - fTimingPrev).ToString();
                        //fTimingPrev = fTiming;

                        m_bSubTh_SideBRTest_Result = false;
                        m_eInspectionResult_SideBR = ResulType.Pass;

                        if (m_smVisionInfo.g_intImageMergeType == 0)        // "No Merge" type. Total grab 2 images.
                        {
                            WaitEventDone(ref m_bGrabImage1Done, true, 10000, "BR1");     // Image 1 (in arrImage[0]) is used for side package
                        }
                        else if (m_smVisionInfo.g_intImageMergeType == 1)   // "Merge Grab 1 and Grab 2" type. Total grab 3 images.
                        {
                            WaitEventDone(ref m_bGrabImage2Done, true, 10000, "BR2");     // Image 2 (in arrImage[0]) is used for side package 
                        }
                        else if (m_smVisionInfo.g_intImageMergeType == 2)   // "Merge All. Grab 1 and Grab 2 and Grab 3" type. Total grab 3 images.
                        {
                            WaitEventDone(ref m_bGrabImage3Done, true, 10000, "BR3");
                        }

                        m_bSubTh_SideBRTest_Result = !Lead3DInspection_SideEdge_MultiThreading(0x0C, ref m_eInspectionResult_SideBR, 0);

                        m_bSubTh_SideBRTest_LeadEdgeResult = m_bSubTh_SideBRTest_Result;
                        m_bSubTh_SideBRTest_LeadEdgeDone = true;    // Set this event to let center thread know that can start center unit edge checking now.

                        if (m_bSubTh_SideBRTest_Result || m_smVisionInfo.AT_VM_OfflineTestAllLead3D)
                        {
                            HiPerfTimer timeout = new HiPerfTimer();
                            timeout.Start();

                            // For for center ROI find the all leads position (blob center point only) done.
                            while (true)
                            {
                                if (m_smVisionInfo.g_arrLead3D[0].ref_blnFindLeadCenterPointsDone == true)
                                {
                                    break;
                                }
                                if (timeout.Timing > 10000)
                                {
                                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 9 - " + "BR4");
                                    STTrackLog.WriteLine("m_bSubTh_CenterTest_Result = " + m_bSubTh_CenterTest_Result.ToString());
                                    STTrackLog.WriteLine("AT_VM_OfflineTestAllLead3D = " + m_smVisionInfo.AT_VM_OfflineTestAllLead3D.ToString());
                                    STTrackLog.WriteLine("Waiting event timeout");
                                    break;
                                }
                                Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
                            }
                            timeout.Stop();
                        }

                        if (!m_smVisionInfo.AT_VM_OfflineTestAllLead3D && (m_eInspectionResult_Center != ResulType.Pass))// (m_eInspectionResult_Center == ResulType.FailLead || m_eInspectionResult_Center == ResulType.FailPosition))
                            m_bSubTh_SideBRTest_Result = true;
                        else if((m_bSubTh_SideBRTest_Result && m_bSubTh_SideTLTest_LeadEdgeResult) || m_smVisionInfo.AT_VM_OfflineTestAllLead3D)
                            m_bSubTh_SideBRTest_Result = !Lead3DInspection_SideLead_MultiThreading(0x0C, ref m_eInspectionResult_SideBR, 0);

                        m_bSubTh_SideBRTest = false;
                    }
                }
                catch (Exception ex)
                {
                    m_bSubTh_SideBRTest = false;
                    SRMMessageBox.Show("Vision1Process->UpdateSubProgress_SideBR() :" + ex.ToString());
                }
                Thread.Sleep(1);
            }

            m_thSubThread_SideBR = null;
            m_blnStopped_SideBRThread = true;
        }

        private void UpdateSubProgress_AfterInspect()
        {
            while (!m_blnStopping)
            {
                try
                {
                    if (m_bSubTh_StartAfterInspect)
                    {
                        //m_bSubTh_StartAfterInspect = false;

                        // Merge 5 side rotated images into 1 rotated image.
                        RotatedImageMerge();

                        ////Copy original image to rotated image first
                        //if (!m_smVisionInfo.g_blnInspectionInProgress)  // 2019 07 02 - CCENG: Skip copy image if next cycle of inspection has started.
                        //{
                        //    m_smVisionInfo.g_arrImages[0].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, 0);
                        //}

                        //TrackTiming(false, "AF1a", false, m_smVisionInfo.g_blnTrackBasic);

                        //if (!m_smVisionInfo.g_blnInspectionInProgress)  // 2019 07 02 - CCENG: Skip copy image if next cycle of inspection has started.
                        //{
                        //    if (m_smVisionInfo.g_arrImages.Count > 1)
                        //        m_smVisionInfo.g_arrImages[1].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, 1);
                        //}

                        //TrackTiming(false, "AF1b", false, m_smVisionInfo.g_blnTrackBasic);
                        //if (!m_smVisionInfo.g_blnInspectionInProgress)  // 2019 07 02 - CCENG: Skip copy image if next cycle of inspection has started.
                        //{
                        //    if (m_smVisionInfo.g_arrImages.Count > 2)
                        //        m_smVisionInfo.g_arrImages[2].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, 2);
                        //}

                        //TrackTiming(false, "AF1c", false, m_smVisionInfo.g_blnTrackBasic);
                        //if (!m_smVisionInfo.g_blnInspectionInProgress)  // 2019 07 02 - CCENG: Skip copy image if next cycle of inspection has started.
                        //{
                        //    if (m_smVisionInfo.g_arrImages.Count > 3)
                        //        m_smVisionInfo.g_arrImages[3].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, 3);
                        //}
                        //TrackTiming(false, "AF1d", false, m_smVisionInfo.g_blnTrackBasic);
                        //if (!m_smVisionInfo.g_blnInspectionInProgress)  // 2019 07 02 - CCENG: Skip copy image if next cycle of inspection has started.
                        //{
                        //    if (m_smVisionInfo.g_arrImages.Count > 4)
                        //        m_smVisionInfo.g_arrImages[4].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, 4);
                        //}
                        //TrackTiming(false, "AF1e", false, m_smVisionInfo.g_blnTrackBasic);
                        //if (!m_smVisionInfo.g_blnInspectionInProgress)  // 2019 07 02 - CCENG: Skip copy image if next cycle of inspection has started.
                        //{
                        //    if (m_blnRotateImageUpdated && m_smVisionInfo.g_blnPadInpected)
                        //        CopyToRotatedImage();
                        //}
                        //TrackTiming(false, "AF1f", false, m_smVisionInfo.g_blnTrackBasic);

                        // Unlock to allow other thread use the pad or lead variables e.g drawing.
                        if (!m_smVisionInfo.g_blnInspectionInProgress)  
                        {
                            if (m_blnCustomWantLead3D)
                            {
                                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                {
                                    if (m_smVisionInfo.g_arrLead3D[i] != null)
                                    {
                                        if (m_smVisionInfo.g_arrLead3D[i].ref_blnLock)
                                        {
                                            m_smVisionInfo.g_arrLead3D[i].ref_blnLock = false;
                                        }
                                    }
                                }
                            }
                        }
                        //else
                        //{
                        //    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        //    {
                        //        if (m_smVisionInfo.g_arrPad[i] != null)
                        //        {
                        //            if (m_smVisionInfo.g_arrPad[i].ref_blnInspectLock)
                        //            {
                        //                m_smVisionInfo.g_arrPad[i].ref_blnInspectLock = false;
                        //                //m_smVisionInfo.g_arrPad[i].m_strTrack2 += "FB,";
                        //            }
                        //        }
                        //    }
                        //}

                        TrackTiming(false, "AF1h", false, m_smVisionInfo.g_blnTrackBasic);
                        m_smVisionInfo.g_blnViewRotatedImage = m_blnRotateImageUpdated;     // Dun reset the g_blnViewRotatedImage to false early of inspection, but set g_blnViewRotatedImage after inspection. This is to prevent drawing keep blinking displaying non rotate and rotate image frequently.
                        m_smVisionInfo.VS_VM_UpdateSmallPictureBox = true;
                        m_smVisionInfo.VS_AT_UpdateQuantity = true;
                        m_smVisionInfo.PR_VM_UpdateQuantity = true;
                        if (m_bWantPHTest)
                        {
                            m_smVisionInfo.g_objPositioning.ref_blnDrawPHResult = true;
                            m_smVisionInfo.g_blnViewLeadInspection = false;
                        }
                        else
                        {
                            if (m_smVisionInfo.g_intViewInspectionSetting == 0)
                                m_smVisionInfo.g_blnViewLeadInspection = true;
                            else
                                m_smVisionInfo.g_strErrorMessage = "";
                            m_smVisionInfo.g_objPositioning.ref_blnDrawPHResult = false;
                        }
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                        //2020-04-30 ZJYEOH : Record Result Log
                        if (m_blnAuto && m_smVisionInfo.g_blnWantRecordResult && (m_smVisionInfo.g_intViewInspectionSetting == 0) && (m_smVisionInfo.g_intTestedTotal <= m_smVisionInfo.g_intResultLogMaxCount))
                            RecordResultLog();

                        m_bSubTh_StartAfterInspect = false;
                    }
                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Vision1Process->UpdateSubProgress_SideBR() :" + ex.ToString());
                }
                Thread.Sleep(1);
            }

            m_thSubThread_AfterInspect = null;
            m_blnStopped_AfterInspect = true;
        }
        private void CopyToRotatedImage()
        {
            //Copy center and 4 side ROI of 5SRotatedImage (Center Top, Left, Bottom, Right) to rotated image 1)
            ROI objROI = new ROI();

            try
            {

                int intViewImageCount = ImageDrawing.GetImageViewCount(m_smVisionInfo.g_intVisionIndex);

                for (int intSelectImageViewIndex = 0; intSelectImageViewIndex < intViewImageCount; intSelectImageViewIndex++)
                {
                    int intImageNo = ImageDrawing.GetArrayImageIndex(intSelectImageViewIndex, m_smVisionInfo.g_intVisionIndex);

                    for (int i = 0; i < 5; i++)
                    {
                        if (m_smVisionInfo.g_blnPadInpected)
                        {
                            if (m_arr5SImageRotated2[i] == null)
                                continue;
                            if (m_arr5SImageRotated2[i][intImageNo])
                                m_arrRotatedROI[i].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[i][intImageNo]);
                            else
                                m_arrRotatedROI[i].AttachImage(m_smVisionInfo.g_arrImages[intImageNo]);

                            objROI.LoadROISetting(m_arrRotatedROI[i].ref_ROIPositionX, m_arrRotatedROI[i].ref_ROIPositionY,
                                m_arrRotatedROI[i].ref_ROIWidth, m_arrRotatedROI[i].ref_ROIHeight);
                            objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[intImageNo]);
                            m_arrRotatedROI[i].CopyImage(ref objROI);
                        }
                    }
                }
            }
            catch
            {
                // let ignore it if exception happen because it is for drawing only. 
                // Exception happen because another new cycle of isnpection start already and the g_arr5SRotatedImages is being used for rotation.
            }
            objROI.Dispose();
        }
        private void RecordResultLog()
        {
            string strLotSaveResultLogPath;
            string strVisionResultLogFileName;
            string strPath;
            if (m_smVisionInfo.g_intVisionResetCount == 0)
            {
                strLotSaveResultLogPath = m_smProductionInfo.g_strHistoryDataLocation + "ResultLog\\" + m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime;
                strVisionResultLogFileName = m_smVisionInfo.g_strVisionFolderName + "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smProductionInfo.g_strLotStartTime;

                if (!Directory.Exists(strLotSaveResultLogPath))
                    Directory.CreateDirectory(strLotSaveResultLogPath);

                strPath = strLotSaveResultLogPath + "\\" + strVisionResultLogFileName + ".txt";
            }
            else
            {
                strLotSaveResultLogPath = m_smProductionInfo.g_strHistoryDataLocation + "ResultLog\\" + m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime;
                strVisionResultLogFileName = m_smVisionInfo.g_strVisionFolderName + "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" + "_" + m_smVisionInfo.g_strVisionResetCountTime;

                if (!Directory.Exists(strLotSaveResultLogPath))
                    Directory.CreateDirectory(strLotSaveResultLogPath);

                strPath = strLotSaveResultLogPath + "\\" + strVisionResultLogFileName + ".txt";
            }

            string ResultData = "";

            for (int a = 0; a < m_smVisionInfo.g_arrLead3D.Length; a++)
            {
                if (!m_smVisionInfo.g_arrLead3D[a].ref_blnSelected)
                    continue;
                
                int intBlobsCount = m_smVisionInfo.g_arrLead3D[a].GetBlobsFeaturesNumber();

                int intBlobsTotalCount_Side = 0;

                if (m_smVisionInfo.g_arrLead3D.Length > 1 && m_smVisionInfo.g_arrLead3D[1].ref_blnSelected)
                    intBlobsTotalCount_Side += m_smVisionInfo.g_arrLead3D[1].GetBlobsFeaturesNumber();
                if (m_smVisionInfo.g_arrLead3D.Length > 2 && m_smVisionInfo.g_arrLead3D[2].ref_blnSelected)
                    intBlobsTotalCount_Side += m_smVisionInfo.g_arrLead3D[2].GetBlobsFeaturesNumber();
                if (m_smVisionInfo.g_arrLead3D.Length > 3 && m_smVisionInfo.g_arrLead3D[3].ref_blnSelected)
                    intBlobsTotalCount_Side += m_smVisionInfo.g_arrLead3D[3].GetBlobsFeaturesNumber();
                if (m_smVisionInfo.g_arrLead3D.Length > 4 && m_smVisionInfo.g_arrLead3D[4].ref_blnSelected)
                    intBlobsTotalCount_Side += m_smVisionInfo.g_arrLead3D[4].GetBlobsFeaturesNumber();
                
                if ((a == 0) && ((m_smVisionInfo.g_arrLead3D[0].GetBlobsFeaturesNumber() + intBlobsTotalCount_Side) > 0))
                    ResultData += "Date Time=" + DateTime.Now.ToString() + ",";

                string strLeadDirection = "";
                switch (a)
                {
                    case 0:
                        strLeadDirection = "Center ";
                        break;
                    case 1:
                        strLeadDirection = "Top ";
                        break;
                    case 2:
                        strLeadDirection = "Right ";
                        break;
                    case 3:
                        strLeadDirection = "Bottom ";
                        break;
                    case 4:
                        strLeadDirection = "Left ";
                        break;
                }

                for (int i = 0; i < intBlobsCount; i++)
                {
                    List<string> arrResultList = new List<string>();

                    arrResultList = m_smVisionInfo.g_arrLead3D[a].GetBlobFeaturesResult_WithPassFailIndicator(i);

                    int intFailMask = Convert.ToInt32(arrResultList[arrResultList.Count - 1]);
                    int intFailOptionMask = m_smVisionInfo.g_arrLead3D[a].ref_intFailOptionMask;
                    bool blnWantCheckWidth = false, blnWantCheckLength = false, blnWantCheckPitchGap = false,
                        blnWantCheckStandOff = false, blnWantCheckCoplan = false;
                    
                    if ((intFailOptionMask & 0x40) > 0)
                        blnWantCheckWidth = true;
                    if ((intFailOptionMask & 0x80) > 0)
                        blnWantCheckLength = true;
                    if ((intFailOptionMask & 0x600) > 0)
                        blnWantCheckPitchGap = true;
                    if ((intFailOptionMask & 0x01) > 0)
                        blnWantCheckStandOff = true;
                    if ((intFailOptionMask & 0x02) > 0)
                        blnWantCheckCoplan = true;

                    if (blnWantCheckWidth)
                    {
                        ResultData += strLeadDirection + "Lead " + (i + 1).ToString() + " Tip Width=" + arrResultList[2].ToString() + ",";
                    }

                    if (blnWantCheckLength && a == 0)
                    {
                        ResultData += strLeadDirection + "Lead " + (i + 1).ToString() + " Tip Length=" + arrResultList[3].ToString() + ",";
                    }

                    if (blnWantCheckPitchGap && a == 0)
                    {
                        ResultData += strLeadDirection + "Lead " + (i + 1).ToString() + " Pitch=" + arrResultList[4].ToString() + ",";

                        ResultData += strLeadDirection + "Lead " + (i + 1).ToString() + " Gap=" + arrResultList[5].ToString() + ",";
                    }

                    if (blnWantCheckStandOff && a == 0)
                    {
                        ResultData += strLeadDirection + "Lead " + (i + 1).ToString() + " Stand Off=" + arrResultList[6].ToString() + ",";
                    }

                    if (blnWantCheckCoplan && a == 0)
                    {
                        ResultData += strLeadDirection + "Lead " + (i + 1).ToString() + " Coplan=" + arrResultList[8].ToString() + ",";
                    }

                }

            }

            bool blnWantCheckPackageSize = false;

            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                if ((m_smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask & 0x01) > 0)
                    blnWantCheckPackageSize = true;

            if (blnWantCheckPackageSize)
            {
                float fWidthMin = m_smVisionInfo.g_arrLead3D[0].GetUnitWidthMin(1);
                float fWidthMax = m_smVisionInfo.g_arrLead3D[0].GetUnitWidthMax(1);
                float fHeightMin = m_smVisionInfo.g_arrLead3D[0].GetUnitHeightMin(1);
                float fHeightMax = m_smVisionInfo.g_arrLead3D[0].GetUnitHeightMax(1);

                float fWidth = (m_smVisionInfo.g_arrLead3D[0].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[0].GetResultDownWidth_RectGauge4L(1)) / 2;

                //// 2019-10-25 ZJYEOH : Add Offset to package width
                //fWidth += m_smVisionInfo.g_arrLead3D[0].ref_fPackageWidthOffsetMM;

                float fHeight = (m_smVisionInfo.g_arrLead3D[0].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[0].GetResultRightHeight_RectGauge4L(1)) / 2;

                //// 2019-10-25 ZJYEOH : Add Offset to package height
                //fHeight += m_smVisionInfo.g_arrLead3D[0].ref_fPackageHeightOffsetMM;
                
                if (ResultData == "")
                    ResultData += "Date Time=" + DateTime.Now.ToString() + ",";

                ResultData += "Package Width=" + fWidth.ToString() + ",";
                ResultData += "Package Length=" + fHeight.ToString() + ",";
                
            }

            if (ResultData != "")
            {
                ResultData = ResultData.Substring(0, ResultData.Length - 1); // Remove comma
                STTrackLog.WriteLine_ForResultLog(strLotSaveResultLogPath, strVisionResultLogFileName, ResultData);
            }
        }
        private void GRRRecord()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intLeadTestMask">0x01=CenterROI, 0x02=TopROI, 0x04=RightROI, 0x08=BottomROI, 0x10=LeftROI</param>
        /// <param name="eInspectionResult"></param>
        /// <param name="intImageIndex"></param>
        /// <returns></returns>
        private bool Lead3DInspection_SideEdge_MultiThreading(int intLeadTestMask, ref ResulType eInspectionResult, int intImageIndex)
        {
            bool bTestFail = false;

            // Loop Lead Test From Middle, Up, Right, Down to Left
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if ((intLeadTestMask & (0x01 << i)) == 0)
                    continue;

                m_smVisionInfo.g_arrLead3D[i].ref_blnLock = true;  // Set true to stop drawing.

                if (m_arr5SFoundUnit[i] && m_arr5SImageRotated2[i][intImageIndex])
                    continue;

                // Identify pad defination for displaying fail message
                string strPosition = GetROIDefinition(i);

                if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                    m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
                
                // Find unit edge, corner point and angle
                if (!m_smVisionInfo.g_arrLead3D[i].InspectSideUnitBaseLine(m_smVisionInfo.g_arrLeadROIs[i][0]))
                {
                    bTestFail = true;
                    eInspectionResult = ResulType.FailLeadStandOff;// ResulType.FailPosition;   //2020 11 02 - CCENG: Zhang Zhong reqest to use Standoff fail counter when cannot find unit.
                    m_arrErrorMessage[i] = strPosition + m_smVisionInfo.g_arrLead3D[i].ref_strErrorMessage;
                    break;
                }
                
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = true;
            }

            if (bTestFail)
            {
                if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                    if (m_intTCPIPResultID == -1)
                        m_intTCPIPResultID = (int)TCPIPResulID.Fail;
            }

            return bTestFail;
        }

        private bool Lead3DInspection_CompileSideLeadToCenterLead_MultiThreading(ref ResulType eInspectionResult, int intImageIndex)
        {
            bool bTestFail = false;

            List<float> arrAllLeadWidth = new List<float>();
            List<float> arrAllLeadStandOff = new List<float>();
            List<int> arrAllLeadID = new List<int>();

            if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 0)    // Horizontal
            {
                List<float> arrSideLeadWidth = new List<float>();
                List<float> arrSideLeadStandOff = new List<float>();
                List<int> arrSideLeadID = new List<int>();

                m_smVisionInfo.g_arrLead3D[2].GetSampleLeadData(ref arrSideLeadID, ref arrSideLeadStandOff, ref arrSideLeadWidth);

                for (int i = 0; i < arrSideLeadID.Count; i++)
                {
                    int intInsertIndex = arrAllLeadID.Count;
                    for (int k = 0; k < arrAllLeadID.Count; k++)
                    {
                        if (arrSideLeadID[i] < arrAllLeadID[k])
                        {
                            intInsertIndex = k;
                            break;
                        }
                    }

                    arrAllLeadID.Insert(intInsertIndex, arrSideLeadID[i]);
                    arrAllLeadStandOff.Insert(intInsertIndex, arrSideLeadStandOff[i]);
                    arrAllLeadWidth.Insert(intInsertIndex, arrSideLeadWidth[i]);
                }

                m_smVisionInfo.g_arrLead3D[4].GetSampleLeadData(ref arrSideLeadID, ref arrSideLeadStandOff, ref arrSideLeadWidth);

                for (int i = 0; i < arrSideLeadID.Count; i++)
                {
                    int intInsertIndex = arrAllLeadID.Count;
                    for (int k = 0; k < arrAllLeadID.Count; k++)
                    {
                        if (arrSideLeadID[i] < arrAllLeadID[k])
                        {
                            intInsertIndex = k;
                            break;
                        }
                    }

                    arrAllLeadID.Insert(intInsertIndex, arrSideLeadID[i]);
                    arrAllLeadStandOff.Insert(intInsertIndex, arrSideLeadStandOff[i]);
                    arrAllLeadWidth.Insert(intInsertIndex, arrSideLeadWidth[i]);
                }
            }
            else
            {
                List<float> arrSideLeadWidth = new List<float>();
                List<float> arrSideLeadStandOff = new List<float>();
                List<int> arrSideLeadID = new List<int>();

                m_smVisionInfo.g_arrLead3D[1].GetSampleLeadData(ref arrSideLeadID, ref arrSideLeadStandOff, ref arrSideLeadWidth);

                for (int i = 0; i < arrSideLeadID.Count; i++)
                {
                    int intInsertIndex = arrAllLeadID.Count;
                    for (int k = 0; k < arrAllLeadID.Count; k++)
                    {
                        if (arrSideLeadID[i] < arrAllLeadID[k])
                        {
                            intInsertIndex = k;
                            break;
                        }
                    }

                    arrAllLeadID.Insert(intInsertIndex, arrSideLeadID[i]);
                    arrAllLeadStandOff.Insert(intInsertIndex, arrSideLeadStandOff[i]);
                    arrAllLeadWidth.Insert(intInsertIndex, arrSideLeadWidth[i]);
                }

                m_smVisionInfo.g_arrLead3D[3].GetSampleLeadData(ref arrSideLeadID, ref arrSideLeadStandOff, ref arrSideLeadWidth);

                for (int i = 0; i < arrSideLeadID.Count; i++)
                {
                    int intInsertIndex = arrAllLeadID.Count;
                    for (int k = 0; k < arrAllLeadID.Count; k++)
                    {
                        if (arrSideLeadID[i] < arrAllLeadID[k])
                        {
                            intInsertIndex = k;
                            break;
                        }
                    }

                    arrAllLeadID.Insert(intInsertIndex, arrSideLeadID[i]);
                    arrAllLeadStandOff.Insert(intInsertIndex, arrSideLeadStandOff[i]);
                    arrAllLeadWidth.Insert(intInsertIndex, arrSideLeadWidth[i]);
                }
            }
            //Find lead
            if (!m_smVisionInfo.g_arrLead3D[0].CheckDimension_AllSideLeads(arrAllLeadID, arrAllLeadStandOff, arrAllLeadWidth))
            {
                bTestFail = true;
                //eInspectionResult = ResulType.FailLead;

                List<int> arrCenterLeadFailMask = new List<int>();
                List<int> arrCenterLeadID = new List<int>();
                m_smVisionInfo.g_arrLead3D[0].GetCenterSampleLeadData(ref arrCenterLeadID, ref arrCenterLeadFailMask);

                if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 0)    // Horizontal
                {
                    m_smVisionInfo.g_arrLead3D[2].SetCenterBlobsFailMaskToSideSampleBlobs(arrCenterLeadID, arrCenterLeadFailMask);
                    m_smVisionInfo.g_arrLead3D[4].SetCenterBlobsFailMaskToSideSampleBlobs(arrCenterLeadID, arrCenterLeadFailMask);
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[1].SetCenterBlobsFailMaskToSideSampleBlobs(arrCenterLeadID, arrCenterLeadFailMask);
                    m_smVisionInfo.g_arrLead3D[3].SetCenterBlobsFailMaskToSideSampleBlobs(arrCenterLeadID, arrCenterLeadFailMask);
                }

                switch (m_smVisionInfo.g_arrLead3D[0].GetLeadFailResultMask())
                {
                    case "FailOffset":
                        eInspectionResult = ResulType.FailLeadOffset;
                        break;
                    case "FailSkew":
                        eInspectionResult = ResulType.FailLeadSkew;
                        break;
                    case "FailWidth":
                        eInspectionResult = ResulType.FailLeadWidth;
                        break;
                    case "FailLength":
                        eInspectionResult = ResulType.FailLeadLength;
                        break;
                    case "FailLengthVariance":
                        eInspectionResult = ResulType.FailLeadLengthVariance;
                        break;
                    case "FailPitchGap":
                        eInspectionResult = ResulType.FailLeadPitchGap;
                        break;
                    case "FailPitchVariance":
                        eInspectionResult = ResulType.FailLeadPitchVariance;
                        break;
                    case "FailStandOff":
                        eInspectionResult = ResulType.FailLeadStandOff;
                        break;
                    case "FailStandOffVariance":
                        eInspectionResult = ResulType.FailLeadStandOffVariance;
                        break;
                    case "FailCoplan":
                        eInspectionResult = ResulType.FailLeadCoplan;
                        break;
                    case "FailAGV":
                        eInspectionResult = ResulType.FailLeadAGV;
                        break;
                    case "FailSpan":
                        eInspectionResult = ResulType.FailLeadSpan;
                        break;
                    case "FailSweeps":
                        eInspectionResult = ResulType.FailLeadSweeps;
                        break;
                    case "FailUnCutTiebar":
                        eInspectionResult = ResulType.FailLeadUnCutTiebar;
                        break;
                    case "FailMissing":
                        eInspectionResult = ResulType.FailLeadMissing;
                        break;
                    case "FailContamination":
                        eInspectionResult = ResulType.FailLeadContamination;
                        break;
                    case "FailLead":
                    default:
                        eInspectionResult = ResulType.FailLead;
                        break;
                }
            }

            if (bTestFail)
            {
                if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                    if (m_intTCPIPResultID == -1)
                        m_intTCPIPResultID = (int)TCPIPResulID.Fail;
            }

            return bTestFail;
        }

        private bool Lead3DInspection_CenterLead_MultiThreading(ref ResulType eInspectionResult, int intImageIndex)
        {
            bool bTestFail = false;

            m_smVisionInfo.g_arrLead3D[0].ref_blnLock = true;  // Set true to stop drawing.

            // Identify pad defination for displaying fail message
            string strPosition = GetROIDefinition(0);

            if (m_smVisionInfo.g_arrLeadROIs[0].Count > 0)
                m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);

            m_smVisionInfo.g_arrLead3D[0].DefineCenterUnitEdge(m_smVisionInfo.g_arrLead3D[1].ref_pCornerPoint_Left, m_smVisionInfo.g_arrLead3D[1].ref_pCornerPoint_Right,
                                        m_smVisionInfo.g_arrLead3D[2].ref_pCornerPoint_Top, m_smVisionInfo.g_arrLead3D[2].ref_pCornerPoint_Bottom,
                                        m_smVisionInfo.g_arrLead3D[3].ref_pCornerPoint_Left, m_smVisionInfo.g_arrLead3D[3].ref_pCornerPoint_Right,
                                        m_smVisionInfo.g_arrLead3D[4].ref_pCornerPoint_Top, m_smVisionInfo.g_arrLead3D[4].ref_pCornerPoint_Bottom);

            if (!m_smVisionInfo.g_arrLead3D[0].MatchWithTemplateUnitPR(m_smVisionInfo.g_arrLeadROIs[0][0]))
            {
                m_arrErrorMessage[0] += "*Fail to find unit.";
                bTestFail = true;
                eInspectionResult = ResulType.FailLeadStandOff;// ResulType.FailPosition;   //2020 11 02 - CCENG: Zhang Zhong reqest to use Standoff fail counter when cannot find unit.
                if (!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.PositionReject != null)
                {
                    m_objVisionIO.PositionReject.SetOn("V3Lead ");
                    //if (m_blnAuto)
                    //    m_smVisionInfo.g_intPositionFailureTotal++;
                }
                else
                {
                    if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                        if (m_intTCPIPResultID == -1)
                            m_intTCPIPResultID = (int)TCPIPResulID.FailPosition;

                    m_blnPositionReject_Out = true;
                }
                if (!m_smVisionInfo.AT_VM_OfflineTestAllLead3D)
                {
                    m_smVisionInfo.g_arrLead3D[0].ref_blnViewLeadResultDrawing = true;
                    return bTestFail;
                }
            }

            if (!m_smVisionInfo.g_arrLead3D[0].CheckPositionUsingCenterPoint_PatternMatch())
            {
                float Angle = 0, XTolerance = 0, YTolerance = 0;
                m_smVisionInfo.g_arrLead3D[0].GetPositionResult_PatternMatch(ref Angle, ref XTolerance, ref YTolerance);

                if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                {
                    m_fOffsetX = XTolerance;
                    m_fOffsetY = YTolerance;
                    m_fOffsetAngle = Angle;
                }

                if (Math.Abs(Angle) >= m_smVisionInfo.g_arrLead3D[0].ref_fAngleTolerance)
                    m_arrErrorMessage[0] += "*" + strPosition + " Fail Position Angle Tolerance : Set = " + m_smVisionInfo.g_arrLead3D[0].ref_fAngleTolerance.ToString("f4") + ", Result = " + Angle.ToString("f4");
                if (Math.Abs(XTolerance) >= m_smVisionInfo.g_arrLead3D[0].ref_fXTolerance)
                    m_arrErrorMessage[0] += "*" + strPosition + " Fail Position X Tolerance : Set = " + m_smVisionInfo.g_arrLead3D[0].ref_fXTolerance.ToString("f4") + ", Result = " + XTolerance.ToString("f4");
                if (Math.Abs(YTolerance) >= m_smVisionInfo.g_arrLead3D[0].ref_fYTolerance)
                    m_arrErrorMessage[0] += "*" + strPosition + " Fail Position Y Tolerance : Set = " + m_smVisionInfo.g_arrLead3D[0].ref_fYTolerance.ToString("f4") + ", Result = " + YTolerance.ToString("f4");
                bTestFail = true;
                eInspectionResult = ResulType.FailLeadStandOff;// ResulType.FailPosition;   //2020 11 02 - CCENG: Zhang Zhong reqest to use Standoff fail counter when cannot find unit.
                if (!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.PositionReject != null)
                {
                    m_objVisionIO.PositionReject.SetOn("V3Lead ");
                    //if (m_blnAuto)
                    //    m_smVisionInfo.g_intPositionFailureTotal++;
                }
                else
                {
                    if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                        if (m_intTCPIPResultID == -1)
                            m_intTCPIPResultID = (int)TCPIPResulID.FailPosition;

                    m_blnPositionReject_Out = true;
                }
                if (!m_smVisionInfo.AT_VM_OfflineTestAllLead3D)
                    return bTestFail;
            }

            //RotateImagesTo0Degree_MultiThreading_Lead3D_PatternMatch(0, 0);
            RotateImagesTo0Degree_MultiThreading_Lead3D_UsingCenterROI(0, 0);
            // Rotate to 0 deg for middle pad
            //RotateImagesTo0Degree_MultiThreading_Lead3D(0, 0);  // Lead inspection always use image 1
            if (m_arr5SImageRotated2[0][0])
                m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[0][0]);

            if (m_smVisionInfo.g_blnWantPin1 && m_smVisionInfo.g_arrPin1 != null && m_smVisionInfo.g_arrPin1.Count > 0 && m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(0))
            {
                if (!StartPin1Test_MultiTreading())
                {
                    m_arrErrorMessage[0] = strPosition + "*Fail Pin 1";
                    m_smVisionInfo.g_blnDrawPin1Result = true;
                    bTestFail = true;
                    eInspectionResult = ResulType.FailPin1;
                    if (!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.PositionReject != null)
                    {
                        m_objVisionIO.PositionReject.SetOn("V3 PositionReject 151");
                    }
                    else
                    {
                        if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                            if (m_intTCPIPResultID == -1)
                                m_intTCPIPResultID = (int)TCPIPResulID.FailPosition;

                        m_blnPositionReject_Out = true;
                    }

                    if (!m_smVisionInfo.AT_VM_OfflineTestAllLead3D)
                        return bTestFail;
                }
            }

            //2020-08-11 ZJYEOH : blnCheckContamination will decide whether to check center pkg contamination
            bool blnCheckContamination = true;
            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                blnCheckContamination = false;
            
            //Find lead
            if (!m_smVisionInfo.g_arrLead3D[0].InspectCenterLeads(m_smVisionInfo.g_arrLeadROIs[0][0], blnCheckContamination))
            {
                bTestFail = true;
                //eInspectionResult = ResulType.FailLead;
                //m_arrErrorMessage[0] += m_smVisionInfo.g_arrLead3D[0].GetLeadFailTestDisplayResult("");   // 2018 12 14 - CCENG: Cannot call this GetLeadFailTestDisplayResult() function here because still got side lead data need to compile in center leads Lead3DInspection_CompileSideLeadToCenterLead_MultiThreading() function. 

                switch (m_smVisionInfo.g_arrLead3D[0].GetLeadFailResultMask())
                {
                    case "FailOffset":
                        eInspectionResult = ResulType.FailLeadOffset;
                        break;
                    case "FailSkew":
                        eInspectionResult = ResulType.FailLeadSkew;
                        break;
                    case "FailWidth":
                        eInspectionResult = ResulType.FailLeadWidth;
                        break;
                    case "FailLength":
                        eInspectionResult = ResulType.FailLeadLength;
                        break;
                    case "FailLengthVariance":
                        eInspectionResult = ResulType.FailLeadLengthVariance;
                        break;
                    case "FailPitchGap":
                        eInspectionResult = ResulType.FailLeadPitchGap;
                        break;
                    case "FailPitchVariance":
                        eInspectionResult = ResulType.FailLeadPitchVariance;
                        break;
                    case "FailStandOff":
                        eInspectionResult = ResulType.FailLeadStandOff;
                        break;
                    case "FailStandOffVariance":
                        eInspectionResult = ResulType.FailLeadStandOffVariance;
                        break;
                    case "FailCoplan":
                        eInspectionResult = ResulType.FailLeadCoplan;
                        break;
                    case "FailAGV":
                        eInspectionResult = ResulType.FailLeadAGV;
                        break;
                    case "FailSpan":
                        eInspectionResult = ResulType.FailLeadSpan;
                        break;
                    case "FailSweeps":
                        eInspectionResult = ResulType.FailLeadSweeps;
                        break;
                    case "FailUnCutTiebar":
                        eInspectionResult = ResulType.FailLeadUnCutTiebar;
                        break;
                    case "FailMissing":
                        eInspectionResult = ResulType.FailLeadMissing;
                        break;
                    case "FailContamination":
                        eInspectionResult = ResulType.FailLeadContamination;
                        break;
                    case "FailLead":
                    default:
                        eInspectionResult = ResulType.FailLead;
                        break;
                }
            }

            m_smVisionInfo.g_arrLead3D[0].ref_blnViewLeadResultDrawing = true;

            //if (m_smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance && !m_smVisionInfo.AT_VM_OfflineTestAllLead3D)
            //    m_smVisionInfo.g_blnViewLeadTipBuildAreaDrawing = true;

            if (bTestFail)
            {
                if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                    if (m_intTCPIPResultID == -1)
                        m_intTCPIPResultID = (int)TCPIPResulID.Fail;
            }

            return bTestFail;

        }

        private bool Lead3DInspection_SideLead_MultiThreading(int intLeadTestMask, ref ResulType eInspectionResult, int intImageIndex)
        {
            bool bTestFail = false;

            // Loop Lead Test From Middle, Up, Right, Down to Left
            for (int i = 1; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if ((intLeadTestMask & (0x01 << i)) == 0)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[i].ref_intLeadDirection == 0)    // Horizontal
                {
                    if (i == 1 || i == 3)   // Skip top and bottom
                        continue;
                }
                else // Vertical
                {
                    if (i == 2 || i == 4)   // Skip Left and right
                        continue;
                }

                int intBorderDirection = 0;
                switch (i)
                {
                    case 1:
                        intBorderDirection = 4;
                        break;
                    case 2:
                        intBorderDirection = 2;
                        break;
                    case 3:
                        intBorderDirection = 8;
                        break;
                    case 4:
                        intBorderDirection = 1;
                        break;
                }

                // Identify pad defination for displaying fail message
                string strPosition = GetROIDefinition(i);

                List<PointF> arrLeadCenterPoints = new List<PointF>();
                List<SizeF> arrLeadSizeF = new List<SizeF>();
                List<int> arrLeadID = new List<int>();
                List<int> arrLeadDirection = new List<int>();
                List<float> arrTipLength = new List<float>();
                List<float> arrWidthOffset = new List<float>();
                List<float> arrStandOffOffset = new List<float>();
                List<float> arrCoplanOffset = new List<float>();
                m_smVisionInfo.g_arrLead3D[0].GetSampleLeadCenterPointsAndSize(intBorderDirection, ref arrLeadID, ref arrLeadCenterPoints, ref arrLeadSizeF, ref arrLeadDirection, ref arrTipLength, ref arrWidthOffset, ref arrStandOffOffset, ref arrCoplanOffset); // 1 = Border Left 

                for (int j = 0; j < arrLeadCenterPoints.Count; j++)
                {
                    arrLeadCenterPoints[j] = new PointF(arrLeadCenterPoints[j].X + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX, arrLeadCenterPoints[j].Y + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY);
                }
               
                // 2020-08-04 ZJYEOH : Collect Side Blob Data for max point stand off method
                if (m_smVisionInfo.g_arrLead3D[i].ref_intLeadStandOffMethod == 0)
                {
                    if ((m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 0 && (i == 2 || i == 4)) ||
                        (m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 1 && (i == 1 || i == 3)))
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].BuildSideLead(m_smVisionInfo.g_arrLeadROIs[i][0], arrLeadID, arrLeadCenterPoints, arrLeadSizeF, arrLeadDirection))
                        {
                            m_bSubTh_SideTest_BuildSideLeadFail = true;
                            bTestFail = true;
                            eInspectionResult = ResulType.FailLeadStandOff;// ResulType.FailPosition;   //2020 11 02 - CCENG: Zhang Zhong reqest to use Standoff fail counter when cannot find unit.
                            m_arrErrorMessage[i] = strPosition + "Build Side Lead Fail";
                            m_smVisionInfo.g_arrLead3D[0].ref_arrFailResultMask[0] |= 0x800000;
                            break;
                        }
                    }
                }
                m_smVisionInfo.g_arrLead3D[i].SortSideLeadSamples(arrLeadID);

                m_smVisionInfo.g_arrLead3D[i].InspectSideLeads(m_smVisionInfo.g_arrLeadROIs[i][0], arrLeadID, arrLeadCenterPoints, arrLeadSizeF, arrTipLength, arrWidthOffset, arrStandOffOffset, arrCoplanOffset);
            }

            if (bTestFail)
            {
                if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                    if (m_intTCPIPResultID == -1)
                        m_intTCPIPResultID = (int)TCPIPResulID.Fail;
            }

            return bTestFail;
        }
        private bool StartPin1Test_MultiTreading()
        {
            // make sure template learn
            if (m_smVisionInfo.g_arrPin1[0].ref_arrTemplateSetting.Count == 0)
            {
                return false;
            }
            m_smVisionInfo.g_arrPin1[0].ResetInspectionData();
            m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

            //int intMatchCount = 0;
            //bool blnResult;
            //string strErrorMessage = "";

            //if (!blnAuto)
            //{
            if (m_smVisionInfo.g_arrPin1[0].ref_objTestROI == null)
                m_smVisionInfo.g_arrPin1[0].ref_objTestROI = new ROI();

            if (m_blnRotateImageUpdated)
                m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[0][0]);
            else
                m_smVisionInfo.g_arrLeadROIs[0][0].AttachImage(m_smVisionInfo.g_arrImages[0]);

            m_smVisionInfo.g_arrPin1[0].ref_objTestROI.AttachImage(m_smVisionInfo.g_arrLeadROIs[0][0]);

            //m_smVisionInfo.g_arrPin1[0].ref_objTestROI.SaveImage("D:\\TS\\TestROI.bmp");
            if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
            {
                m_smVisionInfo.g_arrPin1[0].ref_objTestROI.LoadROISetting(
                    (int)(m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX -
                    m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(0) - m_smVisionInfo.g_arrPin1[0].GetRefOffsetX(0)),
                    (int)(m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY -
                    m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(0) - m_smVisionInfo.g_arrPin1[0].GetRefOffsetY(0)),
                    (int)m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(0) * 2,
                    (int)m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(0) * 2);
            }
            else
            {
                m_smVisionInfo.g_arrPin1[0].ref_objTestROI.LoadROISetting(
                    (int)(m_smVisionInfo.g_arrLead3D[0].GetResultCenterPoint_RectGauge4L().X - m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX -
                    m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(0) * 0.75f - m_smVisionInfo.g_arrPin1[0].GetRefOffsetX(0)),
                    (int)(m_smVisionInfo.g_arrLead3D[0].GetResultCenterPoint_RectGauge4L().Y - m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY -
                    m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(0) * 0.75f - m_smVisionInfo.g_arrPin1[0].GetRefOffsetY(0)),
                    (int)(m_smVisionInfo.g_arrPin1[0].GetPin1PatternWidth(0) * 1.5),
                    (int)(m_smVisionInfo.g_arrPin1[0].GetPin1PatternHeight(0) * 1.5));
            }

            //m_smVisionInfo.g_arrPin1[0].ref_objTestROI.SaveImage("D:\\TS\\TestROI2.bmp");

            m_smVisionInfo.g_arrPin1[0].ref_blnFinalResultPassFail = m_smVisionInfo.g_arrPin1[0].MatchWithTemplate(m_smVisionInfo.g_arrPin1[0].ref_objTestROI, 0);
            m_smVisionInfo.g_arrPin1[0].ref_intFinalResultSelectedTemplate = 0;

            return m_smVisionInfo.g_arrPin1[0].ref_blnFinalResultPassFail;
        }
        private bool StartTest_MultiThreading_Lead(bool blnAuto)
        {
            m_blnAuto = blnAuto;

            m_smVisionInfo.g_objProcessTime.Start();

            TrackTiming(false, "0", false, m_smVisionInfo.g_blnTrackBasic);

            m_smVisionInfo.g_blnInspectionInProgress = true;

            WaitEventDone(ref m_bSubTh_StartAfterInspect, false, 10000, "A22");

            TrackTiming(false, "0a", false, m_smVisionInfo.g_blnTrackBasic);

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_blnInspectLock = true;
                //m_smVisionInfo.g_arrPad[i].m_strTrack2 += "T001,";
            }

            WaitDrawingLockEventDone();

            TrackTiming(false, "0b", false, m_smVisionInfo.g_blnTrackBasic);

            bool blnResultOK = true;
            m_smVisionInfo.g_blnLead3DInpected = false;
            m_smVisionInfo.g_blnViewLeadInspection = false;
            m_smVisionInfo.g_blnDrawPin1Result = false;

            m_blnRotateImageUpdated = false;
            for (int h = 0; h < m_arr5SImageRotated2.Length; h++)
            {
                if (m_arr5SImageRotated2[h] == null)
                    continue;

                for (int i = 0; i < m_arr5SImageRotated2[h].Count; i++)
                {
                    if (m_arr5SImageRotated2[h][i])
                        m_arr5SImageRotated2[h][i] = false;
                }

                m_arr5SFoundUnit[h] = false;
                m_arr5SFoundUnitPkg[h] = false;
                m_arr5SPackagedSizeChecked[h] = false;
            }

            m_smVisionInfo.g_blnGrabbing = true;
            m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = false;
            m_bGrabImageFinalResult = m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = false;
            m_smVisionInfo.g_blnNoGrabTime = false;
            m_smVisionInfo.g_blnViewLeadTipBuildAreaDrawing = false;

            if (blnAuto)
            {
                // Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay); //29-05-2019 ZJYEOH : Moved it to After Grabtime Start counting 

                //Display error message if fail to grab image 
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
                    m_blnGrabbing_Out = false;
                }
            }
            else if (m_smVisionInfo.MN_PR_GrabImage)    // Grab image before offline test 
            {
                m_smVisionInfo.MN_PR_GrabImage = false;

                m_bSubTh1_GrabImage = true;  // Trigger Sub Thread 1 to grab images
            }
            else    // Offline test
            {
                m_smVisionInfo.g_blnGrabbing = false;
                m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;
                m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = true;
                m_smVisionInfo.g_blnNoGrabTime = true;
            }

            // Check Lead Package Template Template Ready
            if (m_smVisionInfo.g_arrLeadROIs.Count == 0)
            {
                if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                    if (m_intTCPIPResultID == -1)
                        m_intTCPIPResultID = (int)TCPIPResulID.Fail;

                m_smVisionInfo.g_strErrorMessage += "*Lead : No Template Found";
                return false;
            }
            else if (m_smVisionInfo.g_arrPHROIs.Count == 0 && m_smVisionInfo.g_blnWantCheckPH && m_smVisionInfo.g_blnCheckPH)
            {
                if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                    if (m_intTCPIPResultID == -1)
                        m_intTCPIPResultID = (int)TCPIPResulID.Fail;

                m_smVisionInfo.g_strErrorMessage += "*Lead : No PH Template Found";
                return false;
            }

            // Reset IO criteria
            //if (m_blnCustomWantPositioning)
            if (!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.PositionReject != null)
            {
                m_objVisionIO.PositionReject.SetOff("V3Lead ");
                m_objVisionIO.EmptyUnit.SetOff("V3Lead ");
            }
            else
            {
                m_blnPositionReject_Out = true;
            }

            // Reset all previous inspection data
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (m_arrErrorMessage[i] != "")
                    m_arrErrorMessage[i] = "";

                m_smVisionInfo.g_arrLead3D[i].ResetInspectionData();
            }

            // 2020-06-01 ZJYEOH : Reset Pin 1 previous inspection data, so that offline page will display latest result
            if (m_smVisionInfo.g_blnWantPin1 && m_smVisionInfo.g_arrPin1 != null && m_smVisionInfo.g_arrPin1.Count > 0 && m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(0))
                m_smVisionInfo.g_arrPin1[0].ResetInspectionData();

            m_bSubTh_SideTLTest_LeadEdgeResult = false;
            m_bSubTh_SideBRTest_LeadEdgeResult = false;
            m_bSubTh_SideTLTest_LeadEdgeDone = false;
            m_bSubTh_SideBRTest_LeadEdgeDone = false;
            m_bSubTh_SideTest_BuildSideLeadFail = false;
            m_eInspectionResult_Center = ResulType.Pass;

            TrackTiming(false, "1", false, m_smVisionInfo.g_blnTrackBasic);

            //#if (DEBUG)

            if (!m_smCustomizeInfo.g_blnWantUseTCPIPIO && m_objVisionIO.CheckPH != null)
            {
                if (m_smVisionInfo.g_blnWantCheckPH && !blnAuto && m_smVisionInfo.g_blnCheckPH)                                     // offline test     : Adv PH true and offline form PH true.
                {
                    m_bWantPHTest = true;
                    m_bSubTh_PHTest = true;
                }
                else if (m_objVisionIO.CheckPH.IsOn(m_smVisionInfo.g_blnCheckPH) && m_smVisionInfo.g_blnWantCheckPH && blnAuto)     // prod test        : IO PH true and Adv PH true
                {
                    m_bWantPHTest = true;
                    m_bSubTh_PHTest = true;
                }
                else
                {
                    m_bWantPHTest = false;
                    m_bSubTh_PHTest_Result = true;
                }
            }
            else if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
            {
                if (m_smVisionInfo.g_blnWantCheckPH && !blnAuto && m_smVisionInfo.g_blnCheckPH)                                     // offline test     : Adv PH true and offline form PH true.
                {
                    m_bWantPHTest = true;
                    m_bSubTh_PHTest = true;
                }
                else if (m_blnCheckPH_In && m_smVisionInfo.g_blnWantCheckPH && blnAuto)     // prod test        : IO PH true and Adv PH true
                {
                    m_bWantPHTest = true;
                    m_bSubTh_PHTest = true;
                }
                else
                {
                    m_bWantPHTest = false;
                    m_bSubTh_PHTest_Result = true;
                }
            }
            else
            {
                m_bWantPHTest = false;
                m_bSubTh_PHTest_Result = true;
            }

            if (!m_bWantPHTest)
            {
                m_bSubTh_CenterTest = true;
                m_bSubTh_SideTLTest = true;
                m_bSubTh_SideBRTest = true;
            }
            else
            {
                m_bSubTh_CenterTest_Result = true;
                m_bSubTh_SideTLTest_Result = true;
                m_bSubTh_SideBRTest_Result = true;

            }
            //#else

            //            if (m_objVisionIO.CheckPH != null)
            //            {
            //                if (m_smVisionInfo.g_blnWantCheckPH && !blnAuto && m_smVisionInfo.g_blnCheckPH)
            //                {
            //                    m_bSubTh_PHTest = true;
            //                }
            //                else if (m_objVisionIO.CheckPH.IsOn() && m_smVisionInfo.g_blnWantCheckPH && blnAuto)
            //                {
            //                    m_bSubTh_PHTest = true;
            //                }
            //                else
            //                {
            //                    m_bSubTh_PHTest_Result = true;
            //                }
            //            }
            //            else
            //            {
            //                m_bSubTh_PHTest_Result = true;
            //            }

            //            if (m_objVisionIO.CheckPH != null)
            //            {
            //                if ((m_smVisionInfo.g_blnWantCheckPH && !m_smVisionInfo.g_blnCheckPH && !blnAuto) ||
            //                (!m_smVisionInfo.g_blnWantCheckPH && !blnAuto) ||
            //                (blnAuto && m_objVisionIO.CheckPH.IsOff(true)) ||
            //                (blnAuto && m_objVisionIO.CheckPH.IsOn() && !m_smVisionInfo.g_blnWantCheckPH))
            //                {
            //                    m_bSubTh_CenterTest = true;
            //                    m_bSubTh_SideTLTest = true;
            //                    m_bSubTh_SideBRTest = true;
            //                }
            //                else
            //                {
            //                   m_bSubTh_CenterTest_Result = true;
            //                    m_bSubTh_SideTLTest_Result = true;
            //                    m_bSubTh_SideBRTest_Result = true;
            //                }
            //            }
            //            else
            //            {
            //                m_bSubTh_CenterTest = true;
            //                m_bSubTh_SideTLTest = true;
            //                m_bSubTh_SideBRTest = true;
            //            }

            //            //////m_bSubTh_CenterTest = true;
            //            //////m_bSubTh_SideTLTest = true;
            //            //////m_bSubTh_SideBRTest = true;
            //#endif
            TrackTiming(false, "2", false, m_smVisionInfo.g_blnTrackBasic);

            // Wait inspection done.
            bool blnInspectionDone = false;
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            while (true)
            {
                if (!m_bSubTh_CenterTest && !m_bSubTh_SideTLTest && !m_bSubTh_SideBRTest && !m_bSubTh_PHTest)
                {
                    blnInspectionDone = true;
                    break;
                }

#if (RELEASE || RTXRelease || Release_2_12)
                if (timeout.Timing > 10000) // 2020 02 18 - CCENG: Change to 10ms to prevent inspection time out due to debug run too fast.
                    {
                        STTrackLog.WriteLine(">>>>>>>>>>>>> Vision 3 Lead - time out 7");
                        break;
                    }
#endif
                Thread.Sleep(1);
            }

            TrackTiming(false, "3", false, m_smVisionInfo.g_blnTrackBasic);

            // 2020 07 17 - Lead package thickenss not ready yet.
            // 2019 06 01 - CCENG: check average thickness here. If center fail, then not need to check 
            bool bThickness_Result = true;
            //if (m_bSubTh_CenterTest_Result)
            //{
            //    // 2019 05 31-CCENG: if not all m_arr5SFoundUnitPkg pass, this mean find package size already fail, so not need to check thickness here.
            //    if (m_arr5SFoundUnitPkg[1] && m_arr5SFoundUnitPkg[2] && m_arr5SFoundUnitPkg[3] && m_arr5SFoundUnitPkg[4])
            //    {
            //        if (!IsPackageThicknessOK_MultiThreading())
            //        {
            //            bThickness_Result = false;
            //        }
            //    }
            //}

            // ---------- Set IO to handler (set before save image for faster UPH) -------------------------------
            if (blnInspectionDone && m_bSubTh_CenterTest_Result && m_bSubTh_SideTLTest_Result && m_bSubTh_SideBRTest_Result && m_bSubTh_PHTest_Result && bThickness_Result)
            {
                // Wait all images grab done to make sure latest images are saved.
                WaitAllImageGrabDone();
                if (blnAuto)
                {
                    if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                    {
                        WaitEventDone(ref m_blnGrabbing_Out, false, 10000, "m_blnGrabbing_Out");//2021-03-24 ZJYEOH : Wait grab done send to handler first

                        //2021-03-24 ZJYEOH : Wait at least 5ms before send result
                        float fDelay = 5 - Math.Max(0, m_smVisionInfo.g_objTotalTime.Timing - m_smVisionInfo.g_objGrabTime.Duration);
                        if (fDelay >= 1)
                            Thread.Sleep((int)fDelay);

                        m_blnPositionReject_Out = false;
                        m_blnPass1_Out = true;
                        if (m_blnCheckOffset_In)
                            m_smTCPIPIO.Send_ResultForCheckOffset(m_smVisionInfo.g_intVisionIndex, true, m_blnOrientResult1_Out, m_blnOrientResult2_Out, m_fOffsetX, m_fOffsetY, m_fOffsetAngle, true);
                        else
                            m_smTCPIPIO.Send_Result(m_smVisionInfo.g_intVisionIndex, true, m_blnOrientResult1_Out, m_blnOrientResult2_Out, m_intTCPIPResultID);

                        //m_blnGrabbing_Out = false;
                        m_blnEndVision_Out = true;
                    }
                    else
                    {
                        WaitEventDone(ref m_blnGrabbing_Out, false, 10000, "m_blnGrabbing_Out");
                        float fGrabDelay = 5 - m_smVisionInfo.g_objGrabDoneTime.Timing;
                        if (fGrabDelay >= 1)
                            Thread.Sleep((int)fGrabDelay);

                        if (m_objVisionIO.PositionReject != null)
                        {
                            if (m_objVisionIO.PositionReject.IsOff())
                                m_objVisionIO.IOPass1.SetOn("V3 IOPass1 18");
                        }
                        else
                            m_objVisionIO.IOPass1.SetOn("V3 IOPass1 19");

                        m_objVisionIO.IOGrabbing.SetOff("V3 IOGrabbing 62");
                        m_objVisionIO.IOEndVision.SetOn("5");
                    }
                    m_blnGrabbing_Out = false;
                }
            }
            else
            {
                if (m_smVisionInfo.g_blnStopAfterFail)
                {
                    m_smVisionInfo.g_intMachineStatus = 1;

                }
                else
                {
                    if (blnAuto)
                    {
                        if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                        {
                            WaitEventDone(ref m_blnGrabbing_Out, false, 10000, "m_blnGrabbing_Out");//2021-03-24 ZJYEOH : Wait grab done send to handler first

                            //2021-03-24 ZJYEOH : Wait at least 5ms before send result
                            float fDelay = 5 - Math.Max(0, m_smVisionInfo.g_objTotalTime.Timing - m_smVisionInfo.g_objGrabTime.Duration);
                            if (fDelay >= 1)
                                Thread.Sleep((int)fDelay);

                            m_blnGrabbing_Out = false;
                            m_blnEndVision_Out = true;
                            if (m_blnCheckOffset_In)
                                m_smTCPIPIO.Send_ResultForCheckOffset(m_smVisionInfo.g_intVisionIndex, false, m_blnOrientResult1_Out, m_blnOrientResult2_Out, m_fOffsetX, m_fOffsetY, m_fOffsetAngle, false);
                            else
                                m_smTCPIPIO.Send_Result(m_smVisionInfo.g_intVisionIndex, false, m_blnOrientResult1_Out, m_blnOrientResult2_Out, m_intTCPIPResultID);
                        }
                        else
                        {
                            m_objVisionIO.IOGrabbing.SetOff("V3 IOGrabbing 61");
                            m_objVisionIO.IOEndVision.SetOn("6");
                        }
                    }
                }
            }

            TrackTiming(false, "4", false, m_smVisionInfo.g_blnTrackBasic);
            // ---------------------------------------------------------------------------------------------------

            // Wait all images grab done to make sure latest images are saved.
            WaitAllImageGrabDone();
            m_smVisionInfo.g_blnGrabbing = false;

            TrackTiming(false, "5", false, m_smVisionInfo.g_blnTrackBasic);

            string strPreMessage;
            if (blnAuto)
                strPreMessage = "";
            else
                strPreMessage = "Offline Test: ";

            // ---------- Check pass fail result --------------------------------------------------------
            if (blnInspectionDone)
            {
                STTrackLog.WriteLine("Inspect A");
                if (!m_bGrabImageFinalResult)
                {
                    STTrackLog.WriteLine("Inspect B");
                    m_smVisionInfo.g_strErrorMessage = strPreMessage + m_smVisionInfo.g_strErrorMessage;
                    AddInspectionCounter(ResulType.NotReady, blnAuto);
                }
                else if (!m_bSubTh_CenterTest_Result)
                {
                    STTrackLog.WriteLine("Inspect C");
                    m_smVisionInfo.g_strErrorMessage = strPreMessage + m_arrErrorMessage[0];
                    AddInspectionCounter(m_eInspectionResult_Center, blnAuto);

                    if (m_smVisionInfo.g_objGRR.ref_intGRRMode == 1)//2019-12-02 ZJYEOH : Record GRR if selected dynamic
                        RecordGRR2();

                    //Record CPK here
                    if (blnAuto && m_eInspectionResult_Center != ResulType.FailPosition)
                        RecordCPK();
                }
                //else if (!bThickness_Result)  // 2020 07 17 - CCENG: Lead 3D Thickness not ready yet
                //{
                //    m_smVisionInfo.g_strErrorMessage = strPreMessage + m_arrErrorMessage[1];    // Thickness error msg is been keeping in index 1 during thickness checking in function IsPackageThicknessOK_MultiThreading()
                //    AddInspectionCounter(ResulType.FailSidePkgDimension, blnAuto);
                //}
                else if (!m_bSubTh_SideTLTest_Result)
                {
                    STTrackLog.WriteLine("Inspect D");
                    if (m_arrErrorMessage[1].Length > 0)
                        m_smVisionInfo.g_strErrorMessage = strPreMessage + m_arrErrorMessage[1];
                    else
                        m_smVisionInfo.g_strErrorMessage = strPreMessage + m_arrErrorMessage[4];

                    AddInspectionCounter(m_eInspectionResult_SideTL, blnAuto);

                    if (m_smVisionInfo.g_objGRR.ref_intGRRMode == 1)//2019-12-02 ZJYEOH : Record GRR if selected dynamic
                        RecordGRR2();

                    //Record CPK here
                    if (blnAuto && m_eInspectionResult_SideTL != ResulType.FailPosition)
                        RecordCPK();
                }
                else if (!m_bSubTh_SideBRTest_Result)
                {
                    STTrackLog.WriteLine("Inspect E");
                    if (m_arrErrorMessage[2].Length > 0)
                        m_smVisionInfo.g_strErrorMessage = strPreMessage + m_arrErrorMessage[2];
                    else
                        m_smVisionInfo.g_strErrorMessage = strPreMessage + m_arrErrorMessage[3];

                    AddInspectionCounter(m_eInspectionResult_SideBR, blnAuto);

                    if (m_smVisionInfo.g_objGRR.ref_intGRRMode == 1)//2019-12-02 ZJYEOH : Record GRR if selected dynamic
                        RecordGRR2();

                    //Record CPK here
                    if (blnAuto && m_eInspectionResult_SideBR != ResulType.FailPosition)
                        RecordCPK();
                }
                else if (!m_bSubTh_PHTest_Result)
                {
                    STTrackLog.WriteLine("Inspect F");
                    m_smVisionInfo.g_strErrorMessage = strPreMessage + m_arrErrorMessage[0];

                    AddInspectionCounter(m_eInspectionResult_Center, blnAuto);
                }
                else
                {
                    STTrackLog.WriteLine("Inspect G");
                    if (m_bWantPHTest)
                    {
                        m_smVisionInfo.g_strErrorMessage = strPreMessage + "*PH Inspection Pass.";
                        if (m_smVisionInfo.g_intViewInspectionSetting == 0) //2020-09-22 ZJYEOH : No need set color when trigger from Pre-Inspection
                            m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                        AddInspectionCounter(ResulType.PassPH, blnAuto);
                    }
                    else
                    {
                        if (!blnAuto)
                        {
                            m_smVisionInfo.g_strErrorMessage = strPreMessage + "*Inspection Pass.";
                            if (m_smVisionInfo.g_intViewInspectionSetting == 0) //2020-09-22 ZJYEOH : No need set color when trigger from Pre-Inspection
                                m_smVisionInfo.g_cErrorMessageColor = Color.Black;
                        }
                        AddInspectionCounter(ResulType.Pass, blnAuto);

                        RecordGRR2();

                        //Record CPK here
                        if (blnAuto)
                            RecordCPK();
                    }
                }
            }
            else
            {
                m_smVisionInfo.g_strErrorMessage = strPreMessage + "Wait inspection timeout.";

                AddInspectionCounter(ResulType.Timesout, blnAuto);
            }

            if (m_smVisionInfo.g_strErrorMessage != "" && m_smVisionInfo.g_intViewInspectionSetting == 0)
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
            else
                m_smVisionInfo.g_strErrorMessage = "";

            // ---------- Send position information to handler ----------------------------
#if (RTXDebug || RTXRelease)

            //if (m_thCOMMPort != null)
            {
                if ((m_smCustomizeInfo.g_intWantPositioning & (0x01 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    RS232.RTXSendData(GetTCPPositionResult_FromPositioningObject(blnResultOK), m_smVisionInfo.g_intVisionPos);
                }
            }

#else
            if ((m_smCustomizeInfo.g_intWantPositioning & (0x01 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if (m_blnCustomWantPackage)
                {
                    if (!m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                        m_smComThread.Send(GetTCPPositionResult_RectGauge4L(blnResultOK));

                }
                else if (m_blnCustomWantPositioning)
                {
                    if (!m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                        m_smComThread.Send(GetTCPPositionResult_FromPositioningObject(blnResultOK));
                }
            }

#endif
            // ----------------------------------------------------------------------------

            // Unlock to allow other thread use the Lead3D variables e.g drawing.
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (m_smVisionInfo.g_arrLead3D[i] != null)
                {
                    if (m_smVisionInfo.g_arrLead3D[i].ref_blnInspectLock)
                    {
                        m_smVisionInfo.g_arrLead3D[i].ref_blnInspectLock = false;
                        //m_smVisionInfo.g_arrLead3D[i].m_strTrack2 += "F001,";
                    }
                }
            }

            m_smVisionInfo.g_blnInspectionInProgress = false;   // Must set before m_bSubTh_StartAfterInspect
            m_bSubTh_StartAfterInspect = true;
            m_smVisionInfo.g_blnLead3DInpected = true;

            m_smVisionInfo.g_objProcessTime.Stop();

            TrackTiming(false, "Last", m_smVisionInfo.g_blnTrackBasic || m_smVisionInfo.g_blnTrackCenter || m_smVisionInfo.g_blnTrackTL || m_smVisionInfo.g_blnTrackBR, true);
            //STTrackLog.WriteLine(m_strTrack + " === " + m_strTrack_Center + " === " + m_strTrack_TL + " === " + m_strTrack_BR);
            //STTrackLog.WriteLine(m_smVisionInfo.g_objProcessTime.Duration.ToString());
            return true;
        }

        private void AddInspectionCounter(ResulType eResultType, bool blnAuto)
        {
            STTrackLog.WriteLine("In AddInspectionCounter");
            switch (eResultType)
            {
                case ResulType.Pass:
                    {
                        m_smVisionInfo.g_strResult = "Pass";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intPassTotal++;
                            m_smVisionInfo.g_intContinuousPassUnitCount++;
                            SavePassImage_AddToBuffer();
                        }
                    }
                    break;
                case ResulType.FailLead:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("FailLead", m_smVisionInfo.g_strErrorMessage);
                            //SaveRejectImage("FailPad");
                        }
                    }
                    break;
                case ResulType.FailPackage:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intPackageFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("FailPackage", m_smVisionInfo.g_strErrorMessage);
                            //SaveRejectImage("FailPackage");
                        }
                    }
                    break;
                case ResulType.FailPin1:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intPin1FailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("FailPin1", m_smVisionInfo.g_strErrorMessage);
                            //SaveRejectImage("FailPosition");
                        }
                    }
                    break;
                case ResulType.FailPosition:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intPositionFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("FailPosition", m_smVisionInfo.g_strErrorMessage);
                            //SaveRejectImage("FailPosition");
                        }
                    }
                    break;
                case ResulType.Timesout:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("Timesout", m_smVisionInfo.g_strErrorMessage);
                            //SaveRejectImage("Timesout");
                        }
                    }
                    break;
                default:
                case ResulType.NotReady:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("Other", m_smVisionInfo.g_strErrorMessage);
                            //SaveRejectImage("Other");
                        }
                    }
                    break;
                case ResulType.FailPH:
                    {
                        m_smVisionInfo.g_strResult = "Fail PH";
                        if (blnAuto)
                        {
                            //m_smVisionInfo.g_intPadFailureTotal++;    // 2019 Dun add counter for PH Fail
                            SaveRejectImage_AddToBuffer("FailPH", m_smVisionInfo.g_strErrorMessage);
                            //SaveRejectImage("Timesout");
                        }
                    }
                    break;
                case ResulType.PassPH:
                    {
                        m_smVisionInfo.g_strResult = "Pass PH";
                        if (blnAuto)
                        {
                            // 2019 09 11 - CCENG: Save Pass PH using SaveRejectImage_AddToBuffer() because need to have different folder name. 
                            //                     Although It is Pass image, but tHe Pass PH save count will refer to FailImageCount also for easier coding. 
                            SaveRejectImage_AddToBuffer("PassPH", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadOffset:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadOffsetFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadOffset", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadSkew:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadSkewFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadSkew", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadWidth:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadWidthFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadWidth", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadLength:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadLengthFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadLength", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadLengthVariance:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadLengthVarianceFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadLengthVariance", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadPitchGap:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadPitchGapFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadPitchGap", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadPitchVariance:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadPitchVarianceFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadPitchVariance", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadStandOff:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadStandOffFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadStandOff", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadStandOffVariance:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadStandOffVarianceFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadStandOffVariance", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadCoplan:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadCoplanFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadCoplan", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadAGV:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadAGVFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadAGV", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadSpan:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadSpanFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadSpan", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadSweeps:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadSweepsFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadSweeps", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadUnCutTiebar:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadUnCutTiebarFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadUnCutTiebar", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadMissing:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadMissingFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadMissing", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadContamination:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadContaminationFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadContamination", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadPkgDefect:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadPkgDefectFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadPkgDefect", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailLeadPkgDimension:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intLeadPkgDimensionFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("LeadPkgDimension", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
                case ResulType.FailEdgeNotFound:
                    {
                        m_smVisionInfo.g_strResult = "Fail";
                        if (blnAuto)
                        {
                            m_smVisionInfo.g_intEdgeNotFoundFailureTotal++;
                            m_smVisionInfo.g_intContinuousFailUnitCount++;
                            SaveRejectImage_AddToBuffer("EdgeNotFound", m_smVisionInfo.g_strErrorMessage);
                        }
                    }
                    break;
            }

            if (blnAuto)
            {
                if (eResultType != ResulType.FailPH && eResultType != ResulType.PassPH)
                {
                    m_smVisionInfo.g_intTestedTotal++;
                    m_smVisionInfo.g_intLowYieldUnitCount++;
                    CheckLowYield();
                    CheckContinuousPass();
                    CheckContinuousFail();
                }
            }
        }

        private void WaitAllImageGrabDone()
        {
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            while (true)
            {
                switch (m_intGrabRequire)
                {
                    case 1:
                        if (m_bGrabImage1Done)
                            return;
                        break;
                    case 2:
                        if (m_bGrabImage2Done)
                            return;
                        break;
                    case 3:
                        if (m_bGrabImage3Done)
                            return;
                        break;
                    case 4:
                        if (m_bGrabImage4Done)
                            return;
                        break;
                    case 5:
                        if (m_bGrabImage5Done)
                            return;
                        break;
                    case 6:
                        if (m_bGrabImage6Done)
                            return;
                        break;
                    case 7:
                        if (m_bGrabImage7Done)
                            return;
                        break;
                }

                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out WaitAllImageGrabDone - " + "Wait Grabing 10000");
                    STTrackLog.WriteLine("Waiting event timeout");

                    break;
                }

                Thread.Sleep(1);
            }
            timeout.Stop();

        }

        private string GetTCPPositionResult_FromPositioningObject(bool blnResult)
        {
            string strMessage = "<P";

            if (!blnResult)
                strMessage += "FN0000N0000";
            else
            {
                strMessage += "P";

                if (m_smVisionInfo.g_objPositioning.ref_fObjectOffsetX < 0)
                    strMessage += "N";
                else
                    strMessage += "P";
                strMessage += string.Format("{0:0000}", Math.Abs(Math.Round(m_smVisionInfo.g_objPositioning.ref_fObjectOffsetX)));

                if (m_smVisionInfo.g_intForceYZero == 1)
                {
                    strMessage += "P0000";
                }
                else
                {
                    if (m_smVisionInfo.g_objPositioning.ref_fObjectOffsetY < 0)
                        strMessage += "N";
                    else
                        strMessage += "P";
                    strMessage += string.Format("{0:0000}", Math.Abs(Math.Round(m_smVisionInfo.g_objPositioning.ref_fObjectOffsetY)));

                }
            }

            strMessage += ">";

            return strMessage;
        }

        private string GetTCPPositionResult_RectGauge4L(bool blnResult)
        {
            string strMessage = "<P";

            if (!blnResult)
                strMessage += "FN0000N0000";
            else
            {
                PointF pResultCenterPoint = m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L();
                float fObjectOffSetX = ((pResultCenterPoint.X - m_smVisionInfo.g_arrPad[0].ref_objPosCrosshair.ref_intCrosshairX) / m_smVisionInfo.g_fCalibPixelX * 1000);
                float fObjectOffSetY = ((pResultCenterPoint.Y - m_smVisionInfo.g_arrPad[0].ref_objPosCrosshair.ref_intCrosshairY) / m_smVisionInfo.g_fCalibPixelY * 1000);

                strMessage += "P";

                if (fObjectOffSetX < 0)
                    strMessage += "N";
                else
                    strMessage += "P";
                strMessage += string.Format("{0:0000}", Math.Abs(Math.Round(fObjectOffSetX)));

                if (m_smVisionInfo.g_intForceYZero == 1)
                {
                    strMessage += "P0000";
                }
                else
                {
                    if (fObjectOffSetY < 0)
                        strMessage += "N";
                    else
                        strMessage += "P";
                    strMessage += string.Format("{0:0000}", Math.Abs(Math.Round(fObjectOffSetY)));

                }
            }

            strMessage += ">";

            return strMessage;
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

                            if ((intTestOption & 0x20) > 0)
                                m_blnCheckPH_In = true;
                            else
                                m_blnCheckPH_In = false;

                            if ((intTestOption & 0x40) > 0)
                                m_blnCheckOffset_In = true;
                            else
                                m_blnCheckOffset_In = false;

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
        private string GetROIDefinition(int intPadIndex)
        {
            switch (intPadIndex)
            {
                case 0:
                    return "*Center ROI: ";
                case 1:
                    return "*Top ROI: ";
                case 2:
                    return "*Right ROI: ";
                case 3:
                    return "*Bottom ROI: ";
                case 4:
                    return "*Left ROI: ";
                default:
                    SRMMessageBox.Show("GetROIDefinition()->Lead Index " + intPadIndex.ToString() + " no exist.");
                    return "";
            }
        }

        private void RotateImagesTo0Degree_MultiThreading(int intLeadIndex, int intImageIndex)
        {
            if (intLeadIndex == 0)
            {
            }
            if (m_arr5SImageRotated2[intLeadIndex][intImageIndex])
                return;

            int intSearchROICenterX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalCenterX;
            int intSearchROICenterY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalCenterY;
            int intSearchROIStartX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionX;
            int intSearchROIStartY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionY;
            int intSearchROIEndX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionX + m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIWidth;
            int intSearchROIEndY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionY + m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIHeight;
            float fUnitCenterPointX = 0;
            float fUnitCenterPointY = 0;
            float fUnitAngle = 0;
            float fRotateROIHalfWidth;
            float fRotateROIHalfHeight;


            if (m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_blnMeasureCenterPkgSizeUsingCorner)
            {
                fUnitCenterPointX = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.X;
                fUnitCenterPointY = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.Y;
                if (m_smVisionInfo.g_arrLead3D[intLeadIndex].IsUnitPRPass())
                {
                    fUnitAngle = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetUnitPRResultAngle();
                }
                else
                    fUnitAngle = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fCenterUnitAngle;
            }
            else
            {
                fUnitCenterPointX = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().X;
                fUnitCenterPointY = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().Y;
                fUnitAngle = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultAngle_RectGauge4L();
            }

            // Get result center point and angle
            //if (m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_blnWantGaugeMeasurePkgSize)
            //{
                //fUnitCenterPointX = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().X;
                //fUnitCenterPointY = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().Y;
                //fUnitAngle = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultAngle_RectGauge4L();
            //}
            //else if (m_blnCustomWantPositioning)
            //{
            //    fUnitCenterPointX = m_smVisionInfo.g_objPositioning.ref_fObjectCenter.X;
            //    fUnitCenterPointY = m_smVisionInfo.g_objPositioning.ref_fObjectCenter.Y;
            //    fUnitAngle = m_smVisionInfo.g_objPositioning.ref_fObjectAngle;
            //}
            //else if (m_blnCustomWantLead3D)
            //{
            //    // Orient PRS Center Point Result
            //    fUnitCenterPointX = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_UnitMatcher().X;
            //    fUnitCenterPointY = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_UnitMatcher().Y;
            //    fUnitAngle = m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultAngle_UnitMatcher();
            //}

            //if (intImageIndex == 0)
            //{
            //    m_arrUnitCenterX[intLeadIndex] = fUnitCenterPointX;
            //    m_arrUnitCenterY[intLeadIndex] = fUnitCenterPointY;
            //}
            //else if (intImageIndex == 2)
            //{
            //    fUnitCenterPointX = m_arrUnitCenterX[intLeadIndex];
            //    fUnitCenterPointY = m_arrUnitCenterY[intLeadIndex];
            //}

            // Error Checking
            if (fUnitCenterPointX == 0 || fUnitCenterPointY == 0)
            {
                SRMMessageBox.Show("RotateImageTo0Degree() -> Unit Center Point should not 0.");
                return;
            }

            // Define Rotate ROI size (between Unit size and Search ROI Size)
            if (fUnitCenterPointX <= intSearchROICenterX)
            {
                fRotateROIHalfWidth = fUnitCenterPointX - intSearchROIStartX;
            }
            else
            {
                fRotateROIHalfWidth = intSearchROIEndX - fUnitCenterPointX;
            }

            if (fUnitCenterPointY <= intSearchROICenterY)
            {
                fRotateROIHalfHeight = fUnitCenterPointY - intSearchROIStartY;
            }
            else
            {
                fRotateROIHalfHeight = intSearchROIEndY - fUnitCenterPointY;
            }

            // Get rotate roi which center ROI point same as result position unit ROI
            m_arrRotatedROI[intLeadIndex].AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
            m_arrRotatedROI[intLeadIndex].LoadROISetting((int)Math.Round(fUnitCenterPointX - fRotateROIHalfWidth, 0, MidpointRounding.AwayFromZero),
                                            (int)Math.Round(fUnitCenterPointY - fRotateROIHalfHeight, 0, MidpointRounding.AwayFromZero),
                                            (int)Math.Round(fRotateROIHalfWidth * 2, 0, MidpointRounding.AwayFromZero),
                                            (int)Math.Round(fRotateROIHalfHeight * 2, 0, MidpointRounding.AwayFromZero));

            // Rotate image 1 to 0 degree
            if (Math.Abs(fUnitAngle) > 0.5)
            {
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[intImageIndex], m_arrRotatedROI[intLeadIndex], fUnitAngle, 4, ref m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex], intImageIndex);
            }
            else
            {
                m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex]);
            }

            m_blnRotateImageUpdated = true;
            m_arr5SImageRotated2[intLeadIndex][intImageIndex] = true;
        }

        private void RotateImagesTo0Degree_MultiThreading_Lead3D(int intLeadIndex, int intImageIndex)
        {
            if (m_arr5SImageRotated2[intLeadIndex][intImageIndex])
                return;

            int intSearchROICenterX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalCenterX;
            int intSearchROICenterY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalCenterY;
            int intSearchROIStartX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionX;
            int intSearchROIStartY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionY;
            int intSearchROIEndX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionX + m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIWidth;
            int intSearchROIEndY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionY + m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIHeight;
            float fUnitCenterPointX = 0;
            float fUnitCenterPointY = 0;
            float fUnitAngle = 0;
            float fRotateROIHalfWidth;
            float fRotateROIHalfHeight;

            // Get result center point and angle
            fUnitCenterPointX = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.X;
            fUnitCenterPointY = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.Y;
            fUnitAngle = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fCenterUnitAngle; // ref_fBaseLineAngle;

            // Error Checking
            if (fUnitCenterPointX == 0 || fUnitCenterPointY == 0)
            {
                SRMMessageBox.Show("RotateImageTo0Degree() -> Unit Center Point should not 0.");
                return;
            }

            // Define Rotate ROI size (between Unit size and Search ROI Size)
            if (fUnitCenterPointX <= intSearchROICenterX)
            {
                fRotateROIHalfWidth = fUnitCenterPointX - intSearchROIStartX;
            }
            else
            {
                fRotateROIHalfWidth = intSearchROIEndX - fUnitCenterPointX;
            }

            if (fUnitCenterPointY <= intSearchROICenterY)
            {
                fRotateROIHalfHeight = fUnitCenterPointY - intSearchROIStartY;
            }
            else
            {
                fRotateROIHalfHeight = intSearchROIEndY - fUnitCenterPointY;
            }

            // Get rotate roi which center ROI point same as result position unit ROI
            m_arrRotatedROI[intLeadIndex].AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
            m_arrRotatedROI[intLeadIndex].LoadROISetting((int)Math.Round(fUnitCenterPointX - fRotateROIHalfWidth, 0, MidpointRounding.AwayFromZero),
                                            (int)Math.Round(fUnitCenterPointY - fRotateROIHalfHeight, 0, MidpointRounding.AwayFromZero),
                                            (int)Math.Round(fRotateROIHalfWidth * 2, 0, MidpointRounding.AwayFromZero),
                                            (int)Math.Round(fRotateROIHalfHeight * 2, 0, MidpointRounding.AwayFromZero));


            // Rotate image 1 to 0 degree
            if (Math.Abs(fUnitAngle) > 0.5)
            {
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[intImageIndex], m_arrRotatedROI[intLeadIndex], fUnitAngle, ref m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex], intImageIndex);
            }
            else
            {
                m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex]);
            }

            m_blnRotateImageUpdated = true;
            m_arr5SImageRotated2[intLeadIndex][intImageIndex] = true;
        }
        private void RotateImagesTo0Degree_MultiThreading_Lead3D_UsingCenterROI(int intLeadIndex, int intImageIndex)
        {
            if (m_arr5SImageRotated2[intLeadIndex][intImageIndex])
                return;

            int intSearchROICenterX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalCenterX;
            int intSearchROICenterY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalCenterY;
            int intSearchROIStartX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionX;
            int intSearchROIStartY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionY;
            int intSearchROIEndX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionX + m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIWidth;
            int intSearchROIEndY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionY + m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIHeight;
            float fUnitCenterPointX = 0;
            float fUnitCenterPointY = 0;
            float fUnitAngle = 0;
            float fRotateROIHalfWidth;
            float fRotateROIHalfHeight;

            //// Get result center point and angle
            //fUnitCenterPointX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalX + m_smVisionInfo.g_arrLead3D[0].GetUnitPRResultCenterX();
            //fUnitCenterPointY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalY + m_smVisionInfo.g_arrLead3D[0].GetUnitPRResultCenterY();
            //fUnitAngle = m_smVisionInfo.g_arrLead3D[0].GetUnitPRResultAngle();

            fUnitCenterPointX = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X;
            fUnitCenterPointY = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y;
            if (m_smVisionInfo.g_arrLead3D[0].ref_intImageRotateOption == 0)
            {
                fUnitCenterPointX = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X;
                fUnitCenterPointY = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y;
                fUnitAngle = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitAngle;
            }
            else
            {
                fUnitCenterPointX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalX + m_smVisionInfo.g_arrLead3D[0].GetUnitPRResultCenterX();
                fUnitCenterPointY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalY + m_smVisionInfo.g_arrLead3D[0].GetUnitPRResultCenterY();
                fUnitAngle = m_smVisionInfo.g_arrLead3D[0].GetUnitPRResultAngle();
            }

            float fAngleResult = -fUnitAngle;//-m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitAngle
            float CenterX = 0;
            float CenterY = 0;
            float fXAfterRotated = 0;
            float fYAfterRotated = 0;

            CenterX = m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX + (float)(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth) / 2;
            CenterY = m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY + (float)(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight) / 2;
            fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - CenterX) * Math.Cos(fAngleResult * Math.PI / 180)) -
                               ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - CenterY) * Math.Sin(fAngleResult * Math.PI / 180)));
            fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - CenterX) * Math.Sin(fAngleResult * Math.PI / 180)) +
                                ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - CenterY) * Math.Cos(fAngleResult * Math.PI / 180)));

            m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center = new PointF(fXAfterRotated, fYAfterRotated);

            //// Error Checking
            //if (fUnitCenterPointX == 0 || fUnitCenterPointY == 0)
            //{
            //    SRMMessageBox.Show("RotateImageTo0Degree() -> Unit Center Point should not 0.");
            //    return;
            //}

            //// Define Rotate ROI size (between Unit size and Search ROI Size)
            //if (fUnitCenterPointX <= intSearchROICenterX)
            //{
            //    fRotateROIHalfWidth = fUnitCenterPointX - intSearchROIStartX;
            //}
            //else
            //{
            //    fRotateROIHalfWidth = intSearchROIEndX - fUnitCenterPointX;
            //}

            //if (fUnitCenterPointY <= intSearchROICenterY)
            //{
            //    fRotateROIHalfHeight = fUnitCenterPointY - intSearchROIStartY;
            //}
            //else
            //{
            //    fRotateROIHalfHeight = intSearchROIEndY - fUnitCenterPointY;
            //}

            // Get rotate roi which center ROI point same as result position unit ROI
            m_arrRotatedROI[intLeadIndex].AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
            m_arrRotatedROI[intLeadIndex].LoadROISetting(m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionX,
                                           m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIPositionY,
                                           m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth,
                                           m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight);

            // Rotate image 1 to 0 degree
            if (Math.Abs(fUnitAngle) > 0.5)
            {
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[intImageIndex], m_arrRotatedROI[intLeadIndex], fUnitAngle, ref m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex], intImageIndex);
            }
            else
            {
                m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex]);
            }

            m_blnRotateImageUpdated = true;
            m_arr5SImageRotated2[intLeadIndex][intImageIndex] = true;
        }
        private void RotateImagesTo0Degree_MultiThreading_Lead3D_PatternMatch(int intLeadIndex, int intImageIndex)
        {
            if (m_arr5SImageRotated2[intLeadIndex][intImageIndex])
                return;

            int intSearchROICenterX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalCenterX;
            int intSearchROICenterY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalCenterY;
            int intSearchROIStartX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionX;
            int intSearchROIStartY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionY;
            int intSearchROIEndX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionX + m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIWidth;
            int intSearchROIEndY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIPositionY + m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROIHeight;
            float fUnitCenterPointX = 0;
            float fUnitCenterPointY = 0;
            float fUnitAngle = 0;
            float fRotateROIHalfWidth;
            float fRotateROIHalfHeight;

            //// Get result center point and angle
            //fUnitCenterPointX = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalX + m_smVisionInfo.g_arrLead3D[0].GetUnitPRResultCenterX();
            //fUnitCenterPointY = m_smVisionInfo.g_arrLeadROIs[intLeadIndex][0].ref_ROITotalY + m_smVisionInfo.g_arrLead3D[0].GetUnitPRResultCenterY();
            //fUnitAngle = m_smVisionInfo.g_arrLead3D[0].GetUnitPRResultAngle();

            fUnitCenterPointX = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X;
            fUnitCenterPointY = m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y;
            fUnitAngle = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitAngle;
            
            // Error Checking
            if (fUnitCenterPointX == 0 || fUnitCenterPointY == 0)
            {
                SRMMessageBox.Show("RotateImageTo0Degree() -> Unit Center Point should not 0.");
                return;
            }

            // Define Rotate ROI size (between Unit size and Search ROI Size)
            if (fUnitCenterPointX <= intSearchROICenterX)
            {
                fRotateROIHalfWidth = fUnitCenterPointX - intSearchROIStartX;
            }
            else
            {
                fRotateROIHalfWidth = intSearchROIEndX - fUnitCenterPointX;
            }

            if (fUnitCenterPointY <= intSearchROICenterY)
            {
                fRotateROIHalfHeight = fUnitCenterPointY - intSearchROIStartY;
            }
            else
            {
                fRotateROIHalfHeight = intSearchROIEndY - fUnitCenterPointY;
            }

            // Get rotate roi which center ROI point same as result position unit ROI
            m_arrRotatedROI[intLeadIndex].AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
            m_arrRotatedROI[intLeadIndex].LoadROISetting((int)Math.Round(fUnitCenterPointX - fRotateROIHalfWidth, 0, MidpointRounding.AwayFromZero),
                                            (int)Math.Round(fUnitCenterPointY - fRotateROIHalfHeight, 0, MidpointRounding.AwayFromZero),
                                            (int)Math.Round(fRotateROIHalfWidth * 2, 0, MidpointRounding.AwayFromZero),
                                            (int)Math.Round(fRotateROIHalfHeight * 2, 0, MidpointRounding.AwayFromZero));
            
            // Rotate image 1 to 0 degree
            if (Math.Abs(fUnitAngle) > 0.5)
            {
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[intImageIndex], m_arrRotatedROI[intLeadIndex], fUnitAngle, ref m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex], intImageIndex);
            }
            else
            {
                m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex]);
            }

            m_blnRotateImageUpdated = true;
            m_arr5SImageRotated2[intLeadIndex][intImageIndex] = true;
        }

        private bool FindUnit_ForPackage_Multithreading(int intLeadIndex, int intImageIndex, ref int intFailType)
        {
            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                return true;

            if (((m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_intFailPkgOptionMask & 0x01) == 0) && !m_smVisionInfo.g_arrLead3D[0].GetWantInspectPackage())
                return true;


            switch (m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_objRectGauge4L.GetGaugeImageMode(0))
            {
                default:
                case 0: // Standard Mode = Image + gain only
                    {
                        if (m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fImageGain == 1)
                        {
                            // Use RectGauge4L to find unit size
                            if (!m_smVisionInfo.g_arrLead3D[intLeadIndex].MeasureEdge_ResetGaugePlaceUsingPRPositionBeforeMeasure(m_smVisionInfo.g_arrImages[intImageIndex], m_smVisionInfo.g_objWhiteImage))
                            {
                                m_arrErrorMessage[intLeadIndex] = "*Fail to find unit." + m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_strErrorMessage;
                                intFailType = 1;
                                return false;
                            }
                        }
                        else
                        {
                            // add gain to image
                            m_arrGainImage[intLeadIndex].SetImageSize(m_smVisionInfo.g_arrImages[intImageIndex].ref_intImageWidth,
                                                        m_smVisionInfo.g_arrImages[intImageIndex].ref_intImageHeight);
                            m_smVisionInfo.g_arrImages[intImageIndex].AddGain(ref m_arrGainImage[intLeadIndex], m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fImageGain);

                            // Use RectGauge4L to find unit size
                            if (!m_smVisionInfo.g_arrLead3D[intLeadIndex].MeasureEdge_ResetGaugePlaceUsingPRPositionBeforeMeasure(m_arrGainImage[intLeadIndex], m_smVisionInfo.g_objWhiteImage))
                            {
                                m_arrErrorMessage[intLeadIndex] = "*Fail to find unit." + m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_strErrorMessage;
                                intFailType = 1;
                                return false;
                            }
                        }
                    }
                    break;
                case 1: // Standard Mode = Image + gain only > Prewitt.
                    {
                        // add gain to image
                        if (m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fImageGain == 1)
                        {
                            m_arrTempGaugeImage[intLeadIndex].SetImageSize(m_smVisionInfo.g_arrImages[intImageIndex].ref_intImageWidth,
                                                            m_smVisionInfo.g_arrImages[intImageIndex].ref_intImageHeight);
                            m_smVisionInfo.g_arrImages[intImageIndex].AddPrewitt(ref m_arrTempGaugeImage[intLeadIndex]);

                            // Use RectGauge4L to find unit size
                            if (!m_smVisionInfo.g_arrLead3D[intLeadIndex].MeasureEdge_ResetGaugePlaceUsingPRPositionBeforeMeasure(m_arrTempGaugeImage[intLeadIndex], m_smVisionInfo.g_objWhiteImage))
                            {
                                m_arrErrorMessage[intLeadIndex] = "*Fail to find unit." + m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_strErrorMessage;
                                intFailType = 1;
                                return false;
                            }
                        }
                        else
                        {
                            m_arrGainImage[intLeadIndex].SetImageSize(m_smVisionInfo.g_arrImages[intImageIndex].ref_intImageWidth,
                                                            m_smVisionInfo.g_arrImages[intImageIndex].ref_intImageHeight);
                            m_smVisionInfo.g_arrImages[intImageIndex].AddGain(ref m_arrGainImage[intLeadIndex], m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fImageGain);

                            m_arrTempGaugeImage[intLeadIndex].SetImageSize(m_smVisionInfo.g_arrImages[intImageIndex].ref_intImageWidth,
                                                            m_smVisionInfo.g_arrImages[intImageIndex].ref_intImageHeight);
                            m_arrGainImage[intLeadIndex].AddPrewitt(ref m_arrTempGaugeImage[intLeadIndex]);

                            // Use RectGauge4L to find unit size
                            if (!m_smVisionInfo.g_arrLead3D[intLeadIndex].MeasureEdge_ResetGaugePlaceUsingPRPositionBeforeMeasure(m_arrTempGaugeImage[intLeadIndex], m_smVisionInfo.g_objWhiteImage))
                            {
                                m_arrErrorMessage[intLeadIndex] = "*Fail to find unit." + m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_strErrorMessage;
                                intFailType = 1;
                                return false;
                            }
                        }
                    }
                    break;
            }


            m_arr5SFoundUnitPkg[intLeadIndex] = true;

            return true;

        }

        private bool FindUnitEdge_ForSideLead_Multithreading(int intLeadIndex, int intImageIndex)
        {




            //{
            //    if (intPadIndex == 0)
            //    {
            //        if (!m_smVisionInfo.g_arrPad[intPadIndex].FindUnitUsingPRS(m_smVisionInfo.g_arrPadROIs[intPadIndex][0], 10, false))
            //        {
            //            m_arrErrorMessage[intPadIndex] = "*Fail to find unit. Unit Pattern Matching Fail.";
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        if (!m_smVisionInfo.g_arrPad[intPadIndex].FindUnitUsingPRS(m_smVisionInfo.g_arrPadROIs[intPadIndex][0], 2, false)) // Side pad angle is a bit only
            //        {
            //            m_arrErrorMessage[intPadIndex] = "*Fail to find unit. Unit Pattern Matching Fail.";
            //            return false;
            //        }
            //    }
            //}

            //m_arr5SFoundUnit[intPadIndex] = true;

            return true;
        }
       
        private void GetOtherImageUnitROI(int intPadIndex, ImageDrawing objAttachedImage, ref ROI objUnitROI)
        {
            // Create new Search ROI (same size and location as First Image Search ROI) for second image
            objUnitROI.AttachImage(objAttachedImage);
            objUnitROI.LoadROISetting(
                m_smVisionInfo.g_arrPadROIs[intPadIndex][0].ref_ROITotalX,
                m_smVisionInfo.g_arrPadROIs[intPadIndex][0].ref_ROITotalY,
                m_smVisionInfo.g_arrPadROIs[intPadIndex][0].ref_ROIWidth,
                m_smVisionInfo.g_arrPadROIs[intPadIndex][0].ref_ROIHeight);

            int intSearchAndUnitROIOffSetX = m_smVisionInfo.g_arrInspectROI[intPadIndex].ref_ROITotalX -
                                            m_smVisionInfo.g_arrPadROIs[intPadIndex][0].ref_ROITotalX;
            int intSearchAndUnitROIOffSetY = m_smVisionInfo.g_arrInspectROI[intPadIndex].ref_ROITotalY -
                                                        m_smVisionInfo.g_arrPadROIs[intPadIndex][0].ref_ROITotalY;

            PointF pSecondImageUnitCenterPoint = new PointF();
            m_smVisionInfo.g_arrPad[intPadIndex].FindSecondImageUnitLocation_UsingBlobs(objUnitROI,
                intSearchAndUnitROIOffSetX, intSearchAndUnitROIOffSetY, ref pSecondImageUnitCenterPoint);

            float fPkgSizeWidth = m_smVisionInfo.g_arrPadROIs[intPadIndex][2].ref_ROIWidth;
            float fPkgSizeHeight = m_smVisionInfo.g_arrPadROIs[intPadIndex][2].ref_ROIHeight;

            objUnitROI.LoadROISetting(
                (int)Math.Round(pSecondImageUnitCenterPoint.X - fPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                (int)Math.Round(pSecondImageUnitCenterPoint.Y - fPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                (int)Math.Round(fPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                (int)Math.Round(fPkgSizeHeight, 0, MidpointRounding.AwayFromZero));
        }

        private bool IsPackageOK(int intPadIndex)
        {
            if (!m_smVisionInfo.g_blnCheckPackage)
                return true;

            //// Question: Using the RectGauge4L measurement size or use the learn PackageROI size?
            //// -------------RectGauge4L-------------------------------------
            ////float fPkgSizeWidth = Math.Max(m_smVisionInfo.g_arrPad[intPadIndex].GetResultUpWidth_RectGauge4L(0),
            ////                               m_smVisionInfo.g_arrPad[intPadIndex].GetResultDownWidth_RectGauge4L(0));
            ////float fPkgSizeHeight = Math.Max(m_smVisionInfo.g_arrPad[intPadIndex].GetResultLeftHeight_RectGauge4L(0),
            ////               m_smVisionInfo.g_arrPad[intPadIndex].GetResultLeftHeight_RectGauge4L(0));
            ////--------------------------------------------------------------
            //float fPkgSizeWidth = m_smVisionInfo.g_arrPadROIs[intPadIndex][2].ref_ROIWidth;
            //float fPkgSizeHeight = m_smVisionInfo.g_arrPadROIs[intPadIndex][2].ref_ROIHeight;

            //// Get First Image Package ROI
            //m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            //m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].LoadROISetting(
            //    (int)Math.Round(m_smVisionInfo.g_arrPad[intPadIndex].GetResultCenterPoint_RectGauge4L().X -
            //     fPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
            //    (int)Math.Round(m_smVisionInfo.g_arrPad[intPadIndex].GetResultCenterPoint_RectGauge4L().Y -
            //     fPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
            //    (int)Math.Round(fPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
            //    (int)Math.Round(fPkgSizeHeight, 0, MidpointRounding.AwayFromZero));

            //// Get Second Image Package ROI
            //if (m_smVisionInfo.g_arrRotatedImages.Count > 1)
            //{
            //    bool blnWantSkipSidePadForImage2 = true;  // Coaxial light cannot view good defect in image 2 side pad, so will skip for defect checking in image 2 for 4 side pads.
            //    if (intPadIndex == 0 || (intPadIndex > 0 && !blnWantSkipSidePadForImage2))
            //    {
            //        ROI objUnitROI = m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][1];
            //        GetOtherImageUnitROI(intPadIndex, m_smVisionInfo.g_arrRotatedImages[1], ref objUnitROI);
            //    }
            //    else
            //        m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][1] = null;
            //}

            //// Get Third Image Package ROI
            //if (m_smVisionInfo.g_arrRotatedImages.Count > 2)
            //{
            //    ROI objUnitROI = m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][2];
            //    GetOtherImageUnitROI(intPadIndex, m_smVisionInfo.g_arrRotatedImages[2], ref objUnitROI);
            //}
            //else
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][2] = null;

            //if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intFailPkgOptionMask & 0x80) > 0)   // Want check mold flash
            //{
            //    if (m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3] == null)
            //        m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3] = new ROI();
            //    int intMoldFlashLength = (int)Math.Ceiling(m_smVisionInfo.g_arrPad[intPadIndex].GetMoldFlashLengthLimit(1) * Math.Max(m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY));
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            //    int intStartX = m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].ref_ROITotalX - intMoldFlashLength;
            //    int intExtendX;
            //    if (intStartX < 0)
            //    {
            //        intExtendX = m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].ref_ROITotalX;
            //        intStartX = 0;
            //    }
            //    else
            //        intExtendX = intMoldFlashLength;

            //    int intStartY = m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].ref_ROITotalY - intMoldFlashLength;
            //    int intExtendY;
            //    if (intStartY < 0)
            //    {
            //        intExtendY = m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].ref_ROITotalY;
            //        intStartY = 0;
            //    }
            //    else
            //        intExtendY = intMoldFlashLength;

            //    //m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3].LoadROISetting(
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3].LoadROISetting(
            //    intStartX, intStartY,
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].ref_ROIWidth + intExtendX * 2,
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].ref_ROIHeight + intExtendY * 2);
            //}
            //else
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3] = null;

            //if (!m_smVisionInfo.g_arrPad[intPadIndex].InspectPackage(
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0],
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][1],
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][2],
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3],
            //    m_blnCustomWantPackage))
            //{
            //    //if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intFailPkgResultMask & 0x07) == 0)
            //    //    m_smVisionInfo.g_intSelectedImage = 1;

            //    // Hide pad inspection drawing and display package inspection drawing
            //    m_smVisionInfo.g_arrPad[intPadIndex].ref_blnViewPadResultDrawing = false;
            //    m_smVisionInfo.g_arrPad[intPadIndex].ref_blnViewPkgResultDrwaing = true;

            //    return false;
            //}

            return true;
        }

        private bool IsPackageOK_MultiThreading(int intLeadIndex, int intImageIndex)
        {
            //if (!m_smVisionInfo.g_blnCheckPackage)
            //    return true;

            // Question: Using the RectGauge4L measurement size or use the learn PackageROI size?
            // -------------RectGauge4L-------------------------------------
            float fPkgSizeWidth = 0;
            float fPkgSizeHeight = 0;

            if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                fPkgSizeWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth;
            else
                fPkgSizeWidth = Math.Max(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultUpWidth_RectGauge4L(0),
                                           m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultDownWidth_RectGauge4L(0));

            if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                fPkgSizeHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight;
            else
                fPkgSizeHeight = Math.Max(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultLeftHeight_RectGauge4L(0),
                                            m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultRightHeight_RectGauge4L(0));

            float fInspectionPkgSizeWidth = fPkgSizeWidth;
            float fInspectionPkgSizeHeight = fPkgSizeHeight;
            ROI objROI = new ROI();

            if (m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0] == null)
                m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0] = new ROI();
            if (m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] == null)
                m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] = new ROI();

            switch (intImageIndex)
            {
                case 0:
                    {
                        // Get First Image Package ROI
                        m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex]);

                        if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].LoadROISetting(
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.X -
                             fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.Y -
                             fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));
                        else
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].LoadROISetting(
                               (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().X -
                                fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                               (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().Y -
                                fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                               (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                               (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));

                        if ((m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_intFailPkgOptionMask & 0x80) > 0)   // Want check mold flash
                        {
                            if (m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] == null)
                                m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] = new ROI();
                            ImageDrawing ImgMoldFlash = new ImageDrawing();
                            ImgMoldFlash.SetImageSize(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex].ref_intImageWidth, m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex].ref_intImageHeight);
                            m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex].CopyTo(ImgMoldFlash);


                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3].AttachImage(ImgMoldFlash);
                            int intStartX = m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROITotalX - (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromLeft;// intMoldFlashLength;
                            int intExtendX;
                            int intStartY = m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROITotalY - (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromEdge;//intMoldFlashLength;
                            int intExtendY;
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3].LoadROISetting(
                            intStartX, intStartY,
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROIWidth + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromLeft + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromRight,
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROIHeight + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromBottom + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromEdge);
                        }
                        else
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] = null;

                        m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_blnViewPkgResultDrawing = true;    // Display package inspection result whether pass or fail
                        if (!m_smVisionInfo.g_arrLead3D[intLeadIndex].InspectPackage_Image1(
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0],
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3],
                            intLeadIndex))
                        {
                            objROI.Dispose();
                            return false;
                        }

                    }
                    break;
                case 1:
                    {
                        // Get Second Image Package ROI
                        m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][1].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex]);

                        if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][1].LoadROISetting(
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.X -
                             fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.Y -
                             fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));
                        else
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][1].LoadROISetting(
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().X -
                             fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().Y -
                             fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));

                        if ((m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_intFailPkgOptionMask & 0x80) > 0)   // Want check mold flash
                        {
                            if (m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] == null)
                                m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] = new ROI();
                          
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex - 1]);
                            int intStartX = m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROITotalX - (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromLeft;//intMoldFlashLength;
                            int intExtendX;
                            int intStartY = m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROITotalY - (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromEdge;//intMoldFlashLength;
                            int intExtendY;
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3].LoadROISetting(
                         intStartX, intStartY,
                         m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROIWidth + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromLeft + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromRight,
                         m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROIHeight + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromBottom + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromEdge);
                        }
                        else
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] = null;
                        
                        objROI.AttachImage(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][0]);
                        objROI.LoadROISetting(
                         (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().X -
                          fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                         (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().Y -
                          fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                         (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                         (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));
                       
                        m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_blnViewPkgResultDrawing = true;    // Display package inspection result whether pass or fail

                        if (!m_smVisionInfo.g_arrLead3D[intLeadIndex].InspectPackage_Image2(
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][1],
                            objROI,
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3],
                            intLeadIndex))
                        {
                            objROI.Dispose();
                            return false;
                        }

                    }

                    break;
                case 2:
                    {
                        // Get First Image Package ROI
                        m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex]);

                        if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].LoadROISetting(
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.X -
                             fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.Y -
                             fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));
                        else
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].LoadROISetting(
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().X -
                             fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().Y -
                             fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));

                        if ((m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_intFailPkgOptionMask & 0x80) > 0)   // Want check mold flash
                        {
                            if (m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] == null)
                                m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] = new ROI();
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex]);
                            int intStartX = m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROITotalX - (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromLeft;//intMoldFlashLength;
                            int intExtendX;
                            int intStartY = m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROITotalY - (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromEdge;//intMoldFlashLength;
                            int intExtendY;
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3].LoadROISetting(
                      intStartX, intStartY,
                      m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROIWidth + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromLeft + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromRight,
                      m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROIHeight + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromBottom + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromEdge);
                        }
                        else
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] = null;

                        objROI.AttachImage(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][0]);

                        if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                            objROI.LoadROISetting(
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.X -
                             fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.Y -
                             fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));
                        else
                            objROI.LoadROISetting(
                         (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().X -
                          fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                         (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().Y -
                          fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                         (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                         (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));

                        m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_blnViewPkgResultDrawing = true;    // Display package inspection result whether pass or fail
                        if (!m_smVisionInfo.g_arrLead3D[intLeadIndex].InspectPackage_Image2(
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0],
                            objROI,
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3],
                            intLeadIndex))
                        {
                            objROI.Dispose();
                            return false;
                        }
                    }
                    break;
                case 4: // Image 5 : Package Dark Field
                    {
                        // Get First Image Package ROI
                        m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex]);

                        if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].LoadROISetting(
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.X -
                             fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.Y -
                             fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));
                        else
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].LoadROISetting(
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().X -
                             fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().Y -
                             fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));

                        if ((m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_intFailPkgOptionMask & 0x80) > 0)   // Want check mold flash
                        {
                            if (m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] == null)
                                m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] = new ROI();
                             m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][intImageIndex]);
                            int intStartX = m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROITotalX - (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromLeft;//intMoldFlashLength;
                            int intExtendX;
                            int intStartY = m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROITotalY - (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromEdge;//intMoldFlashLength;
                            int intExtendY;
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3].LoadROISetting(
                      intStartX, intStartY,
                      m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROIWidth + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromLeft + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromRight,
                      m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0].ref_ROIHeight + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromBottom + (int)m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fMoldStartPixelFromEdge);
                        }
                        else
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3] = null;
                        objROI.AttachImage(m_smVisionInfo.g_arr5SRotatedImages[intLeadIndex][0]);

                        if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                            objROI.LoadROISetting(
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.X -
                             fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_pCornerPoint_Center.Y -
                             fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                            (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));
                        else
                            objROI.LoadROISetting(
                         (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().X -
                          fInspectionPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
                         (int)Math.Round(m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultCenterPoint_RectGauge4L().Y -
                          fInspectionPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
                         (int)Math.Round(fInspectionPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
                         (int)Math.Round(fInspectionPkgSizeHeight, 0, MidpointRounding.AwayFromZero));

                        m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_blnViewPkgResultDrawing = true;    // Display package inspection result whether pass or fail

                        if (!m_smVisionInfo.g_arrLead3D[intLeadIndex].InspectPackage_Image2(
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][0],
                            objROI,
                            m_smVisionInfo.g_arrInspectLead3DPkgROI[intLeadIndex][3],
                            intLeadIndex))
                        {
                            objROI.Dispose();
                            return false;
                        }
                    }

                    break;
            }
            objROI.Dispose();
            return true;
        }

        private bool IsPackageOK_MultiThreading2(int intPadIndex, int intImageIndex)
        {
            if (!m_smVisionInfo.g_blnCheckPackage)
                return true;

            //// Question: Using the RectGauge4L measurement size or use the learn PackageROI size?
            //// -------------RectGauge4L-------------------------------------
            //float fPkgSizeWidth = Math.Max(m_smVisionInfo.g_arrPad[intPadIndex].GetResultUpWidth_RectGauge4L(0),
            //                               m_smVisionInfo.g_arrPad[intPadIndex].GetResultDownWidth_RectGauge4L(0));
            //float fPkgSizeHeight = Math.Max(m_smVisionInfo.g_arrPad[intPadIndex].GetResultLeftHeight_RectGauge4L(0),
            //               m_smVisionInfo.g_arrPad[intPadIndex].GetResultLeftHeight_RectGauge4L(0));
            ////--------------------------------------------------------------
            ////float fPkgSizeWidth = m_smVisionInfo.g_arrPadROIs[intPadIndex][2].ref_ROIWidth;
            ////float fPkgSizeHeight = m_smVisionInfo.g_arrPadROIs[intPadIndex][2].ref_ROIHeight;

            //// Get First Image Package ROI
            //m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[intPadIndex][intImageIndex]);
            //m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].LoadROISetting(
            //    (int)Math.Round(m_smVisionInfo.g_arrPad[intPadIndex].GetResultCenterPoint_RectGauge4L().X -
            //     fPkgSizeWidth / 2, 0, MidpointRounding.AwayFromZero),
            //    (int)Math.Round(m_smVisionInfo.g_arrPad[intPadIndex].GetResultCenterPoint_RectGauge4L().Y -
            //     fPkgSizeHeight / 2, 0, MidpointRounding.AwayFromZero),
            //    (int)Math.Round(fPkgSizeWidth, 0, MidpointRounding.AwayFromZero),
            //    (int)Math.Round(fPkgSizeHeight, 0, MidpointRounding.AwayFromZero));

            ////// Get Second Image Package ROI
            ////if (m_smVisionInfo.g_arr5SRotatedImages[intPadIndex].Count > 1)
            ////{
            ////    bool blnWantSkipSidePadForImage2 = true;  // Coaxial light cannot view good defect in image 2 side pad, so will skip for defect checking in image 2 for 4 side pads.
            ////    if (intPadIndex == 0 || (intPadIndex > 0 && !blnWantSkipSidePadForImage2))
            ////    {
            ////        ROI objUnitROI = m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][1];
            ////        GetOtherImageUnitROI(intPadIndex, m_smVisionInfo.g_arr5SRotatedImages[intPadIndex][1], ref objUnitROI);
            ////    }
            ////    else
            ////        m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][1] = null;
            ////}

            ////// Get Third Image Package ROI
            ////if (m_smVisionInfo.g_arr5SRotatedImages[intPadIndex].Count > 2)
            ////{
            ////    ROI objUnitROI = m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][2];
            ////    GetOtherImageUnitROI(intPadIndex, m_smVisionInfo.g_arr5SRotatedImages[intPadIndex][2], ref objUnitROI);
            ////}
            ////else
            ////    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][2] = null;

            //if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intFailPkgOptionMask & 0x80) > 0)   // Want check mold flash
            //{
            //    if (m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3] == null)
            //        m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3] = new ROI();
            //    int intMoldFlashLength = (int)Math.Ceiling(m_smVisionInfo.g_arrPad[intPadIndex].GetMoldFlashLengthLimit(1) * Math.Max(m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY));
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[intPadIndex][intImageIndex]);
            //    int intStartX = m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].ref_ROITotalX - intMoldFlashLength;
            //    int intExtendX;
            //    if (intStartX < 0)
            //    {
            //        intExtendX = m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].ref_ROITotalX;
            //        intStartX = 0;
            //    }
            //    else
            //        intExtendX = intMoldFlashLength;

            //    int intStartY = m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].ref_ROITotalY - intMoldFlashLength;
            //    int intExtendY;
            //    if (intStartY < 0)
            //    {
            //        intExtendY = m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].ref_ROITotalY;
            //        intStartY = 0;
            //    }
            //    else
            //        intExtendY = intMoldFlashLength;

            //    //m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3].LoadROISetting(
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3].LoadROISetting(
            //    intStartX, intStartY,
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].ref_ROIWidth + intExtendX * 2,
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0].ref_ROIHeight + intExtendY * 2);
            //}
            //else
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3] = null;

            //if (!m_smVisionInfo.g_arrPad[intPadIndex].InspectPackage(
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][0],
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][1],
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][2],
            //    m_smVisionInfo.g_arrInspectPkgROI[intPadIndex][3],
            //    m_blnCustomWantPackage))
            //{
            //    //if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intFailPkgResultMask & 0x07) == 0)
            //    //    m_smVisionInfo.g_intSelectedImage = 1;

            //    // Hide pad inspection drawing and display package inspection drawing
            //    m_smVisionInfo.g_arrPad[intPadIndex].ref_blnViewPkgResultDrwaing = true;

            //    return false;
            //}

            return true;
        }
        private void ImageUniformize(int intImageNo)
        {
            if (!m_smVisionInfo.g_intViewUniformize || m_smVisionInfo.g_intImageMergeType == 0 || m_smVisionInfo.g_arrImageMaskingAvailable.Count <= intImageNo || m_smVisionInfo.g_arrReferenceImages.Count <= intImageNo)
                return;
            List<int> arrROIIndex = new List<int>();
            int intImageIndex = 0;
            //if (m_smVisionInfo.g_arrImageMaskingAvailable[intImageNo] && GetWantImageUniformize(m_smVisionInfo.g_arrImageMaskingSetting, intImageNo, ref arrROIIndex, ref intImageIndex))//m_smVisionInfo.g_arrImageMaskingSetting[intImageNo] != 0
            if (GetWantImageUniformize(m_smVisionInfo.g_arrImageMaskingSetting, intImageNo, ref arrROIIndex, ref intImageIndex))//m_smVisionInfo.g_arrImageMaskingSetting[intImageNo] != 0
            {
                ROI objROI = new ROI();
                objROI.AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
                for (int i = 0; i < arrROIIndex.Count; i++)
                {
                    if (arrROIIndex[i] == 1) //Top
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIPositionY,
                            m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIHeight);
                    }
                    else if (arrROIIndex[i] == 4) //Right
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY,
                            m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight);
                    }
                    else if (arrROIIndex[i] == 3) //Bottom
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIPositionY,
                            m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIHeight);
                    }
                    else if (arrROIIndex[i] == 2) //Left
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY,
                            m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight);
                    }
                    ImageDrawing.UniformizeImage(objROI, m_smVisionInfo.g_arrReferenceImages[intImageIndex], m_smVisionInfo.g_arrInvertedReferenceImages[intImageIndex], m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex], m_smVisionInfo.g_intImageMaskingThreshold);
                }
                objROI.Dispose();
            }
        }

        private void ImageUniformizeColor(int intImageNo)
        {
            if (!m_smVisionInfo.g_intViewUniformize || m_smVisionInfo.g_intImageMergeType == 0 || m_smVisionInfo.g_arrImageMaskingAvailable.Count <= intImageNo || m_smVisionInfo.g_arrReferenceImages.Count <= intImageNo)
                return;
            List<int> arrROIIndex = new List<int>();
            int intImageIndex = 0;
            if (m_smVisionInfo.g_arrImageMaskingAvailable[intImageNo] && GetWantImageUniformize(m_smVisionInfo.g_arrImageMaskingSetting, intImageNo, ref arrROIIndex, ref intImageIndex))//m_smVisionInfo.g_arrImageMaskingSetting[intImageNo] != 0
            {
                CROI objROI = new CROI();
                objROI.AttachImage(m_smVisionInfo.g_arrColorImages[intImageIndex]);
                for (int i = 0; i < arrROIIndex.Count; i++)
                {
                    if (arrROIIndex[i] == 1) //Top
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIPositionY,
                            m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIHeight);
                    }
                    else if (arrROIIndex[i] == 4) //Right
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY,
                            m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight);
                    }
                    else if (arrROIIndex[i] == 3) //Bottom
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIPositionY,
                            m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIHeight);
                    }
                    else if (arrROIIndex[i] == 2) //Left
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY,
                            m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight);
                    }
                    ImageDrawing.UniformizeColorImage(objROI,
                                                    m_smVisionInfo.g_arrReferenceColorImages[intImageIndex],
                                                    m_smVisionInfo.g_arrInvertedReferenceColorImages[intImageIndex],
                                                    m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex],
                                                    m_smVisionInfo.g_intImageMaskingThreshold);
                }
                objROI.Dispose();
            }
        }

        private bool GetWantImageUniformize(List<int> arrMaskingSetting, int intImageNo, ref List<int> arrROIIndex, ref int intImageIndex)
        {
            switch (m_smVisionInfo.g_intImageMergeType)
            {
                case 1:
                    {
                        if (intImageNo == 0)
                            return false;
                        else if (intImageNo == 1)
                        {
                            if (arrMaskingSetting.Count <= 0)
                                return false;

                            if (arrMaskingSetting[0] != 0)
                            {
                                arrROIIndex.Add(1);
                                arrROIIndex.Add(2);
                                arrROIIndex.Add(3);
                                arrROIIndex.Add(4);
                                intImageIndex = 0;
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                        {
                            if (arrMaskingSetting.Count <= intImageNo)
                                return false;

                            if (arrMaskingSetting[intImageNo] != 0)
                            {
                                arrROIIndex.Add(1);
                                arrROIIndex.Add(2);
                                arrROIIndex.Add(3);
                                arrROIIndex.Add(4);
                                intImageIndex = intImageNo;
                                return true;
                            }
                            else
                                return false;
                        }
                    }
                    break;
                case 2:
                    {
                        if (intImageNo == 0)
                            return false;
                        else if (intImageNo == 1)
                        {
                            if (arrMaskingSetting.Count <= 0)
                                return false;

                            if (arrMaskingSetting[0] != 0)
                            {
                                arrROIIndex.Add(1);
                                arrROIIndex.Add(2);
                                intImageIndex = 0;
                                return true;
                            }
                            else
                                return false;
                        }
                        else if (intImageNo == 2)
                        {
                            if (arrMaskingSetting.Count <= 0)
                                return false;

                            if (arrMaskingSetting[0] != 0)
                            {
                                arrROIIndex.Add(3);
                                arrROIIndex.Add(4);
                                intImageIndex = 0;
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                        {
                            if (arrMaskingSetting.Count <= intImageNo)
                                return false;

                            if (arrMaskingSetting[intImageNo] != 0)
                            {
                                arrROIIndex.Add(1);
                                arrROIIndex.Add(2);
                                arrROIIndex.Add(3);
                                arrROIIndex.Add(4);
                                intImageIndex = intImageNo;
                                return true;
                            }
                            else
                                return false;
                        }
                    }
                    break;
                case 3:
                    {
                        if (intImageNo == 0 || intImageNo == 2)
                            return false;
                        else if (intImageNo == 1)
                        {
                            if (arrMaskingSetting.Count <= 0)
                                return false;

                            if (arrMaskingSetting[0] != 0)
                            {
                                arrROIIndex.Add(1);
                                arrROIIndex.Add(2);
                                arrROIIndex.Add(3);
                                arrROIIndex.Add(4);
                                intImageIndex = 0;
                                return true;
                            }
                            else
                                return false;
                        }
                        else if (intImageNo == 3)
                        {
                            if (arrMaskingSetting.Count <= 2)
                                return false;

                            if (arrMaskingSetting[2] != 0)
                            {
                                arrROIIndex.Add(1);
                                arrROIIndex.Add(2);
                                arrROIIndex.Add(3);
                                arrROIIndex.Add(4);
                                intImageIndex = 2;
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                        {
                            if (arrMaskingSetting.Count <= intImageNo)
                                return false;

                            if (arrMaskingSetting[intImageNo] != 0)
                            {
                                arrROIIndex.Add(1);
                                arrROIIndex.Add(2);
                                arrROIIndex.Add(3);
                                arrROIIndex.Add(4);
                                intImageIndex = intImageNo;
                                return true;
                            }
                            else
                                return false;
                        }
                    }
                    break;
                case 4:
                    {
                        if (intImageNo == 0 || intImageNo == 3)
                            return false;
                        else if (intImageNo == 1)
                        {
                            if (arrMaskingSetting.Count <= 0)
                                return false;

                            if (arrMaskingSetting[0] != 0)
                            {
                                arrROIIndex.Add(1);
                                arrROIIndex.Add(2);
                                intImageIndex = 0;
                                return true;
                            }
                            else
                                return false;
                        }
                        else if (intImageNo == 2)
                        {
                            if (arrMaskingSetting.Count <= 0)
                                return false;

                            if (arrMaskingSetting[0] != 0)
                            {
                                arrROIIndex.Add(3);
                                arrROIIndex.Add(4);
                                intImageIndex = 0;
                                return true;
                            }
                            else
                                return false;
                        }
                        else if (intImageNo == 4)
                        {
                            if (arrMaskingSetting.Count <= 3)
                                return false;

                            if (arrMaskingSetting[3] != 0)
                            {
                                arrROIIndex.Add(1);
                                arrROIIndex.Add(2);
                                arrROIIndex.Add(3);
                                arrROIIndex.Add(4);
                                intImageIndex = 3;
                                return true;
                            }
                            else
                                return false;
                        }
                        else
                        {
                            if (arrMaskingSetting.Count <= intImageNo)
                                return false;

                            if (arrMaskingSetting[intImageNo] != 0)
                            {
                                arrROIIndex.Add(1);
                                arrROIIndex.Add(2);
                                arrROIIndex.Add(3);
                                arrROIIndex.Add(4);
                                intImageIndex = intImageNo;
                                return true;
                            }
                            else
                                return false;
                        }
                    }
                    break;

            }
            return false;
        }

        // Merged grab 2 & 3 to grab 1
        private void ImageMerge(int intImageNo)
        {
            if (m_smVisionInfo.g_intImageMergeType != 0)
            {
                if (m_smVisionInfo.g_intImageMergeType == 1)
                {
                    switch (intImageNo)
                    {
                        case 1:
                            //Copy 4 side of ROI (Top, Left, Bottom, Right to grab image 1)
                            for (int i = 1; i < 5; i++)
                            {
                                m_smVisionInfo.g_arrSystemROI[i].AttachImage(m_smVisionInfo.g_arrImages[intImageNo]);

                                ROI objROI = new ROI();
                                objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionY,
                                    m_smVisionInfo.g_arrSystemROI[i].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[i].ref_ROIHeight);
                                objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                                m_smVisionInfo.g_arrSystemROI[i].CopyImage(ref objROI);
                                objROI.Dispose();
                            }
                            break;
                    }
                }
                else
                {
                    switch (intImageNo)
                    {
                        case 1:
                            //Copy 2 side of ROI (Top, Left to grab image 1)
                            for (int i = 1; i < 3; i++)
                            {
                                m_smVisionInfo.g_arrSystemROI[i].AttachImage(m_smVisionInfo.g_arrImages[intImageNo]);

                                ROI objROI = new ROI();
                                objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionY,
                                    m_smVisionInfo.g_arrSystemROI[i].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[i].ref_ROIHeight);
                                objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                                m_smVisionInfo.g_arrSystemROI[i].CopyImage(ref objROI);
                                objROI.Dispose();
                            }
                            break;
                        case 2:
                            //Copy 2 side of ROI (Bottom, Right to grab image 1)
                            for (int i = 3; i < 5; i++)
                            {
                                m_smVisionInfo.g_arrSystemROI[i].AttachImage(m_smVisionInfo.g_arrImages[intImageNo]);

                                ROI objROI = new ROI();
                                objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionY,
                                    m_smVisionInfo.g_arrSystemROI[i].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[i].ref_ROIHeight);
                                objROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                                m_smVisionInfo.g_arrSystemROI[i].CopyImage(ref objROI);
                                objROI.Dispose();
                            }
                            break;
                    }
                }
            }
        }
        private void ImageUniformize(int intImageNo, List<ImageDrawing> arrImage)
        {
            if (!m_smVisionInfo.g_intViewUniformize || m_smVisionInfo.g_intImageMergeType == 0 || m_smVisionInfo.g_arrImageMaskingAvailable.Count < intImageNo || m_smVisionInfo.g_arrReferenceImages.Count < intImageNo)
                return;
            List<int> arrROIIndex = new List<int>();
            int intImageIndex = 0;
            if (m_smVisionInfo.g_arrImageMaskingAvailable[intImageNo] && GetWantImageUniformize(m_smVisionInfo.g_arrImageMaskingSetting, intImageNo, ref arrROIIndex, ref intImageIndex))//m_smVisionInfo.g_arrImageMaskingSetting[intImageNo] != 0
            {
                ROI objROI = new ROI();
                objROI.AttachImage(arrImage[intImageIndex]);
                for (int i = 0; i < arrROIIndex.Count; i++)
                {
                    if (arrROIIndex[i] == 1) //Top
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIPositionY,
                            m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIHeight);
                    }
                    else if (arrROIIndex[i] == 4) //Right
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY,
                            m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight);
                    }
                    else if (arrROIIndex[i] == 3) //Bottom
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIPositionY,
                            m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIHeight);
                    }
                    else if (arrROIIndex[i] == 2) //Left
                    {
                        objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY,
                            m_smVisionInfo.g_arrSystemROI[arrROIIndex[i]].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight);
                    }
                    ImageDrawing.UniformizeImage(objROI, m_smVisionInfo.g_arrReferenceImages[intImageIndex], m_smVisionInfo.g_arrInvertedReferenceImages[intImageIndex], m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex], m_smVisionInfo.g_intImageMaskingThreshold);
                }
                objROI.Dispose();
            }
        }
        private void ImageMerge(int intImageNo, List<ImageDrawing> arrImage)
        {
            if (m_smVisionInfo.g_intImageMergeType != 0)
            {
                if (m_smVisionInfo.g_intImageMergeType == 1)
                {
                    switch (intImageNo)
                    {
                        case 1:
                            //Copy 4 side of ROI (Top, Left, Bottom, Right to grab image 1)
                            for (int i = 1; i < 5; i++)
                            {
                                m_smVisionInfo.g_arrSystemROI[i].AttachImage(arrImage[intImageNo]);

                                ROI objROI = new ROI();
                                objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionY,
                                    m_smVisionInfo.g_arrSystemROI[i].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[i].ref_ROIHeight);
                                objROI.AttachImage(arrImage[0]);
                                m_smVisionInfo.g_arrSystemROI[i].CopyImage(ref objROI);
                                objROI.Dispose();
                            }
                            break;
                    }
                }
                else
                {
                    switch (intImageNo)
                    {
                        case 1:
                            //Copy 2 side of ROI (Top, Left to grab image 1)
                            for (int i = 1; i < 3; i++)
                            {
                                m_smVisionInfo.g_arrSystemROI[i].AttachImage(arrImage[intImageNo]);

                                ROI objROI = new ROI();
                                objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionY,
                                    m_smVisionInfo.g_arrSystemROI[i].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[i].ref_ROIHeight);
                                objROI.AttachImage(arrImage[0]);
                                m_smVisionInfo.g_arrSystemROI[i].CopyImage(ref objROI);
                                objROI.Dispose();
                            }
                            break;
                        case 2:
                            //Copy 2 side of ROI (Bottom, Right to grab image 1)
                            for (int i = 3; i < 5; i++)
                            {
                                m_smVisionInfo.g_arrSystemROI[i].AttachImage(arrImage[intImageNo]);

                                ROI objROI = new ROI();
                                objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionY,
                                    m_smVisionInfo.g_arrSystemROI[i].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[i].ref_ROIHeight);
                                objROI.AttachImage(arrImage[0]);
                                m_smVisionInfo.g_arrSystemROI[i].CopyImage(ref objROI);
                                objROI.Dispose();
                            }
                            break;
                    }
                }
            }
        }

        private int GetArrayImageIndex(int intUserSelectImageIndex)
        {
            if (intUserSelectImageIndex < 0)
                return 0;

            switch (m_smVisionInfo.g_intImageMergeType)
            {
                default:
                case 0: // No merge
                    {
                        return intUserSelectImageIndex;
                    }
                case 1: // Merge grab 1 center and grab 2 side 
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;
                        else if (intUserSelectImageIndex + 1 >= m_smVisionInfo.g_arrImages.Count)
                            return intUserSelectImageIndex;
                        else
                            return (intUserSelectImageIndex + 1);
                    }
                case 2: // Merge grab 1 center, grab 2 top left and grab 3 bottom right.
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;
                        else if (intUserSelectImageIndex + 1 >= m_smVisionInfo.g_arrImages.Count)
                            return intUserSelectImageIndex;
                        else if (intUserSelectImageIndex + 2 >= m_smVisionInfo.g_arrImages.Count)
                            return (intUserSelectImageIndex + 1);
                        else
                            return (intUserSelectImageIndex + 2);
                    }
                case 3: // Merge grab 1 center and grab 2 side and Merge grab 3 center and grab 4 side 
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;

                        if (intUserSelectImageIndex == 2) // select image 2 which is grab 5
                        {
                            if (intUserSelectImageIndex + 2 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 2);
                        }
                        else // select image 1 which is grab 3 and 4
                        {
                            if (intUserSelectImageIndex + 1 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 1);
                        }
                    }
                case 4: // Merge grab 1 center, grab 2 top left and grab 3 bottom right. and Merge grab 4 center and grab 5 side 
                    {
                        if (intUserSelectImageIndex <= 0)
                            return 0;

                        if (intUserSelectImageIndex == 1) // select image 1 which is grab 4 and 5
                        {
                            if (intUserSelectImageIndex + 2 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 2);
                        }
                        else // select other than image 0 or 1
                        {
                            if (intUserSelectImageIndex + 3 >= m_smVisionInfo.g_arrImages.Count)
                                return intUserSelectImageIndex;
                            else
                                return (intUserSelectImageIndex + 3);
                        }
                    }
            }
        }

        private void RotatedImageMerge()
        {
            if (m_smVisionInfo.g_arrSystemROI == null)
                return;

            int[] intSystemIndex = { 0, 1, 4, 3, 2 };
            //Copy center and 4 side ROI of 5SRotatedImage (Center Top, Left, Bottom, Right) to rotated image 1)
            ROI objROI = new ROI();
            int intViewImageCount = ImageDrawing.GetImageViewCount(m_smVisionInfo.g_intVisionIndex);

            for (int intSelectImageViewIndex = 0; intSelectImageViewIndex < intViewImageCount; intSelectImageViewIndex++)
            {
                int intImageNo = ImageDrawing.GetArrayImageIndex(intSelectImageViewIndex, m_smVisionInfo.g_intVisionIndex);

                for (int i = 0; i < 5; i++)
                {
                    if (m_arr5SImageRotated2[i] == null)
                        continue;
                    if (m_arr5SImageRotated2[i][intImageNo])
                        m_smVisionInfo.g_arrSystemROI[intSystemIndex[i]].AttachImage(m_smVisionInfo.g_arr5SRotatedImages[i][intImageNo]);
                    else
                        m_smVisionInfo.g_arrSystemROI[intSystemIndex[i]].AttachImage(m_smVisionInfo.g_arrImages[intImageNo]);

                    objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[intSystemIndex[i]].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[intSystemIndex[i]].ref_ROIPositionY,
                        m_smVisionInfo.g_arrSystemROI[intSystemIndex[i]].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[intSystemIndex[i]].ref_ROIHeight);
                    objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[intImageNo]);
                    m_smVisionInfo.g_arrSystemROI[intSystemIndex[i]].CopyImage(ref objROI);
                }
            }
            objROI.Dispose();
        }

        private void LoadSystemROISetting()
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\System\\ROI.xml";
            ROI.LoadFile(strFolderPath, m_smVisionInfo.g_arrSystemROI);
            ROI objROI;
            for (int i = 0; i < 5; i++)
            {
                switch (i)
                {
                    case 0:
                        //Middle
                        if (m_smVisionInfo.g_arrSystemROI.Count == 0)
                        {
                            objROI = new ROI("System Search ROI", 1);
                            objROI.LoadROISetting(m_smVisionInfo.g_intCameraResolutionWidth / 4, m_smVisionInfo.g_intCameraResolutionHeight / 4, m_smVisionInfo.g_intCameraResolutionWidth / 2, m_smVisionInfo.g_intCameraResolutionHeight / 2);
                            m_smVisionInfo.g_arrSystemROI.Add(objROI);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(((ROI)m_smVisionInfo.g_arrSystemROI[i]).ref_ROIPositionX,
                                                                             ((ROI)m_smVisionInfo.g_arrSystemROI[i]).ref_ROIPositionY,
                                                                            ((ROI)m_smVisionInfo.g_arrSystemROI[i]).ref_ROIWidth,
                                                                            ((ROI)m_smVisionInfo.g_arrSystemROI[i]).ref_ROIHeight);
                        }

                        m_smVisionInfo.g_arrSystemROI[i].AttachImage(m_smVisionInfo.g_arrImages[0]);
                        break;
                    case 1:
                        //Top
                        if (m_smVisionInfo.g_arrSystemROI.Count < 2)
                        {
                            objROI = new ROI("System Search ROI", 1);
                            m_smVisionInfo.g_arrSystemROI.Add(objROI);
                        }
                        m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(0, 0, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX + m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth,
                                                m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY);
                        break;
                    case 2:
                        //Left
                        if (m_smVisionInfo.g_arrSystemROI.Count < 3)
                        {
                            objROI = new ROI("System Search ROI", 1);
                            m_smVisionInfo.g_arrSystemROI.Add(objROI);
                        }
                        m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(0, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX,
                            m_smVisionInfo.g_intCameraResolutionHeight - m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY);
                        break;
                    case 3:
                        //Bottom
                        if (m_smVisionInfo.g_arrSystemROI.Count < 4)
                        {
                            objROI = new ROI("System Search ROI", 1);
                            m_smVisionInfo.g_arrSystemROI.Add(objROI);
                        }
                        m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY + m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight,
                            m_smVisionInfo.g_intCameraResolutionWidth - m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX,
                            m_smVisionInfo.g_intCameraResolutionHeight - (m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY + m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight));
                        break;
                    case 4:
                        //Right
                        if (m_smVisionInfo.g_arrSystemROI.Count < 5)
                        {
                            objROI = new ROI("System Search ROI", 1);
                            m_smVisionInfo.g_arrSystemROI.Add(objROI);
                        }
                        m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX + m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth, 0,
                            m_smVisionInfo.g_intCameraResolutionWidth - (m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX + m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth),
                            m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY + m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight);
                        break;
                }
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

        private void WaitEventDone(ref bool bTriggerEvent, bool bBreakResult)
        {
            while (true)
            {
                if (bTriggerEvent == bBreakResult)
                {
                    return;
                }

                Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
            }
        }
        private bool WaitEventDone(ref bool bTriggerEvent, bool bBreakResult, ref bool bReturnResult)
        {
            while (true)
            {
                if (bTriggerEvent == bBreakResult)
                {
                    return bReturnResult;
                }

                Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
            }

            return false;
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
                    SRMMessageBox.Show("Vision3Lead3DProcess->UpdateSubProgress_SaveImage() :" + ex.ToString());
                }
                Thread.Sleep(1);
            }

            m_thSubThread_SaveImage = null;
            m_blnStopped_SaveImage = true;
        }

        private void SavePassImage_AddToBuffer()
        {
            if (m_smCustomizeInfo.g_blnSavePassImage)
            {
                if (m_smVisionInfo.g_intPassImageCount < m_smCustomizeInfo.g_intPassImagePics)
                {
                    WaitImageBufferClear(ref m_intPassStartNode, ref m_intPassEndNode);

                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[0].CopyTo(ref m_arrPassCImage1Buffer[m_intPassEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[0].CopyTo(ref m_arrPassImage1Buffer[m_intPassEndNode]);

                    if ((m_smVisionInfo.g_arrImages.Count > 1) && WantSaveImageAccordingMergeType(1))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_smVisionInfo.g_arrColorImages[1].CopyTo(ref m_arrPassCImage2Buffer[m_intPassEndNode]);
                        else
                            m_smVisionInfo.g_arrImages[1].CopyTo(ref m_arrPassImage2Buffer[m_intPassEndNode]);
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 2) && WantSaveImageAccordingMergeType(2))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_smVisionInfo.g_arrColorImages[2].CopyTo(ref m_arrPassCImage3Buffer[m_intPassEndNode]);
                        else
                            m_smVisionInfo.g_arrImages[2].CopyTo(ref m_arrPassImage3Buffer[m_intPassEndNode]);
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 3) && WantSaveImageAccordingMergeType(3))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_smVisionInfo.g_arrColorImages[3].CopyTo(ref m_arrPassCImage4Buffer[m_intPassEndNode]);
                        else
                            m_smVisionInfo.g_arrImages[3].CopyTo(ref m_arrPassImage4Buffer[m_intPassEndNode]);
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 4) && WantSaveImageAccordingMergeType(4))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_smVisionInfo.g_arrColorImages[4].CopyTo(ref m_arrPassCImage5Buffer[m_intPassEndNode]);
                        else
                            m_smVisionInfo.g_arrImages[4].CopyTo(ref m_arrPassImage5Buffer[m_intPassEndNode]);
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 5) && WantSaveImageAccordingMergeType(5))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_smVisionInfo.g_arrColorImages[5].CopyTo(ref m_arrPassCImage6Buffer[m_intPassEndNode]);
                        else
                            m_smVisionInfo.g_arrImages[5].CopyTo(ref m_arrPassImage6Buffer[m_intPassEndNode]);
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 6) && WantSaveImageAccordingMergeType(6))
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

        private void SaveRejectImage_AddToBuffer(string strRejectName, string strRejectMessage)
        {
            STTrackLog.WriteLine("SaveRejectImage_AddToBuffer - 0");

            if (m_smCustomizeInfo.g_blnSaveFailImage)
            {
                STTrackLog.WriteLine("SaveRejectImage_AddToBuffer - 1");
                if (m_smCustomizeInfo.g_intSaveImageMode == 0)
                {
                    if (m_smVisionInfo.g_intFailImageCount >= m_smCustomizeInfo.g_intFailImagePics)
                        return;
                }

                STTrackLog.WriteLine("SaveRejectImage_AddToBuffer - 2");
                WaitImageBufferClear(ref m_intFailStartNode, ref m_intFailEndNode);

                STTrackLog.WriteLine("SaveRejectImage_AddToBuffer - 3");
                //To handle case when test fail before all image grab complete
                WaitAllImageGrabDone();

                if (m_smVisionInfo.g_blnViewColorImage)
                    m_smVisionInfo.g_arrColorImages[0].CopyTo(ref m_arrFailCImage1Buffer[m_intFailEndNode]);
                else
                    m_smVisionInfo.g_arrImages[0].CopyTo(ref m_arrFailImage1Buffer[m_intFailEndNode]);

                if ((m_smVisionInfo.g_arrImages.Count > 1) && WantSaveImageAccordingMergeType(1))
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[1].CopyTo(ref m_arrFailCImage2Buffer[m_intFailEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[1].CopyTo(ref m_arrFailImage2Buffer[m_intFailEndNode]);
                }

                if ((m_smVisionInfo.g_arrImages.Count > 2) && WantSaveImageAccordingMergeType(2))
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[2].CopyTo(ref m_arrFailCImage3Buffer[m_intFailEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[2].CopyTo(ref m_arrFailImage3Buffer[m_intFailEndNode]);
                }

                if ((m_smVisionInfo.g_arrImages.Count > 3) && WantSaveImageAccordingMergeType(3))
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[3].CopyTo(ref m_arrFailCImage4Buffer[m_intFailEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[3].CopyTo(ref m_arrFailImage4Buffer[m_intFailEndNode]);
                }

                if ((m_smVisionInfo.g_arrImages.Count > 4) && WantSaveImageAccordingMergeType(4))
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[4].CopyTo(ref m_arrFailCImage5Buffer[m_intFailEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[4].CopyTo(ref m_arrFailImage5Buffer[m_intFailEndNode]);
                }

                if ((m_smVisionInfo.g_arrImages.Count > 5) && WantSaveImageAccordingMergeType(5))
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[5].CopyTo(ref m_arrFailCImage6Buffer[m_intFailEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[5].CopyTo(ref m_arrFailImage6Buffer[m_intFailEndNode]);
                }

                if ((m_smVisionInfo.g_arrImages.Count > 6) && WantSaveImageAccordingMergeType(6))
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[6].CopyTo(ref m_arrFailCImage7Buffer[m_intFailEndNode]);
                    else
                        m_smVisionInfo.g_arrImages[6].CopyTo(ref m_arrFailImage7Buffer[m_intFailEndNode]);
                }

                STTrackLog.WriteLine("SaveRejectImage_AddToBuffer - 4");
                m_arrRejectNameBuffer[m_intFailEndNode] = strRejectName;
                m_arrRejectMessageBuffer[m_intFailEndNode] = strRejectMessage;
                //m_arrFailNoBuffer[m_intFailEndNode] = m_smVisionInfo.g_intFailImageCount;
                m_arrFailNoBuffer[m_intFailEndNode] = m_smVisionInfo.g_intTotalImageCount;  // 2019 09 18 - CCENG: Use total image count instead of Fail Image count so that pass fail image will display in sequence.
                m_smVisionInfo.g_intTotalImageCount++;
                m_smVisionInfo.g_intFailImageCount++;
                STTrackLog.WriteLine("From m_intFailEndNode = " + m_intFailEndNode);
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

                STTrackLog.WriteLine("++ m_intFailEndNode = " + m_intFailEndNode);

            }
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
                        m_arrPassCImage1Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + ".bmp");
                        //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                        m_arrPassCImage1Buffer[m_intPassStartNode].Dispose();
                        m_arrPassCImage1Buffer[m_intPassStartNode] = new CImageDrawing(true);
                    }
                    else
                    {
                        m_arrPassImage1Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + ".bmp");
                        //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                        m_arrPassImage1Buffer[m_intPassStartNode].Dispose();
                        m_arrPassImage1Buffer[m_intPassStartNode] = new ImageDrawing(true);
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 1) && WantSaveImageAccordingMergeType(1))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrPassCImage2Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + "_Image1.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassCImage2Buffer[m_intPassStartNode].Dispose();
                            m_arrPassCImage2Buffer[m_intPassStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrPassImage2Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + "_Image1.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassImage2Buffer[m_intPassStartNode].Dispose();
                            m_arrPassImage2Buffer[m_intPassStartNode] = new ImageDrawing(true);
                        }
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 2) && WantSaveImageAccordingMergeType(2))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrPassCImage3Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + "_Image2.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassCImage3Buffer[m_intPassStartNode].Dispose();
                            m_arrPassCImage3Buffer[m_intPassStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrPassImage3Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + "_Image2.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassImage3Buffer[m_intPassStartNode].Dispose();
                            m_arrPassImage3Buffer[m_intPassStartNode] = new ImageDrawing(true);
                        }
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 3) && WantSaveImageAccordingMergeType(3))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrPassCImage4Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + "_Image3.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassCImage4Buffer[m_intPassStartNode].Dispose();
                            m_arrPassCImage4Buffer[m_intPassStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrPassImage4Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + "_Image3.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassImage4Buffer[m_intPassStartNode].Dispose();
                            m_arrPassImage4Buffer[m_intPassStartNode] = new ImageDrawing(true);
                        }
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 4) && WantSaveImageAccordingMergeType(4))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrPassCImage5Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + "_Image4.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassCImage5Buffer[m_intPassStartNode].Dispose();
                            m_arrPassCImage5Buffer[m_intPassStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrPassImage5Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + "_Image4.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassImage5Buffer[m_intPassStartNode].Dispose();
                            m_arrPassImage5Buffer[m_intPassStartNode] = new ImageDrawing(true);
                        }
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 5) && WantSaveImageAccordingMergeType(5))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrPassCImage6Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + "_Image5.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassCImage6Buffer[m_intPassStartNode].Dispose();
                            m_arrPassCImage6Buffer[m_intPassStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrPassImage6Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + "_Image5.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassImage6Buffer[m_intPassStartNode].Dispose();
                            m_arrPassImage6Buffer[m_intPassStartNode] = new ImageDrawing(true);
                        }
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 6) && WantSaveImageAccordingMergeType(6))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrPassCImage7Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + "_Image6.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassCImage7Buffer[m_intPassStartNode].Dispose();
                            m_arrPassCImage7Buffer[m_intPassStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrPassImage7Buffer[m_intPassStartNode].SaveImage(strPath + m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + "_Image6.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrPassImage7Buffer[m_intPassStartNode].Dispose();
                            m_arrPassImage7Buffer[m_intPassStartNode] = new ImageDrawing(true);
                        }
                    }

                    m_smVisionInfo.g_strLastImageFolder = strPath;
                    m_smVisionInfo.g_strLastImageName = m_arrPassNoBuffer[m_intPassStartNode] + "_Pass" + ".bmp";

                    m_intPassStartNode++;
                    if (m_intPassStartNode == m_smVisionInfo.g_intSaveImageBufferSize)
                    {
                        m_intPassStartNode = 0;
                    }
                }

                if (m_intFailStartNode != m_intFailEndNode)
                {
                    STTrackLog.WriteLine("SaveImageBuffer 1 Start=" + m_intFailStartNode.ToString() + ", End=" + m_intFailEndNode.ToString());

                    //2021-02-24 ZJYEOH : Should use m_smVisionInfo.g_intFailImageCount to compare
                    if (/*m_arrFailNoBuffer[m_intFailStartNode]*/ m_smVisionInfo.g_intFailImageCount > m_smCustomizeInfo.g_intFailImagePics)//>=
                    {
                        STTrackLog.WriteLine("SaveImageBuffer 1a Start=" + m_intFailStartNode.ToString() + ", End=" + m_intFailEndNode.ToString());
                        if (!m_blnLoadRejectImageListPath)
                        {
                            LoadRejectImageListPath();

                            m_blnLoadRejectImageListPath = true;
                        }

                        STTrackLog.WriteLine("SaveImageBuffer 1aq Start=" + m_intFailStartNode.ToString() + ", End=" + m_intFailEndNode.ToString());

                        if (m_arrRejectImageListPath.Count > 0)
                        {
                            STTrackLog.WriteLine("SaveImageBuffer 1ab Start=" + m_intFailStartNode.ToString() + ", End=" + m_intFailEndNode.ToString());
                            string strDeleteFile = m_arrRejectImageListPath[0];
                            if (File.Exists(strDeleteFile))
                            {
                                try
                                {
                                    File.Delete(strDeleteFile);
                                }
                                catch (Exception ex)
                                {
                                    STTrackLog.WriteLine("Vision3Lead3DProcess.cs > SaveImageBuffer Delete _image0 > Ex=" + ex.ToString());
                                }
                            }

                            STTrackLog.WriteLine("SaveImageBuffer 1ac Start=" + m_intFailStartNode.ToString() + ", End=" + m_intFailEndNode.ToString());
                            if (m_smVisionInfo.g_arrImages.Count > 1)
                            {
                                int intStartIndex = strDeleteFile.LastIndexOf(".bmp");
                                if (intStartIndex > 0)
                                {
                                    string strDeleteFileImage1 = strDeleteFile.Substring(0, intStartIndex) + "_Image1.bmp";
                                    if (File.Exists(strDeleteFileImage1))
                                    {
                                        try
                                        {
                                            File.Delete(strDeleteFileImage1);
                                        }
                                        catch (Exception ex)
                                        {
                                            STTrackLog.WriteLine("Vision3Lead3DProcess.cs > SaveImageBuffer Delete _image1 > Ex=" + ex.ToString());
                                        }
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
                                        try
                                        {
                                            File.Delete(strDeleteFileImage2);
                                        }
                                        catch (Exception ex)
                                        {
                                            STTrackLog.WriteLine("Vision3Lead3DProcess.cs > SaveImageBuffer Delete _image2 > Ex=" + ex.ToString());
                                        }
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
                                        try
                                        {
                                            File.Delete(strDeleteFileImage3);
                                        }
                                        catch (Exception ex)
                                        {
                                            STTrackLog.WriteLine("Vision3Lead3DProcess.cs > SaveImageBuffer Delete _image3 > Ex=" + ex.ToString());
                                        }
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
                                        try
                                        {
                                            File.Delete(strDeleteFileImage4);
                                        }
                                        catch (Exception ex)
                                        {
                                            STTrackLog.WriteLine("Vision3Lead3DProcess.cs > SaveImageBuffer Delete _image4 > Ex=" + ex.ToString());
                                        }
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
                                        try
                                        {
                                            File.Delete(strDeleteFileImage5);
                                        }
                                        catch (Exception ex)
                                        {
                                            STTrackLog.WriteLine("Vision3Lead3DProcess.cs > SaveImageBuffer Delete _image5 > Ex=" + ex.ToString());
                                        }
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
                                        try
                                        {
                                            File.Delete(strDeleteFileImage6);
                                        }
                                        catch (Exception ex)
                                        {
                                            STTrackLog.WriteLine("Vision3Lead3DProcess.cs > SaveImageBuffer Delete _image6 > Ex=" + ex.ToString());
                                        }
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

                        STTrackLog.WriteLine("SaveImageBuffer 1b Start=" + m_intFailStartNode.ToString() + ", End=" + m_intFailEndNode.ToString());
                    }
                    else if (m_arrFailNoBuffer[m_intFailStartNode] == 0)
                    {
                        STTrackLog.WriteLine("SaveImageBuffer 1c Start=" + m_intFailStartNode.ToString() + ", End=" + m_intFailEndNode.ToString());
                        m_arrRejectImageListPath.Clear();
                        m_arrRejectImageErrorMessageListPath.Clear();
                        STTrackLog.WriteLine("SaveImageBuffer 1d Start=" + m_intFailStartNode.ToString() + ", End=" + m_intFailEndNode.ToString());
                    }

                    STTrackLog.WriteLine("SaveImageBuffer 2 Start=" + m_intFailStartNode.ToString() + ", End=" + m_intFailEndNode.ToString());
                    //string strPath = m_smVisionInfo.g_strSaveImageLocation +
                    //        m_smProductionInfo.g_strLotID + "_" + m_smProductionInfo.g_strLotStartTime +
                    //        "\\" + m_smVisionInfo.g_strVisionFolderName +
                    //        "(" + m_smVisionInfo.g_strVisionDisplayName + " " + m_smVisionInfo.g_strVisionNameRemark + ")" +
                    //        "\\" + m_arrRejectNameBuffer[m_intFailStartNode] + "\\";

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
                        m_arrFailCImage1Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".bmp");
                        //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                        m_arrFailCImage1Buffer[m_intFailStartNode].Dispose();
                        m_arrFailCImage1Buffer[m_intFailStartNode] = new CImageDrawing(true);
                    }
                    else
                    {
                        m_arrFailImage1Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".bmp");
                        //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                        m_arrFailImage1Buffer[m_intFailStartNode].Dispose();
                        m_arrFailImage1Buffer[m_intFailStartNode] = new ImageDrawing(true);
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 1) && WantSaveImageAccordingMergeType(1))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrFailCImage2Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image1.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailCImage2Buffer[m_intFailStartNode].Dispose();
                            m_arrFailCImage2Buffer[m_intFailStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrFailImage2Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image1.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailImage2Buffer[m_intFailStartNode].Dispose();
                            m_arrFailImage2Buffer[m_intFailStartNode] = new ImageDrawing(true);
                        }
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 2) && WantSaveImageAccordingMergeType(2))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrFailCImage3Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image2.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailCImage3Buffer[m_intFailStartNode].Dispose();
                            m_arrFailCImage3Buffer[m_intFailStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrFailImage3Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image2.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailImage3Buffer[m_intFailStartNode].Dispose();
                            m_arrFailImage3Buffer[m_intFailStartNode] = new ImageDrawing(true);
                        }
                    }

                    if ((m_smVisionInfo.g_arrImages.Count > 3) && WantSaveImageAccordingMergeType(3))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrFailCImage4Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image3.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailCImage4Buffer[m_intFailStartNode].Dispose();
                            m_arrFailCImage4Buffer[m_intFailStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrFailImage4Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image3.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailImage4Buffer[m_intFailStartNode].Dispose();
                            m_arrFailImage4Buffer[m_intFailStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 4 && WantSaveImageAccordingMergeType(4))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrFailCImage5Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image4.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailCImage5Buffer[m_intFailStartNode].Dispose();
                            m_arrFailCImage5Buffer[m_intFailStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrFailImage5Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image4.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailImage5Buffer[m_intFailStartNode].Dispose();
                            m_arrFailImage5Buffer[m_intFailStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 5 && WantSaveImageAccordingMergeType(5))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrFailCImage6Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image5.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailCImage6Buffer[m_intFailStartNode].Dispose();
                            m_arrFailCImage6Buffer[m_intFailStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrFailImage6Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image5.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailImage6Buffer[m_intFailStartNode].Dispose();
                            m_arrFailImage6Buffer[m_intFailStartNode] = new ImageDrawing(true);
                        }
                    }

                    if (m_smVisionInfo.g_arrImages.Count > 6 && WantSaveImageAccordingMergeType(6))
                    {
                        if (m_smVisionInfo.g_blnViewColorImage)
                        {
                            m_arrFailCImage7Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image6.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailCImage7Buffer[m_intFailStartNode].Dispose();
                            m_arrFailCImage7Buffer[m_intFailStartNode] = new CImageDrawing(true);
                        }
                        else
                        {
                            m_arrFailImage7Buffer[m_intFailStartNode].SaveImage(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + "_Image6.bmp");
                            //2021-02-04 ZJYEOH : Dispose and init again can reduce RAM consumption
                            m_arrFailImage7Buffer[m_intFailStartNode].Dispose();
                            m_arrFailImage7Buffer[m_intFailStartNode] = new ImageDrawing(true);
                        }
                    }
                    //m_smVisionInfo.g_strLastImageFolder = strPath;
                    //m_smVisionInfo.g_strLastImageName = m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".bmp";
                    //m_arrRejectImageListPath.Add(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".bmp");
                    //m_intFailStartNode++;
                    //if (m_intFailStartNode == BUFFERSIZE)
                    //{
                    //    m_intFailStartNode = 0;
                    //}

                    STTrackLog.WriteLine("SaveImageBuffer 3 Start=" + m_intFailStartNode.ToString() + ", End=" + m_intFailEndNode.ToString());
                    if (m_smCustomizeInfo.g_blnSaveFailImageErrorMessage)
                    {
                        XmlParser objFile = new XmlParser(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".xml");
                        objFile.WriteSectionElement("ErrorMessage");
                        objFile.WriteElement1Value("Message_" + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode], m_arrRejectMessageBuffer[m_intFailStartNode]);
                        objFile.WriteEndElement();
                    }

                    m_smVisionInfo.g_strLastImageFolder = strPath;
                    m_smVisionInfo.g_strLastImageName = m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".bmp";
                    m_arrRejectImageListPath.Add(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".bmp");
                    m_arrRejectImageErrorMessageListPath.Add(strPath + m_arrFailNoBuffer[m_intFailStartNode] + "_" + m_arrRejectNameBuffer[m_intFailStartNode] + ".xml");

                    STTrackLog.WriteLine("From m_intFailStartNode = " + m_intFailStartNode);
                    m_intFailStartNode++;
                    if (m_intFailStartNode == m_smVisionInfo.g_intSaveImageBufferSize)
                    {
                        m_intFailStartNode = 0;
                    }
                    STTrackLog.WriteLine("++ m_intFailStartNode = " + m_intFailStartNode);
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Vision1Process.cs > SaveImageBuffer > Ex=" + ex.ToString());
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

        private void WaitImageBufferClear(ref int intStartNode, ref int intEndNode)
        {
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            STTrackLog.WriteLine("WaitImageBufferClear Before Loop. intStartNode = " + intStartNode.ToString() + ", intEndNode = " + intEndNode.ToString());
            while (true)
            {
                int intNextEndNode = intEndNode + 1;
                if (intNextEndNode >= m_smVisionInfo.g_intSaveImageBufferSize)
                    intNextEndNode = 0;

                if (intNextEndNode != intStartNode)
                {
                    return;
                }

                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out WaitImageBufferClear - " + "Wait Image 10000");
                    STTrackLog.WriteLine("Waiting event timeout");
                    
                    break;
                }

                Thread.Sleep(1);
            }

            STTrackLog.WriteLine("WaitImageBufferClear After Loop. intStartNode = " + intStartNode.ToString() + ", intEndNode = " + intEndNode.ToString());
            timeout.Stop();

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
                if (intGrabRequire > 1 && WantSaveImageAccordingMergeType(1))
                    m_arrPassCImage2Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 2 && WantSaveImageAccordingMergeType(2))
                    m_arrPassCImage3Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 3 && WantSaveImageAccordingMergeType(3))
                    m_arrPassCImage4Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 4 && WantSaveImageAccordingMergeType(4))
                    m_arrPassCImage5Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 5 && WantSaveImageAccordingMergeType(5))
                    m_arrPassCImage6Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 6 && WantSaveImageAccordingMergeType(6))
                    m_arrPassCImage7Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];

                if (intGrabRequire > 0)
                    m_arrFailCImage1Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 1 && WantSaveImageAccordingMergeType(1))
                    m_arrFailCImage2Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 2 && WantSaveImageAccordingMergeType(2))
                    m_arrFailCImage3Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 3 && WantSaveImageAccordingMergeType(3))
                    m_arrFailCImage4Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 4 && WantSaveImageAccordingMergeType(4))
                    m_arrFailCImage5Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 5 && WantSaveImageAccordingMergeType(5))
                    m_arrFailCImage6Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 6 && WantSaveImageAccordingMergeType(6))
                    m_arrFailCImage7Buffer = new CImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];

                for (int i = 0; i < m_smVisionInfo.g_intSaveImageBufferSize; i++)
                {
                    if (intGrabRequire > 0)
                        m_arrPassCImage1Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 1 && WantSaveImageAccordingMergeType(1))
                        m_arrPassCImage2Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 2 && WantSaveImageAccordingMergeType(2))
                        m_arrPassCImage3Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 3 && WantSaveImageAccordingMergeType(3))
                        m_arrPassCImage4Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 4 && WantSaveImageAccordingMergeType(4))
                        m_arrPassCImage5Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 5 && WantSaveImageAccordingMergeType(5))
                        m_arrPassCImage6Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 6 && WantSaveImageAccordingMergeType(6))
                        m_arrPassCImage7Buffer[i] = new CImageDrawing(true);

                    if (intGrabRequire > 0)
                        m_arrFailCImage1Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 1 && WantSaveImageAccordingMergeType(1))
                        m_arrFailCImage2Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 2 && WantSaveImageAccordingMergeType(2))
                        m_arrFailCImage3Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 3 && WantSaveImageAccordingMergeType(3))
                        m_arrFailCImage4Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 4 && WantSaveImageAccordingMergeType(4))
                        m_arrFailCImage5Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 5 && WantSaveImageAccordingMergeType(5))
                        m_arrFailCImage6Buffer[i] = new CImageDrawing(true);
                    if (intGrabRequire > 6 && WantSaveImageAccordingMergeType(6))
                        m_arrFailCImage7Buffer[i] = new CImageDrawing(true);
                }
            }
            else
            {
                if (intGrabRequire > 0)
                    m_arrPassImage1Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 1 && WantSaveImageAccordingMergeType(1))
                    m_arrPassImage2Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 2 && WantSaveImageAccordingMergeType(2))
                    m_arrPassImage3Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 3 && WantSaveImageAccordingMergeType(3))
                    m_arrPassImage4Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 4 && WantSaveImageAccordingMergeType(4))
                    m_arrPassImage5Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 5 && WantSaveImageAccordingMergeType(5))
                    m_arrPassImage6Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 6 && WantSaveImageAccordingMergeType(6))
                    m_arrPassImage7Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];

                if (intGrabRequire > 0)
                    m_arrFailImage1Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 1 && WantSaveImageAccordingMergeType(1))
                    m_arrFailImage2Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 2 && WantSaveImageAccordingMergeType(2))
                    m_arrFailImage3Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 3 && WantSaveImageAccordingMergeType(3))
                    m_arrFailImage4Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 4 && WantSaveImageAccordingMergeType(4))
                    m_arrFailImage5Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 5 && WantSaveImageAccordingMergeType(5))
                    m_arrFailImage6Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];
                if (intGrabRequire > 6 && WantSaveImageAccordingMergeType(6))
                    m_arrFailImage7Buffer = new ImageDrawing[m_smVisionInfo.g_intSaveImageBufferSize];

                for (int i = 0; i < m_smVisionInfo.g_intSaveImageBufferSize; i++)
                {
                    if (intGrabRequire > 0)
                        m_arrPassImage1Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 1 && WantSaveImageAccordingMergeType(1))
                        m_arrPassImage2Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 2 && WantSaveImageAccordingMergeType(2))
                        m_arrPassImage3Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 3 && WantSaveImageAccordingMergeType(3))
                        m_arrPassImage4Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 4 && WantSaveImageAccordingMergeType(4))
                        m_arrPassImage5Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 5 && WantSaveImageAccordingMergeType(5))
                        m_arrPassImage6Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 6 && WantSaveImageAccordingMergeType(6))
                        m_arrPassImage7Buffer[i] = new ImageDrawing(true);

                    if (intGrabRequire > 0)
                        m_arrFailImage1Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 1 && WantSaveImageAccordingMergeType(1))
                        m_arrFailImage2Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 2 && WantSaveImageAccordingMergeType(2))
                        m_arrFailImage3Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 3 && WantSaveImageAccordingMergeType(3))
                        m_arrFailImage4Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 4 && WantSaveImageAccordingMergeType(4))
                        m_arrFailImage5Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 5 && WantSaveImageAccordingMergeType(5))
                        m_arrFailImage6Buffer[i] = new ImageDrawing(true);
                    if (intGrabRequire > 6 && WantSaveImageAccordingMergeType(6))
                        m_arrFailImage7Buffer[i] = new ImageDrawing(true);
                }
            }
            m_arrPassNoBuffer = new int[m_smVisionInfo.g_intSaveImageBufferSize];
            m_arrFailNoBuffer = new int[m_smVisionInfo.g_intSaveImageBufferSize];
            m_arrRejectNameBuffer = new string[m_smVisionInfo.g_intSaveImageBufferSize];
            m_arrRejectMessageBuffer = new string[m_smVisionInfo.g_intSaveImageBufferSize];
        }

        private bool Package5SInspection_MultiThreading(int intLeadTestMask, ref ResulType eInspectionResult, int intPkgSizeImageIndex, int intPkgDefectImageIndex)
        {
            if (intLeadTestMask == 1)
            {
                m_fTiming2 = m_T2.Timing;
                m_strTrack_Center += ", C3a_" + intPkgDefectImageIndex.ToString() + "= " + (m_fTiming2 - m_fTimingPrev2).ToString();
                m_fTimingPrev2 = m_fTiming2;
            }

            bool bTestFail = false;

            // Loop ROI from Middle, Up, Right, Down to Left
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != 0)
                    break;

                if ((intLeadTestMask & (0x01 << i)) == 0)
                    continue;

                m_smVisionInfo.g_arrLead3D[i].ref_blnLock = true;  // Set true to stop drawing.

                if (intPkgDefectImageIndex >= m_arr5SImageRotated2[i].Count)
                    continue;

                if (m_arr5SFoundUnit[i] && m_arr5SFoundUnitPkg[i] && m_arr5SImageRotated2[i][intPkgDefectImageIndex] && m_arr5SPackagedSizeChecked[i])
                    continue;
                
                // Reset previous inspection data
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewPkgSizeDrawing = false;
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewEdgeNotFoundDrawing = false;

                // Identify pad defination for displaying fail message
                string strPosition = GetROIDefinition(i);

                int intUserSelectedPkgImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(0);
                int intFailType = 0;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                {
                    if (i == 0)
                    {
                        if (!IsPackageSizeUsingCornerOK_MultiThreading(i))
                        {
                            m_arr5SPackagedSizeChecked[i] = true;
                            m_smVisionInfo.g_arrLead3D[i].ref_blnViewPkgSizeDrawing = true; // 20-05-2019 ZJYEOH: Changed to true to draw gauge during production
                            bTestFail = true;
                            eInspectionResult = ResulType.FailLeadPkgDimension;

                            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                                if (m_intTCPIPResultID == -1)
                                    m_intTCPIPResultID = (int)TCPIPResulID.Fail;// FailPackageDimension;

                            m_arrErrorMessage[i] = strPosition + m_arrErrorMessage[i];
                            break;
                        }
                    }
                }
                else
                {
                    // Find unit location, size and angle
                    if (!FindUnit_ForPackage_Multithreading(i, intUserSelectedPkgImage, ref intFailType)) //intPkgSizeImageIndex
                    {
                        bTestFail = true;
                        if (intFailType == 0)
                            eInspectionResult = ResulType.FailLeadStandOff;// ResulType.FailPosition;   //2020 11 02 - CCENG: Zhang Zhong reqest to use Standoff fail counter when cannot find unit.
                        else
                        {
                            eInspectionResult = ResulType.FailEdgeNotFound;
                            m_smVisionInfo.g_arrLead3D[i].ref_blnViewEdgeNotFoundDrawing = true;
                        }
                        m_arrErrorMessage[i] = strPosition + m_arrErrorMessage[i];

                        if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                            if (m_intTCPIPResultID == -1)
                                m_intTCPIPResultID = (int)TCPIPResulID.Fail;//FailPackage;

                        break;
                    }

                    // 2019 06 01-CCENG: Check Package Size Width and Height for center ROI only. Thickness will only be checked after all 4 side inspection done because average of thickness will be used to check pass or fail.
                    if (i == 0)
                    {
                        if (!IsPackageSizeOK_MultiThreading(i))
                        {
                            m_arr5SPackagedSizeChecked[i] = true;
                            m_smVisionInfo.g_arrLead3D[i].ref_blnViewPkgSizeDrawing = true; // 20-05-2019 ZJYEOH: Changed to true to draw gauge during production
                            bTestFail = true;
                            eInspectionResult = ResulType.FailLeadPkgDimension;
                            m_arrErrorMessage[i] = strPosition + m_arrErrorMessage[i];

                            if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                                if (m_intTCPIPResultID == -1)
                                    m_intTCPIPResultID = (int)TCPIPResulID.Fail;//FailPackageDimension;

                            break;
                        }
                    }
                }
              

                m_arr5SPackagedSizeChecked[i] = true;

                if (!m_arr5SImageRotated2[i][intPkgDefectImageIndex])
                {
                    // Rotate to 0 deg for middle Lead
                    RotateImagesTo0Degree_MultiThreading(i, intPkgDefectImageIndex);
                }

                m_smVisionInfo.g_arrLead3D[i].ref_blnViewPkgSizeDrawing = true; // 20-05-2019 ZJYEOH: Changed to true to draw gauge during production
            }


            if (intLeadTestMask == 1)
            {
                m_fTiming2 = m_T2.Timing;
                m_strTrack_Center += ", C3b_" + intPkgDefectImageIndex.ToString() + "= " + (m_fTiming2 - m_fTimingPrev2).ToString();
                m_fTimingPrev2 = m_fTiming2;
            }

            if (!bTestFail)
            {
                // Loop Lead Test From Middle, Up, Right, Down to Left
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {
                    if (i != 0)
                        break;

                    if ((intLeadTestMask & (0x01 << i)) == 0)
                        continue;
                    
                    m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = true;

                    // Identify Lead defination for displaying fail message
                    string strPosition = GetROIDefinition(i);

                    if (!IsPackageOK_MultiThreading(i, intPkgDefectImageIndex))
                    {
                        bTestFail = true;

                        eInspectionResult = ResulType.FailLeadPkgDefect;

                        m_arrErrorMessage[i] = strPosition + m_smVisionInfo.g_arrLead3D[i].GetPackageFailTestDisplayResult(strPosition);
                        break;
                    }
                }
            }

            if (intLeadTestMask == 1)
            {
                m_fTiming2 = m_T2.Timing;
                m_strTrack_Center += ", C3c_" + intPkgDefectImageIndex.ToString() + "= " + (m_fTiming2 - m_fTimingPrev2).ToString();
                m_fTimingPrev2 = m_fTiming2;
            }

            if (bTestFail)
            {
                if (m_smCustomizeInfo.g_blnWantUseTCPIPIO)
                    if (m_intTCPIPResultID == -1)
                        m_intTCPIPResultID = (int)TCPIPResulID.Fail;//FailPackage;
            }

            return bTestFail;
        }

        private bool IsPackageSizeOK_MultiThreading(int intLeadIndex)
        {
            // 2019 06 18 - This function is for Package check package size. So must use gauge tool whether blnWantGaugeMeasurePkgSize is true or not
            //if (!m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantGaugeMeasurePkgSize && ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                return true;

            //if (!m_smVisionInfo.g_blnCheckPackage)
            //    return true;

            if ((m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_intFailPkgOptionMask & 0x01) == 0)
                return true;

            // Get unit size setting
            float fWidthMin = m_smVisionInfo.g_arrLead3D[0].GetUnitWidthMin(1);
            float fWidthMax = m_smVisionInfo.g_arrLead3D[0].GetUnitWidthMax(1);
            float fHeightMin = m_smVisionInfo.g_arrLead3D[0].GetUnitHeightMin(1);
            float fHeightMax = m_smVisionInfo.g_arrLead3D[0].GetUnitHeightMax(1);
            //float fThicknessMin = m_smVisionInfo.g_arrLead3D[0].GetUnitThicknessMin(1);
            //float fThicknessMax = m_smVisionInfo.g_arrLead3D[0].GetUnitThicknessMax(1);

            bool blnTestFail = false;
            if (m_blnCustomWantPackage)
            {
                switch (intLeadIndex)
                {
                    case 0:
                        {
                            float fWidth = (m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultDownWidth_RectGauge4L(1)) / 2;

                            //// 2019-10-25 ZJYEOH : Add Offset to package width
                            //fWidth += m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fPackageWidthOffsetMM;

                            if (fWidth < fWidthMin || fWidth > fWidthMax)
                            {
                                m_arrErrorMessage[intLeadIndex] += "*Package Size Width Fail. Min Tol=" + fWidthMin.ToString() + ", Max Tol=" + fWidthMax.ToString() +
                                    ", Current Size Width=" + fWidth.ToString();

                                m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_intFailPkgResultMask |= 0x01; //Fail package size

                                blnTestFail = true;
                            }

                            float fHeight = (m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[intLeadIndex].GetResultRightHeight_RectGauge4L(1)) / 2;

                            //// 2019-10-25 ZJYEOH : Add Offset to package height
                            //fHeight += m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fPackageHeightOffsetMM;

                            if (fHeight < fHeightMin || fHeight > fHeightMax)
                            {
                                m_arrErrorMessage[intLeadIndex] += "*Package Size Height Fail. Min Tol=" + fHeightMin.ToString() + ", Max Tol=" + fHeightMax.ToString() +
                                    ", Current Size Height=" + fHeight.ToString();

                                m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_intFailPkgResultMask |= 0x01; //Fail package size

                                blnTestFail = true;
                            }
                        }
                        break;

                        //// 2019 03 01 - JBTAN: Check pad thickness by using the average thickness of all side pads
                        //case 1:
                        //    float fThickness = 0;
                        //    float fTotalThickness = 0;
                        //    int intCount = 0;
                        //    for (int j = 1; j < m_smVisionInfo.g_arrLead3D.Length; j++)
                        //    {
                        //        if (j == 1 || j == 3)
                        //            fTotalThickness += m_smVisionInfo.g_arrLead3D[j].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[j].GetResultRightHeight_RectGauge4L(1);
                        //        else
                        //            fTotalThickness += m_smVisionInfo.g_arrLead3D[j].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[j].GetResultDownWidth_RectGauge4L(1);

                        //        intCount += 2;
                        //    }

                        //    fThickness = (float)Math.Round(fTotalThickness / intCount, 4, MidpointRounding.AwayFromZero);

                        //    // 2019-10-25 ZJYEOH : Add Offset to package thickness
                        //    fThickness += m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fPackageThicknessOffsetMM;

                        //    if (fThickness < fThicknessMin || fThickness > fThicknessMax)
                        //    {
                        //        m_arrErrorMessage[intLeadIndex] += "*Package Size Thickness Fail. Min Tol=" + fThicknessMin.ToString() + ", Max Tol=" + fThicknessMax.ToString() +
                        //            ", Current Size Thickness=" + fThickness.ToString();

                        //        m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_intFailPkgResultMask |= 0x01; //Fail package size

                        //        blnTestFail = true;
                        //    }
                        //    break;
                }
            }
            else if (m_blnCustomWantPositioning)
            {
                // Using positioning to check unit size
                float fPkgSizeWidth = 0, fPkgSizeHeight = 0;
                fPkgSizeWidth = m_smVisionInfo.g_objPositioning.ref_fObjectWidth / m_smVisionInfo.g_fCalibPixelX;
                fPkgSizeHeight = m_smVisionInfo.g_objPositioning.ref_fObjectHeight / m_smVisionInfo.g_fCalibPixelX;

                if ((fPkgSizeWidth < fWidthMin) || (fPkgSizeWidth > fWidthMax))
                {
                    m_arrErrorMessage[intLeadIndex] += "*Package Size Width Fail. Min Tol=" + fWidthMin.ToString() + ", Max Tol=" + fWidthMax.ToString() + ", Current Size Width=" + fPkgSizeWidth.ToString();
                    blnTestFail = true;
                }

                if ((fPkgSizeHeight < fHeightMin) || (fPkgSizeHeight > fHeightMax))
                {
                    m_arrErrorMessage[intLeadIndex] += "*Package Size Height Fail. Min Tol=" + fHeightMin.ToString() + ", Max Tol=" + fHeightMax.ToString() + ", Current Size Width=" + fPkgSizeHeight.ToString();
                    blnTestFail = true;
                }
            }

            if (blnTestFail)
            {
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                return false;
            }
            else
                return true;
        }
        private bool IsPackageSizeUsingCornerOK_MultiThreading(int intLeadIndex)
        {
            // 2019 06 18 - This function is for Package check package size. So must use gauge tool whether blnWantGaugeMeasurePkgSize is true or not
            //if (!m_smVisionInfo.g_arrPad[intPadIndex].ref_blnWantGaugeMeasurePkgSize && ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                return true;

            //if (!m_smVisionInfo.g_blnCheckPackage)
            //    return true;

            if ((m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_intFailPkgOptionMask & 0x01) == 0)
                return true;

            // Get unit size setting
            float fWidthMin = m_smVisionInfo.g_arrLead3D[0].GetUnitWidthMin(1);
            float fWidthMax = m_smVisionInfo.g_arrLead3D[0].GetUnitWidthMax(1);
            float fHeightMin = m_smVisionInfo.g_arrLead3D[0].GetUnitHeightMin(1);
            float fHeightMax = m_smVisionInfo.g_arrLead3D[0].GetUnitHeightMax(1);
            //float fThicknessMin = m_smVisionInfo.g_arrLead3D[0].GetUnitThicknessMin(1);
            //float fThicknessMax = m_smVisionInfo.g_arrLead3D[0].GetUnitThicknessMax(1);

            bool blnTestFail = false;
            if (m_blnCustomWantPackage)
            {
                switch (intLeadIndex)
                {
                    case 0:
                        {
                            float fWidth = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fCenterUnitWidthMM;

                            //// 2019-10-25 ZJYEOH : Add Offset to package width
                            //fWidth += m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fPackageWidthOffsetMM;

                            if (fWidth < fWidthMin || fWidth > fWidthMax)
                            {
                                m_arrErrorMessage[intLeadIndex] += "*Package Size Width Fail. Min Tol=" + fWidthMin.ToString() + ", Max Tol=" + fWidthMax.ToString() +
                                    ", Current Size Width=" + fWidth.ToString();

                                m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_intFailPkgResultMask |= 0x01; //Fail package size

                                blnTestFail = true;
                            }

                            float fHeight = m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fCenterUnitHeightMM;

                            //// 2019-10-25 ZJYEOH : Add Offset to package height
                            //fHeight += m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_fPackageHeightOffsetMM;

                            if (fHeight < fHeightMin || fHeight > fHeightMax)
                            {
                                m_arrErrorMessage[intLeadIndex] += "*Package Size Height Fail. Min Tol=" + fHeightMin.ToString() + ", Max Tol=" + fHeightMax.ToString() +
                                    ", Current Size Height=" + fHeight.ToString();

                                m_smVisionInfo.g_arrLead3D[intLeadIndex].ref_intFailPkgResultMask |= 0x01; //Fail package size

                                blnTestFail = true;
                            }
                        }
                        break;
                        
                }
            }
          
            if (blnTestFail)
            {
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                return false;
            }
            else
                return true;
        }

        public void LoadLeadLighting()
        {
            List<int> arrCOMList = new List<int>();
            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {
                bool blnFound = false;
                for (int c = 0; c < arrCOMList.Count; c++)
                {
                    if (arrCOMList[c] == m_smVisionInfo.g_arrLightSource[i].ref_intPortNo)
                    {
                        blnFound = true;
                        break;
                    }
                }

                if (!blnFound)
                    arrCOMList.Add(m_smVisionInfo.g_arrLightSource[i].ref_intPortNo);
            }

            //Set to stop mode
            for (int c = 0; c < arrCOMList.Count; c++)
                LEDi_Control.RunStop(arrCOMList[c], 0, false);  //LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);

            Thread.Sleep(10);
            for (int i = 0; i < m_intGrabRequire; i++)
            {
                int intValue1 = 0;
                int intValue2 = 0;
                int intValue3 = 0;
                int intValue4 = 0;
                int intValue5 = 0;
                int intValue6 = 0;
                int intValue7 = 0;
                int intValue8 = 0;

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
                                    case 3:
                                        if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                        {
                                            intValue4 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                        }
                                        break;
                                    case 4:
                                        if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                        {
                                            intValue5 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                        }
                                        break;
                                    case 5:
                                        if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                        {
                                            intValue6 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                        }
                                        break;
                                    case 6:
                                        if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                        {
                                            intValue7 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                        }
                                        break;
                                    case 7:
                                        if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                                        {
                                            intValue8 = m_smVisionInfo.g_arrLightSource[j].ref_arrValue[intValueNo];
                                        }
                                        break;

                                }

                                break;
                            }
                        }
                    }
                }

                // 2021 04 20 - CCENG: Reupdate intensity according to image display mode
                LEDi_Control.UpdateIntensityValueAccordingToImageDisplayMode(m_smVisionInfo.g_intImageDisplayMode, i,
                                                                            ref intValue1, ref intValue2, ref intValue3, ref intValue4,
                                                                            ref intValue5, ref intValue6, ref intValue7, ref intValue8);

                //Set all light source for sequence light controller for each grab
                //LEDi_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, i, intValue1, intValue2, intValue3, intValue4);

                if (arrCOMList.Count > 0)
                    LEDi_Control.SetSeqIntensity(arrCOMList[0], 0, i, intValue1, intValue2, intValue3, intValue4);
                if (arrCOMList.Count > 1)
                    LEDi_Control.SetSeqIntensity(arrCOMList[1], 0, i, intValue5, intValue6, intValue7, intValue8);

                Thread.Sleep(10);
            }
            for (int c = 0; c < arrCOMList.Count; c++)
                LEDi_Control.SaveIntensity(arrCOMList[c], 0); //LEDi_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0);
            Thread.Sleep(100);
            //Set to run mode
            for (int i = 0; i < arrCOMList.Count; i++)
                LEDi_Control.RunStop(arrCOMList[i], 0, true);   // LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
            Thread.Sleep(10);

        }

        public void LoadPHLighting()
        {
            List<int> arrCOMList = new List<int>();
            for (int i = 0; i < m_smVisionInfo.g_arrLightSource.Count; i++)
            {
                bool blnFound = false;
                for (int c = 0; c < arrCOMList.Count; c++)
                {
                    if (arrCOMList[c] == m_smVisionInfo.g_arrLightSource[i].ref_intPortNo)
                    {
                        blnFound = true;
                        break;
                    }
                }

                if (!blnFound)
                    arrCOMList.Add(m_smVisionInfo.g_arrLightSource[i].ref_intPortNo);
            }

            //Set to stop mode
            for (int c = 0; c < arrCOMList.Count; c++)
                LEDi_Control.RunStop(arrCOMList[c], 0, false);  //LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, false);

            Thread.Sleep(10);
            for (int i = 0; i < 1; i++)
            {
                int intValue1 = 0;
                int intValue2 = 0;
                int intValue3 = 0;
                int intValue4 = 0;
                int intValue5 = 0;
                int intValue6 = 0;
                int intValue7 = 0;
                int intValue8 = 0;

                for (int j = 0; j < m_smVisionInfo.g_arrLightSource.Count; j++)
                {

                    switch (j)
                    {
                        case 0:
                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                            {
                                intValue1 = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            }
                            break;
                        case 1:
                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                            {
                                intValue2 = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            }
                            break;
                        case 2:
                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                            {
                                intValue3 = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            }
                            break;
                        case 3:
                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                            {
                                intValue4 = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            }
                            break;
                        case 4:
                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                            {
                                intValue5 = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            }
                            break;
                        case 5:
                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                            {
                                intValue6 = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            }
                            break;
                        case 6:
                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                            {
                                intValue7 = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            }
                            break;
                        case 7:
                            if ((m_smVisionInfo.g_arrLightSource[j].ref_intSeqNo & (0x01 << i)) > 0)
                            {
                                intValue8 = m_smVisionInfo.g_arrLightSource[j].ref_PHValue;
                            }
                            break;
                    }

                }
                //Set all light source for sequence light controller for each grab
                //LEDi_Control.SetSeqIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, i, intValue1, intValue2, intValue3, intValue4);

                if (arrCOMList.Count > 0)
                    LEDi_Control.SetSeqIntensity(arrCOMList[0], 0, i, intValue1, intValue2, intValue3, intValue4);
                if (arrCOMList.Count > 1)
                    LEDi_Control.SetSeqIntensity(arrCOMList[1], 0, i, intValue5, intValue6, intValue7, intValue8);

                Thread.Sleep(10);
            }

            for (int c = 0; c < arrCOMList.Count; c++)
                LEDi_Control.SaveIntensity(arrCOMList[c], 0); //LEDi_Control.SaveIntensity(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0);
            Thread.Sleep(100);
            //Set to run mode
            for (int i = 0; i < arrCOMList.Count; i++)
                LEDi_Control.RunStop(arrCOMList[i], 0, true);   // LEDi_Control.RunStop(m_smVisionInfo.g_arrLightSource[0].ref_intPortNo, 0, true);
            Thread.Sleep(10);

        }

        public bool GrabImage_Sequence_SetIntensityForPH_Teli(int intGrabImageMask, bool blnForInspection)
        {
            if (!m_objTeliCamera.IsCameraInitDone())
            {
                m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = true;
                m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;
                m_smVisionInfo.g_strErrorMessage = "Camera No Connected";
                return true;
            }
            m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = false;
            // Using Teli Camera
            m_smVisionInfo.g_objGrabTime.Start();

            Thread.Sleep(m_smVisionInfo.g_intCameraGrabDelay);

            bool blnSuccess = true;
            //bool blnSeparateGrab = m_smVisionInfo.g_blnSeparateGrab;
            //int intSelectedImage = m_smVisionInfo.g_intSelectedImage;

            for (int i = 0; i < 2; i++) //m_intGrabRequire
            {
                //if (blnSeparateGrab)
                //{
                //    if (i != intSelectedImage)
                //        continue;
                //}

                if (i > 0) // for when grabbing second image and third image and forth image
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
                                SetExtraGain(i - 1, m_smVisionInfo.g_arrDebugImages);
                                ImageMerge(i - 1, m_smVisionInfo.g_arrDebugImages);
                                ImageUniformize(i - 1, m_smVisionInfo.g_arrDebugImages);
                            }
                            else
                            {
                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, i - 1);
                                m_smVisionInfo.g_arrImages[i - 1].AddGain(m_smVisionInfo.g_arrImageGain[i - 1]);
                                SetExtraGain(i - 1);
                                ImageMerge(i - 1);
                                ImageUniformize(i - 1);
                            }
                        }
                    }
                    else
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }

                    if (i == 1)
                    {
                        if (blnSuccess)
                            m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = true;
                        m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;
                    }
                    //else if (i == 2)
                    //{
                    //    m_bGrabImage2Done = true;
                    //}
                    //else if (i == 3)
                    //{
                    //    m_bGrabImage3Done = true;
                    //}
                }

                // Set light source channel ON/OFF

                // Set camera gain
                if (m_uintPHCameraGainPrev != m_smVisionInfo.g_uintPHCameraGain)
                {
                    m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_uintPHCameraGain);
                    m_uintPHCameraGainPrev = m_smVisionInfo.g_uintPHCameraGain;
                    m_intCameraGainPrev = m_smVisionInfo.g_uintPHCameraGain;
                }

                // Set camera shuttle
                if (m_fPHCameraShuttlePrev != m_smVisionInfo.g_fPHCameraShuttle)
                {
                    m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_fPHCameraShuttle);
                    m_fPHCameraShuttlePrev = m_smVisionInfo.g_fPHCameraShuttle;
                    m_fCameraShuttlePrev = m_smVisionInfo.g_fPHCameraShuttle;
                }

                if (blnSuccess)//2021-10-21 ZJYEOH : No need to grab anymore if not success, as this will reset the camera error message
                {
                    if (!m_objTeliCamera.Grab())
                    {
                        blnSuccess = false;
                        m_blnForceStopProduction = true;
                    }
                }

                Thread.Sleep(10);   // 2019 08 01 - CCENG: delay 10 ms before set grab done.

                SetGrabDone(blnForInspection);
            }

            if (!m_objTeliCamera.WaitFrameReady())
            {
                blnSuccess = false;
                m_blnForceStopProduction = true;
            }

            if (blnSuccess)
            {
                SetGrabDone(blnForInspection);
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
                                SetExtraGain(m_intGrabRequire - 1, m_smVisionInfo.g_arrDebugImages);
                                ImageMerge(m_intGrabRequire - 1, m_smVisionInfo.g_arrDebugImages);
                                ImageUniformize(m_intGrabRequire - 1, m_smVisionInfo.g_arrDebugImages);
                            }
                            else
                            {
                                m_smVisionInfo.g_objMemoryImage.CopyTo(ref m_smVisionInfo.g_arrImages, m_intGrabRequire - 1);
                                m_smVisionInfo.g_arrImages[m_intGrabRequire - 1].AddGain(m_smVisionInfo.g_arrImageGain[m_intGrabRequire - 1]);
                                SetExtraGain(m_intGrabRequire - 1);
                                ImageMerge(m_intGrabRequire - 1);
                                ImageUniformize(m_intGrabRequire - 1);
                            }
                        }
                        if (m_intGrabRequire == 1)
                        {
                            if (blnSuccess)
                                m_bGrabImage1Result = m_bGrabImage2Result = m_bGrabImage3Result = m_bGrabImage4Result = m_bGrabImage5Result = m_bGrabImage6Result = m_bGrabImage7Result = true;
                            m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;
                        }
                    }
                }
            }
            else
                SetGrabDone(blnForInspection);

            m_bGrabImage1Done = m_bGrabImage2Done = m_bGrabImage3Done = m_bGrabImage4Done = m_bGrabImage5Done = m_bGrabImage6Done = m_bGrabImage7Done = true;

            //Reset outport
            if (!m_smCustomizeInfo.g_blnMixController)
            {
                if (m_smCustomizeInfo.g_blnLEDiControl)
                {
                    m_objTeliCamera.OutPort(1, 3);
                    m_objTeliCamera.OutPort(1, 0);
                }
                else if (m_smCustomizeInfo.g_blnVTControl)
                {
                    m_objTeliCamera.OutPort(1, 0);
                }
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

                return false;
            }
            else
            {
                // Set camera gain
                if (m_uintPHCameraGainPrev != m_smVisionInfo.g_uintPHCameraGain)
                {
                    m_objTeliCamera.SetCameraParameter(2, m_smVisionInfo.g_uintPHCameraGain);
                    m_uintPHCameraGainPrev = m_smVisionInfo.g_uintPHCameraGain;

                }

                // Set camera shuttle
                if (m_fPHCameraShuttlePrev != m_smVisionInfo.g_fPHCameraShuttle)
                {
                    m_objTeliCamera.SetCameraParameter(1, m_smVisionInfo.g_fPHCameraShuttle);
                    m_fPHCameraShuttlePrev = m_smVisionInfo.g_fPHCameraShuttle;

                }

                if (m_smVisionInfo.g_bImageStatisticAnalysisON)
                {
                    m_smVisionInfo.g_bImageStatisticAnalysisUpdateInfo = true;

                    HiPerfTimer timesout = new HiPerfTimer();
                    timesout.Start();

                    while (true)
                    {
                        if (!m_smVisionInfo.g_bImageStatisticAnalysisUpdateInfo)
                            break;

                        if (timesout.Timing > 10000)
                        {
                            STTrackLog.WriteLine(">>>>>>>>>>>>> time out 2");
                            break;
                        }

                        Thread.Sleep(1);
                    }
                }

                //AttachImageToROI();
                m_smVisionInfo.g_blnLoadFile = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                m_smVisionInfo.g_objTransferTime.Stop();

                return true;
            }
        }

        private void SetExtraGain(int intImageNo)
        {
            for (int i = 0; i < 5; i++)
            {
                ROI objROI = new ROI();
                try
                {
                    objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionY,
                        m_smVisionInfo.g_arrSystemROI[i].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[i].ref_ROIHeight);
                    objROI.AttachImage(m_smVisionInfo.g_arrImages[intImageNo]);
                    objROI.AddExtraGain(m_smVisionInfo.g_arrImageExtraGain[intImageNo][i]);
                    objROI.Dispose();
                }
                catch
                {
                    if (objROI != null)
                        objROI.Dispose();
                }
            }

        }

        private void SetExtraGain(int intImageNo, List<ImageDrawing> arrImage)
        {
            for (int i = 0; i < 5; i++)
            {
                ROI objROI = new ROI();
                try
                {
                    objROI.LoadROISetting(m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[i].ref_ROIPositionY,
                        m_smVisionInfo.g_arrSystemROI[i].ref_ROIWidth, m_smVisionInfo.g_arrSystemROI[i].ref_ROIHeight);
                    objROI.AttachImage(arrImage[intImageNo]);
                    objROI.AddExtraGain(m_smVisionInfo.g_arrImageExtraGain[intImageNo][i]);
                    objROI.Dispose();
                }
                catch
                {
                    if (objROI != null)
                        objROI.Dispose();
                }
            }
        }

        private void WaitEventDone(ref bool bTriggerEvent, bool bBreakResult, int intTimeout, string strTrackName)
        {
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            while (true)
            {
                if (bTriggerEvent == bBreakResult)
                {
                    //STTrackLog.WriteLine("Time = " + timeout.Timing.ToString());
                    return;
                }

                if (timeout.Timing > intTimeout)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 9 - " + strTrackName);
                    STTrackLog.WriteLine("Waiting event timeout");
                    bTriggerEvent = bBreakResult;
                    break;
                }
                Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
            }

            timeout.Stop();
        }
        private bool WaitEventDone(ref bool bTriggerEvent, bool bBreakResult, ref bool bReturnResult, int intTimeout, string strTrackName)
        {
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            while (true)
            {
                if (bTriggerEvent == bBreakResult)
                {
                    return bReturnResult;
                }

                if (timeout.Timing > intTimeout)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 10 - " + strTrackName);
                    STTrackLog.WriteLine("Waiting event timeout");
                    bTriggerEvent = bBreakResult;
                    break;
                }
                Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
            }

            timeout.Stop();
            return false;
        }
        private void WaitDrawingLockEventDone()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                HiPerfTimer timesout = new HiPerfTimer();
                timesout.Start();

                while (true)
                {
                    if (m_smVisionInfo.g_arrLead3D[i].ref_blnDrawingLock == false)
                    {
                        break;
                    }

                    if (timesout.Timing > 10000)
                    {
                        STTrackLog.WriteLine(">>>>>>>>>>>>> time out 10");
                        break;
                    }

                    Thread.Sleep(1);    // 2018 10 01 - CCENG: Dun use Sleep(0) as it may cause other internal thread hang especially during waiting for grab image done. (Grab frame timeout happen)
                }
            }
        }

        private void RecordCPK()
        {
            // Check CPK turn ON or not
            if (!m_smVisionInfo.g_blnCPKON)
                return;

            if (m_smVisionInfo.g_objCPK.ref_intTestedTotal >= m_smVisionInfo.g_intCPKTestCount)
                return;

            try
            {
                //string strSampleBlobsFeatures;
                //string[] strSampleFeature = new string[100];

                int intRecordIndex = 0;
                string strLeadName = "";
                bool blnRecordFail = true;
                for (int p = 0; p < m_smVisionInfo.g_arrLead3D.Length; p++)
                {
                    switch (p)
                    {
                        case 0:
                            strLeadName = "Middle Lead ";
                            break;
                        case 1:
                            strLeadName = "Top Lead ";
                            break;
                        case 2:
                            strLeadName = "Right Lead ";
                            break;
                        case 3:
                            strLeadName = "Bottom Lead ";
                            break;
                        case 4:
                            strLeadName = "Left Lead ";
                            break;
                    }

                    int intLeadNumber = m_smVisionInfo.g_arrLead3D[p].GetBlobsFeaturesNumber();

                    for (int i = 0; i < intLeadNumber; i++)
                    {
                        List<string> arrResultList = new List<string>();
                        arrResultList = m_smVisionInfo.g_arrLead3D[p].GetBlobFeaturesResult_WithPassFailIndicator(i);

                        int intFailMask = Convert.ToInt32(arrResultList[arrResultList.Count - 1]);

                        if (intFailMask != 0)
                        {
                            //only record 1 fail criteria per unit
                            if (blnRecordFail)
                            {
                                blnRecordFail = false;
                                m_smVisionInfo.g_objCPK.RecordPassFail(intFailMask);
                            }
                        }

                        for (int j = 0; j < arrResultList.Count; j++)
                        {
                            if (arrResultList[j] == "---")
                                arrResultList[j] = "-999";
                        }

                        if (!m_smVisionInfo.g_objCPK.Record(0, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(arrResultList[0])))
                            return;
                        if (!m_smVisionInfo.g_objCPK.Record(1, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(arrResultList[1])))
                            return;
                        if (!m_smVisionInfo.g_objCPK.Record(2, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(arrResultList[2])))
                            return;
                        if (!m_smVisionInfo.g_objCPK.Record(3, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(arrResultList[3])))
                            return;
                        if (!m_smVisionInfo.g_objCPK.Record(4, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(arrResultList[4])))
                            return;
                        if (!m_smVisionInfo.g_objCPK.Record(5, intRecordIndex, strLeadName + (i + 1).ToString(), Convert.ToSingle(arrResultList[5])))
                            return;

                        intRecordIndex++;
                    }
                }

                m_smVisionInfo.g_objCPK.NextIndex();
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Vision 3 CPK Error: " + ex.ToString());
            }
        }
        private bool WantSaveImageAccordingMergeType(int intImageIndex)
        {
            switch (m_smVisionInfo.g_intImageMergeType)
            {
                case 0:
                    return true;
                    break;
                case 1:
                    if (intImageIndex == 1)
                        return false;
                    break;
                case 2:
                    if ((intImageIndex == 1) || (intImageIndex == 2))
                        return false;
                    break;
                case 3:
                    if ((intImageIndex == 1) || (intImageIndex == 3))
                        return false;
                    break;
                case 4:
                    if ((intImageIndex == 1) || (intImageIndex == 2) || (intImageIndex == 4))
                        return false;
                    break;
            }

            return true;
        }
    }
}
