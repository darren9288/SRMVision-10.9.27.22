using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common;
using Lighting;
using SharedMemory;

namespace SRMVision
{
    public partial class DiagnosticBox : Form
    {
        #region Delegate Event

        private delegate void AccessFormMarshalDelegate(string strSender, string strMessage);

        #endregion

        #region Member Variables

        private int m_intMaxLines = 18;
        private ComThread m_thCom;
        private CustomOption m_smCustomOption;
        private ProductionInfo m_smProductionInfo;
        private VisionInfo[] m_smVSInfo;
        private VisionComThread[] m_smVSComThread;
        private TCPIPIO[] m_smVSTCPIPIO;
        private RS232 m_thCOMMPort;

        #endregion

        public DiagnosticBox(ComThread thCom, VisionInfo[] smVSInfo, CustomOption smCustomOption, ProductionInfo smProductionInfo,
            VisionComThread[] smVSComThread, RS232 thCOMMPort, TCPIPIO[] smVSTCPIPIO)
        {
            m_thCom = thCom;
            m_smCustomOption = smCustomOption;
            m_smProductionInfo = smProductionInfo;
            m_smVSInfo = smVSInfo;
            m_smVSComThread = smVSComThread;
            m_smVSTCPIPIO = smVSTCPIPIO;
            m_thCOMMPort = thCOMMPort;

            InitializeComponent();

            AddComboBox();

            m_thCom.UpdateInterfaceEvent += new ComThread.UpdateInterfaceHandle(AccessFormMarshal);

            if (m_thCOMMPort != null)
            {
                m_thCOMMPort.UpdateInterfaceEvent += new RS232.UpdateInterfaceHandle(AccessFormMarshal);
                cbo_Vision.Visible = false;
            }

            for (int i = 0; i < m_smVSComThread.Length; i++)
            {
                if (m_smVSComThread[i] == null) continue;
                m_smVSComThread[i].ref_blnTCPIPEnable = true;
            }

            for (int i = 0; i < m_smVSTCPIPIO.Length; i++)
            {
                if (m_smVSTCPIPIO[i] == null) continue;
                m_smVSTCPIPIO[i].ref_blnTCPIPEnable = true;
            }

            if (!m_smCustomOption.g_blnLEDiControl && !m_smCustomOption.g_blnVTControl)
                TCOSIO_Control.UserInterfaceData += new TCOSIO_Control.UserInterfaceDataEventHandler(AccessFormMarshal);

            bool blnChanged = false;
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (m_smVSInfo[i].g_intMachineStatus == 2)
                {
                    blnChanged = true;
                    break;
                }
            }

            if (blnChanged)    // Idle mode
            {
                MessageTextBox.Enabled = false;
                SendButton.Enabled = false;
                btn_Clear.Enabled = false;
                DataTextBox.Size = new Size(414, 357);
                DataTextBox.Location = new Point(-2, 0);
                group_SelectTemplate.Visible = false;
                this.Size = new Size(418, 387);
                m_intMaxLines = 27;
            }
            else
            {
                MessageTextBox.Enabled = true;
                SendButton.Enabled = true;
                btn_Clear.Enabled = true;
                DataTextBox.Size = new Size(414, 250);
                DataTextBox.Location = new Point(-2, 106);
                group_SelectTemplate.Visible = true;
                this.Size = new Size(418, 431);
                m_intMaxLines = 18;
            }
        }



        /// <summary>
        /// Display communication content among vision PC and Others
        /// </summary>
        /// <param name="strSender">sender</param>
        /// <param name="strMsg">content</param>
        private void SetDataTextBox(string strSender, string strMsg)
        {
            if (DataTextBox.Lines.Length >= m_intMaxLines)
            {
                int intLength = m_intMaxLines - 1;
                string[] strData = new string[intLength];
                Array.Copy(DataTextBox.Lines, 1, strData, 0, intLength);
                DataTextBox.Lines = strData;
            }

            string[] strText = strMsg.Split(';');

            for (int i = 0; i < strText.Length; i++)
            {
                if (strText[i] == "")
                    continue;

                if (strSender == "" || strSender == null)
                    DataTextBox.Text += "\t\t" + strText[i] + Environment.NewLine;
                else if (strSender.Length < 7)
                    DataTextBox.Text += strSender + " :\t\t" + strText[i] + Environment.NewLine;
                else
                    DataTextBox.Text += strSender + " :\t" + strText[i] + Environment.NewLine;
            }
        }

