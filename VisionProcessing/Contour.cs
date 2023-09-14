using System;
using System.Collections.Generic;
using System.Text;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;
using System.Drawing;


namespace VisionProcessing
{
    public class Contour
    {
        #region Member Variables

        private List<EBW8PathVector> m_pvContourList = new List<EBW8PathVector>();
        private List<List<int>> m_intPathDirection = new List<List<int>>();
        #endregion

        #region Properties
        public List<EBW8PathVector> ref_pvContourList { get { return m_pvContourList; } } 
        #endregion

        private List<List<Line>> m_arrLine = new List<List<Line>>();

        public Contour()
        {
        }

        public void ClearContour()
        {
            for (int i = 0; i < m_pvContourList.Count; i++)
            {
                m_pvContourList[i].Dispose();
            }

            m_pvContourList.Clear();
            m_intPathDirection.Clear();
        }

        public bool MatchObject(int intObjectNum, EBW8PathVector pv)
        {
            EBW8PathVector pv2 = m_pvContourList[intObjectNum];
            List<int> intMatchList = new List<int>();
            int intNumElements1 = (int)pv.NumElements;
            int intNumElements2 = (int)pv2.NumElements;
            int[] intX = new int[4];
            int[] intY = new int[4];
            int intTopIndex, intBottomIndex, intLeftIndex, intRightIndex;
            int i, k;

            for (i = 0; i < intNumElements1; i++)
            {
                intTopIndex = intBottomIndex = intLeftIndex = intRightIndex = -1;

                intX[0] = intX[2] = pv.GetElement(i).X;
                intX[1] = intX[3] = -1;
                intY[0] = intY[2] = -1;
                intY[1] = intY[3] = pv.GetElement(i).Y;

                for (k = 0; k < intNumElements2; k++)
                {
                    if ((pv2.GetElement(k).X == pv.GetElement(i).X) &&
                        (pv2.GetElement(k).Y == pv.GetElement(i).Y))
                        return true;

                    if (pv2.GetElement(k).X == pv.GetElement(i).X)
                    {
                        // Get top point
                        if (pv2.GetElement(k).Y < pv.GetElement(i).Y)
                        {
                            if ((intY[0] == -1) || (intY[0] > pv2.GetElement(k).Y))
                            {
                                intY[0] = pv2.GetElement(k).Y;
                                intTopIndex = k;
                            }
                        }
                        // Get bottom point
                        else
                        {
                            if ((intY[2] == -1) || (intY[2] < pv2.GetElement(k).Y))
                            {
                                intY[2] = pv2.GetElement(k).Y;
                                intBottomIndex = k;
                            }
                        }
                    }

                    if (pv2.GetElement(k).Y == pv.GetElement(i).Y)
                    {
                        // Get left point
                        if (pv2.GetElement(k).X < pv.GetElement(i).X)
                        {
                            if ((intX[3] == -1) || (intX[3] > pv2.GetElement(k).X))
                            {
                                intX[3] = pv2.GetElement(k).X;
                                intLeftIndex = k;
                            }

                        }
                        // Get right point
                        else
                        {
                            if ((intX[1] == -1) || (intX[1] < pv2.GetElement(k).X))
                            {
                                intX[1] = pv2.GetElement(k).X;
                                intRightIndex = k;
                            }
                        }
                    }
                }

                if (((m_intPathDirection[intObjectNum][intTopIndex] & 0x08) > 0) &&
                    ((m_intPathDirection[intObjectNum][intRightIndex] & 0x01) > 0) &&
                    ((m_intPathDirection[intObjectNum][intBottomIndex] & 0x04) > 0) &&
                    ((m_intPathDirection[intObjectNum][intLeftIndex] & 0x02) > 0))
                    return true;
            }

            return false;
        }

