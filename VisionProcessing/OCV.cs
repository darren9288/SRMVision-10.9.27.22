using System;
using System.Collections;
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
    public class OCV
    {
        #region Private Variables

        private EOCV m_Ocv = new EOCV();
        private EOCV m_BinaOcv = new EOCV();
        private EOCVChar m_OcvChar = new EOCVChar();
        private EOCVText m_OcvText = new EOCVText();
        private ECodedImage m_blobs = new ECodedImage();
        private ECodedImage m_DefectBlobs = new ECodedImage();
        private EListItem m_ListEasyObject;
        private EBW8 m_bw = new EBW8();
        //private Pen m_penGreen = new Pen(Color.FromArgb(0, 255, 0));
        //private Pen m_penGreenText = new Pen(Color.FromArgb(0, 255, 0), (float)2);
        //private Pen m_penGreenChar = new Pen(Color.FromArgb(0, 255, 0));
        //private Pen m_penRedChar = new Pen(Color.Red);
        //private Pen m_penWhite = new Pen(Color.White);
        //private Pen m_penBlack = new Pen(Color.Black);
        private ERGBColor m_colorRed = new ERGBColor(Color.Red.R, Color.Red.G, Color.Red.B);
        private ERGBColor m_colorGreen = new ERGBColor(Color.Green.R, Color.Green.G, Color.Green.B);
        private ERGBColor m_colorGreenText = new ERGBColor(Color.FromArgb(0, 255, 0).R, Color.FromArgb(0, 255, 0).G, Color.FromArgb(0, 255, 0).B);

        private Font m_Font = new Font("Verdana", 10);
        private Color[] m_colorList = { Color.Aqua, Color.Lime, Color.DeepPink, Color.OrangeRed, Color.Aquamarine, Color.Gold, Color.Fuchsia, Color.MediumSpringGreen, Color.Blue, Color.Violet };
        private TrackLog m_objTrackLog = new TrackLog();

        private bool[] m_blnTextFail = new bool[10];
        private bool[] m_blnCharFail = new bool[100];
        private bool[] m_blnDefectFail = new bool[0];
        private int[] m_intTextDiagnostic = new int[10];
        private int[] m_intCharDiagnostic = new int[100];
        private int[] m_intMatchState;
        private int[] m_intTemplateMatchCount;
        private int[] m_intSampleFailResult;    //0: Pass, 1:Fail with condition, 2:Fail
        private int[] m_intScoreMode;           //0: Gradient Mode, 1: Binarized Mode
        
        private int m_intCharsCount = 0;
        private int m_intMarkFailType = 0;
        private int m_intMarkFailMask = 0;
        private int m_intStatisticCount = 0;
        private int m_intThresholdValue;
        private int m_intMinArea;
        private int m_intMaxArea;
        private int m_intFailMask;
        private int m_intAllLocationScore;
        private int m_intAllArea;
        private int m_intAllCorrelation;
        private int m_intTextShiftLimit;
        private int m_intUncheckAreaTop;
        private int m_intUncheckAreaBottom;
        private int m_intUncheckAreaLeft;
        private int m_intUncheckAreaRight;
        private int m_intAllTextShiftX;
        private int m_intAllTextShiftY;
        private int m_intAllCharShiftX;
        private int m_intAllCharShiftY;
        private int m_intToleranceSize;
        private int m_intGroupMinArea;
        private int m_intExtraMinArea;
        private float m_fAreaScoreFactor = 2;
        private float m_fToleStdDev = 20;
        private float m_fSampleStdDev = 0;

        private bool m_blnInpectBlobsON = true;
        private bool m_blnNewInspection = false;

        private float m_fLocationFailScore = 100;
        private float m_fAreaFailScore = 100;
        private float m_fCorrelationFailScore = 100;
        private float m_fLocationFailScore2 = 100;
        private float m_fAreaFailScore2 = 100;
        private float m_fCorrelationFailScore2 = 100;

        private ImageDrawing m_objLearnImage = new ImageDrawing();
        private ImageDrawing m_objSampleImage = new ImageDrawing();
        private ROI m_objTemplateTextROI = new ROI();
        private ROI m_objSubtractTextROI = new ROI();

        private string m_strErrorMessage = "";

        private List<SampleOCV> m_arrSampleOCV = new List<SampleOCV>();
        private ArrayList m_arrBlobsFeatures = new ArrayList();
        private ArrayList m_arrStatistics = new ArrayList();
        private ArrayList m_arrCharSetting = new ArrayList();
        private ArrayList m_arrCharNo = new ArrayList();
        private BlobsFeatures m_stcBlobsFeatures = new BlobsFeatures();
        private StatisticInfo m_stcStatisticInfo = new StatisticInfo();

        struct BlobsFeatures
        {
            public int intObjNumber;
            public int intArea;
            public float fWidth;
            public float fHeight;
            public float fAngle;
            public float fCenterX;
            public float fCenterY;
            public int intOcvMatchNumber;
            public bool blnMatch;
        }

        struct StatisticInfo
        {
            public int intBgAreaMin;
            public int intBgAreaMax;
            public float fBgAreaFac;
            public int intFgAreaMin;
            public int intFgAreaMax;
            public float fFgAreaFac;
            public float fBgSumMin;
            public float fBgSumMax;
            public float fBgSumFac;
            public float fFgSumMin;
            public float fFgSumMax;
            public float fFgSumFac;
        }

        struct CharFeatures
        {
            public int intCenterX;
            public int intCenterY;
            public int intStartX;
            public int intStartY;
            public int intEndX;
            public int intEndY;
            public int intMax;
            public int intMin;
            public ArrayList arrCenterX;
        }

        struct SampleOCV
        {
            public int intCenterX;
            public int intCenterY;
            public int intStartX;
            public int intStartY;
            public int intEndX;
            public int intEndY;
            public int intWidth;
            public int intHeight;
        }

        #endregion

        #region OCV Properties

        public float ref_fLocationFailScore { get { return m_fLocationFailScore; } set { m_fLocationFailScore = value; } }
        public float ref_fAreaFailScore { get { return m_fAreaFailScore; } set { m_fAreaFailScore = value; } }
        public float ref_fCorrelationFailScore { get { return m_fCorrelationFailScore; } set { m_fCorrelationFailScore = value; } }
        public float ref_fLocationFailScore2 { get { return m_fLocationFailScore2; } set { m_fLocationFailScore2 = value; } }
        public float ref_fAreaFailScore2 { get { return m_fAreaFailScore2; } set { m_fAreaFailScore2 = value; } }
        public float ref_fCorrelationFailScore2 { get { return m_fCorrelationFailScore2; } set { m_fCorrelationFailScore2 = value; } }

        public bool ref_blnInpectBlobsON { get { return m_blnInpectBlobsON; } set { m_blnInpectBlobsON = value; } }
        public int ref_intCharsCount { get { return m_intCharsCount; } set { m_intCharsCount = value; } }
        public int ref_intFailMask { get { return m_intFailMask; } set { m_intFailMask = value; } }
        public int ref_intAllLocationScore { get { return m_intAllLocationScore; } set { m_intAllLocationScore = value; } }
        public int ref_intAllArea { get { return m_intAllArea; } set { m_intAllArea = value; } }
        public int ref_intAllCorrelation { get { return m_intAllCorrelation; } set { m_intAllCorrelation = value; } }
        public int ref_intTextShiftLimit { get { return m_intTextShiftLimit; } set { m_intTextShiftLimit = value; } }
        public int ref_intUncheckAreaTop { get { return m_intUncheckAreaTop; } set { m_intUncheckAreaTop = value; } }
        public int ref_intUncheckAreaBottom { get { return m_intUncheckAreaBottom; } set { m_intUncheckAreaBottom = value; } }
        public int ref_intUncheckAreaLeft { get { return m_intUncheckAreaLeft; } set { m_intUncheckAreaLeft = value; } }
        public int ref_intUncheckAreaRight { get { return m_intUncheckAreaRight; } set { m_intUncheckAreaRight = value; } }
        public int ref_intAllTextShiftX { get { return m_intAllTextShiftX; } set { m_intAllTextShiftX = value; } }
        public int ref_intAllTextShiftY { get { return m_intAllTextShiftY; } set { m_intAllTextShiftY = value; } }
        public int ref_intAllCharShiftX { get { return m_intAllCharShiftX; } set { m_intAllCharShiftX = value; } }
        public int ref_intAllCharShiftY { get { return m_intAllCharShiftY; } set { m_intAllCharShiftY = value; } }
        public int ref_intToleranceSize { get { return m_intToleranceSize; } set { m_intToleranceSize = value; } }
        public int ref_intGroupMinArea { get { return m_intGroupMinArea; } set { m_intGroupMinArea = value; } }
        public int ref_intExtraMinArea { get { return m_intExtraMinArea; } set { m_intExtraMinArea = value; } }
        public float ref_fAreaScoreFactor { get { return m_fAreaScoreFactor; } set { m_fAreaScoreFactor = value; } }
        public int ref_intThresholdValue { get { return m_intThresholdValue; } set { m_intThresholdValue = value; } }
        public int ref_intMinArea { get { return m_intMinArea; } set { m_intMinArea = value; } }
        public int ref_intMaxArea { get { return m_intMaxArea; } set { m_intMaxArea = value; } }
        public int ref_intMarkFailMask { get { return m_intMarkFailMask; } set { m_intMarkFailMask = value; } }
        public int ref_intMarkFailType { get { return m_intMarkFailType; } set { m_intMarkFailType = value; } }

        public int ref_intTemplateTextStartX { get { return m_objTemplateTextROI.ref_ROIPositionX; } set { m_objTemplateTextROI.ref_ROIPositionX = value; } }
        public int ref_intTemplateTextStartY { get { return m_objTemplateTextROI.ref_ROIPositionY; } set { m_objTemplateTextROI.ref_ROIPositionY = value; } }
        public int ref_intTemplateTextWidth { get { return m_objTemplateTextROI.ref_ROIWidth; } set { m_objTemplateTextROI.ref_ROIWidth = value; } }
        public int ref_intTemplateTextHeight { get { return m_objTemplateTextROI.ref_ROIHeight; } set { m_objTemplateTextROI.ref_ROIHeight = value; } }

        public string ref_strErrorMessage { get { return m_strErrorMessage; } }

        #endregion

        #region EOCVChar Properties
#if (Debug_2_12 || Release_2_12)
        public float ref_CharTempLocationScore { get { return m_OcvChar.TemplateLocationScore; } set { m_OcvChar.TemplateLocationScore = value; } }
        public int ref_CharTempBackgroundArea { get { return (int)m_OcvChar.TemplateBackgroundArea; } set { m_OcvChar.TemplateBackgroundArea = (uint)value; } }
        public int ref_CharTempForegroundArea { get { return (int)m_OcvChar.TemplateForegroundArea; } set { m_OcvChar.TemplateForegroundArea = (uint)value; } }
        public float ref_CharTempBackgroundSum { get { return m_OcvChar.TemplateBackgroundSum; } set { m_OcvChar.TemplateBackgroundSum = value; } }
        public float ref_CharTempForegroundSum { get { return m_OcvChar.TemplateForegroundSum; } set { m_OcvChar.TemplateForegroundSum = value; } }

        public float ref_CharSampLocationScore { get { return m_OcvChar.SampleLocationScore; } set { m_OcvChar.SampleLocationScore = value; } }
        public int ref_CharSampBackgroundArea { get { return (int)m_OcvChar.SampleBackgroundArea; } set { m_OcvChar.SampleBackgroundArea = (uint)value; } }
        public int ref_CharSampForegroundArea { get { return (int)m_OcvChar.SampleForegroundArea; } set { m_OcvChar.SampleForegroundArea = (uint)value; } }
        public float ref_CharSampBackgroundSum { get { return m_OcvChar.SampleBackgroundSum; } set { m_OcvChar.SampleBackgroundSum = value; } }
        public float ref_CharSampForegroundSum { get { return m_OcvChar.SampleForegroundSum; } set { m_OcvChar.SampleForegroundSum = value; } }
        public float ref_CharSampCorrelation { get { return m_OcvChar.Correlation; } set { m_OcvChar.Correlation = value; } }

        public float ref_CharToleLocationScore { get { return m_OcvChar.LocationScoreTolerance; } set { m_OcvChar.LocationScoreTolerance = value; } }
        public int ref_CharToleBackgroundArea { get { return (int)m_OcvChar.BackgroundAreaTolerance; } set { m_OcvChar.BackgroundAreaTolerance = (uint)value; } }
        public int ref_CharToleForegroundArea { get { return (int)m_OcvChar.ForegroundAreaTolerance; } set { m_OcvChar.ForegroundAreaTolerance = (uint)value; } }
        public float ref_CharToleBackgroundSum { get { return m_OcvChar.BackgroundSumTolerance; } set { m_OcvChar.BackgroundSumTolerance = value; } }
        public float ref_CharToleForegroundSum { get { return m_OcvChar.ForegroundSumTolerance; } set { m_OcvChar.ForegroundSumTolerance = value; } }
        public float ref_CharToleCorrelation { get { return m_OcvChar.CorrelationTolerance; } set { m_OcvChar.CorrelationTolerance = value; } }

        public float ref_CharShiftXBias { get { return m_OcvChar.ShiftXBias; } set { m_OcvChar.ShiftXBias = value; } }
        public float ref_CharShiftXTolerance { get { return m_OcvChar.ShiftXTolerance; } set { m_OcvChar.ShiftXTolerance = value; } }
        public int ref_CharShiftXStride { get { return (int)m_OcvChar.ShiftXStride; } set { m_OcvChar.ShiftXStride = (uint)value; } }
        public float ref_CharShiftXMea { get { return m_OcvChar.ShiftX; } set { m_OcvChar.ShiftX = value; } }

        public float ref_CharShiftYBias { get { return m_OcvChar.ShiftYBias; } set { m_OcvChar.ShiftYBias = value; } }
        public float ref_CharShiftYTolerance { get { return m_OcvChar.ShiftYTolerance; } set { m_OcvChar.ShiftYTolerance = value; } }
        public int ref_CharShiftYStride { get { return (int)m_OcvChar.ShiftYStride; } set { m_OcvChar.ShiftYStride = (uint)value; } }
        public float ref_CharShiftYMea { get { return m_OcvChar.ShiftY; } set { m_OcvChar.ShiftY = value; } }

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
        public float ref_CharTempLocationScore { get { return m_OcvChar.TemplateLocationScore; } set { m_OcvChar.TemplateLocationScore = value; } }
        public int ref_CharTempBackgroundArea { get { return m_OcvChar.TemplateBackgroundArea; } set { m_OcvChar.TemplateBackgroundArea = value; } }
        public int ref_CharTempForegroundArea { get { return m_OcvChar.TemplateForegroundArea; } set { m_OcvChar.TemplateForegroundArea = value; } }
        public float ref_CharTempBackgroundSum { get { return m_OcvChar.TemplateBackgroundSum; } set { m_OcvChar.TemplateBackgroundSum = value; } }
        public float ref_CharTempForegroundSum { get { return m_OcvChar.TemplateForegroundSum; } set { m_OcvChar.TemplateForegroundSum = value; } }

        public float ref_CharSampLocationScore { get { return m_OcvChar.SampleLocationScore; } set { m_OcvChar.SampleLocationScore = value; } }
        public int ref_CharSampBackgroundArea { get { return m_OcvChar.SampleBackgroundArea; } set { m_OcvChar.SampleBackgroundArea = value; } }
        public int ref_CharSampForegroundArea { get { return m_OcvChar.SampleForegroundArea; } set { m_OcvChar.SampleForegroundArea = value; } }
        public float ref_CharSampBackgroundSum { get { return m_OcvChar.SampleBackgroundSum; } set { m_OcvChar.SampleBackgroundSum = value; } }
        public float ref_CharSampForegroundSum { get { return m_OcvChar.SampleForegroundSum; } set { m_OcvChar.SampleForegroundSum = value; } }
        public float ref_CharSampCorrelation { get { return m_OcvChar.Correlation; } set { m_OcvChar.Correlation = value; } }

        public float ref_CharToleLocationScore { get { return m_OcvChar.LocationScoreTolerance; } set { m_OcvChar.LocationScoreTolerance = value; } }
        public int ref_CharToleBackgroundArea { get { return m_OcvChar.BackgroundAreaTolerance; } set { m_OcvChar.BackgroundAreaTolerance = value; } }
        public int ref_CharToleForegroundArea { get { return m_OcvChar.ForegroundAreaTolerance; } set { m_OcvChar.ForegroundAreaTolerance = value; } }
        public float ref_CharToleBackgroundSum { get { return m_OcvChar.BackgroundSumTolerance; } set { m_OcvChar.BackgroundSumTolerance = value; } }
        public float ref_CharToleForegroundSum { get { return m_OcvChar.ForegroundSumTolerance; } set { m_OcvChar.ForegroundSumTolerance = value; } }
        public float ref_CharToleCorrelation { get { return m_OcvChar.CorrelationTolerance; } set { m_OcvChar.CorrelationTolerance = value; } }

        public float ref_CharShiftXBias { get { return m_OcvChar.ShiftXBias; } set { m_OcvChar.ShiftXBias = value; } }
        public float ref_CharShiftXTolerance { get { return m_OcvChar.ShiftXTolerance; } set { m_OcvChar.ShiftXTolerance = value; } }
        public int ref_CharShiftXStride { get { return m_OcvChar.ShiftXStride; } set { m_OcvChar.ShiftXStride = value; } }
        public float ref_CharShiftXMea { get { return m_OcvChar.ShiftX; } set { m_OcvChar.ShiftX = value; } }

        public float ref_CharShiftYBias { get { return m_OcvChar.ShiftYBias; } set { m_OcvChar.ShiftYBias = value; } }
        public float ref_CharShiftYTolerance { get { return m_OcvChar.ShiftYTolerance; } set { m_OcvChar.ShiftYTolerance = value; } }
        public int ref_CharShiftYStride { get { return m_OcvChar.ShiftYStride; } set { m_OcvChar.ShiftYStride = value; } }
        public float ref_CharShiftYMea { get { return m_OcvChar.ShiftY; } set { m_OcvChar.ShiftY = value; } }

#endif

        #endregion

        #region EOCVText Properties

        public float ref_fToleStdDev { get { return m_fToleStdDev; } set { m_fToleStdDev = value; } }
        public float ref_fSampleStdDev { get { return m_fSampleStdDev; } set { m_fSampleStdDev = value; } }
#if (Debug_2_12 || Release_2_12)
        public float ref_TextTempLocationScore { get { return m_OcvText.TemplateLocationScore; } set { m_OcvText.TemplateLocationScore = value; } }
        public int ref_TextTempBackgroundArea { get { return (int)m_OcvText.TemplateBackgroundArea; } set { m_OcvText.TemplateBackgroundArea = (uint)value; } }
        public int ref_TextTempForegroundArea { get { return (int)m_OcvText.TemplateForegroundArea; } set { m_OcvText.TemplateForegroundArea = (uint)value; } }
        public float ref_TextTempBackgroundSum { get { return m_OcvText.TemplateBackgroundSum; } set { m_OcvText.TemplateBackgroundSum = value; } }
        public float ref_TextTempForegroundSum { get { return m_OcvText.TemplateForegroundSum; } set { m_OcvText.TemplateForegroundSum = value; } }

        public float ref_TextSampLocationScore { get { return m_OcvText.SampleLocationScore; } set { m_OcvText.SampleLocationScore = value; } }
        public int ref_TextSampBackgroundArea { get { return (int)m_OcvText.SampleBackgroundArea; } set { m_OcvText.SampleBackgroundArea = (uint)value; } }
        public int ref_TextSampForegroundArea { get { return (int)m_OcvText.SampleForegroundArea; } set { m_OcvText.SampleForegroundArea = (uint)value; } }
        public float ref_TextSampBackgroundSum { get { return m_OcvText.SampleBackgroundSum; } set { m_OcvText.SampleBackgroundSum = value; } }
        public float ref_TextSampForegroundSum { get { return m_OcvText.SampleForegroundSum; } set { m_OcvText.SampleForegroundSum = value; } }
        public float ref_TextSampCorrelation { get { return m_OcvText.Correlation; } set { m_OcvText.Correlation = value; } }

        public float ref_TextToleLocationScore { get { return m_OcvText.LocationScoreTolerance; } set { m_OcvText.LocationScoreTolerance = value; } }
        public int ref_TextToleBackgroundArea { get { return (int)m_OcvText.BackgroundAreaTolerance; } set { m_OcvText.BackgroundAreaTolerance = (uint)value; } }
        public int ref_TextToleForegroundArea { get { return (int)m_OcvText.ForegroundAreaTolerance; } set { m_OcvText.ForegroundAreaTolerance = (uint)value; } }
        public float ref_TextToleBackgroundSum { get { return m_OcvText.BackgroundSumTolerance; } set { m_OcvText.BackgroundSumTolerance = value; } }
        public float ref_TextToleForegroundSum { get { return m_OcvText.ForegroundSumTolerance; } set { m_OcvText.ForegroundSumTolerance = value; } }
        public float ref_TextToleCorrelation { get { return m_OcvText.CorrelationTolerance; } set { m_OcvText.CorrelationTolerance = value; } }

        public float ref_TextShiftXBias { get { return m_OcvText.ShiftXBias; } set { m_OcvText.ShiftXBias = value; } }
        public float ref_TextShiftXTolerance { get { return m_OcvText.ShiftXTolerance; } set { m_OcvText.ShiftXTolerance = value; } }
        public int ref_TextShiftXStride { get { return (int)m_OcvText.ShiftXStride; } set { m_OcvText.ShiftXStride = (uint)value; } }
        public float ref_TextShiftXMea { get { return m_OcvText.ShiftX; } set { m_OcvText.ShiftX = value; } }

        public float ref_TextShiftYBias { get { return m_OcvText.ShiftYBias; } set { m_OcvText.ShiftYBias = value; } }
        public float ref_TextShiftYTolerance { get { return m_OcvText.ShiftYTolerance; } set { m_OcvText.ShiftYTolerance = value; } }
        public int ref_TextShiftYStride { get { return (int)m_OcvText.ShiftYStride; } set { m_OcvText.ShiftYStride = (uint)value; } }
        public float ref_TextShiftYMea { get { return m_OcvText.ShiftY; } set { m_OcvText.ShiftY = value; } }

        public float ref_TextSkewBias { get { return m_OcvText.SkewBias; } set { m_OcvText.SkewBias = value; } }
        public float ref_TextSkewTolerance { get { return m_OcvText.SkewTolerance; } set { m_OcvText.SkewTolerance = value; } }
        public int ref_TextSkewCount { get { return (int)m_OcvText.SkewCount; } set { m_OcvText.SkewCount = (uint)value; } }
        public float ref_TextSkewMea { get { return m_OcvText.Skew; } set { m_OcvText.Skew = value; } }

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
        public float ref_TextTempLocationScore { get { return m_OcvText.TemplateLocationScore; } set { m_OcvText.TemplateLocationScore = value; } }
        public int ref_TextTempBackgroundArea { get { return m_OcvText.TemplateBackgroundArea; } set { m_OcvText.TemplateBackgroundArea = value; } }
        public int ref_TextTempForegroundArea { get { return m_OcvText.TemplateForegroundArea; } set { m_OcvText.TemplateForegroundArea = value; } }
        public float ref_TextTempBackgroundSum { get { return m_OcvText.TemplateBackgroundSum; } set { m_OcvText.TemplateBackgroundSum = value; } }
        public float ref_TextTempForegroundSum { get { return m_OcvText.TemplateForegroundSum; } set { m_OcvText.TemplateForegroundSum = value; } }

        public float ref_TextSampLocationScore { get { return m_OcvText.SampleLocationScore; } set { m_OcvText.SampleLocationScore = value; } }
        public int ref_TextSampBackgroundArea { get { return m_OcvText.SampleBackgroundArea; } set { m_OcvText.SampleBackgroundArea = value; } }
        public int ref_TextSampForegroundArea { get { return m_OcvText.SampleForegroundArea; } set { m_OcvText.SampleForegroundArea = value; } }
        public float ref_TextSampBackgroundSum { get { return m_OcvText.SampleBackgroundSum; } set { m_OcvText.SampleBackgroundSum = value; } }
        public float ref_TextSampForegroundSum { get { return m_OcvText.SampleForegroundSum; } set { m_OcvText.SampleForegroundSum = value; } }
        public float ref_TextSampCorrelation { get { return m_OcvText.Correlation; } set { m_OcvText.Correlation = value; } }

        public float ref_TextToleLocationScore { get { return m_OcvText.LocationScoreTolerance; } set { m_OcvText.LocationScoreTolerance = value; } }
        public int ref_TextToleBackgroundArea { get { return m_OcvText.BackgroundAreaTolerance; } set { m_OcvText.BackgroundAreaTolerance = value; } }
        public int ref_TextToleForegroundArea { get { return m_OcvText.ForegroundAreaTolerance; } set { m_OcvText.ForegroundAreaTolerance = value; } }
        public float ref_TextToleBackgroundSum { get { return m_OcvText.BackgroundSumTolerance; } set { m_OcvText.BackgroundSumTolerance = value; } }
        public float ref_TextToleForegroundSum { get { return m_OcvText.ForegroundSumTolerance; } set { m_OcvText.ForegroundSumTolerance = value; } }
        public float ref_TextToleCorrelation { get { return m_OcvText.CorrelationTolerance; } set { m_OcvText.CorrelationTolerance = value; } }

        public float ref_TextShiftXBias { get { return m_OcvText.ShiftXBias; } set { m_OcvText.ShiftXBias = value; } }
        public float ref_TextShiftXTolerance { get { return m_OcvText.ShiftXTolerance; } set { m_OcvText.ShiftXTolerance = value; } }
        public int ref_TextShiftXStride { get { return m_OcvText.ShiftXStride; } set { m_OcvText.ShiftXStride = value; } }
        public float ref_TextShiftXMea { get { return m_OcvText.ShiftX; } set { m_OcvText.ShiftX = value; } }

        public float ref_TextShiftYBias { get { return m_OcvText.ShiftYBias; } set { m_OcvText.ShiftYBias = value; } }
        public float ref_TextShiftYTolerance { get { return m_OcvText.ShiftYTolerance; } set { m_OcvText.ShiftYTolerance = value; } }
        public int ref_TextShiftYStride { get { return m_OcvText.ShiftYStride; } set { m_OcvText.ShiftYStride = value; } }
        public float ref_TextShiftYMea { get { return m_OcvText.ShiftY; } set { m_OcvText.ShiftY = value; } }

        public float ref_TextSkewBias { get { return m_OcvText.SkewBias; } set { m_OcvText.SkewBias = value; } }
        public float ref_TextSkewTolerance { get { return m_OcvText.SkewTolerance; } set { m_OcvText.SkewTolerance = value; } }
        public int ref_TextSkewCount { get { return m_OcvText.SkewCount; } set { m_OcvText.SkewCount = value; } }
        public float ref_TextSkewMea { get { return m_OcvText.Skew; } set { m_OcvText.Skew = value; } }

#endif

        #endregion


        public OCV()
        {
        }

        public void AddCharNumber(int intValue)
        {
            m_arrCharNo.Add(intValue);
        }

        public void AddCharSetting(int intValue)
        {
            m_arrCharSetting.Add(intValue);
        }

        public bool AddManualStatistics()
        {
            // Statistics can only be calculated when there are 2 set of data. 
            // If statistic is zero, then add sample sample data as first data.
            // This will change the ocv template data same with sample data.
            if (m_intStatisticCount == 0 || m_arrStatistics.Count == 0)
            {
                // Make sure array is empty
                m_arrStatistics.Clear();

                int intNumTextChars = 0;
                int intNumTexts = (int)m_Ocv.NumTexts;
                int intCenterX, intCenterY;
#if (Debug_2_12 || Release_2_12)
                for (uint tx = 0; tx < intNumTexts; tx++)
                {
                    intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                    for (uint ch = 0; ch < intNumTextChars; ch++)
                    {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                for (int tx = 0; tx < intNumTexts; tx++)
                {
                    intNumTextChars = m_Ocv.GetNumTextChars(tx);
                    for (int ch = 0; ch < intNumTextChars; ch++)
                    {
#endif

                        intCenterX = intCenterY = 0;
                        m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                        m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                        m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                        m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        if (m_OcvChar.SampleBackgroundArea == 0)
                            return false;

                        m_stcStatisticInfo.intBgAreaMin = (int)m_OcvChar.SampleBackgroundArea;
                        m_stcStatisticInfo.intBgAreaMax = (int)m_OcvChar.SampleBackgroundArea;
                        m_stcStatisticInfo.intFgAreaMin = (int)m_OcvChar.SampleForegroundArea;
                        m_stcStatisticInfo.intFgAreaMax = (int)m_OcvChar.SampleForegroundArea;
                        m_stcStatisticInfo.fBgSumMin = m_OcvChar.SampleBackgroundSum;
                        m_stcStatisticInfo.fBgSumMax = m_OcvChar.SampleBackgroundSum;
                        m_stcStatisticInfo.fFgSumMin = m_OcvChar.SampleForegroundSum;
                        m_stcStatisticInfo.fFgSumMax = m_OcvChar.SampleForegroundSum;
                        m_stcStatisticInfo.fBgAreaFac = 1;
                        m_stcStatisticInfo.fFgAreaFac = 1;
                        m_stcStatisticInfo.fBgSumFac = 1;
                        m_stcStatisticInfo.fFgSumFac = 1;
                        m_arrStatistics.Add(m_stcStatisticInfo);
                    }
                }
                if (m_intStatisticCount == 0)
                    m_intStatisticCount++;
            }

            // Add current sample data to statistic
            if (m_intStatisticCount > 0)
            {
                int intNumTextChars = 0;
                int intNumTexts = (int)m_Ocv.NumTexts;
                int intCenterX, intCenterY;
                int intCharsCount = 0;
#if (Debug_2_12 || Release_2_12)
                for (uint tx = 0; tx < intNumTexts; tx++)
                {
                    intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                    for (uint ch = 0; ch < intNumTextChars; ch++)
                    {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                for (int tx = 0; tx < intNumTexts; tx++)
                {
                    intNumTextChars = m_Ocv.GetNumTextChars(tx);
                    for (int ch = 0; ch < intNumTextChars; ch++)
                    {
#endif

                        intCenterX = intCenterY = 0;
                        m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                        m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                        m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                        m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        if (m_OcvChar.SampleBackgroundArea == 0)
                            return false;

                        // Get maximum and minimum data from sample
                        StatisticInfo objStatisticInfo = (StatisticInfo)m_arrStatistics[intCharsCount];
                        if (m_OcvChar.SampleBackgroundArea < objStatisticInfo.intBgAreaMin)
                            objStatisticInfo.intBgAreaMin = (int)m_OcvChar.SampleBackgroundArea;

                        if (m_OcvChar.SampleBackgroundArea > objStatisticInfo.intBgAreaMax)
                            objStatisticInfo.intBgAreaMax = (int)m_OcvChar.SampleBackgroundArea;

                        if (m_OcvChar.SampleForegroundArea < objStatisticInfo.intFgAreaMin)
                            objStatisticInfo.intFgAreaMin = (int)m_OcvChar.SampleForegroundArea;

                        if (m_OcvChar.SampleForegroundArea > objStatisticInfo.intFgAreaMax)
                            objStatisticInfo.intFgAreaMax = (int)m_OcvChar.SampleForegroundArea;

                        if (m_OcvChar.SampleBackgroundSum < objStatisticInfo.fBgSumMin)
                            objStatisticInfo.fBgSumMin = m_OcvChar.SampleBackgroundSum;

                        if (m_OcvChar.SampleBackgroundSum > objStatisticInfo.fBgSumMax)
                            objStatisticInfo.fBgSumMax = m_OcvChar.SampleBackgroundSum;

                        if (m_OcvChar.SampleForegroundSum < objStatisticInfo.fFgSumMin)
                            objStatisticInfo.fFgSumMin = m_OcvChar.SampleForegroundSum;

                        if (m_OcvChar.SampleForegroundSum > objStatisticInfo.fFgSumMax)
                            objStatisticInfo.fFgSumMax = m_OcvChar.SampleForegroundSum;
#if (Debug_2_12 || Release_2_12)
                        // Define new template data based on sample data
                        m_OcvChar.TemplateBackgroundArea = (uint)(objStatisticInfo.intBgAreaMin + objStatisticInfo.intBgAreaMax) / 2;
                        m_OcvChar.TemplateForegroundArea = (uint)(objStatisticInfo.intFgAreaMin + objStatisticInfo.intFgAreaMax) / 2;
                        m_OcvChar.TemplateBackgroundSum = (objStatisticInfo.fBgSumMin + objStatisticInfo.fBgSumMax) / 2;
                        m_OcvChar.TemplateForegroundSum = (objStatisticInfo.fFgSumMin + objStatisticInfo.fFgSumMax) / 2;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        // Define new template data based on sample data
                        m_OcvChar.TemplateBackgroundArea = (objStatisticInfo.intBgAreaMin + objStatisticInfo.intBgAreaMax) / 2;
                        m_OcvChar.TemplateForegroundArea = (objStatisticInfo.intFgAreaMin + objStatisticInfo.intFgAreaMax) / 2;
                        m_OcvChar.TemplateBackgroundSum = (objStatisticInfo.fBgSumMin + objStatisticInfo.fBgSumMax) / 2;
                        m_OcvChar.TemplateForegroundSum = (objStatisticInfo.fFgSumMin + objStatisticInfo.fFgSumMax) / 2;
#endif

                        // Define new tolerance data based on new template data
                        SetValueToChars(m_OcvChar, (int)m_arrCharSetting[(int)m_arrCharNo[intCharsCount]], objStatisticInfo);

                        // Set new setting EOCVChar to ocv object
                        m_Ocv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        m_BinaOcv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                        intCharsCount++;
                    }
                }
                m_intStatisticCount++;
            }
            return true;
        }

        public void GetSampleOcv(int intCharNo)
        {
            SampleOCV objSampleOcv = new SampleOCV();

            int intOcvNumber = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    // find ocv char which match according to char no.
                    if ((int)m_arrCharNo[intCharNo] == intOcvNumber)
                    {
                        objSampleOcv.intStartX = objSampleOcv.intStartY = objSampleOcv.intEndX = objSampleOcv.intEndY = 0;
                        m_Ocv.GetTextCharPoint(tx, ch, out objSampleOcv.intStartX, out objSampleOcv.intStartY, -1, -1);
                        m_Ocv.GetTextCharPoint(tx, ch, out objSampleOcv.intEndX, out objSampleOcv.intEndY, 1, 1);
                        objSampleOcv.intWidth = objSampleOcv.intEndX - objSampleOcv.intStartX;
                        objSampleOcv.intHeight = objSampleOcv.intEndY - objSampleOcv.intStartY;
                        m_arrSampleOCV.Add(objSampleOcv);
                    }
                    intOcvNumber++;
                }
            }
        }

        public void GetBinarizeSampleOcv(int intCharNo)
        {
            SampleOCV objSampleOcv = new SampleOCV();

            int intOcvNumber = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    // find ocv char which match according to char no.
                    if ((int)m_arrCharNo[intCharNo] == intOcvNumber)
                    {
                        objSampleOcv.intStartX = objSampleOcv.intStartY = objSampleOcv.intEndX = objSampleOcv.intEndY = 0;
                        m_BinaOcv.GetTextCharPoint(tx, ch, out objSampleOcv.intStartX, out objSampleOcv.intStartY, -1, -1);
                        m_BinaOcv.GetTextCharPoint(tx, ch, out objSampleOcv.intEndX, out objSampleOcv.intEndY, 1, 1);
                        objSampleOcv.intWidth = objSampleOcv.intEndX - objSampleOcv.intStartX;
                        objSampleOcv.intHeight = objSampleOcv.intEndY - objSampleOcv.intStartY;
                        m_arrSampleOCV.Add(objSampleOcv);
                    }
                    intOcvNumber++;
                }
            }
        }

        public void AddStatistics()
        {
            m_Ocv.UpdateStatistics();
            if (m_Ocv.StatisticsCount == 1)
            {
                return;
            }

            ResetSampleCharsAndTexts(0, 0, 640, 480);
            m_Ocv.SelectSampleTexts(0, 0, 640, 480, ESelectionFlag.False);
            m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.False);

            m_Ocv.AdjustCharsQualityRanges(10, ESelectionFlag.Any);

            int intNumTextChars = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX, intCenterY;
            int intCharsCount = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    SetValueToChars(m_OcvChar, (int)m_arrCharSetting[Convert.ToInt32(m_arrCharNo[intCharsCount])]);
                    m_Ocv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                }
                intCharsCount++;
            }

        }

        public void AddTemplateImageStatistics(string strPath, ROI objTempSearchROI, ROI objTempTrainROI)
        {
            ImageDrawing objTemplateImage = new ImageDrawing();
            objTemplateImage.LoadImage(strPath);
            if (GetStatisticsCount() == 0)
            {
                objTempSearchROI.ref_ROI.Detach();
                objTempSearchROI.ref_ROI.Attach(objTemplateImage.ref_objMainImage);
                objTempTrainROI.ref_ROI.Detach();
                objTempTrainROI.ref_ROI.Attach(objTempSearchROI.ref_ROI);
#if (Debug_2_12 || Release_2_12)
                m_Ocv.Inspect(objTempTrainROI.ref_ROI, (uint)m_intThresholdValue);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                m_Ocv.Inspect(objTempTrainROI.ref_ROI, m_intThresholdValue);
#endif

                AddStatistics();
            }
        }

        public void BuildChars(int intCreationMode)
        {
            if (intCreationMode == 1)
            {
                m_Ocv.CreateTemplateChars(ESelectionFlag.Any, ECharCreationMode.Separate);
            }
            else if (intCreationMode == 2)
            {
                m_Ocv.CreateTemplateChars(ESelectionFlag.Any, ECharCreationMode.Overlap);
            }
        }

        public bool BuildObjects(ROI objROI, bool blnWhiteOnBlack)
        {
            DeleteChars();
            DeleteTexts();
            DeleteChars();
            DeleteTexts();

            if (objROI.ref_ROI.Width == 0)
            {
                m_strErrorMessage = "No ROI Loaded!";
                return false;
            }

            m_Ocv.DeleteTemplateObjects(ESelectionFlag.Any);
            m_blobs.RemoveAllObjects();
#if (Debug_2_12 || Release_2_12)
            // Set threshold value
            m_blobs.SetThreshold((uint)m_intThresholdValue);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Set threshold value
            m_blobs.SetThreshold(m_intThresholdValue);
#endif

            m_blobs.Connexity = EConnexity.Connexity4;

            // Set white black class
            if (blnWhiteOnBlack)
            {
                m_blobs.BlackClass = 0;
                m_blobs.WhiteClass = 1;
            }
            else
            {
                m_blobs.BlackClass = 1;
                m_blobs.WhiteClass = 0;
            }
            m_blobs.BuildObjects(objROI.ref_ROI);

            try
            {
                // Build and select objects
                m_blobs.AnalyseObjects(ELegacyFeature.Area, ELegacyFeature.ObjectNumber, ELegacyFeature.GravityCenterX, ELegacyFeature.GravityCenterY, ELegacyFeature.LimitCenterX, ELegacyFeature.LimitCenterY, ELegacyFeature.LimitWidth, ELegacyFeature.LimitHeight, ELegacyFeature.EllipseAngle);
                m_blobs.SelectObjectsUsingFeature(ELegacyFeature.Area, m_intMinArea, m_intMaxArea, ESelectOption.RemoveOutOfRange);
                m_blobs.SelectObjectsUsingPosition(objROI.ref_ROI, ESelectByPosition.RemoveBorder);
                m_blobs.SortObjectsUsingFeature(ELegacyFeature.ObjectNumber, ESortOption.Descending);

            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_objTrackLog.WriteLine("OCV BuildObjects : " + ex.ToString());
                m_strErrorMessage = ex.ToString();
                return false;
            }

            if (m_blobs.NumSelectedObjects == 0)
            {
                m_strErrorMessage = "OCV BuildObjects : No blobs object selected!";
                return false;
            }
            else
            {
                m_Ocv.CreateTemplateObjects(m_blobs, ESelectionFlag.True);
            }
            return true;
        }

        public void BuildTexts()
        {
            m_Ocv.CreateTemplateTexts(ESelectionFlag.True);
        }

        public void CheckData()
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                // Get OcvText
                m_Ocv.GetTextParameters(m_OcvText, tx);

                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                // Get OcvText
                m_Ocv.GetTextParameters(m_OcvText, tx);

                intNumTextChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    m_Ocv.GetTextCharParameters(m_OcvChar, tx, ch);
                    if (Math.Abs(m_OcvChar.TemplateLocationScore - m_OcvChar.SampleLocationScore) > m_OcvChar.LocationScoreTolerance)
                        return;
                    if (Math.Abs(m_OcvChar.TemplateBackgroundArea - m_OcvChar.SampleBackgroundArea) > m_OcvChar.BackgroundAreaTolerance)
                        return;
                    if (Math.Abs(m_OcvChar.TemplateBackgroundSum - m_OcvChar.SampleBackgroundSum) > m_OcvChar.BackgroundSumTolerance)
                        return;
                    if (Math.Abs(m_OcvChar.TemplateForegroundArea - m_OcvChar.SampleForegroundArea) > m_OcvChar.ForegroundAreaTolerance)
                        return;
                    if (Math.Abs(m_OcvChar.TemplateForegroundSum - m_OcvChar.SampleForegroundSum) > m_OcvChar.ForegroundSumTolerance)
                        return;

                    float correlation1 = m_OcvChar.Correlation;
                    float correlation2 = m_OcvChar.CorrelationTolerance;
                }
            }
        }

        private bool CheckIsInUncheckArea(ROI objROI, float fX, float fY)
        {
            // Check Top ROI Uncheck Area
            if ((fX >= 0) &&
                (fX <= (objROI.ref_ROIWidth)) &&
                (fY >= 0) &&
                (fY < (m_intUncheckAreaTop)))
            {
                return true;
            }

            // Check Bottom ROI Uncheck Area
            if ((fX >= 0) &&
                (fX <= (objROI.ref_ROIWidth)) &&
                (fY > objROI.ref_ROIHeight - m_intUncheckAreaBottom) &&
                (fY <= (objROI.ref_ROIHeight)))
            {
                return true;
            }

            // Check Left ROI Uncheck Area
            if ((fX >= 0) &&
                (fX < (m_intUncheckAreaLeft)) &&
                (fY >= 0) &&
                (fY <= (objROI.ref_ROIHeight)))
            {
                return true;
            }

            // Check Right ROI Uncheck Area
            if ((fX > objROI.ref_ROIWidth - m_intUncheckAreaRight) &&
                (fX <= (objROI.ref_ROIWidth)) &&
                (fY >= 0) &&
                (fY <= (objROI.ref_ROIHeight)))
            {
                return true;
            }

            return false;
        }

        private bool CheckOcvTextShifted(int intROIStartX, int intROIStartY, int intROIEndX, int intROIEndY)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intStartX, intStartY, intEndX, intEndY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                intStartX = intStartY = intEndX = intEndY = 0;

                // Start and End XY value off set from main Image
                m_Ocv.GetTextPoint(tx, out intStartX, out intStartY, -1, -1);
                m_Ocv.GetTextPoint(tx, out intEndX, out intEndY, 1, 1);

                // Make sure sample marks are not shifted out of Shift Limit
                if ((intStartX < (intROIStartX + m_intUncheckAreaLeft)) ||
                    (intStartY < (intROIStartY + m_intUncheckAreaTop)) ||
                    (intEndX > (intROIEndX - m_intUncheckAreaRight)) ||
                    (intEndY > (intROIEndY - m_intUncheckAreaBottom)))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckOcvCharDiagnostic(EOCVChar objOcvChar)
        {
            //if (Math.Abs(objOcvChar.TemplateLocationScore - objOcvChar.SampleLocationScore) > objOcvChar.LocationScoreTolerance)
            if (objOcvChar.TemplateLocationScore > (objOcvChar.SampleLocationScore + objOcvChar.LocationScoreTolerance))
                return false;

            if (Math.Abs(objOcvChar.TemplateBackgroundArea - objOcvChar.SampleBackgroundArea) > objOcvChar.BackgroundAreaTolerance)
                return false;

            if (Math.Abs(objOcvChar.TemplateForegroundArea - objOcvChar.SampleForegroundArea) > objOcvChar.ForegroundAreaTolerance)
                return false;

            if (Math.Abs(objOcvChar.TemplateBackgroundSum - objOcvChar.SampleBackgroundSum) > objOcvChar.BackgroundSumTolerance)
                return false;

            if (Math.Abs(objOcvChar.TemplateForegroundSum - objOcvChar.SampleForegroundSum) > objOcvChar.ForegroundSumTolerance)
                return false;

            if ((objOcvChar.Correlation <= 0) || ((1 - objOcvChar.Correlation) > objOcvChar.CorrelationTolerance))
                return false;

            return true;
        }

        private bool CheckOcvTextDiagnostic(EOCVText objOcvText)
        {
            //if (Math.Abs(objOcvText.TemplateLocationScore - objOcvText.SampleLocationScore) > objOcvText.LocationScoreTolerance)
            if (objOcvText.TemplateLocationScore > (objOcvText.SampleLocationScore + objOcvText.LocationScoreTolerance))
                return false;

            if (Math.Abs(objOcvText.TemplateBackgroundArea - objOcvText.SampleBackgroundArea) > objOcvText.BackgroundAreaTolerance)
                return false;

            if (Math.Abs(objOcvText.TemplateForegroundArea - objOcvText.SampleForegroundArea) > objOcvText.ForegroundAreaTolerance)
                return false;

            if (Math.Abs(objOcvText.TemplateBackgroundSum - objOcvText.SampleBackgroundSum) > objOcvText.BackgroundSumTolerance)
                return false;

            if (Math.Abs(objOcvText.TemplateForegroundSum - objOcvText.SampleForegroundSum) > objOcvText.ForegroundSumTolerance)
                return false;

            if ((objOcvText.Correlation <= 0) || ((1 - objOcvText.Correlation) > objOcvText.CorrelationTolerance))
                return false;

            return true;
        }

        public bool CheckOCVCharValueAvailable()
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX, intCenterY;
            m_OcvText = new EOCVText();
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, 640, 480, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                if (m_OcvText.SampleBackgroundArea == 0 && m_OcvText.SampleForegroundArea == 0 &&
                    m_OcvText.SampleBackgroundSum == 0 && m_OcvText.SampleForegroundSum == 0 &&
                    m_OcvText.SampleLocationScore == 0)
                    return false;
            }
            return true;
        }

        public void ClearArrayBlobsFeatures()
        {
            m_arrBlobsFeatures.Clear();
        }

        public void ClearCharsNumberArray()
        {
            m_arrCharNo.Clear();
        }

        public void ClearCharsSettingArray()
        {
            m_arrCharSetting.Clear();
        }

        public void ClearStatistics()
        {
            m_Ocv.ClearStatistics();

            int intNumTextChars = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX, intCenterY;
            int intCharsCount = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    SetValueToChars(m_OcvChar, (int)m_arrCharSetting[Convert.ToInt32(m_arrCharNo[intCharsCount])]);
                    m_Ocv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                }
                intCharsCount++;
            }
            m_intStatisticCount = 0;
        }

        public bool DefineOCVDiagnostics()
        {
            if (m_Ocv.Diagnostics != (int)EDiagnostic.Undefined)
                return true;

            return false;
        }

        public void DeleteChars()
        {
            m_Ocv.DeleteTemplateChars(ESelectionFlag.Any);
        }

        public void DeleteTexts()
        {
            //m_Ocv.DeleteTemplateObjects(ESelectionFlag.Any);
            m_Ocv.DeleteTemplateTexts(ESelectionFlag.Any);
        }

        public void DrawLine(ImageDrawing objImage, bool bCutMode, bool blnWhiteOnBlack, int intOriginX, int intOriginY)
        {
            if (blnWhiteOnBlack)
            {
                if (bCutMode)
                    m_bw.Value = 0; //Black color
                else
                    m_bw.Value = 255;
            }
            else
            {
                if (bCutMode)
                    m_bw.Value = 255;
                else
                    m_bw.Value = 0;
            }

            objImage.ref_objMainImage.SetPixel(m_bw, intOriginX, intOriginY);

        }

        public void DrawMarkText(Graphics g, int intRoiOrgX, int intRoiOrgY, int intWidth, int intHeight)
        {
            ResetSampleCharsAndTexts(0, 0, intWidth, intHeight);

            float fBlobsCenterX, fBlobsCenterY, fWidth, fHeight;
            int intNoSelectedBlobs = m_blobs.NumSelectedObjects;
            //if (m_blnInpectBlobsON)
            //{
            m_ListEasyObject = m_blobs.FirstObjPtr;
            for (int i = 0; i < intNoSelectedBlobs; i++)
            {
                fBlobsCenterX = fBlobsCenterY = fWidth = fHeight = 0;
                m_blobs.GetObjectFeature(ELegacyFeature.LimitCenterX, m_ListEasyObject, out fBlobsCenterX);
                m_blobs.GetObjectFeature(ELegacyFeature.LimitCenterY, m_ListEasyObject, out fBlobsCenterY);
                m_blobs.GetObjectFeature(ELegacyFeature.LimitWidth, m_ListEasyObject, out fWidth);
                m_blobs.GetObjectFeature(ELegacyFeature.LimitHeight, m_ListEasyObject, out fHeight);

                int intOcvMatchNumber = GetOcvAdvanceMatchCharNumber(intRoiOrgX + fBlobsCenterX, intRoiOrgY + fBlobsCenterY, fWidth, fHeight);
                if (intOcvMatchNumber == -1 && ((m_intFailMask & 0x100) > 0))
                {
                    m_blobs.DrawObjectFeature(g, m_colorRed, ELegacyFeature.LimitHeight, m_ListEasyObject);
                }
                else if (intOcvMatchNumber == -2 && ((m_intFailMask & 0x1000) > 0))
                {
                    m_blobs.DrawObjectFeature(g, m_colorRed, ELegacyFeature.LimitHeight, m_ListEasyObject);
                }
                else if (intOcvMatchNumber >= 0)
                {
                    if (m_blnCharFail[intOcvMatchNumber])
                    {
                        m_blobs.DrawObjectFeature(g, m_colorRed, ELegacyFeature.LimitHeight, m_ListEasyObject);
                    }
                }

                m_ListEasyObject = m_blobs.GetNextObjPtr(m_ListEasyObject);
            }
            //}

            int intArrNum = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intStartX, intStartY;
            int intCenterX, intCenterY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                if (m_blnTextFail[tx])
                {

                }
                else
                {
                    intCenterX = intCenterY = intStartX = intStartY = 0;
                    m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                }

                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                if (m_blnTextFail[tx])
                {

                }
                else
                {
                    intCenterX = intCenterY = intStartX = intStartY = 0;
                    m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                }

                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    if (m_blnCharFail[intArrNum])
                    {

                    }
                    else
                    {
                        intCenterX = intCenterY = 0;

                        m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                        m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    }
                    intStartX = intStartY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    g.DrawString(m_arrCharNo[intArrNum].ToString(), m_Font, new SolidBrush(Color.Red), (float)intStartX, (float)intStartY - 14);

                    intArrNum++;

                }
            }

            m_Ocv.DrawTexts(g, m_colorGreen, ESelectionFlag.True);
            m_Ocv.DrawTextsChars(g, m_colorGreen, ESelectionFlag.Any, ESelectionFlag.True);

            m_Ocv.DrawTexts(g, m_colorRed, ESelectionFlag.False);
            m_Ocv.DrawTextsChars(g, m_colorRed, ESelectionFlag.Any, ESelectionFlag.False);

        }

        public void DrawMarkText(Graphics g, int intRoiOrgX, int intRoiOrgY, int intWidth, int intHeight, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY, int intHScroll, int intVScroll)
        {
            ResetSampleCharsAndTexts(0, 0, intWidth, intHeight);

            float fBlobsCenterX, fBlobsCenterY, fWidth, fHeight;
            int intNoSelectedBlobs = m_blobs.NumSelectedObjects;
            //if (m_blnInpectBlobsON)
            //{
            m_ListEasyObject = m_blobs.FirstObjPtr;
            for (int i = 0; i < intNoSelectedBlobs; i++)
            {
                fBlobsCenterX = fBlobsCenterY = fWidth = fHeight = 0;
                m_blobs.GetObjectFeature(ELegacyFeature.LimitCenterX, m_ListEasyObject, out fBlobsCenterX);
                m_blobs.GetObjectFeature(ELegacyFeature.LimitCenterY, m_ListEasyObject, out fBlobsCenterY);
                m_blobs.GetObjectFeature(ELegacyFeature.LimitWidth, m_ListEasyObject, out fWidth);
                m_blobs.GetObjectFeature(ELegacyFeature.LimitHeight, m_ListEasyObject, out fHeight);

                int intOcvMatchNumber = GetOcvAdvanceMatchCharNumber(intRoiOrgX + fBlobsCenterX, intRoiOrgY + fBlobsCenterY, fWidth, fHeight);
                if (intOcvMatchNumber == -1 && ((m_intFailMask & 0x100) > 0))
                {
                    m_blobs.DrawObject(g, new ERGBColor(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B), m_ListEasyObject);
                    //m_blobs.DrawObjectFeature(g, m_colorRed, ELegacyFeature.LimitHeight, m_ListEasyObject, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
                }
                else if (intOcvMatchNumber == -2 && ((m_intFailMask & 0x1000) > 0))
                {
                    m_blobs.DrawObject(g, new ERGBColor(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B), m_ListEasyObject);
                    //m_blobs.DrawObjectFeature(g, m_colorRed, ELegacyFeature.LimitHeight, m_ListEasyObject, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
                }
                else if (intOcvMatchNumber >= 0)
                {
                    if (m_blnCharFail[intOcvMatchNumber])
                    {
                        //m_blobs.DrawObject(g, new Pen(Color.Yellow), m_ListEasyObject);
                        //m_blobs.DrawObjectFeature(g, m_colorRed, ELegacyFeature.LimitHeight, m_ListEasyObject, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
                    }
                }

                m_ListEasyObject = m_blobs.GetNextObjPtr(m_ListEasyObject);
            }
            //}

            int intArrNum = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intStartX, intStartY;
            int intCenterX, intCenterY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                if (m_blnTextFail[tx])
                {

                }
                else
                {
                    intCenterX = intCenterY = intStartX = intStartY = 0;
                    m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                }

                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                if (m_blnTextFail[tx])
                {

                }
                else
                {
                    intCenterX = intCenterY = intStartX = intStartY = 0;
                    m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                }

                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    if (m_blnCharFail[intArrNum])
                    {

                    }
                    else
                    {
                        intCenterX = intCenterY = 0;

                        m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                        m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    }
                    intArrNum++;

                }
            }

            m_Ocv.DrawTexts(g, m_colorGreen, ESelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTextsChars(g, m_colorGreen, ESelectionFlag.Any, ESelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));

            m_Ocv.DrawTexts(g, m_colorRed, ESelectionFlag.False, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTextsChars(g, m_colorRed, ESelectionFlag.Any, ESelectionFlag.False, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));

            intArrNum = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intStartX = intStartY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    intStartX = Convert.ToInt32(intStartX * fScale) - intHScroll;
                    intStartY = Convert.ToInt32(intStartY * fScale) - intVScroll;
                    g.DrawString(m_arrCharNo[intArrNum].ToString(), m_Font, new SolidBrush(Color.Red), (float)intStartX, (float)intStartY - 3);

                    intArrNum++;
                }
            }
        }

        public void DrawMarkInspectionResult(Graphics g, int intRoiOrgX, int intRoiOrgY, int intWidth, int intHeight, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY, int intHScroll, int intVScroll)
        {
            if (!CheckOCVCharValueAvailable())
                return;

            // Draw fail blobs with yellow color
            try
            {
                int intNoSelectedBlobs = m_blobs.NumSelectedObjects;
                EListItem objListEasyObject = m_blobs.FirstObjPtr;
                for (int i = 0; i < intNoSelectedBlobs; i++)
                {
                    if (m_blnNewInspection)
                        break;

                    if (m_intSampleFailResult[i] == 2)
                        m_blobs.DrawObject(g, new ERGBColor(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B), objListEasyObject);

                    objListEasyObject = m_blobs.GetNextObjPtr(objListEasyObject);
                }

                intNoSelectedBlobs = m_DefectBlobs.NumSelectedObjects;
                objListEasyObject = m_DefectBlobs.FirstObjPtr;
                for (int i = 0; i < intNoSelectedBlobs; i++)
                {
                    if (i < m_blnDefectFail.Length)
                    {
                        if (m_blnDefectFail[i])
                            m_DefectBlobs.DrawObject(g, new ERGBColor(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B), objListEasyObject);
                    }

                    objListEasyObject = m_DefectBlobs.GetNextObjPtr(objListEasyObject);
                }
            }
            catch
            {
            }

            // Select OCV manually
            int intArrNum = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intStartX, intStartY;
            int intEndX, intEndY;
            Pen objPen;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intStartX = intStartY = intEndX = intEndY = 0;
                m_Ocv.GetTextPoint(tx, out intStartX, out intStartY, -1, -1);
                m_Ocv.GetTextPoint(tx, out intEndX, out intEndY, 1, 1);

                intStartX = Convert.ToInt32(intStartX * fScale) - intHScroll; ;
                intStartY = Convert.ToInt32(intStartY * fScale) - intVScroll;
                intEndX = Convert.ToInt32(intEndX * fScale) - intHScroll; ;
                intEndY = Convert.ToInt32(intEndY * fScale) - intVScroll;

                objPen = new Pen(Color.Lime, 1);
                if (m_blnTextFail[tx])
                {
                    objPen.Color = Color.Red;
                }

                g.DrawLine(objPen, intStartX, intStartY, intEndX, intStartY);
                g.DrawLine(objPen, intEndX, intStartY, intEndX, intEndY);
                g.DrawLine(objPen, intEndX, intEndY, intStartX, intEndY);
                g.DrawLine(objPen, intStartX, intEndY, intStartX, intStartY);

                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intStartX = intStartY = intEndX = intEndY = 0;
                m_Ocv.GetTextPoint(tx, out intStartX, out intStartY, -1, -1);
                m_Ocv.GetTextPoint(tx, out intEndX, out intEndY, 1, 1);

                intStartX = Convert.ToInt32(intStartX * fScale) - intHScroll; ;
                intStartY = Convert.ToInt32(intStartY * fScale) - intVScroll;
                intEndX = Convert.ToInt32(intEndX * fScale) - intHScroll; ;
                intEndY = Convert.ToInt32(intEndY * fScale) - intVScroll;

                objPen = new Pen(Color.Lime, 1);
                if (m_blnTextFail[tx])
                {
                    objPen.Color = Color.Red;
                }

                g.DrawLine(objPen, intStartX, intStartY, intEndX, intStartY);
                g.DrawLine(objPen, intEndX, intStartY, intEndX, intEndY);
                g.DrawLine(objPen, intEndX, intEndY, intStartX, intEndY);
                g.DrawLine(objPen, intStartX, intEndY, intStartX, intStartY);

                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intStartX = intStartY = intEndX = intEndY = 0;
                    if (m_intScoreMode[(int)m_arrCharNo[intArrNum]] == 0)
                    {
                        m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                        m_Ocv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);
                    }
                    else
                    {
                        m_BinaOcv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                        m_BinaOcv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);
                    }

                    intStartX = Convert.ToInt32(intStartX * fScale) - intHScroll; ;
                    intStartY = Convert.ToInt32(intStartY * fScale) - intVScroll;
                    intEndX = Convert.ToInt32(intEndX * fScale) - intHScroll; ;
                    intEndY = Convert.ToInt32(intEndY * fScale) - intVScroll;

                    objPen = new Pen(Color.Lime, 1);
                    if (m_blnCharFail[intArrNum])
                    {
                        objPen.Color = Color.Red;
                    }

                    g.DrawLine(objPen, intStartX, intStartY, intEndX, intStartY);
                    g.DrawLine(objPen, intEndX, intStartY, intEndX, intEndY);
                    g.DrawLine(objPen, intEndX, intEndY, intStartX, intEndY);
                    g.DrawLine(objPen, intStartX, intEndY, intStartX, intStartY);

                    intArrNum++;
                }
            }

            // Draw OCV using ocv draw feature
            //m_BinaOcv.DrawTexts(g, m_penGreen, ESelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            //m_BinaOcv.DrawTextsChars(g, m_penGreen, ESelectionFlag.Any, ESelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));

            //m_BinaOcv.DrawTexts(g, m_colorRed, ESelectionFlag.False, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            //m_BinaOcv.DrawTextsChars(g, m_colorRed, ESelectionFlag.Any, ESelectionFlag.False, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));

            // Draw templates number
            intArrNum = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intStartX = intStartY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    intStartX = Convert.ToInt32(intStartX * fScale) - intHScroll;
                    intStartY = Convert.ToInt32(intStartY * fScale) - intVScroll;
                    g.DrawString(m_arrCharNo[intArrNum].ToString(), m_Font, new SolidBrush(Color.Red), (float)intStartX, (float)intStartY - 3);

                    intArrNum++;
                }
            }
        }

        public void DrawSelectedMark(Graphics g, int intMarkTextSelectedNo, int intMarkCharSelectedNo, int intRoiOrgX, int intRoiOrgY, int intWidth, int intHeight, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY, int intHScroll, int intVScroll)
        {
            if (!CheckOCVCharValueAvailable())
                return;

            // Draw fail blobs with yellow color
            int intNoSelectedBlobs = m_blobs.NumSelectedObjects;
            try
            {
                EListItem objListEasyObject = m_blobs.FirstObjPtr;
                for (int i = 0; i < intNoSelectedBlobs; i++)
                {
                    if (m_blnNewInspection)
                        break;

                    if (m_intSampleFailResult[i] == 2)
                        m_blobs.DrawObject(g, new ERGBColor(Color.Yellow.R, Color.Yellow.G, Color.Yellow.B), objListEasyObject);

                    objListEasyObject = m_blobs.GetNextObjPtr(objListEasyObject);
                }
            }
            catch
            {
            }

            // Select OCV manually
            int intArrNum = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intStartX, intStartY;
            int intEndX, intEndY;
            Pen objPen;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intStartX = intStartY = intEndX = intEndY = 0;
                m_Ocv.GetTextPoint(tx, out intStartX, out intStartY, -1, -1);
                m_Ocv.GetTextPoint(tx, out intEndX, out intEndY, 1, 1);

                intStartX = Convert.ToInt32(intStartX * fScale) - intHScroll; ;
                intStartY = Convert.ToInt32(intStartY * fScale) - intVScroll;
                intEndX = Convert.ToInt32(intEndX * fScale) - intHScroll; ;
                intEndY = Convert.ToInt32(intEndY * fScale) - intVScroll;

                objPen = new Pen(Color.Red, 1);
                if ((tx == intMarkTextSelectedNo) && (intMarkCharSelectedNo < 0))
                {
                    objPen.Color = Color.Lime;
                }

                g.DrawLine(objPen, intStartX, intStartY, intEndX, intStartY);
                g.DrawLine(objPen, intEndX, intStartY, intEndX, intEndY);
                g.DrawLine(objPen, intEndX, intEndY, intStartX, intEndY);
                g.DrawLine(objPen, intStartX, intEndY, intStartX, intStartY);

                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intStartX = intStartY = intEndX = intEndY = 0;
                m_Ocv.GetTextPoint(tx, out intStartX, out intStartY, -1, -1);
                m_Ocv.GetTextPoint(tx, out intEndX, out intEndY, 1, 1);

                intStartX = Convert.ToInt32(intStartX * fScale) - intHScroll; ;
                intStartY = Convert.ToInt32(intStartY * fScale) - intVScroll;
                intEndX = Convert.ToInt32(intEndX * fScale) - intHScroll; ;
                intEndY = Convert.ToInt32(intEndY * fScale) - intVScroll;

                objPen = new Pen(Color.Red, 1);
                if ((tx == intMarkTextSelectedNo) && (intMarkCharSelectedNo < 0))
                {
                    objPen.Color = Color.Lime;
                }

                g.DrawLine(objPen, intStartX, intStartY, intEndX, intStartY);
                g.DrawLine(objPen, intEndX, intStartY, intEndX, intEndY);
                g.DrawLine(objPen, intEndX, intEndY, intStartX, intEndY);
                g.DrawLine(objPen, intStartX, intEndY, intStartX, intStartY);

                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intStartX = intStartY = intEndX = intEndY = 0;
                    if (m_intScoreMode[(int)m_arrCharNo[intArrNum]] == 0)
                    {
                        m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                        m_Ocv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);
                    }
                    else
                    {
                        m_BinaOcv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                        m_BinaOcv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);
                    }

                    intStartX = Convert.ToInt32(intStartX * fScale) - intHScroll; ;
                    intStartY = Convert.ToInt32(intStartY * fScale) - intVScroll;
                    intEndX = Convert.ToInt32(intEndX * fScale) - intHScroll; ;
                    intEndY = Convert.ToInt32(intEndY * fScale) - intVScroll;

                    objPen = new Pen(Color.Red, 1);
                    if (intArrNum == intMarkCharSelectedNo)
                    {
                        objPen.Color = Color.Lime;
                    }

                    g.DrawLine(objPen, intStartX, intStartY, intEndX, intStartY);
                    g.DrawLine(objPen, intEndX, intStartY, intEndX, intEndY);
                    g.DrawLine(objPen, intEndX, intEndY, intStartX, intEndY);
                    g.DrawLine(objPen, intStartX, intEndY, intStartX, intStartY);

                    intArrNum++;
                }
            }

            // Draw templates number
            intArrNum = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intStartX = intStartY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    intStartX = Convert.ToInt32(intStartX * fScale) - intHScroll;
                    intStartY = Convert.ToInt32(intStartY * fScale) - intVScroll;
                    g.DrawString(m_arrCharNo[intArrNum].ToString(), m_Font, new SolidBrush(Color.Red), (float)intStartX, (float)intStartY - 3);

                    intArrNum++;
                }
            }
        }

        public void DrawRecSelectedObjects(Graphics g, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY, int intHScroll, int intVScroll)
        {
            int intArrNum = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intStartX, intStartY;

            m_Ocv.DrawTexts(g,m_colorGreen, ESelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTextsChars(g, m_colorGreen, ESelectionFlag.Any, ESelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));

            m_Ocv.DrawTexts(g, m_colorRed, ESelectionFlag.False, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTextsChars(g, m_colorRed, ESelectionFlag.Any, ESelectionFlag.False, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
             for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intStartX = intStartY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    intStartX = Convert.ToInt32(intStartX * fScale) - intHScroll;
                    intStartY = Convert.ToInt32(intStartY * fScale) - intVScroll;
                    g.DrawString(m_arrCharNo[intArrNum].ToString(), m_Font, new SolidBrush(Color.Red), (float)intStartX, (float)intStartY - 3);

                    intArrNum++;
                }
            }

        }

        public void DrawSelectedChars(Graphics g, int intOriginX, int intOriginY)
        {
            m_Ocv.DrawTemplateChars(g, m_colorGreen, ESelectionFlag.True, 1, 1, 0, 0);
            m_Ocv.DrawTemplateChars(g, m_colorRed, ESelectionFlag.False, 1, 1, 0, 0);
            m_Ocv.DrawTemplateObjects(g, m_colorGreen, ESelectionFlag.True, 1, 1, 0, 0);
            m_Ocv.DrawTemplateObjects(g, m_colorRed, ESelectionFlag.False, 1, 1, 0, 0);
        }

        public void DrawSelectedChars(Graphics g, int intOriginX, int intOriginY, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY)
        {
            m_Ocv.DrawTemplateChars(g, m_colorGreen, ESelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTemplateChars(g, m_colorRed, ESelectionFlag.False, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTemplateObjects(g, m_colorGreen, ESelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTemplateObjects(g, m_colorRed, ESelectionFlag.False, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
        }

        public void DrawSelectedObject(Graphics g)
        {
            DrawSelectedObject(g, 1.0f, 0, 0);
        }

        public void DrawSelectedObject(Graphics g, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY)
        {
            m_ListEasyObject = m_blobs.FirstObjPtr;
            int intColorNo = 0;
            int intObjNo = 0;
            int intNumObjects = m_blobs.NumSelectedObjects;

            for (int i = 0; i < intNumObjects; i++)
            {
                m_blobs.GetObjectFeature(ELegacyFeature.ObjectNumber, m_ListEasyObject, out intObjNo);
                m_blobs.DrawObject(g, new ERGBColor(m_colorList[intColorNo].R, m_colorList[intColorNo].G, m_colorList[intColorNo].B), (int)intObjNo, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));

                m_ListEasyObject = m_blobs.GetNextObjPtr(m_ListEasyObject);

                if (++intColorNo >= 10)
                    intColorNo = 0;
            }
        }

        public void DrawSelectedObjects(Graphics g)
        {
            m_blobs.DrawObjects(g, m_colorGreen, ESelectionFlag.True);
            m_blobs.DrawObjects(g, m_colorRed, ESelectionFlag.False);
        }

        public void DrawSelectedTexts(Graphics g, int intOriginX, int intOriginY)
        {
            m_Ocv.DrawTemplateTexts(g, m_colorGreenText, ESelectionFlag.True, 1, 1, 0, 0);
            m_Ocv.DrawTemplateObjects(g, m_colorGreenText, ESelectionFlag.True, 1, 1, 0, 0);
        }

        public void DrawSelectedTexts(Graphics g, int intOriginX, int intOriginY, float fScale, int intZoomImageEdgeX, int intZoomImageEdgeY)
        {
            m_Ocv.DrawTemplateTexts(g, m_colorGreenText, ESelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
            m_Ocv.DrawTemplateObjects(g, m_colorGreenText, ESelectionFlag.True, fScale, fScale, -(intZoomImageEdgeX / fScale), -(intZoomImageEdgeY / fScale));
        }

        public void GatherTextsAndCharsParameters()
        {
            m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
            m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
        }

        public void GatherTextsAndCharsParameters(int intOriginX, int intOriginY)
        {
            // use default(gradient) ocv object when selecting ocv text
            m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
            m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);

            int intMatchNumber = GetOcvMatchCharNumber(intOriginX, intOriginY);
            if (intMatchNumber < 0)
            {
                intMatchNumber = GetBinarizedOcvMatchCharNumber(intOriginX, intOriginY);
                if (intMatchNumber < 0)
                    return;
            }

            if (m_intScoreMode[(int)m_arrCharNo[intMatchNumber]] == 1)
            {
                m_BinaOcv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
            }
        }

        private int GetArrayBlobsFeaturesIndex(int intOcvMatchNumber)
        {
            for (int i = 0; i < m_arrBlobsFeatures.Count; i++)
            {
                m_stcBlobsFeatures = (BlobsFeatures)m_arrBlobsFeatures[i];
                if (m_stcBlobsFeatures.intOcvMatchNumber == intOcvMatchNumber)
                    return i;
            }
            return -1;
        }

        public int GetAutoThresholdValue(ROI objROI, int intThresholdMode)
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

        public string GetBlobsFeatures(ROI objROI, bool blnWhiteOnBlack, int intRoiOrgX, int intRoiOrgY)
        {
            string strBlobsFeatures = "";

            if (objROI.ref_ROI.Width == 0)
            {
                m_strErrorMessage = "OCV GetBlobsFeatures :ROI no loaded!";
                return "";
            }

            bool blnChecking = GetDiagnosticResult();
            ECodedImage objBlobs = new ECodedImage();
            objBlobs = new ECodedImage();
            objBlobs.RemoveAllObjects();
#if (Debug_2_12 || Release_2_12)
            // Set threshold value
            objBlobs.SetThreshold((uint)m_intThresholdValue);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Set threshold value
            objBlobs.SetThreshold(m_intThresholdValue);
#endif

            objBlobs.Connexity = EConnexity.Connexity4;
            // Set white black class
            if (blnWhiteOnBlack)
            {
                objBlobs.BlackClass = 0;
                objBlobs.WhiteClass = 1;
            }
            else
            {
                objBlobs.BlackClass = 1;
                objBlobs.WhiteClass = 0;
            }
            objBlobs.BuildObjects(objROI.ref_ROI);

            try
            {
                // Build and select objects
                objBlobs.AnalyseObjects(ELegacyFeature.Area, ELegacyFeature.ObjectNumber, ELegacyFeature.GravityCenterX, ELegacyFeature.GravityCenterY, ELegacyFeature.LimitCenterX, ELegacyFeature.LimitCenterY, ELegacyFeature.LimitWidth, ELegacyFeature.LimitHeight, ELegacyFeature.EllipseAngle);
                objBlobs.SelectObjectsUsingFeature(ELegacyFeature.Area, m_intMinArea, m_intMaxArea, ESelectOption.RemoveOutOfRange);
                objBlobs.SelectObjectsUsingPosition(objROI.ref_ROI, ESelectByPosition.RemoveBorder);
                objBlobs.SortObjectsUsingFeature(ELegacyFeature.ObjectNumber, ESortOption.Descending);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "OCV GetBlobsFeatures : " + ex.ToString();
                m_objTrackLog.WriteLine("OCV GetBlobsFeatures : " + ex.ToString());
                return "";
            }

            int intNoSelectedBlobs = m_blobs.NumSelectedObjects;
            if (intNoSelectedBlobs == 0)
            {
                m_strErrorMessage = "OCV GetBlobsFeatures : " + "No blobs object selected!";
                return "";
            }
            else
            {
                m_arrBlobsFeatures.Clear();
                int intTempArea, intObjNo, intOcvMatchNumber;
                float fTempWidth, fTempHeight, fTempAngle, fBlobsCenterX, fBlobsCenterY;
                m_ListEasyObject = m_blobs.FirstObjPtr;
                for (int i = 0; i < intNoSelectedBlobs; i++)
                {
                    intTempArea = intObjNo = intOcvMatchNumber = 0;
                    fTempWidth = fTempHeight = fTempAngle = fBlobsCenterX = fBlobsCenterY = 0;
                    objBlobs.GetObjectFeature(ELegacyFeature.Area, m_ListEasyObject, out intTempArea);
                    objBlobs.GetObjectFeature(ELegacyFeature.ObjectNumber, m_ListEasyObject, out intObjNo);
                    objBlobs.GetObjectFeature(ELegacyFeature.LimitWidth, m_ListEasyObject, out fTempWidth);
                    objBlobs.GetObjectFeature(ELegacyFeature.LimitHeight, m_ListEasyObject, out fTempHeight);
                    objBlobs.GetObjectFeature(ELegacyFeature.EllipseAngle, m_ListEasyObject, out fTempAngle);
                    objBlobs.GetObjectFeature(ELegacyFeature.LimitCenterX, m_ListEasyObject, out fBlobsCenterX);
                    objBlobs.GetObjectFeature(ELegacyFeature.LimitCenterY, m_ListEasyObject, out fBlobsCenterY);
                    intOcvMatchNumber = GetOcvMatchCharNumber(intRoiOrgX + fBlobsCenterX, intRoiOrgY + fBlobsCenterY);

                    if (intOcvMatchNumber >= 0)
                    {
                        m_stcBlobsFeatures.intObjNumber = intObjNo;
                        m_stcBlobsFeatures.intArea = intTempArea;
                        m_stcBlobsFeatures.fWidth = fTempWidth;
                        m_stcBlobsFeatures.fHeight = fTempHeight;
                        m_stcBlobsFeatures.fAngle = fTempAngle;
                        m_stcBlobsFeatures.fCenterX = fBlobsCenterX;
                        m_stcBlobsFeatures.fCenterY = fBlobsCenterY;
                        m_stcBlobsFeatures.intOcvMatchNumber = intOcvMatchNumber;
                        m_stcBlobsFeatures.blnMatch = false;
                        m_arrBlobsFeatures.Add(m_stcBlobsFeatures);

                        strBlobsFeatures += intObjNo.ToString() + "#";
                        strBlobsFeatures += intTempArea.ToString() + "#";
                        strBlobsFeatures += fTempWidth.ToString() + "#";
                        strBlobsFeatures += fTempHeight.ToString() + "#";
                        strBlobsFeatures += fTempAngle.ToString() + "#";
                        strBlobsFeatures += fBlobsCenterX.ToString() + "#";
                        strBlobsFeatures += fBlobsCenterY.ToString() + "#";
                        strBlobsFeatures += intOcvMatchNumber.ToString() + "#";
                    }
                    m_ListEasyObject = m_blobs.GetNextObjPtr(m_ListEasyObject);
                }
            }

            objBlobs.Dispose();
            return strBlobsFeatures;
        }

        public int GetCharNumber(int intSelectedIndex)
        {
            return (int)m_arrCharNo[intSelectedIndex];
        }

        public int GetCharsCount()
        {
            int intNumTextChars = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCharsCount = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
                    intCharsCount++;
                }
            }
            m_intCharsCount = intCharsCount;
            return intCharsCount;
        }

        public int GetCharSetting(int intSelectedIndex)
        {
            if (intSelectedIndex < m_arrCharSetting.Count)
            {
                return (int)m_arrCharSetting[intSelectedIndex];
            }
            return 0;
        }

        public int[] GetCharSetting()
        {
            int[] intCharSetting = new int[m_arrCharSetting.Count];
            for (int i = 0; i < m_arrCharSetting.Count; i++)
            {
                intCharSetting[i] = (int)m_arrCharSetting[i];
            }

            return intCharSetting;
        }

        public int[] GetTextSetting(int intWidth, int intHeight)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int[] intTextSetting = new int[intNumTexts];
            int intCenterX, intCenterY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
          for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, intWidth, intHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                string texts = m_OcvText.CorrelationTolerance.ToString();
                intTextSetting[tx] = Convert.ToInt32((1 - m_OcvText.CorrelationTolerance) * 100);
                m_Ocv.ScatterTextsParameters(m_OcvText, ESelectionFlag.True);
            }

            return intTextSetting;
        }

        public int[] GetCharShiftX(int intWidth, int intHeight)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intNumChars = 0;
            int[] intCharShiftX;
            int intCenterX, intCenterY;
            int intCharIndex = 0;
