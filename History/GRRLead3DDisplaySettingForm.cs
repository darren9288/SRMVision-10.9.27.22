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

namespace History
{
    public partial class GRRLead3DDisplaySettingForm : Form
    {
        #region Member Variables

        private int m_intParameterMask;
        private List<string> m_strParameter = new List<string>();
        #endregion

        #region Properties

        public int ref_intParameterMask { get { return m_intParameterMask; } }
        public string[] ref_strParameter { get { return m_strParameter.ToArray(); } }

        #endregion


        public GRRLead3DDisplaySettingForm(int intParameterMask)
        {
            InitializeComponent();

            m_intParameterMask = intParameterMask;

            UpdateGUI();
        }

        /// <summary>
        /// Customize GUI according to GRR display settings
        /// </summary>
        private void UpdateGUI()
        {
            if ((m_intParameterMask & 0x01) > 0)
                chk_OffSet.Checked = true;
            else
                chk_OffSet.Checked = false;
            
            if ((m_intParameterMask & 0x02) > 0)
                chk_Width.Checked = true;
            else
                chk_Width.Checked = false;

            if ((m_intParameterMask & 0x04) > 0)
                chk_Height.Checked = true;
            else
                chk_Height.Checked = false;

            if ((m_intParameterMask & 0x08) > 0)
                chk_Pitch.Checked = true;
            else
                chk_Pitch.Checked = false;

            if ((m_intParameterMask & 0x10) > 0)
                chk_Gap.Checked = true;
            else
                chk_Gap.Checked = false;
            
            if ((m_intParameterMask & 0x20) > 0)
                chk_Skew.Checked = true;
            else
                chk_Skew.Checked = false;

            if ((m_intParameterMask & 0x40) > 0)
                chk_StandOff.Checked = true;
            else
                chk_StandOff.Checked = false;

            if ((m_intParameterMask & 0x80) > 0)
                chk_Coplan.Checked = true;
            else
                chk_Coplan.Checked = false;
        }



        private void btn_Ok_Click(object sender, EventArgs e)
        {
            m_intParameterMask = 0;
            m_strParameter.Clear();

            if (chk_OffSet.Checked)
            {
                m_intParameterMask |= 0x01;
                m_strParameter.Add("OffSet");
            }

            if (chk_Width.Checked)
            {
                m_intParameterMask |= 0x02;
                m_strParameter.Add("Width");
            }

            if (chk_Height.Checked)
            {
                m_intParameterMask |= 0x04;
                m_strParameter.Add("Length");
            }

            if (chk_Pitch.Checked)
            {
                m_intParameterMask |= 0x08;
                m_strParameter.Add("Pitch");
            }

            if (chk_Gap.Checked)
            {
                m_intParameterMask |= 0x10;
                m_strParameter.Add("Gap");
            }

            if (chk_Skew.Checked)
            {
                m_intParameterMask |= 0x20;
                m_strParameter.Add("Skew");
            }

            if (chk_StandOff.Checked)
            {
                m_intParameterMask |= 0x40;
                m_strParameter.Add("StandOff");
            }

            if (chk_Coplan.Checked)
            {
                m_intParameterMask |= 0x80;
                m_strParameter.Add("Coplan");
            }

            if (m_intParameterMask == 0)
            {
                SRMMessageBox.Show("At least one parameter must be selected!");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
