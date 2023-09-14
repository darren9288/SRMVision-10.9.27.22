using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Common;
using Euresys.eVision;

namespace VisionProcessing
{
    public class Mark
    {
        #region Member Variables

        private bool m_blnWhiteOnBlack = true;
        private bool m_blnInspectAllTemplate = true;
        private bool[][] m_blnCharResult;
        private bool[][] m_blnTextResult;
        private int m_intDefaultCharSetting = 75;
        private int m_intFailOptionMask = 0;    
                    //0x01: Extra Mark Char Area / Excess Mark
                    //0x02: Extra Mark Check Area
                    //0x04: Extra Mark Uncheck Area
                    //0x08: Group Extra Mark
                    //0x10: Missing Mark 
                    //0x20: Broken Mark
                    //0x40: Text Shifted
        private int m_intFailResultMask = 0;
                    //0x01: Extra Mark (for all area)
                    //0x02: -
                    //0x04: -
                    //0x08: Group Extra Mark Fail
                    //0x10: Missing Mark Fail
                    //0x20: Broken Mark Fail
                    //0x40: Text Shifted Fail
                    //0x80: -
                    //0x100: Score Fail
                    //0x200: Ocv Text Fail (Loc.Score/Mismatch/OverPrint/UnderPrint)
                    //0x400: OCR Ref Char mismatch
                    //0x800: -
        private int m_intGroupIndex = -1;
        private int m_intGroupNum = 1;
        private int m_intHitCharIndex = -1;
        private int m_intHitTextIndex = -1;
        private int m_intInspectionMode = 0;
        private int m_intMinArea = 20;
        private int m_intMaxArea = 100000;
        private int m_intROIOffSetX = 0;
        private int m_intROIOffSetY = 0;
        private int m_intTemplateIndex = -1;
        private int m_intTemplateMask = 0;          //Keep the template Enable/Disable state
        private int m_intTemplatePriority = 0;      //Keep the template priority number. Each Hex represent each template.       
        private int[] m_intHitCharCounter;
        private int[][] m_intBlobResult;
        private int[][] m_intDefectBlobResult;
        private string m_strErrorMessage = "";

        private Blobs m_objBlobs = new Blobs();
        private Blobs m_objDefectBlobs = new Blobs();
        private Font m_Font = new Font("Verdana", 10);
        private List<String> m_arrRefChars = new List<String>();
        private List<List<NOCV>> m_arrOCV = new List<List<NOCV>>();   // First Dimension: Group, Second Dimension: Template    
        private List<List<TemplateSetting>> m_arrTemplateSetting = new List<List<TemplateSetting>>();   // First Dimension: Group, Second Dimension: Template    
        private OCR m_objOCR = new OCR();               // OCR no group and template dimension. 1 used for all.
        private NOCV m_objOCV = new NOCV();             // For learning purpose. (Tolerance will be reset after learning, separately ocv will be use for learning.)

        class TemplateSetting
        {
            public int intBrokenSize;
            public int intCharShiftXY; 
            public int intExtraMinArea;
            public int intGroupExtraMinArea;
            public int intThreshold;
            public int intUnCheckAreaBottom;
            public int intUnCheckAreaLeft;
            public int intUnCheckAreaRight;
            public int intUnCheckAreaTop;
            public ImageBW8 objTemplateImage = new ImageBW8();
            public List<int> intCharSetting = new List<int>();
            public List<ROI> arrTemplateTextROI = new List<ROI>();
        }

        #endregion

        #region Properties
        public bool ref_blnInspectAllTemplate { get { return m_blnInspectAllTemplate; } set { m_blnInspectAllTemplate = value; } }
        public bool ref_blnWhiteOnBlack { get { return m_blnWhiteOnBlack; } set { m_blnWhiteOnBlack = value; } }
        public int ref_intDefaultCharSetting { get { return m_intDefaultCharSetting; } set { m_intDefaultCharSetting = value; } }
        public int ref_intFailOptionMask { get { return m_intFailOptionMask; } set { m_intFailOptionMask = value; } }
        public int ref_intFailResultMask { get { return m_intFailResultMask; } set { m_intFailResultMask = value; } }
        public int ref_intGroupIndex { get { return m_intGroupIndex; } set { m_intGroupIndex = value; } }
        public int ref_intHitCharIndex { get { return m_intHitCharIndex; } set { m_intHitCharIndex = value; } }
        public int ref_intMinArea { get { return m_intMinArea; } set { m_intMinArea = value; } }
        public int ref_intMaxArea { get { return m_intMaxArea; } set { m_intMaxArea = value; } }
        public int ref_intTemplateIndex { get { return m_intTemplateIndex; } set { m_intTemplateIndex = value; } }
        public int ref_intTemplateMask { get { return m_intTemplateMask; } set { m_intTemplateMask = value; } }
        public int ref_intTemplatePriority { get { return m_intTemplatePriority; } set { m_intTemplatePriority = value; } }
        public string ref_strErrorMessage { get { return m_strErrorMessage; }}
        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="intInspectionMode">0 = OCV mode, 1 = OCR mode</param>
        public Mark(int intInspectionMode)
        {
            m_intInspectionMode = intInspectionMode;
            
            // Only 1 group
            m_intGroupNum = 1;

            if (m_intInspectionMode == 0)
                m_arrOCV.Add(new List<NOCV>());
            else
                m_arrRefChars.Add("");

            m_arrTemplateSetting.Add(new List<TemplateSetting>());
            m_intGroupIndex = 0;
        }

        public Mark(int intInspectionMode, int intGroupNum)
        {
            m_intInspectionMode = intInspectionMode;

            // Multi group
            m_intGroupNum = intGroupNum;

            for (int i = 0; i < intGroupNum; i++)
            {
                if (m_intInspectionMode == 0)
                    m_arrOCV.Add(new List<NOCV>());
                else
                    m_arrRefChars.Add("");

                m_arrTemplateSetting.Add(new List<TemplateSetting>());
            }
            m_intGroupIndex = 0;           
        }



        public bool BuildObject(ROI objROI, bool blnRemoveBorder, bool blnConnexity4)
        {
            return BuildObject(m_objBlobs, objROI, blnRemoveBorder, blnConnexity4, m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intThreshold);
        }

        public bool BuildObject(Blobs objBlobs, ROI objROI, bool blnRemoveBorder, bool blnConnexity4, int intThreshold)
        {
            if (objROI.ref_ROIWidth == 0 || objROI.ref_ROIHeight == 0)
            {
                m_strErrorMessage = "ROI size is zero!";
                return false;
            }

            objBlobs.ref_intThreshold = intThreshold;
            if (blnConnexity4)
                objBlobs.SetConnexity(4);
            else
                objBlobs.SetConnexity(8);
            if (m_blnWhiteOnBlack)
                objBlobs.SetClassSelection(2);
            else
                objBlobs.SetClassSelection(1);
            objBlobs.SetObjectAreaRange(m_intMinArea, m_intMaxArea);
            objBlobs.BuildObjects(objROI, blnRemoveBorder);

            if (objBlobs.ref_intNumSelectedObject == 0)
                return false;

            return true;
        }

        public bool GetCharResult(int intCharIndex)
        {
            if (intCharIndex >= m_blnCharResult.Length)
                return false;

            return m_blnCharResult[intCharIndex];
        }

        public int GetBrokenSize(int intGroupIndex, int intTemplateIndex)
        {
            return m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intBrokenSize;
        }


        public int GetCharShiftXY(int intGroupIndex, int intTemplateIndex)
        {
            return m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intCharShiftXY;
        }

        public int GetCharSetting(int intGroupIndex, int intTemplateIndex, int intCharIndex)
        {
            return m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intCharSetting[intCharIndex];
        }

        public int GetCharSettingCounter(int intGroupIndex, int intTemplateIndex)
        {
            return m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intCharSetting.Count;
        }

        public int GetExtraMinArea(int intGroupIndex, int intTemplateIndex)
        {
            return m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intExtraMinArea;
        }

        public int GetGroupCounter()
        {
            return m_arrTemplateSetting.Count;
        }

        public int GetGroupExtraMinArea(int intGroupIndex, int intTemplateIndex)
        {
            return m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intGroupExtraMinArea;
        }

        public int GetTemplateCounter(int intGroupIndex)
        {
            return m_arrTemplateSetting[intGroupIndex].Count;
        }

        public int GetThreshold(int intGroupIndex, int intTemplateIndex)
        {
            return m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intThreshold;
        }

        public int GetUnCheckAreaBottom(int intGroupIndex, int intTemplateIndex)
        {
            return m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intUnCheckAreaBottom;
        }

        public int GetUnCheckAreaLeft(int intGroupIndex, int intTemplateIndex)
        {
            return m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intUnCheckAreaLeft;
        }

        public int GetUnCheckAreaRight(int intGroupIndex, int intTemplateIndex)
        {
            return m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intUnCheckAreaRight;
        }

        public int GetUnCheckAreaTop(int intGroupIndex, int intTemplateIndex)
        {
            return m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intUnCheckAreaTop;
        }

