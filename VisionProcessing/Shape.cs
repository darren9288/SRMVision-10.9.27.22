using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif

namespace VisionProcessing
{
    public class Shape
    {
        public static EImageBW8 m_objWhiteImage = new EImageBW8(10, 10);
        public static EImageC24 m_objWhiteColorImage = new EImageC24(10, 10);

        public static void InitVariables()
        {
            EBW8 px = new EBW8();
            px.Value = 255;    // the gray color value that will be filled into all selected pixel

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    m_objWhiteImage.SetPixel(px, x, y);
                }
            }
        }

        public static void InitVariables(int intWidth, int intHeight)
        {
            m_objWhiteImage.SetSize(intWidth, intHeight);
            EasyImage.Copy(new EBW8(255), m_objWhiteImage); // Better way to set whole image to white color.
            m_objWhiteColorImage.SetSize(intWidth, intHeight);
            EasyImage.Copy(new EC24(255, 255, 255), m_objWhiteColorImage); // Better way to set whole image to white color.
        }

        /// <summary>
        /// Fill color into Rectangle
        /// </summary>
        /// <param name="objImage">image source</param>
        /// <param name="intStartX">Start Point X of Top Left of rectangle</param>
        /// <param name="intStartY">Start Point Y of Top Left of rectangle</param>
        /// <param name="intEndX">End Point X of Bottom Right of rectangle</param>
        /// <param name="intEndY">End Point Y of Bottom Right of rectangle</param>
        /// <param name="intPixelvalue">color that will be filled in rectangle</param>
        public static void FillRectangle(ImageDrawing objImage, int intStartX, int intStartY, int intEndX, int intEndY, int intPixelValue)
        {
            // Reject this method because it consume too much time. E.g for large image, it consume around 100ms compare to new Subtract/Add sequence just consume around 1ms.
            //BW8 px = new BW8();
            //px.Value = intPixelValue;    // the gray color value that will be filled into all selected pixel

            //for (int x = intStartX; x <= intEndX; x++)
            //{
            //    for (int y = intStartY; y <= intEndY; y++)
            //        objImage.ref_objMainImage.SetPixel(px, x, y);
            //}

            // 2020 08 17 - CCENG: Use LoadROISetting instead of SetPlacement. SetPlacement do not consider the parent size. This wiil cause ROI out of parent error.
            EROIBW8 objROI = new EROIBW8();
            objROI.Attach(objImage.ref_objMainImage);
            ROI.LoadROISetting(ref objROI, intStartX, intStartY, intEndX - intStartX, intEndY - intStartY);
            EROIBW8 objROIWhite = new EROIBW8();
            objROIWhite.Attach(m_objWhiteImage);
            ROI.LoadROISetting(ref objROIWhite, 0, 0, intEndX - intStartX, intEndY - intStartY);

            if (objROI.Width > 0 && objROI.Height > 0)
            {
                if (objROI.Width == objROIWhite.Width && objROI.Height == objROIWhite.Height)
                {
                    if (intPixelValue == 0)
                    {
                        EasyImage.Oper(EArithmeticLogicOperation.Subtract, objROI, objROIWhite, objROI);
                    }
                    else
                    {
                        EasyImage.Oper(EArithmeticLogicOperation.Add, objROI, objROIWhite, objROI);
                    }
                }
            }

            objROI.Dispose();
            objROIWhite.Dispose();

        }
        public static void FillRectangle(CImageDrawing objImage, int intStartX, int intStartY, int intEndX, int intEndY, int intPixelValue)
        {
            // Reject this method because it consume too much time. E.g for large image, it consume around 100ms compare to new Subtract/Add sequence just consume around 1ms.
            //BW8 px = new BW8();
            //px.Value = intPixelValue;    // the gray color value that will be filled into all selected pixel

            //for (int x = intStartX; x <= intEndX; x++)
            //{
            //    for (int y = intStartY; y <= intEndY; y++)
            //        objImage.ref_objMainImage.SetPixel(px, x, y);
            //}

            // 2020 08 17 - CCENG: Use LoadROISetting instead of SetPlacement. SetPlacement do not consider the parent size. This wiil cause ROI out of parent error.
            EROIC24 objROI = new EROIC24();
            objROI.Attach(objImage.ref_objMainCImage);
            CROI.LoadROISetting(ref objROI, intStartX, intStartY, intEndX - intStartX, intEndY - intStartY);
            EROIC24 objROIWhite = new EROIC24();
            objROIWhite.Attach(m_objWhiteColorImage);
            CROI.LoadROISetting(ref objROIWhite, 0, 0, intEndX - intStartX, intEndY - intStartY);

            if (objROI.Width > 0 && objROI.Height > 0)
            {
                if (objROI.Width == objROIWhite.Width && objROI.Height == objROIWhite.Height)
                {
                    if (intPixelValue == 0)
                    {
                        EasyImage.Oper(EArithmeticLogicOperation.Subtract, objROI, objROIWhite, objROI);
                    }
                    else
                    {
                        EasyImage.Oper(EArithmeticLogicOperation.Add, objROI, objROIWhite, objROI);
                    }
                }
            }

            objROI.Dispose();
            objROIWhite.Dispose();

        }
        public static void FillPolygonWithScale(ImageDrawing objImage, List<Point> Polygons, int intPixelValue, float fScaleX, float fScaleY, float fROIStartX, float fROIStartY)
        {
            //for (int i = 0; i < Polygons.Count; i++)
            {
                double dbMinX = double.MaxValue;
                double dbMinY = double.MaxValue;
                double dbMaxX = double.MinValue;
                double dbMaxY = double.MinValue;

                List<polyLine> m_Lines = new List<polyLine>();
                int ni;
                for (int a = 0; a < Polygons.Count; a++)
                {
                    dbMinX = dbMinX < (Polygons[a].X - fROIStartX) / fScaleX ? dbMinX : (Polygons[a].X - fROIStartX) / fScaleX;     // if dbMinX < (polygon[i].X - fROIStartX) is true, dbMinX = dbMinX; if false, dbMinX = (polygon[i].X - fROIStartX)
                    dbMinY = dbMinY < (Polygons[a].Y - fROIStartY) / fScaleY ? dbMinY : (Polygons[a].Y - fROIStartY) / fScaleY;
                    dbMaxX = dbMaxX > (Polygons[a].X - fROIStartX) / fScaleX ? dbMaxX : (Polygons[a].X - fROIStartX) / fScaleX;
                    dbMaxY = dbMaxY > (Polygons[a].Y - fROIStartY) / fScaleY ? dbMaxY : (Polygons[a].Y - fROIStartY) / fScaleY;
                    ni = a + 1 < Polygons.Count ? a + 1 : 0;
                    m_Lines.Add(new polyLine(new PointF(Math.Max(0, (Polygons[a].X - fROIStartX) / fScaleX), Math.Max(0, (Polygons[a].Y - fROIStartY) / fScaleY)), new PointF(Math.Max(0, (Polygons[ni].X - fROIStartX) / fScaleX), Math.Max(0, (Polygons[ni].Y - fROIStartY) / fScaleY))));   // Link 2 points to be a line, p0 -> p1, p1->p2, p3->p0
                }

                int MaxX = Math.Max(0, (int)Math.Ceiling(dbMaxX));
                int MaxY = Math.Max(0, (int)Math.Ceiling(dbMaxY));
                int MinX = Math.Max(0, (int)Math.Floor(dbMinX));
                int MinY = Math.Max(0, (int)Math.Floor(dbMinY));
                EBW8 px = new EBW8((byte)intPixelValue);    // the gray color value that will be filled into all selected pixel

                for (int a = MinY; a <= MaxY; a++)
                {
                    List<double> CrossedX = GetCrossedPoints(a, m_Lines);

                    if (CrossedX.Count % 2 == 1)
                        continue;
                    else
                    {
                        for (int j = 0; j < CrossedX.Count; j += 2)
                        {
                            int MinColumn = (int)Math.Floor(CrossedX[j]); //Ceiling
                            int MaxColumn = (int)Math.Ceiling(CrossedX[j + 1]); //Floor ; j + 1


                            //if (m_intFormMode != 2) //2020-03-19 ZJYEOH : For rectangle and circle, directly fill from start X to End X
                            //{
                                //MinColumn = (int)Math.Floor(CrossedX[j]); //Ceiling
                                //MaxColumn = (int)Math.Ceiling(CrossedX[CrossedX.Count - 1]); //Floor ; j + 1
                            //}

                            for (int k = MinColumn; k <= MaxColumn; k++)
                            {
                                objImage.ref_objMainImage.SetPixel(px, k, a);
                            }
                        }
                    }
                }
            }
        }
        private static List<double> GetCrossedPoints(int y, List<polyLine> m_Lines)
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
        /// <summary>
        /// Fill color into Rectangle
        /// </summary>
        /// <param name="objImage">image source</param>
        /// <param name="objROI">target ROI that will be edited</param>
        /// <param name="intPixelValue">gray pixel value that will be used to fill shape</param>
        public static void FillRectangle(ImageDrawing objImage, ROI objROI, int intPixelValue)
        {
            EBW8 px = new EBW8((byte)intPixelValue);    // the gray color value that will be filled into all selected pixel

            objROI.AttachImage(objImage);
            int intX = objROI.ref_ROIWidth;
            int intY =objROI.ref_ROIHeight;
            for (int x = 0; x <= intX; x++)
            {
                for (int y = 0; y <= intY; y++)
                    objROI.ref_ROI.SetPixel(px, x, y);
            }            
        }


        /// <summary>
        /// Fill color into Rectangle
        /// </summary>
        /// <param name="objParent">parent of target ROI</param>
        /// <param name="objROI">target ROI that will be edited</param>
        /// <param name="intPixelValue">gray pixel value that will be used to fill shape</param>
        public static void FillRectangle(ROI objParent, ROI objROI, int intPixelValue)
        {
            EBW8 px = new EBW8((byte)intPixelValue);    // the gray color value that will be filled into all selected pixel

            objROI.AttachImage(objParent);
            int intX = objROI.ref_ROIWidth;
            int intY = objROI.ref_ROIHeight;
            for (int x = 0; x <= intX; x++)
            {
                for (int y = 0; y <= intY; y++)
                    objROI.ref_ROI.SetPixel(px, x, y);
            }
        }

        /// <summary>
        /// Fill color into Rectangle with black color
        /// </summary>
        /// <param name="objParent">parent of target ROI</param>
        /// <param name="objROI">target ROI that will be edited</param>
        /// <param name="intColorIndex">1= black, 2 = white, 3=red, 4= green, 5 = blue, 6 = yellow</param>
        public static void FillRectangle(CROI objParent, CROI objROI, int intColorIndex)
        {
            EC24 px = new EC24();
            switch (intColorIndex)
            {
                case 1: px.C0 = 0;
                    px.C1 = 0;
                    px.C2 = 0;
                    break;
                case 2: px.C0 = 255;
                    px.C1 = 255;
                    px.C2 = 255;
                    break;
                case 3: px.C0 = 255;
                    px.C1 = 0;
                    px.C2 = 0;
                    break;
                case 4: px.C0 = 0;
                    px.C1 = 192;
                    px.C2 = 0;
                    break;
                case 5: px.C0 = 0;
                    px.C1 = 0;
                    px.C2 = 255;
                    break;
                case 6: px.C0 = 255;
                    px.C1 = 255;
                    px.C2 = 0;
                    break;
            }
           

            objROI.AttachImage(objParent);
            int intX = objROI.ref_ROIWidth;
            int intY = objROI.ref_ROIHeight;
            for (int x = 0; x <= intX; x++)
            {
                for (int y = 0; y <= intY; y++)
                    objROI.ref_CROI.SetPixel(px, x, y);
            }
        }

        public static void FillRectangle(ROI objROI, int intPixelValue)
        {
            EBW8 px = new EBW8((byte)intPixelValue);    // the gray color value that will be filled into all selected pixel

            int intX = objROI.ref_ROIWidth;
            int intY = objROI.ref_ROIHeight;
            for (int x = 0; x <= intX; x++)
            {
                for (int y = 0; y <= intY; y++)
                    objROI.ref_ROI.SetPixel(px, x, y);
            }
        }


       
    }


   
}
