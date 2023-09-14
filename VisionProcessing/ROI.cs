using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;
using System.Windows.Forms;

namespace VisionProcessing
{

    public class ROI
    {
        #region enum

        public enum AreaColor { Black = 0, White = 1 };

        #endregion

        #region Member Variables
        private int m_intStartOffsetX = 0;
        private int m_intStartOffsetY = 0;
        private int m_intType;        // 1 = SearchROI, 2 = TrainROI, 3 = Don't care area, 4 = SubROI
        private int m_intUnitArea;
        private int m_intEmptyPocketArea;
        private float m_fPixelAverage;
        private float m_fEmptyPocketPixel;
        private float m_fDrawingScaleX = 1.0f;
        private float m_fDrawingScaleY = 1.0f;
        private string m_strROIName;

        private int m_intOriPositionX = 0;
        private int m_intOriPositionY = 0;
        private int m_intOriWidth = 0, m_intOriHeight = 0;

        private Color[] m_Color = new Color[]{Color.Lime, Color.Yellow, Color.Lime, Color.Pink, Color.White, Color.Cyan,
            Color.Plum, Color.Honeydew, Color.LawnGreen, Color.Ivory, Color.Cornsilk, Color.DarkOrange, Color.Red};
        private EDragHandle m_Handler;
        private EDragHandle m_Handler2;
        private Font m_Font = new Font("Verdana", 10);
        private EROIBW8 m_ROI;
        private EBW8 m_bw = new EBW8();
        private EImageBW8 m_objParentImage;
        private ERGBColor m_objRGBColor = new ERGBColor(0, 0, 0);

        // Local Use
        private EImageBW8 m_objZoomImage;
        private EImageBW8 m_objDisplayImage;

        #endregion

        #region Properties
        public int ref_intStartOffsetX { get { return m_intStartOffsetX; } set { m_intStartOffsetX = value; } }
        public int ref_intStartOffsetY { get { return m_intStartOffsetY; } set { m_intStartOffsetY = value; } }
        public float ref_fAreaPixel { get { return m_fPixelAverage; } }
        public float ref_fEmptyPocketPixel { get { return m_fEmptyPocketPixel; } }
        public float ref_fDrawingScaleX { get { return m_fDrawingScaleX; } set { m_fDrawingScaleX = value; } }
        public float ref_fDrawingScaleY { get { return m_fDrawingScaleY; } set { m_fDrawingScaleY = value; } }
        public int ref_intUnitArea { get { return m_intUnitArea; } set { m_intUnitArea = value; } }
        public int ref_intEmptyPocketArea { get { return m_intEmptyPocketArea; } set { m_intEmptyPocketArea = value; } }
        public int ref_intType { get { return m_intType; } set { m_intType = value; } }
        public int ref_ROIPositionX { get { return m_ROI.OrgX; } set { m_ROI.OrgX = value; } }
        public int ref_ROIPositionY { get { return m_ROI.OrgY; } set { m_ROI.OrgY = value; } }
        public int ref_ROICenterX { get { return m_ROI.OrgX + m_ROI.Width / 2; } }
        public int ref_ROICenterY { get { return m_ROI.OrgY + m_ROI.Height / 2; } }
        public int ref_ROITotalCenterX { get { return m_ROI.TotalOrgX + m_ROI.Width / 2; } }
        public int ref_ROITotalCenterY { get { return m_ROI.TotalOrgY + m_ROI.Height / 2; } }
        public int ref_ROIHeight { get { return m_ROI.Height; } set { m_ROI.Height = value; } }
        public int ref_ROIWidth { get { return m_ROI.Width; } set { m_ROI.Width = value; } }
        public int ref_ROIOriHeight { get { return m_intOriHeight; } }
        public int ref_ROIOriWidth { get { return m_intOriWidth; } }
        public int ref_ROITotalX
        {
            get
            {
                if (m_ROI.TopParent == null)
                    return 0;
                else
                    return m_ROI.TotalOrgX;
            }
        }
        public int ref_ROITotalY
        { get
            {
                if (m_ROI.TopParent == null)
                    return 0;
                else
                    return m_ROI.TotalOrgY;
            }
        }
        public int ref_ROIOriPositionX { get { return m_intOriPositionX; } set { m_intOriPositionX = value; } }
        public int ref_ROIOriPositionY { get { return m_intOriPositionY; } set { m_intOriPositionY = value; } }
        public string ref_strROIName
        {
            get
            {
                return m_strROIName;
            }
            set
            {
                m_strROIName = value;
                if (m_strROIName == "Mark ROI")
                    m_Color[1] = Color.Aqua;
            }
        }
        public EROIBW8 ref_ROI { get { return m_ROI; } set { m_ROI = value; } }
        public int ref_ROIWidth_TopParent { get { return m_ROI.TopParent.Width; } }
        public int ref_ROIHeight_TopParent { get { return m_ROI.TopParent.Height; } }

        #endregion



        public ROI(string strText, int intType)
        {
            m_intType = intType;
            m_strROIName = strText;
            m_ROI = new EROIBW8();
            m_ROI.SetPlacement(0, 0, 100, 100);
            if (m_strROIName == "Mark ROI")
                m_Color[1] = Color.Aqua;
        }

        public ROI(string strText)
        {
            m_intType = 4;
            m_strROIName = strText;
            m_ROI = new EROIBW8();
            m_ROI.SetPlacement(0, 0, 100, 100);
            if (m_strROIName == "Mark ROI")
                m_Color[1] = Color.Aqua;
        }

        public ROI()
        {
            m_ROI = new EROIBW8();
            m_ROI.SetPlacement(0, 0, 100, 100);
        }


        public static void LoadFile(string strPath, List<ROI> arrROIList)
        {
            if (arrROIList == null)
                return;

            arrROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection("Unit0");
            int intChildCount = objFile.GetSecondSectionCount();
            for (int j = 0; j < intChildCount; j++)
            {
                ROI objROI = new ROI();
                objFile.GetSecondSection("ROI" + j);
                objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);
            
                if (objROI.ref_intType > 1)
                {
                    objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                    objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                }

                arrROIList.Add(objROI);
            }
        }

        public static void LoadFile(string strPath, string strSectionName, List<ROI> arrROIList)
        {
            if (arrROIList == null)
                return;

            arrROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);
            int intChildCount = objFile.GetSecondSectionCount();
            for (int j = 0; j < intChildCount; j++)
            {
                ROI objROI = new ROI();
                objFile.GetSecondSection("ROI" + j);
                objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);

                if (objROI.ref_intType > 1)
                {
                    objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                    objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                }

