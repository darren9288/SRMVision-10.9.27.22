using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Common;

namespace VisionProcessing
{
    public class LineROI
    {
        #region Member Variables
        private int m_intRoundingTol = 1;
        private bool m_bStartPointHandlerON = false; 
        private bool m_bEndPointHandlerON = false;
        private bool m_bCenterPointhandlerON = false;
        private float m_fAngle = 0;
        private int m_intDragBoxTol = 6;
        private PointF m_pCenterPoint = new PointF();
        private PointF m_pStartPoint = new PointF();
        private PointF m_pEndPoint = new PointF();
        private PointF m_pHitPoint = new PointF();
        private Color m_cCyan = Color.Cyan;
        private Pen m_pCyan = new Pen(Color.Cyan, 1);


        private int m_intcount = 0;
        private List<PointF> m_pStartPoint2 = new List<PointF>();
        private List<PointF> m_pCenterPoint2 = new List<PointF>();
        private List<PointF> m_pEndPoint2 = new List<PointF>();
        private List<float> m_arrInwardStartPercent = new List<float>();
        private List<float> m_arrInwardEndPercent = new List<float>();
        //private List<PointF> m_pHitPoint2 = new List<PointF>();
        private PointF m_pHitPoint2 = new PointF();
        #endregion

        #region Properties

        public PointF ref_pCenterPoint { get { return m_pCenterPoint; } set { m_pCenterPoint = value; } }
        public PointF ref_pStartPoint { get { return m_pStartPoint; } set { m_pStartPoint = value; } }
        public PointF ref_pEndPoint { get { return m_pEndPoint; } set { m_pEndPoint = value; } }

        public List<PointF> ref_pCenterPoint2 { get { return m_pCenterPoint2; } set { m_pCenterPoint2 = value; } }
        public List<PointF> ref_pStartPoint2 { get { return m_pStartPoint2; } set { m_pStartPoint2 = value; } }
        public List<PointF> ref_pEndPoint2 { get { return m_pEndPoint2; } set { m_pEndPoint2 = value; } }
        public PointF ref_pHitPoint2 { get { return m_pHitPoint2; } set { m_pHitPoint2 = value; } }
        #endregion

        public LineROI()
        {

        }


        public void ClearDragHandler()
        {
            m_bCenterPointhandlerON = false;
            m_bEndPointHandlerON = false;
            m_bStartPointHandlerON = false;
        }

        public void DrawLine(Graphics g, float fDrawingScaleX, float fDrawingScaleY, Color objColor)
        {
            float fStartX = m_pStartPoint.X * fDrawingScaleX;
            float fStartY = m_pStartPoint.Y * fDrawingScaleY;
            float fEndX = m_pEndPoint.X * fDrawingScaleX;
            float fEndY = m_pEndPoint.Y * fDrawingScaleY;
            g.DrawLine(m_pCyan, fStartX, fStartY, fEndX, fEndY);

            float fLongestDistance = Math.Max(Math.Abs(fStartX - fEndX), Math.Abs(fStartY - fEndY));
            m_intDragBoxTol = (int)Math.Ceiling(Math.Min(fLongestDistance / 5f, 10));

            g.FillRectangle(new SolidBrush(objColor), fStartX - m_intDragBoxTol, fStartY - m_intDragBoxTol, m_intDragBoxTol * 2, m_intDragBoxTol * 2);

            g.FillRectangle(new SolidBrush(objColor), fEndX - m_intDragBoxTol, fEndY - m_intDragBoxTol, m_intDragBoxTol * 2, m_intDragBoxTol * 2);
        }

        public void Drag(ROI objROI, float fX, float fY)
        {
            if (m_bCenterPointhandlerON)
            {
                // Get distance move compare to previous
                float fMoveX = fX - m_pHitPoint.X;
                float fMoveY = fY - m_pHitPoint.Y;

                // Check will start point and endpoint out of image position?
                if ((m_pStartPoint.X + fMoveX) < objROI.ref_ROITotalX)
                {
                    fMoveX = objROI.ref_ROITotalX - m_pStartPoint.X;
                }
                else if ((m_pStartPoint.X + fMoveX) >= (objROI.ref_ROITotalX + objROI.ref_ROIWidth))
                {
                    fMoveX = (objROI.ref_ROITotalX + objROI.ref_ROIWidth) - m_pStartPoint.X;
                }

                if ((m_pStartPoint.Y + fMoveY) < objROI.ref_ROITotalY)
                {
                    fMoveY = objROI.ref_ROITotalY - m_pStartPoint.Y;
                }
                else if ((m_pStartPoint.Y + fMoveY) >= (objROI.ref_ROITotalY + objROI.ref_ROIHeight))
                {
                    fMoveY = (objROI.ref_ROITotalY + objROI.ref_ROIHeight) - m_pStartPoint.Y;
                }

                // Check will start point and endpoint out of image position?
                if ((m_pEndPoint.X + fMoveX) < objROI.ref_ROITotalX)
                {
                    fMoveX = objROI.ref_ROITotalX - m_pEndPoint.X;
                }
                else if ((m_pEndPoint.X + fMoveX) >= (objROI.ref_ROITotalX + objROI.ref_ROIWidth))
                {
                    fMoveX = (objROI.ref_ROITotalX + objROI.ref_ROIWidth) - m_pEndPoint.X;
                }

                if ((m_pEndPoint.Y + fMoveY) < objROI.ref_ROITotalY)
                {
                    fMoveY = objROI.ref_ROITotalY - m_pEndPoint.Y;
                }
                else if ((m_pEndPoint.Y + fMoveY) >= (objROI.ref_ROITotalY + objROI.ref_ROIHeight))
                {
                    fMoveY = (objROI.ref_ROITotalY + objROI.ref_ROIHeight) - m_pEndPoint.Y;
                }

                m_pStartPoint.X += fMoveX;
                m_pStartPoint.Y += fMoveY;
                m_pEndPoint.X += fMoveX;
                m_pEndPoint.Y += fMoveY;
                m_pHitPoint.X += fMoveX;
                m_pHitPoint.Y += fMoveY;
            }
            else if (m_bStartPointHandlerON)
            {
                // Get distance move compare to previous
                float fMoveX = fX - m_pHitPoint.X;
                float fMoveY = fY - m_pHitPoint.Y;

                // Check will start point and endpoint out of image position?
                if ((m_pStartPoint.X + fMoveX) < objROI.ref_ROITotalX)
                {
                    fMoveX = objROI.ref_ROITotalX - m_pStartPoint.X;
                }
                else if ((m_pStartPoint.X + fMoveX) >= (objROI.ref_ROITotalX + objROI.ref_ROIWidth))
                {
                    fMoveX = (objROI.ref_ROITotalX + objROI.ref_ROIWidth) - m_pStartPoint.X;
                }

                if ((m_pStartPoint.Y + fMoveY) < objROI.ref_ROITotalY)
                {
                    fMoveY = objROI.ref_ROITotalY - m_pStartPoint.Y;
                }
                else if ((m_pStartPoint.Y + fMoveY) >= (objROI.ref_ROITotalY + objROI.ref_ROIHeight))
                {
                    fMoveY = (objROI.ref_ROITotalY + objROI.ref_ROIHeight) - m_pStartPoint.Y;
                }

                // Check will start point and endpoint out of image position?
                if ((m_pEndPoint.X + fMoveX) < objROI.ref_ROITotalX)
                {
                    fMoveX = objROI.ref_ROITotalX - m_pEndPoint.X;
                }
                else if ((m_pEndPoint.X + fMoveX) >= (objROI.ref_ROITotalX + objROI.ref_ROIWidth))
                {
                    fMoveX = (objROI.ref_ROITotalX + objROI.ref_ROIWidth) - m_pEndPoint.X;
                }

                if ((m_pEndPoint.Y + fMoveY) < objROI.ref_ROITotalY)
                {
                    fMoveY = objROI.ref_ROITotalY - m_pEndPoint.Y;
                }
                else if ((m_pEndPoint.Y + fMoveY) >= (objROI.ref_ROITotalY + objROI.ref_ROIHeight))
                {
                    fMoveY = (objROI.ref_ROITotalY + objROI.ref_ROIHeight) - m_pEndPoint.Y;
                }

                m_pStartPoint.X += fMoveX;
                m_pStartPoint.Y += fMoveY;
                m_pHitPoint.X += fMoveX;
                m_pHitPoint.Y += fMoveY;
            }
            else if (m_bEndPointHandlerON)
            {
                // Get distance move compare to previous
                float fMoveX = fX - m_pHitPoint.X;
                float fMoveY = fY - m_pHitPoint.Y;

                // Check will start point and endpoint out of image position?
                if ((m_pStartPoint.X + fMoveX) < objROI.ref_ROITotalX)
                {
                    fMoveX = objROI.ref_ROITotalX - m_pStartPoint.X;
                }
                else if ((m_pStartPoint.X + fMoveX) >= (objROI.ref_ROITotalX + objROI.ref_ROIWidth))
                {
                    fMoveX = (objROI.ref_ROITotalX + objROI.ref_ROIWidth) - m_pStartPoint.X;
                }

                if ((m_pStartPoint.Y + fMoveY) < objROI.ref_ROITotalY)
                {
                    fMoveY = objROI.ref_ROITotalY - m_pStartPoint.Y;
                }
                else if ((m_pStartPoint.Y + fMoveY) >= (objROI.ref_ROITotalY + objROI.ref_ROIHeight))
                {
                    fMoveY = (objROI.ref_ROITotalY + objROI.ref_ROIHeight) - m_pStartPoint.Y;
                }

                // Check will start point and endpoint out of image position?
                if ((m_pEndPoint.X + fMoveX) < objROI.ref_ROITotalX)
                {
                    fMoveX = objROI.ref_ROITotalX - m_pEndPoint.X;
                }
                else if ((m_pEndPoint.X + fMoveX) >= (objROI.ref_ROITotalX + objROI.ref_ROIWidth))
                {
                    fMoveX = (objROI.ref_ROITotalX + objROI.ref_ROIWidth) - m_pEndPoint.X;
                }

                if ((m_pEndPoint.Y + fMoveY) < objROI.ref_ROITotalY)
                {
                    fMoveY = objROI.ref_ROITotalY - m_pEndPoint.Y;
                }
                else if ((m_pEndPoint.Y + fMoveY) >= (objROI.ref_ROITotalY + objROI.ref_ROIHeight))
                {
                    fMoveY = (objROI.ref_ROITotalY + objROI.ref_ROIHeight) - m_pEndPoint.Y;
                }

                m_pEndPoint.X += fMoveX;
                m_pEndPoint.Y += fMoveY;
                m_pHitPoint.X += fMoveX;
                m_pHitPoint.Y += fMoveY;
            }
                 

        }

