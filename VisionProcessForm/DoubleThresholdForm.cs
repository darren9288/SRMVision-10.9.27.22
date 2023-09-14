using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using SharedMemory;
using VisionProcessing;

namespace VisionProcessForm
{
    public partial class DoubleThresholdForm : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;
        private bool m_blnWantGain = false;
        private bool m_blnWantSetToAllROICheckBox = false;
        private bool m_blnSplitter1Clicked = false;
        private bool m_blnSplitter2Clicked = false;
        private bool m_blnUpdateHistogram = false;
        private int m_intMouseStartPositionX = 0;
        private int m_intClass = 0x02; // 0x01=Low Threshold Pixel Color with White, 0x02=Middle Threshold Pixel Color with White, 0x04=High Threshold Pixel Color with White
        private int[] m_intColumnWidth = new int[3];
        private Graphics m_Graphic;
        private ROI m_objROI;
        private VisionInfo m_smVisionInfo;
        private Graphics m_Graphic2;
        private ImageDrawing m_Image = new ImageDrawing();
        #endregion

        #region Properties

        public bool ref_blnSetToAllROI { get { return chk_SetToOtherSideROIs.Checked; } }

        #endregion

        public DoubleThresholdForm(VisionInfo smVisionInfo, ROI objROI)
        {
          
            m_objROI = objROI;
            m_smVisionInfo = smVisionInfo;

            InitializeComponent();

            InitDoubleThresholdForm();
        }

        public DoubleThresholdForm(VisionInfo smVisionInfo, ROI objROI, bool blnWantGain, bool blnWantSetToAllROICheckBox)
        {
            m_objROI = objROI;
            m_smVisionInfo = smVisionInfo;
            m_blnWantGain = blnWantGain;
            m_blnWantSetToAllROICheckBox = blnWantSetToAllROICheckBox;

            InitializeComponent();

            InitDoubleThresholdForm();
        }

        public DoubleThresholdForm(VisionInfo smVisionInfo, ROI objROI, int intClass, bool blnWantSetToAllROICheckBox)
        {
            m_objROI = objROI;
            m_smVisionInfo = smVisionInfo;
            m_intClass = intClass;
            m_blnWantSetToAllROICheckBox = blnWantSetToAllROICheckBox;

            InitializeComponent();

            InitDoubleThresholdForm();
        }

        private void InitDoubleThresholdForm()
        {
            if ((m_intClass & 0x01) > 0)
            {
                pnl_LowThreshold.BackColor = Color.White;
            }
            else
            {
                pnl_LowThreshold.BackColor = Color.Black;
            }
            if ((m_intClass & 0x02) > 0)
            {
                pnl_MiddleThreshold.BackColor = Color.White;
            }
            else
            {
                pnl_MiddleThreshold.BackColor = Color.Black;
            }
            if ((m_intClass & 0x04) > 0)
            {
                pnl_HighThreshold.BackColor = Color.White;
            }
            else
            {
                pnl_HighThreshold.BackColor = Color.Black;
            }

            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);
            m_Graphic2 = Graphics.FromHwnd(pic_ROI.Handle);
            UpdateGUI();