        public int[] GetCharSetting(int intGroupIndex, int intTemplateIndex)
        {
            return m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intCharSetting.ToArray();
        }

        public string GetInspectionMessage()
        {
            if (m_strErrorMessage.Length != 0)
                return m_strErrorMessage;

            if ((m_intFailResultMask & 0x400) > 0)
            {
                m_strErrorMessage += "*Sample string [" + m_objOCR.ref_strRecognizeString + "] not match with reference string!";
            }
            if ((m_intFailResultMask & 0x200) > 0)
            {
                m_strErrorMessage += "*Ocv Text Fail!";
            }
            if ((m_intFailResultMask & 0x100) > 0)
            {
                m_strErrorMessage += "*Sample char score fail!";
            }
            if ((m_intFailResultMask & 0x40) > 0)
            {
                m_strErrorMessage += "*Text Shifted Fail!";
            }
            if ((m_intFailResultMask & 0x01) > 0)
            {
                m_strErrorMessage += "*Extra Mark Fail!";
            }
            if ((m_intFailResultMask & 0x08) > 0)
            {
                m_strErrorMessage += "*Group Extra Mark Fail!";
            }
            if ((m_intFailResultMask & 0x10) > 0)
            {
                m_strErrorMessage += "*Missing Mark Fail!";
            }
            if ((m_intFailResultMask & 0x20) > 0)
            {
                m_strErrorMessage += "*Broken Mark Fail!";
            }

            return m_strErrorMessage;
        }

        public string GetRefChars(int intGroupIndex)
        {
            return m_arrRefChars[intGroupIndex];
        }


        public void AddTemplate(bool blnUseDefaultSetting)
        {
            if (m_intInspectionMode == 0)
                m_arrOCV[m_intGroupIndex].Add(new NOCV());

            TemplateSetting objTemplateSetting = new TemplateSetting();
            if (blnUseDefaultSetting || m_arrTemplateSetting[m_intGroupIndex].Count == 0)
            {
                objTemplateSetting.intBrokenSize = 10;
                objTemplateSetting.intCharShiftXY = 5;
                objTemplateSetting.intExtraMinArea = 20;
                objTemplateSetting.intGroupExtraMinArea = 200;
                objTemplateSetting.intThreshold = -4;
                objTemplateSetting.intUnCheckAreaBottom = 5;
                objTemplateSetting.intUnCheckAreaLeft = 5;
                objTemplateSetting.intUnCheckAreaRight = 5;
                objTemplateSetting.intUnCheckAreaTop = 5;
            }
            else
            {
                int intLastTemplateIndex = m_arrTemplateSetting[m_intGroupIndex].Count - 1;
                objTemplateSetting = m_arrTemplateSetting[m_intGroupIndex][intLastTemplateIndex];
            }
            m_arrTemplateSetting[m_intGroupIndex].Add(objTemplateSetting);
        }

        public void DeleteTemplate(string strFolderPath)
        {

        }

        public void DrawBlobsObjects(Graphics g, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY)
        {
            m_objBlobs.DrawSelectedBlobs(g, fScale, intZoomImageEdgeX, intZoomImageEdgeY);
        }

        public bool InspectOCRMark(ImageDrawing objImage, ROI objROI)
        {
            /*
             *  Reset previous inspection data
             *  Get true threshold
             *  Build objects
             *  Ocv/Ocr inspection
             *      - Check ref char
             *      - Check recognized char score
             *      - Check recognized char position
             *  Check Text Shifted
             *  Blobs Inspection : Extra Mark(Check Area, Uncheck Area, Group Area), Missing Mark, Broken Mark)
             *  Subtract Inspection : Extra Mark Char Area
             */
            m_strErrorMessage = "";
            m_intFailResultMask = 0;
            int intNoSelectedBlobs = 0;
            int intNumChars = 0;
            m_blnCharResult = new bool[0];
            m_intBlobResult = new int[0];
            m_intDefectBlobResult = new int[0];
            int j;

            // Get threshold true value
            int intThresholdValue;
            if (m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intThreshold == -4)
                intThresholdValue = ROI.GetAutoThresholdValue(objROI, 3);
            else
                intThresholdValue = m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intThreshold;

            if (objROI.ref_ROI.Width == 0)
            {
                m_strErrorMessage = "ROI image is empty!";
                return false;
            }

            // Get off set ROI for drawing purpose
            m_intROIOffSetX = objROI.ref_ROI.Parent.OrgX + objROI.ref_ROI.OrgX;
            m_intROIOffSetY = objROI.ref_ROI.Parent.OrgY + objROI.ref_ROI.OrgY;

            // Build blobs 
            if (!BuildObject(m_objBlobs, objROI, false, true, intThresholdValue))
            {
                m_strErrorMessage = "BuildObjects : No blobs object selected!";
                return false;
            }

            intNoSelectedBlobs = m_objBlobs.ref_intNumSelectedObject;

            // Init blob data
            m_intBlobResult = new int[intNoSelectedBlobs];
            for (int i = 0; i < intNoSelectedBlobs; i++)
            {
                m_intBlobResult[i] = 0;
            }

            #region OCR Inspection
            /*
                // Get object min max size
                float fMinWidth, fMaxWidth, fMinHeight, fMaxHeight;
                fMinWidth = fMinHeight = float.MaxValue;
                fMaxWidth = fMaxHeight = 0;
                m_objBlobs.GetBlobsMinMaxSize(ref fMinWidth, ref fMinHeight, ref fMaxWidth, ref fMaxHeight);
                
                m_objOCR.SetCharMinMaxSize((int)fMinCharWidth, (int)fMinCharHeight, (int)fMaxWidth, (int)fMaxHeight);
                string strRecogString = m_objOCR.RecognizePattern(objImage, objROI); //m_objOCR.RecognizePattern(objImage, objROI);
                m_objOCR.SetCharMinMaxSize((int)fMinCharWidth, (int)fMinCharHeight, (int)fMaxCharWidth, (int)fMaxCharHeight);
                */

            // Get max position for pattern matching
            int intPatternMinArea = m_objOCR.ref_patternSmallestSize.Width * m_objOCR.ref_patternSmallestSize.Height;
            m_objBlobs.SetFirstListBlobs();
            float fBlobWidth = 0, fBlobHeight = 0;
            m_objOCR.ref_intMaxPosition = 0;
            for (int i = 0; i < intNoSelectedBlobs; i++)
            {
                m_objBlobs.GetSelectedListBlobsWidth(ref fBlobWidth);
                m_objBlobs.GetSelectedListBlobsHeight(ref fBlobHeight);

                m_objOCR.ref_intMaxPosition += (int)Math.Round((fBlobWidth * fBlobHeight) / (float)intPatternMinArea, 0, MidpointRounding.AwayFromZero);

                m_objBlobs.SetListBlobsToNext();
            }

            // Set ref char counter if multi group
            if (m_intGroupNum > 1)
                m_objOCR.SetRefCharCounter(m_arrRefChars[m_intGroupIndex]);

            // Recognize characters
            string strRecogString = m_objOCR.RecognizePatternByMatcher(objImage, objROI); //m_objOCR.RecognizePattern(objImage, objROI);

            // init char data
            intNumChars = m_objOCR.ref_NumChars;
            m_blnCharResult = new bool[intNumChars];
            m_intHitCharCounter = new int[intNumChars];
            for (int i = 0; i < intNumChars; i++)
            {
                m_intHitCharCounter[i] = 0;
            }

            // Check Ref Chars Mismatch
            if (strRecogString != m_arrRefChars[m_intGroupIndex])
                m_intFailResultMask |= 0x400;

            // Check score setting
            int[] intMatchPatternIndex;
            float fScore;
            for (int i = 0; i < intNumChars; i++)
            {
                fScore = m_objOCR.GetCharScore(i);
                intMatchPatternIndex = m_objOCR.GetMatchPatternIndexes(i);
                for (j = 0; j < intMatchPatternIndex.Length; j++)
                {
                    if (fScore >= m_arrTemplateSetting[m_intGroupIndex][0].intCharSetting[intMatchPatternIndex[j]])
                        break;
                }

                if (j == intMatchPatternIndex.Length)
                {
                    m_blnCharResult[i] = false;
                    if ((m_intFailResultMask & 0x100) == 0)
                        m_intFailResultMask |= 0x100;
                }
                else
                    m_blnCharResult[i] = true;
            }

            // Check Recognize Chars Position
            PointF pReferPoint = GetReferPoint();


            for (int i = 0; i < intNumChars; i++)
            {
                

            }

            if (m_intFailResultMask > 0)
                return false;

            #endregion

            #region Check Text Shifted
            if ((m_intFailOptionMask & 0x40) > 0)
            {
                if (!CheckTextShifted(0, 0, objROI.ref_ROIWidth, objROI.ref_ROIHeight))
                {
                    m_intFailResultMask |= 0x40;
                    return false;
                }
            }
            #endregion

            #region Blobs Inspection
            bool blnGroupExtraMark = false;
            float fCenterX, fCenterY, fWidth, fHeight, fAngle, fGravityCenterX, fGravityCenterY, fStartX, fStartY, fEndX, fEndY;
            int intArea = 0;
            int intTotalExtraMarkArea = 0;
            TemplateSetting objTemplateSetting = m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex];
            fCenterX = fCenterY = fWidth = fHeight = fAngle = fGravityCenterX = fGravityCenterY = 0;
            m_objBlobs.SetFirstListBlobs();
            for (int i = 0; i < intNoSelectedBlobs; i++)
            {
                m_objBlobs.GetSelectedListBlobsLimitCenterX(ref fCenterX);
                m_objBlobs.GetSelectedListBlobsLimitCenterY(ref fCenterY);
                m_objBlobs.GetSelectedListBlobsWidth(ref fWidth);
                m_objBlobs.GetSelectedListBlobsHeight(ref fHeight);
                m_objBlobs.GetSelectedListBlobsGravityCenterX(ref fGravityCenterX);
                m_objBlobs.GetSelectedListBlobsGravityCenterY(ref fGravityCenterY);
                m_objBlobs.GetSelectedListBlobsArea(ref intArea);

                // Define min area
                if (m_intMinArea < 5)
                {
                    // Use m_intMinArea as min area if m_intMinArea < 5
                    if (intArea <= m_intMinArea)
                    {
                        m_objBlobs.SetListBlobsToNext();
                        continue;
                    }
                }
                else
                {
                    // Use 5 as min area if m_intMinArea >= 5
                    if (intArea <= 5)
                    {
                        m_objBlobs.SetListBlobsToNext();
                        continue;
                    }
                }

                fStartX = fCenterX - fWidth / 2;
                fStartY = fCenterY - fHeight / 2;
                fEndX = fCenterX + fWidth / 2;
                fEndY = fCenterY + fHeight / 2;

                int[] intMatchNumber = m_objOCR.GetMatchCharIndexes(fStartX, fStartY, fEndX, fEndY);

                if (intMatchNumber.Length == 0)
                {
                    // Check is blob in uncheck area
                    if ((m_intFailOptionMask & 0x04) == 0)
                    {
                        if (CheckIsInUncheckArea(objROI, fGravityCenterX, fGravityCenterY))
                        {
                            m_objBlobs.SetListBlobsToNext();
                            continue;
                        }
                    }

                    // Check Group Extra Mark
                    if ((m_intFailOptionMask & 0x08) > 0)
                    {
                        intTotalExtraMarkArea += intArea;
                        m_intBlobResult[i] = 1;

                        if (intTotalExtraMarkArea > objTemplateSetting.intGroupExtraMinArea)
                        {
                            m_intBlobResult[i] = 2;

                            if (!blnGroupExtraMark)
                            {
                                m_intFailResultMask |= 0x08;
                                blnGroupExtraMark = true;

                                // Upgrade result from 1 to 2
                                for (j = 0; j < intNoSelectedBlobs; j++)
                                {
                                    if (m_intBlobResult[j] == 1)
                                        m_intBlobResult[j] = 2;
                                }
                            }
                        }
                    }

                    // Make sure object area is higher than min area and extra min area
                    if ((intArea < m_intMinArea) && (intArea < objTemplateSetting.intExtraMinArea))
                    {
                        m_objBlobs.SetListBlobsToNext();
                        continue;
                    }

                    // Check Extra Mark 
                    else if (((m_intFailOptionMask & 0x02) > 0) || ((m_intFailOptionMask & 0x04) > 0))
                    {
                        m_intBlobResult[i] = 2;

                        if ((m_intFailResultMask & 0x01) == 0)
                            m_intFailResultMask |= 0x01;
                    }
                }
                else
                {
                    // Make sure area >= min area
                    if (intArea < m_intMinArea)
                    {
                        m_objBlobs.SetListBlobsToNext();
                        continue;
                    }

                    // Set char hit by blob counter
                    for (j = 0; j < intMatchNumber.Length; j++)
                    {
                        m_intHitCharCounter[intMatchNumber[j]]++;
                    }

                    if (intMatchNumber.Length == 1)
                    {
                        // Check Broken Mark (based on size)
                        int intWidth = 0, intHeight = 0;
                        m_objOCR.GetCharSize(intMatchNumber[0], ref intWidth, ref intHeight);
                        if ((m_intFailOptionMask & 0x20) > 0)
                        {
                            if ((fWidth < (intWidth - objTemplateSetting.intBrokenSize)) || (fHeight < (intHeight - objTemplateSetting.intBrokenSize)))
                            {
                                m_intBlobResult[i] = 2;

                                m_blnCharResult[intMatchNumber[0]] = false;

                                if ((m_intFailResultMask & 0x20) == 0)
                                    m_intFailResultMask |= 0x20;
                            }
                        }
                    }
                }
                m_objBlobs.SetListBlobsToNext();
            }