        public bool MatchObject(int intObjectNum, int intStartX, int intStartY, int intEndX, int intEndY, int intLoopCount)
        {
            int intTolerance = 1; //31-07-2019 ZJYEOH : Add tolerance because contour built on ROI after subtracted Dilated template mark
            int intNumElements = (int)m_pvContourList[intObjectNum].NumElements;
            int intSampling = Math.Max(1, (int)m_pvContourList[intObjectNum].NumElements / intLoopCount);
            // Scan to check whether selected contour in rectangle area or not
            for (int i = 0; i < intNumElements; i += intSampling)   // 2019 07 11 - CCENG: use sampling for faster process. use 30 because it is good enough to check all direction. The 30 may change to settingable if need to change next time.
            //for (int i = 0; i < m_pvContourList[intObjectNum].NumElements; i += intSamplingStep)

            {
                if ((m_pvContourList[intObjectNum].GetElement(i).X >= intStartX - intTolerance) && (m_pvContourList[intObjectNum].GetElement(i).Y >= intStartY - intTolerance) &&
                (m_pvContourList[intObjectNum].GetElement(i).X <= intEndX + intTolerance) && (m_pvContourList[intObjectNum].GetElement(i).Y <= intEndY + intTolerance))
                {
                    return true;
                }
            }

            //---------- can improve it by combine all together

            //if (MatchObjectWithUpperXLine(intObjectNum, intStartX, intStartX, intEndY, intSamplingStep))
            //    return true;		m_pvContourList[intObjectNum].GetElement(i).X	244	int

            //if (MatchObjectWithBottomXLine(intObjectNum, intEndX, intStartY, intEndY, intSamplingStep))
            //    return true;
            //if (MatchObjectWithLeftYLine(intObjectNum, intStartY, intStartX, intEndX, intSamplingStep))
            //    return true;
            //if (MatchObjectWithLeftYLine(intObjectNum, intEndY, intStartX, intEndX, intSamplingStep))
            //    return true;

            //-------- time consume a lot here -----------------------------------------------------

            // Scan OCV char top line and bottom line.
            //for (int x = intStartX; x <= intEndX; x += intSamplingStep)
            //{
            //    if (MatchObject(intObjectNum, x, intStartY))
            //        return true;

            //    if (MatchObject(intObjectNum, x, intEndY))
            //        return true;
            //}

            //// Scan OCV char left line and right line
            //for (int y = intStartY; y <= intEndY; y += intSamplingStep)
            //{
            //    if (MatchObject(intObjectNum, intStartX, y))
            //        return true;

            //    if (MatchObject(intObjectNum, intEndX, y))
            //        return true;
            //}

            return false;
        }

        public bool MatchObject(int intObjectNum, int intPointX, int intPointY)
        {
            EBW8PathVector pv = m_pvContourList[intObjectNum];
            int intNumElements = (int)pv.NumElements;
            int intTopX, intTopY, intBottomX, intBottomY, intLeftX, intLeftY, intRightX, intRightY;
            int intTopIndex, intBottomIndex, intLeftIndex, intRightIndex;
            int i;
            int intX, intY;

            intTopIndex = intBottomIndex = intLeftIndex = intRightIndex = -1;
            intTopX = intBottomX = intPointX;
            intRightX = intLeftX = -1;
            intTopY = intBottomY = -1;
            intRightY = intLeftY = intPointY;

            for (i = 0; i < intNumElements; i++)
            {
                intX = pv.GetElement(i).X;
                intY = pv.GetElement(i).Y;

                if ((intX == intPointX) &&
                   (intY == intPointY))
                    return true;

                if (intX == intPointX)
                {
                    // Get top point
                    if (intY < intPointY)
                    {
                        // Pad use if ((intTopY == -1) || (intTopY > intY)), verify pad code
                        if ((intTopY == -1) || (intTopY < intY))
                        {
                            intTopY = intY;
                            intTopIndex = i;
                        }
                    }
                    // Get bottom point
                    else
                    {
                        // Pad use if ((intBottomY == -1) || (intBottomY < intY)), verify pad code
                        if ((intBottomY == -1) || (intBottomY > intY))
                        {
                            intBottomY = intY;
                            intBottomIndex = i;
                        }
                    }
                }

                if (intY == intPointY)
                {
                    // Get left point
                    if (intX < intPointX)
                    {
                        // Pad use if ((intLeftX == -1) || (intLeftX > intX)), verify pad code
                        if ((intLeftX == -1) || (intLeftX < intX))
                        {
                            intLeftX = intX;
                            intLeftIndex = i;
                        }

                    }
                    // Get right point
                    else
                    {
                        // Pad use if ((intRightX == -1) || (intRightX < intX)), verify pad code
                        if ((intRightX == -1) || (intRightX > intX))
                        {
                            intRightX = intX;
                            intRightIndex = i;
                        }
                    }
                }

                // Pad use this code here instead of put the code in below
                //if (((m_intPathDirection[intObjectNum][intTopIndex] & 0x08) > 0) &&
                //    ((m_intPathDirection[intObjectNum][intRightIndex] & 0x01) > 0) &&
                //    ((m_intPathDirection[intObjectNum][intBottomIndex] & 0x04) > 0) &&
                //    ((m_intPathDirection[intObjectNum][intLeftIndex] & 0x02) > 0))
                //    return true;
            }

            if ((intTopIndex != -1) && (intRightIndex != -1) && (intBottomIndex != -1) && (intLeftIndex != -1))
                if (((m_intPathDirection[intObjectNum][intTopIndex] & 0x08) > 0) &&
                    ((m_intPathDirection[intObjectNum][intRightIndex] & 0x01) > 0) &&
                    ((m_intPathDirection[intObjectNum][intBottomIndex] & 0x04) > 0) &&
                    ((m_intPathDirection[intObjectNum][intLeftIndex] & 0x02) > 0))
                    return true;

            return false;
        }

