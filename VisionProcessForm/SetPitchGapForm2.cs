using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VisionProcessForm
{
    public partial class SetPitchGapForm2 : Form
    {
        #region Properties
        public string ref_strToPadNo { get { return cbo_ToPadNo.SelectedItem.ToString(); } }
        #endregion

        public SetPitchGapForm2(int intTotalPadNo, int intCurrentFromPadNo, string strCurrentToPadNo)
        {
            InitializeComponent();

            lbl_FromPadNo.Text = intCurrentFromPadNo.ToString();

            cbo_ToPadNo.Items.Clear();
            cbo_ToPadNo.Items.Add("NA");
            for (int i = 1; i <= intTotalPadNo; i++)
            {
                cbo_ToPadNo.Items.Add(i.ToString());
            }
            cbo_ToPadNo.SelectedItem = strCurrentToPadNo;
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