using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VisionProcessForm
{
    public partial class SetLead3DPitchGapForm : Form
    {
        #region Properties
        public string ref_strToLeadNo { get { return cbo_ToLeadNo.SelectedItem.ToString(); } }
        #endregion

        public SetLead3DPitchGapForm(List<int> arrintTotalLeadNo, int intCurrentFromLeadNo, string strCurrentToLeadNo)
        {
            InitializeComponent();

            lbl_FromLeadNo.Text = intCurrentFromLeadNo.ToString();

            cbo_ToLeadNo.Items.Clear();
            cbo_ToLeadNo.Items.Add("NA");
            for (int i = 0; i < arrintTotalLeadNo.Count; i++)
            {
                cbo_ToLeadNo.Items.Add(arrintTotalLeadNo[i].ToString());
            }
            cbo_ToLeadNo.SelectedItem = strCurrentToLeadNo;
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
