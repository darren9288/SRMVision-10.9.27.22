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
using SharedMemory;

namespace AutoMode
{
    public partial class RecipeCopyToExistingForm : Form
    {
        private string m_strRecipeDirectory;
        VisionInfo[] m_smVSInfo;
        #region properties

        public bool ref_CopyAdvanceSettingOnly { get { return chk_CopyAdvanceSettingOnly.Checked; } }

        public string ref_strRecipeCopyTo
        {
            get { return cbo_CopyToRecipe.SelectedItem.ToString(); }
        }

        public bool ref_blnAllVision
        {
            get { return chk_AllVision.Checked; }
        }
        public bool ref_blnVision1
        {
            get { return chk_Vision1.Checked; }
        }
        public bool ref_blnVision2
        {
            get { return chk_Vision2.Checked; }
        }
        public bool ref_blnVision3
        {
            get { return chk_Vision3.Checked; }
        }
        public bool ref_blnVision4
        {
            get { return chk_Vision4.Checked; }
        }
        public bool ref_blnVision5
        {
            get { return chk_Vision5.Checked; }
        }
        public bool ref_blnVision6
        {
            get { return chk_Vision6.Checked; }
        }
        public bool ref_blnVision7
        {
            get { return chk_Vision7.Checked; }
        }
        public bool ref_blnVision8
        {
            get { return chk_Vision8.Checked; }
        }
        public bool ref_blnVision9
        {
            get { return chk_Vision9.Checked; }
        }
        public bool ref_blnVision10
        {
            get { return chk_Vision10.Checked; }
        }
        #endregion


        public RecipeCopyToExistingForm(string strEditRecipe, string strRecipeDirectory, string[] arrExistingRecipe, VisionInfo[] smVSInfo)
        {
            InitializeComponent();
            m_smVSInfo = smVSInfo;
            m_strRecipeDirectory = strRecipeDirectory;
            txt_CopyFrom.Text = strEditRecipe;

            cbo_CopyToRecipe.Items.Clear();
            cbo_CopyToRecipe.Items.Add("All Existing Recipe");

            foreach (string recipe in arrExistingRecipe)
            {
                if (recipe != strEditRecipe)
                    cbo_CopyToRecipe.Items.Add(recipe);
            }
            cbo_CopyToRecipe.SelectedIndex = 0;

            chk_AllVision.Checked = true;
            string[] arrDir = Directory.GetDirectories(strRecipeDirectory + strEditRecipe + "\\");
            foreach (string dir in arrDir)
            {
                string strVision = dir.Substring(dir.LastIndexOf('\\') + 1, dir.Length - dir.LastIndexOf('\\') - 1);

                if (strVision == "Vision1" && smVSInfo.Length > 0 && smVSInfo[0] != null)
                {
                    chk_Vision1.Text = "Vision1 - " + smVSInfo[0].g_strVisionName;
                    chk_Vision1.Visible = chk_Vision1.Checked = true;
                    chk_Vision1.Enabled = false;
                }

                if (strVision == "Vision2" && smVSInfo.Length > 1 && smVSInfo[1] != null)
                {
                    chk_Vision2.Text = "Vision2 - " + smVSInfo[1].g_strVisionName;
                    chk_Vision2.Visible = chk_Vision2.Checked = true;
                    chk_Vision2.Enabled = false;
                }

                if (strVision == "Vision3" && smVSInfo.Length > 2 && smVSInfo[2] != null)
                {
                    chk_Vision3.Text = "Vision3 - " + smVSInfo[2].g_strVisionName;
                    chk_Vision3.Visible = chk_Vision3.Checked = true;
                    chk_Vision3.Enabled = false;
                }

                if (strVision == "Vision4" && smVSInfo.Length > 3 && smVSInfo[3] != null)
                {
                    chk_Vision4.Text = "Vision4 - " + smVSInfo[3].g_strVisionName;
                    chk_Vision4.Visible = chk_Vision4.Checked = true;
                    chk_Vision4.Enabled = false;
                }

                if (strVision == "Vision5" && smVSInfo.Length > 4 && smVSInfo[4] != null)
                {
                    chk_Vision5.Text = "Vision5 - " + smVSInfo[4].g_strVisionName;
                    chk_Vision5.Visible = chk_Vision5.Checked = true;
                    chk_Vision5.Enabled = false;
                }

                if (strVision == "Vision6" && smVSInfo.Length > 5 && smVSInfo[5] != null)
                {
                    chk_Vision6.Text = "Vision6 - " + smVSInfo[5].g_strVisionName;
                    chk_Vision6.Visible = chk_Vision6.Checked = true;
                    chk_Vision6.Enabled = false;
                }

                if (strVision == "Vision7" && smVSInfo.Length > 6 && smVSInfo[6] != null)
                {
                    chk_Vision7.Text = "Vision7 - " + smVSInfo[6].g_strVisionName;
                    chk_Vision7.Visible = chk_Vision7.Checked = true;
                    chk_Vision7.Enabled = false;
                }

                if (strVision == "Vision8" && smVSInfo.Length > 7 && smVSInfo[7] != null)
                {
                    chk_Vision8.Text = "Vision8 - " + smVSInfo[7].g_strVisionName;
                    chk_Vision8.Visible = chk_Vision8.Checked = true;
                    chk_Vision8.Enabled = false;
                }

                if (strVision == "Vision9" && smVSInfo.Length > 8 && smVSInfo[8] != null)
                {
                    chk_Vision9.Text = "Vision9 - " + smVSInfo[8].g_strVisionName;
                    chk_Vision9.Visible = chk_Vision9.Checked = true;
                    chk_Vision9.Enabled = false;
                }

                if (strVision == "Vision10" && smVSInfo.Length > 9 && smVSInfo[9] != null)
                {
                    chk_Vision10.Text = "Vision10 - " + smVSInfo[9].g_strVisionName;
                    chk_Vision10.Visible = chk_Vision10.Checked = true;
                    chk_Vision10.Enabled = false;
                }
            }
            
        }

