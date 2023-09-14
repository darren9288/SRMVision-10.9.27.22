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
    public class PGauge
    {
        #region Member Variables
        private EPointGauge m_objPointGauge;
        private bool m_Handler = false;
        //font properties
        private Font m_FontMatched = new Font("Tahoma", 8);
        private SolidBrush m_BrushMatched = new SolidBrush(Color.GreenYellow);

        #endregion

        #region Properties
#if (Debug_2_12 || Release_2_12)
        public int ref_intMeasuredPointCount { get { return (int)m_objPointGauge.NumMeasuredPoints; } }
        public bool ref_Handler { get { return m_Handler; } }
        public float ref_GaugeCenterX { get { return m_objPointGauge.Center.X; } }
        public float ref_GaugeCenterY { get { return m_objPointGauge.Center.Y; } }
        public int ref_GaugeMinAmplitude { get { return (int)m_objPointGauge.MinAmplitude; } set { m_objPointGauge.MinAmplitude = (uint)value; } }
        public int ref_GaugeMinArea { get { return (int)m_objPointGauge.MinArea; } set { m_objPointGauge.MinArea = (uint)value; } }
        public int ref_GaugeFilter { get { return (int)m_objPointGauge.Smoothing; } set { m_objPointGauge.Smoothing = (uint)value; } }
        public int ref_GaugeThickness { get { return (int)m_objPointGauge.Thickness; } set { m_objPointGauge.Thickness = (uint)value; } }
        public int ref_GaugeThreshold { get { return (int)m_objPointGauge.Threshold; } set { m_objPointGauge.Threshold = (uint)value; } }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
        public int ref_intMeasuredPointCount { get { return m_objPointGauge.NumMeasuredPoints; } }
        public bool ref_Handler { get { return m_Handler; } }
        public float ref_GaugeCenterX { get { return m_objPointGauge.Center.X; } }
        public float ref_GaugeCenterY { get { return m_objPointGauge.Center.Y; } }
        public int ref_GaugeMinAmplitude { get { return m_objPointGauge.MinAmplitude; } set { m_objPointGauge.MinAmplitude = value; } }
        public int ref_GaugeMinArea { get { return m_objPointGauge.MinArea; } set { m_objPointGauge.MinArea = value; } }
        public int ref_GaugeFilter { get { return m_objPointGauge.Smoothing; } set { m_objPointGauge.Smoothing = value; } }
        public int ref_GaugeThickness { get { return m_objPointGauge.Thickness; } set { m_objPointGauge.Thickness = value; } }
        public int ref_GaugeThreshold { get { return m_objPointGauge.Threshold; } set { m_objPointGauge.Threshold = value; } }
#endif

        public int ref_GaugeTransType { get { return m_objPointGauge.TransitionType.GetHashCode(); } set { m_objPointGauge.TransitionType = (ETransitionType)value; } }
        public int ref_GaugeTransChoice { get { return m_objPointGauge.TransitionChoice.GetHashCode(); } set { m_objPointGauge.TransitionChoice = (ETransitionChoice)value; } }
        public float ref_GaugeTolerance { get { return m_objPointGauge.Tolerance; } set { m_objPointGauge.Tolerance = value; } }
        public float ref_GaugeAngle { get { return m_objPointGauge.ToleranceAngle; } set { m_objPointGauge.ToleranceAngle = value; } }
        public EPointGauge ref_objPointGauge { get { return m_objPointGauge; } set { m_objPointGauge = value; } }
        #endregion

        public PGauge()
        {
            m_objPointGauge = new EPointGauge();

            m_objPointGauge.Dragable = false;
            m_objPointGauge.Rotatable = false;
            m_objPointGauge.Resizable = false;
            m_objPointGauge.TransitionType = ETransitionType.Wb;
            m_objPointGauge.TransitionChoice = ETransitionChoice.NthFromBegin;
        }

        public PGauge(GaugeWorldShape objWorldShape)
        {
            m_objPointGauge = new EPointGauge();

            m_objPointGauge.Dragable = false;
            m_objPointGauge.Rotatable = false;
            m_objPointGauge.Resizable = false;

            m_objPointGauge.MinAmplitude = 0;
            m_objPointGauge.MinArea = 0;
            m_objPointGauge.Smoothing = 1;
            m_objPointGauge.TransitionChoice = ETransitionChoice.NthFromBegin;

            m_objPointGauge.Attach(objWorldShape.ref_objWorldShape);
        }

        public void SetGaugeToleranceAngle(float fGaugeTolerance, float fGaugeAngle)
        {
            m_objPointGauge.SetTolerances(fGaugeTolerance, fGaugeAngle);
        }

        public PointF GetMeasurePoint(int intPointIndex)
        {
            //if (intPointIndex < m_objPointGauge.NumMeasuredPoints)
            //    return new PointF(m_objPointGauge.GetMeasuredPoint(intPointIndex).X, m_objPointGauge.GetMeasuredPoint(intPointIndex).Y);
            //else
            //    return new PointF(0,0);           
            try
            {
                if (m_objPointGauge.NumMeasuredPoints == 0)
                    return new PointF(0, 0);

                if (m_objPointGauge.NumMeasuredPoints == 1)
                    return new PointF(m_objPointGauge.GetMeasuredPoint(0).X, m_objPointGauge.GetMeasuredPoint(0).Y);
                else
                {
                    switch (m_objPointGauge.TransitionChoice)
                    {
                        case ETransitionChoice.NthFromBegin:
                            {
                                return new PointF(m_objPointGauge.GetMeasuredPoint(0).X, m_objPointGauge.GetMeasuredPoint(0).Y);
                            }
                            break;
                        case ETransitionChoice.NthFromEnd:
                            {
#if (Debug_2_12 || Release_2_12)
                                uint intLastIndex = m_objPointGauge.NumMeasuredPoints - 1;
                                return new PointF(m_objPointGauge.GetMeasuredPoint(intLastIndex).X, m_objPointGauge.GetMeasuredPoint(intLastIndex).Y);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                int intLastIndex = m_objPointGauge.NumMeasuredPoints - 1;
                                return new PointF(m_objPointGauge.GetMeasuredPoint(intLastIndex).X, m_objPointGauge.GetMeasuredPoint(intLastIndex).Y);
#endif

                            }
                            break;
                        case ETransitionChoice.LargestAmplitude:
                            {
                                int intSelectedIndex = -1;
                                int intHighestAmplitude = 0;
                                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                                {
#if (Debug_2_12 || Release_2_12)
                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak((uint)i);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak(i);

#endif
                                    //2021-01-19 ZJYEOH : Change < to <= because sometimes the amplitude are same, but drawing for pad gauge blob method will draw the last point
                                    if (intSelectedIndex == -1 || intHighestAmplitude <= Math.Abs(objPeak.Amplitude)) // Amplitude value is from 0 to negative or positive depends on Wto B or B to W

                                        //if (intHighestAmplitude < Math.Abs(objPeak.Amplitude)) // Amplitude value is from 0 to negative
                                        {
                                            intSelectedIndex = i;
                                            intHighestAmplitude = Math.Abs(objPeak.Amplitude);
                                        }

                                }
#if (Debug_2_12 || Release_2_12)
                                if (intSelectedIndex >= 0)
                                    return new PointF(m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).Y);
                                else
                                    return new PointF(0, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                if (intSelectedIndex >= 0)
                                    return new PointF(m_objPointGauge.GetMeasuredPoint(intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint(intSelectedIndex).Y);
                                else
                                    return new PointF(0, 0);
#endif


                            }
                            break;
                        default:
                        case ETransitionChoice.LargestArea:
                            {
                                int intSelectedIndex = -1;
                                float fClosestToSetPoint = 0;
                                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                                {
                                    PointF p;
#if (Debug_2_12 || Release_2_12)
                                    p = new PointF(m_objPointGauge.GetMeasuredPoint((uint)i).X, m_objPointGauge.GetMeasuredPoint((uint)i).Y);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            p = new PointF(m_objPointGauge.GetMeasuredPoint(i).X, m_objPointGauge.GetMeasuredPoint(i).Y);
                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak(i);

#endif
                                    float fDistance = (float)Math.Sqrt(Math.Pow(m_objPointGauge.CenterX - p.X, 2) + Math.Pow(m_objPointGauge.CenterY - p.Y, 2));

                                    if (intSelectedIndex == -1 || (fClosestToSetPoint > fDistance))
                                    {
                                        fClosestToSetPoint = fDistance;
                                        intSelectedIndex = i;
                                    }
                                }
#if (Debug_2_12 || Release_2_12)
                                if (intSelectedIndex >= 0)
                                    return new PointF(m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).Y);
                                else
                                    return new PointF(0, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                if (intSelectedIndex >= 0)
                                    return new PointF(m_objPointGauge.GetMeasuredPoint(intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint(intSelectedIndex).Y);
                                else
                                    return new PointF(0, 0);
#endif


                            }
                            //                            {
                            //                                int intSelectedIndex = -1;
                            //                                int intHighestAmplitude = 0;
                            //                                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                            //                                {
                            //#if (Debug_2_12 || Release_2_12)
                            //                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak((uint)i);

                            //#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            //                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak(i);

                            //#endif

                            //                                    if (intHighestAmplitude > objPeak.Area) // Amplitude value is from 0 to negative
                            //                                    {
                            //                                        intSelectedIndex = i;
                            //                                        intHighestAmplitude = objPeak.Amplitude;
                            //                                    }
                            //                                }
                            //#if (Debug_2_12 || Release_2_12)
                            //                                if (intSelectedIndex >= 0)
                            //                                    return new PointF(m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).Y);
                            //                                else
                            //                                    return new PointF(0, 0);
                            //#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            //                                if (intSelectedIndex >= 0)
                            //                                    return new PointF(m_objPointGauge.GetMeasuredPoint(intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint(intSelectedIndex).Y);
                            //                                else
                            //                                    return new PointF(0, 0);
                            //#endif

                            //                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                return new PointF(0, 0);
            }
        }
        

        public PointF GetMeasurePoint()
        {
            try
            {
                if (m_objPointGauge.NumMeasuredPoints == 0)
                    return new PointF(0, 0);

                if (m_objPointGauge.NumMeasuredPoints == 1)
                {
                    return new PointF(m_objPointGauge.GetMeasuredPoint(0).X, m_objPointGauge.GetMeasuredPoint(0).Y);
                }
                else
                {
                    switch (m_objPointGauge.TransitionChoice)
                    {
                        case ETransitionChoice.NthFromBegin:
                            {
                                return new PointF(m_objPointGauge.GetMeasuredPoint(0).X, m_objPointGauge.GetMeasuredPoint(0).Y);
                            }
                            break;
                        case ETransitionChoice.NthFromEnd:
                            {
#if (Debug_2_12 || Release_2_12)
                                uint intLastIndex = m_objPointGauge.NumMeasuredPoints - 1;
                                return new PointF(m_objPointGauge.GetMeasuredPoint(intLastIndex).X, m_objPointGauge.GetMeasuredPoint(intLastIndex).Y);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                                    int intLastIndex = m_objPointGauge.NumMeasuredPoints - 1;
                                                    return new PointF(m_objPointGauge.GetMeasuredPoint(intLastIndex).X, m_objPointGauge.GetMeasuredPoint(intLastIndex).Y);
#endif

                            }
                            break;
                        case ETransitionChoice.LargestAmplitude:
                            {
                                int intSelectedIndex = -1;
                                int intHighestAmplitude = 0;
                                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                                {
#if (Debug_2_12 || Release_2_12)
                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak((uint)i);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                                        EPeak objPeak = m_objPointGauge.GetMeasuredPeak(i);

#endif
                                    if (intHighestAmplitude < Math.Abs(objPeak.Amplitude)) // Amplitude value is from 0 to negative or positive depends on Wto B or B to W

                                        if (intHighestAmplitude < Math.Abs(objPeak.Amplitude)) // Amplitude value is from 0 to negative
                                        {
                                            intSelectedIndex = i;
                                            intHighestAmplitude = Math.Abs(objPeak.Amplitude);
                                        }

                                }
#if (Debug_2_12 || Release_2_12)
                                if (intSelectedIndex >= 0)
                                    return new PointF(m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).Y);
                                else
                                    return new PointF(0, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                                    if (intSelectedIndex >= 0)
                                                        return new PointF(m_objPointGauge.GetMeasuredPoint(intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint(intSelectedIndex).Y);
                                                    else
                                                        return new PointF(0, 0);
#endif


                            }
                            break;
                        default:
                        case ETransitionChoice.LargestArea:
                            {
                                int intSelectedIndex = -1;
                                    float fClosestToSetPoint = 0;
                                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                                {
                                    PointF p;
#if (Debug_2_12 || Release_2_12)
                                    p = new PointF(m_objPointGauge.GetMeasuredPoint((uint)i).X, m_objPointGauge.GetMeasuredPoint((uint)i).Y);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            p = new PointF(m_objPointGauge.GetMeasuredPoint(i).X, m_objPointGauge.GetMeasuredPoint(i).Y);
                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak(i);

#endif
                                    float fDistance = (float)Math.Sqrt(Math.Pow(m_objPointGauge.CenterX - p.X, 2) + Math.Pow(m_objPointGauge.CenterY - p.Y, 2));

                                    if (intSelectedIndex == -1 || (fClosestToSetPoint > fDistance))
                                    {
                                        fClosestToSetPoint = fDistance;
                                        intSelectedIndex = i;
                                    }
                                }
#if (Debug_2_12 || Release_2_12)
                                if (intSelectedIndex >= 0)
                                    return new PointF(m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).Y);
                                else
                                    return new PointF(0, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                if (intSelectedIndex >= 0)
                                    return new PointF(m_objPointGauge.GetMeasuredPoint(intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint(intSelectedIndex).Y);
                                else
                                    return new PointF(0, 0);
#endif


                            }
                            //                            {
                            //                                int intSelectedIndex = -1;
                            //                                int intHighestAmplitude = 0;
                            //                                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                            //                                {
                            //#if (Debug_2_12 || Release_2_12)
                            //                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak((uint)i);

                            //#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            //                                                        EPeak objPeak = m_objPointGauge.GetMeasuredPeak(i);

                            //#endif

                            //                                    if (intHighestAmplitude > objPeak.Area) // Amplitude value is from 0 to negative
                            //                                    {
                            //                                        intSelectedIndex = i;
                            //                                        intHighestAmplitude = objPeak.Amplitude;
                            //                                    }
                            //                                }
                            //#if (Debug_2_12 || Release_2_12)
                            //                                if (intSelectedIndex >= 0)
                            //                                    return new PointF(m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).Y);
                            //                                else
                            //                                    return new PointF(0, 0);
                            //#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            //                                                    if (intSelectedIndex >= 0)
                            //                                                        return new PointF(m_objPointGauge.GetMeasuredPoint(intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint(intSelectedIndex).Y);
                            //                                                    else
                            //                                                        return new PointF(0, 0);
                            //#endif

                            //                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                return new PointF(0, 0);
            }
        }

        public PointF GetMeasurePoint(float fAngle, float fLimitX, float fLimitY)
        {
            return GetMeasurePoint(fAngle, fLimitX, fLimitY, false);
        }

        public PointF GetMeasurePoint(float fAngle, float fLimitX, float fLimitY, bool blnWantClosestModeFilter)
        {
            try
            {
                if (m_objPointGauge.NumMeasuredPoints == 0)
                    return new PointF(0, 0);

                if (m_objPointGauge.NumMeasuredPoints == 1)
                {
                    // 2022 01 24 - CCENG
                    // Largest Area is used for Closest mode
                    // Only Closest mode and main gauge point (line 1 and 2) will apply closest mode only.
                    if (m_objPointGauge.TransitionChoice == ETransitionChoice.LargestArea && blnWantClosestModeFilter)
                    {
                        if (fAngle == 0)
                        {
                            if (m_objPointGauge.GetMeasuredPoint(0).X > Math.Ceiling(m_objPointGauge.CenterX + 1))
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).X < fLimitX)
                                return new PointF(0, 0);
                        }
                        else if (fAngle == 180)
                        {
                            if (m_objPointGauge.GetMeasuredPoint(0).X < Math.Floor(m_objPointGauge.CenterX - 1))
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).X > fLimitX)
                                return new PointF(0, 0);
                        }
                        else if (fAngle == -90)
                        {
                            if (m_objPointGauge.GetMeasuredPoint(0).Y < Math.Floor(m_objPointGauge.CenterY - 1))
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).Y > fLimitY)
                                return new PointF(0, 0);
                        }
                        else if (fAngle == 90)
                        {
                            if (m_objPointGauge.GetMeasuredPoint(0).Y > Math.Ceiling(m_objPointGauge.CenterY + 1))
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).Y < fLimitY)
                                return new PointF(0, 0);
                        }
                        else if (fAngle > 0 && fAngle < 90) // Bottom Right area
                        {
                            if (m_objPointGauge.GetMeasuredPoint(0).Y > Math.Ceiling(m_objPointGauge.CenterY + 1))
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).X > Math.Ceiling(m_objPointGauge.CenterX + 1))
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).Y < fLimitY)
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).X < fLimitX)
                                return new PointF(0, 0);
                        }
                        else if (fAngle > 90 && fAngle < 180) // Bottom Left Area
                        {
                            if (m_objPointGauge.GetMeasuredPoint(0).Y > Math.Ceiling(m_objPointGauge.CenterY + 1))
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).X < Math.Floor(m_objPointGauge.CenterX - 1))
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).Y < fLimitY)
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).X > fLimitX)
                                return new PointF(0, 0);
                        }
                        else if (fAngle < 0 && fAngle > -90)    // Top Right Area
                        {
                            if (m_objPointGauge.GetMeasuredPoint(0).Y < Math.Floor(m_objPointGauge.CenterY - 1))
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).X > Math.Ceiling(m_objPointGauge.CenterX + 1))
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).Y > fLimitY)
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).X < fLimitX)
                                return new PointF(0, 0);
                        }
                        else if (fAngle < -90 && fAngle > -180) // Top Left Area
                        {
                            if (m_objPointGauge.GetMeasuredPoint(0).Y < Math.Floor(m_objPointGauge.CenterY - 1))
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).X < Math.Floor(m_objPointGauge.CenterX - 1))
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).Y > fLimitY)
                                return new PointF(0, 0);
                            if (m_objPointGauge.GetMeasuredPoint(0).X > fLimitX)
                                return new PointF(0, 0);
                        }
                    }

                    return new PointF(m_objPointGauge.GetMeasuredPoint(0).X, m_objPointGauge.GetMeasuredPoint(0).Y);
                }
                else
                {
                    switch (m_objPointGauge.TransitionChoice)
                    {
                        case ETransitionChoice.NthFromBegin:
                            {
                                return new PointF(m_objPointGauge.GetMeasuredPoint(0).X, m_objPointGauge.GetMeasuredPoint(0).Y);
                            }
                            break;
                        case ETransitionChoice.NthFromEnd:
                            {
#if (Debug_2_12 || Release_2_12)
                                uint intLastIndex = m_objPointGauge.NumMeasuredPoints - 1;
                                return new PointF(m_objPointGauge.GetMeasuredPoint(intLastIndex).X, m_objPointGauge.GetMeasuredPoint(intLastIndex).Y);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                                    int intLastIndex = m_objPointGauge.NumMeasuredPoints - 1;
                                                    return new PointF(m_objPointGauge.GetMeasuredPoint(intLastIndex).X, m_objPointGauge.GetMeasuredPoint(intLastIndex).Y);
#endif

                            }
                            break;
                        case ETransitionChoice.LargestAmplitude:
                            {
                                int intSelectedIndex = -1;
                                int intHighestAmplitude = 0;
                                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                                {
#if (Debug_2_12 || Release_2_12)
                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak((uint)i);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                                        EPeak objPeak = m_objPointGauge.GetMeasuredPeak(i);

#endif
                                    if (intHighestAmplitude < Math.Abs(objPeak.Amplitude)) // Amplitude value is from 0 to negative or positive depends on Wto B or B to W

                                        if (intHighestAmplitude < Math.Abs(objPeak.Amplitude)) // Amplitude value is from 0 to negative
                                        {
                                            intSelectedIndex = i;
                                            intHighestAmplitude = Math.Abs(objPeak.Amplitude);
                                        }

                                }
#if (Debug_2_12 || Release_2_12)
                                if (intSelectedIndex >= 0)
                                    return new PointF(m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).Y);
                                else
                                    return new PointF(0, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                                    if (intSelectedIndex >= 0)
                                                        return new PointF(m_objPointGauge.GetMeasuredPoint(intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint(intSelectedIndex).Y);
                                                    else
                                                        return new PointF(0, 0);
#endif


                            }
                            break;
                        default:
                        case ETransitionChoice.LargestArea:
                            {
                                int intSelectedIndex = -1;
                                float fClosestToSetPoint = 0;
                                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                                {
                                    // 2022 01 24 - CCENG: Only Line 1 and 2 will filter point that out of gauge point location.
                                    //                     Line 1 and 2 is controlled by Blobs size.
                                    //                     Line after 2 is not controllered by blobs size, but drag by user. the result point may be always out of gauge point location.
                                    if (blnWantClosestModeFilter)
                                    {
                                        if (fAngle == 0)
                                        {
#if (Debug_2_12 || Release_2_12)
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).X > Math.Ceiling(m_objPointGauge.CenterX + 1))
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).X < fLimitX)
                                                continue;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                        if (m_objPointGauge.GetMeasuredPoint(i).X > Math.Ceiling(m_objPointGauge.CenterX + 1))
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).X < fLimitX)
                                            continue;
#endif

                                        }
                                        else if (fAngle == 180)
                                        {
#if (Debug_2_12 || Release_2_12)
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).X < Math.Floor(m_objPointGauge.CenterX - 1))
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).X > fLimitX)
                                                continue;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                        if (m_objPointGauge.GetMeasuredPoint(i).X < Math.Floor(m_objPointGauge.CenterX - 1))
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).X > fLimitX)
                                            continue;
