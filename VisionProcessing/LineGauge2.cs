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
using Common;
using System.Drawing;

namespace VisionProcessing
{
    public class LGauge2
    {
        #region Member Variables
        private int m_intStartX = 0;
        private int m_intStartY = 0;
        private int m_intWidthLimit = 0;
        private int m_intHeightLimit = 0;
        private List<PointF> m_arrFilteredPoints = new List<PointF>();
        private Line m_objLine = new Line();
        //private List<List<PointF>> m_arrPocketEdgePoints = new List<List<PointF>>();
        private List<PointF> m_arrPocketEdgePoints = new List<PointF>();
        private List<PGauge> m_arrPocketEdgePGauge = new List<PGauge>();
        private int m_intThickness = 0;
        private bool m_blnDrawDraggingBox = false;
        private bool m_blnDrawSamplingPoint = false;
        private bool m_blnDrawTexture = false;
        private float m_fGaugeImageGain = 1;
        private int m_intGaugeImageThreshold = 125;
        //private int m_intLineOffset = 0;
        //private int m_intMaskThickness = 0;
        private int m_intSearchLineNumber = 0;
        private int m_intROIDirection = 0;//0:Center, 1:Top, 2:Right, 3:Bottom, 4:Left
        #endregion

        #region Properties
        //public int ref_intPocketEdgePGaugeCount { get { return m_arrPocketEdgePGauge.Count; } }
        public List<PGauge> ref_arrPocketEdgePGauge { get { return m_arrPocketEdgePGauge; } }
        public bool ref_blnDrawDraggingBox { get { return m_blnDrawDraggingBox; } set { m_blnDrawDraggingBox = value; } }
        public bool ref_blnDrawSamplingPoint { get { return m_blnDrawSamplingPoint; } set { m_blnDrawSamplingPoint = value; } }
        public bool ref_blnDrawTexture { get { return m_blnDrawTexture; } set { m_blnDrawTexture = value; } }
        public float ref_fGaugeImageGain { get { return m_fGaugeImageGain; } set { m_fGaugeImageGain = value; } }
        public int ref_intGaugeImageThreshold { get { return m_intGaugeImageThreshold; } set { m_intGaugeImageThreshold = value; } }
        public int ref_intSearchLineNumber { get { return m_intSearchLineNumber; } set { m_intSearchLineNumber = value; } }
        public int ref_intThickness { get { return m_intThickness; } set { m_intThickness = value; } }
        public Line ref_objLine { get { return m_objLine; } }

        public float ref_GaugeCenterX { get { return m_arrPocketEdgePGauge[0].ref_GaugeCenterX; } }
        public float ref_GaugeCenterY { get { return m_arrPocketEdgePGauge[0].ref_GaugeCenterY; } }
        public float ref_GaugeAngle { get { return m_arrPocketEdgePGauge[0].ref_GaugeAngle; } /*set { m_arrPocketEdgePGauge[0].ref_GaugeAngle = value; }*/ }
        public int ref_GaugeMinAmplitude { get { return m_arrPocketEdgePGauge[0].ref_GaugeMinAmplitude; } /*set { m_arrPocketEdgePGauge[0].ref_GaugeMinAmplitude = value; }*/ }
        public int ref_GaugeMinArea { get { return m_arrPocketEdgePGauge[0].ref_GaugeMinArea; } /*set { m_arrPocketEdgePGauge[0].ref_GaugeMinArea = value; }*/ }
        public int ref_GaugeFilter { get { return m_arrPocketEdgePGauge[0].ref_GaugeFilter; } /*set { m_arrPocketEdgePGauge[0].ref_GaugeFilter = value; }*/ }
        public int ref_GaugeThickness { get { return m_arrPocketEdgePGauge[0].ref_GaugeThickness; } /*set { m_arrPocketEdgePGauge[0].ref_GaugeThickness = value; }*/ }

        public int ref_GaugeThreshold { get { return m_arrPocketEdgePGauge[0].ref_GaugeThreshold; } /*set { m_arrPocketEdgePGauge[0].ref_GaugeThreshold = value; }*/ }
        public int ref_GaugeTransType { get { return m_arrPocketEdgePGauge[0].ref_GaugeTransType; } /*set { m_arrPocketEdgePGauge[0].ref_GaugeTransType = value; }*/ }
        public int ref_GaugeTransChoice { get { return m_arrPocketEdgePGauge[0].ref_GaugeTransChoice; } /*set { m_arrPocketEdgePGauge[0].ref_GaugeTransChoice = value; }*/ }
        public float ref_GaugeTolerance { get { return m_arrPocketEdgePGauge[0].ref_GaugeTolerance; } /*set { m_arrPocketEdgePGauge[0].ref_GaugeTolerance = value; }*/ }

        // result
        //public float ref_ObjectCenterX { get { return m_LineGauge.MeasuredLine.CenterX; } }
        //public float ref_ObjectCenterY { get { return m_LineGauge.MeasuredLine.CenterY; } }
        #endregion


        public LGauge2(int intROIDirection)
        {
            m_arrPocketEdgePGauge = new List<PGauge>();
            m_arrPocketEdgePGauge.Add(new PGauge());
            m_intROIDirection = intROIDirection;
            m_arrPocketEdgePGauge[0].ref_GaugeTransChoice = 5;
            

        }

        public LGauge2(GaugeWorldShape objWorldShape, int intROIDirection)
        {
            m_arrPocketEdgePGauge = new List<PGauge>();
            m_arrPocketEdgePGauge.Add(new PGauge(objWorldShape));
            m_intROIDirection = intROIDirection;
            m_arrPocketEdgePGauge[0].ref_GaugeTransChoice = 5;
            

        }

        public void SavePocketEdgeGauge(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);
            objFile.WriteElement1Value("ROIDirection", m_intROIDirection, "ROI Direction", true);
            objFile.WriteElement1Value("DrawDraggingBox", m_blnDrawDraggingBox, "Draw Dragging Box", true);
            objFile.WriteElement1Value("DrawSamplingPoint", m_blnDrawSamplingPoint, "Draw Sampling Point", true);
            objFile.WriteElement1Value("DrawTexture", m_blnDrawTexture, "Draw Texture", true);

            objFile.WriteElement1Value("GaugeImageGain", m_fGaugeImageGain, "Gauge Image Gain", true);
            objFile.WriteElement1Value("GaugeImageThreshold", m_intGaugeImageThreshold, "Gauge Image Threshold", true);

