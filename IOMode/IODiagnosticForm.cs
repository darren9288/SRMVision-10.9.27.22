using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Common;
using SharedMemory;
using SRMControl;

namespace IOMode
{
    public partial class IODiagnosticForm : Form
    {    
        #region Members Variables

        // Thread handle
        private readonly object m_objStopLock = new object();
        private bool m_blnStopping = false;
        private bool m_blnStopped = false;
        
        private bool m_blnScan = false; // continue scan if TRUE else stop scan
        private bool m_blnTriggered = false; // true if first pulse is triggered
        private bool m_blnFirstPulse = true;
        private bool m_blnStartScan = false;

        private int m_ioMask;
        private int m_intTriggerIO;
        private int[] m_intPulseCount = new int[4];
        private float m_fScanTimer = 0.0f;
        private float m_fTriggerTime = 0;
        private float[] m_fONValue = new float[4];
        private float[] m_fONPrev = new float[4];
        private float[] m_fON = new float[4];
        private float[] m_fOFFValue = new float[4];
        private float[,] m_fPulseTimer = new float[4,1024];
        private float[,] m_fPulse = new float[4,1024];
        private IOInfo[] m_ioList;
        private HiPerfTimer m_objTimer = new HiPerfTimer();
        private Thread m_thIO;

        #endregion


        public IODiagnosticForm()
        {
            InitializeComponent();

           

            m_thIO = new Thread(new ThreadStart(UpdateProgress));
            m_thIO.IsBackground = true;
            m_thIO.Start();
       }


        public int IOMask
        {
            set { m_ioMask = value; }
        }

        public IOInfo[] IOList
        {
            set
            {
                m_ioList = value;
                InitSignalGraph();
            }
        }



