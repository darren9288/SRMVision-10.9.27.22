using System;
using System.Collections.Generic;
using System.Text;
using Common;
namespace SharedMemory
{
    public class CustomOption
    {
        // Customize Vision
        public int g_intVisionMask;
        public int g_intWantOrient;
        public int g_intWantOrient0Deg; // For Want Mark Inspection but dont want orient result
        public int g_intWantMark;
        public int g_intWantLead;
        public int g_intWantLead3D;
        public int g_intWantPackage;
        public int g_intWantPad;
        public int g_intWantPad5S;
        public int g_intWant3DLead;
        public int g_intWantSideLead;
        public int g_intWantSeal;
        public int g_intWantBarcode;
        public int g_intWantOCR;
        public int g_intWantOCR2;
        public int g_intWantBottom;
        public int g_intUseColorCamera;
        public int g_intWantPositioning;
        public int g_intWantPositioningIndex;   // Want when the module need the position value for motor index (e.g. Auto Replace InPocket With Motor Index)
        public int g_intWantRotatorSignal;
        public int g_intWantCheckPresent;
        public int g_intWant2DCode;

        // Customize Option
        public bool g_blnWantUseTCPIPIO;
        public bool g_blnAutoShutDown;
        public bool g_blnAutoLogout;
        public bool g_blnLEDiControl;
        public bool g_blnVTControl;
        public bool g_blnShareHandlerPC;
        public bool g_blnShowSubLearnButton;
        public bool g_blnConfigShowTCPIC;
        public bool g_blnConfigShowRS232;
        public bool g_blnConfigShowNetwork;
        public bool g_blnDebugMode;
        public bool g_blnPreviousVersion;
        public bool g_blnUseUSBIOCard;
        public int g_intPreviousVersionIndex = 0;
        public bool g_blnSavePassImage;
        public bool g_blnSaveFailImage;
        public bool g_blnSaveFailImageErrorMessage;
        public bool g_blnStopLowYield;
        public bool g_blnGlobalSharingCalibrationData;
        public bool g_blnGlobalSharingCameraData;
        public bool g_blnMixController;
        public int g_intMarkDefaultTolerance;
        public int g_intResolutionHeight;
        public int g_intResolutionWidth;
        public int g_intEmptyPocketTole;
        public int g_intWrongFaceTole;
        public int g_intPassImagePics;
        public int g_intFailImagePics;
        public int g_intSaveImageMode;
        public int g_intMinUnitCheck;
        public int g_intUnitDisplay;                   // 1=mm(default) , 2=mil, 3=micron
        public int g_intMarkUnitDisplay;                   // 0=pixel(default) , 1=mm, 2=mil, 3=micron
        public float g_fLowYield;
        public bool g_blnOnlyNewLot;
        public int g_intOrientIO; // 0 = Clockwise, 1 = Anti-Clockwise

        public string g_strMachineID;
        public string g_strSaveImageLocation;
        public int g_intLanguageCulture;

        public int g_intSelectedVision = 0;

        public NewUserRight objNewUserRight;

        //TCPIP
        public bool g_blnWantExportReport;
        public string g_strExportReportIPAddress;
        public string g_strExportReportDir;
        public int g_intExportReportFormat;
      
        //Network
        public bool g_blnWantNetwork;
        public bool g_blnNetworkPasswordLimit;
        public string g_strHostIP;
        public string g_strNetworkUsername;
        public string g_strNetworkPassword;
        public string g_strDeviceUploadDir;
        public string g_strVisionLotReportUploadDir;

        //SECSGEM
        public bool g_blnWantSECSGEM;
        public string g_strSECSGEMSharedFolderPath;
        public int g_intSECSGEMMaxNoOfCoplanPad;
    }
}
