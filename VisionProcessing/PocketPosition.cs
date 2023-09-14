using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;

namespace VisionProcessing
{
    public class PocketPosition
    {
        #region Member Variables

        private EMatcher m_objMatcher = new EMatcher();
        private TrackLog m_objTrackLog = new TrackLog();
        private bool m_blnDrawSamplingPoint_Plate = true;
        private bool m_blnDrawDraggingBox_Plate = true;
        private bool m_blnDrawSamplingPoint_Pocket = true;
        private bool m_blnDrawDraggingBox_Pocket = true;
        // result
        private bool m_blnGaugeResult = false;
        private PointF m_pObjectCenter = new PointF(0, 0);
        private float m_fObjectOffsetX = 0, m_fObjectOffsetY = 0;
        private float m_fObjectAngle = 0;
        private float m_fTemplateXDistance = 0;
        private float m_fResultXDistance = 0;
        private float m_fResultYDistance = 0;
        private float m_fMatcherCenterX = 0;

        private List<int> m_arrGrabImageIndex = new List<int>(); // 0: Pocket Pattern & Pocket Gauge, 1: Plate Gauge
        private float m_fPositionXTolerance = 0;
        private float m_fPositionYTolerance = 0;
        private float m_fPatternGaugeOffsetX = 0;
        private float m_fPatternGaugeOffsetY = 0;
        private int m_intPocketPositionReference = 400;
        private int m_intPocketPositionTolerance = 50;
        private int m_intMinMatchingScore = 70;
        private string m_strErrorMessage = "";
        private int m_intFailResultMask = 0;

        #endregion

        #region Properties
        public bool ref_blnGaugeResult { get { return m_blnGaugeResult; } set { m_blnGaugeResult = value; } }
        public bool ref_blnDrawSamplingPoint_Plate { get { return m_blnDrawSamplingPoint_Plate; } set { m_blnDrawSamplingPoint_Plate = value; } }
        public bool ref_blnDrawDraggingBox_Plate { get { return m_blnDrawDraggingBox_Plate; } set { m_blnDrawDraggingBox_Plate = value; } }
        public bool ref_blnDrawSamplingPoint_Pocket { get { return m_blnDrawSamplingPoint_Pocket; } set { m_blnDrawSamplingPoint_Pocket = value; } }
        public bool ref_blnDrawDraggingBox_Pocket { get { return m_blnDrawDraggingBox_Pocket; } set { m_blnDrawDraggingBox_Pocket = value; } }
        public int ref_intMinMatchingScore { get { return m_intMinMatchingScore; } set { m_intMinMatchingScore = value; } }
        public int ref_intPocketPositionReference { get { return m_intPocketPositionReference; } set { m_intPocketPositionReference = value; } }
        public int ref_intPocketPositionTolerance { get { return m_intPocketPositionTolerance; } set { m_intPocketPositionTolerance = value; } }
        public float ref_fPositionXTolerance { get { return m_fPositionXTolerance; } set { m_fPositionXTolerance = value; } }
        public float ref_fPositionYTolerance { get { return m_fPositionYTolerance; } set { m_fPositionYTolerance = value; } }
        public float ref_fPatternGaugeOffsetX { get { return m_fPatternGaugeOffsetX; } set { m_fPatternGaugeOffsetX = value; } }
        public float ref_fPatternGaugeOffsetY { get { return m_fPatternGaugeOffsetY; } set { m_fPatternGaugeOffsetY = value; } }
        public float ref_fTemplateXDistance { get { return m_fTemplateXDistance; } set { m_fTemplateXDistance = value; } }
        public float ref_fResultXDistance { get { return m_fResultXDistance; } set { m_fResultXDistance = value; } }
        public float ref_fResultYDistance { get { return m_fResultYDistance; } set { m_fResultYDistance = value; } }
        // result
        public PointF ref_fObjectCenter { get { return m_pObjectCenter; } }

        public string ref_strErrorMessage { get { return m_strErrorMessage; } }
        public int ref_intFailResultMask { get { return m_intFailResultMask; } }
        #endregion

        public PocketPosition()
        {
          
        }

        public void ResetData()
        {
            m_strErrorMessage = "";
            m_fResultXDistance = 0;
            m_fResultYDistance = 0;
        }

        public bool IsPatternLearnt()
        {
            return m_objMatcher.PatternLearnt;
        }

