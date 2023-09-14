using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using ImageAcquisition;
using VisionProcessing;
using SharedMemory;

namespace VisionModule
{
    public partial class Vision7OfflinePage : Form
    {
        #region Member Variables
        private bool m_blnFailBarcode = false;
        private bool m_blnFailPattern = false;

        private int m_intUserGroup = 5;
        private bool m_blnInitDone = false;
        private string m_strSelectedRecipe;

        private VisionInfo m_smVisionInfo;
        private UserRight m_objUserRight = new UserRight();
        private bool m_blnEnterTextBox = false;
        private ProductionInfo m_smProductionInfo;
        
        #endregion

        public Vision7OfflinePage(VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();

            m_intUserGroup = intUserGroup;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            
            UpdateGUI();

            m_blnInitDone = true;
        }

        private void UpdateGUI()
        {
            UpdateTable();

            m_blnFailPattern = false;
            m_blnFailBarcode = false;
        }

        private void UpdateInfo()
        {

            if (m_smVisionInfo.g_strResult == "Pass")
            {
                lbl_TestResultIndicatorUnit1.BackColor = Color.Lime;
                lbl_TestResultIndicatorUnit1.Text = "Pass";
            }
            else if (m_smVisionInfo.g_strResult == "Idle")
            {
                lbl_TestResultIndicatorUnit1.BackColor = Color.Yellow;
                lbl_TestResultIndicatorUnit1.Text = "Idle";
            }
            else
            {
                lbl_TestResultIndicatorUnit1.BackColor = Color.Red;
                //lbl_TestResultIndicatorUnit1.Text = "Fail";
                lbl_TestResultIndicatorUnit1.Text = m_smVisionInfo.g_strResult;
            }
            
            lbl_GrabDelay.Text = m_smVisionInfo.g_intCameraGrabDelay.ToString();
            lbl_GrabTime.Text = (Math.Max(0, m_smVisionInfo.g_objGrabTime.Duration - m_smVisionInfo.g_intCameraGrabDelay)).ToString("f0"); //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
            lbl_ProcessTime.Text = (Math.Max(0, m_smVisionInfo.g_objTotalTime.Duration - m_smVisionInfo.g_objGrabTime.Duration)).ToString("f2");
            lbl_TotalTime.Text = m_smVisionInfo.g_objTotalTime.Duration.ToString("f2");

            UpdateTable();
            m_smVisionInfo.g_blnViewBarcodeInspection = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdateTable()
        {
            dgd_Score.Rows.Clear();
            dgd_Score.Rows.Add();
            dgd_Score.Rows[0].Cells[0].Value = "Pattern";

            if (m_smVisionInfo.g_objBarcode.GetMatchingScore() > m_smVisionInfo.g_objBarcode.ref_intMinMatchingScore)
            {
                dgd_Score.Rows[0].Cells[0].Style.SelectionBackColor = Color.Lime;
                dgd_Score.Rows[0].Cells[0].Style.BackColor = Color.Lime;
                dgd_Score.Rows[0].Cells[0].Style.ForeColor = Color.Black;
                dgd_Score.Rows[0].Cells[0].Style.SelectionForeColor = Color.Black;
                dgd_Score.Rows[0].Cells[1].Style.SelectionBackColor = Color.Lime;
                dgd_Score.Rows[0].Cells[1].Style.BackColor = Color.Lime;
                dgd_Score.Rows[0].Cells[1].Style.ForeColor = Color.Black;
                dgd_Score.Rows[0].Cells[1].Style.SelectionForeColor = Color.Black;
                dgd_Score.Rows[0].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_Score.Rows[0].Cells[2].Style.BackColor = Color.Lime;
                dgd_Score.Rows[0].Cells[2].Style.ForeColor = Color.Black;
                dgd_Score.Rows[0].Cells[2].Style.SelectionForeColor = Color.Black;


                //if (m_smVisionInfo.g_objBarcode.ref_blnTestDone && m_smVisionInfo.g_objBarcode.ref_blnCodeFound && m_smVisionInfo.g_objBarcode.ref_blnCodePassed)
                //{
                //    lbl_BarcodeResult.BackColor = Color.Lime;
                //    lbl_BarcodeResult.Text = m_smVisionInfo.g_objBarcode.ref_strResultCode;
                //}
                //else if (m_smVisionInfo.g_objBarcode.ref_blnTestDone && m_smVisionInfo.g_objBarcode.ref_blnCodeNotMatched)
                //{
                //    lbl_BarcodeResult.BackColor = Color.Red;
                //    lbl_BarcodeResult.Text = m_smVisionInfo.g_objBarcode.ref_strResultCode;
                //}
                //else if (m_smVisionInfo.g_objBarcode.ref_blnTestDone)
                //{
                //    lbl_BarcodeResult.BackColor = Color.Red;
                //    lbl_BarcodeResult.Text = "----";
                //}
                //else
                //{
                //    lbl_BarcodeResult.BackColor = Color.Yellow;
                //    lbl_BarcodeResult.Text = "----";
                //}
            }
            else
            {
                m_blnFailPattern = true;
                dgd_Score.Rows[0].Cells[0].Style.SelectionBackColor = Color.Red;
                dgd_Score.Rows[0].Cells[0].Style.BackColor = Color.Red;
                dgd_Score.Rows[0].Cells[0].Style.ForeColor = Color.Yellow;
                dgd_Score.Rows[0].Cells[0].Style.SelectionForeColor = Color.Yellow;
                dgd_Score.Rows[0].Cells[1].Style.SelectionBackColor = Color.Red;
                dgd_Score.Rows[0].Cells[1].Style.BackColor = Color.Red;
                dgd_Score.Rows[0].Cells[1].Style.ForeColor = Color.Yellow;
                dgd_Score.Rows[0].Cells[1].Style.SelectionForeColor = Color.Yellow;
                dgd_Score.Rows[0].Cells[2].Style.SelectionBackColor = Color.Red;
                dgd_Score.Rows[0].Cells[2].Style.BackColor = Color.Red;
                dgd_Score.Rows[0].Cells[2].Style.ForeColor = Color.Yellow;
                dgd_Score.Rows[0].Cells[2].Style.SelectionForeColor = Color.Yellow;

                lbl_BarcodeResult.BackColor = Color.Red;
                lbl_BarcodeResult.Text = "----";

            }

            lbl_BarcodeResult.Size = new Size(lbl_BarcodeResult.Size.Width, Math.Max(100, (int)Math.Round((lbl_BarcodeResult.Text.Length / (lbl_BarcodeResult.Size.Width / lbl_BarcodeResult.Font.Size)) * lbl_BarcodeResult.Font.Height)));

            dgd_Score.Rows[0].Cells[1].Value = m_smVisionInfo.g_objBarcode.GetMatchingAngle().ToString("F2");
            dgd_Score.Rows[0].Cells[2].Value = m_smVisionInfo.g_objBarcode.GetMatchingScore().ToString("F2");

            dgd_Template.Rows.Clear();
            for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
            {
                dgd_Template.Rows.Add();
                dgd_Template.Rows[i].Cells[0].Value = (i + 1).ToString();
                if (m_smVisionInfo.g_objBarcode.ref_blnTestDone && m_smVisionInfo.g_objBarcode.ref_blnCodeFound[i] && m_smVisionInfo.g_objBarcode.ref_blnCodePassed[i])
                {
                    dgd_Template.Rows[i].Cells[1].Value = m_smVisionInfo.g_objBarcode.ref_strResultCode[i];
                    if (Math.Abs(m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i] - m_smVisionInfo.g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                    {
                        dgd_Template.Rows[i].Cells[2].Style.BackColor = Color.Red;
                        dgd_Template.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                        dgd_Template.Rows[i].Cells[0].Style.BackColor = Color.Red;
                        dgd_Template.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                        m_blnFailBarcode = true;
                    }
                    else
                    {
                        dgd_Template.Rows[i].Cells[2].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                        dgd_Template.Rows[i].Cells[2].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                        dgd_Template.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                        dgd_Template.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                    }
                    dgd_Template.Rows[i].Cells[2].Value = m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i];
         
                }
                else if (m_smVisionInfo.g_objBarcode.ref_blnTestDone && m_smVisionInfo.g_objBarcode.ref_blnCodeNotMatched[i])
                {
                    dgd_Template.Rows[i].Cells[1].Value = m_smVisionInfo.g_objBarcode.ref_strResultCode[i];
                    dgd_Template.Rows[i].Cells[2].Value = m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i];
                    dgd_Template.Rows[i].Cells[0].Style.BackColor = Color.Red;
                    dgd_Template.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                    m_blnFailBarcode = true;
                }
                else if (m_smVisionInfo.g_objBarcode.ref_blnTestDone)
                {
                    dgd_Template.Rows[i].Cells[1].Value = m_smVisionInfo.g_objBarcode.ref_strResultCode[i];
                    dgd_Template.Rows[i].Cells[1].Value = "----";
                    dgd_Template.Rows[i].Cells[0].Style.BackColor = Color.Red;
                    dgd_Template.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                    m_blnFailBarcode = true;
                }
                else
                {
                }
                
                dgd_Template.Rows[i].Cells[1].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                dgd_Template.Rows[i].Cells[1].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
            }
        }

