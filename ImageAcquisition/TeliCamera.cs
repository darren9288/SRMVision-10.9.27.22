using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using Common;


namespace ImageAcquisition
{
    public class TeliCamera
    {
        #region Member Variables

        TeliCameraAPI m_objTeliCamera;

        #endregion

        /// <summary>
        /// AVT Driver
        /// </summary>
        public TeliCamera()
        {
            m_objTeliCamera = new ImageAcquisition.TeliCameraAPI();
        }
        /// <summary>
        /// AVT Driver
        /// </summary>
        /// <param name="intImageWidth">Grab image width</param>
        /// <param name="intImageHeight">Grab image height</param>
        public TeliCamera(int intImageWidth, int intImageHeight)
        {
            m_objTeliCamera = new ImageAcquisition.TeliCameraAPI(intImageWidth, intImageHeight);
        }

        public bool ConvertFrame(int intFrameIndex)
        {
            return m_objTeliCamera.ConvertFrame(intFrameIndex);
        }

        /// <summary>
        /// Reset the frame buffers
        /// </summary>
        /// <returns></returns>
        public bool DiscardFrame()
        {
            return m_objTeliCamera.DiscardFrame();
        }

        public void Dispose()
        {
            m_objTeliCamera.Dispose();
        }

        /// <summary>
        /// Grab an image
        /// </summary>
        /// <returns>true if successfukk, false otherwise</returns>
        public bool Grab()
        {
            return m_objTeliCamera.Grab();
        }
        public bool Grab(int intImageIndex)
        {
            return m_objTeliCamera.Grab(intImageIndex);
        }
        public bool Grab(ref IntPtr ptr, int intImageIndex)
        {
            return m_objTeliCamera.Grab(ref ptr, intImageIndex);
        }
        public bool GetFrame(int intFrameIndex)
        {
            return m_objTeliCamera.GetFrame(intFrameIndex);
        }
        public void ResetImageCount()
        {
            m_objTeliCamera.ref_intImageCount = 0;
        }
        public void SetImageCount(int intImageCount)
        {
            m_objTeliCamera.ref_intImageCount = intImageCount;
        }
        public bool WaitFrameReady()
        {
            return m_objTeliCamera.WaitFrameReady();
        }
        public bool WaitFrameReady(int intImageIndex)
        {
            return m_objTeliCamera.WaitFrameReady(intImageIndex);
        }
        public bool WaitFrameAcquiredReady(int intImageIndex)
        {
            return m_objTeliCamera.WaitFrameAcquiredReady(intImageIndex);
        }
        public bool WaitExposureDone()
        {
            return m_objTeliCamera.WaitExposureDone();
        }
        public bool WaitTriggerWaitDone()
        {
            return m_objTeliCamera.WaitTriggerWaitDone();
        }
        public bool WaitTriggerDone()
        {
            return m_objTeliCamera.WaitTriggerDone();
        }
        public void TriggerImageBufferRead()
        {
            m_objTeliCamera.TriggerImageBufferRead();
        }
        public static string InitializeSystem()
        {
            return TeliCameraAPI.InitializeSystem();
        }

        public bool InitializeCamera(String SerialNo, int intResolutionX, int intResolutionY)
        {
            return m_objTeliCamera.InitializeCamera(SerialNo, "", intResolutionX, intResolutionY);
        }

        public bool InitializeCamera_LowLevelAPI(String SerialNo, int intResolutionX, int intResolutionY, bool blnColor)
        {
            return m_objTeliCamera.InitializeCamera_LowLevelAPI(SerialNo, "", intResolutionX, intResolutionY, blnColor);
        }

        public bool InitializeCameraUsingUserDefineName(String strUserDefineName, int intResolutionX, int intResolutionY)
        {
            return m_objTeliCamera.InitializeCamera("", strUserDefineName, intResolutionX, intResolutionY);
        }
        public void SetWhiteBalance(double dRedRatio, double dBlueRatio)
        {
            m_objTeliCamera.SetWhiteBalance(dRedRatio, dBlueRatio);
        }
        public void SetWhiteBalance_ForRed(double dRedRatio)
        {
            m_objTeliCamera.SetWhiteBalance_ForRed(dRedRatio);
        }
        public void SetWhiteBalance_ForBlue(double dBlueRatio)
        {
            m_objTeliCamera.SetWhiteBalance_ForBlue(dBlueRatio);
        }
        public bool OFFCamera()
        {
            if (!m_objTeliCamera.ref_blnCameraInitDone)
                return false;

            return m_objTeliCamera.OFFCamera();
        }
        public bool OFFCamera_LowLevelAPI()
        {
            if (!m_objTeliCamera.ref_blnCameraInitDone)
                return false;

            return m_objTeliCamera.OFFCamera_LowLevelAPI();
        }
        /// <summary>
        /// Set output bit On/Off
        /// </summary>
        /// <param name="intID">0=output1, 1=output2, 2=output3</param>
        /// <param name="intMode">1= integration enable -> yes, 2 = integration enable -> no, 3= off mode</param>
        /// <returns>false = action fail</returns>
        public bool OutPort(int intID, int intMode)
        {
            return m_objTeliCamera.OutPort(intID, intMode);
        }

        public bool ReleaseImage(int intFrameIndex)
        {
            return m_objTeliCamera.ReleaseImage(intFrameIndex);
        }

        public bool SetCameraParameter(int intParamType, float fValue)
        {
            //STTrackLog.WriteLine("SetCameraParameter =" + intParamType.ToString());
            return m_objTeliCamera.SetCameraParameter(intParamType, fValue);
        }

        public bool IsCameraInitDone()
        {
            return m_objTeliCamera.ref_blnCameraInitDone;
        }

        public IntPtr GetImagePointer()
        {
            return m_objTeliCamera.ref_ptrImagePointer;

        }
        public IntPtr GetImageBufferPointer(int intIndex)
        {
            return m_objTeliCamera.ref_arrBufferPointer[intIndex];

        }
        public IntPtr GetBufferPointer()
        {
            return m_objTeliCamera.ref_ptrBufferPointer;

        }
        public BitmapData GetBitmapData()
        {
            return m_objTeliCamera.ref_objBitmapData;

        }
        public void ConvertImage(BitmapData objBitMapData, IntPtr objBufferPointer)
        {
            m_objTeliCamera.ConvertImage(objBitMapData, objBufferPointer);
        }
        public void ConvertImage(IntPtr objBufferPointer)
        {
            m_objTeliCamera.ConvertImage(objBufferPointer);
        }
        public void ConvertImage(int intImageIndex)
        {
            m_objTeliCamera.ConvertImage(intImageIndex);
        }
        public string GetErrorMessage()
        {
            return m_objTeliCamera.ref_strErrorText;
        }
        public void GetWhiteBalance(ref double dRedValue, ref double dBlueValue)
        {
            m_objTeliCamera.GetWhiteBalance(ref dRedValue, ref dBlueValue);
        }
        public double GetRedRatioMin()
        {
            return m_objTeliCamera.ref_dRedRatioMin;
        }
        public double GetRedRatioMax()
        {
            return m_objTeliCamera.ref_dRedRatioMax;
        }
        public double GetBlueRatioMin()
        {
            return m_objTeliCamera.ref_dBlueRatioMin;
        }
        public double GetBlueRatioMax()
        {
            return m_objTeliCamera.ref_dBlueRatioMax;
        }
        public void SetWhiteBalanceAuto()
        {
            m_objTeliCamera.SetWhiteBalanceAuto();
        }
        public void GetWhiteBalanceAuto()
        {
            m_objTeliCamera.GetWhiteBalanceAuto();
        }

    }
}
