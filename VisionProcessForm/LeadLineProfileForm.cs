using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharedMemory;
using VisionProcessing;
using Common;
namespace VisionProcessForm
{
    public partial class LeadLineProfileForm : Form
    {
        #region enum

        public enum ReadFrom { Lead = 0, Calibration = 1 };

        #endregion

        #region Member Variables
        private int m_intReadFromIndex = 0; // 0: Learn Page, 1: Other Setting Page
        private bool m_blnInitDone = false;
        private bool m_blnShow = false;
        private bool m_blnBuildLead = false;
        private bool m_blnUpdateHistogram = false;
        private string m_strPath = "";
        private bool m_blnTriggerOfflineTest = false;
        private ReadFrom m_eReadFrom = ReadFrom.Lead;    // Default
        private PGauge m_objPointGauge;
        private Graphics m_Graphic;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        #endregion

        #region Properties

        public bool ref_blnShow { get { return m_blnShow; } }
        public bool ref_blnBuildLead { get { return m_blnBuildLead; } set { m_blnBuildLead = value; } }
        #endregion

        public LeadLineProfileForm(VisionInfo smVisionInfo, PGauge objPointGauge, string strPath, ProductionInfo smProductionInfo, int ReadFromIndex)
        {
            InitializeComponent();
            m_intReadFromIndex = ReadFromIndex;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);
            m_objPointGauge = objPointGauge;
            m_strPath = strPath;
            m_smVisionInfo.g_intViewInspectionSetting = 1;
            m_smVisionInfo.g_intPointSelectedNumber = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                m_smVisionInfo.g_arrLead[i].CopySettingToSettingPointGauge(i, m_smVisionInfo.g_intPointSelectedNumber);
            }

            UpdateGUI();

