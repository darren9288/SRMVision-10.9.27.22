using System;
using System.Threading;
using System.Windows.Forms;
using SharedMemory;
using Microsoft.Win32;

namespace SRMVision
{
    public partial class SplashForm : Form
    {
        #region Members Variables

        private static bool m_blnUpsOn;
        private static string m_strVersion;
        private SplashCloseDelegate m_CloseDelegate;

        private static SplashForm m_objSplash;
        private static ProductionInfo m_smProductionInfo;
        private static CustomOption m_smCustomizeInfo;

        private static int m_intLanguageCulture = 1;
        #endregion


        public SplashForm(bool blnUpsOn, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo, int intLanguageCulture)
        {
            InitializeComponent();
            m_intLanguageCulture = intLanguageCulture;
            m_blnUpsOn = blnUpsOn;
            m_smProductionInfo = smProductionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strVersion = Application.ProductVersion;
            
            m_CloseDelegate = new SplashCloseDelegate(InternalCloseSplash);

            UpdateGUI();
        }

        private void UpdateGUI()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subKey = key.CreateSubKey("SVG");
            label1.Text = subKey.GetValue("CompanyName", "SRM Integration (M) Sdn Bhd").ToString();
            if (label1.Text.IndexOf("Integration") > 0)
                label1.Text = "SRM INTEGRATION";
            else
                label1.Text = "SRM HITECH";
        }


        public delegate void SplashCloseDelegate();

        private static void SplashThreadFunc()
        {
            m_objSplash = new SplashForm(m_blnUpsOn, m_smProductionInfo, m_smCustomizeInfo, m_intLanguageCulture);
            m_objSplash.TopMost = true;
            m_objSplash.UpdateTimer.Enabled = true;

            if (m_blnUpsOn)
            {
                m_objSplash.PowerOFFLabel.Visible = true;
                m_objSplash.ShutDownLabel.Visible = true;
                m_objSplash.StatusLabel.Visible = false;
                m_objSplash.InitializingProgressBar.Visible = false;
            }

#if (Debug_2_12 || Release_2_12)
            m_objSplash.VersionLabel.Text = "Version " + m_strVersion + " (2.12)";
#else
            m_objSplash.VersionLabel.Text = "Version " + m_strVersion;
#endif
            
            m_objSplash.ShowDialog();
        }


        public void CloseSplash()
        {
            if (m_objSplash != null)
                m_objSplash.Invoke(m_objSplash.m_CloseDelegate);

            this.Close();
        }

        public void StartSplash()
        {
            // Create and Start the splash thread
            Thread splashThread = new Thread(new ThreadStart(SplashThreadFunc));
            splashThread.Priority = ThreadPriority.Lowest;
            splashThread.Start();
        }


        private void InternalCloseSplash()
        {
            this.Close();
            this.Dispose();
        }



        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if(m_smProductionInfo.AT_SF_CheckingDongle)
            {
                if (m_intLanguageCulture == 1)
                    m_objSplash.StatusLabel.Text = "Checking Dongle ...";
                else
                    m_objSplash.StatusLabel.Text = "检查 Dongle ...";
                if (m_objSplash.InitializingProgressBar.Value < 20)
                    m_objSplash.InitializingProgressBar.Increment(1);
            }
            else if (m_smProductionInfo.AT_SF_ValidatingCamera)
            {
                if (m_intLanguageCulture == 1)
                    m_objSplash.StatusLabel.Text = "Validate Camera ...";
                else
                    m_objSplash.StatusLabel.Text = "验证相机 ...";
                if (m_objSplash.InitializingProgressBar.Value < 30)
                    m_objSplash.InitializingProgressBar.Value = 30;
                else if (m_objSplash.InitializingProgressBar.Value < 50)
                    m_objSplash.InitializingProgressBar.Increment(1);
            }
            else if (m_smProductionInfo.AT_SF_CheckingCamera)
            {
                if (m_intLanguageCulture == 1)
                    m_objSplash.StatusLabel.Text = "Checking Camera Quantity ...";
                else
                    m_objSplash.StatusLabel.Text = "检查相机数量 ...";
                if (m_objSplash.InitializingProgressBar.Value < 20)
                    m_objSplash.InitializingProgressBar.Value = 20;
                else if (m_objSplash.InitializingProgressBar.Value < 30)
                    m_objSplash.InitializingProgressBar.Increment(1);
            }
            else if (m_smProductionInfo.AT_SF_Initializing)
            {
                if (m_intLanguageCulture == 1)
                    m_objSplash.StatusLabel.Text = "Initializing Environment ...";
                else
                    m_objSplash.StatusLabel.Text = "初始化环境 ...";
                if (m_objSplash.InitializingProgressBar.Value < 50)
                    m_objSplash.InitializingProgressBar.Value = 50;
                else if (m_objSplash.InitializingProgressBar.Value < 80)
                    m_objSplash.InitializingProgressBar.Increment(1);
            }
            else if (m_smProductionInfo.AT_SF_ScanningIO)
            {
                if (m_intLanguageCulture == 1)
                    m_objSplash.StatusLabel.Text = "Initializing Environment ...";
                else
                    m_objSplash.StatusLabel.Text = "初始化环境 ...";
                if (m_objSplash.InitializingProgressBar.Value < 80)
                    m_objSplash.InitializingProgressBar.Value = 80;
                else if (m_objSplash.InitializingProgressBar.Value < 90)
                    m_objSplash.InitializingProgressBar.Increment(1);
            }
            else if (m_smProductionInfo.AT_SF_LoadInterface)
            {
                if (m_intLanguageCulture == 1)
                    m_objSplash.StatusLabel.Text = "Loading Interface ...";
                else
                    m_objSplash.StatusLabel.Text = "加载界面 ...";
                if (m_objSplash.InitializingProgressBar.Value < 90)
                    m_objSplash.InitializingProgressBar.Value = 90;
                else if (m_objSplash.InitializingProgressBar.Value < 100)
                    m_objSplash.InitializingProgressBar.Increment(1);
            }

            m_objSplash.InitializingProgressBar.Refresh();
        }

    }
}