        /// <summary>
        /// when this event is triggered, call SetDatatTextBox function
        /// </summary>
        /// <param name="strSender">sender</param>
        /// <param name="strMessage">content</param>
        private void AccessFormMarshal(string strSender, string strMessage)
        {
            if (DataTextBox.InvokeRequired)
            {
                DataTextBox.Invoke(new AccessFormMarshalDelegate(SetDataTextBox),
                    new object[] { strSender, strMessage });
            }
            else
            {
                SetDataTextBox(strSender, strMessage);
            }
        }

        private void AddComboBox()
        {
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                cbo_Vision.Items.Add(m_smVSInfo[i].g_strVisionName + m_smVSInfo[i].g_strVisionNameNo);


            }

            int intComNo = 0;
            if (m_smCustomOption.g_blnLEDiControl)
                intComNo = LEDi_Control.ref_intComNo;
            else if (m_smCustomOption.g_blnVTControl)
                intComNo = VT_Control.ref_intComNo;
            else
                intComNo = TCOSIO_Control.ref_intComNo;

            for (int i = 1; i < intComNo; i++)
            {
                cbo_Vision.Items.Add((i + 1).ToString());
            }
            cbo_Vision.SelectedIndex = 0;
            cbo_CommPort.SelectedIndex = 0;

            //if (m_smCustomOption.g_blnWantUseTCPIPIO)
            //{
            //    for (int i = 0; i < m_smVSInfo.Length; i++)
            //    {
            //        if (m_smVSInfo[i] == null)
            //            continue;

            //        cbo_Vision.Items.Add(m_smVSInfo[i].g_strVisionName + " - TCPIP IO");