#endif

                                        }
                                        else if (fAngle == -90)
                                        {
#if (Debug_2_12 || Release_2_12)
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).Y < Math.Floor(m_objPointGauge.CenterY - 1))
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).Y > fLimitY)
                                                continue;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                        if (m_objPointGauge.GetMeasuredPoint(i).Y < Math.Floor(m_objPointGauge.CenterY - 1))
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).Y > Math.Ceiling(Ceiling))
                                            continue;
#endif
                                        }
                                        else if (fAngle == 90)
                                        {
#if (Debug_2_12 || Release_2_12)
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).Y > Math.Ceiling(m_objPointGauge.CenterY + 1))
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).Y < fLimitY)
                                                continue;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                        if (m_objPointGauge.GetMeasuredPoint(i).Y > Math.Ceiling(m_objPointGauge.CenterY + 1))
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).Y < fLimitY)
                                            continue;
#endif
                                        }
                                        else if (fAngle > 0 && fAngle < 90) // Bottom Right area
                                        {
#if (Debug_2_12 || Release_2_12)
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).Y > Math.Ceiling(m_objPointGauge.CenterY + 1))
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).X > Math.Ceiling(m_objPointGauge.CenterX + 1))
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).Y < fLimitY)
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).X < fLimitX)
                                                continue;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                        if (m_objPointGauge.GetMeasuredPoint(i).Y > Math.Ceiling(m_objPointGauge.CenterY + 1))
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).X > Math.Ceiling(m_objPointGauge.CenterX + 1))
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).Y < fLimitY)
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).X < fLimitX)
                                            continue;
