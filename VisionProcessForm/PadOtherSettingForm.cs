using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using VisionProcessing;
using SharedMemory;
using Microsoft.Win32;


namespace VisionProcessForm
{
    public partial class PadOtherSettingForm : Form
    {
        #region Member Variables
        private bool m_blnPackageROISettingVisible = true;

        private Point m_pPkgTop, m_pPkgRight, m_pPkgBottom, m_pPkgLeft;
        private Point m_pChippedTop, m_pChippedRight, m_pChippedBottom, m_pChippedLeft;
        private Point m_pChippedTop_Dark, m_pChippedRight_Dark, m_pChippedBottom_Dark, m_pChippedLeft_Dark;
        private Point m_pMoldTop, m_pMoldRight, m_pMoldBottom, m_pMoldLeft;
        private Point m_pForeignMaterialTop, m_pForeignMaterialRight, m_pForeignMaterialBottom, m_pForeignMaterialLeft;
        private Point m_pForeignMaterialTop_Pad, m_pForeignMaterialRight_Pad, m_pForeignMaterialBottom_Pad, m_pForeignMaterialLeft_Pad;
        private bool m_blnUpdateSelectedROISetting = false;
        private bool m_blnInitDone = false;
        private bool m_blnDragROIPrev = false;
        private bool m_blnIgnoreTappaeIndexChange = false;
        private bool m_blnIgnoreComboBoxIndexChange = false;
        private int m_intUserGroup = 5;
        private int[] m_intThinIterationPrev;
        private int[] m_intThickIterationPrev;
        private string m_strSelectedRecipe;
        private int m_intSelectedTabPage = 0;

        PadInspectionAreaSettingForm m_objPadInspectionAreaSettingForm;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        LineProfileForm m_objLineProfileForm;
        #endregion

        public PadOtherSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intSelectedTabPage)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_intThinIterationPrev = new int[m_smVisionInfo.g_arrPad.Length];
            m_intThickIterationPrev = new int[m_smVisionInfo.g_arrPad.Length];

            for (int i = 0; i < m_intThinIterationPrev.Length; i++)
            {
                m_intThinIterationPrev[i] = m_smVisionInfo.g_arrPad[i].ref_intMPErodeHalfWidth;
                m_intThickIterationPrev[i] = m_smVisionInfo.g_arrPad[i].ref_intMPDilateHalfWidth;
            }

