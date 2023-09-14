using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using Microsoft.Win32;
using Common;
using System.Threading;
using VTLightMnDLL;

namespace Lighting
{
    public class VT_Control
    {
        #region enum

        private enum E_VTL_Model
        {
            VTL_MODEL_L2402_U0 = (0x301),
            VTL_MODEL_L2A05_U0 = (0x302),
            VTL_MODEL_L2E07_U0 = (0x303),
            VTL_MODEL_L3K28_U0 = (0x031),
            VTL_MODEL_UNKNOWN = (0x00),
        }

        private enum E_LightMode
        {
            VTL_MODE_CONTINUOUS = (0x01),
            VTL_MODE_SEQUENCE = (0x02),
            VTL_MODE_PULSEWIDTH = (0x03),
        }

        private enum E_OperationMode
        {
            VTL_OPMODE_RUN = 1,
            VTL_OPMODE_CONFIG = 2,
        }

        private enum E_BoardStatus
        {
            VTSTATUS_ERROR = (-1),
            VTSTATUS_AVAILABLE = (0),
            VTSTATUS_CONNECTED = (1),
            VTSTATUS_NOT_ACCESSIBLE = (2),
            VTSTATUS_UNKNOWN_DEV = (3),
            VTSTATUS_CONNECTED_BID_ERROR = (4),
            VTSTATUS_AVAILABLE_BID_DUPLICATE = (5),
            VTSTATUS_CONNECTED_OTHER_APP = (6),
        }

        private enum vtErrorCode
        {
            VT_SUCCESS = 0x00,
            VERR_TIMEOUT = (-2),   //Function response time out
            VERR_NO_BOARD = (-3),   //No board detected
            VERR_INVALID_PARAM = (-4),   //Invalid parameter been passing to function
            VERR_BOARD_ID_OVERFLOW = (-8),   //Exceed Max Board ID
            VERR_API_NOT_AVAILABLE = (-12),  //API cannot operate at this moment
            VERR_API_NOT_SUPPORTED = (-16),  //API is not supported for this board
            VERR_COM_FAILED = (-17),    //Serial Communication Failure
            VERR_SYSTEM_NOT_INITIALED = (-20),   //System not yet initialize
            VERR_BOARD_NOT_CONNECTED = (-21),   //Target board is not connected
            VERR_BOARD_CONNECTED = (-22),   //Target board that attempt to connect was already connected 
            VERR_DATA_INVALID = (-30),   //Wrong format of data receive from board
            VERR_SYSTEM_PROGRAM = (-32),    //Error message from board
            VERR_OVER_PRODUCT_SPEC = (-33), //Setting beyond the specification

        }

        #endregion

        #region Member Variables

        private static RegistryKey Key = Registry.LocalMachine.OpenSubKey(@"Software\SVG\LightControl", true);
        private static object m_objLock = new object();
        private static CVTLightControllerManagerMn m_ControllerManager = new CVTLightControllerManagerMn();
        private static List<CVTLightControllerMn> m_arrController = new List<CVTLightControllerMn>();
        private static STLightCtrlInfoMn[] m_arrStcCtrlStatus;
        private static STLightChannelParamG1Mn[] m_arrStcChannelParam;
        private static List<bool> m_arrControllerConnected = new List<bool>();
        //private static List<bool[]> ArrBlnInitLoad = new List<bool[]>();
        private static uint uintSelectedGroupNum = 0;
        private static int[] m_intOperationMode;
        private static int[] intSelectedLightMode;

        // definition of product specifications
        private const int MIN_TRIG_VALUE = 1;
        private const int MAX_TRIG_VALUE = 100000;
        private const int MIN_OUTPUT_VALUE = 1;
        private const int MAX_OUPUT_VALUE = 100000;
        private const int MAX_INTENSITY_SCALE = 13107;
        private const double MAX_INTENSITY_CURRENT = 2.0;


        #endregion

