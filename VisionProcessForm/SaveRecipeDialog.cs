using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Common;

namespace VisionProcessForm
{
    public partial class SaveRecipeDialog : Form
    {
        #region Member Variables        

        private string m_strFolderName = "";
        private string m_strSelectedRecipe = "Default";
        private string m_strDestination;
 

        #endregion

        #region Properties

        public string ref_strSelectedRecipe { get { return m_strDestination; } }

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="strFolderName">Format such as "Vision1\\Orient\\", "Vision1\\Mark\\"</param>
        /// <param name="strSelectedRecipe">selected recipe on production now</param>
        public SaveRecipeDialog(string strFolderName, string strSelectedRecipe)
        {
            InitializeComponent();

            m_strFolderName = strFolderName;
            m_strDestination = m_strSelectedRecipe = strSelectedRecipe;
      
       
            UpdateRecipeAvailable();
            cbo_SaveAsType.SelectedIndex = 0;
        }



        
        public void CopyDirectory(string strSource,string strDestination)
        {
            if (strDestination[strDestination.Length - 1] != Path.DirectorySeparatorChar)
                strDestination += Path.DirectorySeparatorChar;
            if (!Directory.Exists(strDestination)) 
                Directory.CreateDirectory(strDestination);

            String[]  strFiles = Directory.GetFileSystemEntries(strSource);
            foreach (string strElement in strFiles)
            {
                // Sub directories
                if (Directory.Exists(strElement))
                    CopyDirectory(strElement, strDestination + Path.GetFileName(strElement));
                // Files in directory
                else
                    File.Copy(strElement, strDestination + Path.GetFileName(strElement), true);
            }
        }

        private void UpdateRecipeAvailable()
        {
            string strTargetDirectory = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo";
            string[] strDirectoriesList = Directory.GetDirectories(strTargetDirectory);
            int intDirectoryNameLength = strTargetDirectory.Length + 1;

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
                    if(m_strSelectedRecipe!= strRecipeID)
                        lvw_Recipe.Items.Add(new ListViewItem(strRecipeID, 0));
                }
            }
        }




        private void lvw_Recipe_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvw_Recipe.SelectedItems.Count > 0)
            {
                ListViewItem lstItem = lvw_Recipe.SelectedItems[0];
                txt_RecipeName.Text = lstItem.Text;
            }
            
        }



        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (txt_RecipeName.Text == String.Empty)
            {
                SRMMessageBox.Show("Please select a recipe to save the data", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } 
            if (txt_RecipeName.Text == m_strSelectedRecipe)
            {
                SRMMessageBox.Show("Recipe name can't be same with current recipe name !", "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
                
            m_strDestination = txt_RecipeName.Text.ToString();
            string strPath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_strDestination + "\\";
            
            if (Directory.Exists(strPath))
            {
                if (SRMMessageBox.Show("Are you sure you want to rewrite the settings", "SRM Vision", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                    return;
            }
            else
                CopyDirectory(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\Default\\", strPath);

            string[] strFolders = m_strFolderName.Split(Path.DirectorySeparatorChar);
            if (strFolders[1].IndexOf("Mark") > 0)
                Directory.Delete(strPath + strFolders[0] + "\\Mark", true);
            if (strFolders[1].IndexOf("Orient") > 0)
                Directory.Delete(strPath + strFolders[0] + "\\Orient",true);
            if (strFolders[1].IndexOf("Seal") > 0)
            {
                Directory.Delete(strPath + strFolders[0] + "\\Seal", true);
            }

            if (cbo_SaveAsType.SelectedIndex == 1)
            {
                CopyDirectory(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" +
                    m_strSelectedRecipe + "\\", strPath);
            }
            else
            {             
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                strPath += strFolders[0] + "\\";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                string strPath1 = strPath + strFolders[1] + "\\";
                if (!Directory.Exists(strPath))
                    Directory.CreateDirectory(strPath);

                if (m_strFolderName.IndexOf("Mark") > 0)
                {
                    string strTemp = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" +
                    m_strSelectedRecipe + "\\" + strFolders[0] + "\\Mark\\";
                    if (!Directory.Exists(strTemp))
                        Directory.CreateDirectory(strTemp);
                    CopyDirectory(strTemp, strPath + "\\Mark");
                }

                if (m_strFolderName.IndexOf("Orient") > 0)
                {
                    string strTemp = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" +
                    m_strSelectedRecipe + "\\" + strFolders[0] + "\\Orient\\";
                    if (!Directory.Exists(strTemp))
                        Directory.CreateDirectory(strTemp);
                    CopyDirectory(strTemp, strPath + "\\Orient");
                }
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }




        private void RecipeForm_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
        }



       

        
    }
}