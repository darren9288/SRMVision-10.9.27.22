using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Common;
using VisionProcessing;
using ImageAcquisition;

namespace SharedMemory
{
    public class VisionInfo
    {
        public bool g_blnWantReloadRecipe = false;
        public bool g_blnVisionPageTimerON = false;

        //vision module name
        public string g_strVisionName = ""; // Vision module name
        public string g_strVisionDisplayName = "";
        public string g_strVisionNameRemark = "";
        public string g_strVisionFolderName = "";
        public string g_strDeviceFolderName = "";
        public string g_strVisionNameNo = "";    // ""= no same vision name. 2 = 2 vision same name e.g 2 same InPocketPkg vision name.
        public int g_intVisionPos = 0;
        public int g_intVisionIndex = 0;    // Vision same vision module index
        public int g_intVisionSameCount = 0;
        public int g_intVisionNameNo = 0;
        public int g_intVisionResetCount = 0;
        public string g_strVisionResetCountTime = DateTime.Now.ToString("yyyyMMddhhmmss");

        // Status Info
        public int g_intSavingState = 0;
        public string g_strErrorMessage = "";
        public string g_strErrorMessageForSecondUnit = "";
        public Color g_cErrorMessageColor = Color.Red;  // By default, the message is red color. Only need to set to black color when want display message in black color.
        public Color g_cErrorMessageColorForSecondUnit = Color.Red;  // By default, the message is red color. Only need to set to black color when want display message in black color.
        public int g_intMachineStatus = 1; // 1=Idle, 2=Operating

        // TCPIP
        public string g_strTCPMessage = "";
        public int g_intPort = 8081;
        public bool g_blnDiagnosticOpen = false;

        //Yield
        public bool g_blnStopLowYield;
        public float g_fLowYield;
        public int g_intMinUnitCheck;

        public bool g_blnStopContinuousPass;
        public int g_intMinPassUnit;
        public bool g_blnStopContinuousFail;
        public int g_intMinFailUnit;
        public int g_intDelayCheckIO;

        // Reference Tolerance
        public bool g_blnWantLoadRefTolWhenNewLot = false;
        public bool g_blnWantLoadPadRefTol = false;
        public bool g_blnWantLoadPadPackageRefTol = false;
        public bool g_blnWantLoadOtherRefTol = false;
        public string g_strBrowsePath = "";
        
        // Result Log Counter
        public int g_intResultLogMaxCount = 1000;
        public bool g_blnWantRecordResult = false;

        // Counter
        public bool g_blnResetCount = false;
        public bool g_blnResetGUIDisplayCount = false;

        // Counter
        public int g_intPassTotal;
        public int g_intTestedTotal;
        public int g_intHourlyPassCount;
        public int g_intHourlyTestedCount;
        public int g_intLowYieldUnitCount;
        public int g_intLowYieldPass;
        public int g_intContinuousPassUnitCount;
        public int g_intContinuousFailUnitCount;

        public int g_intLotlyPassTotal;
        public int g_intLotlyTestedTotal;
        public int g_intLotlyCheckPresenceFailureTotal;
        public int g_intLotlyLeadFailureTotal;
        public int g_intLotlyMarkFailureTotal;
        public int g_intLotlyOrientFailureTotal;
        public int g_intLotlyPackageFailureTotal;
        public int g_intLotlyPadFailureTotal;
        public int g_intLotlySealFailureTotal;

        public int g_intBarcodeFailureTotal;

        public int g_intEmptyUnitFailureTotal;
        public int g_intPositionFailureTotal;
        public int g_intOrientFailureTotal;
        public int g_intPin1FailureTotal;
        public int g_intMarkFailureTotal;
        public int g_intPackageFailureTotal;
        public int g_intLeadFailureTotal;
        public int g_intPadFailureTotal;
        public int g_intSealFailureTotal;
        public int g_intSealDistanceFailureTotal;
        public int g_intSealOverHeatFailureTotal;   // For Over heat and tape scratches
        public int g_intSealBrokenAreaFailureTotal;
        public int g_intSealBrokenGapFailureTotal;
        public int g_intSealSprocketHoleFailureTotal;
        public int g_intSealSprocketHoleDiameterFailureTotal;
        public int g_intSealSprocketHoleDefectFailureTotal;
        public int g_intSealSprocketHoleBrokenFailureTotal;
        public int g_intSealSprocketHoleRoundnessFailureTotal;
        public int g_intSealEdgeStraightnessFailureTotal;
        public int g_intCheckPresenceFailureTotal;
        public int g_intNoTemplateFailureTotal;
        public int g_intPkgDefectFailureTotal;
        public int g_intPkgColorDefectFailureTotal;
        public int g_intEdgeNotFoundFailureTotal;
        public int g_intAngleFailureTotal;
        public int g_intCenterPadOffsetFailureTotal;
        public int g_intCenterPadAreaFailureTotal;
        public int g_intCenterPadDimensionFailureTotal;
        public int g_intCenterPadPitchGapFailureTotal;
        public int g_intCenterPadBrokenFailureTotal;
        public int g_intCenterPadExcessFailureTotal;
        public int g_intCenterPadSmearFailureTotal;
        public int g_intCenterPadEdgeLimitFailureTotal;
        public int g_intCenterPadStandOffFailureTotal;
        public int g_intCenterPadEdgeDistanceFailureTotal;
        public int g_intCenterPadSpanFailureTotal;
        public int g_intCenterPadContaminationFailureTotal;
        public int g_intCenterPadMissingFailureTotal;
        public int g_intCenterPadColorDefectFailureTotal;
        public int g_intSidePadOffsetFailureTotal;
        public int g_intSidePadAreaFailureTotal;
        public int g_intSidePadDimensionFailureTotal;
        public int g_intSidePadPitchGapFailureTotal;
        public int g_intSidePadBrokenFailureTotal;
        public int g_intSidePadExcessFailureTotal;
        public int g_intSidePadSmearFailureTotal;
        public int g_intSidePadEdgeLimitFailureTotal;
        public int g_intSidePadStandOffFailureTotal;
        public int g_intSidePadEdgeDistanceFailureTotal;
        public int g_intSidePadSpanFailureTotal;
        public int g_intSidePadContaminationFailureTotal;
        public int g_intSidePadMissingFailureTotal;
        public int g_intSidePadColorDefectFailureTotal;
        public int g_intCenterPkgDefectFailureTotal;
        public int g_intCenterPkgDimensionFailureTotal;
        public int g_intSidePkgDefectFailureTotal;
        public int g_intSidePkgDimensionFailureTotal;
        public int g_intLeadOffsetFailureTotal;
        public int g_intLeadSkewFailureTotal;
        public int g_intLeadWidthFailureTotal;
        public int g_intLeadLengthFailureTotal;
        public int g_intLeadLengthVarianceFailureTotal;
        public int g_intLeadPitchGapFailureTotal;
        public int g_intLeadPitchVarianceFailureTotal;
        public int g_intLeadStandOffFailureTotal;
        public int g_intLeadStandOffVarianceFailureTotal;
        public int g_intLeadCoplanFailureTotal;
        public int g_intLeadAGVFailureTotal;
        public int g_intLeadSpanFailureTotal;
        public int g_intLeadSweepsFailureTotal;
        public int g_intLeadUnCutTiebarFailureTotal;
        public int g_intLeadMissingFailureTotal;
        public int g_intLeadContaminationFailureTotal;
        public int g_intLeadPkgDefectFailureTotal;
        public int g_intLeadPkgDimensionFailureTotal;

        public string g_strResult;
        public string g_strResult2;

        public int g_intTotalImageCount;
        public int g_intPassImageCount;
        public int g_intFailImageCount;

        // GRR - Dimention 1: Pad number, 2: Parameters
        public GRR g_objGRR;
        public bool g_blnGRRON = false;

        public string g_strGRRName;
        public int g_intGRRMode = 0;    // 0: Static, 1: Dynamic
        public int g_intPartNo = 10;
        public int g_intOperatorNo = 3;
        public int g_intTrialNo = 3;
        public int g_intPartCount = 0;
        public int g_intOperatorCount = 0;
        public int g_intTrialCount = 0;

        //CPK
        public List<ArrayList> g_arrCPKData = new List<ArrayList>();
        public CPK g_objCPK;
        public bool g_blnCPKON = false;
        public bool g_blnInitCPK = false;
        public bool g_blnReInitCPK = false;
        public int g_intCPKTestCount = 0;
        public bool g_blnRecordAllPadCPKEvenIfFail = false;

