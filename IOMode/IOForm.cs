using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Common;
using User;
using SharedMemory;


namespace IOMode
{
    public partial class IOForm : Form
    {

        #region Members Variables

        private bool m_viewByModule = true;
        private int m_group = 0;
        private int m_lastChannelInput = 0, m_lastChannelOutput = 0;
        private int m_selectedIOIndex = 0;
        private int m_ioMask = 0;
        private int m_intIOInfoItem = -1;  // 0 = input card, 1 = output card
        private string m_module = "";
        private byte[] m_inputByteList = new byte[32];   //To store Input bits
        private byte[] m_outputByteList = new byte[32];   //To store Output bits
        private IOScanInfo[] m_InputScanInfoList = new IOScanInfo[32];    // contain bit information to be scanned
        private IOScanInfo[] m_OutputScanInfoList = new IOScanInfo[32];    // contain bit information to be scanned 
        private DataSet m_ioDataSet = new DataSet();
        private DBCall m_IODBCall = new DBCall("\\access\\simeca.mdb");
        private IOInfo[] m_selectedIOList = new IOInfo[8];
        private IODiagnosticForm m_ioDiagnosticForm;
        private ProductionInfo m_smProductionInfo = new ProductionInfo();

        #endregion

        public IOForm(ProductionInfo smProductionInfo, int group)
        {
            InitializeComponent();

            m_smProductionInfo = smProductionInfo;
            m_group = group;
                       
            ModuleRadioButton.Checked = false;
            CardRadioButton.Checked = true;
            IODiagnosticButton.Enabled = false; 
            m_viewByModule = false;

            //create data set
            m_ioDataSet = new DataSet();
            //fill data set with table name "IO"
            m_IODBCall.Select("SELECT * FROM IO", m_ioDataSet, "IO");

            FillIOModule();

            SynchronizeCheckBox.Checked = true;
        }

        

        private void CloseDiagnosticBox()
        {
            Form[] mdiChild = this.MdiParent.MdiChildren;
            int i = 0;
            for (i = 0; i < mdiChild.Length; i++)
            {
                if (mdiChild[i].Name == "IO Diagnostic")
                {
                    mdiChild[i].Close();
                    mdiChild[i].Dispose();
                    break;
                }
            }
        }

