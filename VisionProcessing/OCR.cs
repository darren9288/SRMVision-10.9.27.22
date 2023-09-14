using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;

namespace VisionProcessing
{
    public class OCR
    {
        #region Constant Variables
        private bool USE_EVISION_OCR = false;
        #endregion

        #region Member Variables
        private int m_intMaxPosition = 0;
        private float[] m_arrCharScore;
        private string m_strErrorMessage = "";
        private string m_strRecognizeString = "";
        private List<int> m_arrSpaceIndex = new List<int>();
        private List<int> m_arrRefCharCounter = new List<int>();
        private List<EImageBW8> m_arrPatternImage = new List<EImageBW8>();    // Keep template char image
        private List<EMatcher> m_arrMatcher = new List<EMatcher>();           // Keep template char matcher
        private List<RecogChar> m_arrRecogChar = new List<RecogChar>();
        private List<TemplateChar> m_arrTemplateChar = new List<TemplateChar>();    // Keep template char information
        private EMatcher m_objTextMatcher = new EMatcher();
        private EOCR m_Ocr = new EOCR();
        private Size m_patternSmallestSize = new Size(int.MaxValue, int.MaxValue);
        private List<Pattern> m_arrPattern = new List<Pattern>();

        class Pattern
        {
            public EImageBW8 objImage = new EImageBW8();
            public EMatcher objMatcher = new EMatcher();
            public int intCode;
            public int intClass;
        }

        struct RecogChar
        {
            public int intCode;
            public int intOrgX;
            public int intOrgY;
            public int intEndX;
            public int intEndY;
            public int intWidth;
            public int intHeight;
            public int intPatternIndex;
            public float fScore;
        }

        struct TemplateChar
        {
            public int intCode;
            public int intOrgX;
            public int intOrgY;
            public int intEndX;
            public int intEndY;
            public int intWidth;
            public int intHeight;
            public int intOffSetX;
            public int intOffSetY;
            public int intPatternIndex;
        }

        #endregion

        #region Properties
        public int ref_intMaxPosition { get { return m_intMaxPosition; } set { m_intMaxPosition = value; } }
        public int ref_MaxCharHeight { get { return m_Ocr.MaxCharHeight; } }
        public int ref_MaxCharWidth { get { return m_Ocr.MaxCharWidth; } }
        public int ref_MinCharHeight { get { return m_Ocr.MinCharHeight; } }
        public int ref_MinCharWidth { get { return m_Ocr.MinCharWidth; } }
        public int ref_NumChars { get 
                                    {
                                        if (USE_EVISION_OCR)
                                            return m_Ocr.NumChars;
                                        else
                                            return m_arrRecogChar.Count;
                                    }
                                }
        public int ref_NumTemplateChars
        {
            get {   return m_arrTemplateChar.Count;    }
        }
        public int ref_NumPatterns { get { return m_arrPattern.Count; } }
        public string ref_strErrorMessage { get { return m_strErrorMessage; } }
        public string ref_strRecognizeString { get { return m_strRecognizeString; } }
        public Size ref_patternSmallestSize { get { return m_patternSmallestSize; } }
        #endregion

        public OCR()
        {
        }


        public bool BuildBlobChars(ROI objROI, bool blnRemoveBorder, bool blnWhiteOnBlack, bool blnConnexity4, int intThreshold, int intMinArea, int intMaxArea)
        {
            // Make sure ROI image exist
            if (objROI.ref_ROIWidth == 0 || objROI.ref_ROIHeight == 0)
            {
                m_strErrorMessage = "ROI size is zero!";
                return false;
            }

            // Build blobs
            Blobs objBlobs = new Blobs();
            objBlobs.ref_intThreshold = intThreshold;
            if (blnConnexity4)
                objBlobs.SetConnexity(4);
            else
                objBlobs.SetConnexity(8);
            if (blnWhiteOnBlack)
                objBlobs.SetClassSelection(2);
            else
                objBlobs.SetClassSelection(1);
            objBlobs.SetObjectAreaRange(intMinArea, intMaxArea);
            objBlobs.BuildObjects(objROI, blnRemoveBorder);

            int intNumSelectedObject = objBlobs.ref_intNumSelectedObject;
            if (intNumSelectedObject == 0)
                return false;

            TemplateChar objTemplateChar;
            m_arrTemplateChar.Clear();
            float fTextStartPointX = float.MaxValue, fTextStartPointY = float.MaxValue, fTextEndPointX = float.MinValue, fTextEndPointY = float.MinValue;
            float fCenterX, fCenterY, fWidth, fHeight, fStartX, fStartY, fEndX, fEndY;
            fCenterX = fCenterY = fWidth = fHeight = 0;
            // Scan all blobs to get data
            objBlobs.SetFirstListBlobs();
            for (int i = 0; i < intNumSelectedObject; i++)
            {
                objBlobs.GetSelectedListBlobsLimitCenterX(ref fCenterX);
                objBlobs.GetSelectedListBlobsLimitCenterY(ref fCenterY);
                objBlobs.GetSelectedListBlobsWidth(ref fWidth);
                objBlobs.GetSelectedListBlobsHeight(ref fHeight);

                fStartX = fCenterX - fWidth / 2;
                fStartY = fCenterY - fHeight / 2;
                fEndX = fCenterX + fWidth / 2;
                fEndY = fCenterY + fHeight / 2;

                // Set blobs data to TemplateChar struct
                objTemplateChar = new TemplateChar();
                objTemplateChar.intOrgX = (int)fStartX;
                objTemplateChar.intOrgY = (int)fStartY;
                objTemplateChar.intEndX = (int)fEndX;
                objTemplateChar.intEndY = (int)fEndY;
                objTemplateChar.intWidth = (int)fWidth;
                objTemplateChar.intHeight = (int)fHeight;
                m_arrTemplateChar.Add(objTemplateChar);

                // Get Text ROI Start and End Point
                if (fStartX < fTextStartPointX)
                    fTextStartPointX = fStartX;
                if (fStartY < fTextStartPointY)
                    fTextStartPointY = fStartY;
                if (fEndX > fTextEndPointX)
                    fTextEndPointX = fEndX;
                if (fEndY > fTextEndPointY)
                    fTextEndPointY = fEndY;

                objBlobs.SetListBlobsToNext();
            }

            // Get Text ROI and Learn the Text ROI image for PRS used
            ROI objTextROI = new ROI();
            objTextROI.LoadROISetting((int)fTextStartPointX, (int)fTextStartPointY,
                                      (int)(fTextEndPointX - fTextStartPointX),
                                      (int)(fTextEndPointY - fTextStartPointY));
            objTextROI.AttachImage(objROI);
#if (Debug_2_12 || Release_2_12)
            m_objTextMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
            m_objTextMatcher.LearnPattern(objTextROI.ref_ROI);

            // Calculate each Template Char offset value from Text ROI start point
            for (int i = 0; i < m_arrTemplateChar.Count; i++)
            {
                objTemplateChar = m_arrTemplateChar[i];
                objTemplateChar.intOffSetX = objTemplateChar.intOrgX + objTemplateChar.intWidth / 2 - (int)fTextStartPointX;
                objTemplateChar.intOffSetY = objTemplateChar.intOrgY + objTemplateChar.intHeight / 2 - (int)fTextStartPointY;

                m_arrTemplateChar[i] = objTemplateChar;
            }

            return true;
        }

