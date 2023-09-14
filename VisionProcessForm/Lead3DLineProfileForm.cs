using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedMemory;
using VisionProcessing;
using Common;
using System.IO;
using Microsoft.Win32;

namespace VisionProcessForm
{
    public partial class Lead3DLineProfileForm : Form
    {
        #region enum

        public enum ReadFrom { Lead3D = 0, Calibration = 1 };

        #endregion

        #region Member Variables
        private int m_intReadFromIndex = 0; // 0: Learn Page, 1: Other Setting Page
        private bool m_blnTriggerOfflineTest = false;
        private int[][] m_arrOriCornerSearchingLength = { new int[2] { 0, 0 }, new int[2] { 0, 0 }, new int[2] { 0, 0 }, new int[2] { 0, 0 }, new int[2] { 0, 0 } };
        private int[] m_arrOriCornerSearchingTolerance_Top = new int[2] { 0, 0 };
        private int[] m_arrOriCornerSearchingTolerance_Right = new int[2] { 0, 0 };
        private int[] m_arrOriCornerSearchingTolerance_Bottom = new int[2] { 0, 0 };
        private int[] m_arrOriCornerSearchingTolerance_Left = new int[2] { 0, 0 };
        private int m_intSelectedCenterDirectionIndex; // 0: TipStart, 1: TipCenter, 2: TipEnd, 3: BaseStart, 4: BaseCenter, 5: BaseEnd
        private int m_intSelectedSideDirectionIndex; // 0: TipCenter, 1: TipStart, 2: TipEnd
        private int m_intSelectedCornerDirectionIndex; // 0: Left/Top, 1: Right/Bottom
        private int m_intSelectedCatergory = 0; // 0: Center, 1: Side, 2: Corner
        private bool m_blnInitDone = false;
        private bool m_blnShow = false;
        private bool m_blnBuildLead = false;
        private bool m_blnUpdateDrawingGaugeAfterBuildLeadDone = false;
        private bool m_blnUpdateHistogram = false;
        private string m_strPath = "";
        private string m_strCenterPath = "";
        private string m_strSidePath = "";
        private string m_strCornerPath = "";
        private int m_intSelectedROI = 0;
        private ReadFrom m_eReadFrom = ReadFrom.Lead3D;    // Default
        private PGauge m_objPointGauge;
        private Graphics m_Graphic;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private ROI m_objROI = new ROI();
        private ROI m_objThresholdROI = new ROI();
        //private ImageDrawing m_objOriImage = new ImageDrawing(true);
        //private ImageDrawing m_objImage = new ImageDrawing(true);
        #endregion

        #region Properties

        public bool ref_blnShow { get { return m_blnShow; } }
        public bool ref_blnBuildLead { get { return m_blnBuildLead; }  set { m_blnBuildLead = value; } }
        public bool ref_blnUpdateDrawingGaugeAfterBuildLeadDone { get { return m_blnUpdateDrawingGaugeAfterBuildLeadDone; } set { m_blnUpdateDrawingGaugeAfterBuildLeadDone = value; } }
        #endregion

        public Lead3DLineProfileForm(VisionInfo smVisionInfo, PGauge objPointGauge, string strPath, ProductionInfo smProductionInfo, int ReadFromIndex)
        {
            InitializeComponent();
            m_intReadFromIndex = ReadFromIndex;
            m_smProductionInfo = smProductionInfo;
            m_smVisionInfo = smVisionInfo;
            m_smVisionInfo.g_intViewInspectionSetting = 1;
            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);
            m_objPointGauge = objPointGauge;
            m_strPath = strPath;
            m_strCenterPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\CenterPointGauge.xml";
            m_strSidePath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\SidePointGauge.xml";
            m_strCornerPath = m_smProductionInfo.g_strRecipePath + m_smProductionInfo.g_arrSingleRecipeID[m_smVisionInfo.g_intVisionIndex] + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\CornerPointGauge.xml";
            m_intSelectedCatergory = 0;
            m_intSelectedCenterDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber = 0;
            m_intSelectedSideDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber_Side = 0;
            m_intSelectedCornerDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber_Corner = 0;
            m_intSelectedROI = m_smVisionInfo.g_intSelectedROI;
            m_smVisionInfo.g_intSelectedROI = 0;
            m_smVisionInfo.g_intLeadSelectedNumber = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].CopyCenterSettingToSettingPointGauge(0, m_intSelectedCenterDirectionIndex);
                //m_arrOriCornerSearchingLength[i] = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intCornerSearchingLength;
                m_arrOriCornerSearchingLength[i] = m_smVisionInfo.g_arrLead3D[i].ref_arrCornerSearchingLength;

