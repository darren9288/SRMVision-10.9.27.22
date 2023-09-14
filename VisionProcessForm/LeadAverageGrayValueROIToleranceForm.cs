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
using VisionProcessing;
using SharedMemory;
using Microsoft.Win32;
using System.IO;

namespace VisionProcessForm
{
    public partial class LeadAverageGrayValueROIToleranceForm : Form
    {
        #region Member Variables
        private int m_intCurrentSelectedTextBox = 1; // 1 = Top; 2 = Right; 3 = Bottom; 4 = Left
        private bool m_blnTriggerOfflineTest = false;
        private Point m_pTop, m_pRight, m_pBottom, m_pLeft;
        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;

        private string m_strSelectedRecipe;

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        
        #endregion

        public LeadAverageGrayValueROIToleranceForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
       
            m_smVisionInfo.g_intViewInspectionSetting = 1;
         
            m_pTop = new Point(pnl_Top.Location.X, pnl_Top.Location.Y);
            m_pRight = new Point(pnl_Right.Location.X, pnl_Right.Location.Y);
            m_pBottom = new Point(pnl_Bottom.Location.X, pnl_Bottom.Location.Y);
            m_pLeft = new Point(pnl_Left.Location.X, pnl_Left.Location.Y);
            
            cbo_ROI.Items.Clear();
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                    continue;

                if (i == 1)
                    cbo_ROI.Items.Add("Top");
                else if (i == 2)
                    cbo_ROI.Items.Add("Right");
                else if (i == 3)
                    cbo_ROI.Items.Add("Bottom");
                else if (i == 4)
                    cbo_ROI.Items.Add("Left");


            }
            if (cbo_ROI.Items.Count > 0)
                cbo_ROI.SelectedIndex = 0;

            UpdateComboBox();
            UpdateGUIPic();

            UpdateGUI();

            m_blnInitDone = true;

