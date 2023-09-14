using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common;
using SharedMemory;
using VisionProcessing;
using System.IO;
using Euresys.Open_eVision_2_12;

namespace VisionProcessForm
{
    public partial class LiterationSettingsForm : Form
    {
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private string m_strSelectedRecipe;
        private bool m_blnInitDone = false;
        private int Erode = -1, Dilate = -1, Missing = -1;
        private ROI tempROiResult = new ROI();
        private ROI temp_ROISampleMissing = new ROI();
        private ROI temp_ROI2TemplateMissing = new ROI();
        private ROI temp_ROI2SampleExcess = new ROI();
        private ROI temp_ROITemplateExcess = new ROI();
        private ROI temp_ROIResult2 = new ROI();


        public LiterationSettingsForm(VisionInfo visionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strselectedRecipe)
        {
            InitializeComponent();
            m_strSelectedRecipe = strselectedRecipe;
            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            txt_MissingMarkOpenMorphology.Text = m_smVisionInfo.g_arrMarks[0].ref_intCharOpenHalfWidth.ToString();
            txt_ThinIteration.Text = m_smVisionInfo.g_arrMarks[0].ref_intCharErodeHalfWidth.ToString();
            txt_ThickIteration.Text = m_smVisionInfo.g_arrMarks[0].ref_intCharDilateHalfWidth.ToString();
            Missing = Convert.ToInt32(txt_MissingMarkOpenMorphology.Text);
            Erode = Convert.ToInt32(txt_ThinIteration.Text);
            Dilate = Convert.ToInt32(txt_ThickIteration.Text);
            m_smVisionInfo.g_blnWantShowLiterationOnly = true;
            tempROiResult.LoadROISetting(0, 0, m_smVisionInfo.g_arrMarks[0].getImage(0).ref_intImageWidth, m_smVisionInfo.g_arrMarks[0].getImage(0).ref_intImageHeight);
            temp_ROIResult2.LoadROISetting(0, 0, m_smVisionInfo.g_arrMarks[0].getImage(1).ref_intImageWidth, m_smVisionInfo.g_arrMarks[0].getImage(1).ref_intImageHeight);
            temp_ROISampleMissing.LoadROISetting(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalX, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalY, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
            temp_ROI2TemplateMissing.LoadROISetting(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalX, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalY, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
            temp_ROITemplateExcess.LoadROISetting(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalX, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalY, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
            temp_ROI2SampleExcess.LoadROISetting(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalX, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalY, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
            TriggerOfflineTest();
            m_blnInitDone = true;
        }

        private void TriggerOfflineTest()
        {
            if (m_smVisionInfo.AT_PR_StartLiveImage)
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            }
            m_smVisionInfo.AT_VM_ManualTestMode = true;
            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
        }


        private void txt_ThinIteration_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || Convert.ToInt32(txt_ThinIteration.Text) > 100)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intCharErodeHalfWidth = Convert.ToInt32(txt_ThinIteration.Text);
            }

            TriggerOfflineTest();
            if (Erode != Convert.ToInt32(txt_ThinIteration.Text))
            {
                pnl_Paint_SampleMissing.Refresh();
                pnl_Paint_TemplateMissing.Refresh();
                pnl_Paint_ResultMissing.Refresh();
            }

            Erode = m_smVisionInfo.g_arrMarks[0].ref_intCharErodeHalfWidth;
        }

