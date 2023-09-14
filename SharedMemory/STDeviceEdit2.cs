using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
//using SharedMemory;
using Common;
namespace SharedMemory
{
    public class STDeviceEdit2
    {
        #region Member Variables
        private static Object m_objLock = new Object();
        private static string m_originalFile;
        private static string m_tempFile;
        private static string m_recipeName;
        private static string m_strGroupName = "";
        private static string m_strUserName = "";
        private static string m_strDeviceNo = "";
        private static string m_strCurrentMonthDB = "";
        private static OleDbDataAdapter m_daDeviceEditLog;
        private static OleDbConnection m_DeviceEditLogConn;
        private static string m_strHistoryDataLocation = "D:\\";
        public static DataSet m_dsDeviceEditLog;
        private static bool m_blnWantEditLog = true;
        #endregion


        public static void InitDeviceEdit(ProductionInfo smProductionInfo)
        {
            STTrackLog.WriteLine("---------- Edit Log 1 : Init");
            m_blnWantEditLog = smProductionInfo.g_blnWantEditLog;
            if (m_blnWantEditLog)
            {
                m_strHistoryDataLocation = smProductionInfo.g_strHistoryDataLocation;
                switch (smProductionInfo.g_intUserGroup)
                {
                    case 5:
                        m_strGroupName = "Operator";
                        break;
                    case 4:
                        m_strGroupName = "Technician";
                        break;
                    case 3:
                        m_strGroupName = "Engineer";
                        break;
                    case 2:
                        m_strGroupName = "Administrator";
                        break;
                    case 1:
                        m_strGroupName = "SRM Vendor";
                        break;
                    default:
                        m_strGroupName = "Unknown User intGroup";
                        break;
                }

                m_strUserName = smProductionInfo.g_strOperatorID;
                m_strDeviceNo = smProductionInfo.g_strRecipeID;

                Init();
            }

            STTrackLog.WriteLine("---------- Edit Log 2 : Done");
        }




        //public DeviceEdit()
        //{
        //    Init();
        //}



        /// <summary>
        /// Fill in the device edit log data into dataset
        /// </summary>
        /// <param name="strDBFile">database file name (.mdb)</param>
        public static void GetDeviceEditLogDataSet(string strDBFile)
        {
            STTrackLog.WriteLine("---------- Edit Log 2 : GetDeviceEditLogDataSet");
            if (m_blnWantEditLog)
            {
                lock (m_objLock)
                {
                    try
                    {
                        m_dsDeviceEditLog = new DataSet();
                        m_dsDeviceEditLog.Tables.Add("DeviceEditLog");
                        InitDeviceEditLogDataAdapter(strDBFile);

                        //2020-12-30 ZJYEOH : Close connection before DataAdapter use Fill function because :
                        // - The DataAdapter automatically opens and closes a Connection as required if it is not already open when a method such as Fill( ), FillSchema( ), or Update( ) is called.
                        // - The Connection must be explicitly closed if it is already open prior to the DataAdapter operation.
                        m_DeviceEditLogConn.Close();
                        OleDbConnection.ReleaseObjectPool();
                        bool blnColumnChanged = false;

                        STTrackLog.WriteLine("-------------------------------- Edit Log 1000 Check ldb file");
                        string strLDBFileName = strDBFile.Substring(0, strDBFile.LastIndexOf('.')) + ".ldb";
                        if (File.Exists(m_strHistoryDataLocation + "Data\\" + strLDBFileName))
                        {
                            try
                            {
                                STTrackLog.WriteLine("-------------------------------- Edit Log 1001 ldb file Exist");
                                File.Delete(m_strHistoryDataLocation + "Data\\" + strLDBFileName);
                                STTrackLog.WriteLine("-------------------------------- Edit Log 1002 ldb file Deleted");
                            }
                            catch (Exception ex)
                            {
                                STTrackLog.WriteLine("DeviceEdit.cs > GetDeviceEditLogDataSet > exception = " + ex.ToString());
                            }
                        }
                        STTrackLog.WriteLine("--------------------- Edit Log 101 m_daDeviceEditLog Fill");
                        m_daDeviceEditLog.Fill(m_dsDeviceEditLog, "DeviceEditLog");
                        m_DeviceEditLogConn.Close();
                        OleDbConnection.ReleaseObjectPool();
                        STTrackLog.WriteLine("--------------------- Edit Log 102 m_daDeviceEditLog FillSchema");
                        //2020-12-31 ZJYEOH : FillSchema will get the table column properties
                        m_daDeviceEditLog.FillSchema(m_dsDeviceEditLog, SchemaType.Source);

                        //2020-12-31 ZJYEOH : if Max Length less than 255, set the column to 255.  255 is the Max Length of Text for .mdb file
                        if (m_dsDeviceEditLog.Tables[1].Columns[0].MaxLength < 255)
                        {
                            blnColumnChanged = true;
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ALTER COLUMN [UserName] TEXT(255)";

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            OleDbConnection.ReleaseObjectPool();
                        }
                        if (m_dsDeviceEditLog.Tables[1].Columns[1].MaxLength < 255)
                        {
                            blnColumnChanged = true;
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ALTER COLUMN [Group] TEXT(255)";

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            OleDbConnection.ReleaseObjectPool();
                        }
                        if (m_dsDeviceEditLog.Tables[1].Columns[2].MaxLength < 255)
                        {
                            blnColumnChanged = true;
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ALTER COLUMN [Module] TEXT(255)";

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            OleDbConnection.ReleaseObjectPool();
                        }
                        if (m_dsDeviceEditLog.Tables[1].Columns[3].MaxLength < 255)
                        {
                            blnColumnChanged = true;
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ALTER COLUMN [Description] TEXT(255)";

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            OleDbConnection.ReleaseObjectPool();
                        }
                        if (m_dsDeviceEditLog.Tables[1].Columns[4].MaxLength < 255)
                        {
                            blnColumnChanged = true;
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ALTER COLUMN [OriginalValue] TEXT(255)";

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            OleDbConnection.ReleaseObjectPool();
                        }
                        if (m_dsDeviceEditLog.Tables[1].Columns[5].MaxLength < 255)
                        {
                            blnColumnChanged = true;
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ALTER COLUMN [NewValue] TEXT(255)";

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            OleDbConnection.ReleaseObjectPool();
                        }

                        //2020-12-31 ZJYEOH : If Column properties changed then reassign the table again
                        if (blnColumnChanged)
                        {
                            m_dsDeviceEditLog = new DataSet();
                            m_dsDeviceEditLog.Tables.Add("DeviceEditLog");
                            InitDeviceEditLogDataAdapter(strDBFile);
                            m_DeviceEditLogConn.Close();
                            OleDbConnection.ReleaseObjectPool();
                            STTrackLog.WriteLine("--------------------- Edit Log 103 : Fill m_daDeviceEditLog");
                            m_daDeviceEditLog.Fill(m_dsDeviceEditLog, "DeviceEditLog");
                        }

                        //2019-10-11 ZJYEOH : Need to Add another column to access database file(.mdb), because old software access database file(.mdb) dont have this column
                        if (m_dsDeviceEditLog.Tables["DeviceEditLog"].Columns.Count < 8 && m_dsDeviceEditLog.Tables["DeviceEditLog"].Columns.Count != 0)
                        {
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ADD COLUMN LotID TEXT(255)"; // 2019-10-11 ZJYEOH : SQL_MAX_CHAR_LITERAL_LEN is 255 Only

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            m_dsDeviceEditLog.Tables["DeviceEditLog"].Columns.Add("LotID");
                            OleDbConnection.ReleaseObjectPool();
                        }


                    }
                    catch (Exception ex)
                    {
                        SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "Recipi Tracing",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        //m_DeviceEditLogConn.Dispose();
                        m_DeviceEditLogConn.Close();
                        OleDbConnection.ReleaseObjectPool();
                    }
                }
            }

            STTrackLog.WriteLine("---------- Edit Log 2 : GetDeviceEditLogDataSet Done");
        }

