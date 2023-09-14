using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;

namespace VisionProcessing
{
    public class NOCV
    {
        #region Member Variables
        private int m_intImageWidth = 640;
        private int m_intImageHeight = 480;
        private string m_strErrorMessage = "";
        private List<int> m_arrTextNo = new List<int>();
        private List<int> m_arrCharNo = new List<int>();
        private List<Sample> m_arrSampleChar = new List<Sample>();
        private List<Sample> m_arrSampleText = new List<Sample>();
        private EOCV m_BinaOcv = new EOCV();
        private EOCV m_Ocv = new EOCV();
        private EOCV m_MinOcv = new EOCV();
        private ERGBColor m_colorRed = new ERGBColor(Color.Red.R, Color.Red.G, Color.Red.B);
        private ERGBColor m_colorGreen = new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B);
        private ERGBColor m_colorGreenText = new ERGBColor(Color.FromArgb(0, 255, 0).R, Color.FromArgb(0, 255, 0).G, Color.FromArgb(0, 255, 0).B);
        private System.Drawing.Point m_pDrawStartPoint = new System.Drawing.Point(0, 0);

        struct Sample
        {
            public int intNo;
            public int intOrgX;
            public int intOrgY;
            public int intEndX;
            public int intEndY;
            public int intWidth;
            public int intHeight;
            public float fScore;
            public int intLocMode;
            public int intForeArea;
            public float fForeAreaSumPercent;
        }

        #endregion

        #region Properties

        public string ref_strErrorMessage { get { return m_strErrorMessage; } }

        #endregion

        ImageDrawing m_objSampleImage = new ImageDrawing(true);
        public NOCV(int intImageWidth, int intImageHeight)
        {
            m_intImageWidth = intImageWidth;
            m_intImageHeight = intImageHeight;
        }


        public bool IsDiagnosticsDefine()
        {
            //if (m_Ocv.Diagnostics == (int)EDiagnostic.Undefined)
            //    return false;

            return true;
        }

        public bool IsDiagnosticsTextNoFound()
        {
            return true; // Temporary
            //return ((m_Ocv.Diagnostics & (int)EDiagnostic.TextNotFound) > 0);
        }

        public bool IsDiagnosticsTextMismatch()
        {
            return true; // Temporary
            //return ((m_Ocv.Diagnostics & (int)EDiagnostic.TextMismatch) > 0);
        }

        public bool IsDiagnosticsTextOverprint()
        {
            return true; // Temporary
            //return ((m_Ocv.Diagnostics & (int)EDiagnostic.TextOverprinting) > 0);
        }

        public bool IsDiagnosticsTextUnderprint()
        {
            return true; // Temporary
            //return ((m_Ocv.Diagnostics & (int)EDiagnostic.TextUnderprinting) > 0);
        }


        public float GetCharScore(int intCharIndex)
        {
            if (intCharIndex >= m_arrSampleChar.Count)
                return -1;

            return m_arrSampleChar[intCharIndex].fScore;
        }

        public float GetTextScore(int intTextIndex)
        {
            if (intTextIndex >= m_arrSampleText.Count)
                return -1;

            return m_arrSampleText[intTextIndex].fScore;
        }

        public bool GetTextAnalyisResult(ref float fTempLocationScore, ref int intTempBackgroundArea, ref int intTempForegroundArea, ref float fTempBackgroundSum,
            ref float fTempForegroundSum, ref float fSampLocationScore, ref int intSampBackgroundArea, ref int intSampForegroundArea, ref float fSampBackgroundSum,
            ref float fSampForegroundSum, ref float fSampCorrelation, ref float fToleLocationScore, ref int intToleBackgroundArea, ref int intToleForegroundArea,
            ref float fToleBackgroundSum, ref float fToleForegroundSum, ref float fToleCorrelation, ref float fShiftXTole, ref float fShiftXMeasure, ref float fShiftYTole,
            ref float fShiftYMeasure, int intTextNo)
        {
            if (m_arrSampleText.Count == 0)
                return false;

            int intCenterX, intCenterY;
            intCenterX = intCenterY = 0;
            EOCVText objOcvText = new EOCVText();
#if (Debug_2_12 || Release_2_12)
            //Text inspection
            if (m_arrSampleText[intTextNo].intLocMode == 1)
            {
                m_Ocv.GetTextPoint((uint)intTextNo, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
            }
            else if (m_arrSampleText[intTextNo].intLocMode == 2)
            {
                m_BinaOcv.GetTextPoint((uint)intTextNo, out intCenterX, out intCenterY, 0, 0);
                m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                m_BinaOcv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_BinaOcv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
            }
            else
            {
                m_MinOcv.GetTextPoint((uint)intTextNo, out intCenterX, out intCenterY, 0, 0);
                m_MinOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                m_MinOcv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_MinOcv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
            }

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            //Text inspection
            if (m_arrSampleText[intTextNo].intLocMode == 1)
            {
                m_Ocv.GetTextPoint(intTextNo, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
            }
            else if (m_arrSampleText[intTextNo].intLocMode == 2)
            {
                m_BinaOcv.GetTextPoint(intTextNo, out intCenterX, out intCenterY, 0, 0);
                m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                m_BinaOcv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_BinaOcv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
            }
            else
            {
                m_MinOcv.GetTextPoint(intTextNo, out intCenterX, out intCenterY, 0, 0);
                m_MinOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                m_MinOcv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_MinOcv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

            }

#endif

            if (objOcvText.TemplateBackgroundArea != -1 && objOcvText.TemplateBackgroundArea != uint.MaxValue)
            {
                fTempLocationScore = objOcvText.TemplateLocationScore;
                intTempBackgroundArea = (int)objOcvText.TemplateBackgroundArea;
                intTempForegroundArea = (int)objOcvText.TemplateForegroundArea;
                fTempBackgroundSum = objOcvText.TemplateBackgroundSum;
                fTempForegroundSum = objOcvText.TemplateForegroundSum;

                fSampLocationScore = objOcvText.SampleLocationScore;
                intSampBackgroundArea = (int)objOcvText.SampleBackgroundArea;
                intSampForegroundArea = (int)objOcvText.SampleForegroundArea;
                fSampBackgroundSum = objOcvText.SampleBackgroundSum;
                fSampForegroundSum = objOcvText.SampleForegroundSum;

                fSampCorrelation = objOcvText.Correlation;

                fToleLocationScore = (int)objOcvText.LocationScoreTolerance;
                intToleBackgroundArea = (int)objOcvText.BackgroundAreaTolerance;
                intToleForegroundArea = (int)objOcvText.ForegroundAreaTolerance;
                fToleBackgroundSum = objOcvText.BackgroundSumTolerance;
                fToleForegroundSum = objOcvText.ForegroundSumTolerance;

                fToleCorrelation = objOcvText.CorrelationTolerance;

                fShiftXTole = objOcvText.ShiftXTolerance;
                fShiftXMeasure = objOcvText.ShiftX;

                fShiftYTole = objOcvText.ShiftYTolerance;
                fShiftYMeasure = objOcvText.ShiftY;
            }
            else
                return false;

            return true;
        }

        public bool GetCharAnalyisResult(ref float fTempLocationScore, ref int intTempBackgroundArea, ref int intTempForegroundArea, ref float fTempBackgroundSum,
            ref float fTempForegroundSum, ref float fSampLocationScore, ref int intSampBackgroundArea, ref int intSampForegroundArea, ref float fSampBackgroundSum,
            ref float fSampForegroundSum, ref float fSampCorrelation, ref float fToleLocationScore, ref int intToleBackgroundArea, ref int intToleForegroundArea,
            ref float fToleBackgroundSum, ref float fToleForegroundSum, ref float fToleCorrelation, ref float fShiftXTole, ref float fShiftXMeasure, ref float fShiftYTole,
            ref float fShiftYMeasure, int intCharNo)
        {
            if (m_arrSampleChar.Count == 0)
                return false;

            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intCenterX, intCenterY;
            intCenterX = intCenterY = 0;
            int intIndex = 0;
            EOCVChar objOcvChar = new EOCVChar();
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
             for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    if (m_arrCharNo[intIndex] == intCharNo)
                    {
                        if (m_arrSampleChar[intCharNo].intLocMode == 1)
                        {
                            m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                            m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                            m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                            m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        }
                        else if (m_arrSampleChar[intCharNo].intLocMode == 2)
                        {
                            m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                            m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                            m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        }
                        else
                        {
                            m_MinOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                            m_MinOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                            m_MinOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                            m_MinOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        }

                        if (objOcvChar.TemplateBackgroundArea != -1 && objOcvChar.TemplateBackgroundArea != uint.MaxValue)
                        {
                            fTempLocationScore = objOcvChar.TemplateLocationScore;
                            intTempBackgroundArea = (int)objOcvChar.TemplateBackgroundArea;
                            intTempForegroundArea = (int)objOcvChar.TemplateForegroundArea;
                            fTempBackgroundSum = objOcvChar.TemplateBackgroundSum;
                            fTempForegroundSum = objOcvChar.TemplateForegroundSum;

                            fSampLocationScore = objOcvChar.SampleLocationScore;
                            intSampBackgroundArea = (int)objOcvChar.SampleBackgroundArea;
                            intSampForegroundArea = (int)objOcvChar.SampleForegroundArea;
                            fSampBackgroundSum = objOcvChar.SampleBackgroundSum;
                            fSampForegroundSum = objOcvChar.SampleForegroundSum;

                            fSampCorrelation = objOcvChar.Correlation;

                            fToleLocationScore = objOcvChar.LocationScoreTolerance;
                            intToleBackgroundArea = (int)objOcvChar.BackgroundAreaTolerance;
                            intToleForegroundArea = (int)objOcvChar.ForegroundAreaTolerance;
                            fToleBackgroundSum = objOcvChar.BackgroundSumTolerance;
                            fToleForegroundSum = objOcvChar.ForegroundSumTolerance;

                            fToleCorrelation = objOcvChar.CorrelationTolerance;

                            fShiftXTole = objOcvChar.ShiftXTolerance;
                            fShiftXMeasure = objOcvChar.ShiftX;

                            fShiftYTole = objOcvChar.ShiftYTolerance;
                            fShiftYMeasure = objOcvChar.ShiftY;
                        }
                        else
                            return false;
                    }

                    intIndex++;
                }
            }

            return true;
        }

        public int GetCharNo(int intCharIndex)
        {
            if (intCharIndex >= m_arrCharNo.Count)
                return 0;

            return m_arrCharNo[intCharIndex];
        }

        public List<int> GetCharNo()
        {
            return m_arrCharNo;
        }

        public int GetSelectedChar(int intMousePositionX, int intMousePositionY)
        {
            int intNumChars = m_arrSampleChar.Count;
            int intStartX, intStartY, intEndX, intEndY;

            for (int i = 0; i < intNumChars; i++)
            {
                intStartX = m_arrSampleChar[i].intOrgX;
                intStartY = m_arrSampleChar[i].intOrgY;
                intEndX = m_arrSampleChar[i].intEndX;
                intEndY = m_arrSampleChar[i].intEndY;

                if (intMousePositionX >= intStartX && intMousePositionX <= intEndX && intMousePositionY >= intStartY && intMousePositionY <= intEndY)
                    return i;
            }

            return -1;
        }

        public int GetSelectedText(int intMousePositionX, int intMousePositionY)
        {
            int intNumText = m_arrSampleText.Count;
            int intStartX, intStartY, intEndX, intEndY;

            for (int i = 0; i < intNumText; i++)
            {
                intStartX = m_arrSampleText[i].intOrgX;
                intStartY = m_arrSampleText[i].intOrgY;
                intEndX = m_arrSampleText[i].intEndX;
                intEndY = m_arrSampleText[i].intEndY;

                if (intMousePositionX >= intStartX && intMousePositionX <= intEndX && intMousePositionY >= intStartY && intMousePositionY <= intEndY)
                    return i;
            }

            return -1;
        }

        public List<int> GetMatchCharIndexes(float fStartX, float fStartY, float fEndX, float fEndY)
        {
            /*
             * Match Sample Blobs Rectangle Points with Sample OcvChar Rectangle Points.
             * 
             */
            List<int> arrMatch = new List<int>();

            int intNumChars = m_arrSampleChar.Count;
            int intStartX, intStartY, intEndX, intEndY;

            for (int i = 0; i < intNumChars; i++)
            {
                intStartX = m_arrSampleChar[i].intOrgX;
                intStartY = m_arrSampleChar[i].intOrgY;
                intEndX = m_arrSampleChar[i].intEndX;
                intEndY = m_arrSampleChar[i].intEndY;


                #region Sometime sample char scale is different than ocv char, reduce the ocv char size to prevent sample char match to wrong ocv char.
                // if char size bigger than 20 pixels, adjust ocv char size smaller 5 pixels from edge.
                if ((int)Math.Abs(intEndX - intStartX) > 20)
                {
                    intStartX += 5;
                    intEndX -= 5;
                }
                // if char size bigger than 12 pixels, adjust ocv char size smaller 3 pixels from edge.
                else if ((int)Math.Abs(intEndX - intStartX) > 12)
                {
                    intStartX += 3;
                    intEndX -= 3;
                }

                // if char size bigger than 20 pixels, adjust ocv char size smaller 5 pixels from edge.
                if ((int)Math.Abs(intEndY - intStartY) > 20)
                {
                    intStartY += 5;
                    intEndY -= 5;
                }
                // if char size bigger than 12 pixels, adjust ocv char size smaller 3 pixels from edge.
                else if ((int)Math.Abs(intEndY - intStartY) > 12)
                {
                    intStartY += 3;
                    intEndY -= 3;
                }
                #endregion

                if (((intStartX > fStartX) && (intStartX < fEndX) && (intStartY > fStartY) && (intStartY < fEndY)) ||
                    ((intStartX > fStartX) && (intStartX < fEndX) && (intEndY > fStartY) && (intEndY < fEndY)) ||
                    ((intEndX > fStartX) && (intEndX < fEndX) && (intStartY > fStartY) && (intStartY < fEndY)) ||
                    ((intEndX > fStartX) && (intEndX < fEndX) && (intEndY > fStartY) && (intEndY < fEndY)) ||
                    ((fStartX > intStartX) && (fStartX < intEndX) && (fStartY > intStartY) && (fStartY < intEndY)) ||
                    ((fStartX > intStartX) && (fStartX < intEndX) && (fEndY > intStartY) && (fEndY < intEndY)) ||
                    ((fEndX > intStartX) && (fEndX < intEndX) && (fStartY > intStartY) && (fStartY < intEndY)) ||
                    ((fEndX > intStartX) && (fEndX < intEndX) && (fEndY > intStartY) && (fEndY < intEndY)) ||
                    ((intStartX > fStartX) && (intStartX < fEndX) && (intStartY < fStartY) && (intEndY > fEndY)) ||   // rectange body close each other without match any center point.
                    ((intStartY > fStartY) && (intStartY < fEndY) && (intStartX < fStartX) && (intEndX > fEndX))     // rectange body close each other without match any center point.
                    )
                {
                    //arrMatch.Add(m_arrCharNo[i]);
                    arrMatch.Add(i);
                }

            }

            return arrMatch;
        }