        #region Properties
        public static int ref_intComNo { get { return m_arrController.Count; } }
        #endregion

        /// <summary>
        /// Initialize light source and open all light source
        /// </summary>
        public static void Init()
        {
            lock (m_objLock)
            {
                TrackLog objTrack = new TrackLog();
                objTrack.WriteLine("VT Init.");

                int intTotalBoard = 0;
                int intError = 0;
                int intCtrlCount = 0;
                STLightCtrlInfoMn[] arrStcCtrlStatusTemp;

                //---------- Check controller in registry ----------//
                checkController();

                //---------- Initial scan to get total number of boards connected ----------//
                intError = m_ControllerManager.InitScan(ref intTotalBoard);

                if (intError == (int)vtErrorCode.VT_SUCCESS)
                {
                    objTrack.WriteLine("VT scan board success.");
                    if (intTotalBoard > 0)
                    {
                        arrStcCtrlStatusTemp = new STLightCtrlInfoMn[intTotalBoard];
                        m_arrStcCtrlStatus = new STLightCtrlInfoMn[intTotalBoard];
                        m_intOperationMode = new int[intTotalBoard];
                        intSelectedLightMode = new int[intTotalBoard];

                        //---------- Get controller information i.e. COM port, model ID, status etc. ----------//
                        intError = m_ControllerManager.GetControllerInfo(ref arrStcCtrlStatusTemp);

                        if (intError == (int)vtErrorCode.VT_SUCCESS)
                        {
                            objTrack.WriteLine("VT get controller info success.");
                            //---------- Get Com port name from registry ----------//
                            Key = Registry.LocalMachine.CreateSubKey(@"Software\SVG\LightControl");

                            string[] arrName = Key.GetSubKeyNames();
                            foreach (string strName in arrName)
                            {
                                RegistryKey SubKey = Key.OpenSubKey(strName);
                                for (int i = 0; i < intTotalBoard; i++)
                                {
                                    if ("COM" + arrStcCtrlStatusTemp[i].iCom.ToString() == strName.ToUpper())
                                    {
                                        //---------- Check the connect status of the controller ----------//
                                        int intConnectStatus = arrStcCtrlStatusTemp[i].iStatus;
                                        if (intConnectStatus == (int)E_BoardStatus.VTSTATUS_AVAILABLE)
                                        {
                                            objTrack.WriteLine("VT controller status is available.");

                                            m_arrStcCtrlStatus[intCtrlCount] = arrStcCtrlStatusTemp[i];
                                            m_arrController.Add(new CVTLightControllerMn());
                                            //m_arrControllerConnected.Add(false);
                                            //---------- Connect to the controller ----------//
                                            intError = m_arrController[intCtrlCount].Connect(m_arrStcCtrlStatus[intCtrlCount]);

                                            if (intError != (int)vtErrorCode.VT_SUCCESS)
                                            {
                                                m_arrControllerConnected[intCtrlCount] = false;
                                                objTrack.WriteLine("Failed to connect to selected controller! Err: " + intError.ToString());
                                                continue;
                                            }
                                            else
                                            {
                                                m_arrControllerConnected[intCtrlCount] = true;
                                                objTrack.WriteLine("Connect to controller success.");
                                                SetInitValueForLightController(intCtrlCount);
                                                intCtrlCount++;
                                            }
                                        }
                                        else
                                        {
                                            //m_arrControllerConnected.Add(false);
                                            objTrack.WriteLine("Controller is currently in use.");
                                        }
                                    }
                                    else
                                    {
                                        //m_arrControllerConnected.Add(false);
                                    }
                                }
                            }
                        }
                        else
                        {
                            //m_arrControllerConnected.Add(false);
                            objTrack.WriteLine("Failed to detect controllers! Error: " + intError.ToString());
                        }
                    }
                    else
                    {
                        //m_arrControllerConnected.Add(false);
                        objTrack.WriteLine("No controllers found!");
                    }
                }
                else
                {
                    //Fail to scan controller
                    //m_arrControllerConnected.Add(false);
                    objTrack.WriteLine("Failed to scan controllers! Error: " + intError.ToString());
                }
            }
        }