        // Camera & Lighting
        public bool g_blnGlobalSharingCameraData = false;
        public bool g_blnWhiteBalanceAuto = false;
        public double g_dRedRatio = 2.309997559;
        public double g_dBlueRatio = 2.539978027;
        public string g_strCameraModel = "AVT";
        public int g_intCameraPortNo = 0;
        public uint g_uintCameraGUID = 0;
        public int g_intImageSharpness = 0;
        public ROI g_objCameraROI = new ROI();
        public int g_intCameraGrabDelay = 0;
        public int g_intLightIntensity = 0;
        public int g_intCameraResolutionWidth = 640;
        public int g_intCameraResolutionHeight = 480;
        public float g_fZoomScale = 1f;
        public float g_fScaleX = 1f;
        public float g_fScaleY = 1f;
        public List<LightSource> g_arrLightSource = new List<LightSource>();
        public string g_strCameraSerialNo = "";
        public bool g_blnGrabbing = false;  // True: Stop drawing during inspection or during change intensity setting for sequence controller.
        public bool g_blnStopAfterFail = false;
        // Color 
        public CROI g_objColorROI;
        public int[] g_intColorThreshold = new int[3];
        public int[] g_intColorTolerance = new int[3];
        public int g_intColorCloseIteration = 0;
        public bool g_blnColorInvertBlackWhite = false;
        public bool g_blnGetPixel = false;
        public bool g_blnViewColorImage = false;
        public bool g_blnViewColorThreshold = false;
        public bool g_blnUseRGBFormat = false; // true = RGB Format; false = LSH format
        public int g_intColorFormat = 0; // 0 = LSH Format; 1 = RGB format; 2 = Saturation format
        public bool g_blnColorThresholdForm = false; // true = color threshold form is opened

        // Image
        public bool g_blnDrawLead3DCalibrationResult = false;
        public bool g_blnDrawThresholdAllSideROI = false;
        public bool g_blnDrawFreeShapeDone = true;
        public int g_intImageViewCount = 0;
        public Image g_objDontCareImage;
        public ImageDrawing g_objBlackWhiteLeftImage = new ImageDrawing();
        public ImageDrawing g_objBlackWhiteRightImage = new ImageDrawing();
        public ImageDrawing g_objBlackWhiteBottomImage = new ImageDrawing();
        public ImageDrawing g_objBlackWhiteTopImage = new ImageDrawing();
        public ImageDrawing g_objBlackImage = new ImageDrawing();
        public ImageDrawing g_objWhiteImage = new ImageDrawing();
        public ImageDrawing g_objBrightReferenceImage = new ImageDrawing();
        public ImageDrawing g_objDarkReferenceImage = new ImageDrawing();
        public List<ImageDrawing> g_arrReferenceImages = new List<ImageDrawing>();
        public List<ImageDrawing> g_arrInvertedReferenceImages = new List<ImageDrawing>();
        public List<CImageDrawing> g_arrReferenceColorImages = new List<CImageDrawing>();
        public List<CImageDrawing> g_arrInvertedReferenceColorImages = new List<CImageDrawing>();
        public ImageDrawing g_objMemoryImage = new ImageDrawing();
        public List<ImageDrawing> g_arrMemoryImage = new List<ImageDrawing>();
        public ImageDrawing g_ojRotateImage = new ImageDrawing();
        public List<ImageDrawing> g_arrRotateImage = new List<ImageDrawing>();
        public List<CImageDrawing> g_arrCRotateImage = new List<CImageDrawing>();
        public CImageDrawing g_objMemoryColorImage = new CImageDrawing();
        public List<CImageDrawing> g_arrMemoryColorImage = new List<CImageDrawing>();
        public List<CImageDrawing> g_arrColorImages;
        public List<CImageDrawing> g_arrColorRotatedImages;
        public List<ImageDrawing> g_arrImages = new List<ImageDrawing>();
        public List<ImageDrawing> g_arrDebugImages = new List<ImageDrawing>();
        public List<CImageDrawing> g_arrDebugColorImages = new List<CImageDrawing>();
        public List<ImageDrawing> g_arrGrabImages = new List<ImageDrawing>();
        public List<ImageDrawing> g_arrRotatedImages = new List<ImageDrawing>();
        public List<ImageDrawing>[] g_arr5SRotatedImages = new List<ImageDrawing>[5];   // 1st Array index = Pad ROI index (0=Center, 1=Top, 2=Right, 3=Bottom, 4=Left). 2nd Array Index = arr Image index.
        public List<CImageDrawing>[] g_arr5SColorRotatedImages = new List<CImageDrawing>[5];   // 1st Array index = Pad ROI index (0=Center, 1=Top, 2=Right, 3=Bottom, 4=Left). 2nd Array Index = arr Image index.
        public ImageDrawing g_objPackageImage;
        public ImageDrawing g_objMarkImage;
        public ImageDrawing g_objLeadImage;
        //public ImageDrawing g_objModifiedPackageImage; //For don't care use
        public ImageDrawing g_objSealImage;
        //public ImageDrawing g_objDontCareImage_Mark = new ImageDrawing();
        //public ImageDrawing g_objDontCareImage_Package = new ImageDrawing();
        public ImageDrawing g_objDontCareImage_Package_Bright = new ImageDrawing();
        public ImageDrawing g_objDontCareImage_Package_Dark = new ImageDrawing();
        public List<uint> g_arrCameraGain = new List<uint>();
        public List<float> g_arrImageGain = new List<float>();
        public List<List<float>> g_arrImageExtraGain = new List<List<float>>();
        public int g_intEnhanceImage_Close = 0;     // Default is 0 because it is new parameter and prevent affecting existing machine that running old software.
        public bool g_blnEnhanceImage_Enable = false;
        public int g_intEnhanceImage_Open = 1;
        public int g_intEnhanceImage_Dilate = 1;
        public int g_intEnhanceImage_Offset = -50;
        public float g_fEnhanceImage_Gain = 2f;
        public List<float> g_arrCameraShuttle = new List<float>();
        public bool g_blnViewPHImage = false; //PH
        //public bool g_blnSelectedPH = false; //PH
        public uint g_uintPHCameraGain = 20; //PH
        public float g_fPHImageGain = 1f; //PH
        public float g_fPHCameraShuttle = 200f; //PH
        public bool g_blnViewEmptyImage = false; //Empty
        public uint g_uintEmptyCameraGain = 20; //Empty
        public float g_fEmptyImageGain = 1f; //Empty
        public float g_fEmptyCameraShuttle = 200f; //Empty
        public float g_fCameraShuttle = 0;
        public int g_intSelectedImage = 0;
        public int g_intProductionViewImage = 0;
        public int g_intSelectedROI = 0;
        public bool g_blnPixelGrayMaxRangeON = false;
        public int g_fPixelGrayMaxRange = 0;
        public byte g_intThresholdDrawLowValue = 0;
        public byte g_intThresholdDrawMiddleValue = 255;
        public byte g_intThresholdDrawHighValue = 0;
        public List<int> g_arrImageMaskingSetting = new List<int>(); //0: No Filter, 1: Bright Filter, 2: Dark Filter
        public List<float> g_arrImageMaskingGain = new List<float>();
        public List<bool> g_arrImageMaskingAvailable = new List<bool>();
        public int g_intImageMaskingThreshold = 50;
        public int g_intWhiteBackgroundImageIndex = 0;
        public bool g_intViewUniformize = true;

        // Pocket Position Mean Statistic Analysis Variables
        public bool g_bPocketPositionMeanStatisticAnalysisON = false;
        public bool g_bPocketPositionMeanStatisticAnalysisUpdateInfo = false;
        public int g_intPocketPositionMeanStatisticAnalysisCount = 0;

        // Image Statistic Analysis Variables
        public bool g_bImageStatisticAnalysisON = false;
        public bool g_bImageStatisticAnalysisUpdateInfo = false;
        public int g_bStatisticAnalysisImageIndex = 0;

        public ImageDrawing g_objPkgProcessImage = new ImageDrawing();

        public int g_intGrabMode = 0;//0:Normal, 1:Grab All First, 2:High  Speed

        public int g_intUnitsOnImage = 1;
        public int g_intTriggerMode = 2;    // Default Setting: Integration Enable Invert No
        public int g_intLightControllerType = 1; // 1 = Normal light controller, 2 = Sequence light controller
        public int g_intImageMergeType = 0; // 0 = No Merge, 1 = Merge Grab 1 and Grab 2, 2 = Merge All, 3 = Merge Grab 1 & 2, Grab 3 & 4, 4 = Merge Grab 1 & 2 & 3, Grab 4 & 5
        public int g_intImageDisplayMode = 0; // 0 = Standard, 1 = pad, 2 = lead
        public int g_intRotateFlip = 0; // 0 = Orignal, 1 = Rotate 180.

