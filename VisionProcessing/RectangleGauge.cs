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
    public class RectGauge
    {
        #region Member Variables
        private bool m_blnDrawSampledPoints = true;
        private bool m_blnDrawDraggingBox = true;

        private ERectangleGauge m_RectGauge = new ERectangleGauge();

        //mouse location
        private PointF m_pfCurrMouse = PointF.Empty;

        //rect gauge size and location
        private float m_fSizeTolerance = 0;
        private float m_fGainValue = 1000f;

        //rect gauge setting
        private int m_intThickness;
        private int m_intThreshold;
        private int m_intFilteringThreshold;
        private int m_intMinAmplitude;
        private int m_intMinArea;

        // rect gauge template center location
        private float m_fTemplateObjectCenterX = 0;
        private float m_fTemplateObjectCenterY = 0;
        private float m_fTemplateObjectWidth = 0;
        private float m_fTemplateObjectHeight = 0;

        //rect pen properties
        private ERGBColor m_colorRectGNomial = new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B);
        private ERGBColor m_colorRectGSampPoints = new ERGBColor(Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B);

        //font properties
        private Font m_FontMatched = new Font("Tahoma", 8);
        private SolidBrush m_BrushMatched = new SolidBrush(Color.GreenYellow);


        #endregion

        #region --------- Rect Gauge Properties -------------------
        public bool ref_blnDrawSamplingPoint { get { return m_blnDrawSampledPoints; } set { m_blnDrawSampledPoints = value; } }
        public bool ref_blnDrawDraggingBox { get { return m_blnDrawDraggingBox; } set { m_blnDrawDraggingBox = value; } }

        //return object result
        public float ref_ObjectWidth { get { return m_RectGauge.MeasuredRectangle.SizeX; } }
        public float ref_ObjectHeight { get { return m_RectGauge.MeasuredRectangle.SizeY; } }
        public float ref_ObjectCenterX { get { return m_RectGauge.MeasuredRectangle.CenterX; } }
        public float ref_ObjectCenterY { get { return m_RectGauge.MeasuredRectangle.CenterY; } }
        public float ref_ObjectAngle { get { return m_RectGauge.MeasuredRectangle.Angle; } }
        
        // return gauge template center location
        public float ref_TemplateObjectCenterX { get { return m_fTemplateObjectCenterX; } set { m_fTemplateObjectCenterX = value; } }
        public float ref_TemplateObjectCenterY { get { return m_fTemplateObjectCenterY; } set { m_fTemplateObjectCenterY = value; } }
        public float ref_TemplateObjectWidth { get { return m_fTemplateObjectWidth; } set { m_fTemplateObjectWidth = value; } }
        public float ref_TemplateObjectHeight { get { return m_fTemplateObjectHeight; } set { m_fTemplateObjectHeight = value; } }

        //return gauge setting
        public float ref_GaugeCenterX
        {
            get { return m_RectGauge.Center.X; }
            set
            {
                m_RectGauge.SetCenterXY(value, m_RectGauge.Center.Y);
            }
        }
        public float ref_GaugeCenterY
        {
            get { return m_RectGauge.Center.Y; }
            set
            {
                m_RectGauge.SetCenterXY(m_RectGauge.Center.X, value);
            }
        }
        public float ref_GaugeWidth
        {
            get
            {
                return m_RectGauge.SizeX;
            }
            set
            {
                m_RectGauge.SetSize(value, m_RectGauge.SizeY);
            }
        }
        public float ref_GaugeHeight
        {
            get
            {
                return m_RectGauge.SizeY;
            }
            set
            {
                m_RectGauge.SetSize(m_RectGauge.SizeX, value);
            }
        }
        public float ref_GaugeAngle { get { return m_RectGauge.Angle; } set { m_RectGauge.Angle = value; } }

        public float ref_GaugeSizeTolerance { get { return m_fSizeTolerance; } set { m_fSizeTolerance = value; } }
        public float ref_fGainValue { get { return m_fGainValue; } set { m_fGainValue = value; } }
        public float ref_GaugeTolerance { get { return m_RectGauge.Tolerance; } set { m_RectGauge.Tolerance = value; } }
        public float ref_GaugeFilterThreshold { get { return m_RectGauge.FilteringThreshold; } set { m_RectGauge.FilteringThreshold = value; } }
