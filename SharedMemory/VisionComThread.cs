using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.IO.Ports;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using Common;
using System.Runtime.InteropServices;


namespace SharedMemory
{
    public class VisionComThread
    {
        #region Delegate Event

        public delegate void ReceiveCommandHandle(string strMessage);
        public event ReceiveCommandHandle ReceiveCommandEvent;

        #endregion
        
        #region Members Variables
 
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

        #endregion

        #region Properties

        public bool ref_blnTCPIPEnable { set { m_blnTCPIPEnable = value; } }
        public bool ref_blnGotMessage { get { return m_blnGotMessage; } set { m_blnGotMessage = value; } }

        #endregion


        public VisionComThread(ProductionInfo smProductionInfo, VisionInfo smVisionInfo, int intDefaultPort)
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
            if (objFile.GetValueAsBoolean("ConfigShowTCPIP", false) && !objFile.GetValueAsBoolean("WantUseTCPIPAsIO", false))
            {
                objFile.GetFirstSection("TCPIP");
                m_intTCPIPTimeOut = objFile.GetValueAsInt("TimeOut", 100);
                m_intPort = objFile.GetValueAsInt(m_smVisionInfo.g_strVisionName + m_smVisionInfo.g_strVisionNameNo, 8000 + m_intDefaultPort);

                if (m_blnFirstInit)
                {
                    m_objTCPServer.ReceiveEvent += new TCPServer.ReceivedDataHandle(TakeAction);
                    m_blnFirstInit = false;
                }
                m_objTCPServer.Init(m_intPort, m_intTCPIPTimeOut);
            }                 
        }

        public void Send(string strMessage)
        {
            try
            {
                if (m_blnTCPIPEnable)
                {
                    m_smVisionInfo.g_strTCPMessage += "*"+strMessage;
                    m_blnGotMessage = true;
                }
                m_objTCPServer.Send(strMessage, m_smProductionInfo.g_blnTrackTCPIP_IO);
            }
            catch (Exception ex)
            {
            }
        }
               
  


     

        private void TakeAction(string strMessage)
        {
            try
            {
                if (strMessage == "Client disconnected.")
                    if (ReceiveCommandEvent != null) ReceiveCommandEvent(strMessage);     
               
                if (m_blnTCPIPEnable)
                {
                    m_smVisionInfo.g_strTCPMessage += "*" + strMessage;                
                    m_blnGotMessage = true;
                }

                if (!(strMessage.StartsWith("<") && strMessage.EndsWith(">")))
                    return;

                if (ReceiveCommandEvent != null) ReceiveCommandEvent(strMessage);              
            }
            catch(Exception ex)
            {
                TrackLog objLog = new TrackLog();
                objLog.WriteLine(ex.ToString());
            }
        }

        

    }
}
