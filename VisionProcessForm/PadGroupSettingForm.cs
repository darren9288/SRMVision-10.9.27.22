using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharedMemory;
using VisionProcessing;
using Common;

namespace VisionProcessForm
{
    public partial class PadGroupSettingForm: Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private string m_strSelectedRecipe;

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;

        public bool ref_blnWantUseGroupSetting { get { return radioBtn_UseGroupSetting.Checked; } }
       
        #endregion

        public PadGroupSettingForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, string strSelectedRecipe, int intUserGroup)
        {
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = smVisionInfo;
            m_intUserGroup = intUserGroup;

            InitializeComponent();

            UpdateGUI();
            m_blnInitDone = true;
        }

             
        
        private void UpdateGUI()
        {
            radioBtn_UseGroupSetting.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantUseGroupToleranceSetting;
        }

        private void btn_SaveAndClose_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].ref_blnWantUseGroupToleranceSetting = radioBtn_UseGroupSetting.Checked;
            }
        }
    }
}