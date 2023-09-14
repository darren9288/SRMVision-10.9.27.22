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
    public partial class ExtraGainSettingForm : Form
    {
        private int m_intUserGroup = 5;
        private int m_intSelectedImage = 0;
        private bool m_blnInitDone = false;
        private List<List<float>> m_arrImageExtraGainPrev = new List<List<float>>();
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private string m_strSelectedRecipe = "";
        private string m_strFilePath;
        private int m_intMergeType;
        private string m_strCameraFilePath;

        public ExtraGainSettingForm(VisionInfo smVisionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo, int intSelectedImage)
        {
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = smVisionInfo;
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

            for (int i = 0; i < m_smVisionInfo.g_arrImageExtraGain.Count; i++)
            {
                m_arrImageExtraGainPrev.Add(new List<float>(5) {0,0,0,0,0});
                for (int j = 0; j < m_smVisionInfo.g_arrImageExtraGain[i].Count; j++)
                {
                    m_arrImageExtraGainPrev[i][j] = m_smVisionInfo.g_arrImageExtraGain[i][j];
                }
            }
            
            for (int j = 0; j < m_arrImageExtraGainPrev[m_intSelectedImage].Count; j++)
            {
                switch (j)
                {
                    case 0:
                        txt_ExtraGain1.Value = Convert.ToDecimal(m_arrImageExtraGainPrev[m_intSelectedImage][j]);
                        break;
                    case 1:
                        txt_ExtraGain2.Value = Convert.ToDecimal(m_arrImageExtraGainPrev[m_intSelectedImage][j]);
                        break;
                    case 2:
                        txt_ExtraGain3.Value = Convert.ToDecimal(m_arrImageExtraGainPrev[m_intSelectedImage][j]);
                        break;
                    case 3:
                        txt_ExtraGain4.Value = Convert.ToDecimal(m_arrImageExtraGainPrev[m_intSelectedImage][j]);
                        break;
                    case 4:
                        txt_ExtraGain5.Value = Convert.ToDecimal(m_arrImageExtraGainPrev[m_intSelectedImage][j]);
                        break;
                }
            }

            switch (m_intMergeType)
            {
                default:
                case 0: // No merge
                    {
                        pnl_ExtraGain.Visible = false;
                    }
                    break;
                case 1: // Merge grab 1 center and grab 2 side 
                    {
                        if (m_intSelectedImage == 0)
                        {
                            pnl_ExtraGain2.Visible = false;
                            pnl_ExtraGain3.Visible = false;
                            pnl_ExtraGain4.Visible = false;
                            pnl_ExtraGain5.Visible = false;
                        }
                        else
                            pnl_ExtraGain1.Visible = false;
                    }
                    break;
                case 2: // Merge grab 1 center, grab 2 top left and grab 3 bottom right.
                    {
                        if (m_intSelectedImage == 0)
                        {
                            pnl_ExtraGain2.Visible = false;
                            pnl_ExtraGain3.Visible = false;
                            pnl_ExtraGain4.Visible = false;
                            pnl_ExtraGain5.Visible = false;
                        }
                        else if (m_intSelectedImage == 1)
                        {
                            pnl_ExtraGain1.Visible = false;
                            pnl_ExtraGain4.Visible = false;
                            pnl_ExtraGain5.Visible = false;
                        }
                        else if (m_intSelectedImage == 2)
                        {
                            pnl_ExtraGain1.Visible = false;
                            pnl_ExtraGain2.Visible = false;
                            pnl_ExtraGain3.Visible = false;
                        }
                    }
                    break;
                case 3: // Merge grab 1 center and grab 2 side and Merge grab 3 center and grab 4 side 
                    {
                        if (m_intSelectedImage == 0 || m_intSelectedImage == 2)
                        {
                            pnl_ExtraGain2.Visible = false;
                            pnl_ExtraGain3.Visible = false;
                            pnl_ExtraGain4.Visible = false;
                            pnl_ExtraGain5.Visible = false;
                        }
                        else if(m_intSelectedImage == 1 || m_intSelectedImage == 3)
                            pnl_ExtraGain1.Visible = false;
                    }
                    break;
                case 4: // Merge grab 1 center, grab 2 top left and grab 3 bottom right. and Merge grab 4 center and grab 5 side 
                    {
                        if (m_intSelectedImage == 0)
                        {
                            pnl_ExtraGain2.Visible = false;
                            pnl_ExtraGain3.Visible = false;
                            pnl_ExtraGain4.Visible = false;
                            pnl_ExtraGain5.Visible = false;
                        }
                        else if (m_intSelectedImage == 1)
                        {
                            pnl_ExtraGain1.Visible = false;
                            pnl_ExtraGain4.Visible = false;
                            pnl_ExtraGain5.Visible = false;
                        }
                        else if (m_intSelectedImage == 2)
                        {
                            pnl_ExtraGain1.Visible = false;
                            pnl_ExtraGain2.Visible = false;
                            pnl_ExtraGain3.Visible = false;
                        }
                        else if (m_intSelectedImage == 3)
                        {
                            pnl_ExtraGain2.Visible = false;
                            pnl_ExtraGain3.Visible = false;
                            pnl_ExtraGain4.Visible = false;
                            pnl_ExtraGain5.Visible = false;
                        }
                        else if (m_intSelectedImage == 4)
                            pnl_ExtraGain1.Visible = false;
                    }
                    break;
            }

        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            XmlParser objFile = new XmlParser(m_strFilePath);
            objFile.WriteSectionElement(m_smVisionInfo.g_strVisionFolderName);
            for (int i = 0; i < m_smVisionInfo.g_arrImageExtraGain.Count; i++)
            {
                objFile.WriteElement1Value("ImageExtraGain" + i.ToString(), "");
                for (int j = 0; j < m_smVisionInfo.g_arrImageExtraGain[i].Count; j++)
                {
                    objFile.WriteElement2Value("ImageExtraGain" + i.ToString() + "_" + j.ToString(), m_smVisionInfo.g_arrImageExtraGain[i][j]);
                }
            }
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
            m_smVisionInfo.g_arrImageExtraGain = m_arrImageExtraGainPrev;

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
    }
}
