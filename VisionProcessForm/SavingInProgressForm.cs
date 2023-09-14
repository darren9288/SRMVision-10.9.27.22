using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedMemory;
using System.Threading;
namespace VisionProcessForm
{
    public partial class SavingInProgressForm : Form
    {
        private string m_strDot = "";
        private SplashCloseDelegate m_CloseDelegate;

        private static SavingInProgressForm m_objSaveForm;
        private static VisionInfo m_smVisionInfo;
        private static CustomOption m_smCustomizeInfo;
        public SavingInProgressForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo)
        {
           
            InitializeComponent();
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_CloseDelegate = new SplashCloseDelegate(InternalCloseSplash);
      
        }
        public void StartSaveForm()
        {
            // Create and Start the splash thread
            Thread splashThread = new Thread(new ThreadStart(SplashThreadFunc));
            splashThread.Priority = ThreadPriority.Lowest;
            splashThread.Start();
        }
        public delegate void SplashCloseDelegate();

        private static void SplashThreadFunc()
        {
            m_objSaveForm = new SavingInProgressForm(m_smVisionInfo, m_smCustomizeInfo);
            m_objSaveForm.TopMost = true;
            m_objSaveForm.timer1.Enabled = true;

            m_objSaveForm.ShowDialog();
        }
        public void CloseSaveForm()
        {
            if (m_objSaveForm != null)
                m_objSaveForm.Invoke(m_objSaveForm.m_CloseDelegate);

            this.Close();
        }
      
        private void InternalCloseSplash()
        {
            this.Close();
            this.Dispose();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_strDot.Length < 5)
                m_strDot += ".";
            else
                m_strDot = "";

            if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                label1.Text = "Saving In Progress" + m_strDot;
            else
                label1.Text = "正在保存" + m_strDot;
            lbl_Percent.Text = m_objSaveForm.progressBar1.Value.ToString() + "%";
            if (m_smVisionInfo.g_intSavingState == 1)
            {
                if (m_objSaveForm.progressBar1.Value < 10)
                    m_objSaveForm.progressBar1.Increment(1);
            }
            else if (m_smVisionInfo.g_intSavingState == 2)
            {
                if (m_objSaveForm.progressBar1.Value < 10)
                    m_objSaveForm.progressBar1.Value = 10;
                else if (m_objSaveForm.progressBar1.Value < 20)
                    m_objSaveForm.progressBar1.Increment(1);
            }
            else if (m_smVisionInfo.g_intSavingState == 3)
            {
                if (m_objSaveForm.progressBar1.Value < 20)
                    m_objSaveForm.progressBar1.Value = 20;
                else if (m_objSaveForm.progressBar1.Value < 30)
                    m_objSaveForm.progressBar1.Increment(1);
            }
            else if (m_smVisionInfo.g_intSavingState == 4)
            {
                if (m_objSaveForm.progressBar1.Value < 30)
                    m_objSaveForm.progressBar1.Value = 30;
                else if (m_objSaveForm.progressBar1.Value < 40)
                    m_objSaveForm.progressBar1.Increment(1);
            }
            else if (m_smVisionInfo.g_intSavingState == 5)
            {
                if (m_objSaveForm.progressBar1.Value < 40)
                    m_objSaveForm.progressBar1.Value = 40;
                else if (m_objSaveForm.progressBar1.Value < 50)
                    m_objSaveForm.progressBar1.Increment(1);
            }
            else if (m_smVisionInfo.g_intSavingState == 6)
            {
                if (m_objSaveForm.progressBar1.Value < 50)
                    m_objSaveForm.progressBar1.Value = 50;
                else if (m_objSaveForm.progressBar1.Value < 60)
                    m_objSaveForm.progressBar1.Increment(1);
            }
            else if (m_smVisionInfo.g_intSavingState == 7)
            {
                if (m_objSaveForm.progressBar1.Value < 60)
                    m_objSaveForm.progressBar1.Value = 60;
                else if (m_objSaveForm.progressBar1.Value < 70)
                    m_objSaveForm.progressBar1.Increment(1);
            }
            else if (m_smVisionInfo.g_intSavingState == 8)
            {
                if (m_objSaveForm.progressBar1.Value < 70)
                    m_objSaveForm.progressBar1.Value = 70;
                else if (m_objSaveForm.progressBar1.Value < 80)
                    m_objSaveForm.progressBar1.Increment(1);
            }
            else if (m_smVisionInfo.g_intSavingState == 9)
            {
                if (m_objSaveForm.progressBar1.Value < 80)
                    m_objSaveForm.progressBar1.Value = 80;
                else if (m_objSaveForm.progressBar1.Value < 90)
                    m_objSaveForm.progressBar1.Increment(1);
            }
            else if (m_smVisionInfo.g_intSavingState == 10)
            {
                if (m_objSaveForm.progressBar1.Value < 90)
                    m_objSaveForm.progressBar1.Value = 90;
                else if (m_objSaveForm.progressBar1.Value < 100)
                    m_objSaveForm.progressBar1.Increment(1);
            }
            m_objSaveForm.progressBar1.Refresh();
        }
    }
}
