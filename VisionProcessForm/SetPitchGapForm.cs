using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VisionProcessForm
{
    public partial class SetPitchGapForm : Form
    {
        #region Properties
        public int ref_intFromPadNo { get { return Convert.ToInt32(cbo_FromPadNo.SelectedItem); } }
        public int ref_intToPadNo { get { return Convert.ToInt32(cbo_ToPadNo.SelectedItem); } }
        #endregion

        public SetPitchGapForm(int intTotalPadNo, int intCurrentFromPadNo, int intCurrentToPadNo)
        {
            InitializeComponent();

            for (int i = 1; i <= intTotalPadNo; i++)
            {
                cbo_FromPadNo.Items.Add(i);
                cbo_ToPadNo.Items.Add(i);
            }
            cbo_FromPadNo.SelectedIndex = intCurrentFromPadNo;
            cbo_ToPadNo.SelectedIndex = intCurrentToPadNo;
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