        /// <summary>
        ///  Fill IO Input and Output Combo Box : Card Number / Module Name
        /// </summary>
        private void FillIOModule()
        {
            try
            {
                string sortStr = "";
                string subInputText = "";
                DataRow[] IOList;              

                InputIOComboBox.Items.Clear();
                OutputIOComboBox.Items.Clear();

                DataSet moduleDataSet = new DataSet();                       
                if (m_viewByModule)
                {
                    lblInputIO.Text = lblOutputIO.Text = "Module :";
                    sortStr = "Module";
                    
                    m_IODBCall.Select("SELECT DISTINCT [Module] FROM IO", moduleDataSet);      
                }
                else
                {
                    lblInputIO.Text = lblOutputIO.Text = "Card Number :";
                    sortStr = "Card Number";

                    m_IODBCall.Select("SELECT DISTINCT [Card Number] FROM IO", moduleDataSet);                    
                }

                IOList = moduleDataSet.Tables[0].Select("", sortStr);
                moduleDataSet.Dispose();
                
                foreach (DataRow inputIO in IOList)
                {                   
                    subInputText = inputIO[0].ToString();

                    if (subInputText != "")
                    {
                        InputIOComboBox.Items.Add(subInputText);
                        OutputIOComboBox.Items.Add(subInputText);
                    }
                }

                if (InputIOComboBox.Items.Count > 0)
                {
                    InputIOComboBox.SelectedIndex = 0;
                    OutputIOComboBox.SelectedIndex = 0;
                }                  
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception Error: \n" + ex.ToString(), "IO Trigger", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Call when activate autoIO
        /// </summary>
        /// <param name="index">autoIO channel to monitor,max 4 channel</param>
        private void GetIOInfo(int index, DataGridView dgdControl)
        {
            m_selectedIOList[index].Number = dgdControl.Rows[m_selectedIOIndex].Cells[2].Value.ToString();
            m_selectedIOList[index].CardNo = Convert.ToInt32(dgdControl.Rows[m_selectedIOIndex].Cells[4].Value);
            m_selectedIOList[index].Channel = Convert.ToInt32(dgdControl.Rows[m_selectedIOIndex].Cells[5].Value);
            m_selectedIOList[index].Bit = Convert.ToInt32(dgdControl.Rows[m_selectedIOIndex].Cells[6].Value);
            m_selectedIOList[index].Type = m_module;
            m_selectedIOList[index].Description = dgdControl.Rows[m_selectedIOIndex].Cells[3].Value.ToString();

            dgdControl.Rows[m_selectedIOIndex].DefaultCellStyle.ForeColor = Color.Red;
            dgdControl.Rows[m_selectedIOIndex].DefaultCellStyle.SelectionForeColor = Color.Red;
        }

        private bool IsIOSelected()
        {
            if (m_intIOInfoItem == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (m_selectedIOList[i].Number == InputIODataGridView.Rows[m_selectedIOIndex].Cells["ColumnInputIONumber"].Value.ToString())
                    {
                        MessageBox.Show(m_selectedIOList[i].Type + " IO-" + m_selectedIOList[i].Number + " was selected for Channel " + Convert.ToString(i + 1) + "! Please select others IO.", "IO Diagnostic", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    if (m_selectedIOList[i].Number == OutputIODataGridView.Rows[m_selectedIOIndex].Cells["ColumnOutputIONumber"].Value.ToString())
                    {
                        MessageBox.Show(m_selectedIOList[i].Type + " IO-" + m_selectedIOList[i].Number + " was selected for Channel " + Convert.ToString(i + 1) + "! Please select others IO.", "IO Diagnostic", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return true;
                    }
                }
            }

            return false;
        }

        private void ResetIODiagnosticChannel(int index)
        {
            for (int i = 0; i < InputIODataGridView.Rows.Count; i++)
            {
                if (InputIODataGridView.Rows[i].Cells["ColumnInputIONumber"].Value.ToString() == m_selectedIOList[index].Number)
                {
                    InputIODataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                    InputIODataGridView.Rows[i].DefaultCellStyle.SelectionForeColor = Color.White;
                    return;
                }
            }

            for (int i = 0; i < OutputIODataGridView.Rows.Count; i++)
            {
                if (OutputIODataGridView.Rows[i].Cells["ColumnOutputIONumber"].Value.ToString() == m_selectedIOList[index].Number)
                {
                    OutputIODataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                    OutputIODataGridView.Rows[i].DefaultCellStyle.SelectionForeColor = Color.White;
                    return;
                }
            }
        }

        /// <summary>
        /// Call when want to remove this channel from monitoring 
        /// </summary>
        /// <param name="index">autoIO channel to remove</param>
        private void ResetIOInfo(int index)
        {
            bool blnFound = false;
            for (int i = 0; !blnFound && i < InputIODataGridView.Rows.Count; i++)
            {
                if (InputIODataGridView.Rows[i].Cells["ColumnInputIONumber"].Value.ToString() == m_selectedIOList[index].Number)
                {
                    InputIODataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                    InputIODataGridView.Rows[i].DefaultCellStyle.SelectionForeColor = Color.White;
                    blnFound = true;
                }
            }

            for (int i = 0; !blnFound && i < OutputIODataGridView.Rows.Count; i++)
            {
                if (OutputIODataGridView.Rows[i].Cells["ColumnOutputIONumber"].Value.ToString() == m_selectedIOList[index].Number)
                {
                    OutputIODataGridView.Rows[i].DefaultCellStyle.ForeColor = Color.Black;
                    OutputIODataGridView.Rows[i].DefaultCellStyle.SelectionForeColor = Color.White;
                    blnFound = true;
                }
            }

            m_selectedIOList[index].Number = "";
            m_selectedIOList[index].CardNo = 0;
            m_selectedIOList[index].Channel = 0;
            m_selectedIOList[index].Bit = 0;
            m_selectedIOList[index].Type = "";
            m_selectedIOList[index].Description = "";
        }





        private void Select1MenuItem_Click(object sender, EventArgs e)
        {
            if (IsIOSelected())
                return;
            CloseDiagnosticBox();

            ResetIODiagnosticChannel(0);
            m_ioMask |= 0x01;
            if (m_intIOInfoItem == 0)
            {
                m_module = "Input";
                GetIOInfo(0, InputIODataGridView);
            }
            else
            {
                m_module = "Output";
                GetIOInfo(0, OutputIODataGridView);
            }

            Remove1MenuItem.Text = "Remove IO-" + m_selectedIOList[0].Number + " from Channel 1";
            Remove1MenuItem.Enabled = true;
        }

        private void Select2MenuItem_Click(object sender, EventArgs e)
        {
            if (IsIOSelected())
                return;
            CloseDiagnosticBox();

            ResetIODiagnosticChannel(1);
            m_ioMask |= 0x02;
            if (m_intIOInfoItem == 0)
            {
                m_module = "Input";
                GetIOInfo(1, InputIODataGridView);
            }
            else
            {
                m_module = "Output";
                GetIOInfo(1, OutputIODataGridView);
            }

            Remove2MenuItem.Text = "Remove IO-" + m_selectedIOList[1].Number + " from Channel 2";
            Remove2MenuItem.Enabled = true;
        }

        private void Select3MenuItem_Click(object sender, EventArgs e)
        {
            if (IsIOSelected())
                return;
            CloseDiagnosticBox();

            ResetIODiagnosticChannel(2);
            m_ioMask |= 0x04;
            if (m_intIOInfoItem == 0)
            {
                m_module = "Input";
                GetIOInfo(2, InputIODataGridView);
            }
            else
            {
                m_module = "Output";
                GetIOInfo(2, OutputIODataGridView);
            }

            Remove3MenuItem.Text = "Remove IO-" + m_selectedIOList[2].Number + " from Channel 3";
            Remove3MenuItem.Enabled = true;
        }

        private void Select4MenuItem_Click(object sender, EventArgs e)
        {
            if (IsIOSelected())
                return;
            CloseDiagnosticBox();

            ResetIODiagnosticChannel(3);
            m_ioMask |= 0x08;
            if (m_intIOInfoItem == 0)
            {
                m_module = "Input";
                GetIOInfo(3, InputIODataGridView);
            }
            else
            {
                m_module = "Output";
                GetIOInfo(3, OutputIODataGridView);
            }
            Remove4MenuItem.Text = "Remove IO-" + m_selectedIOList[3].Number + " from Channel 4";
            Remove4MenuItem.Enabled = true;
        }

        private void Remove1MenuItem_Click(object sender, EventArgs e)
        {
            CloseDiagnosticBox();
            m_ioMask &= ~0x01;
            ResetIOInfo(0);

            Remove1MenuItem.Text = "Remove from Channel 1";
            Remove1MenuItem.Enabled = false;
        }

        private void Remove2MenuItem_Click(object sender, EventArgs e)
        {
            CloseDiagnosticBox();
            m_ioMask &= ~0x02;
            ResetIOInfo(1);

            Remove2MenuItem.Text = "Remove from Channel 2";
            Remove2MenuItem.Enabled = false;
        }

        private void Remove3MenuItem_Click(object sender, EventArgs e)
        {
            CloseDiagnosticBox();
            m_ioMask &= ~0x04;
            ResetIOInfo(2);

            Remove3MenuItem.Text = "Remove from Channel 3";
            Remove3MenuItem.Enabled = false;
        }

        private void Remove4MenuItem_Click(object sender, EventArgs e)
        {
            CloseDiagnosticBox();
            m_ioMask &= ~0x08;
            ResetIOInfo(3);

            Remove4MenuItem.Text = "Remove from Channel 4";
            Remove4MenuItem.Enabled = false;
        }



        private void WantIODiagnosticCheckBox_Click(object sender, EventArgs e)
        {
            if (WantIODiagnosticCheckBox.Checked)
                IODiagnosticButton.Enabled = true;
            else
                IODiagnosticButton.Enabled = false;
        }

        private void IODiagnosticButton_Click(object sender, EventArgs e)
        {
            if (m_ioMask > 0)
            {
                Cursor.Current = Cursors.WaitCursor;

                Form[] childForm = this.MdiParent.MdiChildren;
                int i = 0;
                for (i = 0; i < childForm.Length; i++)
                {
                    if (childForm[i].Name == "IO Diagnostic")
                    {
                        childForm[i].Activate();
                        break;
                    }
                }

                if (i == childForm.Length)
                {
                    m_ioDiagnosticForm = new IODiagnosticForm();
                    m_ioDiagnosticForm.Name = "IO Diagnostic";
                    m_ioDiagnosticForm.IOMask = m_ioMask;
                    m_ioDiagnosticForm.IOList = m_selectedIOList;
                    m_ioDiagnosticForm.MdiParent = this.MdiParent;
                    m_ioDiagnosticForm.Show();
                }                    
            }
            else
                SRMMessageBox.Show("Please select an IO.", "IO Trigger",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }


        private void InputIODataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 2)
            {
                if (WantIODiagnosticCheckBox.Checked)
                {
                    m_intIOInfoItem = 0;
                    m_selectedIOIndex = e.RowIndex;
                    IOContextMenuStrip.Show(MousePosition.X, MousePosition.Y);
                }
            }

            InputIODataGridView.Invalidate();
            InputIOGroupBox.Invalidate();
            InputIOComboBox.Invalidate();
        }

        private void OutputIODataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (e.ColumnIndex == 1)
            {
                if (m_smProductionInfo.AT_ALL_InAuto)
                    return;

                int cardNo = Convert.ToInt32(OutputIODataGridView.Rows[e.RowIndex].Cells[4].Value);   
                int channel = Convert.ToInt32(OutputIODataGridView.Rows[e.RowIndex].Cells[5].Value);
                int bit = Convert.ToInt32(OutputIODataGridView.Rows[e.RowIndex].Cells[6].Value);
                int bitStatus = 0;

                // to trigger off an output bit 
                if ((bool)OutputIODataGridView.Rows[e.RowIndex].Cells[0].Value)
                {
                    bitStatus = 0;
                    OutputIODataGridView.Rows[e.RowIndex].Cells[0].Value = false;
                    OutputIODataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (object)OutputIOImageList.Images[0];
                }
                //to trigger on an output bit
                else
                {
                    bitStatus = 1;
                    OutputIODataGridView.Rows[e.RowIndex].Cells[0].Value = true;
                    OutputIODataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (object)OutputIOImageList.Images[1];
                }

                //update to output byte
                IO.OutPort(cardNo, channel, bit, bitStatus);

                //update previous byte
                byte ioByte = 0;
                int intListChannel = channel - (cardNo * IO.ref_NUM_CHANNEL);
                if ((cardNo < IO.OutputByte.Length) && (intListChannel < IO.OutputByte[cardNo].Length))
                    ioByte = IO.OutputByte[cardNo][intListChannel];
                else
                    ioByte = Convert.ToByte("123");
                m_OutputScanInfoList[intListChannel].BytePrev = ioByte;
            }
            else if (e.ColumnIndex == 2)
            {
                if (WantIODiagnosticCheckBox.Checked)
                {
                    m_intIOInfoItem = 1;
                    m_selectedIOIndex = e.RowIndex;
                    IOContextMenuStrip.Show(MousePosition.X, MousePosition.Y);
                }
            }

            OutputIODataGridView.Invalidate();
            OutputIOGroupBox.Invalidate();
            OutputIOComboBox.Invalidate();
        }

        private void InputIOComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
                // remove all elements in the arrays
                Array.Clear(m_InputScanInfoList, 0, m_InputScanInfoList.Length);
                m_lastChannelInput = 0;

                InputIODataGridView.Rows.Clear();

                string selectedItem = InputIOComboBox.Text.ToString();
                string filterStr = "";             
                int channel = 0;
                int bit = 0;
                int row = 0;
                int currentChannel = 0;
                string ioDescription;

                m_lastChannelInput = 0;

                if (m_viewByModule)
                    filterStr = "Type = 'Input' AND Module = '" + selectedItem + "'";
                else
                    filterStr = "Type = 'Input' AND [Card Number] = '" + selectedItem + "'";

                DataRow[] inputIOList = m_ioDataSet.Tables[0].Select(filterStr, "Number");
                foreach (DataRow inputIO in inputIOList)
                {
                    ioDescription = inputIO["Description"].ToString();
                              
                    InputIODataGridView.Rows.Add(false,
                                                 (object)InputIOImageList.Images[0],
                                                 inputIO["Number"].ToString(),
                                                 inputIO["Description"].ToString(),
                                                 inputIO["Card Number"].ToString(),
                                                 inputIO["Channel Number"].ToString(),
                                                 inputIO["Bit"].ToString());

                    for (int IOChannel = 0; IOChannel < 4; IOChannel++)
                    {
                        if (m_selectedIOList[IOChannel].Number == inputIO["Number"].ToString())
                        {
                            InputIODataGridView.Rows[InputIODataGridView.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.Red;
                            InputIODataGridView.Rows[InputIODataGridView.Rows.Count - 1].DefaultCellStyle.SelectionForeColor = Color.Red;
                            break;
                        }
                    }

                    channel = Convert.ToInt32(inputIO["Channel Number"]);
                    bit = Convert.ToInt32(inputIO["Bit"]);

                    int i;
                    bool newChannel = false;
                    // if 0 channel, add new to the array
                    if (m_lastChannelInput == 0 && currentChannel == 0)
                        newChannel = true;
                    else
                    {
                        // there are channels in the list
                        if (m_InputScanInfoList[currentChannel].Channel != channel)
                        {
                            for (i = 0; i < m_lastChannelInput; i++)
                            {
                                // channel already exists
                                if (m_InputScanInfoList[i].Channel == channel)
                                {
                                    currentChannel = i;
                                    break;
                                }
                            }

                            // the channel is not in the array, add a new channel
                            if (i == m_lastChannelInput)
                                newChannel = true;
                        }
                    }

                    // add a channel
                    if (newChannel)
                    {
                        m_InputScanInfoList[m_lastChannelInput] = new IOScanInfo(0);
                        m_InputScanInfoList[m_lastChannelInput].CardNo = Convert.ToInt32(inputIO["Card Number"]);
                        m_InputScanInfoList[m_lastChannelInput].Channel = channel;
                        currentChannel = m_lastChannelInput;
                        m_lastChannelInput++;

                        for (int a = 0; a < 8; a++)
                            m_InputScanInfoList[currentChannel].ItemNo[a] = 99;
                    }

                    // set the result to position itembit array
                    m_InputScanInfoList[currentChannel].ItemBit |= (byte)(1 << bit);
                    // add the item number to the list
                    m_InputScanInfoList[currentChannel].ItemNo[bit] = row;
                    // item added, set previous byte to 0, so it can be updated
                    m_InputScanInfoList[currentChannel].BytePrev = 0;

                    row++;
                }                

                if (SynchronizeCheckBox.Checked && InputIODataGridView.Rows.Count > 0)
                {
                    string OutputIOSelectedItem = InputIOComboBox.SelectedItem.ToString();
                    
                    if (OutputIOComboBox.Items.IndexOf(OutputIOSelectedItem) >= 0)
                        OutputIOComboBox.SelectedItem = OutputIOSelectedItem;
                }
            }
    
        private void OutputIOComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // remove all elements in the arrays
            Array.Clear(m_OutputScanInfoList, 0, m_OutputScanInfoList.Length);
            m_lastChannelOutput = 0;

            OutputIODataGridView.Rows.Clear();

            string filterStr = "";
            string selectedItem = OutputIOComboBox.Text.ToString();        
            int channel = 0;
            int bit = 0;
            int row = 0;
            int currentChannel = 0;
            string ioDescription;
            m_lastChannelOutput = 0;

            if (m_viewByModule)
                filterStr = "Type = 'Output' AND Module = '" + selectedItem + "'";
            else
                filterStr = "Type = 'Output' AND [Card Number] = '" + selectedItem + "'";
            
            DataRow[] outputIOList = m_ioDataSet.Tables[0].Select(filterStr, "Number");

            foreach (DataRow outputIO in outputIOList)
            {
                ioDescription = outputIO["Description"].ToString();            
                    
                OutputIODataGridView.Rows.Add(false,
                                              (object)OutputIOImageList.Images[0],
                                              outputIO["Number"].ToString(),
                                              outputIO["Description"].ToString(),
                                              outputIO["Card Number"].ToString(),
                                              outputIO["Channel Number"].ToString(),
                                              outputIO["Bit"].ToString(),
                                              outputIO["Group"].ToString());

                for (int IOChannel = 0; IOChannel < 4; IOChannel++)
                {
                    if (m_selectedIOList[IOChannel].Number == outputIO["Number"].ToString())
                    {
                        OutputIODataGridView.Rows[OutputIODataGridView.Rows.Count - 1].DefaultCellStyle.ForeColor = Color.Red;
                        OutputIODataGridView.Rows[OutputIODataGridView.Rows.Count - 1].DefaultCellStyle.SelectionForeColor = Color.Red;
                        break;
                    }
                }

                channel = Convert.ToInt32(outputIO["Channel Number"]);
                bit = Convert.ToInt32(outputIO["Bit"]);

                int i;
                bool newChannel = false;
                // if 0 channel, add new to the array
                if (m_lastChannelOutput == 0 && currentChannel == 0)
                    newChannel = true;
                else
                {
                    // there are channels in the list
                    if (m_OutputScanInfoList[currentChannel].Channel != channel)
                    {
                        for (i = 0; i < m_lastChannelOutput; i++)
                        {
                            // channel already exists
                            if (m_OutputScanInfoList[i].Channel == channel)
                            {
                                currentChannel = i;
                                break;
                            }
                        }

                        // the channel is not in the array, add a new channel
                        if (i == m_lastChannelOutput)
                            newChannel = true;
                    }
                }

                // add a channel
                if (newChannel)
                {
                    m_OutputScanInfoList[m_lastChannelOutput] = new IOScanInfo(0);
                    m_OutputScanInfoList[m_lastChannelOutput].CardNo = Convert.ToInt32(outputIO["Card Number"]);
                    m_OutputScanInfoList[m_lastChannelOutput].Channel = channel;
                    currentChannel = m_lastChannelOutput;
                    m_lastChannelOutput++;

                    for (int a = 0; a < 8; a++)
                        m_OutputScanInfoList[currentChannel].ItemNo[a] = 99;
                }

                // set the result to position itembit array
                m_OutputScanInfoList[currentChannel].ItemBit |= (byte)(1 << bit);
                // add the item number to the list
                m_OutputScanInfoList[currentChannel].ItemNo[bit] = row;
                // item added, set previous byte to 0, so it can be updated
                m_OutputScanInfoList[currentChannel].BytePrev = 0;

                row++;
            }                        

            if (SynchronizeCheckBox.Checked)
            {
                string InputIOSelectedItem = OutputIOComboBox.SelectedItem.ToString();
               
                if (InputIOComboBox.Items.IndexOf(InputIOSelectedItem) >= 0)
                    InputIOComboBox.SelectedItem = InputIOSelectedItem;
            }
        }




        private void CloseButton_Click(object sender, EventArgs e)
        {
            Form[] childForm = this.MdiParent.MdiChildren;
            int i = 0;
            for (i = 0; i < childForm.Length; i++)
            {
                if (childForm[i].Name == "IO Diagnostic")
                {
                    SRMMessageBox.Show("Please close IO diagnostic first", "SRM Vision");
                    return;
                }
            }

            Close();
            Dispose();
        }

        private void RadioButton_Click(object sender, EventArgs e)
        {
            bool blnChanged = false;
            if (ModuleRadioButton.Checked && !m_viewByModule)
            {
                m_viewByModule = true;
                blnChanged = true;
            }
            else if (CardRadioButton.Checked && m_viewByModule)
            {
                m_viewByModule = false;
                blnChanged = true;
            }

            if (blnChanged)
                FillIOModule();
        }





        private void IOForm_Activated(object sender, EventArgs e)
        {
            if (m_ioDiagnosticForm != null)
                m_ioDiagnosticForm.BringToFront();
        }

        private void IOTriggerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Enabled = false;
        }


