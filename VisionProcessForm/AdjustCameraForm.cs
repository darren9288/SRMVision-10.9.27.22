using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using ImageAcquisition;
using SharedMemory;

namespace VisionProcessForm
{
    public partial class AdjustCameraForm : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;
        private string m_strFilePath;
        private string m_strCameraFilePath;
        private string m_strVisionName;

        private AVTVimba m_objAVTFireGrab;
        private uint m_intGammaPrev;
        private uint m_intUBPrev;
        private uint m_intURPrev;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        #endregion

        public AdjustCameraForm(AVTVimba objAVTFireGrab, string strSelectedRecipe, string strVisionName, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            m_smProductionInfo = smProductionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_objAVTFireGrab = objAVTFireGrab;
            m_strFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe +
               "\\Camera.xml";
            m_strCameraFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe;
            m_strVisionName = strVisionName; 

            InitializeComponent();

            UpdateGUI();

            m_blnInitDone = true;
        }





        private void SaveSettings()
        {
            
            STDeviceEdit.CopySettingFile(m_strCameraFilePath, "\\Camera.xml");

            XmlParser objFile = new XmlParser(m_strFilePath);

            objFile.WriteSectionElement(m_strVisionName);

            objFile.WriteElement1Value("Gamma", chk_GammaOnOff.Checked);
            objFile.WriteElement1Value("AutoWB", chk_Auto.Checked);
            objFile.WriteElement1Value("UBValue", trackBar_UB.Value);
            objFile.WriteElement1Value("VRValue", trackBar_VR.Value);
  
            objFile.WriteEndElement();

            STDeviceEdit.XMLChangesTracing(m_strVisionName, m_smProductionInfo.g_strLotID);
            
            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;
        }

        private void UpdateGUI()
        {
            XmlParser objFile = new XmlParser(m_strFilePath);
            objFile.GetFirstSection(m_strVisionName);

            chk_GammaOnOff.Checked = objFile.GetValueAsBoolean("Gamma", false);
            if (chk_GammaOnOff.Checked)
            {
                chk_GammaOnOff.Text = "ON";
                if (m_objAVTFireGrab.GetCameraParameter(4) == 1)
                    if (!m_objAVTFireGrab.SetCameraParameter(4, 1))
                        SRMMessageBox.Show("Fail to set gamma to camera");
            }
            else
            {
                chk_GammaOnOff.Text = "OFF";
                if (m_objAVTFireGrab.GetCameraParameter(4) == 0)
                    if (!m_objAVTFireGrab.SetCameraParameter(4, 0))
                        SRMMessageBox.Show("Fail to set gamma to camera");
            }

            m_intUBPrev = (uint)objFile.GetValueAsInt("UBValue", 0);
            m_intURPrev = (uint)objFile.GetValueAsInt("VRValue", 0);

            if (m_intUBPrev > trackBar_UB.Maximum || m_intUBPrev < trackBar_UB.Minimum)
                m_intUBPrev = 0;

            if (m_intURPrev > trackBar_VR.Maximum || m_intURPrev < trackBar_VR.Minimum)
                m_intURPrev = 0;

            trackBar_UB.Value = (int)m_intUBPrev;
            trackBar_VR.Value = (int)m_intURPrev;

            txt_UB.Text = m_intUBPrev.ToString();
            txt_VR.Text = m_intURPrev.ToString();

            if (!m_objAVTFireGrab.SetCameraParameter(5, (uint)trackBar_UB.Value))
                SRMMessageBox.Show("Fail to set UB to camera");

            if (!m_objAVTFireGrab.SetCameraParameter(6, (uint)trackBar_VR.Value))
                SRMMessageBox.Show("Fail to set VR to camera");

        }