        public bool g_blnViewPosDontCareROI = false;

        public bool g_blnWantNonRotateInspection = false;

        // View       
        public bool g_blnViewAutoTuning = false;
        public bool g_blnClearResult = false;
        public bool[] g_arrblnImageRotated = { true, true, true, true, true, true, true };
        public bool g_blnViewPackageChip = false;
        public bool g_blnViewPackageVoid = false;
        public bool g_blnViewDefect = false;
        public bool g_blnViewDontCareArea = false;
        public bool g_blnViewDoubleThreshold = false;
        public bool g_blnViewRotatedPackageImage = false;
        public bool g_blnViewPackageDefectSetting = false;
        public bool g_blnViewPadPackageDefectSetting = false;
        public bool g_blnViewColorDefectSetting = false;
        public bool g_blnViewLead3DPackageDefectSetting = false;
        public bool g_blnViewCenterPadPositionSetting = false;
        public bool g_blnViewPadEdgeLimitSetting = false;
        public bool g_blnViewPadStandOffSetting = false;
        public bool g_blnViewPadGroupSetting = false;
        public bool g_blnViewLead3DPositionSetting = false;
        public bool g_blnViewPackageImage = false;
        public bool g_blnViewPackageMaskROI = false;
        public bool g_blnViewPackageObjectBuilded = false;
        public bool g_blnViewPHObjectBuilded = false;
        public bool g_blnViewPackageTrainROI = false;
        public bool g_blnViewMoldStartPixelFromEdge = false;
        public bool g_blnViewChipStartPixelFromEdge = false;
        public bool g_blnViewChipStartPixelFromEdge_Dark = false;
        public bool g_blnViewDarkField2StartPixelFromEdge = false;
        public bool g_blnViewDarkField3StartPixelFromEdge = false;
        public bool g_blnViewDarkField4StartPixelFromEdge = false;
        public bool g_blnViewPackageStartPixelFromEdge = false;
        public bool g_blnViewPackageStartPixelFromEdge_Dark = false;
        public bool g_blnViewColorPackageStartPixelFromEdge = false;
        public bool g_blnViewSealSprocketHoleInspectionAreaInwardTolerance = false;
        public bool g_blnViewSealSprocketHoleBrokenOutwardTolerance = false;
        public bool g_blnViewPackageAreaDefect = false;
        public bool g_blnViewPackageMinDefect = false;
        public bool g_blnViewPkgProcessImage = false;
        public bool g_blnViewSealImage = false;

        public bool g_blnViewSearchROI = false;
        public bool g_blnViewSelectedBlobObject = false;
        public bool g_blnViewOrientTrainROI = false;
        public bool g_blnViewPin1TrainROI = false;
        public bool g_blnViewMarkTrainROI = false;
        public bool g_blnViewMark2DCodeROI = false;
        public bool g_blnViewSubROI = false;
        public bool g_blnViewPin1ROI = false;
        public bool g_blnViewUnitROI = false;
        public bool g_blnViewUnitSurfaceROI = false;
        public bool g_blnViewMOGauge = false;

        public bool g_blnViewChipROI = false;
        public bool g_blnViewChipObject = false;

        public bool g_blnViewBarcodeInspectionArea = false;
        public bool g_blnViewBarcodePatternInspectionArea = false;

        public bool g_blnViewROI = false;
        public bool g_blnViewGauge = false;
        public bool g_blnViewRotatedImage = false;
        public bool g_blnViewRotatedImage_AfterMouseUp = false;
        public bool g_blnViewThreshold = false;
        public bool g_blnViewThresholdWithGain = false;
        public bool g_blnViewObjectsBuilded = false;
        public bool g_blnViewCharsBuilded = false;
        public bool g_blnViewTextsBuilded = false;
        // 2021-08-25 : Hide this because no more using Lead base point to offset mark ROI
        //public bool g_blnViewMarkROIOffset = false;
        public bool g_blnViewLeadDontCareInwardTolerance = false;
        public bool g_blnViewMarkInspection = false;
        public bool g_blnViewOrientObject = false;
        public bool g_blnViewOrientSetting = false;
        public bool g_blnViewPadInspection = false;
        public bool g_blnViewSealObjectsBuilded = false;
        public bool g_blnViewUnitPresentObjectsBuilded = false;
        public bool g_blnViewDimension = false;
        public bool g_blnViewGainImage = false;
        public bool g_blnDragROI = false;
        public bool g_blnViewPointGauge = false;
        public string g_strSelectedRectGauge = "";
        public bool g_blnViewLeadInspection = false;
        public bool g_blnViewBarcodeInspection = false;
        public bool g_blnUpdateZoom = false;
        public bool g_blnViewNormalImage = false;
        public string g_strSelectedPage = "";
        public int g_intSelectedSetting = 0;
        public bool g_blnViewOfflinePage = false;
        public bool g_blnSetMultipleImageViewOnOff = false;
        public bool g_blnSetMultipleImageViewOff = false;
        public bool g_blnViewGaugeNotFound = false;
        public bool g_blnInspectionInProgress = false;
        public bool g_blnDrawCompletedAfterInspect = true;
        public int g_intViewInspectionSetting = 0; // 0=No setting view, 1=LineProfileForm View

        public bool g_blncboImageView = true; // decide want to visible combo box for selecting image

        public bool g_blnUseOCRandOCV = false;
        public bool g_blnUseOCR = false;
        public bool g_blnHasDetected = false;
        public bool g_blnHasRecognise = false;

        // Load Image Folder
        public ArrayList g_arrImageFiles = new ArrayList();
        public int g_intFileIndex = 0;
        public bool g_blnLoadFile = false;
        public string g_strLastImageFolder = "";
        public string g_strLastImageName = "";

        // Save Image Path
        public string g_strSaveImageLocation = "";
        public List<string> g_strDeleteImageDate = new List<string>();
        public List<string> g_strDeleteImageFileName = new List<string>();
        public List<List<double>> g_dbFileDataSize = new List<List<double>>();
        public List<double> g_dbFileImageSize = new List<double>();
        public int g_intSaveImageBufferSize = 20;

        // Orient
        public List<List<Orient>> g_arrOrients;     // 1=Unit Index, 2=Template Index
        public List<List<ROI>> g_arrOrientROIs;
        public List<RectGauge> g_arrOrientGauge;
        public List<RectGaugeM4L> g_arrOrientGaugeM4L;
        public int[] g_intOrientResult;
        public bool g_blnWantUseUnitPRFindGauge;
        public bool g_blnOrientWantPackage;