        public List<int> MatchObject2(EBW8PathVector pv1, int[] intCheckList, int intSamplingStep)
        {
            EBW8PathVector pv2;
            List<int> intMatchList = new List<int>();
            int intNumElements1 = (int)pv1.NumElements;
            int intNumElements2;
            int intNumContour = m_pvContourList.Count;
            int intTopX, intTopY, intBottomX, intBottomY, intLeftX, intLeftY, intRightX, intRightY;
            int intTopIndex, intBottomIndex, intLeftIndex, intRightIndex;
            int i, j, c, m;
            int intX1, intY1, intX2, intY2;

            // Scan all contour points (From sample)
            for (i = 0; i < intNumElements1; i += intSamplingStep)
            {
                // Get contour point position
                intX1 = pv1.GetElement(i).X;
                intY1 = pv1.GetElement(i).Y;

                // This function consider done if MatchList == CheckList (MatchList will not higher than CheckList)
                if (intMatchList.Count == intCheckList.Length)
                    break;

                for (c = 0; c < intCheckList.Length; c++)
                {
                    // This function consider done if MatchList == CheckList (MatchList will not higher than CheckList)
                    if (intMatchList.Count == intCheckList.Length)
                        break;

                    if ((intCheckList[c] < 0) || (intCheckList[c] >= m_pvContourList.Count))
                        continue;

                    for (m = 0; m < intMatchList.Count; m++)
                    {
                        if (intMatchList[m] == intCheckList[c])
                            break;
                    }
                    if (m < intMatchList.Count)
                        continue;

                    pv2 = m_pvContourList[intCheckList[c]];
                    intNumElements2 = (int)pv2.NumElements;

                    intTopIndex = intBottomIndex = intLeftIndex = intRightIndex = -1;
                    intTopX = intBottomX = intX1;
                    intRightX = intLeftX = -1;
                    intTopY = intBottomY = -1;
                    intRightY = intLeftY = intY1;

                    bool blnFound = false;
                    // Scan all template contour point 
                    for (j = 0; j < intNumElements2; j++)
                    {
                        intX2 = pv2.GetElement(j).X;
                        intY2 = pv2.GetElement(j).Y;

                        // Template vector point and sample vector point are same
                        if ((intX1 == intX2) &&
                           (intY1 == intY2))
                        {
                            intMatchList.Add(intCheckList[c]);
                            blnFound = true;
                            break;
                        }

                        if (intX1 == intX2) // if both x are sample point, then you have to check whether the sample point 
                        {
                            // Get top point (if template point is at the top of sample point)
                            if (intY2 < intY1)
                            {
                                if ((intTopY == -1) || (intTopY > intY2))
                                {
                                    intTopY = intY2;
                                    intTopIndex = j;
                                }
                            }
                            // Get bottom point (if template point is at the bottom of sample point)
                            else
                            {
                                if ((intBottomY == -1) || (intBottomY < intY2))
                                {
                                    intBottomY = intY2;
                                    intBottomIndex = j;
                                }
                            }
                        }

                        if (intY1 == intY2)
                        {
                            // Get left point
                            if (intX2 < intX1)
                            {
                                if ((intLeftX == -1) || (intLeftX > intX2))
                                {
                                    intLeftX = intX2;
                                    intLeftIndex = j;
                                }

                            }
                            // Get right point
                            else
                            {
                                if ((intRightX == -1) || (intRightX < intX2))
                                {
                                    intRightX = intX2;
                                    intRightIndex = j;
                                }
                            }
                        }
                    }

                    if (blnFound)
                        continue;

                    if ((intTopIndex != -1) && (intRightIndex != -1) && (intBottomIndex != -1) && (intLeftIndex != -1))
                        if (((m_intPathDirection[intCheckList[c]][intTopIndex] & 0x08) > 0) &&
                            ((m_intPathDirection[intCheckList[c]][intRightIndex] & 0x01) > 0) &&
                            ((m_intPathDirection[intCheckList[c]][intBottomIndex] & 0x04) > 0) &&
                            ((m_intPathDirection[intCheckList[c]][intLeftIndex] & 0x02) > 0))
                        {
                            intMatchList.Add(intCheckList[c]);
                        }
                }
            }

            if (intMatchList.Count == 0 && intCheckList.Length == 1)
            {
                pv2 = m_pvContourList[intCheckList[0]];
                intNumElements2 = (int)pv2.NumElements;

                for (j = 0; j < intNumElements2; j++)
                {
                    intX2 = pv2.GetElement(j).X;
                    intY2 = pv2.GetElement(j).Y;

                    intTopIndex = intBottomIndex = intLeftIndex = intRightIndex = -1;
                    intTopX = intBottomX = intX2;
                    intRightX = intLeftX = -1;
                    intTopY = intBottomY = -1;
                    intRightY = intLeftY = intY2;

                    intNumElements1 = (int)pv1.NumElements;
                    bool blnFound = false;
                    
                    for (i = 0; i < intNumElements1; i += 1)
                    {
                        // Get contour point position
                        intX1 = pv1.GetElement(i).X;
                        intY1 = pv1.GetElement(i).Y;

                        // Template vector point and sample vector point are same
                        if ((intX1 == intX2) &&
                           (intY1 == intY2))
                        {
                            intMatchList.Add(intCheckList[0]);
                            blnFound = true;
                            break;
                        }

                        if (intX1 == intX2) // if both x are sample point, then you have to check whether the sample point 
                        {
                            // Get top point (if sample point is at the top of template point)
                            if (intY2 > intY1)
                            {
                                if ((intTopY == -1) || (intTopY < intY2))
                                {
                                    intTopY = intY1;
                                    intTopIndex = j;
                                }
                            }
                            // Get bottom point (if sample point is at the bottom of template point)
                            else
                            {
                                if ((intBottomY == -1) || (intBottomY > intY2))
                                {
                                    intBottomY = intY1;
                                    intBottomIndex = j;
                                }
                            }
                        }

                        if (intY1 == intY2)
                        {
                            // Get left point(if sample point is at the left of template point)
                            if (intX2 > intX1)
                            {
                                if ((intLeftX == -1) || (intLeftX < intX2))
                                {
                                    intLeftX = intX1;
                                    intLeftIndex = j;
                                }

                            }
                            // Get right point(if sample point is at the right of template point)
                            else
                            {
                                if ((intRightX == -1) || (intRightX > intX2))
                                {
                                    intRightX = intX1;
                                    intRightIndex = j;
                                }
                            }
                        }

                        if ((intTopIndex != -1) && (intRightIndex != -1) && (intBottomIndex != -1) && (intLeftIndex != -1))
                        {
                            intMatchList.Add(intCheckList[0]);
                            blnFound = true;
                            break;
                        }
                    }

                    if (blnFound)
                        break;
                }
            }

            return intMatchList;
        }


