using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using VisionProcessing;
using Common;

namespace ImageAcquisition
{
    public class IDSuEyeCamera
    {
        #region Member Variables

        private bool m_blnInitCameraDone = false;
        private bool m_blnGetFrameDone = false;
        private bool m_blnExposureDone = false;
        private string m_strErrorText = "";
        private IntPtr m_ptrImagePointer;
        private uEye.Camera m_Camera;
        private uEye.Defines.Status m_statusRet = 0;
        private Bitmap m_bmpImage = new Bitmap(1280, 1024, PixelFormat.Format8bppIndexed);
        private List<Bitmap> m_arrbmpImage = new List<Bitmap>();
        private HiPerfTimer m_objExposureTime = new HiPerfTimer();
        private HiPerfTimer m_objTransferTime = new HiPerfTimer();
        private object m_objLock = new object();


        #endregion

        #region Propreties

        public string ref_strErrorText { get { return m_strErrorText; } }
        public float ref_fExposureTime { get { return m_objExposureTime.Duration; } }
        public float ref_fTransferTime { get { return m_objTransferTime.Duration; } }

        #endregion

        /// <summary>
        /// IDS Camera Driver
        /// </summary>
        public IDSuEyeCamera()
        {

        }

        public bool Grab()
        {
            try
            {
                if (m_strErrorText.Length > 0)
                    m_strErrorText = string.Empty;

                // Start calculate exposure time
                m_objExposureTime.Start();
                m_objTransferTime.Start();

                // start capture
                m_blnExposureDone = false;
                m_blnGetFrameDone = false;

                lock (m_objLock)
                {
                    m_statusRet = m_Camera.Acquisition.Freeze();
                }

                
            }
            catch (Exception ex)
            {
                m_strErrorText = "IDSuEyeCamera Grab() error : " + ex.ToString();
                return false;
            }

            return true;
        }

        public bool WaitExposureDone()
        {
            int intCount = 0;
            while (!m_blnExposureDone)
            {               
                if (intCount++ > 1000)
                {
                    m_strErrorText = "IDSCamera Wait Exposure Done Timeout!";
                    return false;
                }
                Thread.Sleep(1);
            }

            return true;
        }

        public bool GetFrame(ImageDrawing objEuresysImageDrawing)
        {
            int intCount = 0;
            while (!m_blnGetFrameDone)
            {
                if (intCount++ > 1000)
                {
                    m_strErrorText = "IDSCamera Get Frame Timeout!";
                    return false;
                }
                Thread.Sleep(1);
            }

            try
            {
                Rectangle objDimension = new Rectangle(0, 0, m_bmpImage.Width, m_bmpImage.Height);
                BitmapData bmpDataImage = m_bmpImage.LockBits(objDimension, ImageLockMode.ReadWrite, m_bmpImage.PixelFormat);
                m_ptrImagePointer = bmpDataImage.Scan0;   // Scan0 = The address in memory of the fixed data array 

                objEuresysImageDrawing.LoadImageFromMemory(m_ptrImagePointer, m_bmpImage);

                m_bmpImage.UnlockBits(bmpDataImage);
            }
            catch (Exception ex)
            {
                m_strErrorText = "IDSCamera Get Frame Fail!";
                return false;
            }


            return true;
        }

        public static int CheckCamera()
        {
            return 1;
        }

        private bool SetCamera_General()
        {
            // Get pixel clock range
            uEye.Types.Range<int> pixelClockRange = new uEye.Types.Range<int>();
            m_Camera.Timing.PixelClock.GetRange(out pixelClockRange);

            // Set pixel clock
            m_statusRet = m_Camera.Timing.PixelClock.Set(pixelClockRange.Maximum);    //UI-5250RE Camera is 34// Set pixel clock to maximum to get highest framerate range
            if (m_statusRet != uEye.Defines.Status.SUCCESS)
            {
                m_strErrorText = "Set pixel clock failed";
                return false;
            }

            // get frame rate range
            uEye.Types.Range<double> frameRateRange = new uEye.Types.Range<double>();
            m_Camera.Timing.Framerate.GetFrameRateRange(out frameRateRange);

            // set framerate
            m_statusRet = m_Camera.Timing.Framerate.Set(frameRateRange.Maximum);    // UI-5250RE
            if (m_statusRet != uEye.Defines.Status.SUCCESS)
            {
                m_strErrorText = "Set framerate failed";
                return false;
            }

            // Set exposure time
            m_statusRet = m_Camera.Timing.Exposure.Set(1);
            if (m_statusRet != uEye.Defines.Status.SUCCESS)
            {
                m_strErrorText = "Set Exposure failed";
                return false;
            }

            // set camera pixel format to MONO8
            m_statusRet = m_Camera.PixelFormat.Set(uEye.Defines.ColorMode.Mono8);
            if (m_statusRet != uEye.Defines.Status.SUCCESS)
            {
                m_strErrorText = "Set camera pixel format failed";
                return false;
            }

            return true;
        }

