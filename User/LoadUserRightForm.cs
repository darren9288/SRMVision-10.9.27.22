using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Common;
using System.Data.OleDb;

namespace User
{
    public partial class LoadUserRightForm : Form
    {
        #region Members Variables
        private string m_strSelectedRadio;
        private bool m_blnInitDone = false;
        private int m_intSelectedGroup;
        private bool m_blnUserSelect = true;
        private int m_intGroup;
        //private int[] m_intParentArrayGroup;
        //private int[][] m_intFirstChildArrayGroup;
        //private int[][][] m_intSecondChildArrayGroup;
        //private int[][][][] m_intThirdChildArrayGroup;
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
        //private ProductionInfo m_smProductionInfo;
        //private CustomOption m_smCustomOption;

        private DataSet m_dsFirstChild_Orientation_2;
        private DataSet m_dsFirstChild_MarkOrient_2;
        private DataSet m_dsFirstChild_Pad_2;
        private DataSet m_dsFirstChild_Lead3D_2;
        private DataSet m_dsFirstChild_InPocket_2;
        private DataSet m_dsFirstChild_InPocket2_2;
        private DataSet m_dsFirstChild_Seal_2;
        private DataSet m_dsFirstChild_Seal2_2;
        private DataSet m_dsFirstChild_Barcode_2;
        private DataSet m_dsFirstChild_TopMenu_2;
        private DataSet m_dsFirstChild_BottomMenu_2;
        private DataSet m_dsSecondChild_Orientation_2;
        private DataSet m_dsSecondChild_MarkOrient_2;
        private DataSet m_dsSecondChild_Pad_2;
        private DataSet m_dsSecondChild_Lead3D_2;
        private DataSet m_dsSecondChild_InPocket_2;
        private DataSet m_dsSecondChild_InPocket2_2;
        private DataSet m_dsSecondChild_Seal_2;
        private DataSet m_dsSecondChild_Seal2_2;
        private DataSet m_dsSecondChild_Barcode_2;
        private DataSet m_dsSecondChild_TopMenu_2;
        private DataSet m_dsSecondChild_BottomMenu_2;
        private DataSet m_dsThirdChild_Orientation_2;
        private DataSet m_dsThirdChild_MarkOrient_2;
        private DataSet m_dsThirdChild_Pad_2;
        private DataSet m_dsThirdChild_Lead3D_2;
        private DataSet m_dsThirdChild_InPocket_2;
        private DataSet m_dsThirdChild_InPocket2_2;
        private DataSet m_dsThirdChild_Seal_2;
        private DataSet m_dsThirdChild_Seal2_2;
        private DataSet m_dsThirdChild_Barcode_2;
        private DataSet m_dsIO_2;
        private DataSet m_dsParent_2;
        private DataSet m_dsSetting_2;
        private OleDbDataAdapter m_FirstChildDataAdapter_Orientation_2;
        private OleDbDataAdapter m_FirstChildDataAdapter_MarkOrient_2;
        private OleDbDataAdapter m_FirstChildDataAdapter_Pad_2;
        private OleDbDataAdapter m_FirstChildDataAdapter_Lead3D_2;
        private OleDbDataAdapter m_FirstChildDataAdapter_InPocket_2;
        private OleDbDataAdapter m_FirstChildDataAdapter_InPocket2_2;
        private OleDbDataAdapter m_FirstChildDataAdapter_Seal_2;
        private OleDbDataAdapter m_FirstChildDataAdapter_Seal2_2;
        private OleDbDataAdapter m_FirstChildDataAdapter_Barcode_2;
        private OleDbDataAdapter m_FirstChildDataAdapter_TopMenu_2;
        private OleDbDataAdapter m_FirstChildDataAdapter_BottomMenu_2;
        private OleDbDataAdapter m_SecondChildDataAdapter_Orientation_2;
        private OleDbDataAdapter m_SecondChildDataAdapter_MarkOrient_2;
        private OleDbDataAdapter m_SecondChildDataAdapter_Pad_2;
        private OleDbDataAdapter m_SecondChildDataAdapter_Lead3D_2;
        private OleDbDataAdapter m_SecondChildDataAdapter_InPocket_2;
        private OleDbDataAdapter m_SecondChildDataAdapter_InPocket2_2;
        private OleDbDataAdapter m_SecondChildDataAdapter_Seal_2;
        private OleDbDataAdapter m_SecondChildDataAdapter_Seal2_2;
        private OleDbDataAdapter m_SecondChildDataAdapter_Barcode_2;
        private OleDbDataAdapter m_SecondChildDataAdapter_TopMenu_2;
        private OleDbDataAdapter m_SecondChildDataAdapter_BottomMenu_2;
        private OleDbDataAdapter m_ThirdChildDataAdapter_Orientation_2;
        private OleDbDataAdapter m_ThirdChildDataAdapter_MarkOrient_2;
        private OleDbDataAdapter m_ThirdChildDataAdapter_Pad_2;
        private OleDbDataAdapter m_ThirdChildDataAdapter_Lead3D_2;
        private OleDbDataAdapter m_ThirdChildDataAdapter_InPocket_2;
        private OleDbDataAdapter m_ThirdChildDataAdapter_InPocket2_2;
        private OleDbDataAdapter m_ThirdChildDataAdapter_Seal_2;
        private OleDbDataAdapter m_ThirdChildDataAdapter_Seal2_2;
        private OleDbDataAdapter m_ThirdChildDataAdapter_Barcode_2;
        private OleDbDataAdapter m_IODataAdapter_2;
        private OleDbDataAdapter m_ParentDataAdapter_2;
        private OleDbParameter m_workParam_2;

        private SRMWaitingFormThread m_thWaitingFormThread;

        private bool m_blnOrientExist1 = false;
        private bool m_blnOrientExist2 = false;
        private bool m_blnOrientExist3 = false;
        private bool m_blnMarkExist1 = false;
        private bool m_blnMarkExist2 = false;
        private bool m_blnMarkExist3 = false;
        private bool m_blnInPocketExist1 = false;
        private bool m_blnInPocketExist2 = false;
        private bool m_blnInPocketExist3 = false;
        private bool m_blnInPocket2Exist1 = false;
        private bool m_blnInPocket2Exist2 = false;
        private bool m_blnInPocket2Exist3 = false;
        private bool m_blnPadExist1 = false;
        private bool m_blnPadExist2 = false;
        private bool m_blnPadExist3 = false;
        private bool m_blnLead3DExist1 = false;
        private bool m_blnLead3DExist2 = false;
        private bool m_blnLead3DExist3 = false;
        private bool m_blnSealExist1 = false;
        private bool m_blnSealExist2 = false;
        private bool m_blnSealExist3 = false;
        private bool m_blnSeal2Exist1 = false;
        private bool m_blnSeal2Exist2 = false;
        private bool m_blnSeal2Exist3 = false;
        private bool m_blnBarcodeExist1 = false;
        private bool m_blnBarcodeExist2 = false;
        private bool m_blnBarcodeExist3 = false;
        #endregion

        public LoadUserRightForm()
        {
            InitializeComponent();
        }
        private void StartWaiting(string StrMessage)
        {
            m_thWaitingFormThread = new SRMWaitingFormThread();
            m_thWaitingFormThread.SetStartSplash(StrMessage);
            this.Enabled = false;
        }

        private void StopWaiting()
        {
            m_thWaitingFormThread.SetStopSplash();
            this.Enabled = true;
        }

