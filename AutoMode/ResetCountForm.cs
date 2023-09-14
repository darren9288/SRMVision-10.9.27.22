using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Common;
using SharedMemory;

namespace AutoMode
{
    public partial class ResetCountForm : Form
    {
        private int m_intUserGroup = 5;
        private string m_strRecipeDirectory = "";
        private VisionInfo[] m_smVSInfo;
        private ProductionInfo m_smProductionInfo;
        private int[] m_arrVisionIndex = new int[8];
        private CustomOption m_smCustomizeInfo;
        
        public ResetCountForm(ProductionInfo smProductionInfo, VisionInfo[] smVSInfo, CustomOption smCustomizeInfo)
        {
            InitializeComponent();
            m_smVSInfo = smVSInfo;
            m_smProductionInfo = smProductionInfo;
            m_arrVisionIndex = m_smProductionInfo.g_arrDisplayVisionModule;
            m_smCustomizeInfo = smCustomizeInfo;
            m_intUserGroup = m_smProductionInfo.g_intUserGroup;
            
            UpdateGUI();
            DisableField();

        }
        private void DisableField()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Reset Counter";
            string strChild2 = "";

            strChild2 = "Reset Button";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBottomMenuChild2Group(strChild1, strChild2))
            {
                btn_ResetVision1.Enabled = false;
                btn_ResetVision2.Enabled = false;
                btn_ResetVision3.Enabled = false;
                btn_ResetVision4.Enabled = false;
                btn_ResetVision5.Enabled = false;
                btn_ResetVision6.Enabled = false;
                btn_ResetVision7.Enabled = false;
                btn_ResetVision8.Enabled = false;
                btn_ResetAll.Enabled = false;
            }
            else
            {
                btn_ResetVision1.Enabled = true;
                btn_ResetVision2.Enabled = true;
                btn_ResetVision3.Enabled = true;
                btn_ResetVision4.Enabled = true;
                btn_ResetVision5.Enabled = true;
                btn_ResetVision6.Enabled = true;
                btn_ResetVision7.Enabled = true;
                btn_ResetVision8.Enabled = true;
                btn_ResetAll.Enabled = true;
            }
        }
        private void UpdateGUI()
        {
            if (m_arrVisionIndex.Length > 0)
            {
                if (m_smVSInfo[m_arrVisionIndex[0]] != null)
                {
                    lbl_Vision1Name.Text = m_smVSInfo[m_arrVisionIndex[0]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[0]].g_strVisionNameRemark + "Tested Unit";
                    lbl_Vision1Count.Text = m_smVSInfo[m_arrVisionIndex[0]].g_intTestedTotal.ToString();
                }
                else
                {
                    lbl_Vision1Name.Visible = false;
                    lbl_Vision1Count.Visible = false;
                    btn_ResetVision1.Visible = false;
                }
            }
            else
            {
                lbl_Vision1Name.Visible = false;
                lbl_Vision1Count.Visible = false;
                btn_ResetVision1.Visible = false;
            }

            if (m_arrVisionIndex.Length > 1)
            {
                if (m_smVSInfo[m_arrVisionIndex[1]] != null)
                {
                    lbl_Vision2Name.Text = m_smVSInfo[m_arrVisionIndex[1]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[1]].g_strVisionNameRemark + "Tested Unit";
                    lbl_Vision2Count.Text = m_smVSInfo[m_arrVisionIndex[1]].g_intTestedTotal.ToString();
                }
                else
                {
                    lbl_Vision2Name.Visible = false;
                    lbl_Vision2Count.Visible = false;
                    btn_ResetVision2.Visible = false;
                }
            }
            else
            {
                lbl_Vision2Name.Visible = false;
                lbl_Vision2Count.Visible = false;
                btn_ResetVision2.Visible = false;
            }

            if (m_arrVisionIndex.Length > 2)
            {
                if (m_smVSInfo[m_arrVisionIndex[2]] != null)
                {
                    lbl_Vision3Name.Text = m_smVSInfo[m_arrVisionIndex[2]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[2]].g_strVisionNameRemark + "Tested Unit";
                    lbl_Vision3Count.Text = m_smVSInfo[m_arrVisionIndex[2]].g_intTestedTotal.ToString();
                }
                else
                {
                    lbl_Vision3Name.Visible = false;
                    lbl_Vision3Count.Visible = false;
                    btn_ResetVision3.Visible = false;
                }
            }
            else
            {
                lbl_Vision3Name.Visible = false;
                lbl_Vision3Count.Visible = false;
                btn_ResetVision3.Visible = false;
            }

            if (m_arrVisionIndex.Length > 3)
            {
                if (m_smVSInfo[m_arrVisionIndex[3]] != null)
                {
                    lbl_Vision4Name.Text = m_smVSInfo[m_arrVisionIndex[3]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[3]].g_strVisionNameRemark + "Tested Unit";
                    lbl_Vision4Count.Text = m_smVSInfo[m_arrVisionIndex[3]].g_intTestedTotal.ToString();
                }
                else
                {
                    lbl_Vision4Name.Visible = false;
                    lbl_Vision4Count.Visible = false;
                    btn_ResetVision4.Visible = false;
                }
            }
            else
            {
                lbl_Vision4Name.Visible = false;
                lbl_Vision4Count.Visible = false;
                btn_ResetVision4.Visible = false;
            }

            if (m_arrVisionIndex.Length > 4)
            {
                if (m_smVSInfo[m_arrVisionIndex[4]] != null)
                {
                    lbl_Vision5Name.Text = m_smVSInfo[m_arrVisionIndex[4]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[4]].g_strVisionNameRemark + "Tested Unit";
                    lbl_Vision5Count.Text = m_smVSInfo[m_arrVisionIndex[4]].g_intTestedTotal.ToString();
                }
                else
                {
                    lbl_Vision5Name.Visible = false;
                    lbl_Vision5Count.Visible = false;
                    btn_ResetVision5.Visible = false;
                }
            }
            else
            {
                lbl_Vision5Name.Visible = false;
                lbl_Vision5Count.Visible = false;
                btn_ResetVision5.Visible = false;
            }

            if (m_arrVisionIndex.Length > 5)
            {
                if (m_smVSInfo[m_arrVisionIndex[5]] != null)
                {
                    lbl_Vision6Name.Text = m_smVSInfo[m_arrVisionIndex[5]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[5]].g_strVisionNameRemark + "Tested Unit";
                    lbl_Vision6Count.Text = m_smVSInfo[m_arrVisionIndex[5]].g_intTestedTotal.ToString();
                }
                else
                {
                    lbl_Vision6Name.Visible = false;
                    lbl_Vision6Count.Visible = false;
                    btn_ResetVision6.Visible = false;
                }
            }
            else
            {
                lbl_Vision6Name.Visible = false;
                lbl_Vision6Count.Visible = false;
                btn_ResetVision6.Visible = false;
            }

            if (m_arrVisionIndex.Length > 6)
            {
                if (m_smVSInfo[m_arrVisionIndex[6]] != null)
                {
                    lbl_Vision7Name.Text = m_smVSInfo[m_arrVisionIndex[6]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[6]].g_strVisionNameRemark + "Tested Unit";
                    lbl_Vision7Count.Text = m_smVSInfo[m_arrVisionIndex[6]].g_intTestedTotal.ToString();
                }
                else
                {
                    lbl_Vision7Name.Visible = false;
                    lbl_Vision7Count.Visible = false;
                    btn_ResetVision7.Visible = false;
                }
            }
            else
            {
                lbl_Vision7Name.Visible = false;
                lbl_Vision7Count.Visible = false;
                btn_ResetVision7.Visible = false;
            }

            if (m_arrVisionIndex.Length > 7)
            {
                if (m_smVSInfo[m_arrVisionIndex[7]] != null)
                {
                    lbl_Vision8Name.Text = m_smVSInfo[m_arrVisionIndex[7]].g_strVisionDisplayName + " " + m_smVSInfo[m_arrVisionIndex[7]].g_strVisionNameRemark + "Tested Unit";
                    lbl_Vision8Count.Text = m_smVSInfo[m_arrVisionIndex[7]].g_intTestedTotal.ToString();
                }
                else
                {
                    lbl_Vision8Name.Visible = false;
                    lbl_Vision8Count.Visible = false;
                    btn_ResetVision8.Visible = false;
                }
            }
            else
            {
                lbl_Vision8Name.Visible = false;
                lbl_Vision8Count.Visible = false;
                btn_ResetVision8.Visible = false;
            }
        }

        private void btn_ResetVision1_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[0]].g_intTestedTotal > 0)
            {
                m_smVSInfo[m_arrVisionIndex[0]].g_blnResetCount = true;
                m_smVSInfo[m_arrVisionIndex[0]].g_blnResetGUIDisplayCount = true;
                
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[0].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision1Count.Text, "0", m_smProductionInfo.g_strLotID);
                
                lbl_Vision1Count.Text = "0";

                m_smVSInfo[m_arrVisionIndex[0]].g_intVisionResetCount++;
                m_smVSInfo[m_arrVisionIndex[0]].g_strVisionResetCountTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        private void btn_ResetVision2_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[1]].g_intTestedTotal > 0)
            {
                m_smVSInfo[m_arrVisionIndex[1]].g_blnResetCount = true;
                m_smVSInfo[m_arrVisionIndex[1]].g_blnResetGUIDisplayCount = true;
                
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[1].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision2Count.Text, "0", m_smProductionInfo.g_strLotID);
                
                lbl_Vision2Count.Text = "0";

                m_smVSInfo[m_arrVisionIndex[1]].g_intVisionResetCount++;
                m_smVSInfo[m_arrVisionIndex[1]].g_strVisionResetCountTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        private void btn_ResetVision3_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[2]].g_intTestedTotal > 0)
            {
                m_smVSInfo[m_arrVisionIndex[2]].g_blnResetCount = true;
                m_smVSInfo[m_arrVisionIndex[2]].g_blnResetGUIDisplayCount = true;
                
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[2].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision3Count.Text, "0", m_smProductionInfo.g_strLotID);
                
                lbl_Vision3Count.Text = "0";

                m_smVSInfo[m_arrVisionIndex[2]].g_intVisionResetCount++;
                m_smVSInfo[m_arrVisionIndex[2]].g_strVisionResetCountTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        private void btn_ResetVision4_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[3]].g_intTestedTotal > 0)
            {
                m_smVSInfo[m_arrVisionIndex[3]].g_blnResetCount = true;
                m_smVSInfo[m_arrVisionIndex[3]].g_blnResetGUIDisplayCount = true;
                
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[3].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision4Count.Text, "0", m_smProductionInfo.g_strLotID);
                
                lbl_Vision4Count.Text = "0";

                m_smVSInfo[m_arrVisionIndex[3]].g_intVisionResetCount++;
                m_smVSInfo[m_arrVisionIndex[3]].g_strVisionResetCountTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        private void btn_ResetVision5_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[4]].g_intTestedTotal > 0)
            {
                m_smVSInfo[m_arrVisionIndex[4]].g_blnResetCount = true;
                m_smVSInfo[m_arrVisionIndex[4]].g_blnResetGUIDisplayCount = true;
                
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[4].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision5Count.Text, "0", m_smProductionInfo.g_strLotID);
                
                lbl_Vision5Count.Text = "0";

                m_smVSInfo[m_arrVisionIndex[4]].g_intVisionResetCount++;
                m_smVSInfo[m_arrVisionIndex[4]].g_strVisionResetCountTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        private void btn_ResetVision6_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[5]].g_intTestedTotal > 0)
            {
                m_smVSInfo[m_arrVisionIndex[5]].g_blnResetCount = true;
                m_smVSInfo[m_arrVisionIndex[5]].g_blnResetGUIDisplayCount = true;
                
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[5].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision6Count.Text, "0", m_smProductionInfo.g_strLotID);
                
                lbl_Vision6Count.Text = "0";

                m_smVSInfo[m_arrVisionIndex[5]].g_intVisionResetCount++;
                m_smVSInfo[m_arrVisionIndex[5]].g_strVisionResetCountTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        private void btn_ResetVision7_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[6]].g_intTestedTotal > 0)
            {
                m_smVSInfo[m_arrVisionIndex[6]].g_blnResetCount = true;
                m_smVSInfo[m_arrVisionIndex[6]].g_blnResetGUIDisplayCount = true;
                
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[6].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision7Count.Text, "0", m_smProductionInfo.g_strLotID);
                
                lbl_Vision7Count.Text = "0";

                m_smVSInfo[m_arrVisionIndex[6]].g_intVisionResetCount++;
                m_smVSInfo[m_arrVisionIndex[6]].g_strVisionResetCountTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        private void btn_ResetVision8_Click(object sender, EventArgs e)
        {
            if (m_smVSInfo[m_arrVisionIndex[7]].g_intTestedTotal > 0)
            {
                m_smVSInfo[m_arrVisionIndex[7]].g_blnResetCount = true;
                m_smVSInfo[m_arrVisionIndex[7]].g_blnResetGUIDisplayCount = true;
                
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[7].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision8Count.Text, "0", m_smProductionInfo.g_strLotID);
                
                lbl_Vision8Count.Text = "0";

                m_smVSInfo[m_arrVisionIndex[7]].g_intVisionResetCount++;
                m_smVSInfo[m_arrVisionIndex[7]].g_strVisionResetCountTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        private void btn_ResetAll_Click(object sender, EventArgs e)
        {
            //for (int i = 0; i < m_arrVisionIndex.Length; i ++)
            //{
            //    m_smVSInfo[m_arrVisionIndex[i]].g_blnResetCount = true;
            //    m_smVSInfo[m_arrVisionIndex[i]].g_blnResetGUIDisplayCount = true;
            //}

            // 2019-09-26 ZJYEOH : Need to take care for less than 4 Vision module
            for (int i = 0; i < m_smVSInfo.Length; i++)
            {
                if (m_smVSInfo[i] == null)
                    continue;

                if (m_smVSInfo[m_arrVisionIndex[i]].g_intTestedTotal > 0)
                {
                    m_smVSInfo[m_arrVisionIndex[i]].g_blnResetCount = true;
                    m_smVSInfo[m_arrVisionIndex[i]].g_blnResetGUIDisplayCount = true;

                    m_smVSInfo[m_arrVisionIndex[i]].g_intVisionResetCount++;
                    m_smVSInfo[m_arrVisionIndex[i]].g_strVisionResetCountTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                }
            }
            
            if (m_arrVisionIndex.Length > 0 && m_smVSInfo[0] != null)
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[0].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision1Count.Text, "0", m_smProductionInfo.g_strLotID);
            lbl_Vision1Count.Text = "0";
            if (m_arrVisionIndex.Length > 1 && m_smVSInfo[1] != null)
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[1].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision2Count.Text, "0", m_smProductionInfo.g_strLotID);
            lbl_Vision2Count.Text = "0";
            if (m_arrVisionIndex.Length > 2 && m_smVSInfo[2] != null)
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[2].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision3Count.Text, "0", m_smProductionInfo.g_strLotID);
            lbl_Vision3Count.Text = "0";
            if (m_arrVisionIndex.Length > 3 && m_smVSInfo[3] != null)
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[3].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision4Count.Text, "0", m_smProductionInfo.g_strLotID);
            lbl_Vision4Count.Text = "0";
            if (m_arrVisionIndex.Length > 4 && m_smVSInfo[4] != null)
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[4].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision5Count.Text, "0", m_smProductionInfo.g_strLotID);
            lbl_Vision5Count.Text = "0";
            if (m_arrVisionIndex.Length > 5 && m_smVSInfo[5] != null)
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[5].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision6Count.Text, "0", m_smProductionInfo.g_strLotID);
            lbl_Vision6Count.Text = "0";
            if (m_arrVisionIndex.Length > 6 && m_smVSInfo[6] != null)
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[6].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision7Count.Text, "0", m_smProductionInfo.g_strLotID);
            lbl_Vision7Count.Text = "0";
            if (m_arrVisionIndex.Length > 7 && m_smVSInfo[7] != null)
                STDeviceEdit.SaveDeviceEditLog(m_smVSInfo[7].g_strVisionFolderName + ">Reset Vision Count", "Counter value", lbl_Vision8Count.Text, "0", m_smProductionInfo.g_strLotID);
            lbl_Vision8Count.Text = "0";
            
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_smProductionInfo.AT_ALL_InAuto && m_smProductionInfo.g_intAutoLogOutMinutes > 0 && m_intUserGroup != 5)
            {
                DateTime t = DateTime.Now;
                TimeSpan tSpan = t - m_smProductionInfo.g_DTStartAutoMode_IndividualForm;

                if (tSpan.Minutes >= m_smProductionInfo.g_intAutoLogOutMinutes)
                {
                    m_smProductionInfo.g_intUserGroup = 5;
                    m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
                    DisableField();
                }
            }
        }

        private void ResetCountForm_Load(object sender, EventArgs e)
        {
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            timer1.Start();
        }
    }
}