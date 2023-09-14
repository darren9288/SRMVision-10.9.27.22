using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using SharedMemory;
using System.Runtime.InteropServices;


namespace SRMVision
{
    public class ComThread
    {
        #region Delegate Event

        public delegate void UpdateInterfaceHandle(string strSender, string strMessage);
        public event UpdateInterfaceHandle UpdateInterfaceEvent;

        #endregion


        #region Members Variables
        char[] m_arrSpecialChars = new char[] { '\r', '\n' }; //2021-10-27 ZJYEOH : Carsem machine will have these special chars in the start or end of the message

        // Thread handle
        private readonly object m_objStopLock = new object();
        private bool m_blnStopping = false;
        private bool m_blnStopped = false;

        // TCPIP Client
        private bool m_blnTCPIPEnable = false;
        private bool m_blnFirstInit = true;
        private int m_intPort = 8080;
        private int m_intTCPIPTimeOut = 100;

        // Status
        private bool m_blnShowMessageBox = false;
        private bool m_blnProcessNewLot = false;
        private bool m_blnProcessEndLot = false;
        private bool m_blnProcessRerunLot = false;
        private bool m_blnProcessTMPL = false;
        private bool m_blnProcessReset = false;
        private bool m_blnProcessLoadRecipe = false;
        private bool m_blnProcess2DCode = false;
        private string m_strErrorMessage = "";
        private string m_strNewLotSuccess = "", m_strNewLotFail = "";
        private string m_strEndLotSuccess = "", m_strEndLotFail = "";
        private string m_strRerunLotSuccess = "", m_strRerunLotFail = "";
        private string m_strTMPLSuccess = "", m_strTMPLFail = "";
        private string m_strLoadRecipeSuccess = "", m_strLoadRecipeFail = "";
        private string m_strResetSuccess = "", m_strResetFail = "";
        private string m_str2DCodeSuccess = "", m_str2DCodeFail = "";

        private TCPServer m_objTCPServer = new TCPServer();
        private TrackLog m_objTrack = new TrackLog();

        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private Thread m_thThread;
        private VisionInfo[] m_smVSInfo;

        #endregion

        #region Properties

        public bool ref_blnTCPIPEnable { get { return m_blnTCPIPEnable; } }

        #endregion


        public ComThread(CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, VisionInfo[] smVSInfo)
        {
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_smVSInfo = smVSInfo;

            ReadFromXML();

            //List<string> arrThreadNameBF = new List<string>();
            //List<string> arrThreadNameAF = new List<string>();
            //arrThreadNameBF = ProcessTh.GetThreadsName("SRMVision");

            m_thThread = new Thread(new ThreadStart(UpdateProgress));
            m_thThread.IsBackground = true;
            m_thThread.Start();

            //Thread.Sleep(500);
            //arrThreadNameAF = ProcessTh.GetThreadsName("SRMVision");
            //ProcessTh.GetDifferentThreadsName(arrThreadNameAF, arrThreadNameBF, "5", 0x02);
        }




        /// <summary>
        /// Returns whether the worker thread has stopped.
        /// </summary>
        public bool IsThreadStopped
        {
            get
            {
                lock (m_objStopLock)
                {
                    return m_blnStopped;
                }
            }
        }

        public void ReadFromXML()
        {
            XmlParser objFile = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "\\Option.xml");

            objFile.GetFirstSection("General");
            if (objFile.GetValueAsBoolean("ConfigShowTCPIP", false))
            {
                objFile.GetFirstSection("TCPIP");
                if (objFile.GetValueAsBoolean("Enable", false))
                {
                    m_intTCPIPTimeOut = objFile.GetValueAsInt("TimeOut", 100);
                    m_intPort = objFile.GetValueAsInt("MainPort", 8080);

                    if (m_blnFirstInit)
                    {
                        m_objTCPServer.ReceiveEvent += new TCPServer.ReceivedDataHandle(TakeAction);
                        m_blnFirstInit = false;
                    }

                    m_objTCPServer.Init(m_intPort, m_intTCPIPTimeOut);
                }
                else
                {
                    if (m_objTCPServer != null)
                        m_objTCPServer.CloseConnection();
                    m_objTCPServer = null;
                }

                for (int i = 0; i < m_smProductionInfo.g_arrVisionIDIndex.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    string strValue = objFile.GetValueAsString(m_smVSInfo[i].g_strVisionName + m_smVSInfo[i].g_strVisionNameNo + "_VisionID", (0x01 << m_smProductionInfo.g_arrVisionIDIndex[i]).ToString());
                    int intValue = 0;
                    int.TryParse(strValue, out intValue);
                    m_smProductionInfo.g_arrVisionIDIndex[i] = GetBitIndexFromHex(intValue);
                }
            }
        }

