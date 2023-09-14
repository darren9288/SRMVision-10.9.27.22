using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedMemory;
using System.IO;
using Common;
using Microsoft.Win32;

namespace AutoMode
{
    public partial class RecipeExportForm : Form
    {
        private string m_strSelectedRecipeID;
        private string m_strPath;
        private string m_strRecipePath;
        private string[] m_strVisionName = new string[10];
        private ProductionInfo m_smProductionInfo;
        private int m_intVisionCount = 0;
        public RecipeExportForm(ProductionInfo smProductionInfo, string FileName, string strSelectedRecipeID)
        {
            InitializeComponent();
            lbl_RecipeID.Text = m_strSelectedRecipeID = strSelectedRecipeID;
            m_smProductionInfo = smProductionInfo;
            m_strRecipePath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipeID + "\\";
            if (FileName == "" || FileName == null)
            {
                RegistryKey keyAutoMode = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subkeyAutoMode = keyAutoMode.CreateSubKey("SVG\\AutoMode");
                FileName = (string)subkeyAutoMode.GetValue("RecipeSavePath", "D:\\SavedRecipe\\");
                if (!Directory.Exists(FileName))
                    Directory.CreateDirectory(FileName);
            }

            m_intVisionCount = Directory.GetDirectories(m_strRecipePath).Length;
            chk_Vision1.Visible = chk_Vision1.Checked = m_intVisionCount > 0;
            chk_Vision2.Visible = chk_Vision2.Checked = m_intVisionCount > 1;
            chk_Vision3.Visible = chk_Vision3.Checked = m_intVisionCount > 2;
            chk_Vision4.Visible = chk_Vision4.Checked = m_intVisionCount > 3;
            chk_Vision5.Visible = chk_Vision5.Checked = m_intVisionCount > 4;
            chk_Vision6.Visible = chk_Vision6.Checked = m_intVisionCount > 5;
            chk_Vision7.Visible = chk_Vision7.Checked = m_intVisionCount > 6;
            chk_Vision8.Visible = chk_Vision8.Checked = m_intVisionCount > 7;
            chk_Vision9.Visible = chk_Vision9.Checked = m_intVisionCount > 8;
            chk_Vision10.Visible = chk_Vision10.Checked = m_intVisionCount > 9;

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            RegistryKey subKey1;
            for (int i = 0; i < strVisionList.Length; i++)
            {
                subKey1 = subKey.OpenSubKey(strVisionList[i]);
                int intVisionNo = Convert.ToInt32(strVisionList[i].Substring(6)) - 1;
                m_strVisionName[intVisionNo] = subKey1.GetValue("VisionName", "Vision " + (i + 10)).ToString();
            }

            txt_SelectedPath.Text = m_strPath = FileName;
        }