#if (Debug_2_12 || Release_2_12)
        public int ref_GaugeFilteringPasses { get { return (int)m_RectGauge.NumFilteringPasses; } set { m_RectGauge.NumFilteringPasses = (uint)value; } }
        public float ref_GaugeSamplingStep { get { return m_RectGauge.SamplingStep; } set { m_RectGauge.SamplingStep = value; } }
        public int ref_GaugeThickness { get { return (int)m_RectGauge.Thickness; } set { m_RectGauge.Thickness = (uint)value; } }
        public int ref_GaugeThreshold { get { return (int)m_RectGauge.Threshold; } set { m_RectGauge.Threshold = (uint)value; } }
        public int ref_GaugeMinAmplitude { get { return (int)m_RectGauge.MinAmplitude; } set { m_RectGauge.MinAmplitude = (uint)value; } }
        public int ref_GaugeMinArea { get { return (int)m_RectGauge.MinArea; } set { m_RectGauge.MinArea = (uint)value; } }
        public int ref_GaugeFilter { get { return (int)m_RectGauge.Smoothing; } set { m_RectGauge.Smoothing = (uint)value; } } // Note* Smooting is for Filter, FilteringThreshold is not for Filter

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
        public int ref_GaugeFilteringPasses { get { return m_RectGauge.NumFilteringPasses; } set { m_RectGauge.NumFilteringPasses = value; } }
        public float ref_GaugeSamplingStep { get { return m_RectGauge.SamplingStep; } set { m_RectGauge.SamplingStep = value; } }
        public int ref_GaugeThickness { get { return m_RectGauge.Thickness; } set { m_RectGauge.Thickness = value; } }
        public int ref_GaugeThreshold { get { return m_RectGauge.Threshold; } set { m_RectGauge.Threshold = value; } }
        public int ref_GaugeMinAmplitude { get { return m_RectGauge.MinAmplitude; } set { m_RectGauge.MinAmplitude = value; } }
        public int ref_GaugeMinArea { get { return m_RectGauge.MinArea; } set { m_RectGauge.MinArea = value; } }
        public int ref_GaugeFilter { get { return m_RectGauge.Smoothing; } set { m_RectGauge.Smoothing = value; } } // Note* Smooting is for Filter, FilteringThreshold is not for Filter