        public bool InitializeCamera_AutoSetFrameRate(int intCameraPort)
        {
            if (m_blnInitCameraDone)
                return true;

            lock (m_objLock)
            {

                if (m_strErrorText.Length > 0)
                    m_strErrorText = string.Empty;

                uEye.Types.CameraInformation[] cameraList;
                uEye.Info.Camera.GetCameraList(out cameraList);

                bool bFound = false;
                int intCameraDeviceID = 0;
                foreach (uEye.Types.CameraInformation info in cameraList)
                {
                    if (info.DeviceID == intCameraPort)
                    {
                        intCameraDeviceID = Convert.ToInt32(info.DeviceID);
                        bFound = true;
                        break;
                    }
                }

                m_Camera = new uEye.Camera();

                if (!bFound)
                {
                    m_strErrorText = "Fail Init IDS Camera - No camera is connected to selected port.";
                    return false;
                }

                m_statusRet = m_Camera.Init(intCameraDeviceID | (Int32)uEye.Defines.DeviceEnumeration.UseDeviceID);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Initializing the camera failed";
                    return false;
                }

                m_statusRet = m_Camera.Memory.Allocate();
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Allocating memory failed";
                    return false;
                }

                // --- Disable all auto feature first  ---------

                // Disable Auto Exposure Feature
                m_statusRet = m_Camera.AutoFeatures.Software.Shutter.SetEnable(false);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Disable Auto Exposure failed";
                    return false;
                }

