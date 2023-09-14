using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SRMVision
{
    public partial class WaitShutdownForm : Form
    {
        private bool m_blnStopShutDown = false;
        private bool m_blnShutDownNow = false;

        public bool ref_blnStopShutDown { get { return m_blnStopShutDown; } set { m_blnStopShutDown = value; } }
        public bool ref_blnShutDownNow { get { return m_blnShutDownNow; } set { m_blnShutDownNow = value; } }

        public WaitShutdownForm()
        {
            InitializeComponent();
        }

        public void UpdateTimerDisplay(int intTimer)
        {
            lbl_Timer.Text = intTimer.ToString();
        }

        private void btn_Stop_Click(object sender, EventArgs e)
        {
            m_blnStopShutDown = true;
            this.Close();
        }

        private void btn_ShutdownNow_Click(object sender, EventArgs e)
        {
            m_blnShutDownNow = true;
            this.Close();
        }
    }
}