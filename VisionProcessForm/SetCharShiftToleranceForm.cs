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
using SharedMemory;

namespace VisionProcessForm
{
    public partial class SetCharShiftToleranceForm : Form
    {
        public bool ref_blnSetToAll { get { return chk_SetToAll.Checked; } }
        public int ref_intCharShiftedX { get { return Convert.ToInt32(txt_SetXValue.Text); } }
        public int ref_intCharShiftedY { get { return Convert.ToInt32(txt_SetYValue.Text); } }

        private CustomOption m_smCustomizeInfo;


        public SetCharShiftToleranceForm(CustomOption smCustomizeInfo, string strCharShiftedX, string strCharShiftedY)
        {
            InitializeComponent();

            m_smCustomizeInfo = smCustomizeInfo;

            txt_SetXValue.Text = strCharShiftedX;
            txt_SetYValue.Text = strCharShiftedY;

            switch (smCustomizeInfo.g_intMarkUnitDisplay)
            {
                case 0:
                    txt_SetXValue.DecimalPlaces = 0;
                    txt_SetYValue.DecimalPlaces = 0;
                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    {
                        srmLabel2.Text = "像素";
                        srmLabel3.Text = "像素";
                    }
                    else
                    {
                        srmLabel2.Text = "pix";
                        srmLabel3.Text = "pix";
                    }
                    break;
                case 1:
                    txt_SetXValue.DecimalPlaces = 4;
                    txt_SetYValue.DecimalPlaces = 4;
                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    {
                        srmLabel2.Text = "毫米";
                        srmLabel3.Text = "毫米";
                    }
                    else
                    {
                        srmLabel2.Text = "mm";
                        srmLabel3.Text = "mm";
                    }
                    break;
                case 2:
                    txt_SetXValue.DecimalPlaces = 3;
                    txt_SetYValue.DecimalPlaces = 3;
                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    {
                        srmLabel2.Text = "mil";
                        srmLabel3.Text = "mil";
                    }
                    else
                    {
                        srmLabel2.Text = "mil";
                        srmLabel3.Text = "mil";
                    }
                    break;
                case 3:
                    txt_SetXValue.DecimalPlaces = 1;
                    txt_SetYValue.DecimalPlaces = 1;
                    if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    {
                        srmLabel2.Text = "微米";
                        srmLabel3.Text = "微米";
                    }
                    else
                    {
                        srmLabel2.Text = "micron";
                        srmLabel3.Text = "micron";
                    }
                    break;
            }

        }

        public float GetCharShiftedX()
        {
            if (txt_SetXValue.Text == "")
                return -1;
            else
                return (float)Convert.ToDouble(txt_SetXValue.Text);
        }

        public float GetCharShiftedY()
        {
            if (txt_SetYValue.Text == "")
                return -1;
            else
                return (float)Convert.ToDouble(txt_SetYValue.Text);
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            float fSetXValue = 0;
            if (!(txt_SetXValue.Text == "" || float.TryParse(txt_SetXValue.Text, out fSetXValue)))
            {
                SRMMessageBox.Show("X value is not correct.");
                return;
            }

            float fSetYValue = 0;
            if (!(txt_SetYValue.Text == "" || float.TryParse(txt_SetYValue.Text, out fSetYValue)))
            {
                SRMMessageBox.Show("Y value is not correct.");
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}
