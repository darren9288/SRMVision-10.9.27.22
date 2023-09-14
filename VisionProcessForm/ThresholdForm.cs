using System;
using System.Collections;
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
    public partial class ThresholdForm : Form
    {
        #region Member Variables
        private List<int> m_arrSideThrehsold = new List<int>();
        private bool m_blnWantRelative = false;
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
        public bool ref_blnWantAutoThreshold { get { return chk_MinResidue.Checked; } }
        public int ref_intThresholdValue { get { return Convert.ToInt32(txt_ThresholdValue.Text); } }
        public int ref_intCountAreaColorIndex { get { return m_intCountAreaColorIndex; }}
        public int ref_intCountAreaPixelLimit { get { return m_intCountAreaPixelLimit; } }
        public bool ref_blnSetToAllROI { get { return chk_SetToOtherSideROIs.Checked; } }
        public bool ref_blnSetToAllTemplate { get { return chk_SetToAllTemplate.Checked; } }

        #endregion

        public ThresholdForm(bool blnSetToAllTemplate, VisionInfo smVisionInfo, ROI objROI)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_objROI = objROI;
            chk_SetToAllTemplate.Visible = blnSetToAllTemplate;
            InitThresholdForm();
        }

        public ThresholdForm(VisionInfo smVisionInfo, ROI objROI)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_objROI = objROI;

            InitThresholdForm();
        }
        public ThresholdForm(VisionInfo smVisionInfo, bool blnWantRelative, bool blnWantAutoThreshold, ROI objROI)
        {
            InitializeComponent();
            m_blnWantRelative = blnWantRelative;
         
            m_smVisionInfo = smVisionInfo;
            if (blnWantAutoThreshold)
                m_smVisionInfo.g_intThresholdValue = -4;
            m_objROI = objROI;

            InitThresholdForm();
        }
        public ThresholdForm(VisionInfo smVisionInfo, ROI objROI, bool blnWantAutoThreshold)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_objROI = objROI;
            m_blnWantAutoThreshold = blnWantAutoThreshold;

            InitThresholdForm();
        }

        public ThresholdForm(VisionInfo smVisionInfo, ROI objROI, bool blnWantAutoThreshold, bool blnWantSetToAllROICheckBox, List<int> arrSideThreshold)
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

        public ThresholdForm(VisionInfo smVisionInfo, int intCountAreaColorIndex, ROI objROI)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_objROI = objROI;

            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);
            m_Graphic2 = Graphics.FromHwnd(pic_ROI.Handle);
            UpdateThresholdGUI();

            m_smVisionInfo.g_blnViewThreshold = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // Count Color area and pixel
            cbo_AreaColor.SelectedIndex = intCountAreaColorIndex;
            srmLabel1.Visible = false;
            txt_CountAreaPixel.Visible = false;

            m_blnInitDone = true;
        }

        public ThresholdForm(VisionInfo smVisionInfo, ROI objROI, int intThresholdValue)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_objROI = objROI;

            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);
            m_Graphic2 = Graphics.FromHwnd(pic_ROI.Handle);
            UpdateThresholdGUI(intThresholdValue);

            m_smVisionInfo.g_blnViewThreshold = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            chk_SetToOtherSideROIs.Visible = false;
           // Count Color area and pixel
           srmLabel3.Visible = false;
            cbo_AreaColor.Visible = false;
            srmLabel1.Visible = false;
            txt_CountAreaPixel.Visible = false;

            m_blnInitDone = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smVisionInfo"></param>
        /// <param name="objROI"></param>
        /// <param name="intCountAreaColorIndex">0=Black is Empty, 1=White is Empty</param>
        /// <param name="intCountAreaPixelLimit"></param>
        public ThresholdForm(VisionInfo smVisionInfo, ROI objROI, int intCountAreaColorIndex, int intCountAreaPixelLimit)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_objROI = objROI;
            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);
            m_Graphic2 = Graphics.FromHwnd(pic_ROI.Handle);
            UpdateThresholdGUI();

            m_smVisionInfo.g_blnViewThreshold = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            cbo_AreaColor.SelectedIndex = intCountAreaColorIndex;
            txt_CountAreaPixel.Text = intCountAreaPixelLimit.ToString();

            m_intCountAreaColorIndex = intCountAreaColorIndex;
            m_intCountAreaPixelLimit = intCountAreaPixelLimit;
            m_blnInitDone = true;
        }

        public ThresholdForm(VisionInfo smVisionInfo, ROI objROI, int intColorTransition, int intCountAreaPixelLimit, 
            int intCurrentBlackAreaPixel, int intCurrentWhiteAreaPixel, string strThresholdDefinition)
        {
            InitializeComponent();

            m_smVisionInfo = smVisionInfo;
            m_objROI = objROI;
            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);
            m_Graphic2 = Graphics.FromHwnd(pic_ROI.Handle);
            m_strThresholdDefinition = strThresholdDefinition;

            UpdateThresholdGUI();

            m_smVisionInfo.g_blnViewThreshold = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            cbo_AreaColor.Items.Clear();
            cbo_AreaColor.Items.Add("Black <");
            cbo_AreaColor.Items.Add("White <");
            cbo_AreaColor.Items.Add("Black >");
            cbo_AreaColor.Items.Add("White >");
            cbo_AreaColor.SelectedIndex = intColorTransition;
            txt_CountAreaPixel.Text = intCountAreaPixelLimit.ToString();
            lbl_Transition.Visible = true;
            UpdateColorTransitionLable();
            lbl_PixelDetail.Visible = true;
            lbl_PixelDetail.Text = "Black Pixel = " + intCurrentBlackAreaPixel.ToString() + ", White Pixel = " + intCurrentWhiteAreaPixel.ToString();

            m_intCountAreaColorIndex = intColorTransition;
            m_intCountAreaPixelLimit = intCountAreaPixelLimit;
            m_blnInitDone = true;
        }

        private void UpdateColorTransitionLable()
        {
            switch (cbo_AreaColor.SelectedIndex)
            {
                case 0: lbl_Transition.Text = m_strThresholdDefinition +  " when black area lower than " + txt_CountAreaPixel.Text + " pixel.";
                    break;
                case 1: lbl_Transition.Text = m_strThresholdDefinition + " when white area lower than " + txt_CountAreaPixel.Text + " pixel.";
                    break;
                case 2: lbl_Transition.Text = m_strThresholdDefinition + " when black area higher than " + txt_CountAreaPixel.Text + " pixel.";
                    break;
                case 3: lbl_Transition.Text = m_strThresholdDefinition + " when white area higher than " + txt_CountAreaPixel.Text + " pixel.";
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

                    if (m_blnWantRelative)
                    {
                        pnl_Relative.Visible = true;
                        if (m_smVisionInfo.g_fThresholdRelativeValue < 0)
                        {
                            txt_Relative.Value = -1;
                            trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 3);
                            txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                        }
                        else if (m_smVisionInfo.g_fThresholdRelativeValue == 1)
                        {
                            txt_Relative.Value = 101;
                            trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 2);
                            txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                        }
                        else
                        {
                            txt_Relative.Value = Convert.ToInt32(m_smVisionInfo.g_fThresholdRelativeValue * 100);
                            trackBar_Threshold.Value = ROI.GetRelativeThresholdValue(m_objROI, m_smVisionInfo.g_fThresholdRelativeValue);
                            txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                        }
                    }
                    else
                    {
                        trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 3);
                        txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                    }
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
        /// <summary>
        /// Update GUI for threshold setting
        /// </summary>
        /// <param name="intThresholdValue">Threshold value</param>
        private void UpdateThresholdGUI(int intThresholdValue)
        {
            if (intThresholdValue == -4)
            {
                chk_MinResidue.Checked = true;
                m_smVisionInfo.g_intThresholdValue = intThresholdValue;
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
                trackBar_Threshold.Value = m_smVisionInfo.g_intThresholdValue = intThresholdValue;
                txt_ThresholdValue.Text = m_smVisionInfo.g_intThresholdValue.ToString();
            }
        }
       

        private void chk_MinResidue_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (chk_MinResidue.Checked)
            {
                if (m_blnWantRelative)
                {
                    pnl_Relative.Visible = true;
                    if (m_smVisionInfo.g_fThresholdRelativeValue < 0)
                    {
                        txt_Relative.Value = -1;
                        trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 3);
                        txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                    }
                    else if (m_smVisionInfo.g_fThresholdRelativeValue == 1)
                    {
                        txt_Relative.Value = 101;
                        trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 2);
                        txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                    }
                    else
                    {
                        txt_Relative.Value = Convert.ToInt32(m_smVisionInfo.g_fThresholdRelativeValue * 100);
                        trackBar_Threshold.Value = ROI.GetRelativeThresholdValue(m_objROI, m_smVisionInfo.g_fThresholdRelativeValue);
                        txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                    }
                }
                else
                {
                    trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 3);
                    txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
                    // 2021-01-07 ZJYEOH : Dont Use -4 anymore
                    m_smVisionInfo.g_intThresholdValue = trackBar_Threshold.Value;

                    //m_smVisionInfo.g_intThresholdValue = -4;
                }
                txt_ThresholdValue.Enabled = false;
                    trackBar_Threshold.Enabled = false;
                
            }
            else
            {
                pnl_Relative.Visible = false;
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
                if (m_blnWantRelative)
                {
                    if (m_smVisionInfo.g_fThresholdRelativeValue < 0)
                    {
                        intThresholdValue = ROI.GetAutoThresholdValue(m_objROI, 3);
                    }
                    else if (m_smVisionInfo.g_fThresholdRelativeValue == 1)
                    {
                        intThresholdValue = ROI.GetAutoThresholdValue(m_objROI, 2);
                    }
                    else
                    {
                        intThresholdValue = ROI.GetRelativeThresholdValue(m_objROI, m_smVisionInfo.g_fThresholdRelativeValue);
                    }
                }
                else
                {
                    if (m_smVisionInfo.g_intThresholdValue == -4)
                        intThresholdValue = ROI.GetAutoThresholdValue(m_objROI, 3);
                    else
                        intThresholdValue = m_smVisionInfo.g_intThresholdValue;
                }
                ImageDrawing.DrawHistogram(m_Graphic, m_objROI, intThresholdValue);

                int Max = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
                pic_ROI.Size = new Size(Math.Min((int)Math.Ceiling((250f / Max) * (m_objROI.ref_ROIWidth)), 250), Math.Min((int)Math.Ceiling((250f / Max) * (m_objROI.ref_ROIHeight)), 250));
                //pic_ROI.Size = new Size(250,250);
                m_Image.SetImageSize(m_objROI.ref_ROIWidth, m_objROI.ref_ROIHeight);
                panel1.Height = Math.Min((int)Math.Ceiling((250f / Max) * (m_objROI.ref_ROIHeight)), 250);
                // panel1.Location = new Point(panel1.Location.X, panel4.Location.Y+panel4.Size.Height);
                this.Size = new Size(this.Size.Width, panel2.Location.Y + panel2.Size.Height);
                m_objROI.CopyToImage(ref m_Image);
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

            m_Graphic.Clear(this.BackColor);
            int intThresholdValue;
            if (m_blnWantRelative)
            {
                if (m_smVisionInfo.g_fThresholdRelativeValue < 0)
                {
                    intThresholdValue = ROI.GetAutoThresholdValue(m_objROI, 3);
                }
                else if (m_smVisionInfo.g_fThresholdRelativeValue == 1)
                {
                    intThresholdValue = ROI.GetAutoThresholdValue(m_objROI, 2);
                }
                else
                {
                    intThresholdValue = ROI.GetRelativeThresholdValue(m_objROI, m_smVisionInfo.g_fThresholdRelativeValue);
                }
            }
            else
            {
                if (m_smVisionInfo.g_intThresholdValue == -4)
                    intThresholdValue = ROI.GetAutoThresholdValue(m_objROI, 3);
                else
                    intThresholdValue = m_smVisionInfo.g_intThresholdValue;
            }
            
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

                    if (i == m_smVisionInfo.g_intSelectedROI || m_arrSideThrehsold[i] == -999)
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

        private void txt_Relative_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_fThresholdRelativeValue = Convert.ToInt32(txt_Relative.Value) / 100f;

            if (m_smVisionInfo.g_fThresholdRelativeValue < 0)
            {
                trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 3);
                txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
            }
            else if (m_smVisionInfo.g_fThresholdRelativeValue == 1)
            {
                trackBar_Threshold.Value = ROI.GetAutoThresholdValue(m_objROI, 2);
                txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
            }
            else
            {
                trackBar_Threshold.Value = ROI.GetRelativeThresholdValue(m_objROI, m_smVisionInfo.g_fThresholdRelativeValue);
                txt_ThresholdValue.Text = trackBar_Threshold.Value.ToString();
            }
          
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // Update histogram
            m_blnUpdateHistogram = true;
        }
    }
}