        public void Send(string strMessage)
        {
            if (UpdateInterfaceEvent != null) UpdateInterfaceEvent("Common", strMessage);
            m_objTCPServer.Send(strMessage, m_smProductionInfo.g_blnTrackTCPIP_IO);
        }

        /// <summary>
        /// Tells the thread to stop, typically after completing its 
        /// current work item.
        /// </summary>
        public void StopThread()
        {
            lock (m_objStopLock)
            {
                m_blnStopping = true;
            }
        }






        private void ProcessDeleteTemplateCommand(string[] strMessage)
        {
            m_strTMPLFail = "<TMPLRP,NG," + strMessage[2] + ",>";
            m_strTMPLSuccess = "<TMPLRP,OK," + strMessage[2] + ",>";

            try
            {
                m_smProductionInfo.g_intVisionInvolved = Convert.ToInt32(strMessage[2]);

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null || ((m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_intVisionInvolved != 0 && m_smProductionInfo.g_arrVisionIDIndex[i] != 0))
                        continue;

                    //2021-01-15 ZJYEOH : allow delete template even in production mode
                    //if (m_smVSInfo[i].g_intMachineStatus == 2)
                    //{
                    //    Send(m_strTMPLFail);
                    //    m_strErrorMessage = "Cannot Delete Template In Production Mode";
                    //    m_blnShowMessageBox = true;
                    //    return;
                    //}

                    if (m_smVSInfo[i].VM_AT_SettingInDialog || m_smVSInfo[i].VM_AT_OfflinePageView)
                    {
                        Send(m_strTMPLFail);
                        m_strErrorMessage = "Please exit all setting form and manual test form";
                        m_blnShowMessageBox = true;
                        return;
                    }
                }
                
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (((m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) > 0 || m_smProductionInfo.g_intVisionInvolved == 0 || m_smProductionInfo.g_arrVisionIDIndex[i] == 0))
                    {
                        VisionInfo smVisionInfo = m_smVSInfo[i];
                        smVisionInfo.PR_CO_DeleteTemplateDone = false;
                        smVisionInfo.CO_PR_DeleteTemplate = true;
                    }
                    else if (!m_smVSInfo[i].PR_CO_DeleteProcessSuccess)
                        m_smVSInfo[i].PR_CO_DeleteProcessSuccess = true;
                }

