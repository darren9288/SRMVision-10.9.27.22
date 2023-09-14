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
    public partial class SetCharsEnableForm : Form
    {
        #region Properties
        public bool ref_blnEnableMark { get { return chk_EnableDisable.Checked; } }
        public bool ref_blnSetAllRows { get { return chk_SetAllRows.Checked; } }

        #endregion

        public SetCharsEnableForm(int intRowIndex, bool blnCurrentIsEnable)
        {
            InitializeComponent();

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

            if (intRowIndex >= 0)
            {
                chk_EnableDisable.Visible = true;
                chk_EnableDisable.Checked = blnCurrentIsEnable;
            }
            else
            {
                chk_EnableDisable.Visible = true;
                chk_EnableDisable.Checked = false;
            }
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