        private static void checkController()
        {
            Key = Registry.LocalMachine.CreateSubKey(@"Software\SVG\LightControl");

            string[] arrName = Key.GetSubKeyNames();
            foreach (string strName in arrName)
            {
                m_arrControllerConnected.Add(false);
            }
        }

        /// <summary>
        /// Set Default value for light source controller
        /// </summary>
        private static void SetInitValueForLightController(int intSelectedCtrl)
        {
            if (m_arrStcCtrlStatus[intSelectedCtrl].iModelID == (int)E_VTL_Model.VTL_MODEL_L3K28_U0)
            {
                SetConfigToBoard(intSelectedCtrl);
            }
            else
            {
                //Other types of controller
            }
        }


        /// <summary>
        /// Initialize light source and open all light source
        /// </summary>
        private static void SetConfigToBoard(int intSelectedCtrl)
        {
            SetControllerOperationMode(intSelectedCtrl, 2);
            ResetGroupsAvailable(intSelectedCtrl);
            SetMaximumCurrent(intSelectedCtrl);

        }

        private static void SetControllerOperationMode(int intSelectedCtrl, int intMode)
        {
            int intOperationMode = 0;
            int intError = 0;

            //-------------------- Get operation mode --------------------//
            // iOperationMode: 1 - Run mode; 2 - Configuration mode
            intError = m_arrController[intSelectedCtrl].GetOperationMode(ref intOperationMode);

            if (intError == (int)vtErrorCode.VT_SUCCESS)
            {
                if (intMode == 1)
                {
                    if (intOperationMode == (int)E_OperationMode.VTL_OPMODE_CONFIG)
                    {
                        m_arrController[intSelectedCtrl].SetOperationMode((int)E_OperationMode.VTL_OPMODE_RUN);
                        m_intOperationMode[intSelectedCtrl] = (int)E_OperationMode.VTL_OPMODE_RUN;
                    }
                }
                else if(intMode == 2)
                {
                    if (intOperationMode == (int)E_OperationMode.VTL_OPMODE_RUN)
                    {
                        m_arrController[intSelectedCtrl].SetOperationMode((int)E_OperationMode.VTL_OPMODE_CONFIG);
                        m_intOperationMode[intSelectedCtrl] = (int)E_OperationMode.VTL_OPMODE_CONFIG;
                    }
                }
            }
            else
            {
                TrackLog objTrack = new TrackLog();
                objTrack.WriteLine("Failed to get operation mode.");
            }
        }


        //--------------- Set channel grouping ---------------//
        // dwChannelGrp:
        // Channel     8 | 7 | 6 | 5 | 4 | 3 | 2 | 1
        // -------------------------------------------
        // Bit number  7 | 6 | 5 | 4 | 3 | 2 | 1 | 0
        // Set respective channel bit HIGH to group the channels together
        // e.g. To group Channel 8 and Channel 4 together
        // Set bit 7 and bit 3 high
        // Which result in dwChannelGrp of 136
        //
        // iMode can be:
        // VTL_MODE_CONTINUOUS or VTL_MODE_SEQUENCE
        //
        public static void SetGroupsAvailable(int intSelectedCtrl, uint uintGroupNum)
        {
            lock (m_objLock)
            {
                if (m_arrControllerConnected[intSelectedCtrl])
                {
                    int intError = 0;

                    //-------------------- Get channel grouping --------------------//
                    // Get total number of groups and all the available groups
                    intError = m_arrController[intSelectedCtrl].SetChannelGrouping(uintGroupNum);

                    if (intError == (int)vtErrorCode.VT_SUCCESS)
                    {
                        //--------------- Set light mode by group ---------------//
                        intError = m_arrController[intSelectedCtrl].SetLightModeByGroup(uintGroupNum, (int)E_LightMode.VTL_MODE_SEQUENCE);

                        if (intError != (int)vtErrorCode.VT_SUCCESS)
                        {
                            TrackLog objTrack = new TrackLog();
                            objTrack.WriteLine("Failed to get operation mode.");
                        }
                    }
                    else
                    {
                        TrackLog objTrack = new TrackLog();
                        objTrack.WriteLine("Failed to get operation mode.");
                    }
                }
            }
        }

