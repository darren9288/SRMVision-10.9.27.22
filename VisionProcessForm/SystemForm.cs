using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using ImageAcquisition;
using Lighting;
using SharedMemory;
using VisionProcessing;
using System.Threading;
using System.IO;
namespace VisionProcessForm
{
    public partial class SystemForm : Form
    {
        #region Member Variables
        public List<int> m_arrImageMaskingSetting_Prev = new List<int>();
        public List<float> m_arrImageMaskingGain_Prev = new List<float>();

        private int m_intUserGroup = 5;
        private int m_intSelectedImage = 0;
        private bool m_blnInitDone = false;
        private string m_strFilePath;
        private string m_strSelectedRecipe = "";
        private Graphics m_Graphic;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private int m_intDisplayStepNo = 1;
        #endregion

        public SystemForm(VisionInfo smVisionInfo, string strSelectedRecipe, int intUserGroup, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            m_intUserGroup = intUserGroup;
            m_strSelectedRecipe = strSelectedRecipe;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            
            m_strFilePath = m_smProductionInfo.g_strRecipePath + strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            m_smCustomizeInfo = smCustomizeInfo;
            InitializeComponent();
            UpdateGUI();
            DisableField2();
            m_blnInitDone = true;
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild2 = "Set Front Light View Page";
            string strChild3 = "Save Button";

            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Save.Enabled = false;
            }

            strChild3 = "Live Button"; 
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Live.Enabled = false; //CLLEE 2022-08-01: Change  btn_Save.Enabled = false -> btn_Live.Enabled
            }

