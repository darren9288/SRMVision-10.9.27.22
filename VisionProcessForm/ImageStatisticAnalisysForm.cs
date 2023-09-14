using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using SharedMemory;
using Common;
using VisionProcessing;

namespace VisionProcessForm
{
    public partial class ImageStatisticAnalisysForm : Form
    {
        private bool m_bUpdateInfo = false;
        private int m_intSaveImageCount = 0;
        private string m_strSaveImageFilePath = DBCall.m_strSVGSaveImagePath + "ImageStatisticAnalysis\\";
        private List<ImageDrawing> m_arrReferenceImages = new List<ImageDrawing>();
        private ImageDrawing m_objComparedImages = new ImageDrawing();
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        
        public ImageStatisticAnalisysForm(VisionInfo objVisionInfo, ProductionInfo smProductionInfo)
        {
            InitializeComponent();

            m_smVisionInfo = objVisionInfo;
            m_smProductionInfo = smProductionInfo;
            

            if (!Directory.Exists(m_strSaveImageFilePath))
            {
                Directory.CreateDirectory(m_strSaveImageFilePath);
            }
        }

        private void ImageStatisticAnalisysForm_Load(object sender, EventArgs e)
        {

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_bImageStatisticAnalysisUpdateInfo)
            {
                DoStatisticAnalysis();

                m_smVisionInfo.g_bImageStatisticAnalysisUpdateInfo = false;
            }

            if (!this.TopMost)
                this.TopMost = true;
        }

        public void DoStatisticAnalysis()
        {
            for (int intImageIndex = 0; intImageIndex < m_smVisionInfo.g_arrImages.Count; intImageIndex++)
            {
                ImageDrawing.CompareImage(m_smVisionInfo.g_arrImages[intImageIndex], m_arrReferenceImages[intImageIndex], m_objComparedImages);

                float fGrayMin = 0, fGrayMax = 0, fAverage = 0, fVariance = 0, fStdDev = 0;
                ImageDrawing.PixelStatistic(m_objComparedImages, ref fGrayMin, ref fGrayMax,
                    ref fAverage, ref fVariance, ref fStdDev);

                switch (intImageIndex)
                {
                    case 0:
                        {
                            lbl_GrayMin1.Text = fGrayMin.ToString();
                            lbl_GrayMax1.Text = fGrayMax.ToString();
                            lbl_Average1.Text = fAverage.ToString();
                            lbl_Variance1.Text = fVariance.ToString();
                            lbl_StdDev1.Text = fStdDev.ToString();
                        }
                        break;
                    case 1:
                        {
                            lbl_GrayMin2.Text = fGrayMin.ToString();
                            lbl_GrayMax2.Text = fGrayMax.ToString();
                            lbl_Average2.Text = fAverage.ToString();
                            lbl_Variance2.Text = fVariance.ToString();
                            lbl_StdDev2.Text = fStdDev.ToString();
                        }
                        break;
                    case 2:
                        {
                            lbl_GrayMin3.Text = fGrayMin.ToString();
                            lbl_GrayMax3.Text = fGrayMax.ToString();
                            lbl_Average3.Text = fAverage.ToString();
                            lbl_Variance3.Text = fVariance.ToString();
                            lbl_StdDev3.Text = fStdDev.ToString();
                        }
                        break;
                }
            }

            if (chk_WantSaveToTrackLog.Checked)
            {
                string strTrack = m_intSaveImageCount.ToString() + ", Min=" + lbl_GrayMin1.Text +
                ", Max=" + lbl_GrayMax1.Text +
                ", Ave=" + lbl_Average1.Text +
                ", Var=" + lbl_Variance1.Text +
                ", Std=" + lbl_StdDev1.Text;
                m_smVisionInfo.g_arrImages[0].SaveImage(m_strSaveImageFilePath + "ImageA" + m_intSaveImageCount.ToString() + ".bmp");

                if (m_smVisionInfo.g_arrImages.Count > 1)
                {
                    strTrack += ", Min=" + lbl_GrayMin2.Text +
                    ", Max=" + lbl_GrayMax2.Text +
                    ", Ave=" + lbl_Average2.Text +
                    ", Var=" + lbl_Variance2.Text +
                    ", Std=" + lbl_StdDev2.Text;
                    m_smVisionInfo.g_arrImages[1].SaveImage(m_strSaveImageFilePath + "ImageB" + m_intSaveImageCount.ToString() + ".bmp");
                }

                if (m_smVisionInfo.g_arrImages.Count > 2)
                {
                    strTrack += ", Min=" + lbl_GrayMin3.Text +
                    ", Max=" + lbl_GrayMax3.Text +
                    ", Ave=" + lbl_Average3.Text +
                    ", Var=" + lbl_Variance3.Text +
                    ", Std=" + lbl_StdDev3.Text;
                    m_smVisionInfo.g_arrImages[2].SaveImage(m_strSaveImageFilePath + "ImageC" + m_intSaveImageCount.ToString() + ".bmp");
                }

                STTrackLog.WriteLine_Report(strTrack, "ImageStatisticAnalisysReport", true);
                m_intSaveImageCount++;
            }
        }

        private void btn_SetAsReferenceData_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_arrReferenceImages.Count; i++)
            {
                m_arrReferenceImages[i].Dispose();
            }
            m_arrReferenceImages.Clear();

            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                ImageDrawing objImage = new ImageDrawing(true);
                m_smVisionInfo.g_arrImages[i].CopyTo(ref objImage);
                m_arrReferenceImages.Add(objImage);
            }

            m_intSaveImageCount = 0;
        }

        private void ImageStatisticAnalisysForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Image Statistic Analisys Form Closed", "Exit Image Statistic Analisys Form", "", "", m_smProductionInfo.g_strLotID);
            
            for (int i = 0; i < m_arrReferenceImages.Count; i++)
            {
                m_arrReferenceImages[i].Dispose();
            }
            m_arrReferenceImages.Clear();

            this.Dispose();
        }

        private void btn_StartAnalysis_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_bImageStatisticAnalysisON)
            {
                m_smVisionInfo.g_bImageStatisticAnalysisON = false;
                btn_StartAnalysis.Text = "Start Analysis";
            }
            else
            {
                m_smVisionInfo.g_bImageStatisticAnalysisON = true;
                btn_StartAnalysis.Text = "Stop Analysis";
            }
        }

        private void chk_WantSaveToTrackLog_Click(object sender, EventArgs e)
        {

        }
    }
}
