using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using Common;
using Teli.TeliCamAPI.NET;
using Teli.TeliCamAPI.NET.Utility;

namespace ImageAcquisition
{
    public class TeliCameraAPI
    {
        #region Member Variables
        
        private double m_dRedRatioMin = 0;
        private double m_dRedRatioMax = 0;
        private double m_dBlueRatioMin = 0;
        private double m_dBlueRatioMax = 0;

        //Low Level API
        private const int BufferCount = 8;
        //This is your application AutoResetEvent for waiting an image for 200ms
        private System.Threading.AutoResetEvent _completeEvent = new System.Threading.AutoResetEvent(false);
        private IntPtr[] _imageBuffer;
        private ulong[] _requestHandle;
        private ulong[] curRequestHandle;


        //private HiPerfTimer m_intTImageAcquired = new HiPerfTimer();
        //private HiPerfTimer m_intTExposureEnd = new HiPerfTimer();
        private Size m_SImageSize;
        private bool m_blnCameraInitDone = false;
        private bool m_blnColorCamera = false;
        private bool m_blnIsGrabbing = false;
        private bool m_blnPauseGrab = false;
        private int m_intCameraPort;
        private IntPtr m_ptrImagePointer;
        private IntPtr m_ptrBufferPointer;
        private IntPtr[] m_arrBufferPointer = new IntPtr[10];
        private BitmapData m_objBitmapData;
        private int m_intNextGrabDelay = 4;
        private int m_intSetGainDelay = 0;
        private float m_fGainDivider = 1;
        private double m_dExposurePrev = double.MinValue;
        private double m_dGainPrev = double.MinValue;
        private CameraLineSource m_Line1SrcPrev = CameraLineSource.AcquisitionActive;
        private CameraLineSource m_Line2SrcPrev = CameraLineSource.AcquisitionActive;
        private string m_strErrorText = "";
        private int m_intImageCount = 0;
        public AutoResetEvent FrameAcquired = new AutoResetEvent(false);
        public AutoResetEvent ExposureDone = new AutoResetEvent(false);
        public AutoResetEvent TriggerWaitDone = new AutoResetEvent(false);
        public AutoResetEvent TriggerDone = new AutoResetEvent(false);
        //private enFireWrapResult m_fwResult;
        private CamApiStatus m_fwResult;
        private int number = 0;
        private static CameraSystem system = new CameraSystem();
        private CameraDevice device;
        //private FGFrame m_objGrabbedFrame = new FGFrame();
        //private FGNodeInfoContainer m_objCameraContainer = new FGNodeInfoContainer();                // store all camera ID and total count
        //private FireWrap_Camera m_objCamera = new FireWrap_Camera();                                      // Init camera connection and function
        //private FireWrap_CtrlCenter m_objCameraControlCentre = FireWrap_CtrlCenter.GetInstance();  // Init vision library

        private Bitmap m_bmpImage = new Bitmap(640, 480, PixelFormat.Format24bppRgb);              // 24 bits per pixel; 8 bits each are used for blue, green, red.
        //private List<FGFrame> m_arrGrabbedFrame = new List<FGFrame>();
        private List<Bitmap> m_arrbmpImage = new List<Bitmap>();
        #endregion

        #region Properties
        
        public double ref_dRedRatioMin { get { return m_dRedRatioMin; } }
        public double ref_dRedRatioMax { get { return m_dRedRatioMax; } }
        public double ref_dBlueRatioMin { get { return m_dBlueRatioMin; } }
        public double ref_dBlueRatioMax { get { return m_dBlueRatioMax; } }

        public bool ref_blnCameraInitDone { get { return m_blnCameraInitDone; } }
        public bool ref_blnColorCamera { get { return m_blnColorCamera; } }
        public BitmapData ref_objBitmapData { get { return m_objBitmapData; } set { m_objBitmapData = value; } }
        public IntPtr ref_ptrBufferPointer { get { return m_ptrBufferPointer; } set { m_ptrBufferPointer = value; } }
        public IntPtr[] ref_arrBufferPointer { get { return m_arrBufferPointer; } set { m_arrBufferPointer = value; } }
        public IntPtr ref_ptrImagePointer { get { return m_ptrImagePointer; } set { m_ptrImagePointer = value; } }
        public string ref_strErrorText { get { return m_strErrorText; } }
        public int ref_intNextGrabDelay { get { return m_intNextGrabDelay; } }
        public int ref_intSetGainDelay { get { return m_intSetGainDelay; } }
        public int ref_intImageCount { get { return m_intImageCount; } set { m_intImageCount = value; } }
        #endregion

        /// <summary>
        /// AVT Driver
        /// </summary>
        public TeliCameraAPI()
        {

        }
        /// <summary>
        /// AVT Driver
        /// </summary>
        /// <param name="intImageWidth">Grab image width</param>
        /// <param name="intImageHeight">Grab image height</param>
        public TeliCameraAPI(int intImageWidth, int intImageHeight)
        {
            m_bmpImage = new Bitmap(intImageWidth, intImageHeight, PixelFormat.Format24bppRgb);
        }


        /// <summary>
        /// Get all camera that connect to PC
        /// </summary>
        public ArrayList GetNodeList()
        {
            ArrayList arrNodelist = new ArrayList();

            if (system.GetNumOfCameras(out number) != CamApiStatus.Success || number == 0)
                return arrNodelist;

            Teli.TeliCamAPI.NET.CameraInfo info = null;

            for (int i = 0; i < number; i++)
            {
                if (system.GetCameraInformation(i, ref info) == CamApiStatus.Success)
                    arrNodelist.Add(info.SerialNumber);
            }

            return arrNodelist;
        }

        public void GetCameraWBParameter(ref uint intValueCB, ref uint intValueCR)
        {
            //m_objCamera.GetParameter(enFGParameter.E_WHITEBALCB, ref intValueCB);
            //m_objCamera.GetParameter(enFGParameter.E_WHITEBALCR, ref intValueCR);
            double valueR;
            double valueB;
            device.camControl.GetBalanceRatio(CameraBalanceRatioSelector.Red, out valueR);
            device.camControl.GetBalanceRatio(CameraBalanceRatioSelector.Blue, out valueB);
            intValueCB = Convert.ToUInt32(valueB);
            intValueCR = Convert.ToUInt32(valueR);
        }
        public void GetWhiteBalance(ref double dRedRatio, ref double dBlueRatio)
        {
            if (m_blnCameraInitDone)
            {
                device.camControl.GetBalanceRatio(CameraBalanceRatioSelector.Red, out dRedRatio);
                device.camControl.GetBalanceRatio(CameraBalanceRatioSelector.Blue, out dBlueRatio);
            }
        }

        /// <summary>
        /// Get all camera's ID and port no that connect to PC
        /// </summary>
        /// <param name="arrCameraPortNoList">Store camera port no list</param>
        /// <param name="arrCameraIDList">Store camera ID list</param>
        public void GetNodeList(ref List<int> arrCameraPortNoList, ref List<string> arrCameraIDList, ref List<string> arrCameraModelList)
        {
            arrCameraPortNoList.Clear();
            arrCameraIDList.Clear();
            arrCameraModelList.Clear();

            if (system.GetNumOfCameras(out number) != CamApiStatus.Success || number == 0)
                return;

            Teli.TeliCamAPI.NET.CameraInfo info = null;

            for (int i = 0; i < number; i++)
            {
                if (system.GetCameraInformation(i, ref info) == CamApiStatus.Success)
                {
                    arrCameraPortNoList.Add(i);
                    arrCameraIDList.Add(info.SerialNumber);
                    arrCameraModelList.Add(info.ModelName);
                }
            }
        }



        /// <summary>
        /// Check camera connection
        /// FGInitModule - prepare the whole library for use
        /// FGGetNodeList - Check how many camera is connect to system
        /// </summary>
        public bool CheckCameras()
        {
            ////FGInitModule - prepare the whole library for use
            //if (m_objCameraControlCentre.FGInitModule() != enFireWrapResult.E_NOERROR)
            //    return false;

            ////FGGetNodeList - Check how many camera is connect to system
            //if (m_objCameraContainer.FGGetNodeList() != enFireWrapResult.E_NOERROR)
            //    return false;

            if (system.Initialize() != CamApiStatus.Success)
            {
                m_strErrorText = "Fail System - Fail to initialize API";
                return false;
            }

            if (system.GetNumOfCameras(out number) != CamApiStatus.Success)
            {
                m_strErrorText = "Fail System - Fail to get number of cameras";
                return false;
            }

            return true;
        }
        /// <summary>
        /// Convert the grabbed frame (in AVT format) to Euresys Format and .Net format
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>

        /// <summary>
        /// Convert the grabbed frame (in AVT format) to Euresys Format and .Net format
        /// </summary>
        /// <param name="intFrameIndex">Frame index</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool ConvertFrame(int intFrameIndex)
        {
            return true;
        }
        /// <summary>
        /// Convert the grabbed frame (in AVT format) to Euresys Format and .Net format
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        public bool ConvertFrame()
        {
            //byte[] objImage = new byte[m_objGrabbedFrame.Length];
            //m_objGrabbedFrame.CloneData(objImage);                  //copies the frame from internal buffer to an array 


            //uint uintImageWidth = 0;
            //uint uintImageHeight = 0;
            //enColorMode objColorMode = new enColorMode();
            //// Get color mode
            //m_objCamera.GetCurrentImageFormat(ref uintImageWidth, ref uintImageHeight, ref objColorMode);
            //// Convert frame to RGGB bayer pattern if camera is color model
            //if (m_objCamera.DeviceModel == "Guppy F146C")
            //    FireWrap_Convert.Convert(m_bmpImage, objImage, uintImageWidth, uintImageHeight, objColorMode, enBayerPattern.E_BAYERPATTER_RGGB);
            ////Get a reference to the images pixel data
            //Rectangle objDimension = new Rectangle(0, 0, (int)uintImageWidth, (int)uintImageHeight);
            //// fix a portion of bitmap pixel data array in memory, access it directly and replace the bits in the bitmap with modified data
            //BitmapData bmpDataImage = m_bmpImage.LockBits(objDimension, ImageLockMode.ReadWrite, m_bmpImage.PixelFormat);
            //m_ptrImagePointer = bmpDataImage.Scan0;   // Scan0 = The address in memory of the fixed data array 

            //try
            //{
            //    //Copy the pixel data into the bitmap structure
            //    if (m_objCamera.DeviceModel == "Guppy F146C")
            //        Marshal.Copy(objImage, 0, m_ptrImagePointer, 0);
            //    else
            //        Marshal.Copy(objImage, 0, m_ptrImagePointer, objImage.Length);
            //    m_bmpImage.UnlockBits(bmpDataImage);
            //}
            //catch
            //{
            //    m_bmpImage.UnlockBits(bmpDataImage);
            //    m_blnIsGrabbing = false;
            //    return false;
            //}

            return true;
        }
        /// <summary>
        /// Reset the frame buffers
        /// </summary>
        /// <returns></returns>
        public bool DiscardFrame()
        {
            //try
            //{
            //    m_objCamera.DiscardFrames();
            //}
            //catch (Exception ex)
            //{
            //    m_strErrorText = "Discard Camera Frame Fail - " + ex.ToString();
            //    return false;
            //}

            return true;
        }

