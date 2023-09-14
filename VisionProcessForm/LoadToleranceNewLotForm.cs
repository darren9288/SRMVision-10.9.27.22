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
using VisionProcessing;
using Common;
using System.IO;

namespace VisionProcessForm
{
    public partial class LoadPadToleranceNewLotForm : Form
    {
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private string m_strSelectedRecipe;
        private CustomOption m_smCustomizeInfo;

        public LoadPadToleranceNewLotForm(ProductionInfo smProductionInfo, VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe)
        {
            InitializeComponent();

            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            m_strSelectedRecipe = strSelectedRecipe;

            m_smCustomizeInfo = smCustomizeInfo;
            UpdateGUI();
        }

        private void UpdateGUI()
        {
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Pad":
                case "Pad5S":
                    chk_WantSavePad.Visible = true;
                    chk_WantSaveOthers.Visible = true;

                    chk_WantLoadReference.Checked = m_smVisionInfo.g_blnWantLoadRefTolWhenNewLot;
                    chk_WantSavePad.Checked = m_smVisionInfo.g_blnWantLoadPadRefTol;
                    chk_WantSaveOthers.Checked = m_smVisionInfo.g_blnWantLoadOtherRefTol;

                    if (m_smVisionInfo.g_strBrowsePath != "")
                    {
                        Txt_Browse.Text = Path.GetDirectoryName(m_smVisionInfo.g_strBrowsePath);

                        string[] m_arrFiles2 = Directory.GetFiles(Txt_Browse.Text, "*.stol");
                        if (m_arrFiles2.Length != 0)
                        {
                            foreach (string s in m_arrFiles2)
                            {
                                cbo_stolName.Items.Add(s.Substring(s.LastIndexOf('\\') + 1));

                                if (m_smVisionInfo.g_strBrowsePath.Contains(s.Substring(s.LastIndexOf('\\') + 1)))
                                    cbo_stolName.SelectedItem = s.Substring(s.LastIndexOf('\\') + 1);
                            }
                        }
                    }
                    else
                        Txt_Browse.Text = "";

                    break;
                case "PadPkg":
                case "Pad5SPkg":
                    chk_WantSavePad.Visible = true;
                    chk_WantSavePadPackage.Visible = true;
                    chk_WantSaveOthers.Visible = true;

                    chk_WantLoadReference.Checked = m_smVisionInfo.g_blnWantLoadRefTolWhenNewLot;
                    chk_WantSavePad.Checked = m_smVisionInfo.g_blnWantLoadPadRefTol;
                    chk_WantSavePadPackage.Checked = m_smVisionInfo.g_blnWantLoadPadPackageRefTol;
                    chk_WantSaveOthers.Checked = m_smVisionInfo.g_blnWantLoadOtherRefTol;

                    if (m_smVisionInfo.g_strBrowsePath != "")
                    {
                        Txt_Browse.Text = m_smVisionInfo.g_strBrowsePath;   //Path.GetDirectoryName(m_smVisionInfo.g_strBrowsePath);

                        string[] m_arrFiles2 = Directory.GetFiles(Txt_Browse.Text, "*.stol");
                        if (m_arrFiles2.Length != 0)
                        {
                            foreach (string s in m_arrFiles2)
                            {
                                cbo_stolName.Items.Add(s.Substring(s.LastIndexOf('\\') + 1));

                                if (m_smVisionInfo.g_strBrowsePath.Contains(s.Substring(s.LastIndexOf('\\') + 1)))
                                    cbo_stolName.SelectedItem = s.Substring(s.LastIndexOf('\\') + 1);
                            }
                        }
                    }
                    else
                        Txt_Browse.Text = "";

                    break;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (Txt_Browse.Text == "")
            {
                SRMMessageBox.Show("Path cannot be empty.");
                return;
            }

            if (!Directory.Exists(Txt_Browse.Text))
            {
                SRMMessageBox.Show("Directory is not exist.");
                return;
            }

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionName + "_Setting.stol";

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Pad":
                case "Pad5S":
                case "PadPkg":
                case "Pad5SPkg":
                    SaveSetting(strPath);

                    break;
            }

            this.Close();
            this.Dispose();
        }

        private void SaveSetting(string strPath)
        {
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Pad":
                case "Pad5S":
                case "PadPkg":
                case "Pad5SPkg":
                    m_smVisionInfo.g_blnWantLoadRefTolWhenNewLot = chk_WantLoadReference.Checked;
                    m_smVisionInfo.g_blnWantLoadPadRefTol = chk_WantSavePad.Checked;
                    m_smVisionInfo.g_blnWantLoadPadPackageRefTol = chk_WantSavePadPackage.Checked;
                    m_smVisionInfo.g_blnWantLoadOtherRefTol = chk_WantSaveOthers.Checked;

                    m_smVisionInfo.g_strBrowsePath = Txt_Browse.Text;
                    //if (cbo_stolName.Items.Count != 0 && cbo_stolName.SelectedItem != null)
                    //    m_smVisionInfo.g_strBrowsePath = Txt_Browse.Text + cbo_stolName.SelectedItem.ToString();
                    //else
                    //    m_smVisionInfo.g_strBrowsePath = "";

                   XmlParser objFile = new XmlParser(strPath, false);

                    objFile.WriteSectionElement("Settings", true);
                    objFile.WriteElement1Value("WantLoadRefTolWhenNewLot", chk_WantLoadReference.Checked);
                    objFile.WriteElement1Value("WantLoadPadRefTol", chk_WantSavePad.Checked);
                    objFile.WriteElement1Value("WantLoadPadPackageRefTol", chk_WantSavePadPackage.Checked);
                    objFile.WriteElement1Value("WantLoadOtherRefTol", chk_WantSaveOthers.Checked);
                    objFile.WriteElement1Value("BrowsePath", m_smVisionInfo.g_strBrowsePath);
                    objFile.WriteEndElement();
                    break;
            }

            this.Close();
            this.Dispose();
        }

        private void btn_Browse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string[] m_arrFiles = Directory.GetFiles(folderBrowserDialog1.SelectedPath,"*.stol");
                Txt_Browse.Text = folderBrowserDialog1.SelectedPath;
                cbo_stolName.Items.Clear();

                if(m_arrFiles.Length != 0)
                {
                    foreach (string s in m_arrFiles)
                    {
                        cbo_stolName.Items.Add(s.Substring(s.LastIndexOf('\\') + 1));
                    }
                }
            }
        }

        private void Cancel1Button_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void LoadPadToleranceNewLotForm_Load(object sender, EventArgs e)
        {

        }
    }
}