        public List<int> GetMatchCharIndexes(float fStartX, float fStartY, float fEndX, float fEndY, float fCenterX, float fCenterY, float fWidth, float fHeight, ref bool blnContourCheck)
        {
            /*
             * Match Sample Blobs Rectangle Points with Sample OcvChar Rectangle Points.
             * 
             */
            List<int> arrMatch = new List<int>();

            int intNumChars = m_arrSampleChar.Count;
            int intStartX, intStartY, intEndX, intEndY;

            for (int i = 0; i < intNumChars; i++)
            {
                intStartX = m_arrSampleChar[i].intOrgX;
                intStartY = m_arrSampleChar[i].intOrgY;
                intEndX = m_arrSampleChar[i].intEndX;
                intEndY = m_arrSampleChar[i].intEndY;


                #region Sometime sample char scale is different than ocv char, reduce the ocv char size to prevent sample char match to wrong ocv char.
                // if char size bigger than 20 pixels, adjust ocv char size smaller 5 pixels from edge.
                if ((int)Math.Abs(intEndX - intStartX) > 20)
                {
                    intStartX += 5;
                    intEndX -= 5;
                }
                // if char size bigger than 12 pixels, adjust ocv char size smaller 3 pixels from edge.
                else if ((int)Math.Abs(intEndX - intStartX) > 12)
                {
                    intStartX += 3;
                    intEndX -= 3;
                }

                // if char size bigger than 20 pixels, adjust ocv char size smaller 5 pixels from edge.
                if ((int)Math.Abs(intEndY - intStartY) > 20)
                {
                    intStartY += 5;
                    intEndY -= 5;
                }
                // if char size bigger than 12 pixels, adjust ocv char size smaller 3 pixels from edge.
                else if ((int)Math.Abs(intEndY - intStartY) > 12)
                {
                    intStartY += 3;
                    intEndY -= 3;
                }
                #endregion

                // 2018 12 31 - CCENG: add = to logic below because without =, there is weakness of logic below will miss checking when both border are same point.
                if (((intStartX >= fStartX) && (intStartX <= fEndX) && (intStartY >= fStartY) && (intStartY <= fEndY)) ||
                    ((intStartX >= fStartX) && (intStartX <= fEndX) && (intEndY >= fStartY) && (intEndY <= fEndY)) ||
                    ((intEndX >= fStartX) && (intEndX <= fEndX) && (intStartY >= fStartY) && (intStartY <= fEndY)) ||
                    ((intEndX >= fStartX) && (intEndX <= fEndX) && (intEndY >= fStartY) && (intEndY <= fEndY)) ||
                    ((fStartX >= intStartX) && (fStartX <= intEndX) && (fStartY >= intStartY) && (fStartY <= intEndY)) ||
                    ((fStartX >= intStartX) && (fStartX <= intEndX) && (fEndY >= intStartY) && (fEndY <= intEndY)) ||
                    ((fEndX >= intStartX) && (fEndX <= intEndX) && (fStartY >= intStartY) && (fStartY <= intEndY)) ||
                    ((fEndX >= intStartX) && (fEndX <= intEndX) && (fEndY >= intStartY) && (fEndY <= intEndY)) ||
                    ((intStartX > fStartX) && (intStartX < fEndX) && (intStartY < fStartY) && (intEndY > fEndY)) ||   // rectange body close each other without match any center point.
                    ((intStartY > fStartY) && (intStartY < fEndY) && (intStartX < fStartX) && (intEndX > fEndX))     // rectange body close each other without match any center point.
                    )
                {
                    //arrMatch.Add(m_arrCharNo[i]);
                    arrMatch.Add(i);

                    // 2020-02-19 ZJYEOH : Check Contour is now set by user in advance setting form
                    //if (!blnContourCheck)
                    //{
                    //    if ((Math.Abs(fWidth - m_arrSampleChar[i].intWidth) > (m_arrSampleChar[i].intWidth / 2)) ||
                    //        (Math.Abs(fHeight - m_arrSampleChar[i].intHeight) > (m_arrSampleChar[i].intHeight / 2)) ||
                    //        ((Math.Abs(fCenterX) - (m_arrSampleChar[i].intOrgX + m_arrSampleChar[i].intWidth / 2)) > (m_arrSampleChar[i].intWidth / 2)) ||
                    //        ((Math.Abs(fCenterY) - (m_arrSampleChar[i].intOrgY + m_arrSampleChar[i].intHeight / 2)) > (m_arrSampleChar[i].intHeight / 2)))
                    //    {
                    //        blnContourCheck = true;
                    //    }
                    //}

                }

            }

            return arrMatch;
        }
        public List<int> GetMatchCharIndexes(float fStartX, float fStartY, float fEndX, float fEndY, float fCenterX, float fCenterY, float fWidth, float fHeight, ref bool blnContourCheck,
                                             List<float> arrShiftX, List<float> arrShiftY, float fCharROIOffsetX, float fCharROIOffsetY, bool blnWantCheckExcessWithinOCVAreaOnly)
        {
            /*
             * Match Sample Blobs Rectangle Points with Sample OcvChar Rectangle Points.
             * 
             */
            List<int> arrMatch = new List<int>();

            int intNumChars = m_arrSampleChar.Count;
            int intStartX, intStartY, intEndX, intEndY;

            for (int i = 0; i < intNumChars; i++)
            {
                intStartX = m_arrSampleChar[i].intOrgX;
                intStartY = m_arrSampleChar[i].intOrgY;
                intEndX = m_arrSampleChar[i].intEndX;
                intEndY = m_arrSampleChar[i].intEndY;
                
                #region Sometime sample char scale is different than ocv char, reduce the ocv char size to prevent sample char match to wrong ocv char.
                // if char size bigger than 20 pixels, adjust ocv char size smaller 5 pixels from edge.
                if ((int)Math.Abs(intEndX - intStartX) > 20)
                {
                    intStartX += 5;
                    intEndX -= 5;
                }
                // if char size bigger than 12 pixels, adjust ocv char size smaller 3 pixels from edge.
                else if ((int)Math.Abs(intEndX - intStartX) > 12)
                {
                    intStartX += 3;
                    intEndX -= 3;
                }

                // if char size bigger than 20 pixels, adjust ocv char size smaller 5 pixels from edge.
                if ((int)Math.Abs(intEndY - intStartY) > 20)
                {
                    intStartY += 5;
                    intEndY -= 5;
                }
                // if char size bigger than 12 pixels, adjust ocv char size smaller 3 pixels from edge.
                else if ((int)Math.Abs(intEndY - intStartY) > 12)
                {
                    intStartY += 3;
                    intEndY -= 3;
                }
                #endregion

                if (!blnWantCheckExcessWithinOCVAreaOnly)
                {
                    intStartX -= ((int)Math.Round(arrShiftX[i] + fCharROIOffsetX));
                    intStartY -= ((int)Math.Round(arrShiftY[i] + fCharROIOffsetY));
                    intEndX += ((int)Math.Round(arrShiftX[i] + fCharROIOffsetX));
                    intEndY += ((int)Math.Round(arrShiftY[i] + fCharROIOffsetY));
                }

                // 2018 12 31 - CCENG: add = to logic below because without =, there is weakness of logic below will miss checking when both border are same point.
                if (((intStartX >= fStartX) && (intStartX <= fEndX) && (intStartY >= fStartY) && (intStartY <= fEndY)) ||
                    ((intStartX >= fStartX) && (intStartX <= fEndX) && (intEndY >= fStartY) && (intEndY <= fEndY)) ||
                    ((intEndX >= fStartX) && (intEndX <= fEndX) && (intStartY >= fStartY) && (intStartY <= fEndY)) ||
                    ((intEndX >= fStartX) && (intEndX <= fEndX) && (intEndY >= fStartY) && (intEndY <= fEndY)) ||
                    ((fStartX >= intStartX) && (fStartX <= intEndX) && (fStartY >= intStartY) && (fStartY <= intEndY)) ||
                    ((fStartX >= intStartX) && (fStartX <= intEndX) && (fEndY >= intStartY) && (fEndY <= intEndY)) ||
                    ((fEndX >= intStartX) && (fEndX <= intEndX) && (fStartY >= intStartY) && (fStartY <= intEndY)) ||
                    ((fEndX >= intStartX) && (fEndX <= intEndX) && (fEndY >= intStartY) && (fEndY <= intEndY)) ||
                    ((intStartX > fStartX) && (intStartX < fEndX) && (intStartY < fStartY) && (intEndY > fEndY)) ||   // rectange body close each other without match any center point.
                    ((intStartY > fStartY) && (intStartY < fEndY) && (intStartX < fStartX) && (intEndX > fEndX))     // rectange body close each other without match any center point.
                    )
                {
                    //arrMatch.Add(m_arrCharNo[i]);
                    arrMatch.Add(i);

                    // 2020-02-19 ZJYEOH : Check Contour is now set by user in advance setting form
                    //if (!blnContourCheck)
                    //{
                    //    if ((Math.Abs(fWidth - m_arrSampleChar[i].intWidth) > (m_arrSampleChar[i].intWidth / 2)) ||
                    //        (Math.Abs(fHeight - m_arrSampleChar[i].intHeight) > (m_arrSampleChar[i].intHeight / 2)) ||
                    //        ((Math.Abs(fCenterX) - (m_arrSampleChar[i].intOrgX + m_arrSampleChar[i].intWidth / 2)) > (m_arrSampleChar[i].intWidth / 2)) ||
                    //        ((Math.Abs(fCenterY) - (m_arrSampleChar[i].intOrgY + m_arrSampleChar[i].intHeight / 2)) > (m_arrSampleChar[i].intHeight / 2)))
                    //    {
                    //        blnContourCheck = true;
                    //    }
                    //}

                }

            }

            return arrMatch;
        }
        public List<int> GetMatchCharIndexes_Rotate(float fStartX, float fStartY, float fEndX, float fEndY, float fCenterX, float fCenterY, float fWidth, float fHeight,
            ref bool blnContourCheck, List<float> arrShiftX, List<float> arrShiftY, float fCharROIOffsetX, float fCharROIOffsetY, bool blnWantCheckExcessWithinOCVAreaOnly, int intROIOffSetX, int intROIOffSetY, float fMarkAngle
            , List<PointF> arrCharStartPoint_RotateTo0Deg, List<PointF> arrCharEndPoint_RotateTo0Deg)
        {
            /*
             * Match Sample Blobs Rectangle Points with Sample OcvChar Rectangle Points.
             * 
             */
            List<int> arrMatch = new List<int>();

            int intNumChars = m_arrSampleChar.Count;
            int intStartX, intStartY, intEndX, intEndY;
            float fStartX_Rotated, fStartY_Rotated, fEndX_Rotated, fEndY_Rotated;
            float newSX = 0, newSY = 0, newEX = 0, newEY = 0;

            for (int i = 0; i < intNumChars; i++)
            {
                intStartX = (int)Math.Round(arrCharStartPoint_RotateTo0Deg[i].X);// intROIOffSetX + m_arrSampleChar[i].intOrgX;
                intStartY = (int)Math.Round(arrCharStartPoint_RotateTo0Deg[i].Y);//intROIOffSetY + m_arrSampleChar[i].intOrgY;
                intEndX = (int)Math.Round(arrCharEndPoint_RotateTo0Deg[i].X);//intROIOffSetX + m_arrSampleChar[i].intEndX;
                intEndY = (int)Math.Round(arrCharEndPoint_RotateTo0Deg[i].Y);//intROIOffSetY + m_arrSampleChar[i].intEndY;

                //Math2.GetNewXYAfterRotate_360deg((intEndX + intStartX) / 2, (intEndY + intStartY) / 2, intStartX, intStartY, -fMarkAngle, ref newSX, ref newSY);

                //Math2.GetNewXYAfterRotate_360deg((intEndX + intStartX) / 2, (intEndY + intStartY) / 2, intEndX, intEndY, -fMarkAngle, ref newEX, ref newEY);
                ////Point pStart = new Point((int)Math.Round(intROIOffSetX + newSX), (int)Math.Round(intROIOffSetY + newSY));
                ////Point pEnd = new Point((int)Math.Round(intROIOffSetX + newEX), (int)Math.Round(intROIOffSetY + newEY));
                //intStartX = (int)Math.Round(newSX);
                //intStartY = (int)Math.Round(newSY);
                //intEndX = (int)Math.Round(newEX);
                //intEndY = (int)Math.Round(newEY);

                //#region Sometime sample char scale is different than ocv char, reduce the ocv char size to prevent sample char match to wrong ocv char.
                //// if char size bigger than 20 pixels, adjust ocv char size smaller 5 pixels from edge.
                //if ((int)Math.Abs(intEndX - intStartX - intROIOffSetX) > 20)
                //{
                //    intStartX += 5;
                //    intEndX -= 5;
                //}
                //// if char size bigger than 12 pixels, adjust ocv char size smaller 3 pixels from edge.
                //else if ((int)Math.Abs(intEndX - intStartX - intROIOffSetX) > 12)
                //{
                //    intStartX += 3;
                //    intEndX -= 3;
                //}

                //// if char size bigger than 20 pixels, adjust ocv char size smaller 5 pixels from edge.
                //if ((int)Math.Abs(intEndY - intStartY - intROIOffSetY) > 20)
                //{
                //    intStartY += 5;
                //    intEndY -= 5;
                //}
                //// if char size bigger than 12 pixels, adjust ocv char size smaller 3 pixels from edge.
                //else if ((int)Math.Abs(intEndY - intStartY - intROIOffSetY) > 12)
                //{
                //    intStartY += 3;
                //    intEndY -= 3;
                //}
                //#endregion


                if (!blnWantCheckExcessWithinOCVAreaOnly)
                {
                    intStartX -= ((int)Math.Round(arrShiftX[i] + fCharROIOffsetX));
                    intStartY -= ((int)Math.Round(arrShiftY[i] + fCharROIOffsetY));
                    intEndX += ((int)Math.Round(arrShiftX[i] + fCharROIOffsetX));
                    intEndY += ((int)Math.Round(arrShiftY[i] + fCharROIOffsetY));
                }

                fStartX_Rotated = intROIOffSetX + fStartX;
                fEndX_Rotated = intROIOffSetX + fEndX;
                fStartY_Rotated = intROIOffSetY + fStartY;
                fEndY_Rotated = intROIOffSetY + fEndY;

                Math2.GetNewXYAfterRotate_360deg((arrCharEndPoint_RotateTo0Deg[i].X + arrCharStartPoint_RotateTo0Deg[i].X) / 2, (arrCharEndPoint_RotateTo0Deg[i].Y + arrCharStartPoint_RotateTo0Deg[i].Y) / 2, fStartX_Rotated, fStartY_Rotated, -fMarkAngle, ref newSX, ref newSY);

                Math2.GetNewXYAfterRotate_360deg((arrCharEndPoint_RotateTo0Deg[i].X + arrCharStartPoint_RotateTo0Deg[i].X) / 2, (arrCharEndPoint_RotateTo0Deg[i].Y + arrCharStartPoint_RotateTo0Deg[i].Y) / 2, fEndX_Rotated, fEndY_Rotated, -fMarkAngle, ref newEX, ref newEY);
                //Point pStart = new Point((int)Math.Round(intROIOffSetX + newSX), (int)Math.Round(intROIOffSetY + newSY));
                //Point pEnd = new Point((int)Math.Round(intROIOffSetX + newEX), (int)Math.Round(intROIOffSetY + newEY));
                fStartX_Rotated = newSX;
                fStartY_Rotated = newSY;
                fEndX_Rotated = newEX;
                fEndY_Rotated = newEY;

                // 2018 12 31 - CCENG: add = to logic below because without =, there is weakness of logic below will miss checking when both border are same point.
                if (((intStartX >= fStartX_Rotated) && (intStartX <= fEndX_Rotated) && (intStartY >= fStartY_Rotated) && (intStartY <= fEndY_Rotated)) ||
                    ((intStartX >= fStartX_Rotated) && (intStartX <= fEndX_Rotated) && (intEndY >= fStartY_Rotated) && (intEndY <= fEndY_Rotated)) ||
                    ((intEndX >= fStartX_Rotated) && (intEndX <= fEndX_Rotated) && (intStartY >= fStartY_Rotated) && (intStartY <= fEndY_Rotated)) ||
                    ((intEndX >= fStartX_Rotated) && (intEndX <= fEndX_Rotated) && (intEndY >= fStartY_Rotated) && (intEndY <= fEndY_Rotated)) ||
                    ((fStartX_Rotated >= intStartX) && (fStartX_Rotated <= intEndX) && (fStartY_Rotated >= intStartY) && (fStartY_Rotated <= intEndY)) ||
                    ((fStartX_Rotated >= intStartX) && (fStartX_Rotated <= intEndX) && (fEndY_Rotated >= intStartY) && (fEndY_Rotated <= intEndY)) ||
                    ((fEndX_Rotated >= intStartX) && (fEndX_Rotated <= intEndX) && (fStartY_Rotated >= intStartY) && (fStartY_Rotated <= intEndY)) ||
                    ((fEndX_Rotated >= intStartX) && (fEndX_Rotated <= intEndX) && (fEndY_Rotated >= intStartY) && (fEndY_Rotated <= intEndY)) ||
                    ((intStartX > fStartX_Rotated) && (intStartX < fEndX_Rotated) && (intStartY < fStartY_Rotated) && (intEndY > fEndY_Rotated)) ||   // rectange body close each other without match any center point.
                    ((intStartY > fStartY_Rotated) && (intStartY < fEndY_Rotated) && (intStartX < fStartX_Rotated) && (intEndX > fEndX_Rotated))     // rectange body close each other without match any center point.
                    )
                {
                    //arrMatch.Add(m_arrCharNo[i]);
                    arrMatch.Add(i);

                    // 2020-02-19 ZJYEOH : Check Contour is now set by user in advance setting form
                    //if (!blnContourCheck)
                    //{
                    //    if ((Math.Abs(fWidth - m_arrSampleChar[i].intWidth) > (m_arrSampleChar[i].intWidth / 2)) ||
                    //        (Math.Abs(fHeight - m_arrSampleChar[i].intHeight) > (m_arrSampleChar[i].intHeight / 2)) ||
                    //        ((Math.Abs(fCenterX) - (m_arrSampleChar[i].intOrgX + m_arrSampleChar[i].intWidth / 2)) > (m_arrSampleChar[i].intWidth / 2)) ||
                    //        ((Math.Abs(fCenterY) - (m_arrSampleChar[i].intOrgY + m_arrSampleChar[i].intHeight / 2)) > (m_arrSampleChar[i].intHeight / 2)))
                    //    {
                    //        blnContourCheck = true;
                    //    }
                    //}

                }

            }

            return arrMatch;
        }
        public int GetNumChars()
        {
            int intNumText = (int)m_Ocv.NumTexts;
            int intNumChars = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumText; tx++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumText; tx++)
#endif

            {
                intNumChars += (int)m_Ocv.GetNumTextChars(tx);
            }

            return intNumChars;
        }

        public int GetNumTexts()
        {
            return (int)m_Ocv.NumTexts;
        }

        public int GetSampleText()
        {
            return m_arrSampleText.Count;
        }

        public int HitChar(int intX, int intY)
        {
            for (int i = 0; i < m_arrSampleChar.Count; i++)
            {
                if ((intX > m_arrSampleChar[i].intOrgX) && (intX < m_arrSampleChar[i].intEndX) &&
                    (intY > m_arrSampleChar[i].intOrgY) && (intY < m_arrSampleChar[i].intEndY))
                {
                    return i;
                }
            }

            return -1;
        }

        public int HitText(int intX, int intY)
        {
            for (int i = 0; i < m_arrSampleText.Count; i++)
            {
                if ((intX > m_arrSampleText[i].intOrgX) && (intX < m_arrSampleText[i].intEndX) &&
                    (intY > m_arrSampleText[i].intOrgY) && (intY < m_arrSampleText[i].intEndY))
                {
                    return i;
                }
            }

            return -1;
        }

        public float[] GetCharsShiftX()
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intNumChars = 0;
            float[] fCharShiftX;
            int intCenterX, intCenterY;
            int intCharIndex = 0;
            EOCVChar objOcvChar = new EOCVChar();
#if (Debug_2_12 || Release_2_12)
            // Get chars count in all texts
            for (uint tx = 0; tx < intNumTexts; tx++)
                intNumChars += (int)m_Ocv.GetNumTextChars(tx);

            fCharShiftX = new float[intNumChars];

            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumTextChars; ch++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Get chars count in all texts
                for (int tx = 0; tx < intNumTexts; tx++)
                intNumChars += m_Ocv.GetNumTextChars(tx);

            fCharShiftX = new float[intNumChars];

            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumTextChars; ch++)
#endif

                {
                intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                    if ((int)objOcvChar.ShiftXTolerance >= 0)
                        fCharShiftX[intCharIndex] = objOcvChar.ShiftXTolerance;
                    else
                        fCharShiftX[intCharIndex] = 5;
                    intCharIndex++;
                }
            }

            return fCharShiftX;
        }

        public float[] GetCharsShiftY()
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intNumChars = 0;
            float[] fCharShiftY;
            int intCenterX, intCenterY;
            int intCharIndex = 0;
            EOCVChar objOcvChar = new EOCVChar();
#if (Debug_2_12 || Release_2_12)
            // Get chars count
            for (uint tx = 0; tx < intNumTexts; tx++)
                intNumChars += (int)m_Ocv.GetNumTextChars(tx);

            fCharShiftY = new float[intNumChars];

            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Get chars count
            for (int tx = 0; tx < intNumTexts; tx++)
                intNumChars += m_Ocv.GetNumTextChars(tx);

            fCharShiftY = new float[intNumChars];

            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
