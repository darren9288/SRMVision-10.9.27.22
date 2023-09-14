using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;
using System.Threading;

namespace VisionProcessing
{
    public class ImageDrawing
    {
        #region Constant Variables

        private const int PICBOX_WIDTH = 640;
        private const int PICBOX_HEIGHT = 480;

        #endregion

        #region Member Variables

        private int m_intImageWidth = 0;
        private int m_intImageHeight = 0;
        private int m_intPictureBoxWidth = 640;
        private int m_intPitureBoxHeight = 480;
        private int m_intCameraResolutionWidth = PICBOX_WIDTH;    // camera resolution
        private int m_intCameraResolutionHeight = PICBOX_HEIGHT;  // camera resolution


        private float m_fScale = 1.0f;
        private float m_fDrawingScaleX = 1.0f;
        private float m_fDrawingScaleY = 1.0f;

        private EBW8 m_bw = new EBW8();
        private EROIBW8 m_objZoomROI = new EROIBW8();
        private EImageBW8 m_objMainImage = null;    // new EImageBW8(640, 480);
        private EImageBW8 m_objZoomImage = null;    //new EImageBW8(640, 480);
        private EImageBW8 m_objThresholdImage = null;    //new EImageBW8(640, 480);
        private EImageBW8 m_objMergeImage = null;

        #endregion

        #region Properties
        public EImageBW8 ref_objMainImage {get{return m_objMainImage;}}
        public IntPtr ref_intImageAddress { get { return m_objMainImage.GetImagePtr(); } }//GetImagePtr(639, 0)
        public int ref_intImageWidth { get { return m_objMainImage.Width; } }
        public int ref_intImageHeight { get { return m_objMainImage.Height; } }
        public int ref_intCameraResolutionWidth { get { return m_intCameraResolutionWidth; } }
        public int ref_intCameraResolutionHeight { get { return m_intCameraResolutionHeight; } }
        public float ref_fDrawingScaleX { get { return m_fDrawingScaleX; } set { m_fDrawingScaleX = value; } }
        public float ref_fDrawingScaleY { get { return m_fDrawingScaleY; } set { m_fDrawingScaleY = value; } }

        #endregion

        public ImageDrawing()
        {
            //Easy.Initialize();

            m_objMainImage = new EImageBW8(640, 480);
            m_objMergeImage = new EImageBW8(640, 480);
            m_objZoomImage = new EImageBW8(640, 480);
            m_objThresholdImage = new EImageBW8(640, 480);
        }

        public ImageDrawing(int intImageWidth, int intImageHeight)
        {
            //Easy.Initialize();

            m_intCameraResolutionWidth = intImageWidth;
            m_intCameraResolutionHeight = intImageHeight; 

            m_objMainImage = new EImageBW8(intImageWidth, intImageHeight);
            m_objMergeImage = new EImageBW8(intImageWidth, intImageHeight);
            m_objZoomImage = new EImageBW8(intImageWidth, intImageHeight);
            m_objThresholdImage = new EImageBW8(intImageWidth, intImageHeight);
        }

        public ImageDrawing(bool blnBasic)
        {
            //Easy.Initialize();

            if (blnBasic)
            {
                m_objMainImage = new EImageBW8(640, 480);
            }
            else
            {
                m_objMainImage = new EImageBW8(640, 480);
                m_objMergeImage = new EImageBW8(640, 480);
                m_objZoomImage = new EImageBW8(640, 480);
                m_objThresholdImage = new EImageBW8(640, 480);
            }
        }

        public ImageDrawing(bool blnBasic, int intImageWidth, int intImageHeight)
        {
            //Easy.Initialize();

            m_intCameraResolutionWidth = intImageWidth;
            m_intCameraResolutionHeight = intImageHeight; 

            if (blnBasic)
            {
                m_objMainImage = new EImageBW8(intImageWidth, intImageHeight);
            }
            else
            {
                m_objMainImage = new EImageBW8(intImageWidth, intImageHeight);
                m_objMergeImage = new EImageBW8(intImageWidth, intImageHeight);
                m_objZoomImage = new EImageBW8(intImageWidth, intImageHeight);
                m_objThresholdImage = new EImageBW8(intImageWidth, intImageHeight);
            }
            
        }

        ~ImageDrawing()
        {
            //Easy.Terminate();
        }

        public void Dispose()
        {
            if (m_objZoomROI != null)
                m_objZoomROI.Dispose();

            if (m_objMainImage != null)
                m_objMainImage.Dispose();

            if (m_objZoomImage != null)
                m_objZoomImage.Dispose();

            if (m_objThresholdImage != null)
                m_objThresholdImage.Dispose();

            if (m_objMergeImage != null)
                m_objMergeImage.Dispose();
        }

        /// <summary>
        /// Draw histogram graft
        /// </summary>
        /// <param name="g">destination to draw image</param>
        /// <param name="objROI">ROI</param>
        /// <returns></returns>
        public static int DrawHistogram(Graphics g, ROI objROI)
        {
            int intHighestElement = 0;

            try
            {
                if (objROI.ref_ROI.Parent != null)
                {
                    EBWHistogramVector objHistogram = new EBWHistogramVector();
                    EasyImage.Histogram(objROI.ref_ROI, objHistogram);
                    objHistogram.Draw(g, new ERGBColor(255, 0, 0),  300, 150);

#if (Debug_2_12 || Release_2_12)
                    int intNumElements = (int)objHistogram.NumElements;
                    int intElement;
                    for (int i = 0; i < intNumElements; i++)
                    {
                        intElement = (int)objHistogram.GetElement(i);
                        if (intElement > intHighestElement)
                            intHighestElement = intElement;
                    }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    int intNumElements = objHistogram.NumElements;
                    int intElement;
                    for (int i = 0; i < intNumElements; i++)
                    {
                        intElement = objHistogram.GetElement(i);
                        if (intElement > intHighestElement)
                            intHighestElement = intElement;
                    }
#endif

                }
            }
            catch
            {
            }

            return intHighestElement;
        }

        /// <summary>
        ///  Draw histogram graft using single threshold
        /// </summary>
        /// <param name="g">destination to draw image</param>
        /// <param name="objROI">ROI</param>
        /// <param name="intThreshold">threshold value</param>
        public static void DrawHistogram(Graphics g, ROI objROI, int intThreshold)
        {
            int intHighestElement = DrawHistogram(g, objROI);
            // 29-05-2019 ZJYEOH: Max of the graph will be 241 no matter intHighestElement is what value, Min value for intHighestElement less than 10 will be 31 then for each 10 times of intHighestElement, Min value will increase by 7 
            // Get histogram graph width
            int intGraphWidth =  210;//240
            int intGraphOffSet = 0;
            if (intHighestElement >= 100000)
                intGraphWidth = 175 ; //208
            else if (intHighestElement >= 10000)
                intGraphWidth = 182 ;//216
            else if (intHighestElement >= 1000)
                intGraphWidth = 189 ;//224
            else if (intHighestElement >= 100)
                intGraphWidth = 196 ;//232
            else if (intHighestElement >= 10)
                intGraphWidth = 203;
            intGraphOffSet = 210 - intGraphWidth;//232

            float fX = intGraphOffSet + intThreshold * intGraphWidth / 255;
            g.DrawLine(new Pen(Color.Blue), fX + 31f, 30f, fX + 31f, 130f);//51f
        }

        /// <summary>
        ///  Draw histogram graft using double threshold
        /// </summary>
        /// <param name="g">destination to draw image</param>
        /// <param name="objROI">ROI</param>
        /// <param name="intLowThreshold">low threshold</param>
        /// <param name="intHighThreshold">high threshold</param>
        public static void DrawHistogram(Graphics g, ROI objROI, int intLowThreshold, int intHighThreshold)
        {
            //DrawHistogram(g, objROI);
            //float fX = intLowThreshold * 232 / 255;    // 232 = histogram graph width
            //g.DrawLine(new Pen(Color.Blue), fX + 51f, 30f, fX + 51f, 130f);
            //fX = intHighThreshold * 232 / 255;
            //g.DrawLine(new Pen(Color.Blue), fX + 51f, 30f, fX + 51f, 130f);


            int intHighestElement = DrawHistogram(g, objROI);
            // 29-05-2019 ZJYEOH: Max of the graph will be 241 no matter intHighestElement is what value, Min value for intHighestElement less than 10 will be 31 then for each 10 times of intHighestElement, Min value will increase by 7 
            // Get histogram graph width
            int intGraphWidth = 210;//240
            int intGraphOffSet = 0;
            if (intHighestElement >= 100000)
                intGraphWidth = 175; //208
            else if (intHighestElement >= 10000)
                intGraphWidth = 182;//216
            else if (intHighestElement >= 1000)
                intGraphWidth = 189;//224
            else if (intHighestElement >= 100)
                intGraphWidth = 196;//232
            else if (intHighestElement >= 10)
                intGraphWidth = 203;
            intGraphOffSet = 210 - intGraphWidth;//232

            float fX = intGraphOffSet + intLowThreshold * intGraphWidth / 255;
            g.DrawLine(new Pen(Color.Blue), fX + 31f, 30f, fX + 31f, 130f);//51f
            fX = intGraphOffSet + intHighThreshold * intGraphWidth / 255;
            g.DrawLine(new Pen(Color.Blue), fX + 31f, 30f, fX + 31f, 130f);//51f
        }

        public static void DrawSubtractImage(ImageDrawing objSource1, ImageDrawing objSource2, ImageDrawing objDest,
          ROI objROI)
        {

            EROIBW8 objTrainROI;
            EROIBW8 objTrainSource1 = new EROIBW8();
            EROIBW8 objTrainSource2 = new EROIBW8();

            int intPositionX = objROI.ref_ROIPositionX;
            int intPositionY = objROI.ref_ROIPositionY;
            int intWidth = objROI.ref_ROIWidth;
            int intHeight = objROI.ref_ROIHeight;

            objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(intPositionX, intPositionY, intWidth, intHeight);
            objTrainROI.Detach();
            objTrainROI.Attach(objDest.ref_objMainImage);
            EasyImage.Copy(new EBW8(0), objDest.ref_objMainImage);

            objTrainSource1 = new EROIBW8();
            objTrainSource1.SetPlacement(intPositionX, intPositionY, intWidth, intHeight);
            objTrainSource1.Detach();
            objTrainSource1.Attach(objSource1.ref_objMainImage);

            objTrainSource2 = new EROIBW8();
            objTrainSource2.SetPlacement(intPositionX, intPositionY, intWidth, intHeight);
            objTrainSource2.Detach();
            objTrainSource2.Attach(objSource2.ref_objMainImage);

            EasyImage.Oper(EArithmeticLogicOperation.Subtract, objSource1.ref_objMainImage, objTrainSource2, objTrainROI);
        }

        public static void DrawSubtractImage(ImageDrawing objSource1, ImageDrawing objSource2, ImageDrawing objDest,
            List<List<ROI>> arrROIs)
        {
            EROIBW8 objTrainROI;
            EROIBW8 objTrainSource1 = new EROIBW8();
            EROIBW8 objTrainSource2 = new EROIBW8();

            for (int i = 0; i < arrROIs.Count; i++)   // which unit it belong to
            {
                if (arrROIs[i].Count > 1)
                {
                    int intPositionX = arrROIs[i][1].ref_ROITotalX;
                    int intPositionY = arrROIs[i][1].ref_ROITotalY;
                    int intWidth = arrROIs[i][1].ref_ROIWidth;
                    int intHeight = arrROIs[i][1].ref_ROIHeight;

                    objTrainROI = new EROIBW8();
                    objTrainROI.SetPlacement(intPositionX, intPositionY, intWidth, intHeight);
                    objTrainROI.Detach();
                    objTrainROI.Attach(objDest.ref_objMainImage);
                    EasyImage.Copy(new EBW8(0), objDest.ref_objMainImage);

                    objTrainSource1 = new EROIBW8();
                    objTrainSource1.SetPlacement(intPositionX, intPositionY, intWidth, intHeight);
                    objTrainSource1.Detach();
                    objTrainSource1.Attach(objSource1.ref_objMainImage);

                    objTrainSource2 = new EROIBW8();
                    objTrainSource2.SetPlacement(intPositionX, intPositionY, intWidth, intHeight);
                    objTrainSource2.Detach();
                    objTrainSource2.Attach(objSource2.ref_objMainImage);

                    EasyImage.Oper(EArithmeticLogicOperation.Subtract, objTrainSource1, objTrainSource2, objTrainROI);
                }
            }
        }

        /// <summary>
        /// Rotate image to less than 90 degree of rotation angle
        /// </summary>
        /// <param name="objSourceImage">source image</param>
        /// <param name="fRotateAngle">rotation angle</param>
        /// <param name="objRotatedImage">destination rotated image</param>
        public static void Rotate0Degree(ImageDrawing objSourceImage, float fRotateAngle, ref List<ImageDrawing> arrRotatedImage, int intImageIndex)
        {
            ImageDrawing objLocalSourceImage = new ImageDrawing();
            objSourceImage.CopyTo(ref objLocalSourceImage);     // Copy source image to another new local image to prevent rotated image blur (Happen when source image object == rotated image object)

            float fCenterX = objSourceImage.ref_intImageWidth / 2f;
            float fCenterY = objSourceImage.ref_intImageHeight / 2f;

            EasyImage.ScaleRotate(objLocalSourceImage.ref_objMainImage, fCenterX, fCenterY, fCenterX, fCenterY, 1, 1, fRotateAngle, arrRotatedImage[intImageIndex].ref_objMainImage, 4);
            objLocalSourceImage.Dispose();
        }

