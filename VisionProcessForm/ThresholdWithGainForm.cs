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

namespace VisionProcessForm
{
    public partial class ThresholdWithGainForm : Form
    {
        #region Member Variables
        private List<int> m_arrSideThrehsold = new List<int>();
        private bool m_blnInitDone = false;
        private bool m_blnUpdateHistogram = false;
        private bool m_blnWantAutoThreshold = true;
        private bool m_blnWantSetToAllROICheckBox = false;
        private string m_strThresholdDefinition = "";
        private Graphics m_Graphic;
        private Graphics m_Graphic2;
        private ROI m_objROI;
        private VisionInfo m_smVisionInfo;

        private int m_intCountAreaColorIndex = 0;
        private int m_intCountAreaPixelLimit = 0;
        private ImageDrawing m_Image = new ImageDrawing();

        #endregion

        #region Properties

        public int ref_intThresholdValue { get { return Convert.ToInt32(txt_ThresholdValue.Text); } }
        public int ref_intCountAreaColorIndex { get { return m_intCountAreaColorIndex; } }
        public int ref_intCountAreaPixelLimit { get { return m_intCountAreaPixelLimit; } }
        public bool ref_blnSetToAllROI { get { return chk_SetToOtherSideROIs.Checked; } }

        #endregion

        public ThresholdWithGainForm(VisionInfo smVisionInfo, ROI objROI, bool blnWantAutoThreshold, bool blnWantSetToAllROICheckBox)
        {
            InitializeComponent();
            m_smVisionInfo = smVisionInfo;
            m_objROI = objROI;
            m_blnWantAutoThreshold = blnWantAutoThreshold;
            m_blnWantSetToAllROICheckBox = blnWantSetToAllROICheckBox;

            InitThresholdForm();
        }
        public ThresholdWithGainForm(VisionInfo smVisionInfo, ROI objROI, bool blnWantAutoThreshold, bool blnWantSetToAllROICheckBox, List<int> arrSideThreshold)
        {
            InitializeComponent();
            m_smVisionInfo = smVisionInfo;
            m_objROI = objROI;
            m_blnWantAutoThreshold = blnWantAutoThreshold;
            m_blnWantSetToAllROICheckBox = blnWantSetToAllROICheckBox;
            m_arrSideThrehsold = arrSideThreshold;
            InitThresholdForm();
        }

        private void InitThresholdForm()
        {
            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);
            m_Graphic2 = Graphics.FromHwnd(pic_ROI.Handle);
            UpdateThresholdGUI();
            txt_Gain.Value = Convert.ToDecimal(m_smVisionInfo.g_fThresholdGainValue);
            m_smVisionInfo.g_blnViewThreshold = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // Count Color area and pixel
            srmLabel3.Visible = false;
            cbo_AreaColor.Visible = false;
            srmLabel1.Visible = false;
            txt_CountAreaPixel.Visible = false;

            chk_SetToOtherSideROIs.Visible = m_blnWantSetToAllROICheckBox;

            m_blnInitDone = true;
        }

        private void UpdateColorTransitionLable()
        {
            switch (cbo_AreaColor.SelectedIndex)
            {
                case 0:
                    lbl_Transition.Text = m_strThresholdDefinition + " when black area lower than " + txt_CountAreaPixel.Text + " pixel.";
                    break;
                case 1:
                    lbl_Transition.Text = m_strThresholdDefinition + " when white area lower than " + txt_CountAreaPixel.Text + " pixel.";
                    break;
                case 2:
                    lbl_Transition.Text = m_strThresholdDefinition + " when black area higher than " + txt_CountAreaPixel.Text + " pixel.";
                    break;
                case 3:
                    lbl_Transition.Text = m_strThresholdDefinition + " when white area higher than " + txt_CountAreaPixel.Text + " pixel.";
                    break;
            }
        }

