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
using System.IO;

namespace VisionProcessForm
{
    public partial class ColorPackageMultiThresholdForm : Form
    {
        #region Member Variables
        private int m_intSelectedDontCareROIIndexPrev = 0;
        private int m_intDontCareMode = 0;
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
        private bool m_blnGaugeResult;
        private int m_intOrientAngle = 0;
        ROI m_objMarkTrainROI = new ROI();
        private List<int> arrStartX = new List<int>();
        private List<int> arrStartY = new List<int>();
        private List<int> arrEndX = new List<int>();
        private List<int> arrEndY = new List<int>();
        #endregion

        public bool ref_blnFormOpened { get { return m_blnFormOpened; } }

        public ColorPackageMultiThresholdForm(VisionInfo smVisionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, bool blnGaugeResult)
        {
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_blnFormOpened = true;
            InitializeComponent();
            m_Graphic = Graphics.FromHwnd(pic_ThresholdImage.Handle);
            m_blnGaugeResult = blnGaugeResult;

            if (m_blnGaugeResult)
            {
                if (StartOrientTest())
                {
                    if (StartMarkTest())
                    {
                        CollectOCVData();
                    }
                }
            }
            RotateImage();
            m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0]);
            m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].LoadROISetting((int)Math.Round((m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionX, 0, MidpointRounding.AwayFromZero),
                                         (int)Math.Round((m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) - m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIPositionY, 0, MidpointRounding.AwayFromZero),
                                         (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth, 0, MidpointRounding.AwayFromZero),
                                         (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight, 0, MidpointRounding.AwayFromZero));

            AddColorROI(m_smVisionInfo.g_arrPackageColorROIs);

            m_smVisionInfo.g_intViewInspectionSetting = 1;
            cbo_DontCareAreaDrawMethod.SelectedIndex = 0;
            CustomizeGUI();

            if (dgd_ThresholdSetting.RowCount == 0)
                m_smVisionInfo.g_intSelectedColorThresholdIndex = -1;
            else
                m_smVisionInfo.g_intSelectedColorThresholdIndex = 0;

            UpdateROIToleranceGUI();
            RotateColorImage();
            m_blnInitDone = true;
        }
        private void CustomizeGUI()
        {
            UpdateThresholdListTable();
            UpdateSelectedThresholdValue(0);
            UpdateLSHColorBar();

            if (m_smVisionInfo.g_arrPackageROIs.Count > m_smVisionInfo.g_intSelectedUnit && m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit].Count > 1)
            {
                if (m_smVisionInfo.g_arrPackageColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedUnit)
                {
                    for (int j = 0; j < m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count; j++)
                    {
                        for (int k = 0; k < m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][j].Count; k++)
                        {
                            m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][j][k].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
                        }
                    }
                }

            }

        }
        private void UpdateROIToleranceGUI()
        {
            if (m_smVisionInfo.g_intSelectedColorThresholdIndex == -1)
                return;

            txt_StartPixelFromTop.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
            if (txt_StartPixelFromTop.Text == "0")
            {
                txt_StartPixelFromTop.Text = "1";
                txt_StartPixelFromTop.Text = "0";
            }
            txt_StartPixelFromRight.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
            if (txt_StartPixelFromRight.Text == "0")
            {
                txt_StartPixelFromRight.Text = "1";
                txt_StartPixelFromRight.Text = "0";
            }
            txt_StartPixelFromBottom.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
            if (txt_StartPixelFromBottom.Text == "0")
            {
                txt_StartPixelFromBottom.Text = "1";
                txt_StartPixelFromBottom.Text = "0";
            }
            txt_StartPixelFromLeft.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex].ToString();
            if (txt_StartPixelFromLeft.Text == "0")
            {
                txt_StartPixelFromLeft.Text = "1";
                txt_StartPixelFromLeft.Text = "0";
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

            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectCloseIteration.Count > intRowIndex)
                txt_CloseIteration.Value = m_smVisionInfo.g_intColorCloseIteration = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectCloseIteration[intRowIndex];
            else
                txt_CloseIteration.Value = m_smVisionInfo.g_intColorCloseIteration = 0;

            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectInvertBlackWhite.Count > intRowIndex)
                chk_InvertBlackWhite.Checked = m_smVisionInfo.g_blnColorInvertBlackWhite = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectInvertBlackWhite[intRowIndex];
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

            for (int i = 0; i < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count; i++)
            {
                dgd_ThresholdSetting.Rows.Add();
                UpdateImageNoColumnItem(i);
                dgd_ThresholdSetting.Rows[i].Cells[0].Value = (i + 1);
                dgd_ThresholdSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i];
                dgd_ThresholdSetting.Rows[i].Cells[2].Value = (dgd_ThresholdSetting.Rows[i].Cells[2] as DataGridViewComboBoxCell).Items[m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorSystem[i]];
                if (dgd_ThresholdSetting.Rows[i].Cells[2].Value.ToString() == "RGB")
                {
                    dgd_ThresholdSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[i][0];
                    dgd_ThresholdSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[i][0];
                    dgd_ThresholdSetting.Rows[i].Cells[5].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[i][1];
                    dgd_ThresholdSetting.Rows[i].Cells[6].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[i][1];
                    dgd_ThresholdSetting.Rows[i].Cells[7].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[i][2];
                    dgd_ThresholdSetting.Rows[i].Cells[8].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[i][2];
                }
                else if (dgd_ThresholdSetting.Rows[i].Cells[2].Value.ToString() == "HSL")
                {
                    dgd_ThresholdSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[i][2];
                    dgd_ThresholdSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[i][2];
                    dgd_ThresholdSetting.Rows[i].Cells[5].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[i][1];
                    dgd_ThresholdSetting.Rows[i].Cells[6].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[i][1];
                    dgd_ThresholdSetting.Rows[i].Cells[7].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[i][0];
                    dgd_ThresholdSetting.Rows[i].Cells[8].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[i][0];
                }
                else
                {
                    dgd_ThresholdSetting.Rows[i].Cells[3].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[i][0];
                    dgd_ThresholdSetting.Rows[i].Cells[4].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[i][0];
                    dgd_ThresholdSetting.Rows[i].Cells[5].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[i][1];
                    dgd_ThresholdSetting.Rows[i].Cells[6].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[i][1];
                    dgd_ThresholdSetting.Rows[i].Cells[7].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[i][2];
                    dgd_ThresholdSetting.Rows[i].Cells[8].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[i][2];
                }
                dgd_ThresholdSetting.Rows[i].Cells[9].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorMinArea[i];
                dgd_ThresholdSetting.Rows[i].Cells[10].Value = (dgd_ThresholdSetting.Rows[i].Cells[10] as DataGridViewComboBoxCell).Items[m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectType[i]];
                dgd_ThresholdSetting.Rows[i].Cells[11].Value = (dgd_ThresholdSetting.Rows[i].Cells[11] as DataGridViewComboBoxCell).Items[m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectImageNo[i]];
                dgd_ThresholdSetting.Rows[i].Cells[12].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectCloseIteration[i];
                dgd_ThresholdSetting.Rows[i].Cells[13].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectInvertBlackWhite[i];
                UpdateColorSystemGUI((dgd_ThresholdSetting.Rows[i].Cells[2] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[2].EditedFormattedValue) == 2, i);
                if (i == 0)
                {
                    switch (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode[i])
                    {
                        case 0:
                            radioButton_NoneDontCare.Checked = true;
                            m_intDontCareMode = 0;
                            pnl_DrawMethod.Visible = false;
                            break;
                        case 1:
                            radioButton_MarkDontCare.Checked = true;
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
            string strPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Package\\";
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (u == 0)
                    m_smVisionInfo.g_arrPackage[u].LoadColorPackageOnly(strPath + "Settings.xml", "Settings");
                else
                {
                    if (File.Exists(strPath + "Settings2.xml"))
                        m_smVisionInfo.g_arrPackage[u].LoadColorPackageOnly(strPath + "Settings2.xml", "Settings");
                    else
                        m_smVisionInfo.g_arrPackage[u].LoadColorPackageOnly(strPath + "Settings.xml", "Settings");
                }
            }

            LoadROISetting(strPath + "ColorDontCareROI.xml", m_smVisionInfo.g_arrPackageColorDontCareROIs, m_smVisionInfo.g_intUnitsOnImage);
            for (int i = 0; i < m_smVisionInfo.g_arrPackageROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrPackageROIs[i].Count > 1)
                {
                    if (m_smVisionInfo.g_arrPackageColorDontCareROIs.Count > i)
                    {
                        for (int j = 0; j < m_smVisionInfo.g_arrPackageColorDontCareROIs[i].Count; j++)
                        {
                            for (int k = 0; k < m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j].Count; k++)
                            {
                                m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j][k].AttachImage(m_smVisionInfo.g_arrPackageROIs[i][1]);
                            }
                        }
                    }

                }
            }
            Polygon.LoadPolygon(strPath + "Template\\ColorPolygon.xml", m_smVisionInfo.g_arrPolygon_PackageColor, m_smVisionInfo.g_intUnitsOnImage);

            m_blnFormOpened = false;
            this.Close();
            this.Dispose();
        }
        private bool CheckSameDefectNameImageNo(ref string strErrorMessage)
        {
            bool blnResult = true;
            //List<string> arrDefectName = new List<string>();
            //List<int> arrImageNo = new List<int>();
            List<int> arrSkipNo = new List<int>();
            for (int i = 0; i < dgd_ThresholdSetting.Rows.Count; i++)
            {
                if (arrSkipNo.Contains(i))
                    continue;

                int intDefectImageNo = (dgd_ThresholdSetting.Rows[i].Cells[11] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[i].Cells[11].EditedFormattedValue);
                string strDefectName = dgd_ThresholdSetting.Rows[i].Cells[1].Value.ToString();

                for (int j = 0; j < dgd_ThresholdSetting.Rows.Count; j++)
                {
                    if (i == j || arrSkipNo.Contains(j))
                        continue;

                    int intDefectImageNo2 = (dgd_ThresholdSetting.Rows[j].Cells[11] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[j].Cells[11].EditedFormattedValue);
                    string strDefectName2 = dgd_ThresholdSetting.Rows[j].Cells[1].Value.ToString();
                    if (strDefectName == strDefectName2 && intDefectImageNo != intDefectImageNo2)
                    {
                        blnResult = false;
                        strErrorMessage = "\t- " + strDefectName;
                        break;
                    }
                    else if (strDefectName == strDefectName2)
                    {
                        if (!arrSkipNo.Contains(j))
                            arrSkipNo.Add(j);
                    }
                }

                if (!blnResult)
                    break;
            }
            return blnResult;
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            string strErrorMessage = "";
            if (!CheckSameDefectNameImageNo(ref strErrorMessage))
            {
                SRMMessageBox.Show("Same defect name must use same image number :\n" + strErrorMessage,
                                  "SRM Vision", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<int> arrDontCareMode = new List<int>();
            for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode.Count; j++)
                arrDontCareMode.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode[j]);

            List<int> arrROITop = new List<int>();
            for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Top.Count; j++)
                arrROITop.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Top[j]);

            List<int> arrROIRight = new List<int>();
            for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Right.Count; j++)
                arrROIRight.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Right[j]);

            List<int> arrROIBottom = new List<int>();
            for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Bottom.Count; j++)
                arrROIBottom.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Bottom[j]);

            List<int> arrROILeft = new List<int>();
            for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Left.Count; j++)
                arrROILeft.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Left[j]);

            List<int> arrFailCondition = new List<int>();
            for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionFailCondition.Count; j++)
                arrFailCondition.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionFailCondition[j]);

            List<float> arrWidthSetting = new List<float>();
            for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionWidth.Count; j++)
                arrWidthSetting.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionWidth[j]);

            List<float> arrLengthSetting = new List<float>();
            for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionLength.Count; j++)
                arrLengthSetting.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionLength[j]);

            List<float> arrMinAreaSetting = new List<float>();
            for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionMinArea.Count; j++)
                arrMinAreaSetting.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionMinArea[j]);

            List<float> arrMaxAreaSetting = new List<float>();
            for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionMaxArea.Count; j++)
                arrMaxAreaSetting.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionMaxArea[j]);

            List<float> arrTotalAreaSetting = new List<float>();
            for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionTotalArea.Count; j++)
                arrTotalAreaSetting.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionTotalArea[j]);

            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ResetColorThresholdData();
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
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].AddColorThresholdData(
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
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].AddColorThresholdData(
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
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].AddColorThresholdData(
                        Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[1].Value), intColorSystem, //Convert.ToString(dgd_ThresholdSetting.Rows[i].Cells[2].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[12].Value), Convert.ToBoolean(dgd_ThresholdSetting.Rows[i].Cells[13].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[3].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[5].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[7].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[4].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[6].Value), Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[8].Value),
                        Convert.ToInt32(dgd_ThresholdSetting.Rows[i].Cells[9].Value), intDefectType, intDefectImageNo, intDontCareMode, intFailCondition,
                        fWidth, fLength, fMinArea, fMaxArea, fTotalArea,
                        intROITop, intROIRight, intROIBottom, intROILeft);
                }
            }

            CopyToleranceAndOptionToSameDefectName();

            m_blnFormOpened = false;
            this.Close();
            this.Dispose();
        }
        private void CopyToleranceAndOptionToSameDefectName()
        {
            List<int> arrSkipNo = new List<int>();
            for (int i = 0; i < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName.Count; i++)
            {
                if (arrSkipNo.Contains(i))
                    continue;
                
                string strDefectName = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i].ToString();

                for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName.Count; j++)
                {
                    if (i == j || arrSkipNo.Contains(j))
                        continue;
                    
                    string strDefectName2 = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[j].ToString();
                    if (strDefectName == strDefectName2)
                    {
                        if (!arrSkipNo.Contains(j))
                            arrSkipNo.Add(j);

                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionFailCondition[j] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionFailCondition[i];
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionWidth[j] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionWidth[i];
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionLength[j] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionLength[i];
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionMinArea[j] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionMinArea[i];
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionMaxArea[j] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionMaxArea[i];
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionTotalArea[j] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionTotalArea[i];

                        if (i == 0)
                        {
                            if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x01) > 0)
                            {
                                if (j == 1)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x04;
                                else if (j == 2)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x10;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x100;
                            }
                            else
                            {
                                if (j == 1)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x04;
                                else if (j == 2)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x10;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x100;
                            }

                            if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x02) > 0)
                            {
                                if (j == 1)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x08;
                                else if (j == 2)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x20;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x200;
                            }
                            else
                            {
                                if (j == 1)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x08;
                                else if (j == 2)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x20;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x200;
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
                            if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x04) > 0)
                            {
                                if (j == 2)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x10;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x100;
                            }
                            else
                            {
                                if (j == 2)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x10;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x100;
                            }

                            if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x08) > 0)
                            {
                                if (j == 2)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x20;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x200;
                            }
                            else
                            {
                                if (j == 2)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x20;
                                else if (j == 3)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x200;
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
                            if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x10) > 0)
                            {
                                if (j == 3)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x100;
                            }
                            else
                            {
                                if (j == 3)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x40;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x100;
                            }

                            if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x20) > 0)
                            {
                                if (j == 3)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x200;
                            }
                            else
                            {
                                if (j == 3)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x80;
                                else if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x200;
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
                            if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x40) > 0)
                            {
                                if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x100;
                            }
                            else
                            {
                                if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x100;
                            }

                            if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x80) > 0)
                            {
                                if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask |= 0x200;
                            }
                            else
                            {
                                if (j == 4)
                                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask &= ~0x200;
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
        private void ProduceDontCareImage()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPackageColorDontCareROIs.Count; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedUnit)
                    continue;

                for (int j = 0; j < m_smVisionInfo.g_arrPackageColorDontCareROIs[i].Count; j++)
                {
                    if (j != m_smVisionInfo.g_intSelectedColorThresholdIndex)
                        continue;
                    if (m_objDontCareImage == null)
                        m_objDontCareImage = new ImageDrawing(true, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight);
                    m_smVisionInfo.g_objBlackImage.CopyTo(m_objDontCareImage);

                    for (int k = 0; k < m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j].Count; k++)
                    {
                        //if(k==0)
                        m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k].ClearPolygon();
                        if (m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k].ref_intFormMode != 2)
                        {
                            m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k].AddPoint(new PointF(m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j][k].ref_ROITotalX * m_smVisionInfo.g_fScaleX,
                                m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j][k].ref_ROITotalY * m_smVisionInfo.g_fScaleY));
                            m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k].AddPoint(new PointF((m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j][k].ref_ROITotalX + m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j][k].ref_ROIWidth) * m_smVisionInfo.g_fScaleX,
                                (m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j][k].ref_ROITotalY + m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j][k].ref_ROIHeight) * m_smVisionInfo.g_fScaleY));

                            m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k].AddPolygon((int)(m_smVisionInfo.g_arrPackageROIs[i][1].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX), (int)(m_smVisionInfo.g_arrPackageROIs[i][1].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY));
                        }
                        else
                        {
                            m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k].ResetPointsUsingOffset(m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j][k].ref_ROICenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_arrPackageColorDontCareROIs[i][j][k].ref_ROICenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);
                            m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k].AddPolygon((int)(m_smVisionInfo.g_arrPackageROIs[i][1].ref_ROITotalCenterX * m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale), (int)(m_smVisionInfo.g_arrPackageROIs[i][1].ref_ROITotalCenterY * m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale));
                        }

                        ImageDrawing objImage = new ImageDrawing(true);
                        if (m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k].ref_intFormMode != 2)
                            Polygon.CreateDontCareImageWithPolygonPattern_ExtendEdge(m_smVisionInfo.g_arrPackageROIs[i][1], m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        else
                            Polygon.CreateDontCareImageWithPolygonPattern(m_smVisionInfo.g_arrPackageROIs[i][1], m_smVisionInfo.g_arrPolygon_PackageColor[i][j][k], m_smVisionInfo.g_objBlackImage, ref objImage, m_smVisionInfo.g_fScaleX / m_smVisionInfo.g_fZoomScale, m_smVisionInfo.g_fScaleY / m_smVisionInfo.g_fZoomScale);

                        ImageDrawing.AddTwoImageTogether(ref objImage, ref m_objDontCareImage);
                   
                        objImage.Dispose();


                    }
            
                }
            }

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
                        if (m_smVisionInfo.g_arrPackageROIs != null && m_smVisionInfo.g_arrPackageROIs.Count > m_smVisionInfo.g_intSelectedUnit && m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit].Count > 1)
                        {
                            if (m_objThresholdImage == null)
                                m_objThresholdImage = new ImageDrawing(true, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight);
                            if (m_objDontCareImage == null)
                                m_objDontCareImage = new ImageDrawing(true, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight);
                            m_objDontCareImage.SetImageToBlack();

                            if (m_objROI == null)
                                m_objROI = new ROI();
                            m_objROI.AttachImage(m_objThresholdImage);
                            m_objROI.LoadROISetting(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalX,
                                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROITotalY,
                                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth,
                                m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight);

                            ProduceDontCareImage();
                            //m_objDontCareImage.SaveImage("D:\\TS\\DontCare.bmp");

                            m_smVisionInfo.g_arrPackageColorROIs[m_smVisionInfo.g_intSelectedUnit][0].ThresholdTo_ROIToImage(ref m_objThresholdImage, m_smVisionInfo.g_intColorThreshold, m_smVisionInfo.g_intColorTolerance, m_smVisionInfo.g_intColorFormat, m_smVisionInfo.g_intColorCloseIteration, m_smVisionInfo.g_blnColorInvertBlackWhite);
                            //m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                            
                            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SubtractColorDontCareImage_Learn(m_objROI, m_intDontCareMode, m_objDontCareImage, m_smVisionInfo.g_objBlackImage, arrStartX, arrStartY, arrEndX, arrEndY, m_smVisionInfo.g_arrPackageColorROIs[m_smVisionInfo.g_intSelectedUnit][0]);

                            int Max = Math.Max((int)(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth), (int)(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight));
                            //m_Graphic.Clear(pic_ThresholdImage.BackColor);
                            //m_objThresholdImage.RedrawImage(m_Graphic, pic_ThresholdImage.Size.Width / Max2, pic_ThresholdImage.Size.Height / Max2);

                            pic_ThresholdImage.Size = new Size(Math.Min((int)Math.Ceiling((250f / Max) * (m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth)), 250),
                                Math.Min((int)Math.Ceiling((250f / Max) * (m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight)), 250));

                            m_objThresholdImage.SetImageSize(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIWidth, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight);
                            panel1.Height = Math.Min((int)Math.Ceiling((250f / Max) * (m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1].ref_ROIHeight)), 250);

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
                if (m_smVisionInfo.g_arrPackageColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedUnit && m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex && m_smVisionInfo.g_intSelectedColorThresholdIndex != -1)
                {
                    if ((m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex) && (m_smVisionInfo.g_intSelectedDontCareROIIndex != -1))
                    {
                        cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
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

        }
        private void ColorPackageThresholdForm_Load(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPackageColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedUnit
                && (m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count > 0)
                && m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][0].Count > 0)
            {
                cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][0][0].ref_intFormMode;
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
        private void ColorPackageThresholdForm_FormClosing(object sender, FormClosingEventArgs e)
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
                for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode.Count; j++)
                    arrDontCareMode.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode[j]);

                List<int> arrROITop = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Top.Count; j++)
                    arrROITop.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Top[j]);

                List<int> arrROIRight = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Right.Count; j++)
                    arrROIRight.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Right[j]);

                List<int> arrROIBottom = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Bottom.Count; j++)
                    arrROIBottom.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Bottom[j]);

                List<int> arrROILeft = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Left.Count; j++)
                    arrROILeft.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspection_Left[j]);

                List<int> arrFailCondition = new List<int>();
                for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionFailCondition.Count; j++)
                    arrFailCondition.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionFailCondition[j]);

                List<float> arrWidthSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionWidth.Count; j++)
                    arrWidthSetting.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionWidth[j]);

                List<float> arrLengthSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionLength.Count; j++)
                    arrLengthSetting.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionLength[j]);

                List<float> arrMinAreaSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionMinArea.Count; j++)
                    arrMinAreaSetting.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionMinArea[j]);

                List<float> arrMaxAreaSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionMaxArea.Count; j++)
                    arrMaxAreaSetting.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionMaxArea[j]);

                List<float> arrTotalAreaSetting = new List<float>();
                for (int j = 0; j < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionTotalArea.Count; j++)
                    arrTotalAreaSetting.Add(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorInspectionTotalArea[j]);

                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ResetColorThresholdData();
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
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].AddColorThresholdData(
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
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].AddColorThresholdData(
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
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].AddColorThresholdData(
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
                
                if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode.Count > index)
                {
                    switch (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode[index])
                    {
                        case 0:
                            radioButton_NoneDontCare.Checked = true;
                            m_intDontCareMode = 0;
                            pnl_DrawMethod.Visible = false;
                            break;
                        case 1:
                            radioButton_MarkDontCare.Checked = true;
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
                    
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].DeleteColorThresholdData(m_intRowIndexPrev);
                    DeletePolygon(m_smVisionInfo.g_intSelectedUnit);
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
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode.Count > dgd_ThresholdSetting.RowCount - 1)
                    {
                        switch (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode[dgd_ThresholdSetting.RowCount - 1])
                        {
                            case 0:
                                radioButton_NoneDontCare.Checked = true;
                                m_intDontCareMode = 0;
                                pnl_DrawMethod.Visible = false;
                                break;
                            case 1:
                                radioButton_MarkDontCare.Checked = true;
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

                    if (m_smVisionInfo.g_arrPackageColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedUnit)
                        if (m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex)
                            //if (m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count <= m_smVisionInfo.g_intSelectedDontCareROIIndex)
                                m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1;

                }
            }

            UpdateROIToleranceGUI();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode.Count > e.RowIndex)
            {
                switch (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode[e.RowIndex])
                {
                    case 0:
                        radioButton_NoneDontCare.Checked = true;
                        m_intDontCareMode = 0;
                        pnl_DrawMethod.Visible = false;
                        break;
                    case 1:
                        radioButton_MarkDontCare.Checked = true;
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

            if (m_smVisionInfo.g_arrPackageColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedUnit)
                if (m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex)
                    //if (m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count <= m_smVisionInfo.g_intSelectedDontCareROIIndex)
                        m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1;

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
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[r] = dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString();
                }
            }
            else if (c == 2) // Color System
            {
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString() != "")
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorSystem[r] = (dgd_ThresholdSetting.Rows[r].Cells[c] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[r].Cells[c].EditedFormattedValue);
                }
            }
            else if (c == 3) // Threshold 1 value
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0 && intValue < 256)
                    {
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[r][0] = intValue;
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[r][0];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[r][0];
                }
            }
            else if (c == 4) // Threshold 1 Tolerance
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0)
                    {
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[r][0] = intValue;
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[r][0];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[r][0];
                }
            }
            else if (c == 5) // Threshold 2 value
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0 && intValue < 256)
                    {
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[r][1] = intValue;
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[r][1];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[r][1];
                }
            }
            else if (c == 6) // Threshold 2 Tolerance
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0)
                    {
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[r][1] = intValue;
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[r][1];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[r][1];
                }
            }
            else if (c == 7) // Threshold 3 value
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0 && intValue < 256)
                    {
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[r][2] = intValue;
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[r][2];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor[r][2];
                }
            }
            else if (c == 8) // Threshold 3 Tolerance
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0)
                    {
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[r][2] = intValue;
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[r][2];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorTolerance[r][2];
                }
            }
            else if (c == 9) // Filter Min Area
            {
                int intValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && int.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out intValue))
                {
                    if (intValue >= 0)
                    {
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorMinArea[r] = intValue;
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorMinArea[r];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorMinArea[r];
                }
            }
            else if (c == 10) // Defect Type
            {
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString() != "")
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectType[r] = (dgd_ThresholdSetting.Rows[r].Cells[c] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[r].Cells[c].EditedFormattedValue);
                }
            }
            else if (c == 11) // Image No
            {
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString() != "")
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectImageNo[r] = (dgd_ThresholdSetting.Rows[r].Cells[c] as DataGridViewComboBoxCell).Items.IndexOf(dgd_ThresholdSetting.Rows[r].Cells[c].EditedFormattedValue);

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
                        m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectCloseIteration[r] = intValue;
                    }
                    else
                    {
                        dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectCloseIteration[r];
                    }
                }
                else
                {
                    dgd_ThresholdSetting.Rows[r].Cells[c].Value = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectCloseIteration[r];
                }
            }
            else if (c == 13) // Invert Black White
            {
                bool blnValue;
                if (dgd_ThresholdSetting.Rows[r].Cells[c].Value != null && bool.TryParse(dgd_ThresholdSetting.Rows[r].Cells[c].Value.ToString(), out blnValue))
                {
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectInvertBlackWhite[r] = blnValue;
                }
            }

            UpdateSelectedThresholdValue(r);//e.RowIndex
            UpdateLSHColorBar();

            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void RotateColorImage()
        {
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
            {
                for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                {
                    CROI objRotateROI = new CROI();
                    objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[intImageIndex]);
                    objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                   (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                   m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                   m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                    CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[intImageIndex], objRotateROI,
                                              m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle, 8,
                                              ref m_smVisionInfo.g_arrColorRotatedImages, intImageIndex);
                }
            }
            else
            {
                for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                {
                    m_smVisionInfo.g_arrColorImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[intImageIndex]);
                }
            }

            m_smVisionInfo.g_blnViewRotatedImage = true;

            m_smVisionInfo.g_arrPackageColorROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            
            m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage].ConvertColorToMono(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);

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
                case "radioButton_MarkDontCare":
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
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode[m_smVisionInfo.g_intSelectedColorThresholdIndex] = m_intDontCareMode;
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void btn_Undo_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnDrawFreeShapeDone = true;
            
            if (m_smVisionInfo.g_arrPolygon_PackageColor.Count > m_smVisionInfo.g_intSelectedUnit)
                if (m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex && m_smVisionInfo.g_intSelectedColorThresholdIndex != -1)
                    if (m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex && m_smVisionInfo.g_intSelectedDontCareROIIndex != -1)
                        m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex][m_smVisionInfo.g_intSelectedDontCareROIIndex].UndoPolygon();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void cbo_DontCareAreaDrawMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            
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

            for (int i = m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count; i < dgd_ThresholdSetting.RowCount; i++)
            {
                m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Add(new List<ROI>());
            }

            m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Add(new ROI());
            m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex][m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1].AttachImage(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);

            for (int i = m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit].Count; i < dgd_ThresholdSetting.RowCount; i++)
            {
                m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit].Add(new List<Polygon>());
            }

            m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Add(new Polygon());
            m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex][m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1].ref_intFormMode = cbo_DontCareAreaDrawMethod.SelectedIndex;
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void btn_DeleteDontCareROI_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intSelectedColorThresholdIndex == -1 || m_smVisionInfo.g_intSelectedDontCareROIIndex == -1)
                return;

            if (m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count == 0)
                return;

            m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].RemoveAt(m_smVisionInfo.g_intSelectedDontCareROIIndex);
            m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1;
            if (m_intSelectedDontCareROIIndexPrev != m_smVisionInfo.g_intSelectedDontCareROIIndex)
            {
                m_intSelectedDontCareROIIndexPrev = m_smVisionInfo.g_intSelectedDontCareROIIndex;
                if (m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex && m_smVisionInfo.g_intSelectedColorThresholdIndex != -1)
                {
                    if ((m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count > m_smVisionInfo.g_intSelectedDontCareROIIndex) && (m_smVisionInfo.g_intSelectedDontCareROIIndex != -1))
                    {
                        cbo_DontCareAreaDrawMethod.SelectedIndex = m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex][m_smVisionInfo.g_intSelectedDontCareROIIndex].ref_intFormMode;
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
        private void DeletePolygon(int intUnitIndex)
        {
            if (m_smVisionInfo.g_intSelectedColorThresholdIndex == -1)
                return;

            if (m_smVisionInfo.g_arrPackageColorDontCareROIs[intUnitIndex].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex)
            {
                m_smVisionInfo.g_arrPackageColorDontCareROIs[intUnitIndex].RemoveAt(m_smVisionInfo.g_intSelectedColorThresholdIndex);
                m_smVisionInfo.g_arrPolygon_PackageColor[intUnitIndex].RemoveAt(m_smVisionInfo.g_intSelectedColorThresholdIndex);
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

            for (int i = 0; i < m_smVisionInfo.g_arrPackage.Count; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedUnit)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_StartPixelFromRight.Text = txt_StartPixelFromTop.Text;
                    txt_StartPixelFromBottom.Text = txt_StartPixelFromTop.Text;
                    txt_StartPixelFromLeft.Text = txt_StartPixelFromTop.Text;
                }

                m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                if (chk_SetToAllEdge.Checked)
                {
                    m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                    m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                    m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromTop.Text);
                }
            }
           
            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_StartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || m_smVisionInfo.g_intSelectedColorThresholdIndex == -1)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPackage.Count; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedUnit)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_StartPixelFromTop.Text = txt_StartPixelFromRight.Text;
                    txt_StartPixelFromBottom.Text = txt_StartPixelFromRight.Text;
                    txt_StartPixelFromLeft.Text = txt_StartPixelFromRight.Text;
                }

                m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                if (chk_SetToAllEdge.Checked)
                {
                    m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                    m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                    m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromRight.Text);
                }
            }

            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_StartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || m_smVisionInfo.g_intSelectedColorThresholdIndex == -1)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPackage.Count; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedUnit)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_StartPixelFromTop.Text = txt_StartPixelFromBottom.Text;
                    txt_StartPixelFromRight.Text = txt_StartPixelFromBottom.Text;
                    txt_StartPixelFromLeft.Text = txt_StartPixelFromBottom.Text;
                }

                m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                if (chk_SetToAllEdge.Checked)
                {
                    m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                    m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                    m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromBottom.Text);
                }
            }

            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_StartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || m_smVisionInfo.g_intSelectedColorThresholdIndex == -1)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPackage.Count; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedUnit)
                    continue;

                if (chk_SetToAllEdge.Checked)
                {
                    txt_StartPixelFromTop.Text = txt_StartPixelFromLeft.Text;
                    txt_StartPixelFromRight.Text = txt_StartPixelFromLeft.Text;
                    txt_StartPixelFromBottom.Text = txt_StartPixelFromLeft.Text;
                }

                m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Left[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                if (chk_SetToAllEdge.Checked)
                {
                    m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Top[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                    m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Right[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
                    m_smVisionInfo.g_arrPackage[i].ref_arrDefectColorInspection_Bottom[m_smVisionInfo.g_intSelectedColorThresholdIndex] = Convert.ToInt32(txt_StartPixelFromLeft.Text);
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
            if (m_smVisionInfo.g_arrPackageColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedUnit && m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count > intTo)
                arrROI = m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][intTo];
            if (m_smVisionInfo.g_arrPolygon_PackageColor.Count > m_smVisionInfo.g_intSelectedUnit && m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit].Count > intTo)
                arrPolygon = m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][intTo];

            if (m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count <= intTo)
            {
                m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Add(new List<ROI>());
            }
            if (m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit].Count <= intTo)
            {
                m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit].Add(new List<Polygon>());
            }

            if (m_smVisionInfo.g_arrPackageColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedUnit && m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count > intTo && m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count > intFrom)
                m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][intTo] = m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][intFrom];
            if (m_smVisionInfo.g_arrPolygon_PackageColor.Count > m_smVisionInfo.g_intSelectedUnit && m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit].Count > intTo && m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit].Count > intFrom)
                m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][intTo] = m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][intFrom];

            if (m_smVisionInfo.g_arrPackageColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedUnit && m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count > intFrom)
                m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][intFrom] = arrROI;
            if (m_smVisionInfo.g_arrPolygon_PackageColor.Count > m_smVisionInfo.g_intSelectedUnit && m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit].Count > intFrom)
                m_smVisionInfo.g_arrPolygon_PackageColor[m_smVisionInfo.g_intSelectedUnit][intFrom] = arrPolygon;

            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].SwapColorDefect(intFrom, intTo);


            if (intTo < 0)
                return;

            UpdateSelectedThresholdValue(intTo);
            UpdateLSHColorBar();
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode.Count > intTo)
            {
                switch (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectDontCareMode[intTo])
                {
                    case 0:
                        radioButton_NoneDontCare.Checked = true;
                        m_intDontCareMode = 0;
                        pnl_DrawMethod.Visible = false;
                        break;
                    case 1:
                        radioButton_MarkDontCare.Checked = true;
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

            if (m_smVisionInfo.g_arrPackageColorDontCareROIs.Count > m_smVisionInfo.g_intSelectedUnit)
                if (m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit].Count > m_smVisionInfo.g_intSelectedColorThresholdIndex)
                    //if (m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count <= m_smVisionInfo.g_intSelectedDontCareROIIndex)
                    m_smVisionInfo.g_intSelectedDontCareROIIndex = m_smVisionInfo.g_arrPackageColorDontCareROIs[m_smVisionInfo.g_intSelectedUnit][m_smVisionInfo.g_intSelectedColorThresholdIndex].Count - 1;

            UpdateROIToleranceGUI();

            m_blnUpdateImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private bool StartOrientTest()
        {
            m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
            for (int i = 1; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                m_smVisionInfo.g_arrImages[i].CopyTo(m_smVisionInfo.g_arrRotatedImages[i]);
            }
            // make sure template learn
            if (m_smVisionInfo.g_arrOrients[0].Count == 0 || m_smVisionInfo.g_intTotalTemplates == 0)
            {
                m_smVisionInfo.g_strErrorMessage = "*Orient : No Template Found";
                m_smVisionInfo.g_strErrorMessage += "*Please relearn template using wizard.";
                return false;
            }

            // reset all inspection data
            for (int i = 0; i < m_smVisionInfo.g_arrOrients[0].Count; i++)
            {
                ((Orient)m_smVisionInfo.g_arrOrients[0][i]).ResetInspectionData();
            }

            float fUnitSurfaceOffsetX = 0;
            float fUnitSurfaceOffsetY = 0;
            float fUnitPRResultCenterX = 0;
            float fUnitPRResultCenterY = 0;
            float fUnitPRResultAngle = 0;

            // Use Gauge to find unit angle and rotate it to 0 deg
            if (m_smVisionInfo.g_blnWantGauge && ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
            {
                m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrImages[0]);

                // Add gain value to image and attached all position ROI to gain image.
                float fGaugeAngle;
                bool blnGaugeResult = false;
                //if (m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue != 1000)  // 2019 05 29 - CCENG: Enable this gain feature because hard to get unit edge sometime. Dun know why this feature disabled previously
                //{
                //    m_smVisionInfo.g_arrImages[0].AddGain(ref m_objOrientGainImage, m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fGainValue / 1000);
                //    m_smVisionInfo.g_arrOrientGaugeM4L[0].SetGaugePlace_BasedOnEdgeROI();
                //    blnGaugeResult = m_smVisionInfo.g_arrOrientGaugeM4L[0].Measure_WithDontCareArea(m_objOrientGainImage, m_smVisionInfo.g_objWhiteImage);
                //    fGaugeAngle = m_fOrientGauge = m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fRectAngle;
                //}
                //else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[0].SetGaugePlace_BasedOnEdgeROI();
                    blnGaugeResult = m_smVisionInfo.g_arrOrientGaugeM4L[0].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage);
                    fGaugeAngle = m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_fRectAngle;
                }
                if (!blnGaugeResult)
                {
                    m_smVisionInfo.g_strErrorMessage = "*Orient : " + m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_strErrorMessage;
                    m_smVisionInfo.g_strErrorMessage += "*Please relearn gauge.";
                    return false;
                }
                // RotateROI has same center point with gauge measure center point.
                ROI objRotateROI = new ROI();
                objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                objRotateROI.LoadROISetting(
                    (int)Math.Round(m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_pRectCenterPoint.X -
                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_pRectCenterPoint.Y -
                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth,
                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight);

                // Rotate unit to exact 0 degree (m_fOrientGauge used in Package)
                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotateROI, fGaugeAngle, 0, ref m_smVisionInfo.g_arrRotatedImages, 0); // Clear image is not so important in Orient Matching. Use interpolation 0 to save rotation time.
                objRotateROI.Dispose();
            }
            else // No rect gauge
            {
                //// If Lead Unit, use unit lead pattern to find unit surface ROI
                //if (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_blnUnitPRMatcherExist)
                //{
                //    m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].MatchWithTemplateUnitPR(m_smVisionInfo.g_arrOrientROIs[0][0]);
                //    fUnitPRResultCenterX = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterX();
                //    fUnitPRResultCenterY = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterY();
                //    fUnitPRResultAngle = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultAngle();
                //    fUnitSurfaceOffsetX = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetX;
                //    fUnitSurfaceOffsetY = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetY;

                //    if (!Math2.GetNewXYAfterRotate(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + fUnitPRResultCenterX,
                //                                  m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + fUnitPRResultCenterY,
                //                                  fUnitSurfaceOffsetX,
                //                                  fUnitSurfaceOffsetY,
                //                                  fUnitPRResultAngle,
                //                                  ref fUnitSurfaceOffsetX,
                //                                  ref fUnitSurfaceOffsetY))
                //    { }

                //    ROI objRotateROI = new ROI();
                //    objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                //    objRotateROI.LoadROISetting(
                //        (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX +
                //        fUnitPRResultCenterX -
                //        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2 +
                //        fUnitSurfaceOffsetX, 0, MidpointRounding.AwayFromZero),
                //        (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY +
                //        fUnitPRResultCenterY -
                //        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2 +
                //        fUnitSurfaceOffsetY, 0, MidpointRounding.AwayFromZero),
                //        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth,
                //        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight);


                //    // Rotate unit to exact 0 degree (m_fOrientGauge used in Package)
                //    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotateROI, fUnitPRResultAngle, 0, ref m_smVisionInfo.g_arrRotatedImages, 0); // Clear image is not so important in Orient Matching. Use interpolation 0 to save rotation time.

                //    objRotateROI.Dispose();
                //}
                //else if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) > 0)    // Bottom Orient
                //{
                //    m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
                //}
                //else // if not lead unit, mean it is QFN. There is no way to find unit surface ROI without gauge tool.
                {
                    m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
                }
            }

            int intMatchCount = 0;
            m_smVisionInfo.g_intOrientResult[0] = -1;  // 0:0deg, 1:90deg, 2:180deg, 3:-90, 4:Fail

            //else // Whole active templates test
            if (m_smVisionInfo.g_blnWantSetTemplateBasedOnBinInfo)
            {
                float fHighestScore = -1;
                m_smVisionInfo.g_arrMarks[0].ref_blnInspectAllTemplate = true;
                int intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate;
                if (intTemplateIndex >= 0)
                {
                    int intAngle;
                    if (m_smVisionInfo.g_intTemplateMask == 0 || (m_smVisionInfo.g_intTemplateMask & (1 << intTemplateIndex)) > 0)
                    {
                        m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

                        intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection(
                                m_smVisionInfo.g_arrOrientROIs[0][0], 2, !m_smVisionInfo.g_blnWantGauge);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.


                        if (m_smVisionInfo.g_arrOrients[0][intTemplateIndex].GetMinScore() > fHighestScore)
                        {
                            fHighestScore = m_smVisionInfo.g_fOrientScore[0] = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].GetMinScore();
                            m_smVisionInfo.g_intSelectedOcv[0] = intTemplateIndex;
                            m_smVisionInfo.g_intOrientResult[0] = intAngle;

                        }
                    }
                }
            }
            else
            {

                float fHighestScore = -1;
                m_smVisionInfo.g_arrMarks[0].ref_blnInspectAllTemplate = true;
                do
                {
                    int intTemplateIndex = (int)((m_smVisionInfo.g_intTemplatePriority >> (0x04 * intMatchCount)) & 0x0F) - 1;
                    if (intTemplateIndex >= 0)
                    {
                        int intAngle;
                        bool blnPreciseAngleResult = true;
                        if (m_smVisionInfo.g_intTemplateMask == 0 || (m_smVisionInfo.g_intTemplateMask & (1 << intTemplateIndex)) > 0)
                        {
                            if (((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0) || ((m_smCustomizeInfo.g_intWantOCR & (1 << m_smVisionInfo.g_intVisionPos)) > 0) ||
                                (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                            {
                                m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

                                intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection(
                                m_smVisionInfo.g_arrOrientROIs[0][0], 2, !m_smVisionInfo.g_blnWantGauge,
                                m_smVisionInfo.g_arrMarks[0].GetMarkAngleTolerance(0, intTemplateIndex),
                                ref blnPreciseAngleResult);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.

                                //intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection(
                                //    m_smVisionInfo.g_arrOrientROIs[0][0], 2, !m_smVisionInfo.g_blnWantGauge);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.
                            }
                            else
                            {
                                m_smVisionInfo.g_arrOrientROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

                                intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection_WithSubMatcher4(m_smVisionInfo.g_arrRotatedImages[0],
                                            m_smVisionInfo.g_arrOrientROIs[0][0], m_smVisionInfo.g_arrOrientROIs[0][1], m_smVisionInfo.g_arrOrientROIs[0][2], 2);  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.

                                //intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].DoOrientationInspection_WithSubMatcher(
                                //    m_smVisionInfo.g_arrOrientROIs[0][0], 2, true, m_smVisionInfo.g_blnWantSubROI && (m_smVisionInfo.g_arrOrientROIs[0].Count > 2));  // Use FinalReduction=2 because match center point is not very important in MarkOrient Test.

                                //intAngle = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].MatchWithTemplate(
                                //    m_smVisionInfo.g_arrOrientROIs[0][0], m_smVisionInfo.g_arrRotatedImages[0], true, fHighestScore, true, // false
                                //    m_smVisionInfo.g_blnWantSubROI && (m_smVisionInfo.g_arrOrientROIs[0].Count > 2));
                            }

                            if (m_smVisionInfo.g_arrOrients[0][intTemplateIndex].GetMinScore() > fHighestScore)
                            {
                                fHighestScore = m_smVisionInfo.g_fOrientScore[0] = m_smVisionInfo.g_arrOrients[0][intTemplateIndex].GetMinScore();
                                m_smVisionInfo.g_intSelectedOcv[0] = intTemplateIndex;
                                m_smVisionInfo.g_intOrientResult[0] = intAngle;

                            }
                        }
                    }
                    intMatchCount++;
                    //} while (((fHighestScore < 0.8) && (intMatchCount < m_smVisionInfo.g_arrOrients[0].Count)) || !blnAngleResult); 
                } while (intMatchCount < m_smVisionInfo.g_arrOrients[0].Count);
            }

            Orient objOrient = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]];

            m_smVisionInfo.g_fOrientCenterX[0] = objOrient.ref_fObjectX;
            m_smVisionInfo.g_fOrientCenterY[0] = objOrient.ref_fObjectY;
            m_smVisionInfo.g_fSubOrientCenterX[0] = objOrient.ref_fSubObjectX;
            m_smVisionInfo.g_fSubOrientCenterY[0] = objOrient.ref_fSubObjectY;
            m_smVisionInfo.g_fOrientScore[0] = objOrient.GetMinScore();
            m_smVisionInfo.g_fOrientAngle[0] = objOrient.ref_fDegAngleResult;
            //m_smVisionInfo.g_blnViewOrientObject = true;

            //if ((m_blnWantOCVMark) || ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
            {
                if (m_smVisionInfo.g_intOrientResult[0] < 4)
                {
                    if (objOrient.ref_blnWantUsePositionCheckOrientation)
                    {
                        // 2020 11 20 - CCENG: Method 2 orientation
                        float fOrientationSeparatorX = (float)m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2;
                        float fOrientationSeparatorY = (float)m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2;

                        for (int m = 0; m < objOrient.ref_arrMatchScoreList.Count; m++)
                        {
                            if (m == m_smVisionInfo.g_intOrientResult[0])
                                continue;

                            if (objOrient.ref_arrMatchScoreList[m] >= objOrient.ref_fMinScore)
                            {
                                if (Math.Abs(objOrient.GetMinScore() - objOrient.ref_arrMatchScoreList[m]) < objOrient.ref_fCheckPositionOrientationWhenBelowDifferentScore)
                                {
                                    if (objOrient.ref_intDirections == 2)
                                    {
                                        bool blnSampleIsTop = objOrient.ref_fObjectY - fOrientationSeparatorY < 0;
                                        bool blnDirectionisTop = objOrient.ref_blnTemplateOrientationIsTop != blnSampleIsTop;

                                        if (blnDirectionisTop)            // rotate 180  from template point
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 2;
                                        }
                                        else
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 0;            // no rotate
                                        }
                                    }
                                    else
                                    {
                                        bool blnSampleIsLeft = objOrient.ref_fObjectX - fOrientationSeparatorX < 0;
                                        bool blnSampleIsTop = objOrient.ref_fObjectY - fOrientationSeparatorY < 0;

                                        bool blnDirectionIsLeft = objOrient.ref_blnTemplateOrientationIsLeft != blnSampleIsLeft;
                                        bool blnDirectionisTop = objOrient.ref_blnTemplateOrientationIsTop != blnSampleIsTop;

                                        if (blnDirectionIsLeft && blnDirectionisTop)            // rotate 180  from template point
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 2;
                                        }
                                        else if (blnDirectionIsLeft && !blnDirectionisTop)      // rotate 90 ccw from template point
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 1;            // show result angle 90 when rotate ccw from template point
                                        }
                                        else if (!blnDirectionIsLeft && blnDirectionisTop)      // rotate 90 cw from template point
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 3;            // show result angle -90 when rotate cw from template point
                                        }
                                        else
                                        {
                                            m_smVisionInfo.g_intOrientResult[0] = 0;            // no rotate
                                        }
                                    }

                                    break;
                                }
                            }
                        }
                    }

                    m_intOrientAngle = 0;
                    switch (m_smVisionInfo.g_intOrientResult[0])
                    {
                        default:
                        case 0:
                            m_intOrientAngle = 0;
                            break;
                        case 1:
                            m_intOrientAngle = 90;
                            break;
                        case 2:
                            m_intOrientAngle = 180;
                            break;
                        case 3:
                            m_intOrientAngle = 270;
                            break;
                    }
                    
                    ROI objRotatedROI = new ROI();
                    float fTotalRotateAngle = 0;
                    if (m_smVisionInfo.g_blnWantGauge)
                    {
                        RectGaugeM4L objGauge = m_smVisionInfo.g_arrOrientGaugeM4L[0];

                        // Get Orient Center Point (Final result for next MarkTest and PackageTest)
                        m_smVisionInfo.g_fUnitCenterX[0] = objGauge.ref_pRectCenterPoint.X - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX;
                        m_smVisionInfo.g_fUnitCenterY[0] = objGauge.ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY;

                        // Calculate total angle 
                        fTotalRotateAngle = m_intOrientAngle + objGauge.ref_fRectAngle;

                        // Get RotateROI where the ROI center point == Unit Center Point
                        objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                        float fSizeX, fSizeY;
                        if ((objGauge.ref_fRectWidth + m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth) > m_smVisionInfo.g_arrImages[0].ref_intImageWidth ||
                            (objGauge.ref_fRectHeight + m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight) > m_smVisionInfo.g_arrImages[0].ref_intImageHeight)
                        {
                            fSizeX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight % 2;
                        }
                        else
                        {
                            fSizeX = objGauge.ref_fRectWidth + m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = objGauge.ref_fRectHeight + m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight % 2;
                        }

                        objRotatedROI.LoadROISetting((int)Math.Round(objGauge.ref_pRectCenterPoint.X - fSizeX / 2, 0, MidpointRounding.AwayFromZero),
                                                     (int)Math.Round(objGauge.ref_pRectCenterPoint.Y - fSizeY / 2, 0, MidpointRounding.AwayFromZero),
                                                     (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                                     (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));

                    }
                    else
                    {
                        // For Lead Unit Case
                        if (m_smVisionInfo.g_arrOrientROIs[0].Count > 3 &&
                           (((m_smCustomizeInfo.g_intWantPositioningIndex & (1 << m_smVisionInfo.g_intVisionPos)) > 0) ||
                           (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_blnUnitPRMatcherExist)))    // Mean Unit Surface ROI exist
                        {
                            // Get Unit Surface Center Point (Final result for next MarkTest and PackageTest)
                            m_smVisionInfo.g_fUnitCenterX[0] = fUnitPRResultCenterX + fUnitSurfaceOffsetX;
                            m_smVisionInfo.g_fUnitCenterY[0] = fUnitPRResultCenterY + fUnitSurfaceOffsetY;

                            // Calculate total angle 
                            fTotalRotateAngle = m_intOrientAngle + fUnitPRResultAngle;

                            // Get RotateROI where the ROI center point == Unit Center Point
                            objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                            float fSizeX, fSizeY;
                            fSizeX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight % 2;

                            objRotatedROI.LoadROISetting(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX +
                                                            (int)Math.Round(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterX() -
                                                            fSizeX / 2 +
                                                            m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetX, 0, MidpointRounding.AwayFromZero),
                                                         m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY +
                                                            (int)Math.Round(m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterY() -
                                                            fSizeY / 2 +
                                                            m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetY, 0, MidpointRounding.AwayFromZero),
                                                         (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                                         (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));
                        }
                        else // For No Lead Case
                        {
                            // Calculate total angle 
                            fTotalRotateAngle = m_intOrientAngle + m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fDegAngleResult;
                            
                            // Get RotateROI where the ROI center point == Unit Center Point
                            objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                            float fSizeX, fSizeY;
                            fSizeX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight % 2;

                            // 2019 09 06 - CCENG: If no package size center point, then use Orient Search ROI Center Point
                            objRotatedROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX - fSizeX / 2, 0, MidpointRounding.AwayFromZero),
                                                         (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY - fSizeY / 2, 0, MidpointRounding.AwayFromZero),
                                                         (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                                         (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));

                        }
                    }

                    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotatedROI, fTotalRotateAngle, 4, ref m_smVisionInfo.g_arrRotatedImages, 0);
                    
                    // 2020 04 19 - cceng: Rotate image to mark 0 deg
                    ROI.Rotate0Degree_Better(m_smVisionInfo.g_arrImages[0], objRotatedROI, fTotalRotateAngle, 4, ref m_smVisionInfo.g_objMarkImage);

                    if ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        if (m_smVisionInfo.g_arrImages.Count > m_smVisionInfo.g_arrLead[0].ref_intImageViewNo)
                        {
                            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrLead[0].ref_intImageViewNo], objRotatedROI, fTotalRotateAngle, ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_arrLead[0].ref_intImageViewNo);

                        }
                    }

                    objRotatedROI.Dispose();
                }
            }
            //else
            //{
            //    m_smVisionInfo.g_fOrientCenterX[0] = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fObjectX;
            //    m_smVisionInfo.g_fOrientCenterY[0] = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fObjectY;
            //}

            if (m_smVisionInfo.g_intOrientResult[0] == 4)
            {
                if (m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_strErrorMessage == "" && m_smVisionInfo.g_strErrorMessage == "")
                {
                    m_smVisionInfo.g_strErrorMessage = "*Recipe is corrupted. Please relearn.";
                }
                else
                {
                    m_smVisionInfo.g_strErrorMessage = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_strErrorMessage;
                }

                return false;
            }
            else
            {
                //2022-02-20 ZJYEOH : Dont reposition the orient ROI, because this is not learn mark orient form, reposition orient ROI will cause mark ROI offset during inspection
                ////ROI.RotateROI_Center(objROI, Convert.ToInt32(lbl_OrientationAngle.Text) + m_smVisionInfo.g_fOrientAngle[0], ref m_smVisionInfo.g_arrRotatedImages, 0);

                //float Angle = m_intOrientAngle + m_smVisionInfo.g_fOrientAngle[0]; // this formula is based on clockwise rotation so the angle need to be inverted, 
                //Angle = -Angle;
                //float fXAfterRotated = (float)((m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX) + ((m_smVisionInfo.g_fOrientCenterX[0] - (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX)) * Math.Cos(Angle * Math.PI / 180)) - ((m_smVisionInfo.g_fOrientCenterY[0] - (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY))) * Math.Sin(Angle * Math.PI / 180));

                //float fYAfterRotated = (float)((m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY) + ((m_smVisionInfo.g_fOrientCenterX[0] - (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX)) * Math.Sin(Angle * Math.PI / 180)) + ((m_smVisionInfo.g_fOrientCenterY[0] - (m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY))) * Math.Cos(Angle * Math.PI / 180));


                //m_smVisionInfo.g_arrOrientROIs[0][1].LoadROISetting((int)(fXAfterRotated - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIWidth / 2),
                //  (int)(fYAfterRotated - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIHeight / 2),
                //   m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIWidth,
                //    m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIHeight);

            }
            return true;
        }
        private bool StartMarkTest()
        {
            try
            {
                if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() == 0)
                    {
                        m_smVisionInfo.g_strErrorMessage += "*Mark : No Template Found";
                        return false;
                    }
                    
                    ROI m_objMarkSearchROI = m_smVisionInfo.g_arrMarkROIs[0][0];
                    m_objMarkTrainROI = new ROI();
                    m_objMarkTrainROI.AttachImage(m_objMarkSearchROI);
                    m_objMarkTrainROI.LoadROISetting(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIPositionX, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIPositionY,
                                               m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
                    //m_objMarkTrainROI.SaveImage("D:\\TS\\m_objMarkTrainROI3.bmp");

                    ROI m_objMarkOcvSearchROI = new ROI();
                    m_objMarkOcvSearchROI.AttachImage(m_smVisionInfo.g_objMarkImage);
                    m_objMarkOcvSearchROI.LoadROISetting(m_objMarkSearchROI.ref_ROIPositionX, m_objMarkSearchROI.ref_ROIPositionY,
                                                             m_objMarkSearchROI.ref_ROIWidth, m_objMarkSearchROI.ref_ROIHeight);

                    if (m_smVisionInfo.g_arrOrients[0][0].ref_intCorrectAngleMethod == 0)
                    {
                        //no orient check
                        if (((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) == 0))
                        {
                            //got gauge check
                            if (m_smVisionInfo.g_blnWantGauge)
                            {
                                m_objMarkSearchROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                                m_smVisionInfo.g_arrMarkGaugeM4L[0].Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage);
                                // Rotate unit to exact 0 degree
                                float fGaugeAngle = m_smVisionInfo.g_arrMarkGaugeM4L[0].ref_fRectAngle;

                                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], m_objMarkSearchROI, fGaugeAngle, m_smVisionInfo.g_intRotationInterpolation_Mark, ref m_smVisionInfo.g_arrRotatedImages, 0);
                                m_objMarkSearchROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                                
                            }
                        }
                        else
                        {
                            m_objMarkSearchROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                            if (m_smVisionInfo.g_blnWantGauge)
                            {
                                m_objMarkSearchROI.LoadROISetting(
                                                    (int)Math.Round(m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_pRectCenterPoint.X -
                                                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                    (int)Math.Round(m_smVisionInfo.g_arrOrientGaugeM4L[0].ref_pRectCenterPoint.Y -
                                                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth,
                                                    m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight);
                            }
                        }

                        if (m_smVisionInfo.g_blnWantGauge)
                        {
                            RectGaugeM4L objGauge;
                            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                            {
                                objGauge = m_smVisionInfo.g_arrOrientGaugeM4L[0];
                            }
                            else
                            {
                                objGauge = m_smVisionInfo.g_arrMarkGaugeM4L[0];
                                // Measure search roi to get unit center point
                                objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrRotatedImages, m_objMarkSearchROI, m_smVisionInfo.g_objWhiteImage);
                            }

                            // Get mark gauge measure size
                            int intGaugeWidth = (int)(objGauge.ref_fRectWidth);
                            int intGaugeHeight = (int)(objGauge.ref_fRectHeight);

                            int intGaugeCenterX = (int)(objGauge.ref_pRectCenterPoint.X);
                            int intGaugeCenterY = (int)(objGauge.ref_pRectCenterPoint.Y);
                            
                            // Get Mark ROI size
                            int intROIWidth = m_objMarkTrainROI.ref_ROIWidth;
                            int intROIHeight = m_objMarkTrainROI.ref_ROIHeight;

                            if (intROIWidth >= intGaugeWidth)
                                intROIWidth -= (intROIWidth - intGaugeWidth) / 2;
                            if (intROIHeight >= intGaugeHeight)
                                intROIHeight -= (intROIHeight - intGaugeHeight) / 2;

                            //int intROIOriX = (int)(fXAfterRotated - intROIWidth / 2) - m_objMarkSearchROI.ref_ROIPositionX;
                            //int intROIOriY = (int)(fYAfterRotated - intROIHeight / 2) - m_objMarkSearchROI.ref_ROIPositionY;

                            int intROIOriX = (int)(objGauge.ref_pRectCenterPoint.X - intROIWidth / 2) - m_objMarkSearchROI.ref_ROIPositionX;
                            int intROIOriY = (int)(objGauge.ref_pRectCenterPoint.Y - intROIHeight / 2) - m_objMarkSearchROI.ref_ROIPositionY;


                            // Set train roi start point
                            if ((intROIOriX > 0) &&
                              ((intROIOriX + m_objMarkTrainROI.ref_ROIWidth) <= m_objMarkSearchROI.ref_ROIWidth) &&
                              (intROIOriY > 0) &&
                              ((intROIOriY + m_objMarkTrainROI.ref_ROIHeight) <= m_objMarkSearchROI.ref_ROIHeight))
                            {
                                m_objMarkTrainROI.ref_ROIPositionX = intROIOriX;
                                m_objMarkTrainROI.ref_ROIPositionY = intROIOriY;
                            }
                            else
                            {
                                m_objMarkTrainROI.ref_ROIPositionX = m_objMarkTrainROI.ref_ROIOriPositionX;
                                m_objMarkTrainROI.ref_ROIPositionY = m_objMarkTrainROI.ref_ROIOriPositionY;
                            }
                        }
                        else //No gauge
                        {
                            if (m_blnGaugeResult)
                            {
                                if (m_smVisionInfo.g_fOrientCenterX[0] != -1 && m_smVisionInfo.g_fOrientCenterY[0] != -1)    // Orient test able to get position
                                {
                                    float CenterX = (float)(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionX);
                                    float CenterY = (float)(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionY);

                                    float Angle = m_smVisionInfo.g_fOrientAngle[0] + (m_intOrientAngle); // this formula is based on clockwise rotation so the angle need to be inverted, 
                                    Angle = -Angle;
                                    float fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_fOrientCenterX[0] - CenterX) * Math.Cos(Angle * Math.PI / 180)) - ((m_smVisionInfo.g_fOrientCenterY[0] - CenterY) * Math.Sin(Angle * Math.PI / 180)));

                                    float fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_fOrientCenterX[0] - CenterX) * Math.Sin(Angle * Math.PI / 180)) + ((m_smVisionInfo.g_fOrientCenterY[0] - CenterY) * Math.Cos(Angle * Math.PI / 180)));
                                    
                                    m_objMarkTrainROI.LoadROISetting((int)fXAfterRotated +                  // Orient Result Object Center X
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROICenterX -      // Mark ROI Center X
                                                                    m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROICenterX -    // Orient ROI Center X
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth / 2,     // Half of Mark ROI Size X
                                                                    (int)fYAfterRotated +
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROICenterY -
                                                                    m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROICenterY -
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight / 2,
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
                                }
                                else
                                {
                                    m_objMarkTrainROI.LoadROISetting(m_objMarkSearchROI.ref_ROIWidth / 2 -    // Orient ROI Center X
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth / 2,     // Half of Mark ROI Size X
                                                                    m_objMarkSearchROI.ref_ROIHeight / 2 -
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight / 2,
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
                                }
                                
                                int intROITolerance_Top = 0, intROITolerance_Right = 0, intROITolerance_Bottom = 0, intROITolerance_Left = 0;
                                if (m_smVisionInfo.g_blnWhiteOnBlack || !m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateBrightDarkROITolerance)
                                {
                                    intROITolerance_Top = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge;
                                    intROITolerance_Right = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight;
                                    intROITolerance_Bottom = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom;
                                    intROITolerance_Left = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft;
                                }
                                else
                                {
                                    intROITolerance_Top = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge_Dark;
                                    intROITolerance_Right = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight_Dark;
                                    intROITolerance_Bottom = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom_Dark;
                                    intROITolerance_Left = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft_Dark;
                                }

                                float fEdgeLimit = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2 + intROITolerance_Left;
                                if (m_objMarkTrainROI.ref_ROITotalX < fEdgeLimit)
                                {
                                    m_objMarkTrainROI.LoadROISetting(m_objMarkTrainROI.ref_ROIPositionX + (int)Math.Round(fEdgeLimit - m_objMarkTrainROI.ref_ROITotalX, 0, MidpointRounding.AwayFromZero),
                                                                     m_objMarkTrainROI.ref_ROIPositionY,
                                                                     m_objMarkTrainROI.ref_ROIWidth - (int)Math.Round(fEdgeLimit - m_objMarkTrainROI.ref_ROITotalX, 0, MidpointRounding.AwayFromZero),
                                                                     m_objMarkTrainROI.ref_ROIHeight);
                                }

                                fEdgeLimit = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2 + intROITolerance_Top;
                                if (m_objMarkTrainROI.ref_ROITotalY < fEdgeLimit)
                                {
                                    m_objMarkTrainROI.LoadROISetting(m_objMarkTrainROI.ref_ROIPositionX,
                                                                     m_objMarkTrainROI.ref_ROIPositionY + (int)Math.Round(fEdgeLimit - m_objMarkTrainROI.ref_ROITotalY, 0, MidpointRounding.AwayFromZero),
                                                                     m_objMarkTrainROI.ref_ROIWidth,
                                                                     m_objMarkTrainROI.ref_ROIHeight - ((int)fEdgeLimit - m_objMarkTrainROI.ref_ROITotalY));
                                }

                                fEdgeLimit = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2 - intROITolerance_Bottom;
                                if (m_objMarkTrainROI.ref_ROITotalY + m_objMarkTrainROI.ref_ROIHeight > fEdgeLimit)
                                {
                                    m_objMarkTrainROI.LoadROISetting(m_objMarkTrainROI.ref_ROIPositionX, m_objMarkTrainROI.ref_ROIPositionY,
                                                                     m_objMarkTrainROI.ref_ROIWidth,
                                                                     m_objMarkTrainROI.ref_ROIHeight - (int)Math.Round(m_objMarkTrainROI.ref_ROITotalY + m_objMarkTrainROI.ref_ROIHeight - fEdgeLimit, 0, MidpointRounding.AwayFromZero));
                                }

                                fEdgeLimit = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2 - intROITolerance_Right;
                                if (m_objMarkTrainROI.ref_ROITotalX + m_objMarkTrainROI.ref_ROIWidth > fEdgeLimit)
                                {
                                    m_objMarkTrainROI.LoadROISetting(m_objMarkTrainROI.ref_ROIPositionX,
                                                                     m_objMarkTrainROI.ref_ROIPositionY,
                                                                     m_objMarkTrainROI.ref_ROIWidth - (int)Math.Round(m_objMarkTrainROI.ref_ROITotalX + m_objMarkTrainROI.ref_ROIWidth - fEdgeLimit, 0, MidpointRounding.AwayFromZero),
                                                                     m_objMarkTrainROI.ref_ROIHeight);
                                }

                            }
                           
                            else
                            {
                                if (m_smVisionInfo.g_fOrientCenterX[0] != -1 && m_smVisionInfo.g_fOrientCenterY[0] != -1)    // Orient test able to get position
                                {

                                    // 2019 08 16 ZJYEOH : To load the Mark ROI without gauge, the center point cannot use directly from m_smVisionInfo.g_fOrientCenterX[0] as this point is before rotated
                                    float CenterX = (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionX);
                                    float CenterY = (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionY);

                                    float Angle = m_smVisionInfo.g_fOrientAngle[0] + (m_intOrientAngle); // this formula is based on clockwise rotation so the angle need to be inverted, 
                                    Angle = -Angle;
                                    float fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_fOrientCenterX[0] - CenterX) * Math.Cos(Angle * Math.PI / 180)) - ((m_smVisionInfo.g_fOrientCenterY[0] - CenterY) * Math.Sin(Angle * Math.PI / 180)));

                                    float fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_fOrientCenterX[0] - CenterX) * Math.Sin(Angle * Math.PI / 180)) + ((m_smVisionInfo.g_fOrientCenterY[0] - CenterY) * Math.Cos(Angle * Math.PI / 180)));
                                    
                                    m_objMarkTrainROI.LoadROISetting((int)fXAfterRotated +                  // Orient Result Object Center X
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROICenterX -      // Mark ROI Center X
                                                                    m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROICenterX -    // Orient ROI Center X
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth / 2,     // Half of Mark ROI Size X
                                                                    (int)fYAfterRotated +
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROICenterY -
                                                                    m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROICenterY -
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight / 2,
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
                                }
                                else
                                {
                                    m_objMarkTrainROI.LoadROISetting(m_objMarkSearchROI.ref_ROIWidth / 2 -    // Orient ROI Center X
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth / 2,     // Half of Mark ROI Size X
                                                                    m_objMarkSearchROI.ref_ROIHeight / 2 -
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight / 2,
                                                                    m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
                                }
                            }
                        }
                    }
                    else
                    {
                        m_objMarkSearchROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);

                        if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        {
                            // Find unit ROI (Train ROI)
                            m_objMarkTrainROI.ref_ROIPositionX = (int)Math.Round(m_smVisionInfo.g_fUnitCenterX[0] - (float)m_objMarkTrainROI.ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);   // 2018 10 09 - CCENG: change g_fOrientCenterXY to g_fUnitCenterXY because g_fOrientCenter is used to keep Orient matcher result center point, not orient gauge unit center point.
                            m_objMarkTrainROI.ref_ROIPositionY = (int)Math.Round(m_smVisionInfo.g_fUnitCenterY[0] - (float)m_objMarkTrainROI.ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                    
                    {
                        PointF pPackageCenterPoint = new PointF(-1, -1);
                        SizeF SPackageSize = new SizeF(-1, -1);
                        if (m_blnGaugeResult)
                        {
                            pPackageCenterPoint = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint;
                            SPackageSize = new SizeF(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight);
                        }
                        
                            ImageDrawing objRotatedPackageImage = m_smVisionInfo.g_arrRotatedImages[0];
                            m_smVisionInfo.g_arrMarks[0].InspectOCVOnly(
                                m_objMarkOcvSearchROI,
                                 m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth,
                                m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight,
                                m_objMarkTrainROI,
                                pPackageCenterPoint,
                                SPackageSize,
                                m_smVisionInfo.g_intMarkDefectInspectionMethod,
                                m_smVisionInfo.g_intMarkInspectionAreaGrayValueSensitivity,
                                m_smVisionInfo.g_intMarkBrightSensitivity
                              );
                        
                            m_smVisionInfo.g_blnMarkInspected = true;
                            m_smVisionInfo.g_blnDrawMarkResult = false;

                            if (m_smVisionInfo.g_arrMarks[0].ref_intFailResultMask > 0)
                            {
                                m_smVisionInfo.g_strErrorMessage += m_smVisionInfo.g_arrMarks[0].GetInspectionMessage(-1, false, 0);

                                return false;

                            }

                    }
                }
                
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void RotateImage()
        {
            if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
            {
                for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                {
                    ROI objRotateROI = new ROI();
                    objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);
                    objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                   (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                   m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                   m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[intImageIndex], objRotateROI,
                                           m_intOrientAngle + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle,
                                              ref m_smVisionInfo.g_arrRotatedImages, intImageIndex);
                    
                }
            }
            else
            {
                for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
                {
                    m_smVisionInfo.g_arrImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
                }
            }

            if (m_smVisionInfo.g_blnViewColorImage)
            {
                if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle != 0)
                {
                    for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                    {
                        CROI objRotateROI = new CROI();
                        objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[intImageIndex]);
                        objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                                       (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - (float)m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                                       m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth,
                                                       m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);

                        CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[intImageIndex], objRotateROI,
                                                 m_intOrientAngle + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle, 8,
                                                  ref m_smVisionInfo.g_arrColorRotatedImages, intImageIndex);
                        
                    }
                }
                else
                {
                    for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrColorImages.Count; intImageIndex++)
                    {
                        m_smVisionInfo.g_arrColorImages[intImageIndex].CopyTo(m_smVisionInfo.g_arrColorRotatedImages[intImageIndex]);
                    }
                }
            }
        }

        private void CollectOCVData()
        {
            int intCharDilateHalfWidth = m_smVisionInfo.g_arrMarks[0].ref_intCharDilateHalfWidth; // thick iteration
            
            {
                // ---------- Fill Mark Image's mark area with black color -------------------
                int intNumChars = m_smVisionInfo.g_arrMarks[0].GetNumChars();
                int intStartX = 0, intStartY = 0, intEndX = 0, intEndY = 0;

                int intOffsetX;
                int intOffsetY;

                if (m_smVisionInfo.g_blnWantGauge)  // Mean during mark inspection, orient gauge is used to measure unit position for m_objMarkTrainROI
                {
                    intOffsetX = m_objMarkTrainROI.ref_ROITotalX;
                    intOffsetY = m_objMarkTrainROI.ref_ROITotalY;
                }
                else if (m_smVisionInfo.g_arrMarkROIs[0].Count > 3)    // Mean during mark inspection, Unit PR is used to find unit surface position for m_objMarkTrainROI
                {
                    intOffsetX = m_objMarkTrainROI.ref_ROITotalX;
                    intOffsetY = m_objMarkTrainROI.ref_ROITotalY;
                }
                else // Mean during mark inspection, m_objMarkSearchROI is used.
                {
                    intOffsetX = m_objMarkTrainROI.ref_ROITotalX;
                    intOffsetY = m_objMarkTrainROI.ref_ROITotalY;
                }

                for (int i = 0; i < intNumChars; i++)
                {
                    bool blnIsBarPin1 = m_smVisionInfo.g_arrMarks[0].GetCharIsBarPin1(m_smVisionInfo.g_arrMarks[0].ref_intGroupIndex, m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex, i);
                    int intBarPin1Type = m_smVisionInfo.g_arrMarks[0].GetCharBarPin1Type(m_smVisionInfo.g_arrMarks[0].ref_intGroupIndex, m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex, i);

                    int intRectStartX, intRectStartY, intRectEndX, intRectEndY;
                    intRectStartX = intRectStartY = intRectEndX = intRectEndY = 0;
                    m_smVisionInfo.g_arrMarks[0].GetCharStartXY(i, ref intStartX, ref intStartY);
                    m_smVisionInfo.g_arrMarks[0].GetCharEndXY(i, ref intEndX, ref intEndY);

                    if (m_smVisionInfo.g_arrMarks[0].ref_blnWantDontCareIgnoredMarkWholeArea && !m_smVisionInfo.g_arrMarks[0].GetEnableMarkSetting(i))
                    {
                        //2021-01-18 ZJYEOH : Should use sample position
                        Point pStartTemplate = new Point(intStartX, intStartY);//m_smVisionInfo.g_arrMarks[0].GetTemplateCharROIStartPoint(i);
                        Point pEndTemplate = new Point(intEndX, intEndY);//m_smVisionInfo.g_arrMarks[0].GetTemplateCharROIEndPoint(i);

                        if ((intOffsetX + pStartTemplate.X - m_smVisionInfo.g_arrMarks[0].GetTemplateCharShiftX(i)) >= 0)
                            intRectStartX = intOffsetX + pStartTemplate.X - m_smVisionInfo.g_arrMarks[0].GetTemplateCharShiftX(i);
                        else
                            intRectStartX = intOffsetX;

                        if ((intOffsetY + pStartTemplate.Y - m_smVisionInfo.g_arrMarks[0].GetTemplateCharShiftY(i)) >= 0)
                            intRectStartY = intOffsetY + pStartTemplate.Y - m_smVisionInfo.g_arrMarks[0].GetTemplateCharShiftY(i);
                        else
                            intRectStartY = intOffsetY;

                        if ((intOffsetX + pEndTemplate.X + m_smVisionInfo.g_arrMarks[0].GetTemplateCharShiftX(i)) < m_smVisionInfo.g_intCameraResolutionWidth)
                            intRectEndX = intOffsetX + pEndTemplate.X + m_smVisionInfo.g_arrMarks[0].GetTemplateCharShiftX(i);
                        else
                            continue;
                        if ((intOffsetY + pEndTemplate.Y + m_smVisionInfo.g_arrMarks[0].GetTemplateCharShiftY(i)) < m_smVisionInfo.g_intCameraResolutionHeight)
                            intRectEndY = intOffsetY + pEndTemplate.Y + m_smVisionInfo.g_arrMarks[0].GetTemplateCharShiftY(i);
                        else
                            continue;
                    }
                    else
                    {
                        // 2019 10 11 - CCENG   : add intOffsetXY checking because rectangle will be filled on m_objPkgViewImage (Full image)
                        //                      : without the intOffsetXY, intRectStartX will set to intOffsetX and fill position may be shifted                       
                        if ((intOffsetX + intStartX - intCharDilateHalfWidth) >= 0)  //if ((intStartX - intCharDilateHalfWidth) >= 0)   
                            intRectStartX = intOffsetX + intStartX - intCharDilateHalfWidth;
                        else
                            intRectStartX = intOffsetX;

                        // 2019 10 11 - CCENG   : add intOffsetXY checking because rectangle will be filled on m_objPkgViewImage (Full image)
                        //                      : without the intOffsetXY, intRectStartX will set to intOffsetX and fill position may be shifted
                        if ((intOffsetY + intStartY - intCharDilateHalfWidth) >= 0)    //if ((intStartY - intCharDilateHalfWidth) >= 0) 
                            intRectStartY = intOffsetY + intStartY - intCharDilateHalfWidth;
                        else
                            intRectStartY = intOffsetY;

                        if ((intEndX + intCharDilateHalfWidth) < m_smVisionInfo.g_intCameraResolutionWidth)
                            intRectEndX = intOffsetX + intEndX + intCharDilateHalfWidth;
                        else
                            intRectEndX = intOffsetX + intEndX;

                        if ((intEndY + intCharDilateHalfWidth) < m_smVisionInfo.g_intCameraResolutionHeight)
                            intRectEndY = intOffsetY + intEndY + intCharDilateHalfWidth;
                        else
                            intRectEndY = intOffsetY + intEndY;
                    }

                    //2020-05-28 ZJYEOH : extend the dont care area to fit the m_objMarkTrainROI size
                    if (blnIsBarPin1 && !m_smVisionInfo.g_blnWantCheckBarPin1)
                    {
                        if (intBarPin1Type == 0) // Virtical bar
                        {
                            intRectStartY = intOffsetY;
                            intRectEndY = intOffsetY + m_objMarkTrainROI.ref_ROIHeight;
                        }
                        else if (intBarPin1Type == 1) // Horizontal bar
                        {
                            intRectStartX = intOffsetX;
                            intRectEndX = intOffsetX + m_objMarkTrainROI.ref_ROIWidth;
                        }
                    }

                    arrStartX.Add(intRectStartX);
                    arrStartY.Add(intRectStartY);
                    arrEndX.Add(intRectEndX);
                    arrEndY.Add(intRectEndY);

                }
            }
        }
        private void AddColorROI(List<List<CROI>> arrROIs)
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                if (i >= arrROIs.Count)
                    arrROIs.Add(new List<CROI>());

                float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_pRectCenterPoint.X;
                float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_pRectCenterPoint.Y;
                float fWidth = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_fRectWidth;
                float fHeight = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_fRectHeight;
                int intPositionX = (int)Math.Round(fCenterX - (fWidth / 2), 0, MidpointRounding.AwayFromZero);
                int intPositionY = (int)Math.Round(fCenterY - (fHeight / 2), 0, MidpointRounding.AwayFromZero);

                CROI objROI = new CROI("Color ROI", 1);

                objROI.AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                objROI.LoadROISetting(intPositionX, intPositionY,
                    (int)Math.Round(fWidth, 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(fHeight, 0, MidpointRounding.AwayFromZero));

                if (arrROIs[i].Count == 0)
                    arrROIs[i].Add(objROI);
                else
                    arrROIs[i][0] = objROI;
            }
        }
    }
}