        /// <summary>
        /// Rotate image with source image and destination image must be different
        /// </summary>
        /// <param name="objSourceImage"></param>
        /// <param name="fRotateAngle"></param>
        /// <param name="arrRotatedImage"></param>
        /// <param name="intImageIndex"></param>
        public static void Rotate0Degree_Diff(ImageDrawing objSourceImage, float fRotateAngle, ref List<ImageDrawing> arrRotatedImage, int intImageIndex)
        {
            float fCenterX = objSourceImage.ref_intImageWidth / 2f;
            float fCenterY = objSourceImage.ref_intImageHeight / 2f;

            if (fRotateAngle == 180)
                EasyImage.ScaleRotate(objSourceImage.ref_objMainImage, fCenterX, fCenterY, fCenterX, fCenterY, 1, 1, fRotateAngle, arrRotatedImage[intImageIndex].ref_objMainImage, 0); 
            else
                EasyImage.ScaleRotate(objSourceImage.ref_objMainImage, fCenterX, fCenterY, fCenterX, fCenterY, 1, 1, fRotateAngle, arrRotatedImage[intImageIndex].ref_objMainImage, 4);
        }

        /// <summary>
        /// Copy source image to destination image by resizing the image
        /// </summary>
        /// <param name="objImageDest">destination image to be drawn</param>
        public void CopyTo(ref ImageDrawing objImageDest)
        {
            if ((objImageDest.ref_intImageWidth != m_objMainImage.Width) || (objImageDest.ref_intImageHeight != m_objMainImage.Height))
                objImageDest.ref_objMainImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);
            EasyImage.Copy(m_objMainImage, objImageDest.ref_objMainImage);
            
        }

        public void CopyTo(ref ROI objROIDest)
        {
            if ((objROIDest.ref_ROIWidth != m_objMainImage.Width) || (objROIDest.ref_ROIHeight != m_objMainImage.Height))
                objROIDest.LoadROISetting(0, 0, m_objMainImage.Width, m_objMainImage.Height);

            EasyImage.Copy(m_objMainImage, objROIDest.ref_ROI);
        }

        public void CopyTo(ref List<ImageDrawing> arrImageDest, int intImageIndex)
        {
            if (arrImageDest.Count <= intImageIndex)
                return;

            arrImageDest[intImageIndex].ref_objMainImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);
            EasyImage.Copy(m_objMainImage, arrImageDest[intImageIndex].ref_objMainImage);
        }

        /// <summary>
        /// Copy source image to destination image by resizing the image
        /// </summary>
        /// <param name="objImageDest">destination image to be drawn</param>
        public void CopyTo(ImageDrawing objImageDest)
        {
            objImageDest.ref_objMainImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);
            EasyImage.Copy(m_objMainImage, objImageDest.ref_objMainImage);
        }



        /// <summary>
        /// Copy source image to destination image by resizing the image
        /// </summary>
        /// <param name="objImageDest">destination image to be drawn</param>
        public void Copy(ref EImageBW8 imgDest)
        {
            imgDest.SetSize(m_objMainImage.Width, m_objMainImage.Height);
            EasyImage.Copy(m_objMainImage, imgDest);
        }

        public void AddGain(ref ImageDrawing objImageDest, float fGain)
        {
            if ((objImageDest.ref_intImageWidth != m_objMainImage.Width) || (objImageDest.ref_intImageHeight != m_objMainImage.Height))
                objImageDest.SetImageSize(m_objMainImage.Width, m_objMainImage.Height);

            EasyImage.GainOffset(m_objMainImage, objImageDest.ref_objMainImage, fGain);
        }
        public void AddGain(ref List<ImageDrawing> arrImageDest, int intImageIndex, float fGain)
        {
            if (arrImageDest.Count <= intImageIndex)
                return;

            if ((arrImageDest[intImageIndex].ref_intImageWidth != m_objMainImage.Width) || (arrImageDest[intImageIndex].ref_intImageHeight != m_objMainImage.Height))
                arrImageDest[intImageIndex].SetImageSize(m_objMainImage.Width, m_objMainImage.Height);

            EasyImage.GainOffset(m_objMainImage, arrImageDest[intImageIndex].ref_objMainImage, fGain);
        }

        public void AddGain(float fGain)
        {
            if (fGain != 1f)
                EasyImage.GainOffset(m_objMainImage, m_objMainImage, fGain);
        }

        public void AddPrewitt(ref ImageDrawing objImageDest)
        {
            if ((objImageDest.ref_intImageWidth != m_objMainImage.Width) || (objImageDest.ref_intImageHeight != m_objMainImage.Height))
                objImageDest.SetImageSize(m_objMainImage.Width, m_objMainImage.Height);

            EasyImage.ConvolPrewitt(m_objMainImage, objImageDest.ref_objMainImage);
        }


        public void Thresholding(ref ImageDrawing objImageDest, int intThresholdValue)
        {
#if (Debug_2_12 || Release_2_12)
            EasyImage.Threshold(m_objMainImage, objImageDest.ref_objMainImage, (uint)intThresholdValue);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            EasyImage.Threshold(m_objMainImage, objImageDest.ref_objMainImage, intThresholdValue);
#endif

        }

        /// <summary>
        /// Set pixel on image in order to draw a straight line
        /// </summary>
        /// <param name="bCutMode">true = set black pixel, false = set white pixel</param>
        /// <param name="blnWhiteOnBlack">true = white on black, false = black on white</param>
        /// <param name="intOriginX">point X</param>
        /// <param name="intOriginY">point Y</param>
        public void DrawLine(bool bCutMode, bool blnWhiteOnBlack, int intOriginX, int intOriginY)
        {
            if (blnWhiteOnBlack)
            {
                if (bCutMode)
                    m_bw.Value = 0; //Black color
                else
                    m_bw.Value = 255;
            }
            else
            {
                if (bCutMode)
                    m_bw.Value = 255;
                else
                    m_bw.Value = 0;
            }

            m_objMainImage.SetPixel(m_bw, intOriginX, intOriginY);
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
            m_objZoomImage = new EImageBW8(Convert.ToInt32(m_objMainImage.Width * fZoom), Convert.ToInt32(m_objMainImage.Height * fZoom));
            m_objZoomROI = new EROIBW8();

            EasyImage.ScaleRotate(m_objMainImage, (m_objMainImage.Width) / 2, (m_objMainImage.Height) / 2,
                (m_objZoomImage.Width) / 2, (m_objZoomImage.Height) / 2, fZoom, fZoom, 0.0f, m_objZoomImage);
            m_objZoomROI.Detach();
            m_objZoomROI.Attach(m_objZoomImage);
            m_objZoomROI.SetPlacement(intHScrollBar, intVScrollBar, (m_objMainImage.Width), (m_objMainImage.Height));

            m_objZoomROI.Draw(g);
            m_fScale = fZoom;
        }
        public void DrawBorderLine(ROI objROI, bool blnWhiteOnBlack)
        {
            if (blnWhiteOnBlack)
            {
                m_bw.Value = 0; //Black color
            }
            else
            {
                m_bw.Value = 255;
            }

            int intStartX = objROI.ref_ROITotalX;
            int intStartY = objROI.ref_ROITotalY;
            int intEndX = objROI.ref_ROITotalX + objROI.ref_ROIWidth - 1;
            int intEndY = objROI.ref_ROITotalY + objROI.ref_ROIHeight - 1;
            for (int x = intStartX; x <= intEndX; x++)
            {
                m_objMainImage.SetPixel(m_bw, x, intStartY);
                m_objMainImage.SetPixel(m_bw, x, intEndY);
            }

            for (int y = intStartY; y <= intEndY; y++)
            {
                m_objMainImage.SetPixel(m_bw, intStartX, y);
                m_objMainImage.SetPixel(m_bw, intEndX, y);
            }
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
        /// Load Image to Picture Box from CPU memory address
        /// </summary>
        /// <param name="intImageAddress">CPU memory address</param>
        /// <param name="g">destination to draw the image</param>
        public void LoadImageFromMemory(IntPtr intImageAddress)
        {
            m_objMainImage.SetImagePtr(m_objMainImage.Width, m_objMainImage.Height, intImageAddress);
            m_intImageWidth = m_objMainImage.Width;
            m_intImageHeight = m_objMainImage.Height;
        }

        public void LoadImageFromMemory(IntPtr intImageAddress, Bitmap bmp)
        {
            m_objMainImage.SetSize(bmp.Width, bmp.Height);
            m_objMainImage.SetImagePtr(bmp.Width, bmp.Height, intImageAddress, 24 * bmp.Width);

            m_intImageWidth = m_objMainImage.Width;
            m_intImageHeight = m_objMainImage.Height;
        }

        /// <summary>
        /// Load Image to picture box from selected path
        /// </summary>
        /// <param name="strPath">selected path</param>
        /// <param name="g">destination to draw the image</param>
        public void LoadImage(string strPath)
        {
            if (!File.Exists(strPath))
                return;

            m_objMainImage.Load(strPath);

            m_intImageWidth = m_objMainImage.Width;
            m_intImageHeight = m_objMainImage.Height;

            //if (m_intImageWidth < m_intPictureBoxWidth || m_intImageHeight < m_intPitureBoxHeight)
            //{
            //    EImageBW8 objParent = new EImageBW8();
            //    objParent.SetSize(m_intPictureBoxWidth, m_intPitureBoxHeight);
            //    EasyImage.Copy(new EBW8(0), objParent);

            //    EROIBW8 objChild = new EROIBW8();
            //    objChild.SetPlacement((m_intPictureBoxWidth / 2) - (m_intImageWidth / 2), (m_intPitureBoxHeight / 2) -
            //        (m_intImageHeight / 2), m_objMainImage.Width, m_objMainImage.Height);
            //    objChild.Attach(objParent);
            //    EasyImage.Copy(m_objMainImage, objChild);

            //    m_objMainImage.SetSize(m_intPictureBoxWidth, m_intPitureBoxHeight);         // = new EImageBW8(m_intPictureBoxWidth, m_intPitureBoxHeight);
            //    if (m_objZoomImage != null)
            //        m_objZoomImage.SetSize(m_intPictureBoxWidth, m_intPitureBoxHeight);         // = new EImageBW8(m_intPictureBoxWidth, m_intPitureBoxHeight);
            //    if (m_objThresholdImage != null)
            //        m_objThresholdImage.SetSize(m_intPictureBoxWidth, m_intPitureBoxHeight);    // = new EImageBW8(m_intPictureBoxWidth, m_intPitureBoxHeight);
            //    EasyImage.Copy(objParent, m_objMainImage);

            //    objParent.Dispose();
            //}
            //m_objMainImage.Load(strPath);

            //m_intImageWidth = m_objMainImage.Width;
            //m_intImageHeight = m_objMainImage.Height;
        }

        public void LoadImage_CopyToTempFolderFirst(string strPath)
        {
            if (!File.Exists(strPath))
                return;

            string strTempFolderDirectory = "C:\\TempFolder\\";
            if (!Directory.Exists(strTempFolderDirectory))
                Directory.CreateDirectory(strTempFolderDirectory);

            File.Copy(strPath, strTempFolderDirectory + "TempImage.bmp", true);
            m_objMainImage.Load(strTempFolderDirectory + "TempImage.bmp");

            try
            {
                File.Delete(strTempFolderDirectory + "TempImage.bmp");
            }
            catch
            {
            }

            try
            {
                Directory.Delete(strTempFolderDirectory);
            }
            catch
            {

            }
            m_intImageWidth = m_objMainImage.Width;
            m_intImageHeight = m_objMainImage.Height;
        }

        public void LoadImage_AdjustByCameraResolution(string strPath)
        {
            if (!File.Exists(strPath))
                return;

            m_objMainImage.Load(strPath);

            m_intImageWidth = m_objMainImage.Width;
            m_intImageHeight = m_objMainImage.Height;

            if (m_intImageWidth < m_intCameraResolutionWidth && m_intImageHeight < m_intCameraResolutionHeight)
            {
                EImageBW8 objParent = new EImageBW8();
                objParent.SetSize(m_intCameraResolutionWidth, m_intCameraResolutionHeight);
                EasyImage.Copy(new EBW8(0), objParent);

                EROIBW8 objChild = new EROIBW8();

                objChild.SetPlacement((m_intCameraResolutionWidth / 2) - (m_intImageWidth / 2), (m_intCameraResolutionHeight / 2) -
                    (m_intImageHeight / 2), m_objMainImage.Width, m_objMainImage.Height);
                objChild.Attach(objParent);
                EasyImage.Copy(m_objMainImage, objChild);

                m_objMainImage.SetSize(m_intCameraResolutionWidth, m_intCameraResolutionHeight);         // = new EImageBW8(m_intCameraResolutionWidth, m_intCameraResolutionHeight);
                if (m_objZoomImage != null)
                    m_objZoomImage.SetSize(m_intCameraResolutionWidth, m_intCameraResolutionHeight);         // = new EImageBW8(m_intCameraResolutionWidth, m_intCameraResolutionHeight);
                if (m_objThresholdImage != null)
                    m_objThresholdImage.SetSize(m_intCameraResolutionWidth, m_intCameraResolutionHeight);    // = new EImageBW8(m_intCameraResolutionWidth, m_intCameraResolutionHeight);
                EasyImage.Copy(objParent, m_objMainImage);

                objParent.Dispose();
            }
            //m_objMainImage.Load(strPath);

            //m_intImageWidth = m_objMainImage.Width;
            //m_intImageHeight = m_objMainImage.Height;
        }
        public void RedrawImage(Graphics g, float ScaleX, float ScaleY)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
                m_objMainImage.Draw(g, ScaleX, ScaleY);
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                m_objZoomROI.Draw(g, ScaleX, ScaleY);
            }
        }
        /// <summary>
        /// When refresh page, call this function to redraw the picture box
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        public void RedrawImage(Graphics g)
        {
            if (m_objMainImage == null)
                return;
            
                m_objMainImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
         
        }

        /// <summary>
        /// When refresh page, call this function to redraw the picture box while merge image
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        /// <param name="arrROIs">ROI</param>
        /// <param name="intType">1=combine grab 1&2, 2=combine all</param>
        public void RedrawImage(Graphics g, List<ROI> arrROIs, int intType)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if (m_objMainImage.Width != m_objMergeImage.Width || m_objMainImage.Height != m_objMergeImage.Height)
                    m_objMergeImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                EasyImage.Copy(m_objMainImage, m_objMergeImage);
            }

            EROIBW8 objTrainROI;
            switch (intType)
            {
                case 1:
                    objTrainROI = new EROIBW8();
                    objTrainROI.SetPlacement(((ROI)arrROIs[0]).ref_ROITotalX, ((ROI)arrROIs[0]).ref_ROITotalY,
                        ((ROI)arrROIs[0]).ref_ROIWidth, ((ROI)arrROIs[0]).ref_ROIHeight);

                    objTrainROI.Detach();
                    objTrainROI.Attach(m_objMergeImage);

                    if (arrROIs[0].ref_ROIPositionX >= 0 && arrROIs[0].ref_ROIPositionY >= 0)
                    {
                        EasyImage.Copy(arrROIs[0].ref_ROI, objTrainROI);
                    }
                    break;
                case 2:
                    HiPerfTimer timer = new HiPerfTimer();
                    timer.Start();
                    for (int i = arrROIs.Count - 1; i >= 0; i--)
                    {
                        objTrainROI = new EROIBW8();
                        objTrainROI.SetPlacement(((ROI)arrROIs[i]).ref_ROITotalX, ((ROI)arrROIs[i]).ref_ROITotalY,
                            ((ROI)arrROIs[i]).ref_ROIWidth, ((ROI)arrROIs[i]).ref_ROIHeight);

                        objTrainROI.Detach();
                        objTrainROI.Attach(m_objMergeImage);

                        if (arrROIs[i].ref_ROIPositionX >= 0 && arrROIs[i].ref_ROIPositionY >= 0)
                            EasyImage.Copy(arrROIs[i].ref_ROI, objTrainROI);
                    }
                    timer.Stop();
                    if (timer.Duration > 0.5f)
                        STTrackLog.WriteLine("time = " + timer.Duration.ToString());
                    break;
            }

            m_objMergeImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }

        /// <summary>
        /// When refresh page, call this function to redraw the picture box
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        /// <param name="intThresholdValue">threshold value</param>
        /// <param name="arrROIs">ROI</param>
        public void RedrawImage(Graphics g, int intThresholdValue, List<ROI> arrROIs)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }

            EROIBW8 objTrainROI;

            for (int i = 0; i < arrROIs.Count; i++)
            {
                objTrainROI = new EROIBW8();
                objTrainROI.SetPlacement(((ROI)arrROIs[i]).ref_ROITotalX, ((ROI)arrROIs[i]).ref_ROITotalY,
                    ((ROI)arrROIs[i]).ref_ROIWidth, ((ROI)arrROIs[i]).ref_ROIHeight);

                objTrainROI.Detach();
                objTrainROI.Attach(m_objThresholdImage);
#if (Debug_2_12 || Release_2_12)
                if (m_fScale == 1.0f)
                {
                    EasyImage.Threshold(((ROI)arrROIs[i]).ref_ROI, objTrainROI, (uint)intThresholdValue);
                }
                else
                {
                    m_objZoomROI.Detach();
                    m_objZoomROI.Attach(m_objZoomImage);
                    EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
                }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (m_fScale == 1.0f)
                {
                    EasyImage.Threshold(((ROI)arrROIs[i]).ref_ROI, objTrainROI, intThresholdValue);
                }
                else
                {
                    m_objZoomROI.Detach();
                    m_objZoomROI.Attach(m_objZoomImage);
                    EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
                }
#endif

            }

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }

        /// <summary>
        /// When refresh page, call this function to redraw the picture box
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        /// <param name="intThresholdValue">threshold value</param>
        /// <param name="arrROIs">ROI</param>
        public void RedrawImage(Graphics g, int intThresholdValue, List<List<ROI>> arrROIs)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                EasyImage.Copy(m_objMainImage, m_objThresholdImage);

            }
             EROIBW8 objSourceROI;
            EROIBW8 objDestinationROI;
            for (int i = 0; i < arrROIs.Count; i++)
            {
                objSourceROI = new EROIBW8();
                objSourceROI.SetPlacement(((ROI)arrROIs[i][0]).ref_ROIPositionX,
                    ((ROI)arrROIs[i][0]).ref_ROIPositionY,
                    ((ROI)arrROIs[i][0]).ref_ROIWidth, ((ROI)arrROIs[i][0]).ref_ROIHeight);

                objSourceROI.Detach();
                objSourceROI.Attach(m_objMainImage);
                
                objDestinationROI = new EROIBW8();
                objDestinationROI.SetPlacement(((ROI)arrROIs[i][0]).ref_ROIPositionX,
                    ((ROI)arrROIs[i][0]).ref_ROIPositionY,
                    ((ROI)arrROIs[i][0]).ref_ROIWidth, ((ROI)arrROIs[i][0]).ref_ROIHeight);

                objDestinationROI.Detach();
                objDestinationROI.Attach(m_objThresholdImage);
#if (Debug_2_12 || Release_2_12)
                if (m_fScale == 1.0f)
                {
                    if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                        m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                    EasyImage.Threshold(objSourceROI, objDestinationROI, (uint)intThresholdValue);
                }
                else
                {
                    m_objZoomROI.Detach();
                    m_objZoomROI.Attach(m_objZoomImage);
                    EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
                }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (m_fScale == 1.0f)
                {
                    if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                        m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                    EasyImage.Threshold(objSourceROI, objDestinationROI, intThresholdValue);
                }
                else
                {
                    m_objZoomROI.Detach();
                    m_objZoomROI.Attach(m_objZoomImage);
                    EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
                }
#endif

            }

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);          
        }
        public void RedrawImage(Graphics g, int intThresholdValue, int intSelectedUnit, List<List<ROI>> arrROIs)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                EasyImage.Copy(m_objMainImage, m_objThresholdImage);

            }
            EROIBW8 objSourceROI;
            EROIBW8 objDestinationROI;
            for (int i = 0; i < arrROIs.Count; i++)
            {
                if (i != intSelectedUnit)
                    continue;

                objSourceROI = new EROIBW8();
                objSourceROI.SetPlacement(((ROI)arrROIs[i][0]).ref_ROIPositionX,
                    ((ROI)arrROIs[i][0]).ref_ROIPositionY,
                    ((ROI)arrROIs[i][0]).ref_ROIWidth, ((ROI)arrROIs[i][0]).ref_ROIHeight);

                objSourceROI.Detach();
                objSourceROI.Attach(m_objMainImage);

                objDestinationROI = new EROIBW8();
                objDestinationROI.SetPlacement(((ROI)arrROIs[i][0]).ref_ROIPositionX,
                    ((ROI)arrROIs[i][0]).ref_ROIPositionY,
                    ((ROI)arrROIs[i][0]).ref_ROIWidth, ((ROI)arrROIs[i][0]).ref_ROIHeight);

                objDestinationROI.Detach();
                objDestinationROI.Attach(m_objThresholdImage);
#if (Debug_2_12 || Release_2_12)
                if (m_fScale == 1.0f)
                {
                    if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                        m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                    EasyImage.Threshold(objSourceROI, objDestinationROI, (uint)intThresholdValue);
                }
                else
                {
                    m_objZoomROI.Detach();
                    m_objZoomROI.Attach(m_objZoomImage);
                    EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
                }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (m_fScale == 1.0f)
                {
                    if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                        m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                    EasyImage.Threshold(objSourceROI, objDestinationROI, intThresholdValue);
                }
                else
                {
                    m_objZoomROI.Detach();
                    m_objZoomROI.Attach(m_objZoomImage);
                    EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
                }
#endif

            }

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        /// <summary>
        /// When refresh page, call this function to redraw the picture box
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        /// <param name="intThresholdValue">threshold value</param>
        /// <param name="arrROIs">ROI</param>
        /// <param name="intROIIndex">ROI index</param>
        public void RedrawImage(Graphics g, int intThresholdValue, List<List<ROI>> arrROIs, int intROIIndex)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }
            EROIBW8 objTrainROI;
            for (int i = 0; i < arrROIs.Count; i++)
            {
                objTrainROI = new EROIBW8();
                if (intROIIndex == 0)
                {
                    objTrainROI.SetPlacement(arrROIs[i][0].ref_ROIPositionX,
                        arrROIs[i][0].ref_ROIPositionY,
                        arrROIs[i][0].ref_ROIWidth, arrROIs[i][0].ref_ROIHeight);
                }
                else
                {
                    objTrainROI.SetPlacement(arrROIs[i][0].ref_ROIPositionX + arrROIs[i][1].ref_ROIPositionX,
                     arrROIs[i][0].ref_ROIPositionY + arrROIs[i][1].ref_ROIPositionY,
                     arrROIs[i][1].ref_ROIWidth, arrROIs[i][1].ref_ROIHeight);
                }

                objTrainROI.Detach();
                objTrainROI.Attach(m_objThresholdImage);
#if (Debug_2_12 || Release_2_12)
                if (m_fScale == 1.0f)
                {
                    arrROIs[i][1].AttachImage(arrROIs[i][0]);
                    EasyImage.Threshold(((ROI)arrROIs[i][intROIIndex]).ref_ROI, objTrainROI, (uint)intThresholdValue);
                }
                else
                {
                    m_objZoomROI.Detach();
                    m_objZoomROI.Attach(m_objZoomImage);
                    EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
                }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (m_fScale == 1.0f)
                {
                    arrROIs[i][1].AttachImage(arrROIs[i][0]);
                    EasyImage.Threshold(((ROI)arrROIs[i][intROIIndex]).ref_ROI, objTrainROI, intThresholdValue);
                }
                else
                {
                    m_objZoomROI.Detach();
                    m_objZoomROI.Attach(m_objZoomImage);
                    EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
                }
#endif

            }

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }

        /// <summary>
        /// When refresh page, call this function to redraw the picture box
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        /// <param name="intThresholdValue">threshold value</param>
        /// <param name="arrROIs">ROI</param>
        /// <param name="intROIIndex">ROI index</param>
        public void RedrawImage(Graphics g, int intThresholdValue, List<ROI> arrROIs, int intROIIndex)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }

            EROIBW8 objTrainROI;

            objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(arrROIs[intROIIndex].ref_ROITotalX, arrROIs[intROIIndex].ref_ROITotalY, 
                arrROIs[intROIIndex].ref_ROIWidth, arrROIs[intROIIndex].ref_ROIHeight);
          
            objTrainROI.Detach();
            objTrainROI.Attach(m_objThresholdImage);
