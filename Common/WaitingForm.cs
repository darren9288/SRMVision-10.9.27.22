using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Common
{
    public partial class WaitingForm : Form
    {
        #region Member Variables

        private string m_startTime = DateTime.Now.ToString(new CultureInfo("en-US"));

        #endregion

        public WaitingForm()
        {
            InitializeComponent();            
            StopButton.Visible = false;
        }

        public void UpdateGUI(string message)
        {
            DisplayLabel.Text = message;
        }
   
        private void StopButton_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            Application.DoEvents();

            if (StopButton.Visible)
                return;

            TimeSpan duration = DateTime.Now.Subtract(Convert.ToDateTime(m_startTime, new CultureInfo("en-US")));
            if (duration.TotalSeconds > 20)
            {
                StopButton.Visible = true;
            }
        }
    }

    //Waiting form thread, make into thread so that it can run when other thread is running
    public class SRMWaitingFormThread
    {
        #region Members Variables

        private bool m_blnStopping = false;
        private WaitingForm m_waitForm;
        private Thread m_thWaitingFormThread;
        private bool m_blnStartSplash = false;
        private bool m_blnStopSplash = false;
        private string m_StrMessage = "";
        #endregion

        public SRMWaitingFormThread()
        {
            m_thWaitingFormThread = new Thread(new ThreadStart(UpdateProgress));
            m_thWaitingFormThread.IsBackground = true;
            m_thWaitingFormThread.Priority = ThreadPriority.Lowest;
            m_thWaitingFormThread.Start();
        }

        private void StartSplash()
        {
            if (m_waitForm == null)
            {
                m_waitForm = new WaitingForm();
                m_waitForm.UpdateGUI(m_StrMessage);
                m_waitForm.Show();
                Application.DoEvents();
            }
        }

        private void StopSplash()
        {
            if (m_waitForm != null)
            {
                m_waitForm.Close();
                m_waitForm.Dispose();
                m_waitForm = null;
                Application.DoEvents();
            }
        }

        public void SetStartSplash(string strMessage)
        {
            m_blnStartSplash = true;
            m_StrMessage = strMessage;
        }

        public void SetStopSplash()
        {
            m_blnStopSplash = true;
        }

        private void UpdateProgress()
        {
            STTrackLog.WriteLine("Start SRMWaitingFormThread -> UpdateProgress()");

            try
            {
                while (!m_blnStopping)
                {
                    if (m_blnStartSplash)
                    {
                        m_blnStartSplash = false;
                        StartSplash();
                    }

                    if (m_blnStopSplash)
                    {
                        m_blnStopSplash = false;
                        StopSplash();
                        m_blnStopping = true;
                    }
                    Application.DoEvents();
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                SRMMessageBox.Show("SRMWaitingFormThread UpdateProgress Exception: " + ex.ToString());
            }
            finally
            {
                m_thWaitingFormThread = null;
            }

            STTrackLog.WriteLine("End SRMWaitingFormThread -> UpdateProgress()");
        }
    }
}