        private static void ResetGroupsAvailable(int intSelectedCtrl)
        {
            int intTotalGroup = 0;
            int intError = 0;
            uint[] uintArrCtrlGroup = new uint[8];

            //-------------------- Get channel grouping --------------------//
            // Get total number of groups and all the available groups
            intError = m_arrController[intSelectedCtrl].GetChannelGrouping(ref intTotalGroup, ref uintArrCtrlGroup);

            // If group less than 8, means there are channel grouped together
            if (intTotalGroup < 8)
            {
                if (intError == (int)vtErrorCode.VT_SUCCESS)
                {
                    for (int intGroupCount = 0; intGroupCount < intTotalGroup; intGroupCount++)
                    {
                        //-------------------- Release channel grouping --------------------//
                        intError = m_arrController[intSelectedCtrl].ReleaseChannelGrouping(uintArrCtrlGroup[intGroupCount]);

                        if (intError != (int)vtErrorCode.VT_SUCCESS)
                        {
                            TrackLog objTrack = new TrackLog();
                            objTrack.WriteLine("Failed to release channel grouping! [Err: " + intError.ToString() + "]");
                        }
                    }
                }
                else
                {
                    TrackLog objTrack = new TrackLog();
                    objTrack.WriteLine("Failed to get channel grouping! [Err: " + intError.ToString() + "]");
                }
            }

            for (int intGroupCount = 0; intGroupCount < 8; intGroupCount++)
            {
                uint uintGroupNum = (uint)(1 << intGroupCount);
                int intLightMode = 0;

                //-------------------- Get light mode of a particular group --------------------//
                intError = m_arrController[intSelectedCtrl].GetLightModeByGroup(uintGroupNum, ref intLightMode);

                if (intError == (int)vtErrorCode.VT_SUCCESS)
                {
                    if (intLightMode == (int)E_LightMode.VTL_MODE_CONTINUOUS)
                    {
                        m_arrController[intSelectedCtrl].SetLightModeByGroup(uintGroupNum, (int)E_LightMode.VTL_MODE_SEQUENCE);
                    }
                }
                else
                {
                    TrackLog objTrack = new TrackLog();
                    objTrack.WriteLine("Failed to set channel light mode! [Err: " + intError.ToString() + "]");
                }

                //Off active flag for all setting except setting 1 for all group
                for (int i = 0; i < 30; i++)
                {
                    if (i == 0)
                        intError = m_arrController[intSelectedCtrl].SetActiveFlagByGroup(uintGroupNum, i, 1);
                    else
                        intError = m_arrController[intSelectedCtrl].SetActiveFlagByGroup(uintGroupNum, i, 0);
                }
            }
        }

        public static void SetRunMode(int intSelectedCtrl)
        {
            lock (m_objLock)
            {
                if (m_arrControllerConnected[intSelectedCtrl])
                {
                    //--------------- Set Run operation mode ---------------//
                    if (m_intOperationMode[intSelectedCtrl] != (int)E_OperationMode.VTL_OPMODE_RUN)
                    {
                        int intError = m_arrController[intSelectedCtrl].SetOperationMode((int)E_OperationMode.VTL_OPMODE_RUN);

                        if (intError == (int)vtErrorCode.VT_SUCCESS)
                        {
                            m_intOperationMode[intSelectedCtrl] = (int)E_OperationMode.VTL_OPMODE_RUN;
                            //RefreshGroupData();
                        }
                        else
                        {
                        }
                    }
                }
            }
        }