#if (Debug_2_12 || Release_2_12)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(arrROIs[intROIIndex].ref_ROI, objTrainROI, (uint)intThresholdValue);

            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(arrROIs[intROIIndex].ref_ROI, objTrainROI, intThresholdValue);
                
            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
            }
#endif


            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        public void RedrawThresholdImage(Graphics g, int intThresholdValue, List<ROI> arrROIs, int intSelectedROI, bool blnDrawAllSide)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }
            if (blnDrawAllSide)
            {
                EROIBW8 objTrainROI2;

                objTrainROI2 = new EROIBW8();
                for (int i = 1; i < arrROIs.Count; i++)
                {
                    if (i != intSelectedROI)
                    {
                        objTrainROI2.SetPlacement(arrROIs[i].ref_ROITotalX, arrROIs[i].ref_ROITotalY,
                        arrROIs[i].ref_ROIWidth, arrROIs[i].ref_ROIHeight);

                        objTrainROI2.Detach();
                        objTrainROI2.Attach(m_objThresholdImage);
#if (Debug_2_12 || Release_2_12)
                        if (m_fScale == 1.0f)
                        {
                            EasyImage.Threshold(arrROIs[i].ref_ROI, objTrainROI2, (uint)intThresholdValue);

                        }
                        else
                        {
                            m_objZoomROI.Detach();
                            m_objZoomROI.Attach(m_objZoomImage);
                            EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
                        }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        if (m_fScale == 1.0f)
                        {
                            EasyImage.Threshold(arrROIs[i].ref_ROI, objTrainROI2, intThresholdValue);

                        }
                        else
                        {
                            m_objZoomROI.Detach();
                            m_objZoomROI.Attach(m_objZoomImage);
                            EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
                        }
#endif

                    }
                }
            }
            EROIBW8 objTrainROI;

            objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(arrROIs[intSelectedROI].ref_ROITotalX, arrROIs[intSelectedROI].ref_ROITotalY,
                arrROIs[intSelectedROI].ref_ROIWidth, arrROIs[intSelectedROI].ref_ROIHeight);

            objTrainROI.Detach();
            objTrainROI.Attach(m_objThresholdImage);