        /// <summary>
        /// Update GUI for threshold setting
        /// </summary>
        private void UpdateThresholdGUI()
        {
            if (m_blnWantAutoThreshold)
            {
                if (m_smVisionInfo.g_intThresholdValue == -4)
                {
                    chk_MinResidue.Checked = true;
                    trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 3);
                    txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                    //2021-01-07 ZJYEOH : make sure no more -4 value
                    m_smVisionInfo.g_intThresholdValue = trackBar_Threshold.Value;
                }
                else
                {
                    chk_MinResidue.Checked = false;
                    txt_ThresholdValue.Enabled = true;
                    trackBar_Threshold.Enabled = true;
                    trackBar_Threshold.Value = m_smVisionInfo.g_intThresholdValue;
                    txt_ThresholdValue.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                }
            }
            else
            {
                chk_MinResidue.Checked = false;
                chk_MinResidue.Visible = false;

                txt_ThresholdValue.Enabled = true;
                trackBar_Threshold.Enabled = true;

                if (m_smVisionInfo.g_intThresholdValue == -4)
                {
                    trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 3);
                    txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                    //2021-01-07 ZJYEOH : make sure no more -4 value
                    m_smVisionInfo.g_intThresholdValue = trackBar_Threshold.Value;
                }
                else
                {
                    trackBar_Threshold.Value = m_smVisionInfo.g_intThresholdValue;
                    txt_ThresholdValue.Text = m_smVisionInfo.g_intThresholdValue.ToString();
                }
            }

        }
       
        private void chk_MinResidue_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (chk_MinResidue.Checked)
            {
                trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 3);
                txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();

                // 2021-01-07 ZJYEOH : Dont Use -4 anymore
                m_smVisionInfo.g_intThresholdValue = trackBar_Threshold.Value;
                txt_ThresholdValue.Enabled = false;
                trackBar_Threshold.Enabled = false;
            }
            else
            {
                m_smVisionInfo.g_intThresholdValue = trackBar_Threshold.Value;
                txt_ThresholdValue.Enabled = true;
                trackBar_Threshold.Enabled = true;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void pic_Histogram_Paint(object sender, PaintEventArgs e)
        {
            m_blnUpdateHistogram = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToOtherSideROIs.Checked;
            if (m_blnUpdateHistogram)
            {
                m_blnUpdateHistogram = false;
                m_Graphic.Clear(this.BackColor);
                int intThresholdValue;
                if (m_smVisionInfo.g_intThresholdValue == -4)
                    intThresholdValue = ROI.GetAutoThresholdValue(m_objROI, 3);
                else
                    intThresholdValue = m_smVisionInfo.g_intThresholdValue;

                ImageDrawing.DrawHistogram(m_Graphic, m_objROI, intThresholdValue);

                int Max = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
                pic_ROI.Size = new Size(Math.Min((int)Math.Ceiling((250f / Max) * (m_objROI.ref_ROIWidth)), 250), Math.Min((int)Math.Ceiling((250f / Max) * (m_objROI.ref_ROIHeight)), 250));
                //pic_ROI.Size = new Size(250,250);
                m_Image.SetImageSize(m_objROI.ref_ROIWidth, m_objROI.ref_ROIHeight);
                panel1.Height = Math.Min((int)Math.Ceiling((250f / Max) * (m_objROI.ref_ROIHeight)), 250);
                // panel1.Location = new Point(panel1.Location.X, panel4.Location.Y+panel4.Size.Height);
                this.Size = new Size(this.Size.Width, panel2.Location.Y + panel2.Size.Height);
                m_objROI.CopyToImage(ref m_Image);

                m_Image.AddGain(ref m_Image, (float)txt_Gain.Value);
                // m_objROI.SaveImage("D:\\m_Image.bmp");
                m_Image.RedrawImage(m_Graphic2, 250f / Max, 250f / Max);// Math.Min(Max / 250f, 250f / Max), Math.Min(Max / 250f, 250f / Max));
                                                                        //pic_ROI.Size = new Size((int)(250 * 250 / Max), (int)(250 * 250 / Max));
                pic_ROI.Location = new Point(panel1.Size.Width / 2 - pic_ROI.Width / 2, panel1.Size.Height / 2 - pic_ROI.Height / 2);

            }
            int Max2 = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
            m_Image.RedrawImage(m_Graphic2, 250f / Max2, 250f / Max2);
        }

        private void trackBar_Threshold_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
        }

        private void txt_ThresholdValue_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            trackBar_Threshold.Value = Convert.ToInt32(txt_ThresholdValue.Text);
            m_smVisionInfo.g_intThresholdValue = trackBar_Threshold.Value;

            lbl_Threshold.ForeColor = Color.Black;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // Update histogram
            m_blnUpdateHistogram = true;
        }



        private void ThresholdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_blnDrawThresholdAllSideROI = false;
            m_Image.Dispose();
            m_smVisionInfo.g_blnViewThreshold = false;
            m_smVisionInfo.g_blnViewThresholdWithGain = false;
            m_smVisionInfo.g_blnViewNormalImage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CountAreaPixel_TextChanged(object sender, EventArgs e)
        {
            m_intCountAreaPixelLimit = Convert.ToInt32(txt_CountAreaPixel.Text);

            UpdateColorTransitionLable();
        }

        private void cbo_AreaColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_intCountAreaColorIndex = cbo_AreaColor.SelectedIndex;

            UpdateColorTransitionLable();
        }

        private void ThresholdForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewNormalImage = true;
            m_smVisionInfo.g_blnViewThresholdWithGain = true;
            m_Graphic.Clear(this.BackColor);
            int intThresholdValue;
            if (m_smVisionInfo.g_intThresholdValue == -4)
                intThresholdValue = ROI.GetAutoThresholdValue(m_objROI, 3);
            else
                intThresholdValue = m_smVisionInfo.g_intThresholdValue;

            ImageDrawing.DrawHistogram(m_Graphic, m_objROI, intThresholdValue);

            int Max = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
            pic_ROI.Size = new Size(Math.Min((int)Math.Ceiling((250f / Max) * (m_objROI.ref_ROIWidth)), 250), Math.Min((int)Math.Ceiling((250f / Max) * (m_objROI.ref_ROIHeight)), 250));
            m_Image.SetImageSize(m_objROI.ref_ROIWidth, m_objROI.ref_ROIHeight);
            panel1.Height = Math.Min((int)Math.Ceiling((250f / Max) * (m_objROI.ref_ROIHeight)), 250);
            this.Size = new Size(this.Size.Width, panel2.Location.Y + panel2.Size.Height);
            m_objROI.CopyToImage(ref m_Image);
            m_Image.RedrawImage(m_Graphic2, 250f / Max, 250f / Max);// Math.Min(Max / 250f, 250f / Max), Math.Min(Max / 250f, 250f / Max));
                                                                    //pic_ROI.Size = new Size((int)(250 * 250 / Max), (int)(250 * 250 / Max));
            pic_ROI.Location = new Point(panel1.Size.Width / 2 - pic_ROI.Width / 2, panel1.Size.Height / 2 - pic_ROI.Height / 2);


            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            this.Location = new Point(resolution.Width - this.Size.Width, resolution.Height - this.Size.Height);
        }

        private void txt_Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_fThresholdGainValue = (float)txt_Gain.Value;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // Update histogram
            m_blnUpdateHistogram = true;
        }

        private void chk_SetToOtherSideROIs_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToOtherSideROIs.Checked;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (chk_SetToOtherSideROIs.Checked)
            {
                int intValue = Convert.ToInt32(txt_ThresholdValue.Text);//m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intThresholdValue;

                bool blnSameSetting = true;

                for (int i = 1; i < m_arrSideThrehsold.Count; i++)
                {
                    if (i >= m_arrSideThrehsold.Count)
                        break;

                    if (i == m_smVisionInfo.g_intSelectedROI)
                        continue;

                    if (m_arrSideThrehsold[i] != intValue)
                    {
                        blnSameSetting = false;
                        break;
                    }
                }

                if (!blnSameSetting)
                {
                    lbl_Threshold.ForeColor = Color.Red;
                    //txt_ThresholdValue.ForeColor = Color.Red;
                }
                else
                {
                    lbl_Threshold.ForeColor = Color.Black;
                    //txt_ThresholdValue.ForeColor = Color.Black;
                }
            }
            else
            {
                lbl_Threshold.ForeColor = Color.Black;
                //txt_ThresholdValue.ForeColor = Color.Black;
            }
        }
    }
}
