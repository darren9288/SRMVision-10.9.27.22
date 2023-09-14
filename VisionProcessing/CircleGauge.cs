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
    public class CirGauge
    {
        #region Member Variables
        private ECircleGauge m_CircleGauge;
                
        //font properties
        private Font m_FontMatched = new Font("Tahoma", 8);
        private SolidBrush m_BrushMatched = new SolidBrush(Color.GreenYellow);

        private bool m_blnDrawSampledPoints = true;
        private bool m_blnDrawDraggingBox = true;

        private int m_intMinScore = 40;

        private float m_fTemplateObjectDiameter = 0;
        #endregion

        #region Properties

        public float ref_fTemplateObjectDiameter { get { return m_fTemplateObjectDiameter; } set { m_fTemplateObjectDiameter = value; } }
        public bool ref_blnDrawSamplingPoint { get { return m_blnDrawSampledPoints; } set { m_blnDrawSampledPoints = value; } }
        public bool ref_blnDrawDraggingBox { get { return m_blnDrawDraggingBox; } set { m_blnDrawDraggingBox = value; } }
        public float ref_GaugeCenterX { get { return m_CircleGauge.Center.X; } }
        public float ref_GaugeCenterY { get { return m_CircleGauge.Center.Y; } }
        public float ref_GaugeDiameter { get { return m_CircleGauge.Diameter; } }
#if (Debug_2_12 || Release_2_12)
        public int ref_GaugeMinAmplitude { get { return (int)m_CircleGauge.MinAmplitude; } set { m_CircleGauge.MinAmplitude = (uint)value; } }
        public int ref_GaugeMinArea { get { return (int)m_CircleGauge.MinArea; } set { m_CircleGauge.MinArea = (uint)value; } }
        public int ref_GaugeFilter { get { return (int)m_CircleGauge.Smoothing; } set { m_CircleGauge.Smoothing = (uint)value; } }
        public int ref_GaugeThickness { get { return (int)m_CircleGauge.Thickness; } set { m_CircleGauge.Thickness = (uint)value; } }
        public int ref_intMinScore { get { return m_intMinScore; } set { m_intMinScore = value; } }
        public float ref_GaugeSamplingStep { get { return m_CircleGauge.SamplingStep; } set { m_CircleGauge.SamplingStep = value; } }
        public int ref_GaugeThreshold { get { return (int)m_CircleGauge.Threshold; } set { m_CircleGauge.Threshold = (uint)value; } }
        public int ref_GaugeTransType { get { return m_CircleGauge.TransitionType.GetHashCode(); } set { m_CircleGauge.TransitionType = (ETransitionType)value; } }
        public int ref_GaugeTransChoice { get { return m_CircleGauge.TransitionChoice.GetHashCode(); } set { m_CircleGauge.TransitionChoice = (ETransitionChoice)value; } }
        public float ref_GaugeTolerance { get { return m_CircleGauge.Tolerance; } set { m_CircleGauge.Tolerance = value; } }
        public float ref_GaugeFilterThreshold { get { return m_CircleGauge.FilteringThreshold; } set { m_CircleGauge.FilteringThreshold = value; } }
        public int ref_GaugeFilterPasses { get { return (int)m_CircleGauge.NumFilteringPasses; } set { m_CircleGauge.NumFilteringPasses = (uint)value; } }

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
        public int ref_GaugeMinAmplitude { get { return m_CircleGauge.MinAmplitude; } set { m_CircleGauge.MinAmplitude = value; } }
        public int ref_GaugeMinArea { get { return m_CircleGauge.MinArea; } set { m_CircleGauge.MinArea = value; } }
        public int ref_GaugeFilter { get { return m_CircleGauge.Smoothing; } set { m_CircleGauge.Smoothing = value; } }
        public int ref_GaugeThickness { get { return m_CircleGauge.Thickness; } set { m_CircleGauge.Thickness = value; } }
        public int ref_intMinScore { get { return m_intMinScore; } set { m_intMinScore = value; } }
        public float ref_GaugeSamplingStep { get { return m_CircleGauge.SamplingStep; } set { m_CircleGauge.SamplingStep = value; } }
        public int ref_GaugeThreshold { get { return m_CircleGauge.Threshold; } set { m_CircleGauge.Threshold = value; } }
        public int ref_GaugeTransType { get { return m_CircleGauge.TransitionType.GetHashCode(); } set { m_CircleGauge.TransitionType = (ETransitionType)value; } }
        public int ref_GaugeTransChoice { get { return m_CircleGauge.TransitionChoice.GetHashCode(); } set { m_CircleGauge.TransitionChoice = (ETransitionChoice)value; } }
        public float ref_GaugeTolerance { get { return m_CircleGauge.Tolerance; } set { m_CircleGauge.Tolerance = value; } }
        public float ref_GaugeFilterThreshold { get { return m_CircleGauge.FilteringThreshold; } set { m_CircleGauge.FilteringThreshold = value; } }
        public int ref_GaugeFilterPasses { get { return m_CircleGauge.NumFilteringPasses; } set { m_CircleGauge.NumFilteringPasses = value; } }

#endif

        public float ref_fDiameter { get { return m_CircleGauge.MeasuredCircle.Diameter; } }
        public float ref_ObjectCenterX { get { return m_CircleGauge.MeasuredCircle.Center.X; } }
        public float ref_ObjectCenterY { get { return m_CircleGauge.MeasuredCircle.Center.Y; } }

        public float ref_GaugeScore { get { return m_CircleGauge.NumValidSamples * 100 / (float)m_CircleGauge.NumSamples; } }
        #endregion


        public CirGauge(float fTolerance, float fDiameter, GaugeWorldShape objWorldShape)
        {
            m_CircleGauge = new ECircleGauge();
            m_CircleGauge.Tolerance = fTolerance;
            m_CircleGauge.Diameter = fDiameter;
            m_CircleGauge.SamplingStep = 3;
            m_CircleGauge.NumFilteringPasses = 3;//1
            m_CircleGauge.FilteringThreshold = 3f;//3.1f
            m_CircleGauge.TransitionChoice = ETransitionChoice.NthFromEnd;
            m_CircleGauge.TransitionType = ETransitionType.Wb; 

            m_CircleGauge.Attach(objWorldShape.ref_objWorldShape);
        }



        /// <summary>
        /// Draw circle gauge
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        public void DrawCircleGauge(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnDrawSampledPoints, bool blnDrawDraggingBox)
        {
            g.DrawString("Diameter1: " + m_CircleGauge.MeasuredCircle.Diameter.ToString(), 
                        m_FontMatched, new SolidBrush(Color.Red), 
                        (float)(m_CircleGauge.Center.X - 50) * fDrawingScaleX, 
                        (float)(m_CircleGauge.Center.Y - 10) * fDrawingScaleY);
         
            if (blnDrawSampledPoints)
            {
                m_CircleGauge.Draw(g, new ERGBColor(255, 255, 0), EDrawingMode.SampledPoints, true);
                m_CircleGauge.Draw(g, new ERGBColor(255, 0, 0), EDrawingMode.InvalidSampledPoints, true);
            }
            if(blnDrawDraggingBox)
                m_CircleGauge.Draw(g, new ERGBColor(0, 0, 255));

            m_CircleGauge.Draw(g, new ERGBColor(0, 255, 0), EDrawingMode.Actual, true);
        }
        public void DrawSealCircleGauge(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnDrawSampledPoints, bool blnDrawDraggingBox, Color[] arrColor)
        {
            g.DrawString("Diameter1: " + m_CircleGauge.MeasuredCircle.Diameter.ToString(),
                        m_FontMatched, new SolidBrush(Color.Red),
                        (float)(m_CircleGauge.Center.X - 50) * fDrawingScaleX,
                        (float)(m_CircleGauge.Center.Y - 10) * fDrawingScaleY);

            if (blnDrawSampledPoints)
            {
                m_CircleGauge.Draw(g, new ERGBColor(255, 255, 0), EDrawingMode.SampledPoints, true);
                m_CircleGauge.Draw(g, new ERGBColor(255, 0, 0), EDrawingMode.InvalidSampledPoints, true);
            }
            if (blnDrawDraggingBox)
                m_CircleGauge.Draw(g, new ERGBColor(0, 0, 255));

            m_CircleGauge.Draw(g, new ERGBColor(arrColor[1].R, arrColor[1].G, arrColor[1].B), EDrawingMode.Actual, true);
        }
        public void DrawCircleGauge(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            g.DrawString("Diameter1: " + m_CircleGauge.MeasuredCircle.Diameter.ToString(),
                        m_FontMatched, new SolidBrush(Color.Red),
                        (float)(m_CircleGauge.Center.X - 50) * fDrawingScaleX,
                        (float)(m_CircleGauge.Center.Y - 10) * fDrawingScaleY);
            
            if (m_blnDrawSampledPoints)
            {
                m_CircleGauge.Draw(g, new ERGBColor(255, 255, 0), EDrawingMode.SampledPoints, true);
                m_CircleGauge.Draw(g, new ERGBColor(255, 0, 0), EDrawingMode.InvalidSampledPoints, true);
            }
            if (m_blnDrawDraggingBox)
                m_CircleGauge.Draw(g, new ERGBColor(0, 0, 255));
            m_CircleGauge.Draw(g, new ERGBColor(0, 255, 0), EDrawingMode.Actual, true);
        }
        public void DrawCircleGaugeResult(Graphics g, float fDrawingScaleX, float fDrawingScaleY, Color objColor)
        {
            if ((m_CircleGauge.NumValidSamples * 100 / (float)m_CircleGauge.NumSamples) > m_intMinScore)
                m_CircleGauge.Draw(g, new ERGBColor(objColor.R, objColor.G, objColor.B), EDrawingMode.Actual, true);
            else
                m_CircleGauge.Draw(g, new ERGBColor(255, 0, 0), EDrawingMode.Actual, true);
        }
        /// <summary>
        /// Measure circle gauge
        /// </summary>
        /// <param name="objROI">ROI</param>
        public void Measure(ROI objROI)
        {
            m_CircleGauge.Measure(objROI.ref_ROI.TopParent);
        }
        public void Measure(ImageDrawing objImage)
        {
            //objImage.SaveImage("D:\\TS\\TopParent.bmp");
            //m_CircleGauge.Save("D:\\TS\\CircleGauge.CAL");
            m_CircleGauge.Measure(objImage.ref_objMainImage);
        }
        /// <summary>
        /// Modify circle gauge placement 
        /// </summary>
        /// <param name="objSearchROI">search ROI</param>
        public void ModifyGauge(ROI objSearchROI)
        {
            float fCenterX = objSearchROI.ref_ROIPositionX + objSearchROI.ref_ROIWidth / 2;
            float fCenterY = objSearchROI.ref_ROIPositionY + objSearchROI.ref_ROIHeight / 2;
            m_CircleGauge.SetCenterXY(fCenterX, fCenterY);
        }

        /// <summary>
        /// Set gauge placement
        /// </summary>
        /// <param name="objSearchROI">search ROI</param>
        public void SetGaugePlacement(ROI objSearchROI)
        {
            float fCenterX = objSearchROI.ref_ROIPositionX + objSearchROI.ref_ROIWidth / 2;
            float fCenterY = objSearchROI.ref_ROIPositionY + objSearchROI.ref_ROIHeight / 2;
            m_CircleGauge.SetCenterXY(fCenterX, fCenterY);
            if (objSearchROI.ref_ROIWidth < objSearchROI.ref_ROIHeight)
                m_CircleGauge.Diameter = objSearchROI.ref_ROIWidth - 80;
            else
                m_CircleGauge.Diameter = objSearchROI.ref_ROIHeight - 80;
        }

        public void SetGaugePlacement(ROI objSearchROI, int intTolerance)
        {
            float fCenterX = objSearchROI.ref_ROIPositionX + objSearchROI.ref_ROIWidth / 2;
            float fCenterY = objSearchROI.ref_ROIPositionY + objSearchROI.ref_ROIHeight / 2;
            m_CircleGauge.SetCenterXY(fCenterX, fCenterY);
            if (objSearchROI.ref_ROIWidth < objSearchROI.ref_ROIHeight)
                m_CircleGauge.Diameter = objSearchROI.ref_ROIWidth - intTolerance;
            else
                m_CircleGauge.Diameter = objSearchROI.ref_ROIHeight - intTolerance;
        }
        public void SetGaugePlacement(ROI objSearchROI, int intTolerance, bool blnTop)
        {
            float fCenterX = objSearchROI.ref_ROIPositionX + objSearchROI.ref_ROIWidth / 2;
            float fCenterY = objSearchROI.ref_ROIPositionY + objSearchROI.ref_ROIHeight / 2;

            if (objSearchROI.ref_ROIWidth < objSearchROI.ref_ROIHeight)
            {
                m_CircleGauge.SetCenterXY(fCenterX, fCenterY);
                m_CircleGauge.Diameter = objSearchROI.ref_ROIWidth - intTolerance;
            }
            else
            {
                if (blnTop)
                    fCenterY = objSearchROI.ref_ROIPositionY + objSearchROI.ref_ROIHeight - (objSearchROI.ref_ROIWidth / 2);
                else
                    fCenterY = objSearchROI.ref_ROIPositionY + (objSearchROI.ref_ROIWidth / 2);
                m_CircleGauge.SetCenterXY(fCenterX, fCenterY);
                m_CircleGauge.Diameter = objSearchROI.ref_ROIWidth - intTolerance;
            }
        }
        /// <summary>
        /// Set circle gauge transition choice
        /// </summary>
        /// <param name="intChoiceNo">choice no, 0 = NthFromENd, other = NthFromBegin</param>
        public void SetTransitionChoice(int intChoiceNo)
        {
            m_CircleGauge.TransitionChoice = (ETransitionChoice)intChoiceNo;
        }

        /// <summary>
        /// Set circle gauge transition type
        /// </summary>
        /// <param name="intTypeNo">type no, 0 = white to black, other = black to white</param>
        public void SetTransitionType(int intTransType)
        {
            m_CircleGauge.TransitionType = (ETransitionType)intTransType;
        }

        /// <summary>
        /// Set gauge thickness value
        /// </summary>
        /// <param name="intThickness"></param>
        public void SetThickness(int intThickness)
        {
#if (Debug_2_12 || Release_2_12)
            m_CircleGauge.Thickness = (uint)intThickness;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_CircleGauge.Thickness = intThickness;
#endif

        }

        /// <summary>
        ///  Set gauge threshold value
        /// </summary>
        /// <param name="intThreshold"></param>
        public void SetThreshold(int intThreshold)
        {
#if (Debug_2_12 || Release_2_12)
            m_CircleGauge.Threshold = (uint)intThreshold;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_CircleGauge.Threshold = intThreshold;
#endif

        }

        public void Dispose()
        {
            m_BrushMatched.Dispose();

            if (m_CircleGauge != null)
                m_CircleGauge.Dispose();
        }

        public void SaveCircleGauge(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection, bool blnSaveMeasurementAsTemplate, bool blnSaveMeasuredDiameter)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);

            // Point gauge position setting
            objFile.WriteElement1Value("CenterX", m_CircleGauge.Center.X, "Gauge Setting Center X", true);
            objFile.WriteElement1Value("CenterY", m_CircleGauge.Center.Y, "Gauge Setting Center X", true);
            objFile.WriteElement1Value("Tolerance", m_CircleGauge.Tolerance, "Gauge Setting Tolerance", true);
            objFile.WriteElement1Value("Diameter", m_CircleGauge.Diameter, "Gauge Diameter", true);
            // Rectangle gauge measurement setting
            objFile.WriteElement1Value("TransType", m_CircleGauge.TransitionType.GetHashCode(), "Gauge Setting Transition Type", true);
            objFile.WriteElement1Value("TransChoice", m_CircleGauge.TransitionChoice.GetHashCode(), "Gauge Setting Transition Choice", true);
            objFile.WriteElement1Value("Threshold", m_CircleGauge.Threshold, "Gauge Setting Threshold", true);
            objFile.WriteElement1Value("Thickness", m_CircleGauge.Thickness, "Gauge Setting Thickness", true);
            objFile.WriteElement1Value("MinAmp", m_CircleGauge.MinAmplitude, "Gauge Setting Minimum Amplitude", true);
            objFile.WriteElement1Value("MinArea", m_CircleGauge.MinArea, "Gauge Setting Minimum Area", true);
            objFile.WriteElement1Value("Filter", m_CircleGauge.Smoothing, "Gauge Setting Filter/Smoothing", true);
            objFile.WriteElement1Value("FilterPass", m_CircleGauge.NumFilteringPasses, "Gauge Setting FilterPass", true);
            objFile.WriteElement1Value("FilterThreshold", m_CircleGauge.FilteringThreshold, "Gauge Setting FilterThreshold", true);
            objFile.WriteElement1Value("DrawSampledPoints", m_blnDrawSampledPoints, "Draw Gauge SampledPoints", true);
            objFile.WriteElement1Value("DrawDraggingBox", m_blnDrawDraggingBox, "Draw Gauge DraggingBox", true);
            objFile.WriteElement1Value("MinScore", m_intMinScore, "Gauge Min Score", true);

            if (blnSaveMeasuredDiameter)
            {
                m_fTemplateObjectDiameter = m_CircleGauge.MeasuredCircle.Diameter;
                objFile.WriteElement1Value("TemplateObjectDiameter", m_CircleGauge.MeasuredCircle.Diameter, "Measured Template Circle Diameter", true);
            }

            objFile.WriteEndElement();

        }

        public void LoadCircleGauge(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection(strSectionName);

            // Point gauge position setting
            m_CircleGauge.SetCenterXY(objFile.GetValueAsFloat("CenterX", 0), objFile.GetValueAsFloat("CenterY", 0));
            m_CircleGauge.Tolerance = objFile.GetValueAsFloat("Tolerance", 10);
            m_CircleGauge.Diameter = objFile.GetValueAsFloat("Diameter", 10);
            // Point gauge measurement setting
            m_CircleGauge.TransitionType = (ETransitionType)(objFile.GetValueAsInt("TransType", 0));
            m_CircleGauge.TransitionChoice = (ETransitionChoice)(objFile.GetValueAsInt("TransChoice", 0));
#if (Debug_2_12 || Release_2_12)
            m_CircleGauge.Threshold = objFile.GetValueAsUInt("Threshold", 2);
            m_CircleGauge.Thickness = objFile.GetValueAsUInt("Thickness", 13);
            m_CircleGauge.MinAmplitude = objFile.GetValueAsUInt("MinAmp", 10);
            m_CircleGauge.MinArea = objFile.GetValueAsUInt("MinArea", 0);
            m_CircleGauge.Smoothing = objFile.GetValueAsUInt("Filter", 1);
            m_CircleGauge.NumFilteringPasses = objFile.GetValueAsUInt("FilterPass", 3);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_CircleGauge.Threshold = objFile.GetValueAsInt("Threshold", 2);
            m_CircleGauge.Thickness = objFile.GetValueAsInt("Thickness", 13);
            m_CircleGauge.MinAmplitude = objFile.GetValueAsInt("MinAmp", 10);
            m_CircleGauge.MinArea = objFile.GetValueAsInt("MinArea", 0);
            m_CircleGauge.Smoothing = objFile.GetValueAsInt("Filter", 1);
            m_CircleGauge.NumFilteringPasses = objFile.GetValueAsInt("FilterPass", 3);
#endif

            m_CircleGauge.FilteringThreshold = objFile.GetValueAsInt("FilterThreshold", 3);
            m_blnDrawSampledPoints = objFile.GetValueAsBoolean("DrawSampledPoints", true);
            m_blnDrawDraggingBox = objFile.GetValueAsBoolean("DrawDraggingBox", true);
            m_intMinScore = objFile.GetValueAsInt("MinScore", 0);

            m_fTemplateObjectDiameter = objFile.GetValueAsFloat("TemplateObjectDiameter", 0);
        }

        public float GetPreciseGaugeScore(ImageDrawing objImage)
        {
            float fResultCenterX = m_CircleGauge.MeasuredCircle.Center.X;
            float fResultCenterY = m_CircleGauge.MeasuredCircle.Center.Y;
            float fResultDiameter = m_CircleGauge.MeasuredCircle.Diameter;
            float fGaugeCenterXPrev = m_CircleGauge.Center.X;
            float fGaugeCenterYPrev = m_CircleGauge.Center.Y;
            float fGaugeTolerancePrev = m_CircleGauge.Tolerance;
            float fGaugeDiameterPrev = m_CircleGauge.Diameter;

            //m_CircleGauge.Save("D:\\TS\\CircleGauge.CAL");
            float fOriScore = ref_GaugeScore;


            m_CircleGauge.SetCenterXY(fResultCenterX, fResultCenterY);
            m_CircleGauge.Tolerance = 10;
            m_CircleGauge.Diameter = fResultDiameter;
            Measure(objImage);

            float fPreciseScore = ref_GaugeScore;

            //m_CircleGauge.Save("D:\\TS\\CircleGauge2.CAL");

            m_CircleGauge.SetCenterXY(fGaugeCenterXPrev, fGaugeCenterYPrev);
            m_CircleGauge.Tolerance = fGaugeTolerancePrev;
            m_CircleGauge.Diameter = fGaugeDiameterPrev;
            Measure(objImage);
            float fOriScore2 = ref_GaugeScore;

            float fScore = fPreciseScore / fOriScore * 100;
            return Math.Min(fScore, 100f);
        }
    }
}
