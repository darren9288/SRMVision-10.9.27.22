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
using VisionProcessing;
namespace AutoMode
{
    public partial class RecipeImportForm : Form
    {
        private string m_strDestinationPath;
        private string m_strSelectedRecipeID;
        private string m_strCurrentSelectedRecipePath;
        private string m_strPath;
        private string m_strRecipePath;
        private string[] m_strVisionName = new string[10];
        private ProductionInfo m_smProductionInfo;
        private int m_intVisionCount = 0;
        private VisionInfo[] m_smVSInfo;
        public RecipeImportForm(ProductionInfo smProductionInfo, string FileName, string strSelectedRecipeID, VisionInfo[] smVSInfo, string strCurrentSelectedRecipe)
        {
            InitializeComponent();
            m_smVSInfo = smVSInfo;
            lbl_RecipeID.Text = m_strSelectedRecipeID = strSelectedRecipeID;
            m_smProductionInfo = smProductionInfo;
            m_strRecipePath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipeID + "\\";
            m_strCurrentSelectedRecipePath = m_smProductionInfo.g_strRecipePath + strCurrentSelectedRecipe + "\\";
            if (FileName == "" || FileName == null)
            {
                RegistryKey keyAutoMode = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subkeyAutoMode = keyAutoMode.CreateSubKey("SVG\\AutoMode");
                FileName = (string)subkeyAutoMode.GetValue("RecipeLoadPath", "D:\\SavedRecipe\\");
                if (!Directory.Exists(FileName))
                    Directory.CreateDirectory(FileName);
            }
            
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG\\Visions");
            string[] strVisionList = subKey.GetSubKeyNames();
            m_intVisionCount = strVisionList.Length;
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
        private static bool CopyAllFiles_LightingSetting(string Source, string Destination)
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
                    CopyAllFiles_LearnedSetting(subdir.FullName, temppath);
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
                    //if (file.Name != "Template.xml")
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
                    if (file.Name == "Template.xml" || file.Name == "Settings.xml")
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


            if (SRMMessageBox.Show("Are you sure you want to delete this recipe?", "Recipe Form", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                for (int i = 0; i < m_intVisionCount; i++)
                {
                    string strCheckBoxText = "";
                    string strSource = "";
                    if (i == 0 && chk_Vision1.Checked)
                    {
                        strCheckBoxText = chk_Vision1.Text;
                        strSource = strCheckBoxText + " - " + m_strVisionName[i];
                    }
                    else if (i == 1 && chk_Vision2.Checked)
                    {
                        strCheckBoxText = chk_Vision2.Text;
                        strSource = strCheckBoxText + " - " + m_strVisionName[i];
                    }
                    else if (i == 2 && chk_Vision3.Checked)
                    {
                        strCheckBoxText = chk_Vision3.Text;
                        strSource = strCheckBoxText + " - " + m_strVisionName[i];
                    }
                    else if (i == 3 && chk_Vision4.Checked)
                    {
                        strCheckBoxText = chk_Vision4.Text;
                        strSource = strCheckBoxText + " - " + m_strVisionName[i];
                    }
                    else if (i == 4 && chk_Vision5.Checked)
                    {
                        strCheckBoxText = chk_Vision5.Text;
                        strSource = strCheckBoxText + " - " + m_strVisionName[i];
                    }
                    else if (i == 5 && chk_Vision6.Checked)
                    {
                        strCheckBoxText = chk_Vision6.Text;
                        strSource = strCheckBoxText + " - " + m_strVisionName[i];
                    }
                    else if (i == 6 && chk_Vision7.Checked)
                    {
                        strCheckBoxText = chk_Vision7.Text;
                        strSource = strCheckBoxText + " - " + m_strVisionName[i];
                    }
                    else if (i == 7 && chk_Vision8.Checked)
                    {
                        strCheckBoxText = chk_Vision8.Text;
                        strSource = strCheckBoxText + " - " + m_strVisionName[i];
                    }
                    else if (i == 8 && chk_Vision9.Checked)
                    {
                        strCheckBoxText = chk_Vision9.Text;
                        strSource = strCheckBoxText + " - " + m_strVisionName[i];
                    }
                    else if (i == 9 && chk_Vision10.Checked)
                    {
                        strCheckBoxText = chk_Vision10.Text;
                        strSource = strCheckBoxText + " - " + m_strVisionName[i];
                    }

                    if (strCheckBoxText != "")
                    {
                        string Source = txt_SelectedPath.Text;
                        string Destination = m_strRecipePath;

                        if (chk_LoadLighting.Checked)
                        {
                            string LightingSource = Source + "Lighting Setting\\" + strCheckBoxText;
                            if (Directory.Exists(LightingSource))
                            {
                                SaveLighting(Source + "Lighting Setting\\", Destination, strCheckBoxText);
                                CopyAllFiles_LightingSetting(LightingSource, Destination + strCheckBoxText);
                                m_smProductionInfo.g_blnRecipeImported[i] = true;
                            }
                        }

                        if (chk_LoadLearnedSetting.Checked)
                        {
                            string LearnedSettingSource = Source + "Learned Setting\\" + strSource;
                            if (Directory.Exists(LearnedSettingSource))
                            {
                                CopyAllFiles_LearnedSetting(LearnedSettingSource, Destination + strCheckBoxText);
                                //LoadLearnedSetting(LearnedSettingSource, Destination + "\\" + strCheckBoxText, strCheckBoxText, i);

                                m_smProductionInfo.g_blnRecipeImported[i] = true;
                            }
                        }

                        if (chk_LoadCalibrationSetting.Checked)
                        {
                            string CalibrationSettingSource = Source + "Calibration Setting\\" + strSource;
                            if (Directory.Exists(CalibrationSettingSource))
                                CopyAllFiles(CalibrationSettingSource, Destination + strCheckBoxText);
                            m_smProductionInfo.g_blnRecipeImported[i] = true;
                        }

                        if (chk_LoadToleranceSetting.Checked)
                        {
                            string ToleranceSettingSource = Source + "Tolerance Setting\\" + strSource;
                            if (Directory.Exists(ToleranceSettingSource))
                                LoadToleranceSetting(ToleranceSettingSource, Destination + strCheckBoxText, strCheckBoxText, i);
                            m_smProductionInfo.g_blnRecipeImported[i] = true;
                        }
                    }
                }
            }
            else
            {
                return;
            }

            this.Dispose();
            this.Close();
        }

        private void txt_SelectedPath_TextChanged(object sender, EventArgs e)
        {
            //m_intVisionCount = Directory.GetDirectories(m_strRecipePath).Length;
            chk_Vision1.Visible = chk_Vision1.Checked = false;
            chk_Vision2.Visible = chk_Vision2.Checked = false;
            chk_Vision3.Visible = chk_Vision3.Checked = false;
            chk_Vision4.Visible = chk_Vision4.Checked = false;
            chk_Vision5.Visible = chk_Vision5.Checked = false;
            chk_Vision6.Visible = chk_Vision6.Checked = false;
            chk_Vision7.Visible = chk_Vision7.Checked = false;
            chk_Vision8.Visible = chk_Vision8.Checked = false;
            chk_Vision9.Visible = chk_Vision9.Checked = false;
            chk_Vision10.Visible = chk_Vision10.Checked = false;
            chk_LoadCalibrationSetting.Visible = chk_LoadCalibrationSetting.Checked = false;
            chk_LoadLearnedSetting.Visible = chk_LoadLearnedSetting.Checked = false;
            chk_LoadLighting.Visible = chk_LoadLighting.Checked = false;
            chk_LoadToleranceSetting.Visible = chk_LoadToleranceSetting.Checked = false;

            if (Directory.Exists(txt_SelectedPath.Text))
            {
                DirectoryInfo dir = new DirectoryInfo(txt_SelectedPath.Text);
                DirectoryInfo[] dirs = dir.GetDirectories();
                if (dirs.Length > 0)
                {
                    int i = 0;
                    foreach (DirectoryInfo subdirs in dirs)
                    {
                        if (subdirs.Name == "Lighting Setting")
                            chk_LoadLighting.Visible = chk_LoadLighting.Checked = true;
                        else if (subdirs.Name == "Learned Setting")
                            chk_LoadLearnedSetting.Visible = chk_LoadLearnedSetting.Checked = true;
                        else if (subdirs.Name == "Calibration Setting")
                            chk_LoadCalibrationSetting.Visible = chk_LoadCalibrationSetting.Checked = true;
                        else if (subdirs.Name == "Tolerance Setting")
                            chk_LoadToleranceSetting.Visible = chk_LoadToleranceSetting.Checked = true;

                        DirectoryInfo[] dirss = dirs[i].GetDirectories();
                        foreach (DirectoryInfo subdir in dirss)
                        {
                            string[] VisionName = subdir.Name.Split('-');

                            if (VisionName.Length == 2)
                            {
                                string strVisionNo = VisionName[0].Substring(VisionName[0].Length - 2, 1);
                                int intVisionNo;
                                if (Int32.TryParse(strVisionNo, out intVisionNo))
                                {
                                    if (VisionName[1].TrimStart() == m_strVisionName[intVisionNo - 1])
                                    {
                                        switch (intVisionNo)
                                        {
                                            case 1:
                                                chk_Vision1.Visible = true;
                                                chk_Vision1.Checked = true;
                                                break;
                                            case 2:
                                                chk_Vision2.Visible = true;
                                                chk_Vision2.Checked = true;
                                                break;
                                            case 3:
                                                chk_Vision3.Visible = true;
                                                chk_Vision3.Checked = true;
                                                break;
                                            case 4:
                                                chk_Vision4.Visible = true;
                                                chk_Vision4.Checked = true;
                                                break;
                                            case 5:
                                                chk_Vision5.Visible = true;
                                                chk_Vision5.Checked = true;
                                                break;
                                            case 6:
                                                chk_Vision6.Visible = true;
                                                chk_Vision6.Checked = true;
                                                break;
                                            case 7:
                                                chk_Vision7.Visible = true;
                                                chk_Vision7.Checked = true;
                                                break;
                                            case 8:
                                                chk_Vision8.Visible = true;
                                                chk_Vision8.Checked = true;
                                                break;
                                            case 9:
                                                chk_Vision9.Visible = true;
                                                chk_Vision9.Checked = true;
                                                break;
                                            case 10:
                                                chk_Vision10.Visible = true;
                                                chk_Vision10.Checked = true;
                                                break;

                                        }
                                    }
                                }
                            }
                        }
                        i++;
                    }
                    if (chk_LoadLighting.Checked == true)
                    {
                        for (int j = 0; j < m_intVisionCount; j++)
                        {
                            string Source = txt_SelectedPath.Text;
                            int intVisionNo = GetLighting(Source + "Lighting Setting", "Vision"+(j+1).ToString());
                            switch (intVisionNo)
                            {
                                case 1:
                                    chk_Vision1.Visible = true;
                                    chk_Vision1.Checked = true;
                                    break;
                                case 2:
                                    chk_Vision2.Visible = true;
                                    chk_Vision2.Checked = true;
                                    break;
                                case 3:
                                    chk_Vision3.Visible = true;
                                    chk_Vision3.Checked = true;
                                    break;
                                case 4:
                                    chk_Vision4.Visible = true;
                                    chk_Vision4.Checked = true;
                                    break;
                                case 5:
                                    chk_Vision5.Visible = true;
                                    chk_Vision5.Checked = true;
                                    break;
                                case 6:
                                    chk_Vision6.Visible = true;
                                    chk_Vision6.Checked = true;
                                    break;
                                case 7:
                                    chk_Vision7.Visible = true;
                                    chk_Vision7.Checked = true;
                                    break;
                                case 8:
                                    chk_Vision8.Visible = true;
                                    chk_Vision8.Checked = true;
                                    break;
                                case 9:
                                    chk_Vision9.Visible = true;
                                    chk_Vision9.Checked = true;
                                    break;
                                case 10:
                                    chk_Vision10.Visible = true;
                                    chk_Vision10.Checked = true;
                                    break;

                            }
                        }
                    }
                }
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
                subkey.SetValue("RecipeLoadPath", txt_SelectedPath.Text);
            }
        }

        private bool CopyAllFiles_UsingFilter(string Source, string Destination)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(Source);
                DirectoryInfo[] dirs = dir.GetDirectories();

                if (!dir.Exists)
                {
                    return false;
                }

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
                        if (chk_LoadLighting.Checked)
                        {
                            string LightingDestination = Destination + "\\Lighting Setting";
                            if (!Directory.Exists(LightingDestination))
                            {
                                Directory.CreateDirectory(LightingDestination);
                            }
                            string temppath = Path.Combine(LightingDestination, file.Name);
                            file.CopyTo(temppath, true);
                        }
                    }
                }