        public List<int> MatchObject(EBW8PathVector pv1, int[] intCheckList, int intSamplingStep)
        {
            EBW8PathVector pv2;
            List<int> intMatchList = new List<int>();
            int intNumElements1 = (int)pv1.NumElements;
            int intNumElements2;
            int intNumContour = m_pvContourList.Count;
            int intTopX, intTopY, intBottomX, intBottomY, intLeftX, intLeftY, intRightX, intRightY;
            int intTopIndex, intBottomIndex, intLeftIndex, intRightIndex;
            int i, j, c, m;
            int intX1, intY1, intX2, intY2;

            // Scan all contour points (From sample)
            for (i = 0; i < intNumElements1; i += intSamplingStep)
            {
                // Get contour point position
                intX1 = pv1.GetElement(i).X;
                intY1 = pv1.GetElement(i).Y;

                //System.Threading.Thread.Sleep(1);
                // This function consider done if MatchList == CheckList (MatchList will not higher than CheckList)
                if (intMatchList.Count == intCheckList.Length)
                    break;

                for (c = 0; c < intCheckList.Length; c++)
                {
                    // This function consider done if MatchList == CheckList (MatchList will not higher than CheckList)
                    if (intMatchList.Count == intCheckList.Length)
                        break;

                    if ((intCheckList[c] < 0) || (intCheckList[c] >= m_pvContourList.Count))
                        continue;

                    for (m = 0; m < intMatchList.Count; m++)
                    {
                        if (intMatchList[m] == intCheckList[c])
                            break;
                    }
                    if (m < intMatchList.Count)
                        continue;

                    pv2 = m_pvContourList[intCheckList[c]];
                    intNumElements2 = (int)pv2.NumElements;

                    intTopIndex = intBottomIndex = intLeftIndex = intRightIndex = -1;
                    intTopX = intBottomX = intX1;
                    intRightX = intLeftX = -1;
                    intTopY = intBottomY = -1;
                    intRightY = intLeftY = intY1;

                    bool blnFound = false;
                    // Scan all template contour point 
                    for (j = 0; j < intNumElements2; j++)
                    {
                        intX2 = pv2.GetElement(j).X;
                        intY2 = pv2.GetElement(j).Y;

                        // Template vector point and sample vector point are same
                        if ((intX1 == intX2) &&
                           (intY1 == intY2))
                        {
                            intMatchList.Add(intCheckList[c]);
                            blnFound = true;
                            break;
                        }

                        if (intX1 == intX2) // if both x are sample point, then you have to check whether the sample point 
                        {
                            // Get top point (if template point is at the top of sample point)
                            if (intY2 < intY1)
                            {
                                if ((intTopY == -1) || (intTopY > intY2))
                                {
                                    intTopY = intY2;
                                    intTopIndex = j;
                                }
                            }
                            // Get bottom point (if template point is at the bottom of sample point)
                            else
                            {
                                if ((intBottomY == -1) || (intBottomY < intY2))
                                {
                                    intBottomY = intY2;
                                    intBottomIndex = j;
                                }
                            }
                        }

                        if (intY1 == intY2)
                        {
                            // Get left point
                            if (intX2 < intX1)
                            {
                                if ((intLeftX == -1) || (intLeftX > intX2))
                                {
                                    intLeftX = intX2;
                                    intLeftIndex = j;
                                }

                            }
                            // Get right point
                            else
                            {
                                if ((intRightX == -1) || (intRightX < intX2))
                                {
                                    intRightX = intX2;
                                    intRightIndex = j;
                                }
                            }
                        }
                    }

                    if (blnFound)
                        continue;

                    if ((intTopIndex != -1) && (intRightIndex != -1) && (intBottomIndex != -1) && (intLeftIndex != -1))
                        if (((m_intPathDirection[intCheckList[c]][intTopIndex] & 0x08) > 0) &&
                            ((m_intPathDirection[intCheckList[c]][intRightIndex] & 0x01) > 0) &&
                            ((m_intPathDirection[intCheckList[c]][intBottomIndex] & 0x04) > 0) &&
                            ((m_intPathDirection[intCheckList[c]][intLeftIndex] & 0x02) > 0))
                        {
                            intMatchList.Add(intCheckList[c]);
                        }
                }
            }

            return intMatchList;
        }

