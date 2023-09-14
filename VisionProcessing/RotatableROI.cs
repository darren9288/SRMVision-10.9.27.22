using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace VisionProcessing
{
    public class RotatableROI
    {
        public enum EDragHandle { NoHandle = 0, TopLeft = 1, Top = 2, TopRight = 3, Right = 4, BottomRight = 5, Bottom = 6, BottomLeft = 7, Left = 8, Inside = 9 , Rotate = 10}

        #region Member Variables
        //  1_______2_______3
        //  |               |
        //  |               |
        //  |               |
        //  8               4
        //  |               |
        //  |               |
        //  7_______6_______5

        private PointF m_PointTop; //2
        private PointF m_PointRight; //4
        private PointF m_PointBottom; //6
        private PointF m_PointLeft; //8
        private PointF m_PointTopLeft; //1
        private PointF m_PointTopRight; //3
        private PointF m_PointBottomLeft; //7
        private PointF m_PointBottomRight; //5

        private Color[] m_Color = new Color[]{Color.Lime, Color.Yellow, Color.Blue, Color.Pink, Color.White, Color.Cyan,
            Color.Plum, Color.Honeydew, Color.LawnGreen, Color.Ivory, Color.Cornsilk, Color.DarkOrange, Color.Red};
        private EDragHandle m_Handler;
        private EDragHandle m_Handler2;

        private int m_intOriPosX = -1;
        private int m_intOriPosY = -1;

        private int m_intMaxWidth = 0;
        private int m_intMaxHeight = 0;

        private float m_fCurrentAngle = 0;
        private float m_fAngleDeg = 0;
        private float m_fAngleDegPrev = 0;
        private PointF m_pOriCenterPoint;
        private PointF m_pOriPoint1;
        private Line objLine1 = new Line();
        private Line objLine2 = new Line();
        #endregion

        #region Properties

        public float ref_fCurrentAngle { get { return m_fCurrentAngle; } }
        public float ref_fAngleDeg { get { return m_fAngleDeg; } }
        public float ref_fAngleDegPrev { get { return m_fAngleDegPrev; } }
        #endregion

        public RotatableROI(int intMaxWidth, int intMaxHeight)
        {
            Dispose();
            m_intMaxWidth = intMaxWidth;
            m_intMaxHeight = intMaxHeight;
            LoadROISetting(0, 0, 100, 100);
        }

        public void LoadROISetting(float fStartX, float fStartY, int intWidth, int intHeight)
        {
            m_PointTopLeft = new PointF(fStartX, fStartY);
            m_PointTopRight = new PointF(fStartX + intWidth, fStartY);
            m_PointBottomLeft = new PointF(fStartX, fStartY + intHeight);
            m_PointBottomRight = new PointF(fStartX + intWidth, fStartY + intHeight);

            m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
            m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
            m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
            m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);

            objLine1.CalculateStraightLine(m_PointTopLeft, m_PointBottomRight);
            objLine2.CalculateStraightLine(m_PointBottomLeft, m_PointTopRight);
            PointF pCross = Line.GetCrossPoint(objLine1, objLine2);
            m_pOriCenterPoint = pCross;
            m_pOriPoint1 = new PointF(m_PointTop.X, m_PointTop.Y -20);
        }
        public void LoadROISetting(List<PointF> arrPoints, Graphics g, float fScaleX, float fScaleY)
        {
            Line objLineP1P2 = new Line();
            Line objLineP2P3 = new Line();
            objLineP1P2.CalculateStraightLine(arrPoints[0], arrPoints[1]);
            objLineP2P3.CalculateStraightLine(arrPoints[1], arrPoints[2]);

            double Angle1 = objLineP1P2.ref_dAngle;
            double Angle2 = objLineP2P3.ref_dAngle;

            if (Math.Abs(arrPoints[1].X - arrPoints[0].X) > Math.Abs(arrPoints[0].Y - arrPoints[1].Y))
            {
                if (Angle1 >= 0)
                    Angle1 = 90 - Angle1;
                else
                {
                    Angle1 += 90;
                    Angle1 = -Angle1;
                }
            }

            if (Math.Abs(arrPoints[1].X - arrPoints[2].X) > Math.Abs(arrPoints[1].Y - arrPoints[2].Y))
            {
                if (Angle2 >= 0)
                    Angle2 = 90 - Angle2;
                else
                {
                    Angle2 += 90;
                    Angle2 = -Angle2;
                }
            }

            if (Math.Abs(Angle1) < Math.Abs(Angle2))
            { 
                if (Angle1 < 0)
                {
                    if (Angle2 < 0)
                    {
                        if (Math.Abs(arrPoints[1].X - arrPoints[2].X) > Math.Abs(arrPoints[1].Y - arrPoints[2].Y))
                        {
                            if (Angle1 >= 0)
                                Angle1 = 90 - Angle1;
                            else
                            {
                                Angle1 += 90;
                                Angle1 = -Angle1;
                            }
                        }
                        objLineP2P3.CalculateStraightLine(arrPoints[2], Angle1);
                    }
                    else
                    {
                        if (Math.Abs(arrPoints[1].X - arrPoints[2].X) > Math.Abs(arrPoints[1].Y - arrPoints[2].Y))
                        {
                            if (Angle1 >= 0)
                                Angle1 = 90 - Angle1;
                            else
                            {
                                Angle1 += 90;
                                Angle1 = -Angle1;
                            }
                        }
                        objLineP2P3.CalculateStraightLine(arrPoints[2], Math.Abs(Angle1));
                    }
                }
                else
                {
                    if (Angle2 < 0)
                    {
                        if (Math.Abs(arrPoints[1].X - arrPoints[2].X) > Math.Abs(arrPoints[1].Y - arrPoints[2].Y))
                        {
                            if (Angle1 >= 0)
                                Angle1 = 90 - Angle1;
                            else
                            {
                                Angle1 += 90;
                                Angle1 = -Angle1;
                            }
                        }
                        objLineP2P3.CalculateStraightLine(arrPoints[2], Math.Abs(Angle1));
                    }
                    else
                    {
                        if (Math.Abs(arrPoints[1].X - arrPoints[2].X) > Math.Abs(arrPoints[1].Y - arrPoints[2].Y))
                        {
                            if (Angle1 >= 0)
                                Angle1 = 90 - Angle1;
                            else
                            {
                                Angle1 += 90;
                                Angle1 = -Angle1;
                            }
                        }
                        objLineP2P3.CalculateStraightLine(arrPoints[2], Angle1);
                    }

                }
            }
            else
            {
                if (Angle2 < 0)
                {
                    if (Angle1 < 0)
                    {
                        if (Math.Abs(arrPoints[1].X - arrPoints[0].X) > Math.Abs(arrPoints[0].Y - arrPoints[1].Y))
                        {
                            if (Angle2 >= 0)
                                Angle2 = 90 - Angle2;
                            else
                            {
                                Angle2 += 90;
                                Angle2 = -Angle2;
                            }
                        }
                        objLineP1P2.CalculateStraightLine(arrPoints[0], Angle2);
                    }
                    else
                    {
                        if (Math.Abs(arrPoints[1].X - arrPoints[0].X) > Math.Abs(arrPoints[0].Y - arrPoints[1].Y))
                        {
                            if (Angle2 >= 0)
                                Angle2 = 90 - Angle2;
                            else
                            {
                                Angle2 += 90;
                                Angle2 = -Angle2;
                            }
                        }
                        objLineP1P2.CalculateStraightLine(arrPoints[0], Math.Abs(Angle2));
                    }
                }
                else
                {
                    if (Angle1 < 0)
                    {
                        if (Math.Abs(arrPoints[1].X - arrPoints[0].X) > Math.Abs(arrPoints[0].Y - arrPoints[1].Y))
                        {
                            if (Angle2 >= 0)
                                Angle2 = 90 - Angle2;
                            else
                            {
                                Angle2 += 90;
                                Angle2 = -Angle2;
                            }
                        }
                        objLineP1P2.CalculateStraightLine(arrPoints[0], Math.Abs(Angle2));
                    }
                    else
                    {
                        if (Math.Abs(arrPoints[1].X - arrPoints[0].X) > Math.Abs(arrPoints[0].Y - arrPoints[1].Y))
                        {
                            if (Angle2 >= 0)
                                Angle2 = 90 - Angle2;
                            else
                            {
                                Angle2 += 90;
                                Angle2 = -Angle2;
                            }
                        }
                        objLineP1P2.CalculateStraightLine(arrPoints[0], Angle2);
                    }

                }
            }

            PointF Cross = Line.GetCrossPoint(objLineP1P2, objLineP2P3);
            if (Math.Abs(arrPoints[1].X - arrPoints[0].X) > Math.Abs(arrPoints[1].Y - arrPoints[0].Y))
            {
                objLineP1P2.ShiftYLine(arrPoints[2].Y - Math.Min(Cross.Y, arrPoints[0].Y));
                objLineP2P3.ShiftXLine(Cross.X - arrPoints[0].X);
                objLineP2P3.ShiftYLine(Cross.Y - arrPoints[0].Y);
            }
            else
            {
                objLineP2P3.ShiftYLine(arrPoints[0].Y - Math.Max(Cross.Y, arrPoints[2].Y));
                objLineP1P2.ShiftXLine(Cross.X - arrPoints[2].X);
                objLineP1P2.ShiftYLine(Cross.Y - arrPoints[2].Y);
            }
            PointF Cross1 = Line.GetCrossPoint(objLineP1P2, objLineP2P3);
            if (float.IsNaN(Cross1.X) && float.IsNaN(Cross1.Y))
            {
                Cross1 = new PointF(arrPoints[0].X, arrPoints[2].Y);
            }
            List<PointF> arrNewPoint = new List<PointF>();
            arrNewPoint.Add(arrPoints[0]);
            arrNewPoint.Add(Cross);
            arrNewPoint.Add(arrPoints[2]);
            arrNewPoint.Add(Cross1);

            RectangleF[] rects = new RectangleF[4];
            rects[0] = new RectangleF((arrNewPoint[0].X - 3) * fScaleX, (arrNewPoint[0].Y - 3) * fScaleY, 6 * fScaleX, 6 * fScaleY);
            rects[1] = new RectangleF((arrNewPoint[1].X - 3) * fScaleX, (arrNewPoint[1].Y - 3) * fScaleY, 6 * fScaleX, 6 * fScaleY);
            rects[2] = new RectangleF((arrNewPoint[2].X - 3) * fScaleX, (arrNewPoint[2].Y - 3) * fScaleY, 6 * fScaleX, 6 * fScaleY);
            rects[3] = new RectangleF((arrNewPoint[3].X - 3) * fScaleX, (arrNewPoint[3].Y - 3) * fScaleY, 6 * fScaleX, 6 * fScaleY);

            g.DrawRectangles(new Pen(m_Color[0], 2), rects);
            g.FillRectangles(new SolidBrush(m_Color[4]), rects);

            g.DrawLine(new Pen(m_Color[0]), arrNewPoint[0].X * fScaleX, arrNewPoint[0].Y * fScaleY, arrNewPoint[1].X * fScaleX, arrNewPoint[1].Y * fScaleY);
            g.DrawLine(new Pen(m_Color[0]), arrNewPoint[2].X * fScaleX, arrNewPoint[2].Y * fScaleY, arrNewPoint[1].X * fScaleX, arrNewPoint[1].Y * fScaleY);
            g.DrawLine(new Pen(m_Color[0]), arrNewPoint[2].X * fScaleX, arrNewPoint[2].Y * fScaleY, arrNewPoint[3].X * fScaleX, arrNewPoint[3].Y * fScaleY);
            g.DrawLine(new Pen(m_Color[0]), arrNewPoint[3].X * fScaleX, arrNewPoint[3].Y * fScaleY, arrNewPoint[0].X * fScaleX, arrNewPoint[0].Y * fScaleY);

            Line newLine1 = new Line();
            newLine1.CalculateStraightLine(arrNewPoint[0], arrNewPoint[2]);
            Line newLine2 = new Line();
            newLine2.CalculateStraightLine(arrNewPoint[1], arrNewPoint[3]);
            PointF newCross = Line.GetCrossPoint(newLine1, newLine2);
            
            for (int i = 0; i < arrNewPoint.Count; i++)
            {
                if (arrNewPoint[i].X < newCross.X) // Left Side
                {
                    if (arrNewPoint[i].Y < newCross.Y)
                    {
                        m_PointTopLeft = arrNewPoint[i];
                    }
                    else
                    {
                        m_PointBottomLeft = arrNewPoint[i];
                    }
                }
                else // Right Side
                {
                    if (arrNewPoint[i].Y < newCross.Y)
                    {
                        m_PointTopRight = arrNewPoint[i];
                    }
                    else
                    {
                        m_PointBottomRight = arrNewPoint[i];
                    }

                }
            }

            m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
            m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
            m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
            m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);

            objLine1.CalculateStraightLine(m_PointTopLeft, m_PointBottomRight);
            objLine2.CalculateStraightLine(m_PointBottomLeft, m_PointTopRight);
            PointF pCross = Line.GetCrossPoint(objLine1, objLine2);
            m_pOriCenterPoint = pCross;
            m_pOriPoint1 = new PointF(m_PointTop.X, m_PointTop.Y - 20);
        }

        public void DrawROI(Graphics g, float fScaleX, float fScaleY, bool blnHandler)
        {

            g.DrawLine(new Pen(m_Color[0], 1.5f), m_PointTopLeft.X * fScaleX, m_PointTopLeft.Y * fScaleY, m_PointTopRight.X * fScaleX, m_PointTopRight.Y * fScaleY);
            g.DrawLine(new Pen(m_Color[0], 1.5f), m_PointTopRight.X * fScaleX, m_PointTopRight.Y * fScaleY, m_PointBottomRight.X * fScaleX, m_PointBottomRight.Y * fScaleY);
            g.DrawLine(new Pen(m_Color[0], 1.5f), m_PointBottomLeft.X * fScaleX, m_PointBottomLeft.Y * fScaleY, m_PointBottomRight.X * fScaleX, m_PointBottomRight.Y * fScaleY);
            g.DrawLine(new Pen(m_Color[0], 1.5f), m_PointTopLeft.X * fScaleX, m_PointTopLeft.Y * fScaleY, m_PointBottomLeft.X * fScaleX, m_PointBottomLeft.Y * fScaleY);

            // Draw small Rectangle
            if (blnHandler)
            {
                RectangleF[] rects = new RectangleF[8];
                rects[0] = new RectangleF((m_PointTopLeft.X - 3) * fScaleX, (m_PointTopLeft.Y - 3) * fScaleY, 6 * fScaleX, 6 * fScaleY);
                rects[1] = new RectangleF((m_PointTop.X - 3) * fScaleX, (m_PointTop.Y - 3) * fScaleY, 6 * fScaleX, 6 * fScaleY);
                rects[2] = new RectangleF((m_PointTopRight.X - 3) * fScaleX, (m_PointTopRight.Y - 3) * fScaleY, 6 * fScaleX, 6 * fScaleY);
                rects[3] = new RectangleF((m_PointRight.X - 3) * fScaleX, (m_PointRight.Y - 3) * fScaleY, 6 * fScaleX, 6 * fScaleY);
                rects[4] = new RectangleF((m_PointBottomRight.X - 3) * fScaleX, (m_PointBottomRight.Y - 3) * fScaleY, 6 * fScaleX, 6 * fScaleY);
                rects[5] = new RectangleF((m_PointBottom.X - 3) * fScaleX, (m_PointBottom.Y - 3) * fScaleY, 6 * fScaleX, 6 * fScaleY);
                rects[6] = new RectangleF((m_PointBottomLeft.X - 3) * fScaleX, (m_PointBottomLeft.Y - 3) * fScaleY, 6 * fScaleX, 6 * fScaleY);
                rects[7] = new RectangleF((m_PointLeft.X - 3) * fScaleX, (m_PointLeft.Y - 3) * fScaleY, 6 * fScaleX, 6 * fScaleY);

                g.DrawRectangles(new Pen(m_Color[0], 2), rects);
                g.FillRectangles(new SolidBrush(m_Color[4]), rects);
                g.FillRectangle(new SolidBrush(m_Color[12]), rects[0]);
                g.FillRectangle(new SolidBrush(m_Color[2]), rects[2]);
                //RectangleF rect = new RectangleF((m_PointTop.X - 8) * fScaleX, (m_PointTop.Y - 20 - 8) * fScaleY, 16 * fScaleX, 16 * fScaleY);
                //g.DrawEllipse(new Pen(m_Color[0], 2), rect);
                //g.FillEllipse(new SolidBrush(m_Color[5]), rect);
            }

        }
        public void DragROI(int intPositionX, int intPositionY)
        {
            if (m_intOriPosX < 0)
            {
                m_intOriPosX = intPositionX;
            }

            if (m_intOriPosY < 0)
            {
                m_intOriPosY = intPositionY;
            }

            int intDiffX = intPositionX - m_intOriPosX;
            int intDiffY = intPositionY - m_intOriPosY;

            Pen objPen = new Pen(Color.Red, 10);

            GraphicsPath gpTop = new GraphicsPath();
            gpTop.AddLine(m_PointTopLeft, m_PointTopRight);

            GraphicsPath gpRight = new GraphicsPath();
            gpRight.AddLine(m_PointTopRight, m_PointBottomRight);

            GraphicsPath gpBottom = new GraphicsPath();
            gpBottom.AddLine(m_PointBottomLeft, m_PointBottomRight);

            GraphicsPath gpLeft = new GraphicsPath();
            gpLeft.AddLine(m_PointTopLeft, m_PointBottomLeft);

            switch (m_Handler)
            {
                //case EDragHandle.Rotate:
                //    objLine1.CalculateStraightLine(m_PointTopLeft, m_PointBottomRight);
                //    objLine2.CalculateStraightLine(m_PointBottomLeft, m_PointTopRight);
                //    PointF pCross = Line.GetCrossPoint(objLine1, objLine2);
                //    float DiffX = intPositionX - pCross.X;
                //    float DiffY = intPositionY - pCross.Y;
                //    float fAngle = -(float)Math.Atan(DiffX / DiffY);
   
                //    //fAngle = -fAngle;

                //    float fAngleDeg = (float)(fAngle * 180f / Math.PI);
                //    m_fAngleDeg = fAngleDeg;


                //    float DiffXPrev = m_intOriPosX - pCross.X;
                //    float DiffYPrev = m_intOriPosY - pCross.Y;
                //    float fAnglePrev = -(float)Math.Atan(DiffXPrev / DiffYPrev);
                 
                //    //fAnglePrev = -fAnglePrev;
                    
                //    float fAngleDegPrev = (float)(fAnglePrev * 180f / Math.PI);
                //    m_fAngleDegPrev = fAngleDegPrev;

                //    float CenterX = m_pOriCenterPoint.X;
                //    float CenterY = m_pOriCenterPoint.Y;

                //    if (DiffY < 0)
                //    {
                //        if (fAngleDeg < fAngleDegPrev)
                //        {
                //            m_fCurrentAngle += (fAngleDeg - fAngleDegPrev);

                //            if (m_fCurrentAngle > 30)
                //            {
                //                m_intOriPosX = intPositionX;
                //                m_intOriPosY = intPositionY;
                //                fAngleDeg = fAngleDegPrev = 0;
                //                m_fCurrentAngle = 30;
                //            }
                //            else if (m_fCurrentAngle < -30)
                //            {
                //                m_intOriPosX = intPositionX;
                //                m_intOriPosY = intPositionY;
                //                fAngleDeg = fAngleDegPrev = 0;
                //                m_fCurrentAngle = -30;
                //            }
                //        }
                //        else
                //        {
                //            m_fCurrentAngle += (fAngleDeg - fAngleDegPrev);
                //            if (m_fCurrentAngle > 30)
                //            {
                //                m_intOriPosX = intPositionX;
                //                m_intOriPosY = intPositionY;
                //                fAngleDeg = fAngleDegPrev = 0;
                //                m_fCurrentAngle = 30;
                //            }
                //            else if (m_fCurrentAngle < -30)
                //            {
                //                m_intOriPosX = intPositionX;
                //                m_intOriPosY = intPositionY;
                //                fAngleDeg = fAngleDegPrev = 0;
                //                m_fCurrentAngle = -30;
                //            }
                //        }
                //    }
                //    else
                //    {
                //        if (fAngleDeg < fAngleDegPrev)
                //        {
                //            m_fCurrentAngle += (fAngleDegPrev - fAngleDeg);

                //            if (m_fCurrentAngle > 30)
                //            {
                //                m_intOriPosX = intPositionX;
                //                m_intOriPosY = intPositionY;
                //                fAngleDeg = fAngleDegPrev = 0;
                //                m_fCurrentAngle = 30;
                //            }
                //            else if (m_fCurrentAngle < -30)
                //            {
                //                m_intOriPosX = intPositionX;
                //                m_intOriPosY = intPositionY;
                //                fAngleDeg = fAngleDegPrev = 0;
                //                m_fCurrentAngle = -30;
                //            }
                //        }
                //        else
                //        {
                //            m_fCurrentAngle += (fAngleDegPrev - fAngleDeg);
                //            if (m_fCurrentAngle > 30)
                //            {
                //                m_intOriPosX = intPositionX;
                //                m_intOriPosY = intPositionY;
                //                fAngleDeg = fAngleDegPrev = 0;
                //                m_fCurrentAngle = 30;
                //            }
                //            else if (m_fCurrentAngle < -30)
                //            {
                //                m_intOriPosX = intPositionX;
                //                m_intOriPosY = intPositionY;
                //                fAngleDeg = fAngleDegPrev = 0;
                //                m_fCurrentAngle = -30;
                //            }
                //        }
                //    }

                //    if (Math.Abs(m_fCurrentAngle) <= 30)
                //        {
                //        if (DiffY < 0)
                //            fAngle = (float)((fAngleDeg - fAngleDegPrev) * Math.PI / 180f);
                //        else
                //            fAngle = (float)((fAngleDegPrev - fAngleDeg) * Math.PI / 180f);

                //        float fXAfterRotated = (float)((CenterX) + ((m_PointTopLeft.X - CenterX) * Math.Cos(fAngle)) -
                //                               ((m_PointTopLeft.Y - CenterY) * Math.Sin(fAngle)));
                //            float fYAfterRotated = (float)((CenterY) + ((m_PointTopLeft.X - CenterX) * Math.Sin(fAngle)) +
                //                                ((m_PointTopLeft.Y - CenterY) * Math.Cos(fAngle)));
                //            m_PointTopLeft = new PointF(fXAfterRotated, fYAfterRotated);

                //            fXAfterRotated = (float)((CenterX) + ((m_PointTopRight.X - CenterX) * Math.Cos(fAngle)) -
                //                               ((m_PointTopRight.Y - CenterY) * Math.Sin(fAngle)));
                //            fYAfterRotated = (float)((CenterY) + ((m_PointTopRight.X - CenterX) * Math.Sin(fAngle)) +
                //                                ((m_PointTopRight.Y - CenterY) * Math.Cos(fAngle)));
                //            m_PointTopRight = new PointF(fXAfterRotated, fYAfterRotated);

                //            fXAfterRotated = (float)((CenterX) + ((m_PointBottomLeft.X - CenterX) * Math.Cos(fAngle)) -
                //                               ((m_PointBottomLeft.Y - CenterY) * Math.Sin(fAngle)));
                //            fYAfterRotated = (float)((CenterY) + ((m_PointBottomLeft.X - CenterX) * Math.Sin(fAngle)) +
                //                                ((m_PointBottomLeft.Y - CenterY) * Math.Cos(fAngle)));
                //            m_PointBottomLeft = new PointF(fXAfterRotated, fYAfterRotated);

                //            fXAfterRotated = (float)((CenterX) + ((m_PointBottomRight.X - CenterX) * Math.Cos(fAngle)) -
                //                               ((m_PointBottomRight.Y - CenterY) * Math.Sin(fAngle)));
                //            fYAfterRotated = (float)((CenterY) + ((m_PointBottomRight.X - CenterX) * Math.Sin(fAngle)) +
                //                                ((m_PointBottomRight.Y - CenterY) * Math.Cos(fAngle)));
                //            m_PointBottomRight = new PointF(fXAfterRotated, fYAfterRotated);

                //            m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                //            m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                //            m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                //            m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                //        }
                    
                //    break;
                case EDragHandle.Inside:
                    if (((m_PointTopLeft.X + intDiffX) >= 0) &&
                      ((m_PointTopLeft.Y + intDiffY) >= 0) &&
                              ((m_PointTopLeft.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointTopLeft.Y + intDiffY) <= m_intMaxHeight) &&
                     ((m_PointTopRight.X + intDiffX) >= 0) &&
                      ((m_PointTopRight.Y + intDiffY) >= 0) &&
                              ((m_PointTopRight.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointTopRight.Y + intDiffY) <= m_intMaxHeight) &&
                    ((m_PointBottomRight.X + intDiffX) >= 0) &&
                      ((m_PointBottomRight.Y + intDiffY) >= 0) &&
                              ((m_PointBottomRight.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointBottomRight.Y + intDiffY) <= m_intMaxHeight) &&
                    ((m_PointBottomLeft.X + intDiffX) >= 0) &&
                      ((m_PointBottomLeft.Y + intDiffY) >= 0) &&
                              ((m_PointBottomLeft.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointBottomLeft.Y + intDiffY) <= m_intMaxHeight))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY);
                        m_PointTopRight = new PointF(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY);
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY);
                        m_PointBottomRight = new PointF(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.TopLeft:

                    if (((m_PointTopLeft.X + intDiffX) >= 0) &&
                        ((m_PointTopLeft.Y + intDiffY) >= 0) &&
                                ((m_PointTopLeft.X + intDiffX) <= m_intMaxWidth) &&
                      ((m_PointTopLeft.Y + intDiffY) <= m_intMaxHeight) &&
                        !gpBottom.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen) &&
                        !gpBottom.IsOutlineVisible(m_PointTopRight.X, m_PointTopRight.Y + intDiffY, objPen) &&
                        !gpRight.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen) &&
                        !gpRight.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y, objPen))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY);
                        m_PointTopRight = new PointF(m_PointTopRight.X, m_PointTopRight.Y + intDiffY);
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.Top:
                    if (((m_PointTopLeft.X + intDiffX) >= 0) &&
                      ((m_PointTopLeft.Y + intDiffY) >= 0) &&
                              ((m_PointTopLeft.X + intDiffX) <= m_intMaxWidth) &&
                      ((m_PointTopLeft.Y + intDiffY) <= m_intMaxHeight) &&
                      ((m_PointTopRight.X + intDiffX) >= 0) &&
                      ((m_PointTopRight.Y + intDiffY) >= 0) &&
                              ((m_PointTopRight.X + intDiffX) <= m_intMaxWidth) &&
                      ((m_PointTopRight.Y + intDiffY) <= m_intMaxHeight) &&
                      !gpBottom.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen) &&
                      !gpBottom.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X, m_PointTopLeft.Y + intDiffY);
                        m_PointTopRight = new PointF(m_PointTopRight.X, m_PointTopRight.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.TopRight:
                    if (((m_PointTopRight.X + intDiffX) >= 0) &&
                      ((m_PointTopRight.Y + intDiffY) >= 0) &&
                      ((m_PointTopRight.X + intDiffX) <= m_intMaxWidth) &&
                      ((m_PointTopRight.Y + intDiffY) <= m_intMaxHeight) &&
                      !gpBottom.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen) &&
                       !gpBottom.IsOutlineVisible(m_PointTopLeft.X, m_PointTopLeft.Y + intDiffY, objPen) &&
                      !gpLeft.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen) &&
                      !gpLeft.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y, objPen))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X, m_PointTopLeft.Y + intDiffY);
                        m_PointTopRight = new PointF(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY);
                        m_PointBottomRight = new PointF(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.Right:
                    if (((m_PointTopRight.X + intDiffX) >= 0) &&
                    ((m_PointTopRight.Y + intDiffY) >= 0) &&
                            ((m_PointTopRight.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointTopRight.Y + intDiffY) <= m_intMaxHeight) &&
                    ((m_PointBottomRight.X + intDiffX) >= 0) &&
                    ((m_PointBottomRight.Y + intDiffY) >= 0) &&
                            ((m_PointBottomRight.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointBottomRight.Y + intDiffY) <= m_intMaxHeight) &&
                    !gpLeft.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen) &&
                    !gpLeft.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen))
                    {
                        m_PointTopRight = new PointF(m_PointTopRight.X + intDiffX, m_PointTopRight.Y );
                        m_PointBottomRight = new PointF(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y );

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.BottomRight:
                    if (((m_PointBottomRight.X + intDiffX) >= 0) &&
                    ((m_PointBottomRight.Y + intDiffY) >= 0) &&
                    ((m_PointBottomRight.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointBottomRight.Y + intDiffY) <= m_intMaxHeight) &&
                    !gpTop.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen) &&
                    !gpTop.IsOutlineVisible(m_PointBottomLeft.X, m_PointBottomLeft.Y + intDiffY, objPen) &&
                    !gpLeft.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen) &&
                    !gpLeft.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y, objPen))
                    {
                        m_PointTopRight = new PointF(m_PointTopRight.X + intDiffX, m_PointTopRight.Y );
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X , m_PointBottomLeft.Y + intDiffY);
                        m_PointBottomRight = new PointF(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.Bottom:
                    if (((m_PointBottomLeft.X + intDiffX) >= 0) &&
                   ((m_PointBottomLeft.Y + intDiffY) >= 0) &&
                           ((m_PointBottomLeft.X + intDiffX) <= m_intMaxWidth) &&
                   ((m_PointBottomLeft.Y + intDiffY) <= m_intMaxHeight) &&
                   ((m_PointBottomRight.X + intDiffX) >= 0) &&
                   ((m_PointBottomRight.Y + intDiffY) >= 0) &&
                           ((m_PointBottomRight.X + intDiffX) <= m_intMaxWidth) &&
                   ((m_PointBottomRight.Y + intDiffY) <= m_intMaxHeight) &&
                   !gpTop.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen) &&
                   !gpTop.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen))
                    {
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X , m_PointBottomLeft.Y + intDiffY);
                        m_PointBottomRight = new PointF(m_PointBottomRight.X , m_PointBottomRight.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.BottomLeft:
                    if (((m_PointBottomLeft.X + intDiffX) >= 0) &&
                       ((m_PointBottomLeft.Y + intDiffY) >= 0) &&
                               ((m_PointBottomLeft.X + intDiffX) <= m_intMaxWidth) &&
                     ((m_PointBottomLeft.Y + intDiffY) <= m_intMaxHeight) &&
                       !gpTop.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen) &&
                        !gpTop.IsOutlineVisible(m_PointBottomRight.X, m_PointBottomRight.Y + intDiffY, objPen) &&
                       !gpRight.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen) &&
                       !gpRight.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y, objPen))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y );
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY);
                        m_PointBottomRight = new PointF(m_PointBottomRight.X , m_PointBottomRight.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.Left:
                    if (((m_PointTopLeft.X + intDiffX) >= 0) &&
                   ((m_PointTopLeft.Y + intDiffY) >= 0) &&
                           ((m_PointTopLeft.X + intDiffX) <= m_intMaxWidth) &&
                   ((m_PointTopLeft.Y + intDiffY) <= m_intMaxHeight) &&
                   ((m_PointBottomLeft.X + intDiffX) >= 0) &&
                   ((m_PointBottomLeft.Y + intDiffY) >= 0) &&
                           ((m_PointBottomLeft.X + intDiffX) <= m_intMaxWidth) &&
                   ((m_PointBottomLeft.Y + intDiffY) <= m_intMaxHeight) &&
                   !gpRight.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen) &&
                   !gpRight.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y);
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
            }

            m_intOriPosX = intPositionX;
            m_intOriPosY = intPositionY;
            objLine1.CalculateStraightLine(m_PointTopLeft, m_PointBottomRight);
            objLine2.CalculateStraightLine(m_PointBottomLeft, m_PointTopRight);
            PointF pCrossP = Line.GetCrossPoint(objLine1, objLine2);
            m_pOriCenterPoint = pCrossP;
        }
        public void DragROI2(int intPositionX, int intPositionY)
        {
            if (m_intOriPosX < 0)
            {
                m_intOriPosX = intPositionX;
            }

            if (m_intOriPosY < 0)
            {
                m_intOriPosY = intPositionY;
            }

            int intDiffX = intPositionX - m_intOriPosX;
            int intDiffY = intPositionY - m_intOriPosY;

            m_intOriPosX = intPositionX;
            m_intOriPosY = intPositionY;

            Pen objPen = new Pen(Color.Red, 10);

            GraphicsPath gpTop = new GraphicsPath();
            gpTop.AddLine(m_PointTopLeft, m_PointTopRight);

            GraphicsPath gpRight = new GraphicsPath();
            gpRight.AddLine(m_PointTopRight, m_PointBottomRight);

            GraphicsPath gpBottom = new GraphicsPath();
            gpBottom.AddLine(m_PointBottomLeft, m_PointBottomRight);

            GraphicsPath gpLeft = new GraphicsPath();
            gpLeft.AddLine(m_PointTopLeft, m_PointBottomLeft);

            switch (m_Handler)
            {
                case EDragHandle.Inside:
                    if (((m_PointTopLeft.X + intDiffX) >= 0) &&
                      ((m_PointTopLeft.Y + intDiffY) >= 0) &&
                              ((m_PointTopLeft.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointTopLeft.Y + intDiffY) <= m_intMaxHeight) &&
                     ((m_PointTopRight.X + intDiffX) >= 0) &&
                      ((m_PointTopRight.Y + intDiffY) >= 0) &&
                              ((m_PointTopRight.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointTopRight.Y + intDiffY) <= m_intMaxHeight) &&
                    ((m_PointBottomRight.X + intDiffX) >= 0) &&
                      ((m_PointBottomRight.Y + intDiffY) >= 0) &&
                              ((m_PointBottomRight.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointBottomRight.Y + intDiffY) <= m_intMaxHeight) &&
                    ((m_PointBottomLeft.X + intDiffX) >= 0) &&
                      ((m_PointBottomLeft.Y + intDiffY) >= 0) &&
                              ((m_PointBottomLeft.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointBottomLeft.Y + intDiffY) <= m_intMaxHeight))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY);
                        m_PointTopRight = new PointF(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY);
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY);
                        m_PointBottomRight = new PointF(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.TopLeft:

                    if (((m_PointTopLeft.X + intDiffX) >= 0) &&
                        ((m_PointTopLeft.Y + intDiffY) >= 0) &&
                                ((m_PointTopLeft.X + intDiffX) <= m_intMaxWidth) &&
                      ((m_PointTopLeft.Y + intDiffY) <= m_intMaxHeight) &&
                        !gpBottom.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen) &&
                        !gpRight.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY);
                        m_PointTopRight = new PointF(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY);
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY);
                        
                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.Top:
                    if (((m_PointTopLeft.X + intDiffX) >= 0) &&
                      ((m_PointTopLeft.Y + intDiffY) >= 0) &&
                              ((m_PointTopLeft.X + intDiffX) <= m_intMaxWidth) &&
                      ((m_PointTopLeft.Y + intDiffY) <= m_intMaxHeight) &&
                      ((m_PointTopRight.X + intDiffX) >= 0) &&
                      ((m_PointTopRight.Y + intDiffY) >= 0) &&
                              ((m_PointTopRight.X + intDiffX) <= m_intMaxWidth) &&
                      ((m_PointTopRight.Y + intDiffY) <= m_intMaxHeight) &&
                      !gpBottom.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen) &&
                      !gpBottom.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY);
                        m_PointTopRight = new PointF(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.TopRight:
                    if (((m_PointTopRight.X + intDiffX) >= 0) &&
                      ((m_PointTopRight.Y + intDiffY) >= 0) &&
                      ((m_PointTopRight.X + intDiffX) <= m_intMaxWidth) &&
                      ((m_PointTopRight.Y + intDiffY) <= m_intMaxHeight) &&
                      !gpBottom.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen) &&
                      !gpLeft.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY);
                        m_PointTopRight = new PointF(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY);
                        m_PointBottomRight = new PointF(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.Right:
                    if (((m_PointTopRight.X + intDiffX) >= 0) &&
                    ((m_PointTopRight.Y + intDiffY) >= 0) &&
                            ((m_PointTopRight.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointTopRight.Y + intDiffY) <= m_intMaxHeight) &&
                    ((m_PointBottomRight.X + intDiffX) >= 0) &&
                    ((m_PointBottomRight.Y + intDiffY) >= 0) &&
                            ((m_PointBottomRight.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointBottomRight.Y + intDiffY) <= m_intMaxHeight) &&
                    !gpLeft.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen) &&
                    !gpLeft.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen))
                    {
                        m_PointTopRight = new PointF(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY);
                        m_PointBottomRight = new PointF(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.BottomRight:
                    if (((m_PointBottomRight.X + intDiffX) >= 0) &&
                    ((m_PointBottomRight.Y + intDiffY) >= 0) &&
                    ((m_PointBottomRight.X + intDiffX) <= m_intMaxWidth) &&
                    ((m_PointBottomRight.Y + intDiffY) <= m_intMaxHeight) &&
                    !gpTop.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen) &&
                    !gpLeft.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen))
                    {
                        m_PointTopRight = new PointF(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY);
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY);
                        m_PointBottomRight = new PointF(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.Bottom:
                    if (((m_PointBottomLeft.X + intDiffX) >= 0) &&
                   ((m_PointBottomLeft.Y + intDiffY) >= 0) &&
                           ((m_PointBottomLeft.X + intDiffX) <= m_intMaxWidth) &&
                   ((m_PointBottomLeft.Y + intDiffY) <= m_intMaxHeight) &&
                   ((m_PointBottomRight.X + intDiffX) >= 0) &&
                   ((m_PointBottomRight.Y + intDiffY) >= 0) &&
                           ((m_PointBottomRight.X + intDiffX) <= m_intMaxWidth) &&
                   ((m_PointBottomRight.Y + intDiffY) <= m_intMaxHeight) &&
                   !gpTop.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen) &&
                   !gpTop.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen))
                    {
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY);
                        m_PointBottomRight = new PointF(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.BottomLeft:
                    if (((m_PointBottomLeft.X + intDiffX) >= 0) &&
                       ((m_PointBottomLeft.Y + intDiffY) >= 0) &&
                               ((m_PointBottomLeft.X + intDiffX) <= m_intMaxWidth) &&
                     ((m_PointBottomLeft.Y + intDiffY) <= m_intMaxHeight) &&
                       !gpTop.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen) &&
                       !gpRight.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY);
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY);
                        m_PointBottomRight = new PointF(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
                case EDragHandle.Left:
                    if (((m_PointTopLeft.X + intDiffX) >= 0) &&
                   ((m_PointTopLeft.Y + intDiffY) >= 0) &&
                           ((m_PointTopLeft.X + intDiffX) <= m_intMaxWidth) &&
                   ((m_PointTopLeft.Y + intDiffY) <= m_intMaxHeight) &&
                   ((m_PointBottomLeft.X + intDiffX) >= 0) &&
                   ((m_PointBottomLeft.Y + intDiffY) >= 0) &&
                           ((m_PointBottomLeft.X + intDiffX) <= m_intMaxWidth) &&
                   ((m_PointBottomLeft.Y + intDiffY) <= m_intMaxHeight) &&
                   !gpRight.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen) &&
                   !gpRight.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY);
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY);

                        m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
                        m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
                        m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
                        m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
                    }
                    break;
            }
        }
        public void ClearDragHandle()
        {
            m_Handler = EDragHandle.NoHandle;
            m_Handler2 = EDragHandle.NoHandle;
        }

        public bool GetROIHandle()
        {
            if (m_Handler != EDragHandle.NoHandle)
                return true;
            else
                return false;
        }
        public bool GetROIHandle2()
        {
            if (m_Handler2 != EDragHandle.NoHandle)
                return true;
            else
                return false;
        }
        public float GetMaxWidth()
        {
            return Math.Max(m_PointTopRight.X - m_PointTopLeft.X, m_PointBottomRight.X - m_PointBottomLeft.X);
        }
        public float GetMaxHeight()
        {
            return Math.Max(m_PointBottomLeft.Y - m_PointTopLeft.Y, m_PointBottomRight.Y - m_PointTopRight.Y);
        }
        public EDragHandle HitTest(float fPosX, float fPosY)
        {
            Pen objPen = new Pen(Color.Red, 3);

            GraphicsPath gpTop = new GraphicsPath();
            gpTop.AddLine(m_PointTopLeft, m_PointTopRight);

            GraphicsPath gpRight = new GraphicsPath();
            gpRight.AddLine(m_PointTopRight, m_PointBottomRight);

            GraphicsPath gpBottom = new GraphicsPath();
            gpBottom.AddLine(m_PointBottomLeft, m_PointBottomRight);

            GraphicsPath gpLeft = new GraphicsPath();
            gpLeft.AddLine(m_PointTopLeft, m_PointBottomLeft);

            PointF[] arrPoints = new PointF[8] { m_PointTopLeft, m_PointTop, m_PointTopRight, m_PointRight, m_PointBottomRight, m_PointBottom, m_PointBottomLeft, m_PointLeft };
            PointF pPoint = new PointF(fPosX, fPosY);

            //if ((fPosX > m_PointTop.X - 3) && (fPosX < m_PointTop.X + 3) && (fPosY > m_PointTop.Y - 20 - 3) && (fPosY < m_PointTop.Y - 20 + 3))
            //{
            //    return EDragHandle.Rotate;
            //}
            //else 
            if ((fPosX > m_PointTopLeft.X - 3) && (fPosX < m_PointTopLeft.X + 3) && (fPosY > m_PointTopLeft.Y - 3) && (fPosY < m_PointTopLeft.Y + 3))
            {
                return EDragHandle.TopLeft;
            }
            else if ((fPosX > m_PointTopRight.X - 3) && (fPosX < m_PointTopRight.X + 3) && (fPosY > m_PointTopRight.Y - 3) && (fPosY < m_PointTopRight.Y + 3))
            {
                return EDragHandle.TopRight;
            }
            else if ((fPosX > m_PointBottomRight.X - 3) && (fPosX < m_PointBottomRight.X + 3) && (fPosY > m_PointBottomRight.Y - 3) && (fPosY < m_PointBottomRight.Y + 3))
            {
                return EDragHandle.BottomRight;
            }
            else if ((fPosX > m_PointBottomLeft.X - 3) && (fPosX < m_PointBottomLeft.X + 3) && (fPosY > m_PointBottomLeft.Y - 3) && (fPosY < m_PointBottomLeft.Y + 3))
            {
                return EDragHandle.BottomLeft;
            }
            else if (gpTop.IsOutlineVisible(fPosX, fPosY, objPen))
            {
                return EDragHandle.Top;
            }
            else if (gpRight.IsOutlineVisible(fPosX, fPosY, objPen))
            {
                return EDragHandle.Right;
            }
            else if (gpBottom.IsOutlineVisible(fPosX, fPosY, objPen))
            {
                return EDragHandle.Bottom;
            }
            else if (gpLeft.IsOutlineVisible(fPosX, fPosY, objPen))
            {
                return EDragHandle.Left;
            }
            else if (IsInPolygon(arrPoints, pPoint))
            {
                return EDragHandle.Inside;
            }

            return EDragHandle.NoHandle;
        }

        public static bool IsInPolygon(PointF[] poly, PointF p)
        {
            PointF p1, p2;
            bool inside = false;

            if (poly.Length < 3)
            {
                return inside;
            }

            var oldPoint = new PointF(
                poly[poly.Length - 1].X, poly[poly.Length - 1].Y);

            for (int i = 0; i < poly.Length; i++)
            {
                var newPoint = new PointF(poly[i].X, poly[i].Y);

                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                    && (p.Y - (long)p1.Y) * (p2.X - p1.X)
                    < (p2.Y - (long)p1.Y) * (p.X - p1.X))
                {
                    inside = !inside;
                }

                oldPoint = newPoint;
            }

            return inside;
        }
        public bool VerifyROIArea(int nNewXPoint, int nNewYPoint)
        {
            int intRangeTolerance = 10;
            if (GetMaxWidth() < 40 || GetMaxHeight() < 40)
                intRangeTolerance = 3;
            else if (GetMaxWidth() < 100 || GetMaxHeight() < 100)
                intRangeTolerance = 5;

            Pen objPen = new Pen(Color.Red, intRangeTolerance);

            GraphicsPath gpTop = new GraphicsPath();
            gpTop.AddLine(m_PointTopLeft, m_PointTopRight);

            GraphicsPath gpRight = new GraphicsPath();
            gpRight.AddLine(m_PointTopRight, m_PointBottomRight);

            GraphicsPath gpBottom = new GraphicsPath();
            gpBottom.AddLine(m_PointBottomLeft, m_PointBottomRight);

            GraphicsPath gpLeft = new GraphicsPath();
            gpLeft.AddLine(m_PointTopLeft, m_PointBottomLeft);

            if (((nNewXPoint < (m_PointTop.X + 3))
                    && (nNewYPoint > (m_PointTop.Y - 20 - 8)) && (nNewYPoint < m_PointTop.Y - 20) && (nNewXPoint > m_PointTop.X))
                   || ((nNewXPoint > (m_PointTop.X - 3))
                    && (nNewYPoint < (m_PointTop.Y - 20 + 8)) && (nNewYPoint > m_PointTop.Y - 20) && (nNewXPoint < m_PointTop.X))
                     || ((nNewXPoint < (m_PointTop.X + 3))
                    && (nNewYPoint < (m_PointTop.Y - 20 + 8)) && (nNewYPoint > m_PointTop.Y - 20) && (nNewXPoint > m_PointTop.X))
                     || ((nNewXPoint > (m_PointTop.X - 3))
                    && (nNewYPoint > (m_PointTop.Y - 20 - 8)) && (nNewYPoint < m_PointTop.Y - 20) && (nNewXPoint < m_PointTop.X))
                    )
            {
                m_Handler = HitTest(m_PointTop.X, m_PointTop.Y - 20);
                //if (GetROIHandle())
                //{
                //    m_intOriPosX = (int)Math.Round(m_PointTopLeft.X);
                //    m_intOriPosY = (int)Math.Round(m_PointTopLeft.Y);
                //}
            }
            else if (((nNewXPoint < (m_PointTopLeft.X + intRangeTolerance))
                   && (nNewYPoint > (m_PointTopLeft.Y - intRangeTolerance)) && (nNewYPoint < m_PointTopLeft.Y) && (nNewXPoint > m_PointTopLeft.X))
                  || ((nNewXPoint > (m_PointTopLeft.X - intRangeTolerance))
                   && (nNewYPoint < (m_PointTopLeft.Y + intRangeTolerance)) && (nNewYPoint > m_PointTopLeft.Y) && (nNewXPoint < m_PointTopLeft.X))
                    || ((nNewXPoint < (m_PointTopLeft.X + intRangeTolerance))
                   && (nNewYPoint < (m_PointTopLeft.Y + intRangeTolerance)) && (nNewYPoint > m_PointTopLeft.Y) && (nNewXPoint > m_PointTopLeft.X))
                    || ((nNewXPoint > (m_PointTopLeft.X - intRangeTolerance))
                   && (nNewYPoint > (m_PointTopLeft.Y - intRangeTolerance)) && (nNewYPoint < m_PointTopLeft.Y) && (nNewXPoint < m_PointTopLeft.X))
                   )
            {
                m_Handler = HitTest(m_PointTopLeft.X, m_PointTopLeft.Y);
                //if (GetROIHandle())
                //{
                //    m_intOriPosX = (int)Math.Round(m_PointTopLeft.X);
                //    m_intOriPosY = (int)Math.Round(m_PointTopLeft.Y);
                //}
            }
            else if (((nNewXPoint < (m_PointTopRight.X + intRangeTolerance))
                 && (nNewYPoint > (m_PointTopRight.Y - intRangeTolerance)) && (nNewYPoint < m_PointTopRight.Y) && (nNewXPoint > m_PointTopRight.X))
                || ((nNewXPoint > (m_PointTopRight.X - intRangeTolerance))
                 && (nNewYPoint < (m_PointTopRight.Y + intRangeTolerance)) && (nNewYPoint > m_PointTopRight.Y) && (nNewXPoint < m_PointTopRight.X))
                  || ((nNewXPoint < (m_PointTopRight.X + intRangeTolerance))
                 && (nNewYPoint < (m_PointTopRight.Y + intRangeTolerance)) && (nNewYPoint > m_PointTopRight.Y) && (nNewXPoint > m_PointTopRight.X))
                  || ((nNewXPoint > (m_PointTopRight.X - intRangeTolerance))
                 && (nNewYPoint > (m_PointTopRight.Y - intRangeTolerance)) && (nNewYPoint < m_PointTopRight.Y) && (nNewXPoint < m_PointTopRight.X))
                 )
            {
                m_Handler = HitTest(m_PointTopRight.X, m_PointTopRight.Y);
                //if (GetROIHandle())
                //{
                //    m_intOriPosX = (int)Math.Round(m_PointTopRight.X);
                //    m_intOriPosY = (int)Math.Round(m_PointTopRight.Y);
                //}
            }
            else if (((nNewXPoint < (m_PointBottomLeft.X + intRangeTolerance))
              && (nNewYPoint > (m_PointBottomLeft.Y - intRangeTolerance)) && (nNewYPoint < m_PointBottomLeft.Y) && (nNewXPoint > m_PointBottomLeft.X))
             || ((nNewXPoint > (m_PointBottomLeft.X - intRangeTolerance))
              && (nNewYPoint < (m_PointBottomLeft.Y + intRangeTolerance)) && (nNewYPoint > m_PointBottomLeft.Y) && (nNewXPoint < m_PointBottomLeft.X))
               || ((nNewXPoint < (m_PointBottomLeft.X + intRangeTolerance))
              && (nNewYPoint < (m_PointBottomLeft.Y + intRangeTolerance)) && (nNewYPoint > m_PointBottomLeft.Y) && (nNewXPoint > m_PointBottomLeft.X))
               || ((nNewXPoint > (m_PointBottomLeft.X - intRangeTolerance))
              && (nNewYPoint > (m_PointBottomLeft.Y - intRangeTolerance)) && (nNewYPoint < m_PointBottomLeft.Y) && (nNewXPoint < m_PointBottomLeft.X))
              )
            {
                m_Handler = HitTest(m_PointBottomLeft.X, m_PointBottomLeft.Y);
                //if (GetROIHandle())
                //{
                //    m_intOriPosX = (int)Math.Round(m_PointBottomLeft.X);
                //    m_intOriPosY = (int)Math.Round(m_PointBottomLeft.Y);
                //}
            }
            else if (((nNewXPoint < (m_PointBottomRight.X + intRangeTolerance))
              && (nNewYPoint > (m_PointBottomRight.Y - intRangeTolerance)) && (nNewYPoint < m_PointBottomRight.Y) && (nNewXPoint > m_PointBottomRight.X))
             || ((nNewXPoint > (m_PointBottomRight.X - intRangeTolerance))
              && (nNewYPoint < (m_PointBottomRight.Y + intRangeTolerance)) && (nNewYPoint > m_PointBottomRight.Y) && (nNewXPoint < m_PointBottomRight.X))
               || ((nNewXPoint < (m_PointBottomRight.X + intRangeTolerance))
              && (nNewYPoint < (m_PointBottomRight.Y + intRangeTolerance)) && (nNewYPoint > m_PointBottomRight.Y) && (nNewXPoint > m_PointBottomRight.X))
               || ((nNewXPoint > (m_PointBottomRight.X - intRangeTolerance))
              && (nNewYPoint > (m_PointBottomRight.Y - intRangeTolerance)) && (nNewYPoint < m_PointBottomRight.Y) && (nNewXPoint < m_PointBottomRight.X))
              )
            {
                m_Handler = HitTest(m_PointBottomRight.X, m_PointBottomRight.Y);
                //if (GetROIHandle())
                //{
                //    m_intOriPosX = (int)Math.Round(m_PointBottomRight.X);
                //    m_intOriPosY = (int)Math.Round(m_PointBottomRight.Y);
                //}
            }
            else if (gpTop.IsOutlineVisible(nNewXPoint, nNewYPoint, objPen))
            {
                m_Handler = HitTest(m_PointTop.X, m_PointTop.Y);
                //if (GetROIHandle())
                //{
                //    m_intOriPosX = (int)Math.Round(m_PointTop.X);
                //    m_intOriPosY = (int)Math.Round(m_PointTop.Y);
                //}
            }
            else if (gpRight.IsOutlineVisible(nNewXPoint, nNewYPoint, objPen))
            {
                m_Handler = HitTest(m_PointRight.X, m_PointRight.Y);
                //if (GetROIHandle())
                //{
                //    m_intOriPosX = (int)Math.Round(m_PointRight.X);
                //    m_intOriPosY = (int)Math.Round(m_PointRight.Y);
                //}
            }
            else if (gpBottom.IsOutlineVisible(nNewXPoint, nNewYPoint, objPen))
            {
                m_Handler = HitTest(m_PointBottom.X, m_PointBottom.Y);
                //if (GetROIHandle())
                //{
                //    m_intOriPosX = (int)Math.Round(m_PointBottom.X);
                //    m_intOriPosY = (int)Math.Round(m_PointBottom.Y);
                //}
            }
            else if (gpLeft.IsOutlineVisible(nNewXPoint, nNewYPoint, objPen))
            {
                m_Handler = HitTest(m_PointLeft.X, m_PointLeft.Y);
                //if (GetROIHandle())
                //{
                //    m_intOriPosX = (int)Math.Round(m_PointLeft.X);
                //    m_intOriPosY = (int)Math.Round(m_PointLeft.Y);
                //}
            }
            else
            {
                m_Handler = HitTest(nNewXPoint, nNewYPoint);
                //if (GetROIHandle())
                //{
                //    m_intOriPosX = nNewXPoint;
                //    m_intOriPosY = nNewYPoint;
                //}
            }

            if (GetROIHandle())
            {
                m_intOriPosX = nNewXPoint;
                m_intOriPosY = nNewYPoint;
            }

            return GetROIHandle();
        }
        public bool VerifyROIHandleShape(int nNewXPoint, int nNewYPoint)
        {
            int intRangeTolerance = 10;
            if (GetMaxWidth() < 40 || GetMaxHeight() < 40)
                intRangeTolerance = 3;
            else if (GetMaxWidth() < 100 || GetMaxHeight() < 100)
                intRangeTolerance = 5;

            Pen objPen = new Pen(Color.Red, intRangeTolerance);

            GraphicsPath gpTop = new GraphicsPath();
            gpTop.AddLine(m_PointTopLeft, m_PointTopRight);

            GraphicsPath gpRight = new GraphicsPath();
            gpRight.AddLine(m_PointTopRight, m_PointBottomRight);

            GraphicsPath gpBottom = new GraphicsPath();
            gpBottom.AddLine(m_PointBottomLeft, m_PointBottomRight);

            GraphicsPath gpLeft = new GraphicsPath();
            gpLeft.AddLine(m_PointTopLeft, m_PointBottomLeft);

            if (((nNewXPoint < (m_PointTopLeft.X + intRangeTolerance))
                    && (nNewYPoint > (m_PointTopLeft.Y - intRangeTolerance)) && (nNewYPoint < m_PointTopLeft.Y) && (nNewXPoint > m_PointTopLeft.X))
                   || ((nNewXPoint > (m_PointTopLeft.X - intRangeTolerance))
                    && (nNewYPoint < (m_PointTopLeft.Y + intRangeTolerance)) && (nNewYPoint > m_PointTopLeft.Y) && (nNewXPoint < m_PointTopLeft.X))
                     || ((nNewXPoint < (m_PointTopLeft.X + intRangeTolerance))
                    && (nNewYPoint < (m_PointTopLeft.Y + intRangeTolerance)) && (nNewYPoint > m_PointTopLeft.Y) && (nNewXPoint > m_PointTopLeft.X))
                     || ((nNewXPoint > (m_PointTopLeft.X - intRangeTolerance))
                    && (nNewYPoint > (m_PointTopLeft.Y - intRangeTolerance)) && (nNewYPoint < m_PointTopLeft.Y) && (nNewXPoint < m_PointTopLeft.X))
                    )
            {
                m_Handler2 = HitTest(m_PointTopLeft.X, m_PointTopLeft.Y);

            }
            else if (((nNewXPoint < (m_PointTopRight.X + intRangeTolerance))
                 && (nNewYPoint > (m_PointTopRight.Y - intRangeTolerance)) && (nNewYPoint < m_PointTopRight.Y) && (nNewXPoint > m_PointTopRight.X))
                || ((nNewXPoint > (m_PointTopRight.X - intRangeTolerance))
                 && (nNewYPoint < (m_PointTopRight.Y + intRangeTolerance)) && (nNewYPoint > m_PointTopRight.Y) && (nNewXPoint < m_PointTopRight.X))
                  || ((nNewXPoint < (m_PointTopRight.X + intRangeTolerance))
                 && (nNewYPoint < (m_PointTopRight.Y + intRangeTolerance)) && (nNewYPoint > m_PointTopRight.Y) && (nNewXPoint > m_PointTopRight.X))
                  || ((nNewXPoint > (m_PointTopRight.X - intRangeTolerance))
                 && (nNewYPoint > (m_PointTopRight.Y - intRangeTolerance)) && (nNewYPoint < m_PointTopRight.Y) && (nNewXPoint < m_PointTopRight.X))
                 )
            {
                m_Handler2 = HitTest(m_PointTopRight.X, m_PointTopRight.Y);

            }
            else if (((nNewXPoint < (m_PointBottomLeft.X + intRangeTolerance))
              && (nNewYPoint > (m_PointBottomLeft.Y - intRangeTolerance)) && (nNewYPoint < m_PointBottomLeft.Y) && (nNewXPoint > m_PointBottomLeft.X))
             || ((nNewXPoint > (m_PointBottomLeft.X - intRangeTolerance))
              && (nNewYPoint < (m_PointBottomLeft.Y + intRangeTolerance)) && (nNewYPoint > m_PointBottomLeft.Y) && (nNewXPoint < m_PointBottomLeft.X))
               || ((nNewXPoint < (m_PointBottomLeft.X + intRangeTolerance))
              && (nNewYPoint < (m_PointBottomLeft.Y + intRangeTolerance)) && (nNewYPoint > m_PointBottomLeft.Y) && (nNewXPoint > m_PointBottomLeft.X))
               || ((nNewXPoint > (m_PointBottomLeft.X - intRangeTolerance))
              && (nNewYPoint > (m_PointBottomLeft.Y - intRangeTolerance)) && (nNewYPoint < m_PointBottomLeft.Y) && (nNewXPoint < m_PointBottomLeft.X))
              )
            {
                m_Handler2 = HitTest(m_PointBottomLeft.X, m_PointBottomLeft.Y);

            }
            else if (((nNewXPoint < (m_PointBottomRight.X + intRangeTolerance))
              && (nNewYPoint > (m_PointBottomRight.Y - intRangeTolerance)) && (nNewYPoint < m_PointBottomRight.Y) && (nNewXPoint > m_PointBottomRight.X))
             || ((nNewXPoint > (m_PointBottomRight.X - intRangeTolerance))
              && (nNewYPoint < (m_PointBottomRight.Y + intRangeTolerance)) && (nNewYPoint > m_PointBottomRight.Y) && (nNewXPoint < m_PointBottomRight.X))
               || ((nNewXPoint < (m_PointBottomRight.X + intRangeTolerance))
              && (nNewYPoint < (m_PointBottomRight.Y + intRangeTolerance)) && (nNewYPoint > m_PointBottomRight.Y) && (nNewXPoint > m_PointBottomRight.X))
               || ((nNewXPoint > (m_PointBottomRight.X - intRangeTolerance))
              && (nNewYPoint > (m_PointBottomRight.Y - intRangeTolerance)) && (nNewYPoint < m_PointBottomRight.Y) && (nNewXPoint < m_PointBottomRight.X))
              )
            {
                m_Handler2 = HitTest(m_PointBottomRight.X, m_PointBottomRight.Y);

            }
            else if (gpTop.IsOutlineVisible(nNewXPoint, nNewYPoint, objPen))
            {
                m_Handler2 = HitTest(m_PointTop.X, m_PointTop.Y);

            }
            else if (gpRight.IsOutlineVisible(nNewXPoint, nNewYPoint, objPen))
            {
                m_Handler2 = HitTest(m_PointRight.X, m_PointRight.Y);

            }
            else if (gpBottom.IsOutlineVisible(nNewXPoint, nNewYPoint, objPen))
            {
                m_Handler2 = HitTest(m_PointBottom.X, m_PointBottom.Y);

            }
            else if (gpLeft.IsOutlineVisible(nNewXPoint, nNewYPoint, objPen))
            {
                m_Handler2 = HitTest(m_PointLeft.X, m_PointLeft.Y);

            }
            else
            {
                m_Handler2 = HitTest(nNewXPoint, nNewYPoint);
            }

            switch (m_Handler2)
            {
                case EDragHandle.NoHandle:
                    Cursor.Current = Cursors.Default;
                    break;
                case EDragHandle.Inside:
                    Cursor.Current = Cursors.SizeAll;
                    break;
                case EDragHandle.Top:
                    Cursor.Current = Cursors.SizeNS;
                    break;
                case EDragHandle.Right:
                    Cursor.Current = Cursors.SizeWE;
                    break;
                case EDragHandle.Bottom:
                    Cursor.Current = Cursors.SizeNS;
                    break;
                case EDragHandle.Left:
                    Cursor.Current = Cursors.SizeWE;
                    break;
                case EDragHandle.TopLeft:
                    Cursor.Current = Cursors.SizeNWSE;
                    break;
                case EDragHandle.BottomLeft:
                    Cursor.Current = Cursors.SizeNESW;
                    break;
                case EDragHandle.TopRight:
                    Cursor.Current = Cursors.SizeNESW;
                    break;
                case EDragHandle.BottomRight:
                    Cursor.Current = Cursors.SizeNWSE;
                    break;
                //case EDragHandle.Rotate:
                //    Cursor.Current = Cursors.Cross;
                //    break;
            }
            return true;
        }
        public void Dispose()
        {
            m_PointTop = new PointF();
            m_PointRight = new PointF();
            m_PointBottom = new PointF();
            m_PointLeft = new PointF();
            m_PointTopLeft = new PointF();
            m_PointTopRight = new PointF();
            m_PointBottomLeft = new PointF();
            m_PointBottomRight = new PointF();

            m_Handler = EDragHandle.NoHandle;
            m_Handler2 = EDragHandle.NoHandle;

            m_intOriPosX = -1;
            m_intOriPosY = -1;

            m_intMaxWidth = 0;
            m_intMaxHeight = 0;

            m_fCurrentAngle = 0;
            m_pOriCenterPoint = new PointF();
            m_pOriPoint1 = new PointF();
            objLine1 = new Line();
            objLine2 = new Line();
    }
    }
}
