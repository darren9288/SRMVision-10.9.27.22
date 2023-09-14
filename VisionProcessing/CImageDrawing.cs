using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Common;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif

namespace VisionProcessing
{
    public class CImageDrawing
    {
        #region Constant Variables

        private const int PICBOX_WIDTH = 640;
        private const int PICBOX_HEIGHT = 480;

        #endregion

        #region Member Variables

        private int m_intImageWidth = 0;
        private int m_intImageHeight = 0;
        private int m_intCameraResolutionWidth = PICBOX_WIDTH;    // camera resolution
        private int m_intCameraResolutionHeight = PICBOX_HEIGHT;  // camera resolution


        private float m_fScale = 1.0f;
        private float m_fDrawingScaleX = 1.0f;
        private float m_fDrawingScaleY = 1.0f;

        private int m_intResolutionWidth = 640;
        private int m_intResolutionHeight = 480;
        private int m_intPictureBoxWidth = 640;
        private int m_intPictureBoxHeight = 480;

        private EROIC24 m_objZoomROI = new EROIC24();
        private EImageC24 m_objMainCImage = new EImageC24(640, 480);
        private EImageC24 m_objZoomImage = new EImageC24(640, 480);
        private EImageC24 m_objMergeImage = null;
        private EImageBW8 m_objThresholdImage = new EImageBW8(640, 480);

        private EColorLookup m_objColorLookupLSH = new EColorLookup();
        private EColorLookup m_objColorLookupYSH = new EColorLookup();
        private EColorLookup m_objColorLookupRGB = new EColorLookup();
        #endregion

        #region Properties

        public EImageC24 ref_objMainCImage { get { return m_objMainCImage; } set { m_objMainCImage = value; } }
        public int ref_intImageWidth { get { return m_objMainCImage.Width; } }
        public int ref_intImageHeight { get { return m_objMainCImage.Height; } }
        public int ref_intCameraResolutionWidth { get { return m_intCameraResolutionWidth; } }
        public int ref_intCameraResolutionHeight { get { return m_intCameraResolutionHeight; } }
        public float ref_fDrawingScaleX { get { return m_fDrawingScaleX; } set { m_fDrawingScaleX = value; } }
        public float ref_fDrawingScaleY { get { return m_fDrawingScaleY; } set { m_fDrawingScaleY = value; } }
        #endregion

        public CImageDrawing()
        {
            //Easy.Initialize();
            if (m_objColorLookupYSH.ColorSystemOut != EColorSystem.Ysh)
                m_objColorLookupYSH.ConvertFromRgb(EColorSystem.Ysh);

            if (m_objColorLookupRGB.ColorSystemOut != EColorSystem.Rgb)
                m_objColorLookupRGB.ConvertFromRgb(EColorSystem.Rgb);

            if (m_objColorLookupLSH.ColorSystemOut != EColorSystem.Lsh)
                m_objColorLookupLSH.ConvertFromRgb(EColorSystem.Lsh);
        }
        public CImageDrawing(int intImageWidth, int intImageHeight)
        {
            //Easy.Initialize();

            m_intCameraResolutionWidth = intImageWidth;
            m_intCameraResolutionHeight = intImageHeight;

            m_objMainCImage = new EImageC24(intImageWidth, intImageHeight);
            m_objMergeImage = new EImageC24(intImageWidth, intImageHeight);
            m_objZoomImage = new EImageC24(intImageWidth, intImageHeight);
            m_objThresholdImage = new EImageBW8(intImageWidth, intImageHeight);
            if (m_objColorLookupYSH.ColorSystemOut != EColorSystem.Ysh)
                m_objColorLookupYSH.ConvertFromRgb(EColorSystem.Ysh);

            if (m_objColorLookupRGB.ColorSystemOut != EColorSystem.Rgb)
                m_objColorLookupRGB.ConvertFromRgb(EColorSystem.Rgb);

            if (m_objColorLookupLSH.ColorSystemOut != EColorSystem.Lsh)
                m_objColorLookupLSH.ConvertFromRgb(EColorSystem.Lsh);
        }
        public CImageDrawing(bool blnBasic)
        {
            //Easy.Initialize();

            if (blnBasic)
            {
                m_objMainCImage = new EImageC24(640, 480);
            }
            else
            {
                m_objZoomROI = new EROIC24();
                m_objMainCImage = new EImageC24(640, 480);
                m_objZoomImage = new EImageC24(640, 480);
                m_objThresholdImage = new EImageBW8(640, 480);
            }
            if (m_objColorLookupYSH.ColorSystemOut != EColorSystem.Ysh)
                m_objColorLookupYSH.ConvertFromRgb(EColorSystem.Ysh);

            if (m_objColorLookupRGB.ColorSystemOut != EColorSystem.Rgb)
                m_objColorLookupRGB.ConvertFromRgb(EColorSystem.Rgb);

            if (m_objColorLookupLSH.ColorSystemOut != EColorSystem.Lsh)
                m_objColorLookupLSH.ConvertFromRgb(EColorSystem.Lsh);
        }
        public CImageDrawing(bool blnBasic, int intImageWidth, int intImageHeight)
        {
            //Easy.Initialize();

            m_intCameraResolutionWidth = intImageWidth;
            m_intCameraResolutionHeight = intImageHeight;

            if (blnBasic)
            {
                m_objMainCImage = new EImageC24(intImageWidth, intImageHeight);
            }
            else
            {
                m_objMainCImage = new EImageC24(intImageWidth, intImageHeight);
                m_objMergeImage = new EImageC24(intImageWidth, intImageHeight);
                m_objZoomImage = new EImageC24(intImageWidth, intImageHeight);
                m_objThresholdImage = new EImageBW8(intImageWidth, intImageHeight);
            }
            if (m_objColorLookupYSH.ColorSystemOut != EColorSystem.Ysh)
                m_objColorLookupYSH.ConvertFromRgb(EColorSystem.Ysh);

            if (m_objColorLookupRGB.ColorSystemOut != EColorSystem.Rgb)
                m_objColorLookupRGB.ConvertFromRgb(EColorSystem.Rgb);

            if (m_objColorLookupLSH.ColorSystemOut != EColorSystem.Lsh)
                m_objColorLookupLSH.ConvertFromRgb(EColorSystem.Lsh);
        }

        ~CImageDrawing()
        {
            //Easy.Terminate();
        }

