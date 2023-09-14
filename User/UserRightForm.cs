using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using Common;
using SharedMemory;
namespace User
{
    public partial class UserRightForm : Form
    {
        #region Members Variables
        private int m_strSelectedRadio;
        private bool m_blnUserSelect = true;
        private int m_intGroup;
        private int[] m_intGrandParentArrayGroup;
        private int[][] m_intParentArrayGroup;
        private int[][][] m_intGroupList;
        private DataSet m_dsChild;
        private DataSet m_dsIO;
        private DataSet m_dsRight;
        private DataSet m_dsSetting;
        private OleDbDataAdapter m_childDataAdapter;
        private OleDbDataAdapter m_IODataAdapter;
        private OleDbDataAdapter m_rightDataAdapter;
        private OleDbDataAdapter m_settingDataAdapter;
        private OleDbParameter m_workParam;
        private ProductionInfo m_smProductionInfo;
        #endregion

        public UserRightForm(int group, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_intGroup = group;

            // Fill out the list view
            GetRightDataSet();
            UpdateRightTree();

            m_blnUserSelect = false;
            tre_UserRight.CollapseAll();

            if (m_intGroup > 1)
            {
                radio_SRM.Visible = false;
                lbl_SRM.Visible = false;
            }
        }

        public int UserGroup
        {
            get { return m_intGroup; }
        }

        

