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
    public partial class AdvancedPostSealLGaugeForm : Form
    {
        #region Member Variables
        private bool m_blnInitDone = false;
        private bool m_blnShowAdvanceSetting = false;
        private bool m_blnShowForm = false;

        private string m_strPath = "";
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;

        private bool m_blnBuildGauge = false;

        #endregion
        #region Properties

        public bool ref_blnBuildGauge { get { return m_blnBuildGauge; } set { m_blnBuildGauge = value; } }

        public bool ref_blnShowForm { get { return m_blnShowForm; } set { m_blnShowForm = value; } }

        #endregion
        public AdvancedPostSealLGaugeForm(VisionInfo smVisionInfo, string strPath, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strPath = strPath;
            m_smProductionInfo = smProductionInfo;
            UpdateGUI();
            m_blnInitDone = true;

        }

        private void UpdateGUIPicture()
        {
            if (radioBtn_BW.Checked)
            {
                pic_Up.Image = imageList1.Images[0];

                pic_Down.Image = imageList1.Images[2];

            }
            else if (radioBtn_WB.Checked)
            {
                pic_Up.Image = imageList1.Images[4];

                pic_Down.Image = imageList1.Images[6];

            }

        }

        private void UpdateGUI()
        {
            if (m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeTransType == 0)
                radioBtn_BW.Checked = true;
            else
                radioBtn_WB.Checked = true;

            if (m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeTransChoice == 0)
                radioBtn_FromBegin.Checked = true;
            else if (m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeTransChoice == 1)
                radioBtn_FromEnd.Checked = true;
            else
                radioBtn_LargeAmplitude.Checked = true;

            txt_MeasThickness.Text = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeThickness.ToString();
            trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
            txt_MeasFilter.Text = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeFilter.ToString();
            txt_MeasMinAmp.Text = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeMinAmplitude.ToString();
            trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
            txt_MeasMinArea.Text = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeMinArea.ToString();
            txt_FilteringPass.Text = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeFilterPasses.ToString();
            txt_FilteringThreshold.Text = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeFilterThreshold.ToString();
            txt_threshold.Text = m_smVisionInfo.g_arrSealGauges[0][0].ref_GaugeThreshold.ToString();
            trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);

            UpdateGUIPicture();

            this.Size = new Size(this.Size.Width, this.Size.Height - panel_AdvanceSetting.Size.Height);
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            LGauge.SaveFile(m_strPath, m_smVisionInfo.g_arrSealGauges);

            Close();
            Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            LGauge.LoadFile(m_strPath, m_smVisionInfo.g_arrSealGauges, m_smVisionInfo.g_WorldShape);

            Close();
            Dispose();
        }

        private void txt_MeasMinAmp_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrSealGauges.Count; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrSealGauges[i].Count; j++)
                {
                    m_smVisionInfo.g_arrSealGauges[i][j].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp.Text);
                }
            }

            trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
            m_blnBuildGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void trackBar_MinAmp_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MeasMinAmp.Text = trackBar_MinAmp.Value.ToString();
        }

        private void txt_threshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrSealGauges.Count; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrSealGauges[i].Count; j++)
                {
                    m_smVisionInfo.g_arrSealGauges[i][j].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                }
            }

            trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);
            m_blnBuildGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_Derivative_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_threshold.Text = trackBar_Derivative.Value.ToString();
        }

        private void txt_MeasThickness_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrSealGauges.Count; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrSealGauges[i].Count; j++)
                {
                    m_smVisionInfo.g_arrSealGauges[i][j].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                }
            }

            trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
            m_blnBuildGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_Thickness_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MeasThickness.Text = trackBar_Thickness.Value.ToString();
        }

        private void txt_MeasFilter_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrSealGauges.Count; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrSealGauges[i].Count; j++)
                {
                    m_smVisionInfo.g_arrSealGauges[i][j].ref_GaugeFilter = Convert.ToInt32(txt_MeasFilter.Text);
                }
            }
            m_blnBuildGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringThreshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrSealGauges.Count; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrSealGauges[i].Count; j++)
                {
                    m_smVisionInfo.g_arrSealGauges[i][j].ref_GaugeFilterThreshold = (float)Convert.ToDouble(txt_FilteringThreshold.Text);
                }
            }
            m_blnBuildGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringPass_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrSealGauges.Count; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrSealGauges[i].Count; j++)
                {
                    m_smVisionInfo.g_arrSealGauges[i][j].ref_GaugeFilterPasses = Convert.ToInt32(txt_FilteringPass.Text);
                }
            }
            m_blnBuildGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrSealGauges.Count; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrSealGauges[i].Count; j++)
                {
                    m_smVisionInfo.g_arrSealGauges[i][j].ref_GaugeMinArea = Convert.ToInt32(txt_MeasMinArea.Text);
                }
            }
            m_blnBuildGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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

            for (int i = 0; i < m_smVisionInfo.g_arrSealGauges.Count; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrSealGauges[i].Count; j++)
                {
                    m_smVisionInfo.g_arrSealGauges[i][j].ref_GaugeTransType = intTransTypeIndex;
                }
            }
            UpdateGUIPicture();

            m_blnBuildGauge = true;
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

            for (int i = 0; i < m_smVisionInfo.g_arrSealGauges.Count; i++)
            {
                for (int j = 0; j < m_smVisionInfo.g_arrSealGauges[i].Count; j++)
                {
                    m_smVisionInfo.g_arrSealGauges[i][j].ref_GaugeTransChoice = intTransChoiceIndex;
                }
            }
            m_blnBuildGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_ShowAdv_Click(object sender, EventArgs e)
        {
            m_blnShowAdvanceSetting = !m_blnShowAdvanceSetting;

            if (m_blnShowAdvanceSetting)
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    btn_ShowAdv.Text = "Hide Adv.";
                else
                    btn_ShowAdv.Text = "关闭高级";

                panel_AdvanceSetting.Visible = true;
                this.Size = new Size(this.Size.Width, this.Size.Height + panel_AdvanceSetting.Size.Height);
            }
            else
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    btn_ShowAdv.Text = "Show Adv.";
                else
                    btn_ShowAdv.Text = "显示高级";
                panel_AdvanceSetting.Visible = false;
                this.Size = new Size(this.Size.Width, this.Size.Height - panel_AdvanceSetting.Size.Height);
            }
        }

        private void AdvancedPostSealLGaugeForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objSeal.BuildGauge(m_smVisionInfo.g_arrSealROIs, m_smVisionInfo.g_arrSealGauges, m_smVisionInfo.g_fCalibPixelY, true);
            m_blnShowForm = true;
        }

        private void AdvancedPostSealLGaugeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_blnShowForm = false;
            m_smVisionInfo.g_blnViewGauge = false;
        }

    }
}