            objFile.WriteElement1Value("Thickness", m_intThickness, "Thickness", true);

            //objFile.WriteElement1Value("MaskThickness", m_intMaskThickness, "Mask Thickness", true);
            //objFile.WriteElement1Value("LineOffset", m_intLineOffset, "Line Offset", true);
            objFile.WriteElement1Value("SearchLineNumber", m_intSearchLineNumber, "Search Line Number", true);

            objFile.WriteElement1Value("PocketEdgePGaugeCount", m_arrPocketEdgePGauge.Count, "PocketEdgePGauge Count", true);

            for (int i = 0; i < m_arrPocketEdgePGauge.Count; i++)
            {
                objFile.WriteSectionElement(strSectionName + "_" + i, blnNewSection);

                // gauge position setting
                objFile.WriteElement1Value("CenterX", m_arrPocketEdgePGauge[i].ref_GaugeCenterX, "Gauge Setting Center X", true);
                objFile.WriteElement1Value("CenterY", m_arrPocketEdgePGauge[i].ref_GaugeCenterY, "Gauge Setting Center X", true);
                //objFile.WriteElement1Value("Length", m_arrPocketEdgePGauge[i].ref_GaugeLength, "Gauge Setting Length", true);
                objFile.WriteElement1Value("Angle", m_arrPocketEdgePGauge[i].ref_GaugeAngle, "Gauge Setting Angle", true);
                objFile.WriteElement1Value("Tolerance", m_arrPocketEdgePGauge[i].ref_GaugeTolerance, "Gauge Setting Tolerance", true);

                // gauge measurement setting
                objFile.WriteElement1Value("TransType", m_arrPocketEdgePGauge[i].ref_GaugeTransType, "Gauge Setting Transition Type", true);
                objFile.WriteElement1Value("TransChoice", m_arrPocketEdgePGauge[i].ref_GaugeTransChoice, "Gauge Setting Transition Choice", true);
                objFile.WriteElement1Value("Threshold", m_arrPocketEdgePGauge[i].ref_GaugeThreshold, "Gauge Setting Threshold", true);
                objFile.WriteElement1Value("Thickness", m_arrPocketEdgePGauge[i].ref_GaugeThickness, "Gauge Setting Thickness", true);
                objFile.WriteElement1Value("MinAmp", m_arrPocketEdgePGauge[i].ref_GaugeMinAmplitude, "Gauge Setting Minimum Amplitude", true);
                objFile.WriteElement1Value("Filter", m_arrPocketEdgePGauge[i].ref_GaugeFilter, "Gauge Setting Filter/Smoothing", true);

                // gauge fitting setting
                //objFile.WriteElement1Value("SamplingStep", m_arrPocketEdgePGauge[i].ref_GaugeSamplingStep, "Gauge Setting Sampling Step", true);
                //objFile.WriteElement1Value("FilteringThreshold", m_arrPocketEdgePGauge[i].ref_GaugeFilterThreshold, "Gauge Setting Filtering Threshold", true);
                //objFile.WriteElement1Value("FilteringPasses", m_arrPocketEdgePGauge[i].ref_GaugeFilterPasses, "Gauge Setting Filtering Passes", true);

            }

            objFile.WriteEndElement();

        }