            strChild3 = "Image Setting";
            if (m_smProductionInfo.g_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                grpBox_ImageSetting.Enabled = false;
            }
        }
        private int GetUserRightGroup_Child3(string strChild2, string strChild3)
        {
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(strChild2, strChild3);
                    break;
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad4S":
                case "Pad4SPos":
                case "Pad4SPkg":
                case "Pad4SPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(strChild2, strChild3);
                    break;
                case "Li3D":
                case "Li3DPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(strChild2, strChild3);
                    break;
            }

            return 1;
        }
        public void AddSystemSearchROI()
        {
            ROI objROI;
            for (int i = 0; i < 5; i++)
            {
                switch (i)
                {
                    case 0:
                        //Middle
                        if (m_smVisionInfo.g_arrSystemROI.Count == 0)
                        {
                            objROI = new ROI("System Search ROI", 1);
                            objROI.LoadROISetting(m_smVisionInfo.g_intCameraResolutionWidth / 4, m_smVisionInfo.g_intCameraResolutionHeight / 4, m_smVisionInfo.g_intCameraResolutionWidth / 2, m_smVisionInfo.g_intCameraResolutionHeight / 2);
                            m_smVisionInfo.g_arrSystemROI.Add(objROI);
                        }
                        else
                        {
                            m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(((ROI)m_smVisionInfo.g_arrSystemROI[i]).ref_ROIPositionX,
                                                                             ((ROI)m_smVisionInfo.g_arrSystemROI[i]).ref_ROIPositionY,
                                                                            ((ROI)m_smVisionInfo.g_arrSystemROI[i]).ref_ROIWidth,
                                                                            ((ROI)m_smVisionInfo.g_arrSystemROI[i]).ref_ROIHeight);
                        }

                        m_smVisionInfo.g_arrSystemROI[i].AttachImage(m_smVisionInfo.g_arrImages[0]);
                        break;
                    case 1:
                        //Top
                        if (m_smVisionInfo.g_arrSystemROI.Count < 2)
                        {
                            objROI = new ROI("System Search ROI", 1);
                            m_smVisionInfo.g_arrSystemROI.Add(objROI);
                        }
                        m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(0, 0, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX + m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth,
                                                m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY);
                        break;
                    case 2:
                        //Left
                        if (m_smVisionInfo.g_arrSystemROI.Count < 3)
                        {
                            objROI = new ROI("System Search ROI", 1);
                            m_smVisionInfo.g_arrSystemROI.Add(objROI);
                        }
                        m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(0, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX,
                            m_smVisionInfo.g_intCameraResolutionHeight - m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY);
                        break;
                    case 3:
                        //Bottom
                        if (m_smVisionInfo.g_arrSystemROI.Count < 4)
                        {
                            objROI = new ROI("System Search ROI", 1);
                            m_smVisionInfo.g_arrSystemROI.Add(objROI);
                        }
                        m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY + m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight,
                            m_smVisionInfo.g_intCameraResolutionWidth - m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX,
                            m_smVisionInfo.g_intCameraResolutionHeight - (m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY + m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight));
                        break;
                    case 4:
                        //Right
                        if (m_smVisionInfo.g_arrSystemROI.Count < 5)
                        {
                            objROI = new ROI("System Search ROI", 1);
                            m_smVisionInfo.g_arrSystemROI.Add(objROI);
                        }
                        m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX + m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth, 0,
                            m_smVisionInfo.g_intCameraResolutionWidth - (m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX + m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth),
                            m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY + m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight);
                        break;
                }
            }
        }

        private void UpdateGUI()
        {
            //AddSystemSearchROI();

            int intViewImageCount = ImageDrawing.GetImageViewCount(m_smVisionInfo.g_intVisionIndex);

            for (int i = 0; i < intViewImageCount; i++)
            {
                int intImageIndex = ImageDrawing.GetArrayImageIndex(i, m_smVisionInfo.g_intVisionIndex);
                if (m_smVisionInfo.g_arrImageMaskingSetting.Count <= intImageIndex)
                {
                    for (int j = m_smVisionInfo.g_arrImageMaskingSetting.Count; j <= intImageIndex; j++)
                    {
                        m_smVisionInfo.g_arrImageMaskingSetting.Add(0);
                    }
                }
                if (m_smVisionInfo.g_arrImageMaskingGain.Count <= intImageIndex)
                {
                    for (int j = m_smVisionInfo.g_arrImageMaskingGain.Count; j <= intImageIndex; j++)
                    {
                        m_smVisionInfo.g_arrImageMaskingGain.Add(1f);
                    }
                }

                switch (intImageIndex)
                {
                    case 0:
                        cbo_ImageFilterSetting1.SelectedIndex = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex];
                        txt_Gain1.Value = Convert.ToDecimal(m_smVisionInfo.g_arrImageMaskingGain[intImageIndex]);
                        txt_Gain1.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        lbl_Gain1.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        pnl_ImageView1.Visible = true;
                        lbl_ImageView1.Text = lbl_ImageView1.Text.Substring(0, lbl_ImageView1.Text.Length - 2) + (i + 1).ToString() + ":";
                        break;
                    case 1:
                        cbo_ImageFilterSetting2.SelectedIndex = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex];
                        txt_Gain2.Value = Convert.ToDecimal(m_smVisionInfo.g_arrImageMaskingGain[intImageIndex]);
                        txt_Gain2.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        lbl_Gain2.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        pnl_ImageView2.Visible = true;
                        lbl_ImageView2.Text = lbl_ImageView2.Text.Substring(0, lbl_ImageView2.Text.Length - 2) + (i + 1).ToString() + ":";
                        break;
                    case 2:
                        cbo_ImageFilterSetting3.SelectedIndex = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex];
                        txt_Gain3.Value = Convert.ToDecimal(m_smVisionInfo.g_arrImageMaskingGain[intImageIndex]);
                        txt_Gain3.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        lbl_Gain3.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        pnl_ImageView3.Visible = true;
                        lbl_ImageView3.Text = lbl_ImageView3.Text.Substring(0, lbl_ImageView3.Text.Length - 2) + (i + 1).ToString() + ":";
                        break;
                    case 3:
                        cbo_ImageFilterSetting4.SelectedIndex = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex];
                        txt_Gain4.Value = Convert.ToDecimal(m_smVisionInfo.g_arrImageMaskingGain[intImageIndex]);
                        txt_Gain4.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        lbl_Gain4.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        pnl_ImageView4.Visible = true;
                        lbl_ImageView4.Text = lbl_ImageView4.Text.Substring(0, lbl_ImageView4.Text.Length - 2) + (i + 1).ToString() + ":";
                        break;
                    case 4:
                        cbo_ImageFilterSetting5.SelectedIndex = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex];
                        txt_Gain5.Value = Convert.ToDecimal(m_smVisionInfo.g_arrImageMaskingGain[intImageIndex]);
                        txt_Gain5.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        lbl_Gain5.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        pnl_ImageView5.Visible = true;
                        lbl_ImageView5.Text = lbl_ImageView5.Text.Substring(0, lbl_ImageView5.Text.Length - 2) + (i + 1).ToString() + ":";
                        break;
                    case 5:
                        cbo_ImageFilterSetting6.SelectedIndex = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex];
                        txt_Gain6.Value = Convert.ToDecimal(m_smVisionInfo.g_arrImageMaskingGain[intImageIndex]);
                        txt_Gain6.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        lbl_Gain6.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        pnl_ImageView6.Visible = true;
                        lbl_ImageView6.Text = lbl_ImageView6.Text.Substring(0, lbl_ImageView6.Text.Length - 2) + (i + 1).ToString() + ":";
                        break;
                    case 6:
                        cbo_ImageFilterSetting7.SelectedIndex = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex];
                        txt_Gain7.Value = Convert.ToDecimal(m_smVisionInfo.g_arrImageMaskingGain[intImageIndex]);
                        txt_Gain7.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        lbl_Gain7.Visible = m_smVisionInfo.g_arrImageMaskingSetting[intImageIndex] != 0;
                        pnl_ImageView7.Visible = true;
                        lbl_ImageView7.Text = lbl_ImageView7.Text.Substring(0, lbl_ImageView7.Text.Length - 2) + (i + 1).ToString() + ":";
                        break;
                }
            }

            for (int j = 0; j < m_smVisionInfo.g_arrImageMaskingSetting.Count; j++)
            {
                m_arrImageMaskingSetting_Prev.Add(m_smVisionInfo.g_arrImageMaskingSetting[j]);
            }

            for (int j = 0; j < m_smVisionInfo.g_arrImageMaskingGain.Count; j++)
            {
                m_arrImageMaskingGain_Prev.Add(m_smVisionInfo.g_arrImageMaskingGain[j]);
            }

            //radioBtn_ViewImage1.Checked = true;
            UncheckedAllRadioButton(m_smVisionInfo.g_intSelectedImage);
            //m_smVisionInfo.g_intSelectedImage = 0;    // Display whatever user select the image view no in production page.
            UpdatePackageImage();
        }

        private void UpdateROI()
        {
            for (int i = 1; i < 5; i++)
            {
                switch (i)
                {
                    case 1:
                        //Top
                        m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(0, 0, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX + m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth,
                            m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY);
                        break;
                    case 2:
                        //Left
                        m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(0, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX,
                            m_smVisionInfo.g_intCameraResolutionHeight - m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY);
                        break;
                    case 3:
                        //Bottom
                        m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX, m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY + m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight,
                            m_smVisionInfo.g_intCameraResolutionWidth - m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX,
                            m_smVisionInfo.g_intCameraResolutionHeight - (m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY + m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight));
                        break;
                    case 4:
                        //Right
                        m_smVisionInfo.g_arrSystemROI[i].LoadROISetting(m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX + m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth, 0,
                            m_smVisionInfo.g_intCameraResolutionWidth - (m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX + m_smVisionInfo.g_arrSystemROI[0].ref_ROIWidth),
                            m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY + m_smVisionInfo.g_arrSystemROI[0].ref_ROIHeight);
                        break;
                }
            }
        }

        private void SystemForm_Load(object sender, EventArgs e)
        {
            btn_Live.Text = "Stop Live";
            m_smVisionInfo.AT_PR_PauseLiveImage = false;
            m_smVisionInfo.AT_PR_StartLiveImage = true;
            Cursor.Current = Cursors.Default;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            m_smVisionInfo.g_intViewUniformize = false;
            Thread.Sleep(5);
            m_smVisionInfo.AT_PR_GrabImage = true;
            Thread.Sleep(5);
            m_smVisionInfo.AT_PR_GrabImage = false;
            m_smVisionInfo.g_intLearnStepNo = 0;
            SetupSteps(true);
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void SystemForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Set Front View Form Closed", "Exit Set Front View Form", "", "", m_smProductionInfo.g_strLotID);
            m_smVisionInfo.AT_PR_StartLiveImage = false;
            m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnSeparateGrab = false;
            m_smVisionInfo.g_intViewUniformize = true;
            m_smVisionInfo.AT_PR_GrabImage = true;
            Thread.Sleep(5);
            m_smVisionInfo.AT_PR_GrabImage = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            UpdateROI();


            STDeviceEdit.CopySettingFile(m_strFilePath, "System\\ROI.xml");

            ROI.SaveFile(m_strFilePath + "System\\ROI.xml", m_smVisionInfo.g_arrSystemROI);

            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">System ROI", m_smProductionInfo.g_strLotID);


            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            XmlParser objFile = new XmlParser(m_strFilePath + "System\\Settings.xml");
            objFile.WriteSectionElement("ImageMaskingSetting");
            for (int i = 0; i < m_smVisionInfo.g_arrImageMaskingSetting.Count; i++)
            {
                objFile.WriteElement1Value("ImageMaskingSetting" + i.ToString(), m_smVisionInfo.g_arrImageMaskingSetting[i]);
            }
            objFile.WriteSectionElement("ImageMaskingGain");
            for (int i = 0; i < m_smVisionInfo.g_arrImageMaskingGain.Count; i++)
            {
                objFile.WriteElement1Value("ImageMaskingGain" + i.ToString(), m_smVisionInfo.g_arrImageMaskingGain[i]);
            }

            objFile.WriteSectionElement("ImageMaskingThreshold");
            objFile.WriteElement1Value("ImageMaskingThresholdValue", m_smVisionInfo.g_intImageMaskingThreshold);
            objFile.WriteElement1Value("WhiteBackgroundImageIndex", m_smVisionInfo.g_intWhiteBackgroundImageIndex);

            objFile.WriteEndElement();


            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                if (m_smVisionInfo.g_arrImageMaskingSetting.Count <= i)
                {
                    m_smVisionInfo.g_arrImageMaskingSetting.Add(0);
                }

                if (m_smVisionInfo.g_arrImageMaskingGain.Count <= i)
                {
                    m_smVisionInfo.g_arrImageMaskingGain.Add(1f);
                }

                if (m_smVisionInfo.g_arrImageMaskingAvailable.Count <= i)
                {
                    m_smVisionInfo.g_arrImageMaskingAvailable.Add(false);
                }
                else
                {
                    m_smVisionInfo.g_arrImageMaskingAvailable[i] = false;
                }

                if (m_smVisionInfo.g_arrReferenceImages.Count <= i)
                    m_smVisionInfo.g_arrReferenceImages.Add(new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));

                if (m_smVisionInfo.g_arrInvertedReferenceImages.Count <= i)
                    m_smVisionInfo.g_arrInvertedReferenceImages.Add(new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));

                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_arrReferenceColorImages.Count <= i)
                        m_smVisionInfo.g_arrReferenceColorImages.Add(new CImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));

                    if (m_smVisionInfo.g_arrInvertedReferenceColorImages.Count <= i)
                        m_smVisionInfo.g_arrInvertedReferenceColorImages.Add(new CImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                }

                if (m_smVisionInfo.g_arrImageMaskingSetting[i] != 0)
                {
                    m_smVisionInfo.g_arrReferenceImages[i].AddGain(ref m_smVisionInfo.g_arrReferenceImages, i, m_smVisionInfo.g_arrImageMaskingGain[i]);
                }
                //if (m_smVisionInfo.g_arrImageMaskingSetting[i] != 0)
                //{
                //    m_smVisionInfo.g_arrImages[i].AddGain(ref m_smVisionInfo.g_arrReferenceImages, i, m_smVisionInfo.g_arrImageMaskingGain[i]);
                //}
                //else
                //{
                //    m_smVisionInfo.g_arrImages[i].CopyTo(ref m_smVisionInfo.g_arrReferenceImages, i);
                //}

                //if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                //{
                //    m_smVisionInfo.g_arrColorImages[i].CopyTo(ref m_smVisionInfo.g_arrReferenceColorImages, i);
                //}

                //int intImageIndex = ImageDrawing.GetArrayImageIndex(1, m_smVisionInfo.g_intVisionIndex);

                //m_smVisionInfo.g_arrReferenceImages[i].SaveImage(m_strFilePath + "System\\ReferenceImages" + i + ".bmp");

                //ImageDrawing.InvertImage(m_smVisionInfo.g_arrReferenceImages[i], m_smVisionInfo.g_arrInvertedReferenceImages[i]);

                //m_smVisionInfo.g_arrInvertedReferenceImages[i].SaveImage(m_strFilePath + "System\\InvertedReferenceImages" + i + ".bmp");
            }

            DeleteReferenceImageFiles(false);
            ConvertTempImagesToReferenceImages();

            Close();
            Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            bool blnStartLive = m_smVisionInfo.AT_PR_StartLiveImage;
            bool blnTriggerLive = m_smVisionInfo.AT_PR_TriggerLiveImage;
            m_smVisionInfo.AT_PR_StartLiveImage = false;
            m_smVisionInfo.AT_PR_TriggerLiveImage = true;

            // 2019-09-13 ZJYEOH : Need to stop grab image as g_arrSystemROI will be cleared during load file
            ROI.LoadFile(m_strFilePath + "System\\ROI.xml", m_smVisionInfo.g_arrSystemROI);

            m_smVisionInfo.AT_PR_StartLiveImage = blnStartLive;
            m_smVisionInfo.AT_PR_TriggerLiveImage = blnTriggerLive;

            for (int j = 0; j < m_smVisionInfo.g_arrImageMaskingSetting.Count; j++)
            {
                m_smVisionInfo.g_arrImageMaskingSetting[j] = m_arrImageMaskingSetting_Prev[j];
            }

            for (int j = 0; j < m_smVisionInfo.g_arrImageMaskingGain.Count; j++)
            {
                m_smVisionInfo.g_arrImageMaskingGain[j] = m_arrImageMaskingGain_Prev[j];
            }

            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                if (m_smVisionInfo.g_arrImageMaskingSetting.Count <= i)
                {
                    m_smVisionInfo.g_arrImageMaskingSetting.Add(0);
                }

                if (m_smVisionInfo.g_arrImageMaskingGain.Count <= i)
                {
                    m_smVisionInfo.g_arrImageMaskingGain.Add(1f);
                }

                //if (m_smVisionInfo.g_arrImageMaskingAvailable.Count <= i)
                //{
                //    m_smVisionInfo.g_arrImageMaskingAvailable.Add(false);
                //}
                //else
                //{
                //    m_smVisionInfo.g_arrImageMaskingAvailable[i] = false;
                //}

                if (m_smVisionInfo.g_arrReferenceImages.Count <= i)
                    m_smVisionInfo.g_arrReferenceImages.Add(new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                if (m_smVisionInfo.g_arrInvertedReferenceImages.Count <= i)
                    m_smVisionInfo.g_arrInvertedReferenceImages.Add(new ImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));

                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if (m_smVisionInfo.g_arrReferenceColorImages.Count <= i)
                        m_smVisionInfo.g_arrReferenceColorImages.Add(new CImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));
                    if (m_smVisionInfo.g_arrInvertedReferenceColorImages.Count <= i)
                        m_smVisionInfo.g_arrInvertedReferenceColorImages.Add(new CImageDrawing(true, m_smVisionInfo.g_intCameraResolutionWidth, m_smVisionInfo.g_intCameraResolutionHeight));

                }

                if (File.Exists(m_strFilePath + "System\\ReferenceImages" + i + ".bmp"))
                {
                    m_smVisionInfo.g_arrReferenceImages[i].LoadImage(m_strFilePath + "System\\ReferenceImages" + i + ".bmp");
                    m_smVisionInfo.g_arrImageMaskingAvailable[i] = true;
                }

                if (File.Exists(m_strFilePath + "System\\InvertedReferenceImages" + i + ".bmp"))
                {
                    m_smVisionInfo.g_arrInvertedReferenceImages[i].LoadImage(m_strFilePath + "System\\InvertedReferenceImages" + i + ".bmp");
                }

                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    if (File.Exists(m_strFilePath + "System\\InvertedReferenceColorImages" + i + ".bmp"))
                    {
                        m_smVisionInfo.g_arrInvertedReferenceColorImages[i].LoadImage(m_strFilePath + "System\\InvertedReferenceColorImages" + i + ".bmp");
                    }
                }
            }

            DeleteReferenceImageFiles(true);

            Close();
            Dispose();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_smProductionInfo.AT_ALL_InAuto && m_smProductionInfo.g_intAutoLogOutMinutes > 0 && m_intUserGroup != 5)
            {
                DateTime t = DateTime.Now;
                TimeSpan tSpan = t - m_smProductionInfo.g_DTStartAutoMode_IndividualForm;

                if (tSpan.Minutes >= m_smProductionInfo.g_intAutoLogOutMinutes)
                {
                    m_smProductionInfo.g_intUserGroup = 5;
                    m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
                    DisableField2();
                }
            }

            if (m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX < 0)
                m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionX = 0;
            if (m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY < 0)
                m_smVisionInfo.g_arrSystemROI[0].ref_ROIPositionY = 0;
            UpdateROI();
        }
        private void SetupSteps(bool blnForward)
        {
            bool isUseColorCamera = (m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0;
            int intLearnStepNo = m_smVisionInfo.g_intLearnStepNo;

            if (isUseColorCamera && intLearnStepNo == 1)
            {
                if (blnForward)
                    intLearnStepNo++;
                else
                    intLearnStepNo--;
            }
                

            switch (intLearnStepNo)
            {
                case 0: //Define System ROI
                    AddSystemSearchROI();

                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;

                    lbl_StepNo.BringToFront();
                    lbl_Step1.BringToFront();
                    tabCtrl_SystemROI.SelectedTab = tp_1;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case 1: //Threshold (Only mono camera got this step)
                    UpdateReferenceImageList(true, false);
                    SaveReferenceImagesToFile(true);

                    //btn_Live.Text = "Live";
                    //m_smVisionInfo.AT_PR_StartLiveImage = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = true;

                    cbo_WhiteBackgroundImage.Items.Clear();

                    for (int i = 0; i < m_smVisionInfo.g_intImageViewCount; i++)
                        cbo_WhiteBackgroundImage.Items.Add("Image " + (i + 1).ToString());

                    if (m_smVisionInfo.g_intWhiteBackgroundImageIndex < cbo_WhiteBackgroundImage.Items.Count)
                        cbo_WhiteBackgroundImage.SelectedIndex = m_smVisionInfo.g_intWhiteBackgroundImageIndex;
                    else
                        cbo_WhiteBackgroundImage.SelectedIndex = 0;

                    m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_WhiteBackgroundImage.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                    UncheckedAllRadioButton(m_smVisionInfo.g_intSelectedImage);

                    lbl_StepNo.BringToFront();
                    lbl_Step2.BringToFront();
                    tabCtrl_SystemROI.SelectedTab = tp_Threshold;
                    btn_Next.Enabled = true;
                    btn_Previous.Enabled = true;
                    UpdatePackageImage();
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                case 2: //Define Image Filter Setting
                    UpdateReferenceImageList(false, isUseColorCamera);

                    // check if it is a color vision, if yes, then we will save current image to a temp folder 
                    if (isUseColorCamera)
                        SaveReferenceImagesToFile(true);

                    btn_Live.Text = "Live";
                    m_smVisionInfo.AT_PR_StartLiveImage = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = true;

                    lbl_StepNo.BringToFront();
                    lbl_Step2.BringToFront();
                    tabCtrl_SystemROI.SelectedTab = tp_2;
                    btn_Next.Enabled = false;
                    btn_Previous.Enabled = true;

                    UpdatePackageImage();
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    break;
                default:
                    break;
            }
        }

        private void UpdateReferenceImageList(
            bool isUpdateMonoImages, 
            bool isUpdateColorImages
            )
        {
            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                if (isUpdateMonoImages)
                    m_smVisionInfo.g_arrImages[i].CopyTo(ref m_smVisionInfo.g_arrReferenceImages, i);

                if (isUpdateColorImages)
                {
                    m_smVisionInfo.g_arrColorImages[i].CopyTo(ref m_smVisionInfo.g_arrReferenceColorImages, i);
                    STTrackLog.WriteLine("Update Color Uniormize Ref. Image from g_arrColorImages[" + i + "]");
                    STTrackLog.WriteLine(Environment.StackTrace.ToString());
                }
            }
        }

        private void SaveReferenceImagesToFile(bool isTempFile)
        {
            DeleteReferenceImageFiles(isTempFile);

            // Save current image as reference image
            string strReferenceImagePath = "";
            string strInvertedReferenceImagePath = "";
            string strReferenceColorImagePath = "";
            string strTempPath = (isTempFile) ? "Temp_" : "";
            int intViewImageCount = ImageDrawing.GetImageViewCount(m_smVisionInfo.g_intVisionIndex);

            for (int i = 0; i < intViewImageCount; i++)
            {
                int intImageIndex = ImageDrawing.GetArrayImageIndex(i, m_smVisionInfo.g_intVisionIndex);
                int intSelectedIndex = 0;

                switch (intImageIndex)
                {
                    case 0: intSelectedIndex = cbo_ImageFilterSetting1.SelectedIndex; break;
                    case 1: intSelectedIndex = cbo_ImageFilterSetting2.SelectedIndex; break;
                    case 2: intSelectedIndex = cbo_ImageFilterSetting3.SelectedIndex; break;
                    case 3: intSelectedIndex = cbo_ImageFilterSetting4.SelectedIndex; break;
                    case 4: intSelectedIndex = cbo_ImageFilterSetting5.SelectedIndex; break;
                    case 5: intSelectedIndex = cbo_ImageFilterSetting6.SelectedIndex; break;
                    case 7: intSelectedIndex = cbo_ImageFilterSetting7.SelectedIndex; break;
                }

                strReferenceImagePath = m_strFilePath
                    + "System\\"
                    + strTempPath
                    + "ReferenceImages"
                    + intImageIndex
                    + ".bmp";
                strInvertedReferenceImagePath = m_strFilePath
                    + "System\\"
                    + strTempPath
                    + "InvertedReferenceImages"
                    + intImageIndex
                    + ".bmp";
                strReferenceColorImagePath = m_strFilePath
                    + "System\\"
                    + strTempPath
                    + "ReferenceColorImages"
                    + intImageIndex
                    + ".bmp";

                // Reference + Inverted
                if (intSelectedIndex == 1)
                {
                    m_smVisionInfo.g_arrReferenceImages[intImageIndex].SaveImage(strReferenceImagePath);

                    ImageDrawing.InvertImage(m_smVisionInfo.g_arrReferenceImages[intImageIndex], m_smVisionInfo.g_arrInvertedReferenceImages[intImageIndex]);
                    m_smVisionInfo.g_arrInvertedReferenceImages[intImageIndex].SaveImage(strInvertedReferenceImagePath);
                }
                // Reference 
                else if (intSelectedIndex == 2)
                    m_smVisionInfo.g_arrReferenceImages[intImageIndex].SaveImage(strReferenceImagePath);

                // Color Reference 
                if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                {
                    // Reference (Color doesn't have inverted reference)
                    if (intSelectedIndex == 2)
                    {
                        m_smVisionInfo.g_arrReferenceColorImages[intImageIndex].SaveImage(strReferenceColorImagePath);
                        STTrackLog.WriteLine("Save Color Uniormize Ref. Image: " + strReferenceColorImagePath);
                    }
                }
            }
        }

        private void DeleteReferenceImageFiles(bool isTempFile)
        {
            string strReferenceImagePath = "";
            string strInvertedReferenceImagePath = "";
            string strReferenceColorImagePath = "";
            string strTempPath = (isTempFile) ? "Temp_" : "";

            // Delete ALL reference images if any
            for (int i = 0; i < 10; i++)
            {
                strReferenceImagePath = m_strFilePath
                    + "System\\"
                    + strTempPath
                    + "ReferenceImages"
                    + i
                    + ".bmp";
                strInvertedReferenceImagePath = m_strFilePath
                    + "System\\"
                    + strTempPath
                    + "InvertedReferenceImages"
                    + i
                    + ".bmp";
                strReferenceColorImagePath = m_strFilePath
                    + "System\\"
                    + strTempPath
                    + "ReferenceColorImages"
                    + i
                    + ".bmp";

                if (File.Exists(strReferenceImagePath))
                    File.Delete(strReferenceImagePath);

                if (File.Exists(strInvertedReferenceImagePath))
                    File.Delete(strInvertedReferenceImagePath);

                if (File.Exists(strReferenceColorImagePath))
                    File.Delete(strReferenceColorImagePath);
            }
        }
        
        private void ConvertTempImagesToReferenceImages()
        {

            string strRefImagePath_Temp = "";
            string strInvertedRefImagePath_Temp = "";
            string strRefColorImagePath_Temp = "";
            string strTempPath = "Temp_";

            // Delete ALL reference images if any
            for (int i = 0; i < 10; i++)
            {
                strRefImagePath_Temp = m_strFilePath
                    + "System\\Temp_ReferenceImages" 
                    + i
                    + ".bmp";
                strInvertedRefImagePath_Temp = m_strFilePath 
                    + "System\\Temp_InvertedReferenceImages" 
                    + i 
                    + ".bmp";
                strRefColorImagePath_Temp = m_strFilePath 
                    + "System\\Temp_ReferenceColorImages"
                    + i
                    + ".bmp";

                if (File.Exists(strRefImagePath_Temp))
                    File.Move(strRefImagePath_Temp, strRefImagePath_Temp.Replace(strTempPath, ""));

                if (File.Exists(strInvertedRefImagePath_Temp))
                    File.Move(strInvertedRefImagePath_Temp, strInvertedRefImagePath_Temp.Replace(strTempPath, ""));

                if (File.Exists(strRefColorImagePath_Temp))
                    File.Move(strRefColorImagePath_Temp, strRefColorImagePath_Temp.Replace(strTempPath, ""));
            }
        }

        private void btn_Previous_Click(object sender, EventArgs e)
        {

            if (m_smVisionInfo.g_intLearnStepNo > 0)
                m_smVisionInfo.g_intLearnStepNo--;

            m_intDisplayStepNo--;

            SetupSteps(false);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intLearnStepNo < 99)
                m_smVisionInfo.g_intLearnStepNo++;

            m_intDisplayStepNo++;

            SetupSteps(true);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Live_Click(object sender, EventArgs e)
        {
            bool blnLive = false;
            if (btn_Live.Text == "Live")
            {
                btn_Live.Text = "Stop Live";
                blnLive = true;
            }
            else
            {
                btn_Live.Text = "Live";
                blnLive = false;
            }
            //m_smVisionInfo.AT_PR_PauseLiveImage = !blnLive;
            m_smVisionInfo.AT_PR_StartLiveImage = blnLive;
        }

        private void cbo_ImageFilterSetting_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (Convert.ToInt32(((ComboBox)sender).Tag))
            {
                case 0:
                    m_smVisionInfo.g_arrImageMaskingSetting[0] = cbo_ImageFilterSetting1.SelectedIndex;
                    txt_Gain1.Visible = m_smVisionInfo.g_arrImageMaskingSetting[0] != 0;
                    lbl_Gain1.Visible = m_smVisionInfo.g_arrImageMaskingSetting[0] != 0;
                    break;
                case 1:
                    m_smVisionInfo.g_arrImageMaskingSetting[1] = cbo_ImageFilterSetting2.SelectedIndex;
                    txt_Gain2.Visible = m_smVisionInfo.g_arrImageMaskingSetting[1] != 0;
                    lbl_Gain2.Visible = m_smVisionInfo.g_arrImageMaskingSetting[1] != 0;
                    break;
                case 2:
                    m_smVisionInfo.g_arrImageMaskingSetting[2] = cbo_ImageFilterSetting3.SelectedIndex;
                    txt_Gain3.Visible = m_smVisionInfo.g_arrImageMaskingSetting[2] != 0;
                    lbl_Gain3.Visible = m_smVisionInfo.g_arrImageMaskingSetting[2] != 0;
                    break;
                case 3:
                    m_smVisionInfo.g_arrImageMaskingSetting[3] = cbo_ImageFilterSetting4.SelectedIndex;
                    txt_Gain4.Visible = m_smVisionInfo.g_arrImageMaskingSetting[3] != 0;
                    lbl_Gain4.Visible = m_smVisionInfo.g_arrImageMaskingSetting[3] != 0;
                    break;
                case 4:
                    m_smVisionInfo.g_arrImageMaskingSetting[4] = cbo_ImageFilterSetting5.SelectedIndex;
                    txt_Gain5.Visible = m_smVisionInfo.g_arrImageMaskingSetting[4] != 0;
                    lbl_Gain5.Visible = m_smVisionInfo.g_arrImageMaskingSetting[4] != 0;
                    break;
                case 5:
                    m_smVisionInfo.g_arrImageMaskingSetting[5] = cbo_ImageFilterSetting6.SelectedIndex;
                    txt_Gain6.Visible = m_smVisionInfo.g_arrImageMaskingSetting[5] != 0;
                    lbl_Gain6.Visible = m_smVisionInfo.g_arrImageMaskingSetting[5] != 0;
                    break;
                case 6:
                    m_smVisionInfo.g_arrImageMaskingSetting[6] = cbo_ImageFilterSetting7.SelectedIndex;
                    txt_Gain7.Visible = m_smVisionInfo.g_arrImageMaskingSetting[6] != 0;
                    lbl_Gain7.Visible = m_smVisionInfo.g_arrImageMaskingSetting[6] != 0;
                    break;
            }
            UpdatePackageImage();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Gain_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (Convert.ToInt32(((NumericUpDown)sender).Tag))
            {
                case 0:
                    m_smVisionInfo.g_arrImageMaskingGain[0] = (float)Convert.ToDecimal(txt_Gain1.Value);
                    break;
                case 1:
                    m_smVisionInfo.g_arrImageMaskingGain[1] = (float)Convert.ToDecimal(txt_Gain2.Value);
                    break;
                case 2:
                    m_smVisionInfo.g_arrImageMaskingGain[2] = (float)Convert.ToDecimal(txt_Gain3.Value);
                    break;
                case 3:
                    m_smVisionInfo.g_arrImageMaskingGain[3] = (float)Convert.ToDecimal(txt_Gain4.Value);
                    break;
                case 4:
                    m_smVisionInfo.g_arrImageMaskingGain[4] = (float)Convert.ToDecimal(txt_Gain5.Value);
                    break;
                case 5:
                    m_smVisionInfo.g_arrImageMaskingGain[5] = (float)Convert.ToDecimal(txt_Gain6.Value);
                    break;
                case 6:
                    m_smVisionInfo.g_arrImageMaskingGain[6] = (float)Convert.ToDecimal(txt_Gain7.Value);
                    break;
            }
            UpdatePackageImage();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void radioBtn_View_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedImage = Convert.ToInt32(((RadioButton)sender).Tag);
            UncheckedAllRadioButton(m_smVisionInfo.g_intSelectedImage);
            UpdatePackageImage();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdatePackageImage()
        {
            if (m_smVisionInfo.g_arrImageMaskingSetting[m_smVisionInfo.g_intSelectedImage] != 0)
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrImageMaskingGain[m_smVisionInfo.g_intSelectedImage]);
            }
            else
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_objPackageImage);
            }
        }

        private void UncheckedAllRadioButton(int intIgnoreIndex)
        {
            if (intIgnoreIndex != 0)
                radioBtn_ViewImage1.Checked = false;
            else
                radioBtn_ViewImage1.Checked = true;

            if (intIgnoreIndex != 1)
                radioBtn_ViewImage2.Checked = false;
            else
                radioBtn_ViewImage2.Checked = true;

            if (intIgnoreIndex != 2)
                radioBtn_ViewImage3.Checked = false;
            else
                radioBtn_ViewImage3.Checked = true;

            if (intIgnoreIndex != 3)
                radioBtn_ViewImage4.Checked = false;
            else
                radioBtn_ViewImage4.Checked = true;

            if (intIgnoreIndex != 4)
                radioBtn_ViewImage5.Checked = false;
            else
                radioBtn_ViewImage5.Checked = true;

            if (intIgnoreIndex != 5)
                radioBtn_ViewImage6.Checked = false;
            else
                radioBtn_ViewImage6.Checked = true;

            if (intIgnoreIndex != 6)
                radioBtn_ViewImage7.Checked = false;
            else
                radioBtn_ViewImage7.Checked = true;
        }

        private void btn_LiveOriginal_Click(object sender, EventArgs e)
        {
            bool blnLive = false;
            if (btn_LiveOriginal.Text == "Live")
            {
                btn_LiveOriginal.Text = "Stop Live";
                blnLive = true;

                btn_LiveFiltered.Text = "Live";
            }
            else
            {
                btn_LiveOriginal.Text = "Live";
                blnLive = false;
            }
            m_smVisionInfo.g_intViewUniformize = false;
            m_smVisionInfo.AT_PR_StartLiveImage = blnLive;
        }

        private void btn_LiveFiltered_Click(object sender, EventArgs e)
        {
            bool blnLive = false;
            if (btn_LiveFiltered.Text == "Live")
            {
                btn_LiveFiltered.Text = "Stop Live";
                blnLive = true;

                btn_LiveOriginal.Text = "Live";
            }
            else
            {
                btn_LiveFiltered.Text = "Live";
                blnLive = false;
            }
            m_smVisionInfo.g_intViewUniformize = true;
            m_smVisionInfo.AT_PR_StartLiveImage = blnLive;
        }

        private void btn_Threshold_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_intImageMaskingThreshold;

            ROI objROI = new ROI();
            objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            objROI.LoadROISetting(0, 0, 
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth,
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageHeight);
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, objROI, false);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                m_smVisionInfo.g_intImageMaskingThreshold = m_smVisionInfo.g_intThresholdValue;
            }
            else
            {
                m_smVisionInfo.g_intImageMaskingThreshold = m_smVisionInfo.g_intImageMaskingThreshold;
            }
                
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_WhiteBackgroundImage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_WhiteBackgroundImage.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
            UncheckedAllRadioButton(m_smVisionInfo.g_intSelectedImage);

            m_smVisionInfo.g_intWhiteBackgroundImageIndex = cbo_WhiteBackgroundImage.SelectedIndex;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_SaveFrontROI_Click(object sender, EventArgs e)
        {
            UpdateROI();


            STDeviceEdit.CopySettingFile(m_strFilePath, "System\\ROI.xml");

            ROI.SaveFile(m_strFilePath + "System\\ROI.xml", m_smVisionInfo.g_arrSystemROI);

            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">System ROI", m_smProductionInfo.g_strLotID);

            SaveReferenceImagesToFile(false);

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            Close();
            Dispose();
        }
    }
}