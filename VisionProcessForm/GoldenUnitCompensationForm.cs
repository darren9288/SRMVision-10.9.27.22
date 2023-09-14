using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using ImageAcquisition;
using VisionProcessing;
using SharedMemory;

namespace VisionProcessForm
{
    public partial class GoldenUnitCompensationForm : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;
        private int m_intGoldenUnitSelectedIndex = 0;
        private int m_intUserGroup = 5;
        private int m_intPadIndex;
        private string m_strSelectedRecipe;

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;

        #endregion

        #region Properties

        public int ref_intGoldenUnitSelectedIndex
        {
            get {
                if (dgd_GoldenSetList.Rows.Count > 0)
                    return dgd_GoldenSetList.CurrentCell.RowIndex;
                else
                    return -1;
                }
        }
        public bool ref_intViewGoldenDataColumn { get { return chk_ViewGoldenColumn.Checked; } set { chk_ViewGoldenColumn.Checked = value; } }

        #endregion 

        #region Delegate

        public delegate void DisplayTaskCompletePercentage(float fPercent);

        #endregion


        public GoldenUnitCompensationForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_intUserGroup = intUserGroup;

            UpdateGUI();

            m_blnInitDone = true;
        }

        public void DisplayPercentage(float fPercent)
        {
            srmLabel3.Text = fPercent.ToString() + "%";
        }

        private void UpdateGUI()
        {
            UpdateGoldenDataList();

            if (dgd_GoldenSetList.Rows.Count == 0)
            {
                chk_ViewGoldenColumn.Checked = false;
                chk_ViewGoldenColumn.Enabled = false;
            }

        }

        private void UpdateGoldenDataList()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad[m_intPadIndex].ref_arrGoldenData.Count; i++)
            {
                if (i >= dgd_GoldenSetList.Rows.Count)
                    dgd_GoldenSetList.Rows.Add();
                dgd_GoldenSetList.Rows[i].Cells[0].Value = "Set " + (i + 1).ToString();
                dgd_GoldenSetList.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_arrGoldenDataUsed[i];
            }

            if (m_intGoldenUnitSelectedIndex < dgd_GoldenSetList.Rows.Count) 
                dgd_GoldenSetList.Rows[m_intGoldenUnitSelectedIndex].Selected = true;
        }

        private void btn_AddGoldenDataSet_Click(object sender, EventArgs e)
        {
            int intRowIndex = dgd_GoldenSetList.Rows.Count;

            dgd_GoldenSetList.Rows.Add();

            dgd_GoldenSetList.Rows[intRowIndex].Cells[0].Value = "Set " + (intRowIndex + 1).ToString();
            dgd_GoldenSetList.Rows[intRowIndex].Cells[1].Value = true;

            m_smVisionInfo.g_arrPad[m_intPadIndex].ref_arrGoldenData.Add(new List<List<float>>());
            m_smVisionInfo.g_arrPad[m_intPadIndex].ref_arrGoldenDataUsed.Add(true);

            chk_ViewGoldenColumn.Enabled = true;
        }

        private void btn_DeleteGoldenDataSet_Click(object sender, EventArgs e)
        {
            int intRowIndex = dgd_GoldenSetList.Rows.Count - 1;

            if (intRowIndex < 0)
                return;

            if (SRMMessageBox.Show("Are you sure you want to delete Golden Set " + dgd_GoldenSetList.Rows.Count.ToString() + "?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                dgd_GoldenSetList.Rows.RemoveAt(intRowIndex);

                m_smVisionInfo.g_arrPad[m_intPadIndex].ref_arrGoldenData.RemoveAt(intRowIndex);

                if (dgd_GoldenSetList.Rows.Count == 0)
                {
                    chk_ViewGoldenColumn.Checked = false;
                    chk_ViewGoldenColumn.Enabled = false;
                }
            }
        }

        public void SetSelectedPadIndex(int intPadIndex)
        {
            m_intPadIndex = intPadIndex;

            UpdateGoldenDataList();
        }

        private void dgd_GoldenSetList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (e.ColumnIndex == 1) // Use Column
            {
                if (e.RowIndex < m_smVisionInfo.g_arrPad[m_intPadIndex].ref_arrGoldenDataUsed.Count)
                    m_smVisionInfo.g_arrPad[m_intPadIndex].ref_arrGoldenDataUsed[e.RowIndex] = Convert.ToBoolean(dgd_GoldenSetList.Rows[e.RowIndex].Cells[1].Value);
            }
        }

        private void btn_CalculateCompensation_Click(object sender, EventArgs e)
        {
            float fOffSetX = 0, fOffSetY = 0;
            m_smVisionInfo.g_arrPad[m_intPadIndex].CalculateResolutionOffSetUsingGoldenData(ref fOffSetX, ref fOffSetY);

            lbl_OffSetX.Text = fOffSetX.ToString();
            lbl_OffSetY.Text = fOffSetY.ToString();
        }

        private void btn_CalculateUsingThreshold_Click(object sender, EventArgs e)
        {
            int intStartResolutionOffSet = -10;
            int intEndResolutionOffSet = 10;
            int intResolutionOffSetInterval = 2;

            ACCURACYINSPECTION:

            int intFailCount;
            List<List<int>> arrPassCountList = new List<List<int>>();

            // Keep Previous Threshold
            int intPadThresholdPrev = m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intThresholdValue;

            // Display waiting GUI
            DisplayTaskCompletePercentage delegate_DisplayPercent = DisplayPercentage;
            delegate_DisplayPercent(0f); 

            // Loop resolution off set from -20 to 20 with interval 5
            for (int intResolutionOffset = intStartResolutionOffSet; intResolutionOffset <= intEndResolutionOffSet; intResolutionOffset += intResolutionOffSetInterval)
            {
                // Update Pad calibration value temporary
                if (m_intPadIndex == 0)
                {
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetCalibrationData(
                                                      m_smVisionInfo.g_fCalibPixelX + intResolutionOffset,
                                                      m_smVisionInfo.g_fCalibPixelY + intResolutionOffset,
                                                      m_smVisionInfo.g_fCalibOffSetX + intResolutionOffset,
                                                      m_smVisionInfo.g_fCalibOffSetY + intResolutionOffset, m_smCustomizeInfo.g_intUnitDisplay);
                }
                else
                {
                    m_smVisionInfo.g_arrPad[m_intPadIndex].SetCalibrationData(
                                                     m_smVisionInfo.g_fCalibPixelZ + intResolutionOffset,
                                                     m_smVisionInfo.g_fCalibPixelZ + intResolutionOffset,
                                                     m_smVisionInfo.g_fCalibOffSetZ + intResolutionOffset,
                                                     m_smVisionInfo.g_fCalibOffSetZ + intResolutionOffset, m_smCustomizeInfo.g_intUnitDisplay);
                }
                // Loop threshold value from 0 to 255 with interval 5
                for (int i = 0; i < 256; i += 3)
                {
                    if (Math.Abs(i - intPadThresholdPrev) > 50)
                        continue;

                    // Update waiting GUI
                    delegate_DisplayPercent((intResolutionOffset * i) / (41 * 256));

                    // Update Pad threshold value
                    m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intThresholdValue = i;

                    // Start manual pad inspection
                    m_smVisionInfo.PR_MN_TestDone = false;
                    m_smVisionInfo.MN_PR_StartTest = true;

                    // Wait manual pad inspection done
                    int intCount = 0;
                    while (!m_smVisionInfo.PR_MN_TestDone)
                    {
                        if (intCount++ > 1000)
                            break;

                        System.Threading.Thread.Sleep(1);
                    }

                    if (!m_smVisionInfo.PR_MN_TestDone)
                    {
                        break;
                    }
                    // Get how many data (Width/height/pitch/gap) fail accuracy (0.0125mm)
                    intFailCount = m_smVisionInfo.g_arrPad[m_intPadIndex].CalculateThresholdUsingGoldenData();

                    if (intFailCount >= 0)
                    {
                        // Record the accuracy fail counter with threshold value and resolution off set value
                        arrPassCountList.Add(new List<int>());
                        arrPassCountList[arrPassCountList.Count - 1].Add(intFailCount);  // Add Pad Count
                        arrPassCountList[arrPassCountList.Count - 1].Add(i);             // Add threshold;
                        arrPassCountList[arrPassCountList.Count - 1].Add(intResolutionOffset);
                    }
                }

                if (!m_smVisionInfo.PR_MN_TestDone)
                {
                    break;
                }
            }

            btn_CalculateUsingThreshold.Text = "Calculate";
            // Set pad to previous threshold and calibration value
            m_smVisionInfo.g_arrPad[m_intPadIndex].ref_intThresholdValue = intPadThresholdPrev;
            if (m_intPadIndex == 0)
            {
                m_smVisionInfo.g_arrPad[m_intPadIndex].SetCalibrationData(
                                      m_smVisionInfo.g_fCalibPixelX,
                                      m_smVisionInfo.g_fCalibPixelY,
                                      m_smVisionInfo.g_fCalibOffSetX,
                                      m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
            }
            else
            {
                m_smVisionInfo.g_arrPad[m_intPadIndex].SetCalibrationData(
                                                 m_smVisionInfo.g_fCalibPixelZ,
                                                 m_smVisionInfo.g_fCalibPixelZ,
                                                 m_smVisionInfo.g_fCalibOffSetZ,
                                                 m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
            }
            // Sort the accuracy fail data. (0 index has less fail data compare to higher index)
            List<List<int>> arrSortedList = new List<List<int>>();
            Math2.SortData(arrPassCountList, 0, Math2.Sorting.Increase, ref arrSortedList);

            if (arrSortedList.Count > 0)
            {
                List<int> arrOptimumThreshold = new List<int>();
                List<float> arrOptimumCalib = new List<float>();

                // Get first sorted index which is the best accuracy data
                arrOptimumThreshold.Add(arrSortedList[0][1]);   
                arrOptimumCalib.Add(arrSortedList[0][2]);

                // Get other sorted data which has same accuracy data with the first index.
                for (int i = 1; i < arrSortedList.Count; i++)
                {
                    if (arrSortedList[i][0] > arrSortedList[i - 1][0])
                        break;

                    arrOptimumThreshold.Add(arrSortedList[i][1]);
                    arrOptimumCalib.Add(arrSortedList[i][2]);
                }

                // Reloop the accuracy inspection test if the accuracy data fail is not 0
                if (arrSortedList[0][0] > 0)
                {
                    int intLowestResolutionOffSet = int.MaxValue;
                    int intHighestResolutionOffSet = int.MinValue;

                    for (int i = 0; i < arrOptimumCalib.Count; i++)
                    {
                        if (intLowestResolutionOffSet > arrOptimumCalib[i])
                            intLowestResolutionOffSet = (int)arrOptimumCalib[i];

                        if (intHighestResolutionOffSet < arrOptimumCalib[i])
                            intHighestResolutionOffSet = (int)arrOptimumCalib[i];
                    }

                    intStartResolutionOffSet = intLowestResolutionOffSet - 5;
                    intEndResolutionOffSet = intHighestResolutionOffSet + 5;
                    intResolutionOffSetInterval = 1;

                    if(intResolutionOffSetInterval != 1)
                        goto ACCURACYINSPECTION;
                }

                string strMessage = "Optimum Calib With Threshold: ";
                for (int i = 0; i < arrOptimumThreshold.Count; i++)
                {
                    strMessage += "[" + arrOptimumCalib[i] + "," + arrOptimumThreshold[i] + "]";

                    if (i == arrOptimumThreshold.Count - 1)
                        strMessage += ".";
                    else
                        strMessage += " ,";
                }

                strMessage += "\nAccuracy Fail Count= " + arrSortedList[0][0].ToString();

                SRMMessageBox.Show(strMessage);
            }
            else
            {
                SRMMessageBox.Show("No Result Found!");
            }

        }
    }
}