        /// <summary>
        /// Check whether device no is exist
        /// </summary>
        /// <param name="strRecipeName">recipe name</param>
        /// <returns>true = device no exist, false = device no not exist</returns>
        private bool DeviceNoExist(string strRecipeName)
        {
            if (Directory.Exists(m_strRecipeDirectory + strRecipeName))
                return true;

            return false;
        }



        /// <summary>
        /// Validate new recipe name
        /// </summary>
        private void VerifyData()
        {
            //string strNewRecipe = txt_CopyTo.Text;

            //if (strNewRecipe == "")
            //{
            //    SRMMessageBox.Show("Please key in new Recipe Name!", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            //else
            //{
            //    if (DeviceNoExist(strNewRecipe))
            //    {
            //        SRMMessageBox.Show("This Device No. already existed!", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //        return;
            //    }
            //}

            //if (strNewRecipe.IndexOf("\\") >= 0)
            //{
            //    SRMMessageBox.Show("Invalid Device No. containing [\\]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            //else if (strNewRecipe.IndexOf("/") >= 0)
            //{
            //    SRMMessageBox.Show("Invalid Device No. containing [/]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            //else if (strNewRecipe.IndexOf(":") >= 0)
            //{
            //    SRMMessageBox.Show("Invalid Device No. containing [:]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            //else if (strNewRecipe.IndexOf("*") >= 0)
            //{
            //    SRMMessageBox.Show("Invalid Device No. containing [*]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            //else if (strNewRecipe.IndexOf("?") >= 0)
            //{
            //    SRMMessageBox.Show("Invalid Device No. containing [?]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            //else if (strNewRecipe.IndexOf("\"") >= 0)
            //{
            //    SRMMessageBox.Show("Invalid Device No. containing [\"]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            //else if (strNewRecipe.IndexOf("<") >= 0)
            //{
            //    SRMMessageBox.Show("Invalid Device No. containing [<]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            //else if (strNewRecipe.IndexOf(">") >= 0)
            //{
            //    SRMMessageBox.Show("Invalid Device No. containing [>]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}
            //else if (strNewRecipe.IndexOf("|") >= 0)
            //{
            //    SRMMessageBox.Show("Invalid Device No. containing [|]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    return;
            //}

            this.Close();
            this.DialogResult = DialogResult.OK;
        }



