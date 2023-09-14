using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
namespace History
{
    public partial class CPKDisplaySettingForm : Form
    {
        #region Member Variables

        private int m_intROIMask;
        private List<string> m_strROI = new List<string>();
        #endregion

        #region Properties

        public int ref_intROIMask { get { return m_intROIMask; } }
        public string[] ref_strROI { get { return m_strROI.ToArray(); } }

        #endregion


        public CPKDisplaySettingForm(int intROIMask)
        {
            InitializeComponent();

            m_intROIMask = intROIMask;

            UpdateGUI();
        }



        /// <summary>
        /// Customize GUI according to CPK display settings
        /// </summary>
        private void UpdateGUI()
        {
            if ((m_intROIMask & 0x01) > 0)
                chk_Middle.Checked = true;
            else
                chk_Middle.Checked = false;

            if ((m_intROIMask & 0x02) > 0)
                chk_Top.Checked = true;
            else
                chk_Top.Checked = false;

            if ((m_intROIMask & 0x04) > 0)
                chk_Right.Checked = true;
            else
                chk_Right.Checked = false;

            if ((m_intROIMask & 0x08) > 0)
                chk_Bottom.Checked = true;
            else
                chk_Bottom.Checked = false;

            if ((m_intROIMask & 0x10) > 0)
                chk_Left.Checked = true;
            else
                chk_Left.Checked = false;
        }



        private void btn_Ok_Click(object sender, EventArgs e)
        {
            m_intROIMask = 0;
            m_strROI.Clear();

            if (chk_Middle.Checked)
            {
                m_intROIMask |= 0x01;
                m_strROI.Add("Middle");
            }

            if (chk_Top.Checked)
            {
                m_intROIMask |= 0x02;
                m_strROI.Add("Top");
            }

            if (chk_Right.Checked)
            {
                m_intROIMask |= 0x04;
                m_strROI.Add("Right");
            }

            if (chk_Bottom.Checked)
            {
                m_intROIMask |= 0x08;
                m_strROI.Add("Bottom");
            }

            if (chk_Left.Checked)
            {
                m_intROIMask |= 0x10;
                m_strROI.Add("Left");
            }

            if (m_intROIMask == 0)
            {
                SRMMessageBox.Show("At least one ROI must be selected!");
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