            m_blnInitDone = true;

            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }

        private void UpdateGUI()
        {
            // Init Point Gauge location
            // m_objPointGauge.SetGaugeCenter(530, 350);
            // m_objPointGauge.SetGaugeToleranceAngle(100, 0);
            m_smVisionInfo.g_blnSetToAllPoints = chk_AllPoints.Checked = false;
            m_smVisionInfo.g_blnSetToAllLeadPad = chk_AllLeads.Checked = false;
            m_smVisionInfo.g_blnSetToAllROIs = chk_AllROIs.Checked = false;

            m_objPointGauge.EnableManualDrag();

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
            //if (m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intLeadDirection == 0)
            //{
            //    cbo_ROI.Items.Add("Right");
            //    cbo_ROI.Items.Add("Left");
            //}
            //else
            //{
            //    cbo_ROI.Items.Add("Top");
            //    cbo_ROI.Items.Add("Bottom");
            //}

            if (cbo_ROI.Items.Count > 0)
                cbo_ROI.SelectedIndex = 0;

            UpdateComboBox();

            switch (m_eReadFrom)
            {
                case ReadFrom.Lead:
                    UpdateGaugeToGUI();
                    break;
            }

            //if (m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance)
            //    panel3.BringToFront();
            //else
            //    panel3.SendToBack();
        }

        private void UpdateGaugeToGUI()
        {
            //int LeadIndex = Convert.ToInt32(cbo_LeadNo_Center.SelectedItem) - 1;
            //if (LeadIndex < 0)
            //    LeadIndex = 0;
            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
            if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                m_smVisionInfo.g_intLeadSelectedNumber = 0;
            PGauge objPGauge = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber];

            if (objPGauge.ref_GaugeTransType == 0)
            {
                radioBtn_BlackToWhite.Checked = true;
            }
            else
            {
                radioBtn_WhiteToBlack.Checked = true;
            }
            if (objPGauge.ref_GaugeTransChoice == 0)
                radioBtn_FromBegin.Checked = true;
            else if (objPGauge.ref_GaugeTransChoice == 1)
                radioBtn_FromEnd.Checked = true;
            else
                radioBtn_LargestAmplitude.Checked = true;

            txt_MeasThickness.Text = objPGauge.ref_GaugeThickness.ToString();
            trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);

            txt_threshold.Text = objPGauge.ref_GaugeThreshold.ToString();
            trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);

            txt_MinAmplitude.Text = objPGauge.ref_GaugeMinAmplitude.ToString();
            trackBar_MinAmplitude.Value = Convert.ToInt32(txt_MinAmplitude.Text);

        }

        private void pic_Histogram_Click(object sender, EventArgs e)
        {

        }

        private void LineProfileForm_Load(object sender, EventArgs e)
        {
            m_blnShow = true;
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_blnViewPointGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            switch (m_eReadFrom)
            {
                case ReadFrom.Lead:
                    
                    STDeviceEdit.CopySettingFile(m_strPath, "");
                    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {

                        m_smVisionInfo.g_arrLead[i].ref_objPointGauge.SavePointGauge(
                        m_strPath,
                        false,
                        "Lead" + i.ToString(),
                        true,
                        true);

                        m_smVisionInfo.g_arrLead[i].SaveArrayPointGauge(m_strPath);

                    }
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead Line Profile", m_smProductionInfo.g_strLotID);
                    
                    break;
            }

            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            switch (m_eReadFrom)
            {
                case ReadFrom.Lead:
                    for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    {
                      
                        m_smVisionInfo.g_arrLead[i].ref_objPointGauge.LoadPointGauge(m_strPath,
                                "Lead" + i.ToString());

                        m_smVisionInfo.g_arrLead[i].LoadArrayPointGauge(m_strPath);


                    }
                    break;
            }

            this.Close();
            this.Dispose();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (m_blnUpdateHistogram || m_smVisionInfo.AT_VM_UpdateHistogram)
            {
                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.DrawLineProfile(m_Graphic, pic_Histogram.Width, pic_Histogram.Height);
                
                m_blnUpdateHistogram = false;
                m_smVisionInfo.AT_VM_UpdateHistogram = false;
            }

            if (m_blnTriggerOfflineTest)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
            {
                m_blnTriggerOfflineTest = false;
                m_smVisionInfo.AT_VM_OfflineTestAllLead = true;
                TriggerOfflineTest();
            }

            if (m_smVisionInfo.PR_MN_UpdateInfo)
            {
                SetSettingPointGaugePosition();

                m_smVisionInfo.g_blnViewPointGauge = true;

                m_smVisionInfo.PR_MN_UpdateInfo = false;
            }

            if (m_smVisionInfo.g_blnUpdateSelectedROI)
            {
                switch (m_smVisionInfo.g_intSelectedROI)
                {
                    case 1:
                        if (cbo_ROI.Items.Contains("Top"))
                            cbo_ROI.SelectedItem = "Top";
                        break;
                    case 2:
                        if (cbo_ROI.Items.Contains("Right"))
                            cbo_ROI.SelectedItem = "Right";
                        break;
                    case 3:
                        if (cbo_ROI.Items.Contains("Bottom"))
                            cbo_ROI.SelectedItem = "Bottom";
                        break;
                    case 4:
                        if (cbo_ROI.Items.Contains("Left"))
                            cbo_ROI.SelectedItem = "Left";
                        break;
                }
            }
        }

        private void pic_Histogram_Paint(object sender, PaintEventArgs e)
        {
            m_blnUpdateHistogram = true;
        }

        private void radioBtn_WhiteToBlack_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
            if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                m_smVisionInfo.g_intLeadSelectedNumber = 0;

            switch (m_eReadFrom)
            {
                case ReadFrom.Lead:

                    if (m_smVisionInfo.g_blnSetToAllROIs)
                    {
                        for (int r = 0; r < m_smVisionInfo.g_arrLead.Length; r++)
                        {
                            if (!m_smVisionInfo.g_arrLead[r].ref_blnSelected)
                                continue;

                            if (m_smVisionInfo.g_blnSetToAllLeadPad)
                            {
                                for (int L = 0; L < m_smVisionInfo.g_arrLead[r].ref_intNumberOfLead; L++)
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints)
                                    {
                                        for (int p = 0; p < 6; p++)
                                        {
                                            if (L < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge.Count && 
                                                p < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L].Count)
                                            {
                                                if (radioBtn_BlackToWhite.Checked)
                                                {
                                                    m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][p].ref_GaugeTransType = 0;
                                                }
                                                else
                                                {
                                                    m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][p].ref_GaugeTransType = 1;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (radioBtn_BlackToWhite.Checked)
                                        {
                                            m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransType = 0;
                                        }
                                        else
                                        {
                                            m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransType = 1;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints)
                                {
                                    for (int p = 0; p < 6; p++)
                                    {
                                        if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge.Count && 
                                            p < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                        {
                                            if (radioBtn_BlackToWhite.Checked)
                                                m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransType = 0;
                                            else
                                                m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransType = 1;
                                        }
                                    }
                                }
                                else
                                {
                                    if (radioBtn_BlackToWhite.Checked)
                                        m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransType = 0;
                                    else
                                        m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransType = 1;

                                }
                            }
                        }
                    }
                    else
                    {
                        if (m_smVisionInfo.g_blnSetToAllLeadPad)
                        {
                            for (int L = 0; L < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intNumberOfLead; L++)
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints)
                                {
                                    for (int p = 0; p < 6; p++)
                                    {
                                        if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge.Count && 
                                            p < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                        {
                                            if (radioBtn_BlackToWhite.Checked)
                                            {
                                                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][p].ref_GaugeTransType = 0;
                                            }
                                            else
                                            {
                                                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][p].ref_GaugeTransType = 1;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (radioBtn_BlackToWhite.Checked)
                                    {
                                        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransType = 0;
                                    }
                                    else
                                    {
                                        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransType = 1;
                                    }

                                }
                            }
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnSetToAllPoints)
                            {
                                for (int p = 0; p < 6; p++)
                                {
                                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge.Count && 
                                        p < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                    {
                                        if (radioBtn_BlackToWhite.Checked)
                                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransType = 0;
                                        else
                                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransType = 1;
                                    }
                                }
                            }
                            else
                            {
                                if (radioBtn_BlackToWhite.Checked)
                                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransType = 0;
                                else
                                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransType = 1;

                            }
                        }
                    }


                    if (radioBtn_BlackToWhite.Checked)
                    {
                        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                    }
                    else
                    {
                        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                    }

                    break;
            }

            ROI objROI = new ROI();
            if (m_smVisionInfo.g_blnViewRotatedImage)
                objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            else
                objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].ref_intImageWidth,
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].ref_intImageHeight);
            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.Measure(objROI);
            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(objROI);
            objROI.Dispose();
            // Update histogram
            m_blnUpdateHistogram = true;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }

        private void LineProfileForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_objPointGauge.DisableManualDrag();
            m_smVisionInfo.AT_VM_OfflineTestAllLead = false;
            m_blnShow = false;
            m_smVisionInfo.g_intViewInspectionSetting = 0;
            m_smVisionInfo.g_blnViewLeadInspection = false;
            if (m_intReadFromIndex == 1)
                m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewPointGauge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void radioBtn_Search_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intTransChoiceIndex;
            if (radioBtn_FromBegin.Checked)
                intTransChoiceIndex = 0;
            else if (radioBtn_FromEnd.Checked)
                intTransChoiceIndex = 1;
            else
                intTransChoiceIndex = 2;

            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
            if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                m_smVisionInfo.g_intLeadSelectedNumber = 0;

            if (m_smVisionInfo.g_blnSetToAllROIs)
            {
                for (int r = 0; r < m_smVisionInfo.g_arrLead.Length; r++)
                {
                    if (!m_smVisionInfo.g_arrLead[r].ref_blnSelected)
                        continue;

                    if (m_smVisionInfo.g_blnSetToAllLeadPad)
                    {
                        for (int L = 0; L < m_smVisionInfo.g_arrLead[r].ref_intNumberOfLead; L++)
                        {
                            if (m_smVisionInfo.g_blnSetToAllPoints)
                            {
                                for (int p = 0; p < 6; p++)
                                {
                                    if (L < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge.Count && 
                                        p < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L].Count)
                                    {
                                        if (radioBtn_BlackToWhite.Checked)
                                        {
                                            m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                        }
                                        else
                                        {
                                            m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (radioBtn_BlackToWhite.Checked)
                                {
                                    m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransChoice = intTransChoiceIndex;
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransChoice = intTransChoiceIndex;
                                }

                            }
                        }
                    }
                    else
                    {
                        if (m_smVisionInfo.g_blnSetToAllPoints)
                        {
                            for (int p = 0; p < 6; p++)
                            {
                                if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge.Count && 
                                    p < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                {
                                    if (radioBtn_BlackToWhite.Checked)
                                        m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                    else
                                        m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                }
                            }
                        }
                        else
                        {
                            if (radioBtn_BlackToWhite.Checked)
                                m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransChoice = intTransChoiceIndex;
                            else
                                m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransChoice = intTransChoiceIndex;

                        }
                    }
                }
            }
            else
            {
                if (m_smVisionInfo.g_blnSetToAllLeadPad)
                {
                    for (int L = 0; L < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intNumberOfLead; L++)
                    {
                        if (m_smVisionInfo.g_blnSetToAllPoints)
                        {
                            for (int p = 0; p < 6; p++)
                            {
                                if (L < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge.Count && 
                                    p < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L].Count)
                                {
                                    if (radioBtn_BlackToWhite.Checked)
                                    {
                                        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                    }
                                    else
                                    {
                                        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (radioBtn_BlackToWhite.Checked)
                            {
                                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransChoice = intTransChoiceIndex;
                            }
                            else
                            {
                                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransChoice = intTransChoiceIndex;
                            }

                        }
                    }
                }
                else
                {
                    if (m_smVisionInfo.g_blnSetToAllPoints)
                    {
                        for (int p = 0; p < 6; p++)
                        {
                            if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge.Count && 
                                p < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                            {
                                if (radioBtn_BlackToWhite.Checked)
                                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                else
                                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransChoice = intTransChoiceIndex;
                            }
                        }
                    }
                    else
                    {
                        if (radioBtn_BlackToWhite.Checked)
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransChoice = intTransChoiceIndex;
                        else
                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeTransChoice = intTransChoiceIndex;

                    }
                }
            }

            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransChoice = intTransChoiceIndex;

            ROI objROI = new ROI();
            if (m_smVisionInfo.g_blnViewRotatedImage)
                objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            else
                objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].ref_intImageWidth,
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].ref_intImageHeight);
            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].Measure(objROI);
            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(objROI);
            objROI.Dispose();

            // Update histogram
            m_blnUpdateHistogram = true;
            m_blnBuildLead = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }
        private void radioBtn_CenterDirection_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (sender == radioBtn_CenterTipStart)
                m_smVisionInfo.g_intPointSelectedNumber = (int)Lead.PointIndex.TipStart;
            else if (sender == radioBtn_CenterTipCenter)
                m_smVisionInfo.g_intPointSelectedNumber = (int)Lead.PointIndex.TipCenter;
            else if (sender == radioBtn_CenterTipEnd)
                m_smVisionInfo.g_intPointSelectedNumber = (int)Lead.PointIndex.TipEnd;
            else if (sender == radioBtn_CenterBaseStart)
                m_smVisionInfo.g_intPointSelectedNumber = (int)Lead.PointIndex.BaseStart;
            else if (sender == radioBtn_CenterBaseCenter)
                m_smVisionInfo.g_intPointSelectedNumber = (int)Lead.PointIndex.BaseCenter;
            else if (sender == radioBtn_CenterBaseEnd)
                m_smVisionInfo.g_intPointSelectedNumber = (int)Lead.PointIndex.BaseEnd;

            UpdateGaugeToGUI();
            SetSettingPointGaugePosition();
        }

        private void cbo_LeadNo_Center_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            UpdateGaugeToGUI();
            SetSettingPointGaugePosition();
        }

        private void txt_MeasThickness_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
            if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                m_smVisionInfo.g_intLeadSelectedNumber = 0;

            switch (m_eReadFrom)
            {
                case ReadFrom.Lead:
                    //if (m_smVisionInfo.g_arrLead.Length > m_smVisionInfo.g_intSelectedROI)
                    //{

                    //            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);

                    //    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                    //    trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
                    //}
                    ////for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                    ////{

                    ////    m_smVisionInfo.g_arrLead[i].ref_objPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);

                    ////    m_smVisionInfo.g_arrLead[i].ref_objSettingPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                    ////    trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
                    ////}
                    int intThicknessValue = Convert.ToInt32(txt_MeasThickness.Text);

                    if (m_smVisionInfo.g_blnSetToAllROIs)
                    {
                        for (int r = 0; r < m_smVisionInfo.g_arrLead.Length; r++)
                        {
                            if (!m_smVisionInfo.g_arrLead[r].ref_blnSelected)
                                continue;

                            if (m_smVisionInfo.g_blnSetToAllLeadPad)
                            {
                                for (int L = 0; L < m_smVisionInfo.g_arrLead[r].ref_intNumberOfLead; L++)
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints)
                                    {
                                        for (int p = 0; p < 6; p++)
                                        {
                                            if (L < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge.Count && 
                                                p < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L].Count)
                                            {
                                                m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][p].ref_GaugeThickness = intThicknessValue;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeThickness = intThicknessValue;
                                    }
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints)
                                {
                                    for (int p = 0; p < 6; p++)
                                    {
                                        if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge.Count && 
                                            p < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                        {
                                            m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeThickness = intThicknessValue;
                                        }
                                    }
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeThickness = intThicknessValue;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (m_smVisionInfo.g_blnSetToAllLeadPad)
                        {
                            for (int L = 0; L < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intNumberOfLead; L++)
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints)
                                {
                                    for (int p = 0; p < 6; p++)
                                    {
                                        if (L < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge.Count && 
                                            p < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L].Count)
                                        {
                                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][p].ref_GaugeThickness = intThicknessValue;
                                        }
                                    }
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeThickness = intThicknessValue;
                                }
                            }
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnSetToAllPoints)
                            {
                                for (int p = 0; p < 6; p++)
                                {
                                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge.Count && 
                                        p < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                    {
                                        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeThickness = intThicknessValue;
                                    }
                                }
                            }
                            else
                            {
                                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeThickness = intThicknessValue;
                            }
                        }
                    }

                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                    trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);

                    break;
            }

            ROI objROI = new ROI();
            if (m_smVisionInfo.g_blnViewRotatedImage)
                objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            else
                objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].ref_intImageWidth,
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].ref_intImageHeight);
          
                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.Measure(objROI);
                    
            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(objROI);
            objROI.Dispose();

            // Update histogram
            m_blnUpdateHistogram = true;
            m_blnBuildLead = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }

        private void trackBar_Thickness_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MeasThickness.Text = trackBar_Thickness.Value.ToString();
        }
        private void txt_threshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
            if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                m_smVisionInfo.g_intLeadSelectedNumber = 0;

            switch (m_eReadFrom)
            {
                case ReadFrom.Lead:
                    //if (m_smVisionInfo.g_arrLead.Length > m_smVisionInfo.g_intSelectedROI)
                    //{

                    //            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);

                    //    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                    //    trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);
                    //}

                    int intValue = Convert.ToInt32(txt_threshold.Text);

                    if (m_smVisionInfo.g_blnSetToAllROIs)
                    {
                        for (int r = 0; r < m_smVisionInfo.g_arrLead.Length; r++)
                        {
                            if (!m_smVisionInfo.g_arrLead[r].ref_blnSelected)
                                continue;

                            if (m_smVisionInfo.g_blnSetToAllLeadPad)
                            {
                                for (int L = 0; L < m_smVisionInfo.g_arrLead[r].ref_intNumberOfLead; L++)
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints)
                                    {
                                        for (int p = 0; p < 6; p++)
                                        {
                                            if (L < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge.Count && 
                                                p < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L].Count)
                                            {
                                                m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][p].ref_GaugeThreshold = intValue;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeThreshold = intValue;
                                    }
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints)
                                {
                                    for (int p = 0; p < 6; p++)
                                    {
                                        if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge.Count && 
                                            p < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                        {
                                            m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeThreshold = intValue;
                                        }
                                    }
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeThreshold = intValue;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (m_smVisionInfo.g_blnSetToAllLeadPad)
                        {
                            for (int L = 0; L < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intNumberOfLead; L++)
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints)
                                {
                                    for (int p = 0; p < 6; p++)
                                    {
                                        if (L < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge.Count && 
                                            p < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L].Count)
                                        {
                                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][p].ref_GaugeThreshold = intValue;
                                        }
                                    }
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeThreshold = intValue;
                                }
                            }
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnSetToAllPoints)
                            {
                                for (int p = 0; p < 6; p++)
                                {
                                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge.Count && 
                                        p < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                    {
                                        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeThreshold = intValue;
                                    }
                                }
                            }
                            else
                            {
                                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeThreshold = intValue;
                            }
                        }
                    }


                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                    trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);
                    break;
            }
            ROI objROI = new ROI();
            if (m_smVisionInfo.g_blnViewRotatedImage)
                objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            else
                objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].ref_intImageWidth,
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].ref_intImageHeight);
          
                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.Measure(objROI);

            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(objROI);
            objROI.Dispose();
            // Update histogram
            m_blnUpdateHistogram = true;
            m_blnBuildLead = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }
        private void trackBar_Derivative_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_threshold.Text = trackBar_Derivative.Value.ToString();
        }
        private void SetSettingPointGaugePosition()
        {
            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;

            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].CopySettingToSettingPointGauge(m_smVisionInfo.g_intLeadSelectedNumber, m_smVisionInfo.g_intPointSelectedNumber);
            //int intLeadDataIndex = Convert.ToInt32(cbo_LeadNo_Center.SelectedItem) - 1;
            //if (intLeadDataIndex < 0)
            //    intLeadDataIndex = 0;
            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].SetPointGaugePlacement_UsingInspectedLeadPointGaugePosition(m_smVisionInfo.g_intLeadSelectedNumber, m_smVisionInfo.g_intPointSelectedNumber);//intLeadDataIndex

            if (m_smVisionInfo.g_blnSetToAllROIs)
            {
                for (int r = 0; r < m_smVisionInfo.g_arrLead.Length; r++)
                {
                    if (!m_smVisionInfo.g_arrLead[r].ref_blnSelected || r == m_smVisionInfo.g_intSelectedROI)
                        continue;

                    m_smVisionInfo.g_arrLead[r].CopySettingToSettingPointGauge(m_smVisionInfo.g_intLeadSelectedNumber, m_smVisionInfo.g_intPointSelectedNumber);
                    m_smVisionInfo.g_arrLead[r].SetPointGaugePlacement_UsingInspectedLeadPointGaugePosition(m_smVisionInfo.g_intLeadSelectedNumber, m_smVisionInfo.g_intPointSelectedNumber);
                }
            }

            ROI objROI = new ROI();
            if (m_smVisionInfo.g_blnViewRotatedImage)
                objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            else
                objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].ref_intImageWidth,
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].ref_intImageHeight);
            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(objROI);
            objROI.Dispose();
            m_blnUpdateHistogram = true;
            m_blnBuildLead = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void TriggerOfflineTest()
        {
            m_smVisionInfo.g_blnViewPointGauge = false;

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
            UpdateGaugeToGUI();
            SetSettingPointGaugePosition();
        }

        private void UpdateComboBox()
        {
            cbo_LeadNo_Center.Items.Clear();
            if (m_smVisionInfo.g_arrLead.Length > 0)
            {
                if (m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetBlobsFeaturesNumber() > 0)
                {
                    if (cbo_LeadNo_Center.Items.Count == 0)
                    {
                        List<int> intTotalLead = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetLeadID();
                        for (int p = 0; p < intTotalLead.Count; p++)
                        {
                            cbo_LeadNo_Center.Items.Add((intTotalLead[p]).ToString());
                        }
                    }
                }
            }
            if (cbo_LeadNo_Center.Items.Count > 0)
                cbo_LeadNo_Center.SelectedIndex = 0;
        }

        private void srmLabel4_Click(object sender, EventArgs e)
        {

        }

        private void chk_AllPoints_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnSetToAllPoints = chk_AllPoints.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_AllLeads_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnSetToAllLeadPad = chk_AllLeads.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_AllROIs_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnSetToAllROIs = chk_AllROIs.Checked;

            if (chk_AllROIs.Checked)
                SetSettingPointGaugePosition();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MinAmplitude_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
            if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                m_smVisionInfo.g_intLeadSelectedNumber = 0;

            switch (m_eReadFrom)
            {
                case ReadFrom.Lead:
                    //if (m_smVisionInfo.g_arrLead.Length > m_smVisionInfo.g_intSelectedROI)
                    //{

                    //            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);

                    //    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                    //    trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);
                    //}

                    int intValue = Convert.ToInt32(txt_MinAmplitude.Text);

                    if (m_smVisionInfo.g_blnSetToAllROIs)
                    {
                        for (int r = 0; r < m_smVisionInfo.g_arrLead.Length; r++)
                        {
                            if (!m_smVisionInfo.g_arrLead[r].ref_blnSelected)
                                continue;

                            if (m_smVisionInfo.g_blnSetToAllLeadPad)
                            {
                                for (int L = 0; L < m_smVisionInfo.g_arrLead[r].ref_intNumberOfLead; L++)
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints)
                                    {
                                        for (int p = 0; p < 6; p++)
                                        {
                                            if (L < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge.Count &&
                                                p < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L].Count)
                                            {
                                                m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][p].ref_GaugeMinAmplitude = intValue;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeMinAmplitude = intValue;
                                    }
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints)
                                {
                                    for (int p = 0; p < 6; p++)
                                    {
                                        if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge.Count &&
                                            p < m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                        {
                                            m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeMinAmplitude = intValue;
                                        }
                                    }
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrLead[r].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeMinAmplitude = intValue;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (m_smVisionInfo.g_blnSetToAllLeadPad)
                        {
                            for (int L = 0; L < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intNumberOfLead; L++)
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints)
                                {
                                    for (int p = 0; p < 6; p++)
                                    {
                                        if (L < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge.Count &&
                                            p < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L].Count)
                                        {
                                            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][p].ref_GaugeMinAmplitude = intValue;
                                        }
                                    }
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[L][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeMinAmplitude = intValue;
                                }
                            }
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnSetToAllPoints)
                            {
                                for (int p = 0; p < 6; p++)
                                {
                                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge.Count &&
                                        p < m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                    {
                                        m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeMinAmplitude = intValue;
                                    }
                                }
                            }
                            else
                            {
                                m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_arrPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_smVisionInfo.g_intPointSelectedNumber].ref_GaugeMinAmplitude = intValue;
                            }
                        }
                    }


                    m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                    trackBar_MinAmplitude.Value = Convert.ToInt32(txt_MinAmplitude.Text);
                    break;
            }
            ROI objROI = new ROI();
            if (m_smVisionInfo.g_blnViewRotatedImage)
                objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            else
                objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo]);
            objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].ref_intImageWidth,
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo].ref_intImageHeight);

            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.Measure(objROI);

            m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(objROI);
            objROI.Dispose();
            // Update histogram
            m_blnUpdateHistogram = true;
            m_blnBuildLead = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }

        private void trackBar_MinAmplitude_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MinAmplitude.Text = trackBar_MinAmplitude.Value.ToString();
        }
    }
}

  