                arrROIList.Add(objROI);
            }
        }

        public static void LoadFile_FixROICount(string strPath, List<ROI> arrROIList)
        {
            if (arrROIList == null)
                return;

            //arrROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection("Unit0");
            int intChildCount = objFile.GetSecondSectionCount();
            for (int j = 0; j < intChildCount; j++)
            {
                if (j < arrROIList.Count)
                {
                    objFile.GetSecondSection("ROI" + j);
                    arrROIList[j].ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                    arrROIList[j].ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                    arrROIList[j].ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                    arrROIList[j].ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                    arrROIList[j].ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                    arrROIList[j].ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                    arrROIList[j].SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                    arrROIList[j].ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                    arrROIList[j].ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);

                    if (arrROIList[j].ref_intType > 1)
                    {
                        arrROIList[j].ref_ROIOriPositionX = arrROIList[j].ref_ROIPositionX;
                        arrROIList[j].ref_ROIOriPositionY = arrROIList[j].ref_ROIPositionY;
                    }
                }
            }
        }

        public static void LoadFile(string strPath, List<List<ROI>> arrROIList)
        {
            if (arrROIList == null)
            {
                SRMMessageBox.Show("ROI.cs->LoadFile()->arrROIList is null.");
                return;
            }

            for (int i = 0; i < arrROIList.Count; i++)
            {
                for (int j = 0; j < arrROIList[i].Count; j++)
                {
                    arrROIList[i][j].Dispose();
                }
            }
            arrROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount = 0;
            ROI objROI;

            int intCount = objFile.GetFirstSectionCount();
            for (int i = 0; i < intCount; i++)
            {
                arrROIList.Add(new List<ROI>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    objROI = new ROI();
                    objFile.GetSecondSection("ROI" + j);
                    objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 2);
                    objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 2);
                    objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 2);
                    objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 2);
                    objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 2);
                    objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 2);
                    objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 2));
                    objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 2);
                    objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 2);

                    //if (objROI.ref_intType > 1)
                    //{
                    objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                    objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                    //}

                    arrROIList[i].Add(objROI);
                }
            }
        }

        public static void LoadFile(string strPath, List<List<List<ROI>>> arrROIList)
        {
            if (arrROIList == null)
            {
                SRMMessageBox.Show("ROI.cs->LoadFile()->arrROIList is null.");
                return;
            }

            for (int i = 0; i < arrROIList.Count; i++)
            {
                for (int j = 0; j < arrROIList[i].Count; j++)
                {
                    for (int k = 0; k < arrROIList[i][j].Count; k++)
                    {
                        arrROIList[i][j][k].Dispose();
                    }
                }
            }
            arrROIList.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount = 0;
            int intSecondChildCount = 0;
            ROI objROI;

            int intCount = objFile.GetFirstSectionCount();
            for (int i = 0; i < intCount; i++)
            {
                arrROIList.Add(new List<List<ROI>>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    arrROIList[i].Add(new List<ROI>());
                    objFile.GetSecondSection("BrightOrDark" + j);
                    intSecondChildCount = objFile.GetThirdSectionCount();
                    for (int k = 0; k < intSecondChildCount; k++)
                    {
                        objROI = new ROI();
                        objFile.GetThirdSection("ROI" + k);
                        objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 3);
                        objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 3);
                        objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 3);
                        objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 3);
                        objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 3);
                        objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 3);
                        objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 3));
                        objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 3);
                        objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 3);

                        //if (objROI.ref_intType > 1)
                        //{
                        objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                        objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                        //}

                        arrROIList[i][j].Add(objROI);
                    }
                }
            }
        }

        public static void SaveFile(string strFilePath, List<ROI> arrROIList)
        {
            XmlParser objFile = new XmlParser(strFilePath);

            objFile.WriteSectionElement("Unit0", true);
            for (int i = 0; i < arrROIList.Count; i++)
            {
                objFile.WriteElement1Value("ROI" + i, "");

                objFile.WriteElement2Value("Name", arrROIList[i].ref_strROIName);
                objFile.WriteElement2Value("Type", arrROIList[i].ref_intType);
                objFile.WriteElement2Value("PositionX", arrROIList[i].ref_ROIPositionX);
                objFile.WriteElement2Value("PositionY", arrROIList[i].ref_ROIPositionY);
                objFile.WriteElement2Value("Width", arrROIList[i].ref_ROIWidth);
                objFile.WriteElement2Value("Height", arrROIList[i].ref_ROIHeight);
                objFile.WriteElement2Value("StartOffsetX", arrROIList[i].ref_intStartOffsetX);
                objFile.WriteElement2Value("StartOffsetY", arrROIList[i].ref_intStartOffsetY);

                if (arrROIList[i].ref_intType == 1)
                {
                    float fPixelAverage = arrROIList[i].GetROIAreaPixel();
                    objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                    arrROIList[i].SetROIPixelAverage(fPixelAverage);
                }
            }

            objFile.WriteEndElement();
        }

        public static void SaveFile(string strFilePath, string strSectionName, List<ROI> arrROIList)
        {
            XmlParser objFile = new XmlParser(strFilePath);

            objFile.WriteSectionElement(strSectionName, true);
            for (int i = 0; i < arrROIList.Count; i++)
            {
                objFile.WriteElement1Value("ROI" + i, "");

                objFile.WriteElement2Value("Name", arrROIList[i].ref_strROIName);
                objFile.WriteElement2Value("Type", arrROIList[i].ref_intType);
                objFile.WriteElement2Value("PositionX", arrROIList[i].ref_ROIPositionX);
                objFile.WriteElement2Value("PositionY", arrROIList[i].ref_ROIPositionY);
                objFile.WriteElement2Value("Width", arrROIList[i].ref_ROIWidth);
                objFile.WriteElement2Value("Height", arrROIList[i].ref_ROIHeight);
                objFile.WriteElement2Value("StartOffsetX", arrROIList[i].ref_intStartOffsetX);
                objFile.WriteElement2Value("StartOffsetY", arrROIList[i].ref_intStartOffsetY);

                if (arrROIList[i].ref_intType == 1)
                {
                    float fPixelAverage = arrROIList[i].GetROIAreaPixel();
                    objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                    arrROIList[i].SetROIPixelAverage(fPixelAverage);
                }
            }

            objFile.WriteEndElement();
        }

        public static void SaveFile(string strFilePath, List<List<ROI>> arrROIList)
        {
            XmlParser objFile = new XmlParser(strFilePath);

            for (int i = 0; i < arrROIList.Count; i++)
            {
                objFile.WriteSectionElement("Unit" + i, true);

                for (int j = 0; j < arrROIList[i].Count; j++)
                {
                    objFile.WriteElement1Value("ROI" + j, "");

                    objFile.WriteElement2Value("Name", arrROIList[i][j].ref_strROIName);
                    objFile.WriteElement2Value("Type", arrROIList[i][j].ref_intType);
                    objFile.WriteElement2Value("PositionX", arrROIList[i][j].ref_ROIPositionX);
                    objFile.WriteElement2Value("PositionY", arrROIList[i][j].ref_ROIPositionY);
                    objFile.WriteElement2Value("Width", arrROIList[i][j].ref_ROIWidth);
                    objFile.WriteElement2Value("Height", arrROIList[i][j].ref_ROIHeight);
                    objFile.WriteElement2Value("StartOffsetX", arrROIList[i][j].ref_intStartOffsetX);
                    objFile.WriteElement2Value("StartOffsetY", arrROIList[i][j].ref_intStartOffsetY);

                    if (arrROIList[i][j].ref_intType == 1)
                    {
                        float fPixelAverage = arrROIList[i][j].GetROIAreaPixel();
                        objFile.WriteElement2Value("AreaPixel", fPixelAverage);
                        arrROIList[i][j].SetROIPixelAverage(fPixelAverage);
                    }

                    arrROIList[i][j].ref_ROIOriPositionX = arrROIList[i][j].ref_ROIPositionX;
                    arrROIList[i][j].ref_ROIOriPositionY = arrROIList[i][j].ref_ROIPositionY;
                }
            }

            objFile.WriteEndElement();
        }

        public static void SaveFile(string strFilePath, List<List<List<ROI>>> arrROIList)
        {
            XmlParser objFile = new XmlParser(strFilePath);

            for (int i = 0; i < arrROIList.Count; i++)
            {
                objFile.WriteSectionElement("Unit" + i, true);

                for (int j = 0; j < arrROIList[i].Count; j++)
                {
                    objFile.WriteElement1Value("BrightOrDark" + j, "");
                    for (int k = 0; k < arrROIList[i][j].Count; k++)
                    {
                        objFile.WriteElement2Value("ROI" + k, "");

                        objFile.WriteElement3Value("Name", arrROIList[i][j][k].ref_strROIName);
                        objFile.WriteElement3Value("Type", arrROIList[i][j][k].ref_intType);
                        objFile.WriteElement3Value("PositionX", arrROIList[i][j][k].ref_ROIPositionX);
                        objFile.WriteElement3Value("PositionY", arrROIList[i][j][k].ref_ROIPositionY);
                        objFile.WriteElement3Value("Width", arrROIList[i][j][k].ref_ROIWidth);
                        objFile.WriteElement3Value("Height", arrROIList[i][j][k].ref_ROIHeight);
                        objFile.WriteElement3Value("StartOffsetX", arrROIList[i][j][k].ref_intStartOffsetX);
                        objFile.WriteElement3Value("StartOffsetY", arrROIList[i][j][k].ref_intStartOffsetY);

                        if (arrROIList[i][j][k].ref_intType == 1)
                        {
                            float fPixelAverage = arrROIList[i][j][k].GetROIAreaPixel();
                            objFile.WriteElement3Value("AreaPixel", fPixelAverage);
                            arrROIList[i][j][k].SetROIPixelAverage(fPixelAverage);
                        }

                        arrROIList[i][j][k].ref_ROIOriPositionX = arrROIList[i][j][k].ref_ROIPositionX;
                        arrROIList[i][j][k].ref_ROIOriPositionY = arrROIList[i][j][k].ref_ROIPositionY;
                    }
                }
            }

            objFile.WriteEndElement();
        }

        /// <summary>
        /// Attach child ROI to specific ROI
        /// </summary>
        /// <param name="objROI">child ROI</param>
        public void AttachImage(ROI objROI)
        {
            //objROI.ref_ROI.Parent.Save("D:\\TS\\Parents.bmp");
            //objROI.ref_ROI.Save("D:\\TS\\PadROI.bmp");
            try
            {
                m_ROI.Detach();

                if (m_ROI.OrgX > objROI.ref_ROI.Width)
                    m_ROI.OrgX = 0;
                if (m_ROI.OrgY > objROI.ref_ROI.Height)
                    m_ROI.OrgY = 0;

                if ((m_ROI.OrgX + m_ROI.Width) > objROI.ref_ROI.Width)
                    m_ROI.Width = objROI.ref_ROI.Width - m_ROI.OrgX;

                if ((m_ROI.OrgY + m_ROI.Height) > objROI.ref_ROI.Height)
                    m_ROI.Height = objROI.ref_ROI.Height - m_ROI.OrgY;

                m_ROI.Attach(objROI.ref_ROI);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Attach image to specific ROI
        /// </summary>
        /// <param name="objImageDrawing">image</param>
        public void AttachImage(ImageDrawing objImageDrawing)
        {
            try
            {
                m_ROI.Detach();
                if (m_ROI.OrgX > objImageDrawing.ref_objMainImage.Width)
                    m_ROI.OrgX = 0;
                if (m_ROI.OrgY > objImageDrawing.ref_objMainImage.Height)
                    m_ROI.OrgY = 0;

                if ((m_ROI.OrgX + m_ROI.Width) > objImageDrawing.ref_objMainImage.Width)
                    m_ROI.Width = objImageDrawing.ref_objMainImage.Width - m_ROI.OrgX;

                if ((m_ROI.OrgY + m_ROI.Height) > objImageDrawing.ref_objMainImage.Height)
                    m_ROI.Height = objImageDrawing.ref_objMainImage.Height - m_ROI.OrgY;

                m_ROI.Attach(objImageDrawing.ref_objMainImage);
            }
            catch
            {
            }

        }

        public void AttachROITopParrent(ROI objROI)
        {
            try
            {
                if (objROI.ref_ROI.TopParent == null)
                    return;

                m_ROI.Detach();
                if (m_ROI.OrgX > objROI.ref_ROI.TopParent.Width)
                    m_ROI.OrgX = 0;
                if (m_ROI.OrgY > objROI.ref_ROI.TopParent.Height)
                    m_ROI.OrgY = 0;

                if ((m_ROI.OrgX + m_ROI.Width) > objROI.ref_ROI.TopParent.Width)
                    m_ROI.Width = objROI.ref_ROI.TopParent.Width - m_ROI.OrgX;

                if ((m_ROI.OrgY + m_ROI.Height) > objROI.ref_ROI.TopParent.Height)
                    m_ROI.Height = objROI.ref_ROI.TopParent.Height - m_ROI.OrgY;

                m_ROI.Attach(objROI.ref_ROI.TopParent);
            }
            catch
            {
            }

        }

        /// <summary>
        /// Set the ROI Frame event that no more movement or changes is done on this moment
        /// Call this function in Mouse_Up event
        /// </summary>
        public void ClearDragHandle()
        {
            m_Handler = EDragHandle.NoHandle;
        }

        /// <summary>
        /// if use this function, 2 ROIs are using same memory address. 
        /// If one of ROI's size or location is changed, another will be changed as well.
        /// After using this function, no need to attach image to new ROI object
        /// </summary>
        /// <param name="objROI">ROI</param>
        public void CopyTo(ref ROI objROI)
        {
            m_ROI.CopyTo(objROI.ref_ROI);
            objROI.ref_strROIName = m_strROIName;
            objROI.ref_intType = m_intType;
            objROI.SetROIPixelAverage(m_fPixelAverage);
        }

        /// <summary>
        /// if use this function, 2 ROIs are using different memory address
        /// If one of ROI's size/location is changed, another won't be affected
        /// However, if their image source are being changed, both attached image will be changed.
        /// After using this function, need to attach image to new ROI object
        /// </summary>
        /// <param name="objROI">ROI</param>
        public void CopyToNew(ref ROI objROI)
        {
            if (objROI.ref_ROI.TopParent == null)
                return;
            objROI.m_ROI.SetPlacement(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
            EasyImage.Copy(m_ROI, objROI.m_ROI);
            objROI.ref_strROIName = m_strROIName;
            objROI.ref_intType = m_intType;
            objROI.SetROIPixelAverage(m_fPixelAverage);
        }

        public void CopyImage(ref ROI objROI)
        {
            if (objROI.ref_ROI.Width == m_ROI.Width && objROI.m_ROI.Height == m_ROI.Height)
                EasyImage.Copy(m_ROI, objROI.m_ROI);
        }
        public void CopyImage_Bigger(ref ROI objROI)
        {
            if (objROI.ref_ROI.Width == m_ROI.Width && objROI.m_ROI.Height == m_ROI.Height)
            {

                EROIBW8 objSourceROI = new EROIBW8();
                objSourceROI.Detach();
                objSourceROI.Attach(m_ROI.TopParent);
                ROI.SetROIPlacement(ref objSourceROI, m_ROI.OrgX - 10, m_ROI.OrgY - 10, m_ROI.Width + 20, m_ROI.Height + 20);

                EROIBW8 objDestinationROI = new EROIBW8();
                objDestinationROI.Detach();
                objDestinationROI.Attach(objROI.m_ROI.TopParent);
                ROI.SetROIPlacement(ref objDestinationROI, objROI.m_ROI.OrgX - 10, objROI.m_ROI.OrgY - 10, objROI.m_ROI.Width + 20, objROI.m_ROI.Height + 20);

                //EasyImage.Copy(m_ROI, objROI.m_ROI);
                EasyImage.Copy(objSourceROI, objDestinationROI);
                objSourceROI.Dispose();
                objDestinationROI.Dispose();
                 
            }
        }
        /// <summary>
        /// if use this function, 2 ROIs are using different memory address
        /// If one of ROI's size/location is changed, another won't be affected
        /// However, if their image source are being changed, both attached image will be changed.
        /// After using this function, need to attach image to new ROI object
        /// </summary>
        /// <param name="objROI">ROI</param>
        public void CopyToNew(ref CROI objROI)
        {
            objROI.ref_CROI.SetPlacement(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
            objROI.ref_strROIName = m_strROIName;
            objROI.ref_intType = m_intType;
        }

        /// <summary>
        /// if use this function, 2 ROIs are using different memory address 
        /// If one of ROI's size/location is changed, another won't be affected
        /// However, if their image source are being changed, both attached image will be changed.
        /// After using this function, need to attach image to new ROI object
        /// </summary>
        /// <param name="objROI">ROI</param>
        public void CopyToNew(CROI objROI)
        {
            objROI.ref_CROI.SetPlacement(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
            objROI.ref_strROIName = m_strROIName;
            objROI.ref_intType = m_intType;
        }

        public void CopyToImage(ref ImageDrawing objDestinationImage)
        {
            if (objDestinationImage != null)
            {
                objDestinationImage.Dispose();
                objDestinationImage = null;
                objDestinationImage = new ImageDrawing(m_ROI.Width, m_ROI.Height);
            }

            //if ((m_ROI.Width != objDestinationImage.ref_intImageWidth) || (m_ROI.Height != objDestinationImage.ref_intImageHeight))
            //{
            //    objDestinationImage.SetImageSize(m_ROI.Width, m_ROI.Height);
            //}

            EasyImage.Copy(m_ROI, objDestinationImage.ref_objMainImage);
        }

        public void CopyToTopParentImage(ref ImageDrawing objDestinationImage)
        {
            if (objDestinationImage != null)
            {
                objDestinationImage.Dispose();
                objDestinationImage = null;
                objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            }
            else
                objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);

            //if ((m_ROI.Width != objDestinationImage.ref_intImageWidth) || (m_ROI.Height != objDestinationImage.ref_intImageHeight))
            //{
            //    objDestinationImage.SetImageSize(m_ROI.Width, m_ROI.Height);
            //}

            EasyImage.Copy(m_ROI.TopParent, objDestinationImage.ref_objMainImage);
        }

        public void CopyTo_ROIToROISamePosition(ref ImageDrawing objDestinationImage)
        {
            //if (objDestinationImage != null)
            //{
            //    objDestinationImage.Dispose();
            //    objDestinationImage = null;
            //    objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            //}

            EROIBW8 objDestinationROI = new EROIBW8();
            objDestinationROI.Detach();
            objDestinationROI.Attach(objDestinationImage.ref_objMainImage);
            objDestinationROI.SetPlacement(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

            EasyImage.Copy(m_ROI, objDestinationROI);

            objDestinationROI.Dispose();
        }

        public void GainTo_ROIToROISamePosition(ref ImageDrawing objDestinationImage, float fGainValue)
        {
            //if (objDestinationImage != null)
            //{
            //    objDestinationImage.Dispose();
            //    objDestinationImage = null;
            //    objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            //}

            if (m_ROI.OrgX < 0 || m_ROI.OrgY < 0 || m_ROI.Width <= 0 || m_ROI.Height <= 0)
                return;

            EROIBW8 objDestinationROI = new EROIBW8();
            objDestinationROI.Detach();
            objDestinationROI.Attach(objDestinationImage.ref_objMainImage);
            objDestinationROI.SetPlacement(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

            if (fGainValue == 1)
                EasyImage.Copy(m_ROI, objDestinationROI);
            else
                EasyImage.GainOffset(m_ROI, objDestinationROI, fGainValue);

            objDestinationROI.Dispose();
        }

        public void GainTo_ROIToROISamePosition_Bigger(ref ImageDrawing objDestinationImage, float fGainValue)
        {
            //if (objDestinationImage != null)
            //{
            //    objDestinationImage.Dispose();
            //    objDestinationImage = null;
            //    objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            //}

            EROIBW8 objSourceROI = new EROIBW8();
            objSourceROI.Detach();
            objSourceROI.Attach(m_ROI.TopParent);
            ROI.SetROIPlacement(ref objSourceROI, m_ROI.OrgX - 10, m_ROI.OrgY - 10, m_ROI.Width + 20, m_ROI.Height + 20);

            EROIBW8 objDestinationROI = new EROIBW8();
            objDestinationROI.Detach();
            objDestinationROI.Attach(objDestinationImage.ref_objMainImage);
            ROI.SetROIPlacement(ref objDestinationROI, m_ROI.OrgX - 10, m_ROI.OrgY - 10, m_ROI.Width + 20, m_ROI.Height + 20);

            if (fGainValue == 1)
                EasyImage.Copy(objSourceROI, objDestinationROI);
            else
                EasyImage.GainOffset(objSourceROI, objDestinationROI, fGainValue);

            objSourceROI.Dispose();
            objDestinationROI.Dispose();
        }

        public void OpenCloseTo_ROIToROISamePosition(ref ImageDrawing objDestinationImage, int intOpenCloseValue)
        {
            //if (objDestinationImage != null)
            //{
            //    objDestinationImage.Dispose();
            //    objDestinationImage = null;
            //    objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            //}

            EROIBW8 objDestinationROI = new EROIBW8();
            objDestinationROI.Detach();
            objDestinationROI.Attach(objDestinationImage.ref_objMainImage);
            objDestinationROI.SetPlacement(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
#if (Debug_2_12 || Release_2_12)
            if (intOpenCloseValue > 0)
            EasyImage.OpenDisk(m_ROI, objDestinationROI, (uint)intOpenCloseValue);
            else if(intOpenCloseValue < 0)
            EasyImage.CloseDisk(m_ROI, objDestinationROI, (uint)Math.Abs(intOpenCloseValue));

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (intOpenCloseValue > 0)
                EasyImage.OpenDisk(m_ROI, objDestinationROI, intOpenCloseValue);
            else if (intOpenCloseValue < 0)
                EasyImage.CloseDisk(m_ROI, objDestinationROI, Math.Abs(intOpenCloseValue));

#endif

            objDestinationROI.Dispose();
        }

        public void OpenCloseTo_ROIToROISamePosition_Bigger(ref ImageDrawing objDestinationImage, int intOpenCloseValue)
        {
            //if (objDestinationImage != null)
            //{
            //    objDestinationImage.Dispose();
            //    objDestinationImage = null;
            //    objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            //}

            EROIBW8 objSourceROI = new EROIBW8();
            objSourceROI.Detach();
            objSourceROI.Attach(m_ROI.TopParent);
            ROI.SetROIPlacement(ref objSourceROI, m_ROI.OrgX - 10, m_ROI.OrgY - 10, m_ROI.Width + 20, m_ROI.Height + 20);

            EROIBW8 objDestinationROI = new EROIBW8();
            objDestinationROI.Detach();
            objDestinationROI.Attach(objDestinationImage.ref_objMainImage);
            ROI.SetROIPlacement(ref objDestinationROI, m_ROI.OrgX - 10, m_ROI.OrgY - 10, m_ROI.Width + 20, m_ROI.Height + 20);
#if (Debug_2_12 || Release_2_12)
            if (intOpenCloseValue > 0)
                EasyImage.OpenDisk(objSourceROI, objDestinationROI, (uint)intOpenCloseValue);
            else if (intOpenCloseValue < 0)
                EasyImage.CloseDisk(objSourceROI, objDestinationROI, (uint)Math.Abs(intOpenCloseValue));

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (intOpenCloseValue > 0)
                EasyImage.OpenDisk(objSourceROI, objDestinationROI, intOpenCloseValue);
            else if (intOpenCloseValue < 0)
                EasyImage.CloseDisk(objSourceROI, objDestinationROI, Math.Abs(intOpenCloseValue));

#endif

            objSourceROI.Dispose();
            objDestinationROI.Dispose();
        }

        public void ThresholdTo_ROIToImage(ref ImageDrawing objDestinationImage, int intThresholdValue)
        {
            //if (objDestinationImage != null)
            //{
            //    objDestinationImage.Dispose();
            //    objDestinationImage = null;
            //    objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            //}


            if (m_ROI.Width != objDestinationImage.ref_intImageWidth || m_ROI.Height != objDestinationImage.ref_intImageHeight)
            {
                objDestinationImage.SetImageSize(m_ROI.Width, m_ROI.Height);
            }

#if (Debug_2_12 || Release_2_12)
            EasyImage.Threshold(m_ROI, objDestinationImage.ref_objMainImage, (uint)intThresholdValue);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            EasyImage.Threshold(m_ROI, objDestinationImage.ref_objMainImage, intThresholdValue);
#endif

        }

        public void ThresholdTo_ROIToROISamePosition(ref ImageDrawing objDestinationImage, int intThresholdValue)
        {
            //if (objDestinationImage != null)
            //{
            //    objDestinationImage.Dispose();
            //    objDestinationImage = null;
            //    objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            //}
            if (m_ROI.Width < 0 || m_ROI.Height < 0)
                return;

            EROIBW8 objDestinationROI = new EROIBW8();
            objDestinationROI.Detach();
            objDestinationROI.Attach(objDestinationImage.ref_objMainImage);
            objDestinationROI.SetPlacement(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
#if (Debug_2_12 || Release_2_12)
          EasyImage.Threshold(m_ROI, objDestinationROI, (uint)intThresholdValue);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            EasyImage.Threshold(m_ROI, objDestinationROI, intThresholdValue);
#endif

            objDestinationROI.Dispose();
        }

        public void ThresholdTo_ROIToROISamePosition_Bigger(ref ImageDrawing objDestinationImage, int intThresholdValue)
        {
            //if (objDestinationImage != null)
            //{
            //    objDestinationImage.Dispose();
            //    objDestinationImage = null;
            //    objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            //}
            //m_ROI.TopParent.Save("D:\\TS\\01.TopParent.bmp");
            //m_ROI.Save("D:\\TS\\02.m_ROI.bmp");
            //objDestinationImage.SaveImage("D:\\TS\\03.objDestinationImage.bmp");


            EROIBW8 objSourceROI = new EROIBW8();
            objSourceROI.Detach();
            objSourceROI.Attach(m_ROI.TopParent);
            ROI.SetROIPlacement(ref objSourceROI, m_ROI.OrgX - 10, m_ROI.OrgY - 10, m_ROI.Width + 20, m_ROI.Height + 20);

            EROIBW8 objDestinationROI = new EROIBW8();
            objDestinationROI.Detach();
            objDestinationROI.Attach(objDestinationImage.ref_objMainImage);
            ROI.SetROIPlacement(ref objDestinationROI, m_ROI.OrgX - 10, m_ROI.OrgY - 10, m_ROI.Width + 20, m_ROI.Height + 20);
#if (Debug_2_12 || Release_2_12)
          EasyImage.Threshold(objSourceROI, objDestinationROI, (uint)intThresholdValue);
            
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            EasyImage.Threshold(objSourceROI, objDestinationROI, intThresholdValue);

#endif

            objSourceROI.Dispose();
            objDestinationROI.Dispose();
        }
        public void HighPassTo_ROIToImage(ref ImageDrawing objDestinationImage)
        {
            //if (objDestinationImage != null)
            //{
            //    objDestinationImage.Dispose();
            //    objDestinationImage = null;
            //    objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            //}


            if (m_ROI.Width != objDestinationImage.ref_intImageWidth || m_ROI.Height != objDestinationImage.ref_intImageHeight)
            {
                objDestinationImage.SetImageSize(m_ROI.Width, m_ROI.Height);
            }

            EasyImage.ConvolHighpass2(m_ROI, objDestinationImage.ref_objMainImage);
        }

        public void HighPassTo_ROIToROISamePosition(ref ImageDrawing objDestinationImage)
        {
            //if (objDestinationImage != null)
            //{
            //    objDestinationImage.Dispose();
            //    objDestinationImage = null;
            //    objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            //}
            if (m_ROI.Width < 0 || m_ROI.Height < 0)
                return;

            EROIBW8 objSourceROI = new EROIBW8();
            objSourceROI.Detach();
            objSourceROI.Attach(m_ROI.TopParent);
            ROI.SetROIPlacement(ref objSourceROI, m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

            EROIBW8 objDestinationROI = new EROIBW8();
            objDestinationROI.Detach();
            objDestinationROI.Attach(objDestinationImage.ref_objMainImage);
            ROI.SetROIPlacement(ref objDestinationROI, m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

            //EasyImage.Copy(m_ROI, objROI.m_ROI);
            EasyImage.Copy(objSourceROI, objDestinationROI);

            m_ROI.Detach();
            m_ROI.Attach(objDestinationImage.ref_objMainImage);
            EasyImage.ConvolHighpass2(m_ROI);

            objSourceROI.Dispose();
            objDestinationROI.Dispose();
        }

        public void HighPassTo_ROIToROISamePosition_Bigger(ref ImageDrawing objDestinationImage)
        {
            //if (objDestinationImage != null)
            //{
            //    objDestinationImage.Dispose();
            //    objDestinationImage = null;
            //    objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            //}
            //m_ROI.TopParent.Save("D:\\TS\\01.TopParent.bmp");
            //m_ROI.Save("D:\\TS\\02.m_ROI.bmp");
            //objDestinationImage.SaveImage("D:\\TS\\03.objDestinationImage.bmp");


            EROIBW8 objSourceROI = new EROIBW8();
            objSourceROI.Detach();
            objSourceROI.Attach(m_ROI.TopParent);
            ROI.SetROIPlacement(ref objSourceROI, m_ROI.OrgX - 10, m_ROI.OrgY - 10, m_ROI.Width + 20, m_ROI.Height + 20);

            EROIBW8 objDestinationROI = new EROIBW8();
            objDestinationROI.Detach();
            objDestinationROI.Attach(objDestinationImage.ref_objMainImage);
            ROI.SetROIPlacement(ref objDestinationROI, m_ROI.OrgX - 10, m_ROI.OrgY - 10, m_ROI.Width + 20, m_ROI.Height + 20);

            //EasyImage.Copy(m_ROI, objROI.m_ROI);
            EasyImage.Copy(objSourceROI, objDestinationROI);

            m_ROI.Detach();
            m_ROI.Attach(objDestinationImage.ref_objMainImage);
            EasyImage.ConvolHighpass2(m_ROI);

            objSourceROI.Dispose();
            objDestinationROI.Dispose();
        }
        public void PrewittTo_ROIToROISamePosition(ref ImageDrawing objDestinationImage)
        {
            //if (objDestinationImage != null)
            //{
            //    objDestinationImage.Dispose();
            //    objDestinationImage = null;
            //    objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            //}

            //2020-11-12 ZJYEOH : if Prewitt from one image to another will create noise outside the ROI
            //EROIBW8 objDestinationROI = new EROIBW8();
            //objDestinationROI.Detach();
            //objDestinationROI.Attach(objDestinationImage.ref_objMainImage);
            //objDestinationROI.SetPlacement(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

            EROIBW8 objSourceROI = new EROIBW8();
            objSourceROI.Detach();
            objSourceROI.Attach(m_ROI.TopParent);
            ROI.SetROIPlacement(ref objSourceROI, m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

            EROIBW8 objDestinationROI = new EROIBW8();
            objDestinationROI.Detach();
            objDestinationROI.Attach(objDestinationImage.ref_objMainImage);
            ROI.SetROIPlacement(ref objDestinationROI, m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

            //EasyImage.Copy(m_ROI, objROI.m_ROI);
            EasyImage.Copy(objSourceROI, objDestinationROI);

            m_ROI.Detach();
            m_ROI.Attach(objDestinationImage.ref_objMainImage);
            EasyImage.ConvolPrewitt(m_ROI);//, objDestinationROI);

            objSourceROI.Dispose();
            objDestinationROI.Dispose();
            //objDestinationROI.Dispose();
        }
        public void PrewittOperation(ref ImageDrawing objDestinationImage)
        {
            if (objDestinationImage != null)
            {
                objDestinationImage.Dispose();
                objDestinationImage = null;
                objDestinationImage = new ImageDrawing(true, m_ROI.TopParent.Width, m_ROI.TopParent.Height);
            }

            //2020 - 11 - 12 ZJYEOH: if Prewitt from one image to another will create noise outside the ROI
            EROIBW8 objDestinationROI = new EROIBW8();
            objDestinationROI.Detach();
            objDestinationROI.Attach(objDestinationImage.ref_objMainImage);
            objDestinationROI.SetPlacement(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

            EasyImage.ConvolPrewitt(m_ROI, objDestinationROI);

            objDestinationROI.Dispose();
        }
        /// <summary>
        /// Move ROI Frame to new position on graph
        /// </summary>
        /// <param name="nPositionX">the point of mouse move over</param>
        /// <param name="nPositionY">the point of mouse move over</param>
        public void DragROI(int nNewPositionX, int nNewPositionY)
        {
            try
            {
                m_ROI.Drag(m_Handler, nNewPositionX, nNewPositionY);

                bool blnAdjustX = false;
                bool blnAdjustY = false;
                if (ref_ROITotalX + ref_ROIWidth >= m_ROI.TopParent.Width)
                {
                    int intOffet = m_ROI.TopParent.Width - (ref_ROITotalX + ref_ROIWidth) - 1;
                    LoadROISetting(m_ROI.OrgX + intOffet, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

                    blnAdjustX = true;
                }

                if (ref_ROITotalY + ref_ROIHeight >= m_ROI.TopParent.Height)
                {
                    int intOffet = m_ROI.TopParent.Height - (ref_ROITotalY + ref_ROIHeight) - 1;
                    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY + intOffet, m_ROI.Width, m_ROI.Height);

                    blnAdjustY = true;
                }

                if (ref_ROITotalX < 0)
                {
                    if (blnAdjustX)
                        LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width + ref_ROITotalX, m_ROI.Height);
                    else
                        LoadROISetting(0, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
                }

                if (ref_ROITotalY < 0)
                {
                    if (blnAdjustY)
                        LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height + ref_ROITotalX);
                    else
                        LoadROISetting(m_ROI.OrgX, 0, m_ROI.Width, m_ROI.Height);
                }

                if (ref_ROITotalX <= m_ROI.Parent.TotalOrgX)
                {
                    LoadROISetting(0, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
                }

                if (ref_ROITotalY <= m_ROI.Parent.TotalOrgY)
                {
                    LoadROISetting(m_ROI.OrgX, 0, m_ROI.Width, m_ROI.Height);
                }

                if (ref_ROITotalX + ref_ROIWidth >= m_ROI.Parent.Width)
                {
                    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
                }

                if (ref_ROITotalY + ref_ROIHeight >= m_ROI.Parent.Height)
                {
                    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
                }

            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }
        }
        public void DragFixROI(int nNewPositionX, int nNewPositionY, int intTop, int intRight, int intBottom, int intLeft)
        {
            try
            {
                m_ROI.Drag(m_Handler, nNewPositionX, nNewPositionY);

                bool blnAdjustX = false;
                bool blnAdjustY = false;

                if (ref_ROITotalX + ref_ROIWidth >= intRight - 1)
                {
                    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

                    if (ref_ROIWidth >= intRight - intLeft)
                    {
                        LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, intRight - intLeft, m_ROI.Height);
                    }
                }

                if (ref_ROITotalY + ref_ROIHeight >= intBottom - 1)
                {
                    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

                    if (ref_ROIHeight >= intBottom - intTop)
                    {
                        LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, intBottom - intTop);
                    }
                }

                if (ref_ROITotalX <= intLeft + 1)
                {
                    LoadROISetting(intLeft, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
                    
                    if (ref_ROIWidth >= intRight - intLeft)
                    {
                        LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, intRight - intLeft, m_ROI.Height);
                    }

                }

                if (ref_ROITotalY <= intTop + 1)
                {
                    LoadROISetting(m_ROI.OrgX, intTop, m_ROI.Width, m_ROI.Height);

                    if (ref_ROIHeight >= intBottom - intTop)
                    {
                        LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, intBottom - intTop);
                    }
                }

                if (ref_ROIWidth >= intRight - intLeft)
                {
                    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, intRight - intLeft, m_ROI.Height);
                }

                if (ref_ROIHeight >= intBottom - intTop)
                {
                    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, intBottom - intTop);
                }
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }
        }
        public void DragFixROI2(int nNewPositionX, int nNewPositionY)
        {
            try
            {
                m_ROI.Drag(m_Handler, nNewPositionX, nNewPositionY);

                bool blnAdjustX = false;
                bool blnAdjustY = false;
                if (ref_ROITotalX + ref_ROIWidth >= m_ROI.Parent.Width)
                {
                    int intOffet = 0;// m_ROI.TopParent.Width - (ref_ROITotalX + ref_ROIWidth) - 1;
                    LoadROISetting(m_ROI.OrgX + intOffet, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

                    //blnAdjustX = true;
                }

                if (ref_ROITotalY + ref_ROIHeight >= m_ROI.Parent.Height)
                {
                    int intOffet = 0;// m_ROI.TopParent.Height - (ref_ROITotalY + ref_ROIHeight) - 1;
                    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY + intOffet, m_ROI.Width, m_ROI.Height);

                    //blnAdjustY = true;
                }

                if (ref_ROITotalX < m_ROI.Parent.TotalOrgX)
                {
                    //if (blnAdjustX)
                    //    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width + ref_ROITotalX, m_ROI.Height);
                    //else
                        LoadROISetting(0, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
                }

                if (ref_ROITotalY < m_ROI.Parent.TotalOrgY)
                {
                    //if (blnAdjustY)
                    //    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height + ref_ROITotalX);
                    //else
                        LoadROISetting(m_ROI.OrgX, 0, m_ROI.Width, m_ROI.Height);
                }

            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }
        }
        public void DragFixROI3(int nNewPositionX, int nNewPositionY)
        {
            try
            {
                //m_ROI.Drag(m_Handler, nNewPositionX, nNewPositionY);
                DragFixROI(nNewPositionX, nNewPositionY, 1.0f, 0, 0);
                bool blnAdjustX = false;
                bool blnAdjustY = false;
                if (ref_ROITotalX + ref_ROIWidth >= m_ROI.Parent.Width)
                {
                    int intOffet = 0;// m_ROI.TopParent.Width - (ref_ROITotalX + ref_ROIWidth) - 1;
                    LoadROISetting(m_ROI.OrgX + intOffet, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);

                    //blnAdjustX = true;
                }

                if (ref_ROITotalY + ref_ROIHeight >= m_ROI.Parent.Height)
                {
                    int intOffet = 0;// m_ROI.TopParent.Height - (ref_ROITotalY + ref_ROIHeight) - 1;
                    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY + intOffet, m_ROI.Width, m_ROI.Height);

                    //blnAdjustY = true;
                }

                if (ref_ROITotalX < m_ROI.Parent.TotalOrgX)
                {
                    //if (blnAdjustX)
                    //    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width + ref_ROITotalX, m_ROI.Height);
                    //else
                    LoadROISetting(0, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
                }

                if (ref_ROITotalY < m_ROI.Parent.TotalOrgY)
                {
                    //if (blnAdjustY)
                    //    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height + ref_ROITotalX);
                    //else
                    LoadROISetting(m_ROI.OrgX, 0, m_ROI.Width, m_ROI.Height);
                }

            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }
        }
        public void DragROIWithMinWidth(int nNewPositionX, int nNewPositionY, int intMinWidth)
        {
            try
            {
                m_ROI.Drag(m_Handler, nNewPositionX, nNewPositionY);

                if (m_ROI.Width < intMinWidth)
                {
                    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, intMinWidth, m_ROI.Height);
                }
                else
                {
                    // 2020 12 31 - CCENG   : The ROI will out of parents will call the m_ROI.Drag function.
                    //                      : Need to call LoadROISetting function to control the ROI out of parants. 
                    //                      : If the ROI out of parent, Will get error close when build blob.
                    LoadROISetting(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, m_ROI.Height);
                }
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }
        }

        /// <summary>
        /// Allow ROI move without changing the ROI size. The fix size of ROI will follow the last set ROI size
        /// </summary>
        /// <param name="nNewPositionX">the point of mouse move over</param>
        /// <param name="nNewPositionY">the point of mouse move over</param>
        public void DragFixROI(int nNewPositionX, int nNewPositionY)
        {
            DragFixROI(nNewPositionX, nNewPositionY, 1.0f, 0, 0);
        }

        public void DragFixROI(int nNewPositionX, int nNewPositionY, float fScale, float fPanX, float fPanY)
        {
            try
            {
                m_ROI.Drag(m_Handler, nNewPositionX, nNewPositionY, fScale, fScale, fPanX / fScale, fPanY / fScale);
                m_ROI.Width = m_intOriWidth;
                m_ROI.Height = m_intOriHeight;
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("DragROI: " + ex.ToString());
            }
        }

        /// <summary>
        /// Redraw ROI
        /// </summary>
        /// <param name="g">destination to draw image</param>
        public void Draw(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            m_ROI.Draw(g);
        }

        /// <summary>
        /// Draw dont care area edge
        /// </summary>
        /// <param name="intPixel">edge pixel value to draw, 0 = black, 255 = white</param>
        public void DrawDontCareEdge(int intPixel)
        {
            EBW8 px = new EBW8((byte)intPixel);

            int intEndX = m_ROI.Width;
            int intEndY = m_ROI.Height;

            for (int i = 0; i < intEndX; i++)
            {
                for (int j = 0; j < intEndY; j++)
                {
                    if (j == 0 || j == intEndY - 1)
                        m_ROI.SetPixel(px, i, j);
                    else
                    {
                        if (i == 0 || i == intEndX - 1)
                        {
                            m_ROI.SetPixel(px, i, j);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draw a rectangle frame around an image or ROI
        /// </summary>
        /// <param name="g">window destination to put the drawing up</param>
        /// <param name="blnHandler">true = frame can be modified (change shape or move position), false if otherwise</param>     
        /// <param name="strText">give a name to this ROI frame</param>
        /// <param name="intROINumber">to assign a different color to its name and line. This param is unneccessary if single ROI is used on whole image</param>
        /// <param name="nType">1 = SearchROI, 2 = TrainROI, 3 = Dont care area, 4 = SubROI</param>
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, string strText, int intType, int intROINumber)
        {
            try
            {
                if (m_ROI.Parent == null)
                    return;

                if (m_strROIName != strText)
                    m_strROIName = strText;
                if (m_intType != intType)
                    m_intType = intType;

                if (m_strROIName == "ReTest ROI")
                    m_Color[0] = Color.GreenYellow;
                // Frame Position ON = frame is centered on image edge, Inside = outer edge of frame remain inside of image, Outside = inner edge of frame remain outside of image</param>
                //m_ROI.DrawFrame(g, new Pen(m_Color[intROINumber], 2), EFramePosition.Outside, blnHandler, fDrawingScaleX, fDrawingScaleY);
                //m_ROI.DrawFrame(g, new ERGBColor(m_Color[intROINumber].R, m_Color[intROINumber].G, m_Color[intROINumber].B), blnHandler, fDrawingScaleX, fDrawingScaleY);
                if (m_objRGBColor.Red != m_Color[intROINumber].R)
                    m_objRGBColor.Red = m_Color[intROINumber].R;
                if (m_objRGBColor.Green != m_Color[intROINumber].G)
                    m_objRGBColor.Green = m_Color[intROINumber].G;
                if (m_objRGBColor.Blue != m_Color[intROINumber].B)
                    m_objRGBColor.Blue = m_Color[intROINumber].B;

                //if (m_ROI.OrgX < 0)
                //    m_ROI.OrgX = 0;
                //if (m_ROI.OrgY < 0)
                //    m_ROI.OrgY = 0;
                //if (m_ROI.Width < 10)
                //    m_ROI.Width = 10;
                //if (m_ROI.Height < 10)
                //    m_ROI.Height = 10;

                //if (m_ROI.Parent != null)
                //{
                //    if (m_ROI.Width > m_ROI.Parent.Width)
                //        m_ROI.Width = m_ROI.Parent.Width;


                //    if (m_ROI.Height > m_ROI.Parent.Height)
                //        m_ROI.Height = m_ROI.Parent.Height;

                //    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                //        m_ROI.OrgX = m_ROI.Parent.Width - m_ROI.Width;

                //    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                //        m_ROI.OrgY = m_ROI.Parent.Height - m_ROI.Height;
                //}

                if (m_ROI.OrgX < 0)
                {
                    m_ROI.Width += m_ROI.OrgX;
                    m_ROI.OrgX = 0;
                }
                if (m_ROI.OrgY < 0)
                {
                    m_ROI.Height += m_ROI.OrgY;
                    m_ROI.OrgY = 0;
                }
                if (m_ROI.Width < 10)
                    m_ROI.Width = 10;
                if (m_ROI.Height < 10)
                    m_ROI.Height = 10;

                if (m_ROI.Parent != null)
                {
                    if (m_ROI.Width > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width;

                    if (m_ROI.Height > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height;

                    if (m_ROI.OrgX > m_ROI.Parent.Width - 10)
                        m_ROI.OrgX = m_ROI.Parent.Width - 10;

                    if (m_ROI.OrgY > m_ROI.Parent.Height - 10)
                        m_ROI.OrgY = m_ROI.Parent.Height - 10;

                    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width - m_ROI.OrgX;

                    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height - m_ROI.OrgY;

                }

                m_ROI.DrawFrame(g, m_objRGBColor, blnHandler, fDrawingScaleX, fDrawingScaleY);
                DrawString(intROINumber, g, fDrawingScaleX, fDrawingScaleY);
            }
            catch
            {

            }
        }

        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, string strText, int intType, int intROINumber, Color objColor, int nostring, int dontwantstring)
        {
            try
            {
                if (m_ROI.Parent == null)
                    return;

                if (m_strROIName != strText)
                    m_strROIName = strText;
                if (m_intType != intType)
                    m_intType = intType;

                //if (m_strROIName == "ReTest ROI")
                //    m_Color[0] = Color.GreenYellow;
                // Frame Position ON = frame is centered on image edge, Inside = outer edge of frame remain inside of image, Outside = inner edge of frame remain outside of image</param>
                //m_ROI.DrawFrame(g, new Pen(m_Color[intROINumber], 2), EFramePosition.Outside, blnHandler, fDrawingScaleX, fDrawingScaleY);
                //m_ROI.DrawFrame(g, new ERGBColor(m_Color[intROINumber].R, m_Color[intROINumber].G, m_Color[intROINumber].B), blnHandler, fDrawingScaleX, fDrawingScaleY);
                if (m_objRGBColor.Red != objColor.R)
                    m_objRGBColor.Red = objColor.R;
                if (m_objRGBColor.Green != objColor.G)
                    m_objRGBColor.Green = objColor.G;
                if (m_objRGBColor.Blue != objColor.B)
                    m_objRGBColor.Blue = objColor.B;

                //if (m_ROI.OrgX < 0)
                //    m_ROI.OrgX = 0;
                //if (m_ROI.OrgY < 0)
                //    m_ROI.OrgY = 0;
                //if (m_ROI.Width < 10)
                //    m_ROI.Width = 10;
                //if (m_ROI.Height < 10)
                //    m_ROI.Height = 10;

                //if (m_ROI.Parent != null)
                //{
                //    if (m_ROI.Width > m_ROI.Parent.Width)
                //        m_ROI.Width = m_ROI.Parent.Width;


                //    if (m_ROI.Height > m_ROI.Parent.Height)
                //        m_ROI.Height = m_ROI.Parent.Height;

                //    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                //        m_ROI.OrgX = m_ROI.Parent.Width - m_ROI.Width;

                //    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                //        m_ROI.OrgY = m_ROI.Parent.Height - m_ROI.Height;
                //}

                if (m_ROI.OrgX < 0)
                {
                    m_ROI.Width += m_ROI.OrgX;
                    m_ROI.OrgX = 0;
                }
                if (m_ROI.OrgY < 0)
                {
                    m_ROI.Height += m_ROI.OrgY;
                    m_ROI.OrgY = 0;
                }
                if (m_ROI.Width < 10)
                    m_ROI.Width = 10;
                if (m_ROI.Height < 10)
                    m_ROI.Height = 10;

                if (m_ROI.Parent != null)
                {
                    if (m_ROI.Width > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width;

                    if (m_ROI.Height > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height;

                    if (m_ROI.OrgX > m_ROI.Parent.Width - 10)
                        m_ROI.OrgX = m_ROI.Parent.Width - 10;

                    if (m_ROI.OrgY > m_ROI.Parent.Height - 10)
                        m_ROI.OrgY = m_ROI.Parent.Height - 10;

                    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width - m_ROI.OrgX;

                    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height - m_ROI.OrgY;

                }

                m_ROI.DrawFrame(g, m_objRGBColor, blnHandler, fDrawingScaleX, fDrawingScaleY);
                if (nostring != 0 && dontwantstring != 0)
                    DrawString(intROINumber, g, fDrawingScaleX, fDrawingScaleY, objColor);
            }
            catch
            {

            }
        }

        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, string strText, int intType, int intROINumber, Color objColor)
        {
            try
            {
                if (m_ROI.Parent == null)
                    return;

                if (m_strROIName != strText)
                    m_strROIName = strText;
                if (m_intType != intType)
                    m_intType = intType;

                //if (m_strROIName == "ReTest ROI")
                //    m_Color[0] = Color.GreenYellow;
                // Frame Position ON = frame is centered on image edge, Inside = outer edge of frame remain inside of image, Outside = inner edge of frame remain outside of image</param>
                //m_ROI.DrawFrame(g, new Pen(m_Color[intROINumber], 2), EFramePosition.Outside, blnHandler, fDrawingScaleX, fDrawingScaleY);
                //m_ROI.DrawFrame(g, new ERGBColor(m_Color[intROINumber].R, m_Color[intROINumber].G, m_Color[intROINumber].B), blnHandler, fDrawingScaleX, fDrawingScaleY);
                if (m_objRGBColor.Red != objColor.R)
                    m_objRGBColor.Red = objColor.R;
                if (m_objRGBColor.Green != objColor.G)
                    m_objRGBColor.Green = objColor.G;
                if (m_objRGBColor.Blue != objColor.B)
                    m_objRGBColor.Blue = objColor.B;

                //if (m_ROI.OrgX < 0)
                //    m_ROI.OrgX = 0;
                //if (m_ROI.OrgY < 0)
                //    m_ROI.OrgY = 0;
                //if (m_ROI.Width < 10)
                //    m_ROI.Width = 10;
                //if (m_ROI.Height < 10)
                //    m_ROI.Height = 10;

                //if (m_ROI.Parent != null)
                //{
                //    if (m_ROI.Width > m_ROI.Parent.Width)
                //        m_ROI.Width = m_ROI.Parent.Width;


                //    if (m_ROI.Height > m_ROI.Parent.Height)
                //        m_ROI.Height = m_ROI.Parent.Height;

                //    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                //        m_ROI.OrgX = m_ROI.Parent.Width - m_ROI.Width;

                //    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                //        m_ROI.OrgY = m_ROI.Parent.Height - m_ROI.Height;
                //}

                if (m_ROI.OrgX < 0)
                {
                    m_ROI.Width += m_ROI.OrgX;
                    m_ROI.OrgX = 0;
                }
                if (m_ROI.OrgY < 0)
                {
                    m_ROI.Height += m_ROI.OrgY;
                    m_ROI.OrgY = 0;
                }
                if (m_ROI.Width < 10)
                    m_ROI.Width = 10;
                if (m_ROI.Height < 10)
                    m_ROI.Height = 10;

                if (m_ROI.Parent != null)
                {
                    if (m_ROI.Width > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width;

                    if (m_ROI.Height > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height;

                    if (m_ROI.OrgX > m_ROI.Parent.Width - 10)
                        m_ROI.OrgX = m_ROI.Parent.Width - 10;

                    if (m_ROI.OrgY > m_ROI.Parent.Height - 10)
                        m_ROI.OrgY = m_ROI.Parent.Height - 10;

                    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width - m_ROI.OrgX;

                    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height - m_ROI.OrgY;

                }

                m_ROI.DrawFrame(g, m_objRGBColor, blnHandler, fDrawingScaleX, fDrawingScaleY);
                DrawString(intROINumber, g, fDrawingScaleX, fDrawingScaleY, objColor);
            }
            catch
            {

            }
        }
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, string strText, int intType, int intROINumber, float fPositionX, float fPositionY, Color objColor)
        {
            try
            {
                if (m_ROI.Parent == null)
                    return;

                if (m_strROIName != strText)
                    m_strROIName = strText;
                if (m_intType != intType)
                    m_intType = intType;

                if (m_strROIName == "ReTest ROI")
                    m_Color[0] = Color.GreenYellow;

                // Frame Position ON = frame is centered on image edge, Inside = outer edge of frame remain inside of image, Outside = inner edge of frame remain outside of image</param>
                //m_ROI.DrawFrame(g, new Pen(m_Color[intROINumber], 2), EFramePosition.Outside, blnHandler, fDrawingScaleX, fDrawingScaleY);
                //m_ROI.DrawFrame(g, new ERGBColor(m_Color[intROINumber].R, m_Color[intROINumber].G, m_Color[intROINumber].B), blnHandler, fDrawingScaleX, fDrawingScaleY);
                if (m_objRGBColor.Red != objColor.R)
                    m_objRGBColor.Red = objColor.R;
                if (m_objRGBColor.Green != objColor.G)
                    m_objRGBColor.Green = objColor.G;
                if (m_objRGBColor.Blue != objColor.B)
                    m_objRGBColor.Blue = objColor.B;

                //if (m_ROI.OrgX < 0)
                //    m_ROI.OrgX = 0;
                //if (m_ROI.OrgY < 0)
                //    m_ROI.OrgY = 0;
                //if (m_ROI.Width < 10)
                //    m_ROI.Width = 10;
                //if (m_ROI.Height < 10)
                //    m_ROI.Height = 10;

                //if (m_ROI.Parent != null)
                //{
                //    if (m_ROI.Width > m_ROI.Parent.Width)
                //        m_ROI.Width = m_ROI.Parent.Width;


                //    if (m_ROI.Height > m_ROI.Parent.Height)
                //        m_ROI.Height = m_ROI.Parent.Height;

                //    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                //        m_ROI.OrgX = m_ROI.Parent.Width - m_ROI.Width;

                //    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                //       m_ROI.OrgY = m_ROI.Parent.Height - m_ROI.Height;
                //}
                if (m_ROI.OrgX < 0)
                {
                    m_ROI.Width += m_ROI.OrgX;
                    m_ROI.OrgX = 0;
                }
                if (m_ROI.OrgY < 0)
                {
                    m_ROI.Height += m_ROI.OrgY;
                    m_ROI.OrgY = 0;
                }
                if (m_ROI.Width < 10)
                    m_ROI.Width = 10;
                if (m_ROI.Height < 10)
                    m_ROI.Height = 10;

                if (m_ROI.Parent != null)
                {
                    if (m_ROI.Width > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width;

                    if (m_ROI.Height > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height;

                    if (m_ROI.OrgX > m_ROI.Parent.Width - 10)
                        m_ROI.OrgX = m_ROI.Parent.Width - 10;

                    if (m_ROI.OrgY > m_ROI.Parent.Height - 10)
                        m_ROI.OrgY = m_ROI.Parent.Height - 10;

                    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width - m_ROI.OrgX;

                    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height - m_ROI.OrgY;

                }
                m_ROI.DrawFrame(g, m_objRGBColor, blnHandler, fDrawingScaleX, fDrawingScaleY);
                DrawString(intROINumber, g, fDrawingScaleX, fDrawingScaleY, fPositionX, fPositionY, objColor);
            }
            catch
            {

            }
        }
        /// <summary>
        /// Draw a rectangle frame around an image or ROI
        /// </summary>
        /// <param name="g">window destination to put the drawing up</param>
        /// <param name="blnHandler">true = frame can be modified (change shape or move position), false if otherwise</param>     
        /// <param name="strText">give a name to this ROI frame</param>
        /// <param name="intROINumber">to assign a different color to its name and line. This param is unneccessary if single ROI is used on whole image</param>
        /// <param name="nType">1 = SearchROI, 2 = TrainROI, 3 = Dont care area, 4 = SubROI</param>
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, string strText, int intType, int intROINumber, float fPositionX, float fPositionY)
        {
            try
            {
                if (m_ROI.Parent == null)
                    return;

                if (m_strROIName != strText)
                    m_strROIName = strText;
                if (m_intType != intType)
                    m_intType = intType;

                if (m_strROIName == "ReTest ROI")
                    m_Color[0] = Color.GreenYellow;

                // Frame Position ON = frame is centered on image edge, Inside = outer edge of frame remain inside of image, Outside = inner edge of frame remain outside of image</param>
                //m_ROI.DrawFrame(g, new Pen(m_Color[intROINumber], 2), EFramePosition.Outside, blnHandler, fDrawingScaleX, fDrawingScaleY);
                //m_ROI.DrawFrame(g, new ERGBColor(m_Color[intROINumber].R, m_Color[intROINumber].G, m_Color[intROINumber].B), blnHandler, fDrawingScaleX, fDrawingScaleY);
                if (m_objRGBColor.Red != m_Color[intROINumber].R)
                    m_objRGBColor.Red = m_Color[intROINumber].R;
                if (m_objRGBColor.Green != m_Color[intROINumber].G)
                    m_objRGBColor.Green = m_Color[intROINumber].G;
                if (m_objRGBColor.Blue != m_Color[intROINumber].B)
                    m_objRGBColor.Blue = m_Color[intROINumber].B;

                //if (m_ROI.OrgX < 0)
                //    m_ROI.OrgX = 0;
                //if (m_ROI.OrgY < 0)
                //    m_ROI.OrgY = 0;
                //if (m_ROI.Width < 10)
                //    m_ROI.Width = 10;
                //if (m_ROI.Height < 10)
                //    m_ROI.Height = 10;

                //if (m_ROI.Parent != null)
                //{
                //    if (m_ROI.Width > m_ROI.Parent.Width)
                //        m_ROI.Width = m_ROI.Parent.Width;


                //    if (m_ROI.Height > m_ROI.Parent.Height)
                //        m_ROI.Height = m_ROI.Parent.Height;

                //    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                //        m_ROI.OrgX = m_ROI.Parent.Width - m_ROI.Width;

                //    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                //       m_ROI.OrgY = m_ROI.Parent.Height - m_ROI.Height;
                //}
                if (m_ROI.OrgX < 0)
                {
                    m_ROI.Width += m_ROI.OrgX;
                    m_ROI.OrgX = 0;
                }
                if (m_ROI.OrgY < 0)
                {
                    m_ROI.Height += m_ROI.OrgY;
                    m_ROI.OrgY = 0;
                }
                if (m_ROI.Width < 10)
                    m_ROI.Width = 10;
                if (m_ROI.Height < 10)
                    m_ROI.Height = 10;

                if (m_ROI.Parent != null)
                {
                    if (m_ROI.Width > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width;

                    if (m_ROI.Height > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height;

                    if (m_ROI.OrgX > m_ROI.Parent.Width - 10)
                        m_ROI.OrgX = m_ROI.Parent.Width - 10;

                    if (m_ROI.OrgY > m_ROI.Parent.Height - 10)
                        m_ROI.OrgY = m_ROI.Parent.Height - 10;

                    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width - m_ROI.OrgX;

                    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height - m_ROI.OrgY;

                }
                m_ROI.DrawFrame(g, m_objRGBColor, blnHandler, fDrawingScaleX, fDrawingScaleY);
                DrawString(intROINumber, g, fDrawingScaleX, fDrawingScaleY, fPositionX, fPositionY);
            }
            catch
            {

            }
        }
        public void DrawROI_ForSeal(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, int intROINumber, float fPositionX, float fPositionY, Color objColor)
        {
            try
            {
                if (m_ROI.Parent == null)
                    return;
                
                if (m_strROIName == "ReTest ROI")
                    m_Color[0] = Color.GreenYellow;

                // Frame Position ON = frame is centered on image edge, Inside = outer edge of frame remain inside of image, Outside = inner edge of frame remain outside of image</param>
                //m_ROI.DrawFrame(g, new Pen(m_Color[intROINumber], 2), EFramePosition.Outside, blnHandler, fDrawingScaleX, fDrawingScaleY);
                //m_ROI.DrawFrame(g, new ERGBColor(m_Color[intROINumber].R, m_Color[intROINumber].G, m_Color[intROINumber].B), blnHandler, fDrawingScaleX, fDrawingScaleY);
                if (m_objRGBColor.Red != objColor.R)
                    m_objRGBColor.Red = objColor.R;
                if (m_objRGBColor.Green != objColor.G)
                    m_objRGBColor.Green = objColor.G;
                if (m_objRGBColor.Blue != objColor.B)
                    m_objRGBColor.Blue = objColor.B;

                //if (m_ROI.OrgX < 0)
                //    m_ROI.OrgX = 0;
                //if (m_ROI.OrgY < 0)
                //    m_ROI.OrgY = 0;
                //if (m_ROI.Width < 10)
                //    m_ROI.Width = 10;
                //if (m_ROI.Height < 10)
                //    m_ROI.Height = 10;

                //if (m_ROI.Parent != null)
                //{
                //    if (m_ROI.Width > m_ROI.Parent.Width)
                //        m_ROI.Width = m_ROI.Parent.Width;


                //    if (m_ROI.Height > m_ROI.Parent.Height)
                //        m_ROI.Height = m_ROI.Parent.Height;

                //    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                //        m_ROI.OrgX = m_ROI.Parent.Width - m_ROI.Width;

                //    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                //       m_ROI.OrgY = m_ROI.Parent.Height - m_ROI.Height;
                //}
                if (m_ROI.OrgX < 0)
                {
                    m_ROI.Width += m_ROI.OrgX;
                    m_ROI.OrgX = 0;
                }
                if (m_ROI.OrgY < 0)
                {
                    m_ROI.Height += m_ROI.OrgY;
                    m_ROI.OrgY = 0;
                }
                if (m_ROI.Width < 10)
                    m_ROI.Width = 10;
                if (m_ROI.Height < 10)
                    m_ROI.Height = 10;

                if (m_ROI.Parent != null)
                {
                    if (m_ROI.Width > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width;

                    if (m_ROI.Height > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height;

                    if (m_ROI.OrgX > m_ROI.Parent.Width - 10)
                        m_ROI.OrgX = m_ROI.Parent.Width - 10;

                    if (m_ROI.OrgY > m_ROI.Parent.Height - 10)
                        m_ROI.OrgY = m_ROI.Parent.Height - 10;

                    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width - m_ROI.OrgX;

                    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height - m_ROI.OrgY;

                }
                m_ROI.DrawFrame(g, m_objRGBColor, blnHandler, fDrawingScaleX, fDrawingScaleY);
                DrawString(intROINumber, g, fDrawingScaleX, fDrawingScaleY, fPositionX, fPositionY, intROINumber, objColor);
            }
            catch
            {

            }
        }
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, string strDrawText, int intLineWidth, Color objColor)
        {
            try
            {
                if (m_ROI.Parent == null)
                    return;


                //if (m_ROI.OrgX < 0)
                //    m_ROI.OrgX = 0;
                //if (m_ROI.OrgY < 0)
                //    m_ROI.OrgY = 0;
                //if (m_ROI.Width < 10)
                //    m_ROI.Width = 10;
                //if (m_ROI.Height < 10)
                //    m_ROI.Height = 10;

                //if (m_ROI.Parent != null)
                //{
                //    if (m_ROI.Width > m_ROI.Parent.Width)
                //        m_ROI.Width = m_ROI.Parent.Width;


                //    if (m_ROI.Height > m_ROI.Parent.Height)
                //        m_ROI.Height = m_ROI.Parent.Height;

                //    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                //        m_ROI.OrgX = m_ROI.Parent.Width - m_ROI.Width;

                //    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                //        m_ROI.OrgY = m_ROI.Parent.Height - m_ROI.Height;
                //}
                if (m_ROI.OrgX < 0)
                {
                    m_ROI.Width += m_ROI.OrgX;
                    m_ROI.OrgX = 0;
                }
                if (m_ROI.OrgY < 0)
                {
                    m_ROI.Height += m_ROI.OrgY;
                    m_ROI.OrgY = 0;
                }
                if (m_ROI.Width < 10)
                    m_ROI.Width = 10;
                if (m_ROI.Height < 10)
                    m_ROI.Height = 10;

                if (m_ROI.Parent != null)
                {
                    if (m_ROI.Width > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width;

                    if (m_ROI.Height > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height;

                    if (m_ROI.OrgX > m_ROI.Parent.Width - 10)
                        m_ROI.OrgX = m_ROI.Parent.Width - 10;

                    if (m_ROI.OrgY > m_ROI.Parent.Height - 10)
                        m_ROI.OrgY = m_ROI.Parent.Height - 10;

                    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                        m_ROI.Width = m_ROI.Parent.Width - m_ROI.OrgX;

                    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                        m_ROI.Height = m_ROI.Parent.Height - m_ROI.OrgY;

                }
                if (m_objRGBColor.Red != objColor.R)
                    m_objRGBColor.Red = objColor.R;
                if (m_objRGBColor.Green != objColor.G)
                    m_objRGBColor.Green = objColor.G;
                if (m_objRGBColor.Blue != objColor.B)
                    m_objRGBColor.Blue = objColor.B;
                // Frame Position ON = frame is centered on image edge, Inside = outer edge of frame remain inside of image, Outside = inner edge of frame remain outside of image</param>
                //m_ROI.DrawFrame(g, EFramePosition.Outside, blnHandler, fDrawingScaleX, fDrawingScaleY);
                m_ROI.DrawFrame(g, m_objRGBColor, blnHandler, fDrawingScaleX, fDrawingScaleY);
                g.DrawString(strDrawText, m_Font, new SolidBrush(objColor), m_ROI.TotalOrgX * fDrawingScaleX, m_ROI.TotalOrgY * fDrawingScaleY);
            }
            catch
            {

            }
        }

        /// <summary>
        ///  Draw a rectangle frame around an image or ROI
        /// </summary>
        /// <param name="g">window destination to put the drawing up</param>
        /// <param name="blnHandler">true = frame can be modified (change shape or move position), false if otherwise</param>
        /// <param name="strText">give a name to this ROI frame</param>
        /// <param name="intROINumber">to assign a different color to its name and line. This param is unneccessary if single ROI is used on whole image</param>
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, string strText, int intROINumber)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, strText, m_intType, intROINumber);
        }

        /// <summary>
        ///  Draw a rectangle frame around an image or ROI
        /// </summary>
        /// <param name="g">window destination to put the drawing up</param>
        /// <param name="blnHandler">true = frame can be modified (change shape or move position), false if otherwise</param>
        /// <param name="intType">1 = SearchROI, 2 = TrainROI, 3 = Dont care area, 4 = SubROI</param>
        /// <param name="strText">give a name to this ROI frame</param>
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, int intROINumber, string strText, Color objColor)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, strText, m_intType, intROINumber, objColor);
        }

        /// <summary>
        /// Draw a rectangle frame around an image or ROI
        /// </summary>
        /// <param name="g">window destination to put the drawing up</param>
        /// <param name="blnHandler">true = frame can be modified (change shape or move position), false if otherwise</param>
        /// <param name="intROINumber">to assign a different color to its name and line. This param is unneccessary if single ROI is used on whole image</param>
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, int intROINumber)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, m_strROIName, m_intType, intROINumber);
        }
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, int intROINumber, Color objColor)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, m_strROIName, m_intType, intROINumber, objColor);
        }

        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, int intROINumber, Color objColor, int nostring1, int nostring2)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, m_strROIName, m_intType, intROINumber, objColor, nostring1, nostring2);
        }

        public void DrawDontCareROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, int intROINumber)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, "", m_intType, intROINumber);
        }
        public void DrawDontCareROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, int intROINumber, Color objColor)
        {
            if (blnHandler)
                DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, "", m_intType, intROINumber, objColor);
            else
                DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, "", m_intType, intROINumber, objColor);
        }
        /// <summary>
        /// Draw a rectangle frame around an image or ROI
        /// </summary>
        /// <param name="g">window destination to put the drawing up</param>
        /// <param name="blnHandler">true = frame can be modified (change shape or move position), false if otherwise</param>
        /// <param name="intROINumber">to assign a different color to its name and line. This param is unneccessary if single ROI is used on whole image</param>
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, int intROINumber, float fPositionX, float fPositionY)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, m_strROIName, m_intType, intROINumber, fPositionX, fPositionY);
        }
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, int intROINumber, float fPositionX, float fPositionY, Color objColor)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, m_strROIName, m_intType, intROINumber, fPositionX, fPositionY, objColor);
        }

        /// <summary>
        ///  Draw a rectangle frame around an image or ROI
        /// </summary>
        /// <param name="g">window destination to put the drawing up</param>
        /// <param name="blnHandler">true = frame can be modified (change shape or move position), false if otherwise</param>
        /// <param name="strText">give a name to this ROI frame</param>
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, string strText)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, strText, m_intType, 0);
        }

        /// <summary>
        ///  Draw a rectangle frame around an image or ROI
        /// </summary>
        /// <param name="g">window destination to put the drawing up</param>
        /// <param name="intROILineSize">ROI drawing line size</param>
        /// <param name="blnHandler">true = frame can be modified (change shape or move position), false if otherwise</param>
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intROILineSize, bool blnHandler)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, m_strROIName, m_intType, 0, intROILineSize);
        }

        /// <summary>
        /// Draw a rectangle frame around an image or ROI
        /// </summary>
        /// <param name="g">window destination to put the drawing up</param>
        /// <param name="blnHandler">true = frame can be modified (change shape or move position), false if otherwise</param>     
        /// <param name="strText">give a name to this ROI frame</param>
        /// <param name="intROINumber">to assign a different color to its name and line. This param is unneccessary if single ROI is used on whole image</param>
        /// <param name="nType">1 = SearchROI, 2 = TrainROI, 3 = Dont care area, 4 = SubROI</param>
        /// <param name="intZoomImageEdgeX">translation factor for panning in the horizontal direction</param>
        /// <param name="intZoomImageEdgeY">translation factor for panning in the vertical direction</param>
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, string strText, int intType, int intROINumber, int intROILineSize)
        {
            if (m_ROI.Parent == null)
                return;

            if (m_strROIName != strText)
                m_strROIName = strText;
            if (m_intType != intType)
                m_intType = intType;

            //if (m_ROI.OrgX < 0)
            //    m_ROI.OrgX = 0;
            //if (m_ROI.OrgY < 0)
            //    m_ROI.OrgY = 0;
            //if (m_ROI.Width < 10)
            //    m_ROI.Width = 10;
            //if (m_ROI.Height < 10)
            //    m_ROI.Height = 10;

            //if (m_ROI.Parent != null)
            //{
            //    if (m_ROI.Width > m_ROI.Parent.Width)
            //        m_ROI.Width = m_ROI.Parent.Width;


            //    if (m_ROI.Height > m_ROI.Parent.Height)
            //        m_ROI.Height = m_ROI.Parent.Height;

            //    if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
            //      m_ROI.OrgX = m_ROI.Parent.Width - m_ROI.Width;

            //    if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
            //        m_ROI.OrgY = m_ROI.Parent.Height - m_ROI.Height;

            //}
            if (m_ROI.OrgX < 0)
            {
                m_ROI.Width += m_ROI.OrgX;
                m_ROI.OrgX = 0;
            }
            if (m_ROI.OrgY < 0)
            {
                m_ROI.Height += m_ROI.OrgY;
                m_ROI.OrgY = 0;
            }
            if (m_ROI.Width < 10)
                m_ROI.Width = 10;
            if (m_ROI.Height < 10)
                m_ROI.Height = 10;

            if (m_ROI.Parent != null)
            {
                if (m_ROI.Width > m_ROI.Parent.Width)
                    m_ROI.Width = m_ROI.Parent.Width;

                if (m_ROI.Height > m_ROI.Parent.Height)
                    m_ROI.Height = m_ROI.Parent.Height;

                if (m_ROI.OrgX > m_ROI.Parent.Width - 10)
                    m_ROI.OrgX = m_ROI.Parent.Width - 10;

                if (m_ROI.OrgY > m_ROI.Parent.Height - 10)
                    m_ROI.OrgY = m_ROI.Parent.Height - 10;

                if ((m_ROI.OrgX + m_ROI.Width) > m_ROI.Parent.Width)
                    m_ROI.Width = m_ROI.Parent.Width - m_ROI.OrgX;

                if ((m_ROI.OrgY + m_ROI.Height) > m_ROI.Parent.Height)
                    m_ROI.Height = m_ROI.Parent.Height - m_ROI.OrgY;

            }
            // Frame Position ON = frame is centered on image edge, Inside = outer edge of frame remain inside of image, Outside = inner edge of frame remain outside of image</param>
            m_ROI.DrawFrame(g, EFramePosition.Outside, blnHandler, fDrawingScaleX, fDrawingScaleY);

            DrawString(intROINumber, g, fDrawingScaleX, fDrawingScaleY);
        }

        /// <summary>
        ///  Draw a rectangle frame around an image or ROI
        /// </summary>
        /// <param name="g">window destination to put the drawing up</param>
        /// <param name="blnHandler">true = frame can be modified (change shape or move position), false if otherwise</param>
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, m_strROIName, m_intType, 0);
        }
        public void DrawROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, Color objColor)
        {
            DrawROI(g, fDrawingScaleX, fDrawingScaleY, blnHandler, m_strROIName, m_intType, 0, objColor);
        }

        public void DrawZoomImage(Graphics g, int intPanelWidth, int intPanelHeight)
        {
            if (m_ROI.TopParent == null)
            {
                return;
            }

            if (intPanelWidth == 0)
                intPanelWidth = 1;

            if (intPanelHeight == 0)
                intPanelHeight = 1;

            float fScaleRate = Math.Min(intPanelWidth / (float)m_ROI.Width, intPanelHeight / (float)m_ROI.Height);
            fScaleRate = Math.Min(fScaleRate, 1f);

            if (m_objZoomImage == null)
                m_objZoomImage = new EImageBW8();

            ImageDrawing.SetImageSize(m_objZoomImage, Convert.ToInt32(m_ROI.Width * fScaleRate), Convert.ToInt32(m_ROI.Height * fScaleRate));
            EasyImage.Copy(new EBW8(0), m_objZoomImage);    //Clear image memory to 0

            EasyImage.ScaleRotate(m_ROI, (m_ROI.Width) / 2 - 0.5f, (m_ROI.Height) / 2 - 0.5f,
                (m_objZoomImage.Width) / 2 - 0.5f, (m_objZoomImage.Height) / 2 - 0.5f, fScaleRate, fScaleRate, 0.0f, m_objZoomImage);

            if (m_objDisplayImage == null)
                m_objDisplayImage = new EImageBW8();

            ImageDrawing.SetImageSize(m_objDisplayImage, intPanelWidth, intPanelHeight);
            EasyImage.Copy(new EBW8(0), m_objDisplayImage);    //Clear image memory to 0

            EROIBW8 objDisplayROI = new EROIBW8();
            objDisplayROI.Detach();
            objDisplayROI.Attach(m_objDisplayImage);
            objDisplayROI.SetPlacement(m_objDisplayImage.Width / 2 - m_objZoomImage.Width / 2, m_objDisplayImage.Height / 2 - m_objZoomImage.Height / 2, m_objZoomImage.Width, m_objZoomImage.Height);
            EasyImage.Copy(m_objZoomImage, objDisplayROI);
            objDisplayROI.Dispose();    // 2018 07 10 - Make sure local ROI is disposed.

            m_objDisplayImage.Draw(g);

        }


        /// <summary>
        /// Fill the all pixel in ROI
        /// </summary>
        /// <param name="intPixel">pixel value to fill into, 0 = black, 255 = white </param>
        public void FillROI(int intPixel)
        {
            EBW8 px = new EBW8((byte)intPixel);

            int intEndX = m_ROI.Width;
            int intEndY = m_ROI.Height;

            for (int i = 0; i <= intEndX; i++)
            {
                for (int j = 0; j <= intEndY; j++)
                {
                    m_ROI.SetPixel(px, i, j);
                }
            }
        }

        /// <summary>
        /// Load image from selected path
        /// </summary>
        /// <param name="strPath">selected path</param>
        /// <param name="blnHaveparent">true = has parent, false = do not has parent</param>
        public bool LoadImage(string strPath, bool blnHaveparent)
        {
            try
            {
                if (!blnHaveparent)
                {
                    Image objImage = Image.FromFile(strPath);
                    m_objParentImage = new EImageBW8();
                    m_objParentImage.SetSize(objImage.Width, objImage.Height);

                    m_ROI.Detach();
                    LoadROISetting(0, 0, objImage.Width, objImage.Height);
                    m_ROI.Attach(m_objParentImage);

                }
                m_ROI.Load(strPath);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// When start software, all ROI information needed to be loaded from CPU memory or file
        /// </summary>
        /// <param name="nOrgX">ROI top left angle position x</param>
        /// <param name="nOrgY">ROI top left angle position y</param>
        /// <param name="nWidth">ROI frame width</param>
        /// <param name="nHeight">ROI frame height</param>
        public void LoadROISetting(int nOrgX, int nOrgY, int nWidth, int nHeight)
        {
            if (nOrgX < 0)
                nOrgX = 0;
            if (nOrgY < 0)
                nOrgY = 0;
            if (nWidth < 0)
                nWidth = 1;
            if (nHeight < 0)
                nHeight = 1;

            if (m_ROI.Parent != null)
            {
                try
                {
                    if (nOrgX > m_ROI.Parent.Width)
                        nOrgX = 0;
                    if (nOrgY > m_ROI.Parent.Height)
                        nOrgY = 0;

                    if ((nOrgX + nWidth) > m_ROI.Parent.Width)
                    {
                        //int intOffset = nWidth - ((nOrgX + nWidth) - m_ROI.Parent.Width);     // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when nWidth = 51, nOrgX = 2, and parent.width = 
                        int intOffset = (nOrgX + nWidth) - m_ROI.Parent.Width;
                        if (nOrgX - intOffset >= 0)
                            nOrgX -= intOffset;
                        else
                            nWidth = nWidth - ((nOrgX + nWidth) - m_ROI.Parent.Width);
                    }

                    if ((nOrgY + nHeight) > m_ROI.Parent.Height)
                    {
                        //int intOffset = nHeight - ((nOrgY + nHeight) - m_ROI.Parent.Height);  // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when 
                        int intOffset = (nOrgY + nHeight) - m_ROI.Parent.Height;
                        if (nOrgY - intOffset >= 0)
                            nOrgY -= intOffset;
                        else
                            nHeight = nHeight - ((nOrgY + nHeight) - m_ROI.Parent.Height);
                    }
                }
                catch { }
            }


            if (nWidth < 0)
                nWidth = 0;
            if (nHeight < 0)
                nHeight = 0;

            m_ROI.SetPlacement(nOrgX, nOrgY, nWidth, nHeight);
            m_intOriHeight = nHeight;
            m_intOriWidth = nWidth;
        }

        public void LoadROISetting_MustSameSize(ROI objROI, int nOrgX, int nOrgY, int nWidth, int nHeight)
        {
            if (nOrgX < 0)
                nOrgX = 0;
            if (nOrgY < 0)
                nOrgY = 0;
            if (nWidth < 0)
                nWidth = 1;
            if (nHeight < 0)
                nHeight = 1;

            if (m_ROI.Parent != null)
            {
                try
                {
                    if (nOrgX > m_ROI.Parent.Width)
                        nOrgX = 0;
                    if (nOrgY > m_ROI.Parent.Height)
                        nOrgY = 0;

                    if ((nOrgX + nWidth) > m_ROI.Parent.Width)
                    {
                        //int intOffset = nWidth - ((nOrgX + nWidth) - m_ROI.Parent.Width);     // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when nWidth = 51, nOrgX = 2, and parent.width = 
                        int intOffset = (nOrgX + nWidth) - m_ROI.Parent.Width;
                        if (nOrgX - intOffset >= 0)
                            nOrgX -= intOffset;
                        else
                            nWidth = nWidth - ((nOrgX + nWidth) - m_ROI.Parent.Width);
                    }

                    if ((nOrgY + nHeight) > m_ROI.Parent.Height)
                    {
                        //int intOffset = nHeight - ((nOrgY + nHeight) - m_ROI.Parent.Height);  // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when 
                        int intOffset = (nOrgY + nHeight) - m_ROI.Parent.Height;
                        if (nOrgY - intOffset >= 0)
                            nOrgY -= intOffset;
                        else
                            nHeight = nHeight - ((nOrgY + nHeight) - m_ROI.Parent.Height);
                    }
                }
                catch { }
            }


            if (nWidth < 0)
                nWidth = 0;
            if (nHeight < 0)
                nHeight = 0;

            m_ROI.SetPlacement(nOrgX, nOrgY, nWidth, nHeight);
            m_intOriHeight = nHeight;
            m_intOriWidth = nWidth;

            if (m_ROI.Width != objROI.ref_ROI.Width)
            {
                int intSmallWidth = Math.Min(m_ROI.Width, objROI.ref_ROI.Width);
                m_ROI.SetPlacement(m_ROI.OrgX, m_ROI.OrgY, intSmallWidth, m_ROI.Height);
                objROI.ref_ROI.SetPlacement(objROI.ref_ROI.OrgX, objROI.ref_ROI.OrgY, intSmallWidth, objROI.ref_ROI.Height);
            }

            if (m_ROI.Height != objROI.ref_ROI.Height)
            {
                int intSmallHeight = Math.Min(m_ROI.Height, objROI.ref_ROI.Height);
                m_ROI.SetPlacement(m_ROI.OrgX, m_ROI.OrgY, m_ROI.Width, intSmallHeight);
                objROI.ref_ROI.SetPlacement(objROI.ref_ROI.OrgX, objROI.ref_ROI.OrgY, objROI.ref_ROI.Width, intSmallHeight);
            }
        }

        public void SetPosition_Center(float fCenterX, float fCenterY)
        {
            m_ROI.OrgX = (int)Math.Round(fCenterX - (float)m_ROI.Width / 2, 0, MidpointRounding.AwayFromZero);
            m_ROI.OrgY = (int)Math.Round(fCenterY - (float)m_ROI.Height / 2, 0, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Load angle ROI
        /// </summary>
        /// <param name="fAngle">angle</param>
        /// <param name="fCenterX">center X</param>
        /// <param name="fCenterY">center Y</param>
        /// <param name="fWidth">width</param>
        /// <param name="fHeight">height</param>
        /// <param name="intStartSearchROIX">start search ROI X</param>
        /// <param name="intStartSearchROIY">start search ROI Y</param>
        public void LoadAngleROISetting(float fAngle, float fCenterX, float fCenterY, float fWidth, float fHeight,
                                        int intStartSearchROIX, int intStartSearchROIY)
        {
            double fRadianAngle = (double)fAngle * Math.PI / 180; // Change angle from degree to radius unit

            float dSin = (float)Math.Sin(fRadianAngle);
            float dCos = (float)Math.Cos(fRadianAngle);
            float fHalfWidth = fWidth / 2f;
            float fHalfHeight = fHeight / 2f;
            float fActualCenterX = intStartSearchROIX + fCenterX;
            float fActualCenterY = intStartSearchROIY + fCenterY;
            PointF px = new PointF();
            PointF pX = new PointF();
            PointF py = new PointF();
            PointF pY = new PointF();
            PointF pxy = new PointF();
            PointF pXy = new PointF();
            PointF pxY = new PointF();
            PointF pXY = new PointF();

            px.X = fCenterX - (fHalfWidth * dCos);        // Mid edge X at Left Side
            px.Y = fCenterY - (fHalfWidth * dSin);         // Mid edge Y at Left Side

            pX.X = fCenterX + (fHalfWidth * dCos);       // Mid edge X at Right Side
            pX.Y = fCenterY + (fHalfWidth * dSin);        // Mid edge Y at Right Side

            pY.X = fCenterX - (fHalfHeight * dSin);      // Mid edge X at Top Side
            pY.Y = fCenterY + (fHalfHeight * dCos);      // Mid edge Y at Top Side

            py.X = fCenterX + (fHalfHeight * dSin);       // Mid edge X at Bottom Side
            py.Y = fCenterY - (fHalfHeight * dCos);      // Mid edge Y at Bottom Side

            pxy.X = px.X + (fHalfHeight * dSin);           // Corner  at Left Top 
            pxy.Y = px.Y - (fHalfHeight * dCos);

            pXy.X = py.X + (fHalfWidth * dCos);          // Corner at Right Top
            pXy.Y = py.Y + (fHalfWidth * dSin);

            pxY.X = pY.X - (fHalfWidth * dCos);           // Corner at Left Bottom
            pxY.Y = pY.Y - (fHalfWidth * dSin);

            pXY.X = pX.X - (fHalfHeight * dSin);          // Corner at Right Bottom
            pXY.Y = pX.Y + (fHalfHeight * dCos);

            if (fAngle >= 0)
                m_ROI.SetPlacement(
                    (int)Math.Round(pxY.X, 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(pxy.Y, 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((pXy.X - pxY.X), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((pXY.Y - pxy.Y), 0, MidpointRounding.AwayFromZero));
            else
                m_ROI.SetPlacement(
                    (int)Math.Round(pxy.X, 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(pXy.Y, 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((pXY.X - pxy.X), 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round((pxY.Y - pXy.Y), 0, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// Save ROI to selected path
        /// </summary>
        /// <param name="strFilePath">selected path</param>
        public void SaveImage(string strFilePath)
        {
            try
            {
                if (m_ROI.Width != 0 && m_ROI.Height != 0)
                {
                    string strDirectoryName = System.IO.Path.GetDirectoryName(strFilePath);
                    DirectoryInfo directory = new DirectoryInfo(strDirectoryName);
                    if (!directory.Exists)
                        CreateUnexistDirectory(directory);

                    m_ROI.Save(strFilePath);
                }
            }
            catch (Exception ex)
            {

            }
        }




        /// <summary>
        /// Set the ROI pixel average
        /// </summary>
        /// <param name="fPixel">pixel</param>
        public void SetROIPixelAverage(float fPixel)
        {
            m_fPixelAverage = fPixel;
        }

        /// <summary>
        /// Set ROI empty pocket pixel
        /// </summary>
        /// <param name="fPixel">pixel</param>
        public void SetROIEmptyPocketPixel(float fPixel)
        {
            m_fEmptyPocketPixel = fPixel;
        }



        /// <summary>
        /// Set RO handle - set true if Detects the cursor is placed within ROI area
        /// Usually call this function in Mouse_Down event
        /// </summary>
        /// <param name="nNewXPoint">the point of mouse move over</param>
        /// <param name="nNewYPoint">the point of mouse move over</param>
        public bool VerifyROIArea(int nNewXPoint, int nNewYPoint)
        {
            if (m_ROI.TopParent == null)
                return false;

            int intRangeTolerance = 10;
            if (m_ROI.Width < 40 || m_ROI.Height < 40)
                intRangeTolerance = 3;
            else if (m_ROI.Width < 100 || m_ROI.Height < 100)
                intRangeTolerance = 5;

            if (((nNewXPoint < (m_ROI.TotalOrgX + intRangeTolerance))
                    && (nNewYPoint > (m_ROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY) && (nNewXPoint > m_ROI.TotalOrgX))
                   || ((nNewXPoint > (m_ROI.TotalOrgX - intRangeTolerance))
                    && (nNewYPoint < (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY) && (nNewXPoint < m_ROI.TotalOrgX))
                     || ((nNewXPoint < (m_ROI.TotalOrgX + intRangeTolerance))
                    && (nNewYPoint < (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY) && (nNewXPoint > m_ROI.TotalOrgX))
                     || ((nNewXPoint > (m_ROI.TotalOrgX - intRangeTolerance))
                    && (nNewYPoint > (m_ROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY) && (nNewXPoint < m_ROI.TotalOrgX))
                    )
            {
                m_Handler = m_ROI.HitTest(m_ROI.TotalOrgX, m_ROI.TotalOrgY);

            }
            else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
                 && (nNewYPoint > (m_ROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width))
                || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
                 && (nNewYPoint < (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width))
                  || ((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
                 && (nNewYPoint < (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width))
                  || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
                 && (nNewYPoint > (m_ROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width))
                 )
            {
                m_Handler = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width, m_ROI.TotalOrgY);

            }
            else if (((nNewXPoint < (m_ROI.TotalOrgX + intRangeTolerance))
              && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height) && (nNewXPoint > m_ROI.TotalOrgX))
             || ((nNewXPoint > (m_ROI.TotalOrgX - intRangeTolerance))
              && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height) && (nNewXPoint < m_ROI.TotalOrgX))
               || ((nNewXPoint < (m_ROI.TotalOrgX + intRangeTolerance))
              && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height) && (nNewXPoint > m_ROI.TotalOrgX))
               || ((nNewXPoint > (m_ROI.TotalOrgX - intRangeTolerance))
              && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height) && (nNewXPoint < m_ROI.TotalOrgX))
              )
            {
                m_Handler = m_ROI.HitTest(m_ROI.TotalOrgX, m_ROI.TotalOrgY + m_ROI.Height);

            }
            else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
              && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width))
             || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
              && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width))
               || ((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
              && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width))
               || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
              && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width))
              )
            {
                m_Handler = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width, m_ROI.TotalOrgY + m_ROI.Height);

            }
            else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
                && (nNewYPoint > (m_ROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX + intRangeTolerance)))
            {
                m_Handler = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width / 2, m_ROI.TotalOrgY);

            }
            //else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width / 2 + intRangeTolerance))
            //  && (nNewYPoint > (m_ROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width / 2))
            // || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width / 2 - intRangeTolerance))
            //  && (nNewYPoint < (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width / 2))
            //   || ((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width / 2 + intRangeTolerance))
            //  && (nNewYPoint < (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width / 2))
            //   || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width / 2 - intRangeTolerance))
            //  && (nNewYPoint > (m_ROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width / 2))
            //  )
            //{
            //    m_Handler = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width / 2, m_ROI.TotalOrgY);

            //}
            else if (((nNewXPoint < (m_ROI.TotalOrgX + intRangeTolerance))
                && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX - intRangeTolerance)))
            {
                m_Handler = m_ROI.HitTest(m_ROI.TotalOrgX, m_ROI.TotalOrgY + m_ROI.Height / 2);

            }
            // else if (((nNewXPoint < (m_ROI.TotalOrgX + intRangeTolerance))
            // && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height / 2 - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height / 2) && (nNewXPoint > m_ROI.TotalOrgX))
            //|| ((nNewXPoint > (m_ROI.TotalOrgX - intRangeTolerance))
            // && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height / 2 + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height / 2) && (nNewXPoint < m_ROI.TotalOrgX))
            //  || ((nNewXPoint < (m_ROI.TotalOrgX + intRangeTolerance))
            // && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height / 2 + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height / 2) && (nNewXPoint > m_ROI.TotalOrgX))
            //  || ((nNewXPoint > (m_ROI.TotalOrgX - intRangeTolerance))
            // && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height / 2 - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height / 2) && (nNewXPoint < m_ROI.TotalOrgX))
            // )
            // {
            //     m_Handler = m_ROI.HitTest(m_ROI.TotalOrgX, m_ROI.TotalOrgY + m_ROI.Height / 2);

            // }
            else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
              && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX + intRangeTolerance)))
            {
                m_Handler = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width / 2, m_ROI.TotalOrgY + m_ROI.Height);

            }
            //else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width / 2 + intRangeTolerance))
            //    && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width / 2))
            //    || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width / 2 - intRangeTolerance))
            //    && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width / 2))
            //    || ((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width / 2 + intRangeTolerance))
            //    && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width / 2))
            //    || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width / 2 - intRangeTolerance))
            //    && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width / 2))
            //    )
            //{
            //    m_Handler = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width / 2, m_ROI.TotalOrgY + m_ROI.Height);

            //}
            else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
                && (nNewYPoint > (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance)))
            {
                m_Handler = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width, m_ROI.TotalOrgY + m_ROI.Height / 2);

            }
            //   else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
            // && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height / 2 - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height / 2) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width))
            //|| ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
            // && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height / 2 + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height / 2) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width))
            //  || ((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
            // && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height / 2 + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height / 2) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width))
            //  || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
            // && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height / 2 - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height / 2) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width))
            // )
            //   {
            //       m_Handler = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width, m_ROI.TotalOrgY + m_ROI.Height / 2);

            //   }
            else
            {
                m_Handler = m_ROI.HitTest(nNewXPoint, nNewYPoint);
            }
            return GetROIHandle();
        }

        /// <summary>
        /// Compare Search ROI template area with current unit area
        /// </summary>
        /// <param name="intTolerance">tolerance</param>
        /// <returns>true = difference less than or equal to tolerance, false = difference > tolerance</returns> 
        public bool CompareROIArea(int intTolerance)
        {
            float fDifference = (m_fPixelAverage - GetROIAreaPixel()) / m_fPixelAverage * 100;

            if (Math.Abs(fDifference) > intTolerance)
                return false;

            return true;
        }

        /// <summary>
        /// Check whether unit is present
        /// </summary>
        /// <param name="intTolerance">tolerance</param>
        /// <param name="objROI">ROI</param>
        /// <returns>true = gap < tolerance, false = gap > tolerance</returns>
        public bool IsUnitPresent(int intTolerance, ROI objROI)
        {
            int intArea = GetWhiteObjectArea(objROI);
            int intGap = (intArea - m_intUnitArea) / m_intUnitArea * 100;
            if (Math.Abs(intGap) > intTolerance)
                return false;
            return true;
        }

        public bool IsUnitEdgeOnCenterUnitSurface(int intThresholdValue)
        {
            EBlobs blob = new EBlobs(); // ENG: Wondering This 

            blob.BuildObjects_Filter_GetElement(this, false, true, 0, intThresholdValue,
                Math.Min(m_ROI.Width, m_ROI.Height), 15000, false, 0x08);
            //blob.ref_Blob.BuildObjects(m_ROI);

            float fWidth = 0, fHeight = 0;
            int intBlobNumSelectedObject = blob.ref_intNumSelectedObject;
            if (intBlobNumSelectedObject > 0)
            {
                for (int i = 0; i < intBlobNumSelectedObject; i++)
                {
                    fWidth = blob.ref_arrWidth[i];
                    fHeight = blob.ref_arrHeight[i];

                    if ((fHeight / m_ROI.Height) > 0.9)
                    {
                        return true;
                    }
                    else if ((fWidth / m_ROI.Width) > 0.9)
                    {
                        return true;
                    }
                }
            }

            blob.Dispose();

            return false;
        }

        /// <summary>
        /// Check whether unit is empty
        /// </summary>
        /// <param name="intTolerance">tolerance</param>
        /// <param name="objROI">ROI</param>
        /// <returns>true = gap < tolerance, false = gap > tolerance</returns>
        public bool IsUnitEmpty(int intTolerance, ROI objROI)
        {
            int intArea = GetWhiteObjectArea(objROI);
            int intGap = (intArea - m_intEmptyPocketArea) / m_intEmptyPocketArea * 100;
            if (Math.Abs(intGap) > intTolerance)
                return false;
            return true;
        }


        /// <summary>
        /// Check whether image had been attached to this ROI
        /// </summary>
        /// <returns>false if there is no image being attached to ROI</returns>
        public bool CheckROIParent()
        {
            if (m_ROI.Parent == null)
                return false;

            return true;
        }

        /// <summary>
        /// return true if movement or changing is allowed for this ROI Frame
        /// </summary>
        public bool GetROIHandle()
        {
            if (m_Handler != EDragHandle.NoHandle)
                return true;
            else
                return false;
        }
        public bool GetROIInsideHandle()
        {
            if (m_Handler == EDragHandle.Inside)
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


        public bool VerifyROIHandleShape(int nNewXPoint, int nNewYPoint)
        {
            if (m_ROI.TopParent == null)
                return false;

            int intRangeTolerance = 10;
            if (m_ROI.Width < 40 || m_ROI.Height < 40)
                intRangeTolerance = 3;
            else if (m_ROI.Width < 100 || m_ROI.Height < 100)
                intRangeTolerance = 5;

            if (((nNewXPoint < (m_ROI.TotalOrgX + intRangeTolerance))
                    && (nNewYPoint > (m_ROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX - intRangeTolerance))
                   || ((nNewXPoint > (m_ROI.TotalOrgX - intRangeTolerance))
                    && (nNewYPoint < (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY - intRangeTolerance) && (nNewXPoint < m_ROI.TotalOrgX + intRangeTolerance))
                     || ((nNewXPoint < (m_ROI.TotalOrgX + intRangeTolerance))
                    && (nNewYPoint < (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY - intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX - intRangeTolerance))
                     || ((nNewXPoint > (m_ROI.TotalOrgX - intRangeTolerance))
                    && (nNewYPoint > (m_ROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + intRangeTolerance) && (nNewXPoint < m_ROI.TotalOrgX + intRangeTolerance))
                    )
            {
                m_Handler2 = m_ROI.HitTest(m_ROI.TotalOrgX, m_ROI.TotalOrgY);
            }
            else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
                 && (nNewYPoint > (m_ROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
                || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
                 && (nNewYPoint < (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY - intRangeTolerance) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
                  || ((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
                 && (nNewYPoint < (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY - intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
                  || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
                 && (nNewYPoint > (m_ROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + intRangeTolerance) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
                 )
            {
                m_Handler2 = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width, m_ROI.TotalOrgY);
            }
            else if (((nNewXPoint < (m_ROI.TotalOrgX + intRangeTolerance))
              && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX - intRangeTolerance))
             || ((nNewXPoint > (m_ROI.TotalOrgX - intRangeTolerance))
              && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance) && (nNewXPoint < m_ROI.TotalOrgX + intRangeTolerance))
               || ((nNewXPoint < (m_ROI.TotalOrgX + intRangeTolerance))
              && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX - intRangeTolerance))
               || ((nNewXPoint > (m_ROI.TotalOrgX - intRangeTolerance))
              && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance) && (nNewXPoint < m_ROI.TotalOrgX + intRangeTolerance))
              )
            {
                m_Handler2 = m_ROI.HitTest(m_ROI.TotalOrgX, m_ROI.TotalOrgY + m_ROI.Height);
            }
            else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
              && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
             || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
              && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
               || ((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
              && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
               || ((nNewXPoint > (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
              && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance) && (nNewXPoint < m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
              )
            {
                m_Handler2 = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width, m_ROI.TotalOrgY + m_ROI.Height);

            }
            else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
                && (nNewYPoint > (m_ROI.TotalOrgY - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX + intRangeTolerance)))
            {
                m_Handler2 = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width / 2, m_ROI.TotalOrgY);

            }
            else if (((nNewXPoint < (m_ROI.TotalOrgX + intRangeTolerance))
                && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint > m_ROI.TotalOrgY + intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX - intRangeTolerance)))
            {
                m_Handler2 = m_ROI.HitTest(m_ROI.TotalOrgX, m_ROI.TotalOrgY + m_ROI.Height / 2);

            }
            else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance))
              && (nNewYPoint > (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height + intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX + intRangeTolerance)))
            {
                m_Handler2 = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width / 2, m_ROI.TotalOrgY + m_ROI.Height);

            }
            else if (((nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width + intRangeTolerance))
                && (nNewYPoint > (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint < m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance) && (nNewXPoint > m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance)))
            {
                m_Handler2 = m_ROI.HitTest(m_ROI.TotalOrgX + m_ROI.Width, m_ROI.TotalOrgY + m_ROI.Height / 2);

            }
            else if ((nNewXPoint > (m_ROI.TotalOrgX + intRangeTolerance)) && (nNewXPoint < (m_ROI.TotalOrgX + m_ROI.Width - intRangeTolerance)) && (nNewYPoint > (m_ROI.TotalOrgY + intRangeTolerance)) && (nNewYPoint < (m_ROI.TotalOrgY + m_ROI.Height - intRangeTolerance)))
            {
                m_Handler2 = EDragHandle.Inside;
            }
            else
            {
                m_Handler2 = EDragHandle.NoHandle;
            }

            switch (m_Handler2)
            {
                case EDragHandle.NoHandle:
                    Cursor.Current = Cursors.Default;
                    break;
                case EDragHandle.Inside:
                    Cursor.Current = Cursors.SizeAll;
                    break;
                case EDragHandle.North:
                    Cursor.Current = Cursors.SizeNS;
                    break;
                case EDragHandle.East:
                    Cursor.Current = Cursors.SizeWE;
                    break;
                case EDragHandle.South:
                    Cursor.Current = Cursors.SizeNS;
                    break;
                case EDragHandle.West:
                    Cursor.Current = Cursors.SizeWE;
                    break;
                case EDragHandle.NorthWest:
                    Cursor.Current = Cursors.SizeNWSE;
                    break;
                case EDragHandle.SouthWest:
                    Cursor.Current = Cursors.SizeNESW;
                    break;
                case EDragHandle.NorthEast:
                    Cursor.Current = Cursors.SizeNESW;
                    break;
                case EDragHandle.SouthEast:
                    Cursor.Current = Cursors.SizeNWSE;
                    break;
            }
            return true;
        }

        /// <summary>
        /// Attach all search roi to image
        /// </summary>
        /// <param name="arrROIs"></param>
        /// <param name="objAttachedImage"></param>
        public static void AttachROIToImage(List<List<ROI>> arrROIs, ImageDrawing objAttachedImage)
        {
            for (int i = 0; i < arrROIs.Count; i++)
            {
                if (arrROIs[i].Count > 0)
                    arrROIs[i][0].AttachImage(objAttachedImage);
            }
        }

        /// <summary>
        /// Get white object/blobs area
        /// </summary>
        /// <param name="objROI">ROI</param>
        /// <returns>blobs area</returns>
        public static int GetWhiteObjectArea(ROI objROI)
        {
            EBlobs blob = new EBlobs(); // ENG: Wondering This local create Eblobs and dispose nonstop will cause error or not

            blob.BuildObjects_Filter_GetElement(objROI, false, true, 0, 254,
                0, 15000, false, 0x01);

            int intArea = 0;
            if (blob.ref_intNumSelectedObject > 0)
            {
                intArea = blob.ref_arrArea[0];
            }

            blob.Dispose();

            return intArea;
        }

        public static int GetPixelArea(ROI objROI, int intThresholdValue, int intBlackWhite)
        {
            try
            {
                // Get pixel count for high and low threshold pixel count
                int intLowThresholdPixelCount = 0, intBtwThresholdPixelCount = 0, intHighThresholdPixelCount = 0;
                EBW8 bwThresholdValue = new EBW8((byte)intThresholdValue);
                EasyImage.PixelCount(objROI.ref_ROI, bwThresholdValue, bwThresholdValue,
                                     out intLowThresholdPixelCount, out intBtwThresholdPixelCount, out intHighThresholdPixelCount);

                // Die area is considered as empty if die inverted area lower than limit
                if (intBlackWhite == 0) // count black area
                    return intLowThresholdPixelCount;
                else if (intBlackWhite == 1)
                    return intHighThresholdPixelCount;
                else
                    return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Get Threshold Value according to Selected Mode
        /// </summary>
        /// <param name="objROI">Source Image</param>
        /// <param name="intThresholdMode">0 = absolute, 1 = Isodata, 2 = MaxEntropy, 3 = MinResidue, 4 = Relative</param>
        /// <returns>threshold value</returns>
        public static int GetAutoThresholdValue(ROI objROI, int intThresholdMode)
        {
            EBW8 objBW8;
            if (intThresholdMode == 0)
                objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.Absolute);
            else if (intThresholdMode == 1)
                objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.Isodata);
            else if (intThresholdMode == 2)
                objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MaxEntropy);
            else if (intThresholdMode == 3)
                objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.MinResidue);
            else
                objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.Relative);

            return objBW8.Value;
        }
        public static int GetRelativeThresholdValue(ROI objROI, float fRelativeValue)
        {
            EBW8 objBW8;
            objBW8 = EasyImage.AutoThreshold(objROI.ref_ROI, EThresholdMode.Relative, fRelativeValue);
            return objBW8.Value;
        }
        /// <summary>
        /// Gain offset image
        /// </summary>
        /// <param name="objSearchROISource">source ROI</param>
        /// <param name="objImage">destination image</param>
        public static void ModifyImageGain(ROI objSearchROISource, ImageDrawing objImage)
        {
            ROI objDest = new ROI();
            ImageDrawing objTempImage = new ImageDrawing(true);
            objImage.CopyTo(ref objTempImage);
            objDest.LoadROISetting(objSearchROISource.ref_ROIPositionX, objSearchROISource.ref_ROIPositionY,
                objSearchROISource.ref_ROIWidth, objSearchROISource.ref_ROIHeight);
            objDest.AttachImage(objTempImage);

            EBW8 intMinValue = new EBW8();
            EasyImage.PixelMin(objSearchROISource.ref_ROI, out intMinValue);

            if (intMinValue.Value < 1)
                EasyImage.GainOffset(objSearchROISource.ref_ROI, objDest.ref_ROI, 1, 1);
            EasyImage.Copy(objDest.ref_ROI, objSearchROISource.ref_ROI);
        }

        /// <summary>
        ///  Rotate image to less than 90 degree of rotation angle
        /// </summary>
        /// <param name="objSearchROI">search ROI</param>
        /// <param name="fRotateAngle">rotation angle</param>
        /// <param name="objRotatedImage">Image used for rotation.(Source image and destination image are same object)</param>
        public static void Rotate0Degree(ROI objSearchROI, float fRotateAngle, ref ImageDrawing objRotatedImage)
        {
            Rotate0Degree(objSearchROI, fRotateAngle, 8, ref objRotatedImage);
        }
        /// <summary>
        ///  Rotate image to less than 90 degree of rotation angle
        /// </summary>
        /// <param name="objSearchROI">search ROI</param>
        /// <param name="fRotateAngle">rotation angle</param>
        /// <param name="intInterpolation">Rotation Interpolation;</param>
        /// <param name="objRotatedImage">Image used for rotation.(Source image and destination image are same object)</param>
        public static void Rotate0Degree(ROI objSearchROI, float fRotateAngle, int intInterpolation, ref ImageDrawing objRotatedImage)
        {
            EROIBW8 searchROI = new EROIBW8();
            EROIBW8 destinationROI = new EROIBW8();

            ImageDrawing objSourceImage = new ImageDrawing(true);
            objRotatedImage.CopyTo(ref objSourceImage);        //Copy source to new object to prevent rotation image blur

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROIPositionX;
            searchROI.OrgY = objSearchROI.ref_ROIPositionY;
            searchROI.Attach(objSourceImage.ref_objMainImage);

            destinationROI.SetSize(searchROI.Width, searchROI.Height);
            destinationROI.OrgX = searchROI.OrgX;
            destinationROI.OrgY = searchROI.OrgY;

            if (destinationROI.OrgX + destinationROI.Width >= objRotatedImage.ref_objMainImage.Width)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                objSourceImage.Dispose();
                objSourceImage = null;
                return;
            }

            if (destinationROI.OrgY + destinationROI.Height >= objRotatedImage.ref_objMainImage.Height)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                objSourceImage.Dispose();
                objSourceImage = null;
                return;
            }

            destinationROI.Attach(objRotatedImage.ref_objMainImage);
            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f - 0.5f, searchROI.Height / 2f - 0.5f, destinationROI.Width / 2f - 0.5f, destinationROI.Height / 2f - 0.5f, 1, 1, fRotateAngle, destinationROI, intInterpolation);

            searchROI.Dispose();
            destinationROI.Dispose();
            searchROI = null;
            destinationROI = null;
            objSourceImage.Dispose();
            objSourceImage = null;
        }
        /// <summary>
        ///  Rotate image to less than 90 degree of rotation angle
        /// </summary>
        /// <param name="objSearchROI">search ROI</param>
        /// <param name="fRotateAngle">rotation angle</param>
        /// <param name="objRotatedImage">Image used for rotation.(Source image and destination image are same object)</param>
        /// <param name="intImageIndex">Image index</param>
        public static void Rotate0Degree(ROI objSearchROI, float fRotateAngle, ref List<ImageDrawing> arrRotatedImage, int intImageIndex)
        {
            Rotate0Degree(objSearchROI, fRotateAngle, 8, ref arrRotatedImage, intImageIndex);
        }

        public static void Rotate0Degree(ROI objSearchROI, float fRotateAngle, ref List<ImageDrawing> arrRotatedImage, int intImageIndex, ImageDrawing objTemporarySource)
        {
            Rotate0Degree(objSearchROI, fRotateAngle, 8, ref arrRotatedImage, intImageIndex, objTemporarySource);
        }
        /// <summary>
        ///  Rotate image to less than 90 degree of rotation angle
        /// </summary>
        /// <param name="objSearchROI">search ROI</param>
        /// <param name="fRotateAngle">rotation angle</param>
        /// /// <param name="intInterpolation">Rotation Interpolation;</param>
        /// <param name="objRotatedImage">Image used for rotation.(Source image and destination image are same object)</param>
        /// <param name="intImageIndex">Image index</param>
        public static void Rotate0Degree(ROI objSearchROI, float fRotateAngle, int intInterpolation, ref List<ImageDrawing> arrRotatedImage, int intImageIndex)
        {
            EROIBW8 searchROI = new EROIBW8();
            EROIBW8 destinationROI = new EROIBW8();

            ImageDrawing objRotatedImage = arrRotatedImage[intImageIndex];
            ImageDrawing objSourceImage = new ImageDrawing(true);
            objRotatedImage.CopyTo(ref objSourceImage);        //Copy source to new object to prevent rotation image blur

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROIPositionX;
            searchROI.OrgY = objSearchROI.ref_ROIPositionY;
            searchROI.Attach(objSourceImage.ref_objMainImage);

            destinationROI.SetSize(searchROI.Width, searchROI.Height);
            destinationROI.OrgX = searchROI.OrgX;
            destinationROI.OrgY = searchROI.OrgY;

            if (destinationROI.OrgX + destinationROI.Width >= objRotatedImage.ref_objMainImage.Width)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                objSourceImage.Dispose();
                objSourceImage = null;
                return;
            }

            if (destinationROI.OrgY + destinationROI.Height >= objRotatedImage.ref_objMainImage.Height)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                objSourceImage.Dispose();
                objSourceImage = null;
                return;
            }
            destinationROI.Attach(objRotatedImage.ref_objMainImage);

            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f - 0.5f, searchROI.Height / 2f - 0.5f, destinationROI.Width / 2f - 0.5f, destinationROI.Height / 2f - 0.5f, 1, 1, fRotateAngle, destinationROI, intInterpolation);

            //objSourceImage.Dispose();
            //searchROI.Dispose();
            //destinationROI.Dispose();


            searchROI.Dispose();
            destinationROI.Dispose();
            searchROI = null;
            destinationROI = null;
            objSourceImage.Dispose();
            objSourceImage = null;
        }

        public static void Rotate0Degree(ROI objSearchROI, float fRotateAngle, int intInterpolation, ref List<ImageDrawing> arrRotatedImage, int intImageIndex, ImageDrawing objTemporarySource)
        {
            EROIBW8 searchROI = new EROIBW8();
            EROIBW8 destinationROI = new EROIBW8();

            ImageDrawing objRotatedImage = arrRotatedImage[intImageIndex];
            objRotatedImage.CopyTo(ref objTemporarySource);        //Copy source to new object to prevent rotation image blur

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROIPositionX;
            searchROI.OrgY = objSearchROI.ref_ROIPositionY;
            searchROI.Attach(objTemporarySource.ref_objMainImage);

            destinationROI.SetSize(searchROI.Width, searchROI.Height);
            destinationROI.OrgX = searchROI.OrgX;
            destinationROI.OrgY = searchROI.OrgY;

            if (destinationROI.OrgX + destinationROI.Width >= objRotatedImage.ref_objMainImage.Width)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                return;
            }

            if (destinationROI.OrgY + destinationROI.Height >= objRotatedImage.ref_objMainImage.Height)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                return;
            }

            destinationROI.Attach(objRotatedImage.ref_objMainImage);

            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f - 0.5f, searchROI.Height / 2f - 0.5f, destinationROI.Width / 2f - 0.5f, destinationROI.Height / 2f - 0.5f, 1, 1, fRotateAngle, destinationROI, intInterpolation);

            searchROI.Dispose();
            destinationROI.Dispose();

        }
        /// <summary>
        ///  Rotate image to less than 90 degree of rotation angle
        /// </summary>
        /// <param name="objSourceImage">source image</param>
        /// <param name="objSearchROI">search ROI</param>
        /// <param name="fRotateAngle">rotation angle</param>
        /// <param name="objRotatedImage">destination image</param>
        public static void Rotate0Degree(ImageDrawing objSourceImage, ROI objSearchROI, float fRotateAngle, ref ImageDrawing objRotatedImage)
        {
            Rotate0Degree(objSourceImage, objSearchROI, fRotateAngle, 8, ref objRotatedImage);
        }
        /// <summary>
        ///  Rotate image to less than 90 degree of rotation angle
        /// </summary>
        /// <param name="objSourceImage">source image</param>
        /// <param name="objSearchROI">search ROI</param>
        /// <param name="fRotateAngle">rotation angle</param>
        /// <param name="objRotatedImage">destination image</param>
        public static void Rotate0Degree(ImageDrawing objSourceImage, ROI objSearchROI, float fRotateAngle, int intInterpolation, ref ImageDrawing objRotatedImage)
        {
            EROIBW8 searchROI = new EROIBW8();
            EROIBW8 destinationROI = new EROIBW8();

            objRotatedImage = new ImageDrawing(true);   // declare as new object to prevent image blur after rotate
            objSourceImage.CopyTo(ref objRotatedImage); // copy source to rotated image to prevent rotated image non-ROI area become black

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROIPositionX;
            searchROI.OrgY = objSearchROI.ref_ROIPositionY;
            searchROI.Attach(objSourceImage.ref_objMainImage);

            destinationROI.SetSize(searchROI.Width, searchROI.Height);
            destinationROI.OrgX = searchROI.OrgX;
            destinationROI.OrgY = searchROI.OrgY;

            if (destinationROI.OrgX + destinationROI.Width >= objRotatedImage.ref_objMainImage.Width)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                return;
            }

            if (destinationROI.OrgY + destinationROI.Height >= objRotatedImage.ref_objMainImage.Height)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                return;
            }

            destinationROI.Attach(objRotatedImage.ref_objMainImage);

            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f - 0.5f, searchROI.Height / 2f - 0.5f, destinationROI.Width / 2f - 0.5f, destinationROI.Height / 2f - 0.5f, 1, 1, fRotateAngle, destinationROI, intInterpolation);

            searchROI.Dispose();
            destinationROI.Dispose();

        }
     
        public static void Rotate0Degree_Better(ImageDrawing objSourceImage, ROI objSearchROI, float fRotateAngle, int intInterpolation, ref ImageDrawing objRotatedImage)
        {
            EROIBW8 searchROI = new EROIBW8();
            EROIBW8 destinationROI = new EROIBW8();

            // 2020 05 18 - CCENG: should not new declare again if not able to dispose.
            //objRotatedImage = new ImageDrawing(true);   // declare as new object to prevent image blur after rotate
            objSourceImage.CopyTo(ref objRotatedImage); // copy source to rotated image to prevent rotated image non-ROI area become black

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROIPositionX;
            searchROI.OrgY = objSearchROI.ref_ROIPositionY;
            searchROI.Attach(objSourceImage.ref_objMainImage);

            destinationROI.SetSize(searchROI.Width, searchROI.Height);
            destinationROI.OrgX = searchROI.OrgX;
            destinationROI.OrgY = searchROI.OrgY;

            if (destinationROI.OrgX + destinationROI.Width >= objRotatedImage.ref_objMainImage.Width)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                return;
            }

            if (destinationROI.OrgY + destinationROI.Height >= objRotatedImage.ref_objMainImage.Height)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                return;
            }

            destinationROI.Attach(objRotatedImage.ref_objMainImage);

            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f - 0.5f, searchROI.Height / 2f - 0.5f, destinationROI.Width / 2f - 0.5f, destinationROI.Height / 2f - 0.5f, 1, 1, fRotateAngle, destinationROI, intInterpolation);

            searchROI.Dispose();
            destinationROI.Dispose();

        }
        public static void Rotate0Degree_ForDontCare(ROI objSearchROI, float fRotateAngle, int intInterpolation, ROI objdestinationROI)
        {
            EasyImage.ScaleRotate(objSearchROI.ref_ROI, objSearchROI.ref_ROIWidth / 2f - 0.5f, objSearchROI.ref_ROIHeight / 2f - 0.5f, objdestinationROI.ref_ROIWidth / 2f - 0.5f, objdestinationROI.ref_ROIHeight / 2f - 0.5f, 1, 1, fRotateAngle, objdestinationROI.ref_ROI, intInterpolation);
            
        }
        /// <summary>
        ///  Rotate image to less than 90 degree of rotation angle
        /// </summary>
        /// <param name="objSourceImage">source image</param>
        /// <param name="objSearchROI">search ROI</param>
        /// <param name="fRotateAngle">rotation angle</param>
        /// <param name="objRotatedImage">destination image</param>
        public static void Rotate0Degree(ImageDrawing objSourceImage, ROI objSearchROI, float fRotateAngle, ref List<ImageDrawing> arrRotatedImage, int intImageIndex)
        {
            Rotate0Degree(objSourceImage, objSearchROI, fRotateAngle, 4, ref arrRotatedImage, intImageIndex);
        }
        /// <summary>
        ///  Rotate image to less than 90 degree of rotation angle
        /// </summary>
        /// <param name="objSourceImage">source image</param>
        /// <param name="objSearchROI">search ROI</param>
        /// <param name="fRotateAngle">rotation angle</param>
        /// <param name="objRotatedImage">destination image</param>
        public static void Rotate0Degree(ImageDrawing objSourceImage, ROI objSearchROI, float fRotateAngle, int intInterpolation, ref List<ImageDrawing> arrRotatedImage, int intImageIndex)
        {
            if (objSearchROI.ref_ROIWidth == 0 || objSearchROI.ref_ROIHeight == 0)
                return;

            EROIBW8 searchROI = new EROIBW8();
            EROIBW8 destinationROI = new EROIBW8();

            ImageDrawing objRotatedImage = arrRotatedImage[intImageIndex];
            objSourceImage.CopyTo(ref objRotatedImage); // copy source to rotated image to prevent rotated image non-ROI area become black

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROITotalX;
            searchROI.OrgY = objSearchROI.ref_ROITotalY;
            searchROI.Attach(objSourceImage.ref_objMainImage);

            destinationROI.SetSize(searchROI.Width, searchROI.Height);
            destinationROI.OrgX = searchROI.OrgX;
            destinationROI.OrgY = searchROI.OrgY;

            if (destinationROI.OrgX + destinationROI.Width >= objRotatedImage.ref_objMainImage.Width)
            {
                searchROI.Dispose();
                destinationROI.Dispose();

                return;
            }

            if (destinationROI.OrgY + destinationROI.Height >= objRotatedImage.ref_objMainImage.Height)
            {
                searchROI.Dispose();
                destinationROI.Dispose();

                return;
            }

            destinationROI.Attach(objRotatedImage.ref_objMainImage);

            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f - 0.5f, searchROI.Height / 2f - 0.5f, destinationROI.Width / 2f - 0.5f, destinationROI.Height / 2f - 0.5f, 1, 1, fRotateAngle, destinationROI, intInterpolation);
            searchROI.Dispose();
            destinationROI.Dispose();
        }

        /// <summary>
        /// Rotate image 
        /// </summary>
        /// <param name="objSearchROI">Search ROI for rotation area</param>
        /// <param name="fRotateAngle">Rotation angle</param>
        /// <param name="objRotatedImage">Image used for rotation.(Source image and destination image are same object)</param>
        public static void RotateROI(ROI objSearchROI, float fRotateAngle, ref ImageDrawing objRotatedImage, int intInterpolation)
        {
            /*
             * Source image and rotated image are same object.
             */

            EROIBW8 searchROI = new EROIBW8();
            EROIBW8 destinationROI = new EROIBW8();

            ImageDrawing objSourceImage = new ImageDrawing(true);
            objRotatedImage.CopyTo(ref objSourceImage);        //Copy source to new object to prevent rotation image blur

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROIPositionX;
            searchROI.OrgY = objSearchROI.ref_ROIPositionY;
            searchROI.Attach(objSourceImage.ref_objMainImage);

            if ((fRotateAngle >= 90.0f && fRotateAngle < 180.0f) || (fRotateAngle >= 270.0f && fRotateAngle < 360.0f))
                destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Height, searchROI.Width);
            else
                destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Width, searchROI.Height);

            if (destinationROI.OrgX + destinationROI.Width >= objRotatedImage.ref_objMainImage.Width)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                objSourceImage.Dispose();
                objSourceImage = null;
                return;
            }

            if (destinationROI.OrgY + destinationROI.Height >= objRotatedImage.ref_objMainImage.Height)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                objSourceImage.Dispose();
                objSourceImage = null;
                return;
            }

            destinationROI.Attach(objRotatedImage.ref_objMainImage);

            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f - 0.5f, searchROI.Height / 2f - 0.5f, destinationROI.Width / 2f - 0.5f, destinationROI.Height / 2f - 0.5f, 1, 1, fRotateAngle, destinationROI, intInterpolation);

            searchROI.Dispose();
            destinationROI.Dispose();
            searchROI = null;
            destinationROI = null;
            objSourceImage.Dispose();
            objSourceImage = null;
        }
        /// <summary>
        /// Rotate image
        /// </summary>
        /// <param name="objSourceImage">Source image (Make sure source image and destination image are not same object).</param>
        /// <param name="objSearchROI">Search ROI for rotation area</param>
        /// <param name="fRotateAngle">Rotation angle</param>
        /// <param name="objRotatedImage">Destination image</param>
        public static void RotateROI(ImageDrawing objSourceImage, ROI objSearchROI, float fRotateAngle, ref ImageDrawing objRotatedImage)
        {
            /*
             * Source image and rotated image are not same object.
             */

            EROIBW8 searchROI = new EROIBW8();
            EROIBW8 destinationROI = new EROIBW8();

            objSourceImage.CopyTo(ref objRotatedImage);            //Copy source to rotation image to make sure non-ROI area is not black color.

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROIPositionX;
            searchROI.OrgY = objSearchROI.ref_ROIPositionY;
            searchROI.Attach(objSourceImage.ref_objMainImage);

            if ((fRotateAngle >= 90.0f && fRotateAngle < 180.0f) || (fRotateAngle >= 270.0f && fRotateAngle < 360.0f))
                destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Height, searchROI.Width);
            else
                destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Width, searchROI.Height);

            if (destinationROI.OrgX + destinationROI.Width >= objRotatedImage.ref_objMainImage.Width)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                return;
            }

            if (destinationROI.OrgY + destinationROI.Height >= objRotatedImage.ref_objMainImage.Height)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                return;
            }

            destinationROI.Attach(objRotatedImage.ref_objMainImage);

            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f - 0.5f, searchROI.Height / 2f - 0.5f, destinationROI.Width / 2f - 0.5f, destinationROI.Height / 2f - 0.5f, 1, 1, fRotateAngle, destinationROI, 0);

            searchROI.Dispose();
            destinationROI.Dispose();

        }
        /// <summary>
        /// Rotate image
        /// </summary>
        /// <param name="objSearchROI">Search ROI for rotation area</param>
        /// <param name="fRotateAngle">Rotation angle</param>
        /// <param name="arrRotatedImage">Image used for rotation.(Source image and destination image are same object)</param>
        /// <param name="intImageIndex">Image index</param>
        public static void RotateROI(ROI objSearchROI, float fRotateAngle, ref List<ImageDrawing> arrRotatedImage, int intImageIndex)
        {
            /*
             * Source image and rotated image are same object.
             */

            EROIBW8 searchROI = new EROIBW8();
            EROIBW8 destinationROI = new EROIBW8();

            ImageDrawing objRotatedImage = arrRotatedImage[intImageIndex];
            ImageDrawing objSourceImage = new ImageDrawing(true);
            objRotatedImage.CopyTo(ref objSourceImage);        //Copy source to new object to prevent rotation image blur

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROIPositionX;
            searchROI.OrgY = objSearchROI.ref_ROIPositionY;
            searchROI.Attach(objSourceImage.ref_objMainImage);

            if ((fRotateAngle >= 90.0f && fRotateAngle < 180.0f) || (fRotateAngle >= 270.0f && fRotateAngle < 360.0f))
                destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Height, searchROI.Width);
            else
                destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Width, searchROI.Height);

            if (destinationROI.OrgX + destinationROI.Width >= objRotatedImage.ref_objMainImage.Width)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                objSourceImage.Dispose();
                objSourceImage = null;
                return;
            }

            if (destinationROI.OrgY + destinationROI.Height >= objRotatedImage.ref_objMainImage.Height)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                objSourceImage.Dispose();
                objSourceImage = null;
                return;
            }

            destinationROI.Attach(objRotatedImage.ref_objMainImage);

            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f - 0.5f, searchROI.Height / 2f - 0.5f, destinationROI.Width / 2f - 0.5f, destinationROI.Height / 2f - 0.5f, 1, 1, fRotateAngle, destinationROI, 0);
            //objSourceImage.Dispose();
            //searchROI.Dispose();
            //destinationROI.Dispose();

            searchROI.Dispose();
            destinationROI.Dispose();
            searchROI = null;
            destinationROI = null;
            objSourceImage.Dispose();
            objSourceImage = null;
        }
        /// <summary>
        /// Rotate within center point
        /// </summary>
        /// <param name="objSearchROI"></param>
        /// <param name="fRotateAngle"></param>
        /// <param name="arrRotatedImage"></param>
        /// <param name="intImageIndex"></param>
        public static void RotateROI_Center(ROI objSearchROI, float fRotateAngle, ref List<ImageDrawing> arrRotatedImage, int intImageIndex)
        {
            /*
             * Source image and rotated image are same object.
             */

            EROIBW8 searchROI = new EROIBW8();
            EROIBW8 destinationROI = new EROIBW8();

            ImageDrawing objRotatedImage = arrRotatedImage[intImageIndex];
            ImageDrawing objSourceImage = new ImageDrawing(true);
            objRotatedImage.CopyTo(ref objSourceImage);        //Copy source to new object to prevent rotation image blur

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROIPositionX;
            searchROI.OrgY = objSearchROI.ref_ROIPositionY;
            searchROI.Attach(objSourceImage.ref_objMainImage);
            int intMinDimension = Math.Min(searchROI.Height, searchROI.Width);
            if ((fRotateAngle >= 90.0f && fRotateAngle < 180.0f) || (fRotateAngle >= 270.0f && fRotateAngle < 360.0f))
                destinationROI.SetPlacement((int)(searchROI.OrgX + (searchROI.Width / 2f) - (intMinDimension / 2f)), (int)(searchROI.OrgY + (searchROI.Height / 2f) - (intMinDimension / 2f)), intMinDimension, intMinDimension);
            else
                destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Width, searchROI.Height);

            if (destinationROI.OrgX + destinationROI.Width >= objRotatedImage.ref_objMainImage.Width)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                objSourceImage.Dispose();
                objSourceImage = null;
                return;
            }

            if (destinationROI.OrgY + destinationROI.Height >= objRotatedImage.ref_objMainImage.Height)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                objSourceImage.Dispose();
                objSourceImage = null;
                return;
            }

            destinationROI.Attach(objRotatedImage.ref_objMainImage);

            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f - 0.5f, searchROI.Height / 2f - 0.5f, destinationROI.Width / 2f - 0.5f, destinationROI.Height / 2f - 0.5f, 1, 1, fRotateAngle, destinationROI, 0);
            //objSourceImage.Dispose();
            //searchROI.Dispose();
            //destinationROI.Dispose();

            searchROI.Dispose();
            destinationROI.Dispose();
            searchROI = null;
            destinationROI = null;
            objSourceImage.Dispose();
            objSourceImage = null;
        }
        /// <summary>
        /// Rotate image
        /// </summary>
        /// <param name="objSourceImage">Source image (Make sure source image and destination image are not same object).</param>
        /// <param name="objSearchROI">Search ROI for rotation area</param>
        /// <param name="fRotateAngle">Rotation angle</param>
        /// <param name="arrRotatedImage">List Destination image</param>
        /// <param name="intImageIndex">Destination Image index</param>
        public static void RotateROI(ImageDrawing objSourceImage, ROI objSearchROI, float fRotateAngle, ref List<ImageDrawing> arrRotatedImage, int intImageIndex)
        {
            /*
             * Source image and rotated image are not same object.
             */

            EROIBW8 searchROI = new EROIBW8();
            EROIBW8 destinationROI = new EROIBW8();

            ImageDrawing objRotatedImage = arrRotatedImage[intImageIndex];
            objSourceImage.CopyTo(ref objRotatedImage);            //Copy source to rotation image to make sure non-ROI area is not black color.

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = objSearchROI.ref_ROIPositionX;
            searchROI.OrgY = objSearchROI.ref_ROIPositionY;
            searchROI.Attach(objSourceImage.ref_objMainImage);

            if ((fRotateAngle >= 90.0f && fRotateAngle < 180.0f) || (fRotateAngle >= 270.0f && fRotateAngle < 360.0f))
                destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Height, searchROI.Width);
            else
                destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Width, searchROI.Height);

            if (destinationROI.OrgX + destinationROI.Width >= objRotatedImage.ref_objMainImage.Width)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                return;
            }

            if (destinationROI.OrgY + destinationROI.Height >= objRotatedImage.ref_objMainImage.Height)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                return;
            }

            destinationROI.Attach(objRotatedImage.ref_objMainImage);

            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f - 0.5f, searchROI.Height / 2f - 0.5f, destinationROI.Width / 2f - 0.5f, destinationROI.Height / 2f - 0.5f, 1, 1, fRotateAngle, destinationROI, 0);

            searchROI.Dispose();
            destinationROI.Dispose();

        }

        public static bool MaxROI(ROI objROI1, ROI objROI2, ref ROI objDestinationROI)
        {
            if (objROI1.ref_ROIWidth != objROI2.ref_ROIWidth || objROI1.ref_ROIHeight != objROI2.ref_ROIHeight)
                return false;

            try
            {
                EasyImage.Oper(EArithmeticLogicOperation.Max, objROI1.ref_ROI, objROI2.ref_ROI, objDestinationROI.ref_ROI);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get whole roi area pixel gray value
        /// </summary>
        /// <returns>pixel average</returns>
        public float GetROIAreaPixel()
        {
            float fPixelAverage = 0.0f;

            try
            {
                EROIBW8 objArea = new EROIBW8();
                objArea.SetPlacement((m_ROI.Width / 4), (m_ROI.Height / 4), m_ROI.Width / 2, m_ROI.Height / 2);
                objArea.Attach(m_ROI);

                EasyImage.PixelAverage(objArea, out fPixelAverage);
            }
            catch (Exception ex)
            {
                string a = ex.ToString();
            }
            return fPixelAverage;
        }

        /// <summary>
        /// Draw frame label
        /// </summary>
        /// <param name="intColorNo">color</param>
        /// <param name="g">window destination to put the drawing up</param>
        private void DrawString(int intColorNo, Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            int intTextPositionX = 0, intTestPositionY = 0;

            if (m_intType >= 2)
            {
                intTextPositionX = m_ROI.TotalOrgX;
                intTestPositionY = m_ROI.TotalOrgY;
                g.DrawString(m_strROIName, m_Font, new SolidBrush(m_Color[intColorNo]), intTextPositionX * fDrawingScaleX, intTestPositionY * fDrawingScaleY);
            }
            else
                g.DrawString(m_strROIName, m_Font, new SolidBrush(m_Color[intColorNo]), m_ROI.OrgX * fDrawingScaleX, m_ROI.OrgY * fDrawingScaleY);
        }
        private void DrawString(int intColorNo, Graphics g, float fDrawingScaleX, float fDrawingScaleY, Color objColor)
        {
            int intTextPositionX = 0, intTestPositionY = 0;

            if (m_intType >= 2)
            {
                intTextPositionX = m_ROI.TotalOrgX;
                intTestPositionY = m_ROI.TotalOrgY;
                g.DrawString(m_strROIName, m_Font, new SolidBrush(objColor), intTextPositionX * fDrawingScaleX, intTestPositionY * fDrawingScaleY);
            }
            else
                g.DrawString(m_strROIName, m_Font, new SolidBrush(objColor), m_ROI.OrgX * fDrawingScaleX, m_ROI.OrgY * fDrawingScaleY);
        }
        /// <summary>
        /// Draw frame label
        /// </summary>
        /// <param name="intColorNo">color</param>
        /// <param name="fScale">scale</param>
        /// <param name="g">window destination to put the drawing up</param>
        /// <param name="fPositionX">position X</param>
        /// <param name="fPositionY">position Y</param>
        private void DrawString(int intColorNo, Graphics g, float fDrawingScaleX, float fDrawingScaleY, float fPositionX, float fPositionY)
        {
            float fTextPositionX = 0, fTestPositionY = 0;
            if (m_intType == 2 && m_ROI.Parent != null)
            {
                fTextPositionX = m_ROI.Parent.OrgX + m_ROI.OrgX + fPositionX;
                fTestPositionY = m_ROI.Parent.OrgY + m_ROI.OrgY + fPositionY;
                g.DrawString(m_strROIName, m_Font, new SolidBrush(m_Color[intColorNo]), fTextPositionX * fDrawingScaleX, fTestPositionY * fDrawingScaleY);
            }
            else
                g.DrawString(m_strROIName, m_Font, new SolidBrush(m_Color[intColorNo]), (m_ROI.OrgX + fPositionX) * fDrawingScaleX, (m_ROI.OrgY + fPositionY) * fDrawingScaleY);
        }
        private void DrawString(int intColorNo, Graphics g, float fDrawingScaleX, float fDrawingScaleY, float fPositionX, float fPositionY, int intROINumber, Color objColor)
        {
            float fTextPositionX = 0, fTestPositionY = 0;
            if (m_intType == 2 && m_ROI.Parent != null)
            {
                fTextPositionX = m_ROI.Parent.OrgX + m_ROI.OrgX + fPositionX;
                fTestPositionY = m_ROI.Parent.OrgY + m_ROI.OrgY + fPositionY;
                g.DrawString(m_strROIName, m_Font, new SolidBrush(objColor), fTextPositionX * fDrawingScaleX, fTestPositionY * fDrawingScaleY);
            }
            else
            {
                if (intROINumber > 0 && m_ROI.Parent != null)
                {
                    fTextPositionX = m_ROI.Parent.OrgX + m_ROI.OrgX + fPositionX;
                    fTestPositionY = m_ROI.Parent.OrgY + m_ROI.OrgY + fPositionY;
                    g.DrawString(m_strROIName, m_Font, new SolidBrush(objColor), fTextPositionX * fDrawingScaleX, fTestPositionY * fDrawingScaleY);
                }
                else
                {
                    g.DrawString(m_strROIName, m_Font, new SolidBrush(objColor), (m_ROI.OrgX + fPositionX) * fDrawingScaleX, (m_ROI.OrgY + fPositionY) * fDrawingScaleY);
                }
            }
        }
        private void DrawString(int intColorNo, Graphics g, float fDrawingScaleX, float fDrawingScaleY, float fPositionX, float fPositionY, Color ObjColor)
        {
            float fTextPositionX = 0, fTestPositionY = 0;
            if (m_intType == 2 && m_ROI.Parent != null)
            {
                fTextPositionX = m_ROI.Parent.OrgX + m_ROI.OrgX + fPositionX;
                fTestPositionY = m_ROI.Parent.OrgY + m_ROI.OrgY + fPositionY;
                g.DrawString(m_strROIName, m_Font, new SolidBrush(ObjColor), fTextPositionX * fDrawingScaleX, fTestPositionY * fDrawingScaleY);
            }
            else
                g.DrawString(m_strROIName, m_Font, new SolidBrush(ObjColor), (m_ROI.OrgX + fPositionX) * fDrawingScaleX, (m_ROI.OrgY + fPositionY) * fDrawingScaleY);
        }

        /// <summary>
        /// If the specific directory does not exist, create a new one
        /// </summary>
        private void CreateUnexistDirectory(DirectoryInfo directory)
        {
            if (!directory.Parent.Exists)
            {
                CreateUnexistDirectory(directory.Parent);
            }

            Directory.CreateDirectory(directory.FullName);

        }

        public void SetPixel(int intPosX, int intPosY, EBW8 bw8PixelValue)
        {

            m_ROI.SetPixel(bw8PixelValue, intPosX, intPosY);
        }

        public static float CalculateAveragePixelGrayValue(ROI objROI, int intThreshold, AreaColor blnAreaColor)
        {
            EBW8 px = new EBW8();
            float fTotalValue = 0;
            int intTotalCount = 0;

            EROIBW8 objROIBW8 = objROI.ref_ROI;

            int intWidth = objROI.ref_ROI.Width;
            int intHeight = objROI.ref_ROI.Height;

            if (blnAreaColor == AreaColor.Black)
            {
                for (int x = 0; x < intWidth; x++)
                {
                    for (int y = 0; y < intHeight; y++)
                    {
                        px = objROIBW8.GetPixel(x, y);

                        if (px.Value < intThreshold)
                        {
                            fTotalValue += px.Value;
                            intTotalCount++;
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < objROI.ref_ROI.Width; x++)
                {
                    for (int y = 0; y < objROI.ref_ROI.Height; y++)
                    {
                        px = objROI.ref_ROI.GetPixel(x, y);

                        if (px.Value >= intThreshold)
                        {
                            fTotalValue += px.Value;
                            intTotalCount++;
                        }
                    }
                }
            }

            if (intTotalCount == 0)
                return 0;
            else
                return fTotalValue / intTotalCount;
        }

        public void Dispose()
        {
            m_Font.Dispose();

            if (m_ROI != null)
                m_ROI.Dispose();

            if (m_objParentImage != null)
                m_objParentImage.Dispose();

            if (m_objZoomImage != null)
                m_objZoomImage.Dispose();

            if (m_objDisplayImage != null)
                m_objDisplayImage.Dispose();
        }
        public void DrawBorderLine(bool blnWhiteOnBlack)
        {
            if (blnWhiteOnBlack)
            {
                m_bw.Value = 0; //Black color
            }
            else
            {
                m_bw.Value = 255;
            }

            int intWidth = m_ROI.Width - 1;
            int intHeight = m_ROI.Height - 1;
            for (int x = 0; x <= intWidth; x++)
            {
                m_ROI.SetPixel(m_bw, x, 0);
                m_ROI.SetPixel(m_bw, x, intHeight);
            }

            for (int y = 0; y <= intHeight; y++)
            {
                m_ROI.SetPixel(m_bw, 0, y);
                m_ROI.SetPixel(m_bw, intWidth, y);
            }
        }
        // --------------------------------------------- New Static ROI --------------------------------------------------------------------------------------------
        // --------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void AttachROI(ref EROIBW8 objROI, EImageBW8 objImage)
        {
            objROI.Detach();
            if (objROI.OrgX > objImage.Width)
                objROI.OrgX = 0;
            if (objROI.OrgY > objImage.Height)
                objROI.OrgY = 0;

            if ((objROI.OrgX + objROI.Width) > objImage.Width)
                objROI.Width = objImage.Width - objROI.OrgX;

            if ((objROI.OrgY + objROI.Height) > objImage.Height)
                objROI.Height = objImage.Height - objROI.OrgY;

            objROI.Attach(objImage);
        }

        public void DrawDontCareAreaROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, bool blnHandler, string strText, int intType, int intROINumber)
        {
            try
            {
                if (m_ROI.Parent == null)
                    return;

                if (m_strROIName != strText)
                    m_strROIName = strText;
                if (m_intType != intType)
                    m_intType = intType;

                if (m_strROIName == "Don't Care ROI")
                    m_Color[intROINumber] = Color.Lime;

                if (m_objRGBColor.Red != m_Color[intROINumber].R)
                    m_objRGBColor.Red = m_Color[intROINumber].R;
                if (m_objRGBColor.Green != m_Color[intROINumber].G)
                    m_objRGBColor.Green = m_Color[intROINumber].G;
                if (m_objRGBColor.Blue != m_Color[intROINumber].B)
                    m_objRGBColor.Blue = m_Color[intROINumber].B;


                m_ROI.DrawFrame(g, EFramePosition.Outside, blnHandler, fDrawingScaleX, fDrawingScaleY);
                //m_ROI.DrawFrame(g, m_objRGBColor, blnHandler, fDrawingScaleX, fDrawingScaleY);
                //DrawString(intROINumber, g, fDrawingScaleX, fDrawingScaleY);
            }
            catch
            {

            }
        }

        public static bool SubtractROI(ROI objROI1, ROI objROI2)
        {
            try
            {
                EasyImage.Oper(EArithmeticLogicOperation.Subtract, objROI1.ref_ROI, objROI2.ref_ROI, objROI1.ref_ROI);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SubtractROI2(ROI objROI1, ROI objROI2)
        {
            try
            {
                EasyImage.Oper(EArithmeticLogicOperation.Subtract, objROI1.ref_ROI, objROI2.ref_ROI, objROI2.ref_ROI);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool LogicOperationAddROI(ROI objROI1, ROI objROI2)
        {
            try
            {
                EasyImage.Oper(EArithmeticLogicOperation.Add, objROI1.ref_ROI, objROI2.ref_ROI, objROI1.ref_ROI);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool LogicOperationAddROI2(ROI objROI1, ROI objROI2)
        {
            try
            {
                EasyImage.Oper(EArithmeticLogicOperation.Add, objROI1.ref_ROI, objROI2.ref_ROI, objROI2.ref_ROI);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool LogicOperationBitwiseAndROI(ROI objROI1, ROI objROI2)
        {
            try
            {
                EasyImage.Oper(EArithmeticLogicOperation.BitwiseAnd, objROI1.ref_ROI, objROI2.ref_ROI, objROI1.ref_ROI);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool LogicOperationBitwiseAndROI2(ROI objROI1, ROI objROI2)
        {
            try
            {
                EasyImage.Oper(EArithmeticLogicOperation.BitwiseAnd, objROI1.ref_ROI, objROI2.ref_ROI, objROI2.ref_ROI);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool InvertOperationROI(ROI objROI1)
        {
            try
            {
                //ImageDrawing objImage = new ImageDrawing(true, objROI1.ref_ROIWidth, objROI1.ref_ROIHeight);
                //ROI objROI = new ROI();
                //objROI1.CopyToImage(ref objImage);
                //objROI.AttachImage(objImage);
                //objROI.LoadROISetting(objROI1.ref_ROIPositionX, objROI1.ref_ROIPositionY, objROI1.ref_ROIWidth, objROI1.ref_ROIHeight);


                //EasyImage.Oper(EArithmeticLogicOperation.Invert, objROI.ref_ROI, objROI1.ref_ROI);
                //objROI.Dispose();
                //objImage.Dispose();

                EasyImage.Oper(EArithmeticLogicOperation.Invert, objROI1.ref_ROI, objROI1.ref_ROI);
                return true;
            }
            catch
            {
                SRMMessageBox.Show("Invert ROI Fail");
                return false;
            }
        }
        public static void RotateROI_DifferentImageSize(ROI objSearchROI, float fRotateAngle, ref ImageDrawing objRotatedImage, int W, int H)
        {
            /*
             * Source image and rotated image are same object.
             */

            EROIBW8 searchROI = new EROIBW8();
            EROIBW8 destinationROI = new EROIBW8();

            ImageDrawing objSourceImage = new ImageDrawing(true);
            objRotatedImage.CopyTo(ref objSourceImage);        //Copy source to new object to prevent rotation image blur

            searchROI.SetSize(objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);
            searchROI.OrgX = 0;
            searchROI.OrgY = 0;
            searchROI.Detach();
            searchROI.Attach(objSourceImage.ref_objMainImage);

            //if ((fRotateAngle >= 90.0f && fRotateAngle < 180.0f) || (fRotateAngle >= 270.0f && fRotateAngle < 360.0f))
            //    destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Height, searchROI.Width);
            //else
            //    destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Width, searchROI.Height);

            //  if ((fRotateAngle > 85.0f || fRotateAngle < -85.0f))
            //       destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Height, searchROI.Width);
            // else
            destinationROI.SetPlacement(searchROI.OrgX, searchROI.OrgY, searchROI.Width, searchROI.Height);

            if (destinationROI.OrgX + destinationROI.Width >= objRotatedImage.ref_objMainImage.Width)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                objSourceImage.Dispose();
                objSourceImage = null;
                return;
            }

            if (destinationROI.OrgY + destinationROI.Height >= objRotatedImage.ref_objMainImage.Height)
            {
                searchROI.Dispose();
                destinationROI.Dispose();
                searchROI = null;
                destinationROI = null;
                objSourceImage.Dispose();
                objSourceImage = null;
                return;
            }

            destinationROI.Attach(objRotatedImage.ref_objMainImage);

            EasyImage.ScaleRotate(searchROI, searchROI.Width / 2f - 0.5f, searchROI.Height / 2f - 0.5f, destinationROI.Width / 2f - 0.5f, destinationROI.Height / 2f - 0.5f, 1, 1, fRotateAngle, destinationROI, 8);

            searchROI.Dispose();
            destinationROI.Dispose();
            searchROI = null;
            destinationROI = null;
            objSourceImage.Dispose();
            objSourceImage = null;
        }

        public void AddExtraGain(float fGain)
        {
            if (fGain != 1f)
                EasyImage.GainOffset(m_ROI, m_ROI, fGain);
        }

        /// <summary>
        /// For Seal Sprocket Search ROI. Start X and Width is fixed. Mean user can drag Y and height only.
        /// </summary>
        /// <param name="nNewPositionX"></param>
        /// <param name="nNewPositionY"></param>
        /// <param name="intStartX"></param>
        /// <param name="intROIWidth"></param>
        public void DragROI_SealSprocketSearchROI(int nNewPositionX, int nNewPositionY, int intStartX, int intROIWidth, float fScaleX, float fScaleY, float fPanX, float fPanY)
        {
            try
            {
                m_ROI.Drag(m_Handler, nNewPositionX, nNewPositionY, 1.0f, 1.0f, fPanX / fScaleX, fPanY / fScaleY);
                m_ROI.OrgX = intStartX;
                m_ROI.Width = intROIWidth;
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("DragROI: " + ex.ToString());
            }
        }

        public void LoadROISetting_RemoveOut(int nOrgX, int nOrgY, int nWidth, int nHeight)
        {
            if (nOrgX < 0)
                nOrgX = 0;
            if (nOrgY < 0)
                nOrgY = 0;
            if (nWidth < 0)
                nWidth = 1;
            if (nHeight < 0)
                nHeight = 1;

            if (m_ROI.Parent != null)
            {
                try
                {
                    if (nOrgX > m_ROI.Parent.Width)
                        nOrgX = 0;
                    if (nOrgY > m_ROI.Parent.Height)
                        nOrgY = 0;

                    if ((nOrgX + nWidth) > m_ROI.Parent.Width)
                    {
                        //int intOffset = nWidth - ((nOrgX + nWidth) - m_ROI.Parent.Width);     // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when nWidth = 51, nOrgX = 2, and parent.width = 
                        int intOffset = (nOrgX + nWidth) - m_ROI.Parent.Width;
                        nWidth = nWidth - ((nOrgX + nWidth) - m_ROI.Parent.Width);
                    }

                    if ((nOrgY + nHeight) > m_ROI.Parent.Height)
                    {
                        //int intOffset = nHeight - ((nOrgY + nHeight) - m_ROI.Parent.Height);  // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when 
                        int intOffset = (nOrgY + nHeight) - m_ROI.Parent.Height;
                        nHeight = nHeight - ((nOrgY + nHeight) - m_ROI.Parent.Height);
                    }
                }
                catch { }
            }


            if (nWidth < 0)
                nWidth = 0;
            if (nHeight < 0)
                nHeight = 0;

            m_ROI.SetPlacement(nOrgX, nOrgY, nWidth, nHeight);
            m_intOriHeight = nHeight;
            m_intOriWidth = nWidth;
        }

        public static void SetROIPlacement(ref EROIBW8 objROI, int nOrgX, int nOrgY, int nWidth, int nHeight)
        {
            if (nOrgX < 0)
                nOrgX = 0;
            if (nOrgY < 0)
                nOrgY = 0;
            if (nWidth < 0)
                nWidth = 1;
            if (nHeight < 0)
                nHeight = 1;

            if (objROI.Parent != null)
            {
                try
                {
                    if (nOrgX > objROI.Parent.Width)
                        nOrgX = 0;
                    if (nOrgY > objROI.Parent.Height)
                        nOrgY = 0;

                    if ((nOrgX + nWidth) > objROI.Parent.Width)
                    {
                        //int intOffset = nWidth - ((nOrgX + nWidth) - m_ROI.Parent.Width);     // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when nWidth = 51, nOrgX = 2, and parent.width = 
                        int intOffset = (nOrgX + nWidth) - objROI.Parent.Width;
                        if (nOrgX - intOffset >= 0)
                            nOrgX -= intOffset;
                        else
                            nWidth = nWidth - ((nOrgX + nWidth) - objROI.Parent.Width);
                    }

                    if ((nOrgY + nHeight) > objROI.Parent.Height)
                    {
                        //int intOffset = nHeight - ((nOrgY + nHeight) - m_ROI.Parent.Height);  // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when 
                        int intOffset = (nOrgY + nHeight) - objROI.Parent.Height;
                        if (nOrgY - intOffset >= 0)
                            nOrgY -= intOffset;
                        else
                            nHeight = nHeight - ((nOrgY + nHeight) - objROI.Parent.Height);
                    }
                }
                catch { }
            }


            if (nWidth < 0)
                nWidth = 0;
            if (nHeight < 0)
                nHeight = 0;

            objROI.SetPlacement(nOrgX, nOrgY, nWidth, nHeight);
        }

        public static void LoadROISetting(ref EROIBW8 objROI, int nOrgX, int nOrgY, int nWidth, int nHeight)
        {
            if (nOrgX < 0)
                nOrgX = 0;
            if (nOrgY < 0)
                nOrgY = 0;
            if (nWidth < 0)
                nWidth = 1;
            if (nHeight < 0)
                nHeight = 1;

            if (objROI.Parent != null)
            {
                try
                {
                    if (nOrgX > objROI.Parent.Width)
                        nOrgX = 0;
                    if (nOrgY > objROI.Parent.Height)
                        nOrgY = 0;

                    if ((nOrgX + nWidth) > objROI.Parent.Width)
                    {
                        //int intOffset = nWidth - ((nOrgX + nWidth) - m_ROI.Parent.Width);     // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when nWidth = 51, nOrgX = 2, and parent.width = 
                        int intOffset = (nOrgX + nWidth) - objROI.Parent.Width;
                        if (nOrgX - intOffset >= 0)
                            nOrgX -= intOffset;
                        else
                            nWidth = nWidth - ((nOrgX + nWidth) - objROI.Parent.Width);
                    }

                    if ((nOrgY + nHeight) > objROI.Parent.Height)
                    {
                        //int intOffset = nHeight - ((nOrgY + nHeight) - m_ROI.Parent.Height);  // 2020 02 17 - CCENG: Scenario get inoffset = 50 is wrong when 
                        int intOffset = (nOrgY + nHeight) - objROI.Parent.Height;
                        if (nOrgY - intOffset >= 0)
                            nOrgY -= intOffset;
                        else
                            nHeight = nHeight - ((nOrgY + nHeight) - objROI.Parent.Height);
                    }
                }
                catch { }
            }


            if (nWidth < 0)
                nWidth = 0;
            if (nHeight < 0)
                nHeight = 0;

            objROI.SetPlacement(nOrgX, nOrgY, nWidth, nHeight);
        }
        public void AddWhiteCrossX()
        {
            EBW8 px = new EBW8();
            px.Value = 255;

            Line objLine1 = new Line();
            Line objLine2 = new Line();

            objLine1.CalculateLGStraightLine(new Point(0,0), new Point(m_ROI.Width, m_ROI.Height));
            objLine2.CalculateLGStraightLine(new Point(0, m_ROI.Height), new Point(m_ROI.Width, 0));

            for (int x = 0; x < m_ROI.Width; x++)
            {
                m_ROI.SetPixel(px, x, (int)objLine1.GetPointY(x));
                m_ROI.SetPixel(px, x, (int)objLine2.GetPointY(x));
            }
        }
    }
}
