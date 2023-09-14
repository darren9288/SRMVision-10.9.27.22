using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using SharedMemory;
using User;
using Common;

namespace IOMode
{
    public partial class NewIOMapForm : Form
    {
        #region Constant Variables
        private const int NUM_CHANNEL = 2;           // 1 card has 2 channel
        #endregion

        #region Member Variables
        private bool repeat = false;
        private int tabcount = 0;
        private int m_group = 0;
        private int m_lastinputChannel = 0;
        private int m_lastoutputChannel = 0;
        private ArrayList m_arrModules = new ArrayList();
        private DataSet m_ioMapDataSet = new DataSet();
        private DataSet m_ioDataSet = new DataSet();
        private IOScanInfo[] m_inputScanInfoList = new IOScanInfo[64];    // contain bit information to be scanned
        private IOScanInfo[] m_outputScanInfoList = new IOScanInfo[64];
        private byte[] m_inputByteList = new byte[32];   //To store Input bits
        private byte[] m_outputByteList = new byte[32];   //To store Output bits
        private List<int> m_arrInNumber = new List<int>();
        private List<int> m_arrInCard = new List<int>();
        private List<int> m_arrInPin = new List<int>();
        private List<int> m_arrOutNumber = new List<int>();
        private List<int> m_arrOutCard = new List<int>();
        private List<int> m_arrOutPin = new List<int>();
        private CustomOption m_smCustomizeInfo;
        #endregion
        
        public NewIOMapForm(CustomOption smCustomizeInfo)
        {
            m_smCustomizeInfo = smCustomizeInfo;
            InitializeComponent();
            ClearTab();
            FilterModuleSet();
            GetIODataSet();
            FillModuleTab();
            UpdateInput();
            UpdateOutput();
        }
        
        private void ClearTab()
        {
            if (tab_Module.TabPages.Contains(tab_0))
                tab_Module.TabPages.Remove(tab_0);
            if (tab_Module.TabPages.Contains(tab_1))
                tab_Module.TabPages.Remove(tab_1);
            if (tab_Module.TabPages.Contains(tab_2))
                tab_Module.TabPages.Remove(tab_2);
            if (tab_Module.TabPages.Contains(tab_3))
                tab_Module.TabPages.Remove(tab_3);
            if (tab_Module.TabPages.Contains(tab_4))
                tab_Module.TabPages.Remove(tab_4);
            if (tab_Module.TabPages.Contains(tab_5))
                tab_Module.TabPages.Remove(tab_5);
            if (tab_Module.TabPages.Contains(tab_6))
                tab_Module.TabPages.Remove(tab_6);
            if (tab_Module.TabPages.Contains(tab_7))
                tab_Module.TabPages.Remove(tab_7);
        }

        private void UpdateInput()
        { 
            string filter = "Type = 'Input'";
            bool itemAdded = true;
            int cardno = 0;
            int channel = 0;
            int bit = 0;
            int row = 0;
            int currentChannel = 0;
            m_lastinputChannel = 0;
            //get datarow list from table "IO" in m_ioDataSet,filter by type sort by number
            DataRow[] ioList = m_ioDataSet.Tables["IO"].Select(filter, "Number");

            //run thru whole ioList datarow,insert cell into grid view one by one
            foreach (DataRow io in ioList)
            {
                //check authority level to display on grid view
                if ( m_group > Convert.ToInt32(io["Group"]))
                    itemAdded = false;

                if (itemAdded)
                {
                    dgd_Input.Rows.Add(new DataGridViewRow());
                    dgd_Input.Rows[row].Cells[0].Value = false;
                    dgd_Input.Rows[row].Cells[1].Value = (object)InputImageListTree.Images[0];
                    dgd_Input.Rows[row].Cells[2].Value = io["Card Number"].ToString();
                    dgd_Input.Rows[row].Cells[3].Value = io["Pin Number"].ToString();
                    dgd_Input.Rows[row].Cells[4].Value = io["Channel Number"].ToString();
                    dgd_Input.Rows[row].Cells[5].Value = io["Bit"].ToString();     
                    //get value for cardno,channel,bit
                    cardno = Convert.ToInt32(io["Card Number"]);
                    channel = Convert.ToInt32(io["Channel Number"]);
                    bit = Convert.ToInt32(io["Bit"]);

                    int i;
                    bool newChannel = false;
                    // if both condition is 0 means no channel been addin to ioScanInfo list
                    // add new channel to the array ioScanInfo list
                    if (m_lastinputChannel == 0 && currentChannel == 0)
                        newChannel = true;
                    else
                    {
                        // there are channels in the list
                        // compare current channel from list and current want to add in channel
                        // no same means need to be add in
                        if (m_inputScanInfoList[currentChannel].Channel != channel)
                        {
                            //add up to be less than next channel to add
                            // ex. lastChannel=1, means i only run till 0,which is current index of array list
                            for (i = 0; i < m_lastinputChannel; i++)
                            {
                                // channel already exists
                                if (m_inputScanInfoList[i].Channel == channel)
                                {
                                    currentChannel = i;
                                    break;
                                }
                            }
                            // the channel is not in the array list, add a new channel
                            if (i == m_lastinputChannel)
                                newChannel = true;
                        }
                    }

                    // add a channel to ioScanInfo list
                    if (newChannel)
                    {
                        //only add in a channel at begin of 1st bit
                        m_inputScanInfoList[m_lastinputChannel] = new IOScanInfo(0);
                        m_inputScanInfoList[m_lastinputChannel].CardNo = cardno;
                        m_inputScanInfoList[m_lastinputChannel].Channel = channel;
                        //set current channel
                        currentChannel = m_lastinputChannel;
                        //increment next channel to add
                        m_lastinputChannel++;
                        //in one channel there are 8 bit to reset, items 0-7
                        for (int a = 0; a < 8; a++)
                            m_inputScanInfoList[currentChannel].ItemNo[a] = 99;
                    }
                    // set the result/status as itembit value base on position of itembit array 
                    m_inputScanInfoList[currentChannel].ItemBit |= (byte)(1 << bit);
                    // add the item number to the list same as row index
                    m_inputScanInfoList[currentChannel].ItemNo[bit] = row;
                    // item added, set previous byte to 0
                    // so it can be different from current and trigger updating process in timer tick
                    m_inputScanInfoList[currentChannel].BytePrev = 0;
                    //next row
                    row++;
                }
                itemAdded = true;
            }
        }