#endif
                                        }
                                        else if (fAngle > 90 && fAngle < 180) // Bottom Left Area
                                        {
#if (Debug_2_12 || Release_2_12)
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).Y > Math.Ceiling(m_objPointGauge.CenterY + 1))
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).X < Math.Floor(m_objPointGauge.CenterX - 1))
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).Y < fLimitY)
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).X > fLimitX)
                                                continue;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                        if (m_objPointGauge.GetMeasuredPoint(i).Y > Math.Ceiling(m_objPointGauge.CenterY + 1))
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).X < Math.Floor(m_objPointGauge.CenterX - 1))
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).Y < fLimitY)
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).X > fLimitX)
                                            continue;
#endif
                                        }
                                        else if (fAngle < 0 && fAngle > -90)    // Top Right Area
                                        {
#if (Debug_2_12 || Release_2_12)
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).Y < Math.Floor(m_objPointGauge.CenterY - 1))
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).X > Math.Ceiling(m_objPointGauge.CenterX + 1))
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).Y > fLimitY)
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).X < fLimitX)
                                                continue;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                        if (m_objPointGauge.GetMeasuredPoint(i).Y < Math.Floor(m_objPointGauge.CenterY - 1))
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).X > Math.Ceiling(m_objPointGauge.CenterX + 1))
                                            continue;
