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
    public partial class SetPadGroupNoForm : Form
    {
        private bool m_blnShow = false;

        #region Properties
        public bool ref_blnShow { get { return m_blnShow; } set { m_blnShow = value; } }
        #endregion

        public SetPadGroupNoForm()
        {
            InitializeComponent();
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

        private void SetPadGroupNoForm_Load(object sender, EventArgs e)
        {
            m_blnShow = true;
        }

        private void SetPadGroupNoForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_blnShow = false;
        }
    }
}