        public void DragX(int intLimitMinX, int intLimitMaxX, float fX)
        {
            if (m_bStartPointHandlerON || m_bCenterPointhandlerON || m_bEndPointHandlerON)
            {
                // Get distance move compare to previous
                float fMoveX = fX - m_pHitPoint.X;

                // Check will start point and endpoint out of image position?
                if ((m_pStartPoint.X + fMoveX) < intLimitMinX)
                {
                    fMoveX = intLimitMinX - m_pStartPoint.X;
                }
                else if ((m_pStartPoint.X + fMoveX) >= intLimitMaxX)
                {
                    fMoveX = intLimitMaxX - m_pStartPoint.X;
                }

                // Check will start point and endpoint out of image position?
                if ((m_pEndPoint.X + fMoveX) < intLimitMinX)
                {
                    fMoveX = intLimitMinX - m_pEndPoint.X;
                }
                else if ((m_pEndPoint.X + fMoveX) >= intLimitMaxX)
                {
                    fMoveX = intLimitMaxX - m_pEndPoint.X;
                }

                m_pStartPoint.X += fMoveX;
                m_pEndPoint.X += fMoveX;
                m_pHitPoint.X += fMoveX;
            }
        }

        public void DragY(int intLimitMinY, int intLimitMaxY, float fY)
        {
            if (m_bStartPointHandlerON || m_bCenterPointhandlerON || m_bEndPointHandlerON)
            {
                // Get distance move compare to previous
                float fMoveY = fY - m_pHitPoint.Y;

                // Check will start point and endpoint out of image position?
                if ((m_pStartPoint.Y + fMoveY) < intLimitMinY)
                {
                    fMoveY = intLimitMinY - m_pStartPoint.Y;
                }
                else if ((m_pStartPoint.Y + fMoveY) >= intLimitMaxY)
                {
                    fMoveY = intLimitMaxY - m_pStartPoint.Y;
                }

                // Check will start point and endpoint out of image position?
                if ((m_pEndPoint.Y + fMoveY) < intLimitMinY)
                {
                    fMoveY = intLimitMinY - m_pEndPoint.Y;
                }
                else if ((m_pEndPoint.Y + fMoveY) >= intLimitMaxY)
                {
                    fMoveY = intLimitMaxY - m_pEndPoint.Y;
                }

                m_pStartPoint.Y += fMoveY;
                m_pEndPoint.Y += fMoveY;
                m_pHitPoint.Y += fMoveY;
            }
        }
        
        public bool HitTest(float fX, float fY)
        {
            if ((fX >= m_pStartPoint.X - m_intDragBoxTol) &&
                (fX <= m_pStartPoint.X + m_intDragBoxTol) &&
                (fY >= m_pStartPoint.Y - m_intDragBoxTol) &&
                (fY <= m_pStartPoint.Y + m_intDragBoxTol))
            {
                m_bStartPointHandlerON = true;
                m_pHitPoint = new PointF(fX, fY);

                return true;
            }

            if ((fX >= m_pEndPoint.X - m_intDragBoxTol) &&
                (fX <= m_pEndPoint.X + m_intDragBoxTol) &&
                (fY >= m_pEndPoint.Y - m_intDragBoxTol) &&
                (fY <= m_pEndPoint.Y + m_intDragBoxTol))
            {
                m_bEndPointHandlerON = true;
                m_pHitPoint = new PointF(fX, fY);

                return true;
            }

            if (Math.Abs(m_pStartPoint.X - m_pEndPoint.X) < Math.Abs(m_pStartPoint.Y - m_pEndPoint.Y))
            {
                Line L1 = new Line();
                L1.CalculateStraightLine(new PointF(m_pStartPoint.X - (float)m_intDragBoxTol, m_pStartPoint.Y),
                                         new PointF(m_pEndPoint.X - (float)m_intDragBoxTol, m_pEndPoint.Y));
                Line L2 = new Line();
                L2.CalculateStraightLine(new PointF(m_pStartPoint.X + (float)m_intDragBoxTol, m_pStartPoint.Y),
                                         new PointF(m_pEndPoint.X + (float)m_intDragBoxTol, m_pEndPoint.Y));

                Line L3 = new Line();
                L3.CalculateStraightLine(new PointF(fX, fY), 90);

                PointF pCross1 = Line.GetCrossPoint(L1, L3);
                PointF pCross2 = Line.GetCrossPoint(L2, L3);

                float fStartX = Math.Min(pCross1.X, pCross2.X);
                float fEndX = Math.Max(pCross1.X, pCross2.X);
                float fStartY = Math.Min(m_pStartPoint.Y, m_pEndPoint.Y);
                float fEndY = Math.Max(m_pStartPoint.Y, m_pEndPoint.Y);

                if ((fX >= fStartX) &&
                (fX <= fEndX) &&
                (fY >= fStartY) &&
                (fY <= fEndY))
                {
                    m_bCenterPointhandlerON = true;
                    m_pHitPoint = new PointF(fX, fY);

                    return true;
                }
            }
            else
            {
                Line L1 = new Line();
                L1.CalculateStraightLine(new PointF(m_pStartPoint.X, m_pStartPoint.Y - (float)m_intDragBoxTol),
                                         new PointF(m_pEndPoint.X, m_pEndPoint.Y - (float)m_intDragBoxTol));
                Line L2 = new Line();
                L2.CalculateStraightLine(new PointF(m_pStartPoint.X, m_pStartPoint.Y + (float)m_intDragBoxTol),
                                         new PointF(m_pEndPoint.X, m_pEndPoint.Y + (float)m_intDragBoxTol));

                Line L3 = new Line();
                L3.CalculateStraightLine(new PointF(fX, fY), 0);

                PointF pCross1 = Line.GetCrossPoint(L1, L3);
                PointF pCross2 = Line.GetCrossPoint(L2, L3);

                float fStartY = Math.Min(pCross1.Y, pCross2.Y);
                float fEndY = Math.Max(pCross1.Y, pCross2.Y);
                float fStartX = Math.Min(m_pStartPoint.X, m_pEndPoint.X);
                float fEndX = Math.Max(m_pStartPoint.X, m_pEndPoint.X);

                if ((fX >= fStartX) &&
                (fX <= fEndX) &&
                (fY >= fStartY) &&
                (fY <= fEndY))
                {
                    m_bCenterPointhandlerON = true;
                    m_pHitPoint = new PointF(fX, fY);

                    return true;
                }
            }

            return false;
        }

        public void SetROIPlacement(float fStartX, float fStartY, float fEndX, float fEndY)
        {
            m_pStartPoint = new PointF(fStartX, fStartY);
            m_pEndPoint = new PointF(fEndX, fEndY);
            m_pCenterPoint = new PointF((fStartX + fEndX) / 2, (fStartY + fEndY) / 2);
        }