#endif
                                        }
                                        else if (fAngle < -90 && fAngle > -180) // Top Left Area
                                        {
#if (Debug_2_12 || Release_2_12)
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).Y < Math.Floor(m_objPointGauge.CenterY - 1))
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).X < Math.Floor(m_objPointGauge.CenterX - 1))
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).Y > fLimitY)
                                                continue;
                                            if (m_objPointGauge.GetMeasuredPoint((uint)i).X > fLimitX)
                                                continue;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                        if (m_objPointGauge.GetMeasuredPoint(i).Y < Math.Floor(m_objPointGauge.CenterY - 1))
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).X < Math.Floor(m_objPointGauge.CenterX - 1))
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).Y > fLimitY)
                                            continue;
                                        if (m_objPointGauge.GetMeasuredPoint(i).X > fLimitX)
                                            continue;
#endif
                                        }
                                    }

                                    PointF p;
#if (Debug_2_12 || Release_2_12)
                                    p = new PointF(m_objPointGauge.GetMeasuredPoint((uint)i).X, m_objPointGauge.GetMeasuredPoint((uint)i).Y);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            p = new PointF(m_objPointGauge.GetMeasuredPoint(i).X, m_objPointGauge.GetMeasuredPoint(i).Y);
                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak(i);

#endif
                                    float fDistance = (float)Math.Sqrt(Math.Pow(m_objPointGauge.CenterX - p.X, 2) + Math.Pow(m_objPointGauge.CenterY - p.Y, 2));

                                    if (intSelectedIndex == -1 || (fClosestToSetPoint > fDistance))
                                    {
                                        fClosestToSetPoint = fDistance;
                                        intSelectedIndex = i;
                                    }
                                }
