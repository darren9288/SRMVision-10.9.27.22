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
    public class Calibration
    {
        #region Member Variables

        private string m_strErrorMessage = "";

        // Dot Grid 
        private ECodedImage m_objBlobs;
        private TrackLog m_objTrackLog;
        private EWorldShape m_objWorldShape;

        // 5S
        private int m_intSelectedGaugeTool = 0; // 0=Rectangle Gauge, 1=Circle Gauge center & rectangle gauge side, 2=Circle gauge center & line gauge side
        private bool m_blnDrawSamplingPoint = false;
        private bool m_blnDrawDraggingBox = true;
        private int m_intSelectedImage = 0;
        private int m_intROICount = 5;
        private float m_fImageGain;
        private float m_fSizeX;
        private float m_fSizeY;
        private float m_fSizeDiameter;
        private float m_fSizeZ;
        private float m_fCalibrationX;
        private float m_fCalibrationY;
        private float m_fCalibrationZ;
        private CirGauge m_objCirGauge;
        private List<ROI> m_arrROIs;
        private List<RectGauge4L> m_arrRectGauge4L;
        private List<LGauge> m_arrLGauge;
        private int m_intSelectedROIPrev = 0;
        private bool m_blnCursorShapeVerifying = false;

        #endregion

        #region Properties
        public int ref_intSelectedGaugeTool { get { return m_intSelectedGaugeTool; } set { m_intSelectedGaugeTool = value; } }
        public float ref_fPixelX { get { return m_objWorldShape.XResolution; } }
        public float ref_fPixelY { get { return m_objWorldShape.YResolution; } }
        public string ref_strErrorMessage { get { return m_strErrorMessage; } }
        public int ref_intROICount { get { return m_intROICount; } }
        public bool ref_blnDrawSamplingPoint { get { return m_blnDrawSamplingPoint; } set { m_blnDrawSamplingPoint = value; } }
        public bool ref_blnDrawDraggingBox { get { return m_blnDrawDraggingBox; } set { m_blnDrawDraggingBox = value; } }
        public int ref_intSelectedImage { get { return m_intSelectedImage; } set { m_intSelectedImage = value; } }
        public float ref_fImageGain { get { return m_fImageGain; } set { m_fImageGain = value; } }
        public float ref_fSizeX { get { return m_fSizeX; } set { m_fSizeX = value; } }
        public float ref_fSizeY { get { return m_fSizeY; } set { m_fSizeY = value; } }
        public float ref_fSizeZ { get { return m_fSizeZ; } set { m_fSizeZ = value; } }
        public float ref_fCalibrationX { get { return m_fCalibrationX; } }
        public float ref_fCalibrationY { get { return m_fCalibrationY; } }
        public float ref_fSizeDiameter { get { return m_fSizeDiameter; } set { m_fSizeDiameter = value; } }
        public float ref_fCalibrationZ { get { return m_fCalibrationZ; } }
        public CirGauge ref_objCirGauge { get { return m_objCirGauge; } }
        public List<ROI> ref_arrROIs { get { return m_arrROIs; } }
        public List<RectGauge4L> ref_arrRectGauge4L { get { return m_arrRectGauge4L; } }
        public List<LGauge> ref_arrLGauge { get { return m_arrLGauge; } }

        #endregion

        public Calibration()
        {
            m_objBlobs = new ECodedImage();
            m_objTrackLog = new TrackLog();
            m_objWorldShape = new EWorldShape();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="intActivateCalibrationTool">0=Dot Grid, 1=5S, 2=Pad</param>
        public Calibration(int intActivateCalibrationTool, GaugeWorldShape objWorldShape)
        {
            switch (intActivateCalibrationTool)
            {
                case 0:
                    m_objBlobs = new ECodedImage();
                    m_objTrackLog = new TrackLog();
                    m_objWorldShape = new EWorldShape();
                    break;
                case 1:
                    m_arrROIs = new List<ROI>();
                    m_arrRectGauge4L = new List<RectGauge4L>();
                    m_arrLGauge = new List<LGauge>();
                    m_intROICount = 5;
                    m_objCirGauge = new CirGauge(40, 400, objWorldShape);
                    break;
                case 2:
                    m_arrROIs = new List<ROI>();
                    m_arrRectGauge4L = new List<RectGauge4L>();
                    m_intROICount = 1;
                    m_objCirGauge = new CirGauge(40, 400, objWorldShape);
                    break;
            }
        }

        public void AddGaugeROI(ImageDrawing objImage)
        {

            ROI objROI;

            for (int i = 0; i < m_intROICount; i++)
            {
                if (m_arrROIs.Count > i)
                {
                    m_arrROIs[i].AttachImage(objImage);
                    continue;
                }

                if (i == 0)
                {
                    objROI = new ROI("Center ROI", 1);
                    int intPositionX = 640 - (640 / 2) - 80;
                    int intPositionY = (480 / 2) - 80;
                    objROI.LoadROISetting(intPositionX, intPositionY, 170, 170);
                }
                else if (i == 1)
                {
                    objROI = new ROI("T", 1);
                    int intPostX = 640 - (640 / 2) - 65;
                    int intPostY = (480 / 2) - 180;
                    objROI.LoadROISetting(intPostX, intPostY, 150, 50);
                }
                else if (i == 2)
                {
                    objROI = new ROI("R", 1);
                    int intPostXR = 640 - (640 / 2) + 120;
                    int intPostYR = (480 / 2) - 70;
                    objROI.LoadROISetting(intPostXR, intPostYR, 50, 150);
                }
                else if (i == 3)
                {
                    objROI = new ROI("B", 1);
                    int intPostXB = 640 - (640 / 2) - 65;
                    int intPostYB = (480 / 2) + 120;
                    objROI.LoadROISetting(intPostXB, intPostYB, 150, 50);
                }
                else
                {
                    objROI = new ROI("L", 1);
                    int intPostXL = 640 - (640 / 2) - 170;
                    int intPostYL = (480 / 2) - 70;
                    objROI.LoadROISetting(intPostXL, intPostYL, 50, 150);
                }

                objROI.AttachImage(objImage);
                m_arrROIs.Add(objROI);
            }
        }

        public void AddRectGauge4L(GaugeWorldShape objWorldShape)
        {
            m_arrRectGauge4L.Clear();

            for (int i = 0; i < m_arrROIs.Count; i++)
            {
                m_arrRectGauge4L.Add(new RectGauge4L(objWorldShape, 0, -1));
                m_arrRectGauge4L[i].SetGaugePlace(m_arrROIs[i]);
            }
        }

        // 2019 12 28 - JBTAN: Hard coded parameter settings, need to improve
        public void AddLGauge(GaugeWorldShape objWorldShape)
        {
            for (int i = 0; i < m_arrROIs.Count; i++)
            {
                switch (i)
                {
                    case 1:
                        m_arrLGauge.Add(new LGauge(objWorldShape));
                        m_arrLGauge[0].ref_GaugeAngle = 270;
                        m_arrLGauge[0].ref_GaugeFilter = 1;
                        m_arrLGauge[0].ref_GaugeFilterPasses = 3;
                        m_arrLGauge[0].ref_GaugeFilterThreshold = 3;
                        if (m_arrLGauge[0].ref_GaugeTransType > 1)
                            m_arrLGauge[0].ref_GaugeTransType = 1;
                        m_arrLGauge[0].SetGaugePlacement(m_arrROIs[i]);

                        m_arrLGauge.Add(new LGauge(objWorldShape));
                        m_arrLGauge[1].ref_GaugeAngle = 90;
                        m_arrLGauge[1].ref_GaugeFilter = 1;
                        m_arrLGauge[1].ref_GaugeFilterPasses = 3;
                        m_arrLGauge[1].ref_GaugeFilterThreshold = 3;
                        if (m_arrLGauge[1].ref_GaugeTransType > 1)
                            m_arrLGauge[1].ref_GaugeTransType = 1;
                        m_arrLGauge[1].SetGaugePlacement(m_arrROIs[i]);
                        break;
                    case 2:
                        m_arrLGauge.Add(new LGauge(objWorldShape));
                        m_arrLGauge[2].ref_GaugeAngle = 0;
                        m_arrLGauge[2].ref_GaugeFilter = 1;
                        m_arrLGauge[2].ref_GaugeFilterPasses = 3;
                        m_arrLGauge[2].ref_GaugeFilterThreshold = 3;
                        if (m_arrLGauge[2].ref_GaugeTransType > 1)
                            m_arrLGauge[2].ref_GaugeTransType = 1;
                        m_arrLGauge[2].SetGaugePlacement(m_arrROIs[i]);

                        m_arrLGauge.Add(new LGauge(objWorldShape));
                        m_arrLGauge[3].ref_GaugeAngle = 180;
                        m_arrLGauge[3].ref_GaugeFilter = 1;
                        m_arrLGauge[3].ref_GaugeFilterPasses = 3;
                        m_arrLGauge[3].ref_GaugeFilterThreshold = 3;
                        if (m_arrLGauge[3].ref_GaugeTransType > 1)
                            m_arrLGauge[3].ref_GaugeTransType = 1;
                        m_arrLGauge[3].SetGaugePlacement(m_arrROIs[i]);
                        break;
                    case 3:
                        m_arrLGauge.Add(new LGauge(objWorldShape));
                        m_arrLGauge[4].ref_GaugeAngle = 90;
                        m_arrLGauge[4].ref_GaugeFilter = 1;
                        m_arrLGauge[4].ref_GaugeFilterPasses = 3;
                        m_arrLGauge[4].ref_GaugeFilterThreshold = 3;
                        if (m_arrLGauge[4].ref_GaugeTransType > 1)
                            m_arrLGauge[4].ref_GaugeTransType = 1;
                        m_arrLGauge[4].SetGaugePlacement(m_arrROIs[i]);

                        m_arrLGauge.Add(new LGauge(objWorldShape));
                        m_arrLGauge[5].ref_GaugeAngle = 270;
                        m_arrLGauge[5].ref_GaugeFilter = 1;
                        m_arrLGauge[5].ref_GaugeFilterPasses = 3;
                        m_arrLGauge[5].ref_GaugeFilterThreshold = 3;
                        if (m_arrLGauge[5].ref_GaugeTransType > 1)
                            m_arrLGauge[5].ref_GaugeTransType = 1;
                        m_arrLGauge[5].SetGaugePlacement(m_arrROIs[i]);
                        break;
                    case 4:
                        m_arrLGauge.Add(new LGauge(objWorldShape));
                        m_arrLGauge[6].ref_GaugeAngle = 180;
                        m_arrLGauge[6].ref_GaugeFilter = 1;
                        m_arrLGauge[6].ref_GaugeFilterPasses = 3;
                        m_arrLGauge[6].ref_GaugeFilterThreshold = 3;
                        if (m_arrLGauge[6].ref_GaugeTransType > 1)
                            m_arrLGauge[6].ref_GaugeTransType = 1;
                        m_arrLGauge[6].SetGaugePlacement(m_arrROIs[i]);

                        m_arrLGauge.Add(new LGauge(objWorldShape));
                        m_arrLGauge[7].ref_GaugeAngle = 0;
                        m_arrLGauge[7].ref_GaugeFilter = 1;
                        m_arrLGauge[7].ref_GaugeFilterPasses = 3;
                        m_arrLGauge[7].ref_GaugeFilterThreshold = 3;
                        if (m_arrLGauge[7].ref_GaugeTransType > 1)
                            m_arrLGauge[7].ref_GaugeTransType = 1;
                        m_arrLGauge[7].SetGaugePlacement(m_arrROIs[i]);
                        break;
                }
            }
        }

        public void SetRectGauge4LPlace()
        {
            for (int i = 0; i < m_arrRectGauge4L.Count; i++)
            {
                m_arrRectGauge4L[i].SetGaugePlace(m_arrROIs[i]);
            }
        }

        public void SetLGaugePlace()
        {
            for (int i = 0; i < m_arrROIs.Count; i++)
            {
                switch (i)
                {
                    case 1:
                        m_arrLGauge[0].SetGaugePlacement(m_arrROIs[i]);
                        m_arrLGauge[1].SetGaugePlacement(m_arrROIs[i]);
                        break;
                    case 2:
                        m_arrLGauge[2].SetGaugePlacement(m_arrROIs[i]);
                        m_arrLGauge[3].SetGaugePlacement(m_arrROIs[i]);
                        break;
                    case 3:
                        m_arrLGauge[4].SetGaugePlacement(m_arrROIs[i]);
                        m_arrLGauge[5].SetGaugePlacement(m_arrROIs[i]);
                        break;
                    case 4:
                        m_arrLGauge[6].SetGaugePlacement(m_arrROIs[i]);
                        m_arrLGauge[7].SetGaugePlacement(m_arrROIs[i]);
                        break;
                }
            }
        }

        public void SetCircleGaugePlace()
        {
            if (m_arrROIs.Count > 0)
                m_objCirGauge.SetGaugePlacement(m_arrROIs[0]);
        }
        /// <summary>
        /// To know 1 pixel on image represent real mm in world
        /// </summary>
        /// <param name="intThreshold">threshold value</param>
        /// <param name="intMinArea">blob min area</param>
        /// <param name="intMaxArea">blob max area</param>
        /// <param name="fGridX">pitch X</param>
        /// <param name="fGridY">pitch Y</param>
        /// <param name="objROI">ROI</param>
        /// <param name="intMode">calibration mode(any bit combination) 1 = inverse, 2 = scaled, 4 = skewed, 8 = anisotropic, 16 = tilted, 32 = radial</param>
        /// <returns>true = valid calibration, false = invalid calibration</returns>
        public bool Calibrate(int intThreshold, int intMinArea, int intMaxArea, float fGridX, float fGridY, ROI objROI, int intMode)
        {
            // Remove unwanted blobs (too small, too large and against image borders line)
            try
            {
#if (Debug_2_12 || Release_2_12)
                m_objBlobs.SetThreshold((uint)intThreshold);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                m_objBlobs.SetThreshold(intThreshold);
#endif

                m_objBlobs.BlackClass = 1;
                m_objBlobs.WhiteClass = 0;

                m_objBlobs.BuildObjects(objROI.ref_ROI);

                m_objBlobs.AnalyseObjects(ELegacyFeature.Area, ELegacyFeature.GravityCenterX, ELegacyFeature.GravityCenterY);
                m_objBlobs.SelectObjectsUsingFeature(ELegacyFeature.Area, intMinArea, intMaxArea, ESelectOption.RemoveOutOfRange);
                m_objBlobs.SelectObjectsUsingPosition(objROI.ref_ROI, ESelectByPosition.RemoveBorder);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif

            {
                m_strErrorMessage = "Calibration Calibrate : " + ex.ToString();
                m_objTrackLog.WriteLine(m_strErrorMessage);
                return false;
            }

            if (m_objBlobs.NumSelectedObjects == 0)
            {
                m_strErrorMessage = "Calibration Calibrate : Object Not Found";
                return false;
            }
            else
            {
                try
                {
                    m_objWorldShape.EmptyLandmarks();      // Reset the landmark specification sequence

                    EListItem listEasyObject = m_objBlobs.FirstObjPtr;   // Return pointer to the first object of blobs
                    int intObject = m_objBlobs.NumSelectedObjects;
                    if (intObject < 5)
                    {
                        m_strErrorMessage = "Too little dot for reference";
                        return false;
                    }

                    for (int i = 0; i < intObject; i++)
                    {
                        float fCenterX = 0.0f, fCenterY = 0.0f;

                        m_objBlobs.GetObjectFeature(ELegacyFeature.GravityCenterX, listEasyObject, out fCenterX);
                        m_objBlobs.GetObjectFeature(ELegacyFeature.GravityCenterY, listEasyObject, out fCenterY);

                        EPoint pCenter = new EPoint(fCenterX, fCenterY);
                        m_objWorldShape.AddPoint(pCenter);

                        listEasyObject = m_objBlobs.GetNextObjPtr(listEasyObject);
                    }

                    m_objWorldShape.RebuildGrid(fGridX, fGridY);  //Reconstruct the grid topology 
#if (Debug_2_12 || Release_2_12)
                    m_objWorldShape.Calibrate((uint)intMode);            // Scale Mode / calibration mode
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    m_objWorldShape.Calibrate(intMode);            // Scale Mode / calibration mode
#endif

                }
#if (Debug_2_12 || Release_2_12)
                catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                catch (Euresys.Open_eVision_1_2.EException ex)
#endif
                {
                    m_strErrorMessage = "Calibration Calibrate : " + ex.ToString();
                    m_objTrackLog.WriteLine(m_strErrorMessage);
                    return false;
                }
            }

            return true;
        }

        public bool Calibrate_5SRectGauge4L(int intPadMask)
        {
            for (int i = 0; i < m_arrRectGauge4L.Count; i++)
            {
                if ((intPadMask & (0x01 << i)) > 0)
                {
                    if (m_arrRectGauge4L[i].ref_fRectWidth.ToString() == "NaN")
                    {
                        return false;
                    }

                    if (m_arrRectGauge4L[i].ref_fRectHeight.ToString() == "NaN")
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public bool Calibrate_5SCircleGaugeAndRectGauge4L(int intPadMask, ref string strFailMessage, ImageDrawing objImage)
        {
            if (m_intSelectedGaugeTool == 0)
            {
                for (int i = 0; i < m_arrRectGauge4L.Count; i++)
                {
                    if ((intPadMask & (0x01 << i)) > 0)
                    {
                        // 2019 11 04 - CCENG: Return false also if result dimension is 0
                        //if (m_arrRectGauge4L[i].ref_fRectWidth.ToString() == "NaN")
                        if (m_arrRectGauge4L[i].ref_fRectWidth.ToString() == "NaN" || m_arrRectGauge4L[i].ref_fRectWidth.ToString() == "0")
                        {
                            return false;
                        }

                        // 2019 11 04 - CCENG: Return false also if result dimension is 0
                        //if (m_arrRectGauge4L[i].ref_fRectHeight.ToString() == "NaN")
                        if (m_arrRectGauge4L[i].ref_fRectHeight.ToString() == "NaN" || m_arrRectGauge4L[i].ref_fRectHeight.ToString() == "0")
                        {
                            return false;
                        }
                    }
                }
            }
            else if (m_intSelectedGaugeTool == 1)
            {
                for (int i = 0; i < m_arrRectGauge4L.Count; i++)
                {
                    if ((intPadMask & (0x01 << i)) > 0)
                    {
                        if (i == 0)
                        {
                            // 2019 11 04 - CCENG: Return false also if result dimension is 0
                            //if (m_objCirGauge.ref_fDiameter.ToString() == "NaN")
                            if (m_objCirGauge.ref_fDiameter.ToString() == "NaN" || m_objCirGauge.ref_fDiameter.ToString() == "0")
                                return false;

                            if (m_objCirGauge.ref_GaugeScore < 70)
                            {
                                strFailMessage = "Gauge point measurement is poor.";
                                return false;
                            }

                            if (m_objCirGauge.GetPreciseGaugeScore(objImage) < 80)
                            {
                                strFailMessage = "Gauge point measurement is not at result line.";
                                return false;
                            }
                        }
                        else
                        {
                            // 2019 11 04 - CCENG: Return false also if result dimension is 0
                            //if (m_arrRectGauge4L[i].ref_fRectWidth.ToString() == "NaN")
                            if (m_arrRectGauge4L[i].ref_fRectWidth.ToString() == "NaN" || m_arrRectGauge4L[i].ref_fRectWidth.ToString() == "0")
                            {
                                return false;
                            }

                            // 2019 11 04 - CCENG: Return false also if result dimension is 0
                            //if (m_arrRectGauge4L[i].ref_fRectHeight.ToString() == "NaN") 
                            if (m_arrRectGauge4L[i].ref_fRectHeight.ToString() == "NaN" || m_arrRectGauge4L[i].ref_fRectHeight.ToString() == "0")
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            else if (m_intSelectedGaugeTool == 2)
            {
                // 2019 12 28 - JBTAN: Need improvement, for tempory only
                for (int i = 0; i < m_arrROIs.Count; i++)
                {
                    if ((intPadMask & (0x01 << i)) > 0)
                    {
                        if (i == 0)
                        {
                            // 2019 11 04 - CCENG: Return false also if result dimension is 0
                            //if (m_objCirGauge.ref_fDiameter.ToString() == "NaN")
                            if (m_objCirGauge.ref_fDiameter.ToString() == "NaN" || m_objCirGauge.ref_fDiameter.ToString() == "0")
                                return false;

                            if (m_objCirGauge.ref_GaugeScore < 70)
                            {
                                strFailMessage = "Gauge point measurement is poor.";
                                return false;
                            }

                            if (m_objCirGauge.GetPreciseGaugeScore(objImage) < 80)
                            {
                                strFailMessage = "Gauge point measurement is not at result line.";
                                return false;
                            }
                        }
                        else if (i == 1)
                        {
                            if (m_arrLGauge[0].ref_ObjectScore < 50 || m_arrLGauge[1].ref_ObjectScore < 50)
                                return false;
                        }
                        else if (i == 2)
                        {
                            if (m_arrLGauge[2].ref_ObjectScore < 50 || m_arrLGauge[3].ref_ObjectScore < 50)
                                return false;
                        }
                        else if (i == 3)
                        {
                            if (m_arrLGauge[4].ref_ObjectScore < 50 || m_arrLGauge[5].ref_ObjectScore < 50)
                                return false;
                        }
                        else if (i == 4)
                        {
                            if (m_arrLGauge[6].ref_ObjectScore < 50 || m_arrLGauge[7].ref_ObjectScore < 50)
                                return false;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Redraw selected blobs
        /// </summary>
        /// <param name="g">window destination to put the drawing up</param>
        public void Redraw(Graphics g)
        {
            m_objBlobs.DrawObjects(g, new ERGBColor(Color.Blue.R, Color.Blue.G, Color.Blue.B), ESelectionFlag.True);   // draw blue color on selected items only
            m_objWorldShape.DrawGrid(g, new ERGBColor(Color.Lime.R, Color.Lime.G, Color.Lime.B));
            m_objWorldShape.Draw(g, new ERGBColor(Color.Red.R, Color.Red.G, Color.Red.B), EDrawingMode.Nominal);
        }

        public bool MeasureEdge_UsingRectGauge4L(ImageDrawing objImage)
        {
            for (int i = 0; i < m_arrRectGauge4L.Count; i++)
            {
                m_arrRectGauge4L[i].Measure_Pad5SPackage(objImage, false);
            }

            return true;
        }

        public bool MeasureEdge_UsingCircleGaugeAndRectGauge4L(ImageDrawing objImage)
        {
            if (m_intSelectedGaugeTool == 0)
            {
                for (int i = 0; i < m_arrRectGauge4L.Count; i++)
                {
                    m_arrRectGauge4L[i].Measure_Pad5SPackage(objImage, false);
                }
            }
            else if (m_intSelectedGaugeTool == 1)
            {
                for (int i = 0; i < m_arrRectGauge4L.Count; i++)
                {
                    if (i == 0)
                    {
                        m_objCirGauge.Measure(objImage);
                    }
                    else
                    {
                        m_arrRectGauge4L[i].Measure_Pad5SPackage(objImage, false);
                    }
                }
            }
            else if (m_intSelectedGaugeTool == 2)
            {
                // 2019 12 28 - JBTAN: Need improvement, for tempory only
                m_objCirGauge.Measure(objImage);
                if (m_arrLGauge != null)
                {
                    for (int i = 0; i < m_arrLGauge.Count; i++)
                    {
                        m_arrLGauge[i].Measure(objImage);
                    }
                }
            }
            return true;
        }

        public void DrawCalibrationROI(Graphics g, float fScaleX, float fScaleY)
        {
            for (int i = 0; i < m_arrROIs.Count; i++)
            {
                if (m_arrROIs[i].GetROIHandle())
                    m_arrROIs[i].DrawROI(g, fScaleX, fScaleY, true, 0);
                else
                    m_arrROIs[i].DrawROI(g, fScaleX, fScaleY, false, 0);
                if (m_intSelectedGaugeTool == 0)
                {
                    if (i < m_arrRectGauge4L.Count)
                    {
                        if (m_blnDrawSamplingPoint)
                            m_arrRectGauge4L[i].DrawGaugeResult_SamplingPoint(g);
                        if (m_blnDrawDraggingBox)
                            m_arrRectGauge4L[i].DrawGaugeSetting_Inside(g, fScaleX, fScaleY);
                        else
                            m_arrRectGauge4L[i].DrawGaugeResult_ResultLine(g, fScaleX, fScaleX);
                    }
                }
                else if (m_intSelectedGaugeTool == 1)
                {
                    if (i == 0)
                    {
                        m_objCirGauge.DrawCircleGauge(g, fScaleX, fScaleY, m_blnDrawSamplingPoint, m_blnDrawDraggingBox);

                    }
                    else
                    {
                        if (i < m_arrRectGauge4L.Count)
                        {
                            if (m_blnDrawSamplingPoint)
                                m_arrRectGauge4L[i].DrawGaugeResult_SamplingPoint(g);
                            if (m_blnDrawDraggingBox)
                                m_arrRectGauge4L[i].DrawGaugeSetting_Inside(g, fScaleX, fScaleY);
                            else
                                m_arrRectGauge4L[i].DrawGaugeResult_ResultLine(g, fScaleX, fScaleX);
                        }
                    }
                }
                else if (m_intSelectedGaugeTool == 2)
                {
                    // 2019 12 28 - JBTAN: Need improvement, for tempory only
                    if (i == 0)
                    {
                        m_objCirGauge.DrawCircleGauge(g, fScaleX, fScaleY, m_blnDrawSamplingPoint, m_blnDrawDraggingBox);
                    }
                    else if (i == 1)
                    {
                        if (m_blnDrawSamplingPoint)
                        {
                            m_arrLGauge[0].DrawSamplingPointGauge(g);
                            m_arrLGauge[1].DrawSamplingPointGauge(g);
                        }
                        if (m_blnDrawDraggingBox)
                        {
                            m_arrLGauge[0].DrawDraggingBoxGauge(g);
                            m_arrLGauge[1].DrawDraggingBoxGauge(g);
                        }
                        else
                        {
                            m_arrLGauge[0].DrawResultLineGauge(g);
                            m_arrLGauge[1].DrawResultLineGauge(g);
                        }
                    }
                    else if (i == 2)
                    {
                        if (m_blnDrawSamplingPoint)
                        {
                            m_arrLGauge[2].DrawSamplingPointGauge(g);
                            m_arrLGauge[3].DrawSamplingPointGauge(g);
                        }
                        if (m_blnDrawDraggingBox)
                        {
                            m_arrLGauge[2].DrawDraggingBoxGauge(g);
                            m_arrLGauge[3].DrawDraggingBoxGauge(g);
                        }
                        else
                        {
                            m_arrLGauge[2].DrawResultLineGauge(g);
                            m_arrLGauge[3].DrawResultLineGauge(g);
                        }
                    }
                    else if (i == 3)
                    {
                        if (m_blnDrawSamplingPoint)
                        {
                            m_arrLGauge[4].DrawSamplingPointGauge(g);
                            m_arrLGauge[5].DrawSamplingPointGauge(g);
                        }
                        if (m_blnDrawDraggingBox)
                        {
                            m_arrLGauge[4].DrawDraggingBoxGauge(g);
                            m_arrLGauge[5].DrawDraggingBoxGauge(g);
                        }
                        else
                        {
                            m_arrLGauge[4].DrawResultLineGauge(g);
                            m_arrLGauge[5].DrawResultLineGauge(g);
                        }
                    }
                    else if (i == 4)
                    {
                        if (m_blnDrawSamplingPoint)
                        {
                            m_arrLGauge[6].DrawSamplingPointGauge(g);
                            m_arrLGauge[7].DrawSamplingPointGauge(g);
                        }
                        if (m_blnDrawDraggingBox)
                        {
                            m_arrLGauge[6].DrawDraggingBoxGauge(g);
                            m_arrLGauge[7].DrawDraggingBoxGauge(g);
                        }
                        else
                        {
                            m_arrLGauge[6].DrawResultLineGauge(g);
                            m_arrLGauge[7].DrawResultLineGauge(g);
                        }
                    }
                }
            }
        }

        public void VerifyROI(int intPositionX, int intPositionY)
        {
            for (int i = 0; i < m_arrROIs.Count; i++)
            {

                if (m_arrROIs[i].VerifyROIArea(intPositionX, intPositionY))
                {
                    break;
                }

            }
        }

        public void VerifyROIHandleShape(int intPositionX, int intPositionY)
        {
            for (int i = 0; i < m_arrROIs.Count; i++)
            {
                if (m_blnCursorShapeVerifying && m_intSelectedROIPrev != i)
                    continue;

                m_arrROIs[i].VerifyROIHandleShape(intPositionX, intPositionY);

                m_intSelectedROIPrev = i;
                m_blnCursorShapeVerifying = m_arrROIs[i].GetROIHandle2();
            }
        }
        public void DragROI(int intPositionX, int intPositionY, int m_intClickedPad0T, int m_intClickedPad0R, int m_intClickedPad0B, int m_intClickedPad0L, int m_intClickedPad1B, int m_intClickedPad2L, int m_intClickedPad3T, int m_intClickedPad4R)
        {
            int intPositionX0, intPositionX1, intPositionX2, intPositionX3, intPositionX4;
            intPositionX0 = intPositionX1 = intPositionX2 = intPositionX3 = intPositionX4 = intPositionX;
            int intPositionY0, intPositionY1, intPositionY2, intPositionY3, intPositionY4;
            intPositionY0 = intPositionY1 = intPositionY2 = intPositionY3 = intPositionY4 = intPositionY;
            if (m_intSelectedGaugeTool == 0)
            {
                for (int i = 0; i < m_arrROIs.Count; i++)
                {
                    if (m_arrROIs.Count > 1)
                    {
                        if (i == 0)
                        {
                            if (intPositionX0 + m_intClickedPad0R > (m_arrROIs[2].ref_ROITotalX))
                            {
                                intPositionX0 = m_arrROIs[2].ref_ROITotalX - m_intClickedPad0R;
                            }
                            if (intPositionX0 - m_intClickedPad0L < (m_arrROIs[4].ref_ROITotalX + m_arrROIs[4].ref_ROIWidth))
                            {

                                intPositionX0 = (m_arrROIs[4].ref_ROITotalX + m_arrROIs[4].ref_ROIWidth) + m_intClickedPad0L;

                            }
                            if (intPositionY0 + m_intClickedPad0B > (m_arrROIs[3].ref_ROITotalY))
                            {
                                intPositionY0 = m_arrROIs[3].ref_ROITotalY - m_intClickedPad0B;
                            }
                            if (intPositionY0 - m_intClickedPad0T < (m_arrROIs[1].ref_ROITotalY + m_arrROIs[1].ref_ROIHeight))
                            {

                                intPositionY0 = (m_arrROIs[1].ref_ROITotalY + m_arrROIs[1].ref_ROIHeight) + m_intClickedPad0T;

                            }
                        }
                        else if (i == 1)
                        {

                            if (intPositionY1 + m_intClickedPad1B > (m_arrROIs[0].ref_ROITotalY))
                            {
                                intPositionY1 = m_arrROIs[0].ref_ROITotalY - m_intClickedPad1B;
                            }

                        }
                        else if (i == 2)
                        {

                            if (intPositionX2 - m_intClickedPad2L < (m_arrROIs[0].ref_ROITotalX + m_arrROIs[0].ref_ROIWidth))
                            {

                                intPositionX2 = (m_arrROIs[0].ref_ROITotalX + m_arrROIs[0].ref_ROIWidth) + m_intClickedPad2L;

                            }

                        }
                        else if (i == 3)
                        {

                            if (intPositionY3 - m_intClickedPad3T < (m_arrROIs[0].ref_ROITotalY + m_arrROIs[0].ref_ROIHeight))
                            {

                                intPositionY3 = (m_arrROIs[0].ref_ROITotalY + m_arrROIs[0].ref_ROIHeight) + m_intClickedPad3T;

                            }
                        }
                        else if (i == 4)
                        {
                            if (intPositionX4 + m_intClickedPad4R > (m_arrROIs[0].ref_ROITotalX))
                            {
                                intPositionX4 = m_arrROIs[0].ref_ROITotalX - m_intClickedPad4R;
                            }

                        }
                    }

                    if (m_arrROIs[i].GetROIHandle())
                    {
                        if (i == 0)
                            m_arrROIs[i].DragROI(intPositionX0, intPositionY0);
                        else if (i == 1)
                            m_arrROIs[i].DragROI(intPositionX1, intPositionY1);
                        else if (i == 2)
                            m_arrROIs[i].DragROI(intPositionX2, intPositionY2);
                        else if (i == 3)
                            m_arrROIs[i].DragROI(intPositionX3, intPositionY3);
                        else if (i == 4)
                            m_arrROIs[i].DragROI(intPositionX4, intPositionY4);
                        m_arrRectGauge4L[i].SetGaugePlace(m_arrROIs[i]);
                        break;
                    }
                }
            }
            else if (m_intSelectedGaugeTool == 1)
            {
                for (int i = 0; i < m_arrROIs.Count; i++)
                {
                    if (m_arrROIs[i].GetROIHandle())
                    {
                        m_arrROIs[i].DragROI(intPositionX, intPositionY);

                        if (i == 0)
                        {
                            m_objCirGauge.SetGaugePlacement(m_arrROIs[i]);
                        }
                        else
                        {
                            m_arrRectGauge4L[i].SetGaugePlace(m_arrROIs[i]);
                        }
                        break;
                    }
                }
            }
            else if (m_intSelectedGaugeTool == 2)
            {
                for (int i = 0; i < m_arrROIs.Count; i++)
                {
                    if (m_arrROIs[i].GetROIHandle())
                    {
                        m_arrROIs[i].DragROI(intPositionX, intPositionY);

                        if (i == 0)
                        {
                            m_objCirGauge.SetGaugePlacement(m_arrROIs[i]);
                        }
                        else if (i == 1)
                        {
                            m_arrLGauge[0].SetGaugePlacement(m_arrROIs[i]);
                            m_arrLGauge[1].SetGaugePlacement(m_arrROIs[i]);
                        }
                        else if (i == 2)
                        {
                            m_arrLGauge[2].SetGaugePlacement(m_arrROIs[i]);
                            m_arrLGauge[3].SetGaugePlacement(m_arrROIs[i]);
                        }
                        else if (i == 3)
                        {
                            m_arrLGauge[4].SetGaugePlacement(m_arrROIs[i]);
                            m_arrLGauge[5].SetGaugePlacement(m_arrROIs[i]);
                        }
                        else if (i == 4)
                        {
                            m_arrLGauge[6].SetGaugePlacement(m_arrROIs[i]);
                            m_arrLGauge[7].SetGaugePlacement(m_arrROIs[i]);
                        }
                        break;
                    }
                }
            }
        }

        public float GetCalibrationX(float fSizeX, int intPadIndex)
        {
            return m_arrRectGauge4L[intPadIndex].ref_fRectWidth / fSizeX;
        }

        public float GetCalibrationY(float fSizeY, int intPadIndex)
        {
            return m_arrRectGauge4L[intPadIndex].ref_fRectHeight / fSizeY;
        }
        public float GetCalibrationDiameter(float fDiameter)
        {
            return m_objCirGauge.ref_fDiameter / fDiameter;
        }
        public float GetCalibrationZ(float fSizeZ)
        {
            if (m_arrRectGauge4L.Count <= 1)
                return 1f;

            return (m_arrRectGauge4L[1].ref_fRectHeight +
                m_arrRectGauge4L[2].ref_fRectWidth +
                m_arrRectGauge4L[3].ref_fRectHeight +
                m_arrRectGauge4L[4].ref_fRectWidth) / 4 / fSizeZ;
        }

        public float GetCalibrationZ_LineGauge(float fSizeZ)
        {
            return (Math.Abs(m_arrLGauge[1].ref_ObjectCenterX - m_arrLGauge[0].ref_ObjectCenterX) +
                Math.Abs(m_arrLGauge[3].ref_ObjectCenterY - m_arrLGauge[2].ref_ObjectCenterY) +
                Math.Abs(m_arrLGauge[4].ref_ObjectCenterX - m_arrLGauge[5].ref_ObjectCenterX) +
                Math.Abs(m_arrLGauge[6].ref_ObjectCenterY - m_arrLGauge[7].ref_ObjectCenterY)) / 4 / fSizeZ;
        }

        public float GetBackX(int intPadIndex)
        {
            return m_arrRectGauge4L[intPadIndex].ref_fRectWidth;
        }

        public float GetBackY(int intPadIndex)
        {
            return m_arrRectGauge4L[intPadIndex].ref_fRectHeight;
        }

        public float GetBackZ()
        {
            if (m_arrRectGauge4L.Count <= 1)
                return 1f;

            return ((m_arrRectGauge4L[1].ref_fRectHeight +
                m_arrRectGauge4L[2].ref_fRectWidth +
                m_arrRectGauge4L[3].ref_fRectHeight +
                m_arrRectGauge4L[4].ref_fRectWidth) / 4);
        }

        public float GetBackZ_LineGauge()
        {
            return (Math.Abs(m_arrLGauge[1].ref_ObjectCenterX - m_arrLGauge[0].ref_ObjectCenterX) +
                Math.Abs(m_arrLGauge[3].ref_ObjectCenterY - m_arrLGauge[2].ref_ObjectCenterY) +
                Math.Abs(m_arrLGauge[4].ref_ObjectCenterX - m_arrLGauge[5].ref_ObjectCenterX) +
                Math.Abs(m_arrLGauge[6].ref_ObjectCenterY - m_arrLGauge[7].ref_ObjectCenterY)) / 4;
        }

        public float GetBackDiameter()
        {
            return m_objCirGauge.ref_fDiameter;
        }

        public int GetSelectedROI()
        {
            for (int i = 0; i < m_arrROIs.Count; i++)
            {
                if (m_arrROIs[i].GetROIHandle())
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