#endif

                {
                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    if ((int)objOcvChar.ShiftYTolerance >= 0)
                        fCharShiftY[intCharIndex] = objOcvChar.ShiftYTolerance;
                    else
                        fCharShiftY[intCharIndex] = 5;
                    intCharIndex++;
                }
            }

            return fCharShiftY;
        }

        public int GetCharForeArea(int intCharIndex)
        {
            if (intCharIndex >= m_arrSampleChar.Count)
                return -1;

            return m_arrSampleChar[intCharIndex].intForeArea;
        }
        public float GetCharForeAreaSumPercent(int intCharIndex)
        {
            if (intCharIndex >= m_arrSampleChar.Count)
                return -999;

            return m_arrSampleChar[intCharIndex].fForeAreaSumPercent;
        }
        public System.Drawing.Point GetCharEndXY(int intCharIndex)
        {
            return new System.Drawing.Point(m_arrSampleChar[intCharIndex].intEndX, m_arrSampleChar[intCharIndex].intEndY);
        }

        public System.Drawing.Point GetCharStartXY(int intCharIndex)
        {
            return new System.Drawing.Point(m_arrSampleChar[intCharIndex].intOrgX, m_arrSampleChar[intCharIndex].intOrgY);
        }

        public System.Drawing.Point GetTemplateCharEndXY(int intCharIndex)
        {
            int intEndX, intEndY, intLoopIndex;
            intEndX = intEndY = -1;
            intLoopIndex = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint i = 0; i < m_Ocv.NumTexts; i++)
            {
                for (uint j = 0; j < m_Ocv.GetNumTextChars(i); j++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int i = 0; i < m_Ocv.NumTexts; i++)
            {
                for (int j = 0; j < m_Ocv.GetNumTextChars(i); j++)
                {
#endif

                    if (intLoopIndex < m_arrCharNo.Count)
                    {
                        if (m_arrCharNo[intLoopIndex] == intCharIndex)
                        {
                            m_Ocv.GetTextCharPoint(i, j, out intEndX, out intEndY, 1, 1);
                            goto returnStatement;
                        }

                        intLoopIndex++;
                    }
                    else
                        goto returnStatement;
                }

            }

            returnStatement: return new System.Drawing.Point(intEndX, intEndY);
        }

        public void GetBuildOCVPosition(ref List<int> arrStartX, ref List<int> arrStartY, ref List<int> arrEndX, ref List<int> arrEndY)
        {
            arrStartX.Clear();
            arrStartY.Clear();
            arrEndX.Clear();
            arrEndY.Clear();

            int intStartX = 0, intStartY = 0, intEndX = 0, intEndY = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint i = 0; i < m_Ocv.NumTexts; i++)
            {
                for (uint j = 0; j < m_Ocv.GetNumTextChars(i); j++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int i = 0; i < m_Ocv.NumTexts; i++)
            {
                for (int j = 0; j < m_Ocv.GetNumTextChars(i); j++)
#endif

                {
                        m_Ocv.GetTextCharPoint(i, j, out intStartX, out intStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(i, j, out intEndX, out intEndY, 1, 1);

                    arrStartX.Add(intStartX);
                    arrStartY.Add(intStartY);
                    arrEndX.Add(intEndX);
                    arrEndY.Add(intEndY);
                }
            }
        }

        public void GetBuildOCVPosition(ref List<int> arrCenterX, ref List<int> arrCenterY)
        {
            arrCenterX.Clear();
            arrCenterY.Clear();

            int intStartX = 0, intStartY = 0, intEndX = 0, intEndY = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint i = 0; i < m_Ocv.NumTexts; i++)
            {
                for (uint j = 0; j < m_Ocv.GetNumTextChars(i); j++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int i = 0; i < m_Ocv.NumTexts; i++)
            {
                for (int j = 0; j < m_Ocv.GetNumTextChars(i); j++)
                {
#endif

                    m_Ocv.GetTextCharPoint(i, j, out intStartX, out intStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(i, j, out intEndX, out intEndY, 1, 1);

                    arrCenterX.Add((intStartX + intEndX) / 2);
                    arrCenterY.Add((intStartY + intEndY) / 2);
                }
            }
        }

        public void GetBuildOCVPosition(ref List<int> arrStartX, ref List<int> arrStartY,
                                        ref List<int> arrEndX, ref List<int> arrEndY,
                                        ref List<int> arrCenterX, ref List<int> arrCenterY)
        {
            arrStartX.Clear();
            arrStartY.Clear();
            arrEndX.Clear();
            arrEndY.Clear();

            int intStartX = 0, intStartY = 0, intEndX = 0, intEndY = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint i = 0; i < m_Ocv.NumTexts; i++)
            {
                for (uint j = 0; j < m_Ocv.GetNumTextChars(i); j++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
             for (int i = 0; i < m_Ocv.NumTexts; i++)
            {
                for (int j = 0; j < m_Ocv.GetNumTextChars(i); j++)
#endif

                {
                        m_Ocv.GetTextCharPoint(i, j, out intStartX, out intStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(i, j, out intEndX, out intEndY, 1, 1);

                    arrStartX.Add(intStartX);
                    arrStartY.Add(intStartY);
                    arrEndX.Add(intEndX);
                    arrEndY.Add(intEndY);
                    arrCenterX.Add((intStartX + intEndX) / 2);
                    arrCenterY.Add((intStartY + intEndY) / 2);
                }
            }
        }

        public System.Drawing.Point GetTemplateCharStartXY(int intCharIndex)
        {
            int intStartX, intStartY, intLoopIndex;
            intStartX = intStartY = -1;
            intLoopIndex = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint i = 0; i < m_Ocv.NumTexts; i++)
            {
                for (uint j = 0; j < m_Ocv.GetNumTextChars(i); j++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int i = 0; i < m_Ocv.NumTexts; i++)
            {
                for (int j = 0; j < m_Ocv.GetNumTextChars(i); j++)
                {
#endif

                    if (intLoopIndex < m_arrCharNo.Count)
                    {
                        if (m_arrCharNo[intLoopIndex] == intCharIndex)
                        {
                            m_Ocv.GetTextCharPoint(i, j, out intStartX, out intStartY, -1, -1);
                            goto returnStatement;
                        }

                        intLoopIndex++;
                    }
                    else
                        goto returnStatement;
                }

            }

            returnStatement: return new System.Drawing.Point(intStartX, intStartY);
        }

        public System.Drawing.Point GetTextEndXY()
        {
            int intNumTexts = (int)m_Ocv.NumTexts;

            if (intNumTexts == 0)
                return new System.Drawing.Point(-1, -1);

            System.Drawing.Point pEndPoint = new System.Drawing.Point(0, 0);
            for (int i = 0; i < m_arrSampleText.Count; i++)
            {
                if (m_arrSampleText[i].intEndX > pEndPoint.X)
                    pEndPoint.X = m_arrSampleText[i].intEndX;

                if (m_arrSampleText[i].intEndY > pEndPoint.Y)
                    pEndPoint.Y = m_arrSampleText[i].intEndY;
            }

            return pEndPoint;
        }

        public System.Drawing.Point GetTextEndXY(int intTextIndex)
        {
            if (intTextIndex < m_arrSampleText.Count)
                return new System.Drawing.Point(m_arrSampleText[intTextIndex].intEndX, m_arrSampleText[intTextIndex].intEndY);
            else
                return new System.Drawing.Point(-1, -1);
        }

        public System.Drawing.Point GetTextStartXY()
        {
            int intNumTexts = (int)m_Ocv.NumTexts;

            if (intNumTexts == 0)
                return new System.Drawing.Point(-1, -1);

            System.Drawing.Point pStartPoint = new System.Drawing.Point(m_arrSampleText[0].intOrgX, m_arrSampleText[0].intOrgY);
            for (int i = 1; i < m_arrSampleText.Count; i++)
            {
                if (m_arrSampleText[i].intOrgX < pStartPoint.X)
                    pStartPoint.X = m_arrSampleText[i].intOrgX;

                if (m_arrSampleText[i].intOrgY < pStartPoint.Y)
                    pStartPoint.Y = m_arrSampleText[i].intOrgY;
            }

            return pStartPoint;
        }

        public System.Drawing.Point GetTextStartXY(int intTextIndex)
        {
            if (intTextIndex < m_arrSampleText.Count)
                return new System.Drawing.Point(m_arrSampleText[intTextIndex].intOrgX, m_arrSampleText[intTextIndex].intOrgY);
            else
                return new System.Drawing.Point(-1, -1);
        }

        private void ArrangeCharByCharNo()
        {
            List<Sample> m_arrTempSampleChar = new List<Sample>();

            for (int j = 0; j < m_arrCharNo.Count; j++)
                m_arrTempSampleChar.Add(new Sample());

            for (int i = 0; i < m_arrCharNo.Count; i++)
            {
                m_arrTempSampleChar.RemoveAt(m_arrCharNo[i]);
                m_arrTempSampleChar.Insert(m_arrCharNo[i], m_arrSampleChar[i]);
            }

            m_arrSampleChar = m_arrTempSampleChar;
        }

        /// <summary>
        /// Create Template Characters
        /// </summary>
        /// <param name="intCreationMode">1: Form multi chars, 2: Form single chars, 3: Form auto chars</param>
        /// <returns></returns>
        public void BuildChars(Blobs objBlobs, int intCreationMode)
        {
            m_Ocv = new EOCV();

            // Clear previous objects, chars and Text
            m_Ocv.DeleteTemplateChars(ESelectionFlag.Any);
            m_Ocv.DeleteTemplateObjects(ESelectionFlag.Any);
            m_Ocv.DeleteTemplateTexts(ESelectionFlag.Any);
            m_Ocv.DeleteTemplateChars(ESelectionFlag.Any);
            m_Ocv.DeleteTemplateObjects(ESelectionFlag.Any);
            m_Ocv.DeleteTemplateTexts(ESelectionFlag.Any);

            // Set objects to ocv
            m_Ocv.CreateTemplateObjects(objBlobs.ref_Blob, ESelectionFlag.True);

            if (objBlobs.ref_intNumSelectedObject > 0)
            {
                // Create template chars
                if (intCreationMode == 1)
                    m_Ocv.CreateTemplateChars(ESelectionFlag.Any, ECharCreationMode.Separate);
                else if (intCreationMode == 2)
                    m_Ocv.CreateTemplateChars(ESelectionFlag.Any, ECharCreationMode.Group);
                else
                    m_Ocv.CreateTemplateChars(ESelectionFlag.Any, ECharCreationMode.Overlap);
            }

            m_BinaOcv = new EOCV();

            // Clear previous objects, chars and Text
            m_BinaOcv.DeleteTemplateChars(ESelectionFlag.Any);
            m_BinaOcv.DeleteTemplateObjects(ESelectionFlag.Any);
            m_BinaOcv.DeleteTemplateTexts(ESelectionFlag.Any);
            m_BinaOcv.DeleteTemplateChars(ESelectionFlag.Any);
            m_BinaOcv.DeleteTemplateObjects(ESelectionFlag.Any);
            m_BinaOcv.DeleteTemplateTexts(ESelectionFlag.Any);

            // Set objects to ocv
            m_BinaOcv.CreateTemplateObjects(objBlobs.ref_Blob, ESelectionFlag.True);

            if (objBlobs.ref_intNumSelectedObject > 0)
            {
                // Create template chars
                if (intCreationMode == 1)
                    m_BinaOcv.CreateTemplateChars(ESelectionFlag.Any, ECharCreationMode.Separate);
                else if (intCreationMode == 2)
                    m_BinaOcv.CreateTemplateChars(ESelectionFlag.Any, ECharCreationMode.Group);
                else
                    m_BinaOcv.CreateTemplateChars(ESelectionFlag.Any, ECharCreationMode.Overlap);
            }
        }

        public void BuildTexts()
        {
            m_Ocv.CreateTemplateTexts(ESelectionFlag.True);
        }

        public void DefineShiftTextTolerance(ROI objROI, ref int intXTolerance, ref int intYTolerance)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intStartX, intStartY, intEndX, intEndY;
            int intWidth, intHeight;
            int intTextWidth, intTextHeight;
            intTextWidth = intTextHeight = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
#endif

            {
                        intStartX = intStartY = intEndX = intEndY = 0;
                m_Ocv.GetTextPoint(tx, out intStartX, out intStartY, -1, -1);
                m_Ocv.GetTextPoint(tx, out intEndX, out intEndY, 1, 1);
                intWidth = intEndX - intStartX;
                intHeight = intEndY - intStartY;
                if (intWidth > intTextWidth)
                    intTextWidth = intWidth;
                if (intHeight > intTextHeight)
                    intTextHeight = intHeight;
            }

            intXTolerance = (objROI.ref_ROIWidth - intTextWidth) / 2;
            intYTolerance = (objROI.ref_ROIHeight - intTextHeight) / 2;
        }

        public void DeleteSample()
        {
            m_arrSampleText.Clear();
            m_arrSampleChar.Clear();
        }

        public void DeleteTemplateChars()
        {
            m_Ocv.DeleteTemplateChars(ESelectionFlag.Any);
        }

        public void DeleteTemplateTexts()
        {
            m_Ocv.DeleteTemplateTexts(ESelectionFlag.Any);
        }

        public void DrawTemplateChars(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            m_Ocv.DrawTemplateObjects(g, m_colorGreen, ESelectionFlag.True, fDrawingScaleX, fDrawingScaleY);
            m_Ocv.DrawTemplateObjects(g, m_colorRed, ESelectionFlag.False, fDrawingScaleX, fDrawingScaleY);
            try
            {
                m_Ocv.DrawTemplateChars(g, m_colorGreen, ESelectionFlag.True, fDrawingScaleX, fDrawingScaleY);
                m_Ocv.DrawTemplateChars(g, m_colorRed, ESelectionFlag.False, fDrawingScaleX, fDrawingScaleY);
            }
            catch
            {

            }
        }

        public void DrawTemplateTexts(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            m_Ocv.DrawTemplateChars(g, m_colorGreen, ESelectionFlag.True, fDrawingScaleX, fDrawingScaleY);
            m_Ocv.DrawTemplateChars(g, m_colorRed, ESelectionFlag.False, fDrawingScaleX, fDrawingScaleY);
            m_Ocv.DrawTemplateTexts(g, m_colorGreenText, ESelectionFlag.True, fDrawingScaleX, fDrawingScaleY);
            m_Ocv.DrawTemplateTexts(g, m_colorRed, ESelectionFlag.False, fDrawingScaleX, fDrawingScaleY);
        }

        public void FormMultiSelectedChars()
        {
            m_Ocv.DeleteTemplateChars(ESelectionFlag.True);
            m_Ocv.CreateTemplateChars(ESelectionFlag.True, ECharCreationMode.Separate);
        }

        public void FormSingleSelectedChars()
        {
            m_Ocv.DeleteTemplateChars(ESelectionFlag.True);
            m_Ocv.CreateTemplateChars(ESelectionFlag.True, ECharCreationMode.Group);
        }

        public void GetCharEndXY(int intCharIndex, ref int intEndX, ref int intEndY)
        {
            if (intCharIndex >= m_arrSampleChar.Count)
            {
                intEndX = 0;
                intEndY = 0;
            }
            else
            {
                intEndX = m_arrSampleChar[intCharIndex].intEndX;
                intEndY = m_arrSampleChar[intCharIndex].intEndY;
            }
        }

        public void GetCharStartXY(int intCharIndex, ref int intStartX, ref int intStartY)
        {
            if (intCharIndex >= m_arrSampleChar.Count)
            {
                intStartX = 0;
                intStartY = 0;
            }
            else
            {
                intStartX = m_arrSampleChar[intCharIndex].intOrgX;
                intStartY = m_arrSampleChar[intCharIndex].intOrgY;
            }
        }

        public void GetCharSize(int intCharIndex, ref int intWidth, ref int intHeight)
        {
            if (intCharIndex < m_arrSampleChar.Count)
            {
                intWidth = m_arrSampleChar[intCharIndex].intWidth;
                intHeight = m_arrSampleChar[intCharIndex].intHeight;
            }
        }

        public void GetTextEndXY(int intTextIndex, ref int intEndX, ref int intEndY)
        {
            if (intTextIndex < m_arrSampleText.Count)
            {
                intEndX = m_arrSampleText[intTextIndex].intEndX;
                intEndY = m_arrSampleText[intTextIndex].intEndY;
            }
            else
            {
                intEndX = intEndY = -1;
            }
        }

        public void GetOCVTextStartXY(int intTextIndex, ref int intStartX, ref int intStartY)
        {
            EOCVText objOcvText = new EOCVText();
            int intCenterX = 0, intCenterY = 0;
#if (Debug_2_12 || Release_2_12)
            m_Ocv.GetTextPoint((uint)intTextIndex, out intCenterX, out intCenterY, 0, 0);
            m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
            m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
            m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

            if (objOcvText.TemplateBackgroundArea != -1 && objOcvText.TemplateBackgroundArea != uint.MaxValue)
            {
                m_Ocv.GetTextPoint((uint)intTextIndex, out intStartX, out intStartY, -1, -1);
            }
            else
            {
                intStartX = intStartY = -1;
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_Ocv.GetTextPoint(intTextIndex, out intCenterX, out intCenterY, 0, 0);
            m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
            m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
            m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

            if (objOcvText.TemplateBackgroundArea != -1 && objOcvText.TemplateBackgroundArea != uint.MaxValue)
            {
                m_Ocv.GetTextPoint(intTextIndex, out intStartX, out intStartY, -1, -1);
            }
            else
            {
                intStartX = intStartY = -1;
            }
#endif

        }

        public void GetTextStartXY(int intTextIndex, ref int intStartX, ref int intStartY)
        {
            if (intTextIndex < m_arrSampleText.Count)
            {
                intStartX = m_arrSampleText[intTextIndex].intOrgX;
                intStartY = m_arrSampleText[intTextIndex].intOrgY;
            }
            else
            {
                intStartX = intStartY = -1;
            }
        }

        public void GetOCVTextSize(int intTextIndex, ref int intWidth, ref int intHeight)
        {
            EOCVText objOcvText = new EOCVText();
            int intCenterX, intCenterY, intStartX, intStartY, intEndX, intEndY;
            intCenterX = intCenterY = intStartX = intStartY = intEndX = intEndY = 0;
#if (Debug_2_12 || Release_2_12)
            m_Ocv.GetTextPoint((uint)intTextIndex, out intCenterX, out intCenterY, 0, 0);
            m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
            m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
            m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

            if (objOcvText.TemplateBackgroundArea != -1 && objOcvText.TemplateBackgroundArea != uint.MaxValue)
            {
                m_Ocv.GetTextPoint((uint)intTextIndex, out intStartX, out intStartY, -1, -1);
                m_Ocv.GetTextPoint((uint)intTextIndex, out intEndX, out intEndY, 1, 1);

                intWidth = intEndX - intStartX;
                intHeight = intEndY - intStartY;
            }
            else
            {
                intWidth = intHeight = 0;
            }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_Ocv.GetTextPoint(intTextIndex, out intCenterX, out intCenterY, 0, 0);
            m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
            m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
            m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

            if (objOcvText.TemplateBackgroundArea != -1 && objOcvText.TemplateBackgroundArea != uint.MaxValue)
            {
                m_Ocv.GetTextPoint(intTextIndex, out intStartX, out intStartY, -1, -1);
                m_Ocv.GetTextPoint(intTextIndex, out intEndX, out intEndY, 1, 1);

                intWidth = intEndX - intStartX;
                intHeight = intEndY - intStartY;
            }
            else
            {
                intWidth = intHeight = 0;
            }
#endif

        }

        public void GetTextSize(int intTextIndex, ref int intWidth, ref int intHeight)
        {
            if (intTextIndex < m_arrSampleText.Count)
            {
                intWidth = m_arrSampleText[intTextIndex].intWidth;
                intHeight = m_arrSampleText[intTextIndex].intHeight;
            }
            else
            {
                intWidth = intHeight = 0;
            }
        }

        public void InspectForOCVInformation_ForLearning(ROI objROI, int intThreshold, float[] arrXToleranceChars, float[] arrYToleranceChars, float fCharROIOffsetX, float fCharROIOffsetY)
        {
            // Keep roi start point for drawing
            m_pDrawStartPoint = new System.Drawing.Point(objROI.ref_ROI.TotalOrgX, objROI.ref_ROI.TotalOrgY);

            // Get image roi
            //ImageDrawing objSampleImage = new ImageDrawing();
            ROI objSampleROI = new ROI();
            GetImageROI(objROI, ref m_objSampleImage, ref objSampleROI);

            /* CCENG (2016 Apr 30) - Add try catch here to avoid pop up euresys error Parameter 1 out of range.
             * This error happen because ocv range is bigger then ROI image size, 
             * but still cannot find why user can create this error 
             * because learn step will control user to learn in correct way.
             */
            try
            {
                //2021-01-07 ZJYEOH : make sure OCV wont use -4 threshold value
                if (intThreshold == -4)
                {
                    intThreshold = ROI.GetAutoThresholdValue(objROI, 3);
                }

                // 2020-06-22 ZJYEOH : Get Text shift tolerance
                List<int> arrXToleranceTexts = new List<int>(), arrYToleranceTexts = new List<int>();
                EOCVText objOcvText = new EOCVText();
                int intCenterX = 0, intCenterY = 0;
#if (Debug_2_12 || Release_2_12)
                for (uint i = 0; i < m_Ocv.NumTexts; i++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                for (int i = 0; i < m_Ocv.NumTexts; i++)
#endif
                    
                {
                    // Select OCV Text
                    m_Ocv.GetTextPoint(i, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);

                    //Select text center point with width, height 1
                    m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);

                    // Get EOCVText from OCV
                    m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

                    arrXToleranceTexts.Add((int)objOcvText.ShiftXTolerance);
                    arrYToleranceTexts.Add((int)objOcvText.ShiftYTolerance);
                }
                // 2020-06-18 ZJYEOH : Set shift tolerance to zero for accurate position
                SetCharsShiftXY(0, 0);
                SetTextsShiftXY(0, 0);
                // 2020 04 12 - CCENG: m_Ocv need to use Binarized in order to get correct char position during learn.
                if (m_Ocv.LocationMode != ELocationMode.Binarized)
                    m_Ocv.LocationMode = ELocationMode.Binarized;
#if (Debug_2_12 || Release_2_12)

                //m_Ocv.Save("D:\\TS\\1OCV.ocv");
                //m_BinaOcv.Save("D:\\TS\\2OCV.ocv");
                //objROI.SaveImage("D:\\TS\\3objROI.bmp");

                //Inspect using gradient location mode
                m_Ocv.Inspect(objROI.ref_ROI, (uint)intThreshold);

                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objROI.ref_ROI, (uint)intThreshold);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objROI.ref_ROI, intThreshold);

                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objROI.ref_ROI, intThreshold);

#endif

                // 2020-06-18 ZJYEOH : Set back shift tolerance
                SetCharsShiftXY(arrXToleranceChars, arrYToleranceChars, fCharROIOffsetX, fCharROIOffsetY);
                SetTextsShiftXY(arrXToleranceTexts, arrYToleranceTexts);
            }
            catch (Exception ex)
            {
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("NOCV.cs->InspectForOCVInformation-> exception=" + ex.ToString());
            }
        }
        public void InspectForOCVInformation_ForLearning(ROI objROI, int intThreshold)
        {
            // Keep roi start point for drawing
            m_pDrawStartPoint = new System.Drawing.Point(objROI.ref_ROI.TotalOrgX, objROI.ref_ROI.TotalOrgY);

            // Get image roi
            //ImageDrawing objSampleImage = new ImageDrawing();
            ROI objSampleROI = new ROI();
            GetImageROI(objROI, ref m_objSampleImage, ref objSampleROI);

            /* CCENG (2016 Apr 30) - Add try catch here to avoid pop up euresys error Parameter 1 out of range.
             * This error happen because ocv range is bigger then ROI image size, 
             * but still cannot find why user can create this error 
             * because learn step will control user to learn in correct way.
             */
            try
            {
                //2021-01-07 ZJYEOH : make sure OCV wont use -4 threshold value
                if (intThreshold == -4)
                {
                    intThreshold = ROI.GetAutoThresholdValue(objROI, 3);
                }

                //SetCharsShiftXY(0, 0);
                //SetTextsShiftXY(0, 0);
                // 2020 04 12 - CCENG: m_Ocv need to use Binarized in order to get correct char position during learn.
                if (m_Ocv.LocationMode != ELocationMode.Binarized)
                    m_Ocv.LocationMode = ELocationMode.Binarized;
#if (Debug_2_12 || Release_2_12)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objROI.ref_ROI, (uint)intThreshold);

                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objROI.ref_ROI, (uint)intThreshold);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                //Inspect using gradient location mode
                    m_Ocv.Inspect(objROI.ref_ROI, intThreshold);

                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objROI.ref_ROI, intThreshold);
#endif

            }
            catch (Exception ex)
            {
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("NOCV.cs->InspectForOCVInformation-> exception=" + ex.ToString());
            }
        }
        public void InspectForOCVInformation(ROI objROI, int intThreshold, float[] arrXToleranceChars, float[] arrYToleranceChars, float fCharROIOffsetX, float fCharROIOffsetY)
        {
            // Keep roi start point for drawing
            m_pDrawStartPoint = new System.Drawing.Point(objROI.ref_ROI.TotalOrgX, objROI.ref_ROI.TotalOrgY);

            // Get image roi
            //ImageDrawing objSampleImage = new ImageDrawing();
            ROI objSampleROI = new ROI();
            GetImageROI(objROI, ref m_objSampleImage, ref objSampleROI);

            /* CCENG (2016 Apr 30) - Add try catch here to avoid pop up euresys error Parameter 1 out of range.
             * This error happen because ocv range is bigger then ROI image size, 
             * but still cannot find why user can create this error 
             * because learn step will control user to learn in correct way.
             */
            try
            {
                //2021-01-07 ZJYEOH : make sure OCV wont use -4 threshold value
                if (intThreshold == -4)
                {
                    intThreshold = ROI.GetAutoThresholdValue(objROI, 3);
                }

                //// 2020-06-22 ZJYEOH : Get Text shift tolerance
                //List<int> arrXToleranceTexts = new List<int>(), arrYToleranceTexts = new List<int>();
                //EOCVText objOcvText = new EOCVText();
                //int intCenterX = 0, intCenterY = 0;
                //for (uint i = 0; i < m_Ocv.NumTexts; i++)
                //{
                //    // Select OCV Text
                //    m_Ocv.GetTextPoint(i, out intCenterX, out intCenterY, 0, 0);
                //    m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);

                //    //Select text center point with width, height 1
                //    m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);

                //    // Get EOCVText from OCV
                //    m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

                //    arrXToleranceTexts.Add((int)objOcvText.ShiftXTolerance);
                //    arrYToleranceTexts.Add((int)objOcvText.ShiftYTolerance);
                //}

                // 2020-06-18 ZJYEOH : Set shift tolerance to zero for accurate position
                //SetCharsShiftXY(0, 0);
                //SetTextsShiftXY(100, 100);
             
                // 2020 04 12 - CCENG: Need to make sure m_Ocv is Gradient mood bcos m_Ocv will be set to Binarized mode during learn.
                if (m_Ocv.LocationMode != ELocationMode.Gradient)
                    m_Ocv.LocationMode = ELocationMode.Gradient;
#if (Debug_2_12 || Release_2_12)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objROI.ref_ROI, (uint)intThreshold);

                // 2020-06-17 ZJYEOH : Reset LocationMode to Binarized as m_BinaOcv is directly affected by m_Ocv
                if (m_BinaOcv.LocationMode != ELocationMode.Binarized)
                    m_BinaOcv.LocationMode = ELocationMode.Binarized;
                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objROI.ref_ROI, (uint)intThreshold);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objROI.ref_ROI, intThreshold);

                // 2020-06-17 ZJYEOH : Reset LocationMode to Binarized as m_BinaOcv is directly affected by m_Ocv
                if (m_BinaOcv.LocationMode != ELocationMode.Binarized)
                    m_BinaOcv.LocationMode = ELocationMode.Binarized;
                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objROI.ref_ROI, intThreshold);