        public static void ReloadDeviceEditLogDataSet(string strDBFile)
        {
            STTrackLog.WriteLine("---------- Edit Log 2 : GetDeviceEditLogDataSet");
            if (m_blnWantEditLog)
            {
                lock (m_objLock)
                {
                    try
                    {
                        m_dsDeviceEditLog.Tables.Clear();
                        m_dsDeviceEditLog.Tables.Add("DeviceEditLog");
                        ReInitDeviceEditLogDataAdapter(strDBFile);

                        //2020-12-30 ZJYEOH : Close connection before DataAdapter use Fill function because :
                        // - The DataAdapter automatically opens and closes a Connection as required if it is not already open when a method such as Fill( ), FillSchema( ), or Update( ) is called.
                        // - The Connection must be explicitly closed if it is already open prior to the DataAdapter operation.
                        m_DeviceEditLogConn.Close();
                        OleDbConnection.ReleaseObjectPool();
                        bool blnColumnChanged = false;

                        STTrackLog.WriteLine("-------------------------------- Edit Log 1000 Check ldb file");
                        string strLDBFileName = strDBFile.Substring(0, strDBFile.LastIndexOf('.')) + ".ldb";
                        if (File.Exists(m_strHistoryDataLocation + "Data\\" + strLDBFileName))
                        {
                            try
                            {
                                STTrackLog.WriteLine("-------------------------------- Edit Log 1001 ldb file Exist");
                                File.Delete(m_strHistoryDataLocation + "Data\\" + strLDBFileName);
                                STTrackLog.WriteLine("-------------------------------- Edit Log 1002 ldb file Deleted");
                            }
                            catch (Exception ex)
                            {
                                STTrackLog.WriteLine("DeviceEdit.cs > GetDeviceEditLogDataSet > exception = " + ex.ToString());
                            }
                        }
                        STTrackLog.WriteLine("--------------------- Edit Log 101 m_daDeviceEditLog Fill");
                        m_daDeviceEditLog.Fill(m_dsDeviceEditLog, "DeviceEditLog");
                        m_DeviceEditLogConn.Close();
                        OleDbConnection.ReleaseObjectPool();
                        STTrackLog.WriteLine("--------------------- Edit Log 102 m_daDeviceEditLog FillSchema");
                        //2020-12-31 ZJYEOH : FillSchema will get the table column properties
                        m_daDeviceEditLog.FillSchema(m_dsDeviceEditLog, SchemaType.Source);

                        //2020-12-31 ZJYEOH : if Max Length less than 255, set the column to 255.  255 is the Max Length of Text for .mdb file
                        if (m_dsDeviceEditLog.Tables[1].Columns[0].MaxLength < 255)
                        {
                            blnColumnChanged = true;
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ALTER COLUMN [UserName] TEXT(255)";

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            OleDbConnection.ReleaseObjectPool();
                        }
                        if (m_dsDeviceEditLog.Tables[1].Columns[1].MaxLength < 255)
                        {
                            blnColumnChanged = true;
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ALTER COLUMN [Group] TEXT(255)";

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            OleDbConnection.ReleaseObjectPool();
                        }
                        if (m_dsDeviceEditLog.Tables[1].Columns[2].MaxLength < 255)
                        {
                            blnColumnChanged = true;
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ALTER COLUMN [Module] TEXT(255)";

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            OleDbConnection.ReleaseObjectPool();
                        }
                        if (m_dsDeviceEditLog.Tables[1].Columns[3].MaxLength < 255)
                        {
                            blnColumnChanged = true;
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ALTER COLUMN [Description] TEXT(255)";

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            OleDbConnection.ReleaseObjectPool();
                        }
                        if (m_dsDeviceEditLog.Tables[1].Columns[4].MaxLength < 255)
                        {
                            blnColumnChanged = true;
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ALTER COLUMN [OriginalValue] TEXT(255)";

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            OleDbConnection.ReleaseObjectPool();
                        }
                        if (m_dsDeviceEditLog.Tables[1].Columns[5].MaxLength < 255)
                        {
                            blnColumnChanged = true;
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ALTER COLUMN [NewValue] TEXT(255)";

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            OleDbConnection.ReleaseObjectPool();
                        }

                        //2020-12-31 ZJYEOH : If Column properties changed then reassign the table again
                        if (blnColumnChanged)
                        {
                            m_dsDeviceEditLog = new DataSet();
                            m_dsDeviceEditLog.Tables.Add("DeviceEditLog");
                            InitDeviceEditLogDataAdapter(strDBFile);
                            m_DeviceEditLogConn.Close();
                            OleDbConnection.ReleaseObjectPool();
                            STTrackLog.WriteLine("--------------------- Edit Log 103 : Fill m_daDeviceEditLog");
                            m_daDeviceEditLog.Fill(m_dsDeviceEditLog, "DeviceEditLog");
                        }

                        //2019-10-11 ZJYEOH : Need to Add another column to access database file(.mdb), because old software access database file(.mdb) dont have this column
                        if (m_dsDeviceEditLog.Tables["DeviceEditLog"].Columns.Count < 8 && m_dsDeviceEditLog.Tables["DeviceEditLog"].Columns.Count != 0)
                        {
                            OleDbConnection myAccessConnection = new OleDbConnection();

                            myAccessConnection.ConnectionString = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                                                                  "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                                                                  ";OLE DB Services = -8;";

                            OleDbCommand sqlCommand = new OleDbCommand();

                            myAccessConnection.Open();
                            sqlCommand.Connection = myAccessConnection;

                            sqlCommand.CommandText = "ALTER TABLE [DeviceEditLog] ADD COLUMN LotID TEXT(255)"; // 2019-10-11 ZJYEOH : SQL_MAX_CHAR_LITERAL_LEN is 255 Only

                            sqlCommand.ExecuteNonQuery();

                            myAccessConnection.Close();
                            m_dsDeviceEditLog.Tables["DeviceEditLog"].Columns.Add("LotID");
                            OleDbConnection.ReleaseObjectPool();
                        }


                    }
                    catch (Exception ex)
                    {
                        SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "Recipi Tracing",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        //m_DeviceEditLogConn.Dispose();
                        m_DeviceEditLogConn.Close();
                        OleDbConnection.ReleaseObjectPool();
                    }
                }
            }

            STTrackLog.WriteLine("---------- Edit Log 2 : GetDeviceEditLogDataSet Done");
        }
        public static void Dispose()
        {
            STTrackLog.WriteLine("---------- Edit Log 3 : Dispose");

            STTrackLog.WriteLine("--------------------- Edit Log 101 m_daDeviceEditLog check null");
            if (m_daDeviceEditLog != null)
            {
                STTrackLog.WriteLine("--------------------- Edit Log 101 m_daDeviceEditLog dispose");
                m_daDeviceEditLog.Dispose();
            }
            if (m_DeviceEditLogConn != null)
                m_DeviceEditLogConn.Close();
            OleDbConnection.ReleaseObjectPool();

            STTrackLog.WriteLine("---------- Edit Log 3 : Dispose Done");
        }
        /// <summary>
        /// Save new device edit log into dataset
        /// </summary>
        /// <param name="strModule">module name</param>
        /// <param name="strDescription">change description</param>
        /// <param name="strOriginalValue">original value</param>
        /// <param name="strNewValue">new value</param>
        public static void SaveDeviceEditLog(string strModule, string strDescription, string strOriginalValue, string strNewValue, string strLotID)
        {
            STTrackLog.WriteLine("---------- Edit Log 4 : SaveDeviceEditLog");

            if (m_blnWantEditLog)
            {
                DataRow newRow = m_dsDeviceEditLog.Tables["DeviceEditLog"].NewRow();

                //2020-12-31 ZJYEOH : Limit Text column to 255 because Max Length allowed is 255 only
                if (m_strUserName.Length > 255)
                    newRow["UserName"] = m_strUserName.Substring(0, 255);
                else
                    newRow["UserName"] = m_strUserName;

                if (m_strGroupName.Length > 255)
                    newRow["Group"] = m_strGroupName.Substring(0, 255);
                else
                    newRow["Group"] = m_strGroupName;

                if (strModule.Length > 255)
                    newRow["Module"] = strModule.Substring(0, 255);
                else
                    newRow["Module"] = strModule;

                if ((m_strDeviceNo + ">" + strDescription).ToString().Length > 255)
                {
                    if (m_strDeviceNo != "")
                        newRow["Description"] = (m_strDeviceNo + ">" + strDescription).ToString().Substring(0, 255);
                    else
                        newRow["Description"] = strDescription.Substring(0, 255);
                }
                else
                {
                    if (m_strDeviceNo != "")
                        newRow["Description"] = m_strDeviceNo + ">" + strDescription;
                    else
                        newRow["Description"] = strDescription;
                }

                if (strOriginalValue.Length > 255)
                    newRow["OriginalValue"] = strOriginalValue.Substring(0, 255);
                else
                    newRow["OriginalValue"] = strOriginalValue;

                if (strNewValue.Length > 255)
                    newRow["NewValue"] = strNewValue.Substring(0, 255);
                else
                    newRow["NewValue"] = strNewValue;
                newRow["ModifiedDate"] = DateTime.Now;

                newRow["LotID"] = strLotID;

                m_dsDeviceEditLog.Tables["DeviceEditLog"].Rows.Add(newRow);

                try
                {
                    if (!Directory.Exists(Directory.GetCurrentDirectory()))
                    {
                        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                    }
                    STTrackLog.WriteLine("------------------------------------------------ Edit Log Data = ");
                    STTrackLog.WriteLine(newRow["UserName"].ToString() + ";" + newRow["Group"].ToString() + ";" + newRow["Module"].ToString() + ";" + newRow["Description"].ToString() + ";" + newRow["OriginalValue"].ToString() + ";" + newRow["NewValue"].ToString() + ";" + newRow["ModifiedDate"].ToString() + ";" + newRow["LotID"].ToString());
                    STTrackLog.WriteLine("--------------------- Edit Log 105 m_daDeviceEditLog Update");
                    m_DeviceEditLogConn.Close();
                    OleDbConnection.ReleaseObjectPool();
                    m_daDeviceEditLog.Update(m_dsDeviceEditLog, "DeviceEditLog");
                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "Recipi Tracing",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    //m_DeviceEditLogConn.Dispose();
                    m_DeviceEditLogConn.Close();
                    OleDbConnection.ReleaseObjectPool();
                }
            }

            STTrackLog.WriteLine("---------- Edit Log 4 : SaveDeviceEditLog Done");
        }