#endif

        public int ref_GaugeTransType { get { return m_RectGauge.TransitionType.GetHashCode(); } set { m_RectGauge.TransitionType = (ETransitionType)value; } }
        public int ref_GaugeTransChoice { get { return m_RectGauge.TransitionChoice.GetHashCode(); } set { m_RectGauge.TransitionChoice = (ETransitionChoice)value; } }

        //return gauge pointer
        public ERectangleGauge ref_RectGauge { get { return m_RectGauge; } }
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="roiSource">source ROI</param>
        /// <param name="g">graphics tool of pic box</param>



        public RectGauge()
        {
            m_RectGauge.Angle = 0;

            //set gauge dragable,resizeable,rotatable
            m_RectGauge.Dragable = false;
            m_RectGauge.Rotatable = false;
            m_RectGauge.Resizable = false;
        }

        public RectGauge(GaugeWorldShape objWorldShape)
        {
            m_RectGauge.Attach(objWorldShape.ref_objWorldShape);

            m_RectGauge.Angle = 0;

            //set gauge dragable,resizeable,rotatable
            m_RectGauge.Dragable = false;
            m_RectGauge.Rotatable = false;
            m_RectGauge.Resizable = false;
        }

        public void CopyTo(ref RectGauge objDestRectGauge)
        {
            objDestRectGauge.m_fTemplateObjectCenterX = m_fTemplateObjectCenterX;
            objDestRectGauge.m_fTemplateObjectCenterY = m_fTemplateObjectCenterY;
            objDestRectGauge.m_fTemplateObjectWidth = m_fTemplateObjectWidth;
            objDestRectGauge.m_fTemplateObjectHeight = m_fTemplateObjectHeight;

            objDestRectGauge.ref_RectGauge.SetCenterXY(m_RectGauge.Center.X,
                                                     m_RectGauge.Center.Y);

            objDestRectGauge.ref_RectGauge.SetSize(m_RectGauge.SizeX,
                                                   m_RectGauge.SizeY);

            objDestRectGauge.ref_RectGauge.Angle = m_RectGauge.Angle;
            objDestRectGauge.ref_RectGauge.Tolerance = m_RectGauge.Tolerance;

            objDestRectGauge.ref_RectGauge.TransitionType = m_RectGauge.TransitionType;
            objDestRectGauge.ref_RectGauge.TransitionChoice = m_RectGauge.TransitionChoice;
            objDestRectGauge.ref_RectGauge.Threshold = m_RectGauge.Threshold;
            objDestRectGauge.ref_RectGauge.Thickness = m_RectGauge.Thickness;
            objDestRectGauge.ref_RectGauge.MinAmplitude = m_RectGauge.MinAmplitude;
            objDestRectGauge.ref_RectGauge.MinArea = m_RectGauge.MinArea;
            objDestRectGauge.ref_RectGauge.Smoothing = m_RectGauge.Smoothing;

            objDestRectGauge.ref_RectGauge.SamplingStep = m_RectGauge.SamplingStep;
            objDestRectGauge.ref_RectGauge.FilteringThreshold = m_RectGauge.FilteringThreshold;
            objDestRectGauge.ref_RectGauge.NumFilteringPasses = m_RectGauge.NumFilteringPasses;

            objDestRectGauge.ref_GaugeSizeTolerance = m_fSizeTolerance;
            objDestRectGauge.ref_fGainValue = m_fGainValue;

        }


        /// <summary>
        /// Draw rect gauge on image attach to specify ROI center
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        public void DrawRectGauge(Graphics g)
        {
            if (m_RectGauge == null)
                return;

            m_RectGauge.Draw(g, m_colorRectGNomial, EDrawingMode.Nominal, true);
            m_RectGauge.Draw(g, m_colorRectGSampPoints, EDrawingMode.SampledPoints, true);

            //draw string on gauge box
            g.DrawString
                (m_RectGauge.MeasuredRectangle.Angle.ToString("0.000"), m_FontMatched,
                m_BrushMatched,
                m_RectGauge.MeasuredRectangle.CenterX,
                m_RectGauge.MeasuredRectangle.CenterY);
        }

        /// <summary>
        /// Draw rect gauge on image 
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        public void DrawGaugeResult(Graphics g)
        {
            if (m_RectGauge == null)
                return;

            //draw line result
            if (m_blnDrawSampledPoints)
                m_RectGauge.Draw(g, m_colorRectGSampPoints, EDrawingMode.SampledPoints, true);
            if (m_blnDrawDraggingBox)
                m_RectGauge.Draw(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), EDrawingMode.Nominal, true);

            int intCenterX = Convert.ToInt32(m_RectGauge.MeasuredRectangle.CenterX);
            int intCenterY = Convert.ToInt32(m_RectGauge.MeasuredRectangle.CenterY);

            //draw string on gauge box
            g.DrawString
                (m_RectGauge.MeasuredRectangle.Angle.ToString("0.000"), m_FontMatched,
                m_BrushMatched, m_RectGauge.MeasuredRectangle.CenterX,
                m_RectGauge.MeasuredRectangle.CenterY);

            g.DrawLine(new Pen(m_BrushMatched, 2), new System.Drawing.Point(intCenterX - 5, intCenterY - 5),
                 new System.Drawing.Point(intCenterX + 5, intCenterY + 5));
            g.DrawLine(new Pen(m_BrushMatched, 2), new System.Drawing.Point(intCenterX + 5, intCenterY - 5),
                 new System.Drawing.Point(intCenterX - 5, intCenterY + 5));
        }

        public void DrawGaugeResult(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            if (m_RectGauge == null)
                return;

            //draw line result
            if (m_blnDrawSampledPoints)
                m_RectGauge.Draw(g, m_colorRectGSampPoints, EDrawingMode.SampledPoints, true);
            if (m_blnDrawDraggingBox)
                m_RectGauge.Draw(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), EDrawingMode.Nominal, true);
            
            int intCenterX = Convert.ToInt32(m_RectGauge.MeasuredRectangle.CenterX * fDrawingScaleX);
            int intCenterY = Convert.ToInt32(m_RectGauge.MeasuredRectangle.CenterY * fDrawingScaleY);

            //draw string on gauge box
            g.DrawString
                (m_RectGauge.MeasuredRectangle.Angle.ToString("0.000"), m_FontMatched,
                m_BrushMatched, intCenterX, intCenterY);

            g.DrawLine(new Pen(m_BrushMatched, 2), new System.Drawing.Point(intCenterX - 5, intCenterY - 5),
                 new System.Drawing.Point(intCenterX + 5, intCenterY + 5));
            g.DrawLine(new Pen(m_BrushMatched, 2), new System.Drawing.Point(intCenterX + 5, intCenterY - 5),
                 new System.Drawing.Point(intCenterX - 5, intCenterY + 5));
        }

        /// <summary>
        /// Get the percentage of valid gauge point over all gauge point
        /// </summary>
        /// <returns>percentage of valid gauge point</returns>
        public float GetGaugePointValidScore()
        {
#if (Debug_2_12 || Release_2_12)
            uint intValidPoints = m_RectGauge.NumValidSamples;
            uint intPoints = m_RectGauge.NumSamples;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            int intValidPoints = m_RectGauge.NumValidSamples;
            int intPoints = m_RectGauge.NumSamples;

#endif

            return (float)intValidPoints / (float)intPoints * 100;
        }
        /// <summary>
        /// Measure object within source ROI region without update display
        /// </summary>
        /// <param name="objROI">ROI</param>    
        /// <returns>rectangle gauge angle</returns>
        public float Measure(ImageDrawing objImage)
        {
            try
            {
                ROI objROI = new ROI();
                objROI.AttachImage(objImage);
                objROI.LoadROISetting(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);

                //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent0.bmp");
                //m_RectGauge.Save("D:\\TS\\RectGauge0.CAL");
                m_RectGauge.Measure(objROI.ref_ROI);
                //m_RectGauge.Measure(objImage.ref_objMainImage);   // Dont measure using Image because the measurement is not stable.

                objROI.Dispose();

                return m_RectGauge.MeasuredRectangle.Angle;
            }
            catch
            {
                return 0;
            }
        }

        public float Measure_1(ImageDrawing objImage)
        {
            try
            {
                ROI objROI = new ROI();
                objROI.AttachImage(objImage);
                objROI.LoadROISetting(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);

                //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent1.bmp");
                //m_RectGauge.Save("D:\\TS\\RectGauge1.CAL");
                m_RectGauge.Measure(objROI.ref_ROI);
                //m_RectGauge.Measure(objImage.ref_objMainImage);   // Dont measure using Image because the measurement is not stable.

                objROI.Dispose();

                return m_RectGauge.MeasuredRectangle.Angle;
            }
            catch
            {
                return 0;
            }
        }

        public float Measure_2(ImageDrawing objImage)
        {
            try
            {
                ROI objROI = new ROI();
                objROI.AttachImage(objImage);
                objROI.LoadROISetting(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);

                //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent2.bmp");
                //m_RectGauge.Save("D:\\TS\\RectGauge2.CAL");
                m_RectGauge.Measure(objROI.ref_ROI);
                //m_RectGauge.Measure(objImage.ref_objMainImage);   // Dont measure using Image because the measurement is not stable.

                objROI.Dispose();

                return m_RectGauge.MeasuredRectangle.Angle;
            }
            catch
            {
                return 0;
            }
        }

        public float Measure_3(ImageDrawing objImage)
        {
            try
            {
                ROI objROI = new ROI();
                objROI.AttachImage(objImage);
                objROI.LoadROISetting(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);

                //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent3.bmp");
                //m_RectGauge.Save("D:\\TS\\RectGauge3.CAL");
                m_RectGauge.Measure(objROI.ref_ROI);
                //m_RectGauge.Measure(objImage.ref_objMainImage);   // Dont measure using Image because the measurement is not stable.

                objROI.Dispose();

                return m_RectGauge.MeasuredRectangle.Angle;
            }
            catch
            {
                return 0;
            }
        }
        /// <summary>
        /// Measure object within source ROI region without update display
        /// </summary>
        /// <param name="objROI">ROI</param>    
        /// <returns>rectangle gauge angle</returns>
        public float Measure(ROI objROI)
        {
            //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent.bmp");
            //m_RectGauge.Save("D:\\TS\\RectGauge.CAL");
            if (objROI.ref_ROI != null && objROI.ref_ROI.Parent != null)
                m_RectGauge.Measure(objROI.ref_ROI);

            return m_RectGauge.MeasuredRectangle.Angle;
        }

        /// <summary>
        /// Measure object within source ROI region without update display
        /// </summary>
        /// <param name="objROI">ROI</param>
        /// <param name="intMaxAngle">max angle</param>
        /// <returns>rectangle gauge angle</returns>
        public float Measure(ROI objROI, int intMaxAngle)
        {
            if (objROI.ref_ROI.TopParent == null)
                return 0;
            //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent.bmp");
            //m_RectGauge.Save("D:\\TS\\RectGauge.CAL");
            m_RectGauge.Measure(objROI.ref_ROI.TopParent);
            if (!VerifyGaugeValid(intMaxAngle))
                return 0.0f;

            return m_RectGauge.MeasuredRectangle.Angle;
        }

        //public float Measure_Clean(ImageDrawing objImage)
        //{
        //    try
        //    {
        //        EROIBW8 objROI = new EROIBW8();
        //        ROI.AttachROI(ref objROI, objImage.ref_objMainImage);
        //        objROI.SetPlacement(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);

        //        ERectangleGauge localRectGauge = new ERectangleGauge();
        //        localRectGauge.SetCenterXY(m_RectGauge.Center.X,
        //                                            m_RectGauge.Center.Y);
        //        localRectGauge.SetSize(m_RectGauge.GetSizeX(),
        //                                               m_RectGauge.GetSizeY());
        //        localRectGauge.Angle = m_RectGauge.Angle;
        //        localRectGauge.Tolerance = m_RectGauge.Tolerance;
        //        localRectGauge.TransitionType = m_RectGauge.TransitionType;
        //        localRectGauge.TransitionChoice = m_RectGauge.TransitionChoice;
        //        localRectGauge.Threshold = m_RectGauge.Threshold;
        //        localRectGauge.Thickness = m_RectGauge.Thickness;
        //        localRectGauge.MinAmplitude = m_RectGauge.MinAmplitude;
        //        localRectGauge.MinArea = m_RectGauge.MinArea;
        //        localRectGauge.Smoothing = m_RectGauge.Smoothing;

        //        localRectGauge.SamplingStep = m_RectGauge.SamplingStep;
        //        localRectGauge.FilteringThreshold = m_RectGauge.FilteringThreshold;
        //        localRectGauge.NumFilteringPasses = m_RectGauge.NumFilteringPasses;

        //        localRectGauge.Measure(objROI);

        //        float fAngle = localRectGauge.MeasuredRectangle.Angle;
        //        localRectGauge.Dispose();
        //        objROI.Dispose();
        //        return fAngle;
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}

        /// <summary>
        /// Measure object within source ROI region with update display
        /// </summary>
        /// <param name="objROI">ROI</param>
        /// <param name="g">Destination to draw image</param>
        public bool MeasureAndDrawGauge(ROI objROI, Graphics g)
        {
            try
            {
                //objROI.ref_ROI.TopParent.Save("D:\\TS\\Pkg_TopParent1.bmp");
                //m_RectGauge.Save("D:\\TS\\Pkg_RectGauge1.CAL");
                m_RectGauge.Measure(objROI.ref_ROI);
                DrawGaugeResult(g);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool MeasureAndDrawGauge(ROI objROI, Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            try
            {
                //objROI.ref_ROI.TopParent.Save("D:\\TS\\Pkg_TopParent1.bmp");
                //m_RectGauge.Save("D:\\TS\\Pkg_RectGauge1.CAL");
                m_RectGauge.Measure(objROI.ref_ROI);
                DrawGaugeResult(g, fDrawingScaleX, fDrawingScaleY);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// When Search ROI is moved or resized, change the Gauge as well
        /// </summary>
        /// <param name="objSearchROI">search ROI</param>
        public void ModifyGauge(ROI objSearchROI)
        {
            /* Old ------------------------------------------
            int intROIWidth = objSearchROI.ref_ROIWidth;
            int intROIHeight = objSearchROI.ref_ROIHeight;

            //will set according to ROI source to place gauge position and size
            //set rect gauge position
            m_RectGauge.SetCenterXY(objSearchROI.ref_ROIPositionX + (intROIWidth / 2f), objSearchROI.ref_ROIPositionY + (intROIHeight / 2f));
            m_RectGauge.SetSize(intROIWidth - (m_RectGauge.Tolerance * 2f) - (intROIWidth * m_fSizeTolerance),
                                              intROIHeight - (m_RectGauge.Tolerance * 2f) - (intROIHeight * m_fSizeTolerance));
            */

            // 2018 12 31 - ZJYeoh: New modify gauge method to prevent gauge measure from outside ROI.
            int intROIWidth = objSearchROI.ref_ROIWidth;
            int intROIHeight = objSearchROI.ref_ROIHeight;
            int intROIX = objSearchROI.ref_ROITotalX;
            int intROIY = objSearchROI.ref_ROITotalY;
            int intOriROIX = objSearchROI.ref_ROIOriPositionX;
            int intOriROIY = objSearchROI.ref_ROIOriPositionY;
            //will set according to ROI source to place gauge position and size
            //set rect gauge position
            int TotalWidth = objSearchROI.ref_ROI.TotalWidth;
            int TotalHeight = objSearchROI.ref_ROI.TotalHeight;

            if (intROIX < 0)
            {
                intROIWidth += intROIX;
                intROIX = 0;
            }
            if (intROIY < 0)
            {
                intROIHeight += intROIY;
                intROIY = 0;
            }

            if (intROIX + intROIWidth > TotalWidth)
                intROIWidth = TotalWidth - intROIX;

            if (intROIY + intROIHeight > TotalHeight)
                intROIHeight = TotalHeight - intROIY;


            if (intOriROIX + intROIWidth > TotalWidth)
                if (intROIWidth - (m_RectGauge.Tolerance * 2f) - (intROIWidth * m_fSizeTolerance) > TotalWidth || intROIWidth > TotalWidth)
                    intROIWidth = TotalWidth;

            if (intOriROIY + intROIHeight > TotalHeight)
                if (intROIHeight - (m_RectGauge.Tolerance * 2f) - (intROIHeight * m_fSizeTolerance) > TotalHeight || intROIHeight > TotalHeight)
                    intROIHeight = TotalHeight;
            
            m_RectGauge.SetCenterXY(intROIX + (intROIWidth / 2f), intROIY + (intROIHeight / 2f));
            m_RectGauge.SetSize(intROIWidth - (m_RectGauge.Tolerance * 2f) - (intROIWidth * m_fSizeTolerance),
                intROIHeight - (m_RectGauge.Tolerance * 2f) - (intROIHeight * m_fSizeTolerance));
        
        }
        
        /// <summary>
        /// Set rect gauge fitting
        /// </summary>
        /// <param name="intSampleStep">interval of pixel to calculate average</param>
        public void SetRectGaugeFitting(int intSampleStep)
        {
            m_RectGauge.SamplingStep = intSampleStep;
        }

        /// <summary>
        /// Set rect gauge measurement setting
        /// </summary>
        /// <param name="rectGTransType">transition type at edge : Black to White(Bw) = 0, White to Black(Wb) = 1, Black to White / White to Black(BwOrWb) = 2, 
        /// Bwb = 3, Wbw = 4</param>
        /// <param name="rectGTransChoice">transition choice at edge : 0 = All, 1 = NthFromBegin, 
        /// 2 = NthFromEnd, 3 = LargestArea, 4 = LargestAmplitude, 5 = Closest</param>
        public void SetRectGaugeMeasurement(int intTrasitionType, int intTransitionChoice)
        {
            //keep setting
            m_RectGauge.TransitionType = (ETransitionType)intTrasitionType;
            m_RectGauge.TransitionChoice = (ETransitionChoice)intTransitionChoice;
        }

        public void SetRectGaugePlacement(float fCenterX, float fCenterY)
        {
            //set immediate action on rect gauge position
            m_RectGauge.SetCenterXY(fCenterX, fCenterY);
        }

        /// <summary>
        /// Set rect gauge placement
        /// </summary>
        /// <param name="objROI">parent to gauge</param>
        /// <param name="fTolerance">tolerance of rect gauge</param>
        /// <param name="fSizeTolerance">size difference between gauge and its parent - ROI</param>
        public void SetRectGaugePlacement(ROI objROI, float fTolerance, float fSizeTolerance)
        {
            int intROIWidth = objROI.ref_ROIWidth;
            int intROIHeight = objROI.ref_ROIHeight;
            int intPositionX = objROI.ref_ROIPositionX;
            int intPositionY = objROI.ref_ROIPositionY;
            m_fSizeTolerance = fSizeTolerance / 100.0f;

            //will set according to ROI source to place gauge position and size       
            //set rect gauge position
            m_RectGauge.SetCenterXY(intPositionX + (intROIWidth / 2f), intPositionY + (intROIHeight / 2f));
            m_RectGauge.SetSize(intROIWidth - (fTolerance * 2f) - (intROIWidth * m_fSizeTolerance), intROIHeight - (fTolerance * 2f) - (intROIHeight * m_fSizeTolerance));
            m_RectGauge.Tolerance = fTolerance;
        }

        /// <summary>
        /// Set rect gauge placement
        /// </summary>
        /// <param name="fCenterX">center X</param>
        /// <param name="fCenterY">center Y</param>
        /// <param name="fWidth">width</param>
        /// <param name="fHeight">height</param>
        /// <param name="fTolerance">tolerance of rect gauge</param>
        /// <param name="intSizeTolerance">size difference between gauge and its parent - ROI<</param>
        public void SetRectGaugePlacement(float fCenterX, float fCenterY, float fWidth, float fHeight,
            float fTolerance, int intSizeTolerance)
        {
            //will set according to pass in dimension
            m_fSizeTolerance = intSizeTolerance / 100.0f;

            //set immediate action on rect gauge position
            m_RectGauge.SetCenterXY(fCenterX, fCenterY);
            m_RectGauge.SetSize(fWidth, fHeight);
            m_RectGauge.Tolerance = fTolerance;
        }

        /// <summary>
        /// Adjust the rectangle gauge size
        /// </summary>
        /// <param name="fSizeTolerance">size difference between gauge and its parent - ROI<</param>
        public void SetRectGaugeSize(ROI objROI, float fSizeTolerance)
        {
            int intROIWidth = objROI.ref_ROIWidth;
            int intROIHeight = objROI.ref_ROIHeight;

            m_fSizeTolerance = fSizeTolerance;

            m_RectGauge.SetSize(intROIWidth - (m_RectGauge.Tolerance * 2f) - (intROIWidth * fSizeTolerance), intROIHeight - (m_RectGauge.Tolerance * 2f) - (intROIHeight * fSizeTolerance));
        }
        
        /// <summary>
        /// Adjust the tolerance range of rectange gauge
        /// </summary>
        /// <param name="objROI">search ROI</param>
        /// <param name="fTolerance">tolerance of rect gauge</param>
        public void SetRectGaugeTolerance(ROI objROI, float fTolerance)
        {
            int intROIWidth = objROI.ref_ROIWidth;
            int intROIHeight = objROI.ref_ROIHeight;

            //set rect gauge position
            m_RectGauge.SetSize(intROIWidth - (fTolerance * 2f) - (intROIWidth * m_fSizeTolerance),
                                              intROIHeight - (fTolerance * 2f) - (intROIHeight * m_fSizeTolerance));
            m_RectGauge.Tolerance = fTolerance;
        }

        /// <summary>
        /// Set rect gauge detection setting
        /// </summary>
        /// <param name="intThickness">edge thickness</param>
        /// <param name="intFilter">edge smoothing</param>
        /// <param name="intThreshold">threshold value to differential the transition edge</param>
        /// <param name="intMinAmp">minimum amplitude of transition</param>
        /// <param name="intMinArea">minimum area under the transition</param>
        public void SetRectGaugeSetting(int intThickness, int intFilter, int intThreshold, int intMinAmp, int intMinArea)
        {
#if (Debug_2_12 || Release_2_12)
            //write setting to gauge
            m_RectGauge.Thickness = (uint)intThickness;
            m_RectGauge.Smoothing = (uint)intFilter;
            m_RectGauge.Threshold = (uint)intThreshold;
            m_RectGauge.MinAmplitude = (uint)intMinAmp;
            m_RectGauge.MinArea = (uint)intMinArea;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            //write setting to gauge
            m_RectGauge.Thickness = intThickness;
            m_RectGauge.Smoothing = intFilter;
            m_RectGauge.Threshold = intThreshold;
            m_RectGauge.MinAmplitude = intMinAmp;
            m_RectGauge.MinArea = intMinArea;
#endif

        }

        /// <summary>
        /// Set rect gauge template center point
        /// </summary>
        /// <param name="fTemplateCenterX">template center X</param>
        /// <param name="fTemplateCenterY">template center Y</param>
        public void SetRectGaugeTemplate(float fTemplateCenterX, float fTemplateCenterY)
        {
            m_fTemplateObjectCenterX = fTemplateCenterX;
            m_fTemplateObjectCenterY = fTemplateCenterY;
        }
        public void SetRectGaugeTemplate(float fTemplateCenterX, float fTemplateCenterY, float fWidth, float fHeight)
        {
            m_fTemplateObjectCenterX = fTemplateCenterX;
            m_fTemplateObjectCenterY = fTemplateCenterY;
            m_fTemplateObjectWidth = fWidth;
            m_fTemplateObjectHeight = fHeight;
        }



        /// <summary>
        /// <summary>
        /// Check whether the Gauge Positions had falled into valid rectangle area
        /// </summary>
        /// <param name="intMaxAngle">max angle of gauge that is valid</param>
        /// <returns>true if most of points are valid; false if more than 5% is invalid / gauge angle > +-max angle</returns>
        public bool VerifyGaugeValid(int intMaxAngle)
        {
#if (Debug_2_12 || Release_2_12)
            uint intValidPoints = m_RectGauge.NumValidSamples;
            uint intPoints = m_RectGauge.NumSamples;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            int intValidPoints = m_RectGauge.NumValidSamples;
            int intPoints = m_RectGauge.NumSamples;

#endif

            float intPercentage = (float)intValidPoints / intPoints * 100;
            if (intPercentage < 50)
                return false;

            if (Math.Abs(m_RectGauge.MeasuredRectangle.Angle) >= intMaxAngle)
                return false;

            return true;
        }

        public void SaveGauge(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);

            // Rectangle gauge template measurement result
            objFile.WriteElement1Value("ObjectCenterX", m_RectGauge.MeasuredRectangle.CenterX);
            objFile.WriteElement1Value("ObjectCenterY", m_RectGauge.MeasuredRectangle.CenterY);
            objFile.WriteElement1Value("ObjectWidth", m_RectGauge.MeasuredRectangle.SizeX);
            objFile.WriteElement1Value("ObjectHeight", m_RectGauge.MeasuredRectangle.SizeY);
            objFile.WriteElement1Value("ObjectAngle", m_RectGauge.MeasuredRectangle.Angle);

            // Rectangle gauge position setting
            objFile.WriteElement1Value("CenterX", m_RectGauge.Center.X);
            objFile.WriteElement1Value("CenterY", m_RectGauge.Center.Y);
            objFile.WriteElement1Value("Width", m_RectGauge.SizeX);
            objFile.WriteElement1Value("Height", m_RectGauge.SizeY);
            objFile.WriteElement1Value("Angle", m_RectGauge.Angle);
            objFile.WriteElement1Value("Tolerance", m_RectGauge.Tolerance);

            // Rectangle gauge measurement setting
            objFile.WriteElement1Value("TransType", m_RectGauge.TransitionType.GetHashCode());
            objFile.WriteElement1Value("TransChoice", m_RectGauge.TransitionChoice.GetHashCode());
            objFile.WriteElement1Value("Threshold", m_RectGauge.Threshold);
            objFile.WriteElement1Value("Thickness", m_RectGauge.Thickness);
            objFile.WriteElement1Value("MinAmp", m_RectGauge.MinAmplitude);
            objFile.WriteElement1Value("MinArea", m_RectGauge.MinArea);
            objFile.WriteElement1Value("Filter", m_RectGauge.Smoothing);

            // Rectangle gauge fitting setting
            objFile.WriteElement1Value("SamplingStep", m_RectGauge.SamplingStep);
            objFile.WriteElement1Value("FilteringThreshold", m_RectGauge.FilteringThreshold);
            objFile.WriteElement1Value("FilteringPasses", m_RectGauge.NumFilteringPasses);

            // Other
            objFile.WriteElement1Value("SizeTolerance", m_fSizeTolerance);
            objFile.WriteElement1Value("GainValue", m_fGainValue);
            objFile.WriteElement1Value("DrawSampledPoints", m_blnDrawSampledPoints, "Draw Gauge SampledPoints", true);
            objFile.WriteElement1Value("DrawDraggingBox", m_blnDrawDraggingBox, "Draw Gauge DraggingBox", true);

            objFile.WriteEndElement();
        }
        
        public void LoadGauge(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);

            // Rectangle gauge template measurement result
            m_fTemplateObjectCenterX = objFile.GetValueAsFloat("ObjectCenterX", 0);
            m_fTemplateObjectCenterY = objFile.GetValueAsFloat("ObjectCenterY", 0);
            m_fTemplateObjectWidth = objFile.GetValueAsFloat("ObjectWidth", 0);
            m_fTemplateObjectHeight = objFile.GetValueAsFloat("ObjectHeight", 0);

            // Rectangle gauge position setting
            m_RectGauge.SetCenterXY(objFile.GetValueAsFloat("CenterX", 0),
                                  objFile.GetValueAsFloat("CenterY", 0));
            m_RectGauge.SetSize(objFile.GetValueAsFloat("Width", 100),
                                  objFile.GetValueAsFloat("Height", 100));
            m_RectGauge.Angle = objFile.GetValueAsFloat("Angle", 0);
            m_RectGauge.Tolerance = objFile.GetValueAsFloat("Tolerance", 10);
#if (Debug_2_12 || Release_2_12)
            // Rectangle gauge measurement setting
            m_RectGauge.TransitionType = (ETransitionType)objFile.GetValueAsInt("TransType", 0);
            m_RectGauge.TransitionChoice = (ETransitionChoice)objFile.GetValueAsInt("TransChoice", 0);
            m_RectGauge.Threshold = objFile.GetValueAsUInt("Threshold", 2);
            m_RectGauge.Thickness = objFile.GetValueAsUInt("Thickness", 13);
            m_RectGauge.MinAmplitude = objFile.GetValueAsUInt("MinAmp", 10);
            m_RectGauge.MinArea = objFile.GetValueAsUInt("MinArea", 0);
            m_RectGauge.Smoothing = objFile.GetValueAsUInt("Filter", 1);

            // Rectangle gauge fitting setting
            m_RectGauge.SamplingStep = objFile.GetValueAsInt("SamplingStep", 3);
            m_RectGauge.FilteringThreshold = objFile.GetValueAsInt("FilteringThreshold", 3);
            m_RectGauge.NumFilteringPasses = objFile.GetValueAsUInt("FilteringPasses", 3);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Rectangle gauge measurement setting
            m_RectGauge.TransitionType = (ETransitionType)objFile.GetValueAsInt("TransType", 0);
            m_RectGauge.TransitionChoice = (ETransitionChoice)objFile.GetValueAsInt("TransChoice", 0);
            m_RectGauge.Threshold = objFile.GetValueAsInt("Threshold", 2);
            m_RectGauge.Thickness = objFile.GetValueAsInt("Thickness", 13);
            m_RectGauge.MinAmplitude = objFile.GetValueAsInt("MinAmp", 10);
            m_RectGauge.MinArea = objFile.GetValueAsInt("MinArea", 0);
            m_RectGauge.Smoothing = objFile.GetValueAsInt("Filter", 1);

            // Rectangle gauge fitting setting
            m_RectGauge.SamplingStep = objFile.GetValueAsInt("SamplingStep", 3);
            m_RectGauge.FilteringThreshold = objFile.GetValueAsInt("FilteringThreshold", 3);
            m_RectGauge.NumFilteringPasses = objFile.GetValueAsInt("FilteringPasses", 3);

#endif

            // Other
            m_fSizeTolerance = objFile.GetValueAsFloat("SizeTolerance", 0);
            m_fGainValue = objFile.GetValueAsFloat("GainValue", 1000f);
            m_blnDrawSampledPoints = objFile.GetValueAsBoolean("DrawSampledPoints", true);
            m_blnDrawDraggingBox = objFile.GetValueAsBoolean("DrawDraggingBox", true);
        }

        public void Dispose()
        {
            m_BrushMatched.Dispose();

            if (m_RectGauge != null)
                m_RectGauge.Dispose();
        }

        public void SaveGaugeObject(string strSavePath)
        {
            m_RectGauge.Save(strSavePath);
        }

        public bool CalculateValidSamplePoint(float fMinimum_Score)
        {
            float PassScore = ((float)m_RectGauge.NumValidSamples / m_RectGauge.NumSamples) * 100;
            if (PassScore < fMinimum_Score)
                return false;
            else
                return true;
        }
    }
}