        public List<int> MatchObject(EBW8PathVector pv1, int intSamplingStep)
        {
            EBW8PathVector pv2;
            List<int> intMatchList = new List<int>();
            int intNumElements1 = (int)pv1.NumElements;
            int intNumElements2;
            int intNumContour = m_pvContourList.Count;
            int intTopX, intTopY, intBottomX, intBottomY, intLeftX, intLeftY, intRightX, intRightY;
            int intTopIndex, intBottomIndex, intLeftIndex, intRightIndex;
            int i, j, c, m;
            int intX1, intY1, intX2, intY2;

            for (i = 0; i < intNumElements1; i += intSamplingStep)
            {
                intX1 = pv1.GetElement(i).X;
                intY1 = pv1.GetElement(i).Y;

                for (c = 0; c < intNumContour; c++)
                {
                    for (m = 0; m < intMatchList.Count; m++)
                    {
                        if (intMatchList[m] == c)
                            break;
                    }
                    if (m < intMatchList.Count)
                        continue;

                    pv2 = m_pvContourList[c];
                    intNumElements2 = (int)pv2.NumElements;

                    intTopIndex = intBottomIndex = intLeftIndex = intRightIndex = -1;
                    intTopX = intBottomX = intX1;
                    intRightX = intLeftX = -1;
                    intTopY = intBottomY = -1;
                    intRightY = intLeftY = intY1;

                    for (j = 0; j < intNumElements2; j++)
                    {
                        intX2 = pv2.GetElement(j).X;
                        intY2 = pv2.GetElement(j).Y;

                        if ((intX1 == intX2) &&
                           (intY1 == intY2))
                        {
                            intMatchList.Add(c);
                            break;
                        }

                        if (intX1 == intX2)
                        {
                            // Get top point
                            if (intY2 < intY1)
                            {
                                if ((intTopY == -1) || (intTopY > intY2))
                                {
                                    intTopY = intY2;
                                    intTopIndex = j;
                                }
                            }
                            // Get bottom point
                            else
                            {
                                if ((intBottomY == -1) || (intBottomY < intY2))
                                {
                                    intBottomY = intY2;
                                    intBottomIndex = j;
                                }
                            }
                        }

                        if (intY1 == intY2)
                        {
                            // Get left point
                            if (intX2 < intX1)
                            {
                                if ((intLeftX == -1) || (intLeftX > intX2))
                                {
                                    intLeftX = intX2;
                                    intLeftIndex = j;
                                }

                            }
                            // Get right point
                            else
                            {
                                if ((intRightX == -1) || (intRightX < intX2))
                                {
                                    intRightX = intX2;
                                    intRightIndex = j;
                                }
                            }
                        }
                    }

                    if ((intTopIndex != -1) && (intRightIndex != -1) && (intBottomIndex != -1) && (intLeftIndex != -1))
                        if (((m_intPathDirection[c][intTopIndex] & 0x08) > 0) &&
                            ((m_intPathDirection[c][intRightIndex] & 0x01) > 0) &&
                            ((m_intPathDirection[c][intBottomIndex] & 0x04) > 0) &&
                            ((m_intPathDirection[c][intLeftIndex] & 0x02) > 0))
                        {
                            intMatchList.Add(c);
                        }
                }
            }

            return intMatchList;
        }

