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
using Microsoft.Win32;

namespace VisionProcessForm
{
    public partial class LineProfileForm : Form
    {
        /*
         * Line gauge study For Pad Vision:
         * ------------------
         * - Derivative == Gauge Threshold
         * - Alwasy start from White to Black because pad is white color and will measure from inside pad to outside pad.
         * - Need to make sure point with largest amplitude is the last point also.
         * - How to make sure:
         *   - When peak with largest amplitude is last peak also.
         *   - Adjust gauge filter can help to create this scenario.
         * - Max Derivative = Gauge Threshold + peak highest amplitude.
         * - What is the optimal value for Derivative value? 
         *   - Answer: Max Derivative / 2.
         * 
         */

        #region enum

        public enum ReadFrom { Pad = 0, Calibration = 1 };

        #endregion

        #region Member Variables

        private bool m_blnInitDone = false;
        private bool m_blnShow = false;
        private bool m_blnUpdateHistogram = false;
        private bool m_blnTriggerOfflineTest = false;
        private int m_intSelectedPadIndex = 0;
        private int m_intSelectedPadROIIndex = 0;   // 0=Center, 1=Top, 2=Right, 3=Bottom, 4=Left
        private int m_intLineGaugeSelectedIndex = 0;
        private string m_strPath = "";
        private int m_intSelectedImage = 0;
        private ReadFrom m_eReadFrom = ReadFrom.Pad;    // Default
        private PGauge m_objPointGauge;
        private Graphics m_Graphic;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        #endregion

        #region Properties

        public bool ref_blnShow { get { return m_blnShow; } }

        #endregion