        #region Thread Handle
        /// <summary>
        /// Tells the thread to stop, typically after completing its 
        /// current work item.
        /// </summary>
        private void StopThread()
        {
            lock (m_objStopLock)
            {
                m_blnStopping = true;
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
        #endregion


        private void EnableButton(bool blnEnable)
        {
            TriggerSignalGroupBox.Enabled = blnEnable;
            PositiveCheckBox.Enabled = blnEnable;
            ScaleTrackBar.Enabled = blnEnable;
            ContinueScanCheckBox.Enabled = blnEnable;
            StartButton.Enabled = blnEnable;
            CloseButton.Enabled = blnEnable;
        }

        private void InitSignalGraph()
        {
            SignalGraph.HeaderText = "IO Signal Graph";

            if (m_ioList[0].Number == null || m_ioList[0].Number == "")
            {
                SignalGraph.Signal1Text = "";
                SignalGraph.Signal1Visible = false;
            }
            else
            {
                SignalGraph.Signal1Text = m_ioList[0].Number + ": " + m_ioList[0].Description;
                IO1RadioButton.Text = "IO " + m_ioList[0].Number;
            }

            if (m_ioList[1].Number == null || m_ioList[1].Number == "")
            {
                SignalGraph.Signal2Text = "";
                SignalGraph.Signal2Visible = false;
            }
            else
            {
                SignalGraph.Signal2Text = m_ioList[1].Number + ": " + m_ioList[1].Description;
                IO2RadioButton.Text = "IO " + m_ioList[1].Number;
            }

            if (m_ioList[2].Number == null || m_ioList[2].Number == "")
            {
                SignalGraph.Signal3Text = "";
                SignalGraph.Signal3Visible = false;
            }
            else
            {
                SignalGraph.Signal3Text = m_ioList[2].Number + ": " + m_ioList[2].Description;
                IO3RadioButton.Text = "IO " + m_ioList[2].Number;
            }

            if (m_ioList[3].Number == null || m_ioList[3].Number == "")
            {
                SignalGraph.Signal4Text = "";
                SignalGraph.Signal4Visible = false;
            }
            else
            {
                SignalGraph.Signal4Text = m_ioList[3].Number + ": " + m_ioList[3].Description;
                IO4RadioButton.Text = "IO " + m_ioList[3].Number;
            }

            StartButton.Enabled = true;

            IO4RadioButton.Checked = IO4RadioButton.Enabled = ((m_ioMask & 0x08) > 0);
            IO3RadioButton.Checked = IO3RadioButton.Enabled = ((m_ioMask & 0x04) > 0);
            IO2RadioButton.Checked = IO2RadioButton.Enabled = ((m_ioMask & 0x02) > 0);
            IO1RadioButton.Checked = IO1RadioButton.Enabled = ((m_ioMask & 0x01) > 0);
        }

        private void InitValue()
        {
            m_blnScan = true; // continue scan if TRUE else stop scan
            m_blnTriggered = false; // true if first pulse is triggered
            m_blnFirstPulse = true;
            m_fTriggerTime = 0;

            for (int i = 0; i < 4; i++)
            {
                if (PositiveCheckBox.Checked)
                { // triggered only if signal change from 0 to 1      
                    m_fONValue[i] = 1.0f;
                    m_fONPrev[i] = m_fON[i] = m_fOFFValue[i] = 0.0f;                   
                }
                else
                { // triggered only if signal change from 1 to 0     
                    if ((m_intTriggerIO & (1 << i)) > 0)
                    {
                        m_fONValue[i] = 0.0f;
                        m_fONPrev[i] = m_fON[i] = m_fOFFValue[i] = 1.0f;
                    }
                    else
                    {
                        m_fONValue[i] = 1.0f;
                        m_fONPrev[i] = m_fON[i] = m_fOFFValue[i] = 0.0f;
                    }                  
                }               
                m_intPulseCount[i] = 0;		
            }
            m_objTimer.Start();
        }

      
        private void UpdateGraph()
        {            
            List<ArrayList> arrPulse = new List<ArrayList>();
            
            for(int i =0; i < 4; i++)
            {
                arrPulse.Add(new ArrayList());

                if (m_intPulseCount[i] > 1024)
                    m_intPulseCount[i] = 1024;

                for (int j = 0; j < m_intPulseCount[i]; j++)
                {
                    arrPulse[i].Add(new PointF(m_fPulseTimer[i, j], m_fPulse[i, j]));
                }
            }

            SignalGraph.PlotSignal(arrPulse[0], arrPulse[1], arrPulse[2], arrPulse[3]);
        }




        private void StopButton_Click(object sender, EventArgs e)
        {
            m_blnScan = false;
            m_objTimer.Stop();
            EnableButton(true);         
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            SignalGraph.Clear();       
            SignalGraph.Start();

            m_fScanTimer = (SignalGraph.Width - 40) / ((float)SignalGraph.GridSize / SignalGraph.UnitPerDivision);

            if (IO1RadioButton.Checked)
                m_intTriggerIO = 1;
            else if (IO2RadioButton.Checked)
                m_intTriggerIO = 2;
            else if (IO3RadioButton.Checked)
                m_intTriggerIO = 4;
            else
                m_intTriggerIO = 8;

            m_blnStartScan = true;
            EnableButton(false);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            StopThread();
            Close();
            Dispose();
        }



        private void ScaleTrackBar_Scroll(object sender, EventArgs e)
        {
            if (ContinueScanCheckBox.Checked && ScaleTrackBar.Value < 4)
                ScaleTrackBar.Value = 4;

            switch (ScaleTrackBar.Value)
            {
                case 1:
                    SignalGraph.UnitPerDivision = 0.1f;
                    SignalGraph.XAxisText = "Time (0.1 ms/d)";
                    break;
                case 2:
                    SignalGraph.UnitPerDivision = 0.5f;
                    SignalGraph.XAxisText = "Time (0.5 ms/d)";
                    break;
                case 3:
                    SignalGraph.UnitPerDivision = 1;
                    SignalGraph.XAxisText = "Time (1 ms/d)";
                    break;
                case 4:
                    SignalGraph.UnitPerDivision = 5;
                    SignalGraph.XAxisText = "Time (5 ms/d)";
                    break;
                case 5:
                    SignalGraph.UnitPerDivision = 10;
                    SignalGraph.XAxisText = "Time (10 ms/d)";
                    break;
                case 6:
                    SignalGraph.UnitPerDivision = 20;
                    SignalGraph.XAxisText = "Time (20 ms/d)";
                    break;
                case 7:
                    SignalGraph.UnitPerDivision = 40;
                    SignalGraph.XAxisText = "Time (40 ms/d)";
                    break;
            }
        }



        private void ContinueScanCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ContinueScanCheckBox.Checked && ScaleTrackBar.Value < 4)
            {
                ScaleTrackBar.Value = 4;
                SignalGraph.UnitPerDivision = 5;
            }
        }

