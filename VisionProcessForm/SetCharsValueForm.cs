using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VisionProcessing;

namespace VisionProcessForm
{
    public partial class SetCharsValueForm : Form
    {
        #region Properties
        public int ref_intSetValue { get { return Convert.ToInt32(txt_SetValue.Text); } }
        #endregion

        public SetCharsValueForm(int intRowIndex, int intCurrentSetValue)
        {
            InitializeComponent();

            if (intRowIndex >= 0)
                lbl_Definition.Text += intRowIndex.ToString();
            else
                lbl_Definition.Text = "Set value to all marks";

            txt_SetValue.Text = intCurrentSetValue.ToString();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            /*if (m_intRowIndex >= 0)
            {
                m_objOCV.SetCharSetting(m_intRowIndex, Convert.ToInt32(txt_SetValue.Text));
                m_objOCV.SetCharParameters(640, 480, m_intRowIndex, Convert.ToInt32(txt_SetValue.Text));
            }
            else
            {
                m_objOCV.SetAllCharsSetting(Convert.ToInt32(txt_SetValue.Text));
                m_objOCV.SetAllCharsParameters(640, 480, Convert.ToInt32(txt_SetValue.Text));
            }*/

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