        public void Dispose()
        {
            device.Dispose();
        }

        /// <summary>
        /// Grab an image
        /// </summary>
        /// <returns>true if successfukk, false otherwise</returns>
        /// <summary>
        /// Grab an image
        /// </summary>
        /// <returns>true if successfukk, false otherwise</returns>
        public bool Grab()
        {
            if (!m_blnCameraInitDone)
                return false;


            int intCount = 0;
            while (m_blnPauseGrab && (intCount < 3000))
            {
                Thread.Sleep(1);
                intCount++;
            }

            if (intCount >= 3000)
            {
                m_strErrorText = "Fail Camera - PauseGrab status never end.";
                return false;
            }

            m_blnIsGrabbing = true;
            m_strErrorText = "";
            //STTrackLog.WriteLine("ExecuteSoftwareTrigger");
            ExposureDone.Reset();
            TriggerWaitDone.Reset();
            TriggerDone.Reset();

            m_fwResult = device.camControl.ExecuteSoftwareTrigger();

            if (m_fwResult != CamApiStatus.Success)
            {
                m_strErrorText = m_fwResult.ToString();
                m_blnIsGrabbing = false;
                return false;
            }

            return true;
        }
        public bool Grab(int intImageIndex)
        {
            if (!m_blnCameraInitDone)
                return false;


            int intCount = 0;
            while (m_blnPauseGrab && (intCount < 3000))
            {
                Thread.Sleep(1);
                intCount++;
            }

            if (intCount >= 3000)
            {
                m_strErrorText = "Fail Camera - PauseGrab status never end.";
                STTrackLog.WriteLine("PauseGrab status never end.");
                return false;
            }

            m_blnIsGrabbing = true;
            m_strErrorText = "";
            //STTrackLog.WriteLine("ExecuteSoftwareTrigger");
            ExposureDone.Reset();
            TriggerWaitDone.Reset();
            TriggerDone.Reset();

            if (curRequestHandle[intImageIndex] != 0)
            {
                CamApiStatus status = device.camStream.lowLevelApi.EnqueueRequest(curRequestHandle[intImageIndex]);
                if (status != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Error Status from EnqueueRequest : " + status.ToString();
                    STTrackLog.WriteLine("Error Status from EnqueueRequest : " + status.ToString());
                    m_blnIsGrabbing = false;
                    return false;
                }
            }

            m_fwResult = device.camControl.ExecuteSoftwareTrigger();

            if (m_fwResult != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Trigger Grab Fail" + m_fwResult.ToString();
                STTrackLog.WriteLine("Trigger Grab Fail" + m_fwResult.ToString());
                m_blnIsGrabbing = false;
                return false;
            }

            return true;
        }
        /// <summary>
        /// Get image frame after grab
        /// </summary>
        /// <returns></returns>
        //public bool GetFrame(ImageDrawing objEuresysImageDrawing)
        //{
        //   // TrackLog objTL = new TrackLog();

        //    m_strErrorText = "";

        //    if (!FrameAcquired.WaitOne(1000))
        //    {
        //        //objTL.WriteLine("GetFrame()->WaitOne False");
        //        m_strErrorText = "IDSCamera Get Frame Timeout!";
        //        return false;
        //    }

        //    try
        //    {
        //     //   objTL.WriteLine("GetFrame()->Load memory");

        //        objEuresysImageDrawing.LoadImageFromMemory(2048, 1088, m_ptrImagePointer);

        //       // objTL.WriteLine("GetFrame()->Load memory done");
        //        //Rectangle objDimension = new Rectangle(0, 0, m_bmpImage.Width, m_bmpImage.Height);
        //        //BitmapData bmpDataImage = m_bmpImage.LockBits(objDimension, ImageLockMode.ReadWrite, m_bmpImage.PixelFormat);
        //        //m_ptrImagePointer = bmpDataImage.Scan0;   // Scan0 = The address in memory of the fixed data array 

        //        //objEuresysImageDrawing.LoadImageFromMemory(m_ptrImagePointer, m_bmpImage);

        //        //m_bmpImage.UnlockBits(bmpDataImage);
        //    }
        //    catch (Exception ex)
        //    {
        //        m_strErrorText = "IDSCamera Get Frame Fail! " + ex.ToString();
        //        return false;
        //    }


        //    return true;
        //}
        /// <summary>
        /// Get image frame after grab
        /// </summary>
        /// <param name="intFrameIndex">Frame index</param>
        /// <returns></returns>
        public bool GetFrame(int intFrameIndex)
        {
            return true;
        }

        public bool WaitFrameReady()
        {
            //STTrackLog.WriteLine("Start Waiting");
            //HiPerfTimer tWait = new HiPerfTimer();
            //tWait.Start();
            if (!FrameAcquired.WaitOne(1000))
            {
                //tWait.Stop();
                //STTrackLog.WriteLine("tWait = " + tWait.Duration.ToString());

                m_strErrorText = "Fail Camera - TeliCamera Wait Frame Ready Timeout!";
                STTrackLog.WriteLine("TeliCamera Wait Frame Ready Timeout!");
                return false;
            }
            //tWait.Stop();
            //STTrackLog.WriteLine("tWait = " + tWait.Duration.ToString());

            return true;
        }
        public bool WaitFrameReady(int intImageIndex)
        {
            //STTrackLog.WriteLine("Start Waiting");
            //HiPerfTimer tWait = new HiPerfTimer();
            //tWait.Start();
            if (!FrameAcquired.WaitOne(1000))
            {
                //tWait.Stop();
                //STTrackLog.WriteLine("tWait = " + tWait.Duration.ToString());
                m_strErrorText = "Fail Camera - TeliCamera Wait Frame Ready Timeout!";
                STTrackLog.WriteLine("TeliCamera Wait Frame Ready Timeout!" + "----------> Image " + (intImageIndex + 1).ToString());
                return false;
            }
            //tWait.Stop();
            //STTrackLog.WriteLine("tWait = " + tWait.Duration.ToString());

            return true;
        }
        public bool WaitFrameAcquiredReady(int intImageIndex)
        {
            //STTrackLog.WriteLine("Start Waiting");
            //HiPerfTimer tWait = new HiPerfTimer();
            //tWait.Start();
            bool Flag;
            Flag = _completeEvent.WaitOne(200);

            //Console.WriteLine("AutoResetEvent Status : " + Flag);

            if (!Flag)
            {
                m_strErrorText = "Fail Camera - Image Wait Time Out 200ms";
                STTrackLog.WriteLine("Image Wait Time Out 200ms");
                return false;
            }
            curRequestHandle[intImageIndex] = 0;
            IntPtr imageBuffer = IntPtr.Zero;
            int payloadSize;

            CamApiStatus status = device.camStream.lowLevelApi.DequeueRequest(ref curRequestHandle[intImageIndex], ref imageBuffer, out payloadSize);
            if ((status == CamApiStatus.EmptyCompleteQueue) || (curRequestHandle[intImageIndex] == 0))
            {
                m_strErrorText = "Fail Camera - Error Status from DequeueRequest : " + status.ToString();
                STTrackLog.WriteLine("Error Status from DequeueRequest : " + status.ToString());
                return false;
            }

            m_arrBufferPointer[intImageIndex] = imageBuffer;//m_ptrBufferPointer 
            //ulong curBlockID;
            ////Funtion below only called to get image info and BlockID
            //if (!CheckStreamRequest(curRequestHandle, out curBlockID))
            //{
            //    Console.WriteLine("Error Status from CheckStreamRequest");
            //    return false;
            //}

            //if (!m_arrFrameAcquired[intImageIndex].WaitOne(1000))
            //{
            //    //tWait.Stop();
            //    //STTrackLog.WriteLine("tWait = " + tWait.Duration.ToString());
            //    m_strErrorText = "TeliCamera Wait Frame Acquired Ready Timeout!";
            //    STTrackLog.WriteLine("TeliCamera Wait Frame Acquired Ready Timeout!" + "----------> Image " + (intImageIndex + 1).ToString());
            //    return false;
            //}
            //tWait.Stop();
            //STTrackLog.WriteLine("tWait = " + tWait.Duration.ToString());

            return true;
        }
        public bool WaitExposureDone()
        {
            //STTrackLog.WriteLine("Start Waiting");
            //HiPerfTimer tWait = new HiPerfTimer();
            //tWait.Start();
            if (!ExposureDone.WaitOne(1000))
            {
                //tWait.Stop();
                //STTrackLog.WriteLine("tWait = " + tWait.Duration.ToString());

                m_strErrorText = "Fail Camera - TeliCamera Wait Exposure Done Timeout!";
                STTrackLog.WriteLine("TeliCamera Wait Exposure Done Timeout!");
                return false;
            }
            //tWait.Stop();
            //STTrackLog.WriteLine("tWait = " + tWait.Duration.ToString());

            return true;
        }
        public bool WaitTriggerWaitDone()
        {
            //STTrackLog.WriteLine("Start Waiting");
            //HiPerfTimer tWait = new HiPerfTimer();
            //tWait.Start();
            if (!TriggerWaitDone.WaitOne(2000))
            {
                //tWait.Stop();
                //STTrackLog.WriteLine("tWait = " + tWait.Duration.ToString());

                m_strErrorText = "Fail Camera - TeliCamera Wait Trigger Wait Done Timeout!";
                STTrackLog.WriteLine("TeliCamera Wait Trigger Wait Done Timeout!");
                return false;
            }
            //tWait.Stop();
            //STTrackLog.WriteLine("tWait = " + tWait.Duration.ToString());

            return true;
        }
        public bool WaitTriggerDone()
        {
            //STTrackLog.WriteLine("Start Waiting");
            //HiPerfTimer tWait = new HiPerfTimer();
            //tWait.Start();
            if (!TriggerDone.WaitOne(1000))
            {
                //tWait.Stop();
                //STTrackLog.WriteLine("tWait = " + tWait.Duration.ToString());

                m_strErrorText = "Fail Camera - TeliCamera Wait Trigger Done Timeout!";
                STTrackLog.WriteLine("TeliCamera Wait Trigger Done Timeout!");
                return false;
            }
            //tWait.Stop();
            //STTrackLog.WriteLine("tWait = " + tWait.Duration.ToString());

            return true;
        }