        private void ShowGridChB_CheckedChanged(object sender, EventArgs e)
        {
            SignalGraph.ShowGraphGrid = ShowGridChB.Checked;
        }




        private void IODiagnosticForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Enabled = false;
        }

      


        private void UpdateProgress()
        {
            try
            {
                while (!m_blnStopping)
                {
                    if (m_blnStartScan)
                    {
                        InitValue();
                        m_blnStartScan = false;
                    }

                    if (m_blnScan)
                    {
                        if (m_blnFirstPulse)
                        {
                            m_objTimer.Start();
                        }

                        for (int i = 0; i < 4 && !m_blnStopping; i++)
                        {
                            if ((m_ioMask & (1 << i)) > 0)
                            {

                                if (m_ioList[i].Type == "Input")
                                {
                                    if (IO.InPort(m_ioList[i].CardNo, m_ioList[i].Channel, m_ioList[i].Bit, 1, false))
                                        m_fON[i] = m_fONValue[i];
                                    else
                                        m_fON[i] = m_fOFFValue[i];
                                }
                                else
                                {
                                    if (IO.GetOutPortBitStatus(m_ioList[i].CardNo, m_ioList[i].Channel, m_ioList[i].Bit) == 1)
                                        m_fON[i] = m_fONValue[i];
                                    else
                                        m_fON[i] = m_fOFFValue[i];
                                }

                                if (m_fON[i] != m_fONPrev[i])
                                {
                                    if ((m_intTriggerIO & (1 << i)) > 0 && !m_blnTriggered)
                                    {
                                        m_blnTriggered = true;
                                        m_blnFirstPulse = false;

                                        for (int x = 0; x < 4; x++)
                                        {
                                            if (x == i)
                                                continue;

                                            m_fPulse[x, m_intPulseCount[x]] = m_fONPrev[x];
                                            m_fPulseTimer[x, m_intPulseCount[x]] = m_fTriggerTime;
                                            m_intPulseCount[x]++;
                                        }
                                    }

                                    if (m_blnTriggered)
                                    {
                                        // Store the previous node
                                        m_fPulse[i, m_intPulseCount[i]] = m_fONPrev[i];
                                        m_fPulseTimer[i, m_intPulseCount[i]] = m_fTriggerTime;
                                        m_intPulseCount[i]++;
                                        // Store the new node
                                        m_fPulse[i, m_intPulseCount[i]] = m_fON[i];
                                        m_fPulseTimer[i, m_intPulseCount[i]] = m_fTriggerTime;
                                        m_intPulseCount[i]++;

                                        m_fONPrev[i] = m_fON[i];
                                    }
                                }
                            }
                        }

                        if (m_intPulseCount[0] > 0 || m_intPulseCount[1] > 0 || m_intPulseCount[2] > 0 || m_intPulseCount[3] > 0)
                            m_fTriggerTime += m_objTimer.Timing;

                        if (m_fTriggerTime > m_fScanTimer)
                        {
                            UpdateGraph();
                            m_blnScan = false;

                            if (!ContinueScanCheckBox.Checked)
                            {
                                m_objTimer.Stop();
                            }
                            else
                            {
                                m_blnStartScan = true;
                                m_blnScan = false;
                            }
                        }

                        m_objTimer.Start();
                    }

                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("IODiagnostic: " + ex.ToString());
            }
            finally
            {
                SetStopped();
            }

        }

    }
}