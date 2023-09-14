using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using SharedMemory;
using System.IO;
using Common;
using Microsoft.Win32;
namespace AutoMode
{
    public partial class SaveOptionForm : Form
    {
        private string m_strImageFileName = "CurrentImage"; 
        private string m_strPath;
        private string m_strRecipePath;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;

        public SaveOptionForm(VisionInfo smVisionInfo, ProductionInfo smProductionInfo)//, string FileName)
        {
            InitializeComponent();
            m_smVisionInfo = smVisionInfo;
            m_smProductionInfo = smProductionInfo;
             m_strRecipePath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\";
            cbo_RecipeSaveMode.SelectedIndex = 1;
            //if (FileName == "" || FileName == null)
            //{
            //    FileName = "D:\\SaveOption\\";
            //    if(!Directory.Exists(FileName))
            //    Directory.CreateDirectory(FileName);
            //}
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");

            DirectoryInfo dir = new DirectoryInfo(subkey.GetValue("SaveOptionPath", "D:\\SaveOption\\CurrentImage.bmp").ToString());

            string strSplit = dir.FullName.Substring(0, dir.FullName.Length - dir.Name.Length);
            txt_SelectedPath.Text = m_strPath = strSplit;
            txt_ImageName.Text = m_strImageFileName = dir.Name;
            if (m_strImageFileName.LastIndexOf(".") > 0)
                m_strImageFileName = m_strImageFileName.Remove(m_strImageFileName.LastIndexOf("."));
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
            catch (Exception ex)
            {
          
            }
            return true;
        }

        public void ExportKey(string exportPath, string registryPath)
        {
            string path = "\"" + exportPath + "\"";
            string key = "\"" + registryPath + "\"";
            Process proc = new Process();

            try
            {
                proc.StartInfo.FileName = "regedit.exe";
                proc.StartInfo.UseShellExecute = false;

                proc = Process.Start("regedit.exe", "/e " + path + " " + key);
                proc.WaitForExit();
            }
            catch (Exception)
            {
                proc.Dispose();
            }
        }

