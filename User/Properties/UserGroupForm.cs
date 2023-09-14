using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Common;
using SharedMemory;

namespace User
{
    public partial class UserGroupForm : Form
    {
        #region Members Variables

        private int m_groupID;
        private string m_group;
        private DataSet m_userDataSet;
        private DataSet m_groupDataSet;

        #endregion

        public UserGroupForm(string group, CustomOption option)
        {
            InitializeComponent();
            
            m_group = group;
            m_groupID = 0;

            GetGroupDataSet();
            UpdateUserListView();
        }

        private void GetGroupDataSet()
        {
            m_userDataSet = new DataSet();
            m_groupDataSet = new DataSet();

            DBCall dbCall = new DBCall(@"access\user.mdb");
            dbCall.Select("SELECT * FROM Users", m_userDataSet);
            dbCall.Select("SELECT * FROM Groups", m_groupDataSet);
        }

        private void UpdateUserListView()
        {
            DataRow[] groupList = m_groupDataSet.Tables[0].Select("Name = '" + m_group + "'", "Name");
            foreach (DataRow group in groupList)
            {
                m_groupID = Convert.ToInt32(group["ID"]);
                GroupEditBox.Text = m_group;
                DescriptionEditBox.Text = group["Description"].ToString();
            }

            UserlistView.Items.Clear();

            DataRow[] userList = m_userDataSet.Tables[0].Select("Group = " + m_groupID, "Username");
            foreach (DataRow user in userList)
            {
                ListViewItem newItem = new ListViewItem(user["Username"].ToString(), 1);
                newItem.ImageIndex = 0;

                newItem.SubItems.Add(user["Full Name"].ToString());
                newItem.SubItems.Add(user["Description"].ToString());

                UserlistView.Items.AddRange(new ListViewItem[] { newItem });
            }
        }


        private void GroupFrm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }

  
    }
}