        private void timer_TestResult_Tick(object sender, EventArgs e)
        {
            if (!m_smVisionInfo.VM_AT_OfflinePageView)
                return;
            
            if (m_smVisionInfo.PR_MN_UpdateInfo)
            {
                m_blnFailPattern = false;
                m_blnFailBarcode = false;
                UpdateInfo();
                UpdateTabPageHeaderImage();

                btn_Inspect.Enabled = true;

                m_smVisionInfo.PR_MN_UpdateInfo = false;

            }

            if (m_smVisionInfo.PR_MN_UpdateSettingInfo)
            {
                m_smVisionInfo.g_blnUpdateImageNoComboBox = true; // 2020-11-22 ZJYEOH : After exit any setting form Update image combo box because g_intProductionViewImage may not same with g_intSelectedImage
                m_smVisionInfo.PR_MN_UpdateSettingInfo = false;
            }
        }
        private void Inspect()
        {
            m_smVisionInfo.g_blnViewBarcodeInspection = false;
            m_smVisionInfo.g_blnDrawBarcodeResult = false;
            m_smVisionInfo.AT_VM_ManualTestMode = true;

            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
        }
        public void OnOffTimer(bool blnOn)
        {
            timer_TestResult.Enabled = blnOn;
        }

        public bool GetTimerStatus()
        {
            return timer_TestResult.Enabled;
        }