                m_smVisionInfo.g_arrLead3D[i].ref_intSelectedCenterDirectionIndex = 0;
                m_smVisionInfo.g_arrLead3D[i].ref_intSelectedSideDirectionIndex = 0;
                m_smVisionInfo.g_arrLead3D[i].ref_intSelectedCornerDirectionIndex = 0;
            }
            if (m_smVisionInfo.g_arrLead3D.Length == 5)
            {
                m_arrOriCornerSearchingTolerance_Top = m_smVisionInfo.g_arrLead3D[1].ref_arrCornerSearchingTolerance_Top;
                m_arrOriCornerSearchingTolerance_Right = m_smVisionInfo.g_arrLead3D[2].ref_arrCornerSearchingTolerance_Right;
                m_arrOriCornerSearchingTolerance_Bottom = m_smVisionInfo.g_arrLead3D[3].ref_arrCornerSearchingTolerance_Bottom;
                m_arrOriCornerSearchingTolerance_Left = m_smVisionInfo.g_arrLead3D[4].ref_arrCornerSearchingTolerance_Left;
            }

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_SetToAll_Side.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllPointGauge_Lead3DSide", false));
            chk_SetToAll_Corner.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllPointGauge_Lead3DCorner", false));
            m_smVisionInfo.g_blnViewPackageImage = true;
            UpdateLocalROI();
            UpdateGUI();
          
            m_blnInitDone = true;

            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }

        private void UpdateLocalROI()
        {
            //if (m_smVisionInfo.g_blnViewRotatedImage)
            //    m_objROI.AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            //else
            //    m_objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            //m_objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth,
            //    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageHeight);

            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            else
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);

            m_objThresholdROI.AttachImage(m_smVisionInfo.g_objPackageImage);

            m_objROI.AttachImage(m_smVisionInfo.g_objPackageImage);
            m_objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth,
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageHeight);

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ProduceDontCareImage(m_smVisionInfo.g_arrLeadROIs[i][0], m_smVisionInfo.g_objPackageImage);
            }
            
            if (radioBtn_Corner.Checked)
            {
                m_objThresholdROI.LoadROISetting(m_smVisionInfo.g_arrLeadROIs[m_smVisionInfo.g_intSelectedROI][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadROIs[m_smVisionInfo.g_intSelectedROI][0].ref_ROIPositionY,
                    m_smVisionInfo.g_arrLeadROIs[m_smVisionInfo.g_intSelectedROI][0].ref_ROIWidth,
                    m_smVisionInfo.g_arrLeadROIs[m_smVisionInfo.g_intSelectedROI][0].ref_ROIHeight);
                m_objThresholdROI.ThresholdTo_ROIToROISamePosition_Bigger(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intThresholdValue);

                if (chk_SetToAll_Corner.Checked)
                {
                    for (int i = 1; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                    {
                        if (m_smVisionInfo.g_intSelectedROI == i)
                            continue;

                        m_objThresholdROI.LoadROISetting(m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIPositionX, m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIPositionY,
                    m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIWidth,
                    m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIHeight);
                        m_objThresholdROI.ThresholdTo_ROIToROISamePosition_Bigger(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrLead3D[i].ref_intThresholdValue);
                    }
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void UpdateGUI()
        {
            m_smVisionInfo.g_blnSetToAllPoints_Center = chk_AllPoints_Center.Checked = false;
            m_smVisionInfo.g_blnSetToAllLeadPad_Center = chk_AllLeads_Center.Checked = false;
            m_smVisionInfo.g_blnSetToAllPoints_Side = chk_AllPoints_Side.Checked = false;
            m_smVisionInfo.g_blnSetToAllLeadPad_Side = chk_AllLeads_Side.Checked = false;
            m_smVisionInfo.g_blnSetToAllROIs_Side = chk_SetToAll_Side.Checked = false;
            m_smVisionInfo.g_blnSetToAllPoints_Corner = chk_AllPoints_Corner.Checked = false;
            m_smVisionInfo.g_blnSetToAllROIs_Corner = chk_SetToAll_Corner.Checked = false;

            switch (m_eReadFrom)
            {
                case ReadFrom.Lead3D:
                    UpdateSelectedCatergory();
                    UpdateGaugeToGUI();
                    break;
            }
           
            // Init Point Gauge location
            // m_objPointGauge.SetGaugeCenter(530, 350);
            // m_objPointGauge.SetGaugeToleranceAngle(100, 0);
            m_objPointGauge.EnableManualDrag();
            cbo_LeadNo_Center.Items.Clear();
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
            {
                if (m_smVisionInfo.g_arrLead3D[0].GetBlobsFeaturesNumber() > 0)
                {
                    if (cbo_LeadNo_Center.Items.Count == 0)
                    {
                        int intTotalLead = m_smVisionInfo.g_arrLead3D[0].GetBlobsFeaturesNumber();
                        for (int p = 0; p < intTotalLead; p++)
                        {
                            cbo_LeadNo_Center.Items.Add((p + 1).ToString());
                        }
                    }
                }
            }
            if (cbo_LeadNo_Center.Items.Count > 0)
                cbo_LeadNo_Center.SelectedIndex = 0;

            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
            if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                m_smVisionInfo.g_intLeadSelectedNumber = 0;

            cbo_ROI_Side.Items.Clear();
            if (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intLeadDirection == 0)
            {
                cbo_ROI_Side.Items.Add("Right");
                cbo_ROI_Side.Items.Add("Left");
            }
            else
            {
                cbo_ROI_Side.Items.Add("Top");
                cbo_ROI_Side.Items.Add("Bottom");
            }

            if (cbo_ROI_Side.Items.Count > 0)
                    cbo_ROI_Side.SelectedIndex = 0;

            cbo_ROI_Corner.Items.Clear();

            cbo_ROI_Corner.Items.Add("Top");
            cbo_ROI_Corner.Items.Add("Right");
            cbo_ROI_Corner.Items.Add("Bottom");
            cbo_ROI_Corner.Items.Add("Left");

            if (cbo_ROI_Corner.Items.Count > 0)
                cbo_ROI_Corner.SelectedIndex = 0;

            SetSettingPointGaugePosition();

            //if (m_smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance)
            //    panel3.BringToFront();
            //else
            //    panel3.SendToBack();
        }
        private void UpdateSelectedCatergory()
        {
            switch (m_intSelectedCatergory)
            {
                default:
                case 0: // Center
                    pnl_Center.Visible = true;
                    pnl_Side.Visible = false;
                    pnl_Corner.Visible = false;
                    panel_Setting.Size = new Size(panel_Setting.Size.Width, 112);//90
                    this.Size = new Size(360, 1045 - pnl_Corner.Height - pnl_Side.Height);
                    break;
                case 1: // Side
                    pnl_Center.Visible = false;
                    pnl_Side.Visible = true;
                    pnl_Corner.Visible = false;
                    panel_Setting.Size = new Size(panel_Setting.Size.Width, 112);//90
                    this.Size = new Size(360, 1045 - pnl_Corner.Height - pnl_Center.Height);
                    break;
                case 2: // Corner
                    pnl_Center.Visible = false;
                    pnl_Side.Visible = false;
                    pnl_Corner.Visible = true;
                    panel_Setting.Size = new Size(panel_Setting.Size.Width, 165);//150
                    this.Size = new Size(360, 1105 - pnl_Center.Height - pnl_Side.Height);
                    break;
            }
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            this.Location = new Point(resolution.Width - this.Size.Width, resolution.Height - this.Size.Height);
        }
        private void UpdateGaugeToGUI()
        {
            switch (m_intSelectedCatergory)
            {
                case 0:
                    m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
                    if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                        m_smVisionInfo.g_intLeadSelectedNumber = 0;

                    PGauge objPGauge_Center = m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex];

                    if (objPGauge_Center.ref_GaugeTransType == 0)
                    {
                        radioBtn_BlackToWhite.Checked = true;
                    }
                    else
                    {
                        radioBtn_WhiteToBlack.Checked = true;
                    }
                    if (objPGauge_Center.ref_GaugeTransChoice == 0)
                        radioBtn_FromBegin.Checked = true;
                    else if (objPGauge_Center.ref_GaugeTransChoice == 1)
                        radioBtn_FromEnd.Checked = true;
                    else
                        radioBtn_LargestAmplitude.Checked = true;

                    txt_MeasThickness.Text = objPGauge_Center.ref_GaugeThickness.ToString();
                    trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);

                    txt_threshold.Text = objPGauge_Center.ref_GaugeThreshold.ToString();
                    trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);

                    txt_MinAmplitude.Text = objPGauge_Center.ref_GaugeMinAmplitude.ToString();
                    trackBar_MinAmplitude.Value = Convert.ToInt32(txt_MinAmplitude.Text);

                    break;
                case 1:
                    m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Side.SelectedIndex;
                    if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                        m_smVisionInfo.g_intLeadSelectedNumber = 0;

                    PGauge objPGauge_Side = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex];

                    if (objPGauge_Side.ref_GaugeTransType == 0)
                    {
                        radioBtn_BlackToWhite.Checked = true;
                    }
                    else
                    {
                        radioBtn_WhiteToBlack.Checked = true;
                    }
                    if (objPGauge_Side.ref_GaugeTransChoice == 0)
                        radioBtn_FromBegin.Checked = true;
                    else if (objPGauge_Side.ref_GaugeTransChoice == 1)
                        radioBtn_FromEnd.Checked = true;
                    else
                        radioBtn_LargestAmplitude.Checked = true;

                    txt_MeasThickness.Text = objPGauge_Side.ref_GaugeThickness.ToString();
                    trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);

                    txt_threshold.Text = objPGauge_Side.ref_GaugeThreshold.ToString();
                    trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);

                    txt_MinAmplitude.Text = objPGauge_Side.ref_GaugeMinAmplitude.ToString();
                    trackBar_MinAmplitude.Value = Convert.ToInt32(txt_MinAmplitude.Text);

                    break;
                case 2:
                    PGauge objPGauge_Corner = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex];

                    if (objPGauge_Corner.ref_GaugeTransType == 0)
                    {
                        radioBtn_BlackToWhite.Checked = true;
                    }
                    else
                    {
                        radioBtn_WhiteToBlack.Checked = true;
                    }
                    if (objPGauge_Corner.ref_GaugeTransChoice == 0)
                        radioBtn_FromBegin.Checked = true;
                    else if (objPGauge_Corner.ref_GaugeTransChoice == 1)
                        radioBtn_FromEnd.Checked = true;
                    else
                        radioBtn_LargestAmplitude.Checked = true;

                    txt_MeasThickness.Text = objPGauge_Corner.ref_GaugeThickness.ToString();
                    trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);

                    txt_threshold.Text = objPGauge_Corner.ref_GaugeThreshold.ToString();
                    trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);

                    txt_MinAmplitude.Text = objPGauge_Corner.ref_GaugeMinAmplitude.ToString();
                    trackBar_MinAmplitude.Value = Convert.ToInt32(txt_MinAmplitude.Text);

                    txt_CornerLength.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingLength[m_intSelectedCornerDirectionIndex].ToString();
                    trackBar_CornerLength.Value = Convert.ToInt32(txt_CornerLength.Text);

                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 1:
                            txt_CornerOffset.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingTolerance_Top[m_intSelectedCornerDirectionIndex].ToString();
                            break;
                        case 2:
                            txt_CornerOffset.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingTolerance_Right[m_intSelectedCornerDirectionIndex].ToString();
                            break;
                        case 3:
                            txt_CornerOffset.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingTolerance_Bottom[m_intSelectedCornerDirectionIndex].ToString();
                            break;
                        case 4:
                            txt_CornerOffset.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingTolerance_Left[m_intSelectedCornerDirectionIndex].ToString();
                            break;
                    }
                    trackBar_CornerOffset.Value = Convert.ToInt32(txt_CornerOffset.Text);
                    break;
                default:
                    if (m_smVisionInfo.g_arrLead3D[0].ref_objPointGauge.ref_GaugeTransType == 0)
                    {
                        radioBtn_BlackToWhite.Checked = true;
                    }
                    else
                    {
                        radioBtn_WhiteToBlack.Checked = true;
                    }
                    if (m_smVisionInfo.g_arrLead3D[0].ref_objPointGauge.ref_GaugeTransChoice == 0)
                        radioBtn_FromBegin.Checked = true;
                    else if (m_smVisionInfo.g_arrLead3D[0].ref_objPointGauge.ref_GaugeTransChoice == 1)
                        radioBtn_FromEnd.Checked = true;
                    else
                        radioBtn_LargestAmplitude.Checked = true;

                    txt_MeasThickness.Text = m_smVisionInfo.g_arrLead3D[0].ref_objPointGauge.ref_GaugeThickness.ToString();
                    trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);

                    txt_threshold.Text = m_smVisionInfo.g_arrLead3D[0].ref_objPointGauge.ref_GaugeThreshold.ToString();
                    trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);

                    txt_MinAmplitude.Text = m_smVisionInfo.g_arrLead3D[0].ref_objPointGauge.ref_GaugeMinAmplitude.ToString();
                    trackBar_MinAmplitude.Value = Convert.ToInt32(txt_MinAmplitude.Text);
                    break;
            }
        }

        private void pic_Histogram_Click(object sender, EventArgs e)
        {

        }

        private void LineProfileForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewLeadInspection = false;
            m_smVisionInfo.g_blnViewLeadTipBuildAreaDrawing = false;
            m_smVisionInfo.g_blnViewLead3DPkgToBaseDrawing = false;
            m_blnShow = true;
            m_smVisionInfo.g_blnViewPointGauge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            switch (m_eReadFrom)
            {
                case ReadFrom.Lead3D:

                    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        
                        STDeviceEdit.CopySettingFile(m_strCenterPath, "");
                        m_smVisionInfo.g_arrLead3D[i].SaveArrayPointGauge_Center(
                        m_strCenterPath);
                        STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Line Profile", m_smProductionInfo.g_strLotID);

                        STDeviceEdit.CopySettingFile(m_strSidePath, "");
                        m_smVisionInfo.g_arrLead3D[i].SaveArrayPointGauge_Side(m_strSidePath);
                        STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Line Profile", m_smProductionInfo.g_strLotID);

                        STDeviceEdit.CopySettingFile(m_strCornerPath, "");
                        m_smVisionInfo.g_arrLead3D[i].SaveArrayPointGauge_Corner(
                        m_strCornerPath);
                        STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Line Profile", m_smProductionInfo.g_strLotID);

                        STDeviceEdit.CopySettingFile(m_strPath, "");
                        m_smVisionInfo.g_arrLead3D[i].ref_objPointGauge.SavePointGauge(
                        m_strPath,
                        false,
                        "Lead3D" + i.ToString(),
                        true,
                        true);
                        STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Line Profile", m_smProductionInfo.g_strLotID);
                        
                    }
                         
                          
                    break;
            }

            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            switch (m_eReadFrom)
            {
                case ReadFrom.Lead3D:
                    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        m_smVisionInfo.g_arrLead3D[i].ref_objPointGauge.LoadPointGauge(m_strPath,
                                "Lead3D" + i.ToString());
                        m_smVisionInfo.g_arrLead3D[i].LoadArrayPointGauge_Center(m_strCenterPath, "Lead3D" + i.ToString());
                        m_smVisionInfo.g_arrLead3D[i].LoadArrayPointGauge_Side(m_strSidePath, "Lead3D" + i.ToString());
                        m_smVisionInfo.g_arrLead3D[i].LoadArrayPointGauge_Corner(m_strCornerPath, "Lead3D" + i.ToString());
                        m_smVisionInfo.g_arrLead3D[i].ref_arrCornerSearchingLength = m_arrOriCornerSearchingLength[i];
                    }
                    if (m_smVisionInfo.g_arrLead3D.Length == 5)
                    {
                        m_smVisionInfo.g_arrLead3D[1].ref_arrCornerSearchingTolerance_Top = m_arrOriCornerSearchingTolerance_Top;
                        m_smVisionInfo.g_arrLead3D[2].ref_arrCornerSearchingTolerance_Right = m_arrOriCornerSearchingTolerance_Right;
                        m_smVisionInfo.g_arrLead3D[3].ref_arrCornerSearchingTolerance_Bottom = m_arrOriCornerSearchingTolerance_Bottom;
                        m_smVisionInfo.g_arrLead3D[4].ref_arrCornerSearchingTolerance_Left = m_arrOriCornerSearchingTolerance_Left;
                    }
                    break;
            }

            this.Close();
            this.Dispose();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (m_blnUpdateHistogram || m_smVisionInfo.AT_VM_UpdateHistogram)
            {
                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.DrawLineProfile(m_Graphic, pic_Histogram.Width, pic_Histogram.Height);

                m_blnUpdateHistogram = false;
                m_smVisionInfo.AT_VM_UpdateHistogram = false;
            }

            if (m_blnTriggerOfflineTest)
            {
                m_blnTriggerOfflineTest = false;
                m_smVisionInfo.AT_VM_OfflineTestAllLead3D = true;
                TriggerOfflineTest();
            }

            if (m_smVisionInfo.PR_MN_UpdateInfo)
            {
                SetSettingPointGaugePosition();

                m_smVisionInfo.g_blnViewPointGauge = true;

                m_smVisionInfo.PR_MN_UpdateInfo = false;
            }

            if (m_blnUpdateDrawingGaugeAfterBuildLeadDone)
            {
                m_blnUpdateDrawingGaugeAfterBuildLeadDone = false;

                SetSettingPointGaugePosition();
            }
        }

        private void pic_Histogram_Paint(object sender, PaintEventArgs e)
        {
            m_blnUpdateHistogram = true;
        }
        
        private void radioBtn_WhiteToBlack_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Lead3D:

                    switch (m_intSelectedCatergory)
                    {
                        case 0:
                            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
                            if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                                m_smVisionInfo.g_intLeadSelectedNumber = 0;

                            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                            {
                                if (m_smVisionInfo.g_blnSetToAllLeadPad_Center)
                                {
                                    for (int L = 0; L < m_smVisionInfo.g_arrLead3D[0].GetNumberOfLead(); L++)
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Center)
                                        {
                                            for (int p = 0; p < 6; p++)
                                            {
                                                if (L < m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge.Count &&
                                                     p < m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[L].Count)
                                                {
                                                    if (radioBtn_BlackToWhite.Checked)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[L][p].ref_GaugeTransType = 0;
                                                        m_smVisionInfo.g_arrLead3D[0].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                                    }
                                                    else
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[L][p].ref_GaugeTransType = 1;
                                                        m_smVisionInfo.g_arrLead3D[0].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (L < m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge.Count &&
                                                     m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[L].Count)
                                            {
                                                if (radioBtn_BlackToWhite.Checked)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[L][m_intSelectedCenterDirectionIndex].ref_GaugeTransType = 0;
                                                    m_smVisionInfo.g_arrLead3D[0].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                                }
                                                else
                                                {
                                                    m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[L][m_intSelectedCenterDirectionIndex].ref_GaugeTransType = 1;
                                                    m_smVisionInfo.g_arrLead3D[0].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints_Center)
                                    {
                                        for (int p = 0; p < 6; p++)
                                        {
                                            if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge.Count &&
                                 p < m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                            {
                                                if (radioBtn_BlackToWhite.Checked)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransType = 0;
                                                    m_smVisionInfo.g_arrLead3D[0].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                                }
                                                else
                                                {
                                                    m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransType = 1;
                                                    m_smVisionInfo.g_arrLead3D[0].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge.Count &&
                                                       m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                        {
                                            if (radioBtn_BlackToWhite.Checked)
                                            {
                                                m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].ref_GaugeTransType = 0;
                                                m_smVisionInfo.g_arrLead3D[0].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                            }
                                            else
                                            {
                                                m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].ref_GaugeTransType = 1;
                                                m_smVisionInfo.g_arrLead3D[0].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case 1:
                            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Side.SelectedIndex;
                            if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                                m_smVisionInfo.g_intLeadSelectedNumber = 0;

                            if (m_smVisionInfo.g_blnSetToAllROIs_Side)
                            {
                                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                {
                                    if (m_smVisionInfo.g_blnSetToAllLeadPad_Side)
                                    {
                                        for (int L = 0; L < m_smVisionInfo.g_arrLead3D[i].GetNumberOfLead(); L++)
                                        {
                                            if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                            {
                                                for (int p = 0; p < 3; p++)
                                                {
                                                    if (L < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                   p < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L].Count)
                                                    {
                                                        if (radioBtn_BlackToWhite.Checked)
                                                        {
                                                            m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L][p].ref_GaugeTransType = 0;
                                                            m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                                        }
                                                        else
                                                        {
                                                            m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L][p].ref_GaugeTransType = 1;
                                                            m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (L < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                   m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L].Count)
                                                {
                                                    if (radioBtn_BlackToWhite.Checked)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L][m_intSelectedSideDirectionIndex].ref_GaugeTransType = 0;
                                                        m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                                    }
                                                    else
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L][m_intSelectedSideDirectionIndex].ref_GaugeTransType = 1;
                                                        m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                        {
                                            for (int p = 0; p < 3; p++)
                                            {
                                                if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                               p < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                                {
                                                    if (radioBtn_BlackToWhite.Checked)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransType = 0;
                                                        m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                                    }
                                                    else
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransType = 1;
                                                        m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                               m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                            {
                                                if (radioBtn_BlackToWhite.Checked)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].ref_GaugeTransType = 0;
                                                    m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                                }
                                                else
                                                {
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].ref_GaugeTransType = 1;
                                                    m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_blnSetToAllLeadPad_Side)
                                {
                                    for (int L = 0; L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].GetNumberOfLead(); L++)
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                        {
                                            for (int p = 0; p < 3; p++)
                                            {
                                                if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                               p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L].Count)
                                                {
                                                    if (radioBtn_BlackToWhite.Checked)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L][p].ref_GaugeTransType = 0;
                                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                                    }
                                                    else
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L][p].ref_GaugeTransType = 1;
                                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                               m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L].Count)
                                            {
                                                if (radioBtn_BlackToWhite.Checked)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L][m_intSelectedSideDirectionIndex].ref_GaugeTransType = 0;
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                                }
                                                else
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L][m_intSelectedSideDirectionIndex].ref_GaugeTransType = 1;
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                    {
                                        for (int p = 0; p < 3; p++)
                                        {
                                            if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                           p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                            {
                                                if (radioBtn_BlackToWhite.Checked)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransType = 0;
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                                }
                                                else
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransType = 1;
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                           m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                        {
                                            if (radioBtn_BlackToWhite.Checked)
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].ref_GaugeTransType = 0;
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                            }
                                            else
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].ref_GaugeTransType = 1;
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case 2:
                            if (m_smVisionInfo.g_blnSetToAllROIs_Corner)
                            {
                                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                                    {
                                        for (int p = 0; p < 2; p++)
                                        {
                                            if (p < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                            {
                                                if (radioBtn_BlackToWhite.Checked)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge[p].ref_GaugeTransType = 0;
                                                    m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                                }
                                                else
                                                {
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge[p].ref_GaugeTransType = 1;
                                                    m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                        {
                                            if (radioBtn_BlackToWhite.Checked)
                                            {
                                                m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].ref_GaugeTransType = 0;
                                                m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                            }
                                            else
                                            {
                                                m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].ref_GaugeTransType = 1;
                                                m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                                {
                                    for (int p = 0; p < 2; p++)
                                    {
                                        if (p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                        {
                                            if (radioBtn_BlackToWhite.Checked)
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[p].ref_GaugeTransType = 0;
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                            }
                                            else
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[p].ref_GaugeTransType = 1;
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                    {
                                        if (radioBtn_BlackToWhite.Checked)
                                        {
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].ref_GaugeTransType = 0;
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                        }
                                        else
                                        {
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].ref_GaugeTransType = 1;
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                            {
                                if (radioBtn_BlackToWhite.Checked)
                                {
                                    m_smVisionInfo.g_arrLead3D[i].ref_objPointGauge.ref_GaugeTransType = 0;
                                    m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 0;
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrLead3D[i].ref_objPointGauge.ref_GaugeTransType = 1;
                                    m_smVisionInfo.g_arrLead3D[i].ref_objSettingPointGauge.ref_GaugeTransType = 1;
                                }
                            }
                            break;
                    }
                    break;
            }
            //ROI objROI = new ROI();
            //objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            //objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth,
            //    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageHeight);
            switch (m_intSelectedCatergory)
            {
                case 0:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge.Count &&
                                                   m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[0].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 1:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                          m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 2:
                    if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].Measure(m_objThresholdROI);
                    }
                    break;
                default:
                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.Measure(m_objROI);
                    break;
            }
            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(m_objROI);
            //objROI.Dispose();
            // Update histogram
            m_blnUpdateHistogram = true;
            m_blnBuildLead = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }

        private void LineProfileForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_blnDrawThresholdAllSideROI = false;
            //m_objImage.Dispose();
            //m_objOriImage.Dispose();
            m_objROI.Dispose();
            m_objThresholdROI.Dispose();
            m_smVisionInfo.AT_VM_OfflineTestAllLead3D = false;
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_intViewInspectionSetting = 0;
            m_objPointGauge.DisableManualDrag();
            m_smVisionInfo.g_intSelectedROI = m_intSelectedROI;
            m_blnShow = false;
            m_smVisionInfo.g_blnViewPointGauge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void radioBtn_Search_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            int intTransChoiceIndex;
            if (radioBtn_FromBegin.Checked)
                intTransChoiceIndex = 0;
            else if (radioBtn_FromEnd.Checked)
                intTransChoiceIndex = 1;
            else
                intTransChoiceIndex = 2;

            if(m_smVisionInfo.g_arrLead3D.Length > m_smVisionInfo.g_intSelectedROI)
            {
                switch (m_intSelectedCatergory)
                {
                    case 0:
                        m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
                        if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                            m_smVisionInfo.g_intLeadSelectedNumber = 0;

                        if (m_smVisionInfo.g_blnSetToAllLeadPad_Center)
                        {
                            for (int L = 0; L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].GetNumberOfLead(); L++)
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints_Center)
                                {
                                    for (int p = 0; p < 6; p++)
                                    {
                                        if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                          p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L].Count)
                                        {
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                        }
                                    }
                                }
                                else
                                {
                                    if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                         m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L].Count)
                                    {
                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L][m_intSelectedCenterDirectionIndex].ref_GaugeTransChoice = intTransChoiceIndex;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnSetToAllPoints_Center)
                            {
                                for (int p = 0; p < 6; p++)
                                {
                                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                      p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                    {
                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                    }
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                     m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                {
                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].ref_GaugeTransChoice = intTransChoiceIndex;
                                }
                            }
                        }
                        break;
                    case 1:
                        m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Side.SelectedIndex;
                        if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                            m_smVisionInfo.g_intLeadSelectedNumber = 0;

                        if (m_smVisionInfo.g_blnSetToAllROIs_Side)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                            {
                                if (m_smVisionInfo.g_blnSetToAllLeadPad_Side)
                                {
                                    for (int L = 0; L < m_smVisionInfo.g_arrLead3D[i].GetNumberOfLead(); L++)
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                        {
                                            for (int p = 0; p < 3; p++)
                                            {
                                                if (L < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                    p < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (L < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                       m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L].Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L][m_intSelectedSideDirectionIndex].ref_GaugeTransChoice = intTransChoiceIndex;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                    {
                                        for (int p = 0; p < 3; p++)
                                        {
                                            if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                p < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                   m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                        {
                                            m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].ref_GaugeTransChoice = intTransChoiceIndex;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnSetToAllLeadPad_Side)
                            {
                                for (int L = 0; L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].GetNumberOfLead(); L++)
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                    {
                                        for (int p = 0; p < 3; p++)
                                        {
                                            if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                                p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L].Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                                   m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L].Count)
                                        {
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L][m_intSelectedSideDirectionIndex].ref_GaugeTransChoice = intTransChoiceIndex;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                {
                                    for (int p = 0; p < 3; p++)
                                    {
                                        if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                            p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                        {
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeTransChoice = intTransChoiceIndex;
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                               m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                    {
                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].ref_GaugeTransChoice = intTransChoiceIndex;
                                    }
                                }
                            }
                        }
                        break;
                    case 2:
                        if (m_smVisionInfo.g_blnSetToAllROIs_Corner)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                                {
                                    for (int p = 0; p < 2; p++)
                                    {
                                        if (p < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                        {
                                            m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge[p].ref_GaugeTransChoice = intTransChoiceIndex;
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                    {
                                        m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].ref_GaugeTransChoice = intTransChoiceIndex;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                            {
                                for (int p = 0; p < 2; p++)
                                {
                                    if (p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                    {
                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[p].ref_GaugeTransChoice = intTransChoiceIndex;
                                    }
                                }
                            }
                            else
                            {
                                if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                {
                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].ref_GaugeTransChoice = intTransChoiceIndex;
                                }
                            }
                        }
                        break;
                    default:
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeTransChoice = intTransChoiceIndex;
                        break;
                }
                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeTransChoice = intTransChoiceIndex;
            }

            //ROI objROI = new ROI();
            //objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            //objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth,
            //    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageHeight);
            switch (m_intSelectedCatergory)
            {
                case 0:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                 m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 1:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                     m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 2:
                    if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].Measure(m_objThresholdROI);
                    }
                    break;
                default:
                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.Measure(m_objROI);
                    break;
            }
            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(m_objROI);
            //objROI.Dispose();

            // Update histogram
            m_blnUpdateHistogram = true;
            m_blnBuildLead = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }

        private void txt_MeasThickness_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Lead3D:
                    if (m_smVisionInfo.g_arrLead3D.Length > m_smVisionInfo.g_intSelectedROI)
                    {
                        switch (m_intSelectedCatergory)
                        {
                            case 0:
                                m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
                                if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                                    m_smVisionInfo.g_intLeadSelectedNumber = 0;

                                if (m_smVisionInfo.g_blnSetToAllLeadPad_Center)
                                {
                                    for (int L = 0; L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].GetNumberOfLead(); L++)
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Center)
                                        {
                                            for (int p = 0; p < 6; p++)
                                            {
                                                if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                             p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L][p].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                    m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L].Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L][m_intSelectedCenterDirectionIndex].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                                }
                                        }
                                    }
                                }
                                else
                                    if (m_smVisionInfo.g_blnSetToAllPoints_Center)
                                {
                                    for (int p = 0; p < 6; p++)
                                    {
                                        if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                     p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                        {
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                            m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                    {
                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                    }
                                }
                                break;
                            case 1:
                                m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Side.SelectedIndex;
                                if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                                    m_smVisionInfo.g_intLeadSelectedNumber = 0;

                                if (m_smVisionInfo.g_blnSetToAllROIs_Side)
                                {
                                    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllLeadPad_Side)
                                        {
                                            for (int L = 0; L < m_smVisionInfo.g_arrLead3D[i].GetNumberOfLead(); L++)
                                            {
                                                if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                                {
                                                    for (int p = 0; p < 3; p++)
                                                    {
                                                        if (L < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                       p < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L].Count)
                                                        {
                                                            m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L][p].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (L < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                       m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L].Count)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L][m_intSelectedSideDirectionIndex].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                            {
                                                for (int p = 0; p < 3; p++)
                                                {
                                                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                   p < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                   m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_blnSetToAllLeadPad_Side)
                                    {
                                        for (int L = 0; L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].GetNumberOfLead(); L++)
                                        {
                                            if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                            {
                                                for (int p = 0; p < 3; p++)
                                                {
                                                    if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                                   p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L].Count)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L][p].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                                   m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L][m_intSelectedSideDirectionIndex].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                        {
                                            for (int p = 0; p < 3; p++)
                                            {
                                                if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                               p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                               m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                            }
                                        }
                                    }
                                }
                                break;
                            case 2:
                                if (m_smVisionInfo.g_blnSetToAllROIs_Corner)
                                {
                                    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                                        {
                                            for (int p = 0; p < 2; p++)
                                            {
                                                if (p < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge[p].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                                    {
                                        for (int p = 0; p < 2; p++)
                                        {
                                            if (p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[p].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                        {
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                        }
                                    }
                                }
                                break;
                            default:
                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                                break;
                        }
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
                        trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
                    }
                    break;
            }

            //ROI objROI = new ROI();
            //objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            //objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth,
            //    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageHeight);
            switch (m_intSelectedCatergory)
            {
                case 0:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                             m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 1:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                 m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 2:
                    if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].Measure(m_objThresholdROI);
                    }
                    break;
                default:
                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.Measure(m_objROI);
                    break;
            }
            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(m_objROI);
            //objROI.Dispose();

            // Update histogram
            m_blnUpdateHistogram = true;
            m_blnBuildLead = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }

        private void txt_threshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Lead3D:
                    if (m_smVisionInfo.g_arrLead3D.Length > m_smVisionInfo.g_intSelectedROI)
                    {
                        switch (m_intSelectedCatergory)
                        {
                            case 0:
                                m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
                                if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                                    m_smVisionInfo.g_intLeadSelectedNumber = 0;

                                if (m_smVisionInfo.g_blnSetToAllLeadPad_Center)
                                {
                                    for (int L = 0; L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].GetNumberOfLead(); L++)
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Center)
                                        {
                                            for (int p = 0; p < 6; p++)
                                            {
                                                if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                            p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L][p].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                                }
                                            }
                                        }
                                        else
                                              if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                            m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L].Count)
                                        {
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L][m_intSelectedCenterDirectionIndex].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints_Center)
                                    {
                                        for (int p = 0; p < 6; p++)
                                        {
                                            if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                        p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                            }
                                        }
                                    }
                                    else
                                               if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                             m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                    {
                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                    }
                                }
                                break;
                            case 1:
                                m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Side.SelectedIndex;
                                if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                                    m_smVisionInfo.g_intLeadSelectedNumber = 0;

                                if (m_smVisionInfo.g_blnSetToAllROIs_Side)
                                {
                                    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllLeadPad_Side)
                                        {
                                            for (int L = 0; L < m_smVisionInfo.g_arrLead3D[i].GetNumberOfLead(); L++)
                                            {
                                                if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                                {
                                                    for (int p = 0; p < 3; p++)
                                                    {
                                                        if (L < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                      p < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L].Count)
                                                        {
                                                            m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L][p].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (L < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                  m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L].Count)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L][m_intSelectedSideDirectionIndex].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                            {
                                                for (int p = 0; p < 3; p++)
                                                {
                                                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                  p < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                              m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_blnSetToAllLeadPad_Side)
                                    {
                                        for (int L = 0; L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].GetNumberOfLead(); L++)
                                        {
                                            if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                            {
                                                for (int p = 0; p < 3; p++)
                                                {
                                                    if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                                  p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L].Count)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L][p].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                              m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L][m_intSelectedSideDirectionIndex].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                        {
                                            for (int p = 0; p < 3; p++)
                                            {
                                                if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                              p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                          m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                            }
                                        }
                                    }
                                }
                                break;
                            case 2:
                                if (m_smVisionInfo.g_blnSetToAllROIs_Corner)
                                {
                                    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                                        {
                                            for (int p = 0; p < 2; p++)
                                            {
                                                if (p < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge[p].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                                    {
                                        for (int p = 0; p < 2; p++)
                                        {
                                            if (p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[p].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                        {
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                        }
                                    }
                                }
                                break;
                            default:
                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                                break;
                        }
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_threshold.Text);
                        trackBar_Derivative.Value = Convert.ToInt32(txt_threshold.Text);
                    }
                    break;
            }
            //ROI objROI = new ROI();
            //objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            //objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth,
            //    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageHeight);
            switch (m_intSelectedCatergory)
            {
                case 0:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                           m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 1:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                 m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 2:
                    if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].Measure(m_objThresholdROI);
                    }
                    break;
                default:
                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.Measure(m_objROI);
                    break;
            }
            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(m_objROI);
            //objROI.Dispose();
            // Update histogram
            m_blnUpdateHistogram = true;
            m_blnBuildLead = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }

        private void trackBar_Thickness_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MeasThickness.Text = trackBar_Thickness.Value.ToString();
        }

        private void trackBar_Derivative_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_threshold.Text = trackBar_Derivative.Value.ToString();
        }
        private void UpdateSideComboBox()
        {
            cbo_LeadNo_Side.Items.Clear();
            if (m_smVisionInfo.g_arrLead3D.Length > m_smVisionInfo.g_intSelectedROI)
            {
                if (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].GetBlobsFeaturesNumber() > 0)
                {
                    if (cbo_LeadNo_Side.Items.Count == 0)
                    {
                        int intTotalPad = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].GetBlobsFeaturesNumber();
                        for (int p = 0; p < intTotalPad; p++)
                        {
                            cbo_LeadNo_Side.Items.Add((p + 1).ToString());
                        }
                    }
                }
            }
            if (cbo_LeadNo_Side.Items.Count > 0)
                cbo_LeadNo_Side.SelectedIndex = 0;
        }
        private void radioBtn_Catergory_Click(object sender, EventArgs e)
        {
            if (sender == radioBtn_Center)
            {
                m_smVisionInfo.g_blnViewPackageImage = true;
                m_smVisionInfo.g_intSelectedROI = 0;
                UpdateLocalROI();
                UpdateGaugeToGUI();
                m_intSelectedCatergory = 0;
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    m_smVisionInfo.g_arrLead3D[i].CopyCenterSettingToSettingPointGauge(Math.Max(0 ,cbo_LeadNo_Center.SelectedIndex), m_intSelectedCenterDirectionIndex);
                m_smVisionInfo.g_blnDrawThresholdAllSideROI = false;
            }
            else if (sender == radioBtn_Side)
            {
                m_smVisionInfo.g_blnViewPackageImage = false;
                UpdateLocalROI();
                UpdateGaugeToGUI();
                if (m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intLeadDirection == 0)
                {
                    if (cbo_ROI_Side.SelectedIndex == 0)
                        m_smVisionInfo.g_intSelectedROI = 2;
                    else
                        m_smVisionInfo.g_intSelectedROI = 4;
                }
                else
                {
                    if (cbo_ROI_Side.SelectedIndex == 0)
                        m_smVisionInfo.g_intSelectedROI = 1;
                    else
                        m_smVisionInfo.g_intSelectedROI = 3;
                }
                m_intSelectedCatergory = 1;
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {
                    m_smVisionInfo.g_arrLead3D[i].CopySideSettingToSettingPointGauge(Math.Max(0, cbo_LeadNo_Side.SelectedIndex), m_intSelectedSideDirectionIndex);
                    m_smVisionInfo.g_arrLead3D[i].ref_blnDrawSidePointGauge = true;
                }

                UpdateSideComboBox();

                m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToAll_Side.Checked;
            }
            else if (sender == radioBtn_Corner)
            {
                m_smVisionInfo.g_blnViewPackageImage = true;
               
                switch (cbo_ROI_Corner.SelectedItem.ToString())
                {
                    case "Top":
                        m_smVisionInfo.g_intSelectedROI = 1;
                        break;
                    case "Right":
                        m_smVisionInfo.g_intSelectedROI = 2;
                        break;
                    case "Bottom":
                        m_smVisionInfo.g_intSelectedROI = 3;
                        break;
                    case "Left":
                        m_smVisionInfo.g_intSelectedROI = 4;
                        break;
                }
                UpdateLocalROI();
                m_intSelectedCatergory = 2;
                for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                {
                    m_smVisionInfo.g_arrLead3D[i].CopyCornerSettingToSettingPointGauge(m_intSelectedCornerDirectionIndex);
                    m_smVisionInfo.g_arrLead3D[i].ref_blnDrawSidePointGauge = false;
                }
                m_smVisionInfo.g_blnDrawThresholdAllSideROI = chk_SetToAll_Corner.Checked;
            }
            UpdateSelectedCatergory();
            UpdateGaugeToGUI();
            SetSettingPointGaugePosition();
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
      
        private void cbo_LeadNo_Center_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
            if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                m_smVisionInfo.g_intLeadSelectedNumber = 0;

            UpdateGaugeToGUI();
            SetSettingPointGaugePosition();
        }

        //private void SetSettingPointGaugePosition()
        //{
        //    switch (m_intSelectedCatergory)
        //    {
        //        case 0:
        //            m_smVisionInfo.g_arrLead3D[0].ref_intSelectedLeadNo = cbo_LeadNo_Center.SelectedIndex;
        //            m_smVisionInfo.g_arrLead3D[0].ref_intSelectedCenterDirectionIndex = m_intSelectedCenterDirectionIndex;
        //            break;
        //        case 1:
        //            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intSelectedLeadNo = cbo_LeadNo_Side.SelectedIndex;
        //            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intSelectedSideDirectionIndex = m_intSelectedSideDirectionIndex;
        //            break;
        //        case 2:
        //            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intSelectedCornerDirectionIndex = m_intSelectedCornerDirectionIndex;
        //            break;
        //    }
        //}

        private void SetSettingPointGaugePosition()
        {
            switch (m_intSelectedCatergory)
            {
                case 0:
                    m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
                    if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                        m_smVisionInfo.g_intLeadSelectedNumber = 0;

                    m_smVisionInfo.g_arrLead3D[0].SetCenterPointGaugePlacement_UsingInspectedLead3DPointGaugePosition(m_smVisionInfo.g_intLeadSelectedNumber, m_intSelectedCenterDirectionIndex);
                    break;
                case 1:
                    m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Side.SelectedIndex;
                    if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                        m_smVisionInfo.g_intLeadSelectedNumber = 0;

                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].SetSidePointGaugePlacement_UsingInspectedLead3DPointGaugePosition(m_smVisionInfo.g_intLeadSelectedNumber, m_intSelectedSideDirectionIndex);
                    break;
                case 2:
                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].SetCornerPointGaugePlacement_UsingInspectedLead3DPointGaugePosition(m_intSelectedCornerDirectionIndex);
                    break;
            }
            //ROI objROI = new ROI();
            //objROI.AttachImage(m_smVisionInfo.g_arrImages[0]); //m_smVisionInfo.g_intSelectedImage
            //objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[0].ref_intImageWidth,
            //    m_smVisionInfo.g_arrImages[0].ref_intImageHeight);
            switch (m_intSelectedCatergory)
            {
                case 0:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                 m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 1:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                 m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 2:
                    if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].Measure(m_objThresholdROI);
                    }
                    break;
                default:
                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.Measure(m_objROI);
                    break;
            }
            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(m_objROI);
            //objROI.Dispose();
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void radioBtn_CenterDirection_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (sender == radioBtn_CenterTipStart)
                m_intSelectedCenterDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber = (int)Lead3D.PointIndex.TipStart;
            else if (sender == radioBtn_CenterTipCenter)
                m_intSelectedCenterDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber = (int)Lead3D.PointIndex.TipCenter;
            else if (sender == radioBtn_CenterTipEnd)
                m_intSelectedCenterDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber = (int)Lead3D.PointIndex.TipEnd;
            else if (sender == radioBtn_CenterBaseStart)
                m_intSelectedCenterDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber = (int)Lead3D.PointIndex.BaseStart;
            else if (sender == radioBtn_CenterBaseCenter)
                m_intSelectedCenterDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber = (int)Lead3D.PointIndex.BaseCenter;
            else if (sender == radioBtn_CenterBaseEnd)
                m_intSelectedCenterDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber = (int)Lead3D.PointIndex.BaseEnd;

            UpdateGaugeToGUI();
            SetSettingPointGaugePosition();
        }
        private void radioBtn_SideDirection_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (sender == radioBtn_SideTipStart)
                m_intSelectedSideDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber_Side = (int)Lead3D.PointIndex_Side.TipStart;
            else if (sender == radioBtn_SideTipCenter)
                m_intSelectedSideDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber_Side = (int)Lead3D.PointIndex_Side.TipCenter;
            else if (sender == radioBtn_SideTipEnd)
                m_intSelectedSideDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber_Side = (int)Lead3D.PointIndex_Side.TipEnd;
           
            SetSettingPointGaugePosition();
        }
        private void cbo_ROI_Side_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            switch (cbo_ROI_Side.SelectedItem.ToString())
            {
                case "Top":
                    m_smVisionInfo.g_intSelectedROI = 1;
                    break;
                case "Right":
                    m_smVisionInfo.g_intSelectedROI = 2;
                    break;
                case "Bottom":
                    m_smVisionInfo.g_intSelectedROI = 3;
                    break;
                case "Left":
                    m_smVisionInfo.g_intSelectedROI = 4;
                    break;
            }
            UpdateSideComboBox();
            UpdateGaugeToGUI();
            SetSettingPointGaugePosition();
        }

        private void cbo_LeadNo_Side_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Side.SelectedIndex;
            if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                m_smVisionInfo.g_intLeadSelectedNumber = 0;

            UpdateGaugeToGUI();
            SetSettingPointGaugePosition();
        }

        private void cbo_ROI_Corner_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (cbo_ROI_Corner.SelectedItem.ToString())
            {
                case "Top":
                    m_smVisionInfo.g_intSelectedROI = 1;
                    break;
                case "Right":
                    m_smVisionInfo.g_intSelectedROI = 2;
                    break;
                case "Bottom":
                    m_smVisionInfo.g_intSelectedROI = 3;
                    break;
                case "Left":
                    m_smVisionInfo.g_intSelectedROI = 4;
                    break;
            }
            UpdateLocalROI();
            UpdateGaugeToGUI();
            SetSettingPointGaugePosition();
        }

        private void radioBtn_CornerDirection_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (sender == radioBtn_Corner1)
                m_intSelectedCornerDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber_Corner = 0;
            else if (sender == radioBtn_Corner2)
                m_intSelectedCornerDirectionIndex = m_smVisionInfo.g_intPointSelectedNumber_Corner = 1;

            SetSettingPointGaugePosition();
        }

        private void txt_CornerLength_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_smVisionInfo.g_arrLead3D.Length > m_smVisionInfo.g_intSelectedROI)
            {
                switch (m_intSelectedCatergory)
                {
                    case 2:
                        if (m_smVisionInfo.g_blnSetToAllROIs_Corner)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                                {
                                    for (int p = 0; p < 2; p++)
                                    {
                                        if (p < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                        {
                                            m_smVisionInfo.g_arrLead3D[i].ref_arrCornerSearchingLength[p] = Convert.ToInt32(txt_CornerLength.Text);
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                    {
                                        m_smVisionInfo.g_arrLead3D[i].ref_arrCornerSearchingLength[m_intSelectedCornerDirectionIndex] = Convert.ToInt32(txt_CornerLength.Text);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                            {
                                for (int p = 0; p < 2; p++)
                                {
                                    if (p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                    {
                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingLength[p] = Convert.ToInt32(txt_CornerLength.Text);
                                    }
                                }
                            }
                            else
                            {
                                if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                {
                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingLength[m_intSelectedCornerDirectionIndex] = Convert.ToInt32(txt_CornerLength.Text);
                                }
                            }
                        }
                        
                        trackBar_CornerLength.Value = Convert.ToInt32(txt_CornerLength.Text);
                        break;
                }
            }
            //ROI objROI = new ROI();
            //objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            //objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth,
            //    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageHeight);
            switch (m_intSelectedCatergory)
            {
                case 0:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                              m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 1:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                      m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 2:
                    if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].Measure(m_objThresholdROI);
                    }
                    break;
                default:
                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.Measure(m_objROI);
                    break;
            }
            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(m_objROI);
            //objROI.Dispose();
            // Update histogram
            SetSettingPointGaugePosition();
            m_blnUpdateHistogram = true;
            m_blnBuildLead = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }

        private void txt_CornerOffset_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_smVisionInfo.g_arrLead3D.Length > m_smVisionInfo.g_intSelectedROI)
            {
                switch (m_intSelectedCatergory)
                {
                    case 2:
                        if (m_smVisionInfo.g_blnSetToAllROIs_Corner)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                            {
                                if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                                {
                                    for (int p = 0; p < 2; p++)
                                    {
                                        if (p < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                        {
                                            switch (i)
                                            {
                                                case 1:
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrCornerSearchingTolerance_Top[p] = Convert.ToInt32(txt_CornerOffset.Text);
                                                    break;
                                                case 2:
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrCornerSearchingTolerance_Right[p] = Convert.ToInt32(txt_CornerOffset.Text);
                                                    break;
                                                case 3:
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrCornerSearchingTolerance_Bottom[p] = Convert.ToInt32(txt_CornerOffset.Text);
                                                    break;
                                                case 4:
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrCornerSearchingTolerance_Left[p] = Convert.ToInt32(txt_CornerOffset.Text);
                                                    break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                    {
                                        switch (i)
                                        {
                                            case 1:
                                                m_smVisionInfo.g_arrLead3D[i].ref_arrCornerSearchingTolerance_Top[m_intSelectedCornerDirectionIndex] = Convert.ToInt32(txt_CornerOffset.Text);
                                                break;
                                            case 2:
                                                m_smVisionInfo.g_arrLead3D[i].ref_arrCornerSearchingTolerance_Right[m_intSelectedCornerDirectionIndex] = Convert.ToInt32(txt_CornerOffset.Text);
                                                break;
                                            case 3:
                                                m_smVisionInfo.g_arrLead3D[i].ref_arrCornerSearchingTolerance_Bottom[m_intSelectedCornerDirectionIndex] = Convert.ToInt32(txt_CornerOffset.Text);
                                                break;
                                            case 4:
                                                m_smVisionInfo.g_arrLead3D[i].ref_arrCornerSearchingTolerance_Left[m_intSelectedCornerDirectionIndex] = Convert.ToInt32(txt_CornerOffset.Text);
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                            {
                                for (int p = 0; p < 2; p++)
                                {
                                    if (p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                    {
                                        switch (m_smVisionInfo.g_intSelectedROI)
                                        {
                                            case 1:
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingTolerance_Top[p] = Convert.ToInt32(txt_CornerOffset.Text);
                                                break;
                                            case 2:
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingTolerance_Right[p] = Convert.ToInt32(txt_CornerOffset.Text);
                                                break;
                                            case 3:
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingTolerance_Bottom[p] = Convert.ToInt32(txt_CornerOffset.Text);
                                                break;
                                            case 4:
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingTolerance_Left[p] = Convert.ToInt32(txt_CornerOffset.Text);
                                                break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                {
                                    switch (m_smVisionInfo.g_intSelectedROI)
                                    {
                                        case 1:
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingTolerance_Top[m_intSelectedCornerDirectionIndex] = Convert.ToInt32(txt_CornerOffset.Text);
                                            break;
                                        case 2:
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingTolerance_Right[m_intSelectedCornerDirectionIndex] = Convert.ToInt32(txt_CornerOffset.Text);
                                            break;
                                        case 3:
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingTolerance_Bottom[m_intSelectedCornerDirectionIndex] = Convert.ToInt32(txt_CornerOffset.Text);
                                            break;
                                        case 4:
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerSearchingTolerance_Left[m_intSelectedCornerDirectionIndex] = Convert.ToInt32(txt_CornerOffset.Text);
                                            break;
                                    }
                                }
                            }
                        }
                        trackBar_CornerOffset.Value = Convert.ToInt32(txt_CornerOffset.Text);
                        break;
                }
            }
            //ROI objROI = new ROI();
            //objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            //objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth,
            //    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageHeight);
            //switch (m_intSelectedCatergory)
            //{
            //    case 0:
            //        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objCenterPointGauge.Measure(objROI);
            //        break;
            //    case 1:
            //        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSidePointGauge.Measure(objROI);
            //        break;
            //    case 2:
            //        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objCornerPointGauge.Measure(objROI);
            //        break;
            //    default:
            //        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.Measure(objROI);
            //        break;
            //}
            //m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(objROI);
            //objROI.Dispose();
            //// Update histogram
            //SetSettingPointGaugePosition();
            //m_blnUpdateHistogram = true;
            m_blnBuildLead = true;
            //m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }

        private void trackBar_CornerLength_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

             txt_CornerLength.Text = trackBar_CornerLength.Value.ToString();
        }

        private void trackBar_CornerOffset_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
             txt_CornerOffset.Text = trackBar_CornerOffset.Value.ToString();
        }

        private void chk_SetToAll_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            if (sender == chk_SetToAll_Side)
            {
                subkey.SetValue("SetToAllPointGauge_Lead3DSide", chk_SetToAll_Side.Checked);
                m_smVisionInfo.g_blnSetToAllROIs_Side = chk_SetToAll_Side.Checked;
            }
            else if (sender == chk_SetToAll_Corner)
            {
                subkey.SetValue("SetToAllPointGauge_Lead3DCorner", chk_SetToAll_Corner.Checked);
                m_smVisionInfo.g_blnSetToAllROIs_Corner = chk_SetToAll_Corner.Checked;
                UpdateLocalROI();
            }

            m_smVisionInfo.g_blnDrawThresholdAllSideROI = ((CheckBox)sender).Checked;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void TriggerOfflineTest()
        {
            m_smVisionInfo.g_blnViewPointGauge = false;

            // 29-07-2019 ZJYEOH : If user pressed test button, then Image should stop live
            if (m_smVisionInfo.AT_PR_StartLiveImage)
            {
                m_smVisionInfo.AT_PR_StartLiveImage = false;
                m_smVisionInfo.AT_PR_TriggerLiveImage = true;
            }

            m_smVisionInfo.g_blnViewPHImage = false;
            m_smVisionInfo.g_blnCheckPH = false;

            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                    m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            }

            m_smVisionInfo.AT_VM_ManualTestMode = true;

            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
        }

        private void chk_AllLeads_Center_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnSetToAllLeadPad_Center = chk_AllLeads_Center.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_AllPoints_Center_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnSetToAllPoints_Center = chk_AllPoints_Center.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_AllLeads_Side_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnSetToAllLeadPad_Side = chk_AllLeads_Side.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_AllPoints_Side_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnSetToAllPoints_Side = chk_AllPoints_Side.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_AllPoints_Corner_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnSetToAllPoints_Corner = chk_AllPoints_Corner.Checked;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MinAmplitude_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            switch (m_eReadFrom)
            {
                case ReadFrom.Lead3D:
                    if (m_smVisionInfo.g_arrLead3D.Length > m_smVisionInfo.g_intSelectedROI)
                    {
                        switch (m_intSelectedCatergory)
                        {
                            case 0:
                                m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Center.SelectedIndex;
                                if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                                    m_smVisionInfo.g_intLeadSelectedNumber = 0;

                                if (m_smVisionInfo.g_blnSetToAllLeadPad_Center)
                                {
                                    for (int L = 0; L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].GetNumberOfLead(); L++)
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Center)
                                        {
                                            for (int p = 0; p < 6; p++)
                                            {
                                                if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                            p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L][p].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                                }
                                            }
                                        }
                                        else
                                              if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                            m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L].Count)
                                        {
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[L][m_intSelectedCenterDirectionIndex].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints_Center)
                                    {
                                        for (int p = 0; p < 6; p++)
                                        {
                                            if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                        p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                            }
                                        }
                                    }
                                    else
                                               if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                                             m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                    {
                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                    }
                                }
                                break;
                            case 1:
                                m_smVisionInfo.g_intLeadSelectedNumber = cbo_LeadNo_Side.SelectedIndex;
                                if (m_smVisionInfo.g_intLeadSelectedNumber < 0)
                                    m_smVisionInfo.g_intLeadSelectedNumber = 0;

                                if (m_smVisionInfo.g_blnSetToAllROIs_Side)
                                {
                                    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllLeadPad_Side)
                                        {
                                            for (int L = 0; L < m_smVisionInfo.g_arrLead3D[i].GetNumberOfLead(); L++)
                                            {
                                                if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                                {
                                                    for (int p = 0; p < 3; p++)
                                                    {
                                                        if (L < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                      p < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L].Count)
                                                        {
                                                            m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L][p].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (L < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                  m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L].Count)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[L][m_intSelectedSideDirectionIndex].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                            {
                                                for (int p = 0; p < 3; p++)
                                                {
                                                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                                  p < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge.Count &&
                                              m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_blnSetToAllLeadPad_Side)
                                    {
                                        for (int L = 0; L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].GetNumberOfLead(); L++)
                                        {
                                            if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                            {
                                                for (int p = 0; p < 3; p++)
                                                {
                                                    if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                                  p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L].Count)
                                                    {
                                                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L][p].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (L < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                              m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[L][m_intSelectedSideDirectionIndex].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Side)
                                        {
                                            for (int p = 0; p < 3; p++)
                                            {
                                                if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                              p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][p].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                          m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                            }
                                        }
                                    }
                                }
                                break;
                            case 2:
                                if (m_smVisionInfo.g_blnSetToAllROIs_Corner)
                                {
                                    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                    {
                                        if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                                        {
                                            for (int p = 0; p < 2; p++)
                                            {
                                                if (p < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                                {
                                                    m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge[p].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge.Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[i].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (m_smVisionInfo.g_blnSetToAllPoints_Corner)
                                    {
                                        for (int p = 0; p < 2; p++)
                                        {
                                            if (p < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                            {
                                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[p].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                                        {
                                            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                        }
                                    }
                                }
                                break;
                            default:
                                m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                                break;
                        }
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MinAmplitude.Text);
                        trackBar_MinAmplitude.Value = Convert.ToInt32(txt_MinAmplitude.Text);
                    }
                    break;
            }
            //ROI objROI = new ROI();
            //objROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            //objROI.LoadROISetting(0, 0, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageWidth,
            //    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].ref_intImageHeight);
            switch (m_intSelectedCatergory)
            {
                case 0:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge.Count &&
                           m_intSelectedCenterDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCenterPointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedCenterDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 1:
                    if (m_smVisionInfo.g_intLeadSelectedNumber < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge.Count &&
                                 m_intSelectedSideDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber].Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrSidePointGauge[m_smVisionInfo.g_intLeadSelectedNumber][m_intSelectedSideDirectionIndex].Measure(m_objROI);
                    }
                    break;
                case 2:
                    if (m_intSelectedCornerDirectionIndex < m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge.Count)
                    {
                        m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_arrCornerPointGauge[m_intSelectedCornerDirectionIndex].Measure(m_objThresholdROI);
                    }
                    break;
                default:
                    m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objPointGauge.Measure(m_objROI);
                    break;
            }
            m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_objSettingPointGauge.Measure(m_objROI);
            //objROI.Dispose();
            // Update histogram
            m_blnUpdateHistogram = true;
            m_blnBuildLead = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (m_intReadFromIndex == 1)
                m_blnTriggerOfflineTest = true;
        }

        private void trackBar_MinAmplitude_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MinAmplitude.Text = trackBar_MinAmplitude.Value.ToString();
        }
    }
}
