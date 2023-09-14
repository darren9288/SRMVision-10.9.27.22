using System;
using System.Collections;
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
    public class Blobs
    {
        #region Constant Variables

        //blobs pen properties
        private Color[] m_colorList = { Color.Aqua, Color.Lime, Color.DeepPink, Color.Aquamarine, Color.OrangeRed, Color.Gold, Color.Fuchsia, Color.MediumSpringGreen, Color.Blue, Color.Violet };

        #endregion

        #region Member Variables

        private int m_intCriteria = 7;  // 0x01=Area, 0x02=Gravity CP, 0x04=Limit Width,Hight and CP, 0x08=Contour
        private int m_intMinArea = 0;
        private int m_intMaxArea = 10000;
        private int m_intHoleNumber = 0;
        private List<EBW8PathVector> m_pvContourList = new List<EBW8PathVector>();
        private List<List<int>> m_intPathDirection = new List<List<int>>();
        private Contour m_objContour = new Contour();
        private ERGBColor m_objColor = new ERGBColor(0, 0, 0);
        private ECodedImage m_Blob = new ECodedImage();
        private Object m_Lock = new object();
        private EListItem m_CurrentBlob;
        private EListItem m_DrawingBlob;
        private EListItem m_TemporaryBlob;
        private TrackLog m_objTrackLog = new TrackLog();

        #endregion

        #region Properties

        public EC24 ref_intLowColorThreshold { get { return m_Blob.LowColorThreshold; } set { m_Blob.LowColorThreshold = value; } }
        public EC24 ref_intHighColorThreshold { get { return m_Blob.HighColorThreshold; } set { m_Blob.HighColorThreshold = value; } }
#if (Debug_2_12 || Release_2_12)
        public int ref_intHighThreshold { get { return (int)m_Blob.HighThreshold; } set { m_Blob.HighThreshold = (uint)value; } }
        public int ref_intLowThreshold { get { return (int)m_Blob.LowThreshold; } set { m_Blob.LowThreshold = (uint)value; } }
        public int ref_intThreshold { get { return (int)m_Blob.GetThreshold(); } set { m_Blob.SetThreshold((uint)value); } }
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
        public int ref_intHighThreshold { get { return m_Blob.HighThreshold; } set { m_Blob.HighThreshold = value; } }
        public int ref_intLowThreshold { get { return m_Blob.LowThreshold; } set { m_Blob.LowThreshold = value; } }
        public int ref_intThreshold { get { return m_Blob.GetThreshold(); } set { m_Blob.SetThreshold(value); } }
#endif

        public int ref_intFeature { get { return m_intCriteria ; } set { m_intCriteria = value; } }   // 0x01=Area, 0x02=Gravity CP, 0x04=Limit Width,Hight and CP, 0x08=Contour
        public int ref_intNumSelectedObject { get{ return m_Blob.NumSelectedObjects;} }
        public int ref_intHoleNumber { get { return m_intHoleNumber; } set { m_intHoleNumber = value; } }
        public int ref_intMaxArea { get { return m_intMaxArea; } set { m_intMaxArea = value; } }
        public int ref_intMinArea { get { return m_intMinArea; } set { m_intMinArea = value; } }
        public Contour ref_objContour { get { return m_objContour; } }
        public ECodedImage ref_Blob { get { return m_Blob; } }
        #endregion

        public Blobs()
        {
        }

        /*
        public void LoadBlobs(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName);

            m_Blob.Threshold = objFile.GetValueAsInt("Threshold", -4);
            m_Blob.HighThreshold = objFile.GetValueAsInt("HighThreshold", 125);
            m_Blob.LowThreshold = objFile.GetValueAsInt("LowThreshold", 125);
            m_Blob.BlackClass = (short)objFile.GetValueAsInt("BlackClass", 1);
            m_Blob.WhiteClass = (short)objFile.GetValueAsInt("WhiteClass", 0);
            m_Blob.NeutralClass = (short)objFile.GetValueAsInt("NeutralClass", 0);
            m_Blob.Connexity = (EConnexity)objFile.GetValueAsInt("Connecity", 4);

            m_intHoleNumber = objFile.GetValueAsInt("HoleNumber", -4);
            m_fMinArea = objFile.GetValueAsInt("MinArea", -4);
            m_fMaxArea = objFile.GetValueAsInt("MaxArea", -4);
            m_intCriteria = objFile.GetValueAsInt("Feature", -4);
        }

        public void SaveBlobs(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);

            objFile.WriteElement1Value("Threshold", m_Blob.Threshold, "Threshold Value", true);
            objFile.WriteElement1Value("HighThreshold", m_Blob.HighThreshold, "High Threshold Value", true);
            objFile.WriteElement1Value("LowThreshold", m_Blob.LowThreshold, "Low Threshold Value", true);
            objFile.WriteElement1Value("BlackClass", m_Blob.BlackClass, "Black Class", true);
            objFile.WriteElement1Value("WhiteClass", m_Blob.WhiteClass, "White Class", true);
            objFile.WriteElement1Value("NeutralClass", m_Blob.NeutralClass, "Neutral Class", true);
            objFile.WriteElement1Value("Connecity", (int)m_Blob.Connexity, "Connecity", true);
            
            objFile.WriteElement1Value("HoleNumber", m_intHoleNumber, "Hole Number", true);
            objFile.WriteElement1Value("MinArea", m_fMinArea, "Min Area", true);
            objFile.WriteElement1Value("MaxArea", m_fMaxArea, "Max Area", true);
            objFile.WriteElement1Value("Feature", m_intCriteria, "Select Feature", true);

            objFile.WriteEndElement();
        }*/

        

        /// <summary>
        /// Criteria that will be analyse to be included in selecting objects
        /// </summary>
        /// <param name="intFeature">1 = area, 2 = gravitiy center, 4 = limitcenter and size</param>
        public void AnalyzeObjects(int intFeature)
        {
            m_Blob.AnalyseObjects(ELegacyFeature.ObjectNumber);

            if ((intFeature & 0x01) > 0)
                m_Blob.AnalyseObjects(ELegacyFeature.Area);

            if ((intFeature & 0x02) > 0)
            {
                m_Blob.AnalyseObjects(ELegacyFeature.GravityCenter);
                m_Blob.AnalyseObjects(ELegacyFeature.GravityCenterX);
                m_Blob.AnalyseObjects(ELegacyFeature.GravityCenterY);
            }

            if ((intFeature & 0x04) > 0)
            {
                m_Blob.AnalyseObjects(ELegacyFeature.LimitCenterX);
                m_Blob.AnalyseObjects(ELegacyFeature.LimitCenterY);
                m_Blob.AnalyseObjects(ELegacyFeature.LimitWidth);
                m_Blob.AnalyseObjects(ELegacyFeature.LimitHeight);
            }

            if ((intFeature & 0x08) > 0)
            {
                m_Blob.AnalyseObjects(ELegacyFeature.ContourX);
                m_Blob.AnalyseObjects(ELegacyFeature.ContourY);
            }
        }

        /// <summary>
        /// Build object and filter out the unnecessary object
        /// </summary>
        /// <param name="objROI">parent that contain image</param>
        /// <returns>number of selected objects</returns>
        public int BuildObjects(ROI objROI)
        {
            return BuildObjects(false, objROI);
        }
        /// <summary>
        /// Build object and filter out the unnecessary object
        /// </summary>
        /// <param name="objROI">parent that contain image</param>
        /// <returns>number of selected objects</returns>
        public int BuildObjects(CROI objROI)
        {
            m_Blob.RemoveAllObjects();

            m_Blob.BuildObjects(objROI.ref_CROI);

            if (m_Blob.NumObjects > 0)
            {
                AnalyzeObjects(m_intCriteria);
                SelectObjectsOnAreaFeature();
                SelectObjectsUsingPosition(objROI, ESelectByPosition.RemoveOut);

                SortObjects(1, false);
            }
            else
                return 0;

            return m_Blob.NumSelectedObjects;
        }

        public int BuildObjects(ROI objROI, bool blnRemoveBorder)
        {
            //STTrackLog.WriteLine("BuildObjects - 3621");
            m_Blob.RemoveAllObjects();

            //STTrackLog.WriteLine("BuildObjects - 3622");

            m_Blob.BuildObjects(objROI.ref_ROI);

            //STTrackLog.WriteLine("BuildObjects - 3623");
            if (m_Blob.NumObjects > 0)
            {
                //STTrackLog.WriteLine("BuildObjects - 3624");
                AnalyzeObjects(m_intCriteria);
                //STTrackLog.WriteLine("BuildObjects - 3625");
                SelectObjectsOnAreaFeature();
                //STTrackLog.WriteLine("BuildObjects - 3626");
                if (blnRemoveBorder)
                {
                    //STTrackLog.WriteLine("BuildObjects - 3627");
                    SelectObjectsUsingPosition(objROI, ESelectByPosition.RemoveBorder);

                    //STTrackLog.WriteLine("BuildObjects - 3628");
                }
                else
                {
                    //STTrackLog.WriteLine("BuildObjects - 3629");

                    SelectObjectsUsingPosition(objROI, ESelectByPosition.RemoveOut);

                    //STTrackLog.WriteLine("BuildObjects - 3630");
                }
                SortObjects(1, false);

                //STTrackLog.WriteLine("BuildObjects - 3629");
            }
            else
                return 0;
                   
            return m_Blob.NumSelectedObjects;
        }

        public int BuildObjects_NoSorting(ROI objROI, bool blnRemoveBorder)
        {
            //STTrackLog.WriteLine("BuildObjects - 3621");
            m_Blob.RemoveAllObjects();

            //STTrackLog.WriteLine("BuildObjects - 3622");

            m_Blob.BuildObjects(objROI.ref_ROI);

            //STTrackLog.WriteLine("BuildObjects - 3623");
            if (m_Blob.NumObjects > 0)
            {
                //STTrackLog.WriteLine("BuildObjects - 3624");
                AnalyzeObjects(m_intCriteria);
                //STTrackLog.WriteLine("BuildObjects - 3625");
                SelectObjectsOnAreaFeature();
                //STTrackLog.WriteLine("BuildObjects - 3626");
                if (blnRemoveBorder)
                {
                    //STTrackLog.WriteLine("BuildObjects - 3627");
                    SelectObjectsUsingPosition(objROI, ESelectByPosition.RemoveBorder);

                    //STTrackLog.WriteLine("BuildObjects - 3628");
                }
                else
                {
                    //STTrackLog.WriteLine("BuildObjects - 3629");

                    SelectObjectsUsingPosition(objROI, ESelectByPosition.RemoveOut);

                    //STTrackLog.WriteLine("BuildObjects - 3630");
                }

                //STTrackLog.WriteLine("BuildObjects - 3629");
            }
            else
                return 0;

            return m_Blob.NumSelectedObjects;
        }

        public int BuildObjects(CROI objROI, bool blnRemoveBorder)
        {
            m_Blob.RemoveAllObjects();

            m_Blob.BuildObjects(objROI.ref_CROI);

            if (m_Blob.NumObjects > 0)
            {
                AnalyzeObjects(m_intCriteria);
                SelectObjectsOnAreaFeature();
                if (blnRemoveBorder)
                    SelectObjectsUsingPosition(objROI, ESelectByPosition.RemoveBorder);
                else
                    SelectObjectsUsingPosition(objROI, ESelectByPosition.RemoveOut);

                SortObjects(1, false);
            }
            else
                return 0;

            return m_Blob.NumSelectedObjects;
        }

        public int BuildObjects(ROI objROI, bool blnRemoveBorder, bool blnBuildContour)
        {
            m_Blob.RemoveAllObjects();

            m_Blob.BuildObjects(objROI.ref_ROI);

            if (m_Blob.NumObjects > 0)
            {
                AnalyzeObjects(m_intCriteria);
                SelectObjectsOnAreaFeature();
                if (blnRemoveBorder)
                    SelectObjectsUsingPosition(objROI, ESelectByPosition.RemoveBorder);
                else
                    SelectObjectsUsingPosition(objROI, ESelectByPosition.RemoveOut);

                SortObjects(1, false);

                // Clear contour data each time after build object
                m_objContour.ClearContour();
                if (blnBuildContour)
                {
                    BuildContour(objROI);
                }
            }
            else
                return 0;

            return m_Blob.NumSelectedObjects;
        }

        public int BuildObjects(bool blnAscending, ROI objROI)
        {
            m_Blob.RemoveAllObjects();

            m_Blob.BuildObjects(objROI.ref_ROI);

            if (m_Blob.NumObjects > 0)
            {
                AnalyzeObjects(m_intCriteria);
                SelectObjectsOnAreaFeature();
                SelectObjectsUsingPosition(objROI, ESelectByPosition.RemoveOut);

                SortObjects(1, blnAscending);
            }
            else
                return 0;

            return m_Blob.NumSelectedObjects;
        }

        public int BuildObjectsAndHole(ROI objROI, bool blnRemoveBorder, bool blnBuildContour)
        {
            m_Blob.RemoveAllObjects();

            m_Blob.BuildObjects(objROI.ref_ROI);
            if (m_Blob.NumObjects > 0)
            {
                AnalyzeObjects(m_intCriteria);

                SelectObjectsOnAreaFeature();
                if (blnRemoveBorder)
                    SelectObjectsUsingPosition(objROI, ESelectByPosition.RemoveBorder);
                else
                    SelectObjectsUsingPosition(objROI, ESelectByPosition.RemoveOut);

                m_Blob.BuildHoles();

                if (m_Blob.NumSelectedObjects > 0)
                {
                    AnalyzeObjects(m_intCriteria);

                    SortObjects(1, false);

                    // Clear contour data each time after build object
                    m_objContour.ClearContour();
                    if (blnBuildContour)
                    {
                        BuildContour(objROI);
                    }
                }
            }
            else
                return 0;

            return m_Blob.NumSelectedObjects;
        }

        public void DrawSelectedBlob(Graphics g, float fDrawingScaleX, float fDrawingScaleY, Color objColor, int intObjectNum)
        {
            if (intObjectNum >= m_Blob.NumSelectedObjects)
                return;

            m_CurrentBlob = m_Blob.FirstObjPtr;
            for (int i = 0; i < intObjectNum; i++)
            {
                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                //once current obj pointer loss,reassign
                if (m_Blob.CurrentObjPtr == null)
                {
                    i = 0;
                    m_CurrentBlob = m_Blob.FirstObjPtr;
                }
            }

            if (m_objColor.Red != objColor.R)
                m_objColor.Red = objColor.R;
            if (m_objColor.Green != objColor.G)
                m_objColor.Green = objColor.G;
            if (m_objColor.Blue != objColor.B)
                m_objColor.Blue = objColor.B;

            m_Blob.DrawObject(g, m_objColor, m_CurrentBlob, fDrawingScaleX, fDrawingScaleY);
        }

        public void DrawSelectedBlob(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intColorIndex, int intObjectNum)
        {
            if (intObjectNum >= m_Blob.NumSelectedObjects)
                return;

            intColorIndex %= 10;
            m_CurrentBlob = m_Blob.FirstObjPtr;
            for (int i = 0; i < intObjectNum; i++)
            {
                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                //once current obj pointer loss,reassign
                if (m_Blob.CurrentObjPtr == null)
                {
                    i = 0;
                    m_CurrentBlob = m_Blob.FirstObjPtr;
                }
            }

            if (m_objColor.Red != m_colorList[intColorIndex].R)
                m_objColor.Red = m_colorList[intColorIndex].R;
            if (m_objColor.Green != m_colorList[intColorIndex].G)
                m_objColor.Green = m_colorList[intColorIndex].G;
            if (m_objColor.Blue != m_colorList[intColorIndex].B)
                m_objColor.Blue = m_colorList[intColorIndex].B;

            m_Blob.DrawObject(g, m_objColor, m_CurrentBlob, fDrawingScaleX, fDrawingScaleY);
        }

        /// <summary>
        /// Draw blobs with zoom
        /// </summary>
        /// <param name="g">pic box graphic</param>
        /// <param name="fScale">scale of image</param>
        /// <param name="intZoomImageEdgeX">pan X</param>
        /// <param name="intZoomImageEdgeY">pan Y</param>
        public void DrawSelectedBlobs(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            try
            {

                lock (m_Lock)
                {
                    int colorIndex = 0;
                    int intNumObjects = m_Blob.NumSelectedObjects;

                    m_CurrentBlob = m_Blob.FirstObjPtr;
                    for (int i = 0; i < intNumObjects; i++)
                    {
                        if (!m_Blob.IsHole(m_CurrentBlob))
                        {
                            if (m_objColor.Red != m_colorList[colorIndex].R)
                                m_objColor.Red = m_colorList[colorIndex].R;
                            if (m_objColor.Green != m_colorList[colorIndex].G)
                                m_objColor.Green = m_colorList[colorIndex].G;
                            if (m_objColor.Blue != m_colorList[colorIndex].B)
                                m_objColor.Blue = m_colorList[colorIndex].B;

                            m_Blob.DrawObject(g, m_objColor, m_CurrentBlob, fDrawingScaleX, fDrawingScaleY);
                        }

                        m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                        //once current obj pointer loss,reassign
                        if (m_Blob.CurrentObjPtr == null)
                        {
                            i = 0;
                            m_CurrentBlob = m_Blob.FirstObjPtr;
                        }

                        if (++colorIndex >= 10)
                            colorIndex = 0;
                    }
                }
            }
            catch
            {
            }
        }

        public void DrawSelectedBlobs(ROI objROI, List<int> arrBuildOcvSelectStatus, List<int> arrBuildOcvStartX, List<int> arrBuildOcvStartY, List<int> arrBuildOcvEndX, List<int> arrBuildOcvEndY, string strpath,
                              List<int> arrBuildOcvSelectStatusPrev, List<float> arrCharShiftX, List<float> arrCharShiftY)
        {
            try
            {

                lock (m_Lock)
                {
                    ImageDrawing objImage = new ImageDrawing(true, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height);
                    objImage.SetImageToBlack();
                    //int intTolerance = 2;

                    int intNumObjects = m_Blob.NumSelectedObjects;
                    List<int> arrSkipNo = new List<int>();
                    m_CurrentBlob = m_Blob.FirstObjPtr;
                    for (int i = 0; i < intNumObjects; i++)
                    {
                        if (arrSkipNo.Contains(i))
                            continue;

                        if (!m_Blob.IsHole(m_CurrentBlob))
                        {
                            for (int j = 0; j < arrBuildOcvSelectStatus.Count; j++)
                            {

                                if (arrBuildOcvSelectStatus[j] != 0 && arrBuildOcvSelectStatusPrev[j] != 0)
                                {
                                    float fWidth = 0, fHeight = 0, fCenterX = 0, fCenterY = 0;
                                    GetSelectedListBlobsWidth(ref fWidth);
                                    GetSelectedListBlobsHeight(ref fHeight);
                                    GetSelectedListBlobsLimitCenterX(ref fCenterX);
                                    GetSelectedListBlobsLimitCenterY(ref fCenterY);

                                    fCenterX += objROI.ref_ROITotalX;
                                    fCenterY += objROI.ref_ROITotalY;

                                    float fOCVWidth = arrBuildOcvEndX[j] - arrBuildOcvStartX[j];
                                    float fOCVHeight = arrBuildOcvEndY[j] - arrBuildOcvStartY[j];

                                    if (fCenterX > (arrBuildOcvStartX[j] /*- arrCharShiftX[j]*/) &&
                                        fCenterX < (arrBuildOcvEndX[j] /*+ arrCharShiftX[j]*/) &&
                                        fCenterY > (arrBuildOcvStartY[j] /*- arrCharShiftY[j]*/) &&
                                        fCenterY < (arrBuildOcvEndY[j] /*+ arrCharShiftY[j]*/) //&&
                                                                                               //fWidth <= (fOCVWidth + (arrCharShiftX[j] * 2)) &&
                                                                                               //fWidth >= (fOCVWidth - (arrCharShiftX[j] * 2)) &&
                                                                                               //fHeight <= (fOCVHeight + (arrCharShiftY[j] * 2)) &&
                                                                                               //fHeight >= (fOCVHeight - (arrCharShiftY[j] * 2))
                                        )
                                    {
                                        arrSkipNo.Add(i);
                                        if (m_objColor.Red != Color.White.R)
                                            m_objColor.Red = Color.White.R;
                                        if (m_objColor.Green != Color.White.G)
                                            m_objColor.Green = Color.White.G;
                                        if (m_objColor.Blue != Color.White.B)
                                            m_objColor.Blue = Color.White.B;

                                        IntPtr ptr = Easy.OpenImageGraphicContext(objImage.ref_objMainImage);

                                        m_Blob.DrawObject(ptr, m_objColor, m_CurrentBlob, 1, 1);
                                        Easy.CloseImageGraphicContext(objImage.ref_objMainImage, ptr);
                                        //objImage.SaveImage("D:\\objImage.bmp");
                                        break;
                                    }
                                }
                            }
                        }

                        m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                        //once current obj pointer loss,reassign
                        if (m_Blob.CurrentObjPtr == null)
                        {
                            i = 0;
                            m_CurrentBlob = m_Blob.FirstObjPtr;
                        }

                    }
                    ROI objOCVROI = new ROI();
                    objOCVROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                    objOCVROI.AttachImage(objImage);
                    objOCVROI.SaveImage(strpath);
                    objImage.Dispose();
                    objOCVROI.Dispose();
                }
            }
            catch
            {
            }
        }

        public void SaveLearnIndividualBlobsImage(ROI objROI, int intBuildOcvStartX, int intBuildOcvStartY, int intBuildOcvEndX, int intBuildOcvEndY, string strpath,
                                      List<int> arrBuildOcvStartXOri, List<int> arrBuildOcvStartYOri, List<int> arrBuildOcvEndXOri, List<int> arrBuildOcvEndYOri)
        {
            try
            {
                lock (m_Lock)
                {
                    ImageDrawing objImage = new ImageDrawing(true, objROI.ref_ROI.TopParent.Width, objROI.ref_ROI.TopParent.Height);
                    objImage.SetImageToBlack();

                    int intNumObjects = m_Blob.NumSelectedObjects;
                    List<int> arrSkipNo = new List<int>();
                    m_CurrentBlob = m_Blob.FirstObjPtr;
                    for (int i = 0; i < intNumObjects; i++)
                    {
                        if (arrSkipNo.Contains(i))
                            continue;

                        if (!m_Blob.IsHole(m_CurrentBlob))
                        {
                            int area = 0;
                            GetSelectedListBlobsArea(ref area);
                            float fWidth = 0, fHeight = 0, fCenterX = 0, fCenterY = 0;
                            GetSelectedListBlobsWidth(ref fWidth);
                            GetSelectedListBlobsHeight(ref fHeight);
                            GetSelectedListBlobsLimitCenterX(ref fCenterX);
                            GetSelectedListBlobsLimitCenterY(ref fCenterY);

                            fCenterX += objROI.ref_ROITotalX;
                            fCenterY += objROI.ref_ROITotalY;

                            float fOCVWidth = intBuildOcvEndX - intBuildOcvStartX;
                            float fOCVHeight = intBuildOcvEndY - intBuildOcvStartY;

                            if (fCenterX > (intBuildOcvStartX) &&
                                fCenterX < (intBuildOcvEndX) &&
                                fCenterY > (intBuildOcvStartY) &&
                                fCenterY < (intBuildOcvEndY)
                                )
                            {
                                bool blnMatchOriginalOCV = false;
                                //2021-01-03 ZJYEOH : Only Match Original OCV just can save, to take care merge char
                                for (int k = 0; k < arrBuildOcvStartXOri.Count; k++)
                                {
                                    //if (arrSkipNoOri.Contains(k))
                                    //    continue;

                                    if (fCenterX > (arrBuildOcvStartXOri[k]) &&
                                        fCenterX < (arrBuildOcvEndXOri[k]) &&
                                        fCenterY > (arrBuildOcvStartYOri[k]) &&
                                        fCenterY < (arrBuildOcvEndYOri[k]))
                                    {
                                        //arrSkipNoOri.Add(k);
                                        blnMatchOriginalOCV = true;
                                    }
                                }

                                if (blnMatchOriginalOCV)
                                {
                                    arrSkipNo.Add(i);
                                    if (m_objColor.Red != Color.White.R)
                                        m_objColor.Red = Color.White.R;
                                    if (m_objColor.Green != Color.White.G)
                                        m_objColor.Green = Color.White.G;
                                    if (m_objColor.Blue != Color.White.B)
                                        m_objColor.Blue = Color.White.B;

                                    IntPtr ptr = Easy.OpenImageGraphicContext(objImage.ref_objMainImage);

                                    m_Blob.DrawObject(ptr, m_objColor, m_CurrentBlob, 1, 1);
                                    Easy.CloseImageGraphicContext(objImage.ref_objMainImage, ptr);
                                }
                            }

                        }

                        m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                        //once current obj pointer loss,reassign
                        if (m_Blob.CurrentObjPtr == null)
                        {
                            i = 0;
                            m_CurrentBlob = m_Blob.FirstObjPtr;
                        }

                    }
                    ROI objOCVROI = new ROI();
                    objOCVROI.LoadROISetting(objROI.ref_ROITotalX, objROI.ref_ROITotalY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);
                    objOCVROI.AttachImage(objImage);
                    objOCVROI.SaveImage(strpath);
                    objImage.Dispose();
                    objOCVROI.Dispose();
                }
            }
            catch
            {
            }
        }
        public void DrawSelectedBlobs(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intColorIndex)
        {
            lock (m_Lock)
            {
                intColorIndex = intColorIndex % 10;
                int intNumObjects = m_Blob.NumSelectedObjects;

                try
                {
                    m_CurrentBlob = m_Blob.FirstObjPtr;
                    for (int i = 0; i < intNumObjects; i++)
                    {
                        if (m_objColor.Red != m_colorList[intColorIndex].R)
                            m_objColor.Red = m_colorList[intColorIndex].R;
                        if (m_objColor.Green != m_colorList[intColorIndex].G)
                            m_objColor.Green = m_colorList[intColorIndex].G;
                        if (m_objColor.Blue != m_colorList[intColorIndex].B)
                            m_objColor.Blue = m_colorList[intColorIndex].B;

                        m_Blob.DrawObject(g, m_objColor, m_CurrentBlob, fDrawingScaleX, fDrawingScaleY);
                        m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                        //once current obj pointer loss,reassign
                        if (m_Blob.CurrentObjPtr == null)
                        {
                            i = 0;
                            m_CurrentBlob = m_Blob.FirstObjPtr;
                        }
                    }
                }
                catch(Exception ex)
                {
                    m_objTrackLog.WriteLine("Blobs - DrawSelectedBlobs : " + ex.ToString());
                }
            }
        }
        public void DrawSelectedBlobs(int intMinArea, Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            try
            {
                lock (m_Lock)
                {
                    int intColorIndex = 0;
                    int intNumObjects = m_Blob.NumSelectedObjects;
                    int intArea = 0;

                    m_CurrentBlob = m_Blob.FirstObjPtr;
                    for (int i = 0; i < intNumObjects; i++)
                    {
                        m_Blob.GetObjectFeature(ELegacyFeature.Area, m_CurrentBlob, out intArea);
                        if (intArea < intMinArea)
                        {
                            m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);
                            //once current obj pointer loss,reassign
                            if (m_Blob.CurrentObjPtr == null)
                            {
                                i = 0;
                                m_CurrentBlob = m_Blob.FirstObjPtr;
                            }
                            continue;
                        }

                        if (!m_Blob.IsHole(m_CurrentBlob))
                        {
                            if (m_objColor.Red != m_colorList[intColorIndex].R)
                                m_objColor.Red = m_colorList[intColorIndex].R;
                            if (m_objColor.Green != m_colorList[intColorIndex].G)
                                m_objColor.Green = m_colorList[intColorIndex].G;
                            if (m_objColor.Blue != m_colorList[intColorIndex].B)
                                m_objColor.Blue = m_colorList[intColorIndex].B;

                            m_Blob.DrawObject(g, m_objColor, m_CurrentBlob, fDrawingScaleX, fDrawingScaleY);
                        }

                        m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                        //once current obj pointer loss,reassign
                        if (m_Blob.CurrentObjPtr == null)
                        {
                            i = 0;
                            m_CurrentBlob = m_Blob.FirstObjPtr;
                        }

                        if (++intColorIndex >= 10)
                            intColorIndex= 0;
                    }
                }
            }
            catch
            {
            }
        }
        public void DrawSelectedObjects(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intRoiOrgX, int intRoiOrgY, int intObjectNum)
        {
            if (intObjectNum >= m_Blob.NumSelectedObjects)
                return;

            m_CurrentBlob = m_Blob.FirstObjPtr;
            for (int i = 0; i < intObjectNum; i++)
            {
                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                //once current obj pointer loss,reassign
                if (m_Blob.CurrentObjPtr == null)
                {
                    i = 0;
                    m_CurrentBlob = m_Blob.FirstObjPtr;
                }
            }

            float fLimitCenterX, fLimitCenterY, fLimitWidth, fLimitHeight;
            fLimitCenterX = fLimitCenterY = fLimitWidth = fLimitHeight = 0;
            m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterX, m_CurrentBlob, out fLimitCenterX);
            m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterY, m_CurrentBlob, out fLimitCenterY);
            m_Blob.GetObjectFeature(ELegacyFeature.LimitWidth, m_CurrentBlob, out fLimitWidth);
            m_Blob.GetObjectFeature(ELegacyFeature.LimitHeight, m_CurrentBlob, out fLimitHeight);

            System.Drawing.Point p1 = new System.Drawing.Point(intRoiOrgX + (int)Math.Round((fLimitCenterX - fLimitWidth / 2) * fDrawingScaleX), intRoiOrgY + (int)Math.Round((fLimitCenterY - fLimitHeight / 2) * fDrawingScaleX));
            System.Drawing.Point p2 = new System.Drawing.Point(intRoiOrgX + (int)Math.Round((fLimitCenterX + fLimitWidth / 2) * fDrawingScaleX), intRoiOrgY + (int)Math.Round((fLimitCenterY - fLimitHeight / 2) * fDrawingScaleX));
            System.Drawing.Point p3 = new System.Drawing.Point(intRoiOrgX + (int)Math.Round((fLimitCenterX - fLimitWidth / 2) * fDrawingScaleX), intRoiOrgY + (int)Math.Round((fLimitCenterY + fLimitHeight / 2) * fDrawingScaleX));
            System.Drawing.Point p4 = new System.Drawing.Point(intRoiOrgX + (int)Math.Round((fLimitCenterX + fLimitWidth / 2) * fDrawingScaleX), intRoiOrgY + (int)Math.Round((fLimitCenterY + fLimitHeight / 2) * fDrawingScaleX));
            g.DrawLine(new Pen(Color.Lime), p1, p2);
            g.DrawLine(new Pen(Color.Lime), p2, p4);
            g.DrawLine(new Pen(Color.Lime), p4, p3);
            g.DrawLine(new Pen(Color.Lime), p3, p1);
        }

        public void Dispose()
        {
            lock (m_Lock)
            {

                if (m_Blob != null)
                {
                    m_Blob.Dispose();
                    m_Blob = null;
                }

                if (m_objContour != null)
                {
                    m_objContour.Dispose();
                    m_objContour = null;
                }

                if (m_CurrentBlob != null)
                {
                    m_CurrentBlob.Dispose();
                    m_CurrentBlob = null;
                }

                if (m_DrawingBlob != null)
                {
                    m_DrawingBlob.Dispose();
                    m_DrawingBlob = null;
                }

                if (m_TemporaryBlob != null)
                {
                    m_TemporaryBlob.Dispose();
                    m_TemporaryBlob = null;
                }

                for (int i = 0; i < m_pvContourList.Count; i++)
                {
                    if (m_pvContourList[i] != null)
                    {
                        m_pvContourList[i].Dispose();
                        m_pvContourList[i] = null;
                    }
                }
            }
        }

        public void DisposeListBlob()
        {
            if (m_CurrentBlob != null)
                m_CurrentBlob.Dispose();
        }

        /// <summary>
        /// Get area of particular blobs object
        /// </summary>
        /// <param name="intObjectNum">object no</param>
        /// <returns>area in int</returns>

        public int GetConnexity()
        {
            if (m_Blob.Connexity == EConnexity.Connexity4)
                return 4;
            else
                return 8;
        }

        public void GetBlobsMinMaxArea(ref int intMinArea, ref int intMaxArea)
        {
            int intNumSelectedObject = m_Blob.NumSelectedObjects;

            if (intNumSelectedObject == 0)
                return;

            m_CurrentBlob = m_Blob.FirstObjPtr;
            int intArea = 0;

            m_Blob.GetObjectFeature(ELegacyFeature.Area, m_CurrentBlob, out intArea);
            intMinArea = intMaxArea = intArea;
            m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

            for (int i = 1; i < intNumSelectedObject; i++)
            {
                m_Blob.GetObjectFeature(ELegacyFeature.Area, m_CurrentBlob, out intArea);

                if (intArea < intMinArea)
                    intMinArea = intArea;

                if (intArea > intMaxArea)
                    intMaxArea = intArea;

                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

            }
        }

        public int GetNumberHolePresent(float fMaxHole)
        {
            int intNumberHolePresent = 0;
            int intNumHoles = m_Blob.GetNumHoles(m_CurrentBlob);
            if (intNumHoles > 0)
            {
                EListItem holeBlob = m_Blob.GetFirstHole(m_CurrentBlob);
                for (int i = 0; i < intNumHoles; i++)
                {
                    int intArea = 0;
                    m_Blob.GetObjectFeature(ELegacyFeature.Area, holeBlob, out intArea);
                    if (intArea >= fMaxHole)
                    {
                        intNumberHolePresent++;
                    }

                    holeBlob = m_Blob.GetNextHole(holeBlob);
                }
            }

            return intNumberHolePresent;
        }

        public void GetNumberHolePresent(float fMaxHole, out int intNumberHolePresent, out int intHoleMinArea, out int intHoleMaxArea)
        {
            int intMinArea = -1;
            int intMaxArea = -1;
            int intNumberPresent = 0;
            int intNumHoles = m_Blob.GetNumHoles(m_CurrentBlob);
            if (intNumHoles > 0)
            {
                EListItem holeBlob = m_Blob.GetFirstHole(m_CurrentBlob);
                for (int i = 0; i < intNumHoles; i++)
                {
                    int intArea = 0;
                    m_Blob.GetObjectFeature(ELegacyFeature.Area, holeBlob, out intArea);
                    if (intArea > fMaxHole)
                    {
                        intNumberPresent++;

                        if ((intMinArea == -1) || (intArea < intMinArea))
                        {
                            intMinArea = intArea;
                        }

                        if ((intMaxArea == -1) || (intArea > intMaxArea))
                        {
                            intMaxArea = intArea;
                        }
                    }

                    holeBlob = m_Blob.GetNextHole(holeBlob);
                }
            }

            intNumberHolePresent = intNumberPresent;
            intHoleMinArea = intMinArea;
            intHoleMaxArea = intMaxArea;
        }

        public void GetNumberHolePresent2(float fMaxHole, out int intNumberHolePresent, out int intHoleMinArea, out int intHoleMaxArea, 
            List<int> arrDefectArea, List<float> arrDefectCenterX, List<float> arrDefectCenterY, List<float> arrDefectWidth, List<float> arrDefectHeight)
        {
            int intMinArea = -1;
            int intMaxArea = -1;
            int intNumberPresent = 0;
            int intNumHoles = m_Blob.GetNumHoles(m_CurrentBlob);
            if (intNumHoles > 0)
            {
                int intArea = 0;
                float fCenterX = 0, fCenterY = 0, fWidth = 0, fHeight = 0;
                EListItem holeBlob = m_Blob.GetFirstHole(m_CurrentBlob);
                for (int i = 0; i < intNumHoles; i++)
                {
                    m_Blob.GetObjectFeature(ELegacyFeature.Area, holeBlob, out intArea);
                    m_Blob.GetObjectFeature(ELegacyFeature.LimitWidth, holeBlob, out fWidth);
                    m_Blob.GetObjectFeature(ELegacyFeature.LimitHeight, holeBlob, out fHeight);
                    m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterX, holeBlob, out fCenterX);
                    m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterY, holeBlob, out fCenterY);

                    if (intArea > fMaxHole)
                    {
                        intNumberPresent++;

                        arrDefectArea.Add(intArea);
                        arrDefectCenterX.Add(fCenterX);
                        arrDefectCenterY.Add(fCenterY);
                        arrDefectWidth.Add(fWidth);
                        arrDefectHeight.Add(fHeight);

                        if ((intMinArea == -1) || (intArea < intMinArea))
                        {
                            intMinArea = intArea;
                        }

                        if ((intMaxArea == -1) || (intArea > intMaxArea))
                        {
                            intMaxArea = intArea;
                        }
                    }

                    holeBlob = m_Blob.GetNextHole(holeBlob);
                }
            }

            intNumberHolePresent = intNumberPresent;
            intHoleMinArea = intMinArea;
            intHoleMaxArea = intMaxArea;
        }

        public int GetHoleParentObjectNumber()
        {
            if (m_Blob.IsHole(m_CurrentBlob))
            {
                int intArea = 0;
                m_Blob.GetObjectFeature(ELegacyFeature.Area, m_CurrentBlob, out intArea);
                if (intArea >= 20)
                {
                    EListItem m_SelectedBlob = m_Blob.GetHoleParentObject(m_CurrentBlob);
                    int intObjNumber = -1;
                    m_Blob.GetObjectFeature(ELegacyFeature.ObjectNumber, m_SelectedBlob, out intObjNumber);
                    return intObjNumber;
                }
                else
                {
                    return -1;
                }
            }
            else
                return -1;
        }
        
        public void GetSelectedListBlobsObjectNumber(ref int intObjNumber)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.ObjectNumber, m_CurrentBlob, out intObjNumber);
        }

        public void GetSelectedListBlobsArea(ref int intArea)
        {
            try
            {
                m_Blob.GetObjectFeature(ELegacyFeature.Area, m_CurrentBlob, out intArea);
            }
            catch
            {
                intArea = 0;
            }
        }

        public void GetSelectedListBlobsContourX(ref int intContourX)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.ContourX, m_CurrentBlob, out intContourX);
        }

        public void GetSelectedListBlobsContourY(ref int intContourY)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.ContourY, m_CurrentBlob, out intContourY);
        }

        public void GetSelectedListBlobsGravityCenterX(ref float fGravityCenterX)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.GravityCenterX, m_CurrentBlob, out fGravityCenterX);
        }

        public void GetSelectedListBlobsGravityCenterY(ref float fGravityCenterY)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.GravityCenterY, m_CurrentBlob, out fGravityCenterY);
        }

        public void GetSelectedListBlobsLimitCenterX(ref float fLimitCenterX)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterX, m_CurrentBlob, out fLimitCenterX);
        }

        public void GetSelectedListBlobsLimitCenterY(ref float fLimitCenterY)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterY, m_CurrentBlob, out fLimitCenterY);
        }

        public void GetSelectedListBlobsWidth(ref float fWidth)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitWidth, m_CurrentBlob, out fWidth);
        }

        public void GetSelectedListBlobsHeight(ref float fHeight)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitHeight, m_CurrentBlob, out fHeight);
        }

        public void GetSelectedListBlobsObjectNumber(EListItem objCurrentBlob, out int intObjNumber)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.ObjectNumber, objCurrentBlob, out intObjNumber);
        }

        public void GetSelectedListBlobsArea(EListItem objCurrentBlob, out int intArea)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.Area, objCurrentBlob, out intArea);
        }

        public void GetSelectedListBlobsGravityCenterX(EListItem objCurrentBlob, out float fGravityCenterX)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.GravityCenterX, objCurrentBlob, out fGravityCenterX);
        }

        public void GetSelectedListBlobsGravityCenterY(EListItem objCurrentBlob, out float fGravityCenterY)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.GravityCenterY, objCurrentBlob, out fGravityCenterY);
        }

        public void GetSelectedListBlobsLimitCenterX(EListItem objCurrentBlob, out float fLimitCenterX)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterX, objCurrentBlob, out fLimitCenterX);
        }

        public void GetSelectedListBlobsLimitCenterY(EListItem objCurrentBlob, out float fLimitCenterY)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterY, objCurrentBlob, out fLimitCenterY);
        }

        public void GetSelectedListBlobsWidth(EListItem objCurrentBlob, out float fWidth)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitWidth, objCurrentBlob, out fWidth);
        }

        public void GetSelectedListBlobsHeight(EListItem objCurrentBlob, out float fHeight)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitHeight, objCurrentBlob, out fHeight);
        }

        public void GetSelectedDrawListBlobsGravityCenterX(ref float fGravityCenterX)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.GravityCenterX, m_DrawingBlob, out fGravityCenterX);
        }

        public void GetSelectedDrawListBlobsGravityCenterY(ref float fGravityCenterY)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.GravityCenterY, m_DrawingBlob, out fGravityCenterY);
        }

        public void GetSelectedDrawListBlobsLimitCenterX(ref float fLimitCenterX)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterX, m_DrawingBlob, out fLimitCenterX);
        }

        public void GetSelectedDrawListBlobsLimitCenterY(ref float fLimitCenterY)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterY, m_DrawingBlob, out fLimitCenterY);
        }

        public void GetSelectedDrawListBlobsWidth(ref float fWidth)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitWidth, m_DrawingBlob, out fWidth);
        }

        public void GetSelectedDrawListBlobsHeight(ref float fHeight)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitHeight, m_DrawingBlob, out fHeight);
        }

        public PointF GetSelectedObjectGravityCenter(int intObjectNum)
        {
            float fCenterX = 0;
            float fCenterY = 0;
            if (intObjectNum >= m_Blob.NumSelectedObjects)
                return new PointF(0, 0);

            m_CurrentBlob = m_Blob.FirstObjPtr;
            for (int i = 0; i < intObjectNum; i++)
            {
                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                //once current obj pointer loss,reassign
                if (m_Blob.CurrentObjPtr == null)
                {
                    i = 0;
                    m_CurrentBlob = m_Blob.FirstObjPtr;
                }
            }

            m_Blob.GetObjectFeature(ELegacyFeature.GravityCenterX, m_CurrentBlob, out fCenterX);
            m_Blob.GetObjectFeature(ELegacyFeature.GravityCenterY, m_CurrentBlob, out fCenterY);

            return new PointF(fCenterX, fCenterY);
        }

        public int GetSelectedObjectNumWithMinPostY(int intBlobCount)
        {
            int intMinNo = 0;
            float fMinY = 0.0f;
            float fMin = 0.0f;
            m_CurrentBlob = m_Blob.FirstObjPtr;

            for (int i = 0; i < intBlobCount; i++)
            {
                m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterY, m_CurrentBlob, out fMin);

                if (i == 0)
                    fMinY = fMin;

                if (fMin < fMinY)
                {
                    fMinY = fMin;
                    intMinNo = i;
                }

                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);
            }

            return intMinNo;
        }

        public int GetSelectedObjectNumWithMaxArea()
        {
            int intResult = 0;
            int intObjectNum = 0;
            int intMaxArea = 0;

            m_CurrentBlob = m_Blob.FirstObjPtr;
            for (int i = 0; i < m_Blob.NumSelectedObjects; i++)
            {
                m_Blob.GetObjectFeature(ELegacyFeature.Area, m_CurrentBlob, out intResult);
                if (intResult > intMaxArea)
                {
                    intMaxArea = intResult;
                    intObjectNum = i;
                }

                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);
            }

            return intObjectNum;
        }

        public int GetSelectedObjectNumWithMaxHeight()
        {
            int intMaxNo = 0;
            float fMaxHeight = 0.0f;
            float fMax = 0.0f;
            m_CurrentBlob = m_Blob.FirstObjPtr;

            for (int i = 0; i < m_Blob.NumSelectedObjects; i++)
            {
                m_Blob.GetObjectFeature(ELegacyFeature.LimitHeight, m_CurrentBlob, out fMax);

                if (i == 0)
                    fMaxHeight = fMax;

                if (fMax > fMaxHeight)
                {
                    fMaxHeight = fMax;
                    intMaxNo = i;
                }

                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);
            }

            return intMaxNo;
        }
        public int GetSelectedObjectArea(int intObjectNum)
        {
            int intResult = 0;
            if (intObjectNum >= m_Blob.NumSelectedObjects)
                return 0;

            m_CurrentBlob = m_Blob.FirstObjPtr;
            for (int i = 0; i < intObjectNum; i++)
            {
                 m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                //once current obj pointer loss,reassign
                if (m_Blob.CurrentObjPtr == null)
                {
                    i = 0;
                    m_CurrentBlob = m_Blob.FirstObjPtr;
                }
            }

            m_Blob.GetObjectFeature(ELegacyFeature.Area, m_CurrentBlob, out intResult);

            return intResult;
        }

        public float GetSelectedObjectLimitHeight(int intObjectNum)
        {
            float fResult = 0;
            if (intObjectNum >= m_Blob.NumSelectedObjects)
                return 0;

            m_CurrentBlob = m_Blob.FirstObjPtr;
            for (int i = 0; i < intObjectNum; i++)
            {
                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                //once current obj pointer loss,reassign
                if (m_Blob.CurrentObjPtr == null)
                {
                    i = 0;
                    m_CurrentBlob = m_Blob.FirstObjPtr;
                }
            }

            m_Blob.GetObjectFeature(ELegacyFeature.LimitHeight, m_CurrentBlob, out fResult);

            return fResult;
        }

        public float GetSelectedObjectLimitWidth(int intObjectNum)
        {
            float fResult = 0;
            if (intObjectNum >= m_Blob.NumSelectedObjects)
                return 0;

            m_CurrentBlob = m_Blob.FirstObjPtr;
            for (int i = 0; i < intObjectNum; i++)
            {
                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                //once current obj pointer loss,reassign
                if (m_Blob.CurrentObjPtr == null)
                {
                    i = 0;
                    m_CurrentBlob = m_Blob.FirstObjPtr;
                }
            }

            m_Blob.GetObjectFeature(ELegacyFeature.LimitWidth, m_CurrentBlob, out fResult);

            return fResult;
        }

        public PointF GetSelectedObjectLimitCenter(int intObjectNum)
        {
            float fCenterX = 0;
            float fCenterY = 0;
            if (intObjectNum >= m_Blob.NumSelectedObjects)
                return new PointF(0, 0);

            m_CurrentBlob = m_Blob.FirstObjPtr;
            for (int i = 0; i < intObjectNum; i++)
            {
                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                //once current obj pointer loss,reassign
                if (m_Blob.CurrentObjPtr == null)
                {
                    i = 0;
                    m_CurrentBlob = m_Blob.FirstObjPtr;
                }
            }

            m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterX, m_CurrentBlob, out fCenterX);
            m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterY, m_CurrentBlob, out fCenterY);

            return new PointF(fCenterX, fCenterY);
        }

        public int GetSelectedObjectNumber()
        {
            return m_Blob.NumSelectedObjects;
        }

        /// <summary>
        /// Get selected object features 
        /// </summary>
        /// <param name="intFeature">0x01 = area, 0x02 = gravitiy center, 0x04 = limitcenter, 0x08 = limit size, 0x10 = object no, 0x20 = contour XY</param>
        public string GetSelectedObjectFeatures(int intObjectNum, int intFeature)
        {
            string strFeature = "";
            int intResult = 0;
            float fResult = 0;

            if (intObjectNum >= m_Blob.NumSelectedObjects)
                return "";

            m_CurrentBlob = m_Blob.FirstObjPtr;
            for (int i = 0; i < intObjectNum; i++)
            {
                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);

                //once current obj pointer loss,reassign
                if (m_Blob.CurrentObjPtr == null)
                {
                    i = 0;
                    m_CurrentBlob = m_Blob.FirstObjPtr;
                }
            }

            if ((intFeature & 0x01) > 0)
            {
                m_Blob.GetObjectFeature(ELegacyFeature.Area, m_CurrentBlob, out intResult);
                strFeature += intResult.ToString() + "#";
            }
            if ((intFeature & 0x02) > 0)
            {
                m_Blob.GetObjectFeature(ELegacyFeature.GravityCenterX, m_CurrentBlob, out fResult);
                strFeature += fResult.ToString() + "#";
                m_Blob.GetObjectFeature(ELegacyFeature.GravityCenterY, m_CurrentBlob, out fResult);
                strFeature += fResult.ToString() + "#";
            }
            if ((intFeature & 0x04) > 0)
            {
                m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterX, m_CurrentBlob, out fResult);
                strFeature += fResult.ToString() + "#";
                m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterY, m_CurrentBlob, out fResult);
                strFeature += fResult.ToString() + "#";
            }
            if ((intFeature & 0x08) > 0)
            {
                m_Blob.GetObjectFeature(ELegacyFeature.LimitWidth, m_CurrentBlob, out fResult);
                strFeature += fResult.ToString() + "#";
                m_Blob.GetObjectFeature(ELegacyFeature.LimitHeight, m_CurrentBlob, out fResult);
                strFeature += fResult.ToString() + "#";
            }
            if ((intFeature & 0x10) > 0)
            {
                m_Blob.GetObjectFeature(ELegacyFeature.ObjectNumber, m_CurrentBlob, out intResult);
                strFeature += intResult.ToString() + "#";
            }
            if ((intFeature & 0x20) > 0)
            {
                m_Blob.GetObjectFeature(ELegacyFeature.ContourX, m_CurrentBlob, out intResult);
                strFeature += intResult.ToString() + "#";
                m_Blob.GetObjectFeature(ELegacyFeature.ContourY, m_CurrentBlob, out intResult);
                strFeature += intResult.ToString() + "#";
            }

            return strFeature.Substring(0,strFeature.Length - 1);
        }

        public void GetSelectedTempListBlobsGravityCenterX(ref float fGravityCenterX)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.GravityCenterX, m_TemporaryBlob, out fGravityCenterX);
        }

        public void GetSelectedTempListBlobsGravityCenterY(ref float fGravityCenterY)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.GravityCenterY, m_TemporaryBlob, out fGravityCenterY);
        }

        public void GetSelectedTempListBlobsLimitCenterX(ref float fLimitCenterX)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterX, m_TemporaryBlob, out fLimitCenterX);
        }

        public void GetSelectedTempListBlobsLimitCenterY(ref float fLimitCenterY)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitCenterY, m_TemporaryBlob, out fLimitCenterY);
        }

        public void GetSelectedTempListBlobsWidth(ref float fWidth)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitWidth, m_TemporaryBlob, out fWidth);
        }

        public void GetSelectedTempListBlobsHeight(ref float fHeight)
        {
            m_Blob.GetObjectFeature(ELegacyFeature.LimitHeight, m_TemporaryBlob, out fHeight);
        }

        public bool IsHole()
        {
            return m_Blob.IsHole(m_CurrentBlob);
        }

        public bool IsHole(EListItem objCurrentBlob)
        {
            return m_Blob.IsHole(objCurrentBlob);
        }

        public bool IsHolePresent(float fMaxHole)
        {
            int intNumHoles = m_Blob.GetNumHoles(m_CurrentBlob);
            if (intNumHoles > 0)
            {
                EListItem holeBlob = m_Blob.GetFirstHole(m_CurrentBlob);
                for (int i = 0; i < intNumHoles; i++)
                {
                    int intArea = 0;
                    m_Blob.GetObjectFeature(ELegacyFeature.Area, holeBlob, out intArea);
                    if (intArea >= fMaxHole)
                        return true;

                    holeBlob = m_Blob.GetNextHole(holeBlob);
                }
            }
            return false;
            /*if (m_Blob.GetNumHoles(m_CurrentBlob) > 0)
                return true;
            else
                return false;*/
        }

        public void ResetBlobs()
        {
            m_Blob.UnselectAllObjects();

            if (m_objContour != null)
                m_objContour.Dispose();

            if (m_CurrentBlob != null)
                m_CurrentBlob.Dispose();

            if (m_DrawingBlob != null)
                m_DrawingBlob.Dispose();

            if (m_TemporaryBlob != null)
                m_TemporaryBlob.Dispose();

            for (int i = 0; i < m_pvContourList.Count; i++)
            {
                if (m_pvContourList[i] != null)
                    m_pvContourList[i].Dispose();
            }
        }

        public void SelectObjectsOnAreaFeature()
        {
            m_Blob.SelectObjectsUsingFeature(ELegacyFeature.Area, m_intMinArea, m_intMaxArea, ESelectOption.RemoveOutOfRange);
        }

        public void SelectObjectsUsingPosition(ROI objROI, ESelectByPosition position)
        {
            m_Blob.SelectObjectsUsingPosition(objROI.ref_ROI, position);
        }

        public void SelectObjectsUsingPosition(CROI objROI, ESelectByPosition position)
        {
            m_Blob.SelectObjectsUsingPosition(objROI.ref_CROI, position);
        }


        /// <summary>
        /// Set black, neutral or white class
        /// </summary>
        /// <param name="intClass">0x01 = black class, 0x02 = white class, 0x04 = neutral class</param>
        public void SetDoubleThresholdClassSelection(int intClass)
        {
            short intLevel = 1;

            m_Blob.BlackClass = 0;
            m_Blob.WhiteClass = 0;
            m_Blob.NeutralClass = 0;

            if ((intClass & 0x01) > 0)
            {
                m_Blob.BlackClass = intLevel;
                intLevel++;
            }
            if ((intClass & 0x02) > 0)
            {
                m_Blob.WhiteClass = intLevel;
                intLevel++;
            }
            if ((intClass & 0x04) > 0)
            {
                m_Blob.NeutralClass = intLevel;
                intLevel++;
            }
        }

        public void SetFirstListBlobs()
        {
            if (m_CurrentBlob != null)
                m_CurrentBlob.Dispose();

            m_CurrentBlob = m_Blob.FirstObjPtr;
        }

        public void SetFirstListBlobs(ref EListItem objCurrentBlob)
        {
            objCurrentBlob = m_Blob.FirstObjPtr;
        }

        public void SetFirstDrawListBlobs()
        {
            m_DrawingBlob = m_Blob.FirstObjPtr;
        }

        public void SetFirstTempListBlobs()
        {
            m_TemporaryBlob = m_Blob.FirstObjPtr;
        }

        public void SetListBlobsToNext()
        {
            m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);
        }

        public void SetListBlobsToNext(ref EListItem objCurrentBlob)
        {
            objCurrentBlob = m_Blob.GetNextObjPtr(objCurrentBlob);
        }

        public void SetDrawListBlobsToNext()
        {
            m_DrawingBlob = m_Blob.GetNextObjPtr(m_DrawingBlob);
        }

        public void SetTempListBlobsToNext()
        {
            m_TemporaryBlob = m_Blob.GetNextObjPtr(m_TemporaryBlob);
        }

        public void SetObjectAreaRange(int intMinArea,  int intMaxArea)
        {
            if (m_intMinArea != intMinArea)
                m_intMinArea = intMinArea;
            if (m_intMaxArea != intMaxArea)
                m_intMaxArea = intMaxArea;
        }

        /// <summary>
        /// Set whether it is black on white or white on black
        /// </summary>
        /// <param name="intClass">1 = black on white, 2 = white on black</param>
        public void SetClassSelection(int intClass)
        {
            m_Blob.BlackClass = 0;
            m_Blob.WhiteClass = 0;

            if ((intClass & 0x01) > 0)
                m_Blob.BlackClass = 1;
            if ((intClass & 0x02) > 0)
                m_Blob.WhiteClass = 1;
        }

        /// <summary>
        /// Set the sensitivity of object connection
        /// </summary>
        /// <param name="intValue">4 = more sensitive, 8 = all neighbours need to be disconnect to split 2 object out</param>
        public void SetConnexity(int intValue)
        {
            switch (intValue)
            {
                case 4:
                    if (m_Blob.Connexity != EConnexity.Connexity4)
                        m_Blob.Connexity = EConnexity.Connexity4;
                    break;
                case 8:
                    if (m_Blob.Connexity != EConnexity.Connexity8)
                    m_Blob.Connexity = EConnexity.Connexity8;
                    break;
            }
        }

        /// <summary>
        /// Sort objects by using feature
        /// </summary>
        /// <param name="intFeature">1 = area, 2 = ObjNumber</param>
        public void SortObjects(int intFeature, bool blnAscending)
        {
            //STTrackLog.WriteLine("SortObjects - 1");

            ESortOption sortOption;
            if (blnAscending)
                sortOption = ESortOption.Ascending;
            else
                sortOption = ESortOption.Descending;

            //STTrackLog.WriteLine("SortObjects - 2");
            if ((intFeature & 0x01) > 0)
            {
                //STTrackLog.WriteLine("SortObjects - 3");
                m_Blob.SortObjectsUsingFeature(ELegacyFeature.Area, sortOption);    // 2019 02 12 - weird case where it cause application auto exit in production pc.
            }
            else if ((intFeature & 0x02) > 0)
            {
                //STTrackLog.WriteLine("SortObjects - 4");
                m_Blob.SortObjectsUsingFeature(ELegacyFeature.ObjectNumber, sortOption);
            }

            //STTrackLog.WriteLine("SortObjects - 5");
        }



        private void BuildContour(ROI objROI)
        {

            int intNumSelectedObject = m_Blob.NumSelectedObjects;
            int intContourX = 0;
            int intContourY = 0;
            m_CurrentBlob = m_Blob.FirstObjPtr;
            for (int i = 0; i < intNumSelectedObject; i++)
            {
                // Skip if is hole
                //if (m_Blob.IsHole(m_CurrentBlob))
                //{
                //    m_Blob.GetNextObjPtr(m_CurrentBlob);
                //    continue;
                //}

                m_Blob.GetObjectFeature(ELegacyFeature.ContourX, m_CurrentBlob, out intContourX);
                m_Blob.GetObjectFeature(ELegacyFeature.ContourY, m_CurrentBlob, out intContourY);

                int intConnexity;
                if (m_Blob.Connexity == EConnexity.Connexity4)
                    intConnexity = 4;
                else
                    intConnexity = 8;

                m_objContour.BuildContour(objROI, intContourX, intContourY, (int)m_Blob.GetThreshold(), intConnexity);

                m_CurrentBlob = m_Blob.GetNextObjPtr(m_CurrentBlob);
            }
        }
    }
}