#if (Debug_2_12 || Release_2_12)
            if(m_fScale == 1.0f)
            {
                EasyImage.Threshold(arrROIs[intSelectedROI].ref_ROI, objTrainROI, (uint)intThresholdValue);

            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(arrROIs[intSelectedROI].ref_ROI, objTrainROI, intThresholdValue);

            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
            }
#endif

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        public void RedrawThresholdImage(Graphics g, int intThresholdValue, List<List<ROI>> arrROIs, int intROIIndex,int intSelectedROI, bool blnDrawAllSide) 
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }
            if (blnDrawAllSide)
            {
                EROIBW8 objTrainROI2;

                objTrainROI2 = new EROIBW8();
                for (int i = 1; i < arrROIs.Count; i++)
                {
                    if (i != intSelectedROI)
                    {
                        objTrainROI2.SetPlacement(arrROIs[i][intROIIndex].ref_ROITotalX, arrROIs[i][intROIIndex].ref_ROITotalY,
                        arrROIs[i][intROIIndex].ref_ROIWidth, arrROIs[i][intROIIndex].ref_ROIHeight);

                        objTrainROI2.Detach();
                        objTrainROI2.Attach(m_objThresholdImage);
#if (Debug_2_12 || Release_2_12)
                        if (m_fScale == 1.0f)
                        {
                            EasyImage.Threshold(arrROIs[i][intROIIndex].ref_ROI, objTrainROI2, (uint)intThresholdValue);

                        }
                        else
                        {
                            m_objZoomROI.Detach();
                            m_objZoomROI.Attach(m_objZoomImage);
                            EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
                        }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        if (m_fScale == 1.0f)
                        {
                            EasyImage.Threshold(arrROIs[i][intROIIndex].ref_ROI, objTrainROI2, intThresholdValue);

                        }
                        else
                        {
                            m_objZoomROI.Detach();
                            m_objZoomROI.Attach(m_objZoomImage);
                            EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
                        }
#endif

                    }
                }
            }
            EROIBW8 objTrainROI;

            objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(arrROIs[intSelectedROI][intROIIndex].ref_ROITotalX, arrROIs[intSelectedROI][intROIIndex].ref_ROITotalY,
                arrROIs[intSelectedROI][intROIIndex].ref_ROIWidth, arrROIs[intSelectedROI][intROIIndex].ref_ROIHeight);

            objTrainROI.Detach();
            objTrainROI.Attach(m_objThresholdImage);
#if (Debug_2_12 || Release_2_12)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(arrROIs[intSelectedROI][intROIIndex].ref_ROI, objTrainROI, (uint)intThresholdValue);

            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(arrROIs[intSelectedROI][intROIIndex].ref_ROI, objTrainROI, intThresholdValue);

            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
            }
#endif

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        public void RedrawThresholdImageForLead(Graphics g, int intThresholdValue, List<List<ROI>> arrROIs, int intROIIndex, int intSelectedROI, bool blnDrawAllSide, Lead[] arrLead)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }
            if (blnDrawAllSide)
            {
                EROIBW8 objTrainROI2;

                objTrainROI2 = new EROIBW8();
                for (int i = 1; i < arrROIs.Count; i++)
                {
                    if(arrLead[i].ref_blnSelected)
                    if (i != intSelectedROI)
                    {
                        objTrainROI2.SetPlacement(arrROIs[i][intROIIndex].ref_ROITotalX, arrROIs[i][intROIIndex].ref_ROITotalY,
                        arrROIs[i][intROIIndex].ref_ROIWidth, arrROIs[i][intROIIndex].ref_ROIHeight);

                        objTrainROI2.Detach();
                        objTrainROI2.Attach(m_objThresholdImage);
#if (Debug_2_12 || Release_2_12)
                            if (m_fScale == 1.0f)
                            {
                                EasyImage.Threshold(arrROIs[i][intROIIndex].ref_ROI, objTrainROI2, (uint)intThresholdValue);

                            }
                            else
                            {
                                m_objZoomROI.Detach();
                                m_objZoomROI.Attach(m_objZoomImage);
                                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
                            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        if (m_fScale == 1.0f)
                        {
                            EasyImage.Threshold(arrROIs[i][intROIIndex].ref_ROI, objTrainROI2, intThresholdValue);

                        }
                        else
                        {
                            m_objZoomROI.Detach();
                            m_objZoomROI.Attach(m_objZoomImage);
                            EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
                        }
#endif

                        }
                }
            }
            EROIBW8 objTrainROI;

            objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(arrROIs[intSelectedROI][intROIIndex].ref_ROITotalX, arrROIs[intSelectedROI][intROIIndex].ref_ROITotalY,
                arrROIs[intSelectedROI][intROIIndex].ref_ROIWidth, arrROIs[intSelectedROI][intROIIndex].ref_ROIHeight);

            objTrainROI.Detach();
            objTrainROI.Attach(m_objThresholdImage);
#if (Debug_2_12 || Release_2_12)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(arrROIs[intSelectedROI][intROIIndex].ref_ROI, objTrainROI, (uint)intThresholdValue);

            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(arrROIs[intSelectedROI][intROIIndex].ref_ROI, objTrainROI, intThresholdValue);

            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
            }
#endif

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        /// <summary>
        /// When refresh page, call this function to redraw the picture box
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        /// <param name="intThresholdValue">threshold value</param>
        /// <param name="objROI">ROI</param>
        public void RedrawImage(Graphics g, int intThresholdValue, ROI objROI)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }

            EROIBW8 objTrainROI;
        
            objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(objROI.ref_ROIPositionX, objROI.ref_ROIPositionY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);

            objTrainROI.Detach();
            objTrainROI.Attach(m_objThresholdImage);
#if (Debug_2_12 || Release_2_12)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(objROI.ref_ROI, objTrainROI, (uint)intThresholdValue);
            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(objROI.ref_ROI, objTrainROI, intThresholdValue);
            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
            }
#endif

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }

        public void RedrawImage(Graphics g, int intThresholdValue)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);

                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }


#if (Debug_2_12 || Release_2_12)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(m_objMainImage, m_objThresholdImage, (uint)intThresholdValue);
            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(objROI.ref_ROI, objTrainROI, intThresholdValue);
            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
            }
#endif

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }

        public void RedrawImage(Graphics g, int intLowThreshold, int intHighThreshold, ROI objChildROI)
        {
            RedrawImage(g, intLowThreshold, intHighThreshold, objChildROI, 1f);
        }

        public void RedrawImage(Graphics g, int intLowThreshold, int intHighThreshold, ROI objChildROI, byte lowValue, byte middleValue, byte highValue)
        {
            RedrawImage(g, intLowThreshold, intHighThreshold, objChildROI, 1f, lowValue, middleValue, highValue);
        }

        public void RedrawImage(Graphics g, int intLowThreshold, int intHighThreshold, ROI objChildROI, float fGainValue)
        {
            RedrawImage(g, intLowThreshold, intHighThreshold, objChildROI, fGainValue, 0, 255, 0);
        }

        public void RedrawThresholdImage(Graphics g, int intLowThreshold, int intHighThreshold, List<List<ROI>> arrROIs, float fGainValue, byte lowValue, byte middleValue, byte highValue,int intROIIndex, int intSelectedROI, bool blnDrawAllSide)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if (m_objMainImage.Width != m_objThresholdImage.Width || m_objThresholdImage.Height != m_objMainImage.Height)
                {
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);
                }
                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }

            if (fGainValue != 1f)
                EasyImage.GainOffset(m_objThresholdImage, m_objThresholdImage, fGainValue);

            if (blnDrawAllSide)
            {
                EROIBW8 objTrainROI2;

                objTrainROI2 = new EROIBW8();
                for (int i = 1; i < arrROIs.Count; i++)
                {
                    if (i != intSelectedROI)
                    {
                        objTrainROI2.SetPlacement(arrROIs[i][intROIIndex].ref_ROITotalX, arrROIs[i][intROIIndex].ref_ROITotalY,
                        arrROIs[i][intROIIndex].ref_ROIWidth, arrROIs[i][intROIIndex].ref_ROIHeight);

                        objTrainROI2.Detach();
                        objTrainROI2.Attach(m_objThresholdImage);
                        EasyImage.GainOffset(objTrainROI2, objTrainROI2, fGainValue);

                        if (intHighThreshold < 0)
                        {
                            //EBW8 objBW8 = EasyImage.AutoThreshold(objTrainROI, EThresholdMode.MinResidue);
                            intHighThreshold = intLowThreshold;
                        }
#if (Debug_2_12 || Release_2_12)
                        if (m_fScale == 1.0f)
                        {
                            EasyImage.DoubleThreshold(objTrainROI2, objTrainROI2, (uint)intLowThreshold, (uint)intHighThreshold, lowValue, middleValue, highValue);
                        }
                        else
                        {
                            if (m_objThresholdImage.Width != m_objZoomROI.Width)
                            {
                                m_objThresholdImage.SetSize(m_objZoomROI.Width, m_objZoomROI.Height);
                            }
                            m_objZoomROI.Detach();
                            m_objZoomROI.Attach(m_objZoomImage);

                            EasyImage.DoubleThreshold(m_objZoomROI, m_objThresholdImage, (uint)intLowThreshold, (uint)intHighThreshold, lowValue, middleValue, highValue);
                        }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        if (m_fScale == 1.0f)
                        {
                            EasyImage.DoubleThreshold(objTrainROI2, objTrainROI2, intLowThreshold, intHighThreshold, lowValue, middleValue, highValue);
                        }
                        else
                        {
                            if (m_objThresholdImage.Width != m_objZoomROI.Width)
                            {
                                m_objThresholdImage.SetSize(m_objZoomROI.Width, m_objZoomROI.Height);
                            }
                            m_objZoomROI.Detach();
                            m_objZoomROI.Attach(m_objZoomImage);

                            EasyImage.DoubleThreshold(m_objZoomROI, m_objThresholdImage, intLowThreshold, intHighThreshold, lowValue, middleValue, highValue);
                        }
#endif

                    }
                }
            }

            EROIBW8 objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(arrROIs[intSelectedROI][intROIIndex].ref_ROITotalX, arrROIs[intSelectedROI][intROIIndex].ref_ROITotalY, arrROIs[intSelectedROI][intROIIndex].ref_ROIWidth, arrROIs[intSelectedROI][intROIIndex].ref_ROIHeight);
            objTrainROI.Detach();
            objTrainROI.Attach(m_objThresholdImage);
            EasyImage.GainOffset(objTrainROI, objTrainROI, fGainValue);

            if (intHighThreshold < 0)
            {
                //EBW8 objBW8 = EasyImage.AutoThreshold(objTrainROI, EThresholdMode.MinResidue);
                intHighThreshold = intLowThreshold;
            }
#if (Debug_2_12 || Release_2_12)
            if (m_fScale == 1.0f)
            {
                EasyImage.DoubleThreshold(objTrainROI, objTrainROI, (uint)intLowThreshold, (uint)intHighThreshold, lowValue, middleValue, highValue);
            }
            else
            {
                if (m_objThresholdImage.Width != m_objZoomROI.Width)
                {
                    m_objThresholdImage.SetSize(m_objZoomROI.Width, m_objZoomROI.Height);
                }
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);

                EasyImage.DoubleThreshold(m_objZoomROI, m_objThresholdImage, (uint)intLowThreshold, (uint)intHighThreshold, lowValue, middleValue, highValue);
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (m_fScale == 1.0f)
            {
                EasyImage.DoubleThreshold(objTrainROI, objTrainROI, intLowThreshold, intHighThreshold, lowValue, middleValue, highValue);
            }
            else
            {
                if (m_objThresholdImage.Width != m_objZoomROI.Width)
                {
                    m_objThresholdImage.SetSize(m_objZoomROI.Width, m_objZoomROI.Height);
                }
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);

                EasyImage.DoubleThreshold(m_objZoomROI, m_objThresholdImage, intLowThreshold, intHighThreshold, lowValue, middleValue, highValue);
            }