                //m_objForm.ref_intSavingState = 3;
                foreach (DirectoryInfo subdir in dirs)
                {

                    if (chk_LoadLearnedSetting.Checked)
                    {
                        string LearnedSettingDestination = Destination + "\\Learned Setting";
                        if (!Directory.Exists(LearnedSettingDestination))
                        {
                            Directory.CreateDirectory(LearnedSettingDestination);
                        }
                        string temppath = Path.Combine(LearnedSettingDestination, subdir.Name);
                        CopyAllFiles_WithoutFilter(subdir.FullName, temppath);
                    }
                    if (chk_LoadCalibrationSetting.Checked)
                    {
                        string CalibrationSettingDestination = Destination + "\\Calibration Setting";
                        if (!Directory.Exists(CalibrationSettingDestination))
                        {
                            Directory.CreateDirectory(CalibrationSettingDestination);
                        }
                        string temppath = Path.Combine(CalibrationSettingDestination, subdir.Name);
                        CopyAllFiles_WithoutFilter(subdir.FullName, temppath);
                    }
                    if (chk_LoadCalibrationSetting.Checked)
                    {
                        string ToleranceSettingDestination = Destination + "\\Tolerance Setting";
                        if (!Directory.Exists(ToleranceSettingDestination))
                        {
                            Directory.CreateDirectory(ToleranceSettingDestination);
                        }
                        string temppath = Path.Combine(ToleranceSettingDestination, subdir.Name);
                        CopyAllFiles_WithoutFilter(subdir.FullName, temppath);
                    }
                }

            }
            catch
            {
            }
            return true;
        }
        private static bool CopyAllFiles_WithoutFilter(string Source, string Destination)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = new DirectoryInfo(Source);
                DirectoryInfo[] dirs = dir.GetDirectories();

                if (!dir.Exists)
                {
                  
                    return false;
                }

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
                    CopyAllFiles_WithoutFilter(subdir.FullName, temppath);
                }

            }
            catch
            {
               
            }
            return true;
        }

        private void SaveLighting(string strSourcePath, string strDestinationPath, string SectionName)
        {
            XmlParser objDestinationFile = new XmlParser(strDestinationPath+"Camera.xml");
            objDestinationFile.WriteSectionElement(SectionName, true);
            objDestinationFile.CopyNode(strSourcePath + "\\Camera.xml", SectionName);
            objDestinationFile.WriteEndElement();
        }
        private int GetLighting(string strSourcePath, string SectionName)
        {
            XmlParser objDestinationFile = new XmlParser(strSourcePath + "\\Camera.xml");
            int intVisionNo = objDestinationFile.GetFirstSectionCount();
            int SectionNodesCount = 0;
            for (int i = 0; i < intVisionNo; i++)
            {
                string SectionElement = objDestinationFile.GetSectionElementName(i);
                if (SectionElement == SectionName)
                {
                    SectionNodesCount = objDestinationFile.GetSectionNodesCount(i);
                    if (SectionNodesCount > 0)
                    {
                        
                        return Convert.ToInt32(SectionName.Substring(SectionName.Length - 1, 1));
                    }
                }
            }
            return SectionNodesCount;
        }

        private void LoadToleranceSetting(string strSourcePath, string strDestinationPath, string VisionModuleNo, int intVisionNo)
        {
            if (intVisionNo < m_smVSInfo.Length)
            {
                if (m_smVSInfo[intVisionNo] != null)
                {
                    DirectoryInfo dir = new DirectoryInfo(strSourcePath);
                    DirectoryInfo[] dirs = dir.GetDirectories();

                    if (!dir.Exists)
                        return;
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        //CopyAllFiles_ToleranceSetting(subdir.FullName, m_strCurrentSelectedRecipePath + VisionModuleNo + "\\" + subdir.Name + "\\");
                        switch (subdir.Name)
                        {
                            case "Package":
                                for (int u = 0; u < m_smVSInfo[intVisionNo].g_intUnitsOnImage; u++)
                                {
                                    if (m_smVSInfo[intVisionNo].g_arrPackage[u] != null)
                                    {
                                        if (u == 0)
                                        {
                                            // 1: Load Package from Recipe to be overwrite
                                            m_smVSInfo[intVisionNo].g_arrPackage[u].LoadPackage(m_strRecipePath + VisionModuleNo + "\\Package\\Settings.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                            // 2: Load Tolerance from destination path
                                            m_smVSInfo[intVisionNo].g_arrPackage[u].LoadPackageToleranceOnly(subdir.FullName + "\\Settings.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                            // 3: Save the Package setting back to recipe to be overwrite
                                            m_smVSInfo[intVisionNo].g_arrPackage[u].SavePackage(m_strRecipePath + VisionModuleNo + "\\Package\\Settings.xml", false, "Settings", false, m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                            // 4: Load back Package setting from current selected recipe so that everything back to normal
                                            m_smVSInfo[intVisionNo].g_arrPackage[u].LoadPackage(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Package\\Settings.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                        }
                                        else
                                        {
                                            // 1: Load Package from Recipe to be overwrite
                                            if (File.Exists(m_strRecipePath + VisionModuleNo + "\\Package\\Settings2.xml"))
                                                m_smVSInfo[intVisionNo].g_arrPackage[u].LoadPackage(m_strRecipePath + VisionModuleNo + "\\Package\\Settings2.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                            else
                                                m_smVSInfo[intVisionNo].g_arrPackage[u].LoadPackage(m_strRecipePath + VisionModuleNo + "\\Package\\Settings.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                            // 2: Load Tolerance from destination path
                                            if (File.Exists(subdir.FullName + "\\Settings2.xml"))
                                                m_smVSInfo[intVisionNo].g_arrPackage[u].LoadPackageToleranceOnly(subdir.FullName + "\\Settings2.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                           else
                                                m_smVSInfo[intVisionNo].g_arrPackage[u].LoadPackageToleranceOnly(subdir.FullName + "\\Settings.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                            // 3: Save the Package setting back to recipe to be overwrite
                                            if (File.Exists(m_strRecipePath + VisionModuleNo + "\\Package\\Settings2.xml"))
                                                m_smVSInfo[intVisionNo].g_arrPackage[u].SavePackage(m_strRecipePath + VisionModuleNo + "\\Package\\Settings2.xml", false, "Settings", false, m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                            else
                                                m_smVSInfo[intVisionNo].g_arrPackage[u].SavePackage(m_strRecipePath + VisionModuleNo + "\\Package\\Settings.xml", false, "Settings", false, m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                            // 4: Load back Package setting from current selected recipe so that everything back to normal
                                            if (File.Exists(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Package\\Settings2.xml"))
                                                m_smVSInfo[intVisionNo].g_arrPackage[u].LoadPackage(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Package\\Settings2.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                            else
                                                m_smVSInfo[intVisionNo].g_arrPackage[u].LoadPackage(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Package\\Settings.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                        }
                                    }
                                }
                                break;
                            case "Mark":
                                for (int u = 0; u < m_smVSInfo[intVisionNo].g_intUnitsOnImage; u++)
                                {
                                    if (m_smVSInfo[intVisionNo].g_arrMarks[u] != null)
                                    {
                                        // 1: Load Mark from Recipe to be overwrite
                                        m_smVSInfo[intVisionNo].g_arrMarks[u].LoadTemplate(m_strRecipePath + VisionModuleNo + "\\Mark\\Template\\", m_smVSInfo[intVisionNo].g_arrMarkROIs, m_smVSInfo[intVisionNo].g_arrMarkDontCareROIs.Count, m_smVSInfo[intVisionNo].g_objWhiteImage);
                                        // 2: Load Tolerance from destination path
                                        m_smVSInfo[intVisionNo].g_arrMarks[u].LoadTemplateToleranceOnly(subdir.FullName + "\\Template\\");
                                        // 3: Save the Mark setting back to recipe to be overwrite
                                        m_smVSInfo[intVisionNo].g_arrMarks[u].SaveTemplate(m_strRecipePath + VisionModuleNo + "\\Mark\\Template\\", false);
                                        // 4: Load back Mark setting from current selected recipe so that everything back to normal
                                        m_smVSInfo[intVisionNo].g_arrMarks[u].LoadTemplate(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Mark\\Template\\", m_smVSInfo[intVisionNo].g_arrMarkROIs, m_smVSInfo[intVisionNo].g_arrMarkDontCareROIs.Count, m_smVSInfo[intVisionNo].g_objWhiteImage);
                                    }
                                }
                                break;
                            case "Orient":
                                // 1: Load Tolerance from destination path
                                XmlParser objFileTolerance = new XmlParser(subdir.FullName + "\\Template\\Template.xml");
                                int intParentCountTolerance = objFileTolerance.GetFirstSectionCount();

                                // 2: Save the Orient setting to recipe to be overwrite
                                XmlParser objFile = new XmlParser(m_strRecipePath + VisionModuleNo + "\\Orient\\Template\\Template.xml");

                                for (int i = 0; i < intParentCountTolerance; i++)
                                {
                                    objFileTolerance.GetFirstSection("Template" + i);
                                    objFile.WriteSectionElement("Template" + i);
                                    objFile.WriteElement1Value("MinScore", objFileTolerance.GetValueAsFloat("MinScore", 0.7f));
                                    objFile.WriteElement1Value("MaxAngle", objFileTolerance.GetValueAsFloat("MaxAngle", 0));
                                    objFile.WriteElement1Value("MaxX", objFileTolerance.GetValueAsFloat("MaxX", 0));
                                    objFile.WriteElement1Value("MaxY", objFileTolerance.GetValueAsFloat("MaxY", 0));
                                }
                                objFile.WriteEndElement();

                                //3: Load back setting from current selected recipe so that everything back to normal
                                XmlParser objFileHandle = new XmlParser(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Orient\\Template\\Template.xml");
                                for (int j = 0; j < m_smVSInfo[intVisionNo].g_intUnitsOnImage; j++)
                                {
                                    for (int i = 0; i < intParentCountTolerance; i++)
                                    {
                                        objFileHandle.GetFirstSection("Template" + i);
                                        m_smVSInfo[intVisionNo].g_arrOrients[j][i].ref_fMinScore = objFileHandle.GetValueAsFloat("MinScore", 0.7f);
                                        m_smVSInfo[intVisionNo].g_arrOrients[j][i].ref_fAngleTolerance = objFileHandle.GetValueAsFloat("MaxAngle", 0);
                                        m_smVSInfo[intVisionNo].g_arrOrients[j][i].ref_fXTolerance = objFileHandle.GetValueAsFloat("MaxX", 0);
                                        m_smVSInfo[intVisionNo].g_arrOrients[j][i].ref_fYTolerance = objFileHandle.GetValueAsFloat("MaxY", 0);
                                    }
                                    if (m_smVSInfo[intVisionNo].g_arrPin1 != null)
                                    {
                                        if (m_smVSInfo[intVisionNo].g_arrPin1.Count > j)
                                        {
                                            // 1: Load Pin1 from Recipe to be overwrite
                                            m_smVSInfo[intVisionNo].g_arrPin1[j].LoadTemplate(m_strRecipePath + VisionModuleNo + "\\Orient\\Template\\");
                                            // 2: Load Tolerance from destination path
                                            m_smVSInfo[intVisionNo].g_arrPin1[j].LoadTemplate(subdir.FullName + "\\Template\\");
                                            // 3: Save the Pin1 setting back to recipe to be overwrite
                                            m_smVSInfo[intVisionNo].g_arrPin1[j].SavePin1Setting(m_strRecipePath + VisionModuleNo + "\\Orient\\Template\\");
                                            // 4: Load back Pin1 setting from current selected recipe so that everything back to normal
                                            m_smVSInfo[intVisionNo].g_arrPin1[j].LoadTemplate(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Orient\\Template\\");
                                        }
                                          
                                        
                                    }
                                }
                                break;
                            case "BottomOrientPad":
                            case "BottomOPadPkg":
                            case "Pad":
                                if (m_smVSInfo[intVisionNo].g_arrPad != null)
                                {
                                    for (int i = 0; i < m_smVSInfo[intVisionNo].g_arrPad.Length; i++)
                                    {
                                        if (i > 0 && !m_smVSInfo[intVisionNo].g_blnCheck4Sides)
                                            break;

                                        string strSectionName = "";
                                        if (i == 0)
                                            strSectionName = "CenterROI";
                                        else if (i == 1)
                                            strSectionName = "TopROI";
                                        else if (i == 2)
                                            strSectionName = "RightROI";
                                        else if (i == 3)
                                            strSectionName = "BottomROI";
                                        else if (i == 4)
                                            strSectionName = "LeftROI";

                                        if (File.Exists(m_strRecipePath + VisionModuleNo + "\\Pad\\Template\\Template.xml"))
                                        {
                                            // 1: Load Pad from Recipe to be overwrite
                                            m_smVSInfo[intVisionNo].g_arrPad[i].LoadPad(m_strRecipePath + VisionModuleNo + "\\Pad\\Template\\Template.xml", strSectionName);
                                            // 2: Load Tolerance from destination path
                                            m_smVSInfo[intVisionNo].g_arrPad[i].LoadPadToleranceOnly(subdir.FullName + "\\Template\\Template.xml", strSectionName);
                                            // 3: Save the pad setting back to recipe to be overwrite
                                            m_smVSInfo[intVisionNo].g_arrPad[i].SavePad(m_strRecipePath + VisionModuleNo + "\\Pad\\Template\\Template.xml", false, strSectionName, false);
                                            // 4: Load back pad setting from current selected recipe so that everything back to normal
                                            m_smVSInfo[intVisionNo].g_arrPad[i].LoadPad(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Pad\\Template\\Template.xml", strSectionName);
                                        }
                                    }
                                }
                                if (m_smVSInfo[intVisionNo].g_arrPin1 != null)
                                {
                                    if (m_smVSInfo[intVisionNo].g_arrPin1.Count > 0)
                                    {
                                        if (File.Exists(m_strRecipePath + VisionModuleNo + "\\Pad\\Template\\Pin1Template.xml"))
                                        {
                                            // 1: Load Pin1 from Recipe to be overwrite
                                            m_smVSInfo[intVisionNo].g_arrPin1[0].LoadTemplate(m_strRecipePath + VisionModuleNo + "\\Pad\\Template\\");
                                            // 2: Load Tolerance from destination path
                                            m_smVSInfo[intVisionNo].g_arrPin1[0].LoadTemplate(subdir.FullName + "\\Template\\");
                                            // 3: Save the Pin1 setting back to recipe to be overwrite
                                            m_smVSInfo[intVisionNo].g_arrPin1[0].SaveTemplate(m_strRecipePath + VisionModuleNo + "\\Pad\\Template\\");
                                            // 4: Load back Pin1 setting from current selected recipe so that everything back to normal
                                            m_smVSInfo[intVisionNo].g_arrPin1[0].LoadTemplate(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Pad\\Template\\");
                                        }
                                    }
                                       
                                }
                                break;
                            case "Seal":
                                if (m_smVSInfo[intVisionNo].g_objSeal != null)
                                {
                                    if (File.Exists(m_strRecipePath + VisionModuleNo + "\\Seal\\Settings.xml"))
                                    {
                                        // 1: Load Seal from Recipe to be overwrite
                                        m_smVSInfo[intVisionNo].g_objSeal.LoadSeal(m_strRecipePath + VisionModuleNo + "\\Seal\\Settings.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX);
                                        // 2: Load Tolerance from destination path
                                        m_smVSInfo[intVisionNo].g_objSeal.LoadSealToleranceOnly(subdir.FullName + "\\Settings.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX);
                                        // 3: Save the Seal setting back to recipe to be overwrite
                                        m_smVSInfo[intVisionNo].g_objSeal.SaveSeal(m_strRecipePath + VisionModuleNo + "\\Seal\\Settings.xml", false, "Settings", false, m_smVSInfo[intVisionNo].g_fCalibPixelX);
                                        // 4: Load back Seal setting from current selected recipe so that everything back to normal
                                        m_smVSInfo[intVisionNo].g_objSeal.LoadSeal(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Seal\\Settings.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX);
                                    }
                                    
                                }
                                break;
                            case "Positioning":
                                if (m_smVSInfo[intVisionNo].g_objPositioning != null)
                                {
                                    if (File.Exists(m_strRecipePath + VisionModuleNo + "\\Positioning\\Settings.xml"))
                                    {
                                        // 1: Load Positioning from Recipe to be overwrite
                                        m_smVSInfo[intVisionNo].g_objPositioning.LoadPosition(m_strRecipePath + VisionModuleNo + "\\Positioning\\Settings.xml", "General");
                                        // 2: Load Tolerance from destination path
                                        m_smVSInfo[intVisionNo].g_objPositioning.LoadPosition(subdir.FullName + "\\Settings.xml", "General");
                                        // 3: Save the Positioning setting back to recipe to be overwrite
                                        m_smVSInfo[intVisionNo].g_objPositioning.SavePosition(m_strRecipePath + VisionModuleNo + "\\Positioning\\Settings.xml", false, "General", true);
                                        // 4: Load back Positioning setting from current selected recipe so that everything back to normal
                                        m_smVSInfo[intVisionNo].g_objPositioning.LoadPosition(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Positioning\\Settings.xml", "General");
                                    }

                                }
                                break;
                            case "PocketPosition":
                                if (m_smVSInfo[intVisionNo].g_objPocketPosition != null)
                                {
                                    if (File.Exists(m_strRecipePath + VisionModuleNo + "\\PocketPosition\\Settings.xml"))
                                    {
                                        // 1: Load PocketPosition from Recipe to be overwrite
                                        m_smVSInfo[intVisionNo].g_objPocketPosition.LoadPocketPosition(m_strRecipePath + VisionModuleNo + "\\PocketPosition\\Settings.xml", "Settings");
                                        // 2: Load Tolerance from destination path
                                        m_smVSInfo[intVisionNo].g_objPocketPosition.LoadPocketPosition(subdir.FullName + "\\Settings.xml", "Settings");
                                        // 3: Save the PocketPosition setting back to recipe to be overwrite
                                        m_smVSInfo[intVisionNo].g_objPocketPosition.SavePocketPosition(m_strRecipePath + VisionModuleNo + "\\PocketPosition\\Settings.xml", false, "Settings", true);
                                        // 4: Load back PocketPosition setting from current selected recipe so that everything back to normal
                                        m_smVSInfo[intVisionNo].g_objPocketPosition.LoadPocketPosition(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\PocketPosition\\Settings.xml", "Settings");
                                    }

                                }
                                break;
                        }
                    }
                }
            }
        }
        private void LoadLearnedSetting(string strSourcePath, string strDestinationPath, string VisionModuleNo, int intVisionNo)
        {
            //1: Load From destination path
            //2: Load Tolerance from Recipe to be overwrite
            // 3: Save the setting back to recipe to be overwrite
            // 4: Load back setting from current selected recipe so that everything back to normal
            if (intVisionNo < m_smVSInfo.Length)
            {
                if (m_smVSInfo[intVisionNo] != null)
                {
                    DirectoryInfo dir = new DirectoryInfo(strSourcePath);
                    DirectoryInfo[] dirs = dir.GetDirectories();

                    if (!dir.Exists)
                        return;
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        switch (subdir.Name)
                        {
                            case "Package":
                                for (int u = 0; u < m_smVSInfo[intVisionNo].g_intUnitsOnImage; u++)
                                {
                                    if (m_smVSInfo[intVisionNo].g_arrPackage[u] != null)
                                    {
                                        //1: Load From destination path
                                        m_smVSInfo[intVisionNo].g_arrPackage[u].LoadPackage(subdir.FullName + "\\Template\\Template.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                        //2: Load Tolerance from Recipe to be overwrite
                                        m_smVSInfo[intVisionNo].g_arrPackage[u].LoadPackageToleranceOnly(m_strRecipePath + VisionModuleNo + "\\Package\\Template\\Template.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                        // 3: Save the Package setting back to recipe to be overwrite
                                        m_smVSInfo[intVisionNo].g_arrPackage[u].SavePackage(m_strRecipePath + VisionModuleNo + "\\Package\\Template\\Template.xml", false, "Settings", false, m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                        // 4: Load back Package setting from current selected recipe so that everything back to normal
                                        m_smVSInfo[intVisionNo].g_arrPackage[u].LoadPackage(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Package\\Template\\Template.xml", "Settings", m_smVSInfo[intVisionNo].g_fCalibPixelX, m_smVSInfo[intVisionNo].g_fCalibPixelY);
                                    }
                                }
                                break;
                            case "Mark":
                                for (int u = 0; u < m_smVSInfo[intVisionNo].g_intUnitsOnImage; u++)
                                {
                                    if (m_smVSInfo[intVisionNo].g_arrMarks[u] != null)
                                    {
                                        //1: Load From destination path
                                        m_smVSInfo[intVisionNo].g_arrMarks[u].LoadTemplate(subdir.FullName + "\\Template\\", m_smVSInfo[intVisionNo].g_arrMarkROIs, m_smVSInfo[intVisionNo].g_arrMarkDontCareROIs.Count, m_smVSInfo[intVisionNo].g_objWhiteImage);
                                        //2: Load Tolerance from Recipe to be overwrite
                                        m_smVSInfo[intVisionNo].g_arrMarks[u].LoadTemplateToleranceOnly(m_strRecipePath + VisionModuleNo + "\\Mark\\Template\\");
                                        // 3: Save the Mark setting back to recipe to be overwrite
                                        m_smVSInfo[intVisionNo].g_arrMarks[u].SaveTemplate(m_strRecipePath + VisionModuleNo + "\\Mark\\Template\\", true);
                                        // 4: Load back Mark setting from current selected recipe so that everything back to normal
                                        m_smVSInfo[intVisionNo].g_arrMarks[u].LoadTemplate(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Mark\\Template\\", m_smVSInfo[intVisionNo].g_arrMarkROIs, m_smVSInfo[intVisionNo].g_arrMarkDontCareROIs.Count, m_smVSInfo[intVisionNo].g_objWhiteImage);
                                    }
                                }
                                break;
                            case "Orient":
                                // 1: Load Setting from destination path
                                XmlParser objFileHandler = new XmlParser(subdir.FullName + "\\Template\\Template.xml");
                                int intParentCount = objFileHandler.GetFirstSectionCount();

                                // 2: Save the Orient setting to recipe to be overwrite
                                XmlParser objFile = new XmlParser(m_strRecipePath + VisionModuleNo + "\\Orient\\Template\\Template.xml");

                                for (int i = 0; i < intParentCount; i++)
                                {
                                    objFile.WriteSectionElement("Template" + i);
                                    objFile.WriteElement1Value("SubMatcherCount", objFileHandler.GetValueAsInt("SubMatcherCount", 0));
                                    objFile.WriteElement1Value("TemplateCenterX", objFileHandler.GetValueAsFloat("TemplateCenterX", 0));
                                    objFile.WriteElement1Value("TemplateCenterY", objFileHandler.GetValueAsFloat("TemplateCenterY", 0));
                                }
                                objFile.WriteEndElement();

                                //3: Load back setting from current selected recipe so that everything back to normal
                                XmlParser objFileHandle = new XmlParser(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Orient\\Template\\Template.xml");
                                for (int j = 0; j < m_smVSInfo[intVisionNo].g_intUnitsOnImage; j++)
                                {
                                    if (m_smVSInfo[intVisionNo].g_arrOrients.Count <= j)
                                        m_smVSInfo[intVisionNo].g_arrOrients.Add(new List<Orient>());

                                    for (int i = 0; i < intParentCount; i++)
                                    {
                                        objFileHandle.GetFirstSection("Template" + i);
                                        if (m_smVSInfo[intVisionNo].g_arrOrients[j].Count <= i)
                                            m_smVSInfo[intVisionNo].g_arrOrients[j].Add(new Orient(m_smVSInfo[intVisionNo].g_intCameraResolutionWidth, m_smVSInfo[intVisionNo].g_intCameraResolutionHeight));
                                        m_smVSInfo[intVisionNo].g_arrOrients[j][i].ref_fTemplateX = objFileHandle.GetValueAsFloat("TemplateCenterX", 0);
                                        m_smVSInfo[intVisionNo].g_arrOrients[j][i].ref_fTemplateY = objFileHandle.GetValueAsFloat("TemplateCenterY", 0);
                                    }
                                }

                                break;
                            case "BottomOrientPad":
                            case "BottomOPadPkg":
                            case "Pad":
                                if (m_smVSInfo[intVisionNo].g_arrPad != null)
                                {
                                    for (int i = 0; i < m_smVSInfo[intVisionNo].g_arrPad.Length; i++)
                                    {
                                        if (i > 0 && !m_smVSInfo[intVisionNo].g_blnCheck4Sides)
                                            break;

                                        string strSectionName = "";
                                        if (i == 0)
                                            strSectionName = "CenterROI";
                                        else if (i == 1)
                                            strSectionName = "TopROI";
                                        else if (i == 2)
                                            strSectionName = "RightROI";
                                        else if (i == 3)
                                            strSectionName = "BottomROI";
                                        else if (i == 4)
                                            strSectionName = "LeftROI";

                                        if (File.Exists(m_strRecipePath + VisionModuleNo + "\\Pad\\Template\\Template.xml"))
                                        {
                                            //1: Load From destination path
                                            m_smVSInfo[intVisionNo].g_arrPad[i].LoadPad(subdir.FullName + "\\Template\\Template.xml", strSectionName);
                                            //2: Load Tolerance from Recipe to be overwrite
                                            m_smVSInfo[intVisionNo].g_arrPad[i].LoadPadToleranceOnly(m_strRecipePath + VisionModuleNo + "\\Pad\\Template\\Template.xml", strSectionName);
                                            // 3: Save the pad setting back to recipe to be overwrite
                                            m_smVSInfo[intVisionNo].g_arrPad[i].SavePad(m_strRecipePath + VisionModuleNo + "\\Pad\\Template\\Template.xml", false, strSectionName, false);
                                            // 4: Load back pad setting from current selected recipe so that everything back to normal
                                            m_smVSInfo[intVisionNo].g_arrPad[i].LoadPad(m_strCurrentSelectedRecipePath + VisionModuleNo + "\\Pad\\Template\\Template.xml", strSectionName);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