        public static string InitializeSystem()
        {
            if (system.Initialize() != CamApiStatus.Success)
            {
                //STTrackLog.WriteLine("InitializeSystem() -> Fail System - Fail to initialize API");
                return "Fail System - Fail to initialize API";
            }

            return "";
        }

        public bool InitializeCamera(String SerialNo, string strUserDefineName, int intResolutionX, int intResolutionY)
        {
            //STTrackLog.WriteLine("InitializeCamera SN=" + SerialNo + ", UserDefineName=" + strUserDefineName + ", ResX=" + intResolutionX.ToString() + ", RexY=" + intResolutionY.ToString());
            bool bFound = false;
            int payload = 0;
            Teli.TeliCamAPI.NET.CameraInfo info = null;

            if (system.GetNumOfCameras(out number) != CamApiStatus.Success || number == 0)
            {
                m_strErrorText = "Fail Camera - Unable to find any camera(s) connected";
                return false;
            }

            if (SerialNo.Length == 0 && strUserDefineName.Length == 0)
            {
                m_strErrorText = "Fail Camera - SerialNo or UserDefineName should not be empty.";
                return false;
            }


            for (int i = 0; i < number; i++)
            {
                if (system.GetCameraInformation(i, ref info) == CamApiStatus.Success)
                {
                    if (SerialNo.Length > 0)
                    {
                        if (info.SerialNumber == SerialNo || SerialNo == "1")
                        {
                            bFound = true;
                        }
                        else
                            continue;
                    }

                    if (strUserDefineName.Length > 0)
                    {
                        if (info.UserDefinedName == strUserDefineName)
                        {
                            //STTrackLog.WriteLine("Match User Define Name ==" + info.UserDefinedName);
                            bFound = true;
                        }
                        else
                            continue;
                    }

                    if (bFound)
                    {
                        //STTrackLog.WriteLine("Get device");
                        system.CreateDeviceObject(i, ref device);
                        break;
                    }

                }
            }

            switch (info.ModelName)
            {
                case "BU040":
                case "BU505MCF":
                case "DDU1207MCF":
                case "BU302MG":
                    m_fGainDivider = 2.17f; // 2019 04 29 - ZJYeoh: 1 + (50 / 2.17f) = 24. 24 is the max gain value for BU030/031
                    break;
                case "BU030":
                case "BU031":
                    m_fGainDivider = 2.94f; // 2018 08 09 - JBTAN: 1 + (50 / 2.94f) = 18. 18 is the max gain value for BU030/031
                    break;
                case "BU505MG":
                case "BU160MCF":
                    m_fGainDivider = 1.43f; // 2020 08 27 - ZJYEOH: 1 + (50 / 1.43f) = 36. 36 is the max gain value for BU030/031
                    break;
                default:
                case "BU160":
                case "BU205M":
                    m_fGainDivider = 7.14f; // 2018 08 09 - JBTAN: 1 + (50 / 7.14f) = 8. 8 is the max gain value for BU205M
                    break;
            }


            if (bFound)
            {
                device.ResetPort();
                if (device.Open() != CamApiStatus.Success)
                {
                    device.ResetPort();
                    m_strErrorText = "Fail Camera - Fail to connect with real camera";
                    return false;
                }
            }
            else
            {
                m_strErrorText = "Fail Camera - No camera serial number match to it.";
                return false;
            }

            //if (device.CamType == CameraType.TypeU3v)
            //{
            //    //// Set ExposureTimeControl "Manual", in this sample code.
            //    //if (device.camControl.SetExposureTimeControl(CameraExposureTimeCtrl.Manual) != CamApiStatus.Success)
            //    //{
            //    //    m_strErrorText = "Fail Camera - SetExposureTimeControl error";
            //    //    return false;
            //    //}
            //}
            //else
            //{
            //    // Set ExposureTimeControl "Manual", in this sample code. (ExposureAuto = Off)
            //    if (device.camControl.SetExposureTimeControl(CameraExposureTimeCtrl.Manual) != CamApiStatus.Success)
            //    {
            //        m_strErrorText = "Fail Camera - SetExposureTimeControl error";
            //        return false;
            //    }

            //    // Set GainAuto off.
            //    if (device.camControl.SetGainAuto(CameraGainAuto.Off) != CamApiStatus.Success)
            //    {
            //        m_strErrorText = "Fail Camera - SetGainAuto error";
            //        return false;
            //    }
            //}

            int intWidth = 0;
            int intHeight = 0;
            int intOffsetX = 0;
            int intOffsetY = 0;
            int intSensorSizeX = 0;
            int intSensorSizeY = 0;
            device.camControl.GetRoi(out intWidth, out intHeight, out intOffsetX, out intOffsetY);
            device.camControl.GetSensorWidth(out intSensorSizeX);
            device.camControl.GetSensorHeight(out intSensorSizeY);
            int intNewOffsetX = intSensorSizeX / 2 - intResolutionX / 2;
            int intNewOffsetY = intSensorSizeY / 2 - intResolutionY / 2;
            if (intNewOffsetX % 8 != 0)
            {
                intNewOffsetX = intNewOffsetX - (intNewOffsetX % 8);
            }
            if (intNewOffsetY % 8 != 0)
            {
                intNewOffsetY = intNewOffsetY - (intNewOffsetY % 8);
            }


            if ((intResolutionX != intWidth) || (intResolutionY != intHeight) ||
                (intNewOffsetX != intOffsetX) || (intNewOffsetY != intOffsetY))
            {
                if (device.camControl.SetRoi(intResolutionX, intResolutionY, intNewOffsetX, intNewOffsetY) != CamApiStatus.Success)
                {
                    OFFCamera();
                    m_strErrorText = "Fail Camera - Fail to set image ROI.[" + intResolutionX.ToString() +
                        ", " + intResolutionY.ToString() +
                        ", " + intNewOffsetX.ToString() +
                        ", " + intNewOffsetY.ToString();
                    return false;
                }
            }

            if (device.camControl.SetGamma(1) != CamApiStatus.Success) // dValue is in um
            {
                m_strErrorText = "Fail Camera - Fail to set gammma";
                return false;
            }

            //Set trigger mode on and trigger source to software trigger
            if (device.camControl.SetTriggerMode(true) != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Fail to set trigger mode";
                return false;
            }
            if (device.camControl.SetTriggerSource(CameraTriggerSource.Software) != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Fail to set trigger source";
                return false;
            }


            CameraPixelFormat pixelFormat = CameraPixelFormat.Mono8;
            if (device.camControl.GetPixelFormat(out pixelFormat) != CamApiStatus.Success)
            {
                pixelFormat = CameraPixelFormat.Mono8;
            }

            if (pixelFormat != CameraPixelFormat.Mono8)
            {
                //White Balance Red Raw = 130515, Blue Raw = 98226, dRedRatio =  1.991500854, dBlueRatio = 1.498809814
                //double dRedRatio = 1.991500854;
                //double dBlueRatio = 1.498809814;
                //White Balance (ori from camera) Red Raw = 151388, Blue Raw = 166460, dRedRatio =  2.309997559, dBlueRatio = 2.539978027
                if (pixelFormat != CameraPixelFormat.BayerBG8)
                {
                    device.camControl.SetPixelFormat(CameraPixelFormat.BayerBG8);
                }

                double dRedRatio = 2.309997559;
                double dBlueRatio = 2.539978027;
                if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Red, dRedRatio) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to set White Balance Red Ratio";
                }

