using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Common;

namespace AutoMode
{
    public partial class RecipeCopyForm : Form
    {
        private string m_strRecipeDirectory;

        #region properties

        public string ref_strRecipeCopyTo
        {
            get { return txt_CopyTo.Text; }
        }
        
#endregion


        public RecipeCopyForm(string strEditRecipe, string strRecipeDirectory)
        {
            InitializeComponent();
            m_strRecipeDirectory = strRecipeDirectory;
            txt_CopyFrom.Text = strEditRecipe;
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
            string strNewRecipe = txt_CopyTo.Text;

            if (strNewRecipe == "")
            {
                SRMMessageBox.Show("Please key in new Recipe Name!", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                if (DeviceNoExist(strNewRecipe))
                {
                    SRMMessageBox.Show("This Device No. already existed!", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            if (strNewRecipe.IndexOf("\\") >= 0)
            {
                SRMMessageBox.Show("Invalid Device No. containing [\\]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.IndexOf("/") >= 0)
            {
                SRMMessageBox.Show("Invalid Device No. containing [/]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.IndexOf(":") >= 0)
            {
                SRMMessageBox.Show("Invalid Device No. containing [:]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.IndexOf("*") >= 0)
            {
                SRMMessageBox.Show("Invalid Device No. containing [*]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.IndexOf("?") >= 0)
            {
                SRMMessageBox.Show("Invalid Device No. containing [?]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.IndexOf("\"") >= 0)
            {
                SRMMessageBox.Show("Invalid Device No. containing [\"]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.IndexOf("<") >= 0)
            {
                SRMMessageBox.Show("Invalid Device No. containing [<]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.IndexOf(">") >= 0)
            {
                SRMMessageBox.Show("Invalid Device No. containing [>]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else if (strNewRecipe.IndexOf("|") >= 0)
            {
                SRMMessageBox.Show("Invalid Device No. containing [|]", "Recipe Form", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            this.Close();
            this.DialogResult = DialogResult.OK;
        }



        private void btn_OK_Click(object sender, EventArgs e)
        {
            VerifyData();
        }

      
    }
}