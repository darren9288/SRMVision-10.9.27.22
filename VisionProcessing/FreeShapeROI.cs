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

    public class FreeShapeROI
    {
        public enum EDragHandle { NoHandle = 0, TopLeft = 1, Top = 2, TopRight = 3, Right = 4, BottomRight = 5, Bottom = 6, BottomLeft = 7, Left = 8 , Inside = 9}

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
        
        private Color[] m_Color = new Color[]{Color.Lime, Color.Yellow, Color.Lime, Color.Pink, Color.White, Color.Cyan,
            Color.Plum, Color.Honeydew, Color.LawnGreen, Color.Ivory, Color.Cornsilk, Color.DarkOrange, Color.Red};
        private EDragHandle m_Handler;
        private EDragHandle m_Handler2;

        private int m_intOriPosX = -1;
        private int m_intOriPosY = -1;

        private int m_intMaxWidth = 0;
        private int m_intMaxHeight = 0;
        #endregion
        public FreeShapeROI(int intMaxWidth, int intMaxHeight)
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
        }
        public void LoadROISetting(List<PointF> arrPoints, Graphics g, float fScaleX, float fScaleY)
        {
            arrPoints = arrPoints.OrderBy(p => p.X).ToList();
            PointF PLeft1 = arrPoints[0];
            PointF PLeft2 = arrPoints[1];
            PointF PRight1 = arrPoints[2];
            PointF PRight2 = arrPoints[3];

            if (PLeft1.Y < PLeft2.Y)
            {
                m_PointTopLeft = PLeft1;
                m_PointBottomLeft = PLeft2;
            }
            else
            {
                m_PointTopLeft = PLeft2;
                m_PointBottomLeft = PLeft1;
            }

            if (PRight1.Y < PRight2.Y)
            {
                m_PointTopRight = PRight1;
                m_PointBottomRight = PRight2;
            }
            else
            {
                m_PointTopRight = PRight2;
                m_PointBottomRight = PRight1;
            }
            
            m_PointTop = new PointF((m_PointTopLeft.X + m_PointTopRight.X) / 2, (m_PointTopLeft.Y + m_PointTopRight.Y) / 2);
            m_PointRight = new PointF((m_PointTopRight.X + m_PointBottomRight.X) / 2, (m_PointTopRight.Y + m_PointBottomRight.Y) / 2);
            m_PointBottom = new PointF((m_PointBottomLeft.X + m_PointBottomRight.X) / 2, (m_PointBottomLeft.Y + m_PointBottomRight.Y) / 2);
            m_PointLeft = new PointF((m_PointTopLeft.X + m_PointBottomLeft.X) / 2, (m_PointTopLeft.Y + m_PointBottomLeft.Y) / 2);
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

            m_intOriPosX = intPositionX;
            m_intOriPosY = intPositionY;

            Pen objPen = new Pen(Color.Red, 10);


            //Line objLineTopBorder = new Line();
            //objLineTopBorder.CalculateStraightLine(new PointF(0,0), new PointF(m_intMaxWidth, 0));
            //Line objLineRightBorder = new Line();
            //objLineRightBorder.CalculateStraightLine(new PointF(m_intMaxWidth, 0), new PointF(m_intMaxWidth, m_intMaxHeight));
            //Line objLineBottomBorder = new Line();
            //objLineBottomBorder.CalculateStraightLine(new PointF(0, m_intMaxHeight), new PointF(m_intMaxWidth, m_intMaxHeight));
            //Line objLineLeftBorder = new Line();
            //objLineLeftBorder.CalculateStraightLine(new PointF(0, 0), new PointF(0, m_intMaxHeight));

            //Line objLineTop = new Line();
            //objLineTop.CalculateStraightLine(m_PointTopLeft, m_PointTopRight);
            //Line objLineRight = new Line();
            //objLineRight.CalculateStraightLine(m_PointTopRight, m_PointBottomRight);
            //Line objLineBottom = new Line();
            //objLineBottom.CalculateStraightLine(m_PointBottomLeft, m_PointBottomRight);
            //Line objLineLeft = new Line();
            //objLineLeft.CalculateStraightLine(m_PointTopLeft, m_PointBottomLeft);

            ///////////
            //// 2020-06-09 ZJYEOH : might need to add condition when slope of the line is not infinity, below is for slope is infinity only
            ///////////
            //GraphicsPath gpTop = new GraphicsPath();
            //gpTop.AddLine(Line.GetCrossPoint(objLineRightBorder, objLineTop), Line.GetCrossPoint(objLineLeftBorder, objLineTop));

            //GraphicsPath gpRight = new GraphicsPath();
            //gpRight.AddLine(Line.GetCrossPoint(objLineTopBorder, objLineRight), Line.GetCrossPoint(objLineBottomBorder, objLineRight));

            //GraphicsPath gpBottom = new GraphicsPath();
            //gpBottom.AddLine(Line.GetCrossPoint(objLineRightBorder, objLineBottom), Line.GetCrossPoint(objLineLeftBorder, objLineBottom));

            //GraphicsPath gpLeft = new GraphicsPath();
            //gpLeft.AddLine(Line.GetCrossPoint(objLineBottomBorder, objLineLeft), Line.GetCrossPoint(objLineTopBorder, objLineLeft));

            ////GraphicsPath gpTop = new GraphicsPath();
            ////gpTop.AddLine(m_PointTopLeft, m_PointTopRight);

            ////GraphicsPath gpRight = new GraphicsPath();
            ////gpRight.AddLine(m_PointTopRight, m_PointBottomRight);

            ////GraphicsPath gpBottom = new GraphicsPath();
            ////gpBottom.AddLine(m_PointBottomLeft, m_PointBottomRight);

            ////GraphicsPath gpLeft = new GraphicsPath();
            ////gpLeft.AddLine(m_PointTopLeft, m_PointBottomLeft);

            GraphicsPath gpTopLeftX = new GraphicsPath();
            gpTopLeftX.AddLine(new PointF(0, m_PointTopLeft.Y), new PointF(m_intMaxWidth, m_PointTopLeft.Y));
            GraphicsPath gpTopLeftY = new GraphicsPath();
            gpTopLeftY.AddLine(new PointF(m_PointTopLeft.X, 0), new PointF(m_PointTopLeft.X, m_intMaxHeight));

            GraphicsPath gpTopRightX = new GraphicsPath();
            gpTopRightX.AddLine(new PointF(0, m_PointTopRight.Y), new PointF(m_intMaxWidth, m_PointTopRight.Y));
            GraphicsPath gpTopRightY = new GraphicsPath();
            gpTopRightY.AddLine(new PointF(m_PointTopRight.X, 0), new PointF(m_PointTopRight.X, m_intMaxHeight));

            GraphicsPath gpBottomLeftX = new GraphicsPath();
            gpBottomLeftX.AddLine(new PointF(0, m_PointBottomLeft.Y), new PointF(m_intMaxWidth, m_PointBottomLeft.Y));
            GraphicsPath gpBottomLeftY = new GraphicsPath();
            gpBottomLeftY.AddLine(new PointF(m_PointBottomLeft.X, 0), new PointF(m_PointBottomLeft.X, m_intMaxHeight));

            GraphicsPath gpBottomRightX = new GraphicsPath();
            gpBottomRightX.AddLine(new PointF(0, m_PointBottomRight.Y), new PointF(m_intMaxWidth, m_PointBottomRight.Y));
            GraphicsPath gpBottomRightY = new GraphicsPath();
            gpBottomRightY.AddLine(new PointF(m_PointBottomRight.X, 0), new PointF(m_PointBottomRight.X, m_intMaxHeight));

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
                        !gpBottomLeftX.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen) &&
                        !gpBottomRightX.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen) &&
                        !gpBottomRightY.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen) &&
                        !gpTopRightY.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen))
                    {
                        m_PointTopLeft = new PointF(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY);

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
                      !gpBottomLeftX.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen) &&
                      !gpBottomRightX.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen))
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
                      !gpBottomLeftX.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen) &&
                      !gpBottomRightX.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen) &&
                      !gpBottomLeftY.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen) &&
                      !gpTopLeftY.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen))
                    {
                        m_PointTopRight = new PointF(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY);

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
                    !gpTopLeftY.IsOutlineVisible(m_PointTopRight.X + intDiffX, m_PointTopRight.Y + intDiffY, objPen) &&
                    !gpBottomLeftY.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen))
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
                    !gpTopRightX.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen) &&
                    !gpTopLeftX.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen) &&
                    !gpTopLeftY.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen) &&
                    !gpBottomLeftY.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen))
                    {
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
                   !gpTopLeftX.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen) &&
                   !gpTopRightX.IsOutlineVisible(m_PointBottomRight.X + intDiffX, m_PointBottomRight.Y + intDiffY, objPen))
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
                       !gpTopLeftX.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen) &&
                       !gpTopRightX.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen) &&
                       !gpTopRightY.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen) &&
                       !gpBottomRightY.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen))
                    {
                        m_PointBottomLeft = new PointF(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY);

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
                   !gpTopRightY.IsOutlineVisible(m_PointTopLeft.X + intDiffX, m_PointTopLeft.Y + intDiffY, objPen) &&
                   !gpBottomRightY.IsOutlineVisible(m_PointBottomLeft.X + intDiffX, m_PointBottomLeft.Y + intDiffY, objPen))
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
        }
        public float GetLineAngle(int intLineIndex)
        {
            switch (intLineIndex)
            {
                case 0: // Top
                    Line objLine1 = new Line();
                    objLine1.CalculateStraightLine(m_PointTopLeft, m_PointTopRight);
                    return (float)objLine1.ref_dAngle;
                    break;
                case 1: // Right
                    Line objLine2 = new Line();
                    objLine2.CalculateStraightLine(m_PointTopRight, m_PointBottomRight);
                    return (float)objLine2.ref_dAngle;
                    break;
                case 2: // Bottom
                    Line objLine3 = new Line();
                    objLine3.CalculateStraightLine(m_PointBottomLeft, m_PointBottomRight);
                    return (float)objLine3.ref_dAngle;
                    break;
                case 3: // Left
                    Line objLine4 = new Line();
                    objLine4.CalculateStraightLine(m_PointTopLeft, m_PointBottomLeft);
                    return (float)objLine4.ref_dAngle;
                    break;
            }
            return 0;
        }
      
        public PointF GetLineCenterPoint(int intLineIndex)
        {
            switch (intLineIndex)
            {
                case 0: // Top
                    return m_PointTop;
                    break;
                case 1: // Right
                    return m_PointRight;
                    break;
                case 2: // Bottom
                    return m_PointBottom;
                    break;
                case 3: // Left
                    return m_PointLeft;
                    break;
            }
            return new PointF(0,0);
        }

        public float GetLineInterceptPoint(int intLineIndex, float fPointX, float fPointY)
        {
            switch (intLineIndex)
            {
                case 0: // Top
                    Line objLineTop = new Line();
                    objLineTop.CalculateStraightLine(m_PointTopLeft, m_PointTopRight);
                    return objLineTop.GetPointY(fPointX);
                    break;
                case 1: // Right
                    Line objLineRight = new Line();
                    objLineRight.CalculateStraightLine(m_PointTopRight, m_PointBottomRight);
                    return objLineRight.GetPointX(fPointY);
                    break;
                case 2: // Bottom
                    Line objLineBottom = new Line();
                    objLineBottom.CalculateStraightLine(m_PointBottomLeft, m_PointBottomRight);
                    return objLineBottom.GetPointY(fPointX);
                    break;
                case 3: // Left
                    Line objLineLeft = new Line();
                    objLineLeft.CalculateStraightLine(m_PointTopLeft, m_PointBottomLeft);
                    return objLineLeft.GetPointX(fPointY);
                    break;
            }
            return 0;
        }
    }
}