        public void LoadPocketEdgeGauge(string strPath, string strSectionName, GaugeWorldShape objWorldShape)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);

            m_blnDrawDraggingBox = objFile.GetValueAsBoolean("DrawDraggingBox", true);
            m_blnDrawSamplingPoint = objFile.GetValueAsBoolean("DrawSamplingPoint", true);
            m_blnDrawTexture = objFile.GetValueAsBoolean("DrawTexture", true);
            m_intROIDirection = objFile.GetValueAsInt("ROIDirection", m_intROIDirection);
            m_fGaugeImageGain = objFile.GetValueAsFloat("GaugeImageGain", 1);
            m_intGaugeImageThreshold = objFile.GetValueAsInt("GaugeImageThreshold", 125);
            //m_intMaskThickness = objFile.GetValueAsInt("MaskThickness", 0);
            //m_intLineOffset = objFile.GetValueAsInt("LineOffset", 0);
            m_intSearchLineNumber = objFile.GetValueAsInt("SearchLineNumber", 0);

            m_intThickness = objFile.GetValueAsInt("Thickness", 0);

            //if (m_arrPocketEdgePGauge.Count == 0)
            //{
            //    m_arrPocketEdgePGauge.Add(new PGauge(objWorldShape));
            //}
            int intPocketEdgePGaugeCount = objFile.GetValueAsInt("PocketEdgePGaugeCount", 1);

            for (int i = 0; i < intPocketEdgePGaugeCount; i++)
            {
                if(m_arrPocketEdgePGauge.Count <= i)
                    m_arrPocketEdgePGauge.Add(new PGauge(objWorldShape));

                objFile.GetFirstSection(strSectionName + "_" + i);

                // Rectangle gauge position setting
                m_arrPocketEdgePGauge[i].SetGaugeCenter(objFile.GetValueAsFloat("CenterX", 0),
                                      objFile.GetValueAsFloat("CenterY", 0));
                //m_arrPocketEdgePGauge[i].SetGaugeLength(objFile.GetValueAsFloat("Length", 100));
                //  m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 0)); // Not need to load angle. Angle has been define during init this RectGaugeM4L class.
                m_arrPocketEdgePGauge[i].ref_GaugeTolerance = objFile.GetValueAsFloat("Tolerance", 10);

                switch (m_intROIDirection)
                {
                    case 1:
                        m_arrPocketEdgePGauge[i].ref_GaugeAngle = 90;    // Top
                        break;
                    case 2:
                        m_arrPocketEdgePGauge[i].ref_GaugeAngle = 180;    // Right
                        break;
                    case 3:
                        m_arrPocketEdgePGauge[i].ref_GaugeAngle = -90;    // Bottom
                        break;
                    case 4:
                        m_arrPocketEdgePGauge[i].ref_GaugeAngle = 0;    // Left
                        break;
                }

                // Rectangle gauge measurement setting
                m_arrPocketEdgePGauge[i].ref_GaugeTransType = objFile.GetValueAsInt("TransType", 0);
                m_arrPocketEdgePGauge[i].ref_GaugeTransChoice = objFile.GetValueAsInt("TransChoice", 5);
                m_arrPocketEdgePGauge[i].ref_GaugeThreshold = objFile.GetValueAsInt("Threshold", 2);
                m_arrPocketEdgePGauge[i].ref_GaugeThickness = objFile.GetValueAsInt("Thickness", 13);
                m_arrPocketEdgePGauge[i].ref_GaugeMinAmplitude = objFile.GetValueAsInt("MinAmp", 10);
                m_arrPocketEdgePGauge[i].ref_GaugeMinArea = objFile.GetValueAsInt("MinArea", 0);
                m_arrPocketEdgePGauge[i].ref_GaugeFilter = objFile.GetValueAsInt("Filter", 1);

                //// Rectangle gauge fitting setting
                //m_arrPocketEdgePGauge[i].ref_GaugeSamplingStep = objFile.GetValueAsInt("SamplingStep", 3);
                //m_arrPocketEdgePGauge[i].ref_GaugeFilterThreshold = objFile.GetValueAsInt("FilteringThreshold", 3);
                //m_arrPocketEdgePGauge[i].ref_GaugeFilterPasses = objFile.GetValueAsInt("FilteringPasses", 3);

            }

        }

        public void SetPGaugeSettingToNewAddedGauge(int intIndex)
        {
            for (int i = intIndex; i < m_arrPocketEdgePGauge.Count; i++)
            {

                switch (m_intROIDirection)
                {
                    case 1:
                        m_arrPocketEdgePGauge[i].ref_GaugeAngle = 90;    // Top
                        break;
                    case 2:
                        m_arrPocketEdgePGauge[i].ref_GaugeAngle = 180;    // Right
                        break;
                    case 3:
                        m_arrPocketEdgePGauge[i].ref_GaugeAngle = -90;    // Bottom
                        break;
                    case 4:
                        m_arrPocketEdgePGauge[i].ref_GaugeAngle = 0;    // Left
                        break;
                }

                // Rectangle gauge measurement setting
                m_arrPocketEdgePGauge[i].ref_GaugeTransType = m_arrPocketEdgePGauge[i - 1].ref_GaugeTransType;
                m_arrPocketEdgePGauge[i].ref_GaugeTransChoice = m_arrPocketEdgePGauge[i - 1].ref_GaugeTransChoice;
                m_arrPocketEdgePGauge[i].ref_GaugeThreshold = m_arrPocketEdgePGauge[i - 1].ref_GaugeThreshold;
                m_arrPocketEdgePGauge[i].ref_GaugeThickness = m_arrPocketEdgePGauge[i - 1].ref_GaugeThickness;
                m_arrPocketEdgePGauge[i].ref_GaugeMinAmplitude = m_arrPocketEdgePGauge[i - 1].ref_GaugeMinAmplitude;
                m_arrPocketEdgePGauge[i].ref_GaugeMinArea = m_arrPocketEdgePGauge[i - 1].ref_GaugeMinArea;
                m_arrPocketEdgePGauge[i].ref_GaugeFilter = m_arrPocketEdgePGauge[i - 1].ref_GaugeFilter;

                //// Rectangle gauge fitting setting
                //m_arrPocketEdgePGauge[i].ref_GaugeSamplingStep = m_arrPocketEdgePGauge[i - 1].ref_GaugeSamplingStep;
                //m_arrPocketEdgePGauge[i].ref_GaugeFilterThreshold = m_arrPocketEdgePGauge[i - 1].ref_GaugeFilterThreshold;
                //m_arrPocketEdgePGauge[i].ref_GaugeFilterPasses = m_arrPocketEdgePGauge[i - 1].ref_GaugeFilterPasses;

            }
        }

        public void SetPGaugePlace(int intStartX, int intStartY, int intWidth, int intHeight)
        {
            // Formula for partition = S + ((L/C) / 2) * ((2 * i) + 1)
            // Where :
            // S = Start X or Y 
            // L = Width or Height
            // C = center point count
            // i = 0, 1 , 2, ...., C - 1

            m_intWidthLimit = intWidth;
            m_intHeightLimit = intHeight;
            m_intStartX = intStartX;
            m_intStartY = intStartY;

            switch (m_intROIDirection)
            {
                case 1: // Top
                    for (int i = 0; i < m_arrPocketEdgePGauge.Count; i++)
                    {
                        int intCenterX = intStartX + ((intWidth / m_arrPocketEdgePGauge.Count) / 2) * ((2 * i) + 1);
                        m_arrPocketEdgePGauge[i].ref_GaugeThickness = (intWidth / m_arrPocketEdgePGauge.Count);
                        m_arrPocketEdgePGauge[i].SetGaugePlacementCenter(intCenterX, intStartY + (intHeight / 2));
                        m_arrPocketEdgePGauge[i].ref_GaugeTolerance = (intHeight / 2);
                        m_arrPocketEdgePGauge[i].ref_GaugeAngle = 90;
                    }
                    break;
                case 2: // Right
                    for (int i = 0; i < m_arrPocketEdgePGauge.Count; i++)
                    {
                        int intCenterY = intStartY + ((intHeight / m_arrPocketEdgePGauge.Count) / 2) * ((2 * i) + 1);
                        m_arrPocketEdgePGauge[i].ref_GaugeThickness = (intHeight / m_arrPocketEdgePGauge.Count);
                        m_arrPocketEdgePGauge[i].SetGaugePlacementCenter(intStartX + (intWidth / 2), intCenterY);
                        m_arrPocketEdgePGauge[i].ref_GaugeTolerance = (intWidth / 2);
                        m_arrPocketEdgePGauge[i].ref_GaugeAngle = 180;
                    }
                    break;
                case 3: // Bottom
                    for (int i = 0; i < m_arrPocketEdgePGauge.Count; i++)
                    {
                        int intCenterX = intStartX + ((intWidth / m_arrPocketEdgePGauge.Count) / 2) * ((2 * i) + 1);
                        m_arrPocketEdgePGauge[i].ref_GaugeThickness = (intWidth / m_arrPocketEdgePGauge.Count);
                        m_arrPocketEdgePGauge[i].SetGaugePlacementCenter(intCenterX, intStartY + (intHeight / 2));
                        m_arrPocketEdgePGauge[i].ref_GaugeTolerance = (intHeight / 2);
                        m_arrPocketEdgePGauge[i].ref_GaugeAngle = -90;
                    }
                    break;
                case 4: // Left
                    for (int i = 0; i < m_arrPocketEdgePGauge.Count; i++)
                    {
                        int intCenterY = intStartY + ((intHeight / m_arrPocketEdgePGauge.Count) / 2) * ((2 * i) + 1);
                        m_arrPocketEdgePGauge[i].ref_GaugeThickness = (intHeight / m_arrPocketEdgePGauge.Count);
                        m_arrPocketEdgePGauge[i].SetGaugePlacementCenter(intStartX + (intWidth / 2), intCenterY);
                        m_arrPocketEdgePGauge[i].ref_GaugeTolerance = (intWidth / 2);
                        m_arrPocketEdgePGauge[i].ref_GaugeAngle = 0;
                    }
                    break;
            }

        }

        public void MeasurePGauge(ROI objMainROI, ImageDrawing objMainImage)
        {
            //ImageDrawing arrModifiedImage = new ImageDrawing();
            //ImageDrawing objPreModifiedImage = new ImageDrawing();
            //arrModifiedImage = new ImageDrawing(true, objMainImage.ref_intImageWidth, objMainImage.ref_intImageHeight);
            //objPreModifiedImage = new ImageDrawing(true, objMainImage.ref_intImageWidth, objMainImage.ref_intImageHeight);

            //arrModifiedImage.SetImageToBlack();
            //ROI objROI = new ROI();
            //objROI.LoadROISetting(objMainROI.ref_ROIPositionX, objMainROI.ref_ROIPositionY,
            //objMainROI.ref_ROIWidth, objMainROI.ref_ROIHeight);
            //objROI.AttachImage(arrModifiedImage);
            ////objMainROI.AttachImage(objMainImage);

            //objMainROI.CopyImage_Bigger(ref objROI);

            ////objMainROI.AttachImage(objMainImage);
            //objROI.GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage, m_fGaugeImageGain);

            //ImageDrawing objImage = arrModifiedImage;
            //objROI.AttachImage(objPreModifiedImage);
            //objROI.ThresholdTo_ROIToROISamePosition_Bigger(ref objImage, m_intGaugeImageThreshold);
            //objImage.SaveImage("D:\\TS\\objImage1.bmp");
            //objMainROI.AttachImage(objImage);

            //m_arrPocketEdgePoints = new List<List<PointF>>();
            m_arrPocketEdgePoints = new List<PointF>();
            for (int i = 0; i < m_arrPocketEdgePGauge.Count; i++)
            {
                
                m_arrPocketEdgePGauge[i].Measure(objMainROI);
                m_arrPocketEdgePGauge[i].GetAllMeasurePoints(ref m_arrPocketEdgePoints);
            }

            List<PointF> arrSortPoints = new List<PointF>();

            if (m_intROIDirection == 2 || m_intROIDirection == 4)
                Math2.SortData(m_arrPocketEdgePoints, ref arrSortPoints, Math2.SortingDirection.X);
            else if (m_intROIDirection == 1 || m_intROIDirection == 3)
                Math2.SortData(m_arrPocketEdgePoints, ref arrSortPoints, Math2.SortingDirection.Y);

            float fCenter = 0;
            float fRange = m_intThickness;
            m_arrFilteredPoints = new List<PointF>();
            if (m_intROIDirection == 2 || m_intROIDirection == 4)
                m_arrFilteredPoints = Math2.GetModeRangeCenterValueAndFilteredList(arrSortPoints, fRange, ref fCenter, Math2.SortingDirection.X);
            else if (m_intROIDirection == 1 || m_intROIDirection == 3)
                m_arrFilteredPoints = Math2.GetModeRangeCenterValueAndFilteredList(arrSortPoints, fRange, ref fCenter, Math2.SortingDirection.Y);

            m_objLine.CalculateStraightLine(m_arrFilteredPoints.ToArray());

            if (m_arrFilteredPoints.Count == 0)
            {
                m_arrPocketEdgePoints = new List<PointF>();
                for (int i = 0; i < m_arrPocketEdgePGauge.Count; i++)
                {

                    //m_arrPocketEdgePGauge[i].Measure(objMainROI);
                    m_arrPocketEdgePoints.Add(m_arrPocketEdgePGauge[i].GetMaxAmplitudeMeasuredPoint());
                }
                m_arrFilteredPoints = m_arrPocketEdgePoints;
                //arrSortPoints = new List<PointF>();

                //if (m_intROIDirection == 2 || m_intROIDirection == 4)
                //    Math2.SortData(m_arrPocketEdgePoints, ref arrSortPoints, Math2.SortingDirection.X);
                //else if (m_intROIDirection == 1 || m_intROIDirection == 3)
                //    Math2.SortData(m_arrPocketEdgePoints, ref arrSortPoints, Math2.SortingDirection.Y);

                //fCenter = 0;
                //fRange = 5;
                //m_arrFilteredPoints = new List<PointF>();
                //if (m_intROIDirection == 2 || m_intROIDirection == 4)
                //    m_arrFilteredPoints = Math2.GetModeRangeCenterValueAndFilteredList(arrSortPoints, fRange, ref fCenter, Math2.SortingDirection.X);
                //else if (m_intROIDirection == 1 || m_intROIDirection == 3)
                //    m_arrFilteredPoints = Math2.GetModeRangeCenterValueAndFilteredList(arrSortPoints, fRange, ref fCenter, Math2.SortingDirection.Y);
                if (m_arrFilteredPoints.Count > 1)
                    m_objLine.CalculateStraightLine(m_arrFilteredPoints.ToArray());
                else
                    m_objLine.CalculateStraightLine(m_arrFilteredPoints[0], 0);
            }

            
            //arrModifiedImage.Dispose();
            //objPreModifiedImage.Dispose();
            //objImage.Dispose();
            //objROI.Dispose();

        }

        public void DrawPGauge(Graphics g, float fScaleX, float fScaleY, int intOffset, int intMaskThickness, TextureBrush objTextureBrush)
        {
            for (int i = 0; i < m_arrPocketEdgePGauge.Count; i++)
            {
                m_arrPocketEdgePGauge[i].DrawGauge(g, false, m_blnDrawSamplingPoint);

            }

            if (m_blnDrawSamplingPoint)
            {
                for (int i = 0; i < m_arrFilteredPoints.Count; i++)
                {
                    g.DrawLine(new Pen(Color.Lime), m_arrFilteredPoints[i].X * fScaleX - 5, m_arrFilteredPoints[i].Y * fScaleY - 5, m_arrFilteredPoints[i].X * fScaleX + 5, m_arrFilteredPoints[i].Y * fScaleY + 5);
                    g.DrawLine(new Pen(Color.Lime), m_arrFilteredPoints[i].X * fScaleX + 5, m_arrFilteredPoints[i].Y * fScaleY - 5, m_arrFilteredPoints[i].X * fScaleX - 5, m_arrFilteredPoints[i].Y * fScaleY + 5);
                }
            }

            if (m_blnDrawTexture)
            {
                if (m_intROIDirection == 1)
                {
                    float intStartY = m_objLine.GetPointY(m_intStartX);
                    float intEndY = m_objLine.GetPointY(m_intStartX + m_intWidthLimit);
                    if (!float.IsNaN(intStartY) && !float.IsInfinity(intStartY) && !float.IsNaN(intEndY) && !float.IsInfinity(intEndY) && intStartY < g.ClipBounds.Height && intEndY < g.ClipBounds.Height)
                    {
                        g.DrawLine(new Pen(Color.Red), m_intStartX * fScaleX, (intStartY + intOffset) * fScaleY, (m_intStartX + m_intWidthLimit) * fScaleX, (intEndY + intOffset) * fScaleY);
                        g.DrawLine(new Pen(Color.Red), m_intStartX * fScaleX, (intStartY + intOffset + intMaskThickness) * fScaleY, (m_intStartX + m_intWidthLimit) * fScaleX, (intEndY + intOffset + intMaskThickness) * fScaleY);
                        g.FillRectangle(objTextureBrush, m_intStartX * fScaleX, (intStartY + intOffset) * fScaleY, m_intWidthLimit * fScaleX, intMaskThickness * fScaleY);
                    }
                }
                else if (m_intROIDirection == 2)
                {
                    float intStartX = m_objLine.GetPointX(m_intStartY);
                    float intEndX = m_objLine.GetPointX(m_intStartY + m_intHeightLimit);
                    if (!float.IsNaN(intStartX) && !float.IsInfinity(intStartX) && !float.IsNaN(intEndX) && !float.IsInfinity(intEndX) && intStartX < g.ClipBounds.Width && intEndX < g.ClipBounds.Width)
                    {
                        g.DrawLine(new Pen(Color.Red), (intStartX - intOffset) * fScaleX, m_intStartY * fScaleY, (intEndX - intOffset) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                        g.DrawLine(new Pen(Color.Red), (intStartX - intOffset - intMaskThickness) * fScaleX, m_intStartY * fScaleY, (intEndX - intOffset - intMaskThickness) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                        g.FillRectangle(objTextureBrush, (intEndX - intOffset - intMaskThickness) * fScaleX, m_intStartY * fScaleY, intMaskThickness * fScaleX, m_intHeightLimit * fScaleY);
                    }
                }
                else if (m_intROIDirection == 3)
                {
                    float intStartY = m_objLine.GetPointY(m_intStartX);
                    float intEndY = m_objLine.GetPointY(m_intStartX + m_intWidthLimit);
                    if (!float.IsNaN(intStartY) && !float.IsInfinity(intStartY) && !float.IsNaN(intEndY) && !float.IsInfinity(intEndY) && intStartY < g.ClipBounds.Height && intEndY < g.ClipBounds.Height)
                    {
                        g.DrawLine(new Pen(Color.Red), m_intStartX * fScaleX, (intStartY - intOffset) * fScaleY, (m_intStartX + m_intWidthLimit) * fScaleX, (intEndY - intOffset) * fScaleY);
                        g.DrawLine(new Pen(Color.Red), m_intStartX * fScaleX, (intStartY - intOffset - intMaskThickness) * fScaleY, (m_intStartX + m_intWidthLimit) * fScaleX, (intEndY - intOffset - intMaskThickness) * fScaleY);
                        g.FillRectangle(objTextureBrush, m_intStartX * fScaleX, (intStartY - intOffset - intMaskThickness) * fScaleY, m_intWidthLimit * fScaleX, intMaskThickness * fScaleY);
                    }
                }
                else if (m_intROIDirection == 4)
                {
                    float intStartX = m_objLine.GetPointX(m_intStartY);
                    float intEndX = m_objLine.GetPointX(m_intStartY + m_intHeightLimit);
                    if (!float.IsNaN(intStartX) && !float.IsInfinity(intStartX) && !float.IsNaN(intEndX) && !float.IsInfinity(intEndX) && intStartX < g.ClipBounds.Width && intEndX < g.ClipBounds.Width)
                    {
                        g.DrawLine(new Pen(Color.Red), (intStartX + intOffset) * fScaleX, m_intStartY * fScaleY, (intEndX + intOffset) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                        g.DrawLine(new Pen(Color.Red), (intStartX + intOffset + intMaskThickness) * fScaleX, m_intStartY * fScaleY, (intEndX + intOffset + intMaskThickness) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                        g.FillRectangle(objTextureBrush, (intStartX + intOffset) * fScaleX, m_intStartY * fScaleY, intMaskThickness * fScaleX, m_intHeightLimit * fScaleY);
                    }
                }
            }

        }
        public void DrawPGauge(Graphics g, float fScaleX, float fScaleY, int intOffset, int intMaskThickness, TextureBrush objTextureBrush, bool blnDrawTexture, float fAngle, PointF pCenter)
        {
            if (blnDrawTexture)
            {
               
                if (m_intROIDirection == 1)
                {
                    float intStartY = m_objLine.GetPointY(m_intStartX);
                    float intEndY = m_objLine.GetPointY(m_intStartX + m_intWidthLimit);
                    if (!float.IsNaN(intStartY) && !float.IsInfinity(intStartY) && !float.IsNaN(intEndY) && !float.IsInfinity(intEndY) && intStartY < g.ClipBounds.Height && intEndY < g.ClipBounds.Height)
                    {
                        //g.DrawLine(new Pen(Color.Red), m_intStartX * fScaleX, (intStartY + intOffset) * fScaleY, (m_intStartX + m_intWidthLimit) * fScaleX, (intEndY + intOffset) * fScaleY);
                        //g.DrawLine(new Pen(Color.Red), m_intStartX * fScaleX, (intStartY + intOffset + intMaskThickness) * fScaleY, (m_intStartX + m_intWidthLimit) * fScaleX, (intEndY + intOffset + intMaskThickness) * fScaleY);
                        //g.FillRectangle(objTextureBrush, m_intStartX * fScaleX, (intStartY + intOffset) * fScaleY, m_intWidthLimit * fScaleX, intMaskThickness * fScaleY);
                        PointF pStart = new PointF(m_intStartX * fScaleX, (intStartY + intOffset) * fScaleY);
                        PointF pEnd = new PointF((m_intStartX + m_intWidthLimit) * fScaleX, (intStartY + intOffset + intMaskThickness) * fScaleY);
                        List<PointF> arrPoints = new List<PointF>(); // 0:Top Left, 1: Top Right, 2:Bottom Right, 3:Bottom Left
                        arrPoints.Add(new PointF(pStart.X, pStart.Y));
                        arrPoints.Add(new PointF(pEnd.X, pStart.Y));
                        arrPoints.Add(new PointF(pEnd.X, pEnd.Y));
                        arrPoints.Add(new PointF(pStart.X, pEnd.Y));
                        
                        PointF pTemp = new PointF();
                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[0],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[0] = pTemp;

                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[1],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[1] = pTemp;

                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[2],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[2] = pTemp;

                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[3],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[3] = pTemp;

                        g.DrawLine(new Pen(Color.Red), arrPoints[0], arrPoints[1]);
                        g.DrawLine(new Pen(Color.Red), arrPoints[1], arrPoints[2]);
                        g.DrawLine(new Pen(Color.Red), arrPoints[2], arrPoints[3]);
                        g.DrawLine(new Pen(Color.Red), arrPoints[3], arrPoints[0]);
                        g.FillPolygon(objTextureBrush, arrPoints.ToArray());
                    }
                }
                else if (m_intROIDirection == 2)
                {
                    float intStartX = m_objLine.GetPointX(m_intStartY);
                    float intEndX = m_objLine.GetPointX(m_intStartY + m_intHeightLimit);
                    if (!float.IsNaN(intStartX) && !float.IsInfinity(intStartX) && !float.IsNaN(intEndX) && !float.IsInfinity(intEndX) && intStartX < g.ClipBounds.Width && intEndX < g.ClipBounds.Width)
                    {
                        //g.DrawLine(new Pen(Color.Red), (intStartX - intOffset) * fScaleX, m_intStartY * fScaleY, (intEndX - intOffset) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                        //g.DrawLine(new Pen(Color.Red), (intStartX - intOffset - intMaskThickness) * fScaleX, m_intStartY * fScaleY, (intEndX - intOffset - intMaskThickness) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                        //g.FillRectangle(objTextureBrush, (intEndX - intOffset - intMaskThickness) * fScaleX, m_intStartY * fScaleY, intMaskThickness * fScaleX, m_intHeightLimit * fScaleY);
                        PointF pStart = new PointF((intEndX - intOffset - intMaskThickness) * fScaleX, m_intStartY * fScaleY);
                        PointF pEnd = new PointF((intEndX - intOffset - intMaskThickness + intMaskThickness) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                        List<PointF> arrPoints = new List<PointF>(); // 0:Top Left, 1: Top Right, 2:Bottom Right, 3:Bottom Left
                        arrPoints.Add(new PointF(pStart.X, pStart.Y));
                        arrPoints.Add(new PointF(pEnd.X, pStart.Y));
                        arrPoints.Add(new PointF(pEnd.X, pEnd.Y));
                        arrPoints.Add(new PointF(pStart.X, pEnd.Y));

                        PointF pTemp = new PointF();
                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[0],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[0] = pTemp;

                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[1],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[1] = pTemp;

                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[2],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[2] = pTemp;

                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[3],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[3] = pTemp;

                        g.DrawLine(new Pen(Color.Red), arrPoints[0], arrPoints[1]);
                        g.DrawLine(new Pen(Color.Red), arrPoints[1], arrPoints[2]);
                        g.DrawLine(new Pen(Color.Red), arrPoints[2], arrPoints[3]);
                        g.DrawLine(new Pen(Color.Red), arrPoints[3], arrPoints[0]);
                        g.FillPolygon(objTextureBrush, arrPoints.ToArray());
                    }
                }
                else if (m_intROIDirection == 3)
                {
                    float intStartY = m_objLine.GetPointY(m_intStartX);
                    float intEndY = m_objLine.GetPointY(m_intStartX + m_intWidthLimit);
                    if (!float.IsNaN(intStartY) && !float.IsInfinity(intStartY) && !float.IsNaN(intEndY) && !float.IsInfinity(intEndY) && intStartY < g.ClipBounds.Height && intEndY < g.ClipBounds.Height)
                    {
                        //g.DrawLine(new Pen(Color.Red), m_intStartX * fScaleX, (intStartY - intOffset) * fScaleY, (m_intStartX + m_intWidthLimit) * fScaleX, (intEndY - intOffset) * fScaleY);
                        //g.DrawLine(new Pen(Color.Red), m_intStartX * fScaleX, (intStartY - intOffset - intMaskThickness) * fScaleY, (m_intStartX + m_intWidthLimit) * fScaleX, (intEndY - intOffset - intMaskThickness) * fScaleY);
                        //g.FillRectangle(objTextureBrush, m_intStartX * fScaleX, (intStartY - intOffset - intMaskThickness) * fScaleY, m_intWidthLimit * fScaleX, intMaskThickness * fScaleY);
                        PointF pStart = new PointF(m_intStartX * fScaleX, (intStartY - intOffset - intMaskThickness) * fScaleY);
                        PointF pEnd = new PointF((m_intStartX + m_intWidthLimit) * fScaleX, (intStartY - intOffset - intMaskThickness + intMaskThickness) * fScaleY);
                        List<PointF> arrPoints = new List<PointF>(); // 0:Top Left, 1: Top Right, 2:Bottom Right, 3:Bottom Left
                        arrPoints.Add(new PointF(pStart.X, pStart.Y));
                        arrPoints.Add(new PointF(pEnd.X, pStart.Y));
                        arrPoints.Add(new PointF(pEnd.X, pEnd.Y));
                        arrPoints.Add(new PointF(pStart.X, pEnd.Y));

                        PointF pTemp = new PointF();
                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[0],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[0] = pTemp;

                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[1],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[1] = pTemp;

                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[2],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[2] = pTemp;

                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[3],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[3] = pTemp;

                        g.DrawLine(new Pen(Color.Red), arrPoints[0], arrPoints[1]);
                        g.DrawLine(new Pen(Color.Red), arrPoints[1], arrPoints[2]);
                        g.DrawLine(new Pen(Color.Red), arrPoints[2], arrPoints[3]);
                        g.DrawLine(new Pen(Color.Red), arrPoints[3], arrPoints[0]);
                        g.FillPolygon(objTextureBrush, arrPoints.ToArray());
                    }
                }
                else if (m_intROIDirection == 4)
                {
                    float intStartX = m_objLine.GetPointX(m_intStartY);
                    float intEndX = m_objLine.GetPointX(m_intStartY + m_intHeightLimit);
                    if (!float.IsNaN(intStartX) && !float.IsInfinity(intStartX) && !float.IsNaN(intEndX) && !float.IsInfinity(intEndX) && intStartX < g.ClipBounds.Width && intEndX < g.ClipBounds.Width)
                    {
                        //g.DrawLine(new Pen(Color.Red), (intStartX + intOffset) * fScaleX, m_intStartY * fScaleY, (intEndX + intOffset) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                        //g.DrawLine(new Pen(Color.Red), (intStartX + intOffset + intMaskThickness) * fScaleX, m_intStartY * fScaleY, (intEndX + intOffset + intMaskThickness) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                        //g.FillRectangle(objTextureBrush, (intStartX + intOffset) * fScaleX, m_intStartY * fScaleY, intMaskThickness * fScaleX, m_intHeightLimit * fScaleY);
                        PointF pStart = new PointF((intStartX + intOffset) * fScaleX, m_intStartY * fScaleY);
                        PointF pEnd = new PointF((intStartX + intOffset + intMaskThickness) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                        List<PointF> arrPoints = new List<PointF>(); // 0:Top Left, 1: Top Right, 2:Bottom Right, 3:Bottom Left
                        arrPoints.Add(new PointF(pStart.X, pStart.Y));
                        arrPoints.Add(new PointF(pEnd.X, pStart.Y));
                        arrPoints.Add(new PointF(pEnd.X, pEnd.Y));
                        arrPoints.Add(new PointF(pStart.X, pEnd.Y));

                        PointF pTemp = new PointF();
                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[0],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[0] = pTemp;

                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[1],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[1] = pTemp;

                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[2],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[2] = pTemp;

                        Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                         pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                         arrPoints[3],
                                                         fAngle,
                                                         ref pTemp);
                        arrPoints[3] = pTemp;

                        g.DrawLine(new Pen(Color.Red), arrPoints[0], arrPoints[1]);
                        g.DrawLine(new Pen(Color.Red), arrPoints[1], arrPoints[2]);
                        g.DrawLine(new Pen(Color.Red), arrPoints[2], arrPoints[3]);
                        g.DrawLine(new Pen(Color.Red), arrPoints[3], arrPoints[0]);
                        g.FillPolygon(objTextureBrush, arrPoints.ToArray());
                    }
                }
            }

        }
        public void DrawPGauge(Graphics g, float fScaleX, float fScaleY, int intOffset, int intMaskThickness, float fAngle, PointF pCenter)
        {
            if (m_intROIDirection == 1)
            {
                float intStartY = m_objLine.GetPointY(m_intStartX);
                float intEndY = m_objLine.GetPointY(m_intStartX + m_intWidthLimit);
                if (!float.IsNaN(intStartY) && !float.IsInfinity(intStartY) && !float.IsNaN(intEndY) && !float.IsInfinity(intEndY) && intStartY < g.ClipBounds.Height && intEndY < g.ClipBounds.Height)
                {
                    //g.DrawLine(new Pen(Color.Red), m_intStartX * fScaleX, (intStartY + intOffset) * fScaleY, (m_intStartX + m_intWidthLimit) * fScaleX, (intEndY + intOffset) * fScaleY);
                    //g.DrawLine(new Pen(Color.Red), m_intStartX * fScaleX, (intStartY + intOffset + intMaskThickness) * fScaleY, (m_intStartX + m_intWidthLimit) * fScaleX, (intEndY + intOffset + intMaskThickness) * fScaleY);
                    //g.FillRectangle(objTextureBrush, m_intStartX * fScaleX, (intStartY + intOffset) * fScaleY, m_intWidthLimit * fScaleX, intMaskThickness * fScaleY);
                    PointF pStart = new PointF(m_intStartX * fScaleX, (intStartY + intOffset) * fScaleY);
                    PointF pEnd = new PointF((m_intStartX + m_intWidthLimit) * fScaleX, (intStartY + intOffset + intMaskThickness) * fScaleY);
                    List<PointF> arrPoints = new List<PointF>(); // 0:Top Left, 1: Top Right, 2:Bottom Right, 3:Bottom Left
                    arrPoints.Add(new PointF(pStart.X, pStart.Y));
                    arrPoints.Add(new PointF(pEnd.X, pStart.Y));
                    arrPoints.Add(new PointF(pEnd.X, pEnd.Y));
                    arrPoints.Add(new PointF(pStart.X, pEnd.Y));

                    PointF pTemp = new PointF();
                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[0],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[0] = pTemp;

                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[1],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[1] = pTemp;

                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[2],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[2] = pTemp;

                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[3],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[3] = pTemp;

                    g.DrawLine(new Pen(Color.LightGray), arrPoints[0], arrPoints[1]);
                    g.DrawLine(new Pen(Color.LightGray), arrPoints[1], arrPoints[2]);
                    g.DrawLine(new Pen(Color.LightGray), arrPoints[2], arrPoints[3]);
                    g.DrawLine(new Pen(Color.LightGray), arrPoints[3], arrPoints[0]);
                }
            }
            else if (m_intROIDirection == 2)
            {
                float intStartX = m_objLine.GetPointX(m_intStartY);
                float intEndX = m_objLine.GetPointX(m_intStartY + m_intHeightLimit);
                if (!float.IsNaN(intStartX) && !float.IsInfinity(intStartX) && !float.IsNaN(intEndX) && !float.IsInfinity(intEndX) && intStartX < g.ClipBounds.Width && intEndX < g.ClipBounds.Width)
                {
                    //g.DrawLine(new Pen(Color.Red), (intStartX - intOffset) * fScaleX, m_intStartY * fScaleY, (intEndX - intOffset) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                    //g.DrawLine(new Pen(Color.Red), (intStartX - intOffset - intMaskThickness) * fScaleX, m_intStartY * fScaleY, (intEndX - intOffset - intMaskThickness) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                    //g.FillRectangle(objTextureBrush, (intEndX - intOffset - intMaskThickness) * fScaleX, m_intStartY * fScaleY, intMaskThickness * fScaleX, m_intHeightLimit * fScaleY);
                    PointF pStart = new PointF((intEndX - intOffset - intMaskThickness) * fScaleX, m_intStartY * fScaleY);
                    PointF pEnd = new PointF((intEndX - intOffset - intMaskThickness + intMaskThickness) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                    List<PointF> arrPoints = new List<PointF>(); // 0:Top Left, 1: Top Right, 2:Bottom Right, 3:Bottom Left
                    arrPoints.Add(new PointF(pStart.X, pStart.Y));
                    arrPoints.Add(new PointF(pEnd.X, pStart.Y));
                    arrPoints.Add(new PointF(pEnd.X, pEnd.Y));
                    arrPoints.Add(new PointF(pStart.X, pEnd.Y));

                    PointF pTemp = new PointF();
                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[0],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[0] = pTemp;

                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[1],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[1] = pTemp;

                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[2],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[2] = pTemp;

                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[3],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[3] = pTemp;

                    g.DrawLine(new Pen(Color.LightGray), arrPoints[0], arrPoints[1]);
                    g.DrawLine(new Pen(Color.LightGray), arrPoints[1], arrPoints[2]);
                    g.DrawLine(new Pen(Color.LightGray), arrPoints[2], arrPoints[3]);
                    g.DrawLine(new Pen(Color.LightGray), arrPoints[3], arrPoints[0]);
                }
            }
            else if (m_intROIDirection == 3)
            {
                float intStartY = m_objLine.GetPointY(m_intStartX);
                float intEndY = m_objLine.GetPointY(m_intStartX + m_intWidthLimit);
                if (!float.IsNaN(intStartY) && !float.IsInfinity(intStartY) && !float.IsNaN(intEndY) && !float.IsInfinity(intEndY) && intStartY < g.ClipBounds.Height && intEndY < g.ClipBounds.Height)
                {
                    //g.DrawLine(new Pen(Color.Red), m_intStartX * fScaleX, (intStartY - intOffset) * fScaleY, (m_intStartX + m_intWidthLimit) * fScaleX, (intEndY - intOffset) * fScaleY);
                    //g.DrawLine(new Pen(Color.Red), m_intStartX * fScaleX, (intStartY - intOffset - intMaskThickness) * fScaleY, (m_intStartX + m_intWidthLimit) * fScaleX, (intEndY - intOffset - intMaskThickness) * fScaleY);
                    //g.FillRectangle(objTextureBrush, m_intStartX * fScaleX, (intStartY - intOffset - intMaskThickness) * fScaleY, m_intWidthLimit * fScaleX, intMaskThickness * fScaleY);
                    PointF pStart = new PointF(m_intStartX * fScaleX, (intStartY - intOffset - intMaskThickness) * fScaleY);
                    PointF pEnd = new PointF((m_intStartX + m_intWidthLimit) * fScaleX, (intStartY - intOffset - intMaskThickness + intMaskThickness) * fScaleY);
                    List<PointF> arrPoints = new List<PointF>(); // 0:Top Left, 1: Top Right, 2:Bottom Right, 3:Bottom Left
                    arrPoints.Add(new PointF(pStart.X, pStart.Y));
                    arrPoints.Add(new PointF(pEnd.X, pStart.Y));
                    arrPoints.Add(new PointF(pEnd.X, pEnd.Y));
                    arrPoints.Add(new PointF(pStart.X, pEnd.Y));

                    PointF pTemp = new PointF();
                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[0],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[0] = pTemp;

                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[1],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[1] = pTemp;

                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[2],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[2] = pTemp;

                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[3],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[3] = pTemp;

                    g.DrawLine(new Pen(Color.LightGray), arrPoints[0], arrPoints[1]);
                    g.DrawLine(new Pen(Color.LightGray), arrPoints[1], arrPoints[2]);
                    g.DrawLine(new Pen(Color.LightGray), arrPoints[2], arrPoints[3]);
                    g.DrawLine(new Pen(Color.LightGray), arrPoints[3], arrPoints[0]);
                }
            }
            else if (m_intROIDirection == 4)
            {
                float intStartX = m_objLine.GetPointX(m_intStartY);
                float intEndX = m_objLine.GetPointX(m_intStartY + m_intHeightLimit);
                if (!float.IsNaN(intStartX) && !float.IsInfinity(intStartX) && !float.IsNaN(intEndX) && !float.IsInfinity(intEndX) && intStartX < g.ClipBounds.Width && intEndX < g.ClipBounds.Width)
                {
                    //g.DrawLine(new Pen(Color.Red), (intStartX + intOffset) * fScaleX, m_intStartY * fScaleY, (intEndX + intOffset) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                    //g.DrawLine(new Pen(Color.Red), (intStartX + intOffset + intMaskThickness) * fScaleX, m_intStartY * fScaleY, (intEndX + intOffset + intMaskThickness) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                    //g.FillRectangle(objTextureBrush, (intStartX + intOffset) * fScaleX, m_intStartY * fScaleY, intMaskThickness * fScaleX, m_intHeightLimit * fScaleY);
                    PointF pStart = new PointF((intStartX + intOffset) * fScaleX, m_intStartY * fScaleY);
                    PointF pEnd = new PointF((intStartX + intOffset + intMaskThickness) * fScaleX, (m_intStartY + m_intHeightLimit) * fScaleY);
                    List<PointF> arrPoints = new List<PointF>(); // 0:Top Left, 1: Top Right, 2:Bottom Right, 3:Bottom Left
                    arrPoints.Add(new PointF(pStart.X, pStart.Y));
                    arrPoints.Add(new PointF(pEnd.X, pStart.Y));
                    arrPoints.Add(new PointF(pEnd.X, pEnd.Y));
                    arrPoints.Add(new PointF(pStart.X, pEnd.Y));

                    PointF pTemp = new PointF();
                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[0],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[0] = pTemp;

                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[1],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[1] = pTemp;

                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[2],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[2] = pTemp;

                    Math2.GetNewXYAfterRotate_360deg(pCenter.X * fScaleX, //(pEnd.X + pStart.X) / 2,
                                                     pCenter.Y * fScaleY, //(pEnd.Y + pStart.Y) / 2,
                                                     arrPoints[3],
                                                     fAngle,
                                                     ref pTemp);
                    arrPoints[3] = pTemp;

                    g.DrawLine(new Pen(Color.LightGray), arrPoints[0], arrPoints[1]);
                    g.DrawLine(new Pen(Color.LightGray), arrPoints[1], arrPoints[2]);
                    g.DrawLine(new Pen(Color.LightGray), arrPoints[2], arrPoints[3]);
                    g.DrawLine(new Pen(Color.LightGray), arrPoints[3], arrPoints[0]);
                }
            }
        }
    }
}
