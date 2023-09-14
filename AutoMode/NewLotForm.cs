using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using SharedMemory;
using System.Diagnostics;

namespace AutoMode
{
    public partial class NewLotForm : Form
    {
        #region Member Variables

        private int m_intUserGroup = 5;
        private string m_strLotNoPrev = "";
        private string m_strOperatorIDPrev = "";
        private string m_strRecipeIDPrev = "";
        private string m_strLotStartTimePrev = "";

        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        
        #endregion

        public NewLotForm(ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            InitializeComponent();

            txt_NewLot.Text = m_strLotNoPrev = smProductionInfo.g_strLotID;
            txt_OpID.Text = m_strOperatorIDPrev = smProductionInfo.g_strOperatorID;
            m_strRecipeIDPrev = smProductionInfo.g_strRecipeID;
            m_strLotStartTimePrev = smProductionInfo.g_strLotStartTime;

            m_smProductionInfo = smProductionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_intUserGroup = m_smProductionInfo.g_intUserGroup;

            UpdateRecipeAvailable();

            DisableField();
        }

        private void DisableField()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "New Lot";
            string strChild2 = "";

            strChild2 = "Save Button";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild2Group(strChild1, strChild2))
                btn_OK.Enabled = false;
            else
            {
                btn_OK.Enabled = true;
            }
        }

        /// <summary>
        /// Display all the available recipe, select and display previous recipe 
        /// </summary>
        private void UpdateRecipeAvailable()
        {
            string strTargetDirectory = m_smProductionInfo.g_strRecipePath;
            string[] strDirectoriesList = Directory.GetDirectories(strTargetDirectory);
            int intDirectoryNameLength = strTargetDirectory.Length;

            if (strDirectoriesList.Length == 0)
            {
                SRMMessageBox.Show("Device No. does not exist. Please create a new device", "Device No Setting",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                string strRecipeID = "";
                foreach (string strDirectoryName in strDirectoriesList)
                {
                    strRecipeID = strDirectoryName.Remove(0, intDirectoryNameLength);
                    cbo_RecipeID.Items.Add(strRecipeID);
                }

                int intSelectedRecipeIndex = cbo_RecipeID.FindStringExact(m_strRecipeIDPrev);
                if (intSelectedRecipeIndex != -1)
                {
                    cbo_RecipeID.SelectedIndex = intSelectedRecipeIndex;
                }
            }

        }



        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (txt_OpID.Text == "")
            {
                SRMMessageBox.Show("Please fill in your operator ID", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txt_NewLot.Text.IndexOf("_") >= 0)
            {
                SRMMessageBox.Show("Symbol _ is not allowed in LotID", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txt_NewLot.Text.IndexOf("/") >= 0)
            {
                SRMMessageBox.Show("Symbol / is not allowed in LotID", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txt_NewLot.Text.IndexOf("*") >= 0)
            {
                SRMMessageBox.Show("Symbol * is not allowed in LotID", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txt_NewLot.Text.IndexOf("\\") >= 0)
            {
                SRMMessageBox.Show("Symbol \\ is not allowed in LotID", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cbo_RecipeID.SelectedIndex == -1)
            {
                SRMMessageBox.Show("Please choose a recipe ID", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (txt_NewLot.Text == "")
                txt_NewLot.Text = "SRM";

            string strLotStartTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            string strLotPrev = subkey1.GetValue("LotNo", "SRM").ToString();

            subkey1.SetValue("LotNo", txt_NewLot.Text);
            for (int i = 0; i < 10; i++)
            {
                subkey1.SetValue("SingleLotNo" + i.ToString(), txt_NewLot.Text);
            }
            subkey1.SetValue("OperatorID", txt_OpID.Text);
            subkey1.SetValue("SelectedRecipeID", cbo_RecipeID.SelectedItem.ToString());
            subkey1.SetValue("LotStartTime", strLotStartTime);
            subkey1.SetValue("LotNoPrev", m_strLotNoPrev);
            subkey1.SetValue("OperatorIDPrev", m_strOperatorIDPrev);
            subkey1.SetValue("RecipeIDPrev", m_strRecipeIDPrev);
            subkey1.SetValue("LotStartTimePrev", m_strLotStartTimePrev);

            m_smProductionInfo.g_strLotID = txt_NewLot.Text;
            for (int i = 0; i < 10; i++)
            {
                m_smProductionInfo.g_arrSingleLotID[i] = m_smProductionInfo.g_strLotID;
                subkey1.SetValue("SingleRecipeID" + i.ToString(), m_smProductionInfo.g_arrSingleRecipeID[i]);
            }
            m_smProductionInfo.g_strOperatorID = txt_OpID.Text;
            m_smProductionInfo.g_strRecipeID = cbo_RecipeID.SelectedItem.ToString();
            m_smProductionInfo.g_strLotStartTime = strLotStartTime;


            string strLot = subkey1.GetValue("LotNo", "SRM").ToString();

            if (strLotPrev != strLot)
                STDeviceEdit.SaveDeviceEditLog("New Lot", "Create New Lot", strLotPrev, strLot, m_smProductionInfo.g_strLotID);
            
            DialogResult = DialogResult.OK;
            Close();
            Dispose();
        }



        private void NewLotForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_smProductionInfo.AT_ALL_InAuto && m_smProductionInfo.g_intAutoLogOutMinutes > 0 && m_intUserGroup != 5)
            {
                DateTime t = DateTime.Now;
                TimeSpan tSpan = t - m_smProductionInfo.g_DTStartAutoMode_IndividualForm;

                if (tSpan.Minutes >= m_smProductionInfo.g_intAutoLogOutMinutes)
                {
                    m_smProductionInfo.g_intUserGroup = 5;
                    m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
                    DisableField();
                }
            }
        }

        private void btn_OSK_Click(object sender, EventArgs e)
        {
            if (File.Exists("C:\\Driver\\SRM_OSK.exe"))
                Process.Start("C:\\Driver\\SRM_OSK.exe");
            else
                SRMMessageBox.Show("OSK Application cannot be found!", "Error");
        }
    }
}