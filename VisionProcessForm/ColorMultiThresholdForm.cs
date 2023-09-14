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
    public partial class ColorMultiThresholdForm : Form
    {
        #region Member Variables
        private Point m_pTop, m_pRight, m_pBottom, m_pLeft;
        private int m_intSelectedDontCareROIIndexPrev = 0;
        private int m_intDontCareMode = 0;
        private bool m_blnTriggerOfflineTest = false;
        private bool m_blnInitDone = false;
        private int m_intRowIndexPrev = -1;
        private bool m_blnFormOpened = false;
        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;

        private Graphics m_Graphic;
        private ImageDrawing m_objThresholdImage;
        private ImageDrawing m_objDontCareImage;
        private ROI m_objROI;
        private bool m_blnUpdateImage = false;
        private int m_intSelectedROIPrev = 0;
        #endregion

        public bool ref_blnFormOpened { get { return m_blnFormOpened; } }

        public ColorMultiThresholdForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo)
        {
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_blnFormOpened = true;
            InitializeComponent();
            m_Graphic = Graphics.FromHwnd(pic_ThresholdImage.Handle);

            m_smVisionInfo.g_intViewInspectionSetting = 1;
            cbo_DontCareAreaDrawMethod.SelectedIndex = 0;

            if (m_smVisionInfo.g_arrPad.Length < 2 || !m_smVisionInfo.g_blnCheck4Sides)
                srmGroupBox5.Visible = false; // this.Size = new Size(this.Size.Width, this.Size.Height - 30);
            else
            {
                switch (m_smVisionInfo.g_intSelectedROI)
                {
                    case 0:
                        radioBtn_Middle.Checked = true;
                        break;
                    case 1:
                        radioBtn_Up.Checked = true;
                        break;
                    case 2:
                        radioBtn_Right.Checked = true;
                        break;
                    case 3:
                        radioBtn_Down.Checked = true;
                        break;
                    case 4:
                        radioBtn_Left.Checked = true;
                        break;
                }
            }

            CustomizeGUI();

            if (dgd_ThresholdSetting.RowCount == 0)
                m_smVisionInfo.g_intSelectedColorThresholdIndex = -1;
            else
                m_smVisionInfo.g_intSelectedColorThresholdIndex = 0;

            UpdateROIToleranceGUI();

            m_pTop = new Point(pnl_Top.Location.X, pnl_Top.Location.Y);
            m_pRight = new Point(pnl_Right.Location.X, pnl_Right.Location.Y);
            m_pBottom = new Point(pnl_Bottom.Location.X, pnl_Bottom.Location.Y);
            m_pLeft = new Point(pnl_Left.Location.X, pnl_Left.Location.Y);

            m_blnInitDone = true;
            //m_blnTriggerOfflineTest = true;
            m_smVisionInfo.AT_VM_OfflineTestAllPad = true;
            TriggerOfflineTest();

            System.Threading.Thread.Sleep(200);
            RotateColorImage();
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
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 0 && m_smVisionInfo.g_arrPadROIs[i].Count > 0)
                    m_smVisionInfo.g_arrPadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            }

            m_smVisionInfo.AT_VM_ManualTestMode = true;

            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
        }

        private void CustomizeGUI()
        {
            UpdateThresholdListTable();
            UpdateSelectedThresholdValue(0);
            UpdateLSHColorBar();

            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                {
                    if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[i].Count; j++)
                        {
                            for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
                            {
                                m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                            }
                        }
                    }

                }
            }
        }
        private void UpdateROIToleranceGUI()
        {
            picUnitROI.Image = ils_ImageListTree.Images[m_smVisionInfo.g_intSelectedROI];

            if (m_smVisionInfo.g_intSelectedColorThresholdIndex == -1)
                return;

            switch (m_smVisionInfo.g_intSelectedROI)
            {
                case 0:
                case 1:
                    txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromTop.Text == "0")
                    {
                        txt_StartPixelFromTop.Text = "1";
                        txt_StartPixelFromTop.Text = "0";
                    }
                    txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromRight.Text == "0")
                    {
                        txt_StartPixelFromRight.Text = "1";
                        txt_StartPixelFromRight.Text = "0";
                    }
                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromBottom.Text == "0")
                    {
                        txt_StartPixelFromBottom.Text = "1";
                        txt_StartPixelFromBottom.Text = "0";
                    }
                    txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromLeft.Text == "0")
                    {
                        txt_StartPixelFromLeft.Text = "1";
                        txt_StartPixelFromLeft.Text = "0";
                    }
                    break;
                case 2:
                    txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromTop.Text == "0")
                    {
                        txt_StartPixelFromTop.Text = "1";
                        txt_StartPixelFromTop.Text = "0";
                    }
                    txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromRight.Text == "0")
                    {
                        txt_StartPixelFromRight.Text = "1";
                        txt_StartPixelFromRight.Text = "0";
                    }
                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromBottom.Text == "0")
                    {
                        txt_StartPixelFromBottom.Text = "1";
                        txt_StartPixelFromBottom.Text = "0";
                    }
                    txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromLeft.Text == "0")
                    {
                        txt_StartPixelFromLeft.Text = "1";
                        txt_StartPixelFromLeft.Text = "0";
                    }
                    break;
                case 3:
                    txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromTop.Text == "0")
                    {
                        txt_StartPixelFromTop.Text = "1";
                        txt_StartPixelFromTop.Text = "0";
                    }
                    txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromRight.Text == "0")
                    {
                        txt_StartPixelFromRight.Text = "1";
                        txt_StartPixelFromRight.Text = "0";
                    }
                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromBottom.Text == "0")
                    {
                        txt_StartPixelFromBottom.Text = "1";
                        txt_StartPixelFromBottom.Text = "0";
                    }
                    txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromLeft.Text == "0")
                    {
                        txt_StartPixelFromLeft.Text = "1";
                        txt_StartPixelFromLeft.Text = "0";
                    }
                    break;
                case 4:
                    txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromTop.Text == "0")
                    {
                        txt_StartPixelFromTop.Text = "1";
                        txt_StartPixelFromTop.Text = "0";
                    }
                    txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromRight.Text == "0")
                    {
                        txt_StartPixelFromRight.Text = "1";
                        txt_StartPixelFromRight.Text = "0";
                    }
                    txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromBottom.Text == "0")
                    {
                        txt_StartPixelFromBottom.Text = "1";
                        txt_StartPixelFromBottom.Text = "0";
                    }
                    txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
                    if (txt_StartPixelFromLeft.Text == "0")
                    {
                        txt_StartPixelFromLeft.Text = "1";
                        txt_StartPixelFromLeft.Text = "0";
                    }
                    break;
            }
        }
        private void UpdateImageNoColumnItem(int intRowIndex)
        {

            // Get Total Image View Count
            int intViewImageCount = CImageDrawing.GetImageViewCount(m_smVisionInfo.g_intVisionIndex);

            // Update Image No combo box
            (dgd_ThresholdSetting.Rows[intRowIndex].Cells[11] as DataGridViewComboBoxCell).Items.Clear();
            for (int i = 0; i < intViewImageCount; i++)
            {
                (dgd_ThresholdSetting.Rows[intRowIndex].Cells[11] as DataGridViewComboBoxCell).Items.Add("Image " + (i + 1).ToString());
            }
            dgd_ThresholdSetting.Rows[intRowIndex].Cells[11].Value = (dgd_ThresholdSetting.Rows[intRowIndex].Cells[11] as DataGridViewComboBoxCell).Items[0];
            m_smVisionInfo.g_intSelectedImage = 0;

        }

        private void UpdateLSHColorBar()
        {
            if ((m_intRowIndexPrev < 0) || (m_intRowIndexPrev >= dgd_ThresholdSetting.Rows.Count))
                return;

            // Find the selected row system color type 
            bool blnHSL;
            if (dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[2].Value.ToString() == "RGB")
                blnHSL = false;
            else
                blnHSL = true;

            if (blnHSL)
            {
                int intMinHue = (int)trackBar_Value1.Value - (int)txt_Value1Tolerance.Value;
                if (intMinHue < 0)
                    intMinHue = 0;
                int intMaxHue = (int)trackBar_Value1.Value + (int)txt_Value1Tolerance.Value;
                if (intMaxHue > 255)
                    intMaxHue = 255;
                pic_Hue.Image = ColorProcessing.DrawHueColor(intMinHue, intMaxHue, pic_Hue.Height);

                int intMinSaturation = (int)trackBar_Value2.Value - (int)txt_Value2Tolerance.Value;
                if (intMinSaturation < 0)
                    intMinSaturation = 0;
                int intMaxSaturation = (int)trackBar_Value2.Value + (int)txt_Value2Tolerance.Value;
                if (intMaxSaturation > 255)
                    intMaxSaturation = 255;
                pic_Saturation.Image = ColorProcessing.DrawSaturationColor(intMinSaturation, intMaxSaturation, trackBar_Value1.Value, pic_Saturation.Height);

                int intMinLightness = (int)trackBar_Value3.Value - (int)txt_Value3Tolerance.Value;
                if (intMinLightness < 0)
                    intMinLightness = 0;
                int intMaxLightness = (int)trackBar_Value3.Value + (int)txt_Value3Tolerance.Value;
                if (intMaxLightness > 255)
                    intMaxLightness = 255;
                pic_Lightness.Image = ColorProcessing.DrawLightnessColor(intMinLightness, intMaxLightness, trackBar_Value1.Value, trackBar_Value2.Value, pic_Lightness.Height);
            }
            else
            {
                int intMinRed = (int)trackBar_Value1.Value - (int)txt_Value1Tolerance.Value;
                if (intMinRed < 0)
                    intMinRed = 0;
                int intMaxRed = (int)trackBar_Value1.Value + (int)txt_Value1Tolerance.Value;
                if (intMaxRed > 255)
                    intMaxRed = 255;
                pic_Hue.Image = ColorProcessing.DrawRedColor(intMinRed, intMaxRed, pic_Hue.Height);

                int intMinGreen = (int)trackBar_Value2.Value - (int)txt_Value2Tolerance.Value;
                if (intMinGreen < 0)
                    intMinGreen = 0;
                int intMaxGreen = (int)trackBar_Value2.Value + (int)txt_Value2Tolerance.Value;
                if (intMaxGreen > 255)
                    intMaxGreen = 255;
                pic_Saturation.Image = ColorProcessing.DrawGreenColor(intMinGreen, intMaxGreen, pic_Saturation.Height);

                int intMinBlue = (int)trackBar_Value3.Value - (int)txt_Value3Tolerance.Value;
                if (intMinBlue < 0)
                    intMinBlue = 0;
                int intMaxBlue = (int)trackBar_Value3.Value + (int)txt_Value3Tolerance.Value;
                if (intMaxBlue > 255)
                    intMaxBlue = 255;
                pic_Lightness.Image = ColorProcessing.DrawBlueColor(intMinBlue, intMaxBlue, pic_Lightness.Height);
            }
        }

        private void UpdateSelectedThresholdValue(int intRowIndex)
        {
            if (intRowIndex >= dgd_ThresholdSetting.Rows.Count)
                return;

            m_intRowIndexPrev = intRowIndex;

            dgd_ThresholdSetting.Rows[intRowIndex].Selected = true;

            //txt_ThresholdName.Text = dgd_ThresholdSetting.Rows[intRowIndex].Cells[1].Value.ToString();

            if (dgd_ThresholdSetting.Rows[intRowIndex].Cells[2].Value.ToString() == "HSL")
            {
                group_LSHSetting.Text = "HSL";
                lbl_Hue.Text = "Hue";
                lbl_Saturation.Text = "Saturation";
                lbl_Lightness.Text = "Lightness";
                group_LSHSetting.Visible = true;
                group_ThresholdSetting.Visible = false;
                group_ImageProcessingSetting.Location = new Point(group_ImageProcessingSetting.Location.X, group_LSHSetting.Location.Y + group_LSHSetting.Size.Height);
                m_smVisionInfo.g_intColorFormat = 0;
            }
            else if (dgd_ThresholdSetting.Rows[intRowIndex].Cells[2].Value.ToString() == "RGB")
            {
                group_LSHSetting.Text = "RGB";
                lbl_Hue.Text = "Red";
                lbl_Saturation.Text = "Green";
                lbl_Lightness.Text = "Blue";
                group_LSHSetting.Visible = true;
                group_ThresholdSetting.Visible = false;
                group_ImageProcessingSetting.Location = new Point(group_ImageProcessingSetting.Location.X, group_LSHSetting.Location.Y + group_LSHSetting.Size.Height);
                m_smVisionInfo.g_intColorFormat = 1;
            }
            else
            {
                group_LSHSetting.Visible = false;
                group_ThresholdSetting.Visible = true;
                group_ImageProcessingSetting.Location = new Point(group_ImageProcessingSetting.Location.X, group_ThresholdSetting.Location.Y + group_ThresholdSetting.Size.Height);
                m_smVisionInfo.g_intColorFormat = 2;
            }

            // UpdateSelectedThresholdSetting
            if (dgd_ThresholdSetting.Rows[intRowIndex].Cells[2].Value.ToString() == "RGB")
            {
                m_smVisionInfo.g_intColorThreshold[0] = Convert.ToInt32(dgd_ThresholdSetting.Rows[intRowIndex].Cells[3].Value);
                m_smVisionInfo.g_intColorThreshold[1] = Convert.ToInt32(dgd_ThresholdSetting.Rows[intRowIndex].Cells[5].Value);
                m_smVisionInfo.g_intColorThreshold[2] = Convert.ToInt32(dgd_ThresholdSetting.Rows[intRowIndex].Cells[7].Value);

                m_smVisionInfo.g_intColorTolerance[0] = Convert.ToInt32(dgd_ThresholdSetting.Rows[intRowIndex].Cells[4].Value);
                m_smVisionInfo.g_intColorTolerance[1] = Convert.ToInt32(dgd_ThresholdSetting.Rows[intRowIndex].Cells[6].Value);
                m_smVisionInfo.g_intColorTolerance[2] = Convert.ToInt32(dgd_ThresholdSetting.Rows[intRowIndex].Cells[8].Value);

                txt_Value1.Text = m_smVisionInfo.g_intColorThreshold[0].ToString();
                txt_Value2.Text = m_smVisionInfo.g_intColorThreshold[1].ToString();
                txt_Value3.Text = m_smVisionInfo.g_intColorThreshold[2].ToString();

                trackBar_Value1.Value = m_smVisionInfo.g_intColorThreshold[0];
                trackBar_Value2.Value = m_smVisionInfo.g_intColorThreshold[1];
                trackBar_Value3.Value = m_smVisionInfo.g_intColorThreshold[2];

                txt_Value1Tolerance.Value = m_smVisionInfo.g_intColorTolerance[0];
                txt_Value2Tolerance.Value = m_smVisionInfo.g_intColorTolerance[1];
                txt_Value3Tolerance.Value = m_smVisionInfo.g_intColorTolerance[2];
            }
            else if (dgd_ThresholdSetting.Rows[intRowIndex].Cells[2].Value.ToString() == "HSL")
            {
                m_smVisionInfo.g_intColorThreshold[2] = Convert.ToInt32(dgd_ThresholdSetting.Rows[intRowIndex].Cells[3].Value);
                m_smVisionInfo.g_intColorThreshold[1] = Convert.ToInt32(dgd_ThresholdSetting.Rows[intRowIndex].Cells[5].Value);
                m_smVisionInfo.g_intColorThreshold[0] = Convert.ToInt32(dgd_ThresholdSetting.Rows[intRowIndex].Cells[7].Value);

                m_smVisionInfo.g_intColorTolerance[2] = Convert.ToInt32(dgd_ThresholdSetting.Rows[intRowIndex].Cells[4].Value);
                m_smVisionInfo.g_intColorTolerance[1] = Convert.ToInt32(dgd_ThresholdSetting.Rows[intRowIndex].Cells[6].Value);
                m_smVisionInfo.g_intColorTolerance[0] = Convert.ToInt32(dgd_ThresholdSetting.Rows[intRowIndex].Cells[8].Value);

                txt_Value1.Text = m_smVisionInfo.g_intColorThreshold[2].ToString();
                txt_Value2.Text = m_smVisionInfo.g_intColorThreshold[1].ToString();
                txt_Value3.Text = m_smVisionInfo.g_intColorThreshold[0].ToString();

                trackBar_Value1.Value = m_smVisionInfo.g_intColorThreshold[2];
                trackBar_Value2.Value = m_smVisionInfo.g_intColorThreshold[1];
                trackBar_Value3.Value = m_smVisionInfo.g_intColorThreshold[0];

                txt_Value1Tolerance.Value = m_smVisionInfo.g_intColorTolerance[2];
                txt_Value2Tolerance.Value = m_smVisionInfo.g_intColorTolerance[1];
                txt_Value3Tolerance.Value = m_smVisionInfo.g_intColorTolerance[0];
            }
            else
            {
                txt_Threshold.Text = dgd_ThresholdSetting.Rows[intRowIndex].Cells[3].Value.ToString();
                m_smVisionInfo.g_intColorThreshold[0] = trackBar_Threshold.Value = Convert.ToInt32(txt_Threshold.Text);
            }

            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectCloseIteration.Count > intRowIndex)
                txt_CloseIteration.Value = m_smVisionInfo.g_intColorCloseIteration = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectCloseIteration[intRowIndex];
            else
                txt_CloseIteration.Value = m_smVisionInfo.g_intColorCloseIteration = 0;

            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectInvertBlackWhite.Count > intRowIndex)
                chk_InvertBlackWhite.Checked = m_smVisionInfo.g_blnColorInvertBlackWhite = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectInvertBlackWhite[intRowIndex];
            else
                chk_InvertBlackWhite.Checked = m_smVisionInfo.g_blnColorInvertBlackWhite = false;

            //txt_MinArea.Text = dgd_ThresholdSetting.Rows[intRowIndex].Cells[9].Value.ToString();
            UpdateColorSystemGUI((dgd_ThresholdSetting.Rows[intRowIndex].Cells[2] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[intRowIndex].Cells[2].EditedFormattedValue) == 2, intRowIndex);

            bool blnUpdateImage = true;

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex((dgd_ThresholdSetting.Rows[intRowIndex].Cells[11] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[intRowIndex].Cells[11].EditedFormattedValue), m_smVisionInfo.g_intVisionIndex); ;

            if (blnUpdateImage)
            {
                m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                RotateColorImage();
            }
        }

        private void UpdateRowsNo()
        {
            for (int i = 0; i < dgd_ThresholdSetting.Rows.Count; i++)
            {
                dgd_ThresholdSetting.Rows[i].Cells[0].Value = (i + 1).ToString();
            }

        }
        private void UpdateColorSystemGUI(bool blnSaturation, int intRowIndex)
        {
            if (blnSaturation)
            {
                for (int i = 4; i < 9; i++)
                {
                    dgd_ThresholdSetting.Rows[intRowIndex].Cells[i].ReadOnly = blnSaturation;
                    dgd_ThresholdSetting.Rows[intRowIndex].Cells[i].Style.BackColor = Color.DarkGray;
                    dgd_ThresholdSetting.Rows[intRowIndex].Cells[i].Style.SelectionBackColor = Color.DarkGray;
                    dgd_ThresholdSetting.Rows[intRowIndex].Cells[i].Style.ForeColor = Color.DarkGray;
                    dgd_ThresholdSetting.Rows[intRowIndex].Cells[i].Style.SelectionForeColor = Color.DarkGray;
                }
                dgd_ThresholdSetting.Rows[intRowIndex].Cells[10].Value = (dgd_ThresholdSetting.Rows[intRowIndex].Cells[10] as DataGridViewComboBoxCell).Items[0];
                dgd_ThresholdSetting.Rows[intRowIndex].Cells[10].ReadOnly = blnSaturation;
            }
            else
            {
                for (int i = 4; i < 9; i++)
                {
                    dgd_ThresholdSetting.Rows[intRowIndex].Cells[i].ReadOnly = blnSaturation;
                    dgd_ThresholdSetting.Rows[intRowIndex].Cells[i].Style.BackColor = SystemColors.Window;
                    dgd_ThresholdSetting.Rows[intRowIndex].Cells[i].Style.SelectionBackColor = SystemColors.GradientInactiveCaption;
                    dgd_ThresholdSetting.Rows[intRowIndex].Cells[i].Style.ForeColor = SystemColors.ControlText;
                    dgd_ThresholdSetting.Rows[intRowIndex].Cells[i].Style.SelectionForeColor = Color.Black;
                }
                dgd_ThresholdSetting.Rows[intRowIndex].Cells[10].ReadOnly = blnSaturation;
            }

            if (srmTabControl1.SelectedIndex == 0)
            {
                if (blnSaturation)
                    label1.Visible = false;
                else
                    label1.Visible = true;
            }
        }

        private void UpdateThresholdListTable()
        {
            dgd_ThresholdSetting.Rows.Clear();

            for (int i = 0; i < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor.Count; i++)
            {
                dgd_ThresholdSetting.Rows.Add();
                UpdateImageNoColumnItem(i);
                dgd_ThresholdSetting.Rows[i].Cells[0].Value = (i + 1);
                dgd_ThresholdSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorThresName[i];
                dgd_ThresholdSetting.Rows[i].Cells[2].Value = (dgd_ThresholdSetting.Rows[i].Cells[2] as DataGridViewComboBoxCell).Items[m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorSystem[i]];
                if (dgd_ThresholdSetting.Rows[i].Cells[2].Value.ToString() == "RGB")
                {
                    dgd_ThresholdSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[i][0];
                    dgd_ThresholdSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[i][0];
                    dgd_ThresholdSetting.Rows[i].Cells[5].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[i][1];
                    dgd_ThresholdSetting.Rows[i].Cells[6].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[i][1];
                    dgd_ThresholdSetting.Rows[i].Cells[7].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[i][2];
                    dgd_ThresholdSetting.Rows[i].Cells[8].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[i][2];
                }
                else if (dgd_ThresholdSetting.Rows[i].Cells[2].Value.ToString() == "HSL")
                {
                    dgd_ThresholdSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[i][2];
                    dgd_ThresholdSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[i][2];
                    dgd_ThresholdSetting.Rows[i].Cells[5].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[i][1];
                    dgd_ThresholdSetting.Rows[i].Cells[6].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[i][1];
                    dgd_ThresholdSetting.Rows[i].Cells[7].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[i][0];
                    dgd_ThresholdSetting.Rows[i].Cells[8].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[i][0];
                }
                else
                {
                    dgd_ThresholdSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[i][0];
                    dgd_ThresholdSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[i][0];
                    dgd_ThresholdSetting.Rows[i].Cells[5].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[i][1];
                    dgd_ThresholdSetting.Rows[i].Cells[6].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[i][1];
                    dgd_ThresholdSetting.Rows[i].Cells[7].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[i][2];
                    dgd_ThresholdSetting.Rows[i].Cells[8].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[i][2];
                }
                dgd_ThresholdSetting.Rows[i].Cells[9].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorMinArea[i];
                dgd_ThresholdSetting.Rows[i].Cells[10].Value = (dgd_ThresholdSetting.Rows[i].Cells[10] as DataGridViewComboBoxCell).Items[m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectType[i]];
                dgd_ThresholdSetting.Rows[i].Cells[11].Value = (dgd_ThresholdSetting.Rows[i].Cells[11] as DataGridViewComboBoxCell).Items[m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectImageNo[i]];
                dgd_ThresholdSetting.Rows[i].Cells[12].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectCloseIteration[i];
                dgd_ThresholdSetting.Rows[i].Cells[13].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectInvertBlackWhite[i];
                UpdateColorSystemGUI((dgd_ThresholdSetting.Rows[i].Cells[2] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[2].EditedFormattedValue) == 2, i);
                if (i == 0)
                {
                    switch (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode[i])
                    {
                        case 0:
                            radioButton_NoneDontCare.Checked = true;
                            m_intDontCareMode = 0;
                            pnl_DrawMethod.Visible = false;
                            break;
                        case 1:
                            radioButton_PadDontCare.Checked = true;
                            m_intDontCareMode = 1;
                            pnl_DrawMethod.Visible = false;
                            break;
                        case 2:
                            radioButton_PackageDontCare.Checked = true;
                            m_intDontCareMode = 2;
                            pnl_DrawMethod.Visible = false;
                            break;
                        case 3:
                            radioButton_ManualDontCare.Checked = true;
                            m_intDontCareMode = 3;
                            pnl_DrawMethod.Visible = true;
                            break;
                    }


                }
            }
        }

        private void UpdateChangedDataToTableList()
        {
            if ((m_intRowIndexPrev < 0) || (m_intRowIndexPrev >= dgd_ThresholdSetting.Rows.Count))
                return;

            //dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[1].Value = txt_ThresholdName.Text;
            dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[3].Value = trackBar_Value1.Value;
            dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[5].Value = trackBar_Value2.Value;
            dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[7].Value = trackBar_Value3.Value;

            dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[4].Value = txt_Value1Tolerance.Value;
            dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[6].Value = txt_Value2Tolerance.Value;
            dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[8].Value = txt_Value3Tolerance.Value;
            //dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[9].Value = txt_MinArea.Text;
            //dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[12].Value = txt_CloseIteration.Value;
            //dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[13].Value = chk_InvertBlackWhite.Checked;
        }

        private bool CheckIsDataChanged()
        {
            if ((m_intRowIndexPrev < 0) || (m_intRowIndexPrev >= dgd_ThresholdSetting.Rows.Count))
                return false;

            if (dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[1].Value.ToString() != txt_ThresholdName.Text)
                return true;

            if (Convert.ToInt32(dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[3].Value) != Convert.ToInt32(txt_Value1.Text))
                return true;

            if (Convert.ToInt32(dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[5].Value) != Convert.ToInt32(txt_Value2.Text))
                return true;

            if (Convert.ToInt32(dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[7].Value) != Convert.ToInt32(txt_Value3.Text))
                return true;

            if (Convert.ToInt32(dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[4].Value) != Convert.ToInt32(txt_Value1Tolerance.Text))
                return true;

            if (Convert.ToInt32(dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[6].Value) != Convert.ToInt32(txt_Value2Tolerance.Text))
                return true;

            if (Convert.ToInt32(dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[8].Value) != Convert.ToInt32(txt_Value3Tolerance.Text))
                return true;

            if (Convert.ToInt32(dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[9].Value) != Convert.ToInt32(txt_MinArea.Text))
                return true;

            return false;
        }

        private void trackBar_Hue_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Value1.Text = trackBar_Value1.Value.ToString();
        }

        private void trackBar_Lightness_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Value3.Text = trackBar_Value3.Value.ToString();
        }

        private void trackBar_Saturation_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Value2.Text = trackBar_Value2.Value.ToString();
        }




        private void txt_Hue_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intValue = Convert.ToInt32(txt_Value1.Text);
            if (intValue < 0)
                intValue = 0;

            if (m_smVisionInfo.g_intColorFormat == 1)
                m_smVisionInfo.g_intColorThreshold[0] = trackBar_Value1.Value = intValue;
            else if (m_smVisionInfo.g_intColorFormat == 0)
                m_smVisionInfo.g_intColorThreshold[2] = trackBar_Value1.Value = intValue;
            UpdateLSHColorBar();
            UpdateChangedDataToTableList();
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_HueTolerance_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_smVisionInfo.g_intColorFormat == 1)
                m_smVisionInfo.g_intColorTolerance[0] = (int)txt_Value1Tolerance.Value;
            else if (m_smVisionInfo.g_intColorFormat == 0)
                m_smVisionInfo.g_intColorTolerance[2] = (int)txt_Value1Tolerance.Value;
            UpdateLSHColorBar();
            UpdateChangedDataToTableList();
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Lightness_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intValue = Convert.ToInt32(txt_Value3.Text);
            if (intValue < 0)
                intValue = 0;
            if (m_smVisionInfo.g_intColorFormat == 1)
                m_smVisionInfo.g_intColorThreshold[2] = trackBar_Value3.Value = intValue;
            else if (m_smVisionInfo.g_intColorFormat == 0)
                m_smVisionInfo.g_intColorThreshold[0] = trackBar_Value3.Value = intValue;
            UpdateLSHColorBar();
            UpdateChangedDataToTableList();
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void txt_LightnessTolerance_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_smVisionInfo.g_intColorFormat == 1)
                m_smVisionInfo.g_intColorTolerance[2] = (int)txt_Value3Tolerance.Value;
            else if (m_smVisionInfo.g_intColorFormat == 0)
                m_smVisionInfo.g_intColorTolerance[0] = (int)txt_Value3Tolerance.Value;
            UpdateLSHColorBar();
            UpdateChangedDataToTableList();
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Saturation_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intValue = Convert.ToInt32(txt_Value2.Text);
            if (intValue < 0)
                intValue = 0;

            m_smVisionInfo.g_intColorThreshold[1] = trackBar_Value2.Value = intValue;
            UpdateLSHColorBar();
            UpdateChangedDataToTableList();
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SaturationTolerance_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intColorTolerance[1] = (int)txt_Value2Tolerance.Value;
            UpdateLSHColorBar();
            UpdateChangedDataToTableList();
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            //m_smVisionInfo.g_intColorThreshold = m_intLSHPrev;
            //m_smVisionInfo.g_intColorTolerance = m_intLSHTolerancePrev;
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                // Load Pad Template Setting
                string strSectionName = "";
                if (i == 0)
                    strSectionName = "CenterROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                m_smVisionInfo.g_arrPad[i].LoadColorPadOnly(strFolderPath + "\\Pad\\Template\\Template.xml", strSectionName);
            }

            LoadROISetting(strFolderPath + "\\Pad\\ColorDontCareROI.xml", m_smVisionInfo.g_arrPadColorDontCareROIs, m_smVisionInfo.g_arrPad.Length);
            for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
                {
                    if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[i].Count; j++)
                        {
                            for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
                            {
                                m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
                            }
                        }
                    }

                }
            }
            Polygon.LoadPolygon(strFolderPath + "\\Pad\\Template\\ColorPolygon.xml", m_smVisionInfo.g_arrPolygon_PadColor, m_smVisionInfo.g_arrPad.Length);

            m_blnFormOpened = false;
            this.Close();
            this.Dispose();
        }
        private bool CheckSameDefectNameImageNo(ref string strErrorMessage)
        {
            bool blnResult = true;
            //List<string> arrDefectName = new List<string>();
            //List<int> arrImageNo = new List<int>();
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                List<int> arrSkipNo = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName.Count; j++)
                {
                    if (arrSkipNo.Contains(j))
                        continue;

                    int intDefectImageNo = m_smVisionInfo.g_arrPad[i].ref_arrDefectImageNo[j];
                    string strDefectName = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j];

                    for (int k = 0; k < m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName.Count; k++)
                    {
                        if (j == k || arrSkipNo.Contains(k))
                            continue;

                        int intDefectImageNo2 = m_smVisionInfo.g_arrPad[i].ref_arrDefectImageNo[k];
                        string strDefectName2 = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[k];
                        if (strDefectName == strDefectName2 && intDefectImageNo != intDefectImageNo2)
                        {
                            blnResult = false;
                            if (i == 0)
                                strErrorMessage = "\t Center ROI - " + strDefectName;
                            else if (i == 1)
                                strErrorMessage = "\t Top ROI - " + strDefectName;
                            else if (i == 2)
                                strErrorMessage = "\t Right ROI - " + strDefectName;
                            else if (i == 3)
                                strErrorMessage = "\t Bottom ROI - " + strDefectName;
                            else if (i == 4)
                                strErrorMessage = "\t Left ROI - " + strDefectName;
                            break;
                        }
                        else if (strDefectName == strDefectName2)
                        {
                            if (!arrSkipNo.Contains(k))
                                arrSkipNo.Add(k);
                        }
                    }

                    if (!blnResult)
                        break;
                }

                if (!blnResult)
                    break;
            }
            return blnResult;
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            //m_intLSHPrev = m_smVisionInfo.g_intColorThreshold;
            //m_intLSHTolerancePrev = m_smVisionInfo.g_intColorTolerance;

            //if (CheckIsDataChanged())
            //{
            //    if (SRMMessageBox.Show("Do you want to update the changed threshold value into table?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //    {
            //        UpdateChangedDataToTableList();
            //    }
            //}

            string strErrorMessage = "";
            if (!CheckSameDefectNameImageNo(ref strErrorMessage))
            {
                SRMMessageBox.Show("Same defect name must use same image number :\n" + strErrorMessage,
                                  "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<int> arrDontCareMode = new List<int>();
            for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode.Count; j++)
                arrDontCareMode.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode[j]);

            List<int> arrROITop = new List<int>();
            for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Top.Count; j++)
                arrROITop.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Top[j]);

            List<int> arrROIRight = new List<int>();
            for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Right.Count; j++)
                arrROIRight.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Right[j]);

            List<int> arrROIBottom = new List<int>();
            for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Bottom.Count; j++)
                arrROIBottom.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Bottom[j]);

            List<int> arrROILeft = new List<int>();
            for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Left.Count; j++)
                arrROILeft.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Left[j]);

            List<int> arrFailCondition = new List<int>();
            for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionFailCondition.Count; j++)
                arrFailCondition.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionFailCondition[j]);

            List<float> arrWidthSetting = new List<float>();
            for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionWidth.Count; j++)
                arrWidthSetting.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionWidth[j]);

            List<float> arrLengthSetting = new List<float>();
            for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionLength.Count; j++)
                arrLengthSetting.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionLength[j]);

            List<float> arrMinAreaSetting = new List<float>();
            for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionMinArea.Count; j++)
                arrMinAreaSetting.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionMinArea[j]);

            List<float> arrMaxAreaSetting = new List<float>();
            for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionMaxArea.Count; j++)
                arrMaxAreaSetting.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionMaxArea[j]);

            List<float> arrTotalAreaSetting = new List<float>();
            for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionTotalArea.Count; j++)
                arrTotalAreaSetting.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionTotalArea[j]);

            m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ResetColorThresholdData();
            for (int i = 0; i < dgd_ThresholdSetting.Rows.Count; i++)
            {
                int intDontCareMode = 0;
                if (arrDontCareMode.Count > i)
                    intDontCareMode = arrDontCareMode[i];

                int intROITop = 0;
                if (arrROITop.Count > i)
                    intROITop = arrROITop[i];

                int intROIRight = 0;
                if (arrROIRight.Count > i)
                    intROIRight = arrROIRight[i];

                int intROIBottom = 0;
                if (arrROIBottom.Count > i)
                    intROIBottom = arrROIBottom[i];

                int intROILeft = 0;
                if (arrROILeft.Count > i)
                    intROILeft = arrROILeft[i];

                int intFailCondition = 0;
                if (arrFailCondition.Count > i)
                    intFailCondition = arrFailCondition[i];

                float fWidth = 0;
                if (arrWidthSetting.Count > i)
                    fWidth = arrWidthSetting[i];

                float fLength = 0;
                if (arrLengthSetting.Count > i)
                    fLength = arrLengthSetting[i];

                float fMinArea = 0;
                if (arrMinAreaSetting.Count > i)
                    fMinArea = arrMinAreaSetting[i];

                float fMaxArea = 0;
                if (arrMaxAreaSetting.Count > i)
                    fMaxArea = arrMaxAreaSetting[i];

                float fTotalArea = 0;
                if (arrTotalAreaSetting.Count > i)
                    fTotalArea = arrTotalAreaSetting[i];

                int intColorSystem = (dgd_ThresholdSetting.Rows[i].Cells[2] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[2].EditedFormattedValue);
                int intDefectType = (dgd_ThresholdSetting.Rows[i].Cells[10] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[10].EditedFormattedValue);
                int intDefectImageNo = (dgd_ThresholdSetting.Rows[i].Cells[11] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[11].EditedFormattedValue);
                if (dgd_ThresholdSetting.Rows[i].Cells[2].Value.ToString() == "RGB")
                {
                    m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].AddColorThresholdData(
                        Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem,//Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                            fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                        intROITop, intROIRight, intROIBottom, intROILeft);
                }
                else if (dgd_ThresholdSetting.Rows[i].Cells[2].Value.ToString() == "HSL")
                {
                    m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].AddColorThresholdData(
                        Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem, //Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                            fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                        intROITop, intROIRight, intROIBottom, intROILeft);
                }
                else
                {
                    m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].AddColorThresholdData(
                        Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem, //Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                            fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                        intROITop, intROIRight, intROIBottom, intROILeft);
                }
            }

            for (int a = 0; a < m_smVisionInfo.g_arrPad.Length; a++)
            {
                if (a > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (a == m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrPad[a].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                    continue;

                arrDontCareMode = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectDontCareMode.Count; j++)
                    arrDontCareMode.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectDontCareMode[j]);

                arrROITop = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Top.Count; j++)
                    arrROITop.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Top[j]);

                arrROIRight = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Right.Count; j++)
                    arrROIRight.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Right[j]);

                arrROIBottom = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Bottom.Count; j++)
                    arrROIBottom.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Bottom[j]);

                arrROILeft = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Left.Count; j++)
                    arrROILeft.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Left[j]);

                arrFailCondition = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionFailCondition.Count; j++)
                    arrFailCondition.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionFailCondition[j]);

                arrWidthSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionWidth.Count; j++)
                    arrWidthSetting.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionWidth[j]);

                arrLengthSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionLength.Count; j++)
                    arrLengthSetting.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionLength[j]);

                arrMinAreaSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionMinArea.Count; j++)
                    arrMinAreaSetting.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionMinArea[j]);

                arrMaxAreaSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionMaxArea.Count; j++)
                    arrMaxAreaSetting.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionMaxArea[j]);

                arrTotalAreaSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionTotalArea.Count; j++)
                    arrTotalAreaSetting.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionTotalArea[j]);

                m_smVisionInfo.g_arrPad[a].ResetColorThresholdData();

                for (int i = 0; i < dgd_ThresholdSetting.Rows.Count; i++)
                {
                    int intDontCareMode = 0;
                    if (arrDontCareMode.Count > i)
                        intDontCareMode = arrDontCareMode[i];

                    int intROITop = 0;
                    if (arrROITop.Count > i)
                        intROITop = arrROITop[i];

                    int intROIRight = 0;
                    if (arrROIRight.Count > i)
                        intROIRight = arrROIRight[i];

                    int intROIBottom = 0;
                    if (arrROIBottom.Count > i)
                        intROIBottom = arrROIBottom[i];

                    int intROILeft = 0;
                    if (arrROILeft.Count > i)
                        intROILeft = arrROILeft[i];

                    int intFailCondition = 0;
                    if (arrFailCondition.Count > i)
                        intFailCondition = arrFailCondition[i];

                    float fWidth = 0;
                    if (arrWidthSetting.Count > i)
                        fWidth = arrWidthSetting[i];

                    float fLength = 0;
                    if (arrLengthSetting.Count > i)
                        fLength = arrLengthSetting[i];

                    float fMinArea = 0;
                    if (arrMinAreaSetting.Count > i)
                        fMinArea = arrMinAreaSetting[i];

                    float fMaxArea = 0;
                    if (arrMaxAreaSetting.Count > i)
                        fMaxArea = arrMaxAreaSetting[i];

                    float fTotalArea = 0;
                    if (arrTotalAreaSetting.Count > i)
                        fTotalArea = arrTotalAreaSetting[i];

                    int intColorSystem = (dgd_ThresholdSetting.Rows[i].Cells[2] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[2].EditedFormattedValue);
                    int intDefectType = (dgd_ThresholdSetting.Rows[i].Cells[10] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[10].EditedFormattedValue);
                    int intDefectImageNo = (dgd_ThresholdSetting.Rows[i].Cells[11] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[11].EditedFormattedValue);
                    if (dgd_ThresholdSetting.Rows[i].Cells[2].Value.ToString() == "RGB")
                    {
                        m_smVisionInfo.g_arrPad[a].AddColorThresholdData(
                            Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem, //Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                            fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                            intROITop, intROIRight, intROIBottom, intROILeft);
                    }
                    else if (dgd_ThresholdSetting.Rows[i].Cells[2].Value.ToString() == "HSL")
                    {
                        m_smVisionInfo.g_arrPad[a].AddColorThresholdData(
                            Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem, //Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                            fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                            intROITop, intROIRight, intROIBottom, intROILeft);
                    }
                    else
                    {
                        m_smVisionInfo.g_arrPad[a].AddColorThresholdData(
                            Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem, //Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                            fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                            intROITop, intROIRight, intROIBottom, intROILeft);
                    }
                }
            }


            //for (int i = 0; i < m_smVisionInfo.g_arrPadColorDontCareROIs.Count; i++)
            //{
            //    for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[i].Count; j++)
            //    {
            //        for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
            //        {
            //            if (m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].ref_intFormMode != 2)
            //            {
            //                m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].AddPoint(new PointF(m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROITotalX * m_smVisionInfo.g_fScaleX,
            //                    m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROITotalY * m_smVisionInfo.g_fScaleY));
            //                m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].AddPoint(new PointF((m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROITotalX + m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROIWidth) * m_smVisionInfo.g_fScaleX,
            //                    (m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROITotalY + m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROIHeight) * m_smVisionInfo.g_fScaleY));

            //                m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].AddPolygon((int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX), (int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY));
            //            }
            //            else
            //            {
            //                m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].ResetPointsUsingOffset(m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROICenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROICenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
            //                m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].AddPolygon((int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale), (int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale));
            //            }

            //            //ImageDrawing objImage = new ImageDrawing(true);
            //            //if (m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].ref_intFormMode != 2)
            //            //    Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_PadColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
            //            //else
            //            //    Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_PadColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);

            //            //objImage.Dispose();


            //        }
            //    }
            //}

            //string strPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_strRecipeID + "\\" +
            //              m_smVisionInfo.g_strVisionFolderName + "\\Pad\\";
            //SaveDontCareROISetting(strPath);
            //SaveDontCareSetting(strPath + "Template\\");

            //LoadROISetting(strPath + "ColorDontCareROI.xml", m_smVisionInfo.g_arrPadColorDontCareROIs, m_smVisionInfo.g_arrPad.Length);
            //for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            //{
            //    if (m_smVisionInfo.g_arrPadROIs[i].Count > 3)
            //    {
            //        if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > i)
            //        {
            //            for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[i].Count; j++)
            //            {
            //                for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
            //                {
            //                    m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].AttachImage(m_smVisionInfo.g_arrPadROIs[i][3]);
            //                }
            //            }
            //        }

            //    }
            //}

            CopyToleranceAndOptionToSameDefectName_Center();
            if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
                CopyToleranceAndOptionToSameDefectName_Side();

            m_blnFormOpened = false;
            this.Close();
            this.Dispose();
        }
        private void CopyToleranceAndOptionToSameDefectName_Center()
        {
            List<int> arrSkipNo = new List<int>();
            for (int i = 0; i < m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName.Count; i++)
            {
                if (arrSkipNo.Contains(i))
                    continue;

                string strDefectName = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i].ToString();

                for (int j = 0; j < m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName.Count; j++)
                {
                    if (i == j || arrSkipNo.Contains(j))
                        continue;

                    string strDefectName2 = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[j].ToString();
                    if (strDefectName == strDefectName2)
                    {
                        if (!arrSkipNo.Contains(j))
                            arrSkipNo.Add(j);

                        m_smVisionInfo.g_arrPad[0].ref_arrDefectColorInspectionFailCondition[j] = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorInspectionFailCondition[i];
                        m_smVisionInfo.g_arrPad[0].ref_arrDefectColorInspectionWidth[j] = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorInspectionWidth[i];
                        m_smVisionInfo.g_arrPad[0].ref_arrDefectColorInspectionLength[j] = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorInspectionLength[i];
                        m_smVisionInfo.g_arrPad[0].ref_arrDefectColorInspectionMinArea[j] = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorInspectionMinArea[i];
                        m_smVisionInfo.g_arrPad[0].ref_arrDefectColorInspectionMaxArea[j] = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorInspectionMaxArea[i];
                        m_smVisionInfo.g_arrPad[0].ref_arrDefectColorInspectionTotalArea[j] = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorInspectionTotalArea[i];

                        if (i == 0)
                        {
                            if ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x01) > 0)
                            {
                                if (j == 1)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x04;
                                else if (j == 2)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x10;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x100;
                            }
                            else
                            {
                                if (j == 1)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x04;
                                else if (j == 2)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x10;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x100;
                            }

                            if ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x02) > 0)
                            {
                                if (j == 1)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x08;
                                else if (j == 2)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x20;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x200;
                            }
                            else
                            {
                                if (j == 1)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x08;
                                else if (j == 2)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x20;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x200;
                            }

                            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x01) > 0)
                            {
                                if (j == 1)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x04;
                                else if (j == 2)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x10;
                                else if (j == 3)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                            }
                            else
                            {
                                if (j == 1)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x04;
                                else if (j == 2)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x10;
                                else if (j == 3)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
                            }

                            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x02) > 0)
                            {
                                if (j == 1)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x08;
                                else if (j == 2)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x20;
                                else if (j == 3)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                            }
                            else
                            {
                                if (j == 1)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x08;
                                else if (j == 2)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x20;
                                else if (j == 3)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
                            }
                        }
                        else if (i == 1)
                        {
                            if ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x04) > 0)
                            {
                                if (j == 2)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x10;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x100;
                            }
                            else
                            {
                                if (j == 2)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x10;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x100;
                            }

                            if ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x08) > 0)
                            {
                                if (j == 2)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x20;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x200;
                            }
                            else
                            {
                                if (j == 2)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x20;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x200;
                            }

                            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x04) > 0)
                            {
                                if (j == 2)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x10;
                                else if (j == 3)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                            }
                            else
                            {
                                if (j == 2)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x10;
                                else if (j == 3)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
                            }

                            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x08) > 0)
                            {
                                if (j == 2)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x20;
                                else if (j == 3)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                            }
                            else
                            {
                                if (j == 2)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x20;
                                else if (j == 3)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
                            }
                        }
                        else if (i == 2)
                        {
                            if ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x10) > 0)
                            {
                                if (j == 3)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x100;
                            }
                            else
                            {
                                if (j == 3)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x100;
                            }

                            if ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x20) > 0)
                            {
                                if (j == 3)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x200;
                            }
                            else
                            {
                                if (j == 3)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x200;
                            }

                            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x10) > 0)
                            {
                                if (j == 3)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                            }
                            else
                            {
                                if (j == 3)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
                            }

                            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x20) > 0)
                            {
                                if (j == 3)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                            }
                            else
                            {
                                if (j == 3)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
                            }
                        }
                        else if (i == 3)
                        {
                            if ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x40) > 0)
                            {
                                if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x100;
                            }
                            else
                            {
                                if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x100;
                            }

                            if ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x80) > 0)
                            {
                                if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask |= 0x200;
                            }
                            else
                            {
                                if (j == 4)
                                    m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask &= ~0x200;
                            }

                            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x40) > 0)
                            {
                                if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                            }
                            else
                            {
                                if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
                            }

                            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x80) > 0)
                            {
                                if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                            }
                            else
                            {
                                if (j == 4)
                                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
                            }
                        }
                    }
                }
            }
        }
        private void CopyToleranceAndOptionToSameDefectName_Side()
        {
            for (int a = 1; a < m_smVisionInfo.g_arrPad.Length; a++)
            {
                List<int> arrSkipNo = new List<int>();
                for (int i = 0; i < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorThresName.Count; i++)
                {
                    if (arrSkipNo.Contains(i))
                        continue;

                    string strDefectName = m_smVisionInfo.g_arrPad[a].ref_arrDefectColorThresName[i].ToString();

                    for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorThresName.Count; j++)
                    {
                        if (i == j || arrSkipNo.Contains(j))
                            continue;

                        string strDefectName2 = m_smVisionInfo.g_arrPad[a].ref_arrDefectColorThresName[j].ToString();
                        if (strDefectName == strDefectName2)
                        {
                            if (!arrSkipNo.Contains(j))
                                arrSkipNo.Add(j);

                            m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionFailCondition[j] = m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionFailCondition[i];
                            m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionWidth[j] = m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionWidth[i];
                            m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionLength[j] = m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionLength[i];
                            m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionMinArea[j] = m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionMinArea[i];
                            m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionMaxArea[j] = m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionMaxArea[i];
                            m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionTotalArea[j] = m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionTotalArea[i];

                            if (i == 0)
                            {
                                if ((m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask & 0x01) > 0)
                                {
                                    if (j == 1)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x04;
                                    else if (j == 2)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x10;
                                    else if (j == 3)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x40;
                                    else if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x100;
                                }
                                else
                                {
                                    if (j == 1)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x04;
                                    else if (j == 2)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x10;
                                    else if (j == 3)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x40;
                                    else if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x100;
                                }

                                if ((m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask & 0x02) > 0)
                                {
                                    if (j == 1)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x08;
                                    else if (j == 2)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x20;
                                    else if (j == 3)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x80;
                                    else if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x200;
                                }
                                else
                                {
                                    if (j == 1)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x08;
                                    else if (j == 2)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x20;
                                    else if (j == 3)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x80;
                                    else if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x200;
                                }

                                if (a == 1)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask2 & 0x1000) > 0)
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x4000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x4000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask2 & 0x2000) > 0)
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x8000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x8000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x200000;
                                    }
                                }
                                else if (a == 2)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask3 & 0x1000) > 0)
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x4000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x4000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask3 & 0x2000) > 0)
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x8000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x8000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x200000;
                                    }
                                }
                                else if (a == 3)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask4 & 0x1000) > 0)
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x4000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x4000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask4 & 0x2000) > 0)
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x8000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x8000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x200000;
                                    }
                                }
                                else if (a == 4)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask5 & 0x1000) > 0)
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x4000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x4000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask5 & 0x2000) > 0)
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x8000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 1)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x8000;
                                        else if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x200000;
                                    }
                                }

                            }
                            else if (i == 1)
                            {
                                if ((m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask & 0x04) > 0)
                                {
                                    if (j == 2)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x10;
                                    else if (j == 3)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x40;
                                    else if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x100;
                                }
                                else
                                {
                                    if (j == 2)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x10;
                                    else if (j == 3)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x40;
                                    else if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x100;
                                }

                                if ((m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask & 0x08) > 0)
                                {
                                    if (j == 2)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x20;
                                    else if (j == 3)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x80;
                                    else if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x200;
                                }
                                else
                                {
                                    if (j == 2)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x20;
                                    else if (j == 3)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x80;
                                    else if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x200;
                                }

                                if (a == 1)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask2 & 0x4000) > 0)
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask2 & 0x8000) > 0)
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x200000;
                                    }
                                }
                                else if (a == 2)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask3 & 0x4000) > 0)
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask3 & 0x8000) > 0)
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x200000;
                                    }
                                }
                                else if (a == 3)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask4 & 0x4000) > 0)
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask4 & 0x8000) > 0)
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x200000;
                                    }
                                }
                                else if (a == 4)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask5 & 0x4000) > 0)
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x10000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask5 & 0x8000) > 0)
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 2)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x20000;
                                        else if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x200000;
                                    }
                                }
                            }
                            else if (i == 2)
                            {
                                if ((m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask & 0x10) > 0)
                                {
                                    if (j == 3)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x40;
                                    else if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x100;
                                }
                                else
                                {
                                    if (j == 3)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x40;
                                    else if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x100;
                                }

                                if ((m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask & 0x20) > 0)
                                {
                                    if (j == 3)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x80;
                                    else if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x200;
                                }
                                else
                                {
                                    if (j == 3)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x80;
                                    else if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x200;
                                }

                                if (a == 1)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask2 & 0x10000) > 0)
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask2 & 0x20000) > 0)
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x200000;
                                    }
                                }
                                else if (a == 2)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask3 & 0x10000) > 0)
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask3 & 0x20000) > 0)
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x200000;
                                    }
                                }
                                else if (a == 3)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask4 & 0x10000) > 0)
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask4 & 0x20000) > 0)
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x200000;
                                    }
                                }
                                else if (a == 4)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask5 & 0x10000) > 0)
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x40000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask5 & 0x20000) > 0)
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 3)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x80000;
                                        else if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x200000;
                                    }
                                }
                            }
                            else if (i == 3)
                            {
                                if ((m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask & 0x40) > 0)
                                {
                                    if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x100;
                                }
                                else
                                {
                                    if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x100;
                                }

                                if ((m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask & 0x80) > 0)
                                {
                                    if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask |= 0x200;
                                }
                                else
                                {
                                    if (j == 4)
                                        m_smVisionInfo.g_arrPad[a].ref_intFailColorOptionMask &= ~0x200;
                                }

                                if (a == 1)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask2 & 0x40000) > 0)
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask2 & 0x80000) > 0)
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask2 &= ~0x200000;
                                    }
                                }
                                else if (a == 2)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask3 & 0x40000) > 0)
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask3 & 0x80000) > 0)
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask3 &= ~0x200000;
                                    }
                                }
                                else if (a == 3)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask4 & 0x40000) > 0)
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask4 & 0x80000) > 0)
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask4 &= ~0x200000;
                                    }
                                }
                                else if (a == 4)
                                {
                                    if ((m_smVisionInfo.g_intOptionControlMask5 & 0x40000) > 0)
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x100000;
                                    }
                                    else
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x100000;
                                    }

                                    if ((m_smVisionInfo.g_intOptionControlMask5 & 0x80000) > 0)
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 |= 0x200000;
                                    }
                                    else
                                    {
                                        if (j == 4)
                                            m_smVisionInfo.g_intOptionControlMask5 &= ~0x200000;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private void ProduceDontCareImage()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPadColorDontCareROIs.Count; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[i].Count; j++)
                {
                    if (j != m_smVisionInfo.g_intSelectedColorThresholdIndex)
                        continue;
                    if (m_objDontCareImage == null)
                        m_objDontCareImage = new ImageDrawing(true, m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIHeight);
                    m_smVisionInfo.g_objBlackImage.CopyTo(m_objDontCareImage);

                    for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
                    {
                        //if(k==0)
                        m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].ClearPolygon();
                        if (m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].ref_intFormMode != 2)
                        {
                            m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].AddPoint(new PointF(m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROITotalX * m_smVisionInfo.g_fScaleX,
                                m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROITotalY * m_smVisionInfo.g_fScaleY));
                            m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].AddPoint(new PointF((m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROITotalX + m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROIWidth) * m_smVisionInfo.g_fScaleX,
                                (m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROITotalY + m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROIHeight) * m_smVisionInfo.g_fScaleY));

                            m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].AddPolygon((int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX), (int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY));
                        }
                        else
                        {
                            m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].ResetPointsUsingOffset(m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROICenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_arrPadColorDontCareROIs[i][j][k].ref_ROICenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                            m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].AddPolygon((int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale), (int)(m_smVisionInfo.g_arrPadROIs[i][3].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale));
                        }

                        ImageDrawing objImage = new ImageDrawing(true);
                        if (m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].ref_intFormMode != 2)
                            Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_PadColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        else
                            Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_PadColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);

                        ImageDrawing.AddTwoImageTogether(ref objImage, ref m_objDontCareImage);
                        //ROI objDontCareROI = new ROI();
                        //objDontCareROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[i][3].ref_ROIHeight);
                        //objDontCareROI.AttachImage(objImage);
                        //ROI.SubtractROI(m_smVisionInfo.g_arrPadROIs[i][3], objDontCareROI);
                        //objDontCareROI.Dispose();
                        objImage.Dispose();


                    }
                    //for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
                    //    m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].ClearPolygon();
                }
            }

        }

        private void SaveDontCareROISetting(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "ColorDontCareROI.xml", false);

            for (int t = 0; t < m_smVisionInfo.g_arrPadColorDontCareROIs.Count; t++)
            {
                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[t].Count; j++)
                {
                    objFile.WriteElement1Value("Color Defect" + t + j, "");

                    for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[t][j].Count; k++)
                    {
                        objFile.WriteElement2Value("ROI" + k, "");

                        objFile.WriteElement3Value("Name", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_strROIName);
                        objFile.WriteElement3Value("Type", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_intType);
                        objFile.WriteElement3Value("PositionX", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_ROIPositionX);
                        objFile.WriteElement3Value("PositionY", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_ROIPositionY);
                        objFile.WriteElement3Value("Width", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_ROIWidth);
                        objFile.WriteElement3Value("Height", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_ROIHeight);
                        objFile.WriteElement3Value("StartOffsetX", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_intStartOffsetX);
                        objFile.WriteElement3Value("StartOffsetY", m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].ref_intStartOffsetY);

                        m_smVisionInfo.g_arrPadColorDontCareROIs[t][j][k].AttachImage(m_smVisionInfo.g_arrPadROIs[t][3]);

                    }
                }
            }
            objFile.WriteEndElement();


        }
        private void SaveDontCareSetting(string strFolderPath)
        {
            ImageDrawing objImage = new ImageDrawing();
            ImageDrawing objFinalImage = new ImageDrawing();
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {

                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                //Check is the ROI for drawing dont care changed
                //m_smVisionInfo.g_arrPolygon_Pad[i][0].CheckDontCarePosition(m_smVisionInfo.g_arrPadROIs[i][2], m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                for (int j = 0; j < m_smVisionInfo.g_arrPadColorDontCareROIs[i].Count; j++)
                {
                    m_smVisionInfo.g_objBlackImage.CopyTo(objFinalImage);

                    for (int k = 0; k < m_smVisionInfo.g_arrPadColorDontCareROIs[i][j].Count; k++)
                    {
                        //Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_Pad[i][j], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        if (m_smVisionInfo.g_arrPolygon_PadColor[i][j][k].ref_intFormMode != 2)
                            Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_PadColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        else
                            Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPadROIs[i][3], m_smVisionInfo.g_arrPolygon_PadColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                        ImageDrawing.AddTwoImageTogether(ref objImage, ref objFinalImage);
                    }
                    objFinalImage.SaveImage(strFolderPath + "ColorDontCareImage" + i.ToString() + "_" + j.ToString() + ".bmp");
                }


            }
            objImage.Dispose();
            objFinalImage.Dispose();
            Polygon.SavePolygon(strFolderPath + "ColorPolygon.xml", m_smVisionInfo.g_arrPolygon_PadColor);
        }
        private void LoadROISetting(string strPath, List<List<List<ROI>>> arrROIs, int intROICount)
        {
            arrROIs.Clear();

            XmlParser objFile = new XmlParser(strPath);
            int intChildCount;
            ROI objROI;

            for (int i = 0; i < intROICount; i++)
            {
                arrROIs.Add(new List<List<ROI>>());
                objFile.GetFirstSection("Unit" + i);
                intChildCount = objFile.GetSecondSectionCount();
                for (int j = 0; j < intChildCount; j++)
                {
                    arrROIs[i].Add(new List<ROI>());
                    objFile.GetSecondSection("Color Defect" + i + j);
                    int intThirdChildCount = objFile.GetThirdSectionCount();
                    for (int k = 0; k < intThirdChildCount; k++)
                    {
                        objROI = new ROI();
                        objFile.GetThirdSection("ROI" + k);
                        objROI.ref_strROIName = objFile.GetValueAsString("Name", "", 3);
                        objROI.ref_intType = objFile.GetValueAsInt("Type", 4, 3);
                        objROI.ref_ROIPositionX = objFile.GetValueAsInt("PositionX", 0, 3);
                        objROI.ref_ROIPositionY = objFile.GetValueAsInt("PositionY", 0, 3);
                        objROI.ref_ROIWidth = objFile.GetValueAsInt("Width", 100, 3);
                        objROI.ref_ROIHeight = objFile.GetValueAsInt("Height", 100, 3);
                        objROI.ref_intStartOffsetX = objFile.GetValueAsInt("StartOffsetX", 0, 3);
                        objROI.ref_intStartOffsetY = objFile.GetValueAsInt("StartOffsetY", 0, 3);
                        objROI.SetROIPixelAverage(objFile.GetValueAsFloat("AreaPixel", 100.0f, 3));
                        if (objROI.ref_intType > 1)
                        {
                            objROI.ref_ROIOriPositionX = objROI.ref_ROIPositionX;
                            objROI.ref_ROIOriPositionY = objROI.ref_ROIPositionY;
                        }

                        arrROIs[i][j].Add(objROI);
                    }
                }
            }
        }
        private void chk_Preview_Click(object sender, EventArgs e)
        {
            if (chk_Preview.Checked)
            {
                m_smVisionInfo.g_blnViewColorThreshold = true;
            }
            else
            {
                m_smVisionInfo.g_blnViewColorThreshold = false;
            }
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }





        private void timer_Tick(object sender, EventArgs e)
        {
            srmTabControl1.Enabled = dgd_ThresholdSetting.RowCount != 0;
            if (m_smVisionInfo.g_intColorFormat == 1)
            {
                if (m_smVisionInfo.g_intColorThreshold[0] >= 0 && trackBar_Value1.Value != m_smVisionInfo.g_intColorThreshold[0])
                    txt_Value1.Text = m_smVisionInfo.g_intColorThreshold[0].ToString();

                if (m_smVisionInfo.g_intColorThreshold[1] >= 0 && trackBar_Value2.Value != m_smVisionInfo.g_intColorThreshold[1])
                    txt_Value2.Text = m_smVisionInfo.g_intColorThreshold[1].ToString();

                if (m_smVisionInfo.g_intColorThreshold[2] >= 0 && trackBar_Value3.Value != m_smVisionInfo.g_intColorThreshold[2])
                    txt_Value3.Text = m_smVisionInfo.g_intColorThreshold[2].ToString();
            }
            else if (m_smVisionInfo.g_intColorFormat == 0)
            {
                if (m_smVisionInfo.g_intColorThreshold[0] >= 0 && trackBar_Value3.Value != m_smVisionInfo.g_intColorThreshold[0])
                    txt_Value3.Text = m_smVisionInfo.g_intColorThreshold[0].ToString();

                if (m_smVisionInfo.g_intColorThreshold[1] >= 0 && trackBar_Value2.Value != m_smVisionInfo.g_intColorThreshold[1])
                    txt_Value2.Text = m_smVisionInfo.g_intColorThreshold[1].ToString();

                if (m_smVisionInfo.g_intColorThreshold[2] >= 0 && trackBar_Value1.Value != m_smVisionInfo.g_intColorThreshold[2])
                    txt_Value1.Text = m_smVisionInfo.g_intColorThreshold[2].ToString();
            }
            else
            {
                if (m_smVisionInfo.g_intColorThreshold[0] >= 0 && trackBar_Threshold.Value != m_smVisionInfo.g_intColorThreshold[0])
                    txt_Threshold.Text = m_smVisionInfo.g_intColorThreshold[0].ToString();
            }

            if (m_blnUpdateImage || srmTabControl1.SelectedTab == tp_ROITolerance || (srmTabControl1.SelectedTab == tp_DontCare && m_smVisionInfo.g_blnDrawFreeShapeDone))
            {
                m_blnUpdateImage = false;
                try
                {
                    if (dgd_ThresholdSetting.RowCount > 0)
                    {
                        if (m_smVisionInfo.g_arrPadROIs != null && m_smVisionInfo.g_arrPadROIs.Count > m_smVisionInfo.g_intSelectedROI && m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI].Count > 3)
                        {
                            if (m_objThresholdImage == null)
                                m_objThresholdImage = new ImageDrawing(true, m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIHeight);
                            if (m_objDontCareImage == null)
                                m_objDontCareImage = new ImageDrawing(true, m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIHeight);
                            m_objDontCareImage.SetImageToBlack();

                            if (m_objROI == null)
                                m_objROI = new ROI();
                            m_objROI.AttachImage(m_objThresholdImage);
                            m_objROI.LoadROISetting(m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROITotalX,
                                m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROITotalY,
                                m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIWidth,
                                m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIHeight);

                            ProduceDontCareImage();
                            //m_objDontCareImage.SaveImage("D:\\TS\\DontCare.bmp");
                            //m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].CopyImage(ref m_objROI);
                            m_smVisionInfo.g_arrPadColorROIs[m_smVisionInfo.g_intSelectedROI][0].ThresholdTo_ROIToImage(ref m_objThresholdImage, m_smVisionInfo.g_intColorThreshold, m_smVisionInfo.g_intColorTolerance, m_smVisionInfo.g_intColorFormat, m_smVisionInfo.g_intColorCloseIteration, m_smVisionInfo.g_blnColorInvertBlackWhite);
                            m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                            m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].SubtractColorDontCareImage(m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3], m_objROI, m_intDontCareMode, m_objDontCareImage);

                            //m_objROI.LoadROISetting(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].GetColorDefectInspection_Left(m_smVisionInfo.g_intSelectedColorThresholdIndex),
                            //    m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].GetColorDefectInspection_Top(m_smVisionInfo.g_intSelectedColorThresholdIndex),
                            //    m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIWidth - m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].GetColorDefectInspection_Left(m_smVisionInfo.g_intSelectedColorThresholdIndex) - m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].GetColorDefectInspection_Right(m_smVisionInfo.g_intSelectedColorThresholdIndex),
                            //    m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIHeight - m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].GetColorDefectInspection_Top(m_smVisionInfo.g_intSelectedColorThresholdIndex) - m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].GetColorDefectInspection_Bottom(m_smVisionInfo.g_intSelectedColorThresholdIndex));

                            int Max = Math.Max((int)(m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIWidth), (int)(m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIHeight));
                            //m_Graphic.Clear(pic_ThresholdImage.BackColor);
                            //m_objThresholdImage.RedrawImage(m_Graphic, pic_ThresholdImage.Size.Width / Max2, pic_ThresholdImage.Size.Height / Max2);

                            pic_ThresholdImage.Size = new Size(Math.Min((int)Math.Ceiling((250f / Max) * (m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIWidth)), 250),
                                Math.Min((int)Math.Ceiling((250f / Max) * (m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIHeight)), 250));

                            m_objThresholdImage.SetImageSize(m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIWidth, m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIHeight);
                            panel1.Height = Math.Min((int)Math.Ceiling((250f / Max) * (m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3].ref_ROIHeight)), 250);

                            m_objThresholdImage.RedrawImage(m_Graphic, 250f / Max, 250f / Max);

                            pic_ThresholdImage.Location = new Point(panel1.Size.Width / 2 - pic_ThresholdImage.Width / 2, panel1.Size.Height / 2 - pic_ThresholdImage.Height / 2);

                        }
                    }
                }
                catch { }
            }

            if (m_intSelectedDontCareROIIndexPrev != m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;
                if (m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex && m_smVisionInfo.g_intSelectedColorThresholdIndex != -1)
                {
                    if ((m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex) && (m_smVisionInfo.g_intSelectedDontCareROIIndex != -1))
                    {
                        cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
                    }
                    else
                        cbo_DontCareAreaDrawMethod.SelectedIndex = 0;
                }
                else
                    cbo_DontCareAreaDrawMethod.SelectedIndex = 0;

                if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
                {
                    btn_Undo.Visible = true;
                    pic_Help.Visible = true;
                }
                else
                {
                    btn_Undo.Visible = false;
                    pic_Help.Visible = false;
                }
            }
            //if (m_smVisionInfo.g_blnUpdateSelectedROI)
            //{
            //    m_smVisionInfo.g_blnUpdateSelectedROI = false;
            //    m_blnInitDone = false;

            //    switch (m_smVisionInfo.g_intSelectedROI)
            //    {
            //        case 0:
            //        case 1:
            //            pnl_Top.Location = m_pTop;
            //            pnl_Right.Location = m_pRight;
            //            pnl_Bottom.Location = m_pBottom;
            //            pnl_Left.Location = m_pLeft;
            //            break;
            //        case 2:
            //            pnl_Top.Location = m_pRight;
            //            pnl_Right.Location = m_pBottom;
            //            pnl_Bottom.Location = m_pLeft;
            //            pnl_Left.Location = m_pTop;
            //            break;
            //        case 3:
            //            pnl_Top.Location = m_pBottom;
            //            pnl_Right.Location = m_pLeft;
            //            pnl_Bottom.Location = m_pTop;
            //            pnl_Left.Location = m_pRight;
            //            break;
            //        case 4:
            //            pnl_Top.Location = m_pLeft;
            //            pnl_Right.Location = m_pTop;
            //            pnl_Bottom.Location = m_pRight;
            //            pnl_Left.Location = m_pBottom;
            //            break;
            //    }

            //    if (m_smVisionInfo.g_intSelectedROI != m_intSelectedROIPrev)
            //    {
            //        m_intSelectedROIPrev = m_smVisionInfo.g_intSelectedROI;

            //        CustomizeGUI();

            //        if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor.Count == 0)
            //            m_smVisionInfo.g_intSelectedColorThresholdIndex = -1;
            //        else
            //            m_smVisionInfo.g_intSelectedColorThresholdIndex = 0;
            //    }

            //    UpdateROIToleranceGUI();

            //    m_blnInitDone = true;
            //    m_blnUpdateImage = true;
            //}
        }




        private void ColorThresholdForm_Load(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedROI
                && (m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > 0)
                && m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][0].Count > 0)
            {
                cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][0][0].ref_intFormMode;
            }
            else
                cbo_DontCareAreaDrawMethod.SelectedIndex = 0;

            if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
            {
                btn_Undo.Visible = true;
                pic_Help.Visible = true;
            }
            else
            {
                btn_Undo.Visible = false;
                pic_Help.Visible = false;
            }

            m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;

            //m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnGetPixel = true;
            m_smVisionInfo.g_blnViewColorThreshold = true;
            m_smVisionInfo.g_blnColorThresholdForm = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_blnUpdateImage = true;
        }

        private void ColorThresholdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_blnViewColorPackageStartPixelFromEdge = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_intSelectedDontCareROIIndex = 0;
            m_smVisionInfo.g_intSelectedColorThresholdIndex = -1;
            //m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.g_intViewInspectionSetting = 0;
            m_smVisionInfo.g_blnGetPixel = false;
            m_smVisionInfo.g_blnViewColorThreshold = false;
            m_smVisionInfo.g_blnColorThresholdForm = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_AddThreshold_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            //if (CheckIsDataChanged())
            //{
            //    if (SRMMessageBox.Show("Do you want to update the changed threshold value into table?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //    {
            //        UpdateChangedDataToTableList();
            //    }
            //}

            if (dgd_ThresholdSetting.Rows.Count >= 5)
            {
                SRMMessageBox.Show("Maximum 5 set of thresholds can be set only.");
                return;
            }
            else
            {
                int index = dgd_ThresholdSetting.Rows.Count;
                dgd_ThresholdSetting.Rows.Add();
                UpdateImageNoColumnItem(index);
                dgd_ThresholdSetting.Rows[index].Cells[0].Value = dgd_ThresholdSetting.Rows.Count.ToString();
                dgd_ThresholdSetting.Rows[index].Cells[1].Value = "Color Defect " + (index + 1).ToString();

                //if (radioBtn_HSL.Checked)
                dgd_ThresholdSetting.Rows[index].Cells[2].Value = (dgd_ThresholdSetting.Rows[index].Cells[2] as DataGridViewComboBoxCell).Items[0];
                //else
                //    dgd_ThresholdSetting.Rows[index].Cells[2].Value = "RGB";
                dgd_ThresholdSetting.Rows[index].Cells[3].Value = "125";
                dgd_ThresholdSetting.Rows[index].Cells[4].Value = "10";
                dgd_ThresholdSetting.Rows[index].Cells[5].Value = "125";
                dgd_ThresholdSetting.Rows[index].Cells[6].Value = "10";
                dgd_ThresholdSetting.Rows[index].Cells[7].Value = "125";
                dgd_ThresholdSetting.Rows[index].Cells[8].Value = "10";
                dgd_ThresholdSetting.Rows[index].Cells[9].Value = "30";
                dgd_ThresholdSetting.Rows[index].Cells[10].Value = (dgd_ThresholdSetting.Rows[index].Cells[10] as DataGridViewComboBoxCell).Items[0];
                dgd_ThresholdSetting.Rows[index].Cells[11].Value = (dgd_ThresholdSetting.Rows[index].Cells[11] as DataGridViewComboBoxCell).Items[0];
                dgd_ThresholdSetting.Rows[index].Cells[12].Value = "0";
                dgd_ThresholdSetting.Rows[index].Cells[13].Value = false;

                dgd_ThresholdSetting.Rows[index].Selected = true;

                UpdateRowsNo();

                UpdateSelectedThresholdValue(index);

                List<int> arrDontCareMode = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode.Count; j++)
                    arrDontCareMode.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode[j]);

                List<int> arrROITop = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Top.Count; j++)
                    arrROITop.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Top[j]);

                List<int> arrROIRight = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Right.Count; j++)
                    arrROIRight.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Right[j]);

                List<int> arrROIBottom = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Bottom.Count; j++)
                    arrROIBottom.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Bottom[j]);

                List<int> arrROILeft = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Left.Count; j++)
                    arrROILeft.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspection_Left[j]);

                List<int> arrFailCondition = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionFailCondition.Count; j++)
                    arrFailCondition.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionFailCondition[j]);

                List<float> arrWidthSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionWidth.Count; j++)
                    arrWidthSetting.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionWidth[j]);

                List<float> arrLengthSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionLength.Count; j++)
                    arrLengthSetting.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionLength[j]);

                List<float> arrMinAreaSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionMinArea.Count; j++)
                    arrMinAreaSetting.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionMinArea[j]);

                List<float> arrMaxAreaSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionMaxArea.Count; j++)
                    arrMaxAreaSetting.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionMaxArea[j]);

                List<float> arrTotalAreaSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionTotalArea.Count; j++)
                    arrTotalAreaSetting.Add(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorInspectionTotalArea[j]);

                m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ResetColorThresholdData();
                for (int i = 0; i < dgd_ThresholdSetting.Rows.Count; i++)
                {
                    int intDontCareMode = 0;
                    if (arrDontCareMode.Count > i)
                        intDontCareMode = arrDontCareMode[i];

                    int intROITop = 0;
                    if (arrROITop.Count > i)
                        intROITop = arrROITop[i];

                    int intROIRight = 0;
                    if (arrROIRight.Count > i)
                        intROIRight = arrROIRight[i];

                    int intROIBottom = 0;
                    if (arrROIBottom.Count > i)
                        intROIBottom = arrROIBottom[i];

                    int intROILeft = 0;
                    if (arrROILeft.Count > i)
                        intROILeft = arrROILeft[i];

                    int intFailCondition = 0;
                    if (arrFailCondition.Count > i)
                        intFailCondition = arrFailCondition[i];

                    float fWidth = 0;
                    if (arrWidthSetting.Count > i)
                        fWidth = arrWidthSetting[i];

                    float fLength = 0;
                    if (arrLengthSetting.Count > i)
                        fLength = arrLengthSetting[i];

                    float fMinArea = 0;
                    if (arrMinAreaSetting.Count > i)
                        fMinArea = arrMinAreaSetting[i];

                    float fMaxArea = 0;
                    if (arrMaxAreaSetting.Count > i)
                        fMaxArea = arrMaxAreaSetting[i];

                    float fTotalArea = 0;
                    if (arrTotalAreaSetting.Count > i)
                        fTotalArea = arrTotalAreaSetting[i];

                    int intColorSystem = (dgd_ThresholdSetting.Rows[i].Cells[2] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[2].EditedFormattedValue);
                    int intDefectType = (dgd_ThresholdSetting.Rows[i].Cells[10] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[10].EditedFormattedValue);
                    int intDefectImageNo = (dgd_ThresholdSetting.Rows[i].Cells[11] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[11].EditedFormattedValue);
                    if (dgd_ThresholdSetting.Rows[i].Cells[2].Value.ToString() == "RGB")
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].AddColorThresholdData(
                            Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem,//Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                            fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                            intROITop, intROIRight, intROIBottom, intROILeft);
                    }
                    else if (dgd_ThresholdSetting.Rows[i].Cells[2].Value.ToString() == "HSL")
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].AddColorThresholdData(
                            Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem, //Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                            fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                            intROITop, intROIRight, intROIBottom, intROILeft);
                    }
                    else
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].AddColorThresholdData(
                            Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem, //Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value),
                            Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                            fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                            intROITop, intROIRight, intROIBottom, intROILeft);
                    }
                }


                for (int a = 1; a < m_smVisionInfo.g_arrPad.Length; a++)
                {
                    if (a > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    if (a == m_smVisionInfo.g_intSelectedROI)
                        continue;

                    if (m_smVisionInfo.g_arrPad[a].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                        continue;

                    arrDontCareMode = new List<int>();
                    for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectDontCareMode.Count; j++)
                        arrDontCareMode.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectDontCareMode[j]);

                    arrROITop = new List<int>();
                    for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Top.Count; j++)
                        arrROITop.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Top[j]);

                    arrROIRight = new List<int>();
                    for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Right.Count; j++)
                        arrROIRight.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Right[j]);

                    arrROIBottom = new List<int>();
                    for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Bottom.Count; j++)
                        arrROIBottom.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Bottom[j]);

                    arrROILeft = new List<int>();
                    for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Left.Count; j++)
                        arrROILeft.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspection_Left[j]);

                    arrFailCondition = new List<int>();
                    for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionFailCondition.Count; j++)
                        arrFailCondition.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionFailCondition[j]);

                    arrWidthSetting = new List<float>();
                    for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionWidth.Count; j++)
                        arrWidthSetting.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionWidth[j]);

                    arrLengthSetting = new List<float>();
                    for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionLength.Count; j++)
                        arrLengthSetting.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionLength[j]);

                    arrMinAreaSetting = new List<float>();
                    for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionMinArea.Count; j++)
                        arrMinAreaSetting.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionMinArea[j]);

                    arrMaxAreaSetting = new List<float>();
                    for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionMaxArea.Count; j++)
                        arrMaxAreaSetting.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionMaxArea[j]);

                    arrTotalAreaSetting = new List<float>();
                    for (int j = 0; j < m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionTotalArea.Count; j++)
                        arrTotalAreaSetting.Add(m_smVisionInfo.g_arrPad[a].ref_arrDefectColorInspectionTotalArea[j]);

                    m_smVisionInfo.g_arrPad[a].ResetColorThresholdData();

                    for (int i = 0; i < dgd_ThresholdSetting.Rows.Count; i++)
                    {
                        int intDontCareMode = 0;
                        if (arrDontCareMode.Count > i)
                            intDontCareMode = arrDontCareMode[i];

                        int intROITop = 0;
                        if (arrROITop.Count > i)
                            intROITop = arrROITop[i];

                        int intROIRight = 0;
                        if (arrROIRight.Count > i)
                            intROIRight = arrROIRight[i];

                        int intROIBottom = 0;
                        if (arrROIBottom.Count > i)
                            intROIBottom = arrROIBottom[i];

                        int intROILeft = 0;
                        if (arrROILeft.Count > i)
                            intROILeft = arrROILeft[i];

                        int intFailCondition = 0;
                        if (arrFailCondition.Count > i)
                            intFailCondition = arrFailCondition[i];
                        
                        float fWidth = 0;
                        if (arrWidthSetting.Count > i)
                            fWidth = arrWidthSetting[i];

                        float fLength = 0;
                        if (arrLengthSetting.Count > i)
                            fLength = arrLengthSetting[i];

                        float fMinArea = 0;
                        if (arrMinAreaSetting.Count > i)
                            fMinArea = arrMinAreaSetting[i];

                        float fMaxArea = 0;
                        if (arrMaxAreaSetting.Count > i)
                            fMaxArea = arrMaxAreaSetting[i];

                        float fTotalArea = 0;
                        if (arrTotalAreaSetting.Count > i)
                            fTotalArea = arrTotalAreaSetting[i];

                        int intColorSystem = (dgd_ThresholdSetting.Rows[i].Cells[2] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[2].EditedFormattedValue);
                        int intDefectType = (dgd_ThresholdSetting.Rows[i].Cells[10] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[10].EditedFormattedValue);
                        int intDefectImageNo = (dgd_ThresholdSetting.Rows[i].Cells[11] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[11].EditedFormattedValue);
                        if (dgd_ThresholdSetting.Rows[i].Cells[2].Value.ToString() == "RGB")
                        {
                            m_smVisionInfo.g_arrPad[a].AddColorThresholdData(
                                Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem, //Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                            fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                            intROITop, intROIRight, intROIBottom, intROILeft);
                        }
                        else if (dgd_ThresholdSetting.Rows[i].Cells[2].Value.ToString() == "HSL")
                        {
                            m_smVisionInfo.g_arrPad[a].AddColorThresholdData(
                                Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem, //Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                            fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                            intROITop, intROIRight, intROIBottom, intROILeft);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrPad[a].AddColorThresholdData(
                                Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem, //Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value),
                                Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                            fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                            intROITop, intROIRight, intROIBottom, intROILeft);
                        }
                    }
                }


                if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode.Count > index)
                {
                    switch (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode[index])
                    {
                        case 0:
                            radioButton_NoneDontCare.Checked = true;
                            m_intDontCareMode = 0;
                            pnl_DrawMethod.Visible = false;
                            break;
                        case 1:
                            radioButton_PadDontCare.Checked = true;
                            m_intDontCareMode = 1;
                            pnl_DrawMethod.Visible = false;
                            break;
                        case 2:
                            radioButton_PackageDontCare.Checked = true;
                            m_intDontCareMode = 2;
                            pnl_DrawMethod.Visible = false;
                            break;
                        case 3:
                            radioButton_ManualDontCare.Checked = true;
                            m_intDontCareMode = 3;
                            pnl_DrawMethod.Visible = true;
                            break;
                    }
                }

            }

            m_smVisionInfo.g_intSelectedColorThresholdIndex = dgd_ThresholdSetting.RowCount - 1;

            UpdateROIToleranceGUI();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DeleteThreshold_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (dgd_ThresholdSetting.Rows.Count <= 1)
            {
                SRMMessageBox.Show("Minimum 1 set of thresholds must be set.");
                return;
            }
            else
            {
                if ((m_intRowIndexPrev < 0) || (m_intRowIndexPrev >= dgd_ThresholdSetting.Rows.Count))
                {
                    SRMMessageBox.Show("Please select threshold row first.");
                    return;
                }

                if (SRMMessageBox.Show("Are you sure you want to delete the selected row threshold data?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dgd_ThresholdSetting.Rows.RemoveAt(m_intRowIndexPrev);

                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        if (i == m_smVisionInfo.g_intSelectedROI)
                            continue;

                        if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                            continue;

                        m_smVisionInfo.g_arrPad[i].DeleteColorThresholdData(m_intRowIndexPrev);
                        DeletePolygon(i);
                    }

                    m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].DeleteColorThresholdData(m_intRowIndexPrev);
                    DeletePolygon(m_smVisionInfo.g_intSelectedROI);
                    //m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode.RemoveAt(m_intRowIndexPrev);
                    UpdateRowsNo();

                    while (m_intRowIndexPrev >= 0)
                    {
                        if (m_intRowIndexPrev < dgd_ThresholdSetting.Rows.Count)
                        {
                            dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Selected = true;
                            UpdateSelectedThresholdValue(m_intRowIndexPrev);
                            break;
                        }

                        m_intRowIndexPrev--;
                    }
                    //DeletePolygon();
                    if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode.Count > dgd_ThresholdSetting.RowCount - 1)
                    {
                        switch (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode[dgd_ThresholdSetting.RowCount - 1])
                        {
                            case 0:
                                radioButton_NoneDontCare.Checked = true;
                                m_intDontCareMode = 0;
                                pnl_DrawMethod.Visible = false;
                                break;
                            case 1:
                                radioButton_PadDontCare.Checked = true;
                                m_intDontCareMode = 1;
                                pnl_DrawMethod.Visible = false;
                                break;
                            case 2:
                                radioButton_PackageDontCare.Checked = true;
                                m_intDontCareMode = 2;
                                pnl_DrawMethod.Visible = false;
                                break;
                            case 3:
                                radioButton_ManualDontCare.Checked = true;
                                m_intDontCareMode = 3;
                                pnl_DrawMethod.Visible = true;
                                break;
                        }
                    }

                    m_smVisionInfo.g_intSelectedColorThresholdIndex = dgd_ThresholdSetting.RowCount - 1;

                    if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedROI)
                        if (m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex)
                            //if (m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count <= m_smVisionInfo.g_intSelectedDontCareROIIndex)
                                m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1;

                }
            }

            UpdateROIToleranceGUI();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_UpdateThreshold_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            UpdateChangedDataToTableList();
        }

        private void dgd_ThresholdSetting_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            //if (CheckIsDataChanged())
            //{
            //    if (SRMMessageBox.Show("Do you want to update the changed threshold value into table?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //    {
            //        UpdateChangedDataToTableList();
            //    }
            //}
            
            UpdateSelectedThresholdValue(e.RowIndex);
            UpdateLSHColorBar();
            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode.Count > e.RowIndex)
            {
                switch (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode[e.RowIndex])
                {
                    case 0:
                        radioButton_NoneDontCare.Checked = true;
                        m_intDontCareMode = 0;
                        pnl_DrawMethod.Visible = false;
                        break;
                    case 1:
                        radioButton_PadDontCare.Checked = true;
                        m_intDontCareMode = 1;
                        pnl_DrawMethod.Visible = false;
                        break;
                    case 2:
                        radioButton_PackageDontCare.Checked = true;
                        m_intDontCareMode = 2;
                        pnl_DrawMethod.Visible = false;
                        break;
                    case 3:
                        radioButton_ManualDontCare.Checked = true;
                        m_intDontCareMode = 3;
                        pnl_DrawMethod.Visible = true;
                        break;
                }
            }

            m_smVisionInfo.g_intSelectedColorThresholdIndex = e.RowIndex;

            if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedROI)
                if (m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex)
                    //if (m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count <= m_smVisionInfo.g_intSelectedDontCareROIIndex)
                        m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1;

            UpdateROIToleranceGUI();

            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_ThresholdSetting_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int c = e.ColumnIndex;
            int r = e.RowIndex;


            if (r < 0 || c < 1)
                return;

            if ((dgd_ThresholdSetting.Rows[r].Cells[2] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[r].Cells[2].EditedFormattedValue) == 2 && ((c == 4) || (c == 5) || (c == 6) || (c == 7) || (c == 8)))
                return;

            if (c == 1) // Threshold Name
            {
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString() != "")
                {
                    m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorThresName[r] = dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString();


                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        if (i == m_smVisionInfo.g_intSelectedROI)
                            continue;

                        if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                            continue;

                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[r] = dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString();
                    }

                }
            }
            else if (c == 2) // Color System
            {
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString() != "")
                {
                    m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorSystem[r] = (dgd_ThresholdSetting.Rows[r].Cells[c] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[r].Cells[c].EditedFormattedValue);

                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        if (i == m_smVisionInfo.g_intSelectedROI)
                            continue;

                        if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                            continue;

                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorSystem[r] = (dgd_ThresholdSetting.Rows[r].Cells[c] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[r].Cells[c].EditedFormattedValue);
                    }

                }
            }
            else if (c == 3) // Threshold 1 value
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0 && intValue < 256)
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[r][0] = intValue;
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (i == m_smVisionInfo.g_intSelectedROI)
                                continue;

                            if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                                continue;

                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColor[r][0] = intValue;
                        }
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[r][0];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[r][0];
                }
            }
            else if (c == 4) // Threshold 1 Tolerance
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0)
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[r][0] = intValue;
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (i == m_smVisionInfo.g_intSelectedROI)
                                continue;

                            if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                                continue;

                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorTolerance[r][0] = intValue;
                        }
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[r][0];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[r][0];
                }
            }
            else if (c == 5) // Threshold 2 value
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0 && intValue < 256)
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[r][1] = intValue;

                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (i == m_smVisionInfo.g_intSelectedROI)
                                continue;

                            if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                                continue;

                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColor[r][1] = intValue;
                        }
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[r][1];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[r][1];
                }
            }
            else if (c == 6) // Threshold 2 Tolerance
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0)
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[r][1] = intValue;
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (i == m_smVisionInfo.g_intSelectedROI)
                                continue;

                            if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                                continue;

                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorTolerance[r][1] = intValue;
                        }
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[r][1];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[r][1];
                }
            }
            else if (c == 7) // Threshold 3 value
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0 && intValue < 256)
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[r][2] = intValue;
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (i == m_smVisionInfo.g_intSelectedROI)
                                continue;

                            if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                                continue;

                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColor[r][2] = intValue;
                        }
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[r][2];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor[r][2];
                }
            }
            else if (c == 8) // Threshold 3 Tolerance
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0)
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[r][2] = intValue;
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (i == m_smVisionInfo.g_intSelectedROI)
                                continue;

                            if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                                continue;

                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorTolerance[r][2] = intValue;
                        }
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[r][2];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorTolerance[r][2];
                }
            }
            else if (c == 9) // Filter Min Area
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0)
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorMinArea[r] = intValue;
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (i == m_smVisionInfo.g_intSelectedROI)
                                continue;

                            if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                                continue;

                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorMinArea[r] = intValue;
                        }
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorMinArea[r];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColorMinArea[r];
                }
            }
            else if (c == 10) // Defect Type
            {
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString() != "")
                {
                    m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectType[r] = (dgd_ThresholdSetting.Rows[r].Cells[c] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[r].Cells[c].EditedFormattedValue);
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        if (i == m_smVisionInfo.g_intSelectedROI)
                            continue;

                        if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                            continue;

                        m_smVisionInfo.g_arrPad[i].ref_arrDefectType[r] = (dgd_ThresholdSetting.Rows[r].Cells[c] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[r].Cells[c].EditedFormattedValue);
                    }

                }
            }
            else if (c == 11) // Image No
            {
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString() != "")
                {
                    m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectImageNo[r] = (dgd_ThresholdSetting.Rows[r].Cells[c] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[r].Cells[c].EditedFormattedValue);

                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        if (i == m_smVisionInfo.g_intSelectedROI)
                            continue;

                        if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                            continue;

                        m_smVisionInfo.g_arrPad[i].ref_arrDefectImageNo[r] = (dgd_ThresholdSetting.Rows[r].Cells[c] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[r].Cells[c].EditedFormattedValue);
                    }


                    m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    RotateColorImage();
                }
            }
            else if (c == 12) // Close Iteration
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0 && intValue < 11)
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectCloseIteration[r] = intValue;
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (i == m_smVisionInfo.g_intSelectedROI)
                                continue;

                            if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                                continue;

                            m_smVisionInfo.g_arrPad[i].ref_arrDefectCloseIteration[r] = intValue;
                        }
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectCloseIteration[r];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectCloseIteration[r];
                }
            }
            else if (c == 13) // Invert Black White
            {
                bool blnValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && bool.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out blnValue))
                {
                    m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectInvertBlackWhite[r] = blnValue;
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        if (i == m_smVisionInfo.g_intSelectedROI)
                            continue;

                        if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                            continue;

                        m_smVisionInfo.g_arrPad[i].ref_arrDefectInvertBlackWhite[r] = blnValue;
                    }

                }
            }

            UpdateSelectedThresholdValue(r);//e.RowIndex
            UpdateLSHColorBar();

            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void RotateColorImage()
        {
            if (m_smVisionInfo.g_intSelectedImage == -1)
            {
                if (m_smVisionInfo.g_arrPad[0].GetOverallWantGaugeMeasurePkgSize(false))
                {
                    // Define rotation ROI with ROI center point same as gauge unit center point
                    float fROIWidth = Math.Min(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalX,
                                        m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X);
                    float fROIHeight = Math.Min(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalY,
                                        m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y);

                    CROI objRotateROI = new CROI();
                    objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage]);
                    objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X - fROIWidth, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y - fROIHeight, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(fROIWidth * 2, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(fROIHeight * 2, 0, MidpointRounding.AwayFromZero));

                    // Rotate image to zero degree using RectGauge4L angle result
                    CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage], objRotateROI, // Middle Pad Search ROI
                                      m_smVisionInfo.g_arrPad[0].GetResultAngle_RectGauge4L(), 8,
                    ref m_smVisionInfo.g_arrColorRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_arrPadColorROIs[0][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                }
                else
                {
                    m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_arrColorRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_arrPadColorROIs[0][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                }
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    if (m_smVisionInfo.g_arrPad[i].GetOverallWantGaugeMeasurePkgSize(false))
                    {
                        float fLengthTop = 0;
                        float fLengthRight = 0;
                        float fLengthBottom = 0;
                        float fLengthLeft = 0;
                        int intStartX = 0;
                        int intStartY = 0;
                        int intExtendX = 0;
                        int intExtendX2 = 0;
                        int intExtendY = 0;
                        int intExtendY2 = 0;

                        PointF[] CornerPoints = new PointF[4];


                        fLengthTop = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromTop;
                        fLengthRight = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromRight;
                        fLengthBottom = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromBottom;
                        fLengthLeft = m_smVisionInfo.g_arrPad[i].ref_intPadROIStartPixelFromLeft;

                        intStartX = (int)Math.Round((m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
                                                     (m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2)));
                        intExtendX = (int)(fLengthLeft);
                        intExtendX2 = (int)(fLengthRight);

                        intStartY = (int)Math.Round((m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
                                                    (m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2)));
                        intExtendY = (int)(fLengthTop);
                        intExtendY2 = (int)(fLengthBottom);

                        int intPadROIStartX = intStartX - intExtendX;
                        int intPadROIStartY = intStartY - intExtendY;
                        int intPadROIWidth = intExtendX + intExtendX2 + (int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0));
                        int intPadROIHeight = intExtendY + intExtendY2 + (int)Math.Ceiling(m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0));

                        if (m_smVisionInfo.g_blnViewRotatedImage)
                        {
                            m_smVisionInfo.g_arrPadColorROIs[i][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                            m_smVisionInfo.g_arrPadColorROIs[i][0].LoadROISetting(intPadROIStartX, intPadROIStartY, intPadROIWidth, intPadROIHeight);
                            m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage].ConvertColorToMono(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);

                            m_smVisionInfo.g_arrPadROIs[i][3].LoadROISetting(intPadROIStartX, intPadROIStartY, intPadROIWidth, intPadROIHeight);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrPadColorROIs[i][0].AttachImage(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage]);
                            m_smVisionInfo.g_arrPadColorROIs[i][0].LoadROISetting(intPadROIStartX, intPadROIStartY, intPadROIWidth, intPadROIHeight);
                            m_smVisionInfo.g_arrPadROIs[i][3].LoadROISetting(intPadROIStartX, intPadROIStartY, intPadROIWidth, intPadROIHeight);

                        }
                    }
                    else
                    {
                        PointF pPRCenterPoint = m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_UnitMatcher();
                        SizeF szPRSize = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher();


                        int intPadROIStartX = (int)Math.Round(pPRCenterPoint.X - szPRSize.Width / 2, 0, MidpointRounding.AwayFromZero);
                        int intPadROIStartY = (int)Math.Round(pPRCenterPoint.Y - szPRSize.Height / 2, 0, MidpointRounding.AwayFromZero);
                        int intPadROIWidth = (int)szPRSize.Width;
                        int intPadROIHeight = (int)szPRSize.Height;

                        if (m_smVisionInfo.g_blnViewRotatedImage)
                        {
                            m_smVisionInfo.g_arrPadColorROIs[i][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                            m_smVisionInfo.g_arrPadColorROIs[i][0].LoadROISetting(intPadROIStartX, intPadROIStartY, intPadROIWidth, intPadROIHeight);
                            m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage].ConvertColorToMono(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);

                            m_smVisionInfo.g_arrPadROIs[i][3].LoadROISetting(intPadROIStartX, intPadROIStartY, intPadROIWidth, intPadROIHeight);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrPadColorROIs[i][0].AttachImage(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage]);
                            m_smVisionInfo.g_arrPadColorROIs[i][0].LoadROISetting(intPadROIStartX, intPadROIStartY, intPadROIWidth, intPadROIHeight);
                            m_smVisionInfo.g_arrPadROIs[i][3].LoadROISetting(intPadROIStartX, intPadROIStartY, intPadROIWidth, intPadROIHeight);

                        }
                    }
                }
            }

        }
        private void trackBar_Threshold_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Threshold.Text = trackBar_Threshold.Value.ToString();
        }

        private void txt_Threshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intValue = Convert.ToInt32(txt_Threshold.Text);
            if (intValue < 0)
                intValue = 0;

            m_smVisionInfo.g_intColorThreshold[0] = trackBar_Threshold.Value = intValue;

            dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[3].Value = trackBar_Threshold.Value;
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void pic_Help_MouseEnter(object sender, EventArgs e)
        {

            pnl_HelpMessage.Location = new Point(310, 185);
            pnl_HelpMessage.BringToFront();
            pnl_HelpMessage.Focus();
        }

        private void pic_Help_MouseLeave(object sender, EventArgs e)
        {
            pnl_HelpMessage.Location = new Point(700, 161);
            this.Refresh();
            m_blnUpdateImage = true;
        }

        private void radioButton_DontCare_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (((RadioButton)sender).Name)
            {
                case "radioButton_NoneDontCare":
                    m_intDontCareMode = 0;
                    pnl_DrawMethod.Visible = false;
                    break;
                case "radioButton_PadDontCare":
                    m_intDontCareMode = 1;
                    pnl_DrawMethod.Visible = false;
                    break;
                case "radioButton_PackageDontCare":
                    m_intDontCareMode = 2;
                    pnl_DrawMethod.Visible = false;
                    break;
                case "radioButton_ManualDontCare":
                    m_intDontCareMode = 3;
                    pnl_DrawMethod.Visible = true;
                    break;
            }

            if (m_smVisionInfo.g_intSelectedColorThresholdIndex != -1)
                m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode[m_smVisionInfo.g_intSelectedColorThresholdIndex] = m_intDontCareMode;
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void btn_Undo_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnDrawFreeShapeDone = true;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 3)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][3].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}

            if (m_smVisionInfo.g_arrPolygon_PadColor.Count > m_smVisionInfo.g_intSelectedROI)
                if (m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex && m_smVisionInfo.g_intSelectedColorThresholdIndex != -1)
                    if (m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex && m_smVisionInfo.g_intSelectedDontCareROIIndex != -1)
                        m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex][m_smVisionInfo.g_intSelectedDontCareROIIndex].UndoPolygon();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_DontCareAreaDrawMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            //for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
            //{
            //    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    m_smVisionInfo.g_arrPolygon_Pad[i][0].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;

            //}

            if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
            {
                btn_Undo.Visible = true;
                pic_Help.Visible = true;
            }
            else
            {
                btn_Undo.Visible = false;
                pic_Help.Visible = false;
            }
        }
        private void btn_AddDontCareROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intSelectedColorThresholdIndex == -1)
                return;

            for (int i = m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count; i < dgd_ThresholdSetting.RowCount; i++)
            {
                m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Add(new List<ROI>());
            }

            m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Add(new ROI());
            m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex][m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1].AttachImage(m_smVisionInfo.g_arrPadROIs[m_smVisionInfo.g_intSelectedROI][3]);

            for (int i = m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI].Count; i < dgd_ThresholdSetting.RowCount; i++)
            {
                m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI].Add(new List<Polygon>());
            }

            m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Add(new Polygon());
            m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex][m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DeleteDontCareROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intSelectedColorThresholdIndex == -1 || m_smVisionInfo.g_intSelectedDontCareROIIndex == -1)
                return;

            if (m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count == 0)
                return;

            m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1;
            if (m_intSelectedDontCareROIIndexPrev != m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;
                if (m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex && m_smVisionInfo.g_intSelectedColorThresholdIndex != -1)
                {
                    if ((m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex) && (m_smVisionInfo.g_intSelectedDontCareROIIndex != -1))
                    {
                        cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
                    }
                    else
                        cbo_DontCareAreaDrawMethod.SelectedIndex = 0;
                }
                else
                    cbo_DontCareAreaDrawMethod.SelectedIndex = 0;

                if (cbo_DontCareAreaDrawMethod.SelectedIndex == 2)
                {
                    btn_Undo.Visible = true;
                    pic_Help.Visible = true;
                }
                else
                {
                    btn_Undo.Visible = false;
                    pic_Help.Visible = false;
                }
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void DeletePolygon(int intPadIndex)
        {
            if (m_smVisionInfo.g_intSelectedColorThresholdIndex == -1)
                return;

            if (m_smVisionInfo.g_arrPadColorDontCareROIs[intPadIndex].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex)
            {
                m_smVisionInfo.g_arrPadColorDontCareROIs[intPadIndex].RemoveAt(m_smVisionInfo.g_intSelectedColorThresholdIndex);
                m_smVisionInfo.g_arrPolygon_PadColor[intPadIndex].RemoveAt(m_smVisionInfo.g_intSelectedColorThresholdIndex);
            }
        }
        private void srmTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (((TabControl)sender).SelectedTab.Name)
            {
                case "tp_ColorProfile":
                    m_smVisionInfo.g_blnViewColorPackageStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnGetPixel = true;
                    m_smVisionInfo.g_blnDragROI = false;
                    label1.Visible = true;
                    break;
                case "tp_ROITolerance":
                    m_smVisionInfo.g_blnViewColorPackageStartPixelFromEdge = true;
                    m_smVisionInfo.g_blnGetPixel = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    label1.Visible = false;
                    break;
                case "tp_DontCare":
                    m_smVisionInfo.g_blnViewColorPackageStartPixelFromEdge = false;
                    m_smVisionInfo.g_blnGetPixel = false;
                    m_smVisionInfo.g_blnDragROI = true;
                    label1.Visible = false;
                    break;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_blnUpdateImage = true;
        }

        private void txt_StartPixelFromTop_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || m_smVisionInfo.g_intSelectedColorThresholdIndex == -1)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_StartPixelFromRight.Text = txt_StartPixelFromTop.Text;
                    txt_StartPixelFromBottom.Text = txt_StartPixelFromTop.Text;
                    txt_StartPixelFromLeft.Text = txt_StartPixelFromTop.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i == m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                    continue;

                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                        }
                        break;
                }

            }
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_ThresholdSetting_Click(object sender, EventArgs e)
        {

        }

        private void txt_StartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || m_smVisionInfo.g_intSelectedColorThresholdIndex == -1)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_StartPixelFromTop.Text = txt_StartPixelFromRight.Text;
                    txt_StartPixelFromBottom.Text = txt_StartPixelFromRight.Text;
                    txt_StartPixelFromLeft.Text = txt_StartPixelFromRight.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i == m_smVisionInfo.g_intSelectedROI || i == 0)
                    continue;

                if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                    continue;

                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                        }
                        break;
                }

            }
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CloseIteration_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intColorCloseIteration = (int)txt_CloseIteration.Value;

            dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[12].Value = txt_CloseIteration.Value;
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_InvertBlackWhite_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnColorInvertBlackWhite = chk_InvertBlackWhite.Checked;
            
            dgd_ThresholdSetting.Rows[m_intRowIndexPrev].Cells[13].Value = chk_InvertBlackWhite.Checked;
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void pic_ThresholdImage_Paint(object sender, PaintEventArgs e)
        {
            m_blnUpdateImage = true;
        }

        private void btn_Up_Click(object sender, EventArgs e)
        {
            DataGridView dgv = dgd_ThresholdSetting;
            try
            {
                int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (rowIndex == 0)
                    return;
                // get index of the column for the selected cell
                int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[rowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(rowIndex - 1, selectedRow);
                dgv.ClearSelection();
                dgv.Rows[rowIndex - 1].Cells[colIndex].Selected = true;

                dgd_ThresholdSetting.Rows[rowIndex].Cells[0].Value = (rowIndex + 1).ToString();
                dgd_ThresholdSetting.Rows[rowIndex - 1].Cells[0].Value = (rowIndex).ToString();

                SwapRow(rowIndex, rowIndex - 1);

            }
            catch { }
        }

        private void btn_Down_Click(object sender, EventArgs e)
        {
            DataGridView dgv = dgd_ThresholdSetting;
            try
            {
                int totalRows = dgv.Rows.Count;
                // get index of the row for the selected cell
                int rowIndex = dgv.SelectedCells[0].OwningRow.Index;
                if (rowIndex == totalRows - 1)
                    return;
                // get index of the column for the selected cell
                int colIndex = dgv.SelectedCells[0].OwningColumn.Index;
                DataGridViewRow selectedRow = dgv.Rows[rowIndex];
                dgv.Rows.Remove(selectedRow);
                dgv.Rows.Insert(rowIndex + 1, selectedRow);
                dgv.ClearSelection();
                dgv.Rows[rowIndex + 1].Cells[colIndex].Selected = true;

                dgd_ThresholdSetting.Rows[rowIndex].Cells[0].Value = (rowIndex + 1).ToString();
                dgd_ThresholdSetting.Rows[rowIndex + 1].Cells[0].Value = (rowIndex + 1 + 1).ToString();

                SwapRow(rowIndex, rowIndex + 1);
                
            }
            catch { }
        }

        private void SwapRow(int intFrom, int intTo)
        {
            List<ROI> arrROI = new List<ROI>();
            List<Polygon> arrPolygon = new List<Polygon>();
            if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedROI && m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > intTo)
                arrROI = m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][intTo];
            if (m_smVisionInfo.g_arrPolygon_PadColor.Count > m_smVisionInfo.g_intSelectedROI && m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI].Count > intTo)
                arrPolygon = m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][intTo];

            if (m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count <= intTo)
            {
                m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Add(new List<ROI>());
            }
            if (m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI].Count <= intTo)
            {
                m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI].Add(new List<Polygon>());
            }

            if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedROI && m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > intTo && m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > intFrom)
                m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][intTo] = m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][intFrom];
            if (m_smVisionInfo.g_arrPolygon_PadColor.Count > m_smVisionInfo.g_intSelectedROI && m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI].Count > intTo && m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI].Count > intFrom)
                m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][intTo] = m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][intFrom];

            if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedROI && m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > intFrom)
                m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][intFrom] = arrROI;
            if (m_smVisionInfo.g_arrPolygon_PadColor.Count > m_smVisionInfo.g_intSelectedROI && m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI].Count > intFrom)
                m_smVisionInfo.g_arrPolygon_PadColor[m_smVisionInfo.g_intSelectedROI][intFrom] = arrPolygon;

            m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].SwapColorDefect(intFrom, intTo);


            if (intTo < 0)
                return;

            UpdateSelectedThresholdValue(intTo);
            UpdateLSHColorBar();
            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode.Count > intTo)
            {
                switch (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectDontCareMode[intTo])
                {
                    case 0:
                        radioButton_NoneDontCare.Checked = true;
                        m_intDontCareMode = 0;
                        pnl_DrawMethod.Visible = false;
                        break;
                    case 1:
                        radioButton_PadDontCare.Checked = true;
                        m_intDontCareMode = 1;
                        pnl_DrawMethod.Visible = false;
                        break;
                    case 2:
                        radioButton_PackageDontCare.Checked = true;
                        m_intDontCareMode = 2;
                        pnl_DrawMethod.Visible = false;
                        break;
                    case 3:
                        radioButton_ManualDontCare.Checked = true;
                        m_intDontCareMode = 3;
                        pnl_DrawMethod.Visible = true;
                        break;
                }
            }

            m_smVisionInfo.g_intSelectedColorThresholdIndex = intTo;

            if (m_smVisionInfo.g_arrPadColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedROI)
                if (m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex)
                    //if (m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count <= m_smVisionInfo.g_intSelectedDontCareROIIndex)
                        m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPadColorDontCareROIs[m_smVisionInfo.g_intSelectedROI][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1;

            UpdateROIToleranceGUI();

            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || m_smVisionInfo.g_intSelectedColorThresholdIndex == -1)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_StartPixelFromTop.Text = txt_StartPixelFromBottom.Text;
                    txt_StartPixelFromRight.Text = txt_StartPixelFromBottom.Text;
                    txt_StartPixelFromLeft.Text = txt_StartPixelFromBottom.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i == m_smVisionInfo.g_intSelectedROI || i == 0)
                    continue;

                if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                    continue;

                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                        }
                        break;
                }

            }
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_StartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || m_smVisionInfo.g_intSelectedColorThresholdIndex == -1)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_StartPixelFromTop.Text = txt_StartPixelFromLeft.Text;
                    txt_StartPixelFromRight.Text = txt_StartPixelFromLeft.Text;
                    txt_StartPixelFromBottom.Text = txt_StartPixelFromLeft.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (i == m_smVisionInfo.g_intSelectedROI || i == 0)
                    continue;

                if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intColorPadGroupIndex)
                    continue;

                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        if (chk_SetToAllEdge.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                        }
                        break;
                }

            }
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void radioBtn_PadIndex_Click(object sender, EventArgs e)
        {

            m_blnInitDone = false;
            if (sender == radioBtn_Middle)
            {
                m_smVisionInfo.g_intSelectedROI = 0;
            }
            else if (sender == radioBtn_Up)
            {
                m_smVisionInfo.g_intSelectedROI = 1;
            }
            else if (sender == radioBtn_Right)
            {
                m_smVisionInfo.g_intSelectedROI = 2;
            }
            else if (sender == radioBtn_Down)
            {
                m_smVisionInfo.g_intSelectedROI = 3;
            }
            else if (sender == radioBtn_Left)
            {
                m_smVisionInfo.g_intSelectedROI = 4;
            }

            switch (m_smVisionInfo.g_intSelectedROI)
            {
                case 0:
                case 1:
                    pnl_Top.Location = m_pTop;
                    pnl_Right.Location = m_pRight;
                    pnl_Bottom.Location = m_pBottom;
                    pnl_Left.Location = m_pLeft;
                    break;
                case 2:
                    pnl_Top.Location = m_pRight;
                    pnl_Right.Location = m_pBottom;
                    pnl_Bottom.Location = m_pLeft;
                    pnl_Left.Location = m_pTop;
                    break;
                case 3:
                    pnl_Top.Location = m_pBottom;
                    pnl_Right.Location = m_pLeft;
                    pnl_Bottom.Location = m_pTop;
                    pnl_Left.Location = m_pRight;
                    break;
                case 4:
                    pnl_Top.Location = m_pLeft;
                    pnl_Right.Location = m_pTop;
                    pnl_Bottom.Location = m_pRight;
                    pnl_Left.Location = m_pBottom;
                    break;
            }

            if (m_smVisionInfo.g_intSelectedROI != m_intSelectedROIPrev)
            {
                m_intSelectedROIPrev = m_smVisionInfo.g_intSelectedROI;

                CustomizeGUI();

                if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_arrDefectColor.Count == 0)
                    m_smVisionInfo.g_intSelectedColorThresholdIndex = -1;
                else
                    m_smVisionInfo.g_intSelectedColorThresholdIndex = 0;
            }

            UpdateROIToleranceGUI();

            m_blnInitDone = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = m_blnUpdateImage = true;

        }
    }
}