        private void UpdateWhiteBalanceValue()
        {
            uint intValueUB = 0;
            uint intValueVR = 0;
            m_objAVTFireGrab.GetCameraWBParameter(ref intValueUB, ref intValueVR);
            if (intValueUB != trackBar_UB.Value)
            {
                if (intValueUB <= trackBar_UB.Maximum && intValueUB >= trackBar_UB.Minimum)
                    txt_UB.Text = intValueUB.ToString();
            }
            if (intValueVR != trackBar_VR.Value)
            {
                if (intValueVR <= trackBar_VR.Maximum && intValueVR >= trackBar_VR.Minimum)
                    txt_VR.Text = intValueVR.ToString();
            }
        }





        private void txt_UB_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_UB.Text == "")
                return;

            long intValue = Convert.ToInt64(txt_UB.Text);

            if (intValue > trackBar_UB.Maximum || intValue < trackBar_UB.Minimum)
                return;

            trackBar_UB.Value = (int)intValue;
            m_objAVTFireGrab.SetCameraParameter(5, (uint)trackBar_UB.Value);
        }

        private void txt_VR_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || txt_VR.Text == "")
                return;
            long intValue = Convert.ToInt64(txt_VR.Text);

            if (intValue > trackBar_UB.Maximum || intValue < trackBar_UB.Minimum)
                return;

            trackBar_VR.Value = (int)intValue;
            m_objAVTFireGrab.SetCameraParameter(6, (uint)trackBar_VR.Value);
        }

        private void trackBar_UB_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_UB.Text = trackBar_UB.Value.ToString();
        }

        private void trackBar_VR_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_VR.Text = trackBar_VR.Value.ToString();
        }

        




        private void chk_Auto_Click(object sender, EventArgs e)
        {
            if (chk_Auto.Checked)
            {
                if (m_objAVTFireGrab.AutoCalibrateWhiteBalance(1)) // On Auto White Balance
                {
                    btn_OnePush.Enabled = false;
                    trackBar_UB.Enabled = false;
                    trackBar_VR.Enabled = false;
                    txt_UB.Enabled = false;
                     txt_VR.Enabled = false;
                }
                else
                    chk_Auto.Checked = false;
            }
            else
            {
                if (m_objAVTFireGrab.AutoCalibrateWhiteBalance(2))  // Off Auto White Balance
                {
                    btn_OnePush.Enabled = true;
                    trackBar_UB.Enabled = true;
                    trackBar_VR.Enabled = true;
                    txt_UB.Enabled = true;
                    txt_VR.Enabled = true;
                }
                else
                    chk_Auto.Checked = true;
            }
        }

       
        private void chk_GammaOnOff_Click(object sender, EventArgs e)
        {
            if (chk_GammaOnOff.Checked)
            {
                if (!m_objAVTFireGrab.SetCameraParameter(4, 1))
                {
                    SRMMessageBox.Show("Fail to set gamma ON");
                }
            }
            else
            {
                if (!m_objAVTFireGrab.SetCameraParameter(4, 0))
                {
                    SRMMessageBox.Show("Fail to set gamma OFF");
                }
            }
        }

        private void btn_OnePush_Click(object sender, EventArgs e)
        {
            m_objAVTFireGrab.AutoCalibrateWhiteBalance(2);

            UpdateWhiteBalanceValue();
        }

     

      



        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if (chk_Auto.Checked)
                m_objAVTFireGrab.AutoCalibrateWhiteBalance(2);

            if (!m_objAVTFireGrab.SetCameraParameter(4, m_intGammaPrev))
            {
                SRMMessageBox.Show("Fail to reset camera gamma value");
            }

            if (!m_objAVTFireGrab.SetCameraParameter(5, m_intUBPrev) || !m_objAVTFireGrab.SetCameraParameter(6, m_intURPrev) )
            {
                SRMMessageBox.Show("Fail to reset camera white balance value");
            }

            Close();
            Dispose();
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (chk_Auto.Checked)
                m_objAVTFireGrab.AutoCalibrateWhiteBalance(2);

            SaveSettings();

            Close();
            Dispose();
        }




        private void timer1_Tick(object sender, EventArgs e)
        {
            if (chk_Auto.Checked)
            {
                UpdateWhiteBalanceValue();
            }
        } 

      
    }
}