        public bool BuildContour(ROI objROI, int intStartX, int intStartY, int intThreshold, int intConnexity)
        {
            if (intStartX >= objROI.ref_ROIWidth || intStartY >= objROI.ref_ROIHeight)
                return false;

            EBW8PathVector objPathVector = new EBW8PathVector();

            EConnexity connexity;
            if (intConnexity == 4)
                connexity = EConnexity.Connexity4;
            else
                connexity = EConnexity.Connexity8;

            int intAutoThreshold;
            if (intThreshold == -4)
                intAutoThreshold = ROI.GetAutoThresholdValue(objROI, 3);
            else
                intAutoThreshold = intThreshold;

            intAutoThreshold = Math.Min(intAutoThreshold, objROI.ref_ROI.GetPixel(intStartX, intStartY).Value);

            try
            {
#if (Debug_2_12 || Release_2_12)
                // Use ContourMode ClockwiseAlwaysClosed to get stable contour
                EasyImage.Contour(objROI.ref_ROI, EContourMode.ClockwiseAlwaysClosed, intStartX, intStartY,
                                  EContourThreshold.Above, (uint)intAutoThreshold, connexity, objPathVector);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Use ContourMode ClockwiseAlwaysClosed to get stable contour
                EasyImage.Contour(objROI.ref_ROI, EContourMode.ClockwiseAlwaysClosed, intStartX, intStartY,
                               EContourThreshold.Above, intAutoThreshold, connexity, objPathVector);
                //EasyImage.Contour(objROI.ref_ROI, EContourMode.Anticlockwise, intStartX + 1, intStartY,
                //                  EContourThreshold.Above, 255, connexity, objPathVector);
#endif

                m_pvContourList.Add(objPathVector);

                m_intPathDirection.Add(new List<int>());
                int intPathIndex = m_intPathDirection.Count - 1;
                int intNumElements = (int)objPathVector.NumElements;
                int intOrgX, intOrgY, intX, intY, intDirection;

                // -------------------- 2018 10 03 - CCENG: Consume time. Try improve it in future. -------------------------------------
                // Scan all contour points
                int intSampling = Math.Max(1, intNumElements / 30);         // 2019 07 11 - CCENG: use sampling for faster process. use 30 because it is good enough to check all direction. The 30 may change to settingable if need to change next time.
                for (int i = 0; i < intNumElements; i += intSampling)
                //for (int i = 0; i < intNumElements; i++)
                {
                    intDirection = 0;
                    intOrgX = objPathVector.GetElement(i).X;
                    intOrgY = objPathVector.GetElement(i).Y;

                    // point-x
                    intX = intOrgX - 1;
                    intY = intOrgY;
                    if (intX < 0)
                        intX = 0;
                    if (objROI.ref_ROI.GetPixel(intX, intY).Value >= intAutoThreshold)
                        intDirection |= 0x01;

                    // point-X
                    intX = intOrgX + 1;
                    if (intX >= objROI.ref_ROIWidth)
                        intX = objROI.ref_ROIWidth - 1;
                    if (objROI.ref_ROI.GetPixel(intX, intY).Value >= intAutoThreshold)
                        intDirection |= 0x02;

                    // point-y
                    intX = intOrgX;
                    intY = intOrgY - 1;
                    if (intY < 0)
                        intY = 0;
                    if (objROI.ref_ROI.GetPixel(intX, intY).Value >= intAutoThreshold)
                        intDirection |= 0x04;

                    // point-Y
                    intY = intOrgY + 1;
                    if (intY >= objROI.ref_ROIHeight)
                        intY = objROI.ref_ROIHeight - 1;
                    if (objROI.ref_ROI.GetPixel(intX, intY).Value >= intAutoThreshold)
                        intDirection |= 0x08;

                    m_intPathDirection[intPathIndex].Add(intDirection);
                }
                // ----------------------------------------------------------------------------------------------
            }
            catch (Exception ex)
            {
                m_pvContourList.Add(objPathVector);
                m_intPathDirection.Add(new List<int>());

                return false;
            }

            return true;
        }

