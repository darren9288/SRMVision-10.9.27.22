using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;    
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Microsoft.Win32;
using AutoMode;
using IOMode;
using User;
using Common;
using History;
using ImageAcquisition;
using IOEx;
using Lighting;
using SharedMemory;
using VisionProcessing;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif

namespace SRMVision
{
    public partial class MainForm : Form
    {
        #region DllImport



        [DllImport("winmm.dll")]
        public static extern int timeBeginPeriod(int intPeriod);

        [DllImport("winmm.dll")]
        public static extern int timeEndPeriod(int intPeriod);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

#if(RTXDebug || RTXRelease)
        [DllImport("SRMRtx.dll", CharSet = CharSet.Auto)]
        public static extern long RunUtility();

        [DllImport("SRMRtx.dll", CharSet = CharSet.Auto)]
        public static extern long KillRunUtility(long lRtssPid1);

        [DllImport("SRMRtx.dll", CharSet = CharSet.Auto)]
        public static extern long SRMCreateAllSMemory();

        [DllImport("SRMRtx.dll", CharSet = CharSet.Auto)]
        public static extern long SRMCreateEvent(bool initiallyOwn, bool manualReset, [MarshalAs(UnmanagedType.LPStr)] string eventName);

        [DllImport("SRMRtx.dll", CharSet = CharSet.Auto)]
        public static extern bool SRMSingleLock([MarshalAs(UnmanagedType.LPStr)] string valueName);

#endif

        #endregion

        #region Constant Variables

        private const int SC_SIZE = 0xF000;
        private const int SC_MOVE = 0xF010;
        private const int SC_MINIMIZE = 0xF020;
        private const int SC_RESTORE = 0xF120;
        private const int SW_HIDE = 0;
        private const int SW_RESTORE = 5;

        #endregion

        #region Member Variables

        private static int iHighSpeed = timeBeginPeriod(1);

        private bool m_blnMachineStatusPrev = false;

        private bool m_blnAutoWnd = false;
        private bool m_blnAutoWndPrev = false;
        private bool m_blnIOWnd = false;
        private bool m_blnIOWndPrev = false;
        private bool m_blnHistoryWnd = false;
        private bool m_blnHistoryWndPrev = false;
        private bool m_blnUserWnd = false;
        private bool m_blnUserWndPrev = false;
        private bool m_blnIOMapWnd = false;
        private bool m_blnIOMapWndPrev = false;
        private bool m_blnDiagnosticWnd = false;
        private bool m_blnDiagnosticWndPrev = false;
        private bool m_blnUpsOn = false;
        private bool m_blnCheckShutDown = true;
        private bool m_blnStartShutDownTimer = false;
        private bool m_blnRestartApplication = false;

        private int m_intMousePositionX = 0, m_intMousePositionXPrev = 0;
        private int m_intMousePositionY = 0, m_intMousePositionYPrev = 0;
        private int m_intMousePixel = 0, m_intMousePixelPrev = 0;
        private int[] m_arrMouseRGBPixel = new int[3], m_arrMouseRGBPixelPrev = new int[3];
        private int m_intUserGroup = 5;
        private int m_intCalendarType;
        private int m_intMTBAStartTime;
        private int m_intMTBAStartDay;
        private int m_intMTBAStartMinute;
        private int m_intShiftNo;

        private string m_strUserName = "Operator";
        private string m_strRecipeID = "";

        private SplashForm m_objSplash = null;
        private ComThread m_thComThread;
        private THThread m_thThread;
        private SaveDataThread m_thSaveDataThread;
        private SaveRecipeThread m_thSaveRecipeThread;
        private AutoPurgeThread m_thAutoPurge;
        private SECSGEMThread m_thSECSGEMThread;
        private AVTVimba m_objImageAcquisition = new AVTVimba();
        private VisionIO m_objVisionIO;
        private RS232 m_thCOMMPort;
        private HiPerfTimer tShutDownTimer = new HiPerfTimer();
        private WaitShutdownForm objWait30sForm;
        private AutoForm m_autoMode = null;
        private static long rtssID;
        private SRMWaitingFormThread m_thWaitingFormThread;
        private string m_strSECSGEMSharedFolderPathPrev;
        private int m_intSECSGEMMaxNoOfCoplanPadPrev;
        private int m_intTrackDeleteLoopCount = 0;
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117
        }
        #endregion

        #region Shared Memory Variables

        private CustomOption m_smCustomizeInfo = new CustomOption();
        private ProductionInfo m_smProductionInfo = new ProductionInfo();
        private VisionInfo[] m_smVSInfo = new VisionInfo[10];
        private VisionComThread[] m_smVSComThread = new VisionComThread[10];
        private TCPIPIO[] m_smVSTCPIPIO = new TCPIPIO[10];

        #endregion

