using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Common;

namespace ImageAcquisition
{
    public class AVTVimba
    {
        #region Member Variables
        private bool blnNewCamera = false;
        private string serialno;
     
        private int VisionCount=0;
        private bool m_blnCameraInitDone = false;
        private List<int> m_intCameraPort = new List<int>();
        private List<int> m_intVisionCount = new List<int>();
        private List<string> m_strCameraSerialNo = new List<string>();
        private int m_intErrorCount = 0;
        private int m_intNextGrabDelay = 4;
        private int m_intSetGainDelay = 0;
        private int[] m_arrTriggerOutState = { -1, -1, -1 };
        private double m_dShuttlePrev = double.MaxValue;
        private double m_dGainPrev = double.MaxValue;
        private float m_fGrabTime = 0;
        private string m_strErrorText = "";
        private Bitmap m_bmpImage = new Bitmap(640, 480, PixelFormat.Format24bppRgb);              // 24 bits per pixel; 8 bits each are used for blue, green, red.

        // vimba
        private VimbaHelper m_VimbaHelper = new VimbaHelper();

        #endregion

        #region Properties

        public bool ref_blnCameraInitDone { get { return m_blnCameraInitDone; } }
        public int ref_intErrorCount { get { return m_intErrorCount; } set { m_intErrorCount = value; } }
        public float ref_fGrabTime { get { return m_fGrabTime; } }
        public string ref_strErrorText { get { return m_strErrorText; } }
        public IntPtr ref_ptrImagePointer { get { return m_VimbaHelper.ImagePointer; } set { m_VimbaHelper.ImagePointer = value; } }
        public double ref_dShuttlePrev { get { return m_dShuttlePrev; } }
        public double ref_dGainPrev { get { return m_dGainPrev; } }
        public int ref_intNextGrabDelay { get { return m_intNextGrabDelay; } }
        public int ref_intSetGainDelay { get { return m_intSetGainDelay; } }

        #endregion

        public AVTVimba()
        {
            m_VimbaHelper.Startup();
        }
        /// <summary>
        /// AVT Driver
        /// </summary>
        /// <param name="intPICBOX_WIDTH">Grab image width</param>
        /// <param name="intPICBOX_HEIGHT">Grab image height</param>
        public AVTVimba(int intPICBOX_WIDTH, int intPICBOX_HEIGHT)
        {
            m_VimbaHelper.Startup();
            m_bmpImage = new Bitmap(intPICBOX_WIDTH, intPICBOX_HEIGHT, PixelFormat.Format24bppRgb);
        }
        /// <summary>
        /// Get all camera's ID and port no that connect to PC
        /// </summary>
        /// <param name="arrCameraPortNoList">Store camera port no list</param>
        /// <param name="arrCameraIDList">Store camera ID list</param>
        public void GetNodeList(ref List<int> arrCameraPortNoList, ref List<string> arrCameraIDList, ref List<string> arrCameraModelList, ref List<string> arrCameraSerialNumberList)
        {
            if (m_VimbaHelper.CameraList.Count == 0)
                return;

            List<CameraInfo> cameras = m_VimbaHelper.CameraList;

            int i = 0;
            foreach (CameraInfo cameraInfo in cameras)
            {
                arrCameraPortNoList.Add( (int)m_VimbaHelper.GetCameraConnectedPortNo(i));
                arrCameraIDList.Add(cameraInfo.ID);
                arrCameraModelList.Add(cameraInfo.Model);
                arrCameraSerialNumberList.Add(cameraInfo.SerialNumber);
                //t.WriteLine("Inside GetNodeList");
                //t.WriteLine("i="+i.ToString());
                //t.WriteLine("Port="+(int)m_VimbaHelper.GetCameraConnectedPortNo(i));
                //t.WriteLine("ID="+cameraInfo.ID);
                //t.WriteLine("Model="+cameraInfo.Model);
                //t.WriteLine("Serial Number="+cameraInfo.SerialNumber);
                i++;
            }
        }
        public void GetNodeList2(ref List<int> arrCameraPortNoList2, ref List<string> arrCameraIDList2, ref List<string> arrCameraModelList2, ref List<string> arrCameraSerialNumberList2)
        {
            if (m_VimbaHelper.CameraList.Count == 0)
                return;
            
            List<CameraInfo> cameras = m_VimbaHelper.CameraList;

            int i = 0;
            foreach (CameraInfo cameraInfo in cameras)
            {
                arrCameraPortNoList2.Add((int)m_VimbaHelper.GetCameraConnectedPortNo2(i, ref blnNewCamera, ref serialno));
                arrCameraIDList2.Add(cameraInfo.ID);
                arrCameraModelList2.Add(cameraInfo.Model);
                arrCameraSerialNumberList2.Add(cameraInfo.SerialNumber);
                //t.WriteLine("Inside GetNodeList 2");
                //t.WriteLine("i=" + i.ToString());
                //t.WriteLine("Port=" + (int)m_VimbaHelper.GetCameraConnectedPortNo2(i, ref blnNewCamera, ref serialno));
                //t.WriteLine("ID=" + cameraInfo.ID);
                //t.WriteLine("Model=" + cameraInfo.Model);
                //t.WriteLine("Serial Number=" + cameraInfo.SerialNumber);
                //t.WriteLine("serialno=" + serialno);
                i++;
            }
        }
        public bool CheckCameras()
        {
            if (m_VimbaHelper.CameraList.Count == 0)
                return false;
            else
                return true;
        }

