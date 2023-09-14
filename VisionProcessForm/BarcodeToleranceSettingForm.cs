using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using ImageAcquisition;
using VisionProcessing;
using SharedMemory;
using Microsoft.Win32;
using System.IO;

namespace VisionProcessForm
{
    public partial class BarcodeToleranceSettingForm : Form
    {
        #region Member Variables

        private int m_intUserGroup = 5;
        private bool m_blnInitDone = false;

        private string m_strSelectedRecipe;

        private VisionInfo m_smVisionInfo;
        private UserRight m_objUserRight = new UserRight();
        private bool m_blnEnterTextBox = false;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        #endregion

        public BarcodeToleranceSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductioninfo)
        {
            InitializeComponent();
            m_smProductionInfo = smProductioninfo;
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            

            DisableField2();
            UpdateGUI();

            m_blnInitDone = true;
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            //NewUserRight objUserRight = new NewUserRight(false);
            string strChild1 = "Tolerance";
            string strChild2 = "2D Code TabPage";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBarcodeChild2Group(strChild1, strChild2))
            {
                gb_PatternScoreSetting.Enabled = false;
                gb_PatternSetting.Enabled = false;
            }

            strChild2 = "Pattern TabPage";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBarcodeChild2Group(strChild1, strChild2))
            {
                gb_Barcode.Enabled = false;
            }
        }
        private void UpdateGUI()
        {
            txt_GainRange.Value = Convert.ToDecimal(m_smVisionInfo.g_objBarcode.ref_fGainRangeTolerance);

            txt_BarcodeAngleRange.Value = Convert.ToDecimal(m_smVisionInfo.g_objBarcode.ref_fBarcodeAngleRangeTolerance);

            txt_PatternAngleRange.Value = Convert.ToDecimal(m_smVisionInfo.g_objBarcode.ref_intPatternAngleRangeTolerance);

            trackBar_ScoreTolerance.Value = Convert.ToInt32(m_smVisionInfo.g_objBarcode.ref_intMinMatchingScore);
            txt_ScoreTolerance.Text = trackBar_ScoreTolerance.Value.ToString();
            
            lbl_Score.Text = (m_smVisionInfo.g_objBarcode.GetMatchingScore()).ToString("F2");


            if (m_smVisionInfo.g_objBarcode.ref_intCodeType == 0)
                gb_Barcode.Size = new Size(gb_Barcode.Width, 130);
            else
                gb_Barcode.Size = new Size(gb_Barcode.Width, 70);
        }
        private void txt_GainRange_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objBarcode.ref_fGainRangeTolerance = (float)Convert.ToDecimal(txt_GainRange.Value);
        }

        private void txt_BarcodeAngleRange_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objBarcode.ref_fBarcodeAngleRangeTolerance = (float)Convert.ToDecimal(txt_BarcodeAngleRange.Value);
        }

        private void txt_ScoreTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            trackBar_ScoreTolerance.Value = Convert.ToInt32(txt_ScoreTolerance.Text);
            m_smVisionInfo.g_objBarcode.ref_intMinMatchingScore = Convert.ToInt32(trackBar_ScoreTolerance.Value);
        }

        private void trackBar_ScoreTolerance_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_ScoreTolerance.Text = trackBar_ScoreTolerance.Value.ToString();
            m_smVisionInfo.g_objBarcode.ref_intMinMatchingScore = Convert.ToInt32(trackBar_ScoreTolerance.Value);
        }

        private void txt_PatternAngleRange_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objBarcode.ref_intPatternAngleRangeTolerance = Convert.ToInt32(txt_PatternAngleRange.Value);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_smProductionInfo.AT_ALL_InAuto && m_smProductionInfo.g_intAutoLogOutMinutes > 0 && m_intUserGroup != 5)
            {
                DateTime t = DateTime.Now;
                TimeSpan tSpan = t - m_smProductionInfo.g_DTStartAutoMode_IndividualForm;

                if (tSpan.Minutes >= m_smProductionInfo.g_intAutoLogOutMinutes)
                {
                    m_smProductionInfo.g_intUserGroup = 5;
                    m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
                    DisableField2();
                }
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
              m_smVisionInfo.g_strVisionFolderName + "\\";
            
            STDeviceEdit.CopySettingFile(strPath, "Barcode\\Settings.xml");
            m_smVisionInfo.g_objBarcode.SaveBarcode(strPath + "Barcode\\Settings.xml", false, "Settings", false);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Barcode Tolerance Setting", m_smProductionInfo.g_strLotID);
            
            Close();
            Dispose();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
             m_smVisionInfo.g_strVisionFolderName + "\\";

            m_smVisionInfo.g_objBarcode.LoadBarcode(strPath + "Barcode\\Settings.xml", "Settings");

            Close();
            Dispose();
        }

        private void BarcodeToleranceSettingForm_Load(object sender, EventArgs e)
        {
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
        }

        private void BarcodeToleranceSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Barcode Tolerance Setting Form Closed", "Exit Barcode Tolerance Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_blnViewROI = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }
    }
}
