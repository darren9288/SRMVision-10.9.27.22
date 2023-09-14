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
    public class Polygon
    {
        private int m_intFormMode = 0;  //0: Rectangle, 1: Circle, 2: Freeform
        private bool m_blnDrawDontCareArea = false;
        private List<PointF> m_arrCirCornerPoints = new List<PointF>();
        private List<polyLine> m_Lines = new List<polyLine>();
        private List<PointF> m_arrPoint = new List<PointF>();
        private List<PointF[]> m_arrPolygons = new List<PointF[]>();
        private List<Point> m_arrPolygonsOffset = new List<Point>();
        private List<PointF> m_arrPointsOffset = new List<PointF>();
        private Pen m_penAqua = new Pen(Color.Aqua);
        private SolidBrush m_brushBlack = new SolidBrush(Color.Black);
        private SolidBrush m_brushWhite = new SolidBrush(Color.White);
        private SolidBrush m_brushLime = new SolidBrush(Color.Lime);
        public int ref_intFormMode { get { return m_intFormMode; } set { m_intFormMode = value; } }
        public bool ref_blnDrawDontCareArea { get { return m_blnDrawDontCareArea; } set { m_blnDrawDontCareArea = value; } }
        public List<PointF> ref_arrPoints { get { return m_arrPoint; } set { m_arrPoint = value; } }

        public Polygon()
        {
        }

        public void AddPoint(PointF p)
        {
            if (m_intFormMode == 0)
            {
                if (m_arrPoint.Count == 0)
                    m_arrPoint.Add(p);
                else if (m_arrPoint.Count == 1)
                {
                    m_arrPoint.Add(new PointF(p.X, m_arrPoint[0].Y));
                    m_arrPoint.Add(p);
                    m_arrPoint.Add(new PointF(m_arrPoint[0].X, p.Y));
                }
                else
                {
                    m_arrPoint.RemoveRange(1, 3);
                    m_arrPoint.Add(new PointF(p.X, m_arrPoint[0].Y));
                    m_arrPoint.Add(p);
                    m_arrPoint.Add(new PointF(m_arrPoint[0].X, p.Y));
                }
            }
            else if (m_intFormMode == 1)
            {
                if (m_arrCirCornerPoints.Count == 0)
                    m_arrCirCornerPoints.Add(p);
                else if (m_arrCirCornerPoints.Count > 0)
                {
                    if (m_arrCirCornerPoints.Count > 1)
                        m_arrCirCornerPoints.RemoveRange(1, 1);
                    m_arrCirCornerPoints.Add(p);

                    float fRadiusX = (float)Math.Abs(m_arrCirCornerPoints[0].X - m_arrCirCornerPoints[1].X) / 2;
                    float fRadiusY = (float)Math.Abs(m_arrCirCornerPoints[0].Y - m_arrCirCornerPoints[1].Y) / 2;
                    if ((fRadiusX > 1) && (fRadiusY > 1))
                    {

                        PointF pOriginPoint = new PointF();
                        if (m_arrCirCornerPoints[0].X < m_arrCirCornerPoints[1].X)
                            pOriginPoint.X = m_arrCirCornerPoints[0].X + fRadiusX;
                        else
                            pOriginPoint.X = m_arrCirCornerPoints[1].X + fRadiusX;

                        if (m_arrCirCornerPoints[0].Y < m_arrCirCornerPoints[1].Y)
                            pOriginPoint.Y = m_arrCirCornerPoints[0].Y + fRadiusY;
                        else
                            pOriginPoint.Y = m_arrCirCornerPoints[1].Y + fRadiusY;

                        m_arrPoint.Clear();
                        for (int i = 0; i < 360; i += 10)
                        {
                            m_arrPoint.Add(PointOnOval(fRadiusX, fRadiusY, i, pOriginPoint));
                        }
                    }
                    else
                        m_arrPoint.Clear();
                }
            }
            else
                m_arrPoint.Add(p);
        }

        public void AddPointFromFile(PointF p)
        {
            m_arrPoint.Add(p);
        }
        public void AddOffsetPointFromFile(PointF p)
        {
            m_arrPointsOffset.Add(p);
        }
        public void AddPolygon(int intROICenterX, int intROICenterY)
        {

            if (m_arrPoint.Count > 2)
            {
                // Check is the polygon a line or a point by make sure x or y are not same points
                bool blnSameX = true, blnSameY = true;
                for (int i = 1; i < m_arrPoint.Count; i++)
                {
                    if (m_arrPoint[i].X != m_arrPoint[i - 1].X)
                        blnSameX = false;

                    if (m_arrPoint[i].Y != m_arrPoint[i - 1].Y)
                        blnSameY = false;

                    if (!blnSameX && !blnSameY)
                        break;
                }
                if (!blnSameX && !blnSameY)
                {
                    m_arrPolygons.Add(m_arrPoint.ToArray());
                    m_arrPolygonsOffset.Add(new Point(intROICenterX, intROICenterY));
                }
            }
            if (m_intFormMode != 2)
                m_arrPoint.Clear();
            m_arrCirCornerPoints.Clear();
        }

        public void ClearPolygon()
        {
            if (m_intFormMode != 2)
                m_arrPoint.Clear();
            m_arrPolygons.Clear();
            if (m_intFormMode != 2)
                m_arrPolygonsOffset.Clear();
        }
        public void CopyAllTo(Polygon objDest)
        {
            for (int i = 0; i < m_arrPoint.Count; i++)
            {
                if(objDest.m_arrPoint.Count <= i)
                    objDest.m_arrPoint.Add(new PointF(m_arrPoint[i].X, m_arrPoint[i].Y));
                else
                    objDest.m_arrPoint[i] = new PointF(m_arrPoint[i].X, m_arrPoint[i].Y);

                if (objDest.m_arrPointsOffset.Count <= i)
                    objDest.m_arrPointsOffset.Add(new PointF(m_arrPointsOffset[i].X, m_arrPointsOffset[i].Y));
                else
                    objDest.m_arrPointsOffset[i] = new PointF(m_arrPointsOffset[i].X, m_arrPointsOffset[i].Y);
                
                //objDest.AddPolygon(m_arrPolygonsOffset[i].X, m_arrPolygonsOffset[i].Y);
            }
            objDest.m_intFormMode = m_intFormMode;
        }

        public void CopyTo(Polygon objDest)
        {
            for (int i = 0; i < m_arrPolygons.Count; i++)
            {
                for (int j = 0; j < m_arrPolygons[i].Length; j++)
                {
                    objDest.m_arrPoint.Add(new PointF(m_arrPolygons[i][j].X, m_arrPolygons[i][j].Y));
                }

                objDest.AddPolygon(m_arrPolygonsOffset[i].X, m_arrPolygonsOffset[i].Y);
            }
        }
        public List<PointF> GetPoints(float fScaleX, float fScaleY)
        {
            List<PointF> arrPoints = new List<PointF>();
            for (int i = 0; i < m_arrPoint.Count; i++)
            {
                arrPoints.Add(new PointF(m_arrPoint[i].X * fScaleX, m_arrPoint[i].Y * fScaleY));
            }
            return arrPoints;
        }

        public int GetPointCount()
        {
            return m_arrPoint.Count;
        }

        public int GetPolygonCount()
        {
            return m_arrPolygons.Count;
        }

        public PointF[] GetPolygon(int intPolygonIndex)
        {
            if (intPolygonIndex < m_arrPolygons.Count)
                return m_arrPolygons[intPolygonIndex];
            else
                return null;
        }

        public List<PointF> GetOffsetPoints()
        {
            if (m_arrPointsOffset.Count > 0)
                return m_arrPointsOffset;
            else
                return new List<PointF>();
        }

        public Point GetROICenter(int intPolygonIndex)
        {
            if (intPolygonIndex < m_arrPolygons.Count)
                return m_arrPolygonsOffset[intPolygonIndex];
            else
                return new Point(0, 0);
        }

        public void GetROISize(ref int intX, ref int intY, ref int intWidth, ref int intHeight)
        {
            if (m_arrPoint.Count > 1 && m_intFormMode == 2)
            {
                int intSmallestX = int.MaxValue;
                int intLargestX = 0;
                int intSmallestY = int.MaxValue;
                int intLargestY = 0;
                for (int i =0;i<m_arrPoint.Count;i++)
                {
                    if (intSmallestX > m_arrPoint[i].X)
                        intSmallestX = (int)Math.Floor(m_arrPoint[i].X);

                    if (intLargestX < m_arrPoint[i].X)
                        intLargestX = (int)Math.Ceiling(m_arrPoint[i].X);

                    if (intSmallestY > m_arrPoint[i].Y)
                        intSmallestY = (int)Math.Floor(m_arrPoint[i].Y);

                    if (intLargestY < m_arrPoint[i].Y)
                        intLargestY = (int)Math.Ceiling(m_arrPoint[i].Y);
                }

                intX = intSmallestX;
                intY = intSmallestY;
                intWidth = intLargestX - intSmallestX;
                intHeight = intLargestY - intSmallestY;
            }
        }

        public void SetPointsOffset(float fCenterX, float fCenterY)
        {
            if (m_arrPointsOffset.Count > 0)
                m_arrPointsOffset.Clear();

            for (int i = 0; i < m_arrPoint.Count; i++)
            {
                m_arrPointsOffset.Add(new PointF (fCenterX - m_arrPoint[i].X, fCenterY - m_arrPoint[i].Y));
            }
        }

        public void ResetPointsUsingOffset(float fCenterX, float fCenterY)
        {
            if (m_arrPoint.Count > 0)
            {
                m_arrPoint.Clear();
            }
            for (int i = 0; i < m_arrPointsOffset.Count; i++)
            {
                m_arrPoint.Add(new PointF(fCenterX - m_arrPointsOffset[i].X, fCenterY - m_arrPointsOffset[i].Y));
            }
        }
       

        public void DrawPoint(Graphics g)
        {
            for (int i = 0; i < m_arrPoint.Count; i++)
            {
                g.DrawLine(m_penAqua, m_arrPoint[i].X - 3, m_arrPoint[i].Y - 3, m_arrPoint[i].X + 3, m_arrPoint[i].Y + 3);
                g.DrawLine(m_penAqua, m_arrPoint[i].X + 3, m_arrPoint[i].Y - 3, m_arrPoint[i].X - 3, m_arrPoint[i].Y + 3);
            }
        }

        public void DrawPolygon(Graphics g)
        {
            for (int i = 0; i < m_arrPolygons.Count; i++)
            {
                if (m_arrPolygons[i].Length > 0)
                    g.FillPolygon(m_brushBlack, m_arrPolygons[i]);
            }
            for (int i = 0; i < m_arrPoint.Count; i++)
            {
                if (i == 0)
                    g.DrawLine(new Pen(m_brushLime, 1), m_arrPoint[i], m_arrPoint[m_arrPoint.Count - 1]);
                else
                    g.DrawLine(new Pen(m_brushLime, 1), m_arrPoint[i], m_arrPoint[i - 1]);
            }
            if (m_intFormMode != 2)
            {
                if (m_arrPoint.Count > 1)
                    g.DrawLine(new Pen(m_brushLime, 1), m_arrPoint[0], m_arrPoint[m_arrPoint.Count - 1]);
            }
        }
        public void DrawPoint(Graphics g, float fScaleX, float fScaleY)
        {
            for (int i = 0; i < m_arrPoint.Count; i++)
            {
                g.DrawLine(m_penAqua, (m_arrPoint[i].X - 3)*fScaleX, (m_arrPoint[i].Y - 3) * fScaleY, (m_arrPoint[i].X + 3) * fScaleX, (m_arrPoint[i].Y + 3) * fScaleY);
                g.DrawLine(m_penAqua, (m_arrPoint[i].X + 3) * fScaleX, (m_arrPoint[i].Y - 3) * fScaleY, (m_arrPoint[i].X - 3) * fScaleX, (m_arrPoint[i].Y + 3) * fScaleY);
            }
        }

        public void DrawPolygon(Graphics g, float fScaleX, float fScaleY)
        {
            for (int i = 0; i < m_arrPolygons.Count; i++)
            {
                if (m_arrPolygons[i].Length > 0)
                    g.FillPolygon(m_brushBlack, m_arrPolygons[i]);
            }
            for (int i = 0; i < m_arrPoint.Count; i++)
            {
                if (i == 0)
                    g.DrawLine(new Pen(m_brushLime, 1), m_arrPoint[i].X * fScaleX, m_arrPoint[i].Y * fScaleY, m_arrPoint[m_arrPoint.Count - 1].X * fScaleX, m_arrPoint[m_arrPoint.Count - 1].Y * fScaleY);
                else
                    g.DrawLine(new Pen(m_brushLime, 1), m_arrPoint[i].X * fScaleX, m_arrPoint[i].Y * fScaleY, m_arrPoint[i - 1].X * fScaleX, m_arrPoint[i - 1].Y * fScaleY);
            }
            if (m_intFormMode != 2)
            {
                if (m_arrPoint.Count > 1)
                    g.DrawLine(new Pen(m_brushLime, 1), m_arrPoint[0], m_arrPoint[m_arrPoint.Count - 1]);
            }
        }

        public void DrawPolygon(Graphics g, int intBrushType, float fScaleX , float fScaleY)
        {
            for (int i = 0; i < m_arrPolygons.Count; i++)
            {
                if (m_arrPolygons[i].Length > 0)
                {
                    if (intBrushType == 0)
                        g.FillPolygon(m_brushBlack, m_arrPolygons[i]);
                    else
                        g.FillPolygon(m_brushWhite, m_arrPolygons[i]);
                }
            }
            for (int i = 0; i < m_arrPoint.Count; i++)
            {
                if (i == 0)
                    g.DrawLine(new Pen(m_brushLime, 1), m_arrPoint[i].X * fScaleX, m_arrPoint[i].Y * fScaleY, m_arrPoint[m_arrPoint.Count - 1].X * fScaleX, m_arrPoint[m_arrPoint.Count - 1].Y * fScaleY);
                else
                    g.DrawLine(new Pen(m_brushLime, 1), m_arrPoint[i].X * fScaleX, m_arrPoint[i].Y * fScaleY, m_arrPoint[i - 1].X * fScaleX, m_arrPoint[i - 1].Y * fScaleY);
            }
            if (m_intFormMode != 2)
            {
                if (m_arrPoint.Count > 1)
                    g.DrawLine(new Pen(m_brushLime, 1), m_arrPoint[0], m_arrPoint[m_arrPoint.Count - 1]);
            }
        }

        public void DrawPolygon(Graphics g, int intBrushType)
        {
            for (int i = 0; i < m_arrPolygons.Count; i++)
            {
                if (m_arrPolygons[i].Length > 0)
                {
                    if (intBrushType == 0)
                        g.FillPolygon(m_brushBlack, m_arrPolygons[i]);
                    else
                        g.FillPolygon(m_brushWhite, m_arrPolygons[i]);
                }
            }
            for (int i = 1; i < m_arrPoint.Count; i++)
            {
                g.DrawLine(new Pen(m_brushLime, 1), m_arrPoint[i], m_arrPoint[i - 1]);
            }
            if (m_intFormMode != 2)
            {
                if (m_arrPoint.Count > 1)
                    g.DrawLine(new Pen(m_brushLime, 1), m_arrPoint[0], m_arrPoint[m_arrPoint.Count - 1]);
            }
        }

        public void FillPolygonOnImage(ref ImageDrawing objDestinationImage)
        {
            FillPolygon(objDestinationImage, m_arrPolygons, 0);
        }

        public void FillPolygonOnImage(ImageDrawing objSourceImage, ref ImageDrawing objDestinationImage)
        {
            objSourceImage.CopyTo(ref objDestinationImage);
            FillPolygon(objDestinationImage, m_arrPolygons, 0);
        }

        public void FillPolygonOnImage(ref ImageDrawing objDestinationImage, int intPixelValue)
        {
            FillPolygon(objDestinationImage, m_arrPolygons, intPixelValue);
        }

        public void FillPolygonOnImageWithScale(ref ImageDrawing objDestinationImage, int intPixelValue, float fScaleX, float fScaleY, float fROIStartX, float fROIStartY)
        {
            FillPolygonWithScale(objDestinationImage, m_arrPolygons, intPixelValue, fScaleX, fScaleY, fROIStartX, fROIStartY);
        }
        public void FillPolygonOnImageWithScale(ref ImageDrawing objDestinationImage, int intPixelValue, float fScaleX, float fScaleY, float fROIStartX, float fROIStartY, float fROIEndX, float fROIEndY)
        {
            FillPolygonWithScale(objDestinationImage, m_arrPolygons, intPixelValue, fScaleX, fScaleY, fROIStartX, fROIStartY, fROIEndX, fROIEndY);
        }
        public void FillPolygonOnImage(ref ROI objDestinationROI, int intPixelValue)
        {
            FillPolygon(objDestinationROI, m_arrPolygons, intPixelValue);
        }

        public void SavePolygon(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "Polygon.xml", true);

            for (int i = 0; i < m_arrPolygons.Count; i++)
            {
                objFile.WriteSectionElement("Polygon" + i, false);

                PointF[] arrPoints = m_arrPolygons[i];
                for (int j = 0; j < arrPoints.Length; j++)
                {
                    objFile.WriteElement1Value("Point" + j, "", "Point No.", true);
                    objFile.WriteElement2Value("X", arrPoints[j].X, "Location X", true);
                    objFile.WriteElement2Value("Y", arrPoints[j].Y, "Location X", true);
                }
            }

            objFile.WriteEndElement();
        }

        public static void SavePolygon(string strFilePath, string strSectionName, List<Polygon> arrPolygon)
        {
            XmlParser objFile = new XmlParser(strFilePath);

            objFile.WriteSectionElement(strSectionName, true);

            for (int i = 0; i < arrPolygon.Count; i++)
            {
                objFile.WriteElement1Value("Polygon" + i.ToString(), "");

                for (int j = 0; j < arrPolygon[i].GetPolygonCount(); j++)
                {
                    objFile.WriteElement2Value("FormMode", arrPolygon[i].ref_intFormMode, "Form Mode", true);
                    Point pROICenter = arrPolygon[i].GetROICenter(j);
                    objFile.WriteElement2Value("ROICenterX", pROICenter.X, "ROI Center X", true);
                    objFile.WriteElement2Value("ROICenterY", pROICenter.Y, "ROI Center Y", true);

                    objFile.WriteElement2Value("Polygon" + j, "", "Polygon No.", true);

                    PointF[] objPolygon = arrPolygon[i].GetPolygon(j);
                    for (int k = 0; k < objPolygon.Length; k++)
                    {
                        objFile.WriteElement3Value("Point" + k, objPolygon[k].X.ToString() + "," + objPolygon[k].Y.ToString(), "Polygon Point No", true);
                    }

                    objFile.WriteElement2Value("Offset" + j, "", "Offset No.", true);
                    PointF[] objPointOffset = arrPolygon[i].GetOffsetPoints().ToArray();
                    for (int k = 0; k < objPointOffset.Length; k++)
                    {
                        objFile.WriteElement3Value("PointOffset" + k, objPointOffset[k].X.ToString() + "," + objPointOffset[k].Y.ToString(), "Polygon Point Offset", true);
                    }
                }
            }

            objFile.WriteEndElement();
        }

        public static void SavePolygon(string strFilePath, List<List<Polygon>> arrPolygon)
        {
            XmlParser objFile = new XmlParser(strFilePath);

            for (int h = 0; h < arrPolygon.Count; h++)
            {
                objFile.WriteSectionElement("Type" + h, true);

                for (int i = 0; i < arrPolygon[h].Count; i++)
                {
                    objFile.WriteElement1Value("Template" + i, "");

                    for (int j = 0; j < arrPolygon[h][i].GetPolygonCount(); j++)
                    {
                        objFile.WriteElement2Value("FormMode", arrPolygon[h][i].ref_intFormMode, "Form Mode", true);
                        Point pROICenter = arrPolygon[h][i].GetROICenter(j);
                        objFile.WriteElement2Value("ROICenterX", pROICenter.X, "ROI Center X", true);
                        objFile.WriteElement2Value("ROICenterY", pROICenter.Y, "ROI Center Y", true);

                        objFile.WriteElement2Value("Polygon" + j, "", "Polygon No.", true);

                        PointF[] objPolygon = arrPolygon[h][i].GetPolygon(j);
                        for (int k = 0; k < objPolygon.Length; k++)
                        {
                            objFile.WriteElement3Value("Point" + k, objPolygon[k].X.ToString() + "," + objPolygon[k].Y.ToString(), "Polygon Point No", true);
                        }

                        objFile.WriteElement2Value("Offset" + j, "", "Offset No.", true);
                        PointF[] objPointOffset = arrPolygon[h][i].GetOffsetPoints().ToArray();
                        for (int k = 0; k < objPointOffset.Length; k++)
                        {
                            objFile.WriteElement3Value("PointOffset" + k, objPointOffset[k].X.ToString() + "," + objPointOffset[k].Y.ToString(), "Polygon Point Offset", true);
                        }
                    }
                }
            }

            objFile.WriteEndElement();
        }
        public static void SavePolygon(string strFilePath, List<List<List<Polygon>>> arrPolygon)
        {
            XmlParser objFile = new XmlParser(strFilePath);

            for (int h = 0; h < arrPolygon.Count; h++)
            {
                objFile.WriteSectionElement("Type" + h, true);

                for (int i = 0; i < arrPolygon[h].Count; i++)
                {
                    objFile.WriteElement1Value("Image" + i, "");
                    for (int a = 0; a < arrPolygon[h][i].Count; a++)
                    {
                        objFile.WriteElement2Value("Template" + a, "");
                        for (int j = 0; j < arrPolygon[h][i][a].GetPolygonCount(); j++)
                        {
                            objFile.WriteElement3Value("FormMode", arrPolygon[h][i][a].ref_intFormMode, "Form Mode", true);
                            Point pROICenter = arrPolygon[h][i][a].GetROICenter(j);
                            objFile.WriteElement3Value("ROICenterX", pROICenter.X, "ROI Center X", true);
                            objFile.WriteElement3Value("ROICenterY", pROICenter.Y, "ROI Center Y", true);

                            objFile.WriteElement3Value("Polygon" + j, "", "Polygon No.", true);

                            PointF[] objPolygon = arrPolygon[h][i][a].GetPolygon(j);
                            for (int k = 0; k < objPolygon.Length; k++)
                            {
                                objFile.WriteElement4Value("Point" + k, objPolygon[k].X.ToString() + "," + objPolygon[k].Y.ToString(), "Polygon Point No", true);
                            }

                            objFile.WriteElement3Value("Offset" + j, "", "Offset No.", true);
                            PointF[] objPointOffset = arrPolygon[h][i][a].GetOffsetPoints().ToArray();
                            for (int k = 0; k < objPointOffset.Length; k++)
                            {
                                objFile.WriteElement4Value("PointOffset" + k, objPointOffset[k].X.ToString() + "," + objPointOffset[k].Y.ToString(), "Polygon Point Offset", true);
                            }
                        }
                    }
                }
            }

            objFile.WriteEndElement();
        }

        public static void LoadPolygon(string strPath, string strSectionName, List<Polygon> arrPolygon)
        {
            arrPolygon.Clear();

            XmlParser objFile = new XmlParser(strPath);
            PointF p = new PointF();
            string[] strPoint;

            objFile.GetFirstSection(strSectionName);
            int intPolygonCount = objFile.GetSecondSectionCount();
            for (int j = 0; j < intPolygonCount; j++)
            {
                arrPolygon.Add(new Polygon());

                objFile.GetSecondSection("Polygon" + j.ToString());

                arrPolygon[j].ref_intFormMode = objFile.GetValueAsInt("FormMode", 0, 2);
                int intROICenterX = objFile.GetValueAsInt("ROICenterX", 0, 2);
                int intROICenterY = objFile.GetValueAsInt("ROICenterY", 0, 2);

                objFile.GetThirdSection("Polygon" + j);

                int intPointCount = objFile.GetThirdSectionCount();
                for (int k = 0; k < intPointCount; k++)
                {
                    strPoint = objFile.GetValueAsString("Point" + k, "0,0", 3).Split(',');
                    if (strPoint.Length == 2)
                    {
                        p = new PointF(float.Parse(strPoint[0]), float.Parse(strPoint[1]));
                        arrPolygon[j].AddPointFromFile(p);
                    }
                }
                objFile.GetThirdSection("Offset" + j);
                int intPointOffsetCount = objFile.GetFourthSectionCount();
                for (int k = 0; k < intPointOffsetCount; k++)
                {
                    strPoint = objFile.GetValueAsString("PointOffset" + k, "0,0", 3).Split(',');
                    if (strPoint.Length == 2)
                    {
                        p = new PointF(float.Parse(strPoint[0]), float.Parse(strPoint[1]));
                        arrPolygon[j].AddOffsetPointFromFile(p);
                    }
                }

                arrPolygon[j].AddPolygon(intROICenterX, intROICenterY);
            }
        }

        public static void LoadPolygon(string strPath, List<List<Polygon>> arrPolygon)
        {
            for (int i = 0; i < arrPolygon.Count; i++)
            {
                for (int j = 0; j < arrPolygon[i].Count; j++)
                {
                    if (arrPolygon[i][j] != null)
                        arrPolygon[i][j].Dispose();
                }
            }

            arrPolygon.Clear();

            XmlParser objFile = new XmlParser(strPath);
            PointF p = new PointF();
            string[] strPoint;

            // Load new polygon data
            int intTypeCount = objFile.GetFirstSectionCount();
            for (int h = 0; h < intTypeCount; h++)
            {
                arrPolygon.Add(new List<Polygon>());
                objFile.GetFirstSection("Type" + h);
                int intTemplateCount = objFile.GetSecondSectionCount();
                for (int i = 0; i < intTemplateCount; i++)
                {
                    arrPolygon[h].Add(new Polygon());
                    objFile.GetSecondSection("Template" + i);
                    arrPolygon[h][i].ref_intFormMode = objFile.GetValueAsInt("FormMode", 0, 2);
                    int intPolygonCount = objFile.GetThirdSectionCount();
                    for (int j = 0; j < intPolygonCount; j++)
                    {
                        
                        int intROICenterX = objFile.GetValueAsInt("ROICenterX", 0, 2);
                        int intROICenterY = objFile.GetValueAsInt("ROICenterY", 0, 2);

                        objFile.GetThirdSection("Polygon" + j);

                        int intPointCount = objFile.GetFourthSectionCount();
                        for (int k = 0; k < intPointCount; k++)
                        {
                            strPoint = objFile.GetValueAsString("Point" + k, "0,0", 3).Split(',');
                            if (strPoint.Length == 2)
                            {
                                p = new PointF(float.Parse(strPoint[0]), float.Parse(strPoint[1]));
                                arrPolygon[h][i].AddPointFromFile(p);
                            }
                        }
                        objFile.GetThirdSection("Offset" + j);
                        int intPointOffsetCount = objFile.GetFourthSectionCount();
                        for (int k = 0; k < intPointOffsetCount; k++)
                        {
                            strPoint = objFile.GetValueAsString("PointOffset" + k, "0,0", 3).Split(',');
                            if (strPoint.Length == 2)
                            {
                                p = new PointF(float.Parse(strPoint[0]), float.Parse(strPoint[1]));
                                arrPolygon[h][i].AddOffsetPointFromFile(p);
                            }
                        }
                      
                        arrPolygon[h][i].AddPolygon(intROICenterX, intROICenterY);
                    }
                }
            }
        }
        public static void LoadPolygon(string strPath, List<List<List<Polygon>>> arrPolygon, bool blnInitFirstTwoIndex)
        {
            for (int i = 0; i < arrPolygon.Count; i++)
            {
                for (int j = 0; j < arrPolygon[i].Count; j++)
                {
                    for (int k = 0; k < arrPolygon[i][j].Count; k++)
                    {
                        if (arrPolygon[i][j][k] != null)
                            arrPolygon[i][j][k].Dispose();
                    }
                }
            }

            arrPolygon.Clear();

            XmlParser objFile = new XmlParser(strPath);
            PointF p = new PointF();
            string[] strPoint;

            // Load new polygon data
            int intTypeCount = objFile.GetFirstSectionCount();
            for (int h = 0; h < intTypeCount; h++)
            {
                arrPolygon.Add(new List<List<Polygon>>());
                objFile.GetFirstSection("Type" + h);
                int intTemplateCount = objFile.GetSecondSectionCount();
                for (int i = 0; i < intTemplateCount; i++)
                {
                    arrPolygon[h].Add(new List<Polygon>());
                    objFile.GetSecondSection("Image" + i);
                    int intImageCount = objFile.GetThirdSectionCount();
                    for (int a = 0; a < intImageCount; a++)
                    {
                        arrPolygon[h][i].Add(new Polygon());
                        objFile.GetThirdSection("Template" + a);
                        arrPolygon[h][i][a].ref_intFormMode = objFile.GetValueAsInt("FormMode", 0, 3);
                        int intPolygonCount = objFile.GetFourthSectionCount();
                        for (int j = 0; j < intPolygonCount; j++)
                        {
                            int intROICenterX = objFile.GetValueAsInt("ROICenterX", 0, 3);
                            int intROICenterY = objFile.GetValueAsInt("ROICenterY", 0, 3);

                            objFile.GetFourthSection("Polygon" + j);

                            int intPointCount = objFile.GetFifthSectionCount();
                            for (int k = 0; k < intPointCount; k++)
                            {
                                strPoint = objFile.GetValueAsString("Point" + k, "0,0", 4).Split(',');
                                if (strPoint.Length == 2)
                                {
                                    p = new PointF(float.Parse(strPoint[0]), float.Parse(strPoint[1]));
                                    arrPolygon[h][i][a].AddPointFromFile(p);
                                }
                            }

                            objFile.GetFourthSection("Offset" + j);
                            int intPointOffsetCount = objFile.GetFifthSectionCount();
                            for (int k = 0; k < intPointOffsetCount; k++)
                            {
                                strPoint = objFile.GetValueAsString("PointOffset" + k, "0,0", 4).Split(',');
                                if (strPoint.Length == 2)
                                {
                                    p = new PointF(float.Parse(strPoint[0]), float.Parse(strPoint[1]));
                                    arrPolygon[h][i][a].AddOffsetPointFromFile(p);
                                }
                            }

                            arrPolygon[h][i][a].AddPolygon(intROICenterX, intROICenterY);
                        }
                    }
                }
            }

            if (blnInitFirstTwoIndex)
            {
                //2021-02-23 ZJYEOH : In case Polygon file no load anything 
                if (arrPolygon.Count == 0)
                {
                    arrPolygon.Add(new List<List<Polygon>>());
                    if (arrPolygon[0].Count == 0)
                        arrPolygon[0].Add(new List<Polygon>());
                    if (arrPolygon[0].Count == 1)
                        arrPolygon[0].Add(new List<Polygon>());
                }
                else
                {
                    if (arrPolygon[0].Count == 0)
                        arrPolygon[0].Add(new List<Polygon>());
                    if (arrPolygon[0].Count == 1)
                        arrPolygon[0].Add(new List<Polygon>());
                }
            }
        }
        public static void LoadPolygon(string strPath, List<List<List<Polygon>>> arrPolygon, int intTypeCount)
        {
            for (int i = 0; i < arrPolygon.Count; i++)
            {
                for (int j = 0; j < arrPolygon[i].Count; j++)
                {
                    for (int k = 0; k < arrPolygon[i][j].Count; k++)
                    {
                        if (arrPolygon[i][j][k] != null)
                            arrPolygon[i][j][k].Dispose();
                    }
                }
            }

            arrPolygon.Clear();

            XmlParser objFile = new XmlParser(strPath);
            PointF p = new PointF();
            string[] strPoint;

            // Load new polygon data
            //int intTypeCount = objFile.GetFirstSectionCount();
            for (int h = 0; h < intTypeCount; h++)
            {
                arrPolygon.Add(new List<List<Polygon>>());
                objFile.GetFirstSection("Type" + h);
                int intTemplateCount = objFile.GetSecondSectionCount();
                for (int i = 0; i < intTemplateCount; i++)
                {
                    arrPolygon[h].Add(new List<Polygon>());
                    objFile.GetSecondSection("Image" + i);
                    int intImageCount = objFile.GetThirdSectionCount();
                    for (int a = 0; a < intImageCount; a++)
                    {
                        arrPolygon[h][i].Add(new Polygon());
                        objFile.GetThirdSection("Template" + a);
                        arrPolygon[h][i][a].ref_intFormMode = objFile.GetValueAsInt("FormMode", 0, 3);
                        int intPolygonCount = objFile.GetFourthSectionCount();
                        for (int j = 0; j < intPolygonCount; j++)
                        {
                            int intROICenterX = objFile.GetValueAsInt("ROICenterX", 0, 3);
                            int intROICenterY = objFile.GetValueAsInt("ROICenterY", 0, 3);

                            objFile.GetFourthSection("Polygon" + j);

                            int intPointCount = objFile.GetFifthSectionCount();
                            for (int k = 0; k < intPointCount; k++)
                            {
                                strPoint = objFile.GetValueAsString("Point" + k, "0,0", 4).Split(',');
                                if (strPoint.Length == 2)
                                {
                                    p = new PointF(float.Parse(strPoint[0]), float.Parse(strPoint[1]));
                                    arrPolygon[h][i][a].AddPointFromFile(p);
                                }
                            }

                            objFile.GetFourthSection("Offset" + j);
                            int intPointOffsetCount = objFile.GetFifthSectionCount();
                            for (int k = 0; k < intPointOffsetCount; k++)
                            {
                                strPoint = objFile.GetValueAsString("PointOffset" + k, "0,0", 4).Split(',');
                                if (strPoint.Length == 2)
                                {
                                    p = new PointF(float.Parse(strPoint[0]), float.Parse(strPoint[1]));
                                    arrPolygon[h][i][a].AddOffsetPointFromFile(p);
                                }
                            }

                            arrPolygon[h][i][a].AddPolygon(intROICenterX, intROICenterY);
                        }
                    }
                }
            }
        }
        public void UndoPolygon()
        {
            if (m_arrPoint.Count > 0)
            {
                m_arrPoint.Clear();
            }
            else
            {
                int intLastPolygonIndex = m_arrPolygons.Count - 1;
                if (intLastPolygonIndex >= 0)
                {
                    m_arrPolygons.RemoveAt(intLastPolygonIndex);
                    m_arrPolygonsOffset.RemoveAt(intLastPolygonIndex);
                }
            }
            if (m_arrPointsOffset.Count > 0)
            {
                m_arrPointsOffset.Clear();
            }
        }



        /// <summary>
        /// Fill color into polygons
        /// </summary>
        /// <param name="objImage">image source</param>
        /// <param name="Polygons">all polygons points position</param>
        /// <param name="intPixelValue">gray pixel value that will be used to fill shape</param>
        private void FillPolygon(ImageDrawing objImage, List<PointF[]> Polygons, int intPixelValue)
        {
            foreach (PointF[] polygon in Polygons)
            {
                double dbMinX = double.MaxValue;
                double dbMinY = double.MaxValue;
                double dbMaxX = double.MinValue;
                double dbMaxY = double.MinValue;

                m_Lines.Clear();
                int ni;
                for (int i = 0; i < polygon.Length; i++)
                {
                    dbMinX = dbMinX < polygon[i].X ? dbMinX : polygon[i].X;     // if dbMinX < polygon[i].X is true, dbMinX = dbMinX; if false, dbMinX = polygon[i].X
                    dbMinY = dbMinY < polygon[i].Y ? dbMinY : polygon[i].Y;
                    dbMaxX = dbMaxX > polygon[i].X ? dbMaxX : polygon[i].X;
                    dbMaxY = dbMaxY > polygon[i].Y ? dbMaxY : polygon[i].Y;
                    ni = i + 1 < polygon.Length ? i + 1 : 0;
                    m_Lines.Add(new polyLine(polygon[i], polygon[ni]));   // Link 2 points to be a line, p0 -> p1, p1->p2, p3->p0
                }

                int MaxX = (int)Math.Ceiling(dbMaxX);
                int MaxY = (int)Math.Ceiling(dbMaxY);
                int MinX = (int)Math.Floor(dbMinX);
                int MinY = (int)Math.Floor(dbMinY);
                EBW8 px = new EBW8((byte)intPixelValue);    // the gray color value that will be filled into all selected pixel

                for (int i = MinY; i <= MaxY; i++)
                {
                    List<double> CrossedX = GetCrossedPoints(i);

                    if (CrossedX.Count % 2 == 1)
                        continue;
                    else
                    {
                        for (int j = 0; j < CrossedX.Count; j += 2)
                        {
                            int MinColumn = (int)Math.Ceiling(CrossedX[j]);
                            int MaxColumn = (int)Math.Floor(CrossedX[j + 1]);
                            for (int k = MinColumn; k <= MaxColumn; k++)
                            {
                                objImage.ref_objMainImage.SetPixel(px, k, i);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fill color into polygons
        /// </summary>
        /// <param name="objImage">image source</param>
        /// <param name="Polygons">all polygons points position</param>
        /// <param name="intPixelValue">gray pixel value that will be used to fill shape</param>
        private void FillPolygonWithScale(ImageDrawing objImage, List<PointF[]> Polygons, int intPixelValue, float fScaleX, float fScaleY, float fROIStartX, float fROIStartY)
        {
            foreach (PointF[] polygon in Polygons)
            {
                double dbMinX = double.MaxValue;
                double dbMinY = double.MaxValue;
                double dbMaxX = double.MinValue;
                double dbMaxY = double.MinValue;

                m_Lines.Clear();
                int ni;
                for (int i = 0; i < polygon.Length; i++)
                {
                    dbMinX = dbMinX < (polygon[i].X - fROIStartX) / fScaleX ? dbMinX : (polygon[i].X - fROIStartX) / fScaleX;     // if dbMinX < (polygon[i].X - fROIStartX) is true, dbMinX = dbMinX; if false, dbMinX = (polygon[i].X - fROIStartX)
                    dbMinY = dbMinY < (polygon[i].Y - fROIStartY) / fScaleY ? dbMinY : (polygon[i].Y - fROIStartY) / fScaleY;
                    dbMaxX = dbMaxX > (polygon[i].X - fROIStartX) / fScaleX ? dbMaxX : (polygon[i].X - fROIStartX) / fScaleX;
                    dbMaxY = dbMaxY > (polygon[i].Y - fROIStartY) / fScaleY ? dbMaxY : (polygon[i].Y - fROIStartY) / fScaleY;
                    ni = i + 1 < polygon.Length ? i + 1 : 0;
                    m_Lines.Add(new polyLine(new PointF(Math.Max(0, (polygon[i].X - fROIStartX) / fScaleX), Math.Max(0, (polygon[i].Y - fROIStartY) / fScaleY)), new PointF(Math.Max(0, (polygon[ni].X - fROIStartX) / fScaleX), Math.Max(0, (polygon[ni].Y - fROIStartY) / fScaleY))));   // Link 2 points to be a line, p0 -> p1, p1->p2, p3->p0
                }

                int MaxX = Math.Max(0, (int)Math.Ceiling(dbMaxX));
                int MaxY = Math.Max(0, (int)Math.Ceiling(dbMaxY));
                int MinX = Math.Max(0, (int)Math.Floor(dbMinX));
                int MinY = Math.Max(0, (int)Math.Floor(dbMinY));
                EBW8 px = new EBW8((byte)intPixelValue);    // the gray color value that will be filled into all selected pixel

                for (int i = MinY; i <= MaxY; i++)
                {
                    List<double> CrossedX = GetCrossedPoints(i);

                    if (CrossedX.Count % 2 == 1)
                        continue;
                    else
                    {
                        for (int j = 0; j < CrossedX.Count; j += 2)
                        {
                            int MinColumn = (int)Math.Floor(CrossedX[j]); //Ceiling
                            int MaxColumn = (int)Math.Ceiling(CrossedX[j + 1]); //Floor ; j + 1

                            
                            if (m_intFormMode != 2) //2020-03-19 ZJYEOH : For rectangle and circle, directly fill from start X to End X
                            {
                                 MinColumn = (int)Math.Floor(CrossedX[j]); //Ceiling
                                 MaxColumn = (int)Math.Ceiling(CrossedX[CrossedX.Count - 1]); //Floor ; j + 1
                            }

                            for (int k = MinColumn; k <= MaxColumn; k++)
                            {
                                objImage.ref_objMainImage.SetPixel(px, k, i);
                            }
                        }
                    }
                }
            }
        }
        private void FillPolygonWithScale(ImageDrawing objImage, List<PointF[]> Polygons, int intPixelValue, float fScaleX, float fScaleY, float fROIStartX, float fROIStartY, float fROIEndX, float fROIEndY)
        {
            foreach (PointF[] polygon in Polygons)
            {
                double dbMinX = double.MaxValue;
                double dbMinY = double.MaxValue;
                double dbMaxX = double.MinValue;
                double dbMaxY = double.MinValue;

                //2021-03-09 ZJYEOH : Extend 10% size when user drag rectangle polygon to right or bottom edge
                if(m_intFormMode == 0)
                {
                    for (int i = 0; i < polygon.Length; i++)
                    {
                        if (polygon[i].X == fROIEndX)
                            if (objImage.ref_objMainImage.Width > (((polygon[i].X * 1.1f) - fROIStartX) / fScaleX))
                                polygon[i].X *= 1.1f;

                        if (polygon[i].Y == fROIEndY)
                            if (objImage.ref_objMainImage.Height > (((polygon[i].Y * 1.1f) - fROIStartY) / fScaleY))
                                polygon[i].Y *= 1.1f;
                    }
                }

                m_Lines.Clear();
                int ni;
                for (int i = 0; i < polygon.Length; i++)
                {
                    dbMinX = dbMinX < (polygon[i].X - fROIStartX) / fScaleX ? dbMinX : (polygon[i].X - fROIStartX) / fScaleX;     // if dbMinX < (polygon[i].X - fROIStartX) is true, dbMinX = dbMinX; if false, dbMinX = (polygon[i].X - fROIStartX)
                    dbMinY = dbMinY < (polygon[i].Y - fROIStartY) / fScaleY ? dbMinY : (polygon[i].Y - fROIStartY) / fScaleY;
                    dbMaxX = dbMaxX > (polygon[i].X - fROIStartX) / fScaleX ? dbMaxX : (polygon[i].X - fROIStartX) / fScaleX;
                    dbMaxY = dbMaxY > (polygon[i].Y - fROIStartY) / fScaleY ? dbMaxY : (polygon[i].Y - fROIStartY) / fScaleY;
                    ni = i + 1 < polygon.Length ? i + 1 : 0;
                    m_Lines.Add(new polyLine(new PointF(Math.Max(0, (polygon[i].X - fROIStartX) / fScaleX), Math.Max(0, (polygon[i].Y - fROIStartY) / fScaleY)), new PointF(Math.Max(0, (polygon[ni].X - fROIStartX) / fScaleX), Math.Max(0, (polygon[ni].Y - fROIStartY) / fScaleY))));   // Link 2 points to be a line, p0 -> p1, p1->p2, p3->p0
                }

                int MaxX = Math.Max(0, (int)Math.Ceiling(dbMaxX));
                int MaxY = Math.Max(0, (int)Math.Ceiling(dbMaxY));
                int MinX = Math.Max(0, (int)Math.Floor(dbMinX));
                int MinY = Math.Max(0, (int)Math.Floor(dbMinY));
                EBW8 px = new EBW8((byte)intPixelValue);    // the gray color value that will be filled into all selected pixel

                for (int i = MinY; i <= MaxY; i++)
                {
                    List<double> CrossedX = GetCrossedPoints(i);

                    if (CrossedX.Count % 2 == 1)
                        continue;
                    else
                    {
                        for (int j = 0; j < CrossedX.Count; j += 2)
                        {
                            int MinColumn = (int)Math.Floor(CrossedX[j]); //Ceiling
                            int MaxColumn = (int)Math.Ceiling(CrossedX[j + 1]); //Floor ; j + 1


                            if (m_intFormMode != 2) //2020-03-19 ZJYEOH : For rectangle and circle, directly fill from start X to End X
                            {
                                MinColumn = (int)Math.Floor(CrossedX[j]); //Ceiling
                                MaxColumn = (int)Math.Ceiling(CrossedX[CrossedX.Count - 1]); //Floor ; j + 1
                            }

                            for (int k = MinColumn; k <= MaxColumn; k++)
                            {
                                objImage.ref_objMainImage.SetPixel(px, k, i);
                            }
                        }
                    }
                }
            }
        }
        private void FillPolygon(ROI objROI, List<PointF[]> Polygons, int intPixelValue)
        {
            foreach (PointF[] polygon in Polygons)
            {
                double dbMinX = double.MaxValue;
                double dbMinY = double.MaxValue;
                double dbMaxX = double.MinValue;
                double dbMaxY = double.MinValue;

                m_Lines.Clear();
                int ni;
                for (int i = 0; i < polygon.Length; i++)
                {
                    dbMinX = dbMinX < polygon[i].X ? dbMinX : polygon[i].X;     // if dbMinX < polygon[i].X is true, dbMinX = dbMinX; if false, dbMinX = polygon[i].X
                    dbMinY = dbMinY < polygon[i].Y ? dbMinY : polygon[i].Y;
                    dbMaxX = dbMaxX > polygon[i].X ? dbMaxX : polygon[i].X;
                    dbMaxY = dbMaxY > polygon[i].Y ? dbMaxY : polygon[i].Y;
                    ni = i + 1 < polygon.Length ? i + 1 : 0;
                    m_Lines.Add(new polyLine(polygon[i], polygon[ni]));   // Link 2 points to be a line, p0 -> p1, p1->p2, p3->p0
                }

                int MaxX = (int)Math.Ceiling(dbMaxX);
                int MaxY = (int)Math.Ceiling(dbMaxY);
                int MinX = (int)Math.Floor(dbMinX);
                int MinY = (int)Math.Floor(dbMinY);
                EBW8 px = new EBW8((byte)intPixelValue);    // the gray color value that will be filled into all selected pixel

                for (int i = MinY; i <= MaxY; i++)
                {
                    List<double> CrossedX = GetCrossedPoints(i);

                    if (CrossedX.Count % 2 == 1)
                        continue;
                    else
                    {
                        for (int j = 0; j < CrossedX.Count; j += 2)
                        {
                            int MinColumn = (int)Math.Ceiling(CrossedX[j]);
                            int MaxColumn = (int)Math.Floor(CrossedX[j + 1]);
                            for (int k = MinColumn; k <= MaxColumn; k++)
                            {
                                objROI.ref_ROI.SetPixel(px, k, i);
                            }
                        }
                    }
                }
            }
        }

        public void CheckDontCarePosition(ROI objROI, float fScaleX, float fScaleY)
        {
            bool blnOffsetXChanged = false;
            bool blnOffsetYChanged = false;
            for (int i = m_arrPolygons.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < m_arrPolygons[i].Length; j++)
                {
                    if ((int)(objROI.ref_ROITotalCenterX * fScaleX) != m_arrPolygonsOffset[i].X)
                    {
                        m_arrPolygons[i][j].X += (int)(objROI.ref_ROITotalCenterX * fScaleX) - m_arrPolygonsOffset[i].X;
                        blnOffsetXChanged = true;
                    }

                    if ((int)(objROI.ref_ROITotalCenterY * fScaleY) != m_arrPolygonsOffset[i].Y)
                    {
                        m_arrPolygons[i][j].Y += (int)(objROI.ref_ROITotalCenterY * fScaleY) - m_arrPolygonsOffset[i].Y;
                        blnOffsetYChanged = true;
                    }
                }

                if (blnOffsetXChanged)
                    m_arrPolygonsOffset[i] = new Point((int)(objROI.ref_ROITotalCenterX * fScaleX), m_arrPolygonsOffset[i].Y);

                if (blnOffsetYChanged)
                    m_arrPolygonsOffset[i] = new Point(m_arrPolygonsOffset[i].X, (int)(objROI.ref_ROITotalCenterY * fScaleY));
            }
        }

        private List<double> GetCrossedPoints(int y)
        {
            List<double> m_CrossedX = new List<double>();
            for (int i = 0; i < m_Lines.Count; i++)
            {
                if (y <= m_Lines[i].MaxY && y >= m_Lines[i].MinY)
                {
                    //if (m_Lines[i].M == 0)
                    //    break;
                    //else if (double.IsInfinity(m_Lines[i].M))
                    if (double.IsInfinity(m_Lines[i].M))
                        m_CrossedX.Add(m_Lines[i].MaxX);         // if kecerunan = 0, mean 2 points have same positionX
                    else
                        m_CrossedX.Add(((double)y - m_Lines[i].C) / m_Lines[i].M);    // x = (y - c) / m
                }
            }
            m_CrossedX.Sort();

            if (m_CrossedX.Count >= 3)
            {
                if (m_CrossedX.Count % 2 == 1)
                {
                    for (int i = 0; i < m_CrossedX.Count - 1; i++)
                    {
                        if (m_CrossedX[i].ToString() == "NaN")
                        {
                            m_CrossedX.RemoveAt(i);
                            break;
                        }
                        else if (Math.Abs(m_CrossedX[i] - m_CrossedX[i + 1]) < 1e-5)
                        {
                            double X = 0.5 * (m_CrossedX[i] + m_CrossedX[i + 1]);
                            m_CrossedX.RemoveRange(i, 2);
                            m_CrossedX.Add(X);
                            m_CrossedX.Sort();
                            break;
                        }
                    }
                }
            }
            return m_CrossedX;
        }

        public PointF PointOnCircle(float radius, float angleInDegrees, PointF origin)
        {
            // Convert from degrees to radians via multiplication by PI/180        
            float x = (float)(radius * Math.Cos(angleInDegrees * Math.PI / 180F)) + origin.X;
            float y = (float)(radius * Math.Sin(angleInDegrees * Math.PI / 180F)) + origin.Y;

            return new PointF(x, y);
        }

        public PointF PointOnOval(float radiusX, float radiusY, float angleInDegrees, PointF origin)
        {
            // Convert from degrees to radians via multiplication by PI/180        
            float x = (float)(radiusX * Math.Cos(angleInDegrees * Math.PI / 180F)) + origin.X;
            float y = (float)(radiusY * Math.Sin(angleInDegrees * Math.PI / 180F)) + origin.Y;

            return new PointF(x, y);
        }

        public void Dispose()
        {
            m_brushBlack.Dispose();
            m_brushLime.Dispose();
            m_penAqua.Dispose();
            //    private List<PointF> m_arrCirCornerPoints = new List<PointF>();
            //private List<polyLine> m_Lines = new List<polyLine>();
            //private List<PointF> m_arrPoint = new List<PointF>();
            //private List<PointF[]> m_arrPolygons = new List<PointF[]>();
            //private Pen m_penAqua = new Pen(Color.Aqua);
            //private SolidBrush m_brushBlack = new SolidBrush(Color.Black);
            //private SolidBrush m_brushLime = new SolidBrush(Color.Lime);
        }

        public static bool CreateDontCareImageWithPolygonPattern(ROI objROI, Polygon objPolygon, ImageDrawing objBlackImage, ref ImageDrawing objDontCareImage, float fScaleX, float fScaleY)
        {
            try
            {
                objBlackImage.CopyTo(ref objDontCareImage);
                float fROIStartX = (float)objROI.ref_ROITotalX * fScaleX;
                float fROIStartY = (float)objROI.ref_ROITotalY * fScaleY;
                //for (int h = 0; h < arrPolygon.Count; h++)
                //{
                //    for (int i = 0; i < arrPolygon[h].Count; i++)
                //    {
                //        if (arrPolygon[h][i].GetPolygonCount() > 0)
                //        {
                //            // Fill polygon on image by changing pixel
                //            arrPolygon[h][i].FillPolygonOnImageWithScale(ref objDontCareImage, 255, fScaleX, fScaleY, fROIStartX, fROIStartY);
                //        }
                //    }
                //}

                if (objPolygon.GetPolygonCount() > 0)
                {
                    // Fill polygon on image by changing pixel
                    objPolygon.FillPolygonOnImageWithScale(ref objDontCareImage, 255, fScaleX, fScaleY, fROIStartX, fROIStartY);
                }

            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                return false;
            }
            return true;
        }
        public static bool CreateDontCareImageWithPolygonPattern_ExtendEdge(ROI objROI, Polygon objPolygon, ImageDrawing objBlackImage, ref ImageDrawing objDontCareImage, float fScaleX, float fScaleY)
        {
            try
            {
                objBlackImage.CopyTo(ref objDontCareImage);
                float fROIStartX = (float)objROI.ref_ROITotalX * fScaleX;
                float fROIStartY = (float)objROI.ref_ROITotalY * fScaleY;
                float fROIEndX = (float)(objROI.ref_ROITotalX + objROI.ref_ROIWidth) * fScaleX;
                float fROIEndY = (float)(objROI.ref_ROITotalY + objROI.ref_ROIHeight) * fScaleY;
                //for (int h = 0; h < arrPolygon.Count; h++)
                //{
                //    for (int i = 0; i < arrPolygon[h].Count; i++)
                //    {
                //        if (arrPolygon[h][i].GetPolygonCount() > 0)
                //        {
                //            // Fill polygon on image by changing pixel
                //            arrPolygon[h][i].FillPolygonOnImageWithScale(ref objDontCareImage, 255, fScaleX, fScaleY, fROIStartX, fROIStartY);
                //        }
                //    }
                //}

                if (objPolygon.GetPolygonCount() > 0)
                {
                    // Fill polygon on image by changing pixel
                    objPolygon.FillPolygonOnImageWithScale(ref objDontCareImage, 255, fScaleX, fScaleY, fROIStartX, fROIStartY, fROIEndX, fROIEndY);
                }

            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                return false;
            }
            return true;
        }
    }


    internal class polyLine
    {
        private bool m_IsPerpendicular = false;
        private double m_dbM;
        private double m_dbC;
        private double m_dbMaxX;
        private double m_dbMaxY;
        private double m_dbMinX;
        private double m_dbMinY;

        public polyLine(PointF p1, PointF p2)
        {
            m_dbMaxX = p1.X > p2.X ? p1.X : p2.X;
            m_dbMinX = p1.X < p2.X ? p1.X : p2.X;
            m_dbMaxY = p1.Y > p2.Y ? p1.Y : p2.Y;
            m_dbMinY = p1.Y < p2.Y ? p1.Y : p2.Y;

            if (Math.Abs(p1.X - p2.X) < 1e-10)
            {
                m_IsPerpendicular = true;          // they form an L-shape where angle is 90 deg
                m_dbM = double.PositiveInfinity;
                m_dbC = double.PositiveInfinity;
            }
            else
            {
                m_dbM = (double)(p1.Y - p2.Y) / (double)(p1.X - p2.X);     // kecerunan = y/x
                m_dbC = (double)p1.Y - m_dbM * (double)p1.X;                // y = mx+c -> c= y-mx, can use second point as reference
            }
        }

        public bool IsPerpendicular { get { return m_IsPerpendicular; } }
        public double M { get { return m_dbM; } }
        public double C { get { return m_dbC; } }
        public double MinX { get { return m_dbMinX; } }
        public double MinY { get { return m_dbMinY; } }
        public double MaxX { get { return m_dbMaxX; } }
        public double MaxY { get { return m_dbMaxY; } }
        
    }
}
