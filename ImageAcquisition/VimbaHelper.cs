/*=============================================================================
  Copyright (C) 2012 Allied Vision Technologies.  All Rights Reserved.

  Redistribution of this file, in original or modified form, without
  prior written consent of Allied Vision Technologies is prohibited.

-------------------------------------------------------------------------------

  File:        VimbaHelper.cs

  Description: Implementation file for the VimbaHelper class that demonstrates
               how to implement an asynchronous, continuous image acquisition
               with VimbaNET.

-------------------------------------------------------------------------------

  THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR IMPLIED
  WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF TITLE,
  NON-INFRINGEMENT, MERCHANTABILITY AND FITNESS FOR A PARTICULAR  PURPOSE ARE
  DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT,
  INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED
  AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
  TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
  OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

=============================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using AVT.VmbAPINET;
using System.Runtime.InteropServices;
using Common;

namespace ImageAcquisition
{
    
    //A simple container class for infos (name and ID) about a camera
    public class CameraInfo
    {
        private string m_Model = null;
        private string m_Name = null;
        private string m_ID = null;
        private string m_SerialNumber = null;
        public CameraInfo(string name, string id, string model, string serialnumber)
        {
            if(null == name)
            {
                throw new ArgumentNullException("name");
            }
            if(null == id)
            {
                throw new ArgumentNullException("id");
            }
            if (null == model)
            {
                throw new ArgumentNullException("model");
            }
            if(null == serialnumber)
            {
                throw new ArgumentNullException("serialnumber");
            }
            m_Name = name;
            m_ID = id;
            m_Model = model;
            m_SerialNumber = serialnumber;
        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public string ID
        {
            get
            {
                return m_ID;
            }
        }

        public string Model
        {
            get
            {
                return m_Model;
            }
        }
        public string SerialNumber
        {
            get
            {
                return m_SerialNumber;
            }
        }
        public override string ToString()
        {
            return m_Name;
        }
    }

    //Event args class that will contain a single image
    public class FrameEventArgs : EventArgs
    {
        private Image       m_Image = null;
        private Exception   m_Exception = null;

        public FrameEventArgs(Image image)
        {
            if(null == image)
            {
                throw new ArgumentNullException("image");
            }

            m_Image = image;
        }

        public FrameEventArgs(Exception exception)
        {
            if(null == exception)
            {
                throw new ArgumentNullException("exception");
            }

            m_Exception = exception;
        }

        public Image Image
        {
            get
            {
                return m_Image;
            }
        }

        public Exception Exception
        {
            get
            {
                return m_Exception;
            }
        }
    }

    //Delegates for our callbacks
    public delegate void CameraListChangedHandler(object sender, EventArgs args);
    public delegate void FrameReceivedHandler(object sender, FrameEventArgs args);

    //A helper class as a wrapper around Vimba
    public class VimbaHelper
    {
  
        private bool m_blnCameraInitDone = false;
        private bool m_bReceivedFrame = false;
        private Bitmap m_bmpImage = null;
        private IntPtr m_ptrImagePointer;

        private Vimba                       m_Vimba = null;                     //Main Vimba API entry object
        private CameraListChangedHandler    m_CameraListChangedHandler = null;  //Camera list changed handler
        private Camera                      m_Camera = null;                    //Camera object if camera is open
        private bool                        m_Acquiring = false;                //Flag to remember if acquisition is running
        private FrameReceivedHandler        m_FrameReceivedHandler = null;      //Frames received handler
        private const int                   m_RingBitmapSize = 2;               //Amount of Bitmaps in RingBitmap
        private static RingBitmap           m_RingBitmap = null;                //Bitmaps to display images
        private static readonly object      m_ImageInUseSyncLock = new object();//Protector for m_ImageInUse
        private static bool                 m_ImageInUse = true;                //Signal of picture box that image is used

        private int m_intOutputPinPrev = -1;
        private string[] m_strLineSourcePrev = { "", "", "" };
        private bool[] m_strLineStatusPrev = { false, false, false };

        public VimbaHelper()
        {
            m_RingBitmap = new RingBitmap(m_RingBitmapSize);
        }

        ~VimbaHelper()
        {
            if (m_blnCameraInitDone)
            {
                //Release Vimba API if user forgot to call Shutdown
                ReleaseVimba();
            }
        }


        //set/get flag, signals a displayed image
        public static bool ImageInUse
        {
            set
            {
                lock (m_ImageInUseSyncLock)
                {
                    m_ImageInUse = value;
                }
            }
            get
            {
                lock (m_ImageInUseSyncLock)
                {
                    return m_ImageInUse;
                }
            }
        }

        //Convert frame to displayable image
        private static Image ConvertFrameToImage(Frame frame)
        {
            if (null == frame)
            {
                throw new ArgumentNullException("frame");
            }

            //Check if the image is valid
            if (VmbFrameStatusType.VmbFrameStatusComplete != frame.ReceiveStatus)
            {
                throw new Exception("Invalid frame received. Reason: " + frame.ReceiveStatus.ToString());
            }

            //Convert raw frame data into image (for image display)
            Image image = null;
            switch (frame.PixelFormat)
            {
                case VmbPixelFormatType.VmbPixelFormatMono8:
                    {
                        Bitmap bitmap = new Bitmap((int)frame.Width, (int)frame.Height, PixelFormat.Format8bppIndexed);

                        //Set greyscale palette
                        ColorPalette palette = bitmap.Palette;
                        for (int i = 0; i < palette.Entries.Length; i++)
                        {
                            palette.Entries[i] = Color.FromArgb(i, i, i);
                        }
                        bitmap.Palette = palette;

                        //Copy image data
                        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0,
                                                                                    0,
                                                                                    (int)frame.Width,
                                                                                    (int)frame.Height),
                                                                    ImageLockMode.WriteOnly,
                                                                    PixelFormat.Format8bppIndexed);
                        try
                        {
                            //Copy image data line by line
                            for (int y = 0; y < (int)frame.Height; y++)
                            {
                                System.Runtime.InteropServices.Marshal.Copy(frame.Buffer,
                                                                                y * (int)frame.Width,
                                                                                new IntPtr(bitmapData.Scan0.ToInt64() + y * bitmapData.Stride),
                                                                                (int)frame.Width);
                            }
                        }
                        finally
                        {
                            bitmap.UnlockBits(bitmapData);
                        }

                        image = bitmap;
                    }
                    break;

                case VmbPixelFormatType.VmbPixelFormatBgr8:
                    {
                        Bitmap bitmap = new Bitmap((int)frame.Width, (int)frame.Height, PixelFormat.Format24bppRgb);

                        //Copy image data
                        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0,
                                                                                    0,
                                                                                    (int)frame.Width,
                                                                                    (int)frame.Height),
                                                                    ImageLockMode.WriteOnly,
                                                                    PixelFormat.Format24bppRgb);
                        try
                        {
                            //Copy image data line by line
                            for (int y = 0; y < (int)frame.Height; y++)
                            {
                                System.Runtime.InteropServices.Marshal.Copy(frame.Buffer,
                                                                                y * ((int)frame.Width) * 3,
                                                                                new IntPtr(bitmapData.Scan0.ToInt64() + y * bitmapData.Stride),
                                                                                ((int)(frame.Width) * 3));
                            }
                        }
                        finally
                        {
                            bitmap.UnlockBits(bitmapData);
                        }

                        image = bitmap;
                    }
                    break;

                default:
                    throw new Exception("Current pixel format is not supported by this example (only Mono8 and BRG8Packed are supported).");
            }

            return image;
        }

        private static Bitmap ConvertFrameToBitmap(Frame frame)
        {
            if (null == frame)
            {
                throw new ArgumentNullException("frame");
            }

            //Check if the image is valid
            if (VmbFrameStatusType.VmbFrameStatusComplete != frame.ReceiveStatus)
            {
                throw new Exception("Invalid frame received. Reason: " + frame.ReceiveStatus.ToString());
            }

            //Convert raw frame data into image (for image display)
            Bitmap bitmap = null;
            switch (frame.PixelFormat)
            {
                case VmbPixelFormatType.VmbPixelFormatMono8:
                    {
                        bitmap = new Bitmap((int)frame.Width, (int)frame.Height, PixelFormat.Format8bppIndexed);

                        //Set greyscale palette
                        ColorPalette palette = bitmap.Palette;
                        for (int i = 0; i < palette.Entries.Length; i++)
                        {
                            palette.Entries[i] = Color.FromArgb(i, i, i);
                        }
                        bitmap.Palette = palette;

                        //Copy image data
                        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0,
                                                                                    0,
                                                                                    (int)frame.Width,
                                                                                    (int)frame.Height),
                                                                    ImageLockMode.WriteOnly,
                                                                    PixelFormat.Format8bppIndexed);
                        try
                        {
                            //Copy image data line by line
                            for (int y = 0; y < (int)frame.Height; y++)
                            {
                                System.Runtime.InteropServices.Marshal.Copy(frame.Buffer,
                                                                                y * (int)frame.Width,
                                                                                new IntPtr(bitmapData.Scan0.ToInt64() + y * bitmapData.Stride),
                                                                                (int)frame.Width);

                            }
                        }
                        finally
                        {
                            bitmap.UnlockBits(bitmapData);
                        }
                    }
                    break;

                case VmbPixelFormatType.VmbPixelFormatBgr8:
                    {
                        bitmap = new Bitmap((int)frame.Width, (int)frame.Height, PixelFormat.Format24bppRgb);

                        //Copy image data
                        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0,
                                                                                    0,
                                                                                    (int)frame.Width,
                                                                                    (int)frame.Height),
                                                                    ImageLockMode.WriteOnly,
                                                                    PixelFormat.Format24bppRgb);
                        try
                        {
                            //Copy image data line by line
                            for (int y = 0; y < (int)frame.Height; y++)
                            {
                                System.Runtime.InteropServices.Marshal.Copy(frame.Buffer,
                                                                                y * ((int)frame.Width) * 3,
                                                                                new IntPtr(bitmapData.Scan0.ToInt64() + y * bitmapData.Stride),
                                                                                ((int)(frame.Width) * 3));
                            }
                        }
                        finally
                        {
                            bitmap.UnlockBits(bitmapData);
                        }
                    }
                    break;

                default:
                    throw new Exception("Current pixel format is not supported by this example (only Mono8 and BRG8Packed are supported).");
            }

            return bitmap;
        }

        private static IntPtr ConvertFrameToImagePointer(Frame frame)
        {
            if (null == frame)
            {
                throw new ArgumentNullException("frame");
            }

            //Check if the image is valid
            if (VmbFrameStatusType.VmbFrameStatusComplete != frame.ReceiveStatus)
            {
                throw new Exception("Invalid frame received. Reason: " + frame.ReceiveStatus.ToString());
            }

            //Convert raw frame data into image (for image display)
            Bitmap bitmap = null;
            IntPtr ptrImagePointer = new IntPtr();
            switch (frame.PixelFormat)
            {
                case VmbPixelFormatType.VmbPixelFormatMono8:
                    {
                        bitmap = new Bitmap((int)frame.Width, (int)frame.Height, PixelFormat.Format8bppIndexed);

                        //Set greyscale palette
                        ColorPalette palette = bitmap.Palette;
                        for (int i = 0; i < palette.Entries.Length; i++)
                        {
                            palette.Entries[i] = Color.FromArgb(i, i, i);
                        }
                        bitmap.Palette = palette;

                        //Copy image data
                        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0,
                                                                                    0,
                                                                                    (int)frame.Width,
                                                                                    (int)frame.Height),
                                                                    ImageLockMode.WriteOnly,
                                                                    PixelFormat.Format8bppIndexed);

                        ptrImagePointer = bitmapData.Scan0;

                        try
                        {
                            //Copy image data line by line
                            for (int y = 0; y < (int)frame.Height; y++)
                            {
                                System.Runtime.InteropServices.Marshal.Copy(frame.Buffer,
                                                                                y * (int)frame.Width,
                                                                                new IntPtr(bitmapData.Scan0.ToInt64() + y * bitmapData.Stride),
                                                                                (int)frame.Width);

                            }
                        }
                        finally
                        {
                            bitmap.UnlockBits(bitmapData);
                        }
                    }
                    break;

                case VmbPixelFormatType.VmbPixelFormatBgr8:
                    {
                        bitmap = new Bitmap((int)frame.Width, (int)frame.Height, PixelFormat.Format24bppRgb);

                        //Copy image data
                        BitmapData bitmapData = bitmap.LockBits(new Rectangle(0,
                                                                                    0,
                                                                                    (int)frame.Width,
                                                                                    (int)frame.Height),
                                                                    ImageLockMode.WriteOnly,
                                                                    PixelFormat.Format24bppRgb);
                        try
                        {
                            //Copy image data line by line
                            for (int y = 0; y < (int)frame.Height; y++)
                            {
                                System.Runtime.InteropServices.Marshal.Copy(frame.Buffer,
                                                                                y * ((int)frame.Width) * 3,
                                                                                new IntPtr(bitmapData.Scan0.ToInt64() + y * bitmapData.Stride),
                                                                                ((int)(frame.Width) * 3));
                            }
                        }
                        finally
                        {
                            bitmap.UnlockBits(bitmapData);
                        }
                    }
                    break;

                default:
                    throw new Exception("Current pixel format is not supported by this example (only Mono8 and BRG8Packed are supported).");
            }

            return ptrImagePointer;
        }
        //Adjust pixel format of given camera to match one that can be displayed
        //in this example.
        private void AdjustPixelFormat(Camera camera)
        {
            if(null == camera)
            {
                throw new ArgumentNullException("camera");
            }

            string[] supportedPixelFormats = new string[] { "BGR8Packed", "Mono8" };
            //Check for compatible pixel format
            Feature pixelFormatFeature = camera.Features["PixelFormat"];

            //Determine current pixel format
            string currentPixelFormat = pixelFormatFeature.EnumValue;

            //Check if current pixel format is supported
            bool currentPixelFormatSupported = false;
            foreach(string supportedPixelFormat in supportedPixelFormats)
            {
                if(string.Compare(currentPixelFormat, supportedPixelFormat, StringComparison.Ordinal) == 0)
                {
                    currentPixelFormatSupported = true;
                    break;
                }
            }

            //Only adjust pixel format if we not already have a compatible one.
            if(false == currentPixelFormatSupported)
            {
                //Determine available pixel formats
                string[] availablePixelFormats = pixelFormatFeature.EnumValues;
                    
                //Check if there is a supported pixel format
                bool pixelFormatSet = false;
                foreach(string supportedPixelFormat in supportedPixelFormats)
                {
                    foreach(string availablePixelFormat in availablePixelFormats)
                    {
                        if(     (string.Compare(supportedPixelFormat, availablePixelFormat, StringComparison.Ordinal) == 0)
                            &&  (pixelFormatFeature.IsEnumValueAvailable(supportedPixelFormat) == true))
                        {
                            //Set the found pixel format
                            pixelFormatFeature.EnumValue = supportedPixelFormat;
                            pixelFormatSet = true;
                            break;
                        }
                    }

                    if(true == pixelFormatSet)
                    {
                        break;
                    }
                }

                if(false == pixelFormatSet)
                {
                    throw new Exception("None of the pixel formats that are supported by this example (Mono8 and BRG8Packed) can be set in the camera.");
                }
            }
        }

        private void OnCameraListChange(VmbUpdateTriggerType reason)
        {
            switch(reason)
            {
            case VmbUpdateTriggerType.VmbUpdateTriggerPluggedIn:
            case VmbUpdateTriggerType.VmbUpdateTriggerPluggedOut:
                {
                    CameraListChangedHandler cameraListChangedHandler = m_CameraListChangedHandler;
                    if(null != cameraListChangedHandler)
                    {
                        cameraListChangedHandler(this, EventArgs.Empty);
                    }
                }
                break;

            default:
                break;
            }
        }

        private void OnFrameReceived(Frame frame)
        {
            try
            {
                var ImgPointer = Marshal.AllocHGlobal((int)frame.BufferSize);
                Marshal.Copy(frame.Buffer, 0, ImgPointer, (int)frame.BufferSize);
                m_ptrImagePointer = new IntPtr();
                m_ptrImagePointer = ImgPointer;

                
                //m_ptrImagePointer = ConvertFrameToImagePointer(frame);
                m_bReceivedFrame = true;
            }
            catch(Exception exception)
            {
            }
            finally
            {
                //We make sure to always return the frame to the API
                m_Camera.QueueFrame(frame);
            }
        }

        private void OnFrameReceived_Flip(Frame frame)
        {
            try
            {
                // ------- Vertical Flip ------------------------

                int width = 640;
                int height = 480;
                byte[] Img = new byte[640 * 480];

                unsafe
                {
                    for (int i = height - 1; i >= 0; i--)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            Img[i * width + width - 1 - j] = frame.Buffer[(height - 1 - i) * width + j];
                        }
                    }

                    // Img = 479 * 640 + 640 - 1 - 0 = 306560 + 639
                    // Img = 479 * 640 + 640 - 1 - 1 = 306560 + 638
                    // ....Img = 479 * 640 + 640 - 1 - 639 = 306560 + 0

                    // Img = 478 * 640 + 640 - 1 - 0 = 306560 + 639
                    // Img = 478 * 640 + 640 - 1 - 1 = 306560 + 638
                    // ....Img = 478 * 640 + 640 - 1 - 639 = 306560 + 0


                    // buffer = 480 - 1 - 479 = 0 * 640 + 0 = 0 
                    // buffer = 480 - 1 - 479 = 0 * 640 + 1 = 1; 
                    //  .....  639

                    // buffer = 480 - 1 - 478 = 1 * 640 + 0 = 640
                    // buffer = 480 - 1 - 479 = 0 * 640 + 1 = 641; 
                    //  .....  1279
                }

                var ImgPointer = Marshal.AllocHGlobal((int)frame.BufferSize);
                Marshal.Copy(Img, 0, ImgPointer, (int)frame.BufferSize);
                m_ptrImagePointer = new IntPtr();
                m_ptrImagePointer = ImgPointer;

                m_bReceivedFrame = true;
            }
            catch (Exception exception)
            {
            }
            finally
            {
                //We make sure to always return the frame to the API
                m_Camera.QueueFrame(frame);
            }
        }
        public void ReleaseImage()
        {
            Marshal.FreeHGlobal(m_ptrImagePointer);
        }

        //Release Camera
        private void ReleaseCamera()
        {
            if(null != m_Camera)
            {
                //We can use cascaded try-finally blocks to release the
                //camera step by step to make sure that every step is executed.
                try
                {
                    try
                    {
                        try
                        {
                            if(null != m_FrameReceivedHandler)
                            {
                                m_Camera.OnFrameReceived -= this.OnFrameReceived;
                            }
                        }
                        finally
                        {
                            m_FrameReceivedHandler = null;
                            if(true == m_Acquiring)
                            {
                                m_Camera.StopContinuousImageAcquisition();
                            }
                        }
                    }
                    finally
                    {
                        m_Acquiring = false;
                        m_Camera.Close();
                    }
                }
                finally
                {
                    m_Camera = null;
                }
            }
        }

        //Release Vimba API
        private void ReleaseVimba()
        {
            if(null != m_Vimba)
            {
                //We can use cascaded try-finally blocks to release the
                //Vimba API step by step to make sure that every step is executed.
                try
                {
                    try
                    {
                        try
                        {
                            //First we release the camera (if there is one)
                            ReleaseCamera();
                        }
                        finally
                        {
                            if (null != m_CameraListChangedHandler)
                            {
                                m_Vimba.OnCameraListChanged -= this.OnCameraListChange;
                            }
                        }
                    }
                    finally
                    {
                        //Now finally shutdown the API
                        m_CameraListChangedHandler = null;
                        try
                        {
                            m_Vimba.Shutdown();
                        }
                        catch { }
                    }
                }
                finally
                {
                    m_Vimba = null;
                }
            }
        }

        //Start up Vimba API
        public void Startup(CameraListChangedHandler cameraListChangedHandler)
        {
            //Instanciate main Vimba object
            Vimba vimba = new Vimba();

            //Start up Vimba API
            vimba.Startup();
            m_Vimba = vimba;

            bool bError = true;
            try
            {
                //Register camera list change delegate
                if(null != cameraListChangedHandler)
                {
                    m_Vimba.OnCameraListChanged += this.OnCameraListChange;
                    m_CameraListChangedHandler = cameraListChangedHandler;
                }

                bError = false;
            }
            finally
            {
                //Release Vimba API if an error occured
                if(true == bError)
                {
                    ReleaseVimba();
                }
            }
        }

        public void Startup()
        {
            //Instanciate main Vimba object
            Vimba vimba = new Vimba();

            //Start up Vimba API
            vimba.Startup();
            m_Vimba = vimba;

            bool bError = true;
            try
            {
                bError = false;
            }
            finally
            {
                //Release Vimba API if an error occured
                if (true == bError)
                {
                    ReleaseVimba();
                }
            }
        }

        //Shutdown API
        public void Shutdown()
        {
            //Check if API has been started up at all
            if(null == m_Vimba)
            {
                return;
            }

            ReleaseVimba();

        }
        public String GetVersion()
        {
            if (null == m_Vimba)
            {
                throw new Exception("Vimba has not been started.");
            }
            VmbVersionInfo_t version_info = m_Vimba.Version;
            return String.Format("{0:D}.{1:D}.{2:D}",version_info.major,version_info.minor,version_info.patch);
        }
        //Property to get the current camera list
        public List<CameraInfo> CameraList
        {
            get
            {
                //Check if API has been started up at all
                if(null == m_Vimba)
                {
                    throw new Exception("Vimba is not started.");
                }

                List<CameraInfo> cameraList = new List<CameraInfo>();
                CameraCollection cameras = m_Vimba.Cameras;
                foreach(Camera camera in cameras)
                {
                    cameraList.Add(new CameraInfo(camera.Name, camera.Id, camera.Model, camera.SerialNumber));
                }

                return cameraList;
            }
        }

        public void StartContinuousImageAcquisition(string id, FrameReceivedHandler frameReceivedHandler)
        {
            //Check parameters
            if(null == id)
            {
                throw new ArgumentNullException("id");
            }

            //Check if API has been started up at all
            if(null == m_Vimba)
            {
                throw new Exception("Vimba is not started.");
            }

            //Check if a camera is already open
            if(null != m_Camera)
            {
                throw new Exception("A camera is already open.");
            }

            //Open camera
            m_Camera = m_Vimba.OpenCameraByID(id, VmbAccessModeType.VmbAccessModeFull);
            if(null == m_Camera)
            {
                throw new NullReferenceException("No camera retrieved.");
            }

            // Set the GeV packet size to the highest possible value
            // (In this example we do not test whether this cam actually is a GigE cam)
            try
            {
                m_Camera.Features["GVSPAdjustPacketSize"].RunCommand();
                while (false == m_Camera.Features["GVSPAdjustPacketSize"].IsCommandDone()) {}
            }
            catch {}

            bool bError = true;
            try
            {
                //Set a compatible pixel format
                AdjustPixelFormat(m_Camera);

                //Register frame callback
                if(null != frameReceivedHandler)
                {
                    m_Camera.OnFrameReceived += this.OnFrameReceived;
                    m_FrameReceivedHandler = frameReceivedHandler;
                }

                //Reset member variables
                m_RingBitmap = new RingBitmap(m_RingBitmapSize);
                m_ImageInUse = true;
                m_Acquiring = true;

                //Start synchronous image acquisition (grab)
                m_Camera.StartContinuousImageAcquisition(3);
                
                bError = false;
            }
            finally
            {
                //Close camera already if there was an error
                if(true == bError)
                {
                    ReleaseCamera();
                }
            }
        }

        public void StopContinuousImageAcquisition()
        {
            //Check if API has been started up at all
            if(null == m_Vimba)
            {
                throw new Exception("Vimba is not started.");
            }

            //Check if no camera is open
            if(null == m_Camera)
            {
                throw new Exception("No camera open.");
            }

            //Close camera
            ReleaseCamera();
        }

        // -- Single Grab Mode -------------------------

         public void InitCamera_F033B(string id, bool blnFlip)
        {
            //Check parameters
            if (null == id)
            {
                throw new ArgumentNullException("id");
            }

            //Check if API has been started up at all
            if (null == m_Vimba)
            {
                throw new Exception("Vimba is not started.");
            }

            //Check if a camera is already open
            if (null != m_Camera)
            {
                throw new Exception("A camera is already open.");
            }

            //Open camera
            m_Camera = m_Vimba.OpenCameraByID(id, VmbAccessModeType.VmbAccessModeFull);
            if (null == m_Camera)
            {
                throw new NullReferenceException("No camera retrieved.");
            }

            try
            {
                //Set a compatible pixel format
                AdjustPixelFormat(m_Camera);

                //Register frame callback
                if (blnFlip)
                    m_Camera.OnFrameReceived += this.OnFrameReceived_Flip;
                else
                    m_Camera.OnFrameReceived += this.OnFrameReceived;

                ExternalTriggerMode(false);
                SetFrameRateToMax();
                ExternalTriggerMode(true);   // Must off exernal trigger first before set other feature.
                EnableSoftwareTrigger();
                DisableAutoGain();
                DisableAutoExposure();
                m_Camera.Features["AcquisitionMode"].EnumValue = "Continuous";
                EnableGamma();
                SetExposure(2000);

                GetReadyFrame();

                m_blnCameraInitDone = true;
            }
            catch
            {
                ReleaseCamera();
            }
        }

        public void InitCamera_F033B_2(string id, bool blnFlip, ref Bitmap bmp)
        {
            //Check parameters
            if (null == id)
            {
                throw new ArgumentNullException("id");
            }

            //Check if API has been started up at all
            if (null == m_Vimba)
            {
                throw new Exception("Vimba is not started.");
            }

            //Check if a camera is already open
            if (null != m_Camera)
            {
                throw new Exception("A camera is already open.");
            }

            //Open camera
            m_Camera = m_Vimba.OpenCameraByID(id, VmbAccessModeType.VmbAccessModeFull);

            if (null == m_Camera)
            {
                throw new NullReferenceException("No camera retrieved.");
            }

            try
            {


                //Set a compatible pixel format
                //AdjustPixelFormat(m_Camera);

                //Register frame callback
                //if (blnFlip)
                //    m_Camera.OnFrameReceived += this.OnFrameReceived_Flip;
                //else
                //    m_Camera.OnFrameReceived += this.OnFrameReceived;

                //ExternalTriggerMode(false);
                //SetFrameRateToMax();
                //ExternalTriggerMode(true);   // Must off exernal trigger first before set other feature.
                //EnableSoftwareTrigger();
                //DisableAutoGain();
                //DisableAutoExposure();
                //m_Camera.Features["AcquisitionMode"].EnumValue = "Continuous";
                //EnableGamma();
                //SetExposure(2000);

                //GetReadyFrame();

                // -----------------------------------------------------
                m_Camera.Features["TriggerMode"].EnumValue = "On";
                m_Camera.Features["TriggerSource"].EnumValue = "Software";
                long width = m_Camera.Features["Width"].IntValue;
                long height = m_Camera.Features["Height"].IntValue;
                STTrackLog.WriteLine("Width=" + width.ToString() + ", Height=" + height.ToString());
                bmp = new Bitmap((int)width, (int)height, PixelFormat.Format8bppIndexed);
                ColorPalette mono = bmp.Palette;
                for (int i = 0; i < 256; i++)
                {
                    mono.Entries[i] = Color.FromArgb(i, i, i);
                }
                bmp.Palette = mono;

                m_Camera.OnFrameReceived += new Camera.OnFrameReceivedHandler(OnFrameReceived);
                long payload = m_Camera.Features["PayloadSize"].IntValue;
                Frame[] frameArray = new Frame[3];
                for (int index = 0; index < frameArray.Length; ++index)
                {
                    frameArray[index] = new Frame(payload);
                    m_Camera.AnnounceFrame(frameArray[index]);
                }
                m_Camera.StartCapture();

                for (int index = 0; index < frameArray.Length; ++index)
                {
                    m_Camera.QueueFrame(frameArray[index]);
                }

                m_Camera.Features["AcquisitionStart"].RunCommand();

                m_blnCameraInitDone = true;
            }
            catch
            {
                ReleaseCamera();
            }
        }

        private void GetReadyFrame()
        {
            Frame[] frameArray = new Frame[3];
            long payloadSize = m_Camera.Features["PayloadSize"].IntValue;
            for (int index = 0; index < frameArray.Length; ++index)
            {
                frameArray[index] = new Frame(payloadSize);
                m_Camera.AnnounceFrame(frameArray[index]);
            }

            m_Camera.StartCapture();

            for (int index = 0; index < frameArray.Length; ++index)
            {
                m_Camera.QueueFrame(frameArray[index]);
            }

            m_Camera.Features["AcquisitionStart"].RunCommand();

            System.Threading.Thread.Sleep(50);
        }

        public bool StartFreeRun()
        {
            //Start synchronous image acquisition (grab)
            m_Camera.StartContinuousImageAcquisition(3);

            return true;
        }

        public bool Grab()
        {
            m_bReceivedFrame = false;
            m_Camera.Features["TriggerSoftware"].RunCommand();

            return true;            
        }

        public double GetExposure()
        {
            if (!m_blnCameraInitDone)
                return 0;

            return m_Camera.Features["ExposureTime"].FloatValue;
        }

        public double GetFrameRate()
        {
            if (!m_blnCameraInitDone)
                return 0;

            return m_Camera.Features["AcquisitionFrameRate"].FloatValue;
        }

        public double GetGain()
        {
            if (!m_blnCameraInitDone)
                return 0;

            return m_Camera.Features["Gain"].FloatValue;
        }

        public bool SetExposure(double dExposure)
        {
            try
            {
                m_Camera.Features["ExposureTime"].FloatValue = dExposure;
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool SetFrameRate(double dFrameRate)
        {
            try
            {
                m_Camera.Features["AcquisitionFrameRate"].FloatValue = dFrameRate;
            }
            catch
            {
                return false;
            }

            return true;
            
        }

        public bool SetGain(double dGain)
        {
            try
            {
                dGain = Math.Round(dGain * 0.46, 0, MidpointRounding.AwayFromZero); // 2018 08 09 - JBTAN: 0.46 because max dGain value is now 50

                if (dGain > 23)
                    dGain = 23;

                m_Camera.Features["Gain"].FloatValue = dGain;
            }
            catch
            {
                return false;
            }

            return true;
            
        }

        public bool SetFrameRateToMax()
        {
            try
            {
                double fRangeMax = m_Camera.Features["AcquisitionFrameRate"].FloatRangeMax;
                m_Camera.Features["AcquisitionFrameRate"].FloatValue = fRangeMax;
            }
            catch
            {
                return false;
            }

            return true;

        }
        
        public void DisableAutoExposure()
        {
            m_Camera.Features["ExposureAuto"].EnumValue = "Off";
        }

        public void DisableAutoGain()
        {
            m_Camera.Features["GainAuto"].EnumValue = "Off";
        }

        public void EnableGamma()
        {
            m_Camera.Features["Gamma"].FloatValue = 1;
        }

        public void EnableSoftwareTrigger()
        {
            m_Camera.Features["TriggerSource"].EnumValue = "Software";
        }

        public void ExternalTriggerMode(bool blnON)
        {
            if (blnON)
                m_Camera.Features["TriggerMode"].EnumValue = "On";
            else
                m_Camera.Features["TriggerMode"].EnumValue = "Off";
        }

        public bool OutPort(int intOutputPin, bool blnEnablePort, bool blnInvert)
        {
            try
            {                
                SelectOutputPin(intOutputPin);
                EnableSelectedPort(blnEnablePort);
                InvertPortSignal(blnInvert);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool OutPort(int intOutputPin, bool blnEnablePort)
        {
            try
            {
                SelectOutputPin(intOutputPin);
                EnableSelectedPort(blnEnablePort);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool OutPortDirect(int intOutputPin, bool blnLineStatus)
        {
            try
            {
                SelectOutputPin(intOutputPin);

                if (m_strLineSourcePrev[intOutputPin] != "Direct")
                {
                    m_Camera.Features["LineSource"].EnumValue = "Direct";
                    m_Camera.Features["LineStatus"].BoolValue = blnLineStatus;  // Not need to check same value or not here. 

                    m_strLineSourcePrev[intOutputPin] = "Direct";
                    m_strLineStatusPrev[intOutputPin] = blnLineStatus;
                }
                else
                {
                    if (m_strLineStatusPrev[intOutputPin] != blnLineStatus)
                    {
                        m_Camera.Features["LineStatus"].BoolValue = blnLineStatus;

                        m_strLineStatusPrev[intOutputPin] = blnLineStatus;
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public bool InvertPort(int intOutputPin, bool blnLineStatus)
        {
            try
            {
                SelectOutputPin(intOutputPin);
                InvertPortSignal(false);
            }
            catch
            {
                return false;
            }

            return true;
        }


        public void SelectOutputPin(int intOutputPin)
        {
            if (m_intOutputPinPrev == intOutputPin)
                return;

            switch (intOutputPin)
            {
                default:
                case 0:
                    m_Camera.Features["LineSelector"].EnumValue = "Line4";
                    break;
                case 1: 
                    m_Camera.Features["LineSelector"].EnumValue = "Line5";  
                    break;
                case 2:
                    m_Camera.Features["LineSelector"].EnumValue = "Line6";
                    break;
            }

            m_intOutputPinPrev = intOutputPin;
        }

        public void EnableSelectedPort(bool blnEnablePort)
        {
            if (blnEnablePort)
            {
                m_Camera.Features["LineSource"].EnumValue = "ExposureActive";
            }
            else
            {
                m_Camera.Features["LineSource"].EnumValue = "Off";
            }         
        }

        public void InvertPortSignal(bool blnInvert)
        {
            if (blnInvert)
            {
                m_Camera.Features["LineInverter"].BoolValue = true;
            }
            else
            {
                m_Camera.Features["LineInverter"].BoolValue = false;
            }
        }

        public long GetCameraConnectedPortNo(int intCameraIndex)
        {
            try
            {
                m_Vimba.Cameras[intCameraIndex].Open(VmbAccessModeType.VmbAccessModeRead);
                long intCameraPortNo = m_Vimba.Cameras[intCameraIndex].Features["IIDCBusNumber"].IntValue;
                //t.WriteLine("try in get port 1");
                //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["TriggerSelector"].StringValue.ToString());
                //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["TriggerSource"].EnumValue.ToString());
                //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["TriggerMode"].EnumValue.ToString());
                //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["AcquisitionMode"].EnumValue.ToString());
                m_Vimba.Cameras[intCameraIndex].Close();
                return intCameraPortNo;
            }
            catch
            {
                return -1;
            }
        }
        public long GetCameraConnectedPortNo2(int intCameraIndex, ref bool newCamera, ref string serial)
        {
            long intCameraPortNo;
            try
            {
               
                m_Vimba.Cameras[intCameraIndex].Open(VmbAccessModeType.VmbAccessModeRead);
                //t.WriteLine("try in get port 2");
                //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["TriggerSelector"].StringValue.ToString());
                //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["TriggerSource"].EnumValue.ToString());
                //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["TriggerMode"].EnumValue.ToString());
                //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["AcquisitionMode"].EnumValue.ToString());
                intCameraPortNo = m_Vimba.Cameras[intCameraIndex].Features["IIDCBusNumber"].IntValue;
                serial = m_Vimba.Cameras[intCameraIndex].SerialNumber;
                m_Vimba.Cameras[intCameraIndex].Close();
                newCamera = true;
                return intCameraPortNo;
            }
            catch
            {

                try
                {
                    //t.WriteLine("try in catch in get port 2");
                    m_Vimba.Cameras[intCameraIndex].Features["AcquisitionStop"].RunCommand();
                   // t.WriteLine("After AcquisitionStop");
                    m_Vimba.Cameras[intCameraIndex].EndCapture();
                  //  t.WriteLine("After EndCapture");
                    Frame[] frameArray = new Frame[3];
                    long payloadSize = m_Vimba.Cameras[intCameraIndex].Features["PayloadSize"].IntValue;
                   // t.WriteLine("After PayloadSize");
                    for (int index = 0; index < frameArray.Length; ++index)
                    {
                        frameArray[index] = new Frame(payloadSize);
                        m_Vimba.Cameras[intCameraIndex].AnnounceFrame(frameArray[index]);
                    }
                  //  t.WriteLine("After AnnounceFrame");
                    m_Vimba.Cameras[intCameraIndex].StartCapture();
                  //  t.WriteLine("After StartCapture");
                    for (int index = 0; index < frameArray.Length; ++index)
                    {
                        m_Vimba.Cameras[intCameraIndex].QueueFrame(frameArray[index]);
                    }
                //    t.WriteLine("After QueueFrame");
                    m_Vimba.Cameras[intCameraIndex].Features["AcquisitionStart"].RunCommand();
                 //   t.WriteLine("After AcquisitionStart");
                    newCamera = false;
                    //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["TriggerSelector"].StringValue.ToString());
                    //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["TriggerSource"].EnumValue.ToString());
                    //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["TriggerMode"].EnumValue.ToString());
                    //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["AcquisitionMode"].EnumValue.ToString());
                    serial = m_Vimba.Cameras[intCameraIndex].SerialNumber;
                    intCameraPortNo = m_Vimba.Cameras[intCameraIndex].Features["IIDCBusNumber"].IntValue;
                }
                catch
                {
                 //   t.WriteLine("catch in catch in get port 2");
                    if(m_Vimba!=null)
                    Shutdown();
                    Startup();

                    m_Vimba.Cameras[intCameraIndex].Open(VmbAccessModeType.VmbAccessModeRead);
                    newCamera = true;
                    //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["TriggerSelector"].StringValue.ToString());
                    //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["TriggerSource"].EnumValue.ToString());
                    //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["TriggerMode"].EnumValue.ToString());
                    //t.WriteLine(m_Vimba.Cameras[intCameraIndex].Features["AcquisitionMode"].EnumValue.ToString());
                    serial = m_Vimba.Cameras[intCameraIndex].SerialNumber;
                    intCameraPortNo = m_Vimba.Cameras[intCameraIndex].Features["IIDCBusNumber"].IntValue;
                    m_Vimba.Cameras[intCameraIndex].Close();
                }
               // t.WriteLine("catch in get port 2");
               
                
                //newCamera = false;
                return intCameraPortNo;
            }
        }
        public long GetCameraConnectedPortNo(string id)
        {
            //Check if API has been started up at all
            if (null == m_Vimba)
            {
                throw new Exception("Vimba is not started.");
            }

            //Check if a camera is already open
            if (null != m_Camera)
            {
                throw new Exception("A camera is already open.");
            }

            for (int i = 0; i < m_Vimba.Cameras.Count; i++)
            {
                long pp = m_Vimba.Cameras[i].Features["IIDCBusNumber"].IntValue;
                ////Open camera
                //m_Camera = m_Vimba.OpenCameraByID(id, VmbAccessModeType.VmbAccessModeFull);
                //if (null == m_Camera)
                //{
                //    throw new NullReferenceException("No camera retrieved.");
                //}
            }

            long intPortNo = m_Camera.Features["IIDCBusNumber"].IntValue;

            m_Camera.Close();
            m_Camera = null;

            return intPortNo;
        }

        public Bitmap GetBitmapFrame()
        {
            
            return m_bmpImage;
        }

        public bool IsReceivedFrame()
        {
            return m_bReceivedFrame;
        }

        public IntPtr ImagePointer
        {
            get {
                return m_ptrImagePointer;
            }

            set
            {
                m_ptrImagePointer = value;
            }
            
        }
    }
}