        public static void SaveDeviceEditLogForImage(string strModule, string strDescription, string strOriginalValue, string strNewValue, string strCurrentDateTime, string strLotID)
        {
            STTrackLog.WriteLine("---------- Edit Log 5 : SaveDeviceEditLogForImage");

            if (m_blnWantEditLog)
            {
                DataRow newRow = m_dsDeviceEditLog.Tables["DeviceEditLog"].NewRow();
                newRow["UserName"] = m_strUserName;
                newRow["Group"] = m_strGroupName;
                newRow["Module"] = strModule;
                if (m_strDeviceNo != "")
                    newRow["Description"] = m_strDeviceNo + ">" + strDescription;
                else
                    newRow["Description"] = strDescription;
                newRow["OriginalValue"] = strOriginalValue;
                newRow["NewValue"] = strNewValue;
                newRow["ModifiedDate"] = strCurrentDateTime;
                newRow["LotID"] = strLotID;
                m_dsDeviceEditLog.Tables["DeviceEditLog"].Rows.Add(newRow);

                try
                {
                    if (!Directory.Exists(Directory.GetCurrentDirectory()))
                    {
                        Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                    }
                    STTrackLog.WriteLine("--------------------- Edit Log 106 m_daDeviceEditLog Update");
                    m_DeviceEditLogConn.Close();
                    OleDbConnection.ReleaseObjectPool();
                    m_daDeviceEditLog.Update(m_dsDeviceEditLog, "DeviceEditLog");
                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "Recipi Tracing",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    //m_DeviceEditLogConn.Dispose();
                    m_DeviceEditLogConn.Close();
                    OleDbConnection.ReleaseObjectPool();
                }
            }

            STTrackLog.WriteLine("---------- Edit Log 5 : SaveDeviceEditLogForImage Done");
        }

