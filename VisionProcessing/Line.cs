using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace VisionProcessing
{
    public class Line
    {
        #region Member Variables

        private bool m_blnCalculated = false;
        private double m_dA; // y-intercept
        private double m_dB; // Slope
        private double m_dInterceptX; // x-intercept
        private double m_dAngle;    // 0 degree start from straight line y

        #endregion

        #region Properties

        public double ref_dInterceptY { get { return m_dA; } }
        public double ref_dInterceptX { get { return m_dInterceptX; } }
        public double ref_dSlope { get { return m_dB; } }
        public double ref_dAngle { get { return m_dAngle; } }

        #endregion

        public Line()
        {
        }



        /// <summary>
        /// Get coordinate-x in straight line when coordinate-y is given
        /// </summary>
        /// <param name="fPointY">Coordinate-y</param>
        /// <returns></returns>
        public float GetPointX(float fPointY)
        {
            /*
             * Formula: x= (y - A) / B
             */

            if (double.IsInfinity(m_dB) || double.IsNaN(m_dB))
                return (float)m_dInterceptX;
            else
                return (float)((fPointY - m_dA) / m_dB);
        }
        /// <summary>
        /// Get coordinate-y in straight line when coordinate-x is given
        /// </summary>
        /// <param name="fPointX">Coordinate-x</param>
        /// <returns></returns>
        public float GetPointY(float fPointX)
        {
            /*
             * Formula: y = A + Bx
             */

            return (float)(m_dA + m_dB * fPointX);
        }

        /// <summary>
        ///  Draw line
        /// </summary>
        /// <param name="g">Graphic</param>
        /// <param name="intWidthLimit">Draw width limit</param>
        /// <param name="intHeightLimit">Draw height limit</param>
        /// <param name="objColor">Line color</param>
        /// <param name="intLineWidth">Line width</param>
        public void DrawLine(Graphics g, int intWidthLimit, int intHeightLimit, Color objColor, int intLineWidth)
        {
            //if (!m_blnCalculated)
            //    return;

            if (double.IsInfinity(m_dB) || double.IsNaN(m_dB))
                g.DrawLine(new Pen(objColor, intLineWidth), new Point((int)GetPointX(0), 0), new Point((int)GetPointX(intHeightLimit), intHeightLimit));
            else
            {
                float fStartPointX = 0;
                float fStartPointY = GetPointY(0);
                if (fStartPointY < 0)
                {
                    fStartPointY = 0;
                    fStartPointX = GetPointX(0);
                }
                else if (fStartPointY >= 480)
                {
                    fStartPointY = 480;
                    fStartPointX = GetPointX(480);
                }
                float fEndPointX = 640;
                float fEndPointY = GetPointY(intWidthLimit);
                if (fEndPointY > 480)
                {
                    fEndPointY = 480;
                    fEndPointX = GetPointX(480);
                }
                g.DrawLine(new Pen(objColor, intLineWidth), new Point((int)fStartPointX, (int)fStartPointY), new Point((int)fEndPointX, (int)fEndPointY));
                //g.DrawLine(new Pen(objColor, intLineWidth), new Point(0, (int)GetPointY(0)), new Point(intWidthLimit, (int)GetPointY(intWidthLimit)));
            }
        }

        public void DrawLine(Graphics g, int intStartDrawX, int intStartDrawY, int intWidthLimit, int intHeightLimit, Color objColor, int intLineWidth)
        {
            //if (!m_blnCalculated)
            //    return;

            if (double.IsInfinity(m_dB) || double.IsNaN(m_dB))
                g.DrawLine(new Pen(objColor, intLineWidth), new Point((int)GetPointX(0) + intStartDrawX, 0 + intStartDrawY), new Point((int)GetPointX(intHeightLimit) + intStartDrawX, intHeightLimit + intStartDrawY));
            else
            {
                float fStartPointX = 0;
                float fStartPointY = GetPointY(0);
                if (fStartPointY < 0)
                {
                    fStartPointY = 0;
                    fStartPointX = GetPointX(0);
                }
                else if (fStartPointY >= 480)
                {
                    fStartPointY = 480;
                    fStartPointX = GetPointX(480);
                }
                float fEndPointX = 640;
                float fEndPointY = GetPointY(intWidthLimit);
                if (fEndPointY > 480)
                {
                    fEndPointY = 480;
                    fEndPointX = GetPointX(480);
                }
                g.DrawLine(new Pen(objColor, intLineWidth), new Point((int)fStartPointX + intStartDrawX, (int)fStartPointY + intStartDrawY), new Point((int)fEndPointX + intStartDrawX, (int)fEndPointY + intStartDrawY));
            }
        }
        /// <summary>
        /// Draw line
        /// </summary>
        /// <param name="g">Graphic</param>
        /// <param name="intStartPointX">Start point line X</param>
        /// <param name="intEndPointX">End point line X</param>
        /// <param name="objColor">Line color</param>
        /// <param name="intLineWidth">Line width</param>
        public void DrawLineByPoints(Graphics g, int intStartPointX, int intEndPointX, Color objColor, int intLineWidth)
        {
            if (!m_blnCalculated)
                return;

            g.DrawLine(new Pen(objColor, intLineWidth), new Point(intStartPointX, (int)GetPointY(intStartPointX)), new Point(intEndPointX, (int)GetPointY(intEndPointX)));
        }

        public void DrawLineByPoints(Graphics g, float fDrawingScaleX, float fDrawingScaleY, float fStartPointX, float fEndPointX, Color objColor, int intLineWidth)
        {
            if (!m_blnCalculated)
                return;

            g.DrawLine(new Pen(objColor, intLineWidth), 
                new Point((int)Math.Round(fStartPointX * fDrawingScaleX, 0, MidpointRounding.AwayFromZero), 
                          (int)Math.Round(GetPointY(fStartPointX) * fDrawingScaleY, 0, MidpointRounding.AwayFromZero)), 
                new Point((int)Math.Round(fEndPointX * fDrawingScaleX, 0, MidpointRounding.AwayFromZero), 
                          (int)Math.Round(GetPointY(fEndPointX) * fDrawingScaleY, 0, MidpointRounding.AwayFromZero)));
        }

        /// <summary>
        /// Calculate straight line based on given points
        /// </summary>
        /// <param name="arrPoints">Points in array (Minimum 2 points)</param>
        public void CalculateStraightLine(PointF[] arrPoints)
        {
            /*
             * Using Least Squares formula
             */

            int intCount = arrPoints.Length;
            float fMaxX = 0;
            float fMaxY = 0;
            float fMinX = float.MaxValue;
            float fMinY = float.MaxValue;
            float fXX = 0;
            float fYY = 0;
            float fXY = 0;
            double dSumX = 0;
            double dSumY = 0;
            double dSumXX = 0;
            double dSumYY = 0;
            double dSumXY = 0;
            double dMeanX = 0;
            double dMeanY = 0;

            for (int i = 0; i < intCount; i++)
            {
                fMaxX = Math.Max(fMaxX, arrPoints[i].X);
                fMinX = Math.Min(fMinX, arrPoints[i].X);
                fMaxY = Math.Max(fMaxY, arrPoints[i].Y);
                fMinY = Math.Min(fMinY, arrPoints[i].Y);
                fXX = arrPoints[i].X * arrPoints[i].X;
                fYY = arrPoints[i].Y * arrPoints[i].Y;
                fXY = arrPoints[i].X * arrPoints[i].Y;
                dSumX += (double)arrPoints[i].X;
                dSumY += (double)arrPoints[i].Y;
                dSumXX += (double)fXX;
                dSumYY += (double)fYY;
                dSumXY += (double)fXY;
            }

            dMeanX = dSumX / intCount;
            dMeanY = dSumY / intCount;

            // Get "Change in x" and "Change in y"
            double dXDelta = (dSumXX - (double)intCount * dMeanX * dMeanX);
            double dYDelta = (dSumYY - (double)intCount * dMeanY * dMeanY);

            // Using Least Squares Fitting to get linear Formula y = A + Bx where A = y-intercept and B = slope 
            // Get y-intercept
            m_dA = (dMeanY * dSumXX - dMeanX * dSumXY) / dXDelta;

            // Get slope 
            m_dB = (dSumXY - (double)intCount * dMeanX * dMeanY) / dXDelta;

            if (double.IsInfinity(m_dA) || double.IsNaN(m_dA))
            {
                m_dInterceptX = dMeanX;
            }
            else
            {
                //double yyy = m_dA + m_dB * 198;
                //double xxx = (424 - m_dA) / m_dB;

                //Check coordinate y error after define linear line
                double dErrorY = 0;
                for (int i = 0; i < intCount; i++)
                {
                    dErrorY += Math.Abs(arrPoints[i].Y - GetPointY(arrPoints[i].X));
                }
                dErrorY = dErrorY / intCount;

                // if "Change in y" higher than "Change in x"
                if ((dErrorY > 5) && (dYDelta > dXDelta))
                {
                    // Using Least Squares Fitting to get linear Formula y = A + Bx (X-View)

                    // Get x-intercept
                    m_dInterceptX = (dMeanX * dSumYY - dMeanY * dSumXY) / dYDelta;

                    // Get slope by X-View
                    m_dB = (dSumXY - intCount * dMeanX * dMeanY) / dYDelta;

                    // Turn slope to Y-View
                    m_dB = 1 / m_dB;

                    // Get y-intercept (y-intercept = B * x-intercept where y == 0)
                    m_dA = -m_dB * m_dInterceptX;

                    //double yy = m_dA + m_dB * 198;
                    //double xx = (424 - m_dA) / m_dB;
                }
            }

            // Get angle where 0 degree start from straight line x
            double dAngle = Math.Atan(m_dB) * 180 / Math.PI;

            if (double.IsInfinity(m_dB) || double.IsNaN(m_dB))
            {
                m_dAngle = 0;
            }
            else if (m_dB <= 0)
            {
                m_dAngle = 90 + dAngle;
            }
            else
            {
                m_dAngle = Math.Abs(dAngle) - 90;
            }

            m_blnCalculated = true;
        }
        /// <summary>
        /// Calculate straight line based on given 2 points
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        public void CalculateStraightLine(Point p1, Point p2)
        {
            /* 
             * Formula: y = A + Bx where A = y-intercept and B= slope
             * Tangent value = slope
             */

            // Get slope
            m_dB = (double)((p1.Y - p2.Y) / (p1.X - p2.X));

            // Get y-intercept
            m_dA = p1.Y - m_dB * p1.X;

            // Get angle where 0 degree start from straight line x
            double dAngle = Math.Atan(m_dB) * 180 / Math.PI;

            // Turn angle so that 0 degree start from straight line y
            if (double.IsInfinity(m_dB) || double.IsNaN(m_dB))
            {
                m_dInterceptX = (double)((p1.X + p2.X) / 2);
                m_dAngle = 0;
            }
            else if (m_dB <= 0)
            {
                m_dAngle = 90 + dAngle;
            }
            else
            {
                m_dAngle = Math.Abs(dAngle) - 90;
            }

            m_blnCalculated = true;
            //double yy = m_dA + m_dB * p2.X;
        }
        /// <summary>
        /// Calculate straight line based on given 2 points
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        public void CalculateStraightLine(PointF p1, PointF p2)
        {
            /* 
             * Formula: y = A + Bx where A = y-intercept and B= slope
             * Tangent value = slope
             */

            // Get slope
            m_dB = (double)((p1.Y - p2.Y) / (p1.X - p2.X));

            // Get y-intercept
            m_dA = p1.Y - m_dB * p1.X;

            // Get angle where 0 degree start from straight line x
            double dAngle = Math.Atan(m_dB) * 180 / Math.PI;

            // Turn angle so that 0 degree start from straight line y
            if (double.IsInfinity(m_dB) || double.IsNaN(m_dB))
            {
                m_dInterceptX = (double)((p1.X + p2.X) / 2);
                m_dAngle = 0;
            }
            else if (m_dB <= 0)
            {
                m_dAngle = 90 + dAngle;
            }
            else
            {
                m_dAngle = Math.Abs(dAngle) - 90;
            }

            m_blnCalculated = true;
            //double yy = m_dA + m_dB * p2.X;
        }
        /// <summary>
        /// Calculate straight line based on given point and angle
        /// </summary>
        /// <param name="p">Point</param>
        /// <param name="dAngle">straight line angle</param>
        public void CalculateStraightLine(PointF p, double dAngle)
        {
            /* 
             * Angle (in radian) = angle (in degree) * PI / 180
             * Formula: y = A + Bx where A = y-intercept and B= slope
             * Tangent value = slope
             */
            m_dAngle = dAngle;

            // Turn angle so that 0 degree start from straight line x
            if ((dAngle >= 0) || (dAngle < -90))
                dAngle = 90 - Math.Abs(dAngle);
            else
                dAngle = Math.Abs(dAngle) - 90;

            // slope change to negative because image coordinate-y is reversed from graph coordinate-y
            m_dB = -Math.Tan(dAngle * Math.PI / 180);

            m_dA = p.Y - m_dB * p.X;
            
            m_blnCalculated = true;
        }

        public void CalculateStraightLine_ForLead3D(PointF p, double dAngle)
        {
            /* 
             * Angle (in radian) = angle (in degree) * PI / 180
             * Formula: y = A + Bx where A = y-intercept and B= slope
             * Tangent value = slope
             */
            m_dAngle = dAngle;

            // Turn angle so that 0 degree start from straight line x
            if ((dAngle >= 0) && (dAngle <= 90))
                dAngle = 90 - Math.Abs(dAngle);
            else if ((dAngle > 90) && (dAngle <= 180))
                dAngle = dAngle - 90;
            else if ((dAngle < 0) && (dAngle >= -90))
                dAngle = -(90 - Math.Abs(dAngle));
            else if ((dAngle < -90) && (dAngle >= -180))
                dAngle = dAngle + 90;

            // slope change to negative because image coordinate-y is reversed from graph coordinate-y
            m_dB = -Math.Tan(dAngle * Math.PI / 180);

            m_dA = p.Y - m_dB * p.X;
            
            m_blnCalculated = true;
        }

        public void CalculateLGStraightLine(PointF p1, PointF p2)
        {
            /* 
             * Formula: y = A + Bx where A = y-intercept and B= slope
             * Tangent value = slope
             */

            // Get slope
            m_dB = (double)((p1.Y - p2.Y) / (p1.X - p2.X));

            // Get y-intercept
            m_dA = p1.Y - m_dB * p1.X;

            // Get angle where 0 degree start from straight line x
            double dAngle = Math.Atan(m_dB) * 180 / Math.PI;

            // Turn angle so that 0 degree start from straight line y
            if (double.IsInfinity(m_dB) || double.IsNaN(m_dB))
            {
                m_dInterceptX = (double)((p1.X + p2.X) / 2);
                m_dAngle = 0;//2021-10-13 ZJYEOH : Actualy should be 90, but need some verification
            }
            else
            {
                m_dAngle = dAngle;
            }

            m_blnCalculated = true;
            //double yy = m_dA + m_dB * p2.X;
        }

        public void CalculateLGStraightLine(PointF p, double dAngle)
        {
            /* 
             * Angle (in radian) = angle (in degree) * PI / 180
             * Formula: y = A + Bx where A = y-intercept and B= slope
             * Tangent value = slope
             */
            m_dAngle = dAngle;

            m_dB = Math.Tan(dAngle * Math.PI / 180);

            m_dA = p.Y - m_dB * p.X;
            
            m_blnCalculated = true;
        }

        public void CopyTo(ref Line objLine)
        {
            objLine.m_dA = m_dA;
            objLine.m_dAngle = m_dAngle;
            objLine.m_dB = m_dB;
            objLine.m_dInterceptX = m_dInterceptX;
        }

        /*    public void ShiftXLine(double dShiftXValue)
          {
              // Horizontal line
              if (double.IsInfinity(m_dA) || double.IsNaN(m_dA))  // y-inception is infinity, x = 0; 
              {
                  m_dInterceptX += dShiftXValue;
              }
              // Vertical line
              else if (double.IsInfinity(m_dB) || double.IsNaN(m_dB))
              {
                  return;

              }
              else
              {
                  double y = m_dB * 0 + m_dA;

                  m_dA = y - (m_dB * dShiftXValue);
              }
              m_blnCalculated = true;
          }

        public void ShiftYLine(double dShiftYValue)
          {
              // Horizontal line
              if (double.IsInfinity(m_dA) || double.IsNaN(m_dA))
              {
                  m_dA += dShiftYValue;
              }
              // Vertical line
              else if (double.IsInfinity(m_dB) || double.IsNaN(m_dB))
              {
                  return;
              }
              else if (m_dB == 0)
              {
                  m_dA += dShiftYValue;
              }
              else
              {
                  double x = -m_dA / m_dB;

                  m_dA = dShiftYValue - (m_dB * x);
              }
          }*/

        public void ShiftXLine(double dShiftXValue)
        {
            // m = y/x
            double dShiftYValue = m_dB * dShiftXValue;
            ShiftYLine(dShiftYValue);

            m_dInterceptX = -m_dA / m_dB;
            m_blnCalculated = true;
        }

        public void ShiftXLine2(double dShiftXValue)
        {
            // m = y/x
            double dShiftYValue = m_dB * dShiftXValue;
            ShiftYLine(-dShiftYValue);

            m_dInterceptX = -m_dA / m_dB;
            m_blnCalculated = true;
        }

        public void ShiftYLine(double dShiftYValue)
        {
            m_dA += dShiftYValue;
                m_blnCalculated = true;
        }

        public void RotateLine(float dRotateAngle, PointF pRotatedCenterPoint)
        {
            /* 
             * Formula: y = A + Bx where A = y-intercept and B= slope
             * Tangent value = slope
             */


        }

        /// <summary>
        /// Get cross point between 2 lines
        /// </summary>
        /// <param name="L1">Line 1</param>
        /// <param name="L2">Line 2</param>
        /// <returns></returns>
        public static PointF GetCrossPoint(Line L1, Line L2)
        {
            PointF p = new PointF();
            if (double.IsInfinity(L2.ref_dInterceptY) || double.IsNaN(L2.ref_dInterceptY))
            {
                p.X = (float)(L2.ref_dInterceptX);
                p.Y = (float)(L1.ref_dInterceptY + L1.ref_dSlope * p.X);
            }
            else if (double.IsInfinity(L1.ref_dInterceptY) || double.IsNaN(L1.ref_dInterceptY))
            {
                p.X = (float)(L1.ref_dInterceptX);
                p.Y = (float)(L2.ref_dInterceptY + L2.ref_dSlope * p.X);
            }
            else
            {
                p.X = (float)((L2.ref_dInterceptY - L1.ref_dInterceptY) / (L1.ref_dSlope - L2.ref_dSlope));
                if (Math.Abs(L1.ref_dSlope) < Math.Abs(L2.ref_dSlope))
                    p.Y = (float)(L1.ref_dInterceptY + L1.ref_dSlope * p.X);
                else
                    p.Y = (float)(L2.ref_dInterceptY + L2.ref_dSlope * p.X);

            }

            return p;
        }
    }


}