        public void Drag2(ROI objROI, float fX, float fY)
        {
            if (m_intcount >= 0)
            {
                int i = m_intcount;
                if (m_bCenterPointhandlerON)
                {
                    // Get distance move compare to previous
                    float fMoveX = fX - m_pHitPoint2.X;
                    float fMoveY = fY - m_pHitPoint2.Y;



                    // Check will start point and endpoint out of image position?
                    if ((m_pStartPoint2[i].X + fMoveX) < objROI.ref_ROIPositionX) //ref_ROITotalX
                    {
                        fMoveX = objROI.ref_ROIPositionX - m_pStartPoint2[i].X;
                    }
                    else if ((m_pStartPoint2[i].X + fMoveX) >= (objROI.ref_ROIPositionX + objROI.ref_ROIWidth))
                    {
                        fMoveX = (objROI.ref_ROIPositionX + objROI.ref_ROIWidth) - m_pStartPoint2[i].X;
                    }

                    if ((m_pStartPoint2[i].Y + fMoveY) < objROI.ref_ROIPositionY) //ref_ROITotalY
                    {
                        fMoveY = objROI.ref_ROIPositionY - m_pStartPoint2[i].Y;
                    }
                    else if ((m_pStartPoint2[i].Y + fMoveY) >= (objROI.ref_ROIPositionY + objROI.ref_ROIHeight))
                    {
                        fMoveY = (objROI.ref_ROIPositionY + objROI.ref_ROIHeight) - m_pStartPoint2[i].Y;
                    }

                    // Check will start point and endpoint out of image position?
                    if ((m_pEndPoint2[i].X + fMoveX) < objROI.ref_ROIPositionX) //ref_ROITotalX
                    {
                        fMoveX = objROI.ref_ROIPositionX - m_pEndPoint2[i].X;
                    }
                    else if ((m_pEndPoint2[i].X + fMoveX) >= (objROI.ref_ROIPositionX + objROI.ref_ROIWidth))
                    {
                        fMoveX = (objROI.ref_ROIPositionX + objROI.ref_ROIWidth) - m_pEndPoint2[i].X;
                    }

                    if ((m_pEndPoint2[i].Y + fMoveY) < objROI.ref_ROIPositionY) //ref_ROITotalY
                    {
                        fMoveY = objROI.ref_ROIPositionY - m_pEndPoint2[i].Y;
                    }
                    else if ((m_pEndPoint2[i].Y + fMoveY) >= (objROI.ref_ROIPositionY + objROI.ref_ROIHeight))
                    {
                        fMoveY = (objROI.ref_ROIPositionY + objROI.ref_ROIHeight) - m_pEndPoint2[i].Y;
                    }


                    if ((m_pStartPoint2[i].Y + fMoveY) > 0 && (m_pStartPoint2[i].Y + fMoveY) < objROI.ref_ROIHeight)
                    {
                        if ((m_pStartPoint2[i].X + fMoveX) > 0)
                            fMoveX = 0;
                    }
                    if ((m_pStartPoint2[i].X + fMoveX) > 0 && (m_pStartPoint2[i].X + fMoveX) < objROI.ref_ROIWidth)
                    {
                        if ((m_pStartPoint2[i].Y + fMoveY) > 0)
                            fMoveY = 0;
                    }
                    if ((m_pEndPoint2[i].Y + fMoveY) > 0 && (m_pEndPoint2[i].Y + fMoveY) < objROI.ref_ROIHeight)
                    {
                        if ((m_pEndPoint2[i].X + fMoveX) > 0)
                            fMoveX = 0;
                    }
                    if ((m_pEndPoint2[i].X + fMoveX) > 0 && (m_pEndPoint2[i].X + fMoveX) < objROI.ref_ROIWidth)
                    {
                        if ((m_pEndPoint2[i].Y + fMoveY) > 0)
                            fMoveY = 0;
                    }

                    STTrackLog.WriteLine("MoveX=" + fMoveX.ToString() + ", MoveY=" + fMoveY.ToString());
                    m_pStartPoint2[i] = new PointF(m_pStartPoint2[i].X + fMoveX, m_pStartPoint2[i].Y + fMoveY);
                    m_pEndPoint2[i] = new PointF(m_pEndPoint2[i].X + fMoveX, m_pEndPoint2[i].Y + fMoveY);
                    m_pHitPoint2 = new PointF(m_pHitPoint2.X + fMoveX, m_pHitPoint2.Y + fMoveY);
                    //m_pStartPoint2[i].X += fMoveX;
                    //m_pStartPoint2[i].Y += fMoveY;
                    // m_pEndPoint2[i].X += fMoveX;
                    // m_pEndPoint2[i].Y += fMoveY;
                    //m_pHitPoint2[i].X += fMoveX;
                    // m_pHitPoint2[i].Y += fMoveY;
                }
                else if (m_bStartPointHandlerON)
                {
                    // Get distance move compare to previous
                    float fMoveX = fX - m_pHitPoint2.X;
                    float fMoveY = fY - m_pHitPoint2.Y;
                    float fTempX = fMoveX;
                    float fTempY = fMoveY;

                    // Check will start point and endpoint out of image position?
                    if ((m_pStartPoint2[i].X + fMoveX) < objROI.ref_ROIPositionX) //ref_ROITotalX
                    {
                        fMoveX = objROI.ref_ROIPositionX - m_pStartPoint2[i].X;
                    }
                    else if ((m_pStartPoint2[i].X + fMoveX) >= (objROI.ref_ROIPositionX + objROI.ref_ROIWidth))
                    {
                        fMoveX = (objROI.ref_ROIPositionX + objROI.ref_ROIWidth) - m_pStartPoint2[i].X;
                    }

                    if ((m_pStartPoint2[i].Y + fMoveY) < objROI.ref_ROIPositionY) //ref_ROITotalY
                    {
                        fMoveY = objROI.ref_ROIPositionY - m_pStartPoint2[i].Y;
                    }
                    else if ((m_pStartPoint2[i].Y + fMoveY) >= (objROI.ref_ROIPositionY + objROI.ref_ROIHeight))
                    {
                        fMoveY = (objROI.ref_ROIPositionY + objROI.ref_ROIHeight) - m_pStartPoint2[i].Y;
                    }


                    //if ((m_pStartPoint2[i].Y + fMoveY) > 0 && (m_pStartPoint2[i].Y + fMoveY) < objROI.ref_ROIHeight)
                    if ((m_pStartPoint2[i].Y) > 0 && (m_pStartPoint2[i].Y) < objROI.ref_ROIHeight)
                    {
                        if ((m_pStartPoint2[i].X + fMoveX) >= 0)//|| (m_pStartPoint2[i].X + fMoveX) < objROI.ref_ROIWidth)
                            fMoveX = 0;

                        //if((m_pStartPoint2[i].Y + fMoveY) >= m_pEndPoint2[i].Y)
                        //    fMoveY=0;
                    }


                    if ((m_pStartPoint2[i].X + fMoveX) > 0 && (m_pStartPoint2[i].X + fMoveX) < objROI.ref_ROIWidth)
                    {
                        if ((m_pStartPoint2[i].Y + fMoveY) >= 0)// || (m_pStartPoint2[i].Y + fMoveY) < objROI.ref_ROIHeight)
                            fMoveY = 0;

                        //if((m_pStartPoint2[i].X + fMoveX) >= m_pEndPoint2[i].X)
                        //    fMoveX = 0;
                    }


                    // TrackLog objTL = new TrackLog();
                    // string str = m_pStartPoint2[i].X + ", " + +m_pStartPoint2[i].Y + ", " + m_pEndPoint2[i].X + ", " + m_pEndPoint2[i].Y + ", " + fMoveX + ", " + fMoveY;


                    if (fMoveY < 0)
                    {
                        // str += "-LY-";
                        if ((m_pStartPoint2[i].Y + fMoveY) <= m_pEndPoint2[i].Y && m_pEndPoint2[i].Y == 0)
                        {
                            //      str += "1";
                            fMoveY = 0;
                            fMoveX = 0;
                        }
                        // else
                        //    str += "0";
                    }
                    if (fMoveY >= 0)
                    {
                        //str += "-HY-";

                        if ((m_pStartPoint2[i].Y + fMoveY) >= (objROI.ref_ROIHeight) && m_pEndPoint2[i].Y == (objROI.ref_ROIHeight))
                        {
                            //     str += "1";
                            fMoveY = 0;
                            fMoveX = 0;
                        }
                        // else
                        //     str += "0";
                    }
                    if (fMoveX < 0)
                    {
                        //  str += "-LX-";

                        if ((m_pStartPoint2[i].X + fMoveX) <= m_pEndPoint2[i].X && m_pEndPoint2[i].X == 0)
                        {
                            //     str += "1";
                            fMoveX = 0;
                            fMoveY = 0;
                        }
                        //  else
                        //      str += "0";

                        if (m_pEndPoint2[i].Y == objROI.ref_ROIHeight && (m_pStartPoint2[i].X + fMoveX) <= objROI.ref_ROIWidth && m_pStartPoint2[i].Y >= objROI.ref_ROIHeight / 2)
                        {
                            fMoveX = 0;
                            fMoveY = 0;
                        }

                        if (m_pEndPoint2[i].X == 0 && m_pStartPoint2[i].Y != objROI.ref_ROIHeight && m_pStartPoint2[i].Y >= objROI.ref_ROIHeight / 2 && (m_pStartPoint2[i].X + fMoveX) <= objROI.ref_ROIWidth)
                        {
                            fMoveX = 0;
                        }

                        if (m_pEndPoint2[i].X == 0 && m_pStartPoint2[i].Y != 0 && m_pStartPoint2[i].Y <= objROI.ref_ROIHeight / 2 && (m_pStartPoint2[i].X + fMoveX) <= objROI.ref_ROIWidth)
                        {
                            fMoveX = 0;
                        }

                        if (m_pEndPoint2[i].Y == 0 && m_pStartPoint2[i].Y != objROI.ref_ROIHeight && m_pStartPoint2[i].Y >= objROI.ref_ROIHeight / 2 && (m_pStartPoint2[i].X + fMoveX) <= objROI.ref_ROIWidth)
                        {
                            fMoveX = 0;
                        }
                        if (m_pEndPoint2[i].Y == objROI.ref_ROIHeight && m_pStartPoint2[i].Y != 0 && m_pStartPoint2[i].Y <= objROI.ref_ROIHeight / 2 && (m_pStartPoint2[i].X + fMoveX) <= objROI.ref_ROIWidth)
                        {
                            fMoveX = 0;
                        }
                        //if (m_pEndPoint2[i].Y == objROI.ref_ROIHeight && ((m_pStartPoint2[i].Y == objROI.ref_ROIHeight - 1) || m_pStartPoint2[i].Y == objROI.ref_ROIHeight) && m_pStartPoint2[i].X != objROI.ref_ROIWidth)
                        //{
                        //    fMoveX = 0; fMoveY = 0;
                        //    m_pStartPoint2[i] = new PointF(objROI.ref_ROIWidth, m_pStartPoint2[i].Y);
                        //}
                    }
                    if (fMoveX > 0)
                    {
                        //  str += "-HX-";

                        if ((m_pStartPoint2[i].X + fMoveX) >= objROI.ref_ROIWidth && m_pEndPoint2[i].X == objROI.ref_ROIWidth)
                        {
                            //    str += "1";
                            fMoveX = 0;
                            fMoveY = 0;
                        }
                        // else
                        //     str += "0";

                        if (m_pEndPoint2[i].Y == objROI.ref_ROIHeight && (m_pStartPoint2[i].X + fMoveX) >= 0 && m_pStartPoint2[i].Y >= objROI.ref_ROIHeight / 2)
                        {
                            fMoveX = 0;
                            fMoveY = 0;
                        }

                        if (m_pEndPoint2[i].X == objROI.ref_ROIWidth && m_pStartPoint2[i].Y != objROI.ref_ROIHeight && m_pStartPoint2[i].Y >= objROI.ref_ROIHeight / 2 && (m_pStartPoint2[i].X + fMoveX) >= 0)
                        {
                            fMoveX = 0;
                        }

                        if (m_pEndPoint2[i].X == objROI.ref_ROIWidth && m_pStartPoint2[i].Y != 0 && m_pStartPoint2[i].Y <= objROI.ref_ROIHeight / 2 && (m_pStartPoint2[i].X + fMoveX) >= 0)
                        {
                            fMoveX = 0;
                        }

                        if (m_pEndPoint2[i].Y == 0 && m_pStartPoint2[i].Y != objROI.ref_ROIHeight && m_pStartPoint2[i].Y >= objROI.ref_ROIHeight / 2 && (m_pStartPoint2[i].X + fMoveX) >= 0)
                        {
                            fMoveX = 0;
                        }
                        if (m_pEndPoint2[i].Y == objROI.ref_ROIHeight && m_pStartPoint2[i].Y != 0 && m_pStartPoint2[i].Y <= objROI.ref_ROIHeight / 2 && (m_pStartPoint2[i].X + fMoveX) >= 0)
                        {
                            fMoveX = 0;
                        }
                        //if (m_pEndPoint2[i].Y == objROI.ref_ROIHeight && ((m_pStartPoint2[i].Y == objROI.ref_ROIHeight - 1 || m_pStartPoint2[i].Y == objROI.ref_ROIHeight)) && m_pStartPoint2[i].X != 0)
                        //{
                        //    m_pStartPoint2[i] = new PointF(0, m_pStartPoint2[i].Y); ;
                        //    fMoveX = 0;
                        //    fMoveY = 0;
                        //}
                    }

                    // objTL.WriteLine(str);
                    //if (m_pEndPoint2[i].Y == objROI.ref_ROIHeight)
                    //{
                    //    if (( ((m_pStartPoint2[i].Y + fMoveY) >= m_pEndPoint2[i].Y)) && (m_pStartPoint2[i].X == 0 || m_pStartPoint2[i].X == objROI.ref_ROIWidth))
                    //    {
                    //        fMoveY = 0;
                    //        //if ((m_pStartPoint2[i].X + fMoveX) > 0)
                    //        //    fMoveX = 0;
                    //    }
                    //}
                    //else if (m_pEndPoint2[i].Y ==0)
                    //{
                    //    if(((m_pStartPoint2[i].Y + fMoveY) <= m_pEndPoint2[i].Y) && (m_pStartPoint2[i].X == 0 || m_pStartPoint2[i].X == objROI.ref_ROIWidth))
                    //          fMoveY = 0;
                    //}

                    //if (m_pEndPoint2[i].X == objROI.ref_ROIWidth)
                    //{
                    //    if (( ((m_pStartPoint2[i].X + fMoveX) >= m_pEndPoint2[i].X)) && (m_pStartPoint2[i].Y == 0 || m_pStartPoint2[i].Y == objROI.ref_ROIHeight))
                    //    {
                    //        fMoveX = 0;
                    //        //if ((m_pStartPoint2[i].Y + fMoveY) > 0)
                    //        //    fMoveY = 0;
                    //    }
                    //}
                    //else if(m_pEndPoint2[i].X ==0)
                    //{
                    //    if ((((m_pStartPoint2[i].X + fMoveX) <= m_pEndPoint2[i].X)) && (m_pStartPoint2[i].Y == 0 || m_pStartPoint2[i].Y == objROI.ref_ROIHeight))
                    //    {
                    //        fMoveX = 0;
                    //    }
                    //}
                    // Check will start point and endpoint out of image position?
                    //if ((m_pEndPoint2[i].X + fMoveX) < objROI.ref_ROITotalX )
                    //{
                    //    fMoveX = objROI.ref_ROITotalX - m_pEndPoint2[i].X;
                    //}
                    //else if ((m_pEndPoint2[i].X + fMoveX) >= (objROI.ref_ROITotalX  + objROI.ref_ROIWidth))
                    //{
                    //    fMoveX = (objROI.ref_ROITotalX + objROI.ref_ROIWidth) - m_pEndPoint2[i].X;
                    //}

                    //if ((m_pEndPoint2[i].Y + fMoveY) < objROI.ref_ROITotalY )
                    //{
                    //    fMoveY = objROI.ref_ROITotalY - m_pEndPoint2[i].Y;
                    //}
                    //else if ((m_pEndPoint2[i].Y + fMoveY) >= (objROI.ref_ROITotalY  + objROI.ref_ROIHeight))
                    //{
                    //    fMoveY = (objROI.ref_ROITotalY + objROI.ref_ROIHeight) - m_pEndPoint2[i].Y;
                    //}


                    //if (m_pStartPoint2[i].X == 0 && m_pStartPoint2[i].Y == 0 && m_pEndPoint2[i].X == objROI.ref_ROIWidth && m_pEndPoint2[i].Y == objROI.ref_ROIHeight)
                    //{

                    // if (m_pStartPoint2[i].Y +fY <= objROI.ref_ROIHeight || m_pStartPoint2[i].Y -fY >= 0 )//&& m_pStartPoint2[i].X <= objROI.ref_ROIWidth && m_pStartPoint2[i].X >= 0)
                    // {
                    //fMoveX = fX;
                    // fMoveY = fTempY;
                    //    fMoveY += fY;
                    //    if (fMoveY < 0)
                    //        fMoveY = 0;
                    //    m_pStartPoint2[i] = new PointF(m_pStartPoint2[i].X + fMoveX, m_pStartPoint2[i].Y + fMoveY);
                    //    m_pHitPoint2 = new PointF(m_pHitPoint2.X + fMoveX, m_pHitPoint2.Y + fMoveY);

                    //}
                    // }
                    //else if (m_pStartPoint2[i].X == 0 && m_pEndPoint2[i].Y == 0 && m_pStartPoint2[i].X == objROI.ref_ROIWidth && m_pStartPoint2[i].Y == objROI.ref_ROIHeight)
                    //{

                    //    if (m_pEndPoint2[i].Y > 0)
                    //    {
                    //        fMoveY += fY;
                    //        if (fMoveY > objROI.ref_ROIWidth)
                    //            fMoveY = 0;
                    //        m_pStartPoint2[i] = new PointF(m_pStartPoint2[i].X + fMoveX, m_pStartPoint2[i].Y + fMoveY);
                    //        m_pHitPoint2 = new PointF(m_pHitPoint2.X + fMoveX, m_pHitPoint2.Y + fMoveY);

                    //    }
                    //}

                    //2021-06-21 ZJYEOH : Allow Drag Over for first two lines
                    //if (m_intcount == 0)
                    //{
                    //    if ((m_pStartPoint2[i].Y + fMoveY <= 0) || m_pStartPoint2[i].Y + fMoveY >= objROI.ref_ROIHeight)
                    //    {
                    //        fMoveX = 0;

                    //    }
                    //}
                    //if (m_intcount == 1)
                    //{
                    //    if (m_pStartPoint2[i].X + fMoveX <= 0 || m_pStartPoint2[i].X + fMoveX >= objROI.ref_ROIWidth)
                    //    {
                    //        fMoveY = 0;

                    //    }
                    //}

                    m_pStartPoint2[i] = new PointF(m_pStartPoint2[i].X + fMoveX, m_pStartPoint2[i].Y + fMoveY);
                    // m_pEndPoint2[i] = new PointF(m_pEndPoint2[i].X + fMoveX, m_pEndPoint2[i].Y + fMoveY);
                    m_pHitPoint2 = new PointF(m_pHitPoint2.X + fMoveX, m_pHitPoint2.Y + fMoveY);
                    //m_pStartPoint2[i].X += fMoveX;
                    //m_pStartPoint2[i].Y += fMoveY;
                    //m_pHitPoint2[i].X += fMoveX;
                    //m_pHitPoint2[i].Y += fMoveY;

                }
                else if (m_bEndPointHandlerON)
                {
                    string strTrack = "";
                    // Get distance move compare to previous
                    float fMoveX = fX - m_pHitPoint2.X;
                    float fMoveY = fY - m_pHitPoint2.Y;

                    // Check will start point and endpoint out of image position?
                    //if ((m_pStartPoint2[i].X + fMoveX) < objROI.ref_ROITotalX)
                    //{
                    //    fMoveX = objROI.ref_ROITotalX - m_pStartPoint2[i].X;
                    //}
                    //else if ((m_pStartPoint2[i].X + fMoveX) >= (objROI.ref_ROITotalX + objROI.ref_ROIWidth))
                    //{
                    //    fMoveX = (objROI.ref_ROITotalX + objROI.ref_ROIWidth) - m_pStartPoint2[i].X;
                    //}

                    //if ((m_pStartPoint2[i].Y + fMoveY) < objROI.ref_ROITotalY)
                    //{
                    //    fMoveY = objROI.ref_ROITotalY - m_pStartPoint2[i].Y;
                    //}
                    //else if ((m_pStartPoint2[i].Y + fMoveY) >= (objROI.ref_ROITotalY + objROI.ref_ROIHeight))
                    //{
                    //    fMoveY = (objROI.ref_ROITotalY + objROI.ref_ROIHeight) - m_pStartPoint2[i].Y;
                    //}

                    strTrack += ", #[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";

                    // Check will start point and endpoint out of image position?
                    if ((m_pEndPoint2[i].X + fMoveX) < objROI.ref_ROIPositionX) // ref_ROITotalX
                    {
                        fMoveX = objROI.ref_ROIPositionX - m_pEndPoint2[i].X;
                        strTrack += ", a[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";

                    }
                    else if ((m_pEndPoint2[i].X + fMoveX) >= (objROI.ref_ROIPositionX + objROI.ref_ROIWidth))
                    {                        
                        fMoveX = (objROI.ref_ROIPositionX + objROI.ref_ROIWidth) - m_pEndPoint2[i].X;
                        strTrack += ", b[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                    }

                    if ((m_pEndPoint2[i].Y + fMoveY) < objROI.ref_ROIPositionY) //ref_ROITotalY
                    {
                        fMoveY = objROI.ref_ROIPositionY - m_pEndPoint.Y;
                        strTrack += ", A[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                    }
                    else if ((m_pEndPoint2[i].Y + fMoveY) >= (objROI.ref_ROIPositionY + objROI.ref_ROIHeight))
                    {
                        fMoveY = (objROI.ref_ROIPositionY + objROI.ref_ROIHeight) - m_pEndPoint2[i].Y;
                        strTrack += ", B[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                    }


                    //if ((m_pEndPoint2[i].Y + fMoveY) > 0 && (m_pEndPoint2[i].Y + fMoveY) < objROI.ref_ROIHeight)
                    if ((m_pEndPoint2[i].Y) > 0 && (m_pEndPoint2[i].Y) < objROI.ref_ROIHeight)
                    {
                        if ((m_pEndPoint2[i].X + fMoveX) > 0)
                        {
                            fMoveX = 0;
                            strTrack += ", c[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }
                        //if ((m_pEndPoint2[i].Y + fMoveY) >= m_pStartPoint2[i].Y)
                        //    fMoveY= 0;
                    }


                    if ((m_pEndPoint2[i].X + fMoveX) > 0 && (m_pEndPoint2[i].X + fMoveX) < objROI.ref_ROIWidth)
                    {
                        if ((m_pEndPoint2[i].Y + fMoveY) > 0)
                        {
                            fMoveY = 0;
                            strTrack += ", CC[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }
                        //if ((m_pEndPoint2[i].X + fMoveX) >= m_pStartPoint2[i].X)
                        //    fMoveX = 0;
                    }

                    //if ((((m_pEndPoint2[i].Y + fMoveY)<=0) || ((m_pEndPoint2[i].Y + fMoveY) >= objROI.ref_ROIHeight)) && (m_pEndPoint2[i].X == 0 || m_pEndPoint2[i].X == objROI.ref_ROIWidth))
                    //    fMoveY = 0;
                    //if ((((m_pEndPoint2[i].X + fMoveX) <=0)||((m_pEndPoint2[i].X + fMoveX) >= objROI.ref_ROIWidth)) && (m_pEndPoint2[i].X == 0 || m_pEndPoint2[i].X == objROI.ref_ROIWidth))
                    //    fMoveX = 0;


                    if (fMoveY < 0)
                    {
                        if ((m_pEndPoint2[i].Y + fMoveY) <= m_pStartPoint2[i].Y && m_pStartPoint2[i].Y == 0)
                        {
                            fMoveX = 0;
                            fMoveY = 0;
                            strTrack += ", d[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }
                    }
                    else if (fMoveY > 0)
                    {
                        if ((m_pEndPoint2[i].Y + fMoveY) >= (m_pStartPoint2[i].Y) && m_pStartPoint2[i].Y == objROI.ref_ROIHeight)
                        {
                            fMoveX = 0;
                            fMoveY = 0;
                            strTrack += ", e[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }
                    }
                    if (fMoveX < 0)
                    {
                        if ((m_pEndPoint2[i].X + fMoveX) <= m_pStartPoint2[i].X && m_pStartPoint2[i].X == 0)
                        {
                            fMoveX = 0;
                            fMoveY = 0;
                            strTrack += ", f[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }


                        if (m_pStartPoint2[i].Y == objROI.ref_ROIHeight && (m_pEndPoint2[i].X + fMoveX) <= objROI.ref_ROIWidth && m_pEndPoint2[i].Y >= objROI.ref_ROIHeight / 2)
                        {
                            fMoveX = 0;
                            fMoveY = 0;
                            strTrack += ", g[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }

                        if (m_pStartPoint2[i].X == 0 && m_pEndPoint2[i].Y != objROI.ref_ROIHeight && m_pEndPoint2[i].Y >= objROI.ref_ROIHeight / 2 && (m_pEndPoint2[i].X + fMoveX) <= objROI.ref_ROIWidth)
                        {
                            fMoveX = 0;
                            strTrack += ", h[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }

                        if (m_pStartPoint2[i].X == 0 && m_pEndPoint2[i].Y != 0 && m_pEndPoint2[i].Y <= objROI.ref_ROIHeight / 2 && (m_pEndPoint2[i].X + fMoveX) <= objROI.ref_ROIWidth)
                        {
                            fMoveX = 0;
                            strTrack += ", i[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }

                        if (m_pStartPoint2[i].Y == 0 && m_pEndPoint2[i].Y != objROI.ref_ROIHeight && m_pEndPoint2[i].Y >= objROI.ref_ROIHeight / 2 && (m_pEndPoint2[i].X + fMoveX) <= objROI.ref_ROIWidth)
                        {
                            fMoveX = 0;
                            strTrack += ", j[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }
                        if (m_pStartPoint2[i].Y == objROI.ref_ROIHeight && m_pEndPoint2[i].Y != 0 && m_pEndPoint2[i].Y <= objROI.ref_ROIHeight / 2 && (m_pEndPoint2[i].X + fMoveX) <= objROI.ref_ROIWidth)
                        {
                            fMoveX = 0;
                            strTrack += ", k[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }
                    }
                    else if (fMoveX > 0)
                    {
                        if ((m_pEndPoint2[i].X + fMoveX) >= m_pStartPoint2[i].X && m_pStartPoint2[i].X == objROI.ref_ROIWidth)
                        {
                            fMoveX = 0;
                            fMoveY = 0;
                            strTrack += ", L[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }
                        if (m_pStartPoint2[i].Y == objROI.ref_ROIHeight && (m_pEndPoint2[i].X + fMoveX) >= 0 && m_pEndPoint2[i].Y >= objROI.ref_ROIHeight / 2)
                        {
                            fMoveX = 0;
                            fMoveY = 0;
                            strTrack += ", M[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }
                        if (m_pStartPoint2[i].X == objROI.ref_ROIWidth && m_pEndPoint2[i].Y != objROI.ref_ROIHeight && m_pEndPoint2[i].Y >= objROI.ref_ROIHeight / 2 && (m_pEndPoint2[i].X + fMoveX) >= 0)
                        {
                            fMoveX = 0;
                            strTrack += ", N[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }

                        if (m_pStartPoint2[i].X == objROI.ref_ROIWidth && m_pEndPoint2[i].Y != 0 && m_pEndPoint2[i].Y <= objROI.ref_ROIHeight / 2 && (m_pEndPoint2[i].X + fMoveX) >= 0)
                        {
                            fMoveX = 0;
                            strTrack += ", O[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }

                        if (m_pStartPoint2[i].Y == 0 && m_pEndPoint2[i].Y != objROI.ref_ROIHeight && m_pEndPoint2[i].Y >= objROI.ref_ROIHeight / 2 && (m_pEndPoint2[i].X + fMoveX) >= 0)
                        {
                            fMoveX = 0;
                            strTrack += ", p[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }
                        if (m_pStartPoint2[i].Y == objROI.ref_ROIHeight && m_pEndPoint2[i].Y != 0 && m_pEndPoint2[i].Y <= objROI.ref_ROIHeight / 2 && (m_pEndPoint2[i].X + fMoveX) >= 0)
                        {
                            fMoveX = 0;
                            strTrack += ", Q[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                        }
                    }

                    //2021-06-21 ZJYEOH : Allow Drag Over for first two lines
                    //if (m_intcount == 0)
                    //{
                    //    if (m_pEndPoint2[i].Y + fMoveY <= 0 || m_pEndPoint2[i].Y + fMoveY >= objROI.ref_ROIHeight)
                    //    {
                    //        fMoveX = 0;
                    //        strTrack += ", R[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";

                    //    }
                    //}
                    //if (m_intcount == 1)
                    //{
                    //    if (m_pEndPoint2[i].X + fMoveX <= 0 || m_pEndPoint2[i].X + fMoveX >= objROI.ref_ROIWidth)
                    //    {

                    //        fMoveY = 0;
                    //        strTrack += ", S[" + fMoveX.ToString() + "," + fMoveY.ToString() + "]";
                    //    }
                    //}

                    //m_pStartPoint2[i] = new PointF(m_pStartPoint2[i].X + fMoveX, m_pStartPoint2[i].Y + fMoveY);
                    //STTrackLog.WriteLine("End MoveX=" + fMoveX.ToString() + ", MoveY=" + fMoveY.ToString() + " >> " + strTrack);
                    m_pEndPoint2[i] = new PointF(m_pEndPoint2[i].X + fMoveX, m_pEndPoint2[i].Y + fMoveY);
                    //m_pHitPoint2[i] = new PointF(m_pHitPoint2[i].X + fMoveX, m_pHitPoint2[i].Y + fMoveY);
                    m_pHitPoint2 = new PointF(m_pHitPoint2.X + fMoveX, m_pHitPoint2.Y + fMoveY);
                    //m_pEndPoint2[i].X += fMoveX;
                    //m_pEndPoint2[i].Y += fMoveY;
                    //m_pHitPoint2[i].X += fMoveX;
                    //m_pHitPoint2[i].Y += fMoveY;
                }

            }

        }

        public bool WillHitTest2(float fX, float fY, int x, int y, int w, int h)
        {
            for (int i = 0; i < m_pStartPoint2.Count; i++)
            {
                if ((fX >= m_pStartPoint2[i].X - m_intDragBoxTol) &&
                    (fX <= m_pStartPoint2[i].X + m_intDragBoxTol) &&
                    (fY >= m_pStartPoint2[i].Y - m_intDragBoxTol) &&
                    (fY <= m_pStartPoint2[i].Y + m_intDragBoxTol))
                {
                    //////if (i == 0 || i == 1)
                    //////    m_bCenterPointhandlerON = true;
                    //////else
                    //////    m_bStartPointHandlerON = true;
                    ////////if (m_pHitPoint2.Count == 0)
                    ////////    m_pHitPoint2.Add(new PointF(fX, fY));
                    ////////else
                    ////////m_pHitPoint2[i] = new PointF(fX, fY);
                    //////m_pHitPoint2 = new PointF(fX, fY);
                    //////m_intcount = i;

                    if (m_pStartPoint2[i].X == 0 || m_pStartPoint2[i].X == w)
                    {
                        Cursor.Current = Cursors.SizeNS;
                    }
                    else if (m_pStartPoint2[i].Y == 0 || m_pStartPoint2[i].Y == h)
                    {
                        Cursor.Current = Cursors.SizeWE;
                    }



                    //if ((m_pStartPoint2[i].X == 0 && m_pEndPoint2[i].Y == 0) || (m_pStartPoint2[i].Y == 0 && m_pEndPoint2[i].X == 0)) 
                    //{
                    //    Cursor.Current = Cursors.SizeNESW;
                    //}
                    //else if (m_pStartPoint2[i].X == 0 || m_pEndPoint2[i].X == 0)
                    //    Cursor.Current = Cursors.SizeNS;
                    //else
                    //    Cursor.Current = Cursors.SizeWE;
                    return true;
                }



                if ((fX >= m_pEndPoint2[i].X - m_intDragBoxTol) &&
                    (fX <= m_pEndPoint2[i].X + m_intDragBoxTol) &&
                    (fY >= m_pEndPoint2[i].Y - m_intDragBoxTol) &&
                    (fY <= m_pEndPoint2[i].Y + m_intDragBoxTol))
                {
                    //////if (i == 0 || i == 1)
                    //////    m_bCenterPointhandlerON = true;
                    //////else
                    //////    m_bEndPointHandlerON = true;
                    ////////if (m_pHitPoint2.Count == 0)
                    ////////    m_pHitPoint2.Add(new PointF(fX, fY));
                    ////////else
                    ////////m_pHitPoint2[i] = new PointF(fX, fY);
                    //////m_pHitPoint2 = new PointF(fX, fY);
                    //////m_intcount = i;
                    //if ((m_pStartPoint2[i].X == 0 && m_pEndPoint2[i].Y == 0) || (m_pStartPoint2[i].Y == 0 && m_pEndPoint2[i].X == 0))
                    //{
                    //    Cursor.Current = Cursors.SizeNESW;
                    //}
                    //else if (m_pStartPoint2[i].X == 0 || m_pEndPoint2[i].X == 0)
                    //    Cursor.Current = Cursors.SizeNS;
                    //else
                    //    Cursor.Current = Cursors.SizeWE;
                    if (m_pEndPoint2[i].X == 0 || m_pEndPoint2[i].X == w)
                    {
                        Cursor.Current = Cursors.SizeNS;
                    }
                    else if (m_pEndPoint2[i].Y == 0 || m_pEndPoint2[i].Y == h)
                    {
                        Cursor.Current = Cursors.SizeWE;
                    }

                    return true;
                }

                if (Math.Abs(m_pStartPoint2[i].X - m_pEndPoint2[i].X) < Math.Abs(m_pStartPoint2[i].Y - m_pEndPoint2[i].Y))
                {
                    Line L1 = new Line();
                    L1.CalculateStraightLine(new PointF(m_pStartPoint2[i].X - (float)m_intDragBoxTol, m_pStartPoint2[i].Y),
                                             new PointF(m_pEndPoint2[i].X - (float)m_intDragBoxTol, m_pEndPoint2[i].Y));
                    Line L2 = new Line();
                    L2.CalculateStraightLine(new PointF(m_pStartPoint2[i].X + (float)m_intDragBoxTol, m_pStartPoint2[i].Y),
                                             new PointF(m_pEndPoint2[i].X + (float)m_intDragBoxTol, m_pEndPoint2[i].Y));

                    Line L3 = new Line();
                    L3.CalculateStraightLine(new PointF(fX, fY), 90);

                    PointF pCross1 = Line.GetCrossPoint(L1, L3);
                    PointF pCross2 = Line.GetCrossPoint(L2, L3);

                    float fStartX = Math.Min(pCross1.X, pCross2.X);
                    float fEndX = Math.Max(pCross1.X, pCross2.X);
                    float fStartY = Math.Min(m_pStartPoint2[i].Y, m_pEndPoint2[i].Y);
                    float fEndY = Math.Max(m_pStartPoint2[i].Y, m_pEndPoint2[i].Y);

                    if ((fX >= fStartX) &&
                    (fX <= fEndX) &&
                    (fY >= fStartY) &&
                    (fY <= fEndY))
                    {
                        //////m_bCenterPointhandlerON = true;
                        ////////if (m_pHitPoint2.Count == 0)
                        ////////    m_pHitPoint2.Add(new PointF(fX, fY));
                        ////////else
                        ////////m_pHitPoint2[i] = new PointF(fX, fY);
                        //////m_pHitPoint2 = new PointF(fX, fY);
                        //////m_intcount = i;
                        if ((m_pStartPoint2[i].X == 0 && m_pEndPoint2[i].Y == 0) ||
                            (m_pStartPoint2[i].X == 0 && m_pEndPoint2[i].Y == h)  ||
                            (m_pStartPoint2[i].X == w && m_pEndPoint2[i].Y == 0) ||
                            (m_pStartPoint2[i].X == w&& m_pEndPoint2[i].Y == h))
                        {
                            Cursor.Current = Cursors.Default;
                        }
                        else if (m_pStartPoint2[i].X == 0 || m_pEndPoint2[i].X == 0)
                            Cursor.Current = Cursors.SizeNS;
                        else
                            Cursor.Current = Cursors.SizeWE;
                        return true;
                    }
                }
                else
                {
                    Line L1 = new Line();
                    L1.CalculateStraightLine(new PointF(m_pStartPoint2[i].X, m_pStartPoint2[i].Y - (float)m_intDragBoxTol),
                                             new PointF(m_pEndPoint2[i].X, m_pEndPoint2[i].Y - (float)m_intDragBoxTol));
                    Line L2 = new Line();
                    L2.CalculateStraightLine(new PointF(m_pStartPoint2[i].X, m_pStartPoint2[i].Y + (float)m_intDragBoxTol),
                                             new PointF(m_pEndPoint2[i].X, m_pEndPoint2[i].Y + (float)m_intDragBoxTol));

                    Line L3 = new Line();
                    L3.CalculateStraightLine(new PointF(fX, fY), 0);

                    PointF pCross1 = Line.GetCrossPoint(L1, L3);
                    PointF pCross2 = Line.GetCrossPoint(L2, L3);

                    float fStartY = Math.Min(pCross1.Y, pCross2.Y);
                    float fEndY = Math.Max(pCross1.Y, pCross2.Y);
                    float fStartX = Math.Min(m_pStartPoint2[i].X, m_pEndPoint2[i].X);
                    float fEndX = Math.Max(m_pStartPoint2[i].X, m_pEndPoint2[i].X);

                    if ((fX >= fStartX) &&
                    (fX <= fEndX) &&
                    (fY >= fStartY) &&
                    (fY <= fEndY))
                    {
                        //////m_bCenterPointhandlerON = true;
                        ////////if (m_pHitPoint2.Count == 0)
                        ////////    m_pHitPoint2.Add(new PointF(fX, fY));
                        ////////else
                        ////////m_pHitPoint2[i] = new PointF(fX, fY);
                        //////m_pHitPoint2 = new PointF(fX, fY);
                        //////m_intcount = i;
                        if ((m_pStartPoint2[i].X == 0 && m_pEndPoint2[i].Y == 0) ||
                            (m_pStartPoint2[i].X == 0 && m_pEndPoint2[i].Y == h) ||
                            (m_pStartPoint2[i].X == w && m_pEndPoint2[i].Y == 0) ||
                            (m_pStartPoint2[i].X == w && m_pEndPoint2[i].Y == h))
                        {
                            Cursor.Current = Cursors.Default;
                        }
                        else if (m_pStartPoint2[i].X == 0 || m_pEndPoint2[i].X == 0)
                            Cursor.Current = Cursors.SizeNS;
                        else
                            Cursor.Current = Cursors.SizeWE;
                        return true;
                    }
                }
            }

            if (m_bCenterPointhandlerON || m_bStartPointHandlerON || m_bEndPointHandlerON)
                return true;
            else
                return false;
        }

        public bool HitTest2(float fX, float fY)
        {
            for (int i = 0; i < m_pStartPoint2.Count; i++)
            {
                if ((fX >= m_pStartPoint2[i].X - m_intDragBoxTol) &&
                    (fX <= m_pStartPoint2[i].X + m_intDragBoxTol) &&
                    (fY >= m_pStartPoint2[i].Y - m_intDragBoxTol) &&
                    (fY <= m_pStartPoint2[i].Y + m_intDragBoxTol))
                {
                    //if (i == 0 || i == 1)
                    //    m_bCenterPointhandlerON = true;
                    //else
                        m_bStartPointHandlerON = true;
                    //if (m_pHitPoint2.Count == 0)
                    //    m_pHitPoint2.Add(new PointF(fX, fY));
                    //else
                    //m_pHitPoint2[i] = new PointF(fX, fY);
                    m_pHitPoint2 = new PointF(fX, fY);
                    m_intcount = i;
                    return true;
                }



                if ((fX >= m_pEndPoint2[i].X - m_intDragBoxTol) &&
                    (fX <= m_pEndPoint2[i].X + m_intDragBoxTol) &&
                    (fY >= m_pEndPoint2[i].Y - m_intDragBoxTol) &&
                    (fY <= m_pEndPoint2[i].Y + m_intDragBoxTol))
                {
                    //if (i == 0 || i == 1)
                    //    m_bCenterPointhandlerON = true;
                    //else
                        m_bEndPointHandlerON = true;
                    //if (m_pHitPoint2.Count == 0)
                    //    m_pHitPoint2.Add(new PointF(fX, fY));
                    //else
                    //m_pHitPoint2[i] = new PointF(fX, fY);
                    m_pHitPoint2 = new PointF(fX, fY);
                    m_intcount = i;
                    return true;
                }

                if (Math.Abs(m_pStartPoint2[i].X - m_pEndPoint2[i].X) < Math.Abs(m_pStartPoint2[i].Y - m_pEndPoint2[i].Y))
                {
                    Line L1 = new Line();
                    L1.CalculateStraightLine(new PointF(m_pStartPoint2[i].X - (float)m_intDragBoxTol, m_pStartPoint2[i].Y),
                                             new PointF(m_pEndPoint2[i].X - (float)m_intDragBoxTol, m_pEndPoint2[i].Y));
                    Line L2 = new Line();
                    L2.CalculateStraightLine(new PointF(m_pStartPoint2[i].X + (float)m_intDragBoxTol, m_pStartPoint2[i].Y),
                                             new PointF(m_pEndPoint2[i].X + (float)m_intDragBoxTol, m_pEndPoint2[i].Y));

                    Line L3 = new Line();
                    L3.CalculateStraightLine(new PointF(fX, fY), 90);

                    PointF pCross1 = Line.GetCrossPoint(L1, L3);
                    PointF pCross2 = Line.GetCrossPoint(L2, L3);

                    float fStartX = Math.Min(pCross1.X, pCross2.X);
                    float fEndX = Math.Max(pCross1.X, pCross2.X);
                    float fStartY = Math.Min(m_pStartPoint2[i].Y, m_pEndPoint2[i].Y);
                    float fEndY = Math.Max(m_pStartPoint2[i].Y, m_pEndPoint2[i].Y);

                    if ((fX >= fStartX) &&
                    (fX <= fEndX) &&
                    (fY >= fStartY) &&
                    (fY <= fEndY))
                    {
                        m_bCenterPointhandlerON = true;
                        //if (m_pHitPoint2.Count == 0)
                        //    m_pHitPoint2.Add(new PointF(fX, fY));
                        //else
                        //m_pHitPoint2[i] = new PointF(fX, fY);
                        m_pHitPoint2 = new PointF(fX, fY);
                        m_intcount = i;
                        return true;
                    }
                }
                else
                {
                    Line L1 = new Line();
                    L1.CalculateStraightLine(new PointF(m_pStartPoint2[i].X, m_pStartPoint2[i].Y - (float)m_intDragBoxTol),
                                             new PointF(m_pEndPoint2[i].X, m_pEndPoint2[i].Y - (float)m_intDragBoxTol));
                    Line L2 = new Line();
                    L2.CalculateStraightLine(new PointF(m_pStartPoint2[i].X, m_pStartPoint2[i].Y + (float)m_intDragBoxTol),
                                             new PointF(m_pEndPoint2[i].X, m_pEndPoint2[i].Y + (float)m_intDragBoxTol));

                    Line L3 = new Line();
                    L3.CalculateStraightLine(new PointF(fX, fY), 0);

                    PointF pCross1 = Line.GetCrossPoint(L1, L3);
                    PointF pCross2 = Line.GetCrossPoint(L2, L3);

                    float fStartY = Math.Min(pCross1.Y, pCross2.Y);
                    float fEndY = Math.Max(pCross1.Y, pCross2.Y);
                    float fStartX = Math.Min(m_pStartPoint2[i].X, m_pEndPoint2[i].X);
                    float fEndX = Math.Max(m_pStartPoint2[i].X, m_pEndPoint2[i].X);

                    if ((fX >= fStartX) &&
                    (fX <= fEndX) &&
                    (fY >= fStartY) &&
                    (fY <= fEndY))
                    {
                        m_bCenterPointhandlerON = true;
                        //if (m_pHitPoint2.Count == 0)
                        //    m_pHitPoint2.Add(new PointF(fX, fY));
                        //else
                        //m_pHitPoint2[i] = new PointF(fX, fY);
                        m_pHitPoint2 = new PointF(fX, fY);
                        m_intcount = i;
                        return true;
                    }
                }
            }
            m_intcount = -1;
            return false;
        }

        public void SetROIPlacement2(float fStartX, float fStartY, float fEndX, float fEndY, int i, float fInwardStart, float fInwardEnd)
        {
            //if (m_pStartPoint2.Count == 0)
            //{
            //    m_pStartPoint2.Add(new PointF(fStartX, fStartY));
            //    m_pEndPoint2.Add(new PointF(fEndX, fEndY));
            //    m_pCenterPoint2.Add(new PointF((fStartX + fEndX) / 2, (fStartY + fEndY) / 2));
            //}
            if (m_pStartPoint2.Count <= i)
            {
                m_pStartPoint2.Add(new PointF(fStartX, fStartY));
                m_pEndPoint2.Add(new PointF(fEndX, fEndY));
                m_pCenterPoint2.Add(new PointF((fStartX + fEndX) / 2, (fStartY + fEndY) / 2));
                m_arrInwardStartPercent.Add(fInwardStart);
                m_arrInwardEndPercent.Add(fInwardEnd);
            }
            else
            {
                m_pStartPoint2[i] = (new PointF(fStartX, fStartY));
                m_pEndPoint2[i] = (new PointF(fEndX, fEndY));
                m_pCenterPoint2[i] = (new PointF((fStartX + fEndX) / 2, (fStartY + fEndY) / 2));
                m_arrInwardStartPercent[i] = fInwardStart;
                m_arrInwardEndPercent[i] = fInwardEnd;
            }
        }
        public void SetInwardPercent(int i, float fInwardStart, float fInwardEnd)
        {
            if (m_arrInwardStartPercent.Count <= i)
            {
                m_arrInwardStartPercent.Add(fInwardStart);
                m_arrInwardEndPercent.Add(fInwardEnd);
            }
            else
            {
                m_arrInwardStartPercent[i] = fInwardStart;
                m_arrInwardEndPercent[i] = fInwardEnd;
            }
        }
        public void DrawLine2(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int i, bool greenFirst, float fAngle, int intMeasureMethod)
        {
            // if (m_intcount >= 0)
            // {
            //    int i = m_intcount;

            float fStartX = m_pStartPoint2[i].X * fDrawingScaleX;
            float fStartY = m_pStartPoint2[i].Y * fDrawingScaleY;
            float fEndX = m_pEndPoint2[i].X * fDrawingScaleX;
            float fEndY = m_pEndPoint2[i].Y * fDrawingScaleY;


            Pen m_pen = new Pen(Color.MediumSeaGreen, 1);
            SolidBrush m_brush = new SolidBrush(Color.MediumSeaGreen);

            if (i == 0)
            {
                if (greenFirst)
                {
                    m_pen = new Pen(Color.Lime, 1);
                    m_brush = new SolidBrush(Color.Lime);
                }
                else
                {
                    m_pen = new Pen(Color.Blue, 1);
                    m_brush = new SolidBrush(Color.Blue);
                }
            }
            else if (i == 1)
            {
                if (greenFirst)
                {
                    m_pen = new Pen(Color.Lime, 1); // Blue
                    m_brush = new SolidBrush(Color.Lime);
                }
                else
                {
                    m_pen = new Pen(Color.Blue, 1); // Lime
                    m_brush = new SolidBrush(Color.Blue);
                }
            }
            else if (i == 2)
            {
                m_pen = new Pen(Color.Red, 1);
                m_brush = new SolidBrush(Color.Red);
            }
            else if (i == 3)
            {
                m_pen = new Pen(Color.Yellow, 1);
                m_brush = new SolidBrush(Color.Yellow);
            }
            else if (i == 4)
            {
                m_pen = new Pen(Color.Pink, 1);
                m_brush = new SolidBrush(Color.Pink);
            }
            else if (i == 5)
            {
                m_pen = new Pen(Color.Orange, 1);
                m_brush = new SolidBrush(Color.Orange);
            }
            else if (i == 6)
            {
                m_pen = new Pen(Color.Cyan, 1);
                m_brush = new SolidBrush(Color.Cyan);
            }
            else if (i == 7)
            {
                m_pen = new Pen(Color.Magenta, 1);
                m_brush = new SolidBrush(Color.Magenta);
            }
            else if (i == 8)
            {
                m_pen = new Pen(Color.Silver, 1);
                m_brush = new SolidBrush(Color.Silver);
            }
            else if (i == 9)
            {
                m_pen = new Pen(Color.Wheat, 1);
                m_brush = new SolidBrush(Color.Wheat);
            }
            else if (i == 10)
            {
                m_pen = new Pen(Color.DarkGoldenrod, 1);
                m_brush = new SolidBrush(Color.DarkGoldenrod);
            }

            g.DrawLine(m_pen, fStartX, fStartY, fEndX, fEndY);

            g.FillRectangle(m_brush, fStartX - m_intDragBoxTol, fStartY - m_intDragBoxTol, m_intDragBoxTol * 2, m_intDragBoxTol * 2);

            g.FillRectangle(m_brush, fEndX - m_intDragBoxTol, fEndY - m_intDragBoxTol, m_intDragBoxTol * 2, m_intDragBoxTol * 2);

            float fX_Start = Math.Abs(fStartX - fEndX) * (m_arrInwardStartPercent[i] / 100);
            float fX_End = Math.Abs(fStartX - fEndX) * (m_arrInwardEndPercent[i] / 100);
            float fY_Start = Math.Abs(fStartY - fEndY) * (m_arrInwardStartPercent[i] / 100);
            float fY_End = Math.Abs(fStartY - fEndY) * (m_arrInwardEndPercent[i] / 100);

            float fStartX_Draw = fStartX + fX_Start;
            float fStartY_Draw = fStartY + fY_Start;
            float fEndX_Draw = fEndX - fX_End;
            float fEndY_Draw = fEndY - fY_End;
            if (fStartX > fEndX)
            {
                fStartX_Draw = fStartX - fX_Start;
                fEndX_Draw = fEndX + fX_End;
            }
            if (fStartY > fEndY)
            {
                fStartY_Draw = fStartY - fY_Start;
                fEndY_Draw = fEndY + fY_End;
            }
            g.FillEllipse(m_brush, (fStartX_Draw) - m_intDragBoxTol, (fStartY_Draw) - m_intDragBoxTol, m_intDragBoxTol * 2, m_intDragBoxTol * 2);
            g.FillEllipse(new SolidBrush(Color.White), (fStartX_Draw) - 3, (fStartY_Draw) - 3, 6, 6);

            g.FillEllipse(m_brush, (fEndX_Draw) - m_intDragBoxTol, (fEndY_Draw) - m_intDragBoxTol, m_intDragBoxTol * 2, m_intDragBoxTol * 2);
            g.FillEllipse(new SolidBrush(Color.Black), (fEndX_Draw) - 3, (fEndY_Draw) - 3, 6, 6);


            if (intMeasureMethod == 1)
            {
                Line objLine1 = new Line();
                Line objLine2 = new Line();
                PointF pStart = new PointF(fStartX_Draw, fStartY_Draw);
                PointF pEnd = new PointF(fEndX_Draw, fEndY_Draw);

                objLine1.CalculateStraightLine(pStart, fAngle);
                objLine2.CalculateStraightLine(pEnd, fAngle + 90);

                PointF pIntercept = Line.GetCrossPoint(objLine1, objLine2);
                m_pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                if (i == 0)
                {
                    if (greenFirst)
                        g.DrawLine(m_pen, pStart.X, pStart.Y, pIntercept.X, pIntercept.Y);
                    else
                        g.DrawLine(m_pen, pEnd.X, pEnd.Y, pIntercept.X, pIntercept.Y);
                }
                else if (i == 1)
                {
                    if (!greenFirst)
                        g.DrawLine(m_pen, pEnd.X, pEnd.Y, pIntercept.X, pIntercept.Y);
                    else
                        g.DrawLine(m_pen, pStart.X, pStart.Y, pIntercept.X, pIntercept.Y);
                }
                else
                {
                    g.DrawLine(m_pen, pStart.X, pStart.Y, pIntercept.X, pIntercept.Y);
                }
            }
        }

        public void ClearAllPoints()
        {
            m_pStartPoint2.Clear();
            m_pEndPoint2.Clear();
            m_pCenterPoint2.Clear();
            m_arrInwardStartPercent.Clear();
            m_arrInwardEndPercent.Clear();
        }

        public void RemoveLastLine(int intLineCount)
        {
            if(intLineCount <= m_pStartPoint2.Count)
            m_pStartPoint2.RemoveAt(m_pStartPoint2.Count - 1);
            if (intLineCount <= m_pEndPoint2.Count)
                m_pEndPoint2.RemoveAt(m_pEndPoint2.Count - 1);
            if (intLineCount <= m_pCenterPoint2.Count)
                m_pCenterPoint2.RemoveAt(m_pCenterPoint2.Count - 1);
            if (intLineCount <= m_arrInwardStartPercent.Count)
                m_arrInwardStartPercent.RemoveAt(m_arrInwardStartPercent.Count - 1);
            if (intLineCount <= m_arrInwardEndPercent.Count)
                m_arrInwardEndPercent.RemoveAt(m_arrInwardEndPercent.Count - 1);
        }

    }
}