        private void btn_Browse_Click(object sender, EventArgs e)
        {

            //folderBrowserDialog1.Reset();
            //folderBrowserDialog1.SelectedPath = m_strPath;

            //if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            //{
            //    txt_SelectedPath.Text = folderBrowserDialog1.SelectedPath+"\\";

            //}
            if (dlg_SaveImageFile.ShowDialog() == DialogResult.OK)
            {
                string strDirPath = Path.GetDirectoryName(dlg_SaveImageFile.FileName);
               DirectoryInfo dir = new DirectoryInfo(dlg_SaveImageFile.FileName);
                string[] strsplit = dir.Name.Split('.');
                m_strImageFileName = strsplit[0];
                txt_ImageName.Text = m_strImageFileName + ".bmp";
                txt_SelectedPath.Text = strDirPath + "\\";
            }

            if (Directory.Exists(txt_SelectedPath.Text))
                Directory.SetCurrentDirectory(txt_SelectedPath.Text);
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
        private bool WantSaveImageAccordingMergeType(int intImageIndex)
        {
            switch (m_smVisionInfo.g_intImageMergeType)
            {
                case 0:
                    return true;
                    break;
                case 1:
                    if (intImageIndex == 1)
                        return false;
                    break;
                case 2:
                    if ((intImageIndex == 1) || (intImageIndex == 2))
                        return false;
                    break;
                case 3:
                    if ((intImageIndex == 1) || (intImageIndex == 3))
                        return false;
                    break;
                case 4:
                    if ((intImageIndex == 1) || (intImageIndex == 2) || (intImageIndex == 4))
                        return false;
                    break;
            }

            return true;
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(txt_SelectedPath.Text))
                {
                    try
                    {
                        Directory.CreateDirectory(txt_SelectedPath.Text);
                    }
                    catch
                    {
                        SRMMessageBox.Show("Cannot Find Selected Path!");
                        return;
                    }

                    //SRMMessageBox.Show("Cannot Find Selected Path!");
                    //return;
                }

                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
                subkey.SetValue("SaveOptionPath", txt_SelectedPath.Text + txt_ImageName.Text);

                if (chk_SaveRecipe.Checked)
                {
                    ExportKey(txt_SelectedPath.Text + @"\SVG.reg", @"HKEY_LOCAL_MACHINE\SOFTWARE\SVG");
                    if (cbo_RecipeSaveMode.SelectedIndex == 0)
                    {
                        CopyOptionFile(AppDomain.CurrentDomain.BaseDirectory, txt_SelectedPath.Text);
                        CopyAllFiles(m_strRecipePath, txt_SelectedPath.Text + "\\" + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\");

                        if (Directory.Exists(txt_SelectedPath.Text))
                        {
                            DirectoryInfo dir = new DirectoryInfo(m_smProductionInfo.g_strRecipePath);
                            FileInfo[] files = dir.GetFiles();
                            foreach (FileInfo file in files)
                            {
                                if (file.Name.Contains("Vision") && file.Name.Contains("Calibration.xml"))
                                {
                                    string temppath = Path.Combine(txt_SelectedPath.Text, file.Name);
                                    file.CopyTo(temppath, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        CopyOptionFile(AppDomain.CurrentDomain.BaseDirectory, txt_SelectedPath.Text);
                        CopyAllFiles(m_strRecipePath + "Vision" + (m_smVisionInfo.g_intVisionIndex + 1).ToString() + "\\",
                            txt_SelectedPath.Text + "\\" + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\Vision" + (m_smVisionInfo.g_intVisionIndex + 1).ToString() + "\\");

                        if (Directory.Exists(txt_SelectedPath.Text))
                        {
                            DirectoryInfo dir = new DirectoryInfo(m_smProductionInfo.g_strRecipePath);
                            FileInfo[] files = dir.GetFiles();
                            foreach (FileInfo file in files)
                            {
                                if (file.Name == "Vision" + (m_smVisionInfo.g_intVisionIndex + 1).ToString() + "Calibration.xml")
                                {
                                    string temppath = Path.Combine(txt_SelectedPath.Text, file.Name);
                                    file.CopyTo(temppath, true);
                                }
                            }
                        }
                    }
                    // 2019 09 04 - CCENG: Add save date and software version for easier tracking.
                    File.WriteAllText(txt_SelectedPath.Text + @"\ReadMe.txt", "Save Date : " + DateTime.Now.ToString());
                    File.AppendAllText(txt_SelectedPath.Text + @"\ReadMe.txt", "\n");
                    File.AppendAllText(txt_SelectedPath.Text + @"\ReadMe.txt", "Vision Software Version : " + Application.ProductVersion);
                }

                //2020-03-30 ZJYEOH : Save Option will always save current image
                if (true)//(chk_SaveCurrentImage.Checked)
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorImages[0].SaveImage(txt_SelectedPath.Text + m_strImageFileName + ".bmp");// "\\CurrentImage.bmp");
                    else
                        m_smVisionInfo.g_arrImages[0].SaveImage(txt_SelectedPath.Text + m_strImageFileName + ".bmp");// "\\CurrentImage.bmp");


                    for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
                    {
                        if (!WantSaveImageAccordingMergeType(i))
                            continue;
                        
                        if (m_smVisionInfo.g_blnViewColorImage)
                            m_smVisionInfo.g_arrColorImages[i].SaveImage(txt_SelectedPath.Text + m_strImageFileName + "_Image" + i.ToString() + ".bmp");// "\\CurrentImage_Image" + i.ToString() + ".bmp");
                        else
                            m_smVisionInfo.g_arrImages[i].SaveImage(txt_SelectedPath.Text + m_strImageFileName + "_Image" + i.ToString() + ".bmp"); // "\\CurrentImage_Image" + i.ToString() + ".bmp");
                    }
                }

                if (chk_SaveLogFile.Checked)
                {
                    try
                    {
                        DirectoryInfo dir = new DirectoryInfo("C:\\LogFile\\TrackLog\\");
                        DirectoryInfo[] dirs = dir.GetDirectories();
                        int LatestDate = 0;
                        int LatestMonth = 0;
                        int LatestYear = 0;
                        foreach (DirectoryInfo dirs1 in dirs)
                        {
                            string[] Split = dirs1.Name.Split('-');
                            int Date = Convert.ToInt32(Split[2]);
                            int Month = Convert.ToInt32(Split[1]);
                            int Year = Convert.ToInt32(Split[0]);

                            if (LatestYear <= Year && LatestMonth <= Month && LatestDate <= Date)
                            {
                                LatestDate = Date;
                                LatestMonth = Month;
                                LatestYear = Year;
                            }

                        }

                        CopyAllFiles(@"C:\LogFile\TrackLog\" + LatestYear.ToString() + "-" + LatestMonth.ToString().PadLeft(2, '0') + "-" + LatestDate.ToString().PadLeft(2, '0') + "\\", txt_SelectedPath.Text + "\\" + LatestYear.ToString() + "-" + LatestMonth.ToString().PadLeft(2, '0') + "-" + LatestDate.ToString().PadLeft(2, '0') + "\\");
                    }
                    catch { }
                }

                if (chk_SaveEditLogFile.Checked)
                {
                    try
                    {

                        DirectoryInfo dir = new DirectoryInfo(m_smProductionInfo.g_strHistoryDataLocation + "Data\\");
                        DirectoryInfo[] dirs = dir.GetDirectories();
                        List<string> arrFolderName = new List<string>();
                        //int LatestMonth = 0;
                        //int LatestYear = 0;
                        foreach (DirectoryInfo dirs1 in dirs)
                        {
                            //string[] Split = dirs1.Name.Split('-');
                            //int Month = Convert.ToInt32(Split[1]);
                            //int Year = Convert.ToInt32(Split[0]);

                            //if (LatestYear <= Year && LatestMonth <= Month)
                            //{
                            //    LatestMonth = Month;
                            //    LatestYear = Year;
                            //}
                            arrFolderName.Add(dirs1.Name);
                        }
                        arrFolderName.Sort();
                        if (arrFolderName.Count > 1)
                        {
                            string[] Split = arrFolderName[arrFolderName.Count - 2].Split('-');
                            int Month = Convert.ToInt32(Split[1]);
                            int Year = Convert.ToInt32(Split[0]);
                            CopyAllFiles(@"C:\SVG\Data\" + Year.ToString() + "-" + Month.ToString().PadLeft(2, '0') + "\\", txt_SelectedPath.Text + "\\" + Year.ToString() + "-" + Month.ToString().PadLeft(2, '0') + "\\");
                        }
                        if (arrFolderName.Count > 0)
                        {
                            string[] Split2 = arrFolderName[arrFolderName.Count - 1].Split('-');
                            int Month2 = Convert.ToInt32(Split2[1]);
                            int Year2 = Convert.ToInt32(Split2[0]);
                            CopyAllFiles(@"C:\SVG\Data\" + Year2.ToString() + "-" + Month2.ToString().PadLeft(2, '0') + "\\", txt_SelectedPath.Text + "\\" + Year2.ToString() + "-" + Month2.ToString().PadLeft(2, '0') + "\\");
                        }
                    }
                    catch { }
                }
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("Save Option Return Exception = " + ex.ToString());
            }
        }

        private void chk_SaveRecipe_CheckedChanged(object sender, EventArgs e)
        {
            cbo_RecipeSaveMode.Enabled = chk_SaveRecipe.Checked;
        }

        private static bool CopyOptionFile(string Source, string Destination)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Source);
                if (!dir.Exists)
                    return false;

                FileInfo[] files = dir.GetFiles("*.xml");

                foreach (FileInfo f in files)
                {
                    if (f.FullName.Contains("Option"))
                    {
                        string temppath = Path.Combine(Destination, f.Name);
                        f.CopyTo(temppath, true);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

    }
}
