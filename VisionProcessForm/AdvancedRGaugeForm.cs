using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharedMemory;
using VisionProcessing;

namespace VisionProcessForm
{
    public partial class AdvancedRGaugeForm : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;
        private int m_intTransTypePrev, m_intTransChoicePrev, m_intThresholdPrev, m_intMinAmpPrev, m_intMinAreaPrev, m_intThicknessPrev, m_intFilterPrev, m_intSizeTolerancePrev, m_intFilteringPassPrev, m_intFittingSamplingStepPrev, m_intGainValuePrev;
        private float m_fFilteringThresholdPrev;
        private RectGauge m_objGauge;
        private ROI m_objSearchROI;
        private VisionInfo m_smVisionInfo;
       
        #endregion

        public AdvancedRGaugeForm(VisionInfo smVisionInfo, RectGauge objGauge, ROI objSearchROI)
        {
            m_objGauge = objGauge;
            m_objSearchROI = objSearchROI;
            m_smVisionInfo = smVisionInfo;

            InitializeComponent();

            UpdateGUI();
            m_blnInitDone = true;
        }


        /// <summary>
        /// put default value of rectangle gauge in input box
        /// </summary>
        private void UpdateGUI()
        {
            cbo_TransType.SelectedIndex = m_intTransTypePrev = m_objGauge.ref_GaugeTransType;
            cbo_TransChoice.SelectedIndex = m_intTransChoicePrev = m_objGauge.ref_GaugeTransChoice;
            txt_threshold.Value = m_intThresholdPrev = m_objGauge.ref_GaugeThreshold;
            txt_MinAmplitude.Value =  m_intMinAmpPrev = m_objGauge.ref_GaugeMinAmplitude;
            txt_MinArea.Value = m_intMinAreaPrev = m_objGauge.ref_GaugeMinArea;
            txt_Thickness.Value = m_intThicknessPrev = m_objGauge.ref_GaugeThickness;
            txt_Filter.Value = m_intFilterPrev = Convert.ToInt32(m_objGauge.ref_GaugeFilter);
            txt_SizeTolerance.Value = m_intSizeTolerancePrev = Convert.ToInt32(m_objGauge.ref_GaugeSizeTolerance * 100);
            txt_Tolerance.Value = Convert.ToInt32(m_objGauge.ref_GaugeTolerance);
            txt_FilteringPass.Value = m_intFilteringPassPrev = Convert.ToInt32(m_objGauge.ref_GaugeFilteringPasses);
            m_fFilteringThresholdPrev = Convert.ToSingle(m_objGauge.ref_GaugeFilterThreshold);
            txt_FilteringThreshold.Value = Convert.ToDecimal(m_fFilteringThresholdPrev);
            txt_FittingSamplingStep.Value = m_intFittingSamplingStepPrev = Convert.ToInt32(m_objGauge.ref_GaugeSamplingStep);
            trackBar_Gain.Value = m_intGainValuePrev = Convert.ToInt32(m_objGauge.ref_fGainValue);
            txt_GainValue.Text = (Convert.ToSingle(trackBar_Gain.Value) / 1000).ToString();
        }


        private void cbo_TransChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.ref_GaugeTransChoice = cbo_TransChoice.SelectedIndex;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_TransType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.ref_GaugeTransType = cbo_TransType.SelectedIndex;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
     
        private void txt_Filter_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.ref_GaugeFilter = (int)txt_Filter.Value;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MinAmplitude_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.ref_GaugeMinAmplitude = (int)txt_MinAmplitude.Value;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MinArea_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.ref_GaugeMinArea = (int)txt_MinArea.Value;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_Gain_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.ref_fGainValue = Convert.ToSingle(trackBar_Gain.Value);
            float fGain = Convert.ToSingle(trackBar_Gain.Value) / 1000;
            txt_GainValue.Text = fGain.ToString();

            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, fGain);
            else
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, fGain);

            m_objSearchROI.AttachImage(m_smVisionInfo.g_objPackageImage);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GainValue_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            trackBar_Gain.Value = (int)Math.Round(Convert.ToSingle(txt_GainValue.Text) * 1000, 0, MidpointRounding.AwayFromZero);

            m_objGauge.ref_fGainValue = Convert.ToSingle(trackBar_Gain.Value);
            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, Convert.ToSingle(txt_GainValue.Text));
            else
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, Convert.ToSingle(txt_GainValue.Text));

            m_objSearchROI.AttachImage(m_smVisionInfo.g_objPackageImage);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FittingSamplingStep_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.ref_GaugeSamplingStep = (int)txt_FittingSamplingStep.Value;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SizeTolerance_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.SetRectGaugeSize(m_objSearchROI,  (float)txt_SizeTolerance.Value / 100.0f);          
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Thickness_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.ref_GaugeThickness = (int)txt_Thickness.Value;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_threshold_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.ref_GaugeThreshold = (int)txt_threshold.Value;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

      



        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_objGauge.ref_GaugeTransChoice = m_intTransChoicePrev;
            m_objGauge.ref_GaugeTransType = m_intTransTypePrev;
            m_objGauge.SetRectGaugeSetting(m_intThicknessPrev, m_intFilterPrev, m_intThresholdPrev, m_intMinAmpPrev, m_intMinAreaPrev);
            //m_objGauge.ref_GaugeSizeTolerance = (float)m_intSizeTolerancePrev /100;
            m_objGauge.SetRectGaugeSize(m_objSearchROI, (float)m_intSizeTolerancePrev / 100.0f);
            m_objGauge.ref_GaugeFilteringPasses = m_intFilteringPassPrev;
            m_objGauge.ref_GaugeFilterThreshold = m_fFilteringThresholdPrev;
            m_objGauge.ref_GaugeSamplingStep = m_intFittingSamplingStepPrev;

            m_objGauge.ref_fGainValue = m_intGainValuePrev;
            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, Convert.ToSingle(m_intGainValuePrev) / 1000);
            else
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, Convert.ToSingle(m_intGainValuePrev) / 1000);

            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            Close();
            Dispose();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blncboImageView = true;
            Close();
            Dispose();
        }

        private void AdvancedRGaugeForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blncboImageView = false;
        }

        private void txt_Tolerance_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.SetRectGaugeTolerance(m_objSearchROI, (float)txt_Tolerance.Value);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringPass_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.ref_GaugeFilteringPasses = (int)txt_FilteringPass.Value;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringThreshold_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_objGauge.ref_GaugeFilterThreshold = Convert.ToSingle(txt_FilteringThreshold.Value);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
    }
}