                m_smProductionInfo.AT_CO_DeleteTemplateDone = false;
                m_blnProcessTMPL = true;
            }
            catch (Exception ex)
            {
                m_objTrack.WriteLine("ComThread (ProcessDeleteTemplateCommand): " + ex.ToString());
                Send(m_strTMPLFail);
            }
        }

        private void ProcessEndLotCommand(string[] strMessage)
        {
            m_strEndLotFail = "<ENDRP,NG," + strMessage[2] + "," + strMessage[3] + ">";
            m_strEndLotSuccess = "<ENDRP,OK," + strMessage[2] + "," + strMessage[3] + ">";

            try
            {
                m_smProductionInfo.g_intVisionInvolved = Convert.ToInt32(strMessage[2]);

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null || (m_smProductionInfo.g_intVisionInvolved != 0 && (m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_arrVisionIDIndex[i] != 0))
                        continue;

                    if (m_smVSInfo[i].VM_AT_SettingInDialog || m_smVSInfo[i].VM_AT_OfflinePageView)
                    {
                        Send(m_strEndLotFail);
                        m_strErrorMessage = "Please exit all setting form and manual test form";
                        m_blnShowMessageBox = true;
                        return;
                    }
                }

                m_smProductionInfo.AT_CO_EndLotDone = false;
                m_smProductionInfo.CO_AT_EndLot = true;
                m_blnProcessEndLot = true;
            }
            catch (Exception ex)
            {
                m_objTrack.WriteLine("ComThread (ProcessEndLotCommand) : " + ex.ToString());
                Send(m_strEndLotFail);
            }
        }

        private void ProcessLoadRecipeCommand(string[] strMessage)
        {
            m_strLoadRecipeFail = "<LDRP,NG," + strMessage[2] + ",>";
            m_strLoadRecipeSuccess = "<LDRP,OK," + strMessage[2] + ",>";

            try
            {
                m_smProductionInfo.g_intVisionInvolved = Convert.ToInt32(strMessage[2]);

                if (strMessage[1] == "")
                {
                    Send(m_strLoadRecipeFail);
                    m_strErrorMessage = "This recipe name is blank";
                    m_blnShowMessageBox = true;
                    return;
                }

                if (!Directory.Exists(m_smProductionInfo.g_strRecipePath + strMessage[1]))
                {
                    Send(m_strLoadRecipeFail);
                    m_strErrorMessage = "This recipe - " + strMessage[1] + " is unavailable";
                    m_blnShowMessageBox = true;
                    return;
                }

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null || (m_smProductionInfo.g_intVisionInvolved != 0 && (m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_arrVisionIDIndex[i] != 0))
                        continue;


                    if (m_smVSInfo[i].VM_AT_SettingInDialog || m_smVSInfo[i].VM_AT_OfflinePageView)
                    {
                        Send(m_strLoadRecipeFail);
                        m_strErrorMessage = "Please exit all setting form and manual test form";
                        m_blnShowMessageBox = true;
                        return;
                    }
                }

                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                subkey1.SetValue("RecipeIDPrev", m_smProductionInfo.g_strRecipeID);
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null || (m_smProductionInfo.g_intVisionInvolved != 0 && (m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_arrVisionIDIndex[i] != 0))
                        continue;

                    subkey1.SetValue("SingleRecipeIDPrev" + i.ToString(), m_smProductionInfo.g_arrSingleRecipeID[i]);
                }
                m_smProductionInfo.g_strRecipeID = strMessage[1];

                for (int i = 0; i < 10; i++)
                {
                    if (i >= m_smVSInfo.Length)
                        break;

                    if (m_smVSInfo[i] == null || (m_smProductionInfo.g_intVisionInvolved != 0 && (m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_arrVisionIDIndex[i] != 0))
                        continue;

                    m_smProductionInfo.g_arrSingleRecipeID[i] = m_smProductionInfo.g_strRecipeID;
                }

                //2021-07-15 ZJYEOH : hide this when move from Vanchip because not sure will affect the individual recipe
                //if (m_smProductionInfo.g_strRecipeID != strMessage[1])  // 2021 03 29 - CCENG : No need to load recipe if same recipe name. 
                {
                    m_smProductionInfo.AT_CO_LoadRecipeDone = false;
                    m_smProductionInfo.CO_AT_LoadRecipe = true;
                }

                m_blnProcessLoadRecipe = true;
            }
            catch (Exception ex)
            {
                m_objTrack.WriteLine("ComThread (ProcessLoadRecipeCommand): " + ex.ToString());
                Send(m_strLoadRecipeFail);
            }

        }

        private void ProcessNewLotCommand(string[] strMessage)
        {
            m_strNewLotFail = "<NEWRP,NG," + strMessage[2] + "," + strMessage[3] + ">";
            m_strNewLotSuccess = "<NEWRP,OK," + strMessage[2] + "," + strMessage[3] + ">";

            try
            {
                m_smProductionInfo.g_intVisionInvolved = Convert.ToInt32(strMessage[2]);

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null || (m_smProductionInfo.g_intVisionInvolved != 0 && (m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_arrVisionIDIndex[i] != 0))
                        continue;

                    if (m_smVSInfo[i].VM_AT_SettingInDialog || m_smVSInfo[i].VM_AT_OfflinePageView)
                    {
                        Send(m_strNewLotFail);
                        m_strErrorMessage = "Please exit all setting form and manual test form";
                        m_blnShowMessageBox = true;
                        return;
                    }
                }

                int intAlias = strMessage[1].IndexOf("@");
                int intAsterisk = strMessage[1].IndexOf("*");
                string strOPID = "";
                string strLotID = "";
                if (strMessage[1].IndexOf("@") > 0)
                {
                    strLotID = strMessage[1].Substring(0, intAlias);

                    if (strMessage[1].IndexOf("*") > 0)
                        strOPID = strMessage[1].Substring(intAlias + 1, (intAsterisk - 1) - intAlias);
                    else
                        strOPID = strMessage[1].Substring(intAlias + 1, strMessage[1].Length - (intAlias + 1));
                }
                else if (strMessage[1].IndexOf("*") > 0)
                {
                    strLotID = strMessage[1].Substring(0, intAsterisk);
                }

                //if (strOPID == "")
                //{
                //    Send(m_strNewLotFail);
                //    m_strErrorMessage = "Please fill in your operator ID";
                //    m_blnShowMessageBox = true;
                //    return;
                //}
                if (strLotID.IndexOf("_") >= 0)
                {
                    Send(m_strNewLotFail);
                    m_strErrorMessage = "Symbol _ is not allowed in LotID";
                    m_blnShowMessageBox = true;
                    return;
                }
                if (strLotID.IndexOf("/") >= 0)
                {
                    Send(m_strNewLotFail);
                    m_strErrorMessage = "Symbol / is not allowed in LotID";
                    m_blnShowMessageBox = true;
                    return;
                }
                if (strLotID.IndexOf("*") >= 0)
                {
                    Send(m_strNewLotFail);
                    m_strErrorMessage = "Symbol * is not allowed in LotID";
                    m_blnShowMessageBox = true;
                    return;
                }
                if (strLotID.IndexOf("\\") >= 0)
                {
                    Send(m_strNewLotFail);
                    m_strErrorMessage = "Symbol \\ is not allowed in LotID";
                    m_blnShowMessageBox = true;
                    return;
                }

                if (strLotID == "")
                    strLotID = "SRM";

                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                subkey1.SetValue("LotNoPrev", m_smProductionInfo.g_strLotID);
                subkey1.SetValue("OperatorIDPrev", m_smProductionInfo.g_strOperatorID);
                subkey1.SetValue("LotStartTimePrev", m_smProductionInfo.g_strLotStartTime);

                m_smProductionInfo.g_strLotID = strLotID;
                for (int i = 0; i < 10; i++)
                {
                    if (i >= m_smVSInfo.Length)
                        break;

                    if (m_smVSInfo[i] == null || (m_smProductionInfo.g_intVisionInvolved != 0 && (m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_arrVisionIDIndex[i] != 0))
                        continue;

                    m_smProductionInfo.g_arrIsNewLot[i] = true;
                    subkey1.SetValue("IsNewLot" + i.ToString(), m_smProductionInfo.g_arrIsNewLot[i]);
                    m_smProductionInfo.g_arrSingleLotID[i] = m_smProductionInfo.g_strLotID;
                }

                if (strOPID != "")
                    m_smProductionInfo.g_strOperatorID = strOPID;

                m_smProductionInfo.g_strLotStartTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                //m_smProductionInfo.g_strLotStartTime = strMessage[1].Substring(intAsterisk + 1, strMessage[1].Length - (intAsterisk + 1)).Replace(" ", "");

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (((m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) > 0 || m_smProductionInfo.g_intVisionInvolved == 0 || m_smProductionInfo.g_arrVisionIDIndex[i] == 0))
                    {
                        VisionInfo smVisionInfo = m_smVSInfo[i];
                        if (strMessage[3] == "1")
                        {
                            smVisionInfo.PR_CO_DeleteTemplateDone = false;
                            smVisionInfo.CO_PR_DeleteTemplate = true;
                        }
                    }
                }

                m_smProductionInfo.AT_CO_NewLotDone = false;
                m_smProductionInfo.CO_AT_NewLot = true;
                m_blnProcessNewLot = true;
            }
            catch (Exception ex)
            {
                m_objTrack.WriteLine("ComThread (ProcessNewLotCommand) : " + ex.ToString());
                Send(m_strNewLotFail);
            }
        }

        private void ProcessRerunCommand(string[] strMessage)
        {
            m_strRerunLotFail = "<RERUNRP,NG," + strMessage[2] + "," + strMessage[3] + ">";
            m_strRerunLotSuccess = "<RERUNRP,OK," + strMessage[2] + "," + strMessage[3] + ">";

            try
            {
                m_smProductionInfo.g_intVisionInvolved = Convert.ToInt32(strMessage[2]);

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null || (m_smProductionInfo.g_intVisionInvolved != 0 && (m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_arrVisionIDIndex[i] != 0))
                        continue;

                    if (m_smVSInfo[i].VM_AT_SettingInDialog || m_smVSInfo[i].VM_AT_OfflinePageView)
                    {
                        Send(m_strRerunLotFail);
                        m_strErrorMessage = "Please exit all setting form and manual test form";
                        m_blnShowMessageBox = true;
                        return;
                    }
                }

                int intAlias = strMessage[1].IndexOf("@");
                string strOPID = "";
                string strLotID = "";
                if (strMessage[1].IndexOf("@") > 0)
                {
                    strLotID = strMessage[1].Substring(0, intAlias);
                    strOPID = strMessage[1].Substring(intAlias + 1, strMessage[1].Length - (intAlias + 1));
                }

                if (strLotID.IndexOf("_") >= 0)
                {
                    Send(m_strRerunLotFail);
                    m_strErrorMessage = "Symbol _ is not allowed in LotID";
                    m_blnShowMessageBox = true;
                    return;
                }
                if (strLotID.IndexOf("/") >= 0)
                {
                    Send(m_strRerunLotFail);
                    m_strErrorMessage = "Symbol / is not allowed in LotID";
                    m_blnShowMessageBox = true;
                    return;
                }
                if (strLotID.IndexOf("*") >= 0)
                {
                    Send(m_strRerunLotFail);
                    m_strErrorMessage = "Symbol * is not allowed in LotID";
                    m_blnShowMessageBox = true;
                    return;
                }
                if (strLotID.IndexOf("\\") >= 0)
                {
                    Send(m_strRerunLotFail);
                    m_strErrorMessage = "Symbol \\ is not allowed in LotID";
                    m_blnShowMessageBox = true;
                    return;
                }

                if (strLotID == "")
                    strLotID = "SRM";

                if (strOPID == "")
                    strOPID = m_smProductionInfo.g_strOperatorID;

                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                subkey1.SetValue("LotNoPrev", m_smProductionInfo.g_strLotID);
                subkey1.SetValue("OperatorIDPrev", m_smProductionInfo.g_strOperatorID);
                subkey1.SetValue("LotStartTimePrev", m_smProductionInfo.g_strLotStartTime);

                m_smProductionInfo.g_strLotID = strLotID;
                for (int i = 0; i < 10; i++)
                {
                    if (i >= m_smVSInfo.Length)
                        break;

                    if (m_smVSInfo[i] == null || (m_smProductionInfo.g_intVisionInvolved != 0 && (m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_arrVisionIDIndex[i] != 0))
                        continue;

                    m_smProductionInfo.g_arrIsNewLot[i] = true;
                    subkey1.SetValue("IsNewLot" + i.ToString(), m_smProductionInfo.g_arrIsNewLot[i]);
                    m_smProductionInfo.g_arrSingleLotID[i] = m_smProductionInfo.g_strLotID;
                }
                m_smProductionInfo.g_strOperatorID = strOPID;
                m_smProductionInfo.g_strLotStartTime = DateTime.Now.ToString("yyyyMMddHHmmss");

                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (((m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) > 0 || m_smProductionInfo.g_intVisionInvolved == 0) || m_smProductionInfo.g_arrVisionIDIndex[i] == 0)
                    {
                        VisionInfo smVisionInfo = m_smVSInfo[i];
                        smVisionInfo.PR_CO_PassQuantity = Convert.ToInt32(strMessage[3]);
                    }
                }

                m_smProductionInfo.AT_CO_RerunLotDone = false;
                m_smProductionInfo.CO_AT_RerunLot = true;
                m_blnProcessRerunLot = true;
            }
            catch (Exception ex)
            {
                m_objTrack.WriteLine("ComThread (ProcessRerunLotCommand) : " + ex.ToString());
                Send(m_strRerunLotFail);
            }
        }

        private void ProcessResetCommand(string[] strMessage)
        {
            m_strResetFail = "<RESETRP,NG," + strMessage[1] + ">";
            m_strResetSuccess = "<RESETRP,OK," + strMessage[1] + ">";

            try
            {
                m_smProductionInfo.g_intVisionInvolved = Convert.ToInt32(strMessage[1]);

                m_smProductionInfo.AT_CO_ResetCountDone = false;
                m_smProductionInfo.CO_AT_ResetCount = true;
                m_blnProcessReset = true;
            }
            catch (Exception ex)
            {
                m_objTrack.WriteLine("ComThread (ProcessResetCommmand): " + ex.ToString());
                Send(m_strResetFail);
            }
        }

        private void Process2DCodeCommand(string[] strMessage)
        {
            m_str2DCodeFail = "<RDMCRP,,,>";
            try
            {
                m_smProductionInfo.g_intVisionInvolved = Convert.ToInt32(strMessage[1]);

                m_smProductionInfo.AT_CO_Get2DCodeDone = false;
                m_smProductionInfo.CO_AT_Get2DCode = true;
                m_blnProcess2DCode = true;
            }
            catch (Exception ex)
            {
                m_objTrack.WriteLine("ComThread (Process2DCodeCommmand): " + ex.ToString());
                Send(m_str2DCodeFail);
            }
        }



        /// <summary>
        /// Called by the thread to indicate when it has stopped.
        /// </summary>
        private void SetStopped()
        {
            lock (m_objStopLock)
            {
                m_blnStopped = true;
            }
        }

        private void TakeAction(string strMessage)
        {
            STTrackLog.WriteLine("Inside TakeAction");

            STTrackLog.WriteLine("strMessage = " + strMessage);

            //2021-10-27 ZJYEOH : need trim special chars in the start and end of the message
            strMessage = strMessage.TrimStart(m_arrSpecialChars);
            strMessage = strMessage.TrimEnd(m_arrSpecialChars);

            if (UpdateInterfaceEvent != null) UpdateInterfaceEvent("Common", strMessage);
            STTrackLog.WriteLine("UpdateInterfaceEvent");
            if (!(strMessage.StartsWith("<") && strMessage.EndsWith(">")))
                return;
            
            STTrackLog.WriteLine("Valid Message : " + strMessage);
            string[] strString = strMessage.Substring(1, strMessage.Length - 2).Split(',');
            List<string> strNewString = new List<string>();
            string strCombined = "";
            for (int i = 1; i < strString.Length - 2; i++)
            {
                if (strCombined == "")
                    strCombined = strString[i];
                else
                    strCombined = strCombined + "," + strString[i];
            }
            if (strString.Length == 0 || (strString.Length - 2) < 0)
            {
                m_strErrorMessage = "Receive wrong format message : [" + strMessage + "]";
                m_blnShowMessageBox = true;
                return;
            }
            strNewString.Add(strString[0]);
            strNewString.Add(strCombined);
            strNewString.Add(strString[strString.Length - 2]);
            strNewString.Add(strString[strString.Length - 1]);
            switch (strString[0])
            {
                case "RERUN":
                    //Handler requests vision to rerun lot.
                    ProcessRerunCommand(strNewString.ToArray());
                    break;
                case "LD":
                    //Handler requests vision to load recipe file
                    ProcessLoadRecipeCommand(strNewString.ToArray());
                    break;
                case "NEW":
                    //Handler requests vision to new lot.
                    ProcessNewLotCommand(strNewString.ToArray());
                    break;
                case "END":
                    //Handler requests vision to end lot.
                    ProcessEndLotCommand(strNewString.ToArray());
                    break;
                case "TMPL":
                    //Handler requests vision to clear mark template.
                    ProcessDeleteTemplateCommand(strNewString.ToArray());
                    break;
                case "CLR":
                    //case "RESET":
                    //Handler requests vision to clear counter.
                    ProcessResetCommand(strNewString.ToArray());
                    break;
                case "SV":
                    //Handler requests vision to save current settings to recipe file
                    break;
                case "RUN":
                    //Handler requests vision to start production run.
                    break;
                case "STOP":
                    //Handler requests vision to stop production.
                    break;
                case "OPID":
                    //Handler requests vision to update operator ID.
                    break;
                case "RDMC":
                    Process2DCodeCommand(strNewString.ToArray());
                    break;
                case "OCR":
                    int intVisionIndex = 0;
                    if (strMessage.Length < 3 || !int.TryParse(strString[2], out intVisionIndex))
                        intVisionIndex = -1;
                    //intVisionIndex = (int)Math.Log(intVisionIndex, 2);
                    if (intVisionIndex == -1 /*|| m_smVSInfo[intVisionIndex].g_intMachineStatus != 2*/)
                        Send("<OCRRP,NG," + intVisionIndex + "," + m_smProductionInfo.g_arrSingleRecipeID[m_smVSInfo[intVisionIndex].g_intVisionIndex] + ">");
                    else
                    {
                        Send("<OCRRP,OK," + intVisionIndex + "," + m_smProductionInfo.g_arrSingleRecipeID[m_smVSInfo[intVisionIndex].g_intVisionIndex] + ">");
                    }
                    break;
            }
        }






        private void UpdateProgress()
        {
            try
            {
                while (!m_blnStopping)
                {
                    //TakeAction("<END,,0,>");
                    //TakeAction("<NEW,1022@ope6,0,1>");
                    //TakeAction("<LD,12121,0,>");        // LD command end with ",>" based on protocol.
                    //TakeAction("<NEW, yyy @SRM*20220120 02 021439,0,1 >");
                    if (m_blnShowMessageBox)
                    {
                        m_blnShowMessageBox = false;
                        SRMMessageBox.Show(m_strErrorMessage, "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (m_blnProcessReset && m_smProductionInfo.AT_CO_ResetCountDone)
                    {
                        m_blnProcessReset = false;
                        if (m_smProductionInfo.g_blnResetPass)
                            Send(m_strResetSuccess);
                        else
                            Send(m_strResetFail);
                    }

                    if (m_blnProcessNewLot && m_smProductionInfo.AT_CO_NewLotDone)
                    {
                        m_blnProcessNewLot = false;

                        if (m_smProductionInfo.g_blnNewLotPass)
                        {
                            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                            subkey1.SetValue("LotNo", m_smProductionInfo.g_strLotID);
                            for (int i = 0; i < 10; i++)
                            {
                                subkey1.SetValue("SingleLotNo" + i.ToString(), m_smProductionInfo.g_arrSingleLotID[i]);
                            }
                            subkey1.SetValue("OperatorID", m_smProductionInfo.g_strOperatorID);
                            subkey1.SetValue("LotStartTime", m_smProductionInfo.g_strLotStartTime);
                            Send(m_strNewLotSuccess);
                        }
                        else
                            Send(m_strNewLotFail);
                    }

                    if (m_blnProcessRerunLot && m_smProductionInfo.AT_CO_RerunLotDone)
                    {
                        m_blnProcessRerunLot = false;

                        if (m_smProductionInfo.g_blnRerunLotPass)
                        {
                            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                            subkey1.SetValue("LotNo", m_smProductionInfo.g_strLotID);
                            for (int i = 0; i < 10; i++)
                            {
                                subkey1.SetValue("SingleLotNo" + i.ToString(), m_smProductionInfo.g_arrSingleLotID[i]);
                            }
                            subkey1.SetValue("OperatorID", m_smProductionInfo.g_strOperatorID);
                            subkey1.SetValue("LotStartTime", m_smProductionInfo.g_strLotStartTime);
                            Send(m_strRerunLotSuccess);
                        }
                        else
                            Send(m_strRerunLotFail);
                    }

                    if (m_blnProcessEndLot && m_smProductionInfo.AT_CO_EndLotDone)
                    {
                        m_blnProcessEndLot = false;

                        if (m_smProductionInfo.g_blnEndLotPass)
                        {
                            Send(m_strEndLotSuccess);
                        }
                        else
                            Send(m_strEndLotFail);
                    }

                    if (m_blnProcessTMPL)
                    {
                        if (!m_smProductionInfo.AT_CO_DeleteTemplateDone)
                        {
                            bool blnFinished = true;
                            for (int i = 0; i < m_smVSInfo.Length; i++)
                            {
                                if (m_smVSInfo[i] == null || ((m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_intVisionInvolved != 0 && m_smProductionInfo.g_arrVisionIDIndex[i] != 0))
                                    continue;

                                if (!m_smVSInfo[i].PR_CO_DeleteTemplateDone)
                                {
                                    blnFinished = false;
                                    break;
                                }
                            }

                            if (blnFinished)
                                m_smProductionInfo.AT_CO_DeleteTemplateDone = true;
                        }
                        else
                        {
                            m_blnProcessTMPL = false;

                            bool blnResult = true;
                            for (int i = 0; i < m_smVSInfo.Length; i++)
                            {
                                if (m_smVSInfo[i] == null || ((m_smProductionInfo.g_intVisionInvolved & (1 << m_smProductionInfo.g_arrVisionIDIndex[i])) == 0 && m_smProductionInfo.g_intVisionInvolved != 0 && m_smProductionInfo.g_arrVisionIDIndex[i] != 0))
                                    continue;

                                if (!m_smVSInfo[i].PR_CO_DeleteProcessSuccess)
                                {
                                    blnResult = false;
                                    break;
                                }
                            }

                            if (blnResult)
                                Send(m_strTMPLSuccess);
                            else
                                Send(m_strTMPLFail);
                        }
                    }


                    if (m_blnProcessLoadRecipe && m_smProductionInfo.AT_CO_LoadRecipeDone)
                    {
                        m_blnProcessLoadRecipe = false;
                        if (m_smProductionInfo.g_blnLoadRecipePass)
                        {
                            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                            subkey1.SetValue("SelectedRecipeID", m_smProductionInfo.g_strRecipeID);
                            for (int i = 0; i < 10; i++)
                            {
                                subkey1.SetValue("SingleRecipeID" + i.ToString(), m_smProductionInfo.g_arrSingleRecipeID[i]);
                            }
                            Send(m_strLoadRecipeSuccess);
                        }
                        else
                            Send(m_strLoadRecipeFail);
                    }

                    if (m_blnProcess2DCode && m_smProductionInfo.AT_CO_Get2DCodeDone)
                    {
                        m_blnProcess2DCode = false;
                        if (m_smProductionInfo.g_bln2DCodePass)
                        {
                            m_str2DCodeSuccess = "<RDMCRP," + m_smProductionInfo.AT_CO_DataMatrixCode + ",,>";
                            Send(m_str2DCodeSuccess);
                        }
                        else
                            Send(m_str2DCodeFail);
                    }

                    if (m_smProductionInfo.AT_CO_Send2DCode)
                    {
                        m_smProductionInfo.AT_CO_Send2DCode = false;
                        if (m_smProductionInfo.g_bln2DCodePass)
                        {
                            m_str2DCodeSuccess = "<RDMCRP," + m_smProductionInfo.AT_CO_DataMatrixCode + ",,>";
                            Send(m_str2DCodeSuccess);
                        }
                        else
                            Send(m_str2DCodeFail);
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("ComThread: " + ex.ToString());
            }
            finally
            {
                SetStopped();
                m_thThread = null;
            }
        }
        public int GetBitIndexFromHex(int intHex)
        {
            /*
             * For example 
             * intHex is 0x01, return bit index 0
             * intHex is 0x02, return bit index 1
             * intHex is 0x04, return bit index 2
             * intHex is 0x08, return bit index 3
             * intHex is 0x10, return bit index 4
             * intHex is 0x20, return bit index 5
             */

            for (int i = 0; i < 31; i++)
            {
                if (((intHex >> i) & 0x01) > 0)
                    return i;
            }

            return 0;
        }
    }




}