#if (Debug_2_12 || Release_2_12)
                                if (intSelectedIndex >= 0)
                                    return new PointF(m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).Y);
                                else
                                    return new PointF(0, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                if (intSelectedIndex >= 0)
                                    return new PointF(m_objPointGauge.GetMeasuredPoint(intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint(intSelectedIndex).Y);
                                else
                                    return new PointF(0, 0);
#endif


                            }
                            //                            {
                            //                                int intSelectedIndex = -1;
                            //                                int intHighestAmplitude = 0;
                            //                                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                            //                                {
                            //#if (Debug_2_12 || Release_2_12)
                            //                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak((uint)i);

                            //#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            //                                                        EPeak objPeak = m_objPointGauge.GetMeasuredPeak(i);

                            //#endif

                            //                                    if (intHighestAmplitude > objPeak.Area) // Amplitude value is from 0 to negative
                            //                                    {
                            //                                        intSelectedIndex = i;
                            //                                        intHighestAmplitude = objPeak.Amplitude;
                            //                                    }
                            //                                }
                            //#if (Debug_2_12 || Release_2_12)
                            //                                if (intSelectedIndex >= 0)
                            //                                    return new PointF(m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).Y);
                            //                                else
                            //                                    return new PointF(0, 0);
                            //#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            //                                                    if (intSelectedIndex >= 0)
                            //                                                        return new PointF(m_objPointGauge.GetMeasuredPoint(intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint(intSelectedIndex).Y);
                            //                                                    else
                            //                                                        return new PointF(0, 0);
                            //#endif

                            //                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                return new PointF(0, 0);
            }
        }

        public PointF GetMaxAmplitudeMeasuredPoint()
        {
            try
            {
                if (m_objPointGauge.NumMeasuredPoints == 0)
                    return new PointF(0, 0);

                if (m_objPointGauge.NumMeasuredPoints == 1)
                    return new PointF(m_objPointGauge.GetMeasuredPoint(0).X, m_objPointGauge.GetMeasuredPoint(0).Y);
                else
                {
                    switch (m_objPointGauge.TransitionChoice)
                    {
                        default:
                        case ETransitionChoice.LargestAmplitude:
                            {
                                int intSelectedIndex = -1;
                                int intHighestAmplitude = 0;
                                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                                {
#if (Debug_2_12 || Release_2_12)
                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak((uint)i);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                    EPeak objPeak = m_objPointGauge.GetMeasuredPeak(i);

#endif
                                    if (intHighestAmplitude < Math.Abs(objPeak.Amplitude)) // Amplitude value is from 0 to negative or positive depends on Wto B or B to W

                                        if (intHighestAmplitude < Math.Abs(objPeak.Amplitude)) // Amplitude value is from 0 to negative
                                        {
                                            intSelectedIndex = i;
                                            intHighestAmplitude = Math.Abs(objPeak.Amplitude);
                                        }

                                }
#if (Debug_2_12 || Release_2_12)
                                if (intSelectedIndex >= 0)
                                    return new PointF(m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).Y);
                                else
                                    return new PointF(0, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                if (intSelectedIndex >= 0)
                                    return new PointF(m_objPointGauge.GetMeasuredPoint(intSelectedIndex).X, m_objPointGauge.GetMeasuredPoint(intSelectedIndex).Y);
                                else
                                    return new PointF(0, 0);
#endif


                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                return new PointF(0, 0);
            }
        }
        public void GetAllMeasurePoints(ref List<PointF> arrPoints)
        {
            try
            {
                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                {
#if (Debug_2_12 || Release_2_12)
                    arrPoints.Add(new PointF(m_objPointGauge.GetMeasuredPoint((uint)i).X, m_objPointGauge.GetMeasuredPoint((uint)i).Y));
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    arrPoints.Add(new PointF(m_objPointGauge.GetMeasuredPoint(i).X, m_objPointGauge.GetMeasuredPoint(i).Y));
#endif

                }
            }
            catch (Exception ex)
            {

            }
        }

        public float GetMeasurePointX(int intPointIndex)
        {
            // 2019-10-23 ZJYEOH : Need get point according to setting
            if (m_objPointGauge.TransitionChoice == ETransitionChoice.NthFromBegin)
            {
                if (m_objPointGauge.NumMeasuredPoints > 0)
                    return m_objPointGauge.GetMeasuredPoint(0).X;
                else
                    return 0;
            }
            else if (m_objPointGauge.TransitionChoice == ETransitionChoice.NthFromEnd)
            {
                if (m_objPointGauge.NumMeasuredPoints > 0)
                    return m_objPointGauge.GetMeasuredPoint(m_objPointGauge.NumMeasuredPoints - 1).X;
                else
                    return 0;
            }
            else if (m_objPointGauge.TransitionChoice == ETransitionChoice.LargestAmplitude)
            {
                if (m_objPointGauge.NumMeasuredPoints > 0)
                {
#if (Debug_2_12 || Release_2_12)
                    uint LargestAmplitudeIndex = 0;
                    for (uint i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    int LargestAmplitudeIndex = 0;
                    for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
#endif


                    {
                        if (i != LargestAmplitudeIndex)
                            if (Math.Abs(m_objPointGauge.GetMeasuredPeak(LargestAmplitudeIndex).Amplitude) < Math.Abs(m_objPointGauge.GetMeasuredPeak(i).Amplitude))
                                LargestAmplitudeIndex = i;
                    }
                    return m_objPointGauge.GetMeasuredPoint(LargestAmplitudeIndex).X;
                }
                else
                    return 0;
            }
            else
            {
                int intSelectedIndex = -1;
                float fClosestToSetPoint = 0;
                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                {
                    PointF p;
#if (Debug_2_12 || Release_2_12)
                    p = new PointF(m_objPointGauge.GetMeasuredPoint((uint)i).X, m_objPointGauge.GetMeasuredPoint((uint)i).Y);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            p = new PointF(m_objPointGauge.GetMeasuredPoint(i).X, m_objPointGauge.GetMeasuredPoint(i).Y);
#endif
                    float fDistance = (float)Math.Sqrt(Math.Pow(m_objPointGauge.CenterX - p.X, 2) + Math.Pow(m_objPointGauge.CenterY - p.Y, 2));

                    if (intSelectedIndex == -1 || (fClosestToSetPoint > fDistance))
                    {
                        fClosestToSetPoint = fDistance;
                        intSelectedIndex = i;
                    }
                }
#if (Debug_2_12 || Release_2_12)
                if (intSelectedIndex >= 0)
                    return m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).X;
                else
                    return 0;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                if (intSelectedIndex >= 0)
                                    return m_objPointGauge.GetMeasuredPoint(intSelectedIndex).X;
                                else
                                    return 0;
#endif

                //#if (Debug_2_12 || Release_2_12)
                //                if (intPointIndex < m_objPointGauge.NumMeasuredPoints)
                //                    return m_objPointGauge.GetMeasuredPoint((uint)intPointIndex).X;
                //                else
                //                    return 0;
                //#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                //                if (intPointIndex < m_objPointGauge.NumMeasuredPoints)
                //                    return m_objPointGauge.GetMeasuredPoint(intPointIndex).X;
                //                else
                //                    return 0;
                //#endif

            }
        }

        public float GetMeasurePointY(int intPointIndex)
        {
            // 2019-10-23 ZJYEOH : Need get point according to setting
            if (m_objPointGauge.TransitionChoice == ETransitionChoice.NthFromBegin)
            {
                if (m_objPointGauge.NumMeasuredPoints > 0)
                    return m_objPointGauge.GetMeasuredPoint(0).Y;
                else
                    return 0;
            }
            else if (m_objPointGauge.TransitionChoice == ETransitionChoice.NthFromEnd)
            {
                if (m_objPointGauge.NumMeasuredPoints > 0)
                    return m_objPointGauge.GetMeasuredPoint(m_objPointGauge.NumMeasuredPoints - 1).Y;
                else
                    return 0;
            }
            else if (m_objPointGauge.TransitionChoice == ETransitionChoice.LargestAmplitude)
            {
                if (m_objPointGauge.NumMeasuredPoints > 0)
                {
#if (Debug_2_12 || Release_2_12)
                    uint LargestAmplitudeIndex = 0;
                    for (uint i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                    {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    int LargestAmplitudeIndex = 0;
                    for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                    {
#endif

                        if (i != LargestAmplitudeIndex)
                            if (Math.Abs(m_objPointGauge.GetMeasuredPeak(LargestAmplitudeIndex).Amplitude) < Math.Abs(m_objPointGauge.GetMeasuredPeak(i).Amplitude))
                                LargestAmplitudeIndex = i;
                    }
                    return m_objPointGauge.GetMeasuredPoint(LargestAmplitudeIndex).Y;
                }
                else
                    return 0;
            }
            else
            {
                int intSelectedIndex = -1;
                float fClosestToSetPoint = 0;
                for (int i = 0; i < m_objPointGauge.NumMeasuredPoints; i++)
                {
                    PointF p;
#if (Debug_2_12 || Release_2_12)
                    p = new PointF(m_objPointGauge.GetMeasuredPoint((uint)i).X, m_objPointGauge.GetMeasuredPoint((uint)i).Y);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    p = new PointF(m_objPointGauge.GetMeasuredPoint(i).X, m_objPointGauge.GetMeasuredPoint(i).Y);

#endif
                    float fDistance = (float)Math.Sqrt(Math.Pow(m_objPointGauge.CenterX - p.X, 2) + Math.Pow(m_objPointGauge.CenterY - p.Y, 2));

                    if (intSelectedIndex == -1 || (fClosestToSetPoint > fDistance))
                    {
                        fClosestToSetPoint = fDistance;
                        intSelectedIndex = i;
                    }
                }
#if (Debug_2_12 || Release_2_12)
                if (intSelectedIndex >= 0)
                    return m_objPointGauge.GetMeasuredPoint((uint)intSelectedIndex).Y;
                else
                    return 0;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                                if (intSelectedIndex >= 0)
                                    return m_objPointGauge.GetMeasuredPoint(intSelectedIndex).Y;
                                else
                                    return 0;
#endif
                //#if (Debug_2_12 || Release_2_12)
                //                if (intPointIndex < m_objPointGauge.NumMeasuredPoints)
                //                    return m_objPointGauge.GetMeasuredPoint((uint)intPointIndex).Y;
                //                else
                //                    return 0;
                //#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                //                if (intPointIndex < m_objPointGauge.NumMeasuredPoints)
                //                    return m_objPointGauge.GetMeasuredPoint(intPointIndex).Y;
                //                else
                //                    return 0;
                //#endif

            }
        }

        public void SetGaugeCenter(float fCenterX, float fCenterY)
        {
            if (m_Handler)
            {
                if (m_objPointGauge.HitHandle == EDragHandle.Center)
                    m_objPointGauge.Drag((int)fCenterX, (int)fCenterY);
                else
                    m_objPointGauge.Drag((int)fCenterX, (int)fCenterY);
            }
        }

        public void SetGaugePlacement(float fCenterX, float fCenterY, float fTolerance, float fAngle)
        {
            try
            {
                m_objPointGauge.SetCenterXY(fCenterX, fCenterY);
                m_objPointGauge.SetTolerances(fTolerance, fAngle);
            }
            catch (Exception ex)
            {
                //SRMMessageBox.Show(ex.ToString());
            }
        }
        public void SetGaugePlacementCenter(float fCenterX, float fCenterY)
        {
            m_objPointGauge.SetCenterXY(fCenterX, fCenterY);
        }
        public void Measure(ROI objROI)
        {
            try
            {
                //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent.bmp");
                //m_objPointGauge.Save("D:\\TS\\PointGauge.CAL");
                m_objPointGauge.Measure(objROI.ref_ROI);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }
        }

        public void Measure(ImageDrawing objImage)
        {
            try
            {
                //objImage.ref_objMainImage.Save("D:\\TS\\objimage.bmp");
                //m_objPointGauge.Save("D:\\TS\\PointGauge.CAL");
                m_objPointGauge.Measure(objImage.ref_objMainImage);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }
        }

        public int AutoTuneThreshold(ROI objROI)
        {
            int intThresholdPrev = (int)m_objPointGauge.Threshold;

            int intMinThreshold = -1;
            int intMaxThreshold = -1;
            m_objPointGauge.MinAmplitude = 1;

            for (int i = 1; i < 150; i++)
            {
#if (Debug_2_12 || Release_2_12)
                m_objPointGauge.Threshold = (uint)i;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                m_objPointGauge.Threshold = i;
#endif

                m_objPointGauge.Measure(objROI.ref_ROI);

                if (m_objPointGauge.NumMeasuredPoints > 1)
                {
                    continue;
                }
                else if (intMinThreshold == -1 && m_objPointGauge.NumMeasuredPoints == 1)
                {
                    intMinThreshold = i;
                }
                else if (intMaxThreshold == -1 && m_objPointGauge.NumMeasuredPoints == 0)
                {
                    intMaxThreshold = i - 1;
                }
            }

            if (intMinThreshold == -1 || intMaxThreshold == -1)
                return intThresholdPrev;
            else
                return (int)((intMinThreshold + intMaxThreshold) / 2);
        }

        public void DrawLineProfile(Graphics g)
        {
            try
            {
                g.Clear(Color.White);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Red.R, Color.Red.G, Color.Red.B), EPlotItem.Transitions, 340, 240);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), EPlotItem.Peak, 340, 240);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B), EPlotItem.Points, 340, 240);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B), EPlotItem.Thresholds, 340, 240);
            }
            catch
            {
            }
        }

        public void DrawLineProfile(Graphics g, int intWidth, int intHeight)
        {
            try
            {
                g.Clear(Color.White);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Red.R, Color.Red.G, Color.Red.B), EPlotItem.Transitions, intWidth, intHeight);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), EPlotItem.Peak, intWidth, intHeight);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B), EPlotItem.Points, intWidth, intHeight);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B), EPlotItem.Thresholds, intWidth, intHeight);
            }
            catch
            {
            }
        }

        public void DrawLineProfile(Graphics g, float fGraphWidth, float fGraphHeight)
        {
            try
            {
                g.Clear(Color.White);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Red.R, Color.Red.G, Color.Red.B), EPlotItem.Transitions, fGraphWidth, fGraphHeight);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), EPlotItem.Peak, fGraphWidth, fGraphHeight);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B), EPlotItem.Points, fGraphWidth, fGraphHeight);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B), EPlotItem.Thresholds, fGraphWidth, fGraphHeight);
            }
            catch
            {
            }
        }

        public void DrawLineProfile_SmallerHeight(Graphics g)
        {
            try
            {
                g.Clear(Color.White);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Red.R, Color.Red.G, Color.Red.B), EPlotItem.Transitions, 340, 152);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), EPlotItem.Peak, 340, 152);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B), EPlotItem.Points, 340, 152);
                m_objPointGauge.Plot(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B), EPlotItem.Thresholds, 340, 152);
            }
            catch
            {
            }
        }
        public void DrawGauge(Graphics g)
        {
            //if (!m_blnExecuted)
            //    return;
            //m_objPointGauge.SetCenterXY(100, 100);
            //m_objPointGauge.SetTolerance(50, 0);

            m_objPointGauge.Draw(g, new ERGBColor(Color.GreenYellow.R, Color.GreenYellow.G, Color.GreenYellow.B), EDrawingMode.SampledPoints);
            m_objPointGauge.Draw(g, new ERGBColor(Color.Red.R, Color.Red.G, Color.Red.B), EDrawingMode.Actual);
            m_objPointGauge.Draw(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), EDrawingMode.Nominal);
        }
        public void DrawGaugeSamplePoint(Graphics g, Color objColor)
        {
            //if (!m_blnExecuted)
            //    return;
            //m_objPointGauge.SetCenterXY(100, 100);
            //m_objPointGauge.SetTolerance(50, 0);

            m_objPointGauge.Draw(g, new ERGBColor(objColor.R, objColor.G, objColor.B), EDrawingMode.SampledPoints);
            m_objPointGauge.Draw(g, new ERGBColor(objColor.R, objColor.G, objColor.B), EDrawingMode.Actual);
        }
        public void DrawGauge(Graphics g, bool blnDrawNominal, bool blnDrawSampledPoints)
        {
            //if (!m_blnExecuted)
            //    return;
            //m_objPointGauge.SetCenterXY(100, 100);
            //m_objPointGauge.SetTolerance(50, 0);

            if (blnDrawSampledPoints)
            {
                m_objPointGauge.Draw(g, new ERGBColor(Color.GreenYellow.R, Color.GreenYellow.G, Color.GreenYellow.B), EDrawingMode.SampledPoints);
                m_objPointGauge.Draw(g, new ERGBColor(Color.Red.R, Color.Red.G, Color.Red.B), EDrawingMode.Actual);
            }

            if (blnDrawNominal)
                m_objPointGauge.Draw(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), EDrawingMode.Nominal);

            
        }
        public void SavePointGauge(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection, bool blnSaveMeasurementAsTemplate)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);

            // Point gauge position setting
            objFile.WriteElement1Value("CenterX", m_objPointGauge.Center.X, "Gauge Setting Center X", true);
            objFile.WriteElement1Value("CenterY", m_objPointGauge.Center.Y, "Gauge Setting Center X", true);
            objFile.WriteElement1Value("Angle", m_objPointGauge.ToleranceAngle, "Gauge Setting Angle", true);
            objFile.WriteElement1Value("Tolerance", m_objPointGauge.Tolerance, "Gauge Setting Tolerance", true);

            // Rectangle gauge measurement setting
            objFile.WriteElement1Value("TransType", m_objPointGauge.TransitionType.GetHashCode(), "Gauge Setting Transition Type", true);
            objFile.WriteElement1Value("TransChoice", m_objPointGauge.TransitionChoice.GetHashCode(), "Gauge Setting Transition Choice", true);
            objFile.WriteElement1Value("Threshold", m_objPointGauge.Threshold, "Gauge Setting Threshold", true);
            objFile.WriteElement1Value("Thickness", m_objPointGauge.Thickness, "Gauge Setting Thickness", true);
            objFile.WriteElement1Value("MinAmp", m_objPointGauge.MinAmplitude, "Gauge Setting Minimum Amplitude", true);
            objFile.WriteElement1Value("MinArea", m_objPointGauge.MinArea, "Gauge Setting Minimum Area", true);
            objFile.WriteElement1Value("Filter", m_objPointGauge.Smoothing, "Gauge Setting Filter/Smoothing", true);

            objFile.WriteEndElement();

        }

        public void SavePointGauge_SECSGEM(string strPath, string strSectionName, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            // Point gauge position setting
            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_CenterX", m_objPointGauge.Center.X);
            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_CenterY", m_objPointGauge.Center.Y);
            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Angle", m_objPointGauge.ToleranceAngle);
            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Tolerance", m_objPointGauge.Tolerance);

            // Rectangle gauge measurement setting
            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_TransType", m_objPointGauge.TransitionType.GetHashCode());
            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_TransChoice", m_objPointGauge.TransitionChoice.GetHashCode());
            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Threshold", m_objPointGauge.Threshold);
            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Thickness", m_objPointGauge.Thickness);
            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_MinAmp", m_objPointGauge.MinAmplitude);
            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_MinArea", m_objPointGauge.MinArea);
            objFile.WriteElementValue(strVisionName + "_GaugeSetting_" + strSectionName + "_Filter", m_objPointGauge.Smoothing);

            objFile.WriteEndElement();

        }

        public void LoadPointGauge(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection(strSectionName);

            // Point gauge position setting
            m_objPointGauge.SetCenterXY(objFile.GetValueAsFloat("CenterX", 0), objFile.GetValueAsFloat("CenterY", 0));
            m_objPointGauge.SetTolerances(objFile.GetValueAsFloat("Tolerance", 10), objFile.GetValueAsFloat("Angle", 0));

            // Point gauge measurement setting
            m_objPointGauge.TransitionType = (ETransitionType)(objFile.GetValueAsInt("TransType", 0));
            m_objPointGauge.TransitionChoice = (ETransitionChoice)(objFile.GetValueAsInt("TransChoice", 0));
#if (Debug_2_12 || Release_2_12)
            m_objPointGauge.Threshold = objFile.GetValueAsUInt("Threshold", 2);
            m_objPointGauge.Thickness = objFile.GetValueAsUInt("Thickness", 13);
            m_objPointGauge.MinAmplitude = objFile.GetValueAsUInt("MinAmp", 10);
            m_objPointGauge.MinArea = objFile.GetValueAsUInt("MinArea", 0);
            m_objPointGauge.Smoothing = objFile.GetValueAsUInt("Filter", 1);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_objPointGauge.Threshold = objFile.GetValueAsInt("Threshold", 2);
            m_objPointGauge.Thickness = objFile.GetValueAsInt("Thickness", 13);
            m_objPointGauge.MinAmplitude = objFile.GetValueAsInt("MinAmp", 10);
            m_objPointGauge.MinArea = objFile.GetValueAsInt("MinArea", 0);
            m_objPointGauge.Smoothing = objFile.GetValueAsInt("Filter", 1);
#endif

        }
        public void LoadPointGauge(string strPath, string strSectionName, string strSectionName_Prev)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection(strSectionName);

            XmlParser objFile_Prev = new XmlParser(strPath);
            objFile_Prev.GetFirstSection(strSectionName_Prev);

            // Point gauge position setting
            m_objPointGauge.SetCenterXY(objFile.GetValueAsFloat("CenterX", objFile_Prev.GetValueAsFloat("CenterX", 0)), objFile.GetValueAsFloat("CenterY", objFile_Prev.GetValueAsFloat("CenterY", 0)));
            m_objPointGauge.SetTolerances(objFile.GetValueAsFloat("Tolerance", objFile_Prev.GetValueAsFloat("Tolerance", 10)), objFile.GetValueAsFloat("Angle", objFile_Prev.GetValueAsFloat("Angle", 0)));

            // Point gauge measurement setting
            m_objPointGauge.TransitionType = (ETransitionType)(objFile.GetValueAsInt("TransType", objFile_Prev.GetValueAsInt("TransType", 0)));
            m_objPointGauge.TransitionChoice = (ETransitionChoice)(objFile.GetValueAsInt("TransChoice", objFile_Prev.GetValueAsInt("TransChoice", 0)));