                if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Blue, dBlueRatio) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to set White Balance Blue Ratio";
                }
            }

            uint lineMode = 6;
            if (device.camControl.GetLineModeAll(out lineMode) != CamApiStatus.Success)
            {
                lineMode = 6;
            }

            if (lineMode != 6)
            {
                if (device.camControl.SetLineModeAll(6) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to camera line mode all";
                }
            }

            //STTrackLog.WriteLine("Init -> 1");

            if (device.CamType == CameraType.TypeU3v)
            {
                //Open Stream USB3
                if (device.camStream.Open(out payload) != CamApiStatus.Success)
                {
                    //STTrackLog.WriteLine("Init -> 11");
                    m_strErrorText = "Fail Camera - Fail to open stream";
                    return false;
                }
            }
            else
            {
                //Open Stream GigE set 9000 packet size
                if (device.camStream.Open(null, 0, 9000, out payload) != CamApiStatus.Success)
                {
                    //STTrackLog.WriteLine("Init -> 11");
                    m_strErrorText = "Fail Camera - Fail to open stream";
                    return false;
                }
            }
            //STTrackLog.WriteLine("Init -> 2");

            //Frame     Acquired Event
            device.camStream.ImageAcquired += new ImageAcquiredEventHandler(ImageAcquired);

            if (device.CamType == CameraType.TypeU3v)
            {
                //Open Event Interface
                if (device.camEvent.Open() != CamApiStatus.Success)
                {
                    //STTrackLog.WriteLine("Init -> 33");
                    m_strErrorText = "Fail Camera - Fail to open event";
                    return false;
                }

                //GigE camera dont have ExposureEnd event
                //STTrackLog.WriteLine("Init -> 4");
                //Activate Exposure End Event
                if (device.camEvent.Activate(CameraEventType.ExposureEnd) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to activate event";
                    return false;
                }

                device.camEvent.ExposureEndReceived += new CameraEventReceivedEventHandler(ExposureEnd);
            }
            //Start Stream
            if (device.camStream.Start() != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Fail to start stream";
                return false;
            }

            m_blnCameraInitDone = true;

            //STTrackLog.WriteLine("Connect Camera Pass. Serial No " + SerialNo);

            return true;
        }
        public bool InitializeCamera_LowLevelAPI(String SerialNo, string strUserDefineName, int intResolutionX, int intResolutionY, bool blnColor)
        {
            //STTrackLog.WriteLine("InitializeCamera SN=" + SerialNo + ", UserDefineName=" + strUserDefineName + ", ResX=" + intResolutionX.ToString() + ", RexY=" + intResolutionY.ToString());
            bool bFound = false;
            int payload = 0;
            Teli.TeliCamAPI.NET.CameraInfo info = null;

            if (system.GetNumOfCameras(out number) != CamApiStatus.Success || number == 0)
            {
                m_strErrorText = "Fail Camera - Unable to find any camera(s) connected";
                STTrackLog.WriteLine("Unable to find any camera(s) connected");
                return false;
            }

            if (SerialNo.Length == 0 && strUserDefineName.Length == 0)
            {
                m_strErrorText = "Fail Camera - SerialNo or UserDefineName should not be empty.";
                STTrackLog.WriteLine("SerialNo or UserDefineName should not be empty.");
                return false;
            }


            for (int i = 0; i < number; i++)
            {
                if (system.GetCameraInformation(i, ref info) == CamApiStatus.Success)
                {
                    if (SerialNo.Length > 0)
                    {
                        if (info.SerialNumber == SerialNo || SerialNo == "1")
                        {
                            bFound = true;
                        }
                        else
                            continue;
                    }

                    if (strUserDefineName.Length > 0)
                    {
                        if (info.UserDefinedName == strUserDefineName)
                        {
                            //STTrackLog.WriteLine("Match User Define Name ==" + info.UserDefinedName);
                            bFound = true;
                        }
                        else
                            continue;
                    }

                    if (bFound)
                    {
                        //STTrackLog.WriteLine("Get device");
                        system.CreateDeviceObject(i, ref device);
                        break;
                    }

                }
            }

            switch (info.ModelName)
            {
                case "BU040":
                case "BU505MCF":
                case "DDU1207MCF":
                case "BU302MG":
                    m_fGainDivider = 2.17f; // 2019 04 29 - ZJYeoh: 1 + (50 / 2.17f) = 24. 24 is the max gain value for BU030/031
                    break;
                case "BU030":
                case "BU031":
                    m_fGainDivider = 2.94f; // 2018 08 09 - JBTAN: 1 + (50 / 2.94f) = 18. 18 is the max gain value for BU030/031
                    break;
                case "BU505MG":
                case "BU160MCF":
                    m_fGainDivider = 1.43f; // 2020 08 27 - ZJYEOH: 1 + (50 / 1.43f) = 36. 36 is the max gain value for BU030/031
                    break;
                default:
                case "BU160":
                case "BU205M":
                    m_fGainDivider = 7.14f; // 2018 08 09 - JBTAN: 1 + (50 / 7.14f) = 8. 8 is the max gain value for BU205M
                    break;
            }


            if (bFound)
            {
                device.ResetPort();
                if (device.Open() != CamApiStatus.Success)
                {
                    device.ResetPort();
                    m_strErrorText = "Fail Camera - Fail to connect with real camera";
                    STTrackLog.WriteLine("Fail to connect with real camera");
                    return false;
                }
            }
            else
            {
                m_strErrorText = "Fail Camera - No camera serial number match to it.";
                STTrackLog.WriteLine("No camera serial number match to it.");
                return false;
            }

            //if (device.CamType == CameraType.TypeU3v)
            //{
            //    //// Set ExposureTimeControl "Manual", in this sample code.
            //    //if (device.camControl.SetExposureTimeControl(CameraExposureTimeCtrl.Manual) != CamApiStatus.Success)
            //    //{
            //    //    m_strErrorText = "Fail Camera - SetExposureTimeControl error";
            //    //    return false;
            //    //}
            //}
            //else
            //{
            //    // Set ExposureTimeControl "Manual", in this sample code. (ExposureAuto = Off)
            //    if (device.camControl.SetExposureTimeControl(CameraExposureTimeCtrl.Manual) != CamApiStatus.Success)
            //    {
            //        m_strErrorText = "Fail Camera - SetExposureTimeControl error";
            //        return false;
            //    }

            //    // Set GainAuto off.
            //    if (device.camControl.SetGainAuto(CameraGainAuto.Off) != CamApiStatus.Success)
            //    {
            //        m_strErrorText = "Fail Camera - SetGainAuto error";
            //        return false;
            //    }
            //}

            int intWidth = 0;
            int intHeight = 0;
            int intOffsetX = 0;
            int intOffsetY = 0;
            int intSensorSizeX = 0;
            int intSensorSizeY = 0;
            device.camControl.GetRoi(out intWidth, out intHeight, out intOffsetX, out intOffsetY);
            device.camControl.GetSensorWidth(out intSensorSizeX);
            device.camControl.GetSensorHeight(out intSensorSizeY);
            int intNewOffsetX = intSensorSizeX / 2 - intResolutionX / 2;
            int intNewOffsetY = intSensorSizeY / 2 - intResolutionY / 2;
            if (intNewOffsetX % 8 != 0)
            {
                intNewOffsetX = intNewOffsetX - (intNewOffsetX % 8);
            }
            if (intNewOffsetY % 8 != 0)
            {
                intNewOffsetY = intNewOffsetY - (intNewOffsetY % 8);
            }

            if ((m_SImageSize.Width != intResolutionX) || (m_SImageSize.Height != intResolutionY))
                m_SImageSize = new Size(intResolutionX, intResolutionY);

            if ((intResolutionX != intWidth) || (intResolutionY != intHeight) ||
                (intNewOffsetX != intOffsetX) || (intNewOffsetY != intOffsetY))
            {
                if (device.camControl.SetRoi(intResolutionX, intResolutionY, intNewOffsetX, intNewOffsetY) != CamApiStatus.Success)
                {
                    OFFCamera();
                    m_strErrorText = "Fail Camera - Fail to set image ROI.[" + intResolutionX.ToString() +
                        ", " + intResolutionY.ToString() +
                        ", " + intNewOffsetX.ToString() +
                        ", " + intNewOffsetY.ToString();
                    STTrackLog.WriteLine("Fail to set image ROI.[" + intResolutionX.ToString() +
                        ", " + intResolutionY.ToString() +
                        ", " + intNewOffsetX.ToString() +
                        ", " + intNewOffsetY.ToString());
                    return false;
                }
            }

            if (device.camControl.SetGamma(1) != CamApiStatus.Success) // dValue is in um
            {
                m_strErrorText = "Fail Camera - Fail to set gammma";
                STTrackLog.WriteLine("Fail to set gammma");
                return false;
            }

            //Set trigger mode on and trigger source to software trigger
            if (device.camControl.SetTriggerMode(true) != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Fail to set trigger mode";
                STTrackLog.WriteLine("Fail to set trigger mode");
                return false;
            }
            if (device.camControl.SetTriggerSource(CameraTriggerSource.Software) != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Fail to set trigger source";
                STTrackLog.WriteLine("Fail to set trigger source");
                return false;
            }


            if (blnColor)
            {
                CameraPixelFormat pixelFormat_Set;
                switch (info.ModelName)
                {
                    case "BU160":
                    default:
                        pixelFormat_Set = CameraPixelFormat.BayerBG8;

                        CameraPixelFormat pixelFormat_Current = CameraPixelFormat.BayerBG8;
                        if (device.camControl.GetPixelFormat(out pixelFormat_Current) == CamApiStatus.Success)
                        {
                            if (pixelFormat_Current != pixelFormat_Set)
                            {
                                device.camControl.SetPixelFormat(CameraPixelFormat.BayerBG8);

                            }

                            //White Balance Red Raw = 130515, Blue Raw = 98226, dRedRatio =  1.991500854, dBlueRatio = 1.498809814
                            //double dRedRatio = 1.991500854;
                            //double dBlueRatio = 1.498809814;
                            //White Balance (ori from camera) Red Raw = 151388, Blue Raw = 166460, dRedRatio =  2.309997559, dBlueRatio = 2.539978027

                            double dRedRatio = 2.309997559;
                            double dBlueRatio = 2.539978027;
                            //double dRedRatio = 2.242446899;
                            //double dBlueRatio = 3.343200684;

                            device.camControl.GetBalanceRatioMinMax(CameraBalanceRatioSelector.Red, out m_dRedRatioMin, out m_dRedRatioMax);
                            device.camControl.GetBalanceRatioMinMax(CameraBalanceRatioSelector.Blue, out m_dBlueRatioMin, out m_dBlueRatioMax);

                            if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Red, dRedRatio) != CamApiStatus.Success)
                            {
                                m_strErrorText = "Fail Camera - Fail to set White Balance Red Ratio";
                                STTrackLog.WriteLine("Fail to set White Balance Red Ratio");
                            }

                            if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Blue, dBlueRatio) != CamApiStatus.Success)
                            {
                                m_strErrorText = "Fail Camera - Fail to set White Balance Blue Ratio";
                                STTrackLog.WriteLine("Fail to set White Balance Blue Ratio");
                            }
                        }
                        break;
                }
            }
            else
            {
                CameraPixelFormat pixelFormat = CameraPixelFormat.Mono8;
                if (device.camControl.GetPixelFormat(out pixelFormat) != CamApiStatus.Success)
                {
                    pixelFormat = CameraPixelFormat.Mono8;
                }

                if (pixelFormat != CameraPixelFormat.Mono8)
                {
                    //White Balance Red Raw = 130515, Blue Raw = 98226, dRedRatio =  1.991500854, dBlueRatio = 1.498809814
                    //double dRedRatio = 1.991500854;
                    //double dBlueRatio = 1.498809814;
                    //White Balance (ori from camera) Red Raw = 151388, Blue Raw = 166460, dRedRatio =  2.309997559, dBlueRatio = 2.539978027
                    if (pixelFormat != CameraPixelFormat.BayerBG8)
                    {
                        device.camControl.SetPixelFormat(CameraPixelFormat.BayerBG8);
                    }

                    double dRedRatio = 2.309997559;
                    double dBlueRatio = 2.539978027;
                    if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Red, dRedRatio) != CamApiStatus.Success)
                    {
                        m_strErrorText = "Fail Camera - Fail to set White Balance Red Ratio";
                        STTrackLog.WriteLine("Fail to set White Balance Red Ratio");
                    }

                    if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Blue, dBlueRatio) != CamApiStatus.Success)
                    {
                        m_strErrorText = "Fail Camera - Fail to set White Balance Blue Ratio";
                        STTrackLog.WriteLine("Fail to set White Balance Blue Ratio");
                    }
                }
            }

            uint lineMode = 6;
            if (device.camControl.GetLineModeAll(out lineMode) != CamApiStatus.Success)
            {
                lineMode = 6;
            }

            if (lineMode != 6)
            {
                if (device.camControl.SetLineModeAll(6) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to camera line mode all";
                    STTrackLog.WriteLine("Fail to camera line mode all");
                }
            }

            //device.camControl.SetImageBufferMode(CameraImageBufferMode.On);
            //STTrackLog.WriteLine("Init -> 1");

            if (device.CamType == CameraType.TypeU3v)
            {
                InitializeBuffer();
                //Open Stream USB3
                if (!OpenCamStream())//(device.camStream.Open(out payload) != CamApiStatus.Success)
                {
                    //STTrackLog.WriteLine("Init -> 11");
                    m_strErrorText = "Fail Camera - Fail to open stream";
                    STTrackLog.WriteLine("Fail to open stream");
                    return false;
                }
            }
            else
            {
                //Open Stream GigE set 9000 packet size
                if (device.camStream.Open(null, 0, 9000, out payload) != CamApiStatus.Success)
                {
                    //STTrackLog.WriteLine("Init -> 11");
                    m_strErrorText = "Fail Camera - Fail to open stream";
                    STTrackLog.WriteLine("Fail to open stream");
                    return false;
                }
            }
            //STTrackLog.WriteLine("Init -> 2");

            //Frame     Acquired Event
            //device.camStream.ImageAcquired += new ImageAcquiredEventHandler(ImageAcquired);

            if (device.CamType == CameraType.TypeU3v)
            {
                //Open Event Interface
                if (device.camEvent.Open() != CamApiStatus.Success)
                {
                    //STTrackLog.WriteLine("Init -> 33");
                    m_strErrorText = "Fail Camera - Fail to open event";
                    STTrackLog.WriteLine("Fail to open event");
                    return false;
                }

                //GigE camera dont have ExposureEnd event
                //STTrackLog.WriteLine("Init -> 4");
                //Activate Exposure End Event
                if (device.camEvent.Activate(CameraEventType.ExposureEnd) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to activate event";
                    STTrackLog.WriteLine("Fail to activate event");
                    return false;
                }

                device.camEvent.ExposureEndReceived += new CameraEventReceivedEventHandler(ExposureEnd);

                if (device.camEvent.Activate(CameraEventType.FrameTriggerWait) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to activate event";
                    STTrackLog.WriteLine("Fail to activate event");
                    return false;
                }

                device.camEvent.FrameTriggerWaitReceived += new CameraEventReceivedEventHandler(FrameTriggerWait);

                if (device.camEvent.Activate(CameraEventType.FrameTrigger) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to activate event";
                    STTrackLog.WriteLine("Fail to activate event");
                    return false;
                }

                device.camEvent.FrameTriggerReceived += new CameraEventReceivedEventHandler(FrameTrigger);

                if (device.camEvent.Activate(CameraEventType.FrameTriggerError) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to activate event";
                    STTrackLog.WriteLine("Fail to activate event");
                    return false;
                }

                device.camEvent.FrameTriggerErrorReceived += new CameraEventReceivedEventHandler(FrameTriggerError);

            }

            //Start Stream
            if (device.camStream.Start() != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Fail to start stream";
                STTrackLog.WriteLine("Fail to start stream");
                return false;
            }

            m_blnCameraInitDone = true;

            //STTrackLog.WriteLine("Connect Camera Pass. Serial No " + SerialNo);

            return true;
        }
        public void SetWhiteBalance(double dRedRatio, double dBlueRatio)
        {
            //dRedRatio = 2.309997559;
            //dBlueRatio = 2.539978027;
            if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Red, dRedRatio) != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Fail to set White Balance Red Ratio";
                STTrackLog.WriteLine("Fail to set White Balance Red Ratio");
            }

            if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Blue, dBlueRatio) != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Fail to set White Balance Blue Ratio";
                STTrackLog.WriteLine("Fail to set White Balance Blue Ratio");
            }
        }
        public void SetWhiteBalance_ForRed(double dRedRatio)
        {
            if (dRedRatio < m_dRedRatioMin || dRedRatio > m_dRedRatioMax)
            {
                m_strErrorText = "Fail Camera - Fail to set White Balance Red Ratio";
                STTrackLog.WriteLine("Fail to set White Balance Red Ratio");
            }
            else
            {
                if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Red, dRedRatio) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to set White Balance Red Ratio";
                    STTrackLog.WriteLine("Fail to set White Balance Red Ratio");
                }
            }
        }
        public void SetWhiteBalance_ForBlue(double dBlueRatio)
        {
            if (dBlueRatio < m_dBlueRatioMin || dBlueRatio > m_dBlueRatioMax)
            {
                m_strErrorText = "Fail Camera - Fail to set White Balance Blue Ratio";
                STTrackLog.WriteLine("Fail to set White Balance Blue Ratio");
            }
            else
            {
                if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Blue, dBlueRatio) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to set White Balance Blue Ratio";
                    STTrackLog.WriteLine("Fail to set White Balance Blue Ratio");
                }
            }
        }
        public void SetWhiteBalanceAuto()
        {
            if (device.camControl.SetBalanceWhiteAuto(CameraBalanceWhiteAuto.Once) != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Fail to set White Balance Auto";
                STTrackLog.WriteLine("Fail to set White Balance Auto");
            }
        }
        public bool GetWhiteBalanceAuto()
        {
            CameraBalanceWhiteAuto WhiteBalanceAutoMode;
            device.camControl.GetBalanceWhiteAuto(out WhiteBalanceAutoMode);
            if (WhiteBalanceAutoMode == CameraBalanceWhiteAuto.Once)
            {
                return true;
            }

            return false;
        }
        private void ExposureEnd(CameraEvent sender, EventArgs e)
        {
            //m_intTExposureEnd.Stop();
            //STTrackLog.WriteLine("m_intTExposureEnd = " + m_intTExposureEnd.Duration.ToString());
            //STTrackLog.WriteLine("Exposure End");

            ExposureDone.Set();
        }
        private void FrameTriggerWait(CameraEvent sender, EventArgs e)
        {
            //STTrackLog.WriteLine("m_intTExposureEnd = " + m_intTExposureEnd.Duration.ToString());
            //STTrackLog.WriteLine("FrameTriggerWait");
            TriggerWaitDone.Set();
        }
        private void FrameTrigger(CameraEvent sender, EventArgs e)
        {
            //STTrackLog.WriteLine("m_intTExposureEnd = " + m_intTExposureEnd.Duration.ToString());
            //STTrackLog.WriteLine("FrameTrigger");
            TriggerDone.Set();
        }
        private void FrameTriggerError(CameraEvent sender, EventArgs e)
        {
            //STTrackLog.WriteLine("m_intTExposureEnd = " + m_intTExposureEnd.Duration.ToString());
            m_strErrorText = "Fail Camera - FrameTriggerError";
            STTrackLog.WriteLine("FrameTriggerError");
            TriggerDone.Set();
        }
        public void TriggerImageBufferRead()
        {
            //int i = 0;
            //device.camControl.GetImageBufferFrameCount(out i);

            //STTrackLog.WriteLine("GetImageBufferFrameCount = " + i.ToString());
            device.camControl.ExecuteImageBufferRead();
        }
        private void ImageAcquired(CameraStream sender, ImageAcquiredEventArgs e)
        {
            if (e.ImageInfo.PixelFormat == CameraPixelFormat.Mono8)
            {
                m_ptrImagePointer = e.ImageInfo.BufferPointer;
            }
            else
            {
                //Set the Image Pointer
                if ((m_bmpImage.Width != e.ImageInfo.SizeX) || (m_bmpImage.Height != e.ImageInfo.SizeY))
                {
                    m_bmpImage = new Bitmap(e.ImageInfo.SizeX, e.ImageInfo.SizeY, PixelFormat.Format24bppRgb);
                }

                BitmapData data = m_bmpImage.LockBits(new Rectangle(0, 0, m_bmpImage.Width, m_bmpImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                CameraUtility.ConvertImage(DstPixelFormat.BGR24, e.ImageInfo.PixelFormat, true, data.Scan0, e.ImageInfo.BufferPointer, e.ImageInfo.SizeX, e.ImageInfo.SizeY);
                m_bmpImage.UnlockBits(data);
                m_ptrImagePointer = data.Scan0;
            }


            FrameAcquired.Set();
        }
        public void ConvertImage(IntPtr objBufferPointer)
        {
            //Set the Image Pointer
            if ((m_bmpImage.Width != m_SImageSize.Width) || (m_bmpImage.Height != m_SImageSize.Height))
            {
                m_bmpImage = new Bitmap(m_SImageSize.Width, m_SImageSize.Height, PixelFormat.Format24bppRgb);
            }

            try
            {
                m_objBitmapData = m_bmpImage.LockBits(new Rectangle(0, 0, m_bmpImage.Width, m_bmpImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            }
            catch
            {
                m_bmpImage.UnlockBits(m_objBitmapData);
                m_objBitmapData = m_bmpImage.LockBits(new Rectangle(0, 0, m_bmpImage.Width, m_bmpImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            }
            CameraUtility.ConvertImage(DstPixelFormat.BGR24, CameraPixelFormat.BayerBG8, true, m_objBitmapData.Scan0, objBufferPointer, m_bmpImage.Width, m_bmpImage.Height);
            m_bmpImage.UnlockBits(m_objBitmapData);
            m_ptrImagePointer = m_objBitmapData.Scan0;

        }
        public void ConvertImage(int intImageIndex)
        {
            //Set the Image Pointer
            if ((m_bmpImage.Width != m_SImageSize.Width) || (m_bmpImage.Height != m_SImageSize.Height))
            {
                m_bmpImage = new Bitmap(m_SImageSize.Width, m_SImageSize.Height, PixelFormat.Format24bppRgb);
            }

            try
            {
                m_objBitmapData = m_bmpImage.LockBits(new Rectangle(0, 0, m_bmpImage.Width, m_bmpImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            }
            catch
            {
                m_bmpImage.UnlockBits(m_objBitmapData);
                m_objBitmapData = m_bmpImage.LockBits(new Rectangle(0, 0, m_bmpImage.Width, m_bmpImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            }

            CameraUtility.ConvertImage(DstPixelFormat.BGR24, CameraPixelFormat.BayerBG8, true, m_objBitmapData.Scan0, m_arrBufferPointer[intImageIndex], m_bmpImage.Width, m_bmpImage.Height);

            m_bmpImage.UnlockBits(m_objBitmapData);
            m_ptrImagePointer = m_objBitmapData.Scan0;

        }
        public void ConvertImage(BitmapData objBitMapData, IntPtr objBufferPointer)
        {
            objBitMapData = m_bmpImage.LockBits(new Rectangle(0, 0, m_bmpImage.Width, m_bmpImage.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            CameraUtility.ConvertImage(DstPixelFormat.BGR24, CameraPixelFormat.BayerBG8, true, objBitMapData.Scan0, objBufferPointer, m_bmpImage.Width, m_bmpImage.Height);
            m_bmpImage.UnlockBits(objBitMapData);
            m_ptrImagePointer = objBitMapData.Scan0;

        }
        /// <summary>
        /// switch off camera by stoping all devices
        /// </summary>
        /// <returns>true if success and fail otherwise</returns>
        public bool OFFCamera()
        {
            //if (m_objCamera.StopDevice() != enFireWrapResult.E_NOERROR)    // stop image acquisition of the camera
            //    return false;
            //if (m_objCamera.CloseCapture() != enFireWrapResult.E_NOERROR) // close receive logic in FireGrab framework. Release all buffer and bus allocation resource
            //    return false;
            //if (m_objCamera.Disconnect() != enFireWrapResult.E_NOERROR)    // disconnect object from real camera
            //    return false;

            if (device.camStream.Stop() != CamApiStatus.Success)
                return false;
            if (device.camStream.Close() != CamApiStatus.Success)
                return false;
            if (device.Close() != CamApiStatus.Success)
                return false;

            return true;
        }
        public bool OFFCamera_LowLevelAPI()
        {
            //if (m_objCamera.StopDevice() != enFireWrapResult.E_NOERROR)    // stop image acquisition of the camera
            //    return false;
            //if (m_objCamera.CloseCapture() != enFireWrapResult.E_NOERROR) // close receive logic in FireGrab framework. Release all buffer and bus allocation resource
            //    return false;
            //if (m_objCamera.Disconnect() != enFireWrapResult.E_NOERROR)    // disconnect object from real camera
            //    return false;
            ulong requestHandle = 0;
            IntPtr imageBuffer = IntPtr.Zero;
            int payloadSize;
            CamApiStatus status;

            if (device.camStream.Stop() != CamApiStatus.Success)
                return false;

            if (device.camStream.lowLevelApi.FlushWaitQueue() != CamApiStatus.Success)
                return false;

            for (int i = 0; i < BufferCount; i++)
            {
                status = device.camStream.lowLevelApi.DequeueRequest(ref requestHandle, ref imageBuffer, out payloadSize);
                if (status == CamApiStatus.EmptyCompleteQueue)
                {
                    break;
                }
            }

            for (int i = 0; i < BufferCount; i++)
            {
                if (_requestHandle[i] != 0)
                {
                    status = device.camStream.lowLevelApi.ReleaseRequest(_requestHandle[i]);
                    if (status != CamApiStatus.Success)
                    {
                        m_strErrorText = "Fail Camera - Error Status from ReleaseRequest : " + status.ToString();
                        STTrackLog.WriteLine("Error Status from ReleaseRequest : " + status.ToString());
                    }
                    _requestHandle[i] = 0;
                }

                if (_imageBuffer[i] != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_imageBuffer[i]);
                    _imageBuffer[i] = IntPtr.Zero;
                }
            }

            if (device.camStream.Close() != CamApiStatus.Success)
                return false;
            if (device.Close() != CamApiStatus.Success)
                return false;

            return true;
        }
        /// <summary>
        /// Set output bit On/Off
        /// </summary>
        /// <param name="intID">0=output1, 1=output2, 2=output3</param>
        /// <param name="intMode">1= integration enable -> yes, 2 = integration enable -> no, 0= off mode</param>
        /// <returns>false = action fail</returns>
        public bool OutPort(int intID, int intMode)
        {
            if (!m_blnCameraInitDone)
                return false;

            CameraLineSource LineSrc = new CameraLineSource();
            uint uintLineInverter = 0;
            switch (intMode)
            {
                case 1:
                    LineSrc = CameraLineSource.ExposureActive;
                    device.camControl.GetLineInverterAll(out uintLineInverter);
                    if (uintLineInverter != 0)
                        device.camControl.SetLineInverterAll(0);
                    break;
                case 2:
                    LineSrc = CameraLineSource.ExposureActive;
                    device.camControl.GetLineInverterAll(out uintLineInverter);
                    if (uintLineInverter != 7)
                        device.camControl.SetLineInverterAll(7);
                    break;
                case 3:
                    LineSrc = CameraLineSource.UserOutput;
                    //Set User output to 1
                    device.camControl.SetUserOutputValueAll(1);
                    break;
                default:
                    LineSrc = CameraLineSource.Off;
                    break;
            }
            switch (intID)
            {
                case 0:
                    if (m_Line1SrcPrev == LineSrc)
                        return true;
                    else
                    {
                        if (device.camControl.SetLineSource(CameraLineSelector.Line1, LineSrc) == CamApiStatus.Success)
                        {
                            m_Line1SrcPrev = LineSrc;
                            return true;
                        }
                    }
                    break;
                case 1:
                    if (m_Line2SrcPrev == LineSrc)
                        return true;
                    else
                    {
                        if (device.camControl.SetLineSource(CameraLineSelector.Line2, LineSrc) == CamApiStatus.Success)
                        {
                            m_Line2SrcPrev = LineSrc;
                            return true;
                        }
                    }
                    break;
            }

            return false;
        }
        /// <summary>
        /// Clear object and reconnect camera
        /// </summary>
        /// <returns></returns>
        //public bool ReconnectCamera()
        //{
        //    //m_objCamera.StopDevice();
        //    //m_objCamera.CloseCapture();
        //    //m_objCamera.Disconnect();
        //    Teli.TeliCamAPI.NET.CameraInfo info = null;
        //    String SN;
        //    device.GetInformation(ref info);
        //    SN = info.SerialNumber;
        //    device.camStream.Stop();
        //    device.camStream.Close();
        //    device.Close();

        //    m_strErrorText = "Try to reconnect camera ...";

        //    InitializeCamera(SN);

        //    return true;
        //}
        /// <summary>
        /// Returns a frame to the ¡®DMA queue¡¯ and prepares it to receive new image data.
        /// After this call, frame will not be valid anymore
        /// </summary>
        /// <returns>return true if success or fail otherwise</returns>
        public bool ReleaseImage()
        {
            //if (m_objCamera.PutFrame(m_objGrabbedFrame) != enFireWrapResult.E_NOERROR)
            //{
            //    m_strErrorText = "Fail Camera - Release Image Frame Error";
            //    m_blnIsGrabbing = false;
            //    return false;
            //}
            //m_blnIsGrabbing = false;
            return true;
        }
        /// <summary>
        /// Returns a frame to the ¡®DMA queue¡¯ and prepares it to receive new image data.
        /// After this call, frame will not be valid anymore
        /// </summary>
        /// <param name="intFrameIndex">Frame index</param>
        /// <returns>return true if success or fail otherwise</returns>
        public bool ReleaseImage(int intFrameIndex)
        {
            //if (m_objCamera.PutFrame(m_arrGrabbedFrame[intFrameIndex]) != enFireWrapResult.E_NOERROR)
            //{
            //    m_strErrorText = "Fail Camera - Release Image Frame Error";
            //    m_blnIsGrabbing = false;
            //    return false;
            //}
            //m_blnIsGrabbing = false;
            return true;
        }
        /// <summary>
        /// Set camera settings such as Gain/Offset, shutter and brightness
        /// </summary>
        /// <param name="paramType">1 - Shutter, 2 - Gain, 3 - brightness, 4- Gamma, 5-UB, 6-VR</param>
        /// <param name="value">settings value</param>
        /// <returns>true if success and fail otherwise</returns>
        public bool SetCameraParameter(int intParamType, uint intValue)
        {
            double dValue;
            switch (intParamType)
            {
                case 1:
                    //intValue should be in percenstage
                    dValue = intValue * 100;
                    if (dValue < 30) dValue = 30;               // Minimum exposure value for teli camera is 30
                    if (dValue > 16000000) dValue = 16000000;   // Maximum exposure value for teli camera is 16000k
                    if (device.camControl.SetExposureTime(dValue) == CamApiStatus.Success)
                        return true;
                    break;
                case 2:
                    // intValue should be in percenstage
                    dValue = 1 + (double)intValue / 14.3;
                    if (dValue < 1) dValue = 1; // Minimum gain value for teli camera is 1
                    if (dValue > 8) dValue = 8; // Maximum gain value for teli camera is 8
                    //if (m_objCamera.SetParameter(enFGParameter.E_GAIN, intValue) == enFireWrapResult.E_NOERROR)
                    if (device.camControl.SetGain(dValue) == CamApiStatus.Success)
                        return true;
                    break;
                case 3:
                    //if (m_objCamera.SetParameter(enFGParameter.E_BRIGHTNESS, intValue) == enFireWrapResult.E_NOERROR)
                    if (device.camControl.SetBlackLevel((double)intValue) == CamApiStatus.Success)
                        return true;
                    break;
                case 4:
                    //if (m_objCamera.SetParameter(enFGParameter.E_GAMMA, intValue) == enFireWrapResult.E_NOERROR)
                    if (device.camControl.SetGamma((double)intValue) == CamApiStatus.Success)
                        return true;
                    break;
                case 5:
                    //if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCB, intValue) == enFireWrapResult.E_NOERROR)
                    double intValueB = (double)intValue;
                    if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Blue, intValueB) == CamApiStatus.Success)
                        return true;
                    break;
                case 6:
                    //if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCR, intValue) == enFireWrapResult.E_NOERROR)
                    double intValueR = (double)intValue;
                    if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Red, intValueR) == CamApiStatus.Success)
                        return true;
                    break;
            }
            return false;
        }

        /// <summary>
        /// Set camera settings such as Gain/Offset, shutter and brightness
        /// </summary>
        /// <param name="paramType">1 - Shutter, 2 - Gain, 3 - brightness, 4- Gamma, 5-UB, 6-VR</param>
        /// <param name="value">settings value</param>
        /// <returns>true if success and fail otherwise</returns>
        public bool SetCameraParameter(int intParamType, float fValue)
        {
            if (!m_blnCameraInitDone)
                return false;
            //STTrackLog.WriteLine("TeliCameraAPI->SetCameraParameter->Enter");
            //TrackLog objTL = new TrackLog();

            double dValue;
            switch (intParamType)
            {
                case 1:

                    //STTrackLog.WriteLine("TeliCameraAPI->SetCameraParameter->Case 1 A");
                    //intValue should be in percenstage

                    //objTL.WriteLine("ST fValue before devide= " + fValue.ToString());

                    dValue = fValue;
                    if (dValue < 30) dValue = 30;               // Minimum exposure value for teli camera is 30um
                    if (dValue > 16000000) dValue = 16000000;   // Maximum exposure value for teli camera is 16000k um

                    if (dValue == m_dExposurePrev)
                    {
                        //objTL.WriteLine("ST dValue Same setting = " + dValue.ToString() + ", Prev=" + m_dExposurePrev.ToString());
                        //STTrackLog.WriteLine("TeliCameraAPI->SetCameraParameter->Case 1 B");
                        return true;
                    }
                    else
                    {
                        //objTL.WriteLine("ST dValue final Set = " + dValue.ToString());

                        if (device.camControl.SetExposureTime(dValue) == CamApiStatus.Success) // dValue is in um
                        {
                            //STTrackLog.WriteLine("SetExposureTime = " + dValue.ToString());
                            //STTrackLog.WriteLine("TeliCameraAPI->SetCameraParameter->Case 1 C");
                            m_dExposurePrev = dValue;
                            return true;
                        }
                        else
                        {
                            m_strErrorText = "Fail Camera - Fail to SetExposureTime";
                            STTrackLog.WriteLine("Fail to SetExposureTime");
                        }
                    }
                    break;
                case 2:
                    /*
                     * fValue is sent by parameter is in range 0 to 50
                     * Teli camera BU031 has gain range 0 to 18
                     * Teli camera BU205 has gain range 1 to 8
                     * Decision was made both camera will set gain value in range 1 to 8
                     * 50 / 7 == 7.14 == 7.2. Meaning the fValue will be devided by 7.2 to ensure gain value is not over than 8.
                     */

                    //objTL.WriteLine("fValue before devide= " + fValue.ToString());

                    //STTrackLog.WriteLine("TeliCameraAPI->SetCameraParameter->Case 2");
                    //dValue = 1 + fValue / 3.3;
                    //if (dValue < 1) dValue = 1; // Minimum gain value for teli camera is 1
                    //if (dValue > 8) dValue = 8; // Maximum gain value for teli camera is 8

                    dValue = Math.Round(1 + fValue / m_fGainDivider, 3, MidpointRounding.AwayFromZero);

                    if (dValue > Math.Round(1 + fValue / m_fGainDivider))
                        dValue = Math.Round(1 + fValue / m_fGainDivider);

                    if (dValue == m_dGainPrev)
                    {
                        //objTL.WriteLine("dValue Same setting = " + dValue.ToString() + ", Prev=" + m_dGainPrev.ToString());
                        return true;
                    }
                    else
                    {
                        //objTL.WriteLine("dValue final Set = " + dValue.ToString());

                        if (device.camControl.SetGain(dValue) == CamApiStatus.Success)
                        {
                            //STTrackLog.WriteLine("SetGain = " + dValue.ToString());
                            m_dGainPrev = dValue;
                            return true;
                        }
                        else
                        {
                            m_strErrorText = "Fail Camera - Fail to SetGain";
                            STTrackLog.WriteLine("Fail to SetGain");
                        }
                    }
                    break;
                case 3:
                    //if (m_objCamera.SetParameter(enFGParameter.E_BRIGHTNESS, intValue) == enFireWrapResult.E_NOERROR)
                    if (device.camControl.SetBlackLevel(fValue) == CamApiStatus.Success)
                        return true;
                    break;
                case 4:
                    //if (m_objCamera.SetParameter(enFGParameter.E_GAMMA, intValue) == enFireWrapResult.E_NOERROR)
                    if (device.camControl.SetGamma(fValue) == CamApiStatus.Success)
                        return true;
                    break;
                case 5:
                    //if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCB, intValue) == enFireWrapResult.E_NOERROR)
                    double intValueB = fValue;
                    if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Blue, intValueB) == CamApiStatus.Success)
                        return true;
                    break;
                case 6:
                    //if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCR, intValue) == enFireWrapResult.E_NOERROR)
                    double intValueR = fValue;
                    if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Red, intValueR) == CamApiStatus.Success)
                        return true;
                    break;
            }
            return false;
        }

        /// <summary>
        /// Calibrate camera to get white balance value
        /// </summary>
        /// <param name="intType">1= auto, 2 = one shot, 3= off all action</param>
        /// <returns></returns>
        public bool AutoCalibrateWhiteBalance(int intType)
        {
            switch (intType)
            {
                case 1:
                    //if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCR, enFGParameterState.E_AUTO) == enFireWrapResult.E_NOERROR)
                    //{
                    //    if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCB, enFGParameterState.E_AUTO) == enFireWrapResult.E_NOERROR)
                    //        return true;
                    //}                    
                    break;
                case 2:
                    // Wait last grabbing done
                    int intCount = 0;
                    while (m_blnIsGrabbing && (intCount < 3000))
                    {
                        Thread.Sleep(1);
                        intCount++;
                    }

                    m_blnPauseGrab = true; // Trigger event to pause grab image function 

                    if (intCount >= 3000)
                    {
                        m_strErrorText = "Fail Camera - ONESHOT cannot be triggered because grabbing never end.";
                        m_blnPauseGrab = false;
                        return false;
                    }


                    // Trigger ONESHOT grab image
                    //if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCR, enFGParameterState.E_ONESHOT) == enFireWrapResult.E_NOERROR)
                    //{
                    //    if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCB, enFGParameterState.E_ONESHOT) == enFireWrapResult.E_NOERROR)
                    //    {
                    //        Thread.Sleep(700); // Delay to make sure all ONESHOT image is fully transfered and processed before allow for next grab.
                    //        m_blnPauseGrab = false; // Reset event to allow grab image function.
                    //        return true;
                    //    }
                    //}

                    if (device.camControl.SetBalanceWhiteAuto(CameraBalanceWhiteAuto.Once) == CamApiStatus.Success)
                    {
                        Thread.Sleep(700);
                        m_blnPauseGrab = false;
                        return true;
                    }

                    m_blnPauseGrab = false;
                    break;
                case 3: // Off auto
                    //if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCR, enFGParameterState.E_OFF) == enFireWrapResult.E_NOERROR)
                    //{
                    //    if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCB, enFGParameterState.E_OFF) == enFireWrapResult.E_NOERROR)
                    //        return true;
                    //}
                    if (device.camControl.SetBalanceWhiteAuto(CameraBalanceWhiteAuto.Off) == CamApiStatus.Success)
                        return true;
                    break;
            }

            return false;
        }

        /// <summary>
        /// Get selected port camera's ID 
        /// </summary>
        /// <param name="intPortNo">Port number</param>
        /// <returns></returns>
        public uint GetCameraID(int intPortNo)
        {
            //if (m_objCameraContainer.FGGetNodeList() != enFireWrapResult.E_NOERROR || m_objCameraContainer.Size() == 0)
            //    return 0;

            //FGNodeInfo objNodeinfo = new FGNodeInfo();

            //for (uint i = 0; i < m_objCameraContainer.Size(); i++)
            //{
            //    if (m_objCameraContainer.GetAt(objNodeinfo, i) == enFireWrapResult.E_NOERROR)
            //    {
            //        if ((int)objNodeinfo.CardNumber == intPortNo)
            //        {
            //            return objNodeinfo.Guid.Low;
            //        }
            //    }
            //}

            return 0;
        }
        /// <summary>
        /// Get camera shuttle, gain or brightness value
        /// </summary>
        /// <param name="intParamType">1=Shuttle, 2=Gain, 3=Brightness, 4= Gamma, 5= white balance UB, 6= white balance VR</param>
        /// <returns>Selected parameter value</returns>
        public uint GetCameraParameter(int intParamType)
        {
            double intValue = 0;
            switch (intParamType)
            {
                case 1:
                    //m_objCamera.GetParameter(enFGParameter.E_SHUTTER, ref intValue);
                    device.camControl.GetExposureTime(out intValue);
                    break;
                case 2:
                    //m_objCamera.GetParameter(enFGParameter.E_GAIN, ref intValue);
                    device.camControl.GetGain(out intValue);
                    break;
                case 3:
                    //m_objCamera.GetParameter(enFGParameter.E_BRIGHTNESS, ref intValue);
                    device.camControl.GetBlackLevel(out intValue);
                    break;
                case 4:
                    //m_objCamera.GetParameter(enFGParameter.E_GAMMA, ref intValue);
                    device.camControl.GetGamma(out intValue);
                    break;
                case 5:
                    //m_objCamera.GetParameter(enFGParameter.E_WHITEBALCB, ref intValue);
                    device.camControl.GetBalanceRatio(CameraBalanceRatioSelector.Blue, out intValue);
                    break;
                case 6:
                    //m_objCamera.GetParameter(enFGParameter.E_WHITEBALCR, ref intValue);
                    device.camControl.GetBalanceRatio(CameraBalanceRatioSelector.Red, out intValue);
                    break;
            }

            return Convert.ToUInt32(intValue);
        }

        // Use for camera to blink green light
        public bool OpenCameraOnly(String SerialNo)
        {
            bool bFound = false;
            Teli.TeliCamAPI.NET.CameraInfo info = null;

            if (system.GetNumOfCameras(out number) != CamApiStatus.Success || number == 0)
            {
                return false;
            }

            if (SerialNo.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < number; i++)
            {
                if (system.GetCameraInformation(i, ref info) == CamApiStatus.Success)
                {
                    if (SerialNo.Length > 0)
                    {
                        if (info.SerialNumber == SerialNo)
                        {
                            bFound = true;
                        }
                        else
                            continue;
                    }
                    if (bFound)
                    {
                        system.CreateDeviceObject(i, ref device);
                        break;
                    }
                }
            }
            if (bFound)
            {
                //device.ResetPort();
                //if (device.Open() != CamApiStatus.Success)
                //{
                //    device.ResetPort();
                //    return false;
                //}
            }
            else
            {
                return false;
            }

            return true;
        }

        // Use for camera to blink orange light
        public bool CloseCameraOnly(String SerialNo)
        {
            bool bFound = false;
            int payload = 0;
            Teli.TeliCamAPI.NET.CameraInfo info = null;

            if (system.GetNumOfCameras(out number) != CamApiStatus.Success || number == 0)
            {
                return false;
            }

            if (SerialNo.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < number; i++)
            {
                if (system.GetCameraInformation(i, ref info) == CamApiStatus.Success)
                {
                    if (SerialNo.Length > 0)
                    {
                        if (info.SerialNumber == SerialNo)
                        {
                            bFound = true;
                        }
                        else
                            continue;
                    }

                    if (bFound)
                    {
                        system.CreateDeviceObject(i, ref device);
                        break;
                    }
                }
            }
            if (bFound)
            {
                device.ResetPort();
                if (device.Open() != CamApiStatus.Success)
                {
                    device.ResetPort();
                    return false;
                }
            }
            else
            {
                return false;
            }

            if (device.CamType == CameraType.TypeU3v)
            {
                //Open Stream USB3
                if (device.camStream.Open(out payload) != CamApiStatus.Success)
                {
                    //STTrackLog.WriteLine("Init -> 11");
                    m_strErrorText = "Fail Camera - Fail to open stream";
                    STTrackLog.WriteLine("Fail to open stream");
                    return false;
                }
            }
            else
            {
                //Open Stream GigE set 9000 packet size
                if (device.camStream.Open(null, 0, 9000, out payload) != CamApiStatus.Success)
                {
                    //STTrackLog.WriteLine("Init -> 11");
                    m_strErrorText = "Fail Camera - Fail to open stream";
                    STTrackLog.WriteLine("Fail to open stream");
                    return false;
                }
            }

            //Frame     Acquired Event
            device.camStream.ImageAcquired += new ImageAcquiredEventHandler(ImageAcquired);

            //GigE camera dont have Exposure End event
            if (device.CamType == CameraType.TypeU3v)
            {
                //Open Event Interface
                if (device.camEvent.Open() != CamApiStatus.Success)
                {
                    return false;
                }

                //Activate Exposure End Event
                if (device.camEvent.Activate(CameraEventType.ExposureEnd) != CamApiStatus.Success)
                {
                    return false;
                }

                device.camEvent.ExposureEndReceived += new CameraEventReceivedEventHandler(ExposureEnd);
            }

            //Start Stream
            if (device.camStream.Start() != CamApiStatus.Success)
            {
                return false;
            }

            return true;
        }

        public bool InitializeCamera(String SerialNo)
        {
            bool bFound = false;
            int payload = 0;
            Teli.TeliCamAPI.NET.CameraInfo info = null;

            if (system.GetNumOfCameras(out number) != CamApiStatus.Success || number == 0)
            {
                m_strErrorText = "Fail Camera - Unable to find any camera(s) connected";
                return false;
            }

            if (SerialNo.Length == 0)
            {
                m_strErrorText = "Fail Camera - SerialNo or UserDefineName should not be empty.";
                return false;
            }


            for (int i = 0; i < number; i++)
            {
                if (system.GetCameraInformation(i, ref info) == CamApiStatus.Success)
                {
                    if (SerialNo.Length > 0)
                    {
                        if (info.SerialNumber == SerialNo || SerialNo == "1")
                        {
                            bFound = true;
                        }
                        else
                            continue;
                    }

                    if (bFound)
                    {
                        //STTrackLog.WriteLine("Get device");
                        system.CreateDeviceObject(i, ref device);
                        break;
                    }

                }
            }
            if (bFound)
            {
                device.ResetPort();
                if (device.Open() != CamApiStatus.Success)
                {
                    device.ResetPort();
                    m_strErrorText = "Fail Camera - Fail to connect with real camera";
                    return false;
                }
            }
            else
            {
                m_strErrorText = "Fail Camera - No camera serial number match to it.";
                return false;
            }

            if (device.camControl.SetGamma(1) != CamApiStatus.Success) // dValue is in um
            {
                m_strErrorText = "Fail Camera - Fail to set gammma";
                return false;
            }

            //Set trigger mode on and trigger source to software trigger
            if (device.camControl.SetTriggerMode(true) != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Fail to set trigger mode";
                return false;
            }
            if (device.camControl.SetTriggerSource(CameraTriggerSource.Software) != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Fail to set trigger source";
                return false;
            }

            CameraPixelFormat pixelFormat = CameraPixelFormat.Mono8;
            if (device.camControl.GetPixelFormat(out pixelFormat) != CamApiStatus.Success)
            {
                pixelFormat = CameraPixelFormat.Mono8;
            }

            if (pixelFormat != CameraPixelFormat.Mono8)
            {
                //White Balance Red Raw = 130515, Blue Raw = 98226
                double dRedRatio = 1.991500854;
                double dBlueRatio = 1.498809814;
                if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Red, dRedRatio) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to set White Balance Red Ratio";
                }

                if (device.camControl.SetBalanceRatio(CameraBalanceRatioSelector.Blue, dBlueRatio) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to set White Balance Blue Ratio";
                }
            }

            //STTrackLog.WriteLine("Init -> 1");

            if (device.CamType == CameraType.TypeU3v)
            {
                //Open Stream USB3
                if (device.camStream.Open(out payload) != CamApiStatus.Success)
                {
                    //STTrackLog.WriteLine("Init -> 11");
                    m_strErrorText = "Fail Camera - Fail to open stream";
                    return false;
                }
            }
            else
            {
                //Open Stream GigE set 9000 packet size
                if (device.camStream.Open(null, 0, 9000, out payload) != CamApiStatus.Success)
                {
                    //STTrackLog.WriteLine("Init -> 11");
                    m_strErrorText = "Fail Camera - Fail to open stream";
                    return false;
                }
            }

            //STTrackLog.WriteLine("Init -> 2");

            //Frame     Acquired Event
            device.camStream.ImageAcquired += new ImageAcquiredEventHandler(ImageAcquired);

            //STTrackLog.WriteLine("Init -> 3");

            //GigE camera dont have Exposure End event
            if (device.CamType == CameraType.TypeU3v)
            {
                //Open Event Interface
                if (device.camEvent.Open() != CamApiStatus.Success)
                {
                    //STTrackLog.WriteLine("Init -> 33");
                    m_strErrorText = "Fail Camera - Fail to open event";
                    return false;
                }

                //STTrackLog.WriteLine("Init -> 4");
                //Activate Exposure End Event
                if (device.camEvent.Activate(CameraEventType.ExposureEnd) != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Fail to activate event";
                    return false;
                }

                device.camEvent.ExposureEndReceived += new CameraEventReceivedEventHandler(ExposureEnd);
            }

            //Start Stream
            if (device.camStream.Start() != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Fail to start stream";
                return false;
            }

            m_blnCameraInitDone = true;

            //STTrackLog.WriteLine("Connect Camera Pass. Serial No " + SerialNo);

            return true;
        }

        public bool ConvertBayerBG8ToBayerBGR()
        {
            //CameraUtility.ConvertBGR8PToBGR()

            return true;
        }
        private void InitializeBuffer()
        {

            _requestHandle = new ulong[BufferCount];
            curRequestHandle = new ulong[BufferCount];
            _imageBuffer = new IntPtr[BufferCount];

            for (int i = 0; i < BufferCount; i++)
            {
                curRequestHandle[i] = 0;
                _requestHandle[i] = 0;
                _imageBuffer[i] = IntPtr.Zero;
            }

        }

        private bool OpenCamStream()
        {
            int iSize = 0;
            CamApiStatus _sts = device.camStream.lowLevelApi.Open(_completeEvent, ref iSize);
            if (_sts != CamApiStatus.Success)
            {
                m_strErrorText = "Fail Camera - Cannot Open Stream : " + _sts.ToString();
                STTrackLog.WriteLine("Cannot Open Stream : " + _sts.ToString());
                return false;
            }

            for (int i = 0; i < BufferCount; i++)
            {
                _imageBuffer[i] = Marshal.AllocHGlobal(iSize);
                if (_imageBuffer[i] == IntPtr.Zero)
                {
                    m_strErrorText = "Fail Camera - Cannot allocate memory.";
                    STTrackLog.WriteLine("Cannot allocate memory.");
                    return false;
                }

                _sts = device.camStream.lowLevelApi.CreateRequest(_imageBuffer[i], iSize, ref _requestHandle[i]);
                if (_sts != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Cannot create request.";
                    STTrackLog.WriteLine("Cannot create request");
                    return false;
                }

                _sts = device.camStream.lowLevelApi.EnqueueRequest(_requestHandle[i]);
                if (_sts != CamApiStatus.Success)
                {
                    m_strErrorText = "Fail Camera - Cannot enqueue request.";
                    STTrackLog.WriteLine("Cannot enqueue request");
                    return false;
                }
            }

            return true;
        }

        public bool Grab(ref IntPtr ptr, int intImageIndex)
        {
            CamApiStatus status;
            bool Flag;

            if (curRequestHandle[intImageIndex] != 0)
            {
                status = device.camStream.lowLevelApi.EnqueueRequest(curRequestHandle[intImageIndex]);
                if (status != CamApiStatus.Success)
                {
                    Console.WriteLine("Error Status from EnqueueRequest : " + status.ToString());
                }
            }

            status = device.camControl.ExecuteSoftwareTrigger();

            if (status != CamApiStatus.Success)
            {
                Console.WriteLine("Error Status from Software Trigger : " + status.ToString());
                return false;
            }

            Flag = _completeEvent.WaitOne(200);

            //Console.WriteLine("AutoResetEvent Status : " + Flag);

            if (!Flag)
                return false;

            curRequestHandle[intImageIndex] = 0;
            IntPtr imageBuffer = IntPtr.Zero;
            int payloadSize;

            status = device.camStream.lowLevelApi.DequeueRequest(ref curRequestHandle[intImageIndex], ref imageBuffer, out payloadSize);
            if ((status == CamApiStatus.EmptyCompleteQueue) || (curRequestHandle[intImageIndex] == 0))
            {
                Console.WriteLine("Error Status from DequeueRequest : " + status.ToString());
                return false;
            }

            ptr = imageBuffer;

            ////Funtion below only called to get image info and BlockID
            //if (!CheckStreamRequest(curRequestHandle, out curBlockID))
            //{
            //    Console.WriteLine("Error Status from CheckStreamRequest");
            //    return false;
            //}


            //if (ptr == IntPtr.Zero)
            //{
            //    Console.WriteLine("Camera Image Pointer is  : " + ptr.ToString());
            //    return false;
            //}

            return true;
        }

        private bool CheckStreamRequest(ulong requestHandle, out ulong blockID)
        {
            CamApiStatus status;
            StreamRequestInfo requestInfo;

            blockID = 0;

            status = device.camStream.lowLevelApi.GetRequestInformation(requestHandle, out requestInfo);
            if (status == CamApiStatus.EmptyCompleteQueue)
            {
                Console.WriteLine("Error Status from DequeueRequest : " + status.ToString());
                return false;
            }

            if (requestInfo.u3vInfo.Leader == IntPtr.Zero)
            {
                Console.WriteLine("Error Status from Stream Leader");
                return false;
            }

            U3vStreamImageLeader imageLeader = new U3vStreamImageLeader();
            imageLeader = (U3vStreamImageLeader)Marshal.PtrToStructure(requestInfo.u3vInfo.Leader, imageLeader.GetType());

            if ((imageLeader.payloadType != (ushort)U3vStreamPayloadType.Image) ||
                (requestInfo.u3vInfo.Trailer == IntPtr.Zero))
            {
                Console.WriteLine("Error Status from Stream Trailer");
                return false;
            }

            U3vStreamImageTrailer imageTrailer = new U3vStreamImageTrailer();
            imageTrailer = (U3vStreamImageTrailer)Marshal.PtrToStructure(requestInfo.u3vInfo.Trailer, imageTrailer.GetType());

            if (imageTrailer.status != 0)
            {
                Console.WriteLine("Error Status from Image Trailer status: " + imageTrailer.status.ToString());
                return false;
            }

            blockID = imageTrailer.blockID;

            STTrackLog.WriteLine("imageTrailer.blockID : " + blockID.ToString());
            STTrackLog.WriteLine("imageTrailer.status : " + imageTrailer.status.ToString());

            return true;
        }
    }
}