        public MainForm(bool blnRegister)
        {
#if (RELEASE || RTXRelease || Release_2_12)
            if (!m_smProductionInfo.g_blnLoadSRMVisionForRefreshing)
                StartSplash();
#endif

            InitializeComponent();

            try
            {
                STTrackLog.WriteLine("Mainform 1");

                CheckDongle();

                STTrackLog.WriteLine("Mainform 2");
                if (!blnRegister)
                    CheckCamera();

                STTrackLog.WriteLine("Mainform 3");
                InitEnvironment();
                STTrackLog.WriteLine("Mainform 4");
                InitVisionInfo();

                STTrackLog.WriteLine("Mainform 5");

                Login(true);

                STTrackLog.WriteLine("Mainform 6");
                m_smProductionInfo.AT_SF_ScanningIO = true;
                if (!m_smCustomizeInfo.g_blnShareHandlerPC)
                {
                    //start io scan thread
                    if (!IO.StartScanInput(m_smCustomizeInfo.g_blnUseUSBIOCard))
                    {
#if (RELEASE || RTXRelease || Release_2_12)
                        if (!m_smProductionInfo.g_blnLoadSRMVisionForRefreshing)
                        {
                            CloseSplash();
                            if (SRMMessageBox.Show("IO Card Failure!\nDo you want to continue?", "IO Card Failure", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.No)
                            {
                                Application.Exit();
                                Thread.Sleep(10);
                                Environment.Exit(Environment.ExitCode);
                            }
                        }
#endif
                    }
                    else
                    {
                        m_objVisionIO = new VisionIO("General", "", 0, 0, "", 0);
                    }
                }
                m_smProductionInfo.AT_SF_ScanningIO = false;

                STTrackLog.WriteLine("Mainform 7");
                if (!m_smCustomizeInfo.g_blnMixController)
                {
                    if (m_smCustomizeInfo.g_blnLEDiControl)
                        LEDi_Control.Init();
                    else if (m_smCustomizeInfo.g_blnVTControl)
                        VT_Control.Init();
                    else
                        TCOSIO_Control.Init();
                }
                else
                {
                    // 2018 07 13 - CCENG: Temporary force to use VTControl
                    LEDi_Control.Init();
                    VT_Control.Init();
                }

                STTrackLog.WriteLine("Mainform 8");
                m_smProductionInfo.AT_SF_LoadInterface = true;
                OpenAutoMode();
                m_smProductionInfo.AT_SF_LoadInterface = false;

                STTrackLog.WriteLine("Mainform 9");
                if (m_smProductionInfo.g_blnLoadSRMVisionForRefreshing)
                {
                    // Reload images opened by user
                    string strTempFolderDirectory = "C:\\TempFolder\\";
                    if (Directory.Exists(strTempFolderDirectory))
                    {
                        for (int i = 0; i < m_smVSInfo.Length; i++)
                        {
                            if (m_smVSInfo[i] == null)
                                continue;

                            for (int j = 0; j < m_smVSInfo[i].g_arrImages.Count; j++)
                            {
                                string strFileName = strTempFolderDirectory + "Image_" + i.ToString() + "_" + j.ToString() + ".bmp";
                                if (File.Exists(strTempFolderDirectory + "Image_" + i.ToString() + "_" + j.ToString() + ".bmp"))
                                    m_smVSInfo[i].g_arrImages[j].LoadImage(strTempFolderDirectory + "Image_" + i.ToString() + "_" + j.ToString() + ".bmp");
                            }

                            m_smVSInfo[i].ALL_VM_UpdatePictureBox = true;
                        }
                    }
                }

                STTrackLog.WriteLine("Mainform 10");

#if (RELEASE || RTXRelease || Release_2_12)
                SetSoftwareToPriorityRealTime(true);
#endif

                STTrackLog.WriteLine("Mainform 11");
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString(), "Init SRM Vision Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CloseSplash();

            //2020-06-01 ZJYEOH : Only trigger this when Release mode, as it will cause lagging when hit the breakpoint
#if (RELEASE || RTXRelease || Release_2_12)
            MouseHook.Start();
            MouseHook.MouseAction += new EventHandler(Event);
#endif
        }

        private void SetSoftwareToPriorityRealTime(bool blnEnableOtherProcessToLowPriority)
        {
            int intCoreCount = Environment.ProcessorCount;
            int intAffinity = 0;
            int intStarti = 0;
            if (blnEnableOtherProcessToLowPriority)
                intStarti = 1;
            for (int i = intStarti; i < intCoreCount; i++)
            {
                intAffinity += (0x01 << i);
            }
            TrackLog objTL = new TrackLog();
            objTL.WriteLine("intAffinity=" + intAffinity.ToString());
            Process[] runningProcess = Process.GetProcesses();
            foreach (Process proc in runningProcess)
            {
                if (proc.ProcessName.IndexOf("svchost") >= 0)
                {
                    STTrackLog.WriteLine("ID=" + proc.Id.ToString() + ", StartTime=" + proc.StartTime.ToString());
                }
                if (proc.ProcessName.IndexOf("SRMVision") >= 0)
                {
                    proc.PriorityBoostEnabled = true; ;
                    proc.PriorityClass = ProcessPriorityClass.RealTime;
                    //proc.ProcessorAffinity = (IntPtr)intAffinity;


                    //objTL.WriteLine("Thread Count = " + proc.Threads.Count + ". Affinity=" + intAffinity.ToString());
                    //for (int i = 0; i < proc.Threads.Count; i++)
                    //{
                    //    bool blnFound = false;
                    //    int intHexCore = 0x01;
                    //    for (int s = 0; s < ProcessTh.arrDifferentThreadName.Count; s++)
                    //    {

                    //        if (proc.Threads[i].Id.ToString() == ProcessTh.arrDifferentThreadName[s])
                    //        {
                    //            intHexCore = ProcessTh.arrThreadHexCore[s];
                    //            blnFound = true;
                    //            break;
                    //        }
                    //    }

                    //    try
                    //    {
                    //        if (blnFound)
                    //        {
                    //            objTL.WriteLine("Found Thread " + i.ToString() + " = " + proc.Threads[i].Id.ToString() + ". HexCore=" + intHexCore);

                    //            proc.Threads[i].ProcessorAffinity = (IntPtr)intHexCore;
                    //            proc.Threads[i].PriorityLevel = ThreadPriorityLevel.TimeCritical;
                    //        }
                    //        else
                    //        {
                    //            objTL.WriteLine("NotFound Thread " + i.ToString() + " = " + proc.Threads[i].Id.ToString());

                    //            proc.Threads[i].ProcessorAffinity = (IntPtr)0x01;
                    //            proc.Threads[i].PriorityLevel = ThreadPriorityLevel.Normal;
                    //        }
                    //    }
                    //    catch
                    //    {

                    //    }
                    //}

                    //objTL.WriteLine("----------- Selected Thread -----------");

                    //for (int i = 0; i < ProcessTh.arrDifferentThreadName.Count; i++)
                    //{
                    //    objTL.WriteLine(ProcessTh.arrDifferentThreadName[i] + ", HexCode=" + ProcessTh.arrThreadHexCore[i].ToString());
                    //}
                }
                else
                {
                    try
                    {
                        proc.PriorityBoostEnabled = false;
                        if (blnEnableOtherProcessToLowPriority)
                        {
                            proc.PriorityClass = ProcessPriorityClass.Idle;
                            //proc.ProcessorAffinity = (IntPtr)0x01;

                            //int intThreadCount = proc.Threads.Count;
                            //for (int i = 0; i < intThreadCount; i++)
                            //{
                            //    proc.Threads[i].ProcessorAffinity = (IntPtr)0x01;
                            //    proc.Threads[i].PriorityLevel = ThreadPriorityLevel.Lowest;
                            //}
                        }
                        else
                        {
                            proc.PriorityClass = ProcessPriorityClass.Normal;
                            proc.ProcessorAffinity = (IntPtr)intAffinity;
                            //int intThreadCount = proc.Threads.Count;
                            //for (int i = 0; i < intThreadCount; i++)
                            //{
                            //    proc.Threads[i].ProcessorAffinity = (IntPtr)intAffinity;
                            //    proc.Threads[i].PriorityLevel = ThreadPriorityLevel.Normal;
                            //}
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        /// <summary>
        ///  NOT allow user to move, minimize, restore and resize the Main Windows
        /// </summary>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message message)
        {
            // Listen for operating system messages.
            switch (message.WParam.ToInt32() & 0xfff0)
            {
                case SC_SIZE:
                    break;
                case SC_MOVE:
                    break;
                case SC_MINIMIZE:
                    break;
                case SC_RESTORE:
                    break;
                default:
                    base.WndProc(ref message);
                    break;
            }
        }
        /// <summary>
        /// Disable function of some key on keyboard
        /// </summary>
        protected override bool ProcessCmdKey(ref Message message, Keys keyData)
        {
            if ((message.Msg) == 0x100 && ((int)message.LParam) == 0x190001)
            {
                if (backgroundWorker1.IsBusy != true)
                    backgroundWorker1.RunWorkerAsync();
                return base.ProcessCmdKey(ref message, keyData);
            }

            // Disable ALT + F4
            if ((message.Msg == 0x104) && (((int)message.LParam) == 0x203e0001))
                return true;
            return false;
        }



        /// <summary>
        /// Change interface language
        /// </summary>
        private void ChangeLanguage()
        {
            string strCultureName = "";

            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                strCultureName = "zh-CHS";
            else if (m_smCustomizeInfo.g_intLanguageCulture == 4)
                strCultureName = "zh-CHT";
            else
                strCultureName = "";

            LanguageSwitcher.Instance.ChangeLanguage(this, new CultureInfo(strCultureName));

            SetLanguageButton();

            XmlParser objFileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFileHandle.WriteSectionElement("General");   // start of General section
            objFileHandle.WriteElement1Value("Culture", m_smCustomizeInfo.g_intLanguageCulture.ToString());
            objFileHandle.WriteEndElement();   // end of Option section
        }
        /// <summary>
        /// Get all available camera that link to PC and get the whole list
        /// </summary>
        private void CheckCamera()
        {
            m_smProductionInfo.AT_SF_CheckingCamera = true;

            bool blnCameraExist = false;
#region AVT Camera Check

            if (m_objImageAcquisition.CheckCameras())
            {
                blnCameraExist = true;
#if (RELEASE || RTXRelease || Release_2_12)
                m_smProductionInfo.AT_SF_ValidatingCamera = true;
                if (!ValidateCamera())
                {                    
                    Application.Exit();
                    Thread.Sleep(10);
                    Environment.Exit(Environment.ExitCode);
                }
                m_smProductionInfo.AT_SF_ValidatingCamera = false;
#endif
            }

#endregion

            if (IDSuEyeCamera.CheckCamera() > 0)
            {
                blnCameraExist = true;
            }

            if (!blnCameraExist)
            {
#if (RELEASE || RTXRelease || Release_2_12)
                CloseSplash();
                SRMMessageBox.Show("Camera connection(s) not found", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                Thread.Sleep(10);
                Environment.Exit(Environment.ExitCode);
#endif
            }

            Thread.Sleep(100);
            m_smProductionInfo.AT_SF_CheckingCamera = false;


            //            m_smProductionInfo.AT_SF_CheckingCamera = true;
            //            if (!m_objImageAcquisition.CheckCameras())
            //            {
            //#if(RELEASE || RTXRelease || Release_2_12)    
            //                CloseSplash();
            //                SRMMessageBox.Show("Camera connection(s) not found", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                Application.Exit();
            //                Thread.Sleep(10);
            //                Environment.Exit(Environment.ExitCode);
            //#endif
            //            }
            //            else
            //            {
            //#if(RELEASE || RTXRelease || Release_2_12)               
            //                m_smProductionInfo.AT_SF_ValidatingCamera = true;
            //                if (!ValidateCamera())
            //                {                    
            //                    Application.Exit();
            //                    Thread.Sleep(10);
            //                    Environment.Exit(Environment.ExitCode);
            //                }
            //                m_smProductionInfo.AT_SF_ValidatingCamera = false;
            //#endif
            //            }
            //            Thread.Sleep(100);
            //            m_smProductionInfo.AT_SF_CheckingCamera = false;
        }
        /// <summary>
        /// Check whether eureysis dongle is plugged in PC
        /// </summary>
        private void CheckDongle()
        {
            m_smProductionInfo.AT_SF_CheckingDongle = true;
#if (Debug_2_12 || Release_2_12)
            if (!Easy.CheckLicense(Euresys.Open_eVision_2_12.LicenseFeatures.Features.EasyImage))
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (!Easy.CheckLicense(Euresys.Open_eVision_1_2.LicenseFeatures.Features.EasyImage))
#endif

            {
                CloseSplash();
                SRMMessageBox.Show("Dongle Not Found", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                Thread.Sleep(10);
                Environment.Exit(Environment.ExitCode);
            }

            RegistryKey Key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey1 = Key.CreateSubKey("SVG");
            string strSerial = subKey1.GetValue("HDDSerial", "").ToString();

            string strSavedData = String.Empty;
            string strRealData = String.Empty;
            //Retrieve HDD Serial from Registry
            if (strSerial != string.Empty)
                strSavedData = Crypto.Decrypt(strSerial, "SVG11888");

            //DriveListEx objListHDD = new DriveListEx();

            ////Retrieve Current HDD Serial
            //if (objListHDD.Load() > 0)
            //    strRealData = objListHDD[0].SerialNumber;

            ////Verify Both HDD Serial
            //if (String.Compare(strSavedData, strRealData) != 0)
            //{
            //    CloseSplash();
            //    SRMMessageBox.Show("New HDD Serial Number Mismatch", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    Application.Exit();
            //    Thread.Sleep(10);
            //    Environment.Exit(Environment.ExitCode);
            //}
            Thread.Sleep(100);
            m_smProductionInfo.AT_SF_CheckingDongle = false;

        }
        /// <summary>
        /// if software is not shut down properly previous time, prompt user about this because it may cause some data lost
        /// </summary>
        private void CheckProperShutDown()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG");
            if ((int)subKey.GetValue("ProperShutDown", 0) == 0)
            {
                TrackLog objTrackLog = new TrackLog();
                objTrackLog.WriteLine("Not shut down system in proper way last time");
            }

            subKey.SetValue("ProperShutDown", 0);
        }
        /// <summary>
        /// Close splash form when all loading information is done
        /// </summary>
        private void CloseSplash()
        {
            if (m_objSplash == null)
                return;

            m_objSplash.CloseSplash();
            m_objSplash.Dispose();
            m_objSplash = null;
        }

#if (RTXDebug || RTXRelease)
        /// <summary>
        /// Declare all RTX Event
        /// </summary>
        private void CreateAllRTXEvent()
        {
            if (m_smCustomizeInfo.g_blnShareHandlerPC)
            {
#region Vision module IO

                // ------------------------- Vision 1: M+O+Pkg Vision IO -------------------------
                // ----- InPort -----
                SRMCreateEvent(false, false, "MOPkg_SOV");
                // ----- OutPort -----
                SRMCreateEvent(false, true, "MOPkg_EOV");
                SRMCreateEvent(false, true, "MOPkg_Grabbing");
                SRMCreateEvent(false, false, "MOPkg_Pass");
                SRMCreateEvent(false, false, "MOPkg_OrientResult1");
                SRMCreateEvent(false, false, "MOPkg_OrientResult2");
                SRMCreateEvent(false, false, "MOPkg_PackageFail");

                // ------------------------- Vision 2: M + Pkg Vision IO -------------------------
                // ----- InPort -----
                SRMCreateEvent(false, true, "MPkg_SOV");
                // ----- OutPort -----
                SRMCreateEvent(false, true, "MPkg_EOV");
                SRMCreateEvent(false, true, "MPkg_Grabbing");
                SRMCreateEvent(false, true, "MPkg_Pass");
                SRMCreateEvent(false, true, "MPkg_PackageFail");

                // ------------------------- Vision 3: Pad / 5s Pad Vision IO -------------------------
                // ----- InPort -----
                SRMCreateEvent(false, true, "Pad_SOV");
                // ----- OutPort -----
                SRMCreateEvent(false, true, "Pad_EOV");
                SRMCreateEvent(false, true, "Pad_Grabbing");
                SRMCreateEvent(false, true, "Pad_Pass");

                // ------------------------- Vision 4: InPocket 1 Vision IO --------------------------
                // ----- InPort -----
                SRMCreateEvent(false, true, "IPM1_SOV");
                SRMCreateEvent(false, true, "IPM1_CheckUnit1");
                SRMCreateEvent(false, true, "IPM1_CheckUnit2");
                SRMCreateEvent(false, true, "IPM1_Retest");
                SRMCreateEvent(false, true, "IPM1_Empty");
                SRMCreateEvent(false, true, "IPM1_RotatorSignal1");
                SRMCreateEvent(false, true, "IPM1_RotatorSignal2");
                SRMCreateEvent(false, true, "IPM1_PackageFail");
                SRMCreateEvent(false, true, "IPM1_EndOfRetest");

                // ----- OutPort -----
                SRMCreateEvent(false, true, "IPM1_EOV");
                SRMCreateEvent(false, true, "IPM1_Pass1");
                SRMCreateEvent(false, true, "IPM1_Pass2");


                // ------------------------- Vision 5: InPocket 2 Vision IO --------------------------
                // ----- InPort -----
                SRMCreateEvent(false, true, "IPM2_SOV");
                SRMCreateEvent(false, true, "IPM2_CheckUnit1");
                SRMCreateEvent(false, true, "IPM2_CheckUnit2");
                SRMCreateEvent(false, true, "IPM2_Retest");
                // ----- OutPort -----
                SRMCreateEvent(false, true, "IPM2_EOV");
                SRMCreateEvent(false, true, "IPM2_Pass1");
                SRMCreateEvent(false, true, "IPM2_Pass2");

                // ------------------------- Vision 6: PostSeal 1 Vision IO --------------------------
                // ----- InPort -----
                SRMCreateEvent(false, true, "PS1_SOV");
                SRMCreateEvent(false, true, "PS1_CheckPresence");
                // ----- OutPort -----
                SRMCreateEvent(false, true, "PS1_EOV");
                SRMCreateEvent(false, true, "PS1_Pass");

                // ------------------------- Vision 7: PostSeal 2 Vision IO --------------------------
                // ----- InPort -----
                SRMCreateEvent(false, true, "PS2_SOV");
                SRMCreateEvent(false, true, "PS2_CheckPresence");
                // ----- OutPort -----
                SRMCreateEvent(false, true, "PS2_EOV");
                SRMCreateEvent(false, true, "PS2_Pass");

#endregion

                // ------------------- COM --------------------------------------------------------
                SRMCreateEvent(false, false, "SentNewLotEvent");
                SRMCreateEvent(false, false, "SuccessInNewLotEvent");
                SRMCreateEvent(false, false, "FailInNewLotEvent");
                SRMCreateEvent(false, false, "APP2SRMVisionExitEvent");

                // ------------------- General ----------------------------------------------------
                SRMCreateEvent(false, false, "FailInLoadRecipeEvent");
            }
            else
            {
                SRMCreateEvent(false, false, "SendDataStart1");
                SRMCreateEvent(false, false, "SendDataStart2");
                SRMCreateEvent(false, false, "SendDataStart3");
                SRMCreateEvent(false, false, "SendDataStart4");
                SRMCreateEvent(false, false, "SendDataStart5");
                SRMCreateEvent(false, false, "SendDataStart6");
                SRMCreateEvent(false, false, "SendDataStart7");

                SRMCreateEvent(true, true, "SendDataDone1");
                SRMCreateEvent(true, true, "SendDataDone2");
                SRMCreateEvent(true, true, "SendDataDone3");
                SRMCreateEvent(true, true, "SendDataDone4");
                SRMCreateEvent(true, true, "SendDataDone5");
                SRMCreateEvent(true, true, "SendDataDone6");
                SRMCreateEvent(true, true, "SendDataDone7");
            }

        }

#endif
        /// <summary>
        /// Customize Menu through User access level
        /// </summary>
        private void CustomizeGUI()
        {
            //UserRight objUserRight = new UserRight();
            //int[] intGroupRights = objUserRight.GetGroupLevel1();

            if (m_smCustomizeInfo.objNewUserRight == null)
                m_smCustomizeInfo.objNewUserRight = new NewUserRight(true);
            int[] intGroupRights = m_smCustomizeInfo.objNewUserRight.GetTopMenuChild1GroupList();

            btn_Recipe.Visible = (m_intUserGroup <= intGroupRights[2]);
            if (m_smCustomizeInfo.g_blnShareHandlerPC)
            {
                btn_IO.Visible = false;
                btn_IOMap.Visible = false;
            }
            else
            {
                btn_IO.Visible = (m_intUserGroup <= intGroupRights[3]);
                btn_IOMap.Visible = (m_intUserGroup <= intGroupRights[4]);
            }

            btn_IO.Visible = (m_intUserGroup <= intGroupRights[3]);
            btn_IOMap.Visible = (m_intUserGroup <= intGroupRights[4]);
            btn_User.Visible = (m_intUserGroup <= intGroupRights[5]);
            btn_History.Visible = (m_intUserGroup <= intGroupRights[6]);
            btn_Language.Visible = (m_intUserGroup <= intGroupRights[7]);
            btn_Config.Visible = (m_intUserGroup <= intGroupRights[8]);
            btn_Option.Visible = (m_intUserGroup <= intGroupRights[9]);
            btn_Quit.Visible = (m_intUserGroup <= intGroupRights[10]);
            btn_Diagnostic.Visible = (m_intUserGroup <= intGroupRights[11]) && m_smCustomizeInfo.g_blnConfigShowTCPIC;
        }
        /// <summary>
        /// When different MdiChildren is entered, change user access right on Main Menu
        /// </summary>
        private void EnableMenuItem(object sender, bool blnEnable)
        {
            if (sender == btn_Auto)
            {
                if (m_blnIOWnd)
                    return;
                else if (blnEnable)
                {
                    btn_IO.Enabled = blnEnable;
                    btn_Diagnostic.Enabled = blnEnable;
                }
            }
            else if (sender == btn_IO || sender == btn_Diagnostic)
            {
                if (m_blnAutoWnd)
                    return;
            }
            else
            {
                btn_Auto.Enabled = blnEnable;
                btn_IO.Enabled = blnEnable;
                btn_Diagnostic.Enabled = blnEnable;
            }

            btn_History.Enabled = blnEnable;
            btn_Recipe.Enabled = blnEnable;
            btn_IOMap.Enabled = blnEnable;
            btn_User.Enabled = blnEnable;
            // 2020 03 13 - JBTAN: login button will always enable. If user inside auto form, refresh the page
            //btn_Login.Enabled = blnEnable;
            btn_Config.Enabled = blnEnable;
            btn_Language.Enabled = blnEnable;
            btn_Option.Enabled = blnEnable;

            if (blnEnable && (m_blnDiagnosticWnd))
                btn_Quit.Enabled = false;
            else
                btn_Quit.Enabled = blnEnable;
        }
        /// <summary>
        /// Before exit application, close all thread and object in proper way
        /// </summary>
        private void ExitApplication()
        {
            //stop all thread 
            if (!m_smCustomizeInfo.g_blnShareHandlerPC)
                IO.StopScanInput();

            StopThread();

            //Thread.Sleep(3000);
            if (m_objImageAcquisition != null)
                m_objImageAcquisition.OFFCamera();

            if (m_smCustomizeInfo.g_blnLEDiControl)
            {
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    for (int j = 0; j < m_smVSInfo[i].g_arrLightSource.Count; j++)
                    {
                        if (m_smVSInfo[i].g_arrLightSource[j] == null)
                            continue;

                        LightSource objLightSource = m_smVSInfo[i].g_arrLightSource[j];
                        LEDi_Control.ResetIntensity(objLightSource.ref_intPortNo, objLightSource.ref_intChannel);
                    }
                }

                LEDi_Control.Close();
            }
            else if (m_smCustomizeInfo.g_blnVTControl)
            {
                VT_Control.Close();
            }
            else
            {
                TCOSIO_Control.Close();
            }

            // 2018 07 13 - CCENG: Temporary force to use VTControl
            //VT_Control.Close();

            Form[] mdiChild = MdiChildren;
            //Make sure to ask for saving the doc before exiting the app
            for (int i = 0; i < mdiChild.Length; i++)
                mdiChild[i].Close();

            string strExitTime = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey1 = key.CreateSubKey("SVG");

            subKey1.SetValue("ExitTime", strExitTime);
            subKey1.SetValue("ProperShutDown", 1);


            timeEndPeriod(1);

            STTrackLog.WriteLine("ExitApplication Done.");
#if (RTXDebug || RTXRelease)
            // RtssKill Rtss process
            KillRunUtility(rtssID);
#endif

            if (File.Exists("D:\\Apps\\SpeedUpInitializeEuresys.exe"))
                Process.Start("D:\\Apps\\SpeedUpInitializeEuresys.exe");
        }
        /// <summary>
        /// Get existing record from registry
        /// </summary>
        private void GetAutoModeRegistry()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\AutoMode");

            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intPassTotal = (int)subKey.GetValue("VS" + (i + 1) + "Pass", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intOrientFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "OrientFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intPin1FailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "Pin1Fail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intMarkFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "MarkFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intPackageFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "PackageFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intPadFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "PadFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSealFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SealFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intBarcodeFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "BarcodeFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSealDistanceFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SealDistanceFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSealOverHeatFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SealOverHeatFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSealBrokenAreaFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SealBrokenAreaFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSealBrokenGapFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SealBrokenGapFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSealSprocketHoleFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SealSprocketHoleFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSealSprocketHoleDiameterFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SealSprocketHoleDiameterFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSealSprocketHoleDefectFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SealSprocketHoleDefectFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSealSprocketHoleBrokenFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SealSprocketHoleBrokenFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSealEdgeStraightnessFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SealEdgeStraightness", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSealSprocketHoleRoundnessFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SealSprocketHoleRoundnessFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCheckPresenceFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CheckPresenceFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intPositionFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "PositionFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intNoTemplateFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "NoTemplateFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intEdgeNotFoundFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "EdgeNotFoundFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intAngleFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "AngleFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intPkgDefectFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "PkgDefectFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intPkgColorDefectFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "PkgColorDefectFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadOffsetFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadOffsetFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadAreaFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadAreaFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadDimensionFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadDimensionFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadPitchGapFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadPitchGapFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadBrokenFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadBrokenFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadExcessFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadExcessFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadSmearFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadSmearFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadEdgeLimitFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadEdgeLimitFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadStandOffFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadStandOffFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadEdgeDistanceFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadEdgeDistanceFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadSpanFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadSpanFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadContaminationFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadContaminationFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadMissingFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadMissingFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPadColorDefectFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPadColorDefectFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadOffsetFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadOffsetFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadAreaFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadAreaFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadDimensionFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadDimensionFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadPitchGapFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadPitchGapFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadBrokenFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadBrokenFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadExcessFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadExcessFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadSmearFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadSmearFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadEdgeLimitFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadEdgeLimitFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadStandOffFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadStandOffFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadEdgeDistanceFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadEdgeDistanceFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadSpanFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadSpanFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadContaminationFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadContaminationFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadMissingFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadMissingFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePadColorDefectFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePadColorDefectFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPkgDefectFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPkgDefectFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intCenterPkgDimensionFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "CenterPkgDimensionFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePkgDefectFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePkgDefectFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intSidePkgDimensionFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "SidePkgDimensionFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadOffsetFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadOffsetFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadSkewFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadSkewFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadWidthFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadWidthFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadLengthFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadLengthFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadLengthVarianceFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadLengthVarianceFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadPitchGapFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadPitchGapFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadPitchVarianceFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadPitchVarianceFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadStandOffFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadStandOffFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadStandOffVarianceFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadStandOffVarianceFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadCoplanFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadCoplanFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadAGVFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadAGVFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadSpanFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadSpanFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadSweepsFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadSweepsFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadUnCutTiebarFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadUnCutTiebarFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadMissingFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadMissingFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadContaminationFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadContaminationFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadPkgDefectFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadPkgDefectFail", 0);
                m_smVSInfo[i].g_intTestedTotal += m_smVSInfo[i].g_intLeadPkgDimensionFailureTotal = (int)subKey.GetValue("VS" + (i + 1) + "LeadPkgDimensionFail", 0);

                m_smVSInfo[i].g_intPassImageCount = (int)subKey.GetValue("VS" + (i + 1) + "PassImageCount", 0);
                m_smVSInfo[i].g_intFailImageCount = (int)subKey.GetValue("VS" + (i + 1) + "FailImageCount", 0);
                m_smVSInfo[i].g_intTotalImageCount = (int)subKey.GetValue("VS" + (i + 1) + "TotalImageCount", 0);
                m_smVSInfo[i].g_intLowYieldUnitCount = (int)subKey.GetValue("VS" + (i + 1) + "LowYieldUnit", 0);
                m_smVSInfo[i].g_intContinuousPassUnitCount = (int)subKey.GetValue("VS" + (i + 1) + "ContinuousPassUnit", 0);
                m_smVSInfo[i].g_intContinuousFailUnitCount = (int)subKey.GetValue("VS" + (i + 1) + "ContinuousFailUnit", 0);

                m_smVSInfo[i].g_blnPocket1Pass = Convert.ToBoolean(subKey.GetValue("VS" + (i + 1) + "Pocket1", false));
                m_smVSInfo[i].g_blnPocket2Pass = Convert.ToBoolean(subKey.GetValue("VS" + (i + 1) + "Pocket2", false));
                m_smVSInfo[i].g_intScenario = (int)subKey.GetValue("VS" + (i + 1) + "Scenario", 0x01);

                // 2020 03 27 - JBTAN: Load Vision Reset Count info
                m_smVSInfo[i].g_intVisionResetCount = (int)subKey.GetValue("VS" + (i + 1) + "ResetCount", 0);
                m_smVSInfo[i].g_strVisionResetCountTime = subKey.GetValue("VS" + (i + 1) + "ResetCountTime", DateTime.Now.ToString("yyyyMMddHHmmss")).ToString();
            }
        }
        /// <summary>
        /// Read Option.xml file for general setting such as auto logout when enter autoMode form
        /// </summary>
        private void GetCustomizeOption()
        {
            // 20220706 Theam : To check whether option file corrupted or not
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFile.GetFirstSection("LightSource");
            int intMaxLimit = objFile.GetValueAsInt("LEDiMaxLimit", 31);
            if (intMaxLimit == 31 && File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Option_Backup.xml"))
            {
                if (SRMMessageBox.Show("Option file corrupted!! Do you want to recover from backup file?", "SRM Error Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Option_Backup.xml", AppDomain.CurrentDomain.BaseDirectory + "Option.xml", true);
                }
            }
            else if (intMaxLimit == 31)
                SRMMessageBox.Show("Option File Corrupted!! Please check all option settings!!");
            else if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Option_Backup.xml"))
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Option.xml", AppDomain.CurrentDomain.BaseDirectory + "Option_Backup.xml");
            else if (File.GetLastWriteTime(AppDomain.CurrentDomain.BaseDirectory + "Option.xml") != File.GetLastWriteTime(AppDomain.CurrentDomain.BaseDirectory + "Option_Backup.xml"))
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "Option.xml", AppDomain.CurrentDomain.BaseDirectory + "Option_Backup.xml", true);

            XmlParser objFileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFileHandle.GetFirstSection("General");
            m_smCustomizeInfo.g_intLanguageCulture = objFileHandle.GetValueAsInt("Culture", 1);
            m_smCustomizeInfo.g_strMachineID = objFileHandle.GetValueAsString("MachineID", "SRM Handler");
            m_smCustomizeInfo.g_blnAutoShutDown = objFileHandle.GetValueAsBoolean("AutoStop", false);
            m_smCustomizeInfo.g_blnAutoLogout = objFileHandle.GetValueAsBoolean("AutoLogout", false);
            m_smCustomizeInfo.g_blnShareHandlerPC = false; // objFileHandle.GetValueAsBoolean("ShareHandlerPC", false);// 2020-10-12 ZJYEOH : no use anymore so always set to false 
            m_smCustomizeInfo.g_blnUseUSBIOCard = objFileHandle.GetValueAsBoolean("UseUSBIOCard", false);
            m_smCustomizeInfo.g_blnShowSubLearnButton = objFileHandle.GetValueAsBoolean("ShowSubLearnButton", false);
            btn_Diagnostic.Visible = m_smCustomizeInfo.g_blnConfigShowTCPIC = objFileHandle.GetValueAsBoolean("ConfigShowTCPIP", false);
            if (m_smCustomizeInfo.g_blnConfigShowTCPIC)
                m_smCustomizeInfo.g_blnWantUseTCPIPIO = objFileHandle.GetValueAsBoolean("WantUseTCPIPAsIO", false);
            else
                m_smCustomizeInfo.g_blnWantUseTCPIPIO = false;
            m_smCustomizeInfo.g_blnConfigShowRS232 = objFileHandle.GetValueAsBoolean("ConfigShowRS232", false);
            m_smCustomizeInfo.g_blnConfigShowNetwork = objFileHandle.GetValueAsBoolean("ConfigShowNetwork", false);
            m_smCustomizeInfo.g_intMarkDefaultTolerance = objFileHandle.GetValueAsInt("MarkDefaultTolerance", 75);
            m_smCustomizeInfo.g_intResolutionWidth = objFileHandle.GetValueAsInt("ResolutionWidth", 640);
            m_smCustomizeInfo.g_intResolutionHeight = objFileHandle.GetValueAsInt("ResolutionHeight", 480);
            m_smCustomizeInfo.g_blnMixController = objFileHandle.GetValueAsBoolean("MixController", false);
            m_smCustomizeInfo.g_blnOnlyNewLot = objFileHandle.GetValueAsBoolean("OnlyNewLot", false);
            m_smCustomizeInfo.g_intOrientIO = objFileHandle.GetValueAsInt("OrientIO", 0);

            m_smProductionInfo.g_blnWantEditLog = objFileHandle.GetValueAsBoolean("WantEditLog", true);
            
            m_smProductionInfo.g_strHistoryDataLocation = objFileHandle.GetValueAsString("SaveHistoryDataLocation", "D:\\");
            m_smProductionInfo.g_blnWantBuypassIPM = objFileHandle.GetValueAsBoolean("WantBuypassIPM", false);
            m_smProductionInfo.g_blnWantNonRotateInspection = objFileHandle.GetValueAsBoolean("WantNonRotateInspection", false);
            m_smProductionInfo.g_blnBlankImageBeforeGrab = objFileHandle.GetValueAsBoolean("BlankImageBeforeGrab", false);
            m_smProductionInfo.g_blnDeleteImageDuringProduction = objFileHandle.GetValueAsBoolean("DeleteImageDuringProduction", false);
            m_smProductionInfo.g_blnShowGUIForBelowUserRight = objFileHandle.GetValueAsBoolean("ShowGUIForBelowUserRight", true);
            m_smProductionInfo.g_blnWantRecipeVerification = objFileHandle.GetValueAsBoolean("WantRecipeVerification", false);
            m_smProductionInfo.g_blnWantShowWhiteBalance = objFileHandle.GetValueAsBoolean("WantShowWhiteBalance", false);

            int intCount = 0;
            string strSelectedLocation = "";
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if ((m_smVSInfo[i].g_strVisionName.Contains("InPocket") || m_smVSInfo[i].g_strVisionName.Contains("IPM")))
                {
                    m_smVSInfo[i].g_blnWantNonRotateInspection = m_smProductionInfo.g_blnWantNonRotateInspection;
                }
                else
                {
                    m_smVSInfo[i].g_blnWantNonRotateInspection = false;
                }

                //15-9-2021 cxlim : Software location no needed anymore , default will change to D drive

                if (intCount == 0)
                {
                    strSelectedLocation = objFileHandle.GetValueAsString("V1SaveImageLocation", "D:\\");
                    switch (strSelectedLocation)
                    {
                        case "Software Location":
                            m_smVSInfo[i].g_strSaveImageLocation = "D:\\" + "SaveImage\\";
                            break;
                        default:
                            m_smVSInfo[i].g_strSaveImageLocation = strSelectedLocation + "SaveImage\\";
                            break;
                    }

                    intCount++;
                }
                else if (intCount == 1)
                {
                    strSelectedLocation = objFileHandle.GetValueAsString("V2SaveImageLocation", "D:\\");
                    switch (strSelectedLocation)
                    {
                        case "Software Location":
                            m_smVSInfo[i].g_strSaveImageLocation = "D:\\" + "SaveImage\\";
                            break;
                        default:
                            m_smVSInfo[i].g_strSaveImageLocation = strSelectedLocation + "SaveImage\\";
                            break;
                    }

                    intCount++;
                }
                else if (intCount == 2)
                {
                    strSelectedLocation = objFileHandle.GetValueAsString("V3SaveImageLocation", "D:\\");
                    switch (strSelectedLocation)
                    {
                        case "Software Location":
                            m_smVSInfo[i].g_strSaveImageLocation = "D:\\" + "SaveImage\\";
                            break;
                        default:
                            m_smVSInfo[i].g_strSaveImageLocation = strSelectedLocation + "SaveImage\\";
                            break;
                    }

                    intCount++;
                }
                else if (intCount == 3)
                {
                    strSelectedLocation = objFileHandle.GetValueAsString("V4SaveImageLocation", "D:\\");
                    switch (strSelectedLocation)
                    {
                        case "Software Location":
                            m_smVSInfo[i].g_strSaveImageLocation = "D:\\" + "SaveImage\\";
                            break;
                        default:
                            m_smVSInfo[i].g_strSaveImageLocation = strSelectedLocation + "SaveImage\\";
                            break;
                    }

                    intCount++;
                }
                else if (intCount == 4)
                {
                    strSelectedLocation = objFileHandle.GetValueAsString("V5SaveImageLocation", "D:\\");
                    switch (strSelectedLocation)
                    {
                        case "Software Location":
                            m_smVSInfo[i].g_strSaveImageLocation = "D:\\" + "SaveImage\\";
                            break;
                        default:
                            m_smVSInfo[i].g_strSaveImageLocation = strSelectedLocation + "SaveImage\\";
                            break;
                    }

                    intCount++;
                }
                else if (intCount == 5)
                {
                    strSelectedLocation = objFileHandle.GetValueAsString("V6SaveImageLocation", "D:\\");
                    switch (strSelectedLocation)
                    {
                        case "Software Location":
                            m_smVSInfo[i].g_strSaveImageLocation = "D:\\" + "SaveImage\\";
                            break;
                        default:
                            m_smVSInfo[i].g_strSaveImageLocation = strSelectedLocation + "SaveImage\\";
                            break;
                    }

                    intCount++;
                }
                else if (intCount == 6)
                {
                    strSelectedLocation = objFileHandle.GetValueAsString("V7SaveImageLocation", "D:\\");
                    switch (strSelectedLocation)
                    {
                        case "Software Location":
                            m_smVSInfo[i].g_strSaveImageLocation = "D:\\" + "SaveImage\\";
                            break;
                        default:
                            m_smVSInfo[i].g_strSaveImageLocation = strSelectedLocation + "SaveImage\\";
                            break;
                    }

                    intCount++;
                }
                else if (intCount == 7)
                {
                    strSelectedLocation = objFileHandle.GetValueAsString("V8SaveImageLocation", "D:\\");
                    switch (strSelectedLocation)
                    {
                        case "Software Location":
                            m_smVSInfo[i].g_strSaveImageLocation = "D:\\" + "SaveImage\\";
                            break;
                        default:
                            m_smVSInfo[i].g_strSaveImageLocation = strSelectedLocation + "SaveImage\\";
                            break;
                    }
                }
            }

            objFileHandle.GetFirstSection("LightSource");
            m_smCustomizeInfo.g_blnLEDiControl = objFileHandle.GetValueAsBoolean("LEDiControl", true);
            m_smCustomizeInfo.g_blnVTControl = objFileHandle.GetValueAsBoolean("VTControl", false);     // 2018 12 31 - CCENG: By defautl, only LEDiControl set to true, other control should set to false.
        }

        //For Teli
        private void InitializeCameraSystem()
        {
            TeliCamera.InitializeSystem();
        }

        private void InitializeLanguageLibrary()
        {
            LanguageLibrary.InitLibrary();
        }

        /// <summary>
        /// Initialize all variables and settings
        /// </summary>
        private void InitEnvironment()
        {
            m_smProductionInfo.AT_SF_Initializing = true;
#if (Debug_2_12 || Release_2_12)
            TSlbl_SoftwareVersion.Text = Application.ProductVersion + " (2.12)";
#else
            TSlbl_SoftwareVersion.Text = Application.ProductVersion;
#endif

            TSlbl_Time.Text = DateTime.Now.ToLongTimeString();
            TSlbl_Date.Text = DateTime.Now.ToLongDateString();

            CheckProperShutDown();
            ReadFromRegistry();
            GetCustomizeOption();
            GetAutoModeRegistry();

            InitializeCameraSystem();
            ChangeLanguage();
            InitializeLanguageLibrary();

#if (RTXDebug || RTXRelease)

            // -------- RTX -----------------------------
            if (!m_smCustomizeInfo.g_blnShareHandlerPC)
            {
                // Kill the existing running Rtx process
                KillRunUtility(1);
                // RtssRun the Rtss program
                rtssID = RunUtility();
            }

            SRMCreateAllSMemory();
            CreateAllRTXEvent();

#endif
            m_thThread = new THThread(m_smProductionInfo, m_smVSInfo);
            m_thAutoPurge = new AutoPurgeThread(m_smCustomizeInfo, m_smVSInfo,m_smProductionInfo);

            m_thComThread = new ComThread(m_smCustomizeInfo, m_smProductionInfo, m_smVSInfo);
            m_thSaveRecipeThread = new SaveRecipeThread(m_smProductionInfo, m_smCustomizeInfo, m_smVSInfo);

            if (m_smCustomizeInfo.g_intWant2DCode > 0)
            {
                //Save data for 2DCode
                m_thSaveDataThread = new SaveDataThread(m_smProductionInfo, m_smVSInfo, m_smCustomizeInfo);
            }

            InitSECSGEM();
            m_strSECSGEMSharedFolderPathPrev = m_smCustomizeInfo.g_strSECSGEMSharedFolderPath;
            m_intSECSGEMMaxNoOfCoplanPadPrev = m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad;
            m_thSECSGEMThread = new SECSGEMThread(m_smProductionInfo, m_smVSInfo, m_smCustomizeInfo);

            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "\\Option.xml");
            objFile.GetFirstSection("SerialPort");
            if (objFile.GetValueAsBoolean("Enable", false))
            {
                m_thCOMMPort = new RS232();
                m_thCOMMPort.Init(objFile.GetValueAsString("CommPort", "COM1"), objFile.GetValueAsInt("BitsPerSecond", 9600),
                    objFile.GetValueAsInt("DataBits", 8), objFile.GetValueAsInt("Parity", 0), objFile.GetValueAsInt("StopBits", 0));
            }

            InitSPC(true);

            InitTCPIP();

            InitNetwork();

            InitStaticVariables();

            //InitStaticVariables();    // 2020 08 17 - CCENG: Should not call this function twice

            m_smProductionInfo.AT_SF_Initializing = false;
        }

        private void InitStaticVariables()
        {
            // Get largest size of camera resolution of all visions.
            int intImageMaxWidth = 0;
            int intImageMaxHeight = 0;
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (intImageMaxWidth < m_smVSInfo[i].g_intCameraResolutionWidth)
                    intImageMaxWidth = m_smVSInfo[i].g_intCameraResolutionWidth;

                if (intImageMaxHeight < m_smVSInfo[i].g_intCameraResolutionHeight)
                    intImageMaxHeight = m_smVSInfo[i].g_intCameraResolutionHeight;
            }

            Shape.InitVariables(intImageMaxWidth, intImageMaxHeight);

            //Shape.InitVariables();
        }

        /// <summary>
        /// Get the start time of 1 day, start day of each week
        /// </summary>
        private void InitSPC(bool blnFirstInit)
        {
            XmlParser objFileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFileHandle.GetFirstSection("General");
            //m_smCustomizeInfo.g_blnDebugMode = objFileHandle.GetValueAsBoolean("DebugMode", false);
            //m_smCustomizeInfo.g_blnPreviousVersion = objFileHandle.GetValueAsBoolean("PreviousVersion", false);
            //m_smCustomizeInfo.g_intPreviousVersionIndex = objFileHandle.GetValueAsInt("PreviousVersionIndex", 0);
            m_smCustomizeInfo.g_blnSavePassImage = objFileHandle.GetValueAsBoolean("SavePassImage", false);
            m_smCustomizeInfo.g_blnSaveFailImage = objFileHandle.GetValueAsBoolean("SaveFailImage", true);
            m_smCustomizeInfo.g_blnSaveFailImageErrorMessage = objFileHandle.GetValueAsBoolean("SaveFailImageErrorMessage", true);
            m_smCustomizeInfo.g_intPassImagePics = objFileHandle.GetValueAsInt("PassImagePics", 100);
            m_smCustomizeInfo.g_intFailImagePics = objFileHandle.GetValueAsInt("FailImagePics", 100);
            m_smCustomizeInfo.g_intSaveImageMode = objFileHandle.GetValueAsInt("SaveImageMode", 0);
            m_smCustomizeInfo.g_intEmptyPocketTole = objFileHandle.GetValueAsInt("EmptyPocketTole", 50);
            m_smCustomizeInfo.g_intWrongFaceTole = objFileHandle.GetValueAsInt("WrongFaceTole", 50);

            m_smCustomizeInfo.g_blnStopLowYield = objFileHandle.GetValueAsBoolean("StopLowYield", false);

            if (!blnFirstInit)
            {
                if ((m_smCustomizeInfo.g_blnGlobalSharingCalibrationData != objFileHandle.GetValueAsBoolean("GlobalSharingCalibrationData", false)) ||
                    (m_smCustomizeInfo.g_blnGlobalSharingCameraData != objFileHandle.GetValueAsBoolean("GlobalSharingCameraData", false)))
                {
                    for (int i = 0; i < m_smVSInfo.Length; i++)
                    {
                        if (m_smVSInfo[i] == null)
                            continue;
                        m_smVSInfo[i].g_blnWantReloadRecipe = true;
                    }
                }
            }

            m_smCustomizeInfo.g_blnGlobalSharingCalibrationData = objFileHandle.GetValueAsBoolean("GlobalSharingCalibrationData", false);
            m_smCustomizeInfo.g_blnGlobalSharingCameraData = objFileHandle.GetValueAsBoolean("GlobalSharingCameraData", false);

#if (RELEASE || Release_2_12 || RTXRelease)
            if (m_smCustomizeInfo.g_blnGlobalSharingCalibrationData)
            {
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    m_smVSInfo[i].g_blnGlobalSharingCalibrationData = true;

                    string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");

                    RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                    RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                    string strSelectedRecipe = (string)subkey1.GetValue("SelectedRecipeID", "Default");

                    //2020-11-25 ZJYEOH : Copy global calibration data to local folder
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml"))
                    {
                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml", arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml", true);
                                File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CalibrationSharingMode.txt", "1");
                            }
                        }
                    }
                    //2020-12-11 ZJYEOH : Copy local-global to global if global file missing
                    else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml"))
                    {
                        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml",
                                AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml",
                                true);

                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if ((AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\") == (arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                                continue;

                            if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml", arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml", true);
                                File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CalibrationSharingMode.txt", "1");
                            }
                        }
                    }
                    //2020-12-11 ZJYEOH : if both local-global and global file also missing, then copy selected recipe local file to global
                    else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "Calibration.xml"))
                    {
                        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\Calibration.xml",
                                AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml",
                                true);

                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if ((AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\") == (arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                                continue;

                            if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml", arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + m_smVSInfo[i].g_strVisionFolderName + "Calibration.xml", true);
                                File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CalibrationSharingMode.txt", "1");
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    m_smVSInfo[i].g_blnGlobalSharingCalibrationData = false;

                    string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");

                    for (int j = 0; j < arrDirectory.Length; j++)
                    {
                        if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                        {
                            File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CalibrationSharingMode.txt", "0");
                        }
                    }
                }
            }

            if (m_smCustomizeInfo.g_blnGlobalSharingCameraData)
            {
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    m_smVSInfo[i].g_blnGlobalSharingCameraData = true;

                    string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");

                    RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                    RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                    string strSelectedRecipe = (string)subkey1.GetValue("SelectedRecipeID", "Default");

                    //2020-11-25 ZJYEOH : Copy global Camera data to local folder
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml"))
                    {
                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml", arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml", true);
                                File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CameraSharingMode.txt", "1");
                            }
                        }
                    }
                    //2020-12-11 ZJYEOH : Copy local-global to global if global file missing
                    else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml"))
                    {
                        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml",
                                AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml",
                                true);

                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if ((AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\") == (arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                                continue;

                            if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml", arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml", true);
                                File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CameraSharingMode.txt", "1");
                            }
                        }
                    }
                    //2020-12-11 ZJYEOH : if both local-global and global file also missing, then copy selected recipe local file to global
                    else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "Camera.xml"))
                    {
                        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\Camera.xml",
                                AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml",
                                true);

                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if ((AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\") == (arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                                continue;

                            if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml", arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + m_smVSInfo[i].g_strVisionFolderName + "Camera.xml", true);
                                File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CameraSharingMode.txt", "1");
                            }
                        }
                    }


                    //2020-11-25 ZJYEOH : Copy global Camera data to local folder
                    if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml"))
                    {
                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if (Directory.Exists(arrDirectory[j] + "\\"))
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml", arrDirectory[j] + "\\GlobalCamera.xml", true);
                                File.WriteAllText(arrDirectory[j] + "\\CameraSharingMode.txt", "1");
                            }
                        }
                    }
                    //2020-12-11 ZJYEOH : Copy local-global to global if global file missing
                    else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\GlobalCamera.xml"))
                    {
                        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\GlobalCamera.xml",
                                AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml",
                                true);

                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if ((AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\") == (arrDirectory[j] + "\\"))
                                continue;

                            if (Directory.Exists(arrDirectory[j] + "\\"))
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml", arrDirectory[j] + "\\GlobalCamera.xml", true);
                                File.WriteAllText(arrDirectory[j] + "\\CameraSharingMode.txt", "1");
                            }
                        }
                    }
                    //2020-12-11 ZJYEOH : if both local-global and global file also missing, then copy selected recipe local file to global
                    else if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\GlobalCamera.xml"))
                    {
                        File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\GlobalCamera.xml",
                                AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml",
                                true);

                        for (int j = 0; j < arrDirectory.Length; j++)
                        {
                            if ((AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + strSelectedRecipe + "\\") == (arrDirectory[j] + "\\"))
                                continue;

                            if (Directory.Exists(arrDirectory[j] + "\\"))
                            {
                                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml", arrDirectory[j] + "\\GlobalCamera.xml", true);
                                File.WriteAllText(arrDirectory[j] + "\\CameraSharingMode.txt", "1");
                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    m_smVSInfo[i].g_blnGlobalSharingCameraData = false;

                    string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");

                    for (int j = 0; j < arrDirectory.Length; j++)
                    {
                        if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                        {
                            File.WriteAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CameraSharingMode.txt", "0");
                        }
                    }

                    if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\"))
                    {
                        File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\CameraSharingMode.txt", "0");
                    }

                    for (int j = 0; j < arrDirectory.Length; j++)
                    {
                        if (Directory.Exists(arrDirectory[j] + "\\"))
                        {
                            File.WriteAllText(arrDirectory[j] + "\\CameraSharingMode.txt", "0");
                        }
                    }

                }
            }
