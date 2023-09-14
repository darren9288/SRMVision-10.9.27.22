using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SharedMemory;
using VisionProcessing;
using Common;

namespace VisionProcessForm
{
    public partial class AdvancedRectGaugeM4LForm : Form
    {
        #region enum

        public enum ReadFrom { Pad = 0, Calibration = 1 };

        #endregion

        #region Member Variables
        private Point m_pPreviousLocation;
        private int m_intGaugeTolerance = 3;
        private bool m_blnAutoTuneForAllEdges = false;
        private float m_fSampleScore = 0;
        private float m_fDistance1 = float.MaxValue;
        private float m_fDistance2 = float.MaxValue;
        private float m_fAngle = float.MaxValue;
        private int m_intThickness = 10;
        private int m_intDerivative = 5;
        private int m_intThreshold = 5;
        private int m_intThreshold_Temp = 5;
        private int m_intThickness_Temp = 10, m_intDerivative_Temp = 5;
        private float[] m_arrAngle = new float[4] { 0, 0, 0, 0 };
        private PointF[] m_arrPosition1 = new PointF[4] { new PointF(0, 0), new PointF(0, 0), new PointF(0, 0), new PointF(0, 0) };
        private PointF[] m_arrPosition2 = new PointF[4] { new PointF(0, 0), new PointF(0, 0), new PointF(0, 0), new PointF(0, 0) };
        PGauge m_PointGauge;
        RectGaugeM4L m_RGaugeM4L;
        private List<int> m_arrSampledPointsCount = new List<int>();
        private bool m_blnInitDone = false;
        private bool m_blnWantTopMost = true;
        private bool m_blnSetToAll = true;
        private bool m_blnShowAdvanceSetting = false;
        private bool m_blnShowGraph = false;
        private bool m_blnBlockOtherControlUpdate = false;
        private int m_intEdgeEnableMask = 0;

        private ReadFrom m_eReadFrom = ReadFrom.Pad;    // Default
        private int m_intLineGaugeSelectedIndex = 0;
        private string m_strPath = "";
        private VisionInfo m_smVisionInfo;
        private bool m_blnUpdateHistogram = false;
        private string m_path;
        private Graphics m_Graphic;
        //private PGauge m_objPointGauge;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private SRMWaitingFormThread m_thWaitingFormThread;
        private bool m_blnShowForm = false;
        #endregion

        #region Properties
        public bool ref_blnShowForm { get { return m_blnShowForm; } }
        #endregion


        public AdvancedRectGaugeM4LForm(VisionInfo smVisionInfo, string strPath, string Path, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            InitializeComponent();
            m_Graphic = Graphics.FromHwnd(pic_Histogram.Handle);
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            //m_smVisionInfo.g_blncboImageView = false;
            m_strPath = strPath;
            m_path = Path;
            m_smProductionInfo = smProductionInfo;
            UpdateGUI();
            UpdatePackageImage();

            m_blnInitDone = true;
            
        }
        
        public AdvancedRectGaugeM4LForm(VisionInfo smVisionInfo, string strPath, ReadFrom eReadFrom, string Path, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo)
        {
            InitializeComponent();
           
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_strPath = strPath;
            m_eReadFrom = eReadFrom;
            m_path = Path;
            m_smProductionInfo = smProductionInfo;
            UpdateGUI();

            m_blnInitDone = true;
        }



        private void UpdateGUI()
        {
            // Top is selected by default
            m_intLineGaugeSelectedIndex = 0;
            radioBtn_Up.Checked = true;

            if (!(m_smVisionInfo.g_strVisionName.Contains("BottomOrient") || m_smVisionInfo.g_strVisionName.Contains("BottomPosition")))
                cbo_GaugeMeasureMode.Items.RemoveAt(4);

            // update picture box
            //int intPolarity = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeTransType(m_intLineGaugeSelectedIndex);
            //int intDirection = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeTransChoice(m_intLineGaugeSelectedIndex);
            //if (intPolarity == 0 && intDirection == 0)
            //{
            //    pic_Up.Image = ils_ImageListTree.Images[0];
            //}

            // Update Image No combo box
            cbo_ImageNo.Items.Clear();
            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                cbo_ImageNo.Items.Add((i + 1).ToString());
            }
            m_smVisionInfo.g_intSelectedImage = cbo_ImageNo.SelectedIndex = 0;

            cbo_ImageNoTop.Items.Clear();
            cbo_ImageNoRight.Items.Clear();
            cbo_ImageNoBottom.Items.Clear();
            cbo_ImageNoLeft.Items.Clear();

            for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
            {
                cbo_ImageNoTop.Items.Add("Image " + (i + 1).ToString());
                cbo_ImageNoRight.Items.Add("Image " + (i + 1).ToString());
                cbo_ImageNoBottom.Items.Add("Image " + (i + 1).ToString());
                cbo_ImageNoLeft.Items.Add("Image " + (i + 1).ToString());
            }
            
            UpdateGaugeToGUI();

            m_smVisionInfo.g_intSelectedImage = cbo_ImageNoTop.SelectedIndex = Math.Min(m_smVisionInfo.g_arrImages.Count - 1,m_RGaugeM4L.GetGaugeImageNo(0));
            int intImageNo = m_RGaugeM4L.GetGaugeImageNo(1);
            if (intImageNo >= cbo_ImageNoRight.Items.Count)
                intImageNo = 0;
            cbo_ImageNoRight.SelectedIndex = intImageNo;

            intImageNo = m_RGaugeM4L.GetGaugeImageNo(2);
            if (intImageNo >= cbo_ImageNoRight.Items.Count)
                intImageNo = 0;
            cbo_ImageNoBottom.SelectedIndex = intImageNo;

            intImageNo = m_RGaugeM4L.GetGaugeImageNo(3);
            if (intImageNo >= cbo_ImageNoRight.Items.Count)
                intImageNo = 0;
            cbo_ImageNoLeft.SelectedIndex = intImageNo;

            DefineEdgeEnableMask();

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            panel_AdvanceSetting.Visible = false;
            panel_Graph.Visible = false;

            if (m_smVisionInfo.g_arrPackageGaugeM4L.Count > 0)
                chk_ShowSamplePoints.Checked = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_blnDrawSamplingPoint;

            this.Size = new Size(this.Size.Width, this.Size.Height - panel_AdvanceSetting.Size.Height - 102);//panel_Graph.Size.Height --> 102
        }

