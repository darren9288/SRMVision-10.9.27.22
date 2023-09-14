using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VisionProcessing;

namespace VisionProcessForm
{
    public partial class StatisticsImageForm : Form
    {
        private string m_strVisionName;
        private string m_strSelectedRecipe;
        private int m_intUnitOnImage;
        private int m_intUnitNo;
        private int m_intGroupNo;
        private bool m_blnInitDone = false;
        private List<List<ArrayList>> m_arrOCVs = new List<List<ArrayList>>();

        public StatisticsImageForm(List<List<ArrayList>> arrOCVs, string strVisionName, string strSelectedRecipe, int intUnitNoImage, int intUnitNo, int intGroupNo, int intTemplateNo)
        {
            InitializeComponent();
            m_strVisionName = strVisionName;
            m_strSelectedRecipe = strSelectedRecipe;
            m_intUnitOnImage = intUnitNoImage;
            m_intUnitNo = intUnitNo;
            m_intGroupNo = intGroupNo;
            m_arrOCVs = arrOCVs;

            for (int i = 0; i < m_arrOCVs[0][m_intGroupNo].Count; i++)
            {
                cbo_TemplateNo.Items.Add("Template " + (i + 1));
            }

            cbo_TemplateNo.SelectedIndex = intTemplateNo;

            if (m_intUnitOnImage == 1)
                group_UnitNo.Visible = false;

            if (m_intUnitNo == 0)
                radioBtn_Template1.Checked = true;
            else
                radioBtn_Template2.Checked = true;
         
            LoadTemplateImage();

            m_blnInitDone = true;
        }

        public void LoadTemplateImage()
        {
            string strPath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_strSelectedRecipe + 
                            "\\" + m_strVisionName + "\\Mark\\Statistics\\";

            Control ctrImage = pic_StatisticsImage1;
            int intStatisticCount = ((OCV)m_arrOCVs[m_intUnitNo][m_intGroupNo][cbo_TemplateNo.SelectedIndex]).GetStatisticsCount();
            for (int i = 0; i < 10; i++)
            {
                if (i < intStatisticCount)
                {
                    ((PictureBox)ctrImage).Load(strPath + "Template" + m_intUnitNo + "_" + cbo_TemplateNo.SelectedIndex + "_" + i + ".bmp");
                    
                }
                else
                {
                    ((PictureBox)ctrImage).Image = null;
                }
                ctrImage = GetNextControl(ctrImage, true);
            }
        }

        private void cbo_TemplateNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            LoadTemplateImage();
        }

        private void radioBtn_UnitNo_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (radioBtn_Template1.Checked)
                m_intUnitNo = 0;
            else
                m_intUnitNo = 1;

            LoadTemplateImage();
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }
    }
}