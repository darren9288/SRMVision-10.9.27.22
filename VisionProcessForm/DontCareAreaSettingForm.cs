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
using System.Threading;

namespace VisionProcessForm
{
    public partial class DontCareAreaSettingForm : Form
    {
        #region Member Variables
        private List<List<int>> m_arrAutoDontCareThreshold = new List<List<int>>();
        private List<List<int>> m_arrAutoDontCareOffset = new List<List<int>>();
        private List<List<bool>> m_arrAutoDontCare = new List<List<bool>>();
        private bool m_blnUpdating = false;
        private int[] m_arrMarkDontCareCount, m_arrPackageDontCareCount;
        private List<List<int>> m_arrPadDontCareCount = new List<List<int>>();
        private List<List<int>> m_arrMarkDontCareStartX, m_arrPackageDontCareStartX;
        private List<List<int>> m_arrMarkDontCareStartY, m_arrPackageDontCareStartY;
        private List<List<int>> m_arrMarkDontCareWidth, m_arrPackageDontCareWidth;
        private List<List<int>> m_arrMarkDontCareHeight, m_arrPackageDontCareHeight;
        private List<List<List<int>>> m_arrPadDontCareStartX = new List<List<List<int>>>(), m_arrPadDontCareStartY = new List<List<List<int>>>(), m_arrPadDontCareWidth = new List<List<List<int>>>(), m_arrPadDontCareHeight = new List<List<List<int>>>();
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private CustomOption m_smCustomizeInfo;
        private Graphics m_Graphic;
        private bool m_blnDrawRectDone = true;
        private bool m_blnDrawRect = false;
        private Point m_pLineStart;
        private Point m_pLineStop;
        private Point m_pRectStart;
        private Point m_pRectStop;
        private int m_intMouseHitX = 0;
        private int m_intMouseHitY = 0;
        private string m_strPath = "";
        private bool m_UpdatePictureBox;
        private float m_fZoomScalePrev = 1f;
        private float m_fScaleXPrev = 1f;
        private float m_fScaleYPrev = 1f;
        private float m_fZoomCount = 1f;
        private float m_fZoomCountPrev = 1f;
        private float m_fOriScaleX = 1f;
        private float m_fOriScaleY = 1f;
        private int m_intScollValueXPrev = 0;
        private int m_intScollValueYPrev = 0;
        private int m_intZoomImageFocusPointX = 0;
        private int m_intZoomImageFocusPointY = 0;
        private Rectangle rc = new Rectangle();
        private int m_intSelectedUnit = -1;
        private int m_intSelectedEdgeROI = -1;
        private bool m_blnFirstClick = false;
        #endregion

