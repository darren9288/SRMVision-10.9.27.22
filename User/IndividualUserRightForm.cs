using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using Common;
using SharedMemory;
using Microsoft.Win32;
namespace User
{
    public partial class IndividualUserRightForm : Form
    {
        #region Members Variables
        private string m_strSelectedRadio;
        private bool m_blnInitDone = false;
        private int m_intSelectedGroup;
        private bool m_blnUserSelect = true;
        private int m_intGroup;
        private int[] m_intParentArrayGroup;
        private int[][] m_intFirstChildArrayGroup;
        private int[][][] m_intSecondChildArrayGroup;
        private int[][][][] m_intThirdChildArrayGroup;
        private DataSet m_dsFirstChild_Orientation;
        private DataSet m_dsFirstChild_MarkOrient;
        private DataSet m_dsFirstChild_Pad;
        private DataSet m_dsFirstChild_Lead3D;
        private DataSet m_dsFirstChild_InPocket;
        private DataSet m_dsFirstChild_InPocket2;
        private DataSet m_dsFirstChild_Seal;
        private DataSet m_dsFirstChild_Seal2;
        private DataSet m_dsFirstChild_Barcode;
        private DataSet m_dsFirstChild_TopMenu;
        private DataSet m_dsFirstChild_BottomMenu;
        private DataSet m_dsSecondChild_Orientation;
        private DataSet m_dsSecondChild_MarkOrient;
        private DataSet m_dsSecondChild_Pad;
        private DataSet m_dsSecondChild_Lead3D;
        private DataSet m_dsSecondChild_InPocket;
        private DataSet m_dsSecondChild_InPocket2;
        private DataSet m_dsSecondChild_Seal;
        private DataSet m_dsSecondChild_Seal2;
        private DataSet m_dsSecondChild_Barcode;
        private DataSet m_dsSecondChild_TopMenu;
        private DataSet m_dsSecondChild_BottomMenu;
        private DataSet m_dsThirdChild_Orientation;
        private DataSet m_dsThirdChild_MarkOrient;
        private DataSet m_dsThirdChild_Pad;
        private DataSet m_dsThirdChild_Lead3D;
        private DataSet m_dsThirdChild_InPocket;
        private DataSet m_dsThirdChild_InPocket2;
        private DataSet m_dsThirdChild_Seal;
        private DataSet m_dsThirdChild_Seal2;
        private DataSet m_dsThirdChild_Barcode;
        private DataSet m_dsIO;
        private DataSet m_dsParent;
        private DataSet m_dsSetting;
        private OleDbDataAdapter m_FirstChildDataAdapter_Orientation;
        private OleDbDataAdapter m_FirstChildDataAdapter_MarkOrient;
        private OleDbDataAdapter m_FirstChildDataAdapter_Pad;
        private OleDbDataAdapter m_FirstChildDataAdapter_Lead3D;
        private OleDbDataAdapter m_FirstChildDataAdapter_InPocket;
        private OleDbDataAdapter m_FirstChildDataAdapter_InPocket2;
        private OleDbDataAdapter m_FirstChildDataAdapter_Seal;
        private OleDbDataAdapter m_FirstChildDataAdapter_Seal2;
        private OleDbDataAdapter m_FirstChildDataAdapter_Barcode;
        private OleDbDataAdapter m_FirstChildDataAdapter_TopMenu;
        private OleDbDataAdapter m_FirstChildDataAdapter_BottomMenu;
        private OleDbDataAdapter m_SecondChildDataAdapter_Orientation;
        private OleDbDataAdapter m_SecondChildDataAdapter_MarkOrient;
        private OleDbDataAdapter m_SecondChildDataAdapter_Pad;
        private OleDbDataAdapter m_SecondChildDataAdapter_Lead3D;
        private OleDbDataAdapter m_SecondChildDataAdapter_InPocket;
        private OleDbDataAdapter m_SecondChildDataAdapter_InPocket2;
        private OleDbDataAdapter m_SecondChildDataAdapter_Seal;
        private OleDbDataAdapter m_SecondChildDataAdapter_Seal2;
        private OleDbDataAdapter m_SecondChildDataAdapter_Barcode;
        private OleDbDataAdapter m_SecondChildDataAdapter_TopMenu;
        private OleDbDataAdapter m_SecondChildDataAdapter_BottomMenu;
        private OleDbDataAdapter m_ThirdChildDataAdapter_Orientation;
        private OleDbDataAdapter m_ThirdChildDataAdapter_MarkOrient;
        private OleDbDataAdapter m_ThirdChildDataAdapter_Pad;
        private OleDbDataAdapter m_ThirdChildDataAdapter_Lead3D;
        private OleDbDataAdapter m_ThirdChildDataAdapter_InPocket;
        private OleDbDataAdapter m_ThirdChildDataAdapter_InPocket2;
        private OleDbDataAdapter m_ThirdChildDataAdapter_Seal;
        private OleDbDataAdapter m_ThirdChildDataAdapter_Seal2;
        private OleDbDataAdapter m_ThirdChildDataAdapter_Barcode;
        private OleDbDataAdapter m_IODataAdapter;
        private OleDbDataAdapter m_ParentDataAdapter;
        private OleDbParameter m_workParam;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomOption;
        #endregion

        public IndividualUserRightForm(int group, ProductionInfo smProductionInfo, CustomOption smCustomOption)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_smCustomOption = smCustomOption;
            m_intSelectedGroup = m_intGroup = group;
            GetUserInfo();
            // Fill out the list view

            if (m_smCustomOption.g_intLanguageCulture == 2)
            {
                GetRightDataSet_CH();
                InitParentTree_CH();
            }
            else
            {
                GetRightDataSet();
                InitParentTree();
            }
            
            m_blnUserSelect = false;
            tre_UserRight.CollapseAll();

            UpdateRightTree();

            if (m_intGroup > 1)
            {
                radio_SRM.Visible = false;
                lbl_SRM.Visible = false;
            }

            // 2020-05-05 ZJYEOH : Disable when init because no node selected
            radio_SRM.Enabled = false;
            radio_Admin.Enabled = false;
            radio_Engineer.Enabled = false;
            radio_Technician.Enabled = false;
            radio_Operator.Enabled = false;

            m_blnInitDone = true;
            timer1.Start();
        }

