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
    public partial class AdvancedSealCircleGaugeForm : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;

        private string m_strPath = "";
        private VisionInfo m_smVisionInfo;

        private string m_path;
        //private PGauge m_objPointGauge;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private bool m_blnMeasureGauge = false;
        private bool m_blnShowForm = false;
        #endregion
        #region Properties

        public bool ref_blnMeasureGauge { get { return m_blnMeasureGauge; } set { m_blnMeasureGauge = value; } }
        public bool ref_blnShowForm { get { return m_blnShowForm; } set { m_blnShowForm = value; } }
        #endregion
        public AdvancedSealCircleGaugeForm(VisionInfo smVisionInfo, string strPath, string Path, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strPath = strPath;
            m_path = Path;
            m_smProductionInfo = smProductionInfo;
            UpdateGUI();

            m_blnInitDone = true;
        }

        private void UpdateGUI()
        {
            if (m_smVisionInfo.g_objSealCircleGauges != null)
            {
                if (m_smVisionInfo.g_objSealCircleGauges.ref_GaugeTransType == 0)
                    radioBtn_BW.Checked = true;
                else
                    radioBtn_WB.Checked = true;


                if (m_smVisionInfo.g_objSealCircleGauges.ref_GaugeTransChoice == 0)
                    radioBtn_FromBegin.Checked = true;
                else if (m_smVisionInfo.g_objSealCircleGauges.ref_GaugeTransChoice == 1)
                    radioBtn_FromEnd.Checked = true;
                else
                    radioBtn_LargeAmplitude.Checked = true;

                txt_MeasThickness.Text = m_smVisionInfo.g_objSealCircleGauges.ref_GaugeThickness.ToString();
                trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
                txt_MeasFilter.Text = m_smVisionInfo.g_objSealCircleGauges.ref_GaugeFilter.ToString();
                txt_MeasMinAmp.Text = m_smVisionInfo.g_objSealCircleGauges.ref_GaugeMinAmplitude.ToString();
                trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
                txt_MeasMinArea.Text = m_smVisionInfo.g_objSealCircleGauges.ref_GaugeMinArea.ToString();
                txt_FilteringPass.Text = m_smVisionInfo.g_objSealCircleGauges.ref_GaugeFilterPasses.ToString();
                txt_FilteringThreshold.Text = m_smVisionInfo.g_objSealCircleGauges.ref_GaugeFilterThreshold.ToString();
                txt_threshold.Text = m_smVisionInfo.g_objSealCircleGauges.ref_GaugeThreshold.ToString();
                trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);
                txt_Tolerance.Text = m_smVisionInfo.g_objSealCircleGauges.ref_GaugeTolerance.ToString();
                txt_SamplingStep.Text = m_smVisionInfo.g_objSealCircleGauges.ref_GaugeSamplingStep.ToString();
                txt_GaugeScore.Text = m_smVisionInfo.g_objSealCircleGauges.ref_intMinScore.ToString();
            }
        }

        private void radioBtn_Polarity_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intTransTypeIndex;
            if (radioBtn_BW.Checked)
                intTransTypeIndex = 0;
            else
                intTransTypeIndex = 1;
            m_smVisionInfo.g_objSealCircleGauges.ref_GaugeTransType = intTransTypeIndex;
            m_blnMeasureGauge = true;
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

            m_smVisionInfo.g_objSealCircleGauges.ref_GaugeTransChoice = intTransChoiceIndex;
            m_blnMeasureGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objSealCircleGauges.SaveCircleGauge(
                       m_strPath,
                       false,
                       "CircleGauge",
                       true,
                       true,
                       false);

            Close();
            Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objSealCircleGauges.LoadCircleGauge(m_strPath,
                            "CircleGauge");

            Close();
            Dispose();
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

            m_smVisionInfo.g_objSealCircleGauges.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
            trackBar_Thickness.Value = m_smVisionInfo.g_objSealCircleGauges.ref_GaugeThickness;
            m_blnMeasureGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_threshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSealCircleGauges.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
            trackBar_Derivative.Value = m_smVisionInfo.g_objSealCircleGauges.ref_GaugeThreshold;
            m_blnMeasureGauge = true;
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

            m_smVisionInfo.g_objSealCircleGauges.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp.Text);
            trackBar_MinAmp.Value = m_smVisionInfo.g_objSealCircleGauges.ref_GaugeMinAmplitude;
            m_blnMeasureGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void trackBar_MinAmp_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MeasMinAmp.Text = trackBar_MinAmp.Value.ToString();
        }

        private void txt_Tolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSealCircleGauges.ref_GaugeTolerance = Convert.ToInt32(txt_Tolerance.Text);
            m_blnMeasureGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SamplingStep_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSealCircleGauges.ref_GaugeSamplingStep = Convert.ToInt32(txt_SamplingStep.Text);
            m_blnMeasureGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSealCircleGauges.ref_GaugeMinArea = Convert.ToInt32(txt_MeasMinArea.Text);
            m_blnMeasureGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringPass_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSealCircleGauges.ref_GaugeFilterPasses = Convert.ToInt32(txt_FilteringPass.Text);
            m_blnMeasureGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringThreshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSealCircleGauges.ref_GaugeFilterThreshold = (float)Convert.ToDouble(txt_FilteringThreshold.Text);
            m_blnMeasureGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasFilter_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSealCircleGauges.ref_GaugeFilter = Convert.ToInt32(txt_MeasFilter.Text);
            m_blnMeasureGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void AdvancedSealCircleGaugeForm_Load(object sender, EventArgs e)
        {
            m_blnShowForm = true;
        }

        private void AdvancedSealCircleGaugeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_blnShowForm = false;
            //m_smVisionInfo.g_blnViewGauge = false;
        }

        private void txt_GaugeScore_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objSealCircleGauges.ref_intMinScore = Convert.ToInt32(txt_GaugeScore.Text);
        }
    }
}
