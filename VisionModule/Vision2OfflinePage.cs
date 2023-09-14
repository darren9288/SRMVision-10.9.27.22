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
using SharedMemory;
using VisionProcessing;

namespace VisionModule
{
    public partial class Vision2OfflinePage : Form
    {
        #region Member Variables
        private bool m_blnFailUnitPresent = false;
        private bool m_blnFailUnitOffset = false;

        private bool m_blnInitDone = false;
        private int m_intUserGroup;
        private string m_strFolderPath;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        
        #endregion

        public Vision2OfflinePage(VisionInfo smVisionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            m_intUserGroup = intUserGroup;
            m_smVisionInfo = smVisionInfo;
            m_smProductionInfo = smProductionInfo;
            m_strFolderPath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" +
                m_smVisionInfo.g_strVisionFolderName + "\\CheckPresent\\";
            
            InitializeComponent();

            UpdateGUI();
            m_blnInitDone = true;
        }

        private void UpdateGUI()
        {
            //for (int i = 0; i < m_smVisionInfo.g_objUnitPresent.ref_intTemplateObjectsCount; i++)
            //{
            //    dgd_UnitPresent.Rows.Add();
            //    dgd_UnitPresent.Rows[i].Cells[0].Value = (i + 1).ToString();
            //    dgd_UnitPresent.Rows[i].Cells[1].Value = m_smVisionInfo.g_objUnitPresent.GetTemplateObjectMinArea(i);

            //    dgd_UnitOffSet.Rows.Add();
            //    dgd_UnitOffSet.Rows[i].Cells[0].Value = (i + 1).ToString();
            //    dgd_UnitOffSet.Rows[i].Cells[1].Value = m_smVisionInfo.g_objUnitPresent.GetTemplateObjectMinOffSet(i);

            //}

            if (m_smVisionInfo.g_objUnitPresent.ref_intDefineUnitMethod == 0)
            {
                tab_VisionControl.TabPages.Remove(tabPage_UnitOffSet);
            }
        }

        private void UpdateInfo()
        {
            dgd_UnitPresent.Rows.Clear();

            for (int i = 0; i < m_smVisionInfo.g_objUnitPresent.ref_intTemplateObjectsCount; i++)
            {
                dgd_UnitPresent.Rows.Add();
                dgd_UnitPresent.Rows[i].Cells[0].Value = (i + 1).ToString();
            }

            lbl_GrabDelay.Text = m_smVisionInfo.g_intCameraGrabDelay.ToString();
            lbl_GrabTime.Text = (Math.Max(0, m_smVisionInfo.g_objGrabTime.Duration - m_smVisionInfo.g_intCameraGrabDelay)).ToString("f0"); //29-05-2019 ZJYEOH : Need to minus grab delay as it included inside Grab time 
            lbl_ProcessTime.Text = (Math.Max(0, m_smVisionInfo.g_objTotalTime.Duration - m_smVisionInfo.g_objGrabTime.Duration)).ToString("f2");
            lbl_TotalTime.Text = m_smVisionInfo.g_objTotalTime.Duration.ToString("f2");

            lbl_TestResultIndicatorUnit1.Text = m_smVisionInfo.g_strResult;
            switch (m_smVisionInfo.g_strResult)
            {
                case "Pass":
                    lbl_TestResultIndicatorUnit1.BackColor = Color.Lime;
                    break;
                case "Fail":
                    lbl_TestResultIndicatorUnit1.BackColor = Color.Red;
                    break;
            }

            for (int i = 0; i < dgd_UnitPresent.Rows.Count; i++)
            {
                if (m_smVisionInfo.g_objUnitPresent.ref_intDefineUnitMethod == 0)
                {
                    dgd_UnitPresent.Rows[i].Cells[1].Value = m_smVisionInfo.g_objUnitPresent.GetSampleObjectArea(i);

                    if (m_smVisionInfo.g_objUnitPresent.IsUnitPresent(i))
                    {
                        m_blnFailUnitPresent = true;
                        dgd_UnitPresent.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        dgd_UnitPresent.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_UnitPresent.Rows[i].Cells[2].Value = "Present";
                        dgd_UnitPresent.Rows[i].Cells[2].Style.BackColor = Color.Red;
                        dgd_UnitPresent.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;

                    }
                    else
                    {
                        dgd_UnitPresent.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        dgd_UnitPresent.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                        dgd_UnitPresent.Rows[i].Cells[2].Value = "Empty";
                        dgd_UnitPresent.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                        dgd_UnitPresent.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                    }


                    //if (m_smVisionInfo.g_objUnitPresent.IsUnitEmpty(i) == 1)
                    //{
                    //    dgd_UnitPresent.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                    //    dgd_UnitPresent.Rows[i].Cells[2].Value = "Empty";
                    //    dgd_UnitPresent.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                    //}
                    //else if (m_smVisionInfo.g_objUnitPresent.IsUnitEmpty(i) == 0)
                    //{
                    //    dgd_UnitPresent.Rows[i].Cells[1].Style.BackColor = Color.Red;
                    //    dgd_UnitPresent.Rows[i].Cells[2].Value = "Present";
                    //    dgd_UnitPresent.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    //}
                    //else
                    //{
                    //    dgd_UnitPresent.Rows[i].Cells[1].Style.BackColor = Color.Yellow;
                    //    dgd_UnitPresent.Rows[i].Cells[2].Value = "-";
                    //    dgd_UnitPresent.Rows[i].Cells[2].Style.BackColor = Color.Yellow;
                    //}
                }
                else
                {
                    dgd_UnitPresent.Rows[i].Cells[1].Value = m_smVisionInfo.g_objUnitPresent.GetSampleObjectArea(i);

                    if (m_smVisionInfo.g_objUnitPresent.IsUnitPresent(i))
                    {
                        dgd_UnitPresent.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        dgd_UnitPresent.Rows[i].Cells[1].Style.SelectionBackColor = Color.Lime;
                        dgd_UnitPresent.Rows[i].Cells[2].Value = "Present";
                        dgd_UnitPresent.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                        dgd_UnitPresent.Rows[i].Cells[2].Style.SelectionBackColor = Color.Lime;
                    }
                    else
                    {
                        m_blnFailUnitPresent = true;
                        dgd_UnitPresent.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        dgd_UnitPresent.Rows[i].Cells[1].Style.SelectionBackColor = Color.Red;
                        dgd_UnitPresent.Rows[i].Cells[2].Value = "Empty";
                        dgd_UnitPresent.Rows[i].Cells[2].Style.BackColor = Color.Red;
                        dgd_UnitPresent.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                    }

                    if (m_smVisionInfo.g_objUnitPresent.ref_intUnitOffSetFailMask >= 0)
                    {
                        dgd_UnitOffSet.Rows[i].Cells[1].Value = m_smVisionInfo.g_objUnitPresent.GetSampleObjectOffSet(i);

                        if (m_smVisionInfo.g_objUnitPresent.IsUnitWithinOffSetLimit(i))
                        {
                            dgd_UnitOffSet.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                            dgd_UnitOffSet.Rows[i].Cells[2].Value = "In";
                            dgd_UnitOffSet.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                        }
                        else
                        {
                            m_blnFailUnitOffset = true;
                            dgd_UnitOffSet.Rows[i].Cells[1].Style.BackColor = Color.Red;
                            dgd_UnitOffSet.Rows[i].Cells[2].Value = "Out";
                            dgd_UnitOffSet.Rows[i].Cells[2].Style.BackColor = Color.Red;
                        }
                    }
                    else
                    {
                        dgd_UnitOffSet.Rows[i].Cells[1].Value = "";
                        dgd_UnitOffSet.Rows[i].Cells[1].Style.BackColor = Color.Lime;
                        dgd_UnitOffSet.Rows[i].Cells[2].Value = "";
                        dgd_UnitOffSet.Rows[i].Cells[2].Style.BackColor = Color.Lime;
                    }
                }
            }
        }

