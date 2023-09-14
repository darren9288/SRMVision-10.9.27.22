using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using Microsoft.Win32;
using Common;

namespace Lighting
{
    public class TCOSIO_Control
    {
        #region Delegate Event

        public delegate void UserInterfaceDataEventHandler(string strSender, string strMessage);
        public static event UserInterfaceDataEventHandler UserInterfaceData;

        public delegate void TriggeringDataEventHandler(int intCom, int intChannel);
        public static event TriggeringDataEventHandler TriggeringData;
        #endregion

        #region Member Variables

        private static List<SerialPort> m_arrLightPort = new List<SerialPort>();
        private static RegistryKey Key = Registry.LocalMachine.OpenSubKey(@"Software\SVG\LightControl", true);

        private static object m_objLock = new object();
        #endregion

        #region Properties

        public static int ref_intComNo { get { return m_arrLightPort.Count; } }

        #endregion


        /// <summary>
        /// Close all light source port and light control registry key
        /// </summary>
        public static void Close()
        {
            for (int i = 0; i < m_arrLightPort.Count; i++)
            {
                if (m_arrLightPort[i] != null)
                {
                    m_arrLightPort[i].Close();
                    if (Key != null)  Key.Close();
                }
            }                
        }

        /// <summary>
        /// Get the port no of specific light source
        /// </summary>
        /// <param name="strComName">light source port name</param>
        /// <returns>0 = light source name not found, otherwise = light source port no</returns>
        public static int GetPortNo(string strComName)
        {
            for (int i = 0; i < m_arrLightPort.Count; i++)
            {
                if (m_arrLightPort[i].PortName == strComName)
                    return i;
            }

            return 0;
        }

        /// <summary>
        /// Initialize light source and open all light source
        /// </summary>
        public static void Init()
        {
            XmlParser fileHandle = new XmlParser(AppDomain.CurrentDomain.BaseDirectory + "Option.xml");
            fileHandle.GetFirstSection("LightSource");
            int intTimeOut = fileHandle.GetValueAsInt("RS232TimeOut", 100);
            int intBaudRate = fileHandle.GetValueAsInt("BitsPerSecond", 57600);
            int intDataBits = fileHandle.GetValueAsInt("DataBits", 8);
            int intParity = fileHandle.GetValueAsInt("Parity", 0);
            int intStopBits = fileHandle.GetValueAsInt("StopBits", 0);
           
            Key = Registry.LocalMachine.CreateSubKey(@"Software\SVG\LightControl");

            string[] arrName = Key.GetSubKeyNames();
            foreach (string strName in arrName)
            {
                bool blnFound = false;
                int intPortNo = 0;
                for (int i = 0; i < m_arrLightPort.Count; i++)
                {
                    if (m_arrLightPort[i].PortName == strName.ToUpper())
                    {
                        blnFound = true;
                        intPortNo = i;
                        break;
                    }
                }

                if (!blnFound)
                {
                    bool blnAvailable = false;
                    string[] arrPorts = SerialPort.GetPortNames();
                    for (int x = 0; x < arrPorts.Length; x++)
                    {
                        if (arrPorts[x].Equals(strName.ToUpper()))
                        {
                            blnAvailable = true;
                            break;
                        }
                    }

                    if (!blnAvailable)
                        continue;

                    SerialPort objPort = new SerialPort();
                    objPort.PortName = strName.ToUpper();
                    intPortNo = m_arrLightPort.Count;
                    m_arrLightPort.Add(objPort);
                    m_arrLightPort[intPortNo].DataReceived += new SerialDataReceivedEventHandler(Comm_DataReceived);
                }
                else
                    continue;

                if (m_arrLightPort[intPortNo].IsOpen)
                    m_arrLightPort[intPortNo].Close();

                m_arrLightPort[intPortNo].WriteTimeout = intTimeOut;
                m_arrLightPort[intPortNo].BaudRate = intBaudRate;
                m_arrLightPort[intPortNo].DataBits = intDataBits;                    
                m_arrLightPort[intPortNo].ReadTimeout = SerialPort.InfiniteTimeout;

                switch (intParity)
                {
                    case 0: m_arrLightPort[intPortNo].Parity = Parity.Even;
                        break;
                    case 1: m_arrLightPort[intPortNo].Parity = Parity.Odd;
                        break;
                    case 3: m_arrLightPort[intPortNo].Parity = Parity.Mark;
                        break;
                    case 4: m_arrLightPort[intPortNo].Parity = Parity.Space;
                        break;
                    default: m_arrLightPort[intPortNo].Parity = Parity.None;
                        break;
                }
                switch (intStopBits)
                {
                    case 0: m_arrLightPort[intPortNo].StopBits = StopBits.One;
                        break;
                    case 1: m_arrLightPort[intPortNo].StopBits = StopBits.OnePointFive;
                        break;
                    case 2: m_arrLightPort[intPortNo].StopBits = StopBits.Two;
                        break;
                }

                m_arrLightPort[intPortNo].Open();

                SendMessage(intPortNo, "@SV0*");
                System.Threading.Thread.Sleep(3);
                SendMessage(intPortNo, "@SR1*");               
            } 
        }

        /// <summary>
        /// Send vision command through TCPIP
        /// </summary>
        /// <param name="intComNo"></param>
        /// <param name="strMessage"></param>
        public static void SendMessage(int intComNo, string strMessage)
        {
            if (m_arrLightPort.Count <= intComNo)
                return;

            m_arrLightPort[intComNo].Write(strMessage);
            if (null != UserInterfaceData) UserInterfaceData("SRM Vision", m_arrLightPort[intComNo].PortName + " " + strMessage);
        }

        /// <summary>
        /// Set intensity of the particular light source channel
        /// </summary>
        /// <param name="intComNo">com port no</param>
        /// <param name="intChannel">channel</param>
        /// <param name="intIntensity">intensity value</param>
        public static void SetIntensity(int intComNo, int intChannel, int intIntensity)
        {
            if (m_arrLightPort.Count <= intComNo)
                return;

            lock (m_objLock)
            {
                string strValue = intIntensity.ToString("X2");

                strValue = "@SS" + intChannel + strValue + "*";

                try
                {
                    if (!m_arrLightPort[intComNo].IsOpen)
                        m_arrLightPort[intComNo].Open();
                    m_arrLightPort[intComNo].Write(strValue);
                    if (null != UserInterfaceData) UserInterfaceData("SRM Vision", strValue);
                }
                catch (TimeoutException ex)
                {
                    TrackLog objTrack = new TrackLog();
                    objTrack.WriteLine(ex.ToString());
                }
            }
        }
        
       

        /// <summary>
        /// Send light source controller command
        /// </summary>
        private static void Comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            for (int i = 0; i < m_arrLightPort.Count; i++)
            {
                string strInput = m_arrLightPort[i].ReadExisting();

                if (strInput != "" && strInput != null && strInput.Length < 10)
                {
                    if (null != UserInterfaceData) UserInterfaceData("Light Controller", m_arrLightPort[i].PortName + " " + strInput);
                }

                if (strInput.IndexOf("@GD") >= 0 && strInput.IndexOf("*") > 0)
                {
                    if (null != TriggeringData) TriggeringData(i, Convert.ToInt32(strInput.Substring(strInput.IndexOf("@GD"), 1)));
                }
            }            
        }

    }
}
