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

namespace VisionProcessForm
{
    public partial class GrayValueSensitivitySettingForm : Form
    {
        private ROI m_objMarkSearchROI = new ROI();
        private ROI m_objMarkTrainROI = new ROI();
        private ROI m_objMarkOcvSearchROI = new ROI();
        private int m_intType = 0; //0 : Mark, 1: Package
        private float m_fAverageGrayValue = 0;
        private bool m_blnInitDone = false;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private string m_strSelectedRecipe;
        
        private ImageDrawing m_Image = new ImageDrawing(true);
        private ImageDrawing m_TempImage = new ImageDrawing(true);
        private Graphics m_Graphic2;
        private ROI m_objROI;
        private bool m_blnUpdateImage = false;
        public GrayValueSensitivitySettingForm(VisionInfo visionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intType)
        {
            InitializeComponent();
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strSelectedRecipe = strSelectedRecipe;
            
            m_intType = intType;
           

            m_Graphic2 = Graphics.FromHwnd(pic_ROI.Handle);

            if (m_intType == 1)
            {
                srmLabel1.Visible = true;
                cbo_ImageBrightOrDark.Visible = true;
                chk_WantUsePackageSetting.Visible = false;
                UpdatePackageGUI();
            }
            else if (m_intType == 0)
            {
                chk_ViewThresholdImage.Visible = false;
                srmLabel1.Visible = false;
                cbo_ImageBrightOrDark.Visible = false;
                chk_WantUsePackageSetting.Visible = true;
                groupBox2.Visible = false;
                srmLabel54.Visible = false;
                txt_DarkSensitivity.Visible = false;
                UpdateMarkGUI();
            }
            
            cbo_ImageBrightOrDark.SelectedIndex = 0;
            m_blnInitDone = true;
            timer1.Start();

            m_objROI = new ROI();
            if (m_intType == 1)
            {
                if (MeasureGauge())
                {
                    AddTrainROI();
                    m_objROI.LoadROISetting(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROITotalX, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROITotalY, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROIWidth, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROIHeight);
                    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2], m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle, 4, ref m_smVisionInfo.g_arrRotatedImages, 0);
                    m_smVisionInfo.g_arrRotatedImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
                }
                else
                {
                    m_smVisionInfo.g_strErrorMessage = "*Package : Measure Gauge Fail";
                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    m_objROI.LoadROISetting(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);
                    m_smVisionInfo.g_arrImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
                }
                //m_smVisionInfo.g_arrImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
                m_objROI.AttachImage(m_TempImage);
                m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].DrawGrayValueSensitivity(cbo_ImageBrightOrDark.SelectedIndex, ref m_objROI, Convert.ToInt32(txt_InspectionAreaGrayValueSensitivity.Value), Convert.ToInt32(txt_MergeSensitivity.Value), chk_ViewThresholdImage.Checked, Convert.ToInt32(txt_BrightSensitivity.Value), Convert.ToInt32(txt_DarkSensitivity.Value), ref m_fAverageGrayValue);
            }
            else if (m_intType == 0)
            {
                m_smVisionInfo.g_arrImages[0].CopyTo(ref m_TempImage);
                m_objROI.AttachImage(m_TempImage);

                if (m_smVisionInfo.g_strSelectedPage.Contains("Mark"))
                {
                    m_objROI.LoadROISetting(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalX, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROITotalY, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
                    m_objROI.DrawBorderLine(m_smVisionInfo.g_blnWhiteOnBlack);
                }
                else
                {
                    if (StartOrientTest_using4LGauge_OcvAngleAndPackageAngle_Vision1())
                    {
                        if (StartMarkTest_using4LGauge_OcvAngleAndPackageAngle())
                        {
                            m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_TempImage);
                            m_objROI.LoadROISetting(m_objMarkTrainROI.ref_ROITotalX, m_objMarkTrainROI.ref_ROITotalY, m_objMarkTrainROI.ref_ROIWidth, m_objMarkTrainROI.ref_ROIHeight);
                        }
                        else
                        {
                            //m_smVisionInfo.g_strErrorMessage = "*Package : Measure Gauge Fail";
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            m_objROI.LoadROISetting(m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROITotalX, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROITotalY, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIHeight);
                        }
                    }
                    else
                    {
                        //m_smVisionInfo.g_strErrorMessage = "*Package : Measure Gauge Fail";
                        m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        m_objROI.LoadROISetting(m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROITotalX, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROITotalY, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIHeight);
                    }
                }
                m_smVisionInfo.g_arrMarks[0].DrawGrayValueSensitivity(ref m_objROI, Convert.ToInt32(txt_InspectionAreaGrayValueSensitivity.Value), Convert.ToInt32(txt_MergeSensitivity.Value), true, Convert.ToInt32(txt_BrightSensitivity.Value), Convert.ToInt32(txt_DarkSensitivity.Value), ref m_fAverageGrayValue);
            }
            lbl_AverageGrayValue.Text = m_fAverageGrayValue.ToString();

            int Max = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
            pic_ROI.Size = new Size(Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIWidth)), 350), Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIHeight)), 350));
            m_Image.SetImageSize(m_objROI.ref_ROIWidth, m_objROI.ref_ROIHeight);
            panel1.Height = Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIHeight)), 350);
            m_objROI.CopyToImage(ref m_Image);
            m_Image.RedrawImage(m_Graphic2, 350f / Max, 350f / Max);// Math.Min(Max / 250f, 250f / Max), Math.Min(Max / 250f, 250f / Max));
                                                                    //pic_ROI.Size = new Size((int)(250 * 250 / Max), (int)(250 * 250 / Max));
            pic_ROI.Location = new Point(panel1.Size.Width / 2 - pic_ROI.Width / 2, panel1.Size.Height / 2 - pic_ROI.Height / 2);
            m_blnUpdateImage = true;
        }
        private bool MeasureGauge()
        {
            //if (m_smVisionInfo.g_blnWantGauge) // 2019-12-09 ZJYEOH : g_blnWantGauge will only true When Mark Orient want gauge
            {
                int intUnitEdgeImageIndex = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(0);

                RectGaugeM4L objGauge;
                switch (intUnitEdgeImageIndex)
                {
                    //case 0:
                    //    objGauge = m_smVisionInfo.g_arrOrientGaugeM4L[0];
                    //    break;
                    default:
                        //case 1:
                        objGauge = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit]; // 2019-12-09 ZJYEOH : Package size gauge will always use m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit]
                        break;
                        //case 2:
                        //    objGauge = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit];
                        //    break;
                }
                ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0];
                m_smVisionInfo.g_arrImages[intUnitEdgeImageIndex].CopyTo(m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_arrImages[intUnitEdgeImageIndex].AddGain(ref m_smVisionInfo.g_objPackageImage, Convert.ToSingle(objGauge.ref_fGainValue / 1000));
                objSearchROI.AttachImage(m_smVisionInfo.g_objPackageImage);
                objGauge.SetGaugePlace_BasedOnEdgeROI();
               return objGauge.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, objSearchROI, m_smVisionInfo.g_objWhiteImage);
            }
        }
        private void AddTrainROI()
        {

            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                ROI objSearchROI = m_smVisionInfo.g_arrPackageROIs[i][0];
               
                float fCenterX = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_pRectCenterPoint.X;
                float fCenterY = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_pRectCenterPoint.Y;
                float fWidth = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_fRectWidth;
                float fHeight = m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_fRectHeight;
                int intPositionX = (int)Math.Round(fCenterX - (fWidth / 2), 0, MidpointRounding.AwayFromZero) - objSearchROI.ref_ROIPositionX + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft;
                int intPositionY = (int)Math.Round(fCenterY - (fHeight / 2), 0, MidpointRounding.AwayFromZero) - objSearchROI.ref_ROIPositionY + m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge;

                ROI objROI = new ROI("Package ROI", 2);
                objROI.AttachImage(objSearchROI);
                objROI.LoadROISetting(intPositionX, intPositionY,
                    (int)Math.Round(fWidth - m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromLeft - m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromRight, 0, MidpointRounding.AwayFromZero),
                    (int)Math.Round(fHeight - m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromEdge - m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intStartPixelFromBottom, 0, MidpointRounding.AwayFromZero));


                if (m_smVisionInfo.g_arrPackageROIs[i].Count == 2)
                    m_smVisionInfo.g_arrPackageROIs[i].Add(objROI);
                else
                    m_smVisionInfo.g_arrPackageROIs[i][2] = objROI;
            }
        }
        private void UpdatePackageGUI()
        {

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                              m_smVisionInfo.g_strVisionFolderName + "\\Package\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            txt_InspectionAreaGrayValueSensitivity.Value = Convert.ToInt32(objFileHandle.GetValueAsInt("PackageInspectionAreaGrayValueSensitivity", 45));
            txt_MergeSensitivity.Value = Convert.ToInt32(objFileHandle.GetValueAsInt("PackageMergeSensitivity", 3));
            txt_BrightSensitivity.Value = Convert.ToInt32(objFileHandle.GetValueAsInt("PackageBrightSensitivity", 50));
            txt_DarkSensitivity.Value = Convert.ToInt32(objFileHandle.GetValueAsInt("PackageDarkSensitivity", 30));

        }
        private void UpdateMarkGUI()
        {

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                              m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            txt_InspectionAreaGrayValueSensitivity.Value = Convert.ToInt32(objFileHandle.GetValueAsInt("MarkInspectionAreaGrayValueSensitivity", 45));
            txt_MergeSensitivity.Value = Convert.ToInt32(objFileHandle.GetValueAsInt("MarkMergeSensitivity", 3));
            txt_BrightSensitivity.Value = Convert.ToInt32(objFileHandle.GetValueAsInt("MarkBrightSensitivity", 50));
            txt_DarkSensitivity.Value = Convert.ToInt32(objFileHandle.GetValueAsInt("MarkDarkSensitivity", 30));
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intMarkInspectionAreaGrayValueSensitivity = m_smVisionInfo.g_intMarkInspectionAreaGrayValueSensitivity;
                m_smVisionInfo.g_arrMarks[u].ref_intMarkBrightSensitivity = m_smVisionInfo.g_intMarkBrightSensitivity;
            }
        }
        private void btn_OK_Click(object sender, EventArgs e)
        {
            if (chk_WantUsePackageSetting.Checked)
            {
                if (m_intType == 0)
                {
                    UpdatePackageGUI();
                }
            }
            if (m_intType == 1)
            {
                SavePackageSetting();
            }
            else if (m_intType == 0)
            {
                SaveMarkSetting();
            }
            this.Close();
            this.Dispose();
        }

        private void SavePackageSetting()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
