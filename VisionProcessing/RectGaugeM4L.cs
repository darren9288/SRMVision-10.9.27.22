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
    public class RectGaugeM4L
    {
        #region Member Variables
        private PGauge m_objPointGauge;
        private List<List<PGauge>> m_arrPointGauge = new List<List<PGauge>>();  // Index 0=Top, 1=Right, 2=Bottom, 3=Left,
        private List<PGauge> m_arrPointGauge_ForDrawing = new List<PGauge>();  // Index 0=Top, 1=Right, 2=Bottom, 3=Left,
        // Main variables
        private int m_intVisionIndex = 0;
        private int m_intSelectedEdgeROIPrev = 0;
        private bool m_blnCursorShapeVerifying = false;
        private int m_intDirection = 0;
        private float m_fLength = 0;
        private float m_fUnitSizeWidth = 0;
        private float m_fUnitSizeHeight = 0;
        private float m_f4LGaugeCenterPointX = 0;
        private float m_f4LGaugeCenterPointY = 0;
        private float m_fGainValue = 1000f;
        private List<ROI> m_arrEdgeROI = new List<ROI>();
        private List<List<ROI>> m_arrDontCareROI = new List<List<ROI>>();
        private int[] m_intEdgeROICenterX = new int[4];
        private int[] m_intEdgeROICenterY = new int[4];
        private int[] m_intEdgeROIOffsetX = new int[4];
        private int[] m_intEdgeROIOffsetY = new int[4];
        private List<LGauge> m_arrLineGauge = new List<LGauge>();  // Index 0=Top, 1=Right, 2=Bottom, 3=Left,
        private List<bool> m_arrGaugeEnableSubGauge = new List<bool>();     // Index 0=Top, 1=Right, 2=Bottom, 3=Left,
        private List<bool> m_arrReCheckUsingPointGaugeIfFail = new List<bool>();     // Index 0=Top, 1=Right, 2=Bottom, 3=Left,
        private List<int> m_arrGaugeMeasureMode = new List<int>();   //   Index 0=Top, 1=Right, 2=Bottom, 3=Left      // Measure Mode Value 0 = Standard 1 = Separation 2 = Scan Points 3 = Multi Lines
        private List<int> m_arrGaugeMinScore = new List<int>();  // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<int> m_arrGaugeMaxAngle = new List<int>();  // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<int> m_arrGaugeImageMode = new List<int>();        // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<int> m_arrGaugeImageNo = new List<int>();          // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<int> m_arrGaugeImageThreshold = new List<int>();   // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<int> m_arrPointGaugeOffset = new List<int>();   // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<bool> m_arrGaugeWantImageThreshold = new List<bool>();   // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<float> m_arrGaugeImageGain = new List<float>();    // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<float> m_arrGaugeGroupTolerance = new List<float>();    // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private List<bool> m_blnInToOut = new List<bool>();
        private Line[] m_arrCornerLine = new Line[8];
        private Line m_objVirtualLineWidth = new Line();
        private Line m_objVirtualLineHeight = new Line();
        private int m_intSelectedGaugeEdgeMask = 0x0F;  // 0x01=Top edge, 0x02=Right edge, 0x04=Bottom edge, 0x08=Left edge
        private int[] TestHeight = new int[4];
        private int[] TestWidth = new int[4];
        private List<ROI> m_arrEdgeROI_Temp = new List<ROI>();

        // Rectange Object Result
        private PointF[] m_arrRectCornerPoints = new PointF[4]; // Index 0=TopLeft, 1=TopRight, 2=BottomLeft, 3=BottomRight
        private PointF m_pRectCenterPoint;
        private bool[] m_arrLineResultOK = new bool[4];     // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
        private bool[] m_arrDrawLineUsingCornerPoint = new bool[4] { false, false, false, false };     // Index 0=Top, 1=Right, 2=Bottom, 3=Left, 
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
        private int[] m_arrUserThickness = new int[4];
        private int[] m_arrUserChoice = new int[4];

        // Drawing
        private SolidBrush m_BrushMatched = new SolidBrush(Color.GreenYellow);
        private Pen m_PenRectGNominal = new Pen(Color.Blue);
        private Pen m_PenRectGTransitionTypeArrow = new Pen(Color.Lime);
        private Pen m_PenRectGTransitionTypeArrowNotSelected = new Pen(Color.Green);
        private Pen m_PenDontCareArea = new Pen(Color.Pink, 2);
        private bool m_blnDrawDraggingBox = false;
        private bool m_blnDrawSamplingPoint = false;
        // Learn Template information

        // rect gauge template center location
        private float m_fTemplateObjectCenterX = 0;
        private float m_fTemplateObjectCenterY = 0;
        private float m_fTemplateObjectWidth = 0;
        private float m_fTemplateObjectHeight = 0;

        // Collect Gauge Result
        List<LineGaugeRecord>[] m_arrLineGaugeRecord = new List<LineGaugeRecord>[4];

        private ROI m_objLocalROI;
        private List<List<ROI>> m_arrLocalROI = new List<List<ROI>>();
        private List<List<ImageDrawing>> m_arrModifiedImage = new List<List<ImageDrawing>>();
        private ImageDrawing[] m_objPreModifiedImage = new ImageDrawing[4] { null, null, null, null };

        #endregion

        #region Properties
        public bool ref_blnDrawDraggingBox { get { return m_blnDrawDraggingBox; } set { m_blnDrawDraggingBox = value; } }
        public bool ref_blnDrawSamplingPoint { get { return m_blnDrawSamplingPoint; } set { m_blnDrawSamplingPoint = value; } }
        // Result
        public float ref_f4LGaugeUnitWidth { get { return m_fUnitSizeWidth; } }
        public float ref_f4LGaugeUnitHeight { get { return m_fUnitSizeHeight; } }
        public float ref_f4LGaugeCenterPointX { get { return m_f4LGaugeCenterPointX; } }
        public float ref_f4LGaugeCenterPointY { get { return m_f4LGaugeCenterPointY; } }
        public PointF ref_pRectCenterPoint { get { return m_pRectCenterPoint; } set { m_pRectCenterPoint = value; } }
        public float ref_fRectWidth { get { return m_fRectWidth; } set { m_fRectWidth = value; } }
        public float ref_fRectUpWidth { get { return m_fRectUpWidth; } set { m_fRectUpWidth = value; } }
        public float ref_fRectDownWidth { get { return m_fRectDownWidth; } set { m_fRectDownWidth = value; } }
        public float ref_fRectHeight { get { return m_fRectHeight; } set { m_fRectHeight = value; } }
        public float ref_fRectLeftHeight { get { return m_fRectLeftHeight; } set { m_fRectLeftHeight = value; } }
        public float ref_fRectRightHeight { get { return m_fRectRightHeight; } set { m_fRectRightHeight = value; } }
        public float ref_fRectAngle { get { return m_fRectAngle; } set { m_fRectAngle = value; } }
        public float ref_fGainValue { get { return m_fGainValue; } set { m_fGainValue = value; } }
        public string ref_strErrorMessage { get { return m_strErrorMessage; } set { m_strErrorMessage = value; } }
        public int ref_intFailResultMask { get { return m_intFailResultMask; } set { m_intFailResultMask = value; } }
        public bool[] ref_arrLineResultOK { get { return m_arrLineResultOK; }}
        public List<LGauge> ref_arrLineGauge { get { return m_arrLineGauge; } }
        public List<ROI> ref_arrEdgeROI { get { return m_arrEdgeROI; } }
        // return gauge template center location
        public float ref_TemplateObjectCenterX { get { return m_fTemplateObjectCenterX; } set { m_fTemplateObjectCenterX = value; } }
        public float ref_TemplateObjectCenterY { get { return m_fTemplateObjectCenterY; } set { m_fTemplateObjectCenterY = value; } }
        public float ref_TemplateObjectWidth { get { return m_fTemplateObjectWidth; } set { m_fTemplateObjectWidth = value; } }
        public float ref_TemplateObjectHeight { get { return m_fTemplateObjectHeight; } set { m_fTemplateObjectHeight = value; } }
        public int ref_intSelectedGaugeEdgeMask
        {
            get { return m_intSelectedGaugeEdgeMask; }
            set { m_intSelectedGaugeEdgeMask = value; }
        }

        public PGauge ref_objPointGauge
        {
            get
            {
                return m_objPointGauge;
            }
            set
            {
                m_objPointGauge = value;
            }
        }
        #endregion

        public RectGaugeM4L(GaugeWorldShape objWorldShape, int intDirection, int intVisionIndex)
        {
            m_intDirection = intDirection;
            m_intVisionIndex = intVisionIndex;
            m_objPointGauge = new PGauge(objWorldShape);

            m_arrPointGauge.Add(new List<PGauge>());
            m_arrPointGauge[m_arrPointGauge.Count - 1].Add(new PGauge(objWorldShape));
            m_arrPointGauge[m_arrPointGauge.Count - 1].Add(new PGauge(objWorldShape));
            m_arrPointGauge.Add(new List<PGauge>());
            m_arrPointGauge[m_arrPointGauge.Count - 1].Add(new PGauge(objWorldShape));
            m_arrPointGauge[m_arrPointGauge.Count - 1].Add(new PGauge(objWorldShape));
            m_arrPointGauge.Add(new List<PGauge>());
            m_arrPointGauge[m_arrPointGauge.Count - 1].Add(new PGauge(objWorldShape));
            m_arrPointGauge[m_arrPointGauge.Count - 1].Add(new PGauge(objWorldShape));
            m_arrPointGauge.Add(new List<PGauge>());
            m_arrPointGauge[m_arrPointGauge.Count - 1].Add(new PGauge(objWorldShape));
            m_arrPointGauge[m_arrPointGauge.Count - 1].Add(new PGauge(objWorldShape));

            m_arrPointGauge_ForDrawing.Add(new PGauge(objWorldShape));
            m_arrPointGauge_ForDrawing.Add(new PGauge(objWorldShape));
            m_arrPointGauge_ForDrawing.Add(new PGauge(objWorldShape));
            m_arrPointGauge_ForDrawing.Add(new PGauge(objWorldShape));

            // Init variables
            for (int i = 0; i < 12; i++)
            {
                m_arrLineGauge.Add(new LGauge(objWorldShape));
                m_arrGaugeEnableSubGauge.Add(false);
                m_arrGaugeMeasureMode.Add(0);
                m_arrReCheckUsingPointGaugeIfFail.Add(false);
                m_arrGaugeMinScore.Add(40);
                m_arrPointGaugeOffset.Add(3);
                m_arrGaugeMaxAngle.Add(3);
                m_arrGaugeImageMode.Add(0);
                m_arrGaugeImageNo.Add(0);
                m_arrGaugeImageGain.Add(1);
                m_arrGaugeGroupTolerance.Add(1.5f);
                m_arrGaugeImageThreshold.Add(125);
                m_arrGaugeWantImageThreshold.Add(false);
                m_blnInToOut.Add(true);
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

            m_arrDontCareROI.Add(new List<ROI>());
            m_arrDontCareROI.Add(new List<ROI>());
            m_arrDontCareROI.Add(new List<ROI>());
            m_arrDontCareROI.Add(new List<ROI>());

            // Set line gauge for different direction
            SetDirection(true);

            InitGaugeRecordList();
        }

        ~RectGaugeM4L()
        {
            Dispose();
        }

        public bool Measure(ImageDrawing objImage)
        {
            ROI objROI = new ROI();
            objROI.AttachImage(objImage);
            objROI.LoadROISetting(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);

            bool blnResult = GetPosition_UsingEdgeAndCornerLineGauge(objROI);

            objROI.Dispose();
            return blnResult;
        }

        public bool Measure_WithDontCareArea(List<ImageDrawing> arrImage, ROI objSourceROI, ImageDrawing objWhiteImage)
        {
            List<List<ImageDrawing>> arrModifiedImage = new List<List<ImageDrawing>>();
            ImageDrawing[] objPreModifiedImage = new ImageDrawing[4] { null, null, null, null };

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

                if (intImageIndex >= arrModifiedImage.Count)
                    continue;

                if (m_arrGaugeImageMode[i] == 0)
                {
                    if (m_arrGaugeWantImageThreshold[i])
                    {
                        if (objPreModifiedImage[i] == null)
                        {
                            objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                        }

                        m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageThreshold[i]);

                        m_arrEdgeROI[i].AttachImage(objImage);
                    }
                    else
                    {
                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageGain[i]);
                    }
                }
                else
                {
                    //if (objPreModifiedImage[i] == null)
                    //{
                    //    objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                    //}

                    //m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                    //m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                    //ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                    //m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                    //m_arrEdgeROI[i].PrewittTo_ROIToROISamePosition(ref objImage);

                    if (objPreModifiedImage[i] == null)
                    {
                        objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                    }

                    m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                    m_arrEdgeROI[i].HighPassTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i]);

                    if (m_arrGaugeWantImageThreshold[i])
                    {
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageThreshold[i]);

                        m_arrEdgeROI[i].AttachImage(objImage);
                    }
                    else
                    {
                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageGain[i]);
                    }
                }

                m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
            }

            // if dont care present
            if (m_arrDontCareROI[0].Count != 0 || m_arrDontCareROI[1].Count != 0 || m_arrDontCareROI[2].Count != 0 || m_arrDontCareROI[3].Count != 0)
            {
                ModifyDontCareImage(arrModifiedImage, objWhiteImage);

                List<List<ROI>> arrROI = new List<List<ROI>>();
                for (int i = 0; i < arrModifiedImage.Count; i++)
                {
                    arrROI.Add(new List<ROI>());
                    for (int j = 0; j < arrModifiedImage[i].Count; j++)
                    {
                        if (arrModifiedImage[i][j] != null)
                        {
                            arrROI[i].Add(new ROI());
                            arrROI[i][j].AttachImage(arrModifiedImage[i][j]);
                            arrROI[i][j].LoadROISetting(objSourceROI.ref_ROIPositionX, objSourceROI.ref_ROIPositionY, objSourceROI.ref_ROIWidth, objSourceROI.ref_ROIHeight);
                        }
                    }
                }

                //bool blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);

                ////if (!blnResult && m_arrGaugeEnableSubGauge.Contains(true))
                //if (!blnResult && m_arrGaugeMeasureMode.Contains(1))  
                //    blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);

                bool blnResult;
                if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(arrROI);
                }
                else
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);
                }

                if (!blnResult)
                {
                    if (m_arrGaugeMeasureMode.Contains(1))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);
                    }
                    else
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(arrROI);
                    }
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

                for (int i = 0; i < arrROI.Count; i++)
                {
                    for (int j = 0; j < arrROI[i].Count; j++)
                    {
                        if (arrROI[i][j] != null)
                        {
                            arrROI[i][j].Dispose();
                            arrROI[i][j] = null;
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

                return blnResult;
            }
            else
            {
                List<List<ROI>> arrROI = new List<List<ROI>>();
                for (int i = 0; i < arrModifiedImage.Count; i++)
                {
                    arrROI.Add(new List<ROI>());
                    for (int j = 0; j < arrModifiedImage[i].Count; j++)
                    {
                        if (arrModifiedImage[i][j] != null)
                        {
                            arrROI[i].Add(new ROI());
                            arrROI[i][j].AttachImage(arrModifiedImage[i][j]); // 2020-02-03 ZJYEOH : Should attach to arrModifiedImage instead of arrImage
                            arrROI[i][j].LoadROISetting(objSourceROI.ref_ROIPositionX, objSourceROI.ref_ROIPositionY, objSourceROI.ref_ROIWidth, objSourceROI.ref_ROIHeight);
                        }
                        else
                            arrROI[i].Add(new ROI());
                    }
                }

                //bool blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);

                ////if (!blnResult && m_arrGaugeEnableSubGauge.Contains(true))
                //if (!blnResult && m_arrGaugeMeasureMode.Contains(1))
                //    blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);
                
                bool blnResult;
                if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(arrROI);
                }
                else
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);
                }

                if (!blnResult)
                {
                    if (m_arrGaugeMeasureMode.Contains(1))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);
                    }
                    else
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(arrROI);
                    }
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

                for (int i = 0; i < arrROI.Count; i++)
                {
                    for (int j = 0; j < arrROI[i].Count; j++)
                    {
                        if (arrROI[i][j] != null)
                        {
                            arrROI[i][j].Dispose();
                            arrROI[i][j] = null;
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

                return blnResult;
            }
        }

        public bool Measure_WithDontCareArea(List<ImageDrawing> arrImage, ImageDrawing objWhiteImage)
        {
            List<List<ImageDrawing>> arrModifiedImage = new List<List<ImageDrawing>>();
            ImageDrawing[] objPreModifiedImage = new ImageDrawing[4] { null, null, null, null };

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

                if (m_arrGaugeImageMode[i] == 0)
                {
                    if (m_arrGaugeWantImageThreshold[i])
                    {
                        if (objPreModifiedImage[i] == null)
                        {
                            objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                        }

                        m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageThreshold[i]);

                        m_arrEdgeROI[i].AttachImage(objImage);
                    }
                    else
                    {
                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageGain[i]);
                    }
                }
                else
                {
                    //if (objPreModifiedImage[i] == null)
                    //{
                    //    objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                    //}

                    //m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                    //m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                    //ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                    //m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                    //m_arrEdgeROI[i].PrewittTo_ROIToROISamePosition(ref objImage);

                    if (objPreModifiedImage[i] == null)
                    {
                        objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                    }

                    m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                    m_arrEdgeROI[i].HighPassTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i]);

                    if (m_arrGaugeWantImageThreshold[i])
                    {
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageThreshold[i]);

                        m_arrEdgeROI[i].AttachImage(objImage);
                    }
                    else
                    {
                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageGain[i]);
                    }
                }

                m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
            }

            // if dont care present
            if (m_arrDontCareROI[0].Count != 0 || m_arrDontCareROI[1].Count != 0 || m_arrDontCareROI[2].Count != 0 || m_arrDontCareROI[3].Count != 0)
            {
                ModifyDontCareImage(arrModifiedImage, objWhiteImage);

                List<List<ROI>> arrROI = new List<List<ROI>>();
                for (int i = 0; i < arrModifiedImage.Count; i++)
                {
                    arrROI.Add(new List<ROI>());
                    for (int j = 0; j < arrModifiedImage[i].Count; j++)
                    {
                        arrROI[i].Add(new ROI());

                        if (arrModifiedImage[i][j] != null)
                        {
                            arrROI[i][j].AttachImage(arrModifiedImage[i][i]);
                            arrROI[i][j].LoadROISetting(0, 0, arrModifiedImage[i][j].ref_intImageWidth, arrModifiedImage[i][j].ref_intImageHeight);
                        }
                    }
                }

                //bool blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);

                ////if (!blnResult && m_arrGaugeEnableSubGauge.Contains(true))
                //if (!blnResult && m_arrGaugeMeasureMode.Contains(1))
                //    blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);


                bool blnResult;
                if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(arrROI);
                }
                else
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);
                }

                if (!blnResult)
                {
                    if (m_arrGaugeMeasureMode.Contains(1))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);
                    }
                    else
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(arrROI);
                    }
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

                for (int i = 0; i < arrROI.Count; i++)
                {
                    for (int j = 0; j < arrROI[i].Count; j++)
                    {
                        if (arrROI[i][j] != null)
                        {
                            arrROI[i][j].Dispose();
                            arrROI[i][j] = null;
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

                return blnResult;
            }
            else
            {
                List<List<ROI>> arrROI = new List<List<ROI>>();
                //for (int i = 0; i < arrImage.Count; i++)
                //{
                //    arrROI.Add(new ROI());
                //    arrROI[i].AttachImage(arrImage[i]);
                //    arrROI[i].LoadROISetting(0, 0, arrImage[i].ref_intImageWidth, arrImage[i].ref_intImageHeight);
                //}

                for (int i = 0; i < arrModifiedImage.Count; i++)
                {
                    arrROI.Add(new List<ROI>());
                    for (int j = 0; j < arrModifiedImage[i].Count; j++)
                    {
                        if (arrModifiedImage[i][j] != null)
                        {
                            arrROI[i].Add(new ROI());
                            arrROI[i][j].AttachImage(arrModifiedImage[i][j]);
                            arrROI[i][j].LoadROISetting(0, 0, arrModifiedImage[i][j].ref_intImageWidth, arrModifiedImage[i][j].ref_intImageHeight);
                        }
                        else
                        {
                            arrROI[i].Add(new ROI());
                        }
                    }
                }

                bool blnResult;
                if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(arrROI);
                }
                else
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);
                }

                //if (!blnResult && m_arrGaugeEnableSubGauge.Contains(true))
                //if (!blnResult && m_arrGaugeMeasureMode.Contains(1))
                //    blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);
                if (!blnResult)
                {
                    if (m_arrGaugeMeasureMode.Contains(1))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(arrROI);
                    }
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

                for (int i = 0; i < arrROI.Count; i++)
                {
                    for (int j = 0; j < arrROI[i].Count; j++)
                    {
                        if (arrROI[i][j] != null)
                        {
                            arrROI[i][j].Dispose();
                            arrROI[i][j] = null;
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

                return blnResult;
            }
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
                if (m_arrDontCareROI[i].Count > 0)
                {
                    float fDontCareLengthTotal = 0;
                    if (i == 0 || i == 1)   // Top and Bottom Edge ROI
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
        private void FilterEdgeAngle_Pad(List<LGauge> arrLineGauges, float fMinBorderScore, float[] arrPointGaugeAngle, ref float fTotalAngle, ref int nCount, ref bool[] blnResult)
        {
            blnResult = new bool[4];

            // Filter angle with score
            float[] fUnsortAngle = new float[4];
            for (int i = 0; i < 4; i++)
            {
                if (arrPointGaugeAngle[i] == -999)
                {
                    if (m_arrDontCareROI[i].Count > 0)
                    {
                        float fDontCareLengthTotal = 0;
                        if (i == 0 || i == 1)   // Top and Bottom Edge ROI
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
                    blnResult[i] = true;
                    fUnsortAngle[i] = arrPointGaugeAngle[i];
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
            FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            // Build a virtual edge line if the edge is fail
            if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
            {
                m_arrLineResultOK[0] = false;
                Line objLine = new Line();
                m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                m_arrLineGauge[0].ref_ObjectLine = objLine;
                m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
            }
            else
                m_arrLineResultOK[0] = true;
            if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
            {
                m_arrLineResultOK[1] = false;
                Line objLine = new Line();
                m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                m_arrLineGauge[1].ref_ObjectLine = objLine;
                m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth); //+, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
            }
            else
                m_arrLineResultOK[1] = true;
            if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
            {
                m_arrLineResultOK[2] = false;
                Line objLine = new Line();
                m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                m_arrLineGauge[2].ref_ObjectLine = objLine;
                m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go up
            }
            else
                m_arrLineResultOK[2] = true;
            if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
            {
                m_arrLineResultOK[3] = false;
                Line objLine = new Line();
                m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                m_arrLineGauge[3].ref_ObjectLine = objLine;
                m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);//- , 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
            }
            else
                m_arrLineResultOK[3] = true;
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
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 200");
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
                    if (!arrAngleOK[0] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Left angle
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

                    if (!arrAngleOK[1] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
                    {
                        if (!arrAngleOK[0] || (intBestHorIndex != 0  && intBestHorIndex != -1))
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

                    if (!arrAngleOK[2] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                    if (!arrAngleOK[3] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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
                //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                    m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                //else
                //    isResultOK = false;

                if (m_pRectCenterPoint.X.ToString() != "NaN" && m_pRectCenterPoint.Y.ToString() != "NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "-NaN" && m_pRectCenterPoint.Y.ToString() != "-NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "Infinity" && m_pRectCenterPoint.Y.ToString() != "Infinity" &&
                                     m_pRectCenterPoint.X.ToString() != "-Infinity" && m_pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                     m_fRectWidth != 0 && m_fRectHeight != 0)
                    isResultOK = true;
                else
                {
                   m_strErrorMessage = "Measure Edge Fail!";
                    isResultOK = false;
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
            return true;
        }

        private bool GetPosition_UsingEdgeAndCornerLineGauge(List<ROI> arrROI)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                m_arrLineGauge[i].Measure(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)], i);
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
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            bool blnEnableVirtualLine = false;
            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    m_arrLineResultOK[0] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                else
                    m_arrLineResultOK[0] = true;
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    m_arrLineResultOK[1] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth); //+, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[1] = true;
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    m_arrLineResultOK[2] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go up
                }
                else
                    m_arrLineResultOK[2] = true;
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    m_arrLineResultOK[3] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);//- , 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[3] = true;
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
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 201");
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
                        if (!arrAngleOK[0] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Left angle
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

                        if (!arrAngleOK[1] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[2] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[3] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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
                    else
                        break;
                }

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

                m_fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;


                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;

                // Verify Unit Size
                //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                //else
                //    isResultOK = false;

                if (m_pRectCenterPoint.X.ToString() != "NaN" && m_pRectCenterPoint.Y.ToString() != "NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "-NaN" && m_pRectCenterPoint.Y.ToString() != "-NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "Infinity" && m_pRectCenterPoint.Y.ToString() != "Infinity" &&
                                     m_pRectCenterPoint.X.ToString() != "-Infinity" && m_pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                     m_fRectWidth != 0 && m_fRectHeight != 0)
                    isResultOK = true;
                else
                {
                    m_strErrorMessage = "Measure Edge Fail!";
                    isResultOK = false;
                }

            }

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
            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGauge(List<List<ROI>> arrROI)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intROIIndex >= arrROI.Count)
                    intROIIndex = 0;
                m_arrLineGauge[i].Measure(arrROI[intROIIndex][i], i);
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
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            bool blnEnableVirtualLine = false;
            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    m_arrLineResultOK[0] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                else
                    m_arrLineResultOK[0] = true;
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    m_arrLineResultOK[1] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth); //+, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[1] = true;
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    m_arrLineResultOK[2] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go up
                }
                else
                    m_arrLineResultOK[2] = true;
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    m_arrLineResultOK[3] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);//- , 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[3] = true;
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
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 202");
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
                        if (!arrAngleOK[0] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Left angle
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

                        if (!arrAngleOK[1] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[2] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[3] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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
                    else
                        break;
                }

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

                m_fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;


                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;

                // Verify Unit Size
                //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                //else
                //    isResultOK = false;

                if (m_pRectCenterPoint.X.ToString() != "NaN" && m_pRectCenterPoint.Y.ToString() != "NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "-NaN" && m_pRectCenterPoint.Y.ToString() != "-NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "Infinity" && m_pRectCenterPoint.Y.ToString() != "Infinity" &&
                                     m_pRectCenterPoint.X.ToString() != "-Infinity" && m_pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                     m_fRectWidth != 0 && m_fRectHeight != 0)
                    isResultOK = true;
                else
                {
                    m_strErrorMessage = "Measure Edge Fail!";
                    isResultOK = false;
                }

            }

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
            return true;
        }

        private bool GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(List<ROI> arrROI)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                //if (m_arrGaugeEnableSubGauge[i] && !m_arrLineResultOK[i])
                if (m_arrGaugeMeasureMode[i] == 1 && !m_arrLineResultOK[i])
                    m_arrLineGauge[i].Measure_SubGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)], m_arrGaugeMinScore[i]);
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
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            bool blnEnableVirtualLine = false;
            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    m_arrLineResultOK[0] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                else
                    m_arrLineResultOK[0] = true;
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    m_arrLineResultOK[1] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth); //+, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[1] = true;
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    m_arrLineResultOK[2] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go up
                }
                else
                    m_arrLineResultOK[2] = true;
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    m_arrLineResultOK[3] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);//- , 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[3] = true;
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
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 203");
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
                        if (!arrAngleOK[0] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Left angle
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

                        if (!arrAngleOK[1] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[2] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[3] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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
                    else
                        break;
                }

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

                m_fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;


                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;

                // Verify Unit Size
                //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                //else
                //    isResultOK = false;

                if (m_pRectCenterPoint.X.ToString() != "NaN" && m_pRectCenterPoint.Y.ToString() != "NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "-NaN" && m_pRectCenterPoint.Y.ToString() != "-NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "Infinity" && m_pRectCenterPoint.Y.ToString() != "Infinity" &&
                                     m_pRectCenterPoint.X.ToString() != "-Infinity" && m_pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                     m_fRectWidth != 0 && m_fRectHeight != 0)
                    isResultOK = true;
                else
                {
                    m_strErrorMessage = "Measure Edge Fail!";
                    isResultOK = false;
                }

            }

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
            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(List<List<ROI>> arrROI)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                //if (m_arrGaugeEnableSubGauge[i] && !m_arrLineResultOK[i])
                if (m_arrGaugeMeasureMode[i] == 1 && !m_arrLineResultOK[i])
                    m_arrLineGauge[i].Measure_SubGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i], m_arrGaugeMinScore[i]);
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
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            bool blnEnableVirtualLine = false;
            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    m_arrLineResultOK[0] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                else
                    m_arrLineResultOK[0] = true;
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    m_arrLineResultOK[1] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth); //+, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[1] = true;
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    m_arrLineResultOK[2] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go up
                }
                else
                    m_arrLineResultOK[2] = true;
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    m_arrLineResultOK[3] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);//- , 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[3] = true;
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
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 204");
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
                        if (!arrAngleOK[0] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Left angle
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

                        if (!arrAngleOK[1] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[2] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[3] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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
                    else
                        break;
                }

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

                m_fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;


                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;

                // Verify Unit Size
                //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                //else
                //    isResultOK = false;

                if (m_pRectCenterPoint.X.ToString() != "NaN" && m_pRectCenterPoint.Y.ToString() != "NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "-NaN" && m_pRectCenterPoint.Y.ToString() != "-NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "Infinity" && m_pRectCenterPoint.Y.ToString() != "Infinity" &&
                                     m_pRectCenterPoint.X.ToString() != "-Infinity" && m_pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                     m_fRectWidth != 0 && m_fRectHeight != 0)
                    isResultOK = true;
                else
                {
                    m_strErrorMessage = "Measure Edge Fail!";
                    isResultOK = false;
                }

            }

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
            return true;
        }
        public float AddGrayColorToGaugePoint(ROI objROI, int intGaugeIndex, int intThreshold)
        {
            return m_arrLineGauge[intGaugeIndex].AddGrayColorToGaugePoint(objROI, 0, intGaugeIndex, intThreshold);
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
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 205");
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
                if (arrFailLines2[0] && m_fUnitSizeHeight > 0)
                {
                    Line objLine = new Line();
                    m_arrCornerLine[4].CopyTo(ref objLine);
                    m_arrCornerLine[0] = objLine;
                    m_arrCornerLine[0].ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                else if (arrFailLines2[1] && m_fUnitSizeWidth > 0)
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
                if (arrFailLines2[2] && m_fUnitSizeHeight > 0)
                {
                    Line objLine = new Line();
                    m_arrCornerLine[6].CopyTo(ref objLine);
                    m_arrCornerLine[2] = objLine;
                    m_arrCornerLine[2].ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up

                }
                else if (arrFailLines2[3] && m_fUnitSizeWidth > 0)
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
                if (arrFailLines2[6] && m_fUnitSizeHeight > 0)
                {
                    Line objLine = new Line();
                    m_arrCornerLine[2].CopyTo(ref objLine);
                    m_arrCornerLine[6] = objLine;
                    m_arrCornerLine[6].ShiftYLine(m_fUnitSizeHeight);    // use "+" because Y increase when go down
                }
                else if (arrFailLines2[7] && m_fUnitSizeWidth > 0)
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
                if (arrFailLines2[4] && m_fUnitSizeHeight > 0)
                {
                    Line objLine = new Line();
                    m_arrCornerLine[0].CopyTo(ref objLine);
                    m_arrCornerLine[4] = objLine;
                    m_arrCornerLine[4].ShiftYLine(m_fUnitSizeHeight);    // use "+" because Y increase when go down
                }
                else if (arrFailLines2[5] && m_fUnitSizeWidth > 0)
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
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 206");
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

        public void SetGaugeAngle(float fAngle)
        {
            if (fAngle != -1)
            {
                m_arrLineGauge[0].SetGaugeAngle(180 + fAngle);
                m_arrLineGauge[1].SetGaugeAngle(-90 + fAngle);
                m_arrLineGauge[2].SetGaugeAngle(fAngle);
                m_arrLineGauge[3].SetGaugeAngle(90 + fAngle);
            }
            else
            {
                m_arrLineGauge[0].SetGaugeAngle(180);
                m_arrLineGauge[1].SetGaugeAngle(-90);
                m_arrLineGauge[2].SetGaugeAngle(0);
                m_arrLineGauge[3].SetGaugeAngle(90);
            }
        }

        //Set the placement for blue square
        public void SetGaugePlace(ROI objROI)
        {
            // Top
            m_arrLineGauge[0].SetGaugeLength(objROI.ref_ROI.Width);
            m_arrLineGauge[0].SetGaugeCenter(objROI.ref_ROITotalCenterX, objROI.ref_ROITotalY + m_arrLineGauge[0].ref_GaugeTolerance);
            
            // Right
            m_arrLineGauge[1].SetGaugeLength(objROI.ref_ROIHeight);
            m_arrLineGauge[1].SetGaugeCenter(objROI.ref_ROITotalX + objROI.ref_ROIWidth - m_arrLineGauge[1].ref_GaugeTolerance , objROI.ref_ROITotalCenterY);

            // Bottom
            m_arrLineGauge[2].SetGaugeLength(objROI.ref_ROI.Width);
            m_arrLineGauge[2].SetGaugeCenter(objROI.ref_ROITotalCenterX, objROI.ref_ROITotalY + objROI.ref_ROIHeight - m_arrLineGauge[2].ref_GaugeTolerance );

            // Left
            m_arrLineGauge[3].SetGaugeLength(objROI.ref_ROIHeight);
            m_arrLineGauge[3].SetGaugeCenter(objROI.ref_ROITotalX + m_arrLineGauge[3].ref_GaugeTolerance, objROI.ref_ROITotalCenterY);
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
            objFile.WriteElement1Value("GainValue", m_fGainValue, "Image Gain Value", true);

            if (blnSaveMeasurementAsTemplate)
            {
                m_fUnitSizeWidth = m_fRectWidth;
                m_fUnitSizeHeight = m_fRectHeight;
            }

            objFile.WriteElement1Value("UnitSizeWidth", m_fUnitSizeWidth, "Unit Size Width", true);//m_fUnitSizeWidth
            objFile.WriteElement1Value("UnitSizeHeight", m_fUnitSizeHeight, "Unit Size Height", true);//m_fUnitSizeHeight

            objFile.WriteElement1Value("DrawDraggingBox", m_blnDrawDraggingBox, "Draw Dragging Box", true);
            objFile.WriteElement1Value("DrawSamplingPoint", m_blnDrawSamplingPoint, "Draw Sampling Point", true);
            
            m_f4LGaugeCenterPointX = m_pRectCenterPoint.X; //(m_arrLineGauge[1].ref_GaugeCenterX + m_arrLineGauge[3].ref_GaugeCenterX) / 2; //28-05-2019 ZJYEOH: Should save the corner point formula's center X and Y  
            m_f4LGaugeCenterPointY = m_pRectCenterPoint.Y; //(m_arrLineGauge[0].ref_GaugeCenterY + m_arrLineGauge[2].ref_GaugeCenterY) / 2;
            objFile.WriteElement1Value("Gauge4LCenterPointX", m_f4LGaugeCenterPointX, "4L Line Gauge Combine Center Point X", true);
            objFile.WriteElement1Value("Gauge4LCenterPointY", m_f4LGaugeCenterPointY, "4L Line Gauge Combine Center Point Y", true);
            //objFile.WriteElement1Value("InToOut", IsDirectionInToOut(), "Direction InToOut", true);

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
                objFile.WriteElement1Value("ReCheckUsingPointGaugeIfFail", m_arrReCheckUsingPointGaugeIfFail[i], "Gauge Point Mode", true);
                objFile.WriteElement1Value("GaugeMinScore", m_arrGaugeMinScore[i], "Gauge Min Score", true);
                objFile.WriteElement1Value("GaugeMaxAngle", m_arrGaugeMaxAngle[i], "Gauge Max Angle", true);
                objFile.WriteElement1Value("PointGaugeOffset", m_arrPointGaugeOffset[i], "Point Gauge Offset", true);
                objFile.WriteElement1Value("GaugeImageMode", m_arrGaugeImageMode[i], "Gauge Image Mode", true);
                objFile.WriteElement1Value("GaugeImageNo", m_arrGaugeImageNo[i], "Gauge Image No", true);
                objFile.WriteElement1Value("GaugeImageGain", m_arrGaugeImageGain[i], "Gauge Image Gain", true);
                objFile.WriteElement1Value("GaugeGroupTolerance", m_arrGaugeGroupTolerance[i], "Gauge Group Tolerance", true);
                objFile.WriteElement1Value("GaugeImageThreshold", m_arrGaugeImageThreshold[i], "Gauge Image Threshold", true);
                objFile.WriteElement1Value("GaugeWantImageThreshold", m_arrGaugeWantImageThreshold[i], "Gauge Want Image Threshold", true);

                objFile.WriteElement1Value("IntoOut", m_blnInToOut[i], "Gauge Direction In To Out", true);
            }

            if (strSectionName == "RectG1")                                                         // 2021 05 27 - CCENG: For double inpocket package
                objFile.WriteSectionElement("PadDirectionG1_" + m_intDirection, blnNewSection);     // 2021 05 27 - CCENG: For double inpocket package
            else
                objFile.WriteSectionElement("PadDirection" + m_intDirection, blnNewSection);

            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                objFile.WriteElement1Value("EdgeROI_" + i, "");

                objFile.WriteElement2Value("PositionX", m_arrEdgeROI[i].ref_ROIPositionX);
                objFile.WriteElement2Value("PositionY", m_arrEdgeROI[i].ref_ROIPositionY);
                objFile.WriteElement2Value("Width", m_arrEdgeROI[i].ref_ROIWidth);
                objFile.WriteElement2Value("Height", m_arrEdgeROI[i].ref_ROIHeight);
                objFile.WriteElement2Value("CenterX", m_intEdgeROICenterX[i]);
                objFile.WriteElement2Value("CenterY", m_intEdgeROICenterY[i]);
                objFile.WriteElement2Value("OffsetX", m_intEdgeROIOffsetX[i]);
                objFile.WriteElement2Value("OffsetY", m_intEdgeROIOffsetY[i]);
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

            objFile.WriteEndElement();

            SetUserSettingToUserVariables();
        }

        public void SaveRectGauge4L_SECSGEM(string strPath, string strSectionName, string strVisionName)
        {
            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");

            //XmlParser objFile = new XmlParser(strPath, blnNewFile);
            //objFile.WriteSectionElement(strSectionName + "_RectSetting", blnNewSection);

            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_GainValue", m_fGainValue);
            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_UnitSizeWidth", m_fRectWidth);//m_fUnitSizeWidth
            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_UnitSizeHeight", m_fRectHeight);//m_fUnitSizeHeight

            m_fUnitSizeWidth = m_fRectWidth;
            m_fUnitSizeHeight = m_fRectHeight;

            m_f4LGaugeCenterPointX = m_pRectCenterPoint.X; //(m_arrLineGauge[1].ref_GaugeCenterX + m_arrLineGauge[3].ref_GaugeCenterX) / 2; //28-05-2019 ZJYEOH: Should save the corner point formula's center X and Y  
            m_f4LGaugeCenterPointY = m_pRectCenterPoint.Y; //(m_arrLineGauge[0].ref_GaugeCenterY + m_arrLineGauge[2].ref_GaugeCenterY) / 2;
            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_Gauge4LCenterPointX", m_f4LGaugeCenterPointX);
            objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_Gauge4LCenterPointY", m_f4LGaugeCenterPointY);
            //objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_InToOut", IsDirectionInToOut());

            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                //objFile.WriteSectionElement(strSectionName + "_" + i, blnNewSection);

                //// Rectangle gauge template measurement result
                //if (blnSaveMeasurementAsTemplate)
                //{
                //    //objFile.WriteElementValue("ObjectCenterX", m_RectGauge.MeasuredRectangle.CenterX, "Gauge Measurement Result CenterX", true);
                //    //objFile.WriteElementValue("ObjectCenterY", m_RectGauge.MeasuredRectangle.CenterY, "Gauge Measurement Result CenterY", true);
                //    //objFile.WriteElementValue("ObjectWidth", m_RectGauge.MeasuredRectangle.SizeX, "Gauge Measurement Result Width", true);
                //    //objFile.WriteElementValue("ObjectHeight", m_RectGauge.MeasuredRectangle.SizeY, "Gauge Measurement Result Height", true);
                //    //objFile.WriteElementValue("ObjectAngle", m_RectGauge.MeasuredRectangle.Angle, "Gauge Measurement Result Angle", true);

                //    //m_fTemplateObjectCenterX = m_RectGauge.MeasuredRectangle.CenterX;
                //    //m_fTemplateObjectCenterY = m_RectGauge.MeasuredRectangle.CenterY;
                //    //m_fTemplateObjectWidth = m_RectGauge.MeasuredRectangle.SizeX;
                //    //m_fTemplateObjectHeight = m_RectGauge.MeasuredRectangle.SizeY;
                //}

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
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_ReCheckUsingPointGaugeIfFail", m_arrReCheckUsingPointGaugeIfFail[i]);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeMinScore", m_arrGaugeMinScore[i]);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeMaxAngle", m_arrGaugeMaxAngle[i]);
                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_PointGaugeOffset", m_arrPointGaugeOffset[i]);
                objFile.WriteElement1Value(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeImageMode", m_arrGaugeImageMode[i]);
                objFile.WriteElement1Value(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeImageNo", m_arrGaugeImageNo[i]);
                objFile.WriteElement1Value(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeImageGain", m_arrGaugeImageGain[i]);
                objFile.WriteElement1Value(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeGroupTolerance", m_arrGaugeGroupTolerance[i]);
                objFile.WriteElement1Value(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeImageThreshold", m_arrGaugeImageThreshold[i]);
                objFile.WriteElement1Value(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_GaugeWantImageThreshold", m_arrGaugeWantImageThreshold[i]);

                objFile.WriteElementValue(strVisionName + "_RectSetting_" + strSectionName + "_" + i + "_IntoOut", m_blnInToOut[i]);
            }

            ////objFile.WriteSectionElement("PadDirection" + m_intDirection, blnNewSection);
            //for (int i = 0; i < m_arrEdgeROI.Count; i++)
            //{
            //    //objFile.WriteElementValue(strVisionName + "_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i, "");

            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_PositionX", m_arrEdgeROI[i].ref_ROIPositionX);
            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_PositionY", m_arrEdgeROI[i].ref_ROIPositionY);
            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_Width", m_arrEdgeROI[i].ref_ROIWidth);
            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_Height", m_arrEdgeROI[i].ref_ROIHeight);
            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_CenterX", m_intEdgeROICenterX[i]);
            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_CenterY", m_intEdgeROICenterY[i]);
            //    objFile.WriteElementValue(strVisionName + "_RectSetting_" + "PadDirection" + m_intDirection + "_EdgeROI_" + i + "_DontCareROICount", m_arrDontCareROI[i].Count);
            //    //for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
            //    //{
            //    //    //objFile.WriteElementValue(strVisionName + "_" + "PadDirection" + m_intDirection + "_DontCareROI_" + i + "_" + j, "");
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
                m_arrLineGauge[i].ref_GaugeTransType = objFile.GetValueAsInt("TransType", 0);
                m_arrLineGauge[i].ref_GaugeTransChoice = objFile.GetValueAsInt("TransChoice", 0);
                m_arrLineGauge[i].ref_GaugeThreshold = objFile.GetValueAsInt("Threshold", 2);
                m_arrLineGauge[i].ref_GaugeThickness = objFile.GetValueAsInt("Thickness", 13);
                m_arrLineGauge[i].ref_GaugeMinAmplitude = objFile.GetValueAsInt("MinAmp", 10);
                m_arrLineGauge[i].ref_GaugeMinArea = objFile.GetValueAsInt("MinArea", 0);
                m_arrLineGauge[i].ref_GaugeFilter = objFile.GetValueAsInt("Filter", 1);

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
                m_arrReCheckUsingPointGaugeIfFail[i] = objFile.GetValueAsBoolean("ReCheckUsingPointGaugeIfFail", false);
                m_arrGaugeMinScore[i] = objFile.GetValueAsInt("GaugeMinScore", 40);
                m_arrGaugeMaxAngle[i] = objFile.GetValueAsInt("GaugeMaxAngle", 3);
                m_arrPointGaugeOffset[i] = objFile.GetValueAsInt("PointGaugeOffset", 3);
                m_arrGaugeImageMode[i] = objFile.GetValueAsInt("GaugeImageMode", 0);
                m_arrGaugeImageNo[i] = objFile.GetValueAsInt("GaugeImageNo", 0);
                m_arrGaugeImageGain[i] = objFile.GetValueAsFloat("GaugeImageGain", 1);
                m_arrGaugeGroupTolerance[i] = objFile.GetValueAsFloat("GaugeGroupTolerance", 1.5f);
                m_arrGaugeImageThreshold[i] = objFile.GetValueAsInt("GaugeImageThreshold", 125);
                m_arrGaugeWantImageThreshold[i] = objFile.GetValueAsBoolean("GaugeWantImageThreshold", false);

                // Rectangle gauge other extra setting
                m_fLength = m_arrLineGauge[i].ref_GaugeLength;

                m_blnInToOut[i] = objFile.GetValueAsBoolean("IntoOut", objFile.GetValueAsBoolean("InToOut", true));
                SetDirection(m_blnInToOut[i], i);
            }
        }
        public void LoadRectGauge4L(string strPath, string strSectionName)
        {
            XmlParser objFile = new XmlParser(strPath);

            objFile.GetFirstSection(strSectionName + "_RectSetting");
            m_fGainValue = objFile.GetValueAsFloat("GainValue", 1000);
            m_fUnitSizeWidth = objFile.GetValueAsFloat("UnitSizeWidth", 0);
            m_fUnitSizeHeight = objFile.GetValueAsFloat("UnitSizeHeight", 0);
            m_f4LGaugeCenterPointX = objFile.GetValueAsFloat("Gauge4LCenterPointX", 0F);
            m_f4LGaugeCenterPointY = objFile.GetValueAsFloat("Gauge4LCenterPointY", 0);
            //SetDirection(objFile.GetValueAsBoolean("InToOut", true));

            m_blnDrawDraggingBox = objFile.GetValueAsBoolean("DrawDraggingBox", true);
            m_blnDrawSamplingPoint = objFile.GetValueAsBoolean("DrawSamplingPoint", true);

            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                objFile.GetFirstSection(strSectionName + "_" + i);

                // Rectangle gauge position setting
                m_arrLineGauge[i].SetGaugeCenter(objFile.GetValueAsFloat("CenterX", 0),
                                      objFile.GetValueAsFloat("CenterY", 0));
                m_arrLineGauge[i].SetGaugeLength(objFile.GetValueAsFloat("Length", 100));
                //  m_arrLineGauge[i].SetGaugeAngle(objFile.GetValueAsFloat("Angle", 0)); // Not need to load angle. Angle has been define during init this RectGaugeM4L class.
                m_arrLineGauge[i].ref_GaugeTolerance = objFile.GetValueAsFloat("Tolerance", 10);

                // Rectangle gauge measurement setting
                m_arrLineGauge[i].ref_GaugeTransType = objFile.GetValueAsInt("TransType", 0);
                m_arrLineGauge[i].ref_GaugeTransChoice = objFile.GetValueAsInt("TransChoice", 0);
                m_arrLineGauge[i].ref_GaugeThreshold = objFile.GetValueAsInt("Threshold", 2);
                m_arrLineGauge[i].ref_GaugeThickness = objFile.GetValueAsInt("Thickness", 13);
                m_arrLineGauge[i].ref_GaugeMinAmplitude = objFile.GetValueAsInt("MinAmp", 10);
                m_arrLineGauge[i].ref_GaugeMinArea = objFile.GetValueAsInt("MinArea", 0);
                m_arrLineGauge[i].ref_GaugeFilter = objFile.GetValueAsInt("Filter", 1);

                // Rectangle gauge fitting setting
                m_arrLineGauge[i].ref_GaugeSamplingStep = objFile.GetValueAsInt("SamplingStep", 3);
                m_arrLineGauge[i].ref_GaugeFilterThreshold = objFile.GetValueAsInt("FilteringThreshold", 3);
                m_arrLineGauge[i].ref_GaugeFilterPasses= objFile.GetValueAsInt("FilteringPasses", 3);

                m_arrGaugeEnableSubGauge[i] = objFile.GetValueAsBoolean("GaugeEnableSubGauge", false);
                m_arrGaugeMeasureMode[i] = objFile.GetValueAsInt("GaugeMeasureMode", -1);
                if (m_arrGaugeMeasureMode[i] == -1)   // 2020 03 04 - CCENG: if GaugeMeasureMode no record in database, mean the database is from old version. 
                {
                    if (m_arrGaugeEnableSubGauge[i])    //if sub gauge is true, then set gauge point mode to 1.
                        m_arrGaugeMeasureMode[i] = 1;
                    else
                        m_arrGaugeMeasureMode[i] = 0;
                }
                m_arrReCheckUsingPointGaugeIfFail[i] = objFile.GetValueAsBoolean("ReCheckUsingPointGaugeIfFail", false);
                m_arrGaugeMinScore[i] = objFile.GetValueAsInt("GaugeMinScore", 40);
                m_arrGaugeMaxAngle[i] = objFile.GetValueAsInt("GaugeMaxAngle", 3);
                m_arrPointGaugeOffset[i] = objFile.GetValueAsInt("PointGaugeOffset", 3);
                m_arrGaugeImageMode[i] = objFile.GetValueAsInt("GaugeImageMode", 0);
                m_arrGaugeImageNo[i] = objFile.GetValueAsInt("GaugeImageNo", 0);
                m_arrGaugeImageGain[i] = objFile.GetValueAsFloat("GaugeImageGain", 1);
                m_arrGaugeGroupTolerance[i] = objFile.GetValueAsFloat("GaugeGroupTolerance", 1.5f);
                m_arrGaugeImageThreshold[i] = objFile.GetValueAsInt("GaugeImageThreshold", 125);
                m_arrGaugeWantImageThreshold[i] = objFile.GetValueAsBoolean("GaugeWantImageThreshold", false);

                // Rectangle gauge other extra setting
                m_fLength = m_arrLineGauge[i].ref_GaugeLength;

                m_blnInToOut[i] = objFile.GetValueAsBoolean("IntoOut", objFile.GetValueAsBoolean("InToOut", true));
                SetDirection(m_blnInToOut[i], i);
            }

            if (strSectionName == "RectG1")
                objFile.GetFirstSection("PadDirectionG1_" + m_intDirection);
            else
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
                m_intEdgeROIOffsetX[i] = objFile.GetValueAsInt("OffsetX", 0, 2);
                m_intEdgeROIOffsetY[i] = objFile.GetValueAsInt("OffsetY", 0, 2);

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

            m_arrEdgeROI.Clear();
            m_arrDontCareROI.Clear();
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
            m_arrLineGauge[0].DrawSamplingPointGauge(g);
            m_arrLineGauge[1].DrawSamplingPointGauge(g);
            m_arrLineGauge[2].DrawSamplingPointGauge(g);
            m_arrLineGauge[3].DrawSamplingPointGauge(g);
        }
        public void DrawGaugeResult_SamplingPoint_ForLearning(Graphics g)
        {

            if ((m_intSelectedGaugeEdgeMask & 0x01) > 0)
            {
                if (m_arrReCheckUsingPointGaugeIfFail[0] && m_arrDrawLineUsingCornerPoint[0] && m_arrGaugeMeasureMode[0] == 3)
                {
                    m_arrPointGauge_ForDrawing[0].DrawGaugeSamplePoint(g, Color.Lime);
                }
                else
                {
                    if (m_arrLineResultOK[0])
                        m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.Lime);
                    else
                        m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.Red);

                    if (m_fRectHeight == 0 && (m_arrRectCornerPoints[0].X < 0 || m_arrRectCornerPoints[0].Y < 0 || m_arrRectCornerPoints[1].X < 0 || m_arrRectCornerPoints[1].Y < 0 ||
                        m_arrRectCornerPoints[0].X == m_arrRectCornerPoints[1].X || m_arrRectCornerPoints[0].Y == m_arrRectCornerPoints[1].Y))
                        m_arrLineGauge[0].DrawResultLineGauge(g, Color.Red);
                    else
                        m_arrLineGauge[0].DrawResultLineGauge(g, Color.Yellow);
                }
            }

            if ((m_intSelectedGaugeEdgeMask & 0x02) > 0)
            {
                if (m_arrReCheckUsingPointGaugeIfFail[1] && m_arrDrawLineUsingCornerPoint[1] && m_arrGaugeMeasureMode[1] == 3)
                {
                    m_arrPointGauge_ForDrawing[1].DrawGaugeSamplePoint(g, Color.Lime);
                }
                else
                {
                    if (m_arrLineResultOK[1])
                        m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.Lime);
                    else
                        m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.Red);

                    if (m_fRectWidth == 0 && (m_arrRectCornerPoints[2].X < 0 || m_arrRectCornerPoints[2].Y < 0 || m_arrRectCornerPoints[1].X < 0 || m_arrRectCornerPoints[1].Y < 0 ||
                        m_arrRectCornerPoints[2].X == m_arrRectCornerPoints[1].X || m_arrRectCornerPoints[2].Y == m_arrRectCornerPoints[1].Y))
                        m_arrLineGauge[1].DrawResultLineGauge(g, Color.Red);
                    else
                        m_arrLineGauge[1].DrawResultLineGauge(g, Color.Yellow);
                }
            }

            if ((m_intSelectedGaugeEdgeMask & 0x04) > 0)
            {
                if (m_arrReCheckUsingPointGaugeIfFail[2] && m_arrDrawLineUsingCornerPoint[2] && m_arrGaugeMeasureMode[2] == 3)
                {
                    m_arrPointGauge_ForDrawing[2].DrawGaugeSamplePoint(g, Color.Lime);
                }
                else
                {
                    if (m_arrLineResultOK[2])
                        m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.Lime);
                    else
                        m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.Red);

                    if (m_fRectHeight == 0 && (m_arrRectCornerPoints[2].X < 0 || m_arrRectCornerPoints[2].Y < 0 || m_arrRectCornerPoints[3].X < 0 || m_arrRectCornerPoints[3].Y < 0 ||
                        m_arrRectCornerPoints[2].X == m_arrRectCornerPoints[3].X || m_arrRectCornerPoints[2].Y == m_arrRectCornerPoints[3].Y))
                        m_arrLineGauge[2].DrawResultLineGauge(g, Color.Red);
                    else
                        m_arrLineGauge[2].DrawResultLineGauge(g, Color.Yellow);
                }
            }

            if ((m_intSelectedGaugeEdgeMask & 0x08) > 0)
            {
                if (m_arrReCheckUsingPointGaugeIfFail[3] && m_arrDrawLineUsingCornerPoint[3] && m_arrGaugeMeasureMode[3] == 3)
                {
                    m_arrPointGauge_ForDrawing[3].DrawGaugeSamplePoint(g, Color.Lime);
                }
                else
                {
                    if (m_arrLineResultOK[3])
                        m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.Lime);
                    else
                        m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.Red);

                    if (m_fRectWidth == 0 && (m_arrRectCornerPoints[0].X < 0 || m_arrRectCornerPoints[0].Y < 0 || m_arrRectCornerPoints[3].X < 0 || m_arrRectCornerPoints[3].Y < 0 ||
                        m_arrRectCornerPoints[0].X == m_arrRectCornerPoints[3].X || m_arrRectCornerPoints[0].Y == m_arrRectCornerPoints[3].Y))
                        m_arrLineGauge[3].DrawResultLineGauge(g, Color.Red);
                    else
                        m_arrLineGauge[3].DrawResultLineGauge(g, Color.Yellow);
                }
            }

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
                if (m_arrLineResultOK[0])
                    m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.Green);
                else
                    m_arrLineGauge[0].DrawSamplingPointGauge(g, Color.DarkRed);
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
                if (m_arrLineResultOK[1])
                    m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.Green);
                else
                    m_arrLineGauge[1].DrawSamplingPointGauge(g, Color.DarkRed);
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
                if (m_arrLineResultOK[2])
                    m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.Green);
                else
                    m_arrLineGauge[2].DrawSamplingPointGauge(g, Color.DarkRed);
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
                if (m_arrLineResultOK[3])
                    m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.Green);
                else
                    m_arrLineGauge[3].DrawSamplingPointGauge(g, Color.DarkRed);
            }
        }

        public void DrawGaugeResult_ResultLine(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            //m_arrLineGauge[0].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[1].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[2].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[3].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            if (m_arrDrawLineUsingCornerPoint[0] && m_arrGaugeMeasureMode[0] == 3)
            {
                g.DrawLine(new Pen(Color.Lime), m_arrRectCornerPoints[0].X * fDrawingScaleX, m_arrRectCornerPoints[0].Y * fDrawingScaleY, m_arrRectCornerPoints[1].X * fDrawingScaleX, m_arrRectCornerPoints[1].Y * fDrawingScaleY);
            }
            else
            {
                if (m_arrLineResultOK[0])
                    m_arrLineGauge[0].DrawResultLineGauge(g, Color.LightGreen);
                else
                    m_arrLineGauge[0].DrawResultLineGauge(g, Color.Red);
            }

            if (m_arrDrawLineUsingCornerPoint[1] && m_arrGaugeMeasureMode[1] == 3)
            {
                g.DrawLine(new Pen(Color.Lime), m_arrRectCornerPoints[1].X * fDrawingScaleX, m_arrRectCornerPoints[1].Y * fDrawingScaleY, m_arrRectCornerPoints[2].X * fDrawingScaleX, m_arrRectCornerPoints[2].Y * fDrawingScaleY);
            }
            else
            {
                if (m_arrLineResultOK[1])
                    m_arrLineGauge[1].DrawResultLineGauge(g, Color.LightGreen);
                else
                    m_arrLineGauge[1].DrawResultLineGauge(g, Color.Red);
            }

            if (m_arrDrawLineUsingCornerPoint[2] && m_arrGaugeMeasureMode[2] == 3)
            {
                g.DrawLine(new Pen(Color.Lime), m_arrRectCornerPoints[2].X * fDrawingScaleX, m_arrRectCornerPoints[2].Y * fDrawingScaleY, m_arrRectCornerPoints[3].X * fDrawingScaleX, m_arrRectCornerPoints[3].Y * fDrawingScaleY);
            }
            else
            {
                if (m_arrLineResultOK[2])
                    m_arrLineGauge[2].DrawResultLineGauge(g, Color.LightGreen);
                else
                    m_arrLineGauge[2].DrawResultLineGauge(g, Color.Red);
            }

            if (m_arrDrawLineUsingCornerPoint[3] && m_arrGaugeMeasureMode[3] == 3)
            {
                g.DrawLine(new Pen(Color.Lime), m_arrRectCornerPoints[0].X * fDrawingScaleX, m_arrRectCornerPoints[0].Y * fDrawingScaleY, m_arrRectCornerPoints[3].X * fDrawingScaleX, m_arrRectCornerPoints[3].Y * fDrawingScaleY);
            }
            else
            {
                if (m_arrLineResultOK[3])
                    m_arrLineGauge[3].DrawResultLineGauge(g, Color.LightGreen);
                else
                    m_arrLineGauge[3].DrawResultLineGauge(g, Color.Red);
            }

            if (m_pRectCenterPoint.X != 0 && m_pRectCenterPoint.Y != 0 && m_pRectCenterPoint.X > 0 && m_pRectCenterPoint.Y > 0)
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

        public void DrawGaugeResult_GaugeNotFound(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
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
        }

        public void DrawGaugeResult_ResultLine_Rotated(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        {
            //m_arrLineGauge[0].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[1].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[2].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);
            //m_arrLineGauge[3].ref_ObjectLine.DrawLine(g, 640, 480, Color.Red, 1);

            float fStartX = (m_pRectCenterPoint.X - m_fRectWidth / 2) * fDrawingScaleX;
            float fStartY = (m_pRectCenterPoint.Y - m_fRectHeight / 2) * fDrawingScaleY;
            float fEndX = (m_pRectCenterPoint.X + m_fRectWidth / 2) * fDrawingScaleX;
            float fEndY = (m_pRectCenterPoint.Y + m_fRectHeight / 2) * fDrawingScaleY;
            //g.DrawRectangle(new Pen(Color.Cyan), objSearchROI.ref_ROIOriPositionX+objROI.ref_ROIPositionX, objSearchROI.ref_ROIOriPositionY + objROI.ref_ROIPositionY, objROI.ref_ROIWidth, objROI.ref_ROIHeight);

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
            if (m_arrEdgeROI[0].ref_ROIPositionX == 0 || m_arrEdgeROI[0].ref_ROIPositionY == 0)
            {
                m_intEdgeROICenterX[0] = intCenterX;
                m_intEdgeROICenterY[0] = intLimitStartY;
                m_arrEdgeROI[0].ref_ROIPositionX = m_intEdgeROICenterX[0] - m_arrEdgeROI[0].ref_ROIWidth / 2;
                m_arrEdgeROI[0].ref_ROIPositionY = m_intEdgeROICenterY[0] - m_arrEdgeROI[0].ref_ROIHeight / 2;
            }

            // ------------- Right -------------------------------------
            if (m_arrEdgeROI[1].ref_ROIPositionX == 0 || m_arrEdgeROI[1].ref_ROIPositionY == 0)
            {
                m_intEdgeROICenterX[1] = intLimitEndX;
                m_intEdgeROICenterY[1] = intCenterY;
                m_arrEdgeROI[1].ref_ROIPositionX = m_intEdgeROICenterX[1] - m_arrEdgeROI[1].ref_ROIWidth / 2;
                m_arrEdgeROI[1].ref_ROIPositionY = m_intEdgeROICenterY[1] - m_arrEdgeROI[1].ref_ROIHeight / 2;
            }

            // ------------- Bottom -------------------------------------
            if (m_arrEdgeROI[2].ref_ROIPositionX == 0 || m_arrEdgeROI[2].ref_ROIPositionY == 0)
            {
                m_intEdgeROICenterX[2] = intCenterX;
                m_intEdgeROICenterY[2] = intLimitEndY;
                m_arrEdgeROI[2].ref_ROIPositionX = m_intEdgeROICenterX[2] - m_arrEdgeROI[2].ref_ROIWidth / 2;
                m_arrEdgeROI[2].ref_ROIPositionY = m_intEdgeROICenterY[2] - m_arrEdgeROI[2].ref_ROIHeight / 2;
            }

            // ------------- Left -------------------------------------
            if (m_arrEdgeROI[3].ref_ROIPositionX == 0 || m_arrEdgeROI[3].ref_ROIPositionY == 0)
            {
                m_intEdgeROICenterX[3] = intLimitStartX;
                m_intEdgeROICenterY[3] = intCenterY;
                m_arrEdgeROI[3].ref_ROIPositionX = m_intEdgeROICenterX[3] - m_arrEdgeROI[3].ref_ROIWidth / 2;
                m_arrEdgeROI[3].ref_ROIPositionY = m_intEdgeROICenterY[3] - m_arrEdgeROI[3].ref_ROIHeight / 2;
            }
        }

        public void SetEdgeROIPlacementLimit2(ROI objROI, int UnitCenterX, int UnitCenterY, int UnitWidth, int UnitHeight)
        {
            int intLimitStartX = objROI.ref_ROITotalX;
            int intLimitStartY = objROI.ref_ROITotalY;
            int intLimitEndX = objROI.ref_ROITotalX + objROI.ref_ROIWidth;
            int intLimitEndY = objROI.ref_ROITotalY + objROI.ref_ROIHeight;
            int intUnitCenterX = UnitCenterX;
            int intUnitCenterY = UnitCenterY;
            //int intOriCenterX = m_arrEdgeROI[0].ref_ROITotalCenterX;
            //int intOriCenterY = m_arrEdgeROI[0].ref_ROITotalCenterY;
            // ------------- Top -------------------------------------

                m_intEdgeROICenterX[0] = UnitCenterX;
                m_intEdgeROICenterY[0] = UnitCenterY - UnitHeight / 2;
                m_arrEdgeROI[0].ref_ROIPositionX = m_intEdgeROICenterX[0] - m_arrEdgeROI[0].ref_ROIWidth / 2;
                m_arrEdgeROI[0].ref_ROIPositionY = m_intEdgeROICenterY[0] - m_arrEdgeROI[0].ref_ROIHeight / 2;
            

            // ------------- Right -------------------------------------
            
                m_intEdgeROICenterX[1] = UnitCenterX + UnitWidth / 2;
                m_intEdgeROICenterY[1] = UnitCenterY;
                m_arrEdgeROI[1].ref_ROIPositionX = m_intEdgeROICenterX[1] - m_arrEdgeROI[1].ref_ROIWidth / 2;
                m_arrEdgeROI[1].ref_ROIPositionY = m_intEdgeROICenterY[1] - m_arrEdgeROI[1].ref_ROIHeight / 2;
            

            // ------------- Bottom -------------------------------------
            
                m_intEdgeROICenterX[2] = UnitCenterX;
                m_intEdgeROICenterY[2] = UnitCenterY + UnitHeight / 2;
                m_arrEdgeROI[2].ref_ROIPositionX = m_intEdgeROICenterX[2] - m_arrEdgeROI[2].ref_ROIWidth / 2;
                m_arrEdgeROI[2].ref_ROIPositionY = m_intEdgeROICenterY[2] - m_arrEdgeROI[2].ref_ROIHeight / 2;
            

            // ------------- Left -------------------------------------
           
                m_intEdgeROICenterX[3] = UnitCenterX - UnitWidth / 2;
                m_intEdgeROICenterY[3] = UnitCenterY;
                m_arrEdgeROI[3].ref_ROIPositionX = m_intEdgeROICenterX[3] - m_arrEdgeROI[3].ref_ROIWidth / 2;
                m_arrEdgeROI[3].ref_ROIPositionY = m_intEdgeROICenterY[3] - m_arrEdgeROI[3].ref_ROIHeight / 2;
            
        }

        public void SetEdgeROIPlacementLimit3(ROI objROI, int UnitCenterX, int UnitCenterY, int UnitWidth, int UnitHeight)
        {
            int intLimitStartX = objROI.ref_ROITotalX;
            int intLimitStartY = objROI.ref_ROITotalY;
            int intLimitEndX = objROI.ref_ROITotalX + objROI.ref_ROIWidth;
            int intLimitEndY = objROI.ref_ROITotalY + objROI.ref_ROIHeight;
            int intUnitCenterX = UnitCenterX;
            int intUnitCenterY = UnitCenterY;
            //int intOriCenterX = m_arrEdgeROI[0].ref_ROITotalCenterX;
            //int intOriCenterY = m_arrEdgeROI[0].ref_ROITotalCenterY;
            // ------------- Top -------------------------------------

            m_intEdgeROICenterX[0] = UnitCenterX + m_intEdgeROIOffsetX[0];
            //if (m_intEdgeROIOffsetY[0] > 0)
                m_intEdgeROICenterY[0] = UnitCenterY + m_intEdgeROIOffsetY[0];
            //else
            //    m_intEdgeROICenterY[0] = UnitCenterY - UnitHeight / 2;
            m_arrEdgeROI[0].ref_ROIPositionX = m_intEdgeROICenterX[0] - m_arrEdgeROI[0].ref_ROIWidth / 2;
            m_arrEdgeROI[0].ref_ROIPositionY = m_intEdgeROICenterY[0] - m_arrEdgeROI[0].ref_ROIHeight / 2;


            // ------------- Right -------------------------------------

            //if (m_intEdgeROIOffsetX[1] > 0)
                m_intEdgeROICenterX[1] = UnitCenterX + m_intEdgeROIOffsetX[1];
            //else
            //    m_intEdgeROICenterX[1] = UnitCenterX + UnitWidth / 2;
            m_intEdgeROICenterY[1] = UnitCenterY + m_intEdgeROIOffsetY[1];
            m_arrEdgeROI[1].ref_ROIPositionX = m_intEdgeROICenterX[1] - m_arrEdgeROI[1].ref_ROIWidth / 2;
            m_arrEdgeROI[1].ref_ROIPositionY = m_intEdgeROICenterY[1] - m_arrEdgeROI[1].ref_ROIHeight / 2;


            // ------------- Bottom -------------------------------------

            m_intEdgeROICenterX[2] = UnitCenterX + m_intEdgeROIOffsetX[2];
            //if (m_intEdgeROIOffsetY[2] > 0)
                m_intEdgeROICenterY[2] = UnitCenterY + m_intEdgeROIOffsetY[2];
            //else
            //    m_intEdgeROICenterY[2] = UnitCenterY + UnitHeight / 2;
            m_arrEdgeROI[2].ref_ROIPositionX = m_intEdgeROICenterX[2] - m_arrEdgeROI[2].ref_ROIWidth / 2;
            m_arrEdgeROI[2].ref_ROIPositionY = m_intEdgeROICenterY[2] - m_arrEdgeROI[2].ref_ROIHeight / 2;


            // ------------- Left -------------------------------------

            //if (m_intEdgeROIOffsetX[3] > 0)
                m_intEdgeROICenterX[3] = UnitCenterX + m_intEdgeROIOffsetX[3];
            //else
            //    m_intEdgeROICenterX[3] = UnitCenterX - UnitWidth / 2;
            m_intEdgeROICenterY[3] = UnitCenterY + m_intEdgeROIOffsetY[3];
            m_arrEdgeROI[3].ref_ROIPositionX = m_intEdgeROICenterX[3] - m_arrEdgeROI[3].ref_ROIWidth / 2;
            m_arrEdgeROI[3].ref_ROIPositionY = m_intEdgeROICenterY[3] - m_arrEdgeROI[3].ref_ROIHeight / 2;

        }
        public void SetEdgeROIPlacementLimit_ForAutoTune(ROI objROI, int UnitCenterX, int UnitCenterY, int intLineIndex , bool blnUnitPR)
        {
            if (intLineIndex == 0)
            {
                // ------------- Top -------------------------------------

                m_intEdgeROICenterX[0] = UnitCenterX;
                m_intEdgeROICenterY[0] = UnitCenterY;
                m_arrEdgeROI[0].ref_ROIPositionX = m_intEdgeROICenterX[0] - m_arrEdgeROI[0].ref_ROIWidth / 2;
                m_arrEdgeROI[0].ref_ROIPositionY = m_intEdgeROICenterY[0] - m_arrEdgeROI[0].ref_ROIHeight / 2;
            }
            else if (intLineIndex == 1)
            {
                // ------------- Right -------------------------------------

                m_intEdgeROICenterX[1] = UnitCenterX;
                m_intEdgeROICenterY[1] = UnitCenterY;
                m_arrEdgeROI[1].ref_ROIPositionX = m_intEdgeROICenterX[1] - m_arrEdgeROI[1].ref_ROIWidth / 2;
                m_arrEdgeROI[1].ref_ROIPositionY = m_intEdgeROICenterY[1] - m_arrEdgeROI[1].ref_ROIHeight / 2;
            }
            else if (intLineIndex == 2)
            {
                // ------------- Bottom -------------------------------------

                m_intEdgeROICenterX[2] = UnitCenterX;
                m_intEdgeROICenterY[2] = UnitCenterY;
                m_arrEdgeROI[2].ref_ROIPositionX = m_intEdgeROICenterX[2] - m_arrEdgeROI[2].ref_ROIWidth / 2;
                m_arrEdgeROI[2].ref_ROIPositionY = m_intEdgeROICenterY[2] - m_arrEdgeROI[2].ref_ROIHeight / 2;
            }
            else if (intLineIndex == 3)
            {
                // ------------- Left -------------------------------------

                m_intEdgeROICenterX[3] = UnitCenterX;
                m_intEdgeROICenterY[3] = UnitCenterY;
                m_arrEdgeROI[3].ref_ROIPositionX = m_intEdgeROICenterX[3] - m_arrEdgeROI[3].ref_ROIWidth / 2;
                m_arrEdgeROI[3].ref_ROIPositionY = m_intEdgeROICenterY[3] - m_arrEdgeROI[3].ref_ROIHeight / 2;
            }
        }
        public void SetEdgeROIOffset(int UnitCenterX, int UnitCenterY)
        {
            if (m_arrEdgeROI[0].ref_ROI.TopParent == null)
                return;

            m_intEdgeROIOffsetX[0] = m_arrEdgeROI[0].ref_ROITotalCenterX - UnitCenterX;
            m_intEdgeROIOffsetY[0] = m_arrEdgeROI[0].ref_ROITotalCenterY - UnitCenterY;

            m_intEdgeROIOffsetX[1] = m_arrEdgeROI[1].ref_ROITotalCenterX - UnitCenterX;
            m_intEdgeROIOffsetY[1] = m_arrEdgeROI[1].ref_ROITotalCenterY - UnitCenterY;

            m_intEdgeROIOffsetX[2] = m_arrEdgeROI[2].ref_ROITotalCenterX - UnitCenterX;
            m_intEdgeROIOffsetY[2] = m_arrEdgeROI[2].ref_ROITotalCenterY - UnitCenterY;

            m_intEdgeROIOffsetX[3] = m_arrEdgeROI[3].ref_ROITotalCenterX - UnitCenterX;
            m_intEdgeROIOffsetY[3] = m_arrEdgeROI[3].ref_ROITotalCenterY - UnitCenterY;
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
                        STTrackLog.WriteLine(">>>>>>>>>>>>> time out 211");
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
                        STTrackLog.WriteLine(">>>>>>>>>>>>> time out 212");
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
        public int[] GetCurrentDontCareCount()
        {
            int[] arrDontCareCount = new int[m_arrDontCareROI.Count];
            for (int i = 0; i < m_arrDontCareROI.Count; i++)
            {
                arrDontCareCount[i] = m_arrDontCareROI[i].Count;
            }
            return arrDontCareCount;
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
                    objEdgeROI.AttachImage(arrImage[m_arrGaugeImageNo[i]]);

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
        public void ModifyDontCareImage(List<List<ImageDrawing>> arrImage, ImageDrawing objWhiteImage)
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
                    objEdgeROI.AttachImage(arrImage[m_arrGaugeImageNo[i]][i]);

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

        //public void DrawEdgeROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY)
        //{
        //    for (int i = 0; i < m_arrEdgeROI.Count; i++)
        //    {
        //        m_arrEdgeROI[i].DrawROI(g, fDrawingScaleX, fDrawingScaleY, m_arrEdgeROI[i].GetROIHandle());
        //    }
        //}

        public void DrawEdgeROI(Graphics g, float fDrawingScaleX, float fDrawingScaleY, Color objColor)
        {
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                if ((m_intSelectedGaugeEdgeMask & (0x01 << i)) > 0)
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

        public void CopyTo(ref RectGaugeM4L objDestRectGauge)
        {
            objDestRectGauge.m_fTemplateObjectCenterX = m_fTemplateObjectCenterX;
            objDestRectGauge.m_fTemplateObjectCenterY = m_fTemplateObjectCenterY;
            objDestRectGauge.m_fTemplateObjectWidth = m_fTemplateObjectWidth;
            objDestRectGauge.m_fTemplateObjectHeight = m_fTemplateObjectHeight;

            //2019-10-14 ZJYEOH : Need to copy unit width and height as it will be use in function --> GetPosition_UsingEdgeAndCornerLineGauge(ROI objROI)
            // if no copy, when duplicate opposite line and shift according to width or height (ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight)), will get back same line as width or height is 0, this will cause gauge fail during production but pass during learn.
            objDestRectGauge.m_fUnitSizeWidth = m_fUnitSizeWidth;
            objDestRectGauge.m_fUnitSizeHeight = m_fUnitSizeHeight;

            objDestRectGauge.ref_fGainValue = m_fGainValue;

            for (int i = 0; i < m_arrLineGauge.Count; i++)
            {
                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeAngle = m_arrLineGauge[i].ref_GaugeAngle;
                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeLength = m_arrLineGauge[i].ref_GaugeLength;
                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeTolerance = m_arrLineGauge[i].ref_GaugeTolerance;
                objDestRectGauge.ref_arrLineGauge[i].SetGaugeCenter(m_arrLineGauge[i].ref_GaugeCenterX, m_arrLineGauge[i].ref_GaugeCenterY);

                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeTransType = m_arrLineGauge[i].ref_GaugeTransType;
                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeTransChoice = m_arrLineGauge[i].ref_GaugeTransChoice;
                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeThreshold = m_arrLineGauge[i].ref_GaugeThreshold;
                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeThickness = m_arrLineGauge[i].ref_GaugeThickness;
                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeMinAmplitude = m_arrLineGauge[i].ref_GaugeMinAmplitude;
                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeMinArea = m_arrLineGauge[i].ref_GaugeMinArea;
                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeFilter = m_arrLineGauge[i].ref_GaugeFilter;

                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeFilterPasses = m_arrLineGauge[i].ref_GaugeFilterPasses;
                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeFilterThreshold = m_arrLineGauge[i].ref_GaugeFilterThreshold;
                objDestRectGauge.ref_arrLineGauge[i].ref_GaugeSamplingStep = m_arrLineGauge[i].ref_GaugeSamplingStep;

            }

            for (int i = 0; i < objDestRectGauge.m_arrGaugeMaxAngle.Count; i++)
            {
                objDestRectGauge.m_arrGaugeEnableSubGauge[i] = m_arrGaugeEnableSubGauge[i];
                objDestRectGauge.m_arrGaugeMeasureMode[i] = m_arrGaugeMeasureMode[i];
                objDestRectGauge.m_arrReCheckUsingPointGaugeIfFail[i] = m_arrReCheckUsingPointGaugeIfFail[i];
                objDestRectGauge.m_arrGaugeMaxAngle[i] = m_arrGaugeMaxAngle[i];
                objDestRectGauge.m_arrPointGaugeOffset[i] = m_arrPointGaugeOffset[i];
                objDestRectGauge.m_arrGaugeMinScore[i] = m_arrGaugeMinScore[i];
                objDestRectGauge.m_arrGaugeImageGain[i] = m_arrGaugeImageGain[i];
                objDestRectGauge.m_arrGaugeGroupTolerance[i] = m_arrGaugeGroupTolerance[i];
                objDestRectGauge.m_arrGaugeImageMode[i] = m_arrGaugeImageMode[i];
                objDestRectGauge.m_arrGaugeImageNo[i] = m_arrGaugeImageNo[i];
                objDestRectGauge.m_arrGaugeImageThreshold[i] = m_arrGaugeImageThreshold[i];
                objDestRectGauge.m_arrGaugeWantImageThreshold[i] = m_arrGaugeWantImageThreshold[i];
                objDestRectGauge.m_blnInToOut[i] = m_blnInToOut[i];
            }

            //Copy EdgeROI and DontCareROI info
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                objDestRectGauge.m_arrEdgeROI[i].LoadROISetting(m_arrEdgeROI[i].ref_ROIPositionX, m_arrEdgeROI[i].ref_ROIPositionY, m_arrEdgeROI[i].ref_ROIWidth, m_arrEdgeROI[i].ref_ROIHeight);

                for (int j = 0; j < m_arrDontCareROI[i].Count; j++)
                {
                    if (objDestRectGauge.m_arrDontCareROI[i].Count <= j)
                    {
                        objDestRectGauge.m_arrDontCareROI[i].Add(new ROI());
                        objDestRectGauge.m_arrDontCareROI[i][j].LoadROISetting(m_arrDontCareROI[i][j].ref_ROIPositionX, m_arrDontCareROI[i][j].ref_ROIPositionY, m_arrDontCareROI[i][j].ref_ROIWidth, m_arrDontCareROI[i][j].ref_ROIHeight);
                        objDestRectGauge.m_arrDontCareROI[i][j].AttachImage(objDestRectGauge.m_arrEdgeROI[i]);
                    }
                }
            }

            objDestRectGauge.SetEdgeROIOffsetX(0, m_intEdgeROIOffsetX[0]);
            objDestRectGauge.SetEdgeROIOffsetY(0, m_intEdgeROIOffsetY[0]);
            objDestRectGauge.SetEdgeROIOffsetX(1, m_intEdgeROIOffsetX[1]);
            objDestRectGauge.SetEdgeROIOffsetY(1, m_intEdgeROIOffsetY[1]);
            objDestRectGauge.SetEdgeROIOffsetX(2, m_intEdgeROIOffsetX[2]);
            objDestRectGauge.SetEdgeROIOffsetY(2, m_intEdgeROIOffsetY[2]);
            objDestRectGauge.SetEdgeROIOffsetX(3, m_intEdgeROIOffsetX[3]);
            objDestRectGauge.SetEdgeROIOffsetY(3, m_intEdgeROIOffsetY[3]);
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

        public int GetGaugeMeasureMode(int intLineIndex)
        {
            return m_arrGaugeMeasureMode[intLineIndex];
        }
        public bool GetReCheckUsingPointGaugeIfFail(int intLineIndex)
        {
            return m_arrReCheckUsingPointGaugeIfFail[intLineIndex];
        }
        public int GetGaugeMinScore(int intLineIndex)
        {
            return m_arrGaugeMinScore[intLineIndex];
        }

        public int GetGaugeMaxAngle(int intLineIndex)
        {
            return m_arrGaugeMaxAngle[intLineIndex];
        }

        public int GetPointGaugeOffset(int intLineIndex)
        {
            return m_arrPointGaugeOffset[intLineIndex];
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

        public int GetGaugeImageThreshold(int intLineIndex)
        {
            return m_arrGaugeImageThreshold[intLineIndex];
        }

        public bool GetGaugeWantImageThreshold(int intLineIndex)
        {
            return m_arrGaugeWantImageThreshold[intLineIndex];
        }

        public bool GetGaugeDirection(int intLineIndex)
        {
            bool blnInToOut = false;

            switch (intLineIndex)
            {
                case 0:
                    // Top
                    if (m_arrLineGauge[0].ref_GaugeAngle == 180)
                        blnInToOut = true;
                    break;
                case 1:
                    // Right
                    if (m_arrLineGauge[1].ref_GaugeAngle == -90)
                        blnInToOut = true;
                    break;
                case 2:
                    // Bottom
                    if (m_arrLineGauge[2].ref_GaugeAngle == 0)
                        blnInToOut = true;
                    break;
                case 3:
                    // Left
                    if (m_arrLineGauge[3].ref_GaugeAngle == 90)
                        blnInToOut = true;
                    break;
            }

            return blnInToOut;
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

        public void SetPointGaugeOffset(int intOffset)
        {
            for (int i = 0; i < m_arrPointGaugeOffset.Count; i++)
                m_arrPointGaugeOffset[i] = intOffset;
        }

        public void SetGaugeMinAmplitude(int intMinAmplitude, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;
         
            m_arrLineGauge[intLineIndex].ref_GaugeMinAmplitude = intMinAmplitude;
        }

        public void SetGaugeMinArea(int intMinArea)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
            m_arrLineGauge[i].ref_GaugeMinArea = intMinArea;
        }

        public void SetGaugeMinArea(int intMinArea, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeMinArea = intMinArea;
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
            m_arrLineGauge[i].ref_GaugeTransType = intTransitionTypeIndex;
        }

        public void SetGaugeTransType(int intTransitionTypeIndex, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeTransType = intTransitionTypeIndex;
        }

        public void SetGaugeTransChoice(int intTransitionChoiceIndex)
        {
            for (int i = 0; i < m_arrLineGauge.Count; i++)
            m_arrLineGauge[i].ref_GaugeTransChoice = intTransitionChoiceIndex;
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
            }
            else
            {
                // Set line gauge for different direction
                m_arrLineGauge[0].SetGaugeAngle(0);    // Top
                m_arrLineGauge[1].SetGaugeAngle(90);   // Right
                m_arrLineGauge[2].SetGaugeAngle(180);  // Bottom
                m_arrLineGauge[3].SetGaugeAngle(-90);  // Right
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

        public bool IsDirectionInToOut()
        {
            // when fist array gauge has angle 0 deg, mean the whole rectangle line gauge has Out to In direction.
            if (m_arrLineGauge[0].ref_GaugeAngle == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SetGaugeTransChoice(int intTransitionChoiceIndex, int intLineIndex)
        {
            if (intLineIndex >= m_arrLineGauge.Count)
                return;

            m_arrLineGauge[intLineIndex].ref_GaugeTransChoice = intTransitionChoiceIndex;
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

        public void SetGaugeMeasureMode(int intGaugeMeasureMode, int intLineIndex)
        {
            if (intLineIndex >= m_arrGaugeMeasureMode.Count)
                return;

            m_arrGaugeMeasureMode[intLineIndex] = intGaugeMeasureMode;
        }
        public void SetReCheckUsingPointGaugeIfFail(bool blnReCheckUsingPointGaugeIfFail, int intLineIndex)
        {
            if (intLineIndex >= m_arrReCheckUsingPointGaugeIfFail.Count)
                return;

            m_arrReCheckUsingPointGaugeIfFail[intLineIndex] = blnReCheckUsingPointGaugeIfFail;
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

        public void SetPointGaugeOffset(int intOffset, int intLineIndex)
        {
            if (intLineIndex >= m_arrPointGaugeOffset.Count)
                return;

            m_arrPointGaugeOffset[intLineIndex] = intOffset;
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

        public void SetGaugeImageNo(int intImageNo, int intLineIndex)
        {
            if (intLineIndex >= m_arrGaugeImageNo.Count)
                return;

            m_arrGaugeImageNo[intLineIndex] = intImageNo;
        }

        public void SetGaugeImageGain(float fImageGain)
        {
            for (int i = 0; i < m_arrGaugeImageGain.Count; i++)
                m_arrGaugeImageGain[i] = fImageGain;
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

        public void SetGaugeImageThreshold(int intImageThreshold)
        {
            for (int i = 0; i < m_arrGaugeImageThreshold.Count; i++)
                m_arrGaugeImageThreshold[i] = intImageThreshold;
        }

        public void SetGaugeImageThreshold(bool blnWantImageThreshold)
        {
            for (int i = 0; i < m_arrGaugeWantImageThreshold.Count; i++)
                m_arrGaugeWantImageThreshold[i] = blnWantImageThreshold;
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

        public void SetRectGaugeTemplate(float fTemplateCenterX, float fTemplateCenterY)
        {
            m_fTemplateObjectCenterX = fTemplateCenterX;
            m_fTemplateObjectCenterY = fTemplateCenterY;
        }
        public void SetRectGaugeTemplate(float fTemplateCenterX, float fTemplateCenterY, float fWidth, float fHeight)
        {
            m_fTemplateObjectCenterX = fTemplateCenterX;
            m_fTemplateObjectCenterY = fTemplateCenterY;
            m_fTemplateObjectWidth = fWidth;
            m_fTemplateObjectHeight = fHeight;
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

        public void AddGainForEdgeROI(ref ImageDrawing objSourceImage, ref ImageDrawing objDestinationImage)
        {
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                m_arrEdgeROI[i].AttachImage(objSourceImage);
                m_arrEdgeROI[i].GainTo_ROIToROISamePosition(ref objDestinationImage, m_arrGaugeImageGain[i]);
            }
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
        public void AddHighPassForEdgeROI(ref ImageDrawing objSourceImage, ref ImageDrawing objDestinationImage, int intSelectedEdgeROI)
        {
            if (objDestinationImage.ref_intImageWidth != objSourceImage.ref_intImageWidth || objDestinationImage.ref_intImageHeight != objSourceImage.ref_intImageHeight)
            {
                objDestinationImage.SetImageSize(objSourceImage.ref_intImageWidth, objSourceImage.ref_intImageHeight);
            }

            m_arrEdgeROI[intSelectedEdgeROI].AttachImage(objSourceImage);
            m_arrEdgeROI[intSelectedEdgeROI].HighPassTo_ROIToROISamePosition(ref objDestinationImage);
        }
        public int GetEdgeROIOffsetX(int intIndex)
        {
            return m_intEdgeROIOffsetX[intIndex];
        }

        public int GetEdgeROIOffsetY(int intIndex)
        {
            return m_intEdgeROIOffsetY[intIndex];
        }

        public void SetEdgeROIOffsetX(int intIndex, int intValue)
        {
            m_intEdgeROIOffsetX[intIndex] = intValue;
        }

        public void SetEdgeROIOffsetY(int intIndex, int intValue)
        {
            m_intEdgeROIOffsetY[intIndex] = intValue;
        }

        private bool GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(List<ROI> arrROI)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                m_arrLineGauge[i].Measure(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)]);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            // Double verify Line Gauge result ok or not
            for (int i = 0; i < 4; i++)
            {
                if (blnResult[i])
                {
                    blnResult[i] = m_arrLineGauge[i].VerifyMeasurement_SubGaugePoints(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)]);
                    if (!blnResult[i])
                        nCount--;
                }
            }

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            bool blnEnableVirtualLine = false;
            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    m_arrLineResultOK[0] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                else
                    m_arrLineResultOK[0] = true;
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    m_arrLineResultOK[1] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth); //+, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[1] = true;
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    m_arrLineResultOK[2] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go up
                }
                else
                    m_arrLineResultOK[2] = true;
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    m_arrLineResultOK[3] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);//- , 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[3] = true;
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
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 207");
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
                        if (!arrAngleOK[0] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Left angle
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

                        if (!arrAngleOK[1] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[2] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[3] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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
                    else
                        break;
                }

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

                m_fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;


                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;

                // Verify Unit Size
                //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                //else
                //    isResultOK = false;

                if (m_pRectCenterPoint.X.ToString() != "NaN" && m_pRectCenterPoint.Y.ToString() != "NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "-NaN" && m_pRectCenterPoint.Y.ToString() != "-NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "Infinity" && m_pRectCenterPoint.Y.ToString() != "Infinity" &&
                                     m_pRectCenterPoint.X.ToString() != "-Infinity" && m_pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                     m_fRectWidth != 0 && m_fRectHeight != 0)
                    isResultOK = true;
                else
                {
                    m_strErrorMessage = "Measure Edge Fail!";
                    isResultOK = false;
                }

            }

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

            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(List<List<ROI>> arrROI)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                //m_arrLineGauge[i].Measure(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]);
                int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);  // 2020 08 14 - CCENG - prevent out of index when use wrong recipe
                if (intROIIndex >= arrROI.Count)
                    intROIIndex = 0;

                m_arrLineGauge[i].Measure(arrROI[intROIIndex][i]);
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            // Double verify Line Gauge result ok or not
            for (int i = 0; i < 4; i++)
            {
                if (blnResult[i])
                {
                    //blnResult[i] = m_arrLineGauge[i].VerifyMeasurement_SubGaugePoints(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]);
                    int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);  // 2020 08 14 - CCENG - prevent out of index when use wrong recipe
                    if (intROIIndex >= arrROI.Count)
                        intROIIndex = 0;

                    blnResult[i] = m_arrLineGauge[i].VerifyMeasurement_SubGaugePoints(arrROI[intROIIndex][i]);

                    if (!blnResult[i])
                        nCount--;
                }
            }

            m_arrLineResultOK = blnResult;

            if (nCount < 2)   // only 1 line gauge has valid result. No enough data to generate result
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            bool blnEnableVirtualLine = false;
            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    m_arrLineResultOK[0] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                else
                    m_arrLineResultOK[0] = true;
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    m_arrLineResultOK[1] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth); //+, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[1] = true;
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    m_arrLineResultOK[2] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go up
                }
                else
                    m_arrLineResultOK[2] = true;
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    m_arrLineResultOK[3] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);//- , 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[3] = true;
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
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 208");
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
                        if (!arrAngleOK[0] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Left angle
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

                        if (!arrAngleOK[1] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[2] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[3] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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
                    else
                        break;
                }

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

                m_fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;


                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;

                // Verify Unit Size
                //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                //else
                //    isResultOK = false;

                if (m_pRectCenterPoint.X.ToString() != "NaN" && m_pRectCenterPoint.Y.ToString() != "NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "-NaN" && m_pRectCenterPoint.Y.ToString() != "-NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "Infinity" && m_pRectCenterPoint.Y.ToString() != "Infinity" &&
                                     m_pRectCenterPoint.X.ToString() != "-Infinity" && m_pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                     m_fRectWidth != 0 && m_fRectHeight != 0)
                    isResultOK = true;
                else
                {
                    m_strErrorMessage = "Measure Edge Fail!";
                    isResultOK = false;
                }

            }

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

            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(List<ROI> arrROI)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];

            int intFailLIneResultCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!m_arrLineResultOK[i])
                    intFailLIneResultCount++;
            }

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                if (intFailLIneResultCount > 0)
                {
                    m_arrLineGauge[i].Measure_SubGaugePoints(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)], m_arrGaugeMinScore[i]);
                }
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
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            bool blnEnableVirtualLine = false;
            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    m_arrLineResultOK[0] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                else
                    m_arrLineResultOK[0] = true;
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    m_arrLineResultOK[1] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth); //+, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[1] = true;
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    m_arrLineResultOK[2] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go up
                }
                else
                    m_arrLineResultOK[2] = true;
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    m_arrLineResultOK[3] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);//- , 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[3] = true;
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
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 209");
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
                        if (!arrAngleOK[0] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Left angle
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

                        if (!arrAngleOK[1] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[2] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[3] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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
                    else
                        break;
                }

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

                m_fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;


                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;

                // Verify Unit Size
                //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                //else
                //    isResultOK = false;

                if (m_pRectCenterPoint.X.ToString() != "NaN" && m_pRectCenterPoint.Y.ToString() != "NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "-NaN" && m_pRectCenterPoint.Y.ToString() != "-NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "Infinity" && m_pRectCenterPoint.Y.ToString() != "Infinity" &&
                                     m_pRectCenterPoint.X.ToString() != "-Infinity" && m_pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                     m_fRectWidth != 0 && m_fRectHeight != 0)
                    isResultOK = true;
                else
                {
                    m_strErrorMessage = "Measure Edge Fail!";
                    isResultOK = false;
                }

            }

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
            return true;
        }
        private bool GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(List<List<ROI>> arrROI)
        {
            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];

            int intFailLIneResultCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (!m_arrLineResultOK[i])
                    intFailLIneResultCount++;
            }

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                if (intFailLIneResultCount > 0)
                {
                    int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);  // 2020 08 14 - CCENG - prevent out of index when use wrong recipe
                    if (intROIIndex >= arrROI.Count)
                        intROIIndex = 0;

                    m_arrLineGauge[i].Measure_SubGaugePoints(arrROI[intROIIndex][i], m_arrGaugeMinScore[i]);
                }
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
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[0] && !blnResult[2]) // totally no valid horizontal line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }
            if (!blnResult[1] && !blnResult[3]) // totally no valid vertical line can be referred
            {
                m_strErrorMessage = "Measure edge fail.";
                m_intFailResultMask |= 0x02;
                return false;
            }

            // Get average angle from Edge Line Gauges Angle
            m_fRectAngle = fTotalAngle / nCount;

            bool blnEnableVirtualLine = false;
            if (blnEnableVirtualLine)
            {
                // Build a virtual edge line if the edge is fail
                if (!blnResult[0]) // top border fail. Duplicate using bottom border and sample's height
                {
                    m_arrLineResultOK[0] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[2].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[0].ref_ObjectLine = objLine;
                    m_arrLineGauge[0].ref_ObjectLine.ShiftYLine(-m_fUnitSizeHeight);    // use "-" because Y decrease when go up
                }
                else
                    m_arrLineResultOK[0] = true;
                if (!blnResult[1])  // Right border fail. Duplicate using left border and sample's width
                {
                    m_arrLineResultOK[1] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[3].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[1].ref_ObjectLine = objLine;
                    m_arrLineGauge[1].ref_ObjectLine.ShiftXLine(-m_fUnitSizeWidth); //+, 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[1] = true;
                if (!blnResult[2])  // Bottom border fail. Duplicate using top border and sample's height
                {
                    m_arrLineResultOK[2] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[0].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[2].ref_ObjectLine = objLine;
                    m_arrLineGauge[2].ref_ObjectLine.ShiftYLine(m_fUnitSizeHeight);     // use "+" because Y increase when go up
                }
                else
                    m_arrLineResultOK[2] = true;
                if (!blnResult[3])  // Left border fail. Duplicate using right border and sample's width
                {
                    m_arrLineResultOK[3] = false;
                    Line objLine = new Line();
                    m_arrLineGauge[1].ref_ObjectLine.CopyTo(ref objLine);
                    m_arrLineGauge[3].ref_ObjectLine = objLine;
                    m_arrLineGauge[3].ref_ObjectLine.ShiftXLine(m_fUnitSizeWidth);//- , 09-08-2019 ZJYEOH : Inverted the sign (+/-) , because the coner point for X shifted to negative
                }
                else
                    m_arrLineResultOK[3] = true;
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
                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 210");
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
                        if (!arrAngleOK[0] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Left angle
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

                        if (!arrAngleOK[1] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[2] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        if (!arrAngleOK[3] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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
                    else
                        break;
                }

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

                m_fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;


                // ---------- Hor line ------------

                arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                m_fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;

                // Verify Unit Size
                //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                m_pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                //else
                //    isResultOK = false;

                if (m_pRectCenterPoint.X.ToString() != "NaN" && m_pRectCenterPoint.Y.ToString() != "NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "-NaN" && m_pRectCenterPoint.Y.ToString() != "-NaN" &&
                                     m_pRectCenterPoint.X.ToString() != "Infinity" && m_pRectCenterPoint.Y.ToString() != "Infinity" &&
                                     m_pRectCenterPoint.X.ToString() != "-Infinity" && m_pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                     m_fRectWidth != 0 && m_fRectHeight != 0)
                    isResultOK = true;
                else
                {
                    m_strErrorMessage = "Measure Edge Fail!";
                    isResultOK = false;
                }

            }

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
            return true;
        }

        public bool Measure_WithDontCareArea(List<ImageDrawing> arrImage, ROI objSourceROI, ImageDrawing objWhiteImage, bool bUseTemplateUnitSize, float fTemplateUnitSizeX, float fTemplateUnitSizeY)
        {
            List<List<ImageDrawing>> arrModifiedImage = new List<List<ImageDrawing>>();
            ImageDrawing[] objPreModifiedImage = new ImageDrawing[4] { null, null, null, null };
            
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

            //for (int i = 0; i < arrModifiedImage.Count; i++)
            //{
            //    for (int j = 0; j < arrModifiedImage[i].Count; j++)
            //    {
            //        if (arrModifiedImage[i][j] != null)
            //            arrModifiedImage[i][j].SaveImage("D:\\TS\\arrModifiedImage" + i.ToString() + j.ToString() + ".bmp");
            //    }
            //}

            //for (int i = 0; i < arrImage.Count; i++)
            //{

            //        if (arrImage[i] != null)
            //            arrImage[i].SaveImage("D:\\TS\\arrImage" + i.ToString() + ".bmp");

            //}

            // Add gain to temporary image
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);
                if (intImageIndex >= arrImage.Count)
                    intImageIndex = 0;

                if (m_arrGaugeImageMode[i] == 0)
                {
                    if (m_arrGaugeWantImageThreshold[i])
                    {
                        if (objPreModifiedImage[i] == null)
                        {
                            objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                        }
                        
                        m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageThreshold[i]);

                        m_arrEdgeROI[i].AttachImage(objImage);
                    }
                    else
                    {
                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageGain[i]);
                    }
                }
                else
                {
                    //if (objPreModifiedImage[i] == null)
                    //{
                    //    objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                    //}

                    //m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                    //m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                    //ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                    //m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                    //m_arrEdgeROI[i].PrewittTo_ROIToROISamePosition(ref objImage);

                    if (objPreModifiedImage[i] == null)
                    {
                        objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                    }

                    m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                    m_arrEdgeROI[i].HighPassTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i]);

                    if (m_arrGaugeWantImageThreshold[i])
                    {
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageThreshold[i]);

                        m_arrEdgeROI[i].AttachImage(objImage);
                    }
                    else
                    {
                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageGain[i]);
                    }
                }

                m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
            }

            //for (int i = 0; i < arrModifiedImage.Count; i++)
            //{
            //    for (int j = 0; j < arrModifiedImage[i].Count; j++)
            //    {
            //        if (arrModifiedImage[i][j] != null)
            //            arrModifiedImage[i][j].SaveImage("D:\\TS\\arrModifiedImage" + i.ToString() + j.ToString() + ".bmp");
            //    }
            //}

            //for (int i = 0; i < arrImage.Count; i++)
            //{

            //    if (arrImage[i] != null)
            //        arrImage[i].SaveImage("D:\\TS\\arrImage" + i.ToString() + ".bmp");

            //}

            // if dont care present
            if (m_arrDontCareROI[0].Count != 0 || m_arrDontCareROI[1].Count != 0 || m_arrDontCareROI[2].Count != 0 || m_arrDontCareROI[3].Count != 0)
            {
                ModifyDontCareImage(arrModifiedImage, objWhiteImage);

                List<List<ROI>> arrROI = new List<List<ROI>>();
                for (int i = 0; i < arrModifiedImage.Count; i++)
                {
                    arrROI.Add(new List<ROI>());
                    for (int j = 0; j < arrModifiedImage[i].Count; j++)
                    {
                        if (arrModifiedImage[i][j] != null)
                        {
                            arrROI[i].Add(new ROI());
                            arrROI[i][j].AttachImage(arrModifiedImage[i][j]);
                            arrROI[i][j].LoadROISetting(objSourceROI.ref_ROIPositionX, objSourceROI.ref_ROIPositionY, objSourceROI.ref_ROIWidth, objSourceROI.ref_ROIHeight);
                        }
                    }
                }

                //for (int i = 0; i < arrROI.Count; i++)
                //    for (int j = 0; j < arrROI[i].Count; j++)
                //        if (arrROI[i][j].ref_ROI.TopParent != null)
                //            arrROI[i][j].SaveImage("D:\\TS\\arrROI" + i.ToString() + j.ToString() + ".bmp");

                //for (int i = 0; i < arrROI.Count; i++)
                //    for (int j = 0; j < arrROI[i].Count; j++)
                //        if (arrROI[i][j].ref_ROI.TopParent != null)
                //            arrROI[i][j].ref_ROI.TopParent.Save("D:\\TS\\arrROITopParent" + i.ToString() + j.ToString() + ".bmp");

                //bool blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);

                ////if (!blnResult && m_arrGaugeEnableSubGauge.Contains(true))
                //if (!blnResult && m_arrGaugeMeasureMode.Contains(1))  
                //    blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);

                bool blnResult;
                if (m_arrGaugeMeasureMode.Contains(3))
                {
                    if (bUseTemplateUnitSize)
                    {
                        //blnResult = GetPosition_UsingEdgeAndCornerLineGauge_CollectList(arrROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints_CollectList(arrROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                    }
                    else
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(arrROI);
                    }
                }
                else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(arrROI);
                }
                else
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);
                }

                if (!blnResult)
                {
                    if (m_arrGaugeMeasureMode.Contains(1))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(arrROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(3))
                    {
                        if (!bUseTemplateUnitSize)
                            blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(arrROI);
                    }
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

                for (int i = 0; i < arrROI.Count; i++)
                {
                    for (int j = 0; j < arrROI[i].Count; j++)
                    {
                        if (arrROI[i][j] != null)
                        {
                            arrROI[i][j].Dispose();
                            arrROI[i][j] = null;
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

                return blnResult;
            }
            else
            {
                List<List<ROI>> arrROI = new List<List<ROI>>();
                for (int i = 0; i < arrModifiedImage.Count; i++)
                {
                    arrROI.Add(new List<ROI>());
                    for (int j = 0; j < arrModifiedImage[i].Count; j++)
                    {
                        if (arrModifiedImage[i][j] != null)
                        {
                            arrROI[i].Add(new ROI());
                            arrROI[i][j].AttachImage(arrModifiedImage[i][j]); // 2020-02-03 ZJYEOH : Should attach to arrModifiedImage instead of arrImage
                            arrROI[i][j].LoadROISetting(objSourceROI.ref_ROIPositionX, objSourceROI.ref_ROIPositionY, objSourceROI.ref_ROIWidth, objSourceROI.ref_ROIHeight);
                        }
                        else
                            arrROI[i].Add(new ROI());
                    }
                }

                //for (int i = 0; i < arrROI.Count; i++)
                //    for (int j = 0; j < arrROI[i].Count; j++)
                //        if (arrROI[i][j].ref_ROI.TopParent != null)
                //            arrROI[i][j].SaveImage("D:\\TS\\arrROI" + i.ToString() + j.ToString() + ".bmp");

                //for (int i = 0; i < arrROI.Count; i++)
                //    for (int j = 0; j < arrROI[i].Count; j++)
                //        if (arrROI[i][j].ref_ROI.TopParent != null)
                //            arrROI[i][j].ref_ROI.TopParent.Save("D:\\TS\\arrROITopParent" + i.ToString() + j.ToString() + ".bmp");

                //bool blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);

                ////if (!blnResult && m_arrGaugeEnableSubGauge.Contains(true))
                //if (!blnResult && m_arrGaugeMeasureMode.Contains(1))
                //    blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);


                bool blnResult;
                if (m_arrGaugeMeasureMode.Contains(3))
                {
                    if (bUseTemplateUnitSize)
                    {
                        //blnResult = GetPosition_UsingEdgeAndCornerLineGauge_CollectList(arrROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints_CollectList(arrROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                    }
                    else
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(arrROI);
                    }
                }
                else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(arrROI);
                }
                else
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);
                }

                if (!blnResult)
                {
                    if (m_arrGaugeMeasureMode.Contains(1))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(arrROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(3))
                    {
                        if (!bUseTemplateUnitSize)
                            blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(arrROI);
                    }
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

                for (int i = 0; i < arrROI.Count; i++)
                {
                    for (int j = 0; j < arrROI[i].Count; j++)
                    {
                        if (arrROI[i][j] != null)
                        {
                            arrROI[i][j].Dispose();
                            arrROI[i][j] = null;
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

                return blnResult;
            }
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
            objLineGaugeRecord.fMeasureAngle = objLineGauge.ref_ObjectMeasuredAngle;//ref_ObjectAngle //2021-10-01 ZJYEOH : Should collect measured line angle
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

        public bool Measure_WithDontCareArea(List<ImageDrawing> arrImage, ImageDrawing objWhiteImage, bool bUseTemplateUnitSize, float fTemplateUnitSizeX, float fTemplateUnitSizeY)
        {
            //List<List<ImageDrawing>> arrModifiedImage = new List<List<ImageDrawing>>();
            //ImageDrawing[] objPreModifiedImage = new ImageDrawing[4] { null, null, null, null };
            if (m_objLocalROI == null)
                m_objLocalROI = new ROI();

            // Copy source image to temporary local image .
            for (int i = 0; i < arrImage.Count; i++)
            {
                if (i >= m_arrModifiedImage.Count)
                {
                    m_arrModifiedImage.Add(new List<ImageDrawing>());
                    m_arrModifiedImage[i].Add(null);
                    m_arrModifiedImage[i].Add(null);
                    m_arrModifiedImage[i].Add(null);
                    m_arrModifiedImage[i].Add(null);
                }

                for (int j = 0; j < m_arrModifiedImage[i].Count; j++)
                {
                    if (j < m_arrGaugeImageNo.Count)
                    {
                        if (i == ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[j], m_intVisionIndex))
                        {
                            if (m_arrModifiedImage[i][j] == null)
                                m_arrModifiedImage[i][j] = new ImageDrawing(true, arrImage[i].ref_intImageWidth, arrImage[i].ref_intImageHeight);

                            m_objLocalROI.LoadROISetting(m_arrEdgeROI[j].ref_ROIPositionX, m_arrEdgeROI[j].ref_ROIPositionY,
                            m_arrEdgeROI[j].ref_ROIWidth, m_arrEdgeROI[j].ref_ROIHeight);
                            m_objLocalROI.AttachImage(m_arrModifiedImage[i][j]);
                            m_arrEdgeROI[j].AttachImage(arrImage[i]);

                            m_arrEdgeROI[j].CopyImage_Bigger(ref m_objLocalROI);
                        }
                    }
                }
            }

            // Add gain to temporary image
            for (int i = 0; i < m_arrEdgeROI.Count; i++)
            {
                int intImageIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);

                if (m_arrGaugeImageMode[i] == 0)
                {
                    if (m_arrGaugeWantImageThreshold[i])
                    {
                        if (m_objPreModifiedImage[i] == null)
                        {
                            m_objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                        }

                        m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref m_objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                        ImageDrawing objImage = m_arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(m_objPreModifiedImage[i]);
                        m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageThreshold[i]);

                        m_arrEdgeROI[i].AttachImage(objImage);
                    }
                    else
                    {
                        ImageDrawing objImage = m_arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageGain[i]);
                    }
                }
                else
                {
                    //if (m_objPreModifiedImage[i] == null)
                    //{
                    //    m_objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                    //}

                    //m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                    //m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref m_objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                    //ImageDrawing objImage = m_arrModifiedImage[intImageIndex][i];
                    //m_arrEdgeROI[i].AttachImage(m_objPreModifiedImage[i]);
                    //m_arrEdgeROI[i].PrewittTo_ROIToROISamePosition(ref objImage);

                    if (m_objPreModifiedImage[i] == null)
                    {
                        m_objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                    }

                    m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                    m_arrEdgeROI[i].HighPassTo_ROIToROISamePosition_Bigger(ref m_objPreModifiedImage[i]);

                    if (m_arrGaugeWantImageThreshold[i])
                    {
                        m_arrEdgeROI[i].AttachImage(m_objPreModifiedImage[i]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref m_objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                        ImageDrawing objImage = m_arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(m_objPreModifiedImage[i]);
                        m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageThreshold[i]);

                        m_arrEdgeROI[i].AttachImage(objImage);
                    }
                    else
                    {
                        ImageDrawing objImage = m_arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(m_objPreModifiedImage[i]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageGain[i]);
                    }
                }
                
                m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
            }

            // if dont care present
            if (m_arrDontCareROI[0].Count != 0 || m_arrDontCareROI[1].Count != 0 || m_arrDontCareROI[2].Count != 0 || m_arrDontCareROI[3].Count != 0)
            {
                ModifyDontCareImage(m_arrModifiedImage, objWhiteImage);

                for (int i = 0; i < m_arrModifiedImage.Count; i++)
                {
                    if (i >= m_arrLocalROI.Count)
                        m_arrLocalROI.Add(new List<ROI>());

                    for (int j = 0; j < m_arrModifiedImage[i].Count; j++)
                    {
                        if (j >= m_arrLocalROI[i].Count)
                            m_arrLocalROI[i].Add(new ROI());

                        if (m_arrModifiedImage[i][j] != null)
                        {
                            m_arrLocalROI[i][j].AttachImage(m_arrModifiedImage[i][j]);
                            m_arrLocalROI[i][j].LoadROISetting(0, 0, m_arrModifiedImage[i][j].ref_intImageWidth, m_arrModifiedImage[i][j].ref_intImageHeight);
                        }
                    }
                }

                //bool blnResult = GetPosition_UsingEdgeAndCornerLineGauge(m_arrLocalROI);

                ////if (!blnResult && m_arrGaugeEnableSubGauge.Contains(true))
                //if (!blnResult && m_arrGaugeMeasureMode.Contains(1))
                //    blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(m_arrLocalROI);


                bool blnResult;
                if (m_arrGaugeMeasureMode.Contains(3))
                {
                    if (bUseTemplateUnitSize)
                    {
                        //blnResult = GetPosition_UsingEdgeAndCornerLineGauge_CollectList(m_arrLocalROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints_CollectList(m_arrLocalROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                    }
                    else
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(m_arrLocalROI);
                    }
                }
                else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(m_arrLocalROI);
                }
                else
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge(m_arrLocalROI);
                }

                if (!blnResult)
                {
                    if (m_arrGaugeMeasureMode.Contains(1))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(m_arrLocalROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(m_arrLocalROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(3))
                    {
                        if (!bUseTemplateUnitSize)
                            blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(m_arrLocalROI);
                    }
                }
                //for (int i = 0; i < m_arrModifiedImage.Count; i++)
                //{
                //    for (int j = 0; j < m_arrModifiedImage[i].Count; j++)
                //    {
                //        if (m_arrModifiedImage[i][j] != null)
                //        {
                //            m_arrModifiedImage[i][j].Dispose();
                //            m_arrModifiedImage[i][j] = null;
                //        }
                //    }
                //}

                //for (int i = 0; i < arrROI.Count; i++)
                //{
                //    for (int j = 0; j < arrROI[i].Count; j++)
                //    {
                //        if (arrROI[i][j] != null)
                //        {
                //            arrROI[i][j].Dispose();
                //            arrROI[i][j] = null;
                //        }
                //    }
                //}

                //for (int i = 0; i < m_objPreModifiedImage.Length; i++)
                //{
                //    if (m_objPreModifiedImage[i] != null)
                //    {
                //        m_objPreModifiedImage[i].Dispose();
                //        m_objPreModifiedImage[i] = null;
                //    }
                //}

                return blnResult;
            }
            else
            {
                //for (int i = 0; i < arrImage.Count; i++)
                //{
                //    arrROI.Add(new ROI());
                //    arrROI[i].AttachImage(arrImage[i]);
                //    arrROI[i].LoadROISetting(0, 0, arrImage[i].ref_intImageWidth, arrImage[i].ref_intImageHeight);
                //}

                for (int i = 0; i < m_arrModifiedImage.Count; i++)
                {
                    if (i >= m_arrLocalROI.Count)
                        m_arrLocalROI.Add(new List<ROI>());

                    for (int j = 0; j < m_arrModifiedImage[i].Count; j++)
                    {
                        if (j >= m_arrLocalROI[i].Count)
                            m_arrLocalROI[i].Add(new ROI());

                        if (m_arrModifiedImage[i][j] != null)
                        {
                            m_arrLocalROI[i][j].AttachImage(m_arrModifiedImage[i][j]);
                            m_arrLocalROI[i][j].LoadROISetting(0, 0, m_arrModifiedImage[i][j].ref_intImageWidth, m_arrModifiedImage[i][j].ref_intImageHeight);
                        }
                    }
                }

                bool blnResult;
                if (m_arrGaugeMeasureMode.Contains(3))
                {
                    if (bUseTemplateUnitSize)
                    {
                        //blnResult = GetPosition_UsingEdgeAndCornerLineGauge_CollectList(m_arrLocalROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints_CollectList(m_arrLocalROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                    }
                    else
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(m_arrLocalROI);
                    }
                }
                else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(m_arrLocalROI);
                }
                else
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge(m_arrLocalROI);
                }

                if (!blnResult)
                {
                    if (m_arrGaugeMeasureMode.Contains(1))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(m_arrLocalROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(m_arrLocalROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(3))
                    {
                        if (!bUseTemplateUnitSize)
                            blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(m_arrLocalROI);
                    }
                }
                //for (int i = 0; i < m_arrModifiedImage.Count; i++)
                //{
                //    for (int j = 0; j < m_arrModifiedImage[i].Count; j++)
                //    {
                //        if (m_arrModifiedImage[i][j] != null)
                //        {
                //            m_arrModifiedImage[i][j].Dispose();
                //            m_arrModifiedImage[i][j] = null;
                //        }
                //    }
                //}

                //for (int i = 0; i < arrROI.Count; i++)
                //{
                //    for (int j = 0; j < arrROI[i].Count; j++)
                //    {
                //        if (arrROI[i][j] != null)
                //        {
                //            arrROI[i][j].Dispose();
                //            arrROI[i][j] = null;
                //        }
                //    }
                //}

                //for (int i = 0; i < m_objPreModifiedImage.Length; i++)
                //{
                //    if (m_objPreModifiedImage[i] != null)
                //    {
                //        m_objPreModifiedImage[i].Dispose();
                //        m_objPreModifiedImage[i] = null;
                //    }
                //}

                return blnResult;
            }
        }

        public bool Measure_WithDontCareArea_Old_2020_10_20(List<ImageDrawing> arrImage, ImageDrawing objWhiteImage, bool bUseTemplateUnitSize, float fTemplateUnitSizeX, float fTemplateUnitSizeY)
        {
            List<List<ImageDrawing>> arrModifiedImage = new List<List<ImageDrawing>>();
            ImageDrawing[] objPreModifiedImage = new ImageDrawing[4] { null, null, null, null };

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

                if (m_arrGaugeImageMode[i] == 0)
                {
                    if (m_arrGaugeWantImageThreshold[i])
                    {
                        if (objPreModifiedImage[i] == null)
                        {
                            objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                        }

                        m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageThreshold[i]);

                        m_arrEdgeROI[i].AttachImage(objImage);
                    }
                    else
                    {
                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageGain[i]);
                    }
                }
                else
                {
                    //if (objPreModifiedImage[i] == null)
                    //{
                    //    objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                    //}

                    //m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                    //m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                    //ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                    //m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                    //m_arrEdgeROI[i].PrewittTo_ROIToROISamePosition(ref objImage);

                    if (objPreModifiedImage[i] == null)
                    {
                        objPreModifiedImage[i] = new ImageDrawing(true, arrImage[intImageIndex].ref_intImageWidth, arrImage[intImageIndex].ref_intImageHeight);
                    }

                    m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
                    m_arrEdgeROI[i].HighPassTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i]);

                    if (m_arrGaugeWantImageThreshold[i])
                    {
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objPreModifiedImage[i], m_arrGaugeImageGain[i]);

                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].ThresholdTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageThreshold[i]);

                        m_arrEdgeROI[i].AttachImage(objImage);
                    }
                    else
                    {
                        ImageDrawing objImage = arrModifiedImage[intImageIndex][i];
                        m_arrEdgeROI[i].AttachImage(objPreModifiedImage[i]);
                        m_arrEdgeROI[i].GainTo_ROIToROISamePosition_Bigger(ref objImage, m_arrGaugeImageGain[i]);
                    }
                }

                m_arrEdgeROI[i].AttachImage(arrImage[intImageIndex]);
            }

            // if dont care present
            if (m_arrDontCareROI[0].Count != 0 || m_arrDontCareROI[1].Count != 0 || m_arrDontCareROI[2].Count != 0 || m_arrDontCareROI[3].Count != 0)
            {
                ModifyDontCareImage(arrModifiedImage, objWhiteImage);

                List<List<ROI>> arrROI = new List<List<ROI>>();
                for (int i = 0; i < arrModifiedImage.Count; i++)
                {
                    arrROI.Add(new List<ROI>());
                    for (int j = 0; j < arrModifiedImage[i].Count; j++)
                    {
                        arrROI[i].Add(new ROI());

                        if (arrModifiedImage[i][j] != null)
                        {
                            arrROI[i][j].AttachImage(arrModifiedImage[i][j]);
                            arrROI[i][j].LoadROISetting(0, 0, arrModifiedImage[i][j].ref_intImageWidth, arrModifiedImage[i][j].ref_intImageHeight);
                        }
                    }
                }

                //bool blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);

                ////if (!blnResult && m_arrGaugeEnableSubGauge.Contains(true))
                //if (!blnResult && m_arrGaugeMeasureMode.Contains(1))
                //    blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);


                bool blnResult;
                if (m_arrGaugeMeasureMode.Contains(3))
                {
                    if (bUseTemplateUnitSize)
                    {
                        //blnResult = GetPosition_UsingEdgeAndCornerLineGauge_CollectList(arrROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints_CollectList(arrROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                    }
                    else
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(arrROI);
                    }
                }
                else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(arrROI);
                }
                else
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);
                }

                if (!blnResult)
                {
                    if (m_arrGaugeMeasureMode.Contains(1))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(arrROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(3))
                    {
                        if (!bUseTemplateUnitSize)
                            blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(arrROI);
                    }
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

                for (int i = 0; i < arrROI.Count; i++)
                {
                    for (int j = 0; j < arrROI[i].Count; j++)
                    {
                        if (arrROI[i][j] != null)
                        {
                            arrROI[i][j].Dispose();
                            arrROI[i][j] = null;
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

                return blnResult;
            }
            else
            {
                List<List<ROI>> arrROI = new List<List<ROI>>();
                //for (int i = 0; i < arrImage.Count; i++)
                //{
                //    arrROI.Add(new ROI());
                //    arrROI[i].AttachImage(arrImage[i]);
                //    arrROI[i].LoadROISetting(0, 0, arrImage[i].ref_intImageWidth, arrImage[i].ref_intImageHeight);
                //}

                for (int i = 0; i < arrModifiedImage.Count; i++)
                {
                    arrROI.Add(new List<ROI>());
                    for (int j = 0; j < arrModifiedImage[i].Count; j++)
                    {
                        if (arrModifiedImage[i][j] != null)
                        {
                            arrROI[i].Add(new ROI());
                            arrROI[i][j].AttachImage(arrModifiedImage[i][j]);
                            arrROI[i][j].LoadROISetting(0, 0, arrModifiedImage[i][j].ref_intImageWidth, arrModifiedImage[i][j].ref_intImageHeight);
                        }
                        else
                        {
                            arrROI[i].Add(new ROI());
                        }
                    }
                }

                bool blnResult;
                if (m_arrGaugeMeasureMode.Contains(3))
                {
                    if (bUseTemplateUnitSize)
                    {
                        //blnResult = GetPosition_UsingEdgeAndCornerLineGauge_CollectList(arrROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints_CollectList(arrROI, bUseTemplateUnitSize, fTemplateUnitSizeX, fTemplateUnitSizeY);
                    }
                    else
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(arrROI);
                    }
                }
                else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints(arrROI);
                }
                else
                {
                    blnResult = GetPosition_UsingEdgeAndCornerLineGauge(arrROI);
                }

                if (!blnResult)
                {
                    if (m_arrGaugeMeasureMode.Contains(1))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge(arrROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(2) || m_arrGaugeMeasureMode.Contains(4))
                    {
                        blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(arrROI);
                    }
                    else if (m_arrGaugeMeasureMode.Contains(3))
                    {
                        if (!bUseTemplateUnitSize)
                            blnResult = GetPosition_UsingEdgeAndCornerLineGaugeSubGauge_SubGaugePoints(arrROI);
                    }
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

                for (int i = 0; i < arrROI.Count; i++)
                {
                    for (int j = 0; j < arrROI[i].Count; j++)
                    {
                        if (arrROI[i][j] != null)
                        {
                            arrROI[i][j].Dispose();
                            arrROI[i][j] = null;
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

                return blnResult;
            }
        }

        private bool GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints_CollectList(List<ROI> arrROI, bool bUseTemplateUnitSize, float fTemplateUnitSizeX, float fTemplateUnitSizeY)
        {
            ClearGaugeRecordList();

            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                m_arrLineGauge[i].Measure(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)], i);
            }

            FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            // Double verify Line Gauge result ok or not
            for (int i = 0; i < 4; i++)
            {
                if (blnResult[i])
                {
                    blnResult[i] = m_arrLineGauge[i].VerifyMeasurement_SubGaugePoints(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)]);
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
                List<LineGaugeRecord> arrLineGaugeRecord = m_arrLineGauge[i].MeasureAndCollect_SubGaugePoints(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)], 
                                                                                                              m_arrGaugeMinScore[i],
                                                                                                               m_arrGaugeGroupTolerance[i]);
                
                for (int r = 0; r < arrLineGaugeRecord.Count; r++)
                {
                    m_arrLineGaugeRecord[i].Add(arrLineGaugeRecord[r]);
                }
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            ////////FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);


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

            for (int top = 0; top < m_arrLineGaugeRecord[0].Count; top++)
            {
                for (int right = 0; right < m_arrLineGaugeRecord[1].Count; right++)
                {
                    for (int bottom = 0; bottom < m_arrLineGaugeRecord[2].Count; bottom++)
                    {
                        for (int left = 0; left < m_arrLineGaugeRecord[3].Count; left++)
                        {
                            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                            // Get corner point from Edge Line Gauges (Ver and Hor gauge edge line will close each other to generate corner point)
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

            //int intTop = arrTopIndex[intSelectedIndex];
            //int intRig = arrRightIndex[intSelectedIndex];
            //int intBot = arrBottomIndex[intSelectedIndex];
            //int intLef = arrLeftIndex[intSelectedIndex];
            if (intSelectedIndex >= 0)
            {
                {
                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    if (m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].bUseSubGauge)
                    {
                        m_arrLineGauge[0].SetParameterToSubGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[0], m_intVisionIndex)],
                            m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]]);
                    }

                    if (m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].bUseSubGauge)
                    {
                        m_arrLineGauge[1].SetParameterToSubGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[1], m_intVisionIndex)],
                            m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]]);
                    }

                    if (m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].bUseSubGauge)
                    {
                        m_arrLineGauge[2].SetParameterToSubGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[2], m_intVisionIndex)],
                            m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]]);
                    }

                    if (m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].bUseSubGauge)
                    {
                        m_arrLineGauge[3].SetParameterToSubGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[3], m_intVisionIndex)],
                            m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]]);
                    }

                    FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

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
                    m_fRectAngle = fTotalAngle / nCount;

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
                            if (!arrAngleOK[0] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Left angle
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

                            if (!arrAngleOK[1] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                            if (!arrAngleOK[2] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                            if (!arrAngleOK[3] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                        // ---------- Hor line ------------
                        arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                        m_fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;

                        m_fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;

                        // Verify Unit Size
                        //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                        PointF pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                        //else
                        //    isResultOK = false;

                        if (m_pRectCenterPoint.X.ToString() != "NaN" && m_pRectCenterPoint.Y.ToString() != "NaN" &&
                                             m_pRectCenterPoint.X.ToString() != "-NaN" && m_pRectCenterPoint.Y.ToString() != "-NaN" &&
                                             m_pRectCenterPoint.X.ToString() != "Infinity" && m_pRectCenterPoint.Y.ToString() != "Infinity" &&
                                             m_pRectCenterPoint.X.ToString() != "-Infinity" && m_pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                             m_fRectWidth != 0 && m_fRectHeight != 0)
                        {
                            m_pRectCenterPoint = pRectCenterPoint;

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
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    m_arrLineResultOK[i] = false;
                }
                isResultOK = false;
            }

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
            return true;
        }

        private bool GetPosition_UsingEdgeAndCornerLineGauge_SubGaugePoints_CollectList(List<List<ROI>> arrROI, bool bUseTemplateUnitSize, float fTemplateUnitSizeX, float fTemplateUnitSizeY)
        {
            ClearGaugeRecordList();

            // Init data
            float fTotalAngle = 0;
            int nCount = 0;
            bool[] blnResult = new bool[4];
            m_arrDrawLineUsingCornerPoint = new bool[4] { false, false, false, false };

            // Start measure image with Edge Line gauge
            for (int i = 0; i < 4; i++)
            {
                int intROIIndex = ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex);  // 2020 08 14 - CCENG - prevent out of index when use wrong recipe
                if (intROIIndex >= arrROI.Count)
                    intROIIndex = 0;

                m_arrLineGauge[i].Measure(arrROI[intROIIndex][i], i);
            }

            FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);

            // Double verify Line Gauge result ok or not
            for (int i = 0; i < 4; i++)
            {
                if (blnResult[i])
                {
                    blnResult[i] = m_arrLineGauge[i].VerifyMeasurement_SubGaugePoints(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[i], m_intVisionIndex)][i]);
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
                if (intROIIndex >= arrROI.Count)
                    intROIIndex = 0;

                List<LineGaugeRecord> arrLineGaugeRecord = m_arrLineGauge[i].MeasureAndCollect_SubGaugePoints(arrROI[intROIIndex][i], 
                                                                                                              m_arrGaugeMinScore[i],
                                                                                                               m_arrGaugeGroupTolerance[i]);

                for (int r = 0; r < arrLineGaugeRecord.Count; r++)
                {
                    m_arrLineGaugeRecord[i].Add(arrLineGaugeRecord[r]);
                }
            }

            // Reset all Corner Line Gauge 
            for (int i = 4; i < 12; i++)
            {
                m_arrLineGauge[i].Reset();
            }

            // Check is Edge Line Gauge measurement good
            ////////FilterEdgeAngle_Pad(m_arrLineGauge, 50, ref fTotalAngle, ref nCount, ref blnResult);


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

            int intZeroCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (m_arrReCheckUsingPointGaugeIfFail[i] && m_arrLineGaugeRecord[i].Count == 0)
                    intZeroCount++;
            }

            if (intZeroCount != 1) //2021-10-01 ZJYEOH : for this case, must have other 3 sides data to find the fail gauge
            {
                intZeroCount = 0;
            }

            for (int top = 0; top < Math.Max(m_arrLineGaugeRecord[0].Count, intZeroCount); top++)
            {
                for (int right = 0; right < Math.Max(m_arrLineGaugeRecord[1].Count, intZeroCount); right++)
                {
                    for (int bottom = 0; bottom < Math.Max(m_arrLineGaugeRecord[2].Count, intZeroCount); bottom++)
                    {
                        for (int left = 0; left < Math.Max(m_arrLineGaugeRecord[3].Count, intZeroCount); left++)
                        {
                            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            if (m_arrReCheckUsingPointGaugeIfFail[0] && m_arrLineGaugeRecord[0].Count == 0)
                            {
                                arrVirtualLine.Add(new List<Line>());
                                for (int a = 0; a < 2; a++)
                                {
                                    if (a == 0)
                                    {
                                        //Check Top Right
                                        SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[0], m_intVisionIndex)][0],
                                        m_arrLineGaugeRecord[1][right],
                                        m_arrLineGauge[0],
                                        0,
                                        1,
                                        a);


                                        if (m_arrPointGauge[0][a].ref_intMeasuredPointCount > 0)
                                        {
                                            objLineTop.CalculateLGStraightLine(m_arrPointGauge[0][a].GetMeasurePoint(), m_arrLineGaugeRecord[1][right].fMeasureAngle + 90);
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineTop);
                                        }
                                        else
                                        {
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineTop);
                                            continue;
                                        }
                                    }
                                    else if (a == 1)
                                    {
                                        //Check Top Left
                                        SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[0], m_intVisionIndex)][0],
                                        m_arrLineGaugeRecord[3][left],
                                        m_arrLineGauge[0],
                                        0,
                                        3,
                                        a);

                                        if (m_arrPointGauge[0][a].ref_intMeasuredPointCount > 0)
                                        {
                                            objLineTop.CalculateLGStraightLine(m_arrPointGauge[0][a].GetMeasurePoint(), m_arrLineGaugeRecord[3][left].fMeasureAngle + 90);
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineTop);
                                        }
                                        else
                                        {
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineTop);
                                            continue;
                                        }
                                    }
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
                                            arrVirtualLineIndex.Add(arrVirtualLine.Count - 1);
                                            arrVirtualLineIndex2.Add(a);
                                            isResultOK = true;
                                            m_arrDrawLineUsingCornerPoint[0] = true;
                                            m_arrPointGauge_ForDrawing[0] = m_arrPointGauge[0][a];

                                            if (!blnResult[0])
                                            {
                                                blnResult[0] = true;
                                                nCount++;
                                            }
                                        }
                                        else
                                        {
                                            m_strErrorMessage = "Measure Edge Fail!";
                                            isResultOK = false;
                                        }

                                    }
                                }
                            }
                            else if (m_arrReCheckUsingPointGaugeIfFail[1] && m_arrLineGaugeRecord[1].Count == 0)
                            {
                                arrVirtualLine.Add(new List<Line>());
                                for (int a = 0; a < 2; a++)
                                {
                                    if (a == 0)
                                    {
                                        //Check Top Right
                                        SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[1], m_intVisionIndex)][1],
                                        m_arrLineGaugeRecord[0][top],
                                        m_arrLineGauge[1],
                                        1,
                                        0,
                                        a);


                                        if (m_arrPointGauge[1][a].ref_intMeasuredPointCount > 0)
                                        {
                                            objLineRight.CalculateLGStraightLine(m_arrPointGauge[1][a].GetMeasurePoint(), m_arrLineGaugeRecord[0][top].fMeasureAngle + 90);
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineRight);
                                        }
                                        else
                                        {
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineRight);
                                            continue;
                                        }
                                    }
                                    else if (a == 1)
                                    {
                                        //Check Bottom Right
                                        SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[1], m_intVisionIndex)][1],
                                        m_arrLineGaugeRecord[2][bottom],
                                        m_arrLineGauge[1],
                                        1,
                                        2,
                                        a);

                                        if (m_arrPointGauge[1][a].ref_intMeasuredPointCount > 0)
                                        {
                                            objLineRight.CalculateLGStraightLine(m_arrPointGauge[1][a].GetMeasurePoint(), m_arrLineGaugeRecord[2][bottom].fMeasureAngle + 90);
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineRight);
                                        }
                                        else
                                        {
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineRight);
                                            continue;
                                        }
                                    }
                                    objLineTop = m_arrLineGaugeRecord[0][top].objLine;
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
                                            arrVirtualLineIndex.Add(arrVirtualLine.Count - 1);
                                            arrVirtualLineIndex2.Add(a);
                                            isResultOK = true;
                                            m_arrDrawLineUsingCornerPoint[1] = true;
                                            m_arrPointGauge_ForDrawing[1] = m_arrPointGauge[1][a];

                                            if (!blnResult[1])
                                            {
                                                blnResult[1] = true;
                                                nCount++;
                                            }
                                        }
                                        else
                                        {
                                            m_strErrorMessage = "Measure Edge Fail!";
                                            isResultOK = false;
                                        }

                                    }
                                }
                            }
                            else if (m_arrReCheckUsingPointGaugeIfFail[2] && m_arrLineGaugeRecord[2].Count == 0)
                            {
                                arrVirtualLine.Add(new List<Line>());
                                for (int a = 0; a < 2; a++)
                                {
                                    if (a == 0)
                                    {
                                        //Check Bottom Right
                                        SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[2], m_intVisionIndex)][2],
                                        m_arrLineGaugeRecord[1][right],
                                        m_arrLineGauge[2],
                                        2,
                                        1,
                                        a);


                                        if (m_arrPointGauge[2][a].ref_intMeasuredPointCount > 0)
                                        {
                                            objLineBottom.CalculateLGStraightLine(m_arrPointGauge[2][a].GetMeasurePoint(), m_arrLineGaugeRecord[1][right].fMeasureAngle + 90);
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineBottom);
                                        }
                                        else
                                        {
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineBottom);
                                            continue;
                                        }
                                    }
                                    else if (a == 1)
                                    {
                                        //Check Bottom Left
                                        SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[2], m_intVisionIndex)][2],
                                        m_arrLineGaugeRecord[3][left],
                                        m_arrLineGauge[2],
                                        2,
                                        3,
                                        a);

                                        if (m_arrPointGauge[2][a].ref_intMeasuredPointCount > 0)
                                        {
                                            objLineBottom.CalculateLGStraightLine(m_arrPointGauge[2][a].GetMeasurePoint(), m_arrLineGaugeRecord[3][left].fMeasureAngle + 90);
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineBottom);
                                        }
                                        else
                                        {
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineBottom);
                                            continue;
                                        }
                                    }
                                    objLineRight = m_arrLineGaugeRecord[1][right].objLine;
                                    objLineTop = m_arrLineGaugeRecord[0][top].objLine;
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
                                            arrVirtualLineIndex.Add(arrVirtualLine.Count - 1);
                                            arrVirtualLineIndex2.Add(a);
                                            isResultOK = true;
                                            m_arrDrawLineUsingCornerPoint[2] = true;
                                            m_arrPointGauge_ForDrawing[2] = m_arrPointGauge[2][a];

                                            if (!blnResult[2])
                                            {
                                                blnResult[2] = true;
                                                nCount++;
                                            }
                                        }
                                        else
                                        {
                                            m_strErrorMessage = "Measure Edge Fail!";
                                            isResultOK = false;
                                        }

                                    }
                                }
                            }
                            else if (m_arrReCheckUsingPointGaugeIfFail[3] && m_arrLineGaugeRecord[3].Count == 0)
                            {
                                arrVirtualLine.Add(new List<Line>());
                                for (int a = 0; a < 2; a++)
                                {
                                    if (a == 0)
                                    {
                                        //Check Top Right
                                        SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[3], m_intVisionIndex)][3],
                                        m_arrLineGaugeRecord[0][top],
                                        m_arrLineGauge[3],
                                        3,
                                        0,
                                        a);


                                        if (m_arrPointGauge[3][a].ref_intMeasuredPointCount > 0)
                                        {
                                            objLineLeft.CalculateLGStraightLine(m_arrPointGauge[3][a].GetMeasurePoint(), m_arrLineGaugeRecord[0][top].fMeasureAngle + 90);
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineLeft);
                                        }
                                        else
                                        {
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineLeft);
                                            continue;
                                        }
                                    }
                                    else if (a == 1)
                                    {
                                        //Check Bottom Right
                                        SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[3], m_intVisionIndex)][3],
                                        m_arrLineGaugeRecord[2][bottom],
                                        m_arrLineGauge[3],
                                        3,
                                        2,
                                        a);

                                        if (m_arrPointGauge[3][a].ref_intMeasuredPointCount > 0)
                                        {
                                            objLineLeft.CalculateLGStraightLine(m_arrPointGauge[3][a].GetMeasurePoint(), m_arrLineGaugeRecord[2][bottom].fMeasureAngle + 90);
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineLeft);
                                        }
                                        else
                                        {
                                            arrVirtualLine[arrVirtualLine.Count - 1].Add(objLineLeft);
                                            continue;
                                        }
                                    }
                                    objLineTop = m_arrLineGaugeRecord[0][top].objLine;
                                    objLineBottom = m_arrLineGaugeRecord[2][bottom].objLine;
                                    objLineRight = m_arrLineGaugeRecord[1][right].objLine;

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
                                            arrVirtualLineIndex.Add(arrVirtualLine.Count - 1);
                                            arrVirtualLineIndex2.Add(a);
                                            isResultOK = true;
                                            m_arrDrawLineUsingCornerPoint[3] = true;
                                            m_arrPointGauge_ForDrawing[3] = m_arrPointGauge[3][a];

                                            if (!blnResult[3])
                                            {
                                                blnResult[3] = true;
                                                nCount++;
                                            }
                                        }
                                        else
                                        {
                                            m_strErrorMessage = "Measure Edge Fail!";
                                            isResultOK = false;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

                                if (!isResultOK)
                                {
                                    if (m_arrReCheckUsingPointGaugeIfFail[0] && arrAngleOK[0] && arrAngleOK[2])
                                    {
                                        isResultOK = true;
                                    }
                                    else if (m_arrReCheckUsingPointGaugeIfFail[1] && arrAngleOK[1] && arrAngleOK[3])
                                    {
                                        isResultOK = true;
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

            //int intTop = arrTopIndex[intSelectedIndex];
            //int intRig = arrRightIndex[intSelectedIndex];
            //int intBot = arrBottomIndex[intSelectedIndex];
            //int intLef = arrLeftIndex[intSelectedIndex];
            if (intSelectedIndex >= 0)
            {
                {
                    ////////////Recheck Using Point Gauge //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    List<int> arrTopNewIndex = new List<int>();
                    List<int> arrBottomNewIndex = new List<int>();
                    List<int> arrLeftNewIndex = new List<int>();
                    List<int> arrRightNewIndex = new List<int>();

                    arrTopNewIndex.Add(arrTopIndex[intSelectedIndex]);
                    arrBottomNewIndex.Add(arrBottomIndex[intSelectedIndex]);
                    arrLeftNewIndex.Add(arrLeftIndex[intSelectedIndex]);
                    arrRightNewIndex.Add(arrRightIndex[intSelectedIndex]);

                    bool blnAddPointGauge = AddPointGauge(arrROI, arrRightIndex, arrLeftIndex, intSelectedIndex, ref arrTopNewIndex, 0, 3, 1);
                    blnAddPointGauge |= AddPointGauge(arrROI, arrRightIndex, arrLeftIndex, intSelectedIndex, ref arrBottomNewIndex, 2, 3, 1);
                    blnAddPointGauge |= AddPointGauge(arrROI, arrBottomIndex, arrTopIndex, intSelectedIndex, ref arrRightNewIndex, 1, 0, 2);
                    blnAddPointGauge |= AddPointGauge(arrROI, arrBottomIndex, arrTopIndex, intSelectedIndex, ref arrLeftNewIndex, 3, 0, 2);

                    if (blnAddPointGauge)
                    {
                        int intNewIndex_Start = arrResultWidth.Count;
                        for (int t = 0; t < arrTopNewIndex.Count; t++)
                        {
                            int top = arrTopNewIndex[t];
                            for (int r = 0; r < arrRightNewIndex.Count; r++)
                            {
                                int right = arrRightNewIndex[r];
                                for (int b = 0; b < arrBottomNewIndex.Count; b++)
                                {
                                    int bottom = arrBottomNewIndex[b];
                                    for (int l = 0; l < arrLeftNewIndex.Count; l++)
                                    {
                                        int left = arrLeftNewIndex[l];
                                        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

                                        if (!isResultOK)
                                        {
                                            if (m_arrReCheckUsingPointGaugeIfFail[0] && arrAngleOK[0] && arrAngleOK[2])
                                            {
                                                isResultOK = true;
                                            }
                                            else if (m_arrReCheckUsingPointGaugeIfFail[1] && arrAngleOK[1] && arrAngleOK[3])
                                            {
                                                isResultOK = true;
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

                        // Loop to get the best result
                        //fMinDifferent = float.MaxValue;
                        for (int i = intNewIndex_Start; i < arrResultWidth.Count; i++)
                        {
                            float fWidthDiff = Math.Abs(arrResultWidth[i] - m_fUnitSizeWidth);
                            float fHeightDiff = Math.Abs(arrResultHeight[i] - m_fUnitSizeHeight);

                            if (fMinDifferent > Math.Max(fWidthDiff, fHeightDiff))
                            {
                                fMinDifferent = Math.Max(fWidthDiff, fHeightDiff);
                                intSelectedIndex = i;
                            }
                        }

                    }




                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    float[] arrPointGaugeAngle = new float[] { -999f, -999f, -999f, -999f };


                    if (m_arrLineGaugeRecord[0].Count > arrTopIndex[intSelectedIndex] && m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].bUseSubGauge)
                    {
                        m_arrLineGauge[0].SetParameterToSubGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[0], m_intVisionIndex)][0],
                            m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]]);
                    }
                    else if (m_arrLineGaugeRecord[0].Count > arrTopIndex[intSelectedIndex] && m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].bUsePointGauge)
                    {
                        arrPointGaugeAngle[0] = LGauge.ConvertObjectAngle(0, m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].fMeasureAngle);
                        m_arrDrawLineUsingCornerPoint[0] = true;
                    }

                    if (m_arrLineGaugeRecord[1].Count > arrRightIndex[intSelectedIndex] && m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].bUseSubGauge)
                    {
                        m_arrLineGauge[1].SetParameterToSubGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[1], m_intVisionIndex)][1],
                            m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]]);
                    }
                    else if (m_arrLineGaugeRecord[1].Count > arrRightIndex[intSelectedIndex] && m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].bUsePointGauge)
                    {
                        arrPointGaugeAngle[1] = LGauge.ConvertObjectAngle(90, m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].fMeasureAngle);
                        m_arrDrawLineUsingCornerPoint[1] = true;
                    }

                    if (m_arrLineGaugeRecord[2].Count > arrBottomIndex[intSelectedIndex] && m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].bUseSubGauge)
                    {
                        m_arrLineGauge[2].SetParameterToSubGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[2], m_intVisionIndex)][2],
                            m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]]);
                    }
                    else if (m_arrLineGaugeRecord[2].Count > arrBottomIndex[intSelectedIndex] && m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].bUsePointGauge)
                    {
                        arrPointGaugeAngle[2] = LGauge.ConvertObjectAngle(180, m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].fMeasureAngle);
                        m_arrDrawLineUsingCornerPoint[2] = true;
                    }

                    if (m_arrLineGaugeRecord[3].Count > arrLeftIndex[intSelectedIndex] && m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].bUseSubGauge)
                    {
                        m_arrLineGauge[3].SetParameterToSubGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[3], m_intVisionIndex)][3],
                            m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]]);
                    }
                    else if (m_arrLineGaugeRecord[3].Count > arrLeftIndex[intSelectedIndex] && m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].bUsePointGauge)
                    {
                        arrPointGaugeAngle[3] = LGauge.ConvertObjectAngle(270, m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].fMeasureAngle);
                        m_arrDrawLineUsingCornerPoint[3] = true;
                    }

                    FilterEdgeAngle_Pad(m_arrLineGauge, 50, arrPointGaugeAngle, ref fTotalAngle, ref nCount, ref blnResult);

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
                    m_fRectAngle = fTotalAngle / nCount;

                    if (m_arrReCheckUsingPointGaugeIfFail[0] && m_arrLineGaugeRecord[0].Count == 0)
                    {
                        objLineTop = arrVirtualLine[arrVirtualLineIndex[intSelectedIndex]][arrVirtualLineIndex2[intSelectedIndex]];
                        objLineRight = m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine;
                        objLineBottom = m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine;
                        objLineLeft = m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine;
                    }
                    else if (m_arrReCheckUsingPointGaugeIfFail[1] && m_arrLineGaugeRecord[1].Count == 0)
                    {
                        objLineTop = m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine;
                        objLineRight = arrVirtualLine[arrVirtualLineIndex[intSelectedIndex]][arrVirtualLineIndex2[intSelectedIndex]];
                        objLineBottom = m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine;
                        objLineLeft = m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine;
                    }
                    else if (m_arrReCheckUsingPointGaugeIfFail[2] && m_arrLineGaugeRecord[2].Count == 0)
                    {
                        objLineTop = m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine;
                        objLineRight = m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine;
                        objLineBottom = arrVirtualLine[arrVirtualLineIndex[intSelectedIndex]][arrVirtualLineIndex2[intSelectedIndex]];
                        objLineLeft = m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine;
                    }
                    else if (m_arrReCheckUsingPointGaugeIfFail[3] && m_arrLineGaugeRecord[3].Count == 0)
                    {
                        objLineTop = m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine;
                        objLineRight = m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine;
                        objLineBottom = m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine;
                        objLineLeft = arrVirtualLine[arrVirtualLineIndex[intSelectedIndex]][arrVirtualLineIndex2[intSelectedIndex]];
                    }
                    else
                    {
                        objLineTop = m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine;
                        objLineRight = m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine;
                        objLineBottom = m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine;
                        objLineLeft = m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine;
                    }

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
                            if (!arrAngleOK[0] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Left angle
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

                            if (!arrAngleOK[1] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                            if (!arrAngleOK[2] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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

                            if (!arrAngleOK[3] && m_fUnitSizeHeight > 0 && m_fUnitSizeWidth > 0) // Top Right angle
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
                        //2021-10-01 ZJYEOH : If got parallel line ok, can use to find opposite line
                        if (m_arrReCheckUsingPointGaugeIfFail[0] && blnResult[0] && blnResult[2] && m_arrLineGaugeRecord[0].Count != 0 && m_arrLineGaugeRecord[2].Count != 0) //Top Bottom OK
                        {
                            if (!blnResult[1])
                            {
                                PointF[][] arrRectCornerPoints_Temp = { new PointF[2] { new PointF(), new PointF() }, new PointF[2] { new PointF(), new PointF() } };
                                float[][] arrCornerGaugeAngles_Temp = { new float[2] { 90, 90 }, new float[2] { 90, 90 } };
                                Line objLine_Temp = new Line();
                                bool blnResult1 = true, blnResult2 = true;
                                //Check Top Right
                                SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[1], m_intVisionIndex)][1],
                                m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]],
                                m_arrLineGauge[1],
                                1,
                                0,
                                0);

                                if (m_arrPointGauge[1][0].ref_intMeasuredPointCount > 0)
                                {
                                    objLine_Temp.CalculateLGStraightLine(m_arrPointGauge[1][0].GetMeasurePoint(), m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].fMeasureAngle + 90);

                                    arrRectCornerPoints_Temp[0][0] = Line.GetCrossPoint(m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[0][0] = Math2.GetAngle(m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine, objLine_Temp);

                                    arrRectCornerPoints_Temp[0][1] = Line.GetCrossPoint(m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[0][1] = Math2.GetAngle(m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine, objLine_Temp);
                                }
                                else
                                {
                                    blnResult1 = false;
                                }

                                //Check Bottom Right
                                SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[1], m_intVisionIndex)][1],
                               m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]],
                               m_arrLineGauge[1],
                               1,
                               2,
                               1);

                                if (m_arrPointGauge[1][1].ref_intMeasuredPointCount > 0)
                                {
                                    objLine_Temp.CalculateLGStraightLine(m_arrPointGauge[1][1].GetMeasurePoint(), m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].fMeasureAngle + 90);

                                    arrRectCornerPoints_Temp[1][0] = Line.GetCrossPoint(m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[1][0] = Math2.GetAngle(m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine, objLine_Temp);

                                    arrRectCornerPoints_Temp[1][1] = Line.GetCrossPoint(m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[1][1] = Math2.GetAngle(m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine, objLine_Temp);
                                }
                                else
                                {
                                    blnResult2 = false;
                                }
                                
                                if (!blnResult1 && !blnResult2)
                                    isResultOK = false;
                                else
                                    isResultOK = true;

                                if (isResultOK)
                                {
                                    // Verify corner angle
                                    int intBestAngleIndex = -1;
                                    float fBestAngle = float.MaxValue;
                                    bool[] arrAngleOK = new bool[2];
                                    for (int i = 0; i < arrCornerGaugeAngles_Temp.Length; i++)
                                    {
                                        for (int j = 0; j < arrCornerGaugeAngles_Temp[i].Length; j++)
                                        {
                                            float fAngle = Math.Abs(arrCornerGaugeAngles_Temp[i][j] - 90);
                                            if (fAngle > m_arrGaugeMaxAngle[1])
                                            {
                                                if (isResultOK)
                                                {
                                                    m_strErrorMessage = "*Measure Edge Fail. Corner angle out of tolerance. Setting=" + m_arrGaugeMaxAngle[1].ToString() + ", Result=" + fAngle.ToString();
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
                                    }
                                }

                                if (isResultOK)
                                {
                                    List<float> arrResultWidth_Temp = new List<float>();
                                    List<float> arrResultHeight_Temp = new List<float>();
                                    int intSelectedIndex_Temp = -1;
                                    float fMinDifferent_Temp = float.MaxValue;
                                    for (int i = 0; i < arrRectCornerPoints_Temp.Length; i++)
                                    {
                                        // Get rectangle corner close lines
                                        arrCrossLines = new Line[2];
                                        arrCrossLines[0] = new Line();
                                        arrCrossLines[1] = new Line();
                                        // Calculate center point
                                        // -------- ver line ---------------------------------------
                                        arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], arrRectCornerPoints_Temp[i][1]);

                                        // ---------- Hor line ------------
                                        arrCrossLines[1].CalculateStraightLine(arrRectCornerPoints_Temp[i][0], m_arrRectCornerPoints[3]);

                                        // Verify Unit Size
                                        //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                                        PointF pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                                        float fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], arrRectCornerPoints_Temp[i][0]) + Math2.GetDistanceBtw2Points(arrRectCornerPoints_Temp[i][1], m_arrRectCornerPoints[3])) / 2;
                                        float fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(arrRectCornerPoints_Temp[i][0], arrRectCornerPoints_Temp[i][1])) / 2;
                                        //else
                                        //    isResultOK = false;

                                        if (pRectCenterPoint.X.ToString() != "NaN" && pRectCenterPoint.Y.ToString() != "NaN" &&
                                                             pRectCenterPoint.X.ToString() != "-NaN" && pRectCenterPoint.Y.ToString() != "-NaN" &&
                                                             pRectCenterPoint.X.ToString() != "Infinity" && pRectCenterPoint.Y.ToString() != "Infinity" &&
                                                             pRectCenterPoint.X.ToString() != "-Infinity" && pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                                             fRectWidth != 0 && fRectHeight != 0)
                                        {

                                            arrResultWidth_Temp.Add(fRectWidth);

                                            arrResultHeight_Temp.Add(fRectHeight);

                                            float fWidthDiff = Math.Abs(arrResultWidth_Temp[arrResultHeight_Temp.Count - 1] - m_fUnitSizeWidth);
                                            float fHeightDiff = Math.Abs(arrResultHeight_Temp[arrResultHeight_Temp.Count - 1] - m_fUnitSizeHeight);

                                            if (fMinDifferent_Temp > Math.Max(fWidthDiff, fHeightDiff))
                                            {
                                                fMinDifferent_Temp = Math.Max(fWidthDiff, fHeightDiff);
                                                intSelectedIndex_Temp = i;

                                                m_arrRectCornerPoints[1] = arrRectCornerPoints_Temp[i][0];
                                                m_arrRectCornerPoints[2] = arrRectCornerPoints_Temp[i][1];
                                                m_arrDrawLineUsingCornerPoint[1] = true;
                                                m_arrPointGauge_ForDrawing[1] = m_arrPointGauge[1][i];
                                            }

                                            if (!blnResult[1])
                                            {
                                                m_arrLineResultOK[1] = blnResult[1] = true;
                                                nCount++;
                                            }
                                        }
                                        else
                                        {
                                            //m_strErrorMessage = "Measure Edge Fail!";
                                            //isResultOK = false;
                                        }

                                        if (intSelectedIndex_Temp < 0)
                                        {
                                            isResultOK = false;
                                            m_strErrorMessage = "*Measure Edge Fail.";
                                        }
                                    }

                                }
                                else
                                {
                                    isResultOK = false;
                                    m_strErrorMessage = "*Measure Edge Fail.";
                                    //m_intFailResultMask |= 0x02;
                                    //return false;
                                }
                            }

                            if (!blnResult[3])
                            {
                                PointF[][] arrRectCornerPoints_Temp = { new PointF[2] { new PointF(), new PointF() }, new PointF[2] { new PointF(), new PointF() } };
                                float[][] arrCornerGaugeAngles_Temp = { new float[2] { 90, 90 }, new float[2] { 90, 90 } };
                                Line objLine_Temp = new Line();
                                bool blnResult1 = true, blnResult2 = true;
                                //Check Top Left
                                SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[3], m_intVisionIndex)][3],
                                m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]],
                                m_arrLineGauge[3],
                                3,
                                0,
                                0);

                                if (m_arrPointGauge[3][0].ref_intMeasuredPointCount > 0)
                                {
                                    objLine_Temp.CalculateLGStraightLine(m_arrPointGauge[3][0].GetMeasurePoint(), m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].fMeasureAngle + 90);

                                    arrRectCornerPoints_Temp[0][0] = Line.GetCrossPoint(m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[0][0] = Math2.GetAngle(m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine, objLine_Temp);

                                    arrRectCornerPoints_Temp[0][1] = Line.GetCrossPoint(m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[0][1] = Math2.GetAngle(m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine, objLine_Temp);
                                }
                                else
                                {
                                    blnResult1 = false;
                                }

                                //Check Bottom Left
                                SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[3], m_intVisionIndex)][3],
                               m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]],
                               m_arrLineGauge[3],
                               3,
                               2,
                               1);

                                if (m_arrPointGauge[3][1].ref_intMeasuredPointCount > 0)
                                {
                                    objLine_Temp.CalculateLGStraightLine(m_arrPointGauge[3][1].GetMeasurePoint(), m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].fMeasureAngle + 90);

                                    arrRectCornerPoints_Temp[1][0] = Line.GetCrossPoint(m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[1][0] = Math2.GetAngle(m_arrLineGaugeRecord[0][arrTopIndex[intSelectedIndex]].objLine, objLine_Temp);

                                    arrRectCornerPoints_Temp[1][1] = Line.GetCrossPoint(m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[1][1] = Math2.GetAngle(m_arrLineGaugeRecord[2][arrBottomIndex[intSelectedIndex]].objLine, objLine_Temp);
                                }
                                else
                                {
                                    blnResult2 = false;
                                }

                                if (!blnResult1 && !blnResult2)
                                    isResultOK = false;
                                else
                                    isResultOK = true;

                                if (isResultOK)
                                {
                                    // Verify corner angle
                                    int intBestAngleIndex = -1;
                                    float fBestAngle = float.MaxValue;
                                    bool[] arrAngleOK = new bool[2];
                                    for (int i = 0; i < arrCornerGaugeAngles_Temp.Length; i++)
                                    {
                                        for (int j = 0; j < arrCornerGaugeAngles_Temp[i].Length; j++)
                                        {
                                            float fAngle = Math.Abs(arrCornerGaugeAngles_Temp[i][j] - 90);
                                            if (fAngle > m_arrGaugeMaxAngle[3])
                                            {
                                                if (isResultOK)
                                                {
                                                    m_strErrorMessage = "*Measure Edge Fail. Corner angle out of tolerance. Setting=" + m_arrGaugeMaxAngle[3].ToString() + ", Result=" + fAngle.ToString();
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
                                    }
                                }

                                if (isResultOK)
                                {
                                    List<float> arrResultWidth_Temp = new List<float>();
                                    List<float> arrResultHeight_Temp = new List<float>();
                                    int intSelectedIndex_Temp = -1;
                                    float fMinDifferent_Temp = float.MaxValue;
                                    for (int i = 0; i < arrRectCornerPoints_Temp.Length; i++)
                                    {
                                        // Get rectangle corner close lines
                                        arrCrossLines = new Line[2];
                                        arrCrossLines[0] = new Line();
                                        arrCrossLines[1] = new Line();
                                        // Calculate center point
                                        // -------- ver line ---------------------------------------
                                        arrCrossLines[0].CalculateStraightLine(arrRectCornerPoints_Temp[i][0], m_arrRectCornerPoints[2]);

                                        // ---------- Hor line ------------
                                        arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], arrRectCornerPoints_Temp[i][1]);

                                        // Verify Unit Size
                                        //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                                        PointF pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                                        float fRectWidth = (Math2.GetDistanceBtw2Points(arrRectCornerPoints_Temp[i][0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], arrRectCornerPoints_Temp[i][1])) / 2;
                                        float fRectHeight = (Math2.GetDistanceBtw2Points(arrRectCornerPoints_Temp[i][0], arrRectCornerPoints_Temp[i][1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;
                                        //else
                                        //    isResultOK = false;

                                        if (pRectCenterPoint.X.ToString() != "NaN" && pRectCenterPoint.Y.ToString() != "NaN" &&
                                                             pRectCenterPoint.X.ToString() != "-NaN" && pRectCenterPoint.Y.ToString() != "-NaN" &&
                                                             pRectCenterPoint.X.ToString() != "Infinity" && pRectCenterPoint.Y.ToString() != "Infinity" &&
                                                             pRectCenterPoint.X.ToString() != "-Infinity" && pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                                             fRectWidth != 0 && fRectHeight != 0)
                                        {

                                            arrResultWidth_Temp.Add(fRectWidth);

                                            arrResultHeight_Temp.Add(fRectHeight);

                                            float fWidthDiff = Math.Abs(arrResultWidth_Temp[arrResultHeight_Temp.Count - 1] - m_fUnitSizeWidth);
                                            float fHeightDiff = Math.Abs(arrResultHeight_Temp[arrResultHeight_Temp.Count - 1] - m_fUnitSizeHeight);

                                            if (fMinDifferent_Temp > Math.Max(fWidthDiff, fHeightDiff))
                                            {
                                                fMinDifferent_Temp = Math.Max(fWidthDiff, fHeightDiff);
                                                intSelectedIndex_Temp = i;

                                                m_arrRectCornerPoints[0] = arrRectCornerPoints_Temp[i][0];
                                                m_arrRectCornerPoints[3] = arrRectCornerPoints_Temp[i][1];
                                                m_arrDrawLineUsingCornerPoint[3] = true;
                                                m_arrPointGauge_ForDrawing[3] = m_arrPointGauge[3][i];
                                            }

                                            if (!blnResult[3])
                                            {
                                                m_arrLineResultOK[3] = blnResult[3] = true;
                                                nCount++;
                                            }
                                        }
                                        else
                                        {
                                            //m_strErrorMessage = "Measure Edge Fail!";
                                            //isResultOK = false;
                                        }

                                        if (intSelectedIndex_Temp < 0)
                                        {
                                            isResultOK = false;
                                            m_strErrorMessage = "*Measure Edge Fail.";
                                        }
                                    }

                                }
                                else
                                {
                                    isResultOK = false;
                                    m_strErrorMessage = "*Measure Edge Fail.";
                                    //m_intFailResultMask |= 0x02;
                                    //return false;
                                }
                            }
                        }
                        else if (m_arrReCheckUsingPointGaugeIfFail[1] && blnResult[1] && blnResult[3] && m_arrLineGaugeRecord[1].Count != 0 && m_arrLineGaugeRecord[3].Count != 0)//Left Right OK
                        {
                            if (!blnResult[0])
                            {
                                PointF[][] arrRectCornerPoints_Temp = { new PointF[2] { new PointF(), new PointF() }, new PointF[2] { new PointF(), new PointF() } };
                                float[][] arrCornerGaugeAngles_Temp = { new float[2] { 90, 90 }, new float[2] { 90, 90 } };
                                Line objLine_Temp = new Line();
                                bool blnResult1 = true, blnResult2 = true;
                                //Check Top Right
                                SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[0], m_intVisionIndex)][0],
                                m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]],
                                m_arrLineGauge[0],
                                0,
                                1,
                                0);

                                if (m_arrPointGauge[0][0].ref_intMeasuredPointCount > 0)
                                {
                                    objLine_Temp.CalculateLGStraightLine(m_arrPointGauge[0][0].GetMeasurePoint(), m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].fMeasureAngle + 90);

                                    arrRectCornerPoints_Temp[0][0] = Line.GetCrossPoint(m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[0][0] = Math2.GetAngle(m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine, objLine_Temp);

                                    arrRectCornerPoints_Temp[0][1] = Line.GetCrossPoint(m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[0][1] = Math2.GetAngle(m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine, objLine_Temp);
                                }
                                else
                                {
                                    blnResult1 = false;
                                }

                                //Check Top Left
                                SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[0], m_intVisionIndex)][0],
                               m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]],
                               m_arrLineGauge[0],
                               0,
                               3,
                               1);

                                if (m_arrPointGauge[0][1].ref_intMeasuredPointCount > 0)
                                {
                                    objLine_Temp.CalculateLGStraightLine(m_arrPointGauge[0][1].GetMeasurePoint(), m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].fMeasureAngle + 90);

                                    arrRectCornerPoints_Temp[1][0] = Line.GetCrossPoint(m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[1][0] = Math2.GetAngle(m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine, objLine_Temp);

                                    arrRectCornerPoints_Temp[1][1] = Line.GetCrossPoint(m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[1][1] = Math2.GetAngle(m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine, objLine_Temp);
                                }
                                else
                                {
                                    blnResult2 = false;
                                }

                                if (!blnResult1 && !blnResult2)
                                    isResultOK = false;
                                else
                                    isResultOK = true;

                                if (isResultOK)
                                {
                                    // Verify corner angle
                                    int intBestAngleIndex = -1;
                                    float fBestAngle = float.MaxValue;
                                    bool[] arrAngleOK = new bool[2];
                                    for (int i = 0; i < arrCornerGaugeAngles_Temp.Length; i++)
                                    {
                                        for (int j = 0; j < arrCornerGaugeAngles_Temp[i].Length; j++)
                                        {
                                            float fAngle = Math.Abs(arrCornerGaugeAngles_Temp[i][j] - 90);
                                            if (fAngle > m_arrGaugeMaxAngle[0])
                                            {
                                                if (isResultOK)
                                                {
                                                    m_strErrorMessage = "*Measure Edge Fail. Corner angle out of tolerance. Setting=" + m_arrGaugeMaxAngle[0].ToString() + ", Result=" + fAngle.ToString();
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
                                    }
                                }

                                if (isResultOK)
                                {
                                    List<float> arrResultWidth_Temp = new List<float>();
                                    List<float> arrResultHeight_Temp = new List<float>();
                                    int intSelectedIndex_Temp = -1;
                                    float fMinDifferent_Temp = float.MaxValue;
                                    for (int i = 0; i < arrRectCornerPoints_Temp.Length; i++)
                                    {
                                        // Get rectangle corner close lines
                                        arrCrossLines = new Line[2];
                                        arrCrossLines[0] = new Line();
                                        arrCrossLines[1] = new Line();
                                        // Calculate center point
                                        // -------- ver line ---------------------------------------
                                        arrCrossLines[0].CalculateStraightLine(arrRectCornerPoints_Temp[i][0], m_arrRectCornerPoints[2]);

                                        // ---------- Hor line ------------
                                        arrCrossLines[1].CalculateStraightLine(arrRectCornerPoints_Temp[i][1], m_arrRectCornerPoints[3]);

                                        // Verify Unit Size
                                        //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                                        PointF pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                                        float fRectWidth = (Math2.GetDistanceBtw2Points(arrRectCornerPoints_Temp[i][0], arrRectCornerPoints_Temp[i][1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;
                                        float fRectHeight = (Math2.GetDistanceBtw2Points(arrRectCornerPoints_Temp[i][0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(arrRectCornerPoints_Temp[i][1], m_arrRectCornerPoints[2])) / 2;
                                        //else
                                        //    isResultOK = false;

                                        if (pRectCenterPoint.X.ToString() != "NaN" && pRectCenterPoint.Y.ToString() != "NaN" &&
                                                             pRectCenterPoint.X.ToString() != "-NaN" && pRectCenterPoint.Y.ToString() != "-NaN" &&
                                                             pRectCenterPoint.X.ToString() != "Infinity" && pRectCenterPoint.Y.ToString() != "Infinity" &&
                                                             pRectCenterPoint.X.ToString() != "-Infinity" && pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                                             fRectWidth != 0 && fRectHeight != 0)
                                        {

                                            arrResultWidth_Temp.Add(fRectWidth);

                                            arrResultHeight_Temp.Add(fRectHeight);

                                            float fWidthDiff = Math.Abs(arrResultWidth_Temp[arrResultHeight_Temp.Count - 1] - m_fUnitSizeWidth);
                                            float fHeightDiff = Math.Abs(arrResultHeight_Temp[arrResultHeight_Temp.Count - 1] - m_fUnitSizeHeight);

                                            if (fMinDifferent_Temp > Math.Max(fWidthDiff, fHeightDiff))
                                            {
                                                fMinDifferent_Temp = Math.Max(fWidthDiff, fHeightDiff);
                                                intSelectedIndex_Temp = i;

                                                m_arrRectCornerPoints[0] = arrRectCornerPoints_Temp[i][0];
                                                m_arrRectCornerPoints[1] = arrRectCornerPoints_Temp[i][1];
                                                m_arrDrawLineUsingCornerPoint[0] = true;
                                                m_arrPointGauge_ForDrawing[0] = m_arrPointGauge[0][i];
                                            }

                                            if (!blnResult[0])
                                            {
                                                m_arrLineResultOK[0] = blnResult[0] = true;
                                                nCount++;
                                            }
                                        }
                                        else
                                        {
                                            //m_strErrorMessage = "Measure Edge Fail!";
                                            //isResultOK = false;
                                        }

                                        if (intSelectedIndex_Temp < 0)
                                        {
                                            isResultOK = false;
                                            m_strErrorMessage = "*Measure Edge Fail.";
                                        }
                                    }

                                }
                                else
                                {
                                    isResultOK = false;
                                    m_strErrorMessage = "*Measure Edge Fail.";
                                    //m_intFailResultMask |= 0x02;
                                    //return false;
                                }
                            }

                            if (!blnResult[2])
                            {
                                PointF[][] arrRectCornerPoints_Temp = { new PointF[2] { new PointF(), new PointF() }, new PointF[2] { new PointF(), new PointF() } };
                                float[][] arrCornerGaugeAngles_Temp = { new float[2] { 90, 90 }, new float[2] { 90, 90 } };
                                Line objLine_Temp = new Line();
                                bool blnResult1 = true, blnResult2 = true;
                                //Check Bottom Right
                                SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[2], m_intVisionIndex)][2],
                                m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]],
                                m_arrLineGauge[2],
                                2,
                                1,
                                0);

                                if (m_arrPointGauge[2][0].ref_intMeasuredPointCount > 0)
                                {
                                    objLine_Temp.CalculateLGStraightLine(m_arrPointGauge[2][0].GetMeasurePoint(), m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].fMeasureAngle + 90);

                                    arrRectCornerPoints_Temp[0][0] = Line.GetCrossPoint(m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[0][0] = Math2.GetAngle(m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine, objLine_Temp);

                                    arrRectCornerPoints_Temp[0][1] = Line.GetCrossPoint(m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[0][1] = Math2.GetAngle(m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine, objLine_Temp);
                                }
                                else
                                {
                                    blnResult1 = false;
                                }

                                //Check Bottom Left
                                SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[2], m_intVisionIndex)][2],
                               m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]],
                               m_arrLineGauge[2],
                               2,
                               3,
                               1);

                                if (m_arrPointGauge[2][1].ref_intMeasuredPointCount > 0)
                                {
                                    objLine_Temp.CalculateLGStraightLine(m_arrPointGauge[2][1].GetMeasurePoint(), m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].fMeasureAngle + 90);

                                    arrRectCornerPoints_Temp[1][0] = Line.GetCrossPoint(m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[1][0] = Math2.GetAngle(m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].objLine, objLine_Temp);

                                    arrRectCornerPoints_Temp[1][1] = Line.GetCrossPoint(m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine, objLine_Temp);
                                    arrCornerGaugeAngles_Temp[1][1] = Math2.GetAngle(m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].objLine, objLine_Temp);
                                }
                                else
                                {
                                    blnResult2 = false;
                                }

                                if (!blnResult1 && !blnResult2)
                                    isResultOK = false;
                                else
                                    isResultOK = true;

                                if (isResultOK)
                                {
                                    // Verify corner angle
                                    int intBestAngleIndex = -1;
                                    float fBestAngle = float.MaxValue;
                                    bool[] arrAngleOK = new bool[2];
                                    for (int i = 0; i < arrCornerGaugeAngles_Temp.Length; i++)
                                    {
                                        for (int j = 0; j < arrCornerGaugeAngles_Temp[i].Length; j++)
                                        {
                                            float fAngle = Math.Abs(arrCornerGaugeAngles_Temp[i][j] - 90);
                                            if (fAngle > m_arrGaugeMaxAngle[2])
                                            {
                                                if (isResultOK)
                                                {
                                                    m_strErrorMessage = "*Measure Edge Fail. Corner angle out of tolerance. Setting=" + m_arrGaugeMaxAngle[2].ToString() + ", Result=" + fAngle.ToString();
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
                                    }
                                }

                                if (isResultOK)
                                {
                                    List<float> arrResultWidth_Temp = new List<float>();
                                    List<float> arrResultHeight_Temp = new List<float>();
                                    int intSelectedIndex_Temp = -1;
                                    float fMinDifferent_Temp = float.MaxValue;
                                    for (int i = 0; i < arrRectCornerPoints_Temp.Length; i++)
                                    {
                                        // Get rectangle corner close lines
                                        arrCrossLines = new Line[2];
                                        arrCrossLines[0] = new Line();
                                        arrCrossLines[1] = new Line();
                                        // Calculate center point
                                        // -------- ver line ---------------------------------------
                                        arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], arrRectCornerPoints_Temp[i][1]);

                                        // ---------- Hor line ------------
                                        arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], arrRectCornerPoints_Temp[i][0]);

                                        // Verify Unit Size
                                        //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                                        PointF pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                                        float fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(arrRectCornerPoints_Temp[i][1], arrRectCornerPoints_Temp[i][0])) / 2;
                                        float fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], arrRectCornerPoints_Temp[i][0]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], arrRectCornerPoints_Temp[i][1])) / 2;
                                        //else
                                        //    isResultOK = false;

                                        if (pRectCenterPoint.X.ToString() != "NaN" && pRectCenterPoint.Y.ToString() != "NaN" &&
                                                             pRectCenterPoint.X.ToString() != "-NaN" && pRectCenterPoint.Y.ToString() != "-NaN" &&
                                                             pRectCenterPoint.X.ToString() != "Infinity" && pRectCenterPoint.Y.ToString() != "Infinity" &&
                                                             pRectCenterPoint.X.ToString() != "-Infinity" && pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                                             fRectWidth != 0 && fRectHeight != 0)
                                        {

                                            arrResultWidth_Temp.Add(fRectWidth);

                                            arrResultHeight_Temp.Add(fRectHeight);

                                            float fWidthDiff = Math.Abs(arrResultWidth_Temp[arrResultHeight_Temp.Count - 1] - m_fUnitSizeWidth);
                                            float fHeightDiff = Math.Abs(arrResultHeight_Temp[arrResultHeight_Temp.Count - 1] - m_fUnitSizeHeight);

                                            if (fMinDifferent_Temp > Math.Max(fWidthDiff, fHeightDiff))
                                            {
                                                fMinDifferent_Temp = Math.Max(fWidthDiff, fHeightDiff);
                                                intSelectedIndex_Temp = i;

                                                m_arrRectCornerPoints[3] = arrRectCornerPoints_Temp[i][0];
                                                m_arrRectCornerPoints[2] = arrRectCornerPoints_Temp[i][1];
                                                m_arrDrawLineUsingCornerPoint[2] = true;
                                                m_arrPointGauge_ForDrawing[2] = m_arrPointGauge[2][i];
                                            }

                                            if (!blnResult[2])
                                            {
                                                m_arrLineResultOK[2] = blnResult[2] = true;
                                                nCount++;
                                            }
                                        }
                                        else
                                        {
                                            //m_strErrorMessage = "Measure Edge Fail!";
                                            //isResultOK = false;
                                        }

                                        if (intSelectedIndex_Temp < 0)
                                        {
                                            isResultOK = false;
                                            m_strErrorMessage = "*Measure Edge Fail.";
                                        }
                                    }

                                }
                                else
                                {
                                    isResultOK = false;
                                    m_strErrorMessage = "*Measure Edge Fail.";
                                    //m_intFailResultMask |= 0x02;
                                    //return false;
                                }
                            }
                        }
                        else
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
                        arrCrossLines = new Line[2];
                        arrCrossLines[0] = new Line();
                        arrCrossLines[1] = new Line();

                        // Calculate center point
                        // -------- ver line ---------------------------------------
                        arrCrossLines[0].CalculateStraightLine(m_arrRectCornerPoints[0], m_arrRectCornerPoints[2]);

                        // ---------- Hor line ------------
                        arrCrossLines[1].CalculateStraightLine(m_arrRectCornerPoints[1], m_arrRectCornerPoints[3]);

                        m_fRectWidth = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[1]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[2], m_arrRectCornerPoints[3])) / 2;

                        m_fRectHeight = (Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[0], m_arrRectCornerPoints[3]) + Math2.GetDistanceBtw2Points(m_arrRectCornerPoints[1], m_arrRectCornerPoints[2])) / 2;

                        // Verify Unit Size
                        //if ((Math.Abs(m_fRectWidth - m_fUnitSizeWidth) < 5) && (Math.Abs(m_fRectHeight - m_fUnitSizeHeight) < 5))
                        PointF pRectCenterPoint = Line.GetCrossPoint(arrCrossLines[0], arrCrossLines[1]);
                        //else
                        //    isResultOK = false;

                        if (m_pRectCenterPoint.X.ToString() != "NaN" && m_pRectCenterPoint.Y.ToString() != "NaN" &&
                                             m_pRectCenterPoint.X.ToString() != "-NaN" && m_pRectCenterPoint.Y.ToString() != "-NaN" &&
                                             m_pRectCenterPoint.X.ToString() != "Infinity" && m_pRectCenterPoint.Y.ToString() != "Infinity" &&
                                             m_pRectCenterPoint.X.ToString() != "-Infinity" && m_pRectCenterPoint.Y.ToString() != "-Infinity" &&
                                             m_fRectWidth != 0 && m_fRectHeight != 0)
                        {
                            m_pRectCenterPoint = pRectCenterPoint;

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
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    m_arrLineResultOK[i] = false;
                }
                isResultOK = false;
            }

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
            return true;
        }
        private void SetParameterToPointGauge(ROI objROI, LineGaugeRecord objLineGaugeRecord, LGauge m_LineGauge, int intROIIndex, int intLineGaugeIndex, int intPointGaugeIndex)
        {
            Line objLine = new Line();
            objLine.CalculateLGStraightLine(new PointF(objLineGaugeRecord.fMeasureCenterX, objLineGaugeRecord.fMeasureCenterY), objLineGaugeRecord.fMeasureAngle);

            Line objLine2 = new Line();
            objLine2.CalculateLGStraightLine(new PointF(m_LineGauge.ref_GaugeCenterX, m_LineGauge.ref_GaugeCenterY), m_LineGauge.ref_GaugeAngle);

            PointF pCross = Line.GetCrossPoint(objLine, objLine2);

            switch (intROIIndex)
            {
                case 0://Top
                    if (intLineGaugeIndex == 1)
                    {
                        pCross.X -= m_arrPointGaugeOffset[0];
                        if ((pCross.X + 3) >= (m_LineGauge.ref_GaugeCenterX + (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF((m_LineGauge.ref_GaugeCenterX + (m_LineGauge.ref_GaugeLength / 2)) - 3, pCross.Y);
                    }
                    else if (intLineGaugeIndex == 3)
                    {
                        pCross.X += m_arrPointGaugeOffset[0];
                        if ((pCross.X - 3) <= (m_LineGauge.ref_GaugeCenterX - (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF((m_LineGauge.ref_GaugeCenterX - (m_LineGauge.ref_GaugeLength / 2)) + 3, pCross.Y);
                    }
                    break;
                case 1://Right
                    if (intLineGaugeIndex == 2)
                    {
                        pCross.Y -= m_arrPointGaugeOffset[1];
                        if ((pCross.Y + 3) >= (m_LineGauge.ref_GaugeCenterY + (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF(pCross.X, (m_LineGauge.ref_GaugeCenterY + (m_LineGauge.ref_GaugeLength / 2)) - 3);
                    }
                    else if (intLineGaugeIndex == 0)
                    {
                        pCross.Y += m_arrPointGaugeOffset[1];
                        if ((pCross.Y - 3) <= (m_LineGauge.ref_GaugeCenterY - (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF(pCross.X, (m_LineGauge.ref_GaugeCenterY - (m_LineGauge.ref_GaugeLength / 2)) + 3);
                    }
                    break;
                case 2://Bottom
                    if (intLineGaugeIndex == 1)
                    {
                        pCross.X -= m_arrPointGaugeOffset[2];
                        if ((pCross.X + 3) >= (m_LineGauge.ref_GaugeCenterX + (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF((m_LineGauge.ref_GaugeCenterX + (m_LineGauge.ref_GaugeLength / 2)) - 3, pCross.Y);
                    }
                    else if (intLineGaugeIndex == 3)
                    {
                        pCross.X += m_arrPointGaugeOffset[2];
                        if ((pCross.X - 3) <= (m_LineGauge.ref_GaugeCenterX - (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF((m_LineGauge.ref_GaugeCenterX - (m_LineGauge.ref_GaugeLength / 2)) + 3, pCross.Y);
                    }
                    break;
                case 3://Left
                    if (intLineGaugeIndex == 2)
                    {
                        pCross.Y -= m_arrPointGaugeOffset[3];
                        if ((pCross.Y + 3) >= (m_LineGauge.ref_GaugeCenterY + (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF(pCross.X, (m_LineGauge.ref_GaugeCenterY + (m_LineGauge.ref_GaugeLength / 2)) - 3);
                    }
                    else if (intLineGaugeIndex == 0)
                    {
                        pCross.Y += m_arrPointGaugeOffset[3];
                        if ((pCross.Y - 3) <= (m_LineGauge.ref_GaugeCenterY - (m_LineGauge.ref_GaugeLength / 2)))
                            pCross = new PointF(pCross.X, (m_LineGauge.ref_GaugeCenterY - (m_LineGauge.ref_GaugeLength / 2)) + 3);
                    }
                    break;
            }

            m_arrPointGauge[intROIIndex][intPointGaugeIndex].SetGaugePlacementCenter(pCross.X, pCross.Y);

            m_arrPointGauge[intROIIndex][intPointGaugeIndex].ref_GaugeTolerance = m_LineGauge.ref_GaugeTolerance;

            m_arrPointGauge[intROIIndex][intPointGaugeIndex].ref_GaugeAngle = m_LineGauge.ref_GaugeAngle + 90;

            m_arrPointGauge[intROIIndex][intPointGaugeIndex].ref_GaugeMinAmplitude = m_LineGauge.ref_GaugeMinAmplitude;
            m_arrPointGauge[intROIIndex][intPointGaugeIndex].ref_GaugeMinArea = m_LineGauge.ref_GaugeMinArea;
            m_arrPointGauge[intROIIndex][intPointGaugeIndex].ref_GaugeFilter = m_LineGauge.ref_GaugeFilter;
            m_arrPointGauge[intROIIndex][intPointGaugeIndex].ref_GaugeThickness = 6;
            m_arrPointGauge[intROIIndex][intPointGaugeIndex].ref_GaugeThreshold = m_LineGauge.ref_GaugeThreshold;
            m_arrPointGauge[intROIIndex][intPointGaugeIndex].ref_GaugeTransChoice = m_LineGauge.ref_GaugeTransChoice;
            m_arrPointGauge[intROIIndex][intPointGaugeIndex].ref_GaugeTransType = m_LineGauge.ref_GaugeTransType;


            //objROI.ref_ROI.TopParent.Save("D:\\TS\\TopParent.bmp");
            //m_arrPointGauge[intROIIndex][intPointGaugeIndex].ref_objPointGauge.Save("D:\\TS\\PointGauge.CAL");
            m_arrPointGauge[intROIIndex][intPointGaugeIndex].Measure(objROI);
        }
        private LineGaugeRecord GetLineGaugeRecord(LGauge objLineGuage, PointF fMeasurePoint, float fMeasureAngle)
        {
            LineGaugeRecord objLineGaugeRecord = new LineGaugeRecord();
            objLineGaugeRecord.bUseSubGauge = false;
            objLineGaugeRecord.bUsePointGauge = true;
            objLineGaugeRecord.fMeasureAngle = fMeasureAngle;
            objLineGaugeRecord.fMeasureCenterX = fMeasurePoint.X;
            objLineGaugeRecord.fMeasureCenterY = fMeasurePoint.Y;
            objLineGaugeRecord.fMeasureScore = 100f;

            objLineGaugeRecord.fSetTolerance = objLineGuage.ref_GaugeTolerance;
            objLineGaugeRecord.fSetLength = objLineGuage.ref_GaugeLength;
            objLineGaugeRecord.fSetCenterX = objLineGuage.ref_GaugeCenterX;
            objLineGaugeRecord.fSetCenterY = objLineGuage.ref_GaugeCenterY;
            objLineGaugeRecord.fSetAngle = objLineGuage.ref_GaugeAngle;
            objLineGaugeRecord.intSetMinAmp = (int)objLineGuage.ref_GaugeMinAmplitude;
            objLineGaugeRecord.intSetMinArea = (int)objLineGuage.ref_GaugeMinArea;
            objLineGaugeRecord.intSetSmoothing = (int)objLineGuage.ref_GaugeFilter;
            objLineGaugeRecord.intSetThreshold = (int)objLineGuage.ref_GaugeThreshold;
            objLineGaugeRecord.intSetTransitionChoice = objLineGuage.ref_GaugeTransChoice.GetHashCode();
            objLineGaugeRecord.intSetTransitionType = objLineGuage.ref_GaugeTransType.GetHashCode();

            objLineGaugeRecord.objLine.CalculateLGStraightLine(fMeasurePoint, fMeasureAngle);

            return objLineGaugeRecord;
        }

        private bool AddPointGauge(List<List<ROI>> arrROI, List<int> arrRightBottomIndex, List<int> arrLeftTopIndex, int intSelectedIndex,
            ref List<int> arrNewIndex, int intDirection, int intLeftTopIndex, int intRightBottomIndex)
        {
            bool blnAddPointGauge = false;
            if (m_arrReCheckUsingPointGaugeIfFail[intDirection])
            {
                if ((arrRightBottomIndex[intSelectedIndex] < m_arrLineGaugeRecord[intRightBottomIndex].Count) && (arrLeftTopIndex[intSelectedIndex] < m_arrLineGaugeRecord[intLeftTopIndex].Count))
                {
                    for (int a = 0; a < 2; a++)
                    {
                        if (a == 0)
                        {
                            //Check Top Right
                            SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[intDirection], m_intVisionIndex)][intDirection],
                            m_arrLineGaugeRecord[intRightBottomIndex][arrRightBottomIndex[intSelectedIndex]],
                            m_arrLineGauge[intDirection],
                            intDirection,
                            intRightBottomIndex,
                            a);


                            if (m_arrPointGauge[intDirection][a].ref_intMeasuredPointCount > 0)
                            {
                                m_arrLineGaugeRecord[intDirection].Add(GetLineGaugeRecord(m_arrLineGauge[intDirection],
                                                                               m_arrPointGauge[intDirection][a].GetMeasurePoint(),
                                                                               m_arrLineGaugeRecord[intRightBottomIndex][arrRightBottomIndex[intSelectedIndex]].fMeasureAngle + 90));
                                blnAddPointGauge = true;
                                arrNewIndex.Add(m_arrLineGaugeRecord[intDirection].Count - 1);
                            }
                        }
                        else if (a == 1)
                        {
                            //Check Top Left
                            SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[intDirection], m_intVisionIndex)][intDirection],
                                m_arrLineGaugeRecord[intLeftTopIndex][arrLeftTopIndex[intSelectedIndex]],
                                m_arrLineGauge[intDirection],
                                intDirection,
                                intLeftTopIndex,
                                a);

                            if (m_arrPointGauge[intDirection][a].ref_intMeasuredPointCount > 0)
                            {
                                m_arrLineGaugeRecord[intDirection].Add(GetLineGaugeRecord(m_arrLineGauge[intDirection],
                                                                               m_arrPointGauge[intDirection][a].GetMeasurePoint(),
                                                                               m_arrLineGaugeRecord[intLeftTopIndex][arrLeftTopIndex[intSelectedIndex]].fMeasureAngle + 90));

                                blnAddPointGauge = true;
                                arrNewIndex.Add(m_arrLineGaugeRecord[intDirection].Count - 1);
                            }
                        }
                    }
                }
            }

            return blnAddPointGauge;
        }


        private bool AddPointGauge(List<List<ROI>> arrROI, List<int> arrTopIndex, List<int> arrRightIndex, List<int> arrBottomIndex, List<int> arrLeftIndex, int intSelectedIndex,
            ref List<int> arrTopNewIndex, ref List<int> arrRightNewIndex, ref List<int> arrBottomNewIndex, ref List<int> arrLeftNewIndex)
        {
            bool blnAddPointGauge = false;
            if (m_arrReCheckUsingPointGaugeIfFail[0])
            {
                if ((arrRightIndex[intSelectedIndex] < m_arrLineGaugeRecord[1].Count) && (arrLeftIndex[intSelectedIndex] < m_arrLineGaugeRecord[3].Count))
                {
                    for (int a = 0; a < 2; a++)
                    {
                        if (a == 0)
                        {

                            //Check Top Right
                            SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[0], m_intVisionIndex)][0],
                            m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]],
                            m_arrLineGauge[0],
                            0,
                            1,
                            a);


                            if (m_arrPointGauge[0][a].ref_intMeasuredPointCount > 0)
                            {
                                m_arrLineGaugeRecord[0].Add(GetLineGaugeRecord(m_arrLineGauge[0],
                                                                               m_arrPointGauge[0][a].GetMeasurePoint(),
                                                                               m_arrLineGaugeRecord[1][arrRightIndex[intSelectedIndex]].fMeasureAngle + 90));
                                blnAddPointGauge = true;
                                arrTopNewIndex.Add(m_arrLineGaugeRecord[0].Count - 1);
                            }
                        }
                        else if (a == 1)
                        {
                            //Check Top Left
                            SetParameterToPointGauge(arrROI[ImageDrawing.GetArrayImageIndex(m_arrGaugeImageNo[0], m_intVisionIndex)][0],
                                m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]],
                                m_arrLineGauge[0],
                                0,
                                3,
                                a);

                            if (m_arrPointGauge[0][a].ref_intMeasuredPointCount > 0)
                            {
                                m_arrLineGaugeRecord[0].Add(GetLineGaugeRecord(m_arrLineGauge[0],
                                                                               m_arrPointGauge[0][a].GetMeasurePoint(),
                                                                               m_arrLineGaugeRecord[3][arrLeftIndex[intSelectedIndex]].fMeasureAngle + 90));

                                blnAddPointGauge = true;
                                arrTopNewIndex.Add(m_arrLineGaugeRecord[0].Count - 1);
                            }
                        }
                    }
                }
            }

            return blnAddPointGauge;
        }
    }
}