            //if (m_smVisionInfo.g_intSelectedROIMask == 0)
            //{
            //    m_smVisionInfo.g_intSelectedROIMask = 0x01;
            //    m_smVisionInfo.g_intSelectedROI = 0;    // Reset to selecting center ROI when display form.
            //}

            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    if ((m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
            //    {
            //        if (i < m_smVisionInfo.g_arrPadROIs.Count)
            //        {
            //            if (m_smVisionInfo.g_arrPadROIs[i].Count > 0)
            //            {
            //                m_smVisionInfo.g_arrPadROIs[i][0].VerifyROIArea(m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalCenterX, m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalCenterY);
            //            }
            //        }
            //    }
            //}

            m_smVisionInfo.g_intSelectedROIMask = 0x01;
            m_smVisionInfo.g_intSelectedROI = 0;    // Reset to selecting center ROI when display form.
            cbo_SelectROI.SelectedIndex = m_smVisionInfo.g_intSelectedROI = 0;
            if (m_smVisionInfo.g_arrPadROIs.Count > 0)
            {
                if (m_smVisionInfo.g_arrPadROIs[0].Count > 0)
                {
                    m_smVisionInfo.g_arrPadROIs[0][0].VerifyROIArea(m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalCenterX, m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalCenterY);
                }
            }

            if (m_smVisionInfo.g_arrPad.Length == 1 || !m_smVisionInfo.g_blnCheck4Sides)
            {
                cbo_SelectROI.Visible = false;
            }

            m_intSelectedTabPage = intSelectedTabPage;

            DisableField2();
            UpdateGUI();

            m_pPkgTop = new Point(pnl_PkgTop.Location.X, pnl_PkgTop.Location.Y);
            m_pPkgRight = new Point(pnl_PkgRight.Location.X, pnl_PkgRight.Location.Y);
            m_pPkgBottom = new Point(pnl_PkgBottom.Location.X, pnl_PkgBottom.Location.Y);
            m_pPkgLeft = new Point(pnl_PkgLeft.Location.X, pnl_PkgLeft.Location.Y);

            m_pChippedTop = new Point(pnl_ChippedTop.Location.X, pnl_ChippedTop.Location.Y);
            m_pChippedRight = new Point(pnl_ChippedRight.Location.X, pnl_ChippedRight.Location.Y);
            m_pChippedBottom = new Point(pnl_ChippedBottom.Location.X, pnl_ChippedBottom.Location.Y);
            m_pChippedLeft = new Point(pnl_ChippedLeft.Location.X, pnl_ChippedLeft.Location.Y);

            m_pChippedTop_Dark = new Point(pnl_ChippedTop_Dark.Location.X, pnl_ChippedTop_Dark.Location.Y);
            m_pChippedRight_Dark = new Point(pnl_ChippedRight_Dark.Location.X, pnl_ChippedRight_Dark.Location.Y);
            m_pChippedBottom_Dark = new Point(pnl_ChippedBottom_Dark.Location.X, pnl_ChippedBottom_Dark.Location.Y);
            m_pChippedLeft_Dark = new Point(pnl_ChippedLeft_Dark.Location.X, pnl_ChippedLeft_Dark.Location.Y);

            m_pForeignMaterialTop = new Point(pnl_ForeignMaterialTop.Location.X, pnl_ForeignMaterialTop.Location.Y);
            m_pForeignMaterialRight = new Point(pnl_ForeignMaterialRight.Location.X, pnl_ForeignMaterialRight.Location.Y);
            m_pForeignMaterialBottom = new Point(pnl_ForeignMaterialBottom.Location.X, pnl_ForeignMaterialBottom.Location.Y);
            m_pForeignMaterialLeft = new Point(pnl_ForeignMaterialLeft.Location.X, pnl_ForeignMaterialLeft.Location.Y);

            m_pForeignMaterialTop_Pad = new Point(pnl_ForeignMaterialTop_Pad.Location.X, pnl_ForeignMaterialTop_Pad.Location.Y);
            m_pForeignMaterialRight_Pad = new Point(pnl_ForeignMaterialRight_Pad.Location.X, pnl_ForeignMaterialRight_Pad.Location.Y);
            m_pForeignMaterialBottom_Pad = new Point(pnl_ForeignMaterialBottom_Pad.Location.X, pnl_ForeignMaterialBottom_Pad.Location.Y);
            m_pForeignMaterialLeft_Pad = new Point(pnl_ForeignMaterialLeft_Pad.Location.X, pnl_ForeignMaterialLeft_Pad.Location.Y);

            m_pMoldTop = new Point(pnl_MoldTop.Location.X, pnl_MoldTop.Location.Y);
            m_pMoldRight = new Point(pnl_MoldRight.Location.X, pnl_MoldRight.Location.Y);
            m_pMoldBottom = new Point(pnl_MoldBottom.Location.X, pnl_MoldBottom.Location.Y);
            m_pMoldLeft = new Point(pnl_MoldLeft.Location.X, pnl_MoldLeft.Location.Y);

            m_blnInitDone = true;
        }

        private void DisableField()
        {
            string strChild1 = "Setting Page";
            string strChild2 = "Save Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
            }

            switch (m_intSelectedTabPage)
            {
                case 0:
                    tab_VisionControl.TabPages.Remove(tp_PkgSegmentSimple);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                    //if (m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
                    //    tab_VisionControl.TabPages.Remove(tp_ROI);
                    //else
                    //{
                    //    grpbox_ForeignMaterialROI.Location = new Point(gbox_Pkg.Location.X, gbox_Pkg.Location.Y);
                    //    tp_ROI.Controls.Remove(gbox_Pkg);
                    //    tp_ROI.Controls.Remove(gbox_Pkg_Dark);
                    //}
                    //tab_VisionControl.TabPages.Remove(tp_ROI2);
                    if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrPad[0].GetOverallWantGaugeMeasurePkgSize(false) && !m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON)
                        tab_VisionControl.TabPages.Remove(tp_ROI_Pad);

                    tab_VisionControl.TabPages.Remove(tp_ROI);
                    tab_VisionControl.TabPages.Remove(tp_ROI2);
                    break;
                case 1:
                    if (m_smVisionInfo.g_arrPad[0].ref_blnUseDetailDefectCriteria)
                        tab_VisionControl.TabPages.Remove(tp_PkgSegmentSimple);
                    else
                    {
                        pnl_DarkVoid.Visible = false;
                        //tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                        if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateChippedOffDefectSetting)
                        {
                            pnl_BrightChipped.Dock = DockStyle.None;
                            pnl_DarkChipped.Dock = DockStyle.None;
                            tp_PkgSegmentSimple.Controls.Add(pnl_BrightChipped);
                            tp_PkgSegmentSimple.Controls.Add(pnl_DarkChipped);

                            pnl_BrightChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                            pnl_DarkChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_BrightChipped.Location.Y + pnl_BrightChipped.Size.Height);


                            if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
                            {
                                //pnl_BrightMold.Visible = false;
                                pnl_BrightMold.Dock = DockStyle.None;
                                tp_PkgSegmentSimple.Controls.Add(pnl_BrightMold);
                                pnl_BrightMold.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkChipped.Location.Y + pnl_DarkChipped.Size.Height);

                                if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
                                {
                                    //tp_PkgSegment.Controls.Add(pnl_DarkCrack);
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
                                {
                                    pnl_DarkCrack.Dock = DockStyle.None;
                                    tp_PkgSegmentSimple.Controls.Add(pnl_DarkCrack);
                                    pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkChipped.Location.Y + pnl_DarkChipped.Size.Height);
                                }
                            }

                            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting ||
                  !m_smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
                            {
                                tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                                tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                            }
                        }
                        else
                        {
                            pnl_BrightChipped.Visible = false;
                            pnl_DarkChipped.Visible = false;

                            pnl_BrightMold.Dock = DockStyle.None;
                            pnl_DarkCrack.Dock = DockStyle.None;
                            tp_PkgSegmentSimple.Controls.Add(pnl_BrightMold);
                            tp_PkgSegmentSimple.Controls.Add(pnl_DarkCrack);

                            pnl_BrightMold.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                            pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_BrightMold.Location.Y + pnl_BrightMold.Size.Height);


                            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
                            {
                                pnl_BrightMold.Visible = false;
                                pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                            }

                            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
                            {
                                pnl_DarkCrack.Visible = false;
                            }
                            tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                            tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                        }

                        if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateChippedOffDefectSetting)
                        {
                            gbox_Chip.Visible = false;
                            gbox_Mold.Location = new Point(gbox_Chip.Location.X, gbox_Chip.Location.Y);
                        }
                      

                        if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
                        {
                            gbox_Mold.Visible = false;
                        }
                      
                    }
                    tab_VisionControl.TabPages.Remove(tp_Segment);
                    tab_VisionControl.TabPages.Remove(tp_other);
                    tab_VisionControl.TabPages.Remove(tp_ROI_Pad);
                    break;
            }

            if ((m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg") && m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize && !m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON)
            {
                srmLabel1.Visible = false;
                srmLabel2.Visible = false;
                srmLabel3.Visible = false;
                lbl_LowSurfaceThreshold.Visible = false;
                lbl_HighSurfaceThreshold.Visible = false;
                btn_SurfaceThreshold.Visible = false;
                //grpbox_ForeignMaterialROI.Visible = false;

                int intRelocationY = srmLabel4.Location.Y - srmLabel1.Location.Y;

                srmLabel4.Location = new Point(srmLabel4.Location.X, srmLabel4.Location.Y - intRelocationY);
                txt_MinArea.Location = new Point(txt_MinArea.Location.X, txt_MinArea.Location.Y - intRelocationY);
                srmLabel5.Location = new Point(srmLabel5.Location.X, srmLabel5.Location.Y - intRelocationY);

            }

            // 2019 12 28 - CCENG: check are all broken pad inspection using image view 1?
            bool blnAllImage1 = true;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (m_smVisionInfo.g_arrPad[i].ref_intBrokenPadImageViewNo > 0 || m_smVisionInfo.g_arrPad[i].ref_blnWantSeparateBrokenPadThresholdSetting)
                {
                    blnAllImage1 = false;
                    break;
                }
            }

            // 2019 12 28 - CCENG: Display broken pad threshold setting if broken pad inspection using image view other than 1.
            if (!blnAllImage1)
            {
                grp_ImageMerge2.Visible = true;
                //grp_ImageMerge2.Location = new Point(grp_ImageMerge2.Location.X, txt_MinArea.Location.Y + txt_MinArea.Height + 10);
            }
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            switch (m_intSelectedTabPage)
            {
                case 0:
                    tab_VisionControl.TabPages.Remove(tp_PkgSegmentSimple);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                    tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                    //if (m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
                    //    tab_VisionControl.TabPages.Remove(tp_ROI);
                    //else
                    //{
                    //    grpbox_ForeignMaterialROI.Location = new Point(gbox_Pkg.Location.X, gbox_Pkg.Location.Y);
                    //    tp_ROI.Controls.Remove(gbox_Pkg);
                    //    tp_ROI.Controls.Remove(gbox_Pkg_Dark);
                    //}
                    //tab_VisionControl.TabPages.Remove(tp_ROI2);
                    if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && m_smVisionInfo.g_arrPad[0].GetOverallWantGaugeMeasurePkgSize(false) && !m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON)
                        tab_VisionControl.TabPages.Remove(tp_ROI_Pad);

                    tab_VisionControl.TabPages.Remove(tp_ROI);
                    tab_VisionControl.TabPages.Remove(tp_ROI2);

                    // 2021 08 03 - CCENG: default selection is tp_Segment. This cause combo box and check box no display when first go into pad>Setting>Segment tabpage.
                    if (!tp_Segment.Controls.Contains(cbo_SelectROI))
                        tp_Segment.Controls.Add(cbo_SelectROI);

                    if (!tp_Segment.Controls.Contains(chk_SetToAllSideROI))
                        tp_Segment.Controls.Add(chk_SetToAllSideROI);

                    break;
                case 1:
                    if (m_smVisionInfo.g_arrPad[0].ref_blnUseDetailDefectCriteria)
                        tab_VisionControl.TabPages.Remove(tp_PkgSegmentSimple);
                    else
                    {
                        pnl_DarkVoid.Visible = false;
                        //tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                        if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateChippedOffDefectSetting)
                        {
                            pnl_BrightChipped.Dock = DockStyle.None;
                            pnl_DarkChipped.Dock = DockStyle.None;
                            tp_PkgSegmentSimple.Controls.Add(pnl_BrightChipped);
                            tp_PkgSegmentSimple.Controls.Add(pnl_DarkChipped);

                            pnl_BrightChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                            pnl_DarkChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_BrightChipped.Location.Y + pnl_BrightChipped.Size.Height);


                           if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
                            {
                                //pnl_BrightMold.Visible = false;
                                pnl_BrightMold.Dock = DockStyle.None;
                                tp_PkgSegmentSimple.Controls.Add(pnl_BrightMold);
                                pnl_BrightMold.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkChipped.Location.Y + pnl_DarkChipped.Size.Height);

                                if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
                                {
                                    //tp_PkgSegment.Controls.Add(pnl_DarkCrack);
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
                                {
                                    pnl_DarkCrack.Dock = DockStyle.None;
                                    tp_PkgSegmentSimple.Controls.Add(pnl_DarkCrack);
                                    pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkChipped.Location.Y + pnl_DarkChipped.Size.Height);
                                }
                            }

                            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
                            {
                                pnl_BrightMold.Visible = false;

                            }

                            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
                            {
                                pnl_DarkCrack.Visible = false;
                            }

                            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateForeignMaterialDefectSetting)
                            {
                                pnl_ForeignMaterialDefect.Visible = false;
                                tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                            }


                            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting ||
                  !m_smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
                            {
                                tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                            }
                        }
                        else
                        {
                            pnl_BrightChipped.Visible = false;
                            pnl_DarkChipped.Visible = false;

                            pnl_BrightMold.Dock = DockStyle.None;
                            pnl_DarkCrack.Dock = DockStyle.None;
                            pnl_ForeignMaterialDefect.Dock = DockStyle.None;
                            tp_PkgSegmentSimple.Controls.Add(pnl_BrightMold);
                            tp_PkgSegmentSimple.Controls.Add(pnl_DarkCrack);
                            tp_PkgSegmentSimple.Controls.Add(pnl_ForeignMaterialDefect);


                            pnl_BrightMold.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                            pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_BrightMold.Location.Y + pnl_BrightMold.Size.Height);
                            pnl_ForeignMaterialDefect.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkCrack.Location.Y + pnl_DarkCrack.Size.Height);


                            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
                            {
                                pnl_BrightMold.Visible = false;
                                pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                                pnl_ForeignMaterialDefect.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkCrack.Location.Y + pnl_DarkCrack.Size.Height);
                            }

                            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
                            {
                                pnl_DarkCrack.Visible = false;
                                pnl_ForeignMaterialDefect.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkCrack.Location.Y);
                            }

                            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateForeignMaterialDefectSetting)
                            {
                                pnl_ForeignMaterialDefect.Visible = false;
                            }

                            tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                            tab_VisionControl.TabPages.Remove(tp_PkgSegment2);
                        }

                        if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateChippedOffDefectSetting)
                        {
                            gbox_Chip.Visible = false;
                            gbox_Mold.Location = new Point(gbox_Chip.Location.X, gbox_Chip.Location.Y);
                            //tab_VisionControl.TabPages.Remove(tp_ROI2);
                        }


                        if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
                        {
                            gbox_Mold.Visible = false;
                            //tab_VisionControl.TabPages.Remove(tp_ROI2);
                        }

                        if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateChippedOffDefectSetting && !m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
                        {
                            tab_VisionControl.TabPages.Remove(tp_ROI2);
                        }

                     
                        if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateForeignMaterialDefectSetting)
                        {
                            grpbox_ForeignMaterialROI.Visible = false;
                        }
                        else
                        {
                            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateBrightDarkROITolerance)
                            {
                                grpbox_ForeignMaterialROI.Location = new Point(gbox_Pkg_Dark.Location.X, gbox_Pkg_Dark.Location.Y);
                            }
                        }

                    }
                    tab_VisionControl.TabPages.Remove(tp_Segment);
                    tab_VisionControl.TabPages.Remove(tp_other);
                    tab_VisionControl.TabPages.Remove(tp_ROI_Pad);
                    break;
            }

            if ((m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg") && m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize && !m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON)
            {
                //srmLabel1.Visible = false;
                //srmLabel2.Visible = false;
                //srmLabel3.Visible = false;
                //lbl_LowSurfaceThreshold.Visible = false;
                //lbl_HighSurfaceThreshold.Visible = false;
                //btn_SurfaceThreshold.Visible = false;
                pnl_SurfaceThreshold.Visible = false;
                //grpbox_ForeignMaterialROI.Visible = false;

                int intRelocationY = srmLabel4.Location.Y - srmLabel1.Location.Y;

                srmLabel4.Location = new Point(srmLabel4.Location.X, srmLabel4.Location.Y - intRelocationY);
                txt_MinArea.Location = new Point(txt_MinArea.Location.X, txt_MinArea.Location.Y - intRelocationY);
                srmLabel5.Location = new Point(srmLabel5.Location.X, srmLabel5.Location.Y - intRelocationY);

            }

            // 2019 12 28 - CCENG: check are all broken pad inspection using image view 1?
            bool blnAllImage1 = true;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (m_smVisionInfo.g_arrPad[i].ref_intBrokenPadImageViewNo > 0 || m_smVisionInfo.g_arrPad[i].ref_blnWantSeparateBrokenPadThresholdSetting)
                {
                    blnAllImage1 = false;
                    break;
                }
            }

            // 2019 12 28 - CCENG: Display broken pad threshold setting if broken pad inspection using image view other than 1.
            if (!blnAllImage1 || m_smVisionInfo.g_arrPad[0].ref_blnWantSeparateBrokenPadThresholdSetting)
            {
                grp_ImageMerge2.Visible = true;
                //grp_ImageMerge2.Location = new Point(txt_MinArea.Location.X, txt_MinArea.Location.Y + txt_MinArea.Height + 10);
            }

            string strChild2 = "Setting Page";
            string strChild3 = "";

            strChild3 = "Pad Threshold button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_PadThreshold.Enabled = false; pnl_PadThreshold.Visible = btn_PadThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_SurfaceThreshold.Enabled = false; pnl_SurfaceThreshold.Visible = btn_SurfaceThreshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                btn_PadImageMerge2Threshold.Enabled = false; pnl_PadImageMerge2Threshold.Visible = btn_PadImageMerge2Threshold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Min Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_MinArea.Enabled = false; pnl_MindArea.Visible = txt_MinArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                txt_PadImageMerge2MinArea.Enabled = false; pnl_MinArea_PadImageMerge2Threshold.Visible = txt_PadImageMerge2MinArea.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Pad Thin Iteration";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_ThinIteration.Enabled = false; srmLabel54.Visible = srmLabel52.Visible = txt_ThinIteration.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Pad Thick Iteration";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_ThickIteration.Enabled = false; lbl_ThickIteration.Visible = srmLabel53.Visible = txt_ThickIteration.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;

                if (!txt_ThinIteration.Enabled)
                {
                    srmGroupBox16.Visible = false;
                }
            }

            strChild3 = "Pad Measure Edge Tool Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_LineProfileGaugeSetting.Enabled = false; btn_LineProfileGaugeSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            //strChild3 = "Pad ROI Tolerance Button";
            //if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            //{
            //    btn_PadROIToleranceSetting.Enabled = false; btn_PadROIToleranceSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            //}

            strChild3 = "Pad Individual Inspection Area Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_PadInspectionAreaSetting.Enabled = false; btn_PadInspectionAreaSetting.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Bright Field Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                pnl_BrightDefect.Enabled = false; pnl_BrightDefect.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Dark Field Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                pnl_DarkDefect.Enabled = false; pnl_DarkDefect.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Chipped Off Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                pnl_BrightChipped.Enabled = false; pnl_BrightChipped.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                pnl_DarkChipped.Enabled = false; pnl_DarkChipped.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Crack Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                pnl_DarkCrack.Enabled = false; pnl_DarkCrack.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Mold Flash Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                pnl_BrightMold.Enabled = false; pnl_BrightMold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Foreign Material Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                pnl_ForeignMaterialDefect.Enabled = false; pnl_ForeignMaterialDefect.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }

            strChild3 = "Package ROI Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                gbox_Pkg.Enabled = false; gbox_Pkg.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gbox_Pkg_Dark.Enabled = false; gbox_Pkg_Dark.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                grpbox_ForeignMaterialROI.Enabled = false; grpbox_ForeignMaterialROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                grpbox_ForeignMaterialROI_Pad.Enabled = false; grpbox_ForeignMaterialROI_Pad.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gbox_Chip.Enabled = false; gbox_Chip.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gbox_Chip_Dark.Enabled = false; gbox_Chip_Dark.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                gbox_Mold.Enabled = false; gbox_Mold.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_SetToAllSideROI.Enabled = false; chk_SetToAllSideROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                chk_SetToAll.Enabled = false; chk_SetToAll.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                cbo_SelectROI.Enabled = false; cbo_SelectROI.Visible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
                m_blnPackageROISettingVisible = m_smProductionInfo.g_blnShowGUIForBelowUserRight;
            }
        }

        private int GetUserRightGroup_Child3(string Child2, string Child3)
        {
            //NewUserRight objUserRight = new NewUserRight(false);
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Orient":
                case "BottomOrient":
                    return m_smCustomizeInfo.objNewUserRight.GetOrientationChild3Group(Child2, Child3);
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "Package":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetMarkOrientChild3Group(Child2, Child3);
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetInPocketChild3Group(Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
                //case "BottomOrientPad":
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetPadChild3Group(Child2, Child3);
                    break;
                case "Li3D":
                case "Li3DPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetLead3DChild3Group(Child2, Child3);
                    break;
                case "Seal":
                    return m_smCustomizeInfo.objNewUserRight.GetSealChild3Group(Child2, Child3, m_smVisionInfo.g_intVisionNameNo);
                    break;
                case "Barcode":
                    return m_smCustomizeInfo.objNewUserRight.GetBarcodeChild3Group(Child2, Child3);
                    break;
            }

            return 1;
        }
        private void UpdateGUI()
        {
            m_blnUpdateSelectedROISetting = true;

            switch (m_intSelectedTabPage)
            {
                case 0:
                    txt_ThinIteration.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intMPErodeHalfWidth.ToString();
                    txt_ThickIteration.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intMPDilateHalfWidth.ToString();
                    lbl_PadThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intThresholdValue.ToString();
                    lbl_PadImageMerge2Threshold_Low.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intImageMerge2ThresholdLowValue.ToString();
                    lbl_PadImageMerge2Threshold_High.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intImageMerge2ThresholdHighValue.ToString();
                    lbl_LowSurfaceThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intSurfaceLowThresholdValue.ToString();
                    lbl_HighSurfaceThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intSurfaceThresholdValue.ToString();

                    txt_MinArea.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fBlobsMinArea.ToString();
                    txt_PadImageMerge2MinArea.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fImageMerge2BlobsMinArea.ToString();

                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 0:
                        case 1:
                            txt_ForeignMaterialStartPixelFromEdge_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge_Pad.ToString();
                            txt_ForeignMaterialStartPixelFromRight_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromRight_Pad.ToString();
                            txt_ForeignMaterialStartPixelFromBottom_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromBottom_Pad.ToString();
                            txt_ForeignMaterialStartPixelFromLeft_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromLeft_Pad.ToString();
                            break;
                        case 2:
                            txt_ForeignMaterialStartPixelFromEdge_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromRight_Pad.ToString();
                            txt_ForeignMaterialStartPixelFromRight_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromBottom_Pad.ToString();
                            txt_ForeignMaterialStartPixelFromBottom_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromLeft_Pad.ToString();
                            txt_ForeignMaterialStartPixelFromLeft_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge_Pad.ToString();
                            break;
                        case 3:
                            txt_ForeignMaterialStartPixelFromEdge_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromBottom_Pad.ToString();
                            txt_ForeignMaterialStartPixelFromRight_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromLeft_Pad.ToString();
                            txt_ForeignMaterialStartPixelFromBottom_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge_Pad.ToString();
                            txt_ForeignMaterialStartPixelFromLeft_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromRight_Pad.ToString();
                            break;
                        case 4:
                            txt_ForeignMaterialStartPixelFromEdge_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromLeft_Pad.ToString();
                            txt_ForeignMaterialStartPixelFromRight_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge_Pad.ToString();
                            txt_ForeignMaterialStartPixelFromBottom_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromRight_Pad.ToString();
                            txt_ForeignMaterialStartPixelFromLeft_Pad.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromBottom_Pad.ToString();
                            break;
                    }

                    RegistryKey key1 = Registry.LocalMachine.OpenSubKey("Software", true);
                    RegistryKey subkey11 = key1.CreateSubKey("SVG\\AutoMode");
                    chk_SetToAll.Checked = Convert.ToBoolean(subkey11.GetValue("SetToAllEdges_ROI_Pad", false));
                    if ((m_smVisionInfo.g_arrPad.Length > 1) && m_smVisionInfo.g_blnCheck4Sides)
                        chk_SetToAllSideROI.Checked = Convert.ToBoolean(subkey11.GetValue("SetToAllSide_ROI_Pad", false));
                    else
                        chk_SetToAllSideROI.Checked = false;

                    if ((m_smVisionInfo.g_arrPad.Length > 1) && m_smVisionInfo.g_blnCheck4Sides)
                        chk_SetToAllSideROI.Visible = m_blnPackageROISettingVisible;
                    else
                        chk_SetToAllSideROI.Visible = false;

                    if (m_smVisionInfo.g_blnViewColorImage)
                        m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage].ConvertColorToMono(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && (!m_smVisionInfo.g_arrPad[0].GetOverallWantGaugeMeasurePkgSize(false) || m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON))
                        CheckForeignMaterialROISetting_Pad();
                    break;
                case 1:
                    //pad package

                    if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateBrightDarkROITolerance)
                    {
                        gbox_Pkg_Dark.Visible = false;
                    }

                    lbl_BrightThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intBrightFieldLowThreshold.ToString();
                    lbl_DarkThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intDarkFieldLowThreshold.ToString();
                    lbl_PkgImage1LowThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage1LowSurfaceThreshold.ToString();
                    lbl_PkgImage1HighThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage1HighSurfaceThreshold.ToString();
                    lbl_PkgImage2LowThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2LowThreshold.ToString();
                    lbl_PkgImage2HighThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2HighThreshold.ToString();
                    lbl_MoldFlashThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage1MoldFlashThreshold.ToString();
                    lbl_ForeignMaterialThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intForeignMaterialBrightFieldThreshold.ToString();

                    txt_BrightMinArea.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intBrightFieldMinArea.ToString();
                    txt_DarkMinArea.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intDarkFieldMinArea.ToString();
                    txt_SurfaceMinArea.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fSurfaceMinArea.ToString();
                    txt_Image2SurfaceMinArea.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fImage2SurfaceMinArea.ToString();
                    txt_MoldFlashMinArea.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldFlashMinArea.ToString();
                    txt_VoidMinArea.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fVoidMinArea.ToString();
                    lbl_Void_Threshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2VoidThreshold.ToString();
                    lbl_LowCrackViewThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2LowCrackThreshold.ToString();
                    lbl_HighCrackViewThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2HighCrackThreshold.ToString();
                    txt_CrackMinArea.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fCrackMinArea.ToString();
                    txt_ForeignMaterialMinArea.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intForeignMaterialBrightFieldMinArea.ToString();

                    switch (m_smVisionInfo.g_intSelectedROI)
                    {
                        case 0:
                        case 1:
                            txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge.ToString();
                            txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight.ToString();
                            txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom.ToString();
                            txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft.ToString();

                            txt_PkgStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge_Dark.ToString();
                            txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight_Dark.ToString();
                            txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom_Dark.ToString();
                            txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft_Dark.ToString();

                            txt_ChipStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge.ToString();
                            txt_ChipStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight.ToString();
                            txt_ChipStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom.ToString();
                            txt_ChipStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft.ToString();

                            txt_ChipStartPixelExtendFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromEdge.ToString();
                            txt_ChipStartPixelExtendFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromRight.ToString();
                            txt_ChipStartPixelExtendFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromBottom.ToString();
                            txt_ChipStartPixelExtendFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromLeft.ToString();

                            txt_ChipStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge_Dark.ToString();
                            txt_ChipStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight_Dark.ToString();
                            txt_ChipStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom_Dark.ToString();
                            txt_ChipStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft_Dark.ToString();

                            txt_ChipStartPixelExtendFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromEdge_Dark.ToString();
                            txt_ChipStartPixelExtendFromRight_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromRight_Dark.ToString();
                            txt_ChipStartPixelExtendFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromBottom_Dark.ToString();
                            txt_ChipStartPixelExtendFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromLeft_Dark.ToString();

                            txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromEdge.ToString();
                            txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromRight.ToString();
                            txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromBottom.ToString();
                            txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromLeft.ToString();

                            txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromEdge.ToString();
                            txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromRight.ToString();
                            txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromBottom.ToString();
                            txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromLeft.ToString();

                            txt_ForeignMaterialStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge.ToString();
                            txt_ForeignMaterialStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromRight.ToString();
                            txt_ForeignMaterialStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromBottom.ToString();
                            txt_ForeignMaterialStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromLeft.ToString();

                            break;
                        case 2:
                            txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight.ToString();
                            txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom.ToString();
                            txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft.ToString();
                            txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge.ToString();

                            txt_PkgStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight_Dark.ToString();
                            txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom_Dark.ToString();
                            txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft_Dark.ToString();
                            txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge_Dark.ToString();

                            txt_ChipStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight.ToString();
                            txt_ChipStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom.ToString();
                            txt_ChipStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft.ToString();
                            txt_ChipStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge.ToString();

                            txt_ChipStartPixelExtendFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromRight.ToString();
                            txt_ChipStartPixelExtendFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromBottom.ToString();
                            txt_ChipStartPixelExtendFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromLeft.ToString();
                            txt_ChipStartPixelExtendFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromEdge.ToString();

                            txt_ChipStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight_Dark.ToString();
                            txt_ChipStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom_Dark.ToString();
                            txt_ChipStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft_Dark.ToString();
                            txt_ChipStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge_Dark.ToString();

                            txt_ChipStartPixelExtendFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromRight_Dark.ToString();
                            txt_ChipStartPixelExtendFromRight_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromBottom_Dark.ToString();
                            txt_ChipStartPixelExtendFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromLeft_Dark.ToString();
                            txt_ChipStartPixelExtendFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromEdge_Dark.ToString();

                            txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromRight.ToString();
                            txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromBottom.ToString();
                            txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromLeft.ToString();
                            txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromEdge.ToString();

                            txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromRight.ToString();
                            txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromBottom.ToString();
                            txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromLeft.ToString();
                            txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromEdge.ToString();

                            txt_ForeignMaterialStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromRight.ToString();
                            txt_ForeignMaterialStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromBottom.ToString();
                            txt_ForeignMaterialStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromLeft.ToString();
                            txt_ForeignMaterialStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge.ToString();

                            break;
                        case 3:
                            txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom.ToString();
                            txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft.ToString();
                            txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge.ToString();
                            txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight.ToString();

                            txt_PkgStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom_Dark.ToString();
                            txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft_Dark.ToString();
                            txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge_Dark.ToString();
                            txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight_Dark.ToString();

                            txt_ChipStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom.ToString();
                            txt_ChipStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft.ToString();
                            txt_ChipStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge.ToString();
                            txt_ChipStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight.ToString();

                            txt_ChipStartPixelExtendFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromBottom.ToString();
                            txt_ChipStartPixelExtendFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromLeft.ToString();
                            txt_ChipStartPixelExtendFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromEdge.ToString();
                            txt_ChipStartPixelExtendFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromRight.ToString();

                            txt_ChipStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom_Dark.ToString();
                            txt_ChipStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft_Dark.ToString();
                            txt_ChipStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge_Dark.ToString();
                            txt_ChipStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight_Dark.ToString();

                            txt_ChipStartPixelExtendFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromBottom_Dark.ToString();
                            txt_ChipStartPixelExtendFromRight_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromLeft_Dark.ToString();
                            txt_ChipStartPixelExtendFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromEdge_Dark.ToString();
                            txt_ChipStartPixelExtendFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromRight_Dark.ToString();

                            txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromBottom.ToString();
                            txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromLeft.ToString();
                            txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromEdge.ToString();
                            txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromRight.ToString();

                            txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromBottom.ToString();
                            txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromLeft.ToString();
                            txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromEdge.ToString();
                            txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromRight.ToString();

                            txt_ForeignMaterialStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromBottom.ToString();
                            txt_ForeignMaterialStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromLeft.ToString();
                            txt_ForeignMaterialStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge.ToString();
                            txt_ForeignMaterialStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromRight.ToString();

                            break;
                        case 4:
                            txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft.ToString();
                            txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge.ToString();
                            txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight.ToString();
                            txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom.ToString();

                            txt_PkgStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft_Dark.ToString();
                            txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge_Dark.ToString();
                            txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight_Dark.ToString();
                            txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom_Dark.ToString();

                            txt_ChipStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft.ToString();
                            txt_ChipStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge.ToString();
                            txt_ChipStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight.ToString();
                            txt_ChipStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom.ToString();

                            txt_ChipStartPixelExtendFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromLeft.ToString();
                            txt_ChipStartPixelExtendFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromEdge.ToString();
                            txt_ChipStartPixelExtendFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromRight.ToString();
                            txt_ChipStartPixelExtendFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromBottom.ToString();

                            txt_ChipStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft_Dark.ToString();
                            txt_ChipStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge_Dark.ToString();
                            txt_ChipStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight_Dark.ToString();
                            txt_ChipStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom_Dark.ToString();

                            txt_ChipStartPixelExtendFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromLeft_Dark.ToString();
                            txt_ChipStartPixelExtendFromRight_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromEdge_Dark.ToString();
                            txt_ChipStartPixelExtendFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromRight_Dark.ToString();
                            txt_ChipStartPixelExtendFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelExtendFromBottom_Dark.ToString();

                            txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromLeft.ToString();
                            txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromEdge.ToString();
                            txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromRight.ToString();
                            txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromBottom.ToString();

                            txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromLeft.ToString();
                            txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromEdge.ToString();
                            txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromRight.ToString();
                            txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromBottom.ToString();

                            txt_ForeignMaterialStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromLeft.ToString();
                            txt_ForeignMaterialStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge.ToString();
                            txt_ForeignMaterialStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromRight.ToString();
                            txt_ForeignMaterialStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromBottom.ToString();

                            break;
                    }

                    RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                    RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                    chk_SetToAll.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_ROI_Pad", false));
                    if ((m_smVisionInfo.g_arrPad.Length > 1) && m_smVisionInfo.g_blnCheck4Sides)
                        chk_SetToAllSideROI.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllSide_ROI_Pad", false));
                    else
                        chk_SetToAllSideROI.Checked = false;
                    if ((m_smVisionInfo.g_arrPad.Length > 1) && m_smVisionInfo.g_blnCheck4Sides)
                        chk_SetToAllSideROI.Visible = m_blnPackageROISettingVisible;
                    else
                        chk_SetToAllSideROI.Visible = false;

                    UpdateMoldFlashForeColor();
                    CheckPkgROISetting();
                    CheckPkgROISetting_Dark();
                    CheckChippedOffROISetting();
                    CheckMoldFlashROISetting();
                    CheckForeignMaterialROISetting();

                    break;
            }
            //if ((m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg") && m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize && !m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON)
            //    CheckForeignMaterialROISetting();
            //else
            //{
            //    UpdateMoldFlashForeColor();
            //    CheckPkgROISetting();
            //    CheckPkgROISetting_Dark();
            //    CheckChippedOffROISetting();
            //    CheckMoldFlashROISetting();
            //}
            m_blnUpdateSelectedROISetting = false;
        }

        /// <summary>
        /// Load pad settings from selected path
        /// </summary>
        /// <param name="strPath">selected path</param>
        private void LoadPadSetting(string strPath)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == 0)
                {
                    m_smVisionInfo.g_arrPad[i].SetCalibrationData(
                                                  m_smVisionInfo.g_fCalibPixelX,
                                                  m_smVisionInfo.g_fCalibPixelY,
                                                  m_smVisionInfo.g_fCalibOffSetX,
                                                  m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
                }
                else
                {
                    m_smVisionInfo.g_arrPad[i].SetCalibrationData(
                                                  m_smVisionInfo.g_fCalibPixelZ,
                                                  m_smVisionInfo.g_fCalibPixelZ,
                                                  m_smVisionInfo.g_fCalibOffSetZ,
                                                  m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
                }

                XmlParser objFile;
                // Load Pad Advance Setting
                objFile = new XmlParser(strPath + "Settings.xml");
                objFile.GetFirstSection("Advanced");
                m_smVisionInfo.g_arrPad[i].ref_blnWhiteOnBlack = objFile.GetValueAsBoolean("WhiteOnBlack", true, 1);

                // Load Pad Template Setting
                string strSectionName = "";
                if (i == 0)
                    strSectionName = "CenterROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                m_smVisionInfo.g_arrPad[i].LoadPad(strPath + "Template\\Template.xml", strSectionName);

            }
        }


        /// <summary>
        /// Save pad settings to xml
        /// </summary>
        /// <param name="strFolderPath">xml folder path</param>
        /// <param name="blnNewRecipe"></param>
        private void SavePadSetting(string strFolderPath)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                string strSectionName = "";
                if (i == 0)
                    strSectionName = "CenterROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                
                STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
                m_smVisionInfo.g_arrPad[i].SavePad(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Pad "+ strSectionName + " Other Setting", m_smProductionInfo.g_strLotID);
                
                //objFile.WriteElement1Value("OrientSetting", true);
                //objFile.WriteElement2Value("MatchMinScore", m_smVisionInfo.g_arrPadOrient[i].ref_fMinScore); 
            }

            //if (m_smVisionInfo.g_arrPin1 != null)
            //{
            //    
            //    STDeviceEdit.CopySettingFile(strFolderPath, "Pin1Template.xml");
            //    m_smVisionInfo.g_arrPin1[0].SaveTemplate(strFolderPath + "\\Template\\");
            //    STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Other Settings", strFolderPath, "Pin1Template.xml");
            //}
        }
        private void UpdateMoldFlashForeColor()
        {
            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromEdge < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromEdge)
            {
                switch (m_smVisionInfo.g_intSelectedROI)
                {
                    case 0:
                    case 1:
                        txt_MoldStartPixelFromEdge.ForeColor = Color.Red;
                        txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Red;
                        break;
                    case 2:
                        txt_MoldStartPixelFromLeft.ForeColor = Color.Red;
                        txt_MoldStartPixelFromLeftInner.ForeColor = Color.Red;
                        break;
                    case 3:
                        txt_MoldStartPixelFromBottom.ForeColor = Color.Red;
                        txt_MoldStartPixelFromBottomInner.ForeColor = Color.Red;
                        break;
                    case 4:
                        txt_MoldStartPixelFromRight.ForeColor = Color.Red;
                        txt_MoldStartPixelFromRightInner.ForeColor = Color.Red;
                        break;
                }
            }
            else
            {
                switch (m_smVisionInfo.g_intSelectedROI)
                {
                    case 0:
                    case 1:
                        txt_MoldStartPixelFromEdge.ForeColor = Color.Black;
                        txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Black;
                        break;
                    case 2:
                        txt_MoldStartPixelFromLeft.ForeColor = Color.Black;
                        txt_MoldStartPixelFromLeftInner.ForeColor = Color.Black;
                        break;
                    case 3:
                        txt_MoldStartPixelFromBottom.ForeColor = Color.Black;
                        txt_MoldStartPixelFromBottomInner.ForeColor = Color.Black;
                        break;
                    case 4:
                        txt_MoldStartPixelFromRight.ForeColor = Color.Black;
                        txt_MoldStartPixelFromRightInner.ForeColor = Color.Black;
                        break;
                }
            }

            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromRight < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromRight)
            {
                switch (m_smVisionInfo.g_intSelectedROI)
                {
                    case 0:
                    case 1:
                        txt_MoldStartPixelFromRight.ForeColor = Color.Red;
                        txt_MoldStartPixelFromRightInner.ForeColor = Color.Red;
                        break;
                    case 2:
                        txt_MoldStartPixelFromEdge.ForeColor = Color.Red;
                        txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Red;
                        break;
                    case 3:
                        txt_MoldStartPixelFromLeft.ForeColor = Color.Red;
                        txt_MoldStartPixelFromLeftInner.ForeColor = Color.Red;
                        break;
                    case 4:
                        txt_MoldStartPixelFromBottom.ForeColor = Color.Red;
                        txt_MoldStartPixelFromBottomInner.ForeColor = Color.Red;
                        break;
                }
            }
            else
            {
                switch (m_smVisionInfo.g_intSelectedROI)
                {
                    case 0:
                    case 1:
                        txt_MoldStartPixelFromRight.ForeColor = Color.Black;
                        txt_MoldStartPixelFromRightInner.ForeColor = Color.Black;
                        break;
                    case 2:
                        txt_MoldStartPixelFromEdge.ForeColor = Color.Black;
                        txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Black;
                        break;
                    case 3:
                        txt_MoldStartPixelFromLeft.ForeColor = Color.Black;
                        txt_MoldStartPixelFromLeftInner.ForeColor = Color.Black;
                        break;
                    case 4:
                        txt_MoldStartPixelFromBottom.ForeColor = Color.Black;
                        txt_MoldStartPixelFromBottomInner.ForeColor = Color.Black;
                        break;
                }
            }

            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromBottom < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromBottom)
            {
                switch (m_smVisionInfo.g_intSelectedROI)
                {
                    case 0:
                    case 1:
                        txt_MoldStartPixelFromBottom.ForeColor = Color.Red;
                        txt_MoldStartPixelFromBottomInner.ForeColor = Color.Red;
                        break;
                    case 2:
                        txt_MoldStartPixelFromRight.ForeColor = Color.Red;
                        txt_MoldStartPixelFromRightInner.ForeColor = Color.Red;
                        break;
                    case 3:
                        txt_MoldStartPixelFromEdge.ForeColor = Color.Red;
                        txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Red;
                        break;
                    case 4:
                        txt_MoldStartPixelFromLeft.ForeColor = Color.Red;
                        txt_MoldStartPixelFromLeftInner.ForeColor = Color.Red;
                        break;
                }
            }
            else
            {
                switch (m_smVisionInfo.g_intSelectedROI)
                {
                    case 0:
                    case 1:
                        txt_MoldStartPixelFromBottom.ForeColor = Color.Black;
                        txt_MoldStartPixelFromBottomInner.ForeColor = Color.Black;
                        break;
                    case 2:
                        txt_MoldStartPixelFromRight.ForeColor = Color.Black;
                        txt_MoldStartPixelFromRightInner.ForeColor = Color.Black;
                        break;
                    case 3:
                        txt_MoldStartPixelFromEdge.ForeColor = Color.Black;
                        txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Black;
                        break;
                    case 4:
                        txt_MoldStartPixelFromLeft.ForeColor = Color.Black;
                        txt_MoldStartPixelFromLeftInner.ForeColor = Color.Black;
                        break;
                }
            }

            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromLeft < m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromLeft)
            {
                switch (m_smVisionInfo.g_intSelectedROI)
                {
                    case 0:
                    case 1:
                        txt_MoldStartPixelFromLeft.ForeColor = Color.Red;
                        txt_MoldStartPixelFromLeftInner.ForeColor = Color.Red;
                        break;
                    case 2:
                        txt_MoldStartPixelFromBottom.ForeColor = Color.Red;
                        txt_MoldStartPixelFromBottomInner.ForeColor = Color.Red;
                        break;
                    case 3:
                        txt_MoldStartPixelFromRight.ForeColor = Color.Red;
                        txt_MoldStartPixelFromRightInner.ForeColor = Color.Red;
                        break;
                    case 4:
                        txt_MoldStartPixelFromEdge.ForeColor = Color.Red;
                        txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Red;
                        break;
                }
            }
            else
            {
                switch (m_smVisionInfo.g_intSelectedROI)
                {
                    case 0:
                    case 1:
                        txt_MoldStartPixelFromLeft.ForeColor = Color.Black;
                        txt_MoldStartPixelFromLeftInner.ForeColor = Color.Black;
                        break;
                    case 2:
                        txt_MoldStartPixelFromBottom.ForeColor = Color.Black;
                        txt_MoldStartPixelFromBottomInner.ForeColor = Color.Black;
                        break;
                    case 3:
                        txt_MoldStartPixelFromRight.ForeColor = Color.Black;
                        txt_MoldStartPixelFromRightInner.ForeColor = Color.Black;
                        break;
                    case 4:
                        txt_MoldStartPixelFromEdge.ForeColor = Color.Black;
                        txt_MoldStartPixelFromEdgeInner.ForeColor = Color.Black;
                        break;
                }
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            if (!IsChipInwardOutwardSettingCorrect(true))
            {
                return;
            }

            if (m_intSelectedTabPage == 1 && m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                  
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    if ((m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge < m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge) ||
                        (m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight < m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight) ||
                        (m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom < m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom) ||
                        (m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft < m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft))
                    {
                        SRMMessageBox.Show("Mold Flash Outer tolerance cannot less than Inner tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
            }

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\";

            SavePadSetting(strPath + "Pad\\");

            // Check does user change the Thin or Thick Iteration value? if yes, then need to reload pad erode and dilate template image.
            bool blnThinThickValueChanged = false;
            for (int i = 0; i < m_intThinIterationPrev.Length; i++)
            {
                if (m_intThinIterationPrev[i] != m_smVisionInfo.g_arrPad[i].ref_intMPErodeHalfWidth)
                {
                    blnThinThickValueChanged = true;
                    break;
                }

                if (m_intThickIterationPrev[i] != m_smVisionInfo.g_arrPad[i].ref_intMPDilateHalfWidth)
                {
                    blnThinThickValueChanged = true;
                    break;
                }
            }

            if (blnThinThickValueChanged)
            {
                string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    m_smVisionInfo.g_arrPad[i].LoadPadTemplateImage(strPath + "Pad\\" + "Template\\", i);
                    if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        m_smVisionInfo.g_arrPad[i].LoadPadPackageTemplateImage(strPath + "Package\\" + "Template\\", i);
                }
            }


            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadPadSetting(strFolderPath + "Pad\\");

            this.Close();
            this.Dispose();
        }

        private void PadOtherSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Other Setting Form Closed", "Exit Pad Setting Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_blnViewROI = false;
            m_smVisionInfo.g_blnViewPadSettingDrawing = false;
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_smVisionInfo.g_blncboImageView = true;
        }

        private void PadOtherSettingForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewROI = true;
            m_smVisionInfo.g_blnViewPadSettingDrawing = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            m_smVisionInfo.g_blncboImageView = false;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void btn_PadThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            int intSelectedROI;
            if (m_smVisionInfo.g_blnCheck4Sides)
            {
                intSelectedROI = m_smVisionInfo.g_intSelectedROI;
            }
            else
            {
                intSelectedROI = 0;
            }

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intThresholdValue;
            m_smVisionInfo.g_fThresholdGainValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_fPadImageGain;

            if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count == 0)
                return;

            //if (m_smVisionInfo.g_blnViewRotatedImage)
            //    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrPad[intSelectedROI].ref_intCheckPadDimensionImageIndex]);
            //else
            //    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrPad[intSelectedROI].ref_intCheckPadDimensionImageIndex]);

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(0, m_smVisionInfo.g_intVisionIndex);
            // 2020 03 25 - Update image with sensitivity value before view threshold image.
            if (m_smVisionInfo.g_blnViewRotatedImage)
            {
                m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);

                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_objPackageImage);

                m_smVisionInfo.g_arrPad[intSelectedROI].SensitivityOnPadROI(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage], ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0]);
            }
            else
            {
                m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_objPackageImage);

                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_objPackageImage);

                m_smVisionInfo.g_arrPad[intSelectedROI].SensitivityOnPadROI(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0]);
            }

            m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_objPackageImage);
            m_smVisionInfo.g_blnViewPackageImage = true;

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrPad.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[0].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrPad.Length > 1)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[1].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrPad.Length > 2)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[2].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrPad.Length > 3)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[3].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrPad.Length > 4)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[4].ref_intThresholdValue);

            bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
            ThresholdWithGainForm objThresholdForm = new ThresholdWithGainForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_smVisionInfo.g_arrPad[i].ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;
                        m_smVisionInfo.g_arrPad[i].ref_fPadImageGain = m_smVisionInfo.g_fThresholdGainValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_fPadImageGain = m_smVisionInfo.g_fThresholdGainValue;
                }

                lbl_PadThreshold.Text = m_smVisionInfo.g_intThresholdValue.ToString();
            }
            else
            {
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intThresholdValue;
                m_smVisionInfo.g_fThresholdGainValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_fPadImageGain;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.g_blnViewPackageImage = false;

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_SurfaceThreshold_Click(object sender, EventArgs e)
        {
            bool blnUseDoubleThreshold = true;

            if (blnUseDoubleThreshold)
            {
                // Clear Result drawing
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

                int intSelectedROI;
                if (m_smVisionInfo.g_blnCheck4Sides)
                {
                    intSelectedROI = m_smVisionInfo.g_intSelectedROI;
                }
                else
                {
                    intSelectedROI = 0;
                }

                m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intSurfaceLowThresholdValue;
                m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intSurfaceThresholdValue;

                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[intSelectedROI].ref_intCheckPadDimensionImageIndex, m_smVisionInfo.g_intVisionIndex);
                if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count == 0)
                    return;

                if (m_smVisionInfo.g_blnViewRotatedImage)
                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrPad[intSelectedROI].ref_intCheckPadDimensionImageIndex]);
                else
                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrPad[intSelectedROI].ref_intCheckPadDimensionImageIndex]);

                m_smVisionInfo.g_intThresholdDrawLowValue = 255;
                m_smVisionInfo.g_intThresholdDrawMiddleValue = 0;
                m_smVisionInfo.g_intThresholdDrawHighValue = 255;

                bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
                DoubleThresholdForm objDoubleThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], 0x05, blnWantSetToAllROICheckBox);
                Rectangle resolution = Screen.PrimaryScreen.Bounds;
                objDoubleThresholdForm.Location = new Point(resolution.Width - objDoubleThresholdForm.Size.Width, resolution.Height - objDoubleThresholdForm.Size.Height);
                //objDoubleThresholdForm.Location = new Point(769, 310);
                if (objDoubleThresholdForm.ShowDialog() == DialogResult.OK)
                {
                    if (objDoubleThresholdForm.ref_blnSetToAllROI)
                    {
                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intSurfaceLowThresholdValue = m_smVisionInfo.g_intLowThresholdValue;
                            m_smVisionInfo.g_arrPad[i].ref_intSurfaceThresholdValue = m_smVisionInfo.g_intHighThresholdValue;
                        }
                    }
                    else
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].ref_intSurfaceLowThresholdValue = m_smVisionInfo.g_intLowThresholdValue;
                        m_smVisionInfo.g_arrPad[intSelectedROI].ref_intSurfaceThresholdValue = m_smVisionInfo.g_intHighThresholdValue;
                    }

                    lbl_LowSurfaceThreshold.Text = m_smVisionInfo.g_intLowThresholdValue.ToString();
                    lbl_HighSurfaceThreshold.Text = m_smVisionInfo.g_intHighThresholdValue.ToString();
                }
                else
                {
                    m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intSurfaceLowThresholdValue;
                    m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intSurfaceThresholdValue;
                }
                objDoubleThresholdForm.Dispose();

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
            else
            {
                // Clear Result drawing
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

                int intSelectedROI;
                if (m_smVisionInfo.g_blnCheck4Sides)
                {
                    intSelectedROI = m_smVisionInfo.g_intSelectedROI;
                }
                else
                {
                    intSelectedROI = 0;
                }

                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intSurfaceThresholdValue;

                if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count == 0)
                    return;

                if (m_smVisionInfo.g_blnViewRotatedImage)
                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_arrPad[intSelectedROI].ref_intCheckPadDimensionImageIndex]);
                else
                    m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_arrPad[intSelectedROI].ref_intCheckPadDimensionImageIndex]);

                List<int> arrrThreshold = new List<int>();
                if (m_smVisionInfo.g_arrPad.Length > 0)
                    arrrThreshold.Add(m_smVisionInfo.g_arrPad[0].ref_intSurfaceThresholdValue);
                if (m_smVisionInfo.g_arrPad.Length > 1)
                    arrrThreshold.Add(m_smVisionInfo.g_arrPad[1].ref_intSurfaceThresholdValue);
                if (m_smVisionInfo.g_arrPad.Length > 2)
                    arrrThreshold.Add(m_smVisionInfo.g_arrPad[2].ref_intSurfaceThresholdValue);
                if (m_smVisionInfo.g_arrPad.Length > 3)
                    arrrThreshold.Add(m_smVisionInfo.g_arrPad[3].ref_intSurfaceThresholdValue);
                if (m_smVisionInfo.g_arrPad.Length > 4)
                    arrrThreshold.Add(m_smVisionInfo.g_arrPad[4].ref_intSurfaceThresholdValue);

                bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
                ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold);
                Rectangle resolution = Screen.PrimaryScreen.Bounds;
                objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
                //objThresholdForm.Location = new Point(769, 310);
                if (objThresholdForm.ShowDialog() == DialogResult.OK)
                {
                    if (objThresholdForm.ref_blnSetToAllROI)
                    {
                        // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                        for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrPad.Length; i++)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_intSurfaceThresholdValue = m_smVisionInfo.g_intThresholdValue;
                        }
                    }
                    else
                        m_smVisionInfo.g_arrPad[intSelectedROI].ref_intSurfaceThresholdValue = m_smVisionInfo.g_intThresholdValue;

                    lbl_LowSurfaceThreshold.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intSurfaceThresholdValue.ToString();
                }
                else
                {
                    m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intSurfaceThresholdValue;
                }
                objThresholdForm.Dispose();

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void txt_MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex || (i > 0 && chk_SetToAllSideROI.Checked))
                {
                    m_smVisionInfo.g_arrPad[i].ref_fBlobsMinArea = Convert.ToSingle(txt_MinArea.Text);
                }
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //    m_smVisionInfo.g_arrPad[i].ref_fBlobsMinArea = Convert.ToSingle(txt_MinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (!m_blnInitDone)
                return;
        }

        private void btn_LineProfileGaugeSetting_Click(object sender, EventArgs e)
        {
            m_blnDragROIPrev = m_smVisionInfo.g_blnDragROI;

            m_smVisionInfo.g_blnDragROI = false;
            string strPath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Pad\\PointGauge.xml";

            if (m_objLineProfileForm == null)
                m_objLineProfileForm = new LineProfileForm(m_smVisionInfo, m_smVisionInfo.g_arrPad[0].ref_objPointGauge, strPath, m_smProductionInfo);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            m_objLineProfileForm.StartPosition = FormStartPosition.Manual;
            m_objLineProfileForm.Location = new Point(resolution.Width - m_objLineProfileForm.Size.Width, resolution.Height - m_objLineProfileForm.Size.Height);
            m_objLineProfileForm.Show();

            m_smVisionInfo.g_strSelectedPage = "LineProfileGaugeSetting";
            this.Hide();
        }

        private void timer_Pad_Tick(object sender, EventArgs e)
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

            if (m_smVisionInfo.g_blnUpdateSelectedROI)
            {
                m_smVisionInfo.g_blnUpdateSelectedROI = false;

                UpdateGUI();
            }

            if (m_objLineProfileForm != null)
            {
                if (!m_objLineProfileForm.ref_blnShow)
                {
                    m_smVisionInfo.g_strSelectedPage = "PadOtherSettingForm";
                    m_objLineProfileForm.Close();
                    m_objLineProfileForm.Dispose();
                    m_objLineProfileForm = null;
                    this.Show();

                    m_smVisionInfo.g_blnDragROI = m_blnDragROIPrev;
                }
            }

            if (m_objPadInspectionAreaSettingForm != null)
            {
                if (!m_objPadInspectionAreaSettingForm.ref_blnShow)
                {
                    m_objPadInspectionAreaSettingForm.Close();
                    m_objPadInspectionAreaSettingForm.Dispose();
                    m_objPadInspectionAreaSettingForm = null;
                    tab_VisionControl.Enabled = true;
                    btn_Save.Enabled = true;
                    btn_Cancel.Enabled = true;
                }
            }
        }

        private void btn_PackageSurfaceThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(0, m_smVisionInfo.g_intVisionIndex);

            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage1LowSurfaceThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage1HighSurfaceThreshold;
            if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count == 0)
                return;
            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            //ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[0]);
            bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], false, blnWantSetToAllROICheckBox);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage1LowSurfaceThreshold = intLowThreshold;
                m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage1HighSurfaceThreshold = intHighThreshold;
            }
            else
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_smVisionInfo.g_arrPad[i].ref_intPkgImage1LowSurfaceThreshold = m_smVisionInfo.g_intLowThresholdValue;
                        m_smVisionInfo.g_arrPad[i].ref_intPkgImage1HighSurfaceThreshold = m_smVisionInfo.g_intHighThresholdValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage1LowSurfaceThreshold = m_smVisionInfo.g_intLowThresholdValue;
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage1HighSurfaceThreshold = m_smVisionInfo.g_intHighThresholdValue;
                }

                lbl_PkgImage1LowThreshold.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage1LowSurfaceThreshold.ToString();
                lbl_PkgImage1HighThreshold.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage1HighSurfaceThreshold.ToString();
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Image2Threshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            if (m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg")
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(1, m_smVisionInfo.g_intVisionIndex);
            else
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(1, m_smVisionInfo.g_intVisionIndex);
            // m_smVisionInfo.g_intSelectedImage = 2;
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2LowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2HighThreshold;

            if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count == 0)
                return;
            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            //ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]); //[2]
            bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], false, blnWantSetToAllROICheckBox);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2LowThreshold = intLowThreshold;
                m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2HighThreshold = intHighThreshold;
            }
            else
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_smVisionInfo.g_arrPad[i].ref_intPkgImage2LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                        m_smVisionInfo.g_arrPad[i].ref_intPkgImage2HighThreshold = m_smVisionInfo.g_intHighThresholdValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2HighThreshold = m_smVisionInfo.g_intHighThresholdValue;
                }

                lbl_PkgImage2LowThreshold.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2LowThreshold.ToString();
                lbl_PkgImage2HighThreshold.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2HighThreshold.ToString();
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_MoldFlashThreshold_Click(object sender, EventArgs e)
        {
            List<int> arrrThreshold = new List<int>();
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            

            //m_smVisionInfo.g_intSelectedImage = 0;
            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadPkgMoldFlashImageViewNo, m_smVisionInfo.g_intVisionIndex);
            MeasureGauge();
            RotateImage();
            if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                RotateColorImage();
                m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage].ConvertColorToMono(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            }
            m_smVisionInfo.g_blnViewRotatedImage = true;

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage1MoldFlashThreshold;

            if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count == 0)
                return;

            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);


            if (m_smVisionInfo.g_arrPad.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[0].ref_intPkgImage1MoldFlashThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 1)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[1].ref_intPkgImage1MoldFlashThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 2)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[2].ref_intPkgImage1MoldFlashThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 3)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[3].ref_intPkgImage1MoldFlashThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 4)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[4].ref_intPkgImage1MoldFlashThreshold);

            bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_smVisionInfo.g_arrPad[i].ref_intPkgImage1MoldFlashThreshold = m_smVisionInfo.g_intThresholdValue;
                    }
                }
                else
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage1MoldFlashThreshold = m_smVisionInfo.g_intThresholdValue;

                lbl_MoldFlashThreshold.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage1MoldFlashThreshold.ToString();
            }
            else
            {
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage1MoldFlashThreshold;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }


        private void txt_SurfaceMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex || (i > 0 && chk_SetToAllSideROI.Checked))
                {
                    m_smVisionInfo.g_arrPad[i].ref_fSurfaceMinArea = Convert.ToSingle(txt_SurfaceMinArea.Text);
                }
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //    m_smVisionInfo.g_arrPad[i].ref_fSurfaceMinArea = Convert.ToSingle(txt_SurfaceMinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Image2SurfaceMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex || (i > 0 && chk_SetToAllSideROI.Checked))
                {
                    m_smVisionInfo.g_arrPad[i].ref_fImage2SurfaceMinArea = Convert.ToSingle(txt_Image2SurfaceMinArea.Text);
                }

                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //    m_smVisionInfo.g_arrPad[i].ref_fImage2SurfaceMinArea = Convert.ToSingle(txt_Image2SurfaceMinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldFlashMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex || (i > 0 && chk_SetToAllSideROI.Checked))
                {
                    m_smVisionInfo.g_arrPad[i].ref_fMoldFlashMinArea = Convert.ToSingle(txt_MoldFlashMinArea.Text);
                }

                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldFlashMinArea = Convert.ToSingle(txt_MoldFlashMinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPointFromEdge_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_PkgStartPointFromEdge.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPointFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_PkgStartPointFromEdge.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPointFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_PkgStartPixelFromEdge.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_PkgStartPixelFromEdge.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_PkgStartPixelFromEdge.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_PkgStartPixelFromEdge.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                if (chk_SetToAll.Checked)
                {
                    txt_PkgStartPixelFromRight.Text = txt_PkgStartPixelFromEdge.Text;
                    txt_PkgStartPixelFromBottom.Text = txt_PkgStartPixelFromEdge.Text;
                    txt_PkgStartPixelFromLeft.Text = txt_PkgStartPixelFromEdge.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToSingle(txt_PkgStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPixelFromEdge.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i==0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToSingle(txt_PkgStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPixelFromEdge.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromEdge.Text);
                            }
                            break;
                    }
                }
            }
            CheckPkgROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_PkgStartPixelFromRight.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_PkgStartPixelFromRight.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}

                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_PkgStartPixelFromRight.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_PkgStartPixelFromRight.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_PkgStartPixelFromRight.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_PkgStartPixelFromRight.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_PkgStartPixelFromEdge.Text = txt_PkgStartPixelFromRight.Text;
                    txt_PkgStartPixelFromBottom.Text = txt_PkgStartPixelFromRight.Text;
                    txt_PkgStartPixelFromLeft.Text = txt_PkgStartPixelFromRight.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToSingle(txt_PkgStartPixelFromRight.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPixelFromRight.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToSingle(txt_PkgStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPixelFromRight.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToSingle(txt_PkgStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPixelFromRight.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromRight.Text);
                            }
                            break;
                    }
                }
            }
            CheckPkgROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_PkgStartPixelFromBottom.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_PkgStartPixelFromBottom.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_PkgStartPixelFromBottom.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_PkgStartPixelFromBottom.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_PkgStartPixelFromBottom.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_PkgStartPixelFromBottom.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_PkgStartPixelFromEdge.Text = txt_PkgStartPixelFromBottom.Text;
                    txt_PkgStartPixelFromRight.Text = txt_PkgStartPixelFromBottom.Text;
                    txt_PkgStartPixelFromLeft.Text = txt_PkgStartPixelFromBottom.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPixelFromBottom.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToSingle(txt_PkgStartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPixelFromBottom.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToSingle(txt_PkgStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPixelFromBottom.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToSingle(txt_PkgStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPixelFromBottom.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromBottom.Text);
                            }
                            break;
                    }
                }
            }
            CheckPkgROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_PkgStartPixelFromLeft.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_PkgStartPixelFromLeft.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_PkgStartPixelFromLeft.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_PkgStartPixelFromLeft.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_PkgStartPixelFromLeft.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_PkgStartPixelFromLeft.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                if (chk_SetToAll.Checked)
                {
                    txt_PkgStartPixelFromEdge.Text = txt_PkgStartPixelFromLeft.Text;
                    txt_PkgStartPixelFromRight.Text = txt_PkgStartPixelFromLeft.Text;
                    txt_PkgStartPixelFromBottom.Text = txt_PkgStartPixelFromLeft.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPixelFromLeft.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToSingle(txt_PkgStartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPixelFromLeft.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToSingle(txt_PkgStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPixelFromLeft.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToSingle(txt_PkgStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPixelFromLeft.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft = Convert.ToInt32(txt_PkgStartPixelFromLeft.Text);
                            }
                            break;
                    }
                }
            }
            CheckPkgROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void MeasureGauge()
        {
            //m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPad[0].ref_fImageGain); // 2019-12-16 ZJYEOH : No need add gain for whole image as all ROIs have separated Gain
            // Set RectGauge4L Placement
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI();

                m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ResetGaugeSettingToUserVariables();

                bool blnGaugeResult;
                // 2019 08 01 - CCENG: Measure on g_objPackageImage but display original image
                //if (m_smVisionInfo.g_blnViewPackageImage)
                //blnGaugeResult = m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objWhiteImage);
                if (i == 0 && m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
                {
                    // 2020-03-24 ZJYEOH : no need measure center ROI gauge if use side pkg to measure center pkg
                    blnGaugeResult = true;
                }
                else
                    blnGaugeResult = m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages, m_smVisionInfo.g_objWhiteImage, true); // 2019-12-16 ZJYEOH : Need use this new measure gauge function as all ROIs have separated Gain
                //else
                //    blnGaugeResult = m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

                if (!blnGaugeResult)
                {
                    m_smVisionInfo.g_strErrorMessage = "*" + m_smVisionInfo.g_arrPad[i].ref_strErrorMessage;
                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                }
            }
            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    if (m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize)
            //    {
            //        m_smVisionInfo.g_arrPad[i].SetRectGauge4LPlacement(m_smVisionInfo.g_arrPadROIs[i][1]);

            //        m_smVisionInfo.g_arrPad[i].ref_objRectGauge4L.ResetGaugeSettingToUserVariables();

            //        m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPad[0].ref_fImageGain);
            //        AttachImageToROI(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_objPackageImage);
            //        m_smVisionInfo.g_blnViewPackageImage = true;
            //        if (m_smVisionInfo.g_blnViewPackageImage)
            //            m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_objPackageImage);
            //        else
            //            m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
            //    }
            //}
            if (m_smVisionInfo.g_arrPad.Length == 5 && m_smVisionInfo.g_arrPad[0].ref_blnMeasureCenterPkgSizeUsingSidePkg && m_smVisionInfo.g_blnCheck4Sides)
            {
                m_smVisionInfo.g_arrPad[0].MeasureEdge_UsingSidePkgCornerPoint(
                                      m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[0].X,
                                      m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[0].Y,
                                      m_smVisionInfo.g_arrPad[1].ref_objRectGauge4L.ref_arrRectCornerPoints[1].X,
                                      m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[1].Y,
                                      m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[2].X,
                                      m_smVisionInfo.g_arrPad[2].ref_objRectGauge4L.ref_arrRectCornerPoints[2].Y,
                                      m_smVisionInfo.g_arrPad[3].ref_objRectGauge4L.ref_arrRectCornerPoints[3].X,
                                      m_smVisionInfo.g_arrPad[4].ref_objRectGauge4L.ref_arrRectCornerPoints[3].Y);
            }

        }
        private void FindUnitPattern()
        {
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == 0)
                {
                    if (!m_smVisionInfo.g_arrPad[i].FindUnitUsingPRS(m_smVisionInfo.g_arrPadROIs[i][0], 10, false))
                    {
                        m_smVisionInfo.g_strErrorMessage = "*" + m_smVisionInfo.g_arrPad[i].ref_strErrorMessage;
                        m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    }
                }
                else
                {
                    if (!m_smVisionInfo.g_arrPad[i].FindUnitUsingPRS(m_smVisionInfo.g_arrPadROIs[i][0], 2, false)) // Side pad angle is a bit only
                    {
                        m_smVisionInfo.g_strErrorMessage = "*" + m_smVisionInfo.g_arrPad[i].ref_strErrorMessage;
                        m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                    }
                }
               
            }
           
        }
        private void AttachImageToROI(List<List<ROI>> arrROIs, ImageDrawing objImage)
        {
            for (int i = 0; i < arrROIs.Count; i++)
            {
                for (int j = 0; j < arrROIs[i].Count; j++)
                {
                    ROI objROI = arrROIs[i][j];

                    switch (objROI.ref_intType)
                    {
                        case 1:
                            objROI.AttachImage(objImage);
                            break;
                        case 2:
                            objROI.AttachImage(arrROIs[i][0]);
                            break;
                        case 3:
                            objROI.AttachImage(arrROIs[i][1]);
                            break;
                    }

                }
            }
        }
        private void txt_PkgStartPointFromEdge_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPadPkgStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPointFromEdge_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadPkgStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPointFromRight_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPadPkgStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPointFromRight_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadPkgStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_PkgStartPointFromBottom_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPadPkgStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPointFromBottom_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadPkgStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_PkgStartPointFromLeft_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPadPkgStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPointFromLeft_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadPkgStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPointFromEdge_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromEdge.Text, out fStartPixelFromEdge))
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelFromEdge.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelFromEdge.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelFromBottom.Text = txt_ChipStartPixelFromEdge.Text;
                    txt_ChipStartPixelFromLeft.Text = txt_ChipStartPixelFromEdge.Text;
                    txt_ChipStartPixelFromRight.Text = txt_ChipStartPixelFromEdge.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToSingle(txt_ChipStartPointFromEdge.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToSingle(txt_ChipStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToSingle(txt_ChipStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToSingle(txt_ChipStartPointFromEdge.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToSingle(txt_ChipStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToSingle(txt_ChipStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToSingle(txt_ChipStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToSingle(txt_ChipStartPixelFromEdge.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToSingle(txt_ChipStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToSingle(txt_ChipStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToSingle(txt_ChipStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToSingle(txt_ChipStartPixelFromEdge.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromEdge.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromRight.Text, out fStartPixelFromEdge))
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelFromRight.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelFromRight.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelFromBottom.Text = txt_ChipStartPixelFromRight.Text;
                    txt_ChipStartPixelFromLeft.Text = txt_ChipStartPixelFromRight.Text;
                    txt_ChipStartPixelFromEdge.Text = txt_ChipStartPixelFromRight.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToSingle(txt_ChipStartPixelFromRight.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToSingle(txt_ChipStartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToSingle(txt_ChipStartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToSingle(txt_ChipStartPixelFromRight.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToSingle(txt_ChipStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToSingle(txt_ChipStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToSingle(txt_ChipStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToSingle(txt_ChipStartPixelFromRight.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToSingle(txt_ChipStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToSingle(txt_ChipStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToSingle(txt_ChipStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToSingle(txt_ChipStartPixelFromRight.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromRight.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromBottom.Text, out fStartPixelFromEdge))
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelFromBottom.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelFromBottom.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelFromRight.Text = txt_ChipStartPixelFromBottom.Text;
                    txt_ChipStartPixelFromLeft.Text = txt_ChipStartPixelFromBottom.Text;
                    txt_ChipStartPixelFromEdge.Text = txt_ChipStartPixelFromBottom.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToSingle(txt_ChipStartPixelFromBottom.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToSingle(txt_ChipStartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToSingle(txt_ChipStartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToSingle(txt_ChipStartPixelFromBottom.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToSingle(txt_ChipStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToSingle(txt_ChipStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToSingle(txt_ChipStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToSingle(txt_ChipStartPixelFromBottom.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToSingle(txt_ChipStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToSingle(txt_ChipStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToSingle(txt_ChipStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToSingle(txt_ChipStartPixelFromBottom.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromBottom.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromLeft.Text, out fStartPixelFromEdge))
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelFromLeft.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelFromLeft.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelFromRight.Text = txt_ChipStartPixelFromLeft.Text;
                    txt_ChipStartPixelFromBottom.Text = txt_ChipStartPixelFromLeft.Text;
                    txt_ChipStartPixelFromEdge.Text = txt_ChipStartPixelFromLeft.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToSingle(txt_ChipStartPixelFromLeft.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToSingle(txt_ChipStartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToSingle(txt_ChipStartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToSingle(txt_ChipStartPixelFromLeft.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToSingle(txt_ChipStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToSingle(txt_ChipStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToSingle(txt_ChipStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToSingle(txt_ChipStartPixelFromLeft.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToSingle(txt_ChipStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToSingle(txt_ChipStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToSingle(txt_ChipStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToSingle(txt_ChipStartPixelFromLeft.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft = Convert.ToInt32(txt_ChipStartPixelFromLeft.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void RotateImage()
        {
            if (m_smVisionInfo.g_intSelectedImage == -1)
            {
                //if (m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
                if (true)
                {
                    // Define rotation ROI with ROI center point same as gauge unit center point
                    float fROIWidth = Math.Min(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalX,
                                        m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X);
                    float fROIHeight = Math.Min(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalY,
                                        m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y);

                    ROI objRotateROI = new ROI();
                    objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                    objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X - fROIWidth, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y - fROIHeight, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(fROIWidth * 2, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(fROIHeight * 2, 0, MidpointRounding.AwayFromZero));

                    // Rotate image to zero degree using RectGauge4L angle result
                    ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage], objRotateROI, // Middle Pad Search ROI
                                      m_smVisionInfo.g_arrPad[0].GetResultAngle_RectGauge4L(),
                    ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_arrPadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                }
                else
                {
                    m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_arrPadROIs[0][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                }
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    //if (m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize)
                    if (true)
                    {
                        //m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                        m_smVisionInfo.g_arrPadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                        if (i > 0 && m_smVisionInfo.g_intSelectedImage == 1)    // For side pad and when background is white color.
                            m_smVisionInfo.g_arrPad[i].AddGrayColorOuterGaugePoint(m_smVisionInfo.g_arrPadROIs[i][0], (Pad.PadIndex)i);



                        // Define rotation ROI with ROI center point same as gauge unit center point
                        //float fROIWidth = Math.Min(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().X - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX,
                        //                    m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth - m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().X);
                        //float fROIHeight = Math.Min(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().Y - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY,
                        //                    m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight - m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().Y);

                        //float fROIWidth = m_smVisionInfo.g_arrPad[i].GetResultMaxWidth_RectGauge4L(0) / 2 + 3;
                        //float fROIHeight = m_smVisionInfo.g_arrPad[i].GetResultMaxHeight_RectGauge4L(0) / 2 + 3;


                        //objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);
                        //objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().X - fROIWidth, 0, MidpointRounding.AwayFromZero),
                        //                                        (int)Math.Round(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().Y - fROIHeight, 0, MidpointRounding.AwayFromZero),
                        //                                        (int)Math.Round(fROIWidth * 2, 0, MidpointRounding.AwayFromZero),
                        //                                        (int)Math.Round(fROIHeight * 2, 0, MidpointRounding.AwayFromZero));

                        // --------- 2018 10 19 - CCENG: Use this new formula define ROI position and size. --------------------
                        // ----------------------------: Rotate at gauge center but using search ROI size ----------------------
                        // ----------------------------: Not using gauge size to prevent rotation defect near unit edge --------

                        int intRotateCenterX = (int)Math.Round(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().X, 0, MidpointRounding.AwayFromZero);
                        int intRotateCenterY = (int)Math.Round(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().Y, 0, MidpointRounding.AwayFromZero);

                        int intStartX = Math.Max(0, intRotateCenterX - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth / 2);
                        int intEndX = Math.Min(m_smVisionInfo.g_arrImages[0].ref_intImageWidth, intRotateCenterX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth / 2);
                        int intHalfWidth = Math.Min(intRotateCenterX - intStartX, intEndX - intRotateCenterX);
                        intStartX = intRotateCenterX - intHalfWidth;

                        int intStartY = Math.Max(0, intRotateCenterY - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight / 2);
                        int intEndY = Math.Min(m_smVisionInfo.g_arrImages[0].ref_intImageHeight, intRotateCenterY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight / 2);
                        int intHalfHeight = Math.Min(intRotateCenterY - intStartY, intEndY - intRotateCenterY);
                        intStartY = intRotateCenterY - intHalfHeight;

                        ROI objRotateROI = new ROI();
                        objRotateROI.AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);    // 2018 10 18 - CCENG: New empty ROI must attach to any image. Without attach any image, the TotalOrgXY value will be 0.
                        objRotateROI.LoadROISetting(intStartX, intStartY, intHalfWidth * 2, intHalfHeight * 2);


                        // Rotate image to zero degree using RectGauge4L angle result
                        if (i == 0)
                        {
                            for (int j = 0; j < m_smVisionInfo.g_arrImages.Count; j++)
                            {
                                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[j], objRotateROI, // Middle Pad Search ROI
                                                  m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(),
                                ref m_smVisionInfo.g_arrRotatedImages, j);
                            }
                        }
                        else
                        {
                            for (int j = 0; j < m_smVisionInfo.g_arrImages.Count; j++)
                            {
                                ROI.Rotate0Degree(objRotateROI, // Middle Pad Search ROI
                                              m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(),
                            ref m_smVisionInfo.g_arrRotatedImages, j);
                            }
                        }

                        // if side pad + selected learn image is index 0 + WantConsiderImage2 is true, then rotate image index 1 as well before logic Add image index 1 apply to image index 0 
                        if (i > 0 && m_smVisionInfo.g_intSelectedImage == 0 && m_smVisionInfo.g_arrPad[i].ref_blnWantConsiderPadImage2)
                        {
                            if (i == 1)
                            {
                                ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[1], objRotateROI, // Middle Pad Search ROI
                                                  m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(),
                                ref m_smVisionInfo.g_arrRotatedImages, 1);
                            }
                            else
                            {
                                ROI.Rotate0Degree(objRotateROI, // Middle Pad Search ROI
                                                  m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(),
                                ref m_smVisionInfo.g_arrRotatedImages, 1);
                            }
                        }

                        m_smVisionInfo.g_blnViewRotatedImage = true;

                        m_smVisionInfo.g_arrPadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                    }
                    //else
                    //{
                    //    //if (!m_smVisionInfo.g_blnViewRotatedImage)    // Need to re-rotate and re-copy where the ViewRotatedImage is true or not. Bcos Rotated image has been modified for dont care area 
                    //    {
                    //        float fRotatedDegree = float.Parse(txt_RotateDegree.Text) * -1f;
                    //        if (Math.Abs(fRotatedDegree) > 0)
                    //        {
                    //            ROI.Rotate0Degree(m_smVisionInfo.g_arrImages[0], m_smVisionInfo.g_arrPadROIs[0][0], fRotatedDegree, ref m_smVisionInfo.g_arrRotatedImages, 0);
                    //            m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrPad[0].ref_fImageGain);

                    //            m_smVisionInfo.g_blnViewRotatedImage = true;
                    //            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

                    //        }
                    //        else
                    //        {

                    //            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_arrRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    //            m_smVisionInfo.g_arrPadROIs[i][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                    //            m_smVisionInfo.g_blnViewRotatedImage = true;
                    //            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
                    //        }
                    //    }
                    //}
                }
            }
        }

        private void RotateColorImage()
        {
            if (m_smVisionInfo.g_intSelectedImage == -1)
            {
                if (m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
                {
                    // Define rotation ROI with ROI center point same as gauge unit center point
                    float fROIWidth = Math.Min(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalX,
                                        m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIWidth - m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X);
                    float fROIHeight = Math.Min(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y - m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalY,
                                        m_smVisionInfo.g_arrPadROIs[0][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[0][0].ref_ROIHeight - m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y);

                    CROI objRotateROI = new CROI();
                    objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage]);
                    objRotateROI.LoadROISetting((int)Math.Round(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().X - fROIWidth, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(m_smVisionInfo.g_arrPad[0].GetResultCenterPoint_RectGauge4L().Y - fROIHeight, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(fROIWidth * 2, 0, MidpointRounding.AwayFromZero),
                                                            (int)Math.Round(fROIHeight * 2, 0, MidpointRounding.AwayFromZero));

                    // Rotate image to zero degree using RectGauge4L angle result
                    CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage], objRotateROI, // Middle Pad Search ROI
                                      m_smVisionInfo.g_arrPad[0].GetResultAngle_RectGauge4L(), 8,
                    ref m_smVisionInfo.g_arrColorRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_arrPadColorROIs[0][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                }
                else
                {
                    m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage].CopyTo(ref m_smVisionInfo.g_arrColorRotatedImages, m_smVisionInfo.g_intSelectedImage);
                    m_smVisionInfo.g_blnViewRotatedImage = true;

                    m_smVisionInfo.g_arrPadColorROIs[0][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                }
            }
            else
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                        break;

                    if (m_smVisionInfo.g_arrPad[i].ref_blnWantGaugeMeasurePkgSize)
                    {
                        //m_smVisionInfo.g_arrPad[i].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);

                        //if (i > 0 && m_smVisionInfo.g_intSelectedImage == 1)    // For side pad and when background is white color.
                        //    m_smVisionInfo.g_arrPad[i].AddGrayColorOuterGaugePoint(m_smVisionInfo.g_arrPadROIs[i][0], (Pad.PadIndex)i);

                        // --------- 2018 10 19 - CCENG: Use this new formula define ROI position and size. --------------------
                        // ----------------------------: Rotate at gauge center but using search ROI size ----------------------
                        // ----------------------------: Not using gauge size to prevent rotation defect near unit edge --------

                        int intRotateCenterX = (int)Math.Round(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().X, 0, MidpointRounding.AwayFromZero);
                        int intRotateCenterY = (int)Math.Round(m_smVisionInfo.g_arrPad[i].GetResultCenterPoint_RectGauge4L().Y, 0, MidpointRounding.AwayFromZero);

                        int intStartX = Math.Max(0, intRotateCenterX - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth / 2);
                        int intEndX = Math.Min(m_smVisionInfo.g_arrImages[0].ref_intImageWidth, intRotateCenterX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth / 2);
                        int intHalfWidth = Math.Min(intRotateCenterX - intStartX, intEndX - intRotateCenterX);
                        intStartX = intRotateCenterX - intHalfWidth;

                        int intStartY = Math.Max(0, intRotateCenterY - m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight / 2);
                        int intEndY = Math.Min(m_smVisionInfo.g_arrImages[0].ref_intImageHeight, intRotateCenterY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight / 2);
                        int intHalfHeight = Math.Min(intRotateCenterY - intStartY, intEndY - intRotateCenterY);
                        intStartY = intRotateCenterY - intHalfHeight;

                        CROI objRotateROI = new CROI();
                        objRotateROI.AttachImage(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage]);    // 2018 10 18 - CCENG: New empty ROI must attach to any image. Without attach any image, the TotalOrgXY value will be 0.
                        objRotateROI.LoadROISetting(intStartX, intStartY, intHalfWidth * 2, intHalfHeight * 2);

                        // Rotate image to zero degree using RectGauge4L angle result
                        if (i == 0)
                        {
                            CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage], objRotateROI, // Middle Pad Search ROI
                                     m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(), 8,
                            ref m_smVisionInfo.g_arrColorRotatedImages, m_smVisionInfo.g_intSelectedImage);
                        }
                        else
                        {
                            if (m_smVisionInfo.g_arrPad[i].ref_blnWantRotateSidePadImage)
                            {
                                CROI.Rotate0Degree(objRotateROI, // Middle Pad Search ROI
                                                  m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(), 8,
                                ref m_smVisionInfo.g_arrColorRotatedImages, m_smVisionInfo.g_intSelectedImage);
                            }
                        }

                        // if side pad + selected learn image is index 0 + WantConsiderImage2 is true, then rotate image index 1 as well before logic Add image index 1 apply to image index 0 
                        if (i > 0 && m_smVisionInfo.g_intSelectedImage == 0 && m_smVisionInfo.g_arrPad[i].ref_blnWantConsiderPadImage2)
                        {
                            if (m_smVisionInfo.g_arrPad[i].ref_blnWantRotateSidePadImage)
                            {
                                if (i == 1)
                                {
                                    CROI.Rotate0Degree(m_smVisionInfo.g_arrColorImages[1], objRotateROI, // Middle Pad Search ROI
                                                      m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(), 8,
                                    ref m_smVisionInfo.g_arrColorRotatedImages, 1);
                                }
                                else
                                {
                                    CROI.Rotate0Degree(objRotateROI, // Middle Pad Search ROI
                                                      m_smVisionInfo.g_arrPad[i].GetResultAngle_RectGauge4L(), 8,
                                    ref m_smVisionInfo.g_arrColorRotatedImages, 1);
                                }
                            }
                        }

                        m_smVisionInfo.g_blnViewRotatedImage = true;

                        if (m_smVisionInfo.g_arrPadColorROIs[i].Count > 0)
                            m_smVisionInfo.g_arrPadColorROIs[i][0].AttachImage(m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage]);
                    }
                }
            }

        }

        private void txt_ChipStartPointFromEdge_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPadChipStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPointFromEdge_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadChipStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPointFromRight_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPadChipStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPointFromRight_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadChipStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_ChipStartPointFromBottom_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPadChipStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPointFromBottom_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadChipStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_ChipStartPointFromLeft_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPadChipStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPointFromLeft_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadChipStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPointFromEdge_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll.Checked)
                {
                    if ((i == 0 || i == 1) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromEdge.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 2 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromEdge.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 3 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromEdge.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 4 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromEdge.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                else
                {
                    if ((i == 0 || i == 1) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromEdge.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 2 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromEdge.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 3 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromEdge.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 4 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromEdge.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }

                if (chk_SetToAll.Checked)
                {
                    txt_MoldStartPixelFromRight.Text = txt_MoldStartPixelFromEdge.Text;
                    txt_MoldStartPixelFromBottom.Text = txt_MoldStartPixelFromEdge.Text;
                    txt_MoldStartPixelFromLeft.Text = txt_MoldStartPixelFromEdge.Text;
                }

            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPointFromEdge.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPointFromEdge.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromEdge.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromEdge.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromEdge.Text);
                            }
                            break;
                    }
                }
            }
            UpdateMoldFlashForeColor();
            CheckMoldFlashROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll.Checked)
                {
                    if ((i == 0 || i == 1) && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromRight.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if ((i == 2) && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromRight.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if ((i == 3) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromRight.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if ((i == 4) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromRight.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                else
                {
                    if ((i == 0 || i == 1) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromRight.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if ((i == 2) && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromRight.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if ((i == 3) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromRight.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if ((i == 4) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromRight.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                if (chk_SetToAll.Checked)
                {
                    txt_MoldStartPixelFromEdge.Text = txt_MoldStartPixelFromRight.Text;
                    txt_MoldStartPixelFromBottom.Text = txt_MoldStartPixelFromRight.Text;
                    txt_MoldStartPixelFromLeft.Text = txt_MoldStartPixelFromRight.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromRight.Text);
                            }
                            break;
                    }
                }
            }
            UpdateMoldFlashForeColor();
            CheckMoldFlashROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll.Checked)
                {
                    if ((i == 0 || i == 1) && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromBottom.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 2 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromBottom.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 3 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromBottom.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 4 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromBottom.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                else
                {
                    if ((i == 0 || i == 1) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromBottom.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 2 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromBottom.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 3 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromBottom.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 4 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromBottom.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                if (chk_SetToAll.Checked)
                {
                    txt_MoldStartPixelFromEdge.Text = txt_MoldStartPixelFromBottom.Text;
                    txt_MoldStartPixelFromRight.Text = txt_MoldStartPixelFromBottom.Text;
                    txt_MoldStartPixelFromLeft.Text = txt_MoldStartPixelFromBottom.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromBottom.Text);
                            }
                            break;
                    }
                }
            }
            UpdateMoldFlashForeColor();
            CheckMoldFlashROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll.Checked)
                {
                    if ((i == 0 || i == 1) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromLeft.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 2 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromLeft.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 3 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromLeft.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 4 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromLeft.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge.ToString();
                        txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                else
                {
                    if ((i == 0 || i == 1) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromLeft.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 2 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromLeft.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 3 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromLeft.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 4 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromLeft.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                if (chk_SetToAll.Checked)
                {
                    txt_MoldStartPixelFromEdge.Text = txt_MoldStartPixelFromLeft.Text;
                    txt_MoldStartPixelFromRight.Text = txt_MoldStartPixelFromLeft.Text;
                    txt_MoldStartPixelFromBottom.Text = txt_MoldStartPixelFromLeft.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToInt32(txt_MoldStartPixelFromLeft.Text);
                            }
                            break;
                    }
                }
            }
            UpdateMoldFlashForeColor();
            CheckMoldFlashROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPointFromEdge_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPadPkgMoldFlashImageViewNo, m_smVisionInfo.g_intVisionIndex);
            m_smVisionInfo.g_blnViewPadMoldStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPointFromEdge_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadMoldStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPointFromRight_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPadPkgMoldFlashImageViewNo, m_smVisionInfo.g_intVisionIndex);
            m_smVisionInfo.g_blnViewPadMoldStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPointFromRight_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadMoldStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_MoldStartPointFromBottom_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPadPkgMoldFlashImageViewNo, m_smVisionInfo.g_intVisionIndex);
            m_smVisionInfo.g_blnViewPadMoldStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPointFromBottom_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadMoldStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_MoldStartPointFromLeft_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPadPkgMoldFlashImageViewNo, m_smVisionInfo.g_intVisionIndex);
            m_smVisionInfo.g_blnViewPadMoldStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPointFromLeft_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadMoldStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetToAll_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllEdges_ROI_Pad", chk_SetToAll.Checked);

            if (m_intSelectedTabPage == 0)
            {
                if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && (!m_smVisionInfo.g_arrPad[0].GetOverallWantGaugeMeasurePkgSize(false) || m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON))
                    CheckForeignMaterialROISetting_Pad();
            }
            else
            {
                //if ((m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg") && m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
                //    CheckForeignMaterialROISetting();
                //else
                //{
                //    CheckPkgROISetting();
                //    CheckPkgROISetting_Dark();
                //    CheckChippedOffROISetting();
                //    CheckMoldFlashROISetting();
                //}

                CheckPkgROISetting();
                CheckPkgROISetting_Dark();
                CheckChippedOffROISetting();
                CheckMoldFlashROISetting();
                CheckForeignMaterialROISetting();

            }
        }

        private void btn_Void_Threshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(1, m_smVisionInfo.g_intVisionIndex);

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2VoidThreshold;

            if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count == 0)
                return;
            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrPad.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[0].ref_intPkgImage2VoidThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 1)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[1].ref_intPkgImage2VoidThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 2)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[2].ref_intPkgImage2VoidThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 3)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[3].ref_intPkgImage2VoidThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 4)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[4].ref_intPkgImage2VoidThreshold);

            bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold); //[0]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrPad.Length; i++)

                    {
                        m_smVisionInfo.g_arrPad[i].ref_intPkgImage2VoidThreshold = m_smVisionInfo.g_intThresholdValue;
                    }
                }
                else
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2VoidThreshold = m_smVisionInfo.g_intThresholdValue;

                lbl_Void_Threshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2VoidThreshold.ToString();
            }
            else
            {

                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2VoidThreshold;

            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_CrackThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            if (m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg")
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(1, m_smVisionInfo.g_intVisionIndex);
            else
                m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(1, m_smVisionInfo.g_intVisionIndex); //[2]
                                                                           // m_smVisionInfo.g_intSelectedImage = 2;
            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2LowCrackThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2HighCrackThreshold;

            if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count == 0)
                return;
            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            //ROI.AttachROIToImage(m_smVisionInfo.g_arrPadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]); //[2]

            bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], false, blnWantSetToAllROICheckBox);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2LowCrackThreshold = intLowThreshold;
                m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2HighCrackThreshold = intHighThreshold;
            }
            else
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        m_smVisionInfo.g_arrPad[i].ref_intPkgImage2LowCrackThreshold = m_smVisionInfo.g_intLowThresholdValue;
                        m_smVisionInfo.g_arrPad[i].ref_intPkgImage2HighCrackThreshold = m_smVisionInfo.g_intHighThresholdValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2LowCrackThreshold = m_smVisionInfo.g_intLowThresholdValue;
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2HighCrackThreshold = m_smVisionInfo.g_intHighThresholdValue;
                }

                lbl_PkgImage2LowThreshold.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2LowCrackThreshold.ToString();
                lbl_PkgImage2HighThreshold.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPkgImage2HighCrackThreshold.ToString();
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_VoidMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_VoidMinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex || (i > 0 && chk_SetToAllSideROI.Checked))
                {
                    m_smVisionInfo.g_arrPad[i].ref_fVoidMinArea = Convert.ToInt32(txt_VoidMinArea.Text);
                }
            }

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}


            //m_smVisionInfo.g_arrPad[intSelectedROI].ref_fVoidMinArea = Convert.ToInt32(txt_VoidMinArea.Text);


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CrackMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_CrackMinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex || (i > 0 && chk_SetToAllSideROI.Checked))
                {
                    m_smVisionInfo.g_arrPad[i].ref_fCrackMinArea = Convert.ToInt32(txt_CrackMinArea.Text);
                }
            }

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}


            //m_smVisionInfo.g_arrPad[intSelectedROI].ref_fCrackMinArea = Convert.ToInt32(txt_CrackMinArea.Text);


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_BrightThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadPkgBrightFieldImageViewNo, m_smVisionInfo.g_intVisionIndex);

            MeasureGauge();
            RotateImage();
            if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                RotateColorImage();
                m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage].ConvertColorToMono(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            }
            m_smVisionInfo.g_blnViewRotatedImage = true;


            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intBrightFieldLowThreshold;

            if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count == 0)
                return;

            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrPad.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[0].ref_intBrightFieldLowThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 1)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[1].ref_intBrightFieldLowThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 2)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[2].ref_intBrightFieldLowThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 3)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[3].ref_intBrightFieldLowThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 4)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[4].ref_intBrightFieldLowThreshold);

            bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold); //[0]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        //if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                        //    continue;

                        m_smVisionInfo.g_arrPad[i].ref_intBrightFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intBrightFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                }
                lbl_BrightThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intBrightFieldLowThreshold.ToString();
            }
            else
            {

                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intBrightFieldLowThreshold;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DarkThreshold_Click(object sender, EventArgs e)
        {
            int intSelectedROI = cbo_SelectROI.SelectedIndex;
            List<int> arrrThreshold = new List<int>();

            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}

            

            //if (m_smVisionInfo.g_arrblnImageRotated[m_smVisionInfo.g_intSelectedImage])
            //    m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);
            //else
            //    m_smVisionInfo.g_arrColorImages[m_smVisionInfo.g_intSelectedImage].RedrawImage(m_Graphic);

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadPkgDarkFieldImageViewNo, m_smVisionInfo.g_intVisionIndex);

            MeasureGauge();
            RotateImage();
            if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                RotateColorImage();
                m_smVisionInfo.g_arrColorRotatedImages[m_smVisionInfo.g_intSelectedImage].ConvertColorToMono(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            }
            m_smVisionInfo.g_blnViewRotatedImage = true;
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intDarkFieldLowThreshold;
            m_smVisionInfo.g_fThresholdGainValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_fDarkFieldImageGain;

            if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count == 0)
                return;

            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            if (m_smVisionInfo.g_arrPad.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[0].ref_intDarkFieldLowThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 1)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[1].ref_intDarkFieldLowThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 2)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[2].ref_intDarkFieldLowThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 3)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[3].ref_intDarkFieldLowThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 4)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[4].ref_intDarkFieldLowThreshold);

      
            bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
            //ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox); //[0]
            ThresholdWithGainForm objThresholdForm = new ThresholdWithGainForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold); //[0]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        //if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                        //    continue;

                        m_smVisionInfo.g_arrPad[i].ref_intDarkFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                        m_smVisionInfo.g_arrPad[i].ref_fDarkFieldImageGain = m_smVisionInfo.g_fThresholdGainValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intDarkFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_fDarkFieldImageGain = m_smVisionInfo.g_fThresholdGainValue;
                }
                lbl_DarkThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intDarkFieldLowThreshold.ToString();
            }
            else
            {
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intDarkFieldLowThreshold;
                m_smVisionInfo.g_fThresholdGainValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_fDarkFieldImageGain;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_BrightMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_BrightMinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex || (i > 0 && chk_SetToAllSideROI.Checked))
                {
                    m_smVisionInfo.g_arrPad[i].ref_intBrightFieldMinArea = Convert.ToInt32(txt_BrightMinArea.Text);
                }
            }

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}


            //m_smVisionInfo.g_arrPad[intSelectedROI].ref_intBrightFieldMinArea = Convert.ToInt32(txt_BrightMinArea.Text);


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_PadInspectionAreaSetting_Click(object sender, EventArgs e)
        {
            if (m_objPadInspectionAreaSettingForm == null)
                m_objPadInspectionAreaSettingForm = new PadInspectionAreaSettingForm(m_smCustomizeInfo, m_smVisionInfo,
                       m_strSelectedRecipe, m_intUserGroup, m_smProductionInfo);
            m_objPadInspectionAreaSettingForm.Show();
            Rectangle objScreenRect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            m_objPadInspectionAreaSettingForm.StartPosition = FormStartPosition.Manual;
            m_objPadInspectionAreaSettingForm.Location = new Point(objScreenRect.Width - m_objPadInspectionAreaSettingForm.Width - 10,
                objScreenRect.Height - m_objPadInspectionAreaSettingForm.Height - 10);
            m_objPadInspectionAreaSettingForm.TopMost = true;
            tab_VisionControl.Enabled = false;
            btn_Save.Enabled = false;
            btn_Cancel.Enabled = false;
        }

        private void txt_DarkMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_DarkMinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex || (i > 0 && chk_SetToAllSideROI.Checked))
                {
                    m_smVisionInfo.g_arrPad[i].ref_intDarkFieldMinArea = Convert.ToInt32(txt_DarkMinArea.Text);
                }
            }

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}


            //m_smVisionInfo.g_arrPad[intSelectedROI].ref_intDarkFieldMinArea = Convert.ToInt32(txt_DarkMinArea.Text);
        }

        private void txt_ThinIteration_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex || (i > 0 && chk_SetToAllSideROI.Checked))
                {
                    m_smVisionInfo.g_arrPad[i].ref_intMPErodeHalfWidth = Convert.ToInt32(txt_ThinIteration.Text);
                }

                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //    m_smVisionInfo.g_arrPad[i].ref_intMPErodeHalfWidth = Convert.ToInt32(txt_ThinIteration.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ThickIteration_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex || (i > 0 && chk_SetToAllSideROI.Checked))
                {
                    m_smVisionInfo.g_arrPad[i].ref_intMPDilateHalfWidth = Convert.ToInt32(txt_ThickIteration.Text);
                }

                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //    m_smVisionInfo.g_arrPad[i].ref_intMPDilateHalfWidth = Convert.ToInt32(txt_ThickIteration.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromEdgeInner_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll.Checked)
                {
                    if ((i == 0 || i == 1) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 2 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 3 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 4 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                else
                {
                    if ((i == 0 || i == 1) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 2 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 3 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 4 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }

                if (chk_SetToAll.Checked)
                {
                    txt_MoldStartPixelFromRightInner.Text = txt_MoldStartPixelFromEdgeInner.Text;
                    txt_MoldStartPixelFromBottomInner.Text = txt_MoldStartPixelFromEdgeInner.Text;
                    txt_MoldStartPixelFromLeftInner.Text = txt_MoldStartPixelFromEdgeInner.Text;
                }

            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPointFromEdge.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPointFromEdge.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToSingle(txt_MoldStartPixelFromEdgeInner.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromEdgeInner.Text);
                            }
                            break;
                    }
                }
            }
            UpdateMoldFlashForeColor();
            CheckMoldFlashROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPointExtendFromEdge_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromEdge.Text, out fStartPixelExtendFromEdge))
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelExtendFromEdge.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelExtendFromEdge.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelExtendFromBottom.Text = txt_ChipStartPixelExtendFromEdge.Text;
                    txt_ChipStartPixelExtendFromLeft.Text = txt_ChipStartPixelExtendFromEdge.Text;
                    txt_ChipStartPixelExtendFromRight.Text = txt_ChipStartPixelExtendFromEdge.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToSingle(txt_ChipStartPointFromEdge.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToSingle(txt_ChipStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToSingle(txt_ChipStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToSingle(txt_ChipStartPointFromEdge.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromRight_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromRight.Text, out fStartPixelExtendFromEdge))
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelExtendFromBottom.Text = txt_ChipStartPixelExtendFromRight.Text;
                    txt_ChipStartPixelExtendFromLeft.Text = txt_ChipStartPixelExtendFromRight.Text;
                    txt_ChipStartPixelExtendFromEdge.Text = txt_ChipStartPixelExtendFromRight.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToSingle(txt_ChipStartPixelExtendFromRight.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromRight.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromBottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromBottom.Text, out fStartPixelExtendFromEdge))
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelExtendFromRight.Text = txt_ChipStartPixelExtendFromBottom.Text;
                    txt_ChipStartPixelExtendFromLeft.Text = txt_ChipStartPixelExtendFromBottom.Text;
                    txt_ChipStartPixelExtendFromEdge.Text = txt_ChipStartPixelExtendFromBottom.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromLeft_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fStartPixelExtendFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromLeft.Text, out fStartPixelExtendFromEdge))
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelExtendFromRight.Text = txt_ChipStartPixelExtendFromLeft.Text;
                    txt_ChipStartPixelExtendFromBottom.Text = txt_ChipStartPixelExtendFromLeft.Text;
                    txt_ChipStartPixelExtendFromEdge.Text = txt_ChipStartPixelExtendFromLeft.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPointFromEdge_Dark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelFromEdge_Dark.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelFromEdge_Dark.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelFromBottom_Dark.Text = txt_ChipStartPixelFromEdge_Dark.Text;
                    txt_ChipStartPixelFromLeft_Dark.Text = txt_ChipStartPixelFromEdge_Dark.Text;
                    txt_ChipStartPixelFromRight_Dark.Text = txt_ChipStartPixelFromEdge_Dark.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToSingle(txt_ChipStartPointFromEdge_Dark.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToSingle(txt_ChipStartPointFromEdge_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToSingle(txt_ChipStartPointFromEdge_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToSingle(txt_ChipStartPointFromEdge_Dark.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelFromEdge_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelFromEdge_Dark.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelFromEdge_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelFromEdge_Dark.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromEdge_Dark.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting_Dark();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromRight_Dark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelFromBottom_Dark.Text = txt_ChipStartPixelFromRight_Dark.Text;
                    txt_ChipStartPixelFromLeft_Dark.Text = txt_ChipStartPixelFromRight_Dark.Text;
                    txt_ChipStartPixelFromEdge_Dark.Text = txt_ChipStartPixelFromRight_Dark.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelFromRight_Dark.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromRight_Dark.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting_Dark();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromBottom_Dark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelFromRight_Dark.Text = txt_ChipStartPixelFromBottom_Dark.Text;
                    txt_ChipStartPixelFromLeft_Dark.Text = txt_ChipStartPixelFromBottom_Dark.Text;
                    txt_ChipStartPixelFromEdge_Dark.Text = txt_ChipStartPixelFromBottom_Dark.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelFromBottom_Dark.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromBottom_Dark.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting_Dark();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromLeft_Dark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelFromRight_Dark.Text = txt_ChipStartPixelFromLeft_Dark.Text;
                    txt_ChipStartPixelFromBottom_Dark.Text = txt_ChipStartPixelFromLeft_Dark.Text;
                    txt_ChipStartPixelFromEdge_Dark.Text = txt_ChipStartPixelFromLeft_Dark.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelFromLeft_Dark.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelFromLeft_Dark.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting_Dark();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPointExtendFromEdge_Dark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelExtendFromEdge_Dark.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelExtendFromEdge_Dark.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelExtendFromBottom_Dark.Text = txt_ChipStartPixelExtendFromEdge_Dark.Text;
                    txt_ChipStartPixelExtendFromLeft_Dark.Text = txt_ChipStartPixelExtendFromEdge_Dark.Text;
                    txt_ChipStartPixelExtendFromRight_Dark.Text = txt_ChipStartPixelExtendFromEdge_Dark.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToSingle(txt_ChipStartPointFromEdge_Dark.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToSingle(txt_ChipStartPointFromEdge_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToSingle(txt_ChipStartPointFromEdge_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToSingle(txt_ChipStartPointFromEdge_Dark.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromEdge_Dark.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting_Dark();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromRight_Dark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromRight_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromRight_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelExtendFromBottom_Dark.Text = txt_ChipStartPixelExtendFromRight_Dark.Text;
                    txt_ChipStartPixelExtendFromLeft_Dark.Text = txt_ChipStartPixelExtendFromRight_Dark.Text;
                    txt_ChipStartPixelExtendFromEdge_Dark.Text = txt_ChipStartPixelExtendFromRight_Dark.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromRight_Dark.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting_Dark();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromBottom_Dark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelExtendFromRight_Dark.Text = txt_ChipStartPixelExtendFromBottom_Dark.Text;
                    txt_ChipStartPixelExtendFromLeft_Dark.Text = txt_ChipStartPixelExtendFromBottom_Dark.Text;
                    txt_ChipStartPixelExtendFromEdge_Dark.Text = txt_ChipStartPixelExtendFromBottom_Dark.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromBottom_Dark.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting_Dark();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelExtendFromLeft_Dark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelExtendFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelExtendFromRight_Dark.Text = txt_ChipStartPixelExtendFromLeft_Dark.Text;
                    txt_ChipStartPixelExtendFromBottom_Dark.Text = txt_ChipStartPixelExtendFromLeft_Dark.Text;
                    txt_ChipStartPixelExtendFromEdge_Dark.Text = txt_ChipStartPixelExtendFromLeft_Dark.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToSingle(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromBottom_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromEdge_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromRight_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fChipStartPixelExtendFromLeft_Dark = Convert.ToInt32(txt_ChipStartPixelExtendFromLeft_Dark.Text);
                            }
                            break;
                    }
                }
            }
            CheckChippedOffROISetting_Dark();
            IsChipInwardOutwardSettingCorrect(false);
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromRightInner_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll.Checked)
                {
                    if ((i == 0 || i == 1) && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if ((i == 2) && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if ((i == 3) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if ((i == 4) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                else
                {
                    if ((i == 0 || i == 1) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if ((i == 2) && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if ((i == 3) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if ((i == 4) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                if (chk_SetToAll.Checked)
                {
                    txt_MoldStartPixelFromEdgeInner.Text = txt_MoldStartPixelFromRightInner.Text;
                    txt_MoldStartPixelFromBottomInner.Text = txt_MoldStartPixelFromRightInner.Text;
                    txt_MoldStartPixelFromLeftInner.Text = txt_MoldStartPixelFromRightInner.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromRight.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToSingle(txt_MoldStartPixelFromRightInner.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromRightInner.Text);
                            }
                            break;
                    }
                }
            }
            UpdateMoldFlashForeColor();
            CheckMoldFlashROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromEdge_Dark_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPadChipStartPixelFromEdge_Dark = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromEdge_Dark_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadChipStartPixelFromEdge_Dark = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromEdge_Dark_Enter(object sender, EventArgs e)
        {
            MeasureGauge();
            m_smVisionInfo.g_blnViewPadPkgStartPixelFromEdge_Dark = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromEdge_Dark_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadPkgStartPixelFromEdge_Dark = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromEdge_Dark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_PkgStartPointFromEdge.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPointFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_PkgStartPointFromEdge.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPointFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_PkgStartPixelFromEdge_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_PkgStartPixelFromEdge_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_PkgStartPixelFromEdge_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_PkgStartPixelFromEdge_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromEdge_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                if (chk_SetToAll.Checked)
                {
                    txt_PkgStartPixelFromRight_Dark.Text = txt_PkgStartPixelFromEdge_Dark.Text;
                    txt_PkgStartPixelFromBottom_Dark.Text = txt_PkgStartPixelFromEdge_Dark.Text;
                    txt_PkgStartPixelFromLeft_Dark.Text = txt_PkgStartPixelFromEdge_Dark.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToSingle(txt_PkgStartPixelFromEdge_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToSingle(txt_PkgStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToSingle(txt_PkgStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToSingle(txt_PkgStartPixelFromEdge_Dark.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToSingle(txt_PkgStartPixelFromEdge_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToSingle(txt_PkgStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToSingle(txt_PkgStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToSingle(txt_PkgStartPixelFromEdge_Dark.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromEdge_Dark.Text);
                            }
                            break;
                    }
                }
            }
            CheckPkgROISetting_Dark();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromRight_Dark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}

                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_PkgStartPixelFromEdge_Dark.Text = txt_PkgStartPixelFromRight_Dark.Text;
                    txt_PkgStartPixelFromBottom_Dark.Text = txt_PkgStartPixelFromRight_Dark.Text;
                    txt_PkgStartPixelFromLeft_Dark.Text = txt_PkgStartPixelFromRight_Dark.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToSingle(txt_PkgStartPixelFromRight_Dark.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromRight_Dark.Text);
                            }
                            break;
                    }
                }
            }
            CheckPkgROISetting_Dark();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromBottom_Dark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_PkgStartPixelFromEdge_Dark.Text = txt_PkgStartPixelFromBottom_Dark.Text;
                    txt_PkgStartPixelFromRight_Dark.Text = txt_PkgStartPixelFromBottom_Dark.Text;
                    txt_PkgStartPixelFromLeft_Dark.Text = txt_PkgStartPixelFromBottom_Dark.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToSingle(txt_PkgStartPixelFromBottom_Dark.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromBottom_Dark.Text);
                            }
                            break;
                    }
                }
            }
            CheckPkgROISetting_Dark();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromLeft_Dark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text) + m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft_Dark.Text = m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                if (chk_SetToAll.Checked)
                {
                    txt_PkgStartPixelFromEdge_Dark.Text = txt_PkgStartPixelFromLeft_Dark.Text;
                    txt_PkgStartPixelFromRight_Dark.Text = txt_PkgStartPixelFromLeft_Dark.Text;
                    txt_PkgStartPixelFromBottom_Dark.Text = txt_PkgStartPixelFromLeft_Dark.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToSingle(txt_PkgStartPixelFromLeft_Dark.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromBottom_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromEdge_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromRight_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fPkgStartPixelFromLeft_Dark = Convert.ToInt32(txt_PkgStartPixelFromLeft_Dark.Text);
                            }
                            break;
                    }
                }
            }
            CheckPkgROISetting_Dark();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromBottomInner_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll.Checked)
                {
                    if ((i == 0 || i == 1) && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 2 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 3 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 4 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                else
                {
                    if ((i == 0 || i == 1) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 2 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 3 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 4 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                if (chk_SetToAll.Checked)
                {
                    txt_MoldStartPixelFromEdgeInner.Text = txt_MoldStartPixelFromBottomInner.Text;
                    txt_MoldStartPixelFromRightInner.Text = txt_MoldStartPixelFromBottomInner.Text;
                    txt_MoldStartPixelFromLeftInner.Text = txt_MoldStartPixelFromBottomInner.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromBottom.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToSingle(txt_MoldStartPixelFromBottomInner.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromBottomInner.Text);
                            }
                            break;
                    }
                }
            }
            UpdateMoldFlashForeColor();
            CheckMoldFlashROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ForeignMaterialStartPixelFromEdge_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrPad[i].GetOverallWantGaugeMeasurePkgSize(false))
                {
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                }
                else
                {
                    fUnitWidth = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Width;
                    fUnitHeight = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Height;
                }

                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromEdge.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                if (chk_SetToAll.Checked)
                {
                    txt_ForeignMaterialStartPixelFromRight.Text = txt_ForeignMaterialStartPixelFromEdge.Text;
                    txt_ForeignMaterialStartPixelFromBottom.Text = txt_ForeignMaterialStartPixelFromEdge.Text;
                    txt_ForeignMaterialStartPixelFromLeft.Text = txt_ForeignMaterialStartPixelFromEdge.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge.Text);
                            }
                            break;
                    }
                }
            }
            CheckForeignMaterialROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ForeignMaterialStartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrPad[i].GetOverallWantGaugeMeasurePkgSize(false))
                {
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                }
                else
                {
                    fUnitWidth = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Width;
                    fUnitHeight = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Height;
                }
                
                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_ForeignMaterialStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_ForeignMaterialStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}

                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ForeignMaterialStartPixelFromEdge.Text = txt_ForeignMaterialStartPixelFromRight.Text;
                    txt_ForeignMaterialStartPixelFromBottom.Text = txt_ForeignMaterialStartPixelFromRight.Text;
                    txt_ForeignMaterialStartPixelFromLeft.Text = txt_ForeignMaterialStartPixelFromRight.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight.Text);
                            }
                            break;
                    }
                }
            }
            CheckForeignMaterialROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ForeignMaterialStartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrPad[i].GetOverallWantGaugeMeasurePkgSize(false))
                {
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                }
                else
                {
                    fUnitWidth = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Width;
                    fUnitHeight = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Height;
                }

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_ForeignMaterialStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_ForeignMaterialStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ForeignMaterialStartPixelFromEdge.Text = txt_ForeignMaterialStartPixelFromBottom.Text;
                    txt_ForeignMaterialStartPixelFromRight.Text = txt_ForeignMaterialStartPixelFromBottom.Text;
                    txt_ForeignMaterialStartPixelFromLeft.Text = txt_ForeignMaterialStartPixelFromBottom.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom.Text);
                            }
                            break;
                    }
                }
            }
            CheckForeignMaterialROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ForeignMaterialStartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrPad[i].GetOverallWantGaugeMeasurePkgSize(false))
                {
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                }
                else
                {
                    fUnitWidth = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Width;
                    fUnitHeight = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Height;
                }

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_ForeignMaterialStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_ForeignMaterialStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                if (chk_SetToAll.Checked)
                {
                    txt_ForeignMaterialStartPixelFromEdge.Text = txt_ForeignMaterialStartPixelFromLeft.Text;
                    txt_ForeignMaterialStartPixelFromRight.Text = txt_ForeignMaterialStartPixelFromLeft.Text;
                    txt_ForeignMaterialStartPixelFromBottom.Text = txt_ForeignMaterialStartPixelFromLeft.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft.Text);
                            }
                            break;
                    }
                }
            }
            CheckForeignMaterialROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_ForeignMaterialThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[intSelectedROI].ref_intPadPkgBrightFieldImageViewNo, m_smVisionInfo.g_intVisionIndex);

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intForeignMaterialBrightFieldThreshold;

            if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count == 0)
                return;

            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrPad.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[0].ref_intForeignMaterialBrightFieldThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 1)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[1].ref_intForeignMaterialBrightFieldThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 2)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[2].ref_intForeignMaterialBrightFieldThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 3)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[3].ref_intForeignMaterialBrightFieldThreshold);
            if (m_smVisionInfo.g_arrPad.Length > 4)
                arrrThreshold.Add(m_smVisionInfo.g_arrPad[4].ref_intForeignMaterialBrightFieldThreshold);

            bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold); //[0]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        //if (!m_smVisionInfo.g_arrPad[i].ref_blnSelected)
                        //    continue;

                        m_smVisionInfo.g_arrPad[i].ref_intForeignMaterialBrightFieldThreshold = m_smVisionInfo.g_intThresholdValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intForeignMaterialBrightFieldThreshold = m_smVisionInfo.g_intThresholdValue;
                }
                lbl_ForeignMaterialThreshold.Text = m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_intForeignMaterialBrightFieldThreshold.ToString();
            }
            else
            {

                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intForeignMaterialBrightFieldThreshold;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }
        private void txt_ForeignMaterialStartPixelFromEdge_Pad_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrPad[i].GetOverallWantGaugeMeasurePkgSize(false))
                {
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                }
                else
                {
                    fUnitWidth = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Width;
                    fUnitHeight = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Height;
                }

                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromEdge_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromEdge_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromEdge_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromEdge_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                if (chk_SetToAll.Checked)
                {
                    txt_ForeignMaterialStartPixelFromRight_Pad.Text = txt_ForeignMaterialStartPixelFromEdge_Pad.Text;
                    txt_ForeignMaterialStartPixelFromBottom_Pad.Text = txt_ForeignMaterialStartPixelFromEdge_Pad.Text;
                    txt_ForeignMaterialStartPixelFromLeft_Pad.Text = txt_ForeignMaterialStartPixelFromEdge_Pad.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_PkgStartPointFromEdge.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromEdge_Pad.Text);
                            }
                            break;
                    }
                }
            }
            CheckForeignMaterialROISetting_Pad();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_ForeignMaterialStartPixelFromRight_Pad_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrPad[i].GetOverallWantGaugeMeasurePkgSize(false))
                {
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                }
                else
                {
                    fUnitWidth = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Width;
                    fUnitHeight = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Height;
                }

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_ForeignMaterialStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_ForeignMaterialStartPixelFromRight.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}

                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromRight_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromRight_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromRight_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromRight_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ForeignMaterialStartPixelFromEdge_Pad.Text = txt_ForeignMaterialStartPixelFromRight_Pad.Text;
                    txt_ForeignMaterialStartPixelFromBottom_Pad.Text = txt_ForeignMaterialStartPixelFromRight_Pad.Text;
                    txt_ForeignMaterialStartPixelFromLeft_Pad.Text = txt_ForeignMaterialStartPixelFromRight_Pad.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromRight_Pad.Text);
                            }
                            break;
                    }
                }
            }
            CheckForeignMaterialROISetting_Pad();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_ForeignMaterialStartPixelFromBottom_Pad_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrPad[i].GetOverallWantGaugeMeasurePkgSize(false))
                {
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                }
                else
                {
                    fUnitWidth = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Width;
                    fUnitHeight = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Height;
                }

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_ForeignMaterialStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_ForeignMaterialStartPixelFromBottom.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromBottom_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromBottom_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromBottom_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromBottom_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ForeignMaterialStartPixelFromEdge_Pad.Text = txt_ForeignMaterialStartPixelFromBottom_Pad.Text;
                    txt_ForeignMaterialStartPixelFromRight_Pad.Text = txt_ForeignMaterialStartPixelFromBottom_Pad.Text;
                    txt_ForeignMaterialStartPixelFromLeft_Pad.Text = txt_ForeignMaterialStartPixelFromBottom_Pad.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromBottom_Pad.Text);
                            }
                            break;
                    }
                }
            }
            CheckForeignMaterialROISetting_Pad();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_ForeignMaterialStartPixelFromLeft_Pad_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrPad[i].GetOverallWantGaugeMeasurePkgSize(false))
                {
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                }
                else
                {
                    fUnitWidth = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Width;
                    fUnitHeight = m_smVisionInfo.g_arrPad[i].GetPatternSize_UnitMatcher().Height;
                }

                //if ((i == 0 || i == 1 || i == 3) && Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text) >= (fUnitWidth / 2) * m_smVisionInfo.g_fCalibPixelX)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_ForeignMaterialStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                //else if ((i == 2 || i == 4) && Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text) >= (fUnitHeight / 2) * m_smVisionInfo.g_fCalibPixelY)
                //{
                //    m_blnUpdateSelectedROISetting = false;
                //    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //    txt_ForeignMaterialStartPixelFromLeft.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft.ToString();
                //    m_blnUpdateSelectedROISetting = false;
                //    return;
                //}
                if ((i == 0 || i == 1) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromLeft_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 3) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad) >= (fUnitWidth * m_smVisionInfo.g_fCalibPixelX)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromLeft_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromLeft_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 4) && ((Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft_Pad.Text) + m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad) >= (fUnitHeight * m_smVisionInfo.g_fCalibPixelY)))
                {
                    SRMMessageBox.Show("Edge tolerance cannot goes beyond opposite direction tolerance.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ForeignMaterialStartPixelFromLeft_Pad.Text = m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                if (chk_SetToAll.Checked)
                {
                    txt_ForeignMaterialStartPixelFromEdge_Pad.Text = txt_ForeignMaterialStartPixelFromLeft_Pad.Text;
                    txt_ForeignMaterialStartPixelFromRight_Pad.Text = txt_ForeignMaterialStartPixelFromLeft_Pad.Text;
                    txt_ForeignMaterialStartPixelFromBottom_Pad.Text = txt_ForeignMaterialStartPixelFromLeft_Pad.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToSingle(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromBottom_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromEdge_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromRight_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fForeignMaterialStartPixelFromLeft_Pad = Convert.ToInt32(txt_ForeignMaterialStartPixelFromLeft_Pad.Text);
                            }
                            break;
                    }
                }
            }
            CheckForeignMaterialROISetting_Pad();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_ForeignMaterialMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_ForeignMaterialMinArea.Text == "")
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex || (i > 0 && chk_SetToAllSideROI.Checked))
                {
                    m_smVisionInfo.g_arrPad[i].ref_intForeignMaterialBrightFieldMinArea = Convert.ToInt32(txt_ForeignMaterialMinArea.Text);
                }
            }

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}


            //m_smVisionInfo.g_arrPad[intSelectedROI].ref_intForeignMaterialBrightFieldMinArea = Convert.ToInt32(txt_ForeignMaterialMinArea.Text);


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        
        private void tab_VisionControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_blnIgnoreTappaeIndexChange)
            {
                m_blnIgnoreTappaeIndexChange = false;
                return;
            }

            if (tab_VisionControl.Contains(tp_ROI2))
            {
                if (!IsChipInwardOutwardSettingCorrect(true))
                {
                    m_blnIgnoreTappaeIndexChange = true;
                    tab_VisionControl.SelectedTab = tp_ROI2;
                    return;
                }
            }

            if (tab_VisionControl.SelectedTab == tp_ROI)
            {
                if (!tp_ROI.Controls.Contains(chk_SetToAll))
                    tp_ROI.Controls.Add(chk_SetToAll);

                if (!tp_ROI.Controls.Contains(chk_SetToAllSideROI))
                    tp_ROI.Controls.Add(chk_SetToAllSideROI);

                if (!tp_ROI.Controls.Contains(cbo_SelectROI))
                    tp_ROI.Controls.Add(cbo_SelectROI);
            }
            else if (tab_VisionControl.SelectedTab == tp_ROI2)
            {
                if (!tp_ROI2.Controls.Contains(chk_SetToAll))
                    tp_ROI2.Controls.Add(chk_SetToAll);

                if (!tp_ROI2.Controls.Contains(chk_SetToAllSideROI))
                    tp_ROI2.Controls.Add(chk_SetToAllSideROI);

                if (!tp_ROI2.Controls.Contains(cbo_SelectROI))
                    tp_ROI2.Controls.Add(cbo_SelectROI);
            }
            else if (tab_VisionControl.SelectedTab == tp_ROI_Pad)
            {
                if (!tp_ROI_Pad.Controls.Contains(chk_SetToAll))
                    tp_ROI_Pad.Controls.Add(chk_SetToAll);

                if (!tp_ROI_Pad.Controls.Contains(chk_SetToAllSideROI))
                    tp_ROI_Pad.Controls.Add(chk_SetToAllSideROI);

                if (!tp_ROI_Pad.Controls.Contains(cbo_SelectROI))
                    tp_ROI_Pad.Controls.Add(cbo_SelectROI);
            }
            else if (tab_VisionControl.SelectedTab == tp_Segment)
            {
                if (!tp_Segment.Controls.Contains(cbo_SelectROI))
                    tp_Segment.Controls.Add(cbo_SelectROI);

                if (!tp_Segment.Controls.Contains(chk_SetToAllSideROI))
                    tp_Segment.Controls.Add(chk_SetToAllSideROI);
            }
            else if (tab_VisionControl.SelectedTab == tp_PkgSegmentSimple)
            {
                if (!tp_PkgSegmentSimple.Controls.Contains(cbo_SelectROI))
                    tp_PkgSegmentSimple.Controls.Add(cbo_SelectROI);

                if (!tp_PkgSegmentSimple.Controls.Contains(chk_SetToAllSideROI))
                    tp_PkgSegmentSimple.Controls.Add(chk_SetToAllSideROI);
            }
            else if (tab_VisionControl.SelectedTab == tp_PkgSegment)
            {
                if (!tp_PkgSegment.Controls.Contains(cbo_SelectROI))
                    tp_PkgSegment.Controls.Add(cbo_SelectROI);

                if (!tp_PkgSegment.Controls.Contains(chk_SetToAllSideROI))
                    tp_PkgSegment.Controls.Add(chk_SetToAllSideROI);
            }
            else if (tab_VisionControl.SelectedTab == tp_PkgSegment2)
            {
                if (!tp_PkgSegment2.Controls.Contains(cbo_SelectROI))
                    tp_PkgSegment2.Controls.Add(cbo_SelectROI);

                if (!tp_PkgSegment2.Controls.Contains(chk_SetToAllSideROI))
                    tp_PkgSegment2.Controls.Add(chk_SetToAllSideROI);
            }
            else if (tab_VisionControl.SelectedTab == tp_other)
            {
                if (!tp_other.Controls.Contains(cbo_SelectROI))
                    tp_other.Controls.Add(cbo_SelectROI);

                if (!tp_other.Controls.Contains(chk_SetToAllSideROI))
                    tp_other.Controls.Add(chk_SetToAllSideROI);
            }
        }

        private void txt_MoldStartPixelFromLeftInner_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrPadROIs.Count; j++)
            //{
            //    if (j > 0 && !m_smVisionInfo.g_blnCheck4Sides)
            //        break;

            //    if (m_smVisionInfo.g_arrPadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrPadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);
                fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrPad[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll.Checked)
                {
                    if ((i == 0 || i == 1) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 2 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 3 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 4 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromEdgeInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge.ToString();
                        txt_MoldStartPixelFromRightInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight.ToString();
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        txt_MoldStartPixelFromBottomInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                else
                {
                    if ((i == 0 || i == 1) && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX - Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 2 && (m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY - Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text) <= m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 3 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIWidth) + Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                    else if (i == 4 && ((m_smVisionInfo.g_arrPadROIs[i][2].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][2].ref_ROIHeight) + Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text) >= (m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrPadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromLeftInner.Text = m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }
                }
                if (chk_SetToAll.Checked)
                {
                    txt_MoldStartPixelFromEdgeInner.Text = txt_MoldStartPixelFromLeftInner.Text;
                    txt_MoldStartPixelFromRightInner.Text = txt_MoldStartPixelFromLeftInner.Text;
                    txt_MoldStartPixelFromBottomInner.Text = txt_MoldStartPixelFromLeftInner.Text;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i != intSelectedROI)
                    continue;

                // 2019 01 09 - JBTAN: currently set start pixel is the same for every pad ROI
                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromLeft = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                //if (chk_SetToAll.Checked)
                //{
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromRight = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromBottom = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                //    m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelFromEdge = Convert.ToSingle(txt_MoldStartPixelFromLeft.Text);
                //}
                switch (i)
                {
                    case 0: // Center
                    case 1: // Top
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text);
                        }
                        break;
                    case 2: // Right
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                        }
                        break;
                    case 3: // Bottom
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                        }
                        break;
                    case 4: // Left
                        m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                        }
                        break;
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                if (i == intSelectedROI || i == 0)
                    continue;

                if (chk_SetToAllSideROI.Checked)
                {
                    switch (i)
                    {
                        case 1: // Top
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToSingle(txt_MoldStartPixelFromLeftInner.Text);
                            }
                            break;
                        case 2: // Right
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                            }
                            break;
                        case 3: // Bottom
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                            }
                            break;
                        case 4: // Left
                            m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromBottom = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                            if (chk_SetToAll.Checked)
                            {
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromEdge = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromRight = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                                m_smVisionInfo.g_arrPad[i].ref_fMoldStartPixelInnerFromLeft = Convert.ToInt32(txt_MoldStartPixelFromLeftInner.Text);
                            }
                            break;
                    }
                }
            }
            UpdateMoldFlashForeColor();
            CheckMoldFlashROISetting();
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void cbo_SelectROI_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnIgnoreComboBoxIndexChange)
            {
                m_blnIgnoreComboBoxIndexChange = false;
                return;
            }

            if (!IsChipInwardOutwardSettingCorrect(true))
            {
                m_blnIgnoreComboBoxIndexChange = true;
                cbo_SelectROI.SelectedIndex = m_smVisionInfo.g_intSelectedROI;
                return;
            }

            m_smVisionInfo.g_intSelectedROI = cbo_SelectROI.SelectedIndex;
            m_smVisionInfo.g_intSelectedROIMask = 0x01 << m_smVisionInfo.g_intSelectedROI;
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if ((m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                {
                    if (i < m_smVisionInfo.g_arrPadROIs.Count)
                    {
                        if (m_smVisionInfo.g_arrPadROIs[i].Count > 0)
                        {
                            m_smVisionInfo.g_arrPadROIs[i][0].VerifyROIArea(m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalCenterX, m_smVisionInfo.g_arrPadROIs[i][0].ref_ROITotalCenterY);
                        }
                    }
                }
                else
                {
                    if (i < m_smVisionInfo.g_arrPadROIs.Count)
                    {
                        if (m_smVisionInfo.g_arrPadROIs[i].Count > 0)
                        {
                            m_smVisionInfo.g_arrPadROIs[i][0].ClearDragHandle();
                        }
                    }
                }

            }

            m_smVisionInfo.g_blnUpdateSelectedROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_ForeignMaterialStartPixelFromEdge_Enter(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].GetOverallWantGaugeMeasurePkgSize(false))
                MeasureGauge();
            else
                FindUnitPattern();

            m_smVisionInfo.g_blnViewPadForeignMaterialStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ForeignMaterialStartPixelFromEdge_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadForeignMaterialStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_ForeignMaterialStartPixelFromEdge_Pad_Enter(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].GetOverallWantGaugeMeasurePkgSize(false))
                MeasureGauge();
            else
                FindUnitPattern();

            m_smVisionInfo.g_blnViewPadForeignMaterialStartPixelFromEdge_Pad = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ForeignMaterialStartPixelFromEdge_Pad_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewPadForeignMaterialStartPixelFromEdge_Pad = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void srmGroupBox16_Enter(object sender, EventArgs e)
        {

        }

        private void panel_Other_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txt_PadImageMerge2MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (i == cbo_SelectROI.SelectedIndex || (i > 0 && chk_SetToAllSideROI.Checked))
                {
                    m_smVisionInfo.g_arrPad[i].ref_fImageMerge2BlobsMinArea = Convert.ToSingle(txt_PadImageMerge2MinArea.Text);
                }

                //if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                //    m_smVisionInfo.g_arrPad[i].ref_fImageMerge2BlobsMinArea = Convert.ToSingle(txt_PadImageMerge2MinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

            if (!m_blnInitDone)
                return;
        }

        private void btn_PadImageMerge2Threshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_blnViewPadResultDrawing = false;

            //int intSelectedROI;
            //if (m_smVisionInfo.g_blnCheck4Sides)
            //{
            //    intSelectedROI = m_smVisionInfo.g_intSelectedROI;
            //}
            //else
            //{
            //    intSelectedROI = 0;
            //}
            int intSelectedROI = cbo_SelectROI.SelectedIndex;

            //m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intImageMerge2ThresholdValue;

            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intImageMerge2ThresholdLowValue;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intImageMerge2ThresholdHighValue;

            if (m_smVisionInfo.g_arrPadROIs[intSelectedROI].Count == 0)
                return;

            int intImageIndex = m_smVisionInfo.g_intSelectedImage = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrPad[intSelectedROI].ref_intBrokenPadImageViewNo, m_smVisionInfo.g_intVisionIndex);

            if (m_smVisionInfo.g_blnViewRotatedImage)
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrRotatedImages[intImageIndex]);
            else
                m_smVisionInfo.g_arrPadROIs[intSelectedROI][0].AttachImage(m_smVisionInfo.g_arrImages[intImageIndex]);

            m_smVisionInfo.g_intThresholdDrawLowValue = 0;
            m_smVisionInfo.g_intThresholdDrawMiddleValue = 255;
            m_smVisionInfo.g_intThresholdDrawHighValue = 0;

            bool blnWantSetToAllROICheckBox = (m_smVisionInfo.g_arrPadROIs.Count > 1) && m_smVisionInfo.g_blnCheck4Sides;
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrPadROIs[intSelectedROI][0], false, blnWantSetToAllROICheckBox);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        //m_smVisionInfo.g_arrPad[i].ref_intImageMerge2ThresholdValue = m_smVisionInfo.g_intThresholdValue;
                        m_smVisionInfo.g_arrPad[i].ref_intImageMerge2ThresholdLowValue = m_smVisionInfo.g_intLowThresholdValue;
                        m_smVisionInfo.g_arrPad[i].ref_intImageMerge2ThresholdHighValue = m_smVisionInfo.g_intHighThresholdValue;
                    }
                }
                else
                {
                    //m_smVisionInfo.g_arrPad[intSelectedROI].ref_intImageMerge2ThresholdValue = m_smVisionInfo.g_intThresholdValue;
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intImageMerge2ThresholdLowValue = m_smVisionInfo.g_intLowThresholdValue;
                    m_smVisionInfo.g_arrPad[intSelectedROI].ref_intImageMerge2ThresholdHighValue = m_smVisionInfo.g_intHighThresholdValue;
                }
                lbl_PadImageMerge2Threshold_Low.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intImageMerge2ThresholdLowValue.ToString();
                lbl_PadImageMerge2Threshold_High.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intImageMerge2ThresholdHighValue.ToString();
            }
            else
            {
                //m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrPad[intSelectedROI].ref_intImageMerge2ThresholdValue;
                m_smVisionInfo.g_arrPad[intSelectedROI].ref_intImageMerge2ThresholdLowValue = intLowThreshold;
                m_smVisionInfo.g_arrPad[intSelectedROI].ref_intImageMerge2ThresholdHighValue = intHighThreshold;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetToAllSideROI_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllSide_ROI_Pad", chk_SetToAllSideROI.Checked);

            if (m_intSelectedTabPage == 0)
            {
                if (((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0) && (!m_smVisionInfo.g_arrPad[0].GetOverallWantGaugeMeasurePkgSize(false) || m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON))
                    CheckForeignMaterialROISetting_Pad();
            }
            else
            {
                //if ((m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg") && m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize)
                //    CheckForeignMaterialROISetting();
                //else
                //{
                //    CheckPkgROISetting();
                //    CheckPkgROISetting_Dark();
                //    CheckChippedOffROISetting();
                //    CheckMoldFlashROISetting();
                //}

                CheckPkgROISetting();
                CheckPkgROISetting_Dark();
                CheckChippedOffROISetting();
                CheckMoldFlashROISetting();
                CheckForeignMaterialROISetting();
            }
        }
        
        private void btn_PadROIToleranceSetting_Click(object sender, EventArgs e)
        {
            PadROIToleranceSettingForm objPadROIToleranceSettingForm = new PadROIToleranceSettingForm(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_strSelectedRecipe, m_intUserGroup, m_intSelectedTabPage);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objPadROIToleranceSettingForm.StartPosition = FormStartPosition.Manual;
            objPadROIToleranceSettingForm.Location = new Point(resolution.Width - objPadROIToleranceSettingForm.Size.Width, resolution.Height - objPadROIToleranceSettingForm.Size.Height);
            objPadROIToleranceSettingForm.ShowDialog();
        }
        private void CheckForeignMaterialROISetting()
        {

            bool blnTopSame = true, blnRightSame = true, blnBottomSame = true, blnLeftSame = true;

            if (chk_SetToAllSideROI.Checked && m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (m_smVisionInfo.g_intSelectedROI != 0 && i == 0)
                        continue;

                    switch (i)
                    {
                        case 0:// Center
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 1:// Top
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 2:// Right
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 3:// Bottom
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 4:// Left
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft))
                                    blnLeftSame = false;
                            break;

                    }
                }
            }

            bool blnSame = true;

            //if (chk_SetToAll.Checked)//  && blnTopSame && blnRightSame && blnBottomSame && blnLeftSame)
            //{
            //    if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromBottom) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromLeft) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromRight))
            //        blnSame = false;
            //}

            if (blnTopSame)
                lbl_ForeignMaterialROITop.ForeColor = Color.Black;
            else
                lbl_ForeignMaterialROITop.ForeColor = Color.Red;

            if (blnRightSame)
                lbl_ForeignMaterialROIRight.ForeColor = Color.Black;
            else
                lbl_ForeignMaterialROIRight.ForeColor = Color.Red;

            if (blnBottomSame)
                lbl_ForeignMaterialROIBottom.ForeColor = Color.Black;
            else
                lbl_ForeignMaterialROIBottom.ForeColor = Color.Red;

            if (blnLeftSame)
                lbl_ForeignMaterialROILeft.ForeColor = Color.Black;
            else
                lbl_ForeignMaterialROILeft.ForeColor = Color.Red;

            if (blnSame)
            {
                if (blnTopSame)
                    lbl_ForeignMaterialROITop.ForeColor = Color.Black;
                if (blnRightSame)
                    lbl_ForeignMaterialROIRight.ForeColor = Color.Black;
                if (blnBottomSame)
                    lbl_ForeignMaterialROIBottom.ForeColor = Color.Black;
                if (blnLeftSame)
                    lbl_ForeignMaterialROILeft.ForeColor = Color.Black;
            }
            else
            {
                lbl_ForeignMaterialROITop.ForeColor = Color.Red;
                lbl_ForeignMaterialROIRight.ForeColor = Color.Red;
                lbl_ForeignMaterialROIBottom.ForeColor = Color.Red;
                lbl_ForeignMaterialROILeft.ForeColor = Color.Red;
            }

        }
        private void CheckForeignMaterialROISetting_Pad()
        {

            bool blnTopSame = true, blnRightSame = true, blnBottomSame = true, blnLeftSame = true;

            if (chk_SetToAllSideROI.Checked && m_smVisionInfo.g_arrPad.Length > 1)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (m_smVisionInfo.g_intSelectedROI != 0 && i == 0)
                        continue;

                    switch (i)
                    {
                        case 0:// Center
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge_Pad) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight_Pad) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom_Pad) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft_Pad))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight_Pad) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom_Pad) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft_Pad) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge_Pad))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom_Pad) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft_Pad) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge_Pad) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight_Pad))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft_Pad) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge_Pad) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight_Pad) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom_Pad))
                                    blnLeftSame = false;

                            break;
                        case 1:// Top
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight_Pad) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom_Pad) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft_Pad))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom_Pad) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft_Pad) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge_Pad))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft_Pad) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge_Pad) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight_Pad))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge_Pad) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight_Pad) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom_Pad))
                                    blnLeftSame = false;

                            break;
                        case 2:// Right
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge_Pad) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom_Pad) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft_Pad))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight_Pad) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft_Pad) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge_Pad))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom_Pad) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge_Pad) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight_Pad))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft_Pad) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight_Pad) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom_Pad))
                                    blnLeftSame = false;

                            break;
                        case 3:// Bottom
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight_Pad) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge_Pad) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft_Pad))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom_Pad) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight_Pad) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge_Pad))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft_Pad) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom_Pad) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight_Pad))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge_Pad) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft_Pad) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom_Pad))
                                    blnLeftSame = false;

                            break;
                        case 4:// Left
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromRight_Pad) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromBottom_Pad) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromLeft_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromEdge_Pad))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromBottom_Pad) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromLeft_Pad) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromEdge_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromRight_Pad))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromLeft_Pad) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromEdge_Pad) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromRight_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromBottom_Pad))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[2].ref_fForeignMaterialStartPixelFromEdge_Pad) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[3].ref_fForeignMaterialStartPixelFromRight_Pad) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fForeignMaterialStartPixelFromBottom_Pad != m_smVisionInfo.g_arrPad[1].ref_fForeignMaterialStartPixelFromLeft_Pad))
                                    blnLeftSame = false;
                            break;

                    }
                }
            }

            bool blnSame = true;

            //if (chk_SetToAll.Checked)//  && blnTopSame && blnRightSame && blnBottomSame && blnLeftSame)
            //{
            //    if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromBottom) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromLeft) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fForeignMaterialStartPixelFromRight))
            //        blnSame = false;
            //}

            if (blnTopSame)
                lbl_ForeignMaterialROITop_Pad.ForeColor = Color.Black;
            else
                lbl_ForeignMaterialROITop_Pad.ForeColor = Color.Red;

            if (blnRightSame)
                lbl_ForeignMaterialROIRight_Pad.ForeColor = Color.Black;
            else
                lbl_ForeignMaterialROIRight_Pad.ForeColor = Color.Red;

            if (blnBottomSame)
                lbl_ForeignMaterialROIBottom_Pad.ForeColor = Color.Black;
            else
                lbl_ForeignMaterialROIBottom_Pad.ForeColor = Color.Red;

            if (blnLeftSame)
                lbl_ForeignMaterialROILeft_Pad.ForeColor = Color.Black;
            else
                lbl_ForeignMaterialROILeft_Pad.ForeColor = Color.Red;

            if (blnSame)
            {
                if (blnTopSame)
                    lbl_ForeignMaterialROITop_Pad.ForeColor = Color.Black;
                if (blnRightSame)
                    lbl_ForeignMaterialROIRight_Pad.ForeColor = Color.Black;
                if (blnBottomSame)
                    lbl_ForeignMaterialROIBottom_Pad.ForeColor = Color.Black;
                if (blnLeftSame)
                    lbl_ForeignMaterialROILeft_Pad.ForeColor = Color.Black;
            }
            else
            {
                lbl_ForeignMaterialROITop_Pad.ForeColor = Color.Red;
                lbl_ForeignMaterialROIRight_Pad.ForeColor = Color.Red;
                lbl_ForeignMaterialROIBottom_Pad.ForeColor = Color.Red;
                lbl_ForeignMaterialROILeft_Pad.ForeColor = Color.Red;
            }

        }
        private void CheckPkgROISetting()
        {

            bool blnTopSame = true, blnRightSame = true, blnBottomSame = true, blnLeftSame = true;

            if (chk_SetToAllSideROI.Checked && m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (m_smVisionInfo.g_intSelectedROI != 0 && i == 0)
                        continue;

                    switch (i)
                    {
                        case 0:// Center
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 1:// Top
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 2:// Right
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 3:// Bottom
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 4:// Left
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft))
                                    blnLeftSame = false;
                            break;

                    }
                }
            }

            bool blnSame = true;

            //if (chk_SetToAll.Checked)//  && blnTopSame && blnRightSame && blnBottomSame && blnLeftSame)
            //{
            //    if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight))
            //        blnSame = false;
            //}

            if (blnTopSame)
                lbl_PkgTop.ForeColor = Color.Black;
            else
                lbl_PkgTop.ForeColor = Color.Red;

            if (blnRightSame)
                lbl_PkgRight.ForeColor = Color.Black;
            else
                lbl_PkgRight.ForeColor = Color.Red;

            if (blnBottomSame)
                lbl_PkgBottom.ForeColor = Color.Black;
            else
                lbl_PkgBottom.ForeColor = Color.Red;

            if (blnLeftSame)
                lbl_PkgLeft.ForeColor = Color.Black;
            else
                lbl_PkgLeft.ForeColor = Color.Red;

            if (blnSame)
            {
                if (blnTopSame)
                    lbl_PkgTop.ForeColor = Color.Black;
                if (blnRightSame)
                    lbl_PkgRight.ForeColor = Color.Black;
                if (blnBottomSame)
                    lbl_PkgBottom.ForeColor = Color.Black;
                if (blnLeftSame)
                    lbl_PkgLeft.ForeColor = Color.Black;
            }
            else
            {
                lbl_PkgTop.ForeColor = Color.Red;
                lbl_PkgRight.ForeColor = Color.Red;
                lbl_PkgBottom.ForeColor = Color.Red;
                lbl_PkgLeft.ForeColor = Color.Red;
            }

        }
        private void CheckPkgROISetting_Dark()
        {
            bool blnTopSame = true, blnRightSame = true, blnBottomSame = true, blnLeftSame = true;

            if (chk_SetToAllSideROI.Checked && m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (m_smVisionInfo.g_intSelectedROI != 0 && i == 0)
                        continue;

                    switch (i)
                    {
                        case 0:// Center
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft_Dark))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge_Dark))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight_Dark))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom_Dark))
                                    blnLeftSame = false;

                            break;
                        case 1:// Top
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft_Dark))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge_Dark))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight_Dark))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom_Dark))
                                    blnLeftSame = false;

                            break;
                        case 2:// Right
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft_Dark))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge_Dark))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight_Dark))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom_Dark))
                                    blnLeftSame = false;

                            break;
                        case 3:// Bottom
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft_Dark))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge_Dark))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight_Dark))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom_Dark))
                                    blnLeftSame = false;

                            break;
                        case 4:// Left
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromEdge_Dark))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromRight_Dark))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromBottom_Dark))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[2].ref_fPkgStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[3].ref_fPkgStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fPkgStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[1].ref_fPkgStartPixelFromLeft_Dark))
                                    blnLeftSame = false;
                            break;

                    }
                }
            }

            bool blnSame = true;

            //if (chk_SetToAll.Checked)//  && blnTopSame && blnRightSame && blnBottomSame && blnLeftSame)
            //{
            //    if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight))
            //        blnSame = false;
            //}

            if (blnTopSame)
                lbl_PkgTop.ForeColor = Color.Black;
            else
                lbl_PkgTop.ForeColor = Color.Red;

            if (blnRightSame)
                lbl_PkgRight.ForeColor = Color.Black;
            else
                lbl_PkgRight.ForeColor = Color.Red;

            if (blnBottomSame)
                lbl_PkgBottom.ForeColor = Color.Black;
            else
                lbl_PkgBottom.ForeColor = Color.Red;

            if (blnLeftSame)
                lbl_PkgLeft.ForeColor = Color.Black;
            else
                lbl_PkgLeft.ForeColor = Color.Red;

            if (blnSame)
            {
                if (blnTopSame)
                    lbl_PkgTop.ForeColor = Color.Black;
                if (blnRightSame)
                    lbl_PkgRight.ForeColor = Color.Black;
                if (blnBottomSame)
                    lbl_PkgBottom.ForeColor = Color.Black;
                if (blnLeftSame)
                    lbl_PkgLeft.ForeColor = Color.Black;
            }
            else
            {
                lbl_PkgTop.ForeColor = Color.Red;
                lbl_PkgRight.ForeColor = Color.Red;
                lbl_PkgBottom.ForeColor = Color.Red;
                lbl_PkgLeft.ForeColor = Color.Red;
            }

        }
        private void CheckChippedOffROISetting()
        {

            bool blnTopSame = true, blnRightSame = true, blnBottomSame = true, blnLeftSame = true;

            if (chk_SetToAllSideROI.Checked && m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (m_smVisionInfo.g_intSelectedROI != 0 && i == 0)
                        continue;

                    switch (i)
                    {
                        case 0:// Center
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 1:// Top
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 2:// Right
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 3:// Bottom
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 4:// Left
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft))
                                    blnLeftSame = false;
                            break;

                    }
                }
            }

            bool blnSame = true;

            //if (chk_SetToAll.Checked)// && blnTopSame && blnRightSame && blnBottomSame && blnLeftSame)
            //{
            //    if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight))
            //        blnSame = false;
            //}

            if (blnTopSame)
                lbl_ChipTop.ForeColor = Color.Black;
            else
                lbl_ChipTop.ForeColor = Color.Red;

            if (blnRightSame)
                lbl_ChipRight.ForeColor = Color.Black;
            else
                lbl_ChipRight.ForeColor = Color.Red;

            if (blnBottomSame)
                lbl_ChipBottom.ForeColor = Color.Black;
            else
                lbl_ChipBottom.ForeColor = Color.Red;

            if (blnLeftSame)
                lbl_ChipLeft.ForeColor = Color.Black;
            else
                lbl_ChipLeft.ForeColor = Color.Red;

            if (blnSame)
            {
                if (blnTopSame)
                    lbl_ChipTop.ForeColor = Color.Black;
                if (blnRightSame)
                    lbl_ChipRight.ForeColor = Color.Black;
                if (blnBottomSame)
                    lbl_ChipBottom.ForeColor = Color.Black;
                if (blnLeftSame)
                    lbl_ChipLeft.ForeColor = Color.Black;
            }
            else
            {
                lbl_ChipTop.ForeColor = Color.Red;
                lbl_ChipRight.ForeColor = Color.Red;
                lbl_ChipBottom.ForeColor = Color.Red;
                lbl_ChipLeft.ForeColor = Color.Red;
            }

        }
        
        private void CheckChippedOffROISetting_Dark()
        {

            bool blnTopSame = true, blnRightSame = true, blnBottomSame = true, blnLeftSame = true;

            if (chk_SetToAllSideROI.Checked && m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (m_smVisionInfo.g_intSelectedROI != 0 && i == 0)
                        continue;

                    switch (i)
                    {
                        case 0:// Center
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft_Dark))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge_Dark))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight_Dark))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom_Dark))
                                    blnLeftSame = false;

                            break;
                        case 1:// Top
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft_Dark))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge_Dark))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight_Dark))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom_Dark))
                                    blnLeftSame = false;

                            break;
                        case 2:// Right
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft_Dark))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge_Dark))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight_Dark))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom_Dark))
                                    blnLeftSame = false;

                            break;
                        case 3:// Bottom
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft_Dark))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge_Dark))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight_Dark))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom_Dark))
                                    blnLeftSame = false;

                            break;
                        case 4:// Left
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromLeft_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromLeft_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromEdge_Dark))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromEdge_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromEdge_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromRight_Dark))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromRight_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromBottom_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromRight_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromBottom_Dark))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelFromBottom_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelFromLeft_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[2].ref_fChipStartPixelExtendFromEdge_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[3].ref_fChipStartPixelExtendFromRight_Dark) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fChipStartPixelExtendFromBottom_Dark != m_smVisionInfo.g_arrPad[1].ref_fChipStartPixelExtendFromLeft_Dark))
                                    blnLeftSame = false;
                            break;

                    }
                }
            }

            bool blnSame = true;

            //if (chk_SetToAll.Checked)// && blnTopSame && blnRightSame && blnBottomSame && blnLeftSame)
            //{
            //    if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight))
            //        blnSame = false;
            //}

            if (blnTopSame)
                lbl_ChipTop_Dark.ForeColor = Color.Black;
            else
                lbl_ChipTop_Dark.ForeColor = Color.Red;

            if (blnRightSame)
                lbl_ChipRight_Dark.ForeColor = Color.Black;
            else
                lbl_ChipRight_Dark.ForeColor = Color.Red;

            if (blnBottomSame)
                lbl_ChipBottom_Dark.ForeColor = Color.Black;
            else
                lbl_ChipBottom_Dark.ForeColor = Color.Red;

            if (blnLeftSame)
                lbl_ChipLeft_Dark.ForeColor = Color.Black;
            else
                lbl_ChipLeft_Dark.ForeColor = Color.Red;

            if (blnSame)
            {
                if (blnTopSame)
                    lbl_ChipTop_Dark.ForeColor = Color.Black;
                if (blnRightSame)
                    lbl_ChipRight_Dark.ForeColor = Color.Black;
                if (blnBottomSame)
                    lbl_ChipBottom_Dark.ForeColor = Color.Black;
                if (blnLeftSame)
                    lbl_ChipLeft_Dark.ForeColor = Color.Black;
            }
            else
            {
                lbl_ChipTop_Dark.ForeColor = Color.Red;
                lbl_ChipRight_Dark.ForeColor = Color.Red;
                lbl_ChipBottom_Dark.ForeColor = Color.Red;
                lbl_ChipLeft_Dark.ForeColor = Color.Red;
            }

        }
        
        private void CheckMoldFlashROISetting()
        {

            bool blnTopSame = true, blnRightSame = true, blnBottomSame = true, blnLeftSame = true;

            if (chk_SetToAllSideROI.Checked && m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
            {
                for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    if (m_smVisionInfo.g_intSelectedROI != 0 && i == 0)
                        continue;

                    switch (i)
                    {
                        case 0:// Center
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromRight) ||
                                    (m_smVisionInfo.g_arrPad[0].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 1:// Top
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromRight) ||
                                    (m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 2:// Right
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromRight) ||
                                    (m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 3:// Bottom
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromLeft))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromEdge))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromRight))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromBottom))
                                    blnLeftSame = false;

                            break;
                        case 4:// Left
                            if (blnTopSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromLeft != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromLeft != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromEdge))
                                    blnTopSame = false;

                            if (blnRightSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromRight))
                                    blnRightSame = false;

                            if (blnBottomSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromRight != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromBottom) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromRight != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromBottom))
                                    blnBottomSame = false;

                            if (blnLeftSame)
                                if ((m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelFromBottom != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelFromLeft) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[2].ref_fMoldStartPixelInnerFromEdge) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[3].ref_fMoldStartPixelInnerFromRight) ||
                                    (m_smVisionInfo.g_arrPad[4].ref_fMoldStartPixelInnerFromBottom != m_smVisionInfo.g_arrPad[1].ref_fMoldStartPixelInnerFromLeft))
                                    blnLeftSame = false;
                            break;

                    }
                }
            }

            bool blnSame = true;

            //if (chk_SetToAll.Checked)// && blnTopSame && blnRightSame && blnBottomSame && blnLeftSame)
            //{
            //    if ((m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromBottom) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromLeft) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromRight) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromBottom) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromLeft) ||
            //                (m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromEdge != m_smVisionInfo.g_arrPad[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelInnerFromRight))
            //        blnSame = false;
            //}

            if (blnTopSame)
                lbl_MoldTop.ForeColor = Color.Black;
            else
                lbl_MoldTop.ForeColor = Color.Red;

            if (blnRightSame)
                lbl_MoldRight.ForeColor = Color.Black;
            else
                lbl_MoldRight.ForeColor = Color.Red;

            if (blnBottomSame)
                lbl_MoldBottom.ForeColor = Color.Black;
            else
                lbl_MoldBottom.ForeColor = Color.Red;

            if (blnLeftSame)
                lbl_MoldLeft.ForeColor = Color.Black;
            else
                lbl_MoldLeft.ForeColor = Color.Red;

            if (blnSame)
            {
                if (blnTopSame)
                    lbl_MoldTop.ForeColor = Color.Black;
                if (blnRightSame)
                    lbl_MoldRight.ForeColor = Color.Black;
                if (blnBottomSame)
                    lbl_MoldBottom.ForeColor = Color.Black;
                if (blnLeftSame)
                    lbl_MoldLeft.ForeColor = Color.Black;
            }
            else
            {
                lbl_MoldTop.ForeColor = Color.Red;
                lbl_MoldRight.ForeColor = Color.Red;
                lbl_MoldBottom.ForeColor = Color.Red;
                lbl_MoldLeft.ForeColor = Color.Red;
            }

        }
        private bool IsChipInwardOutwardSettingCorrect(bool blnShowMessage)
        {
            bool blnSettingOK = true;
            float fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromEdge.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromEdge.BackColor = txt_ChipStartPixelFromEdge.NormalBackColor = txt_ChipStartPixelFromEdge.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            float fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromEdge.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelExtendFromEdge.BackColor = txt_ChipStartPixelExtendFromEdge.NormalBackColor = txt_ChipStartPixelExtendFromEdge.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromEdge.BackColor = txt_ChipStartPixelFromEdge.NormalBackColor = txt_ChipStartPixelFromEdge.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromEdge.BackColor = txt_ChipStartPixelExtendFromEdge.NormalBackColor = txt_ChipStartPixelExtendFromEdge.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_ChipStartPixelFromEdge.BackColor = txt_ChipStartPixelFromEdge.NormalBackColor = txt_ChipStartPixelFromEdge.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromEdge.BackColor = txt_ChipStartPixelExtendFromEdge.NormalBackColor = txt_ChipStartPixelExtendFromEdge.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromRight.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromRight.BackColor = txt_ChipStartPixelFromRight.NormalBackColor = txt_ChipStartPixelFromRight.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromRight.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelExtendFromRight.BackColor = txt_ChipStartPixelExtendFromRight.NormalBackColor = txt_ChipStartPixelExtendFromRight.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromRight.BackColor = txt_ChipStartPixelFromRight.NormalBackColor = txt_ChipStartPixelFromRight.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromRight.BackColor = txt_ChipStartPixelExtendFromRight.NormalBackColor = txt_ChipStartPixelExtendFromRight.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }
            }
            else
            {
                txt_ChipStartPixelFromRight.BackColor = txt_ChipStartPixelFromRight.NormalBackColor = txt_ChipStartPixelFromRight.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromRight.BackColor = txt_ChipStartPixelExtendFromRight.NormalBackColor = txt_ChipStartPixelExtendFromRight.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromBottom.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromBottom.BackColor = txt_ChipStartPixelFromBottom.NormalBackColor = txt_ChipStartPixelFromBottom.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromBottom.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelExtendFromBottom.BackColor = txt_ChipStartPixelExtendFromBottom.NormalBackColor = txt_ChipStartPixelExtendFromBottom.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromBottom.BackColor = txt_ChipStartPixelFromBottom.NormalBackColor = txt_ChipStartPixelFromBottom.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromBottom.BackColor = txt_ChipStartPixelExtendFromBottom.NormalBackColor = txt_ChipStartPixelExtendFromBottom.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;

                }
            }
            else
            {
                txt_ChipStartPixelFromBottom.BackColor = txt_ChipStartPixelFromBottom.NormalBackColor = txt_ChipStartPixelFromBottom.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromBottom.BackColor = txt_ChipStartPixelExtendFromBottom.NormalBackColor = txt_ChipStartPixelExtendFromBottom.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromLeft.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromLeft.BackColor = txt_ChipStartPixelFromLeft.NormalBackColor = txt_ChipStartPixelFromLeft.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromLeft.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelExtendFromLeft.BackColor = txt_ChipStartPixelExtendFromLeft.NormalBackColor = txt_ChipStartPixelExtendFromLeft.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromLeft.BackColor = txt_ChipStartPixelFromLeft.NormalBackColor = txt_ChipStartPixelFromLeft.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromLeft.BackColor = txt_ChipStartPixelExtendFromLeft.NormalBackColor = txt_ChipStartPixelExtendFromLeft.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_ChipStartPixelFromLeft.BackColor = txt_ChipStartPixelFromLeft.NormalBackColor = txt_ChipStartPixelFromLeft.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromLeft.BackColor = txt_ChipStartPixelExtendFromLeft.NormalBackColor = txt_ChipStartPixelExtendFromLeft.FocusBackColor = Color.White;

            }

            //-----------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromEdge_Dark.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromEdge_Dark.BackColor = txt_ChipStartPixelFromEdge_Dark.NormalBackColor = txt_ChipStartPixelFromEdge_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromEdge_Dark.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelExtendFromEdge_Dark.BackColor = txt_ChipStartPixelExtendFromEdge_Dark.NormalBackColor = txt_ChipStartPixelExtendFromEdge_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromEdge_Dark.BackColor = txt_ChipStartPixelFromEdge_Dark.NormalBackColor = txt_ChipStartPixelFromEdge_Dark.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromEdge_Dark.BackColor = txt_ChipStartPixelExtendFromEdge_Dark.NormalBackColor = txt_ChipStartPixelExtendFromEdge_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_ChipStartPixelFromEdge_Dark.BackColor = txt_ChipStartPixelFromEdge_Dark.NormalBackColor = txt_ChipStartPixelFromEdge_Dark.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromEdge_Dark.BackColor = txt_ChipStartPixelExtendFromEdge_Dark.NormalBackColor = txt_ChipStartPixelExtendFromEdge_Dark.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromRight_Dark.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromRight_Dark.BackColor = txt_ChipStartPixelFromRight_Dark.NormalBackColor = txt_ChipStartPixelFromRight_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromRight_Dark.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelExtendFromRight_Dark.BackColor = txt_ChipStartPixelExtendFromRight_Dark.NormalBackColor = txt_ChipStartPixelExtendFromRight_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromRight_Dark.BackColor = txt_ChipStartPixelFromRight_Dark.NormalBackColor = txt_ChipStartPixelFromRight_Dark.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromRight_Dark.BackColor = txt_ChipStartPixelExtendFromRight_Dark.NormalBackColor = txt_ChipStartPixelExtendFromRight_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_ChipStartPixelFromRight_Dark.BackColor = txt_ChipStartPixelFromRight_Dark.NormalBackColor = txt_ChipStartPixelFromRight_Dark.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromRight_Dark.BackColor = txt_ChipStartPixelExtendFromRight_Dark.NormalBackColor = txt_ChipStartPixelExtendFromRight_Dark.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromBottom_Dark.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromBottom_Dark.BackColor = txt_ChipStartPixelFromBottom_Dark.NormalBackColor = txt_ChipStartPixelFromBottom_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromBottom_Dark.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelExtendFromBottom_Dark.BackColor = txt_ChipStartPixelExtendFromBottom_Dark.NormalBackColor = txt_ChipStartPixelExtendFromBottom_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromBottom_Dark.BackColor = txt_ChipStartPixelFromBottom_Dark.NormalBackColor = txt_ChipStartPixelFromBottom_Dark.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromBottom_Dark.BackColor = txt_ChipStartPixelExtendFromBottom_Dark.NormalBackColor = txt_ChipStartPixelExtendFromBottom_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_ChipStartPixelFromBottom_Dark.BackColor = txt_ChipStartPixelFromBottom_Dark.NormalBackColor = txt_ChipStartPixelFromBottom_Dark.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromBottom_Dark.BackColor = txt_ChipStartPixelExtendFromBottom_Dark.NormalBackColor = txt_ChipStartPixelExtendFromBottom_Dark.FocusBackColor = Color.White;
            }

            //--------------------

            fInwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelFromLeft_Dark.Text, out fInwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromLeft_Dark.BackColor = txt_ChipStartPixelFromLeft_Dark.NormalBackColor = txt_ChipStartPixelFromLeft_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            fOutwardLimit = 0;
            if (!float.TryParse(txt_ChipStartPixelExtendFromLeft_Dark.Text, out fOutwardLimit))
            {
                blnSettingOK = false;
                txt_ChipStartPixelExtendFromLeft_Dark.BackColor = txt_ChipStartPixelExtendFromLeft_Dark.NormalBackColor = txt_ChipStartPixelExtendFromLeft_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Outward not correct. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }

            if (fInwardLimit < fOutwardLimit)
            {
                blnSettingOK = false;
                txt_ChipStartPixelFromLeft_Dark.BackColor = txt_ChipStartPixelFromLeft_Dark.NormalBackColor = txt_ChipStartPixelFromLeft_Dark.FocusBackColor = Color.Pink;
                txt_ChipStartPixelExtendFromLeft_Dark.BackColor = txt_ChipStartPixelExtendFromLeft_Dark.NormalBackColor = txt_ChipStartPixelExtendFromLeft_Dark.FocusBackColor = Color.Pink;
                if (blnShowMessage)
                {
                    SRMMessageBox.Show("Chipped Off ROI Setting - Inward should not smaller than Outward. Please set again.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                txt_ChipStartPixelFromLeft_Dark.BackColor = txt_ChipStartPixelFromLeft_Dark.NormalBackColor = txt_ChipStartPixelFromLeft_Dark.FocusBackColor = Color.White;
                txt_ChipStartPixelExtendFromLeft_Dark.BackColor = txt_ChipStartPixelExtendFromLeft_Dark.NormalBackColor = txt_ChipStartPixelExtendFromLeft_Dark.FocusBackColor = Color.White;
            }

            return blnSettingOK;
        }
    }
}