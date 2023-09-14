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
    public class LGauge
    {
        #region Member Variables
        private bool m_blnReferSubValue = false;
        private ELineGauge m_LineGauge;
        private ELineGauge m_SubLineGauge;
        private Line m_ObjectLine = new Line();
        private bool m_blnExecuted = false;
        private float m_fSubGaugeAngle = 0;
        private ROI m_objLocalROI = new ROI();

        //font properties
        private Font m_FontMatched = new Font("Tahoma", 8);
        private SolidBrush m_BrushMatched = new SolidBrush(Color.GreenYellow);

        #endregion

        #region Properties

        public float ref_GaugeCenterX { get { return m_LineGauge.Center.X; } }
        public float ref_GaugeCenterY { get { return m_LineGauge.Center.Y; } }
        public float ref_GaugeLength { get { return m_LineGauge.Length; } set { m_LineGauge.Length = value; } }
        public float ref_GaugeAngle { get { return m_LineGauge.Angle; } set { m_LineGauge.Angle = value; } }
#if (Debug_2_12 || Release_2_12)
        public int ref_GaugeMinAmplitude { get { return (int)m_LineGauge.MinAmplitude; } set { m_LineGauge.MinAmplitude = (uint)value; } }
        public int ref_GaugeMinArea { get { return (int)m_LineGauge.MinArea; } set { m_LineGauge.MinArea = (uint)value; } }
        public int ref_GaugeFilter { get { return (int)m_LineGauge.Smoothing; } set { m_LineGauge.Smoothing = (uint)value; } }
        public int ref_GaugeThickness { get { return (int)m_LineGauge.Thickness; } set { m_LineGauge.Thickness = (uint)value; } }

        public float ref_GaugeSamplingStep { get { return m_LineGauge.SamplingStep; } set { m_LineGauge.SamplingStep = value; } }
        public int ref_GaugeThreshold { get { return (int)m_LineGauge.Threshold; } set { m_LineGauge.Threshold = (uint)value; } }
        public int ref_GaugeTransType { get { return m_LineGauge.TransitionType.GetHashCode(); } set { m_LineGauge.TransitionType = (ETransitionType)value; } }
        public int ref_GaugeTransChoice { get { return m_LineGauge.TransitionChoice.GetHashCode(); } set { m_LineGauge.TransitionChoice = (ETransitionChoice)value; } }
        public float ref_GaugeTolerance { get { return m_LineGauge.Tolerance; } set { m_LineGauge.Tolerance = value; } }
        public float ref_GaugeFilterThreshold { get { return m_LineGauge.FilteringThreshold; } set { m_LineGauge.FilteringThreshold = value; } }
        public int ref_GaugeFilterPasses { get { return (int)m_LineGauge.NumFilteringPasses; } set { m_LineGauge.NumFilteringPasses = (uint)value; } }

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
        public int ref_GaugeMinAmplitude { get { return m_LineGauge.MinAmplitude; } set { m_LineGauge.MinAmplitude = value; } }
        public int ref_GaugeMinArea { get { return m_LineGauge.MinArea; } set { m_LineGauge.MinArea = value; } }
        public int ref_GaugeFilter { get { return m_LineGauge.Smoothing; } set { m_LineGauge.Smoothing = value; } }
        public int ref_GaugeThickness { get { return m_LineGauge.Thickness; } set { m_LineGauge.Thickness = value; } }

        public float ref_GaugeSamplingStep { get { return m_LineGauge.SamplingStep; } set { m_LineGauge.SamplingStep = value; } }
        public int ref_GaugeThreshold { get { return m_LineGauge.Threshold; } set { m_LineGauge.Threshold = value; } }
        public int ref_GaugeTransType { get { return m_LineGauge.TransitionType.GetHashCode(); } set { m_LineGauge.TransitionType = (ETransitionType)value; } }
        public int ref_GaugeTransChoice { get { return m_LineGauge.TransitionChoice.GetHashCode(); } set { m_LineGauge.TransitionChoice = (ETransitionChoice)value; } }
        public float ref_GaugeTolerance { get { return m_LineGauge.Tolerance; } set { m_LineGauge.Tolerance = value; } }
        public float ref_GaugeFilterThreshold { get { return m_LineGauge.FilteringThreshold; } set { m_LineGauge.FilteringThreshold = value; } }
        public int ref_GaugeFilterPasses { get { return m_LineGauge.NumFilteringPasses; } set { m_LineGauge.NumFilteringPasses = value; } }

#endif

        // result
        public float ref_ObjectCenterX { get { return m_LineGauge.MeasuredLine.CenterX; } }
        public float ref_ObjectCenterY { get { return m_LineGauge.MeasuredLine.CenterY; } }
        public bool ref_blnReferSubValue { get { return m_blnReferSubValue; } }
        public float ref_ObjectMeasuredAngle { get { return m_LineGauge.MeasuredLine.Angle; } }
        public float ref_ObjectAngle
        {
            get
            {
                return GetLineAngle();
            }
        }

        public float ref_ObjectSubAngle
        {

            // version 3
            // 2019 10 15 - CCENG: better formula of return angle (refer Euresys Line Gauge Angle.xls from server)
            get
            {
                return GetSubLineAngle();
            }
        }

        public float ref_ObjectOriAngle { get { return (m_LineGauge.MeasuredLine.Angle); } }
        public float ref_ObjectScore { get { return (m_LineGauge.NumValidSamples * 100 / (float)m_LineGauge.NumSamples); } }
        public Line ref_ObjectLine { get { return m_ObjectLine; } set { m_ObjectLine = value; } }

        #endregion


        public LGauge()
        {
            m_LineGauge = new ELineGauge();

            m_LineGauge.SamplingStep = 2;
            m_LineGauge.Dragable = false;
            m_LineGauge.Rotatable = false;
            m_LineGauge.Resizable = false;
            m_LineGauge.TransitionChoice = ETransitionChoice.NthFromBegin;

            m_SubLineGauge = new ELineGauge();

            m_SubLineGauge.SamplingStep = 2;
            m_SubLineGauge.Dragable = false;
            m_SubLineGauge.Rotatable = false;
            m_SubLineGauge.Resizable = false;
            m_SubLineGauge.TransitionChoice = ETransitionChoice.NthFromBegin;

        }

        public LGauge(GaugeWorldShape objWorldShape)
        {
            m_LineGauge = new ELineGauge();

            m_LineGauge.SamplingStep = 2;
            m_LineGauge.Dragable = false;
            m_LineGauge.Rotatable = false;
            m_LineGauge.Resizable = false;
            m_LineGauge.TransitionChoice = ETransitionChoice.NthFromBegin;

            m_LineGauge.Attach(objWorldShape.ref_objWorldShape);

            m_SubLineGauge = new ELineGauge();

            m_SubLineGauge.SamplingStep = 2;
            m_SubLineGauge.Dragable = false;
            m_SubLineGauge.Rotatable = false;
            m_SubLineGauge.Resizable = false;
            m_SubLineGauge.TransitionChoice = ETransitionChoice.NthFromBegin;

            m_SubLineGauge.Attach(objWorldShape.ref_objWorldShape);

        }
        
        public static void LoadFile(string strPath, List<LGauge> arrLineGauges, GaugeWorldShape objWorldShape)
        {
            if (arrLineGauges == null)
                return;

            for (int i = 0; i < arrLineGauges.Count; i++)
            {
                if (arrLineGauges[i] != null)
                    arrLineGauges[i].Dispose();
            }

            arrLineGauges.Clear();

            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection("Unit0");
            int intChildCount = objFile.GetSecondSectionCount();
            for (int j = 0; j < intChildCount; j++)
            {
                objFile.GetSecondSection("LineG" + j);

                LGauge objLineG = new LGauge(objWorldShape);
                objLineG.SetGaugePlacement(objFile.GetValueAsFloat("CenterX", 100, 2),
                                           objFile.GetValueAsFloat("CenterY", 100, 2),
                                           objFile.GetValueAsFloat("Tolerance", 10, 2),
                                           objFile.GetValueAsFloat("Length", 100, 2),
                                           objFile.GetValueAsFloat("Angle", 0, 2));

                objLineG.SetGaugeAdvancedSetting(objFile.GetValueAsInt("MinAmp", 10, 2),
                    objFile.GetValueAsInt("MinArea", 0, 2),
                    objFile.GetValueAsInt("Filter", 1, 2),
                    objFile.GetValueAsInt("Thickness", 1, 2),
                    objFile.GetValueAsInt("TransChoice", 0, 2),
                    objFile.GetValueAsInt("TransType", 0, 2),
                    objFile.GetValueAsInt("FilteringPass", 0, 2),
                    objFile.GetValueAsInt("Threshold", 20, 2),
                    objFile.GetValueAsFloat("FilteringThreshold", 3f, 2));

                objLineG.ref_GaugeSamplingStep = objFile.GetValueAsInt("SamplingStep", 5, 2);

                arrLineGauges.Add(objLineG);
            }
        }

        public static void LoadFile(string strPath, List<List<LGauge>> arrLineGauges, GaugeWorldShape objWorldShape)
        {
            for (int i = 0; i < arrLineGauges.Count; i++)
            {
                for (int j = 0; j < arrLineGauges[i].Count; j++)
                {
                    if (arrLineGauges[i][j] != null)
                        arrLineGauges[i][j].Dispose();
                }
            }

            arrLineGauges.Clear();

            XmlParser objFile = new XmlParser(strPath);

            int intCount = objFile.GetFirstSectionCount();
            for (int i = 0; i < intCount; i++)
            {
                objFile.GetFirstSection("Unit" + i.ToString());

                arrLineGauges.Add(new List<LGauge>());
                int intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    objFile.GetSecondSection("LineG" + j);

                    LGauge objLineG = new LGauge(objWorldShape);
                    objLineG.SetGaugePlacement(objFile.GetValueAsFloat("CenterX", 0, 2),
                                               objFile.GetValueAsFloat("CenterY", 0, 2),
                                               objFile.GetValueAsFloat("Tolerance", 10, 2),
                                               objFile.GetValueAsFloat("Length", 10, 2),
                                               objFile.GetValueAsFloat("Angle", 90, 2));


                    objLineG.SetGaugeAdvancedSetting(objFile.GetValueAsInt("MinAmp", 10, 2),
                                                     objFile.GetValueAsInt("MinArea", 0, 2),
                                                     objFile.GetValueAsInt("Filter", 1, 2),
                                                     objFile.GetValueAsInt("Thickness", 1, 2),
                                                     objFile.GetValueAsInt("TransChoice", 0, 2),
                                                     objFile.GetValueAsInt("TransType", 0, 2),
                                                     objFile.GetValueAsInt("FilteringPass", 0, 2),
                                                     objFile.GetValueAsInt("Threshold", 20, 2),
                                                     objFile.GetValueAsFloat("FilteringThreshold", 3f, 2));

                    objLineG.ref_GaugeSamplingStep = objFile.GetValueAsInt("SamplingStep", 5, 2);

                    arrLineGauges[i].Add(objLineG);
                }
            }
        }

        public static void SaveFile(string strPath, List<LGauge> arrLineGauges)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.WriteSectionElement("Unit0", true);
            for (int i = 0; i < arrLineGauges.Count; i++)
            {
                objFile.WriteElement1Value("LineG" + i, "");
                objFile.WriteElement2Value("CenterX", arrLineGauges[i].ref_GaugeCenterX);
                objFile.WriteElement2Value("CenterY", arrLineGauges[i].ref_GaugeCenterY);
                objFile.WriteElement2Value("Tolerance", arrLineGauges[i].ref_GaugeTolerance);
                objFile.WriteElement2Value("Length", arrLineGauges[i].ref_GaugeLength);
                objFile.WriteElement2Value("Angle", (int)arrLineGauges[i].ref_GaugeAngle);

                objFile.WriteElement2Value("SamplingStep", Convert.ToInt32(arrLineGauges[i].ref_GaugeSamplingStep));
                objFile.WriteElement2Value("TransType", arrLineGauges[i].ref_GaugeTransType);
                objFile.WriteElement2Value("TransChoice", arrLineGauges[i].ref_GaugeTransChoice);
                objFile.WriteElement2Value("Thickness", arrLineGauges[i].ref_GaugeThickness);
                objFile.WriteElement2Value("Filter", arrLineGauges[i].ref_GaugeFilter);
                objFile.WriteElement2Value("MinAmp", arrLineGauges[i].ref_GaugeMinAmplitude);
                objFile.WriteElement2Value("MinArea", arrLineGauges[i].ref_GaugeMinArea);
                objFile.WriteElement2Value("Threshold", arrLineGauges[i].ref_GaugeThreshold);
                objFile.WriteElement2Value("FilteringPass", arrLineGauges[i].ref_GaugeFilterPasses);
                objFile.WriteElement2Value("FilteringThreshold", arrLineGauges[i].ref_GaugeFilterThreshold);
            }

            objFile.WriteEndElement();
        }

        public static void SaveFile(string strPath, List<List<LGauge>> arrLineGauges)
        {
            XmlParser objFile = new XmlParser(strPath);

            for (int i = 0; i < arrLineGauges.Count; i++)
            {
                objFile.WriteSectionElement("Unit" + i, true);

                for (int j = 0; j < arrLineGauges[i].Count; j++)
                {
                    objFile.WriteElement1Value("LineG" + j, "");
                    objFile.WriteElement2Value("CenterX", arrLineGauges[i][j].ref_GaugeCenterX);
                    objFile.WriteElement2Value("CenterY", arrLineGauges[i][j].ref_GaugeCenterY);
                    objFile.WriteElement2Value("Tolerance", arrLineGauges[i][j].ref_GaugeTolerance);
                    objFile.WriteElement2Value("Length", arrLineGauges[i][j].ref_GaugeLength);
                    objFile.WriteElement2Value("Angle", (int)arrLineGauges[i][j].ref_GaugeAngle);

                    objFile.WriteElement2Value("SamplingStep", Convert.ToInt32(arrLineGauges[i][j].ref_GaugeSamplingStep));
                    objFile.WriteElement2Value("TransType", arrLineGauges[i][j].ref_GaugeTransType);
                    objFile.WriteElement2Value("TransChoice", arrLineGauges[i][j].ref_GaugeTransChoice);
                    objFile.WriteElement2Value("Thickness", arrLineGauges[i][j].ref_GaugeThickness);
                    objFile.WriteElement2Value("Filter", arrLineGauges[i][j].ref_GaugeFilter);
                    objFile.WriteElement2Value("MinAmp", arrLineGauges[i][j].ref_GaugeMinAmplitude);
                    objFile.WriteElement2Value("MinArea", arrLineGauges[i][j].ref_GaugeMinArea);
                    objFile.WriteElement2Value("Threshold", arrLineGauges[i][j].ref_GaugeThreshold);
                    objFile.WriteElement2Value("FilteringPass", arrLineGauges[i][j].ref_GaugeFilterPasses);
                    objFile.WriteElement2Value("FilteringThreshold", arrLineGauges[i][j].ref_GaugeFilterThreshold);
                }
            }

            objFile.WriteEndElement();
        }

        public static void SaveFile_SECSGEM(string strPath, List<LGauge> arrLineGauges, string strVisionName, int intCount, bool blnSECSGEMFileExist)
        {
            //XmlParser objFile = new XmlParser(strPath);
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");
            if (arrLineGauges != null)
            {
                for (int i = 0; i < intCount; i++)
                {
                    if (arrLineGauges.Count > i)
                    {
                        //objFile.WriteElementValue("LineG" + i, "");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_CenterX", arrLineGauges[i].ref_GaugeCenterX);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_CenterY", arrLineGauges[i].ref_GaugeCenterY);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Tolerance", arrLineGauges[i].ref_GaugeTolerance);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Length", arrLineGauges[i].ref_GaugeLength);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Angle", (int)arrLineGauges[i].ref_GaugeAngle);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_SamplingStep", Convert.ToInt32(arrLineGauges[i].ref_GaugeSamplingStep));
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_TransType", arrLineGauges[i].ref_GaugeTransType);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_TransChoice", arrLineGauges[i].ref_GaugeTransChoice);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Thickness", arrLineGauges[i].ref_GaugeThickness);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Filter", arrLineGauges[i].ref_GaugeFilter);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_MinAmp", arrLineGauges[i].ref_GaugeMinAmplitude);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_MinArea", arrLineGauges[i].ref_GaugeMinArea);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Threshold", arrLineGauges[i].ref_GaugeThreshold);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_FilteringPass", arrLineGauges[i].ref_GaugeFilterPasses);
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_FilteringThreshold", arrLineGauges[i].ref_GaugeFilterThreshold);
                    }
                    else
                    {
                        if (!blnSECSGEMFileExist)
                        {
                            //objFile.WriteElementValue("LineG" + i, "");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_CenterX", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_CenterY", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Tolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Length", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Angle", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_SamplingStep", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_TransType", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_TransChoice", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Thickness", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Filter", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_MinAmp", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Threshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_FilteringPass", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_FilteringThreshold", "NA");
                        }
                    }
                }
            }
            else
            {
                if (!blnSECSGEMFileExist)
                {
                    for (int i = 0; i < intCount; i++)
                    {
                        //objFile.WriteElementValue("LineG" + i, "");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_CenterX", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_CenterY", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Tolerance", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Length", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Angle", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_SamplingStep", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_TransType", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_TransChoice", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Thickness", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Filter", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_MinAmp", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_MinArea", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_Threshold", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_FilteringPass", "NA");
                        objFile.WriteElementValue(strVisionName + "_Unit0" + "_LineG" + i + "_FilteringThreshold", "NA");
                    }
                }
            }

            objFile.WriteEndElement();
        }

        public static void SaveFile_SECSGEM(string strPath, List<List<LGauge>> arrLineGauges, string strVisionName, int intCount, bool blnSECSGEMFileExist)
        {
            //XmlParser objFile = new XmlParser(strPath);

            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            if (arrLineGauges != null)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (arrLineGauges.Count > i)
                    {
                        for (int j = 0; j < intCount; j++)
                        {
                            if (arrLineGauges[i].Count > j)
                            {
                                //objFile.WriteElementValue("LineG" + j, "");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_CenterX", arrLineGauges[i][j].ref_GaugeCenterX);
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_CenterY", arrLineGauges[i][j].ref_GaugeCenterY);
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Tolerance", arrLineGauges[i][j].ref_GaugeTolerance);
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Length", arrLineGauges[i][j].ref_GaugeLength);
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Angle", (int)arrLineGauges[i][j].ref_GaugeAngle);

                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_SamplingStep", Convert.ToInt32(arrLineGauges[i][j].ref_GaugeSamplingStep));
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_TransType", arrLineGauges[i][j].ref_GaugeTransType);
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_TransChoice", arrLineGauges[i][j].ref_GaugeTransChoice);
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Thickness", arrLineGauges[i][j].ref_GaugeThickness);
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Filter", arrLineGauges[i][j].ref_GaugeFilter);
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_MinAmp", arrLineGauges[i][j].ref_GaugeMinAmplitude);
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_MinArea", arrLineGauges[i][j].ref_GaugeMinArea);
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Threshold", arrLineGauges[i][j].ref_GaugeThreshold);
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_FilteringPass", arrLineGauges[i][j].ref_GaugeFilterPasses);
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_FilteringThreshold", arrLineGauges[i][j].ref_GaugeFilterThreshold);
                            }
                            else
                            {
                                if (!blnSECSGEMFileExist)
                                {
                                    //objFile.WriteElementValue("LineG" + j, "");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_CenterX", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_CenterY", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Tolerance", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Length", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Angle", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_SamplingStep", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_TransType", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_TransChoice", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Thickness", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Filter", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_MinAmp", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_MinArea", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Threshold", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_FilteringPass", "NA");
                                    objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_FilteringThreshold", "NA");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!blnSECSGEMFileExist)
                        {
                            for (int j = 0; j < intCount; j++)
                            {
                                //objFile.WriteElementValue("LineG" + j, "");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_CenterX", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_CenterY", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Tolerance", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Length", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Angle", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_SamplingStep", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_TransType", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_TransChoice", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Thickness", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Filter", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_MinAmp", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_MinArea", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Threshold", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_FilteringPass", "NA");
                                objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_FilteringThreshold", "NA");
                            }
                        }
                    }
                }
            }
            else
            {
                if (!blnSECSGEMFileExist)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < intCount; j++)
                        {
                            //objFile.WriteElementValue("LineG" + j, "");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_CenterX", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_CenterY", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Tolerance", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Length", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Angle", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_SamplingStep", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_TransType", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_TransChoice", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Thickness", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Filter", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_MinAmp", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_MinArea", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_Threshold", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_FilteringPass", "NA");
                            objFile.WriteElementValue(strVisionName + "_Unit" + i + "_LineG" + j + "_FilteringThreshold", "NA");
                        }
                    }
                }
            }

            objFile.WriteEndElement();
        }

        /// <summary>
        /// Validate whether the line gauge is a valid 
        /// </summary>
        /// <returns>true = valid line gauge, false = invalid line gauge</returns>
        public bool IsImageValid()
        {
            if (m_LineGauge.NumValidSamples < (m_LineGauge.NumSamples / 2))
                return false;

            return true;
        }

        /// <summary>
        /// Draw line gauge sample points, actual edge, center X label, center Y label and angle label
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        public void DrawLineGauge(Graphics g)
        {
            if (!m_blnExecuted)
                return;

            //m_LineGauge.Draw(g, new Pen(Color.Blue, 1));
            m_LineGauge.Draw(g, new ERGBColor(255, 255, 0), EDrawingMode.SampledPoints);
            m_LineGauge.Draw(g, new ERGBColor(0, 255, 0), EDrawingMode.Actual);
            g.DrawString("x: " + m_LineGauge.MeasuredLine.Center.X.ToString(), m_FontMatched, new SolidBrush(Color.Red), (float)m_LineGauge.MeasuredLine.Center.X, (float)m_LineGauge.MeasuredLine.Center.Y);
            g.DrawString("y: " + m_LineGauge.MeasuredLine.Center.Y.ToString(), m_FontMatched, new SolidBrush(Color.Red), (float)m_LineGauge.MeasuredLine.Center.X, (float)m_LineGauge.MeasuredLine.Center.Y + 10);
            g.DrawString("Angle: " + Math.Round(m_LineGauge.MeasuredLine.Angle, 2), m_FontMatched, new SolidBrush(Color.Red), (float)m_LineGauge.MeasuredLine.Center.X, (float)m_LineGauge.MeasuredLine.Center.Y + 20);
        }

        /// <summary>
        /// Draw line gauge sample points
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        public void DrawGauge(Graphics g)
        {
            if (!m_blnExecuted)
                return;

            if (!m_blnReferSubValue)
            {
                m_LineGauge.Draw(g, new ERGBColor(173, 255, 47), EDrawingMode.SampledPoints);
                m_LineGauge.Draw(g, new ERGBColor(255, 255, 0), EDrawingMode.Actual);
                m_LineGauge.Draw(g, new ERGBColor(0, 0, 255), EDrawingMode.Nominal);
            }
            else
            {
                m_SubLineGauge.Draw(g, new ERGBColor(173, 255, 47), EDrawingMode.SampledPoints);
                m_SubLineGauge.Draw(g, new ERGBColor(255, 255, 0), EDrawingMode.Actual);
                m_SubLineGauge.Draw(g, new ERGBColor(0, 0, 255), EDrawingMode.Nominal);
            }
        }
        public void DrawDraggingBoxGauge(Graphics g)
        {
            if (!m_blnReferSubValue)
            {
                m_LineGauge.Draw(g, new ERGBColor(0, 0, 255));
            }
            else
            {
                m_SubLineGauge.Draw(g, new ERGBColor(0, 0, 255));
            }
        }
        public void DrawRealGauge(Graphics g)
        {
            if (!m_blnReferSubValue)
            {
                m_LineGauge.Draw(g, new ERGBColor(0, 255, 0), EDrawingMode.SampledPoints);
                m_LineGauge.Draw(g, new ERGBColor(255, 255, 0), EDrawingMode.Actual);
            }
            else
            {
                m_SubLineGauge.Draw(g, new ERGBColor(0, 255, 0), EDrawingMode.SampledPoints);
                m_SubLineGauge.Draw(g, new ERGBColor(255, 255, 0), EDrawingMode.Actual);
            }
        }

        public void DrawSamplingPointGauge(Graphics g)
        {
            if (!m_blnReferSubValue)
            {
                m_LineGauge.Draw(g, new ERGBColor(0, 255, 0), EDrawingMode.SampledPoints);
            }
            else
            {
                m_SubLineGauge.Draw(g, new ERGBColor(0, 255, 0), EDrawingMode.SampledPoints);
            }
        }

        public void DrawValidSamplingPointGauge(Graphics g)
        {
            if (!m_blnReferSubValue)
            {
                m_LineGauge.Draw(g, new ERGBColor(0, 255, 0), EDrawingMode.SampledPoints);
            }
            else
            {
                m_SubLineGauge.Draw(g, new ERGBColor(0, 255, 0), EDrawingMode.SampledPoints);
            }
        }
        public void DrawInValidSamplingPointGauge(Graphics g)
        {
            if (!m_blnReferSubValue)
            {
                m_LineGauge.Draw(g, new ERGBColor(255, 0, 0), EDrawingMode.InvalidSampledPoints);
            }
            else
            {
                m_SubLineGauge.Draw(g, new ERGBColor(255, 0, 0), EDrawingMode.InvalidSampledPoints);
            }
        }
        public void DrawSamplingPointGauge(Graphics g, Color objColor)
        {
            if (!m_blnReferSubValue)
            {
                m_LineGauge.Draw(g, new ERGBColor(objColor.R, objColor.G, objColor.B), EDrawingMode.SampledPoints);
            }
            else
            {
                m_SubLineGauge.Draw(g, new ERGBColor(objColor.R, objColor.G, objColor.B), EDrawingMode.SampledPoints);
            }
        }

        public void DrawResultLineGauge(Graphics g)
        {
            if (!m_blnReferSubValue)
            {
                m_LineGauge.Draw(g, new ERGBColor(Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B), EDrawingMode.Actual);
            }
            else
            {
                m_SubLineGauge.Draw(g, new ERGBColor(Color.LightGreen.R, Color.LightGreen.G, Color.LightGreen.B), EDrawingMode.Actual);
            }
        }

        public void DrawResultLineGauge(Graphics g, Color objColor)
        {
            if (!m_blnReferSubValue)
                m_LineGauge.Draw(g, new ERGBColor(objColor.R, objColor.G, objColor.B), EDrawingMode.Actual);
            else
                m_SubLineGauge.Draw(g, new ERGBColor(objColor.R, objColor.G, objColor.B), EDrawingMode.Actual, true);
        }
        public void DrawPositionLineGauge(Graphics g, Color objColor)
        {
            if (m_LineGauge.Tolerance != 0)
                m_LineGauge.Tolerance = 0;
                m_LineGauge.Draw(g, new ERGBColor(objColor.R, objColor.G, objColor.B), EDrawingMode.Nominal);
        }
        /// <summary>
        /// Design top gauge
        /// </summary>
        /// <param name="objGauge">top line gauge</param>
        /// <param name="objROI">ROI</param>
        /// <param name="fGap">gap in float</param>
        public void DuplicateTopGauge(ref LGauge objGauge, ROI objROI, float fGap)
        {
            float fPositionY = m_LineGauge.MeasuredLine.CenterY - fGap;
            float fTolerance = (fPositionY - objROI.ref_ROIPositionY) / 2;

            objGauge.SetGaugePlacement(m_LineGauge.MeasuredLine.CenterX,
                fPositionY - fTolerance, fTolerance, m_LineGauge.Length, 180);

            objGauge.ref_GaugeSamplingStep = m_LineGauge.SamplingStep;
#if (Debug_2_12 || Release_2_12)
            objGauge.SetGaugeAdvancedSetting((int)m_LineGauge.MinAmplitude, (int)m_LineGauge.MinArea, (int)m_LineGauge.Smoothing,
                (int)m_LineGauge.Thickness, (int)m_LineGauge.TransitionChoice, (int)m_LineGauge.TransitionType, (int)m_LineGauge.NumFilteringPasses,
               (int)m_LineGauge.Threshold, m_LineGauge.FilteringThreshold);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            objGauge.SetGaugeAdvancedSetting(m_LineGauge.MinAmplitude, m_LineGauge.MinArea, m_LineGauge.Smoothing,
                m_LineGauge.Thickness, (int)m_LineGauge.TransitionChoice, (int)m_LineGauge.TransitionType, m_LineGauge.NumFilteringPasses,
                m_LineGauge.Threshold, m_LineGauge.FilteringThreshold);
#endif

        }

        /// <summary>
        /// Design bottom gauge
        /// </summary>
        /// <param name="objGauge">bottom line gauge</param>
        /// <param name="objROI">ROI</param>
        /// <param name="fGap">gap in float</param>
        public void DuplicateBottomGauge(ref LGauge objGauge, ROI objROI, float fGap)
        {
            float fPositionY = m_LineGauge.MeasuredLine.CenterY + fGap;
            float fTolerance = (fPositionY + objROI.ref_ROIHeight - m_LineGauge.MeasuredLine.CenterY) / 2;
            objGauge.SetGaugePlacement(m_LineGauge.MeasuredLine.CenterX,
                fPositionY + fTolerance, fTolerance, m_LineGauge.Length, 0);
#if (Debug_2_12 || Release_2_12)
            objGauge.SetGaugeAdvancedSetting((int)m_LineGauge.MinAmplitude, (int)m_LineGauge.MinArea, (int)m_LineGauge.Smoothing,
               (int)m_LineGauge.Thickness, (int)m_LineGauge.TransitionChoice, (int)m_LineGauge.TransitionType, (int)m_LineGauge.NumFilteringPasses,
                (int)m_LineGauge.Threshold, m_LineGauge.FilteringThreshold);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            objGauge.SetGaugeAdvancedSetting(m_LineGauge.MinAmplitude, m_LineGauge.MinArea, m_LineGauge.Smoothing,
                m_LineGauge.Thickness, (int)m_LineGauge.TransitionChoice, (int)m_LineGauge.TransitionType, m_LineGauge.NumFilteringPasses,
                m_LineGauge.Threshold, m_LineGauge.FilteringThreshold);
#endif

        }

        public void DuplicateBottomGauge2(ref LGauge objGauge, ROI objROI, EBlobs objEBlobs)
        {
            float fBlobsHeight = objEBlobs.ref_arrHeight[0];
            float fTolerance = (float)(objROI.ref_ROITotalY + objROI.ref_ROIHeight - m_LineGauge.MeasuredLine.CenterY - fBlobsHeight / 5) / 2;
            float fPositionY = m_LineGauge.MeasuredLine.CenterY + fBlobsHeight / 5 + fTolerance;

            objGauge.SetGaugePlacement(m_LineGauge.MeasuredLine.CenterX,
                fPositionY, fTolerance, m_LineGauge.Length, 0);

            objGauge.ref_GaugeSamplingStep = m_LineGauge.SamplingStep;
#if (Debug_2_12 || Release_2_12)
            objGauge.SetGaugeAdvancedSetting((int)m_LineGauge.MinAmplitude, (int)m_LineGauge.MinArea, (int)m_LineGauge.Smoothing,
               (int)m_LineGauge.Thickness, (int)m_LineGauge.TransitionChoice, (int)m_LineGauge.TransitionType, (int)m_LineGauge.NumFilteringPasses,
               (int)m_LineGauge.Threshold, m_LineGauge.FilteringThreshold);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            objGauge.SetGaugeAdvancedSetting(m_LineGauge.MinAmplitude, m_LineGauge.MinArea, m_LineGauge.Smoothing,
                m_LineGauge.Thickness, (int)m_LineGauge.TransitionChoice, (int)m_LineGauge.TransitionType, m_LineGauge.NumFilteringPasses,
                m_LineGauge.Threshold, m_LineGauge.FilteringThreshold);
#endif

        }

        public Line GetObjectLine()
        {
            if (!m_blnReferSubValue)
            {
                m_ObjectLine.CalculateLGStraightLine(new PointF(m_LineGauge.MeasuredLine.CenterX,
                                                                                   m_LineGauge.MeasuredLine.CenterY),
                                                                                   m_LineGauge.MeasuredLine.Angle);
            }
            else
            {
                m_ObjectLine.CalculateLGStraightLine(new PointF(m_SubLineGauge.MeasuredLine.CenterX,
                                                                                   m_SubLineGauge.MeasuredLine.CenterY),
                                                                                   m_SubLineGauge.MeasuredLine.Angle);
            }
            //ref_GaugeAngle);
            return m_ObjectLine;
        }

        public Line GetObjectLine(float fCenterX, float fCenterY, float fAngle)
        {
            m_ObjectLine.CalculateLGStraightLine(new PointF(fCenterX, fCenterY), fAngle);
            return m_ObjectLine;
        }

        /// <summary>
        /// Measure line gauge
        /// </summary>
        /// <param name="objROI">ROI</param>
        public void Measure(ROI objROI)
        {
            try
            {
                m_blnReferSubValue = false;
                //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent3.bmp");
                //m_LineGauge.Save("D:\\TS\\ELineGauge3.CAL");

                m_LineGauge.Measure(objROI.ref_ROI);
                if (!m_blnExecuted)
                    m_blnExecuted = true;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }
        }

        public void Measure(ROI objROI, int intIndex)
        {
            try
            {
                m_blnReferSubValue = false;
                //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent" + intIndex.ToString() + ".bmp");
                //m_LineGauge.Save("D:\\TS\\ELineGauge" + intIndex.ToString() + ".CAL");
                m_LineGauge.Measure(objROI.ref_ROI);
                if (!m_blnExecuted)
                    m_blnExecuted = true;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }
        }

        public void Measure_SubGauge(ROI objROI, float fMinScore)
        {
            try
            {
                m_blnReferSubValue = false;
                //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent3.bmp");
                //m_LineGauge.Save("D:\\TS\\ELineGauge3.CAL");
                m_LineGauge.Measure(objROI.ref_ROI);

                if (m_SubLineGauge == null)
                    m_SubLineGauge = new ELineGauge();

                m_SubLineGauge.Tolerance = m_LineGauge.Tolerance;
                m_SubLineGauge.Length = m_LineGauge.Length;
                m_SubLineGauge.SetCenterXY(m_LineGauge.CenterX, m_LineGauge.CenterY);
                m_SubLineGauge.Angle = m_LineGauge.Angle;

                m_SubLineGauge.MinAmplitude = m_LineGauge.MinAmplitude;
                m_SubLineGauge.MinArea = m_LineGauge.MinArea;
                m_SubLineGauge.Smoothing = m_LineGauge.Smoothing;
                m_SubLineGauge.Thickness = m_LineGauge.Thickness;
                m_SubLineGauge.NumFilteringPasses = m_LineGauge.NumFilteringPasses;
                m_SubLineGauge.FilteringThreshold = m_LineGauge.FilteringThreshold;
                m_SubLineGauge.Threshold = m_LineGauge.Threshold;
                m_SubLineGauge.TransitionChoice = m_LineGauge.TransitionChoice;
                m_SubLineGauge.TransitionType = m_LineGauge.TransitionType;

                //STTrackLog.WriteLine("------ Start  ----------" + m_LineGauge.Angle.ToString());
                float fToleranceSamplingStep = 3f;
                m_SubLineGauge.Tolerance = fToleranceSamplingStep;
                List<float> arrSubScore = new List<float>();
                List<float> arrSubAngle = new List<float>();

                if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
                {
                    float fStartY = m_LineGauge.CenterY - m_LineGauge.Tolerance;
                    float fEndY = m_LineGauge.CenterY + m_LineGauge.Tolerance;
                    int intHighestScoreIndex = -1;
                    float fHighestScore = 0;
                    float fHigheseScoreCenterY = 0;
                    int intIndex = 0;
                    List<int> arrHighestScoreInddex = new List<int>();
                    List<float> arrHighestScore = new List<float>();
                    List<float> arrHighestScoreCenterY = new List<float>();

                    // ------------------- checking loop timeout ---------------------------------------------------
                    HiPerfTimer timeout = new HiPerfTimer();
                    timeout.Start();


                    do
                    {

                        // ------------------- checking loop timeout ---------------------------------------------------
                        if (timeout.Timing > 10000)
                        {
                            STTrackLog.WriteLine(">>>>>>>>>>>>> time out 908");
                            break;
                        }
                        // ---------------------------------------------------------------------------------------------



                        m_SubLineGauge.SetCenterXY(m_LineGauge.CenterX, fStartY + fToleranceSamplingStep);
                        m_SubLineGauge.Measure(objROI.ref_ROI);

                        arrSubScore.Add(m_SubLineGauge.NumValidSamples * 100 / (float)m_SubLineGauge.NumSamples);
                        arrSubAngle.Add(ConvertObjectAngle(ref m_SubLineGauge));

                        if (arrSubScore[intIndex] >= fHighestScore)
                        {
                            intHighestScoreIndex = intIndex;
                            fHighestScore = arrSubScore[intIndex];
                            fHigheseScoreCenterY = fStartY + fToleranceSamplingStep;
                        }

                        int intInsertIndex = arrHighestScore.Count;
                        for (int i = 0; i < arrHighestScore.Count; i++)
                        {
                            if (arrSubScore[intIndex] > arrHighestScore[i])
                            {
                                intInsertIndex = i;
                                break;
                            }
                        }

                        if (arrSubScore[intIndex] > fMinScore)
                        {
                            arrHighestScore.Insert(intInsertIndex, arrSubScore[intIndex]);
                            arrHighestScoreCenterY.Insert(intInsertIndex, fStartY + fToleranceSamplingStep);
                            arrHighestScoreInddex.Insert(intInsertIndex, intIndex);
                        }

                        //m_SubLineGauge.Save("D:\\TS\\ESubLineGauge" + fStartY.ToString() + ".CAL");
                        //STTrackLog.WriteLine(arrSubScore[arrSubScore.Count - 1] + ", " + arrSubAngle[arrSubAngle.Count - 1]);

                        if ((fStartY + fToleranceSamplingStep * 2) >= fEndY)
                            break;
                        else
                            fStartY += fToleranceSamplingStep;

                        intIndex++;

                    } while (true);
                    timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

                    if (intHighestScoreIndex >= 0)
                    {
                        int intSelectedIndex = 0;
                        // Looking priority line from top to bottom
                        //if (arrHighestScoreCenterY.Count > 0 && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromBegin)
                        if (arrHighestScoreCenterY.Count > 0 &&
                           (((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromBegin) ||   // 0 deg mean going down. 
                            ((m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromEnd)))     // 180 deg mean going up
                        {

                            float fBeginPosition = float.MaxValue;
                            for (int i = 0; i < arrHighestScoreCenterY.Count; i++)
                            {
                                // Select center Y which has smaller value
                                if (fBeginPosition > arrHighestScoreCenterY[i])
                                {
                                    intSelectedIndex = i;
                                    fBeginPosition = arrHighestScoreCenterY[i];
                                }
                            }

                            m_SubLineGauge.SetCenterXY(m_LineGauge.CenterX, arrHighestScoreCenterY[intSelectedIndex]);
                        }
                        // Looking priority line from bottom to top
                        //else if (arrHighestScoreCenterY.Count > 0 && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromEnd)
                        else if (arrHighestScoreCenterY.Count > 0 &&
                           (((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromEnd) ||
                            ((m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromBegin)))
                        {

                            float fEndPosition = float.MinValue;
                            for (int i = 0; i < arrHighestScoreCenterY.Count; i++)
                            {
                                // Select center Y which has bigger value
                                if (fEndPosition < arrHighestScoreCenterY[i])
                                {
                                    intSelectedIndex = i;
                                    fEndPosition = arrHighestScoreCenterY[i];
                                }
                            }

                            m_SubLineGauge.SetCenterXY(m_LineGauge.CenterX, arrHighestScoreCenterY[intSelectedIndex]);
                        }
                        else
                        {
                            m_SubLineGauge.SetCenterXY(m_LineGauge.CenterX, fHigheseScoreCenterY);
                        }



                        m_SubLineGauge.Measure(objROI.ref_ROI);
                        m_fSubGaugeAngle = m_SubLineGauge.MeasuredLine.Angle;
                        m_blnReferSubValue = true;
                    }
                    else
                    {
                        m_blnReferSubValue = false;
                    }

                }
                else
                {
                    float fStartX = m_LineGauge.CenterX - m_LineGauge.Tolerance;
                    float fEndX = m_LineGauge.CenterX + m_LineGauge.Tolerance;
                    int intHighestScoreIndex = -1;
                    float fHighestScore = 0;
                    float fHigheseScoreCenterX = 0;
                    int intIndex = 0;
                    List<int> arrHighestScoreInddex = new List<int>();
                    List<float> arrHighestScore = new List<float>();
                    List<float> arrHighestScoreCenterX = new List<float>();
                    // ------------------- checking loop timeout ---------------------------------------------------
                    HiPerfTimer timeout = new HiPerfTimer();
                    timeout.Start();

                    do
                    {

                        // ------------------- checking loop timeout ---------------------------------------------------
                        if (timeout.Timing > 10000)
                        {
                            STTrackLog.WriteLine(">>>>>>>>>>>>> time out 907");
                            break;
                        }
                        // ---------------------------------------------------------------------------------------------



                        m_SubLineGauge.SetCenterXY(fStartX + fToleranceSamplingStep, m_LineGauge.CenterY);
                        m_SubLineGauge.Measure(objROI.ref_ROI);

                        arrSubScore.Add(m_SubLineGauge.NumValidSamples * 100 / (float)m_SubLineGauge.NumSamples);
                        arrSubAngle.Add(ConvertObjectAngle(ref m_SubLineGauge));

                        if (arrSubScore[intIndex] >= fHighestScore)
                        {
                            intHighestScoreIndex = intIndex;
                            fHighestScore = arrSubScore[intIndex];
                            fHigheseScoreCenterX = fStartX + fToleranceSamplingStep;
                        }

                        int intInsertIndex = arrHighestScore.Count;
                        for (int i = 0; i < arrHighestScore.Count; i++)
                        {
                            if (arrSubScore[intIndex] > arrHighestScore[i])
                            {
                                intInsertIndex = i;
                                break;
                            }
                        }

                        if (arrSubScore[intIndex] > fMinScore)
                        {
                            arrHighestScore.Insert(intInsertIndex, arrSubScore[intIndex]);
                            arrHighestScoreCenterX.Insert(intInsertIndex, fStartX + fToleranceSamplingStep);
                            arrHighestScoreInddex.Insert(intInsertIndex, intIndex);
                        }

                        //STTrackLog.WriteLine(arrSubScore[arrSubScore.Count - 1] + ", " + arrSubAngle[arrSubAngle.Count - 1]);

                        if ((fStartX + fToleranceSamplingStep * 2) >= fEndX)
                            break;
                        else
                            fStartX += fToleranceSamplingStep;

                        intIndex++;

                    } while (true);
                    timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

                    if (intHighestScoreIndex >= 0)
                    {
                        int intSelectedIndex = 0;
                        // Looking priority line from left to right
                        //if (arrHighestScoreCenterX.Count > 0 && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromBegin)
                        if (arrHighestScoreCenterX.Count > 0 &&
                            (((m_LineGauge.Angle > -135 && m_LineGauge.Angle < -45) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromBegin) ||    // -90 mean from left to right
                            ((m_LineGauge.Angle > 45 && m_LineGauge.Angle < 135) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromEnd)))          // 90 deg mean from right to left
                        {

                            float fBeginPosition = float.MaxValue;
                            for (int i = 0; i < arrHighestScoreCenterX.Count; i++)
                            {
                                // Select center X which has smaller value
                                if (fBeginPosition > arrHighestScoreCenterX[i])
                                {
                                    intSelectedIndex = i;
                                    fBeginPosition = arrHighestScoreCenterX[i];
                                }
                            }

                            m_SubLineGauge.SetCenterXY(arrHighestScoreCenterX[intSelectedIndex], m_LineGauge.CenterX);
                        }
                        //else if (arrHighestScoreCenterX.Count > 0 && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromEnd)
                        else if (arrHighestScoreCenterX.Count > 0 &&
                            (((m_LineGauge.Angle > -135 && m_LineGauge.Angle < -45) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromEnd) ||     // -90 mean from left to right
                            ((m_LineGauge.Angle > 45 && m_LineGauge.Angle < 135) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromBegin)))          // 90 deg mean from right to left
                        {

                            float fEndPosition = float.MinValue;
                            for (int i = 0; i < arrHighestScoreCenterX.Count; i++)
                            {
                                // Select center X which has bigger value
                                if (fEndPosition < arrHighestScoreCenterX[i])
                                {
                                    intSelectedIndex = i;
                                    fEndPosition = arrHighestScoreCenterX[i];
                                }
                            }

                            m_SubLineGauge.SetCenterXY(arrHighestScoreCenterX[intSelectedIndex], m_LineGauge.CenterX);
                        }
                        else
                        {
                            m_SubLineGauge.SetCenterXY(fHigheseScoreCenterX, m_LineGauge.CenterY);
                        }

                        m_SubLineGauge.Measure(objROI.ref_ROI);
                        m_fSubGaugeAngle = m_SubLineGauge.MeasuredLine.Angle;
                        m_blnReferSubValue = true;
                    }
                    else
                    {
                        m_blnReferSubValue = false;
                    }
                }

                //STTrackLog.WriteLine("------ End ----------");

                if (!m_blnExecuted)
                    m_blnExecuted = true;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }
        }

        //public void Measure_SubGauge_RDStage(ROI objROI)
        //{
        //    try
        //    {
        //        //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent3.bmp");
        //        //m_LineGauge.Save("D:\\TS\\ELineGauge3.CAL");
        //        m_LineGauge.Measure(objROI.ref_ROI);

        //        //if (m_SubLineGauge == null)
        //        //    m_SubLineGauge = new ELineGauge();

        //        //m_SubLineGauge.Tolerance = m_LineGauge.Tolerance;
        //        //m_SubLineGauge.Length = m_LineGauge.Length;
        //        //m_SubLineGauge.SetCenterXY(m_LineGauge.CenterX, m_LineGauge.CenterY);
        //        //m_SubLineGauge.Angle = m_LineGauge.Angle;

        //        //m_SubLineGauge.MinAmplitude = m_LineGauge.MinAmplitude;
        //        //m_SubLineGauge.MinArea = m_LineGauge.MinArea;
        //        //m_SubLineGauge.Smoothing = m_LineGauge.Smoothing;
        //        //m_SubLineGauge.Thickness = m_LineGauge.Thickness;
        //        //m_SubLineGauge.NumFilteringPasses = m_LineGauge.NumFilteringPasses;
        //        //m_SubLineGauge.FilteringThreshold = m_LineGauge.FilteringThreshold;
        //        //m_SubLineGauge.Threshold = m_LineGauge.Threshold;
        //        //m_SubLineGauge.TransitionChoice = m_LineGauge.TransitionChoice;
        //        //m_SubLineGauge.TransitionType = m_LineGauge.TransitionType;

        //        //m_SubLineGauge.SamplingStep = m_LineGauge.SamplingStep;

        //        if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
        //        {
        //            float fToleranceSamplingStep = 6f;
        //            m_SubLineGauge.Tolerance = fToleranceSamplingStep;
        //            List<float> arrSubScore = new List<float>();
        //            List<float> arrSubAngle = new List<float>();
        //            float fStartY = m_LineGauge.CenterY - m_LineGauge.Tolerance;
        //            float fEndY = m_LineGauge.CenterY + m_LineGauge.Tolerance;
        //            //STTrackLog.WriteLine("------ Start  ----------" + m_LineGauge.Angle.ToString());
        //            do
        //            {
        //                m_SubLineGauge.SetCenterXY(m_LineGauge.CenterX, fStartY + fToleranceSamplingStep);
        //                m_SubLineGauge.Measure(objROI.ref_ROI);

        //                //m_SubLineGauge.Save("D:\\TS\\ESubLineGauge" + fStartY.ToString() + ".CAL");

        //                arrSubScore.Add(m_SubLineGauge.NumValidSamples * 100 / (float)m_SubLineGauge.NumSamples);
        //                arrSubAngle.Add(ConvertObjectAngle(ref m_SubLineGauge));

        //                STTrackLog.WriteLine(arrSubScore[arrSubScore.Count - 1] + ", " + arrSubAngle[arrSubAngle.Count - 1]);

        //                if ((fStartY + fToleranceSamplingStep * 2) >= fEndY)
        //                    break;
        //                else
        //                    fStartY += fToleranceSamplingStep;

        //            } while (true);

        //            STTrackLog.WriteLine("------ End ----------");

        //        }
        //        else
        //        {
        //            float fToleranceSamplingStep = 3f;
        //            m_SubLineGauge.Tolerance = fToleranceSamplingStep;
        //            List<float> arrSubScore = new List<float>();
        //            List<float> arrSubAngle = new List<float>();
        //            float fStartX = m_LineGauge.CenterX - m_LineGauge.Tolerance;
        //            float fEndX = m_LineGauge.CenterX + m_LineGauge.Tolerance;
        //            STTrackLog.WriteLine("------ Start  ----------" + m_LineGauge.Angle.ToString());
        //            int intHighestScoreIndex = -1;
        //            float fHighestScore = 0;
        //            float fHigheseScoreCenterX = 0;
        //            int intIndex = 0;
        //            do
        //            {
        //                m_SubLineGauge.SetCenterXY(fHigheseScoreCenterX, m_LineGauge.CenterY);
        //                m_SubLineGauge.Measure(objROI.ref_ROI);

        //                //m_SubLineGauge.Save("D:\\TS\\ESubLineGauge" + fStartX.ToString() + ".CAL");

        //                arrSubScore.Add(m_SubLineGauge.NumValidSamples * 100 / (float)m_SubLineGauge.NumSamples);
        //                arrSubAngle.Add(ConvertObjectAngle(ref m_SubLineGauge));
        //                float fff = m_SubLineGauge.MeasuredLine.Angle;

        //                if (arrSubScore[intIndex] >= fHighestScore)
        //                {
        //                    //m_SubLineGauge.Save("D:\\TS\\ESubLineGauge" + fStartX.ToString() + "_H.CAL");
        //                    intHighestScoreIndex = intIndex;
        //                    fHighestScore = arrSubScore[intIndex];
        //                    fHigheseScoreCenterX = fStartX + fToleranceSamplingStep;
        //                }

        //                STTrackLog.WriteLine(arrSubScore[arrSubScore.Count - 1] + ", " + arrSubAngle[arrSubAngle.Count - 1]);

        //                if ((fStartX + fToleranceSamplingStep * 2) >= fEndX)
        //                    break;
        //                else
        //                    fStartX += fToleranceSamplingStep;

        //                intIndex++;

        //            } while (true);

        //            if (intHighestScoreIndex >= 0 && m_LineGauge.Angle == 90)
        //            {
        //                m_SubLineGauge.SetCenterXY(fHigheseScoreCenterX, m_LineGauge.CenterY);
        //                m_SubLineGauge.Measure(objROI.ref_ROI);
        //                m_fSubGaugeAngle = ConvertObjectAngle(ref m_SubLineGauge);

        //                //float fAngle0 = m_LineGauge.MeasuredLine.Angle;
        //                //m_LineGauge.Save("D:\\TS\\m_LineGauge" + fStartX.ToString() + "_H0.CAL");
        //                //float fTolerancePrev = m_LineGauge.Tolerance;
        //                //float fCenterXPrev = m_LineGauge.CenterX;
        //                //m_LineGauge.Tolerance = fToleranceSamplingStep;
        //                //m_LineGauge.SetCenterXY(fHigheseScoreCenterX, m_LineGauge.CenterY);
        //                //m_LineGauge.Measure(objROI.ref_ROI);

        //                //m_fSubGaugeAngle = m_LineGauge.MeasuredLine.Angle;
        //                //m_SubLineGauge.SetCenterXY(fHigheseScoreCenterX, m_LineGauge.CenterY);
        //                //m_SubLineGauge.Measure(objROI.ref_ROI);
        //                //float fAngle1 = m_SubLineGauge.MeasuredLine.Angle;
        //                //float fAngle10 = ConvertObjectAngle(ref m_SubLineGauge);
        //                //m_LineGauge.Save("D:\\TS\\m_LineGauge" + fStartX.ToString() + "_H1.CAL");

        //                //m_LineGauge.Tolerance = fTolerancePrev;
        //                //m_LineGauge.SetCenterXY(fCenterXPrev, m_LineGauge.CenterY);
        //                //m_LineGauge.Measure(objROI.ref_ROI);

        //                //float fAngle2 = m_LineGauge.MeasuredLine.Angle;
        //                //m_LineGauge.Save("D:\\TS\\m_LineGauge" + fStartX.ToString() + "_H2.CAL");

        //                m_blnReferSubValue = true;

        //            }
        //            else
        //            {
        //                m_blnReferSubValue = false;
        //            }

        //            STTrackLog.WriteLine("------ End ----------");
        //        }

        //        if (!m_blnExecuted)
        //            m_blnExecuted = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        SRMMessageBox.Show(ex.ToString());
        //    }
        //}

        private float ConvertObjectAngle(ref ELineGauge objLineGauge)
        {
            // version 3
            // 2019 10 15 - CCENG: better formula of return angle (refer Euresys Line Gauge Angle.xls from server)

            if ((objLineGauge.Angle >= 225) && (objLineGauge.Angle <= 315))   // Gauge Direction 270
            {
                if (objLineGauge.MeasuredLine.Angle > 180)
                {
                    return (objLineGauge.MeasuredLine.Angle - 270) - (objLineGauge.Angle - 270);
                }
                else
                {
                    if (objLineGauge.MeasuredLine.Angle > 0)
                        return (objLineGauge.MeasuredLine.Angle + 270) - (objLineGauge.Angle - 270) - 360;
                    else
                        return (objLineGauge.MeasuredLine.Angle - 270) - (objLineGauge.Angle - 270) + 360;
                }
            }
            else if ((objLineGauge.Angle >= 135) && (objLineGauge.Angle <= 225))  // Gauge Direction 180
            {
                if (objLineGauge.MeasuredLine.Angle > 90)
                    return (objLineGauge.MeasuredLine.Angle - 180) - (objLineGauge.Angle - 180);
                else
                    return (objLineGauge.MeasuredLine.Angle - (objLineGauge.Angle - 180));
            }
            else if ((objLineGauge.Angle >= 45) && (objLineGauge.Angle <= 135)) // Gauge Direction 90
            {
                if (objLineGauge.MeasuredLine.Angle > 0)
                    return (objLineGauge.MeasuredLine.Angle - 90) - (objLineGauge.Angle - 90);
                else
                    return (objLineGauge.MeasuredLine.Angle + 90) - (objLineGauge.Angle - 90);
            }
            else if ((objLineGauge.Angle >= -135) && (objLineGauge.Angle <= -45)) // Gauge Direction -90
            {
                if (objLineGauge.MeasuredLine.Angle > 0)
                    return (objLineGauge.MeasuredLine.Angle - 90) - (objLineGauge.Angle + 90);
                else
                    return (objLineGauge.MeasuredLine.Angle + 90) - (objLineGauge.Angle + 90);
            }
            else
                return (objLineGauge.MeasuredLine.Angle - objLineGauge.Angle);
        }

        public void Measure(ImageDrawing objImage)
        {
            /*
             * Measure using ROI, not image. 
             * Measuring image may get unstable result.
             */

            try
            {
                //m_LineGauge.Save("D:\\TS\\LineGauge3A.CAL");
                //objImage.ref_objMainImage.Save("D:\\TS\\objImage3A.bmp");

                //ROI objROI = new ROI();   // 2018 09 10 - CCENG: Use global ROI instead of local ROI because consume time additional 1.5ms for ROI initialization.
                m_blnReferSubValue = false;
                m_objLocalROI.AttachImage(objImage);
                m_objLocalROI.LoadROISetting(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);
                m_LineGauge.Measure(m_objLocalROI.ref_ROI);
                if (!m_blnExecuted)
                    m_blnExecuted = true;
                //objROI.Dispose();
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }
        }

        public void Measure_SubGauge(ImageDrawing objImage, float fMinScore)
        {
            try
            {
                m_blnReferSubValue = false;
                m_objLocalROI.AttachImage(objImage);
                m_objLocalROI.LoadROISetting(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);
                m_LineGauge.Measure(m_objLocalROI.ref_ROI);

                if (m_SubLineGauge == null)
                    m_SubLineGauge = new ELineGauge();

                m_SubLineGauge.Tolerance = m_LineGauge.Tolerance;
                m_SubLineGauge.Length = m_LineGauge.Length;
                m_SubLineGauge.SetCenterXY(m_LineGauge.CenterX, m_LineGauge.CenterY);
                m_SubLineGauge.Angle = m_LineGauge.Angle;

                m_SubLineGauge.MinAmplitude = m_LineGauge.MinAmplitude;
                m_SubLineGauge.MinArea = m_LineGauge.MinArea;
                m_SubLineGauge.Smoothing = m_LineGauge.Smoothing;
                m_SubLineGauge.Thickness = m_LineGauge.Thickness;
                m_SubLineGauge.NumFilteringPasses = m_LineGauge.NumFilteringPasses;
                m_SubLineGauge.FilteringThreshold = m_LineGauge.FilteringThreshold;
                m_SubLineGauge.Threshold = m_LineGauge.Threshold;
                m_SubLineGauge.TransitionChoice = m_LineGauge.TransitionChoice;
                m_SubLineGauge.TransitionType = m_LineGauge.TransitionType;

                //STTrackLog.WriteLine("------ Start  ----------" + m_LineGauge.Angle.ToString());
                float fToleranceSamplingStep = 3f;
                m_SubLineGauge.Tolerance = fToleranceSamplingStep;
                List<float> arrSubScore = new List<float>();
                List<float> arrSubAngle = new List<float>();

                if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
                {

                    float fStartY = m_LineGauge.CenterY - m_LineGauge.Tolerance;
                    float fEndY = m_LineGauge.CenterY + m_LineGauge.Tolerance;
                    int intHighestScoreIndex = -1;
                    float fHighestScore = 0;
                    float fHigheseScoreCenterY = 0;
                    int intIndex = 0;
                    List<int> arrHighestScoreInddex = new List<int>();
                    List<float> arrHighestScore = new List<float>();
                    List<float> arrHighestScoreCenterY = new List<float>();
                    // ------------------- checking loop timeout ---------------------------------------------------
                    HiPerfTimer timeout = new HiPerfTimer();
                    timeout.Start();

                    do
                    {

                        // ------------------- checking loop timeout ---------------------------------------------------
                        if (timeout.Timing > 10000)
                        {
                            STTrackLog.WriteLine(">>>>>>>>>>>>> time out 906");
                            break;
                        }
                        // ---------------------------------------------------------------------------------------------



                        m_SubLineGauge.SetCenterXY(m_LineGauge.CenterX, fStartY + fToleranceSamplingStep);
                        m_SubLineGauge.Measure(m_objLocalROI.ref_ROI);

                        arrSubScore.Add(m_SubLineGauge.NumValidSamples * 100 / (float)m_SubLineGauge.NumSamples);
                        arrSubAngle.Add(ConvertObjectAngle(ref m_SubLineGauge));

                        if (arrSubScore[intIndex] >= fHighestScore)
                        {
                            intHighestScoreIndex = intIndex;
                            fHighestScore = arrSubScore[intIndex];
                            fHigheseScoreCenterY = fStartY + fToleranceSamplingStep;
                        }

                        int intInsertIndex = arrHighestScore.Count;
                        for (int i = 0; i < arrHighestScore.Count; i++)
                        {
                            if (arrSubScore[intIndex] > arrHighestScore[i])
                            {
                                intInsertIndex = i;
                                break;
                            }
                        }

                        if (arrSubScore[intIndex] > fMinScore)
                        {
                            arrHighestScore.Insert(intInsertIndex, arrSubScore[intIndex]);
                            arrHighestScoreCenterY.Insert(intInsertIndex, fStartY + fToleranceSamplingStep);
                            arrHighestScoreInddex.Insert(intInsertIndex, intIndex);
                        }

                        //m_SubLineGauge.Save("D:\\TS\\ESubLineGauge" + fStartY.ToString() + ".CAL");
                        //STTrackLog.WriteLine(arrSubScore[arrSubScore.Count - 1] + ", " + arrSubAngle[arrSubAngle.Count - 1]);

                        if ((fStartY + fToleranceSamplingStep * 2) >= fEndY)
                            break;
                        else
                            fStartY += fToleranceSamplingStep;

                        intIndex++;

                    } while (true);
                    timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

                    if (intHighestScoreIndex >= 0)
                    {
                        int intSelectedIndex = 0;
                        // Looking priority line from top to bottom
                        //if (arrHighestScoreCenterY.Count > 0 && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromBegin)
                        if (arrHighestScoreCenterY.Count > 0 &&
                           (((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromBegin) ||   // 0 deg mean going down. 
                            ((m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromEnd)))     // 180 deg mean going up
                        {

                            float fBeginPosition = float.MaxValue;
                            for (int i = 0; i < arrHighestScoreCenterY.Count; i++)
                            {
                                if (fBeginPosition > arrHighestScoreCenterY[i])
                                {
                                    intSelectedIndex = i;
                                    fBeginPosition = arrHighestScoreCenterY[i];
                                }
                            }

                            m_SubLineGauge.SetCenterXY(m_LineGauge.CenterX, arrHighestScoreCenterY[intSelectedIndex]);
                        }
                        //else if (arrHighestScoreCenterY.Count > 0 && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromEnd)
                        else if (arrHighestScoreCenterY.Count > 0 &&
                           (((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromEnd) ||
                            ((m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromBegin)))

                        {

                            float fEndPosition = float.MinValue;
                            for (int i = 0; i < arrHighestScoreCenterY.Count; i++)
                            {
                                if (fEndPosition < arrHighestScoreCenterY[i])
                                {
                                    intSelectedIndex = i;
                                    fEndPosition = arrHighestScoreCenterY[i];
                                }
                            }

                            m_SubLineGauge.SetCenterXY(m_LineGauge.CenterX, arrHighestScoreCenterY[intSelectedIndex]);
                        }
                        else
                        {
                            m_SubLineGauge.SetCenterXY(m_LineGauge.CenterX, fHigheseScoreCenterY);
                        }



                        m_SubLineGauge.Measure(m_objLocalROI.ref_ROI);
                        m_fSubGaugeAngle = m_SubLineGauge.MeasuredLine.Angle;
                        m_blnReferSubValue = true;
                    }
                    else
                    {
                        m_blnReferSubValue = false;
                    }

                }
                else
                {
                    float fStartX = m_LineGauge.CenterX - m_LineGauge.Tolerance;
                    float fEndX = m_LineGauge.CenterX + m_LineGauge.Tolerance;
                    int intHighestScoreIndex = -1;
                    float fHighestScore = 0;
                    float fHigheseScoreCenterX = 0;
                    int intIndex = 0;
                    List<int> arrHighestScoreInddex = new List<int>();
                    List<float> arrHighestScore = new List<float>();
                    List<float> arrHighestScoreCenterX = new List<float>();
                    // ------------------- checking loop timeout ---------------------------------------------------
                    HiPerfTimer timeout = new HiPerfTimer();
                    timeout.Start();

                    do
                    {

                        // ------------------- checking loop timeout ---------------------------------------------------
                        if (timeout.Timing > 10000)
                        {
                            STTrackLog.WriteLine(">>>>>>>>>>>>> time out 905");
                            break;
                        }
                        // ---------------------------------------------------------------------------------------------



                        m_SubLineGauge.SetCenterXY(fStartX + fToleranceSamplingStep, m_LineGauge.CenterY);
                        m_SubLineGauge.Measure(m_objLocalROI.ref_ROI);

                        arrSubScore.Add(m_SubLineGauge.NumValidSamples * 100 / (float)m_SubLineGauge.NumSamples);
                        arrSubAngle.Add(ConvertObjectAngle(ref m_SubLineGauge));

                        if (arrSubScore[intIndex] >= fHighestScore)
                        {
                            intHighestScoreIndex = intIndex;
                            fHighestScore = arrSubScore[intIndex];
                            fHigheseScoreCenterX = fStartX + fToleranceSamplingStep;
                        }

                        int intInsertIndex = arrHighestScore.Count;
                        for (int i = 0; i < arrHighestScore.Count; i++)
                        {
                            if (arrSubScore[intIndex] > arrHighestScore[i])
                            {
                                intInsertIndex = i;
                                break;
                            }
                        }

                        if (arrSubScore[intIndex] > fMinScore)
                        {
                            arrHighestScore.Insert(intInsertIndex, arrSubScore[intIndex]);
                            arrHighestScoreCenterX.Insert(intInsertIndex, fStartX + fToleranceSamplingStep);
                            arrHighestScoreInddex.Insert(intInsertIndex, intIndex);
                        }

                        //STTrackLog.WriteLine(arrSubScore[arrSubScore.Count - 1] + ", " + arrSubAngle[arrSubAngle.Count - 1]);

                        if ((fStartX + fToleranceSamplingStep * 2) >= fEndX)
                            break;
                        else
                            fStartX += fToleranceSamplingStep;

                        intIndex++;

                    } while (true);
                    timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

                    if (intHighestScoreIndex >= 0)
                    {
                        int intSelectedIndex = 0;
                        // Looking priority line from left to right
                        //if (arrHighestScoreCenterX.Count > 0 && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromBegin)
                        if (arrHighestScoreCenterX.Count > 0 &&
                            (((m_LineGauge.Angle > -135 && m_LineGauge.Angle < -45) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromBegin) ||    // -90 mean from left to right
                            ((m_LineGauge.Angle > 45 && m_LineGauge.Angle < 135) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromEnd)))          // 90 deg mean from right to left
                        {

                            float fBeginPosition = float.MaxValue;
                            for (int i = 0; i < arrHighestScoreCenterX.Count; i++)
                            {
                                if (fBeginPosition > arrHighestScoreCenterX[i])
                                {
                                    intSelectedIndex = i;
                                    fBeginPosition = arrHighestScoreCenterX[i];
                                }
                            }

                            m_SubLineGauge.SetCenterXY(arrHighestScoreCenterX[intSelectedIndex], m_LineGauge.CenterX);
                        }
                        //else if (arrHighestScoreCenterX.Count > 0 && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromEnd)
                        else if (arrHighestScoreCenterX.Count > 0 &&
                            (((m_LineGauge.Angle > -135 && m_LineGauge.Angle < -45) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromEnd) ||     // -90 mean from left to right
                            ((m_LineGauge.Angle > 45 && m_LineGauge.Angle < 135) && m_SubLineGauge.TransitionChoice == ETransitionChoice.NthFromBegin)))          // 90 deg mean from right to left

                        {

                            float fEndPosition = float.MinValue;
                            for (int i = 0; i < arrHighestScoreCenterX.Count; i++)
                            {
                                if (fEndPosition < arrHighestScoreCenterX[i])
                                {
                                    intSelectedIndex = i;
                                    fEndPosition = arrHighestScoreCenterX[i];
                                }
                            }

                            m_SubLineGauge.SetCenterXY(arrHighestScoreCenterX[intSelectedIndex], m_LineGauge.CenterX);
                        }
                        else
                        {
                            m_SubLineGauge.SetCenterXY(fHigheseScoreCenterX, m_LineGauge.CenterY);
                        }

                        m_SubLineGauge.Measure(m_objLocalROI.ref_ROI);
                        m_fSubGaugeAngle = m_SubLineGauge.MeasuredLine.Angle;
                        m_blnReferSubValue = true;
                    }
                    else
                    {
                        m_blnReferSubValue = false;
                    }
                }

                //STTrackLog.WriteLine("------ End ----------");

                if (!m_blnExecuted)
                    m_blnExecuted = true;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Measure line gauge and draw center point 
        /// </summary>
        /// <param name="g">Destination to draw image</param>
        /// <param name="objROI">ROI</param>
        public void Measure(Graphics g, ROI objROI)
        {
            m_blnReferSubValue = false;
            m_LineGauge.Measure(objROI.ref_ROI);

            int intCenterX = Convert.ToInt32(m_LineGauge.MeasuredLine.CenterX);
            int intCenterY = Convert.ToInt32(m_LineGauge.MeasuredLine.CenterY);

            g.DrawLine(new Pen(m_BrushMatched, 1), new System.Drawing.Point(intCenterX - 2, intCenterY - 2),
                new System.Drawing.Point(intCenterX + 2, intCenterY + 2));
            g.DrawLine(new Pen(m_BrushMatched, 1), new System.Drawing.Point(intCenterX + 2, intCenterY - 2),
                 new System.Drawing.Point(intCenterX - 2, intCenterY + 2));

            if (!m_blnExecuted)
                m_blnExecuted = true;
        }

        /// <summary>
        /// When ROI size is changed, modify gauge tolerance and position as well
        /// </summary>
        /// <param name="objTrainROI">parent to gauge</param> 
        public void ModifyGauge(ROI objSearchROI)
        {
            int intROIWidth = objSearchROI.ref_ROIWidth;

            m_LineGauge.Tolerance = intROIWidth;

            //will set according to ROI source to place gauge position and size
            float fCenterX = objSearchROI.ref_ROIPositionX + (intROIWidth / 2f);

            //set gauge position
            m_LineGauge.SetCenterXY(fCenterX, m_LineGauge.Center.Y);

            // -------------- sub gauge -------------------------
            m_SubLineGauge.Tolerance = intROIWidth;

            //set gauge position
            m_SubLineGauge.SetCenterXY(fCenterX, m_SubLineGauge.Center.Y);

        }

        public void Reset()
        {
            m_blnExecuted = false;
        }
        /// <summary>
        /// Set line gauge advanced settings
        /// </summary>
        /// <param name="intMinAmp">min amplitude</param>
        /// <param name="intMinArea">min area</param>
        /// <param name="intFilter">Filter/Smoothing</param>
        /// <param name="intThickness">thickness</param>
        /// /// <param name="intTransChoice">transition choice at edge : 0 = NthFromBegin, 
        /// 1 = NthFromEnd, 2 = LargestArea, 3 = LargestAmplitude, 4 = Closest, 5 = All</param>
        // <param name="intTransChoice">transition choice at edge : 0 = All, 1 = NthFromBegin, 
        // 2 = NthFromEnd, 3 = LargestArea, 4 = LargestAmplitude, 5 = Closest</param>
        /// <param name="intFilterPass"> the number of filtering passes for a line fitting operation.</param>

        public void SetGaugeAdvancedSetting(int intMinAmp, int intMinArea, int intFilter, int intThickness, int intTransChoice,
            int intTransType, int intFilterPass, int intThreshold)
        {
#if (Debug_2_12 || Release_2_12)
            m_LineGauge.MinAmplitude = (uint)intMinAmp;
            m_LineGauge.MinArea = (uint)intMinArea;
            m_LineGauge.Smoothing = (uint)intFilter;
            m_LineGauge.Thickness = (uint)intThickness;
            m_LineGauge.NumFilteringPasses = (uint)intFilterPass;
            m_LineGauge.Threshold = (uint)intThreshold;
            m_LineGauge.TransitionChoice = (ETransitionChoice)intTransChoice;
            m_LineGauge.TransitionType = (ETransitionType)intTransType;

            // -------------- sub gauge -------------------------
            m_SubLineGauge.MinAmplitude = (uint)intMinAmp;
            m_SubLineGauge.MinArea = (uint)intMinArea;
            m_SubLineGauge.Smoothing = (uint)intFilter;
            m_SubLineGauge.Thickness = (uint)intThickness;
            m_SubLineGauge.NumFilteringPasses = (uint)intFilterPass;
            m_SubLineGauge.Threshold = (uint)intThreshold;
            m_SubLineGauge.TransitionChoice = (ETransitionChoice)intTransChoice;
            m_SubLineGauge.TransitionType = (ETransitionType)intTransType;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_LineGauge.MinAmplitude = intMinAmp;
            m_LineGauge.MinArea = intMinArea;
            m_LineGauge.Smoothing = intFilter;
            m_LineGauge.Thickness = intThickness;
            m_LineGauge.NumFilteringPasses = intFilterPass;
            m_LineGauge.Threshold = intThreshold;
            m_LineGauge.TransitionChoice = (ETransitionChoice)intTransChoice;
            m_LineGauge.TransitionType = (ETransitionType)intTransType;

            // -------------- sub gauge -------------------------
            m_SubLineGauge.MinAmplitude = intMinAmp;
            m_SubLineGauge.MinArea = intMinArea;
            m_SubLineGauge.Smoothing = intFilter;
            m_SubLineGauge.Thickness = intThickness;
            m_SubLineGauge.NumFilteringPasses = intFilterPass;
            m_SubLineGauge.Threshold = intThreshold;
            m_SubLineGauge.TransitionChoice = (ETransitionChoice)intTransChoice;
            m_SubLineGauge.TransitionType = (ETransitionType)intTransType;
#endif

        }

        public void SetGaugeAdvancedSetting(int intMinAmp, int intMinArea, int intFilter, int intThickness, int intTransChoice,
                    int intTransType, int intFilterPass, int intThreshold, float fFilterThreshold)
        {
#if (Debug_2_12 || Release_2_12)
            m_LineGauge.MinAmplitude = (uint)intMinAmp;
            m_LineGauge.MinArea = (uint)intMinArea;
            m_LineGauge.Smoothing = (uint)intFilter;
            m_LineGauge.Thickness = (uint)intThickness;
            m_LineGauge.NumFilteringPasses = (uint)intFilterPass;
            m_LineGauge.FilteringThreshold = fFilterThreshold;
            m_LineGauge.Threshold = (uint)intThreshold;
            m_LineGauge.TransitionChoice = (ETransitionChoice)intTransChoice;
            m_LineGauge.TransitionType = (ETransitionType)intTransType;

            // -------------- sub gauge -------------------------
            m_SubLineGauge.MinAmplitude = (uint)intMinAmp;
            m_SubLineGauge.MinArea = (uint)intMinArea;
            m_SubLineGauge.Smoothing = (uint)intFilter;
            m_SubLineGauge.Thickness = (uint)intThickness;
            m_SubLineGauge.NumFilteringPasses = (uint)intFilterPass;
            m_SubLineGauge.FilteringThreshold = fFilterThreshold;
            m_SubLineGauge.Threshold = (uint)intThreshold;
            m_SubLineGauge.TransitionChoice = (ETransitionChoice)intTransChoice;
            m_SubLineGauge.TransitionType = (ETransitionType)intTransType;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_LineGauge.MinAmplitude = intMinAmp;
            m_LineGauge.MinArea = intMinArea;
            m_LineGauge.Smoothing = intFilter;
            m_LineGauge.Thickness = intThickness;
            m_LineGauge.NumFilteringPasses = intFilterPass;
            m_LineGauge.FilteringThreshold = fFilterThreshold;
            m_LineGauge.Threshold = intThreshold;
            m_LineGauge.TransitionChoice = (ETransitionChoice)intTransChoice;
            m_LineGauge.TransitionType = (ETransitionType)intTransType;

            // -------------- sub gauge -------------------------
            m_SubLineGauge.MinAmplitude = intMinAmp;
            m_SubLineGauge.MinArea = intMinArea;
            m_SubLineGauge.Smoothing = intFilter;
            m_SubLineGauge.Thickness = intThickness;
            m_SubLineGauge.NumFilteringPasses = intFilterPass;
            m_SubLineGauge.FilteringThreshold = fFilterThreshold;
            m_SubLineGauge.Threshold = intThreshold;
            m_SubLineGauge.TransitionChoice = (ETransitionChoice)intTransChoice;
            m_SubLineGauge.TransitionType = (ETransitionType)intTransType;
#endif

        }

        public void SetGaugeAngle(float fAngle)
        {
            m_LineGauge.Angle = fAngle;

            // -------------- sub gauge -------------------------
            m_SubLineGauge.Angle = fAngle;
        }

        /// <summary>
        /// Set line gauge placement
        /// </summary>
        /// <param name="objSearchROI">search ROI</param>
        /// <param name="intLength">length</param>
        /// <param name="intPositionX">start X</param>
        /// <param name="intCount">quarter count, eg, 1 = first quarter, 3 = third quarter</param>
        public void SetGaugePlacement(ROI objSearchROI, int intLength, int intPositionX, int intCount)
        {
            int intROIHeight = objSearchROI.ref_ROIHeight;

            m_LineGauge.Tolerance = intROIHeight / 3f;
            m_LineGauge.Length = intLength;

            //will set according to ROI source to place gauge position and size
            float fCenterX = intPositionX + (intLength / 2f);
            float fCenterY = objSearchROI.ref_ROIPositionY + (m_LineGauge.Tolerance * intCount);

            //set gauge position
            m_LineGauge.SetCenterXY(fCenterX, fCenterY);

            // -------------- sub gauge -------------------------

            m_SubLineGauge.Tolerance = intROIHeight / 3f;
            m_SubLineGauge.Length = intLength;

            //set gauge position
            m_SubLineGauge.SetCenterXY(fCenterX, fCenterY);

        }

        /// <summary>
        /// Set line gauge placement without specify the line gauge length
        /// </summary>
        /// <param name="objSearchROI">search ROI</param>
        /// <param name="intCount">quarter count, eg, 1 = first quarter, 3 = third quarter</param>
        public void SetGaugePlacement2(ROI objSearchROI, int intCount)
        {
            m_LineGauge.Tolerance = objSearchROI.ref_ROIHeight / 4f;
            m_LineGauge.Length = objSearchROI.ref_ROIWidth;

            float fX = objSearchROI.ref_ROIPositionX + (m_LineGauge.Length / 2);
            float fY = objSearchROI.ref_ROIPositionY + (m_LineGauge.Tolerance * intCount);

            m_LineGauge.SetCenterXY(fX, fY);

            // -------------- sub gauge -------------------------
            m_SubLineGauge.Tolerance = objSearchROI.ref_ROIHeight / 4f;
            m_SubLineGauge.Length = objSearchROI.ref_ROIWidth;

            m_SubLineGauge.SetCenterXY(fX, fY);
        }

        /// <summary>
        /// Set line gauge center point
        /// </summary>
        /// <param name="fCenterY">offset center Y</param>
        public void SetGaugeCenter(float fCenterY)
        {
            m_LineGauge.SetCenterXY(m_LineGauge.Center.X, fCenterY);

            // -------------- sub gauge -------------------------
            m_SubLineGauge.SetCenterXY(m_LineGauge.Center.X, fCenterY);
        }

        public void SetGaugeCenter(float fCenterX, float fCenterY)
        {
            m_LineGauge.SetCenterXY(fCenterX, fCenterY);

            // -------------- sub gauge -------------------------
            m_SubLineGauge.SetCenterXY(fCenterX, fCenterY);
        }

        public void SetGaugeLength(float fLength)
        {
            //m_LineGauge.Save("D:\\TS\\LineGauge3.CAL");
            m_LineGauge.Length = fLength;

            // -------------- sub gauge -------------------------
            m_SubLineGauge.Length = fLength;
        }

        public void SetGaugeTolerance(float fTolerance)
        {
            m_LineGauge.Tolerance = fTolerance;

            // -------------- sub gauge -------------------------
            m_SubLineGauge.Tolerance = fTolerance;
        }
        /// <summary>
        /// Set line gauge placement
        /// </summary>
        /// <param name="fCenterX">center X</param>
        /// <param name="fCenterY">center Y</param>
        /// <param name="fTolerance">tolerance</param>
        /// <param name="fLength">length</param>
        /// <param name="intAngle">angle</param>
        public void SetGaugePlacement(float fCenterX, float fCenterY, float fTolerance, float fLength, float fAngle)
        {
            m_LineGauge.SetCenterXY(fCenterX, fCenterY);
            m_LineGauge.Tolerance = fTolerance;
            m_LineGauge.Length = fLength;
            m_LineGauge.Angle = fAngle;

            // -------------- sub gauge -------------------------
            m_SubLineGauge.SetCenterXY(fCenterX, fCenterY);
            m_SubLineGauge.Tolerance = fTolerance;
            m_SubLineGauge.Length = fLength;
            m_SubLineGauge.Angle = fAngle;
        }

        /// <summary>
        /// Set line gauge placement
        /// </summary>
        /// <param name="objSearchROI">search ROI</param>
        public void SetGaugePlacement(ROI objSearchROI)
        {
            float fCenterX = objSearchROI.ref_ROIPositionX + objSearchROI.ref_ROIWidth / 2;
            float fCenterY = objSearchROI.ref_ROIPositionY + objSearchROI.ref_ROIHeight / 2;
            m_LineGauge.SetCenterXY(fCenterX, fCenterY);
            if ((float)Math.Round(m_LineGauge.Angle, 0) == 90 || (float)Math.Round(m_LineGauge.Angle, 0) == 270)
            {
                m_LineGauge.Tolerance = objSearchROI.ref_ROIWidth / 2;
                m_LineGauge.Length = objSearchROI.ref_ROIHeight;
            }
            else
            {
                m_LineGauge.Tolerance = objSearchROI.ref_ROIHeight / 2;
                m_LineGauge.Length = objSearchROI.ref_ROIWidth;
            }

            // -------------- sub gauge -------------------------
            m_SubLineGauge.SetCenterXY(fCenterX, fCenterY);
            if ((float)Math.Round(m_SubLineGauge.Angle, 0) == 90 || (float)Math.Round(m_SubLineGauge.Angle, 0) == 270)
            {
                m_SubLineGauge.Tolerance = objSearchROI.ref_ROIWidth / 2;
                m_SubLineGauge.Length = objSearchROI.ref_ROIHeight;
            }
            else
            {
                m_SubLineGauge.Tolerance = objSearchROI.ref_ROIHeight / 2;
                m_SubLineGauge.Length = objSearchROI.ref_ROIWidth;
            }
        }

        /// <summary>
        /// Set gauge measurement setting
        /// </summary>
        /// <param name="rectGTransType">transition type at edge : Black to White(Bw) = 0, White to Black(Wb) = 1, Black to White / White to Black(BwOrWb) = 2, 
        /// Bwb = 3, Wbw = 4</param>
        /// <param name="rectGTransChoice">transition choice at edge : 0 = All, 1 = NthFromBegin, 
        /// 2 = NthFromEnd, 3 = LargestArea, 4 = LargestAmplitude, 5 = Closest</param>
        public void SetGaugeMeasurement(int intTrasitionType)
        {
            m_LineGauge.TransitionType = (ETransitionType)intTrasitionType;

            // -------------- sub gauge -------------------------

            m_SubLineGauge.TransitionType = (ETransitionType)intTrasitionType;
        }

        public float AddGrayColorToGaugePoint(ROI objROI, int intPixelValue, int intDirection, int intThreshold)
        {
            int intX, intY;
            EBW8 objBW8 = new EBW8();
            objBW8.Value = (byte)intPixelValue;
            TrackLog objTL = new TrackLog();
            List<EPoint> arrP = new List<EPoint>();
            for (int i = 0; i < m_LineGauge.NumSamples - 1; i++)
            {
#if (Debug_2_12 || Release_2_12)
                m_LineGauge.MeasureSample(objROI.ref_ROI, (uint)i);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                m_LineGauge.MeasureSample(objROI.ref_ROI, i);

#endif
                
                if (m_LineGauge.Valid)
                {
                    arrP.Add(m_LineGauge.GetMeasuredPoint(0).Center);
                }
            }

            if (intDirection == 0 || intDirection == 3)
            {
                for (int i = 0; i < arrP.Count; i++)
                {
                    intX = (int)Math.Floor(arrP[i].X);
                    intY = (int)Math.Floor(arrP[i].Y);

                    if (intDirection == 0)
                    {
                        if (objROI.ref_ROI.TopParent.GetPixel(intX, intY).Value > intThreshold)
                        {
                            objROI.ref_ROI.TopParent.SetPixel(objBW8, intX, intY);

                            if (objROI.ref_ROI.TopParent.GetPixel(intX, intY + 3).Value <= intThreshold)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    if (objROI.ref_ROI.TopParent.GetPixel(intX, ++intY).Value > intThreshold)
                                    {
                                        objROI.ref_ROI.TopParent.SetPixel(objBW8, intX, intY);
                                    }
                                    else
                                        break;
                                }
                            }
                            else
                            {
                                objROI.ref_ROI.TopParent.SetPixel(objBW8, intX, intY - 1);
                            }
                        }
                    }
                    else
                    {
                        if (objROI.ref_ROI.TopParent.GetPixel(intX, intY).Value > intThreshold)
                        {
                            objROI.ref_ROI.TopParent.SetPixel(objBW8, intX, intY);

                            if (objROI.ref_ROI.TopParent.GetPixel(intX + 3, intY).Value <= intThreshold)
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    if (objROI.ref_ROI.TopParent.GetPixel(++intX, intY).Value > intThreshold)
                                    {
                                        objROI.ref_ROI.TopParent.SetPixel(objBW8, intX, intY);
                                    }
                                    else
                                        break;
                                }
                            }
                            else
                            {
                                objROI.ref_ROI.TopParent.SetPixel(objBW8, intX - 1, intY);
                            }
                        }

                    }

                }
            }
            else
            {
                for (int i = 0; i < arrP.Count; i++)
                {
                    intX = (int)Math.Ceiling(arrP[i].X);
                    intY = (int)Math.Ceiling(arrP[i].Y);

                    if (objROI.ref_ROI.TopParent.GetPixel(intX, intY).Value > intThreshold)
                    {
                        objROI.ref_ROI.TopParent.SetPixel(objBW8, intX, intY);

                        if (intDirection == 2)
                        {
                            if (objROI.ref_ROI.TopParent.GetPixel(intX, intY - 3).Value <= intThreshold)
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    if (objROI.ref_ROI.TopParent.GetPixel(intX, --intY).Value > intThreshold)
                                    {
                                        objROI.ref_ROI.TopParent.SetPixel(objBW8, intX, intY);
                                    }
                                    else
                                        break;
                                }
                            }
                            else
                            {
                                objROI.ref_ROI.TopParent.SetPixel(objBW8, intX, intY + 1);
                            }
                        }
                        else
                        {
                            if (objROI.ref_ROI.TopParent.GetPixel(intX - 3, intY).Value <= intThreshold)
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    if (objROI.ref_ROI.TopParent.GetPixel(--intX, intY).Value > intThreshold)
                                    {
                                        objROI.ref_ROI.TopParent.SetPixel(objBW8, intX, intY);
                                    }
                                    else
                                        break;
                                }
                            }
                            else
                            {
                                objROI.ref_ROI.TopParent.SetPixel(objBW8, intX + 1, intY);
                            }
                        }
                    }

                }
            }
            return 0;
        }

        public void GetMeasurementSamplePoints(ROI objROI, ref List<EPoint> arrMeasuredPoints, ref List<bool> arrPointsValid)
        {
            int intNumSamples = (int)m_LineGauge.NumSamples;

            for (int i = 0; i < intNumSamples; i++)
            {
#if (Debug_2_12 || Release_2_12)
                m_LineGauge.MeasureSample(objROI.ref_ROI, (uint)i);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                m_LineGauge.MeasureSample(objROI.ref_ROI, i);

#endif
                if (m_LineGauge.Valid)
                {
                    arrMeasuredPoints.Add(m_LineGauge.GetMeasuredPoint(0).Center);
                    arrPointsValid.Add(true);
                }
                else
                {
                    arrMeasuredPoints.Add(new EPoint(0, 0));
                    arrPointsValid.Add(false);
                }
            }
        }

        public bool GetMeasurementSampleCenterPoints(ROI objROI, int intTolerance, ref PointF pCenterPoint)
        {
            int intNumSamples = (int)m_LineGauge.NumSamples / 2;

            int intNext = 0;
            int i = intNumSamples;
            //while (true)
            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            while (intNext < intNumSamples) // If use while(true), when no valid point, Next index will exceed total number samples and cause index error
            {

                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 904");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

#if (Debug_2_12 || Release_2_12)
                m_LineGauge.MeasureSample(objROI.ref_ROI,(uint)(intNumSamples + intNext));

                if (m_LineGauge.Valid)
                {
                    pCenterPoint = new PointF(m_LineGauge.GetMeasuredPoint(0).X, m_LineGauge.GetMeasuredPoint(0).Y);
                    return true;
                }

                m_LineGauge.MeasureSample(objROI.ref_ROI, (uint)(intNumSamples - intNext));

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                m_LineGauge.MeasureSample(objROI.ref_ROI, intNumSamples + intNext);

                if (m_LineGauge.Valid)
                {
                    pCenterPoint = new PointF(m_LineGauge.GetMeasuredPoint(0).X, m_LineGauge.GetMeasuredPoint(0).Y);
                    return true;
                }

                m_LineGauge.MeasureSample(objROI.ref_ROI, intNumSamples - intNext);

#endif


                if (m_LineGauge.Valid)
                {
                    pCenterPoint = new PointF(m_LineGauge.GetMeasuredPoint(0).X, m_LineGauge.GetMeasuredPoint(0).Y);
                    return true;
                }

                intNext++;
            }
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            return false;
        }

        public bool GetMeasurementSampleDeepestPoints(ROI objROI, int intTolerance, ref PointF pDeepestPoint)
        {
            int intNumSamples = (int)m_LineGauge.NumSamples / 2;
            int intNumsamplesLimit = (int)m_LineGauge.NumSamples;

            int i = intNumSamples;
            float fDeepestValue = -1;
            float fValue;
            EPoint eP;
            switch (m_LineGauge.Angle.ToString())
            {
                case "0":
                    {
                        fDeepestValue = float.MinValue;
                        // ------------------- checking loop timeout ---------------------------------------------------
                        HiPerfTimer timeout = new HiPerfTimer();
                        timeout.Start();

                        while (true)
                        {

                            // ------------------- checking loop timeout ---------------------------------------------------
                            if (timeout.Timing > 10000)
                            {
                                STTrackLog.WriteLine(">>>>>>>>>>>>> time out 903");
                                break;
                            }
                            // ---------------------------------------------------------------------------------------------


#if (Debug_2_12 || Release_2_12)
                            m_LineGauge.MeasureSample(objROI.ref_ROI, (uint)intNumSamples++);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            m_LineGauge.MeasureSample(objROI.ref_ROI, intNumSamples++);

#endif

                            if (m_LineGauge.Valid)
                            {
                                fValue = m_LineGauge.GetMeasuredPoint(0).Y;

                                if (fDeepestValue < fValue) // for top ROI, deepest mean Y is biggest
                                {
                                    fDeepestValue = fValue;
                                    pDeepestPoint = new PointF(m_LineGauge.GetMeasuredPoint(0).X, m_LineGauge.GetMeasuredPoint(0).Y);
                                }
                            }

                            intNumSamples++;
                            if (intNumSamples >= intNumsamplesLimit)
                                break;
                        }
                        timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------
                    }
                    break;
                case "90":
                    {
                        fDeepestValue = float.MaxValue;
                        // ------------------- checking loop timeout ---------------------------------------------------
                        HiPerfTimer timeout = new HiPerfTimer();
                        timeout.Start();

                        while (true)
                        {

                            // ------------------- checking loop timeout ---------------------------------------------------
                            if (timeout.Timing > 10000)
                            {
                                STTrackLog.WriteLine(">>>>>>>>>>>>> time out 902");
                                break;
                            }
                            // ---------------------------------------------------------------------------------------------



#if (Debug_2_12 || Release_2_12)
                            m_LineGauge.MeasureSample(objROI.ref_ROI, (uint)intNumSamples++);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            m_LineGauge.MeasureSample(objROI.ref_ROI, intNumSamples++);

#endif
                            
                            if (m_LineGauge.Valid)
                            {
                                fValue = m_LineGauge.GetMeasuredPoint(0).X;

                                if (fDeepestValue > fValue) // for Left ROI, deepest mean X is smallest
                                {
                                    fDeepestValue = fValue;
                                    pDeepestPoint = new PointF(m_LineGauge.GetMeasuredPoint(0).X, m_LineGauge.GetMeasuredPoint(0).Y);
                                }
                            }

                            intNumSamples++;
                            if (intNumSamples >= intNumsamplesLimit)
                                break;
                        }
                        timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------
                    }
                    break;
                case "180":
                    {
                        fDeepestValue = float.MaxValue;
                        // ------------------- checking loop timeout ---------------------------------------------------
                        HiPerfTimer timeout = new HiPerfTimer();
                        timeout.Start();

                        while (true)
                        {

                            // ------------------- checking loop timeout ---------------------------------------------------
                            if (timeout.Timing > 10000)
                            {
                                STTrackLog.WriteLine(">>>>>>>>>>>>> time out 901");
                                break;
                            }
                            // ---------------------------------------------------------------------------------------------

#if (Debug_2_12 || Release_2_12)
                            m_LineGauge.MeasureSample(objROI.ref_ROI, (uint)intNumSamples++);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            m_LineGauge.MeasureSample(objROI.ref_ROI, intNumSamples++);

#endif
                            if (m_LineGauge.Valid)
                            {
                                fValue = m_LineGauge.GetMeasuredPoint(0).Y;

                                if (fDeepestValue > fValue) // for Bottom ROI, deepest mean Y is smallest
                                {
                                    fDeepestValue = fValue;
                                    pDeepestPoint = new PointF(m_LineGauge.GetMeasuredPoint(0).X, m_LineGauge.GetMeasuredPoint(0).Y);
                                }
                            }

                            intNumSamples++;
                            if (intNumSamples >= intNumsamplesLimit)
                                break;
                        }
                        timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------
                    }
                    break;
                case "-90":
                    {
                        fDeepestValue = float.MinValue;
                        // ------------------- checking loop timeout ---------------------------------------------------
                        HiPerfTimer timeout = new HiPerfTimer();
                        timeout.Start();

                        while (true)
                        {

                            // ------------------- checking loop timeout ---------------------------------------------------
                            if (timeout.Timing > 10000)
                            {
                                STTrackLog.WriteLine(">>>>>>>>>>>>> time out 900");
                                break;
                            }
                            // ---------------------------------------------------------------------------------------------


#if (Debug_2_12 || Release_2_12)
                            m_LineGauge.MeasureSample(objROI.ref_ROI, (uint)intNumSamples++);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                            m_LineGauge.MeasureSample(objROI.ref_ROI, intNumSamples++);

#endif
                            if (m_LineGauge.Valid)
                            {
                                fValue = m_LineGauge.GetMeasuredPoint(0).X;

                                if (fDeepestValue < fValue) // for Left ROI, deepest mean X is biggest
                                {
                                    fDeepestValue = fValue;
                                    pDeepestPoint = new PointF(m_LineGauge.GetMeasuredPoint(0).X, m_LineGauge.GetMeasuredPoint(0).Y);
                                }
                            }

                            intNumSamples++;
                            if (intNumSamples >= intNumsamplesLimit)
                                break;
                        }
                        timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------
                    }
                    break;
                default:
                    return false;
            }

            return true;
        }

        public void GetMeasurementValidSamplePoints(ROI objROI, ref List<EPoint> arrMeasuredValidPoints)
        {
            int intNumSamples = (int)m_LineGauge.NumSamples;

            List<EPoint> arrP = new List<EPoint>();
            for (int i = 0; i < intNumSamples; i++)
            {
#if (Debug_2_12 || Release_2_12)
                m_LineGauge.MeasureSample(objROI.ref_ROI, (uint)i);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                m_LineGauge.MeasureSample(objROI.ref_ROI, i);

#endif

                if (m_LineGauge.Valid)
                {
                    arrMeasuredValidPoints.Add(m_LineGauge.GetMeasuredPoint(0).Center);
                }
            }
        }

        public void Dispose()
        {
            m_BrushMatched.Dispose();

            if (m_LineGauge != null)
                m_LineGauge.Dispose();

            if (m_objLocalROI != null)
                m_objLocalROI.Dispose();

            if (m_SubLineGauge != null)
                m_SubLineGauge.Dispose();
        }

        public void Save(string strSavePath)
        {
            m_LineGauge.Save(strSavePath);
        }

        public bool VerifyMeasurement_SubGaugePoints(ROI objROI)
        {
            /*
             * 2020 03 04 - CCENG:
             * Verify Gauge Method:
             * This function is used to verify is normal gauge m_LineGauge measurement point ok or not by using sub gauge.
             * All parameter of line gauge will copy to sub gauge except the tolerance.
             * The sub gauge tolerance will be 1/4 narrow or 10pixel compare to line gauge.
             * The line gauge verification is considered OK if 
             *  - the different angle between line gauge and sub gauge < 1 deg
             *  - the valid point of sub gauge > 90% of line gauge
             * 
             */

            try
            {
                if (m_SubLineGauge == null)
                    m_SubLineGauge = new ELineGauge();

                m_SubLineGauge.Tolerance = Math.Max(10, m_LineGauge.Tolerance / 4);
                m_SubLineGauge.Length = m_LineGauge.Length;
                m_SubLineGauge.SetCenterXY(m_LineGauge.MeasuredLine.CenterX, m_LineGauge.MeasuredLine.CenterY);

                //2020-12-16 ZJYEOH : To take care when line gauge angle is -90 but line gauge measured angle sign not same
                if (m_LineGauge.Angle == -90 && m_LineGauge.MeasuredLine.Angle > 0)
                    m_SubLineGauge.Angle = -m_LineGauge.MeasuredLine.Angle;
                else
                    m_SubLineGauge.Angle = m_LineGauge.MeasuredLine.Angle;

                m_SubLineGauge.MinAmplitude = m_LineGauge.MinAmplitude;
                m_SubLineGauge.MinArea = m_LineGauge.MinArea;
                m_SubLineGauge.Smoothing = m_LineGauge.Smoothing;
                m_SubLineGauge.Thickness = m_LineGauge.Thickness;
                m_SubLineGauge.NumFilteringPasses = m_LineGauge.NumFilteringPasses;
                m_SubLineGauge.FilteringThreshold = m_LineGauge.FilteringThreshold;
                m_SubLineGauge.Threshold = m_LineGauge.Threshold;
                m_SubLineGauge.TransitionChoice = m_LineGauge.TransitionChoice;
                m_SubLineGauge.TransitionType = m_LineGauge.TransitionType;
                m_SubLineGauge.SamplingStep = m_LineGauge.SamplingStep;
                //m_SubLineGauge.Save("D:\\TS\\SubLineGauge.CAL");
                //objROI.ref_ROI.Save("D:\\TS\\objROI.bmp");
                //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent.bmp");
                m_SubLineGauge.Measure(objROI.ref_ROI);
                //m_LineGauge.Save("D:\\TS\\m_LineGauge.CAL");
                //m_LineGauge.Measure(objROI.ref_ROI);

                if (Math.Abs(m_SubLineGauge.MeasuredLine.Angle - m_LineGauge.MeasuredLine.Angle) < 1)
                {
                    if (Math.Abs(m_SubLineGauge.MeasuredLine.CenterX - m_LineGauge.MeasuredLine.CenterX) < 1)
                    {
                        if (Math.Abs(m_SubLineGauge.MeasuredLine.CenterY - m_LineGauge.MeasuredLine.CenterY) < 1)
                        {
                            float fSubLineScore = (float)m_SubLineGauge.NumValidSamples / m_SubLineGauge.NumSamples;
                            float fLineScore = (float)m_LineGauge.NumValidSamples / m_LineGauge.NumSamples;

                            if (fSubLineScore > (fLineScore * 0.75))
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }

            return false;
        }

        public void Measure_SubGaugePoints(ROI objROI, float fMinScore)
        {
            try
            {
                m_blnReferSubValue = false;
                m_LineGauge.Measure(objROI.ref_ROI);

                // Get line gauge points
                List<PointF> arrMeasuredPoints = new List<PointF>();
                List<bool> arrMeasuredPointsValid = new List<bool>();
                GetMeasurementSamplePoints_SubGaugePoints(objROI, ref arrMeasuredPoints, ref arrMeasuredPointsValid);

                // ------------------- Use angle between points to filter gauge points
                // Get angle between points
                //////////List<int> arrIndex = new List<int>();
                //////////List<float> arrAngle = new List<float>();
                //////////List<PointF> arrPoint = new List<PointF>();
                //////////for (int i = 0; i < arrMeasuredPoints.Count - 5; i++)
                //////////{
                //////////    Line line = new Line();
                //////////    line.CalculateLGStraightLine(arrMeasuredPoints[i], arrMeasuredPoints[i + 5]);
                //////////    arrAngle.Add((float)line.ref_dAngle);
                //////////    arrPoint.Add(arrMeasuredPoints[i]);
                //////////    arrIndex.Add(i);
                //////////    //STTrackLog.WriteLine("A3=" + line.ref_dAngle + ", DiffX=" + (arrMeasuredPoints[i].X - arrMeasuredPoints[i + 5].X).ToString());
                //////////}

                //////////// Filter angle with given tolerance 
                //////////List<int> arrSelectedIndex = new List<int>();
                //////////List<float> arrSelectedAngle = new List<float>();
                //////////List<PointF> arrSelectedPoint = new List<PointF>();
                //////////float fTolerance = 3f;    // Fix tolerance to 0.1 because for straight line slope, 0.1f tolerance is the most suitable.
                //////////float mode = Math2.DefineOptimumFloat(arrAngle, fTolerance);

                //////////for (int m = 0; m < arrAngle.Count - 1; m++)
                //////////{
                //////////    if ((arrAngle[m] >= (mode - fTolerance)) && (arrAngle[m] <= (mode + fTolerance)))
                //////////    {
                //////////        arrSelectedIndex.Add(arrIndex[m]);
                //////////        arrSelectedAngle.Add(arrAngle[m]);
                //////////        arrSelectedPoint.Add(arrPoint[m]);
                //////////    }
                //////////}

                // ------------------ Use distance between points to filter gauge point

                // Get distance between points
                List<int> arrIndex = new List<int>();
                List<float> arrDistance = new List<float>();
                List<PointF> arrPoint = new List<PointF>();
                for (int i = 0; i < arrMeasuredPoints.Count - 1; i++)
                {
                    arrDistance.Add(Math2.GetDistanceBtw2Points(arrMeasuredPoints[i], arrMeasuredPoints[i + 1]));
                    arrPoint.Add(arrMeasuredPoints[i]);
                    arrIndex.Add(i);
                    //STTrackLog.WriteLine("A3=" + line.ref_dAngle + ", DiffX=" + (arrMeasuredPoints[i].X - arrMeasuredPoints[i + 5].X).ToString());
                }

                // Filter angle with given tolerance 
                List<int> arrSelectedIndex = new List<int>();
                List<float> arrSelectedAngle = new List<float>();
                List<PointF> arrSelectedPoint = new List<PointF>();
                float fTolerance = 0.5f;    // Fix tolerance to 0.1 because for straight line slope, 0.1f tolerance is the most suitable.
                float mode = Math2.DefineOptimumFloat(arrDistance, fTolerance);

                for (int m = 0; m < arrDistance.Count - 1; m++)
                {
                    if ((arrDistance[m] >= (mode - fTolerance)) && (arrDistance[m] <= (mode + fTolerance)))
                    {
                        arrSelectedIndex.Add(arrIndex[m]);
                        arrSelectedAngle.Add(arrDistance[m]);
                        arrSelectedPoint.Add(arrPoint[m]);
                    }
                }

                // -------------------------------------------------------------------------------

                if (arrSelectedPoint.Count > 1)
                {
                    // Assign points to different group 
                    List<List<int>> arrGroupSelectedIndex = new List<List<int>>();
                    List<List<float>> arrGroupSelectedAngle = new List<List<float>>();
                    List<List<PointF>> arrGroupSelectedPoint = new List<List<PointF>>();
                    float fDistanceLimit = m_LineGauge.SamplingStep / 2;
                    int intGroupIndex = 0;
                    bool blnNewGroup = false;    // start with true
                    arrGroupSelectedAngle.Add(new List<float>());
                    arrGroupSelectedIndex.Add(new List<int>());
                    arrGroupSelectedPoint.Add(new List<PointF>());
                    arrGroupSelectedIndex[intGroupIndex].Add(arrSelectedIndex[0]);
                    arrGroupSelectedAngle[intGroupIndex].Add(arrSelectedAngle[0]);
                    arrGroupSelectedPoint[intGroupIndex].Add(arrSelectedPoint[0]);

                    for (int m = 1; m < arrSelectedPoint.Count; m++)
                    {
                        if (arrSelectedPoint[m].X == 0 || arrSelectedPoint[m].Y == 0)
                            continue;

                        if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
                        {
                            if (Math.Abs(arrSelectedPoint[m - 1].Y - arrSelectedPoint[m].Y) > fDistanceLimit)
                                blnNewGroup = true;
                        }
                        else
                        {
                            if (Math.Abs(arrSelectedPoint[m - 1].X - arrSelectedPoint[m].X) > fDistanceLimit)
                                blnNewGroup = true;
                        }

                        if (blnNewGroup)
                        {
                            arrGroupSelectedAngle.Add(new List<float>());
                            arrGroupSelectedIndex.Add(new List<int>());
                            arrGroupSelectedPoint.Add(new List<PointF>());
                            intGroupIndex++;
                        }

                        arrGroupSelectedIndex[intGroupIndex].Add(arrSelectedIndex[m]);
                        arrGroupSelectedAngle[intGroupIndex].Add(arrSelectedAngle[m]);
                        arrGroupSelectedPoint[intGroupIndex].Add(arrSelectedPoint[m]);

                        blnNewGroup = false;
                    }

                    // ================================================================================================
                    // Get line angle of each group
                    float[] arrGroupLineAngle = new float[arrGroupSelectedPoint.Count];
                    Line[] arrGroupLine = new Line[arrGroupSelectedPoint.Count];
                    for (int i = 0; i < arrGroupSelectedPoint.Count; i++)
                    {
                        arrGroupLine[i] = new Line();
                        arrGroupLine[i].CalculateStraightLine(arrGroupSelectedPoint[i].ToArray());
                        arrGroupLineAngle[i] = (float)arrGroupLine[i].ref_dAngle;
                    }

                    List<List<int>> arrGroupSelectedIndex2 = new List<List<int>>();
                    List<List<float>> arrGroupSelectedAngle2 = new List<List<float>>();
                    List<List<PointF>> arrGroupSelectedPoint2 = new List<List<PointF>>();
                    fDistanceLimit = 5f;
                    for (int g = 0; g < arrGroupLine.Length; g++)
                    {
                        arrGroupSelectedAngle2.Add(new List<float>());
                        arrGroupSelectedIndex2.Add(new List<int>());
                        arrGroupSelectedPoint2.Add(new List<PointF>());

                        for (int m = 0; m < arrGroupSelectedPoint[g].Count; m++)
                        {
                            if (arrGroupSelectedPoint[g][m].X == 0 || arrGroupSelectedPoint[g][m].Y == 0)
                                continue;

                            if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
                            {
                                float fLinePointY = arrGroupLine[g].GetPointY(arrGroupSelectedPoint[g][m].X);
                                if (Math.Abs(fLinePointY - arrGroupSelectedPoint[g][m].Y) <= fDistanceLimit)
                                {
                                    arrGroupSelectedIndex2[g].Add(arrGroupSelectedIndex[g][m]);
                                    arrGroupSelectedAngle2[g].Add(arrGroupSelectedAngle[g][m]);
                                    arrGroupSelectedPoint2[g].Add(arrGroupSelectedPoint[g][m]);
                                }
                            }
                            else
                            {
                                float fLinePointX = arrGroupLine[g].GetPointX(arrGroupSelectedPoint[g][m].Y);
                                float fDistance = Math.Abs(fLinePointX - arrGroupSelectedPoint[g][m].X);
                                if (Math.Abs(fLinePointX - arrGroupSelectedPoint[g][m].X) <= fDistanceLimit)
                                {
                                    arrGroupSelectedIndex2[g].Add(arrGroupSelectedIndex[g][m]);
                                    arrGroupSelectedAngle2[g].Add(arrGroupSelectedAngle[g][m]);
                                    arrGroupSelectedPoint2[g].Add(arrGroupSelectedPoint[g][m]);
                                }
                            }
                        }
                    }

                    arrGroupSelectedAngle.Clear();
                    arrGroupSelectedIndex.Clear();
                    arrGroupSelectedPoint.Clear();
                    arrGroupSelectedAngle = arrGroupSelectedAngle2;
                    arrGroupSelectedIndex = arrGroupSelectedIndex2;
                    arrGroupSelectedPoint = arrGroupSelectedPoint2;

                    // ================================================================================================


                    // Get group index that have highest points
                    int intSelectedGroupIndex = 0;
                    for (int i = 1; i < arrGroupSelectedPoint.Count; i++)
                    {
                        if (arrGroupSelectedPoint[intSelectedGroupIndex].Count < arrGroupSelectedPoint[i].Count)
                            intSelectedGroupIndex = i;
                    }

                    arrSelectedPoint = arrGroupSelectedPoint[intSelectedGroupIndex];

                    // Set the selected group to sub gauge.
                    // Measure using sub gauge and get sub gauge result.
                    if (arrSelectedPoint.Count > 1)
                    {
                        float fStartX = arrSelectedPoint[0].X;
                        float fStartY = arrSelectedPoint[0].Y;
                        float fEndX = arrSelectedPoint[0].X;
                        float fEndY = arrSelectedPoint[0].Y;

                        for (int i = 1; i < arrSelectedPoint.Count; i++)
                        {
                            if (fStartX > arrSelectedPoint[i].X)
                                fStartX = arrSelectedPoint[i].X;

                            if (fStartY > arrSelectedPoint[i].Y)
                                fStartY = arrSelectedPoint[i].Y;

                            if (fEndX < arrSelectedPoint[i].X)
                                fEndX = arrSelectedPoint[i].X;

                            if (fEndY < arrSelectedPoint[i].Y)
                                fEndY = arrSelectedPoint[i].Y;
                        }

                        float fLineGaugeCenterX = (fStartX + fEndX) / 2;
                        float fLineGaugeCenterY = (fStartY + fEndY) / 2;

                        float fLineGaugeTolerance;
                        float fLineGaugeLength;

                        if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
                        {
                            fLineGaugeTolerance = Math.Min(m_LineGauge.Tolerance, (fEndY - fStartY) / 2 + 6);
                            fLineGaugeLength = fEndX - fStartX;
                        }
                        else
                        {
                            fLineGaugeTolerance = Math.Min(m_LineGauge.Tolerance, (fEndX - fStartX) / 2 + 6);
                            fLineGaugeLength = fEndY - fStartY;
                        }
                        if (m_SubLineGauge == null)
                            m_SubLineGauge = new ELineGauge();

                        m_SubLineGauge.Tolerance = fLineGaugeTolerance;// m_LineGauge.Tolerance;
                        m_SubLineGauge.Length = fLineGaugeLength;
                        m_SubLineGauge.SetCenterXY(fLineGaugeCenterX, fLineGaugeCenterY);
                        m_SubLineGauge.Angle = m_LineGauge.Angle;

                        m_SubLineGauge.MinAmplitude = m_LineGauge.MinAmplitude;
                        m_SubLineGauge.MinArea = m_LineGauge.MinArea;
                        m_SubLineGauge.Smoothing = m_LineGauge.Smoothing;
                        m_SubLineGauge.Thickness = m_LineGauge.Thickness;
                        m_SubLineGauge.NumFilteringPasses = m_LineGauge.NumFilteringPasses;
                        m_SubLineGauge.FilteringThreshold = m_LineGauge.FilteringThreshold;
                        m_SubLineGauge.Threshold = m_LineGauge.Threshold;
                        m_SubLineGauge.TransitionChoice = m_LineGauge.TransitionChoice;
                        m_SubLineGauge.TransitionType = m_LineGauge.TransitionType;
                        m_SubLineGauge.SamplingStep = m_LineGauge.SamplingStep;

                        //m_SubLineGauge.Save("D:\\TS\\LG.cal");
                        //objROI.ref_ROI.TopParent.Save("D:\\TS\\TOpP.bmp");
                        m_SubLineGauge.Measure(objROI.ref_ROI);
                        m_fSubGaugeAngle = m_SubLineGauge.MeasuredLine.Angle;
                        m_blnReferSubValue = true;
                    }
                    else
                    {
                        m_blnReferSubValue = false;
                    }
                }
                else
                {
                    m_blnReferSubValue = false;
                }
                //STTrackLog.WriteLine("------ End ----------");

                if (!m_blnExecuted)
                    m_blnExecuted = true;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }
        }
        public void Measure_SubGaugePoints_UsingMainLineGauge(ROI objROI)
        {
            try
            {
                //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent3.bmp");
                //m_LineGauge.Save("D:\\TS\\ELineGauge3.CAL");
                m_LineGauge.Measure(objROI.ref_ROI);

                // Get line gauge points
                List<PointF> arrMeasuredPoints = new List<PointF>();
                List<bool> arrMeasuredPointsValid = new List<bool>();
                GetMeasurementSamplePoints_SubGaugePoints(objROI, ref arrMeasuredPoints, ref arrMeasuredPointsValid);

                // ------------------- Use angle between points to filter gauge points
                // Get angle between points
                //////////List<int> arrIndex = new List<int>();
                //////////List<float> arrAngle = new List<float>();
                //////////List<PointF> arrPoint = new List<PointF>();
                //////////for (int i = 0; i < arrMeasuredPoints.Count - 5; i++)
                //////////{
                //////////    Line line = new Line();
                //////////    line.CalculateLGStraightLine(arrMeasuredPoints[i], arrMeasuredPoints[i + 5]);
                //////////    arrAngle.Add((float)line.ref_dAngle);
                //////////    arrPoint.Add(arrMeasuredPoints[i]);
                //////////    arrIndex.Add(i);
                //////////    //STTrackLog.WriteLine("A3=" + line.ref_dAngle + ", DiffX=" + (arrMeasuredPoints[i].X - arrMeasuredPoints[i + 5].X).ToString());
                //////////}

                //////////// Filter angle with given tolerance 
                //////////List<int> arrSelectedIndex = new List<int>();
                //////////List<float> arrSelectedAngle = new List<float>();
                //////////List<PointF> arrSelectedPoint = new List<PointF>();
                //////////float fTolerance = 3f;    // Fix tolerance to 0.1 because for straight line slope, 0.1f tolerance is the most suitable.
                //////////float mode = Math2.DefineOptimumFloat(arrAngle, fTolerance);

                //////////for (int m = 0; m < arrAngle.Count - 1; m++)
                //////////{
                //////////    if ((arrAngle[m] >= (mode - fTolerance)) && (arrAngle[m] <= (mode + fTolerance)))
                //////////    {
                //////////        arrSelectedIndex.Add(arrIndex[m]);
                //////////        arrSelectedAngle.Add(arrAngle[m]);
                //////////        arrSelectedPoint.Add(arrPoint[m]);
                //////////    }
                //////////}

                // ------------------ Use distance between points to filter gauge point

                // Get distance between points
                List<int> arrIndex = new List<int>();
                List<float> arrDistance = new List<float>();
                List<PointF> arrPoint = new List<PointF>();
                for (int i = 0; i < arrMeasuredPoints.Count - 1; i++)
                {
                    arrDistance.Add(Math2.GetDistanceBtw2Points(arrMeasuredPoints[i], arrMeasuredPoints[i + 1]));
                    arrPoint.Add(arrMeasuredPoints[i]);
                    arrIndex.Add(i);
                    //STTrackLog.WriteLine("A3=" + line.ref_dAngle + ", DiffX=" + (arrMeasuredPoints[i].X - arrMeasuredPoints[i + 5].X).ToString());
                }

                // Filter angle with given tolerance 
                List<int> arrSelectedIndex = new List<int>();
                List<float> arrSelectedAngle = new List<float>();
                List<PointF> arrSelectedPoint = new List<PointF>();
                float fTolerance = 0.5f;    // Fix tolerance to 0.1 because for straight line slope, 0.1f tolerance is the most suitable.
                float mode = Math2.DefineOptimumFloat(arrDistance, fTolerance);

                for (int m = 0; m < arrDistance.Count - 1; m++)
                {
                    if ((arrDistance[m] >= (mode - fTolerance)) && (arrDistance[m] <= (mode + fTolerance)))
                    {
                        arrSelectedIndex.Add(arrIndex[m]);
                        arrSelectedAngle.Add(arrDistance[m]);
                        arrSelectedPoint.Add(arrPoint[m]);
                    }
                }

                // -------------------------------------------------------------------------------

                if (arrSelectedPoint.Count > 1)
                {
                    // Assign points to different group 
                    List<List<int>> arrGroupSelectedIndex = new List<List<int>>();
                    List<List<float>> arrGroupSelectedAngle = new List<List<float>>();
                    List<List<PointF>> arrGroupSelectedPoint = new List<List<PointF>>();
                    float fDistanceLimit = m_LineGauge.SamplingStep / 2;
                    int intGroupIndex = 0;
                    bool blnNewGroup = false;    // start with true
                    arrGroupSelectedAngle.Add(new List<float>());
                    arrGroupSelectedIndex.Add(new List<int>());
                    arrGroupSelectedPoint.Add(new List<PointF>());
                    arrGroupSelectedIndex[intGroupIndex].Add(arrSelectedIndex[0]);
                    arrGroupSelectedAngle[intGroupIndex].Add(arrSelectedAngle[0]);
                    arrGroupSelectedPoint[intGroupIndex].Add(arrSelectedPoint[0]);

                    for (int m = 1; m < arrSelectedPoint.Count; m++)
                    {
                        if (arrSelectedPoint[m].X == 0 || arrSelectedPoint[m].Y == 0)
                            continue;

                        if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
                        {
                            if (Math.Abs(arrSelectedPoint[m - 1].Y - arrSelectedPoint[m].Y) > fDistanceLimit)
                                blnNewGroup = true;
                        }
                        else
                        {
                            if (Math.Abs(arrSelectedPoint[m - 1].X - arrSelectedPoint[m].X) > fDistanceLimit)
                                blnNewGroup = true;
                        }

                        if (blnNewGroup)
                        {
                            arrGroupSelectedAngle.Add(new List<float>());
                            arrGroupSelectedIndex.Add(new List<int>());
                            arrGroupSelectedPoint.Add(new List<PointF>());
                            intGroupIndex++;
                        }

                        arrGroupSelectedIndex[intGroupIndex].Add(arrSelectedIndex[m]);
                        arrGroupSelectedAngle[intGroupIndex].Add(arrSelectedAngle[m]);
                        arrGroupSelectedPoint[intGroupIndex].Add(arrSelectedPoint[m]);

                        blnNewGroup = false;
                    }

                    // ================================================================================================
                    // Get line angle of each group
                    float[] arrGroupLineAngle = new float[arrGroupSelectedPoint.Count];
                    Line[] arrGroupLine = new Line[arrGroupSelectedPoint.Count];
                    for (int i = 0; i < arrGroupSelectedPoint.Count; i++)
                    {
                        arrGroupLine[i] = new Line();
                        arrGroupLine[i].CalculateStraightLine(arrGroupSelectedPoint[i].ToArray());
                        arrGroupLineAngle[i] = (float)arrGroupLine[i].ref_dAngle;
                    }

                    List<List<int>> arrGroupSelectedIndex2 = new List<List<int>>();
                    List<List<float>> arrGroupSelectedAngle2 = new List<List<float>>();
                    List<List<PointF>> arrGroupSelectedPoint2 = new List<List<PointF>>();
                    fDistanceLimit = 1f;
                    for (int g = 0; g < arrGroupLine.Length; g++)
                    {
                        arrGroupSelectedAngle2.Add(new List<float>());
                        arrGroupSelectedIndex2.Add(new List<int>());
                        arrGroupSelectedPoint2.Add(new List<PointF>());

                        for (int m = 0; m < arrGroupSelectedPoint[g].Count; m++)
                        {
                            if (arrGroupSelectedPoint[g][m].X == 0 || arrGroupSelectedPoint[g][m].Y == 0)
                                continue;

                            if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
                            {
                                float fLinePointY = arrGroupLine[g].GetPointY(arrGroupSelectedPoint[g][m].X);
                                if (Math.Abs(fLinePointY - arrGroupSelectedPoint[g][m].Y) <= fDistanceLimit)
                                {
                                    arrGroupSelectedIndex2[g].Add(arrGroupSelectedIndex[g][m]);
                                    arrGroupSelectedAngle2[g].Add(arrGroupSelectedAngle[g][m]);
                                    arrGroupSelectedPoint2[g].Add(arrGroupSelectedPoint[g][m]);
                                }
                            }
                            else
                            {
                                float fLinePointX = arrGroupLine[g].GetPointX(arrGroupSelectedPoint[g][m].Y);
                                float fDistance = Math.Abs(fLinePointX - arrGroupSelectedPoint[g][m].X);
                                if (Math.Abs(fLinePointX - arrGroupSelectedPoint[g][m].X) <= fDistanceLimit)
                                {
                                    arrGroupSelectedIndex2[g].Add(arrGroupSelectedIndex[g][m]);
                                    arrGroupSelectedAngle2[g].Add(arrGroupSelectedAngle[g][m]);
                                    arrGroupSelectedPoint2[g].Add(arrGroupSelectedPoint[g][m]);
                                }
                            }
                        }
                    }

                    arrGroupSelectedAngle.Clear();
                    arrGroupSelectedIndex.Clear();
                    arrGroupSelectedPoint.Clear();
                    arrGroupSelectedAngle = arrGroupSelectedAngle2;
                    arrGroupSelectedIndex = arrGroupSelectedIndex2;
                    arrGroupSelectedPoint = arrGroupSelectedPoint2;

                    // ================================================================================================


                    // Get group index that have highest points
                    int intSelectedGroupIndex = 0;
                    for (int i = 1; i < arrGroupSelectedPoint.Count; i++)
                    {
                        if (arrGroupSelectedPoint[intSelectedGroupIndex].Count < arrGroupSelectedPoint[i].Count)
                            intSelectedGroupIndex = i;
                    }

                    arrSelectedPoint = arrGroupSelectedPoint[intSelectedGroupIndex];

                    // Set the selected group to sub gauge.
                    // Measure using sub gauge and get sub gauge result.
                    if (arrSelectedPoint.Count > 1)
                    {
                        float fStartX = arrSelectedPoint[0].X;
                        float fStartY = arrSelectedPoint[0].Y;
                        float fEndX = arrSelectedPoint[0].X;
                        float fEndY = arrSelectedPoint[0].Y;

                        for (int i = 1; i < arrSelectedPoint.Count; i++)
                        {
                            if (fStartX > arrSelectedPoint[i].X)
                                fStartX = arrSelectedPoint[i].X;

                            if (fStartY > arrSelectedPoint[i].Y)
                                fStartY = arrSelectedPoint[i].Y;

                            if (fEndX < arrSelectedPoint[i].X)
                                fEndX = arrSelectedPoint[i].X;

                            if (fEndY < arrSelectedPoint[i].Y)
                                fEndY = arrSelectedPoint[i].Y;
                        }

                        float fLineGaugeCenterX = (fStartX + fEndX) / 2;
                        float fLineGaugeCenterY = (fStartY + fEndY) / 2;

                        float fLineGaugeTolerance;
                        float fLineGaugeLength;

                        if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
                        {
                            fLineGaugeTolerance = Math.Min(m_LineGauge.Tolerance, (fEndY - fStartY) / 2 + 6);
                            fLineGaugeLength = fEndX - fStartX;
                        }
                        else
                        {
                            fLineGaugeTolerance = Math.Min(m_LineGauge.Tolerance, (fEndX - fStartX) / 2 + 6);
                            fLineGaugeLength = fEndY - fStartY;
                        }

                        m_LineGauge.Tolerance = fLineGaugeTolerance;// m_LineGauge.Tolerance;
                        m_LineGauge.Length = fLineGaugeLength;
                        m_LineGauge.SetCenterXY(fLineGaugeCenterX, fLineGaugeCenterY);
                        m_LineGauge.Angle = m_LineGauge.Angle;

                        m_LineGauge.MinAmplitude = m_LineGauge.MinAmplitude;
                        m_LineGauge.MinArea = m_LineGauge.MinArea;
                        m_LineGauge.Smoothing = m_LineGauge.Smoothing;
                        m_LineGauge.Thickness = m_LineGauge.Thickness;
                        m_LineGauge.NumFilteringPasses = m_LineGauge.NumFilteringPasses;
                        m_LineGauge.FilteringThreshold = m_LineGauge.FilteringThreshold;
                        m_LineGauge.Threshold = m_LineGauge.Threshold;
                        m_LineGauge.TransitionChoice = m_LineGauge.TransitionChoice;
                        m_LineGauge.TransitionType = m_LineGauge.TransitionType;
                        m_LineGauge.SamplingStep = m_LineGauge.SamplingStep;

                        //m_LineGauge.Save("D:\\TS\\LG.cal");
                        //objROI.ref_ROI.TopParent.Save("D:\\TS\\TOpP.bmp");
                        m_LineGauge.Measure(objROI.ref_ROI);
                        m_fSubGaugeAngle = m_LineGauge.MeasuredLine.Angle;
                   
                    }
                    else
                    {
                    
                    }
                }
                else
                {
                   
                }
                //STTrackLog.WriteLine("------ End ----------");

                if (!m_blnExecuted)
                    m_blnExecuted = true;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }
        }
        
        public List<LineGaugeRecord> MeasureAndCollect_SubGaugePoints(ROI objROI, float fMinScore, float fGroupTolerance)
        {
            try
            {
                m_blnReferSubValue = false;
                //m_LineGauge.Save("D:\\TS\\LG100.CAL");
                //objROI.ref_ROI.TopParent.Save("D:\\TS\\TP100.bmp");
                //objROI.ref_ROI.Save("D:\\TS\\ROI100.bmp");
                m_LineGauge.Measure(objROI.ref_ROI);

                // Get line gauge points
                List<PointF> arrMeasuredPoints = new List<PointF>();
                List<bool> arrMeasuredPointsValid = new List<bool>();
                GetMeasurementSamplePoints_SubGaugePoints(objROI, ref arrMeasuredPoints, ref arrMeasuredPointsValid);

                // ------------------- Use angle between points to filter gauge points
                // Get angle between points
                //////////List<int> arrIndex = new List<int>();
                //////////List<float> arrAngle = new List<float>();
                //////////List<PointF> arrPoint = new List<PointF>();
                //////////for (int i = 0; i < arrMeasuredPoints.Count - 5; i++)
                //////////{
                //////////    Line line = new Line();
                //////////    line.CalculateLGStraightLine(arrMeasuredPoints[i], arrMeasuredPoints[i + 5]);
                //////////    arrAngle.Add((float)line.ref_dAngle);
                //////////    arrPoint.Add(arrMeasuredPoints[i]);
                //////////    arrIndex.Add(i);
                //////////    //STTrackLog.WriteLine("A3=" + line.ref_dAngle + ", DiffX=" + (arrMeasuredPoints[i].X - arrMeasuredPoints[i + 5].X).ToString());
                //////////}

                //////////// Filter angle with given tolerance 
                //////////List<int> arrSelectedIndex = new List<int>();
                //////////List<float> arrSelectedAngle = new List<float>();
                //////////List<PointF> arrSelectedPoint = new List<PointF>();
                //////////float fTolerance = 3f;    // Fix tolerance to 0.1 because for straight line slope, 0.1f tolerance is the most suitable.
                //////////float mode = Math2.DefineOptimumFloat(arrAngle, fTolerance);

                //////////for (int m = 0; m < arrAngle.Count - 1; m++)
                //////////{
                //////////    if ((arrAngle[m] >= (mode - fTolerance)) && (arrAngle[m] <= (mode + fTolerance)))
                //////////    {
                //////////        arrSelectedIndex.Add(arrIndex[m]);
                //////////        arrSelectedAngle.Add(arrAngle[m]);
                //////////        arrSelectedPoint.Add(arrPoint[m]);
                //////////    }
                //////////}

                // ------------------ Use distance between points to filter gauge point

                // Get distance between points
                List<int> arrIndex = new List<int>();
                List<float> arrDistance = new List<float>();
                List<PointF> arrPoint = new List<PointF>();
                for (int i = 0; i < arrMeasuredPoints.Count - 1; i++)
                {
                    arrDistance.Add(Math2.GetDistanceBtw2Points(arrMeasuredPoints[i], arrMeasuredPoints[i + 1]));
                    arrPoint.Add(arrMeasuredPoints[i]);
                    arrIndex.Add(i);
                    //STTrackLog.WriteLine("A3=" + line.ref_dAngle + ", DiffX=" + (arrMeasuredPoints[i].X - arrMeasuredPoints[i + 5].X).ToString());
                }

                // Filter angle with given tolerance 
                List<int> arrSelectedIndex = new List<int>();
                List<float> arrSelectedAngle = new List<float>();
                List<PointF> arrSelectedPoint = new List<PointF>();
                float fTolerance = 0.5f;    // Fix tolerance to 0.1 because for straight line slope, 0.1f tolerance is the most suitable.
                float mode = Math2.DefineOptimumFloat(arrDistance, fTolerance);

                for (int m = 0; m < arrDistance.Count - 1; m++)
                {
                    if ((arrDistance[m] >= (mode - fTolerance)) && (arrDistance[m] <= (mode + fTolerance)))
                    {
                        arrSelectedIndex.Add(arrIndex[m]);
                        arrSelectedAngle.Add(arrDistance[m]);
                        arrSelectedPoint.Add(arrPoint[m]);
                    }
                }

                // -------------------------------------------------------------------------------

                // Assign points to different group 
                List<List<int>> arrGroupSelectedIndex = new List<List<int>>();
                List<List<float>> arrGroupSelectedAngle = new List<List<float>>();
                List<List<PointF>> arrGroupSelectedPoint = new List<List<PointF>>();
                float fDistanceLimit = m_LineGauge.SamplingStep / 2;
                if (arrSelectedAngle.Count > 0)
                {
                    int intGroupIndex = 0;
                    bool blnNewGroup = false;    // start with true
                    arrGroupSelectedAngle.Add(new List<float>());
                    arrGroupSelectedIndex.Add(new List<int>());
                    arrGroupSelectedPoint.Add(new List<PointF>());
                    arrGroupSelectedIndex[intGroupIndex].Add(arrSelectedIndex[0]);
                    arrGroupSelectedAngle[intGroupIndex].Add(arrSelectedAngle[0]);
                    arrGroupSelectedPoint[intGroupIndex].Add(arrSelectedPoint[0]);

                    for (int m = 1; m < arrSelectedPoint.Count; m++)
                    {
                        if (arrSelectedPoint[m].X == 0 || arrSelectedPoint[m].Y == 0)
                            continue;

                        if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
                        {
                            if (Math.Abs(arrSelectedPoint[m - 1].Y - arrSelectedPoint[m].Y) > fDistanceLimit)
                                blnNewGroup = true;
                        }
                        else
                        {
                            if (Math.Abs(arrSelectedPoint[m - 1].X - arrSelectedPoint[m].X) > fDistanceLimit)
                                blnNewGroup = true;
                        }

                        if (blnNewGroup)
                        {
                            arrGroupSelectedAngle.Add(new List<float>());
                            arrGroupSelectedIndex.Add(new List<int>());
                            arrGroupSelectedPoint.Add(new List<PointF>());
                            intGroupIndex++;
                        }

                        arrGroupSelectedIndex[intGroupIndex].Add(arrSelectedIndex[m]);
                        arrGroupSelectedAngle[intGroupIndex].Add(arrSelectedAngle[m]);
                        arrGroupSelectedPoint[intGroupIndex].Add(arrSelectedPoint[m]);

                        blnNewGroup = false;
                    }
                }

                // ================================================================================================
                // Get line angle of each group
                float[] arrGroupLineAngle = new float[arrGroupSelectedPoint.Count];
                Line[] arrGroupLine = new Line[arrGroupSelectedPoint.Count];
                for (int i = 0; i < arrGroupSelectedPoint.Count; i++)
                {
                    arrGroupLine[i] = new Line();
                    arrGroupLine[i].CalculateStraightLine(arrGroupSelectedPoint[i].ToArray());
                    arrGroupLineAngle[i] = (float)arrGroupLine[i].ref_dAngle;
                }

                List<List<int>> arrGroupSelectedIndex2 = new List<List<int>>();
                List<List<float>> arrGroupSelectedAngle2 = new List<List<float>>();
                List<List<PointF>> arrGroupSelectedPoint2 = new List<List<PointF>>();
                fDistanceLimit = fGroupTolerance;   // Default 1.5f. The more package edge line no stable, the more big value need to set.
                for (int g = 0; g < arrGroupLine.Length; g++)
                {
                    arrGroupSelectedAngle2.Add(new List<float>());
                    arrGroupSelectedIndex2.Add(new List<int>());
                    arrGroupSelectedPoint2.Add(new List<PointF>());

                    for (int m = 0; m < arrGroupSelectedPoint[g].Count; m++)
                    {
                        if (arrGroupSelectedPoint[g][m].X == 0 || arrGroupSelectedPoint[g][m].Y == 0)
                            continue;

                        if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
                        {
                            float fLinePointY = arrGroupLine[g].GetPointY(arrGroupSelectedPoint[g][m].X);
                            if (Math.Abs(fLinePointY - arrGroupSelectedPoint[g][m].Y) <= fDistanceLimit)
                            {
                                arrGroupSelectedIndex2[g].Add(arrGroupSelectedIndex[g][m]);
                                arrGroupSelectedAngle2[g].Add(arrGroupSelectedAngle[g][m]);
                                arrGroupSelectedPoint2[g].Add(arrGroupSelectedPoint[g][m]);
                            }
                        }
                        else
                        {
                            float fLinePointX = arrGroupLine[g].GetPointX(arrGroupSelectedPoint[g][m].Y);
                            float fDistance = Math.Abs(fLinePointX - arrGroupSelectedPoint[g][m].X);
                            if (Math.Abs(fLinePointX - arrGroupSelectedPoint[g][m].X) <= fDistanceLimit)
                            {
                                arrGroupSelectedIndex2[g].Add(arrGroupSelectedIndex[g][m]);
                                arrGroupSelectedAngle2[g].Add(arrGroupSelectedAngle[g][m]);
                                arrGroupSelectedPoint2[g].Add(arrGroupSelectedPoint[g][m]);
                            }
                        }
                    }
                }

                arrGroupSelectedAngle.Clear();
                arrGroupSelectedIndex.Clear();
                arrGroupSelectedPoint.Clear();
                arrGroupSelectedAngle = arrGroupSelectedAngle2;
                arrGroupSelectedIndex = arrGroupSelectedIndex2;
                arrGroupSelectedPoint = arrGroupSelectedPoint2;

                // ================================================================================================
                // ==================== Get partition group =======================================================
                List<List<int>> arrGroupSelectedIndex3 = new List<List<int>>();
                List<List<float>> arrGroupSelectedAngle3 = new List<List<float>>();
                List<List<PointF>> arrGroupSelectedPoint3 = new List<List<PointF>>();
                if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
                {
                    int intPartition = 2;

                    // 2021 09 29 - CCENG: should not reply on ROI. The ROI size is different during learn and during inspection. This will cause different result as well.
                    //int intPartitionLength = objROI.ref_ROIWidth / intPartition;  
                    int intPartitionLength = (int)m_LineGauge.Length / intPartition;

                    for (int p = 0; p < intPartition; p++)
                    {
                        // 2021 09 29 - CCENG: should not reply on ROI. The ROI size is different during learn and during inspection. This will cause different result as well.
                        //int intPartitionStartX = objROI.ref_ROIPositionX + p * intPartitionLength;
                        //int intPartitionEndX = objROI.ref_ROIPositionX + (p + 1) * intPartitionLength;
                        int intPartitionStartX = ((int)m_LineGauge.CenterX - intPartitionLength) + p * intPartitionLength;
                        int intPartitionEndX = ((int)m_LineGauge.CenterX - intPartitionLength) + (p + 1) * intPartitionLength;

                        for (int g = 0; g < arrGroupSelectedPoint2.Count; g++)
                        {
                            // ignore if less than 2 points
                            if (arrGroupSelectedPoint2[g].Count < 2)
                                continue;

                            // ignore if already have full list in this partition.
                            if (((arrGroupSelectedPoint2[g][0].X >= intPartitionStartX) && (arrGroupSelectedPoint2[g][0].X <= intPartitionEndX)) &&
                                ((arrGroupSelectedPoint2[g][arrGroupSelectedPoint2[g].Count - 1].X >= intPartitionStartX) && (arrGroupSelectedPoint2[g][arrGroupSelectedPoint2[g].Count - 1].X <= intPartitionEndX)))
                                continue;

                            // ignore if non of points in this partition
                            if (!((arrGroupSelectedPoint2[g][0].X >= intPartitionStartX) && (arrGroupSelectedPoint2[g][0].X <= intPartitionEndX)) &&
                                !((arrGroupSelectedPoint2[g][arrGroupSelectedPoint2[g].Count - 1].X >= intPartitionStartX) && (arrGroupSelectedPoint2[g][arrGroupSelectedPoint2[g].Count - 1].X <= intPartitionEndX)))
                                continue;

                            arrGroupSelectedIndex3.Add(new List<int>());
                            arrGroupSelectedAngle3.Add(new List<float>());
                            arrGroupSelectedPoint3.Add(new List<PointF>());
                            int intGroupIndex = arrGroupSelectedPoint3.Count - 1;
                            for (int i = 0; i < arrGroupSelectedPoint2[g].Count; i++)
                            {
                                if (arrGroupSelectedPoint2[g][i].X >= intPartitionStartX && arrGroupSelectedPoint2[g][i].X <= intPartitionEndX)
                                {
                                    arrGroupSelectedIndex3[intGroupIndex].Add(arrGroupSelectedIndex2[g][i]);
                                    arrGroupSelectedAngle3[intGroupIndex].Add(arrGroupSelectedAngle2[g][i]);
                                    arrGroupSelectedPoint3[intGroupIndex].Add(arrGroupSelectedPoint2[g][i]);
                                }
                            }

                            if (arrGroupSelectedPoint3[intGroupIndex].Count == 0)
                            {
                                arrGroupSelectedIndex3.RemoveAt(intGroupIndex);
                                arrGroupSelectedAngle3.RemoveAt(intGroupIndex);
                                arrGroupSelectedPoint3.RemoveAt(intGroupIndex);
                            }

                        }
                    }

                    if (arrGroupSelectedPoint3.Count > 0)
                    {
                        for (int i = 0; i < arrGroupSelectedPoint3.Count; i++)
                        {
                            arrGroupSelectedIndex.Add(arrGroupSelectedIndex3[i]);
                            arrGroupSelectedAngle.Add(arrGroupSelectedAngle3[i]);
                            arrGroupSelectedPoint.Add(arrGroupSelectedPoint3[i]);
                        }
                    }

                }
                else
                {
                    int intPartition = 2;

                    // 2021 09 29 - CCENG: should not reply on ROI. The ROI size is different during learn and during inspection. This will cause different result as well.
                    //int intPartitionLength = objROI.ref_ROIHeight / intPartition;
                    int intPartitionLength = (int)m_LineGauge.Length / intPartition;

                    for (int p = 0; p < intPartition; p++)
                    {
                        // 2021 09 29 - CCENG: should not reply on ROI. The ROI size is different during learn and during inspection. This will cause different result as well.
                        //int intPartitionStartY = objROI.ref_ROIPositionY + p * intPartitionLength;
                        //int intPartitionEndY = objROI.ref_ROIPositionY + (p + 1) * intPartitionLength;
                        int intPartitionStartY = ((int)m_LineGauge.CenterY - intPartitionLength) + p * intPartitionLength;
                        int intPartitionEndY = ((int)m_LineGauge.CenterY - intPartitionLength) + (p + 1) * intPartitionLength;

                        for (int g = 0; g < arrGroupSelectedPoint2.Count; g++)
                        {
                            // ignore if less than 2 points
                            if (arrGroupSelectedPoint2[g].Count < 2)
                                continue;

                            // ignore if already have full list in this partition.
                            if (((arrGroupSelectedPoint2[g][0].Y >= intPartitionStartY) && (arrGroupSelectedPoint2[g][0].Y <= intPartitionEndY)) &&
                                ((arrGroupSelectedPoint2[g][arrGroupSelectedPoint2[g].Count - 1].Y >= intPartitionStartY) && (arrGroupSelectedPoint2[g][arrGroupSelectedPoint2[g].Count - 1].Y <= intPartitionEndY)))
                                continue;

                            // ignore if non of points in this partition
                            if (!((arrGroupSelectedPoint2[g][0].Y >= intPartitionStartY) && (arrGroupSelectedPoint2[g][0].Y <= intPartitionEndY)) &&
                                !((arrGroupSelectedPoint2[g][arrGroupSelectedPoint2[g].Count - 1].Y >= intPartitionStartY) && (arrGroupSelectedPoint2[g][arrGroupSelectedPoint2[g].Count - 1].Y <= intPartitionEndY)))
                                continue;

                            arrGroupSelectedIndex3.Add(new List<int>());
                            arrGroupSelectedAngle3.Add(new List<float>());
                            arrGroupSelectedPoint3.Add(new List<PointF>());
                            int intGroupIndex = arrGroupSelectedPoint3.Count - 1;
                            for (int i = 0; i < arrGroupSelectedPoint2[g].Count; i++)
                            {
                                if (arrGroupSelectedPoint2[g][i].Y >= intPartitionStartY && arrGroupSelectedPoint2[g][i].Y <= intPartitionEndY)
                                {
                                    arrGroupSelectedIndex3[intGroupIndex].Add(arrGroupSelectedIndex2[g][i]);
                                    arrGroupSelectedAngle3[intGroupIndex].Add(arrGroupSelectedAngle2[g][i]);
                                    arrGroupSelectedPoint3[intGroupIndex].Add(arrGroupSelectedPoint2[g][i]);
                                }
                            }

                            if (arrGroupSelectedPoint3[intGroupIndex].Count == 0)
                            {
                                arrGroupSelectedIndex3.RemoveAt(intGroupIndex);
                                arrGroupSelectedAngle3.RemoveAt(intGroupIndex);
                                arrGroupSelectedPoint3.RemoveAt(intGroupIndex);
                            }

                        }
                    }

                    if (arrGroupSelectedPoint3.Count > 0)
                    {
                        for (int i = 0; i < arrGroupSelectedPoint3.Count; i++)
                        {
                            arrGroupSelectedIndex.Add(arrGroupSelectedIndex3[i]);
                            arrGroupSelectedAngle.Add(arrGroupSelectedAngle3[i]);
                            arrGroupSelectedPoint.Add(arrGroupSelectedPoint3[i]);
                        }
                    }
                }



                // ================================================================================================
                // ================================================================================================


                List<LineGaugeRecord> arrLineGaugeRecord = new List<LineGaugeRecord>();

                // Get group index that have highest points
                for (int g = 0; g < arrGroupSelectedPoint.Count; g++)
                {
                    arrSelectedPoint = arrGroupSelectedPoint[g];

                    // Set the selected group to sub gauge.
                    // Measure using sub gauge and get sub gauge result.
                    if (arrSelectedPoint.Count > 3)
                    {
                        float fStartX = arrSelectedPoint[0].X;
                        float fStartY = arrSelectedPoint[0].Y;
                        float fEndX = arrSelectedPoint[0].X;
                        float fEndY = arrSelectedPoint[0].Y;

                        for (int i = 1; i < arrSelectedPoint.Count; i++)
                        {
                            if (fStartX > arrSelectedPoint[i].X)
                                fStartX = arrSelectedPoint[i].X;

                            if (fStartY > arrSelectedPoint[i].Y)
                                fStartY = arrSelectedPoint[i].Y;

                            if (fEndX < arrSelectedPoint[i].X)
                                fEndX = arrSelectedPoint[i].X;

                            if (fEndY < arrSelectedPoint[i].Y)
                                fEndY = arrSelectedPoint[i].Y;
                        }

                        float fLineGaugeCenterX = (fStartX + fEndX) / 2;
                        float fLineGaugeCenterY = (fStartY + fEndY) / 2;

                        if (m_SubLineGauge == null)
                            m_SubLineGauge = new ELineGauge();

                        m_SubLineGauge.SetCenterXY(fLineGaugeCenterX, fLineGaugeCenterY);
                        m_SubLineGauge.Angle = m_LineGauge.Angle;

                        m_SubLineGauge.MinAmplitude = m_LineGauge.MinAmplitude;
                        m_SubLineGauge.MinArea = m_LineGauge.MinArea;
                        m_SubLineGauge.Smoothing = m_LineGauge.Smoothing;
                        m_SubLineGauge.Thickness = m_LineGauge.Thickness;
                        m_SubLineGauge.NumFilteringPasses = m_LineGauge.NumFilteringPasses;
                        m_SubLineGauge.FilteringThreshold = m_LineGauge.FilteringThreshold;
                        m_SubLineGauge.Threshold = m_LineGauge.Threshold;
                        m_SubLineGauge.TransitionChoice = m_LineGauge.TransitionChoice;
                        m_SubLineGauge.TransitionType = m_LineGauge.TransitionType;
                        m_SubLineGauge.SamplingStep = m_LineGauge.SamplingStep;

                        // --------------------------
                        float fLineGaugeTolerance;
                        float fLineGaugeLength;

                        if ((m_LineGauge.Angle > -45 && m_LineGauge.Angle < 45) || (m_LineGauge.Angle > 135 && m_LineGauge.Angle < 225))
                        {
                            fLineGaugeTolerance = Math.Min(m_LineGauge.Tolerance, (fEndY - fStartY) / 2 + 6);
                            fLineGaugeLength = fEndX - fStartX;
                        }
                        else
                        {
                            fLineGaugeTolerance = Math.Min(m_LineGauge.Tolerance, (fEndX - fStartX) / 2 + 6);
                            fLineGaugeLength = fEndY - fStartY;
                        }

                        m_SubLineGauge.Tolerance = fLineGaugeTolerance;
                        m_SubLineGauge.Length = fLineGaugeLength;
                        //m_SubLineGauge.Save("D:\\TS\\LG.cal");
                        //objROI.ref_ROI.TopParent.Save("D:\\TS\\TOpP.bmp");
                        m_SubLineGauge.Measure(objROI.ref_ROI);

                        int NumSamples1 = (int)m_SubLineGauge.NumSamples;
                        int NumValidSamples1 = (int)m_SubLineGauge.NumValidSamples;

                        float fSubLineScore = (float)m_SubLineGauge.NumValidSamples / arrSelectedPoint.Count;

                        if (fSubLineScore < (0.75f))
                        {
                            m_SubLineGauge.Tolerance = m_LineGauge.Tolerance;
                            m_SubLineGauge.Length = fLineGaugeLength;
                            //m_SubLineGauge.Save("D:\\TS\\LG.cal");
                            //objROI.ref_ROI.TopParent.Save("D:\\TS\\TOpP.bmp");
                            m_SubLineGauge.Measure(objROI.ref_ROI);
                        }

                        // -------------------------------
                        //m_SubLineGauge.Tolerance = m_LineGauge.Tolerance;

                        //m_SubLineGauge.Save("D:\\TS\\LG.cal");
                        //objROI.ref_ROI.TopParent.Save("D:\\TS\\TOpP.bmp");
                        //m_SubLineGauge.Measure(objROI.ref_ROI);

                        //int NumSamples2 = m_SubLineGauge.NumSamples;
                        //int NumValidSamples2 = m_SubLineGauge.NumValidSamples;


                        m_fSubGaugeAngle = m_SubLineGauge.MeasuredLine.Angle;

                        LineGaugeRecord objLineGaugeRecord = new LineGaugeRecord();
                        objLineGaugeRecord.bUseSubGauge = true;
                        objLineGaugeRecord.fMeasureAngle = m_SubLineGauge.MeasuredLine.Angle;
                        objLineGaugeRecord.fMeasureCenterX = m_SubLineGauge.MeasuredLine.CenterX;
                        objLineGaugeRecord.fMeasureCenterY = m_SubLineGauge.MeasuredLine.CenterY;
                        objLineGaugeRecord.fMeasureScore = (m_SubLineGauge.NumValidSamples * 100 / (float)m_SubLineGauge.NumSamples);

                        objLineGaugeRecord.fSetTolerance = m_SubLineGauge.Tolerance;
                        objLineGaugeRecord.fSetLength = m_SubLineGauge.Length;
                        objLineGaugeRecord.fSetCenterX = m_SubLineGauge.CenterX;
                        objLineGaugeRecord.fSetCenterY = m_SubLineGauge.CenterY;
                        objLineGaugeRecord.fSetAngle = m_SubLineGauge.Angle;
                        objLineGaugeRecord.intSetMinAmp = (int)m_SubLineGauge.MinAmplitude;
                        objLineGaugeRecord.intSetMinArea = (int)m_SubLineGauge.MinArea;
                        objLineGaugeRecord.intSetSmoothing = (int)m_SubLineGauge.Smoothing;
                        objLineGaugeRecord.intSetThreshold = (int)m_SubLineGauge.Threshold;
                        objLineGaugeRecord.intSetTransitionChoice = m_SubLineGauge.TransitionChoice.GetHashCode();
                        objLineGaugeRecord.intSetTransitionType = m_SubLineGauge.TransitionType.GetHashCode();

                        objLineGaugeRecord.objLine.CalculateLGStraightLine(new PointF(m_SubLineGauge.MeasuredLine.CenterX,
                                                                                   m_SubLineGauge.MeasuredLine.CenterY),
                                                                                   m_SubLineGauge.MeasuredLine.Angle);
                        arrLineGaugeRecord.Add(objLineGaugeRecord);
                    }
                }

                if (!m_blnExecuted)
                    m_blnExecuted = true;

                return arrLineGaugeRecord;
                //STTrackLog.WriteLine("------ End ----------");


            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }

            return new List<LineGaugeRecord>();
        }

        public List<LineGaugeRecord> MeasureAndCollect_CornerPointGauge(PointF pLeft, PointF pRight, float fLeftAngle, float fRightAngle)
        {
            try
            {
                List<LineGaugeRecord> arrLineGaugeRecord = new List<LineGaugeRecord>();

                if (pLeft.X != 0 && pLeft.Y != 0 && pRight.X != 0 && pRight.Y != 0)
                {
                    LineGaugeRecord objLineGaugeRecord_2Points = new LineGaugeRecord();
                    objLineGaugeRecord_2Points.objLine.CalculateLGStraightLine(pLeft, pRight);
                    arrLineGaugeRecord.Add(objLineGaugeRecord_2Points);
                }

                if (pLeft.X != 0 && pLeft.Y != 0 && pRight.X != 0 && pRight.Y != 0)
                {
                    LineGaugeRecord objLineGaugeRecord_LeftPoint = new LineGaugeRecord();
                    objLineGaugeRecord_LeftPoint.objLine.CalculateLGStraightLine(pLeft,
                                                                               fLeftAngle);

                    arrLineGaugeRecord.Add(objLineGaugeRecord_LeftPoint);
                }

                if (pLeft.X != 0 && pLeft.Y != 0 && pRight.X != 0 && pRight.Y != 0)
                {
                    LineGaugeRecord objLineGaugeRecord_RightPoint = new LineGaugeRecord();
                    objLineGaugeRecord_RightPoint.objLine.CalculateLGStraightLine(pRight, fRightAngle);

                    arrLineGaugeRecord.Add(objLineGaugeRecord_RightPoint);
                }

                return arrLineGaugeRecord;
                //STTrackLog.WriteLine("------ End ----------");


            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString());
            }

            return new List<LineGaugeRecord>();
        }
        public void GetMeasurementSamplePoints_SubGaugePoints(ROI objROI, ref List<PointF> arrMeasuredPoints, ref List<bool> arrPointsValid)
        {
            int intNumSamples = (int)m_LineGauge.NumSamples;

            for (int i = 0; i < intNumSamples; i++)
            {
#if (Debug_2_12 || Release_2_12)
                m_LineGauge.MeasureSample(objROI.ref_ROI, (uint)i);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                m_LineGauge.MeasureSample(objROI.ref_ROI, i);

#endif

                if (m_LineGauge.Valid)
                {
                    arrMeasuredPoints.Add(new PointF(m_LineGauge.GetMeasuredPoint().Center.X, m_LineGauge.GetMeasuredPoint().Center.Y));
                    arrPointsValid.Add(true);
                }
                else
                {
                    //arrMeasuredPoints.Add(new PointF(0, 0));
                    //arrPointsValid.Add(false);
                }
            }
        }

        public void SetParameterToSubGauge(ROI objROI, LineGaugeRecord objLineGaugeRecord)
        {
            m_SubLineGauge.Tolerance = objLineGaugeRecord.fSetTolerance;
            m_SubLineGauge.Length = objLineGaugeRecord.fSetLength;
            m_SubLineGauge.SetCenterXY(objLineGaugeRecord.fSetCenterX, objLineGaugeRecord.fSetCenterY);
            m_SubLineGauge.Angle = objLineGaugeRecord.fSetAngle;
#if (Debug_2_12 || Release_2_12)
            m_SubLineGauge.MinAmplitude = (uint)objLineGaugeRecord.intSetMinAmp;
            m_SubLineGauge.MinArea = (uint)objLineGaugeRecord.intSetMinArea;
            m_SubLineGauge.Smoothing = (uint)objLineGaugeRecord.intSetSmoothing;
            m_SubLineGauge.Thickness = m_LineGauge.Thickness;
            m_SubLineGauge.NumFilteringPasses = m_LineGauge.NumFilteringPasses;
            m_SubLineGauge.FilteringThreshold = m_LineGauge.FilteringThreshold;
            m_SubLineGauge.Threshold = (uint)objLineGaugeRecord.intSetThreshold;
            m_SubLineGauge.TransitionChoice = m_LineGauge.TransitionChoice;
            m_SubLineGauge.TransitionType = m_LineGauge.TransitionType;
            m_SubLineGauge.SamplingStep = m_LineGauge.SamplingStep;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_SubLineGauge.MinAmplitude = objLineGaugeRecord.intSetMinAmp;
            m_SubLineGauge.MinArea = objLineGaugeRecord.intSetMinArea;
            m_SubLineGauge.Smoothing = objLineGaugeRecord.intSetSmoothing;
            m_SubLineGauge.Thickness = m_LineGauge.Thickness;
            m_SubLineGauge.NumFilteringPasses = m_LineGauge.NumFilteringPasses;
            m_SubLineGauge.FilteringThreshold = m_LineGauge.FilteringThreshold;
            m_SubLineGauge.Threshold = objLineGaugeRecord.intSetThreshold;
            m_SubLineGauge.TransitionChoice = m_LineGauge.TransitionChoice;
            m_SubLineGauge.TransitionType = m_LineGauge.TransitionType;
            m_SubLineGauge.SamplingStep = m_LineGauge.SamplingStep;

#endif

            //m_SubLineGauge.Save("D:\\TS\\LG.cal");
            //objROI.ref_ROI.TopParent.Save("D:\\TS\\TOpP.bmp");
            m_SubLineGauge.Measure(objROI.ref_ROI);
            m_fSubGaugeAngle = m_SubLineGauge.MeasuredLine.Angle;
            m_blnReferSubValue = true;
        }

        public float GetObjectScore()
        {
            if (!m_blnReferSubValue)
                return (m_LineGauge.NumValidSamples * 100 / (float)m_LineGauge.NumSamples);
            else
                return (m_SubLineGauge.NumValidSamples * 100 / (float)m_SubLineGauge.NumSamples);
        }

        public float GetObjectAngle()
        {
            if (!m_blnReferSubValue)
            {
                return GetLineAngle();
            }
            else
            {
                return GetSubLineAngle();
            }
        }

        private float GetLineAngle()
        {
            //version 1
            //get
            //{
            //    if ((m_LineGauge.Angle >= 225) && (m_LineGauge.Angle <= 315))
            //    {
            //        if (m_LineGauge.MeasuredLine.Angle > 180)
            //            return (m_LineGauge.MeasuredLine.Angle - m_LineGauge.Angle);
            //        else
            //            return (m_LineGauge.MeasuredLine.Angle - (360 - m_LineGauge.Angle));
            //    }
            //    else if ((m_LineGauge.Angle >= -135) && (m_LineGauge.Angle <= -45))
            //    {
            //        if (m_LineGauge.MeasuredLine.Angle > 0)
            //            return (m_LineGauge.MeasuredLine.Angle + m_LineGauge.Angle);
            //        else
            //            return (m_LineGauge.MeasuredLine.Angle - m_LineGauge.Angle);
            //    }
            //    else
            //        return (m_LineGauge.MeasuredLine.Angle - m_LineGauge.Angle);
            //}

            //version 2
            //get
            //{
            //    if ((m_LineGauge.Angle >= 225) && (m_LineGauge.Angle <= 315))
            //    {
            //        if (m_LineGauge.MeasuredLine.Angle > 180)
            //            return (m_LineGauge.MeasuredLine.Angle - m_LineGauge.Angle);
            //        else
            //            return (m_LineGauge.MeasuredLine.Angle - (360 - m_LineGauge.Angle));
            //    }
            //    else if ((m_LineGauge.Angle >= -135) && (m_LineGauge.Angle <= -45)) // -90 Tolerance Setting Range
            //    {
            //        /*2019 10 08 - CCENG: when line gauge angle is - 90, the line gauge angle result is as below:
            //             *
            //             *                60    |    120
            //             *                 \    |    /
            //             *                      |
            //             *    0 < --------------------------------------
            //             *                      |
            //             *                 /    |    \
            //             *               -60    |    -120
            //             *
            //             *mean "\" will get line gauge angle result 60 or -120 whereas " / " will get angle result -60 or 120.
            //         */

            //        if (m_LineGauge.MeasuredLine.Angle > 0)
            //        {
            //            // 2019 10 08 - CCENG: update from formula (180 - m_LineGauge.MeasuredLine.Angle + m_LineGauge.Angle) to m_LineGauge.MeasuredLine.Angle + m_LineGauge.Angle;
            //            /*
            //             * 
            //             * Example 1: 
            //             *         - for right edge line gauge
            //             *         - m_LineGauge.Angle is -90
            //             *         - get m_LineGauge.MeasuredLine.Angle 88.9. mean vertical line is "\".
            //             *         - should return neg value == 88.9 + (-90) == -1.1
            //             *         
            //             * Example 2: 
            //             *         - for right edge line gauge
            //             *         - m_LineGauge.Angle is -90
            //             *         - get m_LineGauge.MeasuredLine.Angle 91.1 mean vertical line is "/".
            //             *         - should return neg value == 91.1 + (-90) == 1.1
            //             */
            //            return m_LineGauge.MeasuredLine.Angle + m_LineGauge.Angle; // 2019-10-10 ZJYEOH : For EUI Tilt Angle Unit (180 - m_LineGauge.MeasuredLine.Angle + m_LineGauge.Angle);  
            //        }
            //        else
            //        {
            //            /*
            //             * Example 1: 
            //             *         - for right edge line gauge
            //             *         - m_LineGauge.Angle is -90
            //             *         - get m_LineGauge.MeasuredLine.Angle -88.9. mean vertical line is "/".
            //             *         - should return neg value == -88.9 - (-90) == 1.1
            //             */
            //            return (m_LineGauge.MeasuredLine.Angle - m_LineGauge.Angle);
            //        }
            //    }
            //    else
            //        return (m_LineGauge.MeasuredLine.Angle - m_LineGauge.Angle);
            //}

            // version 3
            // 2019 10 15 - CCENG: better formula of return angle (refer Euresys Line Gauge Angle.xls from server)
            if ((m_LineGauge.Angle >= 225) && (m_LineGauge.Angle <= 315))   // Gauge Direction 270
            {
                if (m_LineGauge.MeasuredLine.Angle > 180)
                {
                    return (m_LineGauge.MeasuredLine.Angle - 270) - (m_LineGauge.Angle - 270);
                }
                else
                {
                    if (m_LineGauge.MeasuredLine.Angle > 0)
                        return (m_LineGauge.MeasuredLine.Angle + 270) - (m_LineGauge.Angle - 270) - 360;
                    else
                        return (m_LineGauge.MeasuredLine.Angle - 270) - (m_LineGauge.Angle - 270) + 360;
                }
            }
            else if ((m_LineGauge.Angle >= 135) && (m_LineGauge.Angle <= 225))  // Gauge Direction 180
            {
                if (m_LineGauge.MeasuredLine.Angle > 90)
                    return (m_LineGauge.MeasuredLine.Angle - 180) - (m_LineGauge.Angle - 180);
                else
                    return (m_LineGauge.MeasuredLine.Angle - (m_LineGauge.Angle - 180));
            }
            else if ((m_LineGauge.Angle >= 45) && (m_LineGauge.Angle <= 135)) // Gauge Direction 90
            {
                if (m_LineGauge.MeasuredLine.Angle > 0)
                    return (m_LineGauge.MeasuredLine.Angle - 90) - (m_LineGauge.Angle - 90);
                else
                    return (m_LineGauge.MeasuredLine.Angle + 90) - (m_LineGauge.Angle - 90);
            }
            else if ((m_LineGauge.Angle >= -135) && (m_LineGauge.Angle <= -45)) // Gauge Direction -90
            {
                if (m_LineGauge.MeasuredLine.Angle > 0)
                    return (m_LineGauge.MeasuredLine.Angle - 90) - (m_LineGauge.Angle + 90);
                else
                    return (m_LineGauge.MeasuredLine.Angle + 90) - (m_LineGauge.Angle + 90);
            }
            else
                return (m_LineGauge.MeasuredLine.Angle - m_LineGauge.Angle);

        }

        private float GetSubLineAngle()
        {

            // version 3
            // 2019 10 15 - CCENG: better formula of return angle (refer Euresys Line Gauge Angle.xls from server)
            if ((m_LineGauge.Angle >= 225) && (m_LineGauge.Angle <= 315))   // Gauge Direction 270
            {
                if (m_fSubGaugeAngle > 180)
                {
                    return (m_fSubGaugeAngle - 270) - (m_LineGauge.Angle - 270);
                }
                else
                {
                    if (m_fSubGaugeAngle > 0)
                        return (m_fSubGaugeAngle + 270) - (m_LineGauge.Angle - 270) - 360;
                    else
                        return (m_fSubGaugeAngle - 270) - (m_LineGauge.Angle - 270) + 360;
                }
            }
            else if ((m_LineGauge.Angle >= 135) && (m_LineGauge.Angle <= 225))  // Gauge Direction 180
            {
                if (m_fSubGaugeAngle > 90)
                    return (m_fSubGaugeAngle - 180) - (m_LineGauge.Angle - 180);
                else
                    return (m_fSubGaugeAngle - (m_LineGauge.Angle - 180));
            }
            else if ((m_LineGauge.Angle >= 45) && (m_LineGauge.Angle <= 135)) // Gauge Direction 90
            {
                if (m_fSubGaugeAngle > 0)
                    return (m_fSubGaugeAngle - 90) - (m_LineGauge.Angle - 90);
                else
                    return (m_fSubGaugeAngle + 90) - (m_LineGauge.Angle - 90);
            }
            else if ((m_LineGauge.Angle >= -135) && (m_LineGauge.Angle <= -45)) // Gauge Direction -90
            {
                if (m_fSubGaugeAngle > 0)
                    return (m_fSubGaugeAngle - 90) - (m_LineGauge.Angle + 90);
                else
                    return (m_fSubGaugeAngle + 90) - (m_LineGauge.Angle + 90);
            }
            else
                return (m_fSubGaugeAngle - m_LineGauge.Angle);

        }
        public static float ConvertObjectAngle(float fDirection, float fLineAngle)
        {
            // version 3
            // 2019 10 15 - CCENG: better formula of return angle (refer Euresys Line Gauge Angle.xls from server)

            if ((fDirection >= 225) && (fDirection <= 315))   // Gauge Direction 270
            {
                if (fLineAngle > 180)
                {
                    return (fLineAngle - 270) - (fDirection - 270);
                }
                else
                {
                    if (fLineAngle > 0)
                        return (fLineAngle + 270) - (fDirection - 270) - 360;
                    else
                        return (fLineAngle - 270) - (fDirection - 270) + 360;
                }
            }
            else if ((fDirection >= 135) && (fDirection <= 225))  // Gauge Direction 180
            {
                if (fLineAngle > 90)
                    return (fLineAngle - 180) - (fDirection - 180);
                else
                    return (fLineAngle - (fDirection - 180));
            }
            else if ((fDirection >= 45) && (fDirection <= 135)) // Gauge Direction 90
            {
                fLineAngle = fLineAngle % 90;

                if (fLineAngle > 0)
                    return (fLineAngle - 90) - (fDirection - 90);
                else
                    return (fLineAngle + 90) - (fDirection - 90);
            }
            else if ((fDirection >= -135) && (fDirection <= -45)) // Gauge Direction -90
            {
                if (fLineAngle > 0)
                    return (fLineAngle - 90) - (fDirection + 90);
                else
                    return (fLineAngle + 90) - (fDirection + 90);
            }
            else
            {
                if (fLineAngle > 90)
                    return (fLineAngle - 180) - (fDirection);
                else
                    return (fLineAngle - fDirection);
            }

        }
    }

    public class LineGaugeRecord
    {
        public bool bUseSubGauge;
        public bool bUsePointGauge;
        public float fMeasureScore;
        public float fMeasureAngle;
        public float fMeasureCenterX;
        public float fMeasureCenterY;

        public float fSetTolerance;
        public float fSetLength;
        public float fSetCenterX;
        public float fSetCenterY;
        public float fSetAngle;
        public int intSetMinAmp;
        public int intSetMinArea;
        public int intSetSmoothing;
        public int intSetThreshold;
        public int intSetTransitionChoice;
        public int intSetTransitionType;

        public Line objLine = new Line();
    }
}