        // Mark
        public List<ROI> g_arrMarkDontCareROIs;
        public List<List<ArrayList>> g_arrOCVs;
        public List<List<ROI>> g_arrMarkROIs;
        public List<RectGauge> g_arrMarkGauge;
        public List<RectGaugeM4L> g_arrMarkGaugeM4L;
        public List<Mark> g_arrMarks;
        public bool g_blnUpdateMarkAdvanceSetting = false;
        public bool g_blnUpdate2DCodeResult = false;
        public bool g_blnWantDontCareArea_Mark = false;
        public bool g_blnWantCheckBarPin1 = false;
        public bool g_blnWantRotateMarkImageUsingPkgAngle = false;
        public bool g_blnSeparateExtraMarkThreshold = false;
        public bool g_blnWantExcessMarkThresholdFollowExtraMarkThreshold = false;
        // 2021-08-25 : Hide this because no more using Lead base point to offset mark ROI
        //public bool g_blnWantUseLeadPointOffsetMarkROI = false;
        public bool g_blnWantRemoveBorderWhenLearnMark = false;
        public bool g_blnWantCheckMarkAngle = false;
        public int g_intExtraExcessMarkInspectionAreaCutMode = 0;//0=No Cut, 1=Cut OCV ROI, 2=Cut Char ROI
        public int g_intCompensateMarkDiffSizeMode = 0;//0=Manual Iteration, 1=Auto Iteration
        public int g_intMarkScoreMode = 1;  // Default Mode is Gradient Only
        public bool g_blnWantCheckMarkBroken = false;
        public bool g_blnWantCheckMarkTotalExcess = false;
        public bool g_blnWantCheckMarkAverageGrayValue = false;
        public bool g_blnWantUseUnitPatternAsMarkPattern = false;
        public float g_fMarkCharROIOffsetX = 0;
        public float g_fMarkCharROIOffsetY = 0;
        public bool g_blnWantUseMarkTypeInspectionSetting = false;
        public bool g_blnUseDefaultMarkScoreAfterClearTemplate = false;
        public List<List<Polygon>> g_arrPolygon_Mark;           //1st Dimension: Unit no, 2nd Dimension: template no
        public int g_intMarkDefectInspectionMethod = 0; // 0 = Classic (Threshold), 1 = Gray Value
        public int g_intMarkTextShiftMethod = 0; // 0 = Max, 1 = Min, 2 = Manual
        public int g_intMarkInspectionAreaGrayValueSensitivity = 45;
        public int g_intMarkMergeSensitivity = 3;
        public int g_intMarkBrightSensitivity = 50;
        public int g_intMarkDarkSensitivity = 30;
        public int g_intFinalReduction_Direction = 2;           // Default Final Reduction 2
        public int g_intFinalReduction_MarkDeg = 0;             // Default Final Reduction 0
        public int g_intRotationInterpolation_Mark = 4;         // Default interpolation 4
        public int g_intRotationInterpolation_PkgBright = 4;    // Default interpolation 4
        public int g_intRotationInterpolation_PkgDark = 4;      // Default interpolation 4
        public bool g_blnWantCheckCharExcessMark = true;
        public bool g_blnWantCheckCharMissingMark = true;
        public bool g_blnWantCheckCharBrokenMark = true;
        public bool g_blnWantCheckLogoExcessMark = true;
        public bool g_blnWantCheckLogoMissingMark = true;
        public bool g_blnWantCheckLogoBrokenMark = true;
        public bool g_blnWantCheckSymbol1ExcessMark = true;
        public bool g_blnWantCheckSymbol1MissingMark = true;
        public bool g_blnWantCheckSymbol1BrokenMark = true;
        public bool g_blnWantCheckSymbol2ExcessMark = true;
        public bool g_blnWantCheckSymbol2MissingMark = true;
        public bool g_blnWantCheckSymbol2BrokenMark = true;
        public bool g_blnWantShowLiterationOnly = false;

        // Pin 1
        public List<Pin1> g_arrPin1;    // Index 0: Unit on image number.

        // Seal
        public int g_intGaugeDisplayIndex = 0; // 0 = Line Gauge; 1 = circle gauge
        public SealBlog g_objSeal;
        public List<List<ROI>> g_arrSealROIs; // 0=locator, 1=seal 1, 2=seal 2, 3=Distance, 4=Overheat, 5[0]=TestROI, 5[1]=PocketROI, 5[2]=MarkROI, 6=Circle Gauge, 7= Seal Edge Straightness
        public List<List<LGauge>> g_arrSealGauges;
        public CirGauge g_objSealCircleGauges;
        public List<ROI> g_arrSealDontCareROIs;
        public List<Polygon> g_arrPolygon_Seal;           //1st Dimension: template no
        public int g_intSealSamplingStep = 5;
        public int g_intLGaugeThreshold = 20;
        public int g_intCPKSample = 100;
        public bool g_blnAdjustROI = false;
        public int g_intPocketTemplateIndex = 0;
        public int g_intPocketTemplateTotal = 0;
        public int g_intPocketTemplateMask = 0;
        public int g_intMarkTemplateIndex = 0;
        public int g_intMarkTemplateTotal = 0;
        public int g_intMarkTemplateMask = 0;
        public bool g_blnViewSealInspection = false;
        public bool g_blnWantDontCareArea_Empty = false;
        public bool g_blnWantClearSealTemplateWhenNewLot = false;
        public bool g_blnViewSprocketHoleDefectThreshold = false;
        public bool g_blnViewSprocketHoleBrokenThreshold = false;
        public bool g_blnViewSprocketHoleRoundnessThreshold = false;

        // Barcode 
        public Barcode g_objBarcode;
        public List<ROI> g_arrBarcodeROIs; // 0: Search ROI, 1: Pattern ROI, 2: Barcode ROI  

        // Color Package
        public List<ColorPackage> g_arrColorPackage;
        public List<List<CROI>> g_arrColorPackageROIs;
        public bool g_blnViewCopperObject = false;
        public bool g_blnViewOxidationObject = false;
        public bool g_blnViewWOBObject = false;
        public bool g_blnViewBurrObject = false;
        public bool g_blnViewLatentScratchObject = false;

        // Package
        public List<ROI> g_arrPackageMoldFlashDontCareROIs;
        public List<List<ROI>> g_arrPackageDontCareROIs; //1st Index : 0=Bright,1=Dark ; 2nd Index: Dont Care ROI Count
        public List<List<List<ROI>>> g_arrPackageColorDontCareROIs; //1st Dimension: Unit No; 2nd Index : Color Threshold Count ; 3rd Index: Dont Care ROI Count
        public List<Package> g_arrPackage;            // Unit index (IPM need this array index for current and next pocket)
        public List<List<ROI>> g_arrPackageROIs;      // 1=Unit Index, 2=Different ROI
        public List<List<CROI>> g_arrPackageColorROIs;      // 1=Unit Index, 2=Different ROI
        public List<RectGauge> g_arrPackageGauge;
        public List<RectGauge> g_arrPackageGauge2;
        public List<RectGaugeM4L> g_arrPackageGaugeM4L;
        public List<RectGaugeM4L> g_arrPackageGauge2M4L;
        public RectGaugeM4L g_objGauge_PkgSize;
        public RectGaugeM4L g_objGauge_PkgSize2;
        public int g_intSelectedBlobNo;
        public int g_intSelectedPackageDefect = -1;
        public int g_intSelectedPackageLengthType = -1;
        public bool g_blnViewPackageTotalAreaDefect = false;
        public float g_fSettingLength;
        public float g_fSettingLength2;
        public bool g_blnCheckPackage = false;
        public bool g_blnWantCheckVoidOnMarkArea = false;
        public bool[] g_blnWantCheckVoidOnMarkArea_SideLight = new bool[3];
        public bool g_blnWantUseSideLightGauge = false;
        public bool g_blnWantUseDetailThreshold_Package = true;
        public bool g_blnWantDontCareArea_Package = false;
        public bool g_blnWantCheckPackageAngle = false;
        public bool g_blnSquareUnit = false;
        public int g_intPackageDefectInspectionMethod = 0; // 0 = Classic (Threshold), 1 = Gray Value
        public int g_intPackageInspectionAreaGrayValueSensitivity = 45;
        public int g_intPackageMergeSensitivity = 3;
        public int g_intPackageBrightSensitivity = 50;
        public int g_intPackageDarkSensitivity = 30;
        public bool g_blnReferTemplateSize = false;
        public List<List<List<Polygon>>> g_arrPolygon_Package;           //1st Dimension: Unit No, 2nd Dimension: Image Bright / Dark, 3rd Dimension: Template No
        public List<List<List<Polygon>>> g_arrPolygon_PackageColor;           //1st Dimension: Unit No, 2nd Dimension: Color Threshold Count, 3rd Dimension: Dont Care ROI Count
        public bool g_blnCheckPackageColor = true;