#if (Debug_2_12 || Release_2_12)
            m_objPointGauge.Threshold = objFile.GetValueAsUInt("Threshold", objFile_Prev.GetValueAsUInt("Threshold", 2));
            m_objPointGauge.Thickness = objFile.GetValueAsUInt("Thickness", objFile_Prev.GetValueAsUInt("Thickness", 13));
            m_objPointGauge.MinAmplitude = objFile.GetValueAsUInt("MinAmp", objFile_Prev.GetValueAsUInt("MinAmp", 10));
            m_objPointGauge.MinArea = objFile.GetValueAsUInt("MinArea", objFile_Prev.GetValueAsUInt("MinArea", 0));
            m_objPointGauge.Smoothing = objFile.GetValueAsUInt("Filter", objFile_Prev.GetValueAsUInt("Filter", 1));
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_objPointGauge.Threshold = objFile.GetValueAsInt("Threshold", objFile_Prev.GetValueAsInt("Threshold", 2));
            m_objPointGauge.Thickness = objFile.GetValueAsInt("Thickness", objFile_Prev.GetValueAsInt("Thickness", 13));
            m_objPointGauge.MinAmplitude = objFile.GetValueAsInt("MinAmp", objFile_Prev.GetValueAsInt("MinAmp", 10));
            m_objPointGauge.MinArea = objFile.GetValueAsInt("MinArea", objFile_Prev.GetValueAsInt("MinArea", 0));
            m_objPointGauge.Smoothing = objFile.GetValueAsInt("Filter", objFile_Prev.GetValueAsInt("Filter", 1));
