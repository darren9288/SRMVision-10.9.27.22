using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using System.Drawing;
using VisionProcessing;

namespace VisionProcessing
{
    public class DontCareWithoutRotateImage
    {

        public static void ProduceImage(List<PointF> arrPoints, ROI objSearchROI, ROI objTargetROI, ImageDrawing objDestinationImage, ImageDrawing objBWTop, ImageDrawing objBWRight, ImageDrawing objBWBottom, ImageDrawing objBWLeft, bool blnWhiteDontCare)
        {
            bool blnDebugImage = false;

            float fMinX = float.MaxValue;
            float fMinY = float.MaxValue;
            float fMaxX = 0;
            float fMaxY = 0;
            for (int i = 0; i < arrPoints.Count; i++)
            {
                if (fMinX > arrPoints[i].X)
                    fMinX = arrPoints[i].X;

                if (fMinY > arrPoints[i].Y)
                    fMinY = arrPoints[i].Y;

                if (fMaxX < arrPoints[i].X)
                    fMaxX = arrPoints[i].X;

                if (fMaxY < arrPoints[i].Y)
                    fMaxY = arrPoints[i].Y;
            }

            if (fMinX < 0)
                fMinX = 0;
            if (fMinY < 0)
                fMinY = 0;
            if (fMaxX > objDestinationImage.ref_intImageWidth)
                fMaxX = objDestinationImage.ref_intImageWidth;
            if (fMaxY > objDestinationImage.ref_intImageHeight)
                fMaxY = objDestinationImage.ref_intImageHeight;

            objTargetROI.LoadROISetting((int)Math.Round(fMinX - objSearchROI.ref_ROIPositionX), (int)Math.Round(fMinY - objSearchROI.ref_ROIPositionY),
                (int)Math.Round(fMaxX - fMinX), (int)Math.Round(fMaxY - fMinY));
            
            ROI objBWROI = new ROI();
            ImageDrawing objTempImg = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);

            Line objLineTop = new Line();
            ROI objROI = new ROI();
            ROI objDontCareROI = new ROI();
            objLineTop.CalculateStraightLine(arrPoints[0], arrPoints[1]);

            objROI.AttachImage(objDestinationImage);

            float fAngle = 0;
            if (objLineTop.ref_dAngle > 0)
                fAngle = 90 - (float)objLineTop.ref_dAngle;
            else
                fAngle = (90 + (float)objLineTop.ref_dAngle);