                // Disable Auto Gain Feature
                m_statusRet = m_Camera.AutoFeatures.Software.Gain.SetEnable(false);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Disable Auto Gain failed";
                    return false;
                }

                // Disable Auto Framerate Feature
                m_statusRet = m_Camera.AutoFeatures.Software.Framerate.SetEnable(false);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Disable Auto Framerate failed";
                    return false;
                }

                // -----------------------------------------------

                SetCamera_General();
                
                //// Set camera Color Convert/hardware
                //m_statusRet = m_Camera.Color.Converter.Set(uEye.Defines.ColorMode.Mono8, uEye.Defines.ColorConvertMode.Hardware3X3);
                //if (m_statusRet != uEye.Defines.Status.SUCCESS)
                //{
                //    m_strErrorText = "Set camera color convert/hardware failed";
                //    return false;
                //}

                // Set camera trigger mode
                SetCameraTriggerMode(0);

                // Set camera to inverted Yes
                SetCamerInverted(true);

                // Init Event
                uEye.Info.Camera.EnableMessage(true);
                m_Camera.EventFirstPacket += new EventHandler(onFirstPacketEvent);
                m_Camera.EventFrame += onFrameEvent;

                m_blnInitCameraDone = true;
                return true;
            }
        }


        public bool InitializeCamera(int intCameraPort)
        {
            if (m_blnInitCameraDone)
                return true;

            lock (m_objLock)
            {

                if (m_strErrorText.Length > 0)
                    m_strErrorText = string.Empty;

                uEye.Types.CameraInformation[] cameraList;
                uEye.Info.Camera.GetCameraList(out cameraList);

                bool bFound = false;
                int intCameraDeviceID = 0;
                foreach (uEye.Types.CameraInformation info in cameraList)
                {
                    if (info.DeviceID == intCameraPort)
                    {
                        intCameraDeviceID = Convert.ToInt32(info.DeviceID);
                        bFound = true;
                        break;
                    }
                }

                m_Camera = new uEye.Camera();

                if (!bFound)
                {
                    m_strErrorText = "Fail Init IDS Camera - No camera is connected to selected port.";
                    return false;
                }

                m_statusRet = m_Camera.Init(intCameraDeviceID | (Int32)uEye.Defines.DeviceEnumeration.UseDeviceID);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Initializing the camera failed";
                    return false;
                }

                m_statusRet = m_Camera.Memory.Allocate();
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Allocating memory failed";
                    return false;
                }

                // --- Disable all auto feature first  ---------

                // Disable Auto Exposure Feature
                m_statusRet = m_Camera.AutoFeatures.Software.Shutter.SetEnable(false);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Disable Auto Exposure failed";
                    return false;
                }

                // Disable Auto Gain Feature
                m_statusRet = m_Camera.AutoFeatures.Software.Gain.SetEnable(false);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Disable Auto Gain failed";
                    return false;
                }

                // Disable Auto Framerate Feature
                m_statusRet = m_Camera.AutoFeatures.Software.Framerate.SetEnable(false);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Disable Auto Framerate failed";
                    return false;
                }

                // -----------------------------------------------

                // Set pixel clock
                m_statusRet = m_Camera.Timing.PixelClock.Set(86);  // Set pixel clock to maximum to get highest framerate range
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Set pixel clock failed";
                    return false;
                }

                // set framerate
                m_statusRet = m_Camera.Timing.Framerate.Set(60);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Set framerate failed";
                    return false;
                }

                // Set exposure time
                m_statusRet = m_Camera.Timing.Exposure.Set(1);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Set Exposure failed";
                    return false;
                }

                // set camera pixel format to MONO8
                m_statusRet = m_Camera.PixelFormat.Set(uEye.Defines.ColorMode.Mono8);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Set camera pixel format failed";
                    return false;
                }

                // Set camera Color Convert/hardware
                m_statusRet = m_Camera.Color.Converter.Set(uEye.Defines.ColorMode.Mono8, uEye.Defines.ColorConvertMode.Hardware3X3);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Set camera color convert/hardware failed";
                    return false;
                }

                // Set camera trigger mode
                SetCameraTriggerMode(0);

                // Set camera to inverted Yes
                SetCamerInverted(true);

                // Init Event
                uEye.Info.Camera.EnableMessage(true);
                m_Camera.EventFirstPacket += new EventHandler(onFirstPacketEvent);
                m_Camera.EventFrame += onFrameEvent;

                m_blnInitCameraDone = true;
                return true;
            }
        }

        /// <summary>
        /// Set camera Trigger Mode
        /// </summary>
        /// <param name="intTriggerMode">Trigger Mode 0=Software Trigger, 1=External Trigger</param>
        /// <returns></returns>
        public bool SetCameraTriggerMode(int intTriggerMode)
        {
            lock (m_objLock)
            {
                switch (intTriggerMode)
                {
                    case 0:  // Software Trigger
                        m_Camera.Trigger.Set(uEye.Defines.TriggerMode.Software);
                        break;
                    case 1:  // External Trigger
                        m_Camera.Trigger.Set(uEye.Defines.TriggerMode.Hi_Lo);
                        break;
                    default:
                        return false;
                        break;
                }

                return true;
            }
        }

        public bool SetCamerInverted(bool blnInvertedYes)
        {
            lock (m_objLock)
            {

                if (blnInvertedYes)
                {
                    m_Camera.IO.Gpio.SetDirection(uEye.Defines.IO.GPIO.One, uEye.Defines.IO.Direction.Out);
                    m_Camera.IO.Gpio.SetState(uEye.Defines.IO.GPIO.One, uEye.Defines.IO.State.Low);
                    m_Camera.IO.Flash.SetMode(uEye.Defines.IO.FlashMode.TriggerHighActive);
                }
                else
                {
                    m_Camera.IO.Gpio.SetDirection(uEye.Defines.IO.GPIO.One, uEye.Defines.IO.Direction.Out);
                    m_Camera.IO.Gpio.SetState(uEye.Defines.IO.GPIO.One, uEye.Defines.IO.State.High);
                    m_Camera.IO.Flash.SetMode(uEye.Defines.IO.FlashMode.TriggerLowActive);
                }

                return true;
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="intOutputIndex">0=output1, 1=output2, 2=output3</param>
        /// <param name="intMode"></param>
        /// <returns></returns>
        public bool Output(int intOutputIndex, int intMode)
        {
            switch (intOutputIndex)
            {
                case 0: // Flash Outport
                    m_Camera.IO.Flash.SetMode(uEye.Defines.IO.FlashMode.TriggerHighActive);
                    break;
                case 1: // GPIO 1
                    m_Camera.IO.Flash.SetMode(uEye.Defines.IO.FlashMode.TriggerHighActive);
                    m_Camera.IO.Gpio.SetState(uEye.Defines.IO.GPIO.One, uEye.Defines.IO.State.High);
                    break;
                case 2: // GPIO 2
                    m_Camera.IO.Flash.SetMode(uEye.Defines.IO.FlashMode.TriggerHighActive);
                    m_Camera.IO.Gpio.SetState(uEye.Defines.IO.GPIO.Two, uEye.Defines.IO.State.High);
                    break;
            }

            return false;
        }

        public float GetShuttle()
        {
            lock (m_objLock)
            {
                double dValue;

                m_Camera.Timing.Exposure.Get(out dValue);

                return (float)dValue;
            }
        }

        public bool SetShuttle(float fValue)
        {
            lock (m_objLock)
            {
                // Set exposure time
                if ((fValue <= 0) || (fValue >10))
                    m_statusRet = m_Camera.Timing.Exposure.Set(1);
                else
                    m_statusRet = m_Camera.Timing.Exposure.Set((double)fValue);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Set Shuttle failed";
                    return false;
                }

                return true;
            }
        }

        public int GetGain()
        {
            lock (m_objLock)
            {
                int intValue;

                m_Camera.Gain.Hardware.Scaled.GetMaster(out intValue);

                return intValue;
            }
        }

        public bool SetGain(int intValue)
        {
            lock (m_objLock)
            {
                // Set gain
                intValue = intValue * 2; // 2018 08 09 - JBTAN: * 2 because previous max intValue is 100, current max intValue is 50
                m_statusRet = m_Camera.Gain.Hardware.Scaled.SetMaster(intValue);
                if (m_statusRet != uEye.Defines.Status.SUCCESS)
                {
                    m_strErrorText = "Set Gain failed";
                    return false;
                }

                return true;
            }
        }

        public void Dispose()
        {
            m_Camera.Exit();
        }
        
        private void onFrameEvent(object sender, EventArgs e)
        {
            lock (m_objLock)
            {
                try
                {
                    // convert sender object to our camera object
                    uEye.Camera camera = sender as uEye.Camera;

                    if (camera.IsOpened)
                    {
                        uEye.Defines.DisplayMode mode;
                        camera.Display.Mode.Get(out mode);

                        // only display in dib mode
                        if (mode == uEye.Defines.DisplayMode.DiB)
                        {
                            // only display in dib mode
                            if (mode == uEye.Defines.DisplayMode.DiB)
                            {
                                // Method 1
                                Int32 s32MemID;
                                camera.Memory.GetActive(out s32MemID);
                                camera.Memory.Lock(s32MemID);
                                m_Camera.Memory.ToBitmap(s32MemID, out m_bmpImage);
                                camera.Memory.Unlock(s32MemID);


                                // Method 2
                                //camera.Memory.ToIntPtr(out m_ptrImagePointer);
                                //byte[] objImage = new byte[0];
                                //camera.Memory.CopyToArray(s32MemID, out objImage);
                                //Marshal.Copy(objImage, 0, m_ptrImagePointer,objImage.Length);

                                //camera.Memory.Unlock(s32MemID);
                                m_objTransferTime.Stop();

                                m_blnGetFrameDone = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_strErrorText = "IDSuEyeCamera OnFrameEvent() error : " + ex.ToString();
                }
            }
        }

        private void onFirstPacketEvent(object sender, EventArgs e)
        {
            try
            {
                m_blnExposureDone = true;
                m_objExposureTime.Stop();
            }
            catch (Exception ex)
            {
                m_strErrorText = "IDSuEyeCamera FirstPackageEvent() error : " + ex.ToString();
            }
        }
    }
}
