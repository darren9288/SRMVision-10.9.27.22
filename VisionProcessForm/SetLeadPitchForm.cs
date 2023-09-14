using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VisionProcessForm
{
    public partial class SetLeadPitchForm : Form
    {
        #region Properties
        public int ref_intFromLeadNo { get { return Convert.ToInt32(cbo_FromLeadNo.SelectedItem); } }
        public int ref_intToLeadNo { get { return Convert.ToInt32(cbo_ToLeadNo.SelectedItem); } }
        #endregion

        public SetLeadPitchForm(int intTotalLeadNo, int intCurrentFromLeadNo, int intCurrentToLeadNo, int intNoID)
        {
            InitializeComponent();

            for (int i = 0; i < intTotalLeadNo; i++)
            {
                cbo_FromLeadNo.Items.Add(i + intNoID);
                cbo_ToLeadNo.Items.Add(i + intNoID);
            }
            cbo_FromLeadNo.SelectedIndex = intCurrentFromLeadNo;
            cbo_ToLeadNo.SelectedIndex = intCurrentToLeadNo;
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