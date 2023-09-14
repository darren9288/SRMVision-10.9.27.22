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
    public partial class UserForm : Form
    {
        #region Members Variables

        private bool m_isNewUser;
        private int m_group;
        private string m_selectedUserName;
        private string m_currentUserName;
        private DataSet m_userDataSet;
        private DataCrypt m_dataCrypt;
        private DBCall m_dbCall = new DBCall(@"access\user.mdb");
        private ProductionInfo m_smProductionInfo;
        #endregion

        public UserForm(string selectedUserName, string currentUserName, bool isNewUser, int group, CustomOption option, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_selectedUserName = selectedUserName;
            m_currentUserName = currentUserName;
            m_isNewUser = isNewUser;
            m_group = group;
            m_dataCrypt = new DataCrypt();
            m_userDataSet = new DataSet();
            m_dbCall.Select("SELECT * FROM Users ORDER BY [Username]", m_userDataSet);

            GetUserInfo();
        }

        public void SaveDataToDatabase()
        {
            m_dataCrypt = new DataCrypt();
            int actualGroup = 0;
            string group = GroupComboBox.SelectedItem.ToString();

            if (group == "SRM")
                actualGroup = 1;
            else if (group == "Administrator")
                actualGroup = 2;
            else if (group == "Engineer")
                actualGroup = 3;
            else if (group == "Technician")
                actualGroup = 4;
            else
                actualGroup = 5;

            string sql = "UPDATE Users SET [Full Name] = '" + FullnameEditBox.Text +
                "', [Description] = '" + DescriptionEditBox.Text +
                "', [Password] = '" + m_dataCrypt.Encrypt(Password1EditBox.Text, UsernameEditBox.Text) +
                "', [Group] = " + actualGroup + " WHERE [Username] = '" + UsernameEditBox.Text + "'";
            m_dbCall.Update(sql);
        }

        public void AddDataToDatabase()
        {
            m_dataCrypt = new DataCrypt();
            int actualGroup = 0;
            string group = GroupComboBox.SelectedItem.ToString();

            if (group == "SRM")
                actualGroup = 1;
            else if (group == "Administrator")
                actualGroup = 2;
            else if (group == "Engineer")
                actualGroup = 3;
            else if (group == "Technician")
                actualGroup = 4;
            else
                actualGroup = 5;

            string sqlInsert = "INSERT INTO Users ([Username], [Description], [Full Name], [Password], [Group]) VALUES ('" +
                UsernameEditBox.Text + "', '" + DescriptionEditBox.Text + "', '" + FullnameEditBox.Text +
                "', '" + m_dataCrypt.Encrypt(Password1EditBox.Text, UsernameEditBox.Text) + "', " + actualGroup + ")";
            m_dbCall.Insert(sqlInsert);
        }


        private void GetUserInfo()
        {
            if ((m_group <= 2) || (m_selectedUserName == m_currentUserName))
            {
                Password1EditBox.Enabled = true;
                Password2EditBox.Enabled = true;
            }

            if (m_selectedUserName == "" || (m_group >= 3))
                DeleteButton.Enabled = false;

            if (m_group <= 1)
                GroupComboBox.Items.Add("SRM");
            if (m_group <= 2)
                GroupComboBox.Items.Add("Administrator");
            if (m_group <= 3)
                GroupComboBox.Items.Add("Engineer");
            if (m_group <= 4)
                GroupComboBox.Items.Add("Technician");
            if (m_group <= 5)
                GroupComboBox.Items.Add("Operator");

            if (m_selectedUserName != "")
            {
                DataRow[] userList = m_userDataSet.Tables[0].Select("Username = '" + m_selectedUserName + "'", "Username");
                foreach (DataRow user in userList)
                {
                    UsernameEditBox.Text = user["Username"].ToString();
                    FullnameEditBox.Text = user["Full Name"].ToString();
                    DescriptionEditBox.Text = user["Description"].ToString();
                    int viewGroup = Convert.ToInt32(user["Group"]);
                    GroupComboBox.SelectedIndex = viewGroup - m_group;
                    Password1EditBox.Text = Password2EditBox.Text =
                        m_dataCrypt.Decrypt(user["Password"].ToString(), user["Username"].ToString());
                }
            }
            else
            {
                UsernameEditBox.Enabled = true;
                GroupComboBox.SelectedIndex = 5 - m_group;
            }
        }

        private bool ValidateEditedCtrl(bool save)
        {
            bool saveEnable = false;

            //data validation
            if (!save)
            {
                if (UsernameEditBox.Text == "" || DescriptionEditBox.Text == "" || FullnameEditBox.Text == "" ||
                    Password1EditBox.Text == "" || Password2EditBox.Text == "")
                    saveEnable = false;
                else
                    saveEnable = true;
            }
            else
            {
                if (Password1EditBox.Text != Password2EditBox.Text)
                {
                    SRMMessageBox.Show("Please validate the password", "User", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Password2EditBox.Select();
                    saveEnable = false;
                }
                else
                    saveEnable = true;
            }

            SaveButton.Enabled = saveEnable;

            if (saveEnable)
                return true;
            else
                return false;
        }




        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (m_selectedUserName != "")
                m_dbCall.Delete("DELETE FROM Users WHERE [Username] = '" + UsernameEditBox.Text + "'");

            this.Close();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!ValidateEditedCtrl(true))
                return;
            
            if (m_selectedUserName != "")
            {
                SaveDataToDatabase();
                STDeviceEdit.SaveDeviceEditLog("User", "Modified existing user", "", m_selectedUserName, m_smProductionInfo.g_strLotID);
            }
            else
            {
                DataRow[] userList = m_userDataSet.Tables[0].Select("Username = '" + UsernameEditBox.Text + "'");
                if (userList.Length > 0)
                {
                    SRMMessageBox.Show("Duplicated User Name", "User", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    UsernameEditBox.Select();
                    return;
                }

                AddDataToDatabase();
               

                STDeviceEdit.SaveDeviceEditLog("User", "Added new user", "", UsernameEditBox.Text, m_smProductionInfo.g_strLotID);
            }

            
            this.Close();
        }


        private void UsernameEditBox_TextChanged(object sender, EventArgs e)
        {
            ValidateEditedCtrl(false);
        }

        private void FullnameEditBox_TextChanged(object sender, EventArgs e)
        {
            ValidateEditedCtrl(false);
        }

        private void DescriptionEditBox_TextChanged(object sender, EventArgs e)
        {
            ValidateEditedCtrl(false);
        }

        private void Password1EditBox_TextChanged(object sender, EventArgs e)
        {
            ValidateEditedCtrl(false);
        }

        private void Password2EditBox_TextChanged(object sender, EventArgs e)
        {
            ValidateEditedCtrl(false);
        }


        private void GroupComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidateEditedCtrl(false);
        }


        private void UserForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }

  
    }
}