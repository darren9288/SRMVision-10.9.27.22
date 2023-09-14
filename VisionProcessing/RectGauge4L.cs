using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
#if (Debug_2_12 || Release_2_12)
using Euresys.Open_eVision_2_12;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
using Euresys.Open_eVision_1_2;
#endif
using Common;

namespace VisionProcessing
{
    public class RectGauge4L
    {
        #region Member Variables

        // Main variables
        private List<int> m_arrAutoDontCareThreshold = new List<int>(); 
        private List<int> m_arrAutoDontCareTotalLength = new List<int>();
        private List<int> m_arrAutoDontCareOffset = new List<int>();
        private List<bool> m_arrAutoDontCare = new List<bool>();
        private EBlobs m_objBlob = new EBlobs();
        private List<EBlobs> m_arrBlob = new List<EBlobs>();
        private int m_intSelectedEdgeROIPrev = 0;
        private int m_intVisionIndex = 0;
        private bool m_blnCursorShapeVerifying = false;
        private int m_intDirection = 0;
        private float m_fLength = 0;
        private float m_fUnitSizeWidth = 0;
        private float m_fUnitSizeHeight = 0;
        private float m_f4LGaugeCenterPointX = 0;
        private float m_f4LGaugeCenterPointY = 0;
        private List<ROI> m_arrEdgeROI = new List<ROI>();
        private List<List<ROI>> m_arrDontCareROI = new List<List<ROI>>();
        private int[] m_intEdgeROICenterX = new int[4];
        private int[] m_intEdgeROICenterY = new int[4];
        private float[] m_fTemplateGaugeTolerance = new float[4];
        private float[] m_fOffsetX = new float[4];
        private float[] m_fOffsetY = new float[4];
        private List<int> m_arrGaugeMeasureMode = new List<int>();   //   Index 0=Top, 1=Right, 2=Bottom, 3=Left      // Measure Mode Value 0 = Standard 1 = Sub Gauge 2 = Blob Method
        private List<PGauge> m_arrPointGauge = new List<PGauge>();       // Index 0=Top, 1=Right, 2=Bottom, 3=Left,
        private List<PGauge> m_arrPointGauge2 = new List<PGauge>();       // Index 0=Top, 1=Right, 2=Bottom, 3=Left,
        private List<LGauge> m_arrLineGauge = new List<LGauge>();       // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<bool> m_arrGaugeEnableSubGauge = new List<bool>();  // Index 0=Top, 1=Right, 2=Bottom, 3=Left,
        private List<int> m_arrGaugeOffset = new List<int>();         // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<int> m_arrGaugeMinScore = new List<int>();         // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<int> m_arrGaugeMaxAngle = new List<int>();         // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<int> m_arrGaugeImageMode = new List<int>();        // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<int> m_arrGaugeImageNo = new List<int>();          // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<int> m_arrGaugeImageThreshold = new List<int>();   // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<int> m_arrGaugeImageOpenCloseIteration = new List<int>();   // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<bool> m_arrGaugeWantImageThreshold = new List<bool>();   // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<float> m_arrGaugeImageGain = new List<float>();    // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<float> m_arrGaugeGroupTolerance = new List<float>();    // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private int m_intGaugeTiltAngle;
        private List<bool> m_blnInToOut = new List<bool>();
        private Line[] m_arrCornerLine = new Line[8];
        private Line m_objVirtualLineWidth = new Line();
        private Line m_objVirtualLineHeight = new Line();

        // Rectange Object Result
        private PointF[] m_arrRectCornerPoints = new PointF[4]; // Index 0=TopLeft, 1=TopRight, 2=BottomRight, 3=BottomLeft
        private PointF m_pRectCenterPoint;
        private bool[] m_arrLineResultOK = new bool[4];     // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private bool[] m_arrCornerPointResult = new bool[4];
        private bool[] m_arrCornerPointResult2 = new bool[4]; 
        private float m_fRectWidth = 0;
        private float m_fRectHeight = 0;
        private float m_fRectUpWidth = 0;
        private float m_fRectDownWidth = 0;
        private float m_fRectLeftHeight = 0;
        private float m_fRectRightHeight = 0;
        private float m_fRectAngle = 0;
        private string m_strErrorMessage = "";
        private int m_intFailResultMask = 0;
        private int[] m_arrUserThreshold = new int[4];
        private int[] m_arrUserThickness = new int[4] {1, 1, 1, 1}; // 2021-02-08 ZJYEOH : Thickness must be at least 1
        private int[] m_arrUserChoice = new int[4];

        // Drawing
        private SolidBrush m_BrushMatched = new SolidBrush(Color.GreenYellow);
        private Pen m_PenRectGNominal = new Pen(Color.Blue);
        private Pen m_PenRectGTransitionTypeArrow = new Pen(Color.Lime);
        private Pen m_PenRectGTransitionTypeArrowNotSelected = new Pen(Color.Green);
        private Pen m_PenDontCareArea = new Pen(Color.Pink, 2);
        // Learn Template information

        // Collect Gauge Result
        List<LineGaugeRecord>[] m_arrLineGaugeRecord = new List<LineGaugeRecord>[4];

        #endregion

        #region Properties

        // Result
        public int ref_intSelectedEdgeROIPrev { get { return m_intSelectedEdgeROIPrev; } set { m_intSelectedEdgeROIPrev = value; } }
        public float ref_f4LGaugeCenterPointX { get { return m_f4LGaugeCenterPointX; } }
        public float ref_f4LGaugeCenterPointY { get { return m_f4LGaugeCenterPointY; } }
        public PointF ref_pRectCenterPoint { get { return m_pRectCenterPoint; } set { m_pRectCenterPoint = value; } }
        public PointF[] ref_arrRectCornerPoints { get { return m_arrRectCornerPoints; } }
        public float ref_fRectWidth { get { return m_fRectWidth; } set { m_fRectWidth = value; } }
        public float ref_fRectUpWidth { get { return m_fRectUpWidth; } set { m_fRectUpWidth = value; } }
        public float ref_fRectDownWidth { get { return m_fRectDownWidth; } set { m_fRectDownWidth = value; } }
        public float ref_fRectHeight { get { return m_fRectHeight; } set { m_fRectHeight = value; } }
        public float ref_fRectLeftHeight { get { return m_fRectLeftHeight; } set { m_fRectLeftHeight = value; } }
        public float ref_fRectRightHeight { get { return m_fRectRightHeight; } set { m_fRectRightHeight = value; } }
        public float ref_fRectAngle { get { return m_fRectAngle; } set { m_fRectAngle = value; } }
        public string ref_strErrorMessage { get { return m_strErrorMessage; } set { m_strErrorMessage = value; } }
        public int ref_intFailResultMask { get { return m_intFailResultMask; } set { m_intFailResultMask = value; } }
        public bool[] ref_arrLineResultOK { get { return m_arrLineResultOK; } }
        public List<ROI> ref_arrEdgeROI { get { return m_arrEdgeROI; } }
        public List<LGauge> ref_arrLineGauge { get { return m_arrLineGauge; } }

        #endregion
      
        public RectGauge4L(GaugeWorldShape objWorldShape, int intDirection)
        {
            m_intDirection = intDirection;

            // Init variables
            for (int i = 0; i < 12; i++)
            {
                m_arrPointGauge.Add(new PGauge(objWorldShape));
                m_arrPointGauge2.Add(new PGauge(objWorldShape));
                m_arrLineGauge.Add(new LGauge(objWorldShape));
                m_arrGaugeEnableSubGauge.Add(false);
                m_arrGaugeMeasureMode.Add(0);
                m_arrGaugeMinScore.Add(40);
                m_arrGaugeOffset.Add(10);
                m_arrGaugeMaxAngle.Add(3);
                m_arrGaugeImageMode.Add(0);
                m_arrGaugeImageNo.Add(0);
                m_arrGaugeImageGain.Add(1);
                m_arrGaugeGroupTolerance.Add(1.5f);
                m_arrGaugeImageThreshold.Add(125);
                m_arrGaugeImageOpenCloseIteration.Add(0);
                m_arrGaugeWantImageThreshold.Add(false);
                m_intGaugeTiltAngle = 0;
                m_blnInToOut.Add(false);
            }
            m_pRectCenterPoint = new PointF();

            for (int i = 0; i < m_arrRectCornerPoints.Length; i++)
            {
                m_arrRectCornerPoints[i] = new PointF();
            }

            m_arrEdgeROI.Add(new ROI("T" + m_intDirection));
            m_arrEdgeROI.Add(new ROI("R" + m_intDirection));
            m_arrEdgeROI.Add(new ROI("B" + m_intDirection));
            m_arrEdgeROI.Add(new ROI("L" + m_intDirection));

            m_arrAutoDontCare.Add(false);
            m_arrAutoDontCare.Add(false);
            m_arrAutoDontCare.Add(false);
            m_arrAutoDontCare.Add(false);

            m_arrBlob.Add(new EBlobs());
            m_arrBlob.Add(new EBlobs());
            m_arrBlob.Add(new EBlobs());
            m_arrBlob.Add(new EBlobs());

            m_arrAutoDontCareOffset.Add(0);
            m_arrAutoDontCareOffset.Add(0);
            m_arrAutoDontCareOffset.Add(0);
            m_arrAutoDontCareOffset.Add(0);

            m_arrAutoDontCareThreshold.Add(125);
            m_arrAutoDontCareThreshold.Add(125);
            m_arrAutoDontCareThreshold.Add(125);
            m_arrAutoDontCareThreshold.Add(125);

            m_arrAutoDontCareTotalLength.Add(0);
            m_arrAutoDontCareTotalLength.Add(0);
            m_arrAutoDontCareTotalLength.Add(0);
            m_arrAutoDontCareTotalLength.Add(0);

            m_arrDontCareROI.Add(new List<ROI>());
            m_arrDontCareROI.Add(new List<ROI>());
            m_arrDontCareROI.Add(new List<ROI>());
            m_arrDontCareROI.Add(new List<ROI>());

            // Set line gauge for different direction
            m_arrLineGauge[0].SetGaugeAngle(0);    // Top
            m_arrLineGauge[1].SetGaugeAngle(90);   // Right
            m_arrLineGauge[2].SetGaugeAngle(180);  // Bottom
            m_arrLineGauge[3].SetGaugeAngle(-90);  // Left
            InitGaugeRecordList();
        }

        public RectGauge4L(GaugeWorldShape objWorldShape, int intDirection, int intVisionIndex)
        {
            m_intDirection = intDirection;
            m_intVisionIndex = intVisionIndex;

            // Init variables
            for (int i = 0; i < 12; i++)
            {
                m_arrPointGauge.Add(new PGauge(objWorldShape));
                m_arrPointGauge2.Add(new PGauge(objWorldShape));
                m_arrLineGauge.Add(new LGauge(objWorldShape));
                m_arrGaugeEnableSubGauge.Add(false);
                m_arrGaugeMeasureMode.Add(0);
                m_arrGaugeMinScore.Add(40);
                m_arrGaugeOffset.Add(10);
                m_arrGaugeMaxAngle.Add(3);
                m_arrGaugeImageMode.Add(0);
                m_arrGaugeImageNo.Add(0);
                m_arrGaugeImageGain.Add(1);
                m_arrGaugeGroupTolerance.Add(1.5f);
                m_arrGaugeImageThreshold.Add(125);
                m_arrGaugeImageOpenCloseIteration.Add(0);
                m_arrGaugeWantImageThreshold.Add(false);
                m_intGaugeTiltAngle = 0;
                m_blnInToOut.Add(false);
            }
            m_pRectCenterPoint = new PointF();

            for (int i = 0; i < m_arrRectCornerPoints.Length; i++)
            {
                m_arrRectCornerPoints[i] = new PointF();
            }

            m_arrEdgeROI.Add(new ROI("T" + m_intDirection));
            m_arrEdgeROI.Add(new ROI("R" + m_intDirection));
            m_arrEdgeROI.Add(new ROI("B" + m_intDirection));
            m_arrEdgeROI.Add(new ROI("L" + m_intDirection));

            m_arrAutoDontCare.Add(false);
            m_arrAutoDontCare.Add(false);
            m_arrAutoDontCare.Add(false);
            m_arrAutoDontCare.Add(false);

            m_arrBlob.Add(new EBlobs());
            m_arrBlob.Add(new EBlobs());
            m_arrBlob.Add(new EBlobs());
            m_arrBlob.Add(new EBlobs());

            m_arrAutoDontCareOffset.Add(0);
            m_arrAutoDontCareOffset.Add(0);
            m_arrAutoDontCareOffset.Add(0);
            m_arrAutoDontCareOffset.Add(0);

            m_arrAutoDontCareThreshold.Add(125);
            m_arrAutoDontCareThreshold.Add(125);
            m_arrAutoDontCareThreshold.Add(125);
            m_arrAutoDontCareThreshold.Add(125);

            m_arrAutoDontCareTotalLength.Add(0);
            m_arrAutoDontCareTotalLength.Add(0);
            m_arrAutoDontCareTotalLength.Add(0);
            m_arrAutoDontCareTotalLength.Add(0);

            m_arrDontCareROI.Add(new List<ROI>());
            m_arrDontCareROI.Add(new List<ROI>());
            m_arrDontCareROI.Add(new List<ROI>());
            m_arrDontCareROI.Add(new List<ROI>());

            // Set line gauge for different direction
            m_arrLineGauge[0].SetGaugeAngle(0);    // Top
            m_arrLineGauge[1].SetGaugeAngle(90);   // Right
            m_arrLineGauge[2].SetGaugeAngle(180);  // Bottom
            m_arrLineGauge[3].SetGaugeAngle(-90);  // Left
            InitGaugeRecordList();
        }

        ~RectGauge4L()
        {
            Dispose();
        }

        public bool Measure(ROI objROI)
        {
            return GetPosition_UsingEdgeAndCornerLineGauge(objROI);
        }

        /// <summary>
        /// This measure function is used to measure positioning in Pad5SPackage Vision.
        /// </summary>
        /// <param name="objROI"></param>
        /// <returns></returns>
        public bool Measure_Pad5SPackage(ImageDrawing objImage, bool blnEnableVirtualLine)
        {
            /*
             * if EnableVirtualLine is true
             *      -> When either side of line gauge are fail, a virtual line will be created based on preset UnitSize setting             
             *      -> *note = Make sure UnitSize value are set before calling this functin with EnableVirtualLine set to true.
             */
            return GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg(objImage, blnEnableVirtualLine, false, 0);
        }

        /// <summary>
        /// This measure function is used to measure positioning in Pad5SPackage Vision.
        /// </summary>
        /// <param name="objROI"></param>
        /// <returns></returns>
        public bool Measure_Pad5SPackage_WithDontCareArea(ImageDrawing objImage, bool blnEnableVirtualLine, ImageDrawing objWhiteImage)
        {
            /*
             * if EnableVirtualLine is true
             *      -> When either side of line gauge are fail, a virtual line will be created based on preset UnitSize setting             
             *      -> *note = Make sure UnitSize value are set before calling this functin with EnableVirtualLine set to true.
             */
            // if dont care present
            if (m_arrDontCareROI[0].Count != 0 || m_arrDontCareROI[1].Count != 0 || m_arrDontCareROI[2].Count != 0 || m_arrDontCareROI[3].Count != 0)
            {
                ImageDrawing objModifiedImage = new ImageDrawing();
                objImage.CopyTo(ref objModifiedImage);
                ModifyDontCareImage(objModifiedImage, objWhiteImage);

                bool blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg(objModifiedImage, blnEnableVirtualLine, false, 0);

                if (!blnResult && m_arrGaugeEnableSubGauge.Contains(true))
                    blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_Pad5SPkg(objModifiedImage, blnEnableVirtualLine, false, 0);

                objModifiedImage.Dispose();
                return blnResult;
            }
            else
            {
                //return GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg(objImage, blnEnableVirtualLine, false, 0);

                bool blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg(objImage, blnEnableVirtualLine, false, 0);

                if (!blnResult && m_arrGaugeEnableSubGauge.Contains(true))
                    blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_Pad5SPkg(objImage, blnEnableVirtualLine, false, 0);

                return blnResult;
            }
        }

        public bool Measure_UseSidePkgCornerPoint(float fCornerX_TL, float fCornerY_TL,
                                                     float fCornerX_TR, float fCornerY_TR,
                                                     float fCornerX_BR, float fCornerY_BR,
                                                     float fCornerX_BL, float fCornerY_BL)
        {
            if (!m_arrLineResultOK[0])
                m_arrLineResultOK[0] = true;
            if (!m_arrLineResultOK[1])
                m_arrLineResultOK[1] = true;
            if (!m_arrLineResultOK[2])
                m_arrLineResultOK[2] = true;
            if (!m_arrLineResultOK[3])
                m_arrLineResultOK[3] = true;

            bool blnRepeat = false;
            bool isResultOK = true;

            Line[] arrLine = new Line[4];
            arrLine[0] = new Line();
            arrLine[1] = new Line();
            arrLine[2] = new Line();
            arrLine[3] = new Line();

            arrLine[0].CalculateStraightLine(new PointF(fCornerX_TL, fCornerY_TL), new PointF(fCornerX_TR, fCornerY_TR));
            arrLine[1].CalculateStraightLine(new PointF(fCornerX_TR, fCornerY_TR), new PointF(fCornerX_BR, fCornerY_BR));
            arrLine[2].CalculateStraightLine(new PointF(fCornerX_BR, fCornerY_BR), new PointF(fCornerX_BL, fCornerY_BL));
            arrLine[3].CalculateStraightLine(new PointF(fCornerX_BL, fCornerY_BL), new PointF(fCornerX_TL, fCornerY_TL));

            float fTotalAngle = GetFormatedAngle((float)arrLine[0].ref_dAngle);
            fTotalAngle += GetFormatedAngle((float)arrLine[1].ref_dAngle);
            fTotalAngle += GetFormatedAngle((float)arrLine[2].ref_dAngle);
            fTotalAngle += GetFormatedAngle((float)arrLine[3].ref_dAngle);

            m_fRectAngle = fTotalAngle / 4;
            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 307");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(arrLine[0], arrLine[3]);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(arrLine[0], arrLine[1]);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(arrLine[2], arrLine[1]);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(arrLine[2], arrLine[3]);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(arrLine[0], arrLine[3]);
                arrCornerGaugeAngles[1] = Math2.GetAngle(arrLine[0], arrLine[1]);
                arrCornerGaugeAngles[2] = Math2.GetAngle(arrLine[2], arrLine[1]);
                arrCornerGaugeAngles[3] = Math2.GetAngle(arrLine[2], arrLine[3]);

                if (false)
                {

                    // Verify corner angle
                    isResultOK = true;
                    int intBestAngleIndex = -1;
                    float fBestAngle = float.MaxValue;
                    bool[] arrAngleOK = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {
                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                        if (fAngle > m_arrGaugeMaxAngle[i])
                        {
                            if (isResultOK)
                                isResultOK = false;
                        }
                        else
                        {
                            arrAngleOK[i] = true;

                            if (fAngle < fBestAngle)
                            {
                                fBestAngle = fAngle;
                                intBestAngleIndex = i;
                            }
                        }

                    }


                    if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                    {
                        blnRepeat = true;

                        int intBestVerIndex = -1, intBestHorIndex = -1;
                        switch (intBestAngleIndex)
                        {
                            case 0:
                                intBestHorIndex = 0;
                                intBestVerIndex = 3;
                                break;
                            case 1:
                                intBestHorIndex = 0;
                                intBestVerIndex = 1;
                                break;
                            case 2:
                                intBestHorIndex = 2;
                                intBestVerIndex = 1;
                                break;
                            case 3:
                                intBestHorIndex = 2;
                                intBestVerIndex = 3;
                                break;
                        }

                        // check can use the good angle to build the virtual line
                        if (!arrAngleOK[0]) // Top Left angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }

                            if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[0] = true;
                        }

                        if (!arrAngleOK[1]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[1] = true;
                        }

                        if (!arrAngleOK[2]) // Top Right angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[2] = true;
                        }

                        if (!arrAngleOK[3]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[3] = true;
                        }
                    }
                    else
                        break;
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Verify Unit Size
            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

            }

            if (!isResultOK)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Filter edge angle with standard condition (General Use)
        /// </summary>
        /// <param name="arrLineGauges"></param>
        /// <param name="fMinBorderScore"></param>
        /// <param name="fTotalAngle"></param>
        /// <param name="nCount"></param>
        /// <param name="blnResult"></param>
        private void FilterEdgeAngle(List<LGauge> arrLineGauges, float fMinBorderScore, ref float fTotalAngle, ref int nCount, ref bool[] blnResult)
        {
            blnResult = new bool[4];

            // Filter angle with score
            float[] fUnsortAngle = new float[4];
            for (int i = 0; i < 4; i++)
            {
                if ((arrLineGauges[i].ref_GaugeThickness < 33 && arrLineGauges[i].ref_ObjectScore < 75) ||
                    (arrLineGauges[i].ref_GaugeThickness < 70 && arrLineGauges[i].ref_ObjectScore < 50) ||
                    (arrLineGauges[i].ref_GaugeThickness >= 70 && arrLineGauges[i].ref_ObjectScore < 35))
                //if (arrLineGauges[i].ref_ObjectScore < 5)
                {
                    blnResult[i] = false;
                }
                else
                {
                    blnResult[i] = true;
                    fUnsortAngle[i] = arrLineGauges[i].ref_ObjectAngle;
                    arrLineGauges[i].GetObjectLine();
                }
            }

            // Sort angle from increase
            List<float> fSortAngle = new List<float>();
            for (int i = 0; i < fUnsortAngle.Length; i++)
            {
                if (!blnResult[i])
                    continue;

                int intIndex = fSortAngle.Count;
                for (int j = 0; j < fSortAngle.Count; j++)
                {
                    if (fSortAngle[j] > fUnsortAngle[i])
                    {
                        intIndex = j;
                        break;
                    }
                }
                fSortAngle.Insert(intIndex, fUnsortAngle[i]);
            }

            // Find the smallest gap between the angle value
            float fSmallestGap = float.MaxValue;
            float fCenterAngle = 0;
            for (int i = 1; i < fSortAngle.Count; i++)
            {
                if ((fSortAngle[i] - fSortAngle[i - 1]) < fSmallestGap)
                {
                    fSmallestGap = fSortAngle[i] - fSortAngle[i - 1];
                    fCenterAngle = (fSortAngle[i] + fSortAngle[i - 1]) / 2;
                }
            }

            // Filter angle with tolerance 1.5 from smallest angle
            fTotalAngle = 0;
            nCount = 0;
            for (int i = 0; i < fSortAngle.Count; i++)
            {
                if ((GetFormatedAngle(fSortAngle[i]) >= (fCenterAngle - 2)) && (GetFormatedAngle(fSortAngle[i]) <= (fCenterAngle + 2)))
                {
                    fTotalAngle += GetFormatedAngle(fSortAngle[i]);
                    nCount++;
                }
                else
                {
                    fSortAngle[i] = -999;
                }

            }

            // Set the blnResult to false if the angle has been filter out.
            for (int j = 0; j < blnResult.Length; j++)
            {
                if (blnResult[j])
                {
                    bool blnFound = false;
                    for (int i = 0; i < fSortAngle.Count; i++)
                    {
                        if (fUnsortAngle[j] == fSortAngle[i])
                        {
                            blnFound = true;
                            break;
                        }
                    }
                    if (!blnFound)
                        blnResult[j] = false;
                }
            }
        }

        /// <summary>
        /// Filter edge angle with loose condition (For pad which has clear edge)
        /// </summary>
        /// <param name="arrLineGauges"></param>
        /// <param name="fMinBorderScore"></param>
        /// <param name="fTotalAngle"></param>
        /// <param name="nCount"></param>
        /// <param name="blnResult"></param>
        private void FilterEdgeAngle_Pad(List<LGauge> arrLineGauges, float fMinBorderScore, ref float fTotalAngle, ref int nCount, ref bool[] blnResult)
        {
            blnResult = new bool[4];

            // Filter angle with score
            float[] fUnsortAngle = new float[4];
            for (int i = 0; i < 4; i++)
            {
                if (!m_arrAutoDontCare[i])
                {
                    if (m_arrDontCareROI[i].Count > 0)
                    {
                        float fDontCareLengthTotal = 0;
                        if (i == 0 || i == 2)   // Top and Bottom Edge ROI
                        {
                            for (int c = 0; c < m_arrDontCareROI[i].Count; c++)
                            {
                                fDontCareLengthTotal += m_arrDontCareROI[i][c].ref_ROIWidth;
                            }
                        }
                        else // Left and Right Edge ROI
                        {
                            for (int c = 0; c < m_arrDontCareROI[i].Count; c++)
                            {
                                fDontCareLengthTotal += m_arrDontCareROI[i][c].ref_ROIHeight;
                            }
                        }

                        float fPassLength = arrLineGauges[i].ref_ObjectScore / 100 * arrLineGauges[i].ref_GaugeLength;
                        float fMeasuredLength = arrLineGauges[i].ref_GaugeLength - fDontCareLengthTotal;

                        float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

                        if (fFinalMeasureScore < m_arrGaugeMinScore[i])
                        {
                            blnResult[i] = false;
                        }
                        else
                        {
                            blnResult[i] = true;
                            if (!arrLineGauges[i].ref_blnReferSubValue)
                                fUnsortAngle[i] = arrLineGauges[i].ref_ObjectAngle;
                            else
                                fUnsortAngle[i] = arrLineGauges[i].ref_ObjectSubAngle;
                            arrLineGauges[i].GetObjectLine();
                        }
                    }
                    else
                    {
                        //if ((arrLineGauges[i].ref_GaugeThickness < 50 && arrLineGauges[i].ref_ObjectScore < 20) ||
                        //    (arrLineGauges[i].ref_GaugeThickness >= 50 && arrLineGauges[i].ref_ObjectScore < 30))

                        if (arrLineGauges[i].ref_ObjectScore < m_arrGaugeMinScore[i])
                        {
                            blnResult[i] = false;
                        }
                        else
                        {
                            blnResult[i] = true;
                            if (!arrLineGauges[i].ref_blnReferSubValue)
                                fUnsortAngle[i] = arrLineGauges[i].ref_ObjectAngle;
                            else
                                fUnsortAngle[i] = arrLineGauges[i].ref_ObjectSubAngle;
                            arrLineGauges[i].GetObjectLine();
                        }
                    }
                }
                else
                {
                    float fDontCareLengthTotal = m_arrAutoDontCareTotalLength[i];
                  
                    float fPassLength = arrLineGauges[i].ref_ObjectScore / 100 * arrLineGauges[i].ref_GaugeLength;
                    float fMeasuredLength = arrLineGauges[i].ref_GaugeLength - fDontCareLengthTotal;

                    float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

                    if (fFinalMeasureScore < m_arrGaugeMinScore[i])
                    {
                        blnResult[i] = false;
                    }
                    else
                    {
                        blnResult[i] = true;
                        if (!arrLineGauges[i].ref_blnReferSubValue)
                            fUnsortAngle[i] = arrLineGauges[i].ref_ObjectAngle;
                        else
                            fUnsortAngle[i] = arrLineGauges[i].ref_ObjectSubAngle;
                        arrLineGauges[i].GetObjectLine();
                    }
                }
            }

            // Sort angle from increase
            List<float> fSortAngle = new List<float>();
            List<int> fSortedMaxAngle = new List<int>();
            for (int i = 0; i < fUnsortAngle.Length; i++)
            {
                if (!blnResult[i])
                    continue;

                int intIndex = fSortAngle.Count;
                for (int j = 0; j < fSortAngle.Count; j++)
                {
                    if (fSortAngle[j] > fUnsortAngle[i])
                    {
                        intIndex = j;
                        break;
                    }
                }
                fSortAngle.Insert(intIndex, fUnsortAngle[i]);
                fSortedMaxAngle.Insert(intIndex, m_arrGaugeMaxAngle[i]);
            }

            // Find the smallest gap between the angle value
            float fSmallestGap = float.MaxValue;
            float fCenterAngle = 0;
            for (int i = 1; i < fSortAngle.Count; i++)
            {
                if ((fSortAngle[i] - fSortAngle[i - 1]) < fSmallestGap)
                {
                    fSmallestGap = fSortAngle[i] - fSortAngle[i - 1];
                    fCenterAngle = (fSortAngle[i] + fSortAngle[i - 1]) / 2;
                }
            }

            // Filter angle with tolerance 3 from smallest angle
            fTotalAngle = 0;
            nCount = 0;
            for (int i = 0; i < fSortAngle.Count; i++)
            {
                if ((GetFormatedAngle(fSortAngle[i]) >= (fCenterAngle - fSortedMaxAngle[i])) && (GetFormatedAngle(fSortAngle[i]) <= (fCenterAngle + fSortedMaxAngle[i])))  // Tolerance 3 value may not suitable for other application
                {
                    fTotalAngle += GetFormatedAngle(fSortAngle[i]);
                    nCount++;
                }
                else
                {
                    fSortAngle[i] = -999;
                }

            }

            // Set the blnResult to false if the angle has been filter out.
            for (int j = 0; j < blnResult.Length; j++)
            {
                if (blnResult[j])
                {
                    bool blnFound = false;
                    for (int i = 0; i < fSortAngle.Count; i++)
                    {
                        if (fUnsortAngle[j] == fSortAngle[i])
                        {
                            blnFound = true;
                            break;
                        }
                    }
                    if (!blnFound)
                        blnResult[j] = false;
                }
            }
        }

        private bool GetPosition_UsingEdgeAndCornerLineGauge(ROI objROI)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                m_arrLineGauge[i].Measure(objROI);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            // Build a virtual edge line if the edge is fail
            if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
            {
                Line objLine = new Line();
                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                m_arrLineGauge[0].ref_ObjectLine = objLine;
                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
            }
            if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
            {
                Line objLine = new Line();
                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                m_arrLineGauge[1].ref_ObjectLine = objLine;
                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
            }
            if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
            {
                Line objLine = new Line();
                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                m_arrLineGauge[2].ref_ObjectLine = objLine;
                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go up
            }
            if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
            {
                Line objLine = new Line();
                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                m_arrLineGauge[3].ref_ObjectLine = objLine;
                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
            }

            bool blnRepeat = false;
            bool isResultOK = true;
            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 306");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Verify corner angle
                isResultOK = true;
                int intBestAngleIndex = -1;
                float fBestAngle = float.MaxValue;
                bool[] arrAngleOK = new bool[4];
                for (int i = 0; i < 4; i++)
                {
                    float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                    if (fAngle > 1)
                    {
                        if (isResultOK)
                            isResultOK = false;
                    }
                    else
                    {
                        arrAngleOK[i] = true;

                        if (fAngle < fBestAngle)
                        {
                            fBestAngle = fAngle;
                            intBestAngleIndex = i;
                        }
                    }

                }

                if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                {
                    blnRepeat = true;

                    int intBestVerIndex = -1, intBestHorIndex = -1;
                    switch (intBestAngleIndex)
                    {
                        case 0:
                            intBestHorIndex = 0;
                            intBestVerIndex = 3;
                            break;
                        case 1:
                            intBestHorIndex = 0;
                            intBestVerIndex = 1;
                            break;
                        case 2:
                            intBestHorIndex = 2;
                            intBestVerIndex = 1;
                            break;
                        case 3:
                            intBestHorIndex = 2;
                            intBestVerIndex = 3;
                            break;
                    }

                    // check can use the good angle to build the virtual line
                    if (!arrAngleOK[0]) // Top Left angle
                    {
                        if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                        {
                            Line objLine = new Line();
                            m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                            m_arrLineGauge[0].ref_ObjectLine = objLine;
                            m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                        }

                        if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                        {
                            Line objLine = new Line();
                            m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                            m_arrLineGauge[3].ref_ObjectLine = objLine;
                            m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                        }

                        arrAngleOK[0] = true;
                    }

                    if (!arrAngleOK[1]) // Top Right angle
                    {
                        if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                        {
                            Line objLine = new Line();
                            m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                            m_arrLineGauge[0].ref_ObjectLine = objLine;
                            m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                        }
                        if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                        {
                            Line objLine = new Line();
                            m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                            m_arrLineGauge[1].ref_ObjectLine = objLine;
                            m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                        }

                        arrAngleOK[1] = true;
                    }

                    if (!arrAngleOK[2]) // Top Right angle
                    {
                        if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                        {
                            Line objLine = new Line();
                            m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                            m_arrLineGauge[2].ref_ObjectLine = objLine;
                            m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                        }
                        if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                        {
                            Line objLine = new Line();
                            m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                            m_arrLineGauge[1].ref_ObjectLine = objLine;
                            m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                        }

                        arrAngleOK[2] = true;
                    }

                    if (!arrAngleOK[3]) // Top Right angle
                    {
                        if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                        {
                            Line objLine = new Line();
                            m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                            m_arrLineGauge[2].ref_ObjectLine = objLine;
                            m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                        }
                        if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                        {
                            Line objLine = new Line();
                            m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                            m_arrLineGauge[3].ref_ObjectLine = objLine;
                            m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                        }

                        arrAngleOK[3] = true;
                    }
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------
            // Verify Unit Size

            if (isResultOK)
            {
                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();
                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;


                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;

                // Verify Unit Size
                if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                else
                    isResultOK = false;
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }
            return true;
        }

        public float AddGrayColorToGaugePoint(ROI objROI, int intGaugeIndex, int intThreshold)
        {
            return m_arrLineGauge[intGaugeIndex].AddGrayColorToGaugePoint(objROI, 0, intGaugeIndex, intThreshold);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objImage"></param>
        /// <param name="blnEnableVirtualLine"></param>
        /// <param name="blnWantCalculateBasedOnPadIndex">True=Calculate Rect Center Point using outer corner point. 
        ///                                               E.g Center ROI using top left corner point to get rect center point x. 
        ///                                               Left ROI using top right corner point to get rect cener point x.
        ///                                     </param>
        /// <param name="intPadIndex"></param>
        /// <returns></returns>
        private bool GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg(ImageDrawing objImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                m_arrLineGauge[i].Measure(objImage);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                }
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go down
                }
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                }
            }
            bool blnRepeat = false;
            bool isResultOK = true;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 304");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                if (blnEnableVirtualLine)
                {

                    // Verify corner angle
                    isResultOK = true;
                    int intBestAngleIndex = -1;
                    float fBestAngle = float.MaxValue;
                    bool[] arrAngleOK = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {

                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                        if (fAngle > m_arrGaugeMaxAngle[i])
                        {
                            if (isResultOK)
                                isResultOK = false;
                        }
                        else
                        {
                            arrAngleOK[i] = true;

                            if (fAngle < fBestAngle)
                            {
                                fBestAngle = fAngle;
                                intBestAngleIndex = i;
                            }
                        }

                    }


                    if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                    {
                        blnRepeat = true;

                        int intBestVerIndex = -1, intBestHorIndex = -1;
                        switch (intBestAngleIndex)
                        {
                            case 0:
                                intBestHorIndex = 0;
                                intBestVerIndex = 3;
                                break;
                            case 1:
                                intBestHorIndex = 0;
                                intBestVerIndex = 1;
                                break;
                            case 2:
                                intBestHorIndex = 2;
                                intBestVerIndex = 1;
                                break;
                            case 3:
                                intBestHorIndex = 2;
                                intBestVerIndex = 3;
                                break;
                        }

                        // check can use the good angle to build the virtual line
                        if (!arrAngleOK[0]) // Top Left angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }

                            if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[0] = true;
                        }

                        if (!arrAngleOK[1]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[1] = true;
                        }

                        if (!arrAngleOK[2]) // Top Right angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[2] = true;
                        }

                        if (!arrAngleOK[3]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[3] = true;
                        }
                    }
                    else
                        break;
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Verify Unit Size

            if (!blnEnableVirtualLine)
            {
                if (nCount < 4)
                {
                    isResultOK = false;
                    m_strErrorMessage = "*Measure Edge Fail.";
                    //m_intFailResultMask |= 0x02;
                    //return false;
                }
            }

            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objImage"></param>
        /// <param name="blnEnableVirtualLine"></param>
        /// <param name="blnWantCalculateBasedOnPadIndex">True=Calculate Rect Center Point using outer corner point. 
        ///                                               E.g Center ROI using top left corner point to get rect center point x. 
        ///                                               Left ROI using top right corner point to get rect cener point x.
        ///                                     </param>
        /// <param name="intPadIndex"></param>
        /// <returns></returns>
        private bool GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_Pad5SPkg(ImageDrawing objImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                m_arrLineGauge[i].Measure(objImage);
            }

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                if (m_arrGaugeEnableSubGauge[i] && !m_arrLineResultOK[i])
                    m_arrLineGauge[i].Measure_SubGauge(objImage, m_arrGaugeMinScore[i]);
                //else 
                //    m_arrLineGauge[i].Measure(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)]);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                }
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go down
                }
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                }
            }
            bool blnRepeat = false;
            bool isResultOK = true;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 308");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                if (blnEnableVirtualLine)
                {

                    // Verify corner angle
                    isResultOK = true;
                    int intBestAngleIndex = -1;
                    float fBestAngle = float.MaxValue;
                    bool[] arrAngleOK = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {

                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                        if (fAngle > m_arrGaugeMaxAngle[i])
                        {
                            if (isResultOK)
                                isResultOK = false;
                        }
                        else
                        {
                            arrAngleOK[i] = true;

                            if (fAngle < fBestAngle)
                            {
                                fBestAngle = fAngle;
                                intBestAngleIndex = i;
                            }
                        }

                    }


                    if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                    {
                        blnRepeat = true;

                        int intBestVerIndex = -1, intBestHorIndex = -1;
                        switch (intBestAngleIndex)
                        {
                            case 0:
                                intBestHorIndex = 0;
                                intBestVerIndex = 3;
                                break;
                            case 1:
                                intBestHorIndex = 0;
                                intBestVerIndex = 1;
                                break;
                            case 2:
                                intBestHorIndex = 2;
                                intBestVerIndex = 1;
                                break;
                            case 3:
                                intBestHorIndex = 2;
                                intBestVerIndex = 3;
                                break;
                        }

                        // check can use the good angle to build the virtual line
                        if (!arrAngleOK[0]) // Top Left angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }

                            if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[0] = true;
                        }

                        if (!arrAngleOK[1]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[1] = true;
                        }

                        if (!arrAngleOK[2]) // Top Right angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[2] = true;
                        }

                        if (!arrAngleOK[3]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[3] = true;
                        }
                    }
                    else
                        break;
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Verify Unit Size

            if (!blnEnableVirtualLine)
            {
                if (nCount < 4)
                {
                    isResultOK = false;
                    m_strErrorMessage = "*Measure Edge Fail.";
                    //m_intFailResultMask |= 0x02;
                    //return false;
                }
            }

            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }

        private bool GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg(List<ImageDrawing> arrImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                m_arrLineGauge[i].Measure(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)]);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                }
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go down
                }
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                }
            }
            bool blnRepeat = false;
            bool isResultOK = true;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 305");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                if (blnEnableVirtualLine)
                {

                    // Verify corner angle
                    isResultOK = true;
                    int intBestAngleIndex = -1;
                    float fBestAngle = float.MaxValue;
                    bool[] arrAngleOK = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {

                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                        if (fAngle > m_arrGaugeMaxAngle[i])
                        {
                            if (isResultOK)
                                isResultOK = false;
                        }
                        else
                        {
                            arrAngleOK[i] = true;

                            if (fAngle < fBestAngle)
                            {
                                fBestAngle = fAngle;
                                intBestAngleIndex = i;
                            }
                        }

                    }


                    if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                    {
                        blnRepeat = true;

                        int intBestVerIndex = -1, intBestHorIndex = -1;
                        switch (intBestAngleIndex)
                        {
                            case 0:
                                intBestHorIndex = 0;
                                intBestVerIndex = 3;
                                break;
                            case 1:
                                intBestHorIndex = 0;
                                intBestVerIndex = 1;
                                break;
                            case 2:
                                intBestHorIndex = 2;
                                intBestVerIndex = 1;
                                break;
                            case 3:
                                intBestHorIndex = 2;
                                intBestVerIndex = 3;
                                break;
                        }

                        // check can use the good angle to build the virtual line
                        if (!arrAngleOK[0]) // Top Left angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }

                            if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[0] = true;
                        }

                        if (!arrAngleOK[1]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[1] = true;
                        }

                        if (!arrAngleOK[2]) // Top Right angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[2] = true;
                        }

                        if (!arrAngleOK[3]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[3] = true;
                        }
                    }
                    else
                        break;
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Verify Unit Size

            if (!blnEnableVirtualLine)
            {
                if (nCount < 4)
                {
                    isResultOK = false;
                    m_strErrorMessage = "*Measure Edge Fail.";
                    //m_intFailResultMask |= 0x02;
                    //return false;
                }
            }

            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg(List<List<ImageDrawing>> arrImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                m_arrLineGauge[i].Measure(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                }
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go down
                }
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                }
            }
            bool blnRepeat = false;
            bool isResultOK = true;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 303");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                if (blnEnableVirtualLine)
                {

                    // Verify corner angle
                    isResultOK = true;
                    int intBestAngleIndex = -1;
                    float fBestAngle = float.MaxValue;
                    bool[] arrAngleOK = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {

                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                        if (fAngle > m_arrGaugeMaxAngle[i])
                        {
                            if (isResultOK)
                                isResultOK = false;
                        }
                        else
                        {
                            arrAngleOK[i] = true;

                            if (fAngle < fBestAngle)
                            {
                                fBestAngle = fAngle;
                                intBestAngleIndex = i;
                            }
                        }

                    }


                    if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                    {
                        blnRepeat = true;

                        int intBestVerIndex = -1, intBestHorIndex = -1;
                        switch (intBestAngleIndex)
                        {
                            case 0:
                                intBestHorIndex = 0;
                                intBestVerIndex = 3;
                                break;
                            case 1:
                                intBestHorIndex = 0;
                                intBestVerIndex = 1;
                                break;
                            case 2:
                                intBestHorIndex = 2;
                                intBestVerIndex = 1;
                                break;
                            case 3:
                                intBestHorIndex = 2;
                                intBestVerIndex = 3;
                                break;
                        }

                        // check can use the good angle to build the virtual line
                        if (!arrAngleOK[0]) // Top Left angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }

                            if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[0] = true;
                        }

                        if (!arrAngleOK[1]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[1] = true;
                        }

                        if (!arrAngleOK[2]) // Top Right angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[2] = true;
                        }

                        if (!arrAngleOK[3]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[3] = true;
                        }
                    }
                    else
                        break;
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------
            // Verify Unit Size

            if (!blnEnableVirtualLine)
            {
                if (nCount < 4)
                {
                    isResultOK = false;
                    m_strErrorMessage = "*Measure Edge Fail.";
                    //m_intFailResultMask |= 0x02;
                    //return false;
                }
            }

            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_Pad5SPkg(List<ImageDrawing> arrImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                if (m_arrGaugeEnableSubGauge[i] && !m_arrLineResultOK[i])
                    m_arrLineGauge[i].Measure_SubGauge(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)], m_arrGaugeMinScore[i]);
                //else 
                //    m_arrLineGauge[i].Measure(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)]);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                }
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go down
                }
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                }
            }
            bool blnRepeat = false;
            bool isResultOK = true;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 3031");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                if (blnEnableVirtualLine)
                {

                    // Verify corner angle
                    isResultOK = true;
                    int intBestAngleIndex = -1;
                    float fBestAngle = float.MaxValue;
                    bool[] arrAngleOK = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {

                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                        if (fAngle > m_arrGaugeMaxAngle[i])
                        {
                            if (isResultOK)
                                isResultOK = false;
                        }
                        else
                        {
                            arrAngleOK[i] = true;

                            if (fAngle < fBestAngle)
                            {
                                fBestAngle = fAngle;
                                intBestAngleIndex = i;
                            }
                        }

                    }


                    if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                    {
                        blnRepeat = true;

                        int intBestVerIndex = -1, intBestHorIndex = -1;
                        switch (intBestAngleIndex)
                        {
                            case 0:
                                intBestHorIndex = 0;
                                intBestVerIndex = 3;
                                break;
                            case 1:
                                intBestHorIndex = 0;
                                intBestVerIndex = 1;
                                break;
                            case 2:
                                intBestHorIndex = 2;
                                intBestVerIndex = 1;
                                break;
                            case 3:
                                intBestHorIndex = 2;
                                intBestVerIndex = 3;
                                break;
                        }

                        // check can use the good angle to build the virtual line
                        if (!arrAngleOK[0]) // Top Left angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }

                            if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[0] = true;
                        }

                        if (!arrAngleOK[1]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[1] = true;
                        }

                        if (!arrAngleOK[2]) // Top Right angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[2] = true;
                        }

                        if (!arrAngleOK[3]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[3] = true;
                        }
                    }
                    else
                        break;
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Verify Unit Size

            if (!blnEnableVirtualLine)
            {
                if (nCount < 4)
                {
                    isResultOK = false;
                    m_strErrorMessage = "*Measure Edge Fail.";
                    //m_intFailResultMask |= 0x02;
                    //return false;
                }
            }

            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_Pad5SPkg(List<List<ImageDrawing>> arrImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                if (!m_arrLineResultOK[i]) //(m_arrGaugeEnableSubGauge[i] && !m_arrLineResultOK[i])
                    m_arrLineGauge[i].Measure_SubGauge(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i], m_arrGaugeMinScore[i]);
                //else 
                //    m_arrLineGauge[i].Measure(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)]);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                }
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go down
                }
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                }
            }
            bool blnRepeat = false;
            bool isResultOK = true;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 3032");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                if (blnEnableVirtualLine)
                {

                    // Verify corner angle
                    isResultOK = true;
                    int intBestAngleIndex = -1;
                    float fBestAngle = float.MaxValue;
                    bool[] arrAngleOK = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {

                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                        if (fAngle > m_arrGaugeMaxAngle[i])
                        {
                            if (isResultOK)
                                isResultOK = false;
                        }
                        else
                        {
                            arrAngleOK[i] = true;

                            if (fAngle < fBestAngle)
                            {
                                fBestAngle = fAngle;
                                intBestAngleIndex = i;
                            }
                        }

                    }


                    if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                    {
                        blnRepeat = true;

                        int intBestVerIndex = -1, intBestHorIndex = -1;
                        switch (intBestAngleIndex)
                        {
                            case 0:
                                intBestHorIndex = 0;
                                intBestVerIndex = 3;
                                break;
                            case 1:
                                intBestHorIndex = 0;
                                intBestVerIndex = 1;
                                break;
                            case 2:
                                intBestHorIndex = 2;
                                intBestVerIndex = 1;
                                break;
                            case 3:
                                intBestHorIndex = 2;
                                intBestVerIndex = 3;
                                break;
                        }

                        // check can use the good angle to build the virtual line
                        if (!arrAngleOK[0]) // Top Left angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }

                            if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[0] = true;
                        }

                        if (!arrAngleOK[1]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[1] = true;
                        }

                        if (!arrAngleOK[2]) // Top Right angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[2] = true;
                        }

                        if (!arrAngleOK[3]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[3] = true;
                        }
                    }
                    else
                        break;
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Verify Unit Size

            if (!blnEnableVirtualLine)
            {
                if (nCount < 4)
                {
                    isResultOK = false;
                    m_strErrorMessage = "*Measure Edge Fail.";
                    //m_intFailResultMask |= 0x02;
                    //return false;
                }
            }

            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_Pad5SPkg_SubGaugePoints(List<List<ImageDrawing>> arrImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);  // 2020 08 14 - CCENG - prevent out of index when use wrong recipe
                if (intROIIndex >= arrImage.Count)
                    intROIIndex = 0;

                ROI objROI = new ROI();
                objROI.AttachImage(arrImage[intROIIndex][i]);
                objROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX, m_arrEdgeROI[i].ref_ROIPositionY, m_arrEdgeROI[i].ref_ROIWidth, m_arrEdgeROI[i].ref_ROIHeight);

                if (!m_arrLineResultOK[i])//(m_arrGaugeEnableSubGauge[i] && !m_arrLineResultOK[i])
                    m_arrLineGauge[i].Measure_SubGaugePoints(objROI, m_arrGaugeMinScore[i]);
                //else 
                //    m_arrLineGauge[i].Measure(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)]);

                objROI.Dispose();
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // 2020 03 08 - CCENG: for side ROI, use top gauge angle bcos most of the pad stand off rely on this top gauge angle.
            //                     Also if using bottom gauge angle, it is not stable normally.
            bool blnSingleAnglePass = false;
            if (intPadIndex == 1)
            {
                if (blnResult[0])
                {
                    m_fRectAngle = m_arrLineGauge[0].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 2)
            {
                if (blnResult[1])
                {
                    m_fRectAngle = m_arrLineGauge[1].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 3)
            {
                if (blnResult[2])
                {
                    m_fRectAngle = m_arrLineGauge[2].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 4)
            {
                if (blnResult[3])
                {
                    m_fRectAngle = m_arrLineGauge[3].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }

            if (!blnSingleAnglePass)
            {
                // Get average angle from Edge Line Gauges Angle
                m_fRectAngle = fTotalAngle / nCount;
            }

            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                }
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go down
                }
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                }
            }
            bool blnRepeat = false;
            bool isResultOK = true;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 3032");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                if (blnEnableVirtualLine)
                {

                    // Verify corner angle
                    isResultOK = true;
                    int intBestAngleIndex = -1;
                    float fBestAngle = float.MaxValue;
                    bool[] arrAngleOK = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {

                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                        if (fAngle > m_arrGaugeMaxAngle[i])
                        {
                            if (isResultOK)
                                isResultOK = false;
                        }
                        else
                        {
                            arrAngleOK[i] = true;

                            if (fAngle < fBestAngle)
                            {
                                fBestAngle = fAngle;
                                intBestAngleIndex = i;
                            }
                        }

                    }


                    if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                    {
                        blnRepeat = true;

                        int intBestVerIndex = -1, intBestHorIndex = -1;
                        switch (intBestAngleIndex)
                        {
                            case 0:
                                intBestHorIndex = 0;
                                intBestVerIndex = 3;
                                break;
                            case 1:
                                intBestHorIndex = 0;
                                intBestVerIndex = 1;
                                break;
                            case 2:
                                intBestHorIndex = 2;
                                intBestVerIndex = 1;
                                break;
                            case 3:
                                intBestHorIndex = 2;
                                intBestVerIndex = 3;
                                break;
                        }

                        // check can use the good angle to build the virtual line
                        if (!arrAngleOK[0]) // Top Left angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }

                            if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[0] = true;
                        }

                        if (!arrAngleOK[1]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[1] = true;
                        }

                        if (!arrAngleOK[2]) // Top Right angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[2] = true;
                        }

                        if (!arrAngleOK[3]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[3] = true;
                        }
                    }
                    else
                        break;
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Verify Unit Size

            if (!blnEnableVirtualLine)
            {
                if (nCount < 4)
                {
                    isResultOK = false;
                    m_strErrorMessage = "*Measure Edge Fail.";
                    //m_intFailResultMask |= 0x02;
                    //return false;
                }
            }

            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }
        private bool DefineCornerPoints(List<LGauge> arrLineGauges, float fUnitWidth, float fUnitHeight, float fMinBorderScore, ref float fTotalAngle, ref int nCount, ref bool[] blnResult, ref PointF[] arrCorner)
        {
            //  4		0	      6
            // -------------------
            //5|corner0	    corner1|7
            // |			       |
            // |			       |
            // |			       |
            //3|			       |1
            // |			       |
            // |			       |
            // |			       |
            // |			       |
            //9|corner3	    corner2|11
            // -------------------
            //  8		2	      10

            blnResult = new bool[arrLineGauges.Count];

            // Filter angle with score
            int intFailCount = 0;
            float[] fUnsortAngle = new float[arrLineGauges.Count];
            for (int i = 0; i < arrLineGauges.Count; i++)
            {
                if ((i < 4) && arrLineGauges[i].ref_ObjectScore < fMinBorderScore)  // reject if edge gauge score lower than setting
                {
                    blnResult[i] = false;
                    intFailCount++;
                }
                else if ((i >= 4) && arrLineGauges[i].ref_ObjectScore < (fMinBorderScore / 3))  // reject if corner gauge score lower then setting/3 (divide 3 because corner gauge will not coverage whole unit edge) 
                {
                    blnResult[i] = false;
                    intFailCount++;
                }
                else
                {
                    blnResult[i] = true;
                    fUnsortAngle[i] = GetFormatedAngle(arrLineGauges[i].ref_ObjectAngle);
                    arrLineGauges[i].GetObjectLine();
                }
            }

            // Get mode 
            float fMinData = float.MaxValue;
            float fMaxData = float.MinValue;
            Math2.MinMax(fUnsortAngle, blnResult, ref fMinData, ref fMaxData);  // Get min angle and max angle
            float fRange = fMaxData - fMinData;                                 // Get the rangle between min and max angle
            float fWidth = 1; // Start with 1.
            float fStartBar;
            float fEndBar;
            int intHighestRatioBar = 0;
            float fHighestRatio = 0;
            float fHigestRatioWidth = 0;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 302");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                int intTotalBar = (int)Math.Ceiling(fRange / fWidth);
                int[] intMatchCount = new int[intTotalBar];
                int intHighestCount = 0;
                int intHigestBar = 0;

                // Scan all bar
                for (int c = 0; c < intTotalBar; c++)
                {
                    // Calculate the bar start and end value
                    fStartBar = fMinData + fWidth * c;
                    fEndBar = fMinData + fWidth * (c + 1);

                    // Assign all angle list into bar
                    for (int i = 0; i < fUnsortAngle.Length; i++)
                    {
                        // skip assign if the angle result is false
                        if (!blnResult[i])
                            continue;

                        // within bar
                        if ((fUnsortAngle[i] >= fStartBar) && (fUnsortAngle[i] < fEndBar))
                        {
                            intMatchCount[c]++;
                        }
                    }

                    // Keep highest count bar
                    if (intHighestCount < intMatchCount[c])
                    {
                        intHighestCount = intMatchCount[c];
                        intHigestBar = c;
                    }
                }

                // ------- Get HIGHTEST RATIO -----------------------------
                float fRatio = (float)intHighestCount / fWidth;
                if (fHighestRatio < fRatio)
                {
                    fHighestRatio = fRatio;
                    intHighestRatioBar = intHigestBar;
                    fHigestRatioWidth = fWidth;
                }
                // --------------------------------------------------------

                // stop check if valid angles is above 75% from angles list.
                float fPercentage = (float)intHighestCount / (fUnsortAngle.Length - intFailCount);
                if (fPercentage >= 0.75 && (fUnsortAngle.Length - intFailCount) != 0)
                    break;

                // Increase bar width 0.5 each loop
                fWidth += 0.5f;

            } while (true);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Find the lowest and highest angle value within the highest ratio bar
            float fLowestValue;
            float fHighestValue;
            fStartBar = fLowestValue = fMinData + fHigestRatioWidth * intHighestRatioBar;
            fEndBar = fHighestValue = fMinData + fHigestRatioWidth * (intHighestRatioBar + 1);
            for (int i = 0; i < fUnsortAngle.Length; i++)
            {
                if (!blnResult[i])
                    continue;

                // Check is angle value within the highest ratio bar
                if ((fUnsortAngle[i] >= fStartBar) && (fUnsortAngle[i] < fEndBar))
                {
                    // Get lowest angle value
                    if (fLowestValue > fUnsortAngle[i])
                        fLowestValue = fUnsortAngle[i];

                    // Get highest angle value
                    if (fHighestValue < fUnsortAngle[i])
                        fHighestValue = fUnsortAngle[i];
                }
            }

            // Get Mode for angle
            float fModeAngle = (fLowestValue + fHighestValue) / 2;
            float fSelectedAngle = fTotalAngle = fModeAngle;

            // create line using line gauge
            for (int i = 0; i < arrLineGauges.Count; i++)
            {
                if (!blnResult[i])
                    continue;

                arrLineGauges[i].GetObjectLine();
            }


            #region Determine which line to use (corner gauge line or edge gauge line

            //  4		0	      6         arrLineGauges
            // -------------------
            //5|corner0	    corner1|7
            // |			       |
            // |			       |
            // |			       |
            //3|			       |1
            // |			       |
            // |			       |
            // |			       |
            // |			       |
            //9|corner3	    corner2|11
            // -------------------
            //  8		2	      10

            m_arrCornerLine = new Line[8];
            bool[] arrFailLines2 = new bool[8];

            if (blnResult[4] && blnResult[0])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[4].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[0].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    m_arrCornerLine[0] = arrLineGauges[4].ref_ObjectLine;
                else
                    m_arrCornerLine[0] = arrLineGauges[0].ref_ObjectLine;
            }
            else if (blnResult[0])
                m_arrCornerLine[0] = arrLineGauges[0].ref_ObjectLine;
            else if (blnResult[4])
                m_arrCornerLine[0] = arrLineGauges[4].ref_ObjectLine;
            else
                arrFailLines2[0] = true;

            if (blnResult[5] && blnResult[3])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[5].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[3].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    m_arrCornerLine[1] = arrLineGauges[5].ref_ObjectLine;
                else
                    m_arrCornerLine[1] = arrLineGauges[3].ref_ObjectLine;
            }
            else if (blnResult[3])
                m_arrCornerLine[1] = arrLineGauges[3].ref_ObjectLine;
            else if (blnResult[5])
                m_arrCornerLine[1] = arrLineGauges[5].ref_ObjectLine;
            else
                arrFailLines2[1] = true;

            if (blnResult[6] && blnResult[0])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[6].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[0].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    m_arrCornerLine[2] = arrLineGauges[6].ref_ObjectLine;
                else
                    m_arrCornerLine[2] = arrLineGauges[0].ref_ObjectLine;
            }
            else if (blnResult[0])
                m_arrCornerLine[2] = arrLineGauges[0].ref_ObjectLine;
            else if (blnResult[6])
                m_arrCornerLine[2] = arrLineGauges[6].ref_ObjectLine;
            else
                arrFailLines2[2] = true;


            if (blnResult[7] && blnResult[1])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[7].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[1].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    m_arrCornerLine[3] = arrLineGauges[7].ref_ObjectLine;
                else
                    m_arrCornerLine[3] = arrLineGauges[1].ref_ObjectLine;
            }
            else if (blnResult[1])
                m_arrCornerLine[3] = arrLineGauges[1].ref_ObjectLine;
            else if (blnResult[7])
                m_arrCornerLine[3] = arrLineGauges[7].ref_ObjectLine;
            else
                arrFailLines2[3] = true;

            if (blnResult[8] && blnResult[2])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[8].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[2].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    m_arrCornerLine[4] = arrLineGauges[8].ref_ObjectLine;
                else
                    m_arrCornerLine[4] = arrLineGauges[2].ref_ObjectLine;
            }
            else if (blnResult[2])
                m_arrCornerLine[4] = arrLineGauges[2].ref_ObjectLine;
            else if (blnResult[8])
                m_arrCornerLine[4] = arrLineGauges[8].ref_ObjectLine;
            else
                arrFailLines2[4] = true;

            if (blnResult[9] && blnResult[3])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[9].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[3].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    m_arrCornerLine[5] = arrLineGauges[9].ref_ObjectLine;
                else
                    m_arrCornerLine[5] = arrLineGauges[3].ref_ObjectLine;
            }
            else if (blnResult[3])
                m_arrCornerLine[5] = arrLineGauges[3].ref_ObjectLine;
            else if (blnResult[9])
                m_arrCornerLine[5] = arrLineGauges[9].ref_ObjectLine;
            else
                arrFailLines2[5] = true;

            if (blnResult[10] && blnResult[2])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[10].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[2].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    m_arrCornerLine[6] = arrLineGauges[10].ref_ObjectLine;
                else
                    m_arrCornerLine[6] = arrLineGauges[2].ref_ObjectLine;
            }
            else if (blnResult[2])
                m_arrCornerLine[6] = arrLineGauges[2].ref_ObjectLine;
            else if (blnResult[10])
                m_arrCornerLine[6] = arrLineGauges[10].ref_ObjectLine;
            else
                arrFailLines2[6] = true;

            if (blnResult[11] && blnResult[1])
            {
                if (Math.Abs(GetFormatedAngle((float)arrLineGauges[11].ref_ObjectLine.ref_dAngle) - fSelectedAngle) < Math.Abs(GetFormatedAngle((float)arrLineGauges[1].ref_ObjectLine.ref_dAngle) - fSelectedAngle))
                    m_arrCornerLine[7] = arrLineGauges[11].ref_ObjectLine;
                else
                    m_arrCornerLine[7] = arrLineGauges[1].ref_ObjectLine;
            }
            else if (blnResult[1])
                m_arrCornerLine[7] = arrLineGauges[1].ref_ObjectLine;
            else if (blnResult[11])
                m_arrCornerLine[7] = arrLineGauges[11].ref_ObjectLine;
            else
                arrFailLines2[7] = true;

            #endregion       

            // Get corner points
            //  0			      2     arrFailLines2/m_arrCornerLine
            //  -------------------
            //1|corner0	    corner1|3
            // |			       |
            // |			       |
            // |			       |
            // |			       |
            // |			       |
            // |			       |
            // |			       |
            // |			       |
            //5|corner3	    corner2|7
            // --------------------
            //  4			      6

            float[] arrCornerGaugeAngles = new float[4];
            if ((arrFailLines2[0] && arrFailLines2[1]) || // if both one top left line gauge fail
                (arrFailLines2[0] && arrFailLines2[4]) ||
                (arrFailLines2[1] && arrFailLines2[3]))
            {

                // Set corner point to 0
                arrCorner[0] = new PointF(0, 0);
                arrCornerGaugeAngles[0] = 0f;
            }
            else
            {
                if (arrFailLines2[0])
                {
                    Line objLine = new Line();
                    m_arrCornerLine[4].CopyTo(ref objLine);
                    m_arrCornerLine[0] = objLine;
                    m_arrCornerLine[0].ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                else if (arrFailLines2[1])
                {
                    Line objLine = new Line();
                    m_arrCornerLine[3].CopyTo(ref objLine);
                    m_arrCornerLine[1] = objLine;
                    m_arrCornerLine[1].ShiftXLine(-m_fUnitSizeWidth);    // use "-" because X decrease when go to left
                }

                // Get corner point and angle base on final selected line gauge
                arrCorner[0] = Line.GetCrossPoint(m_arrCornerLine[0], m_arrCornerLine[1]);
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrCornerLine[0], m_arrCornerLine[1]);
            }

            if ((arrFailLines2[2] && arrFailLines2[3]) ||// if either one top right line gauge fail
                (arrFailLines2[2] && arrFailLines2[6]) ||
                (arrFailLines2[3] && arrFailLines2[1]))
            {
                // Set corner point to 0
                arrCorner[1] = new PointF(0, 0);
                arrCornerGaugeAngles[1] = 0f;
            }
            else
            {
                if (arrFailLines2[2])
                {
                    Line objLine = new Line();
                    m_arrCornerLine[6].CopyTo(ref objLine);
                    m_arrCornerLine[2] = objLine;
                    m_arrCornerLine[2].ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up

                }
                else if (arrFailLines2[3])
                {
                    Line objLine = new Line();
                    m_arrCornerLine[1].CopyTo(ref objLine);
                    m_arrCornerLine[3] = objLine;
                    m_arrCornerLine[3].ShiftXLine(m_fUnitSizeWidth);    // use "+" because X increase when go to right

                }

                // Get corner point and angle base on final selected line gauge
                arrCorner[1] = Line.GetCrossPoint(m_arrCornerLine[2], m_arrCornerLine[3]);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrCornerLine[2], m_arrCornerLine[3]);

            }


            if ((arrFailLines2[6] && arrFailLines2[7]) ||// if either one bottom right line gauge fail
               (arrFailLines2[6] && arrFailLines2[2]) ||
               (arrFailLines2[7] && arrFailLines2[5]))
            {
                // Set corner point to 0
                arrCorner[2] = new PointF(0, 0);
                arrCornerGaugeAngles[2] = 0f;
            }
            else
            {
                if (arrFailLines2[6])
                {
                    Line objLine = new Line();
                    m_arrCornerLine[2].CopyTo(ref objLine);
                    m_arrCornerLine[6] = objLine;
                    m_arrCornerLine[6].ShiftYLine(m_fUnitSizeHeight);    // use "+" because Y increase when go down
                }
                else if (arrFailLines2[7])
                {
                    Line objLine = new Line();
                    m_arrCornerLine[5].CopyTo(ref objLine);
                    m_arrCornerLine[7] = objLine;
                    m_arrCornerLine[7].ShiftXLine(m_fUnitSizeWidth);    // use "+" because X increase when go to right
                }
                // Get corner point and angle base on final selected line gauge
                arrCorner[2] = Line.GetCrossPoint(m_arrCornerLine[6], m_arrCornerLine[7]);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrCornerLine[6], m_arrCornerLine[7]);
            }


            if ((arrFailLines2[4] && arrFailLines2[5]) || // if either one bottom left line gauge fail
                (arrFailLines2[4] && arrFailLines2[0]) ||
                (arrFailLines2[5] && arrFailLines2[7]))
            {
                // Set corner point to 0
                arrCorner[3] = new PointF(0, 0);
                arrCornerGaugeAngles[3] = 0f;
            }
            else
            {
                if (arrFailLines2[4])
                {
                    Line objLine = new Line();
                    m_arrCornerLine[0].CopyTo(ref objLine);
                    m_arrCornerLine[4] = objLine;
                    m_arrCornerLine[4].ShiftYLine(m_fUnitSizeHeight);    // use "+" because Y increase when go down
                }
                else if (arrFailLines2[5])
                {
                    Line objLine = new Line();
                    m_arrCornerLine[7].CopyTo(ref objLine);
                    m_arrCornerLine[5] = objLine;
                    m_arrCornerLine[5].ShiftXLine(-m_fUnitSizeWidth);    // use "-" because X increase when go to left
                }

                // Get corner point and angle base on final selected line gauge
                arrCorner[3] = Line.GetCrossPoint(m_arrCornerLine[4], m_arrCornerLine[5]);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrCornerLine[4], m_arrCornerLine[5]);   // Angle will near to 90deg
            }

            // Loop and Scan all corner angle and check how many angle is out of (85 to 88)deg <-> (95 to 92)deg
            List<int> arrFailAngle5Index = new List<int>();
            float fLimit = 5;

            // ------------------- checking loop timeout ---------------------------------------------------
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 301");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                arrFailAngle5Index.Clear();
                for (int i = 0; i < 4; i++)
                {
                    if (Math.Abs(arrCornerGaugeAngles[i] - 90) > fLimit)
                    {
                        arrFailAngle5Index.Add(i);
                    }
                }

                if (arrFailAngle5Index.Count > 0) // stop checking once got 1 fail.
                    break;

                fLimit -= 0.5f;

            } while (fLimit > 2);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            if (arrFailAngle5Index.Count == 1)    // 1 angle out of (85 to 88)deg <-> (95 to 92)deg
            {
                // Scan all corner angle and check how many angle is out of 88deg <-> 92deg
                List<int> arrFailAngle2Index = new List<int>();
                for (int i = 0; i < 4; i++)
                {
                    if (Math.Abs(arrCornerGaugeAngles[i] - 90) > 2)
                    {
                        arrFailAngle2Index.Add(i);
                    }
                }

                bool blnDone = false;
                if (arrFailAngle2Index.Count == 2)   // if 2 corner out of 88deg <-> 92deg
                {
                    switch (arrFailAngle5Index[0])
                    {
                        case 0:
                        case 2:
                            //  xo
                            //  ox
                            if (arrFailAngle2Index[0] == 0 && arrFailAngle2Index[1] == 2)
                            {
                                arrCorner[0] = Line.GetCrossPoint(m_arrCornerLine[2], m_arrCornerLine[5]);  // Use last line gauge to find the point
                                arrCorner[2] = Line.GetCrossPoint(m_arrCornerLine[3], m_arrCornerLine[4]);
                                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrCornerLine[2], m_arrCornerLine[5]);
                                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrCornerLine[3], m_arrCornerLine[4]);
                                blnDone = true;
                            }
                            break;
                        case 1:
                        case 3:
                            //  ox
                            //  xo
                            if (arrFailAngle2Index[0] == 1 && arrFailAngle2Index[1] == 3)
                            {
                                arrCorner[1] = Line.GetCrossPoint(m_arrCornerLine[0], m_arrCornerLine[7]);
                                arrCorner[3] = Line.GetCrossPoint(m_arrCornerLine[1], m_arrCornerLine[6]);
                                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrCornerLine[0], m_arrCornerLine[7]);
                                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrCornerLine[1], m_arrCornerLine[6]);
                                blnDone = true;
                            }
                            break;
                    }
                }

                if (!blnDone)
                {

                    m_objVirtualLineWidth = new Line();
                    m_objVirtualLineHeight = new Line();
                    // calculate the fail point using other 3 correct points
                    float fShiftHeight, fShiftWidth;
                    switch (arrFailAngle5Index[0])    // fail which corner
                    {
                        case 0: // Fail top left corner
                            // built hor virtual line gauge for top left corner (select better angle line gauge and then copy it to virtual line gauge)
                            if (Math.Abs(m_arrCornerLine[4].ref_dAngle - fSelectedAngle) < Math.Abs(m_arrCornerLine[6].ref_dAngle - fSelectedAngle))
                                m_arrCornerLine[4].CopyTo(ref m_objVirtualLineWidth);
                            else
                                m_arrCornerLine[6].CopyTo(ref m_objVirtualLineWidth);

                            fShiftHeight = Math2.GetDistanceBtw2Points(arrCorner[1], arrCorner[2]); // Get rect height measurement
                            m_objVirtualLineWidth.ShiftYLine(-fShiftHeight);                        // shift the virtual line gauge to replace the 0 index hor line gauge.

                            // built ver virtual line gauge for top left corner (select better angle line gauge and then copy it to virtual line gauge)
                            if (Math.Abs(m_arrCornerLine[3].ref_dAngle - fSelectedAngle) < Math.Abs(m_arrCornerLine[7].ref_dAngle - fSelectedAngle))
                                m_arrCornerLine[3].CopyTo(ref m_objVirtualLineHeight);
                            else
                                m_arrCornerLine[7].CopyTo(ref m_objVirtualLineHeight);

                            fShiftWidth = Math2.GetDistanceBtw2Points(arrCorner[2], arrCorner[3]);  // Get rect width measurement
                            m_objVirtualLineHeight.ShiftXLine2(-fShiftWidth);                       // shift the virtual line gauge to replace the 0 index ver line gauge
                            break;
                        case 1: // Fail top right corner
                            // built hor virtual line gauge for top left corner (select better angle line gauge and then copy it to virtual line gauge)
                            if (Math.Abs(m_arrCornerLine[4].ref_dAngle - fSelectedAngle) < Math.Abs(m_arrCornerLine[6].ref_dAngle - fSelectedAngle))
                                m_arrCornerLine[4].CopyTo(ref m_objVirtualLineWidth);
                            else
                                m_arrCornerLine[6].CopyTo(ref m_objVirtualLineWidth);

                            fShiftHeight = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[3]);
                            m_objVirtualLineWidth.ShiftYLine(-fShiftHeight);

                            // y line
                            if (Math.Abs(m_arrCornerLine[1].ref_dAngle - fSelectedAngle) < Math.Abs(m_arrCornerLine[5].ref_dAngle - fSelectedAngle))
                                m_arrCornerLine[1].CopyTo(ref m_objVirtualLineHeight);
                            else
                                m_arrCornerLine[5].CopyTo(ref m_objVirtualLineHeight);

                            fShiftWidth = Math2.GetDistanceBtw2Points(arrCorner[2], arrCorner[3]);
                            m_objVirtualLineHeight.ShiftXLine2(fShiftWidth);
                            break;
                        case 2: // Fail bottom left corner
                            // x line
                            if (Math.Abs(m_arrCornerLine[0].ref_dAngle - fSelectedAngle) < Math.Abs(m_arrCornerLine[2].ref_dAngle - fSelectedAngle))
                                m_arrCornerLine[0].CopyTo(ref m_objVirtualLineWidth);
                            else
                                m_arrCornerLine[2].CopyTo(ref m_objVirtualLineWidth);

                            fShiftHeight = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[3]);
                            m_objVirtualLineWidth.ShiftYLine(fShiftHeight);

                            // y line
                            if (Math.Abs(m_arrCornerLine[1].ref_dAngle - fSelectedAngle) < Math.Abs(m_arrCornerLine[5].ref_dAngle - fSelectedAngle))
                                m_arrCornerLine[1].CopyTo(ref m_objVirtualLineHeight);
                            else
                                m_arrCornerLine[5].CopyTo(ref m_objVirtualLineHeight);

                            fShiftWidth = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[1]);
                            m_objVirtualLineHeight.ShiftXLine2(fShiftWidth);
                            break;
                        case 3: // Fail bottom right corner
                            // x line
                            if (Math.Abs(m_arrCornerLine[0].ref_dAngle - fSelectedAngle) < Math.Abs(m_arrCornerLine[2].ref_dAngle - fSelectedAngle))
                                m_arrCornerLine[0].CopyTo(ref m_objVirtualLineWidth);
                            else
                                m_arrCornerLine[2].CopyTo(ref m_objVirtualLineWidth);

                            fShiftHeight = Math2.GetDistanceBtw2Points(arrCorner[1], arrCorner[2]);
                            m_objVirtualLineWidth.ShiftYLine(fShiftHeight);

                            // y line
                            if (Math.Abs(m_arrCornerLine[3].ref_dAngle - fSelectedAngle) < Math.Abs(m_arrCornerLine[7].ref_dAngle - fSelectedAngle))
                                m_arrCornerLine[3].CopyTo(ref m_objVirtualLineHeight);
                            else
                                m_arrCornerLine[7].CopyTo(ref m_objVirtualLineHeight);

                            fShiftWidth = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[1]);
                            m_objVirtualLineHeight.ShiftXLine2(-fShiftWidth);
                            break;
                    }

                    arrCorner[arrFailAngle5Index[0]] = Line.GetCrossPoint(m_objVirtualLineWidth, m_objVirtualLineHeight);
                    arrCornerGaugeAngles[arrFailAngle5Index[0]] = Math2.GetAngle(m_objVirtualLineWidth, m_objVirtualLineHeight);
                }
            }
            else if (arrFailAngle5Index.Count == 2)   // 2 angles out of (85 to 88)deg <-> (95 to 92)deg
            {
                if ((arrFailAngle5Index[0] == 0 && arrFailAngle5Index[1] == 2) ||
                    (arrFailAngle5Index[0] == 1 && arrFailAngle5Index[1] == 3))
                {
                    List<int> arrFailAngle2Index = new List<int>();
                    for (int i = 0; i < 4; i++)
                    {
                        if (Math.Abs(arrCornerGaugeAngles[i] - 90) > 2)
                        {
                            arrFailAngle2Index.Add(i);
                        }
                    }

                    if (arrFailAngle2Index.Count == 2)
                    {
                        switch (arrFailAngle5Index[0])
                        {
                            case 0:
                                arrCorner[0] = Line.GetCrossPoint(m_arrCornerLine[2], m_arrCornerLine[5]);
                                arrCorner[2] = Line.GetCrossPoint(m_arrCornerLine[3], m_arrCornerLine[4]);
                                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrCornerLine[2], m_arrCornerLine[5]);
                                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrCornerLine[3], m_arrCornerLine[4]);
                                break;
                            case 1:
                                arrCorner[1] = Line.GetCrossPoint(m_arrCornerLine[0], m_arrCornerLine[7]);
                                arrCorner[3] = Line.GetCrossPoint(m_arrCornerLine[1], m_arrCornerLine[6]);
                                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrCornerLine[0], m_arrCornerLine[7]);
                                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrCornerLine[1], m_arrCornerLine[6]);
                                break;
                        }
                    }
                    else
                    {
                        m_strErrorMessage = "Corner Points Fail. There are 2 or more invalid corner points";
                        m_intFailResultMask |= 0x02;
                        return false;
                    }
                }
                else
                {
                    //bool blnBuildVirtualLineUsingSize = true;
                    //if (blnBuildVirtualLineUsingSize)
                    //{
                    //    List<int> arrFailAngle2Index = new List<int>();
                    //    for (int i = 0; i < 4; i++)
                    //    {
                    //        if (Math.Abs(arrCornerGaugeAngles[i] - 90) > 2)
                    //        {
                    //            arrFailAngle2Index.Add(i);
                    //        }
                    //    }

                    //    if (arrFailAngle2Index.Count == 2)
                    //    {
                    //        if (arrFailAngle5Index[0] == 0 && arrFailAngle5Index[1] == 1)
                    //        {
                    //        }
                    //    }
                    //}

                    m_strErrorMessage = "Corner Points Fail. There are 2 or more invalid corner points";
                    m_intFailResultMask |= 0x02;
                    return false;
                }
            }
            else if (arrFailAngle5Index.Count > 2)
            {
                m_strErrorMessage = "Corner Points Fail. There are 2 or more invalid corner points";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Verify are corners points correct
            //Verify4CornerPoint(fUnitWidth, fUnitHeight, ref arrCorner);
            // create line
            float[] arrCornerAngles = new float[4];
            float[] arrEdgeDistances = new float[4];

            arrEdgeDistances[0] = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[1]);
            arrEdgeDistances[1] = Math2.GetDistanceBtw2Points(arrCorner[1], arrCorner[2]);
            arrEdgeDistances[2] = Math2.GetDistanceBtw2Points(arrCorner[2], arrCorner[3]);
            arrEdgeDistances[3] = Math2.GetDistanceBtw2Points(arrCorner[3], arrCorner[0]);

            float fCloseDistance1 = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[2]);
            float fCloseDistance2 = Math2.GetDistanceBtw2Points(arrCorner[1], arrCorner[3]);

            arrCornerAngles[0] = Math2.GetAngle(arrEdgeDistances[0], arrEdgeDistances[3], fCloseDistance2);
            arrCornerAngles[1] = Math2.GetAngle(arrEdgeDistances[0], arrEdgeDistances[1], fCloseDistance1);
            arrCornerAngles[2] = Math2.GetAngle(arrEdgeDistances[1], arrEdgeDistances[2], fCloseDistance2);
            arrCornerAngles[3] = Math2.GetAngle(arrEdgeDistances[2], arrEdgeDistances[3], fCloseDistance1);

            int intFailCount2 = 0;
            for (int i = 0; i < 4; i++)
            {
                if (Math.Abs(arrCornerAngles[i] - 90) > 3)
                {
                    arrCorner[i] = new PointF(0, 0);
                    intFailCount2++;
                }
            }

            if (intFailCount2 >= 2)
            {
                m_strErrorMessage = "*Fail to measure clear corner edge.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            return true;
        }

        private float GetFormatedAngle(float fSourceAngle)
        {
            if (fSourceAngle > -45 && fSourceAngle <= 45)
                return fSourceAngle;
            else if (fSourceAngle > 45 && fSourceAngle <= 135)
                return fSourceAngle - 90;
            else if (fSourceAngle > 135 && fSourceAngle <= 225)
                return fSourceAngle - 180;
            else if (fSourceAngle > 225 && fSourceAngle <= 315)
                return fSourceAngle - 270;
            else if (fSourceAngle > 315)
                return fSourceAngle - 360;
            else if (fSourceAngle > -135 && fSourceAngle <= -45)
                return fSourceAngle + 90;
            else if (fSourceAngle > -225 && fSourceAngle <= -135)
                return fSourceAngle + 180;
            else if (fSourceAngle > -315 && fSourceAngle <= -225)
                return fSourceAngle + 270;
            else if (fSourceAngle <= -315)
                return fSourceAngle + 360;
            else
                return fSourceAngle;
        }


        private bool GetPositionByCorner(List<LGauge> arrLineGauges, float fUnitWidth, float fUnitHeight, float fMinBorderScore,
                                ref PointF pMeasureCenterPoint, ref PointF[] arrCorner, ref float fMeasureWidth, ref float fMeasureHeight,
                                ref float fMeasureAngle, ref string strErrorMessage)
        {
            //  4		0	      6
            // -------------------
            //5|corner0	    corner1|7
            // |			       |
            // |			       |
            // |			       |
            //3|			       |1
            // |			       |
            // |			       |
            // |			       |
            // |			       |
            //9|corner3	    corner2|11
            // -------------------
            //  8		2	      10



            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[arrLineGauges.Count];

            if (!DefineCornerPoints(arrLineGauges, fUnitWidth, fUnitHeight, fMinBorderScore, ref fTotalAngle, ref nCount, ref blnResult, ref arrCorner))
            {
                return false;
            }

            // Get rectangle edge lines
            PointF[] arrEdgeCenterPoint = new PointF[4];
            Line[] arrEdgeCenterLines = new Line[4];
            Line[] arrEdgeLines = new Line[4];
            fTotalAngle = 0;
            int intPassCount = 0;

            if ((arrCorner[0].X != 0) && (arrCorner[1].X != 0))
            {
                arrEdgeLines[0] = new Line();
                arrEdgeLines[0].CalculateStraightLine(arrCorner[0], arrCorner[1]);
                if (arrEdgeLines[0].ref_dAngle > 0)
                    fTotalAngle += (float)arrEdgeLines[0].ref_dAngle - 90;
                else
                    fTotalAngle += (float)arrEdgeLines[0].ref_dAngle + 90;
                intPassCount++;
            }
            if ((arrCorner[1].X != 0) && (arrCorner[2].X != 0))
            {
                arrEdgeLines[1] = new Line();
                arrEdgeLines[1].CalculateStraightLine(arrCorner[1], arrCorner[2]);
                fTotalAngle += (float)arrEdgeLines[1].ref_dAngle;
                intPassCount++;
            }
            if ((arrCorner[2].X != 0) && (arrCorner[3].X != 0))
            {
                arrEdgeLines[2] = new Line();
                arrEdgeLines[2].CalculateStraightLine(arrCorner[2], arrCorner[3]);
                if (arrEdgeLines[2].ref_dAngle > 0)
                    fTotalAngle += (float)arrEdgeLines[2].ref_dAngle - 90;
                else
                    fTotalAngle += (float)arrEdgeLines[2].ref_dAngle + 90;
                intPassCount++;
            }
            if ((arrCorner[3].X != 0) && (arrCorner[0].X != 0))
            {
                arrEdgeLines[3] = new Line();
                arrEdgeLines[3].CalculateStraightLine(arrCorner[3], arrCorner[0]);
                fTotalAngle += (float)arrEdgeLines[3].ref_dAngle;
                intPassCount++;
            }

            // Calculate rectangle angle
            fMeasureAngle = fTotalAngle / intPassCount;

            // Get rectangle corner close lines
            Line[] arrCrossLines = new Line[2];
            arrCrossLines[0] = new Line();
            arrCrossLines[1] = new Line();

            // Calculate center point
            // -------- ver line ---------------------------------------
            if (arrEdgeLines[0] != null && arrEdgeLines[2] != null)
            {
                arrCrossLines[0].CalculateStraightLine(new PointF((arrCorner[0].X + arrCorner[1].X) / 2, (arrCorner[0].Y + arrCorner[1].Y) / 2),
                                                       new PointF((arrCorner[2].X + arrCorner[3].X) / 2, (arrCorner[2].Y + arrCorner[3].Y) / 2));

                fMeasureWidth = (Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[1]) + Math2.GetDistanceBtw2Points(arrCorner[2], arrCorner[3])) / 2;
            }
            else if (arrEdgeLines[0] != null)
            {
                arrCrossLines[0].CalculateStraightLine(new PointF((arrCorner[0].X + arrCorner[1].X) / 2, (arrCorner[0].Y + arrCorner[1].Y) / 2),
                                                        arrEdgeLines[0].ref_dAngle + 90);

                fMeasureWidth = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[1]);
            }
            else if (arrEdgeLines[2] != null)
            {
                arrCrossLines[0].CalculateStraightLine(new PointF((arrCorner[2].X + arrCorner[3].X) / 2, (arrCorner[2].Y + arrCorner[3].Y) / 2),
                                                        arrEdgeLines[2].ref_dAngle + 90);

                fMeasureWidth = Math2.GetDistanceBtw2Points(arrCorner[2], arrCorner[3]);
            }

            // ---------- Hor line ------------
            if (arrEdgeLines[1] != null && arrEdgeLines[3] != null)
            {
                arrCrossLines[1].CalculateStraightLine(new PointF((arrCorner[0].X + arrCorner[3].X) / 2, (arrCorner[0].Y + arrCorner[3].Y) / 2),
                                                       new PointF((arrCorner[1].X + arrCorner[2].X) / 2, (arrCorner[1].Y + arrCorner[2].Y) / 2));

                fMeasureHeight = (Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[3]) + Math2.GetDistanceBtw2Points(arrCorner[1], arrCorner[2])) / 2;
            }
            else if (arrEdgeLines[1] != null)
            {
                arrCrossLines[1].CalculateStraightLine(new PointF((arrCorner[1].X + arrCorner[2].X) / 2, (arrCorner[1].Y + arrCorner[2].Y) / 2),
                                                        arrEdgeLines[1].ref_dAngle + 90);

                fMeasureHeight = Math2.GetDistanceBtw2Points(arrCorner[1], arrCorner[2]);
            }
            else if (arrEdgeLines[3] != null)
            {
                arrCrossLines[1].CalculateStraightLine(new PointF((arrCorner[0].X + arrCorner[3].X) / 2, (arrCorner[0].Y + arrCorner[3].Y) / 2),
                                                        arrEdgeLines[3].ref_dAngle + 90);

                fMeasureHeight = Math2.GetDistanceBtw2Points(arrCorner[0], arrCorner[3]);
            }

            pMeasureCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);

            return true;
        }


        private bool PlaceCornerLineGaugeLocation(ref List<LGauge> arrLineGauges, ref PointF[] arrMeasureCornerPoint, float fUnitWidth, float fUnitHeight)
        {
            //  4		0	      6
            // -------------------
            //5|corner0	    corner1|7
            // |			       |
            // |			       |
            // |			       |
            //3|			       |1
            // |			       |
            // |			       |
            // |			       |
            // |			       |
            //9|corner3	    corner2|11
            // -------------------
            //  8		2	      10


            if (arrLineGauges.Count < 12)
            {
                SRMMessageBox.Show("PlaceCornerLineGaugeLocation() - line gauge count is not 12.");
                return false;
            }

            float fToleranceEdge = 2;
            float fGuageTolerance = Math.Min(fUnitWidth / 3, fUnitHeight / 3);
            fGuageTolerance = Math.Min(fGuageTolerance, arrLineGauges[0].ref_GaugeTolerance);
            float fGuageLength = fUnitWidth / 3 + fGuageTolerance;

            float fStartX = arrLineGauges[3].ref_GaugeCenterX - arrLineGauges[3].ref_GaugeTolerance;
            fGuageLength = arrMeasureCornerPoint[0].X - fStartX + fUnitWidth / 3;
            arrLineGauges[4].SetGaugePlacement(fStartX + fGuageLength / 2 + fToleranceEdge,
                                                               arrLineGauges[0].ref_GaugeCenterY,
                                                               fGuageTolerance,
                                                               fGuageLength,
                                                               arrLineGauges[0].ref_ObjectAngle);
            arrLineGauges[4].ref_GaugeThickness = (int)fGuageLength / 2;

            float fEndX = arrLineGauges[1].ref_GaugeCenterX + arrLineGauges[1].ref_GaugeTolerance;
            fGuageLength = fEndX - arrMeasureCornerPoint[1].X + fUnitWidth / 3;
            arrLineGauges[6].SetGaugePlacement(fEndX - fGuageLength / 2 - fToleranceEdge,
                                                                           arrLineGauges[0].ref_GaugeCenterY, //arrMeasureCornerPoint[1].Y,
                                                                           fGuageTolerance,
                                                                           fGuageLength,
                                                                           arrLineGauges[0].ref_ObjectAngle);
            arrLineGauges[6].ref_GaugeThickness = (int)fGuageLength / 2;

            fStartX = arrLineGauges[3].ref_GaugeCenterX - arrLineGauges[3].ref_GaugeTolerance;
            fGuageLength = arrMeasureCornerPoint[3].X - fStartX + fUnitWidth / 3;
            arrLineGauges[8].SetGaugePlacement(fStartX + fGuageLength / 2 + fToleranceEdge,
                                                                                       arrLineGauges[2].ref_GaugeCenterY, //arrMeasureCornerPoint[3].Y,
                                                                                       fGuageTolerance,
                                                                                       fGuageLength,
                                                                                       arrLineGauges[2].ref_ObjectAngle + 180);
            arrLineGauges[8].ref_GaugeThickness = (int)fGuageLength / 2;

            fEndX = arrLineGauges[1].ref_GaugeCenterX + arrLineGauges[1].ref_GaugeTolerance;
            fGuageLength = fEndX - arrMeasureCornerPoint[1].X + fUnitWidth / 3;
            arrLineGauges[10].SetGaugePlacement(fEndX - fGuageLength / 2 - fToleranceEdge,
                                                                                       arrLineGauges[2].ref_GaugeCenterY, //arrMeasureCornerPoint[2].Y,
                                                                                       fGuageTolerance,
                                                                                       fGuageLength,
                                                                                       arrLineGauges[2].ref_ObjectAngle + 180);
            arrLineGauges[10].ref_GaugeThickness = (int)fGuageLength / 2;

            float fStartY = arrLineGauges[0].ref_GaugeCenterY - arrLineGauges[0].ref_GaugeTolerance;
            fGuageLength = arrMeasureCornerPoint[0].Y - fStartY + fUnitHeight / 3;
            arrLineGauges[5].SetGaugePlacement(arrLineGauges[3].ref_GaugeCenterX, //arrMeasureCornerPoint[0].X,
                                                               fStartY + fGuageLength / 2 + fToleranceEdge,
                                                               fGuageTolerance,
                                                               fGuageLength,
                                                               arrLineGauges[0].ref_ObjectAngle + 270);
            arrLineGauges[5].ref_GaugeThickness = (int)fGuageLength / 2;

            float fEndY = arrLineGauges[2].ref_GaugeCenterY + arrLineGauges[2].ref_GaugeTolerance;
            fGuageLength = fEndY - arrMeasureCornerPoint[3].Y + fUnitHeight / 3;
            arrLineGauges[9].SetGaugePlacement(arrLineGauges[3].ref_GaugeCenterX, //arrMeasureCornerPoint[3].X,
                                                   fEndY - fGuageLength / 2 - fToleranceEdge,
                                                   fGuageTolerance,
                                                   fGuageLength,
                                                   arrLineGauges[3].ref_ObjectAngle + 270);
            arrLineGauges[9].ref_GaugeThickness = (int)fGuageLength / 2;

            fStartY = arrLineGauges[0].ref_GaugeCenterY - arrLineGauges[0].ref_GaugeTolerance;
            fGuageLength = arrMeasureCornerPoint[1].Y - fStartY + fUnitHeight / 3;
            arrLineGauges[7].SetGaugePlacement(arrLineGauges[1].ref_GaugeCenterX,
                                                   fStartY + fGuageLength / 2 + fToleranceEdge,
                                                   fGuageTolerance,
                                                   fGuageLength,
                                                   arrLineGauges[1].ref_ObjectAngle + 90);
            arrLineGauges[7].ref_GaugeThickness = (int)fGuageLength / 2;

            fEndY = arrLineGauges[2].ref_GaugeCenterY + arrLineGauges[2].ref_GaugeTolerance;
            fGuageLength = fEndY - arrMeasureCornerPoint[2].Y + fUnitHeight / 3;
            arrLineGauges[11].SetGaugePlacement(arrLineGauges[1].ref_GaugeCenterX,
                                                   fEndY - fGuageLength / 2 - fToleranceEdge,
                                                   fGuageTolerance,
                                                   fGuageLength,
                                                   arrLineGauges[2].ref_ObjectAngle + 90);
            arrLineGauges[11].ref_GaugeThickness = (int)fGuageLength / 2;

            return true;
        }

        public void SetCurrentMeasureSizeAsUnitSize()
        {
            m_fUnitSizeWidth = m_fRectWidth;
            m_fUnitSizeHeight = m_fRectHeight;

        }

        public void SetGaugePlace(float fCenterX, float fCenterY)
        {
            float fStartX = m_arrLineGauge[3].ref_GaugeCenterX - m_arrLineGauge[3].ref_GaugeTolerance;
            float fEndX = m_arrLineGauge[1].ref_GaugeCenterX + m_arrLineGauge[1].ref_GaugeTolerance;
            float fCurrentRectCenterX = (fStartX + fEndX) / 2;
            float fStartY = m_arrLineGauge[0].ref_GaugeCenterY - m_arrLineGauge[0].ref_GaugeTolerance;
            float fEndY = m_arrLineGauge[2].ref_GaugeCenterY + m_arrLineGauge[2].ref_GaugeTolerance;
            float fCurrentRectCenterY = (fStartY + fEndY) / 2;
            float fXOffset = fCenterX - fCurrentRectCenterX;
            float fYOffset = fCenterY - fCurrentRectCenterY;

            // Top
            m_arrLineGauge[0].SetGaugeCenter(m_arrLineGauge[0].ref_GaugeCenterX + fXOffset, m_arrLineGauge[0].ref_GaugeCenterY + fYOffset);
            m_arrLineGauge[0].SetGaugeLength(m_fUnitSizeWidth);

            // Right
            m_arrLineGauge[1].SetGaugeCenter(m_arrLineGauge[1].ref_GaugeCenterX + fXOffset, m_arrLineGauge[1].ref_GaugeCenterY + fYOffset);
            m_arrLineGauge[1].SetGaugeLength(m_fUnitSizeHeight);

            // Bottom
            m_arrLineGauge[2].SetGaugeCenter(m_arrLineGauge[2].ref_GaugeCenterX + fXOffset, m_arrLineGauge[2].ref_GaugeCenterY + fYOffset);
            m_arrLineGauge[2].SetGaugeLength(m_fUnitSizeWidth);

            // Left
            m_arrLineGauge[3].SetGaugeCenter(m_arrLineGauge[3].ref_GaugeCenterX + fXOffset, m_arrLineGauge[3].ref_GaugeCenterY + fYOffset);
            m_arrLineGauge[3].SetGaugeLength(m_fUnitSizeHeight);

            //// Top
            //m_arrLineGauge[0].SetGaugeCenter(fCenterX, fCenterY - m_fUnitSizeHeight / 2);
            //m_arrLineGauge[0].SetGaugeLength(m_fUnitSizeWidth);

            //// Right
            //m_arrLineGauge[1].SetGaugeCenter(fCenterX + m_fUnitSizeWidth / 2, fCenterY);
            //m_arrLineGauge[1].SetGaugeLength(m_fUnitSizeHeight);

            //// Bottom
            //m_arrLineGauge[2].SetGaugeCenter(fCenterX, fCenterY + m_fUnitSizeHeight / 2);
            //m_arrLineGauge[2].SetGaugeLength(m_fUnitSizeWidth);

            //// Left
            //m_arrLineGauge[3].SetGaugeCenter(fCenterX - m_fUnitSizeWidth / 2, fCenterY);
            //m_arrLineGauge[3].SetGaugeLength(m_fUnitSizeHeight);
        }

        public void SetGaugePlace_WithOffset(float fCenterX, float fCenterY, float fWidth, float fHeight)
        {
            float fStartX = m_arrLineGauge[3].ref_GaugeCenterX - m_arrLineGauge[3].ref_GaugeTolerance;
            float fEndX = m_arrLineGauge[1].ref_GaugeCenterX + m_arrLineGauge[1].ref_GaugeTolerance;
            float fCurrentRectCenterX = (fStartX + fEndX) / 2;
            float fStartY = m_arrLineGauge[0].ref_GaugeCenterY - m_arrLineGauge[0].ref_GaugeTolerance;
            float fEndY = m_arrLineGauge[2].ref_GaugeCenterY + m_arrLineGauge[2].ref_GaugeTolerance;
            float fCurrentRectCenterY = (fStartY + fEndY) / 2;
            float fXOffset = fCenterX - fCurrentRectCenterX;
            float fYOffset = fCenterY - fCurrentRectCenterY;

            // Top
            m_arrLineGauge[0].SetGaugeCenter(fCenterX + m_fOffsetX[0], (fCenterY - fHeight / 2) + m_fOffsetY[0]);
            //m_arrLineGauge[0].SetGaugeLength(m_fUnitSizeWidth);

            // Right
            m_arrLineGauge[1].SetGaugeCenter((fCenterX + fWidth / 2) + m_fOffsetX[1], fCenterY + m_fOffsetY[1]);
            //m_arrLineGauge[1].SetGaugeLength(m_fUnitSizeHeight);

            // Bottom
            m_arrLineGauge[2].SetGaugeCenter(fCenterX + m_fOffsetX[2], (fCenterY + fHeight / 2) + m_fOffsetY[2]);
            //m_arrLineGauge[2].SetGaugeLength(m_fUnitSizeWidth);

            // Left
            m_arrLineGauge[3].SetGaugeCenter((fCenterX - fWidth / 2) + m_fOffsetX[3], fCenterY + m_fOffsetY[3]);
            //m_arrLineGauge[3].SetGaugeLength(m_fUnitSizeHeight);

            //// Top
            //m_arrLineGauge[0].SetGaugeCenter(fCenterX, fCenterY - m_fUnitSizeHeight / 2);
            //m_arrLineGauge[0].SetGaugeLength(m_fUnitSizeWidth);

            //// Right
            //m_arrLineGauge[1].SetGaugeCenter(fCenterX + m_fUnitSizeWidth / 2, fCenterY);
            //m_arrLineGauge[1].SetGaugeLength(m_fUnitSizeHeight);

            //// Bottom
            //m_arrLineGauge[2].SetGaugeCenter(fCenterX, fCenterY + m_fUnitSizeHeight / 2);
            //m_arrLineGauge[2].SetGaugeLength(m_fUnitSizeWidth);

            //// Left
            //m_arrLineGauge[3].SetGaugeCenter(fCenterX - m_fUnitSizeWidth / 2, fCenterY);
            //m_arrLineGauge[3].SetGaugeLength(m_fUnitSizeHeight);
        }

        public void SetGaugeAngle(float fAngle)
        {

        }

        //Set the placement for blue square
        public void SetGaugePlace(ROI objROI)
        {
            // Top
            m_arrLineGauge[0].SetGaugeLength(objROI.ref_ROI.Width);
            m_arrLineGauge[0].SetGaugeCenter(objROI.ref_ROITotalCenterX, objROI.ref_ROITotalY + m_arrLineGauge[0].ref_GaugeTolerance);

            // Right
            m_arrLineGauge[1].SetGaugeLength(objROI.ref_ROIHeight);
            m_arrLineGauge[1].SetGaugeCenter(objROI.ref_ROITotalX + objROI.ref_ROIWidth - m_arrLineGauge[1].ref_GaugeTolerance, objROI.ref_ROITotalCenterY);

            // Bottom
            m_arrLineGauge[2].SetGaugeLength(objROI.ref_ROI.Width);
            m_arrLineGauge[2].SetGaugeCenter(objROI.ref_ROITotalCenterX, objROI.ref_ROITotalY + objROI.ref_ROIHeight - m_arrLineGauge[2].ref_GaugeTolerance);

            // Left
            m_arrLineGauge[3].SetGaugeLength(objROI.ref_ROIHeight);
            m_arrLineGauge[3].SetGaugeCenter(objROI.ref_ROITotalX + m_arrLineGauge[3].ref_GaugeTolerance, objROI.ref_ROITotalCenterY);
        }

        public void SetGaugePlace_BasedOnEdgeROI()
        {
            // Top
            m_arrLineGauge[0].SetGaugeLength(m_arrEdgeROI[0].ref_ROIWidth);
            m_arrLineGauge[0].SetGaugeCenter(m_arrEdgeROI[0].ref_ROICenterX, m_arrEdgeROI[0].ref_ROICenterY);
            m_arrLineGauge[0].SetGaugeTolerance(m_arrEdgeROI[0].ref_ROIHeight / 2);

            // Right
            m_arrLineGauge[1].SetGaugeLength(m_arrEdgeROI[1].ref_ROIHeight);
            m_arrLineGauge[1].SetGaugeCenter(m_arrEdgeROI[1].ref_ROICenterX, m_arrEdgeROI[1].ref_ROICenterY);
            m_arrLineGauge[1].SetGaugeTolerance(m_arrEdgeROI[1].ref_ROIWidth / 2);

            // Bottom
            m_arrLineGauge[2].SetGaugeLength(m_arrEdgeROI[2].ref_ROIWidth);
            m_arrLineGauge[2].SetGaugeCenter(m_arrEdgeROI[2].ref_ROICenterX, m_arrEdgeROI[2].ref_ROICenterY);
            m_arrLineGauge[2].SetGaugeTolerance(m_arrEdgeROI[2].ref_ROIHeight / 2);

            // left
            m_arrLineGauge[3].SetGaugeLength(m_arrEdgeROI[3].ref_ROIHeight);
            m_arrLineGauge[3].SetGaugeCenter(m_arrEdgeROI[3].ref_ROICenterX, m_arrEdgeROI[3].ref_ROICenterY);
            m_arrLineGauge[3].SetGaugeTolerance(m_arrEdgeROI[3].ref_ROIWidth / 2);
        }

        public void SetGaugePlace(ROI objROI, int intEdgeLimit)
        {
            float fCenterX = objROI.ref_ROI.TotalOrgX + objROI.ref_ROI.Width / 2;
            float fCenterY = objROI.ref_ROI.TotalOrgY + objROI.ref_ROI.Height / 2;

            m_arrLineGauge[0].SetGaugeCenter(fCenterX, Math.Max(fCenterY - m_fUnitSizeHeight / 2, objROI.ref_ROI.TotalOrgY + intEdgeLimit + m_arrLineGauge[0].ref_GaugeTolerance));
            m_arrLineGauge[0].SetGaugeLength(m_fUnitSizeWidth);

            // Right
            m_arrLineGauge[1].SetGaugeCenter(Math.Min(fCenterX + m_fUnitSizeWidth / 2, objROI.ref_ROI.TotalOrgX + objROI.ref_ROI.Width - intEdgeLimit - m_arrLineGauge[1].ref_GaugeTolerance), fCenterY);
            m_arrLineGauge[1].SetGaugeLength(m_fUnitSizeHeight);

            // Bottom
            m_arrLineGauge[2].SetGaugeCenter(fCenterX, Math.Min(fCenterY + m_fUnitSizeHeight / 2, objROI.ref_ROI.TotalOrgY + objROI.ref_ROI.Height - intEdgeLimit - m_arrLineGauge[2].ref_GaugeTolerance));
            m_arrLineGauge[2].SetGaugeLength(m_fUnitSizeWidth);

            // Left
            m_arrLineGauge[3].SetGaugeCenter(Math.Max(fCenterX - m_fUnitSizeWidth / 2, objROI.ref_ROI.TotalOrgX + intEdgeLimit + m_arrLineGauge[3].ref_GaugeTolerance), fCenterY);
            m_arrLineGauge[3].SetGaugeLength(m_fUnitSizeHeight);
        }

        public void SetGaugePlace(ROI objROI, float fPlaceCenterX, float fPlaceCenterY, int intEdgeLimit)
        {
            float fCenterX = objROI.ref_ROI.TotalOrgX + fPlaceCenterX;
            float fCenterY = objROI.ref_ROI.TotalOrgY + fPlaceCenterY;

            m_arrLineGauge[0].SetGaugeCenter(fCenterX, Math.Max(fCenterY - m_fUnitSizeHeight / 2, objROI.ref_ROI.TotalOrgY + intEdgeLimit + m_arrLineGauge[0].ref_GaugeTolerance));
            m_arrLineGauge[0].SetGaugeLength(m_fUnitSizeWidth);

            // Right
            m_arrLineGauge[1].SetGaugeCenter(Math.Min(fCenterX + m_fUnitSizeWidth / 2, objROI.ref_ROI.TotalOrgX + objROI.ref_ROI.Width - intEdgeLimit - m_arrLineGauge[1].ref_GaugeTolerance), fCenterY);
            m_arrLineGauge[1].SetGaugeLength(m_fUnitSizeHeight);

            // Bottom
            m_arrLineGauge[2].SetGaugeCenter(fCenterX, Math.Min(fCenterY + m_fUnitSizeHeight / 2, objROI.ref_ROI.TotalOrgY + objROI.ref_ROI.Height - intEdgeLimit - m_arrLineGauge[2].ref_GaugeTolerance));
            m_arrLineGauge[2].SetGaugeLength(m_fUnitSizeWidth);

            // Left
            m_arrLineGauge[3].SetGaugeCenter(Math.Max(fCenterX - m_fUnitSizeWidth / 2, objROI.ref_ROI.TotalOrgX + intEdgeLimit + m_arrLineGauge[3].ref_GaugeTolerance), fCenterY);
            m_arrLineGauge[3].SetGaugeLength(m_fUnitSizeHeight);
        }


        public void SaveRectGauge4L(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection, bool blnSaveMeasurementAsTemplate)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName + "_RectSetting", blnNewSection);

            if (blnSaveMeasurementAsTemplate)
            {
                m_fUnitSizeWidth = m_fRectWidth;
                m_fUnitSizeHeight = m_fRectHeight;
            }

            objFile.WriteElement1Value("UnitSizeWidth", m_fUnitSizeWidth, "Unit Size Width", true);
            objFile.WriteElement1Value("UnitSizeHeight", m_fUnitSizeHeight, "Unit Size Height", true);

            m_f4LGaugeCenterPointX = m_pRectCenterPoint.X; //(m_arrLineGauge[1].ref_GaugeCenterX + m_arrLineGauge[3].ref_GaugeCenterX) / 2; //28-05-2019 ZJYEOH: Should save the corner point formula's center X and Y  
            m_f4LGaugeCenterPointY = m_pRectCenterPoint.Y; //(m_arrLineGauge[0].ref_GaugeCenterY + m_arrLineGauge[2].ref_GaugeCenterY) / 2;
            objFile.WriteElement1Value("Gauge4LCenterPointX", m_f4LGaugeCenterPointX, "4L Line Gauge Combine Center Point X", true);
            objFile.WriteElement1Value("Gauge4LCenterPointY", m_f4LGaugeCenterPointY, "4L Line Gauge Combine Center Point Y", true);

            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                objFile.WriteSectionElement(strSectionName + "_" + i, blnNewSection);

                // Rectangle gauge template measurement result
                if (blnSaveMeasurementAsTemplate)
                {
                    //objFile.WriteElement1Value("ObjectCenterX", m_RectGauge.MeasuredRectangle.CenterX, "Gauge Measurement Result CenterX", true);
                    //objFile.WriteElement1Value("ObjectCenterY", m_RectGauge.MeasuredRectangle.CenterY, "Gauge Measurement Result CenterY", true);
                    //objFile.WriteElement1Value("ObjectWidth", m_RectGauge.MeasuredRectangle.SizeX, "Gauge Measurement Result Width", true);
                    //objFile.WriteElement1Value("ObjectHeight", m_RectGauge.MeasuredRectangle.SizeY, "Gauge Measurement Result Height", true);
                    //objFile.WriteElement1Value("ObjectAngle", m_RectGauge.MeasuredRectangle.Angle, "Gauge Measurement Result Angle", true);

                    //m_fTemplateObjectCenterX = m_RectGauge.MeasuredRectangle.CenterX;
                    //m_fTemplateObjectCenterY = m_RectGauge.MeasuredRectangle.CenterY;
                    //m_fTemplateObjectWidth = m_RectGauge.MeasuredRectangle.SizeX;
                    //m_fTemplateObjectHeight = m_RectGauge.MeasuredRectangle.SizeY;
                }

                // Rectangle gauge position setting
                objFile.WriteElement1Value("CenterX", m_arrLineGauge[i].ref_GaugeCenterX, "Gauge Setting Center X", true);
                objFile.WriteElement1Value("CenterY", m_arrLineGauge[i].ref_GaugeCenterY, "Gauge Setting Center X", true);
                objFile.WriteElement1Value("Length", m_arrLineGauge[i].ref_GaugeLength, "Gauge Setting Length", true);
                objFile.WriteElement1Value("Angle", m_arrLineGauge[i].ref_GaugeAngle, "Gauge Setting Angle", true);
                objFile.WriteElement1Value("Tolerance", m_arrLineGauge[i].ref_GaugeTolerance, "Gauge Setting Tolerance", true);

                if (i < 4)
                    m_fTemplateGaugeTolerance[i] = m_arrLineGauge[i].ref_GaugeTolerance;

                // Rectangle gauge measurement setting
                objFile.WriteElement1Value("TransType", m_arrLineGauge[i].ref_GaugeTransType, "Gauge Setting Transition Type", true);
                objFile.WriteElement1Value("TransChoice", m_arrLineGauge[i].ref_GaugeTransChoice, "Gauge Setting Transition Choice", true);
                objFile.WriteElement1Value("Threshold", m_arrLineGauge[i].ref_GaugeThreshold, "Gauge Setting Threshold", true);
                objFile.WriteElement1Value("Thickness", m_arrLineGauge[i].ref_GaugeThickness, "Gauge Setting Thickness", true);
                objFile.WriteElement1Value("MinAmp", m_arrLineGauge[i].ref_GaugeMinAmplitude, "Gauge Setting Minimum Amplitude", true);
                objFile.WriteElement1Value("MinArea", m_arrLineGauge[i].ref_GaugeMinArea, "Gauge Setting Minimum Area", true);
                objFile.WriteElement1Value("Filter", m_arrLineGauge[i].ref_GaugeFilter, "Gauge Setting Filter/Smoothing", true);

                // Rectangle gauge fitting setting
                objFile.WriteElement1Value("SamplingStep", m_arrLineGauge[i].ref_GaugeSamplingStep, "Gauge Setting Sampling Step", true);
                objFile.WriteElement1Value("FilteringThreshold", m_arrLineGauge[i].ref_GaugeFilterThreshold, "Gauge Setting Filtering Threshold", true);
                objFile.WriteElement1Value("FilteringPasses", m_arrLineGauge[i].ref_GaugeFilterPasses, "Gauge Setting Filtering Passes", true);

                objFile.WriteElement1Value("GaugeEnableSubGauge", m_arrGaugeEnableSubGauge[i], "Gauge Enable Sub Gauge", true);
                objFile.WriteElement1Value("GaugeMeasureMode", m_arrGaugeMeasureMode[i], "Gauge Point Mode", true);
                objFile.WriteElement1Value("GaugeOffset", m_arrGaugeOffset[i], "Gauge Offset", true); 
                objFile.WriteElement1Value("GaugeMinScore", m_arrGaugeMinScore[i], "Gauge Min Score", true);
                objFile.WriteElement1Value("GaugeMaxAngle", m_arrGaugeMaxAngle[i], "Gauge Max Angle", true);
                objFile.WriteElement1Value("GaugeImageMode", m_arrGaugeImageMode[i], "Gauge Image Mode", true);
                objFile.WriteElement1Value("GaugeImageNo", m_arrGaugeImageNo[i], "Gauge Image No", true);
                objFile.WriteElement1Value("GaugeImageGain", m_arrGaugeImageGain[i], "Gauge Image Gain", true);
                objFile.WriteElement1Value("GaugeGroupTolerance", m_arrGaugeGroupTolerance[i], "Gauge Group Tolerance", true);
                objFile.WriteElement1Value("GaugeImageOpenCloseIteration", m_arrGaugeImageOpenCloseIteration[i], "Gauge Image Open Close Iteration", true); 
                objFile.WriteElement1Value("GaugeImageThreshold", m_arrGaugeImageThreshold[i], "Gauge Image Threshold", true);
                objFile.WriteElement1Value("GaugeWantImageThreshold", m_arrGaugeWantImageThreshold[i], "Gauge Want Image Threshold", true);

                objFile.WriteElement1Value("GaugeTiltAngle", m_intGaugeTiltAngle, "Gauge Tilt Angle", true);
                objFile.WriteElement1Value("IntoOut", m_blnInToOut[i], "Direction In To Out", true);
            }

            objFile.WriteSectionElement("PadDirection" + m_intDirection, blnNewSection);
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                objFile.WriteElement1Value("EdgeROI_" + i, "");

                //m_fOffsetX[i] = m_intEdgeROICenterX[i] - m_arrLineGauge[i].ref_ObjectCenterX; //ref_GaugeCenterX
                //m_fOffsetY[i] = m_intEdgeROICenterY[i] - m_arrLineGauge[i].ref_ObjectCenterY; //ref_GaugeCenterY

                m_fOffsetX[i] = m_arrLineGauge[i].ref_GaugeCenterX - m_arrLineGauge[i].ref_ObjectCenterX;
                m_fOffsetY[i] = m_arrLineGauge[i].ref_GaugeCenterY - m_arrLineGauge[i].ref_ObjectCenterY;

                objFile.WriteElement2Value("PositionX", m_arrEdgeROI[i].ref_ROIPositionX);
                objFile.WriteElement2Value("PositionY", m_arrEdgeROI[i].ref_ROIPositionY);
                objFile.WriteElement2Value("Width", m_arrEdgeROI[i].ref_ROIWidth);
                objFile.WriteElement2Value("Height", m_arrEdgeROI[i].ref_ROIHeight);
                objFile.WriteElement2Value("CenterX", m_intEdgeROICenterX[i]);
                objFile.WriteElement2Value("CenterY", m_intEdgeROICenterY[i]);
                objFile.WriteElement2Value("OffsetX", m_fOffsetX[i]);
                objFile.WriteElement2Value("OffsetY", m_fOffsetY[i]);

                objFile.WriteElement2Value("DontCareROICount", m_arrDontCareROI[i].Count);
                for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
                {
                    objFile.WriteElement1Value("DontCareROI_" + i + "_" + j, "");
                    objFile.WriteElement2Value("PositionX", m_arrDontCareROI[i][j].ref_ROIPositionX);
                    objFile.WriteElement2Value("PositionY", m_arrDontCareROI[i][j].ref_ROIPositionY);
                    objFile.WriteElement2Value("Width", m_arrDontCareROI[i][j].ref_ROIWidth);
                    objFile.WriteElement2Value("Height", m_arrDontCareROI[i][j].ref_ROIHeight);
                }
            }
            
            for (int i = 0; i < m_arrAutoDontCare.Count; i++)
            {
                objFile.WriteElement1Value("AutoDontCare_" + i, m_arrAutoDontCare[i], "Enable Auto Dont Care", true);
            }

            for (int i = 0; i < m_arrAutoDontCareOffset.Count; i++)
            {
                objFile.WriteElement1Value("AutoDontCareOffset_" + i, m_arrAutoDontCareOffset[i], "Auto Dont Care Offset", true);
            }

            for (int i = 0; i < m_arrAutoDontCareThreshold.Count; i++)
            {
                objFile.WriteElement1Value("AutoDontCareThreshold_" + i, m_arrAutoDontCareThreshold[i], "Auto Dont Care Threshold", true);
            }
            
            objFile.WriteEndElement();

            SetUserSettingToUserVariables();
        }

        public void SaveRectGauge4L_SECSGEM(string strPath, string strSectionName, string strVisionName)
        {
            //XmlParser objFile = new XmlParser(strPath, blnNewFile);

            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            //objFile.WriteSectionElement(strSectionName + "_RectSetting", blnNewSection);
            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_UnitSizeWidth", m_fUnitSizeWidth);
            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_UnitSizeHeight", m_fUnitSizeHeight);

            m_f4LGaugeCenterPointX = m_pRectCenterPoint.X; //(m_arrLineGauge[1].ref_GaugeCenterX + m_arrLineGauge[3].ref_GaugeCenterX) / 2; //28-05-2019 ZJYEOH: Should save the corner point formula's center X and Y  
            m_f4LGaugeCenterPointY = m_pRectCenterPoint.Y; //(m_arrLineGauge[0].ref_GaugeCenterY + m_arrLineGauge[2].ref_GaugeCenterY) / 2;
            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_Gauge4LCenterPointX", m_f4LGaugeCenterPointX);
            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_Gauge4LCenterPointY", m_f4LGaugeCenterPointY);

            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                // Rectangle gauge position setting
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_CenterX", m_arrLineGauge[i].ref_GaugeCenterX);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_CenterY", m_arrLineGauge[i].ref_GaugeCenterY);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_Length", m_arrLineGauge[i].ref_GaugeLength);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_Angle", m_arrLineGauge[i].ref_GaugeAngle);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_Tolerance", m_arrLineGauge[i].ref_GaugeTolerance);

                // Rectangle gauge measurement setting
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_TransType", m_arrLineGauge[i].ref_GaugeTransType);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_TransChoice", m_arrLineGauge[i].ref_GaugeTransChoice);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_Threshold", m_arrLineGauge[i].ref_GaugeThreshold);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_Thickness", m_arrLineGauge[i].ref_GaugeThickness);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_MinAmp", m_arrLineGauge[i].ref_GaugeMinAmplitude);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_MinArea", m_arrLineGauge[i].ref_GaugeMinArea);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_Filter", m_arrLineGauge[i].ref_GaugeFilter);

                // Rectangle gauge fitting setting
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_SamplingStep", m_arrLineGauge[i].ref_GaugeSamplingStep);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_FilteringThreshold", m_arrLineGauge[i].ref_GaugeFilterThreshold);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_FilteringPasses", m_arrLineGauge[i].ref_GaugeFilterPasses);

                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeEnableSubGauge", m_arrGaugeEnableSubGauge[i]);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeMeasureMode", m_arrGaugeMeasureMode[i]);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeOffset", m_arrGaugeOffset[i]); 
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeMinScore", m_arrGaugeMinScore[i]);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeMaxAngle", m_arrGaugeMaxAngle[i]);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeImageMode", m_arrGaugeImageMode[i]);
                objFile.WriteElement1Value(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeImageNo", m_arrGaugeImageNo[i]);
                objFile.WriteElement1Value(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeImageGain", m_arrGaugeImageGain[i]);
                objFile.WriteElement1Value(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeGroupTolerance", m_arrGaugeGroupTolerance[i]);
                objFile.WriteElement1Value(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeImageOpenCloseIteration", m_arrGaugeImageOpenCloseIteration[i]);
                objFile.WriteElement1Value(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeImageThreshold", m_arrGaugeImageThreshold[i]);
                objFile.WriteElement1Value(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeWantImageThreshold", m_arrGaugeWantImageThreshold[i]);

                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeTiltAngle", m_intGaugeTiltAngle);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_IntoOut", m_blnInToOut[i]);
            }

            ////objFile.WriteSectionElement("PadDirection" + m_intDirection, blnNewSection);
            //for (int i = 0; i < m_arrEdgeROI.Count; i++)
            //{
            //    //objFile.WriteElementValue("EdgeROI_" + i, "");

            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_PositionX", m_arrEdgeROI[i].ref_ROIPositionX);
            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_PositionY", m_arrEdgeROI[i].ref_ROIPositionY);
            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_Width", m_arrEdgeROI[i].ref_ROIWidth);
            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_Height", m_arrEdgeROI[i].ref_ROIHeight);
            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_CenterX", m_intEdgeROICenterX[i]);
            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_CenterY", m_intEdgeROICenterY[i]);
            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_DontCareROICount", m_arrDontCareROI[i].Count);
            //    //for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
            //    //{
            //    //    //objFile.WriteElementValue("DontCareROI_" + i + "_" + j, "");
            //    //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_DontCareROI_" + i + "_" + j + "_PositionX", m_arrDontCareROI[i][j].ref_ROIPositionX);
            //    //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_DontCareROI_" + i + "_" + j + "_PositionY", m_arrDontCareROI[i][j].ref_ROIPositionY);
            //    //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_DontCareROI_" + i + "_" + j + "_Width", m_arrDontCareROI[i][j].ref_ROIWidth);
            //    //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_DontCareROI_" + i + "_" + j + "_Height", m_arrDontCareROI[i][j].ref_ROIHeight);
            //    //}
            //}
            objFile.WriteEndElement();
        }

        public void LoadRectGauge4LMeasurementSettingOnly(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName + "_RectSetting");

            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                objFile.GetFirstSection(strSectionName + "_" + i);

                m_arrLineGauge[i].ref_GaugeTolerance = objFile.GetValueAsFloat("Tolerance", 10);

                // Rectangle gauge measurement setting
                m_arrPointGauge[i].ref_GaugeTransType = m_arrLineGauge[i].ref_GaugeTransType = objFile.GetValueAsInt("TransType", 0);
                m_arrPointGauge[i].ref_GaugeTransChoice = m_arrLineGauge[i].ref_GaugeTransChoice = objFile.GetValueAsInt("TransChoice", 0);
                m_arrPointGauge[i].ref_GaugeThreshold = m_arrLineGauge[i].ref_GaugeThreshold = objFile.GetValueAsInt("Threshold", 2);
                m_arrLineGauge[i].ref_GaugeThickness = objFile.GetValueAsInt("Thickness", 13);
                m_arrPointGauge[i].ref_GaugeMinAmplitude = m_arrLineGauge[i].ref_GaugeMinAmplitude = objFile.GetValueAsInt("MinAmp", 10);
                m_arrPointGauge[i].ref_GaugeMinArea = m_arrLineGauge[i].ref_GaugeMinArea = objFile.GetValueAsInt("MinArea", 0);
                m_arrPointGauge[i].ref_GaugeFilter = m_arrLineGauge[i].ref_GaugeFilter = objFile.GetValueAsInt("Filter", 1);

                // Rectangle gauge fitting setting
                m_arrLineGauge[i].ref_GaugeSamplingStep = objFile.GetValueAsInt("SamplingStep", 3);
                m_arrLineGauge[i].ref_GaugeFilterThreshold = objFile.GetValueAsInt("FilteringThreshold", 3);
                m_arrLineGauge[i].ref_GaugeFilterPasses = objFile.GetValueAsInt("FilteringPasses", 3);

                m_arrGaugeEnableSubGauge[i] = objFile.GetValueAsBoolean("GaugeEnableSubGauge", false);
                m_arrGaugeMeasureMode[i] = objFile.GetValueAsInt("GaugeMeasureMode", -1);
                if (m_arrGaugeMeasureMode[i] == -1)   // 2020 03 04 - CCENG: if GaugeMeasureMode no record in database, mean the database is from old version. 
                {
                    if (m_arrGaugeEnableSubGauge[i])    //if sub gauge is true, then set gauge point mode to 1.
                        m_arrGaugeMeasureMode[i] = 1;
                    else
                        m_arrGaugeMeasureMode[i] = 0;
                }
                m_arrGaugeOffset[i] = objFile.GetValueAsInt("GaugeOffset", 10);
                m_arrGaugeMinScore[i] = objFile.GetValueAsInt("GaugeMinScore", 40);
                m_arrGaugeMaxAngle[i] = objFile.GetValueAsInt("GaugeMaxAngle", 3);
                m_arrGaugeImageMode[i] = objFile.GetValueAsInt("GaugeImageMode", 0);
                m_arrGaugeImageNo[i] = objFile.GetValueAsInt("GaugeImageNo", 0);
                m_arrGaugeImageGain[i] = objFile.GetValueAsFloat("GaugeImageGain", 1);
                m_arrGaugeGroupTolerance[i] = objFile.GetValueAsFloat("GaugeGroupTolerance", 1.5f);
                m_arrGaugeImageOpenCloseIteration[i] = objFile.GetValueAsInt("GaugeImageOpenCloseIteration", 0);
                m_arrGaugeImageThreshold[i] = objFile.GetValueAsInt("GaugeImageThreshold", 125);
                m_arrGaugeWantImageThreshold[i] = objFile.GetValueAsBoolean("GaugeWantImageThreshold", false);

                m_intGaugeTiltAngle = objFile.GetValueAsInt("GaugeTiltAngle", 0);
                m_blnInToOut[i] = objFile.GetValueAsBoolean("IntoOut", false);
                // 2019 12 05 - CCENG: Load angle for each line gauge due to angle will change according to m_blnInToOut
                if (m_blnInToOut[i])
                {
                    switch (i)
                    {
                        case 0:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 180));    // Top
                            break;
                        case 1:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", -90));   // Right
                            break;
                        case 2:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 0));  // Bottom
                            break;
                        case 3:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 90));  // Right
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 0));    // Top
                            break;
                        case 1:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 90));   // Right
                            break;
                        case 2:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 180));  // Bottom
                            break;
                        case 3:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", -90));  // Right
                            break;
                    }
                }
                // Rectangle gauge other extra setting
                m_fLength = m_arrLineGauge[i].ref_GaugeLength;

            }

            objFile.GetFirstSection("PadDirection" + m_intDirection);
            for (int i = 0; i < m_arrAutoDontCare.Count; i++)
            {
                m_arrAutoDontCare[i] = objFile.GetValueAsBoolean("AutoDontCare_" + i, false);
            }

            for (int i = 0; i < m_arrAutoDontCareOffset.Count; i++)
            {
                m_arrAutoDontCareOffset[i] = objFile.GetValueAsInt("AutoDontCareOffset_" + i, 0);
            }

            for (int i = 0; i < m_arrAutoDontCareThreshold.Count; i++)
            {
                m_arrAutoDontCareThreshold[i] = objFile.GetValueAsInt("AutoDontCareThreshold_" + i, 125);
            }

        }
        public void LoadRectGauge4L(string strPath, string strSectionName, bool blnLoadMeasurementAsTemplate)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName + "_RectSetting");
            m_fUnitSizeWidth = objFile.GetValueAsFloat("UnitSizeWidth", 0);
            m_fUnitSizeHeight = objFile.GetValueAsFloat("UnitSizeHeight", 0);
            m_f4LGaugeCenterPointX = objFile.GetValueAsFloat("Gauge4LCenterPointX", 0F);
            m_f4LGaugeCenterPointY = objFile.GetValueAsFloat("Gauge4LCenterPointY", 0);

            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                objFile.GetFirstSection(strSectionName + "_" + i);

                if (blnLoadMeasurementAsTemplate)
                {
                    //// Rectangle gauge template measurement result
                    //m_fTemplateObjectCenterX = objFile.GetValueAsFloat("ObjectCenterX", 0);
                    //m_fTemplateObjectCenterY = objFile.GetValueAsFloat("ObjectCenterY", 0);
                    //m_fTemplateObjectWidth = objFile.GetValueAsFloat("ObjectWidth", 0);
                    //m_fTemplateObjectHeight = objFile.GetValueAsFloat("ObjectHeight", 0);
                }

                // Rectangle gauge position setting
                m_arrLineGauge[i].SetGaugeCenter(objFile.GetValueAsFloat("CenterX", 0),
                                      objFile.GetValueAsFloat("CenterY", 0));
                m_arrLineGauge[i].SetGaugeLength(objFile.GetValueAsFloat("Length", 100));
                //  m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 0)); // Not need to load angle. Angle has been define during init this RectGauge4L class.
                m_arrLineGauge[i].ref_GaugeTolerance = objFile.GetValueAsFloat("Tolerance", 10);

                if (i < 4)
                    m_fTemplateGaugeTolerance[i] = m_arrLineGauge[i].ref_GaugeTolerance;

                // Rectangle gauge measurement setting
                m_arrPointGauge[i].ref_GaugeTransType = m_arrLineGauge[i].ref_GaugeTransType = objFile.GetValueAsInt("TransType", 0);
                m_arrPointGauge[i].ref_GaugeTransChoice = m_arrLineGauge[i].ref_GaugeTransChoice = objFile.GetValueAsInt("TransChoice", 0);
                m_arrPointGauge[i].ref_GaugeThreshold = m_arrLineGauge[i].ref_GaugeThreshold = objFile.GetValueAsInt("Threshold", 2);
                m_arrLineGauge[i].ref_GaugeThickness = objFile.GetValueAsInt("Thickness", 13);
                m_arrPointGauge[i].ref_GaugeMinAmplitude = m_arrLineGauge[i].ref_GaugeMinAmplitude = objFile.GetValueAsInt("MinAmp", 10);
                m_arrPointGauge[i].ref_GaugeMinArea = m_arrLineGauge[i].ref_GaugeMinArea = objFile.GetValueAsInt("MinArea", 0);
                m_arrPointGauge[i].ref_GaugeFilter = m_arrLineGauge[i].ref_GaugeFilter = objFile.GetValueAsInt("Filter", 1);

                m_arrPointGauge2[i].ref_GaugeTransType = m_arrLineGauge[i].ref_GaugeTransType = objFile.GetValueAsInt("TransType", 0);
                m_arrPointGauge2[i].ref_GaugeTransChoice = m_arrLineGauge[i].ref_GaugeTransChoice = objFile.GetValueAsInt("TransChoice", 0);
                m_arrPointGauge2[i].ref_GaugeThreshold = m_arrLineGauge[i].ref_GaugeThreshold = objFile.GetValueAsInt("Threshold", 2);
                m_arrPointGauge2[i].ref_GaugeMinAmplitude = m_arrLineGauge[i].ref_GaugeMinAmplitude = objFile.GetValueAsInt("MinAmp", 10);
                m_arrPointGauge2[i].ref_GaugeMinArea = m_arrLineGauge[i].ref_GaugeMinArea = objFile.GetValueAsInt("MinArea", 0);
                m_arrPointGauge2[i].ref_GaugeFilter = m_arrLineGauge[i].ref_GaugeFilter = objFile.GetValueAsInt("Filter", 1);

                // Rectangle gauge fitting setting
                m_arrLineGauge[i].ref_GaugeSamplingStep = objFile.GetValueAsInt("SamplingStep", 3);
                m_arrLineGauge[i].ref_GaugeFilterThreshold = objFile.GetValueAsInt("FilteringThreshold", 3);
                m_arrLineGauge[i].ref_GaugeFilterPasses = objFile.GetValueAsInt("FilteringPasses", 3);

                m_arrGaugeEnableSubGauge[i] = objFile.GetValueAsBoolean("GaugeEnableSubGauge", false);
                m_arrGaugeMeasureMode[i] = objFile.GetValueAsInt("GaugeMeasureMode", -1);
                if (m_arrGaugeMeasureMode[i] == -1)   // 2020 03 04 - CCENG: if GaugeMeasureMode no record in database, mean the database is from old version. 
                {
                    if (m_arrGaugeEnableSubGauge[i])    //if sub gauge is true, then set gauge point mode to 1.
                        m_arrGaugeMeasureMode[i] = 1;
                    else
                        m_arrGaugeMeasureMode[i] = 0;
                }
                m_arrGaugeOffset[i] = objFile.GetValueAsInt("GaugeOffset", 10);
                m_arrGaugeMinScore[i] = objFile.GetValueAsInt("GaugeMinScore", 40);
                m_arrGaugeMaxAngle[i] = objFile.GetValueAsInt("GaugeMaxAngle", 3);
                m_arrGaugeImageMode[i] = objFile.GetValueAsInt("GaugeImageMode", 0);
                m_arrGaugeImageNo[i] = objFile.GetValueAsInt("GaugeImageNo", 0);
                m_arrGaugeImageGain[i] = objFile.GetValueAsFloat("GaugeImageGain", 1);
                m_arrGaugeGroupTolerance[i] = objFile.GetValueAsFloat("GaugeGroupTolerance", 1.5f);
                m_arrGaugeImageOpenCloseIteration[i] = objFile.GetValueAsInt("GaugeImageOpenCloseIteration", 0);
                m_arrGaugeImageThreshold[i] = objFile.GetValueAsInt("GaugeImageThreshold", 125);
                m_arrGaugeWantImageThreshold[i] = objFile.GetValueAsBoolean("GaugeWantImageThreshold", false);

                m_intGaugeTiltAngle = objFile.GetValueAsInt("GaugeTiltAngle", 0);
                m_blnInToOut[i] = objFile.GetValueAsBoolean("IntoOut", false);

                // 2019 12 05 - CCENG: Load angle for each line gauge due to angle will change according to m_blnInToOut
                if (m_blnInToOut[i])
                {
                    switch (i)
                    {
                        case 0:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 180));    // Top
                            break;
                        case 1:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", -90));   // Right
                            break;
                        case 2:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 0));  // Bottom
                            break;
                        case 3:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 90));  // Right
                            break;
                    }
                }
                else
                {
                    switch (i)
                    {
                        case 0:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 0));    // Top
                            break;
                        case 1:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 90));   // Right
                            break;
                        case 2:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 180));  // Bottom
                            break;
                        case 3:
                            m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", -90));  // Right
                            break;
                    }
                }

                // Rectangle gauge other extra setting
                m_fLength = m_arrLineGauge[i].ref_GaugeLength;

            }

            objFile.GetFirstSection("PadDirection" + m_intDirection);
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                objFile.GetSecondSection("EdgeROI_" + i);

                m_arrEdgeROI[i].LoadROISetting(objFile.GetValueAsInt("PositionX", 0, 2),
                                               objFile.GetValueAsInt("PositionY", 0, 2),
                                               objFile.GetValueAsInt("Width", 100, 2),
                                               objFile.GetValueAsInt("Height", 100, 2));

                m_intEdgeROICenterX[i] = objFile.GetValueAsInt("CenterX", 0, 2);
                m_intEdgeROICenterY[i] = objFile.GetValueAsInt("CenterY", 0, 2);

                m_fOffsetX[i] = objFile.GetValueAsFloat("OffsetX", 0, 2);
                m_fOffsetY[i] = objFile.GetValueAsFloat("OffsetY", 0, 2);

                m_arrDontCareROI[i].Clear();
                int intCount = objFile.GetValueAsInt("DontCareROICount", 0, 2);
                for (int j = 0; j < intCount; j++)
                {
                    objFile.GetSecondSection("DontCareROI_" + i + "_" + j);

                    if (m_arrDontCareROI[i].Count <= j)
                    {
                        m_arrDontCareROI[i].Add(new ROI());
                        m_arrDontCareROI[i][j].LoadROISetting(objFile.GetValueAsInt("PositionX", 0, 2),
                                   objFile.GetValueAsInt("PositionY", 0, 2),
                                   objFile.GetValueAsInt("Width", 100, 2),
                                   objFile.GetValueAsInt("Height", 100, 2));

                        m_arrDontCareROI[i][j].AttachImage(m_arrEdgeROI[i]);
                    }
                }
            }

            for (int i = 0; i < m_arrAutoDontCare.Count; i++)
            {
                m_arrAutoDontCare[i] = objFile.GetValueAsBoolean("AutoDontCare_" + i, false);
            }

            for (int i = 0; i < m_arrAutoDontCareOffset.Count; i++)
            {
                m_arrAutoDontCareOffset[i] = objFile.GetValueAsInt("AutoDontCareOffset_" + i, 0);
            }

            for (int i = 0; i < m_arrAutoDontCareThreshold.Count; i++)
            {
                m_arrAutoDontCareThreshold[i] = objFile.GetValueAsInt("AutoDontCareThreshold_" + i, 125);
            }

            SetUserSettingToUserVariables();
        }

        public void LoadRectGauge4L_DontCareROIOnly(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection("PadDirection" + m_intDirection);
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                objFile.GetSecondSection("EdgeROI_" + i);

                m_arrEdgeROI[i].LoadROISetting(objFile.GetValueAsInt("PositionX", 0, 2),
                                               objFile.GetValueAsInt("PositionY", 0, 2),
                                               objFile.GetValueAsInt("Width", 100, 2),
                                               objFile.GetValueAsInt("Height", 100, 2));

                m_intEdgeROICenterX[i] = objFile.GetValueAsInt("CenterX", 0, 2);
                m_intEdgeROICenterY[i] = objFile.GetValueAsInt("CenterY", 0, 2);

                m_arrDontCareROI[i].Clear();
                int intCount = objFile.GetValueAsInt("DontCareROICount", 0, 2);
                for (int j = 0; j < intCount; j++)
                {
                    objFile.GetSecondSection("DontCareROI_" + i + "_" + j);

                    if (m_arrDontCareROI[i].Count <= j)
                    {
                        m_arrDontCareROI[i].Add(new ROI());
                        m_arrDontCareROI[i][j].LoadROISetting(objFile.GetValueAsInt("PositionX", 0, 2),
                                   objFile.GetValueAsInt("PositionY", 0, 2),
                                   objFile.GetValueAsInt("Width", 100, 2),
                                   objFile.GetValueAsInt("Height", 100, 2));

                        m_arrDontCareROI[i][j].AttachImage(m_arrEdgeROI[i]);
                    }
                }
            }
        }

        public void Dispose()
        {
            m_BrushMatched.Dispose();
            m_PenRectGNominal.Dispose();
            m_PenRectGTransitionTypeArrow.Dispose();
            m_PenRectGTransitionTypeArrowNotSelected.Dispose();
            m_PenDontCareArea.Dispose();

            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                if (m_arrLineGauge[i] != null)
                    m_arrLineGauge[i].Dispose();
            }

            for (int i = 0; i < m_arrPointGauge.Count; i++)
            {
                if (m_arrPointGauge[i] != null)
                    m_arrPointGauge[i].Dispose();
            }

            for (int i = 0; i < m_arrPointGauge2.Count; i++)
            {
                if (m_arrPointGauge2[i] != null)
                    m_arrPointGauge2[i].Dispose();
            }

            m_arrEdgeROI.Clear();
            m_arrDontCareROI.Clear();
            m_arrBlob.Clear();
            m_objBlob.Dispose();
        }

        public void ResetData()
        {
            m_fRectWidth = -1;
            m_fRectHeight = -1;

            m_fRectUpWidth = -1;
            m_fRectDownWidth = -1;
            m_fRectLeftHeight = -1;
            m_fRectRightHeight = -1;
        }

        public void InitData()
        {
        }

        public bool IsGaugeMeasureOK(int intPadIndex)
        {
            switch (intPadIndex)
            {
                case 0:
                    return (m_arrLineResultOK[0] && m_arrLineResultOK[1] && m_arrLineResultOK[2] && m_arrLineResultOK[3]);
                case 1:
                    return (m_arrLineResultOK[0] && m_arrLineResultOK[1] && m_arrLineResultOK[3]);
                case 2:
                    return (m_arrLineResultOK[0] && m_arrLineResultOK[1] && m_arrLineResultOK[2]);
                case 3:
                    return (m_arrLineResultOK[1] && m_arrLineResultOK[2] && m_arrLineResultOK[3]);
                case 4:
                    return (m_arrLineResultOK[0] && m_arrLineResultOK[2] && m_arrLineResultOK[3]);

            }

            return false;
        }

        public void DrawGaugeResult_SamplingPoint(Graphics g)
        {
            // 2021 10 26 - CCENG/CXLim: Make sure Gauge points are drawed red when gauge line fail due to extra fail condition.
            if (m_arrLineResultOK[0])
                m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.Lime);
            else
                m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.Red);

            if (m_arrLineResultOK[1])
                m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.Lime);
            else
                m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.Red);

            if (m_arrLineResultOK[2])
                m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.Lime);
            else
                m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.Red);

            if (m_arrLineResultOK[3])
                m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.Lime);
            else
                m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.Red);

            //m_arrLineGauge[0].DrawSamplingPointGauge(g);
            //m_arrLineGauge[1].DrawSamplingPointGauge(g);
            //m_arrLineGauge[2].DrawSamplingPointGauge(g);
            //m_arrLineGauge[3].DrawSamplingPointGauge(g);
        }

        public void DrawGaugeResult_SamplingPoint_ForLearning(Graphics g, int intSelectedGaugeEdgeMask)
        {
            if ((intSelectedGaugeEdgeMask & 0x01) > 0)
            {
                if (m_arrLineResultOK[0])
                    m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.Lime);
                else
                    m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.Red);
            }
            else
            {
                //if (m_arrLineResultOK[0])
                //    m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.Green);
                //else
                //    m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.DarkRed);
            }

            if ((intSelectedGaugeEdgeMask & 0x02) > 0)
            {
                if (m_arrLineResultOK[1])
                    m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.Lime);
                else
                    m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.Red);
            }
            else
            {
                //if (m_arrLineResultOK[1])
                //    m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.Green);
                //else
                //    m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.DarkRed);
            }

            if ((intSelectedGaugeEdgeMask & 0x04) > 0)
            {
                if (m_arrLineResultOK[2])
                    m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.Lime);
                else
                    m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.Red);
            }
            else
            {
                //if (m_arrLineResultOK[2])
                //    m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.Green);
                //else
                //    m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.DarkRed);
            }

            if ((intSelectedGaugeEdgeMask & 0x08) > 0)
            {
                if (m_arrLineResultOK[3])
                    m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.Lime);
                else
                    m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.Red);
            }
            else
            {
                //if (m_arrLineResultOK[3])
                //    m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.Green);
                //else
                //    m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.DarkRed);
            }
        }
        public void DrawGaugeResult_SamplingPoint_ForLearning(Graphics g, int intSelectedGaugeEdgeMask, int intPadIndex)
        {
            switch (intPadIndex)
            {
                case 0:
                case 1:
                case 3:
                    if ((intSelectedGaugeEdgeMask & 0x01) > 0)
                    {
                        if (m_arrGaugeMeasureMode.Contains(4) && intPadIndex == 0)  // for CornerPointGaugeTB Mode
                        {
                            if (m_arrLineResultOK[0])
                            {
                                if (m_arrCornerPointResult[0])
                                    m_arrPointGauge[0].DrawGaugeSamplePoint(g, Color.Lime);
                                else
                                    m_arrPointGauge[0].DrawGaugeSamplePoint(g, Color.Yellow);
                            }
                            else
                                m_arrPointGauge[0].DrawGaugeSamplePoint(g, Color.Red);

                            if (m_arrLineResultOK[0])
                            {
                                if (m_arrCornerPointResult2[0])
                                    m_arrPointGauge2[0].DrawGaugeSamplePoint(g, Color.Lime);
                                else
                                    m_arrPointGauge2[0].DrawGaugeSamplePoint(g, Color.Yellow);
                            }
                            else
                                m_arrPointGauge2[0].DrawGaugeSamplePoint(g, Color.Red);
                        }
                        else
                        {
                            if (m_arrLineResultOK[0])
                                m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.Lime);
                            else
                                m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.Red);
                        }
                    }

                    if ((intSelectedGaugeEdgeMask & 0x02) > 0)
                    {
                        if (m_arrGaugeMeasureMode.Contains(2) && intPadIndex != 0)
                        {
                            if (m_arrLineResultOK[1])
                                m_arrPointGauge[1].DrawGaugeSamplePoint(g, Color.Lime);
                            else
                                m_arrPointGauge[1].DrawGaugeSamplePoint(g, Color.Red);
                        }
                        else
                        {
                            if (m_arrLineResultOK[1])
                                m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.Lime);
                            else
                                m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.Red);
                        }
                    }

                    if ((intSelectedGaugeEdgeMask & 0x04) > 0)
                    {
                        if (m_arrGaugeMeasureMode.Contains(4) && intPadIndex == 0)  // for CornerPointGaugeTB Mode
                        {
                            if (m_arrLineResultOK[2])
                            {
                                if (m_arrCornerPointResult[2])
                                    m_arrPointGauge[2].DrawGaugeSamplePoint(g, Color.Lime);
                                else
                                    m_arrPointGauge[2].DrawGaugeSamplePoint(g, Color.Yellow);
                            }
                            else
                                m_arrPointGauge[2].DrawGaugeSamplePoint(g, Color.Red);

                            if (m_arrLineResultOK[2])
                            {
                                if (m_arrCornerPointResult2[2])
                                    m_arrPointGauge2[2].DrawGaugeSamplePoint(g, Color.Lime);
                                else
                                    m_arrPointGauge2[2].DrawGaugeSamplePoint(g, Color.Yellow);
                            }
                            else
                                m_arrPointGauge2[2].DrawGaugeSamplePoint(g, Color.Red);
                        }
                        else
                        {
                            if (m_arrLineResultOK[2])
                                m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.Lime);
                            else
                                m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.Red);
                        }
                    }

                    if ((intSelectedGaugeEdgeMask & 0x08) > 0)
                    {
                        if (m_arrGaugeMeasureMode.Contains(2) && intPadIndex != 0)
                        {
                            if (m_arrLineResultOK[3])
                                m_arrPointGauge[3].DrawGaugeSamplePoint(g, Color.Lime);
                            else
                                m_arrPointGauge[3].DrawGaugeSamplePoint(g, Color.Red);
                        }
                        else
                        {
                            if (m_arrLineResultOK[3])
                                m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.Lime);
                            else
                                m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.Red);
                        }
                    }
                    break;
                case 2:
                case 4:
                    if ((intSelectedGaugeEdgeMask & 0x01) > 0)
                    {
                        if (m_arrGaugeMeasureMode.Contains(2) && intPadIndex != 0)
                        {
                            if (m_arrLineResultOK[0])
                                m_arrPointGauge[0].DrawGaugeSamplePoint(g, Color.Lime);
                            else
                                m_arrPointGauge[0].DrawGaugeSamplePoint(g, Color.Red);
                        }
                        else
                        {
                            if (m_arrLineResultOK[0])
                                m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.Lime);
                            else
                                m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.Red);
                        }
                    }

                    if ((intSelectedGaugeEdgeMask & 0x02) > 0)
                    {
                        if (m_arrLineResultOK[1])
                            m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.Lime);
                        else
                            m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.Red);
                    }

                    if ((intSelectedGaugeEdgeMask & 0x04) > 0)
                    {
                        if (m_arrGaugeMeasureMode.Contains(2) && intPadIndex != 0)
                        {
                            if (m_arrLineResultOK[2])
                                m_arrPointGauge[2].DrawGaugeSamplePoint(g, Color.Lime);
                            else
                                m_arrPointGauge[2].DrawGaugeSamplePoint(g, Color.Red);
                        }
                        else
                        {
                            if (m_arrLineResultOK[2])
                                m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.Lime);
                            else
                                m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.Red);
                        }
                    }

                    if ((intSelectedGaugeEdgeMask & 0x08) > 0)
                    {
                        if (m_arrLineResultOK[3])
                            m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.Lime);
                        else
                            m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.Red);
                    }
                    break;
            }
        }
        public void DrawGaugeResult_ResultLine(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            //m_arrLineGauge[0].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[1].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[2].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[3].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);

            if (m_arrLineResultOK[0])
                m_arrLineGauge[0].DrawResultLineGauge(g, Color.LightGreen);
            else
                m_arrLineGauge[0].DrawResultLineGauge(g, Color.Red);

            if (m_arrLineResultOK[1])
                m_arrLineGauge[1].DrawResultLineGauge(g, Color.LightGreen);
            else
                m_arrLineGauge[1].DrawResultLineGauge(g, Color.Red);

            if (m_arrLineResultOK[2])
                m_arrLineGauge[2].DrawResultLineGauge(g, Color.LightGreen);
            else
                m_arrLineGauge[2].DrawResultLineGauge(g, Color.Red);

            if (m_arrLineResultOK[3])
                m_arrLineGauge[3].DrawResultLineGauge(g, Color.LightGreen);
            else
                m_arrLineGauge[3].DrawResultLineGauge(g, Color.Red);

            if (m_pRectCenterPoint.X != 0 && m_pRectCenterPoint.Y != 0)
            {
                PointF pRectCenterPoint = new PointF();
                if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
                {
                    pRectCenterPoint.X = (int)Math.Round((float)m_pRectCenterPoint.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                    pRectCenterPoint.Y = (int)Math.Round((float)m_pRectCenterPoint.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
                }

                g.DrawLine(new Pen(Color.LightGreen), pRectCenterPoint.X - 5, pRectCenterPoint.Y - 5, pRectCenterPoint.X + 5, pRectCenterPoint.Y + 5);
                g.DrawLine(new Pen(Color.LightGreen), pRectCenterPoint.X + 5, pRectCenterPoint.Y - 5, pRectCenterPoint.X - 5, pRectCenterPoint.Y + 5);
            }
        }
        public void DrawGaugeResult_ResultLine(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intPadIndex)
        {
            switch (intPadIndex)
            {
                case 0:
                case 1:
                case 3:

                    if (m_arrGaugeMeasureMode.Contains(4) && intPadIndex == 0)
                    {
                        if (m_arrLineResultOK[0])
                        {
                            PointF p1 = m_arrPointGauge[0].GetMeasurePoint();
                            PointF p2 = m_arrPointGauge2[0].GetMeasurePoint();

                            p1 = new PointF(p1.X * fDrawingScaleX, p1.Y * fDrawingScaleY);
                            p2 = new PointF(p2.X * fDrawingScaleX, p2.Y * fDrawingScaleY);

                            g.DrawLine(new Pen(Color.Lime, 2f), p1, p2);
                        }
                        else
                        {
                            // No need to draw red line as no enough point to create line
                        }
                    }
                    else
                    {
                        if (m_arrLineResultOK[0])
                            m_arrLineGauge[0].DrawResultLineGauge(g, Color.LightGreen);
                        else
                            m_arrLineGauge[0].DrawResultLineGauge(g, Color.Red);
                    }

                    if (m_arrGaugeMeasureMode.Contains(2) && intPadIndex != 0)
                    {

                        if (m_arrLineResultOK[1])
                            m_arrLineGauge[1].DrawPositionLineGauge(g, Color.LightGreen);
                        else
                            m_arrLineGauge[1].DrawPositionLineGauge(g, Color.Red);

                    }
                    else
                    {

                        if (m_arrLineResultOK[1])
                            m_arrLineGauge[1].DrawResultLineGauge(g, Color.LightGreen);
                        else
                            m_arrLineGauge[1].DrawResultLineGauge(g, Color.Red);

                    }

                    if (m_arrGaugeMeasureMode.Contains(4) && intPadIndex == 0)
                    {
                        if (m_arrLineResultOK[2])
                        {
                            PointF p1 = m_arrPointGauge[2].GetMeasurePoint();
                            PointF p2 = m_arrPointGauge2[2].GetMeasurePoint();

                            p1 = new PointF(p1.X * fDrawingScaleX, p1.Y * fDrawingScaleY);
                            p2 = new PointF(p2.X * fDrawingScaleX, p2.Y * fDrawingScaleY);

                            g.DrawLine(new Pen(Color.Lime, 2f), p1, p2);
                        }
                        else
                        {
                            // No need to draw red line as no enough point to create line
                        }
                    }
                    else
                    {
                        if (m_arrLineResultOK[2])
                            m_arrLineGauge[2].DrawResultLineGauge(g, Color.LightGreen);
                        else
                            m_arrLineGauge[2].DrawResultLineGauge(g, Color.Red);
                    }

                    if (m_arrGaugeMeasureMode.Contains(2) && intPadIndex != 0)
                    {

                        if (m_arrLineResultOK[3])
                            m_arrLineGauge[3].DrawPositionLineGauge(g, Color.LightGreen);
                        else
                            m_arrLineGauge[3].DrawPositionLineGauge(g, Color.Red);

                    }
                    else
                    {

                        if (m_arrLineResultOK[3])
                            m_arrLineGauge[3].DrawResultLineGauge(g, Color.LightGreen);
                        else
                            m_arrLineGauge[3].DrawResultLineGauge(g, Color.Red);

                    }

                    break;
                case 2:
                case 4:

                    if (m_arrGaugeMeasureMode.Contains(2) && intPadIndex != 0)
                    {
                        if (m_arrLineResultOK[0])
                            m_arrLineGauge[0].DrawPositionLineGauge(g, Color.LightGreen);
                        else
                            m_arrLineGauge[0].DrawPositionLineGauge(g, Color.Red);

                    }
                    else
                    {
                        if (m_arrLineResultOK[0])
                            m_arrLineGauge[0].DrawResultLineGauge(g, Color.LightGreen);
                        else
                            m_arrLineGauge[0].DrawResultLineGauge(g, Color.Red);

                    }

                    if (m_arrLineResultOK[1])
                        m_arrLineGauge[1].DrawResultLineGauge(g, Color.LightGreen);
                    else
                        m_arrLineGauge[1].DrawResultLineGauge(g, Color.Red);


                    if (m_arrGaugeMeasureMode.Contains(2) && intPadIndex != 0)
                    {

                        if (m_arrLineResultOK[2])
                            m_arrLineGauge[2].DrawPositionLineGauge(g, Color.LightGreen);
                        else
                            m_arrLineGauge[2].DrawPositionLineGauge(g, Color.Red);

                    }
                    else
                    {

                        if (m_arrLineResultOK[2])
                            m_arrLineGauge[2].DrawResultLineGauge(g, Color.LightGreen);
                        else
                            m_arrLineGauge[2].DrawResultLineGauge(g, Color.Red);

                    }

                    if (m_arrLineResultOK[3])
                        m_arrLineGauge[3].DrawResultLineGauge(g, Color.LightGreen);
                    else
                        m_arrLineGauge[3].DrawResultLineGauge(g, Color.Red);

                    break;
            }

            if (m_pRectCenterPoint.X != 0 && m_pRectCenterPoint.Y != 0)
            {
                PointF pRectCenterPoint = new PointF();
                if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
                {
                    pRectCenterPoint.X = (int)Math.Round((float)m_pRectCenterPoint.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                    pRectCenterPoint.Y = (int)Math.Round((float)m_pRectCenterPoint.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
                }

                g.DrawLine(new Pen(Color.LightGreen), pRectCenterPoint.X - 5, pRectCenterPoint.Y - 5, pRectCenterPoint.X + 5, pRectCenterPoint.Y + 5);
                g.DrawLine(new Pen(Color.LightGreen), pRectCenterPoint.X + 5, pRectCenterPoint.Y - 5, pRectCenterPoint.X - 5, pRectCenterPoint.Y + 5);
            }
        }
        public void DrawGaugeResult_ResultLine_Rotated(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intPadIndex)
        {
            //m_arrLineGauge[0].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[1].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[2].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[3].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            if (false) //(m_intGaugeTiltAngle == 0)
            {
                float fStartX = (m_pRectCenterPoint.X - m_fRectWidth / 2) * fDrawingScaleX;
                float fStartY = (m_pRectCenterPoint.Y - m_fRectHeight / 2) * fDrawingScaleY;
                float fEndX = (m_pRectCenterPoint.X + m_fRectWidth / 2) * fDrawingScaleX;
                float fEndY = (m_pRectCenterPoint.Y + m_fRectHeight / 2) * fDrawingScaleY;

                if (m_arrLineResultOK[0])
                    g.DrawLine(new Pen(Color.LightGreen), fStartX, fStartY, fEndX, fStartY);
                else
                    g.DrawLine(new Pen(Color.Red), fStartX, fStartY, fEndX, fStartY);

                if (m_arrLineResultOK[1])
                    g.DrawLine(new Pen(Color.LightGreen), fEndX, fStartY, fEndX, fEndY);
                else
                    g.DrawLine(new Pen(Color.Red), fEndX, fStartY, fEndX, fEndY);

                if (m_arrLineResultOK[2])
                    g.DrawLine(new Pen(Color.LightGreen), fStartX, fEndY, fEndX, fEndY);
                else
                    g.DrawLine(new Pen(Color.Red), fStartX, fEndY, fEndX, fEndY);

                if (m_arrLineResultOK[3])
                    g.DrawLine(new Pen(Color.LightGreen), fStartX, fStartY, fStartX, fEndY);
                else
                    g.DrawLine(new Pen(Color.Red), fStartX, fStartY, fStartX, fEndY);

                if (m_pRectCenterPoint.X != 0 && m_pRectCenterPoint.Y != 0)
                {
                    PointF pRectCenterPoint = new PointF();
                    if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
                    {
                        pRectCenterPoint.X = (int)Math.Round((float)m_pRectCenterPoint.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                        pRectCenterPoint.Y = (int)Math.Round((float)m_pRectCenterPoint.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
                    }

                    g.DrawLine(new Pen(Color.LightGreen), pRectCenterPoint.X - 5, pRectCenterPoint.Y - 5, pRectCenterPoint.X + 5, pRectCenterPoint.Y + 5);
                    g.DrawLine(new Pen(Color.LightGreen), pRectCenterPoint.X + 5, pRectCenterPoint.Y - 5, pRectCenterPoint.X - 5, pRectCenterPoint.Y + 5);
                }
            }
            else
            {


                float newX1 = m_arrRectCornerPoints[0].X, newY1 = m_arrRectCornerPoints[0].Y;
                float newX2 = m_arrRectCornerPoints[1].X, newY2 = m_arrRectCornerPoints[1].Y;
                float newX3 = m_arrRectCornerPoints[2].X, newY3 = m_arrRectCornerPoints[2].Y;
                float newX4 = m_arrRectCornerPoints[3].X, newY4 = m_arrRectCornerPoints[3].Y;


                //if (intPadIndex == 0)
                {
                    Math2.RotateWithAngleAccordingToReferencePoint(m_pRectCenterPoint.X, m_pRectCenterPoint.Y, m_arrRectCornerPoints[0].X, m_arrRectCornerPoints[0].Y, -m_fRectAngle, ref newX1, ref newY1);
                    Math2.RotateWithAngleAccordingToReferencePoint(m_pRectCenterPoint.X, m_pRectCenterPoint.Y, m_arrRectCornerPoints[1].X, m_arrRectCornerPoints[1].Y, -m_fRectAngle, ref newX2, ref newY2);
                    Math2.RotateWithAngleAccordingToReferencePoint(m_pRectCenterPoint.X, m_pRectCenterPoint.Y, m_arrRectCornerPoints[2].X, m_arrRectCornerPoints[2].Y, -m_fRectAngle, ref newX3, ref newY3);
                    Math2.RotateWithAngleAccordingToReferencePoint(m_pRectCenterPoint.X, m_pRectCenterPoint.Y, m_arrRectCornerPoints[3].X, m_arrRectCornerPoints[3].Y, -m_fRectAngle, ref newX4, ref newY4);
                }
                newX1 *= fDrawingScaleX;
                newY1 *= fDrawingScaleY;
                newX2 *= fDrawingScaleX;
                newY2 *= fDrawingScaleY;
                newX3 *= fDrawingScaleX;
                newY3 *= fDrawingScaleY;
                newX4 *= fDrawingScaleX;
                newY4 *= fDrawingScaleY;

                if (m_arrLineResultOK[0])
                    g.DrawLine(new Pen(Color.LightGreen), newX1, newY1, newX2, newY2);
                else
                    g.DrawLine(new Pen(Color.Red), newX1, newY1, newX2, newY2);

                if (m_arrLineResultOK[1])
                    g.DrawLine(new Pen(Color.LightGreen), newX2, newY2, newX3, newY3);
                else
                    g.DrawLine(new Pen(Color.Red), newX2, newY2, newX3, newY3);

                if (m_arrLineResultOK[2])
                    g.DrawLine(new Pen(Color.LightGreen), newX3, newY3, newX4, newY4);
                else
                    g.DrawLine(new Pen(Color.Red), newX3, newY3, newX4, newY4);

                if (m_arrLineResultOK[3])
                    g.DrawLine(new Pen(Color.LightGreen), newX4, newY4, newX1, newY1);
                else
                    g.DrawLine(new Pen(Color.Red), newX4, newY4, newX1, newY1);

                if (m_pRectCenterPoint.X != 0 && m_pRectCenterPoint.Y != 0)
                {
                    PointF pRectCenterPoint = new PointF();
                    if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
                    {
                        pRectCenterPoint.X = (int)Math.Round((float)m_pRectCenterPoint.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                        pRectCenterPoint.Y = (int)Math.Round((float)m_pRectCenterPoint.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
                    }

                    g.DrawLine(new Pen(Color.LightGreen), pRectCenterPoint.X - 5, pRectCenterPoint.Y - 5, pRectCenterPoint.X + 5, pRectCenterPoint.Y + 5);
                    g.DrawLine(new Pen(Color.LightGreen), pRectCenterPoint.X + 5, pRectCenterPoint.Y - 5, pRectCenterPoint.X - 5, pRectCenterPoint.Y + 5);
                }


            }

        }
        public void DrawGaugeResult_ResultLine_Rotated(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intPadIndex, bool blnWantRotateSidePad)
        {
            float newX1 = m_arrRectCornerPoints[0].X, newY1 = m_arrRectCornerPoints[0].Y;
            float newX2 = m_arrRectCornerPoints[1].X, newY2 = m_arrRectCornerPoints[1].Y;
            float newX3 = m_arrRectCornerPoints[2].X, newY3 = m_arrRectCornerPoints[2].Y;
            float newX4 = m_arrRectCornerPoints[3].X, newY4 = m_arrRectCornerPoints[3].Y;


            if (intPadIndex == 0 || (intPadIndex > 0 && blnWantRotateSidePad))
            {
                Math2.RotateWithAngleAccordingToReferencePoint(m_pRectCenterPoint.X, m_pRectCenterPoint.Y, m_arrRectCornerPoints[0].X, m_arrRectCornerPoints[0].Y, -m_fRectAngle, ref newX1, ref newY1);
                Math2.RotateWithAngleAccordingToReferencePoint(m_pRectCenterPoint.X, m_pRectCenterPoint.Y, m_arrRectCornerPoints[1].X, m_arrRectCornerPoints[1].Y, -m_fRectAngle, ref newX2, ref newY2);
                Math2.RotateWithAngleAccordingToReferencePoint(m_pRectCenterPoint.X, m_pRectCenterPoint.Y, m_arrRectCornerPoints[2].X, m_arrRectCornerPoints[2].Y, -m_fRectAngle, ref newX3, ref newY3);
                Math2.RotateWithAngleAccordingToReferencePoint(m_pRectCenterPoint.X, m_pRectCenterPoint.Y, m_arrRectCornerPoints[3].X, m_arrRectCornerPoints[3].Y, -m_fRectAngle, ref newX4, ref newY4);
            }
            newX1 *= fDrawingScaleX;
            newY1 *= fDrawingScaleY;
            newX2 *= fDrawingScaleX;
            newY2 *= fDrawingScaleY;
            newX3 *= fDrawingScaleX;
            newY3 *= fDrawingScaleY;
            newX4 *= fDrawingScaleX;
            newY4 *= fDrawingScaleY;

            if (m_arrLineResultOK[0])
                g.DrawLine(new Pen(Color.LightGreen), newX1, newY1, newX2, newY2);
            else
                g.DrawLine(new Pen(Color.Red), newX1, newY1, newX2, newY2);

            if (m_arrLineResultOK[1])
                g.DrawLine(new Pen(Color.LightGreen), newX2, newY2, newX3, newY3);
            else
                g.DrawLine(new Pen(Color.Red), newX2, newY2, newX3, newY3);

            if (m_arrLineResultOK[2])
                g.DrawLine(new Pen(Color.LightGreen), newX3, newY3, newX4, newY4);
            else
                g.DrawLine(new Pen(Color.Red), newX3, newY3, newX4, newY4);

            if (m_arrLineResultOK[3])
                g.DrawLine(new Pen(Color.LightGreen), newX4, newY4, newX1, newY1);
            else
                g.DrawLine(new Pen(Color.Red), newX4, newY4, newX1, newY1);

            if (m_pRectCenterPoint.X != 0 && m_pRectCenterPoint.Y != 0)
            {
                PointF pRectCenterPoint = new PointF();
                if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
                {
                    pRectCenterPoint.X = (int)Math.Round((float)m_pRectCenterPoint.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                    pRectCenterPoint.Y = (int)Math.Round((float)m_pRectCenterPoint.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
                }

                g.DrawLine(new Pen(Color.LightGreen), pRectCenterPoint.X - 5, pRectCenterPoint.Y - 5, pRectCenterPoint.X + 5, pRectCenterPoint.Y + 5);
                g.DrawLine(new Pen(Color.LightGreen), pRectCenterPoint.X + 5, pRectCenterPoint.Y - 5, pRectCenterPoint.X - 5, pRectCenterPoint.Y + 5);
            }
        }
        public void DrawGaugeResult_EdgeNotFound(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            if (m_arrLineResultOK[0])
                m_arrLineGauge[0].DrawResultLineGauge(g, Color.LightGreen);
            else
                m_arrLineGauge[0].DrawResultLineGauge(g, Color.Red);

            if (m_arrLineResultOK[1])
                m_arrLineGauge[1].DrawResultLineGauge(g, Color.LightGreen);
            else
                m_arrLineGauge[1].DrawResultLineGauge(g, Color.Red);

            if (m_arrLineResultOK[2])
                m_arrLineGauge[2].DrawResultLineGauge(g, Color.LightGreen);
            else
                m_arrLineGauge[2].DrawResultLineGauge(g, Color.Red);

            if (m_arrLineResultOK[3])
                m_arrLineGauge[3].DrawResultLineGauge(g, Color.LightGreen);
            else
                m_arrLineGauge[3].DrawResultLineGauge(g, Color.Red);
        }

        public void DrawGaugeResult_ResultLine_ScoreCompare(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            //m_arrLineGauge[0].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[1].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[2].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[3].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            m_arrLineGauge[0].DrawResultLineGauge(g);
            m_arrLineGauge[1].DrawResultLineGauge(g);
            m_arrLineGauge[2].DrawResultLineGauge(g);
            m_arrLineGauge[3].DrawResultLineGauge(g);

            PointF pRectCenterPoint = m_pRectCenterPoint;
            if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
            {
                pRectCenterPoint.X = (int)Math.Round((float)pRectCenterPoint.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pRectCenterPoint.Y = (int)Math.Round((float)pRectCenterPoint.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
            }

            g.DrawLine(new Pen(Color.LightGreen), pRectCenterPoint.X - 5, pRectCenterPoint.Y - 5, pRectCenterPoint.X + 5, pRectCenterPoint.Y + 5);
            g.DrawLine(new Pen(Color.LightGreen), pRectCenterPoint.X + 5, pRectCenterPoint.Y - 5, pRectCenterPoint.X - 5, pRectCenterPoint.Y + 5);
        }

        public void DrawGaugeResult_CenterPoint(Graphics g)
        {
            g.DrawLine(new Pen(Color.Lime, 2), m_pRectCenterPoint.X - 5, m_pRectCenterPoint.Y, m_pRectCenterPoint.X + 5, m_pRectCenterPoint.Y);
            g.DrawLine(new Pen(Color.Lime, 2), m_pRectCenterPoint.X, m_pRectCenterPoint.Y - 5, m_pRectCenterPoint.X, m_pRectCenterPoint.Y + 5);
        }

        public void DrawGaugeResult(Graphics g)
        {
            m_arrLineGauge[0].DrawRealGauge(g);
            m_arrLineGauge[1].DrawRealGauge(g);
            m_arrLineGauge[2].DrawRealGauge(g);
            m_arrLineGauge[3].DrawRealGauge(g);
        }

        public void DrawGaugeSetting_Inside(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            PointF pStartPoint = new PointF(m_arrLineGauge[3].ref_GaugeCenterX + m_arrLineGauge[3].ref_GaugeTolerance,
                m_arrLineGauge[0].ref_GaugeCenterY + m_arrLineGauge[0].ref_GaugeTolerance);
            PointF pEndPoint = new PointF(m_arrLineGauge[1].ref_GaugeCenterX - m_arrLineGauge[1].ref_GaugeTolerance,
                m_arrLineGauge[2].ref_GaugeCenterY - m_arrLineGauge[2].ref_GaugeTolerance);

            if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
            {
                pStartPoint.X = (int)Math.Round((float)pStartPoint.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pStartPoint.Y = (int)Math.Round((float)pStartPoint.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
                pEndPoint.X = (int)Math.Round((float)pEndPoint.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pEndPoint.Y = (int)Math.Round((float)pEndPoint.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
            }

            g.DrawRectangle(m_PenRectGNominal, pStartPoint.X, pStartPoint.Y, pEndPoint.X - pStartPoint.X, pEndPoint.Y - pStartPoint.Y);
        }
        public void DrawGaugeSetting_TransitionTypeArrow(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intSelectedGaugeEdgeMask)
        {
            // ------ Draw top gauge arrow ------

            PointF pTail = new PointF(m_arrLineGauge[0].ref_GaugeCenterX + m_arrLineGauge[0].ref_GaugeLength / 4,
                m_arrLineGauge[0].ref_GaugeCenterY - m_arrLineGauge[0].ref_GaugeTolerance);

            PointF pHead = new PointF(m_arrLineGauge[0].ref_GaugeCenterX + m_arrLineGauge[0].ref_GaugeLength / 4,
                m_arrLineGauge[0].ref_GaugeCenterY + m_arrLineGauge[0].ref_GaugeTolerance);

            if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
            {
                pTail.X = (int)Math.Round((float)pTail.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pTail.Y = (int)Math.Round((float)pTail.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
                pHead.X = (int)Math.Round((float)pHead.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pHead.Y = (int)Math.Round((float)pHead.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
            }

            if ((intSelectedGaugeEdgeMask & 0x01) > 0)
            {
                if (m_PenRectGTransitionTypeArrow.Color != Color.Lime)
                    m_PenRectGTransitionTypeArrow.Color = Color.Lime;
            }
            else
            {
                if (m_PenRectGTransitionTypeArrow.Color != Color.Green)
                    m_PenRectGTransitionTypeArrow.Color = Color.Green;
            }

            g.DrawLine(m_PenRectGTransitionTypeArrow, pTail, pHead);
            g.DrawLine(m_PenRectGTransitionTypeArrow, pHead, new PointF(pHead.X - 5, pHead.Y - 5));
            g.DrawLine(m_PenRectGTransitionTypeArrow, pHead, new PointF(pHead.X + 5, pHead.Y - 5));

            // ------ Draw right gauge arrow ------

            pTail = new PointF(m_arrLineGauge[1].ref_GaugeCenterX + m_arrLineGauge[1].ref_GaugeTolerance,
                m_arrLineGauge[1].ref_GaugeCenterY + m_arrLineGauge[1].ref_GaugeLength / 4);

            pHead = new PointF(m_arrLineGauge[1].ref_GaugeCenterX - m_arrLineGauge[1].ref_GaugeTolerance,
                m_arrLineGauge[1].ref_GaugeCenterY + m_arrLineGauge[1].ref_GaugeLength / 4);

            if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
            {
                pTail.X = (int)Math.Round((float)pTail.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pTail.Y = (int)Math.Round((float)pTail.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
                pHead.X = (int)Math.Round((float)pHead.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pHead.Y = (int)Math.Round((float)pHead.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
            }

            if ((intSelectedGaugeEdgeMask & 0x02) > 0)
            {
                if (m_PenRectGTransitionTypeArrow.Color != Color.Lime)
                    m_PenRectGTransitionTypeArrow.Color = Color.Lime;
            }
            else
            {
                if (m_PenRectGTransitionTypeArrow.Color != Color.Green)
                    m_PenRectGTransitionTypeArrow.Color = Color.Green;
            }

            g.DrawLine(m_PenRectGTransitionTypeArrow, pTail, pHead);
            g.DrawLine(m_PenRectGTransitionTypeArrow, pHead, new PointF(pHead.X + 5, pHead.Y - 5));
            g.DrawLine(m_PenRectGTransitionTypeArrow, pHead, new PointF(pHead.X + 5, pHead.Y + 5));

            // ------ Draw bottom gauge arrow ------

            pTail = new PointF(m_arrLineGauge[2].ref_GaugeCenterX - m_arrLineGauge[2].ref_GaugeLength / 4,
                m_arrLineGauge[2].ref_GaugeCenterY + m_arrLineGauge[2].ref_GaugeTolerance);

            pHead = new PointF(m_arrLineGauge[2].ref_GaugeCenterX - m_arrLineGauge[2].ref_GaugeLength / 4,
                m_arrLineGauge[2].ref_GaugeCenterY - m_arrLineGauge[2].ref_GaugeTolerance);

            if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
            {
                pTail.X = (int)Math.Round((float)pTail.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pTail.Y = (int)Math.Round((float)pTail.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
                pHead.X = (int)Math.Round((float)pHead.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pHead.Y = (int)Math.Round((float)pHead.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
            }

            if ((intSelectedGaugeEdgeMask & 0x04) > 0)
            {
                if (m_PenRectGTransitionTypeArrow.Color != Color.Lime)
                    m_PenRectGTransitionTypeArrow.Color = Color.Lime;
            }
            else
            {
                if (m_PenRectGTransitionTypeArrow.Color != Color.Green)
                    m_PenRectGTransitionTypeArrow.Color = Color.Green;
            }

            g.DrawLine(m_PenRectGTransitionTypeArrow, pTail, pHead);
            g.DrawLine(m_PenRectGTransitionTypeArrow, pHead, new PointF(pHead.X + 5, pHead.Y + 5));
            g.DrawLine(m_PenRectGTransitionTypeArrow, pHead, new PointF(pHead.X - 5, pHead.Y + 5));

            // ------ Draw left gauge arrow ------

            pTail = new PointF(m_arrLineGauge[3].ref_GaugeCenterX - m_arrLineGauge[3].ref_GaugeTolerance,
                m_arrLineGauge[3].ref_GaugeCenterY - m_arrLineGauge[3].ref_GaugeLength / 4);

            pHead = new PointF(m_arrLineGauge[3].ref_GaugeCenterX + m_arrLineGauge[3].ref_GaugeTolerance,
                m_arrLineGauge[3].ref_GaugeCenterY - m_arrLineGauge[3].ref_GaugeLength / 4);


            if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
            {
                pTail.X = (int)Math.Round((float)pTail.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pTail.Y = (int)Math.Round((float)pTail.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
                pHead.X = (int)Math.Round((float)pHead.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pHead.Y = (int)Math.Round((float)pHead.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
            }

            if ((intSelectedGaugeEdgeMask & 0x08) > 0)
            {
                if (m_PenRectGTransitionTypeArrow.Color != Color.Lime)
                    m_PenRectGTransitionTypeArrow.Color = Color.Lime;
            }
            else
            {
                if (m_PenRectGTransitionTypeArrow.Color != Color.Green)
                    m_PenRectGTransitionTypeArrow.Color = Color.Green;
            }

            g.DrawLine(m_PenRectGTransitionTypeArrow, pTail, pHead);
            g.DrawLine(m_PenRectGTransitionTypeArrow, pHead, new PointF(pHead.X - 5, pHead.Y + 5));
            g.DrawLine(m_PenRectGTransitionTypeArrow, pHead, new PointF(pHead.X - 5, pHead.Y - 5));

            // Reset to lime as default
            if (m_PenRectGTransitionTypeArrow.Color != Color.Lime)
                m_PenRectGTransitionTypeArrow.Color = Color.Lime;
        }
        public void DrawGaugeSetting(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            PointF pStartPoint = new PointF(m_arrLineGauge[3].ref_GaugeCenterX - m_arrLineGauge[3].ref_GaugeTolerance,
                m_arrLineGauge[0].ref_GaugeCenterY - m_arrLineGauge[0].ref_GaugeTolerance);
            PointF pEndPoint = new PointF(m_arrLineGauge[1].ref_GaugeCenterX + m_arrLineGauge[1].ref_GaugeTolerance,
                m_arrLineGauge[2].ref_GaugeCenterY + m_arrLineGauge[2].ref_GaugeTolerance);

            if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
            {
                pStartPoint.X = (int)Math.Round((float)pStartPoint.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pStartPoint.Y = (int)Math.Round((float)pStartPoint.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
                pEndPoint.X = (int)Math.Round((float)pEndPoint.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pEndPoint.Y = (int)Math.Round((float)pEndPoint.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
            }

            g.DrawRectangle(m_PenRectGNominal, pStartPoint.X, pStartPoint.Y, pEndPoint.X - pStartPoint.X, pEndPoint.Y - pStartPoint.Y);

            pStartPoint = new PointF(m_arrLineGauge[3].ref_GaugeCenterX + m_arrLineGauge[3].ref_GaugeTolerance,
                m_arrLineGauge[0].ref_GaugeCenterY + m_arrLineGauge[0].ref_GaugeTolerance);
            pEndPoint = new PointF(m_arrLineGauge[1].ref_GaugeCenterX - m_arrLineGauge[1].ref_GaugeTolerance,
                m_arrLineGauge[2].ref_GaugeCenterY - m_arrLineGauge[2].ref_GaugeTolerance);

            if (fDrawingScaleX != 1f || fDrawingScaleY != 1f)
            {
                pStartPoint.X = (int)Math.Round((float)pStartPoint.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pStartPoint.Y = (int)Math.Round((float)pStartPoint.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
                pEndPoint.X = (int)Math.Round((float)pEndPoint.X * fDrawingScaleX, 0, MidpointRounding.AwayFromZero);
                pEndPoint.Y = (int)Math.Round((float)pEndPoint.Y * fDrawingScaleY, 0, MidpointRounding.AwayFromZero);
            }

            g.DrawRectangle(m_PenRectGNominal, pStartPoint.X, pStartPoint.Y, pEndPoint.X - pStartPoint.X, pEndPoint.Y - pStartPoint.Y);
        }

        public void AttachEdgeROI(ImageDrawing objImage)
        {
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                m_arrEdgeROI[i].AttachImage(objImage);
            }
        }

        public void SetEdgeROIPlacementLimit(ROI objROI)
        {
            int intLimitStartX = objROI.ref_ROITotalX;
            int intLimitStartY = objROI.ref_ROITotalY;
            int intLimitEndX = objROI.ref_ROITotalX + objROI.ref_ROIWidth;
            int intLimitEndY = objROI.ref_ROITotalY + objROI.ref_ROIHeight;

            // ------------- Top -------------------------------------
            if (m_arrEdgeROI[0].ref_ROIPositionX < intLimitStartX || m_arrEdgeROI[0].ref_ROIPositionX > intLimitEndX || m_arrEdgeROI[0].ref_ROIWidth >= intLimitEndX - intLimitStartX)
            {
                m_arrEdgeROI[0].ref_ROIWidth = intLimitEndX - intLimitStartX;
                m_arrEdgeROI[0].ref_ROIPositionX = intLimitStartX;
            }

            //if (m_arrEdgeROI[0].ref_ROIWidth >= intLimitEndX - intLimitStartX)
            //{
            //    m_arrEdgeROI[0].ref_ROIWidth = intLimitEndX - intLimitStartX;
            //    m_arrEdgeROI[0].ref_ROIPositionX = intLimitStartX;
            //}

            if (m_arrEdgeROI[0].ref_ROIPositionY < intLimitStartY || m_arrEdgeROI[0].ref_ROIPositionY > intLimitEndY)
            {
                m_arrEdgeROI[0].ref_ROIPositionY = intLimitStartY;
            }

            //if (m_arrEdgeROI[0].ref_ROIHeight >= (intLimitEndY - intLimitStartY) / 5)
            //{
            //    m_arrEdgeROI[0].ref_ROIHeight = (intLimitEndY - intLimitStartY) / 5;
            //    m_arrEdgeROI[0].ref_ROIPositionY = intLimitStartY;
            //}

            // ------------- Right -------------------------------------

            if (m_arrEdgeROI[1].ref_ROIPositionX < intLimitStartX || m_arrEdgeROI[1].ref_ROIPositionX > intLimitEndX)
            {
                m_arrEdgeROI[1].ref_ROIPositionX = intLimitEndX - (intLimitEndX - intLimitStartX) / 5;
            }

            //if (m_arrEdgeROI[1].ref_ROIWidth >= (intLimitEndX - intLimitStartX) / 5)
            //{
            //    m_arrEdgeROI[1].ref_ROIWidth = (intLimitEndX - intLimitStartX) / 5;
            //    m_arrEdgeROI[1].ref_ROIPositionX = intLimitEndX - (intLimitEndX - intLimitStartX) / 5;
            //}

            if (m_arrEdgeROI[1].ref_ROIPositionY < intLimitStartY || m_arrEdgeROI[1].ref_ROIPositionY > intLimitEndY || m_arrEdgeROI[1].ref_ROIHeight >= intLimitEndY - intLimitStartY)
            {
                m_arrEdgeROI[1].ref_ROIHeight = intLimitEndY - intLimitStartY;
                m_arrEdgeROI[1].ref_ROIPositionY = intLimitStartY;
            }

            //if (m_arrEdgeROI[1].ref_ROIHeight >= intLimitEndY - intLimitStartY)
            //{
            //    m_arrEdgeROI[1].ref_ROIHeight = intLimitEndY - intLimitStartY;
            //    m_arrEdgeROI[1].ref_ROIPositionY = intLimitStartY;
            //}

            // ------------- Bottom -------------------------------------
            //if (m_arrEdgeROI[2].ref_ROIPositionX < intLimitStartX)
            //{
            //    m_arrEdgeROI[2].ref_ROIPositionX = intLimitStartX;
            //}

            //if ((m_arrEdgeROI[2].ref_ROIPositionX + m_arrEdgeROI[2].ref_ROIWidth) >= intLimitEndX)
            //{
            //    m_arrEdgeROI[2].ref_ROIPositionX = intLimitStartX;
            //    m_arrEdgeROI[2].ref_ROIWidth = intLimitEndX - intLimitStartX;
            //}

            //if ((m_arrEdgeROI[2].ref_ROIPositionY + m_arrEdgeROI[2].ref_ROIHeight) >= intLimitEndY)
            //{
            //    m_arrEdgeROI[2].ref_ROIHeight = objROI.ref_ROIHeight / 5;
            //    m_arrEdgeROI[2].ref_ROIPositionY = intLimitEndY - m_arrEdgeROI[2].ref_ROIHeight;
            //}

            if (m_arrEdgeROI[2].ref_ROIPositionX < intLimitStartX || m_arrEdgeROI[2].ref_ROIPositionX > intLimitEndX || m_arrEdgeROI[2].ref_ROIWidth >= intLimitEndX - intLimitStartX)
            {
                m_arrEdgeROI[2].ref_ROIWidth = intLimitEndX - intLimitStartX;
                m_arrEdgeROI[2].ref_ROIPositionX = intLimitStartX;
            }

            //if (m_arrEdgeROI[2].ref_ROIWidth >= intLimitEndX - intLimitStartX)
            //{
            //    m_arrEdgeROI[2].ref_ROIWidth = intLimitEndX - intLimitStartX;
            //    m_arrEdgeROI[2].ref_ROIPositionX = intLimitStartX;
            //}

            if (m_arrEdgeROI[2].ref_ROIPositionY < intLimitStartY || m_arrEdgeROI[2].ref_ROIPositionY > intLimitEndY)
            {
                m_arrEdgeROI[2].ref_ROIPositionY = intLimitEndY - (intLimitEndY - intLimitStartY) / 5;
            }

            //if (m_arrEdgeROI[2].ref_ROIHeight >= (intLimitEndY - intLimitStartY) / 5)
            //{
            //    m_arrEdgeROI[2].ref_ROIHeight = (intLimitEndY - intLimitStartY) / 5;
            //    m_arrEdgeROI[2].ref_ROIPositionY = intLimitEndY - (intLimitEndY - intLimitStartY) / 5;
            //}

            // ------------- Left -------------------------------------
            //if ((m_arrEdgeROI[3].ref_ROIPositionY + m_arrEdgeROI[3].ref_ROIHeight) >= intLimitEndY)
            //{
            //    m_arrEdgeROI[3].ref_ROIPositionY = intLimitStartY;
            //    m_arrEdgeROI[3].ref_ROIHeight = intLimitEndY - intLimitStartY;
            //}

            //if (m_arrEdgeROI[3].ref_ROIPositionY < intLimitStartY)
            //{
            //    m_arrEdgeROI[3].ref_ROIPositionY = intLimitStartY;
            //}

            //if (m_arrEdgeROI[3].ref_ROIPositionX < intLimitStartX)
            //{
            //    m_arrEdgeROI[3].ref_ROIPositionX = intLimitStartX;
            //}

            if (m_arrEdgeROI[3].ref_ROIPositionX < intLimitStartX || m_arrEdgeROI[3].ref_ROIPositionX > intLimitEndX)
            {
                m_arrEdgeROI[3].ref_ROIPositionX = intLimitStartX;
            }

            //if (m_arrEdgeROI[3].ref_ROIWidth >= (intLimitEndX - intLimitStartX) / 5)
            //{
            //    m_arrEdgeROI[3].ref_ROIWidth = (intLimitEndX - intLimitStartX) / 5;
            //    m_arrEdgeROI[3].ref_ROIPositionX = intLimitStartX;
            //}

            if (m_arrEdgeROI[3].ref_ROIPositionY < intLimitStartY || m_arrEdgeROI[3].ref_ROIPositionY > intLimitEndY || m_arrEdgeROI[3].ref_ROIHeight >= intLimitEndY - intLimitStartY)
            {
                m_arrEdgeROI[3].ref_ROIHeight = intLimitEndY - intLimitStartY;
                m_arrEdgeROI[3].ref_ROIPositionY = intLimitStartY;
            }

            //if (m_arrEdgeROI[3].ref_ROIHeight >= intLimitEndY - intLimitStartY)
            //{
            //    m_arrEdgeROI[3].ref_ROIHeight = intLimitEndY - intLimitStartY;
            //    m_arrEdgeROI[3].ref_ROIPositionY = intLimitStartY;
            //}

            // ------------- Top -------------------------------------
            if ((m_arrEdgeROI[0].ref_ROIPositionY + m_arrEdgeROI[0].ref_ROIHeight) > m_arrEdgeROI[2].ref_ROIPositionY)
            {
                m_arrEdgeROI[0].ref_ROIPositionY = intLimitStartY;
                m_arrEdgeROI[0].ref_ROIHeight = objROI.ref_ROIHeight / 5;
            }

            // ------------- Right -------------------------------------
            if (m_arrEdgeROI[1].ref_ROIPositionX < (m_arrEdgeROI[3].ref_ROIPositionX + m_arrEdgeROI[3].ref_ROIWidth))
            {
                m_arrEdgeROI[1].ref_ROIWidth = objROI.ref_ROIWidth / 5;
                m_arrEdgeROI[1].ref_ROIPositionX = objROI.ref_ROIWidth - m_arrEdgeROI[2].ref_ROIWidth;
            }

            // ------------- Bottom -------------------------------------
            if (m_arrEdgeROI[2].ref_ROIPositionY < (m_arrEdgeROI[0].ref_ROIPositionY + m_arrEdgeROI[0].ref_ROIHeight))
            {
                m_arrEdgeROI[2].ref_ROIHeight = objROI.ref_ROIHeight / 5;
                m_arrEdgeROI[2].ref_ROIPositionY = objROI.ref_ROIHeight - m_arrEdgeROI[2].ref_ROIHeight;
            }

            // ------------- Left -------------------------------------
            if ((m_arrEdgeROI[3].ref_ROIPositionX + m_arrEdgeROI[3].ref_ROIWidth) > m_arrEdgeROI[1].ref_ROIPositionX)
            {
                m_arrEdgeROI[3].ref_ROIPositionX = intLimitStartX;
                m_arrEdgeROI[3].ref_ROIWidth = objROI.ref_ROIWidth / 5;
            }
        }

        public void SetEdgeROIPlacementLimit2(ROI objROI)
        {
            int intLimitStartX = objROI.ref_ROITotalX;
            int intLimitStartY = objROI.ref_ROITotalY;
            int intLimitEndX = objROI.ref_ROITotalX + objROI.ref_ROIWidth;
            int intLimitEndY = objROI.ref_ROITotalY + objROI.ref_ROIHeight;
            int intCenterX = objROI.ref_ROITotalX + (objROI.ref_ROIWidth / 2);
            int intCenterY = objROI.ref_ROITotalY + (objROI.ref_ROIHeight / 2);

            // ------------- Top -------------------------------------
            if (m_arrEdgeROI[0].ref_ROIPositionX < 0 || m_arrEdgeROI[0].ref_ROIPositionY < 0)
            {
                m_intEdgeROICenterX[0] = intCenterX;
                m_intEdgeROICenterY[0] = intLimitStartY;

                int intEdgeROIPositionX = m_intEdgeROICenterX[0] - m_arrEdgeROI[0].ref_ROIWidth / 2;
                if (intEdgeROIPositionX < 0)
                    m_arrEdgeROI[0].ref_ROIPositionX = 0;
                else
                    m_arrEdgeROI[0].ref_ROIPositionX = intEdgeROIPositionX;

                int intEdgeROIPositionY = m_intEdgeROICenterY[0] - m_arrEdgeROI[0].ref_ROIHeight / 2;
                if (intEdgeROIPositionY < 0)
                    m_arrEdgeROI[0].ref_ROIPositionY = 0;
                else
                    m_arrEdgeROI[0].ref_ROIPositionY = intEdgeROIPositionY;
            }

            // ------------- Right -------------------------------------
            if (m_arrEdgeROI[1].ref_ROIPositionX < 0 || m_arrEdgeROI[1].ref_ROIPositionY < 0)
            {
                m_intEdgeROICenterX[1] = intLimitEndX;
                m_intEdgeROICenterY[1] = intCenterY;

                int intEdgeROIPositionX = m_intEdgeROICenterX[1] - m_arrEdgeROI[1].ref_ROIWidth / 2;
                if (intEdgeROIPositionX < 0)
                    m_arrEdgeROI[1].ref_ROIPositionX = 0;
                else
                    m_arrEdgeROI[1].ref_ROIPositionX = intEdgeROIPositionX;

                int intEdgeROIPositionY = m_intEdgeROICenterY[1] - m_arrEdgeROI[1].ref_ROIHeight / 2;
                if (intEdgeROIPositionY < 0)
                    m_arrEdgeROI[1].ref_ROIPositionY = 0;
                else
                    m_arrEdgeROI[1].ref_ROIPositionY = intEdgeROIPositionY;
            }

            // ------------- Bottom -------------------------------------
            if (m_arrEdgeROI[2].ref_ROIPositionX < 0 || m_arrEdgeROI[2].ref_ROIPositionY < 0)
            {
                m_intEdgeROICenterX[2] = intCenterX;
                m_intEdgeROICenterY[2] = intLimitEndY;

                int intEdgeROIPositionX = m_intEdgeROICenterX[2] - m_arrEdgeROI[2].ref_ROIWidth / 2;
                if (intEdgeROIPositionX < 0)
                    m_arrEdgeROI[2].ref_ROIPositionX = 0;
                else
                    m_arrEdgeROI[2].ref_ROIPositionX = intEdgeROIPositionX;

                int intEdgeROIPositionY = m_intEdgeROICenterY[2] - m_arrEdgeROI[2].ref_ROIHeight / 2;
                if (intEdgeROIPositionY < 0)
                    m_arrEdgeROI[2].ref_ROIPositionY = 0;
                else
                    m_arrEdgeROI[2].ref_ROIPositionY = intEdgeROIPositionY;
            }

            // ------------- Left -------------------------------------
            if (m_arrEdgeROI[3].ref_ROIPositionX < 0 || m_arrEdgeROI[3].ref_ROIPositionY < 0)
            {
                m_intEdgeROICenterX[3] = intLimitStartX;
                m_intEdgeROICenterY[3] = intCenterY;

                int intEdgeROIPositionX = m_intEdgeROICenterX[3] - m_arrEdgeROI[3].ref_ROIWidth / 2;
                if (intEdgeROIPositionX < 0)
                    m_arrEdgeROI[3].ref_ROIPositionX = 0;
                else
                    m_arrEdgeROI[3].ref_ROIPositionX = intEdgeROIPositionX;

                int intEdgeROIPositionY = m_intEdgeROICenterY[3] - m_arrEdgeROI[3].ref_ROIHeight / 2;
                if (intEdgeROIPositionY < 0)
                    m_arrEdgeROI[3].ref_ROIPositionY = 0;
                else
                    m_arrEdgeROI[3].ref_ROIPositionY = intEdgeROIPositionY;
            }
        }

        public void ResetEdgeROIPlacement(ROI objROI)
        {
            int intLimitStartX = objROI.ref_ROITotalX;
            int intLimitStartY = objROI.ref_ROITotalY;
            int intLimitEndX = objROI.ref_ROITotalX + objROI.ref_ROIWidth;
            int intLimitEndY = objROI.ref_ROITotalY + objROI.ref_ROIHeight;
            int intCenterX = objROI.ref_ROITotalX + (objROI.ref_ROIWidth / 2);
            int intCenterY = objROI.ref_ROITotalY + (objROI.ref_ROIHeight / 2);

            // ------------- Top -------------------------------------
            //if (m_arrEdgeROI[0].ref_ROIPositionX == 0 || m_arrEdgeROI[0].ref_ROIPositionY == 0)
            {
                m_intEdgeROICenterX[0] = intCenterX;
                m_intEdgeROICenterY[0] = intLimitStartY + m_arrEdgeROI[0].ref_ROIHeight / 2;
                m_arrEdgeROI[0].ref_ROIPositionX = m_intEdgeROICenterX[0] - m_arrEdgeROI[0].ref_ROIWidth / 2;
                m_arrEdgeROI[0].ref_ROIPositionY = m_intEdgeROICenterY[0] - m_arrEdgeROI[0].ref_ROIHeight / 2;
            }

            // ------------- Right -------------------------------------
            //if (m_arrEdgeROI[1].ref_ROIPositionX == 0 || m_arrEdgeROI[1].ref_ROIPositionY == 0)
            {
                m_intEdgeROICenterX[1] = intLimitEndX - m_arrEdgeROI[1].ref_ROIWidth / 2;
                m_intEdgeROICenterY[1] = intCenterY;
                m_arrEdgeROI[1].ref_ROIPositionX = m_intEdgeROICenterX[1] - m_arrEdgeROI[1].ref_ROIWidth / 2;
                m_arrEdgeROI[1].ref_ROIPositionY = m_intEdgeROICenterY[1] - m_arrEdgeROI[1].ref_ROIHeight / 2;
            }

            // ------------- Bottom -------------------------------------
            //if (m_arrEdgeROI[2].ref_ROIPositionX == 0 || m_arrEdgeROI[2].ref_ROIPositionY == 0)
            {
                m_intEdgeROICenterX[2] = intCenterX;
                m_intEdgeROICenterY[2] = intLimitEndY - m_arrEdgeROI[2].ref_ROIHeight / 2;
                m_arrEdgeROI[2].ref_ROIPositionX = m_intEdgeROICenterX[2] - m_arrEdgeROI[2].ref_ROIWidth / 2;
                m_arrEdgeROI[2].ref_ROIPositionY = m_intEdgeROICenterY[2] - m_arrEdgeROI[2].ref_ROIHeight / 2;
            }

            // ------------- Left -------------------------------------
            //if (m_arrEdgeROI[3].ref_ROIPositionX == 0 || m_arrEdgeROI[3].ref_ROIPositionY == 0)
            {
                m_intEdgeROICenterX[3] = intLimitStartX + m_arrEdgeROI[3].ref_ROIWidth / 2;
                m_intEdgeROICenterY[3] = intCenterY;
                m_arrEdgeROI[3].ref_ROIPositionX = m_intEdgeROICenterX[3] - m_arrEdgeROI[3].ref_ROIWidth / 2;
                m_arrEdgeROI[3].ref_ROIPositionY = m_intEdgeROICenterY[3] - m_arrEdgeROI[3].ref_ROIHeight / 2;
            }

        }
        public void SetEdgeROIPlacementLimit_ForAutoTune(ROI objROI, int UnitCenterX, int UnitCenterY, int intROIIndex)
        {
            if (intROIIndex == 0)
            {
                // ------------- Top -------------------------------------

                m_intEdgeROICenterX[0] = UnitCenterX;
                m_intEdgeROICenterY[0] = UnitCenterY;
                m_arrEdgeROI[0].ref_ROIPositionX = m_intEdgeROICenterX[0] - m_arrEdgeROI[0].ref_ROIWidth / 2;
                m_arrEdgeROI[0].ref_ROIPositionY = m_intEdgeROICenterY[0] - m_arrEdgeROI[0].ref_ROIHeight / 2;
            }
            else if (intROIIndex == 1)
            {
                // ------------- Right -------------------------------------

                m_intEdgeROICenterX[1] = UnitCenterX;
                m_intEdgeROICenterY[1] = UnitCenterY;
                m_arrEdgeROI[1].ref_ROIPositionX = m_intEdgeROICenterX[1] - m_arrEdgeROI[1].ref_ROIWidth / 2;
                m_arrEdgeROI[1].ref_ROIPositionY = m_intEdgeROICenterY[1] - m_arrEdgeROI[1].ref_ROIHeight / 2;
            }
            else if (intROIIndex == 2)
            {
                // ------------- Bottom -------------------------------------

                m_intEdgeROICenterX[2] = UnitCenterX;
                m_intEdgeROICenterY[2] = UnitCenterY;
                m_arrEdgeROI[2].ref_ROIPositionX = m_intEdgeROICenterX[2] - m_arrEdgeROI[2].ref_ROIWidth / 2;
                m_arrEdgeROI[2].ref_ROIPositionY = m_intEdgeROICenterY[2] - m_arrEdgeROI[2].ref_ROIHeight / 2;
            }
            else if (intROIIndex == 3)
            {
                // ------------- Left -------------------------------------

                m_intEdgeROICenterX[3] = UnitCenterX;
                m_intEdgeROICenterY[3] = UnitCenterY;
                m_arrEdgeROI[3].ref_ROIPositionX = m_intEdgeROICenterX[3] - m_arrEdgeROI[3].ref_ROIWidth / 2;
                m_arrEdgeROI[3].ref_ROIPositionY = m_intEdgeROICenterY[3] - m_arrEdgeROI[3].ref_ROIHeight / 2;
            }
        }
        public void SetEdgeROIPlacement(int intOffsetX, int intOffsetY)
        {
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                m_arrEdgeROI[i].ref_ROIPositionX += intOffsetX;
                m_arrEdgeROI[i].ref_ROIPositionY += intOffsetY;
            }
        }

        public bool GetEdgeROIHandle()
        {
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                if (m_arrEdgeROI[i].GetROIHandle())
                    return true;
            }
            return false;
        }

        public void AddDontCareROI(Point pRectStart, Point pRectStop)
        {
            int intOrgX = Math.Min(pRectStart.X, pRectStop.X);
            int intOrgY = Math.Min(pRectStart.Y, pRectStop.Y);
            int intWidth = Math.Abs(pRectStop.X - pRectStart.X);
            int intHeight = Math.Abs(pRectStop.Y - pRectStart.Y);

            if (m_intSelectedEdgeROIPrev == 0 || m_intSelectedEdgeROIPrev == 2)
            {
                if (m_arrEdgeROI[m_intSelectedEdgeROIPrev].ref_ROIPositionX > intOrgX)
                    intOrgX = 0;
                else
                    intOrgX = intOrgX - m_arrEdgeROI[m_intSelectedEdgeROIPrev].ref_ROIPositionX;

                intOrgY = 0;

                if (intWidth > m_arrEdgeROI[m_intSelectedEdgeROIPrev].ref_ROIWidth)
                    intWidth = m_arrEdgeROI[m_intSelectedEdgeROIPrev].ref_ROIWidth;

                intHeight = m_arrEdgeROI[m_intSelectedEdgeROIPrev].ref_ROIHeight;
            }
            else
            {
                intOrgX = 0;

                if (m_arrEdgeROI[m_intSelectedEdgeROIPrev].ref_ROIPositionY > intOrgY)
                    intOrgY = 0;
                else
                    intOrgY = intOrgY - m_arrEdgeROI[m_intSelectedEdgeROIPrev].ref_ROIPositionY;


                intWidth = m_arrEdgeROI[m_intSelectedEdgeROIPrev].ref_ROIWidth;

                if (intHeight > m_arrEdgeROI[m_intSelectedEdgeROIPrev].ref_ROIHeight)
                    intHeight = m_arrEdgeROI[m_intSelectedEdgeROIPrev].ref_ROIHeight;
            }

            m_arrDontCareROI[m_intSelectedEdgeROIPrev].Add(new ROI());
            m_arrDontCareROI[m_intSelectedEdgeROIPrev][m_arrDontCareROI[m_intSelectedEdgeROIPrev].Count - 1].LoadROISetting(intOrgX, intOrgY, intWidth, intHeight);
            m_arrDontCareROI[m_intSelectedEdgeROIPrev][m_arrDontCareROI[m_intSelectedEdgeROIPrev].Count - 1].AttachImage(m_arrEdgeROI[m_intSelectedEdgeROIPrev]);
        }
        public void AddDontCareROI(int intEdgeIndex, int intDontCareCount, List<int> intOrgX, List<int> intOrgY, List<int> intWidth, List<int> intHeight)
        {
            if (m_arrDontCareROI.Count > intEdgeIndex)
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                HiPerfTimer timeout = new HiPerfTimer();
                timeout.Start();

                while (m_arrDontCareROI[intEdgeIndex].Count < intDontCareCount)
                {
                    // ------------------- checking loop timeout ---------------------------------------------------
                    if (timeout.Timing > 10000)
                    {
                        STTrackLog.WriteLine(">>>>>>>>>>>>> time out 3011");
                        break;
                    }
                    // ---------------------------------------------------------------------------------------------

                    int intCurrentCount = m_arrDontCareROI[intEdgeIndex].Count;
                    m_arrDontCareROI[intEdgeIndex].Add(new ROI());
                    m_arrDontCareROI[intEdgeIndex][m_arrDontCareROI[intEdgeIndex].Count - 1].LoadROISetting(intOrgX[intCurrentCount], intOrgY[intCurrentCount], intWidth[intCurrentCount], intHeight[intCurrentCount]);
                    m_arrDontCareROI[intEdgeIndex][m_arrDontCareROI[intEdgeIndex].Count - 1].AttachImage(m_arrEdgeROI[intEdgeIndex]);
                }
                timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------
            }
        }
        public void RemoveDontCareROI()
        {
            if (m_arrDontCareROI[m_intSelectedEdgeROIPrev].Count > 0)
                m_arrDontCareROI[m_intSelectedEdgeROIPrev].Remove(m_arrDontCareROI[m_intSelectedEdgeROIPrev][m_arrDontCareROI[m_intSelectedEdgeROIPrev].Count - 1]);
        }
        public void RemoveDontCareROI(int intEdgeIndex, int intDontCareCount)
        {
            if (m_arrDontCareROI.Count > intEdgeIndex)
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                HiPerfTimer timeout = new HiPerfTimer();
                timeout.Start();

                while (m_arrDontCareROI[intEdgeIndex].Count > intDontCareCount)
                {
                    // ------------------- checking loop timeout ---------------------------------------------------
                    if (timeout.Timing > 10000)
                    {
                        STTrackLog.WriteLine(">>>>>>>>>>>>> time out 3012");
                        break;
                    }
                    // ---------------------------------------------------------------------------------------------

                    if (m_arrDontCareROI[intEdgeIndex].Count > 0)
                        m_arrDontCareROI[intEdgeIndex].Remove(m_arrDontCareROI[intEdgeIndex][m_arrDontCareROI[intEdgeIndex].Count - 1]);
                }

                timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------
            }
        }
        public List<List<int>> GetCurrentDontCareStartX()
        {
            List<List<int>> arrDontCareStartX = new List<List<int>>();
            for (int i = 0; i < m_arrDontCareROI.Count; i++)
            {
                arrDontCareStartX.Add(new List<int>());
                for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
                {
                    arrDontCareStartX[i].Add(m_arrDontCareROI[i][j].ref_ROIPositionX);
                }
            }
            return arrDontCareStartX;
        }
        public List<List<int>> GetCurrentDontCareStartY()
        {
            List<List<int>> arrDontCareStartY = new List<List<int>>();
            for (int i = 0; i < m_arrDontCareROI.Count; i++)
            {
                arrDontCareStartY.Add(new List<int>());
                for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
                {
                    arrDontCareStartY[i].Add(m_arrDontCareROI[i][j].ref_ROIPositionY);
                }
            }
            return arrDontCareStartY;
        }
        public List<List<int>> GetCurrentDontCareWidth()
        {
            List<List<int>> arrDontCareWidth = new List<List<int>>();
            for (int i = 0; i < m_arrDontCareROI.Count; i++)
            {
                arrDontCareWidth.Add(new List<int>());
                for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
                {
                    arrDontCareWidth[i].Add(m_arrDontCareROI[i][j].ref_ROIWidth);
                }
            }
            return arrDontCareWidth;
        }
        public List<List<int>> GetCurrentDontCareHeight()
        {
            List<List<int>> arrDontCareHeight = new List<List<int>>();
            for (int i = 0; i < m_arrDontCareROI.Count; i++)
            {
                arrDontCareHeight.Add(new List<int>());
                for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
                {
                    arrDontCareHeight[i].Add(m_arrDontCareROI[i][j].ref_ROIHeight);
                }
            }
            return arrDontCareHeight;
        }
        public List<int> GetCurrentDontCareCount()
        {
            List<int> arrDontCareCount = new List<int>();
            for (int i = 0; i < m_arrDontCareROI.Count; i++)
            {
                arrDontCareCount.Add(m_arrDontCareROI[i].Count);
            }
            return arrDontCareCount;
        }
        
        public bool GetCurrentAutoDontCareEnabled(int intPadIndex)
        {
            if (m_arrAutoDontCare.Count > GetLineGaugeSelectedIndex(intPadIndex))
            {
                return m_arrAutoDontCare[GetLineGaugeSelectedIndex(intPadIndex)];
            }
            return false;
        }
        public bool GetAutoDontCareEnabled(int intROIIndex)
        {
            if (m_arrAutoDontCare.Count > intROIIndex)
            {
                return m_arrAutoDontCare[intROIIndex];
            }
            return false;
        }
        public List<bool> GetCurrentAutoDontCareEnabled()
        {
            List<bool> arrAutoDontCareEnabled = new List<bool>();
            for (int i = 0; i < m_arrAutoDontCare.Count; i++)
            {
                arrAutoDontCareEnabled.Add(m_arrAutoDontCare[i]);
            }
            return arrAutoDontCareEnabled;
        }
        public void SetCurrentAutoDontCareEnabled(bool blnEnabled, int intPadIndex)
        {
            if (m_arrAutoDontCare.Count > GetLineGaugeSelectedIndex(intPadIndex))
            {
                m_arrAutoDontCare[GetLineGaugeSelectedIndex(intPadIndex)] = blnEnabled;
            }
        }
        public void SetCurrentAutoDontCareEnabled(List<bool> arrAutoDontCare)
        {
            for (int i = 0; i < arrAutoDontCare.Count; i++)
            {
                m_arrAutoDontCare[i] = arrAutoDontCare[i];
            }
        }
        public int GetCurrentAutoDontCareOffset(int intPadIndex)
        {
            if (m_arrAutoDontCareOffset.Count > GetLineGaugeSelectedIndex(intPadIndex))
            {
                return m_arrAutoDontCareOffset[GetLineGaugeSelectedIndex(intPadIndex)];
            }
            return 0;
        }
        public List<int> GetCurrentAutoDontCareOffset()
        {
            List<int> arrAutoDontCareOffset = new List<int>();
            for (int i = 0; i < m_arrAutoDontCareOffset.Count; i++)
            {
                arrAutoDontCareOffset.Add(m_arrAutoDontCareOffset[i]);
            }
            return arrAutoDontCareOffset;
        }
        public void SetCurrentAutoDontCareOffset(int intOffset, int intPadIndex)
        {
            if (m_arrAutoDontCareOffset.Count > GetLineGaugeSelectedIndex(intPadIndex))
            {
                m_arrAutoDontCareOffset[GetLineGaugeSelectedIndex(intPadIndex)] = intOffset;
            }
        }
        public void SetCurrentAutoDontCareOffset(List<int> arrAutoDontCareOffset)
        {
            for (int i = 0; i < arrAutoDontCareOffset.Count; i++)
            {
                m_arrAutoDontCareOffset[i] = arrAutoDontCareOffset[i];
            }
        }
        public int GetCurrentAutoDontCareThreshold(int intPadIndex)
        {
            if (m_arrAutoDontCareThreshold.Count > GetLineGaugeSelectedIndex(intPadIndex))
            {
                return m_arrAutoDontCareThreshold[GetLineGaugeSelectedIndex(intPadIndex)];
            }
            return 0;
        }
        public List<int> GetCurrentAutoDontCareThreshold()
        {
            List<int> arrAutoDontCareThreshold = new List<int>();
            for (int i = 0; i < m_arrAutoDontCareThreshold.Count; i++)
            {
                arrAutoDontCareThreshold.Add(m_arrAutoDontCareThreshold[i]);
            }
            return arrAutoDontCareThreshold;
        }
        public void SetCurrentAutoDontCareThreshold(int intThreshold, int intPadIndex)
        {
            if (m_arrAutoDontCareThreshold.Count > GetLineGaugeSelectedIndex(intPadIndex))
            {
                m_arrAutoDontCareThreshold[GetLineGaugeSelectedIndex(intPadIndex)] = intThreshold;
            }
        }
        public void SetCurrentAutoDontCareThreshold(List<int> arrAutoDontCareThreshold)
        {
            for (int i = 0; i < arrAutoDontCareThreshold.Count; i++)
            {
                m_arrAutoDontCareThreshold[i] = arrAutoDontCareThreshold[i];
            }
        }
        private int GetLineGaugeSelectedIndex(int intPadIndex)
        {
            switch (intPadIndex)
            {
                case 0:
                case 1: // Top ROI
                    {
                        if (m_intSelectedEdgeROIPrev == 0)
                            return 0;
                        else if (m_intSelectedEdgeROIPrev == 1)
                            return 1;
                        else if (m_intSelectedEdgeROIPrev == 2)
                            return 2;
                        else if (m_intSelectedEdgeROIPrev == 3)
                            return 3;
                    }
                    break;
                case 2: // Right ROI
                    {
                        if (m_intSelectedEdgeROIPrev == 0)
                            return 1;
                        else if (m_intSelectedEdgeROIPrev == 1)
                            return 2;
                        else if (m_intSelectedEdgeROIPrev == 2)
                            return 3;
                        else if (m_intSelectedEdgeROIPrev == 3)
                            return 0;
                    }
                    break;
                case 3: // Bottom ROI
                    {
                        if (m_intSelectedEdgeROIPrev == 0)
                            return 2;
                        else if (m_intSelectedEdgeROIPrev == 1)
                            return 3;
                        else if (m_intSelectedEdgeROIPrev == 2)
                            return 0;
                        else if (m_intSelectedEdgeROIPrev == 3)
                            return 1;
                    }
                    break;
                case 4: // Left ROI
                    {
                        if (m_intSelectedEdgeROIPrev == 0)
                            return 3;
                        else if (m_intSelectedEdgeROIPrev == 1)
                            return 0;
                        else if (m_intSelectedEdgeROIPrev == 2)
                            return 1;
                        else if (m_intSelectedEdgeROIPrev == 3)
                            return 2;
                    }
                    break;
            }

            return 0;
        }
        private int GetLineGaugeSelectedIndex(int intPadIndex, int intROIIndex)
        {
            switch (intPadIndex)
            {
                case 0:
                case 1: // Top ROI
                    {
                        if (intROIIndex == 0)
                            return 0;
                        else if (intROIIndex == 1)
                            return 1;
                        else if (intROIIndex == 2)
                            return 2;
                        else if (intROIIndex == 3)
                            return 3;
                    }
                    break;
                case 2: // Right ROI
                    {
                        if (intROIIndex == 0)
                            return 1;
                        else if (intROIIndex == 1)
                            return 2;
                        else if (intROIIndex == 2)
                            return 3;
                        else if (intROIIndex == 3)
                            return 0;
                    }
                    break;
                case 3: // Bottom ROI
                    {
                        if (intROIIndex == 0)
                            return 2;
                        else if (intROIIndex == 1)
                            return 3;
                        else if (intROIIndex == 2)
                            return 0;
                        else if (intROIIndex == 3)
                            return 1;
                    }
                    break;
                case 4: // Left ROI
                    {
                        if (intROIIndex == 0)
                            return 3;
                        else if (intROIIndex == 1)
                            return 0;
                        else if (intROIIndex == 2)
                            return 1;
                        else if (intROIIndex == 3)
                            return 2;
                    }
                    break;
            }

            return 0;
        }
        public void ModifyDontCareImage(ImageDrawing objImage, ImageDrawing objWhiteImage)
        {
            for (int i = 0; i < m_arrDontCareROI.Count; i++)
            {
                for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
                {
                    if (i == 0 || i == 2)
                    {
                        m_arrDontCareROI[i][j].ref_ROIHeight = m_arrEdgeROI[i].ref_ROIHeight;
                    }
                    else
                    {
                        m_arrDontCareROI[i][j].ref_ROIWidth = m_arrEdgeROI[i].ref_ROIWidth;
                    }

                    ROI objEdgeROI = new ROI();
                    objEdgeROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX, m_arrEdgeROI[i].ref_ROIPositionY,
                        m_arrEdgeROI[i].ref_ROIWidth, m_arrEdgeROI[i].ref_ROIHeight);
                    objEdgeROI.AttachImage(objImage);

                    ROI objDontCareROI = new ROI();
                    objDontCareROI.LoadROISetting(m_arrDontCareROI[i][j].ref_ROIPositionX, m_arrDontCareROI[i][j].ref_ROIPositionY,
                        m_arrDontCareROI[i][j].ref_ROIWidth, m_arrDontCareROI[i][j].ref_ROIHeight);
                    objDontCareROI.AttachImage(objEdgeROI);

                    ROI objROI = new ROI();
                    objROI.LoadROISetting(m_arrDontCareROI[i][j].ref_ROIPositionX, m_arrDontCareROI[i][j].ref_ROIPositionY,
                        m_arrDontCareROI[i][j].ref_ROIWidth, m_arrDontCareROI[i][j].ref_ROIHeight);
                    objROI.AttachImage(objWhiteImage);

                    ROI.SubtractROI(objDontCareROI, objROI);
                    objDontCareROI.Dispose();
                    objEdgeROI.Dispose();
                    objROI.Dispose();
                }
            }
        }

        public void ModifyDontCareImage(List<ImageDrawing> arrImage, ImageDrawing objWhiteImage)
        {
            for (int i = 0; i < m_arrDontCareROI.Count; i++)
            {
                for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
                {
                    if (i == 0 || i == 2)
                    {
                        m_arrDontCareROI[i][j].ref_ROIHeight = m_arrEdgeROI[i].ref_ROIHeight;
                    }
                    else
                    {
                        m_arrDontCareROI[i][j].ref_ROIWidth = m_arrEdgeROI[i].ref_ROIWidth;
                    }

                    ROI objEdgeROI = new ROI();
                    objEdgeROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX, m_arrEdgeROI[i].ref_ROIPositionY,
                        m_arrEdgeROI[i].ref_ROIWidth, m_arrEdgeROI[i].ref_ROIHeight);

                    int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                    if (intImageIndex >= arrImage.Count)
                        intImageIndex = 0;

                    objEdgeROI.AttachImage(arrImage[intImageIndex]); // 2020-01-06 ZJYEOH : Cannot directly use m_arrGaugeImageNo[i] as Image number , need to see which merge type is currently using
                    //objEdgeROI.AttachImage(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)]); // 2020-01-06 ZJYEOH : Cannot directly use m_arrGaugeImageNo[i] as Image number , need to see which merge type is currently using

                    ROI objDontCareROI = new ROI();
                    objDontCareROI.LoadROISetting(m_arrDontCareROI[i][j].ref_ROIPositionX, m_arrDontCareROI[i][j].ref_ROIPositionY,
                        m_arrDontCareROI[i][j].ref_ROIWidth, m_arrDontCareROI[i][j].ref_ROIHeight);
                    objDontCareROI.AttachImage(objEdgeROI);

                    ROI objROI = new ROI();
                    objROI.LoadROISetting(m_arrDontCareROI[i][j].ref_ROIPositionX, m_arrDontCareROI[i][j].ref_ROIPositionY,
                        m_arrDontCareROI[i][j].ref_ROIWidth, m_arrDontCareROI[i][j].ref_ROIHeight);
                    objROI.AttachImage(objWhiteImage);

                    ROI.SubtractROI(objDontCareROI, objROI);
                    objDontCareROI.Dispose();
                    objEdgeROI.Dispose();
                    objROI.Dispose();
                }
            }
        }
        public void ModifyDontCareImage(List<List<ImageDrawing>> arrImage, ImageDrawing objWhiteImage, int intPadIndex)
        {
            for (int i = 0; i < m_arrDontCareROI.Count; i++)
            {
                int intSelectedLine = GetLineGaugeSelectedIndex(intPadIndex, i);
                if (intSelectedLine >= m_arrAutoDontCare.Count)
                    continue;

                if (!m_arrAutoDontCare[intSelectedLine])
                {
                    for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
                    {
                        if (i == 0 || i == 2)
                        {
                            m_arrDontCareROI[i][j].ref_ROIHeight = m_arrEdgeROI[i].ref_ROIHeight;
                        }
                        else
                        {
                            m_arrDontCareROI[i][j].ref_ROIWidth = m_arrEdgeROI[i].ref_ROIWidth;
                        }

                        ROI objEdgeROI = new ROI();
                        objEdgeROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX, m_arrEdgeROI[i].ref_ROIPositionY,
                            m_arrEdgeROI[i].ref_ROIWidth, m_arrEdgeROI[i].ref_ROIHeight);

                        int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                        if (intImageIndex >= arrImage.Count)
                            intImageIndex = 0;

                        //objEdgeROI.AttachImage(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]); // 2020-01-06 ZJYEOH : Cannot directly use m_arrGaugeImageNo[i] as Image number , need to see which merge type is currently using
                        objEdgeROI.AttachImage(arrImage[intImageIndex][i]); // 2020-01-06 ZJYEOH : Cannot directly use m_arrGaugeImageNo[i] as Image number , need to see which merge type is currently using

                        ROI objDontCareROI = new ROI();
                        objDontCareROI.LoadROISetting(m_arrDontCareROI[i][j].ref_ROIPositionX, m_arrDontCareROI[i][j].ref_ROIPositionY,
                            m_arrDontCareROI[i][j].ref_ROIWidth, m_arrDontCareROI[i][j].ref_ROIHeight);
                        objDontCareROI.AttachImage(objEdgeROI);

                        ROI objROI = new ROI();
                        objROI.LoadROISetting(m_arrDontCareROI[i][j].ref_ROIPositionX, m_arrDontCareROI[i][j].ref_ROIPositionY,
                            m_arrDontCareROI[i][j].ref_ROIWidth, m_arrDontCareROI[i][j].ref_ROIHeight);
                        objROI.AttachImage(objWhiteImage);

                        ROI.SubtractROI(objDontCareROI, objROI);
                        objDontCareROI.Dispose();
                        objEdgeROI.Dispose();
                        objROI.Dispose();
                    }
                }
                else
                {
                    ROI objEdgeROI = new ROI();
                    ROI objDontCareROI = new ROI();
                    ROI objROI = new ROI();
                    
                        objEdgeROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX, m_arrEdgeROI[i].ref_ROIPositionY,
                            m_arrEdgeROI[i].ref_ROIWidth, m_arrEdgeROI[i].ref_ROIHeight);

                        int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                        if (intImageIndex >= arrImage.Count)
                            intImageIndex = 0;

                        //objEdgeROI.AttachImage(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]); // 2020-01-06 ZJYEOH : Cannot directly use m_arrGaugeImageNo[i] as Image number , need to see which merge type is currently using
                        objEdgeROI.AttachImage(arrImage[intImageIndex][i]); // 2020-01-06 ZJYEOH : Cannot directly use m_arrGaugeImageNo[i] as Image number , need to see which merge type is currently using
                                                                            //objEdgeROI.SaveImage("D:\\m_arrEdgeROI"+i.ToString()+".bmp");
                    m_arrBlob[i].BuildObjects_Filter_GetElement(objEdgeROI, false, true, 0, m_arrAutoDontCareThreshold[intSelectedLine],
                        20, objEdgeROI.ref_ROIWidth * objEdgeROI.ref_ROIHeight, false, 0xFF);

                        if (m_arrBlob[i].ref_intNumSelectedObject > 0)
                        {
                            int intIndex = 0;

                            if (i == 0)
                            {
                            if (m_arrAutoDontCareOffset[intSelectedLine] >= 0)
                            {
                                intIndex = m_arrBlob[i].GetTopBlob();

                                objEdgeROI.LoadROISetting(/*Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] - m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionX/*)*/,
                                    Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] - m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionY),
                            /*Math.Min((int)(m_arrBlob[i].ref_arrWidth[intIndex] + 2), */objEdgeROI.ref_ROIWidth/*)*/, Math.Min(m_arrAutoDontCareOffset[intSelectedLine] + 2, objEdgeROI.ref_ROIHeight));
                                //objEdgeROI.SaveImage("D:\\objEdgeROI_Top.bmp");
                            }
                            else
                            {
                                intIndex = m_arrBlob[i].GetBottomBlob();

                                objEdgeROI.LoadROISetting(/*Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] - m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionX/*)*/,
                                    Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] + m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1 + m_arrAutoDontCareOffset[intSelectedLine], m_arrEdgeROI[i].ref_ROIPositionY),
                            /*Math.Min((int)(m_arrBlob[i].ref_arrWidth[intIndex] + 2), */objEdgeROI.ref_ROIWidth/*)*/, Math.Min(Math.Abs(m_arrAutoDontCareOffset[intSelectedLine]) + 2, objEdgeROI.ref_ROIHeight));
                                //objEdgeROI.SaveImage("D:\\objEdgeROI_Top.bmp");
                            }

                            m_arrBlob[i].BuildObjects_Filter_GetElement(objEdgeROI, false, true, 0, m_arrAutoDontCareThreshold[intSelectedLine],
                        5, objEdgeROI.ref_ROIWidth * objEdgeROI.ref_ROIHeight, false, 0xFF);

                                for (int j = 0; j < m_arrBlob[i].ref_intNumSelectedObject; j++)
                                {
                                    objDontCareROI.AttachImage(arrImage[intImageIndex][i]);
                                    objDontCareROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[j] - m_arrBlob[i].ref_arrWidth[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionX),
                                    m_arrEdgeROI[i].ref_ROIPositionY,
                                        Math.Min((int)(m_arrBlob[i].ref_arrWidth[j] + 2), m_arrEdgeROI[i].ref_ROIWidth), m_arrEdgeROI[i].ref_ROIHeight);


                                    //objDontCareROI.SaveImage("D:\\objDontCareROI_Top.bmp");
                                    objROI.AttachImage(objWhiteImage);
                                    objROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[j] - m_arrBlob[i].ref_arrWidth[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionX),
                                    m_arrEdgeROI[i].ref_ROIPositionY,
                                        Math.Min((int)(m_arrBlob[i].ref_arrWidth[j] + 2), m_arrEdgeROI[i].ref_ROIWidth), m_arrEdgeROI[i].ref_ROIHeight);


                                    ROI.SubtractROI(objDontCareROI, objROI);
                                    //arrImage[intImageIndex][i].SaveImage("D:\\FinalImg_Top.bmp");
                                }
                            }
                            else if (i == 1)
                            {
                            if (m_arrAutoDontCareOffset[intSelectedLine] >= 0)
                            {
                                intIndex = m_arrBlob[i].GetRightBlob();

                                objEdgeROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] + m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1 /*- 10*/, m_arrEdgeROI[i].ref_ROIPositionX),
                                 /*  Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] - m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionY/*)*/,
                           Math.Min((int)(m_arrAutoDontCareOffset[intSelectedLine] + 2), objEdgeROI.ref_ROIWidth), /*Math.Min((int)(m_arrBlob[i].ref_arrHeight[intIndex] + 2),*/ objEdgeROI.ref_ROIHeight/*)*/);
                                //objEdgeROI.SaveImage("D:\\objEdgeROI_Right.bmp");
                            }
                            else
                            {
                                intIndex = m_arrBlob[i].GetLeftBlob();

                                objEdgeROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] - m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1 , m_arrEdgeROI[i].ref_ROIPositionX),
                                 /*  Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] - m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionY/*)*/,
                           Math.Min((int)(Math.Abs(m_arrAutoDontCareOffset[intSelectedLine]) + 2), objEdgeROI.ref_ROIWidth), /*Math.Min((int)(m_arrBlob[i].ref_arrHeight[intIndex] + 2),*/ objEdgeROI.ref_ROIHeight/*)*/);
                                //objEdgeROI.SaveImage("D:\\objEdgeROI_Right.bmp");
                            }

                            m_arrBlob[i].BuildObjects_Filter_GetElement(objEdgeROI, false, true, 0, m_arrAutoDontCareThreshold[intSelectedLine],
                        5, objEdgeROI.ref_ROIWidth * objEdgeROI.ref_ROIHeight, false, 0xFF);

                                for (int j = 0; j < m_arrBlob[i].ref_intNumSelectedObject; j++)
                                {
                                    objDontCareROI.AttachImage(arrImage[intImageIndex][i]);
                                    objDontCareROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX,
                                        Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[j] - m_arrBlob[i].ref_arrHeight[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionY),
                                        m_arrEdgeROI[i].ref_ROIWidth, Math.Min((int)(m_arrBlob[i].ref_arrHeight[j] + 2), m_arrEdgeROI[i].ref_ROIHeight));


                                    //objDontCareROI.SaveImage("D:\\objDontCareROI_Right.bmp");
                                    objROI.AttachImage(objWhiteImage);
                                    objROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX,
                                        Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[j] - m_arrBlob[i].ref_arrHeight[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionY),
                                        m_arrEdgeROI[i].ref_ROIWidth, Math.Min((int)(m_arrBlob[i].ref_arrHeight[j] + 2), m_arrEdgeROI[i].ref_ROIHeight));


                                    ROI.SubtractROI(objDontCareROI, objROI);
                                    //arrImage[intImageIndex][i].SaveImage("D:\\FinalImg_Right.bmp");
                                }
                            }
                            else if (i == 2)
                        {
                            if (m_arrAutoDontCareOffset[intSelectedLine] >= 0)
                            {
                                intIndex = m_arrBlob[i].GetBottomBlob();

                                objEdgeROI.LoadROISetting(/*Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] - m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionX/*)*/,
                                  Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] + m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1 - m_arrAutoDontCareOffset[intSelectedLine], m_arrEdgeROI[i].ref_ROIPositionY),
                          /*Math.Min((int)(m_arrBlob[i].ref_arrWidth[intIndex] + 2),*/ objEdgeROI.ref_ROIWidth/*)*/, Math.Min(m_arrAutoDontCareOffset[intSelectedLine] + 2, objEdgeROI.ref_ROIHeight));
                                //objEdgeROI.SaveImage("D:\\objEdgeROI_Bottom.bmp");
                            }
                            else
                            {
                                intIndex = m_arrBlob[i].GetTopBlob();

                                objEdgeROI.LoadROISetting(/*Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] - m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionX/*)*/,
                                  Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] - m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionY),
                          /*Math.Min((int)(m_arrBlob[i].ref_arrWidth[intIndex] + 2),*/ objEdgeROI.ref_ROIWidth/*)*/, Math.Min(Math.Abs(m_arrAutoDontCareOffset[intSelectedLine]) + 2, objEdgeROI.ref_ROIHeight));
                                //objEdgeROI.SaveImage("D:\\objEdgeROI_Bottom.bmp");
                            }

                            m_arrBlob[i].BuildObjects_Filter_GetElement(objEdgeROI, false, true, 0, m_arrAutoDontCareThreshold[intSelectedLine],
                        5, objEdgeROI.ref_ROIWidth * objEdgeROI.ref_ROIHeight, false, 0xFF);

                                for (int j = 0; j < m_arrBlob[i].ref_intNumSelectedObject; j++)
                                {
                                    objDontCareROI.AttachImage(arrImage[intImageIndex][i]);
                                    objDontCareROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[j] - m_arrBlob[i].ref_arrWidth[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionX),
                                    m_arrEdgeROI[i].ref_ROIPositionY,
                                        Math.Min((int)(m_arrBlob[i].ref_arrWidth[j] + 2), m_arrEdgeROI[i].ref_ROIWidth), m_arrEdgeROI[i].ref_ROIHeight);


                                    //objDontCareROI.SaveImage("D:\\objDontCareROI_Bottom.bmp");
                                    objROI.AttachImage(objWhiteImage);
                                    objROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[j] - m_arrBlob[i].ref_arrWidth[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionX),
                                    m_arrEdgeROI[i].ref_ROIPositionY,
                                        Math.Min((int)(m_arrBlob[i].ref_arrWidth[j] + 2), m_arrEdgeROI[i].ref_ROIWidth), m_arrEdgeROI[i].ref_ROIHeight);


                                    ROI.SubtractROI(objDontCareROI, objROI);
                                    //arrImage[intImageIndex][i].SaveImage("D:\\FinalImg_Bottom.bmp");
                                }
                            }
                            else if (i == 3)
                        {
                            if (m_arrAutoDontCareOffset[intSelectedLine] >= 0)
                            {
                                intIndex = m_arrBlob[i].GetLeftBlob();

                                objEdgeROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] - m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionX),
                                 /*  Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] - m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionY/*)*/,
                           Math.Min((int)(m_arrAutoDontCareOffset[intSelectedLine] + 2), objEdgeROI.ref_ROIWidth), /*Math.Min((int)(m_arrBlob[i].ref_arrHeight[intIndex] + 2),*/ objEdgeROI.ref_ROIHeight/*)*/);
                                //objEdgeROI.SaveImage("D:\\objEdgeROI_Left.bmp");
                            }
                            else
                            {
                                intIndex = m_arrBlob[i].GetRightBlob();

                                objEdgeROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] + m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1 + m_arrAutoDontCareOffset[intSelectedLine], m_arrEdgeROI[i].ref_ROIPositionX),
                                 /*  Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] - m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionY/*)*/,
                           Math.Min((int)(Math.Abs(m_arrAutoDontCareOffset[intSelectedLine]) + 2), objEdgeROI.ref_ROIWidth), /*Math.Min((int)(m_arrBlob[i].ref_arrHeight[intIndex] + 2),*/ objEdgeROI.ref_ROIHeight/*)*/);
                                //objEdgeROI.SaveImage("D:\\objEdgeROI_Left.bmp");
                            }

                            m_arrBlob[i].BuildObjects_Filter_GetElement(objEdgeROI, false, true, 0, m_arrAutoDontCareThreshold[intSelectedLine],
                        5, objEdgeROI.ref_ROIWidth * objEdgeROI.ref_ROIHeight, false, 0xFF);

                                for (int j = 0; j < m_arrBlob[i].ref_intNumSelectedObject; j++)
                                {
                                    objDontCareROI.AttachImage(arrImage[intImageIndex][i]);
                                    objDontCareROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX,
                                        Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[j] - m_arrBlob[i].ref_arrHeight[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionY),
                                        m_arrEdgeROI[i].ref_ROIWidth, Math.Min((int)(m_arrBlob[i].ref_arrHeight[j] + 2), m_arrEdgeROI[i].ref_ROIHeight));


                                    //objDontCareROI.SaveImage("D:\\objDontCareROI_Left.bmp");
                                    objROI.AttachImage(objWhiteImage);
                                    objROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX,
                                        Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[j] - m_arrBlob[i].ref_arrHeight[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionY),
                                        m_arrEdgeROI[i].ref_ROIWidth, Math.Min((int)(m_arrBlob[i].ref_arrHeight[j] + 2), m_arrEdgeROI[i].ref_ROIHeight));


                                    ROI.SubtractROI(objDontCareROI, objROI);
                                    //arrImage[intImageIndex][i].SaveImage("D:\\FinalImg_Left.bmp");
                                }
                            }

                        }
                        
                    objDontCareROI.Dispose();
                    objEdgeROI.Dispose();
                    objROI.Dispose();
                }
            }
        }
        public void ModifyAutoDontCareImage(List<List<ImageDrawing>> arrImage, ImageDrawing objWhiteImage, ImageDrawing[] objPreImage, int intPadIndex)
        {
            ROI objEdgeROI = new ROI();
            ROI objDontCareROI = new ROI();
            ROI objROI = new ROI();
            for (int i = 0; i < m_arrAutoDontCare.Count; i++)
            {
                int intSelectedLine = GetLineGaugeSelectedIndex(intPadIndex, i);
                if (intSelectedLine >= m_arrAutoDontCare.Count)
                    continue;

                m_arrAutoDontCareTotalLength[intSelectedLine] = 0;
                if (!m_arrAutoDontCare[intSelectedLine])
                    continue;

                objEdgeROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX, m_arrEdgeROI[i].ref_ROIPositionY,
                    m_arrEdgeROI[i].ref_ROIWidth, m_arrEdgeROI[i].ref_ROIHeight);

                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intImageIndex >= arrImage.Count)
                    intImageIndex = 0;

                //objEdgeROI.AttachImage(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]); // 2020-01-06 ZJYEOH : Cannot directly use m_arrGaugeImageNo[i] as Image number , need to see which merge type is currently using
                objEdgeROI.AttachImage(objPreImage[i]); //arrImage[intImageIndex][i]// 2020-01-06 ZJYEOH : Cannot directly use m_arrGaugeImageNo[i] as Image number , need to see which merge type is currently using
                                                        //objEdgeROI.SaveImage("D:\\m_arrEdgeROI"+i.ToString()+".bmp");
                m_arrBlob[i].BuildObjects_Filter_GetElement(objEdgeROI, false, true, 0, m_arrAutoDontCareThreshold[intSelectedLine],
                20, objEdgeROI.ref_ROIWidth * objEdgeROI.ref_ROIHeight, false, 0xFF);

                if (m_arrBlob[i].ref_intNumSelectedObject > 0)
                {
                    int intIndex = 0;

                    if (i == 0)
                    {
                        if (m_arrAutoDontCareOffset[intSelectedLine] >= 0)
                        {
                            intIndex = m_arrBlob[i].GetTopBlob();

                            objEdgeROI.LoadROISetting(/*Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] - m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionX/*)*/,
                                Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] - m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionY),
                        /*Math.Min((int)(m_arrBlob[i].ref_arrWidth[intIndex] + 2), */objEdgeROI.ref_ROIWidth/*)*/, Math.Min(m_arrAutoDontCareOffset[intSelectedLine] + 2, objEdgeROI.ref_ROIHeight));
                            //objEdgeROI.SaveImage("D:\\objEdgeROI_Top" + intPadIndex + ".bmp");
                        }
                        else
                        {
                            intIndex = m_arrBlob[i].GetBottomBlob();

                            objEdgeROI.LoadROISetting(/*Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] - m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionX/*)*/,
                                Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] + m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1 + m_arrAutoDontCareOffset[intSelectedLine], m_arrEdgeROI[i].ref_ROIPositionY),
                        /*Math.Min((int)(m_arrBlob[i].ref_arrWidth[intIndex] + 2), */objEdgeROI.ref_ROIWidth/*)*/, Math.Min(Math.Abs(m_arrAutoDontCareOffset[intSelectedLine]) + 2, objEdgeROI.ref_ROIHeight));
                            //objEdgeROI.SaveImage("D:\\objEdgeROI_Top" + intPadIndex + ".bmp");
                        }

                        m_arrBlob[i].BuildObjects_Filter_GetElement(objEdgeROI, false, true, 0, m_arrAutoDontCareThreshold[intSelectedLine],
                5, objEdgeROI.ref_ROIWidth * objEdgeROI.ref_ROIHeight, false, 0xFF);

                        for (int j = 0; j < m_arrBlob[i].ref_intNumSelectedObject; j++)
                        {
                            objDontCareROI.AttachImage(arrImage[intImageIndex][i]);
                            objDontCareROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[j] - m_arrBlob[i].ref_arrWidth[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionX),
                            m_arrEdgeROI[i].ref_ROIPositionY,
                                Math.Min((int)(m_arrBlob[i].ref_arrWidth[j] + 2), m_arrEdgeROI[i].ref_ROIWidth), m_arrEdgeROI[i].ref_ROIHeight);

                            m_arrAutoDontCareTotalLength[intSelectedLine] += (int)m_arrBlob[i].ref_arrWidth[j];
                            //objDontCareROI.SaveImage("D:\\objDontCareROI_Top.bmp");
                            objROI.AttachImage(objWhiteImage);
                            objROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[j] - m_arrBlob[i].ref_arrWidth[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionX),
                            m_arrEdgeROI[i].ref_ROIPositionY,
                                Math.Min((int)(m_arrBlob[i].ref_arrWidth[j] + 2), m_arrEdgeROI[i].ref_ROIWidth), m_arrEdgeROI[i].ref_ROIHeight);


                            ROI.SubtractROI(objDontCareROI, objROI);
                            //arrImage[intImageIndex][i].SaveImage("D:\\FinalImg_Top.bmp");
                        }
                    }
                    else if (i == 1)
                    {
                        if (m_arrAutoDontCareOffset[intSelectedLine] >= 0)
                        {
                            intIndex = m_arrBlob[i].GetRightBlob();

                            objEdgeROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] + m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1 - m_arrAutoDontCareOffset[intSelectedLine], m_arrEdgeROI[i].ref_ROIPositionX),
                               /*Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] - m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionY/*)*/,
                       Math.Min((int)(m_arrAutoDontCareOffset[intSelectedLine] + 2), objEdgeROI.ref_ROIWidth), /*Math.Min((int)(m_arrBlob[i].ref_arrHeight[intIndex] + 2), */objEdgeROI.ref_ROIHeight/*)*/);
                            //objEdgeROI.SaveImage("D:\\objEdgeROI_Right" + intPadIndex + ".bmp");
                        }
                        else
                        {
                            intIndex = m_arrBlob[i].GetLeftBlob();

                            objEdgeROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] - m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionX),
                               /*Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] - m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionY/*)*/,
                       Math.Min((int)(Math.Abs(m_arrAutoDontCareOffset[intSelectedLine]) + 2), objEdgeROI.ref_ROIWidth), /*Math.Min((int)(m_arrBlob[i].ref_arrHeight[intIndex] + 2), */objEdgeROI.ref_ROIHeight/*)*/);
                            //objEdgeROI.SaveImage("D:\\objEdgeROI_Right" + intPadIndex + ".bmp");
                        }

                        m_arrBlob[i].BuildObjects_Filter_GetElement(objEdgeROI, false, true, 0, m_arrAutoDontCareThreshold[intSelectedLine],
                5, objEdgeROI.ref_ROIWidth * objEdgeROI.ref_ROIHeight, false, 0xFF);

                        for (int j = 0; j < m_arrBlob[i].ref_intNumSelectedObject; j++)
                        {
                            objDontCareROI.AttachImage(arrImage[intImageIndex][i]);
                            objDontCareROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX,
                                Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[j] - m_arrBlob[i].ref_arrHeight[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionY),
                                m_arrEdgeROI[i].ref_ROIWidth, Math.Min((int)(m_arrBlob[i].ref_arrHeight[j] + 2), m_arrEdgeROI[i].ref_ROIHeight));

                            m_arrAutoDontCareTotalLength[intSelectedLine] += (int)m_arrBlob[i].ref_arrHeight[j];
                            //objDontCareROI.SaveImage("D:\\objDontCareROI_Right.bmp");
                            objROI.AttachImage(objWhiteImage);
                            objROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX,
                                Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[j] - m_arrBlob[i].ref_arrHeight[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionY),
                                m_arrEdgeROI[i].ref_ROIWidth, Math.Min((int)(m_arrBlob[i].ref_arrHeight[j] + 2), m_arrEdgeROI[i].ref_ROIHeight));


                            ROI.SubtractROI(objDontCareROI, objROI);
                            //arrImage[intImageIndex][i].SaveImage("D:\\FinalImg_Right.bmp");
                        }
                    }
                    else if (i == 2)
                    {
                        if (m_arrAutoDontCareOffset[intSelectedLine] >= 0)
                        {
                            intIndex = m_arrBlob[i].GetBottomBlob();

                            objEdgeROI.LoadROISetting(/*Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] - m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionX/*)*/,
                              Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] + m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1 - m_arrAutoDontCareOffset[intSelectedLine], m_arrEdgeROI[i].ref_ROIPositionY),
                      /*Math.Min((int)(m_arrBlob[i].ref_arrWidth[intIndex] + 2),*/ objEdgeROI.ref_ROIWidth/*)*/, Math.Min(m_arrAutoDontCareOffset[intSelectedLine] + 2, objEdgeROI.ref_ROIHeight));
                            //objEdgeROI.SaveImage("D:\\objEdgeROI_Bottom" + intPadIndex + ".bmp");
                        }
                        else
                        {
                            intIndex = m_arrBlob[i].GetTopBlob();

                            objEdgeROI.LoadROISetting(/*Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] - m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1, */m_arrEdgeROI[i].ref_ROIPositionX/*)*/,
                              Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] - m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionY),
                      /*Math.Min((int)(m_arrBlob[i].ref_arrWidth[intIndex] + 2),*/ objEdgeROI.ref_ROIWidth/*)*/, Math.Min(Math.Abs(m_arrAutoDontCareOffset[intSelectedLine]) + 2, objEdgeROI.ref_ROIHeight));
                            //objEdgeROI.SaveImage("D:\\objEdgeROI_Bottom" + intPadIndex + ".bmp");
                        }

                        m_arrBlob[i].BuildObjects_Filter_GetElement(objEdgeROI, false, true, 0, m_arrAutoDontCareThreshold[intSelectedLine],
                5, objEdgeROI.ref_ROIWidth * objEdgeROI.ref_ROIHeight, false, 0xFF);

                        for (int j = 0; j < m_arrBlob[i].ref_intNumSelectedObject; j++)
                        {
                            objDontCareROI.AttachImage(arrImage[intImageIndex][i]);
                            objDontCareROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[j] - m_arrBlob[i].ref_arrWidth[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionX),
                            m_arrEdgeROI[i].ref_ROIPositionY,
                                Math.Min((int)(m_arrBlob[i].ref_arrWidth[j] + 2), m_arrEdgeROI[i].ref_ROIWidth), m_arrEdgeROI[i].ref_ROIHeight);

                            m_arrAutoDontCareTotalLength[intSelectedLine] += (int)m_arrBlob[i].ref_arrWidth[j];
                            //objDontCareROI.SaveImage("D:\\objDontCareROI_Bottom.bmp");
                            objROI.AttachImage(objWhiteImage);
                            objROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[j] - m_arrBlob[i].ref_arrWidth[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionX),
                            m_arrEdgeROI[i].ref_ROIPositionY,
                                Math.Min((int)(m_arrBlob[i].ref_arrWidth[j] + 2), m_arrEdgeROI[i].ref_ROIWidth), m_arrEdgeROI[i].ref_ROIHeight);


                            ROI.SubtractROI(objDontCareROI, objROI);
                            //arrImage[intImageIndex][i].SaveImage("D:\\FinalImg_Bottom.bmp");
                        }
                    }
                    else if (i == 3)
                    {
                        if (m_arrAutoDontCareOffset[intSelectedLine] >= 0)
                        {
                            intIndex = m_arrBlob[i].GetLeftBlob();

                            objEdgeROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] - m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionX),
                               /*Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] - m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1,*/ m_arrEdgeROI[i].ref_ROIPositionY/*)*/,
                       Math.Min((int)(m_arrAutoDontCareOffset[intSelectedLine] + 2), objEdgeROI.ref_ROIWidth), /*Math.Min((int)(m_arrBlob[i].ref_arrHeight[intIndex] + 2),*/ objEdgeROI.ref_ROIHeight/*)*/);
                            //objEdgeROI.SaveImage("D:\\objEdgeROI_Left" + intPadIndex + ".bmp");
                        }
                        else
                        {
                            intIndex = m_arrBlob[i].GetRightBlob();

                            objEdgeROI.LoadROISetting(Math.Max(objEdgeROI.ref_ROIPositionX + (int)(m_arrBlob[i].ref_arrLimitCenterX[intIndex] + m_arrBlob[i].ref_arrWidth[intIndex] / 2) - 1 + m_arrAutoDontCareOffset[intSelectedLine], m_arrEdgeROI[i].ref_ROIPositionX),
                               /*Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[intIndex] - m_arrBlob[i].ref_arrHeight[intIndex] / 2) - 1,*/ m_arrEdgeROI[i].ref_ROIPositionY/*)*/,
                       Math.Min((int)(Math.Abs(m_arrAutoDontCareOffset[intSelectedLine]) + 2), objEdgeROI.ref_ROIWidth), /*Math.Min((int)(m_arrBlob[i].ref_arrHeight[intIndex] + 2),*/ objEdgeROI.ref_ROIHeight/*)*/);
                            //objEdgeROI.SaveImage("D:\\objEdgeROI_Left" + intPadIndex + ".bmp");
                        }

                        m_arrBlob[i].BuildObjects_Filter_GetElement(objEdgeROI, false, true, 0, m_arrAutoDontCareThreshold[intSelectedLine],
                5, objEdgeROI.ref_ROIWidth * objEdgeROI.ref_ROIHeight, false, 0xFF);

                        for (int j = 0; j < m_arrBlob[i].ref_intNumSelectedObject; j++)
                        {
                            objDontCareROI.AttachImage(arrImage[intImageIndex][i]);
                            objDontCareROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX,
                                Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[j] - m_arrBlob[i].ref_arrHeight[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionY),
                                m_arrEdgeROI[i].ref_ROIWidth, Math.Min((int)(m_arrBlob[i].ref_arrHeight[j] + 2), m_arrEdgeROI[i].ref_ROIHeight));

                            m_arrAutoDontCareTotalLength[intSelectedLine] += (int)m_arrBlob[i].ref_arrHeight[j];
                            //objDontCareROI.SaveImage("D:\\objDontCareROI_Left.bmp");
                            objROI.AttachImage(objWhiteImage);
                            objROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX,
                                Math.Max(objEdgeROI.ref_ROIPositionY + (int)(m_arrBlob[i].ref_arrLimitCenterY[j] - m_arrBlob[i].ref_arrHeight[j] / 2) - 1, m_arrEdgeROI[i].ref_ROIPositionY),
                                m_arrEdgeROI[i].ref_ROIWidth, Math.Min((int)(m_arrBlob[i].ref_arrHeight[j] + 2), m_arrEdgeROI[i].ref_ROIHeight));


                            ROI.SubtractROI(objDontCareROI, objROI);
                            //arrImage[intImageIndex][i].SaveImage("D:\\FinalImg_Left.bmp");
                        }
                    }

                }

            }
            objDontCareROI.Dispose();
            objEdgeROI.Dispose();
            objROI.Dispose();
        }

        public void DrawAutoDontCareBlob(Graphics g, float fScaleX, float fScaleY, int intPadIndex)
        {
            for (int i = 0; i < m_arrAutoDontCare.Count; i++)
            {
                int intSelectedLine = GetLineGaugeSelectedIndex(intPadIndex, i);
                if (intSelectedLine >= m_arrAutoDontCare.Count)
                    continue;

                if (!m_arrAutoDontCare[intSelectedLine])
                    continue;

                m_arrBlob[i].DrawSelectedBlob(g, fScaleX, fScaleY, Color.Pink);
            }
        }

        public void DragEdgeROI(int intPositionX, int intPositionY, int intSelectedGaugeEdgeMask)
        {
            if (m_arrEdgeROI[0].GetROIHandle() && (intSelectedGaugeEdgeMask & (0x01 << 0)) > 0)
            {
                int intWidthPrev = m_arrEdgeROI[0].ref_ROIWidth;
                int intHeightPrev = m_arrEdgeROI[0].ref_ROIHeight;

                m_arrEdgeROI[0].DragROI(intPositionX, intPositionY);
                //m_arrEdgeROI[0].DragFixPosROI(intPositionX, intPositionY, m_intEdgeROICenterX[0], m_intEdgeROICenterY[0]);

                if (m_arrDontCareROI[0].Count > 0)
                {
                    int intMaxX = 0;
                    for (int i = 0; i < m_arrDontCareROI[0].Count; i++)
                    {
                        if (intMaxX < (m_arrDontCareROI[0][i].ref_ROIPositionX + m_arrDontCareROI[0][i].ref_ROIWidth))
                        {
                            intMaxX = (m_arrDontCareROI[0][i].ref_ROIPositionX + m_arrDontCareROI[0][i].ref_ROIWidth);
                        }
                    }

                    if (m_arrEdgeROI[0].ref_ROIPositionX < m_arrEdgeROI[3].ref_ROIPositionX)
                    {
                        m_arrEdgeROI[0].ref_ROIPositionX = m_arrEdgeROI[3].ref_ROIPositionX;
                    }

                    if ((m_arrEdgeROI[0].ref_ROIPositionX + m_arrEdgeROI[0].ref_ROIWidth) > (m_arrEdgeROI[1].ref_ROIPositionX + m_arrEdgeROI[1].ref_ROIWidth))
                    {
                        m_arrEdgeROI[0].ref_ROIPositionX = Math.Max(0, (m_arrEdgeROI[1].ref_ROIPositionX + m_arrEdgeROI[1].ref_ROIWidth) - intWidthPrev);
                        m_arrEdgeROI[0].ref_ROIWidth = intWidthPrev;
                    }
                    else if (m_arrEdgeROI[0].ref_ROIWidth < intMaxX)
                    {
                        m_arrEdgeROI[0].ref_ROIWidth = intWidthPrev;
                    }

                    if ((m_arrEdgeROI[0].ref_ROIPositionY + m_arrEdgeROI[0].ref_ROIHeight) > m_arrEdgeROI[2].ref_ROIPositionY)
                    {
                        m_arrEdgeROI[0].ref_ROIPositionY = Math.Max(0, (m_arrEdgeROI[2].ref_ROIPositionY) - intHeightPrev);
                        m_arrEdgeROI[0].ref_ROIHeight = intHeightPrev;
                    }


                }
                else
                {
                    if (m_arrEdgeROI[0].ref_ROIPositionX < m_arrEdgeROI[3].ref_ROIPositionX)
                    {
                        m_arrEdgeROI[0].ref_ROIPositionX = m_arrEdgeROI[3].ref_ROIPositionX;
                    }

                    if ((m_arrEdgeROI[0].ref_ROIPositionX + m_arrEdgeROI[0].ref_ROIWidth) > (m_arrEdgeROI[1].ref_ROIPositionX + m_arrEdgeROI[1].ref_ROIWidth))
                    {
                        m_arrEdgeROI[0].ref_ROIPositionX = Math.Max(0, (m_arrEdgeROI[1].ref_ROIPositionX + m_arrEdgeROI[1].ref_ROIWidth) - intWidthPrev);
                        m_arrEdgeROI[0].ref_ROIWidth = intWidthPrev;
                    }

                    if ((m_arrEdgeROI[0].ref_ROIPositionY + m_arrEdgeROI[0].ref_ROIHeight) > m_arrEdgeROI[2].ref_ROIPositionY)
                    {
                        m_arrEdgeROI[0].ref_ROIPositionY = Math.Max(0, (m_arrEdgeROI[2].ref_ROIPositionY) - intHeightPrev);
                        m_arrEdgeROI[0].ref_ROIHeight = intHeightPrev;
                    }
                }
            }

            if (m_arrEdgeROI[1].GetROIHandle() && (intSelectedGaugeEdgeMask & (0x01 << 1)) > 0)
            {
                int intHeightPrev = m_arrEdgeROI[1].ref_ROIHeight;

                m_arrEdgeROI[1].DragROI(intPositionX, intPositionY);
                //m_arrEdgeROI[1].DragFixPosROI(intPositionX, intPositionY, m_intEdgeROICenterX[1], m_intEdgeROICenterY[1]);

                if (m_arrEdgeROI[1].ref_ROIPositionY < m_arrEdgeROI[0].ref_ROIPositionY)
                {
                    m_arrEdgeROI[1].ref_ROIPositionY = m_arrEdgeROI[0].ref_ROIPositionY;
                }

                if ((m_arrEdgeROI[1].ref_ROIPositionY + m_arrEdgeROI[1].ref_ROIHeight) > (m_arrEdgeROI[2].ref_ROIPositionY + m_arrEdgeROI[2].ref_ROIHeight))
                {
                    m_arrEdgeROI[1].ref_ROIPositionY = Math.Max(0, (m_arrEdgeROI[2].ref_ROIPositionY + m_arrEdgeROI[2].ref_ROIHeight) - intHeightPrev);
                    m_arrEdgeROI[1].ref_ROIHeight = intHeightPrev;
                }

                if ((m_arrEdgeROI[1].ref_ROIPositionX) < (m_arrEdgeROI[3].ref_ROIPositionX + m_arrEdgeROI[3].ref_ROIWidth))
                {
                    m_arrEdgeROI[1].ref_ROIPositionX = (m_arrEdgeROI[3].ref_ROIPositionX + m_arrEdgeROI[3].ref_ROIWidth);
                }
            }


            if (m_arrEdgeROI[2].GetROIHandle() && (intSelectedGaugeEdgeMask & (0x01 << 2)) > 0)
            {
                int intWidthPrev = m_arrEdgeROI[2].ref_ROIWidth;

                m_arrEdgeROI[2].DragROI(intPositionX, intPositionY);
                //m_arrEdgeROI[2].DragFixPosROI(intPositionX, intPositionY, m_intEdgeROICenterX[2], m_intEdgeROICenterY[2]);

                if (m_arrEdgeROI[2].ref_ROIPositionX < m_arrEdgeROI[3].ref_ROIPositionX)
                {
                    m_arrEdgeROI[2].ref_ROIPositionX = m_arrEdgeROI[3].ref_ROIPositionX;
                }

                if ((m_arrEdgeROI[2].ref_ROIPositionX + m_arrEdgeROI[2].ref_ROIWidth) > (m_arrEdgeROI[1].ref_ROIPositionX + m_arrEdgeROI[1].ref_ROIWidth))
                {
                    m_arrEdgeROI[2].ref_ROIPositionX = Math.Max(0, (m_arrEdgeROI[1].ref_ROIPositionX + m_arrEdgeROI[1].ref_ROIWidth) - intWidthPrev);
                    m_arrEdgeROI[2].ref_ROIWidth = intWidthPrev;
                }

                if ((m_arrEdgeROI[2].ref_ROIPositionY) < (m_arrEdgeROI[0].ref_ROIPositionY + m_arrEdgeROI[0].ref_ROIHeight))
                {
                    m_arrEdgeROI[2].ref_ROIPositionY = (m_arrEdgeROI[0].ref_ROIPositionY + m_arrEdgeROI[0].ref_ROIHeight);
                }
            }

            if (m_arrEdgeROI[3].GetROIHandle() && (intSelectedGaugeEdgeMask & (0x01 << 3)) > 0)
            {
                int intWidthPrev = m_arrEdgeROI[3].ref_ROIWidth;
                int intHeightPrev = m_arrEdgeROI[3].ref_ROIHeight;

                m_arrEdgeROI[3].DragROI(intPositionX, intPositionY);
                //m_arrEdgeROI[3].DragFixPosROI(intPositionX, intPositionY, m_intEdgeROICenterX[3], m_intEdgeROICenterY[3]);

                if (m_arrEdgeROI[3].ref_ROIPositionY < m_arrEdgeROI[0].ref_ROIPositionY)
                {
                    m_arrEdgeROI[3].ref_ROIPositionY = m_arrEdgeROI[0].ref_ROIPositionY;
                }

                if ((m_arrEdgeROI[3].ref_ROIPositionY + m_arrEdgeROI[3].ref_ROIHeight) > (m_arrEdgeROI[2].ref_ROIPositionY + m_arrEdgeROI[2].ref_ROIHeight))
                {
                    m_arrEdgeROI[3].ref_ROIPositionY = Math.Max(0, (m_arrEdgeROI[2].ref_ROIPositionY + m_arrEdgeROI[2].ref_ROIHeight) - intHeightPrev);
                    m_arrEdgeROI[3].ref_ROIHeight = intHeightPrev;
                }

                if ((m_arrEdgeROI[3].ref_ROIPositionX + m_arrEdgeROI[3].ref_ROIWidth) > m_arrEdgeROI[1].ref_ROIPositionX)
                {
                    m_arrEdgeROI[3].ref_ROIPositionX = Math.Max(0, (m_arrEdgeROI[1].ref_ROIPositionX) - intWidthPrev);
                    m_arrEdgeROI[3].ref_ROIWidth = intWidthPrev;
                }
            }
        }

        public void DrawEdgeROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                m_arrEdgeROI[i].DrawROI(g, fDrawingScaleX, fDrawingScaleY, m_arrEdgeROI[i].GetROIHandle());
            }
        }

        //public void DrawEdgeROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intSelectedGaugeEdgeMask)
        //{
        //    for (int i = 0; i < m_arrEdgeROI.Count; i++)
        //    {
        //        if ((intSelectedGaugeEdgeMask & (0x01 << i)) > 0)
        //            m_arrEdgeROI[i].DrawROI(g, fDrawingScaleX, fDrawingScaleY, m_arrEdgeROI[i].GetROIHandle());
        //    }
        //}

        public void DrawEdgeROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intSelectedGaugeEdgeMask, Color objColor)
        {
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                if ((intSelectedGaugeEdgeMask & (0x01 << i)) > 0)
                {
                    if (i == 0)
                        m_arrEdgeROI[i].DrawROI(g, fDrawingScaleX, fDrawingScaleY, m_arrEdgeROI[i].GetROIHandle(), 0, (float)(m_arrEdgeROI[i].ref_ROIWidth - 35) / 2, -35f, objColor);
                    else if (i == 1)
                        m_arrEdgeROI[i].DrawROI(g, fDrawingScaleX, fDrawingScaleY, m_arrEdgeROI[i].GetROIHandle(), 0, m_arrEdgeROI[i].ref_ROIWidth, (float)(m_arrEdgeROI[i].ref_ROIHeight - 35) / 2, objColor);
                    else if (i == 2)
                        m_arrEdgeROI[i].DrawROI(g, fDrawingScaleX, fDrawingScaleY, m_arrEdgeROI[i].GetROIHandle(), 0, (float)(m_arrEdgeROI[i].ref_ROIWidth - 35) / 2, m_arrEdgeROI[i].ref_ROIHeight, objColor);
                    else if (i == 3)
                        m_arrEdgeROI[i].DrawROI(g, fDrawingScaleX, fDrawingScaleY, m_arrEdgeROI[i].GetROIHandle(), 0, -40f, (float)(m_arrEdgeROI[i].ref_ROIHeight - 35) / 2, objColor);
                }
            }
        }

        public void DrawDontCareArea(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            for (int i = 0; i < m_arrDontCareROI.Count; i++)
            {
                for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
                {
                    int intOrgX = (int)((m_arrEdgeROI[i].ref_ROIPositionX + m_arrDontCareROI[i][j].ref_ROIPositionX) * fDrawingScaleX);
                    int intOrgY = (int)((m_arrEdgeROI[i].ref_ROIPositionY + m_arrDontCareROI[i][j].ref_ROIPositionY) * fDrawingScaleY);
                    int intWidth = (int)Math.Max(1, (m_arrDontCareROI[i][j].ref_ROIWidth * fDrawingScaleX));    // 2019 10 17 - CCENG: Draw at least 1 pixel if the ROI size too small.
                    int intHeight = (int)Math.Max(1, (m_arrDontCareROI[i][j].ref_ROIHeight * fDrawingScaleX));  // 2019 10 17 - CCENG: Draw at least 1 pixel if the ROI size too small.

                    g.DrawRectangle(m_PenDontCareArea, intOrgX, intOrgY, intWidth, intHeight);

                    if (i == 0 || i == 2)
                    {
                        for (int k = 3; k < intWidth; k += 3)
                        {
                            g.DrawLine(m_PenDontCareArea, intOrgX + k, intOrgY, intOrgX + k, intOrgY + intHeight);
                        }
                    }
                    else
                    {
                        for (int k = 3; k < intHeight; k += 3)
                        {
                            g.DrawLine(m_PenDontCareArea, intOrgX, intOrgY + k, intOrgX + intWidth, intOrgY + k);
                        }
                    }
                }
            }
        }
        public void DrawDontCareArea(Graphics g, float fDrawingScaleX, float fDrawingScaleY, int intSelectedGaugeEdgeMask)
        {
            for (int i = 0; i < m_arrDontCareROI.Count; i++)
            {
                if ((intSelectedGaugeEdgeMask & (0x01 << i)) > 0)
                {
                    for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
                    {
                        int intOrgX = (int)((m_arrEdgeROI[i].ref_ROIPositionX + m_arrDontCareROI[i][j].ref_ROIPositionX) * fDrawingScaleX);
                        int intOrgY = (int)((m_arrEdgeROI[i].ref_ROIPositionY + m_arrDontCareROI[i][j].ref_ROIPositionY) * fDrawingScaleY);
                        int intWidth = (int)Math.Max(1, (m_arrDontCareROI[i][j].ref_ROIWidth * fDrawingScaleX));    // 2019 10 17 - CCENG: Draw at least 1 pixel if the ROI size too small.
                        int intHeight = (int)Math.Max(1, (m_arrDontCareROI[i][j].ref_ROIHeight * fDrawingScaleX));  // 2019 10 17 - CCENG: Draw at least 1 pixel if the ROI size too small.

                        g.DrawRectangle(m_PenDontCareArea, intOrgX, intOrgY, intWidth, intHeight);

                        if (i == 0 || i == 2)
                        {
                            for (int k = 3; k < intWidth; k += 3)
                            {
                                g.DrawLine(m_PenDontCareArea, intOrgX + k, intOrgY, intOrgX + k, intOrgY + intHeight);
                            }
                        }
                        else
                        {
                            for (int k = 3; k < intHeight; k += 3)
                            {
                                g.DrawLine(m_PenDontCareArea, intOrgX, intOrgY + k, intOrgX + intWidth, intOrgY + k);
                            }
                        }
                    }
                }
            }
        }
        public void DrawAutoDontCareBlobLine(Graphics g, float fScaleX, float fScaleY, int intPadIndex)
        {
            for (int i = 0; i < m_arrAutoDontCare.Count; i++)
            {
                int intSelectedLine = GetLineGaugeSelectedIndex(intPadIndex, i);
                if (intSelectedLine >= m_arrAutoDontCare.Count)
                    continue;

                if (!m_arrAutoDontCare[intSelectedLine])
                    continue;

                for (int j = 0; j < m_arrBlob[i].ref_intNumSelectedObject; j++)
                {
                    //m_arrBlob[i].DrawSelectedBlob(g, fScaleX, fScaleY, Color.Red);

                    int intROIStartX = (int)(m_arrEdgeROI[i].ref_ROIPositionX * fScaleX);
                    int intROIStartY = (int)(m_arrEdgeROI[i].ref_ROIPositionY * fScaleY);
                    int intROIWidth = (int)Math.Max(1, (m_arrEdgeROI[i].ref_ROIWidth * fScaleX));
                    int intROIHeight = (int)Math.Max(1, (m_arrEdgeROI[i].ref_ROIHeight * fScaleY));

                    int intOrgX = (int)((m_arrEdgeROI[i].ref_ROIPositionX + m_arrBlob[i].ref_arrLimitCenterX[j] - (m_arrBlob[i].ref_arrWidth[j] / 2)) * fScaleX);
                    int intOrgY = (int)((m_arrEdgeROI[i].ref_ROIPositionY + m_arrBlob[i].ref_arrLimitCenterY[j] - (m_arrBlob[i].ref_arrHeight[j] / 2)) * fScaleY);
                    int intWidth = (int)Math.Max(1, (m_arrBlob[i].ref_arrWidth[j] * fScaleX));
                    int intHeight = (int)Math.Max(1, (m_arrBlob[i].ref_arrHeight[j] * fScaleY));

                    if (i == 0 || i == 2)
                    {
                        for (int k = 3; k < intWidth; k += 3)
                        {
                            g.DrawLine(m_PenDontCareArea, intOrgX + k, intROIStartY, intOrgX + k, intROIStartY + intROIHeight);
                        }
                    }
                    else
                    {
                        for (int k = 3; k < intHeight; k += 3)
                        {
                            g.DrawLine(m_PenDontCareArea, intROIStartX, intOrgY + k, intROIStartX + intROIWidth, intOrgY + k);
                        }
                    }
                }
            }
        }
        public bool VerifyROI(int nNewXPoint, int nNewYPoint)
        {
            if (m_arrEdgeROI[m_intSelectedEdgeROIPrev].VerifyROIArea(nNewXPoint, nNewYPoint))
            {
                return true;
            }

            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                if (m_arrEdgeROI[i].VerifyROIArea(nNewXPoint, nNewYPoint))
                {
                    m_intSelectedEdgeROIPrev = i;
                    return true;
                }
            }

            return false;
        }

        public bool VerifyROI(int nNewXPoint, int nNewYPoint, int intSelectedGaugeEdgeMask)
        {
            if (m_arrEdgeROI[m_intSelectedEdgeROIPrev].VerifyROIArea(nNewXPoint, nNewYPoint))
            {
                return true;
            }

            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                if ((intSelectedGaugeEdgeMask & (0x01 << i)) > 0)
                {
                    if (m_arrEdgeROI[i].VerifyROIArea(nNewXPoint, nNewYPoint))
                    {
                        m_intSelectedEdgeROIPrev = i;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool VerifyROIHandleShape(int nNewXPoint, int nNewYPoint)
        {
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                if (m_blnCursorShapeVerifying && m_intSelectedEdgeROIPrev != i)
                    continue;

                m_arrEdgeROI[i].VerifyROIHandleShape(nNewXPoint, nNewYPoint);

                m_intSelectedEdgeROIPrev = i;
                m_blnCursorShapeVerifying = m_arrEdgeROI[i].GetROIHandle2();
            }
            return m_blnCursorShapeVerifying;
        }

        public void ClearDragHandle()
        {
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                m_arrEdgeROI[i].ClearDragHandle();
            }
        }

        // Get Setting -------------------------------------------------------

        public float GetGaugeTolerance(int intLineIndex)
        {
            return m_arrLineGauge[intLineIndex].ref_GaugeTolerance;
        }

        public int GetGaugeFilter(int intLineIndex)
        {
            return m_arrLineGauge[intLineIndex].ref_GaugeFilter;
        }

        public int GetGaugeThickness(int intLineIndex)
        {
            return m_arrLineGauge[intLineIndex].ref_GaugeThickness;
        }
        public int GetGaugeOffset(int intLineIndex)
        {
            return m_arrGaugeOffset[intLineIndex];
        }
        public int GetGaugeThreshold(int intLineIndex)
        {
            return m_arrLineGauge[intLineIndex].ref_GaugeThreshold;
        }

        public int GetGaugeMinAmplitude(int intLineIndex)
        {
            return m_arrLineGauge[intLineIndex].ref_GaugeMinAmplitude;
        }

        public int GetGaugeMinArea(int intLineIndex)
        {
            return m_arrLineGauge[intLineIndex].ref_GaugeMinArea;
        }

        public float GetGaugeFilterThreshold(int intLineIndex)
        {
            return m_arrLineGauge[intLineIndex].ref_GaugeFilterThreshold;
        }

        public int GetGaugeFilterPasses(int intLineIndex)
        {
            return m_arrLineGauge[intLineIndex].ref_GaugeFilterPasses;
        }

        public int GetGaugeTransType(int intLineIndex)
        {
            return m_arrLineGauge[intLineIndex].ref_GaugeTransType;
        }

        public int GetGaugeTransChoice(int intLineIndex)
        {
            return m_arrLineGauge[intLineIndex].ref_GaugeTransChoice;
        }

        public float GetGaugeSamplingStep(int intLineIndex)
        {
            return m_arrLineGauge[intLineIndex].ref_GaugeSamplingStep;
        }

        public bool GetGaugeEnableSubGauge(int intLineIndex)
        {
            return m_arrGaugeEnableSubGauge[intLineIndex];
        }

        public int GetGaugeMinScore(int intLineIndex)
        {
            return m_arrGaugeMinScore[intLineIndex];
        }

        public int GetGaugeMaxAngle(int intLineIndex)
        {
            return m_arrGaugeMaxAngle[intLineIndex];
        }

        public int GetGaugeImageMode(int intLineIndex)
        {
            return m_arrGaugeImageMode[intLineIndex];
        }

        public int[] GetGaugeImageNoList()
        {
            return m_arrGaugeImageNo.ToArray();
        }

        public int GetGaugeImageNo(int intLineIndex)
        {
            return m_arrGaugeImageNo[intLineIndex];
        }

        public float GetGaugeImageGain(int intLineIndex)
        {
            return m_arrGaugeImageGain[intLineIndex];
        }

        public float GetGaugeGroupTolerance(int intLineIndex)
        {
            return m_arrGaugeGroupTolerance[intLineIndex];
        }

        public int GetGaugeImageOpenCloseIteration(int intLineIndex)
        {
            return m_arrGaugeImageOpenCloseIteration[intLineIndex];
        }
        public int GetGaugeImageThreshold(int intLineIndex)
        {
            return m_arrGaugeImageThreshold[intLineIndex];
        }
        public bool GetGaugeWantImageThreshold(int intLineIndex)
        {
            return m_arrGaugeWantImageThreshold[intLineIndex];
        }
        public int GetGaugeTiltAngle()
        {
            return m_intGaugeTiltAngle;
        }
        public int GetGaugeMeasureMode(int intLineIndex)
        {
            return m_arrGaugeMeasureMode[intLineIndex];
        }

        public float GetAngle(int intLineIndex)
        {
            // Set line gauge for different direction
            float Angle = 0;
            switch (intLineIndex)
            {
                case 0:
                    Angle = m_arrLineGauge[0].ref_GaugeAngle;    // Top
                    break;
                case 1:
                    Angle = m_arrLineGauge[1].ref_GaugeAngle;   // Right
                    break;
                case 2:
                    Angle = m_arrLineGauge[2].ref_GaugeAngle;  // Bottom
                    break;
                case 3:
                    Angle = m_arrLineGauge[3].ref_GaugeAngle;  // Right
                    break;
            }
            return Angle;
        }

        public bool IsDirectionInToOut(int intLineIndex)
        {
            // when fist array gauge has angle 0 deg, mean the whole rectangle line gauge has Out to In direction.
            //if ((m_arrLineGauge[0].ref_GaugeAngle) == 0)
            //{
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}

            if (!m_blnInToOut[intLineIndex])
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public void SetDirection(bool blnInToOut, int intLineIndex)
        {
            // Set line gauge for different direction
            if (blnInToOut)
            {
                switch (intLineIndex)
                {
                    case 0:
                        m_arrLineGauge[0].SetGaugeAngle(180);    // Top
                        m_blnInToOut[0] = true;
                        break;
                    case 1:
                        m_arrLineGauge[1].SetGaugeAngle(-90);   // Right
                        m_blnInToOut[1] = true;
                        break;
                    case 2:
                        m_arrLineGauge[2].SetGaugeAngle(0);  // Bottom
                        m_blnInToOut[2] = true;
                        break;
                    case 3:
                        m_arrLineGauge[3].SetGaugeAngle(90);  // Right
                        m_blnInToOut[3] = true;
                        break;
                }
            }
            else
            {
                switch (intLineIndex)
                {
                    case 0:
                        m_arrLineGauge[0].SetGaugeAngle(0);    // Top
                        m_blnInToOut[0] = false;
                        break;
                    case 1:
                        m_arrLineGauge[1].SetGaugeAngle(90);   // Right
                        m_blnInToOut[1] = false;
                        break;
                    case 2:
                        m_arrLineGauge[2].SetGaugeAngle(180);  // Bottom
                        m_blnInToOut[2] = false;
                        break;
                    case 3:
                        m_arrLineGauge[3].SetGaugeAngle(-90);  // Right
                        m_blnInToOut[3] = false;
                        break;
                }
            }
        }
        // Set Setting -------------------------------------------------------

        public void SetGaugeTolerance(float fTolerance)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
                m_arrLineGauge[i].ref_GaugeTolerance = fTolerance;
        }

        public void SetGaugeTolerance(float fTolerance, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeTolerance = fTolerance;
        }

        public void SetGaugeTolerance(ROI objROI, float fTolerance)
        {
            int intROIWidth = objROI.ref_ROIWidth;
            int intROIHeight = objROI.ref_ROIHeight;

            float fWidth = intROIWidth - (fTolerance * 2f);
            float fHeight = intROIHeight - (fTolerance * 2f);

            //set rect gauge position
            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                if (i == 0 || i == 2)
                    m_arrLineGauge[i].SetGaugeLength(fWidth);
                else
                    m_arrLineGauge[i].SetGaugeLength(fHeight);

                m_arrLineGauge[i].ref_GaugeTolerance = fTolerance;
            }
        }

        public void SetGaugeTolerance(ROI objROI, float fTolerance, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            int intROIWidth = objROI.ref_ROIWidth;
            int intROIHeight = objROI.ref_ROIHeight;

            float fWidth = intROIWidth - (fTolerance * 2f);
            float fHeight = intROIHeight - (fTolerance * 2f);

            //set rect gauge position
            if (intLineIndex == 0 || intLineIndex == 2)
                m_arrLineGauge[intLineIndex].SetGaugeLength(fWidth);
            else
                m_arrLineGauge[intLineIndex].SetGaugeLength(fHeight);

            m_arrLineGauge[intLineIndex].ref_GaugeTolerance = fTolerance;

        }
        public void SetGaugeTolerance_BasedOnEdgeROI(int intToleranceSize, int intLineIndex)
        {
            m_arrLineGauge[intLineIndex].SetGaugeTolerance(intToleranceSize / 2f);

            //// Top
            //m_arrLineGauge[0].SetGaugeTolerance(intToleranceSize / 2f);

            //// Right
            //m_arrLineGauge[1].SetGaugeTolerance(intToleranceSize / 2f);

            //// Bottom
            //m_arrLineGauge[2].SetGaugeTolerance(intToleranceSize / 2f);

            //// left
            //m_arrLineGauge[3].SetGaugeTolerance(intToleranceSize / 2f);
        }
        public void SetGaugeToleranceToTemplate()
        {
            for (int i = 0; i < 4; i++)
                m_arrLineGauge[i].ref_GaugeTolerance = m_fTemplateGaugeTolerance[i];
        }

        public void SetGaugeFilter(int intFilter)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
                m_arrLineGauge[i].ref_GaugeFilter = intFilter;
        }

        public void SetGaugeFilter(int intFilter, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeFilter = intFilter;
        }

        public void SetGaugeThickness(int intThickness)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
                m_arrLineGauge[i].ref_GaugeThickness = intThickness;
        }
        public void SetGaugeOffset(int intOffset)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
                m_arrGaugeOffset[i] = intOffset;
        }

        public bool AppendGaugeThickness(int intAppendThickness, int intLimit)
        {
            bool IsAllOverLimit = true;
            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                if (i >= m_arrUserThickness.Length)
                    break;

                int intNewThickness = m_arrUserThickness[i] + intAppendThickness;
                if (intNewThickness > 0)
                    m_arrLineGauge[i].ref_GaugeThickness = intNewThickness;
                else
                    m_arrLineGauge[i].ref_GaugeThickness = 1;

                //if (i == 0)
                //{
                //    TrackLog objTL = new TrackLog();
                //    objTL.WriteLine("C=" + m_arrLineGauge[i].ref_GaugeThickness + ", " + m_arrUserThickness[i].ToString() + ", " + intAppendThickness.ToString());
                //}

                if (m_arrLineGauge[i].ref_GaugeThickness > 1 && m_arrLineGauge[i].ref_GaugeThickness < intLimit)
                {
                    IsAllOverLimit = false;
                }
                else
                {
                }
            }

            return IsAllOverLimit;

        }

        public void SetGaugeThickness(int intThickness, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeThickness = intThickness;
        }
        public void SetGaugeOffset(int intOffset, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrGaugeOffset[intLineIndex] = intOffset;
        }

        public void SetGaugeThreshold(int intThreshold)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
                m_arrLineGauge[i].ref_GaugeThreshold = intThreshold;
        }

        public bool AppendGaugeThreshold(int intAppendThreshold, int intLimit)
        {
            bool IsAllOverLimit = true;
            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                if (i >= m_arrUserThickness.Length)
                    break;

                int intNewThreshold = m_arrUserThreshold[i] + intAppendThreshold;
                if (intNewThreshold > 0)
                    m_arrLineGauge[i].ref_GaugeThreshold = intNewThreshold;
                else
                    m_arrLineGauge[i].ref_GaugeThreshold = 1;

                //if (i == 0)
                //{
                //    TrackLog objTL = new TrackLog();
                //    objTL.WriteLine("H=" + m_arrLineGauge[i].ref_GaugeThreshold + ", " + m_arrUserThreshold[i].ToString() + ", " + intAppendThreshold.ToString());
                //}
                if (m_arrLineGauge[i].ref_GaugeThreshold > 1 && m_arrLineGauge[i].ref_GaugeThreshold < intLimit)
                {
                    IsAllOverLimit = false;
                }
            }

            return IsAllOverLimit;

        }

        public void SetGaugeThreshold(int intThreshold, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeThreshold = intThreshold;
        }

        public void SetGaugeMinAmplitude(int intMinAmplitude)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
                m_arrLineGauge[i].ref_GaugeMinAmplitude = intMinAmplitude;
        }

        public void SetGaugeMinScore(int intMinScore)
        {
            for (int i = 0; i < m_arrGaugeMinScore.Count; i++)
                m_arrGaugeMinScore[i] = intMinScore;
        }

        public void SetGaugeMaxAngle(int intMaxAngle)
        {
            for (int i = 0; i < m_arrGaugeMaxAngle.Count; i++)
                m_arrGaugeMaxAngle[i] = intMaxAngle;
        }

        public void SetGaugeImageMode(int intImageMode)
        {
            for (int i = 0; i < m_arrGaugeImageMode.Count; i++)
                m_arrGaugeImageMode[i] = intImageMode;
        }

        public void SetGaugeImageMode(int intImageMode, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrGaugeImageMode[intLineIndex] = intImageMode;
        }

        public void SetGaugeImageNo(int intImageNo)
        {
            for (int i = 0; i < m_arrGaugeImageNo.Count; i++)
                m_arrGaugeImageNo[i] = intImageNo;
        }

        public void SetGaugeImageGain(float fImageGain)
        {
            for (int i = 0; i < m_arrGaugeImageGain.Count; i++)
                m_arrGaugeImageGain[i] = fImageGain;
        }

        public void SetGaugeGroupTolerance(float fGroupTolerance)
        {
            for (int i = 0; i < m_arrGaugeGroupTolerance.Count; i++)
                m_arrGaugeGroupTolerance[i] = fGroupTolerance;
        }

        public void SetGaugeImageOpenCloseIteration(int intImageThreshold)
        {
            for (int i = 0; i < m_arrGaugeImageOpenCloseIteration.Count; i++)
                m_arrGaugeImageOpenCloseIteration[i] = intImageThreshold;
        }
        public void SetGaugeImageThreshold(int intImageThreshold)
        {
            for (int i = 0; i < m_arrGaugeImageThreshold.Count; i++)
                m_arrGaugeImageThreshold[i] = intImageThreshold;
        }
        public void SetGaugeWantImageThreshold(bool blnWantImageThreshold)
        {
            for (int i = 0; i < m_arrGaugeWantImageThreshold.Count; i++)
                m_arrGaugeWantImageThreshold[i] = blnWantImageThreshold;
        }
        public void SetGaugeTiltAngle(int intPadSelectedIndex)
        {
            switch (intPadSelectedIndex)
            {
                case 1: // Top Pad
                    if (m_blnInToOut[1])
                    {
                        m_arrLineGauge[1].ref_GaugeAngle = -90 + m_intGaugeTiltAngle;   // Right Side
                    }
                    else
                    {
                        m_arrLineGauge[1].ref_GaugeAngle = 90 + m_intGaugeTiltAngle;   // Right Side
                    }
                    if (m_blnInToOut[3])
                    {
                        m_arrLineGauge[3].ref_GaugeAngle = 90 - m_intGaugeTiltAngle;   // Left Side
                    }
                    else
                    {
                        m_arrLineGauge[3].ref_GaugeAngle = -90 - m_intGaugeTiltAngle;   // Left Side
                    }
                    break;
                case 2:// Right Pad
                    if (m_blnInToOut[2])
                    {
                        m_arrLineGauge[2].ref_GaugeAngle = 0 + m_intGaugeTiltAngle;   // Right Side
                    }
                    else
                    {
                        m_arrLineGauge[2].ref_GaugeAngle = 180 + m_intGaugeTiltAngle;   // Right Side
                    }
                    if (m_blnInToOut[0])
                    {
                        m_arrLineGauge[0].ref_GaugeAngle = 180 - m_intGaugeTiltAngle;   // Left Side
                    }
                    else
                    {
                        m_arrLineGauge[0].ref_GaugeAngle = 0 - m_intGaugeTiltAngle;   // Left Side
                    }
                    break;
                case 3: // Bottom Pad
                    if (m_blnInToOut[3])
                    {
                        m_arrLineGauge[3].ref_GaugeAngle = 90 + m_intGaugeTiltAngle;   // Right Side
                    }
                    else
                    {
                        m_arrLineGauge[3].ref_GaugeAngle = -90 + m_intGaugeTiltAngle;   // Right Side
                    }
                    if (m_blnInToOut[1])
                    {
                        m_arrLineGauge[1].ref_GaugeAngle = -90 - m_intGaugeTiltAngle;   // Left Side
                    }
                    else
                    {
                        m_arrLineGauge[1].ref_GaugeAngle = 90 - m_intGaugeTiltAngle;   // Left Side
                    }
                    break;
                case 4:// Right Pad
                    if (m_blnInToOut[0])
                    {
                        m_arrLineGauge[0].ref_GaugeAngle = 180 + m_intGaugeTiltAngle;   // Right Side
                    }
                    else
                    {
                        m_arrLineGauge[0].ref_GaugeAngle = 0 + m_intGaugeTiltAngle;   // Right Side
                    }
                    if (m_blnInToOut[2])
                    {
                        m_arrLineGauge[2].ref_GaugeAngle = 0 - m_intGaugeTiltAngle;   // Left Side
                    }
                    else
                    {
                        m_arrLineGauge[2].ref_GaugeAngle = 180 - m_intGaugeTiltAngle;   // Left Side
                    }
                    break;

            }
        }
        public void SetGaugeTiltAngle(int intTiltAngle, int intPadSelectedIndex)
        {
            m_intGaugeTiltAngle = intTiltAngle;

            switch (intPadSelectedIndex)
            {
                case 1: // Top Pad
                    if (m_blnInToOut[1])
                    {
                        m_arrLineGauge[1].ref_GaugeAngle = -90 + intTiltAngle;   // Right Side
                    }
                    else
                    {
                        m_arrLineGauge[1].ref_GaugeAngle = 90 + intTiltAngle;   // Right Side
                    }
                    if (m_blnInToOut[3])
                    {
                        m_arrLineGauge[3].ref_GaugeAngle = 90 - intTiltAngle;   // Left Side
                    }
                    else
                    {
                        m_arrLineGauge[3].ref_GaugeAngle = -90 - intTiltAngle;   // Left Side
                    }
                    break;
                case 2:// Right Pad
                    if (m_blnInToOut[2])
                    {
                        m_arrLineGauge[2].ref_GaugeAngle = 0 + intTiltAngle;   // Right Side
                    }
                    else
                    {
                        m_arrLineGauge[2].ref_GaugeAngle = 180 + intTiltAngle;   // Right Side
                    }
                    if (m_blnInToOut[0])
                    {
                        m_arrLineGauge[0].ref_GaugeAngle = 180 - intTiltAngle;   // Left Side
                    }
                    else
                    {
                        m_arrLineGauge[0].ref_GaugeAngle = 0 - intTiltAngle;   // Left Side
                    }
                    break;
                case 3: // Bottom Pad
                    if (m_blnInToOut[3])
                    {
                        m_arrLineGauge[3].ref_GaugeAngle = 90 + intTiltAngle;   // Right Side
                    }
                    else
                    {
                        m_arrLineGauge[3].ref_GaugeAngle = -90 + intTiltAngle;   // Right Side
                    }
                    if (m_blnInToOut[1])
                    {
                        m_arrLineGauge[1].ref_GaugeAngle = -90 - intTiltAngle;   // Left Side
                    }
                    else
                    {
                        m_arrLineGauge[1].ref_GaugeAngle = 90 - intTiltAngle;   // Left Side
                    }
                    break;
                case 4:// Right Pad
                    if (m_blnInToOut[0])
                    {
                        m_arrLineGauge[0].ref_GaugeAngle = 180 + intTiltAngle;   // Right Side
                    }
                    else
                    {
                        m_arrLineGauge[0].ref_GaugeAngle = 0 + intTiltAngle;   // Right Side
                    }
                    if (m_blnInToOut[2])
                    {
                        m_arrLineGauge[2].ref_GaugeAngle = 0 - intTiltAngle;   // Left Side
                    }
                    else
                    {
                        m_arrLineGauge[2].ref_GaugeAngle = 180 - intTiltAngle;   // Left Side
                    }
                    break;

            }
        }

        public void SetGaugeMinAmplitude(int intMinAmplitude, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeMinAmplitude = intMinAmplitude;
            m_arrPointGauge[intLineIndex].ref_GaugeMinAmplitude = intMinAmplitude;
            m_arrPointGauge2[intLineIndex].ref_GaugeMinAmplitude = intMinAmplitude;
        }

        public void SetGaugeMinArea(int intMinArea)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                m_arrLineGauge[i].ref_GaugeMinArea = intMinArea;
                m_arrPointGauge[i].ref_GaugeMinArea = intMinArea;
                m_arrPointGauge2[i].ref_GaugeMinArea = intMinArea;
            }
        }

        public void SetGaugeMinArea(int intMinArea, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeMinArea = intMinArea;
            m_arrPointGauge[intLineIndex].ref_GaugeMinArea = intMinArea;
            m_arrPointGauge2[intLineIndex].ref_GaugeMinArea = intMinArea;
        }

        public void SetGaugeFilterThreshold(float fFilteringThreshold)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
                m_arrLineGauge[i].ref_GaugeFilterThreshold = fFilteringThreshold;
        }

        public void SetGaugeFilterThreshold(float fFilteringThreshold, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeFilterThreshold = fFilteringThreshold;
        }

        public void SetGaugeFilterPasses(int intNumFilteringPasses)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
                m_arrLineGauge[i].ref_GaugeFilterPasses = intNumFilteringPasses;
        }

        public void SetGaugeFilterPasses(int intNumFilteringPasses, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeFilterPasses = intNumFilteringPasses;
        }

        public void SetGaugeTransType(int intTransitionTypeIndex)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                m_arrLineGauge[i].ref_GaugeTransType = intTransitionTypeIndex;
                m_arrPointGauge[i].ref_GaugeTransType = intTransitionTypeIndex;
                m_arrPointGauge2[i].ref_GaugeTransType = intTransitionTypeIndex;
            }
        }

        public void SetGaugeTransType(int intTransitionTypeIndex, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeTransType = intTransitionTypeIndex;
            m_arrPointGauge[intLineIndex].ref_GaugeTransType = intTransitionTypeIndex;
            m_arrPointGauge2[intLineIndex].ref_GaugeTransType = intTransitionTypeIndex;
        }
        public void SetGaugeMeasureMode(int intGaugeMeasureMode, int intLineIndex)
        {
            if (intLineIndex >= m_arrGaugeEnableSubGauge.Count)
                return;

            m_arrGaugeMeasureMode[intLineIndex] = intGaugeMeasureMode;
        }
        public void SetGaugeTransChoice(int intTransitionChoiceIndex)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                m_arrLineGauge[i].ref_GaugeTransChoice = intTransitionChoiceIndex;
                m_arrPointGauge[i].ref_GaugeTransChoice = intTransitionChoiceIndex;
                m_arrPointGauge2[i].ref_GaugeTransChoice = intTransitionChoiceIndex;

            }
        }

        public void SetGaugeTransChoice(int intTransitionChoiceIndex, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeTransChoice = intTransitionChoiceIndex;
            m_arrPointGauge[intLineIndex].ref_GaugeTransChoice = intTransitionChoiceIndex;
            m_arrPointGauge2[intLineIndex].ref_GaugeTransChoice = intTransitionChoiceIndex;
        }

        public void SetGaugeSamplingStep(float fSamplingStep)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
                m_arrLineGauge[i].ref_GaugeSamplingStep = fSamplingStep;
        }

        public void SetGaugeSamplingStep(float fSamplingStep, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeSamplingStep = fSamplingStep;
        }

        public void SetGaugeEnableSubGauge(bool blnEnableSubGauge, int intLineIndex)
        {
            if (intLineIndex >= m_arrGaugeEnableSubGauge.Count)
                return;

            m_arrGaugeEnableSubGauge[intLineIndex] = blnEnableSubGauge;
        }

        public void SetGaugeMinScore(int intMinScore, int intLineIndex)
        {
            if (intLineIndex >= m_arrGaugeMinScore.Count)
                return;

            m_arrGaugeMinScore[intLineIndex] = intMinScore;
        }

        public void SetGaugeMaxAngle(int intMaxAngle, int intLineIndex)
        {
            if (intLineIndex >= m_arrGaugeMaxAngle.Count)
                return;

            m_arrGaugeMaxAngle[intLineIndex] = intMaxAngle;
        }

        public void SetGaugeImageNo(int intImageNo, int intLineIndex)
        {
            if (intLineIndex >= m_arrGaugeImageNo.Count)
                return;

            m_arrGaugeImageNo[intLineIndex] = intImageNo;
        }

        public void SetGaugeImageGain(float fImageGain, int intLineIndex)
        {
            if (intLineIndex >= m_arrGaugeImageGain.Count)
                return;

            m_arrGaugeImageGain[intLineIndex] = fImageGain;
        }

        public void SetGaugeGroupTolerance(float fGroupTolerance, int intLineIndex)
        {
            if (intLineIndex >= m_arrGaugeGroupTolerance.Count)
                return;

            m_arrGaugeGroupTolerance[intLineIndex] = fGroupTolerance;
        }

        public void SetGaugeImageOpenCloseIteration(int intImageThreshold, int intLineIndex)
        {
            if (intLineIndex >= m_arrGaugeImageOpenCloseIteration.Count)
                return;

            m_arrGaugeImageOpenCloseIteration[intLineIndex] = intImageThreshold;
        }
        public void SetGaugeImageThreshold(int intImageThreshold, int intLineIndex)
        {
            if (intLineIndex >= m_arrGaugeImageThreshold.Count)
                return;

            m_arrGaugeImageThreshold[intLineIndex] = intImageThreshold;
        }
        public void SetGaugeWantImageThreshold(bool blnWantImageThreshold, int intLineIndex)
        {
            if (intLineIndex >= m_arrGaugeWantImageThreshold.Count)
                return;

            m_arrGaugeWantImageThreshold[intLineIndex] = blnWantImageThreshold;
        }

        public void SetUserSettingToUserVariables()
        {
            if (m_arrLineGauge.Count > 0)
            {
                for (int i = 0; i < m_arrLineGauge.Count; i++)
                {
                    if (i < 4)
                    {
                        m_arrUserThreshold[i] = m_arrLineGauge[i].ref_GaugeThreshold;
                        m_arrUserThickness[i] = m_arrLineGauge[i].ref_GaugeThickness;
                        m_arrUserChoice[i] = m_arrLineGauge[i].ref_GaugeTransChoice;
                    }
                }
            }
        }

        public void ResetGaugeSettingToUserVariables()
        {
            if (m_arrLineGauge.Count > 0)
            {
                for (int i = 0; i < m_arrLineGauge.Count; i++)
                {
                    if (i < 4)
                    {
                        if (m_arrLineGauge[i].ref_GaugeThreshold != m_arrUserThreshold[i])
                            m_arrLineGauge[i].ref_GaugeThreshold = m_arrUserThreshold[i];
                        if (m_arrLineGauge[i].ref_GaugeThickness != m_arrUserThickness[i])
                            m_arrLineGauge[i].ref_GaugeThickness = m_arrUserThickness[i];
                        if (m_arrLineGauge[i].ref_GaugeTransChoice != m_arrUserChoice[i])
                            m_arrLineGauge[i].ref_GaugeTransChoice = m_arrUserChoice[i];
                    }
                }
            }

        }

        public void IsGaugeSameSetting(ref bool blnThresholdSameSetting, ref bool blnThicknessSameSetting, ref bool blnChoiceSameSetting)
        {
            if ((m_arrUserThreshold[0] == m_arrUserThreshold[1]) && (m_arrUserThreshold[0] == m_arrUserThreshold[2]) && (m_arrUserThreshold[0] == m_arrUserThreshold[3]))
                blnThresholdSameSetting = true;
            else
                blnThresholdSameSetting = false;

            if ((m_arrUserThickness[0] == m_arrUserThickness[1]) && (m_arrUserThickness[0] == m_arrUserThickness[2]) && (m_arrUserThickness[0] == m_arrUserThickness[3]))
                blnThicknessSameSetting = true;
            else
                blnThicknessSameSetting = false;

            if ((m_arrUserChoice[0] == m_arrUserChoice[1]) && (m_arrUserChoice[0] == m_arrUserChoice[2]) && (m_arrUserChoice[0] == m_arrUserChoice[3]))
                blnChoiceSameSetting = true;
            else
                blnChoiceSameSetting = false;
        }

        public int GetUserThreshold(int intIndex)
        {
            if (intIndex < m_arrUserThreshold.Length)
                return m_arrUserThreshold[intIndex];
            else
                return 0;
        }

        public int GetUserThickness(int intIndex)
        {
            if (intIndex < m_arrUserThickness.Length)
                return m_arrUserThickness[intIndex];
            else
                return 0;
        }

        public int GetUserChoice(int intIndex)
        {
            if (intIndex < m_arrUserChoice.Length)
                return m_arrUserChoice[intIndex];
            else
                return 0;
        }

        public bool Is4SidesLineResultOK()
        {
            return m_arrLineResultOK[0] && m_arrLineResultOK[1] && m_arrLineResultOK[2] && m_arrLineResultOK[3];
        }

        public bool IsMeasureSizeUnderTemplateSizeTolerance(float fTolerance)
        {
            if (m_fRectAngle > 1)
                return false;

            return ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) <= fTolerance) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) <= fTolerance));
        }

        public void SetDirection(bool blnInToOut)
        {
            if (blnInToOut)
            {
                // Set line gauge for different direction
                m_arrLineGauge[0].SetGaugeAngle(180);    // Top
                m_arrLineGauge[1].SetGaugeAngle(-90);   // Right
                m_arrLineGauge[2].SetGaugeAngle(0);  // Bottom
                m_arrLineGauge[3].SetGaugeAngle(90);  // Right
                m_blnInToOut[0] = true;
                m_blnInToOut[1] = true;
                m_blnInToOut[2] = true;
                m_blnInToOut[3] = true;
            }
            else
            {
                // Set line gauge for different direction
                m_arrLineGauge[0].SetGaugeAngle(0);    // Top
                m_arrLineGauge[1].SetGaugeAngle(90);   // Right
                m_arrLineGauge[2].SetGaugeAngle(180);  // Bottom
                m_arrLineGauge[3].SetGaugeAngle(-90);  // Right
                m_blnInToOut[0] = false;
                m_blnInToOut[1] = false;
                m_blnInToOut[2] = false;
                m_blnInToOut[3] = false;
            }
        }

        public void AddGainForEdgeROI(ref ImageDrawing objSourceImage, ref ImageDrawing objDestinationImage, int intSelectedEdgeROI)
        {
            if (objDestinationImage.ref_intImageWidth != objSourceImage.ref_intImageWidth || objDestinationImage.ref_intImageHeight != objSourceImage.ref_intImageHeight)
            {
                objDestinationImage.SetImageSize(objSourceImage.ref_intImageWidth, objSourceImage.ref_intImageHeight);
            }

            m_arrEdgeROI[intSelectedEdgeROI].AttachImage(objSourceImage);
            m_arrEdgeROI[intSelectedEdgeROI].GainTo_ROIToROISamePosition(ref objDestinationImage, m_arrGaugeImageGain[intSelectedEdgeROI]);
        }
        public void AddThresholdForEdgeROI(ref ImageDrawing objSourceImage, ref ImageDrawing objDestinationImage, int intSelectedEdgeROI)
        {
            if (objDestinationImage.ref_intImageWidth != objSourceImage.ref_intImageWidth || objDestinationImage.ref_intImageHeight != objSourceImage.ref_intImageHeight)
            {
                objDestinationImage.SetImageSize(objSourceImage.ref_intImageWidth, objSourceImage.ref_intImageHeight);
            }

            m_arrEdgeROI[intSelectedEdgeROI].AttachImage(objSourceImage);
            m_arrEdgeROI[intSelectedEdgeROI].ThresholdTo_ROIToROISamePosition(ref objDestinationImage, m_arrGaugeImageThreshold[intSelectedEdgeROI]);
        }
        public void AddAutoDontCareThresholdForEdgeROI(ref ImageDrawing objSourceImage, ref ImageDrawing objDestinationImage, int intPadIndex)
        {
            if (objDestinationImage.ref_intImageWidth != objSourceImage.ref_intImageWidth || objDestinationImage.ref_intImageHeight != objSourceImage.ref_intImageHeight)
            {
                objDestinationImage.SetImageSize(objSourceImage.ref_intImageWidth, objSourceImage.ref_intImageHeight);
            }

            m_arrEdgeROI[m_intSelectedEdgeROIPrev].AttachImage(objSourceImage);
            m_arrEdgeROI[m_intSelectedEdgeROIPrev].ThresholdTo_ROIToROISamePosition(ref objDestinationImage, m_arrAutoDontCareThreshold[GetLineGaugeSelectedIndex(intPadIndex)]);
        }
        public void AddOpenCloseForEdgeROI(ref ImageDrawing objSourceImage, ref ImageDrawing objDestinationImage, int intSelectedEdgeROI)
        {
            if (objDestinationImage.ref_intImageWidth != objSourceImage.ref_intImageWidth || objDestinationImage.ref_intImageHeight != objSourceImage.ref_intImageHeight)
            {
                objDestinationImage.SetImageSize(objSourceImage.ref_intImageWidth, objSourceImage.ref_intImageHeight);
            }

            m_arrEdgeROI[intSelectedEdgeROI].AttachImage(objSourceImage);
            m_arrEdgeROI[intSelectedEdgeROI].OpenCloseTo_ROIToROISamePosition(ref objDestinationImage, m_arrGaugeImageOpenCloseIteration[intSelectedEdgeROI]);
        }
        public void AddPrewittForEdgeROI(ref ImageDrawing objSourceImage, ref ImageDrawing objDestinationImage, int intSelectedEdgeROI)
        {
            if (objDestinationImage.ref_intImageWidth != objSourceImage.ref_intImageWidth || objDestinationImage.ref_intImageHeight != objSourceImage.ref_intImageHeight)
            {
                objDestinationImage.SetImageSize(objSourceImage.ref_intImageWidth, objSourceImage.ref_intImageHeight);
            }

            m_arrEdgeROI[intSelectedEdgeROI].AttachImage(objSourceImage);
            m_arrEdgeROI[intSelectedEdgeROI].PrewittTo_ROIToROISamePosition(ref objDestinationImage);
        }

        public void AddGainForEdgeROI(ref ImageDrawing objSourceImage, ref ImageDrawing objDestinationImage)
        {
            if (objDestinationImage.ref_intImageWidth != objSourceImage.ref_intImageWidth || objDestinationImage.ref_intImageHeight != objSourceImage.ref_intImageHeight)
            {
                objDestinationImage.SetImageSize(objSourceImage.ref_intImageWidth, objSourceImage.ref_intImageHeight);
            }

            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                m_arrEdgeROI[i].AttachImage(objSourceImage);
                m_arrEdgeROI[i].GainTo_ROIToROISamePosition(ref objDestinationImage, m_arrGaugeImageGain[i]);
            }
        }
        public void AddThresholdForEdgeROI(ref ImageDrawing objSourceImage, ref ImageDrawing objDestinationImage)
        {
            if (objDestinationImage.ref_intImageWidth != objSourceImage.ref_intImageWidth || objDestinationImage.ref_intImageHeight != objSourceImage.ref_intImageHeight)
            {
                objDestinationImage.SetImageSize(objSourceImage.ref_intImageWidth, objSourceImage.ref_intImageHeight);
            }

            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                m_arrEdgeROI[i].AttachImage(objSourceImage);
                m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition(ref objDestinationImage, m_arrGaugeImageThreshold[i]);
            }
        }
        public bool Measure_Pad5SPackage_WithDontCareArea_ImprovedAngleVersion(List<ImageDrawing> arrImage, bool blnEnableVirtualLine, ImageDrawing objWhiteImage, int intPadIndex, bool bUseTemplateUnitSize, float fTemplateUnitSizeX, float fTemplateUnitSizeY)
        {
            List<List<ImageDrawing>> arrModifiedImage = new List<List<ImageDrawing>>();
            ImageDrawing[] objPreModifiedImage = new ImageDrawing[4] { null, null, null, null };
            ImageDrawing[] objOriImage = new ImageDrawing[4] { null, null, null, null };
            // Copy source image to temporary local image .
            for (int i = 0; i < arrImage.Count; i++)
            {
                arrModifiedImage.Add(new List<ImageDrawing>());
                arrModifiedImage[i].Add(null);
                arrModifiedImage[i].Add(null);
                arrModifiedImage[i].Add(null);
                arrModifiedImage[i].Add(null);

                bool blnFound = false;
                for (int j = 0; j < m_arrGaugeImageNo.Count; j++)
                {
                    if (i == ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[j], m_intVisionIndex))
                    {
                        blnFound = true;
                        break;
                    }
                }

                if (blnFound)
                {
                    for (int j = 0; j < arrModifiedImage[i].Count; j++)
                    {
                        arrModifiedImage[i][j] = new ImageDrawing(true, arrImage[i].ref_intImageWidth, arrImage[i].ref_intImageHeight);
                        //arrImage[i].CopyTo(ref arrModifiedImage, i);

                        // 2020-06-12 ZJYEOH : Copy each ROI image to separate arrModifiedImage so that they will not overlap each other for image processing
                        arrModifiedImage[i][j].SetImageToBlack();
                        ROI objROI = new ROI();
                        objROI.LoadROISetting(m_arrEdgeROI[j].ref_ROIPositionX, m_arrEdgeROI[j].ref_ROIPositionY,
                        m_arrEdgeROI[j].ref_ROIWidth, m_arrEdgeROI[j].ref_ROIHeight);
                        objROI.AttachImage(arrModifiedImage[i][j]);
                        m_arrEdgeROI[j].AttachImage(arrImage[i]);

                        m_arrEdgeROI[j].CopyImage_Bigger(ref objROI);

                        objROI.Dispose();

                    }
                }
            }

            // Add gain to temporary image
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intImageIndex >= arrImage.Count)
                    intImageIndex = 0;

                if (objOriImage[i] == null)
                {
                    objOriImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);

                    // 2021 04 25 - CCENG: sometime image is not fully dark but affected by memory noise. This will create unconsistant inspection result.
                    objOriImage[i].SetImageToBlack();
                }
                m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objOriImage[i], m_arrGaugeImageGain[i]);

                if (m_arrGaugeImageMode[i] == 0)
                {
                    if (m_arrGaugeWantImageThreshold[i])
                    {
                        if (objPreModifiedImage[i] == null)
                        {
                            objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                        }

                        if (m_arrGaugeImageOpenCloseIteration[i] == 0)
                        {
                            m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                            m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                            ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                            m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                            m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageThreshold[i]);
                            //objImage.SaveImage("D:\\TS\\objImage1.bmp");
                            m_arrEdgeROI[i].AttachImage(objImage);
                        }
                        else
                        {
                            ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                            m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                            m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageGain[i]);

                            m_arrEdgeROI[i].AttachImage(objImage);
                            m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageThreshold[i]);
                            //objImage.SaveImage("D:\\TS\\objImage1.bmp");
                            m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                            m_arrEdgeROI[i].OpenCloseTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageOpenCloseIteration[i]);
                            //objImage.SaveImage("D:\\TS\\objImage2.bmp");
                            m_arrEdgeROI[i].AttachImage(objImage);
                        }
                    }
                    else
                    {
                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition(ref objImage, m_arrGaugeImageGain[i]);
                    }
                }
                else
                {
                    if (objPreModifiedImage[i] == null)
                    {
                        objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                    }

                    m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                    m_arrEdgeROI[i].GainTo_ROIToROISamePosition(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                    ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                    m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                    m_arrEdgeROI[i].PrewittTo_ROIToROISamePosition(ref objImage);
                }

                m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
            }

            // if dont care present
            if (m_arrDontCareROI[0].Count != 0 || m_arrDontCareROI[1].Count != 0 || m_arrDontCareROI[2].Count != 0 || m_arrDontCareROI[3].Count != 0)
            {
                ModifyDontCareImage(arrModifiedImage, objWhiteImage, intPadIndex);
                bool blnResult = true;
                if (m_arrGaugeMeasureMode.Contains(2))
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_BlobMethod(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex);
                }
                else if (m_arrGaugeMeasureMode.Contains(3))
                {
                    if (bUseTemplateUnitSize)
                    {
                        //blnResult = GetPosition_UsingEdgeAndCornerLineGauge_CollectList(m_arrLocalROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_ImprovedAngleVersion_SubGaugePoints_CollectList(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                    }
                    else
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_ImprovedAngleVersion_SubGaugePoints(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex);
                    }

                    if (!blnResult)
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_Pad5SPkg_SubGaugePoints(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex);
                }
                else
                {
                    //2020-02-10 ZJYEOH : Need to measure using modified image
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_ImprovedAngleVersion(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex);

                    if (m_arrGaugeMeasureMode.Contains(1))
                        if (!blnResult)// && m_arrGaugeEnableSubGauge.Contains(true))
                            blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_Pad5SPkg(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex);
                }

                for (int i = 0; i < arrModifiedImage.Count; i++)
                {
                    for (int j = 0; j < arrModifiedImage[i].Count; j++)
                    {
                        if (arrModifiedImage[i][j] != null)
                        {
                            arrModifiedImage[i][j].Dispose();
                            arrModifiedImage[i][j] = null;
                        }
                    }
                }

                for (int i = 0; i < objPreModifiedImage.Length; i++)
                {
                    if (objPreModifiedImage[i] != null)
                    {
                        objPreModifiedImage[i].Dispose();
                        objPreModifiedImage[i] = null;
                    }
                }

                for (int i = 0; i < objOriImage.Length; i++)
                {
                    if (objOriImage[i] != null)
                    {
                        objOriImage[i].Dispose();
                        objOriImage[i] = null;
                    }
                }

                return blnResult;
            }
            else
            {
                ModifyAutoDontCareImage(arrModifiedImage, objWhiteImage, objOriImage, intPadIndex);
                bool blnResult = true;
                if (m_arrGaugeMeasureMode.Contains(2))
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_BlobMethod(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex);
                }
                else if (m_arrGaugeMeasureMode.Contains(3))
                {
                    if (bUseTemplateUnitSize)
                    {
                        //blnResult = GetPosition_UsingEdgeAndCornerLineGauge_CollectList(m_arrLocalROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_ImprovedAngleVersion_SubGaugePoints_CollectList(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                    }
                    else
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_ImprovedAngleVersion_SubGaugePoints(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex);
                    }

                    if (!blnResult)
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_Pad5SPkg_SubGaugePoints(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex);
                }
                else if (m_arrGaugeMeasureMode.Contains(4))
                {
                    if (bUseTemplateUnitSize)
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_CornerPointGaugeTB(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                    }
                    else
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_CornerPointGaugeTB_Ver1(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                    }
                }
                else
                {
                    //2020-02-10 ZJYEOH : Need to measure using modified image
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_ImprovedAngleVersion(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex);

                    if (m_arrGaugeMeasureMode.Contains(1))
                        if (!blnResult)// && m_arrGaugeEnableSubGauge.Contains(true))
                            blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_Pad5SPkg(arrModifiedImage, blnEnableVirtualLine, false, intPadIndex);
                }

                for (int i = 0; i < arrModifiedImage.Count; i++)
                {
                    for (int j = 0; j < arrModifiedImage[i].Count; j++)
                    {
                        if (arrModifiedImage[i][j] != null)
                        {
                            arrModifiedImage[i][j].Dispose();
                            arrModifiedImage[i][j] = null;
                        }
                    }
                }

                for (int i = 0; i < objPreModifiedImage.Length; i++)
                {
                    if (objPreModifiedImage[i] != null)
                    {
                        objPreModifiedImage[i].Dispose();
                        objPreModifiedImage[i] = null;
                    }
                }

                for (int i = 0; i < objOriImage.Length; i++)
                {
                    if (objOriImage[i] != null)
                    {
                        objOriImage[i].Dispose();
                        objOriImage[i] = null;
                    }
                }

                //return GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg(arrImage, blnEnableVirtualLine, false, 0);

                //bool blnResult = GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg(arrImage, blnEnableVirtualLine, false, 0);

                //if (!blnResult && m_arrGaugeEnableSubGauge.Contains(true))
                //    blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_Pad5SPkg(arrImage, blnEnableVirtualLine, false, 0);

                return blnResult;
            }
        }

        private bool GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_ImprovedAngleVersion(List<ImageDrawing> arrImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex)
        {
            // Init data
            float fFinalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intImageIndex >= arrImage.Count)
                    intImageIndex = 0;

                m_arrLineGauge[i].Measure(arrImage[intImageIndex]);
                //m_arrLineGauge[i].Measure(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)]);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad_ImproveAngleVersion(m_arrLineGauge, 50, ref fFinalAngle, ref nCount, ref blnResult);

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // 2020 03 08 - CCENG: for side ROI, use top gauge angle bcos most of the pad stand off rely on this top gauge angle.
            //                     Also if using bottom gauge angle, it is not stable normally.
            bool blnSingleAnglePass = false;
            if (intPadIndex == 1)
            {
                if (blnResult[0])
                {
                    m_fRectAngle = m_arrLineGauge[0].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 2)
            {
                if (blnResult[1])
                {
                    m_fRectAngle = m_arrLineGauge[1].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 3)
            {
                if (blnResult[2])
                {
                    m_fRectAngle = m_arrLineGauge[2].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 4)
            {
                if (blnResult[3])
                {
                    m_fRectAngle = m_arrLineGauge[3].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }

            if (!blnSingleAnglePass)
            {
                // Get average angle from Edge Line Gauges Angle
                m_fRectAngle = fFinalAngle;
            }

            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                }
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go down
                }
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                }
            }
            bool blnRepeat = false;
            bool isResultOK = true;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 3001");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                if (blnEnableVirtualLine)
                {

                    // Verify corner angle
                    isResultOK = true;
                    int intBestAngleIndex = -1;
                    float fBestAngle = float.MaxValue;
                    bool[] arrAngleOK = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {

                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                        if (fAngle > m_arrGaugeMaxAngle[i])
                        {
                            if (isResultOK)
                                isResultOK = false;
                        }
                        else
                        {
                            arrAngleOK[i] = true;

                            if (fAngle < fBestAngle)
                            {
                                fBestAngle = fAngle;
                                intBestAngleIndex = i;
                            }
                        }

                    }


                    if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                    {
                        blnRepeat = true;

                        int intBestVerIndex = -1, intBestHorIndex = -1;
                        switch (intBestAngleIndex)
                        {
                            case 0:
                                intBestHorIndex = 0;
                                intBestVerIndex = 3;
                                break;
                            case 1:
                                intBestHorIndex = 0;
                                intBestVerIndex = 1;
                                break;
                            case 2:
                                intBestHorIndex = 2;
                                intBestVerIndex = 1;
                                break;
                            case 3:
                                intBestHorIndex = 2;
                                intBestVerIndex = 3;
                                break;
                        }

                        // check can use the good angle to build the virtual line
                        if (!arrAngleOK[0]) // Top Left angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }

                            if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[0] = true;
                        }

                        if (!arrAngleOK[1]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[1] = true;
                        }

                        if (!arrAngleOK[2]) // Top Right angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[2] = true;
                        }

                        if (!arrAngleOK[3]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[3] = true;
                        }
                    }
                    else
                        break;
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Verify Unit Size

            if (!blnEnableVirtualLine)
            {
                if (nCount < 4)
                {
                    isResultOK = false;
                    m_strErrorMessage = "*Measure Edge Fail.";
                    //m_intFailResultMask |= 0x02;
                    //return false;
                }
            }

            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_ImprovedAngleVersion(List<List<ImageDrawing>> arrImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex)
        {
            // Init data
            float fFinalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intImageIndex >= arrImage.Count)
                    intImageIndex = 0;

                m_arrLineGauge[i].Measure(arrImage[intImageIndex][i]);

                //m_arrLineGauge[i].Measure(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad_ImproveAngleVersion(m_arrLineGauge, 50, ref fFinalAngle, ref nCount, ref blnResult);

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // 2020 03 08 - CCENG: for side ROI, use top gauge angle bcos most of the pad stand off rely on this top gauge angle.
            //                     Also if using bottom gauge angle, it is not stable normally.
            bool blnSingleAnglePass = false;
            if (intPadIndex == 1)
            {
                if (blnResult[0])
                {
                    m_fRectAngle = m_arrLineGauge[0].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 2)
            {
                if (blnResult[1])
                {
                    m_fRectAngle = m_arrLineGauge[1].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 3)
            {
                if (blnResult[2])
                {
                    m_fRectAngle = m_arrLineGauge[2].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 4)
            {
                if (blnResult[3])
                {
                    m_fRectAngle = m_arrLineGauge[3].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }

            if (!blnSingleAnglePass)
            {
                // Get average angle from Edge Line Gauges Angle
                m_fRectAngle = fFinalAngle;
            }

            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                }
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go down
                }
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                }
            }
            bool blnRepeat = false;
            bool isResultOK = true;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 3002");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                if (blnEnableVirtualLine)
                {

                    // Verify corner angle
                    isResultOK = true;
                    int intBestAngleIndex = -1;
                    float fBestAngle = float.MaxValue;
                    bool[] arrAngleOK = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {

                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                        if (fAngle > m_arrGaugeMaxAngle[i])
                        {
                            if (isResultOK)
                                isResultOK = false;
                        }
                        else
                        {
                            arrAngleOK[i] = true;

                            if (fAngle < fBestAngle)
                            {
                                fBestAngle = fAngle;
                                intBestAngleIndex = i;
                            }
                        }

                    }


                    if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                    {
                        blnRepeat = true;

                        int intBestVerIndex = -1, intBestHorIndex = -1;
                        switch (intBestAngleIndex)
                        {
                            case 0:
                                intBestHorIndex = 0;
                                intBestVerIndex = 3;
                                break;
                            case 1:
                                intBestHorIndex = 0;
                                intBestVerIndex = 1;
                                break;
                            case 2:
                                intBestHorIndex = 2;
                                intBestVerIndex = 1;
                                break;
                            case 3:
                                intBestHorIndex = 2;
                                intBestVerIndex = 3;
                                break;
                        }

                        // check can use the good angle to build the virtual line
                        if (!arrAngleOK[0]) // Top Left angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }

                            if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[0] = true;
                        }

                        if (!arrAngleOK[1]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[1] = true;
                        }

                        if (!arrAngleOK[2]) // Top Right angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[2] = true;
                        }

                        if (!arrAngleOK[3]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[3] = true;
                        }
                    }
                    else
                        break;
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Verify Unit Size

            if (!blnEnableVirtualLine)
            {
                if (nCount < 4)
                {
                    isResultOK = false;
                    m_strErrorMessage = "*Measure Edge Fail.";
                    //m_intFailResultMask |= 0x02;
                    //return false;
                }
            }

            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_ImprovedAngleVersion_SubGaugePoints(List<List<ImageDrawing>> arrImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex)
        {
            // Init data
            float fFinalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intImageIndex >= arrImage.Count)
                    intImageIndex = 0;

                m_arrLineGauge[i].Measure(arrImage[intImageIndex][i]);

                //m_arrLineGauge[i].Measure(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad_ImproveAngleVersion(m_arrLineGauge, 50, ref fFinalAngle, ref nCount, ref blnResult);

            // Double verify Line Gauge result ok or not
            for (int i = 0; i < 4; i++)
            {
                if (blnResult[i])
                {
                    //blnResult[i] = m_arrLineGauge[i].VerifyMeasurement_SubGaugePoints(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]);
                    int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);  // 2020 08 14 - CCENG - prevent out of index when use wrong recipe
                    if (intROIIndex >= arrImage.Count)
                        intROIIndex = 0;

                    ROI objROI = new ROI();
                    objROI.AttachImage(arrImage[intROIIndex][i]);
                    objROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX, m_arrEdgeROI[i].ref_ROIPositionY, m_arrEdgeROI[i].ref_ROIWidth, m_arrEdgeROI[i].ref_ROIHeight);
                    blnResult[i] = m_arrLineGauge[i].VerifyMeasurement_SubGaugePoints(objROI);
                    objROI.Dispose();
                    if (!blnResult[i])
                        nCount--;
                }
            }

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // 2020 03 08 - CCENG: for side ROI, use top gauge angle bcos most of the pad stand off rely on this top gauge angle.
            //                     Also if using bottom gauge angle, it is not stable normally.
            bool blnSingleAnglePass = false;
            if (intPadIndex == 1)
            {
                if (blnResult[0])
                {
                    m_fRectAngle = m_arrLineGauge[0].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 2)
            {
                if (blnResult[1])
                {
                    m_fRectAngle = m_arrLineGauge[1].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 3)
            {
                if (blnResult[2])
                {
                    m_fRectAngle = m_arrLineGauge[2].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 4)
            {
                if (blnResult[3])
                {
                    m_fRectAngle = m_arrLineGauge[3].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }

            if (!blnSingleAnglePass)
            {
                // Get average angle from Edge Line Gauges Angle
                m_fRectAngle = fFinalAngle;
            }

            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                }
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go down
                }
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                }
            }
            bool blnRepeat = false;
            bool isResultOK = true;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 3002");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                if (blnEnableVirtualLine)
                {

                    // Verify corner angle
                    isResultOK = true;
                    int intBestAngleIndex = -1;
                    float fBestAngle = float.MaxValue;
                    bool[] arrAngleOK = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {

                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                        if (fAngle > m_arrGaugeMaxAngle[i])
                        {
                            if (isResultOK)
                                isResultOK = false;
                        }
                        else
                        {
                            arrAngleOK[i] = true;

                            if (fAngle < fBestAngle)
                            {
                                fBestAngle = fAngle;
                                intBestAngleIndex = i;
                            }
                        }

                    }


                    if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                    {
                        blnRepeat = true;

                        int intBestVerIndex = -1, intBestHorIndex = -1;
                        switch (intBestAngleIndex)
                        {
                            case 0:
                                intBestHorIndex = 0;
                                intBestVerIndex = 3;
                                break;
                            case 1:
                                intBestHorIndex = 0;
                                intBestVerIndex = 1;
                                break;
                            case 2:
                                intBestHorIndex = 2;
                                intBestVerIndex = 1;
                                break;
                            case 3:
                                intBestHorIndex = 2;
                                intBestVerIndex = 3;
                                break;
                        }

                        // check can use the good angle to build the virtual line
                        if (!arrAngleOK[0]) // Top Left angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }

                            if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[0] = true;
                        }

                        if (!arrAngleOK[1]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[1] = true;
                        }

                        if (!arrAngleOK[2]) // Top Right angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[2] = true;
                        }

                        if (!arrAngleOK[3]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[3] = true;
                        }
                    }
                    else
                        break;
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Verify Unit Size

            if (!blnEnableVirtualLine)
            {
                if (nCount < 4)
                {
                    isResultOK = false;
                    m_strErrorMessage = "*Measure Edge Fail.";
                    //m_intFailResultMask |= 0x02;
                    //return false;
                }
            }

            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_ImprovedAngleVersion_SubGaugePoints_CollectList(List<List<ImageDrawing>> arrImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex, bool bUseTemplateUnitSize, float fTemplateUnitSizeX, float fTemplateUnitSizeY)
        {
            bool blnDebugImage = false;
            ClearGaugeRecordList();
            ROI objROI = new ROI();
            // Init data
            float fFinalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intImageIndex >= arrImage.Count)
                    intImageIndex = 0;

                m_arrLineGauge[i].Measure(arrImage[intImageIndex][i]);

                //m_arrLineGauge[i].Measure(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad_ImproveAngleVersion(m_arrLineGauge, 50, ref fFinalAngle, ref nCount, ref blnResult);

            // Double verify Line Gauge result ok or not
            for (int i = 0; i < 4; i++)
            {
                if (blnResult[i])
                {
                    //blnResult[i] = m_arrLineGauge[i].VerifyMeasurement_SubGaugePoints(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]);
                    int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);  // 2020 08 14 - CCENG - prevent out of index when use wrong recipe
                    if (intROIIndex >= arrImage.Count)
                        intROIIndex = 0;
                    
                    objROI.AttachImage(arrImage[intROIIndex][i]);
                    objROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX, m_arrEdgeROI[i].ref_ROIPositionY, m_arrEdgeROI[i].ref_ROIWidth, m_arrEdgeROI[i].ref_ROIHeight);

                    if (blnDebugImage)
                        objROI.SaveImage("D:\\TS\\1.objROI" + i.ToString() + ".bmp");
                    blnResult[i] = m_arrLineGauge[i].VerifyMeasurement_SubGaugePoints(objROI);
                    
                    if (!blnResult[i])
                        nCount--;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (blnResult[i])
                    CollectMeasuredGaugeResult_LineGaugeRecord(m_arrLineGauge[i], i);
            }

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);  // 2020 08 14 - CCENG - prevent out of index when use wrong recipe
                if (intROIIndex >= arrImage.Count)
                    intROIIndex = 0;

                objROI.AttachImage(arrImage[intROIIndex][i]);
                objROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX, m_arrEdgeROI[i].ref_ROIPositionY, m_arrEdgeROI[i].ref_ROIWidth, m_arrEdgeROI[i].ref_ROIHeight);
                if (blnDebugImage)
                    objROI.SaveImage("D:\\TS\\2.objROI" + i.ToString() + ".bmp");
                List<LineGaugeRecord> arrLineGaugeRecord = m_arrLineGauge[i].MeasureAndCollect_SubGaugePoints(objROI, m_arrGaugeMinScore[i], m_arrGaugeGroupTolerance[i]);

                for (int r = 0; r < arrLineGaugeRecord.Count; r++)
                {
                    m_arrLineGaugeRecord[i].Add(arrLineGaugeRecord[r]);
                }
            }
            bool blnRepeat = false;
            bool isResultOK = true;
            List<int> arrTopIndex = new List<int>();
            List<int> arrRightIndex = new List<int>();
            List<int> arrBottomIndex = new List<int>();
            List<int> arrLeftIndex = new List<int>();
            List<float> arrResultWidth = new List<float>();
            List<float> arrResultHeight = new List<float>();
            List<float[]> arrResultConnerGaugeAngle = new List<float[]>();
            List<PointF> arrResultPoint = new List<PointF>();
            Line[] arrCrossLines;

            for (int top = 0; top < m_arrLineGaugeRecord[0].Count; top++)
            {
                for (int right = 0; right < m_arrLineGaugeRecord[1].Count; right++)
                {
                    for (int bottom = 0; bottom < m_arrLineGaugeRecord[2].Count; bottom++)
                    {
                        for (int left = 0; left < m_arrLineGaugeRecord[3].Count; left++)
                        {
                            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                            // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will cross each other to generate corner point)
                            m_arrRectCornerPoints = new PointF[4];
                            m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGaugeRecord[0][top].objLine, m_arrLineGaugeRecord[3][left].objLine);
                            m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGaugeRecord[0][top].objLine, m_arrLineGaugeRecord[1][right].objLine);
                            m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGaugeRecord[1][right].objLine, m_arrLineGaugeRecord[2][bottom].objLine);
                            m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGaugeRecord[2][bottom].objLine, m_arrLineGaugeRecord[3][left].objLine);

                            // Get corner angle
                            float[] arrCornerGaugeAngles = new float[4];
                            arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGaugeRecord[0][top].objLine, m_arrLineGaugeRecord[3][left].objLine);
                            arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGaugeRecord[0][top].objLine, m_arrLineGaugeRecord[1][right].objLine);
                            arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGaugeRecord[1][right].objLine, m_arrLineGaugeRecord[2][bottom].objLine);
                            arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGaugeRecord[2][bottom].objLine, m_arrLineGaugeRecord[3][left].objLine);

                            // Verify corner angle
                            isResultOK = true;
                            int intBestAngleIndex = -1;
                            float fBestAngle = float.MaxValue;
                            bool[] arrAngleOK = new bool[4];
                            for (int i = 0; i < 4; i++)
                            {
                                float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                                if (fAngle > m_arrGaugeMaxAngle[i])
                                {
                                    if (isResultOK)
                                    {
                                        m_strErrorMessage = "*Measure Edge Fail. Corner angle out of tolerance. Setting=" + m_arrGaugeMaxAngle[i].ToString() + ", Result=" + fAngle.ToString();
                                        isResultOK = false;
                                    }
                                }
                                else
                                {
                                    arrAngleOK[i] = true;

                                    if (fAngle < fBestAngle)
                                    {
                                        fBestAngle = fAngle;
                                        intBestAngleIndex = i;
                                    }
                                }

                            }

                            if (isResultOK)
                            {
                                // Get rectangle corner close lines
                                arrCrossLines = new Line[2];
                                arrCrossLines[0] = new Line();
                                arrCrossLines[1] = new Line();
                                // Calculate center point
                                // -------- ver line ---------------------------------------
                                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                                // ---------- Hor line ------------

                                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                                // Verify Unit Size
                                //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                                PointF pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                                float fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;
                                float fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;
                                //else
                                //    isResultOK = false;

                                if (pRectCenterPoint.X.ToString() != "NaN" && pRectCenterPoint.Y.ToString() != "NaN" &&
                                                     pRectCenterPoint.X.ToString() != "-NaN" && pRectCenterPoint.Y.ToString() != "-NaN" &&
                                                     pRectCenterPoint.X.ToString() != "Infinity" && pRectCenterPoint.Y.ToString() != "Infinity" &&
                                                     pRectCenterPoint.X.ToString() != "-Infinity" && pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                                     fRectWidth != 0 && fRectHeight != 0)
                                {

                                    arrResultWidth.Add(fRectWidth);

                                    arrResultHeight.Add(fRectHeight);

                                    arrResultPoint.Add(pRectCenterPoint);
                                    arrTopIndex.Add(top);
                                    arrRightIndex.Add(right);
                                    arrBottomIndex.Add(bottom);
                                    arrLeftIndex.Add(left);

                                    arrResultConnerGaugeAngle.Add(arrCornerGaugeAngles);

                                    isResultOK = true;
                                }
                                else
                                {
                                    m_strErrorMessage = "Measure Edge Fail!";
                                    isResultOK = false;
                                }

                            }

                            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        }
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (m_arrLineGaugeRecord[i].Count > 0)
                    m_arrLineResultOK[i] = true;
                else
                    m_arrLineResultOK[i] = false;

            }

            int intSelectedIndex = -1;
            float fMinDifferent = float.MaxValue;
            // Loop to get the best result
            for (int i = 0; i < arrResultWidth.Count; i++)
            {
                float fWidthDiff = Math.Abs(arrResultWidth[i] - m_fUnitSizeWidth);
                float fHeightDiff = Math.Abs(arrResultHeight[i] - m_fUnitSizeHeight);

                if (fMinDifferent > Math.Max(fWidthDiff, fHeightDiff))
                {
                    fMinDifferent = Math.Max(fWidthDiff, fHeightDiff);
                    intSelectedIndex = i;
                }
            }

            int intAngleSelectedIndex = -1;
            float fMinAngle = float.MaxValue;
            // if other result have almost same with best result (<1 pixel), then choose which one have the smallest corner angle.
            for (int i = 0; i < arrResultConnerGaugeAngle.Count; i++)
            {
                // get different size between the best result and other
                float fWidthDiff = Math.Abs(arrResultWidth[i] - arrResultWidth[intSelectedIndex]);
                float fHeightDiff = Math.Abs(arrResultHeight[i] - arrResultHeight[intSelectedIndex]);

                // check corner angle if other result have < 1 pixel compare to the best result
                if (Math.Max(fWidthDiff, fHeightDiff) < 1)
                {
                    // Get the max angle from the 4 corner angle
                    float fMaxAngle = Math.Max(Math.Abs(arrResultConnerGaugeAngle[i][0] - 90), Math.Abs(arrResultConnerGaugeAngle[i][1] - 90));
                    fMaxAngle = Math.Max(fMaxAngle, Math.Abs(arrResultConnerGaugeAngle[i][2] - 90));
                    fMaxAngle = Math.Max(fMaxAngle, Math.Abs(arrResultConnerGaugeAngle[i][3] - 90));

                    // reselect best result with smallest angle.
                    if (fMinAngle > fMaxAngle)
                    {
                        fMinAngle = fMaxAngle;
                        intAngleSelectedIndex = i;
                    }
                }
            }

            intSelectedIndex = intAngleSelectedIndex;
            

            if (intSelectedIndex >= 0)
            {
                {
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    if (m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].bUseSubGauge)
                    {
                        int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[0], m_intVisionIndex);  // 2020 08 14 - CCENG - prevent out of index when use wrong recipe
                        if (intROIIndex >= arrImage.Count)
                            intROIIndex = 0;

                        objROI.AttachImage(arrImage[intROIIndex][0]);
                        objROI.LoadROISetting(m_arrEdgeROI[0].ref_ROIPositionX, m_arrEdgeROI[0].ref_ROIPositionY, m_arrEdgeROI[0].ref_ROIWidth, m_arrEdgeROI[0].ref_ROIHeight);
                        if (blnDebugImage)
                            objROI.SaveImage("D:\\TS\\3.objROI" + 0.ToString() + ".bmp");
                        m_arrLineGauge[0].SetParameterToSubGauge(objROI,
                            m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]]);
                    }

                    if (m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].bUseSubGauge)
                    {
                        int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[1], m_intVisionIndex);  // 2020 08 14 - CCENG - prevent out of index when use wrong recipe
                        if (intROIIndex >= arrImage.Count)
                            intROIIndex = 0;

                        objROI.AttachImage(arrImage[intROIIndex][1]);
                        objROI.LoadROISetting(m_arrEdgeROI[1].ref_ROIPositionX, m_arrEdgeROI[1].ref_ROIPositionY, m_arrEdgeROI[1].ref_ROIWidth, m_arrEdgeROI[1].ref_ROIHeight);
                        if (blnDebugImage)
                            objROI.SaveImage("D:\\TS\\3.objROI" + 1.ToString() + ".bmp");
                        m_arrLineGauge[1].SetParameterToSubGauge(objROI,
                            m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]]);
                    }

                    if (m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].bUseSubGauge)
                    {
                        int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[2], m_intVisionIndex);  // 2020 08 14 - CCENG - prevent out of index when use wrong recipe
                        if (intROIIndex >= arrImage.Count)
                            intROIIndex = 0;

                        objROI.AttachImage(arrImage[intROIIndex][2]);
                        objROI.LoadROISetting(m_arrEdgeROI[2].ref_ROIPositionX, m_arrEdgeROI[2].ref_ROIPositionY, m_arrEdgeROI[2].ref_ROIWidth, m_arrEdgeROI[2].ref_ROIHeight);
                        if (blnDebugImage)
                            objROI.SaveImage("D:\\TS\\3.objROI" + 2.ToString() + ".bmp");
                        m_arrLineGauge[2].SetParameterToSubGauge(objROI,
                            m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]]);
                    }

                    if (m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].bUseSubGauge)
                    {
                        int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[3], m_intVisionIndex);  // 2020 08 14 - CCENG - prevent out of index when use wrong recipe
                        if (intROIIndex >= arrImage.Count)
                            intROIIndex = 0;

                        objROI.AttachImage(arrImage[intROIIndex][3]);
                        objROI.LoadROISetting(m_arrEdgeROI[3].ref_ROIPositionX, m_arrEdgeROI[3].ref_ROIPositionY, m_arrEdgeROI[3].ref_ROIWidth, m_arrEdgeROI[3].ref_ROIHeight);
                        if (blnDebugImage)
                            objROI.SaveImage("D:\\TS\\3.objROI" + 3.ToString() + ".bmp");
                        m_arrLineGauge[3].SetParameterToSubGauge(objROI,
                            m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]]);
                    }

                    FilterEdgeAngle_Pad_ImproveAngleVersion(m_arrLineGauge, 50, ref fFinalAngle, ref nCount, ref blnResult);
                    
                    m_arrLineResultOK = blnResult;

                    //if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
                    //{
                    //    m_strErrorMessage = "Measure edge fail.";
                    //    m_intFailResultMask |= 0x02;
                    //    return false;
                    //}
                    //if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
                    //{
                    //    m_strErrorMessage = "Measure edge fail.";
                    //    m_intFailResultMask |= 0x02;
                    //    return false;
                    //}
                    //if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
                    //{
                    //    m_strErrorMessage = "Measure edge fail.";
                    //    m_intFailResultMask |= 0x02;
                    //    return false;
                    //}

                    // Get average angle from Edge Line Gauges Angle
                    m_fRectAngle = fFinalAngle;// fTotalAngle / nCount;

                    // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                    m_arrRectCornerPoints = new PointF[4];
                    m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine, m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine);
                    m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine, m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine);
                    m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine, m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine);
                    m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine, m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine);

                    // Get corner angle
                    float[] arrCornerGaugeAngles = new float[4];
                    arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine, m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine);
                    arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine, m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine);
                    arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine, m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine);
                    arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine, m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine);

                    //if (blnEnableVirtualLine)
                    {
                        // Verify corner angle
                        isResultOK = true;
                        int intBestAngleIndex = -1;
                        float fBestAngle = float.MaxValue;
                        bool[] arrAngleOK = new bool[4];
                        for (int i = 0; i < 4; i++)
                        {
                            float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                            if (fAngle > m_arrGaugeMaxAngle[i])
                            {
                                if (isResultOK)
                                {
                                    m_strErrorMessage = "*Measure Edge Fail. Corner angle out of tolerance. Setting=" + m_arrGaugeMaxAngle[i].ToString() + ", Result=" + fAngle.ToString();
                                    isResultOK = false;
                                }
                            }
                            else
                            {
                                arrAngleOK[i] = true;

                                if (fAngle < fBestAngle)
                                {
                                    fBestAngle = fAngle;
                                    intBestAngleIndex = i;
                                }
                            }

                        }

                        if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                        {
                            blnRepeat = true;

                            int intBestVerIndex = -1, intBestHorIndex = -1;
                            switch (intBestAngleIndex)
                            {
                                case 0:
                                    intBestHorIndex = 0;
                                    intBestVerIndex = 3;
                                    break;
                                case 1:
                                    intBestHorIndex = 0;
                                    intBestVerIndex = 1;
                                    break;
                                case 2:
                                    intBestHorIndex = 2;
                                    intBestVerIndex = 1;
                                    break;
                                case 3:
                                    intBestHorIndex = 2;
                                    intBestVerIndex = 3;
                                    break;
                            }

                            // check can use the good angle to build the virtual line
                            if (!arrAngleOK[0]) // Top Left angle
                            {
                                if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                                {
                                    Line objLine = new Line();
                                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                                }

                                if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                                {
                                    Line objLine = new Line();
                                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);//-, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                                }

                                arrAngleOK[0] = true;
                            }

                            if (!arrAngleOK[1]) // Top Right angle
                            {
                                if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                                {
                                    Line objLine = new Line();
                                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                                }
                                if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                                {
                                    Line objLine = new Line();
                                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);//+, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                                }

                                arrAngleOK[1] = true;
                            }

                            if (!arrAngleOK[2]) // Top Right angle
                            {
                                if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                                {
                                    Line objLine = new Line();
                                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                                }
                                if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                                {
                                    Line objLine = new Line();
                                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);//+, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                                }

                                arrAngleOK[2] = true;
                            }

                            if (!arrAngleOK[3]) // Top Right angle
                            {
                                if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                                {
                                    Line objLine = new Line();
                                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                                }
                                if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                                {
                                    Line objLine = new Line();
                                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);//-, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                                }

                                arrAngleOK[3] = true;
                            }
                        }
                    }

                    if (nCount < 4)
                    {
                        isResultOK = false;
                        m_strErrorMessage = "*Measure Edge Fail.";
                        //m_intFailResultMask |= 0x02;
                        //return false;
                    }


                    if (isResultOK)
                    {

                        // Get rectangle corner close lines
                        arrCrossLines = new Line[2];
                        arrCrossLines[0] = new Line();

                        arrCrossLines[1] = new Line();

                        // Calculate center point
                        // -------- ver line ---------------------------------------
                        arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                        m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                        m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                        m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                        // ---------- Hor line ------------

                        arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                        m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                        m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                        m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                        if (blnWantCalculateBasedOnPadIndex)
                        {
                            switch (intPadIndex)
                            {
                                case 0:
                                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                                    (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                                    break;
                                case 1:
                                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                                    (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                                    break;
                                case 2:
                                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                                    (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                                    break;
                                case 3:
                                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                                    (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                                    break;
                                case 4:
                                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                                    (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                                    break;
                            }
                        }
                        else
                        {
                            // Verify Unit Size
                            //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                            //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            //else
                            //    isResultOK = false;
                        }
                    }

                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    m_arrLineResultOK[i] = false;
                }
                isResultOK = false;
            }
            objROI.Dispose();
            if (!isResultOK)
            {
                m_fRectHeight = 0;
                m_fRectWidth = 0;
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_BlobMethod(List<List<ImageDrawing>> arrImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex)
        {
            // Init data
            float fFinalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                int intSelectedLine = GetLineGaugeSelectedIndex(intPadIndex, i);
                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intImageIndex >= arrImage.Count)
                    intImageIndex = 0;
             
                switch (intPadIndex)
                {
                    case 0:
                    case 1:
                    case 3:
                        if (i == 0 || i == 2 || intPadIndex == 0)
                        {
                            m_arrLineGauge[i].Measure(arrImage[intImageIndex][i]);

                            if (!m_arrAutoDontCare[intSelectedLine])
                            {
                                if (m_arrDontCareROI[i].Count > 0)
                                {
                                    float fDontCareLengthTotal = 0;
                                    if (i == 0 || i == 2)   // Top and Bottom Edge ROI
                                    {
                                        for (int c = 0; c < m_arrDontCareROI[i].Count; c++)
                                        {
                                            fDontCareLengthTotal += m_arrDontCareROI[i][c].ref_ROIWidth;
                                        }
                                    }
                                    else // Left and Right Edge ROI
                                    {
                                        for (int c = 0; c < m_arrDontCareROI[i].Count; c++)
                                        {
                                            fDontCareLengthTotal += m_arrDontCareROI[i][c].ref_ROIHeight;
                                        }
                                    }

                                    float fPassLength = m_arrLineGauge[i].ref_ObjectScore / 100 * m_arrLineGauge[i].ref_GaugeLength;
                                    float fMeasuredLength = m_arrLineGauge[i].ref_GaugeLength - fDontCareLengthTotal;

                                    float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

                                    if (fFinalMeasureScore < m_arrGaugeMinScore[i])
                                    {
                                        blnResult[i] = false;
                                    }
                                    else
                                    {
                                        blnResult[i] = true;
                                        nCount++;
                                        m_arrLineGauge[i].GetObjectLine();
                                    }
                                }
                                else
                                {
                                    //if ((arrLineGauges[i].ref_GaugeThickness < 50 && arrLineGauges[i].ref_ObjectScore < 20) ||
                                    //    (arrLineGauges[i].ref_GaugeThickness >= 50 && arrLineGauges[i].ref_ObjectScore < 30))
                                    if (m_arrLineGauge[i].ref_ObjectScore < m_arrGaugeMinScore[i])
                                    {
                                        blnResult[i] = false;
                                    }
                                    else
                                    {
                                        blnResult[i] = true;
                                        nCount++;
                                        m_arrLineGauge[i].GetObjectLine();
                                    }
                                }
                            }
                            else
                            {
                                float fDontCareLengthTotal = m_arrAutoDontCareTotalLength[intSelectedLine];
                          
                                float fPassLength = m_arrLineGauge[i].ref_ObjectScore / 100 * m_arrLineGauge[i].ref_GaugeLength;
                                float fMeasuredLength = m_arrLineGauge[i].ref_GaugeLength - fDontCareLengthTotal;

                                float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

                                if (fFinalMeasureScore < m_arrGaugeMinScore[i])
                                {
                                    blnResult[i] = false;
                                }
                                else
                                {
                                    blnResult[i] = true;
                                    nCount++;
                                    m_arrLineGauge[i].GetObjectLine();
                                }
                            }
                        }
                        else
                        {
                            //2021-01-18 ZJYEOH : Black On White or White On Black decide by direction and transition type
                            bool blnBlackOnWhite = true;

                            if (i == 3)
                            {
                                if (m_arrLineGauge[i].ref_GaugeTransType == 0) // Black to White
                                {
                                    if (m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = true;
                                    }
                                    else if (!m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = false;
                                    }
                                }
                                else // White to Black
                                {
                                    if (!m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = true;
                                    }
                                    else if (m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = false;
                                    }
                                }
                            }
                            else if (i == 1)
                            {
                                if (m_arrLineGauge[i].ref_GaugeTransType == 0) // Black to White
                                {
                                    if (m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = true;
                                    }
                                    else if (!m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = false;
                                    }
                                }
                                else // White to Black
                                {
                                    if (!m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = true;
                                    }
                                    else if (m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = false;
                                    }
                                }
                            }

                            m_objBlob.BuildObjects_Filter_GetElement(m_arrEdgeROI[i], blnBlackOnWhite, true, 0, m_arrGaugeImageThreshold[i],
                    20, m_arrEdgeROI[i].ref_ROIWidth * m_arrEdgeROI[i].ref_ROIHeight, false, 0xFF);

                            if (m_objBlob.ref_intNumSelectedObject > 0)
                            {
                                if (i == 1)
                                {
                                    int intIndex = 0;
                                    int intTiltAngle = m_intGaugeTiltAngle;
                                    if (intPadIndex == 0 || intPadIndex == 1)
                                    {
                                        intIndex = m_objBlob.GetLeftBlob(); //GetTopBlob
                                    }
                                    else
                                    {
                                        intIndex = m_objBlob.GetLeftBlob(); //GetBottomBlob
                                        intTiltAngle = -intTiltAngle;
                                    }

                                    float fAngle = 180 + intTiltAngle;
                                    if (m_blnInToOut[i])
                                        fAngle = 0 + intTiltAngle;

                                    if (intPadIndex == 0 || intPadIndex == 1)
                                        m_arrPointGauge[i].SetGaugePlacement(m_arrEdgeROI[i].ref_ROIPositionX + m_objBlob.ref_arrLimitCenterX[intIndex] + m_objBlob.ref_arrWidth[intIndex] / 4,
                                        m_arrEdgeROI[i].ref_ROIPositionY + m_objBlob.ref_arrLimitCenterY[intIndex] - m_objBlob.ref_arrHeight[intIndex] / 2 + m_arrGaugeOffset[i],
                                        (m_objBlob.ref_arrWidth[intIndex] * 3) / 4, fAngle);
                                    else
                                        m_arrPointGauge[i].SetGaugePlacement(m_arrEdgeROI[i].ref_ROIPositionX + m_objBlob.ref_arrLimitCenterX[intIndex] + m_objBlob.ref_arrWidth[intIndex] / 4,
                                       m_arrEdgeROI[i].ref_ROIPositionY + m_objBlob.ref_arrLimitCenterY[intIndex] + m_objBlob.ref_arrHeight[intIndex] / 2 - m_arrGaugeOffset[i],
                                       (m_objBlob.ref_arrWidth[intIndex] * 3) / 4, fAngle);

                                    ROI objROI = new ROI();
                                    objROI.AttachImage(arrImage[intImageIndex][i]);
                                    objROI.LoadROISetting(0, 0, arrImage[intImageIndex][i].ref_intImageWidth, arrImage[intImageIndex][i].ref_intImageHeight);
                                    m_arrPointGauge[i].Measure(objROI);
                                    objROI.Dispose();
                                    if (m_arrPointGauge[i].ref_intMeasuredPointCount == 0)
                                        blnResult[i] = false;
                                    else
                                    {
                                        blnResult[i] = true;
                                        nCount++;
                                        m_arrLineGauge[i].GetObjectLine(m_arrPointGauge[i].GetMeasurePoint(0).X, m_arrPointGauge[i].GetMeasurePoint(0).Y, m_arrLineGauge[i].ref_GaugeAngle);
                                        m_arrLineGauge[i].SetGaugeCenter(m_arrPointGauge[i].GetMeasurePoint(0).X, m_arrPointGauge[i].GetMeasurePoint(0).Y);
                                    }

                                    //objROI = new ROI();
                                    //objROI.AttachImage(arrImage[intImageIndex][i]);
                                    //objROI.LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX, m_arrEdgeROI[i].ref_ROIPositionY, m_arrEdgeROI[i].ref_ROIWidth, m_arrEdgeROI[i].ref_ROIHeight);
                                    //objROI.SaveImage("D:\\objROI.bmp");

                                    //Contour objContour = new Contour();
                                    //objContour.ClearContour();
                                    //objContour.BuildContour(objROI, (int)Math.Round(m_arrPointGauge[i].GetMeasurePoint(0).X - m_arrEdgeROI[i].ref_ROIPositionX), (int)Math.Round(m_arrPointGauge[i].GetMeasurePoint(0).Y - m_arrEdgeROI[i].ref_ROIPositionY),m_arrGaugeImageThreshold[i],4);
                                    //objContour.SaveBlobsContour("D:\\Contour.xml");
                                    //m_arrEdgeROI[1].SaveImage("D:\\m_arrEdgeROI_Right.bmp");
                                    //m_arrEdgeROI[3].SaveImage("D:\\m_arrEdgeROI_Left.bmp");
                                    //objROI.Dispose();
                                }
                                else
                                {
                                    int intIndex = 0;
                                    int intTiltAngle = m_intGaugeTiltAngle;
                                    if (intPadIndex == 0 || intPadIndex == 1)
                                    {
                                        intIndex = m_objBlob.GetRightBlob();//GetTopBlob
                                        intTiltAngle = -intTiltAngle;
                                    }
                                    else
                                    {
                                        intIndex = m_objBlob.GetRightBlob();//GetBottomBlob
                                    }
                                    
                                    float fAngle = 0 + intTiltAngle;
                                    if (m_blnInToOut[i])
                                        fAngle = 180 + intTiltAngle;

                                    if (intPadIndex == 0 || intPadIndex == 1)
                                        m_arrPointGauge[i].SetGaugePlacement(m_arrEdgeROI[i].ref_ROIPositionX + m_objBlob.ref_arrLimitCenterX[intIndex] - m_objBlob.ref_arrWidth[intIndex] / 4,
                                        m_arrEdgeROI[i].ref_ROIPositionY + m_objBlob.ref_arrLimitCenterY[intIndex] - m_objBlob.ref_arrHeight[intIndex] / 2 + m_arrGaugeOffset[i],
                                        (m_objBlob.ref_arrWidth[intIndex] * 3) / 4, fAngle);
                                    else
                                        m_arrPointGauge[i].SetGaugePlacement(m_arrEdgeROI[i].ref_ROIPositionX + m_objBlob.ref_arrLimitCenterX[intIndex] - m_objBlob.ref_arrWidth[intIndex] / 4,
                                        m_arrEdgeROI[i].ref_ROIPositionY + m_objBlob.ref_arrLimitCenterY[intIndex] + m_objBlob.ref_arrHeight[intIndex] / 2 - m_arrGaugeOffset[i],
                                        (m_objBlob.ref_arrWidth[intIndex] * 3) / 4, fAngle);

                                    ROI objROI = new ROI();
                                    objROI.AttachImage(arrImage[intImageIndex][i]);
                                    objROI.LoadROISetting(0, 0, arrImage[intImageIndex][i].ref_intImageWidth, arrImage[intImageIndex][i].ref_intImageHeight);
                                    m_arrPointGauge[i].Measure(objROI);
                                    objROI.Dispose();
                                    if (m_arrPointGauge[i].ref_intMeasuredPointCount == 0)
                                        blnResult[i] = false;
                                    else
                                    {
                                        blnResult[i] = true;
                                        nCount++;
                                        m_arrLineGauge[i].GetObjectLine(m_arrPointGauge[i].GetMeasurePoint(0).X, m_arrPointGauge[i].GetMeasurePoint(0).Y, m_arrLineGauge[i].ref_GaugeAngle);
                                        m_arrLineGauge[i].SetGaugeCenter(m_arrPointGauge[i].GetMeasurePoint(0).X, m_arrPointGauge[i].GetMeasurePoint(0).Y);
                                    }
                                }
                            }
                            else
                            {
                                blnResult[i] = false;
                            }
                        }
                        break;
                    case 2:
                    case 4:
                        if (i == 1 || i == 3)
                        {
                            m_arrLineGauge[i].Measure(arrImage[intImageIndex][i]);
                            if (!m_arrAutoDontCare[intSelectedLine])
                            {
                                if (m_arrDontCareROI[i].Count > 0)
                                {
                                    float fDontCareLengthTotal = 0;
                                    if (i == 0 || i == 2)   // Top and Bottom Edge ROI
                                    {
                                        for (int c = 0; c < m_arrDontCareROI[i].Count; c++)
                                        {
                                            fDontCareLengthTotal += m_arrDontCareROI[i][c].ref_ROIWidth;
                                        }
                                    }
                                    else // Left and Right Edge ROI
                                    {
                                        for (int c = 0; c < m_arrDontCareROI[i].Count; c++)
                                        {
                                            fDontCareLengthTotal += m_arrDontCareROI[i][c].ref_ROIHeight;
                                        }
                                    }

                                    float fPassLength = m_arrLineGauge[i].ref_ObjectScore / 100 * m_arrLineGauge[i].ref_GaugeLength;
                                    float fMeasuredLength = m_arrLineGauge[i].ref_GaugeLength - fDontCareLengthTotal;

                                    float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

                                    if (fFinalMeasureScore < m_arrGaugeMinScore[i])
                                    {
                                        blnResult[i] = false;
                                    }
                                    else
                                    {
                                        blnResult[i] = true;
                                        nCount++;
                                        m_arrLineGauge[i].GetObjectLine();
                                    }
                                }
                                else
                                {
                                    //if ((arrLineGauges[i].ref_GaugeThickness < 50 && arrLineGauges[i].ref_ObjectScore < 20) ||
                                    //    (arrLineGauges[i].ref_GaugeThickness >= 50 && arrLineGauges[i].ref_ObjectScore < 30))
                                    if (m_arrLineGauge[i].ref_ObjectScore < m_arrGaugeMinScore[i])
                                    {
                                        blnResult[i] = false;
                                    }
                                    else
                                    {
                                        blnResult[i] = true;
                                        nCount++;
                                        m_arrLineGauge[i].GetObjectLine();
                                    }
                                }
                            }
                            else
                            {
                                float fDontCareLengthTotal = m_arrAutoDontCareTotalLength[intSelectedLine];

                                float fPassLength = m_arrLineGauge[i].ref_ObjectScore / 100 * m_arrLineGauge[i].ref_GaugeLength;
                                float fMeasuredLength = m_arrLineGauge[i].ref_GaugeLength - fDontCareLengthTotal;

                                float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

                                if (fFinalMeasureScore < m_arrGaugeMinScore[i])
                                {
                                    blnResult[i] = false;
                                }
                                else
                                {
                                    blnResult[i] = true;
                                    nCount++;
                                    m_arrLineGauge[i].GetObjectLine();
                                }
                            }
                        }
                        else
                        {
                            //2021-01-18 ZJYEOH : Black On White or White On Black decide by direction and transition type
                            bool blnBlackOnWhite = true;

                            if (i == 0)
                            {
                                if (m_arrLineGauge[i].ref_GaugeTransType == 0) // Black to White
                                {
                                    if (m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = true;
                                    }
                                    else if (!m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = false;
                                    }
                                }
                                else // White to Black
                                {
                                    if (!m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = true;
                                    }
                                    else if (m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = false;
                                    }
                                }
                            }
                            else if (i == 2)
                            {
                                if (m_arrLineGauge[i].ref_GaugeTransType == 0) // Black to White
                                {
                                    if (m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = true;
                                    }
                                    else if (!m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = false;
                                    }
                                }
                                else // White to Black
                                {
                                    if (!m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = true;
                                    }
                                    else if (m_blnInToOut[i])
                                    {
                                        blnBlackOnWhite = false;
                                    }
                                }
                            }

                            m_objBlob.BuildObjects_Filter_GetElement(m_arrEdgeROI[i], blnBlackOnWhite, true, 0, m_arrGaugeImageThreshold[i],
                    20, m_arrEdgeROI[i].ref_ROIWidth * m_arrEdgeROI[i].ref_ROIHeight, false, 0xFF);

                            if (m_objBlob.ref_intNumSelectedObject > 0)
                            {
                                if (i == 0)
                                {
                                    int intIndex = 0;
                                    int intTiltAngle = m_intGaugeTiltAngle;
                                    if (intPadIndex == 2)
                                    {
                                        intIndex = m_objBlob.GetBottomBlob();//GetRightBlob
                                        intTiltAngle = -intTiltAngle;
                                    }
                                    else
                                    {
                                        intIndex = m_objBlob.GetBottomBlob();//GetLeftBlob
                                    }

                                    float fAngle = 90 + intTiltAngle;
                                    if (m_blnInToOut[i])
                                        fAngle = -90 + intTiltAngle;

                                    if (intPadIndex == 2)
                                        m_arrPointGauge[i].SetGaugePlacement(m_arrEdgeROI[i].ref_ROIPositionX + m_objBlob.ref_arrLimitCenterX[intIndex] + m_objBlob.ref_arrWidth[intIndex] / 2 - m_arrGaugeOffset[i],
                                        m_arrEdgeROI[i].ref_ROIPositionY + m_objBlob.ref_arrLimitCenterY[intIndex] - m_objBlob.ref_arrHeight[intIndex] / 4,
                                        (m_objBlob.ref_arrHeight[intIndex] * 3) / 4, fAngle);
                                    else
                                        m_arrPointGauge[i].SetGaugePlacement(m_arrEdgeROI[i].ref_ROIPositionX + m_objBlob.ref_arrLimitCenterX[intIndex] - m_objBlob.ref_arrWidth[intIndex] / 2 + m_arrGaugeOffset[i],
                                          m_arrEdgeROI[i].ref_ROIPositionY + m_objBlob.ref_arrLimitCenterY[intIndex] - m_objBlob.ref_arrHeight[intIndex] / 4,
                                          (m_objBlob.ref_arrHeight[intIndex] * 3) / 4, fAngle);

                                    ROI objROI = new ROI();
                                    objROI.AttachImage(arrImage[intImageIndex][i]);
                                    objROI.LoadROISetting(0, 0, arrImage[intImageIndex][i].ref_intImageWidth, arrImage[intImageIndex][i].ref_intImageHeight);
                                    m_arrPointGauge[i].Measure(objROI);
                                    objROI.Dispose();
                                    if (m_arrPointGauge[i].ref_intMeasuredPointCount == 0)
                                        blnResult[i] = false;
                                    else
                                    {
                                        blnResult[i] = true;
                                        nCount++;
                                        m_arrLineGauge[i].GetObjectLine(m_arrPointGauge[i].GetMeasurePoint(0).X, m_arrPointGauge[i].GetMeasurePoint(0).Y, m_arrLineGauge[i].ref_GaugeAngle);
                                        m_arrLineGauge[i].SetGaugeCenter(m_arrPointGauge[i].GetMeasurePoint(0).X, m_arrPointGauge[i].GetMeasurePoint(0).Y);
                                    }
                                }
                                else
                                {
                                    int intIndex = 0;
                                    int intTiltAngle = m_intGaugeTiltAngle;
                                    if (intPadIndex == 2)
                                    {
                                        intIndex = m_objBlob.GetTopBlob();//GetRightBlob
                                    }
                                    else
                                    {
                                        intIndex = m_objBlob.GetTopBlob();//GetLeftBlob
                                        intTiltAngle = -intTiltAngle;
                                    }

                                    float fAngle = -90 + intTiltAngle;
                                    if (m_blnInToOut[i])
                                        fAngle = 90 + intTiltAngle;
                                    
                                    if (intPadIndex == 2)
                                        m_arrPointGauge[i].SetGaugePlacement(m_arrEdgeROI[i].ref_ROIPositionX + m_objBlob.ref_arrLimitCenterX[intIndex] + m_objBlob.ref_arrWidth[intIndex] / 2 - m_arrGaugeOffset[i],
                                        m_arrEdgeROI[i].ref_ROIPositionY + m_objBlob.ref_arrLimitCenterY[intIndex] + m_objBlob.ref_arrHeight[intIndex] / 4,
                                        (m_objBlob.ref_arrHeight[intIndex] * 3) / 4, fAngle);
                                    else
                                        m_arrPointGauge[i].SetGaugePlacement(m_arrEdgeROI[i].ref_ROIPositionX + m_objBlob.ref_arrLimitCenterX[intIndex] - m_objBlob.ref_arrWidth[intIndex] / 2 + m_arrGaugeOffset[i],
                                         m_arrEdgeROI[i].ref_ROIPositionY + m_objBlob.ref_arrLimitCenterY[intIndex] + m_objBlob.ref_arrHeight[intIndex] / 4,
                                         (m_objBlob.ref_arrHeight[intIndex] * 3) / 4, fAngle);

                                    ROI objROI = new ROI();
                                    objROI.AttachImage(arrImage[intImageIndex][i]);
                                    objROI.LoadROISetting(0, 0, arrImage[intImageIndex][i].ref_intImageWidth, arrImage[intImageIndex][i].ref_intImageHeight);
                                    m_arrPointGauge[i].Measure(objROI);
                                    objROI.Dispose();
                                    if (m_arrPointGauge[i].ref_intMeasuredPointCount == 0)
                                        blnResult[i] = false;
                                    else
                                    {
                                        blnResult[i] = true;
                                        nCount++;
                                        m_arrLineGauge[i].GetObjectLine(m_arrPointGauge[i].GetMeasurePoint(0).X, m_arrPointGauge[i].GetMeasurePoint(0).Y, m_arrLineGauge[i].ref_GaugeAngle);
                                        m_arrLineGauge[i].SetGaugeCenter(m_arrPointGauge[i].GetMeasurePoint(0).X, m_arrPointGauge[i].GetMeasurePoint(0).Y);
                                    }
                                }
                            }
                            else
                            {
                                blnResult[i] = false;
                            }
                        }
                        break;
                }
                //m_arrLineGauge[i].Measure(arrImage[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            //// Check is Edge Line Gauge measurement good
            //FilterEdgeAngle_Pad_ImproveAngleVersion(m_arrLineGauge, 50, ref fFinalAngle, ref nCount, ref blnResult);

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            bool blnSingleAnglePass = false;
            if (intPadIndex == 1)
            {
                if (blnResult[0])
                {
                    m_fRectAngle = m_arrLineGauge[0].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 2)
            {
                if (blnResult[1])
                {
                    m_fRectAngle = m_arrLineGauge[1].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 3)
            {
                if (blnResult[2])
                {
                    m_fRectAngle = m_arrLineGauge[2].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 4)
            {
                if (blnResult[3])
                {
                    m_fRectAngle = m_arrLineGauge[3].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }

            if (!blnSingleAnglePass)
            {
                // Get average angle from Edge Line Gauges Angle
                m_fRectAngle = fFinalAngle;
            }

            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                }
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go down
                }
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                }
            }
            bool blnRepeat = false;
            bool isResultOK = true;

            // ------------------- checking loop timeout ---------------------------------------------------
            HiPerfTimer timeout = new HiPerfTimer();
            timeout.Start();

            do
            {
                // ------------------- checking loop timeout ---------------------------------------------------
                if (timeout.Timing > 10000)
                {
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 3002");
                    break;
                }
                // ---------------------------------------------------------------------------------------------

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                // Get corner angle
                float[] arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
                arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
                arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

                if (blnEnableVirtualLine)
                {

                    // Verify corner angle
                    isResultOK = true;
                    int intBestAngleIndex = -1;
                    float fBestAngle = float.MaxValue;
                    bool[] arrAngleOK = new bool[4];
                    for (int i = 0; i < 4; i++)
                    {

                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                        if (fAngle > m_arrGaugeMaxAngle[i])
                        {
                            if (isResultOK)
                                isResultOK = false;
                        }
                        else
                        {
                            arrAngleOK[i] = true;

                            if (fAngle < fBestAngle)
                            {
                                fBestAngle = fAngle;
                                intBestAngleIndex = i;
                            }
                        }

                    }


                    if (!isResultOK && intBestAngleIndex >= 0 && !blnRepeat)    // if angle result not good
                    {
                        blnRepeat = true;

                        int intBestVerIndex = -1, intBestHorIndex = -1;
                        switch (intBestAngleIndex)
                        {
                            case 0:
                                intBestHorIndex = 0;
                                intBestVerIndex = 3;
                                break;
                            case 1:
                                intBestHorIndex = 0;
                                intBestVerIndex = 1;
                                break;
                            case 2:
                                intBestHorIndex = 2;
                                intBestVerIndex = 1;
                                break;
                            case 3:
                                intBestHorIndex = 2;
                                intBestVerIndex = 3;
                                break;
                        }

                        // check can use the good angle to build the virtual line
                        if (!arrAngleOK[0]) // Top Left angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }

                            if (!arrAngleOK[3] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[0] = true;
                        }

                        if (!arrAngleOK[1]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 0 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[0].ref_ObjectLine = objLine;
                                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[1] = true;
                        }

                        if (!arrAngleOK[2]) // Top Right angle
                        {
                            if (!arrAngleOK[1] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[3] || (intBestVerIndex != 1 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[1].ref_ObjectLine = objLine;
                                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth);
                            }

                            arrAngleOK[2] = true;
                        }

                        if (!arrAngleOK[3]) // Top Right angle
                        {
                            if (!arrAngleOK[0] || (intBestHorIndex != 2 && intBestHorIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[2].ref_ObjectLine = objLine;
                                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                            }
                            if (!arrAngleOK[2] || (intBestVerIndex != 3 && intBestVerIndex != -1))
                            {
                                Line objLine = new Line();
                                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                                m_arrLineGauge[3].ref_ObjectLine = objLine;
                                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);
                            }

                            arrAngleOK[3] = true;
                        }
                    }
                    else
                        break;
                }
                else
                    break;

            } while (blnRepeat);
            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

            // Verify Unit Size

            if (!blnEnableVirtualLine)
            {
                if (nCount < 4)
                {
                    isResultOK = false;
                    m_strErrorMessage = "*Measure Edge Fail.";
                    //m_intFailResultMask |= 0x02;
                    //return false;
                }
            }

            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }

        private bool GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_CornerPointGaugeTB(List<List<ImageDrawing>> arrImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex,
            bool bUseTemplateUnitSize, float fTemplateUnitSizeX, float fTemplateUnitSizeY)
        {
            // Init data
            float fFinalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 1; i < 4; i += 2)    // scan 1 (right) and 3 (Left)
            {
                int intSelectedLine = GetLineGaugeSelectedIndex(intPadIndex, i);
                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intImageIndex >= arrImage.Count)
                    intImageIndex = 0;

                m_arrLineGauge[i].Measure(arrImage[intImageIndex][i]);

                if (!m_arrAutoDontCare[intSelectedLine])
                {
                    if (m_arrDontCareROI[i].Count > 0)
                    {
                        float fDontCareLengthTotal = 0;
                        if (i == 0 || i == 2)   // Top and Bottom Edge ROI
                        {
                            for (int c = 0; c < m_arrDontCareROI[i].Count; c++)
                            {
                                fDontCareLengthTotal += m_arrDontCareROI[i][c].ref_ROIWidth;
                            }
                        }
                        else // Left and Right Edge ROI
                        {
                            for (int c = 0; c < m_arrDontCareROI[i].Count; c++)
                            {
                                fDontCareLengthTotal += m_arrDontCareROI[i][c].ref_ROIHeight;
                            }
                        }

                        float fPassLength = m_arrLineGauge[i].ref_ObjectScore / 100 * m_arrLineGauge[i].ref_GaugeLength;
                        float fMeasuredLength = m_arrLineGauge[i].ref_GaugeLength - fDontCareLengthTotal;

                        float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

                        if (fFinalMeasureScore < m_arrGaugeMinScore[i])
                        {
                            blnResult[i] = false;
                        }
                        else
                        {
                            blnResult[i] = true;
                            nCount++;
                            m_arrLineGauge[i].GetObjectLine();
                        }
                    }
                    else
                    {
                        //if ((arrLineGauges[i].ref_GaugeThickness < 50 && arrLineGauges[i].ref_ObjectScore < 20) ||
                        //    (arrLineGauges[i].ref_GaugeThickness >= 50 && arrLineGauges[i].ref_ObjectScore < 30))
                        if (m_arrLineGauge[i].ref_ObjectScore < m_arrGaugeMinScore[i])
                        {
                            blnResult[i] = false;
                        }
                        else
                        {
                            blnResult[i] = true;
                            nCount++;
                            m_arrLineGauge[i].GetObjectLine();
                        }
                    }
                }
                else
                {
                    float fDontCareLengthTotal = m_arrAutoDontCareTotalLength[intSelectedLine];

                    float fPassLength = m_arrLineGauge[i].ref_ObjectScore / 100 * m_arrLineGauge[i].ref_GaugeLength;
                    float fMeasuredLength = m_arrLineGauge[i].ref_GaugeLength - fDontCareLengthTotal;

                    float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

                    if (fFinalMeasureScore < m_arrGaugeMinScore[i])
                    {
                        m_arrLineResultOK[i] = false;
                        blnResult[i] = false;
                    }
                    else
                    {
                        m_arrLineResultOK[i] = true;
                        blnResult[i] = true;
                        nCount++;
                        m_arrLineGauge[i].GetObjectLine();
                    }
                }
            }

            // Corner Ling Gauge TB mode is rely on left and right line gauge. If either one left or right line gauge fail, then will fail overall.
            if (!blnResult[1])
            {
                m_strErrorMessage = "*Measure right edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            if (!blnResult[3])   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure left edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            m_arrLineGaugeRecord[1].Clear();
            m_arrLineGaugeRecord[3].Clear();
            CollectMeasuredGaugeResult_LineGaugeRecord(m_arrLineGauge[1], 1);
            CollectMeasuredGaugeResult_LineGaugeRecord(m_arrLineGauge[3], 3);

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 3; i += 2)    // scan 0 and 2
            {
                m_arrLineGaugeRecord[i].Clear();

                int intSelectedLine = GetLineGaugeSelectedIndex(intPadIndex, i);
                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intImageIndex >= arrImage.Count)
                    intImageIndex = 0;

                PointF pTopLeftPoint = SetParameterToPointGauge(arrImage[intImageIndex][i], m_arrLineGauge[3].ref_ObjectLine, m_arrLineGauge[i], i, 3, 0);
                PointF pTopRightPoint = SetParameterToPointGauge(arrImage[intImageIndex][i], m_arrLineGauge[1].ref_ObjectLine, m_arrLineGauge[i], i, 1, 1);
                

                if (pTopLeftPoint.X == 0 || pTopLeftPoint.Y == 0 || pTopRightPoint.X == 0 || pTopRightPoint.Y == 0)
                {
                    blnResult[i] = false;
                }
                else
                {
                    List<LineGaugeRecord> arrLineGaugeRecord = m_arrLineGauge[i].MeasureAndCollect_CornerPointGauge(
                                                                pTopLeftPoint, pTopRightPoint, 
                                                                m_arrLineGauge[3].ref_ObjectAngle, 
                                                                m_arrLineGauge[1].ref_ObjectAngle);

                    for (int r = 0; r < arrLineGaugeRecord.Count; r++)
                    {
                        m_arrLineGaugeRecord[i].Add(arrLineGaugeRecord[r]);
                    }

                    //blnResult[i] = true;
                    //nCount++;
                    //m_arrLineGauge[i].ref_ObjectLine.CalculateStraightLine(pTopLeftPoint, pTopRightPoint);
                }
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////
            bool blnRepeat = false;
            bool isResultOK = true;
            List<int> arrTopIndex = new List<int>();
            List<int> arrRightIndex = new List<int>();
            List<int> arrBottomIndex = new List<int>();
            List<int> arrLeftIndex = new List<int>();
            List<float> arrResultWidth = new List<float>();
            List<float> arrResultHeight = new List<float>();
            List<PointF> arrResultPoint = new List<PointF>();
            Line[] arrCrossLines;
            Line objLineTop = new Line();
            Line objLineRight = new Line();
            Line objLineBottom = new Line();
            Line objLineLeft = new Line();
            List<List<Line>> arrVirtualLine = new List<List<Line>>();
            List<int> arrVirtualLineIndex = new List<int>();
            List<int> arrVirtualLineIndex2 = new List<int>();
            float[] arrCornerGaugeAngles;

            for (int left = 0; left < m_arrLineGaugeRecord[3].Count; left++)
            {
                for (int right = 0; right < m_arrLineGaugeRecord[1].Count; right++)
                {
                    for (int top = 0; top < m_arrLineGaugeRecord[0].Count; top++)
                    {
                        for (int bottom = 0; bottom < m_arrLineGaugeRecord[2].Count; bottom++)
                        {
                            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            {
                                {
                                    objLineTop = m_arrLineGaugeRecord[0][top].objLine;
                                    objLineRight = m_arrLineGaugeRecord[1][right].objLine;
                                    objLineBottom = m_arrLineGaugeRecord[2][bottom].objLine;
                                    objLineLeft = m_arrLineGaugeRecord[3][left].objLine;

                                    // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                                    m_arrRectCornerPoints = new PointF[4];
                                    m_arrRectCornerPoints[0] = Line.GetCrossPoint(objLineTop, objLineLeft);
                                    m_arrRectCornerPoints[1] = Line.GetCrossPoint(objLineTop, objLineRight);
                                    m_arrRectCornerPoints[2] = Line.GetCrossPoint(objLineRight, objLineBottom);
                                    m_arrRectCornerPoints[3] = Line.GetCrossPoint(objLineBottom, objLineLeft);

                                    // Get corner angle
                                    arrCornerGaugeAngles = new float[4];
                                    arrCornerGaugeAngles[0] = Math2.GetAngle(objLineTop, objLineLeft);
                                    arrCornerGaugeAngles[1] = Math2.GetAngle(objLineTop, objLineRight);
                                    arrCornerGaugeAngles[2] = Math2.GetAngle(objLineRight, objLineBottom);
                                    arrCornerGaugeAngles[3] = Math2.GetAngle(objLineBottom, objLineLeft);

                                    // Verify corner angle
                                    isResultOK = true;
                                    int intBestAngleIndex = -1;
                                    float fBestAngle = float.MaxValue;
                                    bool[] arrAngleOK = new bool[4];
                                    for (int i = 0; i < 4; i++)
                                    {
                                        float fAngle = Math.Abs(arrCornerGaugeAngles[i] - 90);
                                        if (fAngle > m_arrGaugeMaxAngle[i])
                                        {
                                            if (isResultOK)
                                            {
                                                isResultOK = false;
                                            }
                                        }
                                        else
                                        {
                                            arrAngleOK[i] = true;

                                            if (fAngle < fBestAngle)
                                            {
                                                fBestAngle = fAngle;
                                                intBestAngleIndex = i;
                                            }
                                        }

                                    }

                                    if (isResultOK)
                                    {
                                        // Get rectangle corner close lines
                                        arrCrossLines = new Line[2];
                                        arrCrossLines[0] = new Line();
                                        arrCrossLines[1] = new Line();
                                        // Calculate center point
                                        // -------- ver line ---------------------------------------
                                        arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                                        // ---------- Hor line ------------

                                        arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                                        // Verify Unit Size
                                        //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                                        PointF pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                                        float fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;
                                        float fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;
                                        //else
                                        //    isResultOK = false;

                                        if (pRectCenterPoint.X.ToString() != "NaN" && pRectCenterPoint.Y.ToString() != "NaN" &&
                                                             pRectCenterPoint.X.ToString() != "-NaN" && pRectCenterPoint.Y.ToString() != "-NaN" &&
                                                             pRectCenterPoint.X.ToString() != "Infinity" && pRectCenterPoint.Y.ToString() != "Infinity" &&
                                                             pRectCenterPoint.X.ToString() != "-Infinity" && pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                                             fRectWidth != 0 && fRectHeight != 0)
                                        {

                                            arrResultWidth.Add(fRectWidth);

                                            arrResultHeight.Add(fRectHeight);

                                            arrResultPoint.Add(pRectCenterPoint);
                                            arrTopIndex.Add(top);
                                            arrRightIndex.Add(right);
                                            arrBottomIndex.Add(bottom);
                                            arrLeftIndex.Add(left);
                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            int intSelectedIndex = -1;
            float fMinDifferent = float.MaxValue;
            // Loop to get the best result
            for (int i = 0; i < arrResultWidth.Count; i++)
            {
                float fWidthDiff = Math.Abs(arrResultWidth[i] - m_fUnitSizeWidth);
                float fHeightDiff = Math.Abs(arrResultHeight[i] - m_fUnitSizeHeight);

                if (fMinDifferent > Math.Max(fWidthDiff, fHeightDiff))
                {
                    fMinDifferent = Math.Max(fWidthDiff, fHeightDiff);
                    intSelectedIndex = i;
                }
            }

            if (intSelectedIndex >= 0)
            {
                m_arrLineResultOK[0] = true;
                m_arrLineResultOK[1] = true;
                m_arrLineResultOK[2] = true;
                m_arrLineResultOK[3] = true;

                if (arrTopIndex[intSelectedIndex] == 0)
                {
                    m_arrCornerPointResult[0] = true;
                    m_arrCornerPointResult2[0] = true;
                }
                else if (arrTopIndex[intSelectedIndex] == 1)
                {
                    m_arrCornerPointResult[0] = true;
                    m_arrCornerPointResult2[0] = false;
                }
                else if (arrTopIndex[intSelectedIndex] == 2)
                {
                    m_arrCornerPointResult[0] = false;
                    m_arrCornerPointResult2[0] = true;
                }

                if (arrBottomIndex[intSelectedIndex] == 0)
                {
                    m_arrCornerPointResult[2] = true;
                    m_arrCornerPointResult2[2] = true;
                }
                else if (arrBottomIndex[intSelectedIndex] == 1)
                {
                    m_arrCornerPointResult[2] = true;
                    m_arrCornerPointResult2[2] = false;
                }
                else if (arrBottomIndex[intSelectedIndex] == 2)
                {
                    m_arrCornerPointResult[2] = false;
                    m_arrCornerPointResult2[2] = true;
                }

                objLineTop = m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine;
                objLineRight = m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine;
                objLineBottom = m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine;
                objLineLeft = m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine;

                // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
                m_arrRectCornerPoints = new PointF[4];
                m_arrRectCornerPoints[0] = Line.GetCrossPoint(objLineTop, objLineLeft);
                m_arrRectCornerPoints[1] = Line.GetCrossPoint(objLineTop, objLineRight);
                m_arrRectCornerPoints[2] = Line.GetCrossPoint(objLineRight, objLineBottom);
                m_arrRectCornerPoints[3] = Line.GetCrossPoint(objLineBottom, objLineLeft);

                // Get corner angle
                arrCornerGaugeAngles = new float[4];
                arrCornerGaugeAngles[0] = Math2.GetAngle(objLineTop, objLineLeft);
                arrCornerGaugeAngles[1] = Math2.GetAngle(objLineTop, objLineRight);
                arrCornerGaugeAngles[2] = Math2.GetAngle(objLineRight, objLineBottom);
                arrCornerGaugeAngles[3] = Math2.GetAngle(objLineBottom, objLineLeft);

                m_fRectAngle = (GetFormatedAngle(arrCornerGaugeAngles[0]) + GetFormatedAngle(arrCornerGaugeAngles[1]) + 
                                GetFormatedAngle(arrCornerGaugeAngles[2]) + GetFormatedAngle(arrCornerGaugeAngles[3])) / 4;
            }
            else
            {
                m_arrLineResultOK[0] = false;
                m_arrLineResultOK[2] = false;

                m_arrCornerPointResult[0] = false;
                m_arrCornerPointResult2[0] = false;

                m_arrCornerPointResult[2] = false;
                m_arrCornerPointResult2[2] = false;

            }

            ////// Check is Edge Line Gauge measurement good
            //FilterEdgeAngle_Pad_CornerPointGaugeTB(m_arrLineGauge, ref fFinalAngle, ref nCount, ref blnResult);

            //m_arrLineResultOK = blnResult;

            //if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Measure edge fail.";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}
            //if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            //{
            //    m_strErrorMessage = "*Measure edge fail.";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}
            //if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            //{
            //    m_strErrorMessage = "*Measure edge fail.";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            //bool blnSingleAnglePass = false;
            //if (intPadIndex == 1)
            //{
            //    if (blnResult[0])
            //    {
            //        m_fRectAngle = m_arrLineGauge[0].ref_ObjectAngle;
            //        blnSingleAnglePass = true;
            //    }
            //}
            //else if (intPadIndex == 2)
            //{
            //    if (blnResult[1])
            //    {
            //        m_fRectAngle = m_arrLineGauge[1].ref_ObjectAngle;
            //        blnSingleAnglePass = true;
            //    }
            //}
            //else if (intPadIndex == 3)
            //{
            //    if (blnResult[2])
            //    {
            //        m_fRectAngle = m_arrLineGauge[2].ref_ObjectAngle;
            //        blnSingleAnglePass = true;
            //    }
            //}
            //else if (intPadIndex == 4)
            //{
            //    if (blnResult[3])
            //    {
            //        m_fRectAngle = m_arrLineGauge[3].ref_ObjectAngle;
            //        blnSingleAnglePass = true;
            //    }
            //}

            //if (!blnSingleAnglePass)
            //{
            //    // Get average angle from Edge Line Gauges Angle
            //    m_fRectAngle = fFinalAngle;
            //}

            //bool 
            //isResultOK = true;

            //// Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
            //m_arrRectCornerPoints = new PointF[4];
            //m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
            //m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
            //m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
            //m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

            //// Get corner angle
            ////float[] 
            //arrCornerGaugeAngles = new float[4];
            //arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
            //arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
            //arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
            //arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

            // Verify Unit Size
            //if (!blnEnableVirtualLine)
            //{
            //    if (nCount < 4)
            //    {
            //        isResultOK = false;
            //        m_strErrorMessage = "*Measure Edge Fail.";
            //        //m_intFailResultMask |= 0x02;
            //        //return false;
            //    }
            //}

            if (isResultOK)
            {

                // Get rectangle corner close lines
                //Line[] 
                arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }

        private bool GetPosition_UsingEdgeAndCornerLineGauge_Pad5SPkg_CornerPointGaugeTB_Ver1(List<List<ImageDrawing>> arrImage, bool blnEnableVirtualLine, bool blnWantCalculateBasedOnPadIndex, int intPadIndex,
    bool bUseTemplateUnitSize, float fTemplateUnitSizeX, float fTemplateUnitSizeY)
        {
            // Init data
            float fFinalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_pRectCenterPoint = new PointF(0, 0);

            // Start measure image with Edge Line gauge
            for (int i = 1; i < 4; i += 2)    // scan 1 (right) and 3 (Left)
            {
                int intSelectedLine = GetLineGaugeSelectedIndex(intPadIndex, i);
                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intImageIndex >= arrImage.Count)
                    intImageIndex = 0;

                m_arrLineGauge[i].Measure(arrImage[intImageIndex][i]);

                if (!m_arrAutoDontCare[intSelectedLine])
                {
                    if (m_arrDontCareROI[i].Count > 0)
                    {
                        float fDontCareLengthTotal = 0;
                        if (i == 0 || i == 2)   // Top and Bottom Edge ROI
                        {
                            for (int c = 0; c < m_arrDontCareROI[i].Count; c++)
                            {
                                fDontCareLengthTotal += m_arrDontCareROI[i][c].ref_ROIWidth;
                            }
                        }
                        else // Left and Right Edge ROI
                        {
                            for (int c = 0; c < m_arrDontCareROI[i].Count; c++)
                            {
                                fDontCareLengthTotal += m_arrDontCareROI[i][c].ref_ROIHeight;
                            }
                        }

                        float fPassLength = m_arrLineGauge[i].ref_ObjectScore / 100 * m_arrLineGauge[i].ref_GaugeLength;
                        float fMeasuredLength = m_arrLineGauge[i].ref_GaugeLength - fDontCareLengthTotal;

                        float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

                        if (fFinalMeasureScore < m_arrGaugeMinScore[i])
                        {
                            blnResult[i] = false;
                        }
                        else
                        {
                            blnResult[i] = true;
                            nCount++;
                            m_arrLineGauge[i].GetObjectLine();
                        }
                    }
                    else
                    {
                        //if ((arrLineGauges[i].ref_GaugeThickness < 50 && arrLineGauges[i].ref_ObjectScore < 20) ||
                        //    (arrLineGauges[i].ref_GaugeThickness >= 50 && arrLineGauges[i].ref_ObjectScore < 30))
                        if (m_arrLineGauge[i].ref_ObjectScore < m_arrGaugeMinScore[i])
                        {
                            blnResult[i] = false;
                        }
                        else
                        {
                            blnResult[i] = true;
                            nCount++;
                            m_arrLineGauge[i].GetObjectLine();
                        }
                    }
                }
                else
                {
                    float fDontCareLengthTotal = m_arrAutoDontCareTotalLength[intSelectedLine];

                    float fPassLength = m_arrLineGauge[i].ref_ObjectScore / 100 * m_arrLineGauge[i].ref_GaugeLength;
                    float fMeasuredLength = m_arrLineGauge[i].ref_GaugeLength - fDontCareLengthTotal;

                    float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

                    if (fFinalMeasureScore < m_arrGaugeMinScore[i])
                    {
                        blnResult[i] = false;
                    }
                    else
                    {
                        blnResult[i] = true;
                        nCount++;
                        m_arrLineGauge[i].GetObjectLine();
                    }
                }
            }

            // Corner Ling Gauge TB mode is rely on left and right line gauge. If either one left or right line gauge fail, then will fail overall.
            if (!blnResult[1])
            {
                m_strErrorMessage = "*Measure right edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            if (!blnResult[3])   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure left edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 3; i += 2)    // scan 0 and 2
            {
                int intSelectedLine = GetLineGaugeSelectedIndex(intPadIndex, i);
                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intImageIndex >= arrImage.Count)
                    intImageIndex = 0;

                PointF pTopLeftPoint = SetParameterToPointGauge(arrImage[intImageIndex][i], m_arrLineGauge[3].ref_ObjectLine, m_arrLineGauge[i], i, 3, 0);
                PointF pTopRightPoint = SetParameterToPointGauge(arrImage[intImageIndex][i], m_arrLineGauge[1].ref_ObjectLine, m_arrLineGauge[i], i, 1, 1);

                if (pTopLeftPoint.X == 0 || pTopLeftPoint.Y == 0 || pTopRightPoint.X == 0 || pTopRightPoint.Y == 0)
                {
                    blnResult[i] = false;
                }
                else
                {
                    blnResult[i] = true;
                    nCount++;
                    m_arrLineGauge[i].ref_ObjectLine.CalculateStraightLine(pTopLeftPoint, pTopRightPoint);
                }
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            //// Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad_CornerPointGaugeTB(m_arrLineGauge, ref fFinalAngle, ref nCount, ref blnResult);

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "*Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            bool blnSingleAnglePass = false;
            if (intPadIndex == 1)
            {
                if (blnResult[0])
                {
                    m_fRectAngle = m_arrLineGauge[0].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 2)
            {
                if (blnResult[1])
                {
                    m_fRectAngle = m_arrLineGauge[1].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 3)
            {
                if (blnResult[2])
                {
                    m_fRectAngle = m_arrLineGauge[2].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }
            else if (intPadIndex == 4)
            {
                if (blnResult[3])
                {
                    m_fRectAngle = m_arrLineGauge[3].ref_ObjectAngle;
                    blnSingleAnglePass = true;
                }
            }

            if (!blnSingleAnglePass)
            {
                // Get average angle from Edge Line Gauges Angle
                m_fRectAngle = fFinalAngle;
            }

            bool isResultOK = true;

            // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
            m_arrRectCornerPoints = new PointF[4];
            m_arrRectCornerPoints[0] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
            m_arrRectCornerPoints[1] = Line.GetCrossPoint(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
            m_arrRectCornerPoints[2] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
            m_arrRectCornerPoints[3] = Line.GetCrossPoint(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

            // Get corner angle
            float[] arrCornerGaugeAngles = new float[4];
            arrCornerGaugeAngles[0] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);
            arrCornerGaugeAngles[1] = Math2.GetAngle(m_arrLineGauge[0].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
            arrCornerGaugeAngles[2] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[1].ref_ObjectLine);
            arrCornerGaugeAngles[3] = Math2.GetAngle(m_arrLineGauge[2].ref_ObjectLine, m_arrLineGauge[3].ref_ObjectLine);

            // Verify Unit Size
            if (!blnEnableVirtualLine)
            {
                if (nCount < 4)
                {
                    isResultOK = false;
                    m_strErrorMessage = "*Measure Edge Fail.";
                    //m_intFailResultMask |= 0x02;
                    //return false;
                }
            }

            if (isResultOK)
            {

                // Get rectangle corner close lines
                Line[] arrCrossLines = new Line[2];
                arrCrossLines[0] = new Line();

                arrCrossLines[1] = new Line();

                // Calculate center point
                // -------- ver line ---------------------------------------
                arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                m_fRectUpWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]);
                m_fRectDownWidth = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3]);
                m_fRectWidth = (m_fRectUpWidth + m_fRectDownWidth) / 2; // Use average formula. (This formula may not suitable for other vision application) 

                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectLeftHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]);
                m_fRectRightHeight = Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2]);
                m_fRectHeight = (m_fRectLeftHeight + m_fRectRightHeight) / 2;   // Use average formula. (This formula may not suitable for other vision application) 

                if (blnWantCalculateBasedOnPadIndex)
                {
                    switch (intPadIndex)
                    {
                        case 0:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 1:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fUnitSizeHeight / 2);
                            break;
                        case 2:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[1].X + m_arrRectCornerPoints[2].X) / 2 - m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                            break;
                        case 3:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                                                            (m_arrRectCornerPoints[2].Y + m_arrRectCornerPoints[3].Y) / 2 - m_fUnitSizeHeight / 2);
                            break;
                        case 4:
                            m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fUnitSizeWidth / 2,
                                                            (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);

                            break;
                    }
                }
                else
                {
                    // Verify Unit Size
                    //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    //m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                    m_pRectCenterPoint = new PointF((m_arrRectCornerPoints[0].X + m_arrRectCornerPoints[3].X) / 2 + m_fRectWidth / 2,
                        (m_arrRectCornerPoints[0].Y + m_arrRectCornerPoints[1].Y) / 2 + m_fRectHeight / 2);
                    //else
                    //    isResultOK = false;
                }
            }

            if (!isResultOK)
            {
                return false;
                //// Place Corner Line Gauges to the corner point location
                //PlaceCornerLineGaugeLocation(ref m_arrLineGauge, ref m_arrRectCornerPoints, m_fUnitSizeWidth, m_fUnitSizeHeight);

                //// Corner line gauges measure unit
                //for (int i = 4; i < m_arrLineGauge.Count; i++)
                //{
                //    m_arrLineGauge[i].Measure(objROI);
                //}

                //// Get actual unit position based on all Edge and Corner Line Gauges.
                //if (!GetPositionByCorner(m_arrLineGauge, m_fUnitSizeWidth, m_fUnitSizeHeight, 75, ref m_pRectCenterPoint,
                //                            ref m_arrRectCornerPoints, ref m_fRectWidth, ref m_fRectHeight, ref m_fRectAngle, ref m_strErrorMessage))
                //    return false;
            }

            //if (nCount < 4)   // only 1 line gauge has valid result. No enough data to generate result
            //{
            //    m_strErrorMessage = "*Only " + nCount.ToString() + " gauge has valid result";
            //    m_intFailResultMask |= 0x02;
            //    return false;
            //}

            return true;
        }

        private void FilterEdgeAngle_Pad_ImproveAngleVersion(List<LGauge> arrLineGauges, float fMinBorderScore, ref float fFinalAngle, ref int nCount, ref bool[] blnResult)
        {
            blnResult = new bool[4];

            // Filter angle with score
            float[] fUnsortAngle = new float[4];
            for (int i = 0; i < 4; i++)
            {
                if (!m_arrAutoDontCare[i])
                {
                    if (m_arrDontCareROI[i].Count > 0)
                    {
                        float fDontCareLengthTotal = 0;
                        if (i == 0 || i == 2)   // Top and Bottom Edge ROI
                        {
                            for (int c = 0; c < m_arrDontCareROI[i].Count; c++)
                            {
                                fDontCareLengthTotal += m_arrDontCareROI[i][c].ref_ROIWidth;
                            }
                        }
                        else // Left and Right Edge ROI
                        {
                            for (int c = 0; c < m_arrDontCareROI[i].Count; c++)
                            {
                                fDontCareLengthTotal += m_arrDontCareROI[i][c].ref_ROIHeight;
                            }
                        }

                        float fPassLength = arrLineGauges[i].GetObjectScore() / 100 * arrLineGauges[i].ref_GaugeLength;
                        float fMeasuredLength = arrLineGauges[i].ref_GaugeLength - fDontCareLengthTotal;

                        float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

                        if (fFinalMeasureScore < m_arrGaugeMinScore[i])
                        {
                            blnResult[i] = false;
                        }
                        else
                        {
                            blnResult[i] = true;
                            fUnsortAngle[i] = arrLineGauges[i].GetObjectAngle();
                            arrLineGauges[i].GetObjectLine();
                        }
                    }
                    else
                    {
                        //if ((arrLineGauges[i].ref_GaugeThickness < 50 && arrLineGauges[i].ref_ObjectScore < 20) ||
                        //    (arrLineGauges[i].ref_GaugeThickness >= 50 && arrLineGauges[i].ref_ObjectScore < 30))
                        if (arrLineGauges[i].GetObjectScore() < m_arrGaugeMinScore[i])
                        {
                            blnResult[i] = false;
                        }
                        else
                        {
                            blnResult[i] = true;
                            fUnsortAngle[i] = arrLineGauges[i].GetObjectAngle();
                            arrLineGauges[i].GetObjectLine();
                        }
                    }
                }
                else
                {
                        float fDontCareLengthTotal = m_arrAutoDontCareTotalLength[i];
                       

                        float fPassLength = arrLineGauges[i].ref_ObjectScore / 100 * arrLineGauges[i].ref_GaugeLength;
                        float fMeasuredLength = arrLineGauges[i].ref_GaugeLength - fDontCareLengthTotal;

                        float fFinalMeasureScore = fPassLength / fMeasuredLength * 100;

                        if (fFinalMeasureScore < m_arrGaugeMinScore[i])
                        {
                            blnResult[i] = false;
                        }
                        else
                        {
                            blnResult[i] = true;
                            fUnsortAngle[i] = arrLineGauges[i].ref_ObjectAngle;
                            arrLineGauges[i].GetObjectLine();
                        }
                    
                }
            }

            // Sort angle from increase
            List<float> fSortAngle = new List<float>();
            List<int> fSortedMaxAngle = new List<int>();
            for (int i = 0; i < fUnsortAngle.Length; i++)
            {
                if (!blnResult[i])
                    continue;

                int intIndex = fSortAngle.Count;
                for (int j = 0; j < fSortAngle.Count; j++)
                {
                    if (fSortAngle[j] > fUnsortAngle[i])
                    {
                        intIndex = j;
                        break;
                    }
                }
                fSortAngle.Insert(intIndex, fUnsortAngle[i]);
                fSortedMaxAngle.Insert(intIndex, m_arrGaugeMaxAngle[i]);
            }

            // Find the smallest gap between the angle value
            float fSmallestGap = float.MaxValue;
            float fCenterAngle = 0;
            fFinalAngle = 0;
            for (int i = 1; i < fSortAngle.Count; i++)
            {
                if ((fSortAngle[i] - fSortAngle[i - 1]) < fSmallestGap)
                {
                    fSmallestGap = fSortAngle[i] - fSortAngle[i - 1];
                    fCenterAngle = (fSortAngle[i] + fSortAngle[i - 1]) / 2;
                    fFinalAngle = (fSortAngle[i] + fSortAngle[i - 1]) / 2;
                }
            }

            // Filter angle with tolerance 3 from smallest angle
            nCount = 0;
            for (int i = 0; i < fSortAngle.Count; i++)
            {
                if ((GetFormatedAngle(fSortAngle[i]) >= (fCenterAngle - fSortedMaxAngle[i])) && (GetFormatedAngle(fSortAngle[i]) <= (fCenterAngle + fSortedMaxAngle[i])))  // Tolerance 3 value may not suitable for other application
                {
                    nCount++;
                }
                else
                {
                    fSortAngle[i] = -999;
                }

            }

            // Set the blnResult to false if the angle has been filter out.
            for (int j = 0; j < blnResult.Length; j++)
            {
                if (blnResult[j])
                {
                    bool blnFound = false;
                    for (int i = 0; i < fSortAngle.Count; i++)
                    {
                        if (fUnsortAngle[j] == fSortAngle[i])
                        {
                            blnFound = true;
                            break;
                        }
                    }
                    if (!blnFound)
                        blnResult[j] = false;
                }
            }
        }

        private void FilterEdgeAngle_Pad_CornerPointGaugeTB(List<LGauge> arrLineGauges, ref float fFinalAngle, ref int nCount, ref bool[] blnResult)
        {
            //blnResult = new bool[4];

            // Filter angle with score
            float[] fUnsortAngle = new float[4];
            for (int i = 0; i < 4; i++)
            {
                if (i == 0 || i == 2)
                    fUnsortAngle[i] = LGauge.ConvertObjectAngle(i * 90 + 90, (float)arrLineGauges[i].ref_ObjectLine.ref_dAngle);
                else
                    fUnsortAngle[i] = arrLineGauges[i].ref_ObjectAngle;
            }

            // Sort angle from increase
            List<float> fSortAngle = new List<float>();
            List<int> fSortedMaxAngle = new List<int>();
            for (int i = 0; i < fUnsortAngle.Length; i++)
            {
                if (!blnResult[i])
                    continue;

                int intIndex = fSortAngle.Count;
                for (int j = 0; j < fSortAngle.Count; j++)
                {
                    if (fSortAngle[j] > fUnsortAngle[i])
                    {
                        intIndex = j;
                        break;
                    }
                }
                fSortAngle.Insert(intIndex, fUnsortAngle[i]);
                fSortedMaxAngle.Insert(intIndex, m_arrGaugeMaxAngle[i]);
            }

            // Find the smallest gap between the angle value
            float fSmallestGap = float.MaxValue;
            float fCenterAngle = 0;
            fFinalAngle = 0;
            for (int i = 1; i < fSortAngle.Count; i++)
            {
                if ((fSortAngle[i] - fSortAngle[i - 1]) < fSmallestGap)
                {
                    fSmallestGap = fSortAngle[i] - fSortAngle[i - 1];
                    fCenterAngle = (fSortAngle[i] + fSortAngle[i - 1]) / 2;
                    fFinalAngle = (fSortAngle[i] + fSortAngle[i - 1]) / 2;
                }
            }

            // Filter angle with tolerance 3 from smallest angle
            nCount = 0;
            for (int i = 0; i < fSortAngle.Count; i++)
            {
                if ((GetFormatedAngle(fSortAngle[i]) >= (fCenterAngle - fSortedMaxAngle[i])) && (GetFormatedAngle(fSortAngle[i]) <= (fCenterAngle + fSortedMaxAngle[i])))  // Tolerance 3 value may not suitable for other application
                {
                    nCount++;
                }
                else
                {
                    fSortAngle[i] = -999;
                }

            }

            // Set the blnResult to false if the angle has been filter out.
            for (int j = 0; j < blnResult.Length; j++)
            {
                if (blnResult[j])
                {
                    bool blnFound = false;
                    for (int i = 0; i < fSortAngle.Count; i++)
                    {
                        if (fUnsortAngle[j] == fSortAngle[i])
                        {
                            blnFound = true;
                            break;
                        }
                    }
                    if (!blnFound)
                        blnResult[j] = false;
                }
            }
        }

        public int GetEdgeROISize(int intLineIndex)
        {
            int intTolerance = 0;
            switch (intLineIndex)
            {
                case 0:
                    intTolerance = m_arrEdgeROI[0].ref_ROIHeight;
                    break;
                case 1:
                    intTolerance = m_arrEdgeROI[1].ref_ROIWidth;
                    break;
                case 2:
                    intTolerance = m_arrEdgeROI[2].ref_ROIHeight;
                    break;
                case 3:
                    intTolerance = m_arrEdgeROI[3].ref_ROIWidth;
                    break;
            }

            return intTolerance;
        }
        public int GetEdgeROISize(int intLineIndex, bool blnHalfSize, int intTolerance)
        {
            switch (intLineIndex)
            {
                case 0:
                    if (blnHalfSize)
                        intTolerance = m_arrEdgeROI[0].ref_ROIHeight / 2;
                    else
                        intTolerance = m_arrEdgeROI[0].ref_ROIHeight;
                    break;
                case 1:
                    if (blnHalfSize)
                        intTolerance = m_arrEdgeROI[1].ref_ROIWidth / 2;
                    else
                        intTolerance = m_arrEdgeROI[1].ref_ROIWidth;
                    break;
                case 2:
                    if (blnHalfSize)
                        intTolerance = m_arrEdgeROI[2].ref_ROIHeight / 2;
                    else
                        intTolerance = m_arrEdgeROI[2].ref_ROIHeight;
                    break;
                case 3:
                    if (blnHalfSize)
                        intTolerance = m_arrEdgeROI[3].ref_ROIWidth / 2;
                    else
                        intTolerance = m_arrEdgeROI[3].ref_ROIWidth;
                    break;
            }

            return intTolerance;
        }
        private void ClearGaugeRecordList()
        {
            for (int intLineIndex = 0; intLineIndex < 4; intLineIndex++)
            {
                m_arrLineGaugeRecord[intLineIndex].Clear();
            }
        }

        private void InitGaugeRecordList()
        {
            for (int intLineIndex = 0; intLineIndex < 4; intLineIndex++)
            {
                if (m_arrLineGaugeRecord[intLineIndex] == null)
                    m_arrLineGaugeRecord[intLineIndex] = new List<LineGaugeRecord>();
            }

        }

        private void CollectMeasuredGaugeResult_LineGaugeRecord(LGauge objLineGauge, int intLineIndex)
        {
            LineGaugeRecord objLineGaugeRecord = new LineGaugeRecord();

            // Gauge Score
            objLineGaugeRecord.bUseSubGauge = false;
            objLineGaugeRecord.fMeasureScore = objLineGauge.ref_ObjectScore;
            objLineGaugeRecord.fMeasureAngle = objLineGauge.ref_ObjectAngle;
            objLineGaugeRecord.fMeasureCenterX = objLineGauge.ref_ObjectCenterX;
            objLineGaugeRecord.fMeasureCenterY = objLineGauge.ref_ObjectCenterY;

            objLineGaugeRecord.fSetTolerance = objLineGauge.ref_GaugeTolerance;
            objLineGaugeRecord.fSetLength = objLineGauge.ref_GaugeLength;
            objLineGaugeRecord.fSetCenterX = objLineGauge.ref_GaugeCenterX;
            objLineGaugeRecord.fSetCenterY = objLineGauge.ref_GaugeCenterY;
            objLineGaugeRecord.fSetAngle = objLineGauge.ref_GaugeAngle;
            objLineGaugeRecord.intSetMinAmp = objLineGauge.ref_GaugeMinAmplitude;
            objLineGaugeRecord.intSetMinArea = objLineGauge.ref_GaugeMinArea;
            objLineGaugeRecord.intSetSmoothing = objLineGauge.ref_GaugeFilter;
            objLineGaugeRecord.intSetThreshold = objLineGauge.ref_GaugeThreshold;
            objLineGaugeRecord.intSetTransitionChoice = objLineGauge.ref_GaugeTransChoice;
            objLineGaugeRecord.intSetTransitionType = objLineGauge.ref_GaugeTransType;

            objLineGaugeRecord.objLine.CalculateLGStraightLine(new PointF(objLineGauge.ref_ObjectCenterX,
                                                                                   objLineGauge.ref_ObjectCenterY),
                                                                                   objLineGauge.ref_ObjectOriAngle);
            m_arrLineGaugeRecord[intLineIndex].Add(objLineGaugeRecord);
        }

        private PointF SetParameterToPointGauge(ImageDrawing objImage, Line objLine, LGauge m_LineGauge, int intROIIndex, int intLineGaugeIndex, int intPointGaugeIndex)
        {
            Line objLine2 = new Line();
            objLine2.CalculateLGStraightLine(new PointF(m_LineGauge.ref_GaugeCenterX, m_LineGauge.ref_GaugeCenterY), m_LineGauge.ref_GaugeAngle);

            PointF pCross = Line.GetCrossPoint(objLine, objLine2);

            switch (intROIIndex)
            {
                case 0://Top
                    if (intLineGaugeIndex == 1)
                    {
                        pCross.X -= m_arrGaugeOffset[0];
                        if ((pCross.X + 3) >= (m_LineGauge.ref_GaugeCenterX + (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF((m_LineGauge.ref_GaugeCenterX + (m_LineGauge.ref_GaugeLength / 2)) - 3, pCross.Y);
                    }
                    else if (intLineGaugeIndex == 3)
                    {
                        pCross.X += m_arrGaugeOffset[0];
                        if ((pCross.X - 3) <= (m_LineGauge.ref_GaugeCenterX - (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF((m_LineGauge.ref_GaugeCenterX - (m_LineGauge.ref_GaugeLength / 2)) + 3, pCross.Y);
                    }
                    break;
                case 1://Right
                    if (intLineGaugeIndex == 2)
                    {
                        pCross.Y -= m_arrGaugeOffset[1];
                        if ((pCross.Y + 3) >= (m_LineGauge.ref_GaugeCenterY + (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF(pCross.X, (m_LineGauge.ref_GaugeCenterY + (m_LineGauge.ref_GaugeLength / 2)) - 3);
                    }
                    else if (intLineGaugeIndex == 0)
                    {
                        pCross.Y += m_arrGaugeOffset[1];
                        if ((pCross.Y - 3) <= (m_LineGauge.ref_GaugeCenterY - (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF(pCross.X, (m_LineGauge.ref_GaugeCenterY - (m_LineGauge.ref_GaugeLength / 2)) + 3);
                    }
                    break;
                case 2://Bottom
                    if (intLineGaugeIndex == 1)
                    {
                        pCross.X -= m_arrGaugeOffset[2];
                        if ((pCross.X + 3) >= (m_LineGauge.ref_GaugeCenterX + (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF((m_LineGauge.ref_GaugeCenterX + (m_LineGauge.ref_GaugeLength / 2)) - 3, pCross.Y);
                    }
                    else if (intLineGaugeIndex == 3)
                    {
                        pCross.X += m_arrGaugeOffset[2];
                        if ((pCross.X - 3) <= (m_LineGauge.ref_GaugeCenterX - (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF((m_LineGauge.ref_GaugeCenterX - (m_LineGauge.ref_GaugeLength / 2)) + 3, pCross.Y);
                    }
                    break;
                case 3://Left
                    if (intLineGaugeIndex == 2)
                    {
                        pCross.Y -= m_arrGaugeOffset[3];
                        if ((pCross.Y + 3) >= (m_LineGauge.ref_GaugeCenterY + (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF(pCross.X, (m_LineGauge.ref_GaugeCenterY + (m_LineGauge.ref_GaugeLength / 2)) - 3);
                    }
                    else if (intLineGaugeIndex == 0)
                    {
                        pCross.Y += m_arrGaugeOffset[3];
                        if ((pCross.Y - 3) <= (m_LineGauge.ref_GaugeCenterY - (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF(pCross.X, (m_LineGauge.ref_GaugeCenterY - (m_LineGauge.ref_GaugeLength / 2)) + 3);
                    }
                    break;
            }

            if (intPointGaugeIndex == 0)
            {

                m_arrPointGauge[intROIIndex].SetGaugePlacementCenter(pCross.X, pCross.Y);
                m_arrPointGauge[intROIIndex].ref_GaugeTolerance = m_LineGauge.ref_GaugeTolerance;
                m_arrPointGauge[intROIIndex].ref_GaugeAngle = m_LineGauge.ref_GaugeAngle + 90;
                m_arrPointGauge[intROIIndex].ref_GaugeMinAmplitude = m_LineGauge.ref_GaugeMinAmplitude;
                m_arrPointGauge[intROIIndex].ref_GaugeMinArea = m_LineGauge.ref_GaugeMinArea;
                m_arrPointGauge[intROIIndex].ref_GaugeFilter = m_LineGauge.ref_GaugeFilter;
                m_arrPointGauge[intROIIndex].ref_GaugeThickness = 6;
                m_arrPointGauge[intROIIndex].ref_GaugeThreshold = m_LineGauge.ref_GaugeThreshold;
                m_arrPointGauge[intROIIndex].ref_GaugeTransChoice = m_LineGauge.ref_GaugeTransChoice;
                m_arrPointGauge[intROIIndex].ref_GaugeTransType = m_LineGauge.ref_GaugeTransType;
                //objImage.SaveImage("D:\\TS\\TopParent0.bmp");
                //m_arrPointGauge[intROIIndex].ref_objPointGauge.Save("D:\\TS\\PointGauge0.CAL");
                m_arrPointGauge[intROIIndex].Measure(objImage);

                if (m_arrPointGauge[intROIIndex].ref_intMeasuredPointCount > 0)
                {
                    return m_arrPointGauge[intROIIndex].GetMeasurePoint();
                }
            }
            else
            {
                m_arrPointGauge2[intROIIndex].SetGaugePlacementCenter(pCross.X, pCross.Y);
                m_arrPointGauge2[intROIIndex].ref_GaugeTolerance = m_LineGauge.ref_GaugeTolerance;
                m_arrPointGauge2[intROIIndex].ref_GaugeAngle = m_LineGauge.ref_GaugeAngle + 90;
                m_arrPointGauge2[intROIIndex].ref_GaugeMinAmplitude = m_LineGauge.ref_GaugeMinAmplitude;
                m_arrPointGauge2[intROIIndex].ref_GaugeMinArea = m_LineGauge.ref_GaugeMinArea;
                m_arrPointGauge2[intROIIndex].ref_GaugeFilter = m_LineGauge.ref_GaugeFilter;
                m_arrPointGauge2[intROIIndex].ref_GaugeThickness = 6;
                m_arrPointGauge2[intROIIndex].ref_GaugeThreshold = m_LineGauge.ref_GaugeThreshold;
                m_arrPointGauge2[intROIIndex].ref_GaugeTransChoice = m_LineGauge.ref_GaugeTransChoice;
                m_arrPointGauge2[intROIIndex].ref_GaugeTransType = m_LineGauge.ref_GaugeTransType;
                //objImage.SaveImage("D:\\TS\\TopParent1.bmp");
                //m_arrPointGauge2[intROIIndex].ref_objPointGauge.Save("D:\\TS\\PointGauge1.CAL");
                m_arrPointGauge2[intROIIndex].Measure(objImage);

                if (m_arrPointGauge2[intROIIndex].ref_intMeasuredPointCount > 0)
                {
                    return m_arrPointGauge2[intROIIndex].GetMeasurePoint();
                }
            }

            return new PointF(0, 0);
        }
    }
}
