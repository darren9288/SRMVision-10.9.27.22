using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows.Forms;
using Common;
using SharedMemory;

namespace User
{
    public partial class UserManagerForm : Form
    {
        #region Members Variables
        private int m_group;
        private string m_strUserName;
        private CustomOption m_option;
        private ProductionInfo m_smProductionInfo;
        private DataSet m_userDataSet;
        private DataSet m_groupDataSet;

        #endregion

        public int Group
        {
            get { return m_group; }
        }

        public string UserName
        {
            get { return m_strUserName; }
        }

        public UserManagerForm(int group, string userName, CustomOption option, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_group = group;
            m_strUserName = userName;
            m_option = option;
            //UserRight userRight = new UserRight();
            //if (m_group <= userRight.GetGroupLevel2("User Admin", "New Users"))
            //    NewUserButton.Visible = true;
            //if (m_group <= userRight.GetGroupLevel2("User Admin", "Users Rights"))
            //    UserRightButton.Visible = true;

            //NewUserRight userRight = new NewUserRight(false);
            if (m_group <= m_option.objNewUserRight.GetTopMenuChild2Group("User Admin", "New Users"))
                NewUserButton.Visible = true;
            if (m_group <= m_option.objNewUserRight.GetTopMenuChild2Group("User Admin", "Users Rights"))
                UserRightButton.Visible = true;

            // Fill out the list view
            GetUserDataSet();
            UpdateUserGridView();
            UpdateGroupGridView();
        }


        private void GetUserDataSet()
        {
            m_userDataSet = new DataSet();
            m_groupDataSet = new DataSet();

            DBCall dbCall = new DBCall(@"access\user.mdb");
            dbCall.Select("SELECT * FROM Users", m_userDataSet);
            dbCall.Select("SELECT * FROM Groups", m_groupDataSet);
        }

        private void OpenUserDlg(string selectedUserName, string currentUserName, bool isNewUser)
        {
            UserForm user = new UserForm(selectedUserName, currentUserName, isNewUser, m_group, m_option, m_smProductionInfo);
            user.ShowDialog(this);
            user.Dispose();
            GetUserDataSet();
            UpdateUserGridView();
        }

        private void OpenUserRightDlg()
        {
            //UserRightForm userRight = new UserRightForm(m_group, m_smProductionInfo);
            IndividualUserRightForm userRight = new IndividualUserRightForm(m_group, m_smProductionInfo, m_option);
            userRight.ShowDialog(this);
            userRight.Dispose();
            m_option.objNewUserRight.InitTable();
        }

        private void UpdateUserGridView()
        {
            UserGridView.Rows.Clear();

            DataRow[] userList = m_userDataSet.Tables[0].Select("Group >= " + m_group.ToString(), "Username");
            foreach (DataRow user in userList)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.Height = 24;
                newRow.CreateCells(UserGridView, new object[] { user["Username"].ToString(),
                    user["Full Name"].ToString(), user["Description"].ToString(),
                    user["Group"].ToString() });
                UserGridView.Rows.Add(newRow);
            }
        }

        private void UpdateGroupGridView()
        {
            GroupGridView.Rows.Clear();

            DataRow[] groupList = m_groupDataSet.Tables[0].Select("ID > 0", "Name");
            foreach (DataRow group in groupList)
            {
                DataGridViewRow newRow = new DataGridViewRow();
                newRow.Height = 18;
                newRow.CreateCells(GroupGridView, new object[] { group["Name"].ToString(),
                    group["Description"].ToString() });
                GroupGridView.Rows.Add(newRow);
            }
        }


        private void NewUserButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            OpenUserDlg("", "", true);
        }

        private void UserRightButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            OpenUserRightDlg();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            this.Close();
            this.Dispose();
        }


        private void GroupGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                UserGroupForm group = new UserGroupForm(GroupGridView.Rows[e.RowIndex].Cells[0].Value.ToString(), m_option);
                group.ShowDialog(this);
                group.Dispose();
            }
        }

        private void UserGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                OpenUserDlg(UserGridView.Rows[e.RowIndex].Cells[0].Value.ToString(), m_strUserName, false);
        }


        private void UserManagerForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }

        private void btn_LoadUserRight_Click(object sender, EventArgs e)
        {
            LoadUserRightForm userRight = new LoadUserRightForm();
            userRight.ShowDialog(this);
            userRight.Close();
            userRight.Dispose();
        }
    }
}