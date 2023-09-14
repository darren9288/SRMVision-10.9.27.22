using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using SharedMemory;
using System.IO;

namespace VisionProcessForm
{
    public partial class AdvancedCalibrationForm : Form
    {
        #region Member Variables
        private string m_strPath = "";
        private string m_strFolderPath = "";
        private ProductionInfo m_smProductionInfo;
        private VisionInfo m_smVisionInfo;
        #endregion

        public AdvancedCalibrationForm(string strVisionName, string strSelectedRecipe, ProductionInfo smProductionInfo, VisionInfo smVisionInfo)
        {
            InitializeComponent();
            m_smVisionInfo = smVisionInfo;
            m_smProductionInfo = smProductionInfo;

            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
            {
                m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" + strVisionName + "\\";
                m_strPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" + strVisionName + "\\" + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml";
            }
            else
            {
                m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" + strVisionName + "\\";
                m_strPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" + strVisionName + "\\Calibration.xml";
            }
            UpdateGUI();
        }



        /// <summary>
        /// Load advanced calibration settings from xml to the control
        /// </summary>
        private void UpdateGUI()
        {
            XmlParser objFileHandle = new XmlParser(m_strPath);
            objFileHandle.GetFirstSection("Advanced");
            int intSpec = objFileHandle.GetValueAsInt("Mode", 2);

            Control checkBox = chk_Inverse;
            for (int i = 0; i < 6; i++)
            {
                if ((intSpec & (1 << i)) > 0)
                    ((SRMControl.SRMCheckBox) checkBox).Checked = true;
                checkBox = GetNextControl(checkBox, true);
            }
        }



        private void btn_OK_Click(object sender, EventArgs e)
        {
            
            if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                STDeviceEdit.CopySettingFile(m_strFolderPath, m_smVisionInfo.g_strVisionFolderName + "Calibration.xml");
            else
                STDeviceEdit.CopySettingFile(m_strFolderPath, "Calibration.xml");

            XmlParser objFileHandle = new XmlParser(m_strPath);
            objFileHandle.WriteSectionElement("Advanced");

            int intSpec = 0;
            Control checkBox = chk_Inverse;
            for (int i = 0; i < 6; i++)
            {
                if (((SRMControl.SRMCheckBox)checkBox).Checked)
                    intSpec |= 0x01 << i;

                //Loop next check box control sequentially follow by its tab index
                checkBox = GetNextControl(checkBox, true);
            }
            objFileHandle.WriteElement1Value("Mode", intSpec);
            objFileHandle.WriteEndElement();

            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Advanced Calibrate", m_smProductionInfo.g_strLotID);
            
#if (RELEASE || Release_2_12 || RTXRelease)
                //2020-12-10 ZJYEOH : Overwrite global xml and other recipe local-global xml
                if (m_smVisionInfo.g_blnGlobalSharingCalibrationData)
                {
                    File.Copy(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", true);

                    string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                    for (int j = 0; j < arrDirectory.Length; j++)
                    {
                        if (m_strFolderPath == (arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\"))
                            continue;

                        if (Directory.Exists(arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\"))
                        {
                            File.Copy(m_strFolderPath + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\" + m_smVisionInfo.g_strVisionFolderName + "Calibration.xml", true);
                        }
                    }
                }
#endif

            Close();
            Dispose();
        }

    }
}