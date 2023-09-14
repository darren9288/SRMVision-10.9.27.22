using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;

namespace Common
{
    public class DBCall
    {
        public static string m_strSVGDBPath = "C:\\SVG\\SVGDatabase\\";
        public static string m_strSVGDeviceNoPath = "C:\\DeviceNo\\";
        public static string m_strSVGSaveImagePath = "C:\\SaveImage\\";
        public static string m_strSVGTrackLogPath = "C:\\LogFile\\";
        public static string m_strSRMPath = "C:\\SRM\\";
        public static string m_strSVGCasePath = "C:\\Case\\";

        #region Member Variables

        private bool m_blnSucceed = true;
        private OleDbConnection m_oledbConnection;
        private OleDbCommand m_oledbCommand;
        private OleDbDataAdapter m_oledbAdapter;

        #endregion

        public DBCall(string strDBName)
        {
            m_oledbConnection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0; " +
                    "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + strDBName);
        }

        /// <summary>
        /// retrieve data from database to dataset
        /// </summary>
        /// <param name="strSQL">SQL command to execute</param>
        /// <param name="dsSelect">dataset name</param>
        /// <returns>true = success in execution, false = otherwise</returns>
        public bool Select(string strSQL, DataSet dsSelect)
        {
            m_oledbAdapter = new OleDbDataAdapter(strSQL, m_oledbConnection);
            Fill(dsSelect);

            return m_blnSucceed;
        }
        /// <summary>
        /// retrieve data from database to datatable
        /// </summary>
        /// <param name="strSQL">SQL command to execute</param>
        /// <param name="dsSelect">dataset name</param>
        /// <param name="strTableName">datatable name</param>
        /// <returns>true = success in execution, false = otherwise</returns>
        public bool Select(string strSQL, DataSet dsSelect, string strTableName)
        {
            m_oledbAdapter = new OleDbDataAdapter(strSQL, m_oledbConnection);
            Fill(dsSelect, strTableName);

            return m_blnSucceed;
        }
        /// <summary>
        /// Insert data into database
        /// </summary>
        /// <param name="strSQL">SQL command to execute</param>
        /// <returns>true = success in execution, false = otherwise</returns>
        public bool Insert(string strSQL)
        {
            m_oledbCommand = new OleDbCommand(strSQL, m_oledbConnection);
            Execute();

            return m_blnSucceed;
        }
        /// <summary>
        /// change record in database
        /// </summary>
        /// <param name="strSQL">SQL command to execute</param>
        /// <returns>true = success in execution, false = otherwise</returns>
        public bool Update(string strSQL)
        {
            m_oledbCommand = new OleDbCommand(strSQL, m_oledbConnection);
            Execute();

            return m_blnSucceed;
        }
        /// <summary>
        /// Delete record from database
        /// </summary>
        /// <param name="strSQL">SQL command to execute</param>
        /// <returns>true = success in execution, false = otherwise</returns>
        public bool Delete(string strSQL)
        {
            m_oledbCommand = new OleDbCommand(strSQL, m_oledbConnection);
            Execute();

            return m_blnSucceed;
        }
        /// <summary>
        /// establish database connection
        /// </summary>
        /// <returns>true if connection is success, false = otherwise</returns>
        private bool OpenConnection()
        {
            try
            {
                m_oledbConnection.Open();
                return true;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Error: Failed to establish the database connection.\n\nDetails:\n" +
                    ex.ToString(), "DBCall", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        /// <summary>
        /// Close database connection
        /// </summary>
        /// <returns>true = connection is broken successfully, false = otherwise</returns>
        private bool CloseConnection()
        {
            try
            {
                m_oledbConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Error: Failed to close the database connection.\n\nDetails:\n" +
                    ex.ToString(), "DBCall", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        /// <summary>
        /// Fill retrieve record from database to dataset
        /// </summary>
        /// <param name="ds">dataset name</param>
        private void Fill(DataSet ds)
        {
            if (OpenConnection())
            {
                try
                {
                    m_oledbAdapter.Fill(ds);
                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Error:\n" + ex.ToString(), "DBCall",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    m_blnSucceed = false;
                }
                finally
                {
                    CloseConnection();
                }
            }
        }
        /// <summary>
        /// Fill retrieve record from database to datatable
        /// </summary>
        /// <param name="ds">dataset name</param>
        /// <param name="tableName">datatable name</param>
        private void Fill(DataSet ds, string tableName)
        {
            if (OpenConnection())
            {
                try
                {
                    m_oledbAdapter.Fill(ds, tableName);
                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Error:\n" + ex.ToString(), "DBCall",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    m_blnSucceed = false;
                }
                finally
                {
                    CloseConnection();
                }
            }
        }
        /// <summary>
        /// Start execute to command
        /// </summary>
        private void Execute()
        {
            if (OpenConnection())
            {
                try
                {
                    m_oledbCommand.ExecuteReader();
                }
                catch (Exception ex)
                {
                    SRMMessageBox.Show("Error:\n" + ex.ToString(), "DBCall",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    m_blnSucceed = false;
                }
                finally
                {
                    CloseConnection();
                }
            }
        }
    }
}