            for (int i = 0; i < intNumChars; i++)
            {
                // Check Missing Mark
                if ((m_intFailOptionMask & 0x10) > 0)
                {
                    if (m_intHitCharCounter[i] == 0)
                    {
                        m_blnCharResult[i] = false;
                        if ((m_intFailResultMask & 0x10) == 0)
                            m_intFailResultMask |= 0x10;
                    }
                }

                // Check Broken Mark (Based on quantity blobs in char area)
                if ((m_intFailOptionMask & 0x20) > 0)
                {
                    if (m_intHitCharCounter[i] > 3)
                    {
                        m_blnCharResult[i] = false;
                        if ((m_intFailResultMask & 0x20) == 0)
                            m_intFailResultMask |= 0x20;
                    }
                }
            }
            #endregion

            #region Check extra mark in characters area using substract function and grey value

            if (((m_intFailOptionMask & 0x01) > 0) && m_intFailResultMask == 0)
            {
                /*
                 * Sample ROI = objROI
                 * Template ROI = ocr pattern roi
                 * Subtract ROI = new ROI
                 */

                // Init Template ROI
                ROI objTemplateROI;
                // Init Sample ROI
                ROI objSampleROI = new ROI();
                objSampleROI.AttachImage(objROI);
                // Init Subtracted ROI (result)
                ImageDrawing objSubtractedImage = new ImageDrawing();
                objImage.CopyTo(ref objSubtractedImage);
                ROI objSubtractedROI = new ROI();
                objSubtractedROI.AttachImage(objSubtractedImage);

                int intStartX, intStartY, intWidth, intHeight;
                intStartX = intStartY = intWidth = intHeight = 0;
                for (int i = 0; i < intNumChars; i++)
                {
                    objTemplateROI = m_objOCR.GetCharPatternROI(i);
                    m_objOCR.GetCharStartPoint(i, ref intStartX, ref intStartY);
                    m_objOCR.GetCharSize(i, ref intWidth, ref intHeight);

                    objSampleROI.LoadROISetting(intStartX, intStartY, intWidth, intHeight);
                    objSubtractedROI.LoadROISetting(m_intROIOffSetX + intStartX, m_intROIOffSetY + intStartY, intWidth, intHeight);

                    EasyImage.Subtract(objSampleROI.ref_ROI, objTemplateROI.ref_ROI, objSubtractedROI.ref_ROI);
                }

                objSubtractedROI.LoadROISetting(m_intROIOffSetX, m_intROIOffSetY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                if (BuildObject(m_objDefectBlobs, objSubtractedROI, false, true, intThresholdValue))
                {
                    intNoSelectedBlobs = m_objDefectBlobs.ref_intNumSelectedObject;
                    m_intDefectBlobResult = new int[intNoSelectedBlobs];

                    m_objDefectBlobs.SetFirstListBlobs();
                    for (int i = 0; i < intNoSelectedBlobs; i++)
                    {
                        m_objDefectBlobs.GetSelectedListBlobsLimitCenterX(ref fCenterX);
                        m_objDefectBlobs.GetSelectedListBlobsLimitCenterY(ref fCenterY);
                        m_objDefectBlobs.GetSelectedListBlobsWidth(ref fWidth);
                        m_objDefectBlobs.GetSelectedListBlobsHeight(ref fHeight);
                        m_objDefectBlobs.GetSelectedListBlobsArea(ref intArea);

                        fStartX = fCenterX - fWidth / 2;
                        fStartY = fCenterY - fHeight / 2;
                        fEndX = fCenterX + fWidth / 2;
                        fEndY = fCenterY + fHeight / 2;

                        if ((intArea > m_intMinArea) && (intArea > objTemplateSetting.intExtraMinArea) &&
                           (m_objOCR.GetMatchCharIndexes(fStartX, fStartY, fEndX, fEndY).Length > 0))
                        {
                            m_intDefectBlobResult[i] = 2;

                            if ((m_intFailResultMask & 0x01) == 0)
                                m_intFailResultMask |= 0x01;
                        }
                        else
                            m_intDefectBlobResult[i] = 0;

                        m_objDefectBlobs.SetListBlobsToNext();
                    }

                }
            }

            #endregion

            if (m_intFailResultMask > 0)
                return false;
            else
                return true;
        }

