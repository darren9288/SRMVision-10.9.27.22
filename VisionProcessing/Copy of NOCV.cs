using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Euresys.eVision;
using Common;

namespace VisionProcessing
{
    public class NOCV
    {
        #region Constant Variables
        private int m_intImageWidth = 640;
        private int m_intImageHeight = 480;
        #endregion

        #region Member Variables

        private string m_strErrorMessage = "";
        private List<int> m_arrTextNo = new List<int>();
        private List<int> m_arrCharNo = new List<int>();
        private List<Sample> m_arrSampleChar = new List<Sample>();
        private List<Sample> m_arrSampleText = new List<Sample>();
        private Ocv m_BinaOcv = new Ocv();
        private Ocv m_Ocv = new Ocv();
        private Pen m_penGreen = new Pen(Color.FromArgb(0, 255, 0));
        private Pen m_penRed = new Pen(Color.Red);
        private Pen m_penGreenText = new Pen(Color.FromArgb(0, 255, 0), (float)2);

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
        }

        #endregion

        #region Properties

        public string ref_strErrorMessage { get { return m_strErrorMessage; } }

        #endregion

        public NOCV()
        {
        }


        public bool IsDiagnosticsDefine()
        {
            if (m_Ocv.Diagnostics == (int)EasyOcv.Diagnostic.Undefined)
                return false;

            return true;
        }

        public bool IsDiagnosticsTextNoFound()
        {
            return ((m_Ocv.Diagnostics & (int)EasyOcv.Diagnostic.TextNotFound) > 0);
        }

        public bool IsDiagnosticsTextMismatch()
        {
            return ((m_Ocv.Diagnostics & (int)EasyOcv.Diagnostic.TextMismatch) > 0);
        }

        public bool IsDiagnosticsTextOverprint()
        {
            return ((m_Ocv.Diagnostics & (int)EasyOcv.Diagnostic.TextOverprinting) > 0);
        }

        public bool IsDiagnosticsTextUnderprint()
        {
            return ((m_Ocv.Diagnostics & (int)EasyOcv.Diagnostic.TextUnderprinting) > 0);
        }


        public float GetCharScore(int intCharIndex)
        {
            for (int i = 0; i < m_arrSampleChar.Count; i++)
            {
                if (m_arrCharNo[i] == intCharIndex)
                    return m_arrSampleChar[i].fScore;
            }
            return -1;
        }

        public int GetCharNo(int intCharIndex)
        {
            return m_arrCharNo[intCharIndex];
        }

        public int GetNumChars()
        {
            int intNumText = m_Ocv.NumTexts;
            int intNumChars = 0;
            for (int tx = 0; tx < intNumText; tx++)
            {
                intNumChars += m_Ocv.GetNumTextChars(tx);
            }

            return intNumChars;
        }

        public int GetNumTexts()
        {
            return m_Ocv.NumTexts;
        }

        public int HitChar(int intX, int intY)
        {
            for (int i = 0; i < m_arrSampleChar.Count; i++)
            {
                if ((intX > m_arrSampleChar[i].intOrgX) && (intX < m_arrSampleChar[i].intEndX) &&
                    (intY > m_arrSampleChar[i].intOrgY) && (intY < m_arrSampleChar[i].intEndY))
                {
                    return m_arrCharNo[i];
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
                    return m_arrTextNo[i];
                }
            }

            return -1;
        }