#endif

                // 2020-06-17 ZJYEOH : Reset LocationMode to Gradient as m_Ocv is directly affected by m_BinaOcv
                if (m_Ocv.LocationMode != ELocationMode.Gradient)
                    m_Ocv.LocationMode = ELocationMode.Gradient;

                // 2020-06-18 ZJYEOH : Set back shift tolerance
                //SetCharsShiftXY(arrXToleranceChars, arrYToleranceChars, fCharROIOffsetX, fCharROIOffsetY);
                //SetTextsShiftXY(arrXToleranceTexts, arrYToleranceTexts);
            }
            catch (Exception ex)
            {
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("NOCV.cs->InspectForOCVInformation-> exception=" + ex.ToString());
            }
        }

        public void InspectForOCVInformation(ROI objROI, int intThreshold)
        {
            // Keep roi start point for drawing
            m_pDrawStartPoint = new System.Drawing.Point(objROI.ref_ROI.TotalOrgX, objROI.ref_ROI.TotalOrgY);

            // Get image roi
            //ImageDrawing objSampleImage = new ImageDrawing();
            ROI objSampleROI = new ROI();
            GetImageROI(objROI, ref m_objSampleImage, ref objSampleROI);

            /* CCENG (2016 Apr 30) - Add try catch here to avoid pop up euresys error Parameter 1 out of range.
             * This error happen because ocv range is bigger then ROI image size, 
             * but still cannot find why user can create this error 
             * because learn step will control user to learn in correct way.
             */
            try
            {
                //2021-01-07 ZJYEOH : make sure OCV wont use -4 threshold value
                if (intThreshold == -4)
                {
                    intThreshold = ROI.GetAutoThresholdValue(objROI, 3);
                }
                // 2020 04 12 - CCENG: Need to make sure m_Ocv is Gradient mood bcos m_Ocv will be set to Binarized mode during learn.
                if (m_Ocv.LocationMode != ELocationMode.Gradient)
                    m_Ocv.LocationMode = ELocationMode.Gradient;
#if (Debug_2_12 || Release_2_12)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objROI.ref_ROI, (uint)intThreshold);

                // 2020-06-17 ZJYEOH : Reset LocationMode to Binarized as m_BinaOcv is directly affected by m_Ocv
                if (m_BinaOcv.LocationMode != ELocationMode.Binarized)
                    m_BinaOcv.LocationMode = ELocationMode.Binarized;
                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objROI.ref_ROI, (uint)intThreshold);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                 //Inspect using gradient location mode
                m_Ocv.Inspect(objROI.ref_ROI, intThreshold);

                // 2020-06-17 ZJYEOH : Reset LocationMode to Binarized as m_BinaOcv is directly affected by m_Ocv
                if (m_BinaOcv.LocationMode != ELocationMode.Binarized)
                    m_BinaOcv.LocationMode = ELocationMode.Binarized;
                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objROI.ref_ROI, intThreshold);

#endif

                // 2020-06-17 ZJYEOH : Reset LocationMode to Gradient as m_Ocv is directly affected by m_BinaOcv
                if (m_Ocv.LocationMode != ELocationMode.Gradient)
                    m_Ocv.LocationMode = ELocationMode.Gradient;
            }
            catch (Exception ex)
            {
                TrackLog objTL = new TrackLog();
                objTL.WriteLine("NOCV.cs->InspectForOCVInformation-> exception=" + ex.ToString());
            }
        }

        public void Inspect(TrackLog objTL, ROI objROI, int intThreshold, int intOffsetROIX, int intOffsetROIY, float fMinTextOffsetAllowX, float fMinTextOffsetAllowY, bool blnWantUseSampleAreaScore)
        {
            //ImageDrawing objSampleImage = new ImageDrawing();
            try
            {
                // Keep roi start point for drawing
                m_pDrawStartPoint = new System.Drawing.Point(objROI.ref_ROI.TotalOrgX, objROI.ref_ROI.TotalOrgY);

                // Get image roi

                ROI objSampleROI = new ROI();
                GetImageROI(objROI, ref m_objSampleImage, ref objSampleROI);
                //2021-01-07 ZJYEOH : make sure OCV wont use -4 threshold value
                if (intThreshold == -4)
                {
                    intThreshold = ROI.GetAutoThresholdValue(objSampleROI, 3);
                }
                //m_Ocv.Save("D:\\TS\\1OCV.ocv");
                //m_BinaOcv.Save("D:\\TS\\2OCV.ocv");
                //objSampleROI.SaveImage("D:\\TS\\3objSampleROI.bmp");
#if (Debug_2_12 || Release_2_12)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objSampleROI.ref_ROI, (uint)intThreshold);

                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objSampleROI.ref_ROI, (uint)intThreshold);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objSampleROI.ref_ROI, intThreshold);

                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objSampleROI.ref_ROI, intThreshold);

#endif

                m_arrSampleChar.Clear();
                m_arrSampleText.Clear();
                int intNumTextChars;
                int intNumTexts = (int)m_Ocv.NumTexts;
                int intCenterX = 0, intCenterY = 0;
                int intOcvTextCenterX = 0, intOcvTextCenterY = 0;
                int intBinaryOcvTextCenterX = 0, intBinaryOcvTextCenterY = 0;
                float fScoreValue = -1;
                Sample objSample;
                EOCVChar objOcvChar = new EOCVChar();
                EOCVText objOcvText = new EOCVText();
                EOCVText objBinaOcvText = new EOCVText();
#if (Debug_2_12 || Release_2_12)
                for (uint tx = 0; tx < intNumTexts; tx++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                for (int tx = 0; tx < intNumTexts; tx++)
#endif

                {

                    #region Get text inspection data
                    m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, 0, 0);
                    m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                    m_Ocv.SelectSampleTexts(intOcvTextCenterX, intOcvTextCenterY, 1, 1);
                    m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, -1, -1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX + 1, intOcvTextCenterY + 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }
                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, 1, 1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX - 1, intOcvTextCenterY - 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }
                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, -1, 1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX + 1, intOcvTextCenterY - 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }
                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, 1, -1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX - 1, intOcvTextCenterY + 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }

                    if (objOcvText.TemplateBackgroundArea != -1 && objOcvText.TemplateBackgroundArea != uint.MaxValue)
                    {
                        if (objOcvText.Correlation <= 0 || objOcvText.Correlation > 1)
                            fScoreValue = 0;
                        else
                            fScoreValue = objOcvText.Correlation * 100;
                    }

                    m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, 0, 0);
                    m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                    m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX, intBinaryOcvTextCenterY, 1, 1);
                    m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);

                    if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, -1, -1);
                        m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX + 1, intBinaryOcvTextCenterY + 1, 1, 1);
                        m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                    }
                    if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, 1, 1);
                        m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX - 1, intBinaryOcvTextCenterY - 1, 1, 1);
                        m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                    }
                    if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, -1, 1);
                        m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX + 1, intBinaryOcvTextCenterY - 1, 1, 1);
                        m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                    }
                    if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, 1, -1);
                        m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX - 1, intBinaryOcvTextCenterY + 1, 1, 1);
                        m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                    }

                    bool blnWantUseOcvChar = true;
                    bool blnWantuseBinarOcvChar = true;
                    objSample = new Sample();
                    if (objBinaOcvText.TemplateBackgroundArea != -1 && objBinaOcvText.TemplateBackgroundArea != uint.MaxValue)
                    {
                        float fBinaScoreValue;
                        if (objBinaOcvText.Correlation <= 0 || objBinaOcvText.Correlation > 1)
                            fBinaScoreValue = 0;
                        else
                            fBinaScoreValue = objBinaOcvText.Correlation * 100;

                        if (fBinaScoreValue <= fScoreValue)
                        {
                            //2021-05-16 ZJYEOH : if blnWantUseOcvChar == true just use m_Ocv to get position else will draw wrong char 
                            if (blnWantUseOcvChar)
                            {
                                m_Ocv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                m_Ocv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);

                                objSample.intLocMode = 1;
                                objSample.fScore = fScoreValue;
                            }
                            else
                            {
                                m_BinaOcv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                m_BinaOcv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                objSample.intLocMode = 2;
                                objSample.fScore = fBinaScoreValue;
                            }
                        }
                        else
                        {
                            m_BinaOcv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                            m_BinaOcv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);
                            objSample.intLocMode = 2;
                            objSample.fScore = fBinaScoreValue;
                        }

                        objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                        objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                        objSample.intOrgX -= intOffsetROIX;
                        objSample.intOrgY -= intOffsetROIY;
                        objSample.intEndX -= intOffsetROIX;
                        objSample.intEndY -= intOffsetROIY;
                        
                        m_arrSampleText.Add(objSample);

                        if ((Math.Abs(intOcvTextCenterX - intBinaryOcvTextCenterX) < fMinTextOffsetAllowX) &&
                            (Math.Abs(intOcvTextCenterY - intBinaryOcvTextCenterY) < fMinTextOffsetAllowY))
                        {
                            blnWantUseOcvChar = true;
                            blnWantuseBinarOcvChar = true;
                        }
                        else
                        {
                            if (fBinaScoreValue <= fScoreValue)
                            {
                                blnWantUseOcvChar = true;
                                blnWantuseBinarOcvChar = false;
                            }
                            else
                            {
                                blnWantUseOcvChar = false;
                                blnWantuseBinarOcvChar = true;
                            }
                        }
                    }
                    else if (fScoreValue >= 0)
                    {
                        m_Ocv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                        m_Ocv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);
                        objSample.intLocMode = 1;
                        objSample.fScore = fScoreValue;
                        objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                        objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                        objSample.intOrgX -= intOffsetROIX;
                        objSample.intOrgY -= intOffsetROIY;
                        objSample.intEndX -= intOffsetROIX;
                        objSample.intEndY -= intOffsetROIY;
                        m_arrSampleChar.Add(objSample);

                        blnWantUseOcvChar = true;
                        blnWantuseBinarOcvChar = false;
                    }
                    else
                    {
                        objSample.intLocMode = 1;
                        objSample.fScore = 0;
                        m_arrSampleChar.Add(objSample);
                    }

                    #endregion


                    #region Get char inspection data
                    intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
#if (Debug_2_12 || Release_2_12)
                    for (uint ch = 0; ch < intNumTextChars; ch++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    for (int ch = 0; ch < intNumTextChars; ch++)
#endif

                    {
                        objSample = new Sample();
                        if (blnWantUseOcvChar)
                        {
                            m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                            m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                            m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                            m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, -1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX + 1, intCenterY + 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, 1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX - 1, intCenterY - 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, 1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX + 1, intCenterY - 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, -1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX - 1, intCenterY + 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }

                            if (objOcvChar.TemplateBackgroundArea != -1 && objOcvChar.TemplateBackgroundArea != uint.MaxValue)
                            {
                                if (objOcvChar.Correlation <= 0 || objOcvChar.Correlation > 1)
                                    fScoreValue = 0;
                                else
                                    fScoreValue = objOcvChar.Correlation * 100;

                                objSample.intForeArea = (int)objOcvChar.SampleForegroundArea;
                                objSample.fForeAreaSumPercent = ((objOcvChar.SampleForegroundSum - objOcvChar.TemplateForegroundSum) / objOcvChar.TemplateForegroundSum) * 100;

                                if (blnWantUseSampleAreaScore)
                                {

                                    float fAreaScore = (float)Math.Abs(objOcvChar.SampleBackgroundArea - objOcvChar.TemplateBackgroundArea) / objOcvChar.TemplateBackgroundArea * 100;

                                    fScoreValue -= fAreaScore;
                                    if (fScoreValue <= 0)
                                        fScoreValue = 0;
                                }
                            }
                        }
                        else
                        {
                            fScoreValue = 0;
                        }

                        if (blnWantuseBinarOcvChar)
                        {
                            m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                            m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                            m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, -1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY + 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, 1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY - 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, 1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY - 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, -1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY + 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                        }

                        if (blnWantuseBinarOcvChar && objOcvChar.TemplateBackgroundArea != -1 && objOcvChar.TemplateBackgroundArea != uint.MaxValue)
                        {
                            float fBinaScoreValue;
                            if (objOcvChar.Correlation <= 0 || objOcvChar.Correlation > 1)
                                fBinaScoreValue = 0;
                            else
                                fBinaScoreValue = objOcvChar.Correlation * 100;

                            if (blnWantUseSampleAreaScore)
                            {

                                float fAreaScore = (float)Math.Abs(objOcvChar.SampleBackgroundArea - objOcvChar.TemplateBackgroundArea) / objOcvChar.TemplateBackgroundArea * 100;

                                fBinaScoreValue -= fAreaScore;
                                if (fBinaScoreValue <= 0)
                                    fBinaScoreValue = 0;
                            }

                            if (fBinaScoreValue <= fScoreValue)
                            {
                                //2021-05-16 ZJYEOH : if blnWantUseOcvChar == true just use m_Ocv to get position else will draw wrong char 
                                if (blnWantUseOcvChar)
                                {
                                    m_Ocv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                    m_Ocv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                    objSample.fScore = fScoreValue;
                                }
                                else
                                {
                                    m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                    m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                    objSample.fScore = fBinaScoreValue;

                                    objSample.intForeArea = (int)objOcvChar.SampleForegroundArea;
                                    objSample.fForeAreaSumPercent = ((objOcvChar.SampleForegroundSum - objOcvChar.TemplateForegroundSum) / objOcvChar.TemplateForegroundSum) * 100;
                                }
                            }
                            else
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                objSample.fScore = fBinaScoreValue;

                                objSample.intForeArea = (int)objOcvChar.SampleForegroundArea;
                                objSample.fForeAreaSumPercent = ((objOcvChar.SampleForegroundSum - objOcvChar.TemplateForegroundSum) / objOcvChar.TemplateForegroundSum) * 100;

                            }
                            objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                            objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                            objSample.intOrgX -= intOffsetROIX;
                            objSample.intOrgY -= intOffsetROIY;
                            objSample.intEndX -= intOffsetROIX;
                            objSample.intEndY -= intOffsetROIY;
                            m_arrSampleChar.Add(objSample);
                        }
                        else if (blnWantUseOcvChar && fScoreValue >= 0)
                        {
                            m_Ocv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                            m_Ocv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                            objSample.fScore = fScoreValue;
                            objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                            objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                            objSample.intOrgX -= intOffsetROIX;
                            objSample.intOrgY -= intOffsetROIY;
                            objSample.intEndX -= intOffsetROIX;
                            objSample.intEndY -= intOffsetROIY;
                            m_arrSampleChar.Add(objSample);
                        }
                        else
                        {
                            objSample.fScore = 0;
                            m_arrSampleChar.Add(objSample);
                        }
                    }
                    #endregion
                }

                ArrangeCharByCharNo();

                if (objSampleROI != null)
                {
                    objSampleROI.Dispose();
                    objSampleROI = null;
                }
            }
            catch (Exception ex)
            {
                objTL.WriteLine("NOCV->Inspect ex: " + ex.ToString());
            }
        }
        public void Inspect(TrackLog objTL, ROI objROI, int intThreshold, int intOffsetROIX, int intOffsetROIY, float fMinTextOffsetAllowX, float fMinTextOffsetAllowY, bool blnWantUseSampleAreaScore, float fAngle)
        {
            //ImageDrawing objSampleImage = new ImageDrawing();
            try
            {
                // Keep roi start point for drawing
                m_pDrawStartPoint = new System.Drawing.Point(objROI.ref_ROI.TotalOrgX, objROI.ref_ROI.TotalOrgY);

                // Get image roi

                //2020-11-09 ZJYEOH : set angle tolerance for tilt inspect
                SetTextsSkewTolerance(-fAngle);

                ROI objSampleROI = new ROI();
                GetImageROI(objROI, ref m_objSampleImage, ref objSampleROI);
                //2021-01-07 ZJYEOH : make sure OCV wont use -4 threshold value
                if (intThreshold == -4)
                {
                    intThreshold = ROI.GetAutoThresholdValue(objSampleROI, 3);
                }
                //m_Ocv.Save("D:\\TS\\1OCV.ocv");
                //m_BinaOcv.Save("D:\\TS\\2OCV.ocv");
                //objSampleROI.SaveImage("D:\\TS\\3objSampleROI.bmp");
#if (Debug_2_12 || Release_2_12)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objSampleROI.ref_ROI, (uint)intThreshold);

                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objSampleROI.ref_ROI, (uint)intThreshold);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objSampleROI.ref_ROI, intThreshold);

                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objSampleROI.ref_ROI, intThreshold);

