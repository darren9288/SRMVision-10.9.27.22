using System;
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
    public partial class RGBThresholdForm : Form
    {
        #region Member Variables

        private bool m_blnUpdatePictureBox = false;
        private bool m_blnInitDone = false;
        private int[] m_intColorThresholdPrev = new int[3];
        private int[] m_intColorTolerancePrev = new int[3];

        private Graphics m_Graphic; 
        private CROI m_objROI;
        private VisionInfo m_smVisionInfo;
        #endregion    


        public RGBThresholdForm(VisionInfo smVisionInfo, CROI objROI)
        {
            m_intColorThresholdPrev = smVisionInfo.g_intColorThreshold;
            m_intColorTolerancePrev = smVisionInfo.g_intColorTolerance;
            m_objROI = objROI;
            m_smVisionInfo = smVisionInfo;

            InitializeComponent();

            m_Graphic = Graphics.FromHwnd(pic_Threshold.Handle);
            
            CustomizeGUI();

            m_blnInitDone = true;
        }


        private void CustomizeGUI()
        {
            txt_Red.Text = m_intColorThresholdPrev[0].ToString();
            txt_Green.Text = m_intColorThresholdPrev[1].ToString();
            txt_Blue.Text = m_intColorThresholdPrev[2].ToString();

            trackBar_Red.Value = m_intColorThresholdPrev[0];
            trackBar_Green.Value = m_intColorThresholdPrev[1];
            trackBar_Blue.Value = m_intColorThresholdPrev[2];

            txt_RedTolerance.Value = m_intColorTolerancePrev[0];
            txt_GreenTolerance.Value = m_intColorTolerancePrev[1];
            txt_BlueTolerance.Value = m_intColorTolerancePrev[2];

            UpdateRGBColorBar();
        }

        private void UpdatePictureBox()
        {
            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_objROI,
                m_smVisionInfo.g_intColorThreshold, m_smVisionInfo.g_intColorTolerance, true, pic_Threshold.Width, pic_Threshold.Height);
            else
                m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_objROI,
                m_smVisionInfo.g_intColorThreshold, m_smVisionInfo.g_intColorTolerance, true, pic_Threshold.Width, pic_Threshold.Height);
        }

        private void UpdateRGBColorBar()
        {
            int intMin = (int)trackBar_Red.Value - (int)txt_RedTolerance.Value;
            if (intMin < 0)
                intMin = 0;
            int intMax = (int)trackBar_Red.Value + (int)txt_RedTolerance.Value;
            if (intMax > 255)
                intMax = 255;
            pic_Red.Image = ColorProcessing.DrawRedColor(intMin, intMax, pic_Red.Height);

            intMin = (int)trackBar_Green.Value - (int)txt_GreenTolerance.Value;
            if (intMin < 0)
                intMin = 0;
            intMax = (int)trackBar_Green.Value + (int)txt_GreenTolerance.Value;
            if (intMax > 255)
                intMax = 255;
            pic_Green.Image = ColorProcessing.DrawGreenColor(intMin, intMax, pic_Green.Height);

            intMin = (int)trackBar_Blue.Value - (int)txt_BlueTolerance.Value;
            intMax = (int)trackBar_Blue.Value + (int)txt_BlueTolerance.Value;
            if (intMin < 0)
                intMin = 0;        
            if (intMax > 255)
                intMax = 255;
            pic_Blue.Image = ColorProcessing.DrawBlueColor(intMin, intMax, pic_Blue.Height);
        }




        private void trackBar_Red_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Red.Text = trackBar_Red.Value.ToString();
        }

        private void trackBar_Green_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Green.Text = trackBar_Green.Value.ToString();
        }

        private void trackBar_Blue_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Blue.Text = trackBar_Blue.Value.ToString();
        }




        private void txt_Red_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intColorThreshold[0] = trackBar_Red.Value = Convert.ToInt32(txt_Red.Text);
            UpdateRGBColorBar();
            m_blnUpdatePictureBox = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Green_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intColorThreshold[1] = trackBar_Green.Value = Convert.ToInt32(txt_Green.Text);
            UpdateRGBColorBar();
            m_blnUpdatePictureBox = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Blue_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intColorThreshold[2] = trackBar_Blue.Value = Convert.ToInt32(txt_Blue.Text);
            UpdateRGBColorBar();
            m_blnUpdatePictureBox = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }



        private void txt_RedTolerance_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intColorTolerance[0] = (int)txt_RedTolerance.Value;
            UpdateRGBColorBar();
            m_blnUpdatePictureBox = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GreenTolerance_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intColorTolerance[1] = (int)txt_GreenTolerance.Value;
            UpdateRGBColorBar();
            m_blnUpdatePictureBox = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_BlueTolerance_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intColorTolerance[2] = (int)txt_BlueTolerance.Value;
            UpdateRGBColorBar();
            m_blnUpdatePictureBox = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }




       

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intColorThreshold = m_intColorThresholdPrev;
            m_smVisionInfo.g_intColorTolerance = m_intColorTolerancePrev;

            Close();
            Dispose();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {      
            Close();
            Dispose();
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

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (trackBar_Red.Value != m_smVisionInfo.g_intColorThreshold[0])
                txt_Red.Text = m_smVisionInfo.g_intColorThreshold[0].ToString();

            if (trackBar_Green.Value != m_smVisionInfo.g_intColorThreshold[1])
                txt_Green.Text = m_smVisionInfo.g_intColorThreshold[1].ToString();

            if (trackBar_Blue.Value != m_smVisionInfo.g_intColorThreshold[2])
                txt_Blue.Text = m_smVisionInfo.g_intColorThreshold[2].ToString();

            if (m_blnUpdatePictureBox)
            {
                UpdatePictureBox();
                m_blnUpdatePictureBox = false;
            }
        }

        private void RGBThresholdForm_Load(object sender, EventArgs e)
        {
            UpdatePictureBox();
            m_smVisionInfo.g_blnGetPixel = true;
            m_smVisionInfo.g_blnViewColorThreshold = true;
            m_smVisionInfo.g_blnColorThresholdForm = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void RGBThresholdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_blnGetPixel = false;
            m_smVisionInfo.g_blnViewColorThreshold = false;
            m_smVisionInfo.g_blnColorThresholdForm = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }



        private void pic_Threshold_Paint(object sender, PaintEventArgs e)
        {
            m_blnUpdatePictureBox = true;
        }       
    
       
    }
}