        public int[] GetCharsShiftX()
        {
            int intNumTexts = m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intNumChars = 0;
            int[] intCharShiftX;
            int intCenterX, intCenterY;
            int intCharIndex = 0;
            OcvChar objOcvChar = new OcvChar();

            // Get chars count
            for (int tx = 0; tx < intNumTexts; tx++)
                intNumChars += m_Ocv.GetNumTextChars(tx);

            intCharShiftX = new int[intNumChars];

            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, SelectionFlag.Any, SelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);
                    intCharShiftX[intCharIndex] = (int)objOcvChar.ShiftXTolerance;
                    intCharIndex++;
                }
            }

            return intCharShiftX;
        }

        public int[] GetCharsShiftY()
        {
            int intNumTexts = m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intNumChars = 0;
            int[] intCharShiftY;
            int intCenterX, intCenterY;
            int intCharIndex = 0;
            OcvChar objOcvChar = new OcvChar();

            // Get chars count
            for (int tx = 0; tx < intNumTexts; tx++)
                intNumChars += m_Ocv.GetNumTextChars(tx);

            intCharShiftY = new int[intNumChars];

            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, m_intImageWidth, m_intImageHeight, SelectionFlag.Any, SelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);
                    intCharShiftY[intCharIndex] = (int)objOcvChar.ShiftYTolerance;
                    intCharIndex++;
                }
            }

            return intCharShiftY;
        }



        public System.Drawing.Point GetCharEndXY(int intCharIndex)
        {
            for (int i = 0; i < m_arrSampleChar.Count; i++)
            {
                if (m_arrCharNo[i] == intCharIndex)
                    return new System.Drawing.Point(m_arrSampleChar[i].intEndX, m_arrSampleChar[i].intEndY);
            }

            return new System.Drawing.Point(-1, -1);
        }

        public System.Drawing.Point GetCharStartXY(int intCharIndex)
        {
            for (int i = 0; i < m_arrSampleChar.Count; i++)
            {
                if (m_arrCharNo[i] == intCharIndex)
                    return new System.Drawing.Point(m_arrSampleChar[i].intOrgX, m_arrSampleChar[i].intOrgY);
            }

            return new System.Drawing.Point(-1, -1);
        }

        public System.Drawing.Point GetTextEndXY()
        {
            int intNumTexts = m_Ocv.NumTexts;

            if (intNumTexts == 0)
                return new System.Drawing.Point(-1, -1);

            System.Drawing.Point pEndPoint = new System.Drawing.Point(0, 0);
            for (int i = 0; i < m_arrSampleText.Count; i++)
            {
                if (m_arrSampleText[i].intEndX  > pEndPoint.X)
                    pEndPoint.X = m_arrSampleText[i].intEndX;

                if (m_arrSampleText[i].intEndY > pEndPoint.Y)
                    pEndPoint.Y = m_arrSampleText[i].intEndY;
            }

            return pEndPoint;

            //int intNumTexts = m_Ocv.NumTexts;

            //if (intNumTexts == 0)
            //    return new System.Drawing.Point(-1, -1);

            //System.Drawing.Point pEndPoint = new System.Drawing.Point(0, 0);
            //int intX = 0, intY = 0;
            //for (int tx = 0; tx < intNumTexts; tx++)
            //{
            //    m_Ocv.GetTextPoint(tx, ref intX, ref intY, 1, 1);
            //    if (intX > pEndPoint.X)
            //        pEndPoint.X = intX;

            //    if (intY > pEndPoint.Y)
            //        pEndPoint.Y = intY;
            //}

            //return pEndPoint;
        }

        public System.Drawing.Point GetTextEndXY(int intTextIndex)
        {
            if (intTextIndex < m_arrSampleText.Count)
                return new System.Drawing.Point(m_arrSampleText[intTextIndex].intEndX, m_arrSampleText[intTextIndex].intEndY);
            else 
                return new System.Drawing.Point(-1, -1);

            //if (intTextIndex >= m_Ocv.NumTexts)
            //    return new System.Drawing.Point(-1, -1);

            //int intX = 0, intY = 0;
            //m_Ocv.GetTextPoint(intTextIndex, ref intX, ref intY, 1, 1);

            //return new System.Drawing.Point(intX, intY); ;
        }

        public System.Drawing.Point GetTextStartXY()
        {
            int intNumTexts = m_Ocv.NumTexts;

            if (intNumTexts == 0)
                return new System.Drawing.Point(-1, -1);

            System.Drawing.Point pStartPoint = new System.Drawing.Point(0, 0);
            for (int i = 0; i < m_arrSampleText.Count; i++)
            {
                if (m_arrSampleText[i].intOrgX > pStartPoint.X)
                    pStartPoint.X = m_arrSampleText[i].intOrgX;

                if (m_arrSampleText[i].intOrgY > pStartPoint.Y)
                    pStartPoint.Y = m_arrSampleText[i].intOrgY;
            }

            return pStartPoint;

            //int intNumTexts = m_Ocv.NumTexts;

            //if (intNumTexts == 0)
            //    return new System.Drawing.Point(-1, -1);

            //System.Drawing.Point pStartPoint = new System.Drawing.Point(int.MaxValue, int.MaxValue);
            //int intX = 0, intY = 0;
            //for (int tx = 0; tx < intNumTexts; tx++)
            //{
            //    m_Ocv.GetTextPoint(tx, ref intX, ref intY, -1, -1);
            //    if (intX < pStartPoint.X)
            //        pStartPoint.X = intX;

            //    if (intY < pStartPoint.Y)
            //        pStartPoint.Y = intY;
            //}

            //return pStartPoint;
        }

        public System.Drawing.Point GetTextStartXY(int intTextIndex)
        {
            if (intTextIndex < m_arrSampleText.Count)
                return new System.Drawing.Point(m_arrSampleText[intTextIndex].intOrgX, m_arrSampleText[intTextIndex].intOrgY);
            else
                return new System.Drawing.Point(-1, -1);

            //if (intTextIndex >= m_Ocv.NumTexts)
            //    return new System.Drawing.Point(-1, -1);

            //int intX = 0, intY = 0;
            //m_Ocv.GetTextPoint(intTextIndex, ref intX, ref intY, -1, -1);

            //return new System.Drawing.Point(intX, intY);
        }


        /// <summary>
        /// Create Template Characters
        /// </summary>
        /// <param name="intCreationMode">1: Form multi chars, 2: Form single chars, 3: Form auto chars</param>
        /// <returns></returns>
        public void BuildChars(Blobs objBlobs, int intCreationMode)
        {
            // Clear previous objects, chars and Text
            m_Ocv.DeleteTemplateChars(SelectionFlag.Any);
            m_Ocv.DeleteTemplateObjects(SelectionFlag.Any);
            m_Ocv.DeleteTemplateTexts(SelectionFlag.Any);
            m_Ocv.DeleteTemplateChars(SelectionFlag.Any);
            m_Ocv.DeleteTemplateObjects(SelectionFlag.Any);
            m_Ocv.DeleteTemplateTexts(SelectionFlag.Any);

            // Set objects to ocv
            m_Ocv.CreateTemplateObjects(objBlobs.ref_Blob, SelectionFlag.True);

            // Create template chars
            if (intCreationMode == 1)
                m_Ocv.CreateTemplateChars(SelectionFlag.Any, EasyOcv.CharCreationMode.Separate);
            else if (intCreationMode == 2)
                m_Ocv.CreateTemplateChars(SelectionFlag.Any, EasyOcv.CharCreationMode.Group);
            else
                m_Ocv.CreateTemplateChars(SelectionFlag.Any, EasyOcv.CharCreationMode.Overlap);
        }

        public void BuildTexts()
        {
            m_Ocv.CreateTemplateTexts(SelectionFlag.True);
        }

        public void DefineShiftTextTolerance(ROI objROI, ref int intXTolerance, ref int intYTolerance)
        {
            int intNumTexts = m_Ocv.NumTexts;
            int intStartX, intStartY, intEndX, intEndY;
            int intWidth, intHeight;
            int intTextWidth, intTextHeight;
            intTextWidth = intTextHeight = 0;

            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intStartX = intStartY = intEndX = intEndY = 0;
                m_Ocv.GetTextPoint(tx, ref intStartX, ref intStartY, -1, -1);
                m_Ocv.GetTextPoint(tx, ref intEndX, ref intEndY, 1, 1);
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
            m_Ocv.DeleteTemplateChars(SelectionFlag.Any);
        }

        public void DeleteTemplateTexts()
        {
            m_Ocv.DeleteTemplateTexts(SelectionFlag.Any);
        }

        public void DrawTemplateChars(Graphics g, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY)
        {
            m_Ocv.DrawTemplateObjects(g, m_penGreen, SelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTemplateObjects(g, m_penRed, SelectionFlag.False, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTemplateChars(g, m_penGreen, SelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTemplateChars(g, m_penRed, SelectionFlag.False, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
        }

        public void DrawTemplateTexts(Graphics g, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY)
        {
            m_Ocv.DrawTemplateChars(g, m_penGreen, SelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTemplateChars(g, m_penRed, SelectionFlag.False, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTemplateTextsChars(g, m_penGreen, SelectionFlag.True, SelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTemplateTexts(g, m_penGreenText, SelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTemplateTexts(g, m_penRed, SelectionFlag.False, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
        }

        public void FormMultiSelectedChars()
        {
            m_Ocv.DeleteTemplateChars(SelectionFlag.True);
            m_Ocv.CreateTemplateChars(SelectionFlag.True, EasyOcv.CharCreationMode.Separate);
        }

        public void FormSingleSelectedChars()
        {
            m_Ocv.DeleteTemplateChars(SelectionFlag.True);
            m_Ocv.CreateTemplateChars(SelectionFlag.True, EasyOcv.CharCreationMode.Group);
        }

        public void GetCharEndXY(int intCharIndex, ref int intEndX, ref int intEndY)
        {
            for (int i = 0; i < m_arrSampleChar.Count; i++)
            {
                if (m_arrCharNo[i] == intCharIndex)
                {
                    intEndX = m_arrSampleChar[i].intEndX;
                    intEndY = m_arrSampleChar[i].intEndY;
                    return;
                }
            }
            intEndX = intEndY = -1;
        }

        public void GetCharStartXY(int intCharIndex, ref int intStartX, ref int intStartY)
        {
            for (int i = 0; i < m_arrSampleChar.Count; i++)
            {
                if (m_arrCharNo[i] == intCharIndex)
                {
                    intStartX = m_arrSampleChar[i].intOrgX;
                    intStartY = m_arrSampleChar[i].intOrgY;
                    return;
                }
            }
            intStartX = intStartY = -1;
        }

        public void GetCharSize(int intCharIndex, ref int intWidth, ref int intHeight)
        {
            for (int i = 0; i < m_arrSampleChar.Count; i++)
            {
                if (m_arrCharNo[i] == intCharIndex)
                {
                    intWidth = m_arrSampleChar[i].intWidth;
                    intHeight = m_arrSampleChar[i].intHeight;
                    return;
                }
            }
            intWidth = intHeight = 0;
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

        public void Inspect(ROI objROI, int intThreshold)
        {
            m_Ocv.Inspect(objROI.ref_ROI, intThreshold);
            m_BinaOcv.Inspect(objROI.ref_ROI, intThreshold);

            m_arrSampleChar.Clear();
            m_arrSampleText.Clear();
            int intNumTextChars;
            int intNumTexts = m_Ocv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
            float fScoreValue = -1;
            Sample objSample;
            OcvChar objOcvChar = new OcvChar();
            OcvText objOcvText = new OcvText();

            for (int tx = 0; tx < intNumTexts; tx++)
            {
                m_Ocv.GetTextPoint(tx, ref intCenterX, ref intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, 640, 480, SelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(objOcvText, SelectionFlag.True);

                objSample = new Sample();
                if (objOcvText.TemplateBackgroundArea != -1)
                {
                    if (objOcvText.Correlation <= 0)
                        fScoreValue = 0;
                    else
                        fScoreValue = objOcvText.Correlation * 100;

                    m_Ocv.GetTextPoint(tx, ref objSample.intOrgX, ref objSample.intOrgY, -1, -1);
                    m_Ocv.GetTextPoint(tx, ref objSample.intEndX, ref objSample.intEndY, 1, 1);
                    objSample.fScore = fScoreValue;
                    objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                    objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                    m_arrSampleText.Add(objSample);

                }
                else
                {
                    objSample.fScore = 0; 
                    m_arrSampleText.Add(objSample);
                }

                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
                    m_Ocv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, SelectionFlag.Any, SelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);

                    if (objOcvChar.TemplateBackgroundArea == -1)
                    {
                        m_Ocv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, -1, -1);
                        m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, SelectionFlag.Any, SelectionFlag.True);
                        m_Ocv.SelectSampleTextsChars(intCenterX + 1, intCenterY + 1, 1, 1);
                        m_Ocv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);
                    }
                    if (objOcvChar.TemplateBackgroundArea == -1)
                    {
                        m_Ocv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, 1, 1);
                        m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, SelectionFlag.Any, SelectionFlag.True);
                        m_Ocv.SelectSampleTextsChars(intCenterX - 1, intCenterY - 1, 1, 1);
                        m_Ocv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);
                    }
                    if (objOcvChar.TemplateBackgroundArea == -1)
                    {
                        m_Ocv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, -1, 1);
                        m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, SelectionFlag.Any, SelectionFlag.True);
                        m_Ocv.SelectSampleTextsChars(intCenterX + 1, intCenterY - 1, 1, 1);
                        m_Ocv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);
                    }
                    if (objOcvChar.TemplateBackgroundArea == -1)
                    {
                        m_Ocv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, 1, -1);
                        m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, SelectionFlag.Any, SelectionFlag.True);
                        m_Ocv.SelectSampleTextsChars(intCenterX - 1, intCenterY + 1, 1, 1);
                        m_Ocv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);
                    }

                    if (objOcvChar.TemplateBackgroundArea != -1)
                    {
                        if (objOcvChar.Correlation <= 0)
                            fScoreValue = 0;
                        else
                            fScoreValue = objOcvChar.Correlation * 100;
                    }

                    m_BinaOcv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, 0, 0);
                    m_BinaOcv.SelectSampleTextsChars(0, 0, 640, 480, SelectionFlag.Any, SelectionFlag.True);
                    m_BinaOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_BinaOcv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);

                    if (objOcvChar.TemplateBackgroundArea == -1)
                    {
                        m_BinaOcv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, -1, -1);
                        m_BinaOcv.SelectSampleTextsChars(0, 0, 640, 480, SelectionFlag.Any, SelectionFlag.True);
                        m_BinaOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY + 1, 1, 1);
                        m_BinaOcv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);
                    }
                    if (objOcvChar.TemplateBackgroundArea == -1)
                    {
                        m_BinaOcv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, 1, 1);
                        m_BinaOcv.SelectSampleTextsChars(0, 0, 640, 480, SelectionFlag.Any, SelectionFlag.True);
                        m_BinaOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY - 1, 1, 1);
                        m_BinaOcv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);
                    }
                    if (objOcvChar.TemplateBackgroundArea == -1)
                    {
                        m_BinaOcv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, -1, 1);
                        m_BinaOcv.SelectSampleTextsChars(0, 0, 640, 480, SelectionFlag.Any, SelectionFlag.True);
                        m_BinaOcv.SelectSampleTextsChars(intCenterX + 1, intCenterY - 1, 1, 1);
                        m_BinaOcv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);
                    }
                    if (objOcvChar.TemplateBackgroundArea == -1)
                    {
                        m_BinaOcv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, 1, -1);
                        m_BinaOcv.SelectSampleTextsChars(0, 0, 640, 480, SelectionFlag.Any, SelectionFlag.True);
                        m_BinaOcv.SelectSampleTextsChars(intCenterX - 1, intCenterY + 1, 1, 1);
                        m_BinaOcv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);
                    }

                    objSample = new Sample();
                    if (objOcvChar.TemplateBackgroundArea != -1)
                    {
                        float fBinaScoreValue;
                        if (objOcvChar.Correlation <= 0)
                            fBinaScoreValue = 0;
                        else
                            fBinaScoreValue = objOcvChar.Correlation * 100;

                        
                        if (fBinaScoreValue <= fScoreValue)
                        {
                            m_Ocv.GetTextCharPoint(tx, ch, ref objSample.intOrgX, ref objSample.intOrgY, -1, -1);
                            m_Ocv.GetTextCharPoint(tx, ch, ref objSample.intEndX, ref objSample.intEndY, 1, 1);
                            objSample.fScore = fScoreValue;
                        }
                        else
                        {
                            m_BinaOcv.GetTextCharPoint(tx, ch, ref objSample.intOrgX, ref objSample.intOrgY, -1, -1);
                            m_BinaOcv.GetTextCharPoint(tx, ch, ref objSample.intEndX, ref objSample.intEndY, 1, 1);
                            objSample.fScore = fBinaScoreValue;
                        }
                        objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                        objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                        m_arrSampleChar.Add(objSample);
                    }
                    else if (fScoreValue >= 0)
                    {
                        m_Ocv.GetTextCharPoint(tx, ch, ref objSample.intOrgX, ref objSample.intOrgY, -1, -1);
                        m_Ocv.GetTextCharPoint(tx, ch, ref objSample.intEndX, ref objSample.intEndY, 1, 1);
                        objSample.fScore = fScoreValue;
                        objSample.intWidth = objSample.intEndX - objSample.intOrgX;
                        objSample.intHeight = objSample.intEndY - objSample.intOrgY;
                        m_arrSampleChar.Add(objSample);
                    }
                    else
                    {
                        objSample.fScore = 0;
                        m_arrSampleChar.Add(objSample);
                    }
                }
            }
        }

        public void Learn(ROI objROI)
        {
            m_Ocv.Learn(objROI.ref_ROI, SelectionFlag.True);
            m_BinaOcv = m_Ocv;
        }

        public void LoadOCVFile(String strPath)
        {
            m_Ocv.Load(strPath);
            m_Ocv.LocationMode = EasyOcv.LocationMode.Gradient;

            m_BinaOcv.Load(strPath);
            m_BinaOcv.LocationMode = EasyOcv.LocationMode.Binarized;
        }

        public void ResetPreviousSelectedChars()
        {
            m_Ocv.SelectTemplateObjects(0, 0, 640, 480, SelectionFlag.True);
            m_Ocv.SelectTemplateChars(0, 0, 640, 480, SelectionFlag.True);
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

            m_Ocv.SelectTemplateObjects(intOrgX, intOrgY, intWidth, intHeight, SelectionFlag.Any);
            m_Ocv.SelectTemplateChars(intOrgX, intOrgY, intWidth, intHeight, SelectionFlag.Any);
        }

        public void SortTemplateCharsByCenterPoint()
        {
            int intNumChars;
            int intNumTexts = m_Ocv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
            m_arrCharNo.Clear();
            List<PointF> arrCenterPoints = new List<PointF>();

            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumChars; ch++)
                {
                    m_Ocv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, 0, 0);

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

        public void SetCharsShiftXY(int[] arrXTolerance, int[] arrYTolerance)
        {
            int intNumChars;
            int intCharIndex = 0;
            int intNumTexts = m_Ocv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
            OcvChar objOcvChar = new OcvChar();
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumChars; ch++)
                {
                    // Select Ocv Char
                    m_Ocv.GetTextCharPoint(tx, ch, ref intCenterX, ref intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, SelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);

                    // Get OcvChar from Ocv
                    m_Ocv.GatherTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);

                    // Set tolerance to OcvChar
                    if (intCharIndex < arrXTolerance.Length)
                        objOcvChar.ShiftXTolerance = arrXTolerance[intCharIndex];
                    else
                        objOcvChar.ShiftXTolerance = arrXTolerance[arrXTolerance.Length - 1];   // Use the last tolerance if no enough

                    if (intCharIndex < arrYTolerance.Length)
                        objOcvChar.ShiftYTolerance = arrYTolerance[intCharIndex];
                    else
                        objOcvChar.ShiftYTolerance = arrYTolerance[arrYTolerance.Length - 1];   // Use the last tolerance if no enough

                    // Set OcvChar to Ocv
                    m_Ocv.ScatterTextsCharsParameters(objOcvChar, SelectionFlag.Any, SelectionFlag.True);

                    intCharIndex++;
                }
            }
        }

        public void SetTemplateImage(ROI objROI, int intOriX, int intOriY, int intWidth, int intHeight)
        {
            m_Ocv.TemplateImage = new ROIBW8(objROI.ref_ROI, intOriX, intOriY, intWidth, intHeight);
        }

        public void SetTextsShiftXY(int intXTolerance, int intYTolerance)
        {
            int intNumTexts = m_Ocv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
            OcvText objOcvText = new OcvText();
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                // Select Ocv Text
                m_Ocv.GetTextPoint(tx, ref intCenterX, ref intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, 640, 480, SelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);

                // Get OcvText from Ocv
                m_Ocv.GatherTextsParameters(objOcvText, SelectionFlag.True);

                // Set tolerance to OcvText
                objOcvText.ShiftXTolerance = intXTolerance;
                objOcvText.ShiftYTolerance = intYTolerance;

                // Set OcvText to Ocv
                m_Ocv.ScatterTextsParameters(objOcvText, SelectionFlag.True);
            }
        }
    }
}