        public void Dispose()
        {
            if (m_objZoomROI != null)
                m_objZoomROI.Dispose();

            if (m_objMainCImage != null)
                m_objMainCImage.Dispose();

            if (m_objZoomImage != null)
                m_objZoomImage.Dispose();

            if (m_objThresholdImage != null)
                m_objThresholdImage.Dispose();

            if (m_objMergeImage != null)
                m_objMergeImage.Dispose();
        }
        public void AddGain(float fGain, int intColorFormat)
        {
            if (fGain != 1f)
            {
                if (intColorFormat == 1)
                {
                    m_objColorLookupRGB.AdjustGainOffset(EColorSystem.Rgb, fGain, 0.00f, fGain, 0.00f, fGain, 0.00f);
                    m_objColorLookupRGB.Transform(m_objMainCImage, m_objMainCImage);
                }
                else if (intColorFormat == 0)
                {
                    m_objColorLookupLSH.AdjustGainOffset(EColorSystem.Rgb, fGain, 0.00f, fGain, 0.00f, fGain, 0.00f);
                    m_objColorLookupLSH.Transform(m_objMainCImage, m_objMainCImage);
                }
                else
                {
                    m_objColorLookupYSH.AdjustGainOffset(EColorSystem.Rgb, fGain, 0.00f, fGain, 0.00f, fGain, 0.00f);
                    m_objColorLookupYSH.Transform(m_objMainCImage, m_objMainCImage);
                }
                
            }
        }

        /// <summary>
        /// Rotate image to less than 90 degree of rotation angle
        /// </summary>
        /// <param name="objSourceImage">source image</param>
        /// <param name="fRotateAngle">rotation angle</param>
        /// <param name="objRotatedImage">destination rotated image</param>
        public static void Rotate0Degree(CImageDrawing objSourceImage, float fRotateAngle, CImageDrawing objRotatedImage)
        {
            /*
             * Note*
             *      - Color image wont blur when use same object of source image and destination image.
             *      - Beside, without ref, the calling function still can get the rotated image.
             */

            float fCenterX = objSourceImage.ref_intImageWidth / 2f;
            float fCenterY = objSourceImage.ref_intImageHeight / 2f;

            EasyImage.ScaleRotate(objSourceImage.ref_objMainCImage, fCenterX, fCenterY, fCenterX, fCenterY, 1, 1, fRotateAngle, objRotatedImage.ref_objMainCImage, 4);
        }




        /// <summary>
        /// Convert rgb color value to lsh color value
        /// </summary>
        /// <param name="intHitX">mouse hit X value</param>
        /// <param name="intHitY">mouse hit Y value</param>
        /// <returns>lsh value in float array</returns>
        public int[] ConvertRGBToLSH(int intHitX, int intHitY)
        {
            EC24 objColor = new EC24();

            objColor = m_objMainCImage.GetPixel(intHitX, intHitY);

            return ColorProcessing.ConvertRGBToLSH(objColor.C0, objColor.C1, objColor.C2);
        }

        public int[] GetRGBPixelValue(int intHitX, int intHitY)
        {
            EC24 objColor = new EC24();

            if (intHitX >= 0 && intHitX < m_objMainCImage.Width && intHitY >= 0 && intHitY < m_objMainCImage.Height)
            {
                objColor = m_objMainCImage.GetPixel(intHitX, intHitY);
            }
            int[] intRGB = new int[3];
            intRGB[0] = objColor.C0;
            intRGB[1] = objColor.C1;
            intRGB[2] = objColor.C2;

            return intRGB;
        }