        // Pad
        public List<List<ROI>> g_arrPadDontCareROIs; //1st Index : ROI Direction (Middle, Top, Right, Bottom, Left) ; 2nd Index: Dont Care ROI Count
        public List<List<List<ROI>>> g_arrPadColorDontCareROIs; //1st Index : ROI Direction (Middle, Top, Right, Bottom, Left) ; 2nd Index: Color Threshold Count; 3rd Index: Dont Care ROI Count
        public List<List<List<ROI>>> g_arrPadPackageDontCareROIs; //1st Index : ROI Direction (Middle, Top, Right, Bottom, Left) ; 2nd Index: Image Bright/Dark ; 3rd Index: Dont Care ROI Count
        public bool g_blnWantUseDetailThreshold_PadPackage = true;
        public List<List<ROI>> g_arrPadROIs;
        public List<List<CROI>> g_arrPadColorROIs;
        public List<ROI> g_arrPadOrientROIs; // 0: Search ROI, 1: Unit Pattern ROI, 2: Orient ROI
        public ROI[] g_arrInspectSearchROI;
        public ROI[] g_arrInspectROI;    // 0=Middle ROI, 1=Top ROI, 2=Right ROI, 3=Bottom ROI, 4=Left ROI
        public ROI[][] g_arrInspectPadROI;  // 1st Index={0=Middle ROI, 1=Top ROI, 2=Right ROI, 3=Bottom ROI, 4=Left ROI}, 2st Index={0=Pad ROI Image 1, 1=Pad ROI Image2, 2=Pad ROI Image3}
        public ROI[][] g_arrInspectPkgROI;  // 1st Index={0=Middle ROI, 1=Top ROI, 2=Right ROI, 3=Bottom ROI, 4=Left ROI}, 2st Index={0=Pkg ROI Image 1, 1=Pkg ROI Image2, 2=Pkg ROI Image3, 3=MoldFlashROI, 4=ChippedOffROI}
        public ROI g_objTopParentROI;
        //public Orient[] g_arrPadOrient;
        public Orient g_objPadOrient;
        public Pad[] g_arrPad;
        public Pad[] g_arrCalibratePad;
        public bool g_blnPadAllowCustomize = true;   // Use for Lead3D also
        public bool g_blnPadInpected = false;   // Use for Lead3D also
        public bool g_blnPadSelecting = false;
        public bool g_blnUpdatePadSetting = false;
        public bool g_blnUpdateSelectedROI = false;
        public bool g_blnDrawing = false;
        public bool g_blnViewPadSettingDrawing = false;
        public int g_intPadSelectedNumber = -1;
        public int g_intSelectedPadROIIndex = 0;
        public int g_intPadDefectSelectedNumber = -1;
        public int g_intPadPkgDefectSelectedNumber = -1;
        public int g_intSelectedROIMask = 0;    //0x01=Center ROI, 0x02=Top ROI, 0x04=Right ROI, 0x08=Bottom ROI, 0x10=Left ROI
        public bool g_blnUsedPreTemplate = false;
        public bool g_blnCheckPad = true;
        public bool g_blnCheckPadColor = true;
        public bool g_blnCheck4Sides = false;
        public bool g_blnWantShowGRR = false;
        public bool g_blnViewPadExtendROI = false;
        public bool g_blnViewPadInspectionAreaSetting = false;
        public bool g_blnViewPadForeignMaterialStartPixelFromEdge = false;
        public bool g_blnViewPadForeignMaterialStartPixelFromEdge_Pad = false;
        public bool g_blnViewPadPkgStartPixelFromEdge = false;
        public bool g_blnViewPadPkgStartPixelFromEdge_Dark = false;
        public bool g_blnViewPadChipStartPixelFromEdge = false;
        public bool g_blnViewPadChipStartPixelFromEdge_Dark = false;
        public bool g_blnViewPadMoldStartPixelFromEdge = false;
        public int g_intSelectedType = 0; // 0 = bright field dont care, 1 = dark field dont care
        public bool g_blnUseSameDontCareForBrightAndDark = false; //Only use at learnPadPackageForm
        public bool g_blnWantDontCareArea_Pad = false;
        public List<List<Polygon>> g_arrPolygon_Pad; //1st Index : ROI Direction (Middle, Top, Right, Bottom, Left) ; 2nd Index: Dont Care ROI Count
        public List<List<List<Polygon>>> g_arrPolygon_PadColor; //1st Index : ROI Direction (Middle, Top, Right, Bottom, Left) ; 2nd Index: Color Threshold Count; 3rd Index: Dont Care ROI Count
        public List<List<List<Polygon>>> g_arrPolygon_PadPackage;              //1st Dimension: Unit no, 2nd Dimension: Image Bright / Dark, 3rd Dimension: Dont Care Count
                                                                               // Lead3D
        public List<List<ROI>> g_arrLead3DDontCareROIs; //1st Index : ROI Direction (Middle, Top, Right, Bottom, Left) ; 2nd Index: Dont Care ROI Count
        public List<List<Polygon>> g_arrPolygon_Lead3D; //1st Index : ROI Direction (Middle, Top, Right, Bottom, Left) ; 2nd Index: Dont Care ROI Count
        public bool g_blnWantDontCareArea_Lead3D = false;
        public bool g_blnViewAllLead3DNumber = false;
        public bool g_blnViewLead3DAGVROIDrawing = false;
        public bool g_blnViewLead3DPkgToBaseDrawing = false;
        public bool g_blnViewLeadTipBuildAreaDrawing = false;
        public bool g_blnWantUseDetailThreshold_LeadPackage = true;
        public ROI[][] g_arrInspectLead3DPkgROI;  // 1st Index={0=Middle ROI, 1=Top ROI, 2=Right ROI, 3=Bottom ROI, 4=Left ROI}, 2nd Index={0=Pkg ROI Image 1, 1=Pkg ROI Image2, 2=Pkg ROI Image3, 3=MoldFlashROI}
        public bool g_blnLead3DInpected = false;
        public bool g_blnLead3DSelecting = false;
        public bool g_blnUpdateLead3DSetting = false;
        public int g_intLead3DPkgDefectSelectedNumber = -1;
        public bool g_blnViewLead3DSettingDrawing = false;
        public int g_intLead3DSelectedNumber = -1;
        public int g_intLead3DDefectSelectedNumber = -1;

        public bool g_blnViewLeadPkgStartPixelFromEdge = false;
        public bool g_blnViewLeadChipStartPixelFromEdge = false;
        public bool g_blnViewLeadMoldStartPixelFromEdge = false;
        public bool g_blnViewLeadTipBasePoint = false;
        // Lead
        public float g_fPreciseAngle = 0;
        public bool g_blnWantShowPocketDontCareArea = false;
        public List<float> g_arrInwardDontCareROIBlobLimit = new List<float>(5) { 0, 0, 0, 0, 0 };
        public int g_intLeadPocketDontCareROIBlobDistanceX = 0;
        public int g_intLeadPocketDontCareROIBlobDistanceY = 0;
        public int g_intLeadPocketDontCareROIBlobMask = 0; //0x01=Top, 0x02=Right, 0x04=Bottom, 0x08=Left
        public int g_intLeadPocketDontCareROIAutoMask = 0; //0x01=Top, 0x02=Right, 0x04=Bottom, 0x08=Left
        public int g_intLeadPocketDontCareROIManualMask = 0; //0x01=Top, 0x02=Right, 0x04=Bottom, 0x08=Left
        public int g_intLeadPocketDontCareROIFixMask = 0; //0x01=Top, 0x02=Right, 0x04=Bottom, 0x08=Left
        public bool g_blnViewAllLeadNumber = false;
        public bool g_blnViewLeadAGVROIDrawing = false;
        public bool g_blnViewLeadPkgToBaseDrawing = false;
        public List<List<ROI>> g_arrLeadROIs;
        public List<List<ROI>> g_arrLeadPocketDontCareROIsBlob; // [0][0] = no use, [1][0] = Top Pocket Dont Care, [2][0] = Right Pocket Dont Care, [3][0] = Bottom Pocket Dont Care, [4][0] = Left Pocket Dont Care
        public List<List<ROI>> g_arrLeadPocketDontCareROIsAuto; // [0][0] = Reference Search ROI, [0][1] = Reference Pattern ROI, [1][0] = Top Pocket Dont Care, [2][0] = Right Pocket Dont Care, [3][0] = Bottom Pocket Dont Care, [4][0] = Left Pocket Dont Care
        public List<List<ROI>> g_arrLeadPocketDontCareROIsManual; // [0][0] = Reference Search ROI, [0][1] = Reference Pattern ROI, [1][0] = Top Pocket Dont Care, [2][0] = Right Pocket Dont Care, [3][0] = Bottom Pocket Dont Care, [4][0] = Left Pocket Dont Care
        public List<List<ROI>> g_arrLeadPocketDontCareROIsFix; // [0][0] = No Use, [1][0] = Top Pocket Dont Care, [2][0] = Right Pocket Dont Care, [3][0] = Bottom Pocket Dont Care, [4][0] = Left Pocket Dont Care
        public Lead[] g_arrLead;
        public Lead3D[] g_arrLead3D;
        public ROI[] g_arrInspectLeadROI;    // 1=Top ROI, 2=Right ROI, 3=Bottom ROI, 4=Left ROI
        public ROI[] g_arrInspectLeadROI_BaseLead;    // 1=Top ROI, 2=Right ROI, 3=Bottom ROI, 4=Left ROI
        public bool g_blnLeadInspected = false;
        public bool g_blnCheckLead = false;
        public bool g_blnWantPocketDontCareAreaFix_Lead = false;
        public bool g_blnWantPocketDontCareAreaManual_Lead = false;
        public bool g_blnWantPocketDontCareAreaAuto_Lead = false;
        public bool g_blnWantPocketDontCareAreaBlob_Lead = false;
        public bool g_blnLeadSelecting = false;
        public int g_intSelectedLeadExtraBlobID = -1;
        public int g_intLeadSelectedNumber = -1;
        public bool g_blnUpdateLeadSetting = false;
        public int g_intPointSelectedNumber = -1;   // For Lead: 0: TipStart, 1: TipCenter, 2: TipEnd, 3: BaseStart, 4: BaseCenter, 5: BaseEnd
        public int g_intPointSelectedNumber_Side = -1;   // For Side Lead: 0: TipStart, 1: TipCenter, 2: TipEnd
        public int g_intPointSelectedNumber_Corner = -1;   // For Corner: 0: Corner1, 1: Corner2
        public bool g_blnSetToAllROIs = false;
        public bool g_blnSetToAllLeadPad = false;
        public bool g_blnSetToAllPoints = false;
        public bool g_blnSetToAllLeadPad_Center = false;
        public bool g_blnSetToAllPoints_Center = false;
        public bool g_blnSetToAllROIs_Side = false;
        public bool g_blnSetToAllLeadPad_Side = false;
        public bool g_blnSetToAllPoints_Side = false;
        public bool g_blnSetToAllROIs_Corner = false;
        public bool g_blnSetToAllPoints_Corner = false;
        // Positioning
        public Position g_objPositioning;
        public Position g_objPositioning2;
        public List<LGauge> g_arrPositioningGauges;
        public List<LGauge> g_arrPositioningGauges2;
        public List<ROI> g_arrPositioningROIs;
        public List<List<Polygon>> g_arrPolygon;           //1st Dimension: Unit Type, 2nd Dimension: Polygon pointer position XY
        public int g_intPositioningSamplingStep = 5;
        public int g_intTemporaryValue = 0;
        public int g_intTemporaryValue2 = 25;
        public List<ROI> g_arrPHROIs;