        private void dgd_Output_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            if (e.ColumnIndex == 1)   // 3rd column will ON/Off the output channel
            {
                int cardNo = Convert.ToInt32(dgd_Output.Rows[e.RowIndex].Cells[2].Value.ToString());
                int channel = Convert.ToInt32(dgd_Output.Rows[e.RowIndex].Cells[4].Value.ToString());
                int bit = Convert.ToInt32(dgd_Output.Rows[e.RowIndex].Cells[5].Value.ToString());
               
                int bitStatus = 0;

                if ((bool)dgd_Output.Rows[e.RowIndex].Cells[0].Value)
                {
                    bitStatus = 0;
                    dgd_Output.Rows[e.RowIndex].Cells[0].Value = false;
                    dgd_Output.Rows[e.RowIndex].Cells[1].Value = (object)OutputImageListTree.Images[0];
                }
                //to trigger on an output bit
                else
                {
                    bitStatus = 1;
                    dgd_Output.Rows[e.RowIndex].Cells[0].Value = true;
                    dgd_Output.Rows[e.RowIndex].Cells[1].Value = (object)OutputImageListTree.Images[1];
                }
                //update to output byte 
                IO.OutPort(cardNo, channel, bit, bitStatus);
            }
        }
        private void UpdateOutput()
        {
            string filter = "Type = 'Output'";
            bool itemAdded = true;
            int cardno = 0;
            int channel = 0;
            int bit = 0;
            int row = 0;
            int currentChannel = 0;
            m_lastoutputChannel = 0;

            //get datarow list from table "IO" in m_ioDataSet,filter by type sort by number
            DataRow[] ioList = m_ioDataSet.Tables["IO"].Select(filter, "Number");

            //run thru whole ioList datarow,insert cell into grid view one by one
            foreach (DataRow io in ioList)
            {
                //check authority level to display on grid view
                if (m_group > Convert.ToInt32(io["Group"]))
                    itemAdded = false;

                //granted authority
                if (itemAdded)
                {
                    dgd_Output.Rows.Add(new DataGridViewRow());
                    dgd_Output.Rows[row].Cells[0].Value = false;
                    dgd_Output.Rows[row].Cells[1].Value = (object)OutputImageListTree.Images[0];
                    dgd_Output.Rows[row].Cells[2].Value = io["Card Number"].ToString();
                    dgd_Output.Rows[row].Cells[3].Value = io["Pin Number"].ToString();
                    dgd_Output.Rows[row].Cells[4].Value = io["Channel Number"].ToString();
                    dgd_Output.Rows[row].Cells[5].Value = io["Bit"].ToString();
                    //get value for cardno,channel,bit
                    cardno = Convert.ToInt32(io["Card Number"]);
                    channel = Convert.ToInt32(io["Channel Number"]);
                    bit = Convert.ToInt32(io["Bit"]);

                    int i;
                    bool newChannel = false;
                    // if both condition is 0 means no channel been addin to ioScanInfo list
                    // add new channel to the array ioScanInfo list
                    if (m_lastoutputChannel == 0 && currentChannel == 0)
                        newChannel = true;
                    else
                    {
                        // there are channels in the list
                        // compare current channel from list and current want to add in channel
                        // no same means need to be add in
                        if (m_outputScanInfoList[currentChannel].Channel != channel)
                        {
                            //add up to be less than next channel to add
                            // ex. lastChannel=1, means i only run till 0,which is current index of array list
                            for (i = 0; i < m_lastoutputChannel; i++)
                            {
                                // channel already exists
                                if (m_outputScanInfoList[i].Channel == channel)
                                {
                                    currentChannel = i;
                                    break;
                                }
                            }
                            // the channel is not in the array list, add a new channel
                            if (i == m_lastoutputChannel)
                                newChannel = true;
                        }
                    }
                    // add a channel to ioScanInfo list
                    if (newChannel)
                    {
                        //only add in a channel at begin of 1st bit
                        m_outputScanInfoList[m_lastoutputChannel] = new IOScanInfo(0);
                        m_outputScanInfoList[m_lastoutputChannel].CardNo = cardno;
                        m_outputScanInfoList[m_lastoutputChannel].Channel = channel;

                        //set current channel
                        currentChannel = m_lastoutputChannel;
                        //increment next channel to add
                        m_lastoutputChannel++;

                        //in one channel there are 8 bit to reset, items 0-7
                        for (int a = 0; a < 8; a++)
                            m_outputScanInfoList[currentChannel].ItemNo[a] = 99;
                    }
                    // set the result/status as itembit value base on position of itembit array 
                    m_outputScanInfoList[currentChannel].ItemBit |= (byte)(1 << bit);
                    // add the item number to the list same as row index
                    m_outputScanInfoList[currentChannel].ItemNo[bit] = row;
                    // item added, set previous byte to 0
                    // so it can be different from current and trigger updating process in timer tick
                    m_outputScanInfoList[currentChannel].BytePrev = 0;
                    //next row
                    row++;
                }
                itemAdded = true;
            }
        }
        /// <summary>
        /// Fill in node in io tree view
        /// </summary>
        private void FillModuleTab()
        {
            try
            {
                //get database connection
                DBCall dbCall = new DBCall("\\access\\simeca.mdb");
                //create dataset 
                DataSet moduleSet = new DataSet();
                //select from iomap file by module and create table "module" in moduleSet 
                dbCall.Select("SELECT DISTINCT [Module] FROM IOMap", moduleSet, "Module");

                m_ioMapDataSet = new DataSet();
                //select all from iomap file and create table "IOMap" in m_ioMapDataSet
                dbCall.Select("SELECT * FROM IOMap", m_ioMapDataSet, "IOMap");

                GetIOLIst();
                //base on number of module inside moduleSet table "module" 
                for (int i = 0; i < moduleSet.Tables["Module"].Rows.Count; i++)
                {
                    string strModuleName = moduleSet.Tables["Module"].Rows[i]["Module"].ToString();

                    if (!m_arrModules.Contains(strModuleName))
                    { 
                        continue;
                    }
                        tabcount++; 
                    string filter = "Module = '" + strModuleName + "'";
                    string sort = "Name";
                    string nodeText = "";
                    string subNodeTextPrev = "";
                    //1st level node is module name
                    nodeText = moduleSet.Tables["Module"].Rows[i]["Module"].ToString();
                    //get datarow list base on module name and io type
                    DataRow[] inputList = m_ioMapDataSet.Tables["IOMap"].Select(filter + " AND Type = 'Input'", sort);
                    DataRow[] outputList = m_ioMapDataSet.Tables["IOMap"].Select(filter + " AND Type = 'Output'", sort);
                    switch (tabcount-1)
                    {
                        case 0:
                            tab_0.Text= nodeText;
                         
                            if (!tab_Module.TabPages.Contains(tab_0))
                            tab_Module.TabPages.Add(tab_0);
                            int incount = 0;
                            //run thru whole input list and insert node one by one
                            foreach (DataRow input in inputList)
                            {
                                string subNodeText = input["Name"].ToString();
                                string Iname = input["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_In0.Rows.Add(new DataGridViewRow());
                                    dgd_In0.Rows[incount].Cells[0].Value = subNodeText;
                                    dgd_In0.Rows[incount].Cells[2].Value = input["IO Number"].ToString();
                                    UpdateInputPin2("Input", Iname, Column_InPin0, dgd_In0.Rows[incount].Cells[1]);
                                    incount++;
                                }
                                //avoid duplication 
                                subNodeTextPrev = subNodeText;
                            }

                            subNodeTextPrev = "";
                            //run thru whole output list and insert one by one
                            int outcount = 0;
                            foreach (DataRow output in outputList)
                            {
                                string subNodeText = output["Name"].ToString();
                                string Oname = output["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_Out0.Rows.Add(new DataGridViewRow());
                                    dgd_Out0.Rows[outcount].Cells[0].Value = subNodeText;
                                    dgd_Out0.Rows[outcount].Cells[2].Value = output["IO Number"].ToString();
                                    UpdateOutputPin2("Output", Oname, Column_OutPin0, dgd_Out0.Rows[outcount].Cells[1]);
                                    outcount++;
                                }
                                subNodeTextPrev = subNodeText;
                            }
                            break;
                        case 1:
                            tab_1.Text = nodeText;
                        
                            if (!tab_Module.TabPages.Contains(tab_1))
                                tab_Module.TabPages.Add(tab_1);
                            incount = 0;
                            //run thru whole input list and insert node one by one
                            foreach (DataRow input in inputList)
                            {
                                string subNodeText = input["Name"].ToString();
                                string Iname = input["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_In1.Rows.Add(new DataGridViewRow());
                                    dgd_In1.Rows[incount].Cells[0].Value = subNodeText;
                                    dgd_In1.Rows[incount].Cells[2].Value = input["IO Number"].ToString();
                                    UpdateInputPin2("Input", Iname, Column_InPin1, dgd_In1.Rows[incount].Cells[1]);
                                    incount++;
                                }
                                //avoid duplication 
                                subNodeTextPrev = subNodeText;
                            }

                            subNodeTextPrev = "";
                            //run thru whole output list and insert one by one
                             outcount = 0;
                            foreach (DataRow output in outputList)
                            {
                                string subNodeText = output["Name"].ToString();
                                string Oname = output["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_Out1.Rows.Add(new DataGridViewRow());
                                    dgd_Out1.Rows[outcount].Cells[0].Value = subNodeText;
                                    dgd_Out1.Rows[outcount].Cells[2].Value = output["IO Number"].ToString();
                                    UpdateOutputPin2("Output", Oname, Column_OutPin1, dgd_Out1.Rows[outcount].Cells[1]);
                                    outcount++;
                                }
                                subNodeTextPrev = subNodeText;
                            }
                            break;
                        case 2:
                            tab_2.Text = nodeText;
                           
                            if (!tab_Module.TabPages.Contains(tab_2))
                                tab_Module.TabPages.Add(tab_2);
                            incount = 0;
                            //run thru whole input list and insert node one by one
                            foreach (DataRow input in inputList)
                            {
                                string subNodeText = input["Name"].ToString();
                                string Iname = input["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_In2.Rows.Add(new DataGridViewRow());
                                    dgd_In2.Rows[incount].Cells[0].Value = subNodeText;
                                    dgd_In2.Rows[incount].Cells[2].Value = input["IO Number"].ToString();
                                    UpdateInputPin2("Input", Iname, Column_InPin2, dgd_In2.Rows[incount].Cells[1]);
                                    incount++;
                                }
                                //avoid duplication 
                                subNodeTextPrev = subNodeText;
                            }

                            subNodeTextPrev = "";
                            //run thru whole output list and insert one by one
                             outcount = 0;
                            foreach (DataRow output in outputList)
                            {
                                string subNodeText = output["Name"].ToString();
                                string Oname = output["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_Out2.Rows.Add(new DataGridViewRow());
                                    dgd_Out2.Rows[outcount].Cells[0].Value = subNodeText;
                                    dgd_Out2.Rows[outcount].Cells[2].Value = output["IO Number"].ToString();
                                    UpdateOutputPin2("Output", Oname, Column_OutPin2, dgd_Out2.Rows[outcount].Cells[1]);
                                    outcount++;
                                }
                                subNodeTextPrev = subNodeText;
                            }
                            break;
                        case 3:
                            tab_3.Text = nodeText;
                          
                            if (!tab_Module.TabPages.Contains(tab_3))
                                tab_Module.TabPages.Add(tab_3);
                            incount = 0;
                            //run thru whole input list and insert node one by one
                            foreach (DataRow input in inputList)
                            {
                                string subNodeText = input["Name"].ToString();
                                string Iname = input["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_In3.Rows.Add(new DataGridViewRow());
                                    dgd_In3.Rows[incount].Cells[0].Value = subNodeText;
                                    dgd_In3.Rows[incount].Cells[2].Value = input["IO Number"].ToString();
                                    UpdateInputPin2("Input", Iname, Column_InPin3, dgd_In3.Rows[incount].Cells[1]);
                                    incount++;
                                }
                                //avoid duplication 
                                subNodeTextPrev = subNodeText;
                            }

                            subNodeTextPrev = "";
                            //run thru whole output list and insert one by one
                             outcount = 0;
                            foreach (DataRow output in outputList)
                            {
                                string subNodeText = output["Name"].ToString();
                                string Oname = output["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_Out3.Rows.Add(new DataGridViewRow());
                                    dgd_Out3.Rows[outcount].Cells[0].Value = subNodeText;
                                    dgd_Out3.Rows[outcount].Cells[2].Value = output["IO Number"].ToString();
                                    UpdateOutputPin2("Output", Oname, Column_OutPin3, dgd_Out3.Rows[outcount].Cells[1]);
                                    outcount++;
                                }
                                subNodeTextPrev = subNodeText;
                            }
                            break;
                        case 4:
                            tab_4.Text = nodeText;
                           
                            if (!tab_Module.TabPages.Contains(tab_4))
                                tab_Module.TabPages.Add(tab_4);
                            incount = 0;
                            //run thru whole input list and insert node one by one
                            foreach (DataRow input in inputList)
                            {
                                string subNodeText = input["Name"].ToString();
                                string Iname = input["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_In4.Rows.Add(new DataGridViewRow());
                                    dgd_In4.Rows[incount].Cells[0].Value = subNodeText;
                                    dgd_In4.Rows[incount].Cells[2].Value = input["IO Number"].ToString();
                                    UpdateInputPin2("Input", Iname, Column_InPin4, dgd_In4.Rows[incount].Cells[1]);
                                    incount++;
                                }
                                //avoid duplication 
                                subNodeTextPrev = subNodeText;
                            }

                            subNodeTextPrev = "";
                            //run thru whole output list and insert one by one
                             outcount = 0;
                            foreach (DataRow output in outputList)
                            {
                                string subNodeText = output["Name"].ToString();
                                string Oname = output["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_Out4.Rows.Add(new DataGridViewRow());
                                    dgd_Out4.Rows[outcount].Cells[0].Value = subNodeText;
                                    dgd_Out4.Rows[outcount].Cells[2].Value = output["IO Number"].ToString();
                                    UpdateOutputPin2("Output", Oname, Column_OutPin4, dgd_Out4.Rows[outcount].Cells[1]);
                                    outcount++;
                                }
                                subNodeTextPrev = subNodeText;
                            }
                            break;
                        case 5:
                            tab_5.Text = nodeText;

                            if (!tab_Module.TabPages.Contains(tab_5))
                                tab_Module.TabPages.Add(tab_5);
                            incount = 0;
                            //run thru whole input list and insert node one by one
                            foreach (DataRow input in inputList)
                            {
                                string subNodeText = input["Name"].ToString();
                                string Iname = input["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_In5.Rows.Add(new DataGridViewRow());
                                    dgd_In5.Rows[incount].Cells[0].Value = subNodeText;
                                    dgd_In5.Rows[incount].Cells[2].Value = input["IO Number"].ToString();
                                    UpdateInputPin2("Input", Iname, Column_InPin5, dgd_In5.Rows[incount].Cells[1]);
                                    incount++;
                                }
                                //avoid duplication 
                                subNodeTextPrev = subNodeText;
                            }

                            subNodeTextPrev = "";
                            //run thru whole output list and insert one by one
                            outcount = 0;
                            foreach (DataRow output in outputList)
                            {
                                string subNodeText = output["Name"].ToString();
                                string Oname = output["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_Out5.Rows.Add(new DataGridViewRow());
                                    dgd_Out5.Rows[outcount].Cells[0].Value = subNodeText;
                                    dgd_Out5.Rows[outcount].Cells[2].Value = output["IO Number"].ToString();
                                    UpdateOutputPin2("Output", Oname, Column_OutPin5, dgd_Out5.Rows[outcount].Cells[1]);
                                    outcount++;
                                }
                                subNodeTextPrev = subNodeText;
                            }
                            break;
                        case 6:
                            tab_6.Text = nodeText;

                            if (!tab_Module.TabPages.Contains(tab_6))
                                tab_Module.TabPages.Add(tab_6);
                            incount = 0;
                            //run thru whole input list and insert node one by one
                            foreach (DataRow input in inputList)
                            {
                                string subNodeText = input["Name"].ToString();
                                string Iname = input["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_In6.Rows.Add(new DataGridViewRow());
                                    dgd_In6.Rows[incount].Cells[0].Value = subNodeText;
                                    dgd_In6.Rows[incount].Cells[2].Value = input["IO Number"].ToString();
                                    UpdateInputPin2("Input", Iname, Column_InPin6, dgd_In6.Rows[incount].Cells[1]);
                                    incount++;
                                }
                                //avoid duplication 
                                subNodeTextPrev = subNodeText;
                            }

                            subNodeTextPrev = "";
                            //run thru whole output list and insert one by one
                            outcount = 0;
                            foreach (DataRow output in outputList)
                            {
                                string subNodeText = output["Name"].ToString();
                                string Oname = output["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_Out6.Rows.Add(new DataGridViewRow());
                                    dgd_Out6.Rows[outcount].Cells[0].Value = subNodeText;
                                    dgd_Out6.Rows[outcount].Cells[2].Value = output["IO Number"].ToString();
                                    UpdateOutputPin2("Output", Oname, Column_OutPin6, dgd_Out6.Rows[outcount].Cells[1]);
                                    outcount++;
                                }
                                subNodeTextPrev = subNodeText;
                            }
                            break;
                        case 7:
                            tab_7.Text = nodeText;

                            if (!tab_Module.TabPages.Contains(tab_7))
                                tab_Module.TabPages.Add(tab_7);
                            incount = 0;
                            //run thru whole input list and insert node one by one
                            foreach (DataRow input in inputList)
                            {
                                string subNodeText = input["Name"].ToString();
                                string Iname = input["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_In7.Rows.Add(new DataGridViewRow());
                                    dgd_In7.Rows[incount].Cells[0].Value = subNodeText;
                                    dgd_In7.Rows[incount].Cells[2].Value = input["IO Number"].ToString();
                                    UpdateInputPin2("Input", Iname, Column_InPin7, dgd_In7.Rows[incount].Cells[1]);
                                    incount++;
                                }
                                //avoid duplication 
                                subNodeTextPrev = subNodeText;
                            }

                            subNodeTextPrev = "";
                            //run thru whole output list and insert one by one
                            outcount = 0;
                            foreach (DataRow output in outputList)
                            {
                                string subNodeText = output["Name"].ToString();
                                string Oname = output["Name"].ToString();
                                if (subNodeText != "" && subNodeTextPrev != subNodeText)
                                {
                                    dgd_Out7.Rows.Add(new DataGridViewRow());
                                    dgd_Out7.Rows[outcount].Cells[0].Value = subNodeText;
                                    dgd_Out7.Rows[outcount].Cells[2].Value = output["IO Number"].ToString();
                                    UpdateOutputPin2("Output", Oname, Column_OutPin7, dgd_Out7.Rows[outcount].Cells[1]);
                                    outcount++;
                                }
                                subNodeTextPrev = subNodeText;
                            }
                            break;
                        default:
                            break;
                    } 
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "IO Trigger",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

         //   m_userSelect = false;
           
        }

        private ArrayList FilterModuleSet()
        {
            m_arrModules.Clear();
            m_arrModules.Add("General");

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            RegistryKey subKey1;

            for (int i = 0; i < 10; i++)
            {
                if ((m_smCustomizeInfo.g_intVisionMask & (1 << i)) > 0)
                {
                    subKey1 = subKey.OpenSubKey("Vision" + (i + 1));
                    if (subKey1 != null)
                        m_arrModules.Add(subKey1.GetValue("VisionDisplayName", "Vision " + (i + 1)));
                }
            }
            m_arrModules.Add("General");

            return m_arrModules;
        }

        /// <summary>
        /// Get all data from IO table of simeca.mdb file and create table "IO" inside m_ioDataSet
        /// </summary>
        private void GetIODataSet()
        {
            DBCall dbCall = new DBCall("\\access\\simeca.mdb");
            m_ioDataSet = new DataSet();
            dbCall.Select("SELECT * FROM IO", m_ioDataSet, "IO");
        }

        public static void SortData_Ascending(List<int> arrCard, List<int> arrPin, List<int> arrNum,
       ref List<int> arrSortCard, ref List<int> arrSortPin, ref List<int> arrSortNum)
        {
            // Define Total Card
            List<int> arrCardNoList = new List<int>();
            for (int i = 0; i < arrCard.Count; i++)
            {
                bool blnFound = false;
                for (int j = 0; j < arrCardNoList.Count; j++)
                {
                    if (arrCard[i] == arrCardNoList[j])
                    {
                        blnFound = true;
                        break;
                    } 
                }

                if (!blnFound)
                {
                    arrCardNoList.Add(arrCard[i]);
                }
            }

            arrSortCard.Clear();
            arrSortPin.Clear();
            arrSortNum.Clear();
            for (int i = 0; i < arrCardNoList.Count; i++)
            {
                for (int j = 0; j < arrCard.Count; j++)
                {
                    if (arrCard[j] != arrCardNoList[i])
                        continue;

                    int intIndex = arrSortPin.Count;
                    for (int k = 0; k < arrSortPin.Count; k++)
                    {
                        if (arrSortCard[k] != arrCardNoList[i])
                            continue;

                        if (arrSortPin[k] > arrPin[j])
                        {
                            intIndex = k;
                            break;
                        }
                    }
                    arrSortCard.Insert(intIndex, arrCard[j]);
                    arrSortPin.Insert(intIndex, arrPin[j]);
                    arrSortNum.Insert(intIndex, arrNum[j]);
                }
            }
        }

        private void GetIOLIst()
        {
            string filter = "Type = 'Input'";
            bool itemAdded = true;
            //get datarow list from table "IO" in m_ioDataSet,filter by type sort by number
            DataRow[] ioList = m_ioDataSet.Tables["IO"].Select(filter, "Number");

            List<int> arrInNumber = new List<int>();
            List<int> arrInCard = new List<int>();
            List<int> arrInPin = new List<int>();

            foreach (DataRow io in ioList)
            {
                if (m_group > Convert.ToInt32(io["Group"]))
                    itemAdded = false;
                if (itemAdded)
                {
                    if (!m_arrModules.Contains(io["Module"]) && io["Module"].ToString() != "")
                    {
                        UnMapIO(io["Number"].ToString());
                    }
                    arrInNumber.Add(Convert.ToInt32(io["Number"].ToString()));
                    arrInCard.Add(Convert.ToInt32(io["Card Number"].ToString()));
                    arrInPin.Add(Convert.ToInt32(io["Pin Number"].ToString()));
                }
                itemAdded = true;
            }

            SortData_Ascending(arrInCard, arrInPin, arrInNumber, ref m_arrInCard, ref m_arrInPin, ref m_arrInNumber);

            filter = "Type = 'Output'";

            //get datarow list from table "IO" in m_ioDataSet,filter by type sort by number
            ioList = m_ioDataSet.Tables["IO"].Select(filter, "Number");

            List<int> arrOutNumber = new List<int>();
            List<int> arrOutCard = new List<int>();
            List<int> arrOutPin = new List<int>();

            foreach (DataRow io in ioList)
            {
                if (m_group > Convert.ToInt32(io["Group"]))
                    itemAdded = false;
                if (itemAdded)
                {
                    if (!m_arrModules.Contains(io["Module"]) && io["Module"].ToString() != "")
                    {
                        UnMapIO(io["Number"].ToString());
                    }
                    arrOutNumber.Add(Convert.ToInt32(io["Number"].ToString()));
                    arrOutCard.Add(Convert.ToInt32(io["Card Number"].ToString()));
                    arrOutPin.Add(Convert.ToInt32(io["Pin Number"].ToString()));
                }
                itemAdded = true;
            }
            SortData_Ascending(arrOutCard, arrOutPin, arrOutNumber, ref m_arrOutCard, ref m_arrOutPin, ref m_arrOutNumber);
        }

        private void UpdateInputPin2(string parent, string name, DataGridViewComboBoxColumn dc, DataGridViewCell cell1)
        {
            for (int i = 0; i < m_arrInCard.Count; i++)
            {
                if(!dc.Items.Contains("Card" + m_arrInCard[i].ToString() + " > " + "Pin" + m_arrInPin[i].ToString()))
                dc.Items.Add("Card" + m_arrInCard[i].ToString() + " > " + "Pin" + m_arrInPin[i].ToString());// + " > " +"Num"+ m_arrInNumber[i].ToString());
            }
            string filter = "Type = 'Input'";
            DataRow[] ioList = m_ioDataSet.Tables["IO"].Select(filter, "Number");

            foreach (DataRow io in ioList)
            {
                if (name.Trim() == io["Description"].ToString().Trim())
                {
                    cell1.Value = "Card" + io["Card Number"].ToString() + " > " +
                              "Pin" + io["Pin Number"].ToString();// + " > " + "Num" + io["Number"].ToString();
                }
            }
        }

        private void UpdateOutputPin2(string parent, string name, DataGridViewComboBoxColumn dc, DataGridViewCell cell1)
        {
            for (int i = 0; i < m_arrOutCard.Count; i++)
            {
                if(!dc.Items.Contains("Card" + m_arrOutCard[i].ToString() + " > " + "Pin" + m_arrOutPin[i].ToString()))
                dc.Items.Add("Card" + m_arrOutCard[i].ToString() + " > " + "Pin" + m_arrOutPin[i].ToString());// +" > "+"Num"+m_arrOutNumber[i].ToString());
            }
            string filter = "Type = 'Output'";
            DataRow[] ioList = m_ioDataSet.Tables["IO"].Select(filter, "Number");

            foreach (DataRow io in ioList)
            {
                if (name.Trim() == io["Description"].ToString().Trim())
                {
                    cell1.Value = "Card" + io["Card Number"].ToString() + " > " +
                              "Pin" + io["Pin Number"].ToString();// + " > " +"Num" + io["Number"].ToString();
                }
            }
        }
        
        /// <summary>
        /// Unmap io from simeca.mdb file, update IO and IOMap table
        /// </summary>
        /// <param name="ioNumber">io number to be unmap</param>
        private void UnMapIO(string ioNumber)
        {
            DBCall dbCall = new DBCall("//access//simeca.mdb");
            //clear io description,module base on number that match in update criteria on IO table
            string sqlUpdate = "UPDATE IO SET [Description] = null, [Module] = null WHERE [Number] = " + ioNumber;
            dbCall.Update(sqlUpdate);

            //set "IO Number" to -1 where name is match from io tree view selected node on IOMap table
            //         sqlUpdate = "UPDATE IOMap SET [IO Number] = -1 WHERE [Name] = '" + IOTreeView.SelectedNode.Text + "'";
            sqlUpdate = "UPDATE IOMap SET [IO Number] = -1 WHERE [IO Number] = " + ioNumber;
            dbCall.Update(sqlUpdate);
        }

        /// <summary>
        /// Map io to simeca.mdb file,update IO and IOMap table
        /// </summary>
        /// <param name="ioNumber">io number to map on this io</param>
        private void MapIO(string ioNumber, string name)
        {
            DBCall dbCall = new DBCall("//access//simeca.mdb");
            //set update string, fill in io description,module where number is match, update IO table
            string sqlUpdate = "UPDATE IO SET [Description] = '" + name + "', [Module] = '" +
                tab_Module.SelectedTab.Text + "' WHERE [Number] = " + ioNumber;
            dbCall.Update(sqlUpdate);

            //set update string, fill in io number where name is match,update IOMap table
            sqlUpdate = "UPDATE IOMap SET [IO Number] = " + ioNumber + " WHERE [Name] = '" + name + "'";
            dbCall.Update(sqlUpdate);
        }

        /// <summary>
        /// Map io to simeca.mdb file,update IO and IOMap table
        /// </summary>
        /// <param name="ioNumber">io number to map on this io</param>
        private void MapIO_WithModuleName(string ioNumber, string name , string ModuleName)
        {
            DBCall dbCall = new DBCall("//access//simeca.mdb");
            //set update string, fill in io description,module where number is match, update IO table
            string sqlUpdate = "UPDATE IO SET [Description] = '" + name + "', [Module] = '" +
               ModuleName + "' WHERE [Number] = " + ioNumber;
            dbCall.Update(sqlUpdate);

            //set update string, fill in io number where name is match,update IOMap table
            sqlUpdate = "UPDATE IOMap SET [IO Number] = " + ioNumber + " WHERE [Name] = '" + name + "'";
            dbCall.Update(sqlUpdate);
        }

        private void dgd_In0_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;
            string previous = "";
            if (dgd_In0.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_In0.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_In0.Rows.Count > 0)
                for (int i = 0; i < dgd_In0.Rows.Count; i++)
                {
                    if (dgd_In0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In0.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In1.Rows.Count > 0)
                for (int i = 0; i < dgd_In1.Rows.Count; i++)
                {
                    if (dgd_In1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In1.Rows[i].Cells[1].Value.ToString();
                    if ( current == previous)
                    {          
                        dgd_In0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }  
                }
            if (dgd_In2.Rows.Count > 0)
                for (int i = 0; i < dgd_In2.Rows.Count; i++)
                {
                    if (dgd_In2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }  
                }
            if (dgd_In3.Rows.Count > 0)
                for (int i = 0; i < dgd_In3.Rows.Count; i++)
                {
                    if (dgd_In3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In4.Rows.Count > 0)
                for (int i = 0; i < dgd_In4.Rows.Count; i++)
                {
                    if (dgd_In4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In4.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In5.Rows.Count > 0)
                for (int i = 0; i < dgd_In5.Rows.Count; i++)
                {
                    if (dgd_In5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In6.Rows.Count > 0)
                for (int i = 0; i < dgd_In6.Rows.Count; i++)
                {
                    if (dgd_In6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In7.Rows.Count > 0)
                for (int i = 0; i < dgd_In7.Rows.Count; i++)
                {
                    if (dgd_In7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;
           
            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_In0.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_In0.Rows[e.RowIndex].Cells[2].Value.ToString();               
                string ioNum = "";
                string clone = dgd_In0.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ","");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In0.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_In0.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrInNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if(Num!="")
                UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_In0.Rows[e.RowIndex].Cells[2].Value = ioNum;
                dgd_In0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }
 
        private void dgd_In1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;
            string previous = "";
            if (dgd_In1.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_In1.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_In0.Rows.Count > 0)
                for (int i = 0; i < dgd_In0.Rows.Count; i++)
                {
                    if (dgd_In0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In0.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In1.Rows.Count > 0)
                for (int i = 0; i < dgd_In1.Rows.Count; i++)
                {
                    if (dgd_In1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In1.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In2.Rows.Count > 0)
                for (int i = 0; i < dgd_In2.Rows.Count; i++)
                {
                    if (dgd_In2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In3.Rows.Count > 0)
                for (int i = 0; i < dgd_In3.Rows.Count; i++)
                {
                    if (dgd_In3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In4.Rows.Count > 0)
                for (int i = 0; i < dgd_In4.Rows.Count; i++)
                {
                    if (dgd_In4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In4.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In5.Rows.Count > 0)
                for (int i = 0; i < dgd_In5.Rows.Count; i++)
                {
                    if (dgd_In5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In6.Rows.Count > 0)
                for (int i = 0; i < dgd_In6.Rows.Count; i++)
                {
                    if (dgd_In6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In7.Rows.Count > 0)
                for (int i = 0; i < dgd_In7.Rows.Count; i++)
                {
                    if (dgd_In7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;
            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_In1.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num= dgd_In1.Rows[e.RowIndex].Cells[2].Value.ToString();              
                string ioNum = "";
                string clone = dgd_In1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In1.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_In1.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrInNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                dgd_In1.Rows[e.RowIndex].Cells[2].Value = ioNum;
                dgd_In1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
            }
        }
     
        private void dgd_In2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_In2.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_In2.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_In0.Rows.Count > 0)
                for (int i = 0; i < dgd_In0.Rows.Count; i++)
                {
                    if (dgd_In0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In0.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In1.Rows.Count > 0)
                for (int i = 0; i < dgd_In1.Rows.Count; i++)
                {
                    if (dgd_In1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In2.Rows.Count > 0)
                for (int i = 0; i < dgd_In2.Rows.Count; i++)
                {
                    if (dgd_In2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In2.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In3.Rows.Count > 0)
                for (int i = 0; i < dgd_In3.Rows.Count; i++)
                {
                    if (dgd_In3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In4.Rows.Count > 0)
                for (int i = 0; i < dgd_In4.Rows.Count; i++)
                {
                    if (dgd_In4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In4.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In5.Rows.Count > 0)
                for (int i = 0; i < dgd_In5.Rows.Count; i++)
                {
                    if (dgd_In5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In6.Rows.Count > 0)
                for (int i = 0; i < dgd_In6.Rows.Count; i++)
                {
                    if (dgd_In6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In7.Rows.Count > 0)
                for (int i = 0; i < dgd_In7.Rows.Count; i++)
                {
                    if (dgd_In7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;
            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_In2.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_In2.Rows[e.RowIndex].Cells[2].Value.ToString();            
                string ioNum = "";
                string clone = dgd_In2.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In2.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_In2.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrInNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_In2.Rows[e.RowIndex].Cells[2].Value =  ioNum;
                dgd_In2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }
     
        private void dgd_In3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_In3.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_In3.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_In0.Rows.Count > 0)
                for (int i = 0; i < dgd_In0.Rows.Count; i++)
                {
                    if (dgd_In0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In0.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In1.Rows.Count > 0)
                for (int i = 0; i < dgd_In1.Rows.Count; i++)
                {
                    if (dgd_In1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In2.Rows.Count > 0)
                for (int i = 0; i < dgd_In2.Rows.Count; i++)
                {
                    if (dgd_In2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In3.Rows.Count > 0)
                for (int i = 0; i < dgd_In3.Rows.Count; i++)
                {
                    if (dgd_In3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In3.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In4.Rows.Count > 0)
                for (int i = 0; i < dgd_In4.Rows.Count; i++)
                {
                    if (dgd_In4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In4.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In5.Rows.Count > 0)
                for (int i = 0; i < dgd_In5.Rows.Count; i++)
                {
                    if (dgd_In5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In6.Rows.Count > 0)
                for (int i = 0; i < dgd_In6.Rows.Count; i++)
                {
                    if (dgd_In6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In7.Rows.Count > 0)
                for (int i = 0; i < dgd_In7.Rows.Count; i++)
                {
                    if (dgd_In7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;
            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_In3.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_In3.Rows[e.RowIndex].Cells[2].Value.ToString();              
                string ioNum = "";
                string clone = dgd_In3.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin =clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In3.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_In3.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrInNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_In3.Rows[e.RowIndex].Cells[2].Value =  ioNum;
                dgd_In3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }
     
        private void dgd_In4_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_In4.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_In4.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_In0.Rows.Count > 0)
                for (int i = 0; i < dgd_In0.Rows.Count; i++)
                {
                    if (dgd_In0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In0.Rows[i].Cells[1].Value.ToString();
                    if ( current == previous)
                    {
                        dgd_In4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In1.Rows.Count > 0)
                for (int i = 0; i < dgd_In1.Rows.Count; i++)
                {
                    if (dgd_In1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In2.Rows.Count > 0)
                for (int i = 0; i < dgd_In2.Rows.Count; i++)
                {
                    if (dgd_In2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In3.Rows.Count > 0)
                for (int i = 0; i < dgd_In3.Rows.Count; i++)
                {
                    if (dgd_In3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In4.Rows.Count > 0)
                for (int i = 0; i < dgd_In4.Rows.Count; i++)
                {
                    if (dgd_In4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In4.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In5.Rows.Count > 0)
                for (int i = 0; i < dgd_In5.Rows.Count; i++)
                {
                    if (dgd_In5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In6.Rows.Count > 0)
                for (int i = 0; i < dgd_In6.Rows.Count; i++)
                {
                    if (dgd_In6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In7.Rows.Count > 0)
                for (int i = 0; i < dgd_In7.Rows.Count; i++)
                {
                    if (dgd_In7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;
            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_In4.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_In4.Rows[e.RowIndex].Cells[2].Value.ToString();
                string ioNum = "";
                string clone = dgd_In4.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In4.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_In4.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrInNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_In4.Rows[e.RowIndex].Cells[2].Value = ioNum;
                dgd_In4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }
      
        private void dgd_Out0_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_Out0.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_Out0.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_Out0.Rows.Count > 0)
                for (int i = 0; i < dgd_Out0.Rows.Count; i++)
                {
                    if (dgd_Out0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out0.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out1.Rows.Count > 0)
                for (int i = 0; i < dgd_Out1.Rows.Count; i++)
                {
                    if (dgd_Out1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out2.Rows.Count > 0)
                for (int i = 0; i < dgd_Out2.Rows.Count; i++)
                {
                    if (dgd_Out2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out3.Rows.Count > 0)
                for (int i = 0; i < dgd_Out3.Rows.Count; i++)
                {
                    if (dgd_Out3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out4.Rows.Count > 0)
                for (int i = 0; i < dgd_Out4.Rows.Count; i++)
                {
                    if (dgd_Out4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out4.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out5.Rows.Count > 0)
                for (int i = 0; i < dgd_Out5.Rows.Count; i++)
                {
                    if (dgd_Out5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out6.Rows.Count > 0)
                for (int i = 0; i < dgd_Out6.Rows.Count; i++)
                {
                    if (dgd_Out6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out7.Rows.Count > 0)
                for (int i = 0; i < dgd_Out7.Rows.Count; i++)
                {
                    if (dgd_Out7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;
            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_Out0.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_Out0.Rows[e.RowIndex].Cells[2].Value.ToString();
                string ioNum = "";
                string clone = dgd_Out0.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out0.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_Out0.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrOutNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_Out0.Rows[e.RowIndex].Cells[2].Value =  ioNum;
                dgd_Out0.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }
     
        /// <summary>
        /// Call once click on any cell of grid view,indicate that going to map on that io
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgd_Out1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if ( e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_Out1.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_Out1.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_Out0.Rows.Count > 0)
                for (int i = 0; i < dgd_Out0.Rows.Count; i++)
                {
                    if (dgd_Out0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out0.Rows[i].Cells[1].Value.ToString();
                    if ( current == previous)
                    {
                        dgd_Out1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out1.Rows.Count > 0)
                for (int i = 0; i < dgd_Out1.Rows.Count; i++)
                {
                    if (dgd_Out1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out1.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out2.Rows.Count > 0)
                for (int i = 0; i < dgd_Out2.Rows.Count; i++)
                {
                    if (dgd_Out2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out3.Rows.Count > 0)
                for (int i = 0; i < dgd_Out3.Rows.Count; i++)
                {
                    if (dgd_Out3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out4.Rows.Count > 0)
                for (int i = 0; i < dgd_Out4.Rows.Count; i++)
                {
                    if (dgd_Out4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out4.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out5.Rows.Count > 0)
                for (int i = 0; i < dgd_Out5.Rows.Count; i++)
                {
                    if (dgd_Out5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out6.Rows.Count > 0)
                for (int i = 0; i < dgd_Out6.Rows.Count; i++)
                {
                    if (dgd_Out6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out7.Rows.Count > 0)
                for (int i = 0; i < dgd_Out7.Rows.Count; i++)
                {
                    if (dgd_Out7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;
            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_Out1.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_Out1.Rows[e.RowIndex].Cells[2].Value.ToString();
                string ioNum = "";
                string clone = dgd_Out1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out1.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_Out1.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrOutNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                if (Num != "")
                    UnMapIO(Num);

                    MapIO(ioNum, ioName);
                    //refresh io dataset after update latest io map info 
                    GetIODataSet();
                dgd_Out1.Rows[e.RowIndex].Cells[2].Value = ioNum;
                dgd_Out1.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }

        private void dgd_Out2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_Out2.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_Out2.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_Out0.Rows.Count > 0)
                for (int i = 0; i < dgd_Out0.Rows.Count; i++)
                {
                    if (dgd_Out0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out0.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out1.Rows.Count > 0)
                for (int i = 0; i < dgd_Out1.Rows.Count; i++)
                {
                    if (dgd_Out1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out2.Rows.Count > 0)
                for (int i = 0; i < dgd_Out2.Rows.Count; i++)
                {
                    if (dgd_Out2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out2.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out3.Rows.Count > 0)
                for (int i = 0; i < dgd_Out3.Rows.Count; i++)
                {
                    if (dgd_Out3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out4.Rows.Count > 0)
                for (int i = 0; i < dgd_Out4.Rows.Count; i++)
                {
                    if (dgd_Out4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out4.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out5.Rows.Count > 0)
                for (int i = 0; i < dgd_Out5.Rows.Count; i++)
                {
                    if (dgd_Out5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out6.Rows.Count > 0)
                for (int i = 0; i < dgd_Out6.Rows.Count; i++)
                {
                    if (dgd_Out6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out7.Rows.Count > 0)
                for (int i = 0; i < dgd_Out7.Rows.Count; i++)
                {
                    if (dgd_Out7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;
            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_Out2.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_Out2.Rows[e.RowIndex].Cells[2].Value.ToString();
                string ioNum = "";
                string clone = dgd_Out2.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin =clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out2.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_Out2.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrOutNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_Out2.Rows[e.RowIndex].Cells[2].Value =  ioNum;
                dgd_Out2.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }

        private void dgd_Out3_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_Out3.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_Out3.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_Out0.Rows.Count > 0)
                for (int i = 0; i < dgd_Out0.Rows.Count; i++)
                {
                    if (dgd_Out0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out0.Rows[i].Cells[1].Value.ToString();
                    if ( current == previous)
                    {
                        dgd_Out3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out1.Rows.Count > 0)
                for (int i = 0; i < dgd_Out1.Rows.Count; i++)
                {
                    if (dgd_Out1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out2.Rows.Count > 0)
                for (int i = 0; i < dgd_Out2.Rows.Count; i++)
                {
                    if (dgd_Out2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out3.Rows.Count > 0)
                for (int i = 0; i < dgd_Out3.Rows.Count; i++)
                {
                    if (dgd_Out3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out3.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out4.Rows.Count > 0)
                for (int i = 0; i < dgd_Out4.Rows.Count; i++)
                {
                    if (dgd_Out4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out4.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out5.Rows.Count > 0)
                for (int i = 0; i < dgd_Out5.Rows.Count; i++)
                {
                    if (dgd_Out5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out6.Rows.Count > 0)
                for (int i = 0; i < dgd_Out6.Rows.Count; i++)
                {
                    if (dgd_Out6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out7.Rows.Count > 0)
                for (int i = 0; i < dgd_Out7.Rows.Count; i++)
                {
                    if (dgd_Out7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;
            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_Out3.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_Out3.Rows[e.RowIndex].Cells[2].Value.ToString();        
                string ioNum = "";
                string clone = dgd_Out3.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out3.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_Out3.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrOutNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_Out3.Rows[e.RowIndex].Cells[2].Value =  ioNum;
                dgd_Out3.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
         
        }
     
        private void dgd_Out4_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_Out4.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_Out4.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_Out0.Rows.Count > 0)
                for (int i = 0; i < dgd_Out0.Rows.Count; i++)
                {
                    if (dgd_Out0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out0.Rows[i].Cells[1].Value.ToString();
                    if ( current == previous)
                    {
                        dgd_Out4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out1.Rows.Count > 0)
                for (int i = 0; i < dgd_Out1.Rows.Count; i++)
                {
                    if (dgd_Out1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out2.Rows.Count > 0)
                for (int i = 0; i < dgd_Out2.Rows.Count; i++)
                {
                    if (dgd_Out2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out3.Rows.Count > 0)
                for (int i = 0; i < dgd_Out3.Rows.Count; i++)
                {
                    if (dgd_Out3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out4.Rows.Count > 0)
                for (int i = 0; i < dgd_Out4.Rows.Count; i++)
                {
                    if (dgd_Out4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out4.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out5.Rows.Count > 0)
                for (int i = 0; i < dgd_Out5.Rows.Count; i++)
                {
                    if (dgd_Out5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out6.Rows.Count > 0)
                for (int i = 0; i < dgd_Out6.Rows.Count; i++)
                {
                    if (dgd_Out6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out7.Rows.Count > 0)
                for (int i = 0; i < dgd_Out7.Rows.Count; i++)
                {
                    if (dgd_Out7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;

            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_Out4.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_Out4.Rows[e.RowIndex].Cells[2].Value.ToString();
                string ioNum = "";
                string clone = dgd_Out4.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out4.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_Out4.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrOutNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_Out4.Rows[e.RowIndex].Cells[2].Value = ioNum;
                dgd_Out4.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }
        private void IOTimer_Tick(object sender, EventArgs e)
        {
            //run till last channel to scan
            for (int i = 0; i < m_lastinputChannel; i++)
            {
                //assign local variable
                //get card no
                int cardNo = m_inputScanInfoList[i].CardNo;
                // get the channel
                int channel = m_inputScanInfoList[i].Channel - (cardNo * NUM_CHANNEL);
                // previous Byte of the channel
                byte channelPrev = m_inputScanInfoList[i].BytePrev;
                // obtain port byte from Share Memory
                byte ioByte = 0;

                    if ((cardNo < IO.InputByte.Length) && (channel < IO.InputByte[cardNo].Length))
                        ioByte = (byte)IO.InputByte[cardNo][channel];
                    else
                        ioByte = Convert.ToByte("123");

                // no changes, return
                if (ioByte == channelPrev)
                    continue;

                // get current bits status different with the one show in ioGridView 
                int iexclOR = channelPrev ^ ioByte;
                byte exclOR = (byte)iexclOR;

                //not same
                if (exclOR != 0)
                {
                    //update for all bit status under same channel
                    for (int j = 0; j <= 7; j++)
                    {
                        // if bit different 
                        if ((exclOR & (1 << j)) != 0)
                        {
                            int item = m_inputScanInfoList[i].ItemNo[j];
                                    if (item == 99)
                                    {
                                        continue;
                                    }

                            if ((ioByte & (1 << j)) != 0)
                            {
                                    dgd_Input.Rows[item].Cells[0].Value = (object)true;
                                    dgd_Input.Rows[item].Cells[1].Value = (object)InputImageListTree.Images[1];
                            }
                            else
                            {
                                    dgd_Input.Rows[item].Cells[0].Value = (object)false;
                                    dgd_Input.Rows[item].Cells[1].Value = (object)InputImageListTree.Images[0];
                            }  
                        }
                        Thread.Sleep(10);
                    }
                    // set the byte to previous, for future compare status change
                    m_inputScanInfoList[i].BytePrev = ioByte;
                }
                Thread.Sleep(10);
            }

            //run till last channel to scan
            for (int i = 0; i < m_lastoutputChannel; i++)
            {
                //assign local variable
                //get card no
                int cardNo = m_outputScanInfoList[i].CardNo;
                // get the channel
                int channel = m_outputScanInfoList[i].Channel - (cardNo * NUM_CHANNEL);
                // previous Byte of the channel
                byte channelPrev = m_outputScanInfoList[i].BytePrev;
                // obtain port byte from Share Memory
                byte ioByte = 0;

                    if ((cardNo < IO.OutputByte.Length) && (channel < IO.OutputByte[cardNo].Length))
                        ioByte = IO.OutputByte[cardNo][channel];
                    else
                        ioByte = Convert.ToByte("123");
                // no changes, return
                if (ioByte == channelPrev)
                    continue;
                // get current bits status different with the one show in ioGridView 
                int iexclOR = channelPrev ^ ioByte;
                byte exclOR = (byte)iexclOR;

                //not same
                if (exclOR != 0)
                {
                    //update for all bit status under same channel
                    for (int j = 0; j <= 7; j++)
                    {
                        // if bit different 
                        if ((exclOR & (1 << j)) != 0)
                        {
                            int item = m_outputScanInfoList[i].ItemNo[j];
                            if (item == 99)
                            {
                                continue;
                            }

                            if ((ioByte & (1 << j)) != 0)
                            {
                                    dgd_Output.Rows[item].Cells[0].Value = (object)true;
                                    dgd_Output.Rows[item].Cells[1].Value = (object)OutputImageListTree.Images[1];
                            }
                            else
                            {
                                    dgd_Output.Rows[item].Cells[0].Value = (object)false;
                                    dgd_Output.Rows[item].Cells[1].Value = (object)OutputImageListTree.Images[0];
                            }
                        }
                        Thread.Sleep(10);
                    }
                    // set the byte to previous, for future compare status change
                    m_outputScanInfoList[i].BytePrev = ioByte;
                }
                Thread.Sleep(10);
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            if(repeat)
            {
                SRMMessageBox.Show("Selected same output pin", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if(dgd_In0.Rows.Count>0)
                for(int i=0; i<dgd_In0.Rows.Count; i++ )
                {
                    if (dgd_In0.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the input pin with red color in " + tab_0.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_In0.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy input pin in " + tab_0.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_In0.Rows[i].Cells[0].Value.ToString();  
                        string ioNum = "";
                        string clone = dgd_In0.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In0.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_In0.Rows[i].Cells[1].Value);
                            ioNum = m_arrInNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_0.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();
                        dgd_In0.Rows[i].Cells[2].Value = ioNum;
                        dgd_In0.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_In1.Rows.Count > 0)
                for (int i = 0; i < dgd_In1.Rows.Count; i++)
                {
                    if (dgd_In1.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the input pin with red color in " + tab_1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_In1.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy input pin in " + tab_1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_In1.Rows[i].Cells[0].Value.ToString();                    
                        string ioNum = "";
                        string clone = dgd_In1.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In1.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_In1.Rows[i].Cells[1].Value);
                            ioNum = m_arrInNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_1.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();
                        dgd_In1.Rows[i].Cells[2].Value = ioNum;
                        dgd_In1.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_In2.Rows.Count > 0)
                for (int i = 0; i < dgd_In2.Rows.Count; i++)
                {
                    if (dgd_In2.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the input pin with red color in " + tab_2.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_In2.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy input pin in " + tab_2.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_In2.Rows[i].Cells[0].Value.ToString();                   
                        string ioNum = "";
                        string clone = dgd_In2.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In2.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_In2.Rows[i].Cells[1].Value);
                            ioNum = m_arrInNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_2.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();
                        dgd_In2.Rows[i].Cells[2].Value = ioNum;
                        dgd_In2.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_In3.Rows.Count > 0)
                for (int i = 0; i < dgd_In3.Rows.Count; i++)
                {
                    if (dgd_In3.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the input pin with red color in " + tab_3.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_In3.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy input pin in " + tab_3.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_In3.Rows[i].Cells[0].Value.ToString();
                        string ioNum = "";
                        string clone = dgd_In3.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In3.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_In3.Rows[i].Cells[1].Value);
                            ioNum = m_arrInNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_3.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();
                        dgd_In3.Rows[i].Cells[2].Value =  ioNum;
                        dgd_In3.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_In4.Rows.Count > 0)
                for (int i = 0; i < dgd_In4.Rows.Count; i++)
                {
                    if (dgd_In4.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the input pin with red color in " + tab_4.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_In4.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy input pin in " + tab_4.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_In4.Rows[i].Cells[0].Value.ToString();
                        string ioNum = "";
                        string clone = dgd_In4.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In4.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_In4.Rows[i].Cells[1].Value);
                            ioNum = m_arrInNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_4.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();
                        dgd_In4.Rows[i].Cells[2].Value = ioNum;
                        dgd_In4.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_In5.Rows.Count > 0)
                for (int i = 0; i < dgd_In5.Rows.Count; i++)
                {
                    if (dgd_In5.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the input pin with red color in " + tab_5.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_In5.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy input pin in " + tab_5.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_In5.Rows[i].Cells[0].Value.ToString();
                        string ioNum = "";
                        string clone = dgd_In5.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In5.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_In5.Rows[i].Cells[1].Value);
                            ioNum = m_arrInNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_5.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();
                        dgd_In5.Rows[i].Cells[2].Value = ioNum;
                        dgd_In5.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_In6.Rows.Count > 0)
                for (int i = 0; i < dgd_In6.Rows.Count; i++)
                {
                    if (dgd_In6.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the input pin with red color in " + tab_6.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_In6.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy input pin in " + tab_6.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_In6.Rows[i].Cells[0].Value.ToString();
                        string ioNum = "";
                        string clone = dgd_In6.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In6.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_In6.Rows[i].Cells[1].Value);
                            ioNum = m_arrInNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_6.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();
                        dgd_In6.Rows[i].Cells[2].Value = ioNum;
                        dgd_In6.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_In7.Rows.Count > 0)
                for (int i = 0; i < dgd_In7.Rows.Count; i++)
                {
                    if (dgd_In7.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the input pin with red color in " + tab_7.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_In7.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy input pin in " + tab_7.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_In7.Rows[i].Cells[0].Value.ToString();
                        string ioNum = "";
                        string clone = dgd_In7.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In7.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_In7.Rows[i].Cells[1].Value);
                            ioNum = m_arrInNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_7.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();
                        dgd_In7.Rows[i].Cells[2].Value = ioNum;
                        dgd_In7.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_Out0.Rows.Count > 0)
                for (int i = 0; i < dgd_Out0.Rows.Count; i++)
                {
                    if (dgd_Out0.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the output pin with red color in " + tab_0.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_Out0.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy output pin in " + tab_0.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_Out0.Rows[i].Cells[0].Value.ToString();
                        string ioNum = "";
                        string clone = dgd_Out0.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out0.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_Out0.Rows[i].Cells[1].Value);
                            ioNum = m_arrOutNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_0.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();
                        dgd_Out0.Rows[i].Cells[2].Value = ioNum;
                        dgd_Out0.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_Out1.Rows.Count > 0)
                for (int i = 0; i < dgd_Out1.Rows.Count; i++)
                {
                    if (dgd_Out1.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the output pin with red color in " + tab_1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_Out1.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy output pin in " + tab_1.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_Out1.Rows[i].Cells[0].Value.ToString();
                        string ioNum = "";
                        string clone = dgd_Out1.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out1.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_Out1.Rows[i].Cells[1].Value);
                            ioNum = m_arrOutNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_1.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();
                        dgd_Out1.Rows[i].Cells[2].Value = ioNum;
                        dgd_Out1.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_Out2.Rows.Count > 0)
                for (int i = 0; i < dgd_Out2.Rows.Count; i++)
                {
                    if (dgd_Out2.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the output pin with red color in " + tab_2.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_Out2.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy output pin in " + tab_2.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_Out2.Rows[i].Cells[0].Value.ToString(); 
                        string ioNum = "";
                        string clone = dgd_Out2.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out2.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_Out2.Rows[i].Cells[1].Value);
                            ioNum = m_arrOutNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_2.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();

                        dgd_Out2.Rows[i].Cells[2].Value = ioNum;
                        dgd_Out2.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_Out3.Rows.Count > 0)
                for (int i = 0; i < dgd_Out3.Rows.Count; i++)
                {
                    if (dgd_Out3.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the output pin with red color in " + tab_3.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_Out3.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy output pin in " + tab_3.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_Out3.Rows[i].Cells[0].Value.ToString();
                        string ioNum = "";
                        string clone = dgd_Out3.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out3.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_Out3.Rows[i].Cells[1].Value);
                            ioNum = m_arrOutNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_3.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();

                        dgd_Out3.Rows[i].Cells[2].Value = ioNum;
                        dgd_Out3.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_Out4.Rows.Count > 0)
                for (int i = 0; i < dgd_Out4.Rows.Count; i++)
                {
                    if (dgd_Out4.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the output pin with red color in " + tab_4.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_Out4.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy output pin in " + tab_4.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_Out4.Rows[i].Cells[0].Value.ToString();
                        string ioNum = "";
                        string clone = dgd_Out4.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out4.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_Out4.Rows[i].Cells[1].Value);
                            ioNum = m_arrOutNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_4.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();

                        dgd_Out4.Rows[i].Cells[2].Value = ioNum;
                        dgd_Out4.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_Out5.Rows.Count > 0)
                for (int i = 0; i < dgd_Out5.Rows.Count; i++)
                {
                    if (dgd_Out5.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the output pin with red color in " + tab_5.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_Out5.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy output pin in " + tab_5.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_Out5.Rows[i].Cells[0].Value.ToString();
                        string ioNum = "";
                        string clone = dgd_Out5.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out5.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_Out5.Rows[i].Cells[1].Value);
                            ioNum = m_arrOutNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_5.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();

                        dgd_Out5.Rows[i].Cells[2].Value = ioNum;
                        dgd_Out5.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_Out6.Rows.Count > 0)
                for (int i = 0; i < dgd_Out6.Rows.Count; i++)
                {
                    if (dgd_Out6.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the output pin with red color in " + tab_6.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_Out6.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy output pin in " + tab_6.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_Out6.Rows[i].Cells[0].Value.ToString();
                        string ioNum = "";
                        string clone = dgd_Out6.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out6.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_Out6.Rows[i].Cells[1].Value);
                            ioNum = m_arrOutNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_6.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();

                        dgd_Out6.Rows[i].Cells[2].Value = ioNum;
                        dgd_Out6.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            if (dgd_Out7.Rows.Count > 0)
                for (int i = 0; i < dgd_Out7.Rows.Count; i++)
                {
                    if (dgd_Out7.Rows[i].Cells[1].Style.BackColor == Color.Red)
                    {
                        SRMMessageBox.Show("Please select again the output pin with red color in " + tab_7.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else if (dgd_Out7.Rows[i].Cells[1].Value == null)
                    {
                        SRMMessageBox.Show("Emtpy output pin in " + tab_7.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        string ioName = dgd_Out7.Rows[i].Cells[0].Value.ToString();
                        string ioNum = "";
                        string clone = dgd_Out7.Rows[i].Cells[1].Value.ToString();
                        string selectedPin = clone.Replace(" ", "");
                        if (selectedPin != "")// || selectedPin != "UNMAP")
                        {
                            string[] Splited = selectedPin.Split('>');
                            DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out7.Rows[i].Cells[1];
                            int selectedIndex = comboCell.Items.IndexOf(dgd_Out7.Rows[i].Cells[1].Value);
                            ioNum = m_arrOutNumber[selectedIndex].ToString();
                            int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                            int pin = Convert.ToInt32(Splited[1].Substring(3));
                        }
                        MapIO_WithModuleName(ioNum, ioName, tab_7.Text);
                        //refresh io dataset after update latest io map info 
                        GetIODataSet();

                        dgd_Out7.Rows[i].Cells[2].Value = ioNum;
                        dgd_Out7.Rows[i].Cells[1].Style.BackColor = Color.White;
                    }
                }
            this.Enabled = false;
            this.Close();
        }

     private void btn_Extend_Click(object sender, EventArgs e)
        {
            // This will change the Form's Width and Height, respectively.
            if (this.Size == new Size(460, 650))
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    btn_Extend.Text = "Hide";
                else
                    btn_Extend.Text = "隐藏";
                this.Size = new Size(650, 650);
            }
            else if (this.Size == new Size(650, 650))
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    btn_Extend.Text = "Extend";
                else
                    btn_Extend.Text = "延伸";
                this.Size = new Size(460, 650);
            }
        }

        private void dgd_In5_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_In5.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_In5.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_In0.Rows.Count > 0)
                for (int i = 0; i < dgd_In0.Rows.Count; i++)
                {
                    if (dgd_In0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In0.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In1.Rows.Count > 0)
                for (int i = 0; i < dgd_In1.Rows.Count; i++)
                {
                    if (dgd_In1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In2.Rows.Count > 0)
                for (int i = 0; i < dgd_In2.Rows.Count; i++)
                {
                    if (dgd_In2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In3.Rows.Count > 0)
                for (int i = 0; i < dgd_In3.Rows.Count; i++)
                {
                    if (dgd_In3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In4.Rows.Count > 0)
                for (int i = 0; i < dgd_In4.Rows.Count; i++)
                {
                    if (dgd_In4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In4.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In5.Rows.Count > 0)
                for (int i = 0; i < dgd_In5.Rows.Count; i++)
                {
                    if (dgd_In5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In6.Rows.Count > 0)
                for (int i = 0; i < dgd_In6.Rows.Count; i++)
                {
                    if (dgd_In6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In7.Rows.Count > 0)
                for (int i = 0; i < dgd_In7.Rows.Count; i++)
                {
                    if (dgd_In7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;
            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_In5.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_In5.Rows[e.RowIndex].Cells[2].Value.ToString();
                string ioNum = "";
                string clone = dgd_In5.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In5.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_In5.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrInNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_In5.Rows[e.RowIndex].Cells[2].Value = ioNum;
                dgd_In5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }

        private void dgd_Out5_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_Out5.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_Out5.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_Out0.Rows.Count > 0)
                for (int i = 0; i < dgd_Out0.Rows.Count; i++)
                {
                    if (dgd_Out0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out0.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out1.Rows.Count > 0)
                for (int i = 0; i < dgd_Out1.Rows.Count; i++)
                {
                    if (dgd_Out1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out2.Rows.Count > 0)
                for (int i = 0; i < dgd_Out2.Rows.Count; i++)
                {
                    if (dgd_Out2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out3.Rows.Count > 0)
                for (int i = 0; i < dgd_Out3.Rows.Count; i++)
                {
                    if (dgd_Out3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out4.Rows.Count > 0)
                for (int i = 0; i < dgd_Out4.Rows.Count; i++)
                {
                    if (dgd_Out4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out4.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out5.Rows.Count > 0)
                for (int i = 0; i < dgd_Out5.Rows.Count; i++)
                {
                    if (dgd_Out5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out6.Rows.Count > 0)
                for (int i = 0; i < dgd_Out6.Rows.Count; i++)
                {
                    if (dgd_Out6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out7.Rows.Count > 0)
                for (int i = 0; i < dgd_Out7.Rows.Count; i++)
                {
                    if (dgd_Out7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;

            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_Out5.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_Out5.Rows[e.RowIndex].Cells[2].Value.ToString();
                string ioNum = "";
                string clone = dgd_Out5.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out5.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_Out5.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrOutNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_Out5.Rows[e.RowIndex].Cells[2].Value = ioNum;
                dgd_Out5.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }

        private void dgd_In6_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_In6.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_In6.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_In0.Rows.Count > 0)
                for (int i = 0; i < dgd_In0.Rows.Count; i++)
                {
                    if (dgd_In0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In0.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In1.Rows.Count > 0)
                for (int i = 0; i < dgd_In1.Rows.Count; i++)
                {
                    if (dgd_In1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In2.Rows.Count > 0)
                for (int i = 0; i < dgd_In2.Rows.Count; i++)
                {
                    if (dgd_In2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In3.Rows.Count > 0)
                for (int i = 0; i < dgd_In3.Rows.Count; i++)
                {
                    if (dgd_In3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In4.Rows.Count > 0)
                for (int i = 0; i < dgd_In4.Rows.Count; i++)
                {
                    if (dgd_In4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In4.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In5.Rows.Count > 0)
                for (int i = 0; i < dgd_In5.Rows.Count; i++)
                {
                    if (dgd_In5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In6.Rows.Count > 0)
                for (int i = 0; i < dgd_In6.Rows.Count; i++)
                {
                    if (dgd_In6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In7.Rows.Count > 0)
                for (int i = 0; i < dgd_In7.Rows.Count; i++)
                {
                    if (dgd_In7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;
            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_In6.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_In6.Rows[e.RowIndex].Cells[2].Value.ToString();
                string ioNum = "";
                string clone = dgd_In6.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In6.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_In6.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrInNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_In6.Rows[e.RowIndex].Cells[2].Value = ioNum;
                dgd_In6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }

        private void dgd_Out6_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_Out6.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_Out6.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_Out0.Rows.Count > 0)
                for (int i = 0; i < dgd_Out0.Rows.Count; i++)
                {
                    if (dgd_Out0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out0.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out1.Rows.Count > 0)
                for (int i = 0; i < dgd_Out1.Rows.Count; i++)
                {
                    if (dgd_Out1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out2.Rows.Count > 0)
                for (int i = 0; i < dgd_Out2.Rows.Count; i++)
                {
                    if (dgd_Out2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out3.Rows.Count > 0)
                for (int i = 0; i < dgd_Out3.Rows.Count; i++)
                {
                    if (dgd_Out3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out4.Rows.Count > 0)
                for (int i = 0; i < dgd_Out4.Rows.Count; i++)
                {
                    if (dgd_Out4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out4.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out5.Rows.Count > 0)
                for (int i = 0; i < dgd_Out5.Rows.Count; i++)
                {
                    if (dgd_Out5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out6.Rows.Count > 0)
                for (int i = 0; i < dgd_Out6.Rows.Count; i++)
                {
                    if (dgd_Out6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out7.Rows.Count > 0)
                for (int i = 0; i < dgd_Out7.Rows.Count; i++)
                {
                    if (dgd_Out7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;

            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_Out6.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_Out6.Rows[e.RowIndex].Cells[2].Value.ToString();
                string ioNum = "";
                string clone = dgd_Out6.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out6.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_Out6.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrOutNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_Out6.Rows[e.RowIndex].Cells[2].Value = ioNum;
                dgd_Out6.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }

        private void dgd_In7_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_In7.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_In7.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_In0.Rows.Count > 0)
                for (int i = 0; i < dgd_In0.Rows.Count; i++)
                {
                    if (dgd_In0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In0.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In1.Rows.Count > 0)
                for (int i = 0; i < dgd_In1.Rows.Count; i++)
                {
                    if (dgd_In1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In2.Rows.Count > 0)
                for (int i = 0; i < dgd_In2.Rows.Count; i++)
                {
                    if (dgd_In2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In3.Rows.Count > 0)
                for (int i = 0; i < dgd_In3.Rows.Count; i++)
                {
                    if (dgd_In3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_In7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In4.Rows.Count > 0)
                for (int i = 0; i < dgd_In4.Rows.Count; i++)
                {
                    if (dgd_In4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In4.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In5.Rows.Count > 0)
                for (int i = 0; i < dgd_In5.Rows.Count; i++)
                {
                    if (dgd_In5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In6.Rows.Count > 0)
                for (int i = 0; i < dgd_In6.Rows.Count; i++)
                {
                    if (dgd_In6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_In7.Rows.Count > 0)
                for (int i = 0; i < dgd_In7.Rows.Count; i++)
                {
                    if (dgd_In7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_In7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_In7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_In7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;
            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_In7.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_In7.Rows[e.RowIndex].Cells[2].Value.ToString();
                string ioNum = "";
                string clone = dgd_In7.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_In7.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_In7.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrInNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_In7.Rows[e.RowIndex].Cells[2].Value = ioNum;
                dgd_In7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }

        private void dgd_Out7_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)// || m_initialize )
                return;

            string previous = "";
            if (dgd_Out7.Rows[e.RowIndex].Cells[1].Value == null)
                return;
            string current = dgd_Out7.Rows[e.RowIndex].Cells[1].Value.ToString(); ;
            if (dgd_Out0.Rows.Count > 0)
                for (int i = 0; i < dgd_Out0.Rows.Count; i++)
                {
                    if (dgd_Out0.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out0.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out0.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out1.Rows.Count > 0)
                for (int i = 0; i < dgd_Out1.Rows.Count; i++)
                {
                    if (dgd_Out1.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out1.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out1.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out2.Rows.Count > 0)
                for (int i = 0; i < dgd_Out2.Rows.Count; i++)
                {
                    if (dgd_Out2.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out2.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out3.Rows.Count > 0)
                for (int i = 0; i < dgd_Out3.Rows.Count; i++)
                {
                    if (dgd_Out3.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out3.Rows[i].Cells[1].Value.ToString();
                    if (current == previous)
                    {
                        dgd_Out7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out4.Rows.Count > 0)
                for (int i = 0; i < dgd_Out4.Rows.Count; i++)
                {
                    if (dgd_Out4.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out4.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out4.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out5.Rows.Count > 0)
                for (int i = 0; i < dgd_Out5.Rows.Count; i++)
                {
                    if (dgd_Out5.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out5.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out5.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out6.Rows.Count > 0)
                for (int i = 0; i < dgd_Out6.Rows.Count; i++)
                {
                    if (dgd_Out6.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out6.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out6.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            if (dgd_Out7.Rows.Count > 0)
                for (int i = 0; i < dgd_Out7.Rows.Count; i++)
                {
                    if (dgd_Out7.Rows[i].Cells[1].Value == null)
                        continue;
                    previous = dgd_Out7.Rows[i].Cells[1].Value.ToString();
                    if (i != e.RowIndex && current == previous)
                    {
                        dgd_Out7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.Red;
                        dgd_Out7.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        repeat = true;
                        return;
                    }
                }
            repeat = false;

            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //get io name from io tree view node
                string ioName = dgd_Out7.Rows[e.RowIndex].Cells[0].Value.ToString();
                string Num = dgd_Out7.Rows[e.RowIndex].Cells[2].Value.ToString();
                string ioNum = "";
                string clone = dgd_Out7.Rows[e.RowIndex].Cells[1].Value.ToString();
                string selectedPin = clone.Replace(" ", "");
                if (selectedPin != "")// || selectedPin != "UNMAP")
                {
                    string[] Splited = selectedPin.Split('>');
                    DataGridViewComboBoxCell comboCell = (DataGridViewComboBoxCell)dgd_Out7.Rows[e.RowIndex].Cells[1];
                    int selectedIndex = comboCell.Items.IndexOf(dgd_Out7.Rows[e.RowIndex].Cells[1].Value);
                    ioNum = m_arrOutNumber[selectedIndex].ToString();
                    int cardNo = Convert.ToInt32(Splited[0].Substring(4));
                    int pin = Convert.ToInt32(Splited[1].Substring(3));
                }
                //check,set display image
                //get io number to map from grid view
                if (Num != "")
                    UnMapIO(Num);
                MapIO(ioNum, ioName);
                //refresh io dataset after update latest io map info 
                GetIODataSet();
                dgd_Out7.Rows[e.RowIndex].Cells[2].Value = ioNum;
                dgd_Out7.Rows[e.RowIndex].Cells[1].Style.BackColor = Color.White;
            }
        }
    }
}