        private void ChangeGroup_CH(int group)
        {
            if (tre_UserRight.SelectedNode == null)
                return;

            int selectedNodeIndex = 0, GrandGrandParentNodeIndex = 0, GrandParentNodeIndex = 0, ParentNodeIndex = 0;
            string selectedNodeText = "", GrandGrandParentNodeText = "", GrandParentNodeText = "", ParentNodeText = "";
            string filter = "";
            TreeNode selectedNode, GrandGrandParentNode, GrandParentNode, ParentNode;
            DataRow[] DataList;
            DataRow Data;

            selectedNode = tre_UserRight.SelectedNode;
            selectedNodeIndex = selectedNode.Index;
            selectedNodeText = selectedNode.Text;
            ParentNode = selectedNode.Parent;

            if (ParentNode != null)
            {
                ParentNodeText = ParentNode.Text;
                ParentNodeIndex = ParentNode.Index;
                GrandParentNode = ParentNode.Parent;

                if (GrandParentNode != null)
                {
                    GrandParentNodeText = GrandParentNode.Text;
                    GrandParentNodeIndex = GrandParentNode.Index;
                    GrandGrandParentNode = GrandParentNode.Parent;

                    if (GrandGrandParentNode != null)
                    {
                        GrandGrandParentNodeText = GrandGrandParentNode.Text;
                        GrandGrandParentNodeIndex = GrandGrandParentNode.Index;

                        filter = "[Chi Name] = '" + selectedNodeText + "' AND [Chi Child2] = '" + ParentNodeText + "' AND [Chi Child1] = '" + GrandParentNodeText + "'";
                        DataList = GetChildDataRow(GrandGrandParentNodeText, filter, 3);

                        if (DataList.Length > 0)
                        {
                            Data = DataList[0];
                            Data["Chi Name"] = selectedNodeText;
                            Data["Chi Child2"] = ParentNodeText;
                            Data["Group"] = group;
                            Data["Chi Child1"] = GrandParentNodeText;
                            try
                            {
                                UpdateTable(GrandGrandParentNodeText, 3);


                                STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio, GetUserGroupName(group), m_smProductionInfo.g_strLotID);

                                m_strSelectedRadio = GetUserGroupName(group);
                            }
                            catch (Exception ex)
                            {
                                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        m_intThirdChildArrayGroup[GrandGrandParentNodeIndex][GrandParentNodeIndex][ParentNodeIndex][selectedNodeIndex] = group;
                    }
                    else
                    {
                        //if (ParentNodeText == "I/O Trigger")
                        //{
                        //    filter = "Description = '" + selectedNodeText + "' AND Module = '" + ParentNodeText + "' AND Type = 'Output'";
                        //    DataList = m_dsIO.Tables["IO"].Select(filter);
                        //    Data = DataList[0];
                        //    Data["Description"] = selectedNodeText;
                        //    Data["Module"] = ParentNodeText;
                        //    Data["Group"] = group;

                        //    try
                        //    {
                        //        m_IODataAdapter.Update(m_dsIO, "IO");
                        //        //

                        //        //STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio.ToString(), group.ToString(), m_smProductionInfo.g_strLotID);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    }

                        //    m_intSecondChildArrayGroup[GrandParentNodeIndex][ParentNodeIndex][selectedNodeIndex] = group;
                        //}
                        //else
                        {
                            filter = "[Chi Name] = '" + selectedNodeText + "' AND [Chi Child1] = '" + ParentNodeText + "'";
                            DataList = GetChildDataRow(GrandParentNodeText, filter, 2);
                            Data = DataList[0];
                            Data["Chi Name"] = selectedNodeText;
                            Data["Chi Child1"] = ParentNodeText;
                            Data["Group"] = group;

                            try
                            {
                                UpdateTable(GrandParentNodeText, 2);
                                

                                STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio, GetUserGroupName(group), m_smProductionInfo.g_strLotID);
                                
                                m_strSelectedRadio = GetUserGroupName(group);
                            }
                            catch (Exception ex)
                            {
                                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            m_intSecondChildArrayGroup[GrandParentNodeIndex][ParentNodeIndex][selectedNodeIndex] = group;

                        }
                    }
                }
                else
                {
                    filter = "[Chi Name] = '" + selectedNodeText + "' AND [Chi Parent] = '" + ParentNodeText + "'";
                    DataList = GetChildDataRow(ParentNodeText, filter, 1);
                    Data = DataList[0];
                    Data["Chi Name"] = selectedNodeText;
                    Data["Chi Parent"] = ParentNodeText;
                    Data["Group"] = group;

                    try
                    {
                        UpdateTable(ParentNodeText, 1);
                        

                        STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio, GetUserGroupName(group), m_smProductionInfo.g_strLotID);
                        
                        m_strSelectedRadio = GetUserGroupName(group);
                    }
                    catch (Exception ex)
                    {
                        SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    m_intFirstChildArrayGroup[ParentNodeIndex][selectedNodeIndex] = group;

                }
            }
            else
            {
                filter = "[Chi Name] = '" + selectedNodeText + "'";
                DataList = m_dsParent.Tables["Parent"].Select(filter);
                Data = DataList[0];
                Data["Chi Name"] = selectedNodeText;
                Data["Group"] = group;
                try
                {
                    m_ParentDataAdapter.Update(m_dsParent, "Parent");
                    

                    STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio, GetUserGroupName(group), m_smProductionInfo.g_strLotID);
                    
                    m_strSelectedRadio = GetUserGroupName(group);
                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                m_intParentArrayGroup[selectedNodeIndex] = group;

            }
        }
        private void ChangeGroup(int group)
        {
            if (tre_UserRight.SelectedNode == null)
                return;

            int selectedNodeIndex = 0, GrandGrandParentNodeIndex = 0, GrandParentNodeIndex = 0, ParentNodeIndex = 0;
            string selectedNodeText = "", GrandGrandParentNodeText = "", GrandParentNodeText = "", ParentNodeText = "";
            string filter = "";
            TreeNode selectedNode, GrandGrandParentNode, GrandParentNode, ParentNode;
            DataRow[] DataList;
            DataRow Data;

            selectedNode = tre_UserRight.SelectedNode;
            selectedNodeIndex = selectedNode.Index;
            selectedNodeText = selectedNode.Text;
            ParentNode = selectedNode.Parent;

            if (ParentNode != null)
            {
                ParentNodeText = ParentNode.Text;
                ParentNodeIndex = ParentNode.Index;
                GrandParentNode = ParentNode.Parent;

                if (GrandParentNode != null)
                {
                    GrandParentNodeText = GrandParentNode.Text;
                    GrandParentNodeIndex = GrandParentNode.Index;
                    GrandGrandParentNode = GrandParentNode.Parent;

                    if (GrandGrandParentNode != null)
                    {
                        GrandGrandParentNodeText = GrandGrandParentNode.Text;
                        GrandGrandParentNodeIndex = GrandGrandParentNode.Index;

                        filter = "Name = '" + selectedNodeText + "' AND [Child2] = '" + ParentNodeText + "' AND [Child1] = '" + GrandParentNodeText + "'";
                        DataList = GetChildDataRow(GrandGrandParentNodeText, filter, 3);
                        Data = DataList[0];
                        Data["Name"] = selectedNodeText;
                        Data["Child2"] = ParentNodeText;
                        Data["Group"] = group;
                        Data["Child1"] = GrandParentNodeText;
                        try
                        {
                            UpdateTable(GrandGrandParentNodeText, 3);
                            

                            STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio, GetUserGroupName(group), m_smProductionInfo.g_strLotID);
                            
                            m_strSelectedRadio = GetUserGroupName(group);
                        }
                        catch (Exception ex)
                        {
                            SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        m_intThirdChildArrayGroup[GrandGrandParentNodeIndex][GrandParentNodeIndex][ParentNodeIndex][selectedNodeIndex] = group;
                    }
                    else
                    {
                        //if (ParentNodeText == "I/O Trigger")
                        //{
                        //    filter = "Description = '" + selectedNodeText + "' AND Module = '" + ParentNodeText + "' AND Type = 'Output'";
                        //    DataList = m_dsIO.Tables["IO"].Select(filter);
                        //    Data = DataList[0];
                        //    Data["Description"] = selectedNodeText;
                        //    Data["Module"] = ParentNodeText;
                        //    Data["Group"] = group;

                        //    try
                        //    {
                        //        m_IODataAdapter.Update(m_dsIO, "IO");
                        //        //

                        //        //STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio.ToString(), group.ToString(), m_smProductionInfo.g_strLotID);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    }

                        //    m_intSecondChildArrayGroup[GrandParentNodeIndex][ParentNodeIndex][selectedNodeIndex] = group;
                        //}
                        //else
                        {
                            filter = "Name = '" + selectedNodeText + "' AND [Child1] = '" + ParentNodeText + "'";
                            DataList = GetChildDataRow(GrandParentNodeText, filter, 2);
                            Data = DataList[0];
                            Data["Name"] = selectedNodeText;
                            Data["Child1"] = ParentNodeText;
                            Data["Group"] = group;

                            try
                            {
                                UpdateTable(GrandParentNodeText, 2);
                                

                                STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio, GetUserGroupName(group), m_smProductionInfo.g_strLotID);
                                
                                m_strSelectedRadio = GetUserGroupName(group);
                            }
                            catch (Exception ex)
                            {
                                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            m_intSecondChildArrayGroup[GrandParentNodeIndex][ParentNodeIndex][selectedNodeIndex] = group;

                        }
                    }
                }
                else
                {
                    filter = "Name = '" + selectedNodeText + "' AND [Parent] = '" + ParentNodeText + "'";
                    DataList = GetChildDataRow(ParentNodeText, filter, 1);
                    Data = DataList[0];
                    Data["Name"] = selectedNodeText;
                    Data["Parent"] = ParentNodeText;
                    Data["Group"] = group;

                    try
                    {
                        UpdateTable(ParentNodeText, 1);
                        

                        STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio, GetUserGroupName(group), m_smProductionInfo.g_strLotID);
                        
                        m_strSelectedRadio = GetUserGroupName(group);
                    }
                    catch (Exception ex)
                    {
                        SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    m_intFirstChildArrayGroup[ParentNodeIndex][selectedNodeIndex] = group;

                }
            }
            else
            {
                filter = "[Name] = '" + selectedNodeText + "'";
                DataList = m_dsParent.Tables["Parent"].Select(filter);
                Data = DataList[0];
                Data["Name"] = selectedNodeText;
                Data["Group"] = group;
                try
                {
                    m_ParentDataAdapter.Update(m_dsParent, "Parent");
                    

                    STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio, GetUserGroupName(group), m_smProductionInfo.g_strLotID);
                    
                    m_strSelectedRadio = GetUserGroupName(group);
                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                m_intParentArrayGroup[selectedNodeIndex] = group;

            }
        }
        private void GetRightDataSet_CH()
        {
            string sqlUpdate = "";
            OleDbCommand accessCommand;
            OleDbConnection accessConn;

            accessConn = new OleDbConnection();
            accessConn.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                @"data source = " + AppDomain.CurrentDomain.BaseDirectory + "access\\setting.mdb";
            accessConn.Open();

            m_dsParent = new DataSet();
            m_dsParent.Tables.Add("Parent");

            m_dsFirstChild_Orientation = new DataSet();
            m_dsFirstChild_Orientation.Tables.Add("Child1_Orientation");
            m_dsFirstChild_MarkOrient = new DataSet();
            m_dsFirstChild_MarkOrient.Tables.Add("Child1_MarkOrient");
            m_dsFirstChild_Pad = new DataSet();
            m_dsFirstChild_Pad.Tables.Add("Child1_Pad");
            m_dsFirstChild_Lead3D = new DataSet();
            m_dsFirstChild_Lead3D.Tables.Add("Child1_Lead3D");
            m_dsFirstChild_InPocket = new DataSet();
            m_dsFirstChild_InPocket.Tables.Add("Child1_InPocket");
            m_dsFirstChild_InPocket2 = new DataSet();
            m_dsFirstChild_InPocket2.Tables.Add("Child1_InPocket2");
            m_dsFirstChild_Seal = new DataSet();
            m_dsFirstChild_Seal.Tables.Add("Child1_Seal");
            m_dsFirstChild_Seal2 = new DataSet();
            m_dsFirstChild_Seal2.Tables.Add("Child1_Seal2");
            m_dsFirstChild_Barcode = new DataSet();
            m_dsFirstChild_Barcode.Tables.Add("Child1_Barcode");
            m_dsFirstChild_TopMenu = new DataSet();
            m_dsFirstChild_TopMenu.Tables.Add("Child1_TopMenu");
            m_dsFirstChild_BottomMenu = new DataSet();
            m_dsFirstChild_BottomMenu.Tables.Add("Child1_BottomMenu");

            m_dsSecondChild_Orientation = new DataSet();
            m_dsSecondChild_Orientation.Tables.Add("Child2_Orientation");
            m_dsSecondChild_MarkOrient = new DataSet();
            m_dsSecondChild_MarkOrient.Tables.Add("Child2_MarkOrient");
            m_dsSecondChild_Pad = new DataSet();
            m_dsSecondChild_Pad.Tables.Add("Child2_Pad");
            m_dsSecondChild_Lead3D = new DataSet();
            m_dsSecondChild_Lead3D.Tables.Add("Child2_Lead3D");
            m_dsSecondChild_InPocket = new DataSet();
            m_dsSecondChild_InPocket.Tables.Add("Child2_InPocket");
            m_dsSecondChild_InPocket2 = new DataSet();
            m_dsSecondChild_InPocket2.Tables.Add("Child2_InPocket2");
            m_dsSecondChild_Seal = new DataSet();
            m_dsSecondChild_Seal.Tables.Add("Child2_Seal");
            m_dsSecondChild_Seal2 = new DataSet();
            m_dsSecondChild_Seal2.Tables.Add("Child2_Seal2");
            m_dsSecondChild_Barcode = new DataSet();
            m_dsSecondChild_Barcode.Tables.Add("Child2_Barcode");
            m_dsSecondChild_TopMenu = new DataSet();
            m_dsSecondChild_TopMenu.Tables.Add("Child2_TopMenu");
            m_dsSecondChild_BottomMenu = new DataSet();
            m_dsSecondChild_BottomMenu.Tables.Add("Child2_BottomMenu");

            m_dsThirdChild_Orientation = new DataSet();
            m_dsThirdChild_Orientation.Tables.Add("Child3_Orientation");
            m_dsThirdChild_MarkOrient = new DataSet();
            m_dsThirdChild_MarkOrient.Tables.Add("Child3_MarkOrient");
            m_dsThirdChild_Pad = new DataSet();
            m_dsThirdChild_Pad.Tables.Add("Child3_Pad");
            m_dsThirdChild_Lead3D = new DataSet();
            m_dsThirdChild_Lead3D.Tables.Add("Child3_Lead3D");
            m_dsThirdChild_InPocket = new DataSet();
            m_dsThirdChild_InPocket.Tables.Add("Child3_InPocket");
            m_dsThirdChild_InPocket2 = new DataSet();
            m_dsThirdChild_InPocket2.Tables.Add("Child3_InPocket2");
            m_dsThirdChild_Seal = new DataSet();
            m_dsThirdChild_Seal.Tables.Add("Child3_Seal");
            m_dsThirdChild_Seal2 = new DataSet();
            m_dsThirdChild_Seal2.Tables.Add("Child3_Seal2");
            m_dsThirdChild_Barcode = new DataSet();
            m_dsThirdChild_Barcode.Tables.Add("Child3_Barcode");

            accessCommand = new OleDbCommand("SELECT * FROM Parent", accessConn);
            m_ParentDataAdapter = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child1_Orientation", accessConn);
            m_FirstChildDataAdapter_Orientation = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_MarkOrient", accessConn);
            m_FirstChildDataAdapter_MarkOrient = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_Pad", accessConn);
            m_FirstChildDataAdapter_Pad = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_Lead3D", accessConn);
            m_FirstChildDataAdapter_Lead3D = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_InPocket", accessConn);
            m_FirstChildDataAdapter_InPocket = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_InPocket2", accessConn);
            m_FirstChildDataAdapter_InPocket2 = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_Seal", accessConn);
            m_FirstChildDataAdapter_Seal = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_Seal2", accessConn);
            m_FirstChildDataAdapter_Seal2 = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_Barcode", accessConn);
            m_FirstChildDataAdapter_Barcode = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_TopMenu", accessConn);
            m_FirstChildDataAdapter_TopMenu = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_BottomMenu", accessConn);
            m_FirstChildDataAdapter_BottomMenu = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child2_Orientation", accessConn);
            m_SecondChildDataAdapter_Orientation = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_MarkOrient", accessConn);
            m_SecondChildDataAdapter_MarkOrient = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_Pad", accessConn);
            m_SecondChildDataAdapter_Pad = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_Lead3D", accessConn);
            m_SecondChildDataAdapter_Lead3D = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_InPocket", accessConn);
            m_SecondChildDataAdapter_InPocket = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_InPocket2", accessConn);
            m_SecondChildDataAdapter_InPocket2 = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_Seal", accessConn);
            m_SecondChildDataAdapter_Seal = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_Seal2", accessConn);
            m_SecondChildDataAdapter_Seal2 = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_Barcode", accessConn);
            m_SecondChildDataAdapter_Barcode = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_TopMenu", accessConn);
            m_SecondChildDataAdapter_TopMenu = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_BottomMenu", accessConn);
            m_SecondChildDataAdapter_BottomMenu = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child3_Orientation", accessConn);
            m_ThirdChildDataAdapter_Orientation = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_MarkOrient", accessConn);
            m_ThirdChildDataAdapter_MarkOrient = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_Pad", accessConn);
            m_ThirdChildDataAdapter_Pad = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_Lead3D", accessConn);
            m_ThirdChildDataAdapter_Lead3D = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_InPocket", accessConn);
            m_ThirdChildDataAdapter_InPocket = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_InPocket2", accessConn);
            m_ThirdChildDataAdapter_InPocket2 = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_Seal", accessConn);
            m_ThirdChildDataAdapter_Seal = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_Seal2", accessConn);
            m_ThirdChildDataAdapter_Seal2 = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_Barcode", accessConn);
            m_ThirdChildDataAdapter_Barcode = new OleDbDataAdapter(accessCommand);

            sqlUpdate = "UPDATE Parent SET [Group] = @Group WHERE [Chi Name] = @ChiName";
            m_ParentDataAdapter.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ParentDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ParentDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Orientation] SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Parent] = @ChiParent";
            m_FirstChildDataAdapter_Orientation.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_MarkOrient] SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Parent] = @ChiParent";
            m_FirstChildDataAdapter_MarkOrient.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Pad] SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Parent] = @ChiParent";
            m_FirstChildDataAdapter_Pad.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Lead3D] SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Parent] = @ChiParent";
            m_FirstChildDataAdapter_Lead3D.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_InPocket] SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Parent] = @ChiParent";
            m_FirstChildDataAdapter_InPocket.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_InPocket2] SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Parent] = @ChiParent";
            m_FirstChildDataAdapter_InPocket2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Seal] SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Parent] = @ChiParent";
            m_FirstChildDataAdapter_Seal.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Seal2] SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Parent] = @ChiParent";
            m_FirstChildDataAdapter_Seal2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Barcode] SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Parent] = @ChiParent";
            m_FirstChildDataAdapter_Barcode.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_TopMenu] SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Parent] = @ChiParent";
            m_FirstChildDataAdapter_TopMenu.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_TopMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_TopMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_TopMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_BottomMenu] SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Parent] = @ChiParent";
            m_FirstChildDataAdapter_BottomMenu.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_BottomMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_BottomMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_BottomMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiParent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_Orientation SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child1] = @ChiChild1";
            m_SecondChildDataAdapter_Orientation.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_MarkOrient SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child1] = @ChiChild1";
            m_SecondChildDataAdapter_MarkOrient.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_Pad SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child1] = @ChiChild1";
            m_SecondChildDataAdapter_Pad.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_Lead3D SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child1] = @ChiChild1";
            m_SecondChildDataAdapter_Lead3D.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_InPocket SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child1] = @ChiChild1";
            m_SecondChildDataAdapter_InPocket.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_InPocket2 SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child1] = @ChiChild1";
            m_SecondChildDataAdapter_InPocket2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_Seal SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child1] = @ChiChild1";
            m_SecondChildDataAdapter_Seal.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_Seal2 SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child1] = @ChiChild1";
            m_SecondChildDataAdapter_Seal2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_Barcode SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child1] = @ChiChild1";
            m_SecondChildDataAdapter_Barcode.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_TopMenu SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child1] = @ChiChild1";
            m_SecondChildDataAdapter_TopMenu.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_TopMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_TopMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_TopMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_BottomMenu SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child1] = @ChiChild1";
            m_SecondChildDataAdapter_BottomMenu.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_BottomMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_BottomMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_BottomMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_Orientation SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child2] = @ChiChild2 AND [Chi Child1] = @ChiChild1";
            m_ThirdChildDataAdapter_Orientation.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_MarkOrient SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child2] = @ChiChild2 AND [Chi Child1] = @ChiChild1";
            m_ThirdChildDataAdapter_MarkOrient.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_Pad SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child2] = @ChiChild2 AND [Chi Child1] = @ChiChild1";
            m_ThirdChildDataAdapter_Pad.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_Lead3D SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child2] = @ChiChild2 AND [Chi Child1] = @ChiChild1";
            m_ThirdChildDataAdapter_Lead3D.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_InPocket SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child2] = @ChiChild2 AND [Chi Child1] = @ChiChild1";
            m_ThirdChildDataAdapter_InPocket.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_InPocket2 SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child2] = @ChiChild2 AND [Chi Child1] = @ChiChild1";
            m_ThirdChildDataAdapter_InPocket2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_Seal SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child2] = @ChiChild2 AND [Chi Child1] = @ChiChild1";
            m_ThirdChildDataAdapter_Seal.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_Seal2 SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child2] = @ChiChild2 AND [Chi Child1] = @ChiChild1";
            m_ThirdChildDataAdapter_Seal2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_Barcode SET [Group] = @Group WHERE [Chi Name] = @ChiName AND [Chi Child2] = @ChiChild2 AND [Chi Child1] = @ChiChild1";
            m_ThirdChildDataAdapter_Barcode.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiName", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@ChiChild1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Chi Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            try
            {
                m_ParentDataAdapter.Fill(m_dsParent, "Parent");

                m_FirstChildDataAdapter_Orientation.Fill(m_dsFirstChild_Orientation, "Child1_Orientation");
                m_FirstChildDataAdapter_MarkOrient.Fill(m_dsFirstChild_MarkOrient, "Child1_MarkOrient");
                m_FirstChildDataAdapter_Pad.Fill(m_dsFirstChild_Pad, "Child1_Pad");
                m_FirstChildDataAdapter_Lead3D.Fill(m_dsFirstChild_Lead3D, "Child1_Lead3D");
                m_FirstChildDataAdapter_InPocket.Fill(m_dsFirstChild_InPocket, "Child1_InPocket");
                m_FirstChildDataAdapter_InPocket2.Fill(m_dsFirstChild_InPocket2, "Child1_InPocket2");
                m_FirstChildDataAdapter_Seal.Fill(m_dsFirstChild_Seal, "Child1_Seal");
                m_FirstChildDataAdapter_Seal2.Fill(m_dsFirstChild_Seal2, "Child1_Seal2");
                m_FirstChildDataAdapter_Barcode.Fill(m_dsFirstChild_Barcode, "Child1_Barcode");
                m_FirstChildDataAdapter_TopMenu.Fill(m_dsFirstChild_TopMenu, "Child1_TopMenu");
                m_FirstChildDataAdapter_BottomMenu.Fill(m_dsFirstChild_BottomMenu, "Child1_BottomMenu");

                m_SecondChildDataAdapter_Orientation.Fill(m_dsSecondChild_Orientation, "Child2_Orientation");
                m_SecondChildDataAdapter_MarkOrient.Fill(m_dsSecondChild_MarkOrient, "Child2_MarkOrient");
                m_SecondChildDataAdapter_Pad.Fill(m_dsSecondChild_Pad, "Child2_Pad");
                m_SecondChildDataAdapter_Lead3D.Fill(m_dsSecondChild_Lead3D, "Child2_Lead3D");
                m_SecondChildDataAdapter_InPocket.Fill(m_dsSecondChild_InPocket, "Child2_InPocket");
                m_SecondChildDataAdapter_InPocket2.Fill(m_dsSecondChild_InPocket2, "Child2_InPocket2");
                m_SecondChildDataAdapter_Seal.Fill(m_dsSecondChild_Seal, "Child2_Seal");
                m_SecondChildDataAdapter_Seal2.Fill(m_dsSecondChild_Seal2, "Child2_Seal2");
                m_SecondChildDataAdapter_Barcode.Fill(m_dsSecondChild_Barcode, "Child2_Barcode");
                m_SecondChildDataAdapter_TopMenu.Fill(m_dsSecondChild_TopMenu, "Child2_TopMenu");
                m_SecondChildDataAdapter_BottomMenu.Fill(m_dsSecondChild_BottomMenu, "Child2_BottomMenu");

                m_ThirdChildDataAdapter_Orientation.Fill(m_dsThirdChild_Orientation, "Child3_Orientation");
                m_ThirdChildDataAdapter_MarkOrient.Fill(m_dsThirdChild_MarkOrient, "Child3_MarkOrient");
                m_ThirdChildDataAdapter_Pad.Fill(m_dsThirdChild_Pad, "Child3_Pad");
                m_ThirdChildDataAdapter_Lead3D.Fill(m_dsThirdChild_Lead3D, "Child3_Lead3D");
                m_ThirdChildDataAdapter_InPocket.Fill(m_dsThirdChild_InPocket, "Child3_InPocket");
                m_ThirdChildDataAdapter_InPocket2.Fill(m_dsThirdChild_InPocket2, "Child3_InPocket2");
                m_ThirdChildDataAdapter_Seal.Fill(m_dsThirdChild_Seal, "Child3_Seal");
                m_ThirdChildDataAdapter_Seal2.Fill(m_dsThirdChild_Seal2, "Child3_Seal2");
                m_ThirdChildDataAdapter_Barcode.Fill(m_dsThirdChild_Barcode, "Child3_Barcode");

            }
            finally
            {
                accessConn.Close();
            }

            accessConn = new OleDbConnection();
            accessConn.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                @"data source = " + AppDomain.CurrentDomain.BaseDirectory + "access\\simeca.mdb";
            accessConn.Open();

            m_dsIO = new DataSet();
            m_dsIO.Tables.Add("IO");


            accessCommand = new OleDbCommand("SELECT * FROM IO", accessConn);
            m_IODataAdapter = new OleDbDataAdapter(accessCommand);

            sqlUpdate = "UPDATE IO SET [Group] = @Group WHERE [Description] = @Description AND [Module] = @Module AND TYPE = 'Output'";
            m_IODataAdapter.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_IODataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_IODataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Description", OleDbType.VarChar));
            m_workParam.SourceColumn = "Description";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_IODataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Module", OleDbType.VarChar));
            m_workParam.SourceColumn = "Module";
            m_workParam.SourceVersion = DataRowVersion.Current;

            try
            {
                m_IODataAdapter.Fill(m_dsIO, "IO");
            }
            finally
            {
                accessConn.Close();
            }
        }
        private void GetRightDataSet()
        {
            string sqlUpdate = "";
            OleDbCommand accessCommand;
            OleDbConnection accessConn;

            accessConn = new OleDbConnection();
            accessConn.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                @"data source = " + AppDomain.CurrentDomain.BaseDirectory + "access\\setting.mdb";
            accessConn.Open();

            m_dsParent = new DataSet();
            m_dsParent.Tables.Add("Parent");

            m_dsFirstChild_Orientation = new DataSet();
            m_dsFirstChild_Orientation.Tables.Add("Child1_Orientation");
            m_dsFirstChild_MarkOrient = new DataSet();
            m_dsFirstChild_MarkOrient.Tables.Add("Child1_MarkOrient");
            m_dsFirstChild_Pad = new DataSet();
            m_dsFirstChild_Pad.Tables.Add("Child1_Pad");
            m_dsFirstChild_Lead3D = new DataSet();
            m_dsFirstChild_Lead3D.Tables.Add("Child1_Lead3D");
            m_dsFirstChild_InPocket = new DataSet();
            m_dsFirstChild_InPocket.Tables.Add("Child1_InPocket");
            m_dsFirstChild_InPocket2 = new DataSet();
            m_dsFirstChild_InPocket.Tables.Add("Child1_InPocket2");
            m_dsFirstChild_Seal = new DataSet();
            m_dsFirstChild_Seal.Tables.Add("Child1_Seal");
            m_dsFirstChild_Seal2 = new DataSet();
            m_dsFirstChild_Seal2.Tables.Add("Child1_Seal2");
            m_dsFirstChild_Barcode = new DataSet();
            m_dsFirstChild_Barcode.Tables.Add("Child1_Barcode");
            m_dsFirstChild_TopMenu = new DataSet();
            m_dsFirstChild_TopMenu.Tables.Add("Child1_TopMenu");
            m_dsFirstChild_BottomMenu = new DataSet();
            m_dsFirstChild_BottomMenu.Tables.Add("Child1_BottomMenu");

            m_dsSecondChild_Orientation = new DataSet();
            m_dsSecondChild_Orientation.Tables.Add("Child2_Orientation");
            m_dsSecondChild_MarkOrient = new DataSet();
            m_dsSecondChild_MarkOrient.Tables.Add("Child2_MarkOrient");
            m_dsSecondChild_Pad = new DataSet();
            m_dsSecondChild_Pad.Tables.Add("Child2_Pad");
            m_dsSecondChild_Lead3D = new DataSet();
            m_dsSecondChild_Lead3D.Tables.Add("Child2_Lead3D");
            m_dsSecondChild_InPocket = new DataSet();
            m_dsSecondChild_InPocket.Tables.Add("Child2_InPocket");
            m_dsSecondChild_InPocket2 = new DataSet();
            m_dsSecondChild_InPocket2.Tables.Add("Child2_InPocket2");
            m_dsSecondChild_Seal = new DataSet();
            m_dsSecondChild_Seal.Tables.Add("Child2_Seal");
            m_dsSecondChild_Seal2 = new DataSet();
            m_dsSecondChild_Seal2.Tables.Add("Child2_Seal2");
            m_dsSecondChild_Barcode = new DataSet();
            m_dsSecondChild_Barcode.Tables.Add("Child2_Barcode");
            m_dsSecondChild_TopMenu = new DataSet();
            m_dsSecondChild_TopMenu.Tables.Add("Child2_TopMenu");
            m_dsSecondChild_BottomMenu = new DataSet();
            m_dsSecondChild_BottomMenu.Tables.Add("Child2_BottomMenu");

            m_dsThirdChild_Orientation = new DataSet();
            m_dsThirdChild_Orientation.Tables.Add("Child3_Orientation");
            m_dsThirdChild_MarkOrient = new DataSet();
            m_dsThirdChild_MarkOrient.Tables.Add("Child3_MarkOrient");
            m_dsThirdChild_Pad = new DataSet();
            m_dsThirdChild_Pad.Tables.Add("Child3_Pad");
            m_dsThirdChild_Lead3D = new DataSet();
            m_dsThirdChild_Lead3D.Tables.Add("Child3_Lead3D");
            m_dsThirdChild_InPocket = new DataSet();
            m_dsThirdChild_InPocket.Tables.Add("Child3_InPocket");
            m_dsThirdChild_InPocket2 = new DataSet();
            m_dsThirdChild_InPocket2.Tables.Add("Child3_InPocket2");
            m_dsThirdChild_Seal = new DataSet();
            m_dsThirdChild_Seal.Tables.Add("Child3_Seal");
            m_dsThirdChild_Seal2 = new DataSet();
            m_dsThirdChild_Seal2.Tables.Add("Child3_Seal2");
            m_dsThirdChild_Barcode = new DataSet();
            m_dsThirdChild_Barcode.Tables.Add("Child3_Barcode");

            accessCommand = new OleDbCommand("SELECT * FROM Parent", accessConn);
            m_ParentDataAdapter = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child1_Orientation", accessConn);
            m_FirstChildDataAdapter_Orientation = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_MarkOrient", accessConn);
            m_FirstChildDataAdapter_MarkOrient = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_Pad", accessConn);
            m_FirstChildDataAdapter_Pad = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_Lead3D", accessConn);
            m_FirstChildDataAdapter_Lead3D = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_InPocket", accessConn);
            m_FirstChildDataAdapter_InPocket = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_InPocket2", accessConn);
            m_FirstChildDataAdapter_InPocket2 = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_Seal", accessConn);
            m_FirstChildDataAdapter_Seal = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_Seal2", accessConn);
            m_FirstChildDataAdapter_Seal2 = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_Barcode", accessConn);
            m_FirstChildDataAdapter_Barcode = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_TopMenu", accessConn);
            m_FirstChildDataAdapter_TopMenu = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child1_BottomMenu", accessConn);
            m_FirstChildDataAdapter_BottomMenu = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child2_Orientation", accessConn);
            m_SecondChildDataAdapter_Orientation = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_MarkOrient", accessConn);
            m_SecondChildDataAdapter_MarkOrient = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_Pad", accessConn);
            m_SecondChildDataAdapter_Pad = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_Lead3D", accessConn);
            m_SecondChildDataAdapter_Lead3D = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_InPocket", accessConn);
            m_SecondChildDataAdapter_InPocket = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_InPocket2", accessConn);
            m_SecondChildDataAdapter_InPocket2 = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_Seal", accessConn);
            m_SecondChildDataAdapter_Seal = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_Seal2", accessConn);
            m_SecondChildDataAdapter_Seal2 = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_Barcode", accessConn);
            m_SecondChildDataAdapter_Barcode = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_TopMenu", accessConn);
            m_SecondChildDataAdapter_TopMenu = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child2_BottomMenu", accessConn);
            m_SecondChildDataAdapter_BottomMenu = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Child3_Orientation", accessConn);
            m_ThirdChildDataAdapter_Orientation = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_MarkOrient", accessConn);
            m_ThirdChildDataAdapter_MarkOrient = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_Pad", accessConn);
            m_ThirdChildDataAdapter_Pad = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_Lead3D", accessConn);
            m_ThirdChildDataAdapter_Lead3D = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_InPocket", accessConn);
            m_ThirdChildDataAdapter_InPocket = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_InPocket2", accessConn);
            m_ThirdChildDataAdapter_InPocket2 = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_Seal", accessConn);
            m_ThirdChildDataAdapter_Seal = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_Seal2", accessConn);
            m_ThirdChildDataAdapter_Seal2 = new OleDbDataAdapter(accessCommand);
            accessCommand = new OleDbCommand("SELECT * FROM Child3_Barcode", accessConn);
            m_ThirdChildDataAdapter_Barcode = new OleDbDataAdapter(accessCommand);

            sqlUpdate = "UPDATE Parent SET [Group] = @Group WHERE [Name] = @Name";
            m_ParentDataAdapter.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ParentDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ParentDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Orientation] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
            m_FirstChildDataAdapter_Orientation.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_MarkOrient] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
            m_FirstChildDataAdapter_MarkOrient.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Pad] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
            m_FirstChildDataAdapter_Pad.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Lead3D] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
            m_FirstChildDataAdapter_Lead3D.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_InPocket] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
            m_FirstChildDataAdapter_InPocket.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_InPocket2] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
            m_FirstChildDataAdapter_InPocket2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Seal] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
            m_FirstChildDataAdapter_Seal.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Seal2] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
            m_FirstChildDataAdapter_Seal2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_Barcode] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
            m_FirstChildDataAdapter_Barcode.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_TopMenu] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
            m_FirstChildDataAdapter_TopMenu.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_TopMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_TopMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_TopMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [Child1_BottomMenu] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
            m_FirstChildDataAdapter_BottomMenu.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_FirstChildDataAdapter_BottomMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_BottomMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_FirstChildDataAdapter_BottomMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_Orientation SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
            m_SecondChildDataAdapter_Orientation.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_MarkOrient SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
            m_SecondChildDataAdapter_MarkOrient.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_Pad SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
            m_SecondChildDataAdapter_Pad.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_Lead3D SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
            m_SecondChildDataAdapter_Lead3D.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_InPocket SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
            m_SecondChildDataAdapter_InPocket.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_InPocket2 SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
            m_SecondChildDataAdapter_InPocket2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_Seal SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
            m_SecondChildDataAdapter_Seal.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_Seal2 SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
            m_SecondChildDataAdapter_Seal2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_Barcode SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
            m_SecondChildDataAdapter_Barcode.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_TopMenu SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
            m_SecondChildDataAdapter_TopMenu.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_TopMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_TopMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_TopMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child2_BottomMenu SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
            m_SecondChildDataAdapter_BottomMenu.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_SecondChildDataAdapter_BottomMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_BottomMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_SecondChildDataAdapter_BottomMenu.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_Orientation SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
            m_ThirdChildDataAdapter_Orientation.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Orientation.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_MarkOrient SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
            m_ThirdChildDataAdapter_MarkOrient.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_MarkOrient.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_Pad SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
            m_ThirdChildDataAdapter_Pad.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Pad.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_Lead3D SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
            m_ThirdChildDataAdapter_Lead3D.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Lead3D.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_InPocket SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
            m_ThirdChildDataAdapter_InPocket.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_InPocket.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_InPocket2 SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
            m_ThirdChildDataAdapter_InPocket2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_InPocket2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_Seal SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
            m_ThirdChildDataAdapter_Seal.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Seal.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_Seal2 SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
            m_ThirdChildDataAdapter_Seal2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Seal2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Child3_Barcode SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
            m_ThirdChildDataAdapter_Barcode.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_ThirdChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child2";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_ThirdChildDataAdapter_Barcode.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
            m_workParam.SourceColumn = "Child1";
            m_workParam.SourceVersion = DataRowVersion.Current;

            try
            {
                m_ParentDataAdapter.Fill(m_dsParent, "Parent");

                m_FirstChildDataAdapter_Orientation.Fill(m_dsFirstChild_Orientation, "Child1_Orientation");
                m_FirstChildDataAdapter_MarkOrient.Fill(m_dsFirstChild_MarkOrient, "Child1_MarkOrient");
                m_FirstChildDataAdapter_Pad.Fill(m_dsFirstChild_Pad, "Child1_Pad");
                m_FirstChildDataAdapter_Lead3D.Fill(m_dsFirstChild_Lead3D, "Child1_Lead3D");
                m_FirstChildDataAdapter_InPocket.Fill(m_dsFirstChild_InPocket, "Child1_InPocket");
                m_FirstChildDataAdapter_InPocket2.Fill(m_dsFirstChild_InPocket2, "Child1_InPocket2");
                m_FirstChildDataAdapter_Seal.Fill(m_dsFirstChild_Seal, "Child1_Seal");
                m_FirstChildDataAdapter_Seal2.Fill(m_dsFirstChild_Seal2, "Child1_Seal2");
                m_FirstChildDataAdapter_Barcode.Fill(m_dsFirstChild_Barcode, "Child1_Barcode");
                m_FirstChildDataAdapter_TopMenu.Fill(m_dsFirstChild_TopMenu, "Child1_TopMenu");
                m_FirstChildDataAdapter_BottomMenu.Fill(m_dsFirstChild_BottomMenu, "Child1_BottomMenu");

                m_SecondChildDataAdapter_Orientation.Fill(m_dsSecondChild_Orientation, "Child2_Orientation");
                m_SecondChildDataAdapter_MarkOrient.Fill(m_dsSecondChild_MarkOrient, "Child2_MarkOrient");
                m_SecondChildDataAdapter_Pad.Fill(m_dsSecondChild_Pad, "Child2_Pad");
                m_SecondChildDataAdapter_Lead3D.Fill(m_dsSecondChild_Lead3D, "Child2_Lead3D");
                m_SecondChildDataAdapter_InPocket.Fill(m_dsSecondChild_InPocket, "Child2_InPocket");
                m_SecondChildDataAdapter_InPocket2.Fill(m_dsSecondChild_InPocket2, "Child2_InPocket2");
                m_SecondChildDataAdapter_Seal.Fill(m_dsSecondChild_Seal, "Child2_Seal");
                m_SecondChildDataAdapter_Seal2.Fill(m_dsSecondChild_Seal2, "Child2_Seal2");
                m_SecondChildDataAdapter_Barcode.Fill(m_dsSecondChild_Barcode, "Child2_Barcode");
                m_SecondChildDataAdapter_TopMenu.Fill(m_dsSecondChild_TopMenu, "Child2_TopMenu");
                m_SecondChildDataAdapter_BottomMenu.Fill(m_dsSecondChild_BottomMenu, "Child2_BottomMenu");

                m_ThirdChildDataAdapter_Orientation.Fill(m_dsThirdChild_Orientation, "Child3_Orientation");
                m_ThirdChildDataAdapter_MarkOrient.Fill(m_dsThirdChild_MarkOrient, "Child3_MarkOrient");
                m_ThirdChildDataAdapter_Pad.Fill(m_dsThirdChild_Pad, "Child3_Pad");
                m_ThirdChildDataAdapter_Lead3D.Fill(m_dsThirdChild_Lead3D, "Child3_Lead3D");
                m_ThirdChildDataAdapter_InPocket.Fill(m_dsThirdChild_InPocket, "Child3_InPocket");
                m_ThirdChildDataAdapter_InPocket2.Fill(m_dsThirdChild_InPocket2, "Child3_InPocket2");
                m_ThirdChildDataAdapter_Seal.Fill(m_dsThirdChild_Seal, "Child3_Seal");
                m_ThirdChildDataAdapter_Seal2.Fill(m_dsThirdChild_Seal2, "Child3_Seal2");
                m_ThirdChildDataAdapter_Barcode.Fill(m_dsThirdChild_Barcode, "Child3_Barcode");

            }
            finally
            {
                accessConn.Close();
            }

            accessConn = new OleDbConnection();
            accessConn.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                @"data source = " + AppDomain.CurrentDomain.BaseDirectory + "access\\simeca.mdb";
            accessConn.Open();

            m_dsIO = new DataSet();
            m_dsIO.Tables.Add("IO");


            accessCommand = new OleDbCommand("SELECT * FROM IO", accessConn);
            m_IODataAdapter = new OleDbDataAdapter(accessCommand);

            sqlUpdate = "UPDATE IO SET [Group] = @Group WHERE [Description] = @Description AND [Module] = @Module AND TYPE = 'Output'";
            m_IODataAdapter.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_IODataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_IODataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Description", OleDbType.VarChar));
            m_workParam.SourceColumn = "Description";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_IODataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Module", OleDbType.VarChar));
            m_workParam.SourceColumn = "Module";
            m_workParam.SourceVersion = DataRowVersion.Current;

            try
            {
                m_IODataAdapter.Fill(m_dsIO, "IO");
            }
            finally
            {
                accessConn.Close();
            }
        }

        private DataRow[] GetChildDataRow(string ParentName, string filter, string sort, int ChildIndex)
        {
            switch (ParentName)
            {
                case "Top Menu":
                case "顶部菜单":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_TopMenu.Tables["Child1_TopMenu"].Select(filter, sort);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_TopMenu.Tables["Child2_TopMenu"].Select(filter, sort);
                    break;
                case "Bottom Menu":
                case "底部菜单":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_BottomMenu.Tables["Child1_BottomMenu"].Select(filter, sort);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_BottomMenu.Tables["Child2_BottomMenu"].Select(filter, sort);
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Orient":
                case "BottomOrient":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_Orientation.Tables["Child1_Orientation"].Select(filter, sort);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_Orientation.Tables["Child2_Orientation"].Select(filter, sort);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_Orientation.Tables["Child3_Orientation"].Select(filter, sort);
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "Package":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_MarkOrient.Tables["Child1_MarkOrient"].Select(filter, sort);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_MarkOrient.Tables["Child2_MarkOrient"].Select(filter, sort);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_MarkOrient.Tables["Child3_MarkOrient"].Select(filter, sort);
                    break;
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLi":
                case "IPMLiPkg":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_InPocket.Tables["Child1_InPocket"].Select(filter, sort);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_InPocket.Tables["Child2_InPocket"].Select(filter, sort);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_InPocket.Tables["Child3_InPocket"].Select(filter, sort);
                    break;
                case "InPocket2":
                case "InPocketPkg2":
                case "InPocketPkgPos2":
                case "IPMLi2":
                case "IPMLiPkg2":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_InPocket2.Tables["Child1_InPocket2"].Select(filter, sort);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_InPocket2.Tables["Child2_InPocket2"].Select(filter, sort);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_InPocket2.Tables["Child3_InPocket2"].Select(filter, sort);
                    break;

                //case "BottomPosition":
                //case "BottomPositionOrient":
                //    break;
                //case "TapePocketPosition":
                //    break;
                //case "BottomOrientPad":
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_Pad.Tables["Child1_Pad"].Select(filter, sort);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_Pad.Tables["Child2_Pad"].Select(filter, sort);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_Pad.Tables["Child3_Pad"].Select(filter, sort);
                    break;
                case "Li3D":
                case "Li3DPkg":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_Lead3D.Tables["Child1_Lead3D"].Select(filter, sort);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_Lead3D.Tables["Child2_Lead3D"].Select(filter, sort);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_Lead3D.Tables["Child3_Lead3D"].Select(filter, sort);
                    break;
                case "Seal":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_Seal.Tables["Child1_Seal"].Select(filter, sort);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_Seal.Tables["Child2_Seal"].Select(filter, sort);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_Seal.Tables["Child3_Seal"].Select(filter, sort);
                    break;
                case "Seal2":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_Seal2.Tables["Child1_Seal2"].Select(filter, sort);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_Seal2.Tables["Child2_Seal2"].Select(filter, sort);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_Seal2.Tables["Child3_Seal2"].Select(filter, sort);
                    break;
                case "Barcode":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_Barcode.Tables["Child1_Barcode"].Select(filter, sort);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_Barcode.Tables["Child2_Barcode"].Select(filter, sort);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_Barcode.Tables["Child3_Barcode"].Select(filter, sort);
                    break;
                    //default:

                    //    break;
            }

            return new DataRow[0];
        }

        private DataRow[] GetChildDataRow(string ParentName, string filter, int ChildIndex)
        {
            switch (ParentName)
            {
                case "Top Menu":
                case "顶部菜单":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_TopMenu.Tables["Child1_TopMenu"].Select(filter);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_TopMenu.Tables["Child2_TopMenu"].Select(filter);
                    break;
                case "Bottom Menu":
                case "底部菜单":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_BottomMenu.Tables["Child1_BottomMenu"].Select(filter);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_BottomMenu.Tables["Child2_BottomMenu"].Select(filter);
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Orient":
                case "BottomOrient":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_Orientation.Tables["Child1_Orientation"].Select(filter);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_Orientation.Tables["Child2_Orientation"].Select(filter);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_Orientation.Tables["Child3_Orientation"].Select(filter);
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "Package":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_MarkOrient.Tables["Child1_MarkOrient"].Select(filter);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_MarkOrient.Tables["Child2_MarkOrient"].Select(filter);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_MarkOrient.Tables["Child3_MarkOrient"].Select(filter);
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_InPocket.Tables["Child1_InPocket"].Select(filter);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_InPocket.Tables["Child2_InPocket"].Select(filter);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_InPocket.Tables["Child3_InPocket"].Select(filter);
                    break;
                case "IPMLi2":
                case "IPMLiPkg2":
                case "InPocket2":
                case "InPocketPkg2":
                case "InPocketPkgPos2":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_InPocket2.Tables["Child1_InPocket2"].Select(filter);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_InPocket2.Tables["Child2_InPocket2"].Select(filter);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_InPocket2.Tables["Child3_InPocket2"].Select(filter);
                    break;
                //case "BottomPosition":
                //case "BottomPositionOrient":
                //    break;
                //case "TapePocketPosition":
                //    break;
                //case "BottomOrientPad":
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_Pad.Tables["Child1_Pad"].Select(filter);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_Pad.Tables["Child2_Pad"].Select(filter);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_Pad.Tables["Child3_Pad"].Select(filter);
                    break;
                case "Li3D":
                case "Li3DPkg":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_Lead3D.Tables["Child1_Lead3D"].Select(filter);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_Lead3D.Tables["Child2_Lead3D"].Select(filter);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_Lead3D.Tables["Child3_Lead3D"].Select(filter);
                    break;
                case "Seal":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_Seal.Tables["Child1_Seal"].Select(filter);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_Seal.Tables["Child2_Seal"].Select(filter);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_Seal.Tables["Child3_Seal"].Select(filter);
                    break;
                case "Seal2":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_Seal2.Tables["Child1_Seal2"].Select(filter);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_Seal2.Tables["Child2_Seal2"].Select(filter);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_Seal2.Tables["Child3_Seal2"].Select(filter);
                    break;
                case "Barcode":
                    if (ChildIndex == 1)
                        return m_dsFirstChild_Barcode.Tables["Child1_Barcode"].Select(filter);
                    else if (ChildIndex == 2)
                        return m_dsSecondChild_Barcode.Tables["Child2_Barcode"].Select(filter);
                    else if (ChildIndex == 3)
                        return m_dsThirdChild_Barcode.Tables["Child3_Barcode"].Select(filter);
                    break;
                    //default:

                    //    break;
            }

            return new DataRow[0];
        }
        private void InitParentTree_CH()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            int ParentArrayGroup = 0;
            int FirstChildArrayGroup = 0;
            int SecondChildArrayGroup = 0;
            int ThirdChildArrayGroup = 0;

            m_intParentArrayGroup = new int[20]; // 10 Vision + Top Menu
            m_intFirstChildArrayGroup = new int[20][];

            for (int x = 0; x < m_intFirstChildArrayGroup.Length; x++)
            {
                m_intFirstChildArrayGroup[x] = new int[20];
            }

            m_intSecondChildArrayGroup = new int[20][][];
            for (int x = 0; x < m_intFirstChildArrayGroup.Length; x++)
            {
                m_intSecondChildArrayGroup[x] = new int[20][];
                for (int y = 0; y < m_intSecondChildArrayGroup[x].Length; y++)
                {
                    m_intSecondChildArrayGroup[x][y] = new int[20];
                }
            }

            m_intThirdChildArrayGroup = new int[20][][][];
            for (int x = 0; x < m_intSecondChildArrayGroup.Length; x++)
            {
                m_intThirdChildArrayGroup[x] = new int[20][][];
                for (int y = 0; y < m_intThirdChildArrayGroup[x].Length; y++)
                {
                    m_intThirdChildArrayGroup[x][y] = new int[20][];
                    for (int z = 0; z < m_intThirdChildArrayGroup[x][y].Length; z++)
                    {
                        m_intThirdChildArrayGroup[x][y][z] = new int[100];
                    }
                }
            }

            try
            {
                string sort = "Number";
                string filter = "Number > 0";
                DataRow[] ParentList = m_dsParent.Tables["Parent"].Select(filter, sort);
                foreach (DataRow parent in ParentList)
                {
                    string strParentName = parent["Chi Name"].ToString();
                    string strFirstChild = parent["Child1"].ToString();
                    int intParentNum = Convert.ToInt32(parent["Number"]);
                    int intParentGroup = Convert.ToInt32(parent["Group"]);

                    if (m_intGroup <= intParentGroup && strParentName != " ")
                    {
                        TreeNode newParent = new TreeNode(strParentName);
                        tre_UserRight.Nodes.Add(newParent);


                        m_intParentArrayGroup[ParentArrayGroup] = intParentGroup;
                        FirstChildArrayGroup = 0;

                        if (strFirstChild != "")
                        {
                            sort = "Number";
                            filter = "[Parent Number] = " + intParentNum;
                            DataRow[] FirstChildList = GetChildDataRow(strParentName, filter, sort, 1);
                            foreach (DataRow Firstchild in FirstChildList)
                            {
                                string strFirstChildName = Firstchild["Chi Name"].ToString();
                                string strSecondChild = Firstchild["Child2"].ToString();
                                int intFirstChildNo = Convert.ToInt32(Firstchild["Number"]);
                                int intFirstChildGroup = Convert.ToInt32(Firstchild["Group"]);

                                if (m_intGroup <= intFirstChildGroup)
                                {
                                    TreeNode newFirstChild = new TreeNode(strFirstChildName);
                                    newParent.Nodes.Add(newFirstChild);
                                    m_intFirstChildArrayGroup[ParentArrayGroup][FirstChildArrayGroup] = intFirstChildGroup;
                                    SecondChildArrayGroup = 0;
                                    if (strSecondChild != "")
                                    {
                                        sort = "Number";
                                        filter = "[Child1 Number] = " + intFirstChildNo;
                                        DataRow[] SecondChildList = GetChildDataRow(strParentName, filter, sort, 2);
                                        foreach (DataRow SecondChild in SecondChildList)
                                        {

                                            string strSecondChildName = SecondChild["Chi Name"].ToString();
                                            string strThirdChild = SecondChild["Child3"].ToString();
                                            int intSecondChildNo = Convert.ToInt32(SecondChild["Number"]);
                                            int intSecondChildGroup = Convert.ToInt32(SecondChild["Group"]);

                                            if (tre_UserRight.Nodes.Count > 2 && (strFirstChildName == "容许度" || strFirstChildName == "选项"))
                                            {
                                                if (strSecondChildName.Contains("Orient") && ((m_smCustomOption.g_intWantOrient & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                                if (strSecondChildName.Contains("Orient") && ((m_smCustomOption.g_intWantOrient0Deg & (1 << (ParentArrayGroup - 2))) > 0))
                                                {
                                                    continue;
                                                }
                                                if (strSecondChildName.Contains("Mark") && ((m_smCustomOption.g_intWantMark & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                                if (strSecondChildName.Contains("Package") && ((m_smCustomOption.g_intWantPackage & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                                if (strSecondChildName.Contains("Color") && ((m_smCustomOption.g_intUseColorCamera & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                                if (strSecondChildName.Contains("Pad") &&
                                                    ((m_smCustomOption.g_intWantPad5S & (1 << (ParentArrayGroup - 2))) == 0) &&
                                                    ((m_smCustomOption.g_intWantPad & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                                if (strSecondChildName.Contains("Lead") &&
                                                  ((m_smCustomOption.g_intWantLead3D & (1 << (ParentArrayGroup - 2))) == 0) &&
                                                  ((m_smCustomOption.g_intWantLead & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                                if (strSecondChildName.Contains("Seal") && ((m_smCustomOption.g_intWantSeal & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                                if (strSecondChildName.Contains("Barcode") && ((m_smCustomOption.g_intWantBarcode & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                            }
                                            else if (tre_UserRight.Nodes.Count > 2 && (strFirstChildName == "容许度衬垫"))
                                            {

                                                if (strSecondChildName.Contains("Pad") &&
                                                    ((m_smCustomOption.g_intWantPad5S & (1 << (ParentArrayGroup - 2))) == 0) &&
                                                    ((m_smCustomOption.g_intWantPad & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                            }
                                            else if (tre_UserRight.Nodes.Count > 2 && (strFirstChildName == "容许度管脚3D"))
                                            {

                                                if (strSecondChildName.Contains("Lead3D") &&
                                                    ((m_smCustomOption.g_intWantLead3D & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                            }
                                            else if (tre_UserRight.Nodes.Count > 2 && (strFirstChildName == "Pad"))
                                            {

                                                if (strSecondChildName.Contains("Orient") &&
                                                    ((m_smCustomOption.g_intWantOrient & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }


                                                if (strSecondChildName.Contains("Color") &&
                                                   ((m_smCustomOption.g_intUseColorCamera & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                            }
                                            else if (tre_UserRight.Nodes.Count > 2 && (strFirstChildName == "Package"))
                                            {
                                                if (strSecondChildName.Contains("Color") &&
                                                   ((m_smCustomOption.g_intUseColorCamera & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                            }
                                            else if (tre_UserRight.Nodes.Count > 2 && (strFirstChildName == "System"))
                                            {

                                                if (strSecondChildName.Contains("Orient") &&
                                                    ((m_smCustomOption.g_intWantOrient & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                            }

                                            if (m_intGroup <= intSecondChildGroup)
                                            {
                                                TreeNode newSecondChild = new TreeNode(strSecondChildName);
                                                newFirstChild.Nodes.Add(newSecondChild);
                                                m_intSecondChildArrayGroup[ParentArrayGroup][FirstChildArrayGroup][SecondChildArrayGroup] = intSecondChildGroup;
                                                ThirdChildArrayGroup = 0;
                                                if (strThirdChild != "")
                                                {
                                                    sort = "Number";
                                                    filter = "[Child2 Number] = " + intSecondChildNo;
                                                    DataRow[] ThirdChildList = GetChildDataRow(strParentName, filter, sort, 3);
                                                    foreach (DataRow ThirdChild in ThirdChildList)
                                                    {

                                                        string strThirdChildName = ThirdChild["Chi Name"].ToString();
                                                        int intThirdChildGroup = Convert.ToInt32(ThirdChild["Group"]);
                                                        if (m_intGroup <= intThirdChildGroup)
                                                        {
                                                            if ((newParent.Text == "MarkPkg" || newParent.Text == "Mark") && strThirdChildName == "方向")
                                                                continue;

                                                            if (!(newParent.Text == "MOLiPkg" || newParent.Text == "MOLi" || newParent.Text == "IPMLi" || newParent.Text == "IPMLiPkg")
                                                                && (strThirdChildName == "开启使用管脚点来偏移字模ROI"
                                                                || strThirdChildName == "字模ROI管脚Base偏移设置"
                                                                || strThirdChildName == "管脚往内忽略容许度设置"))
                                                                continue;

                                                            if (!(newParent.Text.Contains("InPocket") || newParent.Text.Contains("IPM"))
                                                                && (strThirdChildName == "使用元件Pattern代替字模Pattern"
                                                                || strThirdChildName == "Base管脚视阈图"
                                                                || strThirdChildName == "开启检测Base管脚"
                                                                || strThirdChildName == "Base管脚设置"
                                                                || strThirdChildName == "管脚二进制化"
                                                                || strThirdChildName == "Base 管脚成立对象"))
                                                                continue;

                                                            if ((newParent.Text.Contains("Pad") && !newParent.Text.Contains("Orient")) && strThirdChildName == "Pad Orient Direction")
                                                                continue;

                                                            if (newParent.Text == "Li3DPkg" && strThirdChildName == "管脚3D塑封体")
                                                                continue;

                                                            if (strThirdChildName.Contains("塑封体") && newParent.Text.Contains("Orient") &&
                                                    ((m_smCustomOption.g_intWantPackage & (1 << (ParentArrayGroup - 2))) == 0))
                                                            {
                                                                continue;
                                                            }

                                                            if (strThirdChildName.Contains("颜色") &&
                                                    ((m_smCustomOption.g_intUseColorCamera & (1 << (ParentArrayGroup - 2))) == 0))
                                                            {
                                                                continue;
                                                            }

                                                            TreeNode newThirdChild = new TreeNode(strThirdChildName);
                                                            newSecondChild.Nodes.Add(newThirdChild);
                                                            m_intThirdChildArrayGroup[ParentArrayGroup][FirstChildArrayGroup][SecondChildArrayGroup][ThirdChildArrayGroup] = intThirdChildGroup;
                                                            ThirdChildArrayGroup++;
                                                        }
                                                    }
                                                }
                                                SecondChildArrayGroup++;
                                            }
                                        }
                                    }

                                    FirstChildArrayGroup++;
                                }
                            }
                        }

                        ParentArrayGroup++;
                    }
                }
            }

            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void InitParentTree()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;

            int ParentArrayGroup = 0;
            int FirstChildArrayGroup = 0;
            int SecondChildArrayGroup = 0;
            int ThirdChildArrayGroup = 0;

            m_intParentArrayGroup = new int[20]; // 10 Vision + Top Menu
            m_intFirstChildArrayGroup = new int[20][];

            for (int x = 0; x < m_intFirstChildArrayGroup.Length; x++)
            {
                m_intFirstChildArrayGroup[x] = new int[20];
            }

            m_intSecondChildArrayGroup = new int[20][][];
            for (int x = 0; x < m_intFirstChildArrayGroup.Length; x++)
            {
                m_intSecondChildArrayGroup[x] = new int[20][];
                for (int y = 0; y < m_intSecondChildArrayGroup[x].Length; y++)
                {
                    m_intSecondChildArrayGroup[x][y] = new int[20];
                }
            }

            m_intThirdChildArrayGroup = new int[20][][][];
            for (int x = 0; x < m_intSecondChildArrayGroup.Length; x++)
            {
                m_intThirdChildArrayGroup[x] = new int[20][][];
                for (int y = 0; y < m_intThirdChildArrayGroup[x].Length; y++)
                {
                    m_intThirdChildArrayGroup[x][y] = new int[20][];
                    for (int z = 0; z < m_intThirdChildArrayGroup[x][y].Length; z++)
                    {
                        m_intThirdChildArrayGroup[x][y][z] = new int[100];
                    }
                }
            }

            try
            {
                string sort = "Number";
                string filter = "Number > 0";
                DataRow[] ParentList = m_dsParent.Tables["Parent"].Select(filter, sort);
                foreach (DataRow parent in ParentList)
                {
                    string strParentName = parent["Name"].ToString();
                    string strFirstChild = parent["Child1"].ToString();
                    int intParentNum = Convert.ToInt32(parent["Number"]);
                    int intParentGroup = Convert.ToInt32(parent["Group"]);

                    if (m_intGroup <= intParentGroup && strParentName != " ")
                    {
                        TreeNode newParent = new TreeNode(strParentName);
                        tre_UserRight.Nodes.Add(newParent);
                       

                        m_intParentArrayGroup[ParentArrayGroup] = intParentGroup;
                        FirstChildArrayGroup = 0;

                        if (strFirstChild != "")
                        {
                            sort = "Number";
                            filter = "[Parent Number] = " + intParentNum;
                            DataRow[] FirstChildList = GetChildDataRow(strParentName, filter, sort, 1);
                            foreach (DataRow Firstchild in FirstChildList)
                            {
                                string strFirstChildName = Firstchild["Name"].ToString();
                                string strSecondChild = Firstchild["Child2"].ToString();
                                int intFirstChildNo = Convert.ToInt32(Firstchild["Number"]);
                                int intFirstChildGroup = Convert.ToInt32(Firstchild["Group"]);

                                if (m_intGroup <= intFirstChildGroup)
                                {
                                    TreeNode newFirstChild = new TreeNode(strFirstChildName);
                                    newParent.Nodes.Add(newFirstChild);
                                    m_intFirstChildArrayGroup[ParentArrayGroup][FirstChildArrayGroup] = intFirstChildGroup;
                                    SecondChildArrayGroup = 0;
                                    if (strSecondChild != "")
                                    {
                                        sort = "Number";
                                        filter = "[Child1 Number] = " + intFirstChildNo;
                                        DataRow[] SecondChildList = GetChildDataRow(strParentName, filter, sort, 2);
                                        foreach (DataRow SecondChild in SecondChildList)
                                        {

                                            string strSecondChildName = SecondChild["Name"].ToString();
                                            string strThirdChild = SecondChild["Child3"].ToString();
                                            int intSecondChildNo = Convert.ToInt32(SecondChild["Number"]);
                                            int intSecondChildGroup = Convert.ToInt32(SecondChild["Group"]);

                                            if (tre_UserRight.Nodes.Count > 2 && (strFirstChildName == "Tolerance" || strFirstChildName == "Option"))
                                            {
                                                if (strSecondChildName.Contains("Orient") && ((m_smCustomOption.g_intWantOrient & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                                if (strSecondChildName.Contains("Orient") && ((m_smCustomOption.g_intWantOrient0Deg & (1 << (ParentArrayGroup - 2))) > 0))
                                                {
                                                    continue;
                                                }
                                                if (strSecondChildName.Contains("Mark") && ((m_smCustomOption.g_intWantMark & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                                if (strSecondChildName.Contains("Package") && ((m_smCustomOption.g_intWantPackage & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                                if (strSecondChildName.Contains("Color") && ((m_smCustomOption.g_intUseColorCamera & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                                if (strSecondChildName.Contains("Pad") &&
                                                    ((m_smCustomOption.g_intWantPad5S & (1 << (ParentArrayGroup - 2))) == 0) &&
                                                    ((m_smCustomOption.g_intWantPad & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                                if ((strSecondChildName.Contains("Pin 1") || strSecondChildName.Contains("PH") || strSecondChildName.Contains("Position")) && ((m_smCustomOption.g_intWantBottom & (1 << (ParentArrayGroup - 2))) > 0))
                                                {
                                                    continue;
                                                }

                                                if (strSecondChildName.Contains("Lead") &&
                                                  ((m_smCustomOption.g_intWantLead3D & (1 << (ParentArrayGroup - 2))) == 0) &&
                                                  ((m_smCustomOption.g_intWantLead & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                             
                                                if (strSecondChildName.Contains("Seal") && ((m_smCustomOption.g_intWantSeal & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                                if (strSecondChildName.Contains("Barcode") && ((m_smCustomOption.g_intWantBarcode & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                            }
                                            else if (tre_UserRight.Nodes.Count > 2 && (strFirstChildName == "Tol.Pad"))
                                            {

                                                if (strSecondChildName.Contains("Pad") &&
                                                    ((m_smCustomOption.g_intWantPad5S & (1 << (ParentArrayGroup - 2))) == 0) &&
                                                    ((m_smCustomOption.g_intWantPad & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                            }
                                            else if (tre_UserRight.Nodes.Count > 2 && (strFirstChildName == "Tol.Lead3D"))
                                            {

                                                if (strSecondChildName.Contains("Lead3D") &&
                                                    ((m_smCustomOption.g_intWantLead3D & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                            }
                                            else if(tre_UserRight.Nodes.Count > 2 && (strFirstChildName == "Pad"))
                                            {

                                                if (strSecondChildName.Contains("Orient") &&
                                                    ((m_smCustomOption.g_intWantOrient & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                                if (strSecondChildName.Contains("Color") &&
                                                   ((m_smCustomOption.g_intUseColorCamera & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                            }
                                            else if (tre_UserRight.Nodes.Count > 2 && (strFirstChildName == "Package"))
                                            {
                                                if (strSecondChildName.Contains("Color") &&
                                                   ((m_smCustomOption.g_intUseColorCamera & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }
                                            }
                                            else if (tre_UserRight.Nodes.Count > 2 && (strFirstChildName == "System"))
                                            {

                                                if (strSecondChildName.Contains("Orient") &&
                                                    ((m_smCustomOption.g_intWantOrient & (1 << (ParentArrayGroup - 2))) == 0))
                                                {
                                                    continue;
                                                }

                                            }

                                            if (m_intGroup <= intSecondChildGroup)
                                            {
                                                TreeNode newSecondChild = new TreeNode(strSecondChildName);
                                                newFirstChild.Nodes.Add(newSecondChild);
                                                m_intSecondChildArrayGroup[ParentArrayGroup][FirstChildArrayGroup][SecondChildArrayGroup] = intSecondChildGroup;
                                                ThirdChildArrayGroup = 0;
                                                if (strThirdChild != "")
                                                {
                                                    sort = "Number";
                                                    filter = "[Child2 Number] = " + intSecondChildNo;
                                                    DataRow[] ThirdChildList = GetChildDataRow(strParentName, filter, sort, 3);
                                                    foreach (DataRow ThirdChild in ThirdChildList)
                                                    {

                                                        string strThirdChildName = ThirdChild["Name"].ToString();
                                                        int intThirdChildGroup = Convert.ToInt32(ThirdChild["Group"]);
                                                        if (m_intGroup <= intThirdChildGroup)
                                                        {
                                                            if ((newParent.Text == "MarkPkg" || newParent.Text == "Mark") && strThirdChildName == "Orient Direction")
                                                                continue;

                                                            if (!(newParent.Text == "MOLiPkg" || newParent.Text == "MOLi" || newParent.Text == "IPMLi" || newParent.Text == "IPMLiPkg") 
                                                                && (strThirdChildName == "Mark Use Lead Point Offset Mark ROI"
                                                                || strThirdChildName == "Mark ROI Lead Base Point Offset Setting"
                                                                || strThirdChildName == "Lead Dont Care Inward Tolerance Setting"))
                                                                continue;

                                                            if (!(newParent.Text.Contains("InPocket") || newParent.Text.Contains("IPM"))
                                                                && (strThirdChildName == "Use Unit Pattern As Mark Pattern"
                                                                || strThirdChildName == "Inspect Base Lead"
                                                                || strThirdChildName == "Base Lead Image View No"
                                                                || strThirdChildName == "Base Lead Setting"
                                                                || strThirdChildName == "Base Lead Threshold"
                                                                || strThirdChildName == "Base Lead Object Selection"))
                                                                continue;

                                                            if ((newParent.Text.Contains("Pad") && !newParent.Text.Contains("Orient")) && strThirdChildName == "Pad Orient Direction")
                                                                continue;

                                                            if (newParent.Text == "Li3DPkg" && strThirdChildName == "Lead3D Package ROI Setting")
                                                                continue;

                                                            if (strThirdChildName.Contains("Package") && newParent.Text.Contains("Orient") &&
                                                    ((m_smCustomOption.g_intWantPackage & (1 << (ParentArrayGroup - 2))) == 0))
                                                            {
                                                                continue;
                                                            }

                                                            if (strThirdChildName.Contains("Color") &&
                                                    ((m_smCustomOption.g_intUseColorCamera & (1 << (ParentArrayGroup - 2))) == 0))
                                                            {
                                                                continue;
                                                            }

                                                            TreeNode newThirdChild = new TreeNode(strThirdChildName);
                                                            newSecondChild.Nodes.Add(newThirdChild);
                                                            m_intThirdChildArrayGroup[ParentArrayGroup][FirstChildArrayGroup][SecondChildArrayGroup][ThirdChildArrayGroup] = intThirdChildGroup;
                                                            ThirdChildArrayGroup++;
                                                        }
                                                    }
                                                }
                                                SecondChildArrayGroup++;
                                            }
                                        }
                                    }

                                    FirstChildArrayGroup++;
                                }
                            }
                        }

                        ParentArrayGroup++;
                    }
                }
            }

            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateRightTree()
        {
            for (int i = 0; i < tre_UserRight.Nodes.Count; i++)
            {
                tre_UserRight.Nodes[i].Checked = UpdateCheckBox(tre_UserRight.Nodes[i]);
                for (int j = 0; j < tre_UserRight.Nodes[i].Nodes.Count; j++)
                {
                    tre_UserRight.Nodes[i].Nodes[j].Checked = UpdateCheckBox(tre_UserRight.Nodes[i].Nodes[j]);
                    for (int k = 0; k < tre_UserRight.Nodes[i].Nodes[j].Nodes.Count; k++)
                    {
                        tre_UserRight.Nodes[i].Nodes[j].Nodes[k].Checked = UpdateCheckBox(tre_UserRight.Nodes[i].Nodes[j].Nodes[k]);
                        for (int l = 0; l < tre_UserRight.Nodes[i].Nodes[j].Nodes[k].Nodes.Count; l++)
                        {
                            tre_UserRight.Nodes[i].Nodes[j].Nodes[k].Nodes[l].Checked = UpdateCheckBox(tre_UserRight.Nodes[i].Nodes[j].Nodes[k].Nodes[l]);

                        }
                    }
                }
            }
            m_blnInitDone = true;
        }

        private bool UpdateCheckBox(TreeNode CurrentNode)
        {
            int group = 0, selectedNodeIndex = 0, GrandGrandParentNodeIndex = 0, GrandParentNodeIndex = 0, ParentNodeIndex = 0;
            TreeNode selectedNode, GrandGrandParentNode, GrandParentNode, ParentNode;

            selectedNode = CurrentNode;//e.Node;
            selectedNodeIndex = selectedNode.Index;
            ParentNode = selectedNode.Parent;
            if (ParentNode != null)
            {
                ParentNodeIndex = ParentNode.Index;
                GrandParentNode = ParentNode.Parent;
                if (GrandParentNode != null)
                {
                    GrandParentNodeIndex = GrandParentNode.Index;
                    GrandGrandParentNode = GrandParentNode.Parent;
                    if (GrandGrandParentNode != null)
                    {
                        GrandGrandParentNodeIndex = GrandGrandParentNode.Index;
                        group = m_intThirdChildArrayGroup[GrandGrandParentNodeIndex][GrandParentNodeIndex][ParentNodeIndex][selectedNodeIndex];
                    }
                    else
                        group = m_intSecondChildArrayGroup[GrandParentNodeIndex][ParentNodeIndex][selectedNodeIndex];
                }
                else
                    group = m_intFirstChildArrayGroup[ParentNodeIndex][selectedNodeIndex];
            }
            else
                group = m_intParentArrayGroup[selectedNodeIndex];

            if (m_intSelectedGroup <= group)
                return true;
            else
                return false;  
        }

        private void UserRightTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //UpdateRadioButton(e.Node); // 2020-04-22 ZJYEOH : Move to Node Mouse Click Event
                                         // Cannot put here because this event will not triggered if click one of the child node and then click back its top parent
        }

        private void UserRightTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (!m_blnUserSelect)
            {
                e.Cancel = true;
                m_blnUserSelect = true;
            }
        }

        private void UserRightTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.IsExpanded)
                e.Node.Collapse();
            else
                e.Node.Expand();

            // 2020-05-05 ZJYEOH : Enabled back when node selected
            radio_SRM.Enabled = true;
            radio_Admin.Enabled = true;
            radio_Engineer.Enabled = true;
            radio_Technician.Enabled = true;
            radio_Operator.Enabled = true;

            UpdateRadioButton(e.Node);
        }

        private void UpdateRadioButton(TreeNode ENode)
        {
            int group = 0, selectedNodeIndex = 0, GrandGrandParentNodeIndex = 0, GrandParentNodeIndex = 0, ParentNodeIndex = 0;
            TreeNode selectedNode, GrandGrandParentNode, GrandParentNode, ParentNode;

            selectedNode = ENode; // e.Node;
            selectedNodeIndex = selectedNode.Index;
            ParentNode = selectedNode.Parent;
            if (ParentNode != null)
            {
                ParentNodeIndex = ParentNode.Index;
                GrandParentNode = ParentNode.Parent;
                if (GrandParentNode != null)
                {
                    GrandParentNodeIndex = GrandParentNode.Index;
                    GrandGrandParentNode = GrandParentNode.Parent;
                    if (GrandGrandParentNode != null)
                    {
                        GrandGrandParentNodeIndex = GrandGrandParentNode.Index;
                        group = m_intThirdChildArrayGroup[GrandGrandParentNodeIndex][GrandParentNodeIndex][ParentNodeIndex][selectedNodeIndex];
                    }
                    else
                        group = m_intSecondChildArrayGroup[GrandParentNodeIndex][ParentNodeIndex][selectedNodeIndex];
                }
                else
                    group = m_intFirstChildArrayGroup[ParentNodeIndex][selectedNodeIndex];
            }
            else
                group = m_intParentArrayGroup[selectedNodeIndex];

            switch (group)
            {
                case 1:
                    radio_SRM.Checked = true;
                    m_strSelectedRadio = GetUserGroupName(1);
                    break;
                case 2:
                    radio_Admin.Checked = true;
                    m_strSelectedRadio = GetUserGroupName(2);
                    break;
                case 3:
                    radio_Engineer.Checked = true;
                    m_strSelectedRadio = GetUserGroupName(3);
                    break;
                case 4:
                    radio_Technician.Checked = true;
                    m_strSelectedRadio = GetUserGroupName(4);
                    break;
                case 5:
                    radio_Operator.Checked = true;
                    m_strSelectedRadio = GetUserGroupName(5);
                    break;
            }
        }

        private void SRMRadioButton_Click(object sender, EventArgs e)
        {
            if (m_smCustomOption.g_intLanguageCulture == 2)
                ChangeGroup_CH(1);
            else
                ChangeGroup(1);
            m_blnInitDone = false;
            UpdateRightTree();
        }

        private void AdminRadioButton_Click(object sender, EventArgs e)
        {
            if (m_smCustomOption.g_intLanguageCulture == 2)
                ChangeGroup_CH(2);
            else
                ChangeGroup(2);
            m_blnInitDone = false;
            UpdateRightTree();
        }

        private void EngRadioButton_Click(object sender, EventArgs e)
        {
            if (m_smCustomOption.g_intLanguageCulture == 2)
                ChangeGroup_CH(3);
            else
                ChangeGroup(3);
            m_blnInitDone = false;
            UpdateRightTree();
        }

        private void TechRadioButton_Click(object sender, EventArgs e)
        {
            if (m_smCustomOption.g_intLanguageCulture == 2)
                ChangeGroup_CH(4);
            else
                ChangeGroup(4);
            m_blnInitDone = false;
            UpdateRightTree();
        }

        private void OpRadioButton_Click(object sender, EventArgs e)
        {
            if (m_smCustomOption.g_intLanguageCulture == 2)
                ChangeGroup_CH(5);
            else
                ChangeGroup(5);
            m_blnInitDone = false;
            UpdateRightTree();
        }
        
        private void IndividualUserRightForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }

        private void GetUserInfo()
        {

            if (m_intGroup <= 1)
                GroupComboBox.Items.Add("1. SRM");
            if (m_intGroup <= 2)
            {
                if (m_smCustomOption.g_intLanguageCulture == 2)
                    GroupComboBox.Items.Add("2. 管理员");
                else
                    GroupComboBox.Items.Add("2. Administrator");
            }
            if (m_intGroup <= 3)
            {
                if (m_smCustomOption.g_intLanguageCulture == 2)
                    GroupComboBox.Items.Add("3. 工程师");
                else
                    GroupComboBox.Items.Add("3. Engineer");
            }
            if (m_intGroup <= 4)
            {
                if (m_smCustomOption.g_intLanguageCulture == 2)
                    GroupComboBox.Items.Add("4. 技术员");
                else
                    GroupComboBox.Items.Add("4. Technician");
            }
            if (m_intGroup <= 5)
            {
                if (m_smCustomOption.g_intLanguageCulture == 2)
                    GroupComboBox.Items.Add("5. 操作员");
                else
                    GroupComboBox.Items.Add("5. Operator");
            }

            switch (m_intGroup)
            {
                case 1:
                    if (m_smCustomOption.g_intLanguageCulture == 2)
                        GroupComboBox.SelectedItem = "1. SRM";
                    else
                        GroupComboBox.SelectedItem = "1. SRM";
                    break;
                case 2:
                    if (m_smCustomOption.g_intLanguageCulture == 2)
                        GroupComboBox.SelectedItem = "2. 管理员";
                    else
                        GroupComboBox.SelectedItem = "2. Administrator";
                    break;
                case 3:
                    if (m_smCustomOption.g_intLanguageCulture == 2)
                        GroupComboBox.SelectedItem = "3. 工程师";
                    else
                        GroupComboBox.SelectedItem = "3. Engineer";
                    break;
                case 4:
                    if (m_smCustomOption.g_intLanguageCulture == 2)
                        GroupComboBox.SelectedItem = "4. 技术员";
                    else
                        GroupComboBox.SelectedItem = "4. Technician";
                    break;
                case 5:
                    if (m_smCustomOption.g_intLanguageCulture == 2)
                        GroupComboBox.SelectedItem = "5. 操作员";
                    else
                        GroupComboBox.SelectedItem = "5. Operator";
                    break;
            }
        }

        private void GroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            
            m_blnInitDone = false;

            switch (GroupComboBox.SelectedItem.ToString())
            {
                case "1. SRM":
                    m_intSelectedGroup = 1;
                    UpdateRightTree();
                    //ChangeGroup(1);
                    break;
                case "2. Administrator":
                case "2. 管理员":
                    m_intSelectedGroup = 2;
                    UpdateRightTree();
                    //ChangeGroup(2);
                    break;
                case "3. Engineer":
                case "3. 工程师":
                    m_intSelectedGroup = 3;
                    UpdateRightTree();
                    //ChangeGroup(3);
                    break;
                case "4. Technician":
                case "4. 技术员":
                    m_intSelectedGroup = 4;
                    UpdateRightTree();
                    //ChangeGroup(4);
                    break;
                case "5. Operator":
                case "5. 操作员":
                    m_intSelectedGroup = 5;
                    UpdateRightTree();
                    //ChangeGroup(5);
                    break;
            }

        }

        private void tre_UserRight_AfterCheck(object sender, TreeViewEventArgs e)
        {

            //if (e.Action == TreeViewAction.ByMouse)
            //    e.Node.Checked = !e.Node.Checked;

            //if (!m_blnInitDone)
            //    return;

                //tre_UserRight.SelectedNode = e.Node;

                //switch (GroupComboBox.SelectedItem.ToString())
                //{
                //    case "SRM":
                //        m_intSelectedGroup = 1;
                //        //UpdateRightTree();
                //        if (!tre_UserRight.SelectedNode.Checked)
                //        {
                //            ChangeGroup(1);
                //        }
                //        else
                //        {
                //            ChangeGroup(1);
                //        }
                //        break;
                //    case "Administrator":
                //        m_intSelectedGroup = 2;
                //        //UpdateRightTree();
                //        if (!tre_UserRight.SelectedNode.Checked)
                //        {
                //            ChangeGroup(1);
                //        }
                //        else
                //        {
                //            ChangeGroup(2);
                //        }
                //        break;
                //    case "Engineer":
                //        m_intSelectedGroup = 3;
                //        //UpdateRightTree();
                //        if (!tre_UserRight.SelectedNode.Checked)
                //        {
                //            ChangeGroup(2);
                //        }
                //        else
                //        {
                //            ChangeGroup(3);
                //        }
                //        break;
                //    case "Technician":
                //        m_intSelectedGroup = 4;
                //        //UpdateRightTree();
                //        if (!tre_UserRight.SelectedNode.Checked)
                //        {
                //            ChangeGroup(3);
                //        }
                //        else
                //        {
                //            ChangeGroup(4);
                //        }
                //        break;
                //    case "Operator":
                //        m_intSelectedGroup = 5;
                //        //UpdateRightTree();
                //        if (!tre_UserRight.SelectedNode.Checked)
                //        {
                //            ChangeGroup(4);
                //        }
                //        else
                //        {
                //            ChangeGroup(5);
                //        }
                //        break;
                //}
        }

        private void tre_UserRight_BeforeCheck(object sender, TreeViewCancelEventArgs e)
        {
            //if (!m_blnInitDone)
            //    return;
            //else
            //{ }

            if (e.Action == TreeViewAction.ByMouse)
                e.Cancel = true;
        

            //if (m_intGroup == m_intSelectedGroup)
            //{
            //    SRMMessageBox.Show("Cannot change access for same user level");
            //    e.Cancel = true;
            //}
        }

        private void UpdateTable(string ParentName, int ChildIndex)
        {
            switch (ParentName)
            {
                case "Top Menu":
                case "顶部菜单":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_TopMenu.Update(m_dsFirstChild_TopMenu, "Child1_TopMenu");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_TopMenu.Update(m_dsSecondChild_TopMenu, "Child2_TopMenu");
                    break;
                case "Bottom Menu":
                case "底部菜单":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_BottomMenu.Update(m_dsFirstChild_BottomMenu, "Child1_BottomMenu");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_BottomMenu.Update(m_dsSecondChild_BottomMenu, "Child2_BottomMenu");
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Orient":
                case "BottomOrient":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_Orientation.Update(m_dsFirstChild_Orientation, "Child1_Orientation");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_Orientation.Update(m_dsSecondChild_Orientation, "Child2_Orientation");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_Orientation.Update(m_dsThirdChild_Orientation, "Child3_Orientation");
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "Package":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_MarkOrient.Update(m_dsFirstChild_MarkOrient, "Child1_MarkOrient");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_MarkOrient.Update(m_dsSecondChild_MarkOrient, "Child2_MarkOrient");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_MarkOrient.Update(m_dsThirdChild_MarkOrient, "Child3_MarkOrient");
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_InPocket.Update(m_dsFirstChild_InPocket, "Child1_InPocket");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_InPocket.Update(m_dsSecondChild_InPocket, "Child2_InPocket");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_InPocket.Update(m_dsThirdChild_InPocket, "Child3_InPocket");
                    break;
                case "IPMLi2":
                case "IPMLiPkg2":
                case "InPocket2":
                case "InPocketPkg2":
                case "InPocketPkgPos2":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_InPocket2.Update(m_dsFirstChild_InPocket2, "Child1_InPocket2");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_InPocket2.Update(m_dsSecondChild_InPocket2, "Child2_InPocket2");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_InPocket2.Update(m_dsThirdChild_InPocket2, "Child3_InPocket2");
                    break;
                //case "BottomPosition":
                //case "BottomPositionOrient":
                //    break;
                //case "TapePocketPosition":
                //    break;
                //case "BottomOrientPad":
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_Pad.Update(m_dsFirstChild_Pad, "Child1_Pad");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_Pad.Update(m_dsSecondChild_Pad, "Child2_Pad");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_Pad.Update(m_dsThirdChild_Pad, "Child3_Pad");
                    break;
                case "Li3D":
                case "Li3DPkg":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_Lead3D.Update(m_dsFirstChild_Lead3D, "Child1_Lead3D");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_Lead3D.Update(m_dsSecondChild_Lead3D, "Child2_Lead3D");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_Lead3D.Update(m_dsThirdChild_Lead3D, "Child3_Lead3D");
                    break;
                case "Seal":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_Seal.Update(m_dsFirstChild_Seal, "Child1_Seal");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_Seal.Update(m_dsSecondChild_Seal, "Child2_Seal");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_Seal.Update(m_dsThirdChild_Seal, "Child3_Seal");
                    break;
                case "Seal2":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_Seal2.Update(m_dsFirstChild_Seal2, "Child1_Seal2");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_Seal2.Update(m_dsSecondChild_Seal2, "Child2_Seal2");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_Seal2.Update(m_dsThirdChild_Seal2, "Child3_Seal2");
                    break;
                case "Barcode":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_Barcode.Update(m_dsFirstChild_Barcode, "Child1_Barcode");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_Barcode.Update(m_dsSecondChild_Barcode, "Child2_Barcode");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_Barcode.Update(m_dsThirdChild_Barcode, "Child3_Barcode");
                    break;
                    //default:

                    //    break;
            }
            
        }

        //private void tre_UserRight_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        //{
        //    m_blnInitDone = false;
        //    UpdateRightTreeWithChecking();
        //}

        //private void tre_UserRight_MouseDoubleClick(object sender, MouseEventArgs e)
        //{
        //    m_blnInitDone = false;
        //    UpdateRightTreeWithChecking();
        //}

        //private void tre_UserRight_MouseClick(object sender, MouseEventArgs e)
        //{
        //    m_blnInitDone = false;
        //    UpdateRightTreeWithChecking();
        //}

        //private void tre_UserRight_MouseDown(object sender, MouseEventArgs e)
        //{
        //    m_blnInitDone = false;
        //    UpdateRightTreeWithChecking();
        //}

        //private void tre_UserRight_MouseUp(object sender, MouseEventArgs e)
        //{
        //    m_blnInitDone = false;
        //    UpdateRightTreeWithChecking();
        //}

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(m_blnInitDone)
                UpdateRightTree(); // 2020-04-21 ZJYEOH : Need to always update checkbox because got bug in Visual Studio where user can double click on checkbox to check or uncheck but no event triggered
        }

        private void UpdateRightTreeWithChecking()
        {
            for (int i = 0; i < tre_UserRight.Nodes.Count; i++)
            {
                if (tre_UserRight.Nodes[i].Checked != UpdateCheckBox(tre_UserRight.Nodes[i]))
                    tre_UserRight.Nodes[i].Checked = UpdateCheckBox(tre_UserRight.Nodes[i]);
                for (int j = 0; j < tre_UserRight.Nodes[i].Nodes.Count; j++)
                {
                    if (tre_UserRight.Nodes[i].Nodes[j].Checked != UpdateCheckBox(tre_UserRight.Nodes[i].Nodes[j]))
                        tre_UserRight.Nodes[i].Nodes[j].Checked = UpdateCheckBox(tre_UserRight.Nodes[i].Nodes[j]);
                    for (int k = 0; k < tre_UserRight.Nodes[i].Nodes[j].Nodes.Count; k++)
                    {
                        if (tre_UserRight.Nodes[i].Nodes[j].Nodes[k].Checked != UpdateCheckBox(tre_UserRight.Nodes[i].Nodes[j].Nodes[k]))
                            tre_UserRight.Nodes[i].Nodes[j].Nodes[k].Checked = UpdateCheckBox(tre_UserRight.Nodes[i].Nodes[j].Nodes[k]);
                        for (int l = 0; l < tre_UserRight.Nodes[i].Nodes[j].Nodes[k].Nodes.Count; l++)
                        {
                            if (tre_UserRight.Nodes[i].Nodes[j].Nodes[k].Nodes[l].Checked != UpdateCheckBox(tre_UserRight.Nodes[i].Nodes[j].Nodes[k].Nodes[l]))
                                tre_UserRight.Nodes[i].Nodes[j].Nodes[k].Nodes[l].Checked = UpdateCheckBox(tre_UserRight.Nodes[i].Nodes[j].Nodes[k].Nodes[l]);

                        }
                    }
                }
            }
        }

        private string GetUserGroupName(int intGroup)
        {
            switch (intGroup)
            {
                case 1:
                    if (m_smCustomOption.g_intLanguageCulture == 2)
                        return "SRM";
                    else
                        return "SRM";
                case 2:
                    if (m_smCustomOption.g_intLanguageCulture == 2)
                        return "管理员";
                    else
                        return "Administrator";
                case 3:
                    if (m_smCustomOption.g_intLanguageCulture == 2)
                        return "工程师, 管理员";
                    else
                        return "Engineer, Administrator";
                case 4:
                    if (m_smCustomOption.g_intLanguageCulture == 2)
                        return "技术员, 工程师, 管理员";
                    else
                        return "Technician, Engineer, Administrator";
                case 5:
                    if (m_smCustomOption.g_intLanguageCulture == 2)
                        return "操作员, 技术员, 工程师, 管理员";
                    else
                        return "Operator, Technician, Engineer, Administrator";
            }
            return "";
    }
    }
}
