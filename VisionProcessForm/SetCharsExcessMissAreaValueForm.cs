using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VisionProcessing;
using SharedMemory;

namespace VisionProcessForm
{
    public partial class SetCharsExcessMissAreaValueForm : Form
    {
        #region Properties
        public float ref_fSetValue { get { return (float)Convert.ToDouble(txt_SetValue.Text); } }

        public bool ref_blnSetAllRows { get { return chk_SetAllRows.Checked; } }

        private CustomOption m_smCustomizeInfo;

        #endregion

        public SetCharsExcessMissAreaValueForm(CustomOption smCustomizeInfo, string strSettingType, int intRowIndex, float intCurrentSetValue)
        {
            InitializeComponent();

            m_smCustomizeInfo = smCustomizeInfo;

            if (intRowIndex == -1)
            {
                chk_SetAllRows.Checked = true;
                chk_SetAllRows.Enabled = false;
            }
            else
            {
                chk_SetAllRows.Checked = false;
                chk_SetAllRows.Enabled = true;
            }

            switch (strSettingType)
            {
                case "Max Excess Area":
                    {
                        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                            lbl_Definition.Text = "设置字模的外延面积最大容许像素";
                        else
                            lbl_Definition.Text = "Set value to Mark's Max Excess Area";
                    }
                    break;
                case "Max Miss Area":
                    {
                        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                            lbl_Definition.Text = "设置字模的破损面积最大容许像素";
                        else
                            lbl_Definition.Text = "Set value to Mark's Max Broken Area";
                    }
                    break;
            }

            switch (smCustomizeInfo.g_intMarkUnitDisplay)
            {
                case 0:
                    txt_SetValue.DecimalPlaces = 0;
                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                        srmLabel1.Text = "像素";
                    else
                        srmLabel1.Text = "pix";
                    break;
                case 1:
                    txt_SetValue.DecimalPlaces = 4;
                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                        srmLabel1.Text = "毫米";
                    else
                        srmLabel1.Text = "mm";
                    break;
                case 2:
                    txt_SetValue.DecimalPlaces = 3;
                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                        srmLabel1.Text = "mil";
                    else
                        srmLabel1.Text = "mil";
                    break;
                case 3:
                    txt_SetValue.DecimalPlaces = 1;
                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                        srmLabel1.Text = "微米";
                    else
                        srmLabel1.Text = "micron";
                    break;
            }
            
            txt_SetValue.Text = intCurrentSetValue.ToString();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }
    }
}