        public bool BuildBasicContour(ROI objROI, int intStartX, int intStartY, int intThreshold, int intConnexity)
        {
            EBW8PathVector objPathVector = new EBW8PathVector();

            EConnexity connexity;
            if (intConnexity == 4)
                connexity = EConnexity.Connexity4;
            else
                connexity = EConnexity.Connexity8;

            int intAutoThreshold;
            if (intThreshold == -4)
                intAutoThreshold = ROI.GetAutoThresholdValue(objROI, 3);
            else
                intAutoThreshold = intThreshold;

            intAutoThreshold = Math.Min(intAutoThreshold, objROI.ref_ROI.GetPixel(intStartX, intStartY).Value);

            try
            {
#if (Debug_2_12 || Release_2_12)
                // Use ContourMode ClockwiseAlwaysClosed to get stable contour
                EasyImage.Contour(objROI.ref_ROI, EContourMode.ClockwiseAlwaysClosed, intStartX, intStartY,
                                  EContourThreshold.Above, (uint)intAutoThreshold, connexity, objPathVector);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                // Use ContourMode ClockwiseAlwaysClosed to get stable contour
                EasyImage.Contour(objROI.ref_ROI, EContourMode.ClockwiseAlwaysClosed, intStartX, intStartY,
                                  EContourThreshold.Above, intAutoThreshold, connexity, objPathVector);
#endif

                m_pvContourList.Add(objPathVector);
            }
            catch
            {
                m_pvContourList.Add(objPathVector);
                return false;
            }

            return true;
        }

        public bool Get4DirectionPoints(ROI objROI, ref Point pTop, ref Point pBottom, ref Point pLeft, ref Point pRight)
        {
            if (m_pvContourList.Count == 0)
                return false;

            int intTotalCount = (int)m_pvContourList[0].NumElements;

            int intOuter_YT = int.MaxValue, intOuter_YB = int.MinValue, intOuter_XL = int.MaxValue, intOuter_XR = int.MinValue;
            int intHighestPX_L = -1, intHighestPX_L1 = -1;
            int intHighestPX_R = -1, intHighestPX_R1 = -1;
            int intHighestPX_T = -1, intHighestPX_T1 = -1;
            int intHighestPX_B = -1, intHighestPX_B1 = -11;
            int intX, intY;
            for (int i = 0; i < intTotalCount; i++)
            {
                intX = m_pvContourList[0].GetElement(i).X;
                intY = m_pvContourList[0].GetElement(i).Y;
                EBW8 px = objROI.ref_ROI.GetPixel(intX, intY);

                // ---------- Top ---------------------------
                if (intY < intOuter_YT)
                {
                    intOuter_YT = intY;
                    intHighestPX_T = px.Value;
                    intHighestPX_T1 = -1;
                    pTop.X = intX;
                    pTop.Y = intY;
                }
                else if (intY == intOuter_YT)
                {
                    if (px.Value > intHighestPX_T)
                    {
                        intHighestPX_T = px.Value;
                        intHighestPX_T1 = -1;
                        pTop.X = intX;
                        pTop.Y = intY;
                    }
                    else if (px.Value == intHighestPX_T)
                    {
                        if (px.Value > intHighestPX_T1)
                        {
                            intHighestPX_T1 = px.Value;
                            pTop.X = intX;
                            pTop.Y = intY;
                        }
                    }
                }

                // ---------- Bottom ---------------------------
                if (intY > intOuter_YB)
                {
                    intOuter_YB = intY;
                    intHighestPX_B = px.Value;
                    intHighestPX_B1 = -1;
                    pBottom.X = intX;
                    pBottom.Y = intY;
                }
                else if (intY == intOuter_YB)
                {
                    if (px.Value > intHighestPX_B)
                    {
                        intHighestPX_B = px.Value;
                        intHighestPX_B1 = -1;
                        pBottom.X = intX;
                        pBottom.Y = intY;
                    }
                    else if (px.Value == intHighestPX_B)
                    {
                        if (px.Value > intHighestPX_B1)
                        {
                            intHighestPX_B1 = px.Value;
                            pBottom.X = intX;
                            pBottom.Y = intY;
                        }
                    }
                }

                // ---------- Left ---------------------------
                if (intX < intOuter_XL)
                {
                    intOuter_XL = intX;
                    intHighestPX_L = px.Value;
                    intHighestPX_L1 = -1;
                    pLeft.X = intX;
                    pLeft.Y = intY;
                }
                else if (intX == intOuter_XL)
                {
                    if (px.Value > intHighestPX_L)
                    {
                        intHighestPX_L = px.Value;
                        intHighestPX_L1 = -1;
                        pLeft.X = intX;
                        pLeft.Y = intY;
                    }
                    else if (px.Value == intHighestPX_L)
                    {
                        if (px.Value > intHighestPX_L1)
                        {
                            intHighestPX_L1 = px.Value;
                            pLeft.X = intX;
                            pLeft.Y = intY;
                        }
                    }
                }

                // ---------- Right ---------------------------
                if (intX > intOuter_XR)
                {
                    intOuter_XR = intX;
                    intHighestPX_R = px.Value;
                    intHighestPX_R1 = -1;
                    pRight.X = intX;
                    pRight.Y = intY;
                }
                else if (intX == intOuter_XR)
                {
                    if (px.Value > intHighestPX_R)
                    {
                        intHighestPX_R = px.Value;
                        intHighestPX_R1 = -1;
                        pRight.X = intX;
                        pRight.Y = intY;
                    }
                    else if (px.Value == intHighestPX_R)
                    {
                        if (px.Value > intHighestPX_R1)
                        {
                            intHighestPX_R1 = px.Value;
                            pRight.X = intX;
                            pRight.Y = intY;
                        }
                    }
                }

            }

            return true;
        }
        public void SaveBlobsContour(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);