            m_smVisionInfo.g_blnViewDoubleThreshold = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_blnInitDone = true;
        }




        private bool CheckLowThreshold()
        {
            if (Convert.ToInt32(txt_LowThreshold.Text) >= m_smVisionInfo.g_intHighThresholdValue)
            {
                if (m_smVisionInfo.g_intHighThresholdValue >= 0)
                {
                    txt_LowThreshold.Text = m_smVisionInfo.g_intHighThresholdValue.ToString();
                }
                return false;
            }

            return true;
        }

        private bool CheckHighThreshold()
        {
            if (Convert.ToInt32(txt_HighThreshold.Text) <= m_smVisionInfo.g_intLowThresholdValue)
            {
                txt_HighThreshold.Text = m_smVisionInfo.g_intLowThresholdValue.ToString();
                return false;
            }
            return true;
        }

        private void UpdateGUI()
        {
            if (m_smVisionInfo.g_intHighThresholdValue == -4)
            {
                int intHighThreshold = 0; //ROI.GetAutoThresholdValue(m_objROI, 3);

                if (intHighThreshold < m_smVisionInfo.g_intLowThresholdValue)
                    intHighThreshold = m_smVisionInfo.g_intLowThresholdValue;

                pnl_HighThreshold.Width = 255 - intHighThreshold;
                txt_HighThreshold.Text = intHighThreshold.ToString();

                //2021-01-07 ZJYEOH : make sure no more -4 value
                m_smVisionInfo.g_intHighThresholdValue = intHighThreshold;

            }
            else
            {
                txt_HighThreshold.Text = m_smVisionInfo.g_intHighThresholdValue.ToString();
                pnl_HighThreshold.Width = 255 - m_smVisionInfo.g_intHighThresholdValue;
            }

            if (m_smVisionInfo.g_intLowThresholdValue == -4)
            {
                int intLowThreshold = 0;//ROI.GetAutoThresholdValue(m_objROI, 3);

                if (intLowThreshold > m_smVisionInfo.g_intHighThresholdValue)
                    intLowThreshold = m_smVisionInfo.g_intHighThresholdValue;

                pnl_LowThreshold.Width = intLowThreshold;
                txt_LowThreshold.Text = intLowThreshold.ToString();

                //2021-01-07 ZJYEOH : make sure no more -4 value
                m_smVisionInfo.g_intLowThresholdValue = intLowThreshold;
            }
            else
            {
                txt_LowThreshold.Text = m_smVisionInfo.g_intLowThresholdValue.ToString();
                pnl_LowThreshold.Width = m_smVisionInfo.g_intLowThresholdValue;
            }

            


            if (m_blnWantGain)
            {
                m_smVisionInfo.g_blnViewGainImage = true;  // Set to true since this threshold form is included Gain setting as well. 
                txt_ImageGain.Value = Math.Max(1, Convert.ToDecimal(m_smVisionInfo.g_fGainValue));
            }
            else
                panel_Gain.Visible = false;

            if (!m_blnWantSetToAllROICheckBox)
                chk_SetToOtherSideROIs.Visible = false;
        }




        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (!CheckLowThreshold())
                return;
            if (!CheckHighThreshold())
                return;

            Close();
            Dispose();
        }

        private void pic_Histogram_Paint(object sender, PaintEventArgs e)
        {
            m_blnUpdateHistogram = true;
        }

        private void splitter_SplitterMoved(object sender, SplitterEventArgs e)
        {
            txt_LowThreshold.Text = Convert.ToString(splitter1.Location.X);
            txt_HighThreshold.Text = Convert.ToString(splitter2.Location.X - 20);
            m_blnSplitter1Clicked = false;
            m_blnSplitter2Clicked = false;

            // Refresh to clear the splitter shadow after move
            Refresh();
        }

        private void splitter1_MouseDown(object sender, MouseEventArgs e)
        {
            m_blnSplitter1Clicked = true;
            m_intMouseStartPositionX = e.X;
        }

        private void splitter2_MouseDown(object sender, MouseEventArgs e)
        {
            m_blnSplitter2Clicked = true;
            m_intMouseStartPositionX = e.X;
        }

        private void splitter1_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_blnSplitter1Clicked)
            {
                // Update low threshold panel width to move the splitter
                int intThreshold = pnl_LowThreshold.Width - (m_intMouseStartPositionX - e.X);
                if ((intThreshold < 0) || (intThreshold > m_smVisionInfo.g_intHighThresholdValue))
                    return;

                txt_LowThreshold.Text = intThreshold.ToString();
            }
        }

        private void splitter2_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_blnSplitter2Clicked)
            {
                // Update low threshold panel width to move the splitter
                int intThreshold = 255 - pnl_HighThreshold.Width - (m_intMouseStartPositionX - e.X);
                if ((intThreshold > 255) || (intThreshold < m_smVisionInfo.g_intLowThresholdValue))
                    return;

                txt_HighThreshold.Text = intThreshold.ToString();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //  m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            if (chk_SetToOtherSideROIs.Visible)
                m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToOtherSideROIs.Checked;


            if (m_blnUpdateHistogram)
            {
                m_blnUpdateHistogram = false;
                m_Graphic.Clear(this.BackColor);
                ImageDrawing.DrawHistogram(m_Graphic, m_objROI, m_smVisionInfo.g_intLowThresholdValue, m_smVisionInfo.g_intHighThresholdValue);

                int Max = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
                pic_ROI.Size = new Size(Math.Min((int)Math.Ceiling((250f / Max) * (m_objROI.ref_ROIWidth)), 250), Math.Min((int)Math.Ceiling((250f / Max) * (m_objROI.ref_ROIHeight)), 250));
                panel2.Height = Math.Min((int)Math.Ceiling((250f / Max) * (m_objROI.ref_ROIHeight)), 250);
                this.Size = new Size(this.Size.Width, panel1.Location.Y + panel1.Size.Height);
                //panel1.Location = new Point(panel1.Location.X, panel2.Location.Y + panel2.Height);
                //pic_ROI.Size = new Size(250,250);
                m_Image.SetImageSize(m_objROI.ref_ROIWidth, m_objROI.ref_ROIHeight);
                m_objROI.CopyToImage(ref m_Image);
                // m_objROI.SaveImage("D:\\m_Image.bmp");
                m_Image.RedrawImage(m_Graphic2, 250f / Max, 250f / Max);// Math.Min(Max / 250f, 250f / Max), Math.Min(Max / 250f, 250f / Max));
                pic_ROI.Location = new Point(panel2.Size.Width / 2 - pic_ROI.Width / 2, panel2.Size.Height / 2 - pic_ROI.Height / 2);
            }
            int Max2 = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
            m_Image.RedrawImage(m_Graphic2, 250f / Max2, 250f / Max2);
        }
    

        private void txt_LowThreshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intValue = Convert.ToInt32(txt_LowThreshold.Text);
            if (intValue > m_smVisionInfo.g_intHighThresholdValue)
                return;

            // Update panel width to change the splitter location in run time
            pnl_LowThreshold.Width = intValue;

            // Update image
            m_smVisionInfo.g_intLowThresholdValue = intValue;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // Update histogram
            m_blnUpdateHistogram = true;

            // Update splitter panel
            pnl_Threshold.Refresh();
        }

        private void txt_HighThreshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intValue = Convert.ToInt32(txt_HighThreshold.Text);
            if (intValue < m_smVisionInfo.g_intLowThresholdValue)
                return;

            // Update panel width to change the splitter location in run time
            pnl_HighThreshold.Width = 255 - intValue;

            // Update image
            m_smVisionInfo.g_intHighThresholdValue = intValue;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // Update histogram
            m_blnUpdateHistogram = true;

            // Update splitter panel
            pnl_Threshold.Refresh();
        }

        private void txt_LowThreshold_Leave(object sender, EventArgs e)
        {
            CheckLowThreshold();
        }

        private void txt_HighThreshold_Leave(object sender, EventArgs e)
        {
            CheckHighThreshold();
        }




        private void DoubleThresholdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (chk_SetToOtherSideROIs.Visible)
                m_smVisionInfo.g_blnDrawThresholdAllSideROI = false;

            m_Image.Dispose();
            m_smVisionInfo.g_blnViewDoubleThreshold = false;
            m_smVisionInfo.g_blnViewGainImage = false;
            m_smVisionInfo.g_blnViewNormalImage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ImageGain_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_fGainValue= Convert.ToSingle(txt_ImageGain.Value);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            // Update histogram
            m_blnUpdateHistogram = true;

            // Update splitter panel
            pnl_Threshold.Refresh();
        }

        private void chk_ViewGrayImage_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewDoubleThreshold = !chk_ViewGrayImage.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void DoubleThresholdForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewNormalImage = true;
        }

        private void chk_SetToOtherSideROIs_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToOtherSideROIs.Checked;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
    }
}