        public LineProfileForm(VisionInfo smVisionInfo, PGauge objPointGauge, string strPath, ProductionInfo smProductionInfo)
        {
            InitializeComponent();
           
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);
            m_objPointGauge = objPointGauge;
            m_intSelectedPadROIIndex = m_smVisionInfo.g_intSelectedROI;
            m_intSelectedImage = m_smVisionInfo.g_intSelectedImage;
            m_smVisionInfo.g_intSelectedImage = 0;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_strPath = strPath;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].CopySettingToSettingPointGauge();
                m_smVisionInfo.g_arrPad[i].ref_blnWantCollectPadEdgeGaugePoints = true;
            }

            m_smVisionInfo.g_intViewInspectionSetting = 1;
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_SetToAll.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllPointGauge_Pad5S", false));

            UpdateGUI();
            m_blnInitDone = true;

            m_blnTriggerOfflineTest = true;
        }

        private void UpdateGUI()
        {
            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    UpdateGaugeToGUI();
                    break;
            }

            // Init Point Gauge location
            // m_objPointGauge.SetGaugeCenter(530, 350);
            // m_objPointGauge.SetGaugeToleranceAngle(100, 0);
            m_objPointGauge.EnableManualDrag();
            cbo_ROI.Items.Clear();
            cbo_PadNo.Items.Clear();
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                //if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].GetBlobsFeaturesNumber() > 0) // 2019-12-26 ZJYEOH : Sometimes there might be no pad built on certain ROI, but still need to add the ROI option into combo box
                //{
                if (i == 0)
                        cbo_ROI.Items.Add("Center");
                    else if (i == 1)
                        cbo_ROI.Items.Add("Top");
                    else if (i == 2)
                        cbo_ROI.Items.Add("Right");
                    else if (i == 3)
                        cbo_ROI.Items.Add("Bottom");
                    else if (i == 4)
                        cbo_ROI.Items.Add("Left");

                    if (m_smVisionInfo.g_intSelectedROI == i)
                    {
                        if (cbo_PadNo.Items.Count == 0)
                        {
                            int intTotalPad = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].GetBlobsFeaturesNumber();
                            for (int p = 0; p < intTotalPad; p++)
                            {
                                cbo_PadNo.Items.Add((p + 1).ToString());
                            }
                        }
                    }
                //}
            }

            if (cbo_ROI.Items.Count == 1)
            {
                cbo_ROI.Visible = false;
                lbl_ROI.Visible = false;
                chk_SetToAll.Visible = false;
            }

            if (cbo_ROI.Items.Count >0)
            {
                cbo_ROI.SelectedIndex = m_smVisionInfo.g_intSelectedROI;
            }

            if (cbo_PadNo.Items.Count > 0)
                cbo_PadNo.SelectedIndex = 0;

            //SetSettingPointGaugePosition();
        }

        private void UpdateGaugeToGUI()
        {
            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType == 0)
            {
                radioBtn_BlackToWhite.Checked = true;
            }
            else
            {
                radioBtn_WhiteToBlack.Checked = true;
            }

            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransChoice == 0)
            {
                radioBtn_FromBegin.Checked = true;
            }
            else if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransChoice == 1)
            {
                radioBtn_FromEnd.Checked = true;
            }
            else if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransChoice == 2)
            {
                radioBtn_LargeAmplitude.Checked = true;
            }
            else
            {
                radioBtn_Close.Checked = true;
            }

            txt_MeasThickness.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeThickness.ToString();
            trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);

            txt_threshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeThreshold.ToString();
            trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);

            txt_MeasMinAmp.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeMinAmplitude.ToString();
            trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);

            UpdateGUIPicture();
        }

        private void pic_Histogram_Click(object sender, EventArgs e)
        {

        }

        private void LineProfileForm_Load(object sender, EventArgs e)
        {
            m_blnShow = true;
            m_smVisionInfo.g_blnViewPointGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    
                    STDeviceEdit.CopySettingFile(m_strPath, "");
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.SavePointGauge(
                        m_strPath,
                        false,
                        "Pad" + i.ToString(),
                        true,
                        true);
                        
                    }
                    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Pad Line Profile", m_smProductionInfo.g_strLotID);
                    
                    break;
            }

            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        m_smVisionInfo.g_arrPad[i].ref_objPointGauge.LoadPointGauge(m_strPath,
                                "Pad" + i.ToString());

                        // Permanent set minAmp = 0, min area = 0, filter to 1, choice = from begin
                        //m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeTransChoice = 0; // 2019-09-26 ZJYEOH : No more fixed to from begin
                        //m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeMinAmplitude = 0; // 2019-12-21 CCENG : No more fix Min Amplitude to 0
                        m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeMinArea = 0;
                        m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeFilter = 1;
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
                m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.DrawLineProfile(m_Graphic, pic_Histogram.Width, pic_Histogram.Height);

                m_blnUpdateHistogram = false;
                m_smVisionInfo.AT_VM_UpdateHistogram = false;
            }

            if (m_blnTriggerOfflineTest)
            {
                m_blnTriggerOfflineTest = false;
                m_smVisionInfo.AT_VM_OfflineTestAllPad = true;
                TriggerOfflineTest();
            }

            if (m_smVisionInfo.PR_MN_UpdateInfo)
            {
                SetSettingPointGaugePosition();

                m_smVisionInfo.g_blnViewPointGauge = true;

                m_smVisionInfo.PR_MN_UpdateInfo = false;
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

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_smVisionInfo.g_intSelectedROI == i)
                        {
                            if (radioBtn_BlackToWhite.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeTransType = 0;
                                m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                            }
                            else
                            {
                                m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeTransType = 1;
                                m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                            }
                        }
                        else
                        {
                            if (chk_SetToAll.Checked)
                            {
                                if (radioBtn_BlackToWhite.Checked)
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeTransType = 0;
                                    m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeTransType = 1;
                                    m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                }
                            }
                        }
                    }
                    UpdateGUIPicture();
                    break;
            }

            //SetSettingPointGaugePosition();

            m_blnTriggerOfflineTest = true;
        }

        private void LineProfileForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_objPointGauge.DisableManualDrag();

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                m_smVisionInfo.g_arrPad[i].ref_blnWantCollectPadEdgeGaugePoints = false;
            }

            m_blnShow = false;
            m_smVisionInfo.g_intSelectedImage = m_intSelectedImage;
            m_smVisionInfo.g_intSelectedROI = m_intSelectedPadROIIndex;
            m_smVisionInfo.g_intViewInspectionSetting = 0;
            m_smVisionInfo.g_blnViewPointGauge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void TriggerOfflineTest()
        {
            m_smVisionInfo.g_objPositioning.ref_blnDrawPHResult = false;
            m_smVisionInfo.g_blnViewPadSettingDrawing = false;
            m_smVisionInfo.g_blnViewPointGauge = false;

            // 29-07-2019 ZJYEOH : If user pressed test button, then Image should stop live
            if (m_smVisionInfo.AT_PR_StartLiveImage)
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            }

            m_smVisionInfo.g_blnViewPHImage = false;
            m_smVisionInfo.g_blnCheckPH = false;

            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 0)
                    m_smVisionInfo.g_arrPadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            }

            m_smVisionInfo.AT_VM_ManualTestMode = true;

            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
        }

        private void trackBar_Thickness_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MeasThickness.Text = trackBar_Thickness.Value.ToString();
        }

        private void txt_MeasThickness_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_smVisionInfo.g_intSelectedROI == i)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                            m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                            trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
                        }
                        else
                        {
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                            }
                        }
                    }
                    break;
            }

            //SetSettingPointGaugePosition();

            m_blnTriggerOfflineTest = true;
        }

        private void txt_threshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_smVisionInfo.g_intSelectedROI == i)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                            m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                            trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);
                        }
                        else
                        {
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                            }
                        }
                    }
                    break;
            }

            //SetSettingPointGaugePosition();

            m_blnTriggerOfflineTest = true;

        }

        private void trackBar_Derivative_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_threshold.Text = trackBar_Derivative.Value.ToString();
        }

        private void radioBtn_Left_Click(object sender, EventArgs e)
        {
            if (sender == radioBtn_Up)
            {
                m_intLineGaugeSelectedIndex = 0;
            }
            else if (sender == radioBtn_Right)
            {
                m_intLineGaugeSelectedIndex = 1;
            }
            else if (sender == radioBtn_Down)
            {
                m_intLineGaugeSelectedIndex = 2;
            }
            else if (sender == radioBtn_Left)
            {
                m_intLineGaugeSelectedIndex = 3;
            }

            if (cbo_PadNo.Items.Count == 0)
                return;

            SetSettingPointGaugePosition();
        }

        private void cbo_PadNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            SetSettingPointGaugePosition();
        }

        private void SetSettingPointGaugePosition()
        {
            m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].SetPointGaugePlacement_UsingInspectedPadPointGaugePosition(cbo_PadNo.SelectedIndex, m_intLineGaugeSelectedIndex);

            ROI objROI = new ROI();
            objROI.AttachImage(m_smVisionInfo.g_arrImages[0]); //m_smVisionInfo.g_intSelectedImage
            objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[0].ref_intImageWidth,
                m_smVisionInfo.g_arrImages[0].ref_intImageHeight);
            m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(objROI);
            objROI.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_blnUpdateHistogram = true;
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
            else if (radioBtn_LargeAmplitude.Checked)
                intTransChoiceIndex = 2;
            else
                intTransChoiceIndex = 3;
            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_smVisionInfo.g_intSelectedROI == i)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeTransChoice = intTransChoiceIndex;
                            m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.ref_GaugeTransChoice = intTransChoiceIndex;
                        }
                        else
                        {
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeTransChoice = intTransChoiceIndex;
                                m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.ref_GaugeTransChoice = intTransChoiceIndex;
                            }
                        }
                    }

                    //m_smVisionInfo.g_arrPad[0].ref_objPointGauge.Measure(new ROI());
                    //m_smVisionInfo.g_arrPad[0].ref_objSettingPointGauge.Measure(new ROI());
                    break;
            }

            //SetSettingPointGaugePosition();

            m_blnTriggerOfflineTest = true;
        }

        private void cbo_ROI_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedROI = cbo_ROI.SelectedIndex;
            m_blnInitDone = false;
            UpdateGUI();
            UpdateGUIPicture();
            m_blnInitDone = true;
            SetSettingPointGaugePosition();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdateGUIPicture()
        {
            //Top
            if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeTransType == 0))
                pic_Up.Image = imageList1.Images[2];
            else if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeTransType == 1))
                pic_Up.Image = imageList1.Images[6];

            // Right
            if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeTransType == 0))
                pic_Right.Image = imageList1.Images[3];
            else if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeTransType == 1))
                pic_Right.Image = imageList1.Images[7];

            //Bottom
            if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeTransType == 0))
                pic_Down.Image = imageList1.Images[0];
            else if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeTransType == 1))
                pic_Down.Image = imageList1.Images[4];

            //Left
            if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeTransType == 0))
                pic_Left.Image = imageList1.Images[1];
            else if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeTransType == 1))
                pic_Left.Image = imageList1.Images[5];
        }

        private void chk_SetToAll_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllPointGauge_Pad5S", chk_SetToAll.Checked);
          
        }

        private void trackBar_MinAmp_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

             txt_MeasMinAmp.Text = trackBar_MinAmp.Value.ToString();
        }

        private void txt_MeasMinAmp_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Pad:
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (m_smVisionInfo.g_intSelectedROI == i)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp.Text);
                            m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp.Text);
                            trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
                        }
                        else
                        {
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_objPointGauge.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp.Text);
                                m_smVisionInfo.g_arrPad[i].ref_objSettingPointGauge.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp.Text);
                            }
                        }
                    }
                    break;
            }

            //SetSettingPointGaugePosition();

            m_blnTriggerOfflineTest = true;
        }
    }
}