#endif

                m_arrSampleChar.Clear();
                m_arrSampleText.Clear();
                int intNumTextChars;
                int intNumTexts = (int)m_Ocv.NumTexts;
                int intCenterX = 0, intCenterY = 0;
                int intOcvTextCenterX = 0, intOcvTextCenterY = 0;
                int intBinaryOcvTextCenterX = 0, intBinaryOcvTextCenterY = 0;
                float fScoreValue = -1;
                Sample objSample;
                EOCVChar objOcvChar = new EOCVChar();
                EOCVText objOcvText = new EOCVText();
                EOCVText objBinaOcvText = new EOCVText();
#if (Debug_2_12 || Release_2_12)
                for (uint tx = 0; tx < intNumTexts; tx++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                for (int tx = 0; tx < intNumTexts; tx++)
#endif

                {

                    #region Get text inspection data
                    m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, 0, 0);
                    m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                    m_Ocv.SelectSampleTexts(intOcvTextCenterX, intOcvTextCenterY, 1, 1);
                    m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, -1, -1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX + 1, intOcvTextCenterY + 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }
                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, 1, 1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX - 1, intOcvTextCenterY - 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }
                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, -1, 1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX + 1, intOcvTextCenterY - 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }
                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, 1, -1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX - 1, intOcvTextCenterY + 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }

                    if (objOcvText.TemplateBackgroundArea != -1 && objOcvText.TemplateBackgroundArea != uint.MaxValue)
                    {
                        if (objOcvText.Correlation <= 0 || objOcvText.Correlation > 1)
                            fScoreValue = 0;
                        else
                            fScoreValue = objOcvText.Correlation * 100;
                    }

                    m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, 0, 0);
                    m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                    m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX, intBinaryOcvTextCenterY, 1, 1);
                    m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);

                    if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, -1, -1);
                        m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX + 1, intBinaryOcvTextCenterY + 1, 1, 1);
                        m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                    }
                    if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, 1, 1);
                        m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX - 1, intBinaryOcvTextCenterY - 1, 1, 1);
                        m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                    }
                    if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, -1, 1);
                        m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX + 1, intBinaryOcvTextCenterY - 1, 1, 1);
                        m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                    }
                    if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, 1, -1);
                        m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX - 1, intBinaryOcvTextCenterY + 1, 1, 1);
                        m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                    }

                    bool blnWantUseOcvChar = true;
                    bool blnWantuseBinarOcvChar = true;
                    objSample = new Sample();
                    if (objBinaOcvText.TemplateBackgroundArea != -1 && objBinaOcvText.TemplateBackgroundArea != uint.MaxValue)
                    {
                        float fBinaScoreValue;
                        if (objBinaOcvText.Correlation <= 0 || objBinaOcvText.Correlation > 1)
                            fBinaScoreValue = 0;
                        else
                            fBinaScoreValue = objBinaOcvText.Correlation * 100;

                        if (fBinaScoreValue <= fScoreValue)
                        {
                            //2021-05-16 ZJYEOH : if blnWantUseOcvChar == true just use m_Ocv to get position else will draw wrong char 
                            if (blnWantUseOcvChar)
                            {
                                m_Ocv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                m_Ocv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);

                                objSample.intLocMode = 1;
                                objSample.fScore = fScoreValue;
                            }
                            else
                            {
                                m_BinaOcv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                m_BinaOcv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                objSample.intLocMode = 2;
                                objSample.fScore = fBinaScoreValue;
                            }
                        }
                        else
                        {
                            m_BinaOcv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                            m_BinaOcv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);
                            objSample.intLocMode = 2;
                            objSample.fScore = fBinaScoreValue;
                        }

                        objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                        objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                        objSample.intOrgX -= intOffsetROIX;
                        objSample.intOrgY -= intOffsetROIY;
                        objSample.intEndX -= intOffsetROIX;
                        objSample.intEndY -= intOffsetROIY;

                        m_arrSampleText.Add(objSample);

                        if ((Math.Abs(intOcvTextCenterX - intBinaryOcvTextCenterX) < fMinTextOffsetAllowX) &&
                            (Math.Abs(intOcvTextCenterY - intBinaryOcvTextCenterY) < fMinTextOffsetAllowY))
                        {
                            blnWantUseOcvChar = true;
                            blnWantuseBinarOcvChar = true;
                        }
                        else
                        {
                            if (fBinaScoreValue <= fScoreValue)
                            {
                                blnWantUseOcvChar = true;
                                blnWantuseBinarOcvChar = false;
                            }
                            else
                            {
                                blnWantUseOcvChar = false;
                                blnWantuseBinarOcvChar = true;
                            }
                        }
                    }
                    else if (fScoreValue >= 0)
                    {
                        m_Ocv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                        m_Ocv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);
                        objSample.intLocMode = 1;
                        objSample.fScore = fScoreValue;
                        objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                        objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                        objSample.intOrgX -= intOffsetROIX;
                        objSample.intOrgY -= intOffsetROIY;
                        objSample.intEndX -= intOffsetROIX;
                        objSample.intEndY -= intOffsetROIY;
                        m_arrSampleChar.Add(objSample);

                        blnWantUseOcvChar = true;
                        blnWantuseBinarOcvChar = false;
                    }
                    else
                    {
                        objSample.intLocMode = 1;
                        objSample.fScore = 0;
                        m_arrSampleChar.Add(objSample);
                    }

                    #endregion


                    #region Get char inspection data
                    intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
#if (Debug_2_12 || Release_2_12)
                    for (uint ch = 0; ch < intNumTextChars; ch++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    for (int ch = 0; ch < intNumTextChars; ch++)
#endif

                    {
                        objSample = new Sample();
                        if (blnWantUseOcvChar)
                        {
                            m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                            m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                            m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                            m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, -1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX + 1, intCenterY + 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, 1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX - 1, intCenterY - 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, 1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX + 1, intCenterY - 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, -1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX - 1, intCenterY + 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }

                            if (objOcvChar.TemplateBackgroundArea != -1 && objOcvChar.TemplateBackgroundArea != uint.MaxValue)
                            {
                                if (objOcvChar.Correlation <= 0 || objOcvChar.Correlation > 1)
                                    fScoreValue = 0;
                                else
                                    fScoreValue = objOcvChar.Correlation * 100;

                                objSample.intForeArea = (int)objOcvChar.SampleForegroundArea;
                                objSample.fForeAreaSumPercent = ((objOcvChar.SampleForegroundSum - objOcvChar.TemplateForegroundSum) / objOcvChar.TemplateForegroundSum) * 100;

                                if (blnWantUseSampleAreaScore)
                                {

                                    float fAreaScore = (float)Math.Abs(objOcvChar.SampleBackgroundArea - objOcvChar.TemplateBackgroundArea) / objOcvChar.TemplateBackgroundArea * 100;

                                    fScoreValue -= fAreaScore;
                                    if (fScoreValue <= 0)
                                        fScoreValue = 0;
                                }
                            }
                        }
                        else
                        {
                            fScoreValue = 0;
                        }

                        if (blnWantuseBinarOcvChar)
                        {
                            m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                            m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                            m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, -1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY + 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, 1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY - 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, 1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY - 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, -1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY + 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                        }

                        if (blnWantuseBinarOcvChar && objOcvChar.TemplateBackgroundArea != -1 && objOcvChar.TemplateBackgroundArea != uint.MaxValue)
                        {
                            float fBinaScoreValue;
                            if (objOcvChar.Correlation <= 0 || objOcvChar.Correlation > 1)
                                fBinaScoreValue = 0;
                            else
                                fBinaScoreValue = objOcvChar.Correlation * 100;

                            if (blnWantUseSampleAreaScore)
                            {

                                float fAreaScore = (float)Math.Abs(objOcvChar.SampleBackgroundArea - objOcvChar.TemplateBackgroundArea) / objOcvChar.TemplateBackgroundArea * 100;

                                fBinaScoreValue -= fAreaScore;
                                if (fBinaScoreValue <= 0)
                                    fBinaScoreValue = 0;
                            }

                            if (fBinaScoreValue <= fScoreValue)
                            {
                                //2021-05-16 ZJYEOH : if blnWantUseOcvChar == true just use m_Ocv to get position else will draw wrong char 
                                if (blnWantUseOcvChar)
                                {
                                    m_Ocv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                    m_Ocv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                    objSample.fScore = fScoreValue;
                                }
                                else
                                {
                                    m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                    m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                    objSample.fScore = fBinaScoreValue;

                                    objSample.intForeArea = (int)objOcvChar.SampleForegroundArea;
                                    objSample.fForeAreaSumPercent = ((objOcvChar.SampleForegroundSum - objOcvChar.TemplateForegroundSum) / objOcvChar.TemplateForegroundSum) * 100;
                                }
                            }
                            else
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                objSample.fScore = fBinaScoreValue;

                                objSample.intForeArea = (int)objOcvChar.SampleForegroundArea;
                                objSample.fForeAreaSumPercent = ((objOcvChar.SampleForegroundSum - objOcvChar.TemplateForegroundSum) / objOcvChar.TemplateForegroundSum) * 100;

                            }
                            objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                            objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                            objSample.intOrgX -= intOffsetROIX;
                            objSample.intOrgY -= intOffsetROIY;
                            objSample.intEndX -= intOffsetROIX;
                            objSample.intEndY -= intOffsetROIY;
                            m_arrSampleChar.Add(objSample);
                        }
                        else if (blnWantUseOcvChar && fScoreValue >= 0)
                        {
                            m_Ocv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                            m_Ocv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                            objSample.fScore = fScoreValue;
                            objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                            objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                            objSample.intOrgX -= intOffsetROIX;
                            objSample.intOrgY -= intOffsetROIY;
                            objSample.intEndX -= intOffsetROIX;
                            objSample.intEndY -= intOffsetROIY;
                            m_arrSampleChar.Add(objSample);
                        }
                        else
                        {
                            objSample.fScore = 0;
                            m_arrSampleChar.Add(objSample);
                        }
                    }
                    #endregion
                }

                ArrangeCharByCharNo();

                if (objSampleROI != null)
                {
                    objSampleROI.Dispose();
                    objSampleROI = null;
                }
            }
            catch (Exception ex)
            {
                objTL.WriteLine("NOCV->Inspect ex: " + ex.ToString());
            }
        }
        public bool Inspect_First2(TrackLog objTL, ROI objSearchROI, ROI objMarkROI, int intThreshold, int intOffsetROIX, int intOffsetROIY, float fMinTextOffsetAllowX, float fMinTextOffsetAllowY,
            bool blnWantUseSampleAreaScore, int intMarkScoreOffset, int intMarkOriPositionScore, int intMarkScoreMode)
        {
            //ImageDrawing objSampleImage = new ImageDrawing();
            try
            {
                // Keep roi start point for drawing
                m_pDrawStartPoint = new System.Drawing.Point(objSearchROI.ref_ROI.TotalOrgX, objSearchROI.ref_ROI.TotalOrgY);

                // Get image roi

                ROI objSampleROI = new ROI();
                GetImageROI(objSearchROI, ref m_objSampleImage, ref objSampleROI);

                bool blnWantDebug = false;
                if (blnWantDebug)
                {
                    m_Ocv.Save("D:\\TS\\1OCV.ocv");
                    m_BinaOcv.Save("D:\\TS\\2OCV.ocv");
                    m_MinOcv.Save("D:\\TS\\3OCV.ocv");
                    objSampleROI.SaveImage("D:\\TS\\3objSampleROI.bmp");
                }
                //2021-01-07 ZJYEOH : make sure OCV wont use -4 threshold value
                if (intThreshold == -4)
                {
                    intThreshold = ROI.GetAutoThresholdValue(objSampleROI, 3);
                }
             
                bool blnMinOcvInspected = false;

                //2020-06-17 ZJYEOH : Just in case not using correct LocationMode
                if (m_Ocv.LocationMode != ELocationMode.Gradient)
                    m_Ocv.LocationMode = ELocationMode.Gradient;
#if (Debug_2_12 || Release_2_12)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objSampleROI.ref_ROI, (uint)intThreshold);

                if (intMarkScoreMode != 1)
                {
                    //2020-06-17 ZJYEOH : Just in case not using correct LocationMode
                    if (m_BinaOcv.LocationMode != ELocationMode.Binarized)
                        m_BinaOcv.LocationMode = ELocationMode.Binarized;

                    //Inspect using binarized location mode
                    m_BinaOcv.Inspect(objSampleROI.ref_ROI, (uint)intThreshold);
                }

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objSampleROI.ref_ROI, intThreshold);

                //2020-06-17 ZJYEOH : Just in case not using correct LocationMode
                if (m_BinaOcv.LocationMode != ELocationMode.Binarized)
                    m_BinaOcv.LocationMode = ELocationMode.Binarized;

                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objSampleROI.ref_ROI, intThreshold);

#endif

                //2020-06-17 ZJYEOH : Just in case not using correct LocationMode
                if (m_Ocv.LocationMode != ELocationMode.Gradient)
                    m_Ocv.LocationMode = ELocationMode.Gradient;

                m_arrSampleChar.Clear();
                m_arrSampleText.Clear();
                int intNumTextChars;
                int intNumTexts = (int)m_Ocv.NumTexts;
                int intCenterX = 0, intCenterY = 0;
                int intOcvTextCenterX = 0, intOcvTextCenterY = 0;
                int intBinaryOcvTextCenterX = 0, intBinaryOcvTextCenterY = 0;
                float fScoreValue = -1;
                //bool blnIsTextInsideMarkROI = false;
                List<bool> blnIsCharInsideMarkROI = new List<bool>();
                Sample objSample;
                EOCVChar objOcvChar = new EOCVChar();
                EOCVText objOcvText = new EOCVText();
                EOCVText objBinaOcvText = new EOCVText();
#if (Debug_2_12 || Release_2_12)
                for (uint tx = 0; tx < intNumTexts; tx++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                for (int tx = 0; tx < intNumTexts; tx++)
#endif

                {
                    #region Get text inspection data
                    m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, 0, 0);
                    m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                    m_Ocv.SelectSampleTexts(intOcvTextCenterX, intOcvTextCenterY, 1, 1);
                    m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, -1, -1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX + 1, intOcvTextCenterY + 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }
                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, 1, 1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX - 1, intOcvTextCenterY - 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }
                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, -1, 1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX + 1, intOcvTextCenterY - 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }
                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, 1, -1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX - 1, intOcvTextCenterY + 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }

                    if (objOcvText.TemplateBackgroundArea != -1 && objOcvText.TemplateBackgroundArea != uint.MaxValue)
                    {
                        if (objOcvText.Correlation <= 0 || objOcvText.Correlation > 1)
                            fScoreValue = 0;
                        else
                            fScoreValue = objOcvText.Correlation * 100;
                    }

                    if (intMarkScoreMode != 1)
                    {
                        m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, 0, 0);
                        m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX, intBinaryOcvTextCenterY, 1, 1);
                        m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);

                        if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                        {
                            m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, -1, -1);
                            m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX + 1, intBinaryOcvTextCenterY + 1, 1, 1);
                            m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                        }
                        if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                        {
                            m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, 1, 1);
                            m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX - 1, intBinaryOcvTextCenterY - 1, 1, 1);
                            m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                        }
                        if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                        {
                            m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, -1, 1);
                            m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX + 1, intBinaryOcvTextCenterY - 1, 1, 1);
                            m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                        }
                        if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                        {
                            m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, 1, -1);
                            m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX - 1, intBinaryOcvTextCenterY + 1, 1, 1);
                            m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                        }
                    }

                    bool blnWantUseOcvChar = true;
                    bool blnWantuseBinarOcvChar = true;
                    objSample = new Sample();
                    if (objBinaOcvText.TemplateBackgroundArea != -1 && objBinaOcvText.TemplateBackgroundArea != uint.MaxValue)
                    {
                        float fBinaScoreValue;
                        if (objBinaOcvText.Correlation <= 0 || objBinaOcvText.Correlation > 1)
                            fBinaScoreValue = 0;
                        else
                            fBinaScoreValue = objBinaOcvText.Correlation * 100;

                        if (fBinaScoreValue <= fScoreValue)
                        {
                            //2021-05-16 ZJYEOH : if blnWantUseOcvChar == true just use m_Ocv to get position else will draw wrong char 
                            if (blnWantUseOcvChar)
                            {
                                m_Ocv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                m_Ocv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);

                                objSample.intLocMode = 1;
                                objSample.fScore = fScoreValue;
                            }
                            else
                            {
                                m_BinaOcv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                m_BinaOcv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                objSample.intLocMode = 2;
                                objSample.fScore = fBinaScoreValue;
                            }
                        }
                        else
                        {
                            m_BinaOcv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                            m_BinaOcv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);
                            objSample.intLocMode = 2;
                            objSample.fScore = fBinaScoreValue;
                        }

                        objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                        objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                        objSample.intOrgX -= intOffsetROIX;
                        objSample.intOrgY -= intOffsetROIY;
                        objSample.intEndX -= intOffsetROIX;
                        objSample.intEndY -= intOffsetROIY;

                        //2020-11-03 ZJYEOH : Add Mark Score Offset
                        if (intMarkScoreOffset != 0)
                        {
                            if (intMarkScoreOffset > 0)
                            {
                                if (objSample.fScore < 95)
                                {
                                    objSample.fScore += intMarkScoreOffset;

                                    if (objSample.fScore < 5)
                                        objSample.fScore = 5;
                                    else if (objSample.fScore > 95)
                                        objSample.fScore = 95;

                                }
                            }
                            else
                            {
                                if (objSample.fScore > 5)
                                {
                                    objSample.fScore += intMarkScoreOffset;

                                    if (objSample.fScore < 5)
                                        objSample.fScore = 5;
                                    else if (objSample.fScore > 95)
                                        objSample.fScore = 95;

                                }
                            }
                        }

                        m_arrSampleText.Add(objSample);

                        if ((Math.Abs(intOcvTextCenterX - intBinaryOcvTextCenterX) < fMinTextOffsetAllowX) &&
                            (Math.Abs(intOcvTextCenterY - intBinaryOcvTextCenterY) < fMinTextOffsetAllowY))
                        {
                            blnWantUseOcvChar = true;
                            blnWantuseBinarOcvChar = true;
                        }
                        else
                        {
                            if (fBinaScoreValue <= fScoreValue)
                            {
                                blnWantUseOcvChar = true;
                                blnWantuseBinarOcvChar = false;
                            }
                            else
                            {
                                blnWantUseOcvChar = false;
                                blnWantuseBinarOcvChar = true;
                            }
                        }
                    }
                    else if (fScoreValue >= 0)
                    {
                        m_Ocv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                        m_Ocv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);
                        objSample.intLocMode = 1;
                        objSample.fScore = fScoreValue;
                        objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                        objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                        objSample.intOrgX -= intOffsetROIX;
                        objSample.intOrgY -= intOffsetROIY;
                        objSample.intEndX -= intOffsetROIX;
                        objSample.intEndY -= intOffsetROIY;

                        //2020-11-03 ZJYEOH : Add Mark Score Offset
                        if (intMarkScoreOffset != 0)
                        {
                            if (intMarkScoreOffset > 0)
                            {
                                if (objSample.fScore < 95)
                                {
                                    objSample.fScore += intMarkScoreOffset;

                                    if (objSample.fScore < 5)
                                        objSample.fScore = 5;
                                    else if (objSample.fScore > 95)
                                        objSample.fScore = 95;

                                }
                            }
                            else
                            {
                                if (objSample.fScore > 5)
                                {
                                    objSample.fScore += intMarkScoreOffset;

                                    if (objSample.fScore < 5)
                                        objSample.fScore = 5;
                                    else if (objSample.fScore > 95)
                                        objSample.fScore = 95;

                                }
                            }
                        }

                        m_arrSampleChar.Add(objSample);

                        blnWantUseOcvChar = true;
                        blnWantuseBinarOcvChar = false;
                    }
                    else
                    {
                        objSample.intLocMode = 1;
                        objSample.fScore = 0;
                        m_arrSampleChar.Add(objSample);
                    }

                    #endregion

                    //if (blnWantUseOcvChar)
                    //{
                    //    if ((intOcvTextCenterX > objMarkROI.ref_ROIPositionX && intOcvTextCenterX < (objMarkROI.ref_ROIPositionX + objMarkROI.ref_ROIWidth)) &&
                    //        (intOcvTextCenterY > objMarkROI.ref_ROIPositionY && intOcvTextCenterY < (objMarkROI.ref_ROIPositionY + objMarkROI.ref_ROIHeight)))
                    //    {
                    //        blnIsTextInsideMarkROI = true;
                    //    }
                    //}

                    //if (blnWantuseBinarOcvChar && !blnIsTextInsideMarkROI)
                    //{
                    //    if (!blnIsTextInsideMarkROI)
                    //    {
                    //        if ((intBinaryOcvTextCenterX > objMarkROI.ref_ROIPositionX && intBinaryOcvTextCenterX < (objMarkROI.ref_ROIPositionX + objMarkROI.ref_ROIWidth)) &&
                    //        (intBinaryOcvTextCenterY > objMarkROI.ref_ROIPositionY && intBinaryOcvTextCenterY < (objMarkROI.ref_ROIPositionY + objMarkROI.ref_ROIHeight)))
                    //        {
                    //            blnIsTextInsideMarkROI = true;
                    //        }
                    //    }
                    //}



                    #region Get char inspection data
                    intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