            int i, j, intNumElements;
            for (i = 0; i < m_pvContourList.Count; i++)
            {
                objFile.WriteSectionElement("PathVector" + i);

                intNumElements = (int)m_pvContourList[i].NumElements;
                for (j = 0; j < intNumElements; j++)
                {
                    int intSamplingStep = 10;
                    if(j + intSamplingStep < intNumElements)
                    objFile.WriteElement1Value("Path" + j, m_pvContourList[i].GetElement(j).X + ":" + m_pvContourList[i].GetElement(j).Y+
                        ":"+Math2.GetAngle(new PointF(m_pvContourList[i].GetElement(j).X, m_pvContourList[i].GetElement(j).Y), new PointF(m_pvContourList[i].GetElement(j + intSamplingStep).X, m_pvContourList[i].GetElement(j + intSamplingStep).Y)));
                }
            }
            objFile.WriteEndElement();
        }

        public void LoadBlobsContour(string strPath)
        {
            XmlParser objFile = new XmlParser(strPath);

            m_pvContourList.Clear();
            int i, j, intNumElements;
            string[] strXY;
            EBW8Path bw8Path = new EBW8Path();
            int intPathVectorCount = objFile.GetFirstSectionCount();

            for (i = 0; i < intPathVectorCount; i++)
            {
                objFile.GetFirstSection("PathVector" + i);
                m_pvContourList.Add(new EBW8PathVector());

                intNumElements = objFile.GetSecondSectionCount();
                for (j = 0; j < intNumElements; j++)
                {
                    strXY = objFile.GetValueAsString("Path" + j, "0_0", 2).Split('_');
                    bw8Path.X = Convert.ToInt16(strXY[0]);
                    bw8Path.Y = Convert.ToInt16(strXY[1]);
                    m_pvContourList[i].AddElement(bw8Path);
                }
            }
        }

        public void DefineDeepEdge(int intSamplingStep)
        {
            m_arrLine.Clear();
            for (int i = 0; i < m_pvContourList.Count; i++)
            {
                m_arrLine.Add(new List<Line>());
                Line line = new Line();
                EBW8PathVector pv = m_pvContourList[i];
                int intNumElements = (int)pv.NumElements;
                int k = 0;
                for (int m = intSamplingStep; m < intNumElements; m += intSamplingStep)
                {
                    line.CalculateStraightLine(new PointF(pv.GetElement(k).X, pv.GetElement(k).Y),
                                               new PointF(pv.GetElement(m).X, pv.GetElement(m).Y));
                    k = m;
                }
            }
        }

        public void Dispose()
        {
            for (int i = 0; i < m_pvContourList.Count; i++)
            {
                if (m_pvContourList[i] != null)
                    m_pvContourList[i].Dispose();
            }
        }
    }
}