        public bool InspectOCVMark(ImageDrawing objImage, ROI objROI)
        {            
            /*
             *  Reset previous inspection data
             *  Get true threshold
             *  Build objects
             *  Ocv/Ocr inspection
             *      - Check ref char
             *      - Check recognized char score
             *      - Check recognized char position
             *  Check Text Shifted
             *  Blobs Inspection : Extra Mark(Check Area, Uncheck Area, Group Area), Missing Mark, Broken Mark)
             *  Subtract Inspection : Extra Mark Char Area
             */

            int intNoSelectedBlobs = 0;
            int intNumChars = 0;
            m_intBlobResult = new int[0];
            m_intDefectBlobResult = new int[0];
            int j;

            // Get threshold true value
            int intThresholdValue;
            if (m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intThreshold == -4)
                intThresholdValue = ROI.GetAutoThresholdValue(objROI, 3);
            else
                intThresholdValue = m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intThreshold;

            if (objROI.ref_ROI.Width == 0)
            {
                m_strErrorMessage = "ROI image is empty!";
                return false;
            }

            // Get off set ROI for drawing purpose
            m_intROIOffSetX = objROI.ref_ROI.Parent.OrgX + objROI.ref_ROI.OrgX;
            m_intROIOffSetY = objROI.ref_ROI.Parent.OrgY + objROI.ref_ROI.OrgY;

            // Build blobs 
            if (!BuildObject(m_objBlobs, objROI, false, true, intThresholdValue))
            {
                m_strErrorMessage = "BuildObjects : No blobs object selected!";
                return false;
            }

            intNoSelectedBlobs = m_objBlobs.ref_intNumSelectedObject;

            // Init blob data
            m_intBlobResult = new int[intNoSelectedBlobs];
            for (int i = 0; i < intNoSelectedBlobs; i++)
            {
                m_intBlobResult[i] = 0;
            }
            for (int i = 0; i < m_arrOCV[m_intGroupIndex].Count; i++)
                m_arrOCV[m_intGroupIndex][i].DeleteSample();

            int intTemplateCount = 0;
            // Loop from first template until last to do OCV inspection
            while (intTemplateCount < m_arrOCV[m_intGroupIndex].Count)
            {
                m_strErrorMessage = "";
                m_intFailResultMask = 0;
                m_blnCharResult = new bool[0];
                m_blnTextResult = new bool[0];

                // Get selected template
                if (m_blnInspectAllTemplate)
                    m_intTemplateIndex = ((m_intTemplatePriority >> (0x04 * intTemplateCount)) & 0x0F) - 1;

                if ((m_intTemplateMask & (0x01 << m_intTemplateIndex)) > 0)
                {
                    #region OCV Inspection

                    NOCV objOCV = m_arrOCV[m_intGroupIndex][m_intTemplateIndex];

                    objOCV.Inspect(objROI, intThresholdValue);

                    // Check Ocv Text Fail
                    if (objOCV.IsDiagnosticsDefine())
                    {
                        if (objOCV.IsDiagnosticsTextNoFound() || objOCV.IsDiagnosticsTextMismatch() ||
                            objOCV.IsDiagnosticsTextOverprint() || objOCV.IsDiagnosticsTextUnderprint())
                        {
                            m_intFailResultMask |= 0x200;
                        }
                    }
                    int intNumTexts = objOCV.GetNumTexts();
                    m_blnTextResult = new bool[intNumTexts];
                    for (int i = 0; i < intNumTexts; i++)
                    {

                    }


                    // Check Char Score
                    int intCharSetValue;
                    float fCharScore;
                    //m_arrSampleOCV.Clear();
                    intNumChars = m_arrTemplateSetting[0][m_intTemplateIndex].intCharSetting.Count;
                    m_blnCharResult = new bool[intNumChars];
                    for (int i = 0; i < intNumChars; i++)
                    {
                        intCharSetValue = m_arrTemplateSetting[0][m_intTemplateIndex].intCharSetting[i];
                        fCharScore = objOCV.GetCharScore(i);

                        // Fail if score lower than setting
                        if (fCharScore < intCharSetValue)
                        {
                            m_blnCharResult[i] = false;
                            if ((m_intFailResultMask & 0x100) == 0)
                                m_intFailResultMask |= 0x100;
                        }
                        else
                            m_blnCharResult[i] = true;
                    }

                    #endregion

                    #region Check Text Shifted
                    if ((m_intFailOptionMask & 0x40) > 0)
                    {
                        if (!CheckTextShifted(0, 0, objROI.ref_ROIWidth, objROI.ref_ROIHeight))
                        {
                            m_intFailResultMask |= 0x40;
                            return false;
                        }
                    }
                    #endregion

                    #region Blobs Inspection
                    bool blnGroupExtraMark = false;
                    float fCenterX, fCenterY, fWidth, fHeight, fAngle, fGravityCenterX, fGravityCenterY, fStartX, fStartY, fEndX, fEndY;
                    int intArea = 0;
                    int intTotalExtraMarkArea = 0;
                    TemplateSetting objTemplateSetting = m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex];
                    fCenterX = fCenterY = fWidth = fHeight = fAngle = fGravityCenterX = fGravityCenterY = 0;
                    m_objBlobs.SetFirstListBlobs();
                    //for (int i = 0; i < intNoSelectedBlobs; i++)
                    for (int i = 0; i < 0; i++)
                    {
                        m_objBlobs.GetSelectedListBlobsLimitCenterX(ref fCenterX);
                        m_objBlobs.GetSelectedListBlobsLimitCenterY(ref fCenterY);
                        m_objBlobs.GetSelectedListBlobsWidth(ref fWidth);
                        m_objBlobs.GetSelectedListBlobsHeight(ref fHeight);
                        m_objBlobs.GetSelectedListBlobsGravityCenterX(ref fGravityCenterX);
                        m_objBlobs.GetSelectedListBlobsGravityCenterY(ref fGravityCenterY);
                        m_objBlobs.GetSelectedListBlobsArea(ref intArea);

                        // Define min area
                        if (m_intMinArea < 5)
                        {
                            // Use m_intMinArea as min area if m_intMinArea < 5
                            if (intArea <= m_intMinArea)
                            {
                                m_objBlobs.SetListBlobsToNext();
                                continue;
                            }
                        }
                        else
                        {
                            // Use 5 as min area if m_intMinArea >= 5
                            if (intArea <= 5)
                            {
                                m_objBlobs.SetListBlobsToNext();
                                continue;
                            }
                        }

                        fStartX = fCenterX - fWidth / 2;
                        fStartY = fCenterY - fHeight / 2;
                        fEndX = fCenterX + fWidth / 2;
                        fEndY = fCenterY + fHeight / 2;

                        int[] intMatchNumber = m_objOCR.GetMatchCharIndexes(fStartX, fStartY, fEndX, fEndY);

                        if (intMatchNumber.Length == 0)
                        {
                            // Check is blob in uncheck area
                            if ((m_intFailOptionMask & 0x04) == 0)
                            {
                                if (CheckIsInUncheckArea(objROI, fGravityCenterX, fGravityCenterY))
                                {
                                    m_objBlobs.SetListBlobsToNext();
                                    continue;
                                }
                            }

                            // Check Group Extra Mark
                            if ((m_intFailOptionMask & 0x08) > 0)
                            {
                                intTotalExtraMarkArea += intArea;
                                m_intBlobResult[i] = 1;

                                if (intTotalExtraMarkArea > objTemplateSetting.intGroupExtraMinArea)
                                {
                                    m_intBlobResult[i] = 2;

                                    if (!blnGroupExtraMark)
                                    {
                                        m_intFailResultMask |= 0x08;
                                        blnGroupExtraMark = true;

                                        // Upgrade result from 1 to 2
                                        for (j = 0; j < intNoSelectedBlobs; j++)
                                        {
                                            if (m_intBlobResult[j] == 1)
                                                m_intBlobResult[j] = 2;
                                        }
                                    }
                                }
                            }

                            // Make sure object area is higher than min area and extra min area
                            if ((intArea < m_intMinArea) && (intArea < objTemplateSetting.intExtraMinArea))
                            {
                                m_objBlobs.SetListBlobsToNext();
                                continue;
                            }

                            // Check Extra Mark 
                            else if (((m_intFailOptionMask & 0x02) > 0) || ((m_intFailOptionMask & 0x04) > 0))
                            {
                                m_intBlobResult[i] = 2;

                                if ((m_intFailResultMask & 0x01) == 0)
                                    m_intFailResultMask |= 0x01;
                            }
                        }
                        else
                        {
                            // Make sure area >= min area
                            if (intArea < m_intMinArea)
                            {
                                m_objBlobs.SetListBlobsToNext();
                                continue;
                            }

                            // Set char hit by blob counter
                            for (j = 0; j < intMatchNumber.Length; j++)
                            {
                                m_intHitCharCounter[intMatchNumber[j]]++;
                            }

                            if (intMatchNumber.Length == 1)
                            {
                                // Check Broken Mark (based on size)
                                int intWidth = 0, intHeight = 0;
                                m_objOCR.GetCharSize(intMatchNumber[0], ref intWidth, ref intHeight);
                                if ((m_intFailOptionMask & 0x20) > 0)
                                {
                                    if ((fWidth < (intWidth - objTemplateSetting.intBrokenSize)) || (fHeight < (intHeight - objTemplateSetting.intBrokenSize)))
                                    {
                                        m_intBlobResult[i] = 2;

                                        m_blnCharResult[intMatchNumber[0]] = false;

                                        if ((m_intFailResultMask & 0x20) == 0)
                                            m_intFailResultMask |= 0x20;
                                    }
                                }
                            }
                        }
                        m_objBlobs.SetListBlobsToNext();
                    }

                    //for (int i = 0; i < intNumChars; i++)
                    for (int i = 0; i < 0; i++)
                    {
                        // Check Missing Mark
                        if ((m_intFailOptionMask & 0x10) > 0)
                        {
                            if (m_intHitCharCounter[i] == 0)
                            {
                                m_blnCharResult[i] = false;
                                if ((m_intFailResultMask & 0x10) == 0)
                                    m_intFailResultMask |= 0x10;
                            }
                        }

                        // Check Broken Mark (Based on quantity blobs in char area)
                        if ((m_intFailOptionMask & 0x20) > 0)
                        {
                            if (m_intHitCharCounter[i] > 3)
                            {
                                m_blnCharResult[i] = false;
                                if ((m_intFailResultMask & 0x20) == 0)
                                    m_intFailResultMask |= 0x20;
                            }
                        }
                    }
                    #endregion

                    #region Check extra mark in characters area using substract function and grey value
                    if (((m_intFailOptionMask & 0x01) > 0) && m_intFailResultMask == 0)
                    {
                        ROI objTemplateTextROI = m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].arrTemplateTextROI[0];

                        // Define sample Ocv Text area as Sample ROI
                        ROI objSampleTextROI = new ROI();
                        System.Drawing.Point pStart = objOCV.GetTextStartXY(0);
                        System.Drawing.Point pEnd = objOCV.GetTextEndXY(0);
                        objSampleTextROI.LoadROISetting(pStart.X, pStart.Y, pEnd.X - pStart.X, pEnd.Y - pStart.Y);
                        objSampleTextROI.AttachImage(objImage);

                        ImageDrawing objSubtractedImage = new ImageDrawing();
                        objImage.CopyTo(ref objSubtractedImage);
                        ROI objSubtractedROI = new ROI();
                        objSubtractedROI.AttachImage(objSubtractedImage);
                        objSubtractedROI.LoadROISetting(pStart.X, pStart.Y, pEnd.X - pStart.X, pEnd.Y - pStart.Y);

                        EasyImage.Subtract(objSampleTextROI.ref_ROI, objTemplateTextROI.ref_ROI, objSubtractedROI.ref_ROI);

                        if (BuildObject(m_objDefectBlobs, objSubtractedROI, false, true, intThresholdValue))
                        {
                            intNoSelectedBlobs = m_objDefectBlobs.ref_intNumSelectedObject;
                            m_intDefectBlobResult = new int[intNoSelectedBlobs];

                            m_objDefectBlobs.SetFirstListBlobs();
                            for (int i = 0; i < intNoSelectedBlobs; i++)
                            {
                                m_objDefectBlobs.GetSelectedListBlobsLimitCenterX(ref fCenterX);
                                m_objDefectBlobs.GetSelectedListBlobsLimitCenterY(ref fCenterY);
                                m_objDefectBlobs.GetSelectedListBlobsWidth(ref fWidth);
                                m_objDefectBlobs.GetSelectedListBlobsHeight(ref fHeight);
                                m_objDefectBlobs.GetSelectedListBlobsArea(ref intArea);

                                fStartX = fCenterX - fWidth / 2;
                                fStartY = fCenterY - fHeight / 2;
                                fEndX = fCenterX + fWidth / 2;
                                fEndY = fCenterY + fHeight / 2;

                                if ((intArea > m_intMinArea) && (intArea > objTemplateSetting.intExtraMinArea) &&
                                   (m_objOCR.GetMatchCharIndexes(fStartX, fStartY, fEndX, fEndY).Length > 0))
                                {
                                    m_intDefectBlobResult[i] = 2;

                                    if ((m_intFailResultMask & 0x01) == 0)
                                        m_intFailResultMask |= 0x01;
                                }
                                else
                                    m_intDefectBlobResult[i] = 0;

                                m_objDefectBlobs.SetListBlobsToNext();
                            }

                        }
                    }


                    #endregion

                }

