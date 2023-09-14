using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Common;


namespace Common
{
    public class RS232
    {
        #region DllImport

#if(RTXDebug || RTXRelease)
        [DllImport("SRMRtx.dll", CharSet = CharSet.Auto)]
        public static extern bool SRMSingleLock([MarshalAs(UnmanagedType.LPStr)] string valueName);

        [DllImport("SRMRtx.dll", CharSet = CharSet.Auto)]
        public static extern void SRMSetEvent([MarshalAs(UnmanagedType.LPStr)] string eventName);

        [DllImport("SRMRtx.dll", CharSet = CharSet.Auto)]
        public static extern void SRMResetEvent([MarshalAs(UnmanagedType.LPStr)] string eventName);

        [DllImport("SRMRtx.dll", CharSet = CharSet.Auto)]
        public static extern void SRMSetCOMString([MarshalAs(UnmanagedType.LPStr)] string valueName, [MarshalAs(UnmanagedType.LPStr)] string value);
#endif

        #endregion

        #region Delegate Event

        public delegate void ReceiveDataHandle(string strMessage);
        public event ReceiveDataHandle ReceiveEvent;

        public delegate void UpdateInterfaceHandle(string strSender, string strMessage);
        public event UpdateInterfaceHandle UpdateInterfaceEvent;

        #endregion

        #region Member Variable

        private static object m_objLock = new object();
        private static object m_objInLock = new object();

        private SerialPort comPort = new SerialPort();
       
        private string m_strDataReceived = "";

        public bool m_blnStartTest = false;
        private HiPerfTimer T1 = new HiPerfTimer();



        private string m_strACMDataReceived = "";

        #endregion

        public RS232()
        {
            comPort.DataReceived += new SerialDataReceivedEventHandler(Comm_DataReceived);
        }

        /// <summary>
        /// Check is serial port open?
        /// </summary>
        /// <returns></returns>
        public bool IsOpen()
        {
            return comPort.IsOpen;
        }

        /// <summary>
        /// Close serial port connection
        /// </summary>
        public void CloseConnection()
        {
            if (comPort.IsOpen)
                comPort.Close();
        }
        /// <summary>
        /// init connection according to settings
        /// </summary>
        /// <param name="strPortName">Com port name such as COM1, COM2,....</param>
        /// <param name="intBaudRate">baud rate-57600</param>
        /// <param name="intDataBits">Data Bit-8</param>
        /// <param name="parity">parity-none</param>
        /// <param name="stopBits">Stop Bits-1</param>        
        public void Init(string strPortName, int intBaudRate, int intDataBits, int parity, int stopBits)
        {
            if (comPort.IsOpen)
                comPort.Close();

            try
            {
                comPort.PortName = strPortName;
            }
            catch
            {
                MessageBox.Show("Failed to Open " + strPortName, "COM ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            comPort.BaudRate = intBaudRate;
            comPort.DataBits = intDataBits;
            switch (parity)
            {
                case 0: comPort.Parity = Parity.Even;
                    break;
                case 1: comPort.Parity = Parity.Odd;
                    break;
                case 3: comPort.Parity = Parity.Mark;
                    break;
                case 4: comPort.Parity = Parity.Space;
                    break;
                default: comPort.Parity = Parity.None;
                    break;
            }

            switch (stopBits)
            {
                case 0: comPort.StopBits = StopBits.One;
                    break;
                case 1: comPort.StopBits = StopBits.OnePointFive;
                    break;
                case 2: comPort.StopBits = StopBits.Two;
                    break;
            }

            //comPort.BytesToWrite
            //comPort.DataBits
            //comPort.WriteBufferSize = 9999999;

                        
            // Open the com port
            try
            {
                comPort.Open();
                Thread.Sleep(100);
                if (!comPort.IsOpen)
                   SRMMessageBox.Show("Failed to Open Com Port " + strPortName,
                        "COM ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Failed to Open Com Port " + strPortName + ". Exception: " + ex.ToString(),
                    "COM ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// send message from vision PC to handler PC
        /// </summary>
        /// <param name="data">message</param>
        public void SendData(string data)
        {
            lock (m_objLock)
            {
                if (!comPort.IsOpen)
                {
                    SRMMessageBox.Show("Com Port Connection NOT Open. Fail to send data " + data,
                        "COM ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (UpdateInterfaceEvent != null) UpdateInterfaceEvent(comPort.PortName, data);
                comPort.Write(data);
                //comPort.DiscardOutBuffer();
                T1.Stop();
                float f = T1.Duration;
            }
        }
        
#if (RTXDebug || RTXRelease)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="intDataIndex">Index start with 1...</param>
        public static void RTXSendData(string data, int intDataIndex)
        {
            while (!SRMSingleLock("SendDataDone" + intDataIndex))
            {
                Thread.Sleep(1);
            }

            SRMSetCOMString("uCOMSendData" + intDataIndex, data);
            SRMResetEvent("SendDataDone" + intDataIndex);
            SRMSetEvent("SendDataStart" + intDataIndex);
        }

#endif
        /// <summary>
        /// alwasys listen to port and is triggered when receiving any data
        /// </summary>
        /// <param name="sender">serial port</param>
        /// <param name="e">object event</param>
        private void Comm_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string commInput = comPort.ReadExisting();

            if (m_strDataReceived == "")
            {
                T1.Start();
            }
            
            if (commInput != "" && commInput != null)
            {
                m_strDataReceived += commInput;

                if ((m_strDataReceived.StartsWith("<") & m_strDataReceived.EndsWith(">")) || m_strDataReceived.IndexOf("<") < 0)
                {
                    if (ReceiveEvent!= null) ReceiveEvent(m_strDataReceived);
                    m_strDataReceived = "";
                    //T1.Start();
                    m_blnStartTest = true;

                }
            }

            

        }

    }
}