        private static bool CopyAllFiles(string Source, string Destination)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(Source);
                if (!dir.Exists)
                {
                    return false;
                }
                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(Destination))
                {
                    Directory.CreateDirectory(Destination);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    string temppath = Path.Combine(Destination, file.Name);
                    file.CopyTo(temppath, true);
                }
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(Destination, subdir.Name);
                    CopyAllFiles(subdir.FullName, temppath);
                }

            }
            catch
            {

            }
            return true;
        }
        private static bool CopyAllFiles_LearnedSetting(string Source, string Destination)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(Source);

                if (!dir.Exists)
                {
                    return false;
                }
                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(Destination))
                {
                    Directory.CreateDirectory(Destination);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (!file.Name.Contains("Calibration"))
                    {
                        string temppath = Path.Combine(Destination, file.Name);
                        file.CopyTo(temppath, true);
                    }
                }
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(Destination, subdir.Name);
                    CopyAllFiles_LearnedSetting(subdir.FullName, temppath);
                }

            }
            catch
            {

            }
            return true;
        }
        private static bool CopyAllFiles_CalibrationSetting(string Source, string Destination)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(Source);

                if (!dir.Exists)
                {
                    return false;
                }
                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(Destination))
                {
                    Directory.CreateDirectory(Destination);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (file.Name.Contains("Calibration"))
                    {
                        string temppath = Path.Combine(Destination, file.Name);
                        file.CopyTo(temppath, true);
                    }
                }
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(Destination, subdir.Name);
                    CopyAllFiles_CalibrationSetting(subdir.FullName, temppath);
                }

            }
            catch
            {

            }
            return true;
        }
        private static bool CopyAllFiles_ToleranceSetting(string Source, string Destination)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(Source);

                if (!dir.Exists)
                {
                    return false;
                }
                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(Destination))
                {
                    Directory.CreateDirectory(Destination);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (file.Name == "Pin1Template.xml" || file.Name == "Template.xml" || file.Name == "Settings.xml")
                    {
                        string temppath = Path.Combine(Destination, file.Name);
                        file.CopyTo(temppath, true);
                    }
                }
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(Destination, subdir.Name);
                    CopyAllFiles_ToleranceSetting(subdir.FullName, temppath);
                }

            }
            catch
            {

            }
            return true;
        }
        private static bool CopyAllFiles_Lighting(string Source, string Destination)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(Source);

                if (!dir.Exists)
                {
                    return false;
                }
                DirectoryInfo[] dirs = dir.GetDirectories();
                // If the destination directory doesn't exist, create it.
                if (!Directory.Exists(Destination))
                {
                    Directory.CreateDirectory(Destination);
                }

                // Get the files in the directory and copy them to the new location.
                FileInfo[] files = dir.GetFiles();
                foreach (FileInfo file in files)
                {
                    if (file.Name == "Camera.xml")
                    {
                        string temppath = Path.Combine(Destination, file.Name);
                        file.CopyTo(temppath, true);
                    }
                }
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(Destination, subdir.Name);
                    CopyAllFiles_Lighting(subdir.FullName, temppath);
                }
            }
            catch
            {

            }
            return true;
        }
        private void btn_Browse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Reset();
            folderBrowserDialog1.SelectedPath = m_strPath;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txt_SelectedPath.Text = folderBrowserDialog1.SelectedPath + "\\";

            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(txt_SelectedPath.Text))
            {
                SRMMessageBox.Show("Cannot Find Selected Path!");
                return;
            }

            string DTNow = "_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss");

            for (int i = 0; i < m_intVisionCount; i++)
            {
                string strCheckBoxText = "";
                if(i == 0 && chk_Vision1.Checked)
                    strCheckBoxText = chk_Vision1.Text;
                else if (i == 1 && chk_Vision2.Checked)
                    strCheckBoxText = chk_Vision2.Text;
                else if (i == 2 && chk_Vision3.Checked)
                    strCheckBoxText = chk_Vision3.Text;
                else if (i == 3 && chk_Vision4.Checked)
                    strCheckBoxText = chk_Vision4.Text;
                else if (i == 4 && chk_Vision5.Checked)
                    strCheckBoxText = chk_Vision5.Text;
                else if (i == 5 && chk_Vision6.Checked)
                    strCheckBoxText = chk_Vision6.Text;
                else if (i == 6 && chk_Vision7.Checked)
                    strCheckBoxText = chk_Vision7.Text;
                else if (i == 7 && chk_Vision8.Checked)
                    strCheckBoxText = chk_Vision8.Text;
                else if (i == 8 && chk_Vision9.Checked)
                    strCheckBoxText = chk_Vision9.Text;
                else if (i == 9 && chk_Vision10.Checked)
                    strCheckBoxText = chk_Vision10.Text;

                if (strCheckBoxText != "")
                {
                    
                    if (chk_SaveLearnedSetting.Checked)
                    {
                        CopyAllFiles_LearnedSetting(m_strRecipePath + strCheckBoxText, txt_SelectedPath.Text + m_strSelectedRecipeID + DTNow + "\\Learned Setting\\" + strCheckBoxText + " - " + m_strVisionName[i] + "\\");
                        DeleteEmptyDirectory(txt_SelectedPath.Text + m_strSelectedRecipeID + DTNow + "\\Learned Setting\\");
                    }

                    if (chk_SaveCalibrationSetting.Checked)
                    {
                        CopyAllFiles_CalibrationSetting(m_strRecipePath + strCheckBoxText, txt_SelectedPath.Text + m_strSelectedRecipeID + DTNow + "\\Calibration Setting\\" + strCheckBoxText + " - " + m_strVisionName[i] + "\\");
                        DeleteEmptyDirectory(txt_SelectedPath.Text + m_strSelectedRecipeID + DTNow + "\\Calibration Setting\\");
                    }

                    if (chk_SaveToleranceSetting.Checked)
                    {
                        CopyAllFiles_ToleranceSetting(m_strRecipePath + strCheckBoxText, txt_SelectedPath.Text + m_strSelectedRecipeID + DTNow + "\\Tolerance Setting\\" + strCheckBoxText + " - " + m_strVisionName[i] + "\\");
                        DeleteEmptyDirectory(txt_SelectedPath.Text + m_strSelectedRecipeID + DTNow  + "\\Tolerance Setting\\");
                    }

                    if (chk_SaveLighting.Checked)
                    {
                        CopyAllFiles_Lighting(m_strRecipePath, txt_SelectedPath.Text + m_strSelectedRecipeID + DTNow + "\\Lighting Setting\\");
                        DeleteEmptyDirectory(txt_SelectedPath.Text + m_strSelectedRecipeID + DTNow + "\\Lighting Setting\\");
                    }
                    
                    File.WriteAllText(txt_SelectedPath.Text + m_strSelectedRecipeID + DTNow + @"\ReadMe.txt", "Save Date : " + DateTime.Now.ToString());
                    File.AppendAllText(txt_SelectedPath.Text + m_strSelectedRecipeID + DTNow + @"\ReadMe.txt", "\n");
                    File.AppendAllText(txt_SelectedPath.Text + m_strSelectedRecipeID + DTNow + @"\ReadMe.txt", "Vision Software Version : " + Application.ProductVersion);
                }
            }
            

            this.Dispose();
            this.Close();
        }

        private static void DeleteEmptyDirectory(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                DeleteEmptyDirectory(directory);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }

        private void txt_SelectedPath_TextChanged(object sender, EventArgs e)
        {
            if (Directory.Exists(txt_SelectedPath.Text))
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
                subkey.SetValue("RecipeSavePath", txt_SelectedPath.Text);
            }
        }
    }
}