#endif

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        public void RedrawThresholdImage_ForSeal(Graphics g, int intLowThreshold, int intHighThreshold, List<ROI> arrROIs, float fGainValue, byte lowValue, byte middleValue, byte highValue, int intROIIndex, bool blnDrawAllSide)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if (m_objMainImage.Width != m_objThresholdImage.Width || m_objThresholdImage.Height != m_objMainImage.Height)
                {
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);
                }
                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }

            if (fGainValue != 1f)
                EasyImage.GainOffset(m_objThresholdImage, m_objThresholdImage, fGainValue);

            if (blnDrawAllSide)
            {
                EROIBW8 objTrainROI2;

                objTrainROI2 = new EROIBW8();
                for (int i = 0; i < arrROIs.Count; i++)
                {
                    if (i != intROIIndex)
                    {
                        objTrainROI2.SetPlacement(arrROIs[i].ref_ROITotalX, arrROIs[i].ref_ROITotalY,
                        arrROIs[i].ref_ROIWidth, arrROIs[i].ref_ROIHeight);

                        objTrainROI2.Detach();
                        objTrainROI2.Attach(m_objThresholdImage);
                        EasyImage.GainOffset(objTrainROI2, objTrainROI2, fGainValue);

                        if (intHighThreshold < 0)
                        {
                            //EBW8 objBW8 = EasyImage.AutoThreshold(objTrainROI, EThresholdMode.MinResidue);
                            intHighThreshold = intLowThreshold;
                        }
#if (Debug_2_12 || Release_2_12)
                        if (m_fScale == 1.0f)
                        {
                            EasyImage.DoubleThreshold(objTrainROI2, objTrainROI2, (uint)intLowThreshold, (uint)intHighThreshold, lowValue, middleValue, highValue);
                        }
                        else
                        {
                            if (m_objThresholdImage.Width != m_objZoomROI.Width)
                            {
                                m_objThresholdImage.SetSize(m_objZoomROI.Width, m_objZoomROI.Height);
                            }
                            m_objZoomROI.Detach();
                            m_objZoomROI.Attach(m_objZoomImage);

                            EasyImage.DoubleThreshold(m_objZoomROI, m_objThresholdImage, (uint)intLowThreshold, (uint)intHighThreshold, lowValue, middleValue, highValue);
                        }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        if (m_fScale == 1.0f)
                        {
                            EasyImage.DoubleThreshold(objTrainROI2, objTrainROI2, intLowThreshold, intHighThreshold, lowValue, middleValue, highValue);
                        }
                        else
                        {
                            if (m_objThresholdImage.Width != m_objZoomROI.Width)
                            {
                                m_objThresholdImage.SetSize(m_objZoomROI.Width, m_objZoomROI.Height);
                            }
                            m_objZoomROI.Detach();
                            m_objZoomROI.Attach(m_objZoomImage);

                            EasyImage.DoubleThreshold(m_objZoomROI, m_objThresholdImage, intLowThreshold, intHighThreshold, lowValue, middleValue, highValue);
                        }
#endif

                    }
                }
            }

            EROIBW8 objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(arrROIs[intROIIndex].ref_ROITotalX, arrROIs[intROIIndex].ref_ROITotalY, arrROIs[intROIIndex].ref_ROIWidth, arrROIs[intROIIndex].ref_ROIHeight);
            objTrainROI.Detach();
            objTrainROI.Attach(m_objThresholdImage);
            EasyImage.GainOffset(objTrainROI, objTrainROI, fGainValue);

            if (intHighThreshold < 0)
            {
                //EBW8 objBW8 = EasyImage.AutoThreshold(objTrainROI, EThresholdMode.MinResidue);
                intHighThreshold = intLowThreshold;
            }
#if (Debug_2_12 || Release_2_12)
            if (m_fScale == 1.0f)
            {
                EasyImage.DoubleThreshold(objTrainROI, objTrainROI, (uint)intLowThreshold, (uint)intHighThreshold, lowValue, middleValue, highValue);
            }
            else
            {
                if (m_objThresholdImage.Width != m_objZoomROI.Width)
                {
                    m_objThresholdImage.SetSize(m_objZoomROI.Width, m_objZoomROI.Height);
                }
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);

                EasyImage.DoubleThreshold(m_objZoomROI, m_objThresholdImage, (uint)intLowThreshold, (uint)intHighThreshold, lowValue, middleValue, highValue);
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (m_fScale == 1.0f)
            {
                EasyImage.DoubleThreshold(objTrainROI, objTrainROI, intLowThreshold, intHighThreshold, lowValue, middleValue, highValue);
            }
            else
            {
                if (m_objThresholdImage.Width != m_objZoomROI.Width)
                {
                    m_objThresholdImage.SetSize(m_objZoomROI.Width, m_objZoomROI.Height);
                }
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);

                EasyImage.DoubleThreshold(m_objZoomROI, m_objThresholdImage, intLowThreshold, intHighThreshold, lowValue, middleValue, highValue);
            }
#endif

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }

        public void RedrawImage(Graphics g, int intLowThreshold, int intHighThreshold, ROI objChildROI, float fGainValue, byte lowValue, byte middleValue, byte highValue)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if (m_objMainImage.Width != m_objThresholdImage.Width || m_objThresholdImage.Height != m_objMainImage.Height)
                {
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);
                }
                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }

            if (fGainValue != 1f)
                EasyImage.GainOffset(m_objThresholdImage, m_objThresholdImage, fGainValue);

            EROIBW8 objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(objChildROI.ref_ROITotalX, objChildROI.ref_ROITotalY, objChildROI.ref_ROIWidth, objChildROI.ref_ROIHeight);
            objTrainROI.Detach();
            objTrainROI.Attach(m_objThresholdImage);

            if (intHighThreshold < 0)
            {
                //EBW8 objBW8 = EasyImage.AutoThreshold(objTrainROI, EThresholdMode.MinResidue);
                intHighThreshold = intLowThreshold;
            }
#if (Debug_2_12 || Release_2_12)
            if (m_fScale == 1.0f)
            {
                EasyImage.DoubleThreshold(objTrainROI, objTrainROI, (uint)intLowThreshold, (uint)intHighThreshold, lowValue, middleValue, highValue);
            }
            else
            {
                if (m_objThresholdImage.Width != m_objZoomROI.Width)
                {
                    m_objThresholdImage.SetSize(m_objZoomROI.Width, m_objZoomROI.Height);
                }
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);

                EasyImage.DoubleThreshold(m_objZoomROI, m_objThresholdImage, (uint)intLowThreshold, (uint)intHighThreshold, lowValue, middleValue, highValue);
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (m_fScale == 1.0f)
            {
                EasyImage.DoubleThreshold(objTrainROI, objTrainROI, intLowThreshold, intHighThreshold, lowValue , middleValue, highValue);
            }
            else
            {
                if (m_objThresholdImage.Width != m_objZoomROI.Width)
                {
                    m_objThresholdImage.SetSize(m_objZoomROI.Width, m_objZoomROI.Height);
                }
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);

                EasyImage.DoubleThreshold(m_objZoomROI, m_objThresholdImage, intLowThreshold, intHighThreshold, lowValue, middleValue, highValue);
            }
#endif

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }

        public void RedrawImage(Graphics g, int intLowThreshold, int intHighThreshold, List<List<ROI>> arrSearchROI, float fGainValue, byte lowValue, byte middleValue, byte highValue, int intSelectedUnit)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if (m_objMainImage.Width != m_objThresholdImage.Width || m_objThresholdImage.Height != m_objMainImage.Height)
                {
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);
                }
                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }

            if (fGainValue != 1f)
                EasyImage.GainOffset(m_objThresholdImage, m_objThresholdImage, fGainValue);


            EROIBW8 objTrainROI = new EROIBW8();
            for (int i = 0; i < arrSearchROI.Count; i++)
            {
                if (i != intSelectedUnit)
                    continue;

                objTrainROI.SetPlacement(arrSearchROI[i][0].ref_ROITotalX, arrSearchROI[i][0].ref_ROITotalY, arrSearchROI[i][0].ref_ROIWidth, arrSearchROI[i][0].ref_ROIHeight);
                objTrainROI.Detach();
                objTrainROI.Attach(m_objThresholdImage);

                if (intHighThreshold < 0)
                {
                    //EBW8 objBW8 = EasyImage.AutoThreshold(objTrainROI, EThresholdMode.MinResidue);
                    intHighThreshold = intLowThreshold;
                }
#if (Debug_2_12 || Release_2_12)
                if (m_fScale == 1.0f)
                {
                    EasyImage.DoubleThreshold(objTrainROI, objTrainROI, (uint)intLowThreshold, (uint)intHighThreshold, lowValue, middleValue, highValue);
                }
                else
                {
                    if (m_objThresholdImage.Width != m_objZoomROI.Width)
                    {
                        m_objThresholdImage.SetSize(m_objZoomROI.Width, m_objZoomROI.Height);
                    }
                    m_objZoomROI.Detach();
                    m_objZoomROI.Attach(m_objZoomImage);

                    EasyImage.DoubleThreshold(m_objZoomROI, m_objThresholdImage, (uint)intLowThreshold, (uint)intHighThreshold, lowValue, middleValue, highValue);
                }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (m_fScale == 1.0f)
                {
                    EasyImage.DoubleThreshold(objTrainROI, objTrainROI, intLowThreshold, intHighThreshold, lowValue, middleValue, highValue);
                }
                else
                {
                    if (m_objThresholdImage.Width != m_objZoomROI.Width)
                    {
                        m_objThresholdImage.SetSize(m_objZoomROI.Width, m_objZoomROI.Height);
                    }
                    m_objZoomROI.Detach();
                    m_objZoomROI.Attach(m_objZoomImage);

                    EasyImage.DoubleThreshold(m_objZoomROI, m_objThresholdImage, intLowThreshold, intHighThreshold, lowValue, middleValue, highValue);
                }
#endif

            }

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }

        public void RedrawImageWithGain(Graphics g, int intThresholdValue, List<ROI> arrROIs, int intROIIndex, float fGainValue)
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);


                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }

            EROIBW8 objTrainROI;

            objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(arrROIs[intROIIndex].ref_ROITotalX, arrROIs[intROIIndex].ref_ROITotalY,
                arrROIs[intROIIndex].ref_ROIWidth, arrROIs[intROIIndex].ref_ROIHeight);

            objTrainROI.Detach();
            objTrainROI.Attach(m_objThresholdImage);
            EasyImage.GainOffset(objTrainROI, objTrainROI, fGainValue);
#if (Debug_2_12 || Release_2_12)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(objTrainROI, objTrainROI, (uint)intThresholdValue);

            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(objTrainROI, objTrainROI, intThresholdValue);

            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
            }
#endif

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
        }
        public void RedrawThresholdImageWithGain(Graphics g, int intThresholdValue, List<List<ROI>> arrROIs, int intROIIndex, float fGainValue, int intSelectedROI, bool blnDrawAllSide) 
        {
            if (m_objMainImage == null)
                return;

            if (m_fScale == 1.0f)
            {
                if ((m_objMainImage.Width != m_objThresholdImage.Width) || (m_objMainImage.Height != m_objThresholdImage.Height))
                    m_objThresholdImage.SetSize(m_objMainImage.Width, m_objMainImage.Height);


                EasyImage.Copy(m_objMainImage, m_objThresholdImage);
            }

            if (blnDrawAllSide)
            {
                EROIBW8 objTrainROI2;

                objTrainROI2 = new EROIBW8();
                for (int i = 1; i < arrROIs.Count; i++)
                {
                    if (i != intSelectedROI)
                    {
                        objTrainROI2.SetPlacement(arrROIs[i][intROIIndex].ref_ROITotalX, arrROIs[i][intROIIndex].ref_ROITotalY,
                        arrROIs[i][intROIIndex].ref_ROIWidth, arrROIs[i][intROIIndex].ref_ROIHeight);

                        objTrainROI2.Detach();
                        objTrainROI2.Attach(m_objThresholdImage);
                        EasyImage.GainOffset(objTrainROI2, objTrainROI2, fGainValue);
#if (Debug_2_12 || Release_2_12)
                        if (m_fScale == 1.0f)
                        {
                            EasyImage.Threshold(objTrainROI2, objTrainROI2, (uint)intThresholdValue);

                        }
                        else
                        {
                            m_objZoomROI.Detach();
                            m_objZoomROI.Attach(m_objZoomImage);
                            EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
                        }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        if (m_fScale == 1.0f)
                        {
                            EasyImage.Threshold(objTrainROI2, objTrainROI2, intThresholdValue);

                        }
                        else
                        {
                            m_objZoomROI.Detach();
                            m_objZoomROI.Attach(m_objZoomImage);
                            EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
                        }
#endif

                    }
                }
            }
            

            EROIBW8 objTrainROI;

            objTrainROI = new EROIBW8();
            objTrainROI.SetPlacement(arrROIs[intSelectedROI][intROIIndex].ref_ROITotalX, arrROIs[intSelectedROI][intROIIndex].ref_ROITotalY,
                arrROIs[intSelectedROI][intROIIndex].ref_ROIWidth, arrROIs[intSelectedROI][intROIIndex].ref_ROIHeight);

            objTrainROI.Detach();
            objTrainROI.Attach(m_objThresholdImage);
            EasyImage.GainOffset(objTrainROI, objTrainROI, fGainValue);
#if (Debug_2_12 || Release_2_12)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(objTrainROI, objTrainROI, (uint)intThresholdValue);

            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, (uint)intThresholdValue);
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (m_fScale == 1.0f)
            {
                EasyImage.Threshold(objTrainROI, objTrainROI, intThresholdValue);

            }
            else
            {
                m_objZoomROI.Detach();
                m_objZoomROI.Attach(m_objZoomImage);
                EasyImage.Threshold(m_objZoomROI, m_objThresholdImage, intThresholdValue);
            }