                // Stop to test next template if return pass
                if (m_intFailResultMask == 0)
                    break;


                // Stop to test next template if not inspect all
                if (!m_blnInspectAllTemplate)
                    break;

                // Go to next template
                intTemplateCount++;
            }
        
            if (m_intFailResultMask > 0)
                return false;
            else
                return true;
        }

        public void LoadTemplate(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "Template.xml");
            objFile.GetFirstSection("MarkSettings");
            m_intFailOptionMask = objFile.GetValueAsInt("FailOptionMask", 0);
            m_intMinArea = objFile.GetValueAsInt("MinArea", 20);
            m_intMaxArea = objFile.GetValueAsInt("MaxArea", 100000);
            m_intTemplateMask = objFile.GetValueAsInt("TemplateMask", 0);
            m_intTemplatePriority = objFile.GetValueAsInt("TemplatePriority", 0);
            m_blnWhiteOnBlack = objFile.GetValueAsBoolean("WhiteOnBlack", true);

            for (int i = 0; i < m_intGroupNum; i++)
            {
                objFile.GetFirstSection("Group" + i);

                if (m_intInspectionMode == 0)
                    m_arrOCV[i].Clear();
                else
                    m_arrRefChars[i] = objFile.GetValueAsString("RefChar", "");

                int intTemplateNum = objFile.GetValueAsInt("TemplateNum", 0);
                m_arrTemplateSetting[i].Clear();

                for (int j = 0; j < intTemplateNum; j++)
                {
                    objFile.GetSecondSection("Template" + j);
                    TemplateSetting objTemplateSetting = new TemplateSetting();
                    objTemplateSetting.intBrokenSize = objFile.GetValueAsInt("BrokenSize", 10, 2);
                    objTemplateSetting.intCharShiftXY = objFile.GetValueAsInt("CharShiftXY", 5, 2);
                    objTemplateSetting.intExtraMinArea = objFile.GetValueAsInt("ExtraMinArea", 20, 2);
                    objTemplateSetting.intGroupExtraMinArea = objFile.GetValueAsInt("GroupExtraMinArea", 200, 2);
                    objTemplateSetting.intThreshold = objFile.GetValueAsInt("Threshold", -4, 2);
                    objTemplateSetting.intUnCheckAreaBottom = objFile.GetValueAsInt("UnCheckAreaBottom", 5, 2);
                    objTemplateSetting.intUnCheckAreaLeft = objFile.GetValueAsInt("UnCheckAreaLeft", 5, 2);
                    objTemplateSetting.intUnCheckAreaRight = objFile.GetValueAsInt("UnCheckAreaRight", 5, 2);
                    objTemplateSetting.intUnCheckAreaTop = objFile.GetValueAsInt("UnCheckAreaTop", 5, 2);

                    int intCharNum = objFile.GetValueAsInt("CharNum", 0, 2);
                    objFile.GetThirdSection("CharSetting");
                    for (int k = 0; k < intCharNum; k++)
                    {
                        objTemplateSetting.intCharSetting.Add(objFile.GetValueAsInt("CharSetting" + k, m_intDefaultCharSetting, 3));
                    }

                    if (m_intInspectionMode == 0)
                    {
                        m_arrOCV[i].Add(new NOCV());

                        objFile.GetThirdSection("CharNo");
                        for (int k = 0; k < intCharNum; k++)
                        {
                            m_arrOCV[i][j].SetCharNo(k, objFile.GetValueAsInt("CharNo" + k, k, 3));
                        }

                        objTemplateSetting.objTemplateImage.Load(strFolderPath + "OriTemplate" + i + "_" + j + ".bmp");

                        int intNumTexts = objFile.GetValueAsInt("TextNum", 0, 2);
                        objFile.GetThirdSection("TextROI");
                        for (int tx = 0; tx < intNumTexts; tx++)
                        {
                            objTemplateSetting.arrTemplateTextROI.Add(new ROI());
                            objTemplateSetting.arrTemplateTextROI[tx].LoadROISetting(
                                objFile.GetValueAsInt("TextROIStartX" + tx, 0, 3),
                                objFile.GetValueAsInt("TextROIStartY" + tx, 0, 3),
                                objFile.GetValueAsInt("TextROIWidth" + tx, 0, 3),
                                objFile.GetValueAsInt("TextROIHeight" + tx, 0, 3));
                        }
                    }

                    m_arrTemplateSetting[i].Add(objTemplateSetting);
                }
            }

            if (m_arrTemplateSetting[m_intGroupIndex].Count > 0)
                m_intTemplateIndex = 0;
            else
                m_intTemplateIndex = -1;

            if (m_intInspectionMode == 0)
            {
                // Load OCV objects
                for (int i = 0; i < m_intGroupNum; i++)
                {
                    for (int j = 0; j < m_arrOCV[i].Count; j++)
                    {
                        m_arrOCV[i][j].LoadOCVFile(strFolderPath + "Template" + i + "_" + j + ".ocv");
                    }                   
                }


            }
            else
            {
                // Load OCR objects
                m_objOCR.Load(strFolderPath + "Template.ocr");
                m_objOCR.LoadPattern(strFolderPath);

                if (m_intGroupNum == 1)
                    m_objOCR.SetRefCharCounter(m_arrRefChars[0]);

            }

        }

        public void SaveTemplate(string strFolderPath)
        {
            // Save mark settings
            XmlParser objFile = new XmlParser(strFolderPath + "Template.xml");
            objFile.WriteSectionElement("MarkSettings");
            objFile.WriteElement1Value("FailOptionMask", m_intFailOptionMask);
            objFile.WriteElement1Value("MinArea", m_intMinArea);
            objFile.WriteElement1Value("MaxArea", m_intMaxArea);
            objFile.WriteElement1Value("TemplateMask", m_intTemplateMask);
            objFile.WriteElement1Value("TemplatePriority", m_intTemplatePriority);
            objFile.WriteElement1Value("WhiteOnBlack", m_blnWhiteOnBlack);

            for (int i = 0; i < m_intGroupNum; i++)
            {
                objFile.WriteSectionElement("Group" + i);

                int intTemplateNum;
                if (m_intInspectionMode == 0)
                {
                    intTemplateNum = m_arrOCV[i].Count;
                }
                else
                {
                    intTemplateNum = 1;
                    objFile.WriteElement1Value("RefChar", m_arrRefChars[m_intGroupIndex]);
                }
                
                objFile.WriteElement1Value("TemplateNum", intTemplateNum);

                for (int j = 0; j < intTemplateNum; j++)
                {
                    objFile.WriteElement1Value("Template" + j, "");
                    objFile.WriteElement2Value("BrokenSize", m_arrTemplateSetting[i][j].intBrokenSize);
                    objFile.WriteElement2Value("CharShiftXY", m_arrTemplateSetting[i][j].intCharShiftXY);
                    objFile.WriteElement2Value("ExtraMinArea", m_arrTemplateSetting[i][j].intExtraMinArea);
                    objFile.WriteElement2Value("GroupExtraMinArea", m_arrTemplateSetting[i][j].intGroupExtraMinArea);
                    objFile.WriteElement2Value("Threshold", m_arrTemplateSetting[i][j].intThreshold);
                    objFile.WriteElement2Value("UnCheckAreaBottom", m_arrTemplateSetting[i][j].intUnCheckAreaBottom);
                    objFile.WriteElement2Value("UnCheckAreaLeft", m_arrTemplateSetting[i][j].intUnCheckAreaLeft);
                    objFile.WriteElement2Value("UnCheckAreaRight", m_arrTemplateSetting[i][j].intUnCheckAreaRight);
                    objFile.WriteElement2Value("UnCheckAreaTop", m_arrTemplateSetting[i][j].intUnCheckAreaTop);
                    objFile.WriteElement2Value("CharNum", m_arrTemplateSetting[i][j].intCharSetting.Count);
                    
                    objFile.WriteElement2Value("CharSetting", "");
                    for (int k = 0; k < m_arrTemplateSetting[i][j].intCharSetting.Count; k++)
                    {
                        objFile.WriteElement3Value("CharSetting" + k, m_arrTemplateSetting[i][j].intCharSetting[k]);
                    }

                    if (m_intInspectionMode == 0)
                    {
                        objFile.WriteElement2Value("CharNo", "");
                        for (int k = 0; k < m_arrTemplateSetting[i][j].intCharSetting.Count; k++)
                        {
                            objFile.WriteElement3Value("CharNo" + k, m_objOCV.GetCharNo(k));
                        }

                        int intNumTexts = m_arrTemplateSetting[i][j].arrTemplateTextROI.Count;
                        objFile.WriteElement2Value("TextNum", intNumTexts);
                        objFile.WriteElement2Value("TextROI", "");
                        for (int tx = 0; tx < intNumTexts; tx++)
                        {
                            objFile.WriteElement3Value("TextROIStartX" + tx, m_arrTemplateSetting[i][j].arrTemplateTextROI[tx].ref_ROIPositionX);
                            objFile.WriteElement3Value("TextROIStartY" + tx, m_arrTemplateSetting[i][j].arrTemplateTextROI[tx].ref_ROIPositionY);
                            objFile.WriteElement3Value("TextROIWidth" + tx, m_arrTemplateSetting[i][j].arrTemplateTextROI[tx].ref_ROIWidth);
                            objFile.WriteElement3Value("TextROIHeight" + tx, m_arrTemplateSetting[i][j].arrTemplateTextROI[tx].ref_ROIHeight);
                        }
                    }
                }
            }

            objFile.WriteEndElement();

            if (m_intInspectionMode == 0)
            {
                // Save OCV objects
                m_objOCV.SaveOCV(strFolderPath + "Template" + m_intGroupIndex + "_" + m_intTemplateIndex + ".ocv");
            }
            else
            {
                // Save OCR objects
                m_objOCR.Save(strFolderPath + "Template.ocr");
                m_objOCR.SavePattern(strFolderPath);
            }
        }

        public void DeleteAllPreviousTemplate(string strFolderPath)
        {
            if (m_intInspectionMode == 0)
            {              
                // Delete whole template folder
                if (Directory.Exists(strFolderPath + "Template"))
                    Directory.Delete(strFolderPath + "Template", true);

                // Delete ocv objects from index 1 to last
                m_arrOCV[m_intGroupIndex].RemoveRange(1, m_arrOCV[m_intGroupIndex].Count - 1);

                m_intTemplateMask = 0;
                m_intTemplatePriority = 0;
            }
            else
            {
            }
        }

        public void SetBrokenSize(int intGroupIndex, int intTemplateIndex, int intBrokenSize)
        {
            m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intBrokenSize = intBrokenSize;
        }

        public void SetCharShiftXY(int intGroupIndex, int intTemplateIndex, int intCharShiftXY)
        {
            m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intCharShiftXY = intCharShiftXY;
        }

        public void SetCharSetting(int intGroupIndex, int intTemplateIndex, int intCharIndex, int intSetValue)
        {
            m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intCharSetting[intCharIndex] = intSetValue;
        }

        public void SetExtraMinArea(int intGroupIndex, int intTemplateIndex, int intExtraMinArea)
        {
            m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intExtraMinArea = intExtraMinArea;
        }

        public void SetGroupExtraMinArea(int intGroupIndex, int intTemplateIndex, int intGroupExtraMinArea)
        {
            m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intGroupExtraMinArea = intGroupExtraMinArea;
        }

        public void SetRefChars(int intGroupIndex, string strRefChars)
        {
            for (int i = 0; i < strRefChars.Length; i++)
            {
                if (strRefChars[i] == ' ')
                {
                    strRefChars = strRefChars.Substring(i + 1);
                    i--;
                }
                else
                    break;
            }

            for (int i = strRefChars.Length - 1; i >= 0; i--)
            {
                if (strRefChars[i] == ' ')
                {
                    strRefChars = strRefChars.Substring(0, strRefChars.Length - 1); ;
                }
                else
                    break;
            }

            m_arrRefChars[intGroupIndex] = strRefChars;
            m_objOCR.SetRefCharCounter(strRefChars);
        }

        public void SetThreshold(int intGroupIndex, int intTemplateIndex, int intThreshold)
        {
            m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intThreshold = intThreshold;
        }

        public void SetUnCheckAreaBottom(int intGroupIndex, int intTemplateIndex, int intUnCheckAreaBottom)
        {
            m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intUnCheckAreaBottom = intUnCheckAreaBottom;
        }

        public void SetUnCheckAreaLeft(int intGroupIndex, int intTemplateIndex, int intUnCheckAreaLeft)
        {
            m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intUnCheckAreaLeft = intUnCheckAreaLeft;
        }

        public void SetUnCheckAreaRight(int intGroupIndex, int intTemplateIndex, int intUnCheckAreaRight)
        {
            m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intUnCheckAreaRight = intUnCheckAreaRight;
        }

        public void SetUnCheckAreaTop(int intGroupIndex, int intTemplateIndex, int intUnCheckAreaTop)
        {
            m_arrTemplateSetting[intGroupIndex][intTemplateIndex].intUnCheckAreaTop = intUnCheckAreaTop;
        }



        // --------- OCV function -------------------------------------------

        public int GetNumTemplates()
        {
            return m_arrOCV[m_intGroupIndex].Count;
        }

        public int GetNumTexts()
        {
            return m_objOCV.GetNumTexts();
        }

        public void BuildOCVChars(ROI objROI, int intCreationMode)
        {
            m_objOCV.BuildChars(m_objBlobs, intCreationMode);
        }

        public void DrawOCVChars(Graphics g, ROI objROI, int intOriX, int intOriY, int intWidth, int intHeight, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY)
        {
            m_objOCV.SetTemplateImage(objROI, intOriX, intOriY, intWidth, intHeight);
            m_objOCV.DrawTemplateChars(g, fScale, intZoomImageEdgeX, intZoomImageEdgeY);
        }

        public void DrawOCVTexts(Graphics g, ROI objROI, int intOriX, int intOriY, int intWidth, int intHeight, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY)
        {
            m_objOCV.SetTemplateImage(objROI, intOriX, intOriY, intWidth, intHeight);
            m_objOCV.DrawTemplateTexts(g, fScale, intZoomImageEdgeX, intZoomImageEdgeY);
        }

        public void LearnOCVTemplate(ImageDrawing objTemplateImage, ROI objSearchROI, int intOriX, int intOriY, int intWidth, int intHeight, ROI objLearnROI)
        {
            // Learn image for OCV
            m_objOCV.SetTemplateImage(objSearchROI, intOriX, intOriY, intWidth, intHeight);
            m_objOCV.Learn(objLearnROI);

            // Resorting chars in OCV
            m_objOCV.SortTemplateCharsByCenterPoint();

            // Calculate shift text value
            int intXTolerance = 0;
            int intYTolerance = 0;
            m_objOCV.DefineShiftTextTolerance(objSearchROI, ref intXTolerance, ref intYTolerance);

            // Set shift text tolerence to OcvText
            m_objOCV.SetTextsShiftXY(intXTolerance, intYTolerance);

            // Set shift char tolerance to OcvChar
            int[] arrXToleranceChars = m_arrOCV[m_intGroupIndex][m_intTemplateIndex].GetCharsShiftX();
            int[] arrYToleranceChars = m_arrOCV[m_intGroupIndex][m_intTemplateIndex].GetCharsShiftY();
            if (arrXToleranceChars.Length > 0 && arrYToleranceChars.Length > 0)
                m_objOCV.SetCharsShiftXY(arrXToleranceChars, arrYToleranceChars);   

            // Set chars setting
            int intNewNumChars = m_objOCV.GetNumChars();
            int intPreNumChars = m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intCharSetting.Count;
            if (intPreNumChars == 0)
            {
                for (int i = intPreNumChars; i < intNewNumChars; i++)
                {
                    m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intCharSetting.Add(75);
                }
            }
            else if (intPreNumChars > intNewNumChars)
            {
                m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intCharSetting.RemoveRange(intNewNumChars, intPreNumChars - intNewNumChars);
            }
            else if (intPreNumChars < intNewNumChars)
            {
                int intLastCharSetting = m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intCharSetting[intPreNumChars - 1];

                for (int i = intPreNumChars; i < intNewNumChars; i++)
                {
                    m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intCharSetting.Add(intLastCharSetting);
                }
            }

            m_intTemplateMask |= (0x01 << m_intTemplateIndex);
            if ((m_intTemplatePriority & (0x0F << (0x04 * m_intTemplateIndex))) == 0)
                m_intTemplatePriority |= ((m_intTemplateIndex + 1) << (0x04 * m_intTemplateIndex));

            m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].objTemplateImage = objTemplateImage.ref_objMainImage;

            m_objOCV.Inspect(objLearnROI, m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intThreshold);
            int intNumTexts = m_objOCV.GetNumTexts();
            m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].arrTemplateTextROI.Clear();
            int intTextStartX = 0, intTextStartY = 0, intTextWidth = 0, intTextHeight = 0;
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                m_objOCV.GetTextStartXY(tx, ref intTextStartX, ref intTextStartY);
                m_objOCV.GetTextSize(tx, ref intTextWidth, ref intTextHeight);
                ROI objTextROI = new ROI();
                objTextROI.AttachImage(objTemplateImage);
                objTextROI.LoadROISetting(intTextStartX, intTextStartY, intTextWidth, intTextHeight);
                m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].arrTemplateTextROI.Add(objTextROI);
            }
        }

        public void UndoTemplateChars()
        {
            m_objOCV.DeleteTemplateChars();
        }

        public void UndoTemplateTexts()
        {
            m_objOCV.DeleteTemplateTexts();
        }

        public void FormMultiSelectedChars()
        {
            m_objOCV.FormMultiSelectedChars();
        }

        public void FormSingleSelectedChars()
        {
            m_objOCV.FormSingleSelectedChars();
        }

        public void FormSelectedTexts()
        {
            m_objOCV.BuildTexts();
        }

        public void SelectTemplateChars(ROI objROI, int intOriX, int intOriY, int intWidth, int intHeight, bool blnResetPreviousChars, System.Drawing.Point pStartPoint, System.Drawing.Point pEndPoint)
        {
            if (blnResetPreviousChars)
                m_objOCV.ResetPreviousSelectedChars();

            m_objOCV.SetTemplateImage(objROI, intOriX, intOriY, intWidth, intHeight);

            m_objOCV.SelectTemplateChars(pStartPoint, pEndPoint);
        }

        public void SelectTextAndChar(int intX, int intY)
        {
            m_intHitCharIndex = m_arrOCV[m_intGroupIndex][m_intTemplateIndex].HitChar(intX, intY);
            m_intHitTextIndex = m_arrOCV[m_intGroupIndex][m_intTemplateIndex].HitText(intX, intY);          
        }


        // --------- OCR function -------------------------------------------

        public bool BuildOCRChars(ImageDrawing objImage, ROI objROI)
        {
            // Get off set ROI for drawing purpose
            m_intROIOffSetX = objROI.ref_ROI.Parent.OrgX + objROI.ref_ROI.OrgX;
            m_intROIOffSetY = objROI.ref_ROI.Parent.OrgY + objROI.ref_ROI.OrgY;

            int intNumSelectedObject = m_objBlobs.ref_intNumSelectedObject;

            float fMinWidth, fMaxWidth, fMinHeight, fMaxHeight;
            fMinWidth = fMinHeight = float.MaxValue;
            fMaxWidth = fMaxHeight = 0;
            m_objBlobs.GetBlobsMinMaxSize(ref fMinWidth, ref fMinHeight, ref fMaxWidth, ref fMaxHeight);

            m_intHitCharIndex = -1;

            m_objOCR.SetSegmentationParameters(m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex].intThreshold, 
                            m_blnWhiteOnBlack, false, (int)fMinWidth - 5, (int)fMinHeight - 5,
                            (int)fMaxWidth + 5, (int)fMaxHeight + 5, m_intMinArea);

            if (!m_objOCR.BuildChars(objImage, objROI))
                return false;

            return true;
        }

        public char GetChar(int intCharIndex)
        {
            return m_objOCR.GetFirstLevelChar(intCharIndex);
        }

        public char GetPattern(int intPatternIndex)
        {
            return m_objOCR.GetPattern(intPatternIndex);
        }

        public float GetCharScore(int intCharIndex)
        {
            if (m_intInspectionMode == 0)
                return m_arrOCV[m_intGroupIndex][m_intTemplateIndex].GetCharScore(intCharIndex);
            else
                return m_objOCR.GetCharScore(intCharIndex);
        }

        public int GetNumChars()
        {
            if (m_intInspectionMode == 0)
                return m_arrOCV[m_intGroupIndex][m_intTemplateIndex].GetNumChars();
            else 
                return m_objOCR.ref_NumChars;
        }

        public int GetNumPatterns()
        {
            return m_objOCR.ref_NumPatterns;
        }

        public int GetPatternClass(int intPatternIndex)
        {
            return m_objOCR.GetPatternClass(intPatternIndex);
        }

        public Image GetPatternImage(int intPatternIndex)
        {
            return m_objOCR.GetPatternImage(intPatternIndex);
        }

        public string GetOCRRecognizeResult(ImageDrawing objImage, ROI objROI)
        {
            // Get max position for pattern matching
            int intNoSelectedBlobs = m_objBlobs.ref_intNumSelectedObject;
            int intPatternMinArea = m_objOCR.ref_patternSmallestSize.Width * m_objOCR.ref_patternSmallestSize.Height;
            m_objBlobs.SetFirstListBlobs();
            float fBlobWidth = 0, fBlobHeight = 0;
            m_objOCR.ref_intMaxPosition = 0;
            for (int i = 0; i < intNoSelectedBlobs; i++)
            {
                m_objBlobs.GetSelectedListBlobsWidth(ref fBlobWidth);
                m_objBlobs.GetSelectedListBlobsHeight(ref fBlobHeight);

                m_objOCR.ref_intMaxPosition += (int)Math.Round((fBlobWidth * fBlobHeight) / (float)intPatternMinArea, 0, MidpointRounding.AwayFromZero);

                m_objBlobs.SetListBlobsToNext();
            }

            return m_objOCR.RecognizePatternByMatcher(objImage, objROI);
        }

        public void AddOCRPattern(ImageDrawing objImage, ROI objROI, char cPatternChar, int intClass)
        {
            m_objOCR.SetOCRPattern(objImage, objROI, m_intHitCharIndex, cPatternChar, intClass);

            int intCharSetting;
            if (m_arrTemplateSetting[0][0].intCharSetting.Count > 0)
            {
                int intLastIndex = m_arrTemplateSetting[0][0].intCharSetting.Count - 1;
                intCharSetting = m_arrTemplateSetting[0][0].intCharSetting[intLastIndex];
            }
            else 
                intCharSetting = m_intDefaultCharSetting;
            m_arrTemplateSetting[0][0].intCharSetting.Add(intCharSetting);
        }       

        public void DrawOCRBuildedChars(Graphics g, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY)
        {
            //m_objOCR.DrawBuildedChars(g, fScale, intZoomImageEdgeX, intZoomImageEdgeY);

            int intStartX, intStartY, intWidth, intHeight;
            intStartX = intStartY = intWidth = intHeight = 0;
            int intNumChars = m_objOCR.ref_NumChars;
            for (int i = 0; i < intNumChars; i++)
            {
                m_objOCR.GetCharStartPoint(i, ref intStartX, ref intStartY);
                m_objOCR.GetCharSize(i, ref intWidth, ref intHeight);

                if (i == m_intHitCharIndex)
                {
                    g.DrawRectangle(new Pen(Color.Yellow), (float)m_intROIOffSetX + intStartX,
                        (float)m_intROIOffSetY + intStartY, intWidth, intHeight);
                }
                else
                {
                    g.DrawRectangle(new Pen(Color.Lime), (float)m_intROIOffSetX + intStartX,
                        (float)m_intROIOffSetY + intStartY, intWidth, intHeight);
                }

                g.DrawString(Convert.ToString(i + 1), m_Font, new SolidBrush(Color.Red),
                             m_intROIOffSetX + intStartX, m_intROIOffSetY + intStartY - 3); 
            }
        }

        public void DrawOCRInspectedChars(Graphics g, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY)
        {
            if (m_intInspectionMode == 0)
            {
                int intStartX, intStartY, intWidth, intHeight;
                intStartX = intStartY = intWidth = intHeight = 0;
                NOCV objOCV = m_arrOCV[m_intGroupIndex][m_intTemplateIndex];

                int intNumTexts = objOCV.GetNumTexts();
                for (int i = 0; i < intNumTexts; i++)
                {
                    objOCV.GetTextStartXY(i, ref intStartX, ref intStartY);
                    objOCV.GetTextSize(i, ref intWidth, ref intHeight);
                }

                int intNumChars = objOCV.GetNumChars();
                for (int i = 0; i < intNumChars; i++)
                {
                    objOCV.GetCharStartXY(i, ref intStartX, ref intStartY);
                    objOCV.GetCharSize(i, ref intWidth, ref intHeight);

                    if (i >= m_blnCharResult.Length)
                        continue;

                    if (m_blnCharResult[i])
                    {
                        g.DrawRectangle(new Pen(Color.Lime), intStartX, intStartY, intWidth, intHeight);
                    }
                    else
                    {
                        g.DrawRectangle(new Pen(Color.Red), intStartX, intStartY, intWidth, intHeight);
                    }

                    g.DrawString(Convert.ToString(i + 1), m_Font, new SolidBrush(Color.Red),
                                 intStartX, intStartY - 3);
                }
            }
            else
            {
                int intStartX, intStartY, intWidth, intHeight;
                intStartX = intStartY = intWidth = intHeight = 0;
                int intNumChars = m_objOCR.ref_NumChars;
                for (int i = 0; i < intNumChars; i++)
                {
                    m_objOCR.GetCharStartPoint(i, ref intStartX, ref intStartY);
                    m_objOCR.GetCharSize(i, ref intWidth, ref intHeight);

                    if (i >= m_blnCharResult.Length)
                        continue;

                    if (m_blnCharResult[i])
                    {
                        g.DrawRectangle(new Pen(Color.Lime), (float)m_intROIOffSetX + intStartX,
                            (float)m_intROIOffSetY + intStartY, intWidth, intHeight);
                    }
                    else
                    {
                        g.DrawRectangle(new Pen(Color.Red), (float)m_intROIOffSetX + intStartX,
                            (float)m_intROIOffSetY + intStartY, intWidth, intHeight);
                    }

                    g.DrawString(Convert.ToString(i + 1), m_Font, new SolidBrush(Color.Red),
                                 m_intROIOffSetX + intStartX, m_intROIOffSetY + intStartY - 3);
                }
            }

            int intNoSelectedBlobs = m_objBlobs.ref_intNumSelectedObject;
            m_objBlobs.SetFirstListBlobs();
            for (int i = 0; i < intNoSelectedBlobs; i++)
            {
                if (m_intBlobResult[i] == 2)
                    m_objBlobs.DrawSelectedBlob(g, 1, 0, 0, Color.Yellow, i);

                m_objBlobs.SetListBlobsToNext();
            }
            
            intNoSelectedBlobs = m_objDefectBlobs.ref_intNumSelectedObject;
            m_objDefectBlobs.SetFirstListBlobs();
            for (int i = 0; i < intNoSelectedBlobs; i++)
            {
                if (m_intDefectBlobResult[i] == 2)
                    m_objDefectBlobs.DrawSelectedBlob(g, 1, 0, 0, Color.Yellow, i);

                m_objDefectBlobs.SetListBlobsToNext();
            }
            
        }

        public void HitChars(int intX, int intY)
        {
            m_intHitCharIndex = m_objOCR.HitChars(intX, intY);
        }

        public void RemoveOCRPattern(int intPatternIndex)
        {
            m_objOCR.RemoveOCRPattern(intPatternIndex);
            m_arrTemplateSetting[0][0].intCharSetting.RemoveAt(intPatternIndex);
        }


        // ------------ Private ------------------------------------------------------

        private bool CheckIsInUncheckArea(ROI objROI, float fX, float fY)
        {
            TemplateSetting objTemplateSetting = m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex];

            // Check Top ROI Uncheck Area
            if ((fX >= 0) &&
                (fX <= (objROI.ref_ROIWidth)) &&
                (fY >= 0) &&
                (fY < (objTemplateSetting.intUnCheckAreaTop)))
            {
                return true;
            }

            // Check Bottom ROI Uncheck Area
            if ((fX >= 0) &&
                (fX <= (objROI.ref_ROIWidth)) &&
                (fY > objROI.ref_ROIHeight - objTemplateSetting.intUnCheckAreaBottom) &&
                (fY <= (objROI.ref_ROIHeight)))
            {
                return true;
            }

            // Check Left ROI Uncheck Area
            if ((fX >= 0) &&
                (fX < (objTemplateSetting.intUnCheckAreaLeft)) &&
                (fY >= 0) &&
                (fY <= (objROI.ref_ROIHeight)))
            {
                return true;
            }

            // Check Right ROI Uncheck Area
            if ((fX > objROI.ref_ROIWidth - objTemplateSetting.intUnCheckAreaRight) &&
                (fX <= (objROI.ref_ROIWidth)) &&
                (fY >= 0) &&
                (fY <= (objROI.ref_ROIHeight)))
            {
                return true;
            }

            return false;
        }

        private bool CheckTextShifted(int intROIStartX, int intROIStartY, int intROIEndX, int intROIEndY)
        {
            if (m_intInspectionMode == 0)
            {             
                System.Drawing.Point pStartPoint = m_arrOCV[m_intGroupIndex][m_intTemplateIndex].GetTextStartXY();
                System.Drawing.Point pEndPoint = m_arrOCV[m_intGroupIndex][m_intTemplateIndex].GetTextEndXY();

                // Make sure sample marks are not shifted out of Shift Limit
                TemplateSetting objTemplateSetting = m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex];
                if ((pStartPoint.X < (intROIStartX + objTemplateSetting.intUnCheckAreaLeft)) ||
                    (pStartPoint.Y < (intROIStartY + objTemplateSetting.intUnCheckAreaTop)) ||
                    (pEndPoint.X > (intROIEndX - objTemplateSetting.intUnCheckAreaRight)) ||
                    (pEndPoint.Y > (intROIEndY - objTemplateSetting.intUnCheckAreaBottom)))
                {
                    return false;
                }
            }
            else
            {
                // Get Start and End Point of chars area
                int intStartX, intStartY, intEndX, intEndY;
                intStartX = intStartY = intEndX = intEndY = 0;
                int intNumChars = m_objOCR.ref_NumChars;
                for (int i = 0; i < intNumChars; i++)
                {
                    m_objOCR.GetCharStartPoint(i, ref intStartX, ref intStartY);
                    m_objOCR.GetCharEndPoint(i, ref intEndX, ref intEndY);
                }

                // Make sure sample marks are not shifted out of Shift Limit
                TemplateSetting objTemplateSetting = m_arrTemplateSetting[m_intGroupIndex][m_intTemplateIndex];
                if ((intStartX < (intROIStartX + objTemplateSetting.intUnCheckAreaLeft)) ||
                    (intStartY < (intROIStartY + objTemplateSetting.intUnCheckAreaTop)) ||
                    (intEndX > (intROIEndX - objTemplateSetting.intUnCheckAreaRight)) ||
                    (intEndY > (intROIEndY - objTemplateSetting.intUnCheckAreaBottom)))
                {
                    return false;
                }
            }

            return true;
        }

        private PointF GetReferPoint()
        {
            PointF pReferPoint = new PointF();

            return pReferPoint;
        }

    }
}