        private void OutputIOTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < m_lastChannelOutput; i++)
                {
                    //assign local variable
                    //get card no
                    int cardNo = m_OutputScanInfoList[i].CardNo;
                    // get the channel
                    int channel = m_OutputScanInfoList[i].Channel - (cardNo * IO.ref_NUM_CHANNEL);
                    // previous Byte of the channel
                    byte channelPrev = m_OutputScanInfoList[i].BytePrev;
                    // obtain port byte from Share Memory
                    byte ioByte = 0;


                    if ((cardNo < IO.OutputByte.Length) && (channel < IO.OutputByte[cardNo].Length))
                        ioByte = IO.OutputByte[cardNo][channel];
                    else
                        ioByte = Convert.ToByte("123");


                    // no changes, return
                    if (ioByte == channelPrev)
                        continue;

                    // get bits in the view that are different 
                    int iexclOR = channelPrev ^ ioByte;
                    byte exclOR = (byte)iexclOR;

                    if (exclOR != 0)
                    {
                        for (int j = 0; j <= 7; j++)
                        {
                            int item = m_OutputScanInfoList[i].ItemNo[j];

                            if (item == 99)
                                continue;

                            // if bit different 
                            if ((exclOR & (1 << j)) != 0)
                            {
                                if ((ioByte & (1 << j)) != 0)
                                {
                                    // Set Icon	
                                    OutputIODataGridView.Rows[item].Cells["ColumnOutputIOOnOff"].Value = true;
                                    OutputIODataGridView.Rows[item].Cells["ColumnOutputIOState"].Value = (object)OutputIOImageList.Images[1];
                                }
                                else
                                {
                                    // Reset Icon
                                    OutputIODataGridView.Rows[item].Cells["ColumnOutputIOOnOff"].Value = false;
                                    OutputIODataGridView.Rows[item].Cells["ColumnOutputIOState"].Value = (object)OutputIOImageList.Images[0];
                                }
                            }

                            Thread.Sleep(10);
                        }

                        // set the byte to previous
                        m_OutputScanInfoList[i].BytePrev = ioByte;
                    }

                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("OutputIOTimer: " + ex.ToString());
            }
        }

        private void InputIOTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < m_lastChannelInput; i++)
                {
                    //assign local variable
                    //get card no
                    int cardNo = m_InputScanInfoList[i].CardNo;
                    // get the channel
                    int channel = m_InputScanInfoList[i].Channel - (cardNo * IO.ref_NUM_CHANNEL);
                    // previous Byte of the channel
                    byte channelPrev = m_InputScanInfoList[i].BytePrev;
                    // obtain port byte from Share Memory
                    byte ioByte = 0;


                    if ((cardNo < IO.InputByte.Length) && (channel < IO.InputByte[cardNo].Length))
                        ioByte = (byte)IO.InputByte[cardNo][channel];
                    else
                        ioByte = Convert.ToByte("123");

                    // no changes, return
                    if (ioByte == channelPrev)
                        continue;

                    // get bits in the view that are different 
                    int iexclOR = channelPrev ^ ioByte;
                    byte exclOR = (byte)iexclOR;

                    if (exclOR != 0)
                    {
                        for (int j = 0; j <= 7; j++)
                        {
                            // if bit different 
                            if ((exclOR & (1 << j)) != 0)
                            {
                                int item = m_InputScanInfoList[i].ItemNo[j];

                                if (item == 99)
                                    continue;

                                if ((ioByte & (1 << j)) != 0)
                                {
                                    // Set Icon
                                    InputIODataGridView.Rows[item].Cells["ColumnInputIOOnOff"].Value = true;
                                    InputIODataGridView.Rows[item].Cells["ColumnInputIOState"].Value = (object)InputIOImageList.Images[1];
                                }
                                else
                                {
                                    // Reset Icon
                                    InputIODataGridView.Rows[item].Cells["ColumnInputIOOnOff"].Value = false;
                                    InputIODataGridView.Rows[item].Cells["ColumnInputIOState"].Value = (object)InputIOImageList.Images[0];
                                }
                            }

                            Thread.Sleep(10);
                        }

                        // set the byte to previous
                        m_InputScanInfoList[i].BytePrev = ioByte;
                    }

                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("InputIOTimer: " + ex.ToString());
            }
        }

       

       
    }
}