        public bool LearnPattern(ROI objROI)
        {
            try
            {
                //m_objMatcher.LearnPattern(objROI.ref_ROI);
#if (Debug_2_12 || Release_2_12)
                m_objMatcher.AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif

                m_objMatcher.LearnPattern(objROI.ref_ROI);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Positioning Learn Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Positioning Learn Pattern: " + ex.ToString());
                return false;
            }
            return true;
        }

        public bool LoadPattern(string strFilePath)
        {
            try
            {
                m_objMatcher.Load(strFilePath);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Pocket Position Load Pattern Error: " + ex.ToString() + " FilePath:" + strFilePath;
                m_objTrackLog.WriteLine("Pocket Position Load Pattern: " + ex.ToString() + " FilePath:" + strFilePath);
                return false;
            }
            return true;
        }

        public bool SavePattern(string strFilePath)
        {
            try
            {
                string strDirectoryName = System.IO.Path.GetDirectoryName(strFilePath);
                DirectoryInfo directory = new DirectoryInfo(strDirectoryName);
                if (!directory.Exists)
                    CreateUnexistDirectory(directory);

                m_objMatcher.Save(strFilePath);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Pocket Position Save Pattern Error: " + ex.ToString();
                m_objTrackLog.WriteLine("Pocket Position Save Pattern: " + ex.ToString());
                return false;
            }
            return true;
        }

        public void SetTemplateResult()
        {
            if (m_blnGaugeResult)
                m_fTemplateXDistance = m_fResultXDistance;
        }

        public void LoadPocketPosition(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);

            // Load Pocket Position Setting
            m_fTemplateXDistance = objFile.GetValueAsFloat("TemplateXDistance", 0);
            m_fPositionXTolerance = objFile.GetValueAsFloat("PositionXTolerance", 0);   
            m_fPositionYTolerance = objFile.GetValueAsFloat("PositionYTolerance", 0);
            m_fPatternGaugeOffsetX = objFile.GetValueAsFloat("PatternGaugeOffsetX", 0);
            m_fPatternGaugeOffsetY = objFile.GetValueAsFloat("PatternGaugeOffsetY", 0);
            m_intMinMatchingScore = objFile.GetValueAsInt("MinMatchingScore", 0);
            m_intPocketPositionReference = objFile.GetValueAsInt("PocketPositionReference", 400);
            m_intPocketPositionTolerance = objFile.GetValueAsInt("PocketPositionTolerance", 50);

            // Grab image index
            int intGrabImageIndexCount = objFile.GetValueAsInt("GrabImageIndexCount", 0);
            m_arrGrabImageIndex.Clear();
            for (int j = 0; j < intGrabImageIndexCount; j++)
                m_arrGrabImageIndex.Add(objFile.GetValueAsInt("GrabImageIndex" + j.ToString(), 0));
        }

        public void SavePocketPosition(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);

            // Rectangle gauge template measurement result
            objFile.WriteElement1Value("TemplateXDistance", m_fTemplateXDistance);
            objFile.WriteElement1Value("PositionXTolerance", m_fPositionXTolerance);
            objFile.WriteElement1Value("PositionYTolerance", m_fPositionYTolerance);
            objFile.WriteElement1Value("PatternGaugeOffsetX", m_fPatternGaugeOffsetX);
            objFile.WriteElement1Value("PatternGaugeOffsetY", m_fPatternGaugeOffsetY);
            objFile.WriteElement1Value("MinMatchingScore", m_intMinMatchingScore);
            objFile.WriteElement1Value("PocketPositionReference", m_intPocketPositionReference);
            objFile.WriteElement1Value("PocketPositionTolerance", m_intPocketPositionTolerance);

            // Grab image index
            objFile.WriteElement1Value("GrabImageIndexCount", m_arrGrabImageIndex.Count);
            for (int j = 0; j < m_arrGrabImageIndex.Count; j++)
                objFile.WriteElement1Value("GrabImageIndex" + j.ToString(), m_arrGrabImageIndex[j]);