        // Pocket Position
        public PocketPosition g_objPocketPosition;
        public List<ROI> g_arrPocketPositionROIs; // Index --> 0: Search ROI, 1: Pocket Pattern ROI, 2: Pocket Gauge ROI, 3: Plate Gauge ROI
        public List<LGauge> g_arrPocketPositionGauges; // Index ---> 0: Pocket Gauge, 1: Plate Gauge
        public bool g_blnMeasurePocketPosition = false;
        // Check Presence
        public UnitPresent g_objUnitPresent;

        // Calibrate

        public bool g_blnGlobalSharingCalibrationData = false;

        public Calibration g_objCalibration;
        public CalibrationLead3D g_objCalibrationLead3D;
        public ROI g_objCalibrateROI = new ROI();
        public LGauge[] g_objCalibrateLineGauge = new LGauge[4];
        public ROI[] g_objCalibrateLineROI = new ROI[4];
        public CirGauge g_objCalibrateCircleGauge;
        public float g_fCalibPixelX = 5.0f;
        public float g_fCalibPixelY = 5.0f;
        public float g_fCalibPixelZ = 5.0f;
        public float g_fCalibPixelXInUM = 5.0f;
        public float g_fCalibPixelYInUM = 5.0f;
        public float g_fCalibPixelZInUM = 5.0f;
        public float g_fCalibOffSetX = 0;
        public float g_fCalibOffSetY = 0;
        public float g_fCalibOffSetZ = 0;
        public int g_intCalibrationMode = 2;
        public int g_intCalibrationType = 0;
        public GaugeWorldShape g_WorldShape = new GaugeWorldShape();
        public RectGauge g_objCalibrateRectGauge;

        public int g_intTotalUnits;
        public int g_intTotalGroup;
        public int g_intTotalTemplates;

        public int[] g_intSelectedOcv = { 0, 0 };
        public int[] g_intDisplayOcv = { -1, -1 };
        public int[] g_intMarkCharSelectedNo = { -1, -1 };
        public int[] g_intMarkTextSelectedNo = { -1, -1 };
        public int g_intSelectedGroup = 0;
        public int g_intSelectedUnit = 0;
        public int g_intSelectedDontCareROIIndex = 0;
        public int g_intSelectedColorThresholdIndex = 0;
        public int g_intSelectedMoldFlashDontCareROIIndex = 0;
        public int g_intSelectedTemplate = 0;

        public int g_intHighThresholdValue;
        public int g_intLowThresholdValue;
        public int g_intThresholdValue;
        public float g_fThresholdRelativeValue;
        public float g_fThresholdGainValue = 1f;
        public float g_fGainValue = 1f;
        public int g_intFailMask;
        public int g_intPackageFailMask;
        public int g_intTemplateMask;
        public long g_intTemplatePriority;

        public int g_intSortingMode;
        public float g_fAreaScoreFactor;
        public bool g_blnWantMultiGroups;
        public bool g_blnWhiteOnBlack;
        public bool g_blnBinarizedLocMode;
        public bool g_blnCutMode;
        public bool g_blnWantBuildTexts;
        public bool g_blnWantMultiTemplates;
        public bool g_blnWantSetTemplateBasedOnBinInfo = false;
        public bool g_blnWantRemoveBorderMode;
        public bool g_blnWantGroupExtraMark;
        public bool g_blnWantSet1ToAll;
        public bool g_blnWantRecogPosition;
        public bool g_blnWantSkipMark;
        public bool g_blnWantSubROI;
        public bool g_blnWantPin1;
        public bool g_WantUsePin1OrientationWhenNoMark = false;
        public bool g_blnWantDisplayOrientationInOptionForm = true;
        public bool g_blnWantClearMarkTemplateWhenNewLot;
        public bool g_blnWantCheckNoMark;
        public bool g_blnWantCheckContourOnMark;
        public bool g_blnWantMark2DCode;
        public int g_int2DCodeType;
        public int g_intDefaultMarkScore;
        public int g_intMarkScoreOffset;
        public int g_intMarkOriPositionScore;
        public int g_intCheckMarkAngleMinMaxTolerance;
        public int g_intMinMarkScore;
        public int g_intMaxMarkTemplate;
        public int g_intMaxSealMarkTemplate;
        public int g_intMaxSealEmptyTemplate;
        public PointF[] g_pMarkROIDrawing = new PointF[4]; // 0:Top Left, 1: Top Right, 2: Bottom Left, 3: Bottom Right
        public PointF[] g_pMarkROIDrawing_Lead = new PointF[4]; // 0:Top Left, 1: Top Right, 2: Bottom Left, 3: Bottom Right
        public PointF[] g_pMarkROIDrawing2 = new PointF[4]; // 0:Top Left, 1: Top Right, 2: Bottom Left, 3: Bottom Right
        public PointF g_pLeadDontCareRotateCenter;

        public bool g_blnUpdateMarkTolerance = false;
        public bool g_blnMarkSelecting = false;
        public bool g_blnTemplateManualSelect = false;
        public bool g_blnMarkInspected = false;          // Set to true after production mark inspection done.
        public bool g_blnPackageInspected = false;      // Set to true after produciton package inspection done. 
        public bool[] g_blnUnitInspected = { false, false };
        public bool g_blnMarkDrawing = false;
        public bool g_blnInspectAllTemplate = true;
        
        public int g_intForceZero = 0;
        public int g_intForceYZero = 0;
        public bool g_blnDrawMarkResult = false;
        public bool g_blnDrawPin1Result = false;
        public bool g_blnDrawMark2DCodeResult = false;
        public bool g_blnDrawPkgResult = false;
        public bool g_blnDrawBarcodeResult = false;
        public bool g_blnDrawBarcodeResultDone = true;

        public float g_fMNTestMarkTime = 0;

        // Mark Orient
        public int g_intLearnStepNo = 0;

        // Orient Manual Test
        public float[] g_fOrientScore = new float[2];
        public float[] g_fOrientCenterX = new float[2];
        public float[] g_fOrientCenterY = new float[2];
        public float[] g_fOrientAngle = new float[2];
        public float[] g_fSubOrientCenterX = new float[2];
        public float[] g_fSubOrientCenterY = new float[2];
        public float[] g_fUnitCenterX = new float[2];
        public float[] g_fUnitCenterY = new float[2];

        // Advance Setting
        public bool g_blnWantCheckPH; // Inside Advance setting
        public bool g_blnCheckPH = false; // Inside Vision 3 Offline Test
        public bool g_blnWantCheckEmpty;
        public bool g_blnWantUseEmptyPattern;
        public bool g_blnWantUseEmptyThreshold;
        public bool g_blnWantCheckUnitSitProper;
        public bool g_blnUseAutoRepalceCounter;   // True=Use Single IPM mirror Auto Replace. False=Single or double IPM without auto replace (default)
        public bool g_blnDisableMOGauge;             // True=Use Package Guage to measure unit edge instead of using MO Gauge. 
        public bool g_blnWantGauge = true;                     // True=Use gauge to get unit edge for MarkOrient Test. False=Not using gauge to get unit edge.