        public void CloseOfflinePage()
        {
            m_smVisionInfo.g_blnViewUnitPresentObjectsBuilded = false;
            //m_smVisionInfo.g_objUnitPresent.LoadUnitPresent(m_strFolderPath + "Setting.xml", "UnitPresentSetting");
            m_smVisionInfo.g_blnViewOfflinePage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_OfflinePageView = false;
            m_smVisionInfo.AT_VM_ManualTestMode = false;
            this.Hide();
        }

        private void btn_Inspect_Click(object sender, EventArgs e)
        {
            btn_Inspect.Enabled = false;

            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Start Offline Test", " Pressed Test Button", "", "", m_smProductionInfo.g_strLotID);

            // 29-07-2019 ZJYEOH : If user pressed test button, then Image should stop live
            if (m_smVisionInfo.AT_PR_StartLiveImage)
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            }

            m_smVisionInfo.AT_VM_ManualTestMode = true;
            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
            if (chk_Grab.Checked)
                m_smVisionInfo.MN_PR_GrabImage = true;
        }
        
        private void btn_Close_Click(object sender, EventArgs e)
        {
            m_blnFailUnitPresent = false;
            m_blnFailUnitOffset = false;
            UpdateTabPageHeaderImage();

            //m_STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + "-Offline Test Page Closed", "Exit Offline Test Page", "", "", m_smProductionInfo.g_strLotID);
            m_smVisionInfo.g_blnViewUnitPresentObjectsBuilded = false;
            //m_smVisionInfo.g_objUnitPresent.LoadUnitPresent(m_strFolderPath + "Setting.xml", "UnitPresentSetting");
            m_smVisionInfo.g_blnViewOfflinePage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_OfflinePageView = false;
            m_smVisionInfo.AT_VM_ManualTestMode = false;
            this.Hide();
        }





        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_smVisionInfo.PR_MN_UpdateInfo)
            {
                m_blnFailUnitPresent = false;
                m_blnFailUnitOffset = false;
                UpdateTabPageHeaderImage();

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

        private void Vision2OfflinePage_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_objUnitPresent.ref_bFinalResult = false;
            m_smVisionInfo.VM_AT_OfflinePageView = false;
        }

        private void Vision2OfflinePage_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;
            m_smVisionInfo.VM_AT_OfflinePageView = true;
            m_smVisionInfo.g_blnViewSearchROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        public void LoadEvent()
        {  // Cursor.Current = Cursors.Default;
            m_smVisionInfo.VM_AT_OfflinePageView = true;
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        public void OnOffTimer(bool blnOn)
        {
            timer1.Enabled = blnOn;
        }

        public bool GetTimerStatus()
        {
            return timer1.Enabled;
        }

        public void CustomizeGUI()
        {
            m_smVisionInfo.VM_AT_OfflinePageView = true;
            UpdateInfo();

        }

        private void UpdateTabPageHeaderImage()
        {
            if (m_blnFailUnitPresent)
                tabPage_UnitPresent.ImageIndex = 1;
            else
                tabPage_UnitPresent.ImageIndex = 0;

            if (m_blnFailUnitOffset)
                tabPage_UnitOffSet.ImageIndex = 1;
            else
                tabPage_UnitOffSet.ImageIndex = 0;
        }
    }
}
