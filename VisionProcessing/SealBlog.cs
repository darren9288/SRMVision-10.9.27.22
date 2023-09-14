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
using System.IO;

namespace VisionProcessing
{
    public class SealBlog
    {
        #region constant variables

        private const int m_intFeature = 7;
        private const int m_intConnexity = 4;

        #endregion

        #region enum

        public enum ResultType
        {
            TopSealWidth = 0,
            BtmSealWidth = 1,
            Distance = 2,
            TopSealSmallestWidth = 3,
            TopSealLargestWidth = 4,
            BtmSealSmallestWidth = 5,
            BtmSealLargestWidth = 6,
            SprocketHoleDistance = 7,
            TopSealSmallestWidth_PassOnly = 8,
            TopSealLargestWidth_PassOnly = 9,
            TopSealCenterY_PassOnly = 10,
            BtmSealSmallestWidth_PassOnly = 11,
            BtmSealLargestWidth_PassOnly = 12,
            BtmSealCenterY_PassOnly = 13,
            SprocketHoleDiameter = 14,
            SprocketHoleDefect = 15,
            SprocketHoleBroken = 16,
            SprocketHoleRoundness = 17,
            SealEdgeStraightness = 18,
        };

        #endregion

        #region Member Variables
        private bool[] m_arrFailOverHeat = new bool[5] { false, false, false, false, false };
        private bool[] m_arrFailScratches = new bool[5] { false, false, false, false, false };
        private float m_fCircleGaugeShiftX = 0;
        private int m_intSpocketHolePosition = 0; //0: Top, 1: Bottom
        private bool m_blnCircleGaugeOutOfImageRange = false;
        /// <summary>
        /// 0x01: Seal Width
        /// 0x02: Seal Bubble
        /// 0x04: Seal Shifted
        /// 0x08: Seal Distance
        /// 0x10: Seal Over Heat
        /// 0x20: Seal Broken
        /// 0x40: Check Unit Present
        /// 0x80: Check Unit Orientation
        /// 0x100: Check Sprocket hole Distance
        /// 0x200: Check Sprocket hole Diameter                          //0x400: Check Tape Scratches (Dark Field on tape)
        /// 0x400: Check Sprocket hole Defect
        /// 0x800: Check Sprocket hole Broken
        /// 0x1000: Check Sprocket hole Roundness
        /// </summary>
        private int m_intFailOptionMask = 0;
        private int m_intSealEdgeSensitivity = 0;
        private int m_intSprocketHoleInspectionAreaInwardTolerance = 0;
        private int m_intSprocketHoleBrokenOutwardTolerance_Outer = 0;
        private int m_intSprocketHoleBrokenOutwardTolerance_Inner = 0;
        private int m_intSealEdgeTolerance = 3;
        private int[] m_intSealLineEdgeThreshold = { 125, 125 };
        private int[] m_intSealBrokenAreaThreshold = { 125, 125 };
        private bool m_blnViewSegmentDrawing = false;
        private bool m_blnWantUsePatternCheckUnitPresent = true;
        private bool m_blnWantUsePixelCheckUnitPresent = true;
        private bool m_blnWhiteOnBlack = false;
        private bool m_blnWantDontCareArea = false;
        private bool m_blnWantCheckSealEdgeStraightness = false;
        private bool m_CheckUnitOrient = false;
        private float m_fMarkAreaBelowPercent = 0.03f;  // Default: Unit is considered no mark if fore area cover unit area 3 % only.
        public float m_fImageMatchScore = 0;
        private int m_intPatternAngleTolerance = 10;    // Default 10 deg
        private int m_intCheckMarkMethod = 0; // 0: Normal inspection using pattern match; 1: using standard deviation of pixel 

        private float m_fShiftPositionTolerance = 0;
        private int m_intBuildObjectLength = 10;
        /*
        *0x01	[Bypass user right option : other]
                - Fail Position - No Pattern Learnt
	            - Fail Position - Pattern no found
	            - Fail To Find Sprocket Hole Position.
	            - Position Shifted
        *0x02	[Bypass user right option : seal distance]
	            - Fail to find seal distance blob		
	            - Fail minimum seal distance
        *0x04	[Bypass user right option : seal bubble]
    	        - Bubble is present
        *0x08	[Bypass user right option : seal broken]
	            - No seal was found
	            - Broken Seal
        *0x10	[Bypass user right option : seal width]
    	        - Fail Line Width overseal
        *0x20	[Bypass user right option : seal width]
	            - Fail Line Width insuffcient
        *0x40 	[Bypass user right option : over heat/scratches]
	            - over heat
        *0x80	[Bypass user right option : unit present]
	            - Fail Empty Pocket - Search ROI too small to find "mark" pattern	(in Fail empty pocket function)	something wronng here
	            - Fail Unit Mark - No Pattern Learnt
	            - Fail Unit Mark - Search ROI too small to find mark pattern
	            - Fail Unit Mark - Not fulfill Min Setting
	            - Fail Unit Mark - Find 0 Pattern in Search ROI
	            - Fail Unit Mark - Set white on black pixel 	// set 0x400 also
	            - Fail Orient Angle
        0x100	[Bypass user right option : Check Empty]
                - Fail Empty Pocket - Template no exist
	            - Fail Empty Pocket - No Pattern Learn
	            - Fail Empty Pocket - No Fullfill min setting
	            - Fail Empty Pocket - Find 0 pattern in search ROI
        0x100 	- Fail Empty Pocket - No Pattern Learnt	// for no empty test    
	            - Fail Empty Pocket - Fulfill Min Setting	// for no empty test
        *0x200	[Bypass user right option : sprocket hole distance]
	            - Fail Gauge Measurement for sprocket hole
	            - Fail Sprocket Hole Distance
        *0x800	[Bypass user right option : overheat/scratches]
	            - tape scratches
         0x1000 - Fail Unit Orientation
    */
        private int m_intSealFailMask = 0; // 1 = shift position; 2=distance; 4=bubble found; 8=broken line; 10=overseal; 20=insufficient 40=overheat 80=mark 100=pocket, 200=Sprocket Hole Distance, 0x400=To separate defect from unit present for displaying on offline page, 0x800=Tape Scratches fail, 0x1000=Orientation, 0x2000=Sprocket Hole Diameter, 0x4000=Sprocket Hole Defect, 0x8000=Sprocket Hole Broken, 0x10000=Sprocket Hole Roundness, 0x20000=Seal Edge Straightness, 0x40000=No Template
        private bool m_blnFailSeal1 = false;
        private bool m_blnFailSeal2 = false;
        private int m_intBuildCount = 0;
        private int m_intTapePocketPitch = 0;
        private float m_fTapePocketPitchByImage = -1;   // Set default to -1. If value is -1 during inspection, then just use m_intTapePocketPitch.
        //private int m_intTapeSproketPitch = 0;
        //private int m_intPitchPerSproket = 1;
        private List<int> m_arrGrabImageIndex = new List<int>();   // Record grab image index for different defect checking. // 0: seal, 1: Mark, pocket and overheat
        private int m_intSelectedSealObject = -1;

        private float m_minSealBrokenGap2 = 0;
        private float m_minSealBrokenGap = 0;
        private float m_fWidthLowerTolerance = 0;
        private float m_fWidthLowerTolerance1 = 0;
        private float m_fWidthLowerTolerance2 = 0;
        private float m_fWidthUpperTolerance = 0;
        private float m_fWidthUpperTolerance1 = 0;
        private float m_fWidthUpperTolerance2 = 0;
        private float m_fDistanceMinTolerance = 0;
        private float m_fDistanceMaxTolerance = 0;
        private float m_fSealEdgeStraightnessMaxTolerance = 0;
        private float m_fSprocketHoleDistanceMinTolerance = 0;
        private float m_fSprocketHoleDistanceMaxTolerance = 0;
        private float m_fSprocketHoleDiameterMinTolerance = 0;
        private float m_fSprocketHoleDiameterMaxTolerance = 0;
        private float m_fSprocketHoleDefectMaxTolerance = 0;
        private float m_fSprocketHoleBrokenMaxTolerance = 0;
        private float m_fSprocketHoleRoundnessMaxTolerance = 0.4f; // 0(Min) means round, 1 means oval
        private float[] m_arrOverHeatAreaMinTolerance = new float[5] { 0, 0, 0, 0, 0 };
        private float[] m_arrScratchesAreaMinTolerance = new float[5] { 0, 0, 0, 0, 0 };
        private float[] m_arrSealAreaTolerance = new float[2];
        private string m_strErrorMessage = "";
        private string m_strPackageName = "";
        private float m_fSealScoreTolerance = 0.7f; // Score based on number of line gauge (m_intBuildObjectLength). if total line gauge is 10 and score is 70%, 7/10 line passed will pass the test.
        private bool m_blnWantSkipOrient = false;
        private bool m_blnWantSkipSprocketHole = true;
        private bool m_blnWantSkipSprocketHoleDiameterAndDefect = true;
        private bool m_blnWantSkipSprocketHoleBrokenAndRoundness = true;
        private float[] m_fFailSealScore = new float[2];
        // template
        private int m_intHoleMinArea1 = 0;
        private int m_intHoleMinArea2 = 0;
        private float m_fMinBrokenWidth = 0;
        private float[] m_fTemplateWidth = new float[3];
        private int[] m_intTemplateAreaAVG = new int[2];
        private int m_intPocketTemplateIndex = 0;
        private int m_intMarkTemplateIndex = 0;
        private int m_intPositionTemplateIndex = 0;
        private PointF m_pPositionCenterPoint = new PointF(0, 0);
        private int m_intPocketTemplateNumSelected = 0;
        private int m_intMarkTemplateNumSelected = 0;
        private int m_intMarkMatchIndexSelected = 0;
        private int m_intPositionTemplateNumSelected = 0;
        private int m_intMarkPixelThreshold = 125;
        private List<Point> m_arrTemplateMarkROIPosition = new List<Point>();
        private List<Size> m_arrTemplateMarkROISize = new List<Size>();
        private List<int> m_arrTemplateMarkThreshold = new List<int>();
        private List<float> m_arrTemplateMarkThresholdRelative = new List<float>();
        private List<bool> m_arrTemplateWantAutoThresholdRelative = new List<bool>();
        private List<int> m_arrTemplateMarkWhitePixel = new List<int>();
        private List<int> m_arrTemplateMarkBlackPixel = new List<int>();
        private List<List<string>> m_arrTemplateImageProcessSeq = new List<List<string>>();
        private List<int> m_arrTemplateErodeValue = new List<int>();
        private List<int> m_arrTemplateDilateValue = new List<int>();
        private List<int> m_arrTemplateOpenValue = new List<int>();
        private List<int> m_arrTemplateCloseValue = new List<int>();
        private List<int> m_arrTemplateErodeMinValue = new List<int>();
        private List<int> m_arrTemplateDilateMinValue = new List<int>();
        private List<int> m_arrTemplateOpenMinValue = new List<int>();
        private List<int> m_arrTemplateCloseMinValue = new List<int>();
        private List<int> m_arrTemplateThresholdMinValue = new List<int>();
        private List<int> m_arrTemplateErodeMaxValue = new List<int>();
        private List<int> m_arrTemplateDilateMaxValue = new List<int>();
        private List<int> m_arrTemplateOpenMaxValue = new List<int>();
        private List<int> m_arrTemplateCloseMaxValue = new List<int>();
        private List<int> m_arrTemplateThresholdMaxValue = new List<int>();
        private int m_intTemplateErodeTol = 0;
        private int m_intTemplateDilateTol = 0;
        private int m_intTemplateOpenTol = 0;
        private int m_intTemplateCloseTol = 0;
        private int m_intTemplateThresholdTol = 0;
        // result index
        //0 = Top Seal Width
        //1 = Bottom Seal Width
        //2 = Distance Result
        //3 = Top Seal Smallest Width
        //4 = Top Seal Largest width
        //5 = Bottom Seal Smallest Width
        //6 = Bottom Seal Largest Width
        //7 = Sprocket Hole distance
        //8 = Top Seal Small Distance Pass Only
        //9 = Top Seal Largest Distance Pass Only
        //10= Top Seal Center Y Pass Only
        //11 = Bottom Seal Small Distance Pass Only
        //12 = Bottom Seal Largest Distance Pass Only
        //13 = Bottom Seal Center Y Pass Only
        //14 = Sprocket Hole diameter
        //15 = Sprocket Hole defect
        //16 = Sprocket Hole broken
        //17 = Sprocket Hole roundness
        private float[] m_fLineWidthAverage = new float[19];

        private List<ArrayList> m_arrGaugePositions = new List<ArrayList>();        // store line gauge points position

        private List<EBlobs> m_arrBlackBlobs = new List<EBlobs>();  // For Seal bar which is black color
        private List<EBlobs> m_arrWhiteBlobs = new List<EBlobs>();  // For Seal broken or bubble which is white color
        private EBlobs[] m_arrOverHeatBlobs = new EBlobs[5]{ new EBlobs(), new EBlobs(), new EBlobs(), new EBlobs(), new EBlobs() };
        private EBlobs[] m_arrScratchesBlobs = new EBlobs[5] { new EBlobs(), new EBlobs(), new EBlobs(), new EBlobs(), new EBlobs() };
        private EBlobs m_objDistanceBlobs = new EBlobs();
        private EBlobs m_objSealEdgeStraightnessBlobs = new EBlobs();
        private EBlobs m_objSprocketHoleDefectBlobs = new EBlobs();
        private EBlobs m_objSprocketHoleBrokenBlobs = new EBlobs();
        private EBlobs m_objSprocketHoleRoundnessBlobs = new EBlobs();
        private EBlobs m_objLearnBlobs = new EBlobs();
        private EBlobs m_objSprocketBlobs = new EBlobs();

        private Font m_Font = new Font("Verdana", 10);
        private SolidBrush m_Brush = new SolidBrush(Color.Yellow);
        private List<List<SolidBrush>> m_arrBrushMatched = new List<List<SolidBrush>>();

        private ImageDrawing m_objTemplateCircleImage = new ImageDrawing();

        // Inspection Result Data
        private List<List<PointF>> m_arrFailWidthStartPoint = new List<List<PointF>>();
        private List<List<PointF>> m_arrFailWidthEndPoint = new List<List<PointF>>();
        private List<List<int>> m_arrFailBlobsIndex = new List<List<int>>();
        private List<List<int>> m_arrFailBlobsHoleIndex = new List<List<int>>();
        private List<List<int>> m_arrPassBlobsIndex = new List<List<int>>();
        private List<PointF> m_arrFailSealBrokenStartPoint = new List<PointF>();
        private List<PointF> m_arrFailSealBrokenEndPoint = new List<PointF>();

        private ImageDrawing m_objSealLineImage;
        private EImageBW8 m_imgSealLineImage = new EImageBW8();
        private ROI m_objThresholdSealROI = new ROI();
        private float m_fCalibY = 0;

        // PRS for pattern match
        private List<List<EMatcher>> m_arrMarkMatcher = new List<List<EMatcher>>(); // m_arrMatcher[4 different angle][Template Index (0-3)]
        private List<ImageDrawing> m_arrMatcherImage = new List<ImageDrawing>();
        private List<ImageDrawing> m_arrMatcherThresholdImage = new List<ImageDrawing>();
        private List<List<float>> m_arrMatcherScoreRecord = new List<List<float>>();
        //private ImageDrawing m_objMatcherImage = new ImageDrawing(true);
        private ImageDrawing m_objMatcherThresholdImage = new ImageDrawing(true);
        private ImageDrawing m_objTempComparedImage = new ImageDrawing(true);
        private ImageDrawing m_objTempSampleImage = new ImageDrawing(true);
        private ImageDrawing m_objTempSampleThresholdImage = new ImageDrawing(true);
        private ROI m_objSampleThresholdROI = new ROI();
        private ROI m_objSampleSearchThresholdROI = new ROI();
        private ROI m_objSampleComparedROI = new ROI();
        private ROI m_objTemplateImageMatcherROI = new ROI();
        private List<EMatcher> m_arrPocketMatcher = new List<EMatcher>();
        private List<EMatcher> m_arrPositionMatcher = new List<EMatcher>();

        private PointF m_pResultPositionCenterPoint = new PointF();
        private PointF m_pResultPocketCenterPoint = new PointF();
        private PointF m_pResultMarkCenterPoint = new PointF();
        private SizeF m_SResultMarkSize = new SizeF();
        private float m_fResultMarkAngle = 0;
        //private float m_fUnitAngle = 0f;
        private float m_fPocketMinScore = 0.7f;
        private float m_fMarkMinScore = 0.7f;
        private float m_fMarkTemplateWhiteArea = 1;
        private float m_fMarkMinWhiteArea = 0;
        private float m_fMarkMaxWhiteArea = 0;
        private float[] m_fPocketMatchScore = new float[4];
        private float[] m_fMarkMatchScore = new float[8];
        private int m_intAngleResult = 4;
        private int m_intDirections = 4;
        private float m_fShiftedX = 0;
        private float m_fShiftedY = 0;
        private float[] m_arrFailOverheatArea = new float[5] { 0, 0, 0, 0, 0 };
        private float[] m_arrFailScratchesArea = new float[5] { 0, 0, 0, 0, 0 };
        private float m_FailSealEdgeStraightnessArea = 0f;
        private float m_FailUnitPresentWhiteArea = 0;
        private float m_fSealBorderPositionY = 0;
        private float m_FailDistance = 0f;
        private float m_FailSprocketHoleDistance = 0f;
        private float m_FailSprocketHoleDiameter = 0f;
        private float m_FailSprocketHoleDefectArea = 0f;
        private float m_FailSprocketHoleBrokenArea = 0f;
        private float m_FailSprocketHoleRoundness = 0f;
        private float[] m_FailBubble = new float[2];
        private float[] m_FailBrokenSeal = new float[2];
        private float[] m_FailOverSeal = new float[2];
        private float[] m_FailInsufficient = new float[2];
        private float m_FailPosition = 0f;

        private int m_intStdDevTol = 15;

        ImageDrawing m_objTempImage = null;
        ROI m_objTempROI = null;

        TrackLog t = new TrackLog();
        #endregion

        #region Properties 
        public float ref_fCircleGaugeShiftX { get { return m_fCircleGaugeShiftX; } }
        public float ref_fShiftedX { get { return m_fShiftedX; } }
        public float ref_fShiftedY { get { return m_fShiftedY; } }
        public int ref_intSpocketHolePosition { get { return m_intSpocketHolePosition; } set { m_intSpocketHolePosition = value; } }
        public ImageDrawing ref_objTemplateCircleImage { get { return m_objTemplateCircleImage; } }
        public int ref_intMarkTemplateNumSelected { get { return m_intMarkTemplateNumSelected; } set { m_intMarkTemplateNumSelected = value; } }
        public int ref_intCheckMarkMethod { get { return m_intCheckMarkMethod; } set { m_intCheckMarkMethod = value; } }
        public int ref_intAngleResult { get { return m_intAngleResult; } set { m_intAngleResult = value; } }
        public bool ref_CheckUnitOrient { get { return m_CheckUnitOrient; } set { m_CheckUnitOrient = value; } }
        public float ref_MinBrokenSealGap { get { return m_minSealBrokenGap; } set { m_minSealBrokenGap = value; } }
        public float ref_MinBrokenSealGap2 { get { return m_minSealBrokenGap2; } set { m_minSealBrokenGap2 = value; } }

        public bool ref_blnFailSeal1 { get { return m_blnFailSeal1; } }
        public bool ref_blnFailSeal2 { get { return m_blnFailSeal2; } }
        public bool ref_blnPositionPatternLearnt { get { return m_arrPositionMatcher[0].PatternLearnt; } }
        public bool ref_blnWantUsePatternCheckUnitPresent { get { return m_blnWantUsePatternCheckUnitPresent; } set { m_blnWantUsePatternCheckUnitPresent = value; } }
        public bool ref_blnWantUsePixelCheckUnitPresent { get { return m_blnWantUsePixelCheckUnitPresent; } set { m_blnWantUsePixelCheckUnitPresent = value; } }
        public bool ref_blnWhiteOnBlack { get { return m_blnWhiteOnBlack; } set { m_blnWhiteOnBlack = value; } }
        public bool ref_blnWantDontCareArea { get { return m_blnWantDontCareArea; } set { m_blnWantDontCareArea = value; } }
        public bool ref_blnWantCheckSealEdgeStraightness { get { return m_blnWantCheckSealEdgeStraightness; } set { m_blnWantCheckSealEdgeStraightness = value; } }
        public float ref_fMarkAreaBelowPercent { get { return m_fMarkAreaBelowPercent; } set { m_fMarkAreaBelowPercent = value; } }
        public int ref_intPatternAngleTolerance { get { return m_intPatternAngleTolerance; } set { m_intPatternAngleTolerance = value; } }
        public int ref_intFailOptionMaskSeal { get { return m_intFailOptionMask; } set { m_intFailOptionMask = value; } }
        public int ref_intSealEdgeSensitivity { get { return m_intSealEdgeSensitivity; } set { m_intSealEdgeSensitivity = value; } }
        public int ref_intSealEdgeTolerance { get { return m_intSealEdgeTolerance; } set { m_intSealEdgeTolerance = value; } }
        public int ref_intSprocketHoleInspectionAreaInwardTolerance { get { return m_intSprocketHoleInspectionAreaInwardTolerance; } set { m_intSprocketHoleInspectionAreaInwardTolerance = value; } }
        public int ref_intSprocketHoleBrokenOutwardTolerance_Outer { get { return m_intSprocketHoleBrokenOutwardTolerance_Outer; } set { m_intSprocketHoleBrokenOutwardTolerance_Outer = value; } }
        public int ref_intSprocketHoleBrokenOutwardTolerance_Inner { get { return m_intSprocketHoleBrokenOutwardTolerance_Inner; } set { m_intSprocketHoleBrokenOutwardTolerance_Inner = value; } }
        public float ref_FailPosition { get { return m_FailPosition; } set { m_FailPosition = value; } }
        public float ref_FailOverSeal1 { get { return m_FailOverSeal[0]; } set { m_FailOverSeal[0] = value; } }
        public float ref_FailInsufficient1 { get { return m_FailInsufficient[0]; } set { m_FailInsufficient[0] = value; } }
        public float ref_FailOverSeal2 { get { return m_FailOverSeal[1]; } set { m_FailOverSeal[1] = value; } }
        public float ref_FailInsufficient2 { get { return m_FailInsufficient[1]; } set { m_FailInsufficient[1] = value; } }
        public float ref_FailBrokenSeal1 { get { return m_FailBrokenSeal[0]; } set { m_FailBrokenSeal[0] = value; } }
        public float ref_FailBubble1 { get { return m_FailBubble[0]; } set { m_FailBubble[0] = value; } }
        public float ref_FailBrokenSeal2 { get { return m_FailBrokenSeal[1]; } set { m_FailBrokenSeal[1] = value; } }
        public float ref_FailBubble2 { get { return m_FailBubble[1]; } set { m_FailBubble[1] = value; } }
        public float ref_FailDistance { get { return m_FailDistance; } set { m_FailDistance = value; } }
        public float ref_FailSprocketHoleDistance { get { return m_FailSprocketHoleDistance; } set { m_FailSprocketHoleDistance = value; } }
        public float ref_FailSprocketHoleDiameter { get { return m_FailSprocketHoleDiameter; } set { m_FailSprocketHoleDiameter = value; } }
        public float ref_FailSprocketHoleDefectArea { get { return m_FailSprocketHoleDefectArea; } set { m_FailSprocketHoleDefectArea = value; } }
        public float ref_FailSprocketHoleBrokenArea { get { return m_FailSprocketHoleBrokenArea; } set { m_FailSprocketHoleBrokenArea = value; } }
        public float ref_FailSprocketHoleRoundness { get { return m_FailSprocketHoleRoundness; } set { m_FailSprocketHoleRoundness = value; } }
        //public float ref_FailOverheatArea { get { return m_FailOverheatArea; } set { m_FailOverheatArea = value; } }
        //public float ref_FailScratchesArea { get { return m_FailScratchesArea; } set { m_FailScratchesArea = value; } }
        public float ref_FailSealEdgeStraightnessArea { get { return m_FailSealEdgeStraightnessArea; } set { m_FailSealEdgeStraightnessArea = value; } }
        public float ref_FailUnitPresentWhiteArea { get { return m_FailUnitPresentWhiteArea; } set { m_FailUnitPresentWhiteArea = value; } }
        public int ref_intSelectedSealObject { get { return m_intSelectedSealObject; } set { m_intSelectedSealObject = value; } }
        public bool ref_blnViewSegmentDrawing { get { return m_blnViewSegmentDrawing; } set { m_blnViewSegmentDrawing = value; } }
        public int ref_intSeal1AreaFilter { get { return m_arrBlackBlobs[0].ref_intMinAreaLimit; } set { m_arrBlackBlobs[0].SetObjectAreaRange(value, 99999); } }
        public int ref_intSeal2AreaFilter { get { return m_arrBlackBlobs[1].ref_intMinAreaLimit; } set { m_arrBlackBlobs[1].SetObjectAreaRange(value, 99999); } }
        //public int ref_intOverHeatMinArea { get { return m_objOverHeatBlobs.ref_intMinAreaLimit; } set { m_objOverHeatBlobs.SetObjectAreaRange(value, 99999); } }
        //public int ref_intScratchesMinArea { get { return m_objScratchesBlobs.ref_intMinAreaLimit; } set { m_objScratchesBlobs.SetObjectAreaRange(value, 99999); } }
        //public float ref_fOverHeatAreaMinTolerance { get { return m_fOverHeatAreaMinTolerance; } set { m_fOverHeatAreaMinTolerance = value; } }
        public float ref_fSprocketHoleDefectMaxTolerance { get { return m_fSprocketHoleDefectMaxTolerance; } set { m_fSprocketHoleDefectMaxTolerance = value; } }
        public float ref_fSprocketHoleBrokenMaxTolerance { get { return m_fSprocketHoleBrokenMaxTolerance; } set { m_fSprocketHoleBrokenMaxTolerance = value; } }
        public float ref_fSprocketHoleRoundnessMaxTolerance { get { return m_fSprocketHoleRoundnessMaxTolerance; } set { m_fSprocketHoleRoundnessMaxTolerance = value; } }
        //public float ref_fScratchesAreaMinTolerance { get { return m_fScratchesAreaMinTolerance; } set { m_fScratchesAreaMinTolerance = value; } }
        public int ref_intHoleMinArea1 { get { return m_intHoleMinArea1; } set { m_intHoleMinArea1 = value; } }
        public int ref_intHoleMinArea2 { get { return m_intHoleMinArea2; } set { m_intHoleMinArea2 = value; } }
        public float ref_fMinBrokenWidth { get { return m_fMinBrokenWidth; } set { m_fMinBrokenWidth = value; } }
        public int ref_intMarkPixelThreshold { get { return m_intMarkPixelThreshold; } set { m_intMarkPixelThreshold = value; } }
        public int ref_intStdDevTol { get { return m_intStdDevTol; } set { m_intStdDevTol = value; } }

        public int ref_intBuildObjectLength { get { return m_intBuildObjectLength; } set { m_intBuildObjectLength = value; } }
        public int ref_intTapePocketPitch { get { return m_intTapePocketPitch; } set { m_intTapePocketPitch = value; } }
        //public int ref_intTapeSproketPitch { get { return m_intTapeSproketPitch; } set { m_intTapeSproketPitch = value; } }
        //public int ref_intPitchPerSproket { get { return m_intPitchPerSproket; } set { m_intPitchPerSproket = value; } }
        //public int ref_intSeal1Threshold { get { return m_arrBlackBlobs[0].ref_intAbsoluteThreshold; } set { m_arrBlackBlobs[0].ref_intAbsoluteThreshold = value; } }
        //public int ref_intSeal2Threshold { get { return m_arrBlackBlobs[1].ref_intAbsoluteThreshold; } set { m_arrBlackBlobs[1].ref_intAbsoluteThreshold = value; } }
        public int ref_intSeal1Threshold { get { return m_intSealLineEdgeThreshold[0]; } set { m_intSealLineEdgeThreshold[0] = value; } }
        public int ref_intSeal2Threshold { get { return m_intSealLineEdgeThreshold[1]; } set { m_intSealLineEdgeThreshold[1] = value; } }
        public int ref_intSeal1BrokenAreaThreshold { get { return m_intSealBrokenAreaThreshold[0]; } set { m_intSealBrokenAreaThreshold[0] = value; } }
        public int ref_intSeal2BrokenAreaThreshold { get { return m_intSealBrokenAreaThreshold[1]; } set { m_intSealBrokenAreaThreshold[1] = value; } }

        //public int ref_intOverHeatThreshold { get { return m_objOverHeatBlobs.ref_intAbsoluteThreshold; } set { m_objOverHeatBlobs.ref_intAbsoluteThreshold = value; } }
        //public int ref_intOverHeatLowThreshold { get { return m_objOverHeatBlobs.ref_intAbsoluteLowThreshold; } set { m_objOverHeatBlobs.ref_intAbsoluteLowThreshold = value; } }
        //public int ref_intOverHeatHighThreshold { get { return m_objOverHeatBlobs.ref_intAbsoluteHighThreshold; } set { m_objOverHeatBlobs.ref_intAbsoluteHighThreshold = value; } }
        //public int ref_intScratchesThreshold { get { return m_objScratchesBlobs.ref_intAbsoluteThreshold; } set { m_objScratchesBlobs.ref_intAbsoluteThreshold = value; } }
        public int ref_intDistanceThreshold { get { return m_objDistanceBlobs.ref_intAbsoluteThreshold; } set { m_objDistanceBlobs.ref_intAbsoluteThreshold = value; } }
        public int ref_intSealEdgeStraightnessThreshold { get { return m_objSealEdgeStraightnessBlobs.ref_intAbsoluteThreshold; } set { m_objSealEdgeStraightnessBlobs.ref_intAbsoluteThreshold = value; } }
        public int ref_intSprocketHoleDefectThreshold { get { return m_objSprocketHoleDefectBlobs.ref_intAbsoluteThreshold; } set { m_objSprocketHoleDefectBlobs.ref_intAbsoluteThreshold = value; } }
        public int ref_intSprocketHoleBrokenThreshold { get { return m_objSprocketHoleBrokenBlobs.ref_intAbsoluteThreshold; } set { m_objSprocketHoleBrokenBlobs.ref_intAbsoluteThreshold = value; } }
        public int ref_intSprocketHoleRoundnessThreshold { get { return m_objSprocketHoleRoundnessBlobs.ref_intAbsoluteThreshold; } set { m_objSprocketHoleRoundnessBlobs.ref_intAbsoluteThreshold = value; } }
        public float ref_fShiftPositionTolerance { get { return m_fShiftPositionTolerance; } set { m_fShiftPositionTolerance = value; } }
        public float ref_fWidthLowerTolerance { get { return m_fWidthLowerTolerance; } set { m_fWidthLowerTolerance = value; } }
        public float ref_fWidthLowerTolerance1 { get { return m_fWidthLowerTolerance1; } set { m_fWidthLowerTolerance1 = value; } }
        public float ref_fWidthLowerTolerance2 { get { return m_fWidthLowerTolerance2; } set { m_fWidthLowerTolerance2 = value; } }
        public float ref_fWidthUpperTolerance { get { return m_fWidthUpperTolerance; } set { m_fWidthUpperTolerance = value; } }
        public float ref_fWidthUpperTolerance1 { get { return m_fWidthUpperTolerance1; } set { m_fWidthUpperTolerance1 = value; } }
        public float ref_fWidthUpperTolerance2 { get { return m_fWidthUpperTolerance2; } set { m_fWidthUpperTolerance2 = value; } }
        public float ref_fDistanceMinTolerance { get { return m_fDistanceMinTolerance; } set { m_fDistanceMinTolerance = value; } } /////
        public float ref_fDistanceMaxTolerance { get { return m_fDistanceMaxTolerance; } set { m_fDistanceMaxTolerance = value; } } /////
        public float ref_fSealEdgeStraightnessMaxTolerance { get { return m_fSealEdgeStraightnessMaxTolerance; } set { m_fSealEdgeStraightnessMaxTolerance = value; } }
        public float ref_fSprocketHoleDistanceMinTolerance { get { return m_fSprocketHoleDistanceMinTolerance; } set { m_fSprocketHoleDistanceMinTolerance = value; } }
        public float ref_fSprocketHoleDistanceMaxTolerance { get { return m_fSprocketHoleDistanceMaxTolerance; } set { m_fSprocketHoleDistanceMaxTolerance = value; } }
        public float ref_fSprocketHoleDiameterMinTolerance { get { return m_fSprocketHoleDiameterMinTolerance; } set { m_fSprocketHoleDiameterMinTolerance = value; } }
        public float ref_fSprocketHoleDiameterMaxTolerance { get { return m_fSprocketHoleDiameterMaxTolerance; } set { m_fSprocketHoleDiameterMaxTolerance = value; } }

        public float[] ref_arrSealAreaTolerance { get { return m_arrSealAreaTolerance; } set { m_arrSealAreaTolerance = value; } }
        public string ref_strErrorMessage { get { return m_strErrorMessage; } set { m_strErrorMessage = value; } }
        //public string ref_strPackageName { get { return m_strPackageName; } set { m_strPackageName = value; } }
        public int ref_intDirections { get { return m_intDirections; } set { m_intDirections = value; } }
        public bool ref_blnWantSkipOrient { get { return m_blnWantSkipOrient; } set { m_blnWantSkipOrient = value; } }
        public bool ref_blnWantSkipSprocketHole { get { return m_blnWantSkipSprocketHole; } set { m_blnWantSkipSprocketHole = value; } }
        public bool ref_blnWantSkipSprocketHoleDiameterAndDefect { get { return m_blnWantSkipSprocketHoleDiameterAndDefect; } set { m_blnWantSkipSprocketHoleDiameterAndDefect = value; } }
        public bool ref_blnWantSkipSprocketHoleBrokenAndRoundness { get { return m_blnWantSkipSprocketHoleBrokenAndRoundness; } set { m_blnWantSkipSprocketHoleBrokenAndRoundness = value; } }
        
        // template
        public float[] ref_fTemplateWidth { get { return m_fTemplateWidth; } set { m_fTemplateWidth = value; } }
        public int[] ref_intTemplateAreaAVG { get { return m_intTemplateAreaAVG; } set { m_intTemplateAreaAVG = value; } }
        public int ref_intPositionTemplateIndex { get { return m_intPositionTemplateIndex; } set { m_intPositionTemplateIndex = value; } }
        public int ref_intPocketTemplateIndex { get { return m_intPocketTemplateIndex; } set { m_intPocketTemplateIndex = value; } }
        public int ref_intMarkTemplateIndex { get { return m_intMarkTemplateIndex; } set { m_intMarkTemplateIndex = value; } }
        public float ref_intPositionCenterPointX { get { return m_pPositionCenterPoint.X; } set { m_pPositionCenterPoint.X = value; } }
        public float ref_intPositionCenterPointY { get { return m_pPositionCenterPoint.Y; } set { m_pPositionCenterPoint.Y = value; } }

        // result
        public float[] ref_fLineWidthAverage { get { return m_fLineWidthAverage; } }
        public List<ArrayList> ref_arrGaugePositions { get { return m_arrGaugePositions; } }
        public int ref_intSealFailMask { get { return m_intSealFailMask; } set { m_intSealFailMask = value; } }
        public float ref_fPocketMinScore { get { return m_fPocketMinScore; } set { m_fPocketMinScore = value; } }
        public float ref_fMarkMinScore { get { return m_fMarkMinScore; } set { m_fMarkMinScore = value; } }
        public float ref_fMarkMinWhiteArea { get { return m_fMarkMinWhiteArea; } set { m_fMarkMinWhiteArea = value; } }
        public float ref_fMarkMaxWhiteArea { get { return m_fMarkMaxWhiteArea; } set { m_fMarkMaxWhiteArea = value; } }
        public float ref_fSealScoreTolerance { get { return m_fSealScoreTolerance; } set { m_fSealScoreTolerance = value; } }
        public float ref_fFailSealScore1 { get { return m_fFailSealScore[0]; } set { m_fFailSealScore[0] = value; } }
        public float ref_fFailSealScore2 { get { return m_fFailSealScore[1]; } set { m_fFailSealScore[1] = value; } }
        public PointF ref_pResultPositionCenterPoint { get { return m_pResultPositionCenterPoint; } set { m_pResultPositionCenterPoint = value; } }
        public PointF ref_pResultPocketCenterPoint { get { return m_pResultPocketCenterPoint; } set { m_pResultPocketCenterPoint = value; } }
        public PointF ref_pResultMarkCenterPoint { get { return m_pResultMarkCenterPoint; } set { m_pResultMarkCenterPoint = value; } }
        public SizeF ref_SResultMarkSize { get { return m_SResultMarkSize; } set { m_SResultMarkSize = value; } }
        public float ref_fResultMarkAngle { get { return m_fResultMarkAngle; } set { m_fResultMarkAngle = value; } }

        public List<List<float>> ref_arrMatcherScoreRecord { get { return m_arrMatcherScoreRecord; } set { m_arrMatcherScoreRecord = value; } }
        #endregion

        public string m_strTrack = "";
        public string m_strTrack2 = "";

        public SealBlog(int intCameraResolutionWidth, int intCameraResolutionHeight)
        {
            m_objSealLineImage = new ImageDrawing(intCameraResolutionWidth, intCameraResolutionHeight);

            CreateBlobs();

            for (int i = 0; i < 3; i++)
            {
                m_fLineWidthAverage[i] = new float();
                m_fTemplateWidth[i] = new float();
            }

            for (int i = 0; i < 4; i++)
            {
                m_arrPocketMatcher.Add(new EMatcher());
                m_arrPositionMatcher.Add(new EMatcher());
            }

            for (int i = 0; i < 2; i++)
            {
                m_intTemplateAreaAVG[i] = new int();
                m_arrSealAreaTolerance[i] = new float();
            }
        }



        public bool DoInspection(List<List<ROI>> arrROIs, List<List<LGauge>> arrGauges, CirGauge objCircleGauge, float fCalibY, List<ImageDrawing> arrImages, ImageDrawing objWhiteImage)
        {
            // 2020 10 14 - Do Distance Inspection after Seal width inspection. This is because Distance Inspection will use new formula which is rely on seal line position.
            //if ((m_intFailOptionMask & 0x08) > 0)
            //{
            //    if (!CheckDistanceBtwSealAndBorder(arrROIs[3][0], fCalibY))
            //        return false;
            //}
            //float fDifference = Math.Abs(fGapDistance - m_fTemplateWidth[2]);
            //m_fLineWidthAverage[2] = fGapDistance;
            //if (fDifference > m_intShiftPositionTolerance)
            //{
            //    m_strErrorMessage += "*Shift Position : Set = " + (m_fLineWidthAverage[2] / fCalibY).ToString("f5") + " mm,   Result = " + (fGapDistance / fCalibY).ToString("f5") + " mm";
            //    m_intSealFailMask |= 0x01;
            //    return false;
            //}

            //EasyImage.Copy(arrROIs[0][0].ref_ROI.TopParent, m_objSealLineImage.ref_objMainImage);
            //List<List<ROI>> arrSealROI = new List<List<ROI>>();
            //for (int i = 0; i < arrROIs.Count; i++)
            //{
            //    ROI objROI = new ROI();
            //    objROI.AttachImage(m_objSealLineImage);
            //    objROI.LoadROISetting(arrROIs[i][0].ref_ROITotalX, arrROIs[i][0].ref_ROITotalY, arrROIs[i][0].ref_ROIWidth, arrROIs[i][0].ref_ROIHeight);
            //    arrSealROI.Add(new List<ROI>());
            //    arrSealROI[i].Add(objROI);
            //}

            //if (((m_intFailOptionMask & 0x01) > 0))
            {
                if (!BuildGauge(arrROIs, arrGauges, fCalibY, false))
                {
                    return false;
                }
            }

            if (((m_intFailOptionMask & 0x02) > 0) || ((m_intFailOptionMask & 0x20) > 0))
            {
                //if (!InspectFarAndNearSealArea(arrROIs, fCalibY))
                if (!InspectFarAndNearSealArea_BubbleOrBrokenArea(arrROIs, fCalibY))
                {
                    return false;
                }
            }

            // 2020 10 14 - Do Distance Inspection after Seal width inspection. This is because Distance Inspection will use new formula which is rely on seal line position.
            // 2020 11 27 - Need to check distance if sprocket hole is checking. Sprocket Hole value rely on distance edge value.
            //if ((m_intFailOptionMask & 0x08) > 0)
            if (((m_intFailOptionMask & 0x08) > 0) || (((m_intFailOptionMask & 0x100) > 0) && !m_blnWantSkipSprocketHole))
            {
                if (!CheckDistanceBtwSealAndBorder(arrROIs[3][0], fCalibY))
                    return false;
            }

            // 2020 11 23 - Do Sprocket Hole Distance Inspection after Distance inpection. Sprocket Hole Distance Calculation depends on Seal Border Position calculated in distance inspection
            if (((m_intFailOptionMask & 0x100) > 0) && !m_blnWantSkipSprocketHole)
            {
                if (!CheckSprocketHoleDistance(arrROIs[6][0], objCircleGauge, fCalibY, arrImages[Math.Min(GetGrabImageIndex(6), arrImages.Count - 1)], objWhiteImage))
                    return false;
            }
            
            if ((((m_intFailOptionMask & 0x200) > 0) || ((m_intFailOptionMask & 0x400) > 0)) && !m_blnWantSkipSprocketHoleDiameterAndDefect)
            {
                if (!CheckSprocketHoleDiameterAndDefect(arrROIs[6][0], objCircleGauge, fCalibY, arrImages[Math.Min(GetGrabImageIndex(6), arrImages.Count - 1)], objWhiteImage))
                    return false;
            }

            if ((((m_intFailOptionMask & 0x800) > 0) || ((m_intFailOptionMask & 0x1000) > 0)) && !m_blnWantSkipSprocketHoleBrokenAndRoundness)
            {
                if (!CheckSprocketHoleBrokenAndRoundness(arrROIs[6][0], objCircleGauge, fCalibY, arrImages[Math.Min(GetGrabImageIndex(6), arrImages.Count - 1)], objWhiteImage))
                    return false;
            }

            if ((m_intFailOptionMask & 0x10) > 0)
            {
                for (int i = 0; i < arrROIs[4].Count; i++)
                {
                    if (!CheckOverHeat(arrROIs[4][i], fCalibY, i))
                    {

                        return false;
                    }

                    if (!CheckScratches(arrROIs[4][i], fCalibY, i))
                    {
                        return false;
                    }
                }
            }

            if (((m_intFailOptionMask & 0x2000) > 0) && m_blnWantCheckSealEdgeStraightness)
            {
                if (arrROIs.Count < 8 || arrROIs[7].Count == 0)
                {
                    m_intSealFailMask |= 0x20000;

                    m_strErrorMessage = "*ROI not ready. Please learn Seal Edge Straightness.";
                    return false;
                }

                if (!CheckSealEdgeStraightness(arrROIs[7][0], fCalibY))
                {

                    return false;
                }
            }

            return true;
        }

        public bool CheckOverHeat(ROI objOverHeatROI, float fCalibY, int intROIIndex)
        {
            if (m_arrOverHeatBlobs[intROIIndex] == null)
                m_arrOverHeatBlobs[intROIIndex] = new EBlobs();
            else
                m_arrOverHeatBlobs[intROIIndex].CleanAllBlobs();

            objOverHeatROI.ref_ROIPositionX = objOverHeatROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
            objOverHeatROI.ref_ROIPositionY = objOverHeatROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);

            int intMaxAreaLimit = objOverHeatROI.ref_ROIWidth * objOverHeatROI.ref_ROIHeight + 1;
            if (m_arrOverHeatBlobs[intROIIndex].ref_intAbsoluteHighThreshold == 255)
            {
                m_arrOverHeatBlobs[intROIIndex].BuildObjects_Filter_GetElement_DoubleThreshold(objOverHeatROI, true, false, false, false,
                                                                                  m_arrOverHeatBlobs[intROIIndex].ref_intAbsoluteLowThreshold, m_arrOverHeatBlobs[intROIIndex].ref_intAbsoluteHighThreshold,
                                                                                  1, intMaxAreaLimit, false, 0x01); //m_objOverHeatBlobs.ref_intMinAreaLimit, m_objOverHeatBlobs.ref_intMaxAreaLimit, false, 0x01);
            }
            else
            {
                m_arrOverHeatBlobs[intROIIndex].BuildObjects_Filter_GetElement_DoubleThreshold(objOverHeatROI, true, false, true, false,
                                                                                  m_arrOverHeatBlobs[intROIIndex].ref_intAbsoluteLowThreshold, m_arrOverHeatBlobs[intROIIndex].ref_intAbsoluteHighThreshold,
                                                                                  1, intMaxAreaLimit, false, 0x01); //m_objOverHeatBlobs.ref_intMinAreaLimit, m_objOverHeatBlobs.ref_intMaxAreaLimit, false, 0x01);
            }

            // 2020 08 13 - CCENG: Total all blobs area instead of using 1 highest area blob only
            int intTotalArea = 0;
            for (int i = 0; i < m_arrOverHeatBlobs[intROIIndex].ref_intNumSelectedObject; i++)
            {
                intTotalArea += m_arrOverHeatBlobs[intROIIndex].ref_arrArea[i];
            }

            float fValue = (float)intTotalArea / (fCalibY * fCalibY);
            if (fValue > m_arrOverHeatAreaMinTolerance[intROIIndex])
            {
                m_intSealFailMask |= 0x40;
                m_arrFailOverHeat[intROIIndex] = true;
                m_strErrorMessage = "*Fail Over Heat " + (intROIIndex + 1).ToString() + ". Set Min Area = " + m_arrOverHeatAreaMinTolerance[intROIIndex].ToString("F4") + ", Result=" + fValue.ToString("F4");
                m_arrFailOverheatArea[intROIIndex] = fValue;
                return false;
            }

            m_arrFailOverheatArea[intROIIndex] = fValue;

            return true;
        }

        public bool CheckScratches(ROI objScratchesROI, float fCalibY, int intROIIndex)
        {
            if (m_arrScratchesBlobs[intROIIndex] == null)
                m_arrScratchesBlobs[intROIIndex] = new EBlobs();
            else
                m_arrScratchesBlobs[intROIIndex].CleanAllBlobs();

            objScratchesROI.ref_ROIPositionX = objScratchesROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
            objScratchesROI.ref_ROIPositionY = objScratchesROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);

            int intMaxAreaLimit = objScratchesROI.ref_ROIWidth * objScratchesROI.ref_ROIHeight + 1;
            m_arrScratchesBlobs[intROIIndex].BuildObjects_Filter_GetElement(objScratchesROI, true, m_intConnexity == 4, 0,
                                                                              m_arrScratchesBlobs[intROIIndex].ref_intAbsoluteThreshold,
                                                                              1, intMaxAreaLimit, false, 0x01);

            int intTotalArea = 0;
            for (int i = 0; i < m_arrScratchesBlobs[intROIIndex].ref_intNumSelectedObject; i++)
            {
                intTotalArea += m_arrScratchesBlobs[intROIIndex].ref_arrArea[i];
            }

            float fValue = (float)intTotalArea / (fCalibY * fCalibY);
            if (fValue > m_arrScratchesAreaMinTolerance[intROIIndex])
            {
                m_intSealFailMask |= 0x800;
                m_arrFailScratches[intROIIndex] = true;
                m_strErrorMessage = "*Fail Tape Scratches " + (intROIIndex + 1).ToString() + ". Set Min Area = " + m_arrScratchesAreaMinTolerance[intROIIndex].ToString("F4") + ", Result=" + fValue.ToString("F4");
                m_arrFailScratchesArea[intROIIndex] = fValue;
                return false;
            }
            else
                m_arrFailScratchesArea[intROIIndex] = fValue;

            return true;
        }
        public bool CheckSealEdgeStraightness(ROI objScratchesROI, float fCalibY)
        {
            if (m_objSealEdgeStraightnessBlobs == null)
                m_objSealEdgeStraightnessBlobs = new EBlobs();
            else
                m_objSealEdgeStraightnessBlobs.CleanAllBlobs();

            objScratchesROI.ref_ROIPositionX = objScratchesROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
            objScratchesROI.ref_ROIPositionY = objScratchesROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);

            int intMaxAreaLimit = objScratchesROI.ref_ROIWidth * objScratchesROI.ref_ROIHeight + 1;
            m_objSealEdgeStraightnessBlobs.BuildObjects_Filter_GetElement(objScratchesROI, false, m_intConnexity == 4, 0,
                                                                              m_objSealEdgeStraightnessBlobs.ref_intAbsoluteThreshold,
                                                                              1, intMaxAreaLimit, false, 0x01);

            int intTotalArea = 0;
            for (int i = 0; i < m_objSealEdgeStraightnessBlobs.ref_intNumSelectedObject; i++)
            {
                intTotalArea += m_objSealEdgeStraightnessBlobs.ref_arrArea[i];
            }

            float fValue = (float)intTotalArea / (fCalibY * fCalibY);
            if (intTotalArea > m_fSealEdgeStraightnessMaxTolerance)
            {
                m_intSealFailMask |= 0x20000;

                m_strErrorMessage = "*Fail Seal Edge Straightness. Set Max Area = " + (m_fSealEdgeStraightnessMaxTolerance / (fCalibY * fCalibY)).ToString("F4") + ", Result=" + fValue.ToString("F4");
                m_FailSealEdgeStraightnessArea = fValue;
                return false;
            }
            else
                m_FailSealEdgeStraightnessArea = fValue;

            return true;
        }
        /// <summary>
        /// Check whether ROI that user drag is in the center of Seal 'Far' and Seal 'Near'
        /// </summary>
        /// <param name="objROI">ROI</param>
        /// <param name="fCalibY">calibration value</param>
        /// <returns>0 = valid ROI(in center), otherwise = invalid ROI(not in center) and auto move ROI Y to center</returns>
        public int CheckSealBlogInROI_Reject(ROI objROI, float fCalibY)
        {
            Blobs objBlackBlob = new Blobs();
            objBlackBlob.SetConnexity(m_intConnexity);
            objBlackBlob.SetClassSelection(1);

            objBlackBlob.ref_intThreshold = ROI.GetAutoThresholdValue(objROI, 3);
            objBlackBlob.ref_intFeature = m_intFeature; // area and object center
            objBlackBlob.SetObjectAreaRange(0, 999999);

            int intNumSelectedObject = objBlackBlob.BuildObjects(objROI);
            if (intNumSelectedObject > 0)
            {
                int intObjectNumWithMaxArea = objBlackBlob.GetSelectedObjectNumWithMaxArea();
                PointF pCenter = objBlackBlob.GetSelectedObjectLimitCenter(intObjectNumWithMaxArea);

                float fDistance = objROI.ref_ROICenterY - (objROI.ref_ROIPositionY + pCenter.Y);
                return Convert.ToInt32(fDistance);
            }

            objBlackBlob.Dispose();

            return 0;
        }

        public int CheckSealBlogInROI(ROI objROI, float fCalibY)
        {
            if (m_objLearnBlobs == null)
                m_objLearnBlobs = new EBlobs();
            else
                m_objLearnBlobs.CleanAllBlobs();

            m_objLearnBlobs.BuildObjects_Filter_GetElement(objROI, true, m_intConnexity == 4, 0, ROI.GetAutoThresholdValue(objROI, 3), 0, 999999, false, 0x0F);
            if (m_objLearnBlobs.ref_intNumSelectedObject > 1)
            {
                PointF pCenter = new PointF(m_objLearnBlobs.ref_arrLimitCenterX[0], m_objLearnBlobs.ref_arrLimitCenterY[0]);

                float fDistance = objROI.ref_ROICenterY - (objROI.ref_ROIPositionY + pCenter.Y);
                return Convert.ToInt32(fDistance);
            }

            return 0;
        }

        /// <summary>
        /// Get distance ROI start Y
        /// </summary>
        /// <param name="objROI">ROI</param>
        /// <returns>distance ROI start Y</returns>
        public float GetDistanceBtwSealAndBorder(ROI objROI, ref float fSealTopPositionY)
        {
            if (m_objDistanceBlobs == null)
                m_objDistanceBlobs = new EBlobs();
            else
                m_objDistanceBlobs.CleanAllBlobs();

            m_objDistanceBlobs.BuildObjects_Filter_GetElement(objROI, true, true, 0, m_objDistanceBlobs.ref_intAbsoluteThreshold, objROI.ref_ROIWidth, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0x0F);
            if (m_objDistanceBlobs.ref_intNumSelectedObject < 2)
                return 0;

            float fMinY = 0;
            int intMinPostYNo = 0;
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                    fMinY = m_objDistanceBlobs.ref_arrLimitCenterY[i];

                if (m_objDistanceBlobs.ref_arrLimitCenterY[i] < fMinY)
                {
                    fMinY = m_objDistanceBlobs.ref_arrLimitCenterY[i];
                    intMinPostYNo = i;
                }
            }

            PointF pCenter = new PointF(m_objDistanceBlobs.ref_arrLimitCenterX[intMinPostYNo], m_objDistanceBlobs.ref_arrLimitCenterY[intMinPostYNo]);
            float fHeight = m_objDistanceBlobs.ref_arrHeight[intMinPostYNo];
            float fBorderPositionY = pCenter.Y + (fHeight / 2);

            int intMaxPosYNo = 0;
            if (intMinPostYNo == 0)
                intMaxPosYNo = 1;

            pCenter = new PointF(m_objDistanceBlobs.ref_arrLimitCenterX[intMaxPosYNo], m_objDistanceBlobs.ref_arrLimitCenterY[intMaxPosYNo]);
            fHeight = m_objDistanceBlobs.ref_arrHeight[intMaxPosYNo];
            fSealTopPositionY = pCenter.Y - (fHeight / 2);

            //m_objDistanceBlobs.SetConnexity(m_intConnexity);
            //m_objDistanceBlobs.SetClassSelection(1);
            //m_objDistanceBlobs.ref_intFeature = m_intFeature;
            //m_objDistanceBlobs.SetObjectAreaRange(500, 50000);

            //int intNumSelectedObject = m_objDistanceBlobs.BuildObjects(objROI);
            //if (intNumSelectedObject < 2)
            //    return 0;
            //m_objDistanceBlobs.SortObjects(1, false);  // Get 2 biggest area object in front to be used

            //int intMinPostYNo = m_objDistanceBlobs.GetSelectedObjectNumWithMinPostY(2);
            //PointF pCenter = m_objDistanceBlobs.GetSelectedObjectLimitCenter(intMinPostYNo);
            //float fHeight = m_objDistanceBlobs.GetSelectedObjectLimitHeight(intMinPostYNo);
            //float fBorderPositionY = pCenter.Y + (fHeight / 2);

            //int intMaxPosYNo = 0;
            //if (intMinPostYNo == 0)
            //    intMaxPosYNo = 1;

            //pCenter = m_objDistanceBlobs.GetSelectedObjectLimitCenter(intMaxPosYNo);
            //fHeight = m_objDistanceBlobs.GetSelectedObjectLimitHeight(intMaxPosYNo);
            //float fSealTopPositionY = pCenter.Y - (fHeight / 2);

            return (fSealTopPositionY - fBorderPositionY);
        }

        public void SaveTemplateCircleImage(CirGauge objCircleGauge, ImageDrawing objWhiteImage, string strPath)
        {
            ImageDrawing objImg_Temp = new ImageDrawing(true, objWhiteImage.ref_intImageWidth, objWhiteImage.ref_intImageHeight);
            objWhiteImage.CopyTo(ref objImg_Temp);

            IntPtr ptr = Easy.OpenImageGraphicContext(objImg_Temp.ref_objMainImage);
            Graphics g = Graphics.FromHdc(ptr);

            g.FillEllipse(new SolidBrush(Color.Black), (objImg_Temp.ref_intImageWidth / 2) - (objCircleGauge.ref_fTemplateObjectDiameter / 2),
                                                       (objImg_Temp.ref_intImageHeight / 2) - (objCircleGauge.ref_fTemplateObjectDiameter / 2),
                                                       objCircleGauge.ref_fTemplateObjectDiameter, 
                                                       objCircleGauge.ref_fTemplateObjectDiameter);

            Easy.CloseImageGraphicContext(objImg_Temp.ref_objMainImage, ptr);

            objImg_Temp.SaveImage(strPath);
            objImg_Temp.Dispose();

            if (File.Exists(strPath))
                m_objTemplateCircleImage.LoadImage(strPath);
        }
        public void LoadTemplateCircleImage(string strPath)
        {
            if (File.Exists(strPath))
                m_objTemplateCircleImage.LoadImage(strPath);
        }

        public bool CheckSprocketHoleDistance(ROI objROI, CirGauge objCircleGauge, float fCalibPixelPerMM, ImageDrawing objImage, ImageDrawing objWhiteImage)
        {
            // Shift ROI position
            if (m_intTapePocketPitch == 2)
            {
                objROI.ref_ROIPositionX = objROI.ref_ROIOriPositionX + Convert.ToInt32(m_fCircleGaugeShiftX);
                objROI.ref_ROIPositionY = objROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);
            }
            else
            {
                objROI.ref_ROIPositionX = objROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
                objROI.ref_ROIPositionY = objROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);
            }
            
            objCircleGauge.SetGaugePlacement(objROI, (int)objCircleGauge.ref_GaugeTolerance, m_intSpocketHolePosition == 0);

            objCircleGauge.Measure(objImage);
            
            if (objCircleGauge.ref_GaugeScore < objCircleGauge.ref_intMinScore)
            {
                m_strErrorMessage += "*Fail Sprocket Hole Distance. Gauge Measurement : Set Gauge Score = " + objCircleGauge.ref_intMinScore.ToString() + " %,   Result = " + objCircleGauge.ref_GaugeScore.ToString("f2") + " %"; ;
                m_intSealFailMask |= 0x200;
                return false;
            }

            if ((objCircleGauge.ref_ObjectCenterX < (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterX > (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY < (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY > (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
            {
                m_blnCircleGaugeOutOfImageRange = true;
                m_strErrorMessage += "*Fail Sprocket Hole Distance. Gauge Measurement Out of Image Range";
                m_intSealFailMask |= 0x200;
                return false;
            }

            //if (((m_intFailOptionMask & 0x08) > 0) && ((m_intSealFailMask & 0x02) == 0))//If got check Distance and Distance result also pass, just got value to calculate Sprocket Hole Distance
            if ((m_intSealFailMask & 0x02) == 0)
            {
                float fGaugeEdgePointY;
                if (m_intSpocketHolePosition == 0)
                    fGaugeEdgePointY = objCircleGauge.ref_ObjectCenterY + objCircleGauge.ref_fDiameter / 2;
                else
                    fGaugeEdgePointY = objCircleGauge.ref_ObjectCenterY - objCircleGauge.ref_fDiameter / 2;

                float fGapDistance;
                if (m_intSpocketHolePosition == 0)
                    fGapDistance = m_fSealBorderPositionY - fGaugeEdgePointY;
                else
                    fGapDistance = fGaugeEdgePointY - m_fSealBorderPositionY;

                m_fLineWidthAverage[7] = fGapDistance;
                if (fGapDistance < (m_fSprocketHoleDistanceMinTolerance))
                {
                    m_FailSprocketHoleDistance = (fGapDistance / fCalibPixelPerMM);
                    m_strErrorMessage += "*Fail Sprocket Hole Distance. Minimum Distance Set = " + ((m_fSprocketHoleDistanceMinTolerance) / fCalibPixelPerMM).ToString("f5") + " mm,   Result = " + (fGapDistance / fCalibPixelPerMM).ToString("f3") + " mm";
                    m_intSealFailMask |= 0x200;
                    return false;
                }
                else if (fGapDistance > (m_fSprocketHoleDistanceMaxTolerance))
                {
                    m_FailSprocketHoleDistance = (fGapDistance / fCalibPixelPerMM);
                    m_strErrorMessage += "*Fail Sprocket Hole Distance. Maximum Distance Set = " + ((m_fSprocketHoleDistanceMaxTolerance) / fCalibPixelPerMM).ToString("f5") + " mm,   Result = " + (fGapDistance / fCalibPixelPerMM).ToString("f3") + " mm";
                    m_intSealFailMask |= 0x200;
                    return false;
                }
            }
            
            return true;
        }
        public bool CheckSprocketHoleDiameterAndDefect(ROI objROI, CirGauge objCircleGauge, float fCalibPixelPerMM, ImageDrawing objImage, ImageDrawing objWhiteImage)
        {
            bool blnDebug = false;
            // Shift ROI position
            if (((m_intFailOptionMask & 0x100) == 0) || m_blnWantSkipSprocketHole)
            {
                if (m_intTapePocketPitch == 2)
                {
                    objROI.ref_ROIPositionX = objROI.ref_ROIOriPositionX + Convert.ToInt32(m_fCircleGaugeShiftX);
                    objROI.ref_ROIPositionY = objROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);
                }
                else
                {
                    objROI.ref_ROIPositionX = objROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
                    objROI.ref_ROIPositionY = objROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);
                }

                objCircleGauge.SetGaugePlacement(objROI, (int)objCircleGauge.ref_GaugeTolerance, m_intSpocketHolePosition == 0);

                objCircleGauge.Measure(objImage);

                if (objCircleGauge.ref_GaugeScore < objCircleGauge.ref_intMinScore)
                {
                    if ((m_intFailOptionMask & 0x200) > 0)
                    {
                        m_strErrorMessage += "*Fail Sprocket Hole Diameter. Gauge Measurement : Set Gauge Score = " + objCircleGauge.ref_intMinScore.ToString() + " %,   Result = " + objCircleGauge.ref_GaugeScore.ToString("f2") + " %"; ;
                        m_intSealFailMask |= 0x2000;
                    }
                    else
                    {
                        m_strErrorMessage += "*Fail Sprocket Hole Defect. Gauge Measurement : Set Gauge Score = " + objCircleGauge.ref_intMinScore.ToString() + " %,   Result = " + objCircleGauge.ref_GaugeScore.ToString("f2") + " %"; ;
                        m_intSealFailMask |= 0x4000;
                    }
                    return false;
                }

                if ((objCircleGauge.ref_ObjectCenterX < (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterX > (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY < (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY > (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
                {
                    m_blnCircleGaugeOutOfImageRange = true;
                    if ((m_intFailOptionMask & 0x200) > 0)
                    {
                        m_strErrorMessage += "*Fail Sprocket Hole Diameter. Gauge Measurement Out of Image Range";
                        m_intSealFailMask |= 0x2000;
                    }
                    else
                    {
                        m_strErrorMessage += "*Fail Sprocket Hole Defect. Gauge Measurement Out of Image Range";
                        m_intSealFailMask |= 0x4000;
                    }
                    return false;
                }

            }

            //Check Sprocket Hole diameter
            if ((m_intFailOptionMask & 0x200) > 0)
            {
                m_fLineWidthAverage[14] = objCircleGauge.ref_fDiameter;
                if (objCircleGauge.ref_fDiameter < (m_fSprocketHoleDiameterMinTolerance))
                {
                    m_FailSprocketHoleDiameter = (objCircleGauge.ref_fDiameter / fCalibPixelPerMM);
                    m_strErrorMessage += "*Fail Sprocket Hole Diameter. Minimum Diameter Set = " + ((m_fSprocketHoleDiameterMinTolerance) / fCalibPixelPerMM).ToString("f5") + " mm,   Result = " + (objCircleGauge.ref_fDiameter / fCalibPixelPerMM).ToString("f3") + " mm";
                    m_intSealFailMask |= 0x2000;
                    return false;
                }
                else if (objCircleGauge.ref_fDiameter > (m_fSprocketHoleDiameterMaxTolerance))
                {
                    m_FailSprocketHoleDiameter = (objCircleGauge.ref_fDiameter / fCalibPixelPerMM);
                    m_strErrorMessage += "*Fail Sprocket Hole Diameter. Maximum Diameter Set = " + ((m_fSprocketHoleDiameterMaxTolerance) / fCalibPixelPerMM).ToString("f5") + " mm,   Result = " + (objCircleGauge.ref_fDiameter / fCalibPixelPerMM).ToString("f3") + " mm";
                    m_intSealFailMask |= 0x2000;
                    return false;
                }
            }

            //Check Sprocket Hole defect
            if ((m_intFailOptionMask & 0x400) > 0)
            {
                ImageDrawing objImg_Temp = new ImageDrawing(true, m_objTemplateCircleImage.ref_intImageWidth, m_objTemplateCircleImage.ref_intImageHeight);
               
                float fScale = Math.Max(objCircleGauge.ref_fDiameter - m_intSprocketHoleInspectionAreaInwardTolerance, 5) / objCircleGauge.ref_fTemplateObjectDiameter;

                EasyImage.ScaleRotate(m_objTemplateCircleImage.ref_objMainImage, m_objTemplateCircleImage.ref_intImageWidth / 2, m_objTemplateCircleImage.ref_intImageHeight / 2,
                                      objImg_Temp.ref_intImageWidth / 2, objImg_Temp.ref_intImageHeight / 2,
                                      fScale, fScale, 0, objImg_Temp.ref_objMainImage, 4);

                if (blnDebug)
                    objImg_Temp.SaveImage("D:\\TS\\objImg_Temp.bmp");

                ROI objCircleROI1 = new ROI();
                objCircleROI1.AttachImage(objImg_Temp);
                objCircleROI1.LoadROISetting((int)Math.Round((objImg_Temp.ref_intImageWidth / 2) - (objCircleGauge.ref_fDiameter / 2)),
                                                    (int)Math.Round((objImg_Temp.ref_intImageHeight / 2) - (objCircleGauge.ref_fDiameter / 2)),
                                                    (int)Math.Round(objCircleGauge.ref_fDiameter),
                                                    (int)Math.Round(objCircleGauge.ref_fDiameter));
                
                ROI objCircleROI2 = new ROI();
                objCircleROI2.AttachImage(objImage);
                objCircleROI2.LoadROISetting((int)Math.Round(objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2)),
                                            (int)Math.Round(objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2)),
                                            (int)Math.Round(objCircleGauge.ref_fDiameter),
                                            (int)Math.Round(objCircleGauge.ref_fDiameter));

                if (blnDebug)
                {
                    objCircleROI1.SaveImage("D:\\TS\\objCircleROI1.bmp");
                    objCircleROI2.SaveImage("D:\\TS\\objCircleROI2.bmp");
                }

                ROI.LogicOperationAddROI(objCircleROI1, objCircleROI2);

                if (blnDebug)
                {
                    objROI.SaveImage("D:\\TS\\objROI.bmp");
                    objCircleROI1.SaveImage("D:\\TS\\objCircleROI1_AfterSubtract.bmp");
                }

                if (m_objSprocketHoleDefectBlobs == null)
                    m_objSprocketHoleDefectBlobs = new EBlobs();
                else
                    m_objSprocketHoleDefectBlobs.CleanAllBlobs();

                m_objSprocketHoleDefectBlobs.BuildObjects_Filter_GetElement(objCircleROI1, true, true, 0, m_objSprocketHoleDefectBlobs.ref_intAbsoluteThreshold, 1, objCircleROI1.ref_ROIWidth * objCircleROI1.ref_ROIHeight + 1, false, 0x0F);

                int intTotalArea = 0;
                if (m_objSprocketHoleDefectBlobs.ref_intNumSelectedObject > 0)
                {
                    for (int i = 0; i < m_objSprocketHoleDefectBlobs.ref_intNumSelectedObject; i++)
                    {
                        intTotalArea += m_objSprocketHoleDefectBlobs.ref_arrArea[i];
                    }
                }
              
                m_fLineWidthAverage[15] = intTotalArea;
                if (intTotalArea > (m_fSprocketHoleDefectMaxTolerance))
                {
                    m_FailSprocketHoleDefectArea = (intTotalArea / (fCalibPixelPerMM * fCalibPixelPerMM));
                    m_strErrorMessage += "*Fail Sprocket Hole Defect Area. Maximum Defect Area Set = " + ((m_fSprocketHoleDefectMaxTolerance) / (fCalibPixelPerMM * fCalibPixelPerMM)).ToString("f5") + " mm2,   Result = " + (intTotalArea / (fCalibPixelPerMM * fCalibPixelPerMM)).ToString("f3") + " mm2";
                    m_intSealFailMask |= 0x4000;
                    return false;
                }
                
                objCircleROI2.Dispose();
                objCircleROI1.Dispose();
                objImg_Temp.Dispose();

            }
            return true;
        }
        public bool CheckSprocketHoleBrokenAndRoundness(ROI objROI, CirGauge objCircleGauge, float fCalibPixelPerMM, ImageDrawing objImage, ImageDrawing objWhiteImage)
        {
            bool blnDebug = false;
            // Shift ROI position
            if ((((m_intFailOptionMask & 0x100) == 0) || m_blnWantSkipSprocketHole) && ((((m_intFailOptionMask & 0x200) == 0) && ((m_intFailOptionMask & 0x400) == 0)) || m_blnWantSkipSprocketHoleDiameterAndDefect))
            {
                if (m_intTapePocketPitch == 2)
                {
                    objROI.ref_ROIPositionX = objROI.ref_ROIOriPositionX + Convert.ToInt32(m_fCircleGaugeShiftX);
                    objROI.ref_ROIPositionY = objROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);
                }
                else
                {
                    objROI.ref_ROIPositionX = objROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
                    objROI.ref_ROIPositionY = objROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);
                }

                objCircleGauge.SetGaugePlacement(objROI, (int)objCircleGauge.ref_GaugeTolerance, m_intSpocketHolePosition == 0);

                objCircleGauge.Measure(objImage);

                if (objCircleGauge.ref_GaugeScore < objCircleGauge.ref_intMinScore)
                {
                    if ((m_intFailOptionMask & 0x800) > 0)
                    {
                        m_strErrorMessage += "*Fail Sprocket Hole Broken. Gauge Measurement : Set Gauge Score = " + objCircleGauge.ref_intMinScore.ToString() + " %,   Result = " + objCircleGauge.ref_GaugeScore.ToString("f2") + " %"; ;
                        m_intSealFailMask |= 0x8000;
                    }
                    else
                    {
                        m_strErrorMessage += "*Fail Sprocket Hole Roundness. Gauge Measurement : Set Gauge Score = " + objCircleGauge.ref_intMinScore.ToString() + " %,   Result = " + objCircleGauge.ref_GaugeScore.ToString("f2") + " %"; ;
                        m_intSealFailMask |= 0x10000;
                    }
                    return false;
                }

                if ((objCircleGauge.ref_ObjectCenterX < (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterX > (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY < (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY > (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
                {
                    m_blnCircleGaugeOutOfImageRange = true;
                    if ((m_intFailOptionMask & 0x800) > 0)
                    {
                        m_strErrorMessage += "*Fail Sprocket Hole Broken. Gauge Measurement Out of Image Range";
                        m_intSealFailMask |= 0x8000;
                    }
                    else
                    {
                        m_strErrorMessage += "*Fail Sprocket Hole Roundness. Gauge Measurement Out of Image Range";
                        m_intSealFailMask |= 0x4000;
                    }
                    return false;
                }

            }

            //Check Sprocket Hole Broken
            if ((m_intFailOptionMask & 0x800) > 0)
            {
                ImageDrawing objImg_Temp = new ImageDrawing(true, m_objTemplateCircleImage.ref_intImageWidth, m_objTemplateCircleImage.ref_intImageHeight);

                float fScale = (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / objCircleGauge.ref_fTemplateObjectDiameter;

                EasyImage.ScaleRotate(m_objTemplateCircleImage.ref_objMainImage, m_objTemplateCircleImage.ref_intImageWidth / 2, m_objTemplateCircleImage.ref_intImageHeight / 2,
                                      objCircleGauge.ref_ObjectCenterX, objCircleGauge.ref_ObjectCenterY,
                                      fScale, fScale, 0, objImg_Temp.ref_objMainImage, 4);

                if (blnDebug)
                    objImg_Temp.SaveImage("D:\\TS\\objImg_Temp.bmp");

                ROI objCircleROI1 = new ROI();
                objCircleROI1.AttachImage(objImg_Temp);
                objCircleROI1.LoadROISetting((int)Math.Round((objCircleGauge.ref_ObjectCenterX) - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                    (int)Math.Round((objCircleGauge.ref_ObjectCenterY) - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                    (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                                    (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer)));

                ROI objCircleROI2 = new ROI();
                objCircleROI2.AttachImage(objImage);
                objCircleROI2.LoadROISetting((int)Math.Round(objCircleGauge.ref_ObjectCenterX - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                            (int)Math.Round(objCircleGauge.ref_ObjectCenterY - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                            (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                            (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer)));

                if (blnDebug)
                {
                    objCircleROI1.SaveImage("D:\\TS\\objCircleROI1.bmp");
                    objCircleROI2.SaveImage("D:\\TS\\objCircleROI2.bmp");
                }

                ROI.SubtractROI2(objCircleROI2, objCircleROI1);

                if (blnDebug)
                {
                    objROI.SaveImage("D:\\TS\\objROI.bmp");
                    objCircleROI1.SaveImage("D:\\TS\\objCircleROI1_AfterSubtract.bmp");
                }
                ImageDrawing objImg_Temp2 = new ImageDrawing(true, m_objTemplateCircleImage.ref_intImageWidth, m_objTemplateCircleImage.ref_intImageHeight);

                fScale = (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) / objCircleGauge.ref_fTemplateObjectDiameter;

                EasyImage.ScaleRotate(m_objTemplateCircleImage.ref_objMainImage, m_objTemplateCircleImage.ref_intImageWidth / 2, m_objTemplateCircleImage.ref_intImageHeight / 2,
                                      objCircleGauge.ref_ObjectCenterX, objCircleGauge.ref_ObjectCenterY,
                                      fScale, fScale, 0, objImg_Temp2.ref_objMainImage, 4);

                objCircleROI1.LoadROISetting((int)Math.Round((objCircleGauge.ref_ObjectCenterX) - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)),
                                                    (int)Math.Round((objCircleGauge.ref_ObjectCenterY) - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)),
                                                    (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner)),
                                                    (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner)));
                objCircleROI2.AttachImage(objImg_Temp2);
                objCircleROI2.LoadROISetting((int)Math.Round((objCircleGauge.ref_ObjectCenterX) - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)),
                                                    (int)Math.Round((objCircleGauge.ref_ObjectCenterY) - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)),
                                                    (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner)),
                                                    (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner)));

                if (blnDebug)
                {
                    objCircleROI1.SaveImage("D:\\TS\\objCircleROI12.bmp");
                    objCircleROI2.SaveImage("D:\\TS\\objCircleROI22.bmp");
                }

                ROI.LogicOperationBitwiseAndROI(objCircleROI1, objCircleROI2);

                objCircleROI1.LoadROISetting((int)Math.Round((objCircleGauge.ref_ObjectCenterX) - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                    (int)Math.Round((objCircleGauge.ref_ObjectCenterY) - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                    (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                                    (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer)));

                if (blnDebug)
                {
                    objImg_Temp2.SaveImage("D:\\TS\\objImg_Temp2.bmp");
                    objCircleROI1.SaveImage("D:\\TS\\objCircleROI1_AfterSubtract2.bmp");
                }

                if (m_objSprocketHoleBrokenBlobs == null)
                    m_objSprocketHoleBrokenBlobs = new EBlobs();
                else
                    m_objSprocketHoleBrokenBlobs.CleanAllBlobs();

                m_objSprocketHoleBrokenBlobs.BuildObjects_Filter_GetElement(objCircleROI1, false, true, 0, m_objSprocketHoleBrokenBlobs.ref_intAbsoluteThreshold, 1, objCircleROI1.ref_ROIWidth * objCircleROI1.ref_ROIHeight + 1, false, 0x0F);

                int intTotalArea = 0;
                if (m_objSprocketHoleBrokenBlobs.ref_intNumSelectedObject > 0)
                {
                    for (int i = 0; i < m_objSprocketHoleBrokenBlobs.ref_intNumSelectedObject; i++)
                    {
                        intTotalArea += m_objSprocketHoleBrokenBlobs.ref_arrArea[i];
                    }
                }

                m_fLineWidthAverage[16] = intTotalArea;
                if (intTotalArea > (m_fSprocketHoleBrokenMaxTolerance))
                {
                    m_FailSprocketHoleBrokenArea = (intTotalArea / (fCalibPixelPerMM * fCalibPixelPerMM));
                    m_strErrorMessage += "*Fail Sprocket Hole Broken Area. Maximum Broken Area Set = " + ((m_fSprocketHoleBrokenMaxTolerance) / (fCalibPixelPerMM * fCalibPixelPerMM)).ToString("f5") + " mm2,   Result = " + (intTotalArea / (fCalibPixelPerMM * fCalibPixelPerMM)).ToString("f3") + " mm2";
                    m_intSealFailMask |= 0x8000;
                    return false;
                }
                objImg_Temp2.Dispose();
                objCircleROI2.Dispose();
                objCircleROI1.Dispose();
                objImg_Temp.Dispose();

            }

            //Check Sprocket Hole Roundness
            if ((m_intFailOptionMask & 0x1000) > 0)
            {
                ImageDrawing objImg_Temp = new ImageDrawing(true, m_objTemplateCircleImage.ref_intImageWidth, m_objTemplateCircleImage.ref_intImageHeight);

                float fScale = (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / objCircleGauge.ref_fTemplateObjectDiameter;

                EasyImage.ScaleRotate(m_objTemplateCircleImage.ref_objMainImage, m_objTemplateCircleImage.ref_intImageWidth / 2, m_objTemplateCircleImage.ref_intImageHeight / 2,
                                      objCircleGauge.ref_ObjectCenterX, objCircleGauge.ref_ObjectCenterY,
                                      fScale, fScale, 0, objImg_Temp.ref_objMainImage, 4);

                if (blnDebug)
                    objImg_Temp.SaveImage("D:\\TS\\objImg_Temp.bmp");

                ROI objCircleROI1 = new ROI();
                objCircleROI1.AttachImage(objImg_Temp);
                objCircleROI1.LoadROISetting((int)Math.Round((objCircleGauge.ref_ObjectCenterX) - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                    (int)Math.Round((objCircleGauge.ref_ObjectCenterY) - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                                    (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                                    (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer)));

                ROI objCircleROI2 = new ROI();
                objCircleROI2.AttachImage(objImage);
                objCircleROI2.LoadROISetting((int)Math.Round(objCircleGauge.ref_ObjectCenterX - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                            (int)Math.Round(objCircleGauge.ref_ObjectCenterY - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)),
                                            (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer)),
                                            (int)Math.Round((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer)));

                if (blnDebug)
                {
                    objCircleROI1.SaveImage("D:\\TS\\objCircleROI1.bmp");
                    objCircleROI2.SaveImage("D:\\TS\\objCircleROI2.bmp");
                }

                ROI.SubtractROI2(objCircleROI2, objCircleROI1);

                if (blnDebug)
                {
                    objROI.SaveImage("D:\\TS\\objROI.bmp");
                    objCircleROI1.SaveImage("D:\\TS\\objCircleROI1_AfterSubtract.bmp");
                }
                
                if (m_objSprocketHoleRoundnessBlobs == null)
                    m_objSprocketHoleRoundnessBlobs = new EBlobs();
                else
                    m_objSprocketHoleRoundnessBlobs.CleanAllBlobs();

                m_objSprocketHoleRoundnessBlobs.BuildObjects_Filter_GetElement(objCircleROI1, false, true, 0, m_objSprocketHoleRoundnessBlobs.ref_intAbsoluteThreshold, 1, objCircleROI1.ref_ROIWidth * objCircleROI1.ref_ROIHeight + 1, false, 0x40F);

                if (m_objSprocketHoleRoundnessBlobs.ref_intNumSelectedObject > 0)
                {
                    m_fLineWidthAverage[17] = m_objSprocketHoleRoundnessBlobs.ref_arrEccentricity[0];
                    if (m_objSprocketHoleRoundnessBlobs.ref_arrEccentricity[0] > (m_fSprocketHoleRoundnessMaxTolerance))
                    {
                        m_FailSprocketHoleRoundness = m_objSprocketHoleRoundnessBlobs.ref_arrEccentricity[0];
                        m_strErrorMessage += "*Fail Sprocket Hole Roundness. Maximum Roundness Set = " + ((m_fSprocketHoleRoundnessMaxTolerance)).ToString("f3") + ",   Result = " + (m_objSprocketHoleRoundnessBlobs.ref_arrEccentricity[0]).ToString("f3");
                        m_intSealFailMask |= 0x10000;
                        return false;
                    }
                }
                else
                {
                    m_strErrorMessage += "*Fail Sprocket Hole Roundness. Cannot find blob.";
                    m_intSealFailMask |= 0x10000;
                    return false;
                }

                objCircleROI2.Dispose();
                objCircleROI1.Dispose();
                objImg_Temp.Dispose();

            }
            return true;
        }
        public bool CheckDistanceBtwSealAndBorder(ROI objROI, float fCalibPixelPerMM)
        {
            // Shift ROI position
            objROI.ref_ROIPositionX = objROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
            objROI.ref_ROIPositionY = objROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);

            if (m_objDistanceBlobs == null)
                m_objDistanceBlobs = new EBlobs();
            else
                m_objDistanceBlobs.CleanAllBlobs();

            m_objDistanceBlobs.BuildObjects_Filter_GetElement(objROI, true, true, 0, m_objDistanceBlobs.ref_intAbsoluteThreshold, 500, 100000, false, 0x0F);

            if (m_objDistanceBlobs.ref_intNumSelectedObject < 2)
            {
                m_objDistanceBlobs.BuildObjects_Filter_GetElement(objROI, true, true, 0, m_objDistanceBlobs.ref_intAbsoluteThreshold, objROI.ref_ROIWidth * 2, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0x0F);

                if (m_objDistanceBlobs.ref_intNumSelectedObject < 2)
                {
                    m_strErrorMessage += "*Fail Seal Distance. Cannot find blob.";
                    m_intSealFailMask |= 0x02;
                    return false;
                }
            }

            float fMinY = 0;
            float fMaxY = 0;
            int intPostYNo = 0;

            if (m_intSpocketHolePosition == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                        fMinY = m_objDistanceBlobs.ref_arrLimitCenterY[i];

                    if (m_objDistanceBlobs.ref_arrLimitCenterY[i] < fMinY)
                    {
                        fMinY = m_objDistanceBlobs.ref_arrLimitCenterY[i];
                        intPostYNo = i;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                        fMaxY = m_objDistanceBlobs.ref_arrLimitCenterY[i];

                    if (m_objDistanceBlobs.ref_arrLimitCenterY[i] > fMaxY)
                    {
                        fMaxY = m_objDistanceBlobs.ref_arrLimitCenterY[i];
                        intPostYNo = i;
                    }
                }
            }

            PointF pCenter = new PointF(m_objDistanceBlobs.ref_arrLimitCenterX[intPostYNo], m_objDistanceBlobs.ref_arrLimitCenterY[intPostYNo]);
            float fHeight = m_objDistanceBlobs.ref_arrHeight[intPostYNo];
            float fBorderPositionY;
            if (m_intSpocketHolePosition == 0)
                fBorderPositionY = pCenter.Y + (fHeight / 2);
            else
                fBorderPositionY = pCenter.Y - (fHeight / 2);
            m_fSealBorderPositionY = objROI.ref_ROI.TotalOrgY + fBorderPositionY;

            // 2020 10 16 - CCENG: Direct use Top Seal Line Center Y - Top Seal Line Width / 2 to get Seal Line Top Position Y.
            //            - With this change, user can set distance threshold according to Seal Border. (user sometime hard to set threshold when seal line and border not same gray color)

            //int intMaxPosYNo = 0;
            //if (intMinPostYNo == 0)
            //    intMaxPosYNo = 1;
            //
            //pCenter = new PointF(m_objDistanceBlobs.ref_arrLimitCenterX[intMaxPosYNo], m_objDistanceBlobs.ref_arrLimitCenterY[intMaxPosYNo]);
            //fHeight = m_objDistanceBlobs.ref_arrHeight[intMaxPosYNo];
            //float fSealTopPositionY = pCenter.Y - (fHeight / 2);
            float fSealEdgePositionY;

            if (m_intSpocketHolePosition == 0)
                fSealEdgePositionY = m_fLineWidthAverage[10] - m_fLineWidthAverage[9] / 2;
            else
                fSealEdgePositionY = m_fLineWidthAverage[13] + m_fLineWidthAverage[12] / 2;

            float fGapDistance;
            if (m_intSpocketHolePosition == 0)
                fGapDistance = fSealEdgePositionY - m_fSealBorderPositionY;
            else
                fGapDistance = m_fSealBorderPositionY - fSealEdgePositionY;

            m_strTrack += ", GapDistance=" + fGapDistance.ToString();

            m_fLineWidthAverage[2] = fGapDistance;

            if ((m_intFailOptionMask & 0x08) > 0)
            {
                if (fGapDistance < (m_fDistanceMinTolerance))
                {
                    m_FailDistance = (fGapDistance / fCalibPixelPerMM);
                    m_strErrorMessage += "*Fail Seal Distance. Minimum Distance : Set = " + ((m_fDistanceMinTolerance) / fCalibPixelPerMM).ToString("f5") + " mm,   Result = " + (fGapDistance / fCalibPixelPerMM).ToString("f3") + " mm";
                    m_intSealFailMask |= 0x02;
                    m_arrFailWidthStartPoint.Add(new List<PointF>());
                    m_arrFailWidthEndPoint.Add(new List<PointF>());
                    m_arrFailWidthStartPoint[0].Add(new PointF(objROI.ref_ROI.TotalOrgX + objROI.ref_ROIWidth / 2, fSealEdgePositionY));
                    m_arrFailWidthEndPoint[0].Add(new PointF(objROI.ref_ROI.TotalOrgX + objROI.ref_ROIWidth / 2, m_fSealBorderPositionY));

                    return false;
                }
                else if (fGapDistance > (m_fDistanceMaxTolerance))
                {
                    m_FailDistance = (fGapDistance / fCalibPixelPerMM);
                    m_strErrorMessage += "*Fail Seal Distance. Maximum Distance : Set = " + ((m_fDistanceMaxTolerance) / fCalibPixelPerMM).ToString("f5") + " mm,   Result = " + (fGapDistance / fCalibPixelPerMM).ToString("f3") + " mm";
                    m_intSealFailMask |= 0x02;
                    m_arrFailWidthStartPoint.Add(new List<PointF>());
                    m_arrFailWidthEndPoint.Add(new List<PointF>());
                    m_arrFailWidthStartPoint[0].Add(new PointF(objROI.ref_ROI.TotalOrgX + objROI.ref_ROIWidth / 2, fSealEdgePositionY));
                    m_arrFailWidthEndPoint[0].Add(new PointF(objROI.ref_ROI.TotalOrgX + objROI.ref_ROIWidth / 2, m_fSealBorderPositionY));
                    return false;
                }
            }

            return true;
        }

        public bool CheckDistanceBtwSealAndBorder_Old(ROI objROI, float fCalibPixelPerMM)
        {
            // Shift ROI position
            objROI.ref_ROIPositionX = objROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
            objROI.ref_ROIPositionY = objROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);

            if (m_objDistanceBlobs == null)
                m_objDistanceBlobs = new EBlobs();
            else
                m_objDistanceBlobs.CleanAllBlobs();

            m_objDistanceBlobs.BuildObjects_Filter_GetElement(objROI, true, true, 0, m_objDistanceBlobs.ref_intAbsoluteThreshold, objROI.ref_ROIWidth * 2, objROI.ref_ROIWidth * objROI.ref_ROIHeight, false, 0x0F);

            if (m_objDistanceBlobs.ref_intNumSelectedObject < 2)
            {
                m_strErrorMessage += "*Fail Seal Distance. Cannot find blob.";
                m_intSealFailMask |= 0x02;
                return false;
            }

            float fMinY = 0;
            int intMinPostYNo = 0;
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                    fMinY = m_objDistanceBlobs.ref_arrLimitCenterY[i];

                if (m_objDistanceBlobs.ref_arrLimitCenterY[i] < fMinY)
                {
                    fMinY = m_objDistanceBlobs.ref_arrLimitCenterY[i];
                    intMinPostYNo = i;
                }
            }

            PointF pCenter = new PointF(m_objDistanceBlobs.ref_arrLimitCenterX[intMinPostYNo], m_objDistanceBlobs.ref_arrLimitCenterY[intMinPostYNo]);
            float fHeight = m_objDistanceBlobs.ref_arrHeight[intMinPostYNo];
            float fBorderPositionY = pCenter.Y + (fHeight / 2);
            m_fSealBorderPositionY = objROI.ref_ROI.TotalOrgY + fBorderPositionY;

            int intMaxPosYNo = 0;
            if (intMinPostYNo == 0)
                intMaxPosYNo = 1;

            pCenter = new PointF(m_objDistanceBlobs.ref_arrLimitCenterX[intMaxPosYNo], m_objDistanceBlobs.ref_arrLimitCenterY[intMaxPosYNo]);
            fHeight = m_objDistanceBlobs.ref_arrHeight[intMaxPosYNo];
            float fSealTopPositionY = pCenter.Y - (fHeight / 2);

            float fGapDistance = fSealTopPositionY - fBorderPositionY;

            m_strTrack += ", GapDistance=" + fGapDistance.ToString();

            m_fLineWidthAverage[2] = fGapDistance;
            if (fGapDistance < (m_fDistanceMinTolerance))
            {
                m_FailDistance = (fGapDistance / fCalibPixelPerMM);
                m_strErrorMessage += "*Fail Seal Distance. Minimum Distance : Set = " + ((m_fDistanceMinTolerance) / fCalibPixelPerMM).ToString("f5") + " mm,   Result = " + (fGapDistance / fCalibPixelPerMM).ToString("f3") + " mm";
                m_intSealFailMask |= 0x02;
                m_arrFailWidthStartPoint.Add(new List<PointF>());
                m_arrFailWidthEndPoint.Add(new List<PointF>());
                m_arrFailWidthStartPoint[0].Add(new PointF(objROI.ref_ROI.TotalOrgX + objROI.ref_ROIWidth / 2, objROI.ref_ROI.TotalOrgY + fSealTopPositionY));
                m_arrFailWidthEndPoint[0].Add(new PointF(objROI.ref_ROI.TotalOrgX + objROI.ref_ROIWidth / 2, objROI.ref_ROI.TotalOrgY + fBorderPositionY));
                return false;
            }
            else if (fGapDistance > (m_fDistanceMaxTolerance))
            {
                m_FailDistance = (fGapDistance / fCalibPixelPerMM);
                m_strErrorMessage += "*Fail Seal Distance. Maximum Distance : Set = " + ((m_fDistanceMaxTolerance) / fCalibPixelPerMM).ToString("f5") + " mm,   Result = " + (fGapDistance / fCalibPixelPerMM).ToString("f3") + " mm";
                m_intSealFailMask |= 0x02;
                m_arrFailWidthStartPoint.Add(new List<PointF>());
                m_arrFailWidthEndPoint.Add(new List<PointF>());
                m_arrFailWidthStartPoint[0].Add(new PointF(objROI.ref_ROI.TotalOrgX + objROI.ref_ROIWidth / 2, objROI.ref_ROI.TotalOrgY + fSealTopPositionY));
                m_arrFailWidthEndPoint[0].Add(new PointF(objROI.ref_ROI.TotalOrgX + objROI.ref_ROIWidth / 2, objROI.ref_ROI.TotalOrgY + fBorderPositionY));
                return false;
            }

            return true;
        }

        /// <summary>
        /// Find seal border location from inside to outside and get average of seal width and its maximum and minimum width
        /// Record down all near seal and far seal location for drawing and create white blob ROI purpose
        /// m_arrGaugePositions[][][] structure => [1][][] : 1 = Near Seal (Seal 1), 0 = Far Seal (Seal 2)
        /// m_arrGaugePositions[][][] structure => [][0][] : Gauge 0 Info
        /// m_arrGaugePositions[][][] structure => [][][0] : Keep record of seal coordinate = new PointF(X, Y); 0 = inside seal, 1 = outside seal
        /// </summary>
        /// <param name="arrROIs">ROI defined by user</param>
        /// <param name="arrGauges">Line Gauge Info</param>
        /// <param name="fCalibY">calibration value</param>
        public bool BuildGauge(List<List<ROI>> arrROIs, List<List<LGauge>> arrGauges, float fCalibY, bool blnLearn)
        {
            m_fCalibY = fCalibY;

            float fDistance = 0;
            float fTotalValue = 0;
            int intCount = 0;
            ArrayList arrCount = new ArrayList();
            bool blnTestResult = true;


            for (int i = 0; i < 2; i++)   // far and near seal
            {
                fTotalValue = 0;
                intCount = 0;
                string strSealName = "Seal 1";
                if (i == 1)
                    strSealName = "Seal 2";

                if (!blnLearn)
                {
                    arrROIs[i + 1][0].ref_ROIPositionX = arrROIs[i + 1][0].ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
                    arrROIs[i + 1][0].ref_ROIPositionY = arrROIs[i + 1][0].ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);
                }

                float fLargestDistance = -1;
                float fSmallestDistance = -1;
                float fSmallestDistance_PassOnly = -1;
                float fLargestDistance_PassOnly = -1;
                float fSealCenterY_PassOnly = -1;
                float fCenterY = -1;
                float fStartYForCenterBar_PassOnly = -1;
                float fEndYForCenterBar_PassOnly = -1;
                List<float> arrStartYForCenterBar_PassOnly = new List<float>();
                List<float> arrEndYForCenterBar_PassOnly = new List<float>();
                if (m_arrGaugePositions.Count <= i)
                    m_arrGaugePositions.Add(new ArrayList());

                if (m_arrBrushMatched.Count <= i)
                    m_arrBrushMatched.Add(new List<SolidBrush>());

                m_arrFailWidthStartPoint.Add(new List<PointF>());
                m_arrFailWidthEndPoint.Add(new List<PointF>());

                m_arrGaugePositions[i].Clear();
                m_arrBrushMatched[i].Clear();
#if (Debug_2_12 || Release_2_12)
                if (m_intSealEdgeSensitivity > 0)
                    EasyImage.CloseBox(arrROIs[i + 1][0].ref_ROI.TopParent, m_objSealLineImage.ref_objMainImage, (uint)m_intSealEdgeSensitivity);
                else
                    EasyImage.Copy(arrROIs[i + 1][0].ref_ROI.TopParent, m_objSealLineImage.ref_objMainImage);
                //EasyImage.Threshold(m_objSealLineImage.ref_objMainImage, m_objSealLineImage.ref_objMainImage, (uint)m_arrBlackBlobs[i].ref_intAbsoluteThreshold);
                EasyImage.Threshold(m_objSealLineImage.ref_objMainImage, m_objSealLineImage.ref_objMainImage, (uint)m_intSealLineEdgeThreshold[i]);

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                if (m_intSealEdgeSensitivity > 0)
                    EasyImage.CloseBox(arrROIs[i + 1][0].ref_ROI.TopParent, m_objSealLineImage.ref_objMainImage, m_intSealEdgeSensitivity);
                else
                    EasyImage.Copy(arrROIs[i + 1][0].ref_ROI.TopParent, m_objSealLineImage.ref_objMainImage);
                //EasyImage.Threshold(m_objSealLineImage.ref_objMainImage, m_objSealLineImage.ref_objMainImage, m_arrBlackBlobs[i].ref_intAbsoluteThreshold);
                EasyImage.Threshold(m_objSealLineImage.ref_objMainImage, m_objSealLineImage.ref_objMainImage, m_intSealLineEdgeThreshold[i]);

#endif

                if (m_objThresholdSealROI == null)
                    m_objThresholdSealROI = new ROI();

                m_objThresholdSealROI.AttachImage(m_objSealLineImage);
                m_objThresholdSealROI.LoadROISetting(arrROIs[i + 1][0].ref_ROITotalX, arrROIs[i + 1][0].ref_ROITotalY, arrROIs[i + 1][0].ref_ROIWidth, arrROIs[i + 1][0].ref_ROIHeight);

                if (m_arrBlackBlobs[i] == null)
                    m_arrBlackBlobs[i] = new EBlobs();
                else
                    m_arrBlackBlobs[i].CleanAllBlobs();

                //m_objThresholdSealROI.SaveImage("D:\\TS\\m_objThresholdSealROI.bmp");
                //m_arrBlackBlobs[i].BuildObjects_Filter_GetElement(m_objThresholdSealROI, true, true, 0, m_arrBlackBlobs[i].ref_intAbsoluteThreshold, m_arrBlackBlobs[i].ref_intMinAreaLimit, m_arrBlackBlobs[i].ref_intMaxAreaLimit, false, 0x0F);
                m_arrBlackBlobs[i].BuildObjects_Filter_GetElement(m_objThresholdSealROI, true, true, 0, m_intSealLineEdgeThreshold[i], 1/*m_arrBlackBlobs[i].ref_intMinAreaLimit*/, m_arrBlackBlobs[i].ref_intMaxAreaLimit, false, 0x0F);

                float fLimitCenterY = 0;
                float fNewTolerance = 0;
                float fBlobsHeight = 0;

                if (m_arrBlackBlobs[i].ref_intNumSelectedObject > 0)
                {
                    float fBlobCenterY = 0;
                    fBlobCenterY = m_arrBlackBlobs[i].ref_arrLimitCenterY[0];
                    fBlobsHeight = m_arrBlackBlobs[i].ref_arrHeight[0];
                    if (i == 0)
                    {
                        fNewTolerance = (fBlobsHeight / 2 + arrROIs[i + 1][0].ref_ROIHeight - fBlobCenterY) / 2;
                        fLimitCenterY = arrROIs[i + 1][0].ref_ROITotalY + fBlobCenterY - fBlobsHeight / 2 + fNewTolerance;
                    }
                    else
                    {
                        fNewTolerance = (fBlobsHeight / 2 + arrROIs[i + 1][0].ref_ROIHeight - fBlobCenterY) / 2;
                        fLimitCenterY = arrROIs[i + 1][0].ref_ROITotalY + fBlobCenterY + fBlobsHeight / 2 - fNewTolerance;
                    }
                }
                else
                {
                    if (i == 0)
                        m_blnFailSeal1 = true;
                    else
                        m_blnFailSeal2 = true;
                    m_intSealFailMask |= 0x20; // 2021-01-28 ZJYEOH : Need assign fail mask, because if both seal also cannot find, AddFailCounter() in vision6Process will show no switch case found
                    m_strErrorMessage += "*Fail find " + strSealName;
                    blnTestResult = false;
                    continue;
                }
                int intFailSmallSegmentCount = 0;
                List<ArrayList> arrSmallPoint = new List<ArrayList>();
                List<ArrayList> arrOverPoint = new List<ArrayList>();
                int intFailOverSegmentCount = 0;
                float fSmallestValue = -1;
                float fLargestValue = -1;
                float fSmallestValue_PassOnly = -1;
                float fLargestValue_PassOnly = -1;
                float fLimitCenterX = 0;
                for (int j = 0; j < arrGauges[i].Count; j++)
                {
                    if (j == 30)
                    {

                    }
                    fLimitCenterX = (arrROIs[i + 1][0].ref_ROITotalX + arrGauges[i][j].ref_GaugeLength / 2) + arrGauges[i][j].ref_GaugeLength * j; // 2018 08 17 - JBTAN: gauge center X need to be shifted according to shifted ROI

                    m_intBuildCount = 0;
                    arrGauges[i][j].SetGaugeCenter(fLimitCenterX, fLimitCenterY);
                    arrGauges[i][j].SetGaugeTolerance(fNewTolerance);
                    ArrayList arrPoints = BuildGauge(m_objThresholdSealROI, arrGauges[i][j], i, fCalibY, blnLearn);

                    if (arrPoints == null)
                    {
                        arrPoints = new ArrayList();
                        arrPoints.Add(new PointF(arrGauges[i][j].ref_GaugeCenterX, arrGauges[i][j].ref_GaugeCenterY));
                        arrPoints.Add(new PointF(arrGauges[i][j].ref_GaugeCenterX, arrGauges[i][j].ref_GaugeCenterY));
                        //continue;
                    }

                    m_arrBrushMatched[i].Add(new SolidBrush(Color.Lime));
                    m_arrBrushMatched[i].Add(new SolidBrush(Color.Lime));

                    float fLineStart = ((PointF)arrPoints[0]).Y;
                    float fLineStop = ((PointF)arrPoints[1]).Y;
                    bool blnPassLine = true;
                    fDistance = Convert.ToSingle(Math.Round(Math.Abs(fLineStart - fLineStop), 4, MidpointRounding.AwayFromZero));
                    fCenterY = (fLineStart + fLineStop) / 2;

                    if (i == 0)
                    {
                        if (fDistance < (m_fWidthLowerTolerance1))
                        {
                            if (!blnLearn)
                            {
                                m_arrBrushMatched[i][j * 2].Color = Color.Yellow;
                                m_arrBrushMatched[i][(j * 2) + 1].Color = Color.Yellow;
                            }
                            intFailSmallSegmentCount++;
                            arrSmallPoint.Add(arrPoints);
                            if ((fSmallestValue == -1) || (fDistance < fSmallestValue))
                                fSmallestValue = fDistance;

                            blnPassLine = false;
                        }
                        else
                        {
                            //intFailSmallSegmentCount = 0;
                            arrSmallPoint.Clear();
                            fSmallestValue = fDistance;
                            fSmallestValue_PassOnly = fDistance;
                        }
                    }
                    else if (i == 1)
                    {
                        if (fDistance < (m_fWidthLowerTolerance2))
                        {
                            if (!blnLearn)
                            {
                                m_arrBrushMatched[i][j * 2].Color = Color.Yellow;
                                m_arrBrushMatched[i][(j * 2) + 1].Color = Color.Yellow;
                            }
                            intFailSmallSegmentCount++;
                            arrSmallPoint.Add(arrPoints);
                            if ((fSmallestValue == -1) || (fDistance < fSmallestValue))
                                fSmallestValue = fDistance;

                            blnPassLine = false;
                        }
                        else
                        {
                            //intFailSmallSegmentCount = 0;
                            arrSmallPoint.Clear();
                            fSmallestValue = fDistance;
                            fSmallestValue_PassOnly = fDistance;
                        }
                    }
                    if (i == 0)
                    {
                        if (fDistance > (m_fWidthUpperTolerance1))
                        {
                            if (!blnLearn)
                            {
                                m_arrBrushMatched[i][j * 2].Color = Color.Yellow;
                                m_arrBrushMatched[i][(j * 2) + 1].Color = Color.Yellow;
                            }
                            intFailOverSegmentCount++;
                            arrOverPoint.Add(arrPoints);
                            if ((fLargestValue == -1) || (fDistance > fLargestValue))
                                fLargestValue = fDistance;

                            blnPassLine = false;
                        }
                        else
                        {
                            //intFailOverSegmentCount = 0;
                            arrOverPoint.Clear();
                            fLargestValue = fDistance;
                            fLargestValue_PassOnly = fDistance;
                        }
                    }
                    else if (i == 1)
                    {
                        if (fDistance > (m_fWidthUpperTolerance2))
                        {
                            if (!blnLearn)
                            {
                                m_arrBrushMatched[i][j * 2].Color = Color.Yellow;
                                m_arrBrushMatched[i][(j * 2) + 1].Color = Color.Yellow;
                            }
                            intFailOverSegmentCount++;
                            arrOverPoint.Add(arrPoints);
                            if ((fLargestValue == -1) || (fDistance > fLargestValue))
                                fLargestValue = fDistance;

                            blnPassLine = false;
                        }
                        else
                        {
                            //intFailOverSegmentCount = 0;
                            arrOverPoint.Clear();
                            fLargestValue = fDistance;
                            fLargestValue_PassOnly = fDistance;
                        }
                    }
                    //if (intFailSmallSegmentCount >= m_fMinBrokenWidth)
                    //{
                    for (int p = 0; p < arrSmallPoint.Count; p++)
                    {
                        //m_arrFailWidthStartPoint[i].Add((PointF)arrSmallPoint[p][0]);
                        //m_arrFailWidthEndPoint[i].Add((PointF)arrSmallPoint[p][1]);
                    }

                    if (blnPassLine)
                    {
                        arrStartYForCenterBar_PassOnly.Add(fLineStart);
                        arrEndYForCenterBar_PassOnly.Add(fLineStop);
                        if (fStartYForCenterBar_PassOnly == -1 || fStartYForCenterBar_PassOnly < fLineStart)
                        {
                            fStartYForCenterBar_PassOnly = fLineStart;
                        }
                        if (fEndYForCenterBar_PassOnly == -1 || fEndYForCenterBar_PassOnly < fLineStop)
                        {
                            fEndYForCenterBar_PassOnly = fLineStop;
                        }
                    }

                    if ((fSmallestDistance == -1) || (fSmallestValue < fSmallestDistance) && fSmallestValue != -1)
                        fSmallestDistance = fSmallestValue;

                    if ((fSmallestDistance_PassOnly == -1) || (fSmallestValue_PassOnly < fSmallestDistance_PassOnly) && fSmallestValue_PassOnly != -1)
                    {
                        fSmallestDistance_PassOnly = fSmallestValue_PassOnly;
                        fSealCenterY_PassOnly = fCenterY;
                    }


                    //if (intFailOverSegmentCount >= m_fMinBrokenWidth)
                    //{
                    //for (int p = 0; p < arrOverPoint.Count; p++)
                    //{
                    //    m_arrFailWidthStartPoint[i].Add((PointF)arrOverPoint[p][0]);
                    //    m_arrFailWidthEndPoint[i].Add((PointF)arrOverPoint[p][1]);
                    //}

                    if ((fLargestDistance == -1) || (fLargestValue > fLargestDistance) && fLargestValue != -1)
                        fLargestDistance = fLargestValue;

                    if ((fLargestDistance_PassOnly == -1) || (fLargestValue_PassOnly > fLargestDistance_PassOnly) && fLargestValue_PassOnly != -1)
                        fLargestDistance_PassOnly = fLargestValue_PassOnly;
                    //}

                    //if ((fDistance > (m_fTemplateWidth[i] + m_fWidthUpperTolerance)) ||
                    //    (fDistance < (m_fTemplateWidth[i] - m_fWidthLowerTolerance)))
                    //{
                    //    m_arrFailWidthStartPoint[i].Add((PointF)arrPoints[0]);
                    //    m_arrFailWidthEndPoint[i].Add((PointF)arrPoints[1]);
                    //}

                    //int intDistance = (int)Math.Ceiling(fDistance);
                    fTotalValue += fDistance;
                    intCount++;

                    m_arrGaugePositions[i].Add(arrPoints[0]);
                    m_arrGaugePositions[i].Add(arrPoints[1]);
                }

                if (fStartYForCenterBar_PassOnly != -1 && fEndYForCenterBar_PassOnly != -1)
                {
                    float fStartYForCenterBar = Math2.GetModeValue(arrStartYForCenterBar_PassOnly.ToArray(), 5);
                    float fEndYForCenterBar = Math2.GetModeValue(arrEndYForCenterBar_PassOnly.ToArray(), 5);
                    fSealCenterY_PassOnly = (fStartYForCenterBar + fEndYForCenterBar) / 2;
                    //fSealCenterY_PassOnly = (fStartYForCenterBar_PassOnly + fEndYForCenterBar_PassOnly) / 2;
                }

                if (!blnLearn)
                {
                    m_strTrack += ", Seal " + (i + 1).ToString() + " LargestWidth=" + fLargestDistance.ToString();
                    m_strTrack += ", Seal " + (i + 1).ToString() + " fSmallestWidth=" + fSmallestDistance.ToString();
                    if (i == 0)
                        m_fFailSealScore[0] = (1 - (Convert.ToSingle(intFailSmallSegmentCount + intFailOverSegmentCount) / arrGauges[i].Count));//13/01/2019 ZJYEOH : changed m_intBuildObjectLength to arrGauges[i].Count to avoid negative result
                    else if (i == 1)
                        m_fFailSealScore[1] = (1 - (Convert.ToSingle(intFailSmallSegmentCount + intFailOverSegmentCount) / arrGauges[i].Count));

                    if (m_fSealScoreTolerance > (1 - (Convert.ToSingle(intFailSmallSegmentCount + intFailOverSegmentCount) / arrGauges[i].Count)))
                    {
                        if (((m_intFailOptionMask & 0x01) > 0))
                        {
                            m_strErrorMessage += "*" + strSealName + " Edge Score Fail. Set = " + m_fSealScoreTolerance * 100 + "%,   Result = " + (1 - (Convert.ToSingle(intFailSmallSegmentCount + intFailOverSegmentCount) / arrGauges[i].Count)) * 100 + "%";
                            m_intSealFailMask |= 0x10;
                            if (blnTestResult)
                                blnTestResult = false;

                            if (i == 0)
                            {
                                m_blnFailSeal1 = true;
                            }
                            else
                            {
                                m_blnFailSeal2 = true;
                            }

                            for (int b = 0; b < m_arrBrushMatched[i].Count; b++)
                            {
                                if (m_arrBrushMatched[i][b].Color == Color.Yellow)
                                    m_arrBrushMatched[i][b].Color = Color.Red;
                            }

                            //////if (i == 0)
                            //////{
                            //////    if (fLargestDistance > (m_fWidthUpperTolerance1) && fLargestDistance != -1)
                            //////    {
                            //////        m_blnFailSeal1 = true;
                            //////        m_FailOverSeal[i] = fLargestDistance / fCalibY;
                            //////        m_FailOverSeal[1] = 0;
                            //////        m_strErrorMessage += "*Line Width is Overseal at " + strSealName + " Set = " +
                            //////            ((m_fWidthUpperTolerance1) / fCalibY).ToString("f5") + " mm,   Result = " + Math.Round(fLargestDistance / fCalibY, 3, MidpointRounding.AwayFromZero).ToString() + " mm";
                            //////        m_intSealFailMask |= 0x10;
                            //////        if (blnTestResult)
                            //////            blnTestResult = false;
                            //////    }
                            //////}
                            //////else if (i == 1)
                            //////{
                            //////    if (fLargestDistance > (m_fWidthUpperTolerance2) && fLargestDistance != -1)
                            //////    {
                            //////        m_blnFailSeal2 = true;
                            //////        m_FailOverSeal[0] = 0;
                            //////        m_FailOverSeal[i] = fLargestDistance / fCalibY;
                            //////        m_strErrorMessage += "*Line Width is Overseal at " + strSealName + " Set = " +
                            //////            ((m_fWidthUpperTolerance2) / fCalibY).ToString("f5") + " mm,   Result = " + Math.Round(fLargestDistance / fCalibY, 3, MidpointRounding.AwayFromZero).ToString() + " mm";
                            //////        m_intSealFailMask |= 0x10;
                            //////        if (blnTestResult)
                            //////            blnTestResult = false;
                            //////    }
                            //////}


                            //////if (i == 0)
                            //////{
                            //////    if (fSmallestDistance < (m_fWidthLowerTolerance1) && fSmallestDistance != -1)
                            //////    {
                            //////        m_blnFailSeal1 = true;
                            //////        m_FailInsufficient[i] = fSmallestDistance / fCalibY;
                            //////        m_FailInsufficient[1] = 0;
                            //////        m_strErrorMessage += "*Line Width is insufficient at " + strSealName + " Set = " +
                            //////            ((m_fWidthLowerTolerance1) / fCalibY).ToString("f5") + " mm,   Result = " + Math.Round(fSmallestDistance / fCalibY, 3, MidpointRounding.AwayFromZero).ToString() + " mm";
                            //////        m_intSealFailMask |= 0x20;
                            //////        if (blnTestResult)
                            //////            blnTestResult = false;
                            //////    }
                            //////}
                            //////else if (i == 1)
                            //////{
                            //////    if (fSmallestDistance < (m_fWidthLowerTolerance2) && fSmallestDistance != -1)
                            //////    {
                            //////        m_blnFailSeal2 = true;
                            //////        m_FailInsufficient[0] = 0;
                            //////        m_FailInsufficient[i] = fSmallestDistance / fCalibY;
                            //////        m_strErrorMessage += "*Line Width is insufficient at " + strSealName + " Set = " +
                            //////            ((m_fWidthLowerTolerance2) / fCalibY).ToString("f5") + " mm,   Result = " + Math.Round(fSmallestDistance / fCalibY, 3, MidpointRounding.AwayFromZero).ToString() + " mm";
                            //////        m_intSealFailMask |= 0x20;
                            //////        if (blnTestResult)
                            //////            blnTestResult = false;
                            //////    }
                            //////}
                        }
                    }
                }

                if (intCount > 0)
                    m_fLineWidthAverage[i] = Convert.ToSingle(Math.Round(fTotalValue / intCount, 4, MidpointRounding.AwayFromZero));
                if (i == 0)
                {
                    m_fLineWidthAverage[3] = fSmallestDistance;
                    m_fLineWidthAverage[4] = fLargestDistance;

                    m_fLineWidthAverage[8] = fSmallestDistance_PassOnly;
                    m_fLineWidthAverage[9] = fLargestDistance_PassOnly;
                    m_fLineWidthAverage[10] = fSealCenterY_PassOnly;
                }
                else
                {
                    m_fLineWidthAverage[5] = fSmallestDistance;
                    m_fLineWidthAverage[6] = fLargestDistance;

                    m_fLineWidthAverage[11] = fSmallestDistance_PassOnly;
                    m_fLineWidthAverage[12] = fLargestDistance_PassOnly;
                    m_fLineWidthAverage[13] = fSealCenterY_PassOnly;
                }
            }

            return blnTestResult;
        }

        //public bool BuildGauge2(List<List<ROI>> arrROIs, List<List<LGauge>> arrGauges, float fCalibY, bool blnLearn)
        //{
        //    m_fCalibY = fCalibY;

        //    float fDistance = 0;
        //    int intTotalValue = 0, intCount = 0;
        //    ArrayList arrCount = new ArrayList();
        //    bool blnTestResult = true;


        //    for (int i = 0; i < 2; i++)   // far and near seal
        //    {
        //        string strSealName = "Seal 1";
        //        if (i == 1)
        //            strSealName = "Seal 2";

        //        float fLargestDistance = -1;
        //        float fSmallestDistance = -1;

        //        if (m_arrGaugePositions.Count <= i)
        //            m_arrGaugePositions.Add(new ArrayList());

        //        m_arrFailWidthStartPoint.Add(new List<PointF>());
        //        m_arrFailWidthEndPoint.Add(new List<PointF>());

        //        m_arrGaugePositions[i].Clear();

        //        EasyImage.Copy(arrROIs[i + 1][0].ref_ROI.TopParent, m_objSealLineImage.ref_objMainImage);
        //        EasyImage.Threshold(m_objSealLineImage.ref_objMainImage, m_objSealLineImage.ref_objMainImage, m_arrBlackBlobs[i].ref_intThreshold);
        //        m_objThresholdSealROI = new ROI();
        //        m_objThresholdSealROI.AttachImage(m_objSealLineImage);
        //        m_objThresholdSealROI.LoadROISetting(arrROIs[i + 1][0].ref_ROITotalX, arrROIs[i + 1][0].ref_ROITotalY, arrROIs[i + 1][0].ref_ROIWidth, arrROIs[i + 1][0].ref_ROIHeight);

        //        m_arrBlackBlobs[i].BuildObjects(arrROIs[i + 1][0], false, false);
        //        float fLimitCenterY = 0;
        //        float fNewTolerance = 0;
        //        float fBlobsHeight = 0;
        //        m_arrBlackBlobs[i].SetFirstListBlobs();
        //        if (m_arrBlackBlobs[i].ref_intNumSelectedObject > 0)
        //        {
        //            float fBlobCenterY = 0;
        //            m_arrBlackBlobs[i].GetSelectedListBlobsLimitCenterY(ref fBlobCenterY);
        //            m_arrBlackBlobs[i].GetSelectedListBlobsHeight(ref fBlobsHeight);
        //            if (i == 0)
        //            {
        //                fNewTolerance = (fBlobsHeight / 2 + arrROIs[i + 1][0].ref_ROIHeight - fBlobCenterY) / 2;
        //                fLimitCenterY = arrROIs[i + 1][0].ref_ROITotalY + fBlobCenterY - fBlobsHeight / 2 + fNewTolerance;
        //            }
        //            else
        //            {
        //                fNewTolerance = (fBlobsHeight / 2 + arrROIs[i + 1][0].ref_ROIHeight - fBlobCenterY) / 2;
        //                fLimitCenterY = arrROIs[i + 1][0].ref_ROITotalY + fBlobCenterY + fBlobsHeight / 2 - fNewTolerance;
        //            }
        //        }
        //        else
        //        {
        //            m_strErrorMessage += "*Fail find " + strSealName;
        //            blnTestResult = false;
        //            continue;
        //        }

        //        for (int j = 0; j < arrGauges[i].Count; j++)
        //        {
        //            if (j == 66)
        //            { }

        //            m_intBuildCount = 0;
        //            arrGauges[i][j].SetGaugeCenter(fLimitCenterY);
        //            arrGauges[i][j].SetGaugeTolerance(fNewTolerance);
        //            ArrayList arrPoints = BuildGauge(m_objThresholdSealROI, arrGauges[i][j], i, fCalibY, blnLearn);

        //            if (arrPoints == null)
        //            {
        //                arrPoints = new ArrayList();
        //                arrPoints.Add(new PointF(arrGauges[i][j].ref_GaugeCenterX, arrGauges[i][j].ref_GaugeCenterY));
        //                arrPoints.Add(new PointF(arrGauges[i][j].ref_GaugeCenterX, arrGauges[i][j].ref_GaugeCenterY));
        //                //continue;
        //            }

        //            float fLineStart = ((PointF)arrPoints[0]).Y;
        //            float fLineStop = ((PointF)arrPoints[1]).Y;

        //            fDistance = Math.Abs(fLineStart - fLineStop);

        //            if ((fSmallestDistance == -1) || (fDistance < fSmallestDistance))
        //                fSmallestDistance = fDistance;

        //            if ((fLargestDistance == -1) || (fDistance > fLargestDistance))
        //                fLargestDistance = fDistance;

        //            if ((fDistance > (m_fTemplateWidth[i] + m_fWidthUpperTolerance)) ||
        //                (fDistance < (m_fTemplateWidth[i] - m_fWidthLowerTolerance)))
        //            {
        //                m_arrFailWidthStartPoint[i].Add((PointF)arrPoints[0]);
        //                m_arrFailWidthEndPoint[i].Add((PointF)arrPoints[1]);
        //            }

        //            int intDistance = (int)Math.Ceiling(fDistance);
        //            intTotalValue += intDistance;
        //            intCount++;

        //            m_arrGaugePositions[i].Add(arrPoints[0]);
        //            m_arrGaugePositions[i].Add(arrPoints[1]);
        //        }

        //        if (!blnLearn)
        //        {
        //            if (fLargestDistance > (m_fTemplateWidth[i] + m_fWidthUpperTolerance))
        //            {
        //                m_strErrorMessage += "*Line Width is Overseal at " + strSealName + " Set = " +
        //                    ((m_fTemplateWidth[i] + m_fWidthUpperTolerance) / fCalibY).ToString("f5") + " mm,   Result = " + Math.Round(fLargestDistance / fCalibY, 3, MidpointRounding.AwayFromZero).ToString() + " mm";
        //                m_intSealFailMask |= 0x10;
        //                if (blnTestResult)
        //                    blnTestResult = false;
        //            }
        //            if (fSmallestDistance < (m_fTemplateWidth[i] - m_fWidthLowerTolerance))
        //            {
        //                m_strErrorMessage += "*Line Width is Insufficient Seal at " + strSealName + " Set = " +
        //                    ((m_fTemplateWidth[i] - m_fWidthLowerTolerance) / fCalibY).ToString("f5") + " mm,   Result = " + Math.Round(fSmallestDistance / fCalibY, 3, MidpointRounding.AwayFromZero).ToString() + " mm";
        //                m_intSealFailMask |= 0x20;
        //                if (blnTestResult)
        //                    blnTestResult = false;

        //            }
        //        }

        //        if (intCount > 0)
        //            m_fLineWidthAverage[i] = (float)intTotalValue / intCount;
        //        if (i == 0)
        //        {
        //            m_fLineWidthAverage[3] = fSmallestDistance;
        //            m_fLineWidthAverage[4] = fLargestDistance;
        //        }
        //        else
        //        {
        //            m_fLineWidthAverage[5] = fSmallestDistance;
        //            m_fLineWidthAverage[6] = fLargestDistance;
        //        }
        //    }

        //    return blnTestResult;
        //}
        /// <summary>
        /// Build blobs objects in ROI
        /// </summary>
        /// <param name="arrROIs">ROI</param>
        public void BuildObjects(List<List<ROI>> arrROIs)
        {
            for (int i = 0; i < 2; i++)
            {
                //m_arrBlackBlobs[i].BuildObjects(arrROIs[i + 1][0]);

                if (m_arrBlackBlobs[i] == null)
                    m_arrBlackBlobs[i] = new EBlobs();
                else
                    m_arrBlackBlobs[i].CleanAllBlobs();

                //                arrROIs[i + 1][0].SaveImage("D:\\TS\\ROI.bmp");
                //m_arrBlackBlobs[i].BuildObjects_Filter_GetElement(arrROIs[i + 1][0], true, true, 0, m_arrBlackBlobs[i].ref_intAbsoluteThreshold, m_arrBlackBlobs[i].ref_intMinAreaLimit, m_arrBlackBlobs[i].ref_intMaxAreaLimit, false, 0x0F);
                m_arrBlackBlobs[i].BuildObjects_Filter_GetElement(arrROIs[i + 1][0], true, true, 0, m_intSealLineEdgeThreshold[i], 1/*m_arrBlackBlobs[i].ref_intMinAreaLimit*/, m_arrBlackBlobs[i].ref_intMaxAreaLimit, false, 0x0F);
            }
        }

        //public void DefineSealBlobs(List<List<ROI>> arrROIs)
        //{
        //    BuildObjects(arrROIs);

        //    for (int i = 0; i < 2; i++)
        //    {
        //        if (m_arrBlackBlobs[i].ref_intNumSelectedObject > 1)
        //        {
        //        }
        //    }
        //}


        /// <summary>
        /// Draw gauge
        /// </summary>
        /// <param name="g">destination to draw the image</param>
        public void DrawGauge(Graphics g, List<List<LGauge>> arrSealGauge, List<List<ROI>> arrSealROI, float fScaleX, float fScaleY)
        {
            try
            {
                Font Font = new Font("Tahoma", 8);
                SolidBrush solidBrush = new SolidBrush(Color.Red);
                for (int x = 0; x < m_arrGaugePositions.Count; x++)
                {
                    for (int y = 0; y < m_arrGaugePositions[x].Count; y++)
                    {
                        int intCenterX = Convert.ToInt32(((PointF)m_arrGaugePositions[x][y]).X);
                        int intCenterY = Convert.ToInt32(((PointF)m_arrGaugePositions[x][y]).Y);

                        g.DrawLine(new Pen(m_arrBrushMatched[x][y]), new System.Drawing.Point(Convert.ToInt32((intCenterX - 5) * fScaleX), Convert.ToInt32(intCenterY * fScaleY)),
                            new System.Drawing.Point(Convert.ToInt32((intCenterX + 5) * fScaleX), Convert.ToInt32(intCenterY * fScaleY)));
                        g.DrawLine(new Pen(m_arrBrushMatched[x][y]), new System.Drawing.Point(Convert.ToInt32(intCenterX * fScaleX), Convert.ToInt32((intCenterY - 5) * fScaleY)),
                            new System.Drawing.Point(Convert.ToInt32(intCenterX * fScaleX), Convert.ToInt32((intCenterY + 5) * fScaleY)));
                        //if ((y % 10) >1 && (y % 10) < 4)
                        //g.DrawString(y.ToString(), Font, solidBrush, intCenterX, intCenterY);

                        if (m_blnViewSegmentDrawing)
                        {
                            int intIndexY;
                            if (y % 2 != 0)
                            {
                                intIndexY = (int)Math.Floor((float)y / 2);
                            }
                            else
                            {
                                intIndexY = y / 2;
                            }

                            g.DrawRectangle(new Pen(Color.Yellow), (arrSealGauge[x][intIndexY].ref_GaugeCenterX - arrSealGauge[x][intIndexY].ref_GaugeLength / 2) * fScaleX,
                                                                       (arrSealGauge[x][intIndexY].ref_GaugeCenterY - arrSealGauge[x][intIndexY].ref_GaugeTolerance) * fScaleY,
                                                                        arrSealGauge[x][intIndexY].ref_GaugeLength * fScaleX, (arrSealGauge[x][intIndexY].ref_GaugeTolerance * 2) * fScaleY);

                            LGauge objTempGauge = new LGauge();
                            if (x == 0)
                                arrSealGauge[x][intIndexY].DuplicateTopGauge(ref objTempGauge, arrSealROI[x + 1][0], m_fCalibY / 10);
                            else
                                arrSealGauge[x][intIndexY].DuplicateBottomGauge2(ref objTempGauge, arrSealROI[x + 1][0], m_arrBlackBlobs[1]);

                            g.DrawRectangle(new Pen(Color.Cyan), (objTempGauge.ref_GaugeCenterX - objTempGauge.ref_GaugeLength / 2) * fScaleX,
                                               (objTempGauge.ref_GaugeCenterY - objTempGauge.ref_GaugeTolerance) * fScaleY,
                                                objTempGauge.ref_GaugeLength * fScaleX, (objTempGauge.ref_GaugeTolerance * 2) * fScaleY);

                        }
                    }
                }
            }
            catch { }

        }

        /// <summary>
        /// Draw selected blobs objects
        /// </summary>
        /// <param name="g">destination to draw the image</param>
        public void DrawObjects(Graphics g, List<List<ROI>> arrSealROIs, float fCalibMMPerPixel)
        {
            Font m_FontMatched = new Font("Tahoma", 12);

            for (int i = 0; i < 2; i++)
            {
                m_arrBlackBlobs[i].DrawSelectedBlobs(g, 1f, 1f);

                int intMinArea = 0, intMaxArea = 0;
                intMinArea = intMaxArea = m_arrBlackBlobs[i].ref_arrArea[0];

                for (int j = 1; j < m_arrBlackBlobs[i].ref_intNumSelectedObject; j++)
                {
                    if (m_arrBlackBlobs[i].ref_arrArea[j] > intMaxArea)
                        intMaxArea = m_arrBlackBlobs[i].ref_arrArea[j];

                    if (m_arrBlackBlobs[i].ref_arrArea[j] < intMinArea)
                        intMinArea = m_arrBlackBlobs[i].ref_arrArea[j];
                }

                //m_arrBlackBlobs[i].GetBlobsMinMaxArea(ref intMinArea, ref intMaxArea);
                float fMinArea = (float)Math.Round((float)intMinArea * fCalibMMPerPixel * fCalibMMPerPixel, 5, MidpointRounding.AwayFromZero);
                float fMaxArea = (float)Math.Round((float)intMaxArea * fCalibMMPerPixel * fCalibMMPerPixel, 5, MidpointRounding.AwayFromZero);
                g.DrawString("Blob Min Area = " + fMinArea.ToString() + " , Max Area = " + fMaxArea.ToString(),
                            m_FontMatched,
                            new SolidBrush(Color.Red), arrSealROIs[i + 1][0].ref_ROIPositionX + 5, arrSealROIs[i + 1][0].ref_ROIPositionY - 20);
            }
        }

        public void DrawSProcketDistanceResult(Graphics g, CirGauge objCircleGauge, float fCalibMMPerPixel, float fScaleX, float fScaleY)
        {
            Pen pen1 = new Pen(Color.Red, 4);

            float fStartX = objCircleGauge.ref_ObjectCenterX * fScaleX;

            float fStartY;
            if (m_intSpocketHolePosition == 0)
                fStartY = (objCircleGauge.ref_ObjectCenterY + objCircleGauge.ref_fDiameter / 2) * fScaleY;
            else
                fStartY = (objCircleGauge.ref_ObjectCenterY - objCircleGauge.ref_fDiameter / 2) * fScaleY;

            float fEndX = objCircleGauge.ref_ObjectCenterX * fScaleX;
            float fEndY = m_fSealBorderPositionY * fScaleY;

            if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
            {
                g.DrawLine(pen1, fStartX, fStartY, fEndX, fEndY);
            }

        }
        public void DrawSProcketDiameterResult(Graphics g, CirGauge objCircleGauge, float fCalibMMPerPixel, float fScaleX, float fScaleY)
        {
            Pen pen1 = new Pen(Color.Red, 4);
            Pen penCircle = new Pen(Color.Red, 1);

            float fStartX, fStartY, fEndX, fEndY;

            if (objCircleGauge.ref_fDiameter < m_fSprocketHoleDiameterMinTolerance)
            {
                fStartX = objCircleGauge.ref_ObjectCenterX * fScaleX;

                fStartY = (objCircleGauge.ref_ObjectCenterY - m_fSprocketHoleDiameterMinTolerance / 2) * fScaleY;

                fEndX = objCircleGauge.ref_ObjectCenterX * fScaleX;

                fEndY = (objCircleGauge.ref_ObjectCenterY - objCircleGauge.ref_fDiameter / 2) * fScaleY;

                if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
                {
                    g.DrawLine(pen1, fStartX, fStartY, fEndX, fEndY);
                }

                fStartX = (objCircleGauge.ref_ObjectCenterX - (Math.Abs(m_fSprocketHoleDiameterMinTolerance)) / 2) * fScaleX;

                fStartY = (objCircleGauge.ref_ObjectCenterY - (Math.Abs(m_fSprocketHoleDiameterMinTolerance)) / 2) * fScaleY;

                if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
                {
                    g.DrawEllipse(penCircle, fStartX, fStartY, m_fSprocketHoleDiameterMinTolerance * fScaleX, m_fSprocketHoleDiameterMinTolerance * fScaleY);
                }
            }

            if (objCircleGauge.ref_fDiameter > m_fSprocketHoleDiameterMaxTolerance)
            {
                fStartX = objCircleGauge.ref_ObjectCenterX * fScaleX;

                fStartY = (objCircleGauge.ref_ObjectCenterY - m_fSprocketHoleDiameterMaxTolerance / 2) * fScaleY;

                fEndX = objCircleGauge.ref_ObjectCenterX * fScaleX;

                fEndY = (objCircleGauge.ref_ObjectCenterY - objCircleGauge.ref_fDiameter / 2) * fScaleY;

                if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
                {
                    g.DrawLine(pen1, fStartX, fStartY, fEndX, fEndY);
                }

                fStartX = (objCircleGauge.ref_ObjectCenterX - (Math.Abs(m_fSprocketHoleDiameterMaxTolerance)) / 2) * fScaleX;

                fStartY = (objCircleGauge.ref_ObjectCenterY - (Math.Abs(m_fSprocketHoleDiameterMaxTolerance)) / 2) * fScaleY;

                if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
                {
                    g.DrawEllipse(penCircle, fStartX, fStartY, m_fSprocketHoleDiameterMaxTolerance * fScaleX, m_fSprocketHoleDiameterMaxTolerance * fScaleY);
                }
            }

        }
        public void DrawSProcketDefectResult(Graphics g, CirGauge objCircleGauge, float fCalibMMPerPixel, float fScaleX, float fScaleY, float fPanX, float fPanY)
        {
            try
            {
                float fStartX = (objCircleGauge.ref_ObjectCenterX - (Math.Max(objCircleGauge.ref_fDiameter - m_intSprocketHoleInspectionAreaInwardTolerance, 5)) / 2) * fScaleX;

                float fStartY = (objCircleGauge.ref_ObjectCenterY - (Math.Max(objCircleGauge.ref_fDiameter - m_intSprocketHoleInspectionAreaInwardTolerance, 5)) / 2) * fScaleY;

                if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
                {
                    g.DrawEllipse(new Pen(Color.Red, 1), fStartX, fStartY, (Math.Max(objCircleGauge.ref_fDiameter - m_intSprocketHoleInspectionAreaInwardTolerance, 5)) * fScaleX, (Math.Max(objCircleGauge.ref_fDiameter - m_intSprocketHoleInspectionAreaInwardTolerance, 5)) * fScaleY);
                }

                for (int i = 0; i < m_objSprocketHoleDefectBlobs.ref_intNumSelectedObject; i++)
                {
                    m_objSprocketHoleDefectBlobs.DrawSelectedBlobs(g, fScaleX, fScaleY, Color.Red, -fPanX, -fPanY);
                }
            }
            catch
            {

            }
        }
        public void DrawSProcketBrokenResult(Graphics g, CirGauge objCircleGauge, float fCalibMMPerPixel, float fScaleX, float fScaleY, float fPanX, float fPanY)
        {
            try
            {
                float fStartX = (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2) * fScaleX;

                float fStartY = (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2) * fScaleY;

                if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
                {
                    g.DrawEllipse(new Pen(Color.Red, 1), fStartX, fStartY, (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) * fScaleX, (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) * fScaleY);
                }

                fStartX = (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) / 2) * fScaleX;

                fStartY = (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) / 2) * fScaleY;

                if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
                {
                    g.DrawEllipse(new Pen(Color.Red, 1), fStartX, fStartY, (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) * fScaleX, (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) * fScaleY);
                }

                for (int i = 0; i < m_objSprocketHoleBrokenBlobs.ref_intNumSelectedObject; i++)
                {
                    m_objSprocketHoleBrokenBlobs.DrawSelectedBlobs(g, fScaleX, fScaleY, Color.Red, -fPanX, -fPanY);
                }
            }
            catch
            {

            }
        }
        public void DrawSProcketRoundnessResult(Graphics g, CirGauge objCircleGauge, float fCalibMMPerPixel, float fScaleX, float fScaleY, float fPanX, float fPanY)
        {
            try
            {
                float fStartX = (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2) * fScaleX;

                float fStartY = (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2) * fScaleY;

                if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
                {
                    g.DrawEllipse(new Pen(Color.Red, 1), fStartX, fStartY, (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) * fScaleX, (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) * fScaleY);
                }

                for (int i = 0; i < m_objSprocketHoleRoundnessBlobs.ref_intNumSelectedObject; i++)
                {
                    m_objSprocketHoleRoundnessBlobs.DrawSelectedBlobs(g, fScaleX, fScaleY, Color.Red, -fPanX, -fPanY);
                }
            }
            catch
            {

            }
        }
        public void DrawProductionObjects(Graphics g, List<List<ROI>> arrSealROIs, float fCalibMMPerPixel, float fScaleX, float fScaleY)
        {
            try
            {
                if ((m_intFailOptionMask & 0x02) > 0)
                {
                    for (int i = 0; i < m_arrBlackBlobs.Count; i++) // for (int i = 0; i < m_arrBlackBlobs.Count - 1; i++)
                    {
                        if (i >= m_arrFailBlobsIndex.Count)
                            continue;

                        for (int j = 0; j < m_arrFailBlobsIndex[i].Count; j++)
                        {
                            m_arrBlackBlobs[i].DrawSelectedBlob(g, fScaleX, fScaleY, Color.Red, m_arrFailBlobsIndex[i][j]);

                            // prevent too many objects drawn and cause software crash
                            if (j >= 9)
                                break;
                        }
                    }
                }

                if (((m_intFailOptionMask & 0x01) > 0) || ((m_intFailOptionMask & 0x08) > 0))
                {
                    for (int i = 0; i < m_arrFailWidthStartPoint.Count; i++)
                    {
                        for (int k = 0; k < m_arrFailWidthStartPoint[i].Count; k++)
                        {
                            g.DrawLine(new Pen(Color.Red, 2), m_arrFailWidthStartPoint[i][k].X * fScaleX,
                                                             m_arrFailWidthStartPoint[i][k].Y * fScaleY,
                                                             m_arrFailWidthEndPoint[i][k].X * fScaleX,
                                                             m_arrFailWidthEndPoint[i][k].Y * fScaleY);

                            g.DrawLine(new Pen(Color.Red, 2), m_arrFailWidthStartPoint[i][k].X * fScaleX - 50,
                                                             m_arrFailWidthStartPoint[i][k].Y * fScaleY,
                                                              m_arrFailWidthStartPoint[i][k].X * fScaleX + 50,
                                                             m_arrFailWidthStartPoint[i][k].Y * fScaleY);

                            g.DrawLine(new Pen(Color.Red, 2), m_arrFailWidthEndPoint[i][k].X * fScaleX - 50,
                                                             m_arrFailWidthEndPoint[i][k].Y * fScaleY,
                                                             m_arrFailWidthEndPoint[i][k].X * fScaleX + 50,
                                                             m_arrFailWidthEndPoint[i][k].Y * fScaleY);

                            //g.DrawLine(new Pen(Color.Red, 1), m_arrFailWidthStartPoint[i][k].X * fScaleX, m_arrFailWidthStartPoint[i][k].Y * fScaleY, m_arrFailWidthEndPoint[i][k].X * fScaleX, m_arrFailWidthEndPoint[i][k].Y * fScaleY);
                        }
                    }
                }

                if ((m_intFailOptionMask & 0x10) > 0)
                {
                    for (int a = 0; a < m_arrFailOverHeat.Length; a++)
                    {
                        if (((m_intSealFailMask & 0x40) > 0) && m_arrFailOverHeat[a])
                        {
                            for (int i = 0; i < m_arrOverHeatBlobs[a].ref_intNumSelectedObject; i++)
                            {
                                m_arrOverHeatBlobs[a].DrawSelectedBlobs(g, fScaleX, fScaleY, Color.Red);
                            }
                        }
                    }

                    for (int a = 0; a < m_arrFailScratches.Length; a++)
                    {
                        if (((m_intSealFailMask & 0x800) > 0) && m_arrFailScratches[a])
                        {
                            for (int i = 0; i < m_arrScratchesBlobs[a].ref_intNumSelectedObject; i++)
                            {
                                    m_arrScratchesBlobs[a].DrawSelectedBlobs(g, fScaleX, fScaleY, Color.Red);
                            }
                        }
                    }
                }

                if (((m_intFailOptionMask & 0x2000) > 0) && m_blnWantCheckSealEdgeStraightness)
                {
                    for (int i = 0; i < m_objSealEdgeStraightnessBlobs.ref_intNumSelectedObject; i++)
                    {
                        if ((m_intSealFailMask & 0x20000) > 0)
                            m_objSealEdgeStraightnessBlobs.DrawSelectedBlobs(g, fScaleX, fScaleY, Color.Red);
                    }
                }

                // Draw Seal Broken Gap
                if ((m_intFailOptionMask & 0x20) > 0)
                {
                    for (int i = 0; i < m_arrFailSealBrokenStartPoint.Count; i++)
                    {
                        g.DrawLine(new Pen(Color.Red, 2), m_arrFailSealBrokenStartPoint[i].X * fScaleX,
                                                          m_arrFailSealBrokenStartPoint[i].Y * fScaleY,
                                                          m_arrFailSealBrokenEndPoint[i].X * fScaleX,
                                                          m_arrFailSealBrokenEndPoint[i].Y * fScaleY);
                        g.DrawLine(new Pen(Color.Red, 2), m_arrFailSealBrokenStartPoint[i].X * fScaleX,
                                                          (m_arrFailSealBrokenStartPoint[i].Y - 50) * fScaleY,
                                                          m_arrFailSealBrokenStartPoint[i].X * fScaleX,
                                                          (m_arrFailSealBrokenStartPoint[i].Y + 50) * fScaleY);
                        g.DrawLine(new Pen(Color.Red, 2), m_arrFailSealBrokenEndPoint[i].X * fScaleX,
                                                          (m_arrFailSealBrokenEndPoint[i].Y - 50) * fScaleY,
                                                          m_arrFailSealBrokenEndPoint[i].X * fScaleX,
                                                          (m_arrFailSealBrokenEndPoint[i].Y + 50) * fScaleY);
                    }
                }
            }
            catch
            {

            }
        }

        public void DrawLineWidthTolerance2(Graphics g, List<List<ROI>> arrSealROIs, float fCalibMMPerPixel, float fScaleX, float fScaleY, int i)
        {
            if (arrSealROIs.Count > (i + 1) && arrSealROIs[i + 1].Count > 0)
            {
                Pen pen1 = new Pen(Color.Cyan, 2);
                Pen pen2 = new Pen(Color.Lime, 2);


                float fStartX = arrSealROIs[i + 1][0].ref_ROIPositionX * fScaleX;
                float fStartY = 0;
                float fHeight = 0;
                if (i == 0)
                {
                    fStartY = (arrSealROIs[i + 1][0].ref_ROIPositionY + (float)arrSealROIs[i + 1][0].ref_ROIHeight / 2 -
                               (m_fWidthLowerTolerance1 / 2)) * fScaleY;
                    fHeight = (m_fWidthLowerTolerance1) * fScaleY;
                }
                else if (i == 1)
                {
                    fStartY = (arrSealROIs[i + 1][0].ref_ROIPositionY + (float)arrSealROIs[i + 1][0].ref_ROIHeight / 2 -
                               (m_fWidthLowerTolerance2 / 2)) * fScaleY;
                    fHeight = (m_fWidthLowerTolerance2) * fScaleY;
                }
                float fWidth = arrSealROIs[i + 1][0].ref_ROIWidth * fScaleX;


                g.DrawRectangle(pen1, fStartX, fStartY, fWidth, fHeight);
                if (i == 0)
                {
                    fStartY = (arrSealROIs[i + 1][0].ref_ROIPositionY + (float)arrSealROIs[i + 1][0].ref_ROIHeight / 2 -
                                (m_fWidthUpperTolerance1 / 2)) * fScaleY;

                    fHeight = (m_fWidthUpperTolerance1) * fScaleY;
                }
                else if (i == 1)
                {
                    fStartY = (arrSealROIs[i + 1][0].ref_ROIPositionY + (float)arrSealROIs[i + 1][0].ref_ROIHeight / 2 -
                                (m_fWidthUpperTolerance2 / 2)) * fScaleY;

                    fHeight = (m_fWidthUpperTolerance2) * fScaleY;
                }
                g.DrawRectangle(pen2, fStartX, fStartY, fWidth, fHeight);
            }
        }
        //public void DrawLineWidthTolerance(Graphics g, List<List<ROI>> arrSealROIs, float fCalibMMPerPixel, float fScaleX, float fScaleY, int i)
        //{
        //    Pen pen1 = new Pen(Color.Cyan, 2);
        //    Pen pen2 = new Pen(Color.Lime, 2);


        //        float fStartX = arrSealROIs[i + 1][0].ref_ROIPositionX * fScaleX;
        //        float fStartY =0;
        //        float fHeight = 0;
        //        if (i == 0)
        //        {
        //            fStartY = (arrSealROIs[i + 1][0].ref_ROIPositionY + (float)arrSealROIs[i + 1][0].ref_ROIHeight / 2 -
        //                       (m_fTemplateWidth[i] / 2 - m_fWidthLowerTolerance1 / 2)) * fScaleY;
        //            fHeight = (m_fTemplateWidth[i] - m_fWidthLowerTolerance1) * fScaleY;
        //        }
        //       else if (i == 1)
        //        {
        //            fStartY = (arrSealROIs[i + 1][0].ref_ROIPositionY + (float)arrSealROIs[i + 1][0].ref_ROIHeight / 2 -
        //                       (m_fTemplateWidth[i] / 2 - m_fWidthLowerTolerance2 / 2)) * fScaleY;
        //            fHeight = (m_fTemplateWidth[i] - m_fWidthLowerTolerance2) * fScaleY;
        //        }
        //        float fWidth = arrSealROIs[i + 1][0].ref_ROIWidth * fScaleX;


        //        g.DrawRectangle(pen1, fStartX, fStartY, fWidth, fHeight);
        //        if (i == 0)
        //        {
        //            fStartY = (arrSealROIs[i + 1][0].ref_ROIPositionY + (float)arrSealROIs[i + 1][0].ref_ROIHeight / 2 -
        //                        (m_fTemplateWidth[i] / 2 + m_fWidthUpperTolerance1 / 2)) * fScaleY;

        //            fHeight = (m_fTemplateWidth[i] + m_fWidthUpperTolerance1) * fScaleY;
        //        }
        //        else if (i == 1)
        //        {
        //            fStartY = (arrSealROIs[i + 1][0].ref_ROIPositionY + (float)arrSealROIs[i + 1][0].ref_ROIHeight / 2 -
        //                        (m_fTemplateWidth[i] / 2 + m_fWidthUpperTolerance2 / 2)) * fScaleY;

        //            fHeight = (m_fTemplateWidth[i] + m_fWidthUpperTolerance2) * fScaleY;
        //        }
        //        g.DrawRectangle(pen2, fStartX, fStartY, fWidth, fHeight);

        //}

        public void DrawMinSealObjectSize(Graphics g, List<List<ROI>> arrSealROIs, float fCalibMMPerPixel, float fScaleX, float fScaleY, int intSealSelected)
        {
            float fHeight = (float)Math.Round(m_fTemplateWidth[intSealSelected] * fScaleY, 0, MidpointRounding.AwayFromZero);
            float fWidth = (float)Math.Round((m_arrBlackBlobs[intSealSelected].ref_intMinAreaLimit * (fScaleX * fScaleY)) / fHeight, 0, MidpointRounding.AwayFromZero);
            float fStartX = (float)Math.Round((arrSealROIs[intSealSelected + 1][0].ref_ROIPositionX + (float)arrSealROIs[intSealSelected + 1][0].ref_ROIWidth / 2) * fScaleX - fWidth / 2, 0, MidpointRounding.AwayFromZero);
            float fStartY = (float)Math.Round((arrSealROIs[intSealSelected + 1][0].ref_ROIPositionY + (float)arrSealROIs[intSealSelected + 1][0].ref_ROIHeight / 2 - m_fTemplateWidth[intSealSelected] / 2) * fScaleY, 0, MidpointRounding.AwayFromZero);
            g.FillRectangle(new SolidBrush(Color.Cyan), fStartX, fStartY, fWidth, fHeight);
        }

        public void DrawMinOverHeatSize(Graphics g, List<List<ROI>> arrSealROIs, float fCalibMMPerPixel, float fScaleX, float fScaleY, int intROIIndex)
        {
            if (arrSealROIs.Count > 4 && arrSealROIs[4].Count > intROIIndex)
            {
                Pen pen1 = new Pen(Color.Lime);
                float fWidth = (float)Math.Round((arrSealROIs[4][intROIIndex].ref_ROIWidth - 20) * fScaleX, 0, MidpointRounding.AwayFromZero);

                if (Math.Round(m_arrOverHeatBlobs[intROIIndex].ref_intMinAreaLimit * (fScaleX * fScaleY), 0, MidpointRounding.AwayFromZero) < fWidth)
                    fWidth = (float)Math.Round(m_arrOverHeatBlobs[intROIIndex].ref_intMinAreaLimit * (fScaleX * fScaleY), 0, MidpointRounding.AwayFromZero);

                float fHeight = (float)Math.Round(m_arrOverHeatBlobs[intROIIndex].ref_intMinAreaLimit * (fScaleX * fScaleY) / fWidth, 0, MidpointRounding.AwayFromZero);

                fHeight = fWidth = (float)Math.Ceiling(Math.Sqrt((float)m_arrOverHeatBlobs[intROIIndex].ref_intMinAreaLimit));

                float fStartX = (float)Math.Round((arrSealROIs[4][intROIIndex].ref_ROIPositionX + (float)arrSealROIs[4][intROIIndex].ref_ROIWidth / 2 - fWidth / 2) * fScaleX , 0, MidpointRounding.AwayFromZero);
                float fStartY = (float)Math.Round((arrSealROIs[4][intROIIndex].ref_ROIPositionY + (float)arrSealROIs[4][intROIIndex].ref_ROIHeight / 2 - fHeight / 2) * fScaleY , 0, MidpointRounding.AwayFromZero);
                g.FillRectangle(new SolidBrush(Color.Cyan), fStartX, fStartY, fWidth * fScaleX, fHeight * fScaleY);
            }
        }
        public void DrawMaxSprocketHoleDefectSize(Graphics g, List<List<ROI>> arrSealROIs, float fCalibMMPerPixel, float fScaleX, float fScaleY)
        {
            if (arrSealROIs.Count > 6 && arrSealROIs[6].Count > 0)
            {
                Pen pen1 = new Pen(Color.Lime);
                //float fWidth = (float)Math.Round((arrSealROIs[6][0].ref_ROIWidth - 20) * fScaleX, 0, MidpointRounding.AwayFromZero);

                //if (Math.Round(m_fSprocketHoleDefectMaxTolerance * (fScaleX * fScaleY), 0, MidpointRounding.AwayFromZero) < fWidth)
                //    fWidth = (float)Math.Round(m_fSprocketHoleDefectMaxTolerance * (fScaleX * fScaleY), 0, MidpointRounding.AwayFromZero);

                //float fHeight = (float)Math.Round(m_fSprocketHoleDefectMaxTolerance * (fScaleX * fScaleY) / fWidth, 0, MidpointRounding.AwayFromZero);
                float fHeight, fWidth;
                fHeight = fWidth = (float)Math.Ceiling(Math.Sqrt(m_fSprocketHoleDefectMaxTolerance));

                float fStartX = (float)Math.Round((arrSealROIs[6][0].ref_ROIPositionX + (float)arrSealROIs[6][0].ref_ROIWidth / 2 - fWidth / 2) * fScaleX , 0, MidpointRounding.AwayFromZero);
                float fStartY = (float)Math.Round((arrSealROIs[6][0].ref_ROIPositionY + (float)arrSealROIs[6][0].ref_ROIHeight / 2 - fHeight / 2) * fScaleY , 0, MidpointRounding.AwayFromZero);
                g.FillRectangle(new SolidBrush(Color.Cyan), fStartX, fStartY, fWidth * (fScaleX), fHeight * (fScaleY));
            }
        }
        public void DrawMaxSprocketHoleBrokenSize(Graphics g, List<List<ROI>> arrSealROIs, float fCalibMMPerPixel, float fScaleX, float fScaleY)
        {
            if (arrSealROIs.Count > 6 && arrSealROIs[6].Count > 0)
            {
                Pen pen1 = new Pen(Color.Lime);
                //float fWidth = (float)Math.Round((arrSealROIs[6][0].ref_ROIWidth - 20) * fScaleX, 0, MidpointRounding.AwayFromZero);

                //if (Math.Round(m_fSprocketHoleDefectMaxTolerance * (fScaleX * fScaleY), 0, MidpointRounding.AwayFromZero) < fWidth)
                //    fWidth = (float)Math.Round(m_fSprocketHoleDefectMaxTolerance * (fScaleX * fScaleY), 0, MidpointRounding.AwayFromZero);

                //float fHeight = (float)Math.Round(m_fSprocketHoleDefectMaxTolerance * (fScaleX * fScaleY) / fWidth, 0, MidpointRounding.AwayFromZero);
                float fHeight, fWidth;
                fHeight = fWidth = (float)Math.Ceiling(Math.Sqrt(m_fSprocketHoleBrokenMaxTolerance));

                float fStartX = (float)Math.Round((arrSealROIs[6][0].ref_ROIPositionX + (float)arrSealROIs[6][0].ref_ROIWidth / 2 - fWidth / 2) * fScaleX, 0, MidpointRounding.AwayFromZero);
                float fStartY = (float)Math.Round((arrSealROIs[6][0].ref_ROIPositionY + (float)arrSealROIs[6][0].ref_ROIHeight / 2 - fHeight / 2) * fScaleY, 0, MidpointRounding.AwayFromZero);
                g.FillRectangle(new SolidBrush(Color.Cyan), fStartX, fStartY, fWidth * (fScaleX), fHeight * (fScaleY));
            }
        }
        public void DrawMaxSealEdgeStraightnessSize(Graphics g, List<List<ROI>> arrSealROIs, float fCalibMMPerPixel, float fScaleX, float fScaleY)
        {
            if (arrSealROIs.Count > 7 && arrSealROIs[7].Count > 0)
            {
                float fHeight, fWidth;
                fHeight = fWidth = (float)Math.Ceiling(Math.Sqrt(m_fSealEdgeStraightnessMaxTolerance));

                float fStartX = (float)Math.Round((arrSealROIs[7][0].ref_ROIPositionX + (float)arrSealROIs[7][0].ref_ROIWidth / 2 - fWidth / 2) * fScaleX, 0, MidpointRounding.AwayFromZero);
                float fStartY = (float)Math.Round((arrSealROIs[7][0].ref_ROIPositionY + (float)arrSealROIs[7][0].ref_ROIHeight / 2 - fHeight / 2) * fScaleY, 0, MidpointRounding.AwayFromZero);
                g.FillRectangle(new SolidBrush(Color.Cyan), fStartX, fStartY, fWidth * (fScaleX), fHeight * (fScaleY));
            }
        }
        public void DrawMinTapeScratchesSize(Graphics g, List<List<ROI>> arrSealROIs, float fCalibMMPerPixel, float fScaleX, float fScaleY, int intROIIndex)
        {
            if (arrSealROIs.Count > 4 && arrSealROIs[4].Count > intROIIndex)
            {
                Pen pen1 = new Pen(Color.Lime);
                float fWidth = (float)Math.Round((arrSealROIs[4][intROIIndex].ref_ROIWidth - 20) * fScaleX, 0, MidpointRounding.AwayFromZero);

                if (Math.Round(m_arrScratchesBlobs[intROIIndex].ref_intMinAreaLimit * (fScaleX * fScaleY), 0, MidpointRounding.AwayFromZero) < fWidth)
                    fWidth = (float)Math.Round(m_arrScratchesBlobs[intROIIndex].ref_intMinAreaLimit * (fScaleX * fScaleY), 0, MidpointRounding.AwayFromZero);

                float fHeight = (float)Math.Round(m_arrScratchesBlobs[intROIIndex].ref_intMinAreaLimit * (fScaleX * fScaleY) / fWidth, 0, MidpointRounding.AwayFromZero);

                fHeight = fWidth = (float)Math.Ceiling(Math.Sqrt((float)m_arrScratchesBlobs[intROIIndex].ref_intMinAreaLimit));

                float fStartX = (float)Math.Round((arrSealROIs[4][intROIIndex].ref_ROIPositionX + (float)arrSealROIs[4][intROIIndex].ref_ROIWidth / 2 - fWidth / 2) * fScaleX , 0, MidpointRounding.AwayFromZero);
                float fStartY = (float)Math.Round((arrSealROIs[4][intROIIndex].ref_ROIPositionY + (float)arrSealROIs[4][intROIIndex].ref_ROIHeight / 2 - fHeight / 2) * fScaleY , 0, MidpointRounding.AwayFromZero);
                g.FillRectangle(new SolidBrush(Color.Cyan), fStartX, fStartY, fWidth * (fScaleX), fHeight * (fScaleY));
            }
        }

        public void DrawMinSealBubbleSize(Graphics g, List<List<ROI>> arrSealROIs, float fCalibMMPerPixel, float fScaleX, float fScaleY, int i)
        {
            if (arrSealROIs.Count > (i + 1) && arrSealROIs[i + 1].Count > 0)
            {
                float fHeight = (float)Math.Round((m_fTemplateWidth[i] / 2) , 0, MidpointRounding.AwayFromZero);
                float fWidth = arrSealROIs[i + 1][0].ref_ROIWidth - 20;
                if (i == 0)
                {
                    if ((float)Math.Round((m_intHoleMinArea1 * (fScaleX * fScaleY)) / fHeight, 0, MidpointRounding.AwayFromZero) < fWidth)
                        fWidth = (float)Math.Round((m_intHoleMinArea1 * (fScaleX * fScaleY)) / fHeight, 0, MidpointRounding.AwayFromZero);
                }
                else if (i == 1)
                {
                    if ((float)Math.Round((m_intHoleMinArea2 * (fScaleX * fScaleY)) / fHeight, 0, MidpointRounding.AwayFromZero) < fWidth)
                        fWidth = (float)Math.Round((m_intHoleMinArea2 * (fScaleX * fScaleY)) / fHeight, 0, MidpointRounding.AwayFromZero);
                }
                float fStartX = (float)Math.Round((arrSealROIs[i + 1][0].ref_ROIPositionX + (float)arrSealROIs[i + 1][0].ref_ROIWidth / 2 - fWidth / 2) * fScaleX , 0, MidpointRounding.AwayFromZero);
                float fStartY = (float)Math.Round((arrSealROIs[i + 1][0].ref_ROIPositionY + (float)arrSealROIs[i + 1][0].ref_ROIHeight / 2 - m_fTemplateWidth[i] / 4) * fScaleY, 0, MidpointRounding.AwayFromZero);
                g.FillRectangle(new SolidBrush(Color.Cyan), fStartX, fStartY, fWidth * fScaleX, fHeight * fScaleY);
            }
        }

        public void DrawDistanceWidthTolerance(Graphics g, List<List<ROI>> arrSealROIs, float fCalibMMPerPixel, float fScaleX, float fScaleY)
        {
            Pen pen1 = new Pen(Color.Cyan, 2);
            Pen pen2 = new Pen(Color.Lime, 2);

            if (arrSealROIs.Count > 3 && arrSealROIs[3].Count > 0)
            {
                float fStartX = arrSealROIs[3][0].ref_ROIPositionX * fScaleX;

                float fStartY = (arrSealROIs[3][0].ref_ROIPositionY + (float)arrSealROIs[3][0].ref_ROIHeight / 2 -
                            (m_fDistanceMinTolerance / 2)) * fScaleY;

                float fWidth = arrSealROIs[3][0].ref_ROIWidth * fScaleX;
                float fHeight = (m_fDistanceMinTolerance) * fScaleY;

                g.DrawRectangle(pen1, fStartX, fStartY, fWidth, fHeight);

                fStartY = (arrSealROIs[3][0].ref_ROIPositionY + (float)arrSealROIs[3][0].ref_ROIHeight / 2 -
                            (m_fDistanceMaxTolerance / 2)) * fScaleY;

                fHeight = (m_fDistanceMaxTolerance) * fScaleY;

                g.DrawRectangle(pen2, fStartX, fStartY, fWidth, fHeight);
            }
        }
        public void DrawSprocketHoleDistanceWidthTolerance(Graphics g, CirGauge objCircleGauge, float fCalibMMPerPixel, float fScaleX, float fScaleY)
        {
            Pen pen1 = new Pen(Color.Cyan, 2);
            Pen pen2 = new Pen(Color.Blue, 2);

            float fStartX = objCircleGauge.ref_ObjectCenterX * fScaleX;

            float fStartY;
            if (m_intSpocketHolePosition == 0)
                fStartY = (objCircleGauge.ref_ObjectCenterY + objCircleGauge.ref_fDiameter / 2) * fScaleY;
            else
                fStartY = (objCircleGauge.ref_ObjectCenterY - objCircleGauge.ref_fDiameter / 2) * fScaleY;

            float fEndX = objCircleGauge.ref_ObjectCenterX * fScaleX;
            float fEndY;

            if (m_intSpocketHolePosition == 0)
                fEndY = ((objCircleGauge.ref_ObjectCenterY + objCircleGauge.ref_fDiameter / 2) + m_fSprocketHoleDistanceMaxTolerance) * fScaleY;
            else
                fEndY = ((objCircleGauge.ref_ObjectCenterY - objCircleGauge.ref_fDiameter / 2) - m_fSprocketHoleDistanceMaxTolerance) * fScaleY;

            if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
            {
                g.DrawLine(pen2, fStartX, fStartY, fEndX, fEndY);
            }

            if (m_intSpocketHolePosition == 0)
                fEndY = ((objCircleGauge.ref_ObjectCenterY + objCircleGauge.ref_fDiameter / 2) + (m_fSprocketHoleDistanceMinTolerance)) * fScaleY;
            else
                fEndY = ((objCircleGauge.ref_ObjectCenterY - objCircleGauge.ref_fDiameter / 2) - (m_fSprocketHoleDistanceMinTolerance)) * fScaleY;

            if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
            {
                g.DrawLine(pen1, fStartX, fStartY, fEndX, fEndY);
            }
        }
        public void DrawSprocketHoleDiameterTolerance(Graphics g, CirGauge objCircleGauge, float fCalibMMPerPixel, float fScaleX, float fScaleY)
        {
            Pen pen1 = new Pen(Color.Cyan, 1);
            Pen pen2 = new Pen(Color.Blue, 1);

            float fStartX = (objCircleGauge.ref_ObjectCenterX - (Math.Abs(m_fSprocketHoleDiameterMinTolerance)) / 2) * fScaleX;

            float fStartY = (objCircleGauge.ref_ObjectCenterY - (Math.Abs(m_fSprocketHoleDiameterMinTolerance)) / 2) * fScaleY;

            //float fEndX = objCircleGauge.ref_ObjectCenterX * fScaleX;
            //float fEndY = ((objCircleGauge.ref_ObjectCenterY + objCircleGauge.ref_fDiameter / 2) - (Math.Abs(m_fSprocketHoleDiameterMinTolerance))) * fScaleY;

            if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
            {
                g.DrawEllipse(pen1, fStartX, fStartY, m_fSprocketHoleDiameterMinTolerance * fScaleX, m_fSprocketHoleDiameterMinTolerance * fScaleY);
            }

            fStartX = (objCircleGauge.ref_ObjectCenterX - (Math.Abs(m_fSprocketHoleDiameterMaxTolerance)) / 2) * fScaleX;

            fStartY = (objCircleGauge.ref_ObjectCenterY - (Math.Abs(m_fSprocketHoleDiameterMaxTolerance)) / 2) * fScaleY;

            //fEndY = ((objCircleGauge.ref_ObjectCenterY + objCircleGauge.ref_fDiameter / 2) + m_fSprocketHoleDiameterMaxTolerance) * fScaleY;

            if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
            {
                g.DrawEllipse(pen2, fStartX, fStartY, m_fSprocketHoleDiameterMaxTolerance * fScaleX, m_fSprocketHoleDiameterMaxTolerance * fScaleY);
            }
        }
        public void DrawSprocketHoleRoundnessTolerance(Graphics g, CirGauge objCircleGauge, float fCalibMMPerPixel, float fScaleX, float fScaleY)
        {
            Pen pen1 = new Pen(Color.Cyan, 1);
            Pen pen2 = new Pen(Color.Blue, 1);

            float fWidth = Math.Max((float)Math.Sqrt((double)(Math.Pow(objCircleGauge.ref_fDiameter, 2) - Math.Pow(objCircleGauge.ref_fDiameter * m_fSprocketHoleRoundnessMaxTolerance, 2))), 1);

            float fStartX = (objCircleGauge.ref_ObjectCenterX - (Math.Abs(fWidth)) / 2) * fScaleX;

            float fStartY = (objCircleGauge.ref_ObjectCenterY - (Math.Abs(objCircleGauge.ref_fDiameter)) / 2) * fScaleY;

            if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
            {
                g.DrawEllipse(pen1, fStartX, fStartY, fWidth * fScaleX, objCircleGauge.ref_fDiameter * fScaleY);
            }
        }
        public void DrawSprocketHoleInspectionAreaInwardTolerance(Graphics g, CirGauge objCircleGauge, float fCalibMMPerPixel, float fScaleX, float fScaleY, Color[] arrColor)
        {
            Pen pen1 = new Pen(arrColor[2], 1);
            Pen pen2 = new Pen(arrColor[1], 1);

            float fStartX = (objCircleGauge.ref_ObjectCenterX - ((Math.Max(objCircleGauge.ref_fDiameter - m_intSprocketHoleInspectionAreaInwardTolerance, 5)) / 2)) * fScaleX;

            float fStartY = (objCircleGauge.ref_ObjectCenterY - ((Math.Max(objCircleGauge.ref_fDiameter - m_intSprocketHoleInspectionAreaInwardTolerance, 5)) / 2)) * fScaleY;

            //float fEndX = objCircleGauge.ref_ObjectCenterX * fScaleX;
            //float fEndY = ((objCircleGauge.ref_ObjectCenterY + objCircleGauge.ref_fDiameter / 2) - (Math.Abs(m_fSprocketHoleDiameterMinTolerance))) * fScaleY;

            g.DrawEllipse(pen1, fStartX, fStartY, (Math.Max(objCircleGauge.ref_fDiameter - m_intSprocketHoleInspectionAreaInwardTolerance, 5)) * fScaleX, (Math.Max(objCircleGauge.ref_fDiameter - m_intSprocketHoleInspectionAreaInwardTolerance, 5)) * fScaleY);

            fStartX = (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2)) * fScaleX;

            fStartY = (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2)) * fScaleY;

            //fEndY = ((objCircleGauge.ref_ObjectCenterY + objCircleGauge.ref_fDiameter / 2) + m_fSprocketHoleDiameterMaxTolerance) * fScaleY;

            if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
            {
                g.DrawEllipse(pen2, fStartX, fStartY, objCircleGauge.ref_fDiameter * fScaleX, objCircleGauge.ref_fDiameter * fScaleY);
            }
        }
        public void DrawSprocketHoleBrokenOutwardTolerance(Graphics g, CirGauge objCircleGauge, float fCalibMMPerPixel, float fScaleX, float fScaleY, bool blnDrawInner, Color[] arrColor)
        {
            Pen pen1 = new Pen(arrColor[3], 1);
            Pen pen2 = new Pen(arrColor[4], 1);

            float fStartX = (objCircleGauge.ref_ObjectCenterX - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)) * fScaleX;

            float fStartY = (objCircleGauge.ref_ObjectCenterY - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) / 2)) * fScaleY;

            if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
            {
                g.DrawEllipse(pen1, fStartX, fStartY, (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) * fScaleX, (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Outer) * fScaleY);
            }

            if (blnDrawInner)
            {
                fStartX = (objCircleGauge.ref_ObjectCenterX - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)) * fScaleX;

                fStartY = (objCircleGauge.ref_ObjectCenterY - ((objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) / 2)) * fScaleY;

                if ((objCircleGauge.ref_ObjectCenterX >= (objCircleGauge.ref_ObjectCenterX - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterX <= (objCircleGauge.ref_ObjectCenterX + (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY >= (objCircleGauge.ref_ObjectCenterY - (objCircleGauge.ref_fDiameter / 2))) ||
                    (objCircleGauge.ref_ObjectCenterY <= (objCircleGauge.ref_ObjectCenterY + (objCircleGauge.ref_fDiameter / 2))))
                {
                    g.DrawEllipse(pen2, fStartX, fStartY, (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) * fScaleX, (objCircleGauge.ref_fDiameter + m_intSprocketHoleBrokenOutwardTolerance_Inner) * fScaleY);
                }
            }
        }
        public bool GetSealLineAngle(ROI objDistanceROI, ref float fSealLineAngle)
        {
            LGauge objLineGauge = new LGauge();
            ROI objGaugeROI1 = new ROI();
            objGaugeROI1.LoadROISetting(objDistanceROI.ref_ROITotalX, objDistanceROI.ref_ROITotalY,
                                        objDistanceROI.ref_ROIWidth, objDistanceROI.ref_ROIHeight);
            objLineGauge.SetGaugePlacement(objGaugeROI1);
            objLineGauge.SetGaugeAdvancedSetting(10, 0, 5, 20, 0, 0, 0, 20);

            objLineGauge.Measure(objDistanceROI);

            if (objLineGauge.IsImageValid())
            {
                fSealLineAngle = objLineGauge.ref_ObjectAngle;
                return true;
            }
            else
            {
                m_strErrorMessage = "*Fail to measure seal line angle.";
                return false;
            }
        }
        /// <summary>
        /// Get each seal's line width
        /// </summary>
        /// <param name="arrROIs"></param>
        public void SetSealAreaTemplate(List<List<ROI>> arrROIs)
        {
            for (int i = 0; i < 2; i++)
            {
                if (m_arrBlackBlobs[i] == null)
                    m_arrBlackBlobs[i] = new EBlobs();
                else
                    m_arrBlackBlobs[i].CleanAllBlobs();

                //m_arrBlackBlobs[i].BuildObjects_Filter_GetElement(arrROIs[i + 1][0], true, true, 0, m_arrBlackBlobs[i].ref_intAbsoluteThreshold, m_arrBlackBlobs[i].ref_intMinAreaLimit, m_arrBlackBlobs[i].ref_intMaxAreaLimit, false, 0x01);
                m_arrBlackBlobs[i].BuildObjects_Filter_GetElement(arrROIs[i + 1][0], true, true, 0, m_intSealLineEdgeThreshold[i], 1/*m_arrBlackBlobs[i].ref_intMinAreaLimit*/, m_arrBlackBlobs[i].ref_intMaxAreaLimit, false, 0x01);

                if (m_arrBlackBlobs[i].ref_intNumSelectedObject > 0)
                {
                    m_intTemplateAreaAVG[i] = m_arrBlackBlobs[i].ref_arrArea[0];
                }
                else
                    m_intTemplateAreaAVG[i] = 0;


                //int intNumObject = m_arrBlackBlobs[i].BuildObjects(arrROIs[i + 1][0]);

                //if (intNumObject > 0)
                //{
                //    m_arrBlackBlobs[i].SortObjects(1, false);
                //    m_intTemplateAreaAVG[i] = m_arrBlackBlobs[i].GetSelectedObjectArea(0);
                //}
                //else
                //    m_intTemplateAreaAVG[i] = 0;
            }
        }

        public void ClearBlobData()
        {
            //m_objOverHeatBlobs.ResetBlobs();
            //m_objDistanceBlobs.ResetBlobs();
            //for (int i = 0; i < m_arrBlackBlobs.Count; i++)
            //{
            //    m_arrBlackBlobs[i].ResetBlobs();
            //}
        }

        public void SaveSeal(string strPath, bool blnNewFile, string strSectionName, bool blnNewSection, float fCalibPixelPerMM)
        {
            XmlParser objFile = new XmlParser(strPath, blnNewFile);

            objFile.WriteSectionElement(strSectionName, blnNewSection);
            objFile.WriteElement1Value("SpocketHolePosition", m_intSpocketHolePosition);
            objFile.WriteElement1Value("BuildObjectLength", m_intBuildObjectLength);
            //objFile.WriteElement1Value("Seal1Threshold", m_arrBlackBlobs[0].ref_intAbsoluteThreshold);
            //objFile.WriteElement1Value("Seal2Threshold", m_arrBlackBlobs[1].ref_intAbsoluteThreshold);
            objFile.WriteElement1Value("Seal1Threshold", m_intSealLineEdgeThreshold[0]);
            objFile.WriteElement1Value("Seal2Threshold", m_intSealLineEdgeThreshold[1]);
            objFile.WriteElement1Value("Seal1BrokenAreaThreshold", m_intSealBrokenAreaThreshold[0]);
            objFile.WriteElement1Value("Seal2BrokenAreaThreshold", m_intSealBrokenAreaThreshold[1]);
            for (int i = 0; i < m_arrOverHeatBlobs.Length; i++)
            {
                string strIndex = "";
                if (i > 0)
                    strIndex = i.ToString();
                objFile.WriteElement1Value("OverHeatThreshold" + strIndex, m_arrOverHeatBlobs[i].ref_intAbsoluteThreshold);
                objFile.WriteElement1Value("OverHeatLowThreshold" + strIndex, m_arrOverHeatBlobs[i].ref_intAbsoluteLowThreshold);
                objFile.WriteElement1Value("OverHeatHighThreshold" + strIndex, m_arrOverHeatBlobs[i].ref_intAbsoluteHighThreshold);
            }
            objFile.WriteElement1Value("DistanceThreshold", m_objDistanceBlobs.ref_intAbsoluteThreshold);
            objFile.WriteElement1Value("SealEdgeStraightnessThreshold", m_objSealEdgeStraightnessBlobs.ref_intAbsoluteThreshold);
            objFile.WriteElement1Value("SprocketHoleDefectThreshold", m_objSprocketHoleDefectBlobs.ref_intAbsoluteThreshold);
            objFile.WriteElement1Value("SprocketHoleBrokenThreshold", m_objSprocketHoleBrokenBlobs.ref_intAbsoluteThreshold);
            objFile.WriteElement1Value("SprocketHoleRoundnessThreshold", m_objSprocketHoleRoundnessBlobs.ref_intAbsoluteThreshold);
            STTrackLog.WriteLine("SealBlog > Save ref_intAbsoluteThreshold = " + m_objDistanceBlobs.ref_intAbsoluteThreshold);
            objFile.WriteElement1Value("FailBrokenSeal1", m_minSealBrokenGap / (fCalibPixelPerMM));
            objFile.WriteElement1Value("FailBrokenSeal2", m_minSealBrokenGap2 / (fCalibPixelPerMM));
            objFile.WriteElement1Value("Seal1MinArea", m_arrBlackBlobs[0].ref_intMinAreaLimit / (fCalibPixelPerMM * fCalibPixelPerMM));
            objFile.WriteElement1Value("Seal2MinArea", m_arrBlackBlobs[1].ref_intMinAreaLimit / (fCalibPixelPerMM * fCalibPixelPerMM));
            for (int i = 0; i < m_arrOverHeatAreaMinTolerance.Length; i++)
            {
                string strIndex = "";
                if (i > 0)
                    strIndex = i.ToString();
                objFile.WriteElement1Value("OverHeatMinArea" + strIndex, m_arrOverHeatAreaMinTolerance[i]);
            }
            objFile.WriteElement1Value("SprocketHoleDefectMaxTolerance", m_fSprocketHoleDefectMaxTolerance); 
            objFile.WriteElement1Value("SprocketHoleBrokenMaxTolerance", m_fSprocketHoleBrokenMaxTolerance); 
            objFile.WriteElement1Value("SprocketHoleRoundnessMaxTolerance", m_fSprocketHoleRoundnessMaxTolerance);
            objFile.WriteElement1Value("SealEdgeStraightnessMaxTolerance", m_fSealEdgeStraightnessMaxTolerance);
            for (int i = 0; i < m_arrScratchesAreaMinTolerance.Length; i++)
            {
                string strIndex = "";
                if (i > 0)
                    strIndex = i.ToString();
                objFile.WriteElement1Value("ScratchesAreaMinTolerance" + strIndex, m_arrScratchesAreaMinTolerance[i]);
            }
            objFile.WriteElement1Value("SealHoleMinArea1", m_intHoleMinArea1 / (fCalibPixelPerMM * fCalibPixelPerMM));
            objFile.WriteElement1Value("SealHoleMinArea2", m_intHoleMinArea2 / (fCalibPixelPerMM * fCalibPixelPerMM));
            objFile.WriteElement1Value("MinBrokenWidth", m_fMinBrokenWidth / (fCalibPixelPerMM));
            objFile.WriteElement1Value("PositionCenterX", m_pPositionCenterPoint.X / (fCalibPixelPerMM));
            objFile.WriteElement1Value("PositionCenterY", m_pPositionCenterPoint.Y / (fCalibPixelPerMM));
            objFile.WriteElement1Value("WidthLowerTolerance", m_fWidthLowerTolerance / fCalibPixelPerMM);
            objFile.WriteElement1Value("WidthLowerTolerance1", m_fWidthLowerTolerance1 / fCalibPixelPerMM);
            objFile.WriteElement1Value("WidthLowerTolerance2", m_fWidthLowerTolerance2 / fCalibPixelPerMM);
            objFile.WriteElement1Value("WidthUpperTolerance", m_fWidthUpperTolerance / fCalibPixelPerMM);
            objFile.WriteElement1Value("WidthUpperTolerance1", m_fWidthUpperTolerance1 / fCalibPixelPerMM);
            objFile.WriteElement1Value("WidthUpperTolerance2", m_fWidthUpperTolerance2 / fCalibPixelPerMM);
            objFile.WriteElement1Value("DistanceMinTolerance", m_fDistanceMinTolerance / fCalibPixelPerMM);
            objFile.WriteElement1Value("DistanceMaxTolerance", m_fDistanceMaxTolerance / fCalibPixelPerMM);
            objFile.WriteElement1Value("SprocketHoleDistanceMinTolerance", m_fSprocketHoleDistanceMinTolerance / fCalibPixelPerMM);
            objFile.WriteElement1Value("SprocketHoleDistanceMaxTolerance", m_fSprocketHoleDistanceMaxTolerance / fCalibPixelPerMM);
            objFile.WriteElement1Value("SprocketHoleDiameterMinTolerance", m_fSprocketHoleDiameterMinTolerance / fCalibPixelPerMM);
            objFile.WriteElement1Value("SprocketHoleDiameterMaxTolerance", m_fSprocketHoleDiameterMaxTolerance / fCalibPixelPerMM);
            objFile.WriteElement1Value("ShiftPositionTolerance", m_fShiftPositionTolerance / fCalibPixelPerMM);
            objFile.WriteElement1Value("SealScoreTolerance", m_fSealScoreTolerance);
            objFile.WriteElement1Value("FailMask", m_intFailOptionMask);
            objFile.WriteElement1Value("SealEdgeSensitivity", m_intSealEdgeSensitivity);
            objFile.WriteElement1Value("SealEdgeTolerance", m_intSealEdgeTolerance);
            objFile.WriteElement1Value("SprocketHoleInspectionAreaInwardTolerance", m_intSprocketHoleInspectionAreaInwardTolerance);
            objFile.WriteElement1Value("SprocketHoleBrokenOutwardTolerance_Outer", m_intSprocketHoleBrokenOutwardTolerance_Outer);
            objFile.WriteElement1Value("SprocketHoleBrokenOutwardTolerance_Inner", m_intSprocketHoleBrokenOutwardTolerance_Inner);
            objFile.WriteElement1Value("TapePocketPitchByImage", m_fTapePocketPitchByImage);
            //objFile.WriteElement1Value("WantUsePatternCheckUnitPresent", m_blnWantUsePatternCheckUnitPresent);
            //objFile.WriteElement1Value("WantUsePixelCheckUnitPresent", m_blnWantUsePixelCheckUnitPresent);
            for (int i = 0; i < 2; i++)
            {
                objFile.WriteElement1Value("TemplateSealArea" + i, m_intTemplateAreaAVG[i] / (fCalibPixelPerMM * fCalibPixelPerMM));
                objFile.WriteElement1Value("SealAreaTolerance" + i, m_arrSealAreaTolerance[i] / (fCalibPixelPerMM * fCalibPixelPerMM));
            }
            for (int i = 0; i < 3; i++)
                objFile.WriteElement1Value("TemplateSealLineWidth" + i, m_fTemplateWidth[i] / fCalibPixelPerMM);

            objFile.WriteElement1Value("PocketMinScore", m_fPocketMinScore);
            objFile.WriteElement1Value("MarkMinScore", m_fMarkMinScore);
            objFile.WriteElement1Value("MarkMinWhiteArea", m_fMarkMinWhiteArea);
            objFile.WriteElement1Value("MarkMaxWhiteArea", m_fMarkMaxWhiteArea);
            objFile.WriteElement1Value("MarkPixelThreshold", m_intMarkPixelThreshold);
            objFile.WriteElement1Value("StdDevTol", m_intStdDevTol);

            // Grab image index
            objFile.WriteElement1Value("GrabImageIndexCount", m_arrGrabImageIndex.Count);
            for (int j = 0; j < m_arrGrabImageIndex.Count; j++)
                objFile.WriteElement1Value("GrabImageIndex" + j.ToString(), m_arrGrabImageIndex[j]);

            objFile.WriteElement1Value("TemplateMarkROIPositionCount", m_arrTemplateMarkROIPosition.Count);
            for (int i = 0; i < m_arrTemplateMarkROIPosition.Count; i++)
            {
                objFile.WriteElement1Value("TemplateMarkROIPositionX" + i, m_arrTemplateMarkROIPosition[i].X);
                objFile.WriteElement1Value("TemplateMarkROIPositionY" + i, m_arrTemplateMarkROIPosition[i].Y);
            }

            objFile.WriteElement1Value("TemplateMarkROISizeCount", m_arrTemplateMarkROISize.Count);
            for (int i = 0; i < m_arrTemplateMarkROISize.Count; i++)
            {
                objFile.WriteElement1Value("TemplateMarkROISizeWidth" + i, m_arrTemplateMarkROISize[i].Width);
                objFile.WriteElement1Value("TemplateMarkROISizeHeight" + i, m_arrTemplateMarkROISize[i].Height);
            }

            objFile.WriteElement1Value("TemplateMarkThresholdCount", m_arrTemplateMarkThreshold.Count);
            for (int i = 0; i < m_arrTemplateMarkThreshold.Count; i++)
            {
                objFile.WriteElement1Value("TemplateMarkThreshold" + i, m_arrTemplateMarkThreshold[i]);
            }

            objFile.WriteElement1Value("TemplateMarkThresholdRelativeCount", m_arrTemplateMarkThresholdRelative.Count);
            for (int i = 0; i < m_arrTemplateMarkThresholdRelative.Count; i++)
            {
                objFile.WriteElement1Value("TemplateMarkThresholdRelative" + i, m_arrTemplateMarkThresholdRelative[i]);
            }

            objFile.WriteElement1Value("TemplateWantAutoThresholdRelative", m_arrTemplateWantAutoThresholdRelative.Count);
            for (int i = 0; i < m_arrTemplateWantAutoThresholdRelative.Count; i++)
            {
                objFile.WriteElement1Value("TemplateWantAutoThresholdRelative" + i, m_arrTemplateWantAutoThresholdRelative[i]);
            }

            objFile.WriteElement1Value("TemplateErodeValueCount", m_arrTemplateErodeValue.Count);
            for (int i = 0; i < m_arrTemplateErodeValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateErodeValue" + i, m_arrTemplateErodeValue[i]);
            }

            objFile.WriteElement1Value("TemplateDilateValueCount", m_arrTemplateDilateValue.Count);
            for (int i = 0; i < m_arrTemplateDilateValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateDilateValue" + i, m_arrTemplateDilateValue[i]);
            }

            objFile.WriteElement1Value("TemplateOpenValueCount", m_arrTemplateOpenValue.Count);
            for (int i = 0; i < m_arrTemplateOpenValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateOpenValue" + i, m_arrTemplateOpenValue[i]);
            }

            objFile.WriteElement1Value("TemplateCloseValueCount", m_arrTemplateCloseValue.Count);
            for (int i = 0; i < m_arrTemplateCloseValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateCloseValue" + i, m_arrTemplateCloseValue[i]);
            }

            objFile.WriteElement1Value("TemplateErodeMinValueCount", m_arrTemplateErodeMinValue.Count);
            for (int i = 0; i < m_arrTemplateErodeMinValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateErodeMinValue" + i, m_arrTemplateErodeMinValue[i]);
            }

            objFile.WriteElement1Value("TemplateDilateMinValueCount", m_arrTemplateDilateMinValue.Count);
            for (int i = 0; i < m_arrTemplateDilateMinValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateDilateMinValue" + i, m_arrTemplateDilateMinValue[i]);
            }

            objFile.WriteElement1Value("TemplateOpenMinValueCount", m_arrTemplateOpenMinValue.Count);
            for (int i = 0; i < m_arrTemplateOpenMinValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateOpenMinValue" + i, m_arrTemplateOpenMinValue[i]);
            }

            objFile.WriteElement1Value("TemplateCloseMinValueCount", m_arrTemplateCloseMinValue.Count);
            for (int i = 0; i < m_arrTemplateCloseMinValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateCloseMinValue" + i, m_arrTemplateCloseMinValue[i]);
            }

            objFile.WriteElement1Value("TemplateThresholdMinValueCount", m_arrTemplateThresholdMinValue.Count);
            for (int i = 0; i < m_arrTemplateThresholdMinValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateThresholdMinValue" + i, m_arrTemplateThresholdMinValue[i]);
            }

            objFile.WriteElement1Value("TemplateErodeMaxValueCount", m_arrTemplateErodeMaxValue.Count);
            for (int i = 0; i < m_arrTemplateErodeMaxValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateErodeMaxValue" + i, m_arrTemplateErodeMaxValue[i]);
            }

            objFile.WriteElement1Value("TemplateDilateMaxValueCount", m_arrTemplateDilateMaxValue.Count);
            for (int i = 0; i < m_arrTemplateDilateMaxValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateDilateMaxValue" + i, m_arrTemplateDilateMaxValue[i]);
            }

            objFile.WriteElement1Value("TemplateOpenMaxValueCount", m_arrTemplateOpenMaxValue.Count);
            for (int i = 0; i < m_arrTemplateOpenMaxValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateOpenMaxValue" + i, m_arrTemplateOpenMaxValue[i]);
            }

            objFile.WriteElement1Value("TemplateCloseMaxValueCount", m_arrTemplateCloseMaxValue.Count);
            for (int i = 0; i < m_arrTemplateCloseMaxValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateCloseMaxValue" + i, m_arrTemplateCloseMaxValue[i]);
            }

            objFile.WriteElement1Value("TemplateThresholdMaxValueCount", m_arrTemplateThresholdMaxValue.Count);
            for (int i = 0; i < m_arrTemplateThresholdMaxValue.Count; i++)
            {
                objFile.WriteElement1Value("TemplateThresholdMaxValue" + i, m_arrTemplateThresholdMaxValue[i]);
            }

            objFile.WriteElement1Value("TemplateImageProcessSeqCount", m_arrTemplateImageProcessSeq.Count);
            for (int i = 0; i < m_arrTemplateImageProcessSeq.Count; i++)
            {
                objFile.WriteElement1Value("TemplateImageProcessSeqCount" + i, m_arrTemplateImageProcessSeq[i].Count);
                objFile.WriteElement1Value("TemplateImageProcessSeq" + i, "");
                for (int j = 0; j < m_arrTemplateImageProcessSeq[i].Count; j++)
                {
                    objFile.WriteElement2Value("TemplateImageProcessSeq" + i + j, m_arrTemplateImageProcessSeq[i][j]);
                }
            }

            for (int i = 0; i < m_arrTemplateMarkWhitePixel.Count; i++)
            {
                objFile.WriteElement1Value("TemplateMarkWhitePixel" + i, m_arrTemplateMarkWhitePixel[i]);
            }

            for (int i = 0; i < m_arrTemplateMarkBlackPixel.Count; i++)
            {
                objFile.WriteElement1Value("TemplateMarkBlackPixel" + i, m_arrTemplateMarkBlackPixel[i]);
            }

            objFile.WriteEndElement();
        }

        public void SaveSeal_SECSGEM(string strPath, string strSectionName, string strVisionName, float fCalibPixelPerMM)
        {
            //XmlParser objFile = new XmlParser(strPath, blnNewFile);

            //objFile.WriteSectionElement(strSectionName, blnNewSection);

            XmlParser objFile = new XmlParser(strPath, "SECSGEMData");
            objFile.WriteRootElement("SECSGEMData");
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_SpocketHolePosition", m_intSpocketHolePosition);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_BuildObjectLength", m_intBuildObjectLength);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_Seal1Threshold", m_intSealLineEdgeThreshold[0]); // m_arrBlackBlobs[0].ref_intAbsoluteThreshold);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_Seal2Threshold", m_intSealLineEdgeThreshold[1]); // m_arrBlackBlobs[1].ref_intAbsoluteThreshold);
            for (int i = 0; i < m_arrOverHeatBlobs.Length; i++)
            {
                string strIndex = "";
                if (i > 0)
                    strIndex = i.ToString();
                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_OverHeatThreshold" + strIndex, m_arrOverHeatBlobs[i].ref_intAbsoluteThreshold);
                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_OverHeatLowThreshold" + strIndex, m_arrOverHeatBlobs[i].ref_intAbsoluteLowThreshold);
                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_OverHeatHighThreshold" + strIndex, m_arrOverHeatBlobs[i].ref_intAbsoluteHighThreshold);
            }
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_SprocketHoleDefectThreshold", m_objSprocketHoleDefectBlobs.ref_intAbsoluteThreshold);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_SprocketHoleBrokenThreshold", m_objSprocketHoleBrokenBlobs.ref_intAbsoluteThreshold);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_SprocketHoleRoundnessThreshold", m_objSprocketHoleRoundnessBlobs.ref_intAbsoluteThreshold);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_DistanceThreshold", m_objDistanceBlobs.ref_intAbsoluteThreshold);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_SealEdgeStraightnessThreshold", m_objSealEdgeStraightnessBlobs.ref_intAbsoluteThreshold);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_Seal1MinArea", m_arrBlackBlobs[0].ref_intMinAreaLimit / (fCalibPixelPerMM * fCalibPixelPerMM));
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_Seal2MinArea", m_arrBlackBlobs[1].ref_intMinAreaLimit / (fCalibPixelPerMM * fCalibPixelPerMM));
            for (int i = 0; i < m_arrOverHeatBlobs.Length; i++)
            {
                string strIndex = "";
                if (i > 0)
                    strIndex = i.ToString();
                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_OverHeatMinArea" + strIndex, m_arrOverHeatBlobs[i].ref_intMinAreaLimit / (fCalibPixelPerMM * fCalibPixelPerMM));
            }
            for (int i = 0; i < m_arrScratchesBlobs.Length; i++)
            {
                string strIndex = "";
                if (i > 0)
                    strIndex = i.ToString();
                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_ScratchesMinArea" + strIndex, m_arrScratchesBlobs[i].ref_intMinAreaLimit / (fCalibPixelPerMM * fCalibPixelPerMM));
            }
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_SealHoleMinArea1", m_intHoleMinArea1 / (fCalibPixelPerMM * fCalibPixelPerMM));
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_SealHoleMinArea2", m_intHoleMinArea2 / (fCalibPixelPerMM * fCalibPixelPerMM));
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_MinBrokenWidth", m_fMinBrokenWidth / (fCalibPixelPerMM));
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_PositionCenterX", m_pPositionCenterPoint.X / (fCalibPixelPerMM));
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_PositionCenterY", m_pPositionCenterPoint.Y / (fCalibPixelPerMM));
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_WidthLowerTolerance", m_fWidthLowerTolerance / fCalibPixelPerMM);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_WidthLowerTolerance1", m_fWidthLowerTolerance1 / fCalibPixelPerMM);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_WidthLowerTolerance2", m_fWidthLowerTolerance2 / fCalibPixelPerMM);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_WidthUpperTolerance", m_fWidthUpperTolerance / fCalibPixelPerMM);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_WidthUpperTolerance1", m_fWidthUpperTolerance1 / fCalibPixelPerMM);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_WidthUpperTolerance2", m_fWidthUpperTolerance2 / fCalibPixelPerMM);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_DistanceMinTolerance", m_fDistanceMinTolerance / fCalibPixelPerMM);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_DistanceMaxTolerance", m_fDistanceMaxTolerance / fCalibPixelPerMM);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_ShiftPositionTolerance", m_fShiftPositionTolerance / fCalibPixelPerMM);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_SealScoreTolerance", m_fSealScoreTolerance);

            for (int i = 0; i < 2; i++)
            {
                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_TemplateSealArea" + i, m_intTemplateAreaAVG[i] / (fCalibPixelPerMM * fCalibPixelPerMM));
                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_SealAreaTolerance" + i, m_arrSealAreaTolerance[i] / (fCalibPixelPerMM * fCalibPixelPerMM));
            }
            for (int i = 0; i < 3; i++)
                objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_TemplateSealLineWidth" + i, m_fTemplateWidth[i] / fCalibPixelPerMM);

            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_PocketMinScore", m_fPocketMinScore);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_MarkMinScore", m_fMarkMinScore);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_MarkMinWhiteArea", m_fMarkMinWhiteArea);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_MarkMaxWhiteArea", m_fMarkMaxWhiteArea);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_MarkPixelThreshold", m_intMarkPixelThreshold);
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_StdDevTol", m_intStdDevTol);
            // Grab image index
            objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GrabImageIndexCount", m_arrGrabImageIndex.Count);
            for (int j = 0; j < 8; j++)
            {
                if (m_arrGrabImageIndex.Count > j)
                    objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GrabImageIndex" + j.ToString(), m_arrGrabImageIndex[j]);
                else
                    objFile.WriteElementValue(strVisionName + "_" + strSectionName + "_GrabImageIndex" + j.ToString(), "NA");
            }
            objFile.WriteEndElement();
        }

        public void LoadSeal(string strPath, string strSectionName, float fCalibPixelPerMM)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection(strSectionName);
            m_intSpocketHolePosition = objFile.GetValueAsInt("SpocketHolePosition", 0);
            m_intBuildObjectLength = objFile.GetValueAsInt("BuildObjectLength", 10);
            m_intSealLineEdgeThreshold[0] = m_arrBlackBlobs[0].ref_intAbsoluteThreshold = objFile.GetValueAsInt("Seal1Threshold", -4);
            m_intSealLineEdgeThreshold[1] = m_arrBlackBlobs[1].ref_intAbsoluteThreshold = objFile.GetValueAsInt("Seal2Threshold", -4);
            m_intSealBrokenAreaThreshold[0] = objFile.GetValueAsInt("Seal1BrokenAreaThreshold", m_intSealLineEdgeThreshold[0]); // 2020 11 27 - CCENG: new variable to separate broken area threshold from seal threshold. If setting no exist, then using Seal1Threshold value.
            m_intSealBrokenAreaThreshold[1] = objFile.GetValueAsInt("Seal2BrokenAreaThreshold", m_intSealLineEdgeThreshold[1]); // 2020 11 27 - CCENG: new variable to separate broken area threshold from seal threshold. If setting no exist, then using Seal1Threshold value.
            for (int i = 0; i < m_arrOverHeatBlobs.Length; i++)
            {
                string strIndex = "";
                if (i > 0)
                    strIndex = i.ToString();
                m_arrOverHeatBlobs[i].ref_intAbsoluteThreshold = objFile.GetValueAsInt("OverHeatThreshold" + strIndex, -4);
                // 2020 09 15 - CCENG: m_objScratchesBlobs.ref_intAbsoluteThreshold will be set with m_objOverHeatBlobs low threshold because they are using the same threshold setting.
                m_arrScratchesBlobs[i].ref_intAbsoluteThreshold = m_arrScratchesBlobs[i].ref_intAbsoluteLowThreshold = m_arrOverHeatBlobs[i].ref_intAbsoluteLowThreshold = objFile.GetValueAsInt("OverHeatLowThreshold" + strIndex, -4);
                m_arrScratchesBlobs[i].ref_intAbsoluteHighThreshold = m_arrOverHeatBlobs[i].ref_intAbsoluteHighThreshold = objFile.GetValueAsInt("OverHeatHighThreshold" + strIndex, -4);
            }
            m_objSprocketHoleDefectBlobs.ref_intAbsoluteThreshold = objFile.GetValueAsInt("SprocketHoleDefectThreshold", -4);
            m_objSprocketHoleBrokenBlobs.ref_intAbsoluteThreshold = objFile.GetValueAsInt("SprocketHoleBrokenThreshold", -4);
            m_objSprocketHoleRoundnessBlobs.ref_intAbsoluteThreshold = objFile.GetValueAsInt("SprocketHoleRoundnessThreshold", -4);
            m_objSealEdgeStraightnessBlobs.ref_intAbsoluteThreshold = objFile.GetValueAsInt("SealEdgeStraightnessThreshold", -4);
            m_objDistanceBlobs.ref_intAbsoluteThreshold = objFile.GetValueAsInt("DistanceThreshold", -4);
            STTrackLog.WriteLine("SealBlog > Load ref_intAbsoluteThreshold = " + m_objDistanceBlobs.ref_intAbsoluteThreshold);
            m_minSealBrokenGap = objFile.GetValueAsFloat("FailBrokenSeal1", 0) * fCalibPixelPerMM;
            m_minSealBrokenGap2 = objFile.GetValueAsFloat("FailBrokenSeal2", 0) * fCalibPixelPerMM;
            //m_arrBlackBlobs[0].ref_intMinArea = Convert.ToInt32(objFile.GetValueAsFloat("Seal1MinArea", 0) * fCalibPixelPerMM * fCalibPixelPerMM);
            //m_arrBlackBlobs[1].ref_intMinArea = Convert.ToInt32(objFile.GetValueAsFloat("Seal2MinArea", 0) * fCalibPixelPerMM * fCalibPixelPerMM);
            // Previously deleted, didn't take care of these, the min should become 1
            m_arrBlackBlobs[0].ref_intMinAreaLimit = 1; // Convert.ToInt32(objFile.GetValueAsFloat("Seal1MinArea", 0.05f) * fCalibPixelPerMM * fCalibPixelPerMM);
            m_arrBlackBlobs[1].ref_intMinAreaLimit = 1; // Convert.ToInt32(objFile.GetValueAsFloat("Seal2MinArea", 0.05f) * fCalibPixelPerMM * fCalibPixelPerMM);
            for (int i = 0; i < m_arrOverHeatAreaMinTolerance.Length; i++)
            {
                string strIndex = "";
                if (i > 0)
                    strIndex = i.ToString();
                m_arrOverHeatAreaMinTolerance[i] = objFile.GetValueAsFloat("OverHeatMinArea" + strIndex, 0.05f);
                m_arrOverHeatBlobs[i].ref_intMinAreaLimit = (int)Math.Floor(m_arrOverHeatAreaMinTolerance[i] * fCalibPixelPerMM * fCalibPixelPerMM);
            }
            m_fSprocketHoleDefectMaxTolerance = objFile.GetValueAsFloat("SprocketHoleDefectMaxTolerance", 1f);
            m_fSprocketHoleBrokenMaxTolerance = objFile.GetValueAsFloat("SprocketHoleBrokenMaxTolerance", 1f);
            m_fSprocketHoleRoundnessMaxTolerance = objFile.GetValueAsFloat("SprocketHoleRoundnessMaxTolerance", 1f);
            m_fSealEdgeStraightnessMaxTolerance = objFile.GetValueAsFloat("SealEdgeStraightnessMaxTolerance", 1f);
            for (int i = 0; i < m_arrScratchesAreaMinTolerance.Length; i++)
            {
                string strIndex = "";
                if (i > 0)
                    strIndex = i.ToString();
                m_arrScratchesAreaMinTolerance[i] = objFile.GetValueAsFloat("ScratchesAreaMinTolerance" + strIndex, 0.05f);
                m_arrScratchesBlobs[i].ref_intMinAreaLimit = (int)Math.Floor(m_arrScratchesAreaMinTolerance[i] * fCalibPixelPerMM * fCalibPixelPerMM);
            }

            m_intHoleMinArea1 = Convert.ToInt32(objFile.GetValueAsFloat("SealHoleMinArea1", 0) * fCalibPixelPerMM * fCalibPixelPerMM);
            m_intHoleMinArea2 = Convert.ToInt32(objFile.GetValueAsFloat("SealHoleMinArea2", 0) * fCalibPixelPerMM * fCalibPixelPerMM);
            m_fMinBrokenWidth = objFile.GetValueAsFloat("MinBrokenWidth", 0) * fCalibPixelPerMM;
            m_pPositionCenterPoint.X = objFile.GetValueAsFloat("PositionCenterX", 0) * fCalibPixelPerMM;
            m_pPositionCenterPoint.Y = objFile.GetValueAsFloat("PositionCenterY", 0) * fCalibPixelPerMM;

            m_fWidthLowerTolerance = objFile.GetValueAsFloat("WidthLowerTolerance", 0) * fCalibPixelPerMM;
            m_fWidthLowerTolerance1 = objFile.GetValueAsFloat("WidthLowerTolerance1", 0) * fCalibPixelPerMM;
            m_fWidthLowerTolerance2 = objFile.GetValueAsFloat("WidthLowerTolerance2", 0) * fCalibPixelPerMM;
            m_fWidthUpperTolerance = objFile.GetValueAsFloat("WidthUpperTolerance", 0) * fCalibPixelPerMM;
            m_fWidthUpperTolerance1 = objFile.GetValueAsFloat("WidthUpperTolerance1", 0) * fCalibPixelPerMM;
            m_fWidthUpperTolerance2 = objFile.GetValueAsFloat("WidthUpperTolerance2", 0) * fCalibPixelPerMM;
            m_fDistanceMinTolerance = objFile.GetValueAsFloat("DistanceMinTolerance", 0) * fCalibPixelPerMM;
            m_fDistanceMaxTolerance = objFile.GetValueAsFloat("DistanceMaxTolerance", 0) * fCalibPixelPerMM;
            m_fSprocketHoleDistanceMinTolerance = objFile.GetValueAsFloat("SprocketHoleDistanceMinTolerance", 0) * fCalibPixelPerMM;
            m_fSprocketHoleDistanceMaxTolerance = objFile.GetValueAsFloat("SprocketHoleDistanceMaxTolerance", 0) * fCalibPixelPerMM;
            m_fSprocketHoleDiameterMinTolerance = objFile.GetValueAsFloat("SprocketHoleDiameterMinTolerance", 0) * fCalibPixelPerMM;
            m_fSprocketHoleDiameterMaxTolerance = objFile.GetValueAsFloat("SprocketHoleDiameterMaxTolerance", 0) * fCalibPixelPerMM;

            m_fShiftPositionTolerance = objFile.GetValueAsFloat("ShiftPositionTolerance", 0) * fCalibPixelPerMM;
            m_fSealScoreTolerance = objFile.GetValueAsFloat("SealScoreTolerance", 0.7f);
            m_intFailOptionMask = objFile.GetValueAsInt("FailMask", 0x7FF);
            m_intSealEdgeSensitivity = objFile.GetValueAsInt("SealEdgeSensitivity", 0);
            m_intSealEdgeTolerance = objFile.GetValueAsInt("SealEdgeTolerance", 3);
            m_intSprocketHoleInspectionAreaInwardTolerance = objFile.GetValueAsInt("SprocketHoleInspectionAreaInwardTolerance", 0);
            m_intSprocketHoleBrokenOutwardTolerance_Outer = objFile.GetValueAsInt("SprocketHoleBrokenOutwardTolerance_Outer", 0);
            m_intSprocketHoleBrokenOutwardTolerance_Inner = objFile.GetValueAsInt("SprocketHoleBrokenOutwardTolerance_Inner", 0);
            m_fTapePocketPitchByImage = objFile.GetValueAsFloat("TapePocketPitchByImage", -1);
            //m_blnWantUsePatternCheckUnitPresent = objFile.GetValueAsBoolean("WantUsePatternCheckUnitPresent", true);
            //m_blnWantUsePixelCheckUnitPresent = objFile.GetValueAsBoolean("WantUsePixelCheckUnitPresent", true);
            for (int i = 0; i < 2; i++)
            {
                m_intTemplateAreaAVG[i] = Convert.ToInt32(objFile.GetValueAsFloat("TemplateSealArea" + i, 0) * fCalibPixelPerMM * fCalibPixelPerMM);
                m_arrSealAreaTolerance[i] = objFile.GetValueAsFloat("SealAreaTolerance" + i, 0) * fCalibPixelPerMM * fCalibPixelPerMM;
            }
            for (int i = 0; i < 3; i++)
            {
                m_fTemplateWidth[i] = Convert.ToInt32(objFile.GetValueAsFloat("TemplateSealLineWidth" + i, 0) * fCalibPixelPerMM);
            }

            m_fPocketMinScore = objFile.GetValueAsFloat("PocketMinScore", 0.7f);
            m_fMarkMinScore = objFile.GetValueAsFloat("MarkMinScore", 0.7f);
            m_fMarkMinWhiteArea = objFile.GetValueAsFloat("MarkMinWhiteArea", 0);
            m_fMarkMaxWhiteArea = objFile.GetValueAsFloat("MarkMaxWhiteArea", 0);

            int intTemplateMarkROIPositionCount = objFile.GetValueAsInt("TemplateMarkROIPositionCount", 0);
            m_arrTemplateMarkROIPosition.Clear();
            for (int i = 0; i < intTemplateMarkROIPositionCount; i++)
            {
                m_arrTemplateMarkROIPosition.Add(new Point(objFile.GetValueAsInt("TemplateMarkROIPositionX" + i, 0), objFile.GetValueAsInt("TemplateMarkROIPositionY" + i, 0)));
            }

            int intTemplateMarkROISizeCount = objFile.GetValueAsInt("TemplateMarkROISizeCount", 0);
            m_arrTemplateMarkROISize.Clear();
            for (int i = 0; i < intTemplateMarkROISizeCount; i++)
            {
                m_arrTemplateMarkROISize.Add(new Size(objFile.GetValueAsInt("TemplateMarkROISizeWidth" + i, 0), objFile.GetValueAsInt("TemplateMarkROISizeHeight" + i, 0)));
            }

            int intTemplateMarkThresholdCount = objFile.GetValueAsInt("TemplateMarkThresholdCount", 1);
            m_arrTemplateMarkThreshold.Clear();
            for (int i = 0; i < intTemplateMarkThresholdCount; i++)
            {
                m_arrTemplateMarkThreshold.Add(objFile.GetValueAsInt("TemplateMarkThreshold" + i, 0));
            }

            int intTemplateMarkThresholdRelativeCount = objFile.GetValueAsInt("TemplateMarkThresholdRelativeCount", 1);
            m_arrTemplateMarkThresholdRelative.Clear();
            for (int i = 0; i < intTemplateMarkThresholdRelativeCount; i++)
            {
                m_arrTemplateMarkThresholdRelative.Add(objFile.GetValueAsFloat("TemplateMarkThresholdRelative" + i, 0));
            }

            int intTemplateWantAutoThresholdRelativeCount = objFile.GetValueAsInt("TemplateWantAutoThresholdRelative", 1);
            m_arrTemplateWantAutoThresholdRelative.Clear();
            for (int i = 0; i < intTemplateWantAutoThresholdRelativeCount; i++)
            {
                m_arrTemplateWantAutoThresholdRelative.Add(objFile.GetValueAsBoolean("TemplateWantAutoThresholdRelative" + i, false));
            }

            int intTemplateErodeValueCount = objFile.GetValueAsInt("TemplateErodeValueCount", 1);
            m_arrTemplateErodeValue.Clear();
            for (int i = 0; i < intTemplateErodeValueCount; i++)
            {
                m_arrTemplateErodeValue.Add(objFile.GetValueAsInt("TemplateErodeValue" + i, 0));
            }

            int intTemplateDilateValueCount = objFile.GetValueAsInt("TemplateDilateValueCount", 1);
            m_arrTemplateDilateValue.Clear();
            for (int i = 0; i < intTemplateDilateValueCount; i++)
            {
                m_arrTemplateDilateValue.Add(objFile.GetValueAsInt("TemplateDilateValue" + i, 0));
            }

            int intTemplateOpenValueCount = objFile.GetValueAsInt("TemplateOpenValueCount", 1);
            m_arrTemplateOpenValue.Clear();
            for (int i = 0; i < intTemplateOpenValueCount; i++)
            {
                m_arrTemplateOpenValue.Add(objFile.GetValueAsInt("TemplateOpenValue" + i, 0));
            }

            int intTemplateCloseValueCount = objFile.GetValueAsInt("TemplateCloseValueCount", 1);
            m_arrTemplateCloseValue.Clear();
            for (int i = 0; i < intTemplateCloseValueCount; i++)
            {
                m_arrTemplateCloseValue.Add(objFile.GetValueAsInt("TemplateCloseValue" + i, 0));
            }

            int intTemplateErodeMinValueCount = objFile.GetValueAsInt("TemplateErodeMinValueCount", 1);
            m_arrTemplateErodeMinValue.Clear();
            for (int i = 0; i < intTemplateErodeMinValueCount; i++)
            {
                m_arrTemplateErodeMinValue.Add(objFile.GetValueAsInt("TemplateErodeMinValue" + i, 0));
            }

            int intTemplateDilateMinValueCount = objFile.GetValueAsInt("TemplateDilateMinValueCount", 1);
            m_arrTemplateDilateMinValue.Clear();
            for (int i = 0; i < intTemplateDilateMinValueCount; i++)
            {
                m_arrTemplateDilateMinValue.Add(objFile.GetValueAsInt("TemplateDilateMinValue" + i, 0));
            }

            int intTemplateOpenMinValueCount = objFile.GetValueAsInt("TemplateOpenMinValueCount", 1);
            m_arrTemplateOpenMinValue.Clear();
            for (int i = 0; i < intTemplateOpenMinValueCount; i++)
            {
                m_arrTemplateOpenMinValue.Add(objFile.GetValueAsInt("TemplateOpenMinValue" + i, 0));
            }

            int intTemplateCloseMinValueCount = objFile.GetValueAsInt("TemplateCloseMinValueCount", 1);
            m_arrTemplateCloseMinValue.Clear();
            for (int i = 0; i < intTemplateCloseMinValueCount; i++)
            {
                m_arrTemplateCloseMinValue.Add(objFile.GetValueAsInt("TemplateCloseMinValue" + i, 0));
            }

            int intTemplateThresholdMinValueCount = objFile.GetValueAsInt("TemplateThresholdMinValueCount", 1);
            m_arrTemplateThresholdMinValue.Clear();
            for (int i = 0; i < intTemplateThresholdMinValueCount; i++)
            {
                m_arrTemplateThresholdMinValue.Add(objFile.GetValueAsInt("TemplateThresholdMinValue" + i, 0));
            }

            int intTemplateErodeMaxValueCount = objFile.GetValueAsInt("TemplateErodeMaxValueCount", 1);
            m_arrTemplateErodeMaxValue.Clear();
            for (int i = 0; i < intTemplateErodeMaxValueCount; i++)
            {
                m_arrTemplateErodeMaxValue.Add(objFile.GetValueAsInt("TemplateErodeMaxValue" + i, 0));
            }

            int intTemplateDilateMaxValueCount = objFile.GetValueAsInt("TemplateDilateMaxValueCount", 1);
            m_arrTemplateDilateMaxValue.Clear();
            for (int i = 0; i < intTemplateDilateMaxValueCount; i++)
            {
                m_arrTemplateDilateMaxValue.Add(objFile.GetValueAsInt("TemplateDilateMaxValue" + i, 0));
            }

            int intTemplateOpenMaxValueCount = objFile.GetValueAsInt("TemplateOpenMaxValueCount", 1);
            m_arrTemplateOpenMaxValue.Clear();
            for (int i = 0; i < intTemplateOpenMaxValueCount; i++)
            {
                m_arrTemplateOpenMaxValue.Add(objFile.GetValueAsInt("TemplateOpenMaxValue" + i, 0));
            }

            int intTemplateCloseMaxValueCount = objFile.GetValueAsInt("TemplateCloseMaxValueCount", 1);
            m_arrTemplateCloseMaxValue.Clear();
            for (int i = 0; i < intTemplateCloseMaxValueCount; i++)
            {
                m_arrTemplateCloseMaxValue.Add(objFile.GetValueAsInt("TemplateCloseMaxValue" + i, 0));
            }

            int intTemplateThresholdMaxValueCount = objFile.GetValueAsInt("TemplateThresholdMaxValueCount", 1);
            m_arrTemplateThresholdMaxValue.Clear();
            for (int i = 0; i < intTemplateThresholdMaxValueCount; i++)
            {
                m_arrTemplateThresholdMaxValue.Add(objFile.GetValueAsInt("TemplateThresholdMaxValue" + i, 0));
            }

            int intTemplateImageProcessSeqCount = objFile.GetValueAsInt("TemplateImageProcessSeqCount", 0);
            m_arrTemplateImageProcessSeq.Clear();
            for (int i = 0; i < intTemplateImageProcessSeqCount; i++)
            {
                m_arrTemplateImageProcessSeq.Add(new List<string>());
                int intTemplateImageProcessSeqCount2 = objFile.GetValueAsInt("TemplateImageProcessSeqCount" + i, 0);
                objFile.GetSecondSection("TemplateImageProcessSeq" + i);
                for (int j = 0; j < intTemplateImageProcessSeqCount2; j++)
                {
                    m_arrTemplateImageProcessSeq[i].Add(objFile.GetValueAsString("TemplateImageProcessSeq" + i + j, "", 2));
                }
            }

            m_intStdDevTol = objFile.GetValueAsInt("StdDevTol", 15);

            // Grab image index
            int intGrabImageIndexCount = objFile.GetValueAsInt("GrabImageIndexCount", 0);
            m_arrGrabImageIndex.Clear();
            for (int j = 0; j < intGrabImageIndexCount; j++)
                m_arrGrabImageIndex.Add(objFile.GetValueAsInt("GrabImageIndex" + j.ToString(), 0));
        }
        public void LoadSealToleranceOnly(string strPath, string strSectionName, float fCalibPixelPerMM)
        {
            XmlParser objFile = new XmlParser(strPath);
            objFile.GetFirstSection(strSectionName);

            // Previously deleted, didn't take care of these, the min should become 1
            m_arrBlackBlobs[0].ref_intMinAreaLimit = 1; // Convert.ToInt32(objFile.GetValueAsFloat("Seal1MinArea", 0.05f) * fCalibPixelPerMM * fCalibPixelPerMM);
            m_arrBlackBlobs[1].ref_intMinAreaLimit = 1; // Convert.ToInt32(objFile.GetValueAsFloat("Seal2MinArea", 0.05f) * fCalibPixelPerMM * fCalibPixelPerMM);
            for (int i = 0; i < m_arrOverHeatAreaMinTolerance.Length; i++)
            {
                string strIndex = "";
                if (i > 0)
                    strIndex = i.ToString();
                m_arrOverHeatAreaMinTolerance[i] = objFile.GetValueAsFloat("OverHeatMinArea" + strIndex, 0.05f);
                m_arrOverHeatBlobs[i].ref_intMinAreaLimit = (int)Math.Floor(m_arrOverHeatAreaMinTolerance[i] * fCalibPixelPerMM * fCalibPixelPerMM);
            }
            m_fSprocketHoleDefectMaxTolerance = objFile.GetValueAsFloat("SprocketHoleDefectMaxTolerance", 1f);
            m_fSprocketHoleBrokenMaxTolerance = objFile.GetValueAsFloat("SprocketHoleBrokenMaxTolerance", 1f);
            m_fSprocketHoleRoundnessMaxTolerance = objFile.GetValueAsFloat("SprocketHoleRoundnessMaxTolerance", 1f);
            m_fSealEdgeStraightnessMaxTolerance = objFile.GetValueAsFloat("SealEdgeStraightnessMaxTolerance", 1f);
            for (int i = 0; i < m_arrScratchesAreaMinTolerance.Length; i++)
            {
                string strIndex = "";
                if (i > 0)
                    strIndex = i.ToString();
                m_arrScratchesAreaMinTolerance[i] = objFile.GetValueAsFloat("ScratchesAreaMinTolerance" + strIndex, 0.05f);
                m_arrScratchesBlobs[i].ref_intMinAreaLimit = (int)Math.Floor(m_arrScratchesAreaMinTolerance[i] * fCalibPixelPerMM * fCalibPixelPerMM);
            }
            m_intHoleMinArea1 = Convert.ToInt32(objFile.GetValueAsFloat("SealHoleMinArea1", 0) * fCalibPixelPerMM * fCalibPixelPerMM);
            m_intHoleMinArea2 = Convert.ToInt32(objFile.GetValueAsFloat("SealHoleMinArea2", 0) * fCalibPixelPerMM * fCalibPixelPerMM);
            m_fMinBrokenWidth = objFile.GetValueAsFloat("MinBrokenWidth", 0) * fCalibPixelPerMM;
            m_pPositionCenterPoint.X = objFile.GetValueAsFloat("PositionCenterX", 0) * fCalibPixelPerMM;
            m_pPositionCenterPoint.Y = objFile.GetValueAsFloat("PositionCenterY", 0) * fCalibPixelPerMM;
            m_minSealBrokenGap = objFile.GetValueAsFloat("FailBrokenSeal1", 0) * fCalibPixelPerMM;
            m_minSealBrokenGap2 = objFile.GetValueAsFloat("FailBrokenSeal2", 0) * fCalibPixelPerMM;
            m_fWidthLowerTolerance = objFile.GetValueAsFloat("WidthLowerTolerance", 0) * fCalibPixelPerMM;
            m_fWidthLowerTolerance1 = objFile.GetValueAsFloat("WidthLowerTolerance1", 0) * fCalibPixelPerMM;
            m_fWidthLowerTolerance2 = objFile.GetValueAsFloat("WidthLowerTolerance2", 0) * fCalibPixelPerMM;
            m_fWidthUpperTolerance = objFile.GetValueAsFloat("WidthUpperTolerance", 0) * fCalibPixelPerMM;
            m_fWidthUpperTolerance1 = objFile.GetValueAsFloat("WidthUpperTolerance1", 0) * fCalibPixelPerMM;
            m_fWidthUpperTolerance2 = objFile.GetValueAsFloat("WidthUpperTolerance2", 0) * fCalibPixelPerMM;
            m_fDistanceMinTolerance = objFile.GetValueAsFloat("DistanceMinTolerance", 0) * fCalibPixelPerMM;
            m_fDistanceMaxTolerance = objFile.GetValueAsFloat("DistanceMaxTolerance", 0) * fCalibPixelPerMM;
            m_fSprocketHoleDistanceMinTolerance = objFile.GetValueAsFloat("SprocketHoleDistanceMinTolerance", 0) * fCalibPixelPerMM;
            m_fSprocketHoleDistanceMaxTolerance = objFile.GetValueAsFloat("SprocketHoleDistanceMaxTolerance", 0) * fCalibPixelPerMM;
            m_fSprocketHoleDiameterMinTolerance = objFile.GetValueAsFloat("SprocketHoleDiameterMinTolerance", 0) * fCalibPixelPerMM;
            m_fSprocketHoleDiameterMaxTolerance = objFile.GetValueAsFloat("SprocketHoleDiameterMaxTolerance", 0) * fCalibPixelPerMM;

            m_fShiftPositionTolerance = objFile.GetValueAsFloat("ShiftPositionTolerance", 0) * fCalibPixelPerMM;
            m_fSealScoreTolerance = objFile.GetValueAsFloat("SealScoreTolerance", 0.7f);

            for (int i = 0; i < 2; i++)
            {
                m_intTemplateAreaAVG[i] = Convert.ToInt32(objFile.GetValueAsFloat("TemplateSealArea" + i, 0) * fCalibPixelPerMM * fCalibPixelPerMM);
                m_arrSealAreaTolerance[i] = objFile.GetValueAsFloat("SealAreaTolerance" + i, 0) * fCalibPixelPerMM * fCalibPixelPerMM;
            }
            for (int i = 0; i < 3; i++)
            {
                m_fTemplateWidth[i] = Convert.ToInt32(objFile.GetValueAsFloat("TemplateSealLineWidth" + i, 0) * fCalibPixelPerMM);
            }

            m_fPocketMinScore = objFile.GetValueAsFloat("PocketMinScore", 0.7f);
            m_fMarkMinScore = objFile.GetValueAsFloat("MarkMinScore", 0.7f);
            m_fMarkMinWhiteArea = objFile.GetValueAsFloat("MarkMinWhiteArea", 0);
            m_fMarkMaxWhiteArea = objFile.GetValueAsFloat("MarkMaxWhiteArea", 0);

            int intTemplateMarkROIPositionCount = objFile.GetValueAsInt("TemplateMarkROIPositionCount", 0);
            m_arrTemplateMarkROIPosition.Clear();
            for (int i = 0; i < intTemplateMarkROIPositionCount; i++)
            {
                m_arrTemplateMarkROIPosition.Add(new Point(objFile.GetValueAsInt("TemplateMarkROIPositionX" + i, 0), objFile.GetValueAsInt("TemplateMarkROIPositionY" + i, 0)));
            }

            int intTemplateMarkROISizeCount = objFile.GetValueAsInt("TemplateMarkROISizeCount", 0);
            m_arrTemplateMarkROISize.Clear();
            for (int i = 0; i < intTemplateMarkROISizeCount; i++)
            {
                m_arrTemplateMarkROISize.Add(new Size(objFile.GetValueAsInt("TemplateMarkROISizeWidth" + i, 0), objFile.GetValueAsInt("TemplateMarkROISizeHeight" + i, 0)));
            }

            int intTemplateMarkThresholdCount = objFile.GetValueAsInt("TemplateMarkThresholdCount", 1);
            m_arrTemplateMarkThreshold.Clear();
            for (int i = 0; i < intTemplateMarkThresholdCount; i++)
            {
                m_arrTemplateMarkThreshold.Add(objFile.GetValueAsInt("TemplateMarkThreshold" + i, 0));
            }

            int intTemplateMarkThresholdRelativeCount = objFile.GetValueAsInt("TemplateMarkThresholdRelativeCount", 1);
            m_arrTemplateMarkThresholdRelative.Clear();
            for (int i = 0; i < intTemplateMarkThresholdRelativeCount; i++)
            {
                m_arrTemplateMarkThresholdRelative.Add(objFile.GetValueAsFloat("TemplateMarkThresholdRelative" + i, 0));
            }

            int intTemplateWantAutoThresholdRelativeCount = objFile.GetValueAsInt("TemplateWantAutoThresholdRelative", 1);
            m_arrTemplateWantAutoThresholdRelative.Clear();
            for (int i = 0; i < intTemplateWantAutoThresholdRelativeCount; i++)
            {
                m_arrTemplateWantAutoThresholdRelative.Add(objFile.GetValueAsBoolean("TemplateWantAutoThresholdRelative" + i, false));
            }

            int intTemplateErodeValueCount = objFile.GetValueAsInt("TemplateErodeValueCount", 0);
            m_arrTemplateErodeValue.Clear();
            for (int i = 0; i < intTemplateErodeValueCount; i++)
            {
                m_arrTemplateErodeValue.Add(objFile.GetValueAsInt("TemplateErodeValue" + i, 0));
            }

            int intTemplateDilateValueCount = objFile.GetValueAsInt("TemplateDilateValueCount", 0);
            m_arrTemplateDilateValue.Clear();
            for (int i = 0; i < intTemplateDilateValueCount; i++)
            {
                m_arrTemplateDilateValue.Add(objFile.GetValueAsInt("TemplateDilateValue" + i, 0));
            }

            int intTemplateOpenValueCount = objFile.GetValueAsInt("TemplateOpenValueCount", 0);
            m_arrTemplateOpenValue.Clear();
            for (int i = 0; i < intTemplateOpenValueCount; i++)
            {
                m_arrTemplateOpenValue.Add(objFile.GetValueAsInt("TemplateOpenValue" + i, 0));
            }

            int intTemplateCloseValueCount = objFile.GetValueAsInt("TemplateCloseValueCount", 0);
            m_arrTemplateCloseValue.Clear();
            for (int i = 0; i < intTemplateCloseValueCount; i++)
            {
                m_arrTemplateCloseValue.Add(objFile.GetValueAsInt("TemplateCloseValue" + i, 0));
            }

            int intTemplateErodeMinValueCount = objFile.GetValueAsInt("TemplateErodeMinValueCount", 1);
            m_arrTemplateErodeMinValue.Clear();
            for (int i = 0; i < intTemplateErodeMinValueCount; i++)
            {
                m_arrTemplateErodeMinValue.Add(objFile.GetValueAsInt("TemplateErodeMinValue" + i, 0));
            }

            int intTemplateDilateMinValueCount = objFile.GetValueAsInt("TemplateDilateMinValueCount", 1);
            m_arrTemplateDilateMinValue.Clear();
            for (int i = 0; i < intTemplateDilateMinValueCount; i++)
            {
                m_arrTemplateDilateMinValue.Add(objFile.GetValueAsInt("TemplateDilateMinValue" + i, 0));
            }

            int intTemplateOpenMinValueCount = objFile.GetValueAsInt("TemplateOpenMinValueCount", 1);
            m_arrTemplateOpenMinValue.Clear();
            for (int i = 0; i < intTemplateOpenMinValueCount; i++)
            {
                m_arrTemplateOpenMinValue.Add(objFile.GetValueAsInt("TemplateOpenMinValue" + i, 0));
            }

            int intTemplateCloseMinValueCount = objFile.GetValueAsInt("TemplateCloseMinValueCount", 1);
            m_arrTemplateCloseMinValue.Clear();
            for (int i = 0; i < intTemplateCloseMinValueCount; i++)
            {
                m_arrTemplateCloseMinValue.Add(objFile.GetValueAsInt("TemplateCloseMinValue" + i, 0));
            }

            int intTemplateThresholdMinValueCount = objFile.GetValueAsInt("TemplateThresholdMinValueCount", 1);
            m_arrTemplateThresholdMinValue.Clear();
            for (int i = 0; i < intTemplateThresholdMinValueCount; i++)
            {
                m_arrTemplateThresholdMinValue.Add(objFile.GetValueAsInt("TemplateThresholdMinValue" + i, 0));
            }

            int intTemplateErodeMaxValueCount = objFile.GetValueAsInt("TemplateErodeMaxValueCount", 1);
            m_arrTemplateErodeMaxValue.Clear();
            for (int i = 0; i < intTemplateErodeMaxValueCount; i++)
            {
                m_arrTemplateErodeMaxValue.Add(objFile.GetValueAsInt("TemplateErodeMaxValue" + i, 0));
            }

            int intTemplateDilateMaxValueCount = objFile.GetValueAsInt("TemplateDilateMaxValueCount", 1);
            m_arrTemplateDilateMaxValue.Clear();
            for (int i = 0; i < intTemplateDilateMaxValueCount; i++)
            {
                m_arrTemplateDilateMaxValue.Add(objFile.GetValueAsInt("TemplateDilateMaxValue" + i, 0));
            }

            int intTemplateOpenMaxValueCount = objFile.GetValueAsInt("TemplateOpenMaxValueCount", 1);
            m_arrTemplateOpenMaxValue.Clear();
            for (int i = 0; i < intTemplateOpenMaxValueCount; i++)
            {
                m_arrTemplateOpenMaxValue.Add(objFile.GetValueAsInt("TemplateOpenMaxValue" + i, 0));
            }

            int intTemplateCloseMaxValueCount = objFile.GetValueAsInt("TemplateCloseMaxValueCount", 1);
            m_arrTemplateCloseMaxValue.Clear();
            for (int i = 0; i < intTemplateCloseMaxValueCount; i++)
            {
                m_arrTemplateCloseMaxValue.Add(objFile.GetValueAsInt("TemplateCloseMaxValue" + i, 0));
            }

            int intTemplateThresholdMaxValueCount = objFile.GetValueAsInt("TemplateThresholdMaxValueCount", 1);
            m_arrTemplateThresholdMaxValue.Clear();
            for (int i = 0; i < intTemplateThresholdMaxValueCount; i++)
            {
                m_arrTemplateThresholdMaxValue.Add(objFile.GetValueAsInt("TemplateThresholdMaxValue" + i, 0));
            }

            int intTemplateImageProcessSeqCount = objFile.GetValueAsInt("TemplateImageProcessSeqCount", 0);
            m_arrTemplateImageProcessSeq.Clear();
            for (int i = 0; i < intTemplateImageProcessSeqCount; i++)
            {
                m_arrTemplateImageProcessSeq.Add(new List<string>());
                int intTemplateImageProcessSeqCount2 = objFile.GetValueAsInt("TemplateImageProcessSeqCount" + i, 0);
                objFile.GetSecondSection("TemplateImageProcessSeq" + i);
                for (int j = 0; j < intTemplateImageProcessSeqCount2; j++)
                {
                    m_arrTemplateImageProcessSeq[i].Add(objFile.GetValueAsString("TemplateImageProcessSeq" + i + j, "", 2));
                }
            }

            m_intStdDevTol = objFile.GetValueAsInt("StdDevTol", 15);

        }

        public bool LoadMatcherImage(string strPath)
        {
            //// Load Seal Mark Template image
            //string strFile = strPath;
            //if (File.Exists(strFile))
            //{
            //    m_objMatcherImage.LoadImage(strFile);
            //    m_objMatcherThresholdImage.LoadImage(strFile);
            //    //m_objMatcherImage.LoadImage(strFile);
            //    //m_objMatcherImage.LoadImage(strFile);
            //    EasyImage.Threshold(m_objMatcherImage.ref_objMainImage, m_objMatcherThresholdImage.ref_objMainImage, m_intMarkPixelThreshold);


            //}

            return true;
        }

        //public bool TemplateImageMatching_SRM2(ROI objSampleROI)
        //{

        //    bool blnDebugImage = true;
        //    EBW8 objBW8 = new EBW8(125);
        //    int intHighPixel = 0;
        //    int intMediamPixel = 0;
        //    int intLowPixel = 0;
        //    int intTemplateHighPixel = 0;
        //    int intSampleHighPixel = 0;

        //    int intSmallestSizeX = Math.Min(objSampleROI.ref_ROIWidth, m_objMatcherImage.ref_objMainImage.Width);
        //    int intSmallestSizeY = Math.Min(objSampleROI.ref_ROIHeight, m_objMatcherImage.ref_objMainImage.Height);

        //    m_objSampleThresholdROI.AttachImage(m_objTempSampleImage);
        //    m_objSampleThresholdROI.LoadROISetting(0, 0, objSampleROI.ref_ROIWidth, objSampleROI.ref_ROIHeight);

        //    if (blnDebugImage)
        //        m_objSampleThresholdROI.SaveImage("D:\\TS\\0.m_objSampleThresholdROI.bmp");

        //    float fAngle = m_arrMarkMatcher[0][0].GetPosition(0).Angle;
        //    EasyImage.ScaleRotate(objSampleROI.ref_ROI, objSampleROI.ref_ROI.Width / 2f, objSampleROI.ref_ROI.Height / 2f, 
        //                         m_objSampleThresholdROI.ref_ROIWidth / 2f, m_objSampleThresholdROI.ref_ROIHeight / 2f, 1, 1,
        //                         fAngle, m_objSampleThresholdROI.ref_ROI, 0);

        //    m_objSampleThresholdROI.LoadROISetting(0, 0, intSmallestSizeX, intSmallestSizeY);

        //    bool blnWantSecondThreshold = false;
        //    m_fImageMatchScore = 0;
        //    float fImageMatchScore = 0;
        //    while (true)
        //    {
        //        EasyImage.ScaleRotate(objSampleROI.ref_ROI, objSampleROI.ref_ROI.Width / 2f, objSampleROI.ref_ROI.Height / 2f,
        //                         m_objSampleThresholdROI.ref_ROIWidth / 2f, m_objSampleThresholdROI.ref_ROIHeight / 2f, 1, 1,
        //                         fAngle, m_objSampleThresholdROI.ref_ROI, 0);

        //        if (blnWantSecondThreshold)
        //            EasyImage.Threshold(m_objSampleThresholdROI.ref_ROI, m_objSampleThresholdROI.ref_ROI, 254);
        //        else
        //            EasyImage.Threshold(m_objSampleThresholdROI.ref_ROI, m_objSampleThresholdROI.ref_ROI, m_intMarkPixelThreshold);
        //        if (blnDebugImage)
        //            m_objSampleThresholdROI.SaveImage("D:\\TS\\1.m_objSampleThresholdROI.bmp");
        //        EasyImage.PixelCount(m_objSampleThresholdROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intSampleHighPixel);

        //        m_objSampleComparedROI.AttachImage(m_objTempComparedImage);
        //        m_objSampleComparedROI.LoadROISetting(0, 0, intSmallestSizeX, intSmallestSizeY);
        //        //m_objSampleComparedROI.SaveImage("D:\\TS\\2.m_objSampleComparedROI.bmp");

        //        m_objMatcherImage.CopyTo(ref m_objMatcherThresholdImage);
        //        m_objTemplateImageMatcherROI.AttachImage(m_objMatcherThresholdImage);
        //        m_objTemplateImageMatcherROI.LoadROISetting(0, 0, intSmallestSizeX, intSmallestSizeY);
        //        EasyImage.Threshold(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, m_intMarkPixelThreshold);
        //        EasyImage.PixelCount(m_objTemplateImageMatcherROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intTemplateHighPixel);
        //        if (blnDebugImage)
        //            m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\2.m_objTemplateImageMatcherROI.bmp");

        //        float fStdDevSample = 0;
        //        float fMeanSample = 0;
        //        float fStdDevTemplate = 0;
        //        float fMeanTemplate = 0;
        //        EasyImage.PixelStdDev(m_objSampleThresholdROI.ref_ROI, out fStdDevSample, out fMeanSample);
        //        EasyImage.PixelStdDev(m_objTemplateImageMatcherROI.ref_ROI, out fStdDevTemplate, out fMeanTemplate);
        //        if (intSampleHighPixel > intTemplateHighPixel)
        //        {
        //            m_strTrack2 += "Brigh,";

        //            EasyImage.Dilate(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, 2);
        //            if (blnDebugImage)
        //                m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\31.m_objTemplateImageMatcherROI.bmp");

        //            EasyImage.Oper(EArithmeticLogicOperation.Subtract, m_objSampleThresholdROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, m_objSampleComparedROI.ref_ROI);

        //            if (blnDebugImage)
        //                m_objSampleComparedROI.SaveImage("D:\\TS\\41.m_objSampleComparedROI.bmp");
        //            EasyImage.PixelCount(m_objSampleComparedROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intHighPixel);

        //            if (intTemplateHighPixel == 0)
        //                fImageMatchScore = 1;
        //            else
        //                fImageMatchScore = Math.Max(0, (float)(intTemplateHighPixel - intHighPixel) / intTemplateHighPixel);

        //            if (!blnWantSecondThreshold)
        //            {
        //                if (fImageMatchScore == 0)
        //                {
        //                    blnWantSecondThreshold = true;
        //                }
        //            }
        //            else
        //            {
        //                blnWantSecondThreshold = false;
        //            }
        //        }
        //        else
        //        {
        //            if (blnDebugImage)
        //                m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\22.m_objTemplateImageMatcherROI.bmp");
        //            //2020-02-04 ZJYEOH : Difference Standard Deviation between Template and Sample is low when both ROI pixel distribution almost same
        //            if (Math.Abs(fStdDevTemplate - fStdDevSample) < m_intStdDevTol)
        //            {
        //                EasyImage.Erode(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, 2);
        //                m_strTrack2 += "Dark2,";
        //            }
        //            else
        //            {
        //                EasyImage.Erode(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, 0);
        //                m_strTrack2 += "Dark0,";
        //            }

        //            if (blnDebugImage)
        //                m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\32.m_objTemplateImageMatcherROI.bmp");
        //            EasyImage.PixelCount(m_objTemplateImageMatcherROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intTemplateHighPixel);

        //            EasyImage.Oper(EArithmeticLogicOperation.Subtract, m_objTemplateImageMatcherROI.ref_ROI, m_objSampleThresholdROI.ref_ROI, m_objSampleComparedROI.ref_ROI);
        //            if (blnDebugImage)
        //                m_objSampleComparedROI.SaveImage("D:\\TS\\42.m_objSampleComparedROI.bmp");
        //            EasyImage.PixelCount(m_objSampleComparedROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intHighPixel);

        //            if (intTemplateHighPixel == 0)
        //                fImageMatchScore = 1;
        //            else
        //                fImageMatchScore = Math.Max(0, (float)(intTemplateHighPixel - intHighPixel) / intTemplateHighPixel);

        //        }

        //        if (m_fImageMatchScore < fImageMatchScore)
        //            m_fImageMatchScore = fImageMatchScore;

        //        if (!blnWantSecondThreshold)
        //            break;
        //    }


        //    return true;
        //}

        public bool TemplateImageMatching_SRM1(ROI objSampleROI, int intTemplateNo, int intMatcherIndex)
        {
            bool blnDebugImage = false;
            EBW8 objBW8 = new EBW8(125);
            int intHighPixel = 0;
            int intMediamPixel = 0;
            int intLowPixel = 0;
            int intTemplateLowPixel = 0;
            int intTemplateHighPixel = 0;
            int intSampleLowPixel = 0;
            int intSampleHighPixel = 0;

            int intSmallestSizeX = Math.Min(objSampleROI.ref_ROIWidth, m_arrMatcherImage[intTemplateNo].ref_objMainImage.Width);
            int intSmallestSizeY = Math.Min(objSampleROI.ref_ROIHeight, m_arrMatcherImage[intTemplateNo].ref_objMainImage.Height);

            if ((m_objTempSampleImage.ref_intImageWidth != m_arrMatcherThresholdImage[intTemplateNo].ref_intImageWidth) ||
                (m_objTempSampleImage.ref_intImageHeight != m_arrMatcherThresholdImage[intTemplateNo].ref_intImageHeight))
            {
                m_objTempSampleImage.SetImageSize(m_arrMatcherThresholdImage[intTemplateNo].ref_intImageWidth, m_arrMatcherThresholdImage[intTemplateNo].ref_intImageHeight);
            }

            m_objSampleThresholdROI.AttachImage(m_objTempSampleImage);
            m_objSampleThresholdROI.LoadROISetting(0, 0, objSampleROI.ref_ROIWidth, objSampleROI.ref_ROIHeight);

            if (blnDebugImage)
                m_objSampleThresholdROI.SaveImage("D:\\TS\\0.m_objSampleThresholdROI.bmp");

            float fAngle = m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).Angle + intMatcherIndex * 90;
            EasyImage.ScaleRotate(objSampleROI.ref_ROI, objSampleROI.ref_ROI.Width / 2f, objSampleROI.ref_ROI.Height / 2f,
                                 m_objSampleThresholdROI.ref_ROIWidth / 2f, m_objSampleThresholdROI.ref_ROIHeight / 2f, 1, 1,
                                 fAngle, m_objSampleThresholdROI.ref_ROI, 0);

            m_objSampleThresholdROI.LoadROISetting(0, 0, intSmallestSizeX, intSmallestSizeY);
#if (Debug_2_12 || Release_2_12)
            //EasyImage.Threshold(m_objSampleThresholdROI.ref_ROI, m_objSampleThresholdROI.ref_ROI, (uint)m_intMarkPixelThreshold);
            EasyImage.Threshold(m_objSampleThresholdROI.ref_ROI, m_objSampleThresholdROI.ref_ROI, (uint)m_arrTemplateMarkThreshold[intTemplateNo]);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            //EasyImage.Threshold(m_objSampleThresholdROI.ref_ROI, m_objSampleThresholdROI.ref_ROI, m_intMarkPixelThreshold);
            EasyImage.Threshold(m_objSampleThresholdROI.ref_ROI, m_objSampleThresholdROI.ref_ROI, m_arrTemplateMarkThreshold[intTemplateNo]);
#endif

            if (blnDebugImage)
                m_objSampleThresholdROI.SaveImage("D:\\TS\\1.m_objSampleThresholdROI.bmp");
            EasyImage.PixelCount(m_objSampleThresholdROI.ref_ROI, objBW8, objBW8, out intSampleLowPixel, out intMediamPixel, out intSampleHighPixel);

            if ((m_objTempComparedImage.ref_intImageWidth != m_arrMatcherThresholdImage[intTemplateNo].ref_intImageWidth) ||
                (m_objTempComparedImage.ref_intImageHeight != m_arrMatcherThresholdImage[intTemplateNo].ref_intImageHeight))
            {
                m_objTempComparedImage.SetImageSize(m_arrMatcherThresholdImage[intTemplateNo].ref_intImageWidth, m_arrMatcherThresholdImage[intTemplateNo].ref_intImageHeight);
            }

            m_objSampleComparedROI.AttachImage(m_objTempComparedImage);
            m_objSampleComparedROI.LoadROISetting(0, 0, intSmallestSizeX, intSmallestSizeY);
            //m_objSampleComparedROI.SaveImage("D:\\TS\\2.m_objSampleComparedROI.bmp");

            if ((m_objMatcherThresholdImage.ref_intImageWidth != m_arrMatcherThresholdImage[intTemplateNo].ref_intImageWidth) ||
                (m_objMatcherThresholdImage.ref_intImageHeight != m_arrMatcherThresholdImage[intTemplateNo].ref_intImageHeight))
            {
                m_objMatcherThresholdImage.SetImageSize(m_arrMatcherThresholdImage[intTemplateNo].ref_intImageWidth, m_arrMatcherThresholdImage[intTemplateNo].ref_intImageHeight);
            }

            m_arrMatcherThresholdImage[intTemplateNo].CopyTo(ref m_objMatcherThresholdImage);
            m_objTemplateImageMatcherROI.AttachImage(m_objMatcherThresholdImage);
            m_objTemplateImageMatcherROI.LoadROISetting(0, 0, intSmallestSizeX, intSmallestSizeY);
            //EasyImage.Threshold(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, m_intMarkPixelThreshold);
            EasyImage.PixelCount(m_objTemplateImageMatcherROI.ref_ROI, objBW8, objBW8, out intTemplateLowPixel, out intMediamPixel, out intTemplateHighPixel);
            if (blnDebugImage)
                m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\2.m_objTemplateImageMatcherROI.bmp");

            float fStdDevSample = 0;
            float fMeanSample = 0;
            float fStdDevTemplate = 0;
            float fMeanTemplate = 0;
            EasyImage.PixelStdDev(m_objSampleThresholdROI.ref_ROI, out fStdDevSample, out fMeanSample);
            EasyImage.PixelStdDev(m_objTemplateImageMatcherROI.ref_ROI, out fStdDevTemplate, out fMeanTemplate);
            if (false && intTemplateLowPixel > intTemplateHighPixel)
            {
                if (intSampleLowPixel < intTemplateLowPixel)
                {
                    m_strTrack2 += "Brigh,";
                    if (blnDebugImage)
                    {
                        m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\31a.m_objTemplateImageMatcherROI.bmp");
                    }

                    EasyImage.Dilate(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, 2);
                    if (blnDebugImage)
                    {
                        m_objSampleThresholdROI.SaveImage("D:\\TS\\30.m_objSampleThresholdROI.bmp");
                        m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\31b.m_objTemplateImageMatcherROI.bmp");
                    }

                    EasyImage.Oper(EArithmeticLogicOperation.Subtract, m_objSampleThresholdROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, m_objSampleComparedROI.ref_ROI);

                    if (blnDebugImage)
                        m_objSampleComparedROI.SaveImage("D:\\TS\\41.m_objSampleComparedROI.bmp");
                    EasyImage.PixelCount(m_objSampleComparedROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intHighPixel);

                    // 2020 05 11 - intTemplateHighPixel must at least 1 bcos Not allow devide 0.
                    if (intTemplateHighPixel == 0)
                        intTemplateHighPixel = 1;

                    //if (intTemplateHighPixel == 0)
                    //    m_fImageMatchScore = 1;
                    //else
                    m_fImageMatchScore = Math.Max(0, (float)(intTemplateLowPixel - intHighPixel) / intTemplateLowPixel);
                }
                else
                {
                    if (blnDebugImage)
                        m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\22.m_objTemplateImageMatcherROI.bmp");
                    //2020-02-04 ZJYEOH : Difference Standard Deviation between Template and Sample is low when both ROI pixel distribution almost same
                    if (Math.Abs(fStdDevTemplate - fStdDevSample) < m_intStdDevTol)
                    {
                        EasyImage.Erode(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, 2);
                        m_strTrack2 += "Dark2,";
                    }
                    else
                    {
                        EasyImage.Erode(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, 0);
                        m_strTrack2 += "Dark0,";
                    }

                    if (blnDebugImage)
                        m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\32.m_objTemplateImageMatcherROI.bmp");
                    EasyImage.PixelCount(m_objTemplateImageMatcherROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intTemplateHighPixel);

                    EasyImage.Oper(EArithmeticLogicOperation.Subtract, m_objTemplateImageMatcherROI.ref_ROI, m_objSampleThresholdROI.ref_ROI, m_objSampleComparedROI.ref_ROI);
                    if (blnDebugImage)
                        m_objSampleComparedROI.SaveImage("D:\\TS\\42.m_objSampleComparedROI.bmp");
                    EasyImage.PixelCount(m_objSampleComparedROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intHighPixel);

                    if (intTemplateLowPixel == 0)
                        m_fImageMatchScore = 1;
                    else
                        m_fImageMatchScore = Math.Max(0, (float)(intTemplateLowPixel - intHighPixel) / intTemplateLowPixel);

                }
            }
            else
            {
                if (intSampleHighPixel > intTemplateHighPixel)
                {
                    // 2020 05 11 - CCENG: if user learn blank unit, mean white area is 10% or less.
                    //                   : when this scenario happen, we not using "what is score", but using yes or not only.
                    if (((float)intTemplateHighPixel / (float)(intTemplateLowPixel + intTemplateHighPixel)) < m_fMarkAreaBelowPercent)   // if template fore area cover back area 10% only
                    {
                        if (((float)intSampleHighPixel / (float)(intSampleLowPixel + intSampleHighPixel)) < m_fMarkAreaBelowPercent)   // if sample fore area cover back area 10% only.
                        {
                            m_fImageMatchScore = 1;
                        }
                        else
                        {
                            m_fImageMatchScore = 0;
                        }
                    }
                    else
                    {

                        m_strTrack2 += "Brigh,";

                        EasyImage.Dilate(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, 2);
                        if (blnDebugImage)
                            m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\31.m_objTemplateImageMatcherROI.bmp");

                        EasyImage.Oper(EArithmeticLogicOperation.Subtract, m_objSampleThresholdROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, m_objSampleComparedROI.ref_ROI);

                        if (blnDebugImage)
                            m_objSampleComparedROI.SaveImage("D:\\TS\\41.m_objSampleComparedROI.bmp");
                        EasyImage.PixelCount(m_objSampleComparedROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intHighPixel);

                        // 2020 05 11 - intTemplateHighPixel must at least 1 bcos Not allow devide 0.
                        if (intTemplateHighPixel == 0)
                            intTemplateHighPixel = 1;

                        //if (intTemplateHighPixel == 0)
                        //    m_fImageMatchScore = 1;
                        //else
                        m_fImageMatchScore = Math.Max(0, (float)(intTemplateHighPixel - intHighPixel) / intTemplateHighPixel);
                    }
                }
                else
                {
                    if (blnDebugImage)
                    {
                        m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\22.m_objTemplateImageMatcherROI.bmp");
                        m_objSampleThresholdROI.SaveImage("D:\\TS\\22.m_objSampleThresholdROI.bmp");
                    }
                    //2020-02-04 ZJYEOH : Difference Standard Deviation between Template and Sample is low when both ROI pixel distribution almost same
                    if (Math.Abs(fStdDevTemplate - fStdDevSample) < m_intStdDevTol)
                    {
                        //EasyImage.Erode(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, 2);
                        EasyImage.Dilate(m_objSampleThresholdROI.ref_ROI, m_objSampleThresholdROI.ref_ROI, 2);
                        m_strTrack2 += "Dark2,";
                    }
                    else
                    {
                        EasyImage.Dilate(m_objSampleThresholdROI.ref_ROI, m_objSampleThresholdROI.ref_ROI, 0);
                        m_strTrack2 += "Dark0,";
                    }

                    if (blnDebugImage)
                    {
                        m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\32.m_objTemplateImageMatcherROI.bmp");
                        m_objSampleThresholdROI.SaveImage("D:\\TS\\32.m_objSampleThresholdROI.bmp");
                    }
                    EasyImage.PixelCount(m_objTemplateImageMatcherROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intTemplateHighPixel);

                    EasyImage.Oper(EArithmeticLogicOperation.Subtract, m_objTemplateImageMatcherROI.ref_ROI, m_objSampleThresholdROI.ref_ROI, m_objSampleComparedROI.ref_ROI);
                    if (blnDebugImage)
                        m_objSampleComparedROI.SaveImage("D:\\TS\\42.m_objSampleComparedROI.bmp");
                    EasyImage.PixelCount(m_objSampleComparedROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intHighPixel);

                    if (intTemplateHighPixel == 0)
                        m_fImageMatchScore = 1;
                    else
                        m_fImageMatchScore = Math.Max(0, (float)(intTemplateHighPixel - intHighPixel) / intTemplateHighPixel);

                }
            }

            /*
             *  if (intSampleHighPixel > intTemplateHighPixel)
            {
                m_strTrack2 += "Brigh,";

                EasyImage.Dilate(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, 2);
                if (blnDebugImage)
                    m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\31.m_objTemplateImageMatcherROI.bmp");

                EasyImage.Oper(EArithmeticLogicOperation.Subtract, m_objSampleThresholdROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, m_objSampleComparedROI.ref_ROI);

                if (blnDebugImage)
                    m_objSampleComparedROI.SaveImage("D:\\TS\\41.m_objSampleComparedROI.bmp");
                EasyImage.PixelCount(m_objSampleComparedROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intHighPixel);

                if (intTemplateHighPixel == 0)
                    m_fImageMatchScore = 1;
                else
                    m_fImageMatchScore = Math.Max(0, (float)(intTemplateHighPixel - intHighPixel) / intTemplateHighPixel);
            }
            else
            {
                if (blnDebugImage)
                    m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\22.m_objTemplateImageMatcherROI.bmp");
                //2020-02-04 ZJYEOH : Difference Standard Deviation between Template and Sample is low when both ROI pixel distribution almost same
                if (Math.Abs(fStdDevTemplate - fStdDevSample) < m_intStdDevTol)
                {
                    EasyImage.Erode(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, 2);
                    m_strTrack2 += "Dark2,";
                }
                else
                {
                    EasyImage.Erode(m_objTemplateImageMatcherROI.ref_ROI, m_objTemplateImageMatcherROI.ref_ROI, 0);
                    m_strTrack2 += "Dark0,";
                }

                if (blnDebugImage)
                    m_objTemplateImageMatcherROI.SaveImage("D:\\TS\\32.m_objTemplateImageMatcherROI.bmp");
                EasyImage.PixelCount(m_objTemplateImageMatcherROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intTemplateHighPixel);

                EasyImage.Oper(EArithmeticLogicOperation.Subtract, m_objTemplateImageMatcherROI.ref_ROI, m_objSampleThresholdROI.ref_ROI, m_objSampleComparedROI.ref_ROI);
                if (blnDebugImage)
                    m_objSampleComparedROI.SaveImage("D:\\TS\\42.m_objSampleComparedROI.bmp");
                EasyImage.PixelCount(m_objSampleComparedROI.ref_ROI, objBW8, objBW8, out intLowPixel, out intMediamPixel, out intHighPixel);

                if (intTemplateHighPixel == 0)
                    m_fImageMatchScore = 1;
                else
                    m_fImageMatchScore = Math.Max(0, (float)(intTemplateHighPixel - intHighPixel) / intTemplateHighPixel);

            }
            */

            return true;
        }


        /// <summary>
        /// Build line gauge and collects gauge poisition points
        /// </summary>
        /// <param name="objROI">seal ROI</param>
        /// <param name="objGauge">line gauge</param>
        /// <param name="intLeft">gauge position 1 = near seal, 0 = far seal</param>
        /// <param name="fCalibY">calibration value</param>
        /// <param name="blnLearn">if it is under learning procedure, seal height definition is not set yet</param>
        /// <returns>gauge position points in arraylist</returns>
        private ArrayList BuildGauge(ROI objROI, LGauge objGauge, int intLeft, float fCalibY, bool blnLearn)
        {
            // Set Black to White
            //objGauge.SetGaugeMeasurement(0); // 2019-12-26 ZJYEOH : Should not fix to 0(Black to White) because User can set Black to White or White to Black

            objGauge.Measure(objROI);
            float fDistance = 0;
            ArrayList arr = new ArrayList();

            // Continue on the point above and change to black -> white              
            // To check whether is there double line at side border. It may caused by hole / seal block
            LGauge objTempGauge = new LGauge();
            if (intLeft == 0)
                objGauge.DuplicateTopGauge(ref objTempGauge, objROI, fCalibY / 10);
            else
                objGauge.DuplicateBottomGauge2(ref objTempGauge, objROI, m_arrBlackBlobs[1]);

            m_intBuildCount++;


            if (objGauge.ref_ObjectScore >= 50)
            {
                // Set Black to White
                //objTempGauge.SetGaugeMeasurement(0); // 2019-12-26 ZJYEOH : Should not fix to 0(Black to White) because User can set Black to White or White to Black
                objTempGauge.Measure(objROI);

                if (objTempGauge.ref_ObjectScore < 50) // unable to find another side seal border
                    return null;
            }
            else if (m_intBuildCount == 1) // for the first time if unable to find seal border, the tape may be shift to one way, give 1 chance to search for another side
            {
                arr = BuildGauge(objROI, objTempGauge, intLeft, fCalibY, blnLearn);
                if (arr == null)
                    return null;
            }
            else
                return null;

            if (!blnLearn)
                fDistance = Math.Abs(objGauge.ref_ObjectCenterY - objTempGauge.ref_ObjectCenterY);

            arr.Add(new PointF(objGauge.ref_ObjectCenterX, objGauge.ref_ObjectCenterY));
            arr.Add(new PointF(objTempGauge.ref_ObjectCenterX, objTempGauge.ref_ObjectCenterY));
            return arr;
        }

        ///// <summary>
        ///// Build black blobs objects in ROI to inspect black on white area (Seal Line)
        ///// </summary>
        ///// <param name="arrROIs">ROI</param>
        //private void CreateBlobs()
        //{
        //    m_arrBlackBlobs.Clear();

        //    for (int i = 0; i < 3; i++)
        //    {
        //        Blobs objBlackBlob = new Blobs();
        //        objBlackBlob.SetConnexity(m_intConnexity);
        //        objBlackBlob.SetClassSelection(1);
        //        objBlackBlob.ref_intFeature = m_intFeature;   // area and object center

        //        switch (i)
        //        {
        //            case 0:
        //                objBlackBlob.SetObjectAreaRange(0, 99999);
        //                objBlackBlob.ref_intThreshold = -4;
        //                break;
        //            case 1:
        //                objBlackBlob.SetObjectAreaRange(0, 99999);
        //                objBlackBlob.ref_intThreshold = -4;
        //                break;
        //            case 2:
        //                objBlackBlob.SetObjectAreaRange(0, 99999);
        //                objBlackBlob.ref_intThreshold = -4;
        //                break;
        //        }

        //        m_arrBlackBlobs.Add(objBlackBlob);
        //    }
        //}

        /// <summary>
        /// Build black blobs objects in ROI to inspect black on white area (Seal Line)
        /// </summary>
        /// <param name="arrROIs">ROI</param>
        private void CreateBlobs()
        {
            m_arrBlackBlobs.Clear();

            for (int i = 0; i < 2; i++)
            {
                m_arrBlackBlobs.Add(new EBlobs());
            }
        }

        /// <summary>
        /// Inspect m_arrBlackBlobs by building black blobs and get its area to store in m_arrAreaResults
        /// </summary>
        /// <param name="arrROIs">Far and Near Seal ROI</param>
        /// <param name="fCalibY">calibration value</param>
        //private bool InspectFarAndNearSealArea(List<List<ROI>> arrROIs, float fCalibY)
        //{
        //    string strSealName = "Seal 1";
        //    bool blnResult = true;
        //    for (int i = 0; i < 2; i++)
        //    {
        //        if (i == 1)
        //            strSealName = "Seal 2";

        //        arrROIs[i + 1][0].ref_ROIPositionX = arrROIs[i + 1][0].ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
        //        arrROIs[i + 1][0].ref_ROIPositionY = arrROIs[i + 1][0].ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);

        //        if (m_objTempImage == null)
        //            m_objTempImage = new ImageDrawing(true, arrROIs[i + 1][0].ref_ROI.TopParent.Width, arrROIs[i + 1][0].ref_ROI.TopParent.Height);
        //        if (m_objTempROI == null)
        //            m_objTempROI = new ROI();

        //        arrROIs[i + 1][0].CopyToTopParentImage(ref m_objTempImage);

        //        int intOriX = arrROIs[i + 1][0].ref_ROIPositionX;
        //        int intOriY = arrROIs[i + 1][0].ref_ROIPositionY;
        //        int intOriWidth = arrROIs[i + 1][0].ref_ROIWidth;
        //        int intOriHeight = arrROIs[i + 1][0].ref_ROIHeight;
        //        arrROIs[i + 1][0].LoadROISetting(intOriX - m_intSealEdgeSensitivity,
        //                                    intOriY - m_intSealEdgeSensitivity,
        //                                    intOriWidth + m_intSealEdgeSensitivity * 2,
        //                                    intOriHeight);

        //        m_objTempROI.AttachImage(m_objTempImage);
        //        m_objTempROI.LoadROISetting(arrROIs[i + 1][0].ref_ROITotalX, arrROIs[i + 1][0].ref_ROITotalY, arrROIs[i + 1][0].ref_ROIWidth, arrROIs[i + 1][0].ref_ROIHeight);

        //        //m_objTempROI.ref_ROI.Save("D:\\TS\\Before.bmp");
        //        EasyImage.CloseBox(arrROIs[i + 1][0].ref_ROI, m_objTempROI.ref_ROI, m_intSealEdgeSensitivity);
        //        //m_objTempROI.ref_ROI.Save("D:\\TS\\After.bmp");

        //        arrROIs[i + 1][0].LoadROISetting(intOriX, intOriY, intOriWidth, intOriHeight);
        //        m_objTempROI.LoadROISetting(arrROIs[i + 1][0].ref_ROITotalX, arrROIs[i + 1][0].ref_ROITotalY, arrROIs[i + 1][0].ref_ROIWidth, arrROIs[i + 1][0].ref_ROIHeight);

        //        //m_objTempROI.ref_ROI.Save("D:\\TS\\After2.bmp");

        //        //m_arrBlackBlobs[i].BuildObjects_Filter_GetElement(arrROIs[i + 1][0], true, true, 0, m_arrBlackBlobs[i].ref_intAbsoluteThreshold, m_arrBlackBlobs[i].ref_intMinAreaLimit, m_arrBlackBlobs[i].ref_intMaxAreaLimit, false, 0x0F);
        //        m_arrBlackBlobs[i].BuildObjects_Filter_GetElement(m_objTempROI, true, true, 0, m_arrBlackBlobs[i].ref_intAbsoluteThreshold, m_arrBlackBlobs[i].ref_intMinAreaLimit, m_arrBlackBlobs[i].ref_intMaxAreaLimit, false, 0x0F);

        //        //m_arrBlackBlobs[i].BuildObjectsAndHole(arrROIs[i + 1][0], false, false);
        //        m_arrFailBlobsIndex.Add(new List<int>());
        //        m_arrFailBlobsHoleIndex.Add(new List<int>());
        //        m_arrPassBlobsIndex.Add(new List<int>());

        //        List<int> arrBlackArea = new List<int>();
        //        for (int j = 0; j < m_arrBlackBlobs[i].ref_intNumSelectedObject; j++)
        //        {
        //            int intHoleArea = 0;
        //            int intHoleIndex = 0;

        //            m_arrAreaResults[i] = new float();
        //            arrBlackArea.Add(m_arrBlackBlobs[i].ref_arrArea[j]);
        //            if ((m_intFailOptionMask & 0x02) > 0)
        //            {
        //                if (i == 0)
        //                {
        //                    // Skip if hole is found
        //                    if (m_arrBlackBlobs[i].CheckHole(j, m_intHoleMinArea1, ref intHoleArea, ref intHoleIndex))
        //                    {
        //                        if (i == 0)
        //                        {
        //                            m_FailBubble[1] = 0;
        //                            m_blnFailSeal1 = true;
        //                        }
        //                        else if (i == 1)
        //                        {
        //                            m_FailBubble[0] = 0;
        //                            m_blnFailSeal2 = true;
        //                        }
        //                        m_FailBubble[i] = (intHoleArea / (fCalibY * fCalibY));
        //                        m_strErrorMessage += "*Bubble is present in " + strSealName + " : Set = " + (m_intHoleMinArea1 / (fCalibY * fCalibY)).ToString("f5") +
        //                                " mm^2,   Result = " + (intHoleArea / (fCalibY * fCalibY)).ToString("f5") + " mm^2";
        //                        m_intSealFailMask |= 0x04;
        //                        m_arrFailBlobsIndex[i].Add(j);
        //                        m_arrFailBlobsHoleIndex[i].Add(intHoleIndex);
        //                        blnResult = false;
        //                    }
        //                    else
        //                    {
        //                        //// Fill the hole with black
        //                        //float fCenterX = 0, fCenterY = 0, fWidth = 0, fHeight = 0;
        //                        ////m_arrBlackBlobs[i].GetSelectedListBlobsLimitCenterX(ref fCenterX);
        //                        ////m_arrBlackBlobs[i].GetSelectedListBlobsLimitCenterY(ref fCenterY);
        //                        ////m_arrBlackBlobs[i].GetSelectedListBlobsWidth(ref fWidth);
        //                        ////m_arrBlackBlobs[i].GetSelectedListBlobsHeight(ref fHeight);
        //                        //fCenterX = m_arrBlackBlobs[i].ref_arrLimitCenterX[j];
        //                        //fCenterY = m_arrBlackBlobs[i].ref_arrLimitCenterY[j];
        //                        //fWidth = m_arrBlackBlobs[i].ref_arrWidth[j];
        //                        //fHeight = m_arrBlackBlobs[i].ref_arrHeight[j];
        //                        //EBW8 px = new EBW8();
        //                        //px.Value = 0;

        //                        //float fLimitX = fCenterX + fWidth / 2;
        //                        //float fLimitY = fCenterY + fHeight / 2;
        //                        //for (int x = (int)Math.Floor(fCenterX - fWidth / 2); x < fLimitX; x++)
        //                        //{
        //                        //    for (int y = (int)Math.Floor(fCenterY - fHeight / 2); y < fLimitY; y++)
        //                        //    {
        //                        //        arrROIs[i + 1][0].SetPixel(x, y, px);
        //                        //    }
        //                        //}

        //                        m_arrPassBlobsIndex[i].Add(j);
        //                    }
        //                }
        //                else if (i == 1)
        //                {
        //                    if (m_arrBlackBlobs[i].CheckHole(j, m_intHoleMinArea2, ref intHoleArea, ref intHoleIndex))
        //                    {
        //                        if (i == 0)
        //                        {
        //                            m_FailBubble[1] = 0;
        //                            m_blnFailSeal1 = true;
        //                        }
        //                        else if (i == 1)
        //                        {
        //                            m_FailBubble[0] = 0;
        //                            m_blnFailSeal2 = true;
        //                        }
        //                        m_FailBubble[i] = (intHoleArea / (fCalibY * fCalibY));
        //                        m_strErrorMessage += "*Bubble is present in " + strSealName + " : Set = " + (m_intHoleMinArea2 / (fCalibY * fCalibY)).ToString("f5") +
        //                                " mm^2,   Result = " + (intHoleArea / (fCalibY * fCalibY)).ToString("f5") + " mm^2";
        //                        m_intSealFailMask |= 0x04;
        //                        m_arrFailBlobsIndex[i].Add(j);
        //                        m_arrFailBlobsHoleIndex[i].Add(intHoleIndex);
        //                        blnResult = false;
        //                    }
        //                    else
        //                    {
        //                        //// Fill the hole with black
        //                        //float fCenterX = 0, fCenterY = 0, fWidth = 0, fHeight = 0;
        //                        ////m_arrBlackBlobs[i].GetSelectedListBlobsLimitCenterX(ref fCenterX);
        //                        ////m_arrBlackBlobs[i].GetSelectedListBlobsLimitCenterY(ref fCenterY);
        //                        ////m_arrBlackBlobs[i].GetSelectedListBlobsWidth(ref fWidth);
        //                        ////m_arrBlackBlobs[i].GetSelectedListBlobsHeight(ref fHeight);
        //                        //fCenterX = m_arrBlackBlobs[i].ref_arrLimitCenterX[j];
        //                        //fCenterY = m_arrBlackBlobs[i].ref_arrLimitCenterY[j];
        //                        //fWidth = m_arrBlackBlobs[i].ref_arrWidth[j];
        //                        //fHeight = m_arrBlackBlobs[i].ref_arrHeight[j];
        //                        //EBW8 px = new EBW8();
        //                        //px.Value = 0;

        //                        //float fLimitX = fCenterX + fWidth / 2;
        //                        //float fLimitY = fCenterY + fHeight / 2;
        //                        //for (int x = (int)Math.Floor(fCenterX - fWidth / 2); x < fLimitX; x++)
        //                        //{
        //                        //    for (int y = (int)Math.Floor(fCenterY - fHeight / 2); y < fLimitY; y++)
        //                        //    {
        //                        //        arrROIs[i + 1][0].SetPixel(x, y, px);
        //                        //    }
        //                        //}

        //                        m_arrPassBlobsIndex[i].Add(j);
        //                    }

        //                }
        //            }
        //            //m_arrBlackBlobs[i].SetListBlobsToNext();
        //        }

        //        int intTotalBlackArea = 0;
        //        for (int k = 0; k < arrBlackArea.Count; k++)
        //        {
        //            intTotalBlackArea += arrBlackArea[k];
        //        }

        //        m_strTrack += ", Seal " + (i + 1).ToString() + " Blob Area=" + intTotalBlackArea.ToString();


        //        if (intTotalBlackArea == 0)
        //        {
        //            if (i == 0)
        //                m_blnFailSeal1 = true;
        //            else if (i == 1)
        //                m_blnFailSeal2 = true;

        //            m_strErrorMessage += "No seal was found in " + strSealName + ". Minimum Build Blob Area Set = " + (m_arrBlackBlobs[i].ref_intMinAreaLimit / (fCalibY * fCalibY)).ToString("f5") + " mm^2";
        //            m_intSealFailMask |= 0x08;
        //            blnResult = false;
        //        }
        //        else
        //        {
        //            if ((m_intFailOptionMask & 0x20) > 0)
        //            {
        //                //check for Broken seal, calculate gap between 2 blobs
        //                if (arrBlackArea.Count > 1)
        //                {
        //                    float fMinY = 0;
        //                    int intMinPostYNo = 0;
        //                    for (int m = 0; m < 2; m++)
        //                    {
        //                        if (m == 0)
        //                            fMinY = m_arrBlackBlobs[i].ref_arrLimitCenterY[m];

        //                        if (m_arrBlackBlobs[i].ref_arrLimitCenterY[m] < fMinY)
        //                        {
        //                            fMinY = m_arrBlackBlobs[i].ref_arrLimitCenterY[m];
        //                            intMinPostYNo = m;
        //                        }
        //                    }

        //                    PointF pCenter = new PointF(m_arrBlackBlobs[i].ref_arrLimitCenterX[intMinPostYNo], m_arrBlackBlobs[i].ref_arrLimitCenterY[intMinPostYNo]);
        //                    float fHeight = m_arrBlackBlobs[i].ref_arrHeight[intMinPostYNo];
        //                    float fBorderPositionY = pCenter.Y + (fHeight / 2);

        //                    int intMaxPosYNo = 0;
        //                    if (intMinPostYNo == 0)
        //                        intMaxPosYNo = 1;

        //                    pCenter = new PointF(m_arrBlackBlobs[i].ref_arrLimitCenterX[intMaxPosYNo], m_arrBlackBlobs[i].ref_arrLimitCenterY[intMaxPosYNo]);
        //                    fHeight = m_arrBlackBlobs[i].ref_arrHeight[intMaxPosYNo];
        //                    float fSealTopPositionY = pCenter.Y - (fHeight / 2);

        //                    float fGapDistance = fSealTopPositionY - fBorderPositionY;

        //                    // if the distance gap is larger than gap between broken seal
        //                    if (fSealTopPositionY - (m_fLineWidthAverage[2] / 2) < fBorderPositionY)
        //                    {
        //                        if (i == 0)
        //                        {
        //                            m_FailBrokenSeal[1] = 0;
        //                            m_blnFailSeal1 = true;
        //                        }
        //                        else if (i == 1)
        //                        {
        //                            m_FailBrokenSeal[0] = 0;
        //                            m_blnFailSeal2 = true;
        //                        }
        //                        m_FailBrokenSeal[i] = (intTotalBlackArea / (fCalibY * fCalibY));
        //                        //m_strErrorMessage += "*" + strSealName + " Broken Seal: Total Seal Area = " +
        //                        //    (intTotalBlackArea / (fCalibY * fCalibY)).ToString("f5") +
        //                        //   " mm^2,   First Blob area = " + arrBlackArea[0] / (fCalibY * fCalibY) + " mm^2,   Second Blob area = " + arrBlackArea[1] / (fCalibY * fCalibY) + " mm^2";

        //                        m_strErrorMessage += "*" + strSealName + " Broken Seal: Smallest Blob area = " + arrBlackArea[1] / (fCalibY * fCalibY) + " mm^2";
        //                        m_intSealFailMask |= 0x08;
        //                        blnResult = false;
        //                    }
        //                }
        //            }

        //            ////fail upper tolerance
        //            //if (intTotalBlackArea > (m_intTemplateAreaAVG[i] + m_arrSealAreaTolerance[i]))
        //            //{
        //            //    m_strErrorMessage += "*" + strSealName + " Area Overseal (More than Max tolerance): Seal" + i + " Area tolerance = " +
        //            //        + m_arrSealAreaTolerance[i] / (fCalibY * fCalibY) + ",   Learnt Template area + area tolerance = " +
        //            //        ((m_intTemplateAreaAVG[i] + m_arrSealAreaTolerance[i]) / (fCalibY * fCalibY)).ToString("f5") +
        //            //       " mm^2,   Result = " + intTotalBlackArea / (fCalibY * fCalibY) + " mm^2";
        //            //    m_intSealFailMask |= 0x10;
        //            //    blnResult = false;
        //            //}

        //            ////fail lower tolerance
        //            //if (intTotalBlackArea < (m_intTemplateAreaAVG[i] - m_arrSealAreaTolerance[i]))
        //            //{
        //            //    m_strErrorMessage += "*" + strSealName + " Insufficient Seal (Less than Min tolerance): Seal" + i + " Area tolerance = " +
        //            //        + m_arrSealAreaTolerance[i] / (fCalibY * fCalibY) + ",   Learnt Template area - area tolerance = " +
        //            //        ((m_intTemplateAreaAVG[i] - m_arrSealAreaTolerance[i]) / (fCalibY * fCalibY)).ToString("f5") +
        //            //        " mm^2,   Result = " + intTotalBlackArea / (fCalibY * fCalibY) + " mm^2";
        //            //    m_intSealFailMask |= 0x20;
        //            //    blnResult = false;
        //            //}
        //        }
        //    }

        //    return blnResult;
        //}

        private bool InspectFarAndNearSealArea_BubbleOrBrokenArea(List<List<ROI>> arrROIs, float fCalibY)
        {
            string strSealName = "Seal 1";
            bool blnResult = true;
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    if (i == 1)
                        strSealName = "Seal 2";

                    arrROIs[i + 1][0].ref_ROIPositionX = arrROIs[i + 1][0].ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
                    arrROIs[i + 1][0].ref_ROIPositionY = arrROIs[i + 1][0].ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);

                    if (m_objTempImage == null)
                        m_objTempImage = new ImageDrawing(true, arrROIs[i + 1][0].ref_ROI.TopParent.Width, arrROIs[i + 1][0].ref_ROI.TopParent.Height);
                    if (m_objTempROI == null)
                        m_objTempROI = new ROI();

                    arrROIs[i + 1][0].CopyToTopParentImage(ref m_objTempImage);

                    int intOriX = arrROIs[i + 1][0].ref_ROIPositionX;
                    int intOriY = arrROIs[i + 1][0].ref_ROIPositionY;
                    int intOriWidth = arrROIs[i + 1][0].ref_ROIWidth;
                    int intOriHeight = arrROIs[i + 1][0].ref_ROIHeight;
                    arrROIs[i + 1][0].LoadROISetting(intOriX - m_intSealEdgeSensitivity,
                                                intOriY - m_intSealEdgeSensitivity,
                                                intOriWidth + m_intSealEdgeSensitivity * 2,
                                                intOriHeight);

                    m_objTempROI.AttachImage(m_objTempImage);
                    m_objTempROI.LoadROISetting(arrROIs[i + 1][0].ref_ROITotalX, arrROIs[i + 1][0].ref_ROITotalY, arrROIs[i + 1][0].ref_ROIWidth, arrROIs[i + 1][0].ref_ROIHeight);

                    //m_objTempROI.ref_ROI.Save("D:\\TS\\Before.bmp");
#if (Debug_2_12 || Release_2_12)
                    EasyImage.CloseBox(arrROIs[i + 1][0].ref_ROI, m_objTempROI.ref_ROI, (uint)m_intSealEdgeSensitivity);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    EasyImage.CloseBox(arrROIs[i + 1][0].ref_ROI, m_objTempROI.ref_ROI, m_intSealEdgeSensitivity);
#endif

                    //m_objTempROI.ref_ROI.Save("D:\\TS\\After.bmp");

                    arrROIs[i + 1][0].LoadROISetting(intOriX, intOriY, intOriWidth, intOriHeight);
                    //m_objTempROI.LoadROISetting(arrROIs[i + 1][0].ref_ROITotalX, arrROIs[i + 1][0].ref_ROITotalY, arrROIs[i + 1][0].ref_ROIWidth, arrROIs[i + 1][0].ref_ROIHeight);
                    int intLineWidthCenterYIndex = 10;
                    int intLineWidthSmallestWidthIndex = 8;
                    if (i == 1)
                    {
                        intLineWidthCenterYIndex = 13;
                        intLineWidthSmallestWidthIndex = 11;
                    }

                    int intSealHeight = (int)Math.Round(m_fLineWidthAverage[intLineWidthSmallestWidthIndex] - m_intSealEdgeTolerance * 2);
                    if (intSealHeight <= 0)
                        intSealHeight = 1;

                    m_objTempROI.LoadROISetting(arrROIs[i + 1][0].ref_ROITotalX,
                                                (int)Math.Round(m_fLineWidthAverage[intLineWidthCenterYIndex] - m_fLineWidthAverage[intLineWidthSmallestWidthIndex] / 2 + m_intSealEdgeTolerance, 0, MidpointRounding.AwayFromZero),
                                                arrROIs[i + 1][0].ref_ROIWidth,
                                                intSealHeight);
                    //(int)Math.Round(m_fLineWidthAverage[intLineWidthSmallestWidthIndex] - m_intSealEdgeTolerance * 2, 0, MidpointRounding.AwayFromZero));

                    //m_objTempROI.ref_ROI.Save("D:\\TS\\After2.bmp");

                    // Build white area (blnBlackOnWhite set to false)
                    //m_arrBlackBlobs[i].BuildObjects_Filter_GetElement(m_objTempROI, false, true, 0, m_arrBlackBlobs[i].ref_intAbsoluteThreshold, m_arrBlackBlobs[i].ref_intMinAreaLimit, m_arrBlackBlobs[i].ref_intMaxAreaLimit, false, 0x0F);
                    m_arrBlackBlobs[i].BuildObjects_Filter_GetElement(m_objTempROI, false, true, 0, m_intSealBrokenAreaThreshold[i], m_arrBlackBlobs[i].ref_intMinAreaLimit, m_arrBlackBlobs[i].ref_intMaxAreaLimit, false, 0x0F);

                    m_arrFailBlobsIndex.Add(new List<int>());
                    m_arrFailBlobsHoleIndex.Add(new List<int>());
                    m_arrPassBlobsIndex.Add(new List<int>());

                    if (m_arrBlackBlobs[i].ref_intNumSelectedObject == 0)
                    {
                        m_FailBubble[i] = 0;
                    }
                    else
                    {
                        // Loop white object (bubble or broken area)
                        for (int j = 0; j < m_arrBlackBlobs[i].ref_intNumSelectedObject; j++)
                        {
                            if ((m_intFailOptionMask & 0x02) > 0)   // Check Bubble is ON
                            {
                                if (i == 0)
                                {
                                    float fBubbleArea = m_arrBlackBlobs[i].ref_arrArea[j] / (fCalibY * fCalibY);
                                    if (m_FailBubble[i] == 0 || m_FailBubble[i] < fBubbleArea)
                                    {
                                        m_FailBubble[i] = fBubbleArea;
                                    }

                                    if (fBubbleArea > m_intHoleMinArea1 / (fCalibY * fCalibY))
                                    {
                                        m_strErrorMessage += "*" + strSealName + " - Fail Broken Area / Bubble : Set = " + (m_intHoleMinArea1 / (fCalibY * fCalibY)).ToString("f5") +
                                                   " mm^2,   Result = " + fBubbleArea.ToString("f5") + " mm^2";

                                        m_blnFailSeal1 = true;
                                        m_intSealFailMask |= 0x04;
                                        m_arrFailBlobsIndex[i].Add(j);
                                        blnResult = false;
                                    }
                                    else
                                        m_arrPassBlobsIndex[i].Add(j);
                                }
                                else if (i == 1)
                                {
                                    float fBubbleArea = m_arrBlackBlobs[i].ref_arrArea[j] / (fCalibY * fCalibY);
                                    if (m_FailBubble[i] == 0 || m_FailBubble[i] < fBubbleArea)
                                    {
                                        m_FailBubble[i] = fBubbleArea;
                                    }

                                    if (fBubbleArea > m_intHoleMinArea2 / (fCalibY * fCalibY))
                                    {
                                        m_strErrorMessage += "*" + strSealName + " - Fail Broken Area / Bubble : Set = " + (m_intHoleMinArea1 / (fCalibY * fCalibY)).ToString("f5") +
                                               " mm^2,   Result = " + fBubbleArea.ToString("f5") + " mm^2";

                                        m_blnFailSeal2 = true;
                                        m_intSealFailMask |= 0x04;
                                        m_arrFailBlobsIndex[i].Add(j);
                                        blnResult = false;
                                    }
                                    else
                                    {
                                        m_arrPassBlobsIndex[i].Add(j);
                                    }

                                }
                            }
                        }
                    }

                    // ------------- Check Broken Gap ----------------------------------------------------------------
                    if ((i == 0 && !m_blnFailSeal1) || i == 1 && !m_blnFailSeal2)
                    {

                        // Build Black Area for seal bar (blnBlackOnWhite set to true)
                        //m_arrBlackBlobs[i].BuildObjects_Filter_GetElement(m_objTempROI, true, true, 0, m_arrBlackBlobs[i].ref_intAbsoluteThreshold, m_arrBlackBlobs[i].ref_intMinAreaLimit, m_arrBlackBlobs[i].ref_intMaxAreaLimit, false, 0x0F);
                        m_arrBlackBlobs[i].BuildObjects_Filter_GetElement(m_objTempROI, true, true, 0, m_intSealBrokenAreaThreshold[i], m_arrBlackBlobs[i].ref_intMinAreaLimit, m_arrBlackBlobs[i].ref_intMaxAreaLimit, false, 0x0F);

                        // Check Seal Broken Gap 
                        if ((m_intFailOptionMask & 0x20) > 0)
                        {
                            if (m_arrBlackBlobs[i].ref_intNumSelectedObject == 1)
                            {
                                m_FailBrokenSeal[i] = 0;
                            }
                            else
                            {
                                //check for Broken seal, calculate gap between 2 blobs
                                if (m_arrBlackBlobs[i].ref_intNumSelectedObject > 1)
                                {
                                    float fMinY = 0;
                                    int intMinPostYNo = 0;
                                    for (int m = 0; m < 2; m++)
                                    {
                                        if (m == 0)
                                            fMinY = m_arrBlackBlobs[i].ref_arrLimitCenterY[m];

                                        if (m_arrBlackBlobs[i].ref_arrLimitCenterY[m] < fMinY)
                                        {
                                            fMinY = m_arrBlackBlobs[i].ref_arrLimitCenterY[m];
                                            intMinPostYNo = m;
                                        }
                                    }

                                    PointF pCenter = new PointF(m_arrBlackBlobs[i].ref_arrLimitCenterX[intMinPostYNo], m_arrBlackBlobs[i].ref_arrLimitCenterY[intMinPostYNo]);
                                    float fHeight = m_arrBlackBlobs[i].ref_arrHeight[intMinPostYNo];
                                    float fBorderPositionY = pCenter.Y + (fHeight / 2);

                                    int intMaxPosYNo = 0;
                                    if (intMinPostYNo == 0)
                                        intMaxPosYNo = 1;

                                    pCenter = new PointF(m_arrBlackBlobs[i].ref_arrLimitCenterX[intMaxPosYNo], m_arrBlackBlobs[i].ref_arrLimitCenterY[intMaxPosYNo]);
                                    fHeight = m_arrBlackBlobs[i].ref_arrHeight[intMaxPosYNo];
                                    float fSealTopPositionY = pCenter.Y - (fHeight / 2);

                                    float fGapDistance = fSealTopPositionY - fBorderPositionY;

                                    // if the distance gap is larger than gap between broken seal
                                    if (fSealTopPositionY - (m_fLineWidthAverage[2] / 2) < fBorderPositionY)
                                    {
                                        List<float> arrSealBarFillinStartX = new List<float>();
                                        List<float> arrSealBarFillinEndX = new List<float>();
                                        List<float> arrSealBarFillinStartY = new List<float>();
                                        List<float> arrSealBarFillinEndY = new List<float>();
                                        for (int m = 0; m < m_arrBlackBlobs[i].ref_intNumSelectedObject; m++)
                                        {
                                            int intOverLapStartXIndex = -1;
                                            int intOverLapEndXIndex = -1;
                                            float fChildStartX = m_arrBlackBlobs[i].ref_arrLimitCenterX[m] - m_arrBlackBlobs[i].ref_arrWidth[m] / 2;
                                            float fChildEndX = m_arrBlackBlobs[i].ref_arrLimitCenterX[m] + m_arrBlackBlobs[i].ref_arrWidth[m] / 2;
                                            float fChildStartY = m_arrBlackBlobs[i].ref_arrLimitCenterY[m] - m_arrBlackBlobs[i].ref_arrHeight[m] / 2;
                                            float fChildEndY = m_arrBlackBlobs[i].ref_arrLimitCenterY[m] + m_arrBlackBlobs[i].ref_arrHeight[m] / 2;
                                            for (int f = 0; f < arrSealBarFillinStartX.Count; f++)
                                            {
                                                if (fChildStartX >= arrSealBarFillinStartX[f] && fChildStartX <= arrSealBarFillinEndX[f])
                                                {
                                                    intOverLapStartXIndex = f;
                                                }

                                                if (fChildEndX >= arrSealBarFillinStartX[f] && fChildEndX <= arrSealBarFillinEndX[f])
                                                {
                                                    intOverLapEndXIndex = f;
                                                }

                                                if (intOverLapStartXIndex != -1 && intOverLapEndXIndex != -1)
                                                    break;
                                            }

                                            // if either one over lap, then need to check is it noise line? 
                                            if (intOverLapStartXIndex != -1 || intOverLapEndXIndex != -1)
                                            {
                                                bool blnOutofSealBorder = false;
                                                if (intOverLapStartXIndex != -1)
                                                {
                                                    if (m_arrBlackBlobs[i].ref_arrLimitCenterY[m] < arrSealBarFillinStartY[intOverLapStartXIndex])
                                                    {
                                                        blnOutofSealBorder = true;
                                                    }
                                                    else if (m_arrBlackBlobs[i].ref_arrLimitCenterY[m] > arrSealBarFillinEndY[intOverLapStartXIndex])
                                                    {
                                                        blnOutofSealBorder = true;
                                                    }
                                                }

                                                if (intOverLapEndXIndex != -1)
                                                {
                                                    if (m_arrBlackBlobs[i].ref_arrLimitCenterY[m] < arrSealBarFillinStartY[intOverLapEndXIndex])
                                                    {
                                                        blnOutofSealBorder = true;
                                                    }
                                                    else if (m_arrBlackBlobs[i].ref_arrLimitCenterY[m] > arrSealBarFillinEndY[intOverLapEndXIndex])
                                                    {
                                                        blnOutofSealBorder = true;
                                                    }
                                                }

                                                if (blnOutofSealBorder)
                                                    continue;
                                            }

                                            if (intOverLapStartXIndex != -1 && intOverLapEndXIndex != -1)
                                            {
                                                if (intOverLapStartXIndex == intOverLapEndXIndex)   // child object totally inside parent object
                                                {
                                                    //  ======================  Parent
                                                    //        ==========        child
                                                    // do nothing.
                                                }
                                                else
                                                {
                                                    // =================    ==================      2 parents
                                                    //            =================                 Child
                                                    // Join both parent together
                                                    float fNewStartX = arrSealBarFillinStartX[intOverLapStartXIndex];
                                                    float fNewEndX = arrSealBarFillinEndX[intOverLapEndXIndex];
                                                    float fNewStartY = arrSealBarFillinStartY[intOverLapEndXIndex];
                                                    float fNewEndY = arrSealBarFillinEndY[intOverLapEndXIndex];

                                                    arrSealBarFillinStartX.RemoveAt(intOverLapStartXIndex);
                                                    arrSealBarFillinEndX.RemoveAt(intOverLapStartXIndex);
                                                    arrSealBarFillinStartY.RemoveAt(intOverLapStartXIndex);
                                                    arrSealBarFillinEndY.RemoveAt(intOverLapStartXIndex);
                                                    if (intOverLapEndXIndex > intOverLapStartXIndex)
                                                        intOverLapEndXIndex--;
                                                    arrSealBarFillinStartX.RemoveAt(intOverLapEndXIndex);
                                                    arrSealBarFillinEndX.RemoveAt(intOverLapEndXIndex);
                                                    arrSealBarFillinStartY.RemoveAt(intOverLapEndXIndex);
                                                    arrSealBarFillinEndY.RemoveAt(intOverLapEndXIndex);

                                                    arrSealBarFillinStartX.Add(fNewStartX);
                                                    arrSealBarFillinEndX.Add(fNewEndX);
                                                    arrSealBarFillinStartY.Add(fNewStartY);
                                                    arrSealBarFillinEndY.Add(fNewEndY);
                                                }
                                            }
                                            else if (intOverLapStartXIndex != -1 && intOverLapEndXIndex == -1)
                                            {
                                                // =================                ==================      parents
                                                //            =============                                 Child  
                                                // Extend the parent end X
                                                float fNewStartX = arrSealBarFillinStartX[intOverLapStartXIndex];
                                                float fNewEndX = fChildEndX;
                                                float fNewStartY = arrSealBarFillinStartY[intOverLapStartXIndex];
                                                float fNewEndY = arrSealBarFillinEndY[intOverLapStartXIndex];

                                                arrSealBarFillinStartX.RemoveAt(intOverLapStartXIndex);
                                                arrSealBarFillinEndX.RemoveAt(intOverLapStartXIndex);
                                                arrSealBarFillinStartY.RemoveAt(intOverLapStartXIndex);
                                                arrSealBarFillinEndY.RemoveAt(intOverLapStartXIndex);

                                                arrSealBarFillinStartX.Add(fNewStartX);
                                                arrSealBarFillinEndX.Add(fNewEndX);
                                                arrSealBarFillinStartY.Add(fNewStartY);
                                                arrSealBarFillinEndY.Add(fNewEndY);
                                            }
                                            else if (intOverLapStartXIndex == -1 && intOverLapEndXIndex != -1)
                                            {
                                                // =================                ==================      parents
                                                //                             =============                Child 
                                                // Extend the parent end X
                                                float fNewStartX = fChildStartX;
                                                float fNewEndX = arrSealBarFillinEndX[intOverLapEndXIndex];
                                                float fNewStartY = arrSealBarFillinStartY[intOverLapEndXIndex];
                                                float fNewEndY = arrSealBarFillinEndY[intOverLapEndXIndex];

                                                arrSealBarFillinStartX.RemoveAt(intOverLapEndXIndex);
                                                arrSealBarFillinEndX.RemoveAt(intOverLapEndXIndex);
                                                arrSealBarFillinStartY.RemoveAt(intOverLapEndXIndex);
                                                arrSealBarFillinEndY.RemoveAt(intOverLapEndXIndex);

                                                arrSealBarFillinStartX.Add(fNewStartX);
                                                arrSealBarFillinEndX.Add(fNewEndX);
                                                arrSealBarFillinStartY.Add(fNewStartY);
                                                arrSealBarFillinEndY.Add(fNewEndY);
                                            }
                                            else
                                            {
                                                arrSealBarFillinStartX.Add(fChildStartX);
                                                arrSealBarFillinEndX.Add(fChildEndX);
                                                arrSealBarFillinStartY.Add(fChildStartY);
                                                arrSealBarFillinEndY.Add(fChildEndY);
                                            }
                                        }

                                        List<float> arrSortedCenterX = new List<float>();
                                        List<int> arrSortedBlobIndex = new List<int>();
                                        Math2.SortData(arrSealBarFillinStartX.ToArray(), Math2.Sorting.Increase, ref arrSortedCenterX, ref arrSortedBlobIndex);

                                        for (int m = 0; m <= arrSealBarFillinStartX.Count; m++)
                                        {
                                            float fRightBlobStartX;
                                            if (m == arrSealBarFillinStartX.Count)
                                                fRightBlobStartX = m_objTempROI.ref_ROIWidth;
                                            else
                                                fRightBlobStartX = arrSealBarFillinStartX[arrSortedBlobIndex[m]];
                                            float fLeftBlobEndX;
                                            if (m == 0)
                                                fLeftBlobEndX = 0;
                                            else
                                                fLeftBlobEndX = arrSealBarFillinEndX[arrSortedBlobIndex[m - 1]];

                                            float fGapMM = (float)Math.Max(0, fRightBlobStartX - fLeftBlobEndX) / fCalibY;
                                            if (m_FailBrokenSeal[i] < fGapMM)
                                                m_FailBrokenSeal[i] = fGapMM;
                                            if ((m_minSealBrokenGap / fCalibY) < fGapMM)
                                            {
                                                m_strErrorMessage += "*" + strSealName + " Broken Gap. Set=" + (m_minSealBrokenGap / fCalibY).ToString("F5") + "mm, Result=" + fGapMM.ToString("f4") + " mm";
                                                if (i == 0)
                                                    m_blnFailSeal1 = true;
                                                else
                                                    m_blnFailSeal2 = true;
                                                m_intSealFailMask |= 0x08;
                                                blnResult = false;

                                                PointF pStartPoint;
                                                if (m == 0)
                                                    pStartPoint = new PointF(m_objTempROI.ref_ROITotalX + fLeftBlobEndX,
                                                                                m_objTempROI.ref_ROITotalY + m_arrBlackBlobs[i].ref_arrLimitCenterY[arrSortedBlobIndex[m]]);
                                                else
                                                    pStartPoint = new PointF(m_objTempROI.ref_ROITotalX + fLeftBlobEndX,
                                                                                m_objTempROI.ref_ROITotalY + m_arrBlackBlobs[i].ref_arrLimitCenterY[arrSortedBlobIndex[m - 1]]);

                                                PointF pEndPoint;
                                                if (m == 0)
                                                    pEndPoint = new PointF(m_objTempROI.ref_ROITotalX + fRightBlobStartX,
                                                                                m_objTempROI.ref_ROITotalY + m_arrBlackBlobs[i].ref_arrLimitCenterY[arrSortedBlobIndex[m]]);
                                                else
                                                    pEndPoint = new PointF(m_objTempROI.ref_ROITotalX + fRightBlobStartX,
                                                                                m_objTempROI.ref_ROITotalY + m_arrBlackBlobs[i].ref_arrLimitCenterY[arrSortedBlobIndex[m - 1]]);
                                                m_arrFailSealBrokenStartPoint.Add(pStartPoint);
                                                m_arrFailSealBrokenEndPoint.Add(pEndPoint);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        ////fail upper tolerance
                        //if (intTotalBlackArea > (m_intTemplateAreaAVG[i] + m_arrSealAreaTolerance[i]))
                        //{
                        //    m_strErrorMessage += "*" + strSealName + " Area Overseal (More than Max tolerance): Seal" + i + " Area tolerance = " +
                        //        + m_arrSealAreaTolerance[i] / (fCalibY * fCalibY) + ",   Learnt Template area + area tolerance = " +
                        //        ((m_intTemplateAreaAVG[i] + m_arrSealAreaTolerance[i]) / (fCalibY * fCalibY)).ToString("f5") +
                        //       " mm^2,   Result = " + intTotalBlackArea / (fCalibY * fCalibY) + " mm^2";
                        //    m_intSealFailMask |= 0x10;
                        //    blnResult = false;
                        //}

                        ////fail lower tolerance
                        //if (intTotalBlackArea < (m_intTemplateAreaAVG[i] - m_arrSealAreaTolerance[i]))
                        //{
                        //    m_strErrorMessage += "*" + strSealName + " Insufficient Seal (Less than Min tolerance): Seal" + i + " Area tolerance = " +
                        //        + m_arrSealAreaTolerance[i] / (fCalibY * fCalibY) + ",   Learnt Template area - area tolerance = " +
                        //        ((m_intTemplateAreaAVG[i] - m_arrSealAreaTolerance[i]) / (fCalibY * fCalibY)).ToString("f5") +
                        //        " mm^2,   Result = " + intTotalBlackArea / (fCalibY * fCalibY) + " mm^2";
                        //    m_intSealFailMask |= 0x20;
                        //    blnResult = false;
                        //}


                    }
                }
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("InspectFarAndNearSealArea_BubbleOrBrokenArea Exception : " + ex.ToString());
                SRMMessageBox.Show("InspectFarAndNearSealArea_BubbleOrBrokenArea Exception : " + ex.ToString());
                blnResult = false;
            }

            return blnResult;
        }

        //Load only during empty pocket test
        public void LoadPositionPattern(string strPath)
        {
            try
            {
                string strFile;
                for (int i = 0; i < 4; i++)
                {
                    strFile = strPath + "PositionTemplate0_" + i.ToString() + ".mch";

                    if (File.Exists(strFile))
                        m_arrPositionMatcher[i].Load(strFile);
                }
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("Seal > LoadPositionPattern > Exception = " + ex.ToString());
            }
        }

        //Load only during empty pocket test
        public void LoadPocketPattern(string strPath)
        {
            try
            {
                string strFile;
                for (int i = 0; i < 4; i++)
                {
                    strFile = strPath + "PocketTemplate0_" + i.ToString() + ".mch";

                    if (File.Exists(strFile))
                        m_arrPocketMatcher[i].Load(strFile);
                }
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("Seal > LoadPocketPattern > Exception = " + ex.ToString());
            }
        }

        public void LoadMarkPattern4Direction(string strPath)
        {
            try
            {
                for (int i = 0; i < m_arrMarkMatcher.Count; i++)
                {
                    for (int j = 0; j < m_arrMarkMatcher[i].Count; j++)
                    {
                        if (m_arrMarkMatcher[i][j] != null)
                            m_arrMarkMatcher[i][j].Dispose();
                    }
                }

                m_arrMarkMatcher.Clear();

                string strFile;
                for (int i = 0; i < 8; i++) // 8 mark templates
                {
                    for (int j = 0; j < 4; j++) // 4 direction
                    {
                        strFile = strPath + "MarkTemplate0_" + i.ToString() + "_" + j.ToString() + ".mch";
                        if (File.Exists(strFile))
                        {
                            if (m_arrMarkMatcher.Count == i)
                                m_arrMarkMatcher.Add(new List<EMatcher>());

                            if (m_arrMarkMatcher[i].Count == j)
                                m_arrMarkMatcher[i].Add(new EMatcher());
                            m_arrMarkMatcher[i][j].Load(strFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("Seal > LoadMarkPattern4Direction > Exception = " + ex.ToString());
            }
        }

        public void LoadMarkPatternImage(string strPath)
        {
            for (int i = 0; i < m_arrMatcherImage.Count; i++)
            {
                if (m_arrMatcherImage[i] != null)
                    m_arrMatcherImage[i].Dispose();
            }

            for (int i = 0; i < m_arrMatcherThresholdImage.Count; i++)
            {
                if (m_arrMatcherThresholdImage[i] != null)
                    m_arrMatcherThresholdImage[i].Dispose();
            }

            m_arrMatcherImage.Clear();
            m_arrMatcherThresholdImage.Clear();
            string strFile;
            for (int i = 0; i < 8; i++)     // 8 Mark template 
            {
                strFile = strPath + "MarkTemplate0_" + i.ToString() + ".bmp";
                if (File.Exists(strFile) && i < m_arrTemplateMarkThreshold.Count)
                {
                    if (m_arrMatcherImage.Count == i)
                        m_arrMatcherImage.Add(new ImageDrawing(true));

                    if (m_arrMatcherThresholdImage.Count == i)
                        m_arrMatcherThresholdImage.Add(new ImageDrawing(true));

                    m_arrMatcherImage[i].LoadImage(strFile);
                    m_arrMatcherThresholdImage[i].SetImageSize(m_arrMatcherImage[i].ref_intImageWidth, m_arrMatcherImage[i].ref_intImageHeight);
#if (Debug_2_12 || Release_2_12)
                    EasyImage.Threshold(m_arrMatcherImage[i].ref_objMainImage, m_arrMatcherThresholdImage[i].ref_objMainImage, (uint)m_arrTemplateMarkThreshold[i]);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    EasyImage.Threshold(m_arrMatcherImage[i].ref_objMainImage, m_arrMatcherThresholdImage[i].ref_objMainImage, m_arrTemplateMarkThreshold[i]);
#endif

                }
            }

            CalculateTemplateMarkPixels();
        }

        public bool DoEmptyPocketInspection(ROI objSearchROI, int intFinalReduction, int intTemplateNo)
        {
            m_strErrorMessage = "";

            if (intTemplateNo >= m_arrPocketMatcher.Count)
            {
                m_strErrorMessage = "Fail Empty Pocket - Template " + (intTemplateNo + 1).ToString() + " is not exist";
                m_intSealFailMask |= 0x100;
                return false;
            }

            if (!m_arrPocketMatcher[intTemplateNo].PatternLearnt)
            {
                m_strErrorMessage = "Fail Empty Pocket - No Pattern Learnt";
                m_intSealFailMask |= 0x100;
                return false;
            }

            objSearchROI.ref_ROIPositionX = objSearchROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
            objSearchROI.ref_ROIPositionY = objSearchROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);

            if (m_arrPocketMatcher[intTemplateNo].PatternWidth > objSearchROI.ref_ROI.Width ||
                m_arrPocketMatcher[intTemplateNo].PatternHeight > objSearchROI.ref_ROI.Height)
            {
                m_strErrorMessage = "Fail Empty Pocket - Search ROI too small to find empty pattern";
                m_intSealFailMask |= 0x100;

                return false;
            }

            if (m_arrPocketMatcher[intTemplateNo].MinAngle != 0)
                m_arrPocketMatcher[intTemplateNo].MinAngle = 0;
            if (m_arrPocketMatcher[intTemplateNo].MaxAngle != 0)
                m_arrPocketMatcher[intTemplateNo].MaxAngle = 0;
#if (Debug_2_12 || Release_2_12)
            m_arrPocketMatcher[intTemplateNo].FinalReduction = (uint)intFinalReduction;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_arrPocketMatcher[intTemplateNo].FinalReduction = intFinalReduction;
#endif

            m_arrPocketMatcher[intTemplateNo].Interpolate = true;
            m_arrPocketMatcher[intTemplateNo].Match(objSearchROI.ref_ROI);

            if (m_arrPocketMatcher[intTemplateNo].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
            {
                m_fPocketMatchScore[intTemplateNo] = m_arrPocketMatcher[intTemplateNo].GetPosition(0).Score;
                if (m_fPocketMatchScore[intTemplateNo] < 0)
                    m_fPocketMatchScore[intTemplateNo] = 0;
                if (m_fPocketMatchScore[intTemplateNo] < m_fPocketMinScore)
                {
                    m_strErrorMessage = "Fail Empty Pocket - Not fulfill Min Setting : Set = " + (m_fPocketMinScore * 100).ToString() +
                                        " Score = " + (m_fPocketMatchScore[intTemplateNo] * 100).ToString("f5");
                    m_intSealFailMask |= 0x100;
                }
                else
                {
                    m_pResultPocketCenterPoint = new PointF(objSearchROI.ref_ROITotalX + m_arrPocketMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                    objSearchROI.ref_ROITotalY + m_arrPocketMatcher[intTemplateNo].GetPosition(0).CenterY);

                    m_intSealFailMask &= ~0x100;
                    m_intPocketTemplateNumSelected = intTemplateNo;

                    //m_fUnitAngle = m_arrPocketMatcher[intTemplateNo].GetPosition(0).Angle;

                    m_strTrack += ", Mark Center X=" + m_pResultPocketCenterPoint.X.ToString();
                    m_strTrack += ", Mark Center Y=" + m_pResultPocketCenterPoint.Y.ToString();
                    m_strTrack += ", Mark Score=" + m_fPocketMatchScore.ToString();
                    return true;
                }
            }
            else
            {
                m_strErrorMessage = "Fail Empty Pocket - Find 0 Pattern in Search ROI";
                m_intSealFailMask |= 0x100;
            }

            return false;
        }

        public bool DoNoEmptyPocketInspection(ROI objSearchROI, int intFinalReduction, int intTemplateNo)
        {
            m_strErrorMessage = "";

            if (!m_arrPocketMatcher[intTemplateNo].PatternLearnt)
            {
                m_strErrorMessage = "Fail Empty Pocket - No Pattern Learnt";
                m_intSealFailMask |= 0x100;
            }

            objSearchROI.ref_ROIPositionX = objSearchROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
            objSearchROI.ref_ROIPositionY = objSearchROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);

            if (m_arrPocketMatcher[intTemplateNo].MinAngle != 0)
                m_arrPocketMatcher[intTemplateNo].MinAngle = 0;
            if (m_arrPocketMatcher[intTemplateNo].MaxAngle != 0)
                m_arrPocketMatcher[intTemplateNo].MaxAngle = 0;
#if (Debug_2_12 || Release_2_12)
            m_arrPocketMatcher[intTemplateNo].FinalReduction = (uint)intFinalReduction;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_arrPocketMatcher[intTemplateNo].FinalReduction = intFinalReduction;
#endif

            m_arrPocketMatcher[intTemplateNo].Interpolate = true;
            m_arrPocketMatcher[intTemplateNo].Match(objSearchROI.ref_ROI);

            if (m_arrPocketMatcher[intTemplateNo].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
            {
                m_fPocketMatchScore[intTemplateNo] = m_arrPocketMatcher[intTemplateNo].GetPosition(0).Score;

                if (m_fPocketMatchScore[intTemplateNo] >= m_fPocketMinScore)
                {
                    m_strErrorMessage = "Fail Empty Pocket - Fulfill Min Setting : Set = " + (m_fPocketMinScore * 100).ToString() +
                                        " Score = " + (m_fPocketMatchScore[intTemplateNo] * 100).ToString("f5");
                    m_intSealFailMask |= 0x100;
                }
                else
                {
                    m_pResultPocketCenterPoint = new PointF(objSearchROI.ref_ROITotalX + m_arrPocketMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                    objSearchROI.ref_ROITotalY + m_arrPocketMatcher[intTemplateNo].GetPosition(0).CenterY);

                    m_intPocketTemplateNumSelected = intTemplateNo;

                    //m_fUnitAngle = m_arrPocketMatcher[intTemplateNo].GetPosition(0).Angle;

                    m_strTrack += ", Mark Center X=" + m_pResultPocketCenterPoint.X.ToString();
                    m_strTrack += ", Mark Center Y=" + m_pResultPocketCenterPoint.Y.ToString();
                    m_strTrack += ", Mark Score=" + m_fPocketMatchScore.ToString();
                    return true;
                }
            }
            //else
            //{
            //    m_strErrorMessage = "Fail Empty Pocket - Pattern Not Found";
            //    m_intSealFailMask |= 0x100;
            //}

            return false;
        }

        public bool DoPositionInspection_AlwaysPass(ROI objSearchROI, int intFinalReduction, int intTemplateNo, float fCalibX, bool blnLearnPage)
        {
            m_strTrack = "";

            if (!m_arrPositionMatcher[intTemplateNo].PatternLearnt)
            {
                if (!blnLearnPage)
                {
                    m_strErrorMessage = "Fail Position - No Pattern Learnt";
                    m_intSealFailMask |= 0x01;
                }
                return false;
            }

            if (m_arrPositionMatcher[intTemplateNo].MinAngle != 0)
                m_arrPositionMatcher[intTemplateNo].MinAngle = 0;
            if (m_arrPositionMatcher[intTemplateNo].MaxAngle != 0)
                m_arrPositionMatcher[intTemplateNo].MaxAngle = 0;
#if (Debug_2_12 || Release_2_12)
            m_arrPositionMatcher[intTemplateNo].FinalReduction = (uint)intFinalReduction;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_arrPositionMatcher[intTemplateNo].FinalReduction = intFinalReduction;
#endif

            m_arrPositionMatcher[intTemplateNo].Interpolate = true;
            if (m_arrPositionMatcher[intTemplateNo].MaxPositions < 2)
                m_arrPositionMatcher[intTemplateNo].MaxPositions = 2;

            ROI objROI = new ROI();
            objSearchROI.CopyTo(ref objROI);
            float Score1 = 0, Score2 = 0;
            bool blnCalculateShiftedXY = false;
            float fDistance1, fDistance2;
            PointF pResultPositionCenterPoint;
            if (m_intTapePocketPitch == 2)
            {
                objROI.LoadROISetting(objSearchROI.ref_ROIPositionX, objSearchROI.ref_ROIPositionY,
                                            objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);

                m_arrPositionMatcher[intTemplateNo].Match(objROI.ref_ROI);

                if (m_arrPositionMatcher[intTemplateNo].NumPositions > 0 && m_arrPositionMatcher[intTemplateNo].GetPosition(0).Score > 0.3)     // if macthing result hit the min score, its position will be 1 or more
                {
                    Score1 = m_arrPositionMatcher[intTemplateNo].GetPosition(0).Score;

                    m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                    objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);

                    fDistance1 = Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X);
                    if (fDistance1 > (m_intTapePocketPitch * fCalibX * 1.5))
                    {
                        if (m_arrPositionMatcher[intTemplateNo].NumPositions > 1 && m_arrPositionMatcher[intTemplateNo].GetPosition(1).Score > 0.3)
                        {
                            m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(1).CenterX,
                                                    objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(1).CenterY);
                        }
                    }

                    m_intPositionTemplateNumSelected = intTemplateNo;

                    //objROI.LoadROISetting(objSearchROI.ref_ROIPositionX - (int)(m_intTapePocketPitch * fCalibX), objSearchROI.ref_ROIPositionY,
                    //                        objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);

                    objROI.LoadROISetting(objSearchROI.ref_ROICenterX - (int)(m_intTapePocketPitch * fCalibX) - (objSearchROI.ref_ROIWidth / 2), objSearchROI.ref_ROIPositionY,
                                            objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);

                    m_arrPositionMatcher[intTemplateNo].Match(objROI.ref_ROI);

                    if (m_arrPositionMatcher[intTemplateNo].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                    {
                        Score2 = m_arrPositionMatcher[intTemplateNo].GetPosition(0).Score;
                        if (Score2 > Score1)
                        {
                            pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                       objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);

                            fDistance1 = Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X);
                            fDistance2 = Math.Abs(pResultPositionCenterPoint.X - m_pPositionCenterPoint.X);

                            if ((fDistance1 < (m_intTapePocketPitch * fCalibX * 1.5)) && (fDistance2 < (m_intTapePocketPitch * fCalibX * 1.5)))
                            {
                                m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                 objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);
                            }
                            else if (fDistance2 < (m_intTapePocketPitch * fCalibX * 1.5))
                            {
                                m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                 objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);
                            }

                            m_intPositionTemplateNumSelected = intTemplateNo;
                        }
                        else
                        {
                            if (Math.Abs(Score2 - Score1) < 0.2)
                            {
                                pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                       objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);

                                fDistance1 = Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X);
                                fDistance2 = Math.Abs(pResultPositionCenterPoint.X - m_pPositionCenterPoint.X);

                                if ((fDistance1 < (m_intTapePocketPitch * fCalibX * 1.5)) && (fDistance2 < (m_intTapePocketPitch * fCalibX * 1.5)))
                                {
                                    m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                     objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);
                                }
                                else if (fDistance2 < (m_intTapePocketPitch * fCalibX * 1.5))
                                {
                                    m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                     objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);
                                }
                            }
                        }

                    }

                    blnCalculateShiftedXY = true;
                }
                else
                {
                    objROI.LoadROISetting(objSearchROI.ref_ROICenterX - (int)(m_intTapePocketPitch * fCalibX) - (objSearchROI.ref_ROIWidth / 2), objSearchROI.ref_ROIPositionY,
                                           objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);

                    m_arrPositionMatcher[intTemplateNo].Match(objROI.ref_ROI);

                    if (m_arrPositionMatcher[intTemplateNo].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                    {
                        Score2 = m_arrPositionMatcher[intTemplateNo].GetPosition(0).Score;
                        if (Score2 > Score1)
                        {
                            m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                   objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);

                            m_intPositionTemplateNumSelected = intTemplateNo;
                        }
                    }
                    else
                    {
                        if (!blnLearnPage)
                        {
                            m_strErrorMessage = "Fail Position - Pattern not found";
                            m_intSealFailMask |= 0x01;
                        }
                        return false;
                    }
                }
            }
            else
            {
                objROI.LoadROISetting(objSearchROI.ref_ROIPositionX, objSearchROI.ref_ROIPositionY,
                                            objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);

                m_arrPositionMatcher[intTemplateNo].Match(objROI.ref_ROI);

                if (m_arrPositionMatcher[intTemplateNo].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                    objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);

                    m_intPositionTemplateNumSelected = intTemplateNo;
                    //m_fUnitAngle = m_arrPositionMatcher[intTemplateNo].GetPosition(0).Angle;
                }
                else
                {
                    if (!blnLearnPage)
                    {
                        m_strErrorMessage = "Fail Position - Pattern not found";
                        m_intSealFailMask |= 0x01;
                    }
                    return false;
                }

            }

            if (blnCalculateShiftedXY)
            {
                //Check Shift tolerance first
                //if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) > Math.Abs((Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) - m_fTapePocketPitchByImage)))
                //{
                //    if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - m_fTapePocketPitchByImage) > Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + m_fTapePocketPitchByImage))
                //        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + m_fTapePocketPitchByImage;
                //    else
                //        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - m_fTapePocketPitchByImage;
                //}
                //else
                //    m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;

                // 2020 03 23 - CCENG: Use new variables m_fTapePocketPitchByImage to replace m_intTapePocketPitch during define the shiftedX value.
                float fHalfSprocketPitch;
                if (m_fTapePocketPitchByImage == -1)
                    fHalfSprocketPitch = m_intTapePocketPitch * fCalibX;
                else
                    fHalfSprocketPitch = m_fTapePocketPitchByImage;

                if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) > Math.Abs((Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) - fHalfSprocketPitch)))
                {
                    if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - fHalfSprocketPitch) > Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + fHalfSprocketPitch))
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + fHalfSprocketPitch;
                    else
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - fHalfSprocketPitch;
                }
                else
                    m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;

                m_fShiftedY = m_pResultPositionCenterPoint.Y - m_pPositionCenterPoint.Y;
            }
            else
            {
                // 2020 03 23 - CCENG: Need to recalculate position also
                //m_fShiftedX = 0;
                //m_fShiftedY = 0;
                m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;

                float fHalfSprocketPitch;
                if (m_fTapePocketPitchByImage == -1)
                    fHalfSprocketPitch = m_intTapePocketPitch * fCalibX;
                else
                    fHalfSprocketPitch = m_fTapePocketPitchByImage;

                if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) > Math.Abs((Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) - fHalfSprocketPitch)))
                {
                    if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - fHalfSprocketPitch) > Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + fHalfSprocketPitch))
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + fHalfSprocketPitch;
                    else
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - fHalfSprocketPitch;
                }
                else
                    m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;

                m_fShiftedY = m_pResultPositionCenterPoint.Y - m_pPositionCenterPoint.Y;
            }


            return true;
        }

        public bool DoPositionInspection_FindNearestToTemplateSprocketHolePosition(ROI objSearchROI, int intFinalReduction, int intTemplateNo, float fCalibX, bool blnLearnPage)
        {
            m_strTrack = "";

            if (!m_arrPositionMatcher[intTemplateNo].PatternLearnt)
            {
                if (!blnLearnPage)
                {
                    m_strErrorMessage = "Fail Position - No Pattern Learnt";
                    m_intSealFailMask |= 0x01;
                }
                return false;
            }

            if (m_arrPositionMatcher[intTemplateNo].MinAngle != 0)
                m_arrPositionMatcher[intTemplateNo].MinAngle = 0;
            if (m_arrPositionMatcher[intTemplateNo].MaxAngle != 0)
                m_arrPositionMatcher[intTemplateNo].MaxAngle = 0;
#if (Debug_2_12 || Release_2_12)
            m_arrPositionMatcher[intTemplateNo].FinalReduction = (uint)intFinalReduction;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_arrPositionMatcher[intTemplateNo].FinalReduction = intFinalReduction;
#endif

            m_arrPositionMatcher[intTemplateNo].Interpolate = true;
            if (m_arrPositionMatcher[intTemplateNo].MaxPositions < 2)
                m_arrPositionMatcher[intTemplateNo].MaxPositions = 2;

            ROI objROI = new ROI();
            objSearchROI.CopyTo(ref objROI);
            float Score1 = 0, Score2 = 0;
            bool blnCalculateShiftedXY = false;
            if (m_intTapePocketPitch == 2)
            {
                objROI.LoadROISetting(0, objSearchROI.ref_ROIPositionY,
                                            objSearchROI.ref_ROI.TopParent.Width, objSearchROI.ref_ROIHeight);

                bool blnDebugImage = false;
                if (blnDebugImage)
                {
                    objROI.SaveImage("D:\\TS\\objROI.bmp"); // Debug
                }

                m_arrPositionMatcher[intTemplateNo].Match(objROI.ref_ROI);

                bool blnFoundSprocket = false;
#if (Debug_2_12 || Release_2_12)
                for (uint i = 0; i < m_arrPositionMatcher[intTemplateNo].NumPositions; i++)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                for (int i = 0; i < m_arrPositionMatcher[intTemplateNo].NumPositions; i++)
#endif

                {
                    Score1 = m_arrPositionMatcher[intTemplateNo].GetPosition(i).Score;

                    m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(i).CenterX,
                                objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(i).CenterY);

                    if (m_arrPositionMatcher[intTemplateNo].GetPosition(i).Score > 0.9)
                    {
                        float fDistance1 = Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X);
                        if (fDistance1 < (m_intTapePocketPitch * fCalibX * 1.5))
                        {
                            blnFoundSprocket = true;
                            break;
                        }
                    }
                }

                if (blnFoundSprocket)
                {
                    blnCalculateShiftedXY = true;
                }
                else
                {
                    blnFoundSprocket = false;
#if (Debug_2_12 || Release_2_12)
                    for (uint i = 0; i < m_arrPositionMatcher[intTemplateNo].NumPositions; i++)
                    {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    for (int i = 0; i < m_arrPositionMatcher[intTemplateNo].NumPositions; i++)
                    {
#endif

                        Score1 = m_arrPositionMatcher[intTemplateNo].GetPosition(i).Score;

                        m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(i).CenterX,
                                    objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(i).CenterY);

                        if (m_arrPositionMatcher[intTemplateNo].GetPosition(i).Score > 0.5)
                        {
                            float fHalfSprocketPitch;
                            if (m_fTapePocketPitchByImage <= 0)
                                fHalfSprocketPitch = m_intTapePocketPitch * fCalibX;
                            else
                                fHalfSprocketPitch = m_fTapePocketPitchByImage;

                            float fPocketPitch15 = (fHalfSprocketPitch * 1.5f);
                            float fDistance1 = ((float)Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X));
                            float fReduct = 0;

                            // ------------------- checking loop timeout ---------------------------------------------------
                            HiPerfTimer timeout = new HiPerfTimer();
                            timeout.Start();

                            while (true)
                            {
                                // ------------------- checking loop timeout ---------------------------------------------------
                                if (timeout.Timing > 10000)
                                {
                                    STTrackLog.WriteLine(">>>>>>>>>>>>> time out 102");
                                    break;
                                }
                                // ---------------------------------------------------------------------------------------------

                                fDistance1 -= (fHalfSprocketPitch * 2);
                                fReduct += (fHalfSprocketPitch * 2);
                                if (fDistance1 < fPocketPitch15)
                                {
                                    break;
                                }

                            }
                            timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------

                            if (m_pResultPositionCenterPoint.X < m_pPositionCenterPoint.X)
                                m_pResultPositionCenterPoint.X += fReduct;
                            else
                                m_pResultPositionCenterPoint.X -= fReduct;

                            if (fDistance1 < fPocketPitch15)
                            {
                                blnFoundSprocket = true;
                                break;
                            }
                            break;
                        }
                    }

                    if (blnFoundSprocket)
                    {
                        blnCalculateShiftedXY = true;
                    }
                    else
                    {
                        m_strErrorMessage = "Fail To Find Sprocket Hole Position.";
                        m_intSealFailMask |= 0x01;
                        return false;
                    }
                }

            }
            else
            {
                objROI.LoadROISetting(objSearchROI.ref_ROIPositionX, objSearchROI.ref_ROIPositionY,
                                            objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);

                m_arrPositionMatcher[intTemplateNo].Match(objROI.ref_ROI);

                if (m_arrPositionMatcher[intTemplateNo].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                    objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);

                    m_intPositionTemplateNumSelected = intTemplateNo;
                    //m_fUnitAngle = m_arrPositionMatcher[intTemplateNo].GetPosition(0).Angle;
                }
                else
                {
                    if (!blnLearnPage)
                    {
                        m_strErrorMessage = "Fail Position - Pattern not found";
                        m_intSealFailMask |= 0x01;
                    }
                    return false;
                }

            }

            if (blnCalculateShiftedXY)
            {
                //Check Shift tolerance first
                //if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) > Math.Abs((Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) - m_fTapePocketPitchByImage)))
                //{
                //    if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - m_fTapePocketPitchByImage) > Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + m_fTapePocketPitchByImage))
                //        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + m_fTapePocketPitchByImage;
                //    else
                //        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - m_fTapePocketPitchByImage;
                //}
                //else
                //    m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;

                // 2020 03 23 - CCENG: Use new variables m_fTapePocketPitchByImage to replace m_intTapePocketPitch during define the shiftedX value.
                float fHalfSprocketPitch;
                if (m_fTapePocketPitchByImage == -1)
                    fHalfSprocketPitch = m_intTapePocketPitch * fCalibX;
                else
                    fHalfSprocketPitch = m_fTapePocketPitchByImage;

                if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) > Math.Abs((Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) - fHalfSprocketPitch)))
                {
                    if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - fHalfSprocketPitch) > Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + fHalfSprocketPitch))
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + fHalfSprocketPitch;
                    else
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - fHalfSprocketPitch;
                    
                }
                else
                {
                    m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;
                    
                }

                m_fShiftedY = m_pResultPositionCenterPoint.Y - m_pPositionCenterPoint.Y;


                //2021-11-09 ZJYEOH : For Circle gauge shift
                m_fCircleGaugeShiftX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;
            }
            else
            {
                // 2020 03 23 - CCENG: Need to recalculate position also
                //m_fShiftedX = 0;
                //m_fShiftedY = 0;
                m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;

                float fHalfSprocketPitch;
                if (m_fTapePocketPitchByImage == -1)
                    fHalfSprocketPitch = m_intTapePocketPitch * fCalibX;
                else
                    fHalfSprocketPitch = m_fTapePocketPitchByImage;

                if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) > Math.Abs((Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) - fHalfSprocketPitch)))
                {
                    if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - fHalfSprocketPitch) > Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + fHalfSprocketPitch))
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + fHalfSprocketPitch;
                    else
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - fHalfSprocketPitch;
                    
                }
                else
                {
                    m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;
                    
                }

                m_fShiftedY = m_pResultPositionCenterPoint.Y - m_pPositionCenterPoint.Y;


                //2021-11-09 ZJYEOH : For Circle gauge shift
                m_fCircleGaugeShiftX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;
            }


            return true;
        }

        public bool DoPositionInspection_FindSprocketHolePositionNearestToImageCenter(ROI objSearchROI, int intFinalReduction, int intTemplateNo, float fCalibX, bool blnLearnPage)
        {
            m_strTrack = "";

            if (!m_arrPositionMatcher[intTemplateNo].PatternLearnt)
            {
                if (!blnLearnPage)
                {
                    m_strErrorMessage = "Fail Position - No Pattern Learnt";
                    m_intSealFailMask |= 0x01;
                }
                return false;
            }
            m_fCircleGaugeShiftX = 0;
            if (m_arrPositionMatcher[intTemplateNo].MinAngle != 0)
                m_arrPositionMatcher[intTemplateNo].MinAngle = 0;
            if (m_arrPositionMatcher[intTemplateNo].MaxAngle != 0)
                m_arrPositionMatcher[intTemplateNo].MaxAngle = 0;
#if (Debug_2_12 || Release_2_12)
            m_arrPositionMatcher[intTemplateNo].FinalReduction = (uint)intFinalReduction;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_arrPositionMatcher[intTemplateNo].FinalReduction = intFinalReduction;
#endif

            m_arrPositionMatcher[intTemplateNo].Interpolate = true;
            if (m_arrPositionMatcher[intTemplateNo].MaxPositions < 2)
                m_arrPositionMatcher[intTemplateNo].MaxPositions = 2;

            ROI objROI = new ROI();
            objSearchROI.CopyTo(ref objROI);
            float Score1 = 0, Score2 = 0;
            bool blnCalculateShiftedXY = false;
            if (m_intTapePocketPitch == 2)
            {
                objROI.LoadROISetting(0, objSearchROI.ref_ROIPositionY,
                                            objSearchROI.ref_ROI.TopParent.Width, objSearchROI.ref_ROIHeight);

                m_arrPositionMatcher[intTemplateNo].Match(objROI.ref_ROI);

                List<PointF> arrCenter = new List<PointF>();
                List<float> arrDistanceX = new List<float>();
                List<float> arrScore = new List<float>();
#if (Debug_2_12 || Release_2_12)
                for (uint i = 0; i < m_arrPositionMatcher[intTemplateNo].NumPositions; i++)
                {
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                for (int i = 0; i < m_arrPositionMatcher[intTemplateNo].NumPositions; i++)
                {
#endif

                    arrCenter.Add(new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(i).CenterX,
                                objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(i).CenterY));
                    arrDistanceX.Add(arrCenter[(int)i].X - objROI.ref_ROI.TopParent.Width / 2);
                    arrScore.Add(m_arrPositionMatcher[intTemplateNo].GetPosition(i).Score);
                }

                int intSelectedIndex = 0;
                float fClosestDistance = float.MaxValue;
                for (int i = 0; i < arrCenter.Count; i++)
                {
                    if (arrScore[i] > 0.9)
                    {
                        if (fClosestDistance > arrDistanceX[i])
                        {
                            m_pResultPositionCenterPoint = arrCenter[i];
                            fClosestDistance = arrDistanceX[i];
                            intSelectedIndex = i;
                        }
                    }
                }

                if (intSelectedIndex >= 0)
                {

                }


                //if (blnFoundSprocket)
                //{
                //    blnCalculateShiftedXY = true;
                //}
                //else
                //{
                //    m_strErrorMessage = "Fail To Find Sprocket Hole Position.";
                //    return false;
                //}

            }
            else
            {
                objROI.LoadROISetting(objSearchROI.ref_ROIPositionX, objSearchROI.ref_ROIPositionY,
                                            objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);

                m_arrPositionMatcher[intTemplateNo].Match(objROI.ref_ROI);

                if (m_arrPositionMatcher[intTemplateNo].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                    objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);

                    m_intPositionTemplateNumSelected = intTemplateNo;
                    //m_fUnitAngle = m_arrPositionMatcher[intTemplateNo].GetPosition(0).Angle;
                }
                else
                {
                    if (!blnLearnPage)
                    {
                        m_strErrorMessage = "Fail Position - Pattern not found";
                        m_intSealFailMask |= 0x01;
                    }
                    return false;
                }

            }

            if (blnCalculateShiftedXY)
            {
                //Check Shift tolerance first
                //if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) > Math.Abs((Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) - m_fTapePocketPitchByImage)))
                //{
                //    if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - m_fTapePocketPitchByImage) > Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + m_fTapePocketPitchByImage))
                //        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + m_fTapePocketPitchByImage;
                //    else
                //        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - m_fTapePocketPitchByImage;
                //}
                //else
                //    m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;

                // 2020 03 23 - CCENG: Use new variables m_fTapePocketPitchByImage to replace m_intTapePocketPitch during define the shiftedX value.
                float fHalfSprocketPitch;
                if (m_fTapePocketPitchByImage == -1)
                    fHalfSprocketPitch = m_intTapePocketPitch * fCalibX;
                else
                    fHalfSprocketPitch = m_fTapePocketPitchByImage;

                if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) > Math.Abs((Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) - fHalfSprocketPitch)))
                {
                    if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - fHalfSprocketPitch) > Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + fHalfSprocketPitch))
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + fHalfSprocketPitch;
                    else
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - fHalfSprocketPitch;
                    
                }
                else
                {
                    m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;
                    
                }

                m_fShiftedY = m_pResultPositionCenterPoint.Y - m_pPositionCenterPoint.Y;


                //2021-11-09 ZJYEOH : For Circle gauge shift
                m_fCircleGaugeShiftX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;
            }
            else
            {
                // 2020 03 23 - CCENG: Need to recalculate position also
                //m_fShiftedX = 0;
                //m_fShiftedY = 0;
                m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;

                float fHalfSprocketPitch;
                if (m_fTapePocketPitchByImage == -1)
                    fHalfSprocketPitch = m_intTapePocketPitch * fCalibX;
                else
                    fHalfSprocketPitch = m_fTapePocketPitchByImage;

                if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) > Math.Abs((Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) - fHalfSprocketPitch)))
                {
                    if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - fHalfSprocketPitch) > Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + fHalfSprocketPitch))
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + fHalfSprocketPitch;
                    else
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - fHalfSprocketPitch;
                    
                }
                else
                {
                    m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;

                }

                m_fShiftedY = m_pResultPositionCenterPoint.Y - m_pPositionCenterPoint.Y;


                //2021-11-09 ZJYEOH : For Circle gauge shift
                m_fCircleGaugeShiftX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;
            }


            return true;
        }

        public bool DoPositionInspection(ROI objSearchROI, int intFinalReduction, int intTemplateNo, float fCalibY)
        {
            m_strTrack = "";

            if (!m_arrPositionMatcher[intTemplateNo].PatternLearnt)
            {
                m_strErrorMessage = "Fail Position - No Pattern Learnt";
                m_intSealFailMask |= 0x01;
                return false;
            }

            if (m_arrPositionMatcher[intTemplateNo].MinAngle != 0)
                m_arrPositionMatcher[intTemplateNo].MinAngle = 0;
            if (m_arrPositionMatcher[intTemplateNo].MaxAngle != 0)
                m_arrPositionMatcher[intTemplateNo].MaxAngle = 0;
#if (Debug_2_12 || Release_2_12)
            m_arrPositionMatcher[intTemplateNo].FinalReduction = (uint)intFinalReduction;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            m_arrPositionMatcher[intTemplateNo].FinalReduction = intFinalReduction;
#endif

            m_arrPositionMatcher[intTemplateNo].Interpolate = true;

            ROI objROI = new ROI();
            objSearchROI.CopyTo(ref objROI);

            if (m_intTapePocketPitch == 2)
            {

                objROI.LoadROISetting(objSearchROI.ref_ROIPositionX, objSearchROI.ref_ROIPositionY,
                                            objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);

                m_arrPositionMatcher[intTemplateNo].Match(objROI.ref_ROI);

                if (m_arrPositionMatcher[intTemplateNo].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    if (m_arrPositionMatcher[intTemplateNo].GetPosition(0).Score < 0.8)
                    {
                        objROI.LoadROISetting(objSearchROI.ref_ROIPositionX - (int)(m_intTapePocketPitch * fCalibY), objSearchROI.ref_ROIPositionY,
                                            objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);

                        m_arrPositionMatcher[intTemplateNo].Match(objROI.ref_ROI);

                        if (m_arrPositionMatcher[intTemplateNo].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                        {
                            if (m_arrPositionMatcher[intTemplateNo].GetPosition(0).Score < 0.8)
                            {
                                m_strErrorMessage = "Fail Position - Not fulfill Min Setting";
                                m_intSealFailMask |= 0x01;
                                return false;
                            }
                        }
                        else
                        {
                            m_strErrorMessage = "Fail Position - Not fulfill Min Setting";
                            m_intSealFailMask |= 0x01;
                            return false;
                        }
                    }

                    m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                    objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);

                    m_intPositionTemplateNumSelected = intTemplateNo;
                    //m_fUnitAngle = m_arrPositionMatcher[intTemplateNo].GetPosition(0).Angle;
                }
                else
                {
                    m_strErrorMessage = "Fail Position - Not fulfill Min Setting";
                    m_intSealFailMask |= 0x01;
                    return false;
                }

                //Check Shift tolerance first
                if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) > Math.Abs((Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X) - m_intTapePocketPitch * fCalibY)))
                {
                    if (Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - m_intTapePocketPitch * fCalibY) > Math.Abs(m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + m_intTapePocketPitch * fCalibY))
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X + m_intTapePocketPitch * fCalibY;
                    else
                        m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X - m_intTapePocketPitch * fCalibY;
                }
                else
                    m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;

                m_fShiftedY = m_pResultPositionCenterPoint.Y - m_pPositionCenterPoint.Y;

                m_strTrack += "Position Shifted X=" + m_fShiftedX.ToString();
                m_strTrack += ", Position Shifted Y=" + m_fShiftedY.ToString();
            }
            else
            {
                objROI.LoadROISetting(objSearchROI.ref_ROIPositionX, objSearchROI.ref_ROIPositionY,
                                            objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);

                m_arrPositionMatcher[intTemplateNo].Match(objROI.ref_ROI);

                if (m_arrPositionMatcher[intTemplateNo].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                {
                    if (m_arrPositionMatcher[intTemplateNo].GetPosition(0).Score < 0.8)
                    {
                        m_strErrorMessage = "Fail Position - Not fulfill Min Setting";
                        m_intSealFailMask |= 0x01;
                        return false;
                    }

                    m_pResultPositionCenterPoint = new PointF(objROI.ref_ROITotalX + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterX,
                                                    objROI.ref_ROITotalY + m_arrPositionMatcher[intTemplateNo].GetPosition(0).CenterY);

                    m_intPositionTemplateNumSelected = intTemplateNo;
                    //m_fUnitAngle = m_arrPositionMatcher[intTemplateNo].GetPosition(0).Angle;
                }
                else
                {
                    m_strErrorMessage = "Fail Position - Not fulfill Min Setting";
                    m_intSealFailMask |= 0x01;
                    return false;
                }

                //Check Shift tolerance first
                m_fShiftedX = m_pResultPositionCenterPoint.X - m_pPositionCenterPoint.X;
                m_fShiftedY = m_pResultPositionCenterPoint.Y - m_pPositionCenterPoint.Y;

                m_strTrack += "Position Shifted X=" + m_fShiftedX.ToString();
                m_strTrack += ", Position Shifted Y=" + m_fShiftedY.ToString();
            }

            if (Math.Abs(m_fShiftedY) > m_fShiftPositionTolerance || Math.Abs(m_fShiftedX) > m_fShiftPositionTolerance)
            {
                m_strErrorMessage = "Position Shifted : Set = " + (m_fShiftPositionTolerance / fCalibY).ToString("f5") + " mm,   Result = " + (Math.Max(Math.Abs(m_fShiftedX), Math.Abs(m_fShiftedY)) / fCalibY).ToString("f5") + " mm";
                m_FailPosition = (Math.Max(Math.Abs(m_fShiftedX), Math.Abs(m_fShiftedY)) / fCalibY);
                m_intSealFailMask |= 0x01;
                return false;
            }

            return true;
        }

        public bool CheckPositionPatternPitch(ROI objSearchROI)
        {
            if (m_arrPositionMatcher[m_intPositionTemplateIndex].MinAngle != 0)
                m_arrPositionMatcher[m_intPositionTemplateIndex].MinAngle = 0;
            if (m_arrPositionMatcher[m_intPositionTemplateIndex].MaxAngle != 0)
                m_arrPositionMatcher[m_intPositionTemplateIndex].MaxAngle = 0;
            m_arrPositionMatcher[m_intPositionTemplateIndex].MaxPositions = 2;
            m_arrPositionMatcher[m_intPositionTemplateIndex].FinalReduction = 1;
            m_arrPositionMatcher[m_intPositionTemplateIndex].Interpolate = true;

            ROI objROI = new ROI();
            objSearchROI.CopyTo(ref objROI);
            objROI.LoadROISetting(objSearchROI.ref_ROIPositionX - objSearchROI.ref_ROIWidth * 2, objSearchROI.ref_ROIPositionY,
                                    objSearchROI.ref_ROIWidth * 3, objSearchROI.ref_ROIHeight);

            //m_arrPositionMatcher[m_intPositionTemplateIndex].Save("D:\\TS\\Matcher.mch");
            //objROI.ref_ROI.Save("D:\\TS\\ROI.bmp");
            m_arrPositionMatcher[m_intPositionTemplateIndex].Match(objROI.ref_ROI);

            if (m_arrPositionMatcher[m_intPositionTemplateIndex].NumPositions > 1)     // if macthing result hit the min score, its position will be 1 or more
            {
                if (m_arrPositionMatcher[m_intPositionTemplateIndex].GetPosition(0).Score > 0.7 && m_arrPositionMatcher[m_intPositionTemplateIndex].GetPosition(1).Score > 0.7)
                {
                    //m_intTapeSproketPitch = Convert.ToInt32(Math.Abs(m_arrPositionMatcher[m_intPositionTemplateIndex].GetPosition(0).CenterX - m_arrPositionMatcher[m_intPositionTemplateIndex].GetPosition(1).CenterX));
                }
                else
                    return false;
            }
            else
                return false;

            return true;
        }

        public PointF GetResultPositionCenterPoint_UnitMatcher()
        {
            return m_pPositionCenterPoint;
        }

        public float GetPocketMinScore(int i)
        {
            return m_fPocketMatchScore[i];
        }

        public float GetMarkMinScore(int i)
        {
            return m_fMarkMatchScore[i];
        }

        public int GetSeal1Area()
        {
            if (m_arrBlackBlobs[0].ref_intNumSelectedObject > 0)
                return m_arrBlackBlobs[0].ref_arrArea[0];
            else
                return 0;
        }

        public int GetSeal2Area()
        {
            if (m_arrBlackBlobs[1].ref_intNumSelectedObject > 0)
                return m_arrBlackBlobs[1].ref_arrArea[0];
            else
                return 0;
        }



        /// <summary>
        /// Set grab image index
        /// </summary>
        /// <param name="intArrayIndex">Index which represent different defect inspection. 0: seal</param>
        /// <param name="intGrabImageIndex">Grab image index</param>
        public void SetGrabImageIndex(int intArrayIndex, int intGrabImageIndex)
        {
            if (intArrayIndex >= m_arrGrabImageIndex.Count)
            {
                for (int i = m_arrGrabImageIndex.Count - 1; i <= intArrayIndex; i++)
                {
                    m_arrGrabImageIndex.Add(0);
                }
            }

            m_arrGrabImageIndex[intArrayIndex] = intGrabImageIndex;
        }

        /// <summary>
        /// Get grab image index 
        /// </summary>
        /// <param name="intArrayIndex">Index which represent different defect inspection. 0: Unit Edge</param>
        /// <returns></returns>
        public int GetGrabImageIndex(int intArrayIndex)
        {
            if (intArrayIndex >= m_arrGrabImageIndex.Count)
                return 0;

            return m_arrGrabImageIndex[intArrayIndex];
        }

        public int GetGrabImageIndexCount()
        {
            return m_arrGrabImageIndex.Count;
        }

        public void SaveSealPositionTemplate(string strFolderPath, ROI objPositionROI)
        {
            objPositionROI.SaveImage(strFolderPath + "PositionTemplate0_" + m_intPositionTemplateIndex.ToString() + ".bmp");

            if (objPositionROI.ref_ROIWidth == 0 || objPositionROI.ref_ROIHeight == 0)
                return;

            try
            {
#if (Debug_2_12 || Release_2_12)
                m_arrPositionMatcher[m_intPositionTemplateIndex].AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                m_arrPositionMatcher[m_intPositionTemplateIndex].DontCareThreshold = 1;
                m_arrPositionMatcher[m_intPositionTemplateIndex].MaxPositions = 5;
                m_arrPositionMatcher[m_intPositionTemplateIndex].LearnPattern(objPositionROI.ref_ROI);
            }
            catch (Exception ex)
            {

            }

            m_arrPositionMatcher[m_intPositionTemplateIndex].Save(strFolderPath + "PositionTemplate0_" + m_intPositionTemplateIndex.ToString() + ".mch");
        }

        public void SaveSealPocketTemplate(string strFolderPath, ROI objPocketROI)
        {
            objPocketROI.SaveImage(strFolderPath + "PocketTemplate0_" + m_intPocketTemplateIndex.ToString() + ".bmp");

            if (objPocketROI.ref_ROIWidth == 0 || objPocketROI.ref_ROIHeight == 0)
                return;

            try
            {
#if (Debug_2_12 || Release_2_12)
                m_arrPocketMatcher[m_intPocketTemplateIndex].AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                m_arrPocketMatcher[m_intPocketTemplateIndex].DontCareThreshold = 1;
                m_arrPocketMatcher[m_intPocketTemplateIndex].MaxPositions = 5;
                m_arrPocketMatcher[m_intPocketTemplateIndex].LearnPattern(objPocketROI.ref_ROI);
            }
            catch (Exception ex)
            {

            }

            m_arrPocketMatcher[m_intPocketTemplateIndex].Save(strFolderPath + "PocketTemplate0_" + m_intPocketTemplateIndex.ToString() + ".mch");
        }

        public void SaveSealMarkTemplate4Direction(string strFolderPath, ROI objSourceMarkROI)
        {
            objSourceMarkROI.SaveImage(strFolderPath + "MarkTemplate0_" + m_intMarkTemplateIndex.ToString() + ".bmp");

            if (objSourceMarkROI.ref_ROIWidth == 0 || objSourceMarkROI.ref_ROIHeight == 0)
                return;

            try
            {
                ROI objMarkROI = new ROI();
                ImageDrawing objSourceImage = new ImageDrawing(true);
                objSourceMarkROI.CopyToTopParentImage(ref objSourceImage);
                objMarkROI.AttachImage(objSourceImage);
                objMarkROI.LoadROISetting(objSourceMarkROI.ref_ROITotalX, objSourceMarkROI.ref_ROITotalY, objSourceMarkROI.ref_ROIWidth, objSourceMarkROI.ref_ROIHeight);

                if (m_intCheckMarkMethod == 2)
                {
                    // 2020 10 02 - CCENG,CXLim - Init local Source Image bcos need to do threshold and prewitt image processing on source image
#if (Debug_2_12 || Release_2_12)
                    //EasyImage.Threshold(objMarkROI.ref_ROI, objMarkROI.ref_ROI, (uint)m_intMarkPixelThreshold);
                    EasyImage.Threshold(objMarkROI.ref_ROI, objMarkROI.ref_ROI, (uint)m_arrTemplateMarkThreshold[m_intMarkTemplateIndex]);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    //EasyImage.Threshold(objMarkROI.ref_ROI, objMarkROI.ref_ROI, m_intMarkPixelThreshold);
                    EasyImage.Threshold(objMarkROI.ref_ROI, objMarkROI.ref_ROI, m_arrTemplateMarkThreshold[m_intMarkTemplateIndex]);
#endif

                    EasyImage.ConvolPrewitt(objMarkROI.ref_ROI);
                }
                else if (m_intCheckMarkMethod == 3) //2020-10-09 ZJYEOH : Do image processing based on customize sequence
                {
                    DoImageProcessingSequence(ref objMarkROI, m_intMarkTemplateIndex);
                }

                EImageBW8 objImage = new EImageBW8();
                objImage.SetSize(objMarkROI.ref_ROI.TopParent.Width, objMarkROI.ref_ROI.TopParent.Height);
                EasyImage.Copy(objMarkROI.ref_ROI.TopParent, objImage);

                if (m_intMarkTemplateIndex == m_arrMarkMatcher.Count)
                    m_arrMarkMatcher.Add(new List<EMatcher>());

                for (int i = 0; i < 4; i++)
                {
                    m_arrMarkMatcher[m_intMarkTemplateIndex].Add(new EMatcher());
                    if (i == 0)
                    {
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].MaxPositions = 5;
#if (Debug_2_12 || Release_2_12)
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].LearnPattern(objMarkROI.ref_ROI);
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].Save(strFolderPath + "MarkTemplate0_" + m_intMarkTemplateIndex.ToString() + "_" + i.ToString() + ".mch");
                    }
                    else
                    {
                        EROIBW8 objSourceROI = new EROIBW8();
                        objSourceROI.Detach();
                        objSourceROI.Attach(objMarkROI.ref_ROI.TopParent);
                        float fSquareSize = (float)Math.Max(objMarkROI.ref_ROIWidth, objMarkROI.ref_ROIHeight);
                        float fROICenterX = objMarkROI.ref_ROITotalX + (float)objMarkROI.ref_ROIWidth / 2;
                        float fROICenterY = objMarkROI.ref_ROITotalY + (float)objMarkROI.ref_ROIHeight / 2;
                        objSourceROI.SetPlacement((int)Math.Round(fROICenterX - fSquareSize / 2, 0, MidpointRounding.AwayFromZero),
                                                  (int)Math.Round(fROICenterY - fSquareSize / 2, 0, MidpointRounding.AwayFromZero),
                                                  (int)fSquareSize, (int)fSquareSize);

                        EROIBW8 objDestROI = new EROIBW8();
                        objDestROI.Detach();
                        objDestROI.Attach(objImage);
                        objDestROI.SetPlacement(objSourceROI.OrgX, objSourceROI.OrgY, objSourceROI.Width, objSourceROI.Height);

                        int intRotateAngle;
                        if (i == 1)
                            intRotateAngle = -90;
                        else if (i == 2)
                            intRotateAngle = 180;
                        else
                            intRotateAngle = 90;
                        EasyImage.ScaleRotate(objSourceROI, objSourceROI.Width / 2f, objSourceROI.Height / 2f, objDestROI.Width / 2f, objDestROI.Height / 2f, 1, 1, intRotateAngle, objDestROI, 8);

                        EROIBW8 objPatternROI = new EROIBW8();
                        objPatternROI.Detach();
                        objPatternROI.Attach(objImage);
                        if (i == 2)
                        {
                            objPatternROI.SetPlacement(objMarkROI.ref_ROITotalX, objMarkROI.ref_ROITotalY, objMarkROI.ref_ROIWidth, objMarkROI.ref_ROIHeight);
                        }
                        else
                        {
                            // Width and Height are switched each other   
                            objPatternROI.SetPlacement((int)Math.Round(fROICenterX - (float)objMarkROI.ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                       (int)Math.Round(fROICenterY - (float)objMarkROI.ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                       objMarkROI.ref_ROIHeight, objMarkROI.ref_ROIWidth);
                        }
#if (Debug_2_12 || Release_2_12)
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].AdvancedLearning = false; // 2020-09-23 ZJYEOH : If set to true when MIN MAX angle both are same sign(++/--) then will have error
#endif
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].MaxPositions = 5;
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].LearnPattern(objPatternROI);
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].Save(strFolderPath + "MarkTemplate0_" + m_intMarkTemplateIndex.ToString() + "_" + i.ToString() + ".mch");
                        objSourceROI.Dispose();
                    }

                }

                objSourceImage.Dispose();
                objSourceImage = null;
                objImage.Dispose();
                objMarkROI.Dispose();
            }
#if (Debug_2_12 || Release_2_12)
            catch (Euresys.Open_eVision_2_12.EException ex)
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            catch (Euresys.Open_eVision_1_2.EException ex)
#endif
            {
                m_strErrorMessage = "Orient Learn Pattern Error: " + ex.ToString();
            }
        }

        public void SaveSealTemplateImage(string strFolderPath, List<ImageDrawing> arrTemplateImage)
        {
            for (int i = 0; i < 2; i++)
            {
                if (i == 0)
                    arrTemplateImage[i].SaveImage(strFolderPath + "OriTemplate0.bmp");
                else if (i == 1)
                    arrTemplateImage[i].SaveImage(strFolderPath + "OriTemplate0_Image1.bmp");
            }
        }

        public bool DoUnitPresentInspection_SRM1(ROI objSearchROI, int intFinalReduction)
        {
            return DoUnitPresentInspection_SRM1(objSearchROI, intFinalReduction, m_intMarkTemplateNumSelected);
        }

        public bool DoUnitPresentInspection_SRM1(ROI objSearchROI, int intFinalReduction, int intTemplateNo)
        {
            intTemplateNo = m_intMarkTemplateNumSelected;
            m_strErrorMessage = "";

            if (intTemplateNo >= m_arrMarkMatcher.Count)
                return false;

            objSearchROI.ref_ROIPositionX = objSearchROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
            objSearchROI.ref_ROIPositionY = objSearchROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);

            // Check pattern file exist
            if (m_arrMarkMatcher.Count == 0)
            {
                m_strErrorMessage = "Fail Unit Mark - No Pattern Learnt";
                m_intSealFailMask |= 0x80;

                return false;
            }

            // Check are patterns learnt
            if (!m_arrMarkMatcher[intTemplateNo][0].PatternLearnt)
            {
                m_strErrorMessage = "Fail Unit Mark - No Pattern Learnt";
                m_intSealFailMask |= 0x80;

                return false;
            }

            if (m_arrMarkMatcher[intTemplateNo][0].PatternWidth > objSearchROI.ref_ROI.Width ||
                m_arrMarkMatcher[intTemplateNo][0].PatternHeight > objSearchROI.ref_ROI.Height)
            {
                m_strErrorMessage = "Fail Unit Mark - Search ROI too small to find mark pattern";
                m_intSealFailMask |= 0x80;

                return false;
            }
#if (Debug_2_12 || Release_2_12)
            // Make sure basic setting for matcher file
            if (m_arrMarkMatcher[intTemplateNo][0].FinalReduction != intFinalReduction)
                m_arrMarkMatcher[intTemplateNo][0].FinalReduction = (uint)intFinalReduction;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Make sure basic setting for matcher file
            if (m_arrMarkMatcher[intTemplateNo][0].FinalReduction != intFinalReduction)
                m_arrMarkMatcher[intTemplateNo][0].FinalReduction = intFinalReduction;

#endif

            // 2018 08 20 - JBTAN: temporary put +-5 degree, to see if better
            // 2020 06 23 - CCENG: Change from +-5 deg to +-10 deg. Get better Score after change to 10 deg.
            if (m_arrMarkMatcher[intTemplateNo][0].MinAngle != -10)
                m_arrMarkMatcher[intTemplateNo][0].MinAngle = -10;
            if (m_arrMarkMatcher[intTemplateNo][0].MaxAngle != 10)
                m_arrMarkMatcher[intTemplateNo][0].MaxAngle = 10;
            if (m_arrMarkMatcher[intTemplateNo][0].MinScale != 1)
                m_arrMarkMatcher[intTemplateNo][0].MinScale = 1;
            if (m_arrMarkMatcher[intTemplateNo][0].MaxScale != 1)
                m_arrMarkMatcher[intTemplateNo][0].MaxScale = 1;

            bool blnDebug = false;
            if (blnDebug)
            {
                objSearchROI.ref_ROI.Save("D:\\TS\\objSearchROI.bmp");
                m_arrMarkMatcher[intTemplateNo][0].Save("D:\\TS\\Matcher.mch");
            }

            m_arrMarkMatcher[intTemplateNo][0].Match(objSearchROI.ref_ROI);



            if (m_arrMarkMatcher[intTemplateNo][0].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
            {
                m_fMarkMatchScore[intTemplateNo] = m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).Score;

                ROI objROI = new ROI();
                objROI.AttachImage(objSearchROI);

                int intStartX = (int)Math.Round(m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).CenterX - (float)m_arrMarkMatcher[intTemplateNo][0].PatternWidth / 2, 0, MidpointRounding.AwayFromZero);
                int intStartY = (int)Math.Round(m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).CenterY - (float)m_arrMarkMatcher[intTemplateNo][0].PatternHeight / 2, 0, MidpointRounding.AwayFromZero);
                int intWitdh = m_arrMarkMatcher[intTemplateNo][0].PatternWidth;
                int intHeight = m_arrMarkMatcher[intTemplateNo][0].PatternHeight;

                objROI.LoadROISetting(intStartX, intStartY, intWitdh, intHeight);

                bool blnWantDebug = false;
                if (blnWantDebug)
                {
                    objROI.SaveImage("D:\\TS\\objROI.bmp");
                }

                TemplateImageMatching_SRM1(objROI, intTemplateNo, 0);

                objROI.Dispose();
                m_strTrack2 += "[" + m_fMarkMatchScore[intTemplateNo].ToString() + "],[" + m_fImageMatchScore.ToString() + "]";
                m_fMarkMatchScore[intTemplateNo] = Math.Min(m_fMarkMatchScore[intTemplateNo], m_fImageMatchScore);

                if (m_fMarkMatchScore[intTemplateNo] < m_fMarkMinScore)
                {
                    m_strErrorMessage = "Fail Unit Mark - Not fulfill Min Setting : Set = " + (m_fMarkMinScore * 100).ToString() +
                                        " Score = " + (m_fMarkMatchScore[intTemplateNo] * 100).ToString("f5");
                    m_intSealFailMask |= 0x80;
                }
                else
                {
                    m_pResultMarkCenterPoint = new PointF(objSearchROI.ref_ROITotalX + m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).CenterX,
                            objSearchROI.ref_ROITotalY + m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).CenterY);
                    m_SResultMarkSize = new SizeF(m_arrMarkMatcher[intTemplateNo][0].PatternWidth, m_arrMarkMatcher[intTemplateNo][0].PatternHeight);
                    m_fResultMarkAngle = m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).Angle;
                    m_intSealFailMask &= ~0x80;
                    //m_intMarkTemplateNumSelected = intTemplateNo;
                    return true;
                }
            }
            else
            {
                m_strErrorMessage = "Fail Unit Mark - Find 0 Pattern in Search ROI";
                m_intSealFailMask |= 0x80;
            }

            return false;
        }

        public bool DoUnitPresentInspection(ROI objSearchROI, int intFinalReduction)
        {
            return DoUnitPresentInspection(objSearchROI, intFinalReduction, m_intMarkTemplateNumSelected);
        }

        public bool DoUnitPresentInspection(ROI objSearchROI, int intFinalReduction, int intTemplateNo)
        {
            m_strErrorMessage = "";

            if (intTemplateNo >= m_arrMarkMatcher.Count)
                return false;

            objSearchROI.ref_ROIPositionX = objSearchROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
            objSearchROI.ref_ROIPositionY = objSearchROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);

            // Check pattern file exist
            if (m_arrMarkMatcher.Count == 0)
            {
                m_strErrorMessage = "Fail Unit Mark - No Pattern Learnt";
                m_intSealFailMask |= 0x80;

                return false;
            }

            // Check are patterns learnt
            if (!m_arrMarkMatcher[intTemplateNo][0].PatternLearnt)
            {
                m_strErrorMessage = "Fail Unit Mark - No Pattern Learnt";
                m_intSealFailMask |= 0x80;

                return false;
            }

            if (m_arrMarkMatcher[intTemplateNo][0].PatternWidth > objSearchROI.ref_ROI.Width ||
                m_arrMarkMatcher[intTemplateNo][0].PatternHeight > objSearchROI.ref_ROI.Height)
            {
                m_strErrorMessage = "Fail Unit Mark - Search ROI too small to find mark pattern";
                m_intSealFailMask |= 0x80;

                return false;
            }
#if (Debug_2_12 || Release_2_12)
            // Make sure basic setting for matcher file
            if (m_arrMarkMatcher[intTemplateNo][0].FinalReduction != intFinalReduction)
                m_arrMarkMatcher[intTemplateNo][0].FinalReduction = (uint)intFinalReduction;
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
            // Make sure basic setting for matcher file
            if (m_arrMarkMatcher[intTemplateNo][0].FinalReduction != intFinalReduction)
                m_arrMarkMatcher[intTemplateNo][0].FinalReduction = intFinalReduction;
#endif

            // 2018 08 20 - JBTAN: temporary put +-5 degree, to see if better
            // 2020 06 23 - CCENG: Change from +-5 deg to +-10 deg. Get better Score after change to 10 deg.
            if (m_arrMarkMatcher[intTemplateNo][0].MinAngle != -10)
                m_arrMarkMatcher[intTemplateNo][0].MinAngle = -10;
            if (m_arrMarkMatcher[intTemplateNo][0].MaxAngle != 10)
                m_arrMarkMatcher[intTemplateNo][0].MaxAngle = 10;
            if (m_arrMarkMatcher[intTemplateNo][0].MinScale != 1)
                m_arrMarkMatcher[intTemplateNo][0].MinScale = 1;
            if (m_arrMarkMatcher[intTemplateNo][0].MaxScale != 1)
                m_arrMarkMatcher[intTemplateNo][0].MaxScale = 1;

            bool blnDebug = false;
            if (blnDebug)
            {
                objSearchROI.ref_ROI.Save("D:\\TS\\objSearchROI.bmp");
                m_arrMarkMatcher[intTemplateNo][0].Save("D:\\TS\\Matcher.mch");
            }

            m_arrMarkMatcher[intTemplateNo][0].Match(objSearchROI.ref_ROI);



            if (m_arrMarkMatcher[intTemplateNo][0].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
            {
                m_fMarkMatchScore[intTemplateNo] = m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).Score;

                if (m_fMarkMatchScore[intTemplateNo] < m_fMarkMinScore)
                {
                    m_strErrorMessage = "Fail Unit Mark - Not fulfill Min Setting : Set = " + (m_fMarkMinScore * 100).ToString() +
                                        " Score = " + (m_fMarkMatchScore[intTemplateNo] * 100).ToString("f5");
                    m_intSealFailMask |= 0x80;
                }
                else
                {
                    m_pResultMarkCenterPoint = new PointF(objSearchROI.ref_ROITotalX + m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).CenterX,
                            objSearchROI.ref_ROITotalY + m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).CenterY);
                    m_SResultMarkSize = new SizeF(m_arrMarkMatcher[intTemplateNo][0].PatternWidth, m_arrMarkMatcher[intTemplateNo][0].PatternHeight);
                    m_fResultMarkAngle = m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).Angle;
                    m_intSealFailMask &= ~0x80;
                    m_intMarkTemplateNumSelected = intTemplateNo;
                    return true;
                }
            }
            else
            {
                m_strErrorMessage = "Fail Unit Mark - Find 0 Pattern in Search ROI";
                m_intSealFailMask |= 0x80;
            }

            return false;
        }

        public bool DoUnitPresentInspection_PixelQuantity(ROI objSearchROI, int intTemplateNo)
        {
            intTemplateNo = m_intMarkTemplateNumSelected;
            m_strErrorMessage = "";

            if (!m_blnWantUsePixelCheckUnitPresent)
                return true;

            ROI objUnitPresentROI = new ROI();
            objUnitPresentROI.AttachImage(objSearchROI);
            objUnitPresentROI.LoadROISetting((int)(m_pResultMarkCenterPoint.X - objSearchROI.ref_ROITotalX - m_arrMarkMatcher[intTemplateNo][0].PatternWidth / 2),
                                             (int)(m_pResultMarkCenterPoint.Y - objSearchROI.ref_ROITotalY - m_arrMarkMatcher[intTemplateNo][0].PatternHeight / 2),
                                             m_arrMarkMatcher[intTemplateNo][0].PatternWidth,
                                             m_arrMarkMatcher[intTemplateNo][0].PatternHeight);

            int intWhiteOnBlack = 1;
            string strWhiteOnBlack = "White";
            if (!m_blnWhiteOnBlack)
            {
                intWhiteOnBlack = 0;
                strWhiteOnBlack = "Black";
            }

            // Check thresholded pixel quantity
            if (intTemplateNo < m_arrTemplateMarkThreshold.Count)
                m_FailUnitPresentWhiteArea = ROI.GetPixelArea(objUnitPresentROI, m_arrTemplateMarkThreshold[intTemplateNo], intWhiteOnBlack);
            else
                m_FailUnitPresentWhiteArea = ROI.GetPixelArea(objUnitPresentROI, m_intMarkPixelThreshold, intWhiteOnBlack);

            objUnitPresentROI.Dispose();
            objUnitPresentROI = null;

            bool blnUsingPercentage = true;
            if (blnUsingPercentage)
            {
                float fSmallestGapPercent = 0;
                float fSelectedPercentwhiteArea = 0;
                for (int i = 0; i < m_arrTemplateMarkWhitePixel.Count; i++)
                {
                    float fPercentWhiteArea = m_FailUnitPresentWhiteArea / m_arrTemplateMarkWhitePixel[i] * 100;

                    if (i == 0)
                    {
                        fSmallestGapPercent = Math.Abs(fPercentWhiteArea - 100);
                        fSelectedPercentwhiteArea = fPercentWhiteArea;
                    }
                    else
                    {


                        if (fSmallestGapPercent > Math.Abs(fPercentWhiteArea - 100))
                        {
                            fSmallestGapPercent = Math.Abs(fPercentWhiteArea - 100);
                            fSelectedPercentwhiteArea = fPercentWhiteArea;
                        }
                    }
                }

                m_FailUnitPresentWhiteArea = fSelectedPercentwhiteArea;

                //m_FailUnitPresentWhiteArea = m_FailUnitPresentWhiteArea / m_arrTemplateMarkWhitePixel[intTemplateNo] * 100;

                if ((m_FailUnitPresentWhiteArea < m_fMarkMaxWhiteArea) && (m_FailUnitPresentWhiteArea > m_fMarkMinWhiteArea))
                {
                    return true;
                }
                else
                {
                    if (m_FailUnitPresentWhiteArea > m_fMarkMaxWhiteArea)
                        m_strErrorMessage = "Fail Unit Mark - Set " + strWhiteOnBlack + " Maximum Pixel=" + m_fMarkMaxWhiteArea.ToString() + "%, Result=" + m_FailUnitPresentWhiteArea.ToString() + "%";
                    else
                        m_strErrorMessage = "Fail Unit Mark - Set " + strWhiteOnBlack + " Minimum Pixel=" + m_fMarkMinWhiteArea.ToString() + "%, Result=" + m_FailUnitPresentWhiteArea.ToString() + "%";
                    m_intSealFailMask |= 0x80;
                    m_intSealFailMask |= 0x400; // 2020-02-06 ZJYEOH : To separate defect from unit present for displaying on offline page
                    return false;
                }
            }
            else
            {
                if ((m_FailUnitPresentWhiteArea < m_fMarkMaxWhiteArea) && (m_FailUnitPresentWhiteArea > m_fMarkMinWhiteArea))
                {
                    return true;
                }
                else
                {
                    if (m_FailUnitPresentWhiteArea > m_fMarkMaxWhiteArea)
                        m_strErrorMessage = "Fail Unit Mark - Set " + strWhiteOnBlack + " Maximum Pixel=" + m_fMarkMaxWhiteArea.ToString() + ", Result=" + m_FailUnitPresentWhiteArea.ToString();
                    else
                        m_strErrorMessage = "Fail Unit Mark - Set " + strWhiteOnBlack + " Minimum Pixel=" + m_fMarkMinWhiteArea.ToString() + ", Result=" + m_FailUnitPresentWhiteArea.ToString();
                    m_intSealFailMask |= 0x80;
                    m_intSealFailMask |= 0x400; // 2020-02-06 ZJYEOH : To separate defect from unit present for displaying on offline page
                    return false;
                }
            }
        }

        public bool DoSealOrientationInspection_MultiTemplate2(ROI objSearchROI, int intFinalReduction, int intMarkTemplateMask, bool bWantOrientation)
        {
            string strTrack = "";
            bool blnMarkScorePass = false;

            try
            {
                // Reset inspection result
                m_strErrorMessage = "";
                m_intAngleResult = 0;
                m_CheckUnitOrient = false;
                m_intMarkTemplateIndex = m_intMarkTemplateNumSelected = 0;

                strTrack += "1,";
                objSearchROI.ref_ROIPositionX = objSearchROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
                objSearchROI.ref_ROIPositionY = objSearchROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);
                strTrack += "2,";
                // Check pattern file exist
                if (m_arrMarkMatcher.Count == 0)
                {
                    m_strErrorMessage = "Fail Unit Mark - No Pattern Learnt";
                    m_intSealFailMask |= 0x80;
                    return blnMarkScorePass;
                }
                strTrack += "3,";
                // Check are all patterns learnt
                for (int i = 0; i < m_arrMarkMatcher.Count; i++)
                {
                    for (int j = 0; j < m_arrMarkMatcher[i].Count; j++)
                    {
                        if (!m_arrMarkMatcher[i][j].PatternLearnt)
                            return blnMarkScorePass;
                    }
                }

                float[] arrAngleResult = new float[8];
                float fHighestScore = -1.0f;
                for (int intTemplateNo = 0; intTemplateNo < 8; intTemplateNo++)
                {
                    int intMatcherIndex = 0;

                    strTrack += "4,";
                    if (intTemplateNo >= m_arrMatcherScoreRecord.Count)
                    {
                        m_arrMatcherScoreRecord.Add(new List<float>());
                    }
                    else
                    {
                        m_arrMatcherScoreRecord[intTemplateNo].Clear();
                    }

                    if ((intMarkTemplateMask & (0x01 << intTemplateNo)) <= 0)
                        continue;

                    //List<float> arrMatcherScoreRecord = new List<float>();
                    // ------------------- checking loop timeout ---------------------------------------------------
                    HiPerfTimer timeout = new HiPerfTimer();
                    timeout.Start();

                    do
                    {
                        // ------------------- checking loop timeout ---------------------------------------------------
                        if (timeout.Timing > 10000)
                        {
                            STTrackLog.WriteLine(">>>>>>>>>>>>> time out 101");
                            break;
                        }
                        // ---------------------------------------------------------------------------------------------

                        if (intTemplateNo >= m_arrMarkMatcher.Count)
                            break;
#if (Debug_2_12 || Release_2_12)
                        // Make sure basic setting for matcher file
                        if (m_arrMarkMatcher[intTemplateNo][intMatcherIndex].FinalReduction != intFinalReduction)
                            m_arrMarkMatcher[intTemplateNo][intMatcherIndex].FinalReduction = (uint)intFinalReduction;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        // Make sure basic setting for matcher file
                        if (m_arrMarkMatcher[intTemplateNo][intMatcherIndex].FinalReduction != intFinalReduction)
                            m_arrMarkMatcher[intTemplateNo][intMatcherIndex].FinalReduction = intFinalReduction;

#endif

                        // 2018 08 20 - JBTAN: temporary put +-5 degree, to see if better
                        // 2020 06 23 - CCENG: Change from +-5 deg to +-10 deg. Get better Score after change to 10 deg.
                        if (m_arrMarkMatcher[intTemplateNo][0].MinAngle != -10)
                            m_arrMarkMatcher[intTemplateNo][0].MinAngle = -10;
                        if (m_arrMarkMatcher[intTemplateNo][0].MaxAngle != 10)
                            m_arrMarkMatcher[intTemplateNo][0].MaxAngle = 10;
                        if (m_arrMarkMatcher[intTemplateNo][0].MinScale != 1)
                            m_arrMarkMatcher[intTemplateNo][0].MinScale = 1;
                        if (m_arrMarkMatcher[intTemplateNo][0].MaxScale != 1)
                            m_arrMarkMatcher[intTemplateNo][0].MaxScale = 1;

                        strTrack += "5,";

                        // Start pattern match
                        m_arrMarkMatcher[intTemplateNo][intMatcherIndex].Match(objSearchROI.ref_ROI);

                        strTrack += "6,";
                        if (m_arrMarkMatcher[intTemplateNo][intMatcherIndex].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                        {
                            strTrack += "7,";
                            float fScore = Math.Max(0, m_arrMarkMatcher[intTemplateNo][intMatcherIndex].GetPosition(0).Score);

                            if (m_intCheckMarkMethod == 1)  // For SRM1 mode
                            {
                                ROI objROI = new ROI();
                                objROI.AttachImage(objSearchROI);

                                int intStartX = (int)Math.Round(m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).CenterX - (float)m_arrMarkMatcher[intTemplateNo][0].PatternWidth / 2, 0, MidpointRounding.AwayFromZero);
                                int intStartY = (int)Math.Round(m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).CenterY - (float)m_arrMarkMatcher[intTemplateNo][0].PatternHeight / 2, 0, MidpointRounding.AwayFromZero);
                                int intWitdh = m_arrMarkMatcher[intTemplateNo][0].PatternWidth;
                                int intHeight = m_arrMarkMatcher[intTemplateNo][0].PatternHeight;

                                objROI.LoadROISetting(intStartX, intStartY, intWitdh, intHeight);

                                bool blnWantDebug = false;
                                if (blnWantDebug)
                                {
                                    objROI.SaveImage("D:\\TS\\objROI.bmp");
                                }

                                TemplateImageMatching_SRM1(objROI, intTemplateNo, intMatcherIndex);

                                objROI.Dispose();
                                fScore = Math.Min(fScore, m_fImageMatchScore);
                            }

                            m_arrMatcherScoreRecord[intTemplateNo].Add(fScore);

                            strTrack += "8,";
                            if (fScore > fHighestScore)
                            {
                                strTrack += "9,";
                                fHighestScore = fScore;

                                if (intMatcherIndex == 0)
                                    m_fMarkMatchScore[intTemplateNo] = fScore;

                                strTrack += "10,";
                                m_pResultMarkCenterPoint = new PointF(objSearchROI.ref_ROITotalX + m_arrMarkMatcher[intTemplateNo][intMatcherIndex].GetPosition(0).CenterX,
                                                            objSearchROI.ref_ROITotalY + m_arrMarkMatcher[intTemplateNo][intMatcherIndex].GetPosition(0).CenterY);
                                m_SResultMarkSize = new SizeF(m_arrMarkMatcher[intTemplateNo][intMatcherIndex].PatternWidth, m_arrMarkMatcher[intTemplateNo][intMatcherIndex].PatternHeight);
                                m_fResultMarkAngle = m_arrMarkMatcher[intTemplateNo][intMatcherIndex].GetPosition(0).Angle;
                                strTrack += "11,";
                                m_intMarkTemplateIndex = m_intMarkTemplateNumSelected = intTemplateNo;

                                strTrack += "12,";

                                arrAngleResult[intTemplateNo] = intMatcherIndex;

                                m_intAngleResult = intMatcherIndex;

                                strTrack += "14,";
                            }
                        }
                        else
                        {
                            m_arrMatcherScoreRecord[intTemplateNo].Add(0);
                        }

                        strTrack += "15,";
                        if (m_intDirections == 4)
                            intMatcherIndex++;     // For square unit 0, 90, 180, -90 deg
                        else
                        {
                            m_arrMatcherScoreRecord[intTemplateNo].Add(0);
                            intMatcherIndex += 2; //For rectangle unit 0nly 0 deg and 180 deg
                        }

                        if (!bWantOrientation)
                        {
                            break;
                        }

                        strTrack += "16,";
                    } while (intMatcherIndex < 4);
                    timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------
                }
                strTrack += "17,";
                m_strTrack += ", Mark Center X=" + m_pResultMarkCenterPoint.X.ToString();
                m_strTrack += ", Mark Center Y=" + m_pResultMarkCenterPoint.Y.ToString();



                //m_intMarkTemplateIndex = 0;
                //float fTemplateHighestScore = 0;
                //for (int intTemplateNo = 0; intTemplateNo < m_arrMatcherScoreRecord.Count; intTemplateNo++)
                //{
                //    if ((intMarkTemplateMask & (0x01 << intTemplateNo)) <= 0)
                //        continue;

                //    for (int intMatcherScoreIndex = 0; intMatcherScoreIndex < m_arrMatcherScoreRecord[intTemplateNo].Count; intMatcherScoreIndex++)
                //    {
                //        if (fTemplateHighestScore < m_arrMatcherScoreRecord[intTemplateNo][intMatcherScoreIndex])
                //        {
                //            fTemplateHighestScore = m_arrMatcherScoreRecord[intTemplateNo][intMatcherScoreIndex];
                //            m_intMarkTemplateIndex = intTemplateNo;
                //            m_intAngleResult = intMatcherScoreIndex;
                //        }
                //    }
                //}

                if (m_intAngleResult == 0)
                {
                    if (m_fMarkMatchScore[m_intMarkTemplateNumSelected] < m_fMarkMinScore)
                    {
                        m_strErrorMessage = "Fail Unit Mark - Not fulfill Min Setting : Set = " + (m_fMarkMinScore * 100).ToString() +
                                            " Score = " + (m_fMarkMatchScore[m_intMarkTemplateNumSelected] * 100).ToString("f5");
                        m_intSealFailMask |= 0x80;
                    }
                    else
                        blnMarkScorePass = true;
                }
                else
                {
                    if (m_intAngleResult == 1)
                    {
                        strTrack += "18,";
                        m_strErrorMessage = "Fail Orient Angle : 90 deg";
                        m_intSealFailMask |= 0x1000;
                        m_CheckUnitOrient = true;
                    }
                    if (m_intAngleResult == 2)
                    {
                        strTrack += "19,";
                        m_strErrorMessage = "Fail Orient Angle : 180 deg";
                        m_intSealFailMask |= 0x1000;
                        m_CheckUnitOrient = true;
                    }
                    if (m_intAngleResult == 3)
                    {
                        strTrack += "20,";
                        m_strErrorMessage = "Fail Orient Angle : -90 deg";
                        m_intSealFailMask |= 0x1000;
                        m_CheckUnitOrient = true;
                    }
                    if (m_intAngleResult == 4)
                    {
                        strTrack += "21,";
                        m_strErrorMessage = "Fail Mark - Not fulfill Min Setting : Set = " + (m_fMarkMinScore * 100).ToString() +
                            " Score = " + (m_fMarkMatchScore[m_intMarkTemplateIndex] * 100).ToString("f5");
                        m_intSealFailMask |= 0x80;
                        m_CheckUnitOrient = true;
                    }

                    string strMatcherScoreRecord = "";
                    for (int i = 0; i < m_arrMatcherScoreRecord[m_intMarkTemplateIndex].Count; i++)
                    {
                        if (m_intDirections != 4)
                        {
                            if (i == 1 || i == 3)
                                continue;
                        }

                        strMatcherScoreRecord += "Score " + (i * 90).ToString() + "deg=" + (m_arrMatcherScoreRecord[m_intMarkTemplateIndex][i] * 100).ToString("F2") + "%";

                        if (i < m_arrMatcherScoreRecord.Count - 1)
                        {
                            strMatcherScoreRecord += ", ";
                        }
                    }
                    m_strErrorMessage += "*" + strMatcherScoreRecord;
                }
                strTrack += "22,";

            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("DoSealOrientationInspection strack=" + strTrack + " , exp = " + ex.ToString());
            }

            return blnMarkScorePass;
        }

        public bool DoSealOrientationInspection_MultiTemplate(ROI objSearchROI, ROI objMarkROI, int intFinalReduction, int intMarkTemplateMask, bool bWantOrientation)
        {
            string strTrack = "";
            bool blnMarkScorePass = false;

            try
            {

                // Reset inspection result
                m_strErrorMessage = "";
                m_intAngleResult = 0;
                m_CheckUnitOrient = false;
                m_intMarkTemplateIndex = m_intMarkTemplateNumSelected = 0;

                strTrack += "1,";
                objSearchROI.ref_ROIPositionX = objSearchROI.ref_ROIOriPositionX + Convert.ToInt32(m_fShiftedX);
                objSearchROI.ref_ROIPositionY = objSearchROI.ref_ROIOriPositionY + Convert.ToInt32(m_fShiftedY);

                //                objSearchROI.CopyToTopParentImage(ref m_objTempSampleThresholdImage);
                //                m_objSampleSearchThresholdROI.AttachImage(m_objTempSampleThresholdImage);
                //                m_objSampleSearchThresholdROI.LoadROISetting(objSearchROI.ref_ROITotalX, objSearchROI.ref_ROITotalY, objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);

                //                if (m_intCheckMarkMethod == 2)
                //                {
                //#if (Debug_2_12 || Release_2_12)
                //                    EasyImage.Threshold(m_objSampleSearchThresholdROI.ref_ROI, m_objSampleSearchThresholdROI.ref_ROI, (uint)m_intMarkPixelThreshold);
                //#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                //                    EasyImage.Threshold(m_objSampleSearchThresholdROI.ref_ROI, m_objSampleSearchThresholdROI.ref_ROI, m_intMarkPixelThreshold);
                //#endif
                //                    EasyImage.ConvolPrewitt(m_objSampleSearchThresholdROI.ref_ROI);
                //                }

                strTrack += "2,";
                // Check pattern file exist
                if (m_arrMarkMatcher.Count == 0)
                {
                    m_strErrorMessage = "Fail Unit Mark - No Pattern Learnt";
                    m_intSealFailMask |= 0x80;
                    return blnMarkScorePass;
                }
                strTrack += "3,";
                // Check are all patterns learnt
                for (int i = 0; i < m_arrMarkMatcher.Count; i++)
                {
                    for (int j = 0; j < m_arrMarkMatcher[i].Count; j++)
                    {
                        if (!m_arrMarkMatcher[i][j].PatternLearnt)
                        {
                            m_strErrorMessage = "Fail Unit Mark - No Pattern Learnt";
                            m_intSealFailMask |= 0x80;
                            return blnMarkScorePass;
                        }
                    }
                }
                m_intTemplateErodeTol = 0;
                m_intTemplateDilateTol = 0;
                m_intTemplateOpenTol = 0;
                m_intTemplateCloseTol = 0;
                m_intTemplateThresholdTol = 0;
                float[] arrAngleResult = new float[8];
                float fHighestScore = -1.0f;
                float fOrientHighestScore = -1.0f;
                for (int intTemplateNo = 0; intTemplateNo < 8; intTemplateNo++)
                {
                    bool blnErodeDone = true;
                    bool blnDilateDone = true;
                    bool blnOpenDone = true;
                    bool blnCloseDone = true;
                    bool blnThresholdDone = true;
                    bool blnErodeMaxDone = false;
                    bool blnDilateMaxDone = false;
                    bool blnOpenMaxDone = false;
                    bool blnCloseMaxDone = false;
                    bool blnThresholdMaxDone = false;
                    bool blnThresholdMinDone = false;
                    int intThreshold_Pos = 0;
                    int intThreshold_Neg = 0;
                    bool blnThresholdNegTurn = false;
                    bool blnErodeDilateFirst = false;
                    int intThresholdSeq = 0; // 0 = Threshold first, 1 = Threshold center, 2 = Threshold last

                    if (m_arrTemplateImageProcessSeq.Count > intTemplateNo)
                    {
                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Erode"))
                            blnErodeDone = false;
                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Dilate"))
                            blnDilateDone = false;
                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Open"))
                            blnOpenDone = false;
                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Close"))
                            blnCloseDone = false;
                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold"))
                            blnThresholdDone = false;

                        int intErodeIndex = m_arrTemplateImageProcessSeq[intTemplateNo].IndexOf("Erode");
                        if (intErodeIndex < 0)
                            intErodeIndex = 100;
                        int intDilateIndex = m_arrTemplateImageProcessSeq[intTemplateNo].IndexOf("Dilate");
                        if (intDilateIndex < 0)
                            intDilateIndex = 100;
                        int intOpenIndex = m_arrTemplateImageProcessSeq[intTemplateNo].IndexOf("Open");
                        if (intOpenIndex < 0)
                            intOpenIndex = 100;
                        int intCloseIndex = m_arrTemplateImageProcessSeq[intTemplateNo].IndexOf("Close");
                        if (intCloseIndex < 0)
                            intCloseIndex = 100;
                        int intThresholdIndex = m_arrTemplateImageProcessSeq[intTemplateNo].IndexOf("Threshold");
                        if (intThresholdIndex < 0)
                            intThresholdIndex = 100;
                        if ((intErodeIndex < intOpenIndex) ||
                             (intErodeIndex < intCloseIndex) ||
                             (intDilateIndex < intOpenIndex) ||
                             (intDilateIndex < intCloseIndex))
                            blnErodeDilateFirst = true;

                        if (blnErodeDilateFirst)
                        {
                            if ((intThresholdIndex < intErodeIndex) && (intThresholdIndex < intDilateIndex))
                            {
                                intThresholdSeq = 0;
                            }
                            else
                            {
                                if ((intThresholdIndex < intOpenIndex) && (intThresholdIndex < intCloseIndex))
                                {
                                    intThresholdSeq = 1;
                                }
                                else
                                {
                                    intThresholdSeq = 2;
                                }
                            }
                        }
                        else
                        {
                            if ((intThresholdIndex < intOpenIndex) && (intThresholdIndex < intCloseIndex))
                            {
                                intThresholdSeq = 0;
                            }
                            else
                            {
                                if ((intThresholdIndex < intErodeIndex) && (intThresholdIndex < intDilateIndex))
                                {
                                    intThresholdSeq = 1;
                                }
                                else
                                {
                                    intThresholdSeq = 2;
                                }
                            }
                        }
                    }
                Repeat:
                    int intMatcherIndex = 0;

                    strTrack += "4,";
                    if (intTemplateNo >= m_arrMatcherScoreRecord.Count)
                    {
                        m_arrMatcherScoreRecord.Add(new List<float>());
                    }
                    else
                    {
                        m_arrMatcherScoreRecord[intTemplateNo].Clear();
                    }

                    if ((intMarkTemplateMask & (0x01 << intTemplateNo)) <= 0)
                        continue;

                    objSearchROI.CopyToTopParentImage(ref m_objTempSampleThresholdImage);

                    m_objSampleSearchThresholdROI.AttachImage(m_objTempSampleThresholdImage);
                    m_objSampleSearchThresholdROI.LoadROISetting(objSearchROI.ref_ROITotalX, objSearchROI.ref_ROITotalY, objSearchROI.ref_ROIWidth, objSearchROI.ref_ROIHeight);

                    if (m_intCheckMarkMethod == 2)
                    {
#if (Debug_2_12 || Release_2_12)
                        if (intTemplateNo < m_arrTemplateMarkThreshold.Count)
                            EasyImage.Threshold(m_objSampleSearchThresholdROI.ref_ROI, m_objSampleSearchThresholdROI.ref_ROI, (uint)m_arrTemplateMarkThreshold[intTemplateNo]);
                        else
                            EasyImage.Threshold(m_objSampleSearchThresholdROI.ref_ROI, m_objSampleSearchThresholdROI.ref_ROI, (uint)m_intMarkPixelThreshold);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        if (intTemplateNo < m_arrTemplateMarkThreshold.Count)
                            EasyImage.Threshold(m_objSampleSearchThresholdROI.ref_ROI, m_objSampleSearchThresholdROI.ref_ROI, m_arrTemplateMarkThreshold[intTemplateNo]);
                        else
                            EasyImage.Threshold(m_objSampleSearchThresholdROI.ref_ROI, m_objSampleSearchThresholdROI.ref_ROI, m_intMarkPixelThreshold);
#endif
                        EasyImage.ConvolPrewitt(m_objSampleSearchThresholdROI.ref_ROI);
                    }
                    else if (m_intCheckMarkMethod == 3) //2020-10-09 ZJYEOH : Do image processing based on customize sequence
                    {
                        DoImageProcessingSequence(ref m_objSampleSearchThresholdROI, objMarkROI, intTemplateNo);
                    }

                    //List<float> arrMatcherScoreRecord = new List<float>();
                    // ------------------- checking loop timeout ---------------------------------------------------
                    HiPerfTimer timeout = new HiPerfTimer();
                    timeout.Start();

                    do
                    {
                        // ------------------- checking loop timeout ---------------------------------------------------
                        if (timeout.Timing > 10000)
                        {
                            STTrackLog.WriteLine(">>>>>>>>>>>>> time out 101");
                            break;
                        }
                        // ---------------------------------------------------------------------------------------------

                        if (intTemplateNo >= m_arrMarkMatcher.Count)
                            break;
#if (Debug_2_12 || Release_2_12)
                        // Make sure basic setting for matcher file
                        if (m_arrMarkMatcher[intTemplateNo][intMatcherIndex].FinalReduction != intFinalReduction)
                            m_arrMarkMatcher[intTemplateNo][intMatcherIndex].FinalReduction = (uint)intFinalReduction;

#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        // Make sure basic setting for matcher file
                        if (m_arrMarkMatcher[intTemplateNo][intMatcherIndex].FinalReduction != intFinalReduction)
                            m_arrMarkMatcher[intTemplateNo][intMatcherIndex].FinalReduction = intFinalReduction;

#endif

                        if (m_intCheckMarkMethod == 2)  // Prewitt mode
                        {
                            if (m_arrMarkMatcher[intTemplateNo][intMatcherIndex].CorrelationMode != ECorrelationMode.Standard)
                                m_arrMarkMatcher[intTemplateNo][intMatcherIndex].CorrelationMode = ECorrelationMode.Standard;
                        }
                        else if (m_intCheckMarkMethod == 3)  // Customize mode
                        {
                            if (m_arrTemplateImageProcessSeq.Count > intTemplateNo)
                            {
                                if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Prewitt"))
                                {
                                    if (m_arrMarkMatcher[intTemplateNo][intMatcherIndex].CorrelationMode != ECorrelationMode.Standard)
                                        m_arrMarkMatcher[intTemplateNo][intMatcherIndex].CorrelationMode = ECorrelationMode.Standard;
                                }
                                else
                                {
                                    if (m_arrMarkMatcher[intTemplateNo][intMatcherIndex].CorrelationMode != ECorrelationMode.Normalized)
                                        m_arrMarkMatcher[intTemplateNo][intMatcherIndex].CorrelationMode = ECorrelationMode.Normalized;
                                }
                            }
                            else
                            {
                                if (m_arrMarkMatcher[intTemplateNo][intMatcherIndex].CorrelationMode != ECorrelationMode.Normalized)
                                    m_arrMarkMatcher[intTemplateNo][intMatcherIndex].CorrelationMode = ECorrelationMode.Normalized;
                            }
                        }
                        else
                        {
                            if (m_arrMarkMatcher[intTemplateNo][intMatcherIndex].CorrelationMode != ECorrelationMode.Normalized)
                                m_arrMarkMatcher[intTemplateNo][intMatcherIndex].CorrelationMode = ECorrelationMode.Normalized;
                        }

                        // 2018 08 20 - JBTAN: temporary put +-5 degree, to see if better
                        // 2020 06 23 - CCENG: Change from +-5 deg to +-10 deg. Get better Score after change to 10 deg.
                        if (m_arrMarkMatcher[intTemplateNo][0].MinAngle != -m_intPatternAngleTolerance)
                            m_arrMarkMatcher[intTemplateNo][0].MinAngle = -m_intPatternAngleTolerance;
                        if (m_arrMarkMatcher[intTemplateNo][0].MaxAngle != m_intPatternAngleTolerance)
                            m_arrMarkMatcher[intTemplateNo][0].MaxAngle = m_intPatternAngleTolerance;
                        if (m_arrMarkMatcher[intTemplateNo][0].MinScale != 1)
                            m_arrMarkMatcher[intTemplateNo][0].MinScale = 1;
                        if (m_arrMarkMatcher[intTemplateNo][0].MaxScale != 1)
                            m_arrMarkMatcher[intTemplateNo][0].MaxScale = 1;

                        strTrack += "5,";


                        //m_objSampleSearchThresholdROI.ref_ROI.Save("D:\\TS\\m_objSampleSearchThresholdROI.bmp");
                        //m_arrMarkMatcher[intTemplateNo][intMatcherIndex].Save("D:\\TS\\matcher.mch");

                        // Start pattern match
                        m_arrMarkMatcher[intTemplateNo][intMatcherIndex].Match(m_objSampleSearchThresholdROI.ref_ROI);

                        strTrack += "6,";
                        if (m_arrMarkMatcher[intTemplateNo][intMatcherIndex].NumPositions > 0)     // if macthing result hit the min score, its position will be 1 or more
                        {
                            strTrack += "7,";
                            float fScore = Math.Max(0, m_arrMarkMatcher[intTemplateNo][intMatcherIndex].GetPosition(0).Score);

                            if (fScore > fOrientHighestScore)
                            {
                                fOrientHighestScore = fScore;

                                strTrack += "12,";

                                arrAngleResult[intTemplateNo] = intMatcherIndex;

                                m_intAngleResult = intMatcherIndex;

                            }

                            m_arrMatcherScoreRecord[intTemplateNo].Add(fScore);

                            // if (intMatcherIndex == 0)
                            {
                                if (m_intCheckMarkMethod == 1)  // For SRM1 mode
                                {
                                    ROI objROI = new ROI();
                                    objROI.AttachImage(m_objSampleSearchThresholdROI);

                                    int intStartX = (int)Math.Round(m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).CenterX - (float)m_arrMarkMatcher[intTemplateNo][0].PatternWidth / 2, 0, MidpointRounding.AwayFromZero);
                                    int intStartY = (int)Math.Round(m_arrMarkMatcher[intTemplateNo][0].GetPosition(0).CenterY - (float)m_arrMarkMatcher[intTemplateNo][0].PatternHeight / 2, 0, MidpointRounding.AwayFromZero);
                                    int intWitdh = m_arrMarkMatcher[intTemplateNo][0].PatternWidth;
                                    int intHeight = m_arrMarkMatcher[intTemplateNo][0].PatternHeight;

                                    objROI.LoadROISetting(intStartX, intStartY, intWitdh, intHeight);

                                    bool blnWantDebug = false;
                                    if (blnWantDebug)
                                    {
                                        objROI.SaveImage("D:\\TS\\objROI.bmp");
                                    }

                                    TemplateImageMatching_SRM1(objROI, intTemplateNo, intMatcherIndex);

                                    objROI.Dispose();
                                    fScore = Math.Min(fScore, m_fImageMatchScore);
                                }

                                strTrack += "8,";
                                if (intMatcherIndex == 0)
                                {
                                    strTrack += "8,";
                                    if (fScore > fHighestScore)
                                    {
                                        strTrack += "9,";
                                        fHighestScore = fScore;

                                        m_fMarkMatchScore[intTemplateNo] = fScore;

                                        strTrack += "10,";
                                        m_pResultMarkCenterPoint = new PointF(m_objSampleSearchThresholdROI.ref_ROITotalX + m_arrMarkMatcher[intTemplateNo][intMatcherIndex].GetPosition(0).CenterX,
                                                                    m_objSampleSearchThresholdROI.ref_ROITotalY + m_arrMarkMatcher[intTemplateNo][intMatcherIndex].GetPosition(0).CenterY);
                                        m_SResultMarkSize = new SizeF(m_arrMarkMatcher[intTemplateNo][intMatcherIndex].PatternWidth, m_arrMarkMatcher[intTemplateNo][intMatcherIndex].PatternHeight);
                                        m_fResultMarkAngle = m_arrMarkMatcher[intTemplateNo][intMatcherIndex].GetPosition(0).Angle;
                                        m_intMarkTemplateIndex = m_intMarkTemplateNumSelected = intTemplateNo;

                                        strTrack += "14,";
                                    }
                                }
                            }

                        }
                        else
                        {
                            m_arrMatcherScoreRecord[intTemplateNo].Add(0);
                        }

                        strTrack += "15,";
                        if (m_intDirections == 4)
                            intMatcherIndex++;     // For square unit 0, 90, 180, -90 deg
                        else
                        {
                            m_arrMatcherScoreRecord[intTemplateNo].Add(0);
                            intMatcherIndex += 2; //For rectangle unit 0nly 0 deg and 180 deg
                        }

                        if (!bWantOrientation)
                        {
                            break;
                        }

                        strTrack += "16,";
                    } while (intMatcherIndex < 4);


                    //2020-10-13 ZJYEOH : Repeat test when mark score fail if customize mode and min max value not zero
                    if (m_fMarkMatchScore[m_intMarkTemplateNumSelected] < m_fMarkMinScore)
                    {
                        if (m_intCheckMarkMethod == 3)
                        {
                            if (m_arrTemplateWantAutoThresholdRelative[intTemplateNo])
                                blnThresholdDone = true;

                            if (blnErodeDilateFirst)
                            {
                                if (intThresholdSeq == 0)
                                {
                                    if (!blnThresholdDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && (m_arrTemplateThresholdMinValue[intTemplateNo] != 0 || m_arrTemplateThresholdMaxValue[intTemplateNo] != 0)))
                                    {
                                        if (!blnThresholdDone && (m_arrTemplateThresholdMaxValue[intTemplateNo] > m_arrTemplateMarkThreshold[intTemplateNo]) && !blnThresholdMaxDone && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) >= m_arrTemplateThresholdMaxValue[intTemplateNo]))
                                        {
                                            blnThresholdMaxDone = true;
                                            m_intTemplateThresholdTol = 0;
                                        }
                                        if (!blnThresholdDone && (m_arrTemplateThresholdMinValue[intTemplateNo] < m_arrTemplateMarkThreshold[intTemplateNo]) && !blnThresholdMinDone && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) <= m_arrTemplateThresholdMinValue[intTemplateNo]))
                                        {
                                            blnThresholdMinDone = true;
                                            m_intTemplateThresholdTol = 0;
                                        }

                                        if (!blnThresholdMaxDone && !blnThresholdNegTurn && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) < m_arrTemplateThresholdMaxValue[intTemplateNo]))
                                        {
                                            if (!blnThresholdMinDone)
                                                blnThresholdNegTurn = true;
                                            intThreshold_Pos += Math.Max(1, (int)((m_arrTemplateThresholdMaxValue[intTemplateNo] - m_arrTemplateMarkThreshold[intTemplateNo]) / 10));
                                            m_intTemplateThresholdTol = intThreshold_Pos;
                                        }
                                        else if (!blnThresholdMinDone && blnThresholdNegTurn && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) > m_arrTemplateThresholdMinValue[intTemplateNo]))
                                        {
                                            if (!blnThresholdMaxDone)
                                                blnThresholdNegTurn = false;
                                            intThreshold_Neg -= Math.Max(1, (int)((m_arrTemplateMarkThreshold[intTemplateNo] - m_arrTemplateThresholdMinValue[intTemplateNo]) / 10));
                                            m_intTemplateThresholdTol = intThreshold_Neg;
                                        }
                                        else
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdNegTurn = false;
                                            m_intTemplateThresholdTol = 0;
                                            intThreshold_Pos = 0;
                                            intThreshold_Neg = 0;
                                            blnThresholdDone = true;
                                        }
                                        if (!blnThresholdDone)
                                        {
                                            goto Repeat;
                                        }
                                    }
                                }

                                if (!blnErodeDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Erode") && (m_arrTemplateErodeMinValue[intTemplateNo] != 0 || m_arrTemplateErodeMaxValue[intTemplateNo] != 0)))
                                {
                                    if (!blnErodeDone && (m_arrTemplateErodeMaxValue[intTemplateNo] > m_arrTemplateErodeValue[intTemplateNo]) && ((m_intTemplateErodeTol + m_arrTemplateErodeValue[intTemplateNo]) == m_arrTemplateErodeMaxValue[intTemplateNo]))
                                    {
                                        blnErodeMaxDone = true;
                                        m_intTemplateErodeTol = 0;
                                    }

                                    if (!blnErodeMaxDone && ((m_intTemplateErodeTol + m_arrTemplateErodeValue[intTemplateNo]) < m_arrTemplateErodeMaxValue[intTemplateNo]))
                                        m_intTemplateErodeTol++;
                                    else if ((m_intTemplateErodeTol + m_arrTemplateErodeValue[intTemplateNo]) > m_arrTemplateErodeMinValue[intTemplateNo])
                                        m_intTemplateErodeTol--;
                                    else
                                    {
                                        m_intTemplateErodeTol = 0;
                                        blnErodeDone = true;
                                    }
                                    if (!blnErodeDone)
                                    {
                                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && intThresholdSeq == 0)
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdDone = false;
                                        }
                                        goto Repeat;
                                    }
                                }

                                if (!blnDilateDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Dilate") && (m_arrTemplateDilateMinValue[intTemplateNo] != 0 || m_arrTemplateDilateMaxValue[intTemplateNo] != 0)))
                                {
                                    if (!blnDilateDone && (m_arrTemplateDilateMaxValue[intTemplateNo] > m_arrTemplateDilateValue[intTemplateNo]) && ((m_intTemplateDilateTol + m_arrTemplateDilateValue[intTemplateNo]) == m_arrTemplateDilateMaxValue[intTemplateNo]))
                                    {
                                        blnDilateMaxDone = true;
                                        m_intTemplateDilateTol = 0;
                                    }

                                    if (!blnDilateMaxDone && ((m_intTemplateDilateTol + m_arrTemplateDilateValue[intTemplateNo]) < m_arrTemplateDilateMaxValue[intTemplateNo]))
                                        m_intTemplateDilateTol++;
                                    else if ((m_intTemplateDilateTol + m_arrTemplateDilateValue[intTemplateNo]) > m_arrTemplateDilateMinValue[intTemplateNo])
                                        m_intTemplateDilateTol--;
                                    else
                                    {
                                        m_intTemplateDilateTol = 0;
                                        blnDilateDone = true;
                                    }
                                    if (!blnDilateDone)
                                    {
                                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && intThresholdSeq == 0)
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdDone = false;
                                        }
                                        goto Repeat;
                                    }
                                }

                                if (intThresholdSeq == 1)
                                {
                                    if (!blnThresholdDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && (m_arrTemplateThresholdMinValue[intTemplateNo] != 0 || m_arrTemplateThresholdMaxValue[intTemplateNo] != 0)))
                                    {
                                        if (!blnThresholdDone && (m_arrTemplateThresholdMaxValue[intTemplateNo] > m_arrTemplateMarkThreshold[intTemplateNo]) && !blnThresholdMaxDone && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) >= m_arrTemplateThresholdMaxValue[intTemplateNo]))
                                        {
                                            blnThresholdMaxDone = true;
                                            m_intTemplateThresholdTol = 0;
                                        }
                                        if (!blnThresholdDone && (m_arrTemplateThresholdMinValue[intTemplateNo] < m_arrTemplateMarkThreshold[intTemplateNo]) && !blnThresholdMinDone && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) <= m_arrTemplateThresholdMinValue[intTemplateNo]))
                                        {
                                            blnThresholdMinDone = true;
                                            m_intTemplateThresholdTol = 0;
                                        }

                                        if (!blnThresholdMaxDone && !blnThresholdNegTurn && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) < m_arrTemplateThresholdMaxValue[intTemplateNo]))
                                        {
                                            if (!blnThresholdMinDone)
                                                blnThresholdNegTurn = true;
                                            intThreshold_Pos += Math.Max(1, (int)((m_arrTemplateThresholdMaxValue[intTemplateNo] - m_arrTemplateMarkThreshold[intTemplateNo]) / 10));
                                            m_intTemplateThresholdTol = intThreshold_Pos;
                                        }
                                        else if (!blnThresholdMinDone && blnThresholdNegTurn && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) > m_arrTemplateThresholdMinValue[intTemplateNo]))
                                        {
                                            if (!blnThresholdMaxDone)
                                                blnThresholdNegTurn = false;
                                            intThreshold_Neg -= Math.Max(1, (int)((m_arrTemplateMarkThreshold[intTemplateNo] - m_arrTemplateThresholdMinValue[intTemplateNo]) / 10));
                                            m_intTemplateThresholdTol = intThreshold_Neg;
                                        }
                                        else
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdNegTurn = false;
                                            m_intTemplateThresholdTol = 0;
                                            intThreshold_Pos = 0;
                                            intThreshold_Neg = 0;
                                            blnThresholdDone = true;
                                        }
                                        if (!blnThresholdDone)
                                        {
                                            if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Erode"))
                                            {
                                                blnErodeMaxDone = false;
                                                blnErodeDone = false;
                                            }
                                            else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Dilate"))
                                            {
                                                blnDilateMaxDone = false;
                                                blnDilateDone = false;
                                            }
                                            goto Repeat;
                                        }
                                    }
                                }

                                if (!blnOpenDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Open") && (m_arrTemplateOpenMinValue[intTemplateNo] != 0 || m_arrTemplateOpenMaxValue[intTemplateNo] != 0)))
                                {
                                    if (!blnOpenDone && (m_arrTemplateOpenMaxValue[intTemplateNo] > m_arrTemplateOpenValue[intTemplateNo]) && ((m_intTemplateOpenTol + m_arrTemplateOpenValue[intTemplateNo]) == m_arrTemplateOpenMaxValue[intTemplateNo]))
                                    {
                                        blnOpenMaxDone = true;
                                        m_intTemplateOpenTol = 0;
                                    }

                                    if (!blnOpenMaxDone && ((m_intTemplateOpenTol + m_arrTemplateOpenValue[intTemplateNo]) < m_arrTemplateOpenMaxValue[intTemplateNo]))
                                        m_intTemplateOpenTol++;
                                    else if ((m_intTemplateOpenTol + m_arrTemplateOpenValue[intTemplateNo]) > m_arrTemplateOpenMinValue[intTemplateNo])
                                        m_intTemplateOpenTol--;
                                    else
                                    {
                                        m_intTemplateOpenTol = 0;
                                        blnOpenDone = true;
                                    }

                                    if (!blnOpenDone)
                                    {
                                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && intThresholdSeq == 0)
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdDone = false;
                                        }
                                        else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Erode"))
                                        {
                                            blnErodeMaxDone = false;
                                            blnErodeDone = false;
                                        }
                                        else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Dilate"))
                                        {
                                            blnDilateMaxDone = false;
                                            blnDilateDone = false;
                                        }
                                        else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && intThresholdSeq == 1)
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdDone = false;
                                        }
                                        goto Repeat;
                                    }
                                }

                                if (!blnCloseDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Close") && (m_arrTemplateCloseMinValue[intTemplateNo] != 0 || m_arrTemplateCloseMaxValue[intTemplateNo] != 0)))
                                {
                                    if (!blnCloseDone && (m_arrTemplateCloseMaxValue[intTemplateNo] > m_arrTemplateCloseValue[intTemplateNo]) && ((m_intTemplateCloseTol + m_arrTemplateCloseValue[intTemplateNo]) == m_arrTemplateCloseMaxValue[intTemplateNo]))
                                    {
                                        blnCloseMaxDone = true;
                                        m_intTemplateCloseTol = 0;
                                    }

                                    if (!blnCloseMaxDone && ((m_intTemplateCloseTol + m_arrTemplateCloseValue[intTemplateNo]) < m_arrTemplateCloseMaxValue[intTemplateNo]))
                                        m_intTemplateCloseTol++;
                                    else if ((m_intTemplateCloseTol + m_arrTemplateCloseValue[intTemplateNo]) > m_arrTemplateCloseMinValue[intTemplateNo])
                                        m_intTemplateCloseTol--;
                                    else
                                    {
                                        m_intTemplateCloseTol = 0;
                                        blnCloseDone = true;
                                    }
                                    if (!blnCloseDone)
                                    {
                                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && intThresholdSeq == 0)
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdDone = false;
                                        }
                                        else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Erode"))
                                        {
                                            blnErodeMaxDone = false;
                                            blnErodeDone = false;
                                        }
                                        else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Dilate"))
                                        {
                                            blnDilateMaxDone = false;
                                            blnDilateDone = false;
                                        }
                                        else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && intThresholdSeq == 1)
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdDone = false;
                                        }
                                        goto Repeat;
                                    }
                                }

                                if (intThresholdSeq == 2)
                                {
                                    if (!blnThresholdDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && (m_arrTemplateThresholdMinValue[intTemplateNo] != 0 || m_arrTemplateThresholdMaxValue[intTemplateNo] != 0)))
                                    {
                                        if (!blnThresholdDone && (m_arrTemplateThresholdMaxValue[intTemplateNo] > m_arrTemplateMarkThreshold[intTemplateNo]) && !blnThresholdMaxDone && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) >= m_arrTemplateThresholdMaxValue[intTemplateNo]))
                                        {
                                            blnThresholdMaxDone = true;
                                            m_intTemplateThresholdTol = 0;
                                        }
                                        if (!blnThresholdDone && (m_arrTemplateThresholdMinValue[intTemplateNo] < m_arrTemplateMarkThreshold[intTemplateNo]) && !blnThresholdMinDone && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) <= m_arrTemplateThresholdMinValue[intTemplateNo]))
                                        {
                                            blnThresholdMinDone = true;
                                            m_intTemplateThresholdTol = 0;
                                        }

                                        if (!blnThresholdMaxDone && !blnThresholdNegTurn && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) < m_arrTemplateThresholdMaxValue[intTemplateNo]))
                                        {
                                            if (!blnThresholdMinDone)
                                                blnThresholdNegTurn = true;
                                            intThreshold_Pos += Math.Max(1, (int)((m_arrTemplateThresholdMaxValue[intTemplateNo] - m_arrTemplateMarkThreshold[intTemplateNo]) / 10));
                                            m_intTemplateThresholdTol = intThreshold_Pos;
                                        }
                                        else if (!blnThresholdMinDone && blnThresholdNegTurn && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) > m_arrTemplateThresholdMinValue[intTemplateNo]))
                                        {
                                            if (!blnThresholdMaxDone)
                                                blnThresholdNegTurn = false;
                                            intThreshold_Neg -= Math.Max(1, (int)((m_arrTemplateMarkThreshold[intTemplateNo] - m_arrTemplateThresholdMinValue[intTemplateNo]) / 10));
                                            m_intTemplateThresholdTol = intThreshold_Neg;
                                        }
                                        else
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdNegTurn = false;
                                            m_intTemplateThresholdTol = 0;
                                            intThreshold_Pos = 0;
                                            intThreshold_Neg = 0;
                                            blnThresholdDone = true;
                                        }
                                        if (!blnThresholdDone)
                                        {
                                            if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Erode"))
                                            {
                                                blnErodeMaxDone = false;
                                                blnErodeDone = false;
                                            }
                                            else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Dilate"))
                                            {
                                                blnDilateMaxDone = false;
                                                blnDilateDone = false;
                                            }
                                            else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Open"))
                                            {
                                                blnOpenMaxDone = false;
                                                blnOpenDone = false;
                                            }
                                            else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Close"))
                                            {
                                                blnCloseMaxDone = false;
                                                blnCloseDone = false;
                                            }
                                            goto Repeat;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (intThresholdSeq == 0)
                                {
                                    if (!blnThresholdDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && (m_arrTemplateThresholdMinValue[intTemplateNo] != 0 || m_arrTemplateThresholdMaxValue[intTemplateNo] != 0)))
                                    {
                                        if (!blnThresholdDone && (m_arrTemplateThresholdMaxValue[intTemplateNo] > m_arrTemplateMarkThreshold[intTemplateNo]) && !blnThresholdMaxDone && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) >= m_arrTemplateThresholdMaxValue[intTemplateNo]))
                                        {
                                            blnThresholdMaxDone = true;
                                            m_intTemplateThresholdTol = 0;
                                        }
                                        if (!blnThresholdDone && (m_arrTemplateThresholdMinValue[intTemplateNo] < m_arrTemplateMarkThreshold[intTemplateNo]) && !blnThresholdMinDone && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) <= m_arrTemplateThresholdMinValue[intTemplateNo]))
                                        {
                                            blnThresholdMinDone = true;
                                            m_intTemplateThresholdTol = 0;
                                        }

                                        if (!blnThresholdMaxDone && !blnThresholdNegTurn && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) < m_arrTemplateThresholdMaxValue[intTemplateNo]))
                                        {
                                            if (!blnThresholdMinDone)
                                                blnThresholdNegTurn = true;
                                            intThreshold_Pos += Math.Max(1, (int)((m_arrTemplateThresholdMaxValue[intTemplateNo] - m_arrTemplateMarkThreshold[intTemplateNo]) / 10));
                                            m_intTemplateThresholdTol = intThreshold_Pos;
                                        }
                                        else if (!blnThresholdMinDone && blnThresholdNegTurn && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) > m_arrTemplateThresholdMinValue[intTemplateNo]))
                                        {
                                            if (!blnThresholdMaxDone)
                                                blnThresholdNegTurn = false;
                                            intThreshold_Neg -= Math.Max(1, (int)((m_arrTemplateMarkThreshold[intTemplateNo] - m_arrTemplateThresholdMinValue[intTemplateNo]) / 10));
                                            m_intTemplateThresholdTol = intThreshold_Neg;
                                        }
                                        else
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdNegTurn = false;
                                            m_intTemplateThresholdTol = 0;
                                            intThreshold_Pos = 0;
                                            intThreshold_Neg = 0;
                                            blnThresholdDone = true;
                                        }
                                        if (!blnThresholdDone)
                                        {
                                            goto Repeat;
                                        }
                                    }
                                }

                                if (!blnOpenDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Open") && (m_arrTemplateOpenMinValue[intTemplateNo] != 0 || m_arrTemplateOpenMaxValue[intTemplateNo] != 0)))
                                {
                                    if (!blnOpenDone && (m_arrTemplateOpenMaxValue[intTemplateNo] > m_arrTemplateOpenValue[intTemplateNo]) && (m_arrTemplateOpenMaxValue[intTemplateNo] != 0) && ((m_intTemplateOpenTol + m_arrTemplateOpenValue[intTemplateNo]) == m_arrTemplateOpenMaxValue[intTemplateNo]))
                                    {
                                        blnOpenMaxDone = true;
                                        m_intTemplateOpenTol = 0;
                                    }

                                    if (!blnOpenMaxDone && ((m_intTemplateOpenTol + m_arrTemplateOpenValue[intTemplateNo]) < m_arrTemplateOpenMaxValue[intTemplateNo]))
                                        m_intTemplateOpenTol++;
                                    else if ((m_intTemplateOpenTol + m_arrTemplateOpenValue[intTemplateNo]) > m_arrTemplateOpenMinValue[intTemplateNo])
                                        m_intTemplateOpenTol--;
                                    else
                                    {
                                        m_intTemplateOpenTol = 0;
                                        blnOpenDone = true;
                                    }
                                    if (!blnOpenDone)
                                    {
                                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && intThresholdSeq == 0)
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdDone = false;
                                        }
                                        goto Repeat;
                                    }
                                }

                                if (!blnCloseDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Close") && (m_arrTemplateCloseMinValue[intTemplateNo] != 0 || m_arrTemplateCloseMaxValue[intTemplateNo] != 0)))
                                {
                                    if (!blnCloseDone && (m_arrTemplateCloseMaxValue[intTemplateNo] > m_arrTemplateCloseValue[intTemplateNo]) && (m_arrTemplateCloseMaxValue[intTemplateNo] != 0) && ((m_intTemplateCloseTol + m_arrTemplateCloseValue[intTemplateNo]) == m_arrTemplateCloseMaxValue[intTemplateNo]))
                                    {
                                        blnCloseMaxDone = true;
                                        m_intTemplateCloseTol = 0;
                                    }

                                    if (!blnCloseMaxDone && ((m_intTemplateCloseTol + m_arrTemplateCloseValue[intTemplateNo]) < m_arrTemplateCloseMaxValue[intTemplateNo]))
                                        m_intTemplateCloseTol++;
                                    else if ((m_intTemplateCloseTol + m_arrTemplateCloseValue[intTemplateNo]) > m_arrTemplateCloseMinValue[intTemplateNo])
                                        m_intTemplateCloseTol--;
                                    else
                                    {
                                        m_intTemplateCloseTol = 0;
                                        blnCloseDone = true;
                                    }
                                    if (!blnCloseDone)
                                    {
                                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && intThresholdSeq == 0)
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdDone = false;
                                        }
                                        goto Repeat;
                                    }
                                }

                                if (intThresholdSeq == 1)
                                {
                                    if (!blnThresholdDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && (m_arrTemplateThresholdMinValue[intTemplateNo] != 0 || m_arrTemplateThresholdMaxValue[intTemplateNo] != 0)))
                                    {
                                        if (!blnThresholdDone && (m_arrTemplateThresholdMaxValue[intTemplateNo] > m_arrTemplateMarkThreshold[intTemplateNo]) && !blnThresholdMaxDone && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) >= m_arrTemplateThresholdMaxValue[intTemplateNo]))
                                        {
                                            blnThresholdMaxDone = true;
                                            m_intTemplateThresholdTol = 0;
                                        }
                                        if (!blnThresholdDone && (m_arrTemplateThresholdMinValue[intTemplateNo] < m_arrTemplateMarkThreshold[intTemplateNo]) && !blnThresholdMinDone && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) <= m_arrTemplateThresholdMinValue[intTemplateNo]))
                                        {
                                            blnThresholdMinDone = true;
                                            m_intTemplateThresholdTol = 0;
                                        }

                                        if (!blnThresholdMaxDone && !blnThresholdNegTurn && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) < m_arrTemplateThresholdMaxValue[intTemplateNo]))
                                        {
                                            if (!blnThresholdMinDone)
                                                blnThresholdNegTurn = true;
                                            intThreshold_Pos += Math.Max(1, (int)((m_arrTemplateThresholdMaxValue[intTemplateNo] - m_arrTemplateMarkThreshold[intTemplateNo]) / 10));
                                            m_intTemplateThresholdTol = intThreshold_Pos;
                                        }
                                        else if (!blnThresholdMinDone && blnThresholdNegTurn && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) > m_arrTemplateThresholdMinValue[intTemplateNo]))
                                        {
                                            if (!blnThresholdMaxDone)
                                                blnThresholdNegTurn = false;
                                            intThreshold_Neg -= Math.Max(1, (int)((m_arrTemplateMarkThreshold[intTemplateNo] - m_arrTemplateThresholdMinValue[intTemplateNo]) / 10));
                                            m_intTemplateThresholdTol = intThreshold_Neg;
                                        }
                                        else
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdNegTurn = false;
                                            m_intTemplateThresholdTol = 0;
                                            intThreshold_Pos = 0;
                                            intThreshold_Neg = 0;
                                            blnThresholdDone = true;
                                        }
                                        if (!blnThresholdDone)
                                        {
                                            if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Open"))
                                            {
                                                blnOpenMaxDone = false;
                                                blnOpenDone = false;
                                            }
                                            else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Close"))
                                            {
                                                blnCloseMaxDone = false;
                                                blnCloseDone = false;
                                            }
                                            goto Repeat;
                                        }
                                    }
                                }

                                if (!blnErodeDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Erode") && (m_arrTemplateErodeMinValue[intTemplateNo] != 0 || m_arrTemplateErodeMaxValue[intTemplateNo] != 0)))
                                {
                                    if (!blnErodeDone && (m_arrTemplateErodeMaxValue[intTemplateNo] > m_arrTemplateErodeValue[intTemplateNo]) && ((m_intTemplateErodeTol + m_arrTemplateErodeValue[intTemplateNo]) == m_arrTemplateErodeMaxValue[intTemplateNo]))
                                    {
                                        blnErodeMaxDone = true;
                                        m_intTemplateErodeTol = 0;
                                    }

                                    if (!blnErodeMaxDone && ((m_intTemplateErodeTol + m_arrTemplateErodeValue[intTemplateNo]) < m_arrTemplateErodeMaxValue[intTemplateNo]))
                                        m_intTemplateErodeTol++;
                                    else if ((m_intTemplateErodeTol + m_arrTemplateErodeValue[intTemplateNo]) > m_arrTemplateErodeMinValue[intTemplateNo])
                                        m_intTemplateErodeTol--;
                                    else
                                    {
                                        m_intTemplateErodeTol = 0;
                                        blnErodeDone = true;
                                    }
                                    if (!blnErodeDone)
                                    {
                                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && intThresholdSeq == 0)
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdDone = false;
                                        }
                                        else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Open"))
                                        {
                                            blnOpenMaxDone = false;
                                            blnOpenDone = false;
                                        }
                                        else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Close"))
                                        {
                                            blnCloseMaxDone = false;
                                            blnCloseDone = false;
                                        }
                                        else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && intThresholdSeq == 1)
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdDone = false;
                                        }

                                        goto Repeat;
                                    }
                                }

                                if (!blnDilateDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Dilate") && (m_arrTemplateDilateMinValue[intTemplateNo] != 0 || m_arrTemplateDilateMaxValue[intTemplateNo] != 0)))
                                {
                                    if (!blnDilateDone && (m_arrTemplateDilateMaxValue[intTemplateNo] > m_arrTemplateDilateValue[intTemplateNo]) && ((m_intTemplateDilateTol + m_arrTemplateDilateValue[intTemplateNo]) == m_arrTemplateDilateMaxValue[intTemplateNo]))
                                    {
                                        blnDilateMaxDone = true;
                                        m_intTemplateDilateTol = 0;
                                    }

                                    if (!blnDilateMaxDone && ((m_intTemplateDilateTol + m_arrTemplateDilateValue[intTemplateNo]) < m_arrTemplateDilateMaxValue[intTemplateNo]))
                                        m_intTemplateDilateTol++;
                                    else if ((m_intTemplateDilateTol + m_arrTemplateDilateValue[intTemplateNo]) > m_arrTemplateDilateMinValue[intTemplateNo])
                                        m_intTemplateDilateTol--;
                                    else
                                    {
                                        m_intTemplateDilateTol = 0;
                                        blnDilateDone = true;
                                    }
                                    if (!blnDilateDone)
                                    {
                                        if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && intThresholdSeq == 0)
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdDone = false;
                                        }
                                        else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Open"))
                                        {
                                            blnOpenMaxDone = false;
                                            blnOpenDone = false;
                                        }
                                        else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Close"))
                                        {
                                            blnCloseMaxDone = false;
                                            blnCloseDone = false;
                                        }
                                        else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && intThresholdSeq == 1)
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdDone = false;
                                        }
                                        goto Repeat;
                                    }
                                }

                                if (intThresholdSeq == 2)
                                {
                                    if (!blnThresholdDone && (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Threshold") && (m_arrTemplateThresholdMinValue[intTemplateNo] != 0 || m_arrTemplateThresholdMaxValue[intTemplateNo] != 0)))
                                    {
                                        if (!blnThresholdDone && (m_arrTemplateThresholdMaxValue[intTemplateNo] > m_arrTemplateMarkThreshold[intTemplateNo]) && !blnThresholdMaxDone && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) >= m_arrTemplateThresholdMaxValue[intTemplateNo]))
                                        {
                                            blnThresholdMaxDone = true;
                                            m_intTemplateThresholdTol = 0;
                                        }
                                        if (!blnThresholdDone && (m_arrTemplateThresholdMinValue[intTemplateNo] < m_arrTemplateMarkThreshold[intTemplateNo]) && !blnThresholdMinDone && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) <= m_arrTemplateThresholdMinValue[intTemplateNo]))
                                        {
                                            blnThresholdMinDone = true;
                                            m_intTemplateThresholdTol = 0;
                                        }

                                        if (!blnThresholdMaxDone && !blnThresholdNegTurn && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) < m_arrTemplateThresholdMaxValue[intTemplateNo]))
                                        {
                                            if (!blnThresholdMinDone)
                                                blnThresholdNegTurn = true;
                                            intThreshold_Pos += Math.Max(1, (int)((m_arrTemplateThresholdMaxValue[intTemplateNo] - m_arrTemplateMarkThreshold[intTemplateNo]) / 10));
                                            m_intTemplateThresholdTol = intThreshold_Pos;
                                        }
                                        else if (!blnThresholdMinDone && blnThresholdNegTurn && ((m_intTemplateThresholdTol + m_arrTemplateMarkThreshold[intTemplateNo]) > m_arrTemplateThresholdMinValue[intTemplateNo]))
                                        {
                                            if (!blnThresholdMaxDone)
                                                blnThresholdNegTurn = false;
                                            intThreshold_Neg -= Math.Max(1, (int)((m_arrTemplateMarkThreshold[intTemplateNo] - m_arrTemplateThresholdMinValue[intTemplateNo]) / 10));
                                            m_intTemplateThresholdTol = intThreshold_Neg;
                                        }
                                        else
                                        {
                                            blnThresholdMaxDone = false;
                                            blnThresholdMinDone = false;
                                            blnThresholdNegTurn = false;
                                            m_intTemplateThresholdTol = 0;
                                            intThreshold_Pos = 0;
                                            intThreshold_Neg = 0;
                                            blnThresholdDone = true;
                                        }
                                        if (!blnThresholdDone)
                                        {
                                            if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Open"))
                                            {
                                                blnOpenMaxDone = false;
                                                blnOpenDone = false;
                                            }
                                            else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Close"))
                                            {
                                                blnCloseMaxDone = false;
                                                blnCloseDone = false;
                                            }
                                            else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Erode"))
                                            {
                                                blnErodeMaxDone = false;
                                                blnErodeDone = false;
                                            }
                                            else if (m_arrTemplateImageProcessSeq[intTemplateNo].Contains("Dilate"))
                                            {
                                                blnDilateMaxDone = false;
                                                blnDilateDone = false;
                                            }

                                            goto Repeat;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    m_intTemplateErodeTol = 0;
                    m_intTemplateDilateTol = 0;
                    m_intTemplateOpenTol = 0;
                    m_intTemplateCloseTol = 0;
                    m_intTemplateThresholdTol = 0;

                    timeout.Stop(); // ------------------- checking loop timeout ---------------------------------------------------
                }
                strTrack += "17,";
                m_strTrack += ", Mark Center X=" + m_pResultMarkCenterPoint.X.ToString();
                m_strTrack += ", Mark Center Y=" + m_pResultMarkCenterPoint.Y.ToString();

                //m_intMarkTemplateIndex = 0;
                //float fTemplateHighestScore = 0;
                //for (int intTemplateNo = 0; intTemplateNo < m_arrMatcherScoreRecord.Count; intTemplateNo++)
                //{
                //    if ((intMarkTemplateMask & (0x01 << intTemplateNo)) <= 0)
                //        continue;

                //    for (int intMatcherScoreIndex = 0; intMatcherScoreIndex < m_arrMatcherScoreRecord[intTemplateNo].Count; intMatcherScoreIndex++)
                //    {
                //        if (fTemplateHighestScore < m_arrMatcherScoreRecord[intTemplateNo][intMatcherScoreIndex])
                //        {
                //            fTemplateHighestScore = m_arrMatcherScoreRecord[intTemplateNo][intMatcherScoreIndex];
                //            m_intMarkTemplateIndex = intTemplateNo;
                //            m_intAngleResult = intMatcherScoreIndex;
                //        }
                //    }
                //}

                if (m_intAngleResult == 0)
                {
                    if (m_fMarkMatchScore[m_intMarkTemplateNumSelected] < m_fMarkMinScore)
                    {
                        m_strErrorMessage = "Fail Unit Mark - Not fulfill Min Setting : Set = " + (m_fMarkMinScore * 100).ToString() +
                                            " Score = " + (m_fMarkMatchScore[m_intMarkTemplateNumSelected] * 100).ToString("f5");
                        m_intSealFailMask |= 0x80;
                    }
                    else
                        blnMarkScorePass = true;
                }
                else
                {
                    if (m_fMarkMatchScore[m_intMarkTemplateNumSelected] < m_fMarkMinScore)
                    {
                        m_strErrorMessage = "Fail Unit Mark - Not fulfill Min Setting : Set = " + (m_fMarkMinScore * 100).ToString() +
                                            " Score = " + (m_fMarkMatchScore[m_intMarkTemplateNumSelected] * 100).ToString("f5");
                        m_intSealFailMask |= 0x80;
                    }
                    else
                    {
                        blnMarkScorePass = true;

                        if (m_intAngleResult == 1)
                        {
                            strTrack += "18,";
                            m_strErrorMessage = "Fail Orient Angle : 90 deg";
                            m_intSealFailMask |= 0x1000;
                            m_CheckUnitOrient = true;
                        }
                        if (m_intAngleResult == 2)
                        {
                            strTrack += "19,";
                            m_strErrorMessage = "Fail Orient Angle : 180 deg";
                            m_intSealFailMask |= 0x1000;
                            m_CheckUnitOrient = true;
                        }
                        if (m_intAngleResult == 3)
                        {
                            strTrack += "20,";
                            m_strErrorMessage = "Fail Orient Angle : -90 deg";
                            m_intSealFailMask |= 0x1000;
                            m_CheckUnitOrient = true;
                        }
                        if (m_intAngleResult == 4)
                        {
                            strTrack += "21,";
                            m_strErrorMessage = "Fail Mark - Not fulfill Min Setting : Set = " + (m_fMarkMinScore * 100).ToString() +
                                " Score = " + (m_fMarkMatchScore[m_intMarkTemplateIndex] * 100).ToString("f5");
                            m_intSealFailMask |= 0x80;
                            m_CheckUnitOrient = true;
                        }

                        string strMatcherScoreRecord = "";
                        for (int i = 0; i < m_arrMatcherScoreRecord[m_intMarkTemplateIndex].Count; i++)
                        {
                            if (m_intDirections != 4)
                            {
                                if (i == 1 || i == 3)
                                    continue;
                            }

                            strMatcherScoreRecord += "Score " + (i * 90).ToString() + "deg=" + (m_arrMatcherScoreRecord[m_intMarkTemplateIndex][i] * 100).ToString("F2") + "%";

                            if (i < m_arrMatcherScoreRecord.Count - 1)
                            {
                                strMatcherScoreRecord += ", ";
                            }
                        }

                        m_strErrorMessage += "*" + strMatcherScoreRecord;
                    }
                }
                strTrack += "22,";

            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("DoSealOrientationInspection strack=" + strTrack + " , exp = " + ex.ToString());
            }

            return blnMarkScorePass;
        }

        /// <summary>
        /// Reset previous inspection result
        /// </summary>
        public void ResetMarkInspectionData()
        {

            m_fMarkMatchScore[0] = 0;// -1;
            m_fMarkMatchScore[1] = 0;// -1;
            m_fMarkMatchScore[2] = 0;// -1;
            m_fMarkMatchScore[3] = 0;// -1;


        }

        /// <summary>
        /// Reset previous inspection result
        /// </summary>
        //public void ResetPocketInspectionData()
        //{

        //    m_fPocketMatchScore = -1;
        //}

        public void ResetInspectionData()
        {
            m_strTrack2 = "";

            m_strErrorMessage = "";
            m_intSealFailMask = 0;
            m_fLineWidthAverage = new float[19];

            for (int i = 0; i < m_fLineWidthAverage.Length; i++)
            {
                m_fLineWidthAverage[i] = -1f;
            }

            for (int i = 0; i < m_fFailSealScore.Length; i++)
            {
                m_fFailSealScore[i] = -1f;
            }
            m_blnCircleGaugeOutOfImageRange = false;
            m_fCircleGaugeShiftX = 0;
            m_arrFailWidthStartPoint = new List<List<PointF>>();
            m_arrFailWidthEndPoint = new List<List<PointF>>();
            m_arrGaugePositions = new List<ArrayList>();
            m_arrFailBlobsIndex = new List<List<int>>();
            m_arrFailBlobsHoleIndex = new List<List<int>>();
            m_arrPassBlobsIndex = new List<List<int>>();
            m_arrFailSealBrokenStartPoint = new List<PointF>();
            m_arrFailSealBrokenEndPoint = new List<PointF>();

            for (int i = 0; i < m_arrFailOverHeat.Length; i++)
                m_arrFailOverHeat[i] = false;

            for (int i = 0; i < m_arrFailScratches.Length; i++)
                m_arrFailScratches[i] = false;

            for (int i = 0; i < m_arrOverHeatBlobs.Length; i++)
            {
                if (m_arrOverHeatBlobs[i] == null)
                    m_arrOverHeatBlobs[i] = new EBlobs();
                else
                    m_arrOverHeatBlobs[i].CleanAllBlobs();
            }

            for (int i = 0; i < m_arrScratchesBlobs.Length; i++)
            {
                if (m_arrScratchesBlobs[i] == null)
                    m_arrScratchesBlobs[i] = new EBlobs();
                else
                    m_arrScratchesBlobs[i].CleanAllBlobs();
            }

            //m_objOverHeatBlobs.ResetBlobs();
            //m_objDistanceBlobs.ResetBlobs();
            //for (int i = 0; i < m_arrBlackBlobs.Count; i++)
            //{
            //    m_arrBlackBlobs[i].ResetBlobs();
            //}
            m_pResultPositionCenterPoint = new PointF(0, 0);
            m_pResultPocketCenterPoint = new PointF(0, 0);
            m_pResultMarkCenterPoint = new PointF(0, 0);
            m_SResultMarkSize = new SizeF();
            m_fResultMarkAngle = 0;
            m_intAngleResult = 4;
            m_fMarkMatchScore[0] = -1f;
            m_fMarkMatchScore[1] = -1f;
            m_fMarkMatchScore[2] = -1f;
            m_fMarkMatchScore[3] = -1f;

            m_fSealBorderPositionY = 0;

            m_fPocketMatchScore[0] = -1f;
            m_fPocketMatchScore[1] = -1f;
            m_fPocketMatchScore[2] = -1f;
            m_fPocketMatchScore[3] = -1f;
            m_fShiftedX = 0;
            m_fShiftedY = 0;
            for (int i = 0; i < m_arrFailOverheatArea.Length; i++)
                m_arrFailOverheatArea[i] = -999f;
            for (int i = 0; i < m_arrFailScratchesArea.Length; i++)
                m_arrFailScratchesArea[i] = -999f;
            m_FailSealEdgeStraightnessArea = -999f;
            m_FailUnitPresentWhiteArea = -999;
            m_FailDistance = 0f;
            m_FailSprocketHoleDistance = 0f;
            m_FailSprocketHoleDiameter = 0f;
            m_FailSprocketHoleDefectArea = 0f;
            m_FailSprocketHoleBrokenArea = 0f;
            m_FailSprocketHoleRoundness = 0f;
            m_FailBubble = new float[] { -999f, -999f };
            m_FailBrokenSeal = new float[] { -999f, -999f };
            m_FailOverSeal = new float[2];
            m_FailInsufficient = new float[2];
            m_FailPosition = 0f;
            m_blnFailSeal1 = false;
            m_blnFailSeal2 = false;
        }

        public void Dispose()
        {
            m_arrBrushMatched = new List<List<SolidBrush>>();
            m_Brush.Dispose();

            for (int i = 0; i < m_arrBlackBlobs.Count; i++)
            {
                if (m_arrBlackBlobs[i] != null)
                    m_arrBlackBlobs[i].Dispose();
            }

            for (int i = 0; i < m_arrOverHeatBlobs.Length; i++)
            {
                if (m_arrOverHeatBlobs[i] != null)
                    m_arrOverHeatBlobs[i].Dispose();
            }

            for (int i = 0; i < m_arrScratchesBlobs.Length; i++)
            {
                if (m_arrScratchesBlobs[i] != null)
                    m_arrScratchesBlobs[i].Dispose();
            }

            if (m_objDistanceBlobs != null)
                m_objDistanceBlobs.Dispose();

            if (m_objSealEdgeStraightnessBlobs != null)
                m_objSealEdgeStraightnessBlobs.Dispose();

            if (m_objSprocketHoleDefectBlobs != null)
                m_objSprocketHoleDefectBlobs.Dispose();

            if (m_objSealLineImage != null)
                m_objSealLineImage.Dispose();


            if (m_imgSealLineImage != null)
                m_imgSealLineImage.Dispose();

            if (m_objThresholdSealROI != null)
                m_objThresholdSealROI.Dispose();

            for (int i = 0; i < m_arrPocketMatcher.Count; i++)
            {
                if (m_arrPocketMatcher[i] != null)
                    m_arrPocketMatcher[i].Dispose();
            }

            for (int i = 0; i < m_arrPositionMatcher.Count; i++)
            {
                if (m_arrPositionMatcher[i] != null)
                    m_arrPositionMatcher[i].Dispose();
            }

            for (int i = 0; i < m_arrMarkMatcher.Count; i++)
            {
                for (int j = 0; j < m_arrMarkMatcher[i].Count; j++)
                {
                    if (m_arrMarkMatcher[i][j] != null)
                        m_arrMarkMatcher[i][j].Dispose();
                }
            }


        }
        public int GetMatcherPatternWidth(int intTemplateIndex)
        {
            if (intTemplateIndex >= m_arrMarkMatcher.Count)
                return 0;

            return m_arrMarkMatcher[intTemplateIndex][0].PatternWidth;
        }

        public int GetMatcherPatternHeight(int intTemplateIndex)
        {
            if (intTemplateIndex >= m_arrMarkMatcher.Count)
                return 0;

            return m_arrMarkMatcher[intTemplateIndex][0].PatternHeight;
        }

        public int GetTemplateCount()
        {
            return m_arrMarkMatcher.Count;
        }

        public bool CheckSprocketPitch(ImageDrawing objImage, ROI objSearchSprocketPatternROI, ROI objSprocketPatternROI, float fCalibX)
        {
            bool blnDebug = false;
            if (blnDebug)
            {
                objSearchSprocketPatternROI.SaveImage("D:\\TS\\1.objSearchSprocketPatternROI.bmp");
                objSprocketPatternROI.SaveImage("D:\\TS\\2.objSprocketPatternROI.bmp");
            }

            ROI objROI = new ROI();
            objROI.AttachImage(objImage);
            m_fTapePocketPitchByImage = -1;
            if (objSprocketPatternROI.ref_ROICenterX > (objImage.ref_intImageWidth / 2))
            {
                // ------------ find sprocket blobs for current search ROI ------------------------------------
                if (m_objSprocketBlobs == null)
                    m_objSprocketBlobs = new EBlobs();
                else
                    m_objSprocketBlobs.CleanAllBlobs();

                float fEndX1 = 0;
                m_objSprocketBlobs.BuildObjects_Filter_GetElement(objSprocketPatternROI, false, m_intConnexity == 4, 0, ROI.GetAutoThresholdValue(objSprocketPatternROI, 3), 0, 999999, false, 0x0F);
                if (m_objSprocketBlobs.ref_intNumSelectedObject > 0)
                {
                    float fCenterX = m_objSprocketBlobs.ref_arrLimitCenterX[0];
                    float fCenterY = m_objSprocketBlobs.ref_arrLimitCenterY[0];
                    float fWidth = m_objSprocketBlobs.ref_arrWidth[0];
                    float fHeight = m_objSprocketBlobs.ref_arrHeight[0];
                    fEndX1 = objSprocketPatternROI.ref_ROITotalX + fCenterX + fWidth / 2;
                }

                // get left ROI sprocket hole position
                // 2020 04 15 - CCENG: use objSprocketPatternROI to build blob. objSearchSprocketPatternROI cannot use anymore bcos the X length is same as image now.
                //int intStartX = Math.Max(0, (int)(objSprocketPatternROI.ref_ROICenterX - 4 * fCalibX - objSearchSprocketPatternROI.ref_ROIWidth / 2));
                //int intStartY = objSprocketPatternROI.ref_ROIPositionY;
                //int intEndX = (int)(objSprocketPatternROI.ref_ROICenterX - 4 * fCalibX + objSearchSprocketPatternROI.ref_ROIWidth / 2);
                //int intEndY = objSprocketPatternROI.ref_ROIPositionY + objSprocketPatternROI.ref_ROIHeight;
                int intStartX = Math.Max(0, (int)(objSprocketPatternROI.ref_ROICenterX - 4 * fCalibX - objSprocketPatternROI.ref_ROIWidth));
                int intStartY = objSprocketPatternROI.ref_ROIPositionY;
                int intEndX = (int)(objSprocketPatternROI.ref_ROICenterX - 4 * fCalibX + objSprocketPatternROI.ref_ROIWidth);
                int intEndY = objSprocketPatternROI.ref_ROIPositionY + objSprocketPatternROI.ref_ROIHeight;

                objROI.LoadROISetting_RemoveOut(intStartX, intStartY, intEndX - intStartX, intEndY - intStartY);

                if (blnDebug)
                {
                    objROI.SaveImage("D:\\TS\\3.objROI.bmp");
                }

                float fEndX2 = 0;
                m_objSprocketBlobs.BuildObjects_Filter_GetElement(objROI, false, m_intConnexity == 4, 0, ROI.GetAutoThresholdValue(objROI, 3), 0, 999999, false, 0x0F);
                if (m_objSprocketBlobs.ref_intNumSelectedObject > 0)
                {
                    float fCenterX = m_objSprocketBlobs.ref_arrLimitCenterX[0];
                    float fCenterY = m_objSprocketBlobs.ref_arrLimitCenterY[0];
                    float fWidth = m_objSprocketBlobs.ref_arrWidth[0];
                    float fHeight = m_objSprocketBlobs.ref_arrHeight[0];
                    fEndX2 = objROI.ref_ROITotalX + fCenterX + fWidth / 2;

                    m_fTapePocketPitchByImage = Math.Abs(fEndX1 - fEndX2) / 2;
                }
                else
                {
                    m_fTapePocketPitchByImage = -1;
                }
            }
            else
            {
                // ------------ find sprocket blobs for current search ROI ------------------------------------
                if (m_objSprocketBlobs == null)
                    m_objSprocketBlobs = new EBlobs();
                else
                    m_objSprocketBlobs.CleanAllBlobs();

                float fEndX1 = 0;
                m_objSprocketBlobs.BuildObjects_Filter_GetElement(objSprocketPatternROI, false, m_intConnexity == 4, 0, ROI.GetAutoThresholdValue(objSprocketPatternROI, 3), 0, 999999, false, 0x0F);
                if (m_objSprocketBlobs.ref_intNumSelectedObject > 0)
                {
                    float fCenterX = m_objSprocketBlobs.ref_arrLimitCenterX[0];
                    float fCenterY = m_objSprocketBlobs.ref_arrLimitCenterY[0];
                    float fWidth = m_objSprocketBlobs.ref_arrWidth[0];
                    float fHeight = m_objSprocketBlobs.ref_arrHeight[0];
                    fEndX1 = objSprocketPatternROI.ref_ROITotalX + fCenterX - fWidth / 2;
                }

                // get right ROI sprocket hole position
                // 2020 04 15 - CCENG: use objSprocketPatternROI to build blob. objSearchSprocketPatternROI cannot use anymore bcos the X length is same as image now.
                //int intStartX = Math.Max(0, (int)(objSprocketPatternROI.ref_ROICenterX + 4 * fCalibX - objSearchSprocketPatternROI.ref_ROIWidth / 2));
                //int intStartY = objSprocketPatternROI.ref_ROIPositionY;
                //int intEndX = (int)(objSprocketPatternROI.ref_ROICenterX + 4 * fCalibX + objSearchSprocketPatternROI.ref_ROIWidth / 2);
                //int intEndY = objSprocketPatternROI.ref_ROIPositionY + objSprocketPatternROI.ref_ROIHeight;
                int intStartX = Math.Max(0, (int)(objSprocketPatternROI.ref_ROICenterX + 4 * fCalibX - objSprocketPatternROI.ref_ROIWidth));
                int intStartY = objSprocketPatternROI.ref_ROIPositionY;
                int intEndX = (int)(objSprocketPatternROI.ref_ROICenterX + 4 * fCalibX + objSprocketPatternROI.ref_ROIWidth);
                int intEndY = objSprocketPatternROI.ref_ROIPositionY + objSprocketPatternROI.ref_ROIHeight;

                objROI.LoadROISetting_RemoveOut(intStartX, intStartY, intEndX - intStartX, intEndY - intStartY);

                if (blnDebug)
                {
                    objROI.SaveImage("D:\\TS\\3.objROI.bmp");
                }

                float fEndX2 = 0;
                m_objSprocketBlobs.BuildObjects_Filter_GetElement(objROI, false, m_intConnexity == 4, 0, ROI.GetAutoThresholdValue(objROI, 3), 0, 999999, false, 0x0F);
                if (m_objSprocketBlobs.ref_intNumSelectedObject > 0)
                {
                    float fCenterX = m_objSprocketBlobs.ref_arrLimitCenterX[0];
                    float fCenterY = m_objSprocketBlobs.ref_arrLimitCenterY[0];
                    float fWidth = m_objSprocketBlobs.ref_arrWidth[0];
                    float fHeight = m_objSprocketBlobs.ref_arrHeight[0];
                    fEndX2 = objROI.ref_ROITotalX + fCenterX - fWidth / 2;

                    m_fTapePocketPitchByImage = Math.Abs(fEndX1 - fEndX2) / 2;
                }
                else
                {
                    m_fTapePocketPitchByImage = -1;
                }
            }




            objROI.Dispose();

            return true;
        }

        public int GetPocketMatcherWidth(int intSelectedTemplate)
        {
            if (intSelectedTemplate >= m_arrPocketMatcher.Count)
                return 0;

            return m_arrPocketMatcher[intSelectedTemplate].PatternWidth;
        }

        public int GetPocketMatcherHeight(int intSelectedTemplate)
        {
            if (intSelectedTemplate >= m_arrPocketMatcher.Count)
                return 0;

            return m_arrPocketMatcher[intSelectedTemplate].PatternHeight;
        }

        public int GetPositionMatcherWidth(int intSelectedTemplate)
        {
            if (intSelectedTemplate >= m_arrPositionMatcher.Count)
                return 0;

            return m_arrPositionMatcher[intSelectedTemplate].PatternWidth;
        }

        public int GetPositionMatcherHeight(int intSelectedTemplate)
        {
            if (intSelectedTemplate >= m_arrPositionMatcher.Count)
                return 0;

            return m_arrPositionMatcher[intSelectedTemplate].PatternHeight;
        }

        public void CalculateTemplateMarkPixels()
        {
            m_arrTemplateMarkWhitePixel.Clear();
            m_arrTemplateMarkBlackPixel.Clear();
            for (int t = 0; t < m_arrMatcherImage.Count; t++)
            {
                int intLowThresholdPixelCount = 0, intBtwThresholdPixelCount = 0, intHighThresholdPixelCount = 0;
                //EBW8 bwThreshold = new EBW8((byte)m_intMarkPixelThreshold);
                EBW8 bwThreshold = new EBW8((byte)m_arrTemplateMarkThreshold[t]);
                EasyImage.PixelCount(m_arrMatcherImage[t].ref_objMainImage, bwThreshold, bwThreshold, out intLowThresholdPixelCount, out intBtwThresholdPixelCount, out intHighThresholdPixelCount);

                m_arrTemplateMarkWhitePixel.Add(intHighThresholdPixelCount);
                m_arrTemplateMarkBlackPixel.Add(intLowThresholdPixelCount);
            }
        }
        public void RelearnMarkMatcherPattern(string strFolderPath, int intImageWidth, int intImageHeight)
        {
            ROI objDestinationROI = new ROI();

            ImageDrawing objBackgroundImage = new ImageDrawing(true, intImageWidth, intImageHeight);
            objDestinationROI.AttachImage(objBackgroundImage);

            for (int t = 0; t < m_arrMatcherImage.Count; t++)
            {
                m_intMarkTemplateIndex = t;

                objBackgroundImage.SetImageToBlack();
                objDestinationROI.AttachImage(objBackgroundImage);
                objDestinationROI.LoadROISetting(objBackgroundImage.ref_intImageWidth / 2 - m_arrMatcherImage[t].ref_intImageWidth / 2,
                                                 objBackgroundImage.ref_intImageHeight / 2 - m_arrMatcherImage[t].ref_intImageHeight / 2,
                                                 m_arrMatcherImage[t].ref_intImageWidth,
                                                 m_arrMatcherImage[t].ref_intImageHeight);

                //m_arrMatcherImage[t].SaveImage("D:\\TS\\m_arrMatcherImage.bmp");
                //objSourceMarkROI.AttachImage(m_arrMatcherImage[t]);
                //objSourceMarkROI.LoadROISetting(0, 0, m_arrMatcherImage[t].ref_intImageWidth, m_arrMatcherImage[t].ref_intImageHeight);
                //objSourceMarkROI.SaveImage("D:\\TS\\SourceROI.bmp");

                m_arrMatcherImage[t].CopyTo(ref objDestinationROI);
                //objDestinationROI.SaveImage("D:\\TS\\desROI.bmp");
                //objDestinationROI.ref_ROI.TopParent.Save("D:\\TS\\DesTopParent.bmp");
                //objBackgroundImage.SaveImage("D:\\TS\\bgimage.bmp");

                ReSaveSealMarkTemplate4Direction(strFolderPath, objDestinationROI);
            }
        }

        //public void ReSaveSealMarkTemplate4Direction(string strFolderPath, ROI objSourceMarkROI, int intMarkTemplateIndex)
        //{
        //    objSourceMarkROI.SaveImage(strFolderPath + "MarkTemplate0_" + intMarkTemplateIndex.ToString() + ".bmp");

        //    if (objSourceMarkROI.ref_ROIWidth == 0 || objSourceMarkROI.ref_ROIHeight == 0)
        //        return;

        //    try
        //    {
        //        ROI objMarkROI = new ROI();
        //        ImageDrawing objSourceImage = new ImageDrawing(true);
        //        objSourceMarkROI.CopyToTopParentImage(ref objSourceImage);
        //        objMarkROI.AttachImage(objSourceImage);
        //        objMarkROI.LoadROISetting(0, 0, objSourceMarkROI.ref_ROIWidth, objSourceMarkROI.ref_ROIHeight);

        //        if (m_intCheckMarkMethod == 2)
        //        {
        //            // 2020 10 02 - CCENG,CXLim - Init local Source Image bcos need to do threshold and prewitt image processing on source image
        //            EasyImage.Threshold(objMarkROI.ref_ROI, objMarkROI.ref_ROI, m_intMarkPixelThreshold);
        //            EasyImage.ConvolPrewitt(objMarkROI.ref_ROI);
        //        }

        //        ImageDrawing objImage = new ImageDrawing();
        //        objImage.SetImageSize(objMarkROI.ref_ROI.TopParent.Width, objMarkROI.ref_ROI.TopParent.Height);
        //        EasyImage.Copy(objMarkROI.ref_ROI.TopParent, objImage.ref_objMainImage);

        //        if (intMarkTemplateIndex == m_arrMarkMatcher.Count)
        //            m_arrMarkMatcher.Add(new List<EMatcher>());

        //        for (int i = 0; i < 4; i++)
        //        {
        //            m_arrMarkMatcher[intMarkTemplateIndex].Add(new EMatcher());
        //            if (i == 0)
        //            {
        //                m_arrMarkMatcher[intMarkTemplateIndex][i].MaxPositions = 5;

        //                objMarkROI.ref_ROI.Save("D:\\TS\\objMarkROI" + i.ToString() + ".bmp");
        //                m_arrMarkMatcher[intMarkTemplateIndex][i].LearnPattern(objMarkROI.ref_ROI);
        //                m_arrMarkMatcher[intMarkTemplateIndex][i].Save(strFolderPath + "MarkTemplate0_" + intMarkTemplateIndex.ToString() + "_" + i.ToString() + ".mch");
        //            }
        //            else
        //            {
        //                ROI objSourceROI = new ROI();
        //                objSourceROI.AttachImage(objMarkROI);
        //                float fSquareSize = (float)Math.Max(objMarkROI.ref_ROIWidth, objMarkROI.ref_ROIHeight);
        //                float fROICenterX = objMarkROI.ref_ROITotalX + (float)objMarkROI.ref_ROIWidth / 2;
        //                float fROICenterY = objMarkROI.ref_ROITotalY + (float)objMarkROI.ref_ROIHeight / 2;
        //                objSourceROI.SetPlacement((int)Math.Round(fROICenterX - fSquareSize / 2, 0, MidpointRounding.AwayFromZero),
        //                                          (int)Math.Round(fROICenterY - fSquareSize / 2, 0, MidpointRounding.AwayFromZero),
        //                                          (int)fSquareSize, (int)fSquareSize);


        //                ROI objDestROI = new ROI();
        //                objDestROI.AttachImage(objImage);
        //                objDestROI.LoadROISetting(0,0, objSourceROI.ref_ROIWidth, objSourceROI.ref_ROIHeight);

        //                int intRotateAngle;
        //                if (i == 1)
        //                    intRotateAngle = -90;
        //                else if (i == 2)
        //                    intRotateAngle = 180;
        //                else
        //                    intRotateAngle = 90;

        //                EasyImage.ScaleRotate(objSourceROI.ref_ROI, objSourceROI.ref_ROIWidth / 2f, objSourceROI.ref_ROIHeight / 2f, objDestROI.ref_ROIWidth / 2f, objDestROI.ref_ROIHeight / 2f, 1, 1, intRotateAngle, objDestROI.ref_ROI, 8);

        //                ROI objPatternROI = new ROI();
        //                objPatternROI.AttachImage(objImage);
        //                if (i == 2)
        //                {
        //                    objPatternROI.LoadROISetting(objMarkROI.ref_ROITotalX, objMarkROI.ref_ROITotalY, objMarkROI.ref_ROIWidth, objMarkROI.ref_ROIHeight);
        //                }
        //                else
        //                {
        //                    // Width and Height are switched each other   
        //                    objPatternROI.LoadROISetting((int)Math.Round(fROICenterX - (float)objMarkROI.ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
        //                                               (int)Math.Round(fROICenterY - (float)objMarkROI.ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
        //                                               objMarkROI.ref_ROIHeight, objMarkROI.ref_ROIWidth);
        //                }

        //                objMarkROI.ref_ROI.Save("D:\\TS\\objMarkROI" + i.ToString() + ".bmp");
        //                m_arrMarkMatcher[intMarkTemplateIndex][i].MaxPositions = 5;
        //                m_arrMarkMatcher[intMarkTemplateIndex][i].LearnPattern(objPatternROI.ref);
        //                m_arrMarkMatcher[intMarkTemplateIndex][i].Save(strFolderPath + "MarkTemplate0_" + intMarkTemplateIndex.ToString() + "_" + i.ToString() + ".mch");

        //                objSourceROI.Dispose();
        //                objDestROI.Dispose();
        //                objPatternROI.Dispose();
        //            }

        //        }

        //        objSourceImage.Dispose();
        //        objSourceImage = null;
        //        objImage.Dispose();
        //        objMarkROI.Dispose();
        //    }
        //    catch (Euresys.Open_eVision_1_2.EException ex)
        //    {
        //        m_strErrorMessage = "Orient Learn Pattern Error: " + ex.ToString();
        //    }
        //}

        public void ReSaveSealMarkTemplate4Direction(string strFolderPath, ROI objSourceMarkROI)
        {
            objSourceMarkROI.SaveImage(strFolderPath + "MarkTemplate0_" + m_intMarkTemplateIndex.ToString() + ".bmp");

            if (objSourceMarkROI.ref_ROIWidth == 0 || objSourceMarkROI.ref_ROIHeight == 0)
                return;

            try
            {
                ROI objMarkROI = new ROI();
                ImageDrawing objSourceImage = new ImageDrawing(true);
                objSourceMarkROI.CopyToTopParentImage(ref objSourceImage);
                objMarkROI.AttachImage(objSourceImage);
                objMarkROI.LoadROISetting(objSourceMarkROI.ref_ROITotalX, objSourceMarkROI.ref_ROITotalY, objSourceMarkROI.ref_ROIWidth, objSourceMarkROI.ref_ROIHeight);

                if (m_intCheckMarkMethod == 2)
                {
                    // 2020 10 02 - CCENG,CXLim - Init local Source Image bcos need to do threshold and prewitt image processing on source image
#if (Debug_2_12 || Release_2_12)
                    EasyImage.Threshold(objMarkROI.ref_ROI, objMarkROI.ref_ROI, (uint)m_arrTemplateMarkThreshold[m_intMarkTemplateIndex]);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                    EasyImage.Threshold(objMarkROI.ref_ROI, objMarkROI.ref_ROI, m_arrTemplateMarkThreshold[m_intMarkTemplateIndex]);
#endif
                    EasyImage.ConvolPrewitt(objMarkROI.ref_ROI);
                }
                else if (m_intCheckMarkMethod == 3) //2020-10-09 ZJYEOH : Do image processing based on customize sequence
                {
                    DoImageProcessingSequence(ref objMarkROI, m_intMarkTemplateIndex);
                }

                EImageBW8 objImage = new EImageBW8();
                objImage.SetSize(objMarkROI.ref_ROI.TopParent.Width, objMarkROI.ref_ROI.TopParent.Height);
                EasyImage.Copy(objMarkROI.ref_ROI.TopParent, objImage);

                if (m_intMarkTemplateIndex == m_arrMarkMatcher.Count)
                    m_arrMarkMatcher.Add(new List<EMatcher>());

                for (int i = 0; i < 4; i++)
                {
                    m_arrMarkMatcher[m_intMarkTemplateIndex].Add(new EMatcher());
                    if (i == 0)
                    {
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].MaxPositions = 5;

                        //objMarkROI.ref_ROI.Save("D:\\TS\\objMarkROI" + i.ToString() + ".bmp");
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].LearnPattern(objMarkROI.ref_ROI);
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].Save(strFolderPath + "MarkTemplate0_" + m_intMarkTemplateIndex.ToString() + "_" + i.ToString() + ".mch");
                    }
                    else
                    {
                        EROIBW8 objSourceROI = new EROIBW8();
                        objSourceROI.Detach();
                        objSourceROI.Attach(objMarkROI.ref_ROI.TopParent);
                        float fSquareSize = (float)Math.Max(objMarkROI.ref_ROIWidth, objMarkROI.ref_ROIHeight);
                        float fROICenterX = objMarkROI.ref_ROITotalX + (float)objMarkROI.ref_ROIWidth / 2;
                        float fROICenterY = objMarkROI.ref_ROITotalY + (float)objMarkROI.ref_ROIHeight / 2;
                        objSourceROI.SetPlacement((int)Math.Round(fROICenterX - fSquareSize / 2, 0, MidpointRounding.AwayFromZero),
                                                  (int)Math.Round(fROICenterY - fSquareSize / 2, 0, MidpointRounding.AwayFromZero),
                                                  (int)fSquareSize, (int)fSquareSize);

                        EROIBW8 objDestROI = new EROIBW8();
                        objDestROI.Detach();
                        objDestROI.Attach(objImage);
                        objDestROI.SetPlacement(objSourceROI.OrgX, objSourceROI.OrgY, objSourceROI.Width, objSourceROI.Height);

                        int intRotateAngle;
                        if (i == 1)
                            intRotateAngle = -90;
                        else if (i == 2)
                            intRotateAngle = 180;
                        else
                            intRotateAngle = 90;

                        EasyImage.ScaleRotate(objSourceROI, objSourceROI.Width / 2f, objSourceROI.Height / 2f, objDestROI.Width / 2f, objDestROI.Height / 2f, 1, 1, intRotateAngle, objDestROI, 8);

                        EROIBW8 objPatternROI = new EROIBW8();
                        objPatternROI.Detach();
                        objPatternROI.Attach(objImage);
                        if (i == 2)
                        {
                            objPatternROI.SetPlacement(objMarkROI.ref_ROITotalX, objMarkROI.ref_ROITotalY, objMarkROI.ref_ROIWidth, objMarkROI.ref_ROIHeight);
                        }
                        else
                        {
                            // Width and Height are switched each other   
                            objPatternROI.SetPlacement((int)Math.Round(fROICenterX - (float)objMarkROI.ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                       (int)Math.Round(fROICenterY - (float)objMarkROI.ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                       objMarkROI.ref_ROIHeight, objMarkROI.ref_ROIWidth);
                        }

                        //objMarkROI.ref_ROI.Save("D:\\TS\\objMarkROI" + i.ToString() + ".bmp");
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].MaxPositions = 5;
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].LearnPattern(objPatternROI);
                        m_arrMarkMatcher[m_intMarkTemplateIndex][i].Save(strFolderPath + "MarkTemplate0_" + m_intMarkTemplateIndex.ToString() + "_" + i.ToString() + ".mch");

                        objSourceROI.Dispose();
                    }

                }

                objSourceImage.Dispose();
                objSourceImage = null;
                objImage.Dispose();
                objMarkROI.Dispose();
            }
            catch (Euresys.Open_eVision_1_2.EException ex)
            {
                m_strErrorMessage = "Seal ReLearn Pattern Error: " + ex.ToString();
            }
        }
        public int GetMarkTemplateThreshold()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateMarkThreshold.Count)
                return m_intMarkPixelThreshold;
            else
                return m_arrTemplateMarkThreshold[m_intMarkTemplateIndex];
        }
        public float GetMarkTemplateThresholdRelative()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateMarkThresholdRelative.Count)
                return 0.5f;
            else
                return m_arrTemplateMarkThresholdRelative[m_intMarkTemplateIndex];
        }
        public bool GetTemplateWantAutoThresholdRelative()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateWantAutoThresholdRelative.Count)
                return false;
            else
                return m_arrTemplateWantAutoThresholdRelative[m_intMarkTemplateIndex];
        }
        public int GetTemplateErodeValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateErodeValue.Count)
                return 1;
            else
                return m_arrTemplateErodeValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateDilateValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateDilateValue.Count)
                return 1;
            else
                return m_arrTemplateDilateValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateOpenValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateOpenValue.Count)
                return 1;
            else
                return m_arrTemplateOpenValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateCloseValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateCloseValue.Count)
                return 1;
            else
                return m_arrTemplateCloseValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateErodeMinValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateErodeMinValue.Count)
                return 1;
            else
                return m_arrTemplateErodeMinValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateDilateMinValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateDilateMinValue.Count)
                return 1;
            else
                return m_arrTemplateDilateMinValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateOpenMinValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateOpenMinValue.Count)
                return 1;
            else
                return m_arrTemplateOpenMinValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateCloseMinValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateCloseMinValue.Count)
                return 1;
            else
                return m_arrTemplateCloseMinValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateThresholdMinValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateThresholdMinValue.Count)
                return 1;
            else
                return m_arrTemplateThresholdMinValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateErodeMaxValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateErodeMaxValue.Count)
                return 1;
            else
                return m_arrTemplateErodeMaxValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateDilateMaxValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateDilateMaxValue.Count)
                return 1;
            else
                return m_arrTemplateDilateMaxValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateOpenMaxValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateOpenMaxValue.Count)
                return 1;
            else
                return m_arrTemplateOpenMaxValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateCloseMaxValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateCloseMaxValue.Count)
                return 1;
            else
                return m_arrTemplateCloseMaxValue[m_intMarkTemplateIndex];
        }
        public int GetTemplateThresholdMaxValue()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateThresholdMaxValue.Count)
                return 1;
            else
                return m_arrTemplateThresholdMaxValue[m_intMarkTemplateIndex];
        }
        public List<string> GetTemplateImageProcessingSeq()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateImageProcessSeq.Count)
                return new List<string>();
            else
                return m_arrTemplateImageProcessSeq[m_intMarkTemplateIndex];
        }
        public Point GetTemplateMarkROIPosition()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateMarkROIPosition.Count)
                return new Point(-1, -1);
            else
                return m_arrTemplateMarkROIPosition[m_intMarkTemplateIndex];
        }
        public Size GetTemplateMarkROISize()
        {
            if (m_intMarkTemplateIndex >= m_arrTemplateMarkROISize.Count)
                return new Size(-1, -1);
            else
                return m_arrTemplateMarkROISize[m_intMarkTemplateIndex];
        }
        public void SetMarkTemplateThreshold(int intThresholdValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateMarkThreshold.Count)
            {
                m_arrTemplateMarkThreshold[m_intMarkTemplateIndex] = intThresholdValue;
            }
            else
            {
                for (int i = m_arrTemplateMarkThreshold.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateMarkThreshold.Add(intThresholdValue);
                }
            }
        }
        public void SetMarkTemplateThresholdRelative(float intThresholdRelativeValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateMarkThresholdRelative.Count)
            {
                m_arrTemplateMarkThresholdRelative[m_intMarkTemplateIndex] = intThresholdRelativeValue;
            }
            else
            {
                for (int i = m_arrTemplateMarkThresholdRelative.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateMarkThresholdRelative.Add(intThresholdRelativeValue);
                }
            }
        }
        public void SetTemplateWantAutoThresholdRelative(bool WantAutoThresholdRelative)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateWantAutoThresholdRelative.Count)
            {
                m_arrTemplateWantAutoThresholdRelative[m_intMarkTemplateIndex] = WantAutoThresholdRelative;
            }
            else
            {
                for (int i = m_arrTemplateWantAutoThresholdRelative.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateWantAutoThresholdRelative.Add(WantAutoThresholdRelative);
                }
            }
        }
        public void SetTemplateErodeValue(int intErodeValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateErodeValue.Count)
            {
                m_arrTemplateErodeValue[m_intMarkTemplateIndex] = intErodeValue;
            }
            else
            {
                for (int i = m_arrTemplateErodeValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateErodeValue.Add(intErodeValue);
                }
            }
        }
        public void SetTemplateDilateValue(int intDilateValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateDilateValue.Count)
            {
                m_arrTemplateDilateValue[m_intMarkTemplateIndex] = intDilateValue;
            }
            else
            {
                for (int i = m_arrTemplateDilateValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateDilateValue.Add(intDilateValue);
                }
            }
        }
        public void SetTemplateOpenValue(int intOpenValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateOpenValue.Count)
            {
                m_arrTemplateOpenValue[m_intMarkTemplateIndex] = intOpenValue;
            }
            else
            {
                for (int i = m_arrTemplateOpenValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateOpenValue.Add(intOpenValue);
                }
            }
        }
        public void SetTemplateCloseValue(int intCloseValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateCloseValue.Count)
            {
                m_arrTemplateCloseValue[m_intMarkTemplateIndex] = intCloseValue;
            }
            else
            {
                for (int i = m_arrTemplateCloseValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateCloseValue.Add(intCloseValue);
                }
            }
        }
        public void SetTemplateErodeMinValue(int intErodeMinValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateErodeMinValue.Count)
            {
                m_arrTemplateErodeMinValue[m_intMarkTemplateIndex] = intErodeMinValue;
            }
            else
            {
                for (int i = m_arrTemplateErodeMinValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateErodeMinValue.Add(intErodeMinValue);
                }
            }
        }
        public void SetTemplateDilateMinValue(int intDilateMinValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateDilateMinValue.Count)
            {
                m_arrTemplateDilateMinValue[m_intMarkTemplateIndex] = intDilateMinValue;
            }
            else
            {
                for (int i = m_arrTemplateDilateMinValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateDilateMinValue.Add(intDilateMinValue);
                }
            }
        }
        public void SetTemplateOpenMinValue(int intOpenMinValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateOpenMinValue.Count)
            {
                m_arrTemplateOpenMinValue[m_intMarkTemplateIndex] = intOpenMinValue;
            }
            else
            {
                for (int i = m_arrTemplateOpenMinValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateOpenMinValue.Add(intOpenMinValue);
                }
            }
        }
        public void SetTemplateCloseMinValue(int intCloseMinValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateCloseMinValue.Count)
            {
                m_arrTemplateCloseMinValue[m_intMarkTemplateIndex] = intCloseMinValue;
            }
            else
            {
                for (int i = m_arrTemplateCloseMinValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateCloseMinValue.Add(intCloseMinValue);
                }
            }
        }
        public void SetTemplateThresholdMinValue(int intThresholdMinValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateThresholdMinValue.Count)
            {
                m_arrTemplateThresholdMinValue[m_intMarkTemplateIndex] = intThresholdMinValue;
            }
            else
            {
                for (int i = m_arrTemplateThresholdMinValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateThresholdMinValue.Add(intThresholdMinValue);
                }
            }
        }
        public void SetTemplateErodeMaxValue(int intErodeMaxValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateErodeMaxValue.Count)
            {
                m_arrTemplateErodeMaxValue[m_intMarkTemplateIndex] = intErodeMaxValue;
            }
            else
            {
                for (int i = m_arrTemplateErodeMaxValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateErodeMaxValue.Add(intErodeMaxValue);
                }
            }
        }
        public void SetTemplateDilateMaxValue(int intDilateMaxValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateDilateMaxValue.Count)
            {
                m_arrTemplateDilateMaxValue[m_intMarkTemplateIndex] = intDilateMaxValue;
            }
            else
            {
                for (int i = m_arrTemplateDilateMaxValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateDilateMaxValue.Add(intDilateMaxValue);
                }
            }
        }
        public void SetTemplateOpenMaxValue(int intOpenMaxValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateOpenMaxValue.Count)
            {
                m_arrTemplateOpenMaxValue[m_intMarkTemplateIndex] = intOpenMaxValue;
            }
            else
            {
                for (int i = m_arrTemplateOpenMaxValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateOpenMaxValue.Add(intOpenMaxValue);
                }
            }
        }
        public void SetTemplateCloseMaxValue(int intCloseMaxValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateCloseMaxValue.Count)
            {
                m_arrTemplateCloseMaxValue[m_intMarkTemplateIndex] = intCloseMaxValue;
            }
            else
            {
                for (int i = m_arrTemplateCloseMaxValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateCloseMaxValue.Add(intCloseMaxValue);
                }
            }
        }
        public void SetTemplateThresholdMaxValue(int intThresholdMaxValue)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateThresholdMaxValue.Count)
            {
                m_arrTemplateThresholdMaxValue[m_intMarkTemplateIndex] = intThresholdMaxValue;
            }
            else
            {
                for (int i = m_arrTemplateThresholdMaxValue.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateThresholdMaxValue.Add(intThresholdMaxValue);
                }
            }
        }
        public void SetTemplateImageProcessingSeq(List<string> arr_Seq)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateImageProcessSeq.Count)
            {
                m_arrTemplateImageProcessSeq[m_intMarkTemplateIndex] = arr_Seq;
            }
            else
            {
                for (int i = m_arrTemplateImageProcessSeq.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateImageProcessSeq.Add(new List<string>());
                    m_arrTemplateImageProcessSeq[i] = arr_Seq;
                }
            }
        }
        public void SetTemplateMarkROIPositionAndSize(ROI objMarkROI)
        {
            if (m_intMarkTemplateIndex < m_arrTemplateMarkROIPosition.Count)
            {
                m_arrTemplateMarkROIPosition[m_intMarkTemplateIndex] = new Point(objMarkROI.ref_ROIPositionX, objMarkROI.ref_ROIPositionY);
            }
            else
            {
                for (int i = m_arrTemplateMarkROIPosition.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateMarkROIPosition.Add(new Point(objMarkROI.ref_ROIPositionX, objMarkROI.ref_ROIPositionY));
                }
            }

            if (m_intMarkTemplateIndex < m_arrTemplateMarkROISize.Count)
            {
                m_arrTemplateMarkROISize[m_intMarkTemplateIndex] = new Size(objMarkROI.ref_ROIWidth, objMarkROI.ref_ROIHeight);
            }
            else
            {
                for (int i = m_arrTemplateMarkROISize.Count; i <= m_intMarkTemplateIndex; i++)
                {
                    m_arrTemplateMarkROISize.Add(new Size(objMarkROI.ref_ROIWidth, objMarkROI.ref_ROIHeight));
                }
            }
        }
        public void DrawImageProcessingSequence(ref ROI objROI, int intThresholdValue, int intErode, int intDilate, int intOpen, int intClose, List<string> arr_Seq, int intViewSeqIndex)
        {
            if (objROI == null || arr_Seq.Count == 0)
                return;
            objROI.LoadROISetting(objROI.ref_ROIPositionX - 10, objROI.ref_ROIPositionY - 10, objROI.ref_ROIWidth + 20, objROI.ref_ROIHeight + 20);
            for (int i = 0; i < arr_Seq.Count; i++)
            {
                switch (arr_Seq[i])
                {
                    case "Threshold":

#if (Debug_2_12 || Release_2_12)
                        EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, (uint)intThresholdValue);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, intThresholdValue);
#endif
                        break;
                    case "Erode":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.ErodeBox(objROI.ref_ROI, objROI.ref_ROI, (uint)intErode);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.ErodeBox(objROI.ref_ROI, objROI.ref_ROI, intErode);
#endif

                        break;
                    case "Dilate":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.DilateBox(objROI.ref_ROI, objROI.ref_ROI, (uint)intDilate);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.DilateBox(objROI.ref_ROI, objROI.ref_ROI, intDilate);
#endif

                        break;
                    case "Open":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.OpenBox(objROI.ref_ROI, objROI.ref_ROI, (uint)intOpen);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.OpenBox(objROI.ref_ROI, objROI.ref_ROI, intOpen);
#endif

                        break;
                    case "Close":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.CloseBox(objROI.ref_ROI, objROI.ref_ROI, (uint)intClose);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.CloseBox(objROI.ref_ROI, objROI.ref_ROI, intClose);
#endif

                        break;
                    case "Prewitt":
                        EasyImage.ConvolPrewitt(objROI.ref_ROI);
                        break;
                }

                if (i == intViewSeqIndex)
                    break;

            }
            objROI.LoadROISetting(objROI.ref_ROIPositionX + 10, objROI.ref_ROIPositionY + 10, objROI.ref_ROIWidth - 20, objROI.ref_ROIHeight - 20);
        }
        public void DrawImageProcessingSequenceUntilThresholdStep(ref ROI objROI, int intThresholdValue, int intErode, int intDilate, int intOpen, int intClose, List<string> arr_Seq, int intViewSeqIndex)
        {
            if (objROI == null || arr_Seq.Count == 0)
                return;
            objROI.LoadROISetting(objROI.ref_ROIPositionX - 10, objROI.ref_ROIPositionY - 10, objROI.ref_ROIWidth + 20, objROI.ref_ROIHeight + 20);
            for (int i = 0; i < arr_Seq.Count; i++)
            {
                switch (arr_Seq[i])
                {
                    case "Threshold":
                        goto Finish;
#if (Debug_2_12 || Release_2_12)
                        EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, (uint)intThresholdValue);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, intThresholdValue);
#endif

                        break;
                    case "Erode":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.ErodeBox(objROI.ref_ROI, objROI.ref_ROI, (uint)intErode);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.ErodeBox(objROI.ref_ROI, objROI.ref_ROI, intErode);
#endif

                        break;
                    case "Dilate":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.DilateBox(objROI.ref_ROI, objROI.ref_ROI, (uint)intDilate);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.DilateBox(objROI.ref_ROI, objROI.ref_ROI, intDilate);
#endif

                        break;
                    case "Open":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.OpenBox(objROI.ref_ROI, objROI.ref_ROI, (uint)intOpen);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.OpenBox(objROI.ref_ROI, objROI.ref_ROI, intOpen);
#endif

                        break;
                    case "Close":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.CloseBox(objROI.ref_ROI, objROI.ref_ROI, (uint)intClose);
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.CloseBox(objROI.ref_ROI, objROI.ref_ROI, intClose);
#endif

                        break;
                    case "Prewitt":
                        EasyImage.ConvolPrewitt(objROI.ref_ROI);
                        break;
                }

                if (i == intViewSeqIndex)
                    break;

            }
        Finish:
            objROI.LoadROISetting(objROI.ref_ROIPositionX + 10, objROI.ref_ROIPositionY + 10, objROI.ref_ROIWidth - 20, objROI.ref_ROIHeight - 20);
        }
        public void DoImageProcessingSequence(ref ROI objROI, int intTemplateIndex)
        {
            if (objROI == null || m_arrTemplateImageProcessSeq.Count <= intTemplateIndex)
                return;
            objROI.LoadROISetting(objROI.ref_ROIPositionX - 10, objROI.ref_ROIPositionY - 10, objROI.ref_ROIWidth + 20, objROI.ref_ROIHeight + 20);
            //objROI.SaveImage("D:\\objROI1.bmp");
            //STTrackLog.WriteLine("Template = " + intTemplateIndex.ToString());
            for (int i = 0; i < m_arrTemplateImageProcessSeq[intTemplateIndex].Count; i++)
            {
                switch (m_arrTemplateImageProcessSeq[intTemplateIndex][i])
                {
                    case "Threshold":

#if (Debug_2_12 || Release_2_12)
                        if (intTemplateIndex < m_arrTemplateWantAutoThresholdRelative.Count && m_arrTemplateWantAutoThresholdRelative[intTemplateIndex])
                        {
                            objROI.LoadROISetting(objROI.ref_ROIPositionX + 10, objROI.ref_ROIPositionY + 10, objROI.ref_ROIWidth - 20, objROI.ref_ROIHeight - 20);
                            if (m_arrTemplateMarkThresholdRelative[intTemplateIndex] < 0)
                            {
                                int ThresholdValue = ROI.GetAutoThresholdValue(objROI, 3);
                                objROI.LoadROISetting(objROI.ref_ROIPositionX - 10, objROI.ref_ROIPositionY - 10, objROI.ref_ROIWidth + 20, objROI.ref_ROIHeight + 20);
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, (uint)(ThresholdValue));
                            }
                            else if (m_arrTemplateMarkThresholdRelative[intTemplateIndex] == 1)
                            {
                                int ThresholdValue = ROI.GetAutoThresholdValue(objROI, 2);
                                objROI.LoadROISetting(objROI.ref_ROIPositionX - 10, objROI.ref_ROIPositionY - 10, objROI.ref_ROIWidth + 20, objROI.ref_ROIHeight + 20);
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, (uint)(ThresholdValue));
                            }
                            else
                            {
                                int ThresholdValue = ROI.GetRelativeThresholdValue(objROI, m_arrTemplateMarkThresholdRelative[intTemplateIndex]);
                                objROI.LoadROISetting(objROI.ref_ROIPositionX - 10, objROI.ref_ROIPositionY - 10, objROI.ref_ROIWidth + 20, objROI.ref_ROIHeight + 20);
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, (uint)(ThresholdValue));

                            }
                        }
                        else
                        {
                            if (intTemplateIndex < m_arrTemplateMarkThreshold.Count)
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, (uint)(m_arrTemplateMarkThreshold[intTemplateIndex] + m_intTemplateThresholdTol));
                            else
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, (uint)(m_intMarkPixelThreshold + m_intTemplateThresholdTol));
                        }

# elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        if (m_arrTemplateWantAutoThresholdRelative[intTemplateIndex])
                        {
                            objROI.LoadROISetting(objROI.ref_ROIPositionX + 10, objROI.ref_ROIPositionY + 10, objROI.ref_ROIWidth - 20, objROI.ref_ROIHeight - 20);
                            if (m_arrTemplateMarkThresholdRelative[intTemplateIndex] < 0)
                            {
                                int ThresholdValue = ROI.GetAutoThresholdValue(objROI, 3);
                                objROI.LoadROISetting(objROI.ref_ROIPositionX - 10, objROI.ref_ROIPositionY - 10, objROI.ref_ROIWidth + 20, objROI.ref_ROIHeight + 20);
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, ThresholdValue);
                            }
                            else if (m_arrTemplateMarkThresholdRelative[intTemplateIndex] == 1)
                            {
                                int ThresholdValue = ROI.GetAutoThresholdValue(objROI, 2);
                                objROI.LoadROISetting(objROI.ref_ROIPositionX - 10, objROI.ref_ROIPositionY - 10, objROI.ref_ROIWidth + 20, objROI.ref_ROIHeight + 20);
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, ThresholdValue);
                            }
                            else
                            {
                                int ThresholdValue = ROI.GetRelativeThresholdValue(objROI, m_arrTemplateMarkThresholdRelative[intTemplateIndex]);
                                objROI.LoadROISetting(objROI.ref_ROIPositionX - 10, objROI.ref_ROIPositionY - 10, objROI.ref_ROIWidth + 20, objROI.ref_ROIHeight + 20);
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, ThresholdValue);

                            }
                        }
                        else
                        {
                            if (intTemplateIndex < m_arrTemplateMarkThreshold.Count)
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, m_arrTemplateMarkThreshold[intTemplateIndex] + m_intTemplateThresholdTol);
                            else
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, m_intMarkPixelThreshold + m_intTemplateThresholdTol);
                        }

#endif
                        //objROI.SaveImage("D:\\Threshold.bmp");
                        //STTrackLog.WriteLine("Threshold = " + (m_arrTemplateMarkThreshold[intTemplateIndex] + m_intTemplateThresholdTol).ToString());
                        break;
                    case "Erode":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.ErodeBox(objROI.ref_ROI, objROI.ref_ROI, (uint)(m_arrTemplateErodeValue[intTemplateIndex] + m_intTemplateErodeTol));
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.ErodeBox(objROI.ref_ROI, objROI.ref_ROI, m_arrTemplateErodeValue[intTemplateIndex] + m_intTemplateErodeTol);
#endif
                        //objROI.SaveImage("D:\\Erode.bmp");
                        //STTrackLog.WriteLine("Erode = " + (m_arrTemplateErodeValue[intTemplateIndex] + m_intTemplateErodeTol).ToString());
                        break;
                    case "Dilate":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.DilateBox(objROI.ref_ROI, objROI.ref_ROI, (uint)(m_arrTemplateDilateValue[intTemplateIndex] + m_intTemplateDilateTol));
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.DilateBox(objROI.ref_ROI, objROI.ref_ROI, m_arrTemplateDilateValue[intTemplateIndex] + m_intTemplateDilateTol);
#endif
                        //objROI.SaveImage("D:\\Dilate.bmp");
                        //STTrackLog.WriteLine("Dilate = " + (m_arrTemplateDilateValue[intTemplateIndex] + m_intTemplateDilateTol).ToString());
                        break;
                    case "Open":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.OpenBox(objROI.ref_ROI, objROI.ref_ROI, (uint)(m_arrTemplateOpenValue[intTemplateIndex] + m_intTemplateOpenTol));
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.OpenBox(objROI.ref_ROI, objROI.ref_ROI, m_arrTemplateOpenValue[intTemplateIndex] + m_intTemplateOpenTol);
#endif
                        //objROI.SaveImage("D:\\Open.bmp");
                        //STTrackLog.WriteLine("Open = " + (m_arrTemplateOpenValue[intTemplateIndex] + m_intTemplateOpenTol).ToString());
                        break;
                    case "Close":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.CloseBox(objROI.ref_ROI, objROI.ref_ROI, (uint)(m_arrTemplateCloseValue[intTemplateIndex] + m_intTemplateCloseTol));
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.CloseBox(objROI.ref_ROI, objROI.ref_ROI, m_arrTemplateCloseValue[intTemplateIndex] + m_intTemplateCloseTol);
#endif
                        //objROI.SaveImage("D:\\Close.bmp");
                        //STTrackLog.WriteLine("Close = " + (m_arrTemplateCloseValue[intTemplateIndex] + m_intTemplateCloseTol).ToString());
                        break;
                    case "Prewitt":
                        EasyImage.ConvolPrewitt(objROI.ref_ROI);
                        //objROI.SaveImage("D:\\Prewitt.bmp");
                        break;
                }
            }
            //objROI.SaveImage("D:\\objROI2.bmp");
            objROI.LoadROISetting(objROI.ref_ROIPositionX + 10, objROI.ref_ROIPositionY + 10, objROI.ref_ROIWidth - 20, objROI.ref_ROIHeight - 20);
            //objROI.SaveImage("D:\\objROI3.bmp");
        }
        public void DoImageProcessingSequence(ref ROI objROI, ROI objMarkROI, int intTemplateIndex)
        {
            if (objROI == null || m_arrTemplateImageProcessSeq.Count <= intTemplateIndex)
                return;
            objROI.LoadROISetting(objROI.ref_ROIPositionX - 10, objROI.ref_ROIPositionY - 10, objROI.ref_ROIWidth + 20, objROI.ref_ROIHeight + 20);
            //objROI.SaveImage("D:\\objROI1.bmp");
            //STTrackLog.WriteLine("Template = " + intTemplateIndex.ToString());
            for (int i = 0; i < m_arrTemplateImageProcessSeq[intTemplateIndex].Count; i++)
            {
                switch (m_arrTemplateImageProcessSeq[intTemplateIndex][i])
                {
                    case "Threshold":

#if (Debug_2_12 || Release_2_12)
                        if (intTemplateIndex < m_arrTemplateWantAutoThresholdRelative.Count && m_arrTemplateWantAutoThresholdRelative[intTemplateIndex])
                        {
                            if (intTemplateIndex < m_arrTemplateMarkROIPosition.Count && intTemplateIndex < m_arrTemplateMarkROISize.Count)
                                objMarkROI.LoadROISetting(m_arrTemplateMarkROIPosition[intTemplateIndex].X, m_arrTemplateMarkROIPosition[intTemplateIndex].Y,
                                                          m_arrTemplateMarkROISize[intTemplateIndex].Width, m_arrTemplateMarkROISize[intTemplateIndex].Height);

                            if (m_arrTemplateMarkThresholdRelative[intTemplateIndex] < 0)
                            {
                                int ThresholdValue = ROI.GetAutoThresholdValue(objMarkROI, 3);

                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, (uint)(ThresholdValue));
                            }
                            else if (m_arrTemplateMarkThresholdRelative[intTemplateIndex] == 1)
                            {
                                int ThresholdValue = ROI.GetAutoThresholdValue(objMarkROI, 2);

                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, (uint)(ThresholdValue));
                            }
                            else
                            {
                                int ThresholdValue = ROI.GetRelativeThresholdValue(objMarkROI, m_arrTemplateMarkThresholdRelative[intTemplateIndex]);

                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, (uint)(ThresholdValue));

                            }
                        }
                        else
                        {
                            if (intTemplateIndex < m_arrTemplateMarkThreshold.Count)
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, (uint)(m_arrTemplateMarkThreshold[intTemplateIndex] + m_intTemplateThresholdTol));
                            else
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, (uint)(m_intMarkPixelThreshold + m_intTemplateThresholdTol));
                        }

# elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        if (m_arrTemplateWantAutoThresholdRelative[intTemplateIndex])
                        {
                            if (intTemplateIndex < m_arrTemplateMarkROIPosition.Count && intTemplateIndex < m_arrTemplateMarkROISize.Count)
                                objMarkROI.LoadROISetting(m_arrTemplateMarkROIPosition[intTemplateIndex].X, m_arrTemplateMarkROIPosition[intTemplateIndex].Y,
                                                          m_arrTemplateMarkROISize[intTemplateIndex].Width, m_arrTemplateMarkROISize[intTemplateIndex].Height);
                            if (m_arrTemplateMarkThresholdRelative[intTemplateIndex] < 0)
                            {
                                int ThresholdValue = ROI.GetAutoThresholdValue(objMarkROI, 3);

                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, ThresholdValue);
                            }
                            else if (m_arrTemplateMarkThresholdRelative[intTemplateIndex] == 1)
                            {
                                int ThresholdValue = ROI.GetAutoThresholdValue(objMarkROI, 2);

                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, ThresholdValue);
                            }
                            else
                            {
                                int ThresholdValue = ROI.GetRelativeThresholdValue(objMarkROI, m_arrTemplateMarkThresholdRelative[intTemplateIndex]);

                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, ThresholdValue);

                            }
                        }
                        else
                        {
                            if (intTemplateIndex < m_arrTemplateMarkThreshold.Count)
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, m_arrTemplateMarkThreshold[intTemplateIndex] + m_intTemplateThresholdTol);
                            else
                                EasyImage.Threshold(objROI.ref_ROI, objROI.ref_ROI, m_intMarkPixelThreshold + m_intTemplateThresholdTol);
                        }

#endif
                        //objROI.SaveImage("D:\\Threshold.bmp");
                        //STTrackLog.WriteLine("Threshold = " + (m_arrTemplateMarkThreshold[intTemplateIndex] + m_intTemplateThresholdTol).ToString());
                        break;
                    case "Erode":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.ErodeBox(objROI.ref_ROI, objROI.ref_ROI, (uint)(m_arrTemplateErodeValue[intTemplateIndex] + m_intTemplateErodeTol));
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.ErodeBox(objROI.ref_ROI, objROI.ref_ROI, m_arrTemplateErodeValue[intTemplateIndex] + m_intTemplateErodeTol);
#endif
                        //objROI.SaveImage("D:\\Erode.bmp");
                        //STTrackLog.WriteLine("Erode = " + (m_arrTemplateErodeValue[intTemplateIndex] + m_intTemplateErodeTol).ToString());
                        break;
                    case "Dilate":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.DilateBox(objROI.ref_ROI, objROI.ref_ROI, (uint)(m_arrTemplateDilateValue[intTemplateIndex] + m_intTemplateDilateTol));
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.DilateBox(objROI.ref_ROI, objROI.ref_ROI, m_arrTemplateDilateValue[intTemplateIndex] + m_intTemplateDilateTol);
#endif
                        //objROI.SaveImage("D:\\Dilate.bmp");
                        //STTrackLog.WriteLine("Dilate = " + (m_arrTemplateDilateValue[intTemplateIndex] + m_intTemplateDilateTol).ToString());
                        break;
                    case "Open":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.OpenBox(objROI.ref_ROI, objROI.ref_ROI, (uint)(m_arrTemplateOpenValue[intTemplateIndex] + m_intTemplateOpenTol));
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.OpenBox(objROI.ref_ROI, objROI.ref_ROI, m_arrTemplateOpenValue[intTemplateIndex] + m_intTemplateOpenTol);
#endif
                        //objROI.SaveImage("D:\\Open.bmp");
                        //STTrackLog.WriteLine("Open = " + (m_arrTemplateOpenValue[intTemplateIndex] + m_intTemplateOpenTol).ToString());
                        break;
                    case "Close":
#if (Debug_2_12 || Release_2_12)
                        EasyImage.CloseBox(objROI.ref_ROI, objROI.ref_ROI, (uint)(m_arrTemplateCloseValue[intTemplateIndex] + m_intTemplateCloseTol));
#elif (DEBUG || RELEASE || RTXRelease || RTXDebug)
                        EasyImage.CloseBox(objROI.ref_ROI, objROI.ref_ROI, m_arrTemplateCloseValue[intTemplateIndex] + m_intTemplateCloseTol);
#endif
                        //objROI.SaveImage("D:\\Close.bmp");
                        //STTrackLog.WriteLine("Close = " + (m_arrTemplateCloseValue[intTemplateIndex] + m_intTemplateCloseTol).ToString());
                        break;
                    case "Prewitt":
                        EasyImage.ConvolPrewitt(objROI.ref_ROI);
                        //objROI.SaveImage("D:\\Prewitt.bmp");
                        break;
                }
            }
            //objROI.SaveImage("D:\\objROI2.bmp");
            objROI.LoadROISetting(objROI.ref_ROIPositionX + 10, objROI.ref_ROIPositionY + 10, objROI.ref_ROIWidth - 20, objROI.ref_ROIHeight - 20);
            //objROI.SaveImage("D:\\objROI3.bmp");
        }

        public void ClearTemplateSetting()
        {
            m_arrMarkMatcher.Clear();
        }

        public void DeleteAllPreviousTemplate(string strFolderPath)
        {
            if (Directory.Exists(strFolderPath + "Template\\Mark"))
            {
                try
                {
                    Directory.Delete(strFolderPath + "Template\\Mark", true);
                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show(ex.ToString());
                }
            }
        }

        public int GetOverHeatMinArea(int intROIIndex)
        {
            if (m_arrOverHeatBlobs.Length > intROIIndex && m_arrOverHeatBlobs[intROIIndex] != null)
            {
                return m_arrOverHeatBlobs[intROIIndex].ref_intMinAreaLimit;
            }
            else
                return 0;
        }
        public int GetScratchesMinArea(int intROIIndex)
        {
            if (m_arrScratchesBlobs.Length > intROIIndex && m_arrScratchesBlobs[intROIIndex] != null)
            {
                return m_arrScratchesBlobs[intROIIndex].ref_intMinAreaLimit;
            }
            else
                return 0;
        }
        public float GetOverHeatAreaMinTolerance(int intROIIndex)
        {
            if (m_arrOverHeatAreaMinTolerance.Length > intROIIndex)
            {
                return m_arrOverHeatAreaMinTolerance[intROIIndex];
            }
            else
                return 0;
        }
        public float GetScratchesAreaMinTolerance(int intROIIndex)
        {
            if (m_arrScratchesAreaMinTolerance.Length > intROIIndex)
            {
                return m_arrScratchesAreaMinTolerance[intROIIndex];
            }
            else
                return 0;
        }
        public int GetOverHeatLowThreshold(int intROIIndex)
        {
            if (m_arrOverHeatBlobs.Length > intROIIndex && m_arrOverHeatBlobs[intROIIndex] != null)
            {
                return m_arrOverHeatBlobs[intROIIndex].ref_intAbsoluteLowThreshold;
            }
            else
                return -4;
        }
        public int GetOverHeatHighThreshold(int intROIIndex)
        {
            if (m_arrOverHeatBlobs.Length > intROIIndex && m_arrOverHeatBlobs[intROIIndex] != null)
            {
                return m_arrOverHeatBlobs[intROIIndex].ref_intAbsoluteHighThreshold;
            }
            else
                return -4;
        }
        public int GetScratchesLowThreshold(int intROIIndex)
        {
            if (m_arrScratchesBlobs.Length > intROIIndex && m_arrScratchesBlobs[intROIIndex] != null)
            {
                return m_arrScratchesBlobs[intROIIndex].ref_intAbsoluteThreshold;
            }
            else
                return -4;
        }
        public void SetOverHeatMinArea(int intROIIndex, int intValue)
        {
            if (m_arrOverHeatBlobs.Length > intROIIndex && m_arrOverHeatBlobs[intROIIndex] != null)
            {
                m_arrOverHeatBlobs[intROIIndex].SetObjectAreaRange(intValue, 99999);
            }
        }
        public void SetScratchesMinArea(int intROIIndex, int intValue)
        {
            if (m_arrScratchesBlobs.Length > intROIIndex && m_arrScratchesBlobs[intROIIndex] != null)
            {
                 m_arrScratchesBlobs[intROIIndex].SetObjectAreaRange(intValue, 99999);
            }
        }
        public void SetOverHeatAreaMinTolerance(int intROIIndex, float intValue)
        {
            if (m_arrOverHeatAreaMinTolerance.Length > intROIIndex)
            {
                m_arrOverHeatAreaMinTolerance[intROIIndex] = intValue;
            }
        }
        public void SetScratchesAreaMinTolerance(int intROIIndex, float intValue)
        {
            if (m_arrScratchesAreaMinTolerance.Length > intROIIndex)
            {
                m_arrScratchesAreaMinTolerance[intROIIndex] = intValue;
            }
        }
        public void SetOverHeatLowThreshold(int intROIIndex, int intValue)
        {
            if (m_arrOverHeatBlobs.Length > intROIIndex && m_arrOverHeatBlobs[intROIIndex] != null)
            {
                m_arrOverHeatBlobs[intROIIndex].ref_intAbsoluteLowThreshold = intValue;
            }
        }
        public void SetOverHeatHighThreshold(int intROIIndex, int intValue)
        {
            if (m_arrOverHeatBlobs.Length > intROIIndex && m_arrOverHeatBlobs[intROIIndex] != null)
            {
                 m_arrOverHeatBlobs[intROIIndex].ref_intAbsoluteHighThreshold = intValue;
            }
        }
        public void SetScratchesLowThreshold(int intROIIndex, int intValue)
        {
            if (m_arrScratchesBlobs.Length > intROIIndex && m_arrScratchesBlobs[intROIIndex] != null)
            {
                m_arrScratchesBlobs[intROIIndex].ref_intAbsoluteThreshold = intValue;
            }
        }
        public float GetOverHeatFailArea(int intROIIndex)
        {
            if (m_arrFailOverheatArea.Length > intROIIndex)
            {
                return m_arrFailOverheatArea[intROIIndex];
            }
            else
                return -999;
        }
        public float GetScratchesFailArea(int intROIIndex)
        {
            if (m_arrFailScratchesArea.Length > intROIIndex)
            {
                return m_arrFailScratchesArea[intROIIndex];
            }
            else
                return -999;
        }
        public bool IsOverHeatFail(int intROIIndex)
        {
            if (m_arrFailOverHeat.Length > intROIIndex)
            {
                return m_arrFailOverHeat[intROIIndex];
            }
            else
                return true;
        }
        
    }
}
