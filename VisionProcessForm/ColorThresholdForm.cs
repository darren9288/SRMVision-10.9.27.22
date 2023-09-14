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
    public partial class ColorThresholdForm : Form
    {
        #region Member Variables
        private bool m_blnInitDone = false;
        private int[] m_intLSHPrev = new int[3];
        private int[] m_intLSHTolerancePrev = new int[3];

        private CROI m_objROI;
        private VisionInfo m_smVisionInfo;
        #endregion

      

        public ColorThresholdForm(VisionInfo smVisionInfo, CROI objROI)
        {
            m_objROI = objROI;
            m_smVisionInfo = smVisionInfo;

            InitializeComponent();

            for (int i = 0; i < m_smVisionInfo.g_intColorThreshold.Length; i++)
            {
                m_intLSHPrev[i] = m_smVisionInfo.g_intColorThreshold[i];
                m_intLSHTolerancePrev[i] = m_smVisionInfo.g_intColorTolerance[i];
            }
                  
            CustomizeGUI();         
       
            m_blnInitDone = true;
        }


        private void CustomizeGUI()
        {
            txt_Lightness.Text = m_smVisionInfo.g_intColorThreshold[0].ToString();
            txt_Saturation.Text = m_smVisionInfo.g_intColorThreshold[1].ToString();
            txt_Hue.Text = m_smVisionInfo.g_intColorThreshold[2].ToString();

            trackBar_Lightness.Value = m_smVisionInfo.g_intColorThreshold[0];
            trackBar_Saturation.Value = m_smVisionInfo.g_intColorThreshold[1];
            trackBar_Hue.Value = m_smVisionInfo.g_intColorThreshold[2];

            txt_LightnessTolerance.Value = m_smVisionInfo.g_intColorTolerance[0];
            txt_SaturationTolerance.Value = m_smVisionInfo.g_intColorTolerance[1];
            txt_HueTolerance.Value = m_smVisionInfo.g_intColorTolerance[2];

            UpdateLSHColorBar();
        }

        private void UpdateLSHColorBar()
        {
            int intMinHue = (int)trackBar_Hue.Value - (int)txt_HueTolerance.Value;
            if (intMinHue < 0)
                intMinHue = 0;
            int intMaxHue = (int)trackBar_Hue.Value + (int)txt_HueTolerance.Value;
            if (intMaxHue > 255)
                intMaxHue = 255;
            pic_Hue.Image = ColorProcessing.DrawHueColor(intMinHue, intMaxHue, pic_Hue.Height);

            int intMinSaturation = (int)trackBar_Saturation.Value - (int)txt_SaturationTolerance.Value;
            if (intMinSaturation < 0)
                intMinSaturation = 0;
            int intMaxSaturation = (int)trackBar_Saturation.Value + (int)txt_SaturationTolerance.Value;
            if (intMaxSaturation > 255)
                intMaxSaturation = 255;
            pic_Saturation.Image = ColorProcessing.DrawSaturationColor(intMinSaturation, intMaxSaturation, trackBar_Hue.Value, pic_Saturation.Height);

            int intMinLightness = (int)trackBar_Lightness.Value - (int)txt_LightnessTolerance.Value;
            if (intMinLightness < 0)
                intMinLightness = 0;
            int intMaxLightness = (int)trackBar_Lightness.Value + (int)txt_LightnessTolerance.Value;
            if (intMaxLightness > 255)
                intMaxLightness = 255;
            pic_Lightness.Image = ColorProcessing.DrawLightnessColor(intMinLightness, intMaxLightness, trackBar_Hue.Value, trackBar_Saturation.Value, pic_Lightness.Height);
        }


               
        private void trackBar_Hue_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Hue.Text = trackBar_Hue.Value.ToString();
        }

        private void trackBar_Lightness_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Lightness.Text = trackBar_Lightness.Value.ToString();
        }

        private void trackBar_Saturation_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Saturation.Text = trackBar_Saturation.Value.ToString();
        }




        private void txt_Hue_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intValue = Convert.ToInt32(txt_Hue.Text);
            if (intValue < 0)
                intValue = 0;
            m_smVisionInfo.g_intColorThreshold[2] = trackBar_Hue.Value = intValue;
            UpdateLSHColorBar();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_HueTolerance_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intColorTolerance[2] = (int)txt_HueTolerance.Value;
            UpdateLSHColorBar();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Lightness_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intValue = Convert.ToInt32(txt_Lightness.Text);
            if (intValue < 0)
                intValue = 0;
            m_smVisionInfo.g_intColorThreshold[0] = trackBar_Lightness.Value = intValue;
            UpdateLSHColorBar();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void txt_LightnessTolerance_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intColorTolerance[0] = (int)txt_LightnessTolerance.Value;
            UpdateLSHColorBar();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Saturation_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intValue = Convert.ToInt32(txt_Saturation.Text);
            if (intValue < 0)
                intValue = 0;

            m_smVisionInfo.g_intColorThreshold[1] = trackBar_Saturation.Value = intValue;
            UpdateLSHColorBar();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SaturationTolerance_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intColorTolerance[1] = (int)txt_SaturationTolerance.Value;
            UpdateLSHColorBar();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }




        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intColorThreshold = m_intLSHPrev;
            m_smVisionInfo.g_intColorTolerance = m_intLSHTolerancePrev;

            this.Close();
            this.Dispose();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            m_intLSHPrev = m_smVisionInfo.g_intColorThreshold;
            m_intLSHTolerancePrev = m_smVisionInfo.g_intColorTolerance;

            this.Close();
            this.Dispose();
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





        private void timer_Tick(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intColorThreshold[0] > 0 && trackBar_Lightness.Value != m_smVisionInfo.g_intColorThreshold[0])
                txt_Lightness.Text = m_smVisionInfo.g_intColorThreshold[0].ToString();

            if (m_smVisionInfo.g_intColorThreshold[1] > 0 && trackBar_Saturation.Value != m_smVisionInfo.g_intColorThreshold[1])
                txt_Saturation.Text = m_smVisionInfo.g_intColorThreshold[1].ToString();

            if (m_smVisionInfo.g_intColorThreshold[2] > 0 && trackBar_Hue.Value != m_smVisionInfo.g_intColorThreshold[2] )
                txt_Hue.Text = m_smVisionInfo.g_intColorThreshold[2].ToString(); 
        }




        private void ColorThresholdForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnGetPixel = true;
            m_smVisionInfo.g_blnViewColorThreshold = true;
            m_smVisionInfo.g_blnColorThresholdForm = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void ColorThresholdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_blnGetPixel = false;
            m_smVisionInfo.g_blnViewColorThreshold = false;
            m_smVisionInfo.g_blnColorThresholdForm = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

      
       

     }
}