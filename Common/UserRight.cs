using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;

namespace Common
{
	/// <summary>
	/// Summary description for UserMainObj.
	/// </summary>
	public class UserRight
	{
		#region Members Variables

        private DataSet m_group2DataSet = new DataSet();
        private DataSet m_group3DataSet = new DataSet();
        private DBCall m_dbCall = new DBCall(@"access\setting.mdb");

		#endregion

        public UserRight()
        {
            m_dbCall.Select("SELECT * FROM 1Child", m_group2DataSet);
            m_dbCall.Select("SELECT * FROM Setting", m_group3DataSet);
        }

        public int[] GetGroupLevel1()
        {
            int[] groupList = new int[1];
            DataSet rightDataSet = new DataSet();
            m_dbCall.Select("SELECT * FROM Rights ORDER BY Number", rightDataSet);
            int rowCount = rightDataSet.Tables[0].Rows.Count;
            if (rowCount > 0)
            {
                groupList = new int[rowCount + 1];
                for (int i = 1; i < rowCount + 1; i++)
                    groupList[i] = Convert.ToInt32(rightDataSet.Tables[0].Rows[i - 1]["Group"]);
            }

            return groupList;
        }

        public int GetGroupLevel2(string parent, string child1)
        {
            int group = 1;
            try
            {
                string filter = "[Parent] = '" + parent + "' AND Name = '" + child1 + "'";
                DataRow[] rightRows = m_group2DataSet.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }

        public int GetGroupLevel3(string child1, string child2)
		{
            int group = 1;
            try
            {
                string filter = "[1Child Name] = '" + child1 + "' AND Name = '" + child2 + "'";
                DataRow[] rightRows = m_group3DataSet.Tables[0].Select(filter);
                foreach (DataRow row in rightRows)
                    group = Convert.ToInt32(row["Group"]);
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Exception Error: \n" + ex.ToString(), "User Right",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return group;
        }
    }
}