#elif (DEBUG || Debug_2_12 || RTXDebug)
            //2020-12-10 ZJYEOH : Debug Mode will only see what is the mode save inside CalibrationSharingMode.txt 
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                m_smVSInfo[i].g_blnGlobalSharingCalibrationData = false;

                //string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                //for (int j = 0; j < arrDirectory.Length; j++)
                //{
                //    if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                //    {
                //        if (File.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CalibrationSharingMode.txt"))
                //        {
                //            if (File.ReadAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CalibrationSharingMode.txt") == "1")
                //            {
                //                m_smVSInfo[i].g_blnGlobalSharingCalibrationData = true;
                //            }
                //        }
                //    }
                //}

                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                string strSelectedRecipe = (string)subkey1.GetValue("SelectedRecipeID", "Default");

                string strSingleRecipeID = (string)subkey1.GetValue("SingleRecipeID" + i.ToString(), "");
                if (strSingleRecipeID == "")
                    strSingleRecipeID = strSelectedRecipe;

                if (File.Exists(m_smProductionInfo.g_strRecipePath + strSingleRecipeID + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CalibrationSharingMode.txt"))
                {
                    if (File.ReadAllText(m_smProductionInfo.g_strRecipePath + strSingleRecipeID + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CalibrationSharingMode.txt") == "1")
                    {
                        m_smVSInfo[i].g_blnGlobalSharingCalibrationData = true;
                    }
                }
            }

            //2020-12-10 ZJYEOH : Debug Mode will only see what is the mode save inside CameraSharingMode.txt 
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                m_smVSInfo[i].g_blnGlobalSharingCameraData = false;

                //string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                //for (int j = 0; j < arrDirectory.Length; j++)
                //{
                //    if (Directory.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\"))
                //    {
                //        if (File.Exists(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CameraSharingMode.txt"))
                //        {
                //            if (File.ReadAllText(arrDirectory[j] + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CameraSharingMode.txt") == "1")
                //            {
                //                m_smVSInfo[i].g_blnGlobalSharingCameraData = true;
                //            }
                //        }
                //    }

                //    if (Directory.Exists(arrDirectory[j] + "\\"))
                //    {
                //        if (File.Exists(arrDirectory[j] + "\\" + "CameraSharingMode.txt"))
                //        {
                //            if (File.ReadAllText(arrDirectory[j] + "\\" + "CameraSharingMode.txt") == "1")
                //            {
                //                m_smVSInfo[i].g_blnGlobalSharingCameraData = true;
                //            }
                //        }
                //    }
                //}

                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                string strSelectedRecipe = (string)subkey1.GetValue("SelectedRecipeID", "Default");

                string strSingleRecipeID = (string)subkey1.GetValue("SingleRecipeID" + i.ToString(), "");
                if (strSingleRecipeID == "")
                    strSingleRecipeID = strSelectedRecipe;

                if (File.Exists(m_smProductionInfo.g_strRecipePath + strSingleRecipeID + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CameraSharingMode.txt"))
                {
                    if (File.ReadAllText(m_smProductionInfo.g_strRecipePath + strSingleRecipeID + "\\" + m_smVSInfo[i].g_strVisionFolderName + "\\" + "CameraSharingMode.txt") == "1")
                    {
                        m_smVSInfo[i].g_blnGlobalSharingCameraData = true;
                    }
                }
            }
#endif


            m_smCustomizeInfo.g_fLowYield = objFileHandle.GetValueAsFloat("LowYield", 95);
            m_smCustomizeInfo.g_intMinUnitCheck = objFileHandle.GetValueAsInt("MinUnitCheck", 1000);
            m_smCustomizeInfo.g_intUnitDisplay = objFileHandle.GetValueAsInt("UnitDisplay", 1);    // 1=mm(default) , 2=mil, 3=micron  
            m_smCustomizeInfo.g_intMarkUnitDisplay = objFileHandle.GetValueAsInt("MarkUnitDisplay", 0);    // 0=pixel(default) , 1=mm, 2=mil, 3=micron  
            m_smProductionInfo.g_intAutoLogOutMinutes = objFileHandle.GetValueAsInt("AutoLogOutMinutes", 5);

            objFileHandle.GetFirstSection("SPC");
            m_intCalendarType = objFileHandle.GetValueAsInt("CalendarType", 0);
            m_intMTBAStartTime = objFileHandle.GetValueAsInt("MtbaStartTime", 0);
            m_intMTBAStartMinute = objFileHandle.GetValueAsInt("MtbaStartMinute", 0);
            m_intMTBAStartDay = objFileHandle.GetValueAsInt("MtbaStartDay", 1);
            m_intShiftNo = objFileHandle.GetValueAsInt("ShiftNo", 3);


        }

        private void InitTCPIP()
        {
            XmlParser objFileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFileHandle.GetFirstSection("TCPIP");
            m_smCustomizeInfo.g_blnWantExportReport = objFileHandle.GetValueAsBoolean("ExportReport", false);
            m_smCustomizeInfo.g_strExportReportIPAddress = objFileHandle.GetValueAsString("IPAdd", "");
            m_smCustomizeInfo.g_strExportReportDir = objFileHandle.GetValueAsString("ReportAdd", "");
            m_smCustomizeInfo.g_intExportReportFormat = objFileHandle.GetValueAsInt("ReportFormat", 0);
        }

        /// <summary>
        /// Get the network setting
        /// </summary>
        private void InitNetwork()
        {
            XmlParser objFileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFileHandle.GetFirstSection("Network");
            m_smCustomizeInfo.g_blnWantNetwork = objFileHandle.GetValueAsBoolean("WantNetwork", false);
            m_smCustomizeInfo.g_blnNetworkPasswordLimit = objFileHandle.GetValueAsBoolean("NetworkPasswordLimit", false);
            m_smCustomizeInfo.g_strHostIP = objFileHandle.GetValueAsString("NetworkHostIP", "");
            m_smCustomizeInfo.g_strNetworkUsername = objFileHandle.GetValueAsString("NetworkUser", "");
            m_smCustomizeInfo.g_strNetworkPassword = objFileHandle.GetValueAsString("NetworkPassword", "");
            m_smCustomizeInfo.g_strDeviceUploadDir = objFileHandle.GetValueAsString("DeviceUploadDir", "");
            m_smCustomizeInfo.g_strVisionLotReportUploadDir = objFileHandle.GetValueAsString("NetworkVisionLotReportUploadDir", "");
        }

        /// <summary>
        /// Get the network setting
        /// </summary>
        private void InitSECSGEM()
        {
            XmlParser objFileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFileHandle.GetFirstSection("SECSGEM");
            m_smCustomizeInfo.g_blnWantSECSGEM = objFileHandle.GetValueAsBoolean("WantSECSGEM", false);
            m_smCustomizeInfo.g_strSECSGEMSharedFolderPath = objFileHandle.GetValueAsString("SECSGEMSharedFolderDir", "");
            m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad = objFileHandle.GetValueAsInt("SECSGEMMaxNoOfCoplanPad", 10);
        }

        /// <summary>
        /// declare all object according to each vision station
        /// </summary>
        private void InitVisionInfo()
        {
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                // Init for all vision
                m_smVSInfo[i].g_objCalibrateCircleGauge = new CirGauge(40, 400, m_smVSInfo[i].g_WorldShape);
                m_smVSInfo[i].g_objCalibrateRectGauge = new RectGauge(m_smVSInfo[i].g_WorldShape);

                switch (m_smVSInfo[i].g_strVisionName)
                {
                    case "Measure":
                        break;
                    case "Orient":
                    case "BottomOrient":
                    case "BottomPosition":
                    case "Mark":
                    case "MarkOrient":
                    case "MarkPkg":
                    case "MOPkg":
                    case "MOLiPkg":
                    case "MOLi":
                    case "Package":
                        m_smVSInfo[i].g_arrOrients = new List<List<Orient>>();
                        m_smVSInfo[i].g_arrOrientROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrOrientGauge = new List<RectGauge>();
                        m_smVSInfo[i].g_arrOrientGaugeM4L = new List<RectGaugeM4L>();
                        m_smVSInfo[i].g_intOrientResult = new int[2];

                        m_smVSInfo[i].g_arrOCVs = new List<List<ArrayList>>();
                        m_smVSInfo[i].g_arrMarkROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrMarkDontCareROIs = new List<ROI>();
                        m_smVSInfo[i].g_arrMarkGauge = new List<RectGauge>();
                        m_smVSInfo[i].g_arrMarkGaugeM4L = new List<RectGaugeM4L>();
                        m_smVSInfo[i].g_arrPolygon_Mark = new List<List<Polygon>>();
                        m_smVSInfo[i].g_arrPolygon_Package = new List<List<List<Polygon>>>();
                        m_smVSInfo[i].g_arrPolygon_Package.Add(new List<List<Polygon>>());
                        m_smVSInfo[i].g_arrPolygon_Package[0].Add(new List<Polygon>());
                        m_smVSInfo[i].g_arrPolygon_Package[0].Add(new List<Polygon>());
                        m_smVSInfo[i].g_arrPolygon_PackageColor = new List<List<List<Polygon>>>();
                        m_smVSInfo[i].g_arrPolygon_PackageColor.Add(new List<List<Polygon>>());

                        m_smVSInfo[i].g_arrMarks = new List<Mark>();
                        for (int u = 0; u < m_smVSInfo[i].g_intUnitsOnImage; u++)
                        {
                            Mark objMark;
                            if ((m_smCustomizeInfo.g_intWantOCR & (0x01 << i)) == 0)
                                objMark = new Mark(0, m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);
                            else
                                objMark = new Mark(1, m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);

                            m_smVSInfo[i].g_arrMarks.Add(objMark);

                            if ((m_smCustomizeInfo.g_intWant2DCode & (0x01 << i)) > 0)
                            {

                                m_smVSInfo[i].g_arrMarks[u].Init2DCode();

                            }
                        }

                        m_smVSInfo[i].g_arrPin1 = new List<Pin1>();
                        for (int u = 0; u < m_smVSInfo[i].g_intUnitsOnImage; u++)
                        {
                            m_smVSInfo[i].g_arrPin1.Add(new Pin1());
                        }

                        m_smVSInfo[i].g_arrPackage = new List<Package>();
                        m_smVSInfo[i].g_arrPackageROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrPackageColorROIs = new List<List<CROI>>();
                        m_smVSInfo[i].g_arrPackageMoldFlashDontCareROIs = new List<ROI>();
                        m_smVSInfo[i].g_arrPackageDontCareROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrPackageColorDontCareROIs = new List<List<List<ROI>>>();
                        for (int u = 0; u < m_smVSInfo[i].g_intUnitsOnImage; u++)
                        {
                            m_smVSInfo[i].g_arrPackageColorDontCareROIs.Add(new List<List<ROI>>());
                        }
                        m_smVSInfo[i].g_arrPackageDontCareROIs.Add(new List<ROI>());
                        m_smVSInfo[i].g_arrPackageDontCareROIs.Add(new List<ROI>());
                        m_smVSInfo[i].g_arrPackageGauge = new List<RectGauge>();
                        m_smVSInfo[i].g_arrPackageGauge2 = new List<RectGauge>();
                        m_smVSInfo[i].g_arrPackageGaugeM4L = new List<RectGaugeM4L>();
                        m_smVSInfo[i].g_arrPackageGauge2M4L = new List<RectGaugeM4L>();

                        if ((m_smCustomizeInfo.g_intUseColorCamera & (0x01 << i)) > 0)
                        {
                            m_smVSInfo[i].g_arrColorImages = new List<CImageDrawing>();
                            m_smVSInfo[i].g_arrColorRotatedImages = new List<CImageDrawing>();
                            m_smVSInfo[i].g_objColorROI = new CROI();
                            m_smVSInfo[i].g_blnViewColorImage = true;

                            m_smVSInfo[i].g_arrColorPackage = new List<ColorPackage>();
                            m_smVSInfo[i].g_arrColorPackageROIs = new List<List<CROI>>();
                        }

                        // Use in very small package size which the position is not stable.
                        if ((m_smCustomizeInfo.g_intWantPositioning & (0x01 << i)) > 0)
                        {
                            m_smVSInfo[i].g_objPositioning = new Position(false);
                            m_smVSInfo[i].g_arrPositioningROIs = new List<ROI>();
                            m_smVSInfo[i].g_arrPositioningGauges = new List<LGauge>();
                            m_smVSInfo[i].g_arrPolygon = new List<List<Polygon>>();
                        }

                        if ((m_smCustomizeInfo.g_intWantLead & (0x01 << i)) > 0)
                        {
                            m_smVSInfo[i].g_arrLeadROIs = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsFix = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsFix.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsFix.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsFix.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsFix.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsFix.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsManual = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsManual.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsManual.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsManual.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsManual.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsManual.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsAuto = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsAuto.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsAuto.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsAuto.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsAuto.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsAuto.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsBlob = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsBlob.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsBlob.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsBlob.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsBlob.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsBlob.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLead = new Lead[5];
                            m_smVSInfo[i].g_arrInspectLeadROI = new ROI[5];
                            m_smVSInfo[i].g_arrInspectLeadROI_BaseLead = new ROI[5];

                            for (int w = 0; w < m_smVSInfo[i].g_arrLead.Length; w++)
                            {
                                m_smVSInfo[i].g_arrLead[w] = new Lead(m_smVSInfo[i].g_WorldShape, w, (m_smCustomizeInfo.g_intWantPackage & (0x01 << i)) > 0);
                                m_smVSInfo[i].g_arrInspectLeadROI[w] = new ROI();
                                m_smVSInfo[i].g_arrInspectLeadROI_BaseLead[w] = new ROI();

                            }
                        }

                        m_smVSInfo[i].g_objPackageImage = new ImageDrawing(m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);
                        m_smVSInfo[i].g_objMarkImage = new ImageDrawing(m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);
                        //m_smVSInfo[i].g_objModifiedPackageImage = new ImageDrawing(m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);

                        string[] strPkgItems = { "Width", "Length" };// { "OffSet", "Area", "Width", "Length", "Pitch", "Gap" };
                        m_smVSInfo[i].g_objGRR = new GRR(strPkgItems);

                        break;
                    case "UnitPresent":
                        m_smVSInfo[i].g_objUnitPresent = new UnitPresent();
                        m_smVSInfo[i].g_arrPositioningROIs = new List<ROI>();

                        m_smVSInfo[i].g_objPositioning = new Position(true);
                        m_smVSInfo[i].g_arrPositioningGauges = new List<LGauge>();
                        m_smVSInfo[i].g_arrPolygon = new List<List<Polygon>>();

                        if ((m_smCustomizeInfo.g_intWantOrient & (0x01 << i)) > 0)
                        {
                            m_smVSInfo[i].g_arrOrients = new List<List<Orient>>();
                            m_smVSInfo[i].g_arrOrientROIs = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrOrientGauge = new List<RectGauge>();
                            m_smVSInfo[i].g_arrOrientGaugeM4L = new List<RectGaugeM4L>();
                            m_smVSInfo[i].g_intOrientResult = new int[2];
                        }
                        break;
                    case "BottomPositionOrient":
                    case "TapePocketPosition":
                        m_smVSInfo[i].g_objPositioning = new Position(true);
                        m_smVSInfo[i].g_arrPositioningROIs = new List<ROI>();
                        m_smVSInfo[i].g_arrPositioningGauges = new List<LGauge>();
                        m_smVSInfo[i].g_arrPolygon = new List<List<Polygon>>();

                        if ((m_smCustomizeInfo.g_intWantOrient & (0x01 << i)) > 0)
                        {
                            m_smVSInfo[i].g_arrOrients = new List<List<Orient>>();
                            m_smVSInfo[i].g_arrOrientROIs = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrOrientGauge = new List<RectGauge>();
                            m_smVSInfo[i].g_arrOrientGaugeM4L = new List<RectGaugeM4L>();
                            m_smVSInfo[i].g_intOrientResult = new int[2];
                        }
                        break;
                    case "BottomOrientPad":
                    case "BottomOPadPkg":
                    case "Pad":
                    case "PadPos":
                    case "PadPkg":
                    case "PadPkgPos":
                    case "Pad5S":
                    case "Pad5SPos":
                    case "Pad5SPkg":
                    case "Pad5SPkgPos":
                        m_smVSInfo[i].g_objTopParentROI = new ROI();
                        m_smVSInfo[i].g_arrPadROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrPadColorROIs = new List<List<CROI>>();
                        m_smVSInfo[i].g_arrPadOrientROIs = new List<ROI>();
                        m_smVSInfo[i].g_arrPadDontCareROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrPadColorDontCareROIs = new List<List<List<ROI>>>();

                        m_smVSInfo[i].g_intOrientResult = new int[2];
                        m_smVSInfo[i].g_arrOrients = new List<List<Orient>>();
                        m_smVSInfo[i].g_arrOrientROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrOrientGauge = new List<RectGauge>();
                        m_smVSInfo[i].g_arrPolygon_Pad = new List<List<Polygon>>();
                        m_smVSInfo[i].g_arrPolygon_PadColor = new List<List<List<Polygon>>>();
                        if ((m_smCustomizeInfo.g_intWantPad5S & (0x01 << i)) > 0)
                        {
                            m_smVSInfo[i].g_arrPad = new Pad[5];
                            //m_smVSInfo[i].g_arrPadOrient = new Orient[5];
                            m_smVSInfo[i].g_arrInspectROI = new ROI[5];
                            m_smVSInfo[i].g_arrInspectPkgROI = new ROI[5][];
                            m_smVSInfo[i].g_arrInspectPadROI = new ROI[5][];
                            m_smVSInfo[i].g_arrInspectSearchROI = new ROI[5];
                        }
                        else
                        {
                            m_smVSInfo[i].g_arrPad = new Pad[1];
                            //m_smVSInfo[i].g_arrPadOrient = new Orient[1];
                            m_smVSInfo[i].g_arrInspectROI = new ROI[1];
                            m_smVSInfo[i].g_arrInspectPkgROI = new ROI[1][];
                            m_smVSInfo[i].g_arrInspectPadROI = new ROI[1][];
                            m_smVSInfo[i].g_arrInspectSearchROI = new ROI[5];
                        }

                        m_smVSInfo[i].g_arrPin1 = new List<Pin1>();
                        for (int u = 0; u < m_smVSInfo[i].g_intUnitsOnImage; u++)
                        {
                            m_smVSInfo[i].g_arrPin1.Add(new Pin1());
                        }

                        m_smVSInfo[i].g_objPadOrient = new Orient(m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);

                        for (int w = 0; w < m_smVSInfo[i].g_arrPad.Length; w++)
                        {
                            //m_smVSInfo[i].g_arrPadOrient[w] = new Orient(m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);
                            if (w == 0) // if middle pad, init poscrosshair
                                m_smVSInfo[i].g_arrPad[w] = new Pad(m_smVSInfo[i].g_WorldShape, true, m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight, 0, m_smVSInfo[i].g_intVisionIndex);
                            else
                                m_smVSInfo[i].g_arrPad[w] = new Pad(m_smVSInfo[i].g_WorldShape, m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight, w, m_smVSInfo[i].g_intVisionIndex);

                            m_smVSInfo[i].g_arrInspectROI[w] = new ROI();
                            m_smVSInfo[i].g_arrInspectSearchROI[w] = new ROI();

                            m_smVSInfo[i].g_arrInspectPadROI[w] = new ROI[3];
                            for (int x = 0; x < m_smVSInfo[i].g_arrInspectPadROI[w].Length; x++)
                                m_smVSInfo[i].g_arrInspectPadROI[w][x] = new ROI();

                            m_smVSInfo[i].g_arrInspectPkgROI[w] = new ROI[5];
                            for (int x = 0; x < m_smVSInfo[i].g_arrInspectPkgROI[w].Length; x++)
                                m_smVSInfo[i].g_arrInspectPkgROI[w][x] = new ROI();
                        }

                        if ((m_smCustomizeInfo.g_intWantPackage & (0x01 << i)) > 0)
                        {
                            m_smVSInfo[i].g_arrPackage = new List<Package>();
                            m_smVSInfo[i].g_arrPackageROIs = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrPadPackageDontCareROIs = new List<List<List<ROI>>>();
                            m_smVSInfo[i].g_arrPackageGauge = new List<RectGauge>();
                            m_smVSInfo[i].g_arrPackageGauge2 = new List<RectGauge>();
                            m_smVSInfo[i].g_arrPackageGaugeM4L = new List<RectGaugeM4L>();
                            m_smVSInfo[i].g_arrPackageGauge2M4L = new List<RectGaugeM4L>();
                            m_smVSInfo[i].g_arrPolygon_PadPackage = new List<List<List<Polygon>>>();
                            m_smVSInfo[i].g_arrPolygon_PackageColor = new List<List<List<Polygon>>>();
                            m_smVSInfo[i].g_arrPackageColorDontCareROIs = new List<List<List<ROI>>>();
                            m_smVSInfo[i].g_arrPackageColorROIs = new List<List<CROI>>();
                        }

                        m_smVSInfo[i].g_objPackageImage = new ImageDrawing(m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);

                        //if ((m_smCustomizeInfo.g_intWantPositioning & (0x01 << i)) > 0)
                        {
                            m_smVSInfo[i].g_arrPositioningROIs = new List<ROI>();
                            m_smVSInfo[i].g_objPositioning = new Position(false);
                            m_smVSInfo[i].g_arrPositioningGauges = new List<LGauge>();
                            m_smVSInfo[i].g_objPositioning2 = new Position(false);
                            m_smVSInfo[i].g_arrPositioningGauges2 = new List<LGauge>();
                            m_smVSInfo[i].g_arrPHROIs = new List<ROI>(); // declare for Check PH
                        }

                        if ((m_smCustomizeInfo.g_intUseColorCamera & (0x01 << i)) > 0)
                        {
                            m_smVSInfo[i].g_arrColorImages = new List<CImageDrawing>();
                            m_smVSInfo[i].g_arrColorRotatedImages = new List<CImageDrawing>();
                            m_smVSInfo[i].g_objColorROI = new CROI();
                            m_smVSInfo[i].g_blnViewColorImage = true;

                            m_smVSInfo[i].g_arrColorPackage = new List<ColorPackage>();
                            m_smVSInfo[i].g_arrColorPackageROIs = new List<List<CROI>>();
                        }

                        string[] strItems = { "OffSet", "Area", "Width", "Length", "Pitch", "Gap" };
                        m_smVSInfo[i].g_objGRR = new GRR(strItems);

                        //CPK
                        m_smVSInfo[i].g_objCPK = new CPK(strItems);

                        if (m_smVSInfo[i].g_intImageMergeType != 0)
                        {
                            m_smVSInfo[i].g_arrSystemROI = new List<ROI>();
                        }

                        break;
                    case "Li3D":
                    case "Li3DPkg":
                        m_smVSInfo[i].g_arrLeadROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrLead3DDontCareROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrPolygon_Lead3D = new List<List<Polygon>>();

                        m_smVSInfo[i].g_arrOrients = new List<List<Orient>>();
                        m_smVSInfo[i].g_arrOrientROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrOrientGauge = new List<RectGauge>();

                        m_smVSInfo[i].g_arrLead3D = new Lead3D[5];
                        m_smVSInfo[i].g_arrInspectLeadROI = new ROI[5];
                        m_smVSInfo[i].g_arrInspectLeadROI_BaseLead = new ROI[5];

                        m_smVSInfo[i].g_arrInspectLead3DPkgROI = new ROI[1][];

                        m_smVSInfo[i].g_arrInspectLead3DPkgROI[0] = new ROI[4];
                        for (int x = 0; x < m_smVSInfo[i].g_arrInspectLead3DPkgROI[0].Length; x++)
                            m_smVSInfo[i].g_arrInspectLead3DPkgROI[0][x] = new ROI();

                        for (int w = 0; w < m_smVSInfo[i].g_arrLead3D.Length; w++)
                        {
                            m_smVSInfo[i].g_arrLead3D[w] = new Lead3D(m_smVSInfo[i].g_WorldShape, m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight, w);
                            m_smVSInfo[i].g_arrInspectLeadROI[w] = new ROI();
                            m_smVSInfo[i].g_arrInspectLeadROI_BaseLead[w] = new ROI();

                        }

                        m_smVSInfo[i].g_arrPin1 = new List<Pin1>();
                        for (int u = 0; u < m_smVSInfo[i].g_intUnitsOnImage; u++)
                        {
                            m_smVSInfo[i].g_arrPin1.Add(new Pin1());
                        }

                        if ((m_smCustomizeInfo.g_intWantPackage & (0x01 << i)) > 0)
                        {
                            m_smVSInfo[i].g_arrPackage = new List<Package>();
                            m_smVSInfo[i].g_arrPackageROIs = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrPackageDontCareROIs = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrPackageDontCareROIs.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrPackageDontCareROIs.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrPackageGauge = new List<RectGauge>();
                            m_smVSInfo[i].g_arrPackageGauge2 = new List<RectGauge>();
                            m_smVSInfo[i].g_arrPackageGaugeM4L = new List<RectGaugeM4L>();
                            m_smVSInfo[i].g_arrPackageGauge2M4L = new List<RectGaugeM4L>();
                        }

                        m_smVSInfo[i].g_arrPolygon_Package = new List<List<List<Polygon>>>();
                        m_smVSInfo[i].g_arrPolygon_Package.Add(new List<List<Polygon>>());
                        m_smVSInfo[i].g_arrPolygon_Package[0].Add(new List<Polygon>());
                        m_smVSInfo[i].g_arrPolygon_Package[0].Add(new List<Polygon>());

                        m_smVSInfo[i].g_objPackageImage = new ImageDrawing(m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);

                        m_smVSInfo[i].g_objPositioning = new Position(false);
                        m_smVSInfo[i].g_arrPHROIs = new List<ROI>(); // declare for Check PH

                        string[] strLeadItems = { "OffSet", "Width", "Length", "Pitch", "Gap", "Skew", "StandOff", "Coplan" };
                        m_smVSInfo[i].g_objGRR = new GRR(strLeadItems);

                        if (m_smVSInfo[i].g_intImageMergeType != 0)
                        {
                            m_smVSInfo[i].g_arrSystemROI = new List<ROI>();
                        }
                        break;
                    case "InPocket":
                    case "InPocketPkg":
                    case "InPocketPkgPos":
                    case "IPMLi":
                    case "IPMLiPkg":
                        m_smVSInfo[i].g_arrOrients = new List<List<Orient>>();
                        m_smVSInfo[i].g_arrOrientROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrOrientGauge = new List<RectGauge>();
                        m_smVSInfo[i].g_arrOrientGaugeM4L = new List<RectGaugeM4L>();
                        m_smVSInfo[i].g_intOrientResult = new int[2];

                        m_smVSInfo[i].g_arrOCVs = new List<List<ArrayList>>();
                        m_smVSInfo[i].g_arrMarkROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrMarkDontCareROIs = new List<ROI>();
                        m_smVSInfo[i].g_arrMarkGauge = new List<RectGauge>();
                        m_smVSInfo[i].g_arrMarkGaugeM4L = new List<RectGaugeM4L>();

                        m_smVSInfo[i].g_arrMarks = new List<Mark>();
                        for (int u = 0; u < m_smVSInfo[i].g_intUnitsOnImage; u++)
                        {
                            Mark objMark;
                            if ((m_smCustomizeInfo.g_intWantOCR & (0x01 << i)) == 0)
                                objMark = new Mark(0, m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);
                            else
                                objMark = new Mark(1, m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);
                            m_smVSInfo[i].g_arrMarks.Add(objMark);
                        }

                        m_smVSInfo[i].g_arrPin1 = new List<Pin1>();
                        for (int u = 0; u < m_smVSInfo[i].g_intUnitsOnImage; u++)
                        {
                            m_smVSInfo[i].g_arrPin1.Add(new Pin1());
                        }

                        m_smVSInfo[i].g_arrPackage = new List<Package>();
                        m_smVSInfo[i].g_arrPackageROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrPackageMoldFlashDontCareROIs = new List<ROI>();
                        m_smVSInfo[i].g_arrPackageDontCareROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrPackageDontCareROIs.Add(new List<ROI>());
                        m_smVSInfo[i].g_arrPackageDontCareROIs.Add(new List<ROI>());
                        m_smVSInfo[i].g_arrPackageGauge = new List<RectGauge>();
                        m_smVSInfo[i].g_arrPackageGauge2 = new List<RectGauge>();
                        m_smVSInfo[i].g_arrPackageGaugeM4L = new List<RectGaugeM4L>();
                        m_smVSInfo[i].g_arrPackageGauge2M4L = new List<RectGaugeM4L>();
                        m_smVSInfo[i].g_arrPackageColorROIs = new List<List<CROI>>();
                        m_smVSInfo[i].g_arrPackageMoldFlashDontCareROIs = new List<ROI>();
                        m_smVSInfo[i].g_arrPackageColorDontCareROIs = new List<List<List<ROI>>>();
                        for (int u = 0; u < m_smVSInfo[i].g_intUnitsOnImage; u++)
                        {
                            m_smVSInfo[i].g_arrPackageColorDontCareROIs.Add(new List<List<ROI>>());
                        }
                        // For InPocket with Positioning
                        //if ((m_smCustomizeInfo.g_intWantPositioning & (0x01 << i)) > 0)
                        //{
                        // 19-02-2019: ZJYEOH need to declare everytime for check empty
                        m_smVSInfo[i].g_objPositioning = new Position(false);
                        m_smVSInfo[i].g_arrPositioningROIs = new List<ROI>();
                        m_smVSInfo[i].g_arrPositioningGauges = new List<LGauge>();
                        m_smVSInfo[i].g_arrPolygon = new List<List<Polygon>>();
                        m_smVSInfo[i].g_arrPolygon_Mark = new List<List<Polygon>>();
                        m_smVSInfo[i].g_arrPolygon_Package = new List<List<List<Polygon>>>();
                        m_smVSInfo[i].g_arrPolygon_Package.Add(new List<List<Polygon>>());
                        m_smVSInfo[i].g_arrPolygon_Package[0].Add(new List<Polygon>());
                        m_smVSInfo[i].g_arrPolygon_Package[0].Add(new List<Polygon>());
                        m_smVSInfo[i].g_arrPolygon_PackageColor = new List<List<List<Polygon>>>();
                        m_smVSInfo[i].g_arrPolygon_PackageColor.Add(new List<List<Polygon>>());
                        // }

                        {
                            m_smVSInfo[i].g_arrPocketPositionROIs = new List<ROI>();
                            m_smVSInfo[i].g_objPocketPosition = new PocketPosition();
                            m_smVSInfo[i].g_arrPocketPositionGauges = new List<LGauge>();
                        }

                        if ((m_smCustomizeInfo.g_intUseColorCamera & (0x01 << i)) > 0)
                        {
                            m_smVSInfo[i].g_arrColorImages = new List<CImageDrawing>();
                            m_smVSInfo[i].g_arrColorRotatedImages = new List<CImageDrawing>();
                            m_smVSInfo[i].g_objColorROI = new CROI();
                            m_smVSInfo[i].g_blnViewColorImage = true;

                            m_smVSInfo[i].g_arrColorPackage = new List<ColorPackage>();
                            m_smVSInfo[i].g_arrColorPackageROIs = new List<List<CROI>>();
                        }

                        if ((m_smCustomizeInfo.g_intWantLead & (0x01 << i)) > 0)
                        {
                            m_smVSInfo[i].g_arrLeadROIs = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsFix = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsFix.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsFix.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsFix.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsFix.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsFix.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsManual = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsManual.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsManual.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsManual.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsManual.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsManual.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsAuto = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsAuto.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsAuto.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsAuto.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsAuto.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsAuto.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsBlob = new List<List<ROI>>();
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsBlob.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsBlob.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsBlob.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsBlob.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLeadPocketDontCareROIsBlob.Add(new List<ROI>());
                            m_smVSInfo[i].g_arrLead = new Lead[5];
                            m_smVSInfo[i].g_arrInspectLeadROI = new ROI[5];
                            m_smVSInfo[i].g_arrInspectLeadROI_BaseLead = new ROI[5];

                            for (int w = 0; w < m_smVSInfo[i].g_arrLead.Length; w++)
                            {
                                m_smVSInfo[i].g_arrLead[w] = new Lead(m_smVSInfo[i].g_WorldShape, w, (m_smCustomizeInfo.g_intWantPackage & (0x01 << i)) > 0);
                                m_smVSInfo[i].g_arrInspectLeadROI[w] = new ROI();
                                m_smVSInfo[i].g_arrInspectLeadROI_BaseLead[w] = new ROI();

                            }
                        }

                        m_smVSInfo[i].g_objPackageImage = new ImageDrawing(m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);
                        m_smVSInfo[i].g_objMarkImage = new ImageDrawing(m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);

                        string[] strInPocketPkgItems = { "Width", "Length" };// { "OffSet", "Area", "Width", "Length", "Pitch", "Gap" };
                        m_smVSInfo[i].g_objGRR = new GRR(strInPocketPkgItems);

                        break;
                    case "Seal":
                        m_smVSInfo[i].g_objSeal = new SealBlog(m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);
                        m_smVSInfo[i].g_arrSealROIs = new List<List<ROI>>();
                        m_smVSInfo[i].g_arrSealGauges = new List<List<LGauge>>();
                        m_smVSInfo[i].g_objSealCircleGauges = new CirGauge(40, 400, m_smVSInfo[i].g_WorldShape);
                        m_smVSInfo[i].g_objSealImage = new ImageDrawing(m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);
                        m_smVSInfo[i].g_intOrientResult = new int[2];
                        m_smVSInfo[i].g_arrSealDontCareROIs = new List<ROI>();
                        m_smVSInfo[i].g_arrPolygon_Seal = new List<Polygon>();
                        break;
                    case "Barcode":
                        m_smVSInfo[i].g_objBarcode = new Barcode(m_smVSInfo[i].g_WorldShape.ref_objWorldShape);
                        m_smVSInfo[i].g_arrBarcodeROIs = new List<ROI>();
                        m_smVSInfo[i].g_objPackageImage = new ImageDrawing(m_smVSInfo[i].g_intCameraResolutionWidth, m_smVSInfo[i].g_intCameraResolutionHeight);
                        break;
                    default:
                        SRMMessageBox.Show("InitVisionInfo() --> There is no such vision module name " + m_smVSInfo[i].g_strVisionName + " in this SRMVision software version.");
                        break;
                }
            }
        }
        /// <summary>
        /// Check user group: 5 - Operator while 2 - Administrator
        /// </summary>
        /// <param name="blnFirstTime">if true, login form won't be pop up</param>
        private void Login(bool blnFirstTime)
        {
            Cursor.Current = Cursors.WaitCursor;
#if (RELEASE || RTXRelease || Release_2_12)
            if (m_smProductionInfo.g_blnLoadSRMVisionForRefreshing)
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subKey = key.CreateSubKey("SVG");
                m_intUserGroup = Convert.ToInt32(subKey.GetValue("LastUserGroup", "5"));
                m_strUserName = subKey.GetValue("LastUserName", "Operator").ToString();
            }
            else
            {
                m_intUserGroup = 5;
                m_strUserName = "Operator";
            }
#else
            if (blnFirstTime)
            {
                m_intUserGroup = 1;
                m_strUserName = "SRM";
            }
#endif

            if (!blnFirstTime)
            {
                LoginForm objLogin = new LoginForm(m_smProductionInfo);
                if (objLogin.ShowDialog(this) == DialogResult.OK)
                {
                    m_intUserGroup = objLogin.ref_intUserGroup;
                    m_strUserName = objLogin.ref_strUserName;
                    m_smProductionInfo.g_DTStartAutoMode = DateTime.Now;
                }
                else
                {
                    m_intUserGroup = 5;
                    m_strUserName = "Operator";
                }
                objLogin.Dispose();
            }

            m_smProductionInfo.g_strOperatorID = m_strUserName;
            m_smProductionInfo.g_intUserGroup = m_intUserGroup;

            TSlbl_User.Text = m_strUserName;
            if (m_intUserGroup == 5)
                TSlbl_User.Text += "(Operator)";
            else if (m_intUserGroup == 4)
                TSlbl_User.Text += "(Technician)";
            else if (m_intUserGroup == 3)
                TSlbl_User.Text += "(Engineer)";
            else if (m_intUserGroup == 2)
                TSlbl_User.Text += "(Administrator)";
            else if (m_intUserGroup == 1)
                TSlbl_User.Text += "(SRM Vendor)";

            CustomizeGUI();
        }
        /// <summary>
        /// Open Auto Mode form
        /// </summary>
        private void OpenAutoMode()
        {
            if (STDeviceEdit.m_dsDeviceEditLog == null)
                STDeviceEdit.InitDeviceEdit(m_smProductionInfo, m_smCustomizeInfo, m_smVSInfo);
            //else
            //    STDeviceEdit.InitDeviceEdit(m_smProductionInfo);

            Cursor.Current = Cursors.WaitCursor;

            Form[] mdiChild = MdiChildren;
            int i = 0;
            for (i = 0; i < mdiChild.Length; i++)
            {
                if (mdiChild[i].Name == "Auto")
                {
                    mdiChild[i].Activate();
                    break;
                }
            }
            if (m_smCustomizeInfo.g_blnAutoLogout)
                Login(true);
            if (i == mdiChild.Length)
            {
                if (m_autoMode == null)
                {
                    m_autoMode = new AutoForm(m_smCustomizeInfo, m_smProductionInfo, m_smVSInfo,
                                                                m_intUserGroup, m_smVSComThread, m_thCOMMPort, m_smVSTCPIPIO);
                }
                m_autoMode.MdiParent = this;
                m_autoMode.Name = "Auto";
                m_autoMode.Show();
                m_autoMode.Enabled = true;
                m_autoMode.StartThread(true);
            }
            else
            {
                m_autoMode.MdiParent = this;
                m_autoMode.Name = "Auto";
                m_autoMode.Show();
                m_autoMode.Enabled = true;
                m_autoMode.Focus();

                //StartWaiting("Loading Recipe...");
                m_autoMode.StartThread(false);
                //StopWaiting();
            }
            
            STDeviceEdit.SaveDeviceEditLog("Enter Auto Mode", "", "", "", m_smProductionInfo.g_strLotID);
            
            m_smProductionInfo.g_DTStartAutoMode = DateTime.Now;
            timer_AutoLogOut.Start();
        }

        /// <summary>
        /// Reload Auto Mode form after user login while auto mode is open
        /// </summary>
        private void ReloadAutoMode()
        {
            Cursor.Current = Cursors.WaitCursor;

            Form[] mdiChild = MdiChildren;
            int i = 0;
            for (i = 0; i < mdiChild.Length; i++)
            {
                if (mdiChild[i].Name == "Auto")
                {
                    mdiChild[i].Activate();
                    break;
                }
            }

            if (i == mdiChild.Length)
            {
                if (m_autoMode == null)
                {
                    m_autoMode = new AutoForm(m_smCustomizeInfo, m_smProductionInfo, m_smVSInfo,
                                                                m_intUserGroup, m_smVSComThread, m_thCOMMPort, m_smVSTCPIPIO);
                }
                m_autoMode.MdiParent = this;
                m_autoMode.Name = "Auto";
                m_autoMode.Show();
                m_autoMode.Enabled = true;
                m_autoMode.StartThread(true);
            }
            else
            {
                m_autoMode.MdiParent = this;
                m_autoMode.Name = "Auto";
                m_autoMode.Show();
                m_autoMode.Enabled = true;
                m_autoMode.Focus();

                //StartWaiting("Loading Recipe...");
                m_autoMode.StartThread(false);
                //StopWaiting();
            }
            
            STDeviceEdit.SaveDeviceEditLog("Enter Auto Mode", "", "", "", m_smProductionInfo.g_strLotID);
            
            m_smProductionInfo.g_DTStartAutoMode = DateTime.Now;
            timer_AutoLogOut.Start();
        }
        /// <summary>
        /// Open History Form
        /// </summary>
        private void OpenHistory()
        {
            Cursor.Current = Cursors.WaitCursor;

            Form[] childForm = this.MdiChildren;
            int i = 0;
            for (i = 0; i < childForm.Length; i++)
            {
                if (childForm[i].Name == "History")
                {
                    childForm[i].Activate();
                    break;
                }
            }

            if (i == childForm.Length)
            {
                LotHistory LotHisDialog = new LotHistory(m_smVSInfo, m_smProductionInfo, m_smCustomizeInfo);
                LotHisDialog.MdiParent = this;
                LotHisDialog.Name = "History";
                LotHisDialog.Show();
            }
        }
        /// <summary>
        /// Open IO Mapping Form
        /// </summary>
        private void OpenIOMapping()
        {
            Cursor.Current = Cursors.WaitCursor;

            Form[] mdiChild = MdiChildren;
            int i = 0;
            for (i = 0; i < mdiChild.Length; i++)
            {
                if (mdiChild[i].Name == "IOMap")
                {
                    mdiChild[i].Activate();
                    break;
                }
            }

            if (i == mdiChild.Length)
            {
                //IOMapForm objIOMapForm = new IOMapForm(m_smCustomizeInfo);
                NewIOMapForm objIOMapForm = new NewIOMapForm(m_smCustomizeInfo);
                objIOMapForm.MdiParent = this;
                objIOMapForm.Name = "IOMap";
                objIOMapForm.Show();
            }
        }
        /// <summary>
        /// Open IO form
        /// </summary>
        private void OpenIOMode()
        {
            Cursor.Current = Cursors.WaitCursor;

            Form[] mdiChild = MdiChildren;
            int i = 0;
            for (i = 0; i < mdiChild.Length; i++)
            {
                if (mdiChild[i].Name == "IO")
                {
                    mdiChild[i].Activate();
                    break;
                }
            }

            if (i == mdiChild.Length)
            {
                IOForm objIOForm = new IOForm(m_smProductionInfo, m_intUserGroup);
                objIOForm.MdiParent = this;
                objIOForm.Name = "IO";
                objIOForm.Show();
            }
        }
        /// <summary>
        /// Read From registry to check availability of vision and their features inside
        /// For instance, vision 4 name is InPocket. Its features are mark and lead.
        /// </summary>
        private void ReadFromRegistry()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            string[] strVisionFeatureList;
            RegistryKey subKey1;

            List<string> arrVisionName = new List<string>();
            for (int i = 0; i < strVisionList.Length; i++)
            {
                subKey1 = subKey.OpenSubKey(strVisionList[i]);
                int intVisionNo = Convert.ToInt32(strVisionList[i].Substring(6)) - 1;

                m_smVSInfo[intVisionNo] = new VisionInfo();
                m_smCustomizeInfo.g_intVisionMask |= (1 << intVisionNo);
                m_smVSInfo[intVisionNo].g_strVisionFolderName = strVisionList[i];
                m_smVSInfo[intVisionNo].g_intVisionPos = intVisionNo;
                m_smVSInfo[intVisionNo].g_strCameraModel = subKey1.GetValue("CameraModel", "AVT").ToString();
                m_smVSInfo[intVisionNo].g_intUnitsOnImage = Convert.ToInt32(subKey1.GetValue("ImageUnits", 1));
                m_smVSInfo[intVisionNo].g_intGrabMode = Convert.ToInt32(subKey1.GetValue("GrabMode", 0));
                m_smVSInfo[intVisionNo].g_intTriggerMode = Convert.ToInt32(subKey1.GetValue("TriggerMode", 2));
                m_smVSInfo[intVisionNo].g_intLightControllerType = Convert.ToInt32(subKey1.GetValue("LightControllerType", 1));
                m_smVSInfo[intVisionNo].g_intImageMergeType = Convert.ToInt32(subKey1.GetValue("ImageMerge", 0));
                m_smVSInfo[intVisionNo].g_intImageDisplayMode = Convert.ToInt32(subKey1.GetValue("ImageDisplayMode", 0));
                m_smVSInfo[intVisionNo].g_intRotateFlip = Convert.ToInt32(subKey1.GetValue("RotateFlip", 0));
                m_smVSInfo[intVisionNo].g_strVisionName = subKey1.GetValue("VisionName", "Vision " + (i + 10)).ToString();
                m_smVSInfo[intVisionNo].g_intVisionNameNo = Convert.ToInt32(subKey1.GetValue("VisionNameNo", 0));
                if (m_smVSInfo[intVisionNo].g_intVisionNameNo <= 1)
                    m_smVSInfo[intVisionNo].g_strVisionNameNo = "";
                else
                    m_smVSInfo[intVisionNo].g_strVisionNameNo = m_smVSInfo[intVisionNo].g_intVisionNameNo.ToString();
                m_smVSInfo[intVisionNo].g_strVisionDisplayName = subKey1.GetValue("VisionDisplayName", "Blank").ToString();
                m_smVSInfo[intVisionNo].g_strVisionNameRemark = subKey1.GetValue("VisionNameRemark", "").ToString();
                m_smVSInfo[intVisionNo].g_intCameraPortNo = Convert.ToInt32(subKey1.GetValue("PortNo", -1));
                m_smVSInfo[intVisionNo].g_uintCameraGUID = m_objImageAcquisition.GetCameraID(m_smVSInfo[intVisionNo].g_intCameraPortNo);
                m_smVSInfo[intVisionNo].g_strCameraSerialNo = subKey1.GetValue("SerialNo", "").ToString();
                string[] strResolution = subKey1.GetValue("Resolution", "640x480").ToString().Split('x');
                if (strResolution.Length == 2)
                {
                    m_smVSInfo[intVisionNo].g_intCameraResolutionWidth = Convert.ToInt32(strResolution[0]);
                    m_smVSInfo[intVisionNo].g_intCameraResolutionHeight = Convert.ToInt32(strResolution[1]);
                }

                // 2019 11 27 - CCENG: Add strVisionName to ArrVisionName List first. Else the intIndex will be always zero.
                arrVisionName.Add(m_smVSInfo[intVisionNo].g_strVisionName);

                int intIndex = 0;
                for (int j = 0; j < arrVisionName.Count; j++)
                {
                    if (arrVisionName[j] == m_smVSInfo[intVisionNo].g_strVisionName)
                    {
                        intIndex = j;   // 2019 11 27 - CCENG : Change from ++ to j
                    }
                }

                m_smVSInfo[intVisionNo].g_intVisionIndex = intIndex;
                if (intIndex > 0)
                {
                    for (int j = 0; j < arrVisionName.Count; j++)
                    {
                        if (m_smVSInfo[j].g_strVisionName == m_smVSInfo[intVisionNo].g_strVisionName)
                        {
                            m_smVSInfo[j].g_intVisionSameCount = intIndex;
                        }
                    }
                }

                m_smVSComThread[intVisionNo] = new VisionComThread(m_smProductionInfo, m_smVSInfo[intVisionNo], intVisionNo);

                m_smVSTCPIPIO[intVisionNo] = new TCPIPIO(m_smProductionInfo, m_smVSInfo[intVisionNo], intVisionNo);

                strVisionFeatureList = subKey1.GetValueNames();
                for (int j = 0; j < strVisionFeatureList.Length; j++)
                {
                    switch (strVisionFeatureList[j])
                    {
                        case "Barcode":
                            m_smCustomizeInfo.g_intWantBarcode |= (1 << intVisionNo);
                            break;
                        case "Mark":
                            m_smCustomizeInfo.g_intWantMark |= (1 << intVisionNo);
                            break;
                        case "Orient":
                            m_smCustomizeInfo.g_intWantOrient |= (1 << intVisionNo);
                            break;
                        case "Bottom":
                            m_smCustomizeInfo.g_intWantBottom |= (1 << intVisionNo);
                            break;
                        case "Pad":
                            m_smCustomizeInfo.g_intWantPad |= (1 << intVisionNo);
                            break;
                        case "Pad5S":
                            m_smCustomizeInfo.g_intWantPad5S |= (1 << intVisionNo);
                            break;
                        case "Package":
                            m_smCustomizeInfo.g_intWantPackage |= (1 << intVisionNo);
                            break;
                        case "Seal":
                            m_smCustomizeInfo.g_intWantSeal |= (1 << intVisionNo);
                            break;
                        case "OCR":
                            m_smCustomizeInfo.g_intWantOCR2 |= (0x01 << intVisionNo);
                            break;
                        case "ColorCamera":
                            m_smCustomizeInfo.g_intUseColorCamera |= (1 << intVisionNo);
                            break;
                        case "Position":
                            m_smCustomizeInfo.g_intWantPositioning |= (1 << intVisionNo);
                            break;
                        case "PositionIndex":
                            m_smCustomizeInfo.g_intWantPositioningIndex |= (1 << intVisionNo);
                            break;
                        case "RotatorSignal":
                            m_smCustomizeInfo.g_intWantRotatorSignal |= (1 << intVisionNo);
                            break;
                        case "UnitPresent":
                            m_smCustomizeInfo.g_intWantCheckPresent |= (1 << intVisionNo);
                            break;
                        case "Lead":
                            m_smCustomizeInfo.g_intWantLead |= (1 << intVisionNo);
                            break;
                        case "Lead3D":
                            m_smCustomizeInfo.g_intWantLead3D |= (1 << intVisionNo);
                            break;
                        case "2DCode":
                            m_smCustomizeInfo.g_intWant2DCode |= (1 << intVisionNo);
                            break;
                    }
                }

                if (((m_smCustomizeInfo.g_intWantMark & (1 << m_smVSInfo[intVisionNo].g_intVisionPos)) > 0) && ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVSInfo[intVisionNo].g_intVisionPos)) == 0))
                {
                    m_smCustomizeInfo.g_intWantOrient |= (1 << intVisionNo);
                    m_smCustomizeInfo.g_intWantOrient0Deg |= (1 << intVisionNo);
                }

            }
        }
        /// <summary>
        /// Check the selected language
        /// </summary>
        private void SetLanguageButton()
        {
            btn_ENGLanguage.Checked = ((m_smCustomizeInfo.g_intLanguageCulture & 0x01) == 1);
            btn_CHSLanguage.Checked = ((m_smCustomizeInfo.g_intLanguageCulture & 0x02) == 2);
            btn_CHTLanguage.Checked = ((m_smCustomizeInfo.g_intLanguageCulture & 0x04) == 4);
        }
        /// <summary>
        /// Set software information into registry such as software version
        /// </summary>
        private void SetRegistryKey()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SRMVision");

            subKey.SetValue("SoftwareVersion", Application.ProductVersion);
            subKey.SetValue("Created By", "SRM");
        }
        /// <summary>
        /// Start splash form to let user know that software is in loading mode
        /// </summary>
        private void StartSplash()
        {
            XmlParser objFileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            objFileHandle.GetFirstSection("General");
            m_objSplash = new SplashForm(m_blnUpsOn, m_smProductionInfo, m_smCustomizeInfo, objFileHandle.GetValueAsInt("Culture", 1));
            m_objSplash.StartSplash();
        }
        /// <summary>
        /// Stop all running thread
        /// </summary>
        private void StopThread()
        {
            if (m_thAutoPurge != null) m_thAutoPurge.StopThread();
            if (m_thThread != null) m_thThread.StopThread();
            if (m_thComThread != null) m_thComThread.StopThread();
            if (m_autoMode != null) m_autoMode.StopThread();
            if (m_thSaveRecipeThread != null) m_thSaveRecipeThread.StopThread();
            if (m_thSaveDataThread != null) m_thSaveDataThread.StopThread();
            if (m_thSECSGEMThread != null) m_thSECSGEMThread.StopThread();
        }

        /// <summary>
        /// Check whether the connected camera and registered camera are same in GUID and Camera amount
        /// </summary>
        /// <returns>false if GUID or camera amount is not tally</returns>
        private bool ValidateCamera()
        {
            string strMessage = "";
            List<int> arrCameraPortNoList = new List<int>();
            List<string> arrCameraIDList = new List<string>();
            List<string> arrCameraModelList = new List<string>();
            List<string> arrCameraSerialNumberList = new List<string>();
            m_objImageAcquisition.GetNodeList(ref arrCameraPortNoList, ref arrCameraIDList, ref arrCameraModelList, ref arrCameraSerialNumberList);

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            RegistryKey subKey1;

            string[] strSubkeyList = subKey.GetSubKeyNames();
            int intCount = strSubkeyList.Length;
            string[] strCameraList = new string[intCount];
            int[] intPortNoList = new int[intCount];
            string[] strVisionName = new string[intCount];
            int i, j;
            // Retrieve Camera(s)' GUID from registry
            for (i = 0; i < intCount; i++)
            {
                subKey1 = subKey.OpenSubKey(strSubkeyList[i]);
                intPortNoList[i] = Convert.ToInt32(subKey1.GetValue("PortNo", -1));
                strCameraList[i] = subKey1.GetValue("CameraID", "").ToString();
                strVisionName[i] = subKey1.GetValue("VisionName", "---").ToString();

                string strCameraModel = subKey1.GetValue("CameraModel", "AVT").ToString();

                if (strCameraModel == "AVT")
                {
                    // Check camera connect to registered ports or not
                    for (j = 0; j < arrCameraPortNoList.Count; j++)
                    {
                        if (intPortNoList[i] == arrCameraPortNoList[j])
                            break;
                    }

                    // Fail to find camera connect to registered port
                    if (j == arrCameraPortNoList.Count)
                    {
                        strMessage += "Port " + intPortNoList[i] + "-" + strVisionName[i] + " camera not connected.\n";
                    }
                }
            }

            // Check is there extra camera connect to unregistered port
            for (i = 0; i < arrCameraPortNoList.Count; i++)
            {
                for (j = 0; j < intCount; j++)
                {
                    if (arrCameraPortNoList[i] == intPortNoList[j])
                        break;
                }

                // Found extra camera connect to unregistered port
                if (j == intCount)
                {
                    strMessage += "Camera with ID:" + arrCameraIDList[i] + " has connected to unregistered port " + arrCameraPortNoList[i] + ".\n";
                }
            }

            if (strMessage.Length > 0)
            {
                if (!m_smProductionInfo.g_blnLoadSRMVisionForRefreshing)
                    SRMMessageBox.Show(strMessage);
            }

            return true;
        }

        private void StartWaiting(string StrMessage)
        {
            m_thWaitingFormThread = new SRMWaitingFormThread();
            m_thWaitingFormThread.SetStartSplash(StrMessage);
            this.Enabled = false;
        }

        private void StopWaiting()
        {
            m_thWaitingFormThread.SetStopSplash();
            this.Enabled = true;
        }

        private void btn_AboutSRM_Click(object sender, EventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog("Enter About SRM", "", "", "", m_smProductionInfo.g_strLotID);
            AboutSRMForm aboutForm = new AboutSRMForm();
            aboutForm.ShowDialog(this);
            aboutForm.Dispose();
            STDeviceEdit.SaveDeviceEditLog("Exit About SRM", "", "", "", m_smProductionInfo.g_strLotID);

#if (DEBUG || Debug_2_12 || RTXDebug)
            SetSoftwareToPriorityRealTime(false);
#endif
        }

        private void btn_Auto_Click(object sender, EventArgs e)
        {
            OpenAutoMode();

            Form[] mdiChild = this.MdiChildren;
            string strName = "";
            for (int i = 0; i < mdiChild.Length; i++)
            {
                strName = mdiChild[i].Name;
                if (strName == "Auto")
                    m_blnAutoWnd = mdiChild[i].Enabled;
                else if (strName == "IO")
                    m_blnIOWnd = mdiChild[i].Enabled;
                else if (strName == "History")
                    m_blnHistoryWnd = mdiChild[i].Enabled;
                else if (strName == "User Manager")
                    m_blnUserWnd = mdiChild[i].Enabled;
                else if (strName == "IOMap")
                    m_blnIOMapWnd = mdiChild[i].Enabled;
                else if (strName == "Diagnostic")
                    m_blnDiagnosticWnd = mdiChild[i].Enabled;
            }

            if (m_blnAutoWnd != m_blnAutoWndPrev)
            {
                m_blnAutoWndPrev = m_blnAutoWnd;
                EnableMenuItem(btn_Auto, !m_blnAutoWnd);
            }
            if (m_blnIOWnd != m_blnIOWndPrev)
            {
                m_blnIOWndPrev = m_blnIOWnd;
                EnableMenuItem(btn_IO, !m_blnIOWnd);
            }
            if (m_blnIOMapWnd != m_blnIOMapWndPrev)
            {
                m_blnIOMapWndPrev = m_blnIOMapWnd;
                EnableMenuItem(btn_IOMap, !m_blnIOMapWnd);
            }
            if (m_blnDiagnosticWnd != m_blnDiagnosticWndPrev)
            {
                m_blnDiagnosticWndPrev = m_blnDiagnosticWnd;
                EnableMenuItem(btn_Diagnostic, !m_blnDiagnosticWnd);
                btn_Diagnostic.Checked = m_blnDiagnosticWnd;
            }
            if (m_blnHistoryWnd != m_blnHistoryWndPrev)
            {
                m_blnHistoryWndPrev = m_blnHistoryWnd;
                EnableMenuItem(btn_History, !m_blnHistoryWnd);
            }
            if (m_blnUserWnd != m_blnUserWndPrev)
            {
                m_blnUserWndPrev = m_blnUserWnd;
                EnableMenuItem(btn_User, !m_blnUserWnd);
            }
        }

        private void btn_Config_Click(object sender, EventArgs e)
        {
            m_smProductionInfo.g_blnAutoLogOutBLock = true;

            Cursor.Current = Cursors.WaitCursor;
            
            STDeviceEdit.SaveDeviceEditLog("Enter App Config Form", "", "", "", m_smProductionInfo.g_strLotID);
            AppConfigForm objConfigForm = new AppConfigForm(m_smVSInfo, m_smProductionInfo, m_smCustomizeInfo);
            if (objConfigForm.ShowDialog(this) == DialogResult.OK)
            {
                InitSPC(false);
                InitTCPIP();
                InitNetwork();
                InitSECSGEM();
                if (m_strSECSGEMSharedFolderPathPrev != m_smCustomizeInfo.g_strSECSGEMSharedFolderPath || m_intSECSGEMMaxNoOfCoplanPadPrev != m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad)
                {
                    m_strSECSGEMSharedFolderPathPrev = m_smCustomizeInfo.g_strSECSGEMSharedFolderPath;
                    m_intSECSGEMMaxNoOfCoplanPadPrev = m_smCustomizeInfo.g_intSECSGEMMaxNoOfCoplanPad;
                    m_smProductionInfo.g_blnSECSGEMSInit = true;
                }


                m_thComThread.ReadFromXML();

                for (int i = 0; i < m_smVSComThread.Length; i++)
                {
                    if (m_smVSComThread[i] == null) continue;
                    m_smVSComThread[i].ReadFromXML();
                }

                for (int i = 0; i < m_smVSTCPIPIO.Length; i++)
                {
                    if (m_smVSTCPIPIO[i] == null) continue;
                    m_smVSTCPIPIO[i].ReadFromXML();
                }

                XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "\\Option.xml");
                objFile.GetFirstSection("SerialPort");
                if (objFile.GetValueAsBoolean("Enable", false))
                {
                    if (m_thCOMMPort == null)
                        m_thCOMMPort = new RS232();
                    m_thCOMMPort.Init(objFile.GetValueAsString("CommPort", "COM1"), objFile.GetValueAsInt("BitsPerSecond", 9600),
                        objFile.GetValueAsInt("DataBits", 8), objFile.GetValueAsInt("Parity", 0), objFile.GetValueAsInt("StopBits", 0));
                }
                else if (m_thCOMMPort != null)
                    m_thCOMMPort = null;


            }
            STDeviceEdit.SaveDeviceEditLog("Exit App Config Form", "", "", "", m_smProductionInfo.g_strLotID);
            
            objConfigForm.Dispose();

            m_smProductionInfo.g_blnAutoLogOutBLock = false;
        }

        private void btn_Diagnostic_Click(object sender, EventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog("Enter Diagnostic Form", "", "", "", m_smProductionInfo.g_strLotID);
            
            Cursor.Current = Cursors.WaitCursor;

            Form[] childForm = this.MdiChildren;
            int i = 0;
            for (i = 0; i < childForm.Length; i++)
            {
                if (childForm[i].Name == "Diagnostic")
                {
                    childForm[i].Activate();
                    break;
                }
            }

            if (i == childForm.Length)
            {
                DiagnosticBox objDiagnosticForm = new DiagnosticBox(m_thComThread, m_smVSInfo, m_smCustomizeInfo,
                    m_smProductionInfo, m_smVSComThread, m_thCOMMPort, m_smVSTCPIPIO);
                objDiagnosticForm.MdiParent = this;
                objDiagnosticForm.Name = "Diagnostic";
                objDiagnosticForm.Show();
            }
            //STDeviceEdit.SaveDeviceEditLog("Exit Diagnostic Form", "", "", "", m_smProductionInfo.g_strLotID);
        }

        private void btn_History_Click(object sender, EventArgs e)
        {
            m_smProductionInfo.g_blnAutoLogOutBLock = true;

            
            STDeviceEdit.SaveDeviceEditLog("Enter History Form", "", "", "", m_smProductionInfo.g_strLotID);
            
            OpenHistory();
            //STDeviceEdit.SaveDeviceEditLog("Exit History Form", "", "", "", m_smProductionInfo.g_strLotID);

            m_smProductionInfo.g_blnAutoLogOutBLock = false;
        }

        private void btn_Language_Click(object sender, EventArgs e)
        {
            if (sender == btn_ENGLanguage && m_smCustomizeInfo.g_intLanguageCulture == 1)
                return;
            else if (sender == btn_CHSLanguage && m_smCustomizeInfo.g_intLanguageCulture == 2)
                return;
            else if (sender == btn_CHTLanguage && m_smCustomizeInfo.g_intLanguageCulture == 4)
                return;

            if (SRMMessageBox.Show("SRMVision will restart after change language. Are you sure you want to change language?", "Change Language", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                return;

            Cursor.Current = Cursors.WaitCursor;
            string languagePrev = m_smCustomizeInfo.g_intLanguageCulture.ToString();

            if (sender == btn_ENGLanguage)
            {
                m_smCustomizeInfo.g_intLanguageCulture = 1;
                btn_ENGLanguage.Checked = true;
            }
            else if (sender == btn_CHSLanguage)
            {
                m_smCustomizeInfo.g_intLanguageCulture = 2;
                btn_CHSLanguage.Checked = true;
            }
            else if (sender == btn_CHTLanguage)
            {
                m_smCustomizeInfo.g_intLanguageCulture = 4;
                btn_CHTLanguage.Checked = true;
            }

            
            STDeviceEdit.SaveDeviceEditLog("General", "Change Language", languagePrev, m_smCustomizeInfo.g_intLanguageCulture.ToString(), m_smProductionInfo.g_strLotID);
            

            //Close neccesary form before change language
            //---------------------------------------------------------------------------------------------------
            StopThread();

            if (m_autoMode != null)
            {
                m_autoMode.Close();
                m_autoMode.Dispose();
                m_autoMode = null;
            }

            //Thread.Sleep(3000);
            if (m_objImageAcquisition != null)
                m_objImageAcquisition.OFFCamera();

            if (m_smCustomizeInfo.g_blnLEDiControl)
            {
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    for (int j = 0; j < m_smVSInfo[i].g_arrLightSource.Count; j++)
                    {
                        LightSource objLightSource = m_smVSInfo[i].g_arrLightSource[j];
                        LEDi_Control.ResetIntensity(objLightSource.ref_intPortNo, objLightSource.ref_intChannel);
                    }
                }

                LEDi_Control.Close();
            }
            else if (m_smCustomizeInfo.g_blnVTControl)
            {
                VT_Control.Close();
            }
            else
            {
                TCOSIO_Control.Close();
            }

            // 2018 07 13 - CCENG: Temporary force to use VTControl
            //VT_Control.Close();

            Form[] mdiChild = MdiChildren;
            //Make sure to ask for saving the doc before exiting the app
            for (int i = 0; i < mdiChild.Length; i++)
                mdiChild[i].Close();

            //---------------------------------------------------------------------------------------------------

            ChangeLanguage();

            //Cursor.Current = Cursors.Default;

            m_blnRestartApplication = true;
            Application.Exit();
            Close();
        }

        private void btn_IO_Click(object sender, EventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog("Enter IO Form", "", "", "", m_smProductionInfo.g_strLotID);
            
            OpenIOMode();
            //STDeviceEdit.SaveDeviceEditLog("Exit IO Form", "", "", "", m_smProductionInfo.g_strLotID);
        }

        private void btn_IOMap_Click(object sender, EventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog("Enter IOMap Form", "", "", "", m_smProductionInfo.g_strLotID);
            
            OpenIOMapping();
            //STDeviceEdit.SaveDeviceEditLog("Exit IOMap Form", "", "", "", m_smProductionInfo.g_strLotID);
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            // 2020 03 18 - JBTAN: block login if setting form is open
            if (m_smProductionInfo.AT_ALL_InAuto)
            {
                string strErrorMessage = "";
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (m_smVSInfo[i].VM_AT_SettingInDialog)
                    {
                        strErrorMessage += "- " + m_smVSInfo[i].g_strVisionDisplayName + "\n";
                        //return;
                    }
                }

                if (strErrorMessage.Length > 0)
                {
                    SRMMessageBox.Show("Please close below vision’s setting form and test form before login :\n" + strErrorMessage,
                                        "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    return;
                }

                Login(false);
                ReloadAutoMode();
            }
            else
            {
                Login(false);
            }
        }

        private void btn_Option_Click(object sender, EventArgs e)
        {
            m_smProductionInfo.g_blnAutoLogOutBLock = true;

#if (DEBUG || Debug_2_12 || RTXDebug)
            SetSoftwareToPriorityRealTime(true);
#endif
            
            STDeviceEdit.SaveDeviceEditLog("Enter Option Form", "", "", "", m_smProductionInfo.g_strLotID);
            OptionForm optionForm = new OptionForm(m_smVSInfo, m_smProductionInfo, m_smCustomizeInfo);
            if (optionForm.ShowDialog() == DialogResult.OK)
            {
                GetCustomizeOption();
            }
            STDeviceEdit.SaveDeviceEditLog("Exit Option Form", "", "", "", m_smProductionInfo.g_strLotID);
            
            optionForm.Dispose();

            m_smProductionInfo.g_blnAutoLogOutBLock = false;
        }

        private void btn_Recipe_Click(object sender, EventArgs e)
        {
            m_smProductionInfo.g_blnAutoLogOutBLock = true;

            
            STDeviceEdit.SaveDeviceEditLog("Enter Recipe Form", "", "", "", m_smProductionInfo.g_strLotID);
            RecipeForm objRecipeForm = new RecipeForm(m_intUserGroup, m_smProductionInfo, m_smCustomizeInfo, m_smVSInfo);
            objRecipeForm.ShowDialog();
            STDeviceEdit.SaveDeviceEditLog("Exit Recipe Form", "", "", "", m_smProductionInfo.g_strLotID);
            
            objRecipeForm.Dispose();

            m_smProductionInfo.g_blnAutoLogOutBLock = false;
        }

        private void btn_User_Click(object sender, EventArgs e)
        {
            m_smProductionInfo.g_blnAutoLogOutBLock = true;
            
            STDeviceEdit.SaveDeviceEditLog("Enter User Form", "", "", "", m_smProductionInfo.g_strLotID);
            Cursor.Current = Cursors.WaitCursor;
            UserManagerForm userManager = new UserManagerForm(m_intUserGroup, m_strUserName, m_smCustomizeInfo, m_smProductionInfo);
            userManager.ShowDialog(this);
            STDeviceEdit.SaveDeviceEditLog("Exit User Form", "", "", "", m_smProductionInfo.g_strLotID);
            
            userManager.Dispose();

            if (m_smCustomizeInfo.objNewUserRight != null)
                m_smCustomizeInfo.objNewUserRight.InitTable();

            m_smProductionInfo.g_blnAutoLogOutBLock = false;
        }

        private void btn_Quit_Click(object sender, EventArgs e)
        {
            if (!m_blnUpsOn)
            {
                if (SRMMessageBox.Show("Are you sure you want to Exit Program?", "Exit", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    
                    STDeviceEdit.SaveDeviceEditLog("Exit Program", "", "", "", m_smProductionInfo.g_strLotID);
                    
                    Application.Exit();
                }
                else
                    return;
            }
            Cursor.Current = Cursors.WaitCursor;

            Close();
        }



        private void MainForm_MdiChildActivate(object sender, EventArgs e)
        {
            Form[] mdiChild = this.MdiChildren;
            string strName = "";
            for (int i = 0; i < mdiChild.Length; i++)
            {
                strName = mdiChild[i].Name;
                if (strName == "Auto")
                    m_blnAutoWnd = mdiChild[i].Enabled;
                else if (strName == "IO")
                    m_blnIOWnd = mdiChild[i].Enabled;
                else if (strName == "History")
                    m_blnHistoryWnd = mdiChild[i].Enabled;
                else if (strName == "User Manager")
                    m_blnUserWnd = mdiChild[i].Enabled;
                else if (strName == "IOMap")
                    m_blnIOMapWnd = mdiChild[i].Enabled;
                else if (strName == "Diagnostic")
                    m_blnDiagnosticWnd = mdiChild[i].Enabled;
            }

            if (m_blnAutoWnd != m_blnAutoWndPrev)
            {
                m_blnAutoWndPrev = m_blnAutoWnd;
                EnableMenuItem(btn_Auto, !m_blnAutoWnd);
            }
            if (m_blnIOWnd != m_blnIOWndPrev)
            {
                m_blnIOWndPrev = m_blnIOWnd;
                EnableMenuItem(btn_IO, !m_blnIOWnd);
            }
            if (m_blnIOMapWnd != m_blnIOMapWndPrev)
            {
                m_blnIOMapWndPrev = m_blnIOMapWnd;
                EnableMenuItem(btn_IOMap, !m_blnIOMapWnd);
            }
            if (m_blnDiagnosticWnd != m_blnDiagnosticWndPrev)
            {
                m_blnDiagnosticWndPrev = m_blnDiagnosticWnd;
                EnableMenuItem(btn_Diagnostic, !m_blnDiagnosticWnd);
                btn_Diagnostic.Checked = m_blnDiagnosticWnd;
            }
            if (m_blnHistoryWnd != m_blnHistoryWndPrev)
            {
                m_blnHistoryWndPrev = m_blnHistoryWnd;
                EnableMenuItem(btn_History, !m_blnHistoryWnd);
            }
            if (m_blnUserWnd != m_blnUserWndPrev)
            {
                m_blnUserWndPrev = m_blnUserWnd;
                EnableMenuItem(btn_User, !m_blnUserWnd);
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            SetRegistryKey();

            timer1.Enabled = true;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void btn_OSK_Click(object sender, EventArgs e)
        {
            if (File.Exists("C:\\Driver\\SRM_OSK.exe"))
                Process.Start("C:\\Driver\\SRM_OSK.exe");
            else
                SRMMessageBox.Show("OSK Application cannot be found!", "Error");
        }

        private void timer_AutoLogOut_Tick(object sender, EventArgs e)
        {

            if (!m_smProductionInfo.g_blnAutoLogOutBLock && m_smProductionInfo.g_intAutoLogOutMinutes > 0 && m_intUserGroup != 5)
            {
                DateTime t = DateTime.Now;
                TimeSpan tSpan = t - m_smProductionInfo.g_DTStartAutoMode;

                if (tSpan.Minutes >= m_smProductionInfo.g_intAutoLogOutMinutes)
                {
                    m_smProductionInfo.g_DTStartAutoMode = DateTime.Now;
                    m_intUserGroup = 5;
                    m_strUserName = "Operator";

                    m_smProductionInfo.g_strOperatorID = m_strUserName;
                    m_smProductionInfo.g_intUserGroup = m_intUserGroup;

                    TSlbl_User.Text = m_strUserName;
                    if (m_intUserGroup == 5)
                        TSlbl_User.Text += "(Operator)";
                    else if (m_intUserGroup == 4)
                        TSlbl_User.Text += "(Technician)";
                    else if (m_intUserGroup == 3)
                        TSlbl_User.Text += "(Engineer)";
                    else if (m_intUserGroup == 2)
                        TSlbl_User.Text += "(Administrator)";
                    else if (m_intUserGroup == 1)
                        TSlbl_User.Text += "(SRM Vendor)";

                    CustomizeGUI();

                    if (m_smProductionInfo.AT_ALL_InAuto)
                        ReloadAutoMode();
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
#if (RELEASE || RTXRelease || Release_2_12)
            SetSoftwareToPriorityRealTime(false);
#endif
            ExitApplication();

            if (m_blnRestartApplication)
                System.Diagnostics.Process.Start(Application.ExecutablePath);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_smProductionInfo.g_blnTrackDeleteImage)
            {
                m_intTrackDeleteLoopCount++;

                if (m_intTrackDeleteLoopCount > 100)
                {
                    m_intTrackDeleteLoopCount = 0;
                    m_smProductionInfo.g_blnTrackDeleteImage = false;
                }

            }

            if (m_smProductionInfo.g_blnRefreshingON)
            {
                if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("A1 - stop timer1");
                timer1.Stop();
                timer2.Start();
                return;
            }

            TSlbl_Time.Text = DateTime.Now.ToLongTimeString();
            TSlbl_Date.Text = DateTime.Now.ToLongDateString();

            if (m_strRecipeID != m_smProductionInfo.g_strRecipeID)
            {
                m_strRecipeID = m_smProductionInfo.g_strRecipeID;

                this.Text = "SRM Optical Vision Systems. Recipe : " + m_smProductionInfo.g_strRecipeID;;
            }

            bool blnOperatingMode = false;
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (m_smVSInfo[i].g_intMachineStatus == 2)
                {
                    blnOperatingMode = true;
                    break;
                }
            }

            if (true)// (!blnOperatingMode)
            {
                m_intMousePositionX = m_smProductionInfo.g_intMousePositonX;
                if (m_intMousePositionX != m_intMousePositionXPrev)
                {
                    TSlbl_MousePositionX.Text = m_intMousePositionX.ToString();
                    m_intMousePositionXPrev = m_intMousePositionX;
                }

                m_intMousePositionY = m_smProductionInfo.g_intMousePositonY;
                if (m_intMousePositionY != m_intMousePositionYPrev)
                {
                    TSlbl_MousePositionY.Text = m_intMousePositionY.ToString();
                    m_intMousePositionYPrev = m_intMousePositionY;
                }

                m_intMousePixel = m_smProductionInfo.g_intMousePixel;
                if (m_intMousePixel != m_intMousePixelPrev)
                {
                    TSlbl_Pixel.Text = m_intMousePixel.ToString();
                    m_intMousePixelPrev = m_intMousePixel;
                }

                m_arrMouseRGBPixel = m_smProductionInfo.g_arrMouseRGBPixel;
                if (m_arrMouseRGBPixel != m_arrMouseRGBPixelPrev)
                {
                    TSlbl_RGBPixel.Text = m_arrMouseRGBPixel[0].ToString() + " , " + m_arrMouseRGBPixel[1].ToString() + " , " + m_arrMouseRGBPixel[2].ToString();
                    m_arrMouseRGBPixelPrev = m_arrMouseRGBPixel;
                }

                if (m_smProductionInfo.g_blnDisplayColorPixelInfo)
                {
                    if (!TSlbl_RGBPixelTitle.Visible)
                        TSlbl_RGBPixelTitle.Visible = true;

                    if (!TSlbl_RGBPixel.Visible)
                        TSlbl_RGBPixel.Visible = true;
                }
                else
                {
                    if (TSlbl_RGBPixelTitle.Visible)
                        TSlbl_RGBPixelTitle.Visible = false;

                    if (TSlbl_RGBPixel.Visible)
                        TSlbl_RGBPixel.Visible = false;
                }
            }

            if (m_blnMachineStatusPrev != blnOperatingMode)
            {
                if (m_smProductionInfo.g_blnTrackDeleteImage) STTrackLog.WriteLine("A2 - Machine Status change.");

                if (blnOperatingMode == true)
                    m_thAutoPurge.PauseThread();
                else
                    m_thAutoPurge.ResumeThread();

                m_blnMachineStatusPrev = blnOperatingMode;
            }

#if (RTXDebug || RTXRelease)

            if (m_smCustomizeInfo.g_blnAutoShutDown &&
               (m_smCustomizeInfo.g_blnShareHandlerPC && SRMSingleLock("APP2SRMVisionExitEvent")))
            {
                m_blnUpsOn = true;
                Close();
            }
#endif

            if (m_smCustomizeInfo.g_blnAutoShutDown &&
               (!m_smCustomizeInfo.g_blnShareHandlerPC && m_objVisionIO != null && m_objVisionIO.AutoShutDown.IsOff()))
            {
                // ----- Immediate Shutdown -----------
                //m_blnUpsOn = true;
                //StartSplash();
                //Thread.Sleep(3000);
                //CloseSplash();

                //WindowsController.ExitWindows(RestartOptions.PowerOff, false);
                //Close();
                //-------------------------------------

                if (m_blnCheckShutDown)
                {
                    if (!m_blnStartShutDownTimer)
                    {
                        objWait30sForm = new WaitShutdownForm();
                        objWait30sForm.Show();
                        objWait30sForm.TopMost = true;

                        tShutDownTimer.Start();
                        m_blnStartShutDownTimer = true;
                    }
                    else
                    {
                        objWait30sForm.UpdateTimerDisplay((int)(30 - (tShutDownTimer.Timing / 1000)));

                        bool blnShutDown = false;

                        if (tShutDownTimer.Timing < 30000)
                        {
                            if (objWait30sForm.ref_blnStopShutDown)
                            {
                                objWait30sForm.Dispose();
                                m_blnStartShutDownTimer = false;
                                m_blnCheckShutDown = false;
                            }

                            if (objWait30sForm.ref_blnShutDownNow)
                            {
                                objWait30sForm.Dispose();
                                blnShutDown = true;
                            }
                        }
                        else
                            blnShutDown = true;

                        if (blnShutDown)
                        {
                            m_blnUpsOn = true;
                            StartSplash();
                            Thread.Sleep(3000);
                            CloseSplash();

                            WindowsController.ExitWindows(RestartOptions.ShutDown, false);
                            Close();
                        }
                    }
                }
            }
            else
            {
                if (objWait30sForm != null)
                {
                    objWait30sForm.Dispose();
                    objWait30sForm = null;
                }

                m_blnCheckShutDown = true;
                m_blnStartShutDownTimer = false;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Stop();
            try
            {
                Application.Exit();
                Cursor.Current = Cursors.WaitCursor;
            }
            catch
            {
            }

            Close();
        }

        private void Event(object sender, EventArgs e)
        {
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker wkr = sender as BackgroundWorker;
            //Create Graphics object from the current windows handle
            Graphics GraphicsObject = Graphics.FromHwnd(IntPtr.Zero);
            //Get Handle to the device context associated with this Graphics object
            IntPtr DeviceContextHandle = GraphicsObject.GetHdc();
            //Call GetDeviceCaps with the Handle to retrieve the Screen Height
            int LogicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(DeviceContextHandle, (int)DeviceCap.DESKTOPVERTRES);
            double ScreenScalingFactor = Math.Round(PhysicalScreenHeight / (double)LogicalScreenHeight, 2);
            GraphicsObject.ReleaseHdc(DeviceContextHandle);
            GraphicsObject.Dispose();

            Bitmap bmpScreenCapture = new Bitmap((int)(Screen.PrimaryScreen.Bounds.Width * ScreenScalingFactor), (int)(Screen.PrimaryScreen.Bounds.Height * ScreenScalingFactor));
            Graphics g = Graphics.FromImage(bmpScreenCapture);
            g.CopyFromScreen(new Point(SystemInformation.VirtualScreen.Left, SystemInformation.VirtualScreen.Top), Point.Empty, bmpScreenCapture.Size);
            e.Result = bmpScreenCapture;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker wkr = sender as BackgroundWorker;
            if (e.Error != null)
            {
                Exception err = e.Error;
                while (err.InnerException != null)
                {
                    err = err.InnerException;
                }
            }
            else
            {
                if (e.Result is Bitmap)
                {
                    Bitmap bitmap = (Bitmap)e.Result;
                    using (SaveFileDialog dlg = new SaveFileDialog())
                    {
                        dlg.Title = "Image ScreenShot";
                        dlg.FileName = "";
                        dlg.InitialDirectory = "\\D:";
                        dlg.DefaultExt = "Png";
                        dlg.Filter = "JPEG(*.jpeg)|*.jpeg|Bitmap(*.bmp)|*.bmp|Png(*.png)|*.png";
                        if (dlg.ShowDialog(this) == DialogResult.OK)
                        {
                            if (dlg.FilterIndex == 2)
                            {
                                bitmap.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                            }
                            else if (dlg.FilterIndex == 3)
                            {
                                bitmap.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            }
                            else
                            {
                                bitmap.Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            }
                        }
                    }
                }
            }
        }
    }

    public static class MouseHook
    {
        public static event EventHandler MouseAction = delegate { };

        public static void Start()
        {
            _hookID = SetHook(_proc);


        }
        public static void stop()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private static LowLevelMouseProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                  GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
          int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_MOUSEMOVE == (MouseMessages)wParam)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                MouseAction(null, new EventArgs());
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
          LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
          IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


    }
}