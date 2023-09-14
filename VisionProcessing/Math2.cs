using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Common;

namespace VisionProcessing
{
    public class Math2
    {
        public enum Sorting { Increase = 0, Decrease = 1 };
        public enum SortItem { FileName = 0, DateCreated = 1 };
        public enum SortingDirection { X = 0, Y = 1 };


        public static float GetDistanceBtw2Points(PointF p1, PointF p2)
        {
            return (float)Math.Pow(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2), 0.5);
        }

        public static void SortData(int[] arrData, Sorting sortMethod, ref List<int> intSortData)
        {
            if (sortMethod == Sorting.Increase)
            {
                for (int i = 0; i < arrData.Length; i++)
                {
                    int intIndex = intSortData.Count;
                    for (int j = 0; j < intSortData.Count; j++)
                    {
                        if (intSortData[j] > arrData[i])
                        {
                            intIndex = j;
                            break;
                        }
                    }
                    intSortData.Insert(intIndex, arrData[i]);
                }
            }
            else
            {
                for (int i = 0; i < arrData.Length; i++)
                {
                    int intIndex = intSortData.Count;
                    for (int j = 0; j < intSortData.Count; j++)
                    {
                        if (intSortData[j] < arrData[i])
                        {
                            intIndex = j;
                            break;
                        }
                    }
                    intSortData.Insert(intIndex, arrData[i]);
                }
            }
        }
        public static void SortData(List<List<int>> arrData, int intSortIndex, Sorting sortMethod, ref List<List<int>> arrSortData)
        {
            if (sortMethod == Sorting.Increase)
            {
                for (int i = 0; i < arrData.Count; i++)
                {
                    int intIndex = arrSortData.Count;
                    for (int j = 0; j < arrSortData.Count; j++)
                    {
                        if (arrSortData[j][intSortIndex] > arrData[i][intSortIndex])
                        {
                            intIndex = j;
                            break;
                        }
                    }
                    arrSortData.Insert(intIndex, arrData[i]);
                }
            }
            else
            {
                for (int i = 0; i < arrData.Count; i++)
                {
                    int intIndex = arrSortData.Count;
                    for (int j = 0; j < arrSortData.Count; j++)
                    {
                        if (arrSortData[j][intSortIndex] < arrData[i][intSortIndex])
                        {
                            intIndex = j;
                            break;
                        }
                    }
                    arrSortData.Insert(intIndex, arrData[i]);
                }
            }
        }
        public static void SortData(List<List<float>> arrData, int intSortIndex, Sorting sortMethod, ref List<List<float>> arrSortData)
        {
            if (sortMethod == Sorting.Increase)
            {
                for (int i = 0; i < arrData.Count; i++)
                {
                    int intIndex = arrSortData.Count;
                    for (int j = 0; j < arrSortData.Count; j++)
                    {
                        if (arrSortData[j][intSortIndex] > arrData[i][intSortIndex])
                        {
                            intIndex = j;
                            break;
                        }
                    }
                    arrSortData.Insert(intIndex, arrData[i]);
                }
            }
            else
            {
                for (int i = 0; i < arrData.Count; i++)
                {
                    int intIndex = arrSortData.Count;
                    for (int j = 0; j < arrSortData.Count; j++)
                    {
                        if (arrSortData[j][intSortIndex] < arrData[i][intSortIndex])
                        {
                            intIndex = j;
                            break;
                        }
                    }
                    arrSortData.Insert(intIndex, arrData[i]);
                }
            }
        }
        public static void SortData(List<PointF> arrData, ref List<PointF> fSortData, SortingDirection sortDirection)
        {
            for (int i = 0; i < arrData.Count; i++)
            {
                int intIndex = fSortData.Count;
                for (int j = 0; j < fSortData.Count; j++)
                {
                    if (sortDirection == SortingDirection.X)
                    {
                        if (fSortData[j].X > arrData[i].X)
                        {
                            intIndex = j;
                            break;
                        }
                    }
                    else
                    {
                        if (fSortData[j].Y > arrData[i].Y)
                        {
                            intIndex = j;
                            break;
                        }
                    }
                }
                fSortData.Insert(intIndex, arrData[i]);
            }
        }
        public static void SortData(float[] arrData, Sorting sortMethod, ref List<float> fSortData)
        {
            if (sortMethod == Sorting.Increase)
            {
                for (int i = 0; i < arrData.Length; i++)
                {
                    int intIndex = fSortData.Count;
                    for (int j = 0; j < fSortData.Count; j++)
                    {
                        if (fSortData[j] > arrData[i])
                        {
                            intIndex = j;
                            break;
                        }
                    }
                    fSortData.Insert(intIndex, arrData[i]);
                }
            }
            else
            {
                for (int i = 0; i < arrData.Length; i++)
                {
                    int intIndex = fSortData.Count;
                    for (int j = 0; j < fSortData.Count; j++)
                    {
                        if (fSortData[j] < arrData[i])
                        {
                            intIndex = j;
                            break;
                        }
                    }
                    fSortData.Insert(intIndex, arrData[i]);
                }
            }
        }

        public static void SortData(float[] arrData, Sorting sortMethod, ref List<float> fSortData, ref List<int> intDataIndex)
        {
            if (sortMethod == Sorting.Increase)
            {
                for (int i = 0; i < arrData.Length; i++)
                {
                    int intIndex = fSortData.Count;
                    for (int j = 0; j < fSortData.Count; j++)
                    {
                        if (fSortData[j] > arrData[i])
                        {
                            intIndex = j;
                            break;
                        }
                    }
                    fSortData.Insert(intIndex, arrData[i]);
                    intDataIndex.Insert(intIndex, i);
                }
            }
            else
            {
                for (int i = 0; i < arrData.Length; i++)
                {
                    int intIndex = fSortData.Count;
                    for (int j = 0; j < fSortData.Count; j++)
                    {
                        if (fSortData[j] < arrData[i])
                        {
                            intIndex = j;
                            break;
                        }
                    }
                    fSortData.Insert(intIndex, arrData[i]);
                    intDataIndex.Insert(intIndex, i);
                }
            }
        }

        public static void SortData(string[] arrData, Sorting sortMethod, SortItem sortItem, ref List<string> arrSortData)
        {
        }

        public static float Min(float[] arrData, bool[] arrUsedData)
        {
            if (arrData.Length == 0)
                return 0;

            float fMin = float.MaxValue;
            for (int i = 0; i < arrData.Length; i++)
            {
                if (!arrUsedData[i])
                    continue;

                if (fMin > arrData[i])
                    fMin = arrData[i];
            }

            return fMin;
        }

        public static float Max(float[] arrData, bool[] arrUsedData)
        {
            if (arrData.Length == 0)
                return 0;

            float fMax = float.MinValue;
            for (int i = 0; i < arrData.Length; i++)
            {
                if (!arrUsedData[i])
                    continue;

                if (fMax < arrData[i])
                    fMax = arrData[i];
            }

            return fMax;
        }

        public static void MinMax(float[] arrData, bool[] arrUsedData, ref float fMin, ref float fMax)
        {
            if (arrData.Length == 0)
                return;

            fMin = float.MaxValue;
            fMax = float.MinValue;
            for (int i = 0; i < arrData.Length; i++)
            {
                if (!arrUsedData[i])
                    continue;

                if (fMin > arrData[i])
                    fMin = arrData[i];

                if (fMax < arrData[i])
                    fMax = arrData[i];
            }
        }

        public static float Range(float[] arrData)
        {
            if (arrData.Length == 0)
                return 0;

            float fMin = float.MaxValue;
            float fMax = float.MinValue;
            for (int i = 0; i < arrData.Length; i++)
            {
                if (fMin > arrData[i])
                    fMin = arrData[i];

                if (fMax < arrData[i])
                    fMax = arrData[i];
            }

            return fMax - fMin;
        }

        /// <summary>
        /// Get non 90deg triangle's angle (in degree)
        /// </summary>
        public static float GetAngle(float a, float b, float c)
        {
            /* ===== Formula [The Law of Cosines] ===========================
             * 
             *                       a^2 + b^2 - c^2 
             *  Angle (deg) = Acos (-----------------) * 180 / PI 
             *                            2ab              
             * 
             */

            return (float)(Math.Acos((Math.Pow(a, 2) + Math.Pow(b, 2) - Math.Pow(c, 2)) / (2 * a * b)) * 180 / Math.PI);
        }
        public static float GetAngle(PointF p1, PointF p2)
        {
            /* 
             * Formula: y = A + Bx where A = y-intercept and B= slope
             * Tangent value = slope
             */

            // Get slope
            double dSlope = (double)((p1.Y - p2.Y) / (p1.X - p2.X));

            // Get angle where 0 degree start from straight line x
            double dAngle = Math.Atan(dSlope) * 180 / Math.PI;

            // Turn angle so that 0 degree start from straight line y
            if (double.IsInfinity(dSlope) || double.IsNaN(dSlope))
            {
                dAngle = 0;
            }
            //if (double.IsInfinity(dAngle) || double.IsNaN(dAngle))
            //    dAngle = 0;

            return (float)dAngle;
        }
        /// <summary>
        /// Return angle after 2 linear lines cross each other (return the small angle)
        /// </summary>
        /// <param name="L1"></param>
        /// <param name="L2"></param>
        /// <returns></returns>\
        public static float GetAngle(Line L1, Line L2)
        {
            PointF p = Line.GetCrossPoint(L1, L2);

            PointF pInterceptL1 = new PointF(0, (float)L1.ref_dInterceptY);
            PointF pInterceptL2 = new PointF(0, (float)L2.ref_dInterceptY);

            float a = GetDistanceBtw2Points(p, pInterceptL1);
            float b = GetDistanceBtw2Points(p, pInterceptL2);
            float c = GetDistanceBtw2Points(pInterceptL1, pInterceptL2);

            return GetAngle(a, b, c);
        }

