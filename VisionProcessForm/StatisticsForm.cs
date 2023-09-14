using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VisionProcessing;
using Common;
using System.IO;

namespace VisionProcessForm
{
    public partial class StatisticsForm : Form
    {
        private string m_strVisionName;
        private string m_strSelectedRecipe;
        private string m_strPath;
        private int m_intUnitOnImage;
        private int m_intSelectedGroup;
        private bool m_blnInitDone = false;
        private List<List<ArrayList>> m_arrOCVs = new List<List<ArrayList>>();
        private List<List<ROI>> m_arrROIs = new List<List<ROI>>();

        public StatisticsForm(int intUnitOnImage, int intSelectedGroup, List<List<ArrayList>> arrOCVs, List<List<ROI>> arrROIs , string strVisionName, string strSelectedRecipe) 
        {
            InitializeComponent();

            m_strVisionName = strVisionName;
            m_strSelectedRecipe = strSelectedRecipe;
            m_intUnitOnImage = intUnitOnImage;
            m_intSelectedGroup = intSelectedGroup;
            m_arrOCVs = arrOCVs;
            m_arrROIs = arrROIs;

            for (int i = 0; i < m_arrOCVs[0][m_intSelectedGroup].Count; i++)
            {
                cbo_TemplateNo.Items.Add("Template " + (i + 1));
            }
            cbo_TemplateNo.Items.Add("All Templates");
            cbo_TemplateNo.SelectedIndex = 0;

            lbl_TestUnitCount.Text = ((OCV)m_arrOCVs[0][m_intSelectedGroup][0]).GetStatisticsCount().ToString();
            if (m_intUnitOnImage == 1)
            {
                chk_TestUnit.AutoCheck = false;
                chk_RetestUnit.Enabled = false;
                chk_RetestUnit.Checked = false;
                lbl_RetestUnitCount.Enabled = false;
            }
            else
                lbl_RetestUnitCount.Text = ((OCV)m_arrOCVs[1][m_intSelectedGroup][0]).GetStatisticsCount().ToString();

            m_strPath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_strSelectedRecipe + "\\" + m_strVisionName + "\\Mark\\Statistics\\";
            if (!Directory.Exists(m_strPath))
                Directory.CreateDirectory(m_strPath);

            m_blnInitDone = true;
        }

