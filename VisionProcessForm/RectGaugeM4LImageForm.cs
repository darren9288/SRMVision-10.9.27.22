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
    public partial class RectGaugeM4LImageForm : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        RectGaugeM4L m_RGaugeM4L;

        #endregion

        #region Properties
        #endregion

        public RectGaugeM4LImageForm(VisionInfo smVisionInfo, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo, RectGaugeM4L RGaugeM4L)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_RGaugeM4L = RGaugeM4L;
            UpdateGUI();

            m_blnInitDone = true;
        }

        private void UpdateGUI()
        {
            // limit image no to txt box
            txt_CenterROI_Top.Maximum = txt_CenterROI_Right.Maximum = txt_CenterROI_Bottom.Maximum = txt_CenterROI_Left.Maximum = m_smVisionInfo.g_arrImages.Count;

            txt_CenterROI_Top.Value = Convert.ToDecimal(m_RGaugeM4L.GetGaugeImageNo(0) + 1);
            txt_CenterROI_Right.Value = Convert.ToDecimal(m_RGaugeM4L.GetGaugeImageNo(1) + 1);
            txt_CenterROI_Bottom.Value = Convert.ToDecimal(m_RGaugeM4L.GetGaugeImageNo(2) + 1);
            txt_CenterROI_Left.Value = Convert.ToDecimal(m_RGaugeM4L.GetGaugeImageNo(3) + 1);
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            m_RGaugeM4L.SetGaugeImageNo(Convert.ToInt32(txt_CenterROI_Top.Value) - 1, 0);
            m_RGaugeM4L.SetGaugeImageNo(Convert.ToInt32(txt_CenterROI_Right.Value) - 1, 1);
            m_RGaugeM4L.SetGaugeImageNo(Convert.ToInt32(txt_CenterROI_Bottom.Value) - 1, 2);
            m_RGaugeM4L.SetGaugeImageNo(Convert.ToInt32(txt_CenterROI_Left.Value) - 1, 3);

            this.DialogResult = DialogResult.OK;
            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {

        }
    }
}