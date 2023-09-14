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
using Microsoft.Win32;
using System.Threading;

namespace VisionProcessForm
{
    public partial class LearnBarcodeForm : Form
    {
        #region Member Variables
        private bool m_blnPreTestDone = false;
        private ImageDrawing m_objImage_Temp = new ImageDrawing(true);
        private Point[] arrROIStart = new Point[10];
        private Size[] arrROISize = new Size[10];
        private bool m_blnPatternMatched = false;
        private int m_intSelectedTemplateIndex = 0;
        private int m_intVisionType = 0;
        private bool m_blnPatternMatchResult = false;
        private int m_intCurrentPreciseDeg = 0;
        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private int m_intDisplayStepNo = 1;
        private string m_strSelectedRecipe;
        private string m_strPosition = "";
        private List<ImageDrawing> m_arrCurrentImage = new List<ImageDrawing>();
        private ROI PatternSearchROI = new ROI();

        private VisionInfo m_smVisionInfo;
        private CustomOption m_smCustomizeInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        private SRMWaitingFormThread m_thWaitingFormThread;
        #endregion

        public LearnBarcodeForm(VisionInfo visionInfo, CustomOption smCustomizeInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intVisionType)
        {
            InitializeComponent();

            m_intVisionType = intVisionType;
            m_smVisionInfo = visionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            
            m_strSelectedRecipe = strSelectedRecipe;
            m_intUserGroup = intUserGroup;
            DisableField2();
            UpdateGUI();

            if (m_intVisionType == 1)
            {
                m_smVisionInfo.g_intLearnStepNo = 3;
                Thread.Sleep(5);
                m_smVisionInfo.AT_PR_GrabImage = true;
                Thread.Sleep(5);
                m_smVisionInfo.AT_PR_GrabImage = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
            else
                m_smVisionInfo.g_intLearnStepNo = 0;

            m_blnInitDone = true;
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Barcode";
            string strChild2 = "Learn Page";
            string strChild3 = "Save Button";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(strChild1, strChild2, strChild3))
            {
                btn_Save.Enabled = false;
            }

            strChild3 = "Detect Button";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(strChild1, strChild2, strChild3))
            {
                btn_Detect.Enabled = false;
            }

            strChild3 = "Dont Care Scale Setting";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(strChild1, strChild2, strChild3))
            {
                txt_BarcodeDontCareScale.Enabled = false;
            }

