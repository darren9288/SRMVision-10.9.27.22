using System;
using System.Collections;
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
    public partial class IOMapForm : Form
    {
        #region Constant Variables

        private const int NUM_CHANNEL = 2;           // 1 card has 2 channel

        #endregion

        #region Member Variables

        private bool m_initialize = false;
        private bool m_outputChannel = false;
        private bool m_userSelect = true;
        private int m_group = 0;
        private int m_lastChannel = 0;
        private int m_rowSelected = 0;
        private string m_module = "";
        private ArrayList m_arrModules = new ArrayList();
        private TreeNode m_prevSelectedNode;
        private Color m_prevBackColor;
        private Color m_prevForeColor;
        private DataSet m_ioMapDataSet = new DataSet();
        private DataSet m_ioDataSet = new DataSet();
        private IOScanInfo[] m_ioScanInfoList = new IOScanInfo[64];    // contain bit information to be scanned
        private byte[] m_inputByteList = new byte[32];   //To store Input bits
        private byte[] m_outputByteList = new byte[32];   //To store Output bits

        private CustomOption m_smCustomizeInfo;

        #endregion


        public IOMapForm(CustomOption smCustomizeInfo)
        {
            m_smCustomizeInfo = smCustomizeInfo;

            InitializeComponent();

            //create io table in dataset
            GetIODataSet();
            FillTree();
        }



        /// <summary>
        /// Fill in node in io tree view
        /// </summary>
        private void FillTree()
        {
            IOTreeView.Nodes.Clear();

            try
            {
                //get database connection
                DBCall dbCall = new DBCall("\\access\\simeca.mdb");
                //create dataset 
                DataSet moduleSet = new DataSet();
                //select from iomap file by module and create table "module" in moduleSet 
                dbCall.Select("SELECT DISTINCT [Module] FROM IOMap", moduleSet, "Module");

                FilterModuleSet();

                m_ioMapDataSet = new DataSet();
                //select all from iomap file and create table "IOMap" in m_ioMapDataSet
                dbCall.Select("SELECT * FROM IOMap", m_ioMapDataSet, "IOMap");

                //base on number of module inside moduleSet table "module" 
                for (int i = 0; i < moduleSet.Tables["Module"].Rows.Count; i++)
                {
                    string strModuleName = moduleSet.Tables["Module"].Rows[i]["Module"].ToString();

                    if (!m_arrModules.Contains(strModuleName))
                    {
                        continue;
                    }

                    string filter = "Module = '" + strModuleName + "'";
                    string sort = "Name";
                    string nodeText = "";
                    string subNodeTextPrev = "";

                    //1st level node is module name
                    nodeText = moduleSet.Tables["Module"].Rows[i]["Module"].ToString();
                    TreeNode newNodeModule = new TreeNode(nodeText);
                    IOTreeView.Nodes.Add(newNodeModule);
                    //select image
                    newNodeModule.ImageIndex = 8;
                    newNodeModule.SelectedImageIndex = 8;

                    //get datarow list base on module name and io type
                    DataRow[] inputList = m_ioMapDataSet.Tables["IOMap"].Select(filter + " AND Type = 'Input'", sort);
                    DataRow[] outputList = m_ioMapDataSet.Tables["IOMap"].Select(filter + " AND Type = 'Output'", sort);

                    //2nd level node is io type name
                    nodeText = "Input";
                    TreeNode newNodeInput = new TreeNode(nodeText);
                    newNodeInput.ImageIndex = 0;
                    newNodeInput.SelectedImageIndex = 0;

                    nodeText = "Output";
                    TreeNode newNodeOutput = new TreeNode(nodeText);
                    newNodeOutput.ImageIndex = 1;
                    newNodeOutput.SelectedImageIndex = 1;
                
                    //run thru whole input list and insert node one by one
                    foreach (DataRow input in inputList)
                    {
                        string subNodeText = input["Name"].ToString();

                        if (subNodeText != "" && subNodeTextPrev != subNodeText)
                        {
                            TreeNode newNode = new TreeNode(subNodeText);
                            //io that not map yet show in red string
                            if (input["IO Number"].ToString() == "" || Convert.ToInt32(input["IO Number"]) == -1)
                                newNode.ForeColor = Color.Red;

                            newNodeInput.Nodes.Add(newNode);
                            //select image
                            newNode.ImageIndex = 2;
                            newNode.SelectedImageIndex = 2;
                        }

                        //avoid duplication 
                        subNodeTextPrev = subNodeText;
                    }

                    subNodeTextPrev = "";
                    //run thru whole output list and insert one by one
                    foreach (DataRow output in outputList)
                    {
                        string subNodeText = output["Name"].ToString();

                        if (subNodeText != "" && subNodeTextPrev != subNodeText)
                        {
                            TreeNode newNode = new TreeNode(subNodeText);
                            if (output["IO Number"].ToString() == "" || Convert.ToInt32(output["IO Number"]) == -1)
                                newNode.ForeColor = Color.Red;

                            newNodeOutput.Nodes.Add(newNode);
                            newNode.ImageIndex = 3;
                            newNode.SelectedImageIndex = 3;
                        }

                        subNodeTextPrev = subNodeText;
                    }

                     if (newNodeInput.GetNodeCount(false) > 0)
                        newNodeModule.Nodes.Add(newNodeInput);
                    if (newNodeOutput.GetNodeCount(false) > 0)
                        newNodeModule.Nodes.Add(newNodeOutput);
                }               
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "IO Trigger",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            m_userSelect = false;
            IOTreeView.ExpandAll();
            IOTreeView.Select();
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
                    subKey1 = subKey.OpenSubKey("Vision"+ (i+1));
                    if (subKey1 != null)
                        m_arrModules.Add(subKey1.GetValue("VisionName", "Vision " + (i + 1)));
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

        /// <summary>
        /// Updating grid view after click on io tree view node
        /// </summary>
        /// <param name="parent">io type</param>
        /// <param name="name">io name/description</param>
        private void UpdateGridView(string parent, string name)
        {
            m_module = parent;
            IOGridView.Rows.Clear();

            string filter = "Type = '" + parent + "'";
            bool itemAdded = true;
            int cardno = 0;
            int channel = 0;
            int bit = 0;
            int row = 0;
            int currentChannel = 0;
            m_lastChannel = 0;

            //get datarow list from table "IO" in m_ioDataSet,filter by type sort by number
            DataRow[] ioList = m_ioDataSet.Tables["IO"].Select(filter, "Number");

            //run thru whole ioList datarow,insert cell into grid view one by one
            foreach (DataRow io in ioList)
            {
                //check authority level to display on grid view
                if (m_outputChannel && m_group > Convert.ToInt32(io["Group"]))
                    itemAdded = false;

                //granted authority
                if (itemAdded)
                {
                    if (!m_arrModules.Contains(io["Module"]) && io["Module"].ToString() != "")
                    {
                        UnMapIO(io["Number"].ToString());
                    }

                    //true if type is output
                    m_outputChannel = (parent == "Output");

                    //create grid view row
                    DataGridViewRow newRow = new DataGridViewRow();
                    Image stateImage;
                    bool isMap = false;
                    //no module and description 
                    if ((io["Description"].ToString().Trim() == "" && io["Module"].ToString().Trim() == ""))
                    {
                        //set blank image to display
                        stateImage = IOImageListTree.Images[6];
                        //no mapping been done on that io
                        isMap = false;
                    }
                    else
                    {
                        //set check mark show this io already been map 
                        stateImage = IOImageListTree.Images[4];
                        isMap = true;
                    }

                    Image IOStateImage;
                    if (m_outputChannel)
                        IOStateImage = OutputImageListTree.Images[0];
                    else
                        IOStateImage = InputImageListTree.Images[0];

                    newRow.Height = 32;
                    //create cell for grid view row
                    newRow.CreateCells(IOGridView, new object[] { isMap, stateImage, false, IOStateImage, io["Number"].ToString(), io["Description"].ToString(),
                        io["Card Number"].ToString(), io["Channel Number"].ToString(), io["Bit"].ToString(),io["Pin Number"].ToString(),io["Module"].ToString() });
                    //add new grid view row
                    IOGridView.Rows.Add(newRow);

                    //get value for cardno,channel,bit
                    cardno = Convert.ToInt32(io["Card Number"]);
                    channel = Convert.ToInt32(io["Channel Number"]);
                    bit = Convert.ToInt32(io["Bit"]);

                    int i;
                    bool newChannel = false;
                    // if both condition is 0 means no channel been addin to ioScanInfo list
                    // add new channel to the array ioScanInfo list
                    if (m_lastChannel == 0 && currentChannel == 0)
                        newChannel = true;
                    else
                    {
                        // there are channels in the list
                        // compare current channel from list and current want to add in channel
                        // no same means need to be add in
                        if (m_ioScanInfoList[currentChannel].Channel != channel)
                        {
                            //add up to be less than next channel to add
                            // ex. lastChannel=1, means i only run till 0,which is current index of array list
                            for (i = 0; i < m_lastChannel; i++)
                            {
                                // channel already exists
                                if (m_ioScanInfoList[i].Channel == channel)
                                {
                                    currentChannel = i;
                                    break;
                                }
                            }

                            // the channel is not in the array list, add a new channel
                            if (i == m_lastChannel)
                                newChannel = true;
                        }
                    }

                    // add a channel to ioScanInfo list
                    if (newChannel)
                    {
                        //only add in a channel at begin of 1st bit
                        m_ioScanInfoList[m_lastChannel] = new IOScanInfo(0);
                        m_ioScanInfoList[m_lastChannel].CardNo = cardno;
                        m_ioScanInfoList[m_lastChannel].Channel = channel;

                        //set current channel
                        currentChannel = m_lastChannel;
                        //increment next channel to add
                        m_lastChannel++;

                        //in one channel there are 8 bit to reset, items 0-7
                        for (int a = 0; a < 8; a++)
                            m_ioScanInfoList[currentChannel].ItemNo[a] = 99;
                    }

                    // set the result/status as itembit value base on position of itembit array 
                    m_ioScanInfoList[currentChannel].ItemBit |= (byte)(1 << bit);
                    // add the item number to the list same as row index
                    m_ioScanInfoList[currentChannel].ItemNo[bit] = row;
                    // item added, set previous byte to 0
                    // so it can be different from current and trigger updating process in timer tick
                    m_ioScanInfoList[currentChannel].BytePrev = 0;

                    //next row
                    row++;
                }

                itemAdded = true;
            }

            //select on last row
            if (IOGridView.Rows.Count != 0 && IOGridView.Rows.Count > m_rowSelected)
                IOGridView.Rows[m_rowSelected].Selected = true;
        }

        /// <summary>
        /// Map io to simeca.mdb file,update IO and IOMap table
        /// </summary>
        /// <param name="ioNumber">io number to map on this io</param>
        private void MapIO(string ioNumber)
        {
            DBCall dbCall = new DBCall("//access//simeca.mdb");
            //set update string, fill in io description,module where number is match, update IO table
            string sqlUpdate = "UPDATE IO SET [Description] = '" + IOTreeView.SelectedNode.Text + "', [Module] = '" +
                IOTreeView.SelectedNode.Parent.Parent.Text + "' WHERE [Number] = " + ioNumber;
            dbCall.Update(sqlUpdate);

            //set update string, fill in io number where name is match,update IOMap table
            sqlUpdate = "UPDATE IOMap SET [IO Number] = " + ioNumber + " WHERE [Name] = '" + IOTreeView.SelectedNode.Text + "'";
            dbCall.Update(sqlUpdate);
        }

        /// <summary>
        /// Check ioName map status
        /// </summary>
        /// <param name="ioName">io name</param>
        /// <returns>number that map to this io name</returns>
        private int IsMapped(string ioName)
        {
            DataRow[] ioList = m_ioDataSet.Tables["IO"].Select("Description = '" + ioName + "'");
            if (ioList.Length > 0)
                return Convert.ToInt32(ioList[0]["Number"]);
            else
                return -1;
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
            sqlUpdate = "UPDATE IOMap SET [IO Number] = -1 WHERE [IO Number] = " + ioNumber ;
            dbCall.Update(sqlUpdate);
        }

        private void SetIOGridViewRowSelected(int rowNo)
        {
            for (int i = 0; i < IOGridView.Rows.Count; i++)
                IOGridView.Rows[i].Selected = (i == rowNo);
        }




        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            this.Close();
        }

        /// <summary>
        /// Call after click on io tree view node
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IOTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //to check if level more than one then can proceed
            if (e.Node.Level > 1)
            {
                //node name,io name/description
                string name = IOTreeView.SelectedNode.Text;
                //flag that indicate to other process the grid view is performing updating
                m_initialize = true;

                //if the node name is not module name then can proceed,level is 3
                //parent.text is module name, name is io description
                if ((name != "Input") && (name != "Output"))
                    UpdateGridView(IOTreeView.SelectedNode.Parent.Text, name);
                else
                    UpdateGridView("", "");

                //release flag to other process,grid view updating finish
                m_initialize = false;

                m_prevBackColor = IOTreeView.SelectedNode.BackColor;
                m_prevForeColor = IOTreeView.SelectedNode.ForeColor;
                IOTreeView.SelectedNode.BackColor = SystemColors.Highlight;
                IOTreeView.SelectedNode.ForeColor = SystemColors.HighlightText;
                m_prevSelectedNode = IOTreeView.SelectedNode;
            }
        }

        private void IOTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (!m_userSelect)
            {
                e.Cancel = true;
                m_userSelect = true;
            }

            if (IOTreeView.SelectedNode != null && IOTreeView.SelectedNode.Level > 1)
            {
                IOTreeView.SelectedNode.BackColor = m_prevBackColor;
                IOTreeView.SelectedNode.ForeColor = m_prevForeColor;
            }
        }

        /// <summary>
        /// Call once mouse click on any node on io tree view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IOTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (m_prevSelectedNode != null && m_prevSelectedNode.Level > 1)
            {
                m_prevSelectedNode.BackColor = m_prevBackColor;
                m_prevSelectedNode.ForeColor = m_prevForeColor;
            }

            if (e.Node.IsExpanded)
                e.Node.Collapse();
            else
                e.Node.Expand();
        }

        /// <summary>
        /// Call once click on any cell of grid view,indicate that going to map on that io
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IOGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //m_initialize= true means that io grid view in updating mode, row index is valid
            if (m_initialize || e.RowIndex < 0)
                return;

            //must click on column 1 then be effective,io description area column
            if (e.ColumnIndex == 1)
            {
                //io tree view selected node no null
                if (IOTreeView.SelectedNode == null)
                    return;

                //more than level 2,means selected on io name level
                if (IOTreeView.SelectedNode.Level < 2)
                    return;

                //get io name from io tree view node
                string ioName = IOTreeView.SelectedNode.Text;
                //get row index from io grid view
                m_rowSelected = e.RowIndex;


                // to un-map with select IO, it is mapped now want to unmap it
                if ((bool)IOGridView.Rows[e.RowIndex].Cells[0].Value)
                {
                    //check to ensure select node with io name match with grid view io description
                    if (IOGridView.Rows[e.RowIndex].Cells[5].Value.ToString() != ioName)
                    {
                        SRMMessageBox.Show("Selected Node : " + ioName + "\n"
                            + "Not Match With IO Name In Grid View : "
                            + IOGridView.Rows[e.RowIndex].Cells[5].Value.ToString());
                        return;
                    }

                    //uncheck,set blank display,unmap from simeca.mdb file
                    IOGridView.Rows[e.RowIndex].Cells[0].Value = false;
                    IOGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (object)IOImageListTree.Images[6];
                    UnMapIO(IOGridView.Rows[e.RowIndex].Cells[4].Value.ToString());
                }
                // to map with select IO
                else
                {
                    //if return -1,means the ioName not map yet else return number that map to this ioName
                    int ioNumber = IsMapped(ioName);

                    //ioName been mapped
                    if (ioNumber >= 0)
                    {
                        SRMMessageBox.Show("The selected IO - \"" + ioName + "\" already mapped to IO - " + ioNumber + ".", "IO Mapping",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    //check,set display image
                    IOGridView.Rows[e.RowIndex].Cells[0].Value = true;
                    IOGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (object)IOImageListTree.Images[4];
                    //get io number to map from grid view
                    MapIO(IOGridView.Rows[e.RowIndex].Cells[4].Value.ToString());
                    m_prevForeColor = SystemColors.WindowText;
                }

                //refresh io dataset after update latest io map info 
                GetIODataSet();

                //refresh grid view
                UpdateGridView(IOTreeView.SelectedNode.Parent.Text, ioName);
                //set selected row
                SetIOGridViewRowSelected(e.RowIndex);

                //refresh io tree
                FillTree();

                //enable user to be able select io tree
                m_userSelect = true;
            }
            else if (e.ColumnIndex == 3)   // 3rd column will ON/Off the output channel
            {
                //only be effective if it is at output view
                if (!m_outputChannel && m_userSelect)
                    return;

                if (m_outputChannel && !m_initialize)
                {
                    int cardNo = Convert.ToInt32(IOGridView.Rows[e.RowIndex].Cells[6].Value);
                    int channel = Convert.ToInt32(IOGridView.Rows[e.RowIndex].Cells[7].Value);
                    int bit = Convert.ToInt32(IOGridView.Rows[e.RowIndex].Cells[8].Value);
                    int bitStatus = 0;

                    // to trigger off an output bit 
                    if ((bool)IOGridView.Rows[e.RowIndex].Cells[2].Value)
                    {
                        bitStatus = 0;
                        IOGridView.Rows[e.RowIndex].Cells[2].Value = false;
                        IOGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (object)OutputImageListTree.Images[0];
                    }
                    //to trigger on an output bit
                    else
                    {
                        bitStatus = 1;
                        IOGridView.Rows[e.RowIndex].Cells[2].Value = true;
                        IOGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (object)OutputImageListTree.Images[1];
                    }

                    //update to output byte 
                    IO.OutPort(cardNo, channel, bit, bitStatus);

                }
            }
        }

        private void IOTimer_Tick(object sender, EventArgs e)
        {
            //run till last channel to scan
            for (int i = 0; i < m_lastChannel; i++)
            {
                //assign local variable
                //get card no
                int cardNo = m_ioScanInfoList[i].CardNo;
                // get the channel
                int channel = m_ioScanInfoList[i].Channel - (cardNo * NUM_CHANNEL);
                // previous Byte of the channel
                byte channelPrev = m_ioScanInfoList[i].BytePrev;
                // obtain port byte from Share Memory

                byte ioByte = 0;

                //if user is select output to view in ioGridView
                if (m_outputChannel)
                {
                    if ((cardNo < IO.OutputByte.Length) && (channel < IO.OutputByte[cardNo].Length))
                        ioByte = IO.OutputByte[cardNo][channel];
                    else
                        ioByte = Convert.ToByte("123");
                }
                else
                {
                    if ((cardNo < IO.InputByte.Length) && (channel < IO.InputByte[cardNo].Length))
                        ioByte = (byte)IO.InputByte[cardNo][channel];
                    else
                        ioByte = Convert.ToByte("123");
                }


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
                            int item = m_ioScanInfoList[i].ItemNo[j];

                            if (item == 99)
                                continue;

                            if ((ioByte & (1 << j)) != 0)
                            {
                                // Set Icon	display
                                if (!m_outputChannel)
                                {
                                    //for input
                                    m_userSelect = false;
                                    IOGridView.Rows[item].Cells[2].Value = (object)true;
                                    IOGridView.Rows[item].Cells[3].Value = (object)InputImageListTree.Images[1];
                                    m_userSelect = true;
                                }
                                else
                                {
                                    //for output
                                    IOGridView.Rows[item].Cells[2].Value = (object)true;
                                    IOGridView.Rows[item].Cells[3].Value = (object)OutputImageListTree.Images[1];
                                }
                            }
                            else
                            {
                                // Reset Icon 
                                if (!m_outputChannel)
                                {
                                    //for input
                                    m_userSelect = false;
                                    IOGridView.Rows[item].Cells[2].Value = (object)false;
                                    IOGridView.Rows[item].Cells[3].Value = (object)InputImageListTree.Images[0];
                                    m_userSelect = true;
                                }
                                else
                                {
                                    //for output
                                    IOGridView.Rows[item].Cells[2].Value = (object)false;
                                    IOGridView.Rows[item].Cells[3].Value = (object)OutputImageListTree.Images[0];
                                }
                            }
                        }

                        Thread.Sleep(10);
                    }

                    // set the byte to previous, for future compare status change
                    m_ioScanInfoList[i].BytePrev = ioByte;
                }

                Thread.Sleep(10);
            }
        }
    }
}