        /// <summary>
        /// Turn point with 90, 180, -90 or 270 degree
        /// </summary>
        /// <param name="pReferenceTurnPoint">The circle center point for point to turn</param>
        /// <param name="pTurnPoint">Turning Point</param>
        /// <param name="intTurnDegree">Turning degree (90, 180 -90, 270 only)</param>
        /// <returns></returns>
        public static PointF Turn90Point(PointF pReferenceTurnPoint, PointF pTurnPoint, int intTurnDegree)
        {
            // Get x and y distance between reference turn point and turn point
            float fXDistance = pTurnPoint.X - pReferenceTurnPoint.X;
            float fYDistance = pTurnPoint.Y - pReferenceTurnPoint.Y;

            switch (intTurnDegree)
            {
                case 90:
                    return new PointF(pReferenceTurnPoint.X - fYDistance, pReferenceTurnPoint.Y + fXDistance);
                case 180:
                    return new PointF(pReferenceTurnPoint.X - fXDistance, pReferenceTurnPoint.Y - fYDistance);
                case -90:
                case 270:
                    return new PointF(pReferenceTurnPoint.X + fYDistance, pReferenceTurnPoint.Y - fXDistance);
                default:
                    return pTurnPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objROI"></param>
        /// <param name="fCenterX">Template blob Limit Center X</param>
        /// <param name="fCenterY">Template blob Limit Center Y </param>
        /// <param name="fWidth">Sample blob width</param>
        /// <param name="fHeight">Sample blob height</param>
        /// <param name="intThresholdValue"></param>
        public static void UpdateBlobSizeAndCenterPointWithSubPixel(ROI objROI,
                                                                    float fOriCenterX, float fOriCenterY,
                                                                    ref float fWidth, ref float fHeight,
                                                                    ref float fNewCenterX, ref float fNewCenterY,
                                                                    int intThresholdValue)
        {
            /*
             * Using formula threshold as focus point
             * Update the blobs center point based on new sub pixel width and height
             */

            bool blnWrite = false;

            // Copy Ori Center Point to New Center Point
            fNewCenterX = fOriCenterX;
            fNewCenterY = fOriCenterY;

            // Get Blobs Start X and Y
            int intCheckPixelStartX = (int)Math.Floor(fOriCenterX - fWidth / 2);
            int intCheckPixelStartY = (int)Math.Floor(fOriCenterY - fHeight / 2);
            int intCheckPixelEndX = (int)Math.Ceiling(fOriCenterX + fWidth / 2);
            int intCheckPixelEndY = (int)Math.Ceiling(fOriCenterY + fHeight / 2);

            // Get a fix threshold value if it is AutoThreshold
            if (intThresholdValue == -4)
                intThresholdValue = ROI.GetAutoThresholdValue(objROI, 3);

            // ----- Check Top Pad -----
            float fValue;
            int intGrayValue, intGrayValue2;
            float fIn, fOut;
            int intInLineBiggestValue = -1;
            int intInLineBiggestValue1 = -1;
            int intInLineBiggestValue2 = -1;
            int intOutLineBiggestValue = -1;
            int intCheckInLinePos = -1;
            int intCheckOutLinePos = -1;

            if (blnWrite)
            {
                for (int m = intCheckPixelStartX; m < intCheckPixelEndX; m++)
                {
                    int intGrayValue1 = objROI.ref_ROI.GetPixel(m, intCheckPixelStartY - 1).Value;
                    int intGrayValue22 = objROI.ref_ROI.GetPixel(m, intCheckPixelStartY).Value;
                    int intGrayValue3 = objROI.ref_ROI.GetPixel(m, intCheckPixelStartY + 1).Value;
                    int intGrayValue4 = objROI.ref_ROI.GetPixel(m, intCheckPixelStartY + 2).Value;

                    //objTL2.WriteLine_NoDate(m.ToString() + "= " + intGrayValue1.ToString() + ", " + intGrayValue22.ToString() + ", " + intGrayValue3.ToString() + ", " + intGrayValue4.ToString());
                }
            }

            // Get Check Position for calculation
            for (int x = intCheckPixelStartX; x < intCheckPixelEndX; x++)
            {
                intGrayValue = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY + 1).Value;
                if (intGrayValue >= intThresholdValue)
                {
                    if (intGrayValue > intInLineBiggestValue || intInLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY + 2).Value;
                        fOut = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intInLineBiggestValue = intGrayValue;
                            intCheckInLinePos = x;
                            intInLineBiggestValue1 = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY).Value;
                            intInLineBiggestValue2 = -1;
                        }
                    }
                    else if (intGrayValue == intInLineBiggestValue)
                    {
                        intGrayValue2 = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY).Value;

                        if (intGrayValue2 > intInLineBiggestValue1)
                        {
                            if (intGrayValue2 > intInLineBiggestValue2 || intInLineBiggestValue2 == -1)
                            {
                                intInLineBiggestValue2 = intGrayValue2;
                                intCheckInLinePos = x;
                            }
                        }
                    }
                }
            }

            if (intInLineBiggestValue == -1)
            {
                for (int x = intCheckPixelStartX; x < intCheckPixelEndX; x++)
                {
                    intGrayValue = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY).Value;
                    if (intGrayValue > intOutLineBiggestValue || intOutLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY + 1).Value;
                        fOut = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY - 1).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut)) // check is value In > GrayValue > Out sequence
                        {
                            intOutLineBiggestValue = intGrayValue;
                            intCheckOutLinePos = x;
                        }
                    }
                }
            }

            // Calculate subPixel value

            //if ((intInLineBiggestValue != 255) && (intInLineBiggestValue != -1))

            if (intCheckInLinePos != intCheckOutLinePos)
            {

            }
            if ((intInLineBiggestValue != -1))
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckInLinePos, intCheckPixelStartY + 2).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckInLinePos, intCheckPixelStartY).Value;
                if ((fIn - fOut) > 0)
                {
                    if (intInLineBiggestValue2 != -1)
                        fValue = GetSubPixel(intThresholdValue, fOut, (float)intInLineBiggestValue2, fIn);
                    else
                        fValue = GetSubPixel(intThresholdValue, fOut, (float)intInLineBiggestValue, fIn);
                    fHeight += fValue;
                    fNewCenterY = GetNewCenterPointWhenSizeChange(0, fValue, fNewCenterY);
                }
            }
            else if (intOutLineBiggestValue != -1)
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckOutLinePos, intCheckPixelStartY + 1).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckOutLinePos, intCheckPixelStartY - 1).Value;
                if ((fIn - fOut) > 0)
                {
                    fValue = GetSubPixel(intThresholdValue, fOut, (float)intOutLineBiggestValue, fIn);
                    fHeight += fValue;
                    fNewCenterY = GetNewCenterPointWhenSizeChange(0, fValue, fNewCenterY);
                }
            }

            // ----- Check Bottom Pad -----
            intInLineBiggestValue = -1;
            intInLineBiggestValue1 = -1;
            intInLineBiggestValue2 = -1;
            intOutLineBiggestValue = -1;
            intCheckInLinePos = -1;
            intCheckOutLinePos = -1;

            if (blnWrite)
            {
                //objTL2.WriteLine_NoDate("\n\n=======================================================================================\n\n");
                for (int m = intCheckPixelStartX; m < intCheckPixelEndX; m++)
                {
                    int intGrayValue1 = objROI.ref_ROI.GetPixel(m, intCheckPixelEndY - 2).Value;
                    int intGrayValue22 = objROI.ref_ROI.GetPixel(m, intCheckPixelEndY - 1).Value;
                    int intGrayValue3 = objROI.ref_ROI.GetPixel(m, intCheckPixelEndY).Value;
                    int intGrayValue4 = objROI.ref_ROI.GetPixel(m, intCheckPixelEndY + 1).Value;

                    //objTL2.WriteLine_NoDate(m.ToString() + "= " + intGrayValue1.ToString() + ", " + intGrayValue22.ToString() + ", " + intGrayValue3.ToString() + ", " + intGrayValue4.ToString());
                }
            }

            // Get X for calculation
            for (int x = intCheckPixelStartX; x < intCheckPixelEndX; x++)
            {
                intGrayValue = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY - 1).Value;
                if (intGrayValue >= intThresholdValue)
                {
                    if (intGrayValue > intInLineBiggestValue || intInLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY - 2).Value;
                        fOut = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intInLineBiggestValue = intGrayValue;
                            intCheckInLinePos = x;
                            intInLineBiggestValue1 = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY).Value;
                            intInLineBiggestValue2 = -1;
                        }
                    }
                    else if (intGrayValue == intInLineBiggestValue)
                    {
                        intGrayValue2 = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY).Value;

                        if (intGrayValue2 > intInLineBiggestValue1)
                        {
                            if (intGrayValue2 > intInLineBiggestValue2 || intInLineBiggestValue2 == -1)
                            {
                                intInLineBiggestValue2 = intGrayValue2;
                                intCheckInLinePos = x;
                            }
                        }
                    }
                }
            }

            if (intInLineBiggestValue == -1)
            {
                for (int x = intCheckPixelStartX; x < intCheckPixelEndX; x++)
                {
                    intGrayValue = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY).Value;
                    if (intGrayValue > intOutLineBiggestValue || intOutLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY - 1).Value;
                        fOut = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY + 1).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intOutLineBiggestValue = intGrayValue;
                            intCheckOutLinePos = x;
                        }
                    }
                }
            }

            // Calculate subPixel value
            //if ((intInLineBiggestValue != 255) && (intInLineBiggestValue != -1))
            if ((intInLineBiggestValue != -1))
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckInLinePos, intCheckPixelEndY - 2).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckInLinePos, intCheckPixelEndY).Value;
                if ((fIn - fOut) > 0)
                {
                    if (intInLineBiggestValue2 != -1)
                        fValue = GetSubPixel(intThresholdValue, fOut, (float)intInLineBiggestValue2, fIn);
                    else
                        fValue = GetSubPixel(intThresholdValue, fOut, (float)intInLineBiggestValue, fIn);
                    fHeight += fValue;
                    fNewCenterY = GetNewCenterPointWhenSizeChange(2, fValue, fNewCenterY);
                }
            }
            else if (intOutLineBiggestValue != -1)
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckOutLinePos, intCheckPixelEndY - 1).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckOutLinePos, intCheckPixelEndY + 1).Value;
                if ((fIn - fOut) > 0)
                {
                    fValue = GetSubPixel(intThresholdValue, fOut, (float)intOutLineBiggestValue, fIn);
                    fHeight += fValue;
                    fNewCenterY = GetNewCenterPointWhenSizeChange(2, fValue, fNewCenterY);
                }
            }

            // ----- Check Left Pad -----
            intInLineBiggestValue = -1;
            intInLineBiggestValue1 = -1;
            intInLineBiggestValue2 = -1;
            intOutLineBiggestValue = -1;
            intCheckInLinePos = -1;
            intCheckOutLinePos = -1;

            if (blnWrite)
            {
                //objTL2.WriteLine_NoDate("\n\n=======================================================================================\n\n");
                for (int m = intCheckPixelStartY; m < intCheckPixelEndY; m++)
                {
                    int intGrayValue1 = objROI.ref_ROI.GetPixel(intCheckPixelStartX - 1, m).Value;
                    int intGrayValue22 = objROI.ref_ROI.GetPixel(intCheckPixelStartX, m).Value;
                    int intGrayValue3 = objROI.ref_ROI.GetPixel(intCheckPixelStartX + 1, m).Value;
                    int intGrayValue4 = objROI.ref_ROI.GetPixel(intCheckPixelStartX + 2, m).Value;

                    //objTL2.WriteLine_NoDate(m.ToString() + "= " + intGrayValue1.ToString() + ", " + intGrayValue22.ToString() + ", " + intGrayValue3.ToString() + ", " + intGrayValue4.ToString());
                }
            }

            // Get Check Position for calculation
            for (int y = intCheckPixelStartY; y < intCheckPixelEndY; y++)
            {
                intGrayValue = objROI.ref_ROI.GetPixel(intCheckPixelStartX + 1, y).Value;
                if (intGrayValue >= intThresholdValue)
                {
                    if (intGrayValue > intInLineBiggestValue || intInLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel((intCheckPixelStartX + 2), y).Value;
                        fOut = objROI.ref_ROI.GetPixel(intCheckPixelStartX, y).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intInLineBiggestValue = intGrayValue;
                            intCheckInLinePos = y;
                            intInLineBiggestValue1 = objROI.ref_ROI.GetPixel(intCheckPixelStartX, y).Value;
                            intInLineBiggestValue2 = -1;
                        }
                    }
                    else if (intGrayValue == intInLineBiggestValue)
                    {
                        intGrayValue2 = objROI.ref_ROI.GetPixel(intCheckPixelStartX, y).Value;

                        if (intGrayValue2 > intInLineBiggestValue1)
                        {
                            if (intGrayValue2 > intInLineBiggestValue2 || intInLineBiggestValue2 == -1)
                            {
                                intInLineBiggestValue2 = intGrayValue2;
                                intCheckInLinePos = y;
                            }
                        }
                    }
                }
            }

            if (intInLineBiggestValue == -1)
            {
                for (int y = intCheckPixelStartY; y < intCheckPixelEndY; y++)
                {
                    intGrayValue = objROI.ref_ROI.GetPixel(intCheckPixelStartX, y).Value;
                    if (intGrayValue > intOutLineBiggestValue || intOutLineBiggestValue == -1)
                    {

                        fIn = objROI.ref_ROI.GetPixel((intCheckPixelStartX + 1), y).Value;
                        fOut = objROI.ref_ROI.GetPixel((intCheckPixelStartX - 1), y).Value;
                        // Check is selected gray value btw the in and out gray value?
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intOutLineBiggestValue = intGrayValue;
                            intCheckOutLinePos = y;
                        }
                    }
                }
            }

            // Calculate subPixel value
            //if ((intInLineBiggestValue != 255) && (intInLineBiggestValue != -1))
            if ((intInLineBiggestValue != -1))
            {
                fIn = objROI.ref_ROI.GetPixel((intCheckPixelStartX + 2), intCheckInLinePos).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckPixelStartX, intCheckInLinePos).Value;
                if ((fIn - fOut) > 0)
                {
                    if (intInLineBiggestValue2 != -1)
                        fValue = GetSubPixel(intThresholdValue, fOut, (float)intInLineBiggestValue2, fIn);
                    else
                        fValue = GetSubPixel(intThresholdValue, fOut, (float)intInLineBiggestValue, fIn);
                    fWidth += fValue;
                    fNewCenterX = GetNewCenterPointWhenSizeChange(3, fValue, fNewCenterX);
                }
            }
            else if (intOutLineBiggestValue != -1)
            {
                fIn = objROI.ref_ROI.GetPixel((intCheckPixelStartX + 1), intCheckOutLinePos).Value;
                fOut = objROI.ref_ROI.GetPixel((intCheckPixelStartX - 1), intCheckOutLinePos).Value;
                if ((fIn - fOut) > 0)
                {
                    fValue = GetSubPixel(intThresholdValue, fOut, (float)intOutLineBiggestValue, fIn);


                    fWidth += fValue;
                    fNewCenterX = GetNewCenterPointWhenSizeChange(3, fValue, fNewCenterX);
                }
            }

            // ----- Check Right Pad -----
            intInLineBiggestValue = -1;
            intInLineBiggestValue1 = -1;
            intInLineBiggestValue2 = -1;
            intOutLineBiggestValue = -1;
            intCheckInLinePos = -1;
            intCheckOutLinePos = -1;

            if (blnWrite)
            {
                //objTL2.WriteLine_NoDate("\n\n=======================================================================================\n\n");
                for (int m = intCheckPixelStartY; m < intCheckPixelEndY; m++)
                {
                    int intGrayValue1 = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 2, m).Value;
                    int intGrayValue22 = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 1, m).Value;
                    int intGrayValue3 = objROI.ref_ROI.GetPixel(intCheckPixelEndX, m).Value;
                    int intGrayValue4 = objROI.ref_ROI.GetPixel(intCheckPixelEndX + 1, m).Value;

                    //objTL2.WriteLine_NoDate(m.ToString() + "= " + intGrayValue1.ToString() + ", " + intGrayValue22.ToString() + ", " + intGrayValue3.ToString() + ", " + intGrayValue4.ToString());
                }
            }

            // Get Check Position for calculation
            for (int y = intCheckPixelStartY; y < intCheckPixelEndY; y++)
            {
                intGrayValue = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 1, y).Value;
                if (intGrayValue >= intThresholdValue)
                {
                    if (intGrayValue > intInLineBiggestValue || intInLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 2, y).Value;
                        fOut = objROI.ref_ROI.GetPixel(intCheckPixelEndX, y).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intInLineBiggestValue = intGrayValue;
                            intCheckInLinePos = y;
                            intInLineBiggestValue1 = objROI.ref_ROI.GetPixel(intCheckPixelEndX, y).Value;
                            intInLineBiggestValue2 = -1;
                        }
                    }
                    else if (intGrayValue == intInLineBiggestValue)
                    {
                        intGrayValue2 = objROI.ref_ROI.GetPixel(intCheckPixelEndX, y).Value;

                        if (intGrayValue2 > intInLineBiggestValue1)
                        {
                            if (intGrayValue2 > intInLineBiggestValue2 || intInLineBiggestValue2 == -1)
                            {
                                intInLineBiggestValue2 = intGrayValue2;
                                intCheckInLinePos = y;
                            }
                        }
                    }
                }
            }

            if (intInLineBiggestValue == -1)
            {
                for (int y = intCheckPixelStartY; y < intCheckPixelEndY; y++)
                {
                    intGrayValue = objROI.ref_ROI.GetPixel(intCheckPixelEndX, y).Value;
                    if (intGrayValue > intOutLineBiggestValue || intOutLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 1, y).Value;
                        fOut = objROI.ref_ROI.GetPixel(intCheckPixelEndX + 1, y).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intOutLineBiggestValue = intGrayValue;
                            intCheckOutLinePos = y;
                        }
                    }
                }
            }

            // Calculate subPixel value
            //if ((intInLineBiggestValue != 255) && (intInLineBiggestValue != -1))
            if ((intInLineBiggestValue != -1))
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 2, intCheckInLinePos).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckPixelEndX, intCheckInLinePos).Value;
                if ((fIn - fOut) > 0)
                {
                    if (intInLineBiggestValue2 != -1)
                        fValue = GetSubPixel(intThresholdValue, fOut, (float)intInLineBiggestValue2, fIn);
                    else
                        fValue = GetSubPixel(intThresholdValue, fOut, (float)intInLineBiggestValue, fIn);
                    fWidth += fValue;
                    fNewCenterX = GetNewCenterPointWhenSizeChange(1, fValue, fNewCenterX);
                }
            }
            else if (intOutLineBiggestValue != -1)
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 1, intCheckOutLinePos).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckPixelEndX + 1, intCheckOutLinePos).Value;
                if ((fIn - fOut) > 0)
                {
                    fValue = GetSubPixel(intThresholdValue, fOut, (float)intOutLineBiggestValue, fIn);
                    fWidth += fValue;
                    fNewCenterX = GetNewCenterPointWhenSizeChange(1, fValue, fNewCenterX);
                }
            }

        }

        public static void UpdateBlobSizeAndCenterPointWithSubPixel(ROI objROI,
                                                                   Point pTop, Point pBottom, Point pLeft, Point pRight,
                                                                   float fOriCenterX, float fOriCenterY,
                                                                   ref float fWidth, ref float fHeight,
                                                                   ref float fNewCenterX, ref float fNewCenterY,
                                                                   int intThresholdValue)
        {
            /*
             * Using formula threshold as focus point
             * Update the blobs center point based on new sub pixel width and height
             */

            // Copy Ori Center Point to New Center Point
            fNewCenterX = fOriCenterX;
            fNewCenterY = fOriCenterY;

            // Get a fix threshold value if it is AutoThreshold
            if (intThresholdValue == -4)
                intThresholdValue = ROI.GetAutoThresholdValue(objROI, 3);

            // ----- Check Top Pad -----
            float fValue;

            // Top
            int intOut2Value = objROI.ref_ROI.GetPixel(pTop.X, pTop.Y - 2).Value;
            int intOut1Value = objROI.ref_ROI.GetPixel(pTop.X, pTop.Y - 1).Value;
            int intValue = objROI.ref_ROI.GetPixel(pTop.X, pTop.Y).Value;
            int intIn1Value = objROI.ref_ROI.GetPixel(pTop.X, pTop.Y + 1).Value;
            fValue = GetSubPixel(intThresholdValue, intOut1Value, intValue, intIn1Value);
            fHeight += fValue;
            fNewCenterY = GetNewCenterPointWhenSizeChange(0, fValue, fNewCenterY);

            //////fValue = GetSubPixel(155, 150, 255, 255);
            //////fValue = GetSubPixel(155, 80, 160, 255);
            //////fValue = GetSubPixel(150, 119, 165, 255);
            //////fWidth += fValue;
            //////fNewCenterX = GetNewCenterPointWhenSizeChange(0, fValue, fNewCenterX);


            // Right
            intOut2Value = objROI.ref_ROI.GetPixel(pRight.X + 2, pRight.Y).Value;
            intOut1Value = objROI.ref_ROI.GetPixel(pRight.X + 1, pRight.Y).Value;
            intValue = objROI.ref_ROI.GetPixel(pRight.X, pRight.Y).Value;
            intIn1Value = objROI.ref_ROI.GetPixel(pRight.X - 1, pRight.Y).Value;
            fValue = GetSubPixel(intThresholdValue, intOut1Value, intValue, intIn1Value);
            fWidth += fValue;
            fNewCenterX = GetNewCenterPointWhenSizeChange(0, fValue, fNewCenterX);

            // Bottom
            intOut2Value = objROI.ref_ROI.GetPixel(pBottom.X, pBottom.Y + 2).Value;
            intOut1Value = objROI.ref_ROI.GetPixel(pBottom.X, pBottom.Y + 1).Value;
            intValue = objROI.ref_ROI.GetPixel(pBottom.X, pBottom.Y).Value;
            intIn1Value = objROI.ref_ROI.GetPixel(pBottom.X, pBottom.Y - 1).Value;
            fValue = GetSubPixel(intThresholdValue, intOut1Value, intValue, intIn1Value);
            fHeight += fValue;
            fNewCenterY = GetNewCenterPointWhenSizeChange(0, fValue, fNewCenterY);

            // Right
            intOut2Value = objROI.ref_ROI.GetPixel(pLeft.X - 2, pLeft.Y).Value;
            intOut1Value = objROI.ref_ROI.GetPixel(pLeft.X - 1, pLeft.Y).Value;
            intValue = objROI.ref_ROI.GetPixel(pLeft.X, pLeft.Y).Value;
            intIn1Value = objROI.ref_ROI.GetPixel(pLeft.X + 1, pLeft.Y).Value;
            fValue = GetSubPixel(intThresholdValue, intOut1Value, intValue, intIn1Value);
            fWidth += fValue;
            fNewCenterX = GetNewCenterPointWhenSizeChange(0, fValue, fNewCenterX);
        }

        public static void UpdateBlobSizeAndCenterPointWithSubPixel(ROI objROI,
                                                                   float fOriCenterX, float fOriCenterY,
                                                                   ref float fWidth, ref float fHeight,
                                                                   ref float fNewCenterX, ref float fNewCenterY,
                                                                   int intThresholdValue, bool blnHorizontal,
                                                                   bool blnScanFromBottomToTop, int intFixPointXY)
        {
            /*
             * Using formula threshold as focus point
             * Update the blobs center point based on new sub pixel width and height
             */

            // Copy Ori Center Point to New Center Point
            fNewCenterX = fOriCenterX;
            fNewCenterY = fOriCenterY;

            if (blnHorizontal)
            {
                // Get Blobs Start X
                int intCheckPixelStartX = (int)Math.Floor(fOriCenterX - fWidth / 2);
                int intCheckPixelEndX = (int)Math.Ceiling(fOriCenterX + fWidth / 2);

                // Get a fix threshold value if it is AutoThreshold
                if (intThresholdValue == -4)
                    intThresholdValue = ROI.GetAutoThresholdValue(objROI, 3);

                float fStartPoint, fEndPoint;
                int intGrayValue;
                int intCheckStartPos = -1;
                int intCheckEndPos = -1;
                bool blnFound = false;

                //Break once found value higher than threshold
                if (blnScanFromBottomToTop)
                {
                    for (int i = intFixPointXY; i > fOriCenterY; i--)
                    {
                        for (int x = intCheckPixelStartX; x < intCheckPixelEndX; x++)
                        {
                            intGrayValue = objROI.ref_ROI.GetPixel(x, i).Value;
                            if (intGrayValue >= intThresholdValue)
                            {
                                intCheckStartPos = x;
                                intFixPointXY = i;
                                blnFound = true;
                                break;
                            }
                        }

                        if (blnFound)
                            break;
                    }
                }
                else
                {
                    for (int i = intFixPointXY; i < fOriCenterY; i++)
                    {
                        for (int x = intCheckPixelStartX; x < intCheckPixelEndX; x++)
                        {
                            intGrayValue = objROI.ref_ROI.GetPixel(x, i).Value;
                            if (intGrayValue >= intThresholdValue)
                            {
                                intCheckStartPos = x;
                                intFixPointXY = i;
                                blnFound = true;
                                break;
                            }
                        }

                        if (blnFound)
                            break;
                    }
                }

                // Left side of top or bottom lead
                int intOut1Value = objROI.ref_ROI.GetPixel(intCheckStartPos - 1, intFixPointXY).Value;
                int intValue = objROI.ref_ROI.GetPixel(intCheckStartPos, intFixPointXY).Value;
                int intIn1Value = objROI.ref_ROI.GetPixel(intCheckStartPos + 1, intFixPointXY).Value;
                fStartPoint = intCheckStartPos + GetSubPixel(intThresholdValue, intOut1Value, intValue, intIn1Value);

                //Break once found value higher than threshold
                for (int x = intCheckPixelEndX; x >= intCheckPixelStartX; x--)
                {
                    intGrayValue = objROI.ref_ROI.GetPixel(x, intFixPointXY).Value;
                    if (intGrayValue >= intThresholdValue)
                    {
                        intCheckEndPos = x;
                        break;
                    }
                }

                // Right side of top or bottom lead
                intOut1Value = objROI.ref_ROI.GetPixel(intCheckEndPos + 1, intFixPointXY).Value;
                intValue = objROI.ref_ROI.GetPixel(intCheckEndPos, intFixPointXY).Value;
                intIn1Value = objROI.ref_ROI.GetPixel(intCheckEndPos - 1, intFixPointXY).Value;
                fEndPoint = intCheckEndPos + GetSubPixel(intThresholdValue, intOut1Value, intValue, intIn1Value);

                fWidth = fEndPoint - fStartPoint;
                fNewCenterX = fStartPoint + fWidth / 2;
            }
            else
            {
                // Get Blobs Start Y
                int intCheckPixelStartY = (int)Math.Floor(fOriCenterY - fHeight / 2);
                int intCheckPixelEndY = (int)Math.Ceiling(fOriCenterY + fHeight / 2);

                // Get a fix threshold value if it is AutoThreshold
                if (intThresholdValue == -4)
                    intThresholdValue = ROI.GetAutoThresholdValue(objROI, 3);

                float fStartPoint, fEndPoint;
                int intGrayValue;
                int intCheckStartPos = -1;
                int intCheckEndPos = -1;
                bool blnFound = false;

                //Break once found value higher than threshold
                if (blnScanFromBottomToTop)
                {
                    for (int i = intFixPointXY; i > fOriCenterX; i--)
                    {
                        for (int y = intCheckPixelStartY; y < intCheckPixelEndY; y++)
                        {
                            intGrayValue = objROI.ref_ROI.GetPixel(i, y).Value;
                            if (intGrayValue >= intThresholdValue)
                            {
                                intCheckStartPos = y;
                                intFixPointXY = i;
                                blnFound = true;
                                break;
                            }
                        }

                        if (blnFound)
                            break;
                    }
                }
                else
                {
                    for (int i = intFixPointXY; i < fOriCenterX; i++)
                    {
                        for (int y = intCheckPixelStartY; y < intCheckPixelEndY; y++)
                        {
                            intGrayValue = objROI.ref_ROI.GetPixel(i, y).Value;
                            if (intGrayValue >= intThresholdValue)
                            {
                                intCheckStartPos = y;
                                intFixPointXY = i;
                                blnFound = true;
                                break;
                            }
                        }

                        if (blnFound)
                            break;
                    }
                }

                // Left side of top or bottom lead
                int intOut1Value = objROI.ref_ROI.GetPixel(intFixPointXY, intCheckStartPos - 1).Value;
                int intValue = objROI.ref_ROI.GetPixel(intFixPointXY, intCheckStartPos).Value;
                int intIn1Value = objROI.ref_ROI.GetPixel(intFixPointXY, intCheckStartPos + 1).Value;
                fStartPoint = intCheckStartPos + GetSubPixel(intThresholdValue, intOut1Value, intValue, intIn1Value);

                //Break once found value higher than threshold
                for (int y = intCheckPixelEndY; y >= intCheckPixelStartY; y--)
                {
                    intGrayValue = objROI.ref_ROI.GetPixel(intFixPointXY, y).Value;
                    if (intGrayValue >= intThresholdValue)
                    {
                        intCheckEndPos = y;
                        break;
                    }
                }

                // Right side of top or bottom lead
                intOut1Value = objROI.ref_ROI.GetPixel(intFixPointXY, intCheckEndPos + 1).Value;
                intValue = objROI.ref_ROI.GetPixel(intFixPointXY, intCheckEndPos).Value;
                intIn1Value = objROI.ref_ROI.GetPixel(intFixPointXY, intCheckEndPos - 1).Value;
                fEndPoint = intCheckEndPos + GetSubPixel(intThresholdValue, intOut1Value, intValue, intIn1Value);

                fHeight = fEndPoint - fStartPoint;
                fNewCenterY = fStartPoint + fHeight / 2;
            }
        }


        private static float GetSubPixel(int intThreshold, float fPixelLeft, float fPixelCenter, float fPixelRight)
        {
            // find 2 pixels where threshold value is in the middle
            float fLPixel, fRPixel;
            float fAddPixel;
            if (intThreshold >= fPixelLeft && intThreshold <= fPixelCenter)
            {
                fLPixel = fPixelLeft;
                fRPixel = fPixelCenter;
                fAddPixel = 1;
            }
            else if (intThreshold >= fPixelCenter && intThreshold <= fPixelRight)
            {
                fLPixel = fPixelCenter;
                fRPixel = fPixelRight;
                fAddPixel = 0;
            }
            else
            {
                if ((fPixelLeft <= fPixelCenter) && (fPixelCenter <= fPixelRight))
                    return 1;
                else
                    return 0;
            }

            // Get 2 pixels value different
            float fPixelValueGap = fRPixel - fLPixel;

            if (fPixelValueGap == 0)
                return 0;

            // Get 2 pixels distance
            float fPixelDistance = fRPixel + (255 - fLPixel);

            float fHighPixelWithThresholdGap = fRPixel - intThreshold;

            float fSubPixelValue = fHighPixelWithThresholdGap * fPixelDistance / fPixelValueGap;

            float fNewPixelLocation = fRPixel - fSubPixelValue;

            float fSubPixelUnit = -fNewPixelLocation / 255;

            return fSubPixelUnit;
        }

        private static float GetSubPixel(int intThreshold, float fPixelLeft, float fPixel2, float fPixelCenter, float fPixelRight)
        {
            // find 2 pixels where threshold value is in the middle
            float fLPixel, fRPixel;
            float fAddPixel;
            if (intThreshold >= fPixelLeft && intThreshold <= fPixelCenter)
            {
                fLPixel = fPixelLeft;
                fRPixel = fPixelCenter;
                fAddPixel = 1;
            }
            else if (intThreshold >= fPixelCenter && intThreshold <= fPixelRight)
            {
                fLPixel = fPixelCenter;
                fRPixel = fPixelRight;
                fAddPixel = 0;
            }
            else
            {
                if ((fPixelLeft <= fPixelCenter) && (fPixelCenter <= fPixelRight))
                    return 1;
                else
                    return 0;
            }

            // Get 2 pixels value different
            float fPixelValueGap = fRPixel - fLPixel;

            if (fPixelValueGap == 0)
                return 0;

            // Get 2 pixels distance
            float fPixelDistance = fRPixel + (255 - fLPixel);

            float fHighPixelWithThresholdGap = fRPixel - intThreshold;

            float fSubPixelValue = fHighPixelWithThresholdGap * fPixelDistance / fPixelValueGap;

            float fNewPixelLocation = fRPixel - fSubPixelValue;

            float fSubPixelUnit = -fNewPixelLocation / 255;

            return fSubPixelUnit;
        }


        public static void UpdateBlobSizeWithSubPixel2(ROI objROI,
           float fCenterX, float fCenterY,
           ref float fWidth, ref float fHeight,
           int intThresholdValue)
        {

            #region SubPixel Resolution

            int intCheckPixelStartX = (int)Math.Floor(fCenterX - fWidth / 2);
            int intCheckPixelStartY = (int)Math.Floor(fCenterY - fHeight / 2);
            int intCheckPixelEndX = (int)Math.Ceiling(fCenterX + fWidth / 2);
            int intCheckPixelEndY = (int)Math.Ceiling(fCenterY + fHeight / 2);

            // Get a fix threshold value if it is AutoThreshold
            if (intThresholdValue == -4)
                intThresholdValue = ROI.GetAutoThresholdValue(objROI, 3);

            // ----- Check Top Pad -----
            int intGrayValue;
            float fIn, fOut;
            int intInLineBiggestValue = -1;
            int intOutLineBiggestValue = -1;
            int intCheckInLinePos = -1;
            int intCheckOutLinePos = -1;

            // Get Check Position for calculation
            for (int x = intCheckPixelStartX; x < intCheckPixelEndX; x++)
            {
                intGrayValue = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY + 1).Value;
                if (intGrayValue >= intThresholdValue)
                {
                    if (intGrayValue > intInLineBiggestValue || intInLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY + 2).Value;
                        fOut = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intInLineBiggestValue = intGrayValue;
                            intCheckInLinePos = x;
                        }
                    }
                }

                intGrayValue = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY).Value;
                if (intGrayValue > intOutLineBiggestValue || intOutLineBiggestValue == -1)
                {
                    fIn = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY + 1).Value;
                    fOut = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY - 1).Value;
                    if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut)) // check is value In > GrayValue > Out sequence
                    {
                        intOutLineBiggestValue = intGrayValue;
                        intCheckOutLinePos = x;
                    }
                }
            }

            // Calculate subPixel value

            float fSubPixel = -1;
            //if ((intInLineBiggestValue != 255) && (intInLineBiggestValue != -1))
            if ((intInLineBiggestValue != -1))
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckInLinePos, intCheckPixelStartY + 2).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckInLinePos, intCheckPixelStartY).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intInLineBiggestValue - fOut) / (fIn - fOut);
                    fHeight = fHeight - 1 + fSubPixel;
                }
            }
            else if (intOutLineBiggestValue != -1)
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckOutLinePos, intCheckPixelStartY + 1).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckOutLinePos, intCheckPixelStartY - 1).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intOutLineBiggestValue - fOut) / (fIn - fOut);
                    fHeight = fHeight + fSubPixel;
                }
            }

            // ----- Check Bottom Pad -----
            intInLineBiggestValue = -1;
            intOutLineBiggestValue = -1;
            intCheckInLinePos = -1;
            intCheckOutLinePos = -1;

            // Get X for calculation
            for (int x = intCheckPixelStartX; x < intCheckPixelEndX; x++)
            {
                intGrayValue = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY - 1).Value;
                if (intGrayValue >= intThresholdValue)
                {
                    if (intGrayValue > intInLineBiggestValue || intInLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY - 2).Value;
                        fOut = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intInLineBiggestValue = intGrayValue;
                            intCheckInLinePos = x;
                        }
                    }
                }

                intGrayValue = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY).Value;
                if (intGrayValue > intOutLineBiggestValue || intOutLineBiggestValue == -1)
                {
                    fIn = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY - 1).Value;
                    fOut = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY + 1).Value;
                    if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                    {
                        intOutLineBiggestValue = intGrayValue;
                        intCheckOutLinePos = x;
                    }
                }
            }

            // Calculate subPixel value
            fSubPixel = -1;
            //if ((intInLineBiggestValue != 255) && (intInLineBiggestValue != -1))
            if ((intInLineBiggestValue != -1))
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckInLinePos, intCheckPixelEndY - 2).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckInLinePos, intCheckPixelEndY).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intInLineBiggestValue - fOut) / (fIn - fOut);
                    fHeight = fHeight - 1 + fSubPixel;
                }
            }
            else if (intOutLineBiggestValue != -1)
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckOutLinePos, intCheckPixelEndY - 1).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckOutLinePos, intCheckPixelEndY + 1).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intOutLineBiggestValue - fOut) / (fIn - fOut);
                    fHeight = fHeight + fSubPixel;
                }
            }

            // ----- Check Left Pad -----
            intInLineBiggestValue = -1;
            intOutLineBiggestValue = -1;
            intCheckInLinePos = -1;
            intCheckOutLinePos = -1;

            // Get Check Position for calculation
            for (int y = intCheckPixelStartY; y < intCheckPixelEndY; y++)
            {
                intGrayValue = objROI.ref_ROI.GetPixel(intCheckPixelStartX + 1, y).Value;
                if (intGrayValue >= intThresholdValue)
                {
                    if (intGrayValue > intInLineBiggestValue || intInLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel((intCheckPixelStartX + 2), y).Value;
                        fOut = objROI.ref_ROI.GetPixel(intCheckPixelStartX, y).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intInLineBiggestValue = intGrayValue;
                            intCheckInLinePos = y;
                        }
                    }
                }

                intGrayValue = objROI.ref_ROI.GetPixel(intCheckPixelStartX, y).Value;
                if (intGrayValue > intOutLineBiggestValue || intOutLineBiggestValue == -1)
                {

                    fIn = objROI.ref_ROI.GetPixel((intCheckPixelStartX + 1), y).Value;
                    fOut = objROI.ref_ROI.GetPixel((intCheckPixelStartX - 1), y).Value;
                    if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                    {
                        intOutLineBiggestValue = intGrayValue;
                        intCheckOutLinePos = y;
                    }
                }
            }

            // Calculate subPixel value
            fSubPixel = -1;
            //if ((intInLineBiggestValue != 255) && (intInLineBiggestValue != -1))
            if ((intInLineBiggestValue != -1))
            {
                fIn = objROI.ref_ROI.GetPixel((intCheckPixelStartX + 2), intCheckInLinePos).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckPixelStartX, intCheckInLinePos).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intInLineBiggestValue - fOut) / (fIn - fOut);
                    fWidth = fWidth - 1 + fSubPixel;
                }
            }
            else if (intOutLineBiggestValue != -1)
            {
                fIn = objROI.ref_ROI.GetPixel((intCheckPixelStartX + 1), intCheckOutLinePos).Value;
                fOut = objROI.ref_ROI.GetPixel((intCheckPixelStartX - 1), intCheckOutLinePos).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intOutLineBiggestValue - fOut) / (fIn - fOut);
                    fWidth = fWidth + fSubPixel;
                }
            }

            // ----- Check Right Pad -----
            intInLineBiggestValue = -1;
            intOutLineBiggestValue = -1;
            intCheckInLinePos = -1;
            intCheckOutLinePos = -1;

            // Get Check Position for calculation
            for (int y = intCheckPixelStartY; y < intCheckPixelEndY; y++)
            {
                intGrayValue = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 1, y).Value;
                if (intGrayValue >= intThresholdValue)
                {
                    if (intGrayValue > intInLineBiggestValue || intInLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 2, y).Value;
                        fOut = objROI.ref_ROI.GetPixel(intCheckPixelEndX, y).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intInLineBiggestValue = intGrayValue;
                            intCheckInLinePos = y;
                        }
                    }
                }

                intGrayValue = objROI.ref_ROI.GetPixel(intCheckPixelEndX, y).Value;
                if (intGrayValue > intOutLineBiggestValue || intOutLineBiggestValue == -1)
                {
                    fIn = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 1, y).Value;
                    fOut = objROI.ref_ROI.GetPixel(intCheckPixelEndX + 1, y).Value;
                    if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                    {
                        intOutLineBiggestValue = intGrayValue;
                        intCheckOutLinePos = y;
                    }
                }
            }

            // Calculate subPixel value
            fSubPixel = -1;
            //if ((intInLineBiggestValue != 255) && (intInLineBiggestValue != -1))
            if ((intInLineBiggestValue != -1))
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 2, intCheckInLinePos).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckPixelEndX, intCheckInLinePos).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intInLineBiggestValue - fOut) / (fIn - fOut);
                    fWidth = fWidth - 1 + fSubPixel;
                }
            }
            else if (intOutLineBiggestValue != -1)
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 1, intCheckOutLinePos).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckPixelEndX + 1, intCheckOutLinePos).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intOutLineBiggestValue - fOut) / (fIn - fOut);
                    fWidth = fWidth + fSubPixel;
                }
            }
            #endregion
        }

        public static void UpdateBlobSizeWithSubPixel(ROI objROI,
            float fCenterX, float fCenterY,
            ref float fWidth, ref float fHeight,
            int intThresholdValue)
        {

            #region SubPixel Resolution

            int intCheckPixelStartX = (int)Math.Floor(fCenterX - fWidth / 2);
            int intCheckPixelStartY = (int)Math.Floor(fCenterY - fHeight / 2);
            int intCheckPixelEndX = (int)Math.Ceiling(fCenterX + fWidth / 2);
            int intCheckPixelEndY = (int)Math.Ceiling(fCenterY + fHeight / 2);

            // Get a fix threshold value if it is AutoThreshold
            if (intThresholdValue == -4)
                intThresholdValue = ROI.GetAutoThresholdValue(objROI, 3);

            // ----- Check Top Pad -----
            int intGrayValue;
            float fIn, fOut;
            int intInLineBiggestValue = -1;
            int intOutLineBiggestValue = -1;
            int intCheckInLinePos = -1;
            int intCheckOutLinePos = -1;

            // Get Check Position for calculation
            for (int x = intCheckPixelStartX; x < intCheckPixelEndX; x++)
            {
                intGrayValue = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY + 1).Value;
                if (intGrayValue >= intThresholdValue)
                {
                    if (intGrayValue > intInLineBiggestValue || intInLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY + 2).Value;
                        fOut = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intInLineBiggestValue = intGrayValue;
                            intCheckInLinePos = x;
                        }
                    }
                }

                intGrayValue = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY).Value;
                if (intGrayValue > intOutLineBiggestValue || intOutLineBiggestValue == -1)
                {
                    fIn = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY + 1).Value;
                    fOut = objROI.ref_ROI.GetPixel(x, intCheckPixelStartY - 1).Value;
                    if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut)) // check is value In > GrayValue > Out sequence
                    {
                        intOutLineBiggestValue = intGrayValue;
                        intCheckOutLinePos = x;
                    }
                }
            }

            // Calculate subPixel value

            float fSubPixel = -1;
            if ((intInLineBiggestValue != 255) && (intInLineBiggestValue != -1))
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckInLinePos, intCheckPixelStartY + 2).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckInLinePos, intCheckPixelStartY).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intInLineBiggestValue - fOut) / (fIn - fOut);
                    fHeight = fHeight - 1 + fSubPixel;
                }
            }
            else if (intOutLineBiggestValue != -1)
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckOutLinePos, intCheckPixelStartY + 1).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckOutLinePos, intCheckPixelStartY - 1).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intOutLineBiggestValue - fOut) / (fIn - fOut);
                    fHeight = fHeight + fSubPixel;
                }
            }

            // ----- Check Bottom Pad -----
            intInLineBiggestValue = -1;
            intOutLineBiggestValue = -1;
            intCheckInLinePos = -1;
            intCheckOutLinePos = -1;

            // Get X for calculation
            for (int x = intCheckPixelStartX; x < intCheckPixelEndX; x++)
            {
                intGrayValue = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY - 1).Value;
                if (intGrayValue >= intThresholdValue)
                {
                    if (intGrayValue > intInLineBiggestValue || intInLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY - 2).Value;
                        fOut = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intInLineBiggestValue = intGrayValue;
                            intCheckInLinePos = x;
                        }
                    }
                }

                intGrayValue = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY).Value;
                if (intGrayValue > intOutLineBiggestValue || intOutLineBiggestValue == -1)
                {
                    fIn = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY - 1).Value;
                    fOut = objROI.ref_ROI.GetPixel(x, intCheckPixelEndY + 1).Value;
                    if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                    {
                        intOutLineBiggestValue = intGrayValue;
                        intCheckOutLinePos = x;
                    }
                }
            }

            // Calculate subPixel value
            fSubPixel = -1;
            if ((intInLineBiggestValue != 255) && (intInLineBiggestValue != -1))
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckInLinePos, intCheckPixelEndY - 2).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckInLinePos, intCheckPixelEndY).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intInLineBiggestValue - fOut) / (fIn - fOut);
                    fHeight = fHeight - 1 + fSubPixel;
                }
            }
            else if (intOutLineBiggestValue != -1)
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckOutLinePos, intCheckPixelEndY - 1).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckOutLinePos, intCheckPixelEndY + 1).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intOutLineBiggestValue - fOut) / (fIn - fOut);
                    fHeight = fHeight + fSubPixel;
                }
            }

            // ----- Check Left Pad -----
            intInLineBiggestValue = -1;
            intOutLineBiggestValue = -1;
            intCheckInLinePos = -1;
            intCheckOutLinePos = -1;

            // Get Check Position for calculation
            for (int y = intCheckPixelStartY; y < intCheckPixelEndY; y++)
            {
                intGrayValue = objROI.ref_ROI.GetPixel(intCheckPixelStartX + 1, y).Value;
                if (intGrayValue >= intThresholdValue)
                {
                    if (intGrayValue > intInLineBiggestValue || intInLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel((intCheckPixelStartX + 2), y).Value;
                        fOut = objROI.ref_ROI.GetPixel(intCheckPixelStartX, y).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intInLineBiggestValue = intGrayValue;
                            intCheckInLinePos = y;
                        }
                    }
                }

                intGrayValue = objROI.ref_ROI.GetPixel(intCheckPixelStartX, y).Value;
                if (intGrayValue > intOutLineBiggestValue || intOutLineBiggestValue == -1)
                {

                    fIn = objROI.ref_ROI.GetPixel((intCheckPixelStartX + 1), y).Value;
                    fOut = objROI.ref_ROI.GetPixel((intCheckPixelStartX - 1), y).Value;
                    if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                    {
                        intOutLineBiggestValue = intGrayValue;
                        intCheckOutLinePos = y;
                    }
                }
            }

            // Calculate subPixel value
            fSubPixel = -1;
            if ((intInLineBiggestValue != 255) && (intInLineBiggestValue != -1))
            {
                fIn = objROI.ref_ROI.GetPixel((intCheckPixelStartX + 2), intCheckInLinePos).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckPixelStartX, intCheckInLinePos).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intInLineBiggestValue - fOut) / (fIn - fOut);
                    fWidth = fWidth - 1 + fSubPixel;
                }
            }
            else if (intOutLineBiggestValue != -1)
            {
                fIn = objROI.ref_ROI.GetPixel((intCheckPixelStartX + 1), intCheckOutLinePos).Value;
                fOut = objROI.ref_ROI.GetPixel((intCheckPixelStartX - 1), intCheckOutLinePos).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intOutLineBiggestValue - fOut) / (fIn - fOut);
                    fWidth = fWidth + fSubPixel;
                }
            }

            // ----- Check Right Pad -----
            intInLineBiggestValue = -1;
            intOutLineBiggestValue = -1;
            intCheckInLinePos = -1;
            intCheckOutLinePos = -1;

            // Get Check Position for calculation
            for (int y = intCheckPixelStartY; y < intCheckPixelEndY; y++)
            {
                intGrayValue = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 1, y).Value;
                if (intGrayValue >= intThresholdValue)
                {
                    if (intGrayValue > intInLineBiggestValue || intInLineBiggestValue == -1)
                    {
                        fIn = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 2, y).Value;
                        fOut = objROI.ref_ROI.GetPixel(intCheckPixelEndX, y).Value;
                        if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                        {
                            intInLineBiggestValue = intGrayValue;
                            intCheckInLinePos = y;
                        }
                    }
                }

                intGrayValue = objROI.ref_ROI.GetPixel(intCheckPixelEndX, y).Value;
                if (intGrayValue > intOutLineBiggestValue || intOutLineBiggestValue == -1)
                {
                    fIn = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 1, y).Value;
                    fOut = objROI.ref_ROI.GetPixel(intCheckPixelEndX + 1, y).Value;
                    if ((intGrayValue <= fIn) && (intGrayValue >= fOut) && (fIn != fOut))
                    {
                        intOutLineBiggestValue = intGrayValue;
                        intCheckOutLinePos = y;
                    }
                }
            }

            // Calculate subPixel value
            fSubPixel = -1;
            if ((intInLineBiggestValue != 255) && (intInLineBiggestValue != -1))
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 2, intCheckInLinePos).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckPixelEndX, intCheckInLinePos).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intInLineBiggestValue - fOut) / (fIn - fOut);
                    fWidth = fWidth - 1 + fSubPixel;
                }
            }
            else if (intOutLineBiggestValue != -1)
            {
                fIn = objROI.ref_ROI.GetPixel(intCheckPixelEndX - 1, intCheckOutLinePos).Value;
                fOut = objROI.ref_ROI.GetPixel(intCheckPixelEndX + 1, intCheckOutLinePos).Value;
                if ((fIn - fOut) > 0)
                {
                    fSubPixel = (intOutLineBiggestValue - fOut) / (fIn - fOut);
                    fWidth = fWidth + fSubPixel;
                }
            }
            #endregion
        }


        /// <summary>
        /// Get rectangle corner and middle edge points
        /// </summary>
        /// <param name="fCenterPointX">Rectangle center point x</param>
        /// <param name="fCenterPointY">rectangle center point y</param>
        /// <param name="fAngle">Rectangle angle</param>
        /// <param name="fWidth">Rectangle width</param>
        /// <param name="fHeight">Rectangle Height</param>
        /// <param name="pxy">Top left corner point</param>
        /// <param name="pXy">Top right corner point</param>
        /// <param name="pxY">Bottom left corner point</param>
        /// <param name="pXY">Bottom right corner point</param>
        /// <param name="px">Left line middle edge</param>
        /// <param name="pX">Right line middle edge</param>
        /// <param name="py">Top line middle edge</param>
        /// <param name="pY">Bottom line middle edge</param>
        public static void GetCornerAndMiddlePoints(float fCenterPointX, float fCenterPointY, float fAngle,
            float fWidth, float fHeight,
            ref PointF pxy, ref PointF pXy, ref PointF pxY, ref PointF pXY,
            ref PointF px, ref PointF pX, ref PointF py, ref PointF pY)
        {
            double fRadianAngle = (double)fAngle * Math.PI / 180; // Change angle from degree to radius unit

            float dSin = (float)Math.Sin(fRadianAngle);
            float dCos = (float)Math.Cos(fRadianAngle);
            float fPatternWidth = fWidth / 2f;
            float fPatternHeight = fHeight / 2f;

            px.X = fCenterPointX - (fPatternWidth * dCos);        // Mid edge X at Left Side
            px.Y = fCenterPointY - (fPatternWidth * dSin);         // Mid edge Y at Left Side

            pX.X = fCenterPointX + (fPatternWidth * dCos);       // Mid edge X at Right Side
            pX.Y = fCenterPointY + (fPatternWidth * dSin);        // Mid edge Y at Right Side

            pY.X = fCenterPointX - (fPatternHeight * dSin);      // Mid edge X at Top Side
            pY.Y = fCenterPointY + (fPatternHeight * dCos);      // Mid edge Y at Top Side

            py.X = fCenterPointX + (fPatternHeight * dSin);       // Mid edge X at Bottom Side
            py.Y = fCenterPointY - (fPatternHeight * dCos);      // Mid edge Y at Bottom Side

            pxy.X = px.X + (fPatternHeight * dSin);           // Corner  at Left Top 
            pxy.Y = px.Y - (fPatternHeight * dCos);

            pXy.X = py.X + (fPatternWidth * dCos);          // Corner at Right Top
            pXy.Y = py.Y + (fPatternWidth * dSin);

            pxY.X = pY.X - (fPatternWidth * dCos);           // Corner at Left Bottom
            pxY.Y = pY.Y - (fPatternWidth * dSin);

            pXY.X = pX.X - (fPatternHeight * dSin);          // Corner at Right Bottom
            pXY.Y = pX.Y + (fPatternHeight * dCos);
        }

        /// <summary>
        /// Get rectangle corner and middle edge points
        /// </summary>
        /// <param name="fCenterPointX">Rectangle center point x</param>
        /// <param name="fCenterPointY">rectangle center point y</param>
        /// <param name="fAngle">Rectangle angle</param>
        /// <param name="fWidth">Rectangle width</param>
        /// <param name="fHeight">Rectangle Height</param>
        /// <param name="pxy">Top left corner point</param>
        /// <param name="pXy">Top right corner point</param>
        /// <param name="pxY">Bottom left corner point</param>
        /// <param name="pXY">Bottom right corner point</param>
        public static void GetCornerPoints(float fCenterPointX, float fCenterPointY, float fAngle,
            float fWidth, float fHeight,
            ref PointF pxy, ref PointF pXy, ref PointF pxY, ref PointF pXY)
        {
            double fRadianAngle = (double)fAngle * Math.PI / 180; // Change angle from degree to radius unit

            float dSin = (float)Math.Sin(fRadianAngle);
            float dCos = (float)Math.Cos(fRadianAngle);
            float fPatternWidth = fWidth / 2f;
            float fPatternHeight = fHeight / 2f;

            PointF px = new PointF();
            PointF py = new PointF();
            PointF pX = new PointF();
            PointF pY = new PointF();
            px.X = fCenterPointX - (fPatternWidth * dCos);        // Mid edge X at Left Side
            px.Y = fCenterPointY - (fPatternWidth * dSin);         // Mid edge Y at Left Side

            pX.X = fCenterPointX + (fPatternWidth * dCos);       // Mid edge X at Right Side
            pX.Y = fCenterPointY + (fPatternWidth * dSin);        // Mid edge Y at Right Side

            pY.X = fCenterPointX - (fPatternHeight * dSin);      // Mid edge X at Top Side
            pY.Y = fCenterPointY + (fPatternHeight * dCos);      // Mid edge Y at Top Side

            py.X = fCenterPointX + (fPatternHeight * dSin);       // Mid edge X at Bottom Side
            py.Y = fCenterPointY - (fPatternHeight * dCos);      // Mid edge Y at Bottom Side

            pxy.X = px.X + (fPatternHeight * dSin);           // Corner  at Left Top 
            pxy.Y = px.Y - (fPatternHeight * dCos);

            pXy.X = py.X + (fPatternWidth * dCos);          // Corner at Right Top
            pXy.Y = py.Y + (fPatternWidth * dSin);

            pxY.X = pY.X - (fPatternWidth * dCos);           // Corner at Left Bottom
            pxY.Y = pY.Y - (fPatternWidth * dSin);

            pXY.X = pX.X - (fPatternHeight * dSin);          // Corner at Right Bottom
            pXY.Y = pX.Y + (fPatternHeight * dCos);
        }

        public static float GetModeValue(float[] arrSortedDataList, float fRange)
        {
            // Mode here mean 

            float fData = 0;
            int intMatchCount;
            int intMaxMatchCount = 0;
            int intMaxIndex = -1;
            for (int i = 0; i < arrSortedDataList.Length; i++)
            {
                if (arrSortedDataList[i] == fData)
                    continue;

                fData = arrSortedDataList[i];
                intMatchCount = 0;
                for (int j = 0; j < arrSortedDataList.Length; j++)
                {
                    if (i == j)
                        continue;

                    if ((arrSortedDataList[j] >= (fData - fRange)) &&
                        (arrSortedDataList[j] <= (fData + fRange)))
                    {
                        intMatchCount++;
                    }
                }

                if (intMaxMatchCount < intMatchCount)
                {
                    intMaxMatchCount = intMatchCount;
                    intMaxIndex = i;
                }
            }

            if (intMaxIndex == -1)
                return 0;
            else
                return arrSortedDataList[intMaxIndex];
        }

        public static int GetModeValue(int[] arrSortedDataList, float fRange)
        {
            // Mode here mean 

            float fData = 0;
            int intMatchCount;
            int intMaxMatchCount = 0;
            int intMaxIndex = -1;
            for (int i = 0; i < arrSortedDataList.Length; i++)
            {
                if (arrSortedDataList[i] == fData)
                    continue;

                fData = arrSortedDataList[i];
                intMatchCount = 0;
                for (int j = 0; j < arrSortedDataList.Length; j++)
                {
                    if (i == j)
                        continue;

                    if ((arrSortedDataList[j] >= (fData - fRange)) &&
                        (arrSortedDataList[j] <= (fData + fRange)))
                    {
                        intMatchCount++;
                    }
                }

                if (intMaxMatchCount < intMatchCount)
                {
                    intMaxMatchCount = intMatchCount;
                    intMaxIndex = i;
                }
            }

            if (intMaxIndex == -1)
                return 0;
            else
                return arrSortedDataList[intMaxIndex];
        }

        public static float GetModeRangeCenterValue(float[] arrSortedDataList, float fRange)
        {
            // Mode here mean 

            float fData = 0;
            int intMatchCount;
            int intMaxMatchCount = 0;
            int intMaxIndex = -1;
            float fRangeMinValue;
            float fRangeMaxValue;
            float fRangeCenterValue = 0;
            for (int i = 0; i < arrSortedDataList.Length; i++)
            {
                if (arrSortedDataList[i] == fData)
                    continue;

                fData = arrSortedDataList[i];
                intMatchCount = 0;
                fRangeMinValue = float.MaxValue;
                fRangeMaxValue = float.MinValue;
                for (int j = 0; j < arrSortedDataList.Length; j++)
                {
                    if (i == j)
                        continue;

                    if ((arrSortedDataList[j] >= (fData - fRange)) &&
                        (arrSortedDataList[j] <= (fData + fRange)))
                    {
                        intMatchCount++;

                        if (fRangeMinValue > arrSortedDataList[j])
                            fRangeMinValue = arrSortedDataList[j];
                        if (fRangeMaxValue < arrSortedDataList[j])
                            fRangeMaxValue = arrSortedDataList[j];
                    }
                }

                if (intMaxMatchCount < intMatchCount)
                {
                    intMaxMatchCount = intMatchCount;
                    intMaxIndex = i;
                    fRangeCenterValue = (fRangeMinValue + fRangeMaxValue) / 2;
                }
            }

            if (intMaxIndex == -1)
                return 0;
            else
                return fRangeCenterValue;
        }
        public static List<PointF> GetModeRangeCenterValueAndFilteredList(List<PointF> arrSortedDataList, float fRange, ref float fRangeCenterValue, SortingDirection sortDirection)
        {
            // Mode here mean 

            float fData = 0;
            int intMatchCount;
            int intMaxMatchCount = 0;
            int intMaxIndex = -1;
            float fRangeMinValue;
            float fRangeMaxValue;
            List<PointF> arrFilteredList = new List<PointF>();
            List<PointF> arrFinalFilteredList = new List<PointF>();
            fRangeCenterValue = 0;
            for (int i = 0; i < arrSortedDataList.Count; i++)
            {
                float fCurrentDatai = 0, fCurrentDataj = 0; ;
                arrFilteredList = new List<PointF>();

                if (sortDirection == SortingDirection.X)
                    fCurrentDatai = arrSortedDataList[i].X;
                else
                    fCurrentDatai = arrSortedDataList[i].Y;

                if (sortDirection == SortingDirection.X)
                    arrFilteredList.Add(new PointF(fCurrentDatai, arrSortedDataList[i].Y));
                else
                    arrFilteredList.Add(new PointF(arrSortedDataList[i].X, fCurrentDatai));

                if (fCurrentDatai == fData)
                    continue;

                fData = fCurrentDatai;
                intMatchCount = 0;
                fRangeMinValue = float.MaxValue;
                fRangeMaxValue = float.MinValue;
                for (int j = 0; j < arrSortedDataList.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (sortDirection == SortingDirection.X)
                        fCurrentDataj = arrSortedDataList[j].X;
                    else
                        fCurrentDataj = arrSortedDataList[j].Y;

                    if ((fCurrentDataj >= (fData - fRange)) &&
                        (fCurrentDataj <= (fData + fRange)))
                    {
                        if (sortDirection == SortingDirection.X)
                            arrFilteredList.Add(new PointF(fCurrentDataj, arrSortedDataList[j].Y));
                        else
                            arrFilteredList.Add(new PointF(arrSortedDataList[j].X, fCurrentDataj));

                        intMatchCount++;

                        if (fRangeMinValue > fCurrentDataj)
                            fRangeMinValue = fCurrentDataj;
                        if (fRangeMaxValue < fCurrentDataj)
                            fRangeMaxValue = fCurrentDataj;
                    }
                }

                if (intMaxMatchCount < intMatchCount)
                {
                    arrFinalFilteredList = arrFilteredList;
                    intMaxMatchCount = intMatchCount;
                    intMaxIndex = i;
                    fRangeCenterValue = (fRangeMinValue + fRangeMaxValue) / 2;
                }
            }

            //if (intMaxIndex == -1)
            //    return 0;
            //else
            return arrFinalFilteredList;
        }
        public static bool GetNewXYAfterRotate(float fRotationCenterX, float fRotationCenterY, float fSamplePointX, float fSamplePointY, float fRotateAngle,
           ref float fNewSamplePointX, ref float fNewSamplePointY)
        {
            double dDistanceBtwRotationCenterPointAndSamplePoint = Math.Pow(Math.Pow(fSamplePointX, 2) + Math.Pow(fSamplePointY, 2), 0.5);

            double dSamplePointAngleDeg = (Math.Atan(fSamplePointY / fSamplePointX) * 180 / Math.PI);

            double dNewSamplePointAngleRad = (dSamplePointAngleDeg + fRotateAngle) / 180 * Math.PI;

            fNewSamplePointY = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);

            fNewSamplePointX = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);

            return true;
        }

        public static bool GetNewXYAfterRotate_360deg(float fRotationCenterX, float fRotationCenterY, float fSamplePointX, float fSamplePointY, float fRotateAngle,
            ref float fNewSamplePointX, ref float fNewSamplePointY)
        {
            // define current angle using North-angle formula
            // add rotate angle to current angle = new angle
            // clasify new angle in which direction (TL, TR, BL, or BR)
            // Find angle using Trigonometry

            double dCurrentAngle = 0;
            double dDistanceX = fSamplePointX - fRotationCenterX;
            double dDistanceY = fSamplePointY - fRotationCenterY;
            double dDistanceBtwRotationCenterPointAndSamplePoint = Math.Pow(Math.Pow(dDistanceX, 2) + Math.Pow(dDistanceY, 2), 0.5);
            double dSamplePointAngleDeg = (Math.Atan(dDistanceY / dDistanceX) * 180 / Math.PI);

            double dNewSamplePointAngleRad = 0;
            if (dDistanceX < 0 && dDistanceY < 0)
            {
                // Sample Point at top left
                dCurrentAngle = 270 + dSamplePointAngleDeg;
            }
            else if (dDistanceX > 0 && dDistanceY < 0)
            {
                // Sample Point at top right
                dCurrentAngle = 90 + dSamplePointAngleDeg;
            }
            else if (dDistanceX > 0 && dDistanceY > 0)
            {
                // Sample Point at bottom right
                dCurrentAngle = 90 + dSamplePointAngleDeg;
            }
            else if (dDistanceX < 0 && dDistanceY > 0)
            {
                // Sample Point at bottom left
                dCurrentAngle = 270 + dSamplePointAngleDeg;
            }
            else if (dDistanceX == 0 && dDistanceY == 0)
            {
                // Sample Point at same point of rotation point. 
                fNewSamplePointX = fSamplePointX;
                fNewSamplePointY = fSamplePointY;
                return true;
            }
            else if (dDistanceX == 0 && dDistanceY < 0)
            {
                // Sample Point at same line with y axis and at top side of rotation point
                dCurrentAngle = 0;
            }
            else if (dDistanceX == 0 && dDistanceY > 0)
            {
                // Sample Point at same line with y axis and at bottom side of rotation point
                dCurrentAngle = 180;
            }
            else if (dDistanceY == 0 && dDistanceX < 0)
            {
                // Sample Point at same line with x axis and at left side of rotation point
                dCurrentAngle = 270;
            }
            else if (dDistanceY == 0 && dDistanceX > 0)
            {
                // Sample Point at same line with x axis and at right side of rotation point
                dCurrentAngle = 90;
            }

            dCurrentAngle += fRotateAngle;
            dCurrentAngle %= 360;   // make sure angle no over 360

            if (dCurrentAngle < 0)
            {
                dCurrentAngle = 360 + dCurrentAngle;    // make sure angle in positive value
            }

            // -------- Find angle using Trigonometry ------------------------------
            if (dCurrentAngle >= 0 && dCurrentAngle <= 90)
            {
                dNewSamplePointAngleRad = 90 - dCurrentAngle;

                dNewSamplePointAngleRad = (dNewSamplePointAngleRad) / 180 * Math.PI;

                fNewSamplePointY = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                fNewSamplePointY = fRotationCenterY - fNewSamplePointY;

                fNewSamplePointX = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                fNewSamplePointX = fRotationCenterX + fNewSamplePointX;
            }
            else if (dCurrentAngle > 90 && dCurrentAngle < 180)
            {
                dNewSamplePointAngleRad = dCurrentAngle - 90;

                dNewSamplePointAngleRad = (dNewSamplePointAngleRad) / 180 * Math.PI;

                fNewSamplePointY = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                fNewSamplePointY = fRotationCenterY + fNewSamplePointY;

                fNewSamplePointX = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                fNewSamplePointX = fRotationCenterX + fNewSamplePointX;
            }
            else if (dCurrentAngle >= 180 && dCurrentAngle <= 270)
            {
                dNewSamplePointAngleRad = 270 - dCurrentAngle;

                dNewSamplePointAngleRad = (dNewSamplePointAngleRad) / 180 * Math.PI;

                fNewSamplePointY = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                fNewSamplePointY = fRotationCenterY + fNewSamplePointY;

                fNewSamplePointX = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                fNewSamplePointX = fRotationCenterX - fNewSamplePointX;
            }
            else if (dCurrentAngle > 270 && dCurrentAngle < 360)
            {
                dNewSamplePointAngleRad = dCurrentAngle - 270;

                dNewSamplePointAngleRad = (dNewSamplePointAngleRad) / 180 * Math.PI;

                fNewSamplePointY = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                fNewSamplePointY = fRotationCenterY - fNewSamplePointY;

                fNewSamplePointX = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                fNewSamplePointX = fRotationCenterX - fNewSamplePointX;
            }


            return true;
        }

        public static bool GetNewXYAfterRotate_360deg(float fRotationCenterX, float fRotationCenterY, PointF pSamplePoint, float fRotateAngle,
            ref PointF pNewSamplePoint)
        {
            // define current angle using North-angle formula
            // add rotate angle to current angle = new angle
            // clasify new angle in which direction (TL, TR, BL, or BR)
            // Find angle using Trigonometry

            double dCurrentAngle = 0;
            double dDistanceX = pSamplePoint.X - fRotationCenterX;
            double dDistanceY = pSamplePoint.Y - fRotationCenterY;
            double dDistanceBtwRotationCenterPointAndSamplePoint = Math.Pow(Math.Pow(dDistanceX, 2) + Math.Pow(dDistanceY, 2), 0.5);
            double dSamplePointAngleDeg = (Math.Atan(dDistanceY / dDistanceX) * 180 / Math.PI);

            double dNewSamplePointAngleRad = 0;
            if (dDistanceX < 0 && dDistanceY < 0)
            {
                // Sample Point at top left
                dCurrentAngle = 270 + dSamplePointAngleDeg;
            }
            else if (dDistanceX > 0 && dDistanceY < 0)
            {
                // Sample Point at top right
                dCurrentAngle = 90 + dSamplePointAngleDeg;
            }
            else if (dDistanceX > 0 && dDistanceY > 0)
            {
                // Sample Point at bottom right
                dCurrentAngle = 90 + dSamplePointAngleDeg;
            }
            else if (dDistanceX < 0 && dDistanceY > 0)
            {
                // Sample Point at bottom left
                dCurrentAngle = 270 + dSamplePointAngleDeg;
            }
            else if (dDistanceX == 0 && dDistanceY == 0)
            {
                // Sample Point at same point of rotation point. 
                pNewSamplePoint.X = pSamplePoint.X;
                pNewSamplePoint.Y = pSamplePoint.Y;
                return true;
            }
            else if (dDistanceX == 0 && dDistanceY < 0)
            {
                // Sample Point at same line with y axis and at top side of rotation point
                dCurrentAngle = 0;
            }
            else if (dDistanceX == 0 && dDistanceY > 0)
            {
                // Sample Point at same line with y axis and at bottom side of rotation point
                dCurrentAngle = 180;
            }
            else if (dDistanceY == 0 && dDistanceX < 0)
            {
                // Sample Point at same line with x axis and at left side of rotation point
                dCurrentAngle = 270;
            }
            else if (dDistanceY == 0 && dDistanceX > 0)
            {
                // Sample Point at same line with x axis and at right side of rotation point
                dCurrentAngle = 90;
            }

            dCurrentAngle += fRotateAngle;
            dCurrentAngle %= 360;   // make sure angle no over 360

            if (dCurrentAngle < 0)
            {
                dCurrentAngle = 360 + dCurrentAngle;    // make sure angle in positive value
            }

            // -------- Find angle using Trigonometry ------------------------------
            if (dCurrentAngle >= 0 && dCurrentAngle <= 90)
            {
                dNewSamplePointAngleRad = 90 - dCurrentAngle;

                dNewSamplePointAngleRad = (dNewSamplePointAngleRad) / 180 * Math.PI;

                pNewSamplePoint.Y = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                pNewSamplePoint.Y = fRotationCenterY - pNewSamplePoint.Y;

                pNewSamplePoint.X = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                pNewSamplePoint.X = fRotationCenterX + pNewSamplePoint.X;
            }
            else if (dCurrentAngle > 90 && dCurrentAngle < 180)
            {
                dNewSamplePointAngleRad = dCurrentAngle - 90;

                dNewSamplePointAngleRad = (dNewSamplePointAngleRad) / 180 * Math.PI;

                pNewSamplePoint.Y = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                pNewSamplePoint.Y = fRotationCenterY + pNewSamplePoint.Y;

                pNewSamplePoint.X = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                pNewSamplePoint.X = fRotationCenterX + pNewSamplePoint.X;
            }
            else if (dCurrentAngle >= 180 && dCurrentAngle <= 270)
            {
                dNewSamplePointAngleRad = 270 - dCurrentAngle;

                dNewSamplePointAngleRad = (dNewSamplePointAngleRad) / 180 * Math.PI;

                pNewSamplePoint.Y = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                pNewSamplePoint.Y = fRotationCenterY + pNewSamplePoint.Y;

                pNewSamplePoint.X = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                pNewSamplePoint.X = fRotationCenterX - pNewSamplePoint.X;
            }
            else if (dCurrentAngle > 270 && dCurrentAngle < 360)
            {
                dNewSamplePointAngleRad = dCurrentAngle - 270;

                dNewSamplePointAngleRad = (dNewSamplePointAngleRad) / 180 * Math.PI;

                pNewSamplePoint.Y = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                pNewSamplePoint.Y = fRotationCenterY - pNewSamplePoint.Y;

                pNewSamplePoint.X = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);
                pNewSamplePoint.X = fRotationCenterX - pNewSamplePoint.X;
            }


            return true;
        }

        public static void RotateWithAngleAccordingToReferencePoint(float fRotationCenterX, float fRotationCenterY, float fSamplePointX, float fSamplePointY, float fRotateAngle,
          ref float fNewSamplePointX, ref float fNewSamplePointY)
        {
            fNewSamplePointX = (float)((fRotationCenterX) + ((fSamplePointX - fRotationCenterX) * Math.Cos(fRotateAngle * Math.PI / 180)) -
      ((fSamplePointY - fRotationCenterY) * Math.Sin(fRotateAngle * Math.PI / 180)));

            fNewSamplePointY = (float)((fRotationCenterY) + ((fSamplePointX - fRotationCenterX) * Math.Sin(fRotateAngle * Math.PI / 180)) +
             ((fSamplePointY - fRotationCenterY) * Math.Cos(fRotateAngle * Math.PI / 180)));
        }

        /// <summary>
        /// Rotate Function for New Point 1
        /// </summary>
        /// <param name="fRotationCenterX"></param>
        /// <param name="fRotationCenterY"></param>
        /// <param name="fSamplePointX"></param>
        /// <param name="fSamplePointY"></param>
        /// <param name="fRotateAngle"></param>
        /// <param name="fNewSamplePointX"></param>
        /// <param name="fNewSamplePointY"></param>
        /// <returns></returns>
        public static bool NewXYAfterRotate1(float fRotationCenterX, float fRotationCenterY, float fSamplePointX, float fSamplePointY, float fRotateAngle,
          ref float fNewSamplePointX, ref float fNewSamplePointY)
        {
            double dDistanceBtwRotationCenterPointAndSamplePoint = Math.Pow(Math.Pow(Math.Abs(fRotationCenterX - fSamplePointX), 2) + Math.Pow(Math.Abs(fRotationCenterY - fSamplePointY), 2), 0.5);

            double dSamplePointAngleDeg = (Math.Atan(Math.Abs(fRotationCenterY - fSamplePointY) / Math.Abs(fRotationCenterX - fSamplePointX)) * 180 / Math.PI);

            double dNewSamplePointAngleRad = (dSamplePointAngleDeg + fRotateAngle) / 180 * Math.PI;

            fNewSamplePointY = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);

            fNewSamplePointY = fRotationCenterY - fNewSamplePointY;

            fNewSamplePointX = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);

            fNewSamplePointX = fRotationCenterX - fNewSamplePointX;

            return true;
        }
        /// <summary>
        /// Rotate Function for New Point 2
        /// </summary>
        /// <param name="fRotationCenterX"></param>
        /// <param name="fRotationCenterY"></param>
        /// <param name="fSamplePointX"></param>
        /// <param name="fSamplePointY"></param>
        /// <param name="fRotateAngle"></param>
        /// <param name="fNewSamplePointX"></param>
        /// <param name="fNewSamplePointY"></param>
        /// <returns></returns>
        public static bool NewXYAfterRotate2(float fRotationCenterX, float fRotationCenterY, float fSamplePointX, float fSamplePointY, float fRotateAngle,
        ref float fNewSamplePointX, ref float fNewSamplePointY)
        {
            double dDistanceBtwRotationCenterPointAndSamplePoint = Math.Pow(Math.Pow(Math.Abs(fRotationCenterX - fSamplePointX), 2) + Math.Pow(Math.Abs(fSamplePointY - fRotationCenterY), 2), 0.5);

            double dSamplePointAngleDeg = (Math.Atan(Math.Abs(fSamplePointY - fRotationCenterY) / Math.Abs(fRotationCenterX - fSamplePointX)) * 180 / Math.PI);

            double dNewSamplePointAngleRad = (fRotateAngle - dSamplePointAngleDeg) / 180 * Math.PI;

            fNewSamplePointY = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);

            fNewSamplePointY = fRotationCenterY - fNewSamplePointY;

            fNewSamplePointX = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);

            fNewSamplePointX = fRotationCenterX - fNewSamplePointX;

            return true;
        }
        /// <summary>
        /// Rotate Function for New Point 3
        /// </summary>
        /// <param name="fRotationCenterX"></param>
        /// <param name="fRotationCenterY"></param>
        /// <param name="fSamplePointX"></param>
        /// <param name="fSamplePointY"></param>
        /// <param name="fRotateAngle"></param>
        /// <param name="fNewSamplePointX"></param>
        /// <param name="fNewSamplePointY"></param>
        /// <returns></returns>
        public static bool NewXYAfterRotate3(float fRotationCenterX, float fRotationCenterY, float fSamplePointX, float fSamplePointY, float fRotateAngle,
        ref float fNewSamplePointX, ref float fNewSamplePointY)
        {
            double dDistanceBtwRotationCenterPointAndSamplePoint = Math.Pow(Math.Pow(Math.Abs(fSamplePointX - fRotationCenterX), 2) + Math.Pow(Math.Abs(fRotationCenterY - fSamplePointY), 2), 0.5);

            double dSamplePointAngleDeg = (Math.Atan(Math.Abs(fRotationCenterY - fSamplePointY) / Math.Abs(fSamplePointX - fRotationCenterX)) * 180 / Math.PI);

            double dNewSamplePointAngleRad = (fRotateAngle - dSamplePointAngleDeg) / 180 * Math.PI;

            fNewSamplePointY = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);

            fNewSamplePointY = fRotationCenterY + fNewSamplePointY;

            fNewSamplePointX = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);

            fNewSamplePointX = fRotationCenterX + fNewSamplePointX;

            return true;
        }
        /// <summary>
        /// Rotate Function for New Point 4
        /// </summary>
        /// <param name="fRotationCenterX"></param>
        /// <param name="fRotationCenterY"></param>
        /// <param name="fSamplePointX"></param>
        /// <param name="fSamplePointY"></param>
        /// <param name="fRotateAngle"></param>
        /// <param name="fNewSamplePointX"></param>
        /// <param name="fNewSamplePointY"></param>
        /// <returns></returns>
        public static bool NewXYAfterRotate4(float fRotationCenterX, float fRotationCenterY, float fSamplePointX, float fSamplePointY, float fRotateAngle,
        ref float fNewSamplePointX, ref float fNewSamplePointY)
        {
            double dDistanceBtwRotationCenterPointAndSamplePoint = Math.Pow(Math.Pow(Math.Abs(fSamplePointX - fRotationCenterX), 2) + Math.Pow(Math.Abs(fSamplePointY - fRotationCenterY), 2), 0.5);

            double dSamplePointAngleDeg = (Math.Atan(Math.Abs(fSamplePointY - fRotationCenterY) / Math.Abs(fSamplePointX - fRotationCenterX)) * 180 / Math.PI);

            double dNewSamplePointAngleRad = (dSamplePointAngleDeg + fRotateAngle) / 180 * Math.PI;

            fNewSamplePointY = (float)(Math.Sin(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);

            fNewSamplePointY = fRotationCenterY + fNewSamplePointY;

            fNewSamplePointX = (float)(Math.Cos(dNewSamplePointAngleRad) * dDistanceBtwRotationCenterPointAndSamplePoint);

            fNewSamplePointX = fRotationCenterX + fNewSamplePointX;

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="intDirection">0=Up, 1=Right, 2=Down, 3=Left</param>
        /// <param name="fSizeChangeValue"></param>
        /// <param name="fOriCenterValue"></param>
        /// <returns></returns>
        private static float GetNewCenterPointWhenSizeChange(int intDirection, float fSizeChangeValue, float fOriCenterValue)
        {
            switch (intDirection)
            {
                case 0: // Up
                    return fOriCenterValue - fSizeChangeValue / 2;
                case 1: // Right
                    return fOriCenterValue + fSizeChangeValue / 2;
                case 2: // Down
                    return fOriCenterValue + fSizeChangeValue / 2;
                case 3: // Left
                    return fOriCenterValue - fSizeChangeValue / 2;
            }

            return fOriCenterValue;
        }


        public static void GetCenterPointWithFourPointFormingRectangle(List<PointF> Start, List<PointF> End, ref float CenterX, ref float CenterY)
        {
            //Only Valid for Right Angle Lines

            float slope1, y_Intercept1, slope2, y_Intercept2;

            float MidX1 = (Start[0].X + End[0].X) / 2;
            float MidX2 = (Start[1].X + End[1].X) / 2;
            float MidY1 = (Start[0].Y + End[0].Y) / 2;
            float MidY2 = (Start[1].Y + End[1].Y) / 2;

            slope1 = (End[0].Y - Start[0].Y) / (End[0].X - Start[0].X);
            float slope_Perpendicular1 = -(1 / slope1);

            slope2 = (End[1].Y - Start[1].Y) / (End[1].X - Start[1].X);
            float slope_Perpendicular2 = -(1 / slope2);

            y_Intercept1 = MidY1 - (slope_Perpendicular1 * MidX1);
            y_Intercept2 = MidY2 - (slope_Perpendicular2 * MidX2);
            if (!float.IsInfinity(slope_Perpendicular1) && !float.IsNaN(slope_Perpendicular1) && slope_Perpendicular1 != 0 && !float.IsInfinity(slope_Perpendicular2) && !float.IsNaN(slope_Perpendicular2) && slope_Perpendicular2 != 0)
            {
                CenterX = (y_Intercept2 - y_Intercept1) / (slope_Perpendicular1 - slope_Perpendicular2);
                CenterY = slope_Perpendicular1 * CenterX + y_Intercept1;
            }

        }

        public static float DefineOptimumFloat(List<float> arrValue, float fTolerance)
        {
            int intMaxMatchCount = 0;
            int intMaxMatchCountIndex = -1;
            for (int i = 0; i < arrValue.Count; i++)
            {
                int intMatchCount = 0;
                for (int j = 0; j < arrValue.Count; j++)
                {
                    if (i == j)
                        continue;


                    if ((arrValue[j] >= (arrValue[i] - fTolerance)) && (arrValue[j] <= (arrValue[i] + fTolerance)))
                    {
                        intMatchCount++;
                    }
                }

                if (intMatchCount > intMaxMatchCount)
                {
                    intMaxMatchCount = intMatchCount;
                    intMaxMatchCountIndex = i;
                }
            }

            if (intMaxMatchCountIndex == -1)
                return 0;

            float fTotalValue = 0;
            int intMatchCount2 = 0;
            for (int j = 0; j < arrValue.Count; j++)
            {
                if ((arrValue[j] >= (arrValue[intMaxMatchCountIndex] - fTolerance) && arrValue[j] <= (arrValue[intMaxMatchCountIndex] + fTolerance)) &&
                    (arrValue[j] >= (arrValue[intMaxMatchCountIndex] - fTolerance) && arrValue[j] <= (arrValue[intMaxMatchCountIndex] + fTolerance)))
                {
                    fTotalValue += arrValue[j];
                    intMatchCount2++;
                }
            }

            return (fTotalValue / intMatchCount2);
        }
        
    }
}