            if (objLineTop.ref_dAngle > 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[0].X) - 1, (int)Math.Floor(fMinY) - 1, (int)Math.Ceiling(arrPoints[1].X - arrPoints[0].X) + 2, (int)Math.Ceiling(arrPoints[0].Y - fMinY) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[0].X) - 1, (int)Math.Floor(fMinY) - 1, (int)Math.Ceiling(arrPoints[1].X - arrPoints[0].X) + 2, (int)Math.Ceiling(arrPoints[1].Y - fMinY) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROITop.bmp");
            }
            float fWidth = 0, fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWTop);

            objBWROI.LoadROISetting(0, (int)Math.Round(objBWTop.ref_intImageHeight / 2 - fHeight / 2), (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROITop1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgTop1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineTop.ref_dAngle > 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROITop2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgTop2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROITop1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROITop2.bmp");
                objROI.SaveImage("D:\\TS\\objROITop2.bmp");
            }
            Line objLineBottom = new Line();
            objLineBottom.CalculateStraightLine(arrPoints[2], arrPoints[3]);
            if (objLineBottom.ref_dAngle > 0)
                fAngle = 90 - (float)objLineBottom.ref_dAngle;
            else
                fAngle = (90 + (float)objLineBottom.ref_dAngle);

            if (objLineBottom.ref_dAngle > 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[2].X) - 1, (int)Math.Floor(arrPoints[3].Y) - 1, (int)Math.Ceiling(arrPoints[3].X - arrPoints[2].X) + 2, (int)Math.Ceiling(fMaxY - arrPoints[3].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[2].X) - 1, (int)Math.Floor(arrPoints[2].Y) - 1, (int)Math.Ceiling(arrPoints[3].X - arrPoints[2].X) + 2, (int)Math.Ceiling(fMaxY - arrPoints[2].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROIBottom.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWBottom);

            objBWROI.LoadROISetting(0, (int)Math.Round(objBWBottom.ref_intImageHeight / 2 - fHeight / 2), (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIBottom1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgBottom1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineBottom.ref_dAngle > 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIBottom2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgBottom2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIBottom1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIBottom2.bmp");
                objROI.SaveImage("D:\\TS\\objROIBottom2.bmp");
            }
            Line objLineLeft = new Line();
            objLineLeft.CalculateStraightLine(arrPoints[0], arrPoints[2]);

            if (objLineLeft.ref_dAngle < 0)
                fAngle = 90 - (float)objLineLeft.ref_dAngle;
            else
                fAngle = (90 + (float)objLineLeft.ref_dAngle);

            if (objLineLeft.ref_dAngle < 0)
                objROI.LoadROISetting((int)Math.Floor(fMinX) - 1, (int)Math.Floor(arrPoints[0].Y) - 1, (int)Math.Ceiling(arrPoints[2].X - fMinX) + 2, (int)Math.Ceiling(arrPoints[2].Y - arrPoints[0].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(fMinX) - 1, (int)Math.Floor(arrPoints[0].Y) - 1, (int)Math.Ceiling(arrPoints[0].X - fMinX) + 2, (int)Math.Ceiling(arrPoints[2].Y - arrPoints[0].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROILeft.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWLeft);

            objBWROI.LoadROISetting((int)Math.Round(objBWLeft.ref_intImageWidth / 2 - fWidth / 2), 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROILeft1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgLeft1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineLeft.ref_dAngle < 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROILeft2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgLeft2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROILeft1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROILeft2.bmp");
                objROI.SaveImage("D:\\TS\\objROILeft2.bmp");
            }
            Line objLineRight = new Line();
            objLineRight.CalculateStraightLine(arrPoints[1], arrPoints[3]);

            if (objLineRight.ref_dAngle < 0)
                fAngle = 90 - (float)objLineRight.ref_dAngle;
            else
                fAngle = (90 + (float)objLineRight.ref_dAngle);

            if (objLineRight.ref_dAngle < 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[1].X) - 1, (int)Math.Floor(arrPoints[1].Y) - 1, (int)Math.Ceiling(fMaxX - arrPoints[1].X) + 2, (int)Math.Ceiling(arrPoints[3].Y - arrPoints[1].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[3].X) - 1, (int)Math.Floor(arrPoints[1].Y) - 1, (int)Math.Ceiling(fMaxX - arrPoints[3].X) + 2, (int)Math.Ceiling(arrPoints[3].Y - arrPoints[1].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROIRight.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWRight);

            objBWROI.LoadROISetting((int)Math.Round(objBWRight.ref_intImageWidth / 2 - fWidth / 2), 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIRight1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgRight1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineRight.ref_dAngle < 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIRight2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgRight2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIRight1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIRight2.bmp");
                objROI.SaveImage("D:\\TS\\objROIRight2.bmp");
                objDestinationImage.SaveImage("D:\\TS\\objDestinationImage.bmp");
                objTargetROI.SaveImage("D:\\TS\\objTargetROI.bmp");
            }
            objBWROI.Dispose();
            objTempImg.Dispose();
            objROI.Dispose();
            objDontCareROI.Dispose();
        }
        public static void ProduceImage(List<PointF> arrPoints, ROI objTargetROI, ImageDrawing objDestinationImage, ImageDrawing objBWTop, ImageDrawing objBWRight, ImageDrawing objBWBottom, ImageDrawing objBWLeft, bool blnWhiteDontCare)
        {
            bool blnDebugImage = false;

            if (blnDebugImage)
            {
                objBWTop.SaveImage("D:\\TS\\objBWTop.bmp");
                objBWRight.SaveImage("D:\\TS\\objBWRight.bmp");
                objBWBottom.SaveImage("D:\\TS\\objBWBottom.bmp");
                objBWLeft.SaveImage("D:\\TS\\objBWLeft.bmp");
            }

            float fMinX = float.MaxValue;
            float fMinY = float.MaxValue;
            float fMaxX = 0;
            float fMaxY = 0;
            for (int i = 0; i < arrPoints.Count; i++)
            {
                if (fMinX > arrPoints[i].X)
                    fMinX = arrPoints[i].X;

                if (fMinY > arrPoints[i].Y)
                    fMinY = arrPoints[i].Y;

                if (fMaxX < arrPoints[i].X)
                    fMaxX = arrPoints[i].X;

                if (fMaxY < arrPoints[i].Y)
                    fMaxY = arrPoints[i].Y;
            }

            if (fMinX < 0)
                fMinX = 0;
            if (fMinY < 0)
                fMinY = 0;
            if (fMaxX > objDestinationImage.ref_intImageWidth)
                fMaxX = objDestinationImage.ref_intImageWidth;
            if (fMaxY > objDestinationImage.ref_intImageHeight)
                fMaxY = objDestinationImage.ref_intImageHeight;

            objTargetROI.LoadROISetting((int)Math.Round(fMinX), (int)Math.Round(fMinY),
                (int)Math.Round(fMaxX - fMinX), (int)Math.Round(fMaxY - fMinY));

            ROI objBWROI = new ROI();
            ImageDrawing objTempImg = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);

            Line objLineTop = new Line();
            ROI objROI = new ROI();
            ROI objDontCareROI = new ROI();
            objLineTop.CalculateStraightLine(arrPoints[0], arrPoints[1]);

            objROI.AttachImage(objDestinationImage);

            float fAngle = 0;
            if (objLineTop.ref_dAngle > 0)
                fAngle = 90 - (float)objLineTop.ref_dAngle;
            else
                fAngle = (90 + (float)objLineTop.ref_dAngle);

            if (objLineTop.ref_dAngle > 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[0].X) - 1, (int)Math.Floor(fMinY) - 1, (int)Math.Ceiling(arrPoints[1].X - arrPoints[0].X) + 2, (int)Math.Ceiling(arrPoints[0].Y - fMinY) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[0].X) - 1, (int)Math.Floor(fMinY) - 1, (int)Math.Ceiling(arrPoints[1].X - arrPoints[0].X) + 2, (int)Math.Ceiling(arrPoints[1].Y - fMinY) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROITop.bmp");
            }
            float fWidth = 0, fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWTop);

            objBWROI.LoadROISetting(0, (int)Math.Round(objBWTop.ref_intImageHeight / 2 - fHeight / 2), (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROITop1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgTop1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineTop.ref_dAngle > 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROITop2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgTop2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROITop1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if(blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROITop2.bmp");
                objROI.SaveImage("D:\\TS\\objROITop2.bmp");
            }
            Line objLineBottom = new Line();
            objLineBottom.CalculateStraightLine(arrPoints[2], arrPoints[3]);
            if (objLineBottom.ref_dAngle > 0)
                fAngle = 90 - (float)objLineBottom.ref_dAngle;
            else
                fAngle = (90 + (float)objLineBottom.ref_dAngle);
            
            if (objLineBottom.ref_dAngle > 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[2].X) - 1, (int)Math.Floor(arrPoints[3].Y) - 1, (int)Math.Ceiling(arrPoints[3].X - arrPoints[2].X) + 2, (int)Math.Ceiling(fMaxY - arrPoints[3].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[2].X) - 1, (int)Math.Floor(arrPoints[2].Y) - 1, (int)Math.Ceiling(arrPoints[3].X - arrPoints[2].X) + 2, (int)Math.Ceiling(fMaxY - arrPoints[2].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROIBottom.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWBottom);

            objBWROI.LoadROISetting(0, (int)Math.Round(objBWBottom.ref_intImageHeight / 2 - fHeight / 2), (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIBottom1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgBottom1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineBottom.ref_dAngle > 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIBottom2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgBottom2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIBottom1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIBottom2.bmp");
                objROI.SaveImage("D:\\TS\\objROIBottom2.bmp");
            }
            Line objLineLeft = new Line();
            objLineLeft.CalculateStraightLine(arrPoints[0], arrPoints[2]);

            if (objLineLeft.ref_dAngle < 0)
                fAngle = 90 - (float)objLineLeft.ref_dAngle;
            else
                fAngle = (90 + (float)objLineLeft.ref_dAngle);

            if (objLineLeft.ref_dAngle < 0)
                objROI.LoadROISetting((int)Math.Floor(fMinX) - 1, (int)Math.Floor(arrPoints[0].Y) - 1, (int)Math.Ceiling(arrPoints[2].X - fMinX) + 2, (int)Math.Ceiling(arrPoints[2].Y - arrPoints[0].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(fMinX) - 1, (int)Math.Floor(arrPoints[0].Y) - 1, (int)Math.Ceiling(arrPoints[0].X - fMinX) + 2, (int)Math.Ceiling(arrPoints[2].Y - arrPoints[0].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROILeft.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWLeft);

            objBWROI.LoadROISetting((int)Math.Round(objBWLeft.ref_intImageWidth / 2 - fWidth / 2), 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROILeft1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgLeft1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineLeft.ref_dAngle < 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROILeft2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgLeft2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROILeft1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROILeft2.bmp");
                objROI.SaveImage("D:\\TS\\objROILeft2.bmp");
            }
            Line objLineRight = new Line();
            objLineRight.CalculateStraightLine(arrPoints[1], arrPoints[3]);

            if (objLineRight.ref_dAngle < 0)
                fAngle = 90 - (float)objLineRight.ref_dAngle;
            else
                fAngle = (90 + (float)objLineRight.ref_dAngle);

            if (objLineRight.ref_dAngle < 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[1].X) - 1, (int)Math.Floor(arrPoints[1].Y) - 1, (int)Math.Ceiling(fMaxX - arrPoints[1].X) + 2, (int)Math.Ceiling(arrPoints[3].Y - arrPoints[1].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[3].X) - 1, (int)Math.Floor(arrPoints[1].Y) - 1, (int)Math.Ceiling(fMaxX - arrPoints[3].X) + 2, (int)Math.Ceiling(arrPoints[3].Y - arrPoints[1].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROIRight.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWRight);

            objBWROI.LoadROISetting((int)Math.Round(objBWRight.ref_intImageWidth / 2 - fWidth / 2), 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIRight1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgRight1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineRight.ref_dAngle < 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIRight2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgRight2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIRight1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIRight2.bmp");
                objROI.SaveImage("D:\\TS\\objROIRight2.bmp");
                objDestinationImage.SaveImage("D:\\TS\\objDestinationImage.bmp");
                objTargetROI.SaveImage("D:\\TS\\objTargetROI.bmp");
            }
            objBWROI.Dispose();
            objTempImg.Dispose();
            objROI.Dispose();
            objDontCareROI.Dispose();
        }
        public static void ProduceImage_WithoutLoopMaxPoints(List<PointF> arrPoints, ROI objTargetROI, ImageDrawing objDestinationImage, ImageDrawing objBWTop, ImageDrawing objBWRight, ImageDrawing objBWBottom, ImageDrawing objBWLeft, bool blnWhiteDontCare)
        {
            bool blnDebugImage = false;

            if (blnDebugImage)
            {
                objBWTop.SaveImage("D:\\TS\\objBWTop.bmp");
                objBWRight.SaveImage("D:\\TS\\objBWRight.bmp");
                objBWBottom.SaveImage("D:\\TS\\objBWBottom.bmp");
                objBWLeft.SaveImage("D:\\TS\\objBWLeft.bmp");
            }

            float fMinX = objTargetROI.ref_ROITotalX;
            float fMinY = objTargetROI.ref_ROITotalY;
            float fMaxX = objTargetROI.ref_ROITotalX + objTargetROI.ref_ROIWidth;
            float fMaxY = objTargetROI.ref_ROITotalY + objTargetROI.ref_ROIHeight;
            //for (int i = 0; i < arrPoints.Count; i++)
            //{
            //    if (fMinX > arrPoints[i].X)
            //        fMinX = arrPoints[i].X;

            //    if (fMinY > arrPoints[i].Y)
            //        fMinY = arrPoints[i].Y;

            //    if (fMaxX < arrPoints[i].X)
            //        fMaxX = arrPoints[i].X;

            //    if (fMaxY < arrPoints[i].Y)
            //        fMaxY = arrPoints[i].Y;
            //}
            //objTargetROI.LoadROISetting((int)Math.Round(fMinX), (int)Math.Round(fMinY),
            //    (int)Math.Round(fMaxX - fMinX), (int)Math.Round(fMaxY - fMinY));

            ROI objBWROI = new ROI();
            ImageDrawing objTempImg = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);

            Line objLineTop = new Line();
            ROI objROI = new ROI();
            ROI objDontCareROI = new ROI();
            objLineTop.CalculateStraightLine(arrPoints[0], arrPoints[1]);

            objROI.AttachImage(objDestinationImage);

            float fAngle = 0;
            if (objLineTop.ref_dAngle > 0)
                fAngle = 90 - (float)objLineTop.ref_dAngle;
            else
                fAngle = (90 + (float)objLineTop.ref_dAngle);

            if (objLineTop.ref_dAngle > 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[0].X) - 1, (int)Math.Floor(fMinY) - 1, (int)Math.Ceiling(arrPoints[1].X - arrPoints[0].X) + 2, (int)Math.Ceiling(arrPoints[0].Y - fMinY) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[0].X) - 1, (int)Math.Floor(fMinY) - 1, (int)Math.Ceiling(arrPoints[1].X - arrPoints[0].X) + 2, (int)Math.Ceiling(arrPoints[1].Y - fMinY) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROITop.bmp");
            }
            float fWidth = 0, fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWTop);

            objBWROI.LoadROISetting(0, (int)Math.Round(objBWTop.ref_intImageHeight / 2 - fHeight / 2), (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROITop1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgTop1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineTop.ref_dAngle > 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROITop2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgTop2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROITop1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROITop2.bmp");
                objROI.SaveImage("D:\\TS\\objROITop2.bmp");
            }
            Line objLineBottom = new Line();
            objLineBottom.CalculateStraightLine(arrPoints[2], arrPoints[3]);
            if (objLineBottom.ref_dAngle > 0)
                fAngle = 90 - (float)objLineBottom.ref_dAngle;
            else
                fAngle = (90 + (float)objLineBottom.ref_dAngle);

            if (objLineBottom.ref_dAngle > 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[2].X) - 1, (int)Math.Floor(arrPoints[3].Y) - 1, (int)Math.Ceiling(arrPoints[3].X - arrPoints[2].X) + 2, (int)Math.Ceiling(fMaxY - arrPoints[3].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[2].X) - 1, (int)Math.Floor(arrPoints[2].Y) - 1, (int)Math.Ceiling(arrPoints[3].X - arrPoints[2].X) + 2, (int)Math.Ceiling(fMaxY - arrPoints[2].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROIBottom.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWBottom);

            objBWROI.LoadROISetting(0, (int)Math.Round(objBWBottom.ref_intImageHeight / 2 - fHeight / 2), (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIBottom1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgBottom1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineBottom.ref_dAngle > 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIBottom2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgBottom2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIBottom1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIBottom2.bmp");
                objROI.SaveImage("D:\\TS\\objROIBottom2.bmp");
            }
            Line objLineLeft = new Line();
            objLineLeft.CalculateStraightLine(arrPoints[0], arrPoints[2]);

            if (objLineLeft.ref_dAngle < 0)
                fAngle = 90 - (float)objLineLeft.ref_dAngle;
            else
                fAngle = (90 + (float)objLineLeft.ref_dAngle);

            if (objLineLeft.ref_dAngle < 0)
                objROI.LoadROISetting((int)Math.Floor(fMinX) - 1, (int)Math.Floor(arrPoints[0].Y) - 1, (int)Math.Ceiling(arrPoints[2].X - fMinX) + 2, (int)Math.Ceiling(arrPoints[2].Y - arrPoints[0].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(fMinX) - 1, (int)Math.Floor(arrPoints[0].Y) - 1, (int)Math.Ceiling(arrPoints[0].X - fMinX) + 2, (int)Math.Ceiling(arrPoints[2].Y - arrPoints[0].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROILeft.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWLeft);

            objBWROI.LoadROISetting((int)Math.Round(objBWLeft.ref_intImageWidth / 2 - fWidth / 2), 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROILeft1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgLeft1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineLeft.ref_dAngle < 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROILeft2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgLeft2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROILeft1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROILeft2.bmp");
                objROI.SaveImage("D:\\TS\\objROILeft2.bmp");
            }
            Line objLineRight = new Line();
            objLineRight.CalculateStraightLine(arrPoints[1], arrPoints[3]);

            if (objLineRight.ref_dAngle < 0)
                fAngle = 90 - (float)objLineRight.ref_dAngle;
            else
                fAngle = (90 + (float)objLineRight.ref_dAngle);

            if (objLineRight.ref_dAngle < 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[1].X) - 1, (int)Math.Floor(arrPoints[1].Y) - 1, (int)Math.Ceiling(fMaxX - arrPoints[1].X) + 2, (int)Math.Ceiling(arrPoints[3].Y - arrPoints[1].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[3].X) - 1, (int)Math.Floor(arrPoints[1].Y) - 1, (int)Math.Ceiling(fMaxX - arrPoints[3].X) + 2, (int)Math.Ceiling(arrPoints[3].Y - arrPoints[1].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROIRight.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWRight);

            objBWROI.LoadROISetting((int)Math.Round(objBWRight.ref_intImageWidth / 2 - fWidth / 2), 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIRight1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgRight1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineRight.ref_dAngle < 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIRight2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgRight2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIRight1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIRight2.bmp");
                objROI.SaveImage("D:\\TS\\objROIRight2.bmp");
                objDestinationImage.SaveImage("D:\\TS\\objDestinationImage.bmp");
                objTargetROI.SaveImage("D:\\TS\\objTargetROI.bmp");
            }
            objBWROI.Dispose();
            objTempImg.Dispose();
            objROI.Dispose();
            objDontCareROI.Dispose();
        }
        public static void ProduceImageInner(List<PointF> arrPoints, ROI objTargetROI, ImageDrawing objDestinationImage, ImageDrawing objBWTop, ImageDrawing objBWRight, ImageDrawing objBWBottom, ImageDrawing objBWLeft, bool blnWhiteDontCare)
        {
            bool blnDebugImage = false;

            if (blnDebugImage)
            {
                objBWTop.SaveImage("D:\\TS\\objBWTop.bmp");
                objBWRight.SaveImage("D:\\TS\\objBWRight.bmp");
                objBWBottom.SaveImage("D:\\TS\\objBWBottom.bmp");
                objBWLeft.SaveImage("D:\\TS\\objBWLeft.bmp");
            }

            float fMinX = float.MaxValue;
            float fMinY = float.MaxValue;
            float fMaxX = 0;
            float fMaxY = 0;
            for (int i = 0; i < arrPoints.Count; i++)
            {
                if (fMinX > arrPoints[i].X)
                    fMinX = arrPoints[i].X;

                if (fMinY > arrPoints[i].Y)
                    fMinY = arrPoints[i].Y;

                if (fMaxX < arrPoints[i].X)
                    fMaxX = arrPoints[i].X;

                if (fMaxY < arrPoints[i].Y)
                    fMaxY = arrPoints[i].Y;
            }

            if (fMinX < 0)
                fMinX = 0;
            if (fMinY < 0)
                fMinY = 0;
            if (fMaxX > objDestinationImage.ref_intImageWidth)
                fMaxX = objDestinationImage.ref_intImageWidth;
            if (fMaxY > objDestinationImage.ref_intImageHeight)
                fMaxY = objDestinationImage.ref_intImageHeight;

            objTargetROI.LoadROISetting((int)Math.Round(fMinX), (int)Math.Round(fMinY),
                (int)Math.Round(fMaxX - fMinX), (int)Math.Round(fMaxY - fMinY));

            ROI objBWROI = new ROI();
            ImageDrawing objTempImg = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);

            Line objLineTop = new Line();
            ROI objROI = new ROI();
            ROI objDontCareROI = new ROI();
            objLineTop.CalculateStraightLine(arrPoints[0], arrPoints[1]);

            objROI.AttachImage(objDestinationImage);

            float fAngle = 0;
            if (objLineTop.ref_dAngle > 0)
                fAngle = 90 - (float)objLineTop.ref_dAngle;
            else
                fAngle = (90 + (float)objLineTop.ref_dAngle);

            if (objLineTop.ref_dAngle > 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[0].X) - 1, (int)Math.Floor(fMinY) - 1, (int)Math.Ceiling(arrPoints[1].X - arrPoints[0].X) + 2, (int)Math.Ceiling(arrPoints[0].Y - fMinY) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[0].X) - 1, (int)Math.Floor(fMinY) - 1, (int)Math.Ceiling(arrPoints[1].X - arrPoints[0].X) + 2, (int)Math.Ceiling(arrPoints[1].Y - fMinY) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROITop.bmp");
            }
            float fWidth = 0, fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWTop);

            objBWROI.LoadROISetting(0, (int)Math.Round(objBWTop.ref_intImageHeight / 2 - fHeight / 2), (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROITop1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgTop1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineTop.ref_dAngle > 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROITop2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgTop2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROITop1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            ROI.InvertOperationROI(objDontCareROI);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROITop2.bmp");
                objROI.SaveImage("D:\\TS\\objROITop2.bmp");
            }
            Line objLineBottom = new Line();
            objLineBottom.CalculateStraightLine(arrPoints[2], arrPoints[3]);
            if (objLineBottom.ref_dAngle > 0)
                fAngle = 90 - (float)objLineBottom.ref_dAngle;
            else
                fAngle = (90 + (float)objLineBottom.ref_dAngle);

            if (objLineBottom.ref_dAngle > 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[2].X) - 1, (int)Math.Floor(arrPoints[3].Y) - 1, (int)Math.Ceiling(arrPoints[3].X - arrPoints[2].X) + 2, (int)Math.Ceiling(fMaxY - arrPoints[3].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[2].X) - 1, (int)Math.Floor(arrPoints[2].Y) - 1, (int)Math.Ceiling(arrPoints[3].X - arrPoints[2].X) + 2, (int)Math.Ceiling(fMaxY - arrPoints[2].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROIBottom.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWBottom);

            objBWROI.LoadROISetting(0, (int)Math.Round(objBWBottom.ref_intImageHeight / 2 - fHeight / 2), (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIBottom1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgBottom1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineBottom.ref_dAngle > 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIBottom2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgBottom2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIBottom1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            ROI.InvertOperationROI(objDontCareROI);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIBottom2.bmp");
                objROI.SaveImage("D:\\TS\\objROIBottom2.bmp");
            }
            Line objLineLeft = new Line();
            objLineLeft.CalculateStraightLine(arrPoints[0], arrPoints[2]);

            if (objLineLeft.ref_dAngle < 0)
                fAngle = 90 - (float)objLineLeft.ref_dAngle;
            else
                fAngle = (90 + (float)objLineLeft.ref_dAngle);

            if (objLineLeft.ref_dAngle < 0)
                objROI.LoadROISetting((int)Math.Floor(fMinX) - 1, (int)Math.Floor(arrPoints[0].Y) - 1, (int)Math.Ceiling(arrPoints[2].X - fMinX) + 2, (int)Math.Ceiling(arrPoints[2].Y - arrPoints[0].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(fMinX) - 1, (int)Math.Floor(arrPoints[0].Y) - 1, (int)Math.Ceiling(arrPoints[0].X - fMinX) + 2, (int)Math.Ceiling(arrPoints[2].Y - arrPoints[0].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROILeft.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWLeft);

            objBWROI.LoadROISetting((int)Math.Round(objBWLeft.ref_intImageWidth / 2 - fWidth / 2), 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROILeft1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgLeft1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineLeft.ref_dAngle < 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROILeft2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgLeft2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROILeft1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            ROI.InvertOperationROI(objDontCareROI);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROILeft2.bmp");
                objROI.SaveImage("D:\\TS\\objROILeft2.bmp");
            }
            Line objLineRight = new Line();
            objLineRight.CalculateStraightLine(arrPoints[1], arrPoints[3]);

            if (objLineRight.ref_dAngle < 0)
                fAngle = 90 - (float)objLineRight.ref_dAngle;
            else
                fAngle = (90 + (float)objLineRight.ref_dAngle);

            if (objLineRight.ref_dAngle < 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[1].X) - 1, (int)Math.Floor(arrPoints[1].Y) - 1, (int)Math.Ceiling(fMaxX - arrPoints[1].X) + 2, (int)Math.Ceiling(arrPoints[3].Y - arrPoints[1].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[3].X) - 1, (int)Math.Floor(arrPoints[1].Y) - 1, (int)Math.Ceiling(fMaxX - arrPoints[3].X) + 2, (int)Math.Ceiling(arrPoints[3].Y - arrPoints[1].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROIRight.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBWRight);

            objBWROI.LoadROISetting((int)Math.Round(objBWRight.ref_intImageWidth / 2 - fWidth / 2), 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIRight1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgRight1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineRight.ref_dAngle < 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIRight2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgRight2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIRight1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            ROI.InvertOperationROI(objDontCareROI);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIRight2.bmp");
                objROI.SaveImage("D:\\TS\\objROIRight2.bmp");
                objDestinationImage.SaveImage("D:\\TS\\objDestinationImage.bmp");
                objTargetROI.SaveImage("D:\\TS\\objTargetROI.bmp");
            }
            objBWROI.Dispose();
            objTempImg.Dispose();
            objROI.Dispose();
            objDontCareROI.Dispose();
        }
        public static void ProduceImage(List<Point> arrPoints, ImageDrawing objDestinationImage, ImageDrawing objWhiteImg, ImageDrawing objBlackImg, float fOrientAngle, bool blnWhiteDontCare)
        {
            bool blnDebugImage = false;
            int intOriWidth = Math.Abs(arrPoints[1].X - arrPoints[0].X), intOriHeight = Math.Abs(arrPoints[2].Y - arrPoints[0].Y);
            float fCenterX = (arrPoints[2].X + arrPoints[0].X) / 2, fCenterY = (arrPoints[2].Y + arrPoints[0].Y) / 2;
            PointF pTemp = new PointF();
            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[0],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[0] = new Point((int)Math.Round(pTemp.X), (int)Math.Round(pTemp.Y));

            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[1],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[1] = new Point((int)Math.Round(pTemp.X), (int)Math.Round(pTemp.Y));

            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[2],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[2] = new Point((int)Math.Round(pTemp.X), (int)Math.Round(pTemp.Y));

            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[3],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[3] = new Point((int)Math.Round(pTemp.X), (int)Math.Round(pTemp.Y));

            float fMinX = float.MaxValue;
            float fMinY = float.MaxValue;
            float fMaxX = 0;
            float fMaxY = 0;
            for (int i = 0; i < arrPoints.Count; i++)
            {
                if (fMinX > arrPoints[i].X)
                    fMinX = arrPoints[i].X;

                if (fMinY > arrPoints[i].Y)
                    fMinY = arrPoints[i].Y;

                if (fMaxX < arrPoints[i].X)
                    fMaxX = arrPoints[i].X;

                if (fMaxY < arrPoints[i].Y)
                    fMaxY = arrPoints[i].Y;
            }
            
            if (fMinX < 0)
            {
                //if ((fMaxX + Math.Abs(fMinX)) <= objDestinationImage.ref_intImageWidth)
                //    fMaxX += Math.Abs(fMinX);

                fMinX = 0;
            }
            if (fMinY < 0)
            {
                //if ((fMaxY + Math.Abs(fMinY)) <= objDestinationImage.ref_intImageHeight)
                //    fMaxY += Math.Abs(fMinY);

                fMinY = 0;
            }
            if (fMaxX > objDestinationImage.ref_intImageWidth)
            {
                //if((fMinX - (fMaxX - objDestinationImage.ref_intImageWidth)) > 0)
                //    fMinX = (fMaxX - objDestinationImage.ref_intImageWidth);

                fMaxX = objDestinationImage.ref_intImageWidth;
            }
            if (fMaxY > objDestinationImage.ref_intImageHeight)
            {
                //if ((fMinY - (fMaxY - objDestinationImage.ref_intImageHeight)) > 0)
                //    fMinY = (fMaxY - objDestinationImage.ref_intImageHeight);

                fMaxY = objDestinationImage.ref_intImageHeight;
            }
            
            ROI objBWROI = new ROI();
            ImageDrawing objTempImg = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);
            
            ROI objROI = new ROI();
            ROI objDontCareROI = new ROI();
            
            objROI.AttachImage(objDestinationImage);
            
            objROI.LoadROISetting((int)Math.Floor(fMinX), (int)Math.Floor(fMinY), (int)Math.Ceiling(fMaxX - fMinX), (int)Math.Ceiling(fMaxY - fMinY));
            
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROI.bmp");
            }
            float fWidth = objROI.ref_ROIWidth, fHeight = objROI.ref_ROIHeight;

            objDontCareROI.AttachImage(objBlackImg);
            objDontCareROI.LoadROISetting((int)Math.Round(Math.Abs(intOriWidth - fWidth) / 2), (int)Math.Round(Math.Abs(intOriHeight - fHeight) / 2), intOriWidth, intOriHeight);
            objDontCareROI.CopyToImage(ref objTempImg);
            objBWROI.AttachImage(objTempImg);

            objBWROI.LoadROISetting((int)Math.Round(Math.Abs(intOriWidth - fWidth) / 2), (int)Math.Round(Math.Abs(intOriHeight - fHeight) / 2), intOriWidth, intOriHeight);
            
            objDontCareROI.AttachImage(objWhiteImg);
            //objDontCareROI.LoadROISetting((int)Math.Round(Math.Abs(intOriWidth - fWidth) / 2), (int)Math.Round(Math.Abs(intOriHeight - fHeight) / 2), intOriWidth, intOriHeight);

            ROI.LogicOperationAddROI(objBWROI, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROI1.bmp");
            }
            objBWROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            ImageDrawing objTempImg2 = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);
            //objTempImg2.SetImageToBlack();
            objDontCareROI.AttachImage(objTempImg2);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROI2.bmp");
            }

            ROI.Rotate0Degree_ForDontCare(objBWROI, -fOrientAngle, 0, objDontCareROI);

            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROI3.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROI1.bmp");
            }
          
            if (blnWhiteDontCare)
            {
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            }
            else
            {
                ROI.SubtractROI(objROI, objDontCareROI);
            }

            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROI2.bmp");
                objDestinationImage.SaveImage("D:\\TS\\objDestinationImage.bmp");
            }

            objBWROI.Dispose();
            objTempImg.Dispose();
            objTempImg2.Dispose();
            objROI.Dispose();
            objDontCareROI.Dispose();
        }
        public static void ProduceImage_ForBarcode(List<Point> arrPoints, ImageDrawing objDestinationImage, ImageDrawing objWhiteImg, ImageDrawing objBlackImg, float fOrientAngle, bool blnWhiteDontCare, float fDontCareScale)
        {
            bool blnDebugImage = false;
            int intOriWidth = Math.Max(5, Math.Abs(arrPoints[1].X - arrPoints[0].X)), intOriHeight = Math.Max(5, Math.Abs(arrPoints[2].Y - arrPoints[0].Y));
            float fCenterX = (arrPoints[2].X + arrPoints[0].X) / 2, fCenterY = (arrPoints[2].Y + arrPoints[0].Y) / 2;
            PointF pTemp = new PointF();
            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[0],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[0] = new Point((int)Math.Round(pTemp.X), (int)Math.Round(pTemp.Y));

            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[1],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[1] = new Point((int)Math.Round(pTemp.X), (int)Math.Round(pTemp.Y));

            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[2],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[2] = new Point((int)Math.Round(pTemp.X), (int)Math.Round(pTemp.Y));

            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[3],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[3] = new Point((int)Math.Round(pTemp.X), (int)Math.Round(pTemp.Y));

            float fMinX = float.MaxValue;
            float fMinY = float.MaxValue;
            float fMaxX = 0;
            float fMaxY = 0;
            for (int i = 0; i < arrPoints.Count; i++)
            {
                if (fMinX > arrPoints[i].X)
                    fMinX = arrPoints[i].X;

                if (fMinY > arrPoints[i].Y)
                    fMinY = arrPoints[i].Y;

                if (fMaxX < arrPoints[i].X)
                    fMaxX = arrPoints[i].X;

                if (fMaxY < arrPoints[i].Y)
                    fMaxY = arrPoints[i].Y;
            }

            fMinX = Math.Min(fCenterX - (intOriWidth / 2), fMinX);
            fMinY = Math.Min(fCenterY - (intOriHeight / 2), fMinY);
            fMaxX = Math.Max(fCenterX + (intOriWidth / 2), fMaxX);
            fMaxY = Math.Max(fCenterY + (intOriHeight / 2), fMaxY);

            if (fMinX < 0)
            {
                //if ((fMaxX + Math.Abs(fMinX)) <= objDestinationImage.ref_intImageWidth)
                //    fMaxX += Math.Abs(fMinX);
                fMaxX -= Math.Abs(fMinX);
                fMinX = 0;
            }
            if (fMinY < 0)
            {
                //if ((fMaxY + Math.Abs(fMinY)) <= objDestinationImage.ref_intImageHeight)
                //    fMaxY += Math.Abs(fMinY);
                fMaxY -= Math.Abs(fMinY);
                fMinY = 0;
            }
            if (fMaxX > objDestinationImage.ref_intImageWidth)
            {
                //if((fMinX - (fMaxX - objDestinationImage.ref_intImageWidth)) > 0)
                //    fMinX = (fMaxX - objDestinationImage.ref_intImageWidth);
                fMinX += (fMaxX - objDestinationImage.ref_intImageWidth);
                fMaxX = objDestinationImage.ref_intImageWidth;
            }
            if (fMaxY > objDestinationImage.ref_intImageHeight)
            {
                //if ((fMinY - (fMaxY - objDestinationImage.ref_intImageHeight)) > 0)
                //    fMinY = (fMaxY - objDestinationImage.ref_intImageHeight);
                fMinY += (fMaxY - objDestinationImage.ref_intImageHeight);
                fMaxY = objDestinationImage.ref_intImageHeight;
            }

            //if (fMinX < 0)
            //{
            //    //if ((fMaxX + Math.Abs(fMinX / 2)) <= objDestinationImage.ref_intImageWidth)
            //        fMaxX += Math.Abs(fMinX / 2);

            //    fMinX /= 2;
            //}
            //if (fMinY < 0)
            //{
            //    //if ((fMaxY + Math.Abs(fMinY / 2)) <= objDestinationImage.ref_intImageHeight)
            //        fMaxY += Math.Abs(fMinY / 2);

            //    fMinY /= 2;
            //}
            //if (fMaxX > objDestinationImage.ref_intImageWidth)
            //{
            //    //if ((fMinX - (fMaxX - objDestinationImage.ref_intImageWidth) / 2) > 0)
            //        fMinX = (fMaxX - objDestinationImage.ref_intImageWidth) / 2;

            //    fMaxX = objDestinationImage.ref_intImageWidth - (fMaxX - objDestinationImage.ref_intImageWidth) / 2;
            //}
            //if (fMaxY > objDestinationImage.ref_intImageHeight)
            //{
            //    //if ((fMinY - (fMaxY - objDestinationImage.ref_intImageHeight) / 2) > 0)
            //        fMinY = (fMaxY - objDestinationImage.ref_intImageHeight) / 2;

            //    fMaxY = objDestinationImage.ref_intImageHeight - (fMaxY - objDestinationImage.ref_intImageHeight) / 2;
            //}

            ROI objBWROI = new ROI();
            ImageDrawing objTempImg = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);

            ROI objROI = new ROI();
            ROI objDontCareROI = new ROI();

            objROI.AttachImage(objDestinationImage);

            objROI.LoadROISetting((int)Math.Floor(fMinX), (int)Math.Floor(fMinY), Math.Max(5, (int)Math.Ceiling(fMaxX - fMinX)), Math.Max(5, (int)Math.Ceiling(fMaxY - fMinY)));

            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROI.bmp");
            }
            float fWidth = objROI.ref_ROIWidth, fHeight = objROI.ref_ROIHeight;

            if (intOriWidth >= intOriHeight)
                intOriHeight = (int)(intOriHeight * fDontCareScale);
            else if (intOriHeight >= intOriWidth)
                intOriWidth = (int)(intOriWidth * fDontCareScale);

            objDontCareROI.AttachImage(objBlackImg);
            //objDontCareROI.LoadROISetting((int)Math.Round(Math.Abs(intOriWidth - fWidth) / 2), (int)Math.Round(Math.Abs(intOriHeight - fHeight) / 2), intOriWidth, intOriHeight);
            objDontCareROI.LoadROISetting((int)Math.Round(fCenterX), (int)Math.Round(fCenterY), intOriWidth, intOriHeight);
            objDontCareROI.CopyToImage(ref objTempImg);
            objBWROI.AttachImage(objTempImg);

            //objBWROI.LoadROISetting((int)Math.Round(Math.Abs(intOriWidth - fWidth) / 2), (int)Math.Round(Math.Abs(intOriHeight - fHeight) / 2), intOriWidth, intOriHeight);
            objBWROI.LoadROISetting((int)Math.Round(fCenterX), (int)Math.Round(fCenterY), intOriWidth, intOriHeight);

            objDontCareROI.AttachImage(objWhiteImg);
            //objDontCareROI.LoadROISetting((int)Math.Round(Math.Abs(intOriWidth - fWidth) / 2), (int)Math.Round(Math.Abs(intOriHeight - fHeight) / 2), intOriWidth, intOriHeight);

            ROI.LogicOperationAddROI(objBWROI, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROI1.bmp");
            }
            objBWROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            ImageDrawing objTempImg2 = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);
            //objTempImg2.SetImageToBlack();
            objDontCareROI.AttachImage(objTempImg2);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROI2.bmp");
            }

            ROI.Rotate0Degree_ForDontCare(objBWROI, -fOrientAngle, 0, objDontCareROI);

            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROI3.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROI1.bmp");
            }

            objDontCareROI.LoadROISetting(objDontCareROI.ref_ROITotalCenterX - objROI.ref_ROIWidth / 2,
                objDontCareROI.ref_ROITotalCenterY - objROI.ref_ROIHeight / 2,
                objROI.ref_ROIWidth,
                objROI.ref_ROIHeight);

            if (blnWhiteDontCare)
            {
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            }
            else
            {
                ROI.SubtractROI(objROI, objDontCareROI);
            }

            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROI2.bmp");
                objDestinationImage.SaveImage("D:\\TS\\objDestinationImage.bmp");
            }

            objBWROI.Dispose();
            objTempImg.Dispose();
            objTempImg2.Dispose();
            objROI.Dispose();
            objDontCareROI.Dispose();
        }
        public static void ProduceImage_ForLead(List<Point> arrPoints, ImageDrawing objDestinationImage, ImageDrawing objWhiteImg, ImageDrawing objBlackImg, float fOrientAngle, bool blnWhiteDontCare, ROI objObjectROI)
        {
            bool blnDebugImage = false;
            int intOriWidth = Math.Abs(arrPoints[1].X - arrPoints[0].X), intOriHeight = Math.Abs(arrPoints[2].Y - arrPoints[0].Y);
            float fCenterX = (arrPoints[2].X + arrPoints[0].X) / 2, fCenterY = (arrPoints[2].Y + arrPoints[0].Y) / 2;
            PointF pTemp = new PointF();
            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[0],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[0] = new Point((int)Math.Round(pTemp.X), (int)Math.Round(pTemp.Y));

            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[1],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[1] = new Point((int)Math.Round(pTemp.X), (int)Math.Round(pTemp.Y));

            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[2],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[2] = new Point((int)Math.Round(pTemp.X), (int)Math.Round(pTemp.Y));

            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[3],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[3] = new Point((int)Math.Round(pTemp.X), (int)Math.Round(pTemp.Y));

            float fMinX = float.MaxValue;
            float fMinY = float.MaxValue;
            float fMaxX = 0;
            float fMaxY = 0;
            for (int i = 0; i < arrPoints.Count; i++)
            {
                if (fMinX > arrPoints[i].X)
                    fMinX = arrPoints[i].X;

                if (fMinY > arrPoints[i].Y)
                    fMinY = arrPoints[i].Y;

                if (fMaxX < arrPoints[i].X)
                    fMaxX = arrPoints[i].X;

                if (fMaxY < arrPoints[i].Y)
                    fMaxY = arrPoints[i].Y;
            }

            fMinX = Math.Min(fCenterX - (intOriWidth / 2), fMinX);
            fMinY = Math.Min(fCenterY - (intOriHeight / 2), fMinY);
            fMaxX = Math.Max(fCenterX + (intOriWidth / 2), fMaxX);
            fMaxY = Math.Max(fCenterY + (intOriHeight / 2), fMaxY);

            if (fMinX < 0)
            {
                //if ((fMaxX + Math.Abs(fMinX)) <= objDestinationImage.ref_intImageWidth)
                //    fMaxX += Math.Abs(fMinX);
                fMaxX -= Math.Abs(fMinX);
                fMinX = 0;
            }
            if (fMinY < 0)
            {
                //if ((fMaxY + Math.Abs(fMinY)) <= objDestinationImage.ref_intImageHeight)
                //    fMaxY += Math.Abs(fMinY);
                fMaxY -= Math.Abs(fMinY);
                fMinY = 0;
            }
            if (fMaxX > objDestinationImage.ref_intImageWidth)
            {
                //if((fMinX - (fMaxX - objDestinationImage.ref_intImageWidth)) > 0)
                //    fMinX = (fMaxX - objDestinationImage.ref_intImageWidth);
                fMinX += (fMaxX - objDestinationImage.ref_intImageWidth);
                fMaxX = objDestinationImage.ref_intImageWidth;
            }
            if (fMaxY > objDestinationImage.ref_intImageHeight)
            {
                //if ((fMinY - (fMaxY - objDestinationImage.ref_intImageHeight)) > 0)
                //    fMinY = (fMaxY - objDestinationImage.ref_intImageHeight);
                fMinY += (fMaxY - objDestinationImage.ref_intImageHeight);
                fMaxY = objDestinationImage.ref_intImageHeight;
            }

            //if (fMinX < 0)
            //{
            //    //if ((fMaxX + Math.Abs(fMinX / 2)) <= objDestinationImage.ref_intImageWidth)
            //        fMaxX += Math.Abs(fMinX / 2);

            //    fMinX /= 2;
            //}
            //if (fMinY < 0)
            //{
            //    //if ((fMaxY + Math.Abs(fMinY / 2)) <= objDestinationImage.ref_intImageHeight)
            //        fMaxY += Math.Abs(fMinY / 2);

            //    fMinY /= 2;
            //}
            //if (fMaxX > objDestinationImage.ref_intImageWidth)
            //{
            //    //if ((fMinX - (fMaxX - objDestinationImage.ref_intImageWidth) / 2) > 0)
            //        fMinX = (fMaxX - objDestinationImage.ref_intImageWidth) / 2;

            //    fMaxX = objDestinationImage.ref_intImageWidth - (fMaxX - objDestinationImage.ref_intImageWidth) / 2;
            //}
            //if (fMaxY > objDestinationImage.ref_intImageHeight)
            //{
            //    //if ((fMinY - (fMaxY - objDestinationImage.ref_intImageHeight) / 2) > 0)
            //        fMinY = (fMaxY - objDestinationImage.ref_intImageHeight) / 2;

            //    fMaxY = objDestinationImage.ref_intImageHeight - (fMaxY - objDestinationImage.ref_intImageHeight) / 2;
            //}

            ROI objBWROI = new ROI();
            ImageDrawing objTempImg = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);

            ROI objROI = new ROI();
            ROI objDontCareROI = new ROI();

            objROI.AttachImage(objDestinationImage);

            objROI.LoadROISetting((int)Math.Floor(fMinX), (int)Math.Floor(fMinY), (int)Math.Ceiling(fMaxX - fMinX), (int)Math.Ceiling(fMaxY - fMinY));

            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROI.bmp");
            }
            float fWidth = objROI.ref_ROIWidth, fHeight = objROI.ref_ROIHeight;

            objDontCareROI.AttachImage(objBlackImg);
            //objDontCareROI.LoadROISetting((int)Math.Round(Math.Abs(intOriWidth - fWidth) / 2), (int)Math.Round(Math.Abs(intOriHeight - fHeight) / 2), intOriWidth, intOriHeight);
            objDontCareROI.LoadROISetting((int)Math.Round(fCenterX), (int)Math.Round(fCenterY), intOriWidth, intOriHeight);
            objDontCareROI.CopyToImage(ref objTempImg);
            objBWROI.AttachImage(objTempImg);

            //objBWROI.LoadROISetting((int)Math.Round(Math.Abs(intOriWidth - fWidth) / 2), (int)Math.Round(Math.Abs(intOriHeight - fHeight) / 2), intOriWidth, intOriHeight);
            objBWROI.LoadROISetting((int)Math.Round(fCenterX), (int)Math.Round(fCenterY), intOriWidth, intOriHeight);

            objDontCareROI.AttachImage(objWhiteImg);
            //objDontCareROI.LoadROISetting((int)Math.Round(Math.Abs(intOriWidth - fWidth) / 2), (int)Math.Round(Math.Abs(intOriHeight - fHeight) / 2), intOriWidth, intOriHeight);

            ROI.LogicOperationAddROI(objBWROI, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROI1.bmp");
            }
            objBWROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            ImageDrawing objTempImg2 = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);
            //objTempImg2.SetImageToBlack();
            objDontCareROI.AttachImage(objTempImg2);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROI2.bmp");
            }

            ROI.Rotate0Degree_ForDontCare(objBWROI, -fOrientAngle, 0, objDontCareROI);

            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROI3.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROI1.bmp");
            }

            ROI.InvertOperationROI(objDontCareROI);

            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROI2.bmp");
            }

            if (blnWhiteDontCare)
            {
                ROI.SubtractROI(objROI, objDontCareROI);
            }
            else
            {
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            }

            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROI2.bmp");
                objDestinationImage.SaveImage("D:\\TS\\objDestinationImage.bmp");
            }

            objBWROI.Dispose();
            objTempImg.Dispose();
            objTempImg2.Dispose();
            objROI.Dispose();
            objDontCareROI.Dispose();
        }
        public static void ProduceImage_WithBiggerScale(List<Point> arrPoints, ImageDrawing objDestinationImage, ImageDrawing objWhiteImg, ImageDrawing objBlackImg, float fOrientAngle, bool blnWhiteDontCare, int intTolX, int intTolY)
        {
            bool blnDebugImage = false;
            int intOriWidth = Math.Abs(arrPoints[1].X - arrPoints[0].X) + intTolX, intOriHeight = Math.Abs(arrPoints[2].Y - arrPoints[0].Y) + intTolY;
            float fCenterX = (arrPoints[2].X + arrPoints[0].X) / 2, fCenterY = (arrPoints[2].Y + arrPoints[0].Y) / 2;
            PointF pTemp = new PointF();
            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[0],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[0] = new Point((int)Math.Round(pTemp.X - intTolX), (int)Math.Round(pTemp.Y - intTolY));

            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[1],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[1] = new Point((int)Math.Round(pTemp.X + intTolX), (int)Math.Round(pTemp.Y - intTolY));

            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[2],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[2] = new Point((int)Math.Round(pTemp.X + intTolX), (int)Math.Round(pTemp.Y + intTolY));

            Math2.GetNewXYAfterRotate_360deg(fCenterX, fCenterY,
                                             arrPoints[3],
                                             (fOrientAngle),
                                             ref pTemp);
            arrPoints[3] = new Point((int)Math.Round(pTemp.X - intTolX), (int)Math.Round(pTemp.Y + intTolY));

            float fMinX = float.MaxValue;
            float fMinY = float.MaxValue;
            float fMaxX = 0;
            float fMaxY = 0;
            for (int i = 0; i < arrPoints.Count; i++)
            {
                if (fMinX > arrPoints[i].X)
                    fMinX = arrPoints[i].X;

                if (fMinY > arrPoints[i].Y)
                    fMinY = arrPoints[i].Y;

                if (fMaxX < arrPoints[i].X)
                    fMaxX = arrPoints[i].X;

                if (fMaxY < arrPoints[i].Y)
                    fMaxY = arrPoints[i].Y;
            }

            if (fMinX < 0)
            {
                //if ((fMaxX + Math.Abs(fMinX)) <= objDestinationImage.ref_intImageWidth)
                //    fMaxX += Math.Abs(fMinX);

                fMinX = 0;
            }
            if (fMinY < 0)
            {
                //if ((fMaxY + Math.Abs(fMinY)) <= objDestinationImage.ref_intImageHeight)
                //    fMaxY += Math.Abs(fMinY);

                fMinY = 0;
            }
            if (fMaxX > objDestinationImage.ref_intImageWidth)
            {
                //if((fMinX - (fMaxX - objDestinationImage.ref_intImageWidth)) > 0)
                //    fMinX = (fMaxX - objDestinationImage.ref_intImageWidth);

                fMaxX = objDestinationImage.ref_intImageWidth;
            }
            if (fMaxY > objDestinationImage.ref_intImageHeight)
            {
                //if ((fMinY - (fMaxY - objDestinationImage.ref_intImageHeight)) > 0)
                //    fMinY = (fMaxY - objDestinationImage.ref_intImageHeight);

                fMaxY = objDestinationImage.ref_intImageHeight;
            }

            ROI objBWROI = new ROI();
            ImageDrawing objTempImg = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);

            ROI objROI = new ROI();
            ROI objDontCareROI = new ROI();

            objROI.AttachImage(objDestinationImage);

            objROI.LoadROISetting((int)Math.Floor(fMinX), (int)Math.Floor(fMinY), (int)Math.Ceiling(fMaxX - fMinX), (int)Math.Ceiling(fMaxY - fMinY));

            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROI.bmp");
            }
            float fWidth = objROI.ref_ROIWidth, fHeight = objROI.ref_ROIHeight;

            objDontCareROI.AttachImage(objBlackImg);
            objDontCareROI.LoadROISetting((int)Math.Round(Math.Abs(intOriWidth - fWidth) / 2), (int)Math.Round(Math.Abs(intOriHeight - fHeight) / 2), intOriWidth, intOriHeight);
            objDontCareROI.CopyToImage(ref objTempImg);
            objBWROI.AttachImage(objTempImg);

            objBWROI.LoadROISetting((int)Math.Round(Math.Abs(intOriWidth - fWidth) / 2), (int)Math.Round(Math.Abs(intOriHeight - fHeight) / 2), intOriWidth, intOriHeight);

            objDontCareROI.AttachImage(objWhiteImg);
            //objDontCareROI.LoadROISetting((int)Math.Round(Math.Abs(intOriWidth - fWidth) / 2), (int)Math.Round(Math.Abs(intOriHeight - fHeight) / 2), intOriWidth, intOriHeight);

            ROI.LogicOperationAddROI(objBWROI, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROI1.bmp");
            }
            objBWROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            ImageDrawing objTempImg2 = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);
            //objTempImg2.SetImageToBlack();
            objDontCareROI.AttachImage(objTempImg2);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROI2.bmp");
            }

            ROI.Rotate0Degree_ForDontCare(objBWROI, -fOrientAngle, 0, objDontCareROI);

            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROI3.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROI1.bmp");
            }

            if (blnWhiteDontCare)
            {
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            }
            else
            {
                ROI.SubtractROI(objROI, objDontCareROI);
            }

            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROI2.bmp");
                objDestinationImage.SaveImage("D:\\TS\\objDestinationImage.bmp");
            }

            objBWROI.Dispose();
            objTempImg.Dispose();
            objTempImg2.Dispose();
            objROI.Dispose();
            objDontCareROI.Dispose();
        }
        public static void ProduceImage_Line(List<PointF> arrPoints, ImageDrawing objDestinationImage, ImageDrawing objWhiteImage, ImageDrawing objBlackImage, bool blnWhiteDontCare)
        {
            bool blnDebugImage = false;

            float fMinX = float.MaxValue;
            float fMinY = float.MaxValue;
            float fMaxX = 0;
            float fMaxY = 0;
            for (int i = 0; i < arrPoints.Count; i++)
            {
                if (fMinX > arrPoints[i].X)
                    fMinX = arrPoints[i].X;

                if (fMinY > arrPoints[i].Y)
                    fMinY = arrPoints[i].Y;

                if (fMaxX < arrPoints[i].X)
                    fMaxX = arrPoints[i].X;

                if (fMaxY < arrPoints[i].Y)
                    fMaxY = arrPoints[i].Y;
            }

            if (fMinX < 0)
                fMinX = 0;
            if (fMinY < 0)
                fMinY = 0;
            if (fMaxX > objDestinationImage.ref_intImageWidth)
                fMaxX = objDestinationImage.ref_intImageWidth;
            if (fMaxY > objDestinationImage.ref_intImageHeight)
                fMaxY = objDestinationImage.ref_intImageHeight;

            ROI objBWROI = new ROI();
            ImageDrawing objTempImg = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);
            ImageDrawing objTempImg2 = new ImageDrawing(true, objDestinationImage.ref_intImageWidth, objDestinationImage.ref_intImageHeight);

            Line objLineTop = new Line();
            ROI objROI = new ROI();
            ROI objDontCareROI = new ROI();
            objLineTop.CalculateStraightLine(arrPoints[0], arrPoints[1]);

            objROI.AttachImage(objDestinationImage);

            float fAngle = 0;
            if (objLineTop.ref_dAngle > 0)
                fAngle = 90 - (float)objLineTop.ref_dAngle;
            else
                fAngle = (90 + (float)objLineTop.ref_dAngle);

            if (objLineTop.ref_dAngle > 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[0].X) - 1, (int)Math.Floor(fMinY) - 1, (int)Math.Ceiling(arrPoints[1].X - arrPoints[0].X) + 2, (int)Math.Ceiling(arrPoints[0].Y - fMinY) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[0].X) - 1, (int)Math.Floor(fMinY) - 1, (int)Math.Ceiling(arrPoints[1].X - arrPoints[0].X) + 2, (int)Math.Ceiling(arrPoints[1].Y - fMinY) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROITop.bmp");
            }
            float fWidth = 0, fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBlackImage);

            objBWROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            objDontCareROI.AttachImage(objTempImg2);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            objBWROI.CopyImage(ref objDontCareROI);
            objDontCareROI.LoadROISetting(0, (int)Math.Round(fHeight / 2), (int)Math.Round(fWidth), 1);
            ROI.InvertOperationROI(objDontCareROI);
            objBWROI.AttachImage(objTempImg2);
            
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROITop1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgTop1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineTop.ref_dAngle > 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROITop2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgTop2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROITop1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROITop2.bmp");
                objROI.SaveImage("D:\\TS\\objROITop2.bmp");
            }
            Line objLineBottom = new Line();
            objLineBottom.CalculateStraightLine(arrPoints[2], arrPoints[3]);
            if (objLineBottom.ref_dAngle > 0)
                fAngle = 90 - (float)objLineBottom.ref_dAngle;
            else
                fAngle = (90 + (float)objLineBottom.ref_dAngle);

            if (objLineBottom.ref_dAngle > 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[2].X) - 1, (int)Math.Floor(arrPoints[3].Y) - 1, (int)Math.Ceiling(arrPoints[3].X - arrPoints[2].X) + 2, (int)Math.Ceiling(fMaxY - arrPoints[3].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[2].X) - 1, (int)Math.Floor(arrPoints[2].Y) - 1, (int)Math.Ceiling(arrPoints[3].X - arrPoints[2].X) + 2, (int)Math.Ceiling(fMaxY - arrPoints[2].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROIBottom.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBlackImage);

            objBWROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            objDontCareROI.AttachImage(objTempImg2);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            objBWROI.CopyImage(ref objDontCareROI);
            objDontCareROI.LoadROISetting(0, (int)Math.Round(fHeight / 2), (int)Math.Round(fWidth), 1);
            ROI.InvertOperationROI(objDontCareROI);
            objBWROI.AttachImage(objTempImg2);

            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIBottom1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgBottom1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineBottom.ref_dAngle > 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIBottom2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgBottom2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIBottom1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIBottom2.bmp");
                objROI.SaveImage("D:\\TS\\objROIBottom2.bmp");
            }
            Line objLineLeft = new Line();
            objLineLeft.CalculateStraightLine(arrPoints[0], arrPoints[2]);

            if (objLineLeft.ref_dAngle < 0)
                fAngle = 90 - (float)objLineLeft.ref_dAngle;
            else
                fAngle = (90 + (float)objLineLeft.ref_dAngle);

            if (objLineLeft.ref_dAngle < 0)
                objROI.LoadROISetting((int)Math.Floor(fMinX) - 1, (int)Math.Floor(arrPoints[0].Y) - 1, (int)Math.Ceiling(arrPoints[2].X - fMinX) + 2, (int)Math.Ceiling(arrPoints[2].Y - arrPoints[0].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(fMinX) - 1, (int)Math.Floor(arrPoints[0].Y) - 1, (int)Math.Ceiling(arrPoints[0].X - fMinX) + 2, (int)Math.Ceiling(arrPoints[2].Y - arrPoints[0].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROILeft.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBlackImage);

            objBWROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            objDontCareROI.AttachImage(objTempImg2);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            objBWROI.CopyImage(ref objDontCareROI);
            objDontCareROI.LoadROISetting((int)Math.Round(fWidth / 2), 0, 1, (int)Math.Round(fHeight));
            ROI.InvertOperationROI(objDontCareROI);
            objBWROI.AttachImage(objTempImg2);

            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROILeft1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgLeft1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineLeft.ref_dAngle < 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROILeft2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgLeft2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROILeft1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROILeft2.bmp");
                objROI.SaveImage("D:\\TS\\objROILeft2.bmp");
            }
            Line objLineRight = new Line();
            objLineRight.CalculateStraightLine(arrPoints[1], arrPoints[3]);

            if (objLineRight.ref_dAngle < 0)
                fAngle = 90 - (float)objLineRight.ref_dAngle;
            else
                fAngle = (90 + (float)objLineRight.ref_dAngle);

            if (objLineRight.ref_dAngle < 0)
                objROI.LoadROISetting((int)Math.Floor(arrPoints[1].X) - 1, (int)Math.Floor(arrPoints[1].Y) - 1, (int)Math.Ceiling(fMaxX - arrPoints[1].X) + 2, (int)Math.Ceiling(arrPoints[3].Y - arrPoints[1].Y) + 2);
            else
                objROI.LoadROISetting((int)Math.Floor(arrPoints[3].X) - 1, (int)Math.Floor(arrPoints[1].Y) - 1, (int)Math.Ceiling(fMaxX - arrPoints[3].X) + 2, (int)Math.Ceiling(arrPoints[3].Y - arrPoints[1].Y) + 2);

            //objROI.AttachImage(objDestinationImage);
            if (blnDebugImage)
            {
                objROI.SaveImage("D:\\TS\\objROIRight.bmp");
            }
            fWidth = 0; fHeight = 0;
            if (fAngle < 90)
            {
                fWidth = (int)Math.Round((objROI.ref_ROIWidth * Math.Cos(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((objROI.ref_ROIWidth * Math.Sin(fAngle * Math.PI / 180)) + (objROI.ref_ROIHeight * Math.Cos(fAngle * Math.PI / 180)));
            }
            else if (fAngle == 90)
            {
                fWidth = objROI.ref_ROIWidth;
                fHeight = objROI.ref_ROIHeight;
            }
            else if (fAngle > 90)
            {
                float fWidth_1 = objROI.ref_ROIWidth;
                float fHeight_1 = objROI.ref_ROIHeight;
                fAngle = fAngle - 90;
                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle * Math.PI / 180)));
                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle * Math.PI / 180)));
            }

            objBWROI.AttachImage(objBlackImage);

            objBWROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            objDontCareROI.AttachImage(objTempImg2);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));
            objBWROI.CopyImage(ref objDontCareROI);
            objDontCareROI.LoadROISetting((int)Math.Round(fWidth / 2), 0, 1, (int)Math.Round(fHeight));
            ROI.InvertOperationROI(objDontCareROI);
            objBWROI.AttachImage(objTempImg2);

            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIRight1.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgRight1.bmp");
            }
            objDontCareROI.AttachImage(objTempImg);
            objDontCareROI.LoadROISetting(0, 0, (int)Math.Round(fWidth), (int)Math.Round(fHeight));

            if (objLineRight.ref_dAngle < 0)
                ROI.Rotate0Degree_ForDontCare(objBWROI, fAngle, 0, objDontCareROI);
            else
                ROI.Rotate0Degree_ForDontCare(objBWROI, -fAngle, 0, objDontCareROI);
            if (blnDebugImage)
            {
                objBWROI.SaveImage("D:\\TS\\objBWROIRight2.bmp");
                objTempImg.SaveImage("D:\\TS\\objTempImgRight2.bmp");
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIRight1.bmp");
            }
            objDontCareROI.LoadROISetting((int)Math.Round((fWidth - objROI.ref_ROIWidth) / 2), (int)Math.Round((fHeight - objROI.ref_ROIHeight) / 2), objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            if (blnWhiteDontCare)
                ROI.LogicOperationAddROI(objROI, objDontCareROI);
            else
                ROI.SubtractROI(objROI, objDontCareROI);
            if (blnDebugImage)
            {
                objDontCareROI.SaveImage("D:\\TS\\objDontCareROIRight2.bmp");
                objROI.SaveImage("D:\\TS\\objROIRight2.bmp");
                objDestinationImage.SaveImage("D:\\TS\\objDestinationImage.bmp");
                
            }
            objBWROI.Dispose();
            objTempImg.Dispose();
            objTempImg2.Dispose();
            objROI.Dispose();
            objDontCareROI.Dispose();
        }
    }
}