        public bool BuildChars(ImageDrawing objImage, ROI objROI)
        {
            ROI objOCRROI = new ROI();
            objOCRROI.AttachImage(objImage);
            objOCRROI.LoadROISetting(objROI.ref_ROI.Parent.OrgX + objROI.ref_ROI.OrgX, objROI.ref_ROI.Parent.OrgY + objROI.ref_ROI.OrgY,
                                     objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            m_Ocr.BuildObjects(objOCRROI.ref_ROI);
            m_Ocr.FindAllChars(objOCRROI.ref_ROI);

            if (m_Ocr.NumChars == 0)
            {
                m_strErrorMessage = "No charactes are builded!";
                return false;
            }

            return true;
        }

        public bool IsInOCRCharArea(Contour objContour, int intCharNo)
        {
            if (objContour.MatchObject(0, m_arrRecogChar[intCharNo].intOrgX, m_arrRecogChar[intCharNo].intOrgY,
               m_arrRecogChar[intCharNo].intEndX, m_arrRecogChar[intCharNo].intEndY, 4))
            {
                return true;
            }

            return false;
        }

        public float GetCharScore(int intCharIndex)
        {
            if (USE_EVISION_OCR)
                return m_arrCharScore[intCharIndex];
            else
                return m_arrRecogChar[intCharIndex].fScore;
        }

        public float GetConfidenceLevelScore(int intCharIndex)
        {
            return m_Ocr.GetConfidenceRatio(intCharIndex);
        }

        public float GetFirstLevelCharErr(int intCharIndex)
        {
            return m_Ocr.GetFirstCharDistance(intCharIndex);
        }

        public float GetSecondLevelCharErr(int intCharIndex)
        {
            return m_Ocr.GetSecondCharDistance(intCharIndex);
        }

        public char GetFirstLevelChar(int intCharIndex)
        {
            if (USE_EVISION_OCR)
                return Convert.ToChar(m_Ocr.GetFirstCharCode(intCharIndex));
            else
                return Convert.ToChar(m_arrRecogChar[intCharIndex].intCode);
        }

        public char GetPattern(int intPatternIndex)
        {
            return Convert.ToChar(m_arrPattern[intPatternIndex].intCode);
        }

        public char GetSecondLevelChar(int intCharIndex)
        {
            return Convert.ToChar(m_Ocr.GetSecondCharCode(intCharIndex));
        }

        public int HitChars(int intX, int intY)
        {
#if (Debug_2_12 || Release_2_12)
            uint intCharIndex = 0;
            if (USE_EVISION_OCR)
            {
                m_Ocr.HitChars(intX, intY, out intCharIndex);
            }
            else
            {
                for (int i = 0; i < m_arrTemplateChar.Count; i++)
                {
                    if ((intX >= m_arrTemplateChar[i].intOrgX) && (intX <= m_arrTemplateChar[i].intEndX) &&
                        (intY >= m_arrTemplateChar[i].intOrgY) && (intY <= m_arrTemplateChar[i].intEndY))
                    {
                        intCharIndex = (uint)i;
                        break;
                    }
                }
                //for (int i = 0; i < m_arrRecogChar.Count; i++)
                //{
                //    if ((intX >= m_arrRecogChar[i].intOrgX) && (intX <= m_arrRecogChar[i].intEndX) &&
                //        (intY >= m_arrRecogChar[i].intOrgY) && (intY <= m_arrRecogChar[i].intEndY))
                //    {
                //        intCharIndex = i;
                //        break;
                //    }
                //}
            }
            return (int)intCharIndex;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            int intCharIndex = -1;
            if (USE_EVISION_OCR)
            {
                m_Ocr.HitChars(intX, intY, out intCharIndex);
            }
            else
            {
                for (int i = 0; i < m_arrTemplateChar.Count; i++)
                {
                    if ((intX >= m_arrTemplateChar[i].intOrgX) && (intX <= m_arrTemplateChar[i].intEndX) &&
                        (intY >= m_arrTemplateChar[i].intOrgY) && (intY <= m_arrTemplateChar[i].intEndY))
                    {
                        intCharIndex = i;
                        break;
                    }
                }
                //for (int i = 0; i < m_arrRecogChar.Count; i++)
                //{
                //    if ((intX >= m_arrRecogChar[i].intOrgX) && (intX <= m_arrRecogChar[i].intEndX) &&
                //        (intY >= m_arrRecogChar[i].intOrgY) && (intY <= m_arrRecogChar[i].intEndY))
                //    {
                //        intCharIndex = i;
                //        break;
                //    }
                //}
            }  
            return intCharIndex;         
#endif

        }

        public int GetPatternClass(int intPatternIndex)
        {
            int intClassValue = m_arrPattern[intPatternIndex].intClass;
            int intClass;
            switch (intClassValue)
            {
                case (int)EOCRClass.Digit:
                    intClass = 0;
                    break;
                case (int)EOCRClass.UpperCase:
                    intClass = 1;
                    break;
                case (int)EOCRClass.LowerCase:
                    intClass = 2;
                    break;
                default:
                    intClass = 3;
                    break;
            }

            return intClass;
        }

        public int[] GetMatchPatternIndexes(int intCharIndex)
        {
            List<int> arrMatchPatternIndex = new List<int>();
            char cCharCode;
            if (USE_EVISION_OCR)
                cCharCode = Convert.ToChar(m_Ocr.GetFirstCharCode(intCharIndex));
            else
                cCharCode = Convert.ToChar(m_arrRecogChar[intCharIndex].intCode);
            char cPatternCode;

            int intNumPatterns = m_arrPattern.Count;
            for (int i = 0; i < intNumPatterns; i++)
            {
                cPatternCode = Convert.ToChar(m_arrPattern[i].intCode);
                if (cCharCode == cPatternCode)
                    arrMatchPatternIndex.Add(i);
            }

            return arrMatchPatternIndex.ToArray();
        }

        public List<int> GetMatchCharIndexes(float fStartX, float fStartY, float fEndX, float fEndY)
        {
            List<int> arrMatchIndexes = new List<int>();
            int intNumChars;
            if (USE_EVISION_OCR)
                intNumChars = m_Ocr.NumChars;
            else
                intNumChars = m_arrRecogChar.Count;

            int intStartX, intStartY, intEndX, intEndY;
            intStartX = intStartY = intEndX = intEndY = 0;
            
            for (int i = 0; i < intNumChars; i++)
            {
                if (USE_EVISION_OCR)
                {
                    intStartX = m_Ocr.CharGetOrgX(i);
                    intStartY = m_Ocr.CharGetOrgY(i);
                    intEndX = intStartX + m_Ocr.CharGetWidth(i);
                    intEndY = intStartY + m_Ocr.CharGetHeight(i);
                }
                else
                {
                    intStartX = m_arrRecogChar[i].intOrgX;
                    intStartY = m_arrRecogChar[i].intOrgY;
                    intEndX = intStartX + m_arrRecogChar[i].intWidth;
                    intEndY = intStartY + m_arrRecogChar[i].intHeight;
                }

                if (((intStartX > fStartX) && (intStartX < fEndX) && (intStartY > fStartY) && (intStartY < fEndY)) ||
                    ((intStartX > fStartX) && (intStartX < fEndX) && (intEndY > fStartY) && (intEndY < fEndY)) ||
                    ((intEndX > fStartX) && (intEndX < fEndX) && (intStartY > fStartY) && (intStartY < fEndY)) ||
                    ((intEndX > fStartX) && (intEndX < fEndX) && (intEndY > fStartY) && (intEndY < fEndY)) ||
                    ((fStartX > intStartX) && (fStartX < intEndX) && (fStartY > intStartY) && (fStartY < intEndY)) ||
                    ((fStartX > intStartX) && (fStartX < intEndX) && (fEndY > intStartY) && (fEndY < intEndY)) ||
                    ((fEndX > intStartX) && (fEndX < intEndX) && (fStartY > intStartY) && (fStartY < intEndY)) ||
                    ((fEndX > intStartX) && (fEndX < intEndX) && (fEndY > intStartY) && (fEndY < intEndY)) ||
                    ((intStartX > fStartX) && (intStartX < fEndX) && (intStartY < fStartY) && (intEndY > fEndY)) ||
                    ((intStartY > fStartY) && (intStartY < fEndY) && (intStartX < fStartX) && (intEndX > fEndX)))
                    {
                        arrMatchIndexes.Add(i);
                    }
            }

            return arrMatchIndexes;
        }

        public unsafe Image GetPatternImage(int intPatternIndex)
        {
            Bitmap bm = new Bitmap(m_arrPattern[intPatternIndex].objImage.Width, m_arrPattern[intPatternIndex].objImage.Height);
            Graphics g = Graphics.FromImage(bm);

            m_arrPattern[intPatternIndex].objImage.Draw(g);
            return bm;
        }

        public ROI GetCharPatternROI(int intCharIndex)
        {
            ROI objROI = new ROI();
            objROI.ref_ROI.Detach();
            objROI.ref_ROI.Attach(m_arrPatternImage[m_arrRecogChar[intCharIndex].intPatternIndex]);
            objROI.LoadROISetting(0, 0,
                                m_arrPatternImage[m_arrRecogChar[intCharIndex].intPatternIndex].Width,
                                m_arrPatternImage[m_arrRecogChar[intCharIndex].intPatternIndex].Height);
            return objROI;
        }

        public string RecognizeCharsUsingOCR(ImageDrawing objImage, ROI objROI)
        {
            m_arrRecogChar.Clear();
            m_strRecognizeString = "";
            ROI objOCRROI = new ROI();
            objOCRROI.AttachImage(objImage);
            objOCRROI.LoadROISetting(objROI.ref_ROI.Parent.OrgX + objROI.ref_ROI.OrgX, objROI.ref_ROI.Parent.OrgY + objROI.ref_ROI.OrgY,
                                     objROI.ref_ROIWidth, objROI.ref_ROIHeight);

            m_Ocr.ShiftXTolerance = 0;
            m_Ocr.ShiftYTolerance = 0;
            m_Ocr.BuildObjects(objOCRROI.ref_ROI);
            m_Ocr.FindAllChars(objOCRROI.ref_ROI);
            
            try
            {
                //m_Ocr.Recognize((objOCRROI.ref_ROI, 256, (int)EOCRClass.Digit | (int)EOCRClass.UpperCase | (int)EOCRClass.LowerCase | (int)EOCRClass.Special, ref m_strRecognizeString);
            }
            catch
            {
                return "";
            }

            int[] intPatternIndex;
            int intNumChars = m_Ocr.NumChars;
            m_arrCharScore = new float[intNumChars];
            int j, intStartX, intStartY, intWidth, intHeight;
            float fMatchScore;
            ROI objCharROI = new ROI();
            objCharROI.AttachImage(objOCRROI);
            for (int i = 0; i < intNumChars; i++)
            {
                m_arrCharScore[i] = m_Ocr.GetConfidenceRatio(i);
                intPatternIndex = GetMatchPatternIndexes(i);

                for (j = 0; j < intPatternIndex.Length; j++)
                {
                    // Make sure matcher is exist
                    if (intPatternIndex[j] >= m_arrMatcher.Count)
                        continue;

                    intStartX = m_Ocr.CharGetOrgX(i) - 3; 
                    intStartY = m_Ocr.CharGetOrgY(i) - 3;
                    intWidth = m_Ocr.CharGetWidth(i) + 6;
                    intHeight = m_Ocr.CharGetHeight(i) + 6;
                    if (intStartX < 0)
                        intStartX = 0;
                    if (intStartY < 0)
                        intStartY = 0;
                    if ((intStartX + intWidth) > objOCRROI.ref_ROIWidth)
                        intWidth = intStartX + intWidth - objOCRROI.ref_ROIWidth;
                    if ((intStartY + intHeight) > objOCRROI.ref_ROIHeight)
                        intHeight = intStartY + intHeight - objOCRROI.ref_ROIHeight;
                    objCharROI.LoadROISetting(intStartX, intStartY, intWidth, intHeight);
                    m_arrMatcher[intPatternIndex[j]].Match(objCharROI.ref_ROI);

                    if (m_arrMatcher[intPatternIndex[j]].NumPositions == 0)
                        continue;

                    fMatchScore = m_arrMatcher[intPatternIndex[j]].GetPosition(0).Score * 100;
                    if (fMatchScore > m_arrCharScore[i])
                        m_arrCharScore[i] = fMatchScore;
                }
            }

            return m_strRecognizeString;
        }

        public string RecognizeCharsOverallArea(ImageDrawing objImage, ROI objROI)
        {
            /*
             * 
             */
            m_Ocr.EmptyChars();
            m_arrRecogChar.Clear();
            m_arrSpaceIndex.Clear();
            m_strRecognizeString = "";
            int intStartX1, intStartY1, intEndX1, intEndY1;
            int intStartX2, intStartY2, intEndX2, intEndY2;
            float fCurrentScore;
            int intDeleteState;
            bool blnOverlap;
            RecogChar objRecogChar;

            for (int i = 0; i < m_arrMatcher.Count; i++)
            {
#if (Debug_2_12 || Release_2_12)
                m_arrMatcher[i].MaxPositions = (uint)m_intMaxPosition;
                m_arrMatcher[i].MinScore = 0.5f;
                m_arrMatcher[i].Match(objROI.ref_ROI);
                int intNumPositions = (int)m_arrMatcher[i].NumPositions;

                if (intNumPositions == 0)
                    continue;

                for (uint j = 0; j < intNumPositions; j++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                m_arrMatcher[i].MaxPositions = m_intMaxPosition;
                m_arrMatcher[i].MinScore = 0.5f;
                m_arrMatcher[i].Match(objROI.ref_ROI);
                int intNumPositions = m_arrMatcher[i].NumPositions;
                
                if (intNumPositions == 0)
                    continue;

                for (int j = 0; j < intNumPositions; j++)
#endif

                {
                    intStartX1 = Convert.ToInt32(m_arrMatcher[i].GetPosition(j).CenterX - m_arrMatcher[i].PatternWidth / 2) + 5;
                    intStartY1 = Convert.ToInt32(m_arrMatcher[i].GetPosition(j).CenterY - m_arrMatcher[i].PatternHeight / 2) + 5;
                    intEndX1 = Convert.ToInt32(m_arrMatcher[i].GetPosition(j).CenterX + m_arrMatcher[i].PatternWidth / 2) - 5;
                    intEndY1 = Convert.ToInt32(m_arrMatcher[i].GetPosition(j).CenterY + m_arrMatcher[i].PatternHeight / 2) - 5;
                    fCurrentScore = m_arrMatcher[i].GetPosition(j).Score * 100;

                    intDeleteState = 0;
                    blnOverlap = false;

                    // Check overlap and get max score if overlap
                    for (int k = 0; k < m_arrRecogChar.Count; k++)
                    {
                        intStartX2 = m_arrRecogChar[k].intOrgX;
                        intStartY2 = m_arrRecogChar[k].intOrgY;
                        intEndX2 = m_arrRecogChar[k].intEndX;
                        intEndY2 = m_arrRecogChar[k].intEndY;

                        if (((intStartX1 >= intStartX2) && (intStartX1 <= intEndX2) && (intStartY1 >= intStartY2) && (intStartY1 <= intEndY2)) ||
                            ((intStartX1 >= intStartX2) && (intStartX1 <= intEndX2) && (intEndY1 >= intStartY2) && (intEndY1 <= intEndY2)) ||
                            ((intEndX1 >= intStartX2) && (intEndX1 <= intEndX2) && (intStartY1 >= intStartY2) && (intStartY1 <= intEndY2)) ||
                            ((intEndX1 >= intStartX2) && (intEndX1 <= intEndX2) && (intEndY1 >= intStartY2) && (intEndY1 <= intEndY2)) ||
                            ((intStartX2 >= intStartX1) && (intStartX2 <= intEndX1) && (intStartY2 >= intStartY1) && (intStartY2 <= intEndY1)) ||
                            ((intStartX2 >= intStartX1) && (intStartX2 <= intEndX1) && (intEndY2 >= intStartY1) && (intEndY2 <= intEndY1)) ||
                            ((intEndX2 >= intStartX1) && (intEndX2 <= intEndX1) && (intStartY2 >= intStartY1) && (intStartY2 <= intEndY1)) ||
                            ((intEndX2 >= intStartX1) && (intEndX2 <= intEndX1) && (intEndY2 >= intStartY1) && (intEndY2 <= intEndY1)) ||
                            ((intStartX1 > intStartX2) && (intStartX1 < intEndX2) && (intStartY1 < intStartY2) && (intEndY1 > intEndY2)) ||
                            ((intStartY1 > intStartY2) && (intStartY1 < intEndY2) && (intStartX1 < intStartX2) && (intEndX1 > intEndX2)))
                        {
                            blnOverlap = true;

                            // current score higher than recorded score
                            if (fCurrentScore > m_arrRecogChar[k].fScore)
                            {
                                m_arrRecogChar.RemoveAt(k);
                                k--;
                                if (intDeleteState != -1)
                                    intDeleteState = 1;
                            }
                            else
                            {
                                intDeleteState = -1;
                            }
                        }
                    }


                    if (!blnOverlap || (blnOverlap && intDeleteState == 1))
                    {
                        objRecogChar = new RecogChar();
                        objRecogChar.intCode = m_Ocr.GetPatternCode(i);
                        objRecogChar.intPatternIndex = i;
                        objRecogChar.intOrgX = intStartX1 - 5;
                        objRecogChar.intOrgY = intStartY1 - 5;
                        objRecogChar.intEndX = intEndX1 + 5;
                        objRecogChar.intEndY = intEndY1 + 5;
                        objRecogChar.intWidth = m_arrMatcher[i].PatternWidth;
                        objRecogChar.intHeight = m_arrMatcher[i].PatternHeight;
                        objRecogChar.fScore = fCurrentScore;
                        m_arrRecogChar.Add(objRecogChar);
                    }
                }
            }

            ShortRecogChars();

            // Get string
            for (int i = 0; i < m_arrRecogChar.Count; i++)
            {
                m_strRecognizeString += Convert.ToChar(m_arrRecogChar[i].intCode);

                for (int j = 0; j < m_arrSpaceIndex.Count; j++)
                {
                    if (m_arrSpaceIndex[j] == i)
                    {
                        m_strRecognizeString += " ";
                        break;
                    }
                }
            }
            return m_strRecognizeString;
        }

        public string RecognizeCharsTextArea(ImageDrawing objImage, ROI objROI, bool blnRecogPosition)
        {
            /*
             * 
             */
            m_arrRecogChar.Clear();
            m_arrSpaceIndex.Clear();
            m_strRecognizeString = "";
            int intStartX1, intStartY1, intEndX1, intEndY1;
            int intStartX2, intStartY2, intEndX2, intEndY2;
            float fCurrentScore;
            int intDeleteState;
            bool blnOverlap;
            RecogChar objRecogChar;

            for (int i = 0; i < m_arrPattern.Count; i++)
            {
                EMatcher objMatcher = m_arrPattern[i].objMatcher;
#if (Debug_2_12 || Release_2_12)
                objMatcher.MaxPositions = (uint)m_intMaxPosition;
                objMatcher.MinScore = 0.5f;
                objMatcher.Match(objROI.ref_ROI);
                int intNumPositions = (int)objMatcher.NumPositions;
                if (intNumPositions == 0)
                    continue;

                for (uint j = 0; j < intNumPositions; j++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                objMatcher.MaxPositions = m_intMaxPosition;
                objMatcher.MinScore = 0.5f;
                objMatcher.Match(objROI.ref_ROI);
                int intNumPositions = objMatcher.NumPositions;
                if (intNumPositions == 0)
                    continue;

                for (int j = 0; j < intNumPositions; j++)
#endif

                {
                    intStartX1 = Convert.ToInt32(objMatcher.GetPosition(j).CenterX - objMatcher.PatternWidth / 2) + 5;
                    intStartY1 = Convert.ToInt32(objMatcher.GetPosition(j).CenterY - objMatcher.PatternHeight / 2) + 5;
                    intEndX1 = Convert.ToInt32(objMatcher.GetPosition(j).CenterX + objMatcher.PatternWidth / 2) - 5;
                    intEndY1 = Convert.ToInt32(objMatcher.GetPosition(j).CenterY + objMatcher.PatternHeight / 2) - 5;
                    fCurrentScore = objMatcher.GetPosition(j).Score * 100;

                    intDeleteState = 0;
                    blnOverlap = false;

                    // Check overlap and get max score if overlap
                    for (int k = 0; k < m_arrRecogChar.Count; k++)
                    {
                        intStartX2 = m_arrRecogChar[k].intOrgX;
                        intStartY2 = m_arrRecogChar[k].intOrgY;
                        intEndX2 = m_arrRecogChar[k].intEndX;
                        intEndY2 = m_arrRecogChar[k].intEndY;

                        if (((intStartX1 >= intStartX2) && (intStartX1 <= intEndX2) && (intStartY1 >= intStartY2) && (intStartY1 <= intEndY2)) ||
                            ((intStartX1 >= intStartX2) && (intStartX1 <= intEndX2) && (intEndY1 >= intStartY2) && (intEndY1 <= intEndY2)) ||
                            ((intEndX1 >= intStartX2) && (intEndX1 <= intEndX2) && (intStartY1 >= intStartY2) && (intStartY1 <= intEndY2)) ||
                            ((intEndX1 >= intStartX2) && (intEndX1 <= intEndX2) && (intEndY1 >= intStartY2) && (intEndY1 <= intEndY2)) ||
                            ((intStartX2 >= intStartX1) && (intStartX2 <= intEndX1) && (intStartY2 >= intStartY1) && (intStartY2 <= intEndY1)) ||
                            ((intStartX2 >= intStartX1) && (intStartX2 <= intEndX1) && (intEndY2 >= intStartY1) && (intEndY2 <= intEndY1)) ||
                            ((intEndX2 >= intStartX1) && (intEndX2 <= intEndX1) && (intStartY2 >= intStartY1) && (intStartY2 <= intEndY1)) ||
                            ((intEndX2 >= intStartX1) && (intEndX2 <= intEndX1) && (intEndY2 >= intStartY1) && (intEndY2 <= intEndY1)) ||
                            ((intStartX1 > intStartX2) && (intStartX1 < intEndX2) && (intStartY1 < intStartY2) && (intEndY1 > intEndY2)) ||
                            ((intStartY1 > intStartY2) && (intStartY1 < intEndY2) && (intStartX1 < intStartX2) && (intEndX1 > intEndX2)))
                        {
                            blnOverlap = true;

                            // current score higher than recorded score
                            if (fCurrentScore > m_arrRecogChar[k].fScore)
                            {
                                m_arrRecogChar.RemoveAt(k);
                                k--;
                                if (intDeleteState != -1)
                                    intDeleteState = 1;
                            }
                            else
                            {
                                intDeleteState = -1;
                            }
                        }
                    }


                    if (!blnOverlap || (blnOverlap && intDeleteState == 1))
                    {
                        objRecogChar = new RecogChar();
                        objRecogChar.intCode = m_arrPattern[i].intCode;
                        objRecogChar.intPatternIndex = i;
                        objRecogChar.intOrgX = intStartX1 - 5;
                        objRecogChar.intOrgY = intStartY1 - 5;
                        objRecogChar.intEndX = intEndX1 + 5;
                        objRecogChar.intEndY = intEndY1 + 5;
                        objRecogChar.intWidth = objMatcher.PatternWidth;
                        objRecogChar.intHeight = objMatcher.PatternHeight;
                        objRecogChar.fScore = fCurrentScore;
                        m_arrRecogChar.Add(objRecogChar);
                    }
                }
            }

            if (blnRecogPosition)
                FillterRecogString(objROI);
            ShortRecogChars();

            // Get string
            for (int i = 0; i < m_arrRecogChar.Count; i++)
            {
                m_strRecognizeString += Convert.ToChar(m_arrRecogChar[i].intCode);

                for (int j = 0; j < m_arrSpaceIndex.Count; j++)
                {
                    if (m_arrSpaceIndex[j] == i)
                    {
                        m_strRecognizeString += " ";
                        break;
                    }
                }
            }
            return m_strRecognizeString;
        }

        public string RecognizeCharsPositionTextArea(ImageDrawing objImage, ROI objROI)
        {
            /*
             * 
             */
            m_Ocr.EmptyChars();
            m_arrRecogChar.Clear();
            m_arrSpaceIndex.Clear();
            m_strRecognizeString = "";
            int intStartX1, intStartY1, intEndX1, intEndY1;
            int intStartX2, intStartY2, intEndX2, intEndY2;
            float fCurrentScore;
            int intDeleteState;
            bool blnOverlap;
            RecogChar objRecogChar;
            ROI objCharROI = new ROI();
            objCharROI.AttachImage(objROI);
            m_objTextMatcher.Match(objROI.ref_ROI);
            if (m_objTextMatcher.NumPositions > 0)
            {
                int intTextOrgX = (int)Math.Round(m_objTextMatcher.GetPosition(0).CenterX - m_objTextMatcher.PatternWidth / 2, 0, MidpointRounding.AwayFromZero);
                int intTextOrgY = (int)Math.Round(m_objTextMatcher.GetPosition(0).CenterY - m_objTextMatcher.PatternHeight / 2, 0,MidpointRounding.AwayFromZero);
                for (int i = 0; i < m_arrMatcher.Count; i++)
                {
                    m_arrMatcher[i].MaxPositions = 1;
                    m_arrMatcher[i].MinScore = 0.5f;

                    for (int j = 0; j < m_arrTemplateChar.Count; j++)
                    {
                        objCharROI.LoadROISetting(
                                intTextOrgX + m_arrTemplateChar[j].intOrgX - 5,
                                intTextOrgY + m_arrTemplateChar[j].intOrgY - 5,
                                m_arrTemplateChar[j].intWidth + 10,
                                m_arrTemplateChar[j].intHeight + 10);

                        m_arrMatcher[i].Match(objCharROI.ref_ROI);

                        if (m_arrMatcher[i].NumPositions > 0)
                        {

                        }
                    }

                    m_arrMatcher[i].MaxPositions = 1;
                    m_arrMatcher[i].MinScore = 0.5f;
                    m_arrMatcher[i].Match(objROI.ref_ROI);
                    if (m_arrMatcher[i].NumPositions == 0)
                        continue;
#if (Debug_2_12 || Release_2_12)
                    for (uint j = 0; j < 1; j++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    for (int j = 0; j < 1; j++)
#endif

                    {
                        intStartX1 = Convert.ToInt32(m_arrMatcher[i].GetPosition(j).CenterX - m_arrMatcher[i].PatternWidth / 2) + 5;
                        intStartY1 = Convert.ToInt32(m_arrMatcher[i].GetPosition(j).CenterY - m_arrMatcher[i].PatternHeight / 2) + 5;
                        intEndX1 = Convert.ToInt32(m_arrMatcher[i].GetPosition(j).CenterX + m_arrMatcher[i].PatternWidth / 2) - 5;
                        intEndY1 = Convert.ToInt32(m_arrMatcher[i].GetPosition(j).CenterY + m_arrMatcher[i].PatternHeight / 2) - 5;
                        fCurrentScore = m_arrMatcher[i].GetPosition(j).Score * 100;

                        intDeleteState = 0;
                        blnOverlap = false;

                        // Check overlap and get max score if overlap
                        for (int k = 0; k < m_arrRecogChar.Count; k++)
                        {
                            intStartX2 = m_arrRecogChar[k].intOrgX;
                            intStartY2 = m_arrRecogChar[k].intOrgY;
                            intEndX2 = m_arrRecogChar[k].intEndX;
                            intEndY2 = m_arrRecogChar[k].intEndY;

                            if (((intStartX1 >= intStartX2) && (intStartX1 <= intEndX2) && (intStartY1 >= intStartY2) && (intStartY1 <= intEndY2)) ||
                                ((intStartX1 >= intStartX2) && (intStartX1 <= intEndX2) && (intEndY1 >= intStartY2) && (intEndY1 <= intEndY2)) ||
                                ((intEndX1 >= intStartX2) && (intEndX1 <= intEndX2) && (intStartY1 >= intStartY2) && (intStartY1 <= intEndY2)) ||
                                ((intEndX1 >= intStartX2) && (intEndX1 <= intEndX2) && (intEndY1 >= intStartY2) && (intEndY1 <= intEndY2)) ||
                                ((intStartX2 >= intStartX1) && (intStartX2 <= intEndX1) && (intStartY2 >= intStartY1) && (intStartY2 <= intEndY1)) ||
                                ((intStartX2 >= intStartX1) && (intStartX2 <= intEndX1) && (intEndY2 >= intStartY1) && (intEndY2 <= intEndY1)) ||
                                ((intEndX2 >= intStartX1) && (intEndX2 <= intEndX1) && (intStartY2 >= intStartY1) && (intStartY2 <= intEndY1)) ||
                                ((intEndX2 >= intStartX1) && (intEndX2 <= intEndX1) && (intEndY2 >= intStartY1) && (intEndY2 <= intEndY1)) ||
                                ((intStartX1 > intStartX2) && (intStartX1 < intEndX2) && (intStartY1 < intStartY2) && (intEndY1 > intEndY2)) ||
                                ((intStartY1 > intStartY2) && (intStartY1 < intEndY2) && (intStartX1 < intStartX2) && (intEndX1 > intEndX2)))
                            {
                                blnOverlap = true;

                                // current score higher than recorded score
                                if (fCurrentScore > m_arrRecogChar[k].fScore)
                                {
                                    m_arrRecogChar.RemoveAt(k);
                                    k--;
                                    if (intDeleteState != -1)
                                        intDeleteState = 1;
                                }
                                else
                                {
                                    intDeleteState = -1;
                                }
                            }
                        }


                        if (!blnOverlap || (blnOverlap && intDeleteState == 1))
                        {
                            objRecogChar = new RecogChar();
                            objRecogChar.intCode = m_Ocr.GetPatternCode(i);
                            objRecogChar.intPatternIndex = i;
                            objRecogChar.intOrgX = intStartX1 - 5;
                            objRecogChar.intOrgY = intStartY1 - 5;
                            objRecogChar.intEndX = intEndX1 + 5;
                            objRecogChar.intEndY = intEndY1 + 5;
                            objRecogChar.intWidth = m_arrMatcher[i].PatternWidth;
                            objRecogChar.intHeight = m_arrMatcher[i].PatternHeight;
                            objRecogChar.fScore = fCurrentScore;
                            m_arrRecogChar.Add(objRecogChar);
                        }
                    }
                }
            }
            FillterRecogString(objROI);
            ShortRecogChars();

            // Get string
            for (int i = 0; i < m_arrRecogChar.Count; i++)
            {
                m_strRecognizeString += Convert.ToChar(m_arrRecogChar[i].intCode);

                for (int j = 0; j < m_arrSpaceIndex.Count; j++)
                {
                    if (m_arrSpaceIndex[j] == i)
                    {
                        m_strRecognizeString += " ";
                        break;
                    }
                }
            }
            return m_strRecognizeString;
        }

        public void ClearTemplateChars()
        {
            m_arrTemplateChar.Clear();
        }

        public void DrawBuildedChars(Graphics g, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY)
        {
            m_Ocr.DrawChars(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B), fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
        }

        public void GetCharEndPoint(int intCharIndex, ref int intEndX, ref int intEndY)
        {
            if (USE_EVISION_OCR)
            {
                intEndX = m_Ocr.CharGetOrgX(intCharIndex) + m_Ocr.CharGetWidth(intCharIndex);
                intEndY = m_Ocr.CharGetOrgY(intCharIndex) + m_Ocr.CharGetHeight(intCharIndex); ;
            }
            else
            {
                intEndX = m_arrRecogChar[intCharIndex].intEndX;
                intEndY = m_arrRecogChar[intCharIndex].intEndY;
            }
        }

        public void GetCharSize(int intCharIndex, ref int intWidth, ref int intHeight)
        {
            if (USE_EVISION_OCR)
            {
                intWidth = m_Ocr.CharGetWidth(intCharIndex);
                intHeight = m_Ocr.CharGetHeight(intCharIndex);
            }
            else
            {
                intWidth = m_arrRecogChar[intCharIndex].intWidth;
                intHeight = m_arrRecogChar[intCharIndex].intHeight;
            }
        }

        public void GetCharStartPoint(int intCharIndex, ref int intOrgX, ref int intOrgY)
        {
            if (USE_EVISION_OCR)
            {
                intOrgX = m_Ocr.CharGetOrgX(intCharIndex);
                intOrgY = m_Ocr.CharGetOrgY(intCharIndex);
            }
            else
            {
                intOrgX = m_arrRecogChar[intCharIndex].intOrgX;
                intOrgY = m_arrRecogChar[intCharIndex].intOrgY;
            }
        }

        public void GetTemplateCharEndPoint(int intCharIndex, ref int intEndX, ref int intEndY)
        {
            intEndX = m_arrTemplateChar[intCharIndex].intEndX;
            intEndY = m_arrTemplateChar[intCharIndex].intEndY;
        }

        public void GetTemplateCharOffSet(int intCharIndex, ref int intOffSetX, ref int intOffSetY)
        {
            intOffSetX = m_arrTemplateChar[intCharIndex].intOffSetX;
            intOffSetY = m_arrTemplateChar[intCharIndex].intOffSetY;
        }

        public void GetTemplateCharSize(int intCharIndex, ref int intWidth, ref int intHeight)
        {
            intWidth = m_arrTemplateChar[intCharIndex].intWidth;
            intHeight = m_arrTemplateChar[intCharIndex].intHeight;
        }

        public void GetTemplateCharStartPoint(int intCharIndex, ref int intOrgX, ref int intOrgY)
        {
            intOrgX = m_arrTemplateChar[intCharIndex].intOrgX;
            intOrgY = m_arrTemplateChar[intCharIndex].intOrgY;
        }

        public void Load(string strFolderPath)
        {
            // Load OCR object
            if (File.Exists(strFolderPath + "Template.ocr"))
                m_Ocr.Load(strFolderPath + "Template.ocr");

            // Load Pattern Setting
            XmlParser objFile = new XmlParser(strFolderPath + "PatternSetting.xml");
            objFile.GetFirstSection("PatternSetting");
            int intNumPatterns = objFile.GetValueAsInt("PatternNum", 0);
            m_patternSmallestSize.Width = objFile.GetValueAsInt("PatternSmallestSizeWidth", int.MaxValue);
            m_patternSmallestSize.Height = objFile.GetValueAsInt("PatternSmallestSizeHeight", int.MaxValue);
            m_intMaxPosition = objFile.GetValueAsInt("MaxPosition", 1);

            m_arrPattern.Clear();
            for (int i = 0; i < intNumPatterns; i++)
            {
                Pattern objPattern = new Pattern();

                // Load Pattern Class
                objFile.GetFirstSection("Class");
                objPattern.intClass = objFile.GetValueAsInt("ClassPattern" + i, -1);

                // Load Pattern Code
                objFile.GetFirstSection("Code");
                objPattern.intCode = objFile.GetValueAsInt("CodePattern" + i, -1);

                // Load Pattern EMatcher
                if (File.Exists(strFolderPath + "PatternMatcher" + i + ".mch"))
                    objPattern.objMatcher.Load(strFolderPath + "PatternMatcher" + i + ".mch");

                // Load Pattern Image
                if (File.Exists(strFolderPath + "PatternImage" + i + ".bmp"))
                    objPattern.objImage.Load(strFolderPath + "PatternImage" + i + ".bmp");

                m_arrPattern.Add(objPattern);
            }

            // Load Text ROI EMatcher
            if (File.Exists(strFolderPath + "TextMatcher.mch"))
                m_objTextMatcher.Load(strFolderPath + "TextMatcher.mch");
        }

        public void RegconizeCharsPosition(ROI objROI, int intTextStartPoint, int intTextEndPoint)
        {
#if (Debug_2_12 || Release_2_12)
            m_objTextMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
            m_objTextMatcher.CorrelationMode = ECorrelationMode.OffsetNormalized;
            m_objTextMatcher.LearnPattern(objROI.ref_ROI);

            for (int i = 0; i < m_arrRecogChar.Count; i++)
            {

            }
        }

        public void RemoveOCRPattern(int intPatternIndex)
        {
            m_arrPattern.RemoveAt(intPatternIndex);

            //???
            //m_arrRefCharCounter.RemoveAt(intPatternIndex);
        }

        public void Save(string strFolderPath)
        {
            // Delete previous pattern matchers and images
            string[] strFileNameList = Directory.GetFiles(strFolderPath, "Pattern*");
            for (int i = 0; i < strFileNameList.Length; i++)
            {
                File.Delete(strFileNameList[i]);
            }

            // Save current pattern setting
            XmlParser objFile = new XmlParser(strFolderPath + "PatternSetting.xml");
            objFile.WriteSectionElement("PatternSetting");
            objFile.WriteElement1Value("PatternNum", m_arrPattern.Count);
            objFile.WriteElement1Value("PatternSmallestSizeWidth", m_patternSmallestSize.Width);
            objFile.WriteElement1Value("PatternSmallestSizeHeight", m_patternSmallestSize.Height);
            objFile.WriteElement1Value("MaxPosition", m_intMaxPosition);
            objFile.WriteSectionElement("Class");
            for (int i = 0; i < m_arrPattern.Count; i++)
            {
                objFile.WriteElement1Value("ClassPattern" + i, m_arrPattern[i].intClass);
            }
            objFile.WriteSectionElement("Code");
            for (int i = 0; i < m_arrPattern.Count; i++)
            {
                objFile.WriteElement1Value("CodePattern" + i, m_arrPattern[i].intCode);
            }
            objFile.WriteEndElement();

            // Save OCR object
            m_Ocr.Save(strFolderPath + "Template.ocr");

            // Save current pattern matcher and images
            for (int i = 0; i < m_arrPattern.Count; i++)
            {
                m_arrPattern[i].objMatcher.Save(strFolderPath + "PatternMatcher" + i + ".mch");
                m_arrPattern[i].objImage.Save(strFolderPath + "PatternImage" + i + ".bmp");
            }

            // Save current Text ROI matcher
            m_objTextMatcher.Save(strFolderPath + "TextMatcher.mch");
        }

        public void SetCharMinMaxSize(int intMinCharWidth, int intMinCharHeight, int intMaxCharWidth, int intMaxCharHeight)
        {
            m_Ocr.MinCharWidth = intMinCharWidth;
            m_Ocr.MinCharHeight = intMinCharHeight;
            m_Ocr.MaxCharWidth = intMaxCharWidth;
            m_Ocr.MaxCharHeight = intMaxCharHeight;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objImage"></param>
        /// <param name="objROI"></param>
        /// <param name="intCharIndex"></param>
        /// <param name="cPatternChar"></param>
        /// <param name="intClass">0: Digit, 1: UpperCase, 2: LowerCase, 3: Special Case</param>
        public void SetOCRPattern(ImageDrawing objImage, ROI objROI, int intCharIndex, char cPatternChar, int intClass)
        {
            // Declare new pattern object
            Pattern objPattern = new Pattern();

            // Get Pattern ROI
            int intStartX = m_arrTemplateChar[intCharIndex].intOrgX;
            int intStartY = m_arrTemplateChar[intCharIndex].intOrgY;
            int intWidth = m_arrTemplateChar[intCharIndex].intWidth;
            int intHeight = m_arrTemplateChar[intCharIndex].intHeight;
            ROI objPatternROI = new ROI();
            objPatternROI.AttachImage(objImage);
            objPatternROI.LoadROISetting(objROI.ref_ROI.Parent.OrgX + objROI.ref_ROI.OrgX + intStartX,
                                         objROI.ref_ROI.Parent.OrgY + objROI.ref_ROI.OrgY + intStartY, 
                                         intWidth, intHeight);

            // Keep Pattern image
            EImageBW8 objPatternImage = new EImageBW8(objPatternROI.ref_ROIWidth, objPatternROI.ref_ROIHeight);
            EasyImage.Copy(objPatternROI.ref_ROI, objPatternImage);
            objPattern.objImage = objPatternImage;

            // Learn Pattern ROI for PRS used
            EMatcher objMatcher = new EMatcher();
            objMatcher.CorrelationMode = ECorrelationMode.OffsetNormalized;
#if (Debug_2_12 || Release_2_12)
            objMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
            objMatcher.LearnPattern(objPatternROI.ref_ROI);
            objPattern.objMatcher = objMatcher;

            // Set Pattern class/code
            objPattern.intCode = Convert.ToInt32(cPatternChar);

            switch (intClass)
            {
                case 0:
                    objPattern.intClass = (int)EOCRClass.Digit;
                    break;
                case 1:
                    objPattern.intClass = (int)EOCRClass.UpperCase;
                    break;
                case 2:
                    objPattern.intClass = (int)EOCRClass.LowerCase;
                    break;
                default:
                    objPattern.intClass = (int)EOCRClass.Special;
                    break;
            }

            m_arrPattern.Add(objPattern);

            // Get pattern smallest size
            int intLastIndex = m_arrPattern.Count - 1;
            if (m_arrPattern[intLastIndex].objMatcher.PatternWidth < m_patternSmallestSize.Width)
                m_patternSmallestSize.Width = m_arrPattern[intLastIndex].objMatcher.PatternWidth;

            if (m_arrPattern[intLastIndex].objMatcher.PatternWidth < m_patternSmallestSize.Height)
                m_patternSmallestSize.Height = m_arrPattern[intLastIndex].objMatcher.PatternHeight;

            //???
            //// Add Pattern Count as reference char
            //m_arrRefCharCounter.Add(1);
        }

        public void SetRefCharCounter(string strRefChar)
        {
            int intNumPatterns = m_Ocr.NumPatterns;
            char cPatternCode;
            char cRefCharCode;
            bool blnFirstTime = true;
            for (int i = 0; i < intNumPatterns; i++)
            {
                cPatternCode = Convert.ToChar(m_Ocr.GetPatternCode(i));
                blnFirstTime = true;
                for (int j = 0; j < strRefChar.Length; j++)
                {
                    cRefCharCode = Convert.ToChar(strRefChar.Substring(j, 1));
                    if (cRefCharCode == 32)
                        continue;

                    if (cRefCharCode == cPatternCode)
                    {
                        if (blnFirstTime)
                        {
                            blnFirstTime = false;
                            if (i >= m_arrRefCharCounter.Count)
                                m_arrRefCharCounter.Add(1);
                            else
                                m_arrRefCharCounter[i] = 1;
                        }
                        else
                            m_arrRefCharCounter[i]++;
                    }
                }
            }

            for (int i = 0; i < m_arrRefCharCounter.Count; i++)
            {
                if (m_arrRefCharCounter[i] == 0)
                    m_arrRefCharCounter[i] = 1;
            }
        }

        /// <summary>
        /// Set OCR Segmentation Mode.
        /// </summary>
        /// <param name="intSegmentationMode">1 = KeepObjects Mode, 2 = Repaste Objects Mode</param>
        public void SetSegmentationMode(int intSegmentationMode)
        {
            if (intSegmentationMode == 2)
                m_Ocr.SegmentationMode = ESegmentationMode.RepasteObjects;
            else
                m_Ocr.SegmentationMode = ESegmentationMode.KeepObjects;
        }

        public void SetSegmentationParameters(int intThreshold, bool blnWhiteOnBlack, bool blnSegModeKeepObjects,
                    int intMinCharWidth, int intMinCharHeight, int intMaxCharWidth, int intMaxCharHeight,
                    int intMinArea)
        {
            m_Ocr.Threshold = intThreshold;
            if (blnWhiteOnBlack)
                m_Ocr.TextColor = EOCRColor.WhiteOnBlack;
            else
                m_Ocr.TextColor = EOCRColor.BlackOnWhite;

            if (blnSegModeKeepObjects)
                m_Ocr.SegmentationMode =  ESegmentationMode.KeepObjects;
            else
                m_Ocr.SegmentationMode = ESegmentationMode.RepasteObjects;

            m_Ocr.CutLargeChars = true;
            m_Ocr.ShiftingMode = EShiftingMode.Chars;
            m_Ocr.MatchingMode = EMatchingMode.Normalized;
            m_Ocr.ShiftXTolerance = 5;
            m_Ocr.ShiftYTolerance = 5;

            m_Ocr.MinCharWidth = intMinCharWidth;
            m_Ocr.MinCharHeight = intMinCharHeight;
            m_Ocr.MaxCharWidth = intMaxCharWidth;
            m_Ocr.MaxCharHeight = intMaxCharHeight;
            m_Ocr.NoiseArea = intMinArea;
        }

        public void SetSegmentationParameters(int intThreshold, bool blnWhiteOnBlack, bool blnSegModeKeepObjects, int intMinArea)
        {
            m_Ocr.Threshold = intThreshold;
            if (blnWhiteOnBlack)
                m_Ocr.TextColor = EOCRColor.WhiteOnBlack;
            else
                m_Ocr.TextColor = EOCRColor.BlackOnWhite;

            if (blnSegModeKeepObjects)
                m_Ocr.SegmentationMode = ESegmentationMode.KeepObjects;
            else
                m_Ocr.SegmentationMode = ESegmentationMode.RepasteObjects;

            m_Ocr.CutLargeChars = true;
            m_Ocr.ShiftingMode = EShiftingMode.Chars;
            m_Ocr.MatchingMode = EMatchingMode.Normalized;
            m_Ocr.ShiftXTolerance = 5;
            m_Ocr.ShiftYTolerance = 5;
            m_Ocr.NoiseArea = intMinArea;
            m_Ocr.RemoveBorder = false;
        }

        public void SetTemplateCharEndPoint(int intCharIndex, int intEndX, int intEndY)
        {
            if (intCharIndex < m_arrTemplateChar.Count)
            {
                TemplateChar objTemplateChar = m_arrTemplateChar[intCharIndex];
                objTemplateChar.intEndX = intEndX;
                objTemplateChar.intEndY = intEndY;
                m_arrTemplateChar[intCharIndex] = objTemplateChar;
            }
            else
            {
                TemplateChar objTemplateChar = new TemplateChar();
                objTemplateChar.intEndX = intEndX;
                objTemplateChar.intEndY = intEndY;
                m_arrTemplateChar.Add(objTemplateChar);
            }
        }

        public void SetTemplateCharOffSetXY(int intCharIndex, int intOffSetX, int intOffSetY)
        {
            if (intCharIndex < m_arrTemplateChar.Count)
            {
                TemplateChar objTemplateChar = m_arrTemplateChar[intCharIndex];
                objTemplateChar.intOffSetX = intOffSetX;
                objTemplateChar.intOffSetY = intOffSetY;
                m_arrTemplateChar[intCharIndex] = objTemplateChar;
            }
            else
            {
                TemplateChar objTemplateChar = new TemplateChar();
                objTemplateChar.intOffSetX = intOffSetX;
                objTemplateChar.intOffSetY = intOffSetY;
                m_arrTemplateChar.Add(objTemplateChar);
            }
        }

        public void SetTemplateCharSize(int intCharIndex, int intWidth, int intHeight)
        {
            if (intCharIndex < m_arrTemplateChar.Count)
            {
                TemplateChar objTemplateChar = m_arrTemplateChar[intCharIndex];
                objTemplateChar.intWidth = intWidth;
                objTemplateChar.intHeight = intHeight;
                m_arrTemplateChar[intCharIndex] = objTemplateChar;
            }
            else
            {
                TemplateChar objTemplateChar = new TemplateChar();
                objTemplateChar.intWidth = intWidth;
                objTemplateChar.intHeight = intHeight;
                m_arrTemplateChar.Add(objTemplateChar);
            }
        }

        public void SetTemplateCharStartPoint(int intCharIndex, int intOrgX, int intOrgY)
        {
            if (intCharIndex < m_arrTemplateChar.Count)
            {
                TemplateChar objTemplateChar = m_arrTemplateChar[intCharIndex];
                objTemplateChar.intOrgX = intOrgX;
                objTemplateChar.intOrgY = intOrgY;
                m_arrTemplateChar[intCharIndex] = objTemplateChar;
            }
            else
            {
                TemplateChar objTemplateChar = new TemplateChar();
                objTemplateChar.intOrgX = intOrgX;
                objTemplateChar.intOrgY = intOrgY;
                m_arrTemplateChar.Add(objTemplateChar);
            }
        }

        private void FillterRecogString(ROI objROI)
        {
            m_objTextMatcher.Match(objROI.ref_ROI);

            if (m_objTextMatcher.NumPositions > 0)
            {
                int intTextStartX = (int)Math.Round(m_objTextMatcher.GetPosition(0).CenterX - m_objTextMatcher.PatternWidth / 2, 0, MidpointRounding.AwayFromZero);
                int intTextStartY = (int)Math.Round(m_objTextMatcher.GetPosition(0).CenterY - m_objTextMatcher.PatternHeight / 2, 0, MidpointRounding.AwayFromZero);

                List<RecogChar> arrRecogChar = new List<RecogChar>();
                int intCenterX, intCenterY, intStartX, intStartY, intEndX, intEndY;

                for (int i = 0; i < m_arrRecogChar.Count; i++)
                {
                    intCenterX = m_arrRecogChar[i].intOrgX + m_arrRecogChar[i].intWidth / 2 - intTextStartX;
                    intCenterY = m_arrRecogChar[i].intOrgY + m_arrRecogChar[i].intHeight / 2 - intTextStartY;
                    intStartX = m_arrRecogChar[i].intOrgX - intTextStartX;
                    intStartY = m_arrRecogChar[i].intOrgY - intTextStartY;
                    intEndX = m_arrRecogChar[i].intEndX - intTextStartX;
                    intEndY = m_arrRecogChar[i].intEndY - intTextStartY;

                    for (int j = 0; j < m_arrTemplateChar.Count; j++)
                    {
                        if ((m_arrTemplateChar[j].intOffSetX >= intStartX) &&
                            (m_arrTemplateChar[j].intOffSetY >= intStartY) &&
                            (m_arrTemplateChar[j].intOffSetX <= intEndX) &&
                            (m_arrTemplateChar[j].intOffSetY <= intEndY))
                        {
                            arrRecogChar.Add(m_arrRecogChar[i]);
                        }
                    }
                }

                m_arrRecogChar = arrRecogChar;
            }
        }

        private void ShortRecogChars()
        {
            #region Define Row
            int intRowTolerance = 5;
            int intStartY, intEndY;
            int intMinY = 0, intMaxY = 0;
            List<int> arrMinRow = new List<int>();  // Store min y-coordinate of each row
            List<int> arrMaxRow = new List<int>();  // Store max y-coordinate of each row

            // Define row
            int intNumChars = m_arrRecogChar.Count;
            for (int i = 0; i < intNumChars; i++)
            {
                intStartY = m_arrRecogChar[i].intOrgY + intRowTolerance;
                intEndY = m_arrRecogChar[i].intOrgY + m_arrRecogChar[i].intHeight - intRowTolerance;

                bool blnMatch = false;
                int intMatchRowNum = -1;
                for (int j = 0; j < arrMinRow.Count; j++)
                {
                    // Check is row overlap?
                    if (((intStartY >= arrMinRow[j]) && (intStartY <= arrMaxRow[j])) ||
                        ((intEndY >= arrMinRow[j]) && (intEndY <= arrMaxRow[j])) ||
                        ((intStartY <= arrMinRow[j]) && (intEndY >= arrMaxRow[j])) ||
                        ((intStartY <= arrMaxRow[j]) && (intEndY >= arrMinRow[j])))
                    {
                        // Update min and max y-coordinate 
                        if (intStartY < arrMinRow[j])
                            intMinY = intStartY;
                        else
                            intMinY = arrMinRow[j];

                        if (intEndY > arrMaxRow[j])
                            intMaxY = intEndY;
                        else
                            intMaxY = arrMaxRow[j];

                        blnMatch = true;

                        // Remove previous row which is matched
                        arrMinRow.RemoveAt(j);
                        arrMaxRow.RemoveAt(j);

                        // Get matched row index
                        if (intMatchRowNum == -1)
                            intMatchRowNum = j;

                        j--;
                    }
                }

                int intSelectedRow = 0;
                if (blnMatch)
                {
                    intSelectedRow = intMatchRowNum;
                }
                else
                {
                    for (int j = 0; j < arrMinRow.Count; j++)
                    {
                        if (intStartY > arrMaxRow[j])
                            intSelectedRow++;
                    }

                    intMinY = intStartY;
                    intMaxY = intEndY;
                }

                if (arrMinRow.Count == 0)
                {
                    arrMinRow.Add(intMinY);
                    arrMaxRow.Add(intMaxY);
                }
                else
                {
                    arrMinRow.Insert(intSelectedRow, intMinY);
                    arrMaxRow.Insert(intSelectedRow, intMaxY);
                }
            }
            #endregion

            #region Set recognized char to row
            int intCenterY, intStartX, intColumnIndex;
            List<int>[] arrRowItems = new List<int>[arrMinRow.Count];
            for (int i = 0; i < arrMinRow.Count; i++)
                arrRowItems[i] = new List<int>();
            for (int i = 0; i < intNumChars; i++)
            {
                intStartX = m_arrRecogChar[i].intOrgX;
                intCenterY = m_arrRecogChar[i].intOrgY + m_arrRecogChar[i].intHeight / 2;

                for (int j = 0; j < arrMinRow.Count; j++)
                {
                    if ((intCenterY >= arrMinRow[j]) && (intCenterY <= arrMaxRow[j]))
                    {
                        intColumnIndex = 0;
                        for (int k = 0; k < arrRowItems[j].Count; k++)
                        {
                            if (intStartX > m_arrRecogChar[arrRowItems[j][k]].intOrgX)
                                intColumnIndex++;
                        }
                        arrRowItems[j].Insert(intColumnIndex, i);
                    }
                }
            }
            #endregion

            #region Rearrange m_arrRecogChar
            List<RecogChar> arrRecogChar = new List<RecogChar>();
            for (int i = 0; i < arrRowItems.Length; i++)
            {
                for (int j = 0; j < arrRowItems[i].Count; j++)
                {
                    arrRecogChar.Add(m_arrRecogChar[arrRowItems[i][j]]);
                }

                m_arrSpaceIndex.Add(arrRecogChar.Count - 1);
            }
            if (m_arrSpaceIndex.Count > 0)
                m_arrSpaceIndex.RemoveAt(m_arrSpaceIndex.Count - 1);

            m_arrRecogChar = arrRecogChar;
            #endregion
        }
    }
}