#if (Debug_2_12 || Release_2_12)
            // Get chars count
            for (uint tx = 0; tx < intNumTexts; tx++)
                intNumChars += (int)m_Ocv.GetNumTextChars(tx);

            intCharShiftX = new int[intNumChars];

            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Get chars count
                for (int tx = 0; tx < intNumTexts; tx++)
                intNumChars += m_Ocv.GetNumTextChars(tx);
 
            intCharShiftX = new int[intNumChars];

            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, intWidth, intWidth, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    intCharShiftX[intCharIndex] = (int)m_OcvChar.ShiftXTolerance;
                    intCharIndex++;
                }
            }

            return intCharShiftX;
        }

        public int[] GetCharShiftY(int intWidth, int intHeight)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intNumChars = 0;
            int[] intCharShiftY;
            int intCenterX, intCenterY;
            int intCharIndex = 0;
#if (Debug_2_12 || Release_2_12)
            // Get chars count
            for (uint tx = 0; tx < intNumTexts; tx++)
                intNumChars += (int)m_Ocv.GetNumTextChars(tx);

            intCharShiftY = new int[intNumChars];

            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Get chars count
            for (int tx = 0; tx < intNumTexts; tx++)
                intNumChars += m_Ocv.GetNumTextChars(tx);

            intCharShiftY = new int[intNumChars];

            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, intWidth, intWidth, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    intCharShiftY[intCharIndex] = (int)m_OcvChar.ShiftYTolerance;
                    intCharIndex++;
                }
            }

            return intCharShiftY;
        }

        public int[] GetTextShiftX(int intWidth, int intHeight)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int[] intTextShiftX = new int[intNumTexts];
            int intCenterX, intCenterY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, intWidth, intHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                intTextShiftX[tx] = (int)m_OcvText.ShiftXTolerance;
            }

            return intTextShiftX;
        }

        public int[] GetTextShiftY(int intWidth, int intHeight)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int[] intTextShiftY = new int[intNumTexts];
            int intCenterX, intCenterY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, intWidth, intHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                intTextShiftY[tx] = (int)m_OcvText.ShiftYTolerance;
            }

            return intTextShiftY;
        }

        public bool GetDiagnosticResult()
        {
            if (((m_Ocv.Diagnostics & (int)EDiagnostic.CharNotFound) > 0) ||
                ((m_Ocv.Diagnostics & (int)EDiagnostic.CharMismatch) > 0) ||
                ((m_Ocv.Diagnostics & (int)EDiagnostic.CharOverprinting) > 0) ||
                ((m_Ocv.Diagnostics & (int)EDiagnostic.CharUnderprinting) > 0) ||
                ((m_Ocv.Diagnostics & (int)EDiagnostic.TextNotFound) > 0) ||
                ((m_Ocv.Diagnostics & (int)EDiagnostic.TextMismatch) > 0) ||
                ((m_Ocv.Diagnostics & (int)EDiagnostic.TextOverprinting) > 0) ||
                ((m_Ocv.Diagnostics & (int)EDiagnostic.TextUnderprinting) > 0))
            {
                return false;
            }
            return true;
        }

        public bool GetDiagnosticCharNotFound()
        {
            return ((m_Ocv.Diagnostics & (int)EDiagnostic.CharNotFound) > 0);
        }

        public bool GetDiagnosticCharMismatch()
        {
            return ((m_Ocv.Diagnostics & (int)EDiagnostic.CharMismatch) > 0);
        }

        public bool GetDiagnosticCharOverprint()
        {
            return ((m_Ocv.Diagnostics & (int)EDiagnostic.CharOverprinting) > 0);
        }

        public bool GetDiagnosticCharUnderprint()
        {
            return ((m_Ocv.Diagnostics & (int)EDiagnostic.CharUnderprinting) > 0);
        }

        public bool GetDiagnosticTextNotFound()
        {
            return ((m_Ocv.Diagnostics & (int)EDiagnostic.TextNotFound) > 0);
        }

        public bool GetDiagnosticTextMismatch()
        {
            return ((m_Ocv.Diagnostics & (int)EDiagnostic.TextMismatch) > 0);
        }

        public bool GetDiagnosticTextOverprint()
        {
            return ((m_Ocv.Diagnostics & (int)EDiagnostic.TextOverprinting) > 0);
        }

        public bool GetDiagnosticTextUnderprint()
        {
            return ((m_Ocv.Diagnostics & (int)EDiagnostic.TextUnderprinting) > 0);
        }

        public int GetNumBlobsObjects()
        {
            return m_blobs.NumObjects;
        }

        public int GetNumBlobsSelectedObjects()
        {
            return m_blobs.NumSelectedObjects;
        }

        public int GetNumTextChars(int intTextIndex)
        {
#if (Debug_2_12 || Release_2_12)
            return (int)m_Ocv.GetNumTextChars((uint)intTextIndex);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
             return m_Ocv.GetNumTextChars(intTextIndex);
#endif

        }

        public int NumTexts()
        {
            return (int)m_Ocv.NumTexts;
        }

        public int GetMatchCharNumber(int intX, int intY)
        {
            int intMatchNumber = GetOcvMatchCharNumber(intX, intY);
            if (intMatchNumber < 0)
            {
                intMatchNumber = GetBinarizedOcvMatchCharNumber(intX, intY);
                if (intMatchNumber < 0)
                    return -1;
            }
            return intMatchNumber;
        }

        public int GetMatchTextNumber(int intX, int intY)
        {
            return GetOcvMatchTextNumber(intX, intY);
        }

        private int GetOcvMatchCharNumber(int intX, int intY)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intStartX, intStartY, intEndX, intEndY;
            int intNumber = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intStartX = intStartY = intEndX = intEndY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);

                    if ((intX > intStartX) && (intX < intEndX) &&
                        (intY > intStartY) && (intY < intEndY))
                    {
                        return intNumber;
                    }
                    intNumber++;
                }
            }
            return -1;
        }

        private int GetBinarizedOcvMatchCharNumber(int intX, int intY)
        {
            int intNumTexts = (int)m_BinaOcv.NumTexts;
            int intNumTextChars = 0;
            int intStartX, intStartY, intEndX, intEndY;
            int intNumber = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_BinaOcv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_BinaOcv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intStartX = intStartY = intEndX = intEndY = 0;
                    m_BinaOcv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    m_BinaOcv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);

                    if ((intX > intStartX) && (intX < intEndX) &&
                        (intY > intStartY) && (intY < intEndY))
                    {
                        return intNumber;
                    }
                    intNumber++;
                }
            }
            return -1;
        }

        private int GetOcvAdvanceMatchCharNumber(float fBlobsCenterX, float fBlobsCenterY, float fWidth, float fHeight)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intCenterX, intCenterY, intStartX, intStartY, intEndX, intEndY;
            float fStartX = fBlobsCenterX - (fWidth / 2);
            float fStartY = fBlobsCenterY - (fHeight / 2);
            float fEndX = fBlobsCenterX + (fWidth / 2);
            float fEndY = fBlobsCenterY + (fHeight / 2);
            int intNumber = -1;
            int intOcvMatchNumber = -1;
            int intTotalOcvMatch = 0;
            bool blnFound = false;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                //m_Ocv.GetTextPoint(tx, out intX, out intY, 0, 0);
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                //m_Ocv.GetTextPoint(tx, out intX, out intY, 0, 0);
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intNumber++;
                    intCenterX = intCenterY = intStartX = intStartY = intEndX = intEndY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);

                    //if ((fBlobsCenterX <= (intCenterX + 10)) && (fBlobsCenterY <= (intCenterY + 10)) &&
                    //(fBlobsCenterX >= (intCenterX - 10)) && (fBlobsCenterY >= (intCenterY - 10)))
                    if ((fBlobsCenterX < intEndX) && (fBlobsCenterY < intEndY) &&
                        (fBlobsCenterX > intStartX) && (fBlobsCenterY > intStartY))
                    {
                        int intWidth = intEndX - intStartX;
                        int intHeight = intEndY - intStartY;
                        if ((fWidth <= (intWidth + m_intToleranceSize)) && (fWidth >= (intWidth - m_intToleranceSize)) &&
                            (fHeight <= (intHeight + m_intToleranceSize)) && (fHeight >= (intHeight - m_intToleranceSize)))
                        {
                            // must check the width and height or area also
                            m_intMatchState[intNumber] = 1;
                            intOcvMatchNumber = intNumber;
                            blnFound = true;
                            break;
                        }
                    }
                }
                if (blnFound)
                    break;
            }

            if (blnFound)
            {
                return intOcvMatchNumber;
            }
            else
            {
                intNumber = -1;
                intOcvMatchNumber = -1;
                intTotalOcvMatch = 0;
#if (Debug_2_12 || Release_2_12)
                for (uint tx = 0; tx < intNumTexts; tx++)
                {
                    //m_Ocv.GetTextPoint(tx, out intX, out intY, 0, 0);
                    intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                    for (uint ch = 0; ch < intNumTextChars; ch++)
                    {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                for (int tx = 0; tx < intNumTexts; tx++)
                {
                    //m_Ocv.GetTextPoint(tx, out intX, out intY, 0, 0);
                    intNumTextChars = m_Ocv.GetNumTextChars(tx);
                    for (int ch = 0; ch < intNumTextChars; ch++)
                    {
#endif

                        intNumber++;
                        intCenterX = intCenterY = intStartX = intStartY = intEndX = intEndY = 0;
                        m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                        m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                        m_Ocv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);

                        if (((intStartX > fStartX) && (intStartX < fEndX) && (intStartY > fStartY) && (intStartY < fEndY)) ||
                            ((intStartX > fStartX) && (intStartX < fEndX) && (intEndY > fStartY) && (intEndY < fEndY)) ||
                            ((intEndX > fStartX) && (intEndX < fEndX) && (intStartY > fStartY) && (intStartY < fEndY)) ||
                            ((intEndX > fStartX) && (intEndX < fEndX) && (intEndY > fStartY) && (intEndY < fEndY)) ||
                            ((fStartX > intStartX) && (fStartX < intEndX) && (fStartY > intStartY) && (fStartY < intEndY)) ||
                            ((fStartX > intStartX) && (fStartX < intEndX) && (fEndY > intStartY) && (fEndY < intEndY)) ||
                            ((fEndX > intStartX) && (fEndX < intEndX) && (fStartY > intStartY) && (fStartY < intEndY)) ||
                            ((fEndX > intStartX) && (fEndX < intEndX) && (fEndY > intStartY) && (fEndY < intEndY)) ||
                            ((intStartX > fStartX) && (intStartX < fEndX) && (intStartY < fStartY) && (intEndY > fEndY)) ||
                            ((intStartY > fStartY) && (intStartY < fEndY) && (intStartX < fStartX) && (intEndX > fEndX))
                            )
                        {
                            intTotalOcvMatch++;
                            intOcvMatchNumber = intNumber;
                            //return intOcvMatchNumber;
                        }
                    }
                }
                if (intTotalOcvMatch == 1)
                    return intOcvMatchNumber;
                else if (intTotalOcvMatch > 1)
                    return -2;
                else
                    return -1;
            }
        }

        private int GetOcvMatchTextNumber(int intX, int intY)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intStartX, intStartY, intEndX, intEndY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                intStartX = intStartY = intEndX = intEndY = 0;
                m_Ocv.GetTextPoint(tx, out intStartX, out intStartY, -1, -1);
                m_Ocv.GetTextPoint(tx, out intEndX, out intEndY, 1, 1);

                if ((intX > intStartX) && (intX < intEndX) &&
                    (intY > intStartY) && (intY < intEndY))
                {
                    return (int)tx;
                }
            }
            return -1;
        }

        private int GetOcvCharDiagnostic(EOCVChar objOcvChar, bool blnReturnOnceFail)
        {
            int intDiagnostic = 0;
            float fTempScore = 100;

            //if (Math.Abs(objOcvChar.TemplateLocationScore - objOcvChar.SampleLocationScore) > objOcvChar.LocationScoreTolerance)
            if (objOcvChar.TemplateLocationScore > (objOcvChar.SampleLocationScore + objOcvChar.LocationScoreTolerance))
            {
                if ((m_intFailMask & 0x10) > 0)
                {
                    // Get Sample Location Score
                    //fTempScore = (objOcvChar.TemplateLocationScore - Math.Abs(objOcvChar.TemplateLocationScore - objOcvChar.SampleLocationScore)) / objOcvChar.TemplateLocationScore * 100;
                    if (objOcvChar.TemplateLocationScore > objOcvChar.SampleLocationScore)
                        fTempScore = objOcvChar.SampleLocationScore / objOcvChar.TemplateLocationScore * 100;
                    else
                        fTempScore = 100;
                    if (fTempScore < m_fLocationFailScore)
                        m_fLocationFailScore = fTempScore;

                    intDiagnostic |= 0x10;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
            }

            if ((objOcvChar.Correlation <= 0) || ((1 - objOcvChar.Correlation) > objOcvChar.CorrelationTolerance))
            {
                if ((m_intFailMask & 0x20) > 0)
                {
                    // Get Sample Correlation Score
                    if (objOcvChar.Correlation <= 0)
                        fTempScore = 0;
                    else
                        fTempScore = objOcvChar.Correlation * 100;
                    if (fTempScore < m_fCorrelationFailScore)
                        m_fCorrelationFailScore = fTempScore;

                    intDiagnostic |= 0x20;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
            }

            bool blnBackgroundFail = false;
            bool blnForegroundFail = false;
            if ((Math.Abs(objOcvChar.TemplateBackgroundArea - objOcvChar.SampleBackgroundArea) > objOcvChar.BackgroundAreaTolerance) ||
                (Math.Abs(objOcvChar.TemplateBackgroundSum - objOcvChar.SampleBackgroundSum) > objOcvChar.BackgroundSumTolerance))
            {
                if ((m_intFailMask & 0x40) > 0)
                    blnBackgroundFail = true;
                //intDiagnostic |= 0x40;
                //if (blnReturnOnceFail)
                //    return intDiagnostic;
            }

            if ((Math.Abs(objOcvChar.TemplateForegroundArea - objOcvChar.SampleForegroundArea) > objOcvChar.ForegroundAreaTolerance) ||
                (Math.Abs(objOcvChar.TemplateForegroundSum - objOcvChar.SampleForegroundSum) > objOcvChar.ForegroundSumTolerance))
            {
                if ((m_intFailMask & 0x80) > 0)
                    blnForegroundFail = true;
                //intDiagnostic |= 0x80;
                //if (blnReturnOnceFail)
                //    return intDiagnostic;
            }

            if (blnBackgroundFail && blnForegroundFail)
            {
                if ((((float)objOcvChar.TemplateBackgroundArea - Math.Abs((float)objOcvChar.TemplateBackgroundArea - (float)objOcvChar.SampleBackgroundArea)) / (float)objOcvChar.TemplateBackgroundArea * 100) <
                    (((float)objOcvChar.TemplateForegroundArea - Math.Abs((float)objOcvChar.TemplateForegroundArea - (float)objOcvChar.SampleForegroundArea)) / (float)objOcvChar.TemplateForegroundArea * 100))
                {
                    // Get Sample Background Area Score
                    fTempScore = ((float)objOcvChar.TemplateBackgroundArea - Math.Abs((float)objOcvChar.TemplateBackgroundArea - (float)objOcvChar.SampleBackgroundArea)) / (float)objOcvChar.TemplateBackgroundArea * 100;
                    if (fTempScore < m_fAreaFailScore)
                        m_fAreaFailScore = fTempScore;

                    intDiagnostic |= 0x40;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
                else
                {
                    // Get Sample Foreground Area Score
                    fTempScore = ((float)objOcvChar.TemplateForegroundArea - Math.Abs((float)objOcvChar.TemplateForegroundArea - (float)objOcvChar.SampleForegroundArea)) / (float)objOcvChar.TemplateForegroundArea * 100;
                    if (fTempScore < m_fAreaFailScore)
                        m_fAreaFailScore = fTempScore;

                    intDiagnostic |= 0x80;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
            }
            else if (blnBackgroundFail)
            {
                // Get Sample Background Area Score
                fTempScore = ((float)objOcvChar.TemplateBackgroundArea - Math.Abs((float)objOcvChar.TemplateBackgroundArea - (float)objOcvChar.SampleBackgroundArea)) / (float)objOcvChar.TemplateBackgroundArea * 100;
                if (fTempScore < m_fAreaFailScore)
                    m_fAreaFailScore = fTempScore;

                intDiagnostic |= 0x40;
                if (blnReturnOnceFail)
                    return intDiagnostic;
            }
            else if (blnForegroundFail)
            {
                // Get Sample Foreground Area Score
                fTempScore = ((float)objOcvChar.TemplateForegroundArea - Math.Abs((float)objOcvChar.TemplateForegroundArea - (float)objOcvChar.SampleForegroundArea)) / (float)objOcvChar.TemplateForegroundArea * 100;
                if (fTempScore < m_fAreaFailScore)
                    m_fAreaFailScore = fTempScore;

                intDiagnostic |= 0x80;
                if (blnReturnOnceFail)
                    return intDiagnostic;
            }

            return intDiagnostic;
        }

        private int GetOcvCharDiagnostic2(EOCVChar objOcvChar, bool blnReturnOnceFail)
        {
            int intDiagnostic = 0;
            float fTempScore = 100;

            //if (Math.Abs(objOcvChar.TemplateLocationScore - objOcvChar.SampleLocationScore) > objOcvChar.LocationScoreTolerance)
            if (objOcvChar.TemplateLocationScore > (objOcvChar.SampleLocationScore + objOcvChar.LocationScoreTolerance))
            {
                if ((m_intFailMask & 0x10) > 0)
                {
                    // Get Sample Location Score
                    //fTempScore = (objOcvChar.TemplateLocationScore - Math.Abs(objOcvChar.TemplateLocationScore - objOcvChar.SampleLocationScore)) / objOcvChar.TemplateLocationScore * 100;
                    if (objOcvChar.TemplateLocationScore > objOcvChar.SampleLocationScore)
                        fTempScore = objOcvChar.SampleLocationScore / objOcvChar.TemplateLocationScore * 100;
                    else
                        fTempScore = 100;
                    if (fTempScore < m_fLocationFailScore2)
                        m_fLocationFailScore2 = fTempScore;

                    intDiagnostic |= 0x10;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
            }

            if ((objOcvChar.Correlation <= 0) || ((1 - objOcvChar.Correlation) > objOcvChar.CorrelationTolerance))
            {
                if ((m_intFailMask & 0x20) > 0)
                {
                    // Get Sample Correlation Score
                    if (objOcvChar.Correlation <= 0)
                        fTempScore = 0;
                    else
                        fTempScore = objOcvChar.Correlation * 100;
                    if (fTempScore < m_fCorrelationFailScore2)
                        m_fCorrelationFailScore2 = fTempScore;

                    intDiagnostic |= 0x20;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
            }

            bool blnBackgroundFail = false;
            bool blnForegroundFail = false;
            if ((Math.Abs(objOcvChar.TemplateBackgroundArea - objOcvChar.SampleBackgroundArea) > objOcvChar.BackgroundAreaTolerance) ||
                (Math.Abs(objOcvChar.TemplateBackgroundSum - objOcvChar.SampleBackgroundSum) > objOcvChar.BackgroundSumTolerance))
            {
                if ((m_intFailMask & 0x40) > 0)
                    blnBackgroundFail = true;
                //intDiagnostic |= 0x40;
                //if (blnReturnOnceFail)
                //    return intDiagnostic;
            }

            if ((Math.Abs(objOcvChar.TemplateForegroundArea - objOcvChar.SampleForegroundArea) > objOcvChar.ForegroundAreaTolerance) ||
                (Math.Abs(objOcvChar.TemplateForegroundSum - objOcvChar.SampleForegroundSum) > objOcvChar.ForegroundSumTolerance))
            {
                if ((m_intFailMask & 0x80) > 0)
                    blnForegroundFail = true;
                //intDiagnostic |= 0x80;
                //if (blnReturnOnceFail)
                //    return intDiagnostic;
            }

            if (blnBackgroundFail && blnForegroundFail)
            {
                if ((((float)objOcvChar.TemplateBackgroundArea - Math.Abs((float)objOcvChar.TemplateBackgroundArea - (float)objOcvChar.SampleBackgroundArea)) / (float)objOcvChar.TemplateBackgroundArea * 100) <
                    (((float)objOcvChar.TemplateForegroundArea - Math.Abs((float)objOcvChar.TemplateForegroundArea - (float)objOcvChar.SampleForegroundArea)) / (float)objOcvChar.TemplateForegroundArea * 100))
                {
                    // Get Sample Background Area Score
                    fTempScore = ((float)objOcvChar.TemplateBackgroundArea - Math.Abs((float)objOcvChar.TemplateBackgroundArea - (float)objOcvChar.SampleBackgroundArea)) / (float)objOcvChar.TemplateBackgroundArea * 100;
                    if (fTempScore < m_fAreaFailScore2)
                        m_fAreaFailScore2 = fTempScore;

                    intDiagnostic |= 0x40;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
                else
                {
                    // Get Sample Foreground Area Score
                    fTempScore = ((float)objOcvChar.TemplateForegroundArea - Math.Abs((float)objOcvChar.TemplateForegroundArea - (float)objOcvChar.SampleForegroundArea)) / (float)objOcvChar.TemplateForegroundArea * 100;
                    if (fTempScore < m_fAreaFailScore2)
                        m_fAreaFailScore2 = fTempScore;

                    intDiagnostic |= 0x80;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
            }
            else if (blnBackgroundFail)
            {
                // Get Sample Background Area Score
                fTempScore = ((float)objOcvChar.TemplateBackgroundArea - Math.Abs((float)objOcvChar.TemplateBackgroundArea - (float)objOcvChar.SampleBackgroundArea)) / (float)objOcvChar.TemplateBackgroundArea * 100;
                if (fTempScore < m_fAreaFailScore2)
                    m_fAreaFailScore2 = fTempScore;

                intDiagnostic |= 0x40;
                if (blnReturnOnceFail)
                    return intDiagnostic;
            }
            else if (blnForegroundFail)
            {
                // Get Sample Foreground Area Score
                fTempScore = ((float)objOcvChar.TemplateForegroundArea - Math.Abs((float)objOcvChar.TemplateForegroundArea - (float)objOcvChar.SampleForegroundArea)) / (float)objOcvChar.TemplateForegroundArea * 100;
                if (fTempScore < m_fAreaFailScore2)
                    m_fAreaFailScore2 = fTempScore;

                intDiagnostic |= 0x80;
                if (blnReturnOnceFail)
                    return intDiagnostic;
            }

            return intDiagnostic;
        }

        private int GetOcvTextDiagnostic(EOCVText objOcvText, bool blnReturnOnceFail)
        {
            int intDiagnostic = 0;
            float fTempScore = 100;
            //if (Math.Abs(objOcvText.TemplateLocationScore - objOcvText.SampleLocationScore) > objOcvText.LocationScoreTolerance)
            if (objOcvText.TemplateLocationScore > (objOcvText.SampleLocationScore + objOcvText.LocationScoreTolerance))
            {
                if ((m_intFailMask & 0x01) > 0)
                {
                    // Get Sample Location Score
                    //fTempScore = (objOcvText.TemplateLocationScore - Math.Abs(objOcvText.TemplateLocationScore - objOcvText.SampleLocationScore)) / objOcvText.TemplateLocationScore * 100;
                    if (objOcvText.TemplateLocationScore > objOcvText.SampleLocationScore)
                        fTempScore = objOcvText.SampleLocationScore / objOcvText.TemplateLocationScore * 100;
                    else
                        fTempScore = 100;
                    if (fTempScore < m_fLocationFailScore)
                        m_fLocationFailScore = fTempScore;

                    intDiagnostic |= 0x01;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
            }

            if ((objOcvText.Correlation <= 0) || ((1 - objOcvText.Correlation) > objOcvText.CorrelationTolerance))
            {
                if ((m_intFailMask & 0x02) > 0)
                {
                    // Get Sample Correlation Score
                    if (objOcvText.Correlation <= 0)
                        fTempScore = 0;
                    else
                        fTempScore = objOcvText.Correlation * 100;
                    if (fTempScore < m_fCorrelationFailScore)
                        m_fCorrelationFailScore = fTempScore;

                    intDiagnostic |= 0x02;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
            }

            bool blnBackgroundFail = false;
            bool blnForegroundFail = false;
            if ((Math.Abs(objOcvText.TemplateBackgroundArea - objOcvText.SampleBackgroundArea) > objOcvText.BackgroundAreaTolerance) ||
                (Math.Abs(objOcvText.TemplateBackgroundSum - objOcvText.SampleBackgroundSum) > objOcvText.BackgroundSumTolerance))
            {
                if ((m_intFailMask & 0x04) > 0)
                    blnBackgroundFail = true;
                //intDiagnostic |= 0x04;
                //if (blnReturnOnceFail)
                //    return intDiagnostic;
            }

            if ((Math.Abs(objOcvText.TemplateForegroundArea - objOcvText.SampleForegroundArea) > objOcvText.ForegroundAreaTolerance) ||
                (Math.Abs(objOcvText.TemplateForegroundSum - objOcvText.SampleForegroundSum) > objOcvText.ForegroundSumTolerance))
            {
                if ((m_intFailMask & 0x08) > 0)
                    blnForegroundFail = true;
                //intDiagnostic |= 0x08;
                //if (blnReturnOnceFail)
                //    return intDiagnostic;
            }

            if (blnBackgroundFail && blnForegroundFail)
            {
                if ((((float)objOcvText.TemplateBackgroundArea - Math.Abs((float)objOcvText.TemplateBackgroundArea - (float)objOcvText.SampleBackgroundArea)) / (float)objOcvText.TemplateBackgroundArea * 100) <
                    (((float)objOcvText.TemplateForegroundArea - Math.Abs((float)objOcvText.TemplateForegroundArea - (float)objOcvText.SampleForegroundArea)) / (float)objOcvText.TemplateForegroundArea * 100))
                {
                    // Get Sample Background Area Score
                    fTempScore = ((float)objOcvText.TemplateBackgroundArea - Math.Abs((float)objOcvText.TemplateBackgroundArea - (float)objOcvText.SampleBackgroundArea)) / (float)objOcvText.TemplateBackgroundArea * 100;
                    if (fTempScore < m_fAreaFailScore)
                        m_fAreaFailScore = fTempScore;

                    intDiagnostic |= 0x04;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
                else
                {
                    // Get Sample Foreground Area Score
                    fTempScore = ((float)objOcvText.TemplateForegroundArea - Math.Abs((float)objOcvText.TemplateForegroundArea - (float)objOcvText.SampleForegroundArea)) / (float)objOcvText.TemplateForegroundArea * 100;
                    if (fTempScore < m_fAreaFailScore)
                        m_fAreaFailScore = fTempScore;

                    intDiagnostic |= 0x08;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
            }
            else if (blnBackgroundFail)
            {
                // Get Sample Background Area Score
                fTempScore = ((float)objOcvText.TemplateBackgroundArea - Math.Abs((float)objOcvText.TemplateBackgroundArea - (float)objOcvText.SampleBackgroundArea)) / (float)objOcvText.TemplateBackgroundArea * 100;
                if (fTempScore < m_fAreaFailScore)
                    m_fAreaFailScore = fTempScore;

                intDiagnostic |= 0x04;
                if (blnReturnOnceFail)
                    return intDiagnostic;
            }
            else if (blnForegroundFail)
            {
                // Get Sample Foreground Area Score

                fTempScore = ((float)objOcvText.TemplateForegroundArea - Math.Abs((float)objOcvText.TemplateForegroundArea - (float)objOcvText.SampleForegroundArea)) / (float)objOcvText.TemplateForegroundArea * 100;
                if (fTempScore < m_fAreaFailScore)
                    m_fAreaFailScore = fTempScore;

                intDiagnostic |= 0x08;
                if (blnReturnOnceFail)
                    return intDiagnostic;
            }

            return intDiagnostic;
        }

        private int GetOcvTextDiagnostic2(EOCVText objOcvText, bool blnReturnOnceFail)
        {
            int intDiagnostic = 0;
            float fTempScore = 100;
            //if (Math.Abs(objOcvText.TemplateLocationScore - objOcvText.SampleLocationScore) > objOcvText.LocationScoreTolerance)
            if (objOcvText.TemplateLocationScore > (objOcvText.SampleLocationScore + objOcvText.LocationScoreTolerance))
            {
                if ((m_intFailMask & 0x01) > 0)
                {
                    // Get Sample Location Score
                    //fTempScore = (objOcvText.TemplateLocationScore - Math.Abs(objOcvText.TemplateLocationScore - objOcvText.SampleLocationScore)) / objOcvText.TemplateLocationScore * 100;
                    if (objOcvText.TemplateLocationScore > objOcvText.SampleLocationScore)
                        fTempScore = objOcvText.SampleLocationScore / objOcvText.TemplateLocationScore * 100;
                    else
                        fTempScore = 100;
                    if (fTempScore < m_fLocationFailScore2)
                        m_fLocationFailScore2 = fTempScore;

                    intDiagnostic |= 0x01;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
            }

            if ((objOcvText.Correlation <= 0) || ((1 - objOcvText.Correlation) > objOcvText.CorrelationTolerance))
            {
                if ((m_intFailMask & 0x02) > 0)
                {
                    // Get Sample Correlation Score
                    if (objOcvText.Correlation <= 0)
                        fTempScore = 0;
                    else
                        fTempScore = objOcvText.Correlation * 100;
                    if (fTempScore < m_fCorrelationFailScore2)
                        m_fCorrelationFailScore2 = fTempScore;

                    intDiagnostic |= 0x02;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
            }

            bool blnBackgroundFail = false;
            bool blnForegroundFail = false;
            if ((Math.Abs(objOcvText.TemplateBackgroundArea - objOcvText.SampleBackgroundArea) > objOcvText.BackgroundAreaTolerance) ||
                (Math.Abs(objOcvText.TemplateBackgroundSum - objOcvText.SampleBackgroundSum) > objOcvText.BackgroundSumTolerance))
            {
                if ((m_intFailMask & 0x04) > 0)
                    blnBackgroundFail = true;
                //intDiagnostic |= 0x04;
                //if (blnReturnOnceFail)
                //    return intDiagnostic;
            }

            if ((Math.Abs(objOcvText.TemplateForegroundArea - objOcvText.SampleForegroundArea) > objOcvText.ForegroundAreaTolerance) ||
                (Math.Abs(objOcvText.TemplateForegroundSum - objOcvText.SampleForegroundSum) > objOcvText.ForegroundSumTolerance))
            {
                if ((m_intFailMask & 0x08) > 0)
                    blnForegroundFail = true;
                //intDiagnostic |= 0x08;
                //if (blnReturnOnceFail)
                //    return intDiagnostic;
            }

            if (blnBackgroundFail && blnForegroundFail)
            {
                if ((((float)objOcvText.TemplateBackgroundArea - Math.Abs((float)objOcvText.TemplateBackgroundArea - (float)objOcvText.SampleBackgroundArea)) / (float)objOcvText.TemplateBackgroundArea * 100) <
                    (((float)objOcvText.TemplateForegroundArea - Math.Abs((float)objOcvText.TemplateForegroundArea - (float)objOcvText.SampleForegroundArea)) / (float)objOcvText.TemplateForegroundArea * 100))
                {
                    // Get Sample Background Area Score
                    fTempScore = ((float)objOcvText.TemplateBackgroundArea - Math.Abs((float)objOcvText.TemplateBackgroundArea - (float)objOcvText.SampleBackgroundArea)) / (float)objOcvText.TemplateBackgroundArea * 100;
                    if (fTempScore < m_fAreaFailScore2)
                        m_fAreaFailScore2 = fTempScore;

                    intDiagnostic |= 0x04;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
                else
                {
                    // Get Sample Foreground Area Score
                    fTempScore = ((float)objOcvText.TemplateForegroundArea - Math.Abs((float)objOcvText.TemplateForegroundArea - (float)objOcvText.SampleForegroundArea)) / (float)objOcvText.TemplateForegroundArea * 100;
                    if (fTempScore < m_fAreaFailScore2)
                        m_fAreaFailScore2 = fTempScore;

                    intDiagnostic |= 0x08;
                    if (blnReturnOnceFail)
                        return intDiagnostic;
                }
            }
            else if (blnBackgroundFail)
            {
                // Get Sample Background Area Score
                fTempScore = ((float)objOcvText.TemplateBackgroundArea - Math.Abs((float)objOcvText.TemplateBackgroundArea - (float)objOcvText.SampleBackgroundArea)) / (float)objOcvText.TemplateBackgroundArea * 100;
                if (fTempScore < m_fAreaFailScore2)
                    m_fAreaFailScore2 = fTempScore;

                intDiagnostic |= 0x04;
                if (blnReturnOnceFail)
                    return intDiagnostic;
            }
            else if (blnForegroundFail)
            {
                // Get Sample Foreground Area Score
                fTempScore = ((float)objOcvText.TemplateForegroundArea - Math.Abs((float)objOcvText.TemplateForegroundArea - (float)objOcvText.SampleForegroundArea)) / (float)objOcvText.TemplateForegroundArea * 100;
                if (fTempScore < m_fAreaFailScore2)
                    m_fAreaFailScore2 = fTempScore;

                intDiagnostic |= 0x08;
                if (blnReturnOnceFail)
                    return intDiagnostic;
            }

            return intDiagnostic;
        }

        private int GetOcvMatchCharNumber(float intBlobsCenterX, float intBlobsCenterY)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intCenterX, intCenterY;
            int intBlobsNumber = -1;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intBlobsNumber++;
                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);

                    // 
                    if ((intBlobsCenterX <= (intCenterX + 10)) && (intBlobsCenterY <= (intCenterY + 10)) &&
                    (intBlobsCenterX >= (intCenterX - 10)) && (intBlobsCenterY >= (intCenterY - 10)))
                    {
                        return intBlobsNumber;
                    }
                }
            }
            return -1;
        }

        public float GetOcvScore(int intCharNo)
        {
            int intNumTextChars = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intX, intY;
            int intCharsCount = 0;
            float fScoreValue = 101;
            float fTempScoreValue;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    if ((int)m_arrCharNo[intCharsCount] == intCharNo)
                    {
                        intX = intY = 0;
                        m_Ocv.GetTextCharPoint(tx, ch, out intX, out intY, 0, 0);
                        m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                        m_Ocv.SelectSampleTextsChars(intX, intY, 1, 1);
                        m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        if (m_OcvChar.TemplateBackgroundArea == -1)
                        {
                            intX = intY = 0;
                            m_Ocv.GetTextCharPoint(tx, ch, out intX, out intY, -1, -1);
                            m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                            m_Ocv.SelectSampleTextsChars(intX+1, intY+1, 1, 1);
                            m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        }
                        if (m_OcvChar.TemplateBackgroundArea == -1)
                        {
                            intX = intY = 0;
                            m_Ocv.GetTextCharPoint(tx, ch, out intX, out intY, 1, 1);
                            m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                            m_Ocv.SelectSampleTextsChars(intX-1, intY-1, 1, 1);
                            m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        }
                        if (m_OcvChar.TemplateBackgroundArea == -1)
                        {
                            intX = intY = 0;
                            m_Ocv.GetTextCharPoint(tx, ch, out intX, out intY, -1, 1);
                            m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                            m_Ocv.SelectSampleTextsChars(intX + 1, intY - 1, 1, 1);
                            m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        }
                        if (m_OcvChar.TemplateBackgroundArea == -1)
                        {
                            intX = intY = 0;
                            m_Ocv.GetTextCharPoint(tx, ch, out intX, out intY, 1, -1);
                            m_Ocv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                            m_Ocv.SelectSampleTextsChars(intX - 1, intY + 1, 1, 1);
                            m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        }

                        if (m_OcvChar.TemplateBackgroundArea != -1)
                        {
                            //fTempScoreValue = (m_OcvChar.TemplateLocationScore - Math.Abs(m_OcvChar.TemplateLocationScore - m_OcvChar.SampleLocationScore)) / m_OcvChar.TemplateLocationScore * 100;
                            if (m_OcvChar.TemplateLocationScore > m_OcvChar.SampleLocationScore)
                                fTempScoreValue = m_OcvChar.SampleLocationScore / m_OcvChar.TemplateLocationScore * 100;
                            else
                                fTempScoreValue = 100;
                            if ((fTempScoreValue < fScoreValue) && (fTempScoreValue < 25))
                                fScoreValue = fTempScoreValue;

                            if (m_arrStatistics.Count > 0)
                            {
                                fTempScoreValue = ((float)m_OcvChar.TemplateBackgroundArea - Math.Abs((float)m_OcvChar.TemplateBackgroundArea - (float)m_OcvChar.SampleBackgroundArea)) / (float)m_OcvChar.TemplateBackgroundArea * 100;
                                if ((fTempScoreValue * ((StatisticInfo)m_arrStatistics[intCharNo]).fBgAreaFac) < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                                fTempScoreValue = ((float)m_OcvChar.TemplateForegroundArea - Math.Abs((float)m_OcvChar.TemplateForegroundArea - (float)m_OcvChar.SampleForegroundArea)) / (float)m_OcvChar.TemplateForegroundArea * 100;
                                if ((fTempScoreValue * ((StatisticInfo)m_arrStatistics[intCharNo]).fFgAreaFac) < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                                fTempScoreValue = (m_OcvChar.TemplateBackgroundSum - Math.Abs(m_OcvChar.TemplateBackgroundSum - m_OcvChar.SampleBackgroundSum)) / m_OcvChar.TemplateBackgroundSum * 100;
                                if ((fTempScoreValue * ((StatisticInfo)m_arrStatistics[intCharNo]).fBgSumFac) < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                                fTempScoreValue = (m_OcvChar.TemplateForegroundSum - Math.Abs(m_OcvChar.TemplateForegroundSum - m_OcvChar.SampleForegroundSum)) / m_OcvChar.TemplateForegroundSum * 100;
                                if ((fTempScoreValue * ((StatisticInfo)m_arrStatistics[intCharNo]).fFgSumFac) < fScoreValue)
                                    fScoreValue = fTempScoreValue;
                            }
                            else
                            {
                                fTempScoreValue = ((float)m_OcvChar.TemplateBackgroundArea - Math.Abs((float)m_OcvChar.TemplateBackgroundArea - (float)m_OcvChar.SampleBackgroundArea) / m_fAreaScoreFactor) / (float)m_OcvChar.TemplateBackgroundArea * 100;
                                if (fTempScoreValue < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                                fTempScoreValue = ((float)m_OcvChar.TemplateForegroundArea - Math.Abs((float)m_OcvChar.TemplateForegroundArea - (float)m_OcvChar.SampleForegroundArea) / m_fAreaScoreFactor) / (float)m_OcvChar.TemplateForegroundArea * 100;
                                if (fTempScoreValue < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                                fTempScoreValue = (m_OcvChar.TemplateBackgroundSum - Math.Abs(m_OcvChar.TemplateBackgroundSum - m_OcvChar.SampleBackgroundSum) / m_fAreaScoreFactor) / m_OcvChar.TemplateBackgroundSum * 100;
                                if (fTempScoreValue < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                                fTempScoreValue = (m_OcvChar.TemplateForegroundSum - Math.Abs(m_OcvChar.TemplateForegroundSum - m_OcvChar.SampleForegroundSum) / m_fAreaScoreFactor) / m_OcvChar.TemplateForegroundSum * 100;
                                if (fTempScoreValue < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                            }
                            if (m_OcvChar.Correlation <= 0)
                                fTempScoreValue = 0;
                            else
                                fTempScoreValue = m_OcvChar.Correlation * 100;
                            if (fTempScoreValue < fScoreValue)
                                fScoreValue = fTempScoreValue;
                        }

                    }
                    intCharsCount++;
                }
            }

            if (fScoreValue == 101)
                return 0;
            else 
                return fScoreValue;
        }

        public float GetBinarizedOcvScore(int intCharNo)
        {
            int intNumTextChars = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intX, intY;
            int intCharsCount = 0;
            float fScoreValue = 101;
            float fTempScoreValue;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_BinaOcv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_BinaOcv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    if ((int)m_arrCharNo[intCharsCount] == intCharNo)
                    {
                        intX = intY = 0;
                        m_BinaOcv.GetTextCharPoint(tx, ch, out intX, out intY, 0, 0);
                        m_BinaOcv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                        m_BinaOcv.SelectSampleTextsChars(intX, intY, 1, 1);
                        m_BinaOcv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                        if (m_OcvChar.TemplateBackgroundArea == -1)
                        {
                            intX = intY = 0;
                            m_BinaOcv.GetTextCharPoint(tx, ch, out intX, out intY, -1, -1);
                            m_BinaOcv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTextsChars(intX+1, intY+1, 1, 1);
                            m_BinaOcv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        }
                        if (m_OcvChar.TemplateBackgroundArea == -1)
                        {
                            intX = intY = 0;
                            m_BinaOcv.GetTextCharPoint(tx, ch, out intX, out intY, 1, 1);
                            m_BinaOcv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTextsChars(intX-1, intY-1, 1, 1);
                            m_BinaOcv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        }
                        if (m_OcvChar.TemplateBackgroundArea == -1)
                        {
                            intX = intY = 0;
                            m_BinaOcv.GetTextCharPoint(tx, ch, out intX, out intY, -1, 1);
                            m_BinaOcv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTextsChars(intX + 1, intY - 1, 1, 1);
                            m_BinaOcv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        }
                        if (m_OcvChar.TemplateBackgroundArea == -1)
                        {
                            intX = intY = 0;
                            m_BinaOcv.GetTextCharPoint(tx, ch, out intX, out intY, 1, -1);
                            m_BinaOcv.SelectSampleTextsChars(0, 0, 640, 480, ESelectionFlag.Any, ESelectionFlag.True);
                            m_BinaOcv.SelectSampleTextsChars(intX - 1, intY + 1, 1, 1);
                            m_BinaOcv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        }


                        if (m_OcvChar.TemplateBackgroundArea != -1)
                        {
                            //fTempScoreValue = (m_OcvChar.TemplateLocationScore - Math.Abs(m_OcvChar.TemplateLocationScore - m_OcvChar.SampleLocationScore)) / m_OcvChar.TemplateLocationScore * 100;
                            if (m_OcvChar.TemplateLocationScore > m_OcvChar.SampleLocationScore)
                                fTempScoreValue = m_OcvChar.SampleLocationScore / m_OcvChar.TemplateLocationScore * 100;
                            else
                                fTempScoreValue = 100;
                            if ((fTempScoreValue < fScoreValue) && (fTempScoreValue < 25))
                                fScoreValue = fTempScoreValue;

                            if (m_arrStatistics.Count > 0)
                            {
                                fTempScoreValue = ((float)m_OcvChar.TemplateBackgroundArea - Math.Abs((float)m_OcvChar.TemplateBackgroundArea - (float)m_OcvChar.SampleBackgroundArea)) / (float)m_OcvChar.TemplateBackgroundArea * 100;
                                if ((fTempScoreValue * ((StatisticInfo)m_arrStatistics[intCharNo]).fBgAreaFac) < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                                fTempScoreValue = ((float)m_OcvChar.TemplateForegroundArea - Math.Abs((float)m_OcvChar.TemplateForegroundArea - (float)m_OcvChar.SampleForegroundArea)) / (float)m_OcvChar.TemplateForegroundArea * 100;
                                if ((fTempScoreValue * ((StatisticInfo)m_arrStatistics[intCharNo]).fFgAreaFac) < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                                fTempScoreValue = (m_OcvChar.TemplateBackgroundSum - Math.Abs(m_OcvChar.TemplateBackgroundSum - m_OcvChar.SampleBackgroundSum)) / m_OcvChar.TemplateBackgroundSum * 100;
                                if ((fTempScoreValue * ((StatisticInfo)m_arrStatistics[intCharNo]).fBgSumFac) < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                                fTempScoreValue = (m_OcvChar.TemplateForegroundSum - Math.Abs(m_OcvChar.TemplateForegroundSum - m_OcvChar.SampleForegroundSum)) / m_OcvChar.TemplateForegroundSum * 100;
                                if ((fTempScoreValue * ((StatisticInfo)m_arrStatistics[intCharNo]).fFgSumFac) < fScoreValue)
                                    fScoreValue = fTempScoreValue;
                            }
                            else
                            {
                                fTempScoreValue = ((float)m_OcvChar.TemplateBackgroundArea - Math.Abs((float)m_OcvChar.TemplateBackgroundArea - (float)m_OcvChar.SampleBackgroundArea) / m_fAreaScoreFactor) / (float)m_OcvChar.TemplateBackgroundArea * 100;
                                if (fTempScoreValue < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                                fTempScoreValue = ((float)m_OcvChar.TemplateForegroundArea - Math.Abs((float)m_OcvChar.TemplateForegroundArea - (float)m_OcvChar.SampleForegroundArea) / m_fAreaScoreFactor) / (float)m_OcvChar.TemplateForegroundArea * 100;
                                if (fTempScoreValue < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                                fTempScoreValue = (m_OcvChar.TemplateBackgroundSum - Math.Abs(m_OcvChar.TemplateBackgroundSum - m_OcvChar.SampleBackgroundSum) / m_fAreaScoreFactor) / m_OcvChar.TemplateBackgroundSum * 100;
                                if (fTempScoreValue < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                                fTempScoreValue = (m_OcvChar.TemplateForegroundSum - Math.Abs(m_OcvChar.TemplateForegroundSum - m_OcvChar.SampleForegroundSum) / m_fAreaScoreFactor) / m_OcvChar.TemplateForegroundSum * 100;
                                if (fTempScoreValue < fScoreValue)
                                    fScoreValue = fTempScoreValue;

                            }
                            if (m_OcvChar.Correlation <= 0)
                                fTempScoreValue = 0;
                            else
                                fTempScoreValue = m_OcvChar.Correlation * 100;
                            if (fTempScoreValue < fScoreValue)
                                fScoreValue = fTempScoreValue;
                        }

                    }
                    intCharsCount++;
                }
            }

            if (fScoreValue == 101)
                return 0;
            else
                return fScoreValue;
        }

        public void GetShiftTextValue(ROI objROI, out int intTextWidth, out int intTextHeight)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intStartX, intStartY, intEndX, intEndY;
            int intWidth, intHeight;
            intTextWidth = intTextHeight = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                intStartX = intStartY = intEndX = intEndY = 0;
                m_Ocv.GetTextPoint(tx, out intStartX, out intStartY, -1, -1);
                m_Ocv.GetTextPoint(tx, out intEndX, out intEndY, 1, 1);
                intWidth = intEndX - intStartX;
                intHeight = intEndY - intStartY;
                if (intWidth > intTextWidth)
                    intTextWidth = intWidth;
                if (intHeight > intTextHeight)
                    intTextHeight = intHeight;
            }

            intTextWidth = (objROI.ref_ROIWidth - intTextWidth) / 2;
            intTextHeight = (objROI.ref_ROIHeight - intTextHeight) / 2;
        }

        public int GetStatisticsCount()
        {
            if (m_intStatisticCount == 0)
                return m_intStatisticCount;
            else
                return m_intStatisticCount - 1;
        }

        public void GetTextCharDiagnosticResult(bool blnReturnOnceFail, int intUnitNo)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            if (intUnitNo == 0)
            {
                m_fLocationFailScore = 100;
                m_fAreaFailScore = 100;
                m_fCorrelationFailScore = 100;
            }
            else
            {
                m_fLocationFailScore2 = 100;
                m_fAreaFailScore2 = 100;
                m_fCorrelationFailScore2 = 100;
            }
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                // Get OcvText
                m_Ocv.GetTextParameters(m_OcvText, tx);
                // Get EOCVText Diagnostic result
                if (intUnitNo == 0)
                    m_intTextDiagnostic[tx] = GetOcvTextDiagnostic(m_OcvText, blnReturnOnceFail);
                else
                    m_intTextDiagnostic[tx] = GetOcvTextDiagnostic2(m_OcvText, blnReturnOnceFail);

                // EOCVText Diagnostic return fail
                if (m_intTextDiagnostic[tx] > 0)
                {
                    // Test Once Mode
                    if (blnReturnOnceFail)
                    {
                        if ((m_intMarkFailMask & m_intTextDiagnostic[tx]) > 0)
                        {
                            m_blnTextFail[tx] = true;
                            m_intMarkFailMask |= m_intTextDiagnostic[tx];
                            return;
                        }
                    }
                    // Test All Mode
                    else
                    {
                        m_blnTextFail[tx] = true;
                        m_intMarkFailMask |= m_intTextDiagnostic[tx];
                    }
                }
            }
        }

        public float GetTextSampleScore(int intTextIndex)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX = 0, intCenterY = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                if (tx == intTextIndex)
                {
                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTexts(0, 0, 640, 480, ESelectionFlag.True);
                    m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                    return m_OcvText.SampleLocationScore;
                }
            }
            return 0;
        }

        public System.Drawing.Point GetTextStartPoint(int intTextIndex)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intX = 0, intY = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                if (tx == intTextIndex)
                {
                    intX = intY = 0;
                    m_Ocv.GetTextPoint(tx, out intX, out intY, -1, -1);
                    break;
                }
            }

            return new System.Drawing.Point(intX, intY);
        }

        public System.Drawing.Point GetTextEndPoint(int intTextIndex)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intX = 0, intY = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                if (tx == intTextIndex)
                {
                    intX = intY = 0;
                    m_Ocv.GetTextPoint(tx, out intX, out intY, 1, 1);
                    break;
                }
            }

            return new System.Drawing.Point(intX, intY);
        }

        private bool MatchBlobsCentreXY(float intBlobsCenterX, float intBlobsCenterY)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intCenterX, intCenterY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                //m_Ocv.GetTextPoint(tx, out intX, out intY, 0, 0);
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                //m_Ocv.GetTextPoint(tx, out intX, out intY, 0, 0);
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);

                    if ((intBlobsCenterX <= (intCenterX + 10)) && (intBlobsCenterY <= (intCenterY + 10)) &&
                    (intBlobsCenterX >= (intCenterX - 10)) && (intBlobsCenterY >= (intCenterY - 10)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Inspect(ROI objROI, int intThresholdValue)
        {
#if (Debug_2_12 || Release_2_12)
            m_Ocv.Inspect(objROI.ref_ROI, (uint)intThresholdValue);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_Ocv.Inspect(objROI.ref_ROI, intThresholdValue);
#endif

        }

        public bool InpectMark(ImageDrawing objRotatedImage, ROI objROI, bool blnWhiteOnBlack, bool blnBinarizedLocMode, bool blnWantRemoveBorderMode, bool blnWantGroupExtraMark, int intRoiOrgX, int intRoiOrgY, bool blnReturnOnceFail, bool blnInspectBlobs)
        {
            m_blnNewInspection = true;
            bool blnInspectResult = true;
            m_blnDefectFail = new bool[0];

            for (int i = 0; i < 10; i++)
            {
                m_blnTextFail[i] = false;
                m_intTextDiagnostic[i] = 0;
            }

            for (int i = 0; i < 100; i++)
            {
                m_blnCharFail[i] = false;
                m_intCharDiagnostic[i] = 0;
            }

            // Get threshold value
            int intThresholdValue;
            if (m_intThresholdValue == -4)
                intThresholdValue = GetAutoThresholdValue(objROI, 3);
            else
                intThresholdValue = m_intThresholdValue;
#if (Debug_2_12 || Release_2_12)
            // Inspect OCV
            m_Ocv.Inspect(objROI.ref_ROI, (uint)intThresholdValue);
            m_BinaOcv.Inspect(objROI.ref_ROI, (uint)intThresholdValue);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Inspect OCV
            m_Ocv.Inspect(objROI.ref_ROI, intThresholdValue);
            m_BinaOcv.Inspect(objROI.ref_ROI, intThresholdValue);

#endif

            #region Check Text Shifted
            if ((m_intFailMask & 0x2000) > 0)
            {
                if (!CheckOcvTextShifted(intRoiOrgX, intRoiOrgY, intRoiOrgX + objROI.ref_ROIWidth, intRoiOrgY + objROI.ref_ROIHeight))
                {
                    m_intMarkFailMask |= 0x2000;
                    if (blnInspectResult)
                        blnInspectResult = false;
                    if (blnReturnOnceFail)
                        return false;
                }
            }
            #endregion

            #region Check OCV Text Diagnostics
            if (DefineOCVDiagnostics())
            {
                if (GetDiagnosticTextNotFound())
                {
                    m_intMarkFailMask |= 0x01;
                    if (blnInspectResult)
                        blnInspectResult = false;
                    if (blnReturnOnceFail)
                        return false;
                }

                if (GetDiagnosticTextMismatch())
                {
                    m_intMarkFailMask |= 0x02;
                    if (blnInspectResult)
                        blnInspectResult = false;
                    if (blnReturnOnceFail)
                        return false;
                }

                if (GetDiagnosticTextOverprint())
                {
                    m_intMarkFailMask |= 0x04;
                    if (blnInspectResult)
                        blnInspectResult = false;
                    if (blnReturnOnceFail)
                        return false;
                }

                if (GetDiagnosticTextUnderprint())
                {
                    m_intMarkFailMask |= 0x08;
                    if (blnInspectResult)
                        blnInspectResult = false;
                    if (blnReturnOnceFail)
                        return false;
                }
            }
            #endregion

            #region Check Score
            int intCharSetValue;
            float fGradientScore;
            float fBinarizedScore;
            m_intScoreMode = new int[m_intCharsCount];
            m_arrSampleOCV.Clear();
            for (int i = 0; i < m_intCharsCount; i++)
            {
                intCharSetValue = GetCharSetting(i);
                fGradientScore = GetOcvScore(i);
                fBinarizedScore = GetBinarizedOcvScore(i);
                // Keep result mode
                if ((fGradientScore >= fBinarizedScore) || ((fGradientScore < 101) && (fBinarizedScore == 101)))
                    m_intScoreMode[i] = 0;
                else
                    m_intScoreMode[i] = 1;

                // Set pass/fail status
                if ((intCharSetValue > fGradientScore) &&
                    (intCharSetValue > fBinarizedScore))
                {
                    for (int j = 0; j < m_arrCharNo.Count; j++)
                    {
                        if ((int)m_arrCharNo[j] == i)
                        {
                            m_blnCharFail[j] = true;
                            break;
                        }
                    }

                    m_intMarkFailMask |= 0x20;
                    if (blnInspectResult)
                        blnInspectResult = false;
                    if (blnReturnOnceFail)
                        return false;
                }

                // Save inspection record
                //if (m_intScoreMode[i] == 0)
                //{
                //    GetSampleOcv(i);
                //}
                //else
                //{
                //    GetBinarizeSampleOcv(i);
                //}
            }
            #endregion

            if (objROI.ref_ROI.Width == 0)
            {
                m_strErrorMessage = "OCV BlobsInspect : No ROI loaded!";
                return false;
            }

            m_blobs.RemoveAllObjects();
#if (Debug_2_12 || Release_2_12)
            // Set threshold value
            m_blobs.SetThreshold((uint)intThresholdValue);
            m_blobs.Connexity = EConnexity.Connexity4;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Set threshold value
            m_blobs.SetThreshold(intThresholdValue);
            m_blobs.Connexity = EConnexity.Connexity4;

#endif

            // Set white black class
            if (blnWhiteOnBlack)
            {
                m_blobs.BlackClass = 0;
                m_blobs.WhiteClass = 1;
            }
            else
            {
                m_blobs.BlackClass = 1;
                m_blobs.WhiteClass = 0;
            }

            m_blobs.BuildObjects(objROI.ref_ROI);

            try
            {
                m_blobs.AnalyseObjects(ELegacyFeature.Area, ELegacyFeature.ObjectNumber, ELegacyFeature.GravityCenterX, ELegacyFeature.GravityCenterY, ELegacyFeature.LimitCenterX, ELegacyFeature.LimitCenterY, ELegacyFeature.LimitWidth, ELegacyFeature.LimitHeight, ELegacyFeature.EllipseAngle);
                m_blobs.SelectObjectsUsingFeature(ELegacyFeature.Area, 0, 1000000, ESelectOption.RemoveOutOfRange);
                if (blnWantRemoveBorderMode)
                    m_blobs.SelectObjectsUsingPosition(objROI.ref_ROI, ESelectByPosition.RemoveBorder);
                else
                    m_blobs.SelectObjectsUsingPosition(objROI.ref_ROI, ESelectByPosition.RemoveOut);
                m_blobs.SortObjectsUsingFeature(ELegacyFeature.ObjectNumber, ESortOption.Descending);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "OCV BlobsInpect : " + ex.ToString();
                m_objTrackLog.WriteLine("OCV BlobsInpect : " + ex.ToString());
                return false;
            }

            int intNoSelectedBlobs = m_blobs.NumSelectedObjects;

            if (intNoSelectedBlobs == 0)
            {
                m_strErrorMessage = "OCV BlobsInspect : No blobs object selected!";
                return false;
            }
            else
            {
                // Init/reset inspection data
                int intCharCount = GetCharsCount();
                m_intMatchState = new int[intCharCount];
                m_intTemplateMatchCount = new int[intCharCount];
                for (int i = 0; i < intCharCount; i++)
                {
                    m_intMatchState[i] = 0;
                    m_intTemplateMatchCount[i] = 0;
                }
                for (int i = 0; i < m_arrBlobsFeatures.Count; i++)
                {
                    m_stcBlobsFeatures = (BlobsFeatures)m_arrBlobsFeatures[i];
                    m_stcBlobsFeatures.blnMatch = false; // no use
                }
                m_intSampleFailResult = new int[intNoSelectedBlobs];
                for (int i = 0; i < intNoSelectedBlobs; i++)
                {
                    m_intSampleFailResult[i] = 0;
                }

                if (blnInspectBlobs)
                {
                    float fBlobsCenterX, fBlobsCenterY, fWidth, fHeight, fAngle, fGravityCenterX, fGravityCenterY;
                    int intArea;

                    #region Check Extra Mark / Joint Mark / Eccess Mark / Broken Mark / Missing Mark

                    bool blnExtraMark = false;
                    bool blnGroupExtraMark = false;
                    bool blnJointMark = false;
                    int intTotalMatch = 0;
                    int intTotalExtraMarkArea = 0;
                    m_ListEasyObject = m_blobs.FirstObjPtr;
                    for (int i = 0; i < intNoSelectedBlobs; i++)
                    {
                        fBlobsCenterX = fBlobsCenterY = fWidth = fHeight = fAngle = fGravityCenterX = fGravityCenterY = 0;
                        intArea = 0;
                        m_blobs.GetObjectFeature(ELegacyFeature.LimitCenterX, m_ListEasyObject, out fBlobsCenterX);
                        m_blobs.GetObjectFeature(ELegacyFeature.LimitCenterY, m_ListEasyObject, out fBlobsCenterY);
                        m_blobs.GetObjectFeature(ELegacyFeature.LimitWidth, m_ListEasyObject, out fWidth);
                        m_blobs.GetObjectFeature(ELegacyFeature.LimitHeight, m_ListEasyObject, out fHeight);
                        m_blobs.GetObjectFeature(ELegacyFeature.EllipseAngle, m_ListEasyObject, out fAngle);
                        m_blobs.GetObjectFeature(ELegacyFeature.GravityCenterX, m_ListEasyObject, out fGravityCenterX);
                        m_blobs.GetObjectFeature(ELegacyFeature.GravityCenterY, m_ListEasyObject, out fGravityCenterY);
                        m_blobs.GetObjectFeature(ELegacyFeature.Area, m_ListEasyObject, out intArea);

                        // Define min area
                        if (m_intMinArea < 5)
                        {
                            // Use m_intMinArea as min area if m_intMinArea < 5
                            if (intArea <= m_intMinArea)
                            {
                                m_ListEasyObject = m_blobs.GetNextObjPtr(m_ListEasyObject);
                                continue;
                            }
                        }
                        else
                        {
                            // Use 5 as min area if m_intMinArea >= 5
                            if (intArea <= 5)
                            {
                                m_ListEasyObject = m_blobs.GetNextObjPtr(m_ListEasyObject);
                                continue;
                            }
                        }

                        int intOcvMatchNumber = GetOcvAdvanceMatchCharNumber(intRoiOrgX + fBlobsCenterX, intRoiOrgY + fBlobsCenterY, fWidth, fHeight);

                        if (intOcvMatchNumber == -1)
                        {
                            // Check is in uncheck area
                            if ((m_intFailMask & 0x80) == 0)
                            {
                                if (CheckIsInUncheckArea(objROI, fGravityCenterX, fGravityCenterY))
                                {
                                    m_ListEasyObject = m_blobs.GetNextObjPtr(m_ListEasyObject);
                                    continue;
                                }
                            }

                            // Check Group Extra Mark
                            if ((m_intFailMask & 0x4000) > 0)
                            {
                                intTotalExtraMarkArea += intArea;
                                m_intSampleFailResult[i] = 1;

                                if (intTotalExtraMarkArea > m_intGroupMinArea)
                                {
                                    m_intSampleFailResult[i] = 2;

                                    if (!blnGroupExtraMark)
                                    {
                                        m_intMarkFailMask |= 0x4000;
                                        if (blnInspectResult)
                                            blnInspectResult = false;
                                        if (blnReturnOnceFail)
                                            return false;
                                        blnGroupExtraMark = true;

                                        // Upgrade result from 1 to 2
                                        for (int j = 0; j < intNoSelectedBlobs; j++)
                                        {
                                            if (m_intSampleFailResult[j] == 1)
                                                m_intSampleFailResult[j] = 2;
                                        }
                                    }
                                }
                            }

                            // Make sure object area is higher than min area and extra min area
                            if ((intArea < m_intMinArea) && (intArea < m_intExtraMinArea))
                            {
                                m_ListEasyObject = m_blobs.GetNextObjPtr(m_ListEasyObject);
                                continue;
                            }
                            // Check Extra Mark
                            else if (((m_intFailMask & 0x100) > 0) || ((m_intFailMask & 0x80) > 0))
                            {
                                m_intSampleFailResult[i] = 2;

                                if (!blnExtraMark)
                                {
                                    m_intMarkFailMask |= 0x100;
                                    if (blnInspectResult)
                                        blnInspectResult = false;
                                    if (blnReturnOnceFail)
                                        return false;
                                    blnExtraMark = true;
                                }
                            }
                        }
                        else if (intOcvMatchNumber == -2)
                        {
                            // Make sure area >= min area
                            if (intArea < m_intMinArea)
                            {
                                m_ListEasyObject = m_blobs.GetNextObjPtr(m_ListEasyObject);
                                continue;
                            }
                            // Check Joint Mark
                            else if ((m_intFailMask & 0x1000) > 0)
                            {
                                m_intSampleFailResult[i] = 2;

                                if (!blnJointMark)
                                {
                                    m_intMarkFailMask |= 0x1000;
                                    if (blnInspectResult)
                                        blnInspectResult = false;
                                    if (blnReturnOnceFail)
                                        return false;
                                    blnJointMark = true;
                                }
                            }
                        }
                        else
                        {
                            // Make sure area >= min area
                            if (intArea < m_intMinArea)
                            {
                                m_ListEasyObject = m_blobs.GetNextObjPtr(m_ListEasyObject);
                                continue;
                            }

                            m_intTemplateMatchCount[intOcvMatchNumber]++;
                            intTotalMatch++;
                            int intArrayNo = GetArrayBlobsFeaturesIndex(intOcvMatchNumber);

                            if (intArrayNo != -1)
                            {
                                // Check Excess Mark
                                if ((fWidth > (m_stcBlobsFeatures.fWidth + m_intToleranceSize)) || (fHeight > (m_stcBlobsFeatures.fHeight + m_intToleranceSize)))
                                {
                                    if ((m_intFailMask & 0x400) > 0)
                                    {
                                        m_intSampleFailResult[i] = 2;

                                        m_blnCharFail[intOcvMatchNumber] = true;
                                        m_intCharDiagnostic[intOcvMatchNumber] |= 0x400;
                                        m_intMarkFailMask |= 0x400;
                                        if (blnInspectResult)
                                            blnInspectResult = false;
                                        if (blnReturnOnceFail)
                                            return false;
                                    }
                                }
                                // Check Broken Mark
                                else if ((fWidth < (m_stcBlobsFeatures.fWidth - m_intToleranceSize)) || (fHeight < (m_stcBlobsFeatures.fHeight - m_intToleranceSize)))
                                {
                                    if ((m_intFailMask & 0x800) > 0)
                                    {
                                        m_intSampleFailResult[i] = 2;

                                        m_blnCharFail[intOcvMatchNumber] = true;
                                        m_intCharDiagnostic[intOcvMatchNumber] |= 0x800;
                                        m_intMarkFailMask |= 0x800;
                                        if (blnInspectResult)
                                            blnInspectResult = false;
                                        if (blnReturnOnceFail)
                                            return false;
                                    }
                                }
                                else if ((fAngle < (m_stcBlobsFeatures.fAngle - 5)) || (fAngle > (m_stcBlobsFeatures.fAngle + 5)))
                                {
                                    //m_arrCharFail[intOcvMatchNumber] = true;
                                    //if (!blnSkrewFail)
                                    //{
                                    //    ref_strErrorMessage += "Skrew/Shear Mark! ";
                                    //    blnSkrewFail = true;
                                    //}
                                }
                            }
                        }
                        m_ListEasyObject = m_blobs.GetNextObjPtr(m_ListEasyObject);
                    }

                    // Check Missing Mark
                    for (int i = 0; i < m_intTemplateMatchCount.Length; i++)
                    {
                        if (((m_intFailMask & 0x200) > 0) && ((m_intMarkFailMask & 0x200) == 0))
                        {
                            if (m_intTemplateMatchCount[i] == 0)
                            {
                                m_intMarkFailMask |= 0x200;
                                if (blnInspectResult)
                                    blnInspectResult = false;
                                if (blnReturnOnceFail)
                                    return false;
                            }
                        }

                        if (((m_intFailMask & 0x800) > 0) && ((m_intMarkFailMask & 0x800) == 0))
                        {
                            if (m_intTemplateMatchCount[i] > 1)
                            {
                                m_intMarkFailMask |= 0x800;
                                if (blnInspectResult)
                                    blnInspectResult = false;
                                if (blnReturnOnceFail)
                                    return false;
                            }
                        }
                    }
                    #endregion
                }


                #region Check extra mark in characters area using substract function and grey value
                if (!GetDiagnosticTextMismatch() && !GetDiagnosticTextNotFound() &&
                    !GetDiagnosticTextOverprint() && !GetDiagnosticTextUnderprint() && ((m_intFailMask & 0x8000) > 0))
                {
                    // Define learn OCV Text area as Tempate ROI                    
                    ImageDrawing objTemplateImage = new ImageDrawing();
                    m_objLearnImage.CopyTo(ref objTemplateImage);
                    m_objTemplateTextROI.AttachImage(objTemplateImage);

                    // Define sample OCV Text area as Sample ROI
                    ROI objSampleTextROI = new ROI();
                    m_objSampleImage = new ImageDrawing();
                    objRotatedImage.CopyTo(ref m_objSampleImage);
                    System.Drawing.Point pStart = GetTextStartPoint(0);
                    System.Drawing.Point pEnd = GetTextEndPoint(0);
                    objSampleTextROI.ref_ROIPositionX = pStart.X;// -intRoiOrgX;
                    objSampleTextROI.ref_ROIPositionY = pStart.Y;// -intRoiOrgY;
                    objSampleTextROI.ref_ROIWidth = pEnd.X - pStart.X;
                    objSampleTextROI.ref_ROIHeight = pEnd.Y - pStart.Y;
                    objSampleTextROI.AttachImage(objRotatedImage);

                    // Subtract Sample ROI with Template ROI
                    m_objSubtractTextROI = new ROI();
                    m_objSubtractTextROI.AttachImage(m_objSampleImage);
                    m_objSubtractTextROI.LoadROISetting(objSampleTextROI.ref_ROIPositionX, objSampleTextROI.ref_ROIPositionY,
                                                        objSampleTextROI.ref_ROIWidth, objSampleTextROI.ref_ROIHeight);
                    EasyImage.Oper(EArithmeticLogicOperation.Subtract, objSampleTextROI.ref_ROI, m_objTemplateTextROI.ref_ROI, m_objSubtractTextROI.ref_ROI);

                    // Reload ROI size and placement to same location and same size as Mark ROI
                    m_objSubtractTextROI.LoadROISetting(objROI.ref_ROI.Parent.OrgX + objROI.ref_ROIPositionX,
                                                    objROI.ref_ROI.Parent.OrgY + objROI.ref_ROIPositionY,
                                                    objROI.ref_ROIWidth, objROI.ref_ROIHeight);

                    if (m_DefectBlobs != null)
                    {
                        m_DefectBlobs.RemoveAllObjects();
                        m_DefectBlobs.Dispose();
                        m_DefectBlobs = null;
                    }
                    m_DefectBlobs = new ECodedImage();
#if (Debug_2_12 || Release_2_12)
                    m_DefectBlobs.SetThreshold((uint)intThresholdValue);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    m_DefectBlobs.SetThreshold(intThresholdValue);
#endif

                    m_DefectBlobs.Connexity = EConnexity.Connexity4;

                    // Set white black class
                    if (blnWhiteOnBlack)
                    {
                        m_DefectBlobs.BlackClass = 0;
                        m_DefectBlobs.WhiteClass = 1;
                    }
                    else
                    {
                        m_DefectBlobs.BlackClass = 1;
                        m_DefectBlobs.WhiteClass = 0;
                    }

                    m_DefectBlobs.BuildObjects(m_objSubtractTextROI.ref_ROI);

                    try
                    {
                        m_DefectBlobs.AnalyseObjects(ELegacyFeature.Area, ELegacyFeature.ObjectNumber, ELegacyFeature.GravityCenterX, ELegacyFeature.GravityCenterY, ELegacyFeature.LimitCenterX, ELegacyFeature.LimitCenterY, ELegacyFeature.LimitWidth, ELegacyFeature.LimitHeight, ELegacyFeature.EllipseAngle);
                        m_DefectBlobs.SelectObjectsUsingFeature(ELegacyFeature.Area, 0, 1000000, ESelectOption.RemoveOutOfRange);
                        m_DefectBlobs.SelectObjectsUsingPosition(m_objSubtractTextROI.ref_ROI, ESelectByPosition.RemoveOut);
                        m_DefectBlobs.SortObjectsUsingFeature(ELegacyFeature.Area, ESortOption.Descending);
                    }
                    catch
                    {
                    }

                    int intArea = 0;
                    int intStartX, intStartY, intEndX, intEndY;
                    float fBlobsCenterX, fBlobsCenterY, fWidth, fHeight;
                    fBlobsCenterX = fBlobsCenterY = fWidth = fHeight = 0;
                    intNoSelectedBlobs = m_DefectBlobs.NumSelectedObjects;
                    m_ListEasyObject = m_DefectBlobs.FirstObjPtr;
                    m_blnDefectFail = new bool[intNoSelectedBlobs];
                    for (int i = 0; i < intNoSelectedBlobs; i++)
                    {
                        m_DefectBlobs.GetObjectFeature(ELegacyFeature.LimitCenterX, m_ListEasyObject, out fBlobsCenterX);
                        m_DefectBlobs.GetObjectFeature(ELegacyFeature.LimitCenterY, m_ListEasyObject, out fBlobsCenterY);
                        m_DefectBlobs.GetObjectFeature(ELegacyFeature.LimitWidth, m_ListEasyObject, out fWidth);
                        m_DefectBlobs.GetObjectFeature(ELegacyFeature.LimitHeight, m_ListEasyObject, out fHeight);
                        m_DefectBlobs.GetObjectFeature(ELegacyFeature.Area, m_ListEasyObject, out intArea);


                        intStartX = (int)(fBlobsCenterX - fWidth / 2) + intRoiOrgX;
                        intStartY = (int)(fBlobsCenterY - fHeight / 2) + intRoiOrgY;
                        intEndX = (int)(fBlobsCenterX + fWidth / 2) + intRoiOrgX;
                        intEndY = (int)(fBlobsCenterY + fHeight / 2) + intRoiOrgY;

                        if ((intArea > m_intMinArea) && (intArea > m_intExtraMinArea) &&
                            IsInOcvCharArea(intStartX, intStartY, intEndX, intEndY))
                        {
                            m_blnDefectFail[i] = true;
                            m_intMarkFailMask |= 0x100;
                            if (blnInspectResult)
                                blnInspectResult = false;
                            if (blnReturnOnceFail)
                                return false;
                        }
                        else
                            m_blnDefectFail[i] = false;

                        m_ListEasyObject = m_DefectBlobs.GetNextObjPtr(m_ListEasyObject);
                    }
                }
                #endregion
            }

            m_blnNewInspection = false;

            return blnInspectResult;
        }

        public void InspectOcv(ROI objROI)
        {
#if (Debug_2_12 || Release_2_12)
            m_Ocv.Inspect(objROI.ref_ROI, (uint)m_intThresholdValue);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_Ocv.Inspect(objROI.ref_ROI, m_intThresholdValue);
#endif

        }

        private bool IsInOcvCharArea(int intStartX, int intStartY, int intEndX, int intEndY)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumChars;
            int intCharStartX = 0, intCharStartY = 0, intCharEndX = 0, intCharEndY = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumChars; ch++)
                {
#endif

                    // Get OCV Text start point and end point
                    m_Ocv.GetTextCharPoint(tx, ch, out intCharStartX, out intCharStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(tx, ch, out intCharEndX, out intCharEndY, 1, 1);

                    // Check is object area overlap ocv text area or not
                    if (((intStartX > intCharStartX) && (intStartX < intCharEndX) && (intStartY > intCharStartY) && (intStartY < intCharEndY)) ||
                        ((intStartX > intCharStartX) && (intStartX < intCharEndX) && (intEndY > intCharStartY) && (intEndY < intCharEndY)) ||
                        ((intEndX > intCharStartX) && (intEndX < intCharEndX) && (intStartY > intCharStartY) && (intStartY < intCharEndY)) ||
                        ((intEndX > intCharStartX) && (intEndX < intCharEndX) && (intEndY > intCharStartY) && (intEndY < intCharEndY)))
                    {
                        return true;
                    }

                    // Check is ocv text area overlap object area or not
                    if (((intCharStartX > intStartX) && (intCharStartX < intEndX) && (intCharStartY > intStartY) && (intCharStartY < intEndY)) ||
                        ((intCharStartX > intStartX) && (intCharStartX < intEndX) && (intCharEndY > intStartY) && (intCharEndY < intEndY)) ||
                        ((intCharEndX > intStartX) && (intCharEndX < intEndX) && (intCharStartY > intStartY) && (intCharStartY < intEndY)) ||
                        ((intCharEndX > intStartX) && (intCharEndX < intEndX) && (intCharEndY > intStartY) && (intCharEndY < intEndY)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsInOcvTextArea(int intStartX, int intStartY, int intEndX, int intEndY)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intTextStartX = 0, intTextStartY = 0, intTextEndX = 0, intTextEndY = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                // Get OCV Text start point and end point
                m_Ocv.GetTextPoint(tx, out intTextStartX, out intTextStartY, -1, -1);
                m_Ocv.GetTextPoint(tx, out intTextEndX, out intTextEndY, 1, 1);

                // Check is object area overlap ocv text area or not
                if (((intStartX > intTextStartX) && (intStartX < intTextEndX) && (intStartY > intTextStartY) && (intStartY < intTextEndY)) ||
                    ((intStartX > intTextStartX) && (intStartX < intTextEndX) && (intEndY > intTextStartY) && (intEndY < intTextEndY)) ||
                    ((intEndX > intTextStartX) && (intEndX < intTextEndX) && (intStartY > intTextStartY) && (intStartY < intTextEndY)) ||
                    ((intEndX > intTextStartX) && (intEndX < intTextEndX) && (intEndY > intTextStartY) && (intEndY < intTextEndY)))
                {
                    return true;
                }

                // Check is ocv text area overlap object area or not
                if (((intTextStartX > intStartX) && (intTextStartX < intEndX) && (intTextStartY > intStartY) && (intTextStartY < intEndY)) ||
                    ((intTextStartX > intStartX) && (intTextStartX < intEndX) && (intTextEndY > intStartY) && (intTextEndY < intEndY)) ||
                    ((intTextEndX > intStartX) && (intTextEndX < intEndX) && (intTextStartY > intStartY) && (intTextStartY < intEndY)) ||
                    ((intTextEndX > intStartX) && (intTextEndX < intEndX) && (intTextEndY > intStartY) && (intTextEndY < intEndY)))
                {
                    return true;
                }
            }

            return false;
        }

        public bool LearnROI(ROI objROI)
        {
            try
            {
                m_Ocv.Learn(objROI.ref_ROI, ESelectionFlag.True);
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_objTrackLog.WriteLine("OCV LearnROI : " + ex.ToString());
                return false;
            }

            return true;
        }

        public void LoadOCVFile(String strPath)
        {
            m_BinaOcv.Load(strPath);
            m_Ocv.Load(strPath);
            m_BinaOcv.LocationMode = ELocationMode.Binarized;
            m_Ocv.LocationMode = ELocationMode.Gradient;
            
        }

        public bool ModifyObjects(ROI objROI, bool blnWhiteOnBlack)
        {
            if (objROI.ref_ROI.Width == 0)
            {
                m_strErrorMessage = "No ROI Loaded!";
                return false;
            }

            m_blobs.RemoveAllObjects();
#if (Debug_2_12 || Release_2_12)
            // Set threshold value
            m_blobs.SetThreshold((uint)m_intThresholdValue);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Set threshold value
            m_blobs.SetThreshold(m_intThresholdValue);
#endif

            m_blobs.Connexity = EConnexity.Connexity4;

            // Set white black class
            if (blnWhiteOnBlack)
            {
                m_blobs.BlackClass = 0;
                m_blobs.WhiteClass = 1;
                m_bw.Value = 255;
            }
            else
            {
                m_blobs.BlackClass = 1;
                m_blobs.WhiteClass = 0;
                m_bw.Value = 0;
            }
            m_blobs.BuildObjects(objROI.ref_ROI);

            try
            {
                // Build and select objects
                m_blobs.AnalyseObjects(ELegacyFeature.Area, ELegacyFeature.ObjectNumber, ELegacyFeature.GravityCenterX, ELegacyFeature.GravityCenterY, ELegacyFeature.LimitCenterX, ELegacyFeature.LimitCenterY, ELegacyFeature.LimitWidth, ELegacyFeature.LimitHeight, ELegacyFeature.EllipseAngle);
                m_blobs.SelectObjectsUsingFeature(ELegacyFeature.Area, m_intMinArea, m_intMaxArea, ESelectOption.RemoveOutOfRange);
                m_blobs.SelectObjectsUsingPosition(objROI.ref_ROI, ESelectByPosition.RemoveBorder);
                m_blobs.SortObjectsUsingFeature(ELegacyFeature.ObjectNumber, ESortOption.Descending);

            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_objTrackLog.WriteLine("OCV BuildObjects : " + ex.ToString());
                m_strErrorMessage = ex.ToString();
                return false;
            }

            if (m_blobs.NumSelectedObjects == 0)
            {
                m_strErrorMessage = "OCV BuildObjects : No blobs object selected!";
                return false;
            }
            else
            {
                int intThresholdValue;
                if (m_intThresholdValue == -4)
                    intThresholdValue = GetAutoThresholdValue(objROI, 3);
                else
                    intThresholdValue = m_intThresholdValue;

                float fBlobsCenterX, fBlobsCenterY, fWidth, fHeight;
                int intNoSelectedBlobs = m_blobs.NumSelectedObjects;
                m_ListEasyObject = m_blobs.FirstObjPtr;
                for (int i = 0; i < intNoSelectedBlobs; i++)
                {
                    fBlobsCenterX = fBlobsCenterY = fWidth = fHeight = 0;
                    m_blobs.GetObjectFeature(ELegacyFeature.LimitCenterX, m_ListEasyObject, out fBlobsCenterX);
                    m_blobs.GetObjectFeature(ELegacyFeature.LimitCenterY, m_ListEasyObject, out fBlobsCenterY);
                    m_blobs.GetObjectFeature(ELegacyFeature.LimitWidth, m_ListEasyObject, out fWidth);
                    m_blobs.GetObjectFeature(ELegacyFeature.LimitHeight, m_ListEasyObject, out fHeight);

                    int intStartX = (int)Math.Floor(fBlobsCenterX - fWidth / 2);//(int)Math.Round(fBlobsCenterX - fWidth / 2);
                    int intStartY = (int)Math.Floor(fBlobsCenterY - fHeight / 2);//(int)Math.Round(fBlobsCenterY - fHeight / 2);
                    int intEndX = (int)Math.Ceiling(fBlobsCenterX + fWidth / 2);//(int)Math.Round(fBlobsCenterX + fWidth / 2);
                    int intEndY = (int)Math.Ceiling(fBlobsCenterY + fHeight / 2);//(int)Math.Round(fBlobsCenterY + fHeight / 2);

                    int intSelectedStartX;
                    int intSelectedStartY;
                    int intSelectedEndX;
                    int intSelectedEndY;
                    int intSelectedX;
                    int intSelectedY;

                    #region Modify Left
                    intSelectedEndX = intStartX;
                    intSelectedY = intStartY;

                    int intStartTole = (int)Math.Floor(fWidth / 4);
                    if (intStartTole > 4)
                        intStartTole = 4;
                    for (int intTole = intStartTole; intTole > 1; intTole--)
                    {
                        // Get first time selected Y (Scan from left top to left bottom)
                        for (int y = intStartY + 1; y <= intEndY - 1; y++)
                        {
                            if (objROI.ref_ROI.GetPixel(intStartX + 1, y).Value >= intThresholdValue)
                            {
                                intSelectedY = y;
                                break;
                            }
                        }

                        bool blnLineDrawed = false;
                        if (intSelectedY < fBlobsCenterY)
                        {
                            // Scan from top to bottom
                            for (int y = intSelectedY; y <= intEndY - 1; y++)
                            {
                                bool blnEndXFound = false;
                                // Get End X for drawing
                                for (int x = intStartX + 1; x <= intEndX - 1; x++)
                                {
                                    if (objROI.ref_ROI.GetPixel(x, y).Value >= intThresholdValue)
                                    {
                                        intSelectedEndX = x;
                                        blnEndXFound = true;
                                        break;
                                    }
                                }

                                if (blnEndXFound)
                                {
                                    // Get Selected Y
                                    bool blnYFound = true;
                                    for (int m = intStartX - intTole; m <= intStartX; m++)
                                    {
                                        for (int n = y - 1; n <= y + 1; n++)
                                        {
                                            if (objROI.ref_ROI.GetPixel(m, n).Value >= intThresholdValue)
                                            {
                                                blnYFound = false;
                                                break;
                                            }
                                        }
                                        if (!blnYFound)
                                            break;
                                    }

                                    if (blnYFound && ((intSelectedEndX - intStartX - intTole + 1) <= 5))
                                    //if (objROI.ref_ROI.GetPixel(intStartX - intTole, y).Value < intThresholdValue) 
                                    {
                                        intSelectedY = y;
                                        m_bw.Value = (byte)(intThresholdValue + 1);
                                        for (int x = intStartX - intTole + 1; x < intSelectedEndX; x++)
                                        {
                                            objROI.ref_ROI.SetPixel(m_bw, x, intSelectedY);
                                        }
                                        blnLineDrawed = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Scan from bottom to top
                            for (int y = intSelectedY; y >= intStartY + 1; y--)
                            {
                                bool blnEndXFound = false;
                                // Get End X for drawing
                                for (int x = intStartX + 1; x <= intEndX - 1; x++)
                                {
                                    if (objROI.ref_ROI.GetPixel(x, y).Value >= intThresholdValue)
                                    {
                                        intSelectedEndX = x;
                                        blnEndXFound = true;
                                        break;
                                    }
                                }

                                if (blnEndXFound)
                                {
                                    // Get Selected Y
                                    bool blnYFound = true;
                                    for (int m = intStartX - intTole; m <= intStartX; m++)
                                    {
                                        for (int n = y - 1; n <= y + 1; n++)
                                        {
                                            if (objROI.ref_ROI.GetPixel(m, n).Value >= intThresholdValue)
                                            {
                                                blnYFound = false;
                                                break;
                                            }
                                        }
                                        if (!blnYFound)
                                            break;
                                    }

                                    if (blnYFound && ((intSelectedEndX - intStartX - intTole + 1) <= 5))
                                    //if (objROI.ref_ROI.GetPixel(intStartX - intTole, y).Value < intThresholdValue) 
                                    {
                                        intSelectedY = y;
                                        m_bw.Value = (byte)(intThresholdValue + 1);
                                        for (int x = intStartX - intTole + 1; x < intSelectedEndX; x++)
                                        {
                                            objROI.ref_ROI.SetPixel(m_bw, x, intSelectedY);
                                        }
                                        blnLineDrawed = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (blnLineDrawed)
                            break;
                    }
                    #endregion

                    #region Modify Right
                    intSelectedStartX = intEndX;
                    intSelectedY = intStartY;

                    intStartTole = (int)Math.Floor(fWidth / 4);
                    if (intStartTole > 4)
                        intStartTole = 4;
                    for (int intTole = intStartTole; intTole > 1; intTole--)
                    {
                        // Get first time selected Y (Scan from rigth top to right bottom
                        for (int y = intStartY + 1; y <= intEndY - 1; y++)
                        {
                            if (objROI.ref_ROI.GetPixel(intEndX - 1, y).Value >= intThresholdValue)
                            {
                                intSelectedY = y;
                                break;
                            }
                        }

                        bool blnLineDrawed = false;
                        if (intSelectedY < fBlobsCenterY)
                        {
                            // Scan from top to bottom
                            for (int y = intSelectedY; y <= intEndY - 1; y++)
                            {
                                bool blnStartXFound = false;
                                // Get start X for drawing
                                for (int x = intEndX - 1; x >= intStartX + 1; x--)
                                {
                                    if (objROI.ref_ROI.GetPixel(x, y).Value >= intThresholdValue)
                                    {
                                        intSelectedStartX = x;
                                        blnStartXFound = true;
                                        break;
                                    }
                                }

                                if (blnStartXFound)
                                {
                                    // Get Selected Y
                                    bool blnYFound = true;
                                    for (int m = intEndX; m <= intEndX + intTole; m++)
                                    {
                                        for (int n = y - 1; n <= y + 1; n++)
                                        {
                                            if (objROI.ref_ROI.GetPixel(m, n).Value >= intThresholdValue)
                                            {
                                                blnYFound = false;
                                                break;
                                            }
                                        }
                                        if (!blnYFound)
                                            break;
                                    }

                                    //if (objROI.ref_ROI.GetPixel(intEndX + intTole, y).Value < intThresholdValue)
                                    if (blnYFound && ((intEndX + intTole - 1 - intSelectedStartX) <= 5))
                                    {
                                        intSelectedY = y;
                                        m_bw.Value = (byte)(intThresholdValue + 1);
                                        for (int x = intSelectedStartX; x < intEndX + intTole - 1; x++)
                                        {
                                            objROI.ref_ROI.SetPixel(m_bw, x, intSelectedY);
                                        }
                                        blnLineDrawed = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Scan from bottom to top
                            for (int y = intSelectedY; y >= intStartY + 1; y--)
                            {
                                bool blnStartXFound = false;
                                // Get Start X for drawing
                                for (int x = intEndX - 1; x >= intStartX + 1; x--)
                                {
                                    if (objROI.ref_ROI.GetPixel(x, y).Value >= intThresholdValue)
                                    {
                                        intSelectedStartX = x;
                                        blnStartXFound = true;
                                        break;
                                    }
                                }

                                if (blnStartXFound)
                                {
                                    // Get Selected Y
                                    bool blnYFound = true;
                                    for (int m = intEndX; m <= intEndX + intTole; m++)
                                    {
                                        for (int n = y - 1; n <= y + 1; n++)
                                        {
                                            if (objROI.ref_ROI.GetPixel(m, n).Value >= intThresholdValue)
                                            {
                                                blnYFound = false;
                                                break;
                                            }
                                        }
                                        if (!blnYFound)
                                            break;
                                    }

                                    //if (objROI.ref_ROI.GetPixel(intEndX + intTole, y).Value < intThresholdValue)
                                    if (blnYFound && ((intEndX + intTole - 1 - intSelectedStartX) <= 5))
                                    {
                                        intSelectedY = y;
                                        m_bw.Value = (byte)(intThresholdValue + 1);
                                        for (int x = intSelectedStartX; x < intEndX + intTole - 1; x++)
                                        {
                                            objROI.ref_ROI.SetPixel(m_bw, x, intSelectedY);
                                        }
                                        blnLineDrawed = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (blnLineDrawed)
                            break;
                    }
                    #endregion

                    #region Modify Top
                    intSelectedEndY = intStartY;
                    intSelectedX = intStartX;

                    intStartTole = (int)Math.Floor(fHeight / 4);
                    if (intStartTole > 4)
                        intStartTole = 4;
                    for (int intTole = intStartTole; intTole > 1; intTole--)
                    {
                        // Get first time selected X
                        for (int x = intStartX + 1; x <= intEndX - 1; x++)
                        {
                            if (objROI.ref_ROI.GetPixel(x, intStartY + 1).Value >= intThresholdValue)
                            {
                                intSelectedX = x;
                                break;
                            }
                        }

                        bool blnLineDrawed = false;
                        if (intSelectedX < fBlobsCenterX)
                        {
                            // Scan from left to right
                            for (int x = intSelectedX; x <= intEndX - 1; x++)
                            {
                                bool blnEndYFound = false;
                                // Get End Y for drawing
                                for (int y = intStartY + 1; y <= intEndY - 1; y++)
                                {
                                    if (objROI.ref_ROI.GetPixel(x, y).Value >= intThresholdValue)
                                    {
                                        intSelectedEndY = y;
                                        blnEndYFound = true;
                                        break;
                                    }
                                }

                                if (blnEndYFound)
                                {
                                    // Get Selected X
                                    bool blnXFound = true;
                                    for (int n = intStartY - intTole; n <= intStartY; n++)
                                    {
                                        for (int m = x - 1; m <= x + 1; m++)
                                        {
                                            if (objROI.ref_ROI.GetPixel(m, n).Value >= intThresholdValue)
                                            {
                                                blnXFound = false;
                                                break;
                                            }
                                        }
                                        if (!blnXFound)
                                            break;
                                    }

                                    if (blnXFound && ((intSelectedEndY - intStartY - intTole + 1) <= 5))
                                    //if (objROI.ref_ROI.GetPixel(x, intStartY - intTole).Value < intThresholdValue)
                                    {
                                        intSelectedX = x;
                                        m_bw.Value = (byte)(intThresholdValue + 1);
                                        for (int y = intStartY - intTole + 1; y < intSelectedEndY; y++)
                                        {
                                            objROI.ref_ROI.SetPixel(m_bw, intSelectedX, y);
                                        }
                                        blnLineDrawed = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Scan from right to left
                            for (int x = intSelectedX; x >= intStartX + 1; x--)
                            {
                                bool blnEndYFound = false;
                                // Get End Y for drawing
                                for (int y = intStartY + 1; y <= intEndY - 1; y++)
                                {
                                    if (objROI.ref_ROI.GetPixel(x, y).Value >= intThresholdValue)
                                    {
                                        intSelectedEndY = y;
                                        blnEndYFound = true;
                                        break;
                                    }
                                }

                                if (blnEndYFound)
                                {
                                    // Get Selected X
                                    bool blnXFound = true;
                                    for (int n = intStartY - intTole; n <= intStartY; n++)
                                    {
                                        for (int m = x - 1; m <= x + 1; m++)
                                        {
                                            if (objROI.ref_ROI.GetPixel(m, n).Value >= intThresholdValue)
                                            {
                                                blnXFound = false;
                                                break;
                                            }
                                        }
                                        if (!blnXFound)
                                            break;
                                    }

                                    if (blnXFound && ((intSelectedEndY - intStartY - intTole + 1) <= 5))
                                    //if (objROI.ref_ROI.GetPixel(x, intStartY - intTole).Value < intThresholdValue)
                                    {
                                        intSelectedX = x;
                                        m_bw.Value = (byte)(intThresholdValue + 1);
                                        for (int y = intStartY - intTole + 1; y < intSelectedEndY; y++)
                                        {
                                            objROI.ref_ROI.SetPixel(m_bw, intSelectedX, y);
                                        }
                                        blnLineDrawed = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (blnLineDrawed)
                            break;
                    }
                    #endregion

                    #region Modify Bottom
                    intSelectedStartY = intEndY;
                    intSelectedX = intStartX;
                    intStartTole = (int)Math.Floor(fHeight / 4);
                    if (intStartTole > 4)
                        intStartTole = 4;
                    for (int intTole = intStartTole; intTole > 1; intTole--)
                    {
                        // Get first time selected X
                        for (int x = intStartX + 1; x <= intEndX - 1; x++)
                        {
                            if (objROI.ref_ROI.GetPixel(x, intEndY - 1).Value >= intThresholdValue)
                            {
                                intSelectedX = x;
                                break;
                            }
                        }

                        bool blnLineDrawed = false;
                        if (intSelectedX < fBlobsCenterX)
                        {
                            // Scan from left to right
                            for (int x = intSelectedX; x <= intEndX - 1; x++)
                            {
                                bool blnStartYFound = false;
                                // Get Start Y for drawing
                                for (int y = intEndY - 1; y >= intStartY + 1; y--)
                                {
                                    if (objROI.ref_ROI.GetPixel(x, y).Value >= intThresholdValue)
                                    {
                                        intSelectedStartY = y;
                                        blnStartYFound = true;
                                        break;
                                    }
                                }

                                if (blnStartYFound)
                                {
                                    // Get Selected X
                                    bool blnXFound = true;
                                    for (int n = intEndY; n <= intEndY + intTole; n++)
                                    {
                                        for (int m = x - 1; m <= x + 1; m++)
                                        {
                                            if (objROI.ref_ROI.GetPixel(m, n).Value >= intThresholdValue)
                                            {
                                                blnXFound = false;
                                                break;
                                            }
                                        }
                                        if (!blnXFound)
                                            break;
                                    }

                                    if (blnXFound && ((intEndY + intTole - 1 - intSelectedStartY) <= 5))
                                    //if (objROI.ref_ROI.GetPixel(x, intEndY + intTole).Value < intThresholdValue)
                                    {
                                        intSelectedX = x;
                                        m_bw.Value = (byte)(intThresholdValue + 1);
                                        for (int y = intSelectedStartY; y < intEndY + intTole - 1; y++)
                                        {
                                            objROI.ref_ROI.SetPixel(m_bw, intSelectedX, y);
                                        }
                                        blnLineDrawed = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Scan from rigth to left
                            for (int x = intSelectedX; x >= intStartX + 1; x--)
                            {
                                bool blnStartYFound = false;
                                // Get Start Y for drawing
                                for (int y = intEndY - 1; y >= intStartY + 1; y--)
                                {
                                    if (objROI.ref_ROI.GetPixel(x, y).Value >= intThresholdValue)
                                    {
                                        intSelectedStartY = y;
                                        blnStartYFound = true;
                                        break;
                                    }
                                }

                                if (blnStartYFound)
                                {
                                    // Get Selected X
                                    bool blnXFound = true;
                                    for (int n = intEndY; n <= intEndY + intTole; n++)
                                    {
                                        for (int m = x - 1; m <= x + 1; m++)
                                        {
                                            if (objROI.ref_ROI.GetPixel(m, n).Value >= intThresholdValue)
                                            {
                                                blnXFound = false;
                                                break;
                                            }
                                        }
                                        if (!blnXFound)
                                            break;
                                    }

                                    if (blnXFound && ((intEndY + intTole - 1 - intSelectedStartY) <= 5))
                                    //if (objROI.ref_ROI.GetPixel(x, intEndY + intTole).Value < intThresholdValue)
                                    {
                                        intSelectedX = x;
                                        m_bw.Value = (byte)(intThresholdValue + 1);
                                        for (int y = intSelectedStartY; y < intEndY + intTole - 1; y++)
                                        {
                                            objROI.ref_ROI.SetPixel(m_bw, intSelectedX, y);
                                        }
                                        blnLineDrawed = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (blnLineDrawed)
                            break;
                    }
                    #endregion

                    m_ListEasyObject = m_blobs.GetNextObjPtr(m_ListEasyObject);
                }
            }
            return true;
        }

        public void OcvFormMultiChars()
        {
            m_Ocv.DeleteTemplateChars(ESelectionFlag.True);
            m_Ocv.CreateTemplateChars(ESelectionFlag.True, ECharCreationMode.Separate);
        }

        public void OcvFormSingleChars()
        {
            m_Ocv.DeleteTemplateChars(ESelectionFlag.True);
            m_Ocv.CreateTemplateChars(ESelectionFlag.True, ECharCreationMode.Group);
        }

        public void OcvFormTexts()
        {
            //m_Ocv.DeleteTemplateTexts(ESelectionFlag.True);
            m_Ocv.CreateTemplateTexts(ESelectionFlag.True);
        }

        public void OcvUndoChars()
        {
            m_Ocv.DeleteTemplateChars(ESelectionFlag.Any);
        }

        public void OcvUndoTexts()
        {
            m_Ocv.DeleteTemplateTexts(ESelectionFlag.True);
        }

        public void ResetParameters()
        {
            m_OcvChar.ResetParameters();
            m_OcvText.ResetParameters();
        }

        public void ResetSampleCharsAndTexts(int intOriginX, int intOriginY, int intWidth, int intHeight)
        {
            m_Ocv.SelectSampleTexts(intOriginX, intOriginY, intWidth, intHeight, ESelectionFlag.True);
            m_Ocv.SelectSampleTextsChars(intOriginX, intOriginY, intWidth, intHeight, ESelectionFlag.Any, ESelectionFlag.True);
            m_BinaOcv.SelectSampleTexts(intOriginX, intOriginY, intWidth, intHeight, ESelectionFlag.True);
            m_BinaOcv.SelectSampleTextsChars(intOriginX, intOriginY, intWidth, intHeight, ESelectionFlag.Any, ESelectionFlag.True);
        }

        public void SaveOCV(string strPath)
        {
            m_Ocv.Save(strPath);
        }

        public void SaveToleranceCharsAndTexts()
        {
            if (m_OcvText.TemplateBackgroundArea != -1)
                m_Ocv.ScatterTextsParameters(m_OcvText, ESelectionFlag.True);
            if (m_OcvChar.TemplateBackgroundArea != -1)
                m_Ocv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);

        }

        public void SelectSampleCharsAndTexts(int intOriginX, int intOriginY)
        {
            m_Ocv.SelectSampleTexts(intOriginX, intOriginY, 1, 1);
            m_Ocv.SelectSampleTextsChars(intOriginX, intOriginY, 1, 1);
            m_BinaOcv.SelectSampleTexts(intOriginX, intOriginY, 1, 1);
            m_BinaOcv.SelectSampleTextsChars(intOriginX, intOriginY, 1, 1);
        }

        public void SelectTemplateCharacters(System.Drawing.Point p1, System.Drawing.Point p2)
        {
            int intOrgX, intOrgY, intWidth, intHeight;

            if (p1.X < p2.X)
            {
                intOrgX = p1.X;
                intWidth = p2.X - p1.X;
            }
            else
            {
                intOrgX = p2.X;
                intWidth = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                intOrgY = p1.Y;
                intHeight = p2.Y - p1.Y;
            }
            else
            {
                intOrgY = p2.Y;
                intHeight = p1.Y - p2.Y;
            }

            m_Ocv.SelectTemplateObjects(intOrgX, intOrgY, intWidth, intHeight, ESelectionFlag.Any);
            m_Ocv.SelectTemplateChars(intOrgX, intOrgY, intWidth, intHeight, ESelectionFlag.Any);
        }

        public void SelectTemplateCharacters(int intOrgX, int intOrgY, int intWidth, int intHeight)
        {
            m_Ocv.SelectTemplateObjects(intOrgX, intOrgY, intWidth, intHeight, ESelectionFlag.Any);
            m_Ocv.SelectTemplateChars(intOrgX, intOrgY, intWidth, intHeight, ESelectionFlag.Any);
        }

        public void SetAllCharsParameters(int intWidth, int intHeight, int intCharSetValue)
        {
            int intNumTextChars = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX, intCenterY;
            int intCharsCount = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, intWidth, intHeight, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    if (m_arrStatistics.Count > 0)
                        SetValueToChars(m_OcvChar, intCharSetValue, (StatisticInfo)m_arrStatistics[intCharsCount]);
                    else
                        SetValueToChars(m_OcvChar, intCharSetValue);
                    m_Ocv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    intCharsCount++;
                }
            }

            // Set for Binarized OCV
            intNumTexts = (int)m_BinaOcv.NumTexts;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_BinaOcv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_BinaOcv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_BinaOcv.SelectSampleTextsChars(0, 0, intWidth, intHeight, ESelectionFlag.Any, ESelectionFlag.True);
                    m_BinaOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_BinaOcv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    if (m_arrStatistics.Count > 0)
                        SetValueToChars(m_OcvChar, intCharSetValue, (StatisticInfo)m_arrStatistics[intCharsCount]);
                    else
                        SetValueToChars(m_OcvChar, intCharSetValue);
                    m_BinaOcv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    intCharsCount++;
                }
            }
        }

        public void SetAllCharsSetting(int intCharSetValue)
        {
            m_arrCharSetting.Clear();
            for (int i = 0; i < m_intCharsCount; i++)
            {
                m_arrCharSetting.Add(intCharSetValue);
            }
        }

        public void SetCharsNumberByCenterPoint()
        {
            int intNumTextChars = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX, intCenterY, intStartX, intStartY, intEndX, intEndY;
            m_arrCharNo.Clear();
            ArrayList arrCharFeatures = new ArrayList();
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = intStartX = intStartY = intEndX = intEndY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);
                    int intSelectedIndex = 0;
                    for (int i = 0; i < arrCharFeatures.Count; i++)
                    {
                        // same row
                        if ((intCenterY < ((CharFeatures)arrCharFeatures[i]).intCenterY + 10) &&
                            (intCenterY > ((CharFeatures)arrCharFeatures[i]).intCenterY - 10))
                        {
                            if (intCenterX > ((CharFeatures)arrCharFeatures[i]).intCenterX)
                                intSelectedIndex++;
                        }
                        // different row
                        else
                        {
                            if (intCenterY > ((CharFeatures)arrCharFeatures[i]).intCenterY - 10)
                                intSelectedIndex++;
                        }
                    }
                    CharFeatures stcCharFeatures = new CharFeatures();
                    stcCharFeatures.intCenterX = intCenterX;
                    stcCharFeatures.intCenterY = intCenterY;
                    stcCharFeatures.intStartX = intStartX;
                    stcCharFeatures.intStartY = intStartY;
                    stcCharFeatures.intEndX = intEndX;
                    stcCharFeatures.intEndY = intEndY;

                    if (intSelectedIndex == m_arrCharNo.Count)
                    {
                        arrCharFeatures.Add(stcCharFeatures);
                    }
                    else
                    {
                        arrCharFeatures.Insert(intSelectedIndex, stcCharFeatures);
                    }
                    for (int i = 0; i < m_arrCharNo.Count; i++)
                    {
                        if ((int)m_arrCharNo[i] >= intSelectedIndex)
                            m_arrCharNo[i] = (int)m_arrCharNo[i] + 1;
                    }
                    m_arrCharNo.Add(intSelectedIndex);
                }
            }


        }

        public void SetCharsNumberByColumn()
        {
            int intNumTextChars = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX, intCenterY, intStartX, intStartY, intEndX, intEndY;
            ArrayList arrCharFeatures = new ArrayList();
            CharFeatures stcCharFeatures = new CharFeatures();
            stcCharFeatures.arrCenterX = new ArrayList();

            #region Define rows
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = intStartX = intStartY = intEndX = intEndY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);
                    bool blnMatch = false;
                    int intMatchRowNumber = -1;
                    for (int i = 0; i < arrCharFeatures.Count; i++)
                    {
                        if (((intStartX >= ((CharFeatures)arrCharFeatures[i]).intMin) && (intStartX <= ((CharFeatures)arrCharFeatures[i]).intMax)) ||
                            ((intEndX >= ((CharFeatures)arrCharFeatures[i]).intMin) && (intEndX <= ((CharFeatures)arrCharFeatures[i]).intMax)) ||
                            ((intStartX <= ((CharFeatures)arrCharFeatures[i]).intMin) && (intEndX >= ((CharFeatures)arrCharFeatures[i]).intMax)) ||
                            ((intStartX <= ((CharFeatures)arrCharFeatures[i]).intMax) && (intEndX >= ((CharFeatures)arrCharFeatures[i]).intMin)))
                        {
                            if (intStartX < ((CharFeatures)arrCharFeatures[i]).intMin)
                                stcCharFeatures.intMin = intStartX;
                            else
                                stcCharFeatures.intMin = ((CharFeatures)arrCharFeatures[i]).intMin;

                            if (intEndX > ((CharFeatures)arrCharFeatures[i]).intMax)
                                stcCharFeatures.intMax = intEndX;
                            else
                                stcCharFeatures.intMax = ((CharFeatures)arrCharFeatures[i]).intMax;

                            arrCharFeatures.RemoveAt(i);
                            blnMatch = true;
                            if (intMatchRowNumber == -1)
                                intMatchRowNumber = i;
                            i--;
                        }
                    }

                    int intSelectedRow = 0;
                    if (blnMatch)
                    {
                        intSelectedRow = intMatchRowNumber;
                    }
                    else
                    {

                        for (int i = 0; i < arrCharFeatures.Count; i++)
                        {
                            if (intStartX > ((CharFeatures)arrCharFeatures[i]).intMin)
                                intSelectedRow++;
                        }

                        stcCharFeatures.intMin = intStartX;
                        stcCharFeatures.intMax = intEndX;
                    }

                    stcCharFeatures.arrCenterX = new ArrayList();
                    if (arrCharFeatures.Count == 0)
                    {
                        arrCharFeatures.Add(stcCharFeatures);
                    }
                    else
                    {
                        arrCharFeatures.Insert(intSelectedRow, stcCharFeatures);
                    }
                }
            }
            #endregion

            #region Define columns
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = intStartX = intStartY = intEndX = intEndY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);
                    for (int i = 0; i < arrCharFeatures.Count; i++)
                    {
                        stcCharFeatures = (CharFeatures)arrCharFeatures[i];
                        if ((intCenterX < stcCharFeatures.intMax) && (intCenterX > stcCharFeatures.intMin))
                        {
                            stcCharFeatures.arrCenterX.Add(intCenterY);
                        }
                    }

                }
            }
            #endregion

            #region Define Characters Number
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = intStartX = intStartY = intEndX = intEndY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);
                    int intCharIndex = 0;
                    for (int i = 0; i < arrCharFeatures.Count; i++)
                    {
                        stcCharFeatures = (CharFeatures)arrCharFeatures[i];
                        if ((intCenterX < stcCharFeatures.intMax) && (intCenterX > stcCharFeatures.intMin))
                        {
                            for (int j = 0; j < stcCharFeatures.arrCenterX.Count; j++)
                            {
                                if (intCenterY > Convert.ToInt32(stcCharFeatures.arrCenterX[j]))
                                {
                                    intCharIndex++;
                                }
                            }

                        }
                        else if (intCenterX > stcCharFeatures.intMax)
                            intCharIndex += stcCharFeatures.arrCenterX.Count;
                    }
                    m_arrCharNo.Add(intCharIndex);
                }
            }
            #endregion
        }

        public void SetCharsNumberByRow()
        {
            int intNumTextChars = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX, intCenterY, intStartX, intStartY, intEndX, intEndY;
            ArrayList arrCharFeatures = new ArrayList();
            CharFeatures stcCharFeatures = new CharFeatures();
            stcCharFeatures.arrCenterX = new ArrayList();

            #region Define rows
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = intStartX = intStartY = intEndX = intEndY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);
                    bool blnMatch = false;
                    int intMatchRowNumber = -1;
                    for (int i = 0; i < arrCharFeatures.Count; i++)
                    {
                        if (((intStartY >= ((CharFeatures)arrCharFeatures[i]).intMin) && (intStartY <= ((CharFeatures)arrCharFeatures[i]).intMax)) ||
                            ((intEndY >= ((CharFeatures)arrCharFeatures[i]).intMin) && (intEndY <= ((CharFeatures)arrCharFeatures[i]).intMax)) ||
                            ((intStartY <= ((CharFeatures)arrCharFeatures[i]).intMin) && (intEndY >= ((CharFeatures)arrCharFeatures[i]).intMax)) ||
                            ((intStartY <= ((CharFeatures)arrCharFeatures[i]).intMax) && (intEndY >= ((CharFeatures)arrCharFeatures[i]).intMin)))
                        {
                            if (intStartY < ((CharFeatures)arrCharFeatures[i]).intMin)
                                stcCharFeatures.intMin = intStartY;
                            else
                                stcCharFeatures.intMin = ((CharFeatures)arrCharFeatures[i]).intMin;

                            if (intEndY > ((CharFeatures)arrCharFeatures[i]).intMax)
                                stcCharFeatures.intMax = intEndY;
                            else
                                stcCharFeatures.intMax = ((CharFeatures)arrCharFeatures[i]).intMax;

                            arrCharFeatures.RemoveAt(i);
                            blnMatch = true;
                            if (intMatchRowNumber == -1)
                                intMatchRowNumber = i;
                            i--;
                        }
                    }

                    int intSelectedRow = 0;
                    if (blnMatch)
                    {
                        intSelectedRow = intMatchRowNumber;
                    }
                    else
                    {

                        for (int i = 0; i < arrCharFeatures.Count; i++)
                        {
                            if (intStartY > ((CharFeatures)arrCharFeatures[i]).intMin)
                                intSelectedRow++;
                        }

                        stcCharFeatures.intMin = intStartY;
                        stcCharFeatures.intMax = intEndY;
                    }

                    stcCharFeatures.arrCenterX = new ArrayList();
                    if (arrCharFeatures.Count == 0)
                    {
                        arrCharFeatures.Add(stcCharFeatures);
                    }
                    else
                    {
                        arrCharFeatures.Insert(intSelectedRow, stcCharFeatures);
                    }
                }
            }
            #endregion

            #region Define columns
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = intStartX = intStartY = intEndX = intEndY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);
                    for (int i = 0; i < arrCharFeatures.Count; i++)
                    {
                        stcCharFeatures = (CharFeatures)arrCharFeatures[i];
                        if ((intCenterY < stcCharFeatures.intMax) && (intCenterY > stcCharFeatures.intMin))
                        {
                            stcCharFeatures.arrCenterX.Add(intCenterX);
                        }
                    }

                }
            }
            #endregion

            #region Define Characters Number
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);

                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);

                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = intStartX = intStartY = intEndX = intEndY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.GetTextCharPoint(tx, ch, out intStartX, out intStartY, -1, -1);
                    m_Ocv.GetTextCharPoint(tx, ch, out intEndX, out intEndY, 1, 1);
                    int intCharIndex = 0;
                    for (int i = 0; i < arrCharFeatures.Count; i++)
                    {
                        stcCharFeatures = (CharFeatures)arrCharFeatures[i];
                        if ((intCenterY < stcCharFeatures.intMax) && (intCenterY > stcCharFeatures.intMin))
                        {
                            for (int j = 0; j < stcCharFeatures.arrCenterX.Count; j++)
                            {
                                if (intCenterX > Convert.ToInt32(stcCharFeatures.arrCenterX[j]))
                                {
                                    intCharIndex++;
                                }
                            }

                        }
                        else if (intCenterY > stcCharFeatures.intMax)
                            intCharIndex += stcCharFeatures.arrCenterX.Count;
                    }
                    m_arrCharNo.Add(intCharIndex);
                }
            }
            #endregion
        }

        public void SetCharParameters(int intWidth, int intHeight, int intCharNo, int intCharSetValue)
        {
            int intNumTextChars = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intCenterX, intCenterY;
            int intCharsCount = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    if ((int)m_arrCharNo[intCharsCount] == intCharNo)
                    {
                        intCenterX = intCenterY = 0;
                        m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                        m_Ocv.SelectSampleTextsChars(0, 0, intWidth, intHeight, ESelectionFlag.Any, ESelectionFlag.True);
                        m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                        m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                        if (m_arrStatistics.Count > 0)
                            SetValueToChars(m_OcvChar, intCharSetValue, (StatisticInfo)m_arrStatistics[intCharsCount]);
                        else
                            SetValueToChars(m_OcvChar, intCharSetValue);
                        m_Ocv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                        return;
                    }
                    intCharsCount++;
                }
            }
        }

        public void SetCharSetting(int intSelectedIndex, int intCharSetValue)
        {
            m_arrCharSetting.RemoveAt(intSelectedIndex);
            m_arrCharSetting.Insert(intSelectedIndex, intCharSetValue);
        }

        public void SetCharsSettingDefault(int intCharSetValue)
        {
            m_arrCharSetting.Clear();

            int intNumTextChars = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    m_arrCharSetting.Add(intCharSetValue);
                }
            }
        }

        public void SetCharsSettingPrevious(int[] intCharSetValue, int intCharDefaultSetValue)
        {
            m_arrCharSetting.Clear();
            int intLastSetValue = intCharDefaultSetValue;
            int intNumTextChars = 0;
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intIndex = 0;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    if (intIndex < intCharSetValue.Length)
                    {
                        m_arrCharSetting.Add(intCharSetValue[intIndex]);
                        intLastSetValue = intCharSetValue[intIndex];
                    }
                    else
                        m_arrCharSetting.Add(intLastSetValue);

                    intIndex++;
                }
            }
        }

        public void SetLocationMode(int intMode)
        {
            // mode 0: Raw, 1: Binarized, 2/Default: Gradient, 3: Laplacian

            if (intMode == 0)
                m_Ocv.LocationMode = ELocationMode.Raw;
            else if (intMode == 1)
                m_Ocv.LocationMode = ELocationMode.Binarized;
            else if (intMode == 2)
                m_Ocv.LocationMode = ELocationMode.Gradient;
            else if (intMode == 3)
                m_Ocv.LocationMode = ELocationMode.Laplacian;
            else
                m_Ocv.LocationMode = ELocationMode.Gradient;

        }

        private void SetPositionToChars(EOCVChar objOcvChar, int intShiftTole)
        {
            objOcvChar.ShiftXTolerance = intShiftTole;
            objOcvChar.ShiftYTolerance = intShiftTole;
        }

        private void SetPositionToTexts(EOCVText objOcvText, int intShiftTole)
        {
            objOcvText.ShiftXTolerance = intShiftTole;
            objOcvText.ShiftYTolerance = intShiftTole;
        }

        public void SetTemplateTextImage(ImageDrawing objImage, int intROIOffSetX, int intROIOffSetY)
        {
            objImage.CopyTo(ref m_objLearnImage);

            System.Drawing.Point pStart= GetTextStartPoint(0);
            System.Drawing.Point pEnd = GetTextEndPoint(0);

            m_objTemplateTextROI.ref_ROIPositionX = pStart.X;
            m_objTemplateTextROI.ref_ROIPositionY = pStart.Y;
            m_objTemplateTextROI.ref_ROIWidth = pEnd.X - pStart.X;
            m_objTemplateTextROI.ref_ROIHeight = pEnd.Y - pStart.Y;

            m_objTemplateTextROI.AttachImage(m_objLearnImage);
            EasyImage.DilateBox(m_objTemplateTextROI.ref_ROI);
        }

        public void SetTemplateTextImage(ImageDrawing objImage, int intTextStartX, int intTextStartY, int intTextWidth, int intTextHeight)
        {
            objImage.CopyTo(ref m_objLearnImage);

            m_objTemplateTextROI.ref_ROIPositionX = intTextStartX;
            m_objTemplateTextROI.ref_ROIPositionY = intTextStartY;
            m_objTemplateTextROI.ref_ROIWidth = intTextWidth;
            m_objTemplateTextROI.ref_ROIHeight = intTextHeight;

            m_objTemplateTextROI.AttachImage(m_objLearnImage);
            EasyImage.DilateBox(m_objTemplateTextROI.ref_ROI);
        }

        public void SetOcvSettings(int intThresholdValue, int intMinArea, int intMaxArea, int intFailMask, 
            int intAllLocationScore, int intAllArea, int intAllCorrelation, int intTestShiftLimit, 
            int intUncheckAreaTop, int intUncheckAreaBottom, int intUncheckAreaLeft, int intUncheckAreaRight,
            int intTextShiftX, int intTextShiftY, int intCharShiftX, int intCharShiftY, int intGroupMinArea, 
            int intExtraMinArea, int intToleranceSize, float fAreaScoreFactor, int intStatisticsCount)
        {
            int iintNumTextChars = (int)m_Ocv.GetNumTextChars(0);
            m_intThresholdValue = intThresholdValue;
            m_intMinArea = intMinArea;
            m_intMaxArea = intMaxArea;

            m_intFailMask = intFailMask;
            m_intAllLocationScore = intAllLocationScore;
            m_intAllArea = intAllArea;
            m_intAllCorrelation = intAllCorrelation;
            m_intTextShiftLimit = intTestShiftLimit;
            m_intUncheckAreaTop = intUncheckAreaTop;
            m_intUncheckAreaBottom = intUncheckAreaBottom;
            m_intUncheckAreaLeft = intUncheckAreaLeft;
            m_intUncheckAreaRight = intUncheckAreaRight;
            m_intAllTextShiftX = intTextShiftX;
            m_intAllTextShiftY = intTextShiftY;
            m_intAllCharShiftX = intCharShiftX;
            m_intAllCharShiftY = intCharShiftY;
            m_intGroupMinArea = intGroupMinArea;
            m_intExtraMinArea = intExtraMinArea;
            m_intToleranceSize = intToleranceSize;
            m_fAreaScoreFactor = fAreaScoreFactor;
            if (intStatisticsCount == 0)
                m_intStatisticCount = 0;
            else
                m_intStatisticCount = intStatisticsCount + 1;
        }

        public void SetValueToParameters(int intWidth, int intHeight, int intCharShiftTole)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intCenterX, intCenterY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, intWidth, intWidth, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    SetPositionToChars(m_OcvChar, intCharShiftTole);
                    m_Ocv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                }
            }

            intNumTexts = (int)m_BinaOcv.NumTexts;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = (int)m_BinaOcv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intNumTextChars = m_BinaOcv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_BinaOcv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_BinaOcv.SelectSampleTextsChars(0, 0, intWidth, intWidth, ESelectionFlag.Any, ESelectionFlag.True);
                    m_BinaOcv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_BinaOcv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    SetPositionToChars(m_OcvChar, intCharShiftTole);
                    m_BinaOcv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                }
            }
        }

        public void SetValueToParameters(int intWidth, int intHeight, int intTextShiftTole, int intCharShiftTole)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intCenterX, intCenterY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, intWidth, intHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                SetPositionToTexts(m_OcvText, intTextShiftTole);
                m_Ocv.ScatterTextsParameters(m_OcvText, ESelectionFlag.True);

                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, intWidth, intHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                SetPositionToTexts(m_OcvText, intTextShiftTole);
                m_Ocv.ScatterTextsParameters(m_OcvText, ESelectionFlag.True);

                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, intWidth, intWidth, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    SetPositionToChars(m_OcvChar, intCharShiftTole);
                    m_Ocv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                }
            }
        }

        public void SetValueToParameters(int intWidth, int intHeight, int intScore, int intTextShiftTole, int intCharShiftTole)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intCenterX, intCenterY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, intWidth, intHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                SetValueToTexts(m_OcvText, intScore, intTextShiftTole);
                m_Ocv.ScatterTextsParameters(m_OcvText, ESelectionFlag.True);

                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, intWidth, intHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                SetValueToTexts(m_OcvText, intScore, intTextShiftTole);
                m_Ocv.ScatterTextsParameters(m_OcvText, ESelectionFlag.True);

                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, intWidth, intWidth, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    SetValueToChars(m_OcvChar, intScore, intCharShiftTole);
                    m_Ocv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                }
            }
        }

        public void SetValueToParameters(int intWidth, int intHeight, int intScore, int intTextShiftXTole, int intTextShiftYTole, int intCharShiftXTole, int intCharShiftYTole)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intCenterX, intCenterY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, intWidth, intHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                SetValueToTexts(m_OcvText, intScore, intTextShiftXTole, intTextShiftYTole);
                m_Ocv.ScatterTextsParameters(m_OcvText, ESelectionFlag.True);

                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, intWidth, intHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                SetValueToTexts(m_OcvText, intScore, intTextShiftXTole, intTextShiftYTole);
                m_Ocv.ScatterTextsParameters(m_OcvText, ESelectionFlag.True);

                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, intWidth, intWidth, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    SetValueToChars(m_OcvChar, intScore, intCharShiftXTole, intCharShiftYTole);
                    m_Ocv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                }
            }
        }

        public void SetValueToParameters(int intWidth, int intHeight, int[] intTextScore, int[] intCharScore, int[] intTextShiftXTole, int[] intTextShiftYTole, int[] intCharShiftXTole, int[] intCharShiftYTole,
            int intDefaultScore, int intTextShiftXDefaultTole, int intTextShiftYDefaultTole, int intCharShiftXDefaultTole, int intCharShiftYDefaultTole)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intCenterX, intCenterY;
            int intTextIndex = 0;
            int intCharIndex = 0;

            int intScore;
            int intShiftX;
            int intShiftY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