        public bool ConvertFrame(int intFrameIndex)
        {
            return true;
        }

        public bool DiscardFrame()
        {
            return true;
        }

        public bool Grab()
        {
            if (!m_blnCameraInitDone)
                return false;

            m_VimbaHelper.Grab();
            return true;
        }

        public bool GetFrame(int intFrameIndex)
        {
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            while(true)
            {
                if (m_VimbaHelper.IsReceivedFrame())
                    break;

                if (timeout.Timing > 3000)
                {
                    //if (SRMMessageBox.Show("Fail Camera - Get Image Timeout", "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    //{
                    //    
                    //    STTrackLog.WriteLine("Fail Camera - Get Image Timeout");
                    //}
                    m_strErrorText = "Fail Camera - Get Image Timeout";
                    timeout.Stop();
                    return false;
                }

                Thread.Sleep(1);
            }
            timeout.Stop();

            return true;
        }

        public bool WaitFrameReady(int intFrameIndex)
        {
            return true;
        }

      public bool VerifyCamera(string Serial, int Port, int Vision)
        {
            for (int i = 0; i < m_intVisionCount.Count; i++)
            {
               // t.WriteLine("In Config Verify Camera");
               //t.WriteLine("Vision "+m_intVisionCount[i].ToString());
               // t.WriteLine("Port "+m_intCameraPort[i].ToString());
               // t.WriteLine("Serial "+m_strCameraSerialNo[i]);
                if (m_intVisionCount[i] == Vision && m_strCameraSerialNo[i] == Serial && m_intCameraPort[i] == Port)
                    return true;
            }
            return false;
        }
        public bool InitializeCamera(string intSerialNo, int intPortNo)// , bool Display)
        {

            //m_intCameraSerialNo = intSerialNo;
            //m_intCameraPort = intPortNo;
           
            List<CameraInfo> cameras =  m_VimbaHelper.CameraList;

            bool bFound = false;
            int i = 0;
            CameraInfo newSelectedItem = null;
            foreach (CameraInfo cameraInfo in cameras)
            {
                string intCameraSerialNo = cameraInfo.SerialNumber;
                int intCameraPortNo = (int)m_VimbaHelper.GetCameraConnectedPortNo2(i,ref blnNewCamera, ref serialno);
               // t.WriteLine("Inside Config: "+" Serial "+ intCameraSerialNo+" Port "+ intCameraPortNo.ToString());
                if (intCameraSerialNo == intSerialNo && intCameraPortNo == intPortNo)
                {
                 //   t.WriteLine("Matched : registry port" + intPortNo.ToString() + " with camera "+ intCameraSerialNo + " at Port " + intCameraPortNo.ToString());
                    newSelectedItem = cameraInfo;
                    bFound = true;
                    break;
                }

                i++;
            }

            if (!bFound)// && Display )
            {
//#if(!RTXDebug)

//                if (SRMMessageBox.Show("Camera with Serial Number " + intSerialNo.ToString()+ " is not connected to Port " + intPortNo.ToString(), "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
//                {

//                    //STTrackLog.WriteLine("Camera is not connected to interface port " + intPortNo.ToString());
//                }
//#endif
//                m_strErrorText = "Camera with Serial Number " + intSerialNo.ToString() + " is not connected to Port " + intPortNo.ToString();
                return false;
            }

            // --------- init parameter setting ---------------------------
            if (blnNewCamera)
            {
                switch (newSelectedItem.Model)
                {
                    
                    case "Guppy PRO F031B":
                        if (serialno == newSelectedItem.SerialNumber)
                            return false;
                        PreSetCameraParameters_GPF031B(newSelectedItem);
                        break;
                    case "Guppy PRO F033B":
                        if (serialno == newSelectedItem.SerialNumber)
                            return false;
                        PreSetCameraParameters_GPF033B(newSelectedItem, false);
                        break;
                    default:
                        if (SRMMessageBox.Show("This camera model " + newSelectedItem.Model + " is not supported by this SRMVision software.", "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                        {

                            //STTrackLog.WriteLine("This camera model " + newSelectedItem.Model + " is not supported by this SRMVision software.");
                        }
                        m_strErrorText = "This camera model " + newSelectedItem.Model + " is not supported by this SRMVision software.";
                        return false;
                }
            }
            m_blnCameraInitDone = true;
            return true;
        }
        public bool InitializeCamera(int intPortNo, bool blnFlip)
        {
           // m_intCameraPort = intPortNo;
           // t.WriteLine("Start getting camera list");
            List<CameraInfo> cameras = m_VimbaHelper.CameraList;
           
           bool bFound = false;
            int i = 0;
            CameraInfo newSelectedItem = null;
            foreach (CameraInfo cameraInfo in cameras)
            {
              //  t.WriteLine(cameras[i].Model + " " + cameras[i].SerialNumber);
                int intCameraPortNo = (int)m_VimbaHelper.GetCameraConnectedPortNo(i);
               // t.WriteLine("Loop : "+i+" - Port "+ intCameraPortNo.ToString());
                if (intCameraPortNo == intPortNo)
                {
                  //  t.WriteLine("Matched : registry port" + intPortNo.ToString() + " with camera Port " + intCameraPortNo.ToString());
                    newSelectedItem = cameraInfo;
                    bFound = true;
                    VisionCount+=1;
                    m_intVisionCount.Add(VisionCount);
                    m_intCameraPort.Add(intCameraPortNo);
                    m_strCameraSerialNo.Add(cameraInfo.SerialNumber);
                    //t.WriteLine("Vision " + m_intVisionCount[i].ToString());
                    //t.WriteLine("Port " + m_intCameraPort[i].ToString());
                    //t.WriteLine("Serial " + m_strCameraSerialNo[i]);
                    break;
                }

                i++;
            }

            if (!bFound)
            {
#if(!RTXDebug)

                if (SRMMessageBox.Show("Camera is not connected to interface port " + intPortNo.ToString(), "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                {
                    
                    //STTrackLog.WriteLine("Camera is not connected to interface port " + intPortNo.ToString());
                }
#endif
                m_strErrorText = "Camera is not connected to interface port " + intPortNo.ToString();
                return false;
            }

            // --------- init parameter setting ---------------------------
            switch (newSelectedItem.Model)
            {
                case "Guppy PRO F031B":
                    PreSetCameraParameters_GPF031B(newSelectedItem);
                    break;
                case "Guppy PRO F033B":
                    PreSetCameraParameters_GPF033B(newSelectedItem, blnFlip);
                    break;
                default:
                    if (SRMMessageBox.Show("This camera model " + newSelectedItem.Model + " is not supported by this SRMVision software.", "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    {
                        
                        //STTrackLog.WriteLine("This camera model " + newSelectedItem.Model + " is not supported by this SRMVision software.");
                    }
                    m_strErrorText = "This camera model " + newSelectedItem.Model + " is not supported by this SRMVision software.";
                    return false;
            }

            m_blnCameraInitDone = true;
            return true;
        }

        public bool InitializeCamera2(int intPortNo, bool blnFlip)
        {
         //   m_intCameraPort = intPortNo;

            List<CameraInfo> cameras = m_VimbaHelper.CameraList;

            bool bFound = false;
            int i = 0;
            CameraInfo newSelectedItem = null;
            foreach (CameraInfo cameraInfo in cameras)
            {
                int intCameraPortNo = (int)m_VimbaHelper.GetCameraConnectedPortNo(i);
                if (intCameraPortNo == intPortNo)
                {
                    newSelectedItem = cameraInfo;
                    bFound = true;
                    break;
                }

                i++;
            }

            if (!bFound)
            {
#if(!RTXDebug)

                if (SRMMessageBox.Show("Camera is not connected to interface port " + intPortNo.ToString(), "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                {

                    //STTrackLog.WriteLine("Camera is not connected to interface port " + intPortNo.ToString());
                }
#endif
                m_strErrorText = "Camera is not connected to interface port " + intPortNo.ToString();
                return false;
            }

            // --------- init parameter setting ---------------------------
            switch (newSelectedItem.Model)
            {
                case "Guppy PRO F031B":
                    PreSetCameraParameters_GPF031B(newSelectedItem);
                    break;
                case "Guppy PRO F033B":
                    PreSetCameraParameters_GPF033B(newSelectedItem, blnFlip);
                    break;
                default:
                    if (SRMMessageBox.Show("This camera model " + newSelectedItem.Model + " is not supported by this SRMVision software.", "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK)
                    {

                        //STTrackLog.WriteLine("This camera model " + newSelectedItem.Model + " is not supported by this SRMVision software.");
                    }
                    m_strErrorText = "This camera model " + newSelectedItem.Model + " is not supported by this SRMVision software.";
                    return false;
            }

            m_blnCameraInitDone = true;
            return true;
        }

        public void GetCameraWBParameter(ref uint intValueCB, ref uint intValueCR)
        {
            //m_objCamera.GetParameter(enFGParameter.E_WHITEBALCB, ref intValueCB);
            //m_objCamera.GetParameter(enFGParameter.E_WHITEBALCR, ref intValueCR);
        }

        private bool PreSetCameraParameters_GPF031B(CameraInfo objCameraInfo)
        {
            m_VimbaHelper.InitCamera_F033B(objCameraInfo.ID, false);

            // Camera Pre Setting


            return true;
        }

        private bool PreSetCameraParameters_GPF033B(CameraInfo objCameraInfo, bool blnFlip)
        {
            m_VimbaHelper.InitCamera_F033B_2(objCameraInfo.ID, blnFlip, ref m_bmpImage);

            // Camera Pre Setting

            return true;
        }

        public bool OFFCamera()
        {
            //if (m_blnCameraInitDone)
                m_VimbaHelper.Shutdown();

            return true;
        }
        public bool StartCamera()
        {
            //if (m_blnCameraInitDone)
            m_VimbaHelper.Startup();

            return true;
        }
        public bool OutPort(int intSelectOutport, int intMode)
        {
            if (m_arrTriggerOutState[intSelectOutport] == intMode)
                return true;

            switch (intMode)
            {
                case 1:
                    return m_VimbaHelper.OutPort(intSelectOutport, true, true); // Set Integration Enable Mode + Invert Yes
                case 2: 
                    return m_VimbaHelper.OutPort(intSelectOutport, true, false); // Set Integration Enable Mode + Invert No
                case 4:
                    return m_VimbaHelper.OutPortDirect(intSelectOutport, true); // Set Direct High
                case 5:
                    return m_VimbaHelper.OutPortDirect(intSelectOutport, false);   // Set Direct Low
                case 0:
                default: 
                    return m_VimbaHelper.OutPort(intSelectOutport, false, false); // Set Off Mode
            }
        }

        public bool QuickOutPort(int intSelectOutport, int intMode)
        {
            if (m_arrTriggerOutState[intSelectOutport] == intMode)
                return true;

            switch (intMode)
            {
                case 1:
                    return m_VimbaHelper.OutPort(intSelectOutport, true); // Set Integration Enable Mode + Invert Yes
                case 2:
                    return m_VimbaHelper.OutPort(intSelectOutport, true); // Set Integration Enable Mode + Invert No
                case 4:
                    return m_VimbaHelper.OutPortDirect(intSelectOutport, true); // Set Direct High
                case 5:
                    return m_VimbaHelper.OutPortDirect(intSelectOutport, false);   // Set Direct Low
                case 0:
                default:
                    return m_VimbaHelper.OutPort(intSelectOutport, false); // Set Off Mode
            }
        }

        public bool InvertPort(int intSelectOutport, bool blnInverted)
        {
            return m_VimbaHelper.InvertPort(intSelectOutport, blnInverted);
        }

        public bool ReleaseImage(int intFrameIndex)
        {
            m_VimbaHelper.ReleaseImage();
            return true;
        }

        public bool SetCameraParameter_Force(int intParamType, double dValue)
        {
            switch (intParamType)
            {
                case 1:
                    if (m_VimbaHelper.SetExposure(dValue))
                    {
                        Thread.Sleep(10);
                        m_dShuttlePrev = dValue;
                        return true;
                    }

                    break;
                case 2:

                    if (m_VimbaHelper.SetGain(dValue))
                    {
                        Thread.Sleep(10);

                        m_dGainPrev = dValue;
                        return true;
                    }

                    break;
            }
            return false;
        }

        public double GetCameraParameter(int intParamType)
        {
            double dValue = 0;
            switch (intParamType)
            {
                case 1:
                    dValue = m_VimbaHelper.GetExposure();
                    break;
                case 2:
                    m_VimbaHelper.GetGain();
                    break;
                case 3:
                    // Get Brightness
                    break;
                case 4:
                    // Get Gama
                    break;
                case 5:
                    // Get White Balance CB
                    break;
                case 6:
                    // Get White Balance CR
                    break;
            }

            return dValue;
        }

        public bool SetCameraParameter(int intParamType, double dValue)
        {
            switch (intParamType)
            {
                case 1:
                    //if (m_uintShuttlePrev != intValue)
                    if (m_VimbaHelper.GetExposure() != dValue)
                    {
                        if (m_VimbaHelper.SetExposure(dValue))
                        {
                            Thread.Sleep(10);
                            m_dShuttlePrev = dValue;
                            return true;
                        }


                    }
                    else
                        return true;
                    break;
                case 2:
                    //if (m_uintGainPrev != intValue)
                    double fff = m_VimbaHelper.GetGain();
                    if (m_VimbaHelper.GetGain() != dValue)
                    {
                        if (m_VimbaHelper.SetGain(dValue))
                        {
                            Thread.Sleep(10);

                            m_dGainPrev = dValue;
                            return true;
                        }


                    }
                    else
                        return true;
                    break;
            }
            return false;
        }

        public bool SetCameraParameter_Quick(int intParamType, double dValue)
        {
            switch (intParamType)
            {
                case 1:
                    if (m_dShuttlePrev != dValue)
                    {
                        if (m_VimbaHelper.SetExposure(dValue))
                        {
                            m_dShuttlePrev = dValue;
                            return true;
                        }
                    }
                    else
                        return true;
                    break;
                case 2:
                    if (m_dGainPrev != dValue)
                    {
                        if (m_VimbaHelper.SetGain(dValue))
                        {
                            m_dGainPrev = dValue;
                            return true;
                        }


                    }
                    else
                        return true;
                    break;
            }
            return false;
        }

        public bool SetCameraFrameSize(int intStartX, int intStartY, int intWidth, int intHeight)
        {
            return true;
        }

        //  ------------ Rotary Project --------------------------------------------------------------------------------------------------------        
        public bool AutoCalibrateWhiteBalance(int intType)
        {
            return true;    // Temporary

            //switch (intType)
            //{
            //    case 1:
            //        if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCR, enFGParameterState.E_AUTO) == enFireWrapResult.E_NOERROR)
            //        {
            //            if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCB, enFGParameterState.E_AUTO) == enFireWrapResult.E_NOERROR)
            //                return true;
            //        }
            //        break;
            //    case 2:
            //        // Wait last grabbing done
            //        int intCount = 0;
            //        while (m_blnIsGrabbing && (intCount < 3000))
            //        {
            //            Thread.Sleep(1);
            //            intCount++;
            //        }

            //        m_blnPauseGrab = true; // Trigger event to pause grab image function 

            //        if (intCount >= 3000)
            //        {
            //            m_strErrorText = "Fail Camera - ONESHOT cannot be triggered because grabbing never end.";
            //            m_blnPauseGrab = false;
            //            return false;
            //        }


            //        // Trigger ONESHOT grab image
            //        if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCR, enFGParameterState.E_ONESHOT) == enFireWrapResult.E_NOERROR)
            //        {
            //            if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCB, enFGParameterState.E_ONESHOT) == enFireWrapResult.E_NOERROR)
            //            {
            //                Thread.Sleep(700); // Delay to make sure all ONESHOT image is fully transfered and processed before allow for next grab.
            //                m_blnPauseGrab = false; // Reset event to allow grab image function.
            //                return true;
            //            }
            //        }

            //        m_blnPauseGrab = false;
            //        break;
            //    case 3: // Off auto
            //        if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCR, enFGParameterState.E_OFF) == enFireWrapResult.E_NOERROR)
            //        {
            //            if (m_objCamera.SetParameter(enFGParameter.E_WHITEBALCB, enFGParameterState.E_OFF) == enFireWrapResult.E_NOERROR)
            //                return true;
            //        }
            //        break;
            //}

            //return false;
        }

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
    }
}