        private void btn_AddStatistics_Click(object sender, EventArgs e)
        {
            if (m_intUnitOnImage == 1)
            {
                for (int i = 0; i < m_arrOCVs[0][m_intSelectedGroup].Count; i++)
                {
                    if ((cbo_TemplateNo.SelectedIndex != (cbo_TemplateNo.Items.Count - 1)) && (cbo_TemplateNo.SelectedIndex != i))
                        continue;

                    OCV objUnit1OCV = (OCV)m_arrOCVs[0][m_intSelectedGroup][i];

                    if (chk_TestUnit.Checked && (objUnit1OCV.GetStatisticsCount() >= 10))
                    {
                        SRMMessageBox.Show("Ten image can be added to statistics only!");
                        return;
                    }
                }

                if (SRMMessageBox.Show("Are you sure want to add new statistic?", "Statistic", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                for (int i = 0; i < m_arrOCVs[0][m_intSelectedGroup].Count; i++)
                {
                    if ((cbo_TemplateNo.SelectedIndex != (cbo_TemplateNo.Items.Count - 1)) && (cbo_TemplateNo.SelectedIndex != i))
                        continue;

                    OCV objUnit1OCV = (OCV)m_arrOCVs[0][m_intSelectedGroup][i];

                    if (objUnit1OCV.AddManualStatistics())
                    {
                        ((ROI)m_arrROIs[0][1]).SaveImage(m_strPath + "Template0_" + i + "_" + Convert.ToString(objUnit1OCV.GetStatisticsCount() - 1) + ".bmp");
                        lbl_TestUnitCount.Text = objUnit1OCV.GetStatisticsCount().ToString();
                    }
                    else
                    {
                        SRMMessageBox.Show("Please test mark first before add statistics!");
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < m_arrOCVs[0][m_intSelectedGroup].Count; i++)
                {
                    if ((cbo_TemplateNo.SelectedIndex != (cbo_TemplateNo.Items.Count - 1)) && (cbo_TemplateNo.SelectedIndex != i))
                        continue;

                    OCV objUnit1OCV = (OCV)m_arrOCVs[0][m_intSelectedGroup][i];
                    OCV objUnit2OCV = (OCV)m_arrOCVs[1][m_intSelectedGroup][i];

                    if ((chk_TestUnit.Checked && (objUnit1OCV.GetStatisticsCount() >= 10)) ||
                        (chk_RetestUnit.Checked && (objUnit2OCV.GetStatisticsCount() >= 10)))
                    {
                        SRMMessageBox.Show("Ten image can be added to statistics only!");
                        return;
                    }
                }

                if (SRMMessageBox.Show("Are you sure want to add new statistic?", "Statistic", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                for (int i = 0; i < m_arrOCVs[0][m_intSelectedGroup].Count; i++)
                {
                    if ((cbo_TemplateNo.SelectedIndex != (cbo_TemplateNo.Items.Count - 1)) && (cbo_TemplateNo.SelectedIndex != i))
                        continue;

                    OCV objUnit1OCV = (OCV)m_arrOCVs[0][m_intSelectedGroup][i];
                    OCV objUnit2OCV = (OCV)m_arrOCVs[1][m_intSelectedGroup][i];

                    if (chk_TestUnit.Checked)
                    {
                        if (objUnit1OCV.AddManualStatistics())
                        {
                            string strPath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_strSelectedRecipe + "\\" +
                           m_strVisionName + "\\";
                            ((ROI)m_arrROIs[0][1]).SaveImage(m_strPath + "Template0_" + i + "_" + Convert.ToString(objUnit1OCV.GetStatisticsCount() - 1) + ".bmp");
                            lbl_TestUnitCount.Text = objUnit1OCV.GetStatisticsCount().ToString();
                        }
                        else
                        {
                            SRMMessageBox.Show("Please test mark unit 1 first before add statistics!");
                            break;
                        }
                    }

                    if (chk_RetestUnit.Checked)
                    {
                        if (objUnit2OCV.AddManualStatistics())
                        {
                            string strPath = AppDomain.CurrentDomain.BaseDirectory + "DeviceNo\\" + m_strSelectedRecipe + "\\" +
                            m_strVisionName + "\\";
                            ((ROI)m_arrROIs[1][1]).SaveImage(m_strPath + "Template1_" + i + "_" + Convert.ToString(objUnit2OCV.GetStatisticsCount() - 1) + ".bmp");
                            lbl_RetestUnitCount.Text = objUnit2OCV.GetStatisticsCount().ToString();
                        }
                        else
                        {
                            SRMMessageBox.Show("Please test mark unit 2 first before add statistics!");
                            break;
                        }
                    }
                }
            }
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_arrOCVs[0][m_intSelectedGroup].Count; i++)
            {
                if ((cbo_TemplateNo.SelectedIndex != (cbo_TemplateNo.Items.Count - 1)) && (cbo_TemplateNo.SelectedIndex != i))
                    continue;

                if (chk_TestUnit.Checked)
                {
                    OCV objOCV = (OCV)m_arrOCVs[0][m_intSelectedGroup][i];
                    if (objOCV.GetStatisticsCount() > 0)
                    {
                        objOCV.ClearStatistics();
                        lbl_TestUnitCount.Text = objOCV.GetStatisticsCount().ToString();
                    }
                }

                if (chk_RetestUnit.Checked)
                {
                    OCV objOCV = (OCV)m_arrOCVs[1][m_intSelectedGroup][i];
                    if (objOCV.GetStatisticsCount() > 0)
                    {
                        objOCV.ClearStatistics();
                        lbl_RetestUnitCount.Text = objOCV.GetStatisticsCount().ToString();
                    }
                }
            }
        }

        private void btn_Close_Click(object sender, EventArgs e)
        {
            Close();
            Dispose();
        }

        private void btn_Images_Click(object sender, EventArgs e)
        {
            int intUnitNo;
            if (chk_TestUnit.Checked)
                intUnitNo = 0;
            else
                intUnitNo = 1;

            StatisticsImageForm objStatisticsImageForm = new StatisticsImageForm(m_arrOCVs, m_strVisionName, m_strSelectedRecipe, m_intUnitOnImage, intUnitNo, m_intSelectedGroup,cbo_TemplateNo.SelectedIndex);
            objStatisticsImageForm.ShowDialog();
        }

        private void cbo_TemplateNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_arrOCVs[0][m_intSelectedGroup].Count > cbo_TemplateNo.SelectedIndex)
                lbl_TestUnitCount.Text = ((OCV)m_arrOCVs[0][m_intSelectedGroup][cbo_TemplateNo.SelectedIndex]).GetStatisticsCount().ToString();
            if (m_intUnitOnImage > 1)
                if (m_arrOCVs[1][m_intSelectedGroup].Count > cbo_TemplateNo.SelectedIndex)
                    lbl_RetestUnitCount.Text = ((OCV)m_arrOCVs[1][m_intSelectedGroup][cbo_TemplateNo.SelectedIndex]).GetStatisticsCount().ToString();
        }

        private void chk_TestUnit_CheckedChanged(object sender, EventArgs e)
        {
            if (!chk_TestUnit.Checked && !chk_RetestUnit.Checked)
                chk_TestUnit.Checked = true;
        }

        private void chk_RetestUnit_CheckedChanged(object sender, EventArgs e)
        {
            if (!chk_RetestUnit.Checked && !chk_TestUnit.Checked)
                chk_RetestUnit.Checked = true;
        }

    }
}