#endif

            m_objThresholdImage.Draw(g, m_fDrawingScaleX, m_fDrawingScaleY);
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
                m_objMainImage.Save(strPath);
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
        /// Set image size
        /// </summary>
        /// <param name="intWidth">Image size width</param>
        /// <param name="intHeight">Image size height</param>
        public void SetImageSize(int intWidth, int intHeight)
        {
            if (m_objMainImage.Width != intWidth || m_objMainImage.Height != intHeight)
                m_objMainImage.SetSize(intWidth, intHeight);               
        }

        public void SetImageToBlack()
        {
            EasyImage.Copy(new EBW8(0), m_objMainImage);
        }
        
        public void SetImageToWhite()
        {
            EasyImage.Copy(new EBW8(255), m_objMainImage);
        }

        public static void SetImageSize(EImageBW8 objImage, int intWidth, int intHeight)
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
                SRMMessageBox.Show("ImageDrawing.cs -> SetImageSize() :" + ex.ToString());
            }
        }
        
        /// <summary>
        /// Is Main image available
        /// </summary>
        /// <param name="objImage">Source Image</param>
        /// <returns>true if image is available</returns>
        public bool VerifyImageAvailable(ImageDrawing objImage)
        {
            if (objImage.m_objMainImage != null)
                return true;

            return false;
        }
        
        /// <summary>
        ///  Get pixel value at the selected point
        /// </summary>
        /// <param name="intPositionX">point X</param>
        /// <param name="intPositionY">point Y</param>
        /// <returns>gray pixel of specific location</returns>
        public int GetImageGrayPixel(int intPositionX, int intPositionY)
        {
            if (intPositionX > 0 && intPositionY > 0 && intPositionX < m_objMainImage.Width && intPositionY < m_objMainImage.Height)
                return m_objMainImage.GetPixel(intPositionX, intPositionY).Value;

            return 0;
        }
        
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

        /// <summary>
        /// Modify the white area become smaller
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static bool ErodeImage(ROI objSourceROI, ref ROI objDestROI, int intHalfWidth)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                EasyImage.ErodeDisk(objSourceROI.ref_ROI, objDestROI.ref_ROI, (uint)intHalfWidth);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.ErodeDisk(objSourceROI.ref_ROI, objDestROI.ref_ROI, intHalfWidth);
#endif

            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Modify the white area become bigger
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static bool DilateImage(ROI objSourceROI, ref ROI objDestROI, int intHalfWidth)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                EasyImage.DilateDisk(objSourceROI.ref_ROI, objDestROI.ref_ROI, (uint)intHalfWidth);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.DilateDisk(objSourceROI.ref_ROI, objDestROI.ref_ROI, intHalfWidth);
#endif

            }
            catch
            {
                return false;
            }

            return true;
        }
        public static bool InvertImage(ImageDrawing objSourceImage, ImageDrawing objDestImage)
        {
            if (objDestImage.ref_objMainImage.Width != objSourceImage.ref_objMainImage.Width || objDestImage.ref_objMainImage.Height != objSourceImage.ref_objMainImage.Height)
                objDestImage.SetImageSize(objSourceImage.ref_objMainImage.Width, objSourceImage.ref_objMainImage.Height);

            EasyImage.Oper(EArithmeticLogicOperation.Invert, objSourceImage.ref_objMainImage, objDestImage.ref_objMainImage);

            return true;
        }
        public static bool SubtractImage(ImageDrawing objImage1, ImageDrawing objImage2, ImageDrawing objDestImage)
        {
            objDestImage.SetImageSize(objImage1.ref_objMainImage.Width, objImage1.ref_objMainImage.Height);
            EasyImage.Oper(EArithmeticLogicOperation.Subtract, objImage1.ref_objMainImage, objImage2.ref_objMainImage, objDestImage.ref_objMainImage);

            return true;
        }
        public static bool SubtractImage(ImageDrawing objImage1, ImageDrawing objImage2)
        {
            EasyImage.Oper(EArithmeticLogicOperation.Subtract, objImage1.ref_objMainImage, objImage2.ref_objMainImage, objImage1.ref_objMainImage);

            return true;
        }
        public static bool CompareImage(ImageDrawing objImage1, ImageDrawing objImage2, ImageDrawing objDestImage)
        {
            objDestImage.SetImageSize(objImage1.ref_objMainImage.Width, objImage1.ref_objMainImage.Height);
            EasyImage.Oper(EArithmeticLogicOperation.Compare, objImage1.ref_objMainImage, objImage2.ref_objMainImage, objDestImage.ref_objMainImage);

            return true;
        }

        public static bool PixelStatistic(ImageDrawing objSourceImage, ref float fGrayMin, ref float fGrayMax,
            ref float fAverage, ref float fVariance, ref float fStdDev)
        {
            EBW8 eGrayMin = new EBW8();
            EBW8 eGrayMax = new EBW8();
            float fMean = 0;

            EasyImage.PixelStat(objSourceImage.ref_objMainImage, out eGrayMin, out eGrayMax, out fAverage);
            fGrayMin = eGrayMin.Value;
            fGrayMax = eGrayMax.Value;
            EasyImage.PixelVariance(objSourceImage.ref_objMainImage, out fVariance, out fMean);
            EasyImage.PixelStdDev(objSourceImage.ref_objMainImage, out fStdDev, out fMean);

            return true;
        }

        public static int GetHightPixelGrayValue(ImageDrawing objImage)
        {
            EBW8 pixel = new EBW8();
            EasyImage.PixelMax(objImage.ref_objMainImage, out pixel);
            return pixel.Value;
        }

        public void Rotate90Image(ref ImageDrawing objImageDest, bool blnRotateColorImage)
        {
            try
            {
                //if (blnRotateColorImage)
                //{
                //    if (m_blnWantColor)
                //    {
                //        if (objImageDest == null)
                //            objImageDest = new ImageDrawing(true, m_blnWantColor);

                //        if (objImageDest.ref_objMainColorImage == null)
                //        {
                //            objImageDest.ref_objMainColorImage = new EImageC24();
                //        }

                //        if (objImageDest.ref_objMainColorImage.Width != m_objMainColorImage.Width || objImageDest.ref_objMainColorImage.Height != m_objMainColorImage.Height)
                //            objImageDest.SetImageSize(m_objMainColorImage.Width, m_objMainColorImage.Height);

                //        EasyImage.ScaleRotate(m_objMainColorImage,
                //                            m_objMainColorImage.Width / 2, m_objMainColorImage.Height / 2,
                //                            objImageDest.ref_objMainColorImage.Width / 2, objImageDest.ref_objMainColorImage.Height / 2, 1, 1, 90, objImageDest.ref_objMainColorImage);
                //    }

                //}

                if (objImageDest == null)
                {
                    objImageDest = new ImageDrawing(true);
                }

                if (objImageDest.ref_intImageWidth != m_objMainImage.Width || objImageDest.ref_intImageHeight != m_objMainImage.Height)
                    objImageDest.SetImageSize(m_objMainImage.Width, m_objMainImage.Height);


                EasyImage.ScaleRotate(m_objMainImage,
                                    m_objMainImage.Width / 2, m_objMainImage.Height / 2,
                                    objImageDest.ref_objMainImage.Width / 2, objImageDest.ref_objMainImage.Height / 2, 1, 1, 90, objImageDest.ref_objMainImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("ImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void Rotate90Image(ref List<ImageDrawing> arrImageDest, int intImageIndex)
        {
            if (arrImageDest.Count <= intImageIndex)
                return;

            try
            {
                //if (blnRotateColorImage)
                //{
                //    if (m_blnWantColor)
                //    {
                //        if (objImageDest == null)
                //            objImageDest = new ImageDrawing(true, m_blnWantColor);

                //        if (objImageDest.ref_objMainColorImage == null)
                //        {
                //            objImageDest.ref_objMainColorImage = new EImageC24();
                //        }

                //        if (objImageDest.ref_objMainColorImage.Width != m_objMainColorImage.Width || objImageDest.ref_objMainColorImage.Height != m_objMainColorImage.Height)
                //            objImageDest.SetImageSize(m_objMainColorImage.Width, m_objMainColorImage.Height);

                //        EasyImage.ScaleRotate(m_objMainColorImage,
                //                            m_objMainColorImage.Width / 2, m_objMainColorImage.Height / 2,
                //                            objImageDest.ref_objMainColorImage.Width / 2, objImageDest.ref_objMainColorImage.Height / 2, 1, 1, 90, objImageDest.ref_objMainColorImage);
                //    }

                //}

                if (arrImageDest[intImageIndex] == null)
                {
                    arrImageDest[intImageIndex] = new ImageDrawing(true, m_objMainImage.Width, m_objMainImage.Height);
                }

                if (arrImageDest[intImageIndex].ref_intImageWidth != m_objMainImage.Width || arrImageDest[intImageIndex].ref_intImageHeight != m_objMainImage.Height)
                    arrImageDest[intImageIndex].SetImageSize(m_objMainImage.Width, m_objMainImage.Height);


                EasyImage.ScaleRotate(m_objMainImage,
                                    m_objMainImage.Width / 2, m_objMainImage.Height / 2,
                                    arrImageDest[intImageIndex].ref_objMainImage.Width / 2, arrImageDest[intImageIndex].ref_objMainImage.Height / 2, 1, 1, 90, arrImageDest[intImageIndex].ref_objMainImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("ImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void RotateMinus90Image(ref ImageDrawing objImageDest, bool blnRotateColorImage)
        {
            try
            {
                //if (blnRotateColorImage)
                //{
                //    if (m_blnWantColor)
                //    {
                //        if (objImageDest == null)
                //            objImageDest = new ImageDrawing(true, m_blnWantColor);

                //        if (objImageDest.ref_objMainColorImage == null)
                //        {
                //            objImageDest.ref_objMainColorImage = new EImageC24();
                //        }

                //        if (objImageDest.ref_objMainColorImage.Width != m_objMainColorImage.Width || objImageDest.ref_objMainColorImage.Height != m_objMainColorImage.Height)
                //            objImageDest.SetImageSize(m_objMainColorImage.Width, m_objMainColorImage.Height);

                //        EasyImage.ScaleRotate(m_objMainColorImage,
                //                            m_objMainColorImage.Width / 2, m_objMainColorImage.Height / 2,
                //                            objImageDest.ref_objMainColorImage.Width / 2, objImageDest.ref_objMainColorImage.Height / 2, 1, 1, 90, objImageDest.ref_objMainColorImage);
                //    }

                //}

                if (objImageDest == null)
                {
                    objImageDest = new ImageDrawing(true);
                }

                if (objImageDest.ref_intImageWidth != m_objMainImage.Width || objImageDest.ref_intImageHeight != m_objMainImage.Height)
                    objImageDest.SetImageSize(m_objMainImage.Width, m_objMainImage.Height);


                EasyImage.ScaleRotate(m_objMainImage,
                                    m_objMainImage.Width / 2, m_objMainImage.Height / 2,
                                    objImageDest.ref_objMainImage.Width / 2, objImageDest.ref_objMainImage.Height / 2, 1, 1, -90, objImageDest.ref_objMainImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("ImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void RotateMinus90Image(ref List<ImageDrawing> arrImageDest, int intImageIndex)
        {
            if (arrImageDest.Count <= intImageIndex)
                return;

            try
            {
                //if (blnRotateColorImage)
                //{
                //    if (m_blnWantColor)
                //    {
                //        if (objImageDest == null)
                //            objImageDest = new ImageDrawing(true, m_blnWantColor);

                //        if (objImageDest.ref_objMainColorImage == null)
                //        {
                //            objImageDest.ref_objMainColorImage = new EImageC24();
                //        }

                //        if (objImageDest.ref_objMainColorImage.Width != m_objMainColorImage.Width || objImageDest.ref_objMainColorImage.Height != m_objMainColorImage.Height)
                //            objImageDest.SetImageSize(m_objMainColorImage.Width, m_objMainColorImage.Height);

                //        EasyImage.ScaleRotate(m_objMainColorImage,
                //                            m_objMainColorImage.Width / 2, m_objMainColorImage.Height / 2,
                //                            objImageDest.ref_objMainColorImage.Width / 2, objImageDest.ref_objMainColorImage.Height / 2, 1, 1, 90, objImageDest.ref_objMainColorImage);
                //    }

                //}

                if (arrImageDest[intImageIndex] == null)
                {
                    arrImageDest[intImageIndex] = new ImageDrawing(true, m_objMainImage.Width, m_objMainImage.Height);
                }

                if (arrImageDest[intImageIndex].ref_intImageWidth != m_objMainImage.Width || arrImageDest[intImageIndex].ref_intImageHeight != m_objMainImage.Height)
                    arrImageDest[intImageIndex].SetImageSize(m_objMainImage.Width, m_objMainImage.Height);


                EasyImage.ScaleRotate(m_objMainImage,
                                    m_objMainImage.Width / 2, m_objMainImage.Height / 2,
                                    arrImageDest[intImageIndex].ref_objMainImage.Width / 2, arrImageDest[intImageIndex].ref_objMainImage.Height / 2, 1, 1, -90, arrImageDest[intImageIndex].ref_objMainImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("ImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void Rotate180Image(ref ImageDrawing objImageDest, bool blnRotateColorImage)
        {
            try
            {
                //if (blnRotateColorImage)
                //{
                //    if (m_blnWantColor)
                //    {
                //        if (objImageDest == null)
                //            objImageDest = new ImageDrawing(true, m_blnWantColor);

                //        if (objImageDest.ref_objMainColorImage == null)
                //        {
                //            objImageDest.ref_objMainColorImage = new EImageC24();
                //        }

                //        if (objImageDest.ref_objMainColorImage.Width != m_objMainColorImage.Width || objImageDest.ref_objMainColorImage.Height != m_objMainColorImage.Height)
                //            objImageDest.SetImageSize(m_objMainColorImage.Width, m_objMainColorImage.Height);

                //        EasyImage.ScaleRotate(m_objMainColorImage,
                //                            m_objMainColorImage.Width / 2, m_objMainColorImage.Height / 2,
                //                            objImageDest.ref_objMainColorImage.Width / 2, objImageDest.ref_objMainColorImage.Height / 2, 1, 1, 180, objImageDest.ref_objMainColorImage);
                //    }

                //}

                if (objImageDest == null)
                {
                    objImageDest = new ImageDrawing(true);
                }

                if (objImageDest.ref_intImageWidth != m_objMainImage.Width || objImageDest.ref_intImageHeight != m_objMainImage.Height)
                    objImageDest.SetImageSize(m_objMainImage.Width, m_objMainImage.Height);


                EasyImage.ScaleRotate(m_objMainImage,
                                    m_objMainImage.Width / 2, m_objMainImage.Height / 2,
                                    objImageDest.ref_objMainImage.Width / 2, objImageDest.ref_objMainImage.Height / 2, 1, 1, 180, objImageDest.ref_objMainImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("ImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void Rotate180Image(ref List<ImageDrawing> arrImageDest, int intImageIndex)
        {
            if (arrImageDest.Count <= intImageIndex)
                return;

            try
            {
                //if (blnRotateColorImage)
                //{
                //    if (m_blnWantColor)
                //    {
                //        if (objImageDest == null)
                //            objImageDest = new ImageDrawing(true, m_blnWantColor);

                //        if (objImageDest.ref_objMainColorImage == null)
                //        {
                //            objImageDest.ref_objMainColorImage = new EImageC24();
                //        }

                //        if (objImageDest.ref_objMainColorImage.Width != m_objMainColorImage.Width || objImageDest.ref_objMainColorImage.Height != m_objMainColorImage.Height)
                //            objImageDest.SetImageSize(m_objMainColorImage.Width, m_objMainColorImage.Height);

                //        EasyImage.ScaleRotate(m_objMainColorImage,
                //                            m_objMainColorImage.Width / 2, m_objMainColorImage.Height / 2,
                //                            objImageDest.ref_objMainColorImage.Width / 2, objImageDest.ref_objMainColorImage.Height / 2, 1, 1, 180, objImageDest.ref_objMainColorImage);
                //    }

                //}

                if (arrImageDest[intImageIndex] == null)
                {
                    arrImageDest[intImageIndex] = new ImageDrawing(true, m_objMainImage.Width, m_objMainImage.Height);
                }

                if (arrImageDest[intImageIndex].ref_intImageWidth != m_objMainImage.Width || arrImageDest[intImageIndex].ref_intImageHeight != m_objMainImage.Height)
                    arrImageDest[intImageIndex].SetImageSize(m_objMainImage.Width, m_objMainImage.Height);


                EasyImage.ScaleRotate(m_objMainImage,
                                    m_objMainImage.Width / 2, m_objMainImage.Height / 2,
                                    arrImageDest[intImageIndex].ref_objMainImage.Width / 2, arrImageDest[intImageIndex].ref_objMainImage.Height / 2, 1, 1, 180, arrImageDest[intImageIndex].ref_objMainImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("ImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void FlipHorizontalImage(ref ImageDrawing objImageDest, bool blnRotateColorImage)
        {
            try
            {
                //if (blnRotateColorImage)
                //{
                //    if (m_blnWantColor)
                //    {
                //        if (objImageDest == null)
                //            objImageDest = new ImageDrawing(true, m_blnWantColor);

                //        if (objImageDest.ref_objMainColorImage == null)
                //        {
                //            objImageDest.ref_objMainColorImage = new EImageC24();
                //        }

                //        if (objImageDest.ref_objMainColorImage.Width != m_objMainColorImage.Width || objImageDest.ref_objMainColorImage.Height != m_objMainColorImage.Height)
                //            objImageDest.SetImageSize(m_objMainColorImage.Width, m_objMainColorImage.Height);

                //        EasyImage.ScaleRotate(m_objMainColorImage,
                //                            m_objMainColorImage.Width / 2, m_objMainColorImage.Height / 2,
                //                            objImageDest.ref_objMainColorImage.Width / 2, objImageDest.ref_objMainColorImage.Height / 2, 1, 1, 180, objImageDest.ref_objMainColorImage);
                //    }

                //}

                if (objImageDest == null)
                {
                    objImageDest = new ImageDrawing(true);
                }

                if (objImageDest.ref_intImageWidth != m_objMainImage.Width || objImageDest.ref_intImageHeight != m_objMainImage.Height)
                    objImageDest.SetImageSize(m_objMainImage.Width, m_objMainImage.Height);


                EasyImage.ScaleRotate(m_objMainImage,
                                    m_objMainImage.Width / 2, m_objMainImage.Height / 2,
                                    objImageDest.ref_objMainImage.Width / 2, objImageDest.ref_objMainImage.Height / 2, -1, 1, 0, objImageDest.ref_objMainImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("ImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void FlipHorizontalImage(ref List<ImageDrawing> arrImageDest, int intImageIndex)
        {
            if (arrImageDest.Count <= intImageIndex)
                return;

            try
            {
                //if (blnRotateColorImage)
                //{
                //    if (m_blnWantColor)
                //    {
                //        if (objImageDest == null)
                //            objImageDest = new ImageDrawing(true, m_blnWantColor);

                //        if (objImageDest.ref_objMainColorImage == null)
                //        {
                //            objImageDest.ref_objMainColorImage = new EImageC24();
                //        }

                //        if (objImageDest.ref_objMainColorImage.Width != m_objMainColorImage.Width || objImageDest.ref_objMainColorImage.Height != m_objMainColorImage.Height)
                //            objImageDest.SetImageSize(m_objMainColorImage.Width, m_objMainColorImage.Height);

                //        EasyImage.ScaleRotate(m_objMainColorImage,
                //                            m_objMainColorImage.Width / 2, m_objMainColorImage.Height / 2,
                //                            objImageDest.ref_objMainColorImage.Width / 2, objImageDest.ref_objMainColorImage.Height / 2, 1, 1, 180, objImageDest.ref_objMainColorImage);
                //    }

                //}

                if (arrImageDest[intImageIndex] == null)
                {
                    arrImageDest[intImageIndex] = new ImageDrawing(true, m_objMainImage.Width, m_objMainImage.Height);
                }

                if (arrImageDest[intImageIndex].ref_intImageWidth != m_objMainImage.Width || arrImageDest[intImageIndex].ref_intImageHeight != m_objMainImage.Height)
                    arrImageDest[intImageIndex].SetImageSize(m_objMainImage.Width, m_objMainImage.Height);


                EasyImage.ScaleRotate(m_objMainImage,
                                    m_objMainImage.Width / 2, m_objMainImage.Height / 2,
                                    arrImageDest[intImageIndex].ref_objMainImage.Width / 2, arrImageDest[intImageIndex].ref_objMainImage.Height / 2, -1, 1, 0, arrImageDest[intImageIndex].ref_objMainImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("ImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void FlipVerticalImage(ref ImageDrawing objImageDest, bool blnRotateColorImage)
        {
            try
            {
                //if (blnRotateColorImage)
                //{
                //    if (m_blnWantColor)
                //    {
                //        if (objImageDest == null)
                //            objImageDest = new ImageDrawing(true, m_blnWantColor);

                //        if (objImageDest.ref_objMainColorImage == null)
                //        {
                //            objImageDest.ref_objMainColorImage = new EImageC24();
                //        }

                //        if (objImageDest.ref_objMainColorImage.Width != m_objMainColorImage.Width || objImageDest.ref_objMainColorImage.Height != m_objMainColorImage.Height)
                //            objImageDest.SetImageSize(m_objMainColorImage.Width, m_objMainColorImage.Height);

                //        EasyImage.ScaleRotate(m_objMainColorImage,
                //                            m_objMainColorImage.Width / 2, m_objMainColorImage.Height / 2,
                //                            objImageDest.ref_objMainColorImage.Width / 2, objImageDest.ref_objMainColorImage.Height / 2, 1, 1, 180, objImageDest.ref_objMainColorImage);
                //    }

                //}

                if (objImageDest == null)
                {
                    objImageDest = new ImageDrawing(true);
                }

                if (objImageDest.ref_intImageWidth != m_objMainImage.Width || objImageDest.ref_intImageHeight != m_objMainImage.Height)
                    objImageDest.SetImageSize(m_objMainImage.Width, m_objMainImage.Height);


                EasyImage.ScaleRotate(m_objMainImage,
                                    m_objMainImage.Width / 2, m_objMainImage.Height / 2,
                                    objImageDest.ref_objMainImage.Width / 2, objImageDest.ref_objMainImage.Height / 2, 1, -1, 0, objImageDest.ref_objMainImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("ImageDrawing->RotateImage->ex = " + ex.ToString());
            }

        }
        public void FlipVerticalImage(ref List<ImageDrawing> arrImageDest, int intImageIndex)
        {
            if (arrImageDest.Count <= intImageIndex)
                return;

            try
            {
                //if (blnRotateColorImage)
                //{
                //    if (m_blnWantColor)
                //    {
                //        if (objImageDest == null)
                //            objImageDest = new ImageDrawing(true, m_blnWantColor);

                //        if (objImageDest.ref_objMainColorImage == null)
                //        {
                //            objImageDest.ref_objMainColorImage = new EImageC24();
                //        }

                //        if (objImageDest.ref_objMainColorImage.Width != m_objMainColorImage.Width || objImageDest.ref_objMainColorImage.Height != m_objMainColorImage.Height)
                //            objImageDest.SetImageSize(m_objMainColorImage.Width, m_objMainColorImage.Height);

                //        EasyImage.ScaleRotate(m_objMainColorImage,
                //                            m_objMainColorImage.Width / 2, m_objMainColorImage.Height / 2,
                //                            objImageDest.ref_objMainColorImage.Width / 2, objImageDest.ref_objMainColorImage.Height / 2, 1, 1, 180, objImageDest.ref_objMainColorImage);
                //    }

                //}

                if (arrImageDest[intImageIndex] == null)
                {
                    arrImageDest[intImageIndex] = new ImageDrawing(true, m_objMainImage.Width, m_objMainImage.Height);
                }

                if (arrImageDest[intImageIndex].ref_intImageWidth != m_objMainImage.Width || arrImageDest[intImageIndex].ref_intImageHeight != m_objMainImage.Height)
                    arrImageDest[intImageIndex].SetImageSize(m_objMainImage.Width, m_objMainImage.Height);


                EasyImage.ScaleRotate(m_objMainImage,
                                    m_objMainImage.Width / 2, m_objMainImage.Height / 2,
                                    arrImageDest[intImageIndex].ref_objMainImage.Width / 2, arrImageDest[intImageIndex].ref_objMainImage.Height / 2, 1, -1, 0, arrImageDest[intImageIndex].ref_objMainImage);
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("ImageDrawing->RotateImage->ex = " + ex.ToString());
            }
        }
        public static bool OpenImage(ImageDrawing objSourceImage, ref ImageDrawing objDestImage, int intHalfWidth)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                EasyImage.OpenDisk(objSourceImage.ref_objMainImage, objDestImage.ref_objMainImage, (uint)intHalfWidth);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.OpenDisk(objSourceImage.ref_objMainImage, objDestImage.ref_objMainImage, intHalfWidth);
#endif

            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool CloseBoxImage(ImageDrawing objSourceImage, ref ImageDrawing objDestImage, int intHalfWidth)
        {
            try
            {
#if (Debug_2_12 || Release_2_12)
                EasyImage.CloseBox(objSourceImage.ref_objMainImage, objDestImage.ref_objMainImage, (uint)intHalfWidth);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.CloseBox(objSourceImage.ref_objMainImage, objDestImage.ref_objMainImage, intHalfWidth);
#endif

            }
            catch
            {
                return false;
            }

            return true;
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
        public static int GetImageViewNo(int intUserSelectImageIndex, int intVisionIndex)
        {
            if (intUserSelectImageIndex < 0)
                return 0;

            switch (m_arrImageMergeType[intVisionIndex])
            {
                default:
                case 0: // No merge
                    {
                        return intUserSelectImageIndex;
                    }
                case 1: // Merge grab 1 center and grab 2 side 
                    {
                        if (intUserSelectImageIndex < 2)
                            return 0;
                        else
                            return intUserSelectImageIndex - 1;
                    }
                case 2: // Merge grab 1 center, grab 2 top left and grab 3 bottom right.
                    {
                        if (intUserSelectImageIndex < 3)
                            return 0;
                        else
                            return intUserSelectImageIndex - 2;
                    }
                case 3: // Merge grab 1 center and grab 2 side and Merge grab 3 center and grab 4 side 
                    {
                        if (intUserSelectImageIndex < 2)
                            return 0;
                        else if (intUserSelectImageIndex < 4)
                            return 1;
                        else
                            return intUserSelectImageIndex - 2;
                    }
                case 4: // Merge grab 1 center, grab 2 top left and grab 3 bottom right. and Merge grab 3 center and grab 4 side 
                    {
                        if (intUserSelectImageIndex < 3)
                            return 0;
                        else if (intUserSelectImageIndex < 5)
                            return 1;
                        else
                            return intUserSelectImageIndex - 2;
                    }
            }
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

        public static int[] GetArrayImageIndex(int[] arrUserSelectImageViewIndex, int intVisionIndex)
        {
            int[] arrArrayImageIndex = new int[arrUserSelectImageViewIndex.Length];
            for (int i = 0; i < arrUserSelectImageViewIndex.Length; i++)
            {
                if (arrUserSelectImageViewIndex[i] < 0)
                    arrArrayImageIndex[i] = 0;

                switch (m_arrImageMergeType[intVisionIndex])
                {
                    default:
                    case 0: // No merge
                        {
                            arrArrayImageIndex[i] = arrUserSelectImageViewIndex[i];
                        }
                        break;
                    case 1: // Merge grab 1 center and grab 2 side 
                        {
                            if (arrUserSelectImageViewIndex[i] <= 0)
                                arrArrayImageIndex[i] = 0;
                            else if (arrUserSelectImageViewIndex[i] + 1 >= m_arrImageCount[intVisionIndex])
                                arrArrayImageIndex[i] = arrUserSelectImageViewIndex[i];
                            else
                                arrArrayImageIndex[i] = (arrUserSelectImageViewIndex[i] + 1);
                        }
                        break;
                    case 2: // Merge grab 1 center, grab 2 top left and grab 3 bottom right.
                        {
                            if (arrUserSelectImageViewIndex[i] <= 0)    // select image 0 which is grab 1
                                arrArrayImageIndex[i] = 0;
                            else if (arrUserSelectImageViewIndex[i] + 1 >= m_arrImageCount[intVisionIndex])
                                arrArrayImageIndex[i] = arrUserSelectImageViewIndex[i];
                            else if (arrUserSelectImageViewIndex[i] + 2 >= m_arrImageCount[intVisionIndex])
                                arrArrayImageIndex[i] = (arrUserSelectImageViewIndex[i] + 1);
                            else
                                arrArrayImageIndex[i] = (arrUserSelectImageViewIndex[i] + 2);
                        }
                        break;
                    case 3: // Merge grab 1 center and grab 2 side and Merge grab 3 center and grab 4 side 
                        {
                            if (arrUserSelectImageViewIndex[i] <= 0)
                                arrArrayImageIndex[i] = 0;
                            else if (arrUserSelectImageViewIndex[i] == 2) // select image 2 which is grab 5
                            {
                                if (arrUserSelectImageViewIndex[i] + 2 >= m_arrImageCount[intVisionIndex])
                                    arrArrayImageIndex[i] = arrUserSelectImageViewIndex[i];
                                else
                                    arrArrayImageIndex[i] = (arrUserSelectImageViewIndex[i] + 2);
                            }
                            else // select image 1 which is grab 3 and 4
                            {
                                //if (arrUserSelectImageViewIndex[i] + 1 >= m_intImageCount)
                                //    arrArrayImageIndex[i] = arrUserSelectImageViewIndex[i];
                                //else
                                //    arrArrayImageIndex[i] = (arrUserSelectImageViewIndex[i] + 1);

                                if (arrUserSelectImageViewIndex[i] <= 0)    // select image 0 which is grab 1
                                    arrArrayImageIndex[i] = 0;
                                else if (arrUserSelectImageViewIndex[i] + 1 >= m_arrImageCount[intVisionIndex])
                                    arrArrayImageIndex[i] = arrUserSelectImageViewIndex[i];
                                else if (arrUserSelectImageViewIndex[i] + 2 >= m_arrImageCount[intVisionIndex])
                                    arrArrayImageIndex[i] = (arrUserSelectImageViewIndex[i] + 1);
                                else
                                    arrArrayImageIndex[i] = (arrUserSelectImageViewIndex[i] + 2);
                            }
                        }
                        break;
                    case 4: // Merge grab 1 center, grab 2 top left and grab 3 bottom right. and Merge grab 3 center and grab 4 side 
                        {
                            if (arrUserSelectImageViewIndex[i] <= 0)
                                arrArrayImageIndex[i] = 0;
                            else if (arrUserSelectImageViewIndex[i] == 1) // select image 1 which is grab 4 and 5
                            {
                                if (arrUserSelectImageViewIndex[i] + 2 >= m_arrImageCount[intVisionIndex])
                                    arrArrayImageIndex[i] = arrUserSelectImageViewIndex[i];
                                else
                                    arrArrayImageIndex[i] = arrUserSelectImageViewIndex[i] + 2;
                            }
                            else // select other than image 0 or 1
                            {
                                if (arrUserSelectImageViewIndex[i] + 3 >= m_arrImageCount[intVisionIndex])
                                    arrArrayImageIndex[i] = arrUserSelectImageViewIndex[i];
                                else
                                    arrArrayImageIndex[i] = (arrUserSelectImageViewIndex[i] + 3);
                            }
                        }
                        break;
                }

            }
            return arrArrayImageIndex;
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

        public static void AddTwoImageTogether(ref ImageDrawing objImage1,ref ImageDrawing objImage2)
        {
            if (objImage1.ref_intImageWidth != objImage2.ref_intImageWidth || objImage1.ref_intImageHeight != objImage2.ref_intImageHeight)
                return;

            EasyImage.Oper(EArithmeticLogicOperation.Add, objImage1.ref_objMainImage, objImage2.ref_objMainImage, objImage2.ref_objMainImage);
        }

        public static void UniformizeImage(ImageDrawing objImage1, ImageDrawing objBrightImage, ImageDrawing objDarkImage, ref ImageDrawing objImage2, int Gain)
        {
            if (objImage1.ref_intImageWidth != objImage2.ref_intImageWidth || objImage1.ref_intImageHeight != objImage2.ref_intImageHeight 
                || objImage1.ref_intImageWidth != objBrightImage.ref_intImageWidth || objImage1.ref_intImageHeight != objBrightImage.ref_intImageHeight
                || objImage1.ref_intImageWidth != objDarkImage.ref_intImageWidth || objImage1.ref_intImageHeight != objDarkImage.ref_intImageHeight)
                return;

            EasyImage.Uniformize(objImage1.ref_objMainImage, new EBW8(255), objBrightImage.ref_objMainImage, new EBW8(0), objDarkImage.ref_objMainImage, objImage2.ref_objMainImage);

            EasyImage.GainOffset(objImage2.ref_objMainImage, objImage2.ref_objMainImage, Gain);
        }

        public static void UniformizeColorImage(CImageDrawing objImage1, CImageDrawing objBrightImage, CImageDrawing objDarkImage, ref CImageDrawing objImage2, int Gain)
        {
            if (objImage1.ref_intImageWidth != objImage2.ref_intImageWidth || objImage1.ref_intImageHeight != objImage2.ref_intImageHeight
                || objImage1.ref_intImageWidth != objBrightImage.ref_intImageWidth || objImage1.ref_intImageHeight != objBrightImage.ref_intImageHeight
                || objImage1.ref_intImageWidth != objDarkImage.ref_intImageWidth || objImage1.ref_intImageHeight != objDarkImage.ref_intImageHeight)
                return;

            EasyImage.Uniformize(objImage1.ref_objMainCImage, new EC24(255, 255, 255), objBrightImage.ref_objMainCImage, new EC24(0, 0, 0), objDarkImage.ref_objMainCImage, objImage2.ref_objMainCImage);

            //EasyImage.GainOffset(objImage2.ref_objMainCImage, objImage2.ref_objMainCImage, Gain);
        }

        public static void UniformizeImage(ROI objROI, ImageDrawing objReferenceImage, ImageDrawing objReferenceInvertedImage, int intSetting, int intMaskingThreshold)
        {
            if (intSetting == 1)
            {
                ROI objBrightROI = new ROI();
                ROI objDarkROI = new ROI();
                objBrightROI.AttachImage(objReferenceInvertedImage);
                objDarkROI.AttachImage(objReferenceImage);
                objBrightROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                objDarkROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                EasyImage.Uniformize(objROI.ref_ROI, new EBW8(255), objBrightROI.ref_ROI, new EBW8(0), objDarkROI.ref_ROI, objROI.ref_ROI);
                objBrightROI.Dispose();
                objDarkROI.Dispose();
            }
            else if (intSetting == 2)
            {
                // 2024 04 27 - CCENG: Hide this Uniformaize masking method
                //ROI objBrightROI = new ROI();
                //objBrightROI.AttachImage(objReferenceImage); //objBrightROI.AttachImage(objReferenceInvertedImage); // 2022 01 04 - CCENG: Use objReferenceImage image for masking the dark shodow in Pad5SPkg-White Background image.
                //objBrightROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                //EasyImage.Uniformize(objROI.ref_ROI, new EBW8(255), objBrightROI.ref_ROI, objROI.ref_ROI, true);
                //objBrightROI.Dispose();
                bool blnIsDebug = false;
                if (blnIsDebug) objROI.SaveImage("D:\\TS\\0_objROI.bmp");
                // 2024 04 27 - CCENG: Use this Uniformize + Threshold masking method.
                ROI objBrightROI = new ROI();
                objBrightROI.AttachImage(objReferenceImage); //objBrightROI.AttachImage(objReferenceInvertedImage); // 2022 01 04 - CCENG: Use objReferenceImage image for masking the dark shodow in Pad5SPkg-White Background image.
                objBrightROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                if (blnIsDebug) objBrightROI.SaveImage("D:\\TS\\1_BrightROI.bmp");

                ImageDrawing objThresholdImage = new ImageDrawing(true);
                objThresholdImage.SetImageSize(objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                if (blnIsDebug) objThresholdImage.SaveImage("D:\\TS\\2_ThresholdIMage.bmp");

                ROI objThresholdROI = new ROI();
                objThresholdROI.AttachImage(objThresholdImage);
                objThresholdROI.LoadROISetting(0, 0, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                if (blnIsDebug) objThresholdROI.SaveImage("D:\\TS\\3_ThresholdROI.bmp");

                EasyImage.Threshold(objROI.ref_ROI, objThresholdROI.ref_ROI, (uint)intMaskingThreshold); // 72);
                if (blnIsDebug) objThresholdROI.SaveImage("D:\\TS\\4_ThresholdROI.bmp");

                EasyImage.Oper(EArithmeticLogicOperation.Invert, objThresholdROI.ref_ROI, objThresholdROI.ref_ROI);
                if (blnIsDebug) objThresholdROI.SaveImage("D:\\TS\\5_ThresholdROI.bmp");

                EasyImage.Oper(EArithmeticLogicOperation.Max, objThresholdROI.ref_ROI, objBrightROI.ref_ROI, objThresholdROI.ref_ROI);
                if (blnIsDebug) objThresholdROI.SaveImage("D:\\TS\\6_ThresholdROI.bmp");

                EasyImage.Uniformize(objROI.ref_ROI, new EBW8(255), objThresholdROI.ref_ROI, objROI.ref_ROI, true);
                if (blnIsDebug) objROI.SaveImage(   "D:\\TS\\7_ObjROI.bmp");

                objBrightROI.Dispose();
                objThresholdImage.Dispose();
                objThresholdImage = null;
                objThresholdROI.Dispose();
            }
        }

        public static void UniformizeColorImage(CROI objROI, CImageDrawing objReferenceImage, CImageDrawing objReferenceInvertedImage, int intSetting, int intMaskingThreshold)
        {
            if (intSetting == 1)
            {
                CROI objBrightROI = new CROI();
                CROI objDarkROI = new CROI();
                objBrightROI.AttachImage(objReferenceInvertedImage);
                objDarkROI.AttachImage(objReferenceImage);
                objBrightROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                objDarkROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);

                objROI.ref_CROI.TopParent.Save("D:\\TS\\S1_CUniformize_objROITopParent.bmp");
                objReferenceInvertedImage.SaveImage("D:\\TS\\S1_CUniformize_objReferenceInvertedImage.bmp");
                objReferenceImage.SaveImage("D:\\TS\\S1_CUniformize_objReferenceImage.bmp");

                EasyImage.Uniformize(
                    objROI.ref_CROI, 
                    new EC24(255, 255, 255),
                    objBrightROI.ref_CROI, 
                    new EC24(0, 0, 0),
                    objDarkROI.ref_CROI, 
                    objROI.ref_CROI
                    );
                objBrightROI.Dispose();
                objDarkROI.Dispose();
            }
            else if (intSetting == 2)
            {

                CROI objBrightROI = new CROI();
                objBrightROI.AttachImage(objReferenceImage); //objBrightROI.AttachImage(objReferenceInvertedImage); // 2022 01 04 - CCENG: Use objReferenceImage image for masking the dark shodow in Pad5SPkg-White Background image.
                objBrightROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);

                objROI.ref_CROI.TopParent.Save("D:\\TS\\S2_CUniformize_objROITopParent.bmp");
                objReferenceImage.SaveImage("D:\\TS\\S2_CUniformize_objReferenceImage.bmp");

                EasyImage.Uniformize(
                    objROI.ref_CROI,
                    new EC24(255, 255, 255),
                    objBrightROI.ref_CROI,
                    objROI.ref_CROI
                    );
                objBrightROI.Dispose();
            }
        }

        public void AddOffset(float fOffset)
        {
            if (fOffset >= -255 && fOffset <= 255)
                EasyImage.GainOffset(m_objMainImage, m_objMainImage, 1, fOffset);
        }

        public void AddDilate(int intDilate)
        {
            if (intDilate >= 0)
#if (Debug_2_12 || Release_2_12)
                EasyImage.DilateBox(m_objMainImage, m_objMainImage, (uint)intDilate);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.DilateBox(m_objMainImage, m_objMainImage, intDilate);
#endif
        }

        public void AddOpenMorphology(int intOpen)
        {
            if (intOpen >= 0)
#if (Debug_2_12 || Release_2_12)
                EasyImage.OpenBox(m_objMainImage, m_objMainImage, (uint)intOpen);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.OpenBox(m_objMainImage, m_objMainImage, intOpen);
#endif
        }
        public void AddCloseMorphology(int intClose)
        {
            if (intClose >= 0)
#if (Debug_2_12 || Release_2_12)
                EasyImage.CloseBox(m_objMainImage, m_objMainImage, (uint)intClose);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                EasyImage.CloseBox(m_objMainImage, m_objMainImage, intClose);
#endif
        }

        public static void ScaleImage(ImageDrawing objSourceImage, ref ImageDrawing objDestinationImage, float fSourcePivotX, float fSourcePivotY, float fDestinationPivotX, float fDestinationPivotY, float fScaleX, float fScaleY)
        {
            EasyImage.ScaleRotate(objSourceImage.ref_objMainImage, fSourcePivotX, fSourcePivotY,
                                    fDestinationPivotX, fDestinationPivotY,
                                    fScaleX, fScaleY, 0, objDestinationImage.ref_objMainImage, 4);
        }
    }
}