#if (Debug_2_12 || Release_2_12)
                    for (uint ch = 0; ch < intNumTextChars; ch++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    for (int ch = 0; ch < intNumTextChars; ch++)
#endif

                    {
                        objSample = new Sample();
                        blnIsCharInsideMarkROI.Add(false);

                        if (blnWantUseOcvChar)
                        {
                            m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                            m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                            m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                            m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, -1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX + 1, intCenterY + 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, 1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX - 1, intCenterY - 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, 1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX + 1, intCenterY - 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, -1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX - 1, intCenterY + 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }

                            if (objOcvChar.TemplateBackgroundArea != -1 && objOcvChar.TemplateBackgroundArea != uint.MaxValue)
                            {
                                if (objOcvChar.Correlation <= 0 || objOcvChar.Correlation > 1)
                                    fScoreValue = 0;
                                else
                                    fScoreValue = objOcvChar.Correlation * 100;

                                objSample.intForeArea = (int)objOcvChar.SampleForegroundArea;
                                objSample.fForeAreaSumPercent = ((objOcvChar.SampleForegroundSum - objOcvChar.TemplateForegroundSum) / objOcvChar.TemplateForegroundSum) * 100;

                                if (blnWantUseSampleAreaScore)
                                {

                                    float fAreaScore = (float)Math.Abs(objOcvChar.SampleBackgroundArea - objOcvChar.TemplateBackgroundArea) / objOcvChar.TemplateBackgroundArea * 100;

                                    fScoreValue -= fAreaScore;
                                    if (fScoreValue <= 0)
                                        fScoreValue = 0;
                                }
                            }
                        }
                        else
                        {
                            fScoreValue = 0;
                        }

                        if (blnWantuseBinarOcvChar)
                        {
                            m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                            m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                            m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, -1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY + 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, 1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY - 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, 1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY - 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, -1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY + 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                        }
                        
                        if (blnWantuseBinarOcvChar && objOcvChar.TemplateBackgroundArea != -1 && objOcvChar.TemplateBackgroundArea != uint.MaxValue)
                        {
                            float fBinaScoreValue;
                            if (objOcvChar.Correlation <= 0 || objOcvChar.Correlation > 1)
                                fBinaScoreValue = 0;
                            else
                                fBinaScoreValue = objOcvChar.Correlation * 100;

                            if (blnWantUseSampleAreaScore)
                            {

                                float fAreaScore = (float)Math.Abs(objOcvChar.SampleBackgroundArea - objOcvChar.TemplateBackgroundArea) / objOcvChar.TemplateBackgroundArea * 100;

                                fBinaScoreValue -= fAreaScore;
                                if (fBinaScoreValue <= 0)
                                    fBinaScoreValue = 0;
                            }

                            if (fBinaScoreValue <= fScoreValue)
                            {
                                //2021-05-16 ZJYEOH : if blnWantUseOcvChar == true just use m_Ocv to get position else will draw wrong char 
                                if (blnWantUseOcvChar)
                                {
                                    m_Ocv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                    m_Ocv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                    objSample.fScore = fScoreValue;
                                }
                                else
                                {
                                    m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                    m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                    objSample.fScore = fBinaScoreValue;

                                    objSample.intForeArea = (int)objOcvChar.SampleForegroundArea;
                                    objSample.fForeAreaSumPercent = ((objOcvChar.SampleForegroundSum - objOcvChar.TemplateForegroundSum) / objOcvChar.TemplateForegroundSum) * 100;

                                }
                            }
                            else
                            {
                                if (float.IsInfinity(fBinaScoreValue))
                                {
                                    m_Ocv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                    m_Ocv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                    objSample.fScore = fScoreValue;
                                }
                                else
                                {
                                    m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                    m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                    objSample.fScore = fBinaScoreValue;

                                    objSample.intForeArea = (int)objOcvChar.SampleForegroundArea;
                                    objSample.fForeAreaSumPercent = ((objOcvChar.SampleForegroundSum - objOcvChar.TemplateForegroundSum) / objOcvChar.TemplateForegroundSum) * 100;

                                }
                            }

                            if (objSample.fScore < intMarkOriPositionScore)
                            {
                                if (!blnMinOcvInspected)
                                {
#if (Debug_2_12 || Release_2_12)
                                    //Inspect using gradient location mode
                                    m_MinOcv.Inspect(objSampleROI.ref_ROI, (uint)intThreshold);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                    //Inspect using gradient location mode
                                    m_MinOcv.Inspect(objSampleROI.ref_ROI, intThreshold);
#endif
                                    blnMinOcvInspected = true;
                                }

                                m_MinOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                                m_MinOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_MinOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                                m_MinOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                                if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                                {
                                    m_MinOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, -1);
                                    m_MinOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                    m_MinOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY + 1, 1, 1);
                                    m_MinOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                                }
                                if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                                {
                                    m_MinOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, 1);
                                    m_MinOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                    m_MinOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY - 1, 1, 1);
                                    m_MinOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                                }
                                if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                                {
                                    m_MinOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, 1);
                                    m_MinOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                    m_MinOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY - 1, 1, 1);
                                    m_MinOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                                }
                                if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                                {
                                    m_MinOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, -1);
                                    m_MinOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                    m_MinOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY + 1, 1, 1);
                                    m_MinOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                                }

                                if (objOcvChar.TemplateBackgroundArea != -1 && objOcvChar.TemplateBackgroundArea != uint.MaxValue)
                                {
                                    if (objOcvChar.Correlation <= 0 || objOcvChar.Correlation > 1)
                                        fScoreValue = 0;
                                    else
                                        fScoreValue = objOcvChar.Correlation * 100;

                                    if (blnWantUseSampleAreaScore)
                                    {

                                        float fAreaScore = (float)Math.Abs(objOcvChar.SampleBackgroundArea - objOcvChar.TemplateBackgroundArea) / objOcvChar.TemplateBackgroundArea * 100;

                                        fScoreValue -= fAreaScore;
                                        if (fScoreValue <= 0)
                                            fScoreValue = 0;
                                    }
                                }

                                if (objSample.fScore <= fScoreValue)
                                {
                                    m_MinOcv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                    m_MinOcv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                    objSample.fScore = fScoreValue;
                                }
                            }

                            objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                            objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                            objSample.intOrgX -= intOffsetROIX;
                            objSample.intOrgY -= intOffsetROIY;
                            objSample.intEndX -= intOffsetROIX;
                            objSample.intEndY -= intOffsetROIY;

                            //2020-11-03 ZJYEOH : Add Mark Score Offset
                            if (intMarkScoreOffset != 0)
                            {
                                if (intMarkScoreOffset > 0)
                                {
                                    if (objSample.fScore < 95)
                                    {
                                        objSample.fScore += intMarkScoreOffset;

                                        if (objSample.fScore < 5)
                                            objSample.fScore = 5;
                                        else if (objSample.fScore > 95)
                                            objSample.fScore = 95;

                                    }
                                }
                                else
                                {
                                    if (objSample.fScore > 5)
                                    {
                                        objSample.fScore += intMarkScoreOffset;

                                        if (objSample.fScore < 5)
                                            objSample.fScore = 5;
                                        else if (objSample.fScore > 95)
                                            objSample.fScore = 95;

                                    }
                                }
                            }

                            m_arrSampleChar.Add(objSample);
                        }
                        else if (blnWantUseOcvChar && fScoreValue >= 0)
                        {
                            m_Ocv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                            m_Ocv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                            objSample.fScore = fScoreValue;
                            objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                            objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                            objSample.intOrgX -= intOffsetROIX;
                            objSample.intOrgY -= intOffsetROIY;
                            objSample.intEndX -= intOffsetROIX;
                            objSample.intEndY -= intOffsetROIY;

                            //2020-11-03 ZJYEOH : Add Mark Score Offset
                            if (intMarkScoreOffset != 0)
                            {
                                if (intMarkScoreOffset > 0)
                                {
                                    if (objSample.fScore < 95)
                                    {
                                        objSample.fScore += intMarkScoreOffset;

                                        if (objSample.fScore < 5)
                                            objSample.fScore = 5;
                                        else if (objSample.fScore > 95)
                                            objSample.fScore = 95;

                                    }
                                }
                                else
                                {
                                    if (objSample.fScore > 5)
                                    {
                                        objSample.fScore += intMarkScoreOffset;

                                        if (objSample.fScore < 5)
                                            objSample.fScore = 5;
                                        else if (objSample.fScore > 95)
                                            objSample.fScore = 95;

                                    }
                                }
                            }

                            m_arrSampleChar.Add(objSample);
                        }
                        else
                        {
                            objSample.fScore = 0;
                            m_arrSampleChar.Add(objSample);
                        }
                        
                        int intCharCenterX = ((m_arrSampleChar[(int)ch].intOrgX + intOffsetROIX) + (m_arrSampleChar[(int)ch].intEndX + intOffsetROIX)) / 2;
                        int intCharCenterY = ((m_arrSampleChar[(int)ch].intOrgY + intOffsetROIY) + (m_arrSampleChar[(int)ch].intEndY + intOffsetROIY)) / 2;

                        if (blnWantUseOcvChar || blnWantuseBinarOcvChar)
                        {
                            if ((intCharCenterX > objMarkROI.ref_ROIPositionX && intCharCenterX < (objMarkROI.ref_ROIPositionX + objMarkROI.ref_ROIWidth)) &&
                                (intCharCenterY > objMarkROI.ref_ROIPositionY && intCharCenterY < (objMarkROI.ref_ROIPositionY + objMarkROI.ref_ROIHeight)))
                            {
                                blnIsCharInsideMarkROI[(int)ch] = true;
                            }
                        }
                        
                    }
                    #endregion

                    // 2021 03 02 - CCENG: Add Min Ocv Inspection. Sometime customer put OCV shift tolerance too high, this cause ocv cannot get the best correlation score value.



                    
                }

                ArrangeCharByCharNo();

                bool blnCharInsideMarkROI = true;
                for (int i = 0; i < blnIsCharInsideMarkROI.Count; i++)
                {
                    if (!blnIsCharInsideMarkROI[i])
                        blnCharInsideMarkROI = false;
                }

                if (objSampleROI != null)
                {
                    objSampleROI.Dispose();
                    objSampleROI = null;
                }

                return blnCharInsideMarkROI;
            }
            catch (Exception ex)
            {
                objTL.WriteLine("NOCV->Inspect ex: " + ex.ToString());
            }

            return false;
        }
        public bool Inspect_First2(TrackLog objTL, ROI objSearchROI, ROI objMarkROI, int intThreshold, int intOffsetROIX, int intOffsetROIY, float fMinTextOffsetAllowX, float fMinTextOffsetAllowY,
            bool blnWantUseSampleAreaScore, int intMarkScoreOffset, int intMarkOriPositionScore, float fAngle, int intMarkScoreMode)
        {
            //ImageDrawing objSampleImage = new ImageDrawing();
            try
            {
                // Keep roi start point for drawing
                m_pDrawStartPoint = new System.Drawing.Point(objSearchROI.ref_ROI.TotalOrgX, objSearchROI.ref_ROI.TotalOrgY);

                // Get image roi

                ROI objSampleROI = new ROI();
                GetImageROI(objSearchROI, ref m_objSampleImage, ref objSampleROI);

                bool blnWantDebug = false;
                if (blnWantDebug)
                {
                    m_BinaOcv.Save("D:\\TS\\2OCV.ocv");
                    m_Ocv.Save("D:\\TS\\1OCV.ocv");
                    objSampleROI.SaveImage("D:\\TS\\3objSampleROI.bmp");
                }

                bool blnMinOcvInspected = false;

                //2020-11-09 ZJYEOH : set angle tolerance for tilt inspect
                SetTextsSkewTolerance(-fAngle);

                //2020-06-17 ZJYEOH : Just in case not using correct LocationMode
                if (m_Ocv.LocationMode != ELocationMode.Gradient)
                    m_Ocv.LocationMode = ELocationMode.Gradient;
#if (Debug_2_12 || Release_2_12)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objSampleROI.ref_ROI, (uint)intThreshold);

                if (intMarkScoreMode != 1)
                {
                    //2020-06-17 ZJYEOH : Just in case not using correct LocationMode
                    if (m_BinaOcv.LocationMode != ELocationMode.Binarized)
                        m_BinaOcv.LocationMode = ELocationMode.Binarized;

                    //Inspect using binarized location mode
                    m_BinaOcv.Inspect(objSampleROI.ref_ROI, (uint)intThreshold);
                }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                //Inspect using gradient location mode
                m_Ocv.Inspect(objSampleROI.ref_ROI, intThreshold);

                //2020-06-17 ZJYEOH : Just in case not using correct LocationMode
                if (m_BinaOcv.LocationMode != ELocationMode.Binarized)
                    m_BinaOcv.LocationMode = ELocationMode.Binarized;

                //Inspect using binarized location mode
                m_BinaOcv.Inspect(objSampleROI.ref_ROI, intThreshold);

#endif

                //2020-06-17 ZJYEOH : Just in case not using correct LocationMode
                if (m_Ocv.LocationMode != ELocationMode.Gradient)
                    m_Ocv.LocationMode = ELocationMode.Gradient;

                m_arrSampleChar.Clear();
                m_arrSampleText.Clear();
                int intNumTextChars;
                int intNumTexts = (int)m_Ocv.NumTexts;
                int intCenterX = 0, intCenterY = 0;
                int intOcvTextCenterX = 0, intOcvTextCenterY = 0;
                int intBinaryOcvTextCenterX = 0, intBinaryOcvTextCenterY = 0;
                float fScoreValue = -1;
                //bool blnIsTextInsideMarkROI = false;
                List<bool> blnIsCharInsideMarkROI = new List<bool>();
                Sample objSample;
                EOCVChar objOcvChar = new EOCVChar();
                EOCVText objOcvText = new EOCVText();
                EOCVText objBinaOcvText = new EOCVText();
#if (Debug_2_12 || Release_2_12)
                for (uint tx = 0; tx < intNumTexts; tx++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                for (int tx = 0; tx < intNumTexts; tx++)
#endif

                {
                    #region Get text inspection data
                    m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, 0, 0);
                    m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                    m_Ocv.SelectSampleTexts(intOcvTextCenterX, intOcvTextCenterY, 1, 1);
                    m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, -1, -1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX + 1, intOcvTextCenterY + 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }
                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, 1, 1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX - 1, intOcvTextCenterY - 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }
                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, -1, 1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX + 1, intOcvTextCenterY - 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }
                    if (objOcvText.TemplateBackgroundArea == -1 || objOcvText.TemplateBackgroundArea == uint.MaxValue)
                    {
                        m_Ocv.GetTextPoint(tx, out intOcvTextCenterX, out intOcvTextCenterY, 1, -1);
                        m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_Ocv.SelectSampleTexts(intOcvTextCenterX - 1, intOcvTextCenterY + 1, 1, 1);
                        m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);
                    }

                    if (objOcvText.TemplateBackgroundArea != -1 && objOcvText.TemplateBackgroundArea != uint.MaxValue)
                    {
                        if (objOcvText.Correlation <= 0 || objOcvText.Correlation > 1)
                            fScoreValue = 0;
                        else
                            fScoreValue = objOcvText.Correlation * 100;
                    }

                    if (intMarkScoreMode != 1)
                    {
                        m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, 0, 0);
                        m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                        m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX, intBinaryOcvTextCenterY, 1, 1);
                        m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);

                        if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                        {
                            m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, -1, -1);
                            m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX + 1, intBinaryOcvTextCenterY + 1, 1, 1);
                            m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                        }
                        if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                        {
                            m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, 1, 1);
                            m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX - 1, intBinaryOcvTextCenterY - 1, 1, 1);
                            m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                        }
                        if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                        {
                            m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, -1, 1);
                            m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX + 1, intBinaryOcvTextCenterY - 1, 1, 1);
                            m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                        }
                        if (objBinaOcvText.TemplateBackgroundArea == -1 || objBinaOcvText.TemplateBackgroundArea == uint.MaxValue)
                        {
                            m_BinaOcv.GetTextPoint(tx, out intBinaryOcvTextCenterX, out intBinaryOcvTextCenterY, 1, -1);
                            m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTexts(intBinaryOcvTextCenterX - 1, intBinaryOcvTextCenterY + 1, 1, 1);
                            m_BinaOcv.GatherTextsParameters(objBinaOcvText, ESelectionFlag.True);
                        }
                    }

                    bool blnWantUseOcvChar = true;
                    bool blnWantuseBinarOcvChar = true;
                    objSample = new Sample();
                    if (objBinaOcvText.TemplateBackgroundArea != -1 && objBinaOcvText.TemplateBackgroundArea != uint.MaxValue)
                    {
                        float fBinaScoreValue;
                        if (objBinaOcvText.Correlation <= 0 || objBinaOcvText.Correlation > 1)
                            fBinaScoreValue = 0;
                        else
                            fBinaScoreValue = objBinaOcvText.Correlation * 100;

                        if (fBinaScoreValue <= fScoreValue)
                        {
                            //2021-05-16 ZJYEOH : if blnWantUseOcvChar == true just use m_Ocv to get position else will draw wrong char 
                            if (blnWantUseOcvChar)
                            {
                                m_Ocv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                m_Ocv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);

                                objSample.intLocMode = 1;
                                objSample.fScore = fScoreValue;
                            }
                            else
                            {
                                m_BinaOcv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                m_BinaOcv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                objSample.intLocMode = 2;
                                objSample.fScore = fBinaScoreValue;
                            }
                        }
                        else
                        {
                            m_BinaOcv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                            m_BinaOcv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);
                            objSample.intLocMode = 2;
                            objSample.fScore = fBinaScoreValue;
                        }

                        objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                        objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                        objSample.intOrgX -= intOffsetROIX;
                        objSample.intOrgY -= intOffsetROIY;
                        objSample.intEndX -= intOffsetROIX;
                        objSample.intEndY -= intOffsetROIY;

                        //2020-11-03 ZJYEOH : Add Mark Score Offset
                        if (intMarkScoreOffset != 0)
                        {
                            if (intMarkScoreOffset > 0)
                            {
                                if (objSample.fScore < 95)
                                {
                                    objSample.fScore += intMarkScoreOffset;

                                    if (objSample.fScore < 5)
                                        objSample.fScore = 5;
                                    else if (objSample.fScore > 95)
                                        objSample.fScore = 95;

                                }
                            }
                            else
                            {
                                if (objSample.fScore > 5)
                                {
                                    objSample.fScore += intMarkScoreOffset;

                                    if (objSample.fScore < 5)
                                        objSample.fScore = 5;
                                    else if (objSample.fScore > 95)
                                        objSample.fScore = 95;

                                }
                            }
                        }

                        m_arrSampleText.Add(objSample);

                        if ((Math.Abs(intOcvTextCenterX - intBinaryOcvTextCenterX) < fMinTextOffsetAllowX) &&
                            (Math.Abs(intOcvTextCenterY - intBinaryOcvTextCenterY) < fMinTextOffsetAllowY))
                        {
                            blnWantUseOcvChar = true;
                            blnWantuseBinarOcvChar = true;
                        }
                        else
                        {
                            if (fBinaScoreValue <= fScoreValue)
                            {
                                blnWantUseOcvChar = true;
                                blnWantuseBinarOcvChar = false;
                            }
                            else
                            {
                                blnWantUseOcvChar = false;
                                blnWantuseBinarOcvChar = true;
                            }
                        }
                    }
                    else if (fScoreValue >= 0)
                    {
                        m_Ocv.GetTextPoint(tx, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                        m_Ocv.GetTextPoint(tx, out objSample.intEndX, out objSample.intEndY, 1, 1);
                        objSample.intLocMode = 1;
                        objSample.fScore = fScoreValue;
                        objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                        objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                        objSample.intOrgX -= intOffsetROIX;
                        objSample.intOrgY -= intOffsetROIY;
                        objSample.intEndX -= intOffsetROIX;
                        objSample.intEndY -= intOffsetROIY;

                        //2020-11-03 ZJYEOH : Add Mark Score Offset
                        if (intMarkScoreOffset != 0)
                        {
                            if (intMarkScoreOffset > 0)
                            {
                                if (objSample.fScore < 95)
                                {
                                    objSample.fScore += intMarkScoreOffset;

                                    if (objSample.fScore < 5)
                                        objSample.fScore = 5;
                                    else if (objSample.fScore > 95)
                                        objSample.fScore = 95;

                                }
                            }
                            else
                            {
                                if (objSample.fScore > 5)
                                {
                                    objSample.fScore += intMarkScoreOffset;

                                    if (objSample.fScore < 5)
                                        objSample.fScore = 5;
                                    else if (objSample.fScore > 95)
                                        objSample.fScore = 95;

                                }
                            }
                        }

                        m_arrSampleChar.Add(objSample);

                        blnWantUseOcvChar = true;
                        blnWantuseBinarOcvChar = false;
                    }
                    else
                    {
                        objSample.intLocMode = 1;
                        objSample.fScore = 0;
                        m_arrSampleChar.Add(objSample);
                    }

                    #endregion

                    //if (blnWantUseOcvChar)
                    //{
                    //    if ((intOcvTextCenterX > objMarkROI.ref_ROIPositionX && intOcvTextCenterX < (objMarkROI.ref_ROIPositionX + objMarkROI.ref_ROIWidth)) &&
                    //        (intOcvTextCenterY > objMarkROI.ref_ROIPositionY && intOcvTextCenterY < (objMarkROI.ref_ROIPositionY + objMarkROI.ref_ROIHeight)))
                    //    {
                    //        blnIsTextInsideMarkROI = true;
                    //    }
                    //}

                    //if (blnWantuseBinarOcvChar && !blnIsTextInsideMarkROI)
                    //{
                    //    if (!blnIsTextInsideMarkROI)
                    //    {
                    //        if ((intBinaryOcvTextCenterX > objMarkROI.ref_ROIPositionX && intBinaryOcvTextCenterX < (objMarkROI.ref_ROIPositionX + objMarkROI.ref_ROIWidth)) &&
                    //        (intBinaryOcvTextCenterY > objMarkROI.ref_ROIPositionY && intBinaryOcvTextCenterY < (objMarkROI.ref_ROIPositionY + objMarkROI.ref_ROIHeight)))
                    //        {
                    //            blnIsTextInsideMarkROI = true;
                    //        }
                    //    }
                    //}



                    #region Get char inspection data
                    intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
#if (Debug_2_12 || Release_2_12)
                    for (uint ch = 0; ch < intNumTextChars; ch++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    for (int ch = 0; ch < intNumTextChars; ch++)
#endif

                    {
                        objSample = new Sample();
                        blnIsCharInsideMarkROI.Add(false);

                        if (blnWantUseOcvChar)
                        {
                            m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                            m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                            m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                            m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, -1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX + 1, intCenterY + 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, 1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX - 1, intCenterY - 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, 1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX + 1, intCenterY - 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, -1);
                                m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_Ocv.SelectSampleTextsChars(intCenterX - 1, intCenterY + 1, 1, 1);
                                m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }

                            if (objOcvChar.TemplateBackgroundArea != -1 && objOcvChar.TemplateBackgroundArea != uint.MaxValue)
                            {
                                if (objOcvChar.Correlation <= 0 || objOcvChar.Correlation > 1)
                                    fScoreValue = 0;
                                else
                                    fScoreValue = objOcvChar.Correlation * 100;

                                objSample.intForeArea = (int)objOcvChar.SampleForegroundArea;
                                objSample.fForeAreaSumPercent = ((objOcvChar.SampleForegroundSum - objOcvChar.TemplateForegroundSum) / objOcvChar.TemplateForegroundSum) * 100;

                                if (blnWantUseSampleAreaScore)
                                {

                                    float fAreaScore = (float)Math.Abs(objOcvChar.SampleBackgroundArea - objOcvChar.TemplateBackgroundArea) / objOcvChar.TemplateBackgroundArea * 100;

                                    fScoreValue -= fAreaScore;
                                    if (fScoreValue <= 0)
                                        fScoreValue = 0;
                                }
                            }
                        }
                        else
                        {
                            fScoreValue = 0;
                        }

                        if (blnWantuseBinarOcvChar)
                        {
                            m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                            m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                            m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, -1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY + 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, 1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY - 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, 1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY - 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                            if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, -1);
                                m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_BinaOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY + 1, 1, 1);
                                m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                            }
                        }

                        if (blnWantuseBinarOcvChar && objOcvChar.TemplateBackgroundArea != -1 && objOcvChar.TemplateBackgroundArea != uint.MaxValue)
                        {
                            float fBinaScoreValue;
                            if (objOcvChar.Correlation <= 0 || objOcvChar.Correlation > 1)
                                fBinaScoreValue = 0;
                            else
                                fBinaScoreValue = objOcvChar.Correlation * 100;

                            if (blnWantUseSampleAreaScore)
                            {

                                float fAreaScore = (float)Math.Abs(objOcvChar.SampleBackgroundArea - objOcvChar.TemplateBackgroundArea) / objOcvChar.TemplateBackgroundArea * 100;

                                fBinaScoreValue -= fAreaScore;
                                if (fBinaScoreValue <= 0)
                                    fBinaScoreValue = 0;
                            }

                            if (fBinaScoreValue <= fScoreValue)
                            {
                                //2021-05-16 ZJYEOH : if blnWantUseOcvChar == true just use m_Ocv to get position else will draw wrong char 
                                if (blnWantUseOcvChar)
                                {
                                    m_Ocv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                    m_Ocv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                    objSample.fScore = fScoreValue;
                                }
                                else
                                {
                                    m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                    m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                    objSample.fScore = fBinaScoreValue;

                                    objSample.intForeArea = (int)objOcvChar.SampleForegroundArea;
                                    objSample.fForeAreaSumPercent = ((objOcvChar.SampleForegroundSum - objOcvChar.TemplateForegroundSum) / objOcvChar.TemplateForegroundSum) * 100;

                                }
                            }
                            else
                            {
                                m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                m_BinaOcv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                objSample.fScore = fBinaScoreValue;

                                objSample.intForeArea = (int)objOcvChar.SampleForegroundArea;
                                objSample.fForeAreaSumPercent = ((objOcvChar.SampleForegroundSum - objOcvChar.TemplateForegroundSum) / objOcvChar.TemplateForegroundSum) * 100;

                            }

                            if (objSample.fScore < intMarkOriPositionScore)
                            {
                                if (!blnMinOcvInspected)
                                {
#if (Debug_2_12 || Release_2_12)
                                    //Inspect using gradient location mode
                                    m_MinOcv.Inspect(objSampleROI.ref_ROI, (uint)intThreshold);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                    //Inspect using gradient location mode
                                    m_MinOcv.Inspect(objSampleROI.ref_ROI, intThreshold);
#endif
                                    blnMinOcvInspected = true;
                                }

                                m_MinOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                                m_MinOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                m_MinOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                                m_MinOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                                if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                                {
                                    m_MinOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, -1);
                                    m_MinOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                    m_MinOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY + 1, 1, 1);
                                    m_MinOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                                }
                                if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                                {
                                    m_MinOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, 1);
                                    m_MinOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                    m_MinOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY - 1, 1, 1);
                                    m_MinOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                                }
                                if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                                {
                                    m_MinOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, -1, 1);
                                    m_MinOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                    m_MinOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY - 1, 1, 1);
                                    m_MinOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                                }
                                if (objOcvChar.TemplateBackgroundArea == -1 || objOcvChar.TemplateBackgroundArea == uint.MaxValue)
                                {
                                    m_MinOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 1, -1);
                                    m_MinOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                                    m_MinOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY + 1, 1, 1);
                                    m_MinOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                                }

                                if (objOcvChar.TemplateBackgroundArea != -1 && objOcvChar.TemplateBackgroundArea != uint.MaxValue)
                                {
                                    if (objOcvChar.Correlation <= 0 || objOcvChar.Correlation > 1)
                                        fScoreValue = 0;
                                    else
                                        fScoreValue = objOcvChar.Correlation * 100;

                                    if (blnWantUseSampleAreaScore)
                                    {

                                        float fAreaScore = (float)Math.Abs(objOcvChar.SampleBackgroundArea - objOcvChar.TemplateBackgroundArea) / objOcvChar.TemplateBackgroundArea * 100;

                                        fScoreValue -= fAreaScore;
                                        if (fScoreValue <= 0)
                                            fScoreValue = 0;
                                    }
                                }

                                if (objSample.fScore <= fScoreValue)
                                {
                                    m_MinOcv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                                    m_MinOcv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                                    objSample.fScore = fScoreValue;
                                }
                            }

                            objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                            objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                            objSample.intOrgX -= intOffsetROIX;
                            objSample.intOrgY -= intOffsetROIY;
                            objSample.intEndX -= intOffsetROIX;
                            objSample.intEndY -= intOffsetROIY;

                            //2020-11-03 ZJYEOH : Add Mark Score Offset
                            if (intMarkScoreOffset != 0)
                            {
                                if (intMarkScoreOffset > 0)
                                {
                                    if (objSample.fScore < 95)
                                    {
                                        objSample.fScore += intMarkScoreOffset;

                                        if (objSample.fScore < 5)
                                            objSample.fScore = 5;
                                        else if (objSample.fScore > 95)
                                            objSample.fScore = 95;

                                    }
                                }
                                else
                                {
                                    if (objSample.fScore > 5)
                                    {
                                        objSample.fScore += intMarkScoreOffset;

                                        if (objSample.fScore < 5)
                                            objSample.fScore = 5;
                                        else if (objSample.fScore > 95)
                                            objSample.fScore = 95;

                                    }
                                }
                            }

                            m_arrSampleChar.Add(objSample);
                        }
                        else if (blnWantUseOcvChar && fScoreValue >= 0)
                        {
                            m_Ocv.GetTextCharPoint(tx, ch, out objSample.intOrgX, out objSample.intOrgY, -1, -1);
                            m_Ocv.GetTextCharPoint(tx, ch, out objSample.intEndX, out objSample.intEndY, 1, 1);
                            objSample.fScore = fScoreValue;
                            objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                            objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                            objSample.intOrgX -= intOffsetROIX;
                            objSample.intOrgY -= intOffsetROIY;
                            objSample.intEndX -= intOffsetROIX;
                            objSample.intEndY -= intOffsetROIY;

                            //2020-11-03 ZJYEOH : Add Mark Score Offset
                            if (intMarkScoreOffset != 0)
                            {
                                if (intMarkScoreOffset > 0)
                                {
                                    if (objSample.fScore < 95)
                                    {
                                        objSample.fScore += intMarkScoreOffset;

                                        if (objSample.fScore < 5)
                                            objSample.fScore = 5;
                                        else if (objSample.fScore > 95)
                                            objSample.fScore = 95;

                                    }
                                }
                                else
                                {
                                    if (objSample.fScore > 5)
                                    {
                                        objSample.fScore += intMarkScoreOffset;

                                        if (objSample.fScore < 5)
                                            objSample.fScore = 5;
                                        else if (objSample.fScore > 95)
                                            objSample.fScore = 95;

                                    }
                                }
                            }

                            m_arrSampleChar.Add(objSample);
                        }
                        else
                        {
                            objSample.fScore = 0;
                            m_arrSampleChar.Add(objSample);
                        }

                        int intCharCenterX = ((m_arrSampleChar[(int)ch].intOrgX + intOffsetROIX) + (m_arrSampleChar[(int)ch].intEndX + intOffsetROIX)) / 2;
                        int intCharCenterY = ((m_arrSampleChar[(int)ch].intOrgY + intOffsetROIY) + (m_arrSampleChar[(int)ch].intEndY + intOffsetROIY)) / 2;

                        if (blnWantUseOcvChar || blnWantuseBinarOcvChar)
                        {
                            if ((intCharCenterX > objMarkROI.ref_ROIPositionX && intCharCenterX < (objMarkROI.ref_ROIPositionX + objMarkROI.ref_ROIWidth)) &&
                                (intCharCenterY > objMarkROI.ref_ROIPositionY && intCharCenterY < (objMarkROI.ref_ROIPositionY + objMarkROI.ref_ROIHeight)))
                            {
                                blnIsCharInsideMarkROI[(int)ch] = true;
                            }
                        }

                    }
                    #endregion



                }

                ArrangeCharByCharNo();

                bool blnCharInsideMarkROI = true;
                for (int i = 0; i < blnIsCharInsideMarkROI.Count; i++)
                {
                    if (!blnIsCharInsideMarkROI[i])
                        blnCharInsideMarkROI = false;
                }

                if (objSampleROI != null)
                {
                    objSampleROI.Dispose();
                    objSampleROI = null;
                }

                return blnCharInsideMarkROI;
            }
            catch (Exception ex)
            {
                objTL.WriteLine("NOCV->Inspect ex: " + ex.ToString());
            }

            return false;
        }
        public bool IsInOcvCharArea(Contour objContour, int intCharNo)
        {
            int intSamplingStep = (m_arrSampleChar[intCharNo].intWidth + m_arrSampleChar[intCharNo].intHeight) / 10;

            if (intSamplingStep < 4)
                intSamplingStep = 4;
            else if (intSamplingStep > 10)
                intSamplingStep = 10;

            if (objContour.MatchObject(0, m_arrSampleChar[intCharNo].intOrgX, m_arrSampleChar[intCharNo].intOrgY,
               m_arrSampleChar[intCharNo].intEndX, m_arrSampleChar[intCharNo].intEndY, intSamplingStep))
            {
                return true;
            }

            return false;
        }

        public bool IsInOcvCharArea(Contour objContour, int intCharNo, int intBlobOCVOffSetX, int intBlobOCVOffSetY)
        {
            // 2019 07 11 - CCENG: use sampling for faster process. use 30 because it is good enough to check all direction. The 30 may change to settingable if need to change next time.
            int intLoopCount = 30;   // (m_arrSampleChar[intCharNo].intWidth + m_arrSampleChar[intCharNo].intHeight) / 10;

            //if (intSamplingStep < 4)
            //    intSamplingStep = 4;
            //else if (intSamplingStep > 10)
            //    intSamplingStep = 10;

            if (objContour.MatchObject(0, m_arrSampleChar[intCharNo].intOrgX - intBlobOCVOffSetX, m_arrSampleChar[intCharNo].intOrgY - intBlobOCVOffSetY,
               m_arrSampleChar[intCharNo].intEndX - intBlobOCVOffSetX, m_arrSampleChar[intCharNo].intEndY - intBlobOCVOffSetY, intLoopCount))
            {
                return true;
            }

            return false;
        }

        public bool IsInOcvCharArea(Contour objContour, ref int intMatchCharNo)
        {
            for (int i = 0; i < m_arrSampleChar.Count; i++)
            {
                // 2019 07 11 - CCENG: use sampling for faster process. use 30 because it is good enough to check all direction. The 30 may change to settingable if need to change next time.
                int intLoopCount = 30; // (m_arrSampleChar[i].intWidth + m_arrSampleChar[i].intHeight) / 10;

                //if (intSamplingStep < 4)
                //    intSamplingStep = 4;
                //else if (intSamplingStep > 10)
                //    intSamplingStep = 10;

                if (objContour.MatchObject(0, m_arrSampleChar[i].intOrgX, m_arrSampleChar[i].intOrgY,
                    m_arrSampleChar[i].intEndX, m_arrSampleChar[i].intEndY, intLoopCount))
                {
                    intMatchCharNo = i;
                    return true;
                }
            }

            return false;
        }

        public void Learn(ROI objROI, bool blnWhiteOnBlack)
        {
            m_Ocv.WhiteOnBlack = blnWhiteOnBlack;
            m_Ocv.Learn(objROI.ref_ROI, ESelectionFlag.True);
            m_BinaOcv = m_Ocv;
        }

        public void LoadOCVFile(String strPath)
        {
            //2020-06-17 ZJYEOH : Dispose and declare new OCV because previously make m_BinaOcv = m_Ocv

            //m_Ocv.Dispose();
            //m_Ocv = new EOCV();
            //m_Ocv.Load(strPath);
            //m_Ocv.LocationMode = ELocationMode.Gradient;

            //m_BinaOcv.Dispose();
            //m_BinaOcv = new EOCV();
            //m_BinaOcv.Load(strPath);
            //m_BinaOcv.LocationMode = ELocationMode.Binarized;

            if (m_BinaOcv != null)
            {
                m_BinaOcv.Dispose();
                m_BinaOcv = null;
            }

            if (m_Ocv != null)
            {
                m_Ocv.Dispose();
                m_Ocv = null;
            }

            m_Ocv = new EOCV();
            m_Ocv.Load(strPath);
            m_Ocv.LocationMode = ELocationMode.Gradient;

            m_BinaOcv = new EOCV();
            m_BinaOcv.Load(strPath);
            m_BinaOcv.LocationMode = ELocationMode.Binarized;

            if (m_MinOcv != null)
            {
                m_MinOcv.Dispose();
                m_MinOcv = null;
            }

            m_MinOcv = new EOCV();
            m_MinOcv.Load(strPath);
            m_MinOcv.LocationMode = ELocationMode.Gradient;

            SetCharsShiftXY_ForMinOcv(3, 3);
        }

        public void ResetPreviousSelectedChars()
        {
            m_Ocv.SelectTemplateObjects(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
            m_Ocv.SelectTemplateChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
        }

        public void SaveOCV(string strPath)
        {
            m_Ocv.Save(strPath);
        }

        public void SetCharNo(int intCharIndex, int intCharNo)
        {
            if (intCharIndex < m_arrCharNo.Count)
                m_arrCharNo[intCharIndex] = intCharNo;
            else
            {
                m_arrCharNo.Add(intCharNo);
            }
        }
        public void SetCharScore_ExcessMissingAffectScore(int intCharIndex, float fScore_50percent, bool IsBarPin1)
        {
            if (intCharIndex < m_arrSampleChar.Count)
            {
                Sample objSample = m_arrSampleChar[intCharIndex];

                float fOriginalScore = objSample.fScore;
                if (IsBarPin1)
                {
                    if (fOriginalScore < 70)
                        fOriginalScore = objSample.fScore / 2;
                    else
                    {
                        fOriginalScore = (((objSample.fScore - 70) / 30 * 130) + 70) / 2;
                    }
                }

                objSample.fScore = Math.Min(fOriginalScore, (fOriginalScore / 2) + fScore_50percent);
                m_arrSampleChar[intCharIndex] = objSample;
            }
        }

        public void SetCharScore_BarPin1AffectScore(int intCharIndex)
        {
            if (intCharIndex < m_arrSampleChar.Count)
            {
                Sample objSample = m_arrSampleChar[intCharIndex];

                // 2022 01 12 - CCENG: Bar Pin 1 formula : 
                float fNewBarPin1Score = objSample.fScore * 2f;

                if (fNewBarPin1Score < 70)
                {
                    objSample.fScore = fNewBarPin1Score;
                }
                else
                {
                    float fScoreAfter70 = (fNewBarPin1Score - 70) / 130 * 30 + 70;

                    objSample.fScore = fScoreAfter70;
                }
                m_arrSampleChar[intCharIndex] = objSample;
            }
        }

        public void SetCharXY(int intCharIndex, int intCenterX, int intCenterY)
        {
            if (intCharIndex < m_arrSampleChar.Count)
            {
                Sample objSample = new Sample();
                objSample = m_arrSampleChar[intCharIndex];
                objSample.intOrgX = intCenterX - m_arrSampleChar[intCharIndex].intWidth / 2;
                objSample.intOrgY = intCenterY - m_arrSampleChar[intCharIndex].intHeight / 2;
                objSample.intEndX = intCenterX + m_arrSampleChar[intCharIndex].intWidth / 2;
                objSample.intEndY = intCenterY + m_arrSampleChar[intCharIndex].intHeight / 2;
                m_arrSampleChar.RemoveAt(intCharIndex);
                m_arrSampleChar.Insert(intCharIndex, objSample);
            }
        }
        public void SelectTemplateChars(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            int intOrgX, intOrgY, intWidth, intHeight;

            if (p1.X < p2.X)
            {
                intOrgX = p1.X;
                intWidth = p2.X - p1.X;
            }
            else
            {
                intOrgX = p2.X;
                intWidth = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                intOrgY = p1.Y;
                intHeight = p2.Y - p1.Y;
            }
            else
            {
                intOrgY = p2.Y;
                intHeight = p1.Y - p2.Y;
            }

            m_Ocv.SelectTemplateObjects(intOrgX, intOrgY, intWidth, intHeight, ESelectionFlag.Any);
            m_Ocv.SelectTemplateChars(intOrgX, intOrgY, intWidth, intHeight, ESelectionFlag.Any);
        }

        public void SelectTemplateChars_AlwaysTrue(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            int intOrgX, intOrgY, intWidth, intHeight;

            if (p1.X < p2.X)
            {
                intOrgX = p1.X;
                intWidth = p2.X - p1.X;
            }
            else
            {
                intOrgX = p2.X;
                intWidth = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                intOrgY = p1.Y;
                intHeight = p2.Y - p1.Y;
            }
            else
            {
                intOrgY = p2.Y;
                intHeight = p1.Y - p2.Y;
            }

            m_Ocv.SelectTemplateObjects(intOrgX, intOrgY, intWidth, intHeight, ESelectionFlag.False);    // SelectionFlag.False mean selected.
            m_Ocv.SelectTemplateChars(intOrgX, intOrgY, intWidth, intHeight, ESelectionFlag.False);

        }

        public void SelectTemplateChars_AlwaysFalse(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            int intOrgX, intOrgY, intWidth, intHeight;

            if (p1.X < p2.X)
            {
                intOrgX = p1.X;
                intWidth = p2.X - p1.X;
            }
            else
            {
                intOrgX = p2.X;
                intWidth = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                intOrgY = p1.Y;
                intHeight = p2.Y - p1.Y;
            }
            else
            {
                intOrgY = p2.Y;
                intHeight = p1.Y - p2.Y;
            }

            m_Ocv.SelectTemplateObjects(intOrgX, intOrgY, intWidth, intHeight, ESelectionFlag.True);    // SelectionFlag.True mean deselected.
            m_Ocv.SelectTemplateChars(intOrgX, intOrgY, intWidth, intHeight, ESelectionFlag.True);

        }

        public void SortTemplateCharsByCenterPoint()
        {
            int intNumChars;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
            m_arrCharNo.Clear();
            List<PointF> arrCenterPoints = new List<PointF>();
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumChars; ch++)
                {
#endif

                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);

                    int intSelectedIndex = 0;
                    for (int i = 0; i < arrCenterPoints.Count; i++)
                    {
                        // same row
                        if ((intCenterY < (arrCenterPoints[i].Y + 10)) &&
                            (intCenterY > (arrCenterPoints[i].Y - 10)))
                        {
                            if (intCenterX > arrCenterPoints[i].X)
                                intSelectedIndex++;
                        }
                        // different row
                        else
                        {
                            if (intCenterY > (arrCenterPoints[i].Y - 10))
                                intSelectedIndex++;
                        }
                    }

                    if (intSelectedIndex == m_arrCharNo.Count)
                        arrCenterPoints.Add(new PointF(intCenterX, intCenterY));
                    else
                        arrCenterPoints.Insert(intSelectedIndex, new PointF(intCenterX, intCenterY));

                    for (int i = 0; i < m_arrCharNo.Count; i++)
                    {
                        if ((int)m_arrCharNo[i] >= intSelectedIndex)
                            m_arrCharNo[i] = (int)m_arrCharNo[i] + 1;
                    }

                    m_arrCharNo.Add(intSelectedIndex);
                }
            }
        }

        public void SetCharsShiftXY(float fXTolerance, float fYTolerance)
        {
            int intNumChars;
            int intCharIndex = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
            EOCVChar objOcvChar = new EOCVChar();
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumChars; ch++)
                {
#endif

                    // Select OCV Char
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);

                    // Get OcvChar from OCV
                    m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                    // Set tolerance to OcvChar
                    objOcvChar.ShiftXTolerance = (int)Math.Round(fXTolerance);
                    objOcvChar.ShiftYTolerance = (int)Math.Round(fYTolerance);

                    // Set OcvChar to OCV
                    m_Ocv.ScatterTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                    // ------- Binary OCV -----------------------
                    // Select OCV Char
                    m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                    m_BinaOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);

                    // Get OcvChar from OCV
                    m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                    // Set tolerance to OcvChar
                    objOcvChar.ShiftXTolerance = (int)Math.Round(fXTolerance);
                    objOcvChar.ShiftYTolerance = (int)Math.Round(fYTolerance);

                    // Set OcvChar to OCV
                    m_BinaOcv.ScatterTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);


                    intCharIndex++;
                }
            }
        }

        public void SetCharsShiftXY(float[] arrXTolerance, float[] arrYTolerance, float fCharROIOffsetX, float fCharROIOffsetY)
        {
            int intNumChars;
            int intCharIndex = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
            EOCVChar objOcvChar = new EOCVChar();
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumChars; ch++)
                {
#endif

                    // Get OcvChar from OCV
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    //m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True); // 2019 08 13 - CCENG: Change from ESelectionFlag.True to ESelectionFlag.Any, ESelectionFlag.True 
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);

                    // Get OcvChar from OCV
                    m_Ocv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                    // Set tolerance to OcvChar
                    if (intCharIndex < arrXTolerance.Length)
                        objOcvChar.ShiftXTolerance = (int)Math.Round(arrXTolerance[intCharIndex] + fCharROIOffsetX);
                    else if(arrXTolerance.Length > 0) //2021-08-04 ZJYEOH : To avoid index out of range happen
                        objOcvChar.ShiftXTolerance = (int)Math.Round(arrXTolerance[arrXTolerance.Length - 1] + fCharROIOffsetX);   // Use the last tolerance if no enough
                    else
                        objOcvChar.ShiftXTolerance = (int)Math.Round(fCharROIOffsetX);

                    if (intCharIndex < arrYTolerance.Length)
                        objOcvChar.ShiftYTolerance = (int)Math.Round(arrYTolerance[intCharIndex] + fCharROIOffsetY);
                    else if (arrYTolerance.Length > 0) //2021-08-04 ZJYEOH : To avoid index out of range happen
                        objOcvChar.ShiftYTolerance = (int)Math.Round(arrYTolerance[arrXTolerance.Length - 1] + fCharROIOffsetY);   // Use the last tolerance if no enough
                    else
                        objOcvChar.ShiftYTolerance = (int)Math.Round(fCharROIOffsetY);

                    // Set OcvChar to OCV
                    m_Ocv.ScatterTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                    // Get OcvChar from OCV
                    m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    //m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                    m_BinaOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True); // 2019 08 13 - CCENG: Change from ESelectionFlag.True to ESelectionFlag.Any, ESelectionFlag.True 
                    m_BinaOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);

                    // Get OcvChar from OCV
                    m_BinaOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                    // Set tolerance to OcvChar
                    if (intCharIndex < arrXTolerance.Length)
                        objOcvChar.ShiftXTolerance = (int)Math.Round(arrXTolerance[intCharIndex] + fCharROIOffsetX);
                    else if (arrXTolerance.Length > 0) //2021-08-04 ZJYEOH : To avoid index out of range happen
                        objOcvChar.ShiftXTolerance = (int)Math.Round(arrXTolerance[arrXTolerance.Length - 1] + fCharROIOffsetX);   // Use the last tolerance if no enough
                    else
                        objOcvChar.ShiftXTolerance = (int)Math.Round(fCharROIOffsetX);

                    if (intCharIndex < arrYTolerance.Length)
                        objOcvChar.ShiftYTolerance = (int)Math.Round(arrYTolerance[intCharIndex] + fCharROIOffsetY);
                    else if (arrYTolerance.Length > 0) //2021-08-04 ZJYEOH : To avoid index out of range happen
                        objOcvChar.ShiftYTolerance = (int)Math.Round(arrYTolerance[arrXTolerance.Length - 1] + fCharROIOffsetY);   // Use the last tolerance if no enough
                    else
                        objOcvChar.ShiftYTolerance = (int)Math.Round(fCharROIOffsetY);

                    // Set OcvChar to OCV
                    m_BinaOcv.ScatterTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                    intCharIndex++;
                }
            }
        }

        public void SetTemplateImage(ROI objROI, int intOriX, int intOriY, int intWidth, int intHeight)
        {
            //m_Ocv.TemplateImage = new EROIBW8(objROI.ref_ROI, intOriX, intOriY, intWidth, intHeight);
            //m_BinaOcv.TemplateImage = new EROIBW8(objROI.ref_ROI, intOriX, intOriY, intWidth, intHeight);
            //////m_Ocv.TemplateImage = new EROIBW8();
            m_Ocv.TemplateImage = objROI.ref_ROI;
            m_Ocv.TemplateImage.OrgX = intOriX;
            m_Ocv.TemplateImage.OrgY = intOriY;
            m_Ocv.TemplateImage.Width = intWidth;
            m_Ocv.TemplateImage.Height = intHeight;

            ////////m_BinaOcv.TemplateImage = new EROIBW8();
            m_BinaOcv.TemplateImage = objROI.ref_ROI;
            m_BinaOcv.TemplateImage.OrgX = intOriX;
            m_BinaOcv.TemplateImage.OrgY = intOriY;
            m_BinaOcv.TemplateImage.Width = intWidth;
            m_BinaOcv.TemplateImage.Height = intHeight;
  
        }

        public void SetTemplateImage(ROI objROI)
        {
            m_Ocv.TemplateImage = objROI.ref_ROI;
            m_BinaOcv.TemplateImage = objROI.ref_ROI;
        }

        public void SetTextsShiftXY(int intXTolerance, int intYTolerance)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
            EOCVText objOcvText = new EOCVText();
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                // Select OCV Text
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);

                //Select text center point with width, height 1
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);

                // Get EOCVText from OCV
                m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

                // Set tolerance to OcvText
                objOcvText.ShiftXTolerance = intXTolerance;
                objOcvText.ShiftYTolerance = intYTolerance;

                // Set EOCVText to OCV
                m_Ocv.ScatterTextsParameters(objOcvText, ESelectionFlag.True);
            }
        }
        public void SetTextsShiftXY(List<int> intXTolerance, List<int> intYTolerance)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
            EOCVText objOcvText = new EOCVText();
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                // Select OCV Text
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);

                //Select text center point with width, height 1
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);

                // Get EOCVText from OCV
                m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

                // Set tolerance to OcvText
                objOcvText.ShiftXTolerance = intXTolerance[(int)tx];
                objOcvText.ShiftYTolerance = intYTolerance[(int)tx];

                // Set EOCVText to OCV
                m_Ocv.ScatterTextsParameters(objOcvText, ESelectionFlag.True);
            }
        }
        public void SetTextsShiftXY(int intROIStartX, int intROIStartY, int intROIEndX, int intROIEndY, 
            int intMethod, int intTolX, int intTolY)
        {
            int intStartX, intStartY, intEndX, intEndY;
            int intXTolerance = intTolX, intYTolerance = intTolY;

            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
            EOCVText objOcvText = new EOCVText();
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                intStartX = intStartY = intEndX = intEndY = 0;

                // Get ocv text start point and end point
                m_Ocv.GetTextPoint(tx, out intStartX, out intStartY, -1, -1);
                m_Ocv.GetTextPoint(tx, out intEndX, out intEndY, 1, 1);

                if (intMethod == 0) // Max Method
                {
                    // Calculate ocv text position tolerance. Get longest tolerance.
                    if ((intStartX - intROIStartX) > (intROIEndX - intEndX))
                        intXTolerance = intStartX - intROIStartX;
                    else
                        intXTolerance = intROIEndX - intEndX;

                    if ((intStartY - intROIStartY) > (intROIEndY - intEndY))
                        intYTolerance = intStartY - intROIStartY;
                    else
                        intYTolerance = intROIEndY - intEndY;
                }
                else if (intMethod == 1) // Min Method
                {
                    // Calculate ocv text position tolerance. Get longest tolerance.
                    if ((intStartX - intROIStartX) < (intROIEndX - intEndX))
                        intXTolerance = intStartX - intROIStartX;
                    else
                        intXTolerance = intROIEndX - intEndX;

                    if ((intStartY - intROIStartY) < (intROIEndY - intEndY))
                        intYTolerance = intStartY - intROIStartY;
                    else
                        intYTolerance = intROIEndY - intEndY;
                }
               
                // Select OCV Text
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);

                //Select text center point with width, height 1
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);

                // Get EOCVText from OCV
                m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

                // Set tolerance to OcvText
                objOcvText.ShiftXTolerance = intXTolerance;
                objOcvText.ShiftYTolerance = intYTolerance;

                // Set EOCVText to OCV
                m_Ocv.ScatterTextsParameters(objOcvText, ESelectionFlag.True);
            }
        }
        public void SetTextsSkewTolerance(float fTolerance)
        {
            int intStartX, intStartY, intEndX, intEndY;

            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
            EOCVText objOcvText = new EOCVText();
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                intStartX = intStartY = intEndX = intEndY = 0;

                // Get ocv text start point and end point
                m_Ocv.GetTextPoint(tx, out intStartX, out intStartY, -1, -1);
                m_Ocv.GetTextPoint(tx, out intEndX, out intEndY, 1, 1);
                
                // Select OCV Text
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);

                //Select text center point with width, height 1
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);

                // Get EOCVText from OCV
                m_Ocv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

                // Set tolerance to OcvText
                objOcvText.SkewTolerance = fTolerance;

                // Set EOCVText to OCV
                m_Ocv.ScatterTextsParameters(objOcvText, ESelectionFlag.True);

                // ------- Binary OCV -----------------------

                intStartX = intStartY = intEndX = intEndY = 0;

                // Get ocv text start point and end point
                m_BinaOcv.GetTextPoint(tx, out intStartX, out intStartY, -1, -1);
                m_BinaOcv.GetTextPoint(tx, out intEndX, out intEndY, 1, 1);

                // Select OCV Text
                m_BinaOcv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_BinaOcv.SelectSampleTexts(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);

                //Select text center point with width, height 1
                m_BinaOcv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);

                // Get EOCVText from OCV
                m_BinaOcv.GatherTextsParameters(objOcvText, ESelectionFlag.True);

                // Set tolerance to OcvText
                objOcvText.SkewTolerance = fTolerance;

                // Set EOCVText to OCV
                m_BinaOcv.ScatterTextsParameters(objOcvText, ESelectionFlag.True);

            }
        }

        /// <summary>
        /// Get image roi. 
        /// Image roi mean image size same as attched roi.
        /// </summary>
        /// <param name="objROI">Input roi</param>
        /// <param name="objImage">Image where image roi attach to</param>
        /// <param name="objImageROI">Image roi</param>
        private void GetImageROI(ROI objROI, ref ImageDrawing objImage, ref ROI objImageROI)
        {
            objImage.SetImageSize(objROI.ref_ROIWidth, objROI.ref_ROIHeight);
            EasyImage.Copy(objROI.ref_ROI, objImage.ref_objMainImage);

            objImageROI.LoadROISetting(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);
            objImageROI.AttachImage(objImage);
        }

        public void Dispose()
        {
            if (m_BinaOcv != null)
                m_BinaOcv.Dispose();

            if (m_Ocv != null)
                m_Ocv.Dispose();

            if (m_MinOcv != null)
                m_MinOcv.Dispose();

            if (m_objSampleImage != null)
            {
                m_objSampleImage.Dispose();
                m_objSampleImage = null;
            }
        }

        public int[] GetCharsCenterX()
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            if (intNumTexts == 0) // 2020-11-19 ZJYEOH : intNumTexts will become 0 if user disable all mark
                return new int[0];
            int intNumTextChars = 0;
            int intNumChars = 0;
            int[] arrCharCenterX;
            int intCenterX, intCenterY;
            int intCharIndex = 0;
            EOCVChar objOcvChar = new EOCVChar();

            int intTextStartX = 0;
            int intTextStartY = 0;
            m_Ocv.GetTextPoint(0, out intTextStartX, out intTextStartY, -1, -1);
