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
    public partial class SetValueForm : Form
    {
        private bool m_blnMouseClickFirstTime = true;

        #region Properties
        public float ref_fSetValue { get { return float.Parse(txt_SetValue.Text); } }
        public bool ref_blnSetAllRows { get { return chk_SetAllRows.Checked; } }
        public bool ref_blnSetAllSameGroup { get { return chk_SetAllSameGroup.Checked; } }
        public bool ref_blnSetAllROI { get { return chk_SetAllROI.Checked; } }
        public bool ref_blnSetAllEdges { get { return chk_SetAllEdges.Checked; } }
        #endregion

        public SetValueForm(string strDescription, string strUnitLable, int intRowIndex, int intDecimalPlaces, string strCurrentSetValue)
        {
            InitializeComponent();
            
            srmLabel1.Text = strUnitLable;

            lbl_Definition.Text = strDescription;

            if (intRowIndex >= 0)
            {
                txt_SetValue.DecimalPlaces = intDecimalPlaces;
                txt_SetValue.Text = strCurrentSetValue;
            }
        }

        public SetValueForm(string strDescription, string strUnitLable, int intRowIndex, int intDecimalPlaces, string strCurrentSetValue, int intSetAllROI, bool blnSetAllEdgesVisible, bool blnSetAllSameGroupVisble)
        {
            InitializeComponent();
            
            srmLabel1.Text = strUnitLable;
            lbl_Definition.Text = strDescription;

            if (intRowIndex >= 0)
            {
                txt_SetValue.DecimalPlaces = intDecimalPlaces;
                txt_SetValue.Text = strCurrentSetValue;
            }

            if (intSetAllROI != 0)
            {
                pnl_SetAllROI.Visible = true;
                chk_SetAllROI.Enabled = false;
            }
            else
                pnl_SetAllROI.Visible = false;

            pnl_SetAllEdges.Visible = blnSetAllEdgesVisible;

            pnl_SetAllSameGroup.Visible = blnSetAllSameGroupVisble;
        }

        public SetValueForm(string strDescription, string strUnitLable, int intRowIndex, int intDecimalPlaces, string strCurrentSetValue, bool blnSetAllRows, bool blnSetAllEdgesVisible, bool blnSetAllSameGroupVisble)
        {
            InitializeComponent();
            
            srmLabel1.Text = strUnitLable;
            lbl_Definition.Text = strDescription;

            if (blnSetAllRows)
            {
                chk_SetAllRows.Checked = blnSetAllRows;
                chk_SetAllRows.Enabled = false;
                txt_SetValue.DecimalPlaces = intDecimalPlaces;
                txt_SetValue.Text = strCurrentSetValue;
            }
            else
            {
                if (intRowIndex >= 0)
                {
                    chk_SetAllRows.Checked = blnSetAllRows;
                    chk_SetAllRows.Enabled = false;
                    txt_SetValue.DecimalPlaces = intDecimalPlaces;
                    txt_SetValue.Text = strCurrentSetValue;
                }
            }

            pnl_SetAllEdges.Visible = blnSetAllEdgesVisible;

            if(chk_SetAllRows.Enabled)
                pnl_SetAllSameGroup.Visible = blnSetAllSameGroupVisble;
            else
                pnl_SetAllSameGroup.Visible = false;
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

        private void chk_SetAllSameGroupRows_Click(object sender, EventArgs e)
        {
            //if (chk_SetAllROI.Checked)
            //{
            //    if (chk_SetAllRows.Checked)
            //        chk_SetAllRows.Checked = false;
            //}
        }

        private void chk_SetAllRows_Click(object sender, EventArgs e)
        {
            if (chk_SetAllRows.Checked)
            {
                chk_SetAllROI.Enabled = true;
                if (pnl_SetAllSameGroup.Visible)
                {
                    if (chk_SetAllSameGroup.Checked)
                        chk_SetAllSameGroup.Checked = false;
                }
            }
            else
            {
                chk_SetAllROI.Enabled = false;
                chk_SetAllROI.Checked = false;
            }
        }

        private void txt_SetValue_MouseClick(object sender, MouseEventArgs e)
        {
            if (m_blnMouseClickFirstTime)
            {
                m_blnMouseClickFirstTime = false;

                // 2019 06 27 - select character that have number only.
                char[] arrChar = {'1', '2', '3', '4', '5', '6', '7', '8', '9' };
                int intSelectionStart = txt_SetValue.Text.IndexOfAny(arrChar);
                int intSelectionLength = txt_SetValue.Text.Length - intSelectionStart;
                if (intSelectionStart < 0)
                    return;
                txt_SetValue.Focus();
                txt_SetValue.SelectionStart = intSelectionStart;
                txt_SetValue.SelectionLength = intSelectionLength;
            }
        }

        private void chk_SetAllSameGroup_Click(object sender, EventArgs e)
        {
            if (chk_SetAllSameGroup.Checked)
            {
                if (pnl_SetAllRows.Visible)
                {
                    if (chk_SetAllRows.Checked)
                    {
                        chk_SetAllRows.Checked = false;
                        chk_SetAllROI.Enabled = false;
                        chk_SetAllROI.Checked = false;
                    }
                }

                if (pnl_SetAllROI.Visible)
                {
                    if (chk_SetAllROI.Checked)
                        chk_SetAllROI.Checked = false;
                }
            }
        }

        private void chk_SetAllROI_Click(object sender, EventArgs e)
        {
            if (chk_SetAllROI.Checked)
            {
                if (pnl_SetAllSameGroup.Visible)
                {
                    if (chk_SetAllSameGroup.Checked)
                        chk_SetAllSameGroup.Checked = false;
                }
            }
            
        }
    }
}