        public DontCareAreaSettingForm(VisionInfo smVisionInfo, ProductionInfo smProductionInfo, CustomOption smCustomizeInfo, string strPath)
        {
            InitializeComponent();
            m_smVisionInfo = smVisionInfo;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smProductionInfo = smProductionInfo;
            m_Graphic = Graphics.FromHwnd(pic_Image.Handle);
            m_strPath = strPath;
            m_fScaleXPrev = m_smVisionInfo.g_fScaleX;
            m_fScaleYPrev = m_smVisionInfo.g_fScaleY;
            m_fZoomScalePrev = m_smVisionInfo.g_fZoomScale;
            m_fOriScaleX = m_fScaleXPrev / m_fZoomScalePrev;
            m_fOriScaleY = m_fScaleYPrev / m_fZoomScalePrev;
            m_smVisionInfo.g_fZoomScale = 1f;
            m_fZoomCountPrev = 0;
            GetCurrentDontCareInfo();
            m_UpdatePictureBox = true;
        }
        private void UpdateDontCareSetting()
        {
            m_blnUpdating = true;
            switch (m_smVisionInfo.g_strSelectedPage)
            {
                //case "Orient":
                //case "MarkOrient":
                //case "MOLi":
                //    if (m_smVisionInfo.g_intLearnStepNo == 12)
                //    {
                //        if(m_smVisionInfo.g_arrOrientGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //        }
                //    }
                //    break;
                //case "Package":
                //    if (m_smVisionInfo.g_intLearnStepNo == 1)
                //    {
                //        if(m_smVisionInfo.g_arrPackageGauge2M4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //        }
                //    }
                //    else if (m_smVisionInfo.g_intLearnStepNo == 0)
                //    {
                //        if(m_smVisionInfo.g_arrPackageGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].GetCurrentAutoDontCareEnabled();
                //        }
                //    }
                //    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    if(m_smVisionInfo.g_arrPad.Length > m_smVisionInfo.g_intSelectedUnit)
                    {
                        if (!chk_Auto.Visible)
                            chk_Auto.Visible = true;
                        pnl_Auto.Visible = chk_Auto.Checked = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].ref_objRectGauge4L.GetCurrentAutoDontCareEnabled(m_smVisionInfo.g_intSelectedUnit);
                        txt_Offset.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].ref_objRectGauge4L.GetCurrentAutoDontCareOffset(m_smVisionInfo.g_intSelectedUnit).ToString();
                        trackBar_Offset.Value = Convert.ToInt32(txt_Offset.Text);
                        txt_Threshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].ref_objRectGauge4L.GetCurrentAutoDontCareThreshold(m_smVisionInfo.g_intSelectedUnit).ToString();
                        trackBar_Threshold.Value = Convert.ToInt32(txt_Threshold.Text);
                        pnl_Manual.Visible = !chk_Auto.Checked;
                        m_UpdatePictureBox = true;

                    }
                    break;
            }
            m_blnUpdating = false;
        }

        private void GetCurrentDontCareInfo()
        {
            switch (m_smVisionInfo.g_strSelectedPage)
            {
                case "Orient":
                case "MarkOrient":
                case "MOLi":
                        if (m_smVisionInfo.g_intLearnStepNo == 12)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrOrientGaugeM4L.Count; i++)
                            {
                                m_arrMarkDontCareCount = m_smVisionInfo.g_arrOrientGaugeM4L[i].GetCurrentDontCareCount();
                                m_arrMarkDontCareStartX = m_smVisionInfo.g_arrOrientGaugeM4L[i].GetCurrentDontCareStartX();
                                m_arrMarkDontCareStartY = m_smVisionInfo.g_arrOrientGaugeM4L[i].GetCurrentDontCareStartY();
                                m_arrMarkDontCareWidth = m_smVisionInfo.g_arrOrientGaugeM4L[i].GetCurrentDontCareWidth();
                                m_arrMarkDontCareHeight = m_smVisionInfo.g_arrOrientGaugeM4L[i].GetCurrentDontCareHeight();
                            }
                        }
                    break;
                case "Package":
                        if (m_smVisionInfo.g_intLearnStepNo == 1)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge2M4L.Count; i++)
                            {
                                m_arrPackageDontCareCount = m_smVisionInfo.g_arrPackageGauge2M4L[i].GetCurrentDontCareCount();
                                m_arrPackageDontCareStartX = m_smVisionInfo.g_arrPackageGauge2M4L[i].GetCurrentDontCareStartX();
                                m_arrPackageDontCareStartY = m_smVisionInfo.g_arrPackageGauge2M4L[i].GetCurrentDontCareStartY();
                                m_arrPackageDontCareWidth = m_smVisionInfo.g_arrPackageGauge2M4L[i].GetCurrentDontCareWidth();
                                m_arrPackageDontCareHeight = m_smVisionInfo.g_arrPackageGauge2M4L[i].GetCurrentDontCareHeight();
                            }
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 0)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
                            {
                                m_arrPackageDontCareCount = m_smVisionInfo.g_arrPackageGaugeM4L[i].GetCurrentDontCareCount();
                                m_arrPackageDontCareStartX = m_smVisionInfo.g_arrPackageGaugeM4L[i].GetCurrentDontCareStartX();
                                m_arrPackageDontCareStartY = m_smVisionInfo.g_arrPackageGaugeM4L[i].GetCurrentDontCareStartY();
                                m_arrPackageDontCareWidth = m_smVisionInfo.g_arrPackageGaugeM4L[i].GetCurrentDontCareWidth();
                                m_arrPackageDontCareHeight = m_smVisionInfo.g_arrPackageGaugeM4L[i].GetCurrentDontCareHeight();
                            }
                        }
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        m_arrPadDontCareCount.Add(new List<int>());
                        m_arrPadDontCareStartX.Add(new List<List<int>>());
                        m_arrPadDontCareStartY.Add(new List<List<int>>());
                        m_arrPadDontCareWidth.Add(new List<List<int>>());
                        m_arrPadDontCareHeight.Add(new List<List<int>>());
                        m_arrPadDontCareCount[i] = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetCurrentDontCareCount();
                        m_arrPadDontCareStartX[i] = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetCurrentDontCareStartX();
                        m_arrPadDontCareStartY[i] = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetCurrentDontCareStartY();
                        m_arrPadDontCareWidth[i] = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetCurrentDontCareWidth();
                        m_arrPadDontCareHeight[i] = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetCurrentDontCareHeight();
                        m_arrAutoDontCareThreshold.Add(new List<int>());
                        m_arrAutoDontCareOffset.Add(new List<int>());
                        m_arrAutoDontCare.Add(new List<bool>());
                        m_arrAutoDontCareThreshold[i] = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetCurrentAutoDontCareThreshold();
                        m_arrAutoDontCareOffset[i] = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetCurrentAutoDontCareOffset();
                        m_arrAutoDontCare[i] = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetCurrentAutoDontCareEnabled();
                    }
                    break;
            }
        }

        private void DrawMainImage()
        {
            if (m_smVisionInfo.g_blnViewThreshold)
            {
                switch (m_smVisionInfo.g_strSelectedPage)
                {
                    case "Calibrate":
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_objCalibrateROI);
                        break;
                    case "BottomOrientPad":
                    case "BottomOPadPkg":
                    case "Pad":
                        {
                            int intSelectedROI = -1;
                            for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
                            {
                                if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                if (!m_smVisionInfo.g_arrPad[j].ref_blnSelected)
                                    continue;

                                if (intSelectedROI == -1)
                                    intSelectedROI = j;

                                if (m_smVisionInfo.g_arrPadROIs[j].Count > 0 && m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())   // 0: search ROI
                                {
                                    intSelectedROI = j;
                                }
                            }

                            if (intSelectedROI != -1)
                            {
                                if (m_smVisionInfo.g_blnViewRotatedImage)
                                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPadROIs[intSelectedROI], 0);
                                else
                                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPadROIs[intSelectedROI], 0);
                            }
                        }
                        break;
                    case "PadPackage":
                        {
                            int intSelectedROI = -1;
                            for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
                            {
                                if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                if (!m_smVisionInfo.g_arrPad[j].ref_blnSelected)
                                    continue;

                                if (intSelectedROI == -1)
                                    intSelectedROI = j;

                                if (m_smVisionInfo.g_arrPadROIs[j].Count > 0 && m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())   // 0: search ROI
                                {
                                    intSelectedROI = j;
                                }
                            }

                            if (intSelectedROI != -1)
                            {
                                if (m_smVisionInfo.g_blnViewRotatedImage)
                                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPadROIs[intSelectedROI], 0);
                                else
                                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPadROIs[intSelectedROI], 0);
                            }
                        }
                        break;
                    case "MNPad":
                    case "PadOtherSettingForm":
                        {
                            int intSelectedROI = 0;
                            for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
                            {
                                if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                if (m_smVisionInfo.g_arrPadROIs[j].Count > 0 && m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())   // 0: search ROI
                                {
                                    intSelectedROI = j;
                                }
                            }

                            if (m_smVisionInfo.g_blnViewRotatedImage)
                                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPadROIs[intSelectedROI], 0);
                            else
                                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPadROIs[intSelectedROI], 0);
                        }
                        break;
                    case "Pad5S":
                        for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
                        {
                            if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (m_smVisionInfo.g_arrPadROIs[j].Count > 1 && m_smVisionInfo.g_arrPadROIs[j][1].GetROIHandle())
                            {
                                if (m_smVisionInfo.g_blnViewRotatedImage)
                                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPadROIs[j], 1);
                                else
                                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPadROIs[j], 1);
                            }
                        }
                        break;
                    case "MNPad5S":
                        for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
                        {
                            if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            if (m_smVisionInfo.g_arrPadROIs[j].Count > 0 && m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
                            {
                                if (m_smVisionInfo.g_blnViewRotatedImage)
                                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPadROIs[j], 1);
                                else
                                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPadROIs[j], 1);
                            }
                        }
                        break;
                    case "Seal":
                    case "MNSeal":
                    case "SealOtherSettingForm":
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, (ROI)m_smVisionInfo.g_arrSealROIs[m_smVisionInfo.g_intSelectedUnit][0]);
                        break;
                    case "MNPackage":
                    case "Package":
                        // 2019 08 21 - CCENG: LearnPackageForm > Display threshold image when user press Threshold button.
                        if (m_smVisionInfo.g_blnViewRotatedImage && m_smProductionInfo.g_blnViewInspection)
                            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPackageROIs);
                        else
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPackageROIs);

                        //if (m_smVisionInfo.g_blnViewRotatedImage && m_smProductionInfo.g_blnViewInspection)
                        //    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrOrientROIs);
                        //else
                        //    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrOrientROIs);


                        //if (m_smVisionInfo.g_blnViewPackageChip)
                        //    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPackageROIs, 0);
                        //else
                        //    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPackageROIs, 0);
                        break;
                    case "ColorPackage":
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                        {
                            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue,
                            m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_intSelectedROI);
                        }
                        else
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue,
                            m_smVisionInfo.g_arrPackageROIs, m_smVisionInfo.g_intSelectedROI);
                        break;
                    case "MNPosition":
                        if (m_smVisionInfo.g_arrPositioningROIs.Count > 0)
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPositioningROIs[0]);
                        break;
                    case "UnitPresent":
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPositioningROIs[0]);
                        break;
                    case "Lead":
                        {
                            int intSelectedROI = 0;
                            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                            {
                                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                                    intSelectedROI++;
                                else
                                    break;
                            }

                            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
                            {
                                if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())   // 0: search ROI
                                {
                                    intSelectedROI = j;
                                }
                            }

                            if (m_smVisionInfo.g_blnViewRotatedImage)
                                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrLeadROIs[intSelectedROI], 0);
                            else
                                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrLeadROIs[intSelectedROI], 0);
                        }
                        break;
                    case "Li3D":
                        {
                            int intSelectedROI = 0;

                            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
                            {
                                if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())   // 0: search ROI
                                {
                                    intSelectedROI = j;
                                }
                            }

                            if (m_smVisionInfo.g_blnViewRotatedImage)
                                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrLeadROIs[intSelectedROI], 0);
                            else
                                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrLeadROIs[intSelectedROI], 0);
                        }
                        break;
                    case "MarkOtherSettingForm":
                        if (m_smVisionInfo.g_intSelectedSetting == 1)
                        {
                            //Drawing for lead
                            int intSelectedROI = 0;
                            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                            {
                                if (!m_smVisionInfo.g_arrLead[i].ref_blnSelected)
                                    intSelectedROI++;
                                else
                                    break;
                            }

                            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
                            {
                                if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())   // 0: search ROI
                                {
                                    intSelectedROI = j;
                                }
                            }

                            if (m_smVisionInfo.g_blnViewRotatedImage)
                                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrLeadROIs[intSelectedROI], 0);
                            else
                                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrLeadROIs[intSelectedROI], 0);
                        }
                        else if (m_smVisionInfo.g_intSelectedSetting == 2)   // MarkOtherSettingForm.cs > Package Tab Page
                        {
                            //Drawing for package
                            if (m_smVisionInfo.g_blnViewRotatedImage && m_smProductionInfo.g_blnViewInspection)
                                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPackageROIs);
                            else
                                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPackageROIs);
                        }
                        else
                        {
                            //Drawing for mark
                            if (m_smVisionInfo.g_blnViewRotatedImage)
                            {
                                //if (m_smVisionInfo.g_arrPackageROIs.Count > 0)
                                //    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPackageROIs, 0);
                                //else
                                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrMarkROIs, 0);

                            }
                            else
                            {
                                //if (m_smVisionInfo.g_arrPackageROIs.Count > 0)
                                //    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPackageROIs, 0);
                                //else
                                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrMarkROIs, 0);
                            }
                        }
                        break;
                    case "PH":
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPHROIs, 0);
                        break;
                    case "Position":
                        if (m_smVisionInfo.g_blnWantCheckEmpty)
                        {
                            ROI objROI = m_smVisionInfo.g_arrPositioningROIs[0];
                            int intROIindex = 0;
                            if (m_smVisionInfo.g_arrPositioningROIs.Count > 1 && m_smVisionInfo.g_blnWantUseEmptyPattern)
                            {
                                objROI = m_smVisionInfo.g_arrPositioningROIs[1];
                                intROIindex = 1;
                            }
                            else
                            {
                                objROI = m_smVisionInfo.g_arrPositioningROIs[0];
                                intROIindex = 0;
                            }
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrPositioningROIs, intROIindex);

                            int intBlackArea = ROI.GetPixelArea(objROI, m_smVisionInfo.g_intThresholdValue, 0);

                            m_Graphic.DrawString("Black Area=" + intBlackArea.ToString(), new Font("Verdana", 12, FontStyle.Bold), new SolidBrush(Color.Cyan), m_smVisionInfo.g_arrPositioningROIs[0].ref_ROIPositionX, m_smVisionInfo.g_arrPositioningROIs[0].ref_ROIPositionY - 20);
                        }
                        break;
                    default:
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                        {
                            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrMarkROIs, 0);

                        }
                        else
                        {
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intThresholdValue, m_smVisionInfo.g_arrMarkROIs, 0);
                        }

                        break;
                }

            }
            else if (m_smVisionInfo.g_blnViewDoubleThreshold)
            {
                switch (m_smVisionInfo.g_strSelectedPage)
                {
                    case "ColorPackage":
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue,
                            m_smVisionInfo.g_intHighThresholdValue, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
                        else
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue,
                            m_smVisionInfo.g_intHighThresholdValue, m_smVisionInfo.g_arrPackageROIs[m_smVisionInfo.g_intSelectedUnit][1]);
                        break;
                    case "MNPackage":
                    case "MNMarkOrient":
                    case "Package":
                    case "MarkOtherSettingForm":
                        m_smVisionInfo.g_objPkgProcessImage.RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue,
                              m_smVisionInfo.g_intHighThresholdValue, m_smVisionInfo.g_arrPackageROIs, 1f, 0, 255, 0, m_smVisionInfo.g_intSelectedUnit);
                        break;
                    case "BottomOrientPad":
                    case "BottomOPadPkg":
                    case "Pad":
                    case "Pad5S":
                        if (m_smVisionInfo.g_intLearnStepNo == 11)
                        {
                            m_smVisionInfo.g_arrRotatedImages[0].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue,
                            m_smVisionInfo.g_intHighThresholdValue, m_smVisionInfo.g_arrPadROIs[0][1]);
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 12)
                        {
                            m_smVisionInfo.g_arrRotatedImages[1].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue,
                            m_smVisionInfo.g_intHighThresholdValue, m_smVisionInfo.g_arrPadROIs[0][1]);
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 13)
                        {
                            m_smVisionInfo.g_arrRotatedImages[2].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue,
                            m_smVisionInfo.g_intHighThresholdValue, m_smVisionInfo.g_arrPadROIs[0][1]);
                        }
                        break;
                    case "PadPackage":
                        {
                            int intSelectedROI = -1;
                            if (m_smVisionInfo.g_intLearnStepNo == 4)   // Image 1 package double threshold view
                            {
                                for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
                                {
                                    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                        break;

                                    if (!m_smVisionInfo.g_arrPad[j].ref_blnSelected)
                                        continue;

                                    if (intSelectedROI == -1)
                                        intSelectedROI = j;

                                    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0 && m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())   // 0: search ROI
                                    {
                                        intSelectedROI = j;
                                    }
                                }

                                if (intSelectedROI != -1)
                                {
                                    if (m_smVisionInfo.g_blnViewRotatedImage)
                                        m_smVisionInfo.g_arrRotatedImages[0].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue, m_smVisionInfo.g_intHighThresholdValue, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0]);
                                    else
                                        m_smVisionInfo.g_arrImages[0].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue, m_smVisionInfo.g_intHighThresholdValue, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0]);
                                }
                            }
                            else if (m_smVisionInfo.g_intLearnStepNo == 5)  // Image 2 package double threshold view
                            {
                                for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
                                {
                                    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                        break;

                                    if (!m_smVisionInfo.g_arrPad[j].ref_blnSelected)
                                        continue;

                                    if (intSelectedROI == -1)
                                        intSelectedROI = j;

                                    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0 && m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())   // 0: search ROI
                                    {
                                        intSelectedROI = j;
                                    }
                                }

                                if (intSelectedROI != -1)
                                {
                                    if (m_smVisionInfo.g_blnViewRotatedImage)
                                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue, m_smVisionInfo.g_intHighThresholdValue, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0]);
                                    else
                                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue, m_smVisionInfo.g_intHighThresholdValue, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0]);
                                }
                            }
                            else if (m_smVisionInfo.g_intLearnStepNo == 8)      // Crack double threshold view
                            {
                                for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
                                {
                                    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                        break;

                                    if (!m_smVisionInfo.g_arrPad[j].ref_blnSelected)
                                        continue;

                                    if (intSelectedROI == -1)
                                        intSelectedROI = j;

                                    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0 && m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())   // 0: search ROI
                                    {
                                        intSelectedROI = j;
                                    }
                                }

                                if (intSelectedROI != -1)
                                {
                                    if (m_smVisionInfo.g_blnViewRotatedImage)
                                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue, m_smVisionInfo.g_intHighThresholdValue, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0]);
                                    else
                                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue, m_smVisionInfo.g_intHighThresholdValue, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0]);
                                }
                            }
                        }
                        break;
                    //case "PadOtherSettingForm":
                    //case "MNPad":
                    //case "MNPad5S":
                    //    {
                    //        int intSelectedROI = 0;
                    //        for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
                    //        {
                    //            if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    //                break;

                    //            if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())   // 0: search ROI
                    //            {
                    //                intSelectedROI = j;
                    //            }
                    //        }

                    //        if (m_intSelectedTabPage == 0)
                    //        {
                    //            if (m_smVisionInfo.g_blnViewRotatedImage)
                    //            {
                    //                ImageDrawing objImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
                    //                ROI objROI = new ROI();
                    //                objROI.AttachImage(objImage);
                    //                //objROI.LoadROISetting(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);
                    //                objROI.LoadROISetting(m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROITotalX,
                    //                                                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROITotalY,
                    //                                                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROIWidth,
                    //                                                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROIHeight);
                    //                if (m_smVisionInfo.g_blnViewGainImage)
                    //                {
                    //                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue,
                    //                                           m_smVisionInfo.g_intHighThresholdValue, objROI, m_smVisionInfo.g_fGainValue,
                    //                                           m_smVisionInfo.g_intThresholdDrawLowValue, m_smVisionInfo.g_intThresholdDrawMiddleValue, m_smVisionInfo.g_intThresholdDrawHighValue);
                    //                }
                    //                else
                    //                {
                    //                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue,
                    //                                            m_smVisionInfo.g_intHighThresholdValue, objROI,
                    //                                            m_smVisionInfo.g_intThresholdDrawLowValue, m_smVisionInfo.g_intThresholdDrawMiddleValue, m_smVisionInfo.g_intThresholdDrawHighValue);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                ImageDrawing objImage = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage];
                    //                ROI objROI = new ROI();
                    //                objROI.AttachImage(objImage);
                    //                //objROI.LoadROISetting(0, 0, objImage.ref_intImageWidth, objImage.ref_intImageHeight);
                    //                objROI.LoadROISetting(m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROITotalX,
                    //                                                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROITotalY,
                    //                                                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROIWidth,
                    //                                                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROIHeight);
                    //                if (m_smVisionInfo.g_blnViewGainImage)
                    //                {
                    //                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue,
                    //                                            m_smVisionInfo.g_intHighThresholdValue, objROI, m_smVisionInfo.g_fGainValue,
                    //                                            m_smVisionInfo.g_intThresholdDrawLowValue, m_smVisionInfo.g_intThresholdDrawMiddleValue, m_smVisionInfo.g_intThresholdDrawHighValue);
                    //                }
                    //                else
                    //                {
                    //                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue,
                    //                                        m_smVisionInfo.g_intHighThresholdValue, objROI,
                    //                                        m_smVisionInfo.g_intThresholdDrawLowValue, m_smVisionInfo.g_intThresholdDrawMiddleValue, m_smVisionInfo.g_intThresholdDrawHighValue);
                    //                }
                    //            }
                    //        }
                    //        else if (m_intSelectedTabPage == 1)
                    //        {
                    //            if (m_smVisionInfo.g_blnViewRotatedImage)
                    //            {
                    //                ImageDrawing objImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
                    //                ROI objROI = new ROI();
                    //                objROI.AttachImage(objImage);
                    //                objROI.LoadROISetting(m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROITotalX,
                    //                                                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROITotalY,
                    //                                                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROIWidth,
                    //                                                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROIHeight);

                    //                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue, m_smVisionInfo.g_intHighThresholdValue, objROI);
                    //            }
                    //            else
                    //            {
                    //                ImageDrawing objImage = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage];
                    //                ROI objROI = new ROI();
                    //                objROI.AttachImage(objImage);
                    //                objROI.LoadROISetting(m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROITotalX,
                    //                                                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROITotalY,
                    //                                                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROIWidth,
                    //                                                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].ref_ROIHeight);

                    //                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_intLowThresholdValue, m_smVisionInfo.g_intHighThresholdValue, objROI);
                    //            }
                    //        }
                    //    }
                    //    break;
                }
            }
            else if (m_smVisionInfo.g_blnViewColorThreshold)
            {
                switch (m_smVisionInfo.g_strSelectedPage)
                {
                    case "ColorPackage":
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                            m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_arrColorPackageROIs[0][m_smVisionInfo.g_intSelectedROI],
                            m_smVisionInfo.g_intColorThreshold, m_smVisionInfo.g_intColorTolerance, m_smVisionInfo.g_blnUseRGBFormat);
                        else
                            m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic, m_smVisionInfo.g_arrColorPackageROIs[0][m_smVisionInfo.g_intSelectedROI],
                            m_smVisionInfo.g_intColorThreshold, m_smVisionInfo.g_intColorTolerance, m_smVisionInfo.g_blnUseRGBFormat);
                        break;
                }
            }
            else
            {
                if (m_smVisionInfo.g_blnGrabbing)
                    return;

                //Production ON
                if (m_smVisionInfo.g_intMachineStatus == 2)
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        if (m_smVisionInfo.g_blnViewRotatedImage)
                            m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                        else
                            m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                    }
                    else
                    {
                        // View rotated image if Mark turn ON
                        if (m_smProductionInfo.g_blnViewInspection && m_smVisionInfo.VS_AT_ProductionTestDone)
                        {
                            if (m_smVisionInfo.g_intSelectedImage < m_smVisionInfo.g_arrRotatedImages.Count)
                            {
                                if (m_smVisionInfo.g_arrblnImageRotated[m_smVisionInfo.g_intSelectedImage])
                                    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                                else
                                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                            }
                        }
                        else
                        {
                            if (m_smVisionInfo.g_intSelectedImage < m_smVisionInfo.g_arrImages.Count)
                            {
                                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                            }
                        }

                    }
                }
                else if (m_smVisionInfo.g_blnViewPkgProcessImage)
                {
                    m_smVisionInfo.g_objPkgProcessImage.RedrawImage(m_Graphic);
                }
                else if (m_smVisionInfo.g_blnViewPackageImage && m_smVisionInfo.g_objPackageImage != null)
                {
                    if (m_smVisionInfo.g_strSelectedPage == "Calibrate5S")
                    {
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(m_smVisionInfo.g_objPackageImage);
                        if (m_smVisionInfo.g_intSelectedImage < m_smVisionInfo.g_arrImages.Count)
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objCalibration.ref_fImageGain);

                    }
                    else if (m_smVisionInfo.g_strSelectedPage == "BottomOrientPad" || m_smVisionInfo.g_strSelectedPage == "BottomOPadPkg" || m_smVisionInfo.g_strSelectedPage == "Pad" || m_smVisionInfo.g_strSelectedPage == "Pad5S" || m_smVisionInfo.g_strSelectedPage == "PadPackage")
                    {
                        //if (m_smVisionInfo.g_blnViewRotatedImage)
                        //    m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        //else
                        //    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
                        UpdatePackageImage_ForRectGauge4LPad();
                        if (chk_Auto.Checked)
                            m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].ref_objRectGauge4L.AddAutoDontCareThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_intSelectedUnit);
                    }
                    m_smVisionInfo.g_objPackageImage.RedrawImage(m_Graphic);

                }
                else if (m_smVisionInfo.g_blnViewGainImage)
                {
                    if (m_smVisionInfo.g_blnViewRotatedImage && m_smProductionInfo.g_blnViewInspection)
                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_fGainValue);
                    else
                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_fGainValue);

                    m_smVisionInfo.g_objPackageImage.RedrawImage(m_Graphic);
                }
                else if (m_smVisionInfo.g_blnViewSealImage)
                {
                    m_smVisionInfo.g_objSealImage.RedrawImage(m_Graphic);
                }
                else
                {
                    if (m_smVisionInfo.g_blnViewColorImage)
                    {
                        if (m_smVisionInfo.g_blnViewRotatedImage && m_smVisionInfo.g_arrColorRotatedImages.Count > m_smVisionInfo.g_intSelectedImage)
                            m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                        else if (m_smVisionInfo.g_arrColorRotatedImages.Count > m_smVisionInfo.g_intSelectedImage)
                            m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                    }
                    else
                    {

                        //View rotated image if true
                        if ((!m_smVisionInfo.VM_AT_SettingInDialog && !m_smVisionInfo.VM_AT_OfflinePageView) || m_smVisionInfo.AT_VM_ManualTestMode)
                        {
                            if (m_smVisionInfo.g_blnViewRotatedImage && m_smProductionInfo.g_blnViewInspection)
                            {
                                if (m_smVisionInfo.g_intSelectedImage < m_smVisionInfo.g_arrRotatedImages.Count)
                                {
                                    if (m_smVisionInfo.g_arrblnImageRotated[m_smVisionInfo.g_intSelectedImage])
                                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                                    else
                                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_intSelectedImage < m_smVisionInfo.g_arrImages.Count)
                                {
                                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                                }
                            }
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnViewRotatedImage)
                            {
                                if (m_smVisionInfo.g_intSelectedImage < m_smVisionInfo.g_arrRotatedImages.Count)
                                {
                                    if (m_smVisionInfo.g_arrblnImageRotated[m_smVisionInfo.g_intSelectedImage])
                                        m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                                    else
                                        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_intSelectedImage < m_smVisionInfo.g_arrImages.Count)
                                {
                                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawDontCareArea()
        {
            switch (m_smVisionInfo.g_strSelectedPage)
            {
                case "Orient":
                case "MarkOrient":
                case "MOLi":
                    if (m_blnDrawRectDone)
                    {
                        if (m_smVisionInfo.g_intLearnStepNo == 12)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrOrientGaugeM4L.Count; i++)
                            {
                                m_smVisionInfo.g_arrOrientGaugeM4L[i].DrawDontCareArea(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                            }
                        }
                    }
                    break;
                case "Package":
                    if (m_blnDrawRectDone)
                    {
                        if (m_smVisionInfo.g_intLearnStepNo == 1)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge2M4L.Count; i++)
                            {
                                m_smVisionInfo.g_arrPackageGauge2M4L[i].DrawDontCareArea(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                            }
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 0)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
                            {
                                m_smVisionInfo.g_arrPackageGaugeM4L[i].DrawDontCareArea(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                            }
                        }
                    }
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    if (m_blnDrawRectDone)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.DrawDontCareArea(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                        }
                    }

                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.DrawAutoDontCareBlob(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY, i);
                    }
                    break;
            }
        }

        private void DrawRectangle(Point p1, Point p2)
        {
            // Convert the points to screen coordinates.
            p1 = PointToScreen(p1);
            p2 = PointToScreen(p2);
            // Normalize the rectangle.
            if (p1.X < p2.X)
            {
                rc.X = p1.X;
                rc.Width = p2.X - p1.X;
            }
            else
            {
                rc.X = p2.X;
                rc.Width = p1.X - p2.X;
            }
            if (p1.Y < p2.Y)
            {
                rc.Y = p1.Y;
                rc.Height = p2.Y - p1.Y;
            }
            else
            {
                rc.Y = p2.Y;
                rc.Height = p1.Y - p2.Y;
            }
            // Draw the reversible frame.
            ControlPaint.DrawReversibleFrame(rc,
                            Color.White, FrameStyle.Dashed);
        }

        private void DrawROI()
        {
            if (m_smVisionInfo.g_blnViewROI)
            {
                switch (m_smVisionInfo.g_strSelectedPage)
                {
                    case "Mark":
                    case "MarkOrient":
                    case "MOLi":
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrOrientGaugeM4L.Count; i++)
                            {
                                m_smVisionInfo.g_arrOrientGaugeM4L[i].DrawEdgeROI(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY, m_smVisionInfo.g_arrMarkOrientROIColor[6][0]);
                            }

                            for (int i = 0; i < m_smVisionInfo.g_arrOrientGaugeM4L.Count; i++)
                            {
                                m_smVisionInfo.g_arrOrientGaugeM4L[i].DrawDontCareArea(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                            }
                        }
                        break;
                    case "ColorPackage":
                    case "Package":
                        {
                            if (m_smVisionInfo.g_intLearnStepNo == 1)
                            {
                                for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge2M4L.Count; i++)
                                {
                                    m_smVisionInfo.g_arrPackageGauge2M4L[i].DrawEdgeROI(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY, m_smVisionInfo.g_arrPackageROIColor[5][1]);
                                }

                                for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge2M4L.Count; i++)
                                {
                                    m_smVisionInfo.g_arrPackageGauge2M4L[i].DrawDontCareArea(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
                                {
                                    m_smVisionInfo.g_arrPackageGaugeM4L[i].DrawEdgeROI(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY, m_smVisionInfo.g_arrPackageROIColor[5][0]);
                                }

                                for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
                                {
                                    m_smVisionInfo.g_arrPackageGaugeM4L[i].DrawDontCareArea(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                                }
                            }
                        }
                        break;
                    case "BottomOrientPad":
                    case "BottomOPadPkg":
                    case "Pad":
                    case "Pad5S":
                    case "PadPackage":
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                    break;

                                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.DrawEdgeROI(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.DrawDontCareArea(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
                            }
                        }
                        break;
                }
            }
        }

        private void ModifyMarkImage()
        {
            if (m_smVisionInfo.g_intLearnStepNo == 12)
            {
                if (!m_blnDrawRectDone)
                {
                    m_blnDrawRectDone = true;
                    if (m_blnDrawRect)
                    {

                        if (m_smVisionInfo.g_fScaleX != 1f || m_smVisionInfo.g_fScaleY != 1f)
                        {
                            m_pRectStart = new Point((int)(m_pRectStart.X / m_smVisionInfo.g_fScaleX), (int)(m_pRectStart.Y / m_smVisionInfo.g_fScaleY));
                            m_pRectStop = new Point((int)(m_pRectStop.X / m_smVisionInfo.g_fScaleX), (int)(m_pRectStop.Y / m_smVisionInfo.g_fScaleY));
                        }

                        m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].AddDontCareROI(m_pRectStart, m_pRectStop);

                        m_blnDrawRect = false;
                        m_UpdatePictureBox = true;

                    }
                }
            }
        }

        private void ModifyPackageImage()
        {
            if (m_smVisionInfo.g_intLearnStepNo == 1)
            {
                if (!m_blnDrawRectDone)
                {
                    m_blnDrawRectDone = true;
                    if (m_blnDrawRect)
                    {
                        if (m_smVisionInfo.g_fScaleX != 1f || m_smVisionInfo.g_fScaleY != 1f)
                        {
                            m_pRectStart = new Point((int)(m_pRectStart.X / m_smVisionInfo.g_fScaleX), (int)(m_pRectStart.Y / m_smVisionInfo.g_fScaleY));
                            m_pRectStop = new Point((int)(m_pRectStop.X / m_smVisionInfo.g_fScaleX), (int)(m_pRectStop.Y / m_smVisionInfo.g_fScaleY));
                        }

                        m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].AddDontCareROI(m_pRectStart, m_pRectStop);

                        m_blnDrawRect = false;
                        m_UpdatePictureBox = true;
                    }
                }
            }
            else if (m_smVisionInfo.g_intLearnStepNo == 0)
            {
                if (!m_blnDrawRectDone)
                {
                    m_blnDrawRectDone = true;
                    if (m_blnDrawRect)
                    {
                        if (m_smVisionInfo.g_fScaleX != 1f || m_smVisionInfo.g_fScaleY != 1f)
                        {
                            m_pRectStart = new Point((int)(m_pRectStart.X / m_smVisionInfo.g_fScaleX), (int)(m_pRectStart.Y / m_smVisionInfo.g_fScaleY));
                            m_pRectStop = new Point((int)(m_pRectStop.X / m_smVisionInfo.g_fScaleX), (int)(m_pRectStop.Y / m_smVisionInfo.g_fScaleY));
                        }

                        m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].AddDontCareROI(m_pRectStart, m_pRectStop);

                        m_blnDrawRect = false;
                        m_UpdatePictureBox = true;
                    }
                }
            }
        }

        private void ModifyPadImage()
        {
            if (!m_blnDrawRectDone)
            {
                m_blnDrawRectDone = true;
                if (m_blnDrawRect)
                {

                    if (m_smVisionInfo.g_fScaleX != 1f || m_smVisionInfo.g_fScaleY != 1f)
                    {
                        m_pRectStart = new Point((int)(m_pRectStart.X / m_smVisionInfo.g_fScaleX), (int)(m_pRectStart.Y / m_smVisionInfo.g_fScaleY));
                        m_pRectStop = new Point((int)(m_pRectStop.X / m_smVisionInfo.g_fScaleX), (int)(m_pRectStop.Y / m_smVisionInfo.g_fScaleY));
                    }
                    
                    m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].ref_objRectGauge4L.AddDontCareROI(m_pRectStart, m_pRectStop);
                    m_blnDrawRect = false;
                    m_UpdatePictureBox = true;

                }
            }
        }

        private void VerifyROIArea(int intPositionX, int intPositionY)
        {
            int i = 0;
            // Hit Test - Detects if the cursor is placed over one of the dragging handles
            switch (m_smVisionInfo.g_strSelectedPage)
            {
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                    for (i = 0; i < m_smVisionInfo.g_arrOrientGaugeM4L.Count; i++)
                    {
                        if (m_smVisionInfo.g_arrOrientGaugeM4L[i].VerifyROI(intPositionX, intPositionY))
                        {
                            m_UpdatePictureBox = true;
                            m_smVisionInfo.g_intSelectedUnit = i;
                        }
                    }
                    break;
                case "Package":
                    if (m_smVisionInfo.g_intLearnStepNo == 1)
                    {
                        for (i = 0; i < m_smVisionInfo.g_arrPackageGauge2M4L.Count; i++)
                        {
                            if (m_smVisionInfo.g_arrPackageGauge2M4L[i].VerifyROI(intPositionX, intPositionY))
                            {
                                m_UpdatePictureBox = true;
                                m_smVisionInfo.g_intSelectedUnit = i;
                            }
                        }
                    }
                    else
                    {
                        for (i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
                        {
                            if (m_smVisionInfo.g_arrPackageGaugeM4L[i].VerifyROI(intPositionX, intPositionY))
                            {
                                m_UpdatePictureBox = true;
                                m_smVisionInfo.g_intSelectedUnit = i;
                            }
                        }
                    }
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    for (i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        if (m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.VerifyROI(intPositionX, intPositionY))
                        {
                            m_UpdatePictureBox = true;
                            m_smVisionInfo.g_intSelectedUnit = i;
                            if (m_intSelectedUnit != m_smVisionInfo.g_intSelectedUnit)
                            {
                                m_intSelectedUnit = m_smVisionInfo.g_intSelectedUnit;
                                m_intSelectedEdgeROI = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_intSelectedEdgeROIPrev;
                                m_blnFirstClick = true;
                            }
                            else
                            {
                                if (m_intSelectedEdgeROI != m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_intSelectedEdgeROIPrev)
                                {
                                    m_intSelectedEdgeROI = m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_intSelectedEdgeROIPrev;
                                    m_blnFirstClick = true;
                                }
                                else
                                    m_blnFirstClick = false;
                            }
                        }
                    }
                    UpdateDontCareSetting();
                    break;
            }
        }

        private void pic_Image_Paint(object sender, PaintEventArgs e)
        {
            m_UpdatePictureBox = true;
        }

        private void pnl_PictureBox_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
            {
                int intScollValue = e.NewValue;
                m_intScollValueXPrev = intScollValue;
            }

            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                int intScollValue = e.NewValue;
                m_intScollValueYPrev = intScollValue;
            }
        }

        private void DontCareAreaSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_fScaleX = m_fScaleXPrev;
            m_smVisionInfo.g_fScaleY = m_fScaleYPrev;
            m_fZoomCount = m_smVisionInfo.g_fZoomScale = m_fZoomScalePrev;
            DrawZoomInOutImage();
        }

        private void ClearDragHandle()
        {
            switch (m_smVisionInfo.g_strSelectedPage)
            {
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                    for (int i = 0; i < m_smVisionInfo.g_arrOrientGaugeM4L.Count; i++)
                    {
                        m_smVisionInfo.g_arrOrientGaugeM4L[i].ClearDragHandle();
                    }
                    break;
                case "Package":
                    for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
                    {
                        m_smVisionInfo.g_arrPackageGaugeM4L[i].ClearDragHandle();
                    }
                    for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge2M4L.Count; i++)
                    {
                        m_smVisionInfo.g_arrPackageGauge2M4L[i].ClearDragHandle();
                    }
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ClearDragHandle();
                    }
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_UpdatePictureBox)
            {
                m_UpdatePictureBox = false;
                DrawZoomInOutImage();
                DrawMainImage();
                DrawROI();
                DrawDontCareArea();
                MeasureGauge();
            }

            if (!this.TopMost)
                this.TopMost = true;

            //2021-01-08 ZJYEOH : To solve form missing because running in background
            if (!this.Focused)
                this.BringToFront();
        }

        private void chk_Auto_Click(object sender, EventArgs e)
        {
            if (m_blnUpdating)
                return;

            switch (m_smVisionInfo.g_strSelectedPage)
            {
                //case "Orient":
                //case "MarkOrient":
                //case "MOLi":
                //    if (m_smVisionInfo.g_intLearnStepNo == 12)
                //    {
                //        if(m_smVisionInfo.g_arrOrientGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //        }
                //    }
                //    break;
                //case "Package":
                //    if (m_smVisionInfo.g_intLearnStepNo == 1)
                //    {
                //        if(m_smVisionInfo.g_arrPackageGauge2M4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //        }
                //    }
                //    else if (m_smVisionInfo.g_intLearnStepNo == 0)
                //    {
                //        if(m_smVisionInfo.g_arrPackageGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //        }
                //    }
                //    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    if (m_smVisionInfo.g_arrPad.Length > m_smVisionInfo.g_intSelectedUnit)
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].ref_objRectGauge4L.SetCurrentAutoDontCareEnabled(chk_Auto.Checked, m_smVisionInfo.g_intSelectedUnit);

                        pnl_Auto.Visible = chk_Auto.Checked;
                        pnl_Manual.Visible = !chk_Auto.Checked;
                    }
                    break;
            }
            m_UpdatePictureBox = true;
        }

        private void txt_Offset_TextChanged(object sender, EventArgs e)
        {
            if (m_blnUpdating)
                return;

            trackBar_Offset.Value = Convert.ToInt32( txt_Offset.Text);

            switch (m_smVisionInfo.g_strSelectedPage)
            {
                //case "Orient":
                //case "MarkOrient":
                //case "MOLi":
                //    if (m_smVisionInfo.g_intLearnStepNo == 12)
                //    {
                //        if(m_smVisionInfo.g_arrOrientGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //        }
                //    }
                //    break;
                //case "Package":
                //    if (m_smVisionInfo.g_intLearnStepNo == 1)
                //    {
                //        if(m_smVisionInfo.g_arrPackageGauge2M4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //        }
                //    }
                //    else if (m_smVisionInfo.g_intLearnStepNo == 0)
                //    {
                //        if(m_smVisionInfo.g_arrPackageGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //        }
                //    }
                //    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    if (m_smVisionInfo.g_arrPad.Length > m_smVisionInfo.g_intSelectedUnit)
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].ref_objRectGauge4L.SetCurrentAutoDontCareOffset(Convert.ToInt32(txt_Offset.Text), m_smVisionInfo.g_intSelectedUnit);
                        m_UpdatePictureBox = true;
                    }
                    break;
            }
        }

        private void trackBar_Offset_Scroll(object sender, EventArgs e)
        {
            if (m_blnUpdating)
                return;

            txt_Offset.Text = trackBar_Offset.Value.ToString();
        }

        private void txt_Threshold_TextChanged(object sender, EventArgs e)
        {
            if (m_blnUpdating)
                return;

            trackBar_Threshold.Value = Convert.ToInt32(txt_Threshold.Text);

            switch (m_smVisionInfo.g_strSelectedPage)
            {
                //case "Orient":
                //case "MarkOrient":
                //case "MOLi":
                //    if (m_smVisionInfo.g_intLearnStepNo == 12)
                //    {
                //        if(m_smVisionInfo.g_arrOrientGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //        }
                //    }
                //    break;
                //case "Package":
                //    if (m_smVisionInfo.g_intLearnStepNo == 1)
                //    {
                //        if(m_smVisionInfo.g_arrPackageGauge2M4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //        }
                //    }
                //    else if (m_smVisionInfo.g_intLearnStepNo == 0)
                //    {
                //        if(m_smVisionInfo.g_arrPackageGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //        }
                //    }
                //    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    if (m_smVisionInfo.g_arrPad.Length > m_smVisionInfo.g_intSelectedUnit)
                    {
                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].ref_objRectGauge4L.SetCurrentAutoDontCareThreshold(Convert.ToInt32(txt_Threshold.Text), m_smVisionInfo.g_intSelectedUnit);
                        m_UpdatePictureBox = true;
                    }
                    break;
            }
        }

        private void trackBar_Threshold_Scroll(object sender, EventArgs e)
        {
            if (m_blnUpdating)
                return;

            txt_Threshold.Text = trackBar_Threshold.Value.ToString();
        }

        private void pic_Image_MouseDown(object sender, MouseEventArgs e)
        {
            int intPositionX = m_intMouseHitX = (int)Math.Round(e.X / m_smVisionInfo.g_fScaleX, 0, MidpointRounding.AwayFromZero);
            int intPositionY = m_intMouseHitY = (int)Math.Round(e.Y / m_smVisionInfo.g_fScaleY, 0, MidpointRounding.AwayFromZero);

            ClearDragHandle();
            VerifyROIArea(intPositionX, intPositionY);

            switch (m_smVisionInfo.g_strSelectedPage)
            {
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                    if (m_blnDrawRectDone)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrOrientGaugeM4L.Count; i++)
                        {
                            //Make sure mouse is within ROI
                            if (m_smVisionInfo.g_arrOrientGaugeM4L[i].GetEdgeROIHandle())
                            {
                                m_pRectStart.X = e.X;   // For Drawing, use e.X value (picture box location), not intPositionX (Actual size Image location)
                                m_pRectStart.Y = e.Y;   // For Drawing, use e.Y value (picture box location), not intPositionY (Actual size Image location)
                                m_pRectStop.X = -1;
                                m_pRectStop.Y = -1;
                                m_blnDrawRectDone = false;
                            }
                        }
                    }
                    break;
                case "Package":
                    if (m_blnDrawRectDone)
                    {
                        if (m_smVisionInfo.g_intLearnStepNo == 1)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPackageGauge2M4L.Count; i++)
                            {
                                if (m_smVisionInfo.g_arrPackageROIs[i].Count > 0)
                                {
                                    if (i < m_smVisionInfo.g_arrPackageROIs.Count && m_smVisionInfo.g_arrPackageROIs[i].Count > 1)
                                    {
                                        //Make sure mouse is within ROI
                                        if (m_smVisionInfo.g_arrPackageGauge2M4L[i].GetEdgeROIHandle())
                                        {
                                            m_pRectStart.X = e.X;   // For Drawing, use e.X value (picture box location), not intPositionX (Actual size Image location)
                                            m_pRectStart.Y = e.Y;   // For Drawing, use e.Y value (picture box location), not intPositionY (Actual size Image location)
                                            m_pRectStop.X = -1;
                                            m_pRectStop.Y = -1;
                                            m_blnDrawRectDone = false;
                                        }
                                    }
                                }
                            }
                        }
                        else if (m_smVisionInfo.g_intLearnStepNo == 0)
                        {
                            for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L.Count; i++)
                            {
                                if (m_smVisionInfo.g_arrPackageROIs[i].Count > 0)
                                {
                                    if (i < m_smVisionInfo.g_arrPackageROIs.Count && m_smVisionInfo.g_arrPackageROIs[i].Count > 1)
                                    {
                                        //Make sure mouse is within ROI
                                        if (m_smVisionInfo.g_arrPackageGaugeM4L[i].GetEdgeROIHandle())
                                        {
                                            m_pRectStart.X = e.X;   // For Drawing, use e.X value (picture box location), not intPositionX (Actual size Image location)
                                            m_pRectStart.Y = e.Y;   // For Drawing, use e.Y value (picture box location), not intPositionY (Actual size Image location)
                                            m_pRectStop.X = -1;
                                            m_pRectStop.Y = -1;
                                            m_blnDrawRectDone = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    if (m_blnDrawRectDone && !chk_Auto.Checked && !m_blnFirstClick)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                                break;

                            //Make sure mouse is within ROI
                            if (m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.GetEdgeROIHandle())
                            {
                                m_pRectStart.X = e.X;   // For Drawing, use e.X value (picture box location), not intPositionX (Actual size Image location)
                                m_pRectStart.Y = e.Y;   // For Drawing, use e.Y value (picture box location), not intPositionY (Actual size Image location)
                                m_pRectStop.X = -1;
                                m_pRectStop.Y = -1;
                                m_blnDrawRectDone = false;
                            }
                        }
                    }
                    break;
            }
        }

        private void pic_Image_MouseMove(object sender, MouseEventArgs e)
        {
            //if (m_smVisionInfo.g_strSelectedPage == "Mark" || m_smVisionInfo.g_strSelectedPage == "MarkOrient")
            //{
            //    if (m_smVisionInfo.g_intLearnStepNo == 12 && !m_blnDrawRectDone)
            //    {
            //        Point pCurPoint = new Point(e.X, e.Y);    // current mouse move rectange points (use e.X and e.Y picture box location, not intPositionXY (actual size image location)
            //        if (m_pRectStop.X != -1)
            //        {
            //            DrawRectangle(m_pRectStart, m_pRectStop);
            //        }
            //        m_pRectStop = pCurPoint;
            //        // Draw new lines.
            //        DrawRectangle(m_pRectStart, m_pRectStop);
            //        m_blnDrawRect = true;
            //    }
            //}
            //else if (m_smVisionInfo.g_strSelectedPage == "Package")
            //{
            //    if ((m_smVisionInfo.g_intLearnStepNo == 0 || m_smVisionInfo.g_intLearnStepNo == 1) && !m_blnDrawRectDone)
            //    {
            //        Point pCurPoint = new Point(e.X, e.Y);    // current mouse move rectange points (use e.X and e.Y picture box location, not intPositionXY (actual size image location)
            //        if (m_pRectStop.X != -1)
            //        {
            //            DrawRectangle(m_pRectStart, m_pRectStop);
            //        }
            //        m_pRectStop = pCurPoint;
            //        // Draw new lines.
            //        DrawRectangle(m_pRectStart, m_pRectStop);
            //        m_blnDrawRect = true;
            //    }
            //}
            //else if (m_smVisionInfo.g_strSelectedPage == "Pad" || m_smVisionInfo.g_strSelectedPage == "Pad5S" || m_smVisionInfo.g_strSelectedPage == "PadPackage")
            //{
            //    if (!m_blnDrawRectDone)
            //    {
            //        Point pCurPoint = new Point(e.X, e.Y);    // current mouse move rectange points (use e.X and e.Y picture box location, not intPositionXY (actual size image location)
            //        if (m_pRectStop.X != -1)
            //        {
            //            DrawRectangle(m_pRectStart, m_pRectStop);
            //        }
            //        m_pRectStop = pCurPoint;
            //        // Draw new lines.
            //        DrawRectangle(m_pRectStart, m_pRectStop);
            //        m_blnDrawRect = true;
            //    }
            //}

            if (!m_blnDrawRectDone)
            {
                Point pCurPoint = new Point(e.X, e.Y);    // current mouse move rectange points (use e.X and e.Y picture box location, not intPositionXY (actual size image location)
                if (m_pRectStop.X != -1)
                {
                    DrawRectangle(m_pRectStart, m_pRectStop);
                }
                m_pRectStop = pCurPoint;
                // Draw new lines.
                DrawRectangle(m_pRectStart, m_pRectStop);
                m_blnDrawRect = true;
            }

            m_UpdatePictureBox = true;
        }

        private void pic_Image_MouseUp(object sender, MouseEventArgs e)
        {
            switch (m_smVisionInfo.g_strSelectedPage)
            {
                case "Package":
                    ModifyPackageImage();
                    break;
                case "MarkOrient":
                case "MOLi":
                    ModifyMarkImage();
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    ModifyPadImage();
                    break;
            }
        }

        private void btn_UndoROI_Click(object sender, EventArgs e)
        {
            switch (m_smVisionInfo.g_strSelectedPage)
            {
                case "Orient":
                case "MarkOrient":
                case "MOLi":
                    m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].RemoveDontCareROI();
                    break;
                case "Package":
                    if (m_smVisionInfo.g_intLearnStepNo == 1)
                    {
                        m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].RemoveDontCareROI();
                    }
                    else
                    {
                        m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].RemoveDontCareROI();
                    }
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].ref_objRectGauge4L.RemoveDontCareROI();
                    break;
            }

            m_UpdatePictureBox = true;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            //Dont save here save at parent page
            ClearDragHandle();
            Close();
            Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            ClearDragHandle();

            //2020-06-23 ZJYEOH : cannot load from xml, as it will delete all current dont care area before save gauge setting
            //switch (m_smVisionInfo.g_strSelectedPage)
            //{
            //    case "Orient":
            //    case "MarkOrient":
            //    case "MOLi":
            //        m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].LoadRectGauge4L_DontCareROIOnly(m_strPath, "RectG" + m_smVisionInfo.g_intSelectedUnit.ToString());
            //        break;
            //    case "Package":
            //        if (m_smVisionInfo.g_intLearnStepNo == 1)
            //        {
            //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].LoadRectGauge4L_DontCareROIOnly(m_strPath, "RectG" + m_smVisionInfo.g_intSelectedUnit.ToString());
            //        }
            //        else
            //        {
            //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].LoadRectGauge4L_DontCareROIOnly(m_strPath, "RectG" + m_smVisionInfo.g_intSelectedUnit.ToString());
            //        }
            //        break;
            //    case "Pad":
            //    case "Pad5S":
            //    case "PadPackage":
            //        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].ref_objRectGauge4L.LoadRectGauge4L_DontCareROIOnly(m_strPath, "RectG" + m_smVisionInfo.g_intSelectedUnit.ToString());
            //        break;
            //}

            switch (m_smVisionInfo.g_strSelectedPage)
            {
                case "Orient":
                case "MarkOrient":
                case "MOLi":
                    for (int i = 0; i < m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_arrEdgeROI.Count; i++)
                    {
                        m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].RemoveDontCareROI(i, m_arrMarkDontCareCount[i]);
                        m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].AddDontCareROI(i, m_arrMarkDontCareCount[i], m_arrMarkDontCareStartX[i], m_arrMarkDontCareStartY[i], m_arrMarkDontCareWidth[i], m_arrMarkDontCareHeight[i]);
                    }
                    break;
                case "Package":
                    if (m_smVisionInfo.g_intLearnStepNo == 1)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_arrEdgeROI.Count; i++)
                        {
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].RemoveDontCareROI(i, m_arrPackageDontCareCount[i]);
                            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].AddDontCareROI(i, m_arrPackageDontCareCount[i], m_arrPackageDontCareStartX[i], m_arrPackageDontCareStartY[i], m_arrPackageDontCareWidth[i], m_arrPackageDontCareHeight[i]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].ref_arrEdgeROI.Count; i++)
                        {
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].RemoveDontCareROI(i, m_arrPackageDontCareCount[i]);
                            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].AddDontCareROI(i, m_arrPackageDontCareCount[i], m_arrPackageDontCareStartX[i], m_arrPackageDontCareStartY[i], m_arrPackageDontCareWidth[i], m_arrPackageDontCareHeight[i]);
                        }
                    }
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                            break;

                        for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ref_arrEdgeROI.Count; j++)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.RemoveDontCareROI(j, m_arrPadDontCareCount[i][j]);
                            m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.AddDontCareROI(j, m_arrPadDontCareCount[i][j], m_arrPadDontCareStartX[i][j], m_arrPadDontCareStartY[i][j], m_arrPadDontCareWidth[i][j], m_arrPadDontCareHeight[i][j]);
                        }
                        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetCurrentAutoDontCareThreshold(m_arrAutoDontCareThreshold[i]);
                        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetCurrentAutoDontCareOffset(m_arrAutoDontCareOffset[i]);
                        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetCurrentAutoDontCareEnabled(m_arrAutoDontCare[i]);
                    }
                    break;
            }

            Close();
            Dispose();
        }
        private void DrawZoomInOutImage()
        {
            if (m_fZoomCountPrev != m_fZoomCount)
            {
                m_fZoomCountPrev = m_fZoomCount;

                m_smVisionInfo.g_fScaleX = m_fOriScaleX * m_fZoomCount;
                m_smVisionInfo.g_fScaleY = m_fOriScaleY * m_fZoomCount;

                SetScaleToComponents(false, false, false);

                if (m_fZoomCount == 1)
                {
                    pic_Image.Size = pnl_PictureBox.Size;

                    pnl_PictureBox.AutoScroll = false;

                    m_intScollValueXPrev = 0;
                    m_intScollValueYPrev = 0;

                    if ((float)m_smVisionInfo.g_intCameraResolutionHeight / (float)m_smVisionInfo.g_intCameraResolutionWidth != 0.75)
                    {
                        if ((float)pic_Image.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth >= (float)pic_Image.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight)
                        {
                            pnl_PicSideBlock.Location = new Point((int)(m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX), 0);
                            pnl_PicSideBlock.Width = (int)(pic_Image.Size.Width - m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX);
                            pnl_PicSideBlock.Height = pic_Image.Size.Height;
                            pnl_PicSideBlock.BringToFront();
                        }
                        else
                        {
                            pnl_PicSideBlock.Location = new Point(0, (int)(m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY));
                            pnl_PicSideBlock.Width = pic_Image.Size.Width;
                            pnl_PicSideBlock.Height = (int)(pic_Image.Size.Height - m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY);
                            pnl_PicSideBlock.BringToFront();
                        }
                    }
                }
                else
                {
                    int intFocusPointX = 0;
                    int intFocusPointY = 0;
                    float fFocusRatioX = 0;
                    float fFocusRatioY = 0;
                    int intPicImageWidthPrev = 0;
                    int intPicImageHeightPrev = 0;

                    intPicImageWidthPrev = pic_Image.Size.Width;
                    intPicImageHeightPrev = pic_Image.Size.Height;
                    pic_Image.Size = new Size((int)Math.Ceiling((float)pnl_PictureBox.Size.Width * m_fZoomCount), (int)Math.Ceiling((float)pnl_PictureBox.Size.Height * m_fZoomCount));
                    if ((float)m_smVisionInfo.g_intCameraResolutionHeight / (float)m_smVisionInfo.g_intCameraResolutionWidth != 0.75)
                    {
                        if ((float)pic_Image.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth >= (float)pic_Image.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight)
                        {
                            pnl_PicSideBlock.Location = new Point((int)(m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX), 0);
                            pnl_PicSideBlock.Width = (int)(pic_Image.Size.Width - m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX);
                            pnl_PicSideBlock.Height = pic_Image.Size.Height;
                            pnl_PicSideBlock.BringToFront();
                        }
                        else
                        {
                            pnl_PicSideBlock.Location = new Point(0, (int)(m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY));
                            pnl_PicSideBlock.Width = pic_Image.Size.Width;
                            pnl_PicSideBlock.Height = (int)(pic_Image.Size.Height - m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY);
                            pnl_PicSideBlock.BringToFront();
                        }
                    }

                    fFocusRatioX = (float)m_intZoomImageFocusPointX / intPicImageWidthPrev;
                    fFocusRatioY = (float)m_intZoomImageFocusPointY / intPicImageHeightPrev;
                    intFocusPointX = (int)Math.Ceiling(fFocusRatioX * pic_Image.Size.Width - 320);
                    intFocusPointY = (int)Math.Ceiling(fFocusRatioY * pic_Image.Size.Height - 240);

                    pnl_PictureBox.AutoScroll = true;

                    if ((pic_Image.Size.Width - 623) >= intFocusPointX && intFocusPointX >= 0)
                    {
                        while (pnl_PictureBox.HorizontalScroll.Value != intFocusPointX)
                        {
                            if (intFocusPointX > 0) // 2019-12-26 ZJYEOH : Negative value will cause error
                            {
                                pnl_PictureBox.HorizontalScroll.Value = intFocusPointX;
                                break;
                            }
                            else
                                break;
                            Thread.Sleep(1);
                        }
                    }
                    else if (intFocusPointX < 0)
                    {
                        while (pnl_PictureBox.HorizontalScroll.Value != 0)
                        {
                            pnl_PictureBox.HorizontalScroll.Value = 0;
                            Thread.Sleep(1);
                        }
                    }
                    else
                    {
                        while (pnl_PictureBox.HorizontalScroll.Value != (pic_Image.Size.Width - 623)) // Picture size width - panel width + 17 is the maximum value for horizontal
                        {
                            if ((pic_Image.Size.Width - 623) > 0)// 2019-12-26 ZJYEOH : Negative value will cause error
                            {
                                pnl_PictureBox.HorizontalScroll.Value = (pic_Image.Size.Width - 623);
                                break;
                            }
                            else
                            {
                                break;
                            }
                            Thread.Sleep(1);
                        }
                    }

                    if ((pic_Image.Size.Height - 463) >= intFocusPointY && intFocusPointY >= 0)
                    {
                        while (pnl_PictureBox.VerticalScroll.Value != intFocusPointY)
                        {
                            if (intFocusPointY > 0)// 2019-12-26 ZJYEOH : Negative value will cause error
                            {
                                pnl_PictureBox.VerticalScroll.Value = intFocusPointY;
                                break;
                            }
                            else
                                break;
                            Thread.Sleep(1);
                        }
                    }
                    else if (intFocusPointY < 0)
                    {
                        while (pnl_PictureBox.VerticalScroll.Value != 0)
                        {
                            pnl_PictureBox.VerticalScroll.Value = 0;
                            Thread.Sleep(1);
                        }
                    }
                    else
                    {
                        while (pnl_PictureBox.VerticalScroll.Value != (pic_Image.Size.Height - 463)) // Picture size Height - panel height + 17 is the maximum value for vertical
                        {
                            if ((pic_Image.Size.Height - 463) > 0)// 2019-12-26 ZJYEOH : Negative value will cause error
                            {
                                pnl_PictureBox.VerticalScroll.Value = (pic_Image.Size.Height - 463);
                                break;
                            }
                            else
                                break;
                            Thread.Sleep(1);
                        }
                    }
                }

                m_Graphic = Graphics.FromHwnd(pic_Image.Handle);
            }
        }
        private void SetScaleToComponents(bool blnFirstTime, bool blnScaleToPictureBox, bool blnScaleToPictureBox2)
        {
            if (blnFirstTime)
            {
                if ((float)m_smVisionInfo.g_intCameraResolutionHeight / (float)m_smVisionInfo.g_intCameraResolutionWidth != 0.75)
                {
                    if ((float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth >= (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight)
                    {
                        m_smVisionInfo.g_fScaleX = m_fOriScaleX = (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight;
                        m_smVisionInfo.g_fScaleY = m_fOriScaleY = (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight;

                        pnl_PicSideBlock.Location = new Point((int)(m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX), 0);
                        pnl_PicSideBlock.Width = (int)(pnl_PictureBox.Size.Width - m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX);
                        pnl_PicSideBlock.Height = pnl_PictureBox.Size.Height;
                        pnl_PicSideBlock.BringToFront();
                    }
                    else
                    {
                        m_smVisionInfo.g_fScaleX = m_fOriScaleX = (float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth;
                        m_smVisionInfo.g_fScaleY = m_fOriScaleY = (float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth;

                        pnl_PicSideBlock.Location = new Point(0, (int)(m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY));
                        pnl_PicSideBlock.Width = pnl_PictureBox.Size.Width;
                        pnl_PicSideBlock.Height = (int)(pnl_PictureBox.Size.Height - m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY);
                        pnl_PicSideBlock.BringToFront();
                    }
                }
                else
                {
                    m_smVisionInfo.g_fScaleX = m_fOriScaleX = (float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth;
                    m_smVisionInfo.g_fScaleY = m_fOriScaleY = (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight;
                }
            }

            if (blnScaleToPictureBox)
            {
                if ((float)m_smVisionInfo.g_intCameraResolutionHeight / (float)m_smVisionInfo.g_intCameraResolutionWidth != 0.75)
                {
                    if ((float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth >= (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight)
                    {
                        m_smVisionInfo.g_fScaleX = (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight;
                        m_smVisionInfo.g_fScaleY = (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight;

                        pnl_PicSideBlock.Location = new Point((int)(m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX), 0);
                        pnl_PicSideBlock.Width = (int)(pnl_PictureBox.Size.Width - m_smVisionInfo.g_intCameraResolutionWidth * m_smVisionInfo.g_fScaleX);
                        pnl_PicSideBlock.Height = pnl_PictureBox.Size.Height;
                        pnl_PicSideBlock.BringToFront();
                    }
                    else
                    {
                        m_smVisionInfo.g_fScaleX = (float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth;
                        m_smVisionInfo.g_fScaleY = (float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth;

                        pnl_PicSideBlock.Location = new Point(0, (int)(m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY));
                        pnl_PicSideBlock.Width = pnl_PictureBox.Size.Width;
                        pnl_PicSideBlock.Height = (int)(pnl_PictureBox.Size.Height - m_smVisionInfo.g_intCameraResolutionHeight * m_smVisionInfo.g_fScaleY);
                        pnl_PicSideBlock.BringToFront();
                    }
                }
                else
                {
                    m_smVisionInfo.g_fScaleX = (float)pnl_PictureBox.Size.Width / (float)m_smVisionInfo.g_intCameraResolutionWidth;
                    m_smVisionInfo.g_fScaleY = (float)pnl_PictureBox.Size.Height / (float)m_smVisionInfo.g_intCameraResolutionHeight;
                }
            }


            if (m_smVisionInfo.g_objMemoryImage != null)
            {
                m_smVisionInfo.g_objMemoryImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_objMemoryImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_ojRotateImage != null)
            {
                m_smVisionInfo.g_ojRotateImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_ojRotateImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_objPackageImage != null)
            {
                m_smVisionInfo.g_objPackageImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_objPackageImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_objPkgProcessImage != null)
            {
                m_smVisionInfo.g_objPkgProcessImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_objPkgProcessImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_objSealImage != null)
            {
                m_smVisionInfo.g_objSealImage.ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                m_smVisionInfo.g_objSealImage.ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
            }

            if (m_smVisionInfo.g_arrImages != null)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrImages.Count; i++)
                {
                    if (m_smVisionInfo.g_arrImages[i] != null)
                    {
                        m_smVisionInfo.g_arrImages[i].ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                        m_smVisionInfo.g_arrImages[i].ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
                    }
                }
            }

            if (m_smVisionInfo.g_arrRotatedImages != null)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrRotatedImages.Count; i++)
                {
                    if (m_smVisionInfo.g_arrRotatedImages[i] != null)
                    {
                        m_smVisionInfo.g_arrRotatedImages[i].ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                        m_smVisionInfo.g_arrRotatedImages[i].ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
                    }
                }
            }

            for (int h = 0; h < m_smVisionInfo.g_arr5SRotatedImages.Length; h++)
            {
                if (m_smVisionInfo.g_arr5SRotatedImages[h] != null)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arr5SRotatedImages[h].Count; i++)
                    {
                        if (m_smVisionInfo.g_arr5SRotatedImages[h][i] != null)
                        {
                            m_smVisionInfo.g_arr5SRotatedImages[h][i].ref_fDrawingScaleX = m_smVisionInfo.g_fScaleX;
                            m_smVisionInfo.g_arr5SRotatedImages[h][i].ref_fDrawingScaleY = m_smVisionInfo.g_fScaleY;
                        }
                    }
                }
            }

            if (m_smVisionInfo.g_WorldShape != null)
                m_smVisionInfo.g_WorldShape.SetZoom(m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY);
        }

        private void MeasureGauge()
        {
            if (m_blnUpdating || !chk_Auto.Checked)
                return;
            
            switch (m_smVisionInfo.g_strSelectedPage)
            {
                //case "Orient":
                //case "MarkOrient":
                //case "MOLi":
                //    if (m_smVisionInfo.g_intLearnStepNo == 12)
                //    {
                //        if(m_smVisionInfo.g_arrOrientGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrOrientGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //        }
                //    }
                //    break;
                //case "Package":
                //    if (m_smVisionInfo.g_intLearnStepNo == 1)
                //    {
                //        if(m_smVisionInfo.g_arrPackageGauge2M4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGauge2M4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //        }
                //    }
                //    else if (m_smVisionInfo.g_intLearnStepNo == 0)
                //    {
                //        if(m_smVisionInfo.g_arrPackageGaugeM4L.Count > m_smVisionInfo.g_intSelectedUnit)
                //        {
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //            m_smVisionInfo.g_arrPackageGaugeM4L[m_smVisionInfo.g_intSelectedUnit].SetCurrentAutoDontCareEnabled(chk_Auto.Checked);
                //        }
                //    }
                //    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "Pad5S":
                case "PadPackage":
                    if (m_smVisionInfo.g_arrPad.Length > m_smVisionInfo.g_intSelectedUnit)
                    {
                        if (m_smVisionInfo.g_intSelectedUnit == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                        {
                            // 2020-02-17 ZJYEOH : no need measure center ROI gauge if use side pkg to measure center pkg
                        }
                        else
                        {
                            if (m_smVisionInfo.g_blnViewRotatedImage)
                                m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_objWhiteImage, m_smVisionInfo.g_blnReferTemplateSize);
                            else
                                m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage, m_smVisionInfo.g_blnReferTemplateSize);
                        }

                        m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedUnit].ref_objRectGauge4L.DrawAutoDontCareBlob(m_Graphic, m_smVisionInfo.g_fScaleX, m_smVisionInfo.g_fScaleY, m_smVisionInfo.g_intSelectedUnit);
                    }
                    break;
            }
        }
        private void UpdatePackageImage_ForRectGauge4LPad()
        {
            if (m_smVisionInfo.g_blnViewRotatedImage)
            {
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            }
            else
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);
            }

            int intPadSelectedIndex = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                    continue;

                if (m_smVisionInfo.g_intSelectedROI != i)
                    continue;

                intPadSelectedIndex = i;
                //if (m_intPadSelectedIndex == 0)
                //{
                //    intPadSelectedIndex = m_intPadSelectedIndex;
                //}
                //else
                //{
                //    if (i == 0)
                //        continue;

                //    if (chk_ApplyToAllSideROI.Checked)
                //    {
                //        intPadSelectedIndex = i;
                //    }
                //    else
                //    {
                //        intPadSelectedIndex = m_intPadSelectedIndex;
                //    }
                //}

                if (m_smVisionInfo.g_blnViewRotatedImage)
                {
                    //if (m_blnSetToAll)
                    {
                        ImageDrawing objSourceImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
                        for (int j = 0; j < 4; j++)
                        {
                            if ((m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_intSelectedGaugeEdgeMask & (0x01 << j)) > 0)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(j) == 0)
                                {

                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, j);
                                    if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(j))
                                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddOpenCloseForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, j);
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, j);
                                }
                            }
                        }
                    }
                    //else
                    //{
                    //    int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                    //    ImageDrawing objSourceImage = m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage];
                    //    if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(intLineGaugeSelectedIndex) == 0)
                    //    {
                    //        m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                    //        if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(intLineGaugeSelectedIndex))
                    //            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                    //        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddOpenCloseForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                    //    }
                    //    else
                    //    {
                    //        m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, intLineGaugeSelectedIndex);
                    //        m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);

                    //    }
                    //}
                }
                else
                {
                    //if (m_blnSetToAll)
                    {
                        ImageDrawing objSourceImage = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage];
                        for (int j = 0; j < 4; j++)
                        {
                            if ((m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_intSelectedGaugeEdgeMask & (0x01 << j)) > 0)
                            {
                                if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(j) == 0)
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, j);
                                    if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(j))
                                        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddOpenCloseForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, j);
                                }
                                else
                                {
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, j);
                                    m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, j);
                                }
                            }

                        }
                    }
                    //else
                    //{
                    //    int intLineGaugeSelectedIndex = GetLineGaugeSelectedIndex(intPadSelectedIndex);
                    //    ImageDrawing objSourceImage = m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage];
                    //    if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeImageMode(intLineGaugeSelectedIndex) == 0)
                    //    {
                    //        m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                    //        if (m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.GetGaugeWantImageThreshold(intLineGaugeSelectedIndex))
                    //            m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddThresholdForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                    //        m_smVisionInfo.g_arrPad[intPadSelectedIndex].ref_objRectGauge4L.AddOpenCloseForEdgeROI(ref m_smVisionInfo.g_objPackageImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                    //    }
                    //    else
                    //    {
                    //        m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddGainForEdgeROI(ref objSourceImage, ref m_smVisionInfo.g_objPkgProcessImage, intLineGaugeSelectedIndex);
                    //        m_smVisionInfo.g_arrPad[intPadSelectedIndex].AddPrewittForEdgeROI(ref m_smVisionInfo.g_objPkgProcessImage, ref m_smVisionInfo.g_objPackageImage, intLineGaugeSelectedIndex);
                    //    }
                    //}
                }

                //if (!chk_ApplyToAllSideROI.Checked)
                //    break;
            }
        }
    }
}
