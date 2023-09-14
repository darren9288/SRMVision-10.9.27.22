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
    public partial class LoginForm : Form
    {
        #region Members Variables

        private int m_intUserGroup;
        private string m_strUserName;
        private DBCall m_dbCall = new DBCall(@"access\user.mdb");
        private ProductionInfo m_smProductionInfo;
        #endregion

        public LoginForm(ProductionInfo smProductionInfo)
        {
            m_smProductionInfo = smProductionInfo;
            InitializeComponent();

#if (DEBUG || Debug_2_12 || RTXDebug)
            UserNameTextBox.Text = "srm";
            PasswordTextBox.Text = "srm";
#endif
        }

        public int ref_intUserGroup
        {
            get { return this.m_intUserGroup; }
        }

        public string ref_strUserName
        {
            get { return this.m_strUserName; }
        }

        public string GetPassword()
        {
            string password = "";
            DataSet userDataSet = new DataSet();

            m_dbCall.Select("SELECT * FROM Users WHERE Username = '" + m_strUserName + "'", userDataSet);

            if (userDataSet.Tables[0].Rows.Count > 0)
            {
                password = userDataSet.Tables[0].Rows[0]["Password"].ToString();
                m_intUserGroup = Convert.ToInt32(userDataSet.Tables[0].Rows[0]["Group"]);
            }

            return password;
        }

        public void SavePassword(string userName, string password)
        {
            string sql = "UPDATE Users SET [Password] = '" + password +
                "' WHERE [Username] = '" + userName + "'";
            m_dbCall.Update(sql);
        }


        private bool LoginPassword()
        {
            DataCrypt dataCrypt = new DataCrypt();

            this.m_strUserName = UserNameTextBox.Text;
            string password = PasswordTextBox.Text;

            // Check if username / password is null
            if (this.m_strUserName == "" && password == "")
            {
                UserNameTextBox.Select();
                return false;
            }
            else if (this.m_strUserName == "")
            {
                UserNameTextBox.Select();
                return false;
            }
            else if (password == "")
            {
                PasswordTextBox.Select();
                return false;
            }

            string dbPassword = GetPassword();

            if (dbPassword == "")
            {
                SRMMessageBox.Show("Sorry. You are not a valid user", "Login", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                UserNameTextBox.Select();
                return false;
            }

            string decryptedPassword = dataCrypt.Decrypt(dbPassword, m_strUserName);
            if (password == decryptedPassword)
            {
                

                STDeviceEdit.SaveDeviceEditLog("Login", " login as "+ m_strUserName, "", m_strUserName, m_smProductionInfo.g_strLotID);
                


                return true;
            }
            else
            {
                SRMMessageBox.Show("Sorry. Wrong Password", "Login", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                PasswordTextBox.Select();
                return false;
            }
        }

        private void VerifyUser()
        {
            m_intUserGroup = 5;
            if (LoginPassword())
            {
                this.Close();
                this.DialogResult = DialogResult.OK;
            }
            else
                this.BringToFront();
        }


        private void OkButton_Click(object sender, EventArgs e)
        {
            VerifyUser();
        }

        private void Cancel1Button_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void LoginForm_Load(object sender, EventArgs e)
        {
            UserNameTextBox.Select();
            Cursor.Current = Cursors.Default;
        }

      
    }
}