        private void btn_OK_Click(object sender, EventArgs e)
        {
            VerifyData();
        }

        private void chk_AllVision_Click(object sender, EventArgs e)
        {
            if (chk_AllVision.Checked)
            {
                if (chk_Vision1.Visible)
                {
                    chk_Vision1.Visible = chk_Vision1.Checked = true;
                    chk_Vision1.Enabled = false;
                }

                if (chk_Vision2.Visible)
                {
                    chk_Vision2.Visible = chk_Vision2.Checked = true;
                    chk_Vision2.Enabled = false;
                }

                if (chk_Vision3.Visible)
                {
                    chk_Vision3.Visible = chk_Vision3.Checked = true;
                    chk_Vision3.Enabled = false;
                }

                if (chk_Vision4.Visible)
                {
                    chk_Vision4.Visible = chk_Vision4.Checked = true;
                    chk_Vision4.Enabled = false;
                }

                if (chk_Vision5.Visible)
                {
                    chk_Vision5.Visible = chk_Vision5.Checked = true;
                    chk_Vision5.Enabled = false;
                }

                if (chk_Vision6.Visible)
                {
                    chk_Vision6.Visible = chk_Vision6.Checked = true;
                    chk_Vision6.Enabled = false;
                }

                if (chk_Vision7.Visible)
                {
                    chk_Vision7.Visible = chk_Vision7.Checked = true;
                    chk_Vision7.Enabled = false;
                }

                if (chk_Vision8.Visible)
                {
                    chk_Vision8.Visible = chk_Vision8.Checked = true;
                    chk_Vision8.Enabled = false;
                }

                if (chk_Vision9.Visible)
                {
                    chk_Vision9.Visible = chk_Vision9.Checked = true;
                    chk_Vision9.Enabled = false;
                }

                if (chk_Vision10.Visible)
                {
                    chk_Vision10.Visible = chk_Vision10.Checked = true;
                    chk_Vision10.Enabled = false;
                }

            }
            else
            {
                if (chk_Vision1.Visible)
                {
                    chk_Vision1.Checked = false;
                    chk_Vision1.Enabled = true;
                }

                if (chk_Vision2.Visible)
                {
                    chk_Vision2.Checked = false;
                    chk_Vision2.Enabled = true;
                }

                if (chk_Vision3.Visible)
                {
                    chk_Vision3.Checked = false;
                    chk_Vision3.Enabled = true;
                }

                if (chk_Vision4.Visible)
                {
                    chk_Vision4.Checked = false;
                    chk_Vision4.Enabled = true;
                }

                if (chk_Vision5.Visible)
                {
                    chk_Vision5.Checked = false;
                    chk_Vision5.Enabled = true;
                }

                if (chk_Vision6.Visible)
                {
                    chk_Vision6.Checked = false;
                    chk_Vision6.Enabled = true;
                }

                if (chk_Vision7.Visible)
                {
                    chk_Vision7.Checked = false;
                    chk_Vision7.Enabled = true;
                }

                if (chk_Vision8.Visible)
                {
                    chk_Vision8.Checked = false;
                    chk_Vision8.Enabled = true;
                }

                if (chk_Vision9.Visible)
                {
                    chk_Vision9.Checked = false;
                    chk_Vision9.Enabled = true;
                }

                if (chk_Vision10.Visible)
                {
                    chk_Vision10.Checked = false;
                    chk_Vision10.Enabled = true;
                }

            }
        }
    }
}
