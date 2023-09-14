using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using SharedMemory;
using VisionProcessing;
using System.Threading;
using System.IO;

namespace VisionProcessForm
{
    public partial class EnchanceImageSettingForm : Form
    {
        private int m_intUserGroup = 5;
        private int m_intSelectedImage = 0;
        private bool m_blnInitDone = false;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private string m_strSelectedRecipe = "";
        private string m_strFilePath;
        private int m_intMergeType;
        private string m_strCameraFilePath;

        public EnchanceImageSettingForm(VisionInfo smVisionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo, int intSelectedImage)
        {
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            if (m_smVisionInfo.g_blnGlobalSharingCameraData)
                m_strFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\GlobalCamera.xml";
            else
                m_strFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe +
                 "\\Camera.xml";
            // m_strPHPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\PHCamera.xml";
            m_strCameraFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\";
            m_intMergeType = m_smVisionInfo.g_intImageMergeType;
            InitializeComponent();
            m_intSelectedImage = intSelectedImage; 

            UpdateGUI();
            
            m_blnInitDone = true;
        }
        
        private void UpdateGUI()
        {
            chk_EnableContrast.Checked = m_smVisionInfo.g_blnEnhanceImage_Enable;
            txt_Close.Value = m_smVisionInfo.g_intEnhanceImage_Close;
            txt_Open.Value = m_smVisionInfo.g_intEnhanceImage_Open;
            txt_Dilate.Value = m_smVisionInfo.g_intEnhanceImage_Dilate;
            txt_Offset.Value = m_smVisionInfo.g_intEnhanceImage_Offset;
            txt_Gain.Value = Convert.ToDecimal(m_smVisionInfo.g_fEnhanceImage_Gain);

        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            XmlParser objFile = new XmlParser(m_strFilePath);
            objFile.WriteSectionElement(m_smVisionInfo.g_strVisionFolderName);
            objFile.WriteElement1Value("EnhanceImage", "");
            objFile.WriteElement2Value("EnhanceImage_Enable", m_smVisionInfo.g_blnEnhanceImage_Enable);
            objFile.WriteElement2Value("EnhanceImage_Close", m_smVisionInfo.g_intEnhanceImage_Close.ToString());
            objFile.WriteElement2Value("EnhanceImage_Open", m_smVisionInfo.g_intEnhanceImage_Open.ToString());
            objFile.WriteElement2Value("EnhanceImage_Dilate", m_smVisionInfo.g_intEnhanceImage_Dilate.ToString());
            objFile.WriteElement2Value("EnhanceImage_Offset", m_smVisionInfo.g_intEnhanceImage_Offset.ToString());
            objFile.WriteElement2Value("EnhanceImage_Gain", m_smVisionInfo.g_fEnhanceImage_Gain.ToString());
            objFile.WriteEndElement();

#if (RELEASE || Release_2_12 || RTXRelease)
            //2020-12-10 ZJYEOH : Overwrite global xml and other recipe local-global xml
            if (m_smVisionInfo.g_blnGlobalSharingCameraData)
                {
                    File.Copy(m_strCameraFilePath + "GlobalCamera.xml", AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\GlobalCamera.xml", true);

                    string[] arrDirectory = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\");
                    for (int j = 0; j < arrDirectory.Length; j++)
                    {
                        if (m_strCameraFilePath == (arrDirectory[j] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\"))
                            continue;

                        if (Directory.Exists(arrDirectory[j] + "\\"))
                        {
                            File.Copy(m_strCameraFilePath + "GlobalCamera.xml", arrDirectory[j] + "GlobalCamera.xml", true);
                        }
                    }
                }
#endif
            Close();
            Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            // Load EnhanceImage Setting
            XmlParser objFile = new XmlParser(m_strFilePath);
            objFile.GetFirstSection(m_smVisionInfo.g_strVisionFolderName);
            objFile.GetSecondSection("EnhanceImage");
            bool blnEnable = objFile.GetValueAsBoolean("EnhanceImage_Enable", false, 2);
            m_smVisionInfo.g_blnEnhanceImage_Enable = objFile.GetValueAsBoolean("EnhanceImage_Enable", false, 2);
            m_smVisionInfo.g_intEnhanceImage_Close = objFile.GetValueAsInt("EnhanceImage_Close", 0, 2);
            m_smVisionInfo.g_intEnhanceImage_Open = objFile.GetValueAsInt("EnhanceImage_Open", 1, 2);
            m_smVisionInfo.g_intEnhanceImage_Dilate = objFile.GetValueAsInt("EnhanceImage_Dilate", 1, 2);
            m_smVisionInfo.g_intEnhanceImage_Offset = objFile.GetValueAsInt("EnhanceImage_Offset", -50, 2);
            m_smVisionInfo.g_fEnhanceImage_Gain = objFile.GetValueAsFloat("EnhanceImage_Gain", 2, 2);
            Close();
            Dispose();
        }

        private void txt_ExtraGain_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fValue = Convert.ToSingle(((NumericUpDown)sender).Value);

            m_smVisionInfo.g_arrImageExtraGain[m_intSelectedImage][Convert.ToInt32(((NumericUpDown)sender).Tag)] = fValue;
            
        }

        private void chk_EnableContrast_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnEnhanceImage_Enable = chk_EnableContrast.Checked;
        }

        private void txt_Open_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intEnhanceImage_Open = Convert.ToInt32(txt_Open.Value);
        }

        private void txt_Dilate_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intEnhanceImage_Dilate = Convert.ToInt32(txt_Dilate.Value);
        }

        private void txt_Offset_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intEnhanceImage_Offset = Convert.ToInt32(txt_Offset.Value);
        }

        private void txt_Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_fEnhanceImage_Gain = Convert.ToSingle(txt_Gain.Value);

        }

        private void txt_Close_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intEnhanceImage_Close = Convert.ToInt32(txt_Close.Value);
        }

    }
}
