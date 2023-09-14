using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace SharedMemory
{
    public class TCPIPIO
    {
        #region Delegate Event

        public delegate void ReceiveCommandHandle(string strMessage);
        public event ReceiveCommandHandle ReceiveCommandEvent;

        #endregion

        #region Members Variables
        private static object m_objLock = new object();
        // TCPIP Client
        private bool m_blnGotMessage = false;
        private bool m_blnTCPIPEnable = false;
        private bool m_blnFirstInit = true;
        private int m_intPort = 8080;
        private int m_intTCPIPTimeOut = 100;
        private int m_intDefaultPort;

        private TCPServer m_objTCPServer = new TCPServer();
        private TrackLog m_objTrack = new TrackLog();

        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;

        private int m_intTestOption = 0;

        // Same IO function among visions
        public bool StartVision_In = false;
        public bool EndVision_Out = true;
        public bool Grabbing_Out = false;

        // Rotary Machine
        public bool InPocketCheckUnit1_In = false;
        public bool InPocketReTest_In = false;
        public bool InPocketReCheckUnit2_In = false;
        public bool InPocketEndOfReTest_In = false;
        public bool InPocketCheckEmpty_In = false;
        public bool RotatorSignal1_In = false;
        public bool RotatorSignal2_In = false;
        public bool IOPass1_Out = true;
        public bool IOPass2_Out = true;
        public bool OrientResult2_Out = false;
        public bool OrientResult1_Out = false;
        public bool UnitPresent_Out = true;
        public bool PackageFail_Out = false;        // Fail criteria: Package
        public bool EmptyUnit_Out = false;          // Fail criteria: Empty Unit           
        public bool WrongOrientation_Out = false;   // Fail criteria: Wrong Orientation
        public bool PositionReject_Out = false;     // Fail criteria: Position Reject
        public bool MarkFail_Out = false;           // Fail criteria: Mark
        public bool FailLead_Out = false;           // Fail criteria in Mark Orient : Trigger this IO when fail criteria is not mark
        public bool FailNoMark_Out = false;         // Fail criteria: no Mark
        // Zyro Machine IO
        public bool ResultBit1_Out = false;
        public bool ResultBit2_Out = false;
        public bool ResultBit3_Out = false;
        public bool ResultBit4_Out = false;
        public bool ResultBit5_Out = false;
        public bool ResultBit6_Out = false;

        // Grab Image IO
        public bool Grab1_Out = false;
        public bool Grab2_Out = false;
        public bool Grab3_Out = false;

        // Specific for Seal
        public bool CheckPresentQA_In = false;
        public bool CheckPresent_In = false;
        public bool Retest_In = false;

        //New Added 

        // Reserved
        public bool LaserDone_Out = false;
        public bool PCBFail_Out = false;

        //MarkPkg
        public bool Data1_In = false;
        public bool Data2_In = false;
        public bool Data4_In = false;

        //InPocketPkg
        public bool FailOffset_Out = false;
        public bool OffsetDone_Out = false;
        public bool InPocketRollbackRetest_In = false;

        //PadPkg
        public bool CheckPH_In = false;

        #endregion


        #region Properties

        public bool ref_blnTCPIPEnable { set { m_blnTCPIPEnable = value; } }
        public bool ref_blnGotMessage { get { return m_blnGotMessage; } set { m_blnGotMessage = value; } }

        #endregion

        public TCPIPIO(ProductionInfo smProductionInfo, VisionInfo smVisionInfo, int intDefaultPort)
        {
            m_intDefaultPort = intDefaultPort;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;

            ReadFromXML();
        }
        public void ReadFromXML()
        {
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "\\Option.xml");
            objFile.GetFirstSection("General");
            if (objFile.GetValueAsBoolean("ConfigShowTCPIP", false) && objFile.GetValueAsBoolean("WantUseTCPIPAsIO", false))
            {
                objFile.GetFirstSection("TCPIP");
                m_intTCPIPTimeOut = objFile.GetValueAsInt("TimeOut", 100);
                m_intPort = objFile.GetValueAsInt(m_smVisionInfo.g_strVisionName + m_smVisionInfo.g_strVisionNameNo + "_TCPIPIO", 6010 + m_intDefaultPort);

                if (m_blnFirstInit)
                {
                    m_objTCPServer.ReceiveEvent += new TCPServer.ReceivedDataHandle(TakeAction);
                    m_blnFirstInit = false;
                }
                m_objTCPServer.Init(m_intPort, m_intTCPIPTimeOut, "10.0.0.1");
            }
        }
        public void Send(string strMessage)
        {
            lock (m_objLock)
            {
                try
                {
                    //if (m_blnTCPIPEnable)
                    {
                        m_smVisionInfo.g_strTCPMessage += "*" + strMessage;
                        m_blnGotMessage = true;
                    }
                    m_objTCPServer.Send(strMessage, m_smProductionInfo.g_blnTrackTCPIP_IO);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void Send(int intVisionID, string strMessage, bool blnResult, int intTestOption)
        {
            lock (m_objLock)
            {
                try
                {
                    string strFinalMessage = "";
                    //if (m_blnTCPIPEnable)
                    {
                        string strResult = "OK";

                        if (!blnResult)
                            strResult = "NG";

                        if (intTestOption >= 0)
                            strFinalMessage = "<" + Math.Pow(2, intVisionID).ToString() + "," + strMessage + "," + strResult + "," + intTestOption.ToString() + ">";
                        else
                            strFinalMessage = "<" + Math.Pow(2, intVisionID).ToString() + "," + strMessage + "," + strResult + ",>";
                        m_smVisionInfo.g_strTCPMessage += "*" + strFinalMessage;
                        m_blnGotMessage = true;
                    }
                    m_objTCPServer.Send(strFinalMessage, m_smProductionInfo.g_blnTrackTCPIP_IO);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void Send_Result(int intVisionID, bool blnResult, bool blnOrientResult1, bool blnOrientResult2, int intResultID)
        {
            lock (m_objLock)
            {
                try
                {
                    string strFinalMessage = "";
                    //if (m_blnTCPIPEnable)
                    {
                        if (!blnResult && intResultID < 0)
                            strFinalMessage = "<" + Math.Pow(2, intVisionID).ToString() + ",RESRP,NG,>";
                        else
                        {
                            string strResult = "OK";
                            string strTestResult = "P";

                            if (!blnResult)
                            {
                                strTestResult = "F";
                                //strResult = "NG";
                            }

                            //int intOrientResult1 = 0;
                            //if (blnOrientResult1)
                            //    intOrientResult1 = 1;

                            //int intOrientResult2 = 0;
                            //if (blnOrientResult2)
                            //    intOrientResult2 = 1;

                            int intOrientResult = 0;
                            if (blnOrientResult1 && !blnOrientResult2)
                                intOrientResult = 1;
                            else if (blnOrientResult1 && blnOrientResult2)
                                intOrientResult = 2;
                            else if (!blnOrientResult1 && blnOrientResult2)
                                intOrientResult = 3;

                            string strResultID = "";
                            if (intResultID.ToString().Length == 0)
                                strResultID = "00";
                            else if (intResultID.ToString().Length == 1)
                                strResultID = "0" + intResultID.ToString();
                            else
                                strResultID = intResultID.ToString();

                            if (blnResult)
                                strFinalMessage = "<" + Math.Pow(2, intVisionID).ToString() + ",RESRP," + strResult + "," + strTestResult + "0" + intOrientResult.ToString() + ">";
                            else
                                strFinalMessage = "<" + Math.Pow(2, intVisionID).ToString() + ",RESRP," + strResult + "," + strTestResult + strResultID + ">";
                        }
                        m_smVisionInfo.g_strTCPMessage += "*" + strFinalMessage;
                        m_blnGotMessage = true;
                    }
                    m_objTCPServer.Send(strFinalMessage, m_smProductionInfo.g_blnTrackTCPIP_IO);
                }
                catch (Exception ex)
                {
                }
            }
        }

        public void Send_ResultForCheckOffset(int intVisionID, bool blnResult, bool blnOrientResult1, bool blnOrientResult2, float fOffsetX, float fOffsetY, float fAngle, bool blnPocketPositionResult)
        {
            lock (m_objLock)
            {
                try
                {
                    string strFinalMessage = "";
                    //if (m_blnTCPIPEnable)
                    {
                        string strResult = "OK";
                        string strTestResult = "P";

                        if (!blnResult)
                        {
                            strTestResult = "F";
                            //strResult = "NG";
                        }

                        //int intOrientResult1 = 0;
                        //if (blnResult && blnOrientResult1)
                        //    intOrientResult1 = 1;

                        //int intOrientResult2 = 0;
                        //if (blnResult && blnOrientResult2)
                        //    intOrientResult2 = 1;

                        int intOrientResult = 0;
                        if (blnOrientResult1 && !blnOrientResult2)
                            intOrientResult = 1;
                        else if (blnOrientResult1 && blnOrientResult2)
                            intOrientResult = 2;
                        else if (!blnOrientResult1 && blnOrientResult2)
                            intOrientResult = 3;

                        //string strPositionXSign = "P";
                        //if (fOffsetX < 0)
                        //    strPositionXSign = "N";

                        //string strPositionYSign = "P";
                        //if (fOffsetY < 0)
                        //    strPositionYSign = "N";

                        //string strAngleSign = "P";
                        //if (fAngle < 0)
                        //    strAngleSign = "N";

                        float fObjectOffsetX = fOffsetX;
                        fObjectOffsetX = fObjectOffsetX * 1000 / m_smVisionInfo.g_fCalibPixelX; // Convert to micron

                        //if (Math.Abs(Math.Round(fObjectOffsetX)) > 100000)
                        //    fObjectOffsetX = 0;

                        fObjectOffsetX -= m_smVisionInfo.g_objPocketPosition.ref_intPocketPositionReference;

                        if (!blnPocketPositionResult) //2021-12-15 ZJYEOH : Set offset X to 0 if pocket position fail
                            fObjectOffsetX = 0;

                        float fObjectOffsetY = fOffsetY;
                        //fObjectOffsetY = fObjectOffsetY * 1000 / m_smVisionInfo.g_fCalibPixelY; // Convert to micron
                        fObjectOffsetY = 0; //2019-12-11 ZJYEOH : Temparory set to zero because no Y Data

                        

                        string strPositionXSign = "P";
                        if (fObjectOffsetX < 0)
                            strPositionXSign = "N";

                        string strPositionYSign = "P";
                        if (fObjectOffsetY < 0)
                            strPositionYSign = "N";

                        string strAngleSign = "P";
                        if (fAngle < 0)
                            strAngleSign = "N";


                        strFinalMessage = "<" + Math.Pow(2, intVisionID).ToString() + ",RESRP," + strResult + "," + strTestResult + "0" + intOrientResult.ToString() +
                                          "@X" + strPositionXSign + string.Format("{0:00000}", Math.Abs(Math.Round(fObjectOffsetX))) +
                                          "*Y" + strPositionYSign + string.Format("{0:00000}", Math.Abs(Math.Round(fObjectOffsetY))) +
                                          "*A" + strAngleSign + string.Format("{0:00000}", Math.Abs(Math.Round(fAngle))) + ">";
                        m_smVisionInfo.g_strTCPMessage += "*" + strFinalMessage;
                        m_blnGotMessage = true;
                    }
                    m_objTCPServer.Send(strFinalMessage, m_smProductionInfo.g_blnTrackTCPIP_IO);
                }
                catch (Exception ex)
                {
                }
            }
        }
        private void TakeAction(string strMessage)
        {
            lock (m_objLock)
            {
                try
                {
                    if (strMessage == "Client disconnected.")
                        if (ReceiveCommandEvent != null) ReceiveCommandEvent(strMessage);

                    //if (m_blnTCPIPEnable)
                    {
                        m_smVisionInfo.g_strTCPMessage += "*" + strMessage;
                        m_blnGotMessage = true;
                    }

                    if (!(strMessage.StartsWith("<") && strMessage.EndsWith(">")))
                        return;

                    if (ReceiveCommandEvent != null) ReceiveCommandEvent(strMessage);
                }
                catch (Exception ex)
                {
                    TrackLog objLog = new TrackLog();
                    objLog.WriteLine(ex.ToString());
                }
            }
        }
    }
}
