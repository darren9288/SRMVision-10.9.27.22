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
using VisionProcessing;
using Common;

namespace VisionProcessForm
{
    public partial class AdvancedLead3DCalibrationGaugeForm : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;

        private string m_strPath = "";
        private VisionInfo m_smVisionInfo;
        
        //private PGauge m_objPointGauge;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        #endregion

        public AdvancedLead3DCalibrationGaugeForm(VisionInfo smVisionInfo, string strPath, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strPath = strPath;
         
            m_smProductionInfo = smProductionInfo;
            UpdateGUI();

            m_blnInitDone = true;
        }

        private void UpdateGUI()
        {
            if (m_smVisionInfo.g_objCalibrationLead3D != null)
            {
                if (m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeTransitionChoice == 0)
                    radioBtn_FromBegin.Checked = true;
                else if (m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeTransitionChoice == 1)
                    radioBtn_FromEnd.Checked = true;
                else
                    radioBtn_LargeAmplitude.Checked = true;

                txt_MeasThickness.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeThickness.ToString();
                trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
                txt_MeasFilter.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeFilter.ToString();
                txt_MeasMinAmp.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeMinAmplitude.ToString();
                trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
                txt_MeasMinArea.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeMinArea.ToString();
                txt_FilteringPass.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeFilterPass.ToString();
                txt_FilteringThreshold.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_fGaugeFilterThreshold.ToString();
                txt_threshold.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeThreshold.ToString();
                trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);
                txt_SamplingStep.Text = m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeSamplingSteps.ToString();

            }
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

            m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeTransitionChoice = intTransChoiceIndex;

            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objCalibrationLead3D.SaveGauge(
                       m_strPath,
                       false,
                       "CalibrationGauge",
                       true,
                       true);

            Close();
            Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objCalibrationLead3D.LoadGauge(m_strPath,
                            "CalibrationGauge");
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

            m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
            trackBar_Thickness.Value = m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeThickness;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_threshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeThreshold = Convert.ToInt32(txt_threshold.Text);
            trackBar_Derivative.Value = m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeThreshold;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_Derivative_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_threshold.Text = trackBar_Derivative.Value.ToString();
        }

        private void txt_MeasMinAmp_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp.Text);
            trackBar_MinAmp.Value = m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeMinAmplitude;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void trackBar_MinAmp_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MeasMinAmp.Text = trackBar_MinAmp.Value.ToString();
        }

        private void txt_SamplingStep_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeSamplingSteps = Convert.ToInt32(txt_SamplingStep.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasMinArea_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_FilteringPass_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeFilterPass = Convert.ToInt32(txt_FilteringPass.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringThreshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objCalibrationLead3D.ref_fGaugeFilterThreshold = (float)Convert.ToDouble(txt_FilteringThreshold.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasFilter_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objCalibrationLead3D.ref_intGaugeFilter = Convert.ToInt32(txt_MeasFilter.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
    }
}
