using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Common;

namespace IOMode
{
    public class IO
    {
        #region Constant Variables

        // Each IO Card Input/Output has 2 channel
        private static int NUM_CHANNEL = 2;
        #endregion

        #region Member Variables

        private static int iRet;
        private static uint uiInputData = 0;
        private static bool m_blnPCI64C = false;
        private static bool m_blnUSBIOCard = false;
        private static int NUM_IO_CARD = 0;
        private static byte m_byteMask = 1;
        private static byte[][] m_outputByte;
        private static int[][] m_inputByte;

        private static int[] m_intVieBoardIndex;
        private static int[][] m_intVieBoardChannel;
        private static List<VIE.Board> m_vieBoards = new List<VIE.Board>();
        private static Common.HiPerfTimer IOTimer = new Common.HiPerfTimer();
        private static Common.HiPerfTimer IOTimer2 = new Common.HiPerfTimer();

        private static Thread StartScan;
        private static object m_objLock = new object();
        private static object m_objInLock = new object();

        //private static VieUSB8IO m_objVieUSB8IO;

        #endregion

        #region Properties

        public static int ref_NUM_CHANNEL { get { return NUM_CHANNEL; } }

        #endregion

        /// <summary>
        /// Call from Main From once application start run
        /// </summary>
        /// <returns>successful return true</returns>
        public static bool StartScanInput(bool blnUSBIOCard)
        {
            /*
             * PCI 16IO card: There are 2 channel Input and Output in this card
             * PCI 64C card : Thare are 64 chanel Input and Output in this card
             * USB IO Card: There are 1 channel Input and Output in this card
             */
            try
            {
                m_blnUSBIOCard = blnUSBIOCard;

                if (!m_blnUSBIOCard)
                {
                    VIE.BoardManager vieManager = VIE.BoardManager.Instance;
                    m_vieBoards = vieManager.GetAvailableBoards();

                    // Check is new PCI 64C IO card
                    for (int i = 0; i < m_vieBoards.Count; i++)
                    {
                        // Other PCI IO card will be ignored if PCI 64C exist
                        if (m_vieBoards[i].BoardType.Name == "PCI 64C")
                        {
                            m_blnPCI64C = true;
                            break;
                        }
                    }

                    if (m_blnPCI64C)
                    {
                        for (int i = 0; i < m_vieBoards.Count; i++)
                        {
                            // Other PCI IO card will be remove if PCI 64C exist
                            if (m_vieBoards[i].BoardType.Name != "PCI 64C")
                            {
                                m_vieBoards.RemoveAt(i);
                                i--;
                            }
                        }

                        // Set max total card(virtual) to 2, but each card index num is 0 (because all card are from same physical card)
                        NUM_IO_CARD = 2;
                        m_intVieBoardIndex = new int[NUM_IO_CARD];
                        m_intVieBoardChannel = new int[NUM_IO_CARD][];
                        for (int i = 0; i < NUM_IO_CARD; i++)
                        {
                            m_intVieBoardIndex[i] = 0;
                            m_intVieBoardChannel[i] = new int[NUM_CHANNEL];
                            // channel index is from 0 to 3
                            for (int j = 0; j < NUM_CHANNEL; j++)
                                m_intVieBoardChannel[i][j] = i * 2 + j;
                        }
                    }
                    else
                    {
                        //Note* m_vieBoards array is based on Slot No.

                        // Set total card based on available card
                        NUM_IO_CARD = m_vieBoards.Count;
                        STTrackLog.WriteLine("Total IO card = " + NUM_IO_CARD);

                        m_intVieBoardIndex = new int[NUM_IO_CARD];
                        m_intVieBoardChannel = new int[NUM_IO_CARD][];
                        for (int i = 0; i < NUM_IO_CARD; i++)
                        {
                            m_intVieBoardIndex[i] = i;
                            m_intVieBoardChannel[i] = new int[NUM_CHANNEL];
                            // channel index is from 0 to 1 each card
                            for (int j = 0; j < NUM_CHANNEL; j++)
                                m_intVieBoardChannel[i][j] = j;
                        }

                    }

                    //create byte array for input and output
                    m_outputByte = new byte[NUM_IO_CARD][];
                    m_inputByte = new int[NUM_IO_CARD][];
                    for (int cardNo = 0; cardNo < NUM_IO_CARD; cardNo++)
                    {
                        m_outputByte[cardNo] = new byte[NUM_CHANNEL];
                        m_inputByte[cardNo] = new int[NUM_CHANNEL];
                    }

                    if (m_vieBoards.Count == 0)
                        return false;
                }
                else
                {
                    //if (m_objVieUSB8IO == null)
                    //{
                    //    m_objVieUSB8IO = new VieUSB8IO(3);
                    //}

                    //if (!m_objVieUSB8IO.Connect())
                    //{
                    //    SRMMessageBox.Show("Failed to connect!");
                    //    return false;
                    //}

                    NUM_IO_CARD = 1;
                    NUM_CHANNEL = 1;
                    m_blnUSBIOCard = true;

                    //create byte array for input and output
                    m_outputByte = new byte[NUM_IO_CARD][];
                    m_inputByte = new int[NUM_IO_CARD][];
                    for (int cardNo = 0; cardNo < NUM_IO_CARD; cardNo++)
                    {
                        m_outputByte[cardNo] = new byte[NUM_CHANNEL];
                        m_inputByte[cardNo] = new int[NUM_CHANNEL];
                    }
                }




            }
            catch (Exception ex)
            {
                SRMMessageBox.Show(ex.ToString(), "Get Available Boards Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //List<string> arrThreadNameBF = new List<string>();
            //List<string> arrThreadNameAF = new List<string>();
            //arrThreadNameBF = ProcessTh.GetThreadsName("SRMVision");

            //create thread name "ScanInput" to scan input status
            StartScan = new Thread(ScanInput);
            StartScan.Priority = ThreadPriority.AboveNormal;
            StartScan.Start();

            //Thread.Sleep(500);
            //arrThreadNameAF = ProcessTh.GetThreadsName("SRMVision");
            //ProcessTh.GetDifferentThreadsName(arrThreadNameAF, arrThreadNameBF, "3", 0x02);

            return true;
        }

        /// <summary>
        /// Get whole input byte status
        /// </summary>
        public static int[][] InputByte
        {
            get { return m_inputByte; }
        }

        /// <summary>
        /// Get whole output byte status
        /// </summary>
        public static byte[][] OutputByte
        {
            get { return m_outputByte; }
        }

        /// <summary>
        /// Stop io scan
        /// </summary>
        public static void StopScanInput()
        {
            if (StartScan != null)
                if (StartScan.IsAlive)
                    StartScan.Abort();

            ResetAllOutPort();
        }

        /// <summary>
        /// Read input status from input byte array
        /// </summary>
        /// <param name="cardNo">card no</param>
        /// <param name="channel">channel no</param>
        /// <param name="bit">bit to check</param>
        /// <param name="bitStatus">bit status to check 0/1 off/on</param>
        /// <returns>true if condition meet</returns>
        // 2020 12 30 - CCENG:  Add blnDefaultValue parameter bcos sometime card no exist during release. 
        //                      If always return false, will make cause inspection fail or pass with weird condition.
        //public static bool InPort(int cardNo, int channel, int bit, int bitStatus)
        public static bool InPort(int cardNo, int channel, int bit, int bitStatus, bool blnDefaultValue)    
        {
            if (cardNo < 0) // 2019 06 11 - cardNo value is -1 if this IO name no exist in IO Map database.
                return blnDefaultValue;

            lock (m_objInLock)
            {
                //avoid miss match of card no
                if (cardNo >= NUM_IO_CARD)
                {
                    //SRMMessageBox.Show("INVALID Card Number.", "IO Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return blnDefaultValue;
                }

                channel -= (cardNo * NUM_CHANNEL);

                if (channel >= m_outputByte[cardNo].Length)
                {
                    return blnDefaultValue;
                }

                bool isTrue = false; // indicate bitStatus checked is TRUE or FALSE
                int mask = 1 << bit;
                if ((m_inputByte[cardNo][channel] & mask) > 0)
                    isTrue = true && (bitStatus == 1);
                else
                    isTrue = false || (bitStatus == 0);

                return isTrue;
            }
        }

        /// <summary>
        /// Set output status 
        /// </summary>
        /// <param name="cardNo"></param>
        /// <param name="channel"></param>
        /// <param name="bit"></param>
        /// <param name="bitStatus"></param>
        public static void OutPort(int cardNo, int channel, int bit, int bitStatus)
        {
            if (cardNo < 0) // 2019 06 11 - cardNo value is -1 if this IO name no exist in IO Map database.
                return;

            lock (m_objLock)
            {
                //avoid mismatch on card no
                if (cardNo >= NUM_IO_CARD)
                {
                    //SRMMessageBox.Show("INVALID Card Number.", "IO Output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                channel -= (cardNo * NUM_CHANNEL);

                if (channel >= m_outputByte[cardNo].Length)
                {
                    return;
                }

                //set output status in output byte array
                m_byteMask = (byte)(1 << bit);
                if (bitStatus == 0)
                {
                    m_byteMask ^= byte.MaxValue;
                    m_outputByte[cardNo][channel] &= m_byteMask;
                }
                else
                    m_outputByte[cardNo][channel] |= m_byteMask;

                //set output from io card
    
                if (m_blnUSBIOCard)
                {
                    HiPerfTimer T1 = new HiPerfTimer();
                    T1.Start();

                    //m_objVieUSB8IO.SetOutput(m_outputByte[cardNo][channel]);

                    //if (bitStatus == 1)
                    //    iRet = VTioCom64.VTCOM_SetOutput(2);
                    //else
                    //    iRet = VTioCom64.VTCOM_SetOutput(0);


                    T1.Stop();
                    STTrackLog.WriteLine(T1.Duration.ToString());
                }
                else
                {
                    //set output from io card
                    m_vieBoards[m_intVieBoardIndex[cardNo]].Send(m_intVieBoardChannel[cardNo][channel], m_outputByte[cardNo][channel]);

                }
            }
        }

        public static void ResetAllOutPort()
        {
            for (int cardNo = 0; cardNo < NUM_IO_CARD; cardNo++)
            {
                //bit value 0 means stats is on, so in order to indicate 1 in program as on, inverse it
                for (int channel = 0; channel < NUM_CHANNEL; channel++)
                {
                    m_outputByte[cardNo][channel] = 0;

                    //set output from io card
                    if (m_blnUSBIOCard)
                    {
                        //m_objVieUSB8IO.SetOutput(m_outputByte[cardNo][channel]);
                    }
                    else
                    {
                        m_vieBoards[m_intVieBoardIndex[cardNo]].Send(m_intVieBoardChannel[cardNo][channel], m_outputByte[cardNo][channel]);
                    }
                }
            }
        }

        /// <summary>
        /// Thread to scan input,created from function StartScanInput call from main form
        /// </summary>
        public static void ScanInput()
        {
            while (true)
            {
                //scan all io base on available card no
                for (int cardNo = 0; cardNo < NUM_IO_CARD; cardNo++)
                {
                    if (m_blnUSBIOCard)
                    {
                        //m_objVieUSB8IO.GetInput(ref uiInputData);
                        uiInputData &= 0x0000FFFF;

                        //bit value 1 means stats is on
                        for (int channel = 0; channel < NUM_CHANNEL; channel++)
                        {
                            m_inputByte[cardNo][channel] = (int)uiInputData;
                        }

                    }
                    else if (m_blnPCI64C)
                    {
                        //bit value 1 means stats is on
                        for (int channel = 0; channel < NUM_CHANNEL; channel++)
                            m_inputByte[cardNo][channel] = m_vieBoards[m_intVieBoardIndex[cardNo]].Receive(m_intVieBoardChannel[cardNo][channel]);
                    }
                    else
                    {
                        //bit value 0 means stats is on, so in order to indicate 1 in program as on, inverse it
                        for (int channel = 0; channel < NUM_CHANNEL; channel++)
                            m_inputByte[cardNo][channel] = 255 - m_vieBoards[m_intVieBoardIndex[cardNo]].Receive(m_intVieBoardChannel[cardNo][channel]);
                    }
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Get output bit status
        /// </summary>
        /// <param name="cardNo">card no</param>
        /// <param name="channel">channel</param>
        /// <param name="bit">bit</param>
        /// <returns>output bit status in memory</returns>
        public static int GetOutPortBitStatus(int cardNo, int channel, int bit)
        {
            if (cardNo < 0) // 2019 06 11 - cardNo value is -1 if this IO name no exist in IO Map database.
                return 0;

            //avoid mismatch on card no
            if (cardNo >= NUM_IO_CARD)
            {
                //SRMMessageBox.Show("INVALID Card Number.", "IO Output", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            channel -= (cardNo * NUM_CHANNEL);

            if (channel >= m_outputByte[cardNo].Length)
            {
                return 0;
            }

            int mask = 1 << bit;
            if ((IO.OutputByte[cardNo][channel] & mask) > 0)
                return 1;
            else
                return 0;
        }

        public static bool ConnectUSBIOCard(int intComNo)
        {
            //if (m_objVieUSB8IO == null)
            //{
            //    m_objVieUSB8IO = new VieUSB8IO(3);
            //}

            //if (m_objVieUSB8IO.Connect())
            //{
            //    SRMMessageBox.Show("Failed to connect!");
            //    return false;
            //}

            NUM_IO_CARD = 1;
            NUM_CHANNEL = 1;
            m_blnUSBIOCard = true;

            //create byte array for input and output
            m_outputByte = new byte[NUM_IO_CARD][];
            m_inputByte = new int[NUM_IO_CARD][];
            for (int cardNo = 0; cardNo < NUM_IO_CARD; cardNo++)
            {
                m_outputByte[cardNo] = new byte[NUM_CHANNEL];
                m_inputByte[cardNo] = new int[NUM_CHANNEL];
            }

            return true;
        }
    }
}