            m_blnTriggerOfflineTest = true;
        }
        private void UpdateGUI()
        {

            int intTolTop = 0, intTolRight = 0, intTolBottom = 0, intTolLeft = 0;

            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, ref intTolTop, ref intTolRight, ref intTolBottom, ref intTolLeft);

            switch (cbo_ROI.SelectedItem.ToString())
            {
                case "Top": //Top
                    txt_StartPixelFromTop.Text = intTolTop.ToString();
                    txt_StartPixelFromRight.Text = intTolRight.ToString();
                    txt_StartPixelFromBottom.Text = intTolBottom.ToString();
                    txt_StartPixelFromLeft.Text = intTolLeft.ToString();
                    break;
                case "Bottom": //Bottom
                    txt_StartPixelFromTop.Text = intTolBottom.ToString();
                    txt_StartPixelFromRight.Text = intTolLeft.ToString();
                    txt_StartPixelFromBottom.Text = intTolTop.ToString();
                    txt_StartPixelFromLeft.Text = intTolRight.ToString();
                    break;
                case "Left": //Left
                    txt_StartPixelFromTop.Text = intTolLeft.ToString();
                    txt_StartPixelFromRight.Text = intTolTop.ToString();
                    txt_StartPixelFromBottom.Text = intTolRight.ToString();
                    txt_StartPixelFromLeft.Text = intTolBottom.ToString();
                    break;
                case "Right": //Right
                    txt_StartPixelFromTop.Text = intTolRight.ToString();
                    txt_StartPixelFromRight.Text = intTolBottom.ToString();
                    txt_StartPixelFromBottom.Text = intTolLeft.ToString();
                    txt_StartPixelFromLeft.Text = intTolTop.ToString();
                    break;
            }

        }

        private void UpdateGUIPic()
        {
            if (cbo_LeadNo.Items.Count == 0)
                return;
            
            switch (cbo_ROI.SelectedItem.ToString())
            {
                case "Top": // Top
                    pic_Lead.Image = imageList1.Images[0];
                    pnl_Top.Location = m_pTop;
                    pnl_Right.Location = m_pRight;
                    pnl_Bottom.Location = m_pBottom;
                    pnl_Left.Location = m_pLeft;
                    break;
                case "Bottom": // Bottom
                    pic_Lead.Image = imageList1.Images[1];
                    pnl_Top.Location = m_pBottom;
                    pnl_Right.Location = m_pLeft;
                    pnl_Bottom.Location = m_pTop;
                    pnl_Left.Location = m_pRight;
                    break;
                case "Left": // Left
                    pic_Lead.Image = imageList1.Images[2];
                    pnl_Top.Location = m_pLeft;
                    pnl_Right.Location = m_pTop;
                    pnl_Bottom.Location = m_pRight;
                    pnl_Left.Location = m_pBottom;
                    break;
                case "Right": // Right
                    pic_Lead.Image = imageList1.Images[3];
                    pnl_Top.Location = m_pRight;
                    pnl_Right.Location = m_pBottom;
                    pnl_Bottom.Location = m_pLeft;
                    pnl_Left.Location = m_pTop;
                    break;
            }

            //m_smVisionInfo.g_blnViewLeadAGVROIDrawing = true;
            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo.SelectedIndex;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_LeadNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            m_blnInitDone = false;
            UpdateGUIPic();

            UpdateGUI();
            m_blnInitDone = true;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            SaveLeadSettings(strFolderPath + "Lead\\");

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }
        private void SaveLeadSettings(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                string strSectionName = "";
                if (i == 0)
                    strSectionName = "SearchROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                
                m_smVisionInfo.g_arrLead[i].SaveLead(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                
            }
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead Template", m_smProductionInfo.g_strLotID);
            
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadLeadSetting(strFolderPath + "Lead\\");

            this.Close();
            this.Dispose();
        }
        private void LoadLeadSetting(string strFolderPath)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                m_smVisionInfo.g_arrLead[i].SetCalibrationData(
                              m_smVisionInfo.g_fCalibPixelX,
                              m_smVisionInfo.g_fCalibPixelY,
                              m_smVisionInfo.g_fCalibOffSetX,
                              m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);

                // Load Lead Template Setting
                string strSectionName = "";
                if (i == 0)
                    strSectionName = "SearchROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                m_smVisionInfo.g_arrLead[i].LoadLead(strFolderPath + "Template\\Template.xml", strSectionName, m_smVisionInfo.g_arrImages.Count);

                m_smVisionInfo.g_arrLead[i].LoadLeadTemplateImage(strFolderPath + "Template\\", i);

            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_blnTriggerOfflineTest)
            {
                m_blnTriggerOfflineTest = false;
                m_smVisionInfo.AT_VM_OfflineTestAllLead = true;
                TriggerOfflineTest();
                m_smVisionInfo.g_blnViewLeadAGVROIDrawing = true;
                m_smVisionInfo.g_blnViewRotatedImage = true;
            }
        }

        private void LeadAverageGrayValueROIToleranceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.AT_VM_OfflineTestAllLead = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_intViewInspectionSetting = 0;
            m_smVisionInfo.g_blnViewLeadAGVROIDrawing = false;
            m_smVisionInfo.g_blnViewAllLeadNumber = false;
            m_smVisionInfo.g_intLeadSelectedNumber = -1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_ROI_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (!m_blnInitDone)
            //    return;
            switch (cbo_ROI.SelectedItem.ToString())
            {
                case "Top":
                    m_smVisionInfo.g_intSelectedROI = 1;
                    break;
                case "Right":
                    m_smVisionInfo.g_intSelectedROI = 2;
                    break;
                case "Bottom":
                    m_smVisionInfo.g_intSelectedROI = 3;
                    break;
                case "Left":
                    m_smVisionInfo.g_intSelectedROI = 4;
                    break;
            }
            UpdateComboBox();
        }

        private void UpdateComboBox()
        {
            cbo_LeadNo.Items.Clear();
            if (m_smVisionInfo.g_arrLead.Length > 0)
            {
                if (m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetBlobsFeaturesNumber() > 0)
                {
                    if (cbo_LeadNo.Items.Count == 0)
                    {
                        List<int> intTotalLead = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetLeadID();
                        for (int p = 0; p < intTotalLead.Count; p++)
                        {
                            cbo_LeadNo.Items.Add((intTotalLead[p]).ToString());
                        }
                    }
                }
            }
            if (cbo_LeadNo.Items.Count > 0)
                cbo_LeadNo.SelectedIndex = 0;
        }
        private void txt_StartPixel_Enter(object sender, EventArgs e)
        {
            switch (((TextBox)sender).Name.ToString())
            {
                case "txt_StartPixelFromTop":
                    m_intCurrentSelectedTextBox = 1;
                    break;
                case "txt_StartPixelFromRight":
                    m_intCurrentSelectedTextBox = 2;
                    break;
                case "txt_StartPixelFromBottom":
                    m_intCurrentSelectedTextBox = 3;
                    break;
                case "txt_StartPixelFromLeft":
                    m_intCurrentSelectedTextBox = 4;
                    break;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetToAllLead_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewAllLeadNumber = chk_SetToAllLead.Checked;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void TriggerOfflineTest()
        {
            // 29-07-2019 ZJYEOH : If user pressed test button, then Image should stop live
            if (m_smVisionInfo.AT_PR_StartLiveImage)
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                    m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            }

            m_smVisionInfo.AT_VM_ManualTestMode = true;

            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
        }

        private void txt_StartPixelFromTop_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intValue = Convert.ToInt32(txt_StartPixelFromTop.Text);
            int intTolTop = 0, intTolRight = 0, intTolBottom = 0, intTolLeft = 0;
            bool blnResult = true;

            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, ref intTolTop, ref intTolRight, ref intTolBottom, ref intTolLeft);
            switch (m_smVisionInfo.g_intSelectedROI)
            {
                case 1:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolBottom) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolTop.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolTop.ToString();
                            txt_StartPixelFromRight.Text = intTolRight.ToString();
                            txt_StartPixelFromBottom.Text = intTolBottom.ToString();
                            txt_StartPixelFromLeft.Text = intTolLeft.ToString();
                            blnResult = false;
                        }
                    }
                    break;
                case 3:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolTop) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolBottom.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolBottom.ToString();
                            txt_StartPixelFromRight.Text = intTolLeft.ToString();
                            txt_StartPixelFromBottom.Text = intTolTop.ToString();
                            txt_StartPixelFromLeft.Text = intTolRight.ToString();
                            blnResult = false;
                        }
                    }
                    break;
                case 4:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolRight) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolLeft.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolLeft.ToString();
                            txt_StartPixelFromRight.Text = intTolTop.ToString();
                            txt_StartPixelFromBottom.Text = intTolRight.ToString();
                            txt_StartPixelFromLeft.Text = intTolBottom.ToString();
                            blnResult = false;
                        }
                    }
                    break;
                case 2:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolLeft) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolRight.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolRight.ToString();
                            txt_StartPixelFromRight.Text = intTolBottom.ToString();
                            txt_StartPixelFromBottom.Text = intTolLeft.ToString();
                            txt_StartPixelFromLeft.Text = intTolTop.ToString();
                            blnResult = false;
                        }
                    }
                    break;
            }

            if (blnResult)
            {
                if (!chk_SetToAllEdge.Checked)
                {
                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 1, intValue);
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 3, intValue);
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 4, intValue);
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 2, intValue);
                            break;
                    }
                }
                else
                {
                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 1, intValue);
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 3, intValue);
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 4, intValue);
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 2, intValue);
                            break;
                    }
                    if (m_intCurrentSelectedTextBox == 1)
                    {
                        //txt_StartPixelFromTop.Text = intValue.ToString();
                        txt_StartPixelFromRight.Text = intValue.ToString();
                        txt_StartPixelFromBottom.Text = intValue.ToString();
                        txt_StartPixelFromLeft.Text = intValue.ToString();
                    }
                }
            }

            if (chk_SetToAllLead.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                        continue;

                    for (int j = 0; j < m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead; j++)
                    {
                        if (j == cbo_LeadNo.SelectedIndex && i == m_smVisionInfo.g_intSelectedROI)
                            continue;

                        blnResult = true;
                        m_smVisionInfo.g_arrLead[i].GetAGVROIToleranceValue(j, ref intTolTop, ref intTolRight, ref intTolBottom, ref intTolLeft);
                        switch (i)
                        {
                            case 1:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolBottom) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolTop.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolTop.ToString();
                                        //txt_StartPixelFromRight.Text = intTolRight.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolBottom.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolLeft.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                            case 3:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolTop) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolBottom.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolBottom.ToString();
                                        //txt_StartPixelFromRight.Text = intTolLeft.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolTop.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolRight.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                            case 4:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolRight) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolLeft.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolLeft.ToString();
                                        //txt_StartPixelFromRight.Text = intTolTop.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolRight.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolBottom.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                            case 2:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolLeft) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolRight.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolRight.ToString();
                                        //txt_StartPixelFromRight.Text = intTolBottom.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolLeft.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolTop.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                        }

                        if (blnResult)
                        {
                            if (!chk_SetToAllEdge.Checked)
                            {
                                switch (i)
                                {
                                    case 1: // Top
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 1, intValue);
                                        break;
                                    case 3: // Bottom
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 3, intValue);
                                        break;
                                    case 4: // Left
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 4, intValue);
                                        break;
                                    case 2: // Right
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 2, intValue);
                                        break;
                                }
                            }
                            else
                            {
                                switch (i)
                                {
                                    case 1: // Top
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 1, intValue);
                                        break;
                                    case 3: // Bottom
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 3, intValue);
                                        break;
                                    case 4: // Left
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 4, intValue);
                                        break;
                                    case 2: // Right
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 2, intValue);
                                        break;
                                }
                                ////txt_StartPixelFromTop.Text = intValue.ToString();
                                //txt_StartPixelFromRight.Text = intValue.ToString();
                                //txt_StartPixelFromBottom.Text = intValue.ToString();
                                //txt_StartPixelFromLeft.Text = intValue.ToString();
                            }
                        }
                    }
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intValue = Convert.ToInt32(txt_StartPixelFromRight.Text);
            int intTolTop = 0, intTolRight = 0, intTolBottom = 0, intTolLeft = 0;
            bool blnResult = true;
            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, ref intTolTop, ref intTolRight, ref intTolBottom, ref intTolLeft);
            switch (m_smVisionInfo.g_intSelectedROI)
            {
                case 1:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolLeft) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromRight.Text = intTolRight.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolTop.ToString();
                            txt_StartPixelFromRight.Text = intTolRight.ToString();
                            txt_StartPixelFromBottom.Text = intTolBottom.ToString();
                            txt_StartPixelFromLeft.Text = intTolLeft.ToString();
                            blnResult = false;
                        }
                    }
                    break;
                case 3:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolRight) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromRight.Text = intTolLeft.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolBottom.ToString();
                            txt_StartPixelFromRight.Text = intTolLeft.ToString();
                            txt_StartPixelFromBottom.Text = intTolTop.ToString();
                            txt_StartPixelFromLeft.Text = intTolRight.ToString();
                            blnResult = false;
                        }
                    }
                    break;
                case 4:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolBottom) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromRight.Text = intTolTop.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolLeft.ToString();
                            txt_StartPixelFromRight.Text = intTolTop.ToString();
                            txt_StartPixelFromBottom.Text = intTolRight.ToString();
                            txt_StartPixelFromLeft.Text = intTolBottom.ToString();
                            blnResult = false;
                        }
                    }
                    break;
                case 2:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolTop) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromRight.Text = intTolBottom.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolRight.ToString();
                            txt_StartPixelFromRight.Text = intTolBottom.ToString();
                            txt_StartPixelFromBottom.Text = intTolLeft.ToString();
                            txt_StartPixelFromLeft.Text = intTolTop.ToString();
                            blnResult = false;
                        }
                    }
                    break;
            }

            if (blnResult)
            {
                if (!chk_SetToAllEdge.Checked)
                {
                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 2, intValue);
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 4, intValue);
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 1, intValue);
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 3, intValue);
                            break;
                    }
                }
                else
                {
                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 2, intValue);
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 4, intValue);
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 1, intValue);
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 3, intValue);
                            break;
                    }
                    if (m_intCurrentSelectedTextBox == 2)
                    {
                        txt_StartPixelFromTop.Text = intValue.ToString();
                        //txt_StartPixelFromRight.Text = intValue.ToString();
                        txt_StartPixelFromBottom.Text = intValue.ToString();
                        txt_StartPixelFromLeft.Text = intValue.ToString();
                    }
                }
            }

            if (chk_SetToAllLead.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                        continue;

                    for (int j = 0; j < m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead; j++)
                    {
                        if (j == cbo_LeadNo.SelectedIndex && i == m_smVisionInfo.g_intSelectedROI)
                            continue;

                        blnResult = true;
                        m_smVisionInfo.g_arrLead[i].GetAGVROIToleranceValue(j, ref intTolTop, ref intTolRight, ref intTolBottom, ref intTolLeft);
                        switch (i)
                        {
                            case 1:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolLeft) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromRight.Text = intTolRight.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolTop.ToString();
                                        //txt_StartPixelFromRight.Text = intTolRight.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolBottom.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolLeft.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                            case 3:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolRight) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromRight.Text = intTolLeft.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolBottom.ToString();
                                        //txt_StartPixelFromRight.Text = intTolLeft.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolTop.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolRight.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                            case 4:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolBottom) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromRight.Text = intTolTop.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolLeft.ToString();
                                        //txt_StartPixelFromRight.Text = intTolTop.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolRight.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolBottom.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                            case 2:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolTop) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromRight.Text = intTolBottom.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolRight.ToString();
                                        //txt_StartPixelFromRight.Text = intTolBottom.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolLeft.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolTop.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                        }

                        if (blnResult)
                        {
                            if (!chk_SetToAllEdge.Checked)
                            {
                                switch (i)
                                {
                                    case 1: // Top
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 2, intValue);
                                        break;
                                    case 3: // Bottom
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 4, intValue);
                                        break;
                                    case 4: // Left
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 1, intValue);
                                        break;
                                    case 2: // Right
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 3, intValue);
                                        break;
                                }
                            }
                            else
                            {
                                switch (i)
                                {
                                    case 1: // Top
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 2, intValue);
                                        break;
                                    case 3: // Bottom
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 4, intValue);
                                        break;
                                    case 4: // Left
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 1, intValue);
                                        break;
                                    case 2: // Right
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 3, intValue);
                                        break;
                                }
                                //txt_StartPixelFromTop.Text = intValue.ToString();
                                ////txt_StartPixelFromRight.Text = intValue.ToString();
                                //txt_StartPixelFromBottom.Text = intValue.ToString();
                                //txt_StartPixelFromLeft.Text = intValue.ToString();
                            }
                        }
                    }
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intValue = Convert.ToInt32(txt_StartPixelFromBottom.Text);
            int intTolTop = 0, intTolRight = 0, intTolBottom = 0, intTolLeft = 0;
            bool blnResult = true;
            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, ref intTolTop, ref intTolRight, ref intTolBottom, ref intTolLeft);
            switch (m_smVisionInfo.g_intSelectedROI)
            {
                case 1:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolTop) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromBottom.Text = intTolBottom.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolTop.ToString();
                            txt_StartPixelFromRight.Text = intTolRight.ToString();
                            txt_StartPixelFromBottom.Text = intTolBottom.ToString();
                            txt_StartPixelFromLeft.Text = intTolLeft.ToString();
                            blnResult = false;
                        }
                    }
                    break;
                case 3:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolBottom) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromBottom.Text = intTolTop.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolBottom.ToString();
                            txt_StartPixelFromRight.Text = intTolLeft.ToString();
                            txt_StartPixelFromBottom.Text = intTolTop.ToString();
                            txt_StartPixelFromLeft.Text = intTolRight.ToString();
                            blnResult = false;
                        }
                    }
                    break;
                case 4:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolLeft) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromBottom.Text = intTolRight.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolLeft.ToString();
                            txt_StartPixelFromRight.Text = intTolTop.ToString();
                            txt_StartPixelFromBottom.Text = intTolRight.ToString();
                            txt_StartPixelFromLeft.Text = intTolBottom.ToString();
                            blnResult = false;
                        }
                    }
                    break;
                case 2:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolRight) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromBottom.Text = intTolLeft.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolRight.ToString();
                            txt_StartPixelFromRight.Text = intTolBottom.ToString();
                            txt_StartPixelFromBottom.Text = intTolLeft.ToString();
                            txt_StartPixelFromLeft.Text = intTolTop.ToString();
                            blnResult = false;
                        }
                    }
                    break;
            }

            if (blnResult)
            {
                if (!chk_SetToAllEdge.Checked)
                {
                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 3, intValue);
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 1, intValue);
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 2, intValue);
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 4, intValue);
                            break;
                    }
                }
                else
                {
                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 3, intValue);
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 1, intValue);
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 2, intValue);
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 4, intValue);
                            break;
                    }
                    if (m_intCurrentSelectedTextBox == 3)
                    {
                        txt_StartPixelFromTop.Text = intValue.ToString();
                        txt_StartPixelFromRight.Text = intValue.ToString();
                        //txt_StartPixelFromBottom.Text = intValue.ToString();
                        txt_StartPixelFromLeft.Text = intValue.ToString();
                    }
                }
            }

            if (chk_SetToAllLead.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                        continue;

                    for (int j = 0; j < m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead; j++)
                    {
                        if (j == cbo_LeadNo.SelectedIndex && i == m_smVisionInfo.g_intSelectedROI)
                            continue;
                        blnResult = true;
                        m_smVisionInfo.g_arrLead[i].GetAGVROIToleranceValue(j, ref intTolTop, ref intTolRight, ref intTolBottom, ref intTolLeft);
                        switch (i)
                        {
                            case 1:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolTop) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromBottom.Text = intTolBottom.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolTop.ToString();
                                        //txt_StartPixelFromRight.Text = intTolRight.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolBottom.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolLeft.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                            case 3:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolBottom) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromBottom.Text = intTolTop.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolBottom.ToString();
                                        //txt_StartPixelFromRight.Text = intTolLeft.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolTop.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolRight.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                            case 4:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolLeft) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromBottom.Text = intTolRight.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolLeft.ToString();
                                        //txt_StartPixelFromRight.Text = intTolTop.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolRight.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolBottom.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                            case 2:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolRight) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromBottom.Text = intTolLeft.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolRight.ToString();
                                        //txt_StartPixelFromRight.Text = intTolBottom.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolLeft.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolTop.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                        }

                        if (blnResult)
                        {
                            if (!chk_SetToAllEdge.Checked)
                            {
                                switch (i)
                                {
                                    case 1: // Top
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 3, intValue);
                                        break;
                                    case 3: // Bottom
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 1, intValue);
                                        break;
                                    case 4: // Left
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 2, intValue);
                                        break;
                                    case 2: // Right
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 4, intValue);
                                        break;
                                }
                            }
                            else
                            {
                                switch (i)
                                {
                                    case 1: // Top
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 3, intValue);
                                        break;
                                    case 3: // Bottom
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 1, intValue);
                                        break;
                                    case 4: // Left
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 2, intValue);
                                        break;
                                    case 2: // Right
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 4, intValue);
                                        break;
                                }
                                //txt_StartPixelFromTop.Text = intValue.ToString();
                                //txt_StartPixelFromRight.Text = intValue.ToString();
                                ////txt_StartPixelFromBottom.Text = intValue.ToString();
                                //txt_StartPixelFromLeft.Text = intValue.ToString();
                            }
                        }
                    }
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intValue = Convert.ToInt32(txt_StartPixelFromLeft.Text);
            int intTolTop = 0, intTolRight = 0, intTolBottom = 0, intTolLeft = 0;
            bool blnResult = true;
            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, ref intTolTop, ref intTolRight, ref intTolBottom, ref intTolLeft);
            switch (m_smVisionInfo.g_intSelectedROI)
            {
                case 1:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolRight) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromLeft.Text = intTolLeft.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolTop.ToString();
                            txt_StartPixelFromRight.Text = intTolRight.ToString();
                            txt_StartPixelFromBottom.Text = intTolBottom.ToString();
                            txt_StartPixelFromLeft.Text = intTolLeft.ToString();
                            blnResult = false;
                        }
                    }
                    break;
                case 3:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolLeft) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromLeft.Text = intTolRight.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolBottom.ToString();
                            txt_StartPixelFromRight.Text = intTolLeft.ToString();
                            txt_StartPixelFromBottom.Text = intTolTop.ToString();
                            txt_StartPixelFromLeft.Text = intTolRight.ToString();
                            blnResult = false;
                        }
                    }
                    break;
                case 4:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolTop) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromLeft.Text = intTolBottom.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolLeft.ToString();
                            txt_StartPixelFromRight.Text = intTolTop.ToString();
                            txt_StartPixelFromBottom.Text = intTolRight.ToString();
                            txt_StartPixelFromLeft.Text = intTolBottom.ToString();
                            blnResult = false;
                        }
                    }
                    break;
                case 2:
                    if (!chk_SetToAllEdge.Checked)
                    {
                        if ((intValue + intTolBottom) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromLeft.Text = intTolTop.ToString();
                            blnResult = false;
                        }
                    }
                    else
                    {
                        if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipLength(cbo_LeadNo.SelectedIndex)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetSampleLeadTipWidth(cbo_LeadNo.SelectedIndex)))
                        {
                            SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (cbo_LeadNo.SelectedIndex + 1).ToString(), "Wrong Setting");
                            txt_StartPixelFromTop.Text = intTolRight.ToString();
                            txt_StartPixelFromRight.Text = intTolBottom.ToString();
                            txt_StartPixelFromBottom.Text = intTolLeft.ToString();
                            txt_StartPixelFromLeft.Text = intTolTop.ToString();
                            blnResult = false;
                        }
                    }
                    break;
            }

            if (blnResult)
            {
                if (!chk_SetToAllEdge.Checked)
                {
                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 4, intValue);
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 2, intValue);
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 3, intValue);
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 1, intValue);
                            break;
                    }
                }
                else
                {
                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 4, intValue);
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 2, intValue);
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 3, intValue);
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetAGVROIToleranceValue(cbo_LeadNo.SelectedIndex, 1, intValue);
                            break;
                    }
                    if (m_intCurrentSelectedTextBox == 4)
                    {
                        txt_StartPixelFromTop.Text = intValue.ToString();
                        txt_StartPixelFromRight.Text = intValue.ToString();
                        txt_StartPixelFromBottom.Text = intValue.ToString();
                        //txt_StartPixelFromLeft.Text = intValue.ToString();
                    }
                }
            }

            if (chk_SetToAllLead.Checked)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                        continue;

                    for (int j = 0; j < m_smVisionInfo.g_arrLead[i].ref_intNumberOfLead; j++)
                    {
                        if (j == cbo_LeadNo.SelectedIndex && i == m_smVisionInfo.g_intSelectedROI)
                            continue;
                        blnResult = true;
                        m_smVisionInfo.g_arrLead[i].GetAGVROIToleranceValue(j, ref intTolTop, ref intTolRight, ref intTolBottom, ref intTolLeft);
                        switch (i)
                        {
                            case 1:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolRight) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromLeft.Text = intTolLeft.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolTop.ToString();
                                        //txt_StartPixelFromRight.Text = intTolRight.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolBottom.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolLeft.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                            case 3:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolLeft) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromLeft.Text = intTolRight.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolBottom.ToString();
                                        //txt_StartPixelFromRight.Text = intTolLeft.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolTop.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolRight.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                            case 4:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolTop) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromLeft.Text = intTolBottom.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolLeft.ToString();
                                        //txt_StartPixelFromRight.Text = intTolTop.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolRight.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolBottom.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                            case 2:
                                if (!chk_SetToAllEdge.Checked)
                                {
                                    if ((intValue + intTolBottom) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromLeft.Text = intTolTop.ToString();
                                        blnResult = false;
                                    }
                                }
                                else
                                {
                                    if (((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipLength(j)) || ((intValue + intValue) >= m_smVisionInfo.g_arrLead[i].GetSampleLeadTipWidth(j)))
                                    {
                                        SRMMessageBox.Show("Tolerence value cannot exceed the tolerance value of opposite direction.\n - Lead No " + (j + 1).ToString(), "Wrong Setting");
                                        //txt_StartPixelFromTop.Text = intTolRight.ToString();
                                        //txt_StartPixelFromRight.Text = intTolBottom.ToString();
                                        //txt_StartPixelFromBottom.Text = intTolLeft.ToString();
                                        //txt_StartPixelFromLeft.Text = intTolTop.ToString();
                                        blnResult = false;
                                    }
                                }
                                break;
                        }

                        if (blnResult)
                        {
                            if (!chk_SetToAllEdge.Checked)
                            {
                                switch (i)
                                {
                                    case 1: // Top
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 4, intValue);
                                        break;
                                    case 3: // Bottom
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 2, intValue);
                                        break;
                                    case 4: // Left
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 3, intValue);
                                        break;
                                    case 2: // Right
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 1, intValue);
                                        break;
                                }
                            }
                            else
                            {
                                switch (i)
                                {
                                    case 1: // Top
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 4, intValue);
                                        break;
                                    case 3: // Bottom
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 2, intValue);
                                        break;
                                    case 4: // Left
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 3, intValue);
                                        break;
                                    case 2: // Right
                                        m_smVisionInfo.g_arrLead[i].SetAGVROIToleranceValue(j, 1, intValue);
                                        break;
                                }
                                //txt_StartPixelFromTop.Text = intValue.ToString();
                                //txt_StartPixelFromRight.Text = intValue.ToString();
                                //txt_StartPixelFromBottom.Text = intValue.ToString();
                                //txt_StartPixelFromLeft.Text = intValue.ToString();
                            }
                        }
                    }
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
    }
}