            //    }
            //}
        }



        private void btn_Clear_Click(object sender, EventArgs e)
        {
            DataTextBox.Text = "";
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if (MessageTextBox.Text != "" && MessageTextBox.Text != null)
            {
                if (radioBtn_TCPIP.Checked)
                {
                    if (m_thCOMMPort == null)
                    {
                        switch (cbo_Vision.SelectedItem.ToString())
                        {
                            case "Common":
                                m_thCom.Send(MessageTextBox.Text);
                                break;
                            default:
                                if (m_smCustomOption.g_blnWantUseTCPIPIO)
                                {
                                    for (int i = 0; i < m_smVSInfo.Length; i++)
                                    {
                                        if (m_smVSInfo[i] == null || (m_smVSInfo[i].g_strVisionName + m_smVSInfo[i].g_strVisionNameNo) != cbo_Vision.SelectedItem.ToString())
                                            continue;

                                        m_smVSTCPIPIO[i].Send(MessageTextBox.Text);
                                        break;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < m_smVSInfo.Length; i++)
                                    {
                                        if (m_smVSInfo[i] == null || (m_smVSInfo[i].g_strVisionName + m_smVSInfo[i].g_strVisionNameNo) != cbo_Vision.SelectedItem.ToString())
                                            continue;

                                        m_smVSComThread[i].Send(MessageTextBox.Text);
                                        break;
                                    }
                                }
                                break;
                        }
                    }
                    else
                        m_thCOMMPort.SendData(MessageTextBox.Text);
                }
                else
                    TCOSIO_Control.SendMessage(cbo_CommPort.SelectedIndex, MessageTextBox.Text);

                MessageTextBox.Focus();
            }
        }



        private void DiagnosticBox_Load(object sender, EventArgs e)
        {
        }

        private void DiagnosticBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                m_thCom.UpdateInterfaceEvent -= new ComThread.UpdateInterfaceHandle(AccessFormMarshal);

                if (m_thCOMMPort != null)
                    m_thCOMMPort.UpdateInterfaceEvent -= new RS232.UpdateInterfaceHandle(AccessFormMarshal);

                for (int i = 0; i < m_smVSComThread.Length; i++)
                {
                    if (m_smVSComThread[i] == null) continue;
                    m_smVSComThread[i].ref_blnTCPIPEnable = false;
                }

                for (int i = 0; i < m_smVSTCPIPIO.Length; i++)
                {
                    if (m_smVSTCPIPIO[i] == null) continue;
                    m_smVSTCPIPIO[i].ref_blnTCPIPEnable = false;
                }

                if (!m_smCustomOption.g_blnLEDiControl && !m_smCustomOption.g_blnVTControl)
                    TCOSIO_Control.UserInterfaceData -= new TCOSIO_Control.UserInterfaceDataEventHandler(AccessFormMarshal);
            }
            catch
            {
            }
            this.Enabled = false;
        }



        private void ClientTimer_Tick(object sender, EventArgs e)
        {
            ClientTimer.Enabled = false;

            if (MessageTextBox.Enabled)
            {
                bool blnChanged = false;
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (m_smVSInfo[i].g_intMachineStatus == 2)
                    {
                        blnChanged = true;
                        break;
                    }
                }

                if (blnChanged)
                {
                    MessageTextBox.Enabled = false;
                    SendButton.Enabled = false;
                    btn_Clear.Enabled = false;
                    DataTextBox.Size = new Size(414, 357);
                    DataTextBox.Location = new Point(-2, 0);
                    group_SelectTemplate.Visible = false;
                    this.Size = new Size(418, 387);
                    m_intMaxLines = 27;
                }
            }
            else if (!MessageTextBox.Enabled)
            {
                bool blnChanged = true;
                for (int i = 0; i < m_smVSInfo.Length; i++)
                {
                    if (m_smVSInfo[i] == null)
                        continue;

                    if (m_smVSInfo[i].g_intMachineStatus == 2)
                    {
                        blnChanged = false;
                        break;
                    }
                }

                if (blnChanged)
                {
                    MessageTextBox.Enabled = true;
                    SendButton.Enabled = true;
                    btn_Clear.Enabled = true;
                    DataTextBox.Size = new Size(414, 250);
                    DataTextBox.Location = new Point(-2, 106);
                    group_SelectTemplate.Visible = true;
                    this.Size = new Size(418, 431);
                    m_intMaxLines = 18;
                }
            }

            //if (!m_smProductionInfo.AT_ALL_InAuto && cbo_Vision.Items.Count > 1)
            //{
            //    cbo_Vision.Items.Clear();
            //    cbo_Vision.Items.Add("Common");
            //    cbo_Vision.SelectedIndex = 0;
            //}
            //else if (m_smProductionInfo.AT_ALL_InAuto && cbo_Vision.Items.Count == 1)
            //    AddComboBox();

            for (int i = 0; i < m_smVSComThread.Length; i++)
            {
                if (m_smVSComThread[i] == null) continue;
                if (m_smVSComThread[i].ref_blnGotMessage)
                {
                    string[] strMessage = m_smVSInfo[i].g_strTCPMessage.Split('*');
                    m_smVSInfo[i].g_strTCPMessage = "";
                    foreach (string text in strMessage)
                        SetDataTextBox(m_smVSInfo[i].g_strVisionName + m_smVSInfo[i].g_strVisionNameNo, text);

                    m_smVSComThread[i].ref_blnGotMessage = false;
                }
            }

            for (int i = 0; i < m_smVSTCPIPIO.Length; i++)
            {
                if (m_smVSTCPIPIO[i] == null) continue;
                if (m_smVSTCPIPIO[i].ref_blnGotMessage)
                {
                    string[] strMessage = m_smVSInfo[i].g_strTCPMessage.Split('*');
                    m_smVSInfo[i].g_strTCPMessage = "";
                    foreach (string text in strMessage)
                        SetDataTextBox(m_smVSInfo[i].g_strVisionName + m_smVSInfo[i].g_strVisionNameNo, text);

                    m_smVSTCPIPIO[i].ref_blnGotMessage = false;
                }
            }

            ClientTimer.Enabled = true;
        }

    }




}