        public int g_intForceFailCounter = 0;

        // Pocket Position
        public bool g_blnWantCheckPocketPosition;
        public bool g_blnWantUsePocketPattern;
        public bool g_blnWantUsePocketGauge;
        public bool g_blnDrawPocketPositionResult;

        // Timing
        public HiPerfTimer g_objLightingTime = new HiPerfTimer();
        public HiPerfTimer g_objWaitLightingTime = new HiPerfTimer();
        public HiPerfTimer g_objGrabDoneTime = new HiPerfTimer();
        public HiPerfTimer g_objGrabTime = new HiPerfTimer();
        public HiPerfTimer g_objTransferTime = new HiPerfTimer();
        public HiPerfTimer g_objProcessTime = new HiPerfTimer();
        public HiPerfTimer g_objTotalTime = new HiPerfTimer();

        // Color Guideline
        public bool VM_AT_ColorGuideline = false;

        // Camera Event
        public bool AT_PR_AttachImagetoROI = false;
        public bool ALL_VM_UpdatePictureBox = false;
        public bool VS_VM_UpdateSmallPictureBox = false;
        public bool AT_PR_GrabImage = false;
        public bool MN_PR_GrabImage = false;
        public bool g_blnNoGrabTime = false;
        public bool AT_PR_StartLiveImage = false;
        public bool PR_MN_GrabImageDone = false;
        public bool AT_PR_SaveImage = false;
        public bool AT_PR_TriggerLiveImage = false;
        public bool AT_VM_UpdateHistogram = false;
        public bool g_blnSeparateGrab = false;
        public bool AT_PR_PauseLiveImage = false;

        // Learn Set up
        public bool AT_VM_UpdateResult = false;

        // Production Test Event
        public bool IO_VM_SOV = false;
        public bool VS_AT_UpdateQuantity = false;
        public bool PR_VM_UpdateQuantity = false;
        public bool PR_MN_UpdateInfo = false;
        public bool PR_TL_UpdateInfo = false;
        public bool PR_TL_UpdateInfo2 = false;
        public bool VM_PR_ByPassUnit = false;
        public bool AT_VP_NewLot = false;
        public bool m_blnUpdateResultUsingByPassSetting = false;
        public bool PG_VM_LoadTemplate = false;

        public bool VS_AT_ProductionTestDone = false;

        // LightingEvent
        public bool VM_AT_StrobeLight = false;
        public bool VM_AT_InitConstIntensity = false;
        public bool VM_AT_InitStrobeIntensity = false;
        public bool VM_AT_InitStrobeDuration = false;
        public bool AT_VM_StrobeLightDone = false;

        // Manual test Event
        public bool AT_VM_ManualTestMode = false;       // TRUE: Allow auto inspection when user press the next/previous/load image button.
        public bool AT_VM_OfflineTestForSetting = false;
        public bool g_blnUpdateImageNoComboBox = false;
        public bool MN_PR_StartTest = false;
        public bool MN_PR_StartTest_Verification = false;
        public bool MN_PR_TestUnit1ON = false;
        public bool MN_PR_TestUnit2ON = false;
        public bool MN_PR_CheckEmptyUnit = false;       // Use in Auto Repalce InPocket With Position Index.
        public bool MN_PR_CheckPosition = false;        // Use in Auto Repalce InPocket With Position Index.
        public bool PR_MN_TestDone = true;
        public bool VM_AT_BlockImageUpdate = false;     // TRUE: Block image update (Next/Previous Image button) when inspection is in progress
        public bool PR_MN_UpdateSettingInfo = false;
        public bool AT_VM_OfflineTestAllPad = false;       // TRUE: Allow other Pad ROI to Inspect when one of the Pad ROI fail ---> trigger when line profile want start offline test
        public bool AT_VM_OfflineTestAllLead3D = false;       // TRUE: Allow other Lead ROI to Inspect when one of the Lead ROI fail ---> trigger when Lead3D line profile want start offline test
        public bool AT_VM_OfflineTestAllLead = false;       // TRUE: Allow other Lead ROI to Inspect when one of the Lead ROI fail ---> trigger when Lead line profile want start offline test

        // Auto Form
        public bool AT_VM_UpdateProduction = false;
        public bool AT_VM_UpdateProductionDisplayAll = false;
        public bool VM_AT_DisableImageLoad = false;
        public bool VM_AT_SettingInDialog = false;
        public bool VM_AT_OfflinePageView = false;
        public bool VM_AT_TemplateNotLearn = false;
        public bool VM_AT_UpdateErrorMessage = false;
        public bool VM_AT_ReloadRecipe = false;
        public bool AT_VM_ReloadRecipe = false;
        public bool g_blnUpdateContextMenu = false;
        public bool g_blnEnableMarkContextMenu = true;
        public string g_strContextMenuType = "Production";
        public List<string> g_arrEditedDir = new List<string>();

        // save pass fail image
        public bool g_blnWantClearSaveImageInfo = false;

        // In Pocket tracking counter
        public bool g_blnPocket1Pass = false;
        public bool g_blnPocket2Pass = false;
        public int g_intScenario = 0x01;
        public bool g_blnInPocketRetestEnd = false;
        public bool g_blnInPocketLastUnit = false;

        // Communication Event       
        public bool CO_PR_LoadRecipe = false;
        public bool CO_PR_DeleteTemplate = false;
        public bool PR_CO_DeleteProcessSuccess = true;
        public bool PR_CO_DeleteTemplateDone = false;
        public int PR_CO_PassQuantity = 0;

        public bool g_blnDebugRUN = false;
        public bool g_blnCenterDebugRUN = false;
        public int g_intSleep = 1;

        public object m_objInspectLock = new object();
        public object m_objGrabImageLock = new object();

        //System
        public List<ROI> g_arrSystemROI; //0=Middle ROI, 1=Top ROI, 2=Left ROI, 3=Bottom ROI, 4=Right ROI

        //Option
        public long g_intOptionControlMask = 0;
        public int g_intOptionControlMask2 = 0; // 2020-10-19 ZJYEOH : use for center color pad, side Top color pad and color package
        public int g_intOptionControlMask3 = 0; // 2021-07-07 ZJYEOH : use for side Right color pad
        public int g_intOptionControlMask4 = 0; // 2021-07-07 ZJYEOH : use for side Bottom color pad
        public int g_intOptionControlMask5 = 0; // 2021-07-07 ZJYEOH : use for side Left color pad
        public long g_intPkgOptionControlMask = 0;
        public int g_intPkgOptionControlMask2 = 0;
        public int g_intLeadOptionControlMask = 0;

        //SECSGEM
        public bool g_blnUpdateSECSGEMFile = false;

        //public bool g_blnTrackInspectionTiming = false;
        public bool g_blnTrackBasic = false;
        public bool g_blnTrackCenter = false;
        public bool g_blnTrackTL = false;
        public bool g_blnTrackBR = false;
        public bool g_blnTrackPocketPosition = false;
        public bool g_blnTrackPocketCounter = false;
        public bool g_blnTrackIPMEdgeROI = false;
        public bool g_blnTrackSealImage = false;
        public bool g_blnTrackSealOption = false;
        public bool g_blnTrackBarcodeOption = false;
        public bool g_blnSaveImageAfterGrab = false;
        public bool g_blnTrackIO = false;
        public bool g_blnTrackSaveImageFile = false;
        public bool g_blnTrackSavePassLastImage = false;
        public bool g_blnTrackTiming = false;
        public float g_fUnitPRScoreLimit = 0.5f;
        public string g_strTrackPad = "";
        public string g_strTrackTiming = "";
        public HiPerfTimer m_tTrackTiming = new HiPerfTimer();
        public HiPerfTimer[] m_arrTrackTiming = new HiPerfTimer[4];

        //Pre Test
        public List<bool> g_arrPreTestInspect = new List<bool>(new bool[20]);
        public List<int> g_arrPreTestExpectedResult = new List<int>(new int[20]); //0=Pass, 1=Fail
        public List<bool> g_arrPreTestResult = new List<bool>(new bool[20]);
        public List<string> g_arrPreTestErrorMessage = new List<string>(new string[20]);
        public List<Color> g_arrPreTestErrorMessageColor = new List<Color>(new Color[20]);

        //ROI Color

