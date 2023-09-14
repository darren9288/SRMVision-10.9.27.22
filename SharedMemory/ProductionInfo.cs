using System;
using System.Collections.Generic;
using System.Text;
using Common;
namespace SharedMemory
{
    public class ProductionInfo
    {
        // Lot Information shared among vision(s)        
        public string g_strLotID = "SRM";
        public string[] g_arrSingleLotID = new string[10];
        public bool[] g_arrIsNewLot = new bool[10];
        public string g_strLotStartTime = DateTime.Now.ToString("yyyyMMddhhmmss");
        public string g_strOperatorID = "Op";
        public string g_strRecipeID = "Default";
        public string[] g_arrSingleRecipeID = new string[10];
        public string g_strRecipePath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\";

        public bool[] g_blnRecipeImported = new bool[10] { false, false, false, false, false, false, false, false, false, false };
        public int g_intUserGroup = 5;
        public bool m_blnTrackON = false;

        // Mouse coordinate
        public int g_intMousePositonX;
        public int g_intMousePositonY;
        public int g_intMousePixel;
        public int[] g_arrMouseRGBPixel = new int[3]; // 0: Red , 1: Green , 2: Blue

        // View
        public bool g_blnDisplayColorPixelInfo = false;
        public bool g_blnViewCrosshair = false;
        public bool g_blnViewSearchROI = false;
        public bool g_blnViewROITool = false;
        public bool g_blnViewROIDetails = false;
        public bool g_blnViewInspection = false;
        public bool g_blnViewPadResult = false;
        public bool g_blnViewPackageResult = false;
        public bool g_blnViewPackageTolerance = false;
        public bool g_blnViewPocketPositionResult = false;
        public bool g_blnViewPadInspectionArea = false;
        public bool g_blnViewBarcodeInspectionArea = false;
        public bool g_blnViewBarcodePatternInspectionArea = false;
        public bool g_blnViewPosCrosshair = false;
        public bool g_blnViewMarkObject = false;
        public bool g_blnViewPadObject = false;
        public bool g_blnViewPositionXY = false;
        public bool g_blnViewGrayPixel = false;
        public bool g_blnView5SRuler = false;
        public int g_intViewMask = 0;
        public bool g_blnViewMultipleImageOff = false;
        public bool g_blnViewPadPackageToleranceROI = false;
        public bool g_blnViewPadROIToleranceROI = false;
        public bool g_blnViewMarkROI = false;
        public bool g_blnViewLeadDontCareROI = false;

        // Display Vision Module
        public int[] g_arrDisplayVisionModule = new int[8];
        public bool g_blnInBarCodePage = false;
        public bool g_blnInDisplayAllPage = false;

        // RS2322
        public string g_strSendMessage = "";

        public bool AT_ALL_InAuto = false;
              
        public bool VM_TH_UpdateCount = false;
        public bool GO_AT_UpdateOutput = false;
        public bool AT_ALL_MessageSendSuccess = false;
        public bool AT_ALL_MessageSendFail = false;
        public bool AT_ALL_ConnectionBroken = false;
        public bool VM_TH_SaveData = false;

        // test
        public bool VM_AT_StopInspect = false;

        // Start Up
        public bool AT_SF_CheckingDongle = false;
        public bool AT_SF_CheckingCamera = false;
        public bool AT_SF_ValidatingCamera = false;
        public bool AT_SF_Initializing = false;
        public bool AT_SF_ScanningIO = false;
        public bool AT_SF_LoadInterface = false;
        public bool PR_AT_StopProduction = false;

        // Communication TCP/IP
        public int g_intVisionInvolved = 0;
        public int[] g_arrVisionIDIndex = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };// { 0, 1, 3, 5, 0, 0, 0, 0, 0, 0 };
        public bool CO_AT_NewLot = false;
        public bool CO_AT_EndLot = false;
        public bool CO_AT_RerunLot = false;
        public bool CO_AT_LoadRecipe = false;
        public bool CO_AT_ResetCount = false;
        public bool CO_AT_Get2DCode = false;
        public bool AT_CO_NewLotDone = true;
        public bool AT_CO_EndLotDone = true;
        public bool AT_CO_RerunLotDone = true;
        public bool AT_CO_LoadRecipeDone = true;
        public bool AT_CO_ResetCountDone = true;
        public bool AT_CO_DeleteTemplateDone = true;
        public bool AT_CO_Get2DCodeDone = true;
        public bool g_blnResetPass = true;
        public bool g_blnNewLotPass = true;
        public bool g_blnEndLotPass = true;
        public bool g_blnRerunLotPass = true;
        public bool g_blnLoadRecipePass = true;
        public bool g_bln2DCodePass = true;
        public string AT_CO_DataMatrixCode = "";
        public bool AT_CO_Send2DCode = false;

        public int g_intDebugImageToUse = 0; // 0: Use Current Image, 1: Use Multipe Images
        public bool g_blnAllRunWithoutGrabImage = false;    // No grab image for every inspection.
        public bool g_blnAllRunGrabWithoutUseImage = false; // Grab image, but copy to arrDebugImage. Inspection will use the manual loaded image.
        public bool g_blnAllRunFromCenter = false;
        public bool g_blnMultiThreadingON = true;
        public bool g_blnSaveRecipeToServer = false;

        public bool g_blnRefreshingON = false;
        public bool g_blnLoadSRMVisionForRefreshing = false;
        public bool g_blnEndLotStatus = false;

        public List<string> g_arrThreadInfo = new List<string>();

        public HiPerfTimer g_Timer = new HiPerfTimer();
        public bool g_Test1Done = false;
        public bool g_Test2Done = false;
        public bool g_StartTest = false;

        public bool g_blnSECSGEMSInit = false;

        public string g_strHistoryDataLocation = "D:\\";

        public bool g_blnWantEditLog = true;

        public DateTime g_DTStartAutoMode;
        public DateTime g_DTStartAutoMode_IndividualForm;
        public int g_intAutoLogOutMinutes = 5;
        public bool g_blnAutoLogOutBLock = false;
        public bool g_blnWantBuypassIPM = false;
        public bool g_blnWantNonRotateInspection = false;
        public bool g_blnShowGUIForBelowUserRight = true;   // Show GUI by default.
        public bool g_blnWantRecipeVerification = false;
        public bool g_blnWantShowWhiteBalance = false;
        public bool g_blnBlankImageBeforeGrab = false;

        public bool g_blnTrackDeleteImage = false;
        public bool g_blnTrackTCPIP_IO = false;

        public bool g_blnDeleteImageDuringProduction = false;

    }
}