            objFile.WriteEndElement();

     
        }

        public void ResetPositionData()
        {
            m_fObjectOffsetX = 0;
            m_fObjectOffsetY = 0;
            m_fObjectAngle = 0;
            m_intMinMatchingScore = 0;
             m_pObjectCenter = new PointF(0, 0);
           
            m_fObjectAngle = 0;
            m_strErrorMessage = "";
            m_intFailResultMask = 0;
            
        }

        private void CreateUnexistDirectory(DirectoryInfo directory)
        {
            if (!directory.Parent.Exists)
            {
                CreateUnexistDirectory(directory.Parent);
            }

            Directory.CreateDirectory(directory.FullName);

        }
        public bool MeasurePlate(ImageDrawing objImage, List<LGauge> arrLineGauges, bool blnWantUsePattern, bool blnWantUseGauge)
        {
            bool blnResult = false;


            arrLineGauges[1].Measure(objImage);

            float fPassLength = arrLineGauges[1].ref_ObjectScore / 100 * arrLineGauges[1].ref_GaugeLength;
            float fMeasuredLength = arrLineGauges[1].ref_GaugeLength;

            float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

            if (fFinalMeasureScore < 40)
            {
                m_blnGaugeResult = false;
                blnResult = false;
            }
            else
            {
                m_blnGaugeResult = true;
                blnResult = true;
            }

            if (!blnResult)
            {
                m_strErrorMessage += "*Pocket Position : Plate Gauge Measurement Fail!";
            }
            //else
            //{
            //    if (blnWantUsePattern && !blnWantUseGauge)
            //        m_fResultXDistance = arrLineGauges[1].ref_ObjectCenterX - m_fMatcherCenterX;

            //    else //if (!blnWantUsePattern && blnWantUseGauge)
            //        m_fResultXDistance = arrLineGauges[1].ref_ObjectCenterX - arrLineGauges[0].ref_ObjectCenterX;

            //}
            return blnResult;
        }
        public bool MeasurePlatePosition(ImageDrawing objImage, List<LGauge> arrLineGauges, bool blnWantUsePattern, bool blnWantUseGauge)
        {
            bool blnResult = false;

           
            arrLineGauges[1].Measure(objImage);

            float fPassLength = arrLineGauges[1].ref_ObjectScore / 100 * arrLineGauges[1].ref_GaugeLength;
            float fMeasuredLength = arrLineGauges[1].ref_GaugeLength;

            float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

            if (fFinalMeasureScore < 40)
            {
                m_blnGaugeResult = false;
                blnResult = false;
            }
            else
            {
                m_blnGaugeResult = true;
                blnResult = true;
            }

            if (!blnResult)
            {
                m_strErrorMessage += "*Pocket Position : Plate Gauge Measurement Fail!";
            }
            else
            {
                if (blnWantUsePattern && !blnWantUseGauge)
                    m_fResultXDistance = arrLineGauges[1].ref_ObjectCenterX - m_fMatcherCenterX;

                else //if (!blnWantUsePattern && blnWantUseGauge)
                    m_fResultXDistance = arrLineGauges[1].ref_ObjectCenterX - arrLineGauges[0].ref_ObjectCenterX;

            }
            return blnResult;
        }
        public bool MeasurePocketPosition(ImageDrawing objImage, List<LGauge> arrLineGauges)
        {
            bool blnResult = false;


            arrLineGauges[0].Measure(objImage);

            float fPassLength = arrLineGauges[0].ref_ObjectScore / 100 * arrLineGauges[0].ref_GaugeLength;
            float fMeasuredLength = arrLineGauges[0].ref_GaugeLength;

            float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

            if (fFinalMeasureScore < 40)
            {
                m_blnGaugeResult = false;
                blnResult = false;
            }
            else
            {
                m_blnGaugeResult = true;
                blnResult = true;
            }

            if (!blnResult)
            {
                m_strErrorMessage += "*Pocket Position : Pocket Gauge Measurement Fail!";
            }
            else
            {
                //m_fResultXTolerance = arrLineGauges[0].ref_ObjectCenterX - m_fMatcherCenterX;


            }
            return blnResult;
        }
        public bool MatchPocketPattern(ROI objROI)
        {
            try
            {

                if (m_objMatcher.PatternLearnt)
                {
                    if (m_objMatcher.MinAngle != 0)
                        m_objMatcher.MinAngle = 0;
                    if (m_objMatcher.MaxAngle != 0)
                        m_objMatcher.MaxAngle = 0;
                    if (m_objMatcher.FinalReduction != 2)
                        m_objMatcher.FinalReduction = 2;
                    if (m_objMatcher.MaxPositions < 2)
                        m_objMatcher.MaxPositions = 2;  // 2020 03 10 - Set the MaxPositions to 2 will stabilize the pattern matching result.

                    m_objMatcher.Match(objROI.ref_ROI);
                    if (m_objMatcher.NumPositions > 0)
                    {
                        if ((m_objMatcher.GetPosition(0).Score * 100) > m_intMinMatchingScore)
                        {
                            m_fMatcherCenterX = objROI.ref_ROITotalX + m_objMatcher.GetPosition(0).CenterX;
                            ref_blnGaugeResult = true;
                            return true;
                        }
                        else
                        {
                            ref_blnGaugeResult = false;
                            m_strErrorMessage = "*Pocket Position : Pocket Matching Score Fail. Set = " + m_intMinMatchingScore.ToString() + ", Result = " + Math.Max(0, m_objMatcher.GetPosition(0).Score) * 100;
                            return false;
                        }
                    }
                    else
                    {
                        ref_blnGaugeResult = false;
                        m_strErrorMessage = "*Pocket Position : No Pocket Pattern Found!";
                        return false;
                    }
                }
                else
                {
                    ref_blnGaugeResult = false;
                    m_strErrorMessage = "*Pocket Position : No Pocket Pattern Learned!";
                    return false;
                }
            }
            catch (Exception ex)
            {
                //STTrackLog.WriteLine("OriX=" + objROI.ref_ROI.OrgX +
                //                     ", OriY=" + objROI.ref_ROI.OrgY +
                //                     ", Width=" + objROI.ref_ROI.Width +
                //                     ", Height=" + objROI.ref_ROI.Height);

                //objROI.SaveImage("D:\\objROI.bmp");
                SRMMessageBox.Show("Vision4Process : Pocket Position() -> Exception: " + ex.ToString());
                return false;
            }

            return false;
        }
        public float GetMatchingScore()
        {
            if (m_objMatcher.NumPositions > 0)
                return m_objMatcher.GetPosition(0).Score * 100;

            return 0;
        }
        public float GetMatchingCenterX()
        {
            if (m_objMatcher.NumPositions > 0)
                return m_objMatcher.GetPosition(0).CenterX;

            return 0;
        }
        public float GetMatchingCenterY()
        {
            if (m_objMatcher.NumPositions > 0)
                return m_objMatcher.GetPosition(0).CenterY;

            return 0;
        }
        public void DrawGaugeMeasurePosition(Graphics g, List<LGauge> arrLineGauges, int intGaugeIndex)
        {
            //for (int v = 0; v < arrLineGauges.Count; v++)
            if (intGaugeIndex < arrLineGauges.Count)
            {
                switch (intGaugeIndex)
                {
                    case 0:
                        if (m_blnDrawSamplingPoint_Pocket)
                        {
                            arrLineGauges[intGaugeIndex].DrawValidSamplingPointGauge(g);
                            arrLineGauges[intGaugeIndex].DrawInValidSamplingPointGauge(g);
                        }
                        if (m_blnDrawDraggingBox_Pocket)
                            arrLineGauges[intGaugeIndex].DrawDraggingBoxGauge(g);
                        break;
                    case 1:
                        if (m_blnDrawSamplingPoint_Plate)
                        {
                            arrLineGauges[intGaugeIndex].DrawValidSamplingPointGauge(g);
                            arrLineGauges[intGaugeIndex].DrawInValidSamplingPointGauge(g);
                        }
                        if (m_blnDrawDraggingBox_Plate)
                            arrLineGauges[intGaugeIndex].DrawDraggingBoxGauge(g);
                        break;
                }
                if (m_blnGaugeResult)
                    arrLineGauges[intGaugeIndex].DrawResultLineGauge(g, Color.Yellow);
                else
                    arrLineGauges[intGaugeIndex].DrawResultLineGauge(g, Color.Red);

            }
            
        }

        public void MeasureGauge(ImageDrawing objImage, List<LGauge> arrLineGauges, int intGaugeIndex)
        {
            bool blnResult = false;
            
            arrLineGauges[intGaugeIndex].Measure(objImage);

            float fPassLength = arrLineGauges[intGaugeIndex].ref_ObjectScore / 100 * arrLineGauges[intGaugeIndex].ref_GaugeLength;
            float fMeasuredLength = arrLineGauges[intGaugeIndex].ref_GaugeLength;

            float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

            if (fFinalMeasureScore < 40)
            {
                m_blnGaugeResult = false;
                blnResult = false;
            }
            else
            {
                m_blnGaugeResult = true;
                blnResult = true;
            }
            if (!blnResult)
            {
                m_strErrorMessage = "Gauge Measurement Fail!";
                m_pObjectCenter = new PointF(0, 0);
            }
            else
            {
                m_strErrorMessage = "";
                m_pObjectCenter = new PointF(arrLineGauges[intGaugeIndex].ref_ObjectCenterX, arrLineGauges[intGaugeIndex].ref_ObjectCenterY);
            }
            
        }

        public void SetPatternGaugeOffset(ROI objGaugeROI, ROI objPattern)
        {
            m_fPatternGaugeOffsetX = objGaugeROI.ref_ROITotalCenterX - objPattern.ref_ROITotalCenterX;
            m_fPatternGaugeOffsetY = objGaugeROI.ref_ROITotalCenterY - objPattern.ref_ROITotalCenterY;
        }
        /// <summary>
        /// Set grab image index
        /// </summary>
        /// <param name="intArrayIndex"> 0: Pocket Pattern, 1: Pocket Gauge, 2: Plate Gauge </param>
        /// <param name="intGrabImageIndex">Select which image view no use to inspection</param>
        public void SetGrabImageIndex(int intArrayIndex, int intGrabImageIndex)
        {
            if (intArrayIndex >= m_arrGrabImageIndex.Count)
            {
                for (int i = m_arrGrabImageIndex.Count - 1; i <= intArrayIndex; i++)
                {
                    m_arrGrabImageIndex.Add(0);
                }
            }

            m_arrGrabImageIndex[intArrayIndex] = intGrabImageIndex;
        }

        /// <summary>
        /// Get grab image index 
        /// </summary>
        /// <param name="intArrayIndex"> 0: Pocket Pattern, 1: Pocket Gauge, 2: Plate Gauge </param>
        /// <returns></returns>
        public int GetGrabImageIndex(int intArrayIndex)
        {
            if (intArrayIndex >= m_arrGrabImageIndex.Count)
                return 0;

            return m_arrGrabImageIndex[intArrayIndex];
        }

        public int GetGrabImageIndexCount()
        {
            return m_arrGrabImageIndex.Count;
        }

        public void DrawPocketPositionResult(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnWantUsePocketPattern, bool blnWantUsePocketGauge, List<ROI> arrPocketPositionROIs, List<LGauge> arrPocketPositionGauges, Color[][] arrColor)
        {
            if (blnWantUsePocketPattern && !blnWantUsePocketGauge)
            {
                if (m_objMatcher.NumPositions > 0)
                {
                    g.DrawRectangle(new Pen(arrColor[1][0]), (arrPocketPositionROIs[0].ref_ROITotalX + GetMatchingCenterX() - m_objMatcher.PatternWidth / 2) * fDrawingScaleX,
                        (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() - m_objMatcher.PatternHeight / 2) * fDrawingScaleY,
                        m_objMatcher.PatternWidth * fDrawingScaleX, m_objMatcher.PatternHeight *fDrawingScaleY);

                    if (m_blnGaugeResult)
                    {
                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[1].ref_ObjectCenterX) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY()) * fDrawingScaleY,
                            (arrPocketPositionROIs[0].ref_ROITotalX + GetMatchingCenterX()) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY()) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[1].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() - 3) * fDrawingScaleY,
                             (arrPocketPositionGauges[1].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() + 3) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[1].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() - 3) * fDrawingScaleY,
                           (arrPocketPositionGauges[1].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() + 3) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionROIs[0].ref_ROITotalX + GetMatchingCenterX() - 3) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() - 3) * fDrawingScaleY,
                           (arrPocketPositionROIs[0].ref_ROITotalX + GetMatchingCenterX() + 3) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() + 3) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionROIs[0].ref_ROITotalX + GetMatchingCenterX() + 3) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() - 3) * fDrawingScaleY,
                           (arrPocketPositionROIs[0].ref_ROITotalX + GetMatchingCenterX() - 3) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() + 3) * fDrawingScaleY);
                    }
                    else
                    {
                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY()) * fDrawingScaleY,
                       (arrPocketPositionROIs[0].ref_ROITotalX + GetMatchingCenterX()) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY()) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX - 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY - 3 + GetMatchingCenterY()) * fDrawingScaleY,
                       (arrPocketPositionROIs[3].ref_ROITotalX + 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() + 3) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX + 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY - 3 + GetMatchingCenterY()) * fDrawingScaleY,
                      (arrPocketPositionROIs[3].ref_ROITotalX - 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() + 3) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[0].ref_ROITotalX + GetMatchingCenterX() - 3) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() - 3) * fDrawingScaleY,
                      (arrPocketPositionROIs[0].ref_ROITotalX + GetMatchingCenterX() + 3) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() + 3) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[0].ref_ROITotalX + GetMatchingCenterX() + 3) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() - 3) * fDrawingScaleY,
                      (arrPocketPositionROIs[0].ref_ROITotalX + GetMatchingCenterX() - 3) * fDrawingScaleX, (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() + 3) * fDrawingScaleY);
                    }
                }
                else
                {
                    g.DrawRectangle(new Pen(Color.Red), arrPocketPositionROIs[1].ref_ROITotalX * fDrawingScaleX, arrPocketPositionROIs[1].ref_ROITotalY * fDrawingScaleY, 
                        arrPocketPositionROIs[1].ref_ROIWidth * fDrawingScaleX, arrPocketPositionROIs[1].ref_ROIHeight * fDrawingScaleY);

                    if (m_blnGaugeResult)
                    {
                        g.DrawLine(new Pen(Color.Lime), arrPocketPositionGauges[1].ref_ObjectCenterX * fDrawingScaleX, (arrPocketPositionROIs[1].ref_ROITotalY + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY,
                        (arrPocketPositionROIs[1].ref_ROITotalX + arrPocketPositionROIs[1].ref_ROIWidth / 2) * fDrawingScaleX, (arrPocketPositionROIs[1].ref_ROITotalY + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[1].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionROIs[1].ref_ROITotalY - 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY,
                        (arrPocketPositionGauges[1].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionROIs[1].ref_ROITotalY + 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[1].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionROIs[1].ref_ROITotalY - 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY,
                      (arrPocketPositionGauges[1].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionROIs[1].ref_ROITotalY + 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionROIs[1].ref_ROITotalX - 3 + arrPocketPositionROIs[1].ref_ROIWidth / 2) * fDrawingScaleX, (arrPocketPositionROIs[1].ref_ROITotalY - 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY,
                      (arrPocketPositionROIs[1].ref_ROITotalX + 3 + arrPocketPositionROIs[1].ref_ROIWidth / 2) * fDrawingScaleX, (arrPocketPositionROIs[1].ref_ROITotalY + 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionROIs[1].ref_ROITotalX + 3 + arrPocketPositionROIs[1].ref_ROIWidth / 2) * fDrawingScaleX, (arrPocketPositionROIs[1].ref_ROITotalY - 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY,
                      (arrPocketPositionROIs[1].ref_ROITotalX - 3 + arrPocketPositionROIs[1].ref_ROIWidth / 2) * fDrawingScaleX, (arrPocketPositionROIs[1].ref_ROITotalY + 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY);
                    }
                    else
                    {
                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
                            (arrPocketPositionROIs[1].ref_ROITotalY + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY,
                        (arrPocketPositionROIs[1].ref_ROITotalX + arrPocketPositionROIs[1].ref_ROIWidth / 2) * fDrawingScaleX,
                        (arrPocketPositionROIs[1].ref_ROITotalY + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX - 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
                            (arrPocketPositionROIs[1].ref_ROITotalY - 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY,
                       (arrPocketPositionROIs[3].ref_ROITotalX + 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
                        (arrPocketPositionROIs[1].ref_ROITotalY + 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX + 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
                          (arrPocketPositionROIs[1].ref_ROITotalY - 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY,
                     (arrPocketPositionROIs[3].ref_ROITotalX - 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
                      (arrPocketPositionROIs[1].ref_ROITotalY + 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[1].ref_ROITotalX - 3 + arrPocketPositionROIs[1].ref_ROIWidth / 2) * fDrawingScaleX,
                          (arrPocketPositionROIs[1].ref_ROITotalY - 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY,
                      (arrPocketPositionROIs[1].ref_ROITotalX + 3 + arrPocketPositionROIs[1].ref_ROIWidth / 2) * fDrawingScaleX,
                      (arrPocketPositionROIs[1].ref_ROITotalY + 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[1].ref_ROITotalX + 3 + arrPocketPositionROIs[1].ref_ROIWidth / 2) * fDrawingScaleX,
                          (arrPocketPositionROIs[1].ref_ROITotalY - 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY,
                      (arrPocketPositionROIs[1].ref_ROITotalX - 3 + arrPocketPositionROIs[1].ref_ROIWidth / 2) * fDrawingScaleX,
                      (arrPocketPositionROIs[1].ref_ROITotalY + 3 + arrPocketPositionROIs[1].ref_ROIHeight / 2) * fDrawingScaleY);
                    }
                }
            }
            if (!blnWantUsePocketPattern && blnWantUsePocketGauge)
            {
                if (m_blnGaugeResult)
                {
                    g.DrawLine(new Pen(Color.Lime), arrPocketPositionGauges[1].ref_ObjectCenterX * fDrawingScaleX, arrPocketPositionGauges[0].ref_ObjectCenterY * fDrawingScaleY,
                    arrPocketPositionGauges[0].ref_ObjectCenterX * fDrawingScaleX, arrPocketPositionGauges[0].ref_ObjectCenterY * fDrawingScaleY);

                    g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[1].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY - 3) * fDrawingScaleY,
                    (arrPocketPositionGauges[1].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY + 3) * fDrawingScaleY);

                    g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[1].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY - 3) * fDrawingScaleY,
                 (arrPocketPositionGauges[1].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY + 3) * fDrawingScaleY);

                    g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[0].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY - 3) * fDrawingScaleY,
                 (arrPocketPositionGauges[0].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY + 3) * fDrawingScaleY);

                    g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[0].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY - 3) * fDrawingScaleY,
                 (arrPocketPositionGauges[0].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY + 3) * fDrawingScaleY);
                }
                else
                {
                    g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
                        (arrPocketPositionROIs[2].ref_ROITotalY + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
                    (arrPocketPositionROIs[2].ref_ROITotalX + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
                    (arrPocketPositionROIs[2].ref_ROITotalY + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);

                    g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX - 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
                           (arrPocketPositionROIs[2].ref_ROITotalY - 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
                       (arrPocketPositionROIs[3].ref_ROITotalX + 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
                       (arrPocketPositionROIs[2].ref_ROITotalY + 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);

                    g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX + 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
                       (arrPocketPositionROIs[2].ref_ROITotalY - 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
                   (arrPocketPositionROIs[3].ref_ROITotalX - 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
                   (arrPocketPositionROIs[2].ref_ROITotalY + 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);

                    g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[2].ref_ROITotalX - 3 + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
                       (arrPocketPositionROIs[2].ref_ROITotalY - 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
                   (arrPocketPositionROIs[2].ref_ROITotalX + 3 + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
                   (arrPocketPositionROIs[2].ref_ROITotalY + 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);

                    g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[2].ref_ROITotalX + 3 + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
                       (arrPocketPositionROIs[2].ref_ROITotalY - 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
                   (arrPocketPositionROIs[2].ref_ROITotalX - 3 + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
                   (arrPocketPositionROIs[2].ref_ROITotalY + 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);
                }
            }
            if (blnWantUsePocketPattern && blnWantUsePocketGauge)
            {
                if (m_objMatcher.NumPositions > 0)
                {
                    g.DrawRectangle(new Pen(Color.Yellow), (arrPocketPositionROIs[0].ref_ROITotalX + GetMatchingCenterX() - m_objMatcher.PatternWidth / 2) * fDrawingScaleX,
                        (arrPocketPositionROIs[0].ref_ROITotalY + GetMatchingCenterY() - m_objMatcher.PatternHeight / 2) * fDrawingScaleY, m_objMatcher.PatternWidth * fDrawingScaleX, m_objMatcher.PatternHeight * fDrawingScaleY);

                    if (m_blnGaugeResult)
                    {
                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[1].ref_ObjectCenterX) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY) * fDrawingScaleY,
                            (arrPocketPositionGauges[0].ref_ObjectCenterX) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[1].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY - 3) * fDrawingScaleY,
                                  (arrPocketPositionGauges[1].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY + 3) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[1].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY - 3) * fDrawingScaleY,
                           (arrPocketPositionGauges[1].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY + 3) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[0].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY - 3) * fDrawingScaleY,
                            (arrPocketPositionGauges[0].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY + 3) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[0].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY - 3) * fDrawingScaleY,
                            (arrPocketPositionGauges[0].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY + 3) * fDrawingScaleY);
                    }
                    else
                    {
                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
       (arrPocketPositionROIs[2].ref_ROITotalY + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
   (arrPocketPositionROIs[2].ref_ROITotalX + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
   (arrPocketPositionROIs[2].ref_ROITotalY + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX - 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
        (arrPocketPositionROIs[2].ref_ROITotalY - 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
    (arrPocketPositionROIs[3].ref_ROITotalX + 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
    (arrPocketPositionROIs[2].ref_ROITotalY + 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX + 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
       (arrPocketPositionROIs[2].ref_ROITotalY - 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
   (arrPocketPositionROIs[3].ref_ROITotalX - 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
   (arrPocketPositionROIs[2].ref_ROITotalY + 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[2].ref_ROITotalX - 3 + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
       (arrPocketPositionROIs[2].ref_ROITotalY - 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
   (arrPocketPositionROIs[2].ref_ROITotalX + 3 + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
   (arrPocketPositionROIs[2].ref_ROITotalY + 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[2].ref_ROITotalX + 3 + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
       (arrPocketPositionROIs[2].ref_ROITotalY - 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
   (arrPocketPositionROIs[2].ref_ROITotalX - 3 + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
   (arrPocketPositionROIs[2].ref_ROITotalY + 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);
                    }
                }
                else
                {
                    g.DrawRectangle(new Pen(Color.Red), arrPocketPositionROIs[1].ref_ROITotalX * fDrawingScaleX, arrPocketPositionROIs[1].ref_ROITotalY * fDrawingScaleY, 
                        arrPocketPositionROIs[1].ref_ROIWidth * fDrawingScaleX, arrPocketPositionROIs[1].ref_ROIHeight * fDrawingScaleY);

                    if (m_blnGaugeResult)
                    {
                        g.DrawLine(new Pen(Color.Lime), arrPocketPositionGauges[1].ref_ObjectCenterX * fDrawingScaleX, arrPocketPositionGauges[0].ref_ObjectCenterY * fDrawingScaleY,
                            arrPocketPositionGauges[0].ref_ObjectCenterX * fDrawingScaleX, arrPocketPositionGauges[0].ref_ObjectCenterY * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[1].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY - 3) * fDrawingScaleY,
                             (arrPocketPositionGauges[1].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY + 3) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[1].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY - 3) * fDrawingScaleY,
                            (arrPocketPositionGauges[1].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY + 3) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[0].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY - 3) * fDrawingScaleY,
                            (arrPocketPositionGauges[0].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY + 3) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Lime), (arrPocketPositionGauges[0].ref_ObjectCenterX + 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY - 3) * fDrawingScaleY,
                            (arrPocketPositionGauges[0].ref_ObjectCenterX - 3) * fDrawingScaleX, (arrPocketPositionGauges[0].ref_ObjectCenterY + 3) * fDrawingScaleY);
                    }
                    else
                    {
                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
       (arrPocketPositionROIs[2].ref_ROITotalY + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
   (arrPocketPositionROIs[2].ref_ROITotalX + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
   (arrPocketPositionROIs[2].ref_ROITotalY + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX - 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
          (arrPocketPositionROIs[2].ref_ROITotalY - 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
      (arrPocketPositionROIs[3].ref_ROITotalX + 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
      (arrPocketPositionROIs[2].ref_ROITotalY + 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[3].ref_ROITotalX + 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
       (arrPocketPositionROIs[2].ref_ROITotalY - 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
   (arrPocketPositionROIs[3].ref_ROITotalX - 3 + arrPocketPositionROIs[3].ref_ROIWidth / 2) * fDrawingScaleX,
   (arrPocketPositionROIs[2].ref_ROITotalY + 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[2].ref_ROITotalX - 3 + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
       (arrPocketPositionROIs[2].ref_ROITotalY - 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
   (arrPocketPositionROIs[2].ref_ROITotalX + 3 + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
   (arrPocketPositionROIs[2].ref_ROITotalY + 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);

                        g.DrawLine(new Pen(Color.Red), (arrPocketPositionROIs[2].ref_ROITotalX + 3 + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
       (arrPocketPositionROIs[2].ref_ROITotalY - 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY,
   (arrPocketPositionROIs[2].ref_ROITotalX - 3 + arrPocketPositionROIs[2].ref_ROIWidth / 2) * fDrawingScaleX,
   (arrPocketPositionROIs[2].ref_ROITotalY + 3 + arrPocketPositionROIs[2].ref_ROIHeight / 2) * fDrawingScaleY);
                    }
                }
            }
        }
    }
}
