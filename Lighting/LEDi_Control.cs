using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using Microsoft.Win32;
using System.Windows;
using Common;

namespace Lighting
{
    public class LEDi_Control
    {
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
            lock (m_objLock)
            {
                for (int i = 0; i < m_arrLightPort.Count; i++)
                {
                    if (m_arrLightPort[i] != null)
                    {
                        m_arrLightPort[i].Close();
                        if (Key != null) Key.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Initialize light source and open all light source
        /// </summary>
        public static void Init()
        {
            lock (m_objLock)
            {
                Key = Registry.LocalMachine.CreateSubKey(@"Software\SVG\LightControl");

                string[] arrName = Key.GetSubKeyNames();
                foreach (string strName in arrName)
                {
                    try
                    {
                        bool blnFound = false;
                        int intPortNo = 0;
                        for (int i = 0; i < m_arrLightPort.Count; i++)
                        {
                            if (m_arrLightPort[i].PortName == strName.ToUpper())
                            {
                                blnFound = true;
                                intPortNo = i;
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
                        }
                        else
                            continue;

                        if (m_arrLightPort[intPortNo].IsOpen)
                            m_arrLightPort[intPortNo].Close();

                        m_arrLightPort[intPortNo].WriteTimeout = 100;
                        m_arrLightPort[intPortNo].Open();
                    }
                    catch (Exception ex)
                    {
                        STTrackLog.WriteLine("LEDi_Control.cs > Init() > Exception: " + ex.ToString());
                        m_arrLightPort.RemoveAt(m_arrLightPort.Count - 1);
                    }
                }
            }
        }

        /// <summary>
        /// Reset intensity of the particular light source channel
        /// </summary>
        /// <param name="intComNo">com port no</param>
        /// <param name="Channel">channel</param>
        public static void ResetIntensity(int intComNo, int Channel)
        {
            lock (m_objLock)
            {
                //if (m_arrLightPort.Count <= intComNo)
                //    return;

                //byte byte1 = (byte)((byte)0xa0 | (byte)(Channel));
                //byte[] m_byte = new byte[] { byte1 };

                //try
                //{
                //    if (!m_arrLightPort[intComNo].IsOpen)
                //        m_arrLightPort[intComNo].Open();
                //    m_arrLightPort[intComNo].Write(m_byte, 0, 1);
                //}
                //catch (TimeoutException ex)
                //{
                //    TrackLog objTrack = new TrackLog();
                //    objTrack.WriteLine(ex.ToString());
                //}

                //System.Threading.Thread.Sleep(2);

                int intLightPortIndex = -1;
                for (int i = 0; i < m_arrLightPort.Count; i++)
                {
                    if (m_arrLightPort[i].PortName == ("COM" + (intComNo + 1).ToString()))
                    {
                        intLightPortIndex = i;
                        break;
                    }
                }
                if (intLightPortIndex == -1)
                    return;

                byte byte1 = (byte)((byte)0xa0 | (byte)(Channel));
                byte[] m_byte = new byte[] { byte1 };

                try
                {
                    if (!m_arrLightPort[intLightPortIndex].IsOpen)
                        m_arrLightPort[intLightPortIndex].Open();
                    m_arrLightPort[intLightPortIndex].Write(m_byte, 0, 1);
                }
                catch (TimeoutException ex)
                {
                    TrackLog objTrack = new TrackLog();
                    objTrack.WriteLine(ex.ToString());
                }

                System.Threading.Thread.Sleep(2);
            }
        }

        /// <summary>
        /// Set intensity of the particular light source channel
        /// </summary>
        /// <param name="intComNo">com port no</param>
        /// <param name="Channel">channel</param>
        /// <param name="Intensity">intensity value</param>
        public static void SetIntensity(int intComNo, int Channel, byte Intensity)
        {
            lock (m_objLock)
            {
                //if (m_arrLightPort.Count <= intComNo)
                //    return;

                //byte byte1 = (byte)((byte)0xa0 | (byte)(Channel) | (byte)0x10);
                //byte byte2 = (byte)Intensity;
                //byte[] m_byte = new byte[] { byte1, byte2 };

                //try
                //{
                //    if (!m_arrLightPort[intComNo].IsOpen)
                //        m_arrLightPort[intComNo].Open();
                //    m_arrLightPort[intComNo].Write(m_byte, 0, 2);
                //}
                //catch (TimeoutException ex)
                //{
                //    TrackLog objTrack = new TrackLog();
                //    objTrack.WriteLine(ex.ToString());
                //}

                ////System.Threading.Thread.Sleep(2);

                int intLightPortIndex = -1;
                for (int i = 0; i < m_arrLightPort.Count; i++)
                {
                    if (m_arrLightPort[i].PortName == ("COM" + (intComNo + 1).ToString()))
                    {
                        intLightPortIndex = i;
                        break;
                    }
                }
                if (intLightPortIndex == -1)
                    return;

                byte byte1 = (byte)((byte)0xa0 | (byte)(Channel) | (byte)0x10);
                byte byte2 = (byte)Intensity;
                byte[] m_byte = new byte[] { byte1, byte2 };

                try
                {
                    if (!m_arrLightPort[intLightPortIndex].IsOpen)
                        m_arrLightPort[intLightPortIndex].Open();
                    m_arrLightPort[intLightPortIndex].Write(m_byte, 0, 2);
                }
                catch (TimeoutException ex)
                {
                    TrackLog objTrack = new TrackLog();
                    objTrack.WriteLine(ex.ToString());
                }

                //System.Threading.Thread.Sleep(2);
            }
        }



        /// <summary>
        /// Get the port no of specific light source
        /// </summary>
        /// <param name="strComName">light source port name</param>
        /// <returns>0 = light source name not found, otherwise = light source port no</returns>
        public static int GetPortNo(string strComName)
        {
            lock (m_objLock)
            {
                for (int i = 0; i < m_arrLightPort.Count; i++)
                {
                    if (m_arrLightPort[i].PortName == strComName)
                        return i;
                }

                return 0;
            }
        }

        // ----------------------------------------------------------- Sequential Light Controller --------------------------------------------------------------------


        /// <summary>
        /// Set to run or stop all sequences
        /// </summary>
        /// <param name="intComNo">com port no</param>
        /// <param name="intSequenceID">Sequence ID from 1 to 8, max 8 sequences are available</param>
        /// <param name="blnRun">true = run, false = stop</param>
        public static void RunStop(int intComNo, int intSequenceID, bool blnRun)
        {
            lock (m_objLock)
            {
                //if (m_arrLightPort.Count <= intComNo)
                //    return;

                //byte byte1 = (byte)((byte)0xa0 + intSequenceID);
                //byte byte2 = 0x57;
                //byte byte3;
                //if (blnRun)
                //{
                //    byte3 = 0x52;
                //}
                //else
                //{
                //    byte3 = 0x54;
                //}

                //byte[] m_byte = new byte[] { byte1, byte2, byte3 };

                //if (!m_arrLightPort[intComNo].IsOpen)
                //    m_arrLightPort[intComNo].Open();
                //m_arrLightPort[intComNo].Write(m_byte, 0, 3);

                int intLightPortIndex = -1;
                for (int i = 0; i < m_arrLightPort.Count; i++)
                {
                    if (m_arrLightPort[i].PortName == ("COM" + (intComNo + 1).ToString()))
                    {
                        intLightPortIndex = i;
                        break;
                    }
                }
                if (intLightPortIndex == -1)
                    return;

                byte byte1 = (byte)((byte)0xa0 + intSequenceID);
                byte byte2 = 0x57;
                byte byte3;
                if (blnRun)
                {
                    byte3 = 0x52;
                }
                else
                {
                    byte3 = 0x54;
                }

                byte[] m_byte = new byte[] { byte1, byte2, byte3 };

                if (!m_arrLightPort[intLightPortIndex].IsOpen)
                    m_arrLightPort[intLightPortIndex].Open();
                m_arrLightPort[intLightPortIndex].Write(m_byte, 0, 3);
            }
        }

        /// <summary>
        /// Save intensity when save button is pressed
        /// </summary>
        /// <param name="intComNo">com port no</param>
        /// <param name="intSequenceID">Sequence ID from 1 to 8, max 8 sequences are available</param>
        public static void SaveIntensity(int intComNo, int intSequenceID)
        {
            lock (m_objLock)
            {
                //if (m_arrLightPort.Count <= intComNo)
                //    return;

                //byte byte1 = (byte)((byte)0xa0 + intSequenceID);
                //byte byte2 = 0x57;
                //byte byte3 = 0x53;

                //byte[] m_byte = new byte[] { byte1, byte2, byte3 };

                //try
                //{
                //    if (!m_arrLightPort[intComNo].IsOpen)
                //        m_arrLightPort[intComNo].Open();
                //    m_arrLightPort[intComNo].Write(m_byte, 0, 3);
                //}
                //catch (TimeoutException ex)
                //{
                //    TrackLog objTrack = new TrackLog();
                //    objTrack.WriteLine(ex.ToString());
                //}

                int intLightPortIndex = -1;
                for (int i = 0; i < m_arrLightPort.Count; i++)
                {
                    if (m_arrLightPort[i].PortName == ("COM" + (intComNo + 1).ToString()))
                    {
                        intLightPortIndex = i;
                        break;
                    }
                }
                if (intLightPortIndex == -1)
                    return;

                byte byte1 = (byte)((byte)0xa0 + intSequenceID);
                byte byte2 = 0x57;
                byte byte3 = 0x53;

                byte[] m_byte = new byte[] { byte1, byte2, byte3 };

                try
                {
                    if (!m_arrLightPort[intLightPortIndex].IsOpen)
                        m_arrLightPort[intLightPortIndex].Open();
                    m_arrLightPort[intLightPortIndex].Write(m_byte, 0, 3);
                }
                catch (TimeoutException ex)
                {
                    TrackLog objTrack = new TrackLog();
                    objTrack.WriteLine(ex.ToString());
                }
            }

        }

        /// <summary>
        /// Set sequence intensity. For sequence light controller, have to set all the light source intensity at once
        /// </summary>
        /// <param name="intComNo">com port no</param>
        /// <param name="intSequenceID">Sequence ID from 1 to 8, max 8 sequences are available</param>
        /// <param name="intSeq">sequence number</param>
        /// <param name="intIntensity1">intensity value for channel l</param>
        /// <param name="intIntensity2">intensity value for channel 2</param>
        /// <param name="intIntensity3">intensity value for channel 3</param>
        /// <param name="intIntensity4">intensity value for channel 4</param>
        public static void SetSeqIntensity(int intComNo, int intSequenceID, int intSeq, int intIntensity1, int intIntensity2, int intIntensity3, int intIntensity4)
        {
            lock (m_objLock)
            {
                //if (m_arrLightPort.Count <= intComNo)
                //    return;

                //byte byte1 = (byte)((byte)0xa0 + intSequenceID);
                //byte byte2 = 0x57;
                //byte byte3 = (byte)intSeq;
                //byte byte4 = (byte)intIntensity1;
                //byte byte5 = (byte)intIntensity2;
                //byte byte6 = (byte)intIntensity3;
                //byte byte7 = (byte)intIntensity4;


                //byte[] m_byte = new byte[] { byte1, byte2, byte3, byte4, byte5, byte6, byte7 };

                //try
                //{
                //    if (!m_arrLightPort[intComNo].IsOpen)
                //        m_arrLightPort[intComNo].Open();
                //    m_arrLightPort[intComNo].Write(m_byte, 0, 7);
                //}
                //catch (TimeoutException ex)
                //{
                //    TrackLog objTrack = new TrackLog();
                //    objTrack.WriteLine(ex.ToString());
                //}

                STTrackLog.WriteLine("m_arrLightPort count = " + m_arrLightPort.Count.ToString());
                int intLightPortIndex = -1;
                for (int i = 0; i < m_arrLightPort.Count; i++)
                {
                    STTrackLog.WriteLine("Loop " + i.ToString() + " with port name " + m_arrLightPort[i].PortName);
                    if (m_arrLightPort[i].PortName == ("COM" + (intComNo + 1).ToString()))
                    {
                        STTrackLog.WriteLine("Matched Port");
                        intLightPortIndex = i;
                        break;
                    }
                }

                STTrackLog.WriteLine("Confirm value before to write to port [" + intComNo.ToString() + "] Seq [" + intSeq.ToString() + "]=[" + intIntensity1.ToString() + "][" + intIntensity2.ToString() + "][" + intIntensity3.ToString() + "][" + intIntensity4.ToString() + "]");

                if (intLightPortIndex == -1)
                    return;

                STTrackLog.WriteLine("Get ready to write to port step 1");
                byte byte1 = (byte)((byte)0xa0 + intSequenceID);
                byte byte2 = 0x57;
                byte byte3 = (byte)intSeq;
                byte byte4 = (byte)intIntensity1;
                byte byte5 = (byte)intIntensity2;
                byte byte6 = (byte)intIntensity3;
                byte byte7 = (byte)intIntensity4;


                byte[] m_byte = new byte[] { byte1, byte2, byte3, byte4, byte5, byte6, byte7 };

                try
                {
                    STTrackLog.WriteLine("Get ready to write to port step 2");
                    if (!m_arrLightPort[intLightPortIndex].IsOpen)
                        m_arrLightPort[intLightPortIndex].Open();

                    STTrackLog.WriteLine("Get ready to write to port step 3. Port name - " + m_arrLightPort[intLightPortIndex].PortName);
                    m_arrLightPort[intLightPortIndex].Write(m_byte, 0, 7);

                    STTrackLog.WriteLine("Get ready to write to port step 4");
                }
                catch (TimeoutException ex)
                {
                    TrackLog objTrack = new TrackLog();
                    objTrack.WriteLine(ex.ToString());
                }
            }
        }

        public static void SetAddress(int intComNo, int intSequenceIDFrom, int intSequenceIDTo)
        {
            lock (m_objLock)
            {
                //if (m_arrLightPort.Count <= intComNo)
                //    return;

                //byte byte1 = (byte)((byte)0xa0 + intSequenceIDFrom);
                //byte byte2 = 0x4A;
                //byte byte3 = (byte)((byte)0x00 + intSequenceIDTo);

                //byte[] m_byte = new byte[] { byte1, byte2, byte3 };

                //try
                //{
                //    if (!m_arrLightPort[intComNo].IsOpen)
                //        m_arrLightPort[intComNo].Open();
                //    m_arrLightPort[intComNo].Write(m_byte, 0, 3);
                //}
                //catch (TimeoutException ex)
                //{
                //    TrackLog objTrack = new TrackLog();
                //    objTrack.WriteLine(ex.ToString());
                //}

                int intLightPortIndex = -1;
                for (int i = 0; i < m_arrLightPort.Count; i++)
                {
                    if (m_arrLightPort[i].PortName == ("COM" + (intComNo + 1).ToString()))
                    {
                        intLightPortIndex = i;
                        break;
                    }
                }
                if (intLightPortIndex == -1)
                    return;

                byte byte1 = (byte)((byte)0xa0 + intSequenceIDFrom);
                byte byte2 = 0x4A;
                byte byte3 = (byte)((byte)0x00 + intSequenceIDTo);

                byte[] m_byte = new byte[] { byte1, byte2, byte3 };

                try
                {
                    if (!m_arrLightPort[intLightPortIndex].IsOpen)
                        m_arrLightPort[intLightPortIndex].Open();
                    m_arrLightPort[intLightPortIndex].Write(m_byte, 0, 3);
                }
                catch (TimeoutException ex)
                {
                    TrackLog objTrack = new TrackLog();
                    objTrack.WriteLine(ex.ToString());
                }
            }
        }

        public static void UpdateIntensityValueAccordingToImageDisplayMode(int intImageDisplayMode, int intImageIndex, ref int intValue1, ref int intValue2, ref int intValue3, ref int intValue4,
                                                                        ref int intValue5, ref int intValue6, ref int intValue7, ref int intValue8)
        {
            // 2020-10-10 ZJYEOH : image view format refer Z:\Backup\SRM Optical Vision Systems\MISC Doc\SRMVision Programing Guidline\Lighting 8 Channel Rename.xls
            switch (intImageDisplayMode)
            {
                case 0: // Standard

                    break;
                case 1: // Pad
                    {
                        switch (intImageIndex)
                        {
                            case 1: // Top Left
                                //Channel 1 remain, Channel 2 Set zero
                                intValue2 = 0;

                                //Channel 3 remain, Channel 4 Set zero
                                intValue4 = 0;

                                //Channel 5 remain, Channel 6 Set zero
                                intValue6 = 0;

                                //Channel 7 remain, Channel 8 Set zero
                                intValue8 = 0;
                                break;
                            case 2: // Bottom Right
                                //Channel 1 Set zero, Channel 2 remain
                                intValue1 = 0;

                                //Channel 3 Set zero, Channel 4 remain
                                intValue3 = 0;

                                //Channel 5 Set zero, Channel 6 remain
                                intValue5 = 0;

                                //Channel 7 Set zero, Channel 8 remain
                                intValue7 = 0;
                                break;
                            case 0: // Center
                            default: // Other
                                //Channel 1 & 2 Combine
                                intValue2 = intValue1;

                                //Channel 3 & 4 Combine
                                intValue4 = intValue3;

                                //Channel 5 & 6 Combine
                                intValue6 = intValue5;

                                //Channel 7 & 8 Combine
                                intValue8 = intValue7;

                                break;
                        }
                    }
                    break;
                case 2: // Lead
                    {
                        switch (intImageIndex)
                        {
                            // 2020-10-10 ZJYEOH : Side Light will trigger in opposite direction for Lead3D
                            case 1:
                                //Channel 2 & 4 Combine, Channel 1 & 3 set to zero
                                intValue1 = 0;
                                intValue3 = 0;

                                //Channel 5 remain, Channel 6 set to zero
                                intValue6 = 0;

                                //Channel 7 remain, Channel 8 set to zero
                                intValue8 = 0;
                                break;
                            case 2:
                                //Channel 1 & 3 Combine, Channel 2 & 4 set to zero
                                intValue2 = 0;
                                intValue4 = 0;

                                //Channel 6 remain, Channel 5 set to zero
                                intValue5 = 0;

                                //Channel 8 remain, Channel 7 set to zero
                                intValue7 = 0;
                                break;
                            case 0:
                            default:
                                //Channel 1 & 2 & 3 & 4 Combine
                                intValue4 = intValue3 = intValue2 = intValue1;

                                //Channel 5 & 6 Combine
                                intValue6 = intValue5;

                                //Channel 7 & 8 Combine
                                intValue8 = intValue7;
                                break;
                        }
                    }
                    break;
            }

        }
    }
}