        // Mark Orient Index 1: [0]=Search ROI, [1]=ReTest ROI, [2]=Mark ROI, [3]=Orient ROI, [4]=Unit ROI, [5]=Pin1 ROI, [6]=Dont Care ROI, [7]=Gauge ROI, [8]=Char ROI, [9]=Mark 2D ROI
        // Mark Orient Index 2: [8][0]=Inspect Char ROI, [8][1]=Selected Char ROI, [8][2]=Ignored Char ROI, [8][3]=Filtered Char ROI , [8][4]=Char Split Line 
        public Color[][] g_arrMarkOrientROIColor = new Color[][]{ new Color[] { Color.Lime },new Color[] { Color.GreenYellow }, new Color[] { Color.Cyan }, new Color[] { Color.Yellow }, new Color[] { Color.Lime }, new Color[] { Color.Lime },
                                                                  new Color[] {Color.Red },new Color[] { Color.Lime }, new Color[] {Color.Lime, Color.Yellow, Color.Purple, Color.Red, Color.Cyan },
                                                                  new Color[] {Color.Yellow} };

        // Package Index 1: [0]=Package ROI, [1]=Crack ROI, [2]=Chipped Off ROI, [3]=Mold Flash ROI, [4]=Dont Care ROI, [5]=Gauge ROI
        // Package Index 2: [0][0]=Package Bright ROI, [0][1]=Package Dark Defect ROI, [0][1]=Package Dark 2 Defect ROI
        // Package Index 2: [2][0]=Chipped Bright ROI, [2][1]=Chipped Dark ROI 
        // Package Index 2: [4][0]=Package Dont Care ROI, [4][1]=Mold Flash Dont Care ROI 
        // Package Index 2: [5][0]=Side Light Gauge ROI, [5][1]=Top Light Gauge ROI 
        public Color[][] g_arrPackageROIColor = new Color[][]{ new Color[] { Color.Cyan, Color.Cyan, Color.Cyan }, new Color[] { Color.Cyan }, new Color[] { Color.Cyan, Color.Cyan },
                                                            new Color[] { Color.Cyan }, new Color[] { Color.Red, Color.Red }, new Color[] { Color.Lime, Color.Lime }};

        // Seal Index 1: [0]=Position ROI, [1]=Seal Line ROI, [2]=Distance ROI, [3]=Sproket Hole ROI, [4]=OverHeat ROI, [5]=Test ROI, [6]=Empty ROI, [7]=Mark ROI, [8]=Seal Edge Straightness ROI
        // Seal Index 2: [0][0]=Position Search ROI, [0][1]=Position Pattern ROI
        // Seal Index 2: [3][0]=Circle Gauge ROI, [3][1]=Circle Gauge Result, [3][2]=Defect ROI Inward, [3][3]=Broken ROI Outward Outer, [3][4]=Broken ROI Outward Inner
        // Seal Index 2: [6][0]=Empty  ROI, [6][1]=Empty Dont Care ROI
        public Color[][] g_arrSealROIColor = new Color[][]{ new Color[] { Color.Lime, Color.Yellow }, new Color[] { Color.Lime }, new Color[] { Color.Lime },
                                                            new Color[] { Color.Lime, Color.Lime, Color.Cyan, Color.Cyan, Color.Cyan }, new Color[] { Color.Lime },
                                                            new Color[] { Color.Lime }, new Color[] { Color.Yellow, Color.Red }, new Color[] { Color.Yellow }, new Color[] { Color.Lime }};

        // Barcode Index 1: [0]=Search ROI, [1]=Pattern ROI, [2]=Barcode ROI, [3]=Barcode Area Tolerance, [4]=Pattern Area Tolerance
        public Color[][] g_arrBarcodeROIColor = new Color[][] { new Color[] { Color.Lime }, new Color[] { Color.Yellow }, new Color[] { Color.Lime }, new Color[] { Color.Cyan }, new Color[] { Color.Yellow } };

        // Pad Index 1: [0]=Search ROI, [1]=Pattern ROI, [2]=Gauge ROI, [3]=Pad ROI Tolerance, [4]=Individual Pad, [5]=Individual Pad Inspection Area ROI, [6]=Pin 1 ROI, [7]=PH ROI,
        //              [8]=Color ROI Tolerance, [9]=Pitch Gap Link, [10]=Dont Care ROI, [11]=Unit ROI, [12]=Orient ROI, [13]=Edge Limit Setting, [14]=Stand Off Setting, [15]=Position Setting
        // Pad Index 2: [4][0]=Individual Pad(Inspect), [4][1]=Individual Pad(Disable), [4][2]=Individual Pad(Selected), [4][3]=Individual Pad(label)
        // Pad Index 2: [10][0]=Pad Dont Care ROI, [10][1]=Color Dont Care ROI
        public Color[][] g_arrPadROIColor = new Color[][] { new Color[] { Color.Lime }, new Color[] { Color.Red }, new Color[] { Color.Lime }, new Color[] { Color.Yellow }, new Color[] { Color.Lime, Color.Purple, Color.Yellow, Color.Red }, new Color[] { Color.Cyan },
                                                            new Color[] { Color.Lime }, new Color[] { Color.Lime }, new Color[] { Color.Cyan }, new Color[] { Color.Red }, new Color[] { Color.Red, Color.Red },
                                                            new Color[] { Color.Lime }, new Color[] { Color.Yellow }, new Color[] { Color.Cyan }, new Color[] { Color.Cyan }, new Color[] { Color.Yellow }};

        // PadPackage Index 1: [0]=Package ROI, [1]=Crack ROI, [2]=Chipped Off ROI, [3]=Mold Flash ROI, [4]=Dont Care ROI, [5]=Foreign Material ROI
        // PadPackage Index 2: [0][0]=Package Bright ROI, [0][1]=Package Dark Defect ROI
        // PadPackage Index 2: [2][0]=Chipped Bright ROI, [2][1]=Chipped Dark ROI 
        public Color[][] g_arrPadPackageROIColor = new Color[][]{ new Color[] { Color.Cyan, Color.Cyan }, new Color[] { Color.Cyan }, new Color[] { Color.Cyan, Color.Cyan },
                                                            new Color[] { Color.Cyan }, new Color[] { Color.Red }, new Color[] { Color.Cyan }};

        // Lead3D Index 1: [0]=Search ROI, [1]=Package To Base Tolerance, [2]=Pitch Gap Link, [3]=Dont Care ROI, [4]=Average Gray Value ROI Tolerance, [5]=Pin 1 ROI, [6]=PH ROI, [7]=Package ROI, [8]=Position Setting, [9]=Tip Build Area ROI TOlerance
        public Color[][] g_arrLead3DROIColor = new Color[][] { new Color[] { Color.Lime }, new Color[] { Color.Yellow }, new Color[] { Color.Red }, new Color[] { Color.Red }, new Color[] { Color.Cyan }, new Color[] { Color.Lime }, new Color[] { Color.Lime }, new Color[] { Color.Yellow }, new Color[] { Color.Cyan }, new Color[] { Color.Lime } };

        // Lead Index 1: [0]=Search ROI, [1]=Lead ROI, [2]=Pitch Gap Link, [3]=Average Gray Value ROI Tolerance, [4]=Gauge ROI, [5]=Package To Base Tolerance, [6]=Dont Care Auto, [7]=Dont Care Manual
        // Lead Index 2: [6][0]=Reference Search ROI, [6][1]=Reference Pattern ROI, [6][2]= Search Area ROI
        // Lead Index 2: [7][0]=Reference Search ROI, [7][1]=Reference Pattern ROI, [7][2]=Dont Care Area ROI
        public Color[][] g_arrLeadROIColor = new Color[][] { new Color[] { Color.Lime }, new Color[] { Color.Lime }, new Color[] { Color.Red }, new Color[] { Color.Cyan }, new Color[] { Color.Lime }, new Color[] { Color.Yellow },
                                                             new Color[] { Color.Lime, Color.Yellow, Color.Lime}, new Color[] { Color.Lime, Color.Yellow, Color.Lime} };

        // Empty Index 1: [0]=Empty ROI
        public Color[][] g_arrEmptyROIColor = new Color[][] { new Color[] { Color.Yellow } };

        // Pocket Position Index 1: [0]=Search ROI, [1]=Pattern ROI, [2]=Pocket Gauge ROI, [3]=Plate Gauge ROI
        public Color[][] g_arrPocketPositionROIColor = new Color[][] { new Color[] { Color.Lime }, new Color[] { Color.Yellow }, new Color[] { Color.Lime }, new Color[] { Color.Pink } };

    }
}
 