        private void txt_ThickIteration_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || Convert.ToInt32(txt_ThickIteration.Text) > 100)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intCharDilateHalfWidth = Convert.ToInt32(txt_ThickIteration.Text);
            }

            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            Directory.CreateDirectory(strFolderPath + "Mark\\Template_Temp\\");

            foreach (string newPath in Directory.GetFiles(strFolderPath + "Mark\\Template\\", "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(strFolderPath + "Mark\\Template\\", strFolderPath + "Mark\\Template_Temp\\"), true);
            }

            SaveMarkSettings(strFolderPath + "Mark\\Template_Temp\\");
            LoadMarkSettings(strFolderPath + "Mark\\Template_Temp\\");  

            TriggerOfflineTest();          
            if (Dilate != Convert.ToInt32(txt_ThickIteration.Text))
            {
                pnl_Paint_TemplateExcess.Refresh();
                pnl_Paint_ResultExcess.Refresh();
            }

            Dilate = m_smVisionInfo.g_arrMarks[0].ref_intCharDilateHalfWidth;
        }

        private void LoadMarkSettings(string strFolderPath)
        {
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].LoadTemplate(strFolderPath, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
            }
        }

        private void SaveMarkSettings(string strFolderPath)
        {
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].SaveTemplate(strFolderPath, false);
            }
        }

        private void txt_MissingMarkOpenMorphology_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || Convert.ToInt32(txt_MissingMarkOpenMorphology.Text) > 100)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intCharOpenHalfWidth = Convert.ToInt32(txt_MissingMarkOpenMorphology.Text);
            }

            TriggerOfflineTest();
            if (Missing != Convert.ToInt32(txt_MissingMarkOpenMorphology.Text))
                pnl_Paint_ResultMissing.Refresh();

            Missing = m_smVisionInfo.g_arrMarks[0].ref_intCharOpenHalfWidth;
        }

        private void pnl_Paint_Paint(object sender, PaintEventArgs e)
        {
            do
            {
                //do nothing wait for test done

            } while (!m_smVisionInfo.PR_MN_TestDone);

            tempROiResult.AttachImage(m_smVisionInfo.g_arrMarks[0].getImage(0));
            tempROiResult.DrawZoomImage(e.Graphics, pnl_Paint_ResultMissing.Width, pnl_Paint_ResultMissing.Height);
        }

        private void pnl_Paint_SampleMissing_Paint(object sender, PaintEventArgs e)
        {
            ImageDrawing newImage = new ImageDrawing();
            int intThresholdValue = m_smVisionInfo.g_arrMarks[0].GetThreshold();
            m_smVisionInfo.g_arrImages[0].CopyTo(ref newImage);
            temp_ROISampleMissing.AttachImage(newImage);
            EasyImage.Threshold(temp_ROISampleMissing.ref_ROI, temp_ROISampleMissing.ref_ROI, (uint)intThresholdValue);
            EasyImage.DilateBox(temp_ROISampleMissing.ref_ROI, temp_ROISampleMissing.ref_ROI, (uint)m_smVisionInfo.g_arrMarks[0].ref_intCharErodeHalfWidth);
            temp_ROISampleMissing.DrawZoomImage(e.Graphics, pnl_Paint_SampleMissing.Width, pnl_Paint_SampleMissing.Height);
            newImage.Dispose();
            newImage = null;
        }

        private void pnl_Paint_TemplateMissing_Paint(object sender, PaintEventArgs e)
        {
            int intThresholdValue = m_smVisionInfo.g_arrMarks[0].GetThreshold();
            ImageDrawing newImage2 = new ImageDrawing();
            newImage2.LoadImage(m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\" + "OriTemplate" + m_smVisionInfo.g_intSelectedGroup + "_" + m_smVisionInfo.g_intSelectedTemplate + ".bmp");
            temp_ROI2TemplateMissing.AttachImage(newImage2);
            EasyImage.Threshold(temp_ROI2TemplateMissing.ref_ROI, temp_ROI2TemplateMissing.ref_ROI, (uint)intThresholdValue);
            temp_ROI2TemplateMissing.DrawZoomImage(e.Graphics, pnl_Paint_TemplateMissing.Width, pnl_Paint_TemplateMissing.Height);
            newImage2.Dispose();
            newImage2 = null;
        }

        private void Pnl_Paint_SampleExcess_Paint(object sender, PaintEventArgs e)
        {
            ImageDrawing newImage2 = new ImageDrawing();
            m_smVisionInfo.g_arrImages[0].CopyTo(ref newImage2);
            temp_ROI2SampleExcess.AttachImage(newImage2);
            temp_ROI2SampleExcess.DrawZoomImage(e.Graphics, Pnl_Paint_SampleExcess.Width, Pnl_Paint_SampleExcess.Height);
            newImage2.Dispose();
            newImage2 = null;
        }

        private void pnl_Paint_TemplateExcess_Paint(object sender, PaintEventArgs e)
        {
            int intThresholdValue = m_smVisionInfo.g_arrMarks[0].GetThreshold();
            ImageDrawing newImage = new ImageDrawing();
            newImage.LoadImage(m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Template\\" + "OriTemplate" + m_smVisionInfo.g_intSelectedGroup + "_" + m_smVisionInfo.g_intSelectedTemplate + ".bmp");
            temp_ROITemplateExcess.AttachImage(newImage);
            EasyImage.Threshold(temp_ROITemplateExcess.ref_ROI, temp_ROITemplateExcess.ref_ROI, (uint)intThresholdValue);
            EasyImage.DilateBox(temp_ROITemplateExcess.ref_ROI, temp_ROITemplateExcess.ref_ROI, (uint)m_smVisionInfo.g_arrMarks[0].ref_intCharDilateHalfWidth);
            temp_ROITemplateExcess.DrawZoomImage(e.Graphics, pnl_Paint_TemplateExcess.Width, pnl_Paint_TemplateExcess.Height);
            newImage.Dispose();
            newImage = null;
        }

        private void pnl_paint2_Paint(object sender, PaintEventArgs e)
        {
            do
            {        
                //do nothing wait for test done

            } while (!m_smVisionInfo.PR_MN_TestDone);

            int intThresholdValue = m_smVisionInfo.g_arrMarks[0].GetThreshold();
            temp_ROIResult2.AttachImage(m_smVisionInfo.g_arrMarks[0].getImage(1));
            EasyImage.Threshold(temp_ROIResult2.ref_ROI, temp_ROIResult2.ref_ROI, (uint)intThresholdValue);
            temp_ROIResult2.DrawZoomImage(e.Graphics, pnl_Paint_ResultExcess.Width, pnl_Paint_ResultExcess.Height);
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnWantShowLiterationOnly = false;
            tempROiResult.Dispose();
            temp_ROIResult2.Dispose();
            temp_ROI2SampleExcess.Dispose();
            temp_ROISampleMissing.Dispose();
            temp_ROI2TemplateMissing.Dispose();
            temp_ROITemplateExcess.Dispose();
            tempROiResult = null;
            temp_ROIResult2 = null;
            temp_ROITemplateExcess = null;
            temp_ROI2SampleExcess = null;
            temp_ROI2TemplateMissing = null;
            temp_ROISampleMissing = null;
        }
    }
}