        public static bool CopySettingFile(string originalFilePath, string originalFileName)
        {
            STTrackLog.WriteLine("---------- Edit Log 6 : CopySettingFile");

            if (m_blnWantEditLog)
            {
                m_originalFile = originalFilePath + originalFileName;

                if (originalFilePath.Contains(".xml"))
                {
                    string str = originalFilePath.Substring(0, originalFilePath.LastIndexOf("\\") + 1);
                    m_tempFile = str + "temp.xml";
                }
                else if (originalFilePath.Contains(".stol"))
                {
                    string str = originalFilePath.Substring(0, originalFilePath.LastIndexOf("\\") + 1);
                    m_tempFile = str + "temp.stol";
                }
                else if (originalFilePath.Contains(".pdf"))
                {
                    string str = originalFilePath.Substring(0, originalFilePath.LastIndexOf("\\") + 1);
                    m_tempFile = str + "temp.pdf";
                }
                else
                    m_tempFile = originalFilePath + "\\temp.xml";
                
                m_recipeName = originalFilePath + originalFileName;
                if (!File.Exists(m_tempFile))
                {
                    XmlDocument original = new XmlDocument();
                    XmlDeclaration xmlDeclaration = original.CreateXmlDeclaration("1.0", "UTF-8", null);
                    XmlElement root = original.DocumentElement;
                    original.InsertBefore(xmlDeclaration, root);

                    //(2) string.Empty makes cleaner code
                    XmlElement element1 = original.CreateElement(string.Empty, "body", string.Empty);
                    original.AppendChild(element1);
                    XmlElement element2 = original.CreateElement(string.Empty, "level1", string.Empty);
                    element1.AppendChild(element2);

                    XmlElement element3 = original.CreateElement(string.Empty, "level2", string.Empty);
                    XmlText text1 = original.CreateTextNode("text");
                    element3.AppendChild(text1);
                    element2.AppendChild(element3);

                    XmlElement element4 = original.CreateElement(string.Empty, "level2", string.Empty);
                    XmlText text2 = original.CreateTextNode("other text");
                    element4.AppendChild(text2);
                    element2.AppendChild(element4);

                    XmlElement element5 = original.CreateElement(string.Empty, "level3", string.Empty);
                    XmlText text3 = original.CreateTextNode("other text");
                    element5.AppendChild(text3);
                    element2.AppendChild(element5);

                    XmlElement element6 = original.CreateElement(string.Empty, "level4", string.Empty);
                    XmlText text4 = original.CreateTextNode("other text");
                    element6.AppendChild(text4);
                    element2.AppendChild(element6);

                    if (Directory.Exists(m_tempFile))
                        original.Save(m_tempFile);

                }

                if (File.Exists(m_originalFile))
                {
                    FileInfo fileInfo = new FileInfo(m_originalFile);
                    fileInfo.CopyTo(m_tempFile, true);
                }
                else
                {
                    STTrackLog.WriteLine("---------- Edit Log 6 : CopySettingFile Done False");

                    // SaveSettingEditLog(m_recipeName, "Create New Settings File", "", originalFileName, DateTime.Now);
                    return false;
                }
            }

            STTrackLog.WriteLine("---------- Edit Log 6 : CopySettingFile Done True");
            return true;

        }
        /// <summary>
        /// Trace if there is any changes in particular module
        /// </summary>
        /// <param name="strModule">module name</param>
        /// <param name="strPath">folder path</param>
        /// <param name="strFileName">file name</param>
        public static void XMLChangesTracing(string strModule, string strLotID)
        {
            STTrackLog.WriteLine("---------- Edit Log 7 : XMLChangesTracing");
            if (m_blnWantEditLog)
            {
                if (!File.Exists(m_originalFile))
                    return;
                if (!File.Exists(m_tempFile))
                    return;
                TrackLog t = new TrackLog();

                //string message = "Changed Value:\n";
                string description = "";   // the description for item
                string description1 = "";
                string description2 = "";
                string description3 = "";
                string description4 = "";
                XmlNodeList newList;    // node list get from updated device no.
                XmlNodeList newListSection1;
                XmlNodeList newListSection2;
                XmlNodeList newListSection3;
                XmlNodeList newListSection4;
                XmlNodeList oriList;
                XmlNodeList oriListSection1;
                XmlNodeList oriListSection2;
                XmlNodeList oriListSection3;
                XmlNodeList oriListSection4;
                XmlElement sectionElement;   // section from the original device no.
                XmlElement sectionElement1;
                XmlElement sectionElement2;
                XmlElement sectionElement3;
                XmlElement sectionElement4;
                XmlNode originalNode;   // node from original device no.
                XmlNode originalNode1;
                XmlNode originalNode2;
                XmlNode originalNode3;
                XmlNode originalNode4;
                XmlNode TempNode;
                DateTime modifiedDate = DateTime.Now;

                XmlDocument originalDoc = new XmlDocument();
                XmlDocument newDoc = new XmlDocument();

                //try
                //{

                //    original.Load(strPath + "temp.xml");
                //    XMLNew.Load(strPath + strFileName );

                //    int intCount = Math.Min(XMLNew.ChildNodes.Count, original.ChildNodes.Count);
                //    for (int i = 0; i < intCount; i++)
                //    {
                //        SearchXMLAttribute(strModule, original.ChildNodes[i], XMLNew.ChildNodes[i]);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    SRMMessageBox.Show("Error: Failed to load XML file.\n\nDetails:\n" + ex.ToString(),
                //        "Recipi Tracing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //}
                try
                {
                    originalDoc.Load(m_tempFile);
                    newDoc.Load(m_originalFile);
                    for (int x = 0; x < newDoc.ChildNodes.Count; x++)
                    {
                        //if (x >= originalDoc.ChildNodes.Count)  // 2019 03 08 - CCENG: newDoc and originalDoc childNodes will be different sometime especially start with new empty recipe.
                        //{
                        //    continue;
                        //}

                        for (int i = 0; i < newDoc.ChildNodes[x].ChildNodes.Count; i++)
                        {
                            //if (i >= originalDoc.ChildNodes[x].ChildNodes.Count)    // 2019 03 08 - CCENG: newDoc and originalDoc childNodes will be different sometime especially start with new empty recipe.
                            //{
                            //    continue;
                            //}
                            // get the section node list from updated device no.
                            newList = newDoc.ChildNodes[x].ChildNodes[i].ChildNodes;
                            oriList = originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes;
                            if (newList.Count > oriList.Count)
                            {

                                for (int j = 0; j < newList.Count; j++)
                                {
                                    if (j >= originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes.Count)    // 2019 03 08 - CCENG: newDoc and originalDoc childNodes will be different sometime especially start with new empty recipe.
                                    {
                                        if (newList[j].InnerXml.Contains("description"))
                                        {
                                            XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                            descAttribute.Value = "";

                                            XmlElement newElement = originalDoc.CreateElement(newList[j].Name);
                                            newElement.SetAttributeNode(descAttribute);
                                            newElement.InnerText = "";
                                            originalDoc.ChildNodes[x].ChildNodes[i].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j]);

                                            originalDoc.Save(m_tempFile);
                                            originalDoc.Load(m_tempFile);
                                        }
                                        //XmlParser fileHandle = new XmlParser(m_tempFile);
                                        //fileHandle.WriteSectionElement(originalDoc.ChildNodes[x].ChildNodes[i].Name);
                                        //fileHandle.WriteElement1Value(newList[j].Name, newList[j].Value);
                                        //fileHandle.WriteEndElement();
                                        else
                                        {
                                            XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                            descAttribute.Value = "";

                                            XmlElement newElement = originalDoc.CreateElement(newList[j].Name);
                                            newElement.SetAttributeNode(descAttribute);
                                            newElement.InnerText = newList[j].InnerText;
                                            originalDoc.ChildNodes[x].ChildNodes[i].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j]);
                                            SaveDeviceEditLog(strModule, "New element added - " + newElement.Name, "", newElement.InnerText, strLotID);
                                            originalDoc.Save(m_tempFile);
                                            originalDoc.Load(m_tempFile);
                                        }
                                        continue;
                                    }
                                    if (originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].Name != newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].Name)//(j > oriList.Count - 1)
                                    {
                                        if (newList[j].InnerXml.Contains("description"))
                                        {
                                            XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                            descAttribute.Value = "";

                                            XmlElement newElement = originalDoc.CreateElement(newList[j].Name);
                                            newElement.SetAttributeNode(descAttribute);
                                            newElement.InnerText = "";
                                            originalDoc.ChildNodes[x].ChildNodes[i].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j]);

                                            originalDoc.Save(m_tempFile);
                                            originalDoc.Load(m_tempFile);
                                        }
                                        // TempNode = newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].Clone();
                                        //originalDoc.ImportNode(newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j],true);
                                        //originalDoc.Save(m_tempFile);
                                        //originalDoc.Load(m_tempFile);
                                        else
                                        {
                                            XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                            descAttribute.Value = "";

                                            XmlElement newElement = originalDoc.CreateElement(newList[j].Name);
                                            newElement.SetAttributeNode(descAttribute);
                                            newElement.InnerText = newList[j].InnerText;
                                            originalDoc.ChildNodes[x].ChildNodes[i].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j]);
                                            SaveDeviceEditLog(strModule, "New element added - " + newElement.Name, "", newElement.InnerText, strLotID);
                                            originalDoc.Save(m_tempFile);
                                            originalDoc.Load(m_tempFile);
                                            //XmlParser fileHandle = new XmlParser(m_tempFile);
                                            //fileHandle.WriteSectionElement(originalDoc.ChildNodes[x].ChildNodes[i].Name);
                                            //fileHandle.WriteElement1Value(newList[j].Name, newList[j].InnerText);
                                            //fileHandle.WriteEndElement();
                                        }
                                    }
                                }

                            }

                            for (int j = 0; j < newDoc.ChildNodes[x].ChildNodes[i].ChildNodes.Count; j++)
                            {
                                //if (j >= originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes.Count)    // 2019 03 08 - CCENG: newDoc and originalDoc childNodes will be different sometime especially start with new empty recipe.
                                //{
                                //    continue;
                                //}
                                // get the section node list from updated device no.
                                newListSection1 = newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes;
                                oriListSection1 = originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes;
                                if (newListSection1.Count > oriListSection1.Count)
                                {

                                    for (int k = 0; k < newListSection1.Count; k++)
                                    {
                                        if (k >= originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes.Count)    // 2019 03 08 - CCENG: newDoc and originalDoc childNodes will be different sometime especially start with new empty recipe.
                                        {
                                            if (newListSection1[k].InnerXml.Contains("description"))
                                            {
                                                XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                                descAttribute.Value = "";
                                                XmlElement newElement = originalDoc.CreateElement(newListSection1[k].Name);
                                                newElement.SetAttributeNode(descAttribute);
                                                newElement.InnerText = "";
                                                originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k]);
                                                originalDoc.Save(m_tempFile);
                                                originalDoc.Load(m_tempFile);
                                                //XmlAttribute descAttribute1 = originalDoc.CreateAttribute("");
                                                //descAttribute.Value = "";
                                                //XmlElement newElement1 = originalDoc.CreateElement(newListSection1[k].Name);
                                                //newElement.SetAttributeNode(descAttribute);
                                                //newElement.InnerText = "";
                                                //originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k]);
                                                //originalDoc.Save(m_tempFile);
                                                //originalDoc.Load(m_tempFile);
                                            }
                                            else
                                            {
                                                XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                                descAttribute.Value = "";
                                                XmlElement newElement = originalDoc.CreateElement(newListSection1[k].Name);
                                                newElement.SetAttributeNode(descAttribute);
                                                newElement.InnerText = newListSection1[k].InnerText;
                                                originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k]);
                                                SaveDeviceEditLog(strModule, "New element added - " + newElement.Name, "", newElement.InnerText, strLotID);
                                                originalDoc.Save(m_tempFile);
                                                originalDoc.Load(m_tempFile);
                                            }
                                            //XmlParser fileHandle = new XmlParser(m_tempFile);
                                            //fileHandle.WriteSectionElement(originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].Name);
                                            //fileHandle.WriteElement2Value(newListSection1[k].Name, newListSection1[k].Value);
                                            //fileHandle.WriteEndElement();
                                            continue;
                                        }
                                        if (originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k].Name != newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k].Name) //(k > oriListSection1.Count - 1)
                                        {

                                            if (newListSection1[k].InnerXml.Contains("description"))
                                            {
                                                XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                                descAttribute.Value = "";
                                                XmlElement newElement = originalDoc.CreateElement(newListSection1[k].Name);
                                                newElement.SetAttributeNode(descAttribute);
                                                newElement.InnerText = "";
                                                originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k]);
                                                originalDoc.Save(m_tempFile);
                                                originalDoc.Load(m_tempFile);
                                                //XmlAttribute descAttribute1 = originalDoc.CreateAttribute("");
                                                //descAttribute.Value = "";
                                                //XmlElement newElement1 = originalDoc.CreateElement(newListSection1[k].Name);
                                                //newElement.SetAttributeNode(descAttribute);
                                                //newElement.InnerText = "";
                                                //originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k]);
                                                //originalDoc.Save(m_tempFile);
                                                //originalDoc.Load(m_tempFile);
                                            }
                                            else
                                            {
                                                XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                                descAttribute.Value = "";

                                                XmlElement newElement = originalDoc.CreateElement(newListSection1[k].Name);
                                                newElement.SetAttributeNode(descAttribute);
                                                newElement.InnerText = newListSection1[k].InnerText;
                                                originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k]);
                                                SaveDeviceEditLog(strModule, "New element added - " + newElement.Name, "", newElement.InnerText, strLotID);
                                                originalDoc.Save(m_tempFile);
                                                originalDoc.Load(m_tempFile);
                                                //XmlParser fileHandle = new XmlParser(m_tempFile);
                                                //fileHandle.WriteSectionElement(originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].Name);
                                                //fileHandle.WriteElement2Value(newListSection1[k].Name, newListSection1[k].Value);
                                                //fileHandle.WriteEndElement();
                                            }
                                        }
                                    }

                                }

                                for (int m = 0; m < newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes.Count; m++)
                                {
                                    //if (m >= originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes.Count)    // 2019 03 08 - CCENG: newDoc and originalDoc childNodes will be different sometime especially start with new empty recipe.
                                    //{
                                    //    continue;
                                    //}
                                    // get the section node list from updated device no.
                                    newListSection2 = newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes;
                                    oriListSection2 = originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes;
                                    if (newListSection2.Count > oriListSection2.Count)
                                    {

                                        for (int n = 0; n < newListSection2.Count; n++)
                                        {
                                            if (n >= originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes.Count)    // 2019 03 08 - CCENG: newDoc and originalDoc childNodes will be different sometime especially start with new empty recipe.
                                            {
                                                if (newListSection2[n].InnerXml.Contains("description"))
                                                {
                                                    XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                                    descAttribute.Value = "";

                                                    XmlElement newElement = originalDoc.CreateElement(newListSection2[n].Name);
                                                    newElement.SetAttributeNode(descAttribute);
                                                    newElement.InnerText = "";
                                                    originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[n]);

                                                    originalDoc.Save(m_tempFile);
                                                    originalDoc.Load(m_tempFile);
                                                }
                                                else
                                                {
                                                    XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                                    descAttribute.Value = "";

                                                    XmlElement newElement = originalDoc.CreateElement(newListSection2[n].Name);
                                                    newElement.SetAttributeNode(descAttribute);
                                                    newElement.InnerText = newListSection2[n].InnerText;
                                                    originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[n]);
                                                    SaveDeviceEditLog(strModule, "New element added - " + newElement.Name, "", newElement.InnerText, strLotID);
                                                    originalDoc.Save(m_tempFile);
                                                    originalDoc.Load(m_tempFile);
                                                }
                                                //XmlParser fileHandle = new XmlParser(m_tempFile);
                                                //fileHandle.WriteSectionElement(originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].Name);
                                                //fileHandle.WriteElement3Value(newListSection2[n].Name, newListSection2[n].Value);
                                                //fileHandle.WriteEndElement();
                                                continue;
                                            }
                                            if (originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[n].Name != newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[n].Name) //(n > oriListSection2.Count - 1)
                                            {
                                                if (newListSection2[n].InnerXml.Contains("description"))
                                                {
                                                    XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                                    descAttribute.Value = "";

                                                    XmlElement newElement = originalDoc.CreateElement(newListSection2[n].Name);
                                                    newElement.SetAttributeNode(descAttribute);
                                                    newElement.InnerText = "";
                                                    originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[n]);

                                                    originalDoc.Save(m_tempFile);
                                                    originalDoc.Load(m_tempFile);
                                                }
                                                else
                                                {
                                                    XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                                    descAttribute.Value = "";

                                                    XmlElement newElement = originalDoc.CreateElement(newListSection2[n].Name);
                                                    newElement.SetAttributeNode(descAttribute);
                                                    newElement.InnerText = newListSection2[n].InnerText;
                                                    originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[n]);
                                                    SaveDeviceEditLog(strModule, "New element added - " + newElement.Name, "", newElement.InnerText, strLotID);
                                                    originalDoc.Save(m_tempFile);
                                                    originalDoc.Load(m_tempFile);
                                                    //XmlParser fileHandle = new XmlParser(m_tempFile);
                                                    //fileHandle.WriteSectionElement(originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].Name);
                                                    //fileHandle.WriteElement3Value(newListSection2[n].Name, newListSection2[n].Value);
                                                    //fileHandle.WriteEndElement();
                                                }
                                            }
                                        }

                                    }

                                    for (int p = 0; p < newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes.Count; p++)
                                    {
                                        //if (p >= originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes.Count)    // 2019 03 08 - CCENG: newDoc and originalDoc childNodes will be different sometime especially start with new empty recipe.
                                        //{
                                        //    continue;
                                        //}
                                        // get the section node list from updated device no.
                                        newListSection3 = newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].ChildNodes;
                                        oriListSection3 = originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].ChildNodes;
                                        if (newListSection3.Count > oriListSection3.Count)
                                        {

                                            for (int q = 0; q < newListSection3.Count; q++)
                                            {
                                                if (q >= originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].ChildNodes.Count)    // 2019 03 08 - CCENG: newDoc and originalDoc childNodes will be different sometime especially start with new empty recipe.
                                                {
                                                    if (newListSection3[q].InnerXml.Contains("description"))
                                                    {
                                                        XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                                        descAttribute.Value = "";

                                                        XmlElement newElement = originalDoc.CreateElement(newListSection3[q].Name);
                                                        newElement.SetAttributeNode(descAttribute);
                                                        newElement.InnerText = "";
                                                        originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].ChildNodes[q]);

                                                        originalDoc.Save(m_tempFile);
                                                        originalDoc.Load(m_tempFile);
                                                    }
                                                    else
                                                    {
                                                        XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                                        descAttribute.Value = "";

                                                        XmlElement newElement = originalDoc.CreateElement(newListSection3[q].Name);
                                                        newElement.SetAttributeNode(descAttribute);
                                                        newElement.InnerText = newListSection3[q].InnerText;
                                                        originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].ChildNodes[q]);
                                                        SaveDeviceEditLog(strModule, "New element added - " + newElement.Name, "", newElement.InnerText, strLotID);
                                                        originalDoc.Save(m_tempFile);
                                                        originalDoc.Load(m_tempFile);
                                                    }
                                                    //XmlParser fileHandle = new XmlParser(m_tempFile);
                                                    //fileHandle.WriteSectionElement(originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].Name);
                                                    //fileHandle.WriteElement4Value(newListSection3[q].Name, newListSection3[q].Value);
                                                    //fileHandle.WriteEndElement();
                                                    continue;
                                                }
                                                if (originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].ChildNodes[q].Name != newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].ChildNodes[q].Name) //(q > oriListSection3.Count - 1)
                                                {
                                                    if (q >= originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].ChildNodes.Count)    // 2019 03 08 - CCENG: newDoc and originalDoc childNodes will be different sometime especially start with new empty recipe.
                                                    {
                                                        if (newListSection3[q].InnerXml.Contains("description"))
                                                        {
                                                            XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                                            descAttribute.Value = "";

                                                            XmlElement newElement = originalDoc.CreateElement(newListSection3[q].Name);
                                                            newElement.SetAttributeNode(descAttribute);
                                                            newElement.InnerText = "";
                                                            originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].ChildNodes[q]);

                                                            originalDoc.Save(m_tempFile);
                                                            originalDoc.Load(m_tempFile);
                                                        }
                                                        else
                                                        {
                                                            XmlAttribute descAttribute = originalDoc.CreateAttribute("description");
                                                            descAttribute.Value = "";

                                                            XmlElement newElement = originalDoc.CreateElement(newListSection3[q].Name);
                                                            newElement.SetAttributeNode(descAttribute);
                                                            newElement.InnerText = newListSection3[q].InnerText;
                                                            originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].InsertBefore(newElement, originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].ChildNodes[q]);

                                                            //if(newListSection3[q].NodeType==XmlNodeType.Element)
                                                            //originalDoc.DocumentElement.AppendChild(newListSection3[q]);
                                                            //else
                                                            SaveDeviceEditLog(strModule, "New element added - " + newElement.Name, "", newElement.InnerText, strLotID);
                                                            originalDoc.Save(m_tempFile);
                                                            originalDoc.Load(m_tempFile);
                                                        }
                                                        //XmlParser fileHandle = new XmlParser(m_tempFile);
                                                        //fileHandle.WriteSectionElement(originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[m].ChildNodes[p].Name);
                                                        //fileHandle.WriteElement4Value(newListSection3[q].Name, newListSection3[q].Value);
                                                        //fileHandle.WriteEndElement();
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }

                    originalDoc.Load(m_tempFile);
                    newDoc.Load(m_originalFile);
                    for (int x = 0; x < newDoc.ChildNodes.Count; x++)
                    {
                        for (int i = 0; i < newDoc.ChildNodes[x].ChildNodes.Count; i++)
                        {
                            // get the section node list from updated device no.
                            newList = newDoc.ChildNodes[x].ChildNodes[i].ChildNodes;

                            //if (originalDoc.ChildNodes[x].ChildNodes.Count > i)
                            //{
                            // get the section element from the original device no.
                            sectionElement = originalDoc.DocumentElement[originalDoc.ChildNodes[x].ChildNodes[i].Name];
                            for (int j = 0; j < newList.Count; j++)
                            {

                                // get the original device node that have same item with new device node
                                originalNode = sectionElement.SelectSingleNode(newList[j].Name);

                                //t.WriteLine("New :" + newList[j].Name + " - " + newList[j].InnerText);
                                //t.WriteLine("Temp :" + originalNode.Name + " - " + originalNode.InnerText);
                                //t.WriteLine(newList[j].Attributes.Count.ToString());
                                if (newList[j].Attributes.Count > 0)
                                {
                                    // get the description of the node
                                    if (newList[j].ChildNodes.Count > 1)
                                        description = newList[j].Attributes["description"].Value;
                                    else
                                        description = newList[j].InnerText;
                                    //t.WriteLine("description = " + description);
                                    // if the description exist, get the value
                                    if ((description != null) && (description != ""))
                                    {
                                        //if (originalNode == null)
                                        //{
                                        //    // new add item
                                        //    SaveDeviceEditLog(strModule, newList[j].Attributes["description"].Value, "-", newList[j].InnerText);
                                        //}
                                        //else
                                        //{
                                        //    if (newList[j].InnerText != originalNode.InnerText)
                                        //    {
                                        //        SaveDeviceEditLog(strModule, newList[j].Attributes["description"].Value, originalNode.InnerText, newList[j].InnerText);

                                        //    }
                                        //}
                                        if (newList[j].InnerText != originalNode.InnerText)
                                        {
                                            //2020-12-29 ZJYEOH : Added description for easier tracking --> Attributes["description"].Value
                                            SaveDeviceEditLog(strModule + ">" + newList[j].Attributes["description"].Value, "value of " + newList[j].Name + " changed", originalNode.InnerText, newList[j].InnerText, strLotID);
                                        }
                                    }
                                    else
                                    {
                                        if (newList[j].HasChildNodes)
                                        {
                                            newListSection1 = newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes;
                                            oriListSection1 = originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes;
                                            for (int k = 0; k < newListSection1.Count; k++)
                                            {
                                                // originalNode1 = oriListSection1.(newListSection1[k].Name);

                                                if (newListSection1[k].Attributes.Count > 0)
                                                {
                                                    // get the description of the node
                                                    if (newListSection1[k].ChildNodes.Count > 1)
                                                        description1 = newListSection1[k].Attributes["description"].Value;
                                                    else
                                                        description1 = newListSection1[k].InnerText;


                                                    //t.WriteLine("description = " + description);
                                                    // if the description exist, get the value
                                                    if ((description1 != null) && (description1 != ""))
                                                    {
                                                        //    if (originalNode1 == null)
                                                        //    {
                                                        //        // new add item
                                                        //        SaveDeviceEditLog(strModule, newListSection1[k].Attributes["description"].Value, "-", newListSection1[k].InnerText);
                                                        //    }
                                                        //    else
                                                        //    {
                                                        //        if (newListSection1[k].InnerText != originalNode1.InnerText)
                                                        //        {
                                                        //            SaveDeviceEditLog(strModule, newListSection1[k].Attributes["description"].Value, originalNode1.InnerText, newListSection1[k].InnerText);

                                                        //        }
                                                        //    }
                                                        if (newListSection1[k].InnerText != oriListSection1[k].InnerText)
                                                        {
                                                            //2020-12-29 ZJYEOH : Added description for easier tracking --> Attributes["description"].Value
                                                            SaveDeviceEditLog(strModule + ">" + newListSection1[k].Attributes["description"].Value, "value of " + newListSection1[k].Name + " changed", oriListSection1[k].InnerText, newListSection1[k].InnerText, strLotID);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (newListSection1[k].HasChildNodes)
                                                        {
                                                            newListSection2 = newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k].ChildNodes;
                                                            oriListSection2 = originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k].ChildNodes;
                                                            for (int m = 0; m < newListSection2.Count; m++)
                                                            {
                                                                // originalNode1 = oriListSection1.(newListSection1[k].Name);

                                                                if (newListSection2[m].Attributes.Count > 0)
                                                                {
                                                                    // get the description of the node
                                                                    if (newListSection2[m].ChildNodes.Count > 1)
                                                                        description2 = newListSection2[m].Attributes["description"].Value;
                                                                    else
                                                                        description2 = newListSection2[m].InnerText;


                                                                    //t.WriteLine("description = " + description);
                                                                    // if the description exist, get the value
                                                                    if ((description2 != null) && (description2 != ""))
                                                                    {
                                                                        //    if (originalNode1 == null)
                                                                        //    {
                                                                        //        // new add item
                                                                        //        SaveDeviceEditLog(strModule, newListSection1[k].Attributes["description"].Value, "-", newListSection1[k].InnerText);
                                                                        //    }
                                                                        //    else
                                                                        //    {
                                                                        //        if (newListSection1[k].InnerText != originalNode1.InnerText)
                                                                        //        {
                                                                        //            SaveDeviceEditLog(strModule, newListSection1[k].Attributes["description"].Value, originalNode1.InnerText, newListSection1[k].InnerText);

                                                                        //        }
                                                                        //    }
                                                                        if (newListSection2[m].InnerText != oriListSection2[m].InnerText)
                                                                        {
                                                                            //2020-12-29 ZJYEOH : Added description for easier tracking --> Attributes["description"].Value
                                                                            SaveDeviceEditLog(strModule + ">" + newListSection2[m].Attributes["description"].Value, "value of " + newListSection2[m].Name + " changed", oriListSection2[m].InnerText, newListSection2[m].InnerText, strLotID);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (newListSection2[m].HasChildNodes)
                                                                        {
                                                                            newListSection3 = newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k].ChildNodes[m].ChildNodes;
                                                                            oriListSection3 = originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k].ChildNodes[m].ChildNodes;
                                                                            for (int n = 0; n < newListSection3.Count; n++)
                                                                            {
                                                                                // originalNode1 = oriListSection1.(newListSection1[k].Name);

                                                                                if (newListSection3[n].Attributes.Count > 0)
                                                                                {
                                                                                    // get the description of the node
                                                                                    if (newListSection3[n].ChildNodes.Count > 1)
                                                                                        description3 = newListSection3[n].Attributes["description"].Value;
                                                                                    else
                                                                                        description3 = newListSection3[n].InnerText;


                                                                                    //t.WriteLine("description = " + description);
                                                                                    // if the description exist, get the value
                                                                                    if ((description3 != null) && (description3 != ""))
                                                                                    {
                                                                                        //    if (originalNode1 == null)
                                                                                        //    {
                                                                                        //        // new add item
                                                                                        //        SaveDeviceEditLog(strModule, newListSection1[k].Attributes["description"].Value, "-", newListSection1[k].InnerText);
                                                                                        //    }
                                                                                        //    else
                                                                                        //    {
                                                                                        //        if (newListSection1[k].InnerText != originalNode1.InnerText)
                                                                                        //        {
                                                                                        //            SaveDeviceEditLog(strModule, newListSection1[k].Attributes["description"].Value, originalNode1.InnerText, newListSection1[k].InnerText);

                                                                                        //        }
                                                                                        //    }
                                                                                        if (newListSection3[n].InnerText != oriListSection3[n].InnerText)
                                                                                        {
                                                                                            //2020-12-29 ZJYEOH : Added description for easier tracking --> Attributes["description"].Value
                                                                                            SaveDeviceEditLog(strModule + ">" + newListSection3[n].Attributes["description"].Value, "value of " + newListSection3[n].Name + " changed", oriListSection3[n].InnerText, newListSection3[n].InnerText, strLotID);
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (newListSection3[n].HasChildNodes)
                                                                                        {
                                                                                            newListSection4 = newDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k].ChildNodes[m].ChildNodes[n].ChildNodes;
                                                                                            oriListSection4 = originalDoc.ChildNodes[x].ChildNodes[i].ChildNodes[j].ChildNodes[k].ChildNodes[m].ChildNodes[n].ChildNodes;
                                                                                            for (int p = 0; p < newListSection4.Count; p++)
                                                                                            {
                                                                                                // originalNode1 = oriListSection1.(newListSection1[k].Name);

                                                                                                if (newListSection4[p].Attributes.Count > 0)
                                                                                                {
                                                                                                    // get the description of the node
                                                                                                    if (newListSection4[p].ChildNodes.Count > 1)
                                                                                                        description4 = newListSection4[p].Attributes["description"].Value;
                                                                                                    else
                                                                                                        description4 = newListSection4[p].InnerText;


                                                                                                    //t.WriteLine("description = " + description);
                                                                                                    // if the description exist, get the value
                                                                                                    if ((description4 != null) && (description4 != ""))
                                                                                                    {
                                                                                                        //    if (originalNode1 == null)
                                                                                                        //    {
                                                                                                        //        // new add item
                                                                                                        //        SaveDeviceEditLog(strModule, newListSection1[k].Attributes["description"].Value, "-", newListSection1[k].InnerText);
                                                                                                        //    }
                                                                                                        //    else
                                                                                                        //    {
                                                                                                        //        if (newListSection1[k].InnerText != originalNode1.InnerText)
                                                                                                        //        {
                                                                                                        //            SaveDeviceEditLog(strModule, newListSection1[k].Attributes["description"].Value, originalNode1.InnerText, newListSection1[k].InnerText);

                                                                                                        //        }
                                                                                                        //    }
                                                                                                        if (newListSection4[p].InnerText != oriListSection4[p].InnerText)
                                                                                                        {
                                                                                                            //2020-12-29 ZJYEOH : Added description for easier tracking --> Attributes["description"].Value
                                                                                                            SaveDeviceEditLog(strModule + ">" + newListSection4[p].Attributes["description"].Value, "value of " + newListSection4[p].Name + " changed", oriListSection4[p].InnerText, newListSection4[p].InnerText, strLotID);
                                                                                                        }
                                                                                                    }


                                                                                                }

                                                                                            }
                                                                                        }
                                                                                    }

                                                                                }

                                                                            }
                                                                        }
                                                                    }


                                                                }

                                                            }
                                                        }
                                                    }

                                                }

                                            }
                                        }
                                    }

                                }



                            }
                            //}
                            //else
                            //{ // new attributes add in 
                            //    for (int j = 0; j < newList.Count; j++)
                            //    {
                            //        if (newList[j].Attributes.Count > 0)
                            //        {
                            //            // get the description of the node
                            //            description = newList[j].Attributes["description"].Value;
                            //            // if the description exist, get the value
                            //            if ((description != null) && (description != ""))
                            //            {
                            //                // new add item
                            //                SaveDeviceEditLog(strModule, newList[j].Attributes["description"].Value, "-", newList[j].InnerText);

                            //            }
                            //        }
                            //    }
                            //}
                        }
                    }
                    if (File.Exists(m_tempFile))
                        File.Delete(m_tempFile);    // Delete temporary file after use
                }
                catch (Exception ex)
                {

                    if (File.Exists(m_tempFile))
                        File.Delete(m_tempFile);

                    //SRMMessageBox.Show("Error: Failed to trace " + m_originalFile + " file changes.\n\nDetails:\n" + ex.ToString(),
                    //    "Setting Tracing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            STTrackLog.WriteLine("---------- Edit Log 7 : XMLChangesTracing Done");
        }



        /// <summary>
        /// Initialize device edit log and fill in the device edit log data
        /// </summary>
        private static void Init()
        {
            STTrackLog.WriteLine("---------- Edit Log 8 : Init");

            if (m_blnWantEditLog)
            {
                STTrackLog.WriteLine("---------- Edit Log 8 : Init Want Edit Log");

                // 2020-03-30 ZJYEOH : Now Data will save in selected Disk Partition Location
                if (!Directory.Exists(m_strHistoryDataLocation + "Data\\"))
                {
                    Directory.CreateDirectory(m_strHistoryDataLocation + "Data\\");
                }

                string[] arrstrFiles = Directory.GetFiles(m_strHistoryDataLocation + "Data\\");

                //2020-03-30 ZJYEOH : Copy all files in C:\SVG\Data to selected Disk Partition Location
                if (arrstrFiles.Length == 0)
                {
                    CopyFiles objCopy = new CopyFiles();
                    objCopy.CopyAllFiles(AppDomain.CurrentDomain.BaseDirectory + "Data\\", m_strHistoryDataLocation + "Data\\");
                }

                //2020-03-30 ZJYEOH : Delete all files in C:\SVG\Data
                string[] arrstrSVGDirectories = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "Data\\");
                string[] arrstrSVGFiles = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Data\\");
                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Data\\"))
                {
                    foreach (string strPath in arrstrSVGDirectories)
                    {
                        // 2020 05 23 - CCENG: sometime not able to delete the folder and error "Directory is not empty" happen.
                        try
                        {
                            Directory.Delete(strPath, true);
                        }
                        catch (Exception ex)
                        {
                            STTrackLog.WriteLine("DeviceEdit > Init() > ex = " + ex.ToString());
                        }
                    }
                    foreach (string strFile in arrstrSVGFiles)
                    {
                        if (!strFile.Contains("History.mdb"))
                            File.Delete(strFile);
                    }

                }

                int intMonth = DateTime.Now.Month;
                int intYear = DateTime.Now.Year;
                string strMonthText = "";

                if (intMonth < 10)
                    strMonthText = "0" + intMonth.ToString();
                else
                    strMonthText = intMonth.ToString();

                m_strCurrentMonthDB = intYear.ToString() + "-" + strMonthText + ".mdb";
                if (!File.Exists(m_strHistoryDataLocation + "Data\\" + m_strCurrentMonthDB))
                    NewDatabase();

                GetDeviceEditLogDataSet(m_strCurrentMonthDB);
            }

            STTrackLog.WriteLine("---------- Edit Log 8 : Init Done");
        }

        /// <summary>
        /// Get the device edit data from paticular database file and store into data adapter
        /// </summary>
        /// <param name="strDBFile">database file name (.mdb)</param>
        private static void InitDeviceEditLogDataAdapter(string strDBFile)
        {
            STTrackLog.WriteLine("---------- Edit Log 9 : InitDeviceEditLogDataAdapter");

            if (m_blnWantEditLog)
            {
                STTrackLog.WriteLine("---------- Edit Log 9 : InitDeviceEditLogDataAdapter Want Edit Log");

                string strConnection = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                ";OLE DB Services = -8;";
                //2020-12-28 ZJYEOH : Disable pooling services solved the access protected memory error
                // Services enabled                                                     |   Value in connection string
                //-----------------------------------------------------------------------------------------------------------------------
                // All services(the default)                                            |   "OLE DB Services = -1;"
                // All services except pooling                                          |   "OLE DB Services = -2;"
                // All services except pooling and auto - enlistment                    |   "OLE DB Services = -4;"
                // All services except client cursor                                    |   "OLE DB Services = -5;"
                // All services except client cursor and pooling                        |   "OLE DB Services = -6;"
                // All services except auto - enlistment and client cursor              |   "OLE DB Services = -7;"
                // All services except pooling and auto - enlistment and client cursor  |   "OLE DB Services = -8;"
                // No services                                                          |   "OLE DB Services = 0;"

                if (!Directory.Exists(Directory.GetCurrentDirectory()))
                {
                    Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                }
                OleDbConnection.ReleaseObjectPool();
                OleDbParameter DeviceEditLogWorkParam = new OleDbParameter();
                m_DeviceEditLogConn = new OleDbConnection(strConnection);
                m_DeviceEditLogConn.Open();
                //m_DeviceEditLogConn.Close();
                //m_DeviceEditLogConn.Dispose();
                string strSQLSelect = "SELECT * FROM DeviceEditLog ORDER BY [ModifiedDate] DESC";
                OleDbCommand dbCommand = new OleDbCommand(strSQLSelect, m_DeviceEditLogConn);

                STTrackLog.WriteLine("--------------------- Edit Log 100 : new m_daDeviceEditLog");
                m_daDeviceEditLog = new OleDbDataAdapter(dbCommand);

                string strSQLInsert = "INSERT INTO DeviceEditLog ([UserName], [Group], [Module], [Description], [OriginalValue], " +
                    "[NewValue], [ModifiedDate], [LotID]) VALUES (@UserName , @Group, @Module, @Description, @OriginalValue," +
                    "@NewValue, @ModifiedDate, @LotID)";
                m_daDeviceEditLog.InsertCommand = new OleDbCommand(strSQLInsert, m_DeviceEditLogConn);

                // Insert Command: Build parameters for each column in table
                DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@UserName", OleDbType.VarChar, 255));
                DeviceEditLogWorkParam.SourceColumn = "UserName";
                DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.VarChar, 255));
                DeviceEditLogWorkParam.SourceColumn = "Group";
                DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@Module", OleDbType.VarChar, 255));
                DeviceEditLogWorkParam.SourceColumn = "Module";
                DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@Description", OleDbType.VarChar, 255));
                DeviceEditLogWorkParam.SourceColumn = "Description";
                DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@OriginalValue", OleDbType.VarChar, 255));
                DeviceEditLogWorkParam.SourceColumn = "OriginalValue";
                DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@NewValue", OleDbType.VarChar, 255));
                DeviceEditLogWorkParam.SourceColumn = "NewValue";
                DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@ModifiedDate", OleDbType.Date));
                DeviceEditLogWorkParam.SourceColumn = "ModifiedDate";
                DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@LotID", OleDbType.VarChar, 255));
                DeviceEditLogWorkParam.SourceColumn = "LotID";
                DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;
            }

            STTrackLog.WriteLine("---------- Edit Log 9 : InitDeviceEditLogDataAdapter Done");
        }

        private static void ReInitDeviceEditLogDataAdapter(string strDBFile)
        {
            STTrackLog.WriteLine("---------- Edit Log 9 : InitDeviceEditLogDataAdapter");

            if (m_blnWantEditLog)
            {
                STTrackLog.WriteLine("---------- Edit Log 9 : InitDeviceEditLogDataAdapter Want Edit Log");

                string strConnection = "Provider = Microsoft.ACE.OLEDB.12.0;" +
                "Data Source = " + m_strHistoryDataLocation + "Data\\" + strDBFile +
                ";OLE DB Services = -8;";
                //2020-12-28 ZJYEOH : Disable pooling services solved the access protected memory error
                // Services enabled                                                     |   Value in connection string
                //-----------------------------------------------------------------------------------------------------------------------
                // All services(the default)                                            |   "OLE DB Services = -1;"
                // All services except pooling                                          |   "OLE DB Services = -2;"
                // All services except pooling and auto - enlistment                    |   "OLE DB Services = -4;"
                // All services except client cursor                                    |   "OLE DB Services = -5;"
                // All services except client cursor and pooling                        |   "OLE DB Services = -6;"
                // All services except auto - enlistment and client cursor              |   "OLE DB Services = -7;"
                // All services except pooling and auto - enlistment and client cursor  |   "OLE DB Services = -8;"
                // No services                                                          |   "OLE DB Services = 0;"

                if (!Directory.Exists(Directory.GetCurrentDirectory()))
                {
                    Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                }
                OleDbConnection.ReleaseObjectPool();
               // OleDbParameter DeviceEditLogWorkParam = new OleDbParameter();
                //m_DeviceEditLogConn = new OleDbConnection(strConnection);
                m_DeviceEditLogConn.Open();
                //m_DeviceEditLogConn.Close();
                //m_DeviceEditLogConn.Dispose();
                string strSQLSelect = "SELECT * FROM DeviceEditLog ORDER BY [ModifiedDate] DESC";
                OleDbCommand dbCommand = new OleDbCommand(strSQLSelect, m_DeviceEditLogConn);

                STTrackLog.WriteLine("--------------------- Edit Log 100 : new m_daDeviceEditLog");
                //m_daDeviceEditLog = new OleDbDataAdapter(dbCommand);

                string strSQLInsert = "INSERT INTO DeviceEditLog ([UserName], [Group], [Module], [Description], [OriginalValue], " +
                    "[NewValue], [ModifiedDate], [LotID]) VALUES (@UserName , @Group, @Module, @Description, @OriginalValue," +
                    "@NewValue, @ModifiedDate, @LotID)";
                m_daDeviceEditLog.InsertCommand = new OleDbCommand(strSQLInsert, m_DeviceEditLogConn);

                // Insert Command: Build parameters for each column in table
                //DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@UserName", OleDbType.VarChar, 255));
                //DeviceEditLogWorkParam.SourceColumn = "UserName";
                //DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                //DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@Group", OleDbType.VarChar, 255));
                //DeviceEditLogWorkParam.SourceColumn = "Group";
                //DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                //DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@Module", OleDbType.VarChar, 255));
                //DeviceEditLogWorkParam.SourceColumn = "Module";
                //DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                //DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@Description", OleDbType.VarChar, 255));
                //DeviceEditLogWorkParam.SourceColumn = "Description";
                //DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                //DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@OriginalValue", OleDbType.VarChar, 255));
                //DeviceEditLogWorkParam.SourceColumn = "OriginalValue";
                //DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                //DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@NewValue", OleDbType.VarChar, 255));
                //DeviceEditLogWorkParam.SourceColumn = "NewValue";
                //DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                //DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@ModifiedDate", OleDbType.Date));
                //DeviceEditLogWorkParam.SourceColumn = "ModifiedDate";
                //DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;

                //DeviceEditLogWorkParam = m_daDeviceEditLog.InsertCommand.Parameters.Add(new OleDbParameter("@LotID", OleDbType.VarChar, 255));
                //DeviceEditLogWorkParam.SourceColumn = "LotID";
                //DeviceEditLogWorkParam.SourceVersion = DataRowVersion.Current;
            }

            STTrackLog.WriteLine("---------- Edit Log 9 : InitDeviceEditLogDataAdapter Done");
        }

        /// <summary>
        /// Copy default database file to current year month database file and overwrite the existing file
        /// </summary>
        private static void NewDatabase()
        {
            STTrackLog.WriteLine("---------- Edit Log 10 : NewDatabase");
            if (m_blnWantEditLog)
            {
                STTrackLog.WriteLine("---------- Edit Log 10 : NewDatabase Want Edit");

                string strDefaultFile = AppDomain.CurrentDomain.BaseDirectory + "Data\\History.mdb";
                string strNewFile = m_strHistoryDataLocation + "Data\\" + m_strCurrentMonthDB;
                FileInfo fDefault = new FileInfo(strDefaultFile);
                fDefault.CopyTo(strNewFile, true);
            }

            STTrackLog.WriteLine("---------- Edit Log 10 : NewDatabase Done");
        }

        /// <summary>
        /// Search through xml to trace if there is any changes, if have changes, save into device edit log
        /// </summary>
        /// <param name="strModule">module name</param>
        /// <param name="ExNode">original node</param>
        /// <param name="NewNode">new node</param>
        private static void SearchXMLAttribute(string strModule, XmlNode ExNode, XmlNode NewNode, string strLotID)
        {
            STTrackLog.WriteLine("---------- Edit Log 11 : SearchXMLAttribute");
            if (m_blnWantEditLog)
            {
                STTrackLog.WriteLine("---------- Edit Log 11 : SearchXMLAttribute Want Edit");
                if (NewNode.ChildNodes.Count > 0 && ExNode.ChildNodes.Count > 0)
                {
                    if (NewNode.ChildNodes[0].HasChildNodes && ExNode.ChildNodes[0].HasChildNodes)
                    {
                        int intCount = Math.Min(NewNode.ChildNodes.Count, ExNode.ChildNodes.Count);
                        for (int i = 0; i < intCount; i++)
                        {
                            SearchXMLAttribute(strModule, ExNode.ChildNodes[i], NewNode.ChildNodes[i], strLotID);
                        }
                    }
                }
                else
                {

                    if (ExNode.Name == NewNode.Name && ExNode.InnerText != NewNode.InnerText && NewNode.InnerText != "")
                        SaveDeviceEditLog(strModule, NewNode.Attributes["Description"].Value.ToString(), ExNode.InnerText, NewNode.InnerText, strLotID);
                }
            }

            STTrackLog.WriteLine("---------- Edit Log 11 : SearchXMLAttribute Done");
        }

    }
}