m_smVisionInfo.g_strVisionFolderName + "\\Package\\Settings.xml";

            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("PackageInspectionAreaGrayValueSensitivity", Convert.ToInt32(txt_InspectionAreaGrayValueSensitivity.Value));
            objFileHandle.WriteElement1Value("PackageMergeSensitivity", Convert.ToInt32(txt_MergeSensitivity.Value));
            objFileHandle.WriteElement1Value("PackageBrightSensitivity", Convert.ToInt32(txt_BrightSensitivity.Value));
            objFileHandle.WriteElement1Value("PackageDarkSensitivity", Convert.ToInt32(txt_DarkSensitivity.Value));
            
            if (m_smVisionInfo.g_intPackageInspectionAreaGrayValueSensitivity != Convert.ToInt32(txt_InspectionAreaGrayValueSensitivity.Value))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Gray Value Setting>" + "Inspection Area Gray Value Sensitivity", m_smVisionInfo.g_intPackageInspectionAreaGrayValueSensitivity.ToString(), txt_InspectionAreaGrayValueSensitivity.Value.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intPackageMergeSensitivity != Convert.ToInt32(txt_MergeSensitivity.Value))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Gray Value Setting>" + "Merge Sensitivity", m_smVisionInfo.g_intPackageMergeSensitivity.ToString(), txt_MergeSensitivity.Value.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intPackageBrightSensitivity != Convert.ToInt32(txt_BrightSensitivity.Value))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Gray Value Setting>" + "Bright Sensitivity", m_smVisionInfo.g_intPackageBrightSensitivity.ToString(), txt_BrightSensitivity.Value.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intPackageDarkSensitivity != Convert.ToInt32(txt_DarkSensitivity.Value))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Gray Value Setting>" + "Dark Sensitivity", m_smVisionInfo.g_intPackageDarkSensitivity.ToString(), txt_DarkSensitivity.Value.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            m_smVisionInfo.g_intPackageInspectionAreaGrayValueSensitivity = Convert.ToInt32(txt_InspectionAreaGrayValueSensitivity.Value);
            m_smVisionInfo.g_intPackageMergeSensitivity = Convert.ToInt32(txt_MergeSensitivity.Value);
            m_smVisionInfo.g_intPackageBrightSensitivity = Convert.ToInt32(txt_BrightSensitivity.Value);
            m_smVisionInfo.g_intPackageDarkSensitivity = Convert.ToInt32(txt_DarkSensitivity.Value);

            objFileHandle.WriteEndElement();


        }
        private void SaveMarkSetting()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