        private void ChangeGroup(int group)
        {
            if (tre_UserRight.SelectedNode == null)
                return;

            int selectedNodeIndex = 0, parentNodeIndex = 0, grandParentNodeIndex = 0;
            string selectedNodeText = "", parentNodeText = "", grandParentNodeText = " ";
            string filter = "";
            TreeNode selectedNode, parentNode, grandParentNode;
            DataRow[] rightList;
            DataRow right;

            selectedNode = tre_UserRight.SelectedNode;
            selectedNodeIndex = selectedNode.Index;
            selectedNodeText = selectedNode.Text;
            parentNode = selectedNode.Parent;

            if (parentNode != null)
            {
                parentNodeText = parentNode.Text;
                parentNodeIndex = parentNode.Index;
                grandParentNode = parentNode.Parent;

                if (grandParentNode != null)
                {
                    grandParentNodeText = grandParentNode.Text;
                    grandParentNodeIndex = grandParentNode.Index;

                    if (grandParentNodeText == "AutoMode")
                    {
                        filter = "Name = '" + selectedNodeText + "' AND [1Child Name] = '" + parentNodeText + "'";
                        rightList = m_dsSetting.Tables["Setting"].Select(filter);
                        right = rightList[0];
                        right["Name"] = selectedNodeText;
                        right["1Child Name"] = parentNodeText;
                        right["Group"] = group;

                        try
                        {
                            m_settingDataAdapter.Update(m_dsSetting, "Setting");
                            

                            STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio.ToString(), group.ToString(), m_smProductionInfo.g_strLotID);
                            
                        }
                        catch (Exception ex)
                        {
                            SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        m_intGroupList[grandParentNodeIndex][parentNodeIndex][selectedNodeIndex] = group;
                      
                    }
                    else if (grandParentNodeText == "I/O Trigger")
                    {
                        filter = "Description = '" + selectedNodeText + "' AND Module = '" + parentNodeText + "' AND Type = 'Output'";
                        rightList = m_dsIO.Tables["IO"].Select(filter);
                        right = rightList[0];
                        right["Description"] = selectedNodeText;
                        right["Module"] = parentNodeText;
                        right["Group"] = group;

                        try
                        {
                            m_IODataAdapter.Update(m_dsIO, "IO");
                            

                            STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio.ToString(), group.ToString(), m_smProductionInfo.g_strLotID);
                            
                        }
                        catch (Exception ex)
                        {
                            SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        m_intGroupList[grandParentNodeIndex][parentNodeIndex][selectedNodeIndex] = group;
                      
                    }
                }
                else
                {
                    filter = "Name = '" + selectedNodeText + "' AND [Parent] = '" + parentNodeText + "'";
                    rightList = m_dsChild.Tables["1Child"].Select(filter);
                    right = rightList[0];
                    right["Name"] = selectedNodeText;
                    right["Parent"] = parentNodeText;
                    right["Group"] = group;

                    try
                    {
                        m_childDataAdapter.Update(m_dsChild, "1Child");
                        

                        STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio.ToString(), group.ToString(), m_smProductionInfo.g_strLotID);
                        
                    }
                    catch (Exception ex)
                    {
                        SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    m_intParentArrayGroup[parentNodeIndex][selectedNodeIndex] = group;
                   
                }
            }
            else
            {
                filter = "[Name] = '" + selectedNodeText + "'";
                rightList = m_dsRight.Tables["Rights"].Select(filter);
                right = rightList[0];
                right["Name"] = selectedNodeText;
                right["Group"] = group;
                try
                {
                    m_rightDataAdapter.Update(m_dsRight, "Rights");
                    

                    STDeviceEdit.SaveDeviceEditLog("User", "user group value changed in " + selectedNodeText, m_strSelectedRadio.ToString(), group.ToString(), m_smProductionInfo.g_strLotID);
                    
                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                m_intGrandParentArrayGroup[selectedNodeIndex] = group;
            
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

            m_dsRight = new DataSet();
            m_dsRight.Tables.Add("Rights");
            m_dsChild = new DataSet();
            m_dsChild.Tables.Add("1Child");
            m_dsSetting = new DataSet();
            m_dsSetting.Tables.Add("Setting");

            accessCommand = new OleDbCommand("SELECT * FROM Rights", accessConn);
            m_rightDataAdapter = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM 1Child", accessConn);
            m_childDataAdapter = new OleDbDataAdapter(accessCommand);

            accessCommand = new OleDbCommand("SELECT * FROM Setting", accessConn);
            m_settingDataAdapter = new OleDbDataAdapter(accessCommand);

            sqlUpdate = "UPDATE Rights SET [Group] = @Group WHERE [Name] = @Name";
            m_rightDataAdapter.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_rightDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_rightDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE [1Child] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
            m_childDataAdapter.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_childDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_childDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
            m_workParam.SourceColumn = "Parent";
            m_workParam.SourceVersion = DataRowVersion.Current;

            sqlUpdate = "UPDATE Setting SET [Group] = @Group WHERE [Name] = @Name AND [1Child Name] = @ChildName";
            m_settingDataAdapter.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam = m_settingDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam.SourceColumn = "Group";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_settingDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam.SourceColumn = "Name";
            m_workParam.SourceVersion = DataRowVersion.Current;
            m_workParam = m_settingDataAdapter.UpdateCommand.Parameters.Add(new OleDbParameter("@ChildName", OleDbType.VarChar));
            m_workParam.SourceColumn = "1Child Name";
            m_workParam.SourceVersion = DataRowVersion.Current;

            try
            {
                m_rightDataAdapter.Fill(m_dsRight, "Rights");
                m_childDataAdapter.Fill(m_dsChild, "1Child");
                m_settingDataAdapter.Fill(m_dsSetting, "Setting");
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

        private void UpdateRightTree()
        {
            int arrayGroup = 0;
            int parentArrayGroup = 0;
            int grandParentArrayGroup = 0;

            m_intGrandParentArrayGroup = new int[11];
            m_intParentArrayGroup = new int[11][];

            for (int x = 0; x < m_intParentArrayGroup.Length; x++)
            {
                m_intParentArrayGroup[x] = new int[30];
            }

            m_intGroupList = new int[11][][];
            for (int y = 0; y < m_intParentArrayGroup.Length; y++)
            {
                m_intGroupList[y] = new int[30][];
                for (int z = 0; z < m_intGroupList[y].Length; z++)
                {
                    m_intGroupList[y][z] = new int[300];
                }
            }

            try
            {
                string sort = "Number";
                string filter = "Number > 0";
                DataRow[] rightList = m_dsRight.Tables["Rights"].Select(filter, sort);
                foreach (DataRow right in rightList)
                {
                    string rightName = right["Name"].ToString();
                    string rightChild = right["1Child"].ToString();
                    int parent = Convert.ToInt32(right["Number"]);
                    int parentGroup = Convert.ToInt32(right["Group"]);

                    if (m_intGroup <= parentGroup)
                    {
                        TreeNode newRight = new TreeNode(rightName);
                        tre_UserRight.Nodes.Add(newRight);

                        switch (rightName.ToLower().Trim())
                        {
                            case "automode":
                                newRight.ImageIndex = 0;
                                newRight.SelectedImageIndex = 0;
                                break;
                            case "deviceno":
                                newRight.ImageIndex = 1;
                                newRight.SelectedImageIndex = 1;
                                break;
                            case "i/o trigger":
                                newRight.ImageIndex = 2;
                                newRight.SelectedImageIndex = 2;
                                break;
                            case "i/o mapping":
                                newRight.ImageIndex = 3;
                                newRight.SelectedImageIndex = 3;
                                break;
                            case "user admin":
                                newRight.ImageIndex = 4;
                                newRight.SelectedImageIndex = 4;
                                break;
                            case "history":
                                newRight.ImageIndex = 5;
                                newRight.SelectedImageIndex = 5;
                                break;
                            case "language":
                                newRight.ImageIndex = 6;
                                newRight.SelectedImageIndex = 6;
                                break;               
                            case "configuration":
                                newRight.ImageIndex = 7;
                                newRight.SelectedImageIndex = 7;
                                break;
                            case "option":
                                newRight.ImageIndex = 8;
                                newRight.SelectedImageIndex = 8;
                                break;
                            case "exit":
                                newRight.ImageIndex = 9;
                                newRight.SelectedImageIndex = 9;
                                break;
                        }

                        m_intGrandParentArrayGroup[grandParentArrayGroup] = parentGroup;
                        parentArrayGroup = 0;

                        if (rightChild != "")
                        {
                            sort = "Number";
                            filter = "[Parent Number] = " + parent;
                            DataRow[] childList = m_dsChild.Tables["1Child"].Select(filter, sort);
                            foreach (DataRow child in childList)
                            {
                                 
                                string childName = child["Name"].ToString();
                               
                                string rightChild2 = child["2Child"].ToString();
                                int childNo = Convert.ToInt32(child["Number"]);
                                int childGroup = Convert.ToInt32(child["Group"]);

                                if (m_intGroup <= childGroup)
                                {
                                    TreeNode newChild = new TreeNode(childName);
                                    newRight.Nodes.Add(newChild);
                                    newChild.ImageIndex = 14;
                                    newChild.SelectedImageIndex = 14;
                                    m_intParentArrayGroup[grandParentArrayGroup][parentArrayGroup] = childGroup;
                                    arrayGroup = 0;
                                    if (childName == "Yield Page")
                                    {
                                        newChild.ImageIndex = 16;
                                        newChild.SelectedImageIndex = 16;
                                    }
                                    if (rightChild2 == "Setting")
                                    {
                                        sort = "Number";
                                        filter = "[1Child Number] = " + childNo;
                                        DataRow[] dr2Child = m_dsSetting.Tables["Setting"].Select(filter, sort);
                                        foreach (DataRow dr2 in dr2Child)
                                        {
                                          
                                            string str2Child = dr2["Name"].ToString();
                                            int n2ChildGroup = Convert.ToInt32(dr2["Group"]);
                                          
                                            if (m_intGroup <= n2ChildGroup)
                                            {
                                                TreeNode new2Child = new TreeNode(str2Child);
                                                newChild.Nodes.Add(new2Child);
                                                new2Child.ImageIndex = 13;
                                                new2Child.SelectedImageIndex = 13;
                                                m_intGroupList[grandParentArrayGroup][parentArrayGroup][arrayGroup] = n2ChildGroup;
                                                arrayGroup++;

                                                switch (childName.ToLower().Trim())
                                                {                                                  
                                                    case "setup page":       
                                                        newChild.ImageIndex = 10;
                                                        newChild.SelectedImageIndex = 10;
                                                        break;                                                 
                                                    case "test page":                                                   
                                                        newChild.ImageIndex = 11;
                                                        newChild.SelectedImageIndex = 11;
                                                        break;
                                                    case "ByPass Button":
                                                        newChild.ImageIndex = 15;
                                                        newChild.SelectedImageIndex = 15;
                                                        break;
                                                    default:
                                                        newChild.ImageIndex = 12;
                                                        newChild.SelectedImageIndex = 12;
                                                        break;
                                                }
                                            }
                                        }
                                    }

                                    parentArrayGroup++;
                                }
                            }
                        }

                        grandParentArrayGroup++;
                    }
                }
            }

            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void UserRightTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int group = 0, selectedNodeIndex = 0, parentNodeIndex = 0, grandParentNodeIndex = 0;
            TreeNode selectedNode, parentNode, grandParentNode;

            selectedNode = e.Node;
            selectedNodeIndex = selectedNode.Index;
            parentNode = selectedNode.Parent;
            if (parentNode != null)
            {
                parentNodeIndex = parentNode.Index;
                grandParentNode = parentNode.Parent;
                if (grandParentNode != null)
                {
                    grandParentNodeIndex = grandParentNode.Index;
                    group = m_intGroupList[grandParentNodeIndex][parentNodeIndex][selectedNodeIndex];
                }
                else
                    group = m_intParentArrayGroup[parentNodeIndex][selectedNodeIndex];
            }
            else
                group = m_intGrandParentArrayGroup[selectedNodeIndex];

            switch (group)
            {
                case 1:
                    radio_SRM.Checked = true;
                    m_strSelectedRadio = 1;
                    break;
                case 2:
                    radio_Admin.Checked = true;
                    m_strSelectedRadio =2;
                    break;
                case 3:
                    radio_Engineer.Checked = true;
                    m_strSelectedRadio = 3;
                    break;
                case 4:
                    radio_Technician.Checked = true;
                    m_strSelectedRadio = 4;
                    break;
                case 5:
                    radio_Operator.Checked = true;
                    m_strSelectedRadio = 5;
                    break;
            }
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
        }



        private void SRMRadioButton_Click(object sender, EventArgs e)
        {
            ChangeGroup(1);
        }

        private void AdminRadioButton_Click(object sender, EventArgs e)
        {
            ChangeGroup(2);
        }

        private void EngRadioButton_Click(object sender, EventArgs e)
        {
            ChangeGroup(3);
        }

        private void TechRadioButton_Click(object sender, EventArgs e)
        {
            ChangeGroup(4);
        }

        private void OpRadioButton_Click(object sender, EventArgs e)
        {
            ChangeGroup(5);
        }



        private void UserRightForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }


    }
}