        private void btn_Browse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Database Files|*.mdb";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string strDirPath = openFileDialog1.FileName;
                if (strDirPath.Contains("Setting.mdb"))
                {
                    DirectoryInfo dir = new DirectoryInfo(openFileDialog1.FileName);
                    txt_SelectedPath.Text = dir.ToString();
                }
                else
                {
                    SRMMessageBox.Show("Please Select Setting.mdb File!");
                }
            }
            
        }

        private void txt_SelectedPath_DragDrop(object sender, DragEventArgs e)
        {
            //DirectoryInfo dir = new DirectoryInfo();
            //txt_SelectedPath.Text = dir.ToString();
            //if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            //{
            //    e.Effect = DragDropEffects.All;
            //}
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length != 0)
            {
                DirectoryInfo dir = new DirectoryInfo(files[0]);
                txt_SelectedPath.Text = dir.ToString();
            }
        }

        private void txt_SelectedPath_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //e.Effect = DragDropEffects.Copy;
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 1 || !files[0].Contains("Setting.mdb"))
                    e.Effect = DragDropEffects.None;
                else
                    e.Effect = DragDropEffects.Move;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        private void btn_Load_Click(object sender, EventArgs e)
        {
            if (txt_SelectedPath.Text == "" || !File.Exists(txt_SelectedPath.Text))
            {
                SRMMessageBox.Show("File not exist!");
                return;
            }

            StartWaiting("Loading User Right Setting...");
            GetRightDataSet();
            GetRightDataSet_2();
            InitParentTree2();
            StopWaiting();

            this.DialogResult = DialogResult.OK;
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
        private void GetRightDataSet_2()
        {
            string sqlUpdate = "";
            OleDbCommand accessCommand;
            OleDbConnection accessConn;

            accessConn = new OleDbConnection();
            accessConn.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                @"data source = " + txt_SelectedPath.Text;
            accessConn.Open();

            m_dsParent_2 = new DataSet();
            m_dsParent_2.Tables.Add("Parent");

            m_dsFirstChild_Orientation_2 = new DataSet();
            m_dsFirstChild_Orientation_2.Tables.Add("Child1_Orientation");
            m_dsFirstChild_MarkOrient_2 = new DataSet();
            m_dsFirstChild_MarkOrient_2.Tables.Add("Child1_MarkOrient");
            m_dsFirstChild_Pad_2 = new DataSet();
            m_dsFirstChild_Pad_2.Tables.Add("Child1_Pad");
            m_dsFirstChild_Lead3D_2 = new DataSet();
            m_dsFirstChild_Lead3D_2.Tables.Add("Child1_Lead3D");
            m_dsFirstChild_InPocket_2 = new DataSet();
            m_dsFirstChild_InPocket_2.Tables.Add("Child1_InPocket");
            m_dsFirstChild_InPocket2_2 = new DataSet();
            m_dsFirstChild_InPocket2_2.Tables.Add("Child1_InPocket2");
            m_dsFirstChild_Seal_2 = new DataSet();
            m_dsFirstChild_Seal_2.Tables.Add("Child1_Seal");
            m_dsFirstChild_Seal2_2 = new DataSet();
            m_dsFirstChild_Seal2_2.Tables.Add("Child1_Seal2");
            m_dsFirstChild_Barcode_2 = new DataSet();
            m_dsFirstChild_Barcode_2.Tables.Add("Child1_Barcode");
            m_dsFirstChild_TopMenu_2 = new DataSet();
            m_dsFirstChild_TopMenu_2.Tables.Add("Child1_TopMenu");
            m_dsFirstChild_BottomMenu_2 = new DataSet();
            m_dsFirstChild_BottomMenu_2.Tables.Add("Child1_BottomMenu");

            m_dsSecondChild_Orientation_2 = new DataSet();
            m_dsSecondChild_Orientation_2.Tables.Add("Child2_Orientation");
            m_dsSecondChild_MarkOrient_2 = new DataSet();
            m_dsSecondChild_MarkOrient_2.Tables.Add("Child2_MarkOrient");
            m_dsSecondChild_Pad_2 = new DataSet();
            m_dsSecondChild_Pad_2.Tables.Add("Child2_Pad");
            m_dsSecondChild_Lead3D_2 = new DataSet();
            m_dsSecondChild_Lead3D_2.Tables.Add("Child2_Lead3D");
            m_dsSecondChild_InPocket_2 = new DataSet();
            m_dsSecondChild_InPocket_2.Tables.Add("Child2_InPocket");
            m_dsSecondChild_InPocket2_2 = new DataSet();
            m_dsSecondChild_InPocket2_2.Tables.Add("Child2_InPocket2");
            m_dsSecondChild_Seal_2 = new DataSet();
            m_dsSecondChild_Seal_2.Tables.Add("Child2_Seal");
            m_dsSecondChild_Seal2_2 = new DataSet();
            m_dsSecondChild_Seal2_2.Tables.Add("Child2_Seal2");
            m_dsSecondChild_Barcode_2 = new DataSet();
            m_dsSecondChild_Barcode_2.Tables.Add("Child2_Barcode");
            m_dsSecondChild_TopMenu_2 = new DataSet();
            m_dsSecondChild_TopMenu_2.Tables.Add("Child2_TopMenu");
            m_dsSecondChild_BottomMenu_2 = new DataSet();
            m_dsSecondChild_BottomMenu_2.Tables.Add("Child2_BottomMenu");

            m_dsThirdChild_Orientation_2 = new DataSet();
            m_dsThirdChild_Orientation_2.Tables.Add("Child3_Orientation");
            m_dsThirdChild_MarkOrient_2 = new DataSet();
            m_dsThirdChild_MarkOrient_2.Tables.Add("Child3_MarkOrient");
            m_dsThirdChild_Pad_2 = new DataSet();
            m_dsThirdChild_Pad_2.Tables.Add("Child3_Pad");
            m_dsThirdChild_Lead3D_2 = new DataSet();
            m_dsThirdChild_Lead3D_2.Tables.Add("Child3_Lead3D");
            m_dsThirdChild_InPocket_2 = new DataSet();
            m_dsThirdChild_InPocket_2.Tables.Add("Child3_InPocket");
            m_dsThirdChild_InPocket2_2 = new DataSet();
            m_dsThirdChild_InPocket2_2.Tables.Add("Child3_InPocket2");
            m_dsThirdChild_Seal_2 = new DataSet();
            m_dsThirdChild_Seal_2.Tables.Add("Child3_Seal");
            m_dsThirdChild_Seal2_2 = new DataSet();
            m_dsThirdChild_Seal2_2.Tables.Add("Child3_Seal2");
            m_dsThirdChild_Barcode_2 = new DataSet();
            m_dsThirdChild_Barcode_2.Tables.Add("Child3_Barcode");

            accessCommand = new OleDbCommand("SELECT * FROM Parent", accessConn);
            m_ParentDataAdapter_2 = new OleDbDataAdapter(accessCommand);
            
            var schema = accessConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
         
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child1_Orientation".ToLower()))
            {
                m_blnOrientExist1 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child1_Orientation", accessConn);
                m_FirstChildDataAdapter_Orientation_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnOrientExist1 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child1_MarkOrient".ToLower()))
            {
                m_blnMarkExist1 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child1_MarkOrient", accessConn);
                m_FirstChildDataAdapter_MarkOrient_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnMarkExist1 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child1_Pad".ToLower()))
            {
                m_blnPadExist1 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child1_Pad", accessConn);
                m_FirstChildDataAdapter_Pad_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnPadExist1 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child1_Lead3D".ToLower()))
            {
                m_blnLead3DExist1 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child1_Lead3D", accessConn);
                m_FirstChildDataAdapter_Lead3D_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnLead3DExist1 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child1_InPocket".ToLower()))
            {
                m_blnInPocketExist1 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child1_InPocket", accessConn);
                m_FirstChildDataAdapter_InPocket_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnInPocketExist1 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child1_InPocket2".ToLower()))
            {
                m_blnInPocket2Exist1 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child1_InPocket2", accessConn);
                m_FirstChildDataAdapter_InPocket2_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnInPocket2Exist1 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child1_Seal".ToLower()))
            {
                m_blnSealExist1 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child1_Seal", accessConn);
                m_FirstChildDataAdapter_Seal_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnSealExist1 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child1_Seal2".ToLower()))
            {
                m_blnSeal2Exist1 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child1_Seal2", accessConn);
                m_FirstChildDataAdapter_Seal2_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnSeal2Exist1 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child1_Barcode".ToLower()))
            {
                m_blnBarcodeExist1 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child1_Barcode", accessConn);
                m_FirstChildDataAdapter_Barcode_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnBarcodeExist1 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child1_TopMenu".ToLower()))
            {
                accessCommand = new OleDbCommand("SELECT * FROM Child1_TopMenu", accessConn);
                m_FirstChildDataAdapter_TopMenu_2 = new OleDbDataAdapter(accessCommand);
            }
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child1_BottomMenu".ToLower()))
            {
                accessCommand = new OleDbCommand("SELECT * FROM Child1_BottomMenu", accessConn);
                m_FirstChildDataAdapter_BottomMenu_2 = new OleDbDataAdapter(accessCommand);
            }

            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child2_Orientation".ToLower()))
            {
                m_blnOrientExist2 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child2_Orientation", accessConn);
                m_SecondChildDataAdapter_Orientation_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnOrientExist2 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child2_MarkOrient".ToLower()))
            {
                m_blnMarkExist2 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child2_MarkOrient", accessConn);
                m_SecondChildDataAdapter_MarkOrient_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnMarkExist2 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child2_PadChild2_Pad".ToLower()))
            {
                m_blnPadExist2 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child2_Pad", accessConn);
                m_SecondChildDataAdapter_Pad_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnPadExist2 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child2_Lead3D".ToLower()))
            {
                m_blnLead3DExist2 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child2_Lead3D", accessConn);
                m_SecondChildDataAdapter_Lead3D_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnLead3DExist2 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child2_InPocket".ToLower()))
            {
                m_blnInPocketExist2 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child2_InPocket", accessConn);
                m_SecondChildDataAdapter_InPocket_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnInPocketExist2 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child2_InPocket2".ToLower()))
            {
                m_blnInPocket2Exist2 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child2_InPocket2", accessConn);
                m_SecondChildDataAdapter_InPocket2_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnInPocket2Exist2 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child2_Seal".ToLower()))
            {
                m_blnSealExist2 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child2_Seal", accessConn);
                m_SecondChildDataAdapter_Seal_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnSealExist2 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child2_Seal2".ToLower()))
            {
                m_blnSeal2Exist2 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child2_Seal2", accessConn);
                m_SecondChildDataAdapter_Seal2_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnSeal2Exist2 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child2_Barcode".ToLower()))
            {
                m_blnBarcodeExist2 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child2_Barcode", accessConn);
                m_SecondChildDataAdapter_Barcode_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnBarcodeExist2 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child2_TopMenu".ToLower()))
            {
                accessCommand = new OleDbCommand("SELECT * FROM Child2_TopMenu", accessConn);
                m_SecondChildDataAdapter_TopMenu_2 = new OleDbDataAdapter(accessCommand);
            }
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child2_BottomMenu".ToLower()))
            {
                accessCommand = new OleDbCommand("SELECT * FROM Child2_BottomMenu", accessConn);
                m_SecondChildDataAdapter_BottomMenu_2 = new OleDbDataAdapter(accessCommand);
            }

            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child3_Orientation".ToLower()))
            {
                m_blnOrientExist3 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child3_Orientation", accessConn);
                m_ThirdChildDataAdapter_Orientation_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnOrientExist3 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child3_MarkOrient".ToLower()))
            {
                m_blnMarkExist3 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child3_MarkOrient", accessConn);
                m_ThirdChildDataAdapter_MarkOrient_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnMarkExist3 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child3_Pad".ToLower()))
            {
                m_blnPadExist3 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child3_Pad", accessConn);
                m_ThirdChildDataAdapter_Pad_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnPadExist3 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child3_Lead3D".ToLower()))
            {
                m_blnLead3DExist3 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child3_Lead3D", accessConn);
                m_ThirdChildDataAdapter_Lead3D_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnLead3DExist3 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child3_InPocket".ToLower()))
            {
                m_blnInPocketExist3 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child3_InPocket", accessConn);
                m_ThirdChildDataAdapter_InPocket_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnInPocketExist3 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child3_InPocket2".ToLower()))
            {
                m_blnInPocket2Exist3 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child3_InPocket2", accessConn);
                m_ThirdChildDataAdapter_InPocket2_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnInPocket2Exist3 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child3_Seal".ToLower()))
            {
                m_blnSealExist3 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child3_Seal", accessConn);
                m_ThirdChildDataAdapter_Seal_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnSealExist3 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child3_Seal2".ToLower()))
            {
                m_blnSeal2Exist3 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child3_Seal2", accessConn);
                m_ThirdChildDataAdapter_Seal2_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnSeal2Exist3 = false;
            if (schema.Rows.OfType<DataRow>().Any(r => r.ItemArray[2].ToString().ToLower() == "Child3_Barcode".ToLower()))
            {
                m_blnBarcodeExist3 = true;
                accessCommand = new OleDbCommand("SELECT * FROM Child3_Barcode", accessConn);
                m_ThirdChildDataAdapter_Barcode_2 = new OleDbDataAdapter(accessCommand);
            }
            else
                m_blnBarcodeExist3 = false;

            sqlUpdate = "UPDATE Parent SET [Group] = @Group WHERE [Name] = @Name";
            m_ParentDataAdapter_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam_2 = m_ParentDataAdapter_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam_2.SourceColumn = "Group";
            m_workParam_2.SourceVersion = DataRowVersion.Current;
            m_workParam_2 = m_ParentDataAdapter_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
            m_workParam_2.SourceColumn = "Name";
            m_workParam_2.SourceVersion = DataRowVersion.Current;

            if (m_blnOrientExist1)
            {
                sqlUpdate = "UPDATE [Child1_Orientation] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
                m_FirstChildDataAdapter_Orientation_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_FirstChildDataAdapter_Orientation_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_Orientation_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_Orientation_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Parent";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnMarkExist1)
            {
                sqlUpdate = "UPDATE [Child1_MarkOrient] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
                m_FirstChildDataAdapter_MarkOrient_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_FirstChildDataAdapter_MarkOrient_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_MarkOrient_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_MarkOrient_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Parent";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnPadExist1)
            {
                sqlUpdate = "UPDATE [Child1_Pad] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
                m_FirstChildDataAdapter_Pad_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_FirstChildDataAdapter_Pad_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_Pad_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_Pad_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Parent";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnLead3DExist1)
            {
                sqlUpdate = "UPDATE [Child1_Lead3D] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
                m_FirstChildDataAdapter_Lead3D_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_FirstChildDataAdapter_Lead3D_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_Lead3D_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_Lead3D_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Parent";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnInPocketExist1)
            {
                sqlUpdate = "UPDATE [Child1_InPocket] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
                m_FirstChildDataAdapter_InPocket_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_FirstChildDataAdapter_InPocket_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_InPocket_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_InPocket_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Parent";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnInPocket2Exist1)
            {
                sqlUpdate = "UPDATE [Child1_InPocket2] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
                m_FirstChildDataAdapter_InPocket2_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_FirstChildDataAdapter_InPocket2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_InPocket2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_InPocket2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Parent";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnSealExist1)
            {
                sqlUpdate = "UPDATE [Child1_Seal] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
                m_FirstChildDataAdapter_Seal_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_FirstChildDataAdapter_Seal_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_Seal_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_Seal_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Parent";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnSeal2Exist1)
            {
                sqlUpdate = "UPDATE [Child1_Seal2] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
                m_FirstChildDataAdapter_Seal2_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_FirstChildDataAdapter_Seal2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_Seal2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_Seal2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Parent";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnBarcodeExist1)
            {
                sqlUpdate = "UPDATE [Child1_Barcode] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
                m_FirstChildDataAdapter_Barcode_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_FirstChildDataAdapter_Barcode_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_Barcode_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_Barcode_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Parent";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_FirstChildDataAdapter_TopMenu_2 != null)
            {
                sqlUpdate = "UPDATE [Child1_TopMenu] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
                m_FirstChildDataAdapter_TopMenu_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_FirstChildDataAdapter_TopMenu_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_TopMenu_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_TopMenu_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Parent";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_FirstChildDataAdapter_BottomMenu_2 != null)
            {
                sqlUpdate = "UPDATE [Child1_BottomMenu] SET [Group] = @Group WHERE [Name] = @Name AND [Parent] = @Parent";
                m_FirstChildDataAdapter_BottomMenu_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_FirstChildDataAdapter_BottomMenu_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_BottomMenu_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_FirstChildDataAdapter_BottomMenu_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Parent", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Parent";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnOrientExist2)
            {
                sqlUpdate = "UPDATE Child2_Orientation SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
                m_SecondChildDataAdapter_Orientation_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_SecondChildDataAdapter_Orientation_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_Orientation_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_Orientation_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnMarkExist2)
            {
                sqlUpdate = "UPDATE Child2_MarkOrient SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
                m_SecondChildDataAdapter_MarkOrient_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_SecondChildDataAdapter_MarkOrient_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_MarkOrient_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_MarkOrient_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnPadExist2)
            {
                sqlUpdate = "UPDATE Child2_Pad SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
                m_SecondChildDataAdapter_Pad_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_SecondChildDataAdapter_Pad_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_Pad_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_Pad_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnLead3DExist2)
            {
                sqlUpdate = "UPDATE Child2_Lead3D SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
                m_SecondChildDataAdapter_Lead3D_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_SecondChildDataAdapter_Lead3D_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_Lead3D_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_Lead3D_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnInPocketExist2)
            {
                sqlUpdate = "UPDATE Child2_InPocket SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
                m_SecondChildDataAdapter_InPocket_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_SecondChildDataAdapter_InPocket_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_InPocket_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_InPocket_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnInPocket2Exist2)
            {
                sqlUpdate = "UPDATE Child2_InPocket2 SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
                m_SecondChildDataAdapter_InPocket2_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_SecondChildDataAdapter_InPocket2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_InPocket2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_InPocket2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnSealExist2)
            {
                sqlUpdate = "UPDATE Child2_Seal SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
                m_SecondChildDataAdapter_Seal_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_SecondChildDataAdapter_Seal_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_Seal_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_Seal_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnSeal2Exist2)
            {
                sqlUpdate = "UPDATE Child2_Seal2 SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
                m_SecondChildDataAdapter_Seal2_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_SecondChildDataAdapter_Seal2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_Seal2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_Seal2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnBarcodeExist2)
            {
                sqlUpdate = "UPDATE Child2_Barcode SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
                m_SecondChildDataAdapter_Barcode_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_SecondChildDataAdapter_Barcode_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_Barcode_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_Barcode_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_SecondChildDataAdapter_TopMenu_2 != null)
            {
                sqlUpdate = "UPDATE Child2_TopMenu SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
                m_SecondChildDataAdapter_TopMenu_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_SecondChildDataAdapter_TopMenu_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_TopMenu_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_TopMenu_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_SecondChildDataAdapter_BottomMenu_2 != null)
            {
                sqlUpdate = "UPDATE Child2_BottomMenu SET [Group] = @Group WHERE [Name] = @Name AND [Child1] = @Child1";
                m_SecondChildDataAdapter_BottomMenu_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_SecondChildDataAdapter_BottomMenu_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_BottomMenu_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_SecondChildDataAdapter_BottomMenu_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnOrientExist3)
            {
                sqlUpdate = "UPDATE Child3_Orientation SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
                m_ThirdChildDataAdapter_Orientation_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_ThirdChildDataAdapter_Orientation_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Orientation_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Orientation_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child2";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Orientation_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnMarkExist3)
            {
                sqlUpdate = "UPDATE Child3_MarkOrient SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
                m_ThirdChildDataAdapter_MarkOrient_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_ThirdChildDataAdapter_MarkOrient_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_MarkOrient_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_MarkOrient_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child2";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_MarkOrient_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnPadExist3)
            {
                sqlUpdate = "UPDATE Child3_Pad SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
                m_ThirdChildDataAdapter_Pad_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_ThirdChildDataAdapter_Pad_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Pad_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Pad_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child2";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Pad_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnLead3DExist3)
            {
                sqlUpdate = "UPDATE Child3_Lead3D SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
                m_ThirdChildDataAdapter_Lead3D_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_ThirdChildDataAdapter_Lead3D_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Lead3D_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Lead3D_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child2";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Lead3D_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnInPocketExist3)
            {
                sqlUpdate = "UPDATE Child3_InPocket SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
                m_ThirdChildDataAdapter_InPocket_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_ThirdChildDataAdapter_InPocket_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_InPocket_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_InPocket_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child2";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_InPocket_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnInPocket2Exist3)
            {
                sqlUpdate = "UPDATE Child3_InPocket2 SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
                m_ThirdChildDataAdapter_InPocket2_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_ThirdChildDataAdapter_InPocket2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_InPocket2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_InPocket2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child2";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_InPocket2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnSealExist3)
            {
                sqlUpdate = "UPDATE Child3_Seal SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
                m_ThirdChildDataAdapter_Seal_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_ThirdChildDataAdapter_Seal_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Seal_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Seal_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child2";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Seal_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnSeal2Exist3)
            {
                sqlUpdate = "UPDATE Child3_Seal2 SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
                m_ThirdChildDataAdapter_Seal2_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_ThirdChildDataAdapter_Seal2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Seal2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Seal2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child2";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Seal2_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            if (m_blnBarcodeExist3)
            {
                sqlUpdate = "UPDATE Child3_Barcode SET [Group] = @Group WHERE [Name] = @Name AND [Child2] = @Child2 AND [Child1] = @Child1";
                m_ThirdChildDataAdapter_Barcode_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
                m_workParam_2 = m_ThirdChildDataAdapter_Barcode_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
                m_workParam_2.SourceColumn = "Group";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Barcode_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Name", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Name";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Barcode_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child2", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child2";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
                m_workParam_2 = m_ThirdChildDataAdapter_Barcode_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Child1", OleDbType.VarChar));
                m_workParam_2.SourceColumn = "Child1";
                m_workParam_2.SourceVersion = DataRowVersion.Current;
            }

            try
            {
                if (m_ParentDataAdapter_2 != null)
                    m_ParentDataAdapter_2.Fill(m_dsParent_2, "Parent");

                if (m_blnOrientExist1)
                    m_FirstChildDataAdapter_Orientation_2.Fill(m_dsFirstChild_Orientation_2, "Child1_Orientation");
                if (m_blnMarkExist1)
                    m_FirstChildDataAdapter_MarkOrient_2.Fill(m_dsFirstChild_MarkOrient_2, "Child1_MarkOrient");
                if (m_blnPadExist1)
                    m_FirstChildDataAdapter_Pad_2.Fill(m_dsFirstChild_Pad_2, "Child1_Pad");
                if (m_blnLead3DExist1)
                    m_FirstChildDataAdapter_Lead3D_2.Fill(m_dsFirstChild_Lead3D_2, "Child1_Lead3D");
                if (m_blnInPocketExist1)
                    m_FirstChildDataAdapter_InPocket_2.Fill(m_dsFirstChild_InPocket_2, "Child1_InPocket");
                if (m_blnInPocket2Exist1)
                    m_FirstChildDataAdapter_InPocket2_2.Fill(m_dsFirstChild_InPocket2_2, "Child1_InPocket2");
                if (m_blnSealExist1)
                    m_FirstChildDataAdapter_Seal_2.Fill(m_dsFirstChild_Seal_2, "Child1_Seal");
                if (m_blnSeal2Exist1)
                    m_FirstChildDataAdapter_Seal2_2.Fill(m_dsFirstChild_Seal2_2, "Child1_Seal2");
                if (m_blnBarcodeExist1)
                    m_FirstChildDataAdapter_Barcode_2.Fill(m_dsFirstChild_Barcode_2, "Child1_Barcode");
                if (m_FirstChildDataAdapter_TopMenu_2 != null)
                    m_FirstChildDataAdapter_TopMenu_2.Fill(m_dsFirstChild_TopMenu_2, "Child1_TopMenu");
                if (m_FirstChildDataAdapter_BottomMenu_2 != null)
                    m_FirstChildDataAdapter_BottomMenu_2.Fill(m_dsFirstChild_BottomMenu_2, "Child1_BottomMenu");

                if (m_blnOrientExist2)
                    m_SecondChildDataAdapter_Orientation_2.Fill(m_dsSecondChild_Orientation_2, "Child2_Orientation");
                if (m_blnMarkExist2)
                    m_SecondChildDataAdapter_MarkOrient_2.Fill(m_dsSecondChild_MarkOrient_2, "Child2_MarkOrient");
                if (m_blnPadExist2)
                    m_SecondChildDataAdapter_Pad_2.Fill(m_dsSecondChild_Pad_2, "Child2_Pad");
                if (m_blnLead3DExist2)
                    m_SecondChildDataAdapter_Lead3D_2.Fill(m_dsSecondChild_Lead3D_2, "Child2_Lead3D");
                if (m_blnInPocketExist2)
                    m_SecondChildDataAdapter_InPocket_2.Fill(m_dsSecondChild_InPocket_2, "Child2_InPocket");
                if (m_blnInPocket2Exist2)
                    m_SecondChildDataAdapter_InPocket2_2.Fill(m_dsSecondChild_InPocket2_2, "Child2_InPocket2");
                if (m_blnSealExist2)
                    m_SecondChildDataAdapter_Seal_2.Fill(m_dsSecondChild_Seal_2, "Child2_Seal");
                if (m_blnSeal2Exist2)
                    m_SecondChildDataAdapter_Seal2_2.Fill(m_dsSecondChild_Seal2_2, "Child2_Seal2");
                if (m_blnBarcodeExist2)
                    m_SecondChildDataAdapter_Barcode_2.Fill(m_dsSecondChild_Barcode_2, "Child2_Barcode");
                if (m_SecondChildDataAdapter_TopMenu_2 != null)
                    m_SecondChildDataAdapter_TopMenu_2.Fill(m_dsSecondChild_TopMenu_2, "Child2_TopMenu");
                if (m_SecondChildDataAdapter_BottomMenu_2 != null)
                    m_SecondChildDataAdapter_BottomMenu_2.Fill(m_dsSecondChild_BottomMenu_2, "Child2_BottomMenu");

                if (m_blnOrientExist3)
                    m_ThirdChildDataAdapter_Orientation_2.Fill(m_dsThirdChild_Orientation_2, "Child3_Orientation");
                if (m_blnMarkExist3)
                    m_ThirdChildDataAdapter_MarkOrient_2.Fill(m_dsThirdChild_MarkOrient_2, "Child3_MarkOrient");
                if (m_blnPadExist3)
                    m_ThirdChildDataAdapter_Pad_2.Fill(m_dsThirdChild_Pad_2, "Child3_Pad");
                if (m_blnLead3DExist3)
                    m_ThirdChildDataAdapter_Lead3D_2.Fill(m_dsThirdChild_Lead3D_2, "Child3_Lead3D");
                if (m_blnInPocketExist3)
                    m_ThirdChildDataAdapter_InPocket_2.Fill(m_dsThirdChild_InPocket_2, "Child3_InPocket");
                if (m_blnInPocket2Exist3)
                    m_ThirdChildDataAdapter_InPocket2_2.Fill(m_dsThirdChild_InPocket2_2, "Child3_InPocket2");
                if (m_blnSealExist3)
                    m_ThirdChildDataAdapter_Seal_2.Fill(m_dsThirdChild_Seal_2, "Child3_Seal");
                if (m_blnSeal2Exist3)
                    m_ThirdChildDataAdapter_Seal2_2.Fill(m_dsThirdChild_Seal2_2, "Child3_Seal2");
                if (m_blnBarcodeExist3)
                    m_ThirdChildDataAdapter_Barcode_2.Fill(m_dsThirdChild_Barcode_2, "Child3_Barcode");

            }
            catch (Exception ex)
            {
                STTrackLog.WriteLine("LoadUserRightForm.cs > GetRightDataSet_2() > Exception : " + ex.ToString());
            }
            finally
            {
                accessConn.Close();
            }

            accessConn = new OleDbConnection();
            accessConn.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                @"data source = " + AppDomain.CurrentDomain.BaseDirectory + "access\\simeca.mdb";
            accessConn.Open();

            m_dsIO_2 = new DataSet();
            m_dsIO_2.Tables.Add("IO");


            accessCommand = new OleDbCommand("SELECT * FROM IO", accessConn);
            m_IODataAdapter_2 = new OleDbDataAdapter(accessCommand);

            sqlUpdate = "UPDATE IO SET [Group] = @Group WHERE [Description] = @Description AND [Module] = @Module AND TYPE = 'Output'";
            m_IODataAdapter_2.UpdateCommand = new OleDbCommand(sqlUpdate, accessConn);
            m_workParam_2 = m_IODataAdapter_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.Integer));
            m_workParam_2.SourceColumn = "Group";
            m_workParam_2.SourceVersion = DataRowVersion.Current;
            m_workParam_2 = m_IODataAdapter_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Description", OleDbType.VarChar));
            m_workParam_2.SourceColumn = "Description";
            m_workParam_2.SourceVersion = DataRowVersion.Current;
            m_workParam_2 = m_IODataAdapter_2.UpdateCommand.Parameters.Add(new OleDbParameter("@Module", OleDbType.VarChar));
            m_workParam_2.SourceColumn = "Module";
            m_workParam_2.SourceVersion = DataRowVersion.Current;

            try
            {
                m_IODataAdapter_2.Fill(m_dsIO, "IO");
            }
            finally
            {
                accessConn.Close();
            }
        }
        private DataRow[] GetChildDataRow(string ParentName, string filter, string sort, int ChildIndex, bool blnDataTable_2)
        {
            switch (ParentName)
            {
                case "TopMenu":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_TopMenu_2.Tables["Child1_TopMenu"].Select(filter, sort);
                        else
                            return m_dsFirstChild_TopMenu.Tables["Child1_TopMenu"].Select(filter, sort);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_TopMenu_2.Tables["Child2_TopMenu"].Select(filter, sort);
                        else
                            return m_dsSecondChild_TopMenu.Tables["Child2_TopMenu"].Select(filter, sort);
                    }
                    break;
                case "BottomMenu":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_BottomMenu_2.Tables["Child1_BottomMenu"].Select(filter, sort);
                        else
                            return m_dsFirstChild_BottomMenu.Tables["Child1_BottomMenu"].Select(filter, sort);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_BottomMenu_2.Tables["Child2_BottomMenu"].Select(filter, sort);
                        else
                            return m_dsSecondChild_BottomMenu.Tables["Child2_BottomMenu"].Select(filter, sort);
                    }
                    break;
                case "Orientation":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Orientation_2.Tables["Child1_Orientation"].Select(filter, sort);
                        else
                            return m_dsFirstChild_Orientation.Tables["Child1_Orientation"].Select(filter, sort);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Orientation_2.Tables["Child2_Orientation"].Select(filter, sort);
                        else
                            return m_dsSecondChild_Orientation.Tables["Child2_Orientation"].Select(filter, sort);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Orientation_2.Tables["Child3_Orientation"].Select(filter, sort);
                        else
                            return m_dsThirdChild_Orientation.Tables["Child3_Orientation"].Select(filter, sort);
                    }
                    break;
                case "MarkOrient":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_MarkOrient_2.Tables["Child1_MarkOrient"].Select(filter, sort);
                        else
                            return m_dsFirstChild_MarkOrient.Tables["Child1_MarkOrient"].Select(filter, sort);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_MarkOrient_2.Tables["Child2_MarkOrient"].Select(filter, sort);
                        else
                            return m_dsSecondChild_MarkOrient.Tables["Child2_MarkOrient"].Select(filter, sort);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_MarkOrient_2.Tables["Child3_MarkOrient"].Select(filter, sort);
                        else
                            return m_dsThirdChild_MarkOrient.Tables["Child3_MarkOrient"].Select(filter, sort);
                    }
                    break;
                case "InPocket":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_InPocket_2.Tables["Child1_InPocket"].Select(filter, sort);
                        else
                            return m_dsFirstChild_InPocket.Tables["Child1_InPocket"].Select(filter, sort);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_InPocket_2.Tables["Child2_InPocket"].Select(filter, sort);
                        else
                            return m_dsSecondChild_InPocket.Tables["Child2_InPocket"].Select(filter, sort);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_InPocket_2.Tables["Child3_InPocket"].Select(filter, sort);
                        else
                            return m_dsThirdChild_InPocket.Tables["Child3_InPocket"].Select(filter, sort);
                    }
                    break;
                case "InPocket2":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_InPocket2_2.Tables["Child1_InPocket2"].Select(filter, sort);
                        else
                            return m_dsFirstChild_InPocket2.Tables["Child1_InPocket2"].Select(filter, sort);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_InPocket2_2.Tables["Child2_InPocket2"].Select(filter, sort);
                        else
                            return m_dsSecondChild_InPocket2.Tables["Child2_InPocket2"].Select(filter, sort);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_InPocket2_2.Tables["Child3_InPocket2"].Select(filter, sort);
                        else
                            return m_dsThirdChild_InPocket2.Tables["Child3_InPocket2"].Select(filter, sort);
                    }
                    break;
                case "Pad":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Pad_2.Tables["Child1_Pad"].Select(filter, sort);
                        else
                            return m_dsFirstChild_Pad.Tables["Child1_Pad"].Select(filter, sort);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Pad_2.Tables["Child2_Pad"].Select(filter, sort);
                        else
                            return m_dsSecondChild_Pad.Tables["Child2_Pad"].Select(filter, sort);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Pad_2.Tables["Child3_Pad"].Select(filter, sort);
                        else
                            return m_dsThirdChild_Pad.Tables["Child3_Pad"].Select(filter, sort);
                    }
                    break;
                case "Lead3D":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Lead3D_2.Tables["Child1_Lead3D"].Select(filter, sort);
                        else
                            return m_dsFirstChild_Lead3D.Tables["Child1_Lead3D"].Select(filter, sort);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Lead3D_2.Tables["Child2_Lead3D"].Select(filter, sort);
                        else
                            return m_dsSecondChild_Lead3D.Tables["Child2_Lead3D"].Select(filter, sort);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Lead3D_2.Tables["Child3_Lead3D"].Select(filter, sort);
                        else
                            return m_dsThirdChild_Lead3D.Tables["Child3_Lead3D"].Select(filter, sort);
                    }
                    break;
                case "Seal":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Seal_2.Tables["Child1_Seal"].Select(filter, sort);
                        else
                            return m_dsFirstChild_Seal.Tables["Child1_Seal"].Select(filter, sort);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Seal_2.Tables["Child2_Seal"].Select(filter, sort);
                        else
                            return m_dsSecondChild_Seal.Tables["Child2_Seal"].Select(filter, sort);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Seal_2.Tables["Child3_Seal"].Select(filter, sort);
                        else
                            return m_dsThirdChild_Seal.Tables["Child3_Seal"].Select(filter, sort);
                    }
                    break;
                case "Seal2":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Seal2_2.Tables["Child1_Seal2"].Select(filter, sort);
                        else
                            return m_dsFirstChild_Seal2.Tables["Child1_Seal2"].Select(filter, sort);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Seal2_2.Tables["Child2_Seal2"].Select(filter, sort);
                        else
                            return m_dsSecondChild_Seal2.Tables["Child2_Seal2"].Select(filter, sort);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Seal2_2.Tables["Child3_Seal2"].Select(filter, sort);
                        else
                            return m_dsThirdChild_Seal2.Tables["Child3_Seal2"].Select(filter, sort);
                    }
                    break;
                case "Barcode":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Barcode_2.Tables["Child1_Barcode"].Select(filter, sort);
                        else
                            return m_dsFirstChild_Barcode.Tables["Child1_Barcode"].Select(filter, sort);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Barcode_2.Tables["Child2_Barcode"].Select(filter, sort);
                        else
                            return m_dsSecondChild_Barcode.Tables["Child2_Barcode"].Select(filter, sort);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Barcode_2.Tables["Child3_Barcode"].Select(filter, sort);
                        else
                            return m_dsThirdChild_Barcode.Tables["Child3_Barcode"].Select(filter, sort);
                    }
                    break;
            }

            return new DataRow[0];
        }

        private DataRow[] GetChildDataRow(string ParentName, string filter, int ChildIndex, bool blnDataTable_2)
        {
            switch (ParentName)
            {
                case "TopMenu":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_TopMenu_2.Tables["Child1_TopMenu"].Select(filter);
                        else
                            return m_dsFirstChild_TopMenu.Tables["Child1_TopMenu"].Select(filter);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_TopMenu_2.Tables["Child2_TopMenu"].Select(filter);
                        else
                            return m_dsSecondChild_TopMenu.Tables["Child2_TopMenu"].Select(filter);
                    }
                    break;
                case "BottomMenu":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_BottomMenu_2.Tables["Child1_BottomMenu"].Select(filter);
                        else
                            return m_dsFirstChild_BottomMenu.Tables["Child1_BottomMenu"].Select(filter);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_BottomMenu_2.Tables["Child2_BottomMenu"].Select(filter);
                        else
                            return m_dsSecondChild_BottomMenu.Tables["Child2_BottomMenu"].Select(filter);
                    }
                    break;
                case "Orientation":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Orientation_2.Tables["Child1_Orientation"].Select(filter);
                        else
                            return m_dsFirstChild_Orientation.Tables["Child1_Orientation"].Select(filter);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Orientation_2.Tables["Child2_Orientation"].Select(filter);
                        else
                            return m_dsSecondChild_Orientation.Tables["Child2_Orientation"].Select(filter);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Orientation_2.Tables["Child3_Orientation"].Select(filter);
                        else
                            return m_dsThirdChild_Orientation.Tables["Child3_Orientation"].Select(filter);
                    }
                    break;
                case "MarkOrient":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_MarkOrient_2.Tables["Child1_MarkOrient"].Select(filter);
                        else
                            return m_dsFirstChild_MarkOrient.Tables["Child1_MarkOrient"].Select(filter);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_MarkOrient_2.Tables["Child2_MarkOrient"].Select(filter);
                        else
                            return m_dsSecondChild_MarkOrient.Tables["Child2_MarkOrient"].Select(filter);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_MarkOrient_2.Tables["Child3_MarkOrient"].Select(filter);
                        else
                            return m_dsThirdChild_MarkOrient.Tables["Child3_MarkOrient"].Select(filter);
                    }
                    break;
                case "InPocket":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_InPocket_2.Tables["Child1_InPocket"].Select(filter);
                        else
                            return m_dsFirstChild_InPocket.Tables["Child1_InPocket"].Select(filter);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_InPocket_2.Tables["Child2_InPocket"].Select(filter);
                        else
                            return m_dsSecondChild_InPocket.Tables["Child2_InPocket"].Select(filter);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_InPocket_2.Tables["Child3_InPocket"].Select(filter);
                        else
                            return m_dsThirdChild_InPocket.Tables["Child3_InPocket"].Select(filter);
                    }
                    break;
                case "InPocket2":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_InPocket2_2.Tables["Child1_InPocket2"].Select(filter);
                        else
                            return m_dsFirstChild_InPocket2.Tables["Child1_InPocket2"].Select(filter);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_InPocket2_2.Tables["Child2_InPocket2"].Select(filter);
                        else
                            return m_dsSecondChild_InPocket2.Tables["Child2_InPocket2"].Select(filter);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_InPocket2_2.Tables["Child3_InPocket2"].Select(filter);
                        else
                            return m_dsThirdChild_InPocket2.Tables["Child3_InPocket2"].Select(filter);
                    }
                    break;
                case "Pad":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Pad_2.Tables["Child1_Pad"].Select(filter);
                        else
                            return m_dsFirstChild_Pad.Tables["Child1_Pad"].Select(filter);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Pad_2.Tables["Child2_Pad"].Select(filter);
                        else
                            return m_dsSecondChild_Pad.Tables["Child2_Pad"].Select(filter);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Pad_2.Tables["Child3_Pad"].Select(filter);
                        else
                            return m_dsThirdChild_Pad.Tables["Child3_Pad"].Select(filter);
                    }
                    break;
                case "Lead3D":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Lead3D_2.Tables["Child1_Lead3D"].Select(filter);
                        else
                            return m_dsFirstChild_Lead3D.Tables["Child1_Lead3D"].Select(filter);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Lead3D_2.Tables["Child2_Lead3D"].Select(filter);
                        else
                            return m_dsSecondChild_Lead3D.Tables["Child2_Lead3D"].Select(filter);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Lead3D_2.Tables["Child3_Lead3D"].Select(filter);
                        else
                            return m_dsThirdChild_Lead3D.Tables["Child3_Lead3D"].Select(filter);
                    }
                    break;
                case "Seal":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Seal_2.Tables["Child1_Seal"].Select(filter);
                        else
                            return m_dsFirstChild_Seal.Tables["Child1_Seal"].Select(filter);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Seal_2.Tables["Child2_Seal"].Select(filter);
                        else
                            return m_dsSecondChild_Seal.Tables["Child2_Seal"].Select(filter);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Seal_2.Tables["Child3_Seal"].Select(filter);
                        else
                            return m_dsThirdChild_Seal.Tables["Child3_Seal"].Select(filter);
                    }
                    break;
                case "Seal2":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Seal2_2.Tables["Child1_Seal2"].Select(filter);
                        else
                            return m_dsFirstChild_Seal2.Tables["Child1_Seal2"].Select(filter);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Seal2_2.Tables["Child2_Seal2"].Select(filter);
                        else
                            return m_dsSecondChild_Seal2.Tables["Child2_Seal2"].Select(filter);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Seal2_2.Tables["Child3_Seal2"].Select(filter);
                        else
                            return m_dsThirdChild_Seal2.Tables["Child3_Seal2"].Select(filter);
                    }
                    break;
                case "Barcode":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Barcode_2.Tables["Child1_Barcode"].Select(filter);
                        else
                            return m_dsFirstChild_Barcode.Tables["Child1_Barcode"].Select(filter);
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Barcode_2.Tables["Child2_Barcode"].Select(filter);
                        else
                            return m_dsSecondChild_Barcode.Tables["Child2_Barcode"].Select(filter);
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Barcode_2.Tables["Child3_Barcode"].Select(filter);
                        else
                            return m_dsThirdChild_Barcode.Tables["Child3_Barcode"].Select(filter);
                    }
                    break;
            }

            return new DataRow[0];
        }
        private DataRow[] GetChildDataRow(string ParentName, int ChildIndex, bool blnDataTable_2)
        {
            switch (ParentName)
            {
                case "TopMenu":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_TopMenu_2.Tables["Child1_TopMenu"].Select();
                        else
                            return m_dsFirstChild_TopMenu.Tables["Child1_TopMenu"].Select();
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_TopMenu_2.Tables["Child2_TopMenu"].Select();
                        else
                            return m_dsSecondChild_TopMenu.Tables["Child2_TopMenu"].Select();
                    }
                    break;
                case "BottomMenu":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_BottomMenu_2.Tables["Child1_BottomMenu"].Select();
                        else
                            return m_dsFirstChild_BottomMenu.Tables["Child1_BottomMenu"].Select();
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_BottomMenu_2.Tables["Child2_BottomMenu"].Select();
                        else
                            return m_dsSecondChild_BottomMenu.Tables["Child2_BottomMenu"].Select();
                    }
                    break;
                case "Orientation":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Orientation_2.Tables["Child1_Orientation"].Select();
                        else
                            return m_dsFirstChild_Orientation.Tables["Child1_Orientation"].Select();
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Orientation_2.Tables["Child2_Orientation"].Select();
                        else
                            return m_dsSecondChild_Orientation.Tables["Child2_Orientation"].Select();
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Orientation_2.Tables["Child3_Orientation"].Select();
                        else
                            return m_dsThirdChild_Orientation.Tables["Child3_Orientation"].Select();
                    }
                    break;
                case "MarkOrient":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_MarkOrient_2.Tables["Child1_MarkOrient"].Select();
                        else
                            return m_dsFirstChild_MarkOrient.Tables["Child1_MarkOrient"].Select();
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_MarkOrient_2.Tables["Child2_MarkOrient"].Select();
                        else
                            return m_dsSecondChild_MarkOrient.Tables["Child2_MarkOrient"].Select();
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_MarkOrient_2.Tables["Child3_MarkOrient"].Select();
                        else
                            return m_dsThirdChild_MarkOrient.Tables["Child3_MarkOrient"].Select();
                    }
                    break;
                case "InPocket":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_InPocket_2.Tables["Child1_InPocket"].Select();
                        else
                            return m_dsFirstChild_InPocket.Tables["Child1_InPocket"].Select();
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_InPocket_2.Tables["Child2_InPocket"].Select();
                        else
                            return m_dsSecondChild_InPocket.Tables["Child2_InPocket"].Select();
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_InPocket_2.Tables["Child3_InPocket"].Select();
                        else
                            return m_dsThirdChild_InPocket.Tables["Child3_InPocket"].Select();
                    }
                    break;
                case "InPocket2":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_InPocket2_2.Tables["Child1_InPocket2"].Select();
                        else
                            return m_dsFirstChild_InPocket2.Tables["Child1_InPocket2"].Select();
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_InPocket2_2.Tables["Child2_InPocket2"].Select();
                        else
                            return m_dsSecondChild_InPocket2.Tables["Child2_InPocket2"].Select();
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_InPocket2_2.Tables["Child3_InPocket2"].Select();
                        else
                            return m_dsThirdChild_InPocket2.Tables["Child3_InPocket2"].Select();
                    }
                    break;
                case "Pad":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Pad_2.Tables["Child1_Pad"].Select();
                        else
                            return m_dsFirstChild_Pad.Tables["Child1_Pad"].Select();
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Pad_2.Tables["Child2_Pad"].Select();
                        else
                            return m_dsSecondChild_Pad.Tables["Child2_Pad"].Select();
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Pad_2.Tables["Child3_Pad"].Select();
                        else
                            return m_dsThirdChild_Pad.Tables["Child3_Pad"].Select();
                    }
                    break;
                case "Lead3D":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Lead3D_2.Tables["Child1_Lead3D"].Select();
                        else
                            return m_dsFirstChild_Lead3D.Tables["Child1_Lead3D"].Select();
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Lead3D_2.Tables["Child2_Lead3D"].Select();
                        else
                            return m_dsSecondChild_Lead3D.Tables["Child2_Lead3D"].Select();
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Lead3D_2.Tables["Child3_Lead3D"].Select();
                        else
                            return m_dsThirdChild_Lead3D.Tables["Child3_Lead3D"].Select();
                    }
                    break;
                case "Seal":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Seal_2.Tables["Child1_Seal"].Select();
                        else
                            return m_dsFirstChild_Seal.Tables["Child1_Seal"].Select();
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Seal_2.Tables["Child2_Seal"].Select();
                        else
                            return m_dsSecondChild_Seal.Tables["Child2_Seal"].Select();
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Seal_2.Tables["Child3_Seal"].Select();
                        else
                            return m_dsThirdChild_Seal.Tables["Child3_Seal"].Select();
                    }
                    break;
                case "Seal2":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Seal2_2.Tables["Child1_Seal2"].Select();
                        else
                            return m_dsFirstChild_Seal2.Tables["Child1_Seal2"].Select();
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Seal2_2.Tables["Child2_Seal2"].Select();
                        else
                            return m_dsSecondChild_Seal2.Tables["Child2_Seal2"].Select();
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Seal2_2.Tables["Child3_Seal2"].Select();
                        else
                            return m_dsThirdChild_Seal2.Tables["Child3_Seal2"].Select();
                    }
                    break;
                case "Barcode":
                    if (ChildIndex == 1)
                    {
                        if (blnDataTable_2)
                            return m_dsFirstChild_Barcode_2.Tables["Child1_Barcode"].Select();
                        else
                            return m_dsFirstChild_Barcode.Tables["Child1_Barcode"].Select();
                    }
                    else if (ChildIndex == 2)
                    {
                        if (blnDataTable_2)
                            return m_dsSecondChild_Barcode_2.Tables["Child2_Barcode"].Select();
                        else
                            return m_dsSecondChild_Barcode.Tables["Child2_Barcode"].Select();
                    }
                    else if (ChildIndex == 3)
                    {
                        if (blnDataTable_2)
                            return m_dsThirdChild_Barcode_2.Tables["Child3_Barcode"].Select();
                        else
                            return m_dsThirdChild_Barcode.Tables["Child3_Barcode"].Select();
                    }
                    break;
            }

            return new DataRow[0];
        }
        private void UpdateTable(string ParentName, int ChildIndex)
        {
            switch (ParentName)
            {
                case "TopMenu":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_TopMenu.Update(m_dsFirstChild_TopMenu, "Child1_TopMenu");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_TopMenu.Update(m_dsSecondChild_TopMenu, "Child2_TopMenu");
                    break;
                case "BottomMenu":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_BottomMenu.Update(m_dsFirstChild_BottomMenu, "Child1_BottomMenu");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_BottomMenu.Update(m_dsSecondChild_BottomMenu, "Child2_BottomMenu");
                    break;
                case "Orientation":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_Orientation.Update(m_dsFirstChild_Orientation, "Child1_Orientation");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_Orientation.Update(m_dsSecondChild_Orientation, "Child2_Orientation");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_Orientation.Update(m_dsThirdChild_Orientation, "Child3_Orientation");
                    break;
                case "MarkOrient":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_MarkOrient.Update(m_dsFirstChild_MarkOrient, "Child1_MarkOrient");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_MarkOrient.Update(m_dsSecondChild_MarkOrient, "Child2_MarkOrient");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_MarkOrient.Update(m_dsThirdChild_MarkOrient, "Child3_MarkOrient");
                    break;
                case "InPocket":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_InPocket.Update(m_dsFirstChild_InPocket, "Child1_InPocket");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_InPocket.Update(m_dsSecondChild_InPocket, "Child2_InPocket");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_InPocket.Update(m_dsThirdChild_InPocket, "Child3_InPocket");
                    break;
                case "InPocket2":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_InPocket2.Update(m_dsFirstChild_InPocket2, "Child1_InPocket2");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_InPocket2.Update(m_dsSecondChild_InPocket2, "Child2_InPocket2");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_InPocket2.Update(m_dsThirdChild_InPocket2, "Child3_InPocket2");
                    break;
                case "Pad":
                    if (ChildIndex == 1)
                        m_FirstChildDataAdapter_Pad.Update(m_dsFirstChild_Pad, "Child1_Pad");
                    else if (ChildIndex == 2)
                        m_SecondChildDataAdapter_Pad.Update(m_dsSecondChild_Pad, "Child2_Pad");
                    else if (ChildIndex == 3)
                        m_ThirdChildDataAdapter_Pad.Update(m_dsThirdChild_Pad, "Child3_Pad");
                    break;
                case "Lead3D":
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
            }

        }
        private void InitParentTree()
        {
            //int ParentArrayGroup = 0;
            //int FirstChildArrayGroup = 0;
            //int SecondChildArrayGroup = 0;
            //int ThirdChildArrayGroup = 0;

            //m_intParentArrayGroup = new int[20]; // 10 Vision + Top Menu
            //m_intFirstChildArrayGroup = new int[20][];

            //for (int x = 0; x < m_intFirstChildArrayGroup.Length; x++)
            //{
            //    m_intFirstChildArrayGroup[x] = new int[20];
            //}

            //m_intSecondChildArrayGroup = new int[20][][];
            //for (int x = 0; x < m_intFirstChildArrayGroup.Length; x++)
            //{
            //    m_intSecondChildArrayGroup[x] = new int[20][];
            //    for (int y = 0; y < m_intSecondChildArrayGroup[x].Length; y++)
            //    {
            //        m_intSecondChildArrayGroup[x][y] = new int[20];
            //    }
            //}

            //m_intThirdChildArrayGroup = new int[20][][][];
            //for (int x = 0; x < m_intSecondChildArrayGroup.Length; x++)
            //{
            //    m_intThirdChildArrayGroup[x] = new int[20][][];
            //    for (int y = 0; y < m_intThirdChildArrayGroup[x].Length; y++)
            //    {
            //        m_intThirdChildArrayGroup[x][y] = new int[20][];
            //        for (int z = 0; z < m_intThirdChildArrayGroup[x][y].Length; z++)
            //        {
            //            m_intThirdChildArrayGroup[x][y][z] = new int[100];
            //        }
            //    }
            //}

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
                        //TreeNode newParent = new TreeNode(strParentName);
                        //tre_UserRight.Nodes.Add(newParent);


                        //m_intParentArrayGroup[ParentArrayGroup] = intParentGroup;
                        //FirstChildArrayGroup = 0;

                        if (strFirstChild != "")
                        {
                            sort = "Number";
                            filter = "[Parent Number] = " + intParentNum;
                            DataRow[] FirstChildList = GetChildDataRow(strParentName, filter, sort, 1, false);
                            foreach (DataRow Firstchild in FirstChildList)
                            {
                                string strFirstChildName = Firstchild["Name"].ToString();
                                string strSecondChild = Firstchild["Child2"].ToString();
                                int intFirstChildNo = Convert.ToInt32(Firstchild["Number"]);
                                int intFirstChildGroup = Convert.ToInt32(Firstchild["Group"]);

                                if (m_intGroup <= intFirstChildGroup)
                                {
                                    //TreeNode newFirstChild = new TreeNode(strFirstChildName);
                                    //newParent.Nodes.Add(newFirstChild);
                                    //m_intFirstChildArrayGroup[ParentArrayGroup][FirstChildArrayGroup] = intFirstChildGroup;
                                    //SecondChildArrayGroup = 0;
                                    if (strSecondChild != "")
                                    {
                                        sort = "Number";
                                        filter = "[Child1 Number] = " + intFirstChildNo;
                                        DataRow[] SecondChildList = GetChildDataRow(strParentName, filter, sort, 2, false);
                                        foreach (DataRow SecondChild in SecondChildList)
                                        {

                                            string strSecondChildName = SecondChild["Name"].ToString();
                                            string strThirdChild = SecondChild["Child3"].ToString();
                                            int intSecondChildNo = Convert.ToInt32(SecondChild["Number"]);
                                            int intSecondChildGroup = Convert.ToInt32(SecondChild["Group"]);
                                            
                                            if (m_intGroup <= intSecondChildGroup)
                                            {
                                                //TreeNode newSecondChild = new TreeNode(strSecondChildName);
                                                //newFirstChild.Nodes.Add(newSecondChild);
                                                //m_intSecondChildArrayGroup[ParentArrayGroup][FirstChildArrayGroup][SecondChildArrayGroup] = intSecondChildGroup;
                                                //ThirdChildArrayGroup = 0;
                                                if (strThirdChild != "")
                                                {
                                                    sort = "Number";
                                                    filter = "[Child2 Number] = " + intSecondChildNo;
                                                    DataRow[] ThirdChildList = GetChildDataRow(strParentName, filter, sort, 3, false);
                                                    foreach (DataRow ThirdChild in ThirdChildList)
                                                    {

                                                        string strThirdChildName = ThirdChild["Name"].ToString();
                                                        int intThirdChildGroup = Convert.ToInt32(ThirdChild["Group"]);
                                                        if (m_intGroup <= intThirdChildGroup)
                                                        {
                                                            
                                                            //TreeNode newThirdChild = new TreeNode(strThirdChildName);
                                                            //newSecondChild.Nodes.Add(newThirdChild);
                                                            //m_intThirdChildArrayGroup[ParentArrayGroup][FirstChildArrayGroup][SecondChildArrayGroup][ThirdChildArrayGroup] = intThirdChildGroup;
                                                            //ThirdChildArrayGroup++;
                                                        }
                                                    }
                                                }
                                                //SecondChildArrayGroup++;
                                            }
                                        }
                                    }

                                    //FirstChildArrayGroup++;
                                }
                            }
                        }

                        //ParentArrayGroup++;
                    }
                }
            }

            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitParentTree2()
        {
         
            try
            {
                //string sort = "Number";
                string filter;// = "Number > 0";
                DataRow[] DataList = m_dsParent.Tables["Parent"].Select();
                foreach (DataRow Data in DataList)
                {
                    string strName = Data["Description"].ToString();
                   
                    filter = "[Description] = '" + strName + "'";
                    DataRow[] DataList_2 = m_dsParent_2.Tables["Parent"].Select(filter);
                    if (DataList_2.Length > 0)
                    {
                        DataRow Data_2 = DataList_2[0];
                        int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                        Data["Group"] = intGroup_2;

                    }
                }
                m_ParentDataAdapter.Update(m_dsParent, "Parent");

                DataList = GetChildDataRow("TopMenu", 1, false);
                foreach (DataRow Data in DataList)
                {
                    string strName = Data["Name"].ToString();

                    filter = "[Name] = '" + strName + "'";
                    DataRow[] DataList_2 = GetChildDataRow("TopMenu", filter, 1, true);
                    if (DataList_2.Length > 0)
                    {
                        DataRow Data_2 = DataList_2[0];
                        int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                        Data["Group"] = intGroup_2;

                    }
                }
                UpdateTable("TopMenu", 1);

                DataList = GetChildDataRow("TopMenu", 2, false);
                foreach (DataRow Data in DataList)
                {
                    string strName = Data["Name"].ToString();

                    filter = "[Name] = '" + strName + "'";
                    DataRow[] DataList_2 = GetChildDataRow("TopMenu", filter, 2, true);
                    if (DataList_2.Length > 0)
                    {
                        DataRow Data_2 = DataList_2[0];
                        int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                        Data["Group"] = intGroup_2;

                    }
                }
                UpdateTable("TopMenu", 2);

                DataList = GetChildDataRow("BottomMenu", 1, false);
                foreach (DataRow Data in DataList)
                {
                    string strName = Data["Name"].ToString();

                    filter = "[Name] = '" + strName + "'";
                    DataRow[] DataList_2 = GetChildDataRow("BottomMenu", filter, 1, true);
                    if (DataList_2.Length > 0)
                    {
                        DataRow Data_2 = DataList_2[0];
                        int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                        Data["Group"] = intGroup_2;

                    }
                }
                UpdateTable("BottomMenu", 1);

                DataList = GetChildDataRow("BottomMenu", 2, false);
                foreach (DataRow Data in DataList)
                {
                    string strName = Data["Name"].ToString();

                    filter = "[Name] = '" + strName + "'";
                    DataRow[] DataList_2 = GetChildDataRow("BottomMenu", filter, 2, true);
                    if (DataList_2.Length > 0)
                    {
                        DataRow Data_2 = DataList_2[0];
                        int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                        Data["Group"] = intGroup_2;

                    }
                }
                UpdateTable("BottomMenu", 2);

                if (m_blnOrientExist1)
                {
                    DataList = GetChildDataRow("Orientation", 1, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Orientation", filter, 1, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Orientation", 1);
                }

                if (m_blnOrientExist2)
                {
                    DataList = GetChildDataRow("Orientation", 2, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Orientation", filter, 2, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Orientation", 2);
                }

                if (m_blnOrientExist3)
                {
                    DataList = GetChildDataRow("Orientation", 3, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Orientation", filter, 3, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Orientation", 3);
                }

                if (m_blnMarkExist1)
                {
                    DataList = GetChildDataRow("MarkOrient", 1, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("MarkOrient", filter, 1, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("MarkOrient", 1);
                }

                if (m_blnMarkExist2)
                {
                    DataList = GetChildDataRow("MarkOrient", 2, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("MarkOrient", filter, 2, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("MarkOrient", 2);
                }

                if (m_blnMarkExist3)
                {
                    DataList = GetChildDataRow("MarkOrient", 3, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("MarkOrient", filter, 3, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("MarkOrient", 3);
                }

                if (m_blnInPocketExist1)
                {
                    DataList = GetChildDataRow("InPocket", 1, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("InPocket", filter, 1, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("InPocket", 1);
                }

                if (m_blnInPocket2Exist1)
                {
                    DataList = GetChildDataRow("InPocket2", 1, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("InPocket2", filter, 1, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                    UpdateTable("InPocket2", 1);
                }

                if (m_blnInPocketExist2)
                {
                    DataList = GetChildDataRow("InPocket", 2, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("InPocket", filter, 2, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("InPocket", 2);
                }

                if (m_blnInPocket2Exist2)
                {
                    DataList = GetChildDataRow("InPocket2", 2, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("InPocket2", filter, 2, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                    UpdateTable("InPocket2", 2);
                }

                if (m_blnInPocketExist3)
                {
                    DataList = GetChildDataRow("InPocket", 3, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("InPocket", filter, 3, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("InPocket", 3);
                }

                if (m_blnInPocket2Exist3)
                {
                    DataList = GetChildDataRow("InPocket2", 3, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("InPocket2", filter, 3, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                    UpdateTable("InPocket2", 3);
                }

                if (m_blnPadExist1)
                {
                    DataList = GetChildDataRow("Pad", 1, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Pad", filter, 1, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Pad", 1);
                }

                if (m_blnPadExist2)
                {
                    DataList = GetChildDataRow("Pad", 2, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Pad", filter, 2, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Pad", 2);
                }

                if (m_blnPadExist3)
                {
                    DataList = GetChildDataRow("Pad", 3, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Pad", filter, 3, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Pad", 3);
                }

                if (m_blnLead3DExist1)
                {
                    DataList = GetChildDataRow("Lead3D", 1, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Lead3D", filter, 1, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Lead3D", 1);
                }

                if (m_blnLead3DExist2)
                {
                    DataList = GetChildDataRow("Lead3D", 2, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Lead3D", filter, 2, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Lead3D", 2);
                }

                if (m_blnLead3DExist3)
                {
                    DataList = GetChildDataRow("Lead3D", 3, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Lead3D", filter, 3, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Lead3D", 3);
                }

                if (m_blnSealExist1)
                {
                    DataList = GetChildDataRow("Seal", 1, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Seal", filter, 1, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Seal", 1);
                }

                if (m_blnSeal2Exist1)
                {
                    DataList = GetChildDataRow("Seal2", 1, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Seal2", filter, 1, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                    UpdateTable("Seal2", 1);
                }

                if (m_blnSealExist2)
                {
                    DataList = GetChildDataRow("Seal", 2, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Seal", filter, 2, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Seal", 2);
                }

                if (m_blnSeal2Exist2)
                {
                    DataList = GetChildDataRow("Seal2", 2, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Seal2", filter, 2, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                    UpdateTable("Seal2", 2);
                }

                if (m_blnSealExist3)
                {
                    DataList = GetChildDataRow("Seal", 3, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Seal", filter, 3, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Seal", 3);
                }

                if (m_blnSeal2Exist3)
                {
                    DataList = GetChildDataRow("Seal2", 3, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Seal2", filter, 3, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                    UpdateTable("Seal2", 3);
                }

                if (m_blnBarcodeExist1)
                {
                    DataList = GetChildDataRow("Barcode", 1, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Barcode", filter, 1, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Barcode", 1);
                }

                if (m_blnBarcodeExist2)
                {
                    DataList = GetChildDataRow("Barcode", 2, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Barcode", filter, 2, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Barcode", 2);
                }

                if (m_blnBarcodeExist3)
                {
                    DataList = GetChildDataRow("Barcode", 3, false);
                    foreach (DataRow Data in DataList)
                    {
                        string strName = Data["Name"].ToString();
                        int intNum = Convert.ToInt32(Data["Number"]);

                        filter = "Name = '" + strName + "' AND [Number] = " + intNum;
                        DataRow[] DataList_2 = GetChildDataRow("Barcode", filter, 3, true);
                        if (DataList_2.Length > 0)
                        {
                            DataRow Data_2 = DataList_2[0];
                            int intGroup_2 = Convert.ToInt32(Data_2["Group"]);
                            Data["Group"] = intGroup_2;

                        }
                    }
                            UpdateTable("Barcode", 3);
                }
            }

            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