m_smVisionInfo.g_strVisionFolderName + "\\Mark\\Settings.xml";

            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("MarkInspectionAreaGrayValueSensitivity", Convert.ToInt32(txt_InspectionAreaGrayValueSensitivity.Value));
            objFileHandle.WriteElement1Value("MarkMergeSensitivity", Convert.ToInt32(txt_MergeSensitivity.Value));
            objFileHandle.WriteElement1Value("MarkBrightSensitivity", Convert.ToInt32(txt_BrightSensitivity.Value));
            objFileHandle.WriteElement1Value("MarkDarkSensitivity", Convert.ToInt32(txt_DarkSensitivity.Value));
            
            if (m_smVisionInfo.g_intMarkInspectionAreaGrayValueSensitivity != Convert.ToInt32(txt_InspectionAreaGrayValueSensitivity.Value))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Gray Value Setting>" + "Inspection Area Gray Value Sensitivity", m_smVisionInfo.g_intMarkInspectionAreaGrayValueSensitivity.ToString(), txt_InspectionAreaGrayValueSensitivity.Value.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intMarkMergeSensitivity != Convert.ToInt32(txt_MergeSensitivity.Value))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Gray Value Setting>" + "Merge Sensitivity", m_smVisionInfo.g_intMarkMergeSensitivity.ToString(), txt_MergeSensitivity.Value.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intMarkBrightSensitivity != Convert.ToInt32(txt_BrightSensitivity.Value))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Gray Value Setting>" + "Bright Sensitivity", m_smVisionInfo.g_intMarkBrightSensitivity.ToString(), txt_BrightSensitivity.Value.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_intMarkDarkSensitivity != Convert.ToInt32(txt_DarkSensitivity.Value))
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Gray Value Setting>" + "Dark Sensitivity", m_smVisionInfo.g_intMarkDarkSensitivity.ToString(), txt_DarkSensitivity.Value.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            m_smVisionInfo.g_intMarkInspectionAreaGrayValueSensitivity = Convert.ToInt32(txt_InspectionAreaGrayValueSensitivity.Value);
            m_smVisionInfo.g_intMarkMergeSensitivity = Convert.ToInt32(txt_MergeSensitivity.Value);
            m_smVisionInfo.g_intMarkBrightSensitivity = Convert.ToInt32(txt_BrightSensitivity.Value);
            m_smVisionInfo.g_intMarkDarkSensitivity = Convert.ToInt32(txt_DarkSensitivity.Value);

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intMarkInspectionAreaGrayValueSensitivity = m_smVisionInfo.g_intMarkInspectionAreaGrayValueSensitivity;
                m_smVisionInfo.g_arrMarks[u].ref_intMarkBrightSensitivity = m_smVisionInfo.g_intMarkBrightSensitivity;
            }

            objFileHandle.WriteEndElement();


        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
        
        private void GrayValueSensitivitySettingForm_Load(object sender, EventArgs e)
        {
            //m_objROI = new ROI();
            //m_objROI.LoadROISetting(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);
            //m_smVisionInfo.g_arrImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
            //m_objROI.AttachImage(m_TempImage);
            //m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].DrawGayValueSensitivity(cbo_ImageBrightOrDark.SelectedIndex, ref m_objROI, Convert.ToInt32(txt_InspectionAreaGrayValueSensitivity.Value), Convert.ToInt32(txt_MergeSensitivity.Value), chk_ViewThresholdImage.Checked, Convert.ToInt32(txt_BrightSensitivity.Value), Convert.ToInt32(txt_DarkSensitivity.Value), ref m_fAverageGrayValue);
            //lbl_AverageGrayValue.Text = m_fAverageGrayValue.ToString();

            //int Max = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
            //pic_ROI.Size = new Size(Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIWidth)), 350), Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIHeight)), 350));
            //m_Image.SetImageSize(m_objROI.ref_ROIWidth, m_objROI.ref_ROIHeight);
            //panel1.Height = Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIHeight)), 350);
            //m_objROI.CopyToImage(ref m_Image);
            //m_Image.RedrawImage(m_Graphic2, 350f / Max, 350f / Max);// Math.Min(Max / 250f, 250f / Max), Math.Min(Max / 250f, 250f / Max));
            //                                                        //pic_ROI.Size = new Size((int)(250 * 250 / Max), (int)(250 * 250 / Max));
            //pic_ROI.Location = new Point(panel1.Size.Width / 2 - pic_ROI.Width / 2, panel1.Size.Height / 2 - pic_ROI.Height / 2);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_blnUpdateImage)
            {
                m_blnUpdateImage = false;

                if (m_intType == 1)
                {
                    if (MeasureGauge())
                    {
                        //AddTrainROI();
                        //m_objROI.LoadROISetting(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROITotalX, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROITotalY, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROIWidth, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2].ref_ROIHeight);
                        ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][2], m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle, 4, ref m_smVisionInfo.g_arrRotatedImages, 0);
                        m_smVisionInfo.g_arrRotatedImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
                    }
                    else
                    {
                        //m_smVisionInfo.g_strErrorMessage = "*Package : Measure Gauge Fail";
                        //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        //m_objROI.LoadROISetting(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalX, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalY, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIWidth, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROIHeight);
                        m_smVisionInfo.g_arrImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
                    }
                    //m_smVisionInfo.g_arrImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
                    m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].DrawGrayValueSensitivity(cbo_ImageBrightOrDark.SelectedIndex, ref m_objROI, Convert.ToInt32(txt_InspectionAreaGrayValueSensitivity.Value), Convert.ToInt32(txt_MergeSensitivity.Value), chk_ViewThresholdImage.Checked, Convert.ToInt32(txt_BrightSensitivity.Value), Convert.ToInt32(txt_DarkSensitivity.Value), ref m_fAverageGrayValue);
                }
                else if (m_intType == 0)
                {
                    //m_smVisionInfo.g_arrImages[0].CopyTo(ref m_TempImage);
                    if (m_smVisionInfo.g_strSelectedPage.Contains("Mark"))
                    {
                        if (m_smVisionInfo.g_intLearnStepNo == 7)
                            m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_TempImage);
                        else
                            m_smVisionInfo.g_arrImages[0].CopyTo(ref m_TempImage);
                        
                        m_objROI.DrawBorderLine(m_smVisionInfo.g_blnWhiteOnBlack);
                    }
                    else
                    {
                        if (StartOrientTest_using4LGauge_OcvAngleAndPackageAngle_Vision1())
                        {
                            if (StartMarkTest_using4LGauge_OcvAngleAndPackageAngle())
                            {
                                m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_TempImage);
                                //m_objROI.LoadROISetting(m_objMarkTrainROI.ref_ROITotalX, m_objMarkTrainROI.ref_ROITotalY, m_objMarkTrainROI.ref_ROIWidth, m_objMarkTrainROI.ref_ROIHeight);
                            }
                            else
                            {
                                m_smVisionInfo.g_arrImages[0].CopyTo(ref m_TempImage);
                                //m_smVisionInfo.g_strErrorMessage = "*Package : Measure Gauge Fail";
                                //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                                //m_objROI.LoadROISetting(m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROITotalX, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROITotalY, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIHeight);
                            }
                        }
                        else
                        {
                            m_smVisionInfo.g_arrImages[0].CopyTo(ref m_TempImage);
                            //m_smVisionInfo.g_strErrorMessage = "*Package : Measure Gauge Fail";
                            //m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            //m_objROI.LoadROISetting(m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROITotalX, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROITotalY, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIHeight);
                        }
                    }
                    m_smVisionInfo.g_arrMarks[0].DrawGrayValueSensitivity(ref m_objROI, Convert.ToInt32(txt_InspectionAreaGrayValueSensitivity.Value), Convert.ToInt32(txt_MergeSensitivity.Value), true, Convert.ToInt32(txt_BrightSensitivity.Value), Convert.ToInt32(txt_DarkSensitivity.Value), ref m_fAverageGrayValue);
                }

                lbl_AverageGrayValue.Text = m_fAverageGrayValue.ToString();

                int Max = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
                pic_ROI.Size = new Size(Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIWidth)), 350), Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIHeight)), 350));
                //pic_ROI.Size = new Size(250,250);
                m_Image.SetImageSize(m_objROI.ref_ROIWidth, m_objROI.ref_ROIHeight);
                panel1.Height = Math.Min((int)Math.Ceiling((350f / Max) * (m_objROI.ref_ROIHeight)), 350);
                // panel1.Location = new Point(panel1.Location.X, panel4.Location.Y+panel4.Size.Height);
                m_objROI.CopyToImage(ref m_Image);
                // m_objROI.SaveImage("D:\\m_Image.bmp");
                m_Image.RedrawImage(m_Graphic2, 350f / Max, 350f / Max);// Math.Min(Max / 250f, 250f / Max), Math.Min(Max / 250f, 250f / Max));
                                                                        //pic_ROI.Size = new Size((int)(250 * 250 / Max), (int)(250 * 250 / Max));
                pic_ROI.Location = new Point(panel1.Size.Width / 2 - pic_ROI.Width / 2, panel1.Size.Height / 2 - pic_ROI.Height / 2);

            }
            //int Max2 = Math.Max((int)(m_objROI.ref_ROIWidth), (int)(m_objROI.ref_ROIHeight));
            //m_Image.RedrawImage(m_Graphic2, 350f / Max2, 350f / Max2);
        }

        private void txt_InspectionAreaGrayValueSensitivity_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            //m_smVisionInfo.g_arrImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
            m_blnUpdateImage = true;
        }

        private void txt_MergeSensitivity_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            //m_smVisionInfo.g_arrImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
            m_blnUpdateImage = true;
        }

        private void cbo_ImageBrightOrDark_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            //m_smVisionInfo.g_arrImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
            m_blnUpdateImage = true;
        }

        private void txt_BrightSensitivity_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || (cbo_ImageBrightOrDark.SelectedIndex == 1))
                return;
            //m_smVisionInfo.g_arrImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
            m_blnUpdateImage = true;
        }

        private void txt_DarkSensitivity_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone || (cbo_ImageBrightOrDark.SelectedIndex == 0))
                return;

            //m_smVisionInfo.g_arrImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
            m_blnUpdateImage = true;
        }

        private void chk_ViewThresholdImage_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            //m_smVisionInfo.g_arrImages[ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetGrabImageIndex(cbo_ImageBrightOrDark.SelectedIndex + 2), m_smVisionInfo.g_intVisionIndex)].CopyTo(ref m_TempImage);
            m_blnUpdateImage = true;
        }

        private void chk_WantUsePackageSetting_Click(object sender, EventArgs e)
        {
            m_blnInitDone = false;
            if (chk_WantUsePackageSetting.Checked)
            {
                if (m_intType == 0)
                {
                    UpdatePackageGUI();
                }
            }
            else
            {
                if (m_intType == 0)
                {
                    UpdateMarkGUI();
                }
            }
            m_blnInitDone = true;
        }

        private bool StartOrientTest_using4LGauge_OcvAngleAndPackageAngle_Vision1()
        {
            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                return true;
            
            // make sure template learn
            if (m_smVisionInfo.g_arrOrients[0].Count == 0)
            {
                m_smVisionInfo.g_strErrorMessage += "*Mark : No Template Found";
                return false;
            }

            if (m_smVisionInfo.g_blnWantSetTemplateBasedOnBinInfo)
            {
                //Get template IO data
                m_smVisionInfo.g_intSelectedTemplate = 0;
              
                // make sure template learn
                if (m_smVisionInfo.g_arrOrients[0].Count <= m_smVisionInfo.g_intSelectedTemplate)
                {
                    m_smVisionInfo.g_strErrorMessage += "*Mark : Selected Template " + (m_smVisionInfo.g_intSelectedTemplate + 1).ToString() + " not Learnt";
                    return false;
                }

                m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate;
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
                // If Lead Unit, use unit lead pattern to find unit surface ROI
                if (((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_blnUnitPRMatcherExist)
                {
                    m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].MatchWithTemplateUnitPR(m_smVisionInfo.g_arrOrientROIs[0][0]);
                    fUnitPRResultCenterX = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterX();
                    fUnitPRResultCenterY = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultCenterY();
                    fUnitPRResultAngle = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetUnitPRResultAngle();
                    fUnitSurfaceOffsetX = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetX;
                    fUnitSurfaceOffsetY = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].ref_intUnitSurfaceOffsetY;

                    if (!Math2.GetNewXYAfterRotate(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + fUnitPRResultCenterX,
                                                  m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + fUnitPRResultCenterY,
                                                  fUnitSurfaceOffsetX,
                                                  fUnitSurfaceOffsetY,
                                                  fUnitPRResultAngle,
                                                  ref fUnitSurfaceOffsetX,
                                                  ref fUnitSurfaceOffsetY))
                    { }

                    ROI objRotateROI = new ROI();
                    objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                    objRotateROI.LoadROISetting(
                        (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX +
                        fUnitPRResultCenterX -
                        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth / 2 +
                        fUnitSurfaceOffsetX, 0, MidpointRounding.AwayFromZero),
                        (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY +
                        fUnitPRResultCenterY -
                        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight / 2 +
                        fUnitSurfaceOffsetY, 0, MidpointRounding.AwayFromZero),
                        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth,
                        m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight);


                    // Rotate unit to exact 0 degree (m_fOrientGauge used in Package)
                    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotateROI, fUnitPRResultAngle, 0, ref m_smVisionInfo.g_arrRotatedImages, 0); // Clear image is not so important in Orient Matching. Use interpolation 0 to save rotation time.

                    objRotateROI.Dispose();
                }
                else if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) > 0)    // Bottom Orient
                {
                    m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
                }
                else // if not lead unit, mean it is QFN. There is no way to find unit surface ROI without gauge tool.
                {
                    m_smVisionInfo.g_arrImages[0].CopyTo(m_smVisionInfo.g_arrRotatedImages[0]);
                }
            }

            int intMatchCount = 0;
            m_smVisionInfo.g_intOrientResult[0] = -1;  // 0:0deg, 1:90deg, 2:180deg, 3:-90, 4:Fail
       
            // 2019 10 08 - CCENG: m_smVisionInfo.g_blnInspectAllTemplate no longer set to false.
            // 2019 10 08 - CCENG: When g_blnWantSetTemplateBasedOnBinInfo is true, DoOrientationInspection instead of using MatchWithTemplate.
            // Single template test
            //if (!blnAuto && !m_smVisionInfo.g_blnInspectAllTemplate)
            //if ((!blnAuto && !m_smVisionInfo.g_blnInspectAllTemplate) || m_smVisionInfo.g_blnWantSetTemplateBasedOnBinInfo)
            //if ((!blnAuto && !m_smVisionInfo.g_blnInspectAllTemplate))
            //{
            //    m_smVisionInfo.g_arrMarks[0].ref_blnInspectAllTemplate = false;
            //    m_smVisionInfo.g_intOrientResult[0] = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].MatchWithTemplate(
            //        m_smVisionInfo.g_arrOrientROIs[0][0], m_smVisionInfo.g_arrRotatedImages[0], true);
            //    m_smVisionInfo.g_intSelectedOcv[0] = m_smVisionInfo.g_intSelectedTemplate;
            //    m_smVisionInfo.g_fOrientScore[0] = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedTemplate].GetMinScore();
            //}
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
            m_smVisionInfo.g_blnViewOrientObject = true;

            if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if (m_smVisionInfo.g_intOrientResult[0] < 4)
                {
                    if (objOrient.ref_fObjectOriX != -1 && objOrient.ref_fObjectOriY != -1 &&
                        (((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterX + m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIWidth / 2) < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) &&
                        ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterY + m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIHeight / 2) < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY))) ||
                        (((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterX - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIWidth / 2) > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) &&
                        ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterY + m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIHeight / 2) < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY))) ||
                        (((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterX + m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIWidth / 2) < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) &&
                        ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterY - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIHeight / 2) > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY))) ||
                        (((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterX - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIWidth / 2) > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) &&
                        ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterY - m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROIHeight / 2) > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY))))
                    {
                        if ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterX < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterY > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                        {
                            if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 0;
                            }
                            else if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 1;
                            }
                            else if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 2;
                            }
                            else if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 3;
                            }
                        }
                        else if ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterX < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterY < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                        {
                            if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 0;
                            }
                            else if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 1;
                            }
                            else if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 2;
                            }
                            else if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 3;
                            }
                        }
                        else if ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterX > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterY < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                        {
                            if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 0;
                            }
                            else if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 1;
                            }
                            else if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 2;
                            }
                            else if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 3;
                            }
                        }
                        else if ((m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterX > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROITotalCenterY > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                        {
                            if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 0;
                            }
                            else if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 1;
                            }
                            else if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 2;
                            }
                            else if ((m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalX + objOrient.ref_fObjectOriX > (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX)) && (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalY + objOrient.ref_fObjectOriY < (m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY)))
                            {
                                m_smVisionInfo.g_intOrientResult[0] = 3;
                            }
                        }
                    }

                    int intOrientAngle;
                    switch (m_smVisionInfo.g_intOrientResult[0])
                    {
                        default:
                        case 0:
                            intOrientAngle = 0;
                            break;
                        case 1:
                            intOrientAngle = 90;
                            break;
                        case 2:
                            intOrientAngle = 180;
                            break;
                        case 3:
                            intOrientAngle = -90;
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
                        fTotalRotateAngle = intOrientAngle + objGauge.ref_fRectAngle;

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
                            fTotalRotateAngle = intOrientAngle + fUnitPRResultAngle;

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
                            fTotalRotateAngle = intOrientAngle + m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fDegAngleResult;

                            // Get RotateROI where the ROI center point == Unit Center Point
                            objRotatedROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                            float fSizeX, fSizeY;
                            fSizeX = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIWidth % 2; // why %2? To get "even" number
                            fSizeY = m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIHeight % 2;

                            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                            {
                                if (MeasureGauge())
                                {
                                    // Get RotateROI center point where the ROI center point == Package Unit Center Point
                                    objRotatedROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - fSizeX / 2, 0, MidpointRounding.AwayFromZero),
                                                                 (int)Math.Round(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - fSizeY / 2, 0, MidpointRounding.AwayFromZero),
                                                                 (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                                                 (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));

                                    // 2020 03 03 - CCENG: Enable this command to use package angle instead of mark angle. 
                                    // 2020 05 02 - No longer need this feature because mark and package angle are separated to different images.
                                    //if (m_smVisionInfo.g_blnWantRotateMarkImageUsingPkgAngle)
                                    //    fTotalRotateAngle = m_smVisionInfo.g_objGauge_PkgSize.ref_fRectAngle; // 2020 04 19 - Should add m_intOrientAngle? 
                                }
                                else
                                {
                                    objRotatedROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalCenterX - fSizeX / 2, 0, MidpointRounding.AwayFromZero),
                                                                (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[m_smVisionInfo.g_intSelectedUnit][0].ref_ROITotalCenterY - fSizeY / 2, 0, MidpointRounding.AwayFromZero),
                                                                (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                                                (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));
                                }

                            }
                            else
                            {
                                // 2019 09 06 - CCENG: If no package size center point, then use Orient Search ROI Center Point
                                objRotatedROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterX - fSizeX / 2, 0, MidpointRounding.AwayFromZero),
                                                             (int)Math.Round(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROITotalCenterY - fSizeY / 2, 0, MidpointRounding.AwayFromZero),
                                                             (int)Math.Round(fSizeX, 0, MidpointRounding.AwayFromZero),
                                                             (int)Math.Round(fSizeY, 0, MidpointRounding.AwayFromZero));

                            }
                        }
                    }

                    // Start Rotate image 1 to zero orientation and zero angle degree.
                    // 2020 05 02 - Rotate image to package 0 deg
                    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], objRotatedROI, m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectAngle, 4, ref m_smVisionInfo.g_arrRotatedImages, 0);
                    //m_smVisionInfo.g_arrRotatedImages[0].SaveImage("D:\\g_arrRotatedImages.bmp");
                    // 2020 04 19 - cceng: Rotate image to mark 0 deg
                    ROI.Rotate0Degree_Better(m_smVisionInfo.g_arrImages[0], objRotatedROI, fTotalRotateAngle, 4, ref m_smVisionInfo.g_objMarkImage);
                    //m_smVisionInfo.g_objMarkImage.SaveImage("D:\\g_objMarkImage.bmp");



                    objRotatedROI.Dispose();
                }
            }
            else
            {
                m_smVisionInfo.g_fOrientCenterX[0] = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fObjectX;
                m_smVisionInfo.g_fOrientCenterY[0] = m_smVisionInfo.g_arrOrients[0][m_smVisionInfo.g_intSelectedOcv[0]].ref_fObjectY;
            }
            
            return true;
        }

        private bool StartMarkTest_using4LGauge_OcvAngleAndPackageAngle()
        {
            try
            {
                if ((m_smCustomizeInfo.g_intWantMark & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_arrMarks[0].GetNumTemplates() == 0)
                    {
                        // 2020 04 15 - JBTAN: check mark option enable only will set result fail
                        if (m_smVisionInfo.g_arrMarks[0].ref_blnCheckMark)
                        {
                            m_smVisionInfo.g_strErrorMessage += "*Mark : No Template Found";
                            
                        }
                        return false;
                    }
                   

                    m_objMarkSearchROI = m_smVisionInfo.g_arrMarkROIs[0][0];
                    if (m_objMarkTrainROI == null)
                        m_objMarkTrainROI = new ROI();
                    m_objMarkTrainROI.AttachImage(m_objMarkSearchROI);
                    m_objMarkTrainROI.LoadROISetting(m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIPositionX, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIPositionY,
                                               m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);
                    //m_objMarkTrainROI.SaveImage("D:\\TS\\m_objMarkTrainROI3.bmp");

                    if (m_objMarkOcvSearchROI == null)
                        m_objMarkOcvSearchROI = new ROI();
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

                                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], m_objMarkSearchROI, fGaugeAngle, ref m_smVisionInfo.g_arrRotatedImages, 0);
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

                            //         float Angle = m_intOrientAngle + objGauge.ref_fRectAngle;
                            //         float CenterX = (float)(m_objMarkSearchROI.ref_ROITotalCenterX);
                            //         float CenterY = (float)(m_objMarkSearchROI.ref_ROITotalCenterY);
                            //         float fXAfterRotated = (float)((CenterX) + ((intGaugeCenterX - CenterX) * Math.Cos(Angle * Math.PI / 180)) -
                            //((intGaugeCenterY - CenterY) * Math.Sin(Angle * Math.PI / 180)));
                            //         float fYAfterRotated = (float)((CenterY) + ((intGaugeCenterX - CenterX) * Math.Sin(Angle * Math.PI / 180)) +
                            //          ((intGaugeCenterY - CenterY) * Math.Cos(Angle * Math.PI / 180)));

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
                        //else if (m_smVisionInfo.g_arrMarkROIs[0].Count > 3)    // Mean Unit Surface ROI exist
                        //{
                        //    // Only positioning and lead will use unit surface ROI
                        //    if (((m_smCustomizeInfo.g_intWantPositioningIndex & (1 << m_smVisionInfo.g_intVisionPos)) > 0) ||
                        //        ((m_smCustomizeInfo.g_intWantLead & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
                        //    {
                        //        // Get offset value between Unit Surface ROI and Mark ROI center points
                        //        int intMarkROIOffsetX = m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROICenterX - m_smVisionInfo.g_arrMarkROIs[0][3].ref_ROICenterX;
                        //        int intMarkROIOffsetY = m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROICenterY - m_smVisionInfo.g_arrMarkROIs[0][3].ref_ROICenterY;

                        //        // Find unit ROI (Train ROI)
                        //        m_objMarkTrainROI.ref_ROIPositionX = (int)Math.Round(m_smVisionInfo.g_fUnitCenterX[0] - (float)m_objMarkTrainROI.ref_ROIWidth / 2 + intMarkROIOffsetX, 0, MidpointRounding.AwayFromZero);   // 2018 10 09 - CCENG: change g_fOrientCenterXY to g_fUnitCenterXY because g_fOrientCenter is used to keep Orient matcher result center point, not orient gauge unit center point.
                        //        m_objMarkTrainROI.ref_ROIPositionY = (int)Math.Round(m_smVisionInfo.g_fUnitCenterY[0] - (float)m_objMarkTrainROI.ref_ROIHeight / 2 + intMarkROIOffsetY, 0, MidpointRounding.AwayFromZero);

                        //    }
                        //}
                        else //No gauge
                        {
                            /* 2019 07 12 - CCENG: 
                             * No gauge mean dun know where is the unit.
                             * Mark ROI will be relocated according to orient PR result + offset between orient ROI and mark ROI
                             */


                            //m_objMarkTrainROI.LoadROISetting((int)m_smVisionInfo.g_fOrientCenterX[0] +                  // Orient Result Object Center X
                            //                                   m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROICenterX -      // Mark ROI Center X
                            //                                   m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROICenterX -    // Orient ROI Center X
                            //                                   m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth / 2,     // Half of Mark ROI Size X
                            //                                   (int)m_smVisionInfo.g_fOrientCenterY[0] +
                            //                                   m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROICenterY -
                            //                                   m_smVisionInfo.g_arrOrientROIs[0][1].ref_ROICenterY -
                            //                                   m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight / 2,
                            //                                   m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);

                            bool blnUsePackageCenterPoint = false;

                            // 2019-12-17 JBTAN: will not run this sequence if disable package size test
                            if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x1000) != 0)
                            {
                                if (MeasureGauge())
                                {
                                    blnUsePackageCenterPoint = true;
                                }
                                else
                                {
                                    m_smVisionInfo.g_strErrorMessage += "*Mark : Measure Gauge Fail";
                                    // 2019-11-08 ZJYEOH : Reset the inspection data so that offline page result does not store previous unit result
                                    m_smVisionInfo.g_arrMarks[0].ResetInspectionData(true);
                                    // 2019-11-08 ZJYEOH : if package fail, no need to check mark anymore
                                    return false;
                                }
                            }

                            int intOrientAngle;
                            switch (m_smVisionInfo.g_intOrientResult[0])
                            {
                                default:
                                case 0:
                                    intOrientAngle = 0;
                                    break;
                                case 1:
                                    intOrientAngle = 90;
                                    break;
                                case 2:
                                    intOrientAngle = 180;
                                    break;
                                case 3:
                                    intOrientAngle = -90;
                                    break;
                            }

                            if (blnUsePackageCenterPoint)
                            {
                                // Get RotateROI center point where the ROI center point == Unit Center Point
                                //m_objMarkTrainROI.LoadROISetting(
                                //            (int)Math.Round(m_smVisionInfo.g_objGauge_PkgSize.ref_pRectCenterPoint.X -
                                //            m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIPositionX -
                                //            m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero),
                                //            (int)Math.Round(m_smVisionInfo.g_objGauge_PkgSize.ref_pRectCenterPoint.Y -
                                //            m_smVisionInfo.g_arrMarkROIs[0][0].ref_ROIPositionY -
                                //            m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero),
                                //            m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIWidth, m_smVisionInfo.g_arrMarkROIs[0][1].ref_ROIHeight);

                                if (m_smVisionInfo.g_fOrientCenterX[0] != -1 && m_smVisionInfo.g_fOrientCenterY[0] != -1)    // Orient test able to get position
                                {
                                    // 2019 10 15 - CCENG: Use m_smVisionInfo.g_objGauge_PkgSize measurement center point as rotation center point reference to get mark train roi "0 orient direction and 0 deg" center points.
                                    float CenterX = (float)(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionX);
                                    float CenterY = (float)(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionY);

                                    float Angle = m_smVisionInfo.g_fOrientAngle[0] + (intOrientAngle); // this formula is based on clockwise rotation so the angle need to be inverted, 
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

                                // 2019-11-08 ZJYEOH : Limit Mark inspection ROI within package size area, so that extra mark wont happen outside the package size area

                                if (m_objMarkTrainROI.ref_ROITotalX < (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2))
                                {
                                    m_objMarkTrainROI.LoadROISetting(m_objMarkTrainROI.ref_ROIPositionX + (int)(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) - m_objMarkTrainROI.ref_ROITotalX,
                                m_objMarkTrainROI.ref_ROIPositionY,
                                m_objMarkTrainROI.ref_ROIWidth - ((int)(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2) - m_objMarkTrainROI.ref_ROITotalX), m_objMarkTrainROI.ref_ROIHeight);
                                }

                                if (m_objMarkTrainROI.ref_ROITotalY < (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2))
                                {
                                    m_objMarkTrainROI.LoadROISetting(m_objMarkTrainROI.ref_ROIPositionX,
                                                   m_objMarkTrainROI.ref_ROIPositionY + (int)(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) - m_objMarkTrainROI.ref_ROITotalY,
                                                    m_objMarkTrainROI.ref_ROIWidth, m_objMarkTrainROI.ref_ROIHeight - ((int)(m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y - m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2) - m_objMarkTrainROI.ref_ROITotalY));
                                }

                                if (m_objMarkTrainROI.ref_ROITotalY + m_objMarkTrainROI.ref_ROIHeight > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2))
                                {
                                    m_objMarkTrainROI.LoadROISetting(m_objMarkTrainROI.ref_ROIPositionX, m_objMarkTrainROI.ref_ROIPositionY,
                                m_objMarkTrainROI.ref_ROIWidth, m_objMarkTrainROI.ref_ROIHeight - (int)(m_objMarkTrainROI.ref_ROITotalY + m_objMarkTrainROI.ref_ROIHeight - (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.Y + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectHeight / 2)));
                                }

                                if (m_objMarkTrainROI.ref_ROITotalX + m_objMarkTrainROI.ref_ROIWidth > (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2))
                                {
                                    m_objMarkTrainROI.LoadROISetting(m_objMarkTrainROI.ref_ROIPositionX, m_objMarkTrainROI.ref_ROIPositionY,
                                m_objMarkTrainROI.ref_ROIWidth - (int)(m_objMarkTrainROI.ref_ROITotalX + m_objMarkTrainROI.ref_ROIWidth - (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_pRectCenterPoint.X + m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_fRectWidth / 2)), m_objMarkTrainROI.ref_ROIHeight);
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_fOrientCenterX[0] != -1 && m_smVisionInfo.g_fOrientCenterY[0] != -1)    // Orient test able to get position
                                {

                                    // 2019 08 16 ZJYEOH : To load the Mark ROI without gauge, the center point cannot use directly from m_smVisionInfo.g_fOrientCenterX[0] as this point is before rotated
                                    float CenterX = (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROICenterX - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionX);
                                    float CenterY = (float)(m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROICenterY - m_smVisionInfo.g_arrOrientROIs[0][0].ref_ROIPositionY);

                                    float Angle = m_smVisionInfo.g_fOrientAngle[0] + (intOrientAngle); // this formula is based on clockwise rotation so the angle need to be inverted, 
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

                        if (((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0))
                        {
                            // Find unit ROI (Train ROI)
                            m_objMarkTrainROI.ref_ROIPositionX = (int)Math.Round(m_smVisionInfo.g_fUnitCenterX[0] - (float)m_objMarkTrainROI.ref_ROIWidth / 2, 0, MidpointRounding.AwayFromZero);   // 2018 10 09 - CCENG: change g_fOrientCenterXY to g_fUnitCenterXY because g_fOrientCenter is used to keep Orient matcher result center point, not orient gauge unit center point.
                            m_objMarkTrainROI.ref_ROIPositionY = (int)Math.Round(m_smVisionInfo.g_fUnitCenterY[0] - (float)m_objMarkTrainROI.ref_ROIHeight / 2, 0, MidpointRounding.AwayFromZero);
                        }
                    }
                    
                    }
                

                return true;

            }
            catch (Exception ex)
            {
                //objTL1.WriteLine("Vision1Process StartMarkTest ex: " + ex.ToString());
                return false;
            }
        }
    }
}