#endif

                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, intWidth, intHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);

                // Get ocvText
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);

                // Get ocvText setting
                if (intTextIndex < intTextScore.Length)
                    intScore = intTextScore[intTextIndex];
                else
                    intScore = intDefaultScore;

                if (intTextIndex < intTextShiftXTole.Length)
                    intShiftX = intTextShiftXTole[intTextIndex];
                else 
                    intShiftX = intTextShiftXDefaultTole;

                if (intTextIndex < intTextShiftYTole.Length)
                    intShiftY = intTextShiftYTole[intTextIndex];
                else
                    intShiftY = intTextShiftYDefaultTole;

                // Set ocvText setting
                SetValueToTexts(m_OcvText, intScore, intShiftX, intShiftY);

                // Store back ocvText to OCV
                m_Ocv.ScatterTextsParameters(m_OcvText, ESelectionFlag.True);

                intTextIndex++;
#if (Debug_2_12 || Release_2_12)
                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, intWidth, intWidth, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);

                    // Get ocvChar
                    m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                    // Get ocvChar setting
                    if (intCharIndex < intCharScore.Length)
                        intScore = intCharScore[intCharIndex];
                    else
                        intScore = intDefaultScore;

                    if (intCharIndex < intCharShiftXTole.Length)
                        intShiftX = intCharShiftXTole[intCharIndex];
                    else
                        intShiftX = intCharShiftXDefaultTole;

                    if (intCharIndex < intCharShiftYTole.Length)
                        intShiftY = intCharShiftYTole[intCharIndex];
                    else
                        intShiftY = intCharShiftYDefaultTole;

                    // Set ocvChar setting
                    SetValueToChars(m_OcvChar, intScore, intShiftX, intShiftY);

                    // Store back ocvChar to OCV
                    m_Ocv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);

                    intCharIndex++;
                }
            }
        }

        public void SetValueToParameters(int intWidth, int intHeight, int intLocScoreTole, int intAreaSumTole, int intCorrelationTole, int intTextShiftXTole, int intTextShiftYTole, int intCharShiftXTole, int intCharShiftYTole)
        {
            int intNumTexts = (int)m_Ocv.NumTexts;
            int intNumTextChars = 0;
            int intCenterX, intCenterY;
#if (Debug_2_12 || Release_2_12)
            for (uint tx = 0; tx < intNumTexts; tx++)
            {
                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, intWidth, intHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                SetValueToTexts(m_OcvText, intLocScoreTole, intAreaSumTole, intCorrelationTole, intTextShiftXTole, intTextShiftXTole);
                m_Ocv.ScatterTextsParameters(m_OcvText, ESelectionFlag.True);

                intNumTextChars = (int)m_Ocv.GetNumTextChars(tx);
                for (uint ch = 0; ch < intNumTextChars; ch++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            for (int tx = 0; tx < intNumTexts; tx++)
            {
                intCenterX = intCenterY = 0;
                m_Ocv.GetTextPoint(tx, out intCenterX, out intCenterY, 0, 0);
                m_Ocv.SelectSampleTexts(0, 0, intWidth, intHeight, ESelectionFlag.True);
                m_Ocv.SelectSampleTexts(intCenterX, intCenterY, 1, 1);
                m_Ocv.GatherTextsParameters(m_OcvText, ESelectionFlag.True);
                SetValueToTexts(m_OcvText, intLocScoreTole, intAreaSumTole, intCorrelationTole, intTextShiftXTole, intTextShiftXTole);
                m_Ocv.ScatterTextsParameters(m_OcvText, ESelectionFlag.True);

                intNumTextChars = m_Ocv.GetNumTextChars(tx);
                for (int ch = 0; ch < intNumTextChars; ch++)
                {
#endif

                    intCenterX = intCenterY = 0;
                    m_Ocv.GetTextCharPoint(tx, ch, out intCenterX, out intCenterY, 0, 0);
                    m_Ocv.SelectSampleTextsChars(0, 0, intWidth, intWidth, ESelectionFlag.Any, ESelectionFlag.True);
                    m_Ocv.SelectSampleTextsChars(intCenterX, intCenterY, 1, 1);
                    m_Ocv.GatherTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                    SetValueToChars(m_OcvChar, intLocScoreTole, intAreaSumTole, intCorrelationTole, intCharShiftXTole, intCharShiftXTole);
                    m_Ocv.ScatterTextsCharsParameters(m_OcvChar, ESelectionFlag.Any, ESelectionFlag.True);
                }
            }
        }

        private void SetValueToChars(EOCVChar objOcvChar, int intScore)
        {
            if (intScore > 25)
                objOcvChar.LocationScoreTolerance = objOcvChar.TemplateLocationScore * 75 / 100;
            else
                objOcvChar.LocationScoreTolerance = objOcvChar.TemplateLocationScore * (100 - intScore) / 100;
#if (Debug_2_12 || Release_2_12)
            objOcvChar.BackgroundAreaTolerance = (uint)Math.Round(objOcvChar.TemplateBackgroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
            objOcvChar.ForegroundAreaTolerance = (uint)Math.Round(objOcvChar.TemplateForegroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            objOcvChar.BackgroundAreaTolerance = (int)Math.Round(objOcvChar.TemplateBackgroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
            objOcvChar.ForegroundAreaTolerance = (int)Math.Round(objOcvChar.TemplateForegroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
#endif

            objOcvChar.BackgroundSumTolerance = objOcvChar.TemplateBackgroundSum * (100 - intScore) * m_fAreaScoreFactor / 100;
            objOcvChar.ForegroundSumTolerance = objOcvChar.TemplateForegroundSum * (100 - intScore) * m_fAreaScoreFactor / 100;
            objOcvChar.CorrelationTolerance = (100 - (float)intScore) / 100;
        }

        private void SetValueToChars(EOCVChar objOcvChar, int intScore, int intShiftTole)
        {
            if (intScore > 25)
                objOcvChar.LocationScoreTolerance = objOcvChar.TemplateLocationScore * 75 / 100;
            else
                objOcvChar.LocationScoreTolerance = objOcvChar.TemplateLocationScore * (100 - intScore) * m_fAreaScoreFactor / 100;
#if (Debug_2_12 || Release_2_12)
            objOcvChar.BackgroundAreaTolerance = (uint)Math.Round(objOcvChar.TemplateBackgroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
            objOcvChar.ForegroundAreaTolerance = (uint)Math.Round(objOcvChar.TemplateForegroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            objOcvChar.BackgroundAreaTolerance = (int)Math.Round(objOcvChar.TemplateBackgroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
            objOcvChar.ForegroundAreaTolerance = (int)Math.Round(objOcvChar.TemplateForegroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
#endif

            objOcvChar.BackgroundSumTolerance = objOcvChar.TemplateBackgroundSum * (100 - intScore) * m_fAreaScoreFactor / 100;
            objOcvChar.ForegroundSumTolerance = objOcvChar.TemplateForegroundSum * (100 - intScore) * m_fAreaScoreFactor / 100;
            objOcvChar.CorrelationTolerance = (100 - (float)intScore) / 100;

            objOcvChar.ShiftXTolerance = intShiftTole;
            objOcvChar.ShiftYTolerance = intShiftTole;
        }

        private void SetValueToChars(EOCVChar objOcvChar, int intScore, int intShiftXTole, int intShiftYTole)
        {
            if (intScore > 25)
                objOcvChar.LocationScoreTolerance = objOcvChar.TemplateLocationScore * 75 / 100;
            else
                objOcvChar.LocationScoreTolerance = objOcvChar.TemplateLocationScore * (100 - intScore) * m_fAreaScoreFactor / 100;
#if (Debug_2_12 || Release_2_12)
            objOcvChar.BackgroundAreaTolerance = (uint)Math.Round(objOcvChar.TemplateBackgroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
            objOcvChar.ForegroundAreaTolerance = (uint)Math.Round(objOcvChar.TemplateForegroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            objOcvChar.BackgroundAreaTolerance = (int)Math.Round(objOcvChar.TemplateBackgroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
            objOcvChar.ForegroundAreaTolerance = (int)Math.Round(objOcvChar.TemplateForegroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
#endif

            objOcvChar.BackgroundSumTolerance = objOcvChar.TemplateBackgroundSum * (100 - intScore) * m_fAreaScoreFactor / 100;
            objOcvChar.ForegroundSumTolerance = objOcvChar.TemplateForegroundSum * (100 - intScore) * m_fAreaScoreFactor / 100;
            objOcvChar.CorrelationTolerance = (100 - (float)intScore) / 100;

            objOcvChar.ShiftXTolerance = intShiftXTole;
            objOcvChar.ShiftYTolerance = intShiftYTole;
        }

        private void SetValueToChars(EOCVChar objOcvChar, int intScore, StatisticInfo objStatisticInfo)
        {
            int intTempValue;
            float fTempValue;

            if (intScore > 25)
                objOcvChar.LocationScoreTolerance = objOcvChar.TemplateLocationScore * 75 / 100;
            else
                objOcvChar.LocationScoreTolerance = objOcvChar.TemplateLocationScore * (100 - intScore) / 100;

            intTempValue = (int)Math.Round((objOcvChar.TemplateBackgroundArea * m_fAreaScoreFactor) * (100 - intScore) / 100, 0);
#if (Debug_2_12 || Release_2_12)
            if (intTempValue > ((objStatisticInfo.intBgAreaMax - objStatisticInfo.intBgAreaMin) / 2))
            {
                objOcvChar.BackgroundAreaTolerance = (uint)intTempValue;
                objStatisticInfo.fBgAreaFac = 1;
            }
            else
            {
                objOcvChar.BackgroundAreaTolerance = (uint)(((objStatisticInfo.intBgAreaMax - objStatisticInfo.intBgAreaMin) / 2) * 1.5);
                objStatisticInfo.fBgAreaFac = (float)intScore / ((float)objOcvChar.BackgroundAreaTolerance / (float)objOcvChar.TemplateBackgroundArea * 100);

            }
            intTempValue = (int)Math.Round((objOcvChar.TemplateForegroundArea * m_fAreaScoreFactor) * (100 - intScore) / 100, 0);
            if (intTempValue > ((objStatisticInfo.intFgAreaMax - objStatisticInfo.intFgAreaMin) / 2))
            {
                objOcvChar.ForegroundAreaTolerance = (uint)intTempValue;
                objStatisticInfo.fFgAreaFac = 1;
            }
            else
            {
                objOcvChar.ForegroundAreaTolerance = (uint)(((objStatisticInfo.intFgAreaMax - objStatisticInfo.intFgAreaMin) / 2) * 1.5);
                objStatisticInfo.fFgAreaFac = (float)intScore / ((float)objOcvChar.ForegroundAreaTolerance / (float)objOcvChar.TemplateForegroundArea * 100);
            }

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            if (intTempValue > ((objStatisticInfo.intBgAreaMax - objStatisticInfo.intBgAreaMin) / 2))
            {
                objOcvChar.BackgroundAreaTolerance = intTempValue;
                objStatisticInfo.fBgAreaFac = 1;
            }
            else
            {
                objOcvChar.BackgroundAreaTolerance = (int)(((objStatisticInfo.intBgAreaMax - objStatisticInfo.intBgAreaMin) / 2) * 1.5);
                objStatisticInfo.fBgAreaFac = (float)intScore / ((float)objOcvChar.BackgroundAreaTolerance / (float)objOcvChar.TemplateBackgroundArea * 100);

            }
            intTempValue = (int)Math.Round((objOcvChar.TemplateForegroundArea * m_fAreaScoreFactor) * (100 - intScore) / 100, 0);
            if (intTempValue > ((objStatisticInfo.intFgAreaMax - objStatisticInfo.intFgAreaMin) / 2))
            {
                objOcvChar.ForegroundAreaTolerance = intTempValue;
                objStatisticInfo.fFgAreaFac = 1;
            }
            else
            {
                objOcvChar.ForegroundAreaTolerance = (int)(((objStatisticInfo.intFgAreaMax - objStatisticInfo.intFgAreaMin) / 2) * 1.5);
                objStatisticInfo.fFgAreaFac = (float)intScore / ((float)objOcvChar.ForegroundAreaTolerance / (float)objOcvChar.TemplateForegroundArea * 100);
            }

#endif

            fTempValue = (objOcvChar.TemplateBackgroundSum * m_fAreaScoreFactor) * (100 - intScore) / 100;
            if (fTempValue > ((objStatisticInfo.fBgSumMax - objStatisticInfo.fBgSumMin) / 2))
            {
                objOcvChar.BackgroundSumTolerance = fTempValue;
                objStatisticInfo.fBgSumFac = 1;
            }
            else
            {
                objOcvChar.BackgroundSumTolerance = (float)(((objStatisticInfo.fBgSumMax - objStatisticInfo.fBgSumMin) / 2) * 1.5);
                objStatisticInfo.fBgSumFac = (float)intScore / ((float)objOcvChar.BackgroundSumTolerance / (float)objOcvChar.TemplateBackgroundSum * 100);
            }

            fTempValue = (objOcvChar.TemplateForegroundSum * m_fAreaScoreFactor) * (100 - intScore) / 100;
            if (fTempValue > ((objStatisticInfo.fFgSumMax - objStatisticInfo.fFgSumMin) / 2))
            {
                objOcvChar.ForegroundSumTolerance = fTempValue;
                objStatisticInfo.fFgSumFac = 1;
            }
            else
            {
                objOcvChar.ForegroundSumTolerance = (float)(((objStatisticInfo.fFgSumMax - objStatisticInfo.fFgSumMin) / 2) * 1.5);
                objStatisticInfo.fFgSumFac = (float)intScore / ((float)objOcvChar.ForegroundSumTolerance / (float)objOcvChar.TemplateForegroundSum * 100);
            }

            objOcvChar.CorrelationTolerance = (100 - (float)intScore) / 100;
        }

        private void SetValueToChars(EOCVChar objOcvChar, int intLocScoreTole, int intAreaSumTole, int intCorrelationTole, int intShiftXTole, int intShiftYTole)
        {
            objOcvChar.LocationScoreTolerance = objOcvChar.TemplateLocationScore * (100 - intLocScoreTole) / 100;
#if (Debug_2_12 || Release_2_12)
            objOcvChar.BackgroundAreaTolerance = objOcvChar.TemplateBackgroundArea * (uint)(100 - intAreaSumTole) / 100;
            objOcvChar.ForegroundAreaTolerance = objOcvChar.TemplateForegroundArea * (uint)(100 - intAreaSumTole) / 100;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            objOcvChar.BackgroundAreaTolerance = objOcvChar.TemplateBackgroundArea * (100 - intAreaSumTole) / 100;
            objOcvChar.ForegroundAreaTolerance = objOcvChar.TemplateForegroundArea * (100 - intAreaSumTole) / 100;
#endif

            objOcvChar.BackgroundSumTolerance = objOcvChar.TemplateBackgroundSum * (100 - intAreaSumTole) / 100;
            objOcvChar.ForegroundSumTolerance = objOcvChar.TemplateForegroundSum * (100 - intAreaSumTole) / 100;
            objOcvChar.CorrelationTolerance = (100 - (float)intCorrelationTole) / 100;

            objOcvChar.ShiftXTolerance = intShiftXTole;
            objOcvChar.ShiftYTolerance = intShiftYTole;
        }

        private void SetValueToTexts(EOCVText objOcvText, int intScore, int intShiftTole)
        {
            if (intScore > 25)
                objOcvText.LocationScoreTolerance = objOcvText.TemplateLocationScore * 75 / 100;
            else
                objOcvText.LocationScoreTolerance = objOcvText.TemplateLocationScore * (100 - intScore) / 100;
#if (Debug_2_12 || Release_2_12)
            objOcvText.BackgroundAreaTolerance = (uint)Math.Round(objOcvText.TemplateBackgroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
            objOcvText.ForegroundAreaTolerance = (uint)Math.Round(objOcvText.TemplateForegroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            objOcvText.BackgroundAreaTolerance = (int)Math.Round(objOcvText.TemplateBackgroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
            objOcvText.ForegroundAreaTolerance = (int)Math.Round(objOcvText.TemplateForegroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
#endif

            objOcvText.BackgroundSumTolerance = objOcvText.TemplateBackgroundSum * (100 - intScore) * m_fAreaScoreFactor / 100;
            objOcvText.ForegroundSumTolerance = objOcvText.TemplateForegroundSum * (100 - intScore) * m_fAreaScoreFactor / 100;
            objOcvText.CorrelationTolerance = (100 - (float)intScore) / 100;

            objOcvText.ShiftXTolerance = intShiftTole;
            objOcvText.ShiftYTolerance = intShiftTole;
        }

        private void SetValueToTexts(EOCVText objOcvText, int intScore, int intShiftXTole, int intShiftYTole)
        {
            if (intScore > 25)
                objOcvText.LocationScoreTolerance = objOcvText.TemplateLocationScore * 75 / 100;
            else
                objOcvText.LocationScoreTolerance = objOcvText.TemplateLocationScore * (100 - intScore) / 100;
#if (Debug_2_12 || Release_2_12)
            objOcvText.BackgroundAreaTolerance = (uint)Math.Round(objOcvText.TemplateBackgroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
            objOcvText.ForegroundAreaTolerance = (uint)Math.Round(objOcvText.TemplateForegroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            objOcvText.BackgroundAreaTolerance = (int)Math.Round(objOcvText.TemplateBackgroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
            objOcvText.ForegroundAreaTolerance = (int)Math.Round(objOcvText.TemplateForegroundArea * (100 - intScore) * m_fAreaScoreFactor / 100, 0);
#endif

            objOcvText.BackgroundSumTolerance = objOcvText.TemplateBackgroundSum * (100 - intScore) * m_fAreaScoreFactor / 100;
            objOcvText.ForegroundSumTolerance = objOcvText.TemplateForegroundSum * (100 - intScore) * m_fAreaScoreFactor / 100;
            objOcvText.CorrelationTolerance = (100 - (float)intScore) / 100;

            objOcvText.ShiftXTolerance = intShiftXTole;
            objOcvText.ShiftYTolerance = intShiftYTole;
        }

        private void SetValueToTexts(EOCVText objOcvText, int intLocScoreTole, int intAreaSumTole, int intCorrelationTole, int intShiftXTole, int intShiftYTole)
        {
            objOcvText.LocationScoreTolerance = objOcvText.TemplateLocationScore * (100 - intLocScoreTole) / 100;
#if (Debug_2_12 || Release_2_12)
            objOcvText.BackgroundAreaTolerance = objOcvText.TemplateBackgroundArea * (uint)(100 - intAreaSumTole) / 100;
            objOcvText.ForegroundAreaTolerance = objOcvText.TemplateForegroundArea * (uint)(100 - intAreaSumTole) / 100;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            objOcvText.BackgroundAreaTolerance = objOcvText.TemplateBackgroundArea * (100 - intAreaSumTole) / 100;
            objOcvText.ForegroundAreaTolerance = objOcvText.TemplateForegroundArea * (100 - intAreaSumTole) / 100;
#endif

            objOcvText.BackgroundSumTolerance = objOcvText.TemplateBackgroundSum * (100 - intAreaSumTole) / 100;
            objOcvText.ForegroundSumTolerance = objOcvText.TemplateForegroundSum * (100 - intAreaSumTole) / 100;
            objOcvText.CorrelationTolerance = (100 - (float)intCorrelationTole) / 100;

            objOcvText.ShiftXTolerance = intShiftXTole;
            objOcvText.ShiftYTolerance = intShiftYTole;
        }

        public void SetTemplateBlobsFeatures(int intObjNumber, int intArea, float fWidth, float fHeight, float fAngle, float fCenterX, float fCenterY, int intOcvMatchNumber)
        {
            if (intObjNumber < 0)
                return;
            m_stcBlobsFeatures.intObjNumber = intObjNumber;
            m_stcBlobsFeatures.intArea = intArea;
            m_stcBlobsFeatures.fWidth = fWidth;
            m_stcBlobsFeatures.fHeight = fHeight;
            m_stcBlobsFeatures.fAngle = fAngle;
            m_stcBlobsFeatures.fCenterX = fCenterX;
            m_stcBlobsFeatures.fCenterY = fCenterY;
            m_stcBlobsFeatures.intOcvMatchNumber = intOcvMatchNumber;
            m_stcBlobsFeatures.blnMatch = false;
            m_arrBlobsFeatures.Add(m_stcBlobsFeatures);
        }

        private void SplitMarkFromBackground(ImageDrawing objImage, int intThresholdValue)
        {
            if (intThresholdValue < 0)
                intThresholdValue = 0;
            if (intThresholdValue > 255)
                intThresholdValue = 255;

            EBW8 objPixel = new EBW8((byte)intThresholdValue);
            int intStartX, intStartY, intEndX, intEndY, x, y;
            for (int i = 0; i < m_intCharsCount; i++)
            {
                intStartX = m_arrSampleOCV[i].intStartX;
                intStartY = m_arrSampleOCV[i].intStartY;
                intEndX = m_arrSampleOCV[i].intEndX;
                intEndY = m_arrSampleOCV[i].intEndY;

                for (x = intStartX; x < intEndX; x++)
                {
                    objImage.ref_objMainImage.SetPixel(objPixel, x, intStartY);
                    objImage.ref_objMainImage.SetPixel(objPixel, x, intEndY);
                }

                for (y = intStartY; y < intEndY; y++)
                {
                    objImage.ref_objMainImage.SetPixel(objPixel, intStartX, y);
                    objImage.ref_objMainImage.SetPixel(objPixel, intEndX, y);
                }
            }
        }
    }
}