        public void CloseOfflinePage()
        {
            m_smVisionInfo.g_blnViewROI = m_smVisionInfo.g_blnViewSearchROI = false;
            m_smVisionInfo.g_blnViewBarcodeInspection = false;
            
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewThreshold = false;
            
            m_smVisionInfo.g_blnViewOfflinePage = false;
            m_smVisionInfo.g_blnDrawBarcodeResult = false;
            m_smVisionInfo.AT_VM_ManualTestMode = false;
            
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;
            m_smVisionInfo.g_blnUpdateImageNoComboBox = true;

            m_smVisionInfo.VM_AT_OfflinePageView = false; // 2019 01 10 - CCENG: Dont put this event in Form_Closing, bcos pressing close button is just to hide the form, not close the form.
            
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            this.Hide();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            m_blnFailPattern = false;
            m_blnFailBarcode = false;
            UpdateTabPageHeaderImage();

            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Offline Test Page Closed", "Exit Offline Test Page", "", "", m_smProductionInfo.g_strLotID);
            m_smVisionInfo.g_blnViewROI = m_smVisionInfo.g_blnViewSearchROI = false;
            m_smVisionInfo.g_blnViewMarkInspection = false;
            m_smVisionInfo.g_blnViewLeadInspection = false;

            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewThreshold = false;
            m_smVisionInfo.g_blnViewOfflinePage = false;
            m_smVisionInfo.g_blnDrawBarcodeResult = false;
            m_smVisionInfo.AT_VM_ManualTestMode = false;
            
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_intProductionViewImage;
            m_smVisionInfo.g_blnUpdateImageNoComboBox = true;

            m_smVisionInfo.VM_AT_OfflinePageView = false; // 2019 01 10 - CCENG: Dont put this event in Form_Closing, bcos pressing close button is just to hide the form, not close the form.
            
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            this.Hide();
        }

        private void btn_Inspect_Click(object sender, EventArgs e)
        {

            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Start Offline Test", " Pressed Test Button", "", "", m_smProductionInfo.g_strLotID);

            // 29-07-2019 ZJYEOH : If user pressed test button, then Image should stop live
            if (m_smVisionInfo.AT_PR_StartLiveImage)
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            }

            btn_Inspect.Enabled = false;

            if (chk_Grab.Checked)
                m_smVisionInfo.MN_PR_GrabImage = true;

            Inspect();
        }
        public void LoadEvent()
        {
            // 2019 01 11 - CCENG: Don't put this function in Load Form Event function because the Form_Load event will cannot be triggered when you hide and show it again.
            Cursor.Current = Cursors.Default;
            m_smVisionInfo.g_blnViewMarkInspection = true;
            m_smVisionInfo.VM_AT_OfflinePageView = true;
        }
        private void UpdateTabPageHeaderImage()
        {
            
            if (m_blnFailPattern)
                tp_Score.ImageIndex = 1;
            else
                tp_Score.ImageIndex = 0;

            if (m_blnFailBarcode)
                tp_Result.ImageIndex = 1;
            else
                tp_Result.ImageIndex = 0;

        }
        public void CustomizeGUI()
        {
           
            m_smVisionInfo.VM_AT_OfflinePageView = true;
            UpdateInfo();
            //Cursor.Current = Cursors.Default;


            //m_smVisionInfo.g_blnViewROI = true;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void Vision7OfflinePage_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            m_smVisionInfo.VM_AT_OfflinePageView = true;
            //m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void Vision7OfflinePage_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.VM_AT_OfflinePageView = false;
        }
    }
}
