using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using SharedMemory;
using Common;
using VisionProcessing;

namespace VisionProcessForm
{
    public partial class PocketPositionMeanStatisticForm : Form
    {
        
        private bool m_bUpdateInfo = false;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private float m_fMinValue = 0;
        private float m_fMaxValue = float.MaxValue;
        private float m_fRange = 0;
        private float m_fCenter = 0;
        private float m_fMean = 0;
        private float m_fTotalSum = 0;
        public PocketPositionMeanStatisticForm(VisionInfo objVisionInfo, ProductionInfo objProductionInfo)
        {
            InitializeComponent();

            m_smVisionInfo = objVisionInfo;
            m_smProductionInfo = objProductionInfo;
            
            lbl_PocketPositionRef.Text = m_smVisionInfo.g_objPocketPosition.ref_intPocketPositionReference.ToString();
        }

        private void btn_StartAnalysis_Click(object sender, EventArgs e)
        {
            if (txt_TotalDataCount.Text == null || txt_TotalDataCount.Text == "0")
            {
                SRMMessageBox.Show("Data Count cannot be zero!");
                return;
            }

            if (m_smVisionInfo.g_bPocketPositionMeanStatisticAnalysisON)
            {
                m_smVisionInfo.g_bPocketPositionMeanStatisticAnalysisON = false;
                btn_StartAnalysis.Text = "Start Analysis";
                txt_TotalDataCount.Enabled = true;
            }
            else
            {
                ResetAllData();
                m_smVisionInfo.g_bPocketPositionMeanStatisticAnalysisON = true;
                btn_StartAnalysis.Text = "Stop Analysis";
                txt_TotalDataCount.Enabled = false;
            }
        }

        private void PocketPositionMeanStatisticForm_Load(object sender, EventArgs e)
        {

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lbl_PocketPositionRef.Text) != m_smVisionInfo.g_objPocketPosition.ref_intPocketPositionReference)
            {
                lbl_PocketPositionRef.Text = m_smVisionInfo.g_objPocketPosition.ref_intPocketPositionReference.ToString();
            }


            if (m_smVisionInfo.g_bPocketPositionMeanStatisticAnalysisUpdateInfo)
            {
                DoStatisticAnalysis();

                m_smVisionInfo.g_bPocketPositionMeanStatisticAnalysisUpdateInfo = false;
            }

            if (m_smVisionInfo.g_intPocketPositionMeanStatisticAnalysisCount == Convert.ToInt32(txt_TotalDataCount.Text))
            {
                if (m_smVisionInfo.g_bPocketPositionMeanStatisticAnalysisON)
                {
                    m_smVisionInfo.g_bPocketPositionMeanStatisticAnalysisON = false;
                    m_smVisionInfo.g_bPocketPositionMeanStatisticAnalysisUpdateInfo = false;
                    btn_StartAnalysis.Text = "Start Analysis";
                    txt_TotalDataCount.Enabled = true;

                    //ResetAllData();
                }
            }

            if (!this.TopMost)
                this.TopMost = true;
        }

        private void PocketPositionMeanStatisticForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pocket Position Statistic Form Closed", "Exit Pocket Position Statistic Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_bPocketPositionMeanStatisticAnalysisON = false;
            m_smVisionInfo.g_bPocketPositionMeanStatisticAnalysisUpdateInfo = false;
            this.Dispose();
            this.Close();
        }

        public void DoStatisticAnalysis()
        {
            lbl_DataCount.Text = m_smVisionInfo.g_intPocketPositionMeanStatisticAnalysisCount.ToString();
            float fObjectOffsetX = m_smVisionInfo.g_objPocketPosition.ref_fResultXDistance;
            fObjectOffsetX = fObjectOffsetX * 1000 / m_smVisionInfo.g_fCalibPixelX; // Convert to micron

            if (Math.Abs(Math.Round(fObjectOffsetX)) > 100000)
                fObjectOffsetX = 0;

            if (m_smVisionInfo.g_intPocketPositionMeanStatisticAnalysisCount == 1)
            {
                m_fMinValue = fObjectOffsetX;
                lbl_Min.Text = fObjectOffsetX.ToString();
                m_fMaxValue = fObjectOffsetX;
                lbl_Max.Text = fObjectOffsetX.ToString();
            }


            if (m_fMinValue > fObjectOffsetX)
            {
                m_fMinValue = fObjectOffsetX;
                lbl_Min.Text = fObjectOffsetX.ToString();
            }

            if (m_fMaxValue < fObjectOffsetX)
            {
                m_fMaxValue = fObjectOffsetX;
                lbl_Max.Text = fObjectOffsetX.ToString();
            }

            lbl_Range.Text = (m_fMaxValue - m_fMinValue).ToString();

            lbl_Center.Text = ((m_fMinValue + m_fMaxValue) / 2).ToString();

            m_fTotalSum += fObjectOffsetX;
            lbl_Mean.Text = (m_fTotalSum / m_smVisionInfo.g_intPocketPositionMeanStatisticAnalysisCount).ToString();
        }

        private void ResetAllData()
        {
            m_smVisionInfo.g_intPocketPositionMeanStatisticAnalysisCount = 0;

            m_fMinValue = 0;
            m_fMaxValue = float.MaxValue;
            m_fRange = 0;
            m_fCenter = 0;
            m_fMean = 0;
            m_fTotalSum = 0;

            lbl_Min.Text = "0";
            lbl_Max.Text = "0";
            lbl_Range.Text = "0";
            lbl_Center.Text = "0";
            lbl_Mean.Text = "0";
            lbl_DataCount.Text = "0";
        }

        private void btn_Set_Click(object sender, EventArgs e)
        {
            if (SRMMessageBox.Show("Are you sure you want to set average as reference?", "" , MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation) == DialogResult.OK)
            {
                string PocketPositionstrFolderPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\PocketPosition\\";
                m_smVisionInfo.g_objPocketPosition.ref_intPocketPositionReference = (int)Math.Round(Convert.ToDouble(lbl_Mean.Text));
                m_smVisionInfo.g_objPocketPosition.SavePocketPosition(PocketPositionstrFolderPath + "Settings.xml", false, "Settings", true);
                lbl_PocketPositionRef.Text = m_smVisionInfo.g_objPocketPosition.ref_intPocketPositionReference.ToString();
            }
        }
    }
}