#if (Debug_2_12 || Release_2_12)
            // Get chars count in all texts
            for (uint tx = 0; tx < intNumTexts; tx++)
                intNumChars += (int)m_Ocv.GetNumTextChars(tx);

            arrCharCenterX = new int[intNumChars];

            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Get chars count in all texts
                for (int tx = 0; tx < intNumTexts; tx++)
                intNumChars += m_Ocv.GetNumTextChars(tx);

            arrCharCenterX = new int[intNumChars];

            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    arrCharCenterX[m_arrCharNo[intCharIndex]] = intCenterX - intTextStartX;
                    intCharIndex++;
                }
            }

            return arrCharCenterX;
        }

        public int[] GetCharsCenterY()
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            if (intNumTexts == 0) // 2020-11-19 ZJYEOH : intNumTexts will become 0 if user disable all mark
                return new int[0];
            int intNumTextChars = 0;
            int intNumChars = 0;
            int[] arrCharCenterY;
            int intCenterX, intCenterY;
            int intCharIndex = 0;
            EOCVChar objOcvChar = new EOCVChar();

            int intTextStartX = 0;
            int intTextStartY = 0;
            m_Ocv.GetTextPoint(0, out intTextStartX, out intTextStartY, -1, -1);
#if (Debug_2_12 || Release_2_12)
            // Get chars count in all texts
            for (uint tx = 0; tx < intNumTexts; tx++)
                intNumChars += (int)m_Ocv.GetNumTextChars(tx);

            arrCharCenterY = new int[intNumChars];

            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Get chars count in all texts
                    for (int tx = 0; tx < intNumTexts; tx++)
                intNumChars += m_Ocv.GetNumTextChars(tx);

            arrCharCenterY = new int[intNumChars];

            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    arrCharCenterY[m_arrCharNo[intCharIndex]] = intCenterY - intTextStartY;
                    intCharIndex++;
                }
            }

            return arrCharCenterY;
        }

        public void ClearCharData()
        {
            m_arrTextNo.Clear();
            m_arrCharNo.Clear();
            m_arrSampleChar.Clear();
            m_arrSampleText.Clear();
        }

        public void SetCharsShiftXY_ForMinOcv(float fXTolerance, float fYTolerance)
        {
            int intNumChars;
            int intCharIndex = 0;
            int intNumTexts = (int)m_MinOcv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
            EOCVChar objOcvChar = new EOCVChar();
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumChars = (int)m_MinOcv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumChars = m_MinOcv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumChars; ch++)
                {
#endif

                    // Select OCV Char
                    m_MinOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_MinOcv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, ESelectionFlag.True);
                    m_MinOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);

                    // Get OcvChar from OCV
                    m_MinOcv.GatherTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                    // Set tolerance to OcvChar
                    objOcvChar.ShiftXTolerance = fXTolerance;
                    objOcvChar.ShiftYTolerance = fYTolerance;

                    // Set OcvChar to OCV
                    m_MinOcv.ScatterTextsCharsParameters(objOcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                    intCharIndex++;
                }
            }
        }
    }
}