        public void ConvertColorToMono(ref List<ImageDrawing> arrBinaryImage, int intImageIndex)
        {
            if (intImageIndex >= arrBinaryImage.Count)
                return;

            arrBinaryImage[intImageIndex].SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);
            EasyColor.GetComponent(m_objMainCImage, arrBinaryImage[intImageIndex].ref_objMainImage, 0);
            //EasyImage.Convert(m_objMainCImage, arrBinaryImage[intImageIndex].ref_objMainImage);
        }
        public void ConvertColorToMono(ref List<List<ImageDrawing>> arrBinaryImage, int intImageIndex, int intIndex)
        {
            if (intIndex >= arrBinaryImage.Count)
                return;

            if (intImageIndex >= arrBinaryImage[intIndex].Count)
                return;

            arrBinaryImage[intIndex][intImageIndex].SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);
            //EasyImage.Convert(m_objMainCImage, arrBinaryImage[intIndex][intImageIndex].ref_objMainImage);
            EasyColor.GetComponent(m_objMainCImage, arrBinaryImage[intIndex][intImageIndex].ref_objMainImage, 0);
        }

        public void ConvertColorToMono(ref ImageDrawing objBinaryImage)
        {
            objBinaryImage.SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);
            //EasyImage.Convert(m_objMainCImage, objBinaryImage.ref_objMainImage);
            EasyColor.GetComponent(m_objMainCImage, objBinaryImage.ref_objMainImage, 0);
        }

        public void ConvertColorToMono(ImageDrawing objBinaryImage)
        {
            objBinaryImage.SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);
            //EasyImage.Convert(m_objMainCImage, objBinaryImage.ref_objMainImage);
            EasyColor.GetComponent(m_objMainCImage, objBinaryImage.ref_objMainImage, 0);
        }


        /// <summary>
        /// Copy color source image to color destination image by resizing the image
        /// </summary>
        /// <param name="objCImageDest">color destination image to be drawn</param>
        public void CopyTo(ref CImageDrawing objCImageDest)
        {
            objCImageDest.SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);
            EasyImage.Copy(m_objMainCImage, objCImageDest.ref_objMainCImage);
        }

        public void CopyTo(ref List<CImageDrawing> arrImageDest, int intImageIndex)
        {
            arrImageDest[intImageIndex].SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);
            EasyImage.Copy(m_objMainCImage, arrImageDest[intImageIndex].ref_objMainCImage);
        }

        /// <summary>
        /// Copy color source image to color destination image by resizing the image
        /// </summary>
        /// <param name="objCImageDest">color destination image to be drawn</param>
        public void CopyTo(CImageDrawing objCImageDest)
        {
            objCImageDest.SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);
            EasyImage.Copy(m_objMainCImage, objCImageDest.ref_objMainCImage);
        }


        /// <summary>
        /// Draw zoomed image according to given scale
        /// After zooming the image, reset the scale to 1 as normal
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        /// <param name="fZoom">Scale to Zoom image for both X and Y</param>
        public void DrawZoomImage(Graphics g, float fZoom)
        {
            DrawZoomImage(g, fZoom, 0, 0);

            m_fScale = 1.0f;
        }

        /// <summary>
        /// Draw zoomed image according to given scale
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        /// <param name="fZoom">Scale to Zoom image for both X and Y</param>
        /// <param name="intHScrollBar">translation factor for panning in the horizontal direction</param>
        /// <param name="intVScrollBar">translation factor for panning in the vertical direction</param>
        public void DrawZoomImage(Graphics g, float fZoom, int intHScrollBar, int intVScrollBar)
        {
            m_objZoomImage = new EImageC24(Convert.ToInt32(m_objMainCImage.Width * fZoom), Convert.ToInt32(m_objMainCImage.Height * fZoom));
            m_objZoomROI = new EROIC24();

            EasyImage.ScaleRotate(m_objMainCImage, (m_objMainCImage.Width) / 2, (m_objMainCImage.Height) / 2,
                (m_objZoomImage.Width) / 2, (m_objZoomImage.Height) / 2, fZoom, fZoom, 0.0f, m_objZoomImage);
            m_objZoomROI.Detach();
            m_objZoomROI.Attach(m_objZoomImage);
            m_objZoomROI.SetPlacement(intHScrollBar, intVScrollBar, (m_objMainCImage.Width), (m_objMainCImage.Height));

            m_objZoomROI.Draw(g);
            m_fScale = fZoom;
        }

        /// <summary>
        /// Load Image to picture box from selected path
        /// </summary>
        /// <param name="strPath">selected path</param>
        /// <param name="g">destination to draw the image</param>
        public void LoadImage(string strPath)
        {
            m_objMainCImage.Load(strPath);
        }

        /// <summary>
        /// Load color Image to picture box from selected path
        /// </summary>
        /// <param name="strPath">selected path</param>
        /// <param name="intImageWidth">image width</param>
        /// <param name="intImageHeight">image height</param>
        public void LoadImage(string strPath, int intImageWidth, int intImageHeight)
        {
            // Set roi to center location if image smaller than picture box size
            if (intImageWidth < m_intPictureBoxWidth && intImageHeight < m_intPictureBoxHeight)
            {
                m_objMainCImage = new EImageC24(m_intPictureBoxWidth, m_intPictureBoxHeight);
                CROI objCROI = new CROI();
                objCROI.LoadROISetting((m_intPictureBoxWidth / 2) - (intImageWidth / 2), (m_intPictureBoxHeight / 2) - (intImageHeight / 2),
                    intImageWidth, intImageHeight);
                objCROI.AttachImage(this);
                objCROI.LoadImage(strPath, true);
            }
            else
                m_objMainCImage.Load(strPath);
        }

        /// <summary>
        /// Load Image to Picture Box from CPU memory address
        /// </summary>
        /// <param name="intImageAddress">CPU memory address</param>
        /// <param name="g">destination to draw the image</param>
        public void LoadImageFromMemory(IntPtr intImageAddress)
        {
            m_objMainCImage.SetImagePtr(m_objMainCImage.Width, m_objMainCImage.Height, intImageAddress);
        }
        public void LoadImageFromMemoryAndCopyToLocal(IntPtr intImageAddress)
        {
            CImageDrawing objImage = new CImageDrawing(m_objMainCImage.Width, m_objMainCImage.Height);
            objImage.ref_objMainCImage.SetImagePtr(m_objMainCImage.Width, m_objMainCImage.Height, intImageAddress);
            objImage.ref_objMainCImage.CopyTo(m_objMainCImage);
            objImage.Dispose();
        }
        /// <summary>
        /// When refresh page, call this function to redraw color image on the picture box
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        public void RedrawImage(Graphics g)
        {
            //if (m_objMainCImage == null)
            //    return;

            //if (m_fScale == 1.0f)
            //{
            //    if (m_objMainCImage.Width < m_intPictureBoxWidth || m_objMainCImage.Height < m_intPictureBoxHeight)
            //    {
            //        EImageC24 objParent = new EImageC24();
            //        objParent.SetSize(m_intPictureBoxWidth, m_intPictureBoxHeight);

            //        EROIC24 objChild = new EROIC24();
            //        objChild.SetPlacement((m_intPictureBoxWidth / 2) - (m_intResolutionWidth / 2), (m_intPictureBoxHeight / 2) -
            //            (m_intResolutionHeight / 2), m_objMainCImage.Width, m_objMainCImage.Height);
            //        objChild.Attach(objParent);
            //        EasyImage.Copy(m_objMainCImage, objChild);

            //        m_objMainCImage = new EImageC24(m_intPictureBoxWidth, m_intPictureBoxHeight);
            //        EasyImage.Copy(objParent, m_objMainCImage);
            //    }

            //    m_objMainCImage.Draw(g);
            //}
            //else
            //{
            //    m_objZoomROI.Draw(g);
            //}
            if (m_objMainCImage == null)
                return;

            m_objMainCImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        public void RedrawImage(Graphics g, float ScaleX, float ScaleY)
        {
            //if (m_objMainCImage == null)
            //    return;

            //if (m_fScale == 1.0f)
            //{
            //    if (m_objMainCImage.Width < m_intPictureBoxWidth || m_objMainCImage.Height < m_intPictureBoxHeight)
            //    {
            //        EImageC24 objParent = new EImageC24();
            //        objParent.SetSize(m_intPictureBoxWidth, m_intPictureBoxHeight);

            //        EROIC24 objChild = new EROIC24();
            //        objChild.SetPlacement((m_intPictureBoxWidth / 2) - (m_intResolutionWidth / 2), (m_intPictureBoxHeight / 2) -
            //            (m_intResolutionHeight / 2), m_objMainCImage.Width, m_objMainCImage.Height);
            //        objChild.Attach(objParent);
            //        EasyImage.Copy(m_objMainCImage, objChild);

            //        m_objMainCImage = new EImageC24(m_intPictureBoxWidth, m_intPictureBoxHeight);
            //        EasyImage.Copy(objParent, m_objMainCImage);
            //    }

            //    m_objMainCImage.Draw(g);
            //}
            //else
            //{
            //    m_objZoomROI.Draw(g);
            //}
            if (m_objMainCImage == null)
                return;

            if (m_fScale == 1.0f)
                m_objMainCImage.Draw(g, ScaleX, ScaleY);
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                m_objZoomROI.Draw(g, ScaleX, ScaleY);
            }
        }
        /// <summary>
        /// Draw Threshold image
        /// </summary>
        /// <param name="g">destination of image</param>
        /// <param name="objROI">ROI that contain image to be threshold</param>
        /// <param name="intColorThreshold">value for red, green and blue</param>
        /// <param name="intColorTolerance">value for every color tolerance</param>
        ///<param name="blnRGBFormat">true = RGB format; false = LSH format</param>
        public void RedrawImage(Graphics g, CROI objROI, int[] intColorThreshold, int[] intColorTolerance, bool blnRGBFormat)
        {
            if (m_objMainCImage == null || !objROI.CheckROIParent())
                return;

            m_objThresholdImage.SetSize(m_objMainCImage.Width, m_objMainCImage.Height);
            //EasyImage.Convert(m_objMainCImage, m_objThresholdImage);
            EasyColor.GetComponent(m_objMainCImage, m_objThresholdImage, 0);

            EROIBW8 objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            objTrainROI.Attach(m_objThresholdImage);
            //objROI.SaveImage("D:\\objROI.bmp");
            //objTrainROI.Save("D:\\objTrainROI.bmp");
            Color24 objMinColor = ColorProcessing.CalculateMinColor(intColorThreshold, intColorTolerance);
            Color24 objMaxColor = ColorProcessing.CalculateMaxColor(intColorThreshold, intColorTolerance);

            if (blnRGBFormat)
            {
                EasyImage.Threshold(objROI.ref_CROI, objTrainROI, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupRGB);
            }
            else
            {
                EasyImage.Threshold(objROI.ref_CROI, objTrainROI, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupLSH);
            }

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        public void RedrawThresholdImage(Graphics g, List<List<CROI>> arrROIs, int[] intColorThreshold, int[] intColorTolerance, bool blnRGBFormat, int intSelectedROI, bool blnDrawAllSide)
        {
            if (m_objMainCImage == null)
                return;

            m_objThresholdImage.SetSize(m_objMainCImage.Width, m_objMainCImage.Height);
            //EasyImage.Convert(m_objMainCImage, m_objThresholdImage);
            EasyColor.GetComponent(m_objMainCImage, m_objThresholdImage, 0);

            Color24 objMinColor = ColorProcessing.CalculateMinColor(intColorThreshold, intColorTolerance);
            Color24 objMaxColor = ColorProcessing.CalculateMaxColor(intColorThreshold, intColorTolerance);

            if (blnDrawAllSide)
            {
                EROIBW8 objTrainROI2;

                objTrainROI2 = new EROIBW8();
                for (int i = 1; i < arrROIs.Count; i++)
                {
                    if (i != intSelectedROI)
                    {
                        objTrainROI2.SetPlacement(arrROIs[i][0].ref_ROITotalX, arrROIs[i][0].ref_ROITotalY, arrROIs[i][0].ref_ROIWidth, arrROIs[i][0].ref_ROIHeight);
                        objTrainROI2.Detach();
                        objTrainROI2.Attach(m_objThresholdImage);

                        if (blnRGBFormat)
                        {
                            EasyImage.Threshold(arrROIs[i][0].ref_CROI, objTrainROI2, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupRGB);
                        }
                        else
                        {
                            EasyImage.Threshold(arrROIs[i][0].ref_CROI, objTrainROI2, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupLSH);
                        }

                    }
                }
            }


            EROIBW8 objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(arrROIs[intSelectedROI][0].ref_ROITotalX, arrROIs[intSelectedROI][0].ref_ROITotalY, arrROIs[intSelectedROI][0].ref_ROIWidth, arrROIs[intSelectedROI][0].ref_ROIHeight);
            objTrainROI.Attach(m_objThresholdImage);
            //objROI.SaveImage("D:\\objROI.bmp");
            //objTrainROI.Save("D:\\objTrainROI.bmp");
           
            if (blnRGBFormat)
            {
                EasyImage.Threshold(arrROIs[intSelectedROI][0].ref_CROI, objTrainROI, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupRGB);
            }
            else
            {
                EasyImage.Threshold(arrROIs[intSelectedROI][0].ref_CROI, objTrainROI, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupLSH);
            }

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        public void RedrawThresholdImage(Graphics g, List<List<CROI>> arrROIs, int[] intColorThreshold, int[] intColorTolerance, int intColorFormat, int intSelectedROI, bool blnDrawAllSide)
        {
            if (m_objMainCImage == null)
                return;

            m_objThresholdImage.SetSize(m_objMainCImage.Width, m_objMainCImage.Height);
            //EasyImage.Convert(m_objMainCImage, m_objThresholdImage);
            EasyColor.GetComponent(m_objMainCImage, m_objThresholdImage, 0);

            Color24 objMinColor = ColorProcessing.CalculateMinColor(intColorThreshold, intColorTolerance);
            Color24 objMaxColor = ColorProcessing.CalculateMaxColor(intColorThreshold, intColorTolerance);

            if (blnDrawAllSide)
            {
                EROIBW8 objTrainROI2;

                objTrainROI2 = new EROIBW8();
                for (int i = 1; i < arrROIs.Count; i++)
                {
                    if (i != intSelectedROI)
                    {
                        objTrainROI2.SetPlacement(arrROIs[i][0].ref_ROITotalX, arrROIs[i][0].ref_ROITotalY, arrROIs[i][0].ref_ROIWidth, arrROIs[i][0].ref_ROIHeight);
                        objTrainROI2.Detach();
                        objTrainROI2.Attach(m_objThresholdImage);

                        if (intColorFormat == 1)
                        {
                            EasyImage.Threshold(arrROIs[i][0].ref_CROI, objTrainROI2, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupRGB);
                        }
                        else if (intColorFormat == 0)
                        {
                            EasyImage.Threshold(arrROIs[i][0].ref_CROI, objTrainROI2, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupLSH);
                        }
                        else
                        {
                            EasyColor.GetComponent(arrROIs[i][0].ref_CROI, objTrainROI2, 1, m_objColorLookupYSH);
#if (Debug_2_12 || Release_2_12)
                            EasyImage.Threshold(objTrainROI2, objTrainROI2, (uint)intColorThreshold[0]);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            EasyImage.Threshold(objTrainROI2, objTrainROI2, intColorThreshold[0]);
#endif
                            
                        }
                    }
                }
            }


            EROIBW8 objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(arrROIs[intSelectedROI][0].ref_ROITotalX, arrROIs[intSelectedROI][0].ref_ROITotalY, arrROIs[intSelectedROI][0].ref_ROIWidth, arrROIs[intSelectedROI][0].ref_ROIHeight);
            objTrainROI.Attach(m_objThresholdImage);
            //objROI.SaveImage("D:\\objROI.bmp");
            //objTrainROI.Save("D:\\objTrainROI.bmp");

            if (intColorFormat == 1)
            {
                EasyImage.Threshold(arrROIs[intSelectedROI][0].ref_CROI, objTrainROI, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupRGB);
            }
            else if (intColorFormat == 0)
            {
                EasyImage.Threshold(arrROIs[intSelectedROI][0].ref_CROI, objTrainROI, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupLSH);
            }
            else
            {
                EasyColor.GetComponent(arrROIs[intSelectedROI][0].ref_CROI, objTrainROI, 1, m_objColorLookupYSH);
#if (Debug_2_12 || Release_2_12)
                EasyImage.Threshold(objTrainROI, objTrainROI, (uint)intColorThreshold[0]);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.Threshold(objTrainROI, objTrainROI, intColorThreshold[0]);
#endif

            }

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        public void RedrawThresholdImage(Graphics g, List<List<CROI>> arrROIs, int[] intColorThreshold, int[] intColorTolerance, int intColorFormat, int intSelectedROI, int intGroupIndex, bool blnDrawSide, int intCloseIteration, bool blnColorInvertBlackWhite)
        {
            if (m_objMainCImage == null)
                return;

            m_objThresholdImage.SetSize(m_objMainCImage.Width, m_objMainCImage.Height);
            //EasyImage.Convert(m_objMainCImage, m_objThresholdImage);
            EasyColor.GetComponent(m_objMainCImage, m_objThresholdImage, 0);

            Color24 objMinColor = ColorProcessing.CalculateMinColor(intColorThreshold, intColorTolerance);
            Color24 objMaxColor = ColorProcessing.CalculateMaxColor(intColorThreshold, intColorTolerance);


            EROIBW8 objTrainROI2;

            objTrainROI2 = new EROIBW8();
            for (int i = 0; i < arrROIs.Count; i++)
            {
                if (i > 0 && !blnDrawSide)
                    continue;

                if ((i == 0) && ((intGroupIndex & 0x01) == 0))
                    continue;
                else if ((i == 1) && ((intGroupIndex & 0x02) == 0))
                    continue;
                else if ((i == 2) && ((intGroupIndex & 0x04) == 0))
                    continue;
                else if ((i == 3) && ((intGroupIndex & 0x08) == 0))
                    continue;
                else if ((i == 4) && ((intGroupIndex & 0x10) == 0))
                    continue;

                objTrainROI2.SetPlacement(arrROIs[i][0].ref_ROITotalX, arrROIs[i][0].ref_ROITotalY, arrROIs[i][0].ref_ROIWidth, arrROIs[i][0].ref_ROIHeight);
                objTrainROI2.Detach();
                objTrainROI2.Attach(m_objThresholdImage);

                if (intColorFormat == 1)
                {
                    EasyImage.Threshold(arrROIs[i][0].ref_CROI, objTrainROI2, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupRGB);
                }
                else if (intColorFormat == 0)
                {
                    EasyImage.Threshold(arrROIs[i][0].ref_CROI, objTrainROI2, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupLSH);
                }
                else
                {
                    EasyColor.GetComponent(arrROIs[i][0].ref_CROI, objTrainROI2, 1, m_objColorLookupYSH);
#if (Debug_2_12 || Release_2_12)
                    EasyImage.Threshold(objTrainROI2, objTrainROI2, (uint)intColorThreshold[0]);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    EasyImage.Threshold(objTrainROI2, objTrainROI2, intColorThreshold[0]);
#endif
                 
                }

                if (intCloseIteration > 0)
                {
#if (Debug_2_12 || Release_2_12)
                    EasyImage.CloseBox(objTrainROI2, objTrainROI2, (uint)intCloseIteration);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    EasyImage.CloseBox(objTrainROI2, objTrainROI2, intCloseIteration);
#endif
                }

                if (blnColorInvertBlackWhite)
                {
                    EasyImage.Oper(EArithmeticLogicOperation.Invert, objTrainROI2, objTrainROI2);
                }
            }

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        public void RedrawThresholdImage(Graphics g, List<CROI> arrROIs, int[] intColorThreshold, int[] intColorTolerance, int intColorFormat, int intSelectedROI, int intCloseIteration, bool blnColorInvertBlackWhite)
        {
            if (m_objMainCImage == null)
                return;

            m_objThresholdImage.SetSize(m_objMainCImage.Width, m_objMainCImage.Height);
            //EasyImage.Convert(m_objMainCImage, m_objThresholdImage);
            EasyColor.GetComponent(m_objMainCImage, m_objThresholdImage, 0);

            Color24 objMinColor = ColorProcessing.CalculateMinColor(intColorThreshold, intColorTolerance);
            Color24 objMaxColor = ColorProcessing.CalculateMaxColor(intColorThreshold, intColorTolerance);


            EROIBW8 objTrainROI2;

            objTrainROI2 = new EROIBW8();
            for (int i = 0; i < arrROIs.Count; i++)
            {
                objTrainROI2.SetPlacement(arrROIs[i].ref_ROITotalX, arrROIs[i].ref_ROITotalY, arrROIs[i].ref_ROIWidth, arrROIs[i].ref_ROIHeight);
                objTrainROI2.Detach();
                objTrainROI2.Attach(m_objThresholdImage);

                if (intColorFormat == 1)
                {
                    EasyImage.Threshold(arrROIs[i].ref_CROI, objTrainROI2, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupRGB);
                }
                else if (intColorFormat == 0)
                {
                    EasyImage.Threshold(arrROIs[i].ref_CROI, objTrainROI2, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupLSH);
                }
                else
                {
                    EasyColor.GetComponent(arrROIs[i].ref_CROI, objTrainROI2, 1, m_objColorLookupYSH);
#if (Debug_2_12 || Release_2_12)
                    EasyImage.Threshold(objTrainROI2, objTrainROI2, (uint)intColorThreshold[0]);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    EasyImage.Threshold(objTrainROI2, objTrainROI2, intColorThreshold[0]);
#endif

                }

                if (intCloseIteration > 0)
                {
#if (Debug_2_12 || Release_2_12)
                    EasyImage.CloseBox(objTrainROI2, objTrainROI2, (uint)intCloseIteration);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.CloseBox(objTrainROI2, objTrainROI2, intCloseIteration);
#endif
                }

                if (blnColorInvertBlackWhite)
                {
                    EasyImage.Oper(EArithmeticLogicOperation.Invert, objTrainROI2, objTrainROI2);
                }
            }

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        /// <summary>
        /// Draw Threshold image of ROI part only
        /// </summary>
        /// <param name="g">destination of image</param>
        /// <param name="objROI">ROI that contain image to be threshold</param>
        /// <param name="intColorThreshold">value for red, green and blue</param>
        /// <param name="intColorTolerance">value for every color tolerance</param>
        ///<param name="blnRGBFormat">true = RGB format; false = LSH format</param>
        ///<param name="intPictureWidth">size of picture box to decide ROI zoom scale</param>
        ///<param name="intPictureHeight">size of picture box to decide ROI zoom scale</intPictureHeight>
        public void RedrawImage(Graphics g, CROI objROI, int[] intColorThreshold, int[] intColorTolerance, bool blnRGBFormat, int intPictureWidth, int intPictureHeight)
        {
            if (m_objMainCImage == null || !objROI.CheckROIParent())
                return;

            m_objThresholdImage.SetSize(m_objMainCImage.Width, m_objMainCImage.Height);
            //EasyImage.Convert(m_objMainCImage, m_objThresholdImage);
            EasyColor.GetComponent(m_objMainCImage, m_objThresholdImage, 0);

            EROIBW8 objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            objTrainROI.Attach(m_objThresholdImage);

            Color24 objMinColor = ColorProcessing.CalculateMinColor(intColorThreshold, intColorTolerance);
            Color24 objMaxColor = ColorProcessing.CalculateMaxColor(intColorThreshold, intColorTolerance);

            if (blnRGBFormat)
            {
                EasyImage.Threshold(objROI.ref_CROI, objTrainROI, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupRGB);
            }
            else
            {
                EasyImage.Threshold(objROI.ref_CROI, objTrainROI, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupLSH);
            }
         
            float fZoom = Math.Min(intPictureWidth / (float)objTrainROI.Width, intPictureHeight / (float)objTrainROI.Height);
            EImageBW8 objZoomImage = new EImageBW8(intPictureWidth, intPictureHeight);                         
            EasyImage.ScaleRotate(objTrainROI, (objTrainROI.Width) / 2, (objTrainROI.Height) / 2,
                (objZoomImage.Width) / 2, (objZoomImage.Height) / 2, fZoom, fZoom, 0.0f, objZoomImage);          
            objZoomImage.Draw(g);           
        }
        
        /// <summary>
        ///  When refresh page, call this function to redraw color image on the picture box
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        /// <param name="intColorThreshold">value for red, green and blue</param>
        /// <param name="intColorTolerance">value for every color tolerance</param>
        ///<param name="blnRGBFormat">true = RGB format; false = LSH format</param>
        public void RedrawImage(Graphics g, int[] intColorThreshold, int[] intColorTolerance, bool blnRGBFormat)
        {
            if (m_objMainCImage == null)
                return;

            Color24 objMinColor = ColorProcessing.CalculateMinColor(intColorThreshold, intColorTolerance);
            Color24 objMaxColor = ColorProcessing.CalculateMaxColor(intColorThreshold, intColorTolerance);

            m_objThresholdImage.SetSize(m_objMainCImage.Width, m_objMainCImage.Height);

            if (blnRGBFormat)
            {
                EasyImage.Threshold(m_objMainCImage, m_objThresholdImage, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupRGB);        
            }
            else
            {
                EasyImage.Threshold(m_objMainCImage, m_objThresholdImage, objMinColor.ref_Color24, objMaxColor.ref_Color24, m_objColorLookupLSH);        
            }            
     
            m_objThresholdImage.Draw(g);
        }



        /// <summary>
        /// Save Image to desired folder and file name as well
        /// </summary>
        /// <param name="strPath">selected path</param>
        public void SaveImage(string strPath)
        {
            string strDirectoryName = System.IO.Path.GetDirectoryName(strPath);
            DirectoryInfo directory = new DirectoryInfo(strDirectoryName);
            if (!directory.Exists)
                CreateUnexistDirectory(directory);

            try
            {
                m_objMainCImage.Save(strPath);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                string a = ex.ToString();
            }
        }

        /// <summary>
        /// Reset color image size
        /// </summary>
        /// <param name="intImageWidth">image width</param>
        /// <param name="intImageHeight">image height</param>
        public void SetImageSize(int intImageWidth, int intImageHeight)
        {
            m_intResolutionWidth = intImageWidth;
            m_intResolutionHeight = intImageHeight;

            if (m_objMainCImage.Width != intImageWidth || m_objMainCImage.Height != intImageHeight)
            {
                m_objMainCImage.SetSize(intImageWidth, intImageHeight);
            }
        }

        //public void AddGain(float fGain)
        //{
        //    if (fGain != 1f)
        //        EasyImage.GainOffset(m_objMainImage, m_objMainImage, fGain);
        //}

        /// <summary>
        /// If the specific directory does not exist, create a new one
        /// </summary>
        private void CreateUnexistDirectory(DirectoryInfo directory)
        {
            if (!directory.Parent.Exists)
            {
                CreateUnexistDirectory(directory.Parent);
            }

            Directory.CreateDirectory(directory.FullName);

        }
        public static void SetImageSize(EImageC24 objImage, int intWidth, int intHeight)
        {
            try
            {
                if (objImage == null)
                    return;

                if ((objImage.Width != intWidth) || (objImage.Height != intHeight))
                {
                    if (objImage.FirstSubROI != null)
                    {
                        // if child roi start point higher than image size, then child roi will be set placement same size as image size. The purpose is to prevent "Parent Out of Limit" error.
                        if ((objImage.FirstSubROI.OrgX > intWidth) || (objImage.FirstSubROI.OrgY > intHeight))
                        {
                            objImage.FirstSubROI.SetPlacement(0, 0, intWidth, intHeight);
                        }
                    }

                    try
                    {
                        objImage.SetSize(intWidth, intHeight);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("CImageDrawing.cs -> SetImageSize() :" + ex.ToString());
            }
        }

        private static List<int> m_arrImageCount = new List<int>();
        private static List<int> m_arrImageMergeType = new List<int>();

        public static void SetImageCount(int intImageCount, int intVisionIndex)
        {
            for (int i = m_arrImageCount.Count; i < intVisionIndex; i++)
            {
                m_arrImageCount.Add(0);
            }

            m_arrImageCount.Add(intImageCount);
        }

        public static void SetImageMergeType(int intImageMergeType, int intVisionIndex)
        {
            for (int i = m_arrImageMergeType.Count; i < intVisionIndex; i++)
            {
                m_arrImageMergeType.Add(0);
            }

            m_arrImageMergeType.Add(intImageMergeType);
        }

        public static int GetArrayImageIndex(int intUserSelectImageViewIndex, int intVisionIndex)
        {
            if (intUserSelectImageViewIndex < 0)
                return 0;

            switch (m_arrImageMergeType[intVisionIndex])
            {
                default:
                case 0: // No merge
                    {
                        return intUserSelectImageViewIndex;
                    }
                case 1: // Merge grab 1 center and grab 2 side 
                    {
                        if (intUserSelectImageViewIndex <= 0)
                            return 0;
                        else if (intUserSelectImageViewIndex + 1 >= m_arrImageCount[intVisionIndex])
                            return intUserSelectImageViewIndex;
                        else
                            return (intUserSelectImageViewIndex + 1);
                    }
                case 2: // Merge grab 1 center, grab 2 top left and grab 3 bottom right.
                    {
                        if (intUserSelectImageViewIndex <= 0)
                            return 0;
                        else if (intUserSelectImageViewIndex + 1 >= m_arrImageCount[intVisionIndex])
                            return intUserSelectImageViewIndex;
                        else if (intUserSelectImageViewIndex + 2 >= m_arrImageCount[intVisionIndex])
                            return (intUserSelectImageViewIndex + 1);
                        else
                            return (intUserSelectImageViewIndex + 2);
                    }
                case 3: // Merge grab 1 center and grab 2 side and Merge grab 3 center and grab 4 side 
                    {
                        if (intUserSelectImageViewIndex <= 0)
                            return 0;

                        if (intUserSelectImageViewIndex == 2) // select image 2 which is grab 5
                        {
                            if (intUserSelectImageViewIndex + 2 >= m_arrImageCount[intVisionIndex])
                                return intUserSelectImageViewIndex;
                            else
                                return (intUserSelectImageViewIndex + 2);
                        }
                        else // select image 1 which is grab 3 and 4
                        {
                            if (intUserSelectImageViewIndex + 1 >= m_arrImageCount[intVisionIndex])
                                return intUserSelectImageViewIndex;
                            else
                                return (intUserSelectImageViewIndex + 1);
                        }
                    }
                case 4: // Merge grab 1 center, grab 2 top left and grab 3 bottom right. and Merge grab 3 center and grab 4 side 
                    {
                        if (intUserSelectImageViewIndex <= 0)
                            return 0;

                        if (intUserSelectImageViewIndex == 1) // select image 1 which is grab 4 and 5
                        {
                            if (intUserSelectImageViewIndex + 2 >= m_arrImageCount[intVisionIndex])
                                return intUserSelectImageViewIndex;
                            else
                                return intUserSelectImageViewIndex + 2;
                        }
                        else // select other than image 0 or 1
                        {
                            if (intUserSelectImageViewIndex + 3 >= m_arrImageCount[intVisionIndex])
                                return intUserSelectImageViewIndex;
                            else
                                return (intUserSelectImageViewIndex + 3);
                        }
                    }
            }
        }

        public void ConvertToSaturationImage(ref ImageDrawing objDestinationImage)
        {
            EasyColor.GetComponent(m_objMainCImage, objDestinationImage.ref_objMainImage, 1, m_objColorLookupYSH);
        }
        public void Rotate90Image(ref CImageDrawing objImageDest)
        {
            try
            {
              
                if (objImageDest == null)
                {
                    objImageDest = new CImageDrawing(true);
                }

                if (objImageDest.ref_intImageWidth != m_objMainCImage.Width || objImageDest.ref_intImageHeight != m_objMainCImage.Height)
                    objImageDest.SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);


                EasyImage.ScaleRotate(m_objMainCImage,
                                    m_objMainCImage.Width / 2, m_objMainCImage.Height / 2,
                                    objImageDest.ref_objMainCImage.Width / 2, objImageDest.ref_objMainCImage.Height / 2, 1, 1, 90, objImageDest.ref_objMainCImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("CImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void Rotate90Image(ref List<CImageDrawing> arrImageDest, int intImageIndex)
        {
            if (arrImageDest.Count <= intImageIndex)
                return;

            try
            {
                if (arrImageDest[intImageIndex] == null)
                {
                    arrImageDest[intImageIndex] = new CImageDrawing(true, m_objMainCImage.Width, m_objMainCImage.Height);
                }

                if (arrImageDest[intImageIndex].ref_intImageWidth != m_objMainCImage.Width || arrImageDest[intImageIndex].ref_intImageHeight != m_objMainCImage.Height)
                    arrImageDest[intImageIndex].SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);


                EasyImage.ScaleRotate(m_objMainCImage,
                                    m_objMainCImage.Width / 2, m_objMainCImage.Height / 2,
                                    arrImageDest[intImageIndex].ref_objMainCImage.Width / 2, arrImageDest[intImageIndex].ref_objMainCImage.Height / 2, 1, 1, 90, arrImageDest[intImageIndex].ref_objMainCImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("CImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void RotateMinus90Image(ref CImageDrawing objImageDest)
        {
            try
            {
                if (objImageDest == null)
                {
                    objImageDest = new CImageDrawing(true);
                }

                if (objImageDest.ref_intImageWidth != m_objMainCImage.Width || objImageDest.ref_intImageHeight != m_objMainCImage.Height)
                    objImageDest.SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);


                EasyImage.ScaleRotate(m_objMainCImage,
                                    m_objMainCImage.Width / 2, m_objMainCImage.Height / 2,
                                    objImageDest.ref_objMainCImage.Width / 2, objImageDest.ref_objMainCImage.Height / 2, 1, 1, -90, objImageDest.ref_objMainCImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("CImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void RotateMinus90Image(ref List<CImageDrawing> arrImageDest, int intImageIndex)
        {
            if (arrImageDest.Count <= intImageIndex)
                return;

            try
            {
                if (arrImageDest[intImageIndex] == null)
                {
                    arrImageDest[intImageIndex] = new CImageDrawing(true, m_objMainCImage.Width, m_objMainCImage.Height);
                }

                if (arrImageDest[intImageIndex].ref_intImageWidth != m_objMainCImage.Width || arrImageDest[intImageIndex].ref_intImageHeight != m_objMainCImage.Height)
                    arrImageDest[intImageIndex].SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);


                EasyImage.ScaleRotate(m_objMainCImage,
                                    m_objMainCImage.Width / 2, m_objMainCImage.Height / 2,
                                    arrImageDest[intImageIndex].ref_objMainCImage.Width / 2, arrImageDest[intImageIndex].ref_objMainCImage.Height / 2, 1, 1, -90, arrImageDest[intImageIndex].ref_objMainCImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("CImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void Rotate180Image(ref CImageDrawing objImageDest)
        {
            try
            {
                if (objImageDest == null)
                {
                    objImageDest = new CImageDrawing(true);
                }

                if (objImageDest.ref_intImageWidth != m_objMainCImage.Width || objImageDest.ref_intImageHeight != m_objMainCImage.Height)
                    objImageDest.SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);


                EasyImage.ScaleRotate(m_objMainCImage,
                                    m_objMainCImage.Width / 2, m_objMainCImage.Height / 2,
                                    objImageDest.ref_objMainCImage.Width / 2, objImageDest.ref_objMainCImage.Height / 2, 1, 1, 180, objImageDest.ref_objMainCImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("CImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void Rotate180Image(ref List<CImageDrawing> arrImageDest, int intImageIndex)
        {
            if (arrImageDest.Count <= intImageIndex)
                return;

            try
            {
                if (arrImageDest[intImageIndex] == null)
                {
                    arrImageDest[intImageIndex] = new CImageDrawing(true, m_objMainCImage.Width, m_objMainCImage.Height);
                }

                if (arrImageDest[intImageIndex].ref_intImageWidth != m_objMainCImage.Width || arrImageDest[intImageIndex].ref_intImageHeight != m_objMainCImage.Height)
                    arrImageDest[intImageIndex].SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);


                EasyImage.ScaleRotate(m_objMainCImage,
                                    m_objMainCImage.Width / 2, m_objMainCImage.Height / 2,
                                    arrImageDest[intImageIndex].ref_objMainCImage.Width / 2, arrImageDest[intImageIndex].ref_objMainCImage.Height / 2, 1, 1, 180, arrImageDest[intImageIndex].ref_objMainCImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("CImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void FlipHorizontalImage(ref CImageDrawing objImageDest)
        {
            try
            {
                if (objImageDest == null)
                {
                    objImageDest = new CImageDrawing(true);
                }

                if (objImageDest.ref_intImageWidth != m_objMainCImage.Width || objImageDest.ref_intImageHeight != m_objMainCImage.Height)
                    objImageDest.SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);


                EasyImage.ScaleRotate(m_objMainCImage,
                                    m_objMainCImage.Width / 2, m_objMainCImage.Height / 2,
                                    objImageDest.ref_objMainCImage.Width / 2, objImageDest.ref_objMainCImage.Height / 2, -1, 1, 0, objImageDest.ref_objMainCImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("CImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void FlipHorizontalImage(ref List<CImageDrawing> arrImageDest, int intImageIndex)
        {
            if (arrImageDest.Count <= intImageIndex)
                return;

            try
            {
                if (arrImageDest[intImageIndex] == null)
                {
                    arrImageDest[intImageIndex] = new CImageDrawing(true, m_objMainCImage.Width, m_objMainCImage.Height);
                }

                if (arrImageDest[intImageIndex].ref_intImageWidth != m_objMainCImage.Width || arrImageDest[intImageIndex].ref_intImageHeight != m_objMainCImage.Height)
                    arrImageDest[intImageIndex].SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);


                EasyImage.ScaleRotate(m_objMainCImage,
                                    m_objMainCImage.Width / 2, m_objMainCImage.Height / 2,
                                    arrImageDest[intImageIndex].ref_objMainCImage.Width / 2, arrImageDest[intImageIndex].ref_objMainCImage.Height / 2, -1, 1, 0, arrImageDest[intImageIndex].ref_objMainCImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("CImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void FlipVerticalImage(ref CImageDrawing objImageDest)
        {
            try
            {
                if (objImageDest == null)
                {
                    objImageDest = new CImageDrawing(true);
                }

                if (objImageDest.ref_intImageWidth != m_objMainCImage.Width || objImageDest.ref_intImageHeight != m_objMainCImage.Height)
                    objImageDest.SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);


                EasyImage.ScaleRotate(m_objMainCImage,
                                    m_objMainCImage.Width / 2, m_objMainCImage.Height / 2,
                                    objImageDest.ref_objMainCImage.Width / 2, objImageDest.ref_objMainCImage.Height / 2, 1, -1, 0, objImageDest.ref_objMainCImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("CImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void FlipVerticalImage(ref List<CImageDrawing> arrImageDest, int intImageIndex)
        {
            if (arrImageDest.Count <= intImageIndex)
                return;

            try
            {
                if (arrImageDest[intImageIndex] == null)
                {
                    arrImageDest[intImageIndex] = new CImageDrawing(true, m_objMainCImage.Width, m_objMainCImage.Height);
                }

                if (arrImageDest[intImageIndex].ref_intImageWidth != m_objMainCImage.Width || arrImageDest[intImageIndex].ref_intImageHeight != m_objMainCImage.Height)
                    arrImageDest[intImageIndex].SetImageSize(m_objMainCImage.Width, m_objMainCImage.Height);


                EasyImage.ScaleRotate(m_objMainCImage,
                                    m_objMainCImage.Width / 2, m_objMainCImage.Height / 2,
                                    arrImageDest[intImageIndex].ref_objMainCImage.Width / 2, arrImageDest[intImageIndex].ref_objMainCImage.Height / 2, 1, -1, 0, arrImageDest[intImageIndex].ref_objMainCImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("CImageDrawing->RotateImage->ex = " + ex.ToString());
            }
        }
        public void SetImageToBlack()
        {
            EasyImage.Copy(new EC24(0, 0, 0), m_objMainCImage);
        }

        public void AddOffset(float fOffset, int intColorFormat)
        {
            if (fOffset >= -255 && fOffset <= 255)
            {
                if (intColorFormat == 1)
                {
                    m_objColorLookupRGB.AdjustGainOffset(EColorSystem.Rgb, 1f, fOffset, 1f, fOffset, 1f, fOffset);
                    m_objColorLookupRGB.Transform(m_objMainCImage, m_objMainCImage);
                }
                else if (intColorFormat == 0)
                {
                    m_objColorLookupLSH.AdjustGainOffset(EColorSystem.Rgb, 1f, fOffset, 1f, fOffset, 1f, fOffset);
                    m_objColorLookupLSH.Transform(m_objMainCImage, m_objMainCImage);
                }
                else
                {
                    m_objColorLookupYSH.AdjustGainOffset(EColorSystem.Rgb, 1f, fOffset, 1f, fOffset, 1f, fOffset);
                    m_objColorLookupYSH.Transform(m_objMainCImage, m_objMainCImage);
                }
            }
        }

        public void AddDilate(int intDilate)
        {
            if (intDilate >= 0)
#if (Debug_2_12 || Release_2_12)
                EasyImage.DilateBox(m_objMainCImage, m_objMainCImage, (uint)intDilate);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.DilateBox(m_objMainImage, m_objMainImage, intDilate);
#endif
        }

        public void AddOpenMorphology(int intOpen)
        {
            if (intOpen >= 0)
#if (Debug_2_12 || Release_2_12)
                EasyImage.OpenBox(m_objMainCImage, m_objMainCImage, (uint)intOpen);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.OpenBox(m_objMainImage, m_objMainImage, intOpen);
#endif
        }
        public void AddCloseMorphology(int intClose)
        {
            if (intClose >= 0)
#if (Debug_2_12 || Release_2_12)
                EasyImage.CloseBox(m_objMainCImage, m_objMainCImage, (uint)intClose);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.CloseBox(m_objMainImage, m_objMainImage, intClose);
#endif
        }
        public static int GetImageViewCount(int intVisionIndex)
        {
            if (m_arrImageCount[intVisionIndex] == 0)
                return 0;

            switch (m_arrImageMergeType[intVisionIndex])
            {
                default:
                case 0: // No merge
                    {
                        return m_arrImageCount[intVisionIndex];
                    }
                case 1: // Merge grab 1 center and grab 2 side 
                    {
                        if (m_arrImageCount[intVisionIndex] <= 2)
                            return 1;
                        else
                            return m_arrImageCount[intVisionIndex] - 1;
                    }
                case 2: // Merge grab 1 center, grab 2 top left and grab 3 bottom right.
                    {
                        if (m_arrImageCount[intVisionIndex] <= 3)
                            return 1;
                        else
                            return m_arrImageCount[intVisionIndex] - 2;
                    }
                case 3: // Merge grab 1 center and grab 2 side and Merge grab 3 center and grab 4 side 
                    {
                        if (m_arrImageCount[intVisionIndex] <= 2)
                            return 1;
                        else if (m_arrImageCount[intVisionIndex] <= 4)
                            return 2;
                        else
                            return m_arrImageCount[intVisionIndex] - 2;
                    }
                case 4: // Merge grab 1 center, grab 2 top left and grab 3 bottom right. and Merge grab 4 center and grab 5 side 
                    {
                        if (m_arrImageCount[intVisionIndex] <= 3)
                            return 1;
                        else if (m_arrImageCount[intVisionIndex] <= 5)
                            return 2;
                        else
                            return m_arrImageCount[intVisionIndex] - 3;
                    }
            }
        }
    }
}