        private void UpdateGaugeToGUI()
        {
            m_PointGauge = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_objPointGauge;
            m_RGaugeM4L = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit];
            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_smVisionInfo.g_intSelectedUnit < m_smVisionInfo.g_arrPackageGauge2M4L.Count)
                {
                    m_PointGauge = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].ref_objPointGauge;
                    m_RGaugeM4L = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit];

                    txt_ImageGain.Enabled = true;

                    // ----- Image Gain -----
                    txt_ImageGain.Value = Convert.ToDecimal(m_RGaugeM4L.GetGaugeImageGain(m_intLineGaugeSelectedIndex));

                    // ----- Image Threshold -----
                    chk_EnableThreshold.Enabled = true;
                    chk_EnableThreshold.Checked = m_RGaugeM4L.GetGaugeWantImageThreshold(m_intLineGaugeSelectedIndex);
                    txt_ImageThreshold.Enabled = chk_EnableThreshold.Checked;
                    txt_ImageThreshold.Value = Convert.ToDecimal(m_RGaugeM4L.GetGaugeImageThreshold(m_intLineGaugeSelectedIndex));

                    txt_Threshold.Enabled = chk_EnableThreshold.Checked;
                    txt_Threshold.Text = m_RGaugeM4L.GetGaugeImageThreshold(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_Threshold.Value = Convert.ToInt32(txt_Threshold.Text);

                    if (m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeTransType(m_intLineGaugeSelectedIndex) == 0)
                        radioBtn_BW.Checked = true;
                    else
                        radioBtn_WB.Checked = true;

                    if (m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeDirection(m_intLineGaugeSelectedIndex))//IsDirectionInToOut()
                        radioBtn_InOut.Checked = true;
                    else
                        radioBtn_OutIn.Checked = true;

                    if (m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeTransChoice(m_intLineGaugeSelectedIndex) == 0)
                        radioBtn_FromBegin.Checked = true;
                    else if (m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeTransChoice(m_intLineGaugeSelectedIndex) == 1)
                        radioBtn_FromEnd.Checked = true;
                    else
                        radioBtn_LargeAmplitude.Checked = true;

                    cbo_ImageMode.SelectedIndex = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeImageMode(m_intLineGaugeSelectedIndex);
                    txt_MeasThickness.Text = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeThickness(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
                    txt_MeasFilter.Text = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeFilter(m_intLineGaugeSelectedIndex).ToString();
                    txt_MeasMinAmp.Text = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMinAmplitude(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
                    txt_MeasMinArea.Text = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMinArea(m_intLineGaugeSelectedIndex).ToString();
                    txt_FilteringPass.Text = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeFilterPasses(m_intLineGaugeSelectedIndex).ToString();
                    txt_FilteringThreshold.Text = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeFilterThreshold(m_intLineGaugeSelectedIndex).ToString();
                    txt_Derivative.Text = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeThreshold(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_Derivative.Value = Convert.ToInt32(txt_Derivative.Text);
                    txt_SamplingStep.Text = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeSamplingStep(m_intLineGaugeSelectedIndex).ToString();
                    txt_MinScore.Text = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMinScore(m_intLineGaugeSelectedIndex).ToString();
                    txt_MaxAngle.Text = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMaxAngle(m_intLineGaugeSelectedIndex).ToString();
                    txt_PointGaugeOffset.Text = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetPointGaugeOffset(m_intLineGaugeSelectedIndex).ToString();
                    txt_GroupTol.Text = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeGroupTolerance(m_intLineGaugeSelectedIndex).ToString();
                    chk_EnableSubGauge.Checked = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeEnableSubGauge(m_intLineGaugeSelectedIndex);
                    cbo_GaugeMeasureMode.SelectedIndex = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMeasureMode(m_intLineGaugeSelectedIndex);
                    chk_ReCheckUsingPointGaugeIfFail.Checked = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetReCheckUsingPointGaugeIfFail(m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_smVisionInfo.g_intSelectedUnit < m_smVisionInfo.g_arrOrientGaugeM4L.Count)
                {
                    m_PointGauge = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_objPointGauge;
                    m_RGaugeM4L = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit];

                    txt_ImageGain.Enabled = true;

                    // ----- Image Gain -----
                    txt_ImageGain.Value = Convert.ToDecimal(m_RGaugeM4L.GetGaugeImageGain(m_intLineGaugeSelectedIndex));

                    // ----- Image Threshold -----
                    chk_EnableThreshold.Enabled = true;
                    chk_EnableThreshold.Checked = m_RGaugeM4L.GetGaugeWantImageThreshold(m_intLineGaugeSelectedIndex);
                    txt_ImageThreshold.Enabled = chk_EnableThreshold.Checked;
                    txt_ImageThreshold.Value = Convert.ToDecimal(m_RGaugeM4L.GetGaugeImageThreshold(m_intLineGaugeSelectedIndex));

                    txt_Threshold.Enabled = chk_EnableThreshold.Checked;
                    txt_Threshold.Text = m_RGaugeM4L.GetGaugeImageThreshold(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_Threshold.Value = Convert.ToInt32(txt_Threshold.Text);

                    if (m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeTransType(m_intLineGaugeSelectedIndex) == 0)
                        radioBtn_BW.Checked = true;
                    else
                        radioBtn_WB.Checked = true;

                    if (m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeDirection(m_intLineGaugeSelectedIndex))//IsDirectionInToOut()
                        radioBtn_InOut.Checked = true;
                    else
                        radioBtn_OutIn.Checked = true;

                    if (m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeTransChoice(m_intLineGaugeSelectedIndex) == 0)
                        radioBtn_FromBegin.Checked = true;
                    else if (m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeTransChoice(m_intLineGaugeSelectedIndex) == 1)
                        radioBtn_FromEnd.Checked = true;
                    else
                        radioBtn_LargeAmplitude.Checked = true;

                    cbo_ImageMode.SelectedIndex = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeImageMode(m_intLineGaugeSelectedIndex);
                    txt_MeasThickness.Text = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeThickness(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
                    txt_MeasFilter.Text = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeFilter(m_intLineGaugeSelectedIndex).ToString();
                    txt_MeasMinAmp.Text = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMinAmplitude(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
                    txt_MeasMinArea.Text = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMinArea(m_intLineGaugeSelectedIndex).ToString();
                    txt_FilteringPass.Text = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeFilterPasses(m_intLineGaugeSelectedIndex).ToString();
                    txt_FilteringThreshold.Text = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeFilterThreshold(m_intLineGaugeSelectedIndex).ToString();
                    txt_Derivative.Text = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeThreshold(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_Derivative.Value = Convert.ToInt32(txt_Derivative.Text);
                    txt_SamplingStep.Text = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeSamplingStep(m_intLineGaugeSelectedIndex).ToString();
                    txt_MinScore.Text = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMinScore(m_intLineGaugeSelectedIndex).ToString();
                    txt_MaxAngle.Text = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMaxAngle(m_intLineGaugeSelectedIndex).ToString();
                    txt_PointGaugeOffset.Text = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetPointGaugeOffset(m_intLineGaugeSelectedIndex).ToString();
                    txt_GroupTol.Text = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeGroupTolerance(m_intLineGaugeSelectedIndex).ToString();
                    chk_EnableSubGauge.Checked = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeEnableSubGauge(m_intLineGaugeSelectedIndex);
                    cbo_GaugeMeasureMode.SelectedIndex = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMeasureMode(m_intLineGaugeSelectedIndex);
                    chk_ReCheckUsingPointGaugeIfFail.Checked = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetReCheckUsingPointGaugeIfFail(m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_smVisionInfo.g_intSelectedUnit < m_smVisionInfo.g_arrPackageGaugeM4L.Count)
                {
                    m_PointGauge = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_objPointGauge;
                    m_RGaugeM4L = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit];

                    txt_ImageGain.Enabled = true;
                    
                    // ----- Image Gain -----
                    txt_ImageGain.Value = Convert.ToDecimal(m_RGaugeM4L.GetGaugeImageGain(m_intLineGaugeSelectedIndex));
                    txt_Gain.Text = m_RGaugeM4L.GetGaugeImageGain(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_Gain.Value = Convert.ToInt32(Convert.ToSingle(txt_Gain.Text) * 10f);

                    // ----- Image Threshold -----
                    chk_EnableThreshold.Enabled = true;

                    chk_EnableThreshold.Checked = m_RGaugeM4L.GetGaugeWantImageThreshold(m_intLineGaugeSelectedIndex);
                    txt_ImageThreshold.Enabled = chk_EnableThreshold.Checked;
                    txt_ImageThreshold.Value = Convert.ToDecimal(m_RGaugeM4L.GetGaugeImageThreshold(m_intLineGaugeSelectedIndex));

                    txt_Threshold.Enabled = chk_EnableThreshold.Checked;
                    txt_Threshold.Text = m_RGaugeM4L.GetGaugeImageThreshold(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_Threshold.Value = Convert.ToInt32(txt_Threshold.Text); 

                    if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeTransType(m_intLineGaugeSelectedIndex) == 0)
                        radioBtn_BW.Checked = true;
                    else
                        radioBtn_WB.Checked = true;

                    if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeDirection(m_intLineGaugeSelectedIndex))//IsDirectionInToOut()
                        radioBtn_InOut.Checked = true;
                    else
                        radioBtn_OutIn.Checked = true;

                    if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeTransChoice(m_intLineGaugeSelectedIndex) == 0)
                        radioBtn_FromBegin.Checked = true;
                    else if (m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeTransChoice(m_intLineGaugeSelectedIndex) == 1)
                        radioBtn_FromEnd.Checked = true;
                    else
                        radioBtn_LargeAmplitude.Checked = true;

                    cbo_ImageMode.SelectedIndex = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeImageMode(m_intLineGaugeSelectedIndex);
                    txt_MeasThickness.Text = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeThickness(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
                    txt_MeasFilter.Text = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeFilter(m_intLineGaugeSelectedIndex).ToString();
                    txt_MeasMinAmp.Text = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMinAmplitude(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
                    txt_MeasMinArea.Text = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMinArea(m_intLineGaugeSelectedIndex).ToString();
                    txt_FilteringPass.Text = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeFilterPasses(m_intLineGaugeSelectedIndex).ToString();
                    txt_FilteringThreshold.Text = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeFilterThreshold(m_intLineGaugeSelectedIndex).ToString();
                    txt_Derivative.Text = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeThreshold(m_intLineGaugeSelectedIndex).ToString();
                    trackBar_Derivative.Value = Convert.ToInt32(txt_Derivative.Text);
                    txt_SamplingStep.Text = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeSamplingStep(m_intLineGaugeSelectedIndex).ToString();
                    txt_MinScore.Text = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMinScore(m_intLineGaugeSelectedIndex).ToString();
                    txt_MaxAngle.Text = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMaxAngle(m_intLineGaugeSelectedIndex).ToString();
                    txt_PointGaugeOffset.Text = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetPointGaugeOffset(m_intLineGaugeSelectedIndex).ToString();
                    txt_GroupTol.Text = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeGroupTolerance(m_intLineGaugeSelectedIndex).ToString();
                    chk_EnableSubGauge.Checked = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeEnableSubGauge(m_intLineGaugeSelectedIndex);
                    cbo_GaugeMeasureMode.SelectedIndex = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetGaugeMeasureMode(m_intLineGaugeSelectedIndex);
                    chk_ReCheckUsingPointGaugeIfFail.Checked = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetReCheckUsingPointGaugeIfFail(m_intLineGaugeSelectedIndex);
                }
            }

            chk_ReCheckUsingPointGaugeIfFail.Visible = btn_SaveAndSetSizeAsReference.Visible = cbo_GaugeMeasureMode.SelectedIndex == 3;

            m_PointGauge.ref_GaugeTransType = m_RGaugeM4L.GetGaugeTransType(m_intLineGaugeSelectedIndex);
            m_PointGauge.ref_GaugeTransChoice = m_RGaugeM4L.GetGaugeTransChoice(m_intLineGaugeSelectedIndex);
            m_PointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_Derivative.Text);
            m_PointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
            m_PointGauge.ref_GaugeMinArea = Convert.ToInt32(txt_MeasMinArea.Text);
            m_PointGauge.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp.Text);
            m_PointGauge.ref_GaugeFilter = Convert.ToInt32(txt_MeasFilter.Text);

            m_PointGauge.SetGaugePlacement(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterX,
                m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterY,
                 m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeTolerance, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeAngle+90);

            m_PointGauge.Measure(m_RGaugeM4L.ref_arrEdgeROI[m_intLineGaugeSelectedIndex]);
            UpdateGUIPicture();
            m_blnUpdateHistogram = true;
        }
        private void UpdateGUIPicture()
        {
            //if (m_blnSetToAll)
            //{
            //    if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
            //    {
            //        if ((m_intEdgeEnableMask & (0x01 << 0)) > 0)
            //            pic_Up.Image = ils_ImageListTree.Images[0];
            //        if ((m_intEdgeEnableMask & (0x01 << 1)) > 0)
            //            pic_Right.Image = ils_ImageListTree.Images[1];
            //        if ((m_intEdgeEnableMask & (0x01 << 2)) > 0)
            //            pic_Down.Image = ils_ImageListTree.Images[2];
            //        if ((m_intEdgeEnableMask & (0x01 << 3)) > 0)
            //            pic_Left.Image = ils_ImageListTree.Images[3];
            //    }
            //    else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
            //    {
            //        if ((m_intEdgeEnableMask & (0x01 << 0)) > 0)
            //            pic_Up.Image = ils_ImageListTree.Images[2];
            //        if ((m_intEdgeEnableMask & (0x01 << 1)) > 0)
            //            pic_Right.Image = ils_ImageListTree.Images[3];
            //        if ((m_intEdgeEnableMask & (0x01 << 2)) > 0)
            //            pic_Down.Image = ils_ImageListTree.Images[0];
            //        if ((m_intEdgeEnableMask & (0x01 << 3)) > 0)
            //            pic_Left.Image = ils_ImageListTree.Images[1];
            //    }
            //    else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
            //    {
            //        if ((m_intEdgeEnableMask & (0x01 << 0)) > 0)
            //            pic_Up.Image = ils_ImageListTree.Images[4];
            //        if ((m_intEdgeEnableMask & (0x01 << 1)) > 0)
            //            pic_Right.Image = ils_ImageListTree.Images[5];
            //        if ((m_intEdgeEnableMask & (0x01 << 2)) > 0)
            //            pic_Down.Image = ils_ImageListTree.Images[6];
            //        if ((m_intEdgeEnableMask & (0x01 << 3)) > 0)
            //            pic_Left.Image = ils_ImageListTree.Images[7];
            //    }
            //    else
            //    {
            //        if ((m_intEdgeEnableMask & (0x01 << 0)) > 0)
            //            pic_Up.Image = ils_ImageListTree.Images[6];
            //        if ((m_intEdgeEnableMask & (0x01 << 1)) > 0)
            //            pic_Right.Image = ils_ImageListTree.Images[7];
            //        if ((m_intEdgeEnableMask & (0x01 << 2)) > 0)
            //            pic_Down.Image = ils_ImageListTree.Images[4];
            //        if ((m_intEdgeEnableMask & (0x01 << 3)) > 0)
            //            pic_Left.Image = ils_ImageListTree.Images[5];
            //    }
            //}
            //else
            {
                //switch (m_intLineGaugeSelectedIndex)
                //{
                //    case 0: //Top
                //        if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                //            pic_Up.Image = ils_ImageListTree.Images[0];
                //        else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                //            pic_Up.Image = ils_ImageListTree.Images[2];
                //        else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                //            pic_Up.Image = ils_ImageListTree.Images[4];
                //        else
                //            pic_Up.Image = ils_ImageListTree.Images[6];
                //        break;
                //    case 1: // Right
                //        if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                //            pic_Right.Image = ils_ImageListTree.Images[1];
                //        else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                //            pic_Right.Image = ils_ImageListTree.Images[3];
                //        else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                //            pic_Right.Image = ils_ImageListTree.Images[5];
                //        else
                //            pic_Right.Image = ils_ImageListTree.Images[7];
                //        break;
                //    case 2: //Bottom
                //        if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                //            pic_Down.Image = ils_ImageListTree.Images[2];
                //        else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                //            pic_Down.Image = ils_ImageListTree.Images[0];
                //        else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                //            pic_Down.Image = ils_ImageListTree.Images[6];
                //        else
                //            pic_Down.Image = ils_ImageListTree.Images[4];
                //        break;
                //    case 3://Left
                //        if (radioBtn_BW.Checked && radioBtn_OutIn.Checked)
                //            pic_Left.Image = ils_ImageListTree.Images[3];
                //        else if (radioBtn_BW.Checked && radioBtn_InOut.Checked)
                //            pic_Left.Image = ils_ImageListTree.Images[1];
                //        else if (radioBtn_WB.Checked && radioBtn_OutIn.Checked)
                //            pic_Left.Image = ils_ImageListTree.Images[7];
                //        else
                //            pic_Left.Image = ils_ImageListTree.Images[5];
                //        break;
                //}

                //Top
                if ((m_RGaugeM4L.GetGaugeTransType(0) == 0) && (!m_RGaugeM4L.GetGaugeDirection(0)))// IsDirectionInToOut()
                    pic_Up.Image = ils_ImageListTree.Images[0];
                else if ((m_RGaugeM4L.GetGaugeTransType(0) == 0) && (m_RGaugeM4L.GetGaugeDirection(0)))// IsDirectionInToOut()
                    pic_Up.Image = ils_ImageListTree.Images[2];
                else if ((m_RGaugeM4L.GetGaugeTransType(0) == 1) && (!m_RGaugeM4L.GetGaugeDirection(0)))// IsDirectionInToOut()
                    pic_Up.Image = ils_ImageListTree.Images[4];
                else
                    pic_Up.Image = ils_ImageListTree.Images[6];

                // Right
                if ((m_RGaugeM4L.GetGaugeTransType(1) == 0) && (!m_RGaugeM4L.GetGaugeDirection(1)))// IsDirectionInToOut()
                    pic_Right.Image = ils_ImageListTree.Images[1];
                else if ((m_RGaugeM4L.GetGaugeTransType(1) == 0) && (m_RGaugeM4L.GetGaugeDirection(1)))// IsDirectionInToOut()
                    pic_Right.Image = ils_ImageListTree.Images[3];
                else if ((m_RGaugeM4L.GetGaugeTransType(1) == 1) && (!m_RGaugeM4L.GetGaugeDirection(1)))// IsDirectionInToOut()
                    pic_Right.Image = ils_ImageListTree.Images[5];
                else
                    pic_Right.Image = ils_ImageListTree.Images[7];

                //Bottom
                if ((m_RGaugeM4L.GetGaugeTransType(2) == 0) && (!m_RGaugeM4L.GetGaugeDirection(2)))// IsDirectionInToOut()
                    pic_Down.Image = ils_ImageListTree.Images[2];
                else if ((m_RGaugeM4L.GetGaugeTransType(2) == 0) && (m_RGaugeM4L.GetGaugeDirection(2)))// IsDirectionInToOut()
                    pic_Down.Image = ils_ImageListTree.Images[0];
                else if ((m_RGaugeM4L.GetGaugeTransType(2) == 1) && (!m_RGaugeM4L.GetGaugeDirection(2)))// IsDirectionInToOut()
                    pic_Down.Image = ils_ImageListTree.Images[6];
                else
                    pic_Down.Image = ils_ImageListTree.Images[4];

                //Left
                if ((m_RGaugeM4L.GetGaugeTransType(3) == 0) && (!m_RGaugeM4L.GetGaugeDirection(3)))// IsDirectionInToOut()
                    pic_Left.Image = ils_ImageListTree.Images[3];
                else if ((m_RGaugeM4L.GetGaugeTransType(3) == 0) && (m_RGaugeM4L.GetGaugeDirection(3)))// IsDirectionInToOut()
                    pic_Left.Image = ils_ImageListTree.Images[1];
                else if ((m_RGaugeM4L.GetGaugeTransType(3) == 1) && (!m_RGaugeM4L.GetGaugeDirection(3)))// IsDirectionInToOut()
                    pic_Left.Image = ils_ImageListTree.Images[7];
                else
                    pic_Left.Image = ils_ImageListTree.Images[5];
            }
        }

        private void radioBtn_Left_Click(object sender, EventArgs e)
        {
            m_blnBlockOtherControlUpdate = true;

            if (sender == radioBtn_Up)
            {
                m_intLineGaugeSelectedIndex = 0;
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoTop.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                DefineEdgeEnableMask(cbo_ImageNoTop.SelectedIndex);
            }
            else if (sender == radioBtn_Right)
            {
                m_intLineGaugeSelectedIndex = 1;
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoRight.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                DefineEdgeEnableMask(cbo_ImageNoRight.SelectedIndex);
            }
            else if (sender == radioBtn_Down)
            {
                m_intLineGaugeSelectedIndex = 2;
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoBottom.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                DefineEdgeEnableMask(cbo_ImageNoBottom.SelectedIndex);
            }
            else if (sender == radioBtn_Left)
            {
                m_intLineGaugeSelectedIndex = 3;
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoLeft.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                DefineEdgeEnableMask(cbo_ImageNoLeft.SelectedIndex);
            }

            UpdateSelectedGaugeEdgeMask();

            UpdateGaugeToGUI();

            UpdatePackageImage();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_blnBlockOtherControlUpdate = false;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].ref_intSelectedGaugeEdgeMask = 0x0F;
                m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].LoadRectGauge4L(m_strPath, "RectG" + m_smVisionInfo.g_intSelectedUnit.ToString());
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_intSelectedGaugeEdgeMask = 0x0F;
                m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].LoadRectGauge4L(m_strPath, "RectG" + m_smVisionInfo.g_intSelectedUnit.ToString());
            }
            else
            {
                m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_intSelectedGaugeEdgeMask = 0x0F;
                m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].LoadRectGauge4L(m_strPath, "RectG" + m_smVisionInfo.g_intSelectedUnit.ToString());
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            Close();
            Dispose();
        }

        private void txt_MeasMinAmp_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinAmplitude(Convert.ToInt32(txt_MeasMinAmp.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinAmplitude(Convert.ToInt32(txt_MeasMinAmp.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinAmplitude(Convert.ToInt32(txt_MeasMinAmp.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinAmplitude(Convert.ToInt32(txt_MeasMinAmp.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinAmplitude(Convert.ToInt32(txt_MeasMinAmp.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinAmplitude(Convert.ToInt32(txt_MeasMinAmp.Text), m_intLineGaugeSelectedIndex);
                }
            }
            trackBar_MinAmp.Value = Convert.ToInt32(txt_MeasMinAmp.Text);
            
            m_PointGauge.ref_GaugeMinAmplitude = Convert.ToInt32(txt_MeasMinAmp.Text);

            m_PointGauge.SetGaugePlacement(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterX,
                m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterY,
                 m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeTolerance, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeAngle + 90);

            m_PointGauge.Measure(m_RGaugeM4L.ref_arrEdgeROI[m_intLineGaugeSelectedIndex]);

            m_blnUpdateHistogram = true;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinArea(Convert.ToInt32(txt_MeasMinArea.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinArea(Convert.ToInt32(txt_MeasMinArea.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinArea(Convert.ToInt32(txt_MeasMinArea.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinArea(Convert.ToInt32(txt_MeasMinArea.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinArea(Convert.ToInt32(txt_MeasMinArea.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinArea(Convert.ToInt32(txt_MeasMinArea.Text), m_intLineGaugeSelectedIndex);
                }
            }
            
            m_PointGauge.ref_GaugeMinArea = Convert.ToInt32(txt_MeasMinArea.Text);

            m_PointGauge.SetGaugePlacement(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterX,
                m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterY,
                 m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeTolerance, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeAngle + 90);

            m_PointGauge.Measure(m_RGaugeM4L.ref_arrEdgeROI[m_intLineGaugeSelectedIndex]);

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_threshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;
            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeThreshold(Convert.ToInt32(txt_Derivative.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeThreshold(Convert.ToInt32(txt_Derivative.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeThreshold(Convert.ToInt32(txt_Derivative.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeThreshold(Convert.ToInt32(txt_Derivative.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeThreshold(Convert.ToInt32(txt_Derivative.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeThreshold(Convert.ToInt32(txt_Derivative.Text), m_intLineGaugeSelectedIndex);
                }
            }
            trackBar_Derivative.Value = Convert.ToInt32(txt_Derivative.Text);

            m_PointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_Derivative.Text);

            m_PointGauge.SetGaugePlacement(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterX,
                 m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterY,
                  m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeTolerance, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeAngle + 90);

            m_PointGauge.Measure(m_RGaugeM4L.ref_arrEdgeROI[m_intLineGaugeSelectedIndex]);
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasThickness_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeThickness(Convert.ToInt32(txt_MeasThickness.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeThickness(Convert.ToInt32(txt_MeasThickness.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeThickness(Convert.ToInt32(txt_MeasThickness.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeThickness(Convert.ToInt32(txt_MeasThickness.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeThickness(Convert.ToInt32(txt_MeasThickness.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeThickness(Convert.ToInt32(txt_MeasThickness.Text), m_intLineGaugeSelectedIndex);
                }
            }
            trackBar_Thickness.Value = Convert.ToInt32(txt_MeasThickness.Text);
        
            m_PointGauge.ref_GaugeThickness = Convert.ToInt32(txt_MeasThickness.Text);
            
            m_PointGauge.SetGaugePlacement(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterX,
              m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterY,
               m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeTolerance, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeAngle + 90);

            m_PointGauge.Measure(m_RGaugeM4L.ref_arrEdgeROI[m_intLineGaugeSelectedIndex]);
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MeasFilter_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;
            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilter(Convert.ToInt32(txt_MeasFilter.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilter(Convert.ToInt32(txt_MeasFilter.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilter(Convert.ToInt32(txt_MeasFilter.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilter(Convert.ToInt32(txt_MeasFilter.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilter(Convert.ToInt32(txt_MeasFilter.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilter(Convert.ToInt32(txt_MeasFilter.Text), m_intLineGaugeSelectedIndex);
                }
            }

            m_PointGauge.ref_GaugeFilter = Convert.ToInt32(txt_MeasFilter.Text);

            m_PointGauge.SetGaugePlacement(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterX,
                  m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterY,
                   m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeTolerance, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeAngle + 90);

            m_PointGauge.Measure(m_RGaugeM4L.ref_arrEdgeROI[m_intLineGaugeSelectedIndex]);
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringPass_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;
            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilterPasses(Convert.ToInt32(txt_FilteringPass.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilterPasses(Convert.ToInt32(txt_FilteringPass.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilterPasses(Convert.ToInt32(txt_FilteringPass.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilterPasses(Convert.ToInt32(txt_FilteringPass.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilterPasses(Convert.ToInt32(txt_FilteringPass.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilterPasses(Convert.ToInt32(txt_FilteringPass.Text), m_intLineGaugeSelectedIndex);
                }
            }
        
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_FilteringThreshold_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilterThreshold(Convert.ToInt32(txt_FilteringThreshold.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilterThreshold(Convert.ToSingle(txt_FilteringThreshold.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilterThreshold(Convert.ToInt32(txt_FilteringThreshold.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilterThreshold(Convert.ToSingle(txt_FilteringThreshold.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilterThreshold((float)Convert.ToDecimal(txt_FilteringThreshold.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeFilterThreshold(Convert.ToSingle(txt_FilteringThreshold.Text), m_intLineGaugeSelectedIndex);
                }
            }

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void radioBtn_Polarity_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            int intTransTypeIndex;
            if (radioBtn_BW.Checked)
                intTransTypeIndex = 0;
            else
                intTransTypeIndex = 1;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeTransType(intTransTypeIndex, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeTransType(intTransTypeIndex, m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeTransType(intTransTypeIndex, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeTransType(intTransTypeIndex, m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeTransType(intTransTypeIndex, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeTransType(intTransTypeIndex, m_intLineGaugeSelectedIndex);
                }
            }
            m_PointGauge.ref_GaugeTransType = intTransTypeIndex;
           
            m_PointGauge.SetGaugePlacement(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterX,
                m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterY,
                 m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeTolerance, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeAngle + 90);

            m_PointGauge.Measure(m_RGaugeM4L.ref_arrEdgeROI[m_intLineGaugeSelectedIndex]);
            UpdateGUIPicture();
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_TransType_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void radioBtn_Direction_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetDirection(radioBtn_InOut.Checked, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetDirection(radioBtn_InOut.Checked, m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetDirection(radioBtn_InOut.Checked, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetDirection(radioBtn_InOut.Checked, m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetDirection(radioBtn_InOut.Checked, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetDirection(radioBtn_InOut.Checked, m_intLineGaugeSelectedIndex);
                }
            }
            m_PointGauge.SetGaugePlacement(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterX,
                m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterY,
                 m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeTolerance, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeAngle + 90);

            m_PointGauge.Measure(m_RGaugeM4L.ref_arrEdgeROI[m_intLineGaugeSelectedIndex]);
            UpdateGUIPicture();
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SamplingStep_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeSamplingStep(Convert.ToSingle(txt_SamplingStep.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeSamplingStep(Convert.ToSingle(txt_SamplingStep.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeSamplingStep(Convert.ToSingle(txt_SamplingStep.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeSamplingStep(Convert.ToSingle(txt_SamplingStep.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeSamplingStep(Convert.ToSingle(txt_SamplingStep.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeSamplingStep(Convert.ToSingle(txt_SamplingStep.Text), m_intLineGaugeSelectedIndex);
                }
            }
         
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            SaveSetting(false);

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            Close();
            Dispose();
        }

        private void btn_SaveAndSetSizeAsReference_Click(object sender, EventArgs e)
        {
            SaveSetting(true);

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            Close();
            Dispose();
        }
        private void SaveSetting(bool blnSetSizeAsReference)
        {
            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].ref_intSelectedGaugeEdgeMask = 0x0F;
                m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SaveRectGauge4L(
                       m_strPath,
                       false,
                       "RectG" + m_smVisionInfo.g_intSelectedUnit.ToString(),
                       true,
                       blnSetSizeAsReference);
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_intSelectedGaugeEdgeMask = 0x0F;
                m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SaveRectGauge4L(
                        m_strPath,
                        false,
                        "RectG" + m_smVisionInfo.g_intSelectedUnit.ToString(),
                        true,
                        blnSetSizeAsReference);
            }
            else
            {
                m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_intSelectedGaugeEdgeMask = 0x0F;
                m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SaveRectGauge4L(
                        m_strPath,
                        false,
                        "RectG" + m_smVisionInfo.g_intSelectedUnit.ToString(),
                        true,
                        blnSetSizeAsReference);
            }
        }
        private void btn_ShowAdv_Click(object sender, EventArgs e)
        {
            m_blnShowAdvanceSetting = !m_blnShowAdvanceSetting;

            if (m_blnShowAdvanceSetting)
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    btn_ShowAdv.Text = "Hide Adv.";
                else
                    btn_ShowAdv.Text = "关闭高级";
                panel_AdvanceSetting.Visible = true;
                this.Size = new Size(this.Size.Width, this.Size.Height + panel_AdvanceSetting.Size.Height);
                
                if (m_blnShowGraph)
                {
                    m_blnShowGraph = !m_blnShowGraph;

                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                        btn_ShowGraph.Text = "Show Graph";
                    else
                        btn_ShowGraph.Text = "显示图表";
                    panel_Graph.Visible = false;
                    this.Size = new Size(this.Size.Width, this.Size.Height - panel_Graph.Size.Height);
                }
            }
            else
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    btn_ShowAdv.Text = "Show Adv.";
                else
                    btn_ShowAdv.Text = "显示高级";
                panel_AdvanceSetting.Visible = false;
                this.Size = new Size(this.Size.Width, this.Size.Height - panel_AdvanceSetting.Size.Height);
            }
        }

        private void panel_Setting_Paint(object sender, PaintEventArgs e)
        {

        }

        private int GetLineGaugeSelectedIndex(int intPadIndex)
        {
            switch (intPadIndex)
            {
                case 0:
                case 1: // Top ROI
                    {
                        if (radioBtn_Up.Checked)
                            return 0;
                        else if (radioBtn_Right.Checked)
                            return 1;
                        else if (radioBtn_Down.Checked)
                            return 2;
                        else if (radioBtn_Left.Checked)
                            return 3;
                    }
                    break;
                case 2: // Right ROI
                    {
                        if (radioBtn_Up.Checked)
                            return 1;
                        else if (radioBtn_Right.Checked)
                            return 2;
                        else if (radioBtn_Down.Checked)
                            return 3;
                        else if (radioBtn_Left.Checked)
                            return 0;
                    } 
                    break;
                case 3: // Bottom ROI
                    {
                        if (radioBtn_Up.Checked)
                            return 2;
                        else if (radioBtn_Right.Checked)
                            return 3;
                        else if (radioBtn_Down.Checked)
                            return 0;
                        else if (radioBtn_Left.Checked)
                            return 1;
                    } 
                    break;
                case 4: // Left ROI
                    {
                        if (radioBtn_Up.Checked)
                            return 3;
                        else if (radioBtn_Right.Checked)
                            return 0;
                        else if (radioBtn_Down.Checked)
                            return 1;
                        else if (radioBtn_Left.Checked)
                            return 2;
                    } 
                    break;
            }

            return 0;
        }

        private void btn_ShowGraph_Click(object sender, EventArgs e)
        {
            m_blnShowGraph = !m_blnShowGraph;

            if (m_blnShowGraph)
            {
                m_smVisionInfo.AT_VM_UpdateHistogram = true;
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    btn_ShowGraph.Text = "Hide Graph";
                else
                    btn_ShowGraph.Text = "关闭图表";
                panel_Graph.Visible = true;
                this.Size = new Size(this.Size.Width, this.Size.Height + panel_Graph.Size.Height);


                if (m_blnShowAdvanceSetting)
                {
                    m_blnShowAdvanceSetting = !m_blnShowAdvanceSetting;

                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                        btn_ShowAdv.Text = "Show Adv.";
                    else
                        btn_ShowAdv.Text = "显示高级";
                    panel_AdvanceSetting.Visible = false;
                    this.Size = new Size(this.Size.Width, this.Size.Height - panel_AdvanceSetting.Size.Height);
                }
            }
            else
            {
                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    btn_ShowGraph.Text = "Show Graph";
                else
                    btn_ShowGraph.Text = "显示图表";
                panel_Graph.Visible = false;
                this.Size = new Size(this.Size.Width, this.Size.Height - panel_Graph.Size.Height);
            }
        }

        private void pic_Histogram_Paint(object sender, PaintEventArgs e)
        {
            m_blnUpdateHistogram = true;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (m_blnUpdateHistogram || m_smVisionInfo.AT_VM_UpdateHistogram)
            {
                if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].ref_objPointGauge.DrawLineProfile(m_Graphic, pic_Histogram.Width, pic_Histogram.Height);
                else if (m_smVisionInfo.g_blnViewMOGauge)
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_objPointGauge.DrawLineProfile(m_Graphic, pic_Histogram.Width, pic_Histogram.Height);
                else
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_objPointGauge.DrawLineProfile(m_Graphic, pic_Histogram.Width, pic_Histogram.Height);

                //2020-11-20 ZJYEOH : Update label color to Red if setting not same
                CheckSettingSame();

                m_blnUpdateHistogram = false;
                m_smVisionInfo.AT_VM_UpdateHistogram = false;
            }

            if (m_smVisionInfo.g_blnUpdateSelectedROI)
            {
                m_smVisionInfo.g_blnUpdateSelectedROI = false;

                UpdateGaugeToGUI();
            }

            if (m_blnWantTopMost)
            {
                if (!this.TopMost)
                    this.TopMost = true;
            }
            else
            {
                if (this.TopMost)
                    this.TopMost = false;
            }
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

            txt_Derivative.Text = trackBar_Derivative.Value.ToString();
        }

        private void txt_MinScore_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinScore(Convert.ToInt32(txt_MinScore.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinScore(Convert.ToInt32(txt_MinScore.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinScore(Convert.ToInt32(txt_MinScore.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinScore(Convert.ToInt32(txt_MinScore.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinScore(Convert.ToInt32(txt_MinScore.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMinScore(Convert.ToInt32(txt_MinScore.Text), m_intLineGaugeSelectedIndex);
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MaxAngle_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMaxAngle(Convert.ToInt32(txt_MaxAngle.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMaxAngle(Convert.ToInt32(txt_MaxAngle.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMaxAngle(Convert.ToInt32(txt_MaxAngle.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMaxAngle(Convert.ToInt32(txt_MaxAngle.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMaxAngle(Convert.ToInt32(txt_MaxAngle.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMaxAngle(Convert.ToInt32(txt_MaxAngle.Text), m_intLineGaugeSelectedIndex);
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void radioBtn_Search_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            int intTransChoiceIndex;
            if (radioBtn_FromBegin.Checked)
                intTransChoiceIndex = 0;
            else if (radioBtn_FromEnd.Checked)
                intTransChoiceIndex = 1;
            else
                intTransChoiceIndex = 2;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeTransChoice(intTransChoiceIndex, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeTransChoice(intTransChoiceIndex, m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeTransChoice(intTransChoiceIndex, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeTransChoice(intTransChoiceIndex, m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeTransChoice(intTransChoiceIndex, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeTransChoice(intTransChoiceIndex, m_intLineGaugeSelectedIndex);
                }
            }
            m_PointGauge.ref_GaugeTransChoice = intTransChoiceIndex;

            m_PointGauge.SetGaugePlacement(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterX,
                    m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterY,
                     m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeTolerance, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeAngle + 90);

            m_PointGauge.Measure(m_RGaugeM4L.ref_arrEdgeROI[m_intLineGaugeSelectedIndex]);
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void gb_AdvSetting_Enter(object sender, EventArgs e)
        {

        }

        private void trackBar_MinAmp_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_MeasMinAmp.Text = trackBar_MinAmp.Value.ToString();
        }

        private void btn_UndoDontCareROI_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnWantTopMost = false;

            DontCareAreaSettingForm objDontCareAreaSettingForm = new DontCareAreaSettingForm(m_smVisionInfo, m_smProductionInfo, m_smCustomizeInfo, m_strPath);
            objDontCareAreaSettingForm.ShowDialog();

            m_blnWantTopMost = true;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            objDontCareAreaSettingForm.Dispose();
        }

        private void AdvancedRectGaugeM4LForm_Load(object sender, EventArgs e)
        {
            m_blnShowForm = true;
        }

        private void AdvancedRectGaugeM4LForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_blnShowForm = false;
        }

        private void cbo_ImageNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNo.SelectedIndex, m_smVisionInfo.g_intVisionIndex);

            DefineEdgeEnableMask();

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            UpdatePackageImage();

            m_blnBlockOtherControlUpdate = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void DefineEdgeEnableMask(int intSelectedImageIndex)
        {
            m_intEdgeEnableMask = 0;
            if (m_RGaugeM4L.GetGaugeImageNo(0) == intSelectedImageIndex)
            {
                m_intEdgeEnableMask |= 0x01;
            }

            if (m_RGaugeM4L.GetGaugeImageNo(1) == intSelectedImageIndex)
            {
                m_intEdgeEnableMask |= 0x02;
            }

            if (m_RGaugeM4L.GetGaugeImageNo(2) == intSelectedImageIndex)
            {
                m_intEdgeEnableMask |= 0x04;
            }

            if (m_RGaugeM4L.GetGaugeImageNo(3) == intSelectedImageIndex)
            {
                m_intEdgeEnableMask |= 0x08;
            }
        }
        private void DefineEdgeEnableMask()
        {
            m_intEdgeEnableMask = 0;
            if (m_RGaugeM4L.GetGaugeImageNo(0) == cbo_ImageNoTop.SelectedIndex)
            {
                m_intEdgeEnableMask |= 0x01;
            }

            if (m_RGaugeM4L.GetGaugeImageNo(1) == cbo_ImageNoRight.SelectedIndex)
            {
                m_intEdgeEnableMask |= 0x02;
            }

            if (m_RGaugeM4L.GetGaugeImageNo(2) == cbo_ImageNoBottom.SelectedIndex)
            {
                m_intEdgeEnableMask |= 0x04;
            }

            if (m_RGaugeM4L.GetGaugeImageNo(3) == cbo_ImageNoLeft.SelectedIndex)
            {
                m_intEdgeEnableMask |= 0x08;
            }
        }
        private void UpdatePackageImage()
        {
            if (m_smVisionInfo.g_blnViewRotatedImage)
            {
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            }
            else
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            }

            if (m_smVisionInfo.g_blnViewRotatedImage)
            {
                if (m_blnSetToAll)
                {
                    ImageDrawing objSourceImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            if (m_RGaugeM4L.GetGaugeImageMode(j) == 0)
                            {
                                m_RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, j);
                                if (m_RGaugeM4L.GetGaugeWantImageThreshold(j))
                                    m_RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                            }
                            else
                            {
                                //m_RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, j);
                                //m_RGaugeM4L.AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, j);

                                m_RGaugeM4L.AddHighPassForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, j);
                                m_RGaugeM4L.AddGainForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                                if (m_RGaugeM4L.GetGaugeWantImageThreshold(j))
                                    m_RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                            }
                        }
                    }
                }
                else
                {
                    int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(0);
                    ImageDrawing objSourceImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
                    if (m_RGaugeM4L.GetGaugeImageMode(intLineGaugeSelectedIndex) == 0)
                    {
                        m_RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                        if (m_RGaugeM4L.GetGaugeWantImageThreshold(intLineGaugeSelectedIndex))
                            m_RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);

                    }
                    else
                    {
                        //m_RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, intLineGaugeSelectedIndex);
                        //m_RGaugeM4L.AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);

                        m_RGaugeM4L.AddHighPassForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                        m_RGaugeM4L.AddGainForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                        if (m_RGaugeM4L.GetGaugeWantImageThreshold(intLineGaugeSelectedIndex))
                            m_RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                    }
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    ImageDrawing objSourceImage = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage];
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            if (m_RGaugeM4L.GetGaugeImageMode(j) == 0)
                            {
                                m_RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, j);
                                if (m_RGaugeM4L.GetGaugeWantImageThreshold(j))
                                    m_RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                            }
                            else
                            {
                                //m_RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, j);
                                //m_RGaugeM4L.AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, j);

                                m_RGaugeM4L.AddHighPassForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, j);
                                m_RGaugeM4L.AddGainForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                                if (m_RGaugeM4L.GetGaugeWantImageThreshold(j))
                                    m_RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                            }
                        }

                    }
                }
                else
                {
                    int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(0);
                    ImageDrawing objSourceImage = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage];
                    if (m_RGaugeM4L.GetGaugeImageMode(intLineGaugeSelectedIndex) == 0)
                    {
                        m_RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                        if (m_RGaugeM4L.GetGaugeWantImageThreshold(intLineGaugeSelectedIndex))
                            m_RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);

                    }
                    else
                    {
                        //m_RGaugeM4L.AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, intLineGaugeSelectedIndex);
                        //m_RGaugeM4L.AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);

                        m_RGaugeM4L.AddHighPassForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                        m_RGaugeM4L.AddGainForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                        if (m_RGaugeM4L.GetGaugeWantImageThreshold(intLineGaugeSelectedIndex))
                            m_RGaugeM4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                    }
                }
            }

        }

        private void UpdateEdgeRadioButtonGUI()
        {
            // ----- Image No -----
            bool blnAlreadyChecked = false;
            int intSelectedEdge = -1;
            if (m_RGaugeM4L.GetGaugeImageNo(0) == cbo_ImageNoTop.SelectedIndex)
            {
                if (intSelectedEdge == -1)
                    intSelectedEdge = 0;

                radioBtn_Up.Enabled = true;

                if (radioBtn_Up.Checked)
                    blnAlreadyChecked = true;
            }
            else
            {
                radioBtn_Up.Enabled = false;
            }

            if (m_RGaugeM4L.GetGaugeImageNo(1) == cbo_ImageNoRight.SelectedIndex)
            {
                if (intSelectedEdge == -1)
                    intSelectedEdge = 1;

                radioBtn_Right.Enabled = true;

                if (radioBtn_Right.Checked)
                    blnAlreadyChecked = true;
            }
            else
            {
                radioBtn_Right.Enabled = false;
            }

            if (m_RGaugeM4L.GetGaugeImageNo(2) == cbo_ImageNoBottom.SelectedIndex)
            {
                if (intSelectedEdge == -1)
                    intSelectedEdge = 2;

                radioBtn_Down.Enabled = true;

                if (radioBtn_Down.Checked)
                    blnAlreadyChecked = true;
            }
            else
            {
                radioBtn_Down.Enabled = false;
            }

            if (m_RGaugeM4L.GetGaugeImageNo(3) == cbo_ImageNoLeft.SelectedIndex)
            {
                if (intSelectedEdge == -1)
                    intSelectedEdge = 3;

                radioBtn_Left.Enabled = true;

                if (radioBtn_Left.Checked)
                    blnAlreadyChecked = true;
            }
            else
            {
                radioBtn_Left.Enabled = false;
            }

            if (!blnAlreadyChecked)
            {
                if (intSelectedEdge >= 0)
                {
                    switch (intSelectedEdge)
                    {
                        case 0:
                            radioBtn_Up.Checked = true;
                            break;
                        case 1:
                            radioBtn_Right.Checked = true;
                            break;
                        case 2:
                            radioBtn_Down.Checked = true;
                            break;
                        case 3:
                            radioBtn_Left.Checked = true;
                            break;
                    }
                }
            }
        }

        private void UpdateSelectedGaugeEdgeMask()
        {
            if (chk_SetToAll.Checked)
            {
                m_blnSetToAll = true;
                m_RGaugeM4L.ref_intSelectedGaugeEdgeMask = m_intEdgeEnableMask;
            }
            else
            {
                m_blnSetToAll = false;
                m_intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(0);
                m_RGaugeM4L.ref_intSelectedGaugeEdgeMask = (m_intEdgeEnableMask & (0x01 << m_intLineGaugeSelectedIndex));
            }
        }

        private void btn_ImageNo_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            RectGaugeM4LImageForm objForm = new RectGaugeM4LImageForm(m_smVisionInfo, m_smProductionInfo, m_smCustomizeInfo, m_RGaugeM4L);
            objForm.ShowDialog();

            cbo_ImageNoTop.SelectedIndex = m_RGaugeM4L.GetGaugeImageNo(0);
            cbo_ImageNoRight.SelectedIndex = m_RGaugeM4L.GetGaugeImageNo(1);
            cbo_ImageNoBottom.SelectedIndex = m_RGaugeM4L.GetGaugeImageNo(2);
            cbo_ImageNoLeft.SelectedIndex = m_RGaugeM4L.GetGaugeImageNo(3);

            if (m_intLineGaugeSelectedIndex == 0)
            {
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoTop.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                DefineEdgeEnableMask(cbo_ImageNoTop.SelectedIndex);
            }
            else if (m_intLineGaugeSelectedIndex == 1)
            {
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoRight.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                DefineEdgeEnableMask(cbo_ImageNoRight.SelectedIndex);
            }
            else if (m_intLineGaugeSelectedIndex == 2)
            {
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoBottom.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                DefineEdgeEnableMask(cbo_ImageNoBottom.SelectedIndex);
            }
            else if (m_intLineGaugeSelectedIndex == 3)
            {
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoLeft.SelectedIndex, m_smVisionInfo.g_intVisionIndex);
                DefineEdgeEnableMask(cbo_ImageNoLeft.SelectedIndex);
            }

            //DefineEdgeEnableMask();

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            UpdatePackageImage();

            m_blnBlockOtherControlUpdate = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            objForm.Dispose();

        }

        private void chk_SetToAll_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            UpdateSelectedGaugeEdgeMask();

            UpdateGaugeToGUI();

            UpdatePackageImage();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            m_blnBlockOtherControlUpdate = false;
        }

        private void txt_ImageGain_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            txt_ImageThreshold.Enabled = chk_EnableThreshold.Checked;

            float fImageGain = Convert.ToSingle(txt_ImageGain.Value);
            int intImageThreshld = Convert.ToInt32(txt_ImageThreshold.Value);

            if (m_blnSetToAll)
            {
                for (int j = 0; j < 4; j++)
                {
                    if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                    {
                        m_RGaugeM4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, j);
                        m_RGaugeM4L.SetGaugeImageGain(fImageGain, j);
                        m_RGaugeM4L.SetGaugeImageThreshold(intImageThreshld, j);
                    }
                }
            }
            else
            {
                int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(0);
                m_RGaugeM4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, intLineGaugeSelectedIndex);
                m_RGaugeM4L.SetGaugeImageGain(fImageGain, intLineGaugeSelectedIndex);
                m_RGaugeM4L.SetGaugeImageThreshold(intImageThreshld, intLineGaugeSelectedIndex);
            }

            UpdatePackageImage();

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void chk_EnableSubGauge_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeEnableSubGauge(chk_EnableSubGauge.Checked, j);
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeEnableSubGauge(chk_EnableSubGauge.Checked, m_intLineGaugeSelectedIndex);
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeEnableSubGauge(chk_EnableSubGauge.Checked, j);
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeEnableSubGauge(chk_EnableSubGauge.Checked, m_intLineGaugeSelectedIndex);
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeEnableSubGauge(chk_EnableSubGauge.Checked, j);
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeEnableSubGauge(chk_EnableSubGauge.Checked, m_intLineGaugeSelectedIndex);
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, m_intLineGaugeSelectedIndex);
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_ImageNoTop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            m_RGaugeM4L.SetGaugeImageNo(cbo_ImageNoTop.SelectedIndex, 0);

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoTop.SelectedIndex, m_smVisionInfo.g_intVisionIndex);

            DefineEdgeEnableMask(cbo_ImageNoTop.SelectedIndex);

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            UpdatePackageImage();

            m_blnBlockOtherControlUpdate = false;

            radioBtn_Up.PerformClick();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_ImageNoRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            m_RGaugeM4L.SetGaugeImageNo(cbo_ImageNoRight.SelectedIndex, 1);

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoRight.SelectedIndex, m_smVisionInfo.g_intVisionIndex);

            DefineEdgeEnableMask(cbo_ImageNoRight.SelectedIndex);

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            UpdatePackageImage();

            m_blnBlockOtherControlUpdate = false;

            radioBtn_Right.PerformClick();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_ImageNoBottom_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;
            
            m_blnBlockOtherControlUpdate = true;

            m_RGaugeM4L.SetGaugeImageNo(cbo_ImageNoBottom.SelectedIndex, 2);

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoBottom.SelectedIndex, m_smVisionInfo.g_intVisionIndex);

            DefineEdgeEnableMask(cbo_ImageNoBottom.SelectedIndex);

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            UpdatePackageImage();

            m_blnBlockOtherControlUpdate = false;

            radioBtn_Down.PerformClick();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_ImageNoLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            m_blnBlockOtherControlUpdate = true;

            m_RGaugeM4L.SetGaugeImageNo(cbo_ImageNoLeft.SelectedIndex, 3);

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(cbo_ImageNoLeft.SelectedIndex, m_smVisionInfo.g_intVisionIndex);

            DefineEdgeEnableMask(cbo_ImageNoLeft.SelectedIndex);

            UpdateEdgeRadioButtonGUI();

            UpdateSelectedGaugeEdgeMask();

            UpdatePackageImage();

            m_blnBlockOtherControlUpdate = false;

            radioBtn_Left.PerformClick();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            txt_ImageThreshold.Enabled = chk_EnableThreshold.Checked;

            float fImageGain = Convert.ToSingle(txt_ImageGain.Value);
            int intImageThreshld = Convert.ToInt32(txt_ImageThreshold.Value);

            if (m_blnSetToAll)
            {
                for (int j = 0; j < 4; j++)
                {
                    if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                    {
                        m_RGaugeM4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, j);
                        m_RGaugeM4L.SetGaugeImageGain(fImageGain, j);
                        m_RGaugeM4L.SetGaugeImageThreshold(intImageThreshld, j);
                    }
                }
            }
            else
            {
                int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(0);
                m_RGaugeM4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, intLineGaugeSelectedIndex);
                m_RGaugeM4L.SetGaugeImageGain(fImageGain, intLineGaugeSelectedIndex);
                m_RGaugeM4L.SetGaugeImageThreshold(intImageThreshld, intLineGaugeSelectedIndex);
            }

            UpdatePackageImage();

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void chk_EnableThreshold_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            txt_ImageThreshold.Enabled = chk_EnableThreshold.Checked;
            txt_Threshold.Enabled = chk_EnableThreshold.Checked;
            trackBar_Threshold.Enabled = chk_EnableThreshold.Checked;

            float fImageGain = Convert.ToSingle(txt_Gain.Text);
            int intImageThreshld = Convert.ToInt32(txt_Threshold.Text);

            if (m_blnSetToAll)
            {
                for (int j = 0; j < 4; j++)
                {
                    if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                    {
                        m_RGaugeM4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, j);
                        //m_RGaugeM4L.SetGaugeImageGain(fImageGain, j);
                        //m_RGaugeM4L.SetGaugeImageThreshold(intImageThreshld, j);
                    }
                }
            }
            else
            {
                int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(0);
                m_RGaugeM4L.SetGaugeWantImageThreshold(chk_EnableThreshold.Checked, intLineGaugeSelectedIndex);
                //m_RGaugeM4L.SetGaugeImageGain(fImageGain, intLineGaugeSelectedIndex);
                //m_RGaugeM4L.SetGaugeImageThreshold(intImageThreshld, intLineGaugeSelectedIndex);
            }

            UpdatePackageImage();

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_GaugeMeasureMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                //if (m_blnSetToAll)
                //{
                for (int j = 0; j < 4; j++)
                {
                    //if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                    {
                        m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, j);
                    }
                }
                //}
                //else
                //{
                //    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, m_intLineGaugeSelectedIndex);
                //}
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                //if (m_blnSetToAll)
                //{
                for (int j = 0; j < 4; j++)
                {
                    //                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                    {
                        m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, j);
                    }
                }
                //}
                //    else
                //    {
                //        m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, m_intLineGaugeSelectedIndex);
                //    }
            }
            else
            {
                //if (m_blnSetToAll)
                //{
                    for (int j = 0; j < 4; j++)
                    {
                        //if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeEnableSubGauge(chk_EnableSubGauge.Checked, j);
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, j);
                        }
                    }
                //}
                //else
                //{
                //    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeEnableSubGauge(chk_EnableSubGauge.Checked, m_intLineGaugeSelectedIndex);
                //    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeMeasureMode(cbo_GaugeMeasureMode.SelectedIndex, m_intLineGaugeSelectedIndex);
                //}
            }

            chk_ReCheckUsingPointGaugeIfFail.Visible = btn_SaveAndSetSizeAsReference.Visible = cbo_GaugeMeasureMode.SelectedIndex == 3;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void trackBar_Gain_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Gain.Text = (Convert.ToSingle(trackBar_Gain.Value) / 10).ToString();
        }

        private void trackBar_Threshold_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_Threshold.Text = trackBar_Threshold.Value.ToString();
        }

        private void txt_Threshold_TextChanged_1(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageThreshold(Convert.ToInt32(txt_Threshold.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageThreshold(Convert.ToInt32(txt_Threshold.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageThreshold(Convert.ToInt32(txt_Threshold.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageThreshold(Convert.ToInt32(txt_Threshold.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageThreshold(Convert.ToInt32(txt_Threshold.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageThreshold(Convert.ToInt32(txt_Threshold.Text), m_intLineGaugeSelectedIndex);
                }
            }
            trackBar_Threshold.Value = Convert.ToInt32(txt_Threshold.Text);

            UpdatePackageImage();
//            m_PointGauge.ref_GaugeThickness = Convert.ToInt32(txt_Threshold.Text);

            m_PointGauge.SetGaugePlacement(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterX,
              m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterY,
               m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeTolerance, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeAngle + 90);

            m_PointGauge.Measure(m_RGaugeM4L.ref_arrEdgeROI[m_intLineGaugeSelectedIndex]);
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Gain_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;
            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageGain(Convert.ToSingle(txt_Gain.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageGain(Convert.ToSingle(txt_Gain.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageGain(Convert.ToSingle(txt_Gain.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageGain(Convert.ToSingle(txt_Gain.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageGain(Convert.ToSingle(txt_Gain.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageGain(Convert.ToSingle(txt_Gain.Text), m_intLineGaugeSelectedIndex);
                }
            }
            trackBar_Gain.Value = Convert.ToInt32(Convert.ToSingle(txt_Gain.Text) * 10f);

            UpdatePackageImage();

            //m_PointGauge.ref_GaugeThreshold = Convert.ToInt32(txt_Gain.Text);

            m_PointGauge.SetGaugePlacement(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterX,
                 m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeCenterY,
                  m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeTolerance, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_GaugeAngle + 90);

            m_PointGauge.Measure(m_RGaugeM4L.ref_arrEdgeROI[m_intLineGaugeSelectedIndex]);
            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_AutoTuning_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            AutoTuneGaugeDrawForm objAutoTuneGaugeDrawForm = new AutoTuneGaugeDrawForm(m_smVisionInfo, m_smProductionInfo, m_smCustomizeInfo, m_strPath);
            if (objAutoTuneGaugeDrawForm.ShowDialog() == DialogResult.OK)
            {
                m_blnAutoTuneForAllEdges = objAutoTuneGaugeDrawForm.ref_blnSetToAll;
                if (objAutoTuneGaugeDrawForm.ref_blnAutoPlace)
                {
                    if (m_blnAutoTuneForAllEdges)
                    {
                        m_RGaugeM4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(0).Y), 0, false);
                        m_RGaugeM4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(1).Y), 1, false);
                        m_RGaugeM4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(2).Y), 2, false);
                        m_RGaugeM4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(3).Y), 3, false);
                    }
                    else
                        m_RGaugeM4L.SetEdgeROIPlacementLimit_ForAutoTune(m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(m_intLineGaugeSelectedIndex).X), (int)Math.Round(objAutoTuneGaugeDrawForm.GetLineCenterPoint(m_intLineGaugeSelectedIndex).Y), m_intLineGaugeSelectedIndex, false);
                }

                objAutoTuneGaugeDrawForm.GetLineAngle(0, ref m_arrAngle[0]);
                objAutoTuneGaugeDrawForm.GetLineAngle(1, ref m_arrAngle[1]);
                objAutoTuneGaugeDrawForm.GetLineAngle(2, ref m_arrAngle[2]);
                objAutoTuneGaugeDrawForm.GetLineAngle(3, ref m_arrAngle[3]);

                //2020-06-11 ZJYEOH : adjust to line gauge angle
                if (m_arrAngle[0] > 0)
                {
                    m_arrAngle[0] -= 90;
                }
                else
                {
                    m_arrAngle[0] += 90;
                }

                if (m_arrAngle[2] > 0)
                {
                    m_arrAngle[2] -= 90;
                }
                else
                {
                    m_arrAngle[2] += 90;
                }

                m_arrPosition1[0].Y = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(0, m_RGaugeM4L.ref_arrEdgeROI[0].ref_ROITotalCenterX, m_RGaugeM4L.ref_arrEdgeROI[0].ref_ROITotalCenterY - (m_RGaugeM4L.ref_arrEdgeROI[0].ref_ROIHeight / 10));
                m_arrPosition1[1].X = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(1, m_RGaugeM4L.ref_arrEdgeROI[1].ref_ROITotalCenterX - (m_RGaugeM4L.ref_arrEdgeROI[1].ref_ROIWidth / 10), m_RGaugeM4L.ref_arrEdgeROI[1].ref_ROITotalCenterY);
                m_arrPosition1[2].Y = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(2, m_RGaugeM4L.ref_arrEdgeROI[2].ref_ROITotalCenterX, m_RGaugeM4L.ref_arrEdgeROI[2].ref_ROITotalCenterY - (m_RGaugeM4L.ref_arrEdgeROI[2].ref_ROIHeight / 10));
                m_arrPosition1[3].X = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(3, m_RGaugeM4L.ref_arrEdgeROI[3].ref_ROITotalCenterX - (m_RGaugeM4L.ref_arrEdgeROI[3].ref_ROIWidth / 10), m_RGaugeM4L.ref_arrEdgeROI[3].ref_ROITotalCenterY);

                m_arrPosition2[0].Y = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(0, m_RGaugeM4L.ref_arrEdgeROI[0].ref_ROITotalCenterX, m_RGaugeM4L.ref_arrEdgeROI[0].ref_ROITotalCenterY + (m_RGaugeM4L.ref_arrEdgeROI[0].ref_ROIHeight / 10));
                m_arrPosition2[1].X = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(1, m_RGaugeM4L.ref_arrEdgeROI[1].ref_ROITotalCenterX + (m_RGaugeM4L.ref_arrEdgeROI[1].ref_ROIWidth / 10), m_RGaugeM4L.ref_arrEdgeROI[1].ref_ROITotalCenterY);
                m_arrPosition2[2].Y = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(2, m_RGaugeM4L.ref_arrEdgeROI[2].ref_ROITotalCenterX, m_RGaugeM4L.ref_arrEdgeROI[2].ref_ROITotalCenterY + (m_RGaugeM4L.ref_arrEdgeROI[2].ref_ROIHeight / 10));
                m_arrPosition2[3].X = objAutoTuneGaugeDrawForm.GetLineInterceptPoint(3, m_RGaugeM4L.ref_arrEdgeROI[3].ref_ROITotalCenterX + (m_RGaugeM4L.ref_arrEdgeROI[3].ref_ROIWidth / 10), m_RGaugeM4L.ref_arrEdgeROI[3].ref_ROITotalCenterY);

                StartWaiting("Auto Tuning...");
                
                if (m_blnAutoTuneForAllEdges)
                {
                    if (m_intLineGaugeSelectedIndex != 0)
                        radioBtn_Up.PerformClick();
                }

                Timer_AutoTune.Enabled = true;
            }

            
            objAutoTuneGaugeDrawForm.Dispose();
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
 
        }

        private void Timer_AutoTune_Tick(object sender, EventArgs e)
        {
            if (m_blnBlockOtherControlUpdate)
                return;

            m_smVisionInfo.g_blnViewAutoTuning = true;

            //RectGaugeM4L RGaugeM4L = m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit];
            //if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            //{
            //    RGaugeM4L = m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit];
            //}
            //else if (m_smVisionInfo.g_blnViewMOGauge)
            //{
            //    RGaugeM4L = m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit];
            //}
           

            if (chk_EnableThreshold.Checked)
            {
                m_RGaugeM4L.SetGaugeImageThreshold(m_intThreshold_Temp, m_intLineGaugeSelectedIndex);
            }
            else
            {
                m_RGaugeM4L.SetGaugeThickness(m_intThickness_Temp, m_intLineGaugeSelectedIndex);
                m_RGaugeM4L.SetGaugeThreshold(m_intDerivative_Temp, m_intLineGaugeSelectedIndex);
            }

            //if (m_smVisionInfo.g_blnViewPackageImage)
            //    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            //else
            //    m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            m_RGaugeM4L.SetGaugePlace_BasedOnEdgeROI();

            // 2020-06-15 ZJYEOH : Hide tolerance changing as finaaly will measure gauge using ROI Size as tolerance
            //if (chk_EnableThreshold.Checked)
            //{
            //    if (m_intThreshold_Temp > 249 && m_fSampleScore == 0 && m_intGaugeTolerance != m_RGaugeM4L.GetEdgeROISize(m_intLineGaugeSelectedIndex))
            //    {
            //        m_fSampleScore = 0;
            //        m_fAngle = 1000;
            //        m_intThreshold_Temp = 5;
            //        m_RGaugeM4L.SetGaugeImageThreshold(m_intThreshold_Temp, m_intLineGaugeSelectedIndex);
            //        if (m_intGaugeTolerance < 9)
            //            m_intGaugeTolerance += 3;
            //        else if(m_intGaugeTolerance < m_RGaugeM4L.GetEdgeROISize(m_intLineGaugeSelectedIndex, true, m_intGaugeTolerance))
            //        {
            //            m_intGaugeTolerance = m_RGaugeM4L.GetEdgeROISize(m_intLineGaugeSelectedIndex, true, m_intGaugeTolerance);
            //        }
            //        else if (m_intGaugeTolerance < m_RGaugeM4L.GetEdgeROISize(m_intLineGaugeSelectedIndex, false, m_intGaugeTolerance))
            //        {
            //            m_intGaugeTolerance = m_RGaugeM4L.GetEdgeROISize(m_intLineGaugeSelectedIndex, false, m_intGaugeTolerance);
            //        }

            //    }

            //    m_RGaugeM4L.SetGaugeTolerance_BasedOnEdgeROI(Math.Min(m_intGaugeTolerance, m_RGaugeM4L.GetEdgeROISize(m_intLineGaugeSelectedIndex)), m_intLineGaugeSelectedIndex);
            //}

            m_RGaugeM4L.Measure_WithDontCareArea(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][0], m_smVisionInfo.g_objWhiteImage,
                                         m_smVisionInfo.g_blnReferTemplateSize, m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_fTemplateUnitSizeX, m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_fTemplateUnitSizeY);

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            //if (Math.Abs(m_fAngle - m_arrAngle[m_intLineGaugeSelectedIndex]) + 0.5 > Math.Abs(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intLineGaugeSelectedIndex]))
            //if(Math2.GetDistanceBtw2Points(m_arrPosition[m_intLineGaugeSelectedIndex], new PointF(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectCenterX, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectCenterY)) <
            //    m_fDistance)
            if ((GetDistance(m_intLineGaugeSelectedIndex, true) < m_fDistance1) && (GetDistance(m_intLineGaugeSelectedIndex, false) < m_fDistance2))// && (Math.Abs(m_fAngle - m_arrAngle[m_intLineGaugeSelectedIndex]) > Math.Abs(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intLineGaugeSelectedIndex])))
            {
                switch (m_intLineGaugeSelectedIndex)
                {
                    case 0:
                        if (//(m_fSampleScore <= m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore) &&
                            (m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore > m_RGaugeM4L.GetGaugeMinScore(m_intLineGaugeSelectedIndex)))
                        {
                            //if (Math.Abs(m_fAngle - m_arrAngle[m_intLineGaugeSelectedIndex]) > Math.Abs(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intLineGaugeSelectedIndex]))
                            //    m_fAngle = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle;
                            m_fDistance1 = GetDistance(m_intLineGaugeSelectedIndex, true);//Math2.GetDistanceBtw2Points(m_arrPosition[m_intLineGaugeSelectedIndex], new PointF(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectCenterX, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectCenterY));
                            m_fDistance2 = GetDistance(m_intLineGaugeSelectedIndex, false);
                            if (chk_EnableThreshold.Checked)
                            {
                                m_intThreshold = m_intThreshold_Temp;
                            }
                            else
                            {
                                m_intThickness = m_intThickness_Temp;
                                m_intDerivative = m_intDerivative_Temp;
                            }
                            m_fSampleScore = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore;
                        }
                        break;
                    case 1:
                        if (//(m_fSampleScore <= m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore) &&
                         (m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore > m_RGaugeM4L.GetGaugeMinScore(m_intLineGaugeSelectedIndex)))
                        {
                            //if (Math.Abs(m_fAngle - m_arrAngle[m_intLineGaugeSelectedIndex]) > Math.Abs(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intLineGaugeSelectedIndex]))
                            //    m_fAngle = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle;
                            m_fDistance1 = GetDistance(m_intLineGaugeSelectedIndex, true);// Math2.GetDistanceBtw2Points(m_arrPosition[m_intLineGaugeSelectedIndex], new PointF(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectCenterX, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectCenterY));
                            m_fDistance2 = GetDistance(m_intLineGaugeSelectedIndex, false);
                            if (chk_EnableThreshold.Checked)
                            {
                                m_intThreshold = m_intThreshold_Temp;
                            }
                            else
                            {
                                m_intThickness = m_intThickness_Temp;
                                m_intDerivative = m_intDerivative_Temp;
                            }
                            m_fSampleScore = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore;
                        }
                        break;
                    case 2:
                        if (//(m_fSampleScore <= m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore) &&
                          (m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore > m_RGaugeM4L.GetGaugeMinScore(m_intLineGaugeSelectedIndex)))
                        {
                            //if (Math.Abs(m_fAngle - m_arrAngle[m_intLineGaugeSelectedIndex]) > Math.Abs(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intLineGaugeSelectedIndex]))
                            //    m_fAngle = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle;
                            m_fDistance1 = GetDistance(m_intLineGaugeSelectedIndex, true);// Math2.GetDistanceBtw2Points(m_arrPosition[m_intLineGaugeSelectedIndex], new PointF(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectCenterX, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectCenterY));
                            m_fDistance2 = GetDistance(m_intLineGaugeSelectedIndex, false);
                            if (chk_EnableThreshold.Checked)
                            {
                                m_intThreshold = m_intThreshold_Temp;
                            }
                            else
                            {
                                m_intThickness = m_intThickness_Temp;
                                m_intDerivative = m_intDerivative_Temp;
                            }
                            m_fSampleScore = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore;
                        }
                        break;
                    case 3:
                        if (//(m_fSampleScore <= m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore) &&
                          (m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore > m_RGaugeM4L.GetGaugeMinScore(m_intLineGaugeSelectedIndex)))
                        {
                            //if (Math.Abs(m_fAngle - m_arrAngle[m_intLineGaugeSelectedIndex]) > Math.Abs(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intLineGaugeSelectedIndex]))
                            //    m_fAngle = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle;
                            m_fDistance1 = GetDistance(m_intLineGaugeSelectedIndex, true);// Math2.GetDistanceBtw2Points(m_arrPosition[m_intLineGaugeSelectedIndex], new PointF(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectCenterX, m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectCenterY));
                            m_fDistance2 = GetDistance(m_intLineGaugeSelectedIndex, false);
                            if (chk_EnableThreshold.Checked)
                            {
                                m_intThreshold = m_intThreshold_Temp;
                            }
                            else
                            {
                                m_intThickness = m_intThickness_Temp;
                                m_intDerivative = m_intDerivative_Temp;
                            }
                            m_fSampleScore = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore;
                        }
                        break;
                }
            }
            //if (m_fSampleScore < m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore)
            //{
            //    switch (m_intLineGaugeSelectedIndex)
            //    {
            //        case 0:
            //            if (Math.Abs(m_fAngle - m_arrAngle[m_intLineGaugeSelectedIndex]) > Math.Abs(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intLineGaugeSelectedIndex]))
            //            {
            //                m_fAngle = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle;
            //                if (chk_EnableThreshold.Checked)
            //                {
            //                   m_intThreshold = m_intThreshold_Temp;
            //                }
            //                else
            //                {
            //                    m_intThickness = m_intThickness_Temp;
            //                    m_intDerivative = m_intDerivative_Temp;
            //                }
            //                m_fSampleScore = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore;
            //            }
            //            break;
            //        case 1:
            //            if (Math.Abs(m_fAngle - m_arrAngle[m_intLineGaugeSelectedIndex]) > Math.Abs(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intLineGaugeSelectedIndex]))
            //            {
            //                m_fAngle = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle;
            //                if (chk_EnableThreshold.Checked)
            //                {
            //                    m_intThreshold = m_intThreshold_Temp;
            //                }
            //                else
            //                {
            //                    m_intThickness = m_intThickness_Temp;
            //                    m_intDerivative = m_intDerivative_Temp;
            //                }
            //                m_fSampleScore = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore;
            //            }
            //            break;
            //        case 2:
            //            if (Math.Abs(m_fAngle - m_arrAngle[m_intLineGaugeSelectedIndex]) > Math.Abs(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intLineGaugeSelectedIndex]))
            //            {
            //                m_fAngle = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle;
            //                if (chk_EnableThreshold.Checked)
            //                {
            //                    m_intThreshold = m_intThreshold_Temp;
            //                }
            //                else
            //                {
            //                    m_intThickness = m_intThickness_Temp;
            //                    m_intDerivative = m_intDerivative_Temp;
            //                }
            //                m_fSampleScore = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore;
            //            }
            //            break;
            //        case 3:
            //            if (Math.Abs(m_fAngle - m_arrAngle[m_intLineGaugeSelectedIndex]) > Math.Abs(m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle - m_arrAngle[m_intLineGaugeSelectedIndex]))
            //            {
            //                m_fAngle = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectAngle;
            //                if (chk_EnableThreshold.Checked)
            //                {
            //                    m_intThreshold = m_intThreshold_Temp;
            //                }
            //                else
            //                {
            //                    m_intThickness = m_intThickness_Temp;
            //                    m_intDerivative = m_intDerivative_Temp;
            //                }
            //                m_fSampleScore = m_RGaugeM4L.ref_arrLineGauge[m_intLineGaugeSelectedIndex].ref_ObjectScore;
            //            }
            //            break;
            //    }
            //}
            if (chk_EnableThreshold.Checked)
            {
                m_intThreshold_Temp += 2;
                
                if (m_intThreshold_Temp > 254)
                {
                    m_RGaugeM4L.SetGaugePlace_BasedOnEdgeROI(); // 2020-06-12 ZJYEOH : resize back the tolerance as it is being modified for threshold auto tune
                    m_RGaugeM4L.SetGaugeImageThreshold(m_intThreshold, m_intLineGaugeSelectedIndex);
                    m_fSampleScore = 0;
                    m_fAngle = float.MaxValue;
                    m_fDistance1 = float.MaxValue;
                    m_fDistance2 = float.MaxValue;
                    m_intThickness = 10;
                    m_intDerivative = 5;
                    m_intThreshold = 5;
                    m_intGaugeTolerance = 3;
                    m_intThreshold_Temp = 5;
                    m_intThickness_Temp = 10;
                    m_intDerivative_Temp = 5;
                    m_smVisionInfo.g_blnViewAutoTuning = false;
                    m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                    if (m_blnAutoTuneForAllEdges)
                    {
                        if (m_intLineGaugeSelectedIndex == 3)
                        {
                            radioBtn_Left.PerformClick();
                            StopWaiting();
                            Timer_AutoTune.Enabled = false;
                        }
                    }
                    else
                    {
                        if (m_intLineGaugeSelectedIndex == 0)
                            radioBtn_Up.PerformClick();
                        else if (m_intLineGaugeSelectedIndex == 1)
                            radioBtn_Right.PerformClick();
                        else if (m_intLineGaugeSelectedIndex == 2)
                            radioBtn_Down.PerformClick();
                        else if (m_intLineGaugeSelectedIndex == 3)
                        {
                            radioBtn_Left.PerformClick();
                        }
                        StopWaiting();
                        Timer_AutoTune.Enabled = false;
                    }

                    if (m_blnAutoTuneForAllEdges)
                    {
                        if (m_intLineGaugeSelectedIndex == 0)
                            radioBtn_Right.PerformClick();
                        else if (m_intLineGaugeSelectedIndex == 1)
                            radioBtn_Down.PerformClick();
                        else if (m_intLineGaugeSelectedIndex == 2)
                            radioBtn_Left.PerformClick();
                    }
                }
                
            }
            else
            {
                m_intDerivative_Temp += 5;

                if (m_intDerivative_Temp > 65)
                {
                    m_intThickness_Temp += 5;
                    m_intDerivative_Temp = 5;
                    if (m_intThickness_Temp > 75)
                    {
                        m_RGaugeM4L.SetGaugeThickness(m_intThickness, m_intLineGaugeSelectedIndex);
                        m_RGaugeM4L.SetGaugeThreshold(m_intDerivative, m_intLineGaugeSelectedIndex);
                        m_fSampleScore = 0;
                        m_fAngle = float.MaxValue;
                        m_fDistance1 = float.MaxValue;
                        m_fDistance2 = float.MaxValue;
                        m_intThickness = 10;
                        m_intDerivative = 5;
                        m_intThreshold = 5;
                        m_intGaugeTolerance = 3;
                        m_intThreshold_Temp = 5;
                        m_intThickness_Temp = 10;
                        m_intDerivative_Temp = 5;
                        m_smVisionInfo.g_blnViewAutoTuning = false;
                        m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                        if (m_blnAutoTuneForAllEdges)
                        {
                            if (m_intLineGaugeSelectedIndex == 3)
                            {
                                radioBtn_Left.PerformClick();
                                StopWaiting();
                                Timer_AutoTune.Enabled = false;
                            }
                        }
                        else
                        {
                            if (m_intLineGaugeSelectedIndex == 0)
                                radioBtn_Up.PerformClick();
                            else if (m_intLineGaugeSelectedIndex == 1)
                                radioBtn_Right.PerformClick();
                            else if (m_intLineGaugeSelectedIndex == 2)
                                radioBtn_Down.PerformClick();
                            else if (m_intLineGaugeSelectedIndex == 3)
                            {
                                radioBtn_Left.PerformClick();
                            }
                            StopWaiting();
                            Timer_AutoTune.Enabled = false;
                        }

                        if (m_blnAutoTuneForAllEdges)
                        {
                            if (m_intLineGaugeSelectedIndex == 0)
                                radioBtn_Right.PerformClick();
                            else if (m_intLineGaugeSelectedIndex == 1)
                                radioBtn_Down.PerformClick();
                            else if (m_intLineGaugeSelectedIndex == 2)
                                radioBtn_Left.PerformClick();
                        }
                    }
                }
            }
            
           
        }

        private void StartWaiting(string StrMessage)
        {
            m_thWaitingFormThread = new SRMWaitingFormThread();
            m_thWaitingFormThread.SetStartSplash(StrMessage);
            m_pPreviousLocation = new Point(this.Location.X, this.Location.Y);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            this.Location = new Point(resolution.Width, resolution.Height);
        }

        private void StopWaiting()
        {
            m_thWaitingFormThread.SetStopSplash();
            this.Location = m_pPreviousLocation;
        }
        private void chk_ShowSamplePoints_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
            {
                m_smVisionInfo.g_arrPackageGaugeM4L[i].ref_blnDrawSamplingPoint = chk_ShowSamplePoints.Checked;
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private float GetDistance(int intLineIndex, bool blnDistance1)
        {
            switch (intLineIndex)
            {
                case 0:
                case 2:
                    if (blnDistance1)
                        return Math.Abs(m_arrPosition1[intLineIndex].Y - m_RGaugeM4L.ref_arrLineGauge[intLineIndex].ref_ObjectLine.GetPointY(m_RGaugeM4L.ref_arrEdgeROI[intLineIndex].ref_ROITotalCenterX - (m_RGaugeM4L.ref_arrEdgeROI[intLineIndex].ref_ROIWidth / 10)));
                    else
                        return Math.Abs(m_arrPosition2[intLineIndex].Y - m_RGaugeM4L.ref_arrLineGauge[intLineIndex].ref_ObjectLine.GetPointY(m_RGaugeM4L.ref_arrEdgeROI[intLineIndex].ref_ROITotalCenterX + (m_RGaugeM4L.ref_arrEdgeROI[intLineIndex].ref_ROIWidth / 10)));
                case 1:
                case 3:
                    if (blnDistance1)
                        return Math.Abs(m_arrPosition1[intLineIndex].X - m_RGaugeM4L.ref_arrLineGauge[intLineIndex].ref_ObjectLine.GetPointX(m_RGaugeM4L.ref_arrEdgeROI[intLineIndex].ref_ROITotalCenterY - (m_RGaugeM4L.ref_arrEdgeROI[intLineIndex].ref_ROIHeight / 10)));
                    else
                        return Math.Abs(m_arrPosition2[intLineIndex].X - m_RGaugeM4L.ref_arrLineGauge[intLineIndex].ref_ObjectLine.GetPointX(m_RGaugeM4L.ref_arrEdgeROI[intLineIndex].ref_ROITotalCenterY + (m_RGaugeM4L.ref_arrEdgeROI[intLineIndex].ref_ROIHeight / 10)));
            }
            return 0;
        }

        private void cbo_ImageMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageMode(cbo_ImageMode.SelectedIndex, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageMode(cbo_ImageMode.SelectedIndex, m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageMode(cbo_ImageMode.SelectedIndex, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageMode(cbo_ImageMode.SelectedIndex, m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageMode(cbo_ImageMode.SelectedIndex, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeImageMode(cbo_ImageMode.SelectedIndex, m_intLineGaugeSelectedIndex);
                }
            }

            UpdatePackageImage();

            m_blnUpdateHistogram = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_ReCheckUsingPointGaugeIfFail_Click(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetReCheckUsingPointGaugeIfFail(chk_ReCheckUsingPointGaugeIfFail.Checked, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetReCheckUsingPointGaugeIfFail(chk_ReCheckUsingPointGaugeIfFail.Checked, m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetReCheckUsingPointGaugeIfFail(chk_ReCheckUsingPointGaugeIfFail.Checked, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetReCheckUsingPointGaugeIfFail(chk_ReCheckUsingPointGaugeIfFail.Checked, m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetReCheckUsingPointGaugeIfFail(chk_ReCheckUsingPointGaugeIfFail.Checked, j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetReCheckUsingPointGaugeIfFail(chk_ReCheckUsingPointGaugeIfFail.Checked, m_intLineGaugeSelectedIndex);
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GroupTol_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeGroupTolerance(Convert.ToSingle(txt_GroupTol.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeGroupTolerance(Convert.ToSingle(txt_GroupTol.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeGroupTolerance(Convert.ToSingle(txt_GroupTol.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeGroupTolerance(Convert.ToSingle(txt_GroupTol.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeGroupTolerance(Convert.ToSingle(txt_GroupTol.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetGaugeGroupTolerance(Convert.ToSingle(txt_GroupTol.Text), m_intLineGaugeSelectedIndex);
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PointGaugeOffset_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnBlockOtherControlUpdate)
                return;

            if (m_smVisionInfo.g_intLearnStepNo == 1 && m_smVisionInfo.g_strSelectedPage != "Lead")
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetPointGaugeOffset(Convert.ToInt32(txt_PointGaugeOffset.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetPointGaugeOffset(Convert.ToInt32(txt_PointGaugeOffset.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else if (m_smVisionInfo.g_blnViewMOGauge)
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetPointGaugeOffset(Convert.ToInt32(txt_PointGaugeOffset.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetPointGaugeOffset(Convert.ToInt32(txt_PointGaugeOffset.Text), m_intLineGaugeSelectedIndex);
                }
            }
            else
            {
                if (m_blnSetToAll)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetPointGaugeOffset(Convert.ToInt32(txt_PointGaugeOffset.Text), j);
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetPointGaugeOffset(Convert.ToInt32(txt_PointGaugeOffset.Text), m_intLineGaugeSelectedIndex);
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void CheckSettingSame()
        {
            bool blnGainSame = true,
             blnThresholdSame = true,
             blnPolaritySame = true,
             blnDirectionSame = true,
             blnSearchSame = true,
             blnThicknessSame = true,
             blnFilterSame = true,
             blnMinAmplitudeSame = true,
             blnMinAreaSame = true,
             blnFilterPassSame = true,
             blnFilterThresholdSame = true,
             blnDerivativeSame = true,
             blnSamplingStepSame = true,
             blnMinScoreSame = true,
             blnMaxAngleSame = true;

            if (m_blnSetToAll)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (m_intLineGaugeSelectedIndex == j)
                        continue;

                    if ((m_intEdgeEnableMask & (0x01 << j)) > 0)
                    {
                        if (blnGainSame)
                        {
                            if (m_RGaugeM4L.GetGaugeImageGain(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeImageGain(j))
                            {
                                blnGainSame = false;
                            }
                        }

                        if (blnThresholdSame)
                        {
                            if ((m_RGaugeM4L.GetGaugeWantImageThreshold(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeWantImageThreshold(j))
                                || m_RGaugeM4L.GetGaugeImageThreshold(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeImageThreshold(j))
                            {
                                blnThresholdSame = false;
                            }
                        }

                        if (blnPolaritySame)
                        {
                            if (m_RGaugeM4L.GetGaugeTransType(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeTransType(j))
                            {
                                blnPolaritySame = false;
                            }
                        }

                        if (blnDirectionSame)
                        {
                            if (m_RGaugeM4L.GetGaugeDirection(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeDirection(j))
                            {
                                blnDirectionSame = false;
                            }
                        }

                        if (blnSearchSame)
                        {
                            if (m_RGaugeM4L.GetGaugeTransChoice(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeTransChoice(j))
                            {
                                blnSearchSame = false;
                            }
                        }

                        if (blnThicknessSame)
                        {
                            if (m_RGaugeM4L.GetGaugeThickness(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeThickness(j))
                            {
                                blnThicknessSame = false;
                            }
                        }

                        if (blnFilterSame)
                        {
                            if (m_RGaugeM4L.GetGaugeFilter(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeFilter(j))
                            {
                                blnFilterSame = false;
                            }
                        }

                        if (blnMinAmplitudeSame)
                        {
                            if (m_RGaugeM4L.GetGaugeMinAmplitude(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeMinAmplitude(j))
                            {
                                blnMinAmplitudeSame = false;
                            }
                        }

                        if (blnMinAreaSame)
                        {
                            if (m_RGaugeM4L.GetGaugeMinArea(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeMinArea(j))
                            {
                                blnMinAreaSame = false;
                            }
                        }

                        if (blnFilterPassSame)
                        {
                            if (m_RGaugeM4L.GetGaugeFilterPasses(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeFilterPasses(j))
                            {
                                blnFilterPassSame = false;
                            }
                        }

                        if (blnFilterThresholdSame)
                        {
                            if (m_RGaugeM4L.GetGaugeFilterThreshold(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeFilterThreshold(j))
                            {
                                blnFilterThresholdSame = false;
                            }
                        }

                        if (blnDerivativeSame)
                        {
                            if (m_RGaugeM4L.GetGaugeThreshold(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeThreshold(j))
                            {
                                blnDerivativeSame = false;
                            }
                        }

                        if (blnSamplingStepSame)
                        {
                            if (m_RGaugeM4L.GetGaugeSamplingStep(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeSamplingStep(j))
                            {
                                blnSamplingStepSame = false;
                            }
                        }

                        if (blnMinScoreSame)
                        {
                            if (m_RGaugeM4L.GetGaugeMinScore(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeMinScore(j))
                            {
                                blnMinScoreSame = false;
                            }
                        }

                        if (blnMaxAngleSame)
                        {
                            if (m_RGaugeM4L.GetGaugeMaxAngle(m_intLineGaugeSelectedIndex) != m_RGaugeM4L.GetGaugeMaxAngle(j))
                            {
                                blnMaxAngleSame = false;
                            }
                        }

                    }
                }
            }


            if (blnGainSame)
                lbl_Gain.ForeColor = Color.Black;
            else
                lbl_Gain.ForeColor = Color.Red;

            if (blnThresholdSame)
                lbl_Threshold.ForeColor = Color.Black;
            else
                lbl_Threshold.ForeColor = Color.Red;

            if (blnPolaritySame)
                lbl_Polarity.ForeColor = Color.Black;
            else
                lbl_Polarity.ForeColor = Color.Red;

            if (blnDirectionSame)
                lbl_Direction.ForeColor = Color.Black;
            else
                lbl_Direction.ForeColor = Color.Red;

            if (blnSearchSame)
                lbl_Search.ForeColor = Color.Black;
            else
                lbl_Search.ForeColor = Color.Red;

            if (blnThicknessSame)
                lbl_Thickness.ForeColor = Color.Black;
            else
                lbl_Thickness.ForeColor = Color.Red;

            if (blnFilterSame)
                lbl_Filter.ForeColor = Color.Black;
            else
                lbl_Filter.ForeColor = Color.Red;

            if (blnMinAmplitudeSame)
                lbl_MinAmplitude.ForeColor = Color.Black;
            else
                lbl_MinAmplitude.ForeColor = Color.Red;

            if (blnMinAreaSame)
                lbl_MinArea.ForeColor = Color.Black;
            else
                lbl_MinArea.ForeColor = Color.Red;

            if (blnFilterPassSame)
                lbl_FilterPass.ForeColor = Color.Black;
            else
                lbl_FilterPass.ForeColor = Color.Red;

            if (blnFilterThresholdSame)
                lbl_FilterThreshold.ForeColor = Color.Black;
            else
                lbl_FilterThreshold.ForeColor = Color.Red;

            if (blnDerivativeSame)
                lbl_Derivative.ForeColor = Color.Black;
            else
                lbl_Derivative.ForeColor = Color.Red;

            if (blnSamplingStepSame)
                lbl_SamplingStep.ForeColor = Color.Black;
            else
                lbl_SamplingStep.ForeColor = Color.Red;

            if (blnMinScoreSame)
                lbl_MinScore.ForeColor = Color.Black;
            else
                lbl_MinScore.ForeColor = Color.Red;

            if (blnMaxAngleSame)
                lbl_MaxAngle.ForeColor = Color.Black;
            else
                lbl_MaxAngle.ForeColor = Color.Red;

        }
    }
}