        public static void SetConfigMode(int intSelectedCtrl)
        {
            lock (m_objLock)
            {
                try
                {
                    if (m_arrControllerConnected[intSelectedCtrl])
                    {
                        //--------------- Set Config operation mode ---------------//
                        if (m_intOperationMode[intSelectedCtrl] != (int)E_OperationMode.VTL_OPMODE_CONFIG)
                        {
                            int intError = m_arrController[intSelectedCtrl].SetOperationMode((int)E_OperationMode.VTL_OPMODE_CONFIG);

                            if (intError == (int)vtErrorCode.VT_SUCCESS)
                            {
                                m_intOperationMode[intSelectedCtrl] = (int)E_OperationMode.VTL_OPMODE_CONFIG;
                                //RefreshGroupData();
                            }
                            else
                            {
                            }
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private static void SetMaximumCurrent(int intSelectedCtrl)
        {
            int intError = 0;
            //-------------------- set maximum current for all 8 channel --------------------//
            for (int i = 0; i < 8; i++)
            {
                intError = m_arrController[intSelectedCtrl].SetMaxCurrent(i, MAX_INTENSITY_CURRENT);

                if (intError != (int)vtErrorCode.VT_SUCCESS)
                {
                    TrackLog objTrack = new TrackLog();
                    objTrack.WriteLine("Failed to set maximum current for channel " + i + "! [Err: " + intError.ToString() + "]");
                }
            }
        }

        public static void SetIntensity(int intSelectedCtrl, int intChannel, int intCurrent)
        {
            lock (m_objLock)
            {
                if (m_arrControllerConnected[intSelectedCtrl])
                {
                    int intCurrentToScale = Convert.ToInt32(intCurrent * 51.4f); // convert from maximum 255 to 13107

                    if (intCurrentToScale <= MAX_INTENSITY_SCALE)
                    {
                        int intError = 0;

                        //-------------------- set intensity by scale of a setting --------------------//
                        intError = m_arrController[intSelectedCtrl].SetIntensityByScale(intChannel, 0, intCurrentToScale);

                        if (intError != (int)vtErrorCode.VT_SUCCESS)
                        {
                            TrackLog objTrack = new TrackLog();
                            objTrack.WriteLine("Failed to set intensity for channel " + intChannel + "! [Err: " + intError.ToString() + "]");
                        }
                    }
                    else
                    {
                        TrackLog objTrack = new TrackLog();
                        objTrack.WriteLine("Failed to set intensity for channel " + intChannel + " due to maximum intensity reached.");
                    }
                }
            }
        }

        public static void SetSeqIntensity(int intSelectedCtrl, int intSeq, int intChannel, int intCurrent)
        {
            lock (m_objLock)
            {
                if (m_arrControllerConnected[intSelectedCtrl])
                {
                    int intCurrentToScale = Convert.ToInt32(intCurrent * 51.4f); // convert from maximum 255 to 13107

                    if (intCurrentToScale <= MAX_INTENSITY_SCALE)
                    {
                        int intError = 0;

                        //-------------------- set intensity by scale of a setting --------------------//
                        intError = m_arrController[intSelectedCtrl].SetIntensityByScale(intChannel, intSeq, intCurrentToScale);

                        if (intError != (int)vtErrorCode.VT_SUCCESS)
                        {
                            TrackLog objTrack = new TrackLog();
                            objTrack.WriteLine("Failed to set intensity for channel " + intChannel + "! [Err: " + intError.ToString() + "]");
                        }
                    }
                    else
                    {
                        TrackLog objTrack = new TrackLog();
                        objTrack.WriteLine("Failed to set intensity for channel " + intChannel + " due to maximum intensity reached.");
                    }
                }
            }
        }

        private static void SetTriggerPulseWidth(int intSelectedCtrl, int intGroup, int intTriggerPulseWidth)
        {
            lock (m_objLock)
            {
                if (m_arrControllerConnected[intSelectedCtrl])
                {
                    uint uintGroupNum = Convert.ToUInt32(intGroup);

                    //-------------------- set trigger pulse width of a group --------------------//
                    int intError = m_arrController[intSelectedCtrl].SetMinTriggerPulseWidth(uintGroupNum, intTriggerPulseWidth);

                    if (intError != (int)vtErrorCode.VT_SUCCESS)
                    {
                        TrackLog objTrack = new TrackLog();
                        objTrack.WriteLine("Failed to set trigger value.");
                    }
                }
            }
        }

        private static void SetOutputPulseWidthLimit(int intSelectedCtrl, int intGroup, int intOutputPulseWidthLimit)
        {
            lock (m_objLock)
            {
                if (m_arrControllerConnected[intSelectedCtrl])
                {
                    uint uintGroupNum = Convert.ToUInt32(intGroup);

                    //-------------------- set output pulse width limit of a group --------------------//
                    int intError = m_arrController[intSelectedCtrl].SetOutputPulseWidthLimit(uintGroupNum, intOutputPulseWidthLimit);

                    if (intError != (int)vtErrorCode.VT_SUCCESS)
                    {
                        TrackLog objTrack = new TrackLog();
                        objTrack.WriteLine("Failed to set output value.");
                    }
                }
            }
        }

        public static void SetActiveOutFlag(int intSelectedCtrl, uint uintGroupNum, int intSettingID, int intCheckState)
        {
            //-------------------- Set active flag of group --------------------//
            // Set active flag of setting
            // intSettingID: 0 to 29
            // intActiveFlag: 0 - OFF; 1 - ON

            lock (m_objLock)
            {
                if (m_arrControllerConnected[intSelectedCtrl])
                {
                    if (m_arrControllerConnected[intSelectedCtrl])
                    {
                        int intError = m_arrController[intSelectedCtrl].SetActiveFlagByGroup(uintGroupNum, intSettingID, intCheckState);

                        if (intError != (int)vtErrorCode.VT_SUCCESS)
                        {
                            TrackLog objTrack = new TrackLog();
                            objTrack.WriteLine("Failed to set active flag.");
                        }
                    }
                }
            }
        }

        public static void Close()
        {
            lock (m_objLock)
            {
                for (int i = 0; i < m_arrController.Count; i++)
                {
                    if (m_arrController[i] != null)
                    {
                        if (m_arrControllerConnected[i])
                        {
                            m_arrControllerConnected[i] = false;
                            m_arrController[i].Disconnect();
                        }
                        if (Key != null) Key.Close();
                    }
                }
            }
        }

        ///// <summary>
        ///// Load channel parameters
        ///// </summary>
        ///// <param name="intComNo">com port no</param>
        ///// <param name="intChannel">channel</param>
        //public static void LoadIntensity(int intCtrlCount, int intChannel)
        //{
        //    lock (m_objLock)
        //    {
        //        if (m_arrControllerConnected[intCtrlCount])
        //        {
        //            int intError = 0;

        //            //---------- Load configuration of selected channel from flash memory ----------//
        //            int check = m_arrController[intCtrlCount].CheckConnectivity();
        //            intError = m_arrController[intCtrlCount].LoadSetting(intChannel, ref m_arrStcChannelParam[intChannel]);

        //            if (intError != (int)vtErrorCode.VT_SUCCESS)
        //            {
        //                TrackLog objTrack = new TrackLog();
        //                objTrack.WriteLine("Failed to load channel" + (intChannel + 1).ToString() + ", Error: " + intError.ToString());
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Save channel parameters
        /// </summary>
        /// <param name="intComNo">com port no</param>
        /// <param name="intChannel">channel</param>
        public static void SaveIntensity(int intSelectedCtrl, int intChannel)
        {
            lock (m_objLock)
            {
                if (m_arrControllerConnected[intSelectedCtrl])
                {
                    int intError = 0;

                    //---------- Save configuration of selected channel from flash memory ----------//
                    intError = m_arrController[intSelectedCtrl].SaveSetting(intChannel);

                    if (intError != (int)vtErrorCode.VT_SUCCESS)
                    {
                        TrackLog objTrack = new TrackLog();
                        objTrack.WriteLine("Failed to save channel" + (intChannel + 1).ToString() + ", Error: " + intError.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Save channel parameters
        /// </summary>
        /// <param name="intComNo">com port no</param>
        /// <param name="intChannel">channel</param>
        public static void SaveAllSetting(int intSelectedCtrl)
        {
            lock (m_objLock)
            {
                if (m_arrControllerConnected[intSelectedCtrl])
                {
                    int intError = 0;

                    //---------- Save configuration of selected channel from flash memory ----------//
                    intError = m_arrController[intSelectedCtrl].SaveSetting();

                    if (intError != (int)vtErrorCode.VT_SUCCESS)
                    {
                        TrackLog objTrack = new TrackLog();
                        objTrack.WriteLine("Failed to save all settings, Error: " + intError.ToString());
                    }
                }
            }
        }



        ///// <summary>
        ///// Set intensity of the particular light source channel (non sequence)
        ///// </summary>
        ///// <param name="intComNo">com port no</param>
        ///// <param name="Channel">channel</param>
        ///// <param name="Intensity">intensity value</param>
        //public static void SetIntensity(int intCtrlCount, int intChannel, int intIntensity)
        //{
        //    lock (m_objLock)
        //    {
        //        if (m_arrControllerConnected[intCtrlCount])
        //        {
        //            if (m_arrController.Count <= intCtrlCount)
        //                return;

        //            // There is 4 intensity setting for each channel, only set setting 3, other set to 0
        //            for (int i = 0; i < 4; i++)
        //            {
        //                int iError = 0;
        //                if (i == 2)
        //                {
        //                    //---------- Set intensity of setting 3 in a channel -----//
        //                    iError = m_arrController[intCtrlCount].SetIntensityByScale(intChannel, 2, intIntensity);

        //                    if (iError == (int)vtErrorCode.VT_SUCCESS)
        //                        m_arrStcChannelParam[intChannel].iIntensity[2] = intIntensity;
        //                }
        //                else
        //                {
        //                    if (m_arrStcChannelParam[intChannel].iIntensity[i] != 0)
        //                    {
        //                        iError = m_arrController[intCtrlCount].SetIntensityByScale(intChannel, i, 0);
        //                        if (iError == (int)vtErrorCode.VT_SUCCESS)
        //                            m_arrStcChannelParam[intChannel].iIntensity[i] = 0;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// Set sequence intensity of the particular light source channel
        ///// </summary>
        ///// <param name="intComNo">com port no</param>
        ///// <param name="intSeq">sequence number</param>
        ///// <param name="intIntensity">intensity value</param>
        //public static void SetSeqIntensity(int intCtrlCount, int intSeq, int intChannel, int intIntensity)
        //{
        //    lock (m_objLock)
        //    {
        //        if (m_arrControllerConnected[intCtrlCount])
        //        {
        //            if (m_arrController.Count <= intCtrlCount)
        //                return;

        //            try
        //            {
        //                int iError = 0;

        //                //---------- Set intensity of setting 1 in a channel -----//
        //                iError = m_arrController[intCtrlCount].SetIntensityByScale(intChannel, intSeq, intIntensity);

        //                if (iError == (int)vtErrorCode.VT_SUCCESS)
        //                    m_arrStcChannelParam[intChannel].iIntensity[intSeq] = intIntensity;
        //            }
        //            catch (TimeoutException ex)
        //            {
        //                TrackLog objTrack = new TrackLog();
        //                objTrack.WriteLine(ex.ToString());
        //            }
        //        }
        //    }
        //}
    }
}