#endif

        }
        public void EnableManualDrag()
        {
            m_objPointGauge.Dragable = true;
            m_objPointGauge.Rotatable = true;
            m_objPointGauge.Resizable = true;
        }

        public void DisableManualDrag()
        {
            m_objPointGauge.Dragable = false;
            m_objPointGauge.Rotatable = false;
            m_objPointGauge.Resizable = false;
        }

        //public bool VerifyGaugeArea()
        //{
        //    bool m_Handler = m_objPointGauge.HitTest();
        //    if (m_Handler)
        //    {
        //    }

        //    EDragHandle dh = m_objPointGauge.HitHandle;

        //    return m_Handler;
        //}

        public bool VerifyGaugeArea(int nNewXPoint, int nNewYPoint)
        {
            m_objPointGauge.SetCursor(nNewXPoint, nNewYPoint);

            //m_objPointGauge.HitHandle

            m_Handler = m_objPointGauge.HitTest(true);

            return m_Handler;
        }

        public void ClearDragHandle()
        {
            if (m_Handler)
                m_Handler = false;
        }

        public void DragGauge(int nNewPositionX, int nNewPositionY)
        {
            //m_objPointGauge.HitTest(true);
            EDragHandle dh = m_objPointGauge.HitHandle;
            if (dh != EDragHandle.NoHandle)
            {
            }
            m_objPointGauge.Drag(nNewPositionX, nNewPositionY);
        }

        public void Dispose()
        {
            m_BrushMatched.Dispose();

            if (m_objPointGauge != null)
                m_objPointGauge.Dispose();
        }
    }
}