            strChild3 = "Gain Range Setting";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(strChild1, strChild2, strChild3))
            {
                txt_GainRange.Enabled = false;
                chk_WantUseGainRange.Enabled = false;
            }

            strChild3 = "Angle Range Setting";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(strChild1, strChild2, strChild3))
            {
                txt_BarcodeAngleRange.Enabled = false;
                chk_WantUseAngleRange.Enabled = false;
            }

            strChild3 = "Add Button";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(strChild1, strChild2, strChild3))
            {
                btn_Add.Enabled = false;
            }

            strChild3 = "Delete Button";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(strChild1, strChild2, strChild3))
            {
                btn_Delete.Enabled = false;
            }

            strChild2 = "Learn Reference Image Page";
            strChild3 = "Save Button";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(strChild1, strChild2, strChild3))
            {
                btn_SaveRefenceImage.Enabled = false;
            }

            strChild3 = "Live Button";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(strChild1, strChild2, strChild3))
            {
                btn_Live.Enabled = false;
            }

            strChild3 = "Gain Setting";
            if (m_smProductionInfo.g_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(strChild1, strChild2, strChild3))
            {
                gb_GainSetting.Enabled = false;
            }

        }

        private void UpdateGUI()
        {
            txt_BarcodeDontCareScale.Value = Convert.ToInt32(m_smVisionInfo.g_objBarcode.ref_fDontCareScale * 100);
            txt_GainRange.Value = Convert.ToDecimal(m_smVisionInfo.g_objBarcode.ref_fGainRangeTolerance);
            txt_BarcodeAngleRange.Value = Convert.ToDecimal(m_smVisionInfo.g_objBarcode.ref_fBarcodeAngleRangeTolerance);

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                  m_smVisionInfo.g_strVisionFolderName + "\\Barcode\\Settings.xml";

            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.GetFirstSection("Advanced");
            chk_WantUseAngleRange.Checked = objFileHandle.GetValueAsBoolean("WantUseAngleRange", false);
            chk_WantUseGainRange.Checked = objFileHandle.GetValueAsBoolean("WantUseGainRange", false);
            txt_Gain.Text = m_smVisionInfo.g_objBarcode.ref_fImageGain.ToString();
            trackBar_Gain.Value = (int)(m_smVisionInfo.g_objBarcode.ref_fImageGain * 10);
            cbo_CodeType.SelectedIndex = m_smVisionInfo.g_objBarcode.ref_intCodeType;

            if (m_smVisionInfo.g_objBarcode.ref_intCodeType == 0)
                gb_Barcode.Size = new Size(gb_Barcode.Width, 90);
            else
                gb_Barcode.Size = new Size(gb_Barcode.Width, 60);

            UpdateTemlateTable();
        }

        private void UpdateTemlateTable()
        {
            dgd_Template.Rows.Clear();
            for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
            {
                dgd_Template.Rows.Add();
                dgd_Template.Rows[i].Cells[0].Value = (i + 1).ToString();
                dgd_Template.Rows[i].Cells[1].Value = m_smVisionInfo.g_objBarcode.ref_strTemplateCode[i];
                dgd_Template.Rows[i].Cells[1].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                dgd_Template.Rows[i].Cells[1].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                dgd_Template.Rows[i].Cells[2].Value = m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i];
                dgd_Template.Rows[i].Cells[2].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                dgd_Template.Rows[i].Cells[2].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
            }

            if (dgd_Template.RowCount == 0)
            {
                dgd_Template.Rows.Add();
                dgd_Template.Rows[0].Cells[0].Value = (1).ToString();
                dgd_Template.Rows[0].Cells[1].Value = "----";
                m_smVisionInfo.g_objBarcode.ref_intTemplateCount = 1;
                dgd_Template.Rows[0].Cells[1].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[0];
                dgd_Template.Rows[0].Cells[1].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[0];
                dgd_Template.Rows[0].Cells[2].Value = "----";
                dgd_Template.Rows[0].Cells[2].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[0];
                dgd_Template.Rows[0].Cells[2].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[0];
            }

            m_intSelectedTemplateIndex = 0;
            dgd_Template.Rows[0].Selected = true;
            dgd_Template.CurrentCell = dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[0];
        }

        private void LearnBarcodeForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.AT_PR_PauseLiveImage = false;
            m_smVisionInfo.AT_PR_StartLiveImage = false;
            m_smVisionInfo.g_intSelectedImage = 0;
            m_smVisionInfo.g_blnDrawBarcodeResult = false;
            m_smVisionInfo.g_blnViewBarcodeInspection = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            btn_Save.Enabled = false;
            Cursor.Current = Cursors.Default;
            //m_smVisionInfo.g_intLearnStepNo = 0;
            SetupSteps(true);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
        }

        private void LearnBarcodeForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_objImage_Temp.Dispose();
            //m_smVisionInfo.AT_PR_PauseLiveImage = true;
            m_smVisionInfo.g_objBarcode.ResetBarcodeObject();
            m_smVisionInfo.AT_PR_StartLiveImage = false;
            m_smVisionInfo.g_blnDrawBarcodeResult = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.g_intLearnStepNo = 0;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnViewGauge = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnDragROI = false;
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {

            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            //m_smVisionInfo.g_intSelectedImage = 0;

            SaveBarcodeSetting();

            ROI.SaveFile(strFolderPath + "Barcode\\ROI.xml", m_smVisionInfo.g_arrBarcodeROIs);

            LoadBarcodeSetting();

            m_smVisionInfo.AT_PR_AttachImagetoROI = true;

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }
        private void SaveBarcodeSetting()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                               m_smVisionInfo.g_strVisionFolderName + "\\Barcode\\";
            
            STDeviceEdit.CopySettingFile(strPath + "\\Settings.xml", "");
            XmlParser objFileHandle = new XmlParser(strPath + "\\Settings.xml");
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WantUseAngleRange", chk_WantUseAngleRange.Checked);
            objFileHandle.WriteElement1Value("WantUseGainRange", chk_WantUseGainRange.Checked);
            objFileHandle.WriteEndElement();
            
            m_smVisionInfo.g_objBarcode.ref_blnWantUseAngleRange = chk_WantUseAngleRange.Checked;
            m_smVisionInfo.g_objBarcode.ref_blnWantUseGainRange = chk_WantUseGainRange.Checked;


            //m_smVisionInfo.g_arrBarcodeROIs[0].SaveImage(strPath + "Template\\OriTemplate.bmp");
            m_smVisionInfo.g_arrImages[0].SaveImage(strPath + "Template\\OriTemplate.bmp");
            // Save Pocket Reference Pattern
            m_smVisionInfo.g_arrBarcodeROIs[1].SaveImage(strPath + "Template\\Template0.bmp");
            m_smVisionInfo.g_objBarcode.LearnPattern(m_smVisionInfo.g_arrBarcodeROIs[1]);
            m_smVisionInfo.g_objBarcode.SavePattern(strPath + "\\Template\\Template0.mch");

            //m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX = m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterX - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeCenterX;
            //m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY = m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterY - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeCenterY;

            m_smVisionInfo.g_objBarcode.SaveBarcode(strPath + "Settings.xml", false, "Settings", true);
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Learn Barcode", m_smProductionInfo.g_strLotID);
            

        }
        private void LoadBarcodeSetting()
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                    m_smVisionInfo.g_strVisionFolderName + "\\Barcode\\";

            XmlParser objFileHandle = new XmlParser(strPath + "\\Settings.xml");
            objFileHandle.GetFirstSection("Advanced");
            m_smVisionInfo.g_objBarcode.ref_blnWantUseAngleRange = objFileHandle.GetValueAsBoolean("WantUseAngleRange", false);
            m_smVisionInfo.g_objBarcode.ref_blnWantUseGainRange = objFileHandle.GetValueAsBoolean("WantUseGainRange", false);
            m_smVisionInfo.g_objBarcode.ref_blnWantUseReferenceImage = objFileHandle.GetValueAsBoolean("WantUseReferenceImage", false, 1);

            m_smVisionInfo.g_objBarcode.LoadBarcode(strPath + "Settings.xml", "Settings");
            m_smVisionInfo.g_objBarcode.LoadPattern(strPath + "\\Template\\Template0.mch");
            ROI.LoadFile(strPath + "\\ROI.xml", m_smVisionInfo.g_arrBarcodeROIs);
            for (int i = 0; i < m_smVisionInfo.g_arrBarcodeROIs.Count; i++)
            {
                if (i == 1)
                    m_smVisionInfo.g_arrBarcodeROIs[i].AttachImage(m_smVisionInfo.g_arrBarcodeROIs[0]);
                else
                    m_smVisionInfo.g_arrBarcodeROIs[i].AttachImage(m_smVisionInfo.g_arrImages[0]);
            }
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            LoadBarcodeSetting();

            this.Close();
            this.Dispose();
        }
        private void btn_Previous_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intLearnStepNo > 0)
                m_smVisionInfo.g_intLearnStepNo--;

            if (m_intDisplayStepNo > 0)
                m_intDisplayStepNo--;

            SetupSteps(false);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {

            m_smVisionInfo.g_intLearnStepNo++;

            m_intDisplayStepNo++;
            SetupSteps(true);

            lbl_StepNo.Text = lbl_StepNo.Text.Substring(0, lbl_StepNo.Text.Length - 2) + m_intDisplayStepNo.ToString() + ":";
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void SetupSteps(bool blnForward)
        {
            switch (m_smVisionInfo.g_intLearnStepNo)
            {

                case 0: // Define Search ROI
                    lbl_StepNo.BringToFront();
                    lbl_Step1.BringToFront();
                    tabCtrl_Barcode.SelectedTab = tp_ReferenceSearchROI;

                    AddROI("Search ROI", 1, 0);

                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;

                    btn_Previous.Enabled = false;
                    btn_Next.Enabled = true;
                    btn_Save.Visible = false;
                    break;

                case 1: // Define Pattern ROI

                    lbl_Step2.BringToFront();
                    tabCtrl_Barcode.SelectedTab = tp_ReferencePatternROI;
                    txt_ToleranceTop.Text = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top.ToString();
                    txt_ToleranceLeft.Text = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left.ToString();
                    txt_ToleranceRight.Text = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right.ToString();
                    txt_ToleranceBottom.Text = m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom.ToString();

                    PatternSearchROI.LoadROISetting(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left
                        , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top
                        , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIWidth - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right
                        , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIHeight - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom);

                    PatternSearchROI.AttachImage(m_smVisionInfo.g_arrImages[0]);

                    AddROI("Pattern ROI", 2, 1);

                    m_smVisionInfo.g_arrBarcodeROIs[1].AttachImage(PatternSearchROI);

                    if (!m_blnPatternMatched)
                    {
                        m_blnPatternMatchResult = m_smVisionInfo.g_objBarcode.MatchReferencePattern(PatternSearchROI);
                        if (m_blnPatternMatchResult)
                        {
                            m_smVisionInfo.g_arrBarcodeROIs[1].LoadROISetting(
                                (int)(m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - (m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIWidth / 2)),
                                (int)(m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - (m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIHeight / 2)),
                                m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIWidth,
                                m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIHeight);
                        }
                        m_blnPatternMatched = true;
                    }

                    m_smVisionInfo.g_blnViewSearchROI = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;

                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = true;
                    btn_Save.Visible = false;
                    break;

                case 2: // Define Barcode

                    lbl_Step3.BringToFront();
                    tabCtrl_Barcode.SelectedTab = tp_Barcode;
                    AddROI("Barcode ROI", 1, 2);

                    //float MatcherCenterX = m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX();
                    //float MatcherCenterY = m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY();

                    if (m_blnPatternMatchResult)
                    {
                        bool blnOrientationFail = false;
                        //  m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                        //(int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX - (m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROIWidth / 2)),
                        //(int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY - (m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROIHeight / 2)),
                        //m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROIWidth,
                        //m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROIHeight);
                        if (m_smVisionInfo.g_objBarcode.ref_intTemplateCount > 0)
                            StartWaiting("Detecting Barcode...");
                        
                        for (int i = 0; i < m_smVisionInfo.g_objBarcode.ref_intTemplateCount; i++)
                        {
                            m_smVisionInfo.g_objWhiteImage.CopyTo(m_objImage_Temp);
                            ROI objTestROI = new ROI();
                            objTestROI.AttachImage(m_objImage_Temp);
                            ////2021-04-03 ZJYEOH : If pattern ROI center different pattern matched center more than 3 pixel, assume user moved pattern ROI to other place
                            //if (Math.Abs(m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterX - m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX - m_smVisionInfo.g_objBarcode.GetMatchingCenterX()) > 2 ||
                            //    Math.Abs(m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterY - m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY - m_smVisionInfo.g_objBarcode.GetMatchingCenterY()) > 2)
                            //{
                            //    double AngleResult = Math.Atan2(m_smVisionInfo.g_objBarcode.ref_intPatternTemplateCenterY - (m_smVisionInfo.g_intCameraResolutionHeight / 2), m_smVisionInfo.g_objBarcode.ref_intPatternTemplateCenterX - (m_smVisionInfo.g_intCameraResolutionWidth / 2))
                            //                  - Math.Atan2(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - (m_smVisionInfo.g_intCameraResolutionHeight / 2), m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - (m_smVisionInfo.g_intCameraResolutionWidth / 2));
                            //    double AngleResultInDegree = (AngleResult * 180 / Math.PI);
                            //    if (AngleResult < 0)
                            //        AngleResult += (2 * Math.PI);
                            //    AngleResultInDegree = -(AngleResult * 180 / Math.PI);
                            //    float fTemplateRotatedX = (float)(((m_smVisionInfo.g_intCameraResolutionWidth / 2)) + ((m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeCenterX[i] - (m_smVisionInfo.g_intCameraResolutionWidth / 2)) * Math.Cos(AngleResultInDegree * Math.PI / 180)) -
                            //                   ((m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeCenterY[i] - (m_smVisionInfo.g_intCameraResolutionHeight / 2)) * Math.Sin(AngleResultInDegree * Math.PI / 180)));
                            //    float fTemplateRotatedY = (float)(((m_smVisionInfo.g_intCameraResolutionHeight / 2)) + ((m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeCenterX[i] - (m_smVisionInfo.g_intCameraResolutionWidth / 2)) * Math.Sin(AngleResultInDegree * Math.PI / 180)) +
                            //                    ((m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeCenterY[i] - (m_smVisionInfo.g_intCameraResolutionHeight / 2)) * Math.Cos(AngleResultInDegree * Math.PI / 180)));
                            //    //m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX[i] = m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterX - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeCenterX[i];
                            //    //m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY[i] = m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterY - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeCenterY[i];
                            //    m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX[i] = m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterX - fTemplateRotatedX;
                            //    m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY[i] = m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterY - fTemplateRotatedY;
                            //}
                            float fWidth = 0;
                            float fHeight = 0;
                            float fPatternAngle = m_smVisionInfo.g_objBarcode.GetMatchingAngle();
                            float fAngle = 0;

                            if (m_blnPreTestDone)
                                fPatternAngle = 0;

                            if (fPatternAngle > 0)
                            {
                                if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] > 0)
                                {
                                    if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] > 90)
                                        fAngle = 180 - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] - fPatternAngle;
                                    else
                                        fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + fPatternAngle;
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] < -90)
                                        fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + 180 + fPatternAngle;
                                    else
                                        fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + fPatternAngle;
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] > 0)
                                {
                                    if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] > 90)
                                        fAngle = 180 - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] - fPatternAngle;
                                    else
                                        fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + fPatternAngle;
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] < -90)
                                        fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + 180 + fPatternAngle;
                                    else
                                        fAngle = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeAngle[i] + fPatternAngle;
                                }
                            }

                            if (fAngle < 0)
                                fAngle = 180 + fAngle;

                            float fBarcodeAngle = fAngle;

                            if (fAngle < 90)
                            {
                                fWidth = (int)Math.Round((m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i] * Math.Cos(fAngle * Math.PI / 180)) + (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i] * Math.Sin(fAngle * Math.PI / 180)));
                                fHeight = (int)Math.Round((m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i] * Math.Sin(fAngle * Math.PI / 180)) + (m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i] * Math.Cos(fAngle * Math.PI / 180)));
                            }
                            else if (fAngle == 90)
                            {
                                fWidth = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i];
                                fHeight = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i];
                            }
                            else if (fAngle > 90)
                            {
                                float fWidth_1 = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i];
                                float fHeight_1 = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i];
                                float fAngle_1 = fAngle - 90;
                                fWidth = (int)Math.Round((fWidth_1 * Math.Cos(fAngle_1 * Math.PI / 180)) + (fHeight_1 * Math.Sin(fAngle_1 * Math.PI / 180)));
                                fHeight = (int)Math.Round((fWidth_1 * Math.Sin(fAngle_1 * Math.PI / 180)) + (fHeight_1 * Math.Cos(fAngle_1 * Math.PI / 180)));
                            }

                            //int intOffsetX = (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX[i]);
                            //int intOffsetY = (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY[i]);

                            int intOffsetX = (int)(m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterX - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX[i]);
                            int intOffsetY = (int)(m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterY - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY[i]);

                            //m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                            //    (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX[i] - (fWidth / 2)),
                            //    (int)(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY[i] - (fHeight / 2)),
                            //    (int)fWidth,
                            //    (int)fHeight);

                            //m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI.bmp");

                            float CenterX = 0;
                            float CenterY = 0;
                            float fXAfterRotated = 0;
                            float fYAfterRotated = 0;

                            //CenterX = m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX();
                            //CenterY = m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY();

                            CenterX = m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterX;
                            CenterY = m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterY;

                            //fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROICenterX - CenterX) * Math.Cos(m_smVisionInfo.g_objBarcode.GetMatchingAngle() * Math.PI / 180)) -
                            //                   ((m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROICenterY - CenterY) * Math.Sin(m_smVisionInfo.g_objBarcode.GetMatchingAngle() * Math.PI / 180)));

                            //fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROICenterX - CenterX) * Math.Sin(m_smVisionInfo.g_objBarcode.GetMatchingAngle() * Math.PI / 180)) +
                            //                    ((m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROICenterY - CenterY) * Math.Cos(m_smVisionInfo.g_objBarcode.GetMatchingAngle() * Math.PI / 180)));
                            //if (Math.Abs(m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterX - m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX - m_smVisionInfo.g_objBarcode.GetMatchingCenterX()) > 2 ||
                            //   Math.Abs(m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterY - m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY - m_smVisionInfo.g_objBarcode.GetMatchingCenterY()) > 2)
                            //{
                            //    fXAfterRotated = intOffsetX;

                            //    fYAfterRotated = intOffsetY;
                            //    //fXAfterRotated = (float)((m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.GetMatchingCenterX()) + ((intOffsetX - m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX - m_smVisionInfo.g_objBarcode.GetMatchingCenterX()) * Math.Cos(fPatternAngle * Math.PI / 180)) -
                            //    //           ((intOffsetY - m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY - m_smVisionInfo.g_objBarcode.GetMatchingCenterY()) * Math.Sin(fPatternAngle * Math.PI / 180)));

                            //    //fYAfterRotated = (float)((m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.GetMatchingCenterY()) + ((intOffsetX - m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX - m_smVisionInfo.g_objBarcode.GetMatchingCenterX()) * Math.Sin(fPatternAngle * Math.PI / 180)) +
                            //    //                    ((intOffsetY - m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY - m_smVisionInfo.g_objBarcode.GetMatchingCenterY()) * Math.Cos(fPatternAngle * Math.PI / 180)));
                            //}
                            //else
                            {
                                fXAfterRotated = (float)((CenterX) + ((intOffsetX - CenterX) * Math.Cos(fPatternAngle * Math.PI / 180)) -
                                               ((intOffsetY - CenterY) * Math.Sin(fPatternAngle * Math.PI / 180)));

                                fYAfterRotated = (float)((CenterY) + ((intOffsetX - CenterX) * Math.Sin(fPatternAngle * Math.PI / 180)) +
                                                    ((intOffsetY - CenterY) * Math.Cos(fPatternAngle * Math.PI / 180)));
                            }

                            float fBarcodeWidth = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeWidth[i] * 2f;//2f
                            float fBarcodeHeight = m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeHeight[i] * 2f;//2f

                            List<Point> arrPoints = new List<Point>();
                            arrPoints.Add(new Point((int)(fXAfterRotated - (fBarcodeWidth / 2)), (int)(fYAfterRotated - (fBarcodeHeight / 2))));
                            arrPoints.Add(new Point((int)(fXAfterRotated + (fBarcodeWidth / 2)), (int)(fYAfterRotated - (fBarcodeHeight / 2))));
                            arrPoints.Add(new Point((int)(fXAfterRotated + (fBarcodeWidth / 2)), (int)(fYAfterRotated + (fBarcodeHeight / 2))));
                            arrPoints.Add(new Point((int)(fXAfterRotated - (fBarcodeWidth / 2)), (int)(fYAfterRotated + (fBarcodeHeight / 2))));

                            DontCareWithoutRotateImage.ProduceImage_ForBarcode(arrPoints, m_objImage_Temp, m_smVisionInfo.g_objWhiteImage, m_smVisionInfo.g_objBlackImage, fBarcodeAngle, false, m_smVisionInfo.g_objBarcode.ref_fDontCareScale);

                            fXAfterRotated = fXAfterRotated - (fWidth / 2);
                            fYAfterRotated = fYAfterRotated - (fHeight / 2);


                            //m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                            //    (int)fXAfterRotated,
                            //    (int)fYAfterRotated,
                            //    (int)fWidth,
                            //    (int)fHeight);
                            float fScale = 0.2f;//0.2f;
                            objTestROI.LoadROISetting(
                            (int)(fXAfterRotated - (fWidth * fScale)),
                            (int)(fYAfterRotated - (fHeight * fScale)),
                            (int)(fWidth + (fWidth * fScale * 2)),
                            (int)(fHeight + (fHeight * fScale * 2)));

                            m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                            (int)(fXAfterRotated - (fWidth * fScale)),
                            (int)(fYAfterRotated - (fHeight * fScale)),
                            (int)(fWidth + (fWidth * fScale * 2)),
                            (int)(fHeight + (fHeight * fScale * 2)));

                            ROI.SubtractROI2(m_smVisionInfo.g_arrBarcodeROIs[2], objTestROI);

                            //m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI" + i.ToString() + ".bmp");
                            //objTestROI.SaveImage("D:\\objTestROI" + i.ToString() + ".bmp");

                            int intFailType = 0;
                            if (m_smVisionInfo.g_objBarcode.ref_intCodeType == 0)
                                intFailType = m_smVisionInfo.g_objBarcode.ReadBarcodeObjects(objTestROI, false, i, true, fAngle, m_smVisionInfo.g_objWhiteImage, m_smVisionInfo.g_objBlackImage);
                            else if (m_smVisionInfo.g_objBarcode.ref_intCodeType == 1)
                                intFailType = m_smVisionInfo.g_objBarcode.ReadQRCodeObjects(objTestROI, false);
                            else
                                intFailType = m_smVisionInfo.g_objBarcode.ReadMatrixCodeObjects(objTestROI, false);
                            if (intFailType == 1 || intFailType == 2 || intFailType == 3)
                            {
                                dgd_Template.Rows[i].Cells[0].Style.BackColor = Color.Red;
                                dgd_Template.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                                dgd_Template.Rows[i].Cells[2].Value = "----";
                                m_smVisionInfo.g_strErrorMessage += "*" + m_smVisionInfo.g_objBarcode.ref_strErrorMessage;
                                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                            }
                            else
                            {
                                dgd_Template.Rows[i].Cells[0].Style.BackColor = Color.Lime;
                                dgd_Template.Rows[i].Cells[0].Style.SelectionBackColor = Color.Lime;
                                dgd_Template.Rows[i].Cells[2].Value = m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i];
                                //arrROIStart[i] = new Point((int)(fXAfterRotated - 15), (int)(fYAfterRotated - 15));
                                //arrROISize[i] = new Size((int)fWidth + 30, (int)fHeight + 30);

                                if (Math.Abs(m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[i] - m_smVisionInfo.g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                                {
                                    dgd_Template.Rows[i].Cells[2].Style.BackColor = Color.Red;
                                    dgd_Template.Rows[i].Cells[2].Style.SelectionBackColor = Color.Red;
                                    dgd_Template.Rows[i].Cells[0].Style.BackColor = Color.Red;
                                    dgd_Template.Rows[i].Cells[0].Style.SelectionBackColor = Color.Red;
                                    blnOrientationFail = true;
                                }
                                else
                                {
                                    dgd_Template.Rows[i].Cells[2].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                                    dgd_Template.Rows[i].Cells[2].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                                }
                            }

                            //arrROIStart[i] = new Point((int)(fXAfterRotated - 15), (int)(fYAfterRotated - 15));
                            //arrROISize[i] = new Size((int)fWidth + 30, (int)fHeight + 30);
                            arrROIStart[i] = new Point((int)(fXAfterRotated - (fWidth * 0.08)), (int)(fYAfterRotated - (fHeight * 0.08)));
                            arrROISize[i] = new Size((int)(fWidth + (fWidth * 0.08 * 2)), (int)(fHeight + (fHeight * 0.08 * 2)));
                            /////////////////////
                            if (intFailType == 0)
                            {
                                m_smVisionInfo.g_objBarcode.ResetInspectionData_Learn(i);
                                
                                if (cbo_CodeType.SelectedIndex == 0)
                                    m_smVisionInfo.g_objBarcode.ReadBarcodeObjects(m_smVisionInfo.g_arrBarcodeROIs[2], true, i, false, 0, m_smVisionInfo.g_objWhiteImage, m_smVisionInfo.g_objBlackImage);
                                else if (cbo_CodeType.SelectedIndex == 1)
                                    m_smVisionInfo.g_objBarcode.ReadQRCodeObjects(m_smVisionInfo.g_arrBarcodeROIs[2], true);
                                else
                                    m_smVisionInfo.g_objBarcode.ReadMatrixCodeObjects(m_smVisionInfo.g_arrBarcodeROIs[2], true);
                             
                                if (m_smVisionInfo.g_objBarcode.ref_blnCodeFound[i] && m_smVisionInfo.g_objBarcode.ref_blnCodePassed[i])
                                {
                                    m_smVisionInfo.g_objBarcode.ref_strTemplateCode[i] = m_smVisionInfo.g_objBarcode.ref_strResultCode[i];
                                    m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX[i] = m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterX - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeCenterX[i];
                                    m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY[i] = m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterY - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeCenterY[i];
                                 
                                }
                            }
                            ////////////////
                            if ((i + 1) >= m_smVisionInfo.g_objBarcode.ref_intTemplateCount)
                                m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                                   arrROIStart[0].X,
                                   arrROIStart[0].Y,
                                   arrROISize[0].Width,
                                   arrROISize[0].Height);

                            objTestROI.Dispose();
                        }

                        if (blnOrientationFail)
                        {
                            m_smVisionInfo.g_strErrorMessage += "*Barcode Orientation Fail.";
                            m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                        }

                        m_smVisionInfo.g_blnDrawBarcodeResult = true;
                        btn_Save.Enabled = true;
                        for (int i = 0; i < dgd_Template.RowCount; i++)
                        {
                            if (dgd_Template.Rows[i].Cells[0].Style.BackColor == Color.Red || dgd_Template.Rows[i].Cells[0].Style.BackColor == Color.Empty || dgd_Template.Rows[i].Cells[2].Style.BackColor == Color.Red)
                                btn_Save.Enabled = false;
                        }
                        if (m_smVisionInfo.g_objBarcode.ref_intTemplateCount > 0)
                            StopWaiting();
                    }
                    m_blnPreTestDone = true;
                    m_smVisionInfo.g_blnDragROI = true;
                    m_smVisionInfo.g_blnViewROI = true;
                    m_smVisionInfo.g_blnViewSearchROI = true;
                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = false;
                    btn_Save.Visible = true;
                    break;
                case 3: // Bright Reference Image
                    lbl_StepNo.BringToFront();
                    lbl_Step4.BringToFront();
                    tabCtrl_Barcode.SelectedTab = tp_BrightReferenceImage;
                    //m_smVisionInfo.g_ojRotateImage.CopyTo(ref m_smVisionInfo.g_objPackageImage);
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = false;
                    m_smVisionInfo.g_blnViewPackageImage = false;
                    btn_Previous.Enabled = false;
                    btn_Next.Enabled = true;
                    break;
                case 4: // Dark Reference Image
                    lbl_StepNo.BringToFront();
                    lbl_Step5.BringToFront();
                    tabCtrl_Barcode.SelectedTab = tp_DarkReferenceImage;
                    btn_Live.Text = "Live";
                    m_smVisionInfo.AT_PR_StartLiveImage = false;
                    m_smVisionInfo.g_blnViewSearchROI = false;
                    m_smVisionInfo.g_blnDragROI = false;
                    m_smVisionInfo.g_blnViewROI = false;
                    m_smVisionInfo.g_blnViewPackageImage = true;

                    UpdateGainImage();

                    btn_Previous.Enabled = true;
                    btn_Next.Enabled = false;
                    break;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void AddROI(string strROIName, int intROIType, int intArrayIndex)
        {
            if (intArrayIndex >= m_smVisionInfo.g_arrBarcodeROIs.Count)
            {
                for (int i = m_smVisionInfo.g_arrBarcodeROIs.Count; i <= intArrayIndex; i++)
                {
                    if (i == intArrayIndex)
                    {
                        ROI objROI = new ROI();
                        objROI.ref_strROIName = strROIName;
                        objROI.ref_intType = intROIType;
                        if (intArrayIndex == 1)
                        {
                            objROI.ref_ROIPositionX = 0;
                            objROI.ref_ROIPositionY = 0;
                            objROI.ref_ROIWidth = 100;
                            objROI.ref_ROIHeight = 100;
                        }
                        else
                        {
                            objROI.ref_ROIPositionX = 100;
                            objROI.ref_ROIPositionY = 100;
                            objROI.ref_ROIWidth = 100;
                            objROI.ref_ROIHeight = 100;
                        }
                        m_smVisionInfo.g_arrBarcodeROIs.Add(objROI);
                    }
                    else
                    {
                        m_smVisionInfo.g_arrBarcodeROIs.Add(new ROI());
                    }
                }
            }

            if (m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].ref_strROIName != strROIName)
            {
                m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].ref_strROIName = strROIName;
            }

            if (m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].ref_intType != intROIType)
            {
                m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].ref_intType = intROIType;
            }


            if (strROIName == "Search ROI")
            {
                // Attach Search ROI to image
                if (m_intCurrentPreciseDeg != 0)
                    m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                else
                    m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrImages[0]);
            }
            //else if (strROIName == "Pattern ROI")
            //{
            //    m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrBarcodeROIs[0]);
            //}
            else 
            {
                if (strROIName == "Barcode ROI")
                {
                    if (m_intCurrentPreciseDeg != 0)
                        m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
                    else
                        m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].AttachImage(m_smVisionInfo.g_arrImages[0]);
                }
                if (m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].ref_ROIWidth == 0 || m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].ref_ROIHeight == 0)
                {
                    m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].ref_ROIWidth = 100;
                    m_smVisionInfo.g_arrBarcodeROIs[intArrayIndex].ref_ROIHeight = 100;

                }

            }
        }
        private void RotatePrecise()
        {
            btn_ClockWise.Enabled = false;
            btn_CounterClockWise.Enabled = false;

            //ROI objROI = new ROI();

            //    objROI = m_smVisionInfo.g_arrBarcodeROIs[0];
            ImageDrawing.Rotate0Degree(m_smVisionInfo.g_arrImages[0], -m_intCurrentPreciseDeg, ref m_smVisionInfo.g_arrRotatedImages, 0);

            //ImageDrawing objRotatedImage = m_smVisionInfo.g_arrRotatedImages[0];
            //m_smVisionInfo.g_arrImages[0].CopyTo(ref objRotatedImage);
            //ROI.Rotate0Degree(objROI, - m_intCurrentPreciseDeg, 8, ref m_smVisionInfo.g_arrRotatedImages, 0);
            if (m_smVisionInfo.g_blnViewPackageImage)
                m_smVisionInfo.g_arrRotatedImages[0].CopyTo(ref m_smVisionInfo.g_objPackageImage);

            // After rotating the image, attach the rotated image into ROI again
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {

                m_smVisionInfo.g_arrBarcodeROIs[0].AttachImage(m_smVisionInfo.g_arrRotatedImages[0]);
            }

            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            srmLabel28.Text = m_intCurrentPreciseDeg.ToString() + " deg";
            btn_ClockWise.Enabled = true;
            btn_CounterClockWise.Enabled = true;

        }

        private void btn_ClockWisePrecisely_Click(object sender, EventArgs e)
        {
            m_intCurrentPreciseDeg++;
            RotatePrecise();
        }

        private void btn_CounterClockWisePrecisely_Click(object sender, EventArgs e)
        {
            m_intCurrentPreciseDeg--;
            RotatePrecise();
        }

        private void btn_Detect_Click(object sender, EventArgs e)
        {
            btn_Detect.Enabled = false;
            m_smVisionInfo.g_objBarcode.ResetInspectionData_Learn(m_intSelectedTemplateIndex);

            ////ImageDrawing objUniformImage = new ImageDrawing(true, m_smVisionInfo.g_arrImages[0].ref_intImageWidth, m_smVisionInfo.g_arrImages[0].ref_intImageHeight);
            //////m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI1.bmp");
            ////m_smVisionInfo.g_arrImages[0].CopyTo(objUniformImage);
            ////string strPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_strRecipeID + "\\" +
            ////           m_smVisionInfo.g_strVisionFolderName + "\\Barcode\\";
            ////if (m_smVisionInfo.g_objBarcode.ref_blnWantUseReferenceImage && File.Exists(strPath + "\\Template\\" + "BrightReferenceImage.bmp") && File.Exists(strPath + "\\Template\\" + "DarkReferenceImage.bmp"))
            ////{
            ////    ImageDrawing.UniformizeImage(m_smVisionInfo.g_arrImages[0], m_smVisionInfo.g_objBrightReferenceImage, m_smVisionInfo.g_objDarkReferenceImage, ref objUniformImage);

            ////    m_smVisionInfo.g_arrBarcodeROIs[2].AttachImage(objUniformImage);

            ////    //m_smVisionInfo.g_arrImages[0].SaveImage("D:\\img.bmp");
            ////    //objUniformImage.SaveImage("D:\\objUniformImage.bmp");
            ////    //m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI2.bmp");

            ////}
            //m_smVisionInfo.g_arrBarcodeROIs[2].SaveImage("D:\\ROI1.bmp");
            if (cbo_CodeType.SelectedIndex == 0)
                m_smVisionInfo.g_objBarcode.ReadBarcodeObjects(m_smVisionInfo.g_arrBarcodeROIs[2], true, m_intSelectedTemplateIndex, false, 0, m_smVisionInfo.g_objWhiteImage, m_smVisionInfo.g_objBlackImage);
            else if (cbo_CodeType.SelectedIndex == 1)
                m_smVisionInfo.g_objBarcode.ReadQRCodeObjects(m_smVisionInfo.g_arrBarcodeROIs[2], true);
            else
                m_smVisionInfo.g_objBarcode.ReadMatrixCodeObjects(m_smVisionInfo.g_arrBarcodeROIs[2], true);
            ////m_smVisionInfo.g_arrBarcodeROIs[2].AttachImage(m_smVisionInfo.g_arrImages[0]);
            ////objUniformImage.Dispose();

            btn_Detect.Enabled = true;
            bool blnOrientationFail = false;
            if (m_smVisionInfo.g_objBarcode.ref_blnCodeFound[m_intSelectedTemplateIndex] && m_smVisionInfo.g_objBarcode.ref_blnCodePassed[m_intSelectedTemplateIndex])
            {
                m_smVisionInfo.g_objBarcode.ref_strTemplateCode[m_intSelectedTemplateIndex] = m_smVisionInfo.g_objBarcode.ref_strResultCode[m_intSelectedTemplateIndex];
                btn_Save.Enabled = true;
                m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetX[m_intSelectedTemplateIndex] = m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterX - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeCenterX[m_intSelectedTemplateIndex];
                m_smVisionInfo.g_objBarcode.ref_fPatternReferenceOffsetY[m_intSelectedTemplateIndex] = m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROITotalCenterY - m_smVisionInfo.g_objBarcode.ref_fTemplateBarcodeCenterY[m_intSelectedTemplateIndex];
                dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[0].Style.BackColor = Color.Lime;
                dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[0].Style.SelectionBackColor = Color.Lime;
                arrROIStart[m_intSelectedTemplateIndex] = new Point(m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROITotalX, m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROITotalY);
                arrROISize[m_intSelectedTemplateIndex] = new Size(m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROIWidth, m_smVisionInfo.g_arrBarcodeROIs[2].ref_ROIHeight);
                if (Math.Abs(m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[m_intSelectedTemplateIndex] - m_smVisionInfo.g_objBarcode.ref_fBarcodeOrientationAngle) > 45)
                {
                    dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[2].Style.BackColor = Color.Red;
                    dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[2].Style.SelectionBackColor = Color.Red;
                    dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[0].Style.BackColor = Color.Red;
                    dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[0].Style.SelectionBackColor = Color.Red;
                    blnOrientationFail = true;
                }
                else
                {
                    dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[2].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[m_intSelectedTemplateIndex];
                    dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[2].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[m_intSelectedTemplateIndex];
                }

                if (blnOrientationFail)
                {
                    m_smVisionInfo.g_strErrorMessage += "*Barcode Orientation Fail.";
                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                }

                for (int i = 0; i < dgd_Template.RowCount; i++)
                {
                    if (dgd_Template.Rows[i].Cells[0].Style.BackColor == Color.Red || dgd_Template.Rows[i].Cells[0].Style.BackColor == Color.Empty || dgd_Template.Rows[i].Cells[2].Style.BackColor == Color.Red)
                        btn_Save.Enabled = false;
                }
            }
            else
            {
                m_smVisionInfo.g_objBarcode.ref_strResultCode[m_intSelectedTemplateIndex] = "----";
                m_smVisionInfo.g_objBarcode.ref_strTemplateCode[m_intSelectedTemplateIndex] = "";
                btn_Save.Enabled = false;
                m_smVisionInfo.g_strErrorMessage = "Fail to detect " + GetCodeType();
                m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[0].Style.BackColor = Color.Red;
                dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[0].Style.SelectionBackColor = Color.Red;
            }

            dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[1].Value = m_smVisionInfo.g_objBarcode.ref_strResultCode[m_intSelectedTemplateIndex];
            dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[1].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[m_intSelectedTemplateIndex];
            dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[1].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[m_intSelectedTemplateIndex];
            if (m_smVisionInfo.g_objBarcode.ref_strResultCode[m_intSelectedTemplateIndex] != "----")
                dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[2].Value = m_smVisionInfo.g_objBarcode.ref_fBarcodeAngle[m_intSelectedTemplateIndex];
            else
                dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[2].Value = "----";
            //dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[2].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[m_intSelectedTemplateIndex];
            //dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[2].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[m_intSelectedTemplateIndex];
            lbl_BarcodeCodeResult.Text = m_smVisionInfo.g_objBarcode.ref_strResultCode[m_intSelectedTemplateIndex];

            lbl_BarcodeCodeResult.Size = new Size(lbl_BarcodeCodeResult.Size.Width, Math.Max(75, (int)Math.Round((lbl_BarcodeCodeResult.Text.Length / (lbl_BarcodeCodeResult.Size.Width / lbl_BarcodeCodeResult.Font.Size)) * lbl_BarcodeCodeResult.Font.Height)));


            m_smVisionInfo.g_blnDrawBarcodeResult = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private string GetCodeType()
        {
            switch (cbo_CodeType.SelectedIndex)
            {
                case 0:
                    return "Barcode";
                case 1:
                    return "QR Code";
                case 2:
                    return "Matrix Code";
            }
            return "Barcode";
        }
        private void txt_GainRange_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objBarcode.ref_fGainRangeTolerance = (float)Convert.ToDecimal(txt_GainRange.Value);
        }

        private void txt_BarcodeAngleRange_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objBarcode.ref_fBarcodeAngleRangeTolerance = (float)Convert.ToDecimal(txt_BarcodeAngleRange.Value);
        }

        private void chk_WantUseGainRange_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objBarcode.ref_blnWantUseGainRange = chk_WantUseGainRange.Checked;
        }

        private void chk_WantUseAngleRange_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_objBarcode.ref_blnWantUseAngleRange = chk_WantUseAngleRange.Checked;
        }

        private void timer_Barcode_Tick(object sender, EventArgs e)
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

        }

        private void trackBar_Gain_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Gain.Text = (Convert.ToSingle(trackBar_Gain.Value) / 10).ToString();
        }

        private void txt_Gain_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fImageGain = 0;
            if (!float.TryParse(txt_Gain.Text, out fImageGain))
                return;

            m_smVisionInfo.g_objBarcode.ref_fImageGain = fImageGain;

            UpdateGainImage();
        }

        private void UpdateGainImage()
        {
            m_smVisionInfo.g_ojRotateImage.CopyTo(m_smVisionInfo.g_objPackageImage);
            
            m_smVisionInfo.g_ojRotateImage.AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objBarcode.ref_fImageGain);
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

        private void btn_SaveRefenceImage_Click(object sender, EventArgs e)
        {

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                               m_smVisionInfo.g_strVisionFolderName + "\\Barcode\\";
            //m_smVisionInfo.g_intSelectedImage = 0;

            //SaveBarcodeSetting();

            //ROI.SaveFile(strFolderPath + "Barcode\\ROI.xml", m_smVisionInfo.g_arrBarcodeROIs);

            //LoadBarcodeSetting();

            //m_smVisionInfo.AT_PR_AttachImagetoROI = true;

            //if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
            //    m_smProductionInfo.g_blnSaveRecipeToServer = true;

            //m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            m_smVisionInfo.g_objBarcode.SaveBarcode(strPath + "Settings.xml", false, "Settings", true);

            m_smVisionInfo.g_ojRotateImage.SaveImage(strPath + "\\Template\\" + "BrightReferenceImage.bmp");
            m_smVisionInfo.g_objPackageImage.SaveImage(strPath + "\\Template\\" + "DarkReferenceImage.bmp");

            if (File.Exists(strPath + "\\Template\\" + "BrightReferenceImage.bmp"))
                m_smVisionInfo.g_objBrightReferenceImage.LoadImage(strPath + "\\Template\\" + "BrightReferenceImage.bmp");
            if (File.Exists(strPath + "\\Template\\" + "BrightReferenceImage.bmp"))
                m_smVisionInfo.g_objDarkReferenceImage.LoadImage(strPath + "\\Template\\" + "DarkReferenceImage.bmp");

            this.Close();
            this.Dispose();
        }

        private void cbo_CodeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objBarcode.ref_intCodeType = cbo_CodeType.SelectedIndex;

            if (m_smVisionInfo.g_objBarcode.ref_intCodeType == 0)
                gb_Barcode.Size = new Size(gb_Barcode.Width, 90);
            else
                gb_Barcode.Size = new Size(gb_Barcode.Width, 60);
        }

        private void dgd_Template_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            m_intSelectedTemplateIndex = e.RowIndex;
            
            m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                arrROIStart[e.RowIndex].X,
                arrROIStart[e.RowIndex].Y,
                arrROISize[e.RowIndex].Width,
                arrROISize[e.RowIndex].Height);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;   
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            if (dgd_Template.RowCount < 10)
            {
                dgd_Template.Rows.Add();
                dgd_Template.Rows[dgd_Template.RowCount - 1].Cells[0].Value = (dgd_Template.RowCount).ToString();
                dgd_Template.Rows[dgd_Template.RowCount - 1].Cells[1].Value = "----";
                dgd_Template.Rows[dgd_Template.RowCount - 1].Cells[1].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[dgd_Template.RowCount - 1];
                dgd_Template.Rows[dgd_Template.RowCount - 1].Cells[1].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[dgd_Template.RowCount - 1];
                dgd_Template.Rows[dgd_Template.RowCount - 1].Cells[2].Value = "----";
                dgd_Template.Rows[dgd_Template.RowCount - 1].Cells[2].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[dgd_Template.RowCount - 1];
                dgd_Template.Rows[dgd_Template.RowCount - 1].Cells[2].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[dgd_Template.RowCount - 1];
                m_smVisionInfo.g_objBarcode.ref_intTemplateCount++;
                m_intSelectedTemplateIndex = dgd_Template.RowCount - 1;
                dgd_Template.Rows[dgd_Template.RowCount - 1].Selected = true;
                dgd_Template.CurrentCell = dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[0];
                arrROIStart[m_intSelectedTemplateIndex] = new Point((int)(m_smVisionInfo.g_intCameraResolutionWidth / 2 - 50), (int)(m_smVisionInfo.g_intCameraResolutionHeight / 2 - 50));
                arrROISize[m_intSelectedTemplateIndex] = new Size(100, 100);

                m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                    arrROIStart[m_intSelectedTemplateIndex].X,
                    arrROIStart[m_intSelectedTemplateIndex].Y,
                    arrROISize[m_intSelectedTemplateIndex].Width,
                    arrROISize[m_intSelectedTemplateIndex].Height);

                for (int i = 0; i < dgd_Template.RowCount; i++)
                {
                    if (dgd_Template.Rows[i].Cells[0].Style.BackColor == Color.Red || dgd_Template.Rows[i].Cells[0].Style.BackColor == Color.Empty)
                        btn_Save.Enabled = false;
                }

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
            else
            {
                SRMMessageBox.Show("Only maximum of 10 templates are allowed!");
            }
        }

        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (dgd_Template.RowCount > 1)
            {
                dgd_Template.Rows.RemoveAt(m_intSelectedTemplateIndex);
                for (int i = m_intSelectedTemplateIndex; i < dgd_Template.RowCount; i++)
                {
                    dgd_Template.Rows[i].Cells[0].Value = (i + 1).ToString();
                    dgd_Template.Rows[i].Cells[1].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                    dgd_Template.Rows[i].Cells[1].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                    dgd_Template.Rows[i].Cells[2].Style.BackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                    dgd_Template.Rows[i].Cells[2].Style.SelectionBackColor = m_smVisionInfo.g_objBarcode.ref_Color[i];
                    arrROIStart[i] = arrROIStart[i + 1];
                    arrROISize[i] = arrROISize[i + 1];
                }
                m_smVisionInfo.g_objBarcode.ResetInspectionDataToPrevious_Learn(m_intSelectedTemplateIndex);
                m_smVisionInfo.g_objBarcode.ref_intTemplateCount--;
                if (m_intSelectedTemplateIndex > 0)
                    m_intSelectedTemplateIndex--;
                dgd_Template.Rows[m_intSelectedTemplateIndex].Selected = true;
                dgd_Template.CurrentCell = dgd_Template.Rows[m_intSelectedTemplateIndex].Cells[0];
                m_smVisionInfo.g_arrBarcodeROIs[2].LoadROISetting(
                    arrROIStart[m_intSelectedTemplateIndex].X,
                    arrROIStart[m_intSelectedTemplateIndex].Y,
                    arrROISize[m_intSelectedTemplateIndex].Width,
                    arrROISize[m_intSelectedTemplateIndex].Height);

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                btn_Save.Enabled = true;
                for (int i = 0; i < dgd_Template.RowCount; i++)
                {
                    if (dgd_Template.Rows[i].Cells[0].Style.BackColor == Color.Red || dgd_Template.Rows[i].Cells[0].Style.BackColor == Color.Empty)
                        btn_Save.Enabled = false;
                }
            }
            else
            {
                SRMMessageBox.Show("Please keep at least one template");
            }
        }
        private void StartWaiting(string StrMessage)
        {
            m_thWaitingFormThread = new SRMWaitingFormThread();
            m_thWaitingFormThread.SetStartSplash(StrMessage);
            this.Enabled = false;
        }

        private void StopWaiting()
        {
            m_thWaitingFormThread.SetStopSplash();
            this.Enabled = true;
        }

        private void txt_BarcodeDontCareScale_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_objBarcode.ref_fDontCareScale = (float)Convert.ToDouble(txt_BarcodeDontCareScale.Value / 100);

        }

        private void txt_ToleranceTop_TextChanged(object sender, EventArgs e)
        {
            if (txt_ToleranceTop.Text.StartsWith("-"))
            {
                txt_ToleranceTop.Text = "0";
                return;
            }

            short result = -1;
            if (Int16.TryParse(txt_ToleranceTop.Text, out result))
            {
                m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top = Int16.Parse(txt_ToleranceTop.Text);
                PatternSearchROI.LoadROISetting(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left
                    , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top
                    , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIWidth - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right
                    , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIHeight - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom);
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                PatternSearchROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                m_smVisionInfo.g_objBarcode.MatchReferencePattern(PatternSearchROI);
                m_smVisionInfo.g_arrBarcodeROIs[1].LoadROISetting(
                    (int)(m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - (m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIWidth / 2)),
                    (int)(m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - (m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIHeight / 2)),
                    m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIWidth,
                    m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIHeight);
            }
            else
                return;
        }

        private void txt_ToleranceLeft_TextChanged(object sender, EventArgs e)
        {
            if (txt_ToleranceLeft.Text.StartsWith("-"))
            {
                txt_ToleranceLeft.Text = "0";
                return;
            }

            short result = -1;
            if (Int16.TryParse(txt_ToleranceLeft.Text, out result))
            {
                m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left = Int16.Parse(txt_ToleranceLeft.Text);
                PatternSearchROI.LoadROISetting(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left
                    , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top
                    , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIWidth - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right
                    , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIHeight - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom);
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                PatternSearchROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                m_smVisionInfo.g_objBarcode.MatchReferencePattern(PatternSearchROI);
                m_smVisionInfo.g_arrBarcodeROIs[1].LoadROISetting(
                    (int)(m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - (m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIWidth / 2)),
                    (int)(m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - (m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIHeight / 2)),
                    m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIWidth,
                    m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIHeight);
            }
            else
                return;
        }

        private void txt_ToleranceRight_TextChanged(object sender, EventArgs e)
        {
            if (txt_ToleranceRight.Text.StartsWith("-"))
            {
                txt_ToleranceRight.Text = "0";
                return; 
            }

            short result = -1;
            if (Int16.TryParse(txt_ToleranceRight.Text, out result))
            {
                m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right = Int16.Parse(txt_ToleranceRight.Text);
                PatternSearchROI.LoadROISetting(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left
                , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top
                , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIWidth - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right
                , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIHeight - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom);
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                PatternSearchROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                m_smVisionInfo.g_objBarcode.MatchReferencePattern(PatternSearchROI);
                m_smVisionInfo.g_arrBarcodeROIs[1].LoadROISetting(
                    (int)(m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - (m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIWidth / 2)),
                    (int)(m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - (m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIHeight / 2)),
                    m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIWidth,
                    m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIHeight);
            }
            else
                return;
        }

        private void txt_ToleranceBottom_TextChanged(object sender, EventArgs e)
        {
            if (txt_ToleranceBottom.Text.StartsWith("-"))
            {
                txt_ToleranceBottom.Text = "0";
                return;
            }

            short result = -1;
            if (Int16.TryParse(txt_ToleranceBottom.Text, out result))
            {
                m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom = Int16.Parse(txt_ToleranceBottom.Text);
                PatternSearchROI.LoadROISetting(m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionX + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left
                , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIPositionY + m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top
                , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIWidth - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Left - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Right
                , m_smVisionInfo.g_arrBarcodeROIs[0].ref_ROIHeight - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Top - m_smVisionInfo.g_objBarcode.ref_intPatternDetectionAreaTolerance_Bottom);
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                PatternSearchROI.AttachImage(m_smVisionInfo.g_arrImages[0]);
                m_smVisionInfo.g_objBarcode.MatchReferencePattern(PatternSearchROI);
                m_smVisionInfo.g_arrBarcodeROIs[1].LoadROISetting(
                    (int)(m_smVisionInfo.g_objBarcode.GetMatchingCenterX() - (m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIWidth / 2)),
                    (int)(m_smVisionInfo.g_objBarcode.GetMatchingCenterY() - (m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIHeight / 2)),
                    m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIWidth,
                    m_smVisionInfo.g_arrBarcodeROIs[1].ref_ROIHeight);
            }
            else
                return;
        }
    }
}
