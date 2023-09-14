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
using VisionProcessing;
using SharedMemory;
using Microsoft.Win32;
using System.IO;

namespace VisionProcessForm
{
    public partial class Lead3DOtherSettingForm : Form
    {
        #region Member Variables

        private bool m_blnUpdateSelectedROISetting = false;
        private bool m_blnInitDone = false;
        private bool m_blnDragROIPrev = false;
        private int m_intUserGroup = 5;

        private string m_strSelectedRecipe;
        private int m_intSelectedTabPage = 0;

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        Lead3DLineProfileForm m_objLead3DLineProfileForm;

        private float m_fStartPixelFromEdgePrev = -1;
        private float m_fStartPixelFromRightPrev = -1;
        private float m_fStartPixelFromBottomPrev = -1;
        private float m_fStartPixelFromLeftPrev = -1;
        private float m_fStartPixelFromEdgePrev_Chipped = -1;
        private float m_fStartPixelFromRightPrev_Chipped = -1;
        private float m_fStartPixelFromBottomPrev_Chipped = -1;
        private float m_fStartPixelFromLeftPrev_Chipped = -1;
        private float m_fStartPixelFromEdgePrev_Mold = -1;
        private float m_fStartPixelFromRightPrev_Mold = -1;
        private float m_fStartPixelFromBottomPrev_Mold = -1;
        private float m_fStartPixelFromLeftPrev_Mold = -1;
        #endregion

        public Lead3DOtherSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intSelectedTabPage)
        {
            InitializeComponent();
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;


            //if (m_smVisionInfo.g_intSelectedROIMask == 0)
            //{
            //    m_smVisionInfo.g_intSelectedROIMask = 0x01;
            //    m_smVisionInfo.g_intSelectedROI = 0;    // Reset to selecting center ROI when display form.
            //}
            
            cbo_SelectROI.SelectedIndex = m_smVisionInfo.g_intSelectedROI = 0;

            m_intSelectedTabPage = intSelectedTabPage;

            DisableField2();
            UpdateGUI();

            // Set display events
            m_smVisionInfo.g_blnViewROI = m_smVisionInfo.g_blnViewSearchROI = true;

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
                    //tab_VisionControl.TabPages.Remove(tp_ROI);
                    gbox_Chip.Visible = false;
                    gbox_Mold.Visible = false;
                    break;
                case 1:
                 
                        pnl_DarkVoid.Visible = false;
                        //tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                        if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting)
                        {
                            pnl_BrightChipped.Dock = DockStyle.None;
                            pnl_DarkChipped.Dock = DockStyle.None;
                            tp_PkgSegmentSimple.Controls.Add(pnl_BrightChipped);
                            tp_PkgSegmentSimple.Controls.Add(pnl_DarkChipped);

                            pnl_BrightChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                            pnl_DarkChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_BrightChipped.Location.Y + pnl_BrightChipped.Size.Height);


                            if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                            {
                                //pnl_BrightMold.Visible = false;
                                pnl_BrightMold.Dock = DockStyle.None;
                                tp_PkgSegmentSimple.Controls.Add(pnl_BrightMold);
                                pnl_BrightMold.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkChipped.Location.Y + pnl_DarkChipped.Size.Height);

                                if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                                {
                                    //tp_PkgSegment.Controls.Add(pnl_DarkCrack);
                                }
                            }
                            else
                            {
                                if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                                {
                                    pnl_DarkCrack.Dock = DockStyle.None;
                                    tp_PkgSegmentSimple.Controls.Add(pnl_DarkCrack);
                                    pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkChipped.Location.Y + pnl_DarkChipped.Size.Height);
                                }
                            }

                            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting ||
                  !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                                tab_VisionControl.TabPages.Remove(tp_PkgSegment);

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


                            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                            {
                                pnl_BrightMold.Visible = false;
                                pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                            }

                            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                            {
                                pnl_DarkCrack.Visible = false;
                            }
                            tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                        }

                        if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting)
                        {
                            gbox_Chip.Visible = false;
                            gbox_Mold.Location = new Point(gbox_Chip.Location.X, gbox_Chip.Location.Y);
                        }


                        if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                        {
                            gbox_Mold.Visible = false;
                        }

                    
                    tab_VisionControl.TabPages.Remove(tp_Segment);
                    tab_VisionControl.TabPages.Remove(tp_other);
                    break;
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
                    //tab_VisionControl.TabPages.Remove(tp_ROI);
                    gbox_Chip.Visible = false;
                    gbox_Mold.Visible = false;
                    break;
                case 1:

                    pnl_DarkVoid.Visible = false;
                    //tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                    if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting)
                    {
                        pnl_BrightChipped.Dock = DockStyle.None;
                        pnl_DarkChipped.Dock = DockStyle.None;
                        tp_PkgSegmentSimple.Controls.Add(pnl_BrightChipped);
                        tp_PkgSegmentSimple.Controls.Add(pnl_DarkChipped);

                        pnl_BrightChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                        pnl_DarkChipped.Location = new Point(pnl_BrightDefect.Location.X, pnl_BrightChipped.Location.Y + pnl_BrightChipped.Size.Height);


                        if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                        {
                            //pnl_BrightMold.Visible = false;
                            pnl_BrightMold.Dock = DockStyle.None;
                            tp_PkgSegmentSimple.Controls.Add(pnl_BrightMold);
                            pnl_BrightMold.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkChipped.Location.Y + pnl_DarkChipped.Size.Height);

                            if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                            {
                                //tp_PkgSegment.Controls.Add(pnl_DarkCrack);
                            }
                        }
                        else
                        {
                            if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                            {
                                pnl_DarkCrack.Dock = DockStyle.None;
                                tp_PkgSegmentSimple.Controls.Add(pnl_DarkCrack);
                                pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkChipped.Location.Y + pnl_DarkChipped.Size.Height);
                            }
                        }

                        if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting ||
              !m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                            tab_VisionControl.TabPages.Remove(tp_PkgSegment);

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


                        if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                        {
                            pnl_BrightMold.Visible = false;
                            pnl_DarkCrack.Location = new Point(pnl_BrightDefect.Location.X, pnl_DarkDefect.Location.Y + pnl_DarkDefect.Size.Height);
                        }

                        if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                        {
                            pnl_DarkCrack.Visible = false;
                        }
                        tab_VisionControl.TabPages.Remove(tp_PkgSegment);
                    }

                    if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting)
                    {
                        gbox_Chip.Visible = false;
                        gbox_Mold.Location = new Point(gbox_Chip.Location.X, gbox_Chip.Location.Y);
                    }


                    if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                    {
                        gbox_Mold.Visible = false;
                    }


                    tab_VisionControl.TabPages.Remove(tp_Segment);
                    tab_VisionControl.TabPages.Remove(tp_other);
                    break;
            }


            string strChild2 = "Setting Page";
            string strChild3 = "";

            strChild3 = "Lead3D Threshold button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_LeadThreshold.Enabled = false;
            }

            strChild3 = "Lead3D Min Area";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_MinArea.Enabled = false;
            }
            
            strChild3 = "Lead3D Measure Edge Tool Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_LineProfileGaugeSetting.Enabled = false;
            }

            strChild3 = "Lead3D Package To Base Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                group_PkgToBaseToleranceSetting.Enabled = false;
            }

            strChild3 = "Lead3D Tip Build Area Tolerance";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                group_TipBuildAreaTolerance.Enabled = false;
            }

            strChild3 = "Lead3D Inward Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                txt_BaseOffset.Enabled = false;
                txt_TipOffset.Enabled = false;
                txt_TipOffsetSide.Enabled = false;
            }

            strChild3 = "Bright Field Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                pnl_BrightDefect.Enabled = false;
            }

            strChild3 = "Dark Field Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                pnl_DarkDefect.Enabled = false;
            }

            strChild3 = "Chipped Off Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                pnl_BrightChipped.Enabled = false;
                pnl_DarkChipped.Enabled = false;
            }

            strChild3 = "Crack Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                pnl_DarkCrack.Enabled = false;
            }

            strChild3 = "Mold Flash Setting";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                pnl_BrightMold.Enabled = false;
            }

            if (m_intSelectedTabPage > 0)
            {
                strChild3 = "Package ROI Setting";
                if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
                {
                    gbox_Pkg.Enabled = false;
                    gbox_Chip.Enabled = false;
                    gbox_Mold.Enabled = false;
                    chk_SetToAllSideROI.Enabled = false;
                    chk_SetToAll.Enabled = false;
                }
            }
            else
            {
                strChild3 = "Lead3D Package ROI Setting";
                if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
                {
                    gbox_Pkg.Enabled = false;
                    gbox_Chip.Enabled = false;
                    gbox_Mold.Enabled = false;
                    chk_SetToAllSideROI.Enabled = false;
                    chk_SetToAll.Enabled = false;
                }
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
        private void AddTrainPackageROI(List<List<ROI>> arrROIs)
        {
            ROI objROI;

            for (int i = 0; i < arrROIs.Count; i++)
            {
                if (i != 0)
                    break;
                if (arrROIs[i].Count == 0)
                {
                    SRMMessageBox.Show("Search ROI no exist!");
                    return;
                }

                if (arrROIs[i].Count == 1)
                {
                    objROI = new ROI("Pkg ROI", 2);
                    arrROIs[i].Add(objROI);
                }

                arrROIs[i][1].AttachImage(arrROIs[i][0]);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    m_smVisionInfo.g_arrLeadROIs[i][1].LoadROISetting(
     (int)Math.Round(m_smVisionInfo.g_arrLead3D[i].ref_pCornerPoint_Center.X -
     (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth / 2) -
       m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalX),
     (int)Math.Round(m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y -
     (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight / 2) -
      m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalY),
     (int)Math.Ceiling(m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth),
     (int)Math.Ceiling(m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight));
                else
                    m_smVisionInfo.g_arrLeadROIs[i][1].LoadROISetting(
    (int)Math.Round(m_smVisionInfo.g_arrLead3D[i].ref_objRectGauge4L.ref_pRectCenterPoint.X -
    (m_smVisionInfo.g_arrLead3D[i].GetResultMaxWidth_RectGauge4L(0) / 2) -
      m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalX),
    (int)Math.Round(m_smVisionInfo.g_arrLead3D[i].ref_objRectGauge4L.ref_pRectCenterPoint.Y -
    (m_smVisionInfo.g_arrLead3D[i].GetResultMaxHeight_RectGauge4L(0) / 2) -
     m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalY),
    (int)Math.Ceiling(m_smVisionInfo.g_arrLead3D[i].GetResultMaxWidth_RectGauge4L(0)),
    (int)Math.Ceiling(m_smVisionInfo.g_arrLead3D[i].GetResultMaxHeight_RectGauge4L(0)));

            }
        }
        private void UpdateGUI()
        {
            m_blnUpdateSelectedROISetting = true;

            switch (m_intSelectedTabPage)
            {
                case 0:
                    lbl_LeadThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intThresholdValue.ToString();
                    txt_MinArea.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intFilterMinArea.ToString();
                    txt_PkgToBaseTolerance_Top.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Top.ToString();
                    txt_PkgToBaseTolerance_Bottom.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Bottom.ToString();
                    txt_PkgToBaseTolerance_Left.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Left.ToString();
                    txt_PkgToBaseTolerance_Right.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Right.ToString();
                    txt_TipBuildAreaTolerance_Top.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Top.ToString();
                    txt_TipBuildAreaTolerance_Bottom.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Bottom.ToString();
                    txt_TipBuildAreaTolerance_Left.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Left.ToString();
                    txt_TipBuildAreaTolerance_Right.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Right.ToString();

                    txt_BaseOffset.Text = m_smVisionInfo.g_arrLead3D[0].ref_intBaseOffset.ToString();
                    txt_TipOffset.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipOffset.ToString();
                    txt_TipOffsetSide.Text = m_smVisionInfo.g_arrLead3D[1].ref_intTipOffset.ToString();

                    if (m_smVisionInfo.g_arrLead3D[0].ref_intLeadDirection == 0) // Horizontal
                    {
                        srmLabel63.Visible = false;
                        txt_PkgToBaseTolerance_Top.Visible = false;
                        srmLabel52.Visible = false;
                        txt_TipBuildAreaTolerance_Top.Visible = false;

                        srmLabel62.Visible = false;
                        txt_PkgToBaseTolerance_Bottom.Visible = false;
                        srmLabel25.Visible = false;
                        txt_TipBuildAreaTolerance_Bottom.Visible = false;
                    }
                    else
                    {
                        srmLabel60.Visible = false;
                        txt_PkgToBaseTolerance_Left.Visible = false;
                        srmLabel2.Visible = false;
                        txt_TipBuildAreaTolerance_Left.Visible = false;
                        srmLabel61.Visible = false;
                        txt_PkgToBaseTolerance_Right.Visible = false;
                        srmLabel3.Visible = false;
                        txt_TipBuildAreaTolerance_Right.Visible = false;
                    }

                    if (m_smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance)
                    {
                        group_TipBuildAreaTolerance.Visible = true;
                        group_PkgToBaseToleranceSetting.Visible = true;
                    }
                    else
                    {
                        group_TipBuildAreaTolerance.Visible = false;
                        group_PkgToBaseToleranceSetting.Visible = false;
                    }

                    txt_PkgStartPointFromEdge.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge.ToString();
                    txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight.ToString();
                    txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom.ToString();
                    txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft.ToString();

                    m_fStartPixelFromEdgePrev = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromEdge);
                    m_fStartPixelFromRightPrev = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromRight);
                    m_fStartPixelFromBottomPrev = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromBottom);
                    m_fStartPixelFromLeftPrev = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromLeft);

                    AddTrainPackageROI(m_smVisionInfo.g_arrLeadROIs);
                    break;
                case 1:
                    //lead3D package
                    lbl_BrightThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intBrightFieldLowThreshold.ToString();
                    lbl_DarkThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intDarkFieldLowThreshold.ToString();
                    lbl_PkgImage1LowThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage1LowSurfaceThreshold.ToString();
                    lbl_PkgImage1HighThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage1HighSurfaceThreshold.ToString();
                    lbl_PkgImage2LowThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2LowThreshold.ToString();
                    lbl_PkgImage2HighThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2HighThreshold.ToString();
                    lbl_MoldFlashThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage1MoldFlashThreshold.ToString();

                    txt_BrightMinArea.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intBrightFieldMinArea.ToString();
                    txt_DarkMinArea.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intDarkFieldMinArea.ToString();
                    txt_SurfaceMinArea.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fSurfaceMinArea.ToString();
                    txt_Image2SurfaceMinArea.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fImage2SurfaceMinArea.ToString();
                    txt_MoldFlashMinArea.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldFlashMinArea.ToString();
                    txt_VoidMinArea.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fVoidMinArea.ToString();
                    lbl_Void_Threshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2VoidThreshold.ToString();
                    lbl_LowCrackViewThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2LowCrackThreshold.ToString();
                    lbl_HighCrackViewThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intPkgImage2HighCrackThreshold.ToString();
                    txt_CrackMinArea.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fCrackMinArea.ToString();

                    txt_PkgStartPointFromEdge.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromEdge.ToString();
                    txt_PkgStartPixelFromRight.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromRight.ToString();
                    txt_PkgStartPixelFromBottom.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromBottom.ToString();
                    txt_PkgStartPixelFromLeft.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fPkgStartPixelFromLeft.ToString();

                    m_fStartPixelFromEdgePrev = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromEdge);
                    m_fStartPixelFromRightPrev = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromRight);
                    m_fStartPixelFromBottomPrev = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromBottom);
                    m_fStartPixelFromLeftPrev = (m_smVisionInfo.g_arrLead3D[0].ref_fPkgStartPixelFromLeft);

                    txt_ChipStartPointFromEdge.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromEdge.ToString();
                    txt_ChipStartPixelFromRight.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromRight.ToString();
                    txt_ChipStartPixelFromBottom.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromBottom.ToString();
                    txt_ChipStartPixelFromLeft.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fChipStartPixelFromLeft.ToString();

                    m_fStartPixelFromEdgePrev_Chipped = (m_smVisionInfo.g_arrLead3D[0].ref_fChipStartPixelFromEdge);
                    m_fStartPixelFromRightPrev_Chipped = (m_smVisionInfo.g_arrLead3D[0].ref_fChipStartPixelFromRight);
                    m_fStartPixelFromBottomPrev_Chipped = (m_smVisionInfo.g_arrLead3D[0].ref_fChipStartPixelFromBottom);
                    m_fStartPixelFromLeftPrev_Chipped = (m_smVisionInfo.g_arrLead3D[0].ref_fChipStartPixelFromLeft);

                    txt_MoldStartPointFromEdge.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromEdge.ToString();
                    txt_MoldStartPixelFromRight.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromRight.ToString();
                    txt_MoldStartPixelFromBottom.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromBottom.ToString();
                    txt_MoldStartPixelFromLeft.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_fMoldStartPixelFromLeft.ToString();

                    m_fStartPixelFromEdgePrev_Mold = (m_smVisionInfo.g_arrLead3D[0].ref_fMoldStartPixelFromEdge);
                    m_fStartPixelFromRightPrev_Mold = (m_smVisionInfo.g_arrLead3D[0].ref_fMoldStartPixelFromRight);
                    m_fStartPixelFromBottomPrev_Mold = (m_smVisionInfo.g_arrLead3D[0].ref_fMoldStartPixelFromBottom);
                    m_fStartPixelFromLeftPrev_Mold = (m_smVisionInfo.g_arrLead3D[0].ref_fMoldStartPixelFromLeft);

                    RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
                    RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
                    chk_SetToAll.Checked = Convert.ToBoolean(subkey1.GetValue("SetToAllEdges_ROI_LeadPackage", false));
                    break;
            }

            m_blnUpdateSelectedROISetting = false;
        }

        private void LoadLead3DSetting(string strPath)
        {
            //if (!tab_VisionControl.Controls.Contains(tp_Segment))
            //    return;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i == 0)
                {
                    m_smVisionInfo.g_arrLead3D[i].SetCalibrationData(
                                                  m_smVisionInfo.g_fCalibPixelX,
                                                  m_smVisionInfo.g_fCalibPixelY,
                                                  m_smVisionInfo.g_fCalibOffSetX,
                                                  m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[i].SetCalibrationData(
                                                  m_smVisionInfo.g_fCalibPixelZ,
                                                  m_smVisionInfo.g_fCalibPixelZ,
                                                  m_smVisionInfo.g_fCalibOffSetZ,
                                                  m_smVisionInfo.g_fCalibOffSetZ, m_smCustomizeInfo.g_intUnitDisplay);
                }
                // Load Lead Template Setting
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

                m_smVisionInfo.g_arrLead3D[i].LoadLead3D(strPath + "Template\\Template.xml", strSectionName);

                m_smVisionInfo.g_arrLead3D[i].LoadLeadTemplateImage(strPath + "Template\\", i);
                if (i == 0)
                    m_smVisionInfo.g_arrLead3D[i].LoadUnitPattern(strPath + "Template\\PatternMatcher0.mch");
            }
        }
      
        private void btn_Save_Click(object sender, EventArgs e)
        {
            // 2020 07 27 - CCENG: Not suppose to call this function. This function will redefine tolerance setting.
            //m_smVisionInfo.g_arrLead3D[0].DefineTolerance(false);

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                        m_smVisionInfo.g_strVisionFolderName + "\\";

            //m_smVisionInfo.g_intSelectedImage = 0;
            SaveLead3DSetting(strPath + "Lead3D\\");

            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            
            LoadLead3DSetting(strFolderPath + "Lead3D\\");

            m_smVisionInfo.AT_PR_AttachImagetoROI = true;

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            this.Close();
            this.Dispose();
        }

        private void SaveLead3DSetting(string strFolderPath)
        {
            
            
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
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

                //
                STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
                m_smVisionInfo.g_arrLead3D[i].SaveLead3D(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Other Setting", m_smProductionInfo.g_strLotID);
                
            }
        }

        private void SaveROISetting(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
            XmlParser objFile = new XmlParser(strFolderPath + "ROI.xml", false);

            ROI objROI;
            for (int t = 0; t < m_smVisionInfo.g_arrLeadROIs.Count; t++)
            {
                objFile.WriteSectionElement("Unit" + t, true);

                for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs[t].Count; j++)
                {
                    objROI = m_smVisionInfo.g_arrLeadROIs[t][j];
                    objFile.WriteElement1Value("ROI" + j, "");

                    objFile.WriteElement2Value("Name", objROI.ref_strROIName);
                    objFile.WriteElement2Value("Type", objROI.ref_intType);
                    objFile.WriteElement2Value("PositionX", objROI.ref_ROIPositionX);
                    objFile.WriteElement2Value("PositionY", objROI.ref_ROIPositionY);
                    objFile.WriteElement2Value("Width", objROI.ref_ROIWidth);
                    objFile.WriteElement2Value("Height", objROI.ref_ROIHeight);
                    objFile.WriteElement2Value("StartOffsetX", objROI.ref_intStartOffsetX);
                    objFile.WriteElement2Value("StartOffsetY", objROI.ref_intStartOffsetY);
                }
            }
            objFile.WriteEndElement();
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead3D ROI", m_smProductionInfo.g_strLotID);
            
        }

        private void btn_LineProfileGaugeSetting_Click(object sender, EventArgs e)
        {
            m_blnDragROIPrev = m_smVisionInfo.g_blnDragROI;

            m_smVisionInfo.g_blnDragROI = false;
            string strPath = m_smProductionInfo.g_strRecipePath +
                                m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\Lead3D\\PointGauge.xml";

            if (m_objLead3DLineProfileForm == null)
                m_objLead3DLineProfileForm = new Lead3DLineProfileForm(m_smVisionInfo, m_smVisionInfo.g_arrLead3D[0].ref_objPointGauge, strPath, m_smProductionInfo, 1);

            m_objLead3DLineProfileForm.Show();

            m_smVisionInfo.g_strSelectedPage = "Lead3DLineProfileGaugeSetting";
            this.Hide();
        }
        private void UpdateSelectedROISetting(int intSelectedROIIndex)
        {
            m_blnUpdateSelectedROISetting = true;

            lbl_LeadThreshold.Text = m_smVisionInfo.g_arrLead3D[intSelectedROIIndex].ref_intThresholdValue.ToString();
            txt_MinArea.Text = m_smVisionInfo.g_arrLead3D[intSelectedROIIndex].ref_intFilterMinArea.ToString();

            m_blnUpdateSelectedROISetting = false;
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

            if (m_smVisionInfo.g_blnUpdateSelectedROI)
            {
                m_smVisionInfo.g_blnUpdateSelectedROI = false;

                UpdateSelectedROISetting(m_smVisionInfo.g_intSelectedROI);
            }

            if (m_objLead3DLineProfileForm != null)
            {
                if (!m_objLead3DLineProfileForm.ref_blnShow)
                {
                    m_smVisionInfo.g_strSelectedPage = "Lead3DOtherSettingForm";
                    m_objLead3DLineProfileForm.Close();
                    m_objLead3DLineProfileForm.Dispose();
                    m_objLead3DLineProfileForm = null;
                    this.Show();

                    m_smVisionInfo.g_blnDragROI = m_blnDragROIPrev;
                }
            }
        }

        private void btn_LeadThreshold_Click(object sender, EventArgs e)
        {
            int intSelectedROI = 0;
            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                    intSelectedROI++;
                else
                    break;
            }

            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {
                if (!m_smVisionInfo.g_arrLead3D[j].ref_blnSelected)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                        m_smVisionInfo.g_arrLeadROIs[j][0].ClearDragHandle();
                }

                if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())   // 0: search ROI
                {
                    intSelectedROI = j;
                }
            }

            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intThresholdValue;

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[0].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[1].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[2].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[3].ref_intThresholdValue);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[4].ref_intThresholdValue);

            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0], true, true, arrrThreshold);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        // 2020-07-08 ZJYEOH : Set to all side ROI only
                        if (intSelectedROI != 0 && i == 0)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;
                        lbl_LeadThreshold.Text = m_smVisionInfo.g_arrLead3D[i].ref_intThresholdValue.ToString();
                        //m_smVisionInfo.g_arrLead3D[i].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[i][0]);

                        ////Clear Temp Blobs Features
                        //m_smVisionInfo.g_arrLead3D[i].ClearTempBlobsFeatures();

                        ////Set blobs features data into Temp Blobs Features
                        //m_smVisionInfo.g_arrLead3D[i].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrLeadROIs[i][0]);

                        ////Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                        //m_smVisionInfo.g_arrLead3D[i].CompareSelectedBlobs();
                    }
                    
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intThresholdValue = m_smVisionInfo.g_intThresholdValue;
                    lbl_LeadThreshold.Text = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intThresholdValue.ToString();
                    //m_smVisionInfo.g_arrLead3D[intSelectedROI].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                    ////Clear Temp Blobs Features
                    //m_smVisionInfo.g_arrLead3D[intSelectedROI].ClearTempBlobsFeatures();

                    ////Set blobs features data into Temp Blobs Features
                    //m_smVisionInfo.g_arrLead3D[intSelectedROI].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                    ////Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                    //m_smVisionInfo.g_arrLead3D[intSelectedROI].CompareSelectedBlobs();
                }
            }
            else
            {
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intThresholdValue;
                lbl_LeadThreshold.Text = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intThresholdValue.ToString();
                //m_smVisionInfo.g_arrLead3D[intSelectedROI].BuildOnlyLeadObjects(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                ////Clear Temp Blobs Features
                //m_smVisionInfo.g_arrLead3D[intSelectedROI].ClearTempBlobsFeatures();

                ////Set blobs features data into Temp Blobs Features
                //m_smVisionInfo.g_arrLead3D[intSelectedROI].SetBlobsFeaturesToTempArray(m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0]);

                ////Compare to find new selected blobs and replace new selected blobs data into m_arrBlobsFeatures
                //m_smVisionInfo.g_arrLead3D[intSelectedROI].CompareSelectedBlobs();
            }

            objThresholdForm.Dispose();
            m_smVisionInfo.g_blnViewObjectsBuilded = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            int intMinArea = Convert.ToInt32(txt_MinArea.Text);

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_intFilterMinArea = intMinArea;
            }
            
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void AttachImageToROI(List<List<ROI>> arrROIs, ImageDrawing objImage)
        {
            for (int i = 0; i < arrROIs.Count; i++)
            {
                //if (i != 0)
                //    break;
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
        private void Lead3DOtherSettingForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intViewInspectionSetting = 1;
            if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
            {
                m_smVisionInfo.AT_VM_OfflineTestAllLead3D = true;
                TriggerOfflineTest();
            }
            else
            {
                m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(0);
                switch (m_smVisionInfo.g_arrLead3D[0].ref_objRectGauge4L.GetGaugeImageMode(0))
                {
                    default:
                    case 0:
                        {
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_arrLead3D[0].ref_fImageGain);
                        }
                        break;
                    case 1:
                        {
                            m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage].AddGain(ref m_smVisionInfo.g_objPkgProcessImage, m_smVisionInfo.g_arrLead3D[0].ref_fImageGain);
                            m_smVisionInfo.g_objPkgProcessImage.AddPrewitt(ref m_smVisionInfo.g_objPackageImage);
                        }
                        break;
                }
                AttachImageToROI(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_objPackageImage);
                m_smVisionInfo.g_arrLead3D[0].ref_objRectGauge4L.SetEdgeROIPlacementLimit2(m_smVisionInfo.g_arrLeadROIs[0][0]);
                m_smVisionInfo.g_arrLead3D[0].ref_objRectGauge4L.SetGaugePlace_BasedOnEdgeROI();

                //m_smVisionInfo.g_blnViewPackageImage = true;
                bool blnGaugeResult;
            
                blnGaugeResult = m_smVisionInfo.g_arrLead3D[0].MeasureEdge_UsingRectGauge4L(m_smVisionInfo.g_objPackageImage, m_smVisionInfo.g_objWhiteImage);
               
                if (!blnGaugeResult)
                {
                    m_smVisionInfo.g_strErrorMessage = "*" + m_smVisionInfo.g_arrLead3D[0].ref_strErrorMessage;
                    m_smVisionInfo.VM_AT_UpdateErrorMessage = true;
                }
            }
            m_smVisionInfo.g_blnViewLeadInspection = false; 
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.g_blncboImageView = false;
        }

        private void Lead3DOtherSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.AT_VM_OfflineTestAllLead3D = false;
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Other Setting Form Closed", "Exit Lead3D Setting Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_blnViewPackageImage = false;
            m_smVisionInfo.g_blnViewRotatedImage = false;
            m_smVisionInfo.g_blnViewLead3DPkgToBaseDrawing = false;
            m_smVisionInfo.g_blnViewLeadTipBuildAreaDrawing = false;
            m_smVisionInfo.g_blncboImageView = true;
            m_smVisionInfo.g_blnViewROI = m_smVisionInfo.g_blnViewSearchROI = false;
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
            m_smVisionInfo.g_intSelectedSetting = 0;
            m_smVisionInfo.g_blnLeadInspected = false;
            m_smVisionInfo.g_intViewInspectionSetting = 0;
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
            
            LoadLead3DSetting(strFolderPath + "Lead3D\\");

            this.Close();
            this.Dispose();
        }

        private void txt_PkgToBaseTolerance_Top_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight / 2) - Convert.ToInt32(txt_PkgToBaseTolerance_Top.Text)) < m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY)
            {
                SRMMessageBox.Show("Package To Base Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_PkgToBaseTolerance_Top.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Top.ToString();
                return;
            }
            m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Top = Convert.ToInt32(txt_PkgToBaseTolerance_Top.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Bottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y + (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight / 2) + Convert.ToInt32(txt_PkgToBaseTolerance_Bottom.Text)) > (m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight))
            {
                SRMMessageBox.Show("Package To Base Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_PkgToBaseTolerance_Bottom.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Bottom.ToString();
                return;
            }

            m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Bottom = Convert.ToInt32(txt_PkgToBaseTolerance_Bottom.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Left_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth / 2) - Convert.ToInt32(txt_PkgToBaseTolerance_Left.Text)) < (m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX))
            {
                SRMMessageBox.Show("Package To Base Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_PkgToBaseTolerance_Left.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Left.ToString();
                return;
            }
            m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Left = Convert.ToInt32(txt_PkgToBaseTolerance_Left.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Right_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X + (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth / 2) + Convert.ToInt32(txt_PkgToBaseTolerance_Right.Text)) > (m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth))
            {
                SRMMessageBox.Show("Package To Base Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_PkgToBaseTolerance_Right.Text = m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Right.ToString();
                return;
            }
            m_smVisionInfo.g_arrLead3D[0].ref_intPkgToBaseTolerance_Right = Convert.ToInt32(txt_PkgToBaseTolerance_Right.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        
        private void txt_TipBuildAreaTolerance_Top_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y - (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight / 2) - Convert.ToInt32(txt_TipBuildAreaTolerance_Top.Text)) < m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY)
            {
                SRMMessageBox.Show("Tip Build Area Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_TipBuildAreaTolerance_Top.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Top.ToString();
                return;
            }

            m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Top = Convert.ToInt32(txt_TipBuildAreaTolerance_Top.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipBuildAreaTolerance_Bottom_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.Y + (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeight / 2) + Convert.ToInt32(txt_TipBuildAreaTolerance_Bottom.Text)) > (m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIHeight))
            {
                SRMMessageBox.Show("Tip Build Area Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_TipBuildAreaTolerance_Bottom.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Bottom.ToString();
                return;
            }

            m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Bottom = Convert.ToInt32(txt_TipBuildAreaTolerance_Bottom.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipBuildAreaTolerance_Left_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X - (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth / 2) - Convert.ToInt32(txt_TipBuildAreaTolerance_Left.Text)) < (m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX))
            {
                SRMMessageBox.Show("Tip Build Area Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_TipBuildAreaTolerance_Left.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Left.ToString();
                return;
            }

            m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Left = Convert.ToInt32(txt_TipBuildAreaTolerance_Left.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipBuildAreaTolerance_Right_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if ((m_smVisionInfo.g_arrLead3D[0].ref_pCornerPoint_Center.X + (m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidth / 2) + Convert.ToInt32(txt_TipBuildAreaTolerance_Right.Text)) > (m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[0][0].ref_ROIWidth))
            {
                SRMMessageBox.Show("Tip Build Area Tolerance cannot exceed the search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                txt_TipBuildAreaTolerance_Right.Text = m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Right.ToString();
                return;
            }

            m_smVisionInfo.g_arrLead3D[0].ref_intTipBuildAreaTolerance_Right = Convert.ToInt32(txt_TipBuildAreaTolerance_Right.Text);
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Enter(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewLead3DPkgToBaseDrawing = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgToBaseTolerance_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewLead3DPkgToBaseDrawing = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipBuildAreaTolerance_Enter(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewLeadTipBuildAreaDrawing = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipBuildAreaTolerance_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_blnViewLeadTipBuildAreaDrawing = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_BrightMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_BrightMinArea.Text == "")
                return;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {

                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }


            m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intBrightFieldMinArea = Convert.ToInt32(txt_BrightMinArea.Text);


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_DarkMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_DarkMinArea.Text == "")
                return;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {

                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }


            m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intDarkFieldMinArea = Convert.ToInt32(txt_DarkMinArea.Text);


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_BrightThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = false;

            int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            //{

            //    if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[intSelectedROI].GetGrabImageIndex(1);       
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intBrightFieldLowThreshold;

            if (m_smVisionInfo.g_arrLeadROIs[intSelectedROI].Count == 0)
                return;

            if (m_smVisionInfo.g_blnViewRotatedImage)
                ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[0].ref_intBrightFieldLowThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[1].ref_intBrightFieldLowThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[2].ref_intBrightFieldLowThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[3].ref_intBrightFieldLowThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[4].ref_intBrightFieldLowThreshold);

            bool blnWantSetToAllROICheckBox = false;// (m_smVisionInfo.g_arrLeadROIs.Count > 1);
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold); //[0]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intBrightFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intBrightFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                }
                lbl_BrightThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intBrightFieldLowThreshold.ToString();
            }
            else
            {

                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intBrightFieldLowThreshold;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_DarkThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = false;

            int intSelectedROI = 0;
            //for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            //{

            //    if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
            //    {
            //        if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
            //        {
            //            intSelectedROI = j;
            //            break;
            //        }
            //    }
            //}

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[intSelectedROI].GetGrabImageIndex(2);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intDarkFieldLowThreshold;
            m_smVisionInfo.g_fThresholdGainValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_fDarkFieldImageGain;

            if (m_smVisionInfo.g_arrLeadROIs[intSelectedROI].Count == 0)
                return;

            if (m_smVisionInfo.g_blnViewRotatedImage)
                ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);


            bool blnWantSetToAllROICheckBox = false;// (m_smVisionInfo.g_arrLeadROIs.Count > 1);
            ThresholdWithGainForm objThresholdForm = new ThresholdWithGainForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox); //[0]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intDarkFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                        m_smVisionInfo.g_arrLead3D[i].ref_fDarkFieldImageGain = m_smVisionInfo.g_fThresholdGainValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intDarkFieldLowThreshold = m_smVisionInfo.g_intThresholdValue;
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_fDarkFieldImageGain = m_smVisionInfo.g_fThresholdGainValue;
                }
                lbl_DarkThreshold.Text = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_intDarkFieldLowThreshold.ToString();
            }
            else
            {

                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intDarkFieldLowThreshold;
                m_smVisionInfo.g_fThresholdGainValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_fDarkFieldImageGain;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_SurfaceMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (m_smVisionInfo.g_arrLead3D.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                    m_smVisionInfo.g_arrLead3D[i].ref_fSurfaceMinArea = Convert.ToSingle(txt_SurfaceMinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_PackageSurfaceThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = false;

            int intLeadSelectedIndex = 0;

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(1);

            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1LowSurfaceThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1HighSurfaceThreshold;

            if (m_smVisionInfo.g_blnViewRotatedImage)
                ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            bool blnWantSetToAllROICheckBox = false;//(m_smVisionInfo.g_arrLeadROIs.Count > 1);
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intLeadSelectedIndex][0], false, blnWantSetToAllROICheckBox);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1LowSurfaceThreshold = intLowThreshold;
                m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1HighSurfaceThreshold = intHighThreshold;
            }
            else
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intLeadSelectedIndex, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage1LowSurfaceThreshold = m_smVisionInfo.g_intLowThresholdValue;
                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage1HighSurfaceThreshold = m_smVisionInfo.g_intHighThresholdValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1LowSurfaceThreshold = m_smVisionInfo.g_intLowThresholdValue;
                    m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1HighSurfaceThreshold = m_smVisionInfo.g_intHighThresholdValue;
                }
                lbl_PkgImage1LowThreshold.Text = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1LowSurfaceThreshold.ToString();
                lbl_PkgImage1HighThreshold.Text = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage1HighSurfaceThreshold.ToString();
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Image2SurfaceMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (m_smVisionInfo.g_arrLead3D.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                    m_smVisionInfo.g_arrLead3D[i].ref_fImage2SurfaceMinArea = Convert.ToSingle(txt_Image2SurfaceMinArea.Text);
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_Image2Threshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = false;

            int intLeadSelectedIndex = 0;

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(2);

            int intLowThreshold = m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2LowThreshold;
            int intHighThreshold = m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2HighThreshold;

            if (m_smVisionInfo.g_blnViewRotatedImage)
                ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            bool blnWantSetToAllROICheckBox = false;//(m_smVisionInfo.g_arrLeadROIs.Count > 1);
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intLeadSelectedIndex][0], false, blnWantSetToAllROICheckBox); //[1]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.Cancel)
            {
                m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2LowThreshold = intLowThreshold;
                m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2HighThreshold = intHighThreshold;
            }
            else
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intLeadSelectedIndex, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2HighThreshold = m_smVisionInfo.g_intHighThresholdValue;
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2LowThreshold = m_smVisionInfo.g_intLowThresholdValue;
                    m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2HighThreshold = m_smVisionInfo.g_intHighThresholdValue;
                }
                lbl_PkgImage2LowThreshold.Text = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2LowThreshold.ToString();
                lbl_PkgImage2HighThreshold.Text = m_smVisionInfo.g_arrLead3D[intLeadSelectedIndex].ref_intPkgImage2HighThreshold.ToString();
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldFlashMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (m_smVisionInfo.g_arrLead3D.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                {
                    m_smVisionInfo.g_arrLead3D[i].ref_fMoldFlashMinArea = Convert.ToSingle(txt_MoldFlashMinArea.Text);
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
                    {
                        m_smVisionInfo.g_arrLead3D[i].ref_fSurfaceMinArea = Convert.ToSingle(txt_MoldFlashMinArea.Text);
                    }
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_MoldFlashThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = false;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {

                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }

            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[intSelectedROI].GetGrabImageIndex(1);
            m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage1MoldFlashThreshold;

            if (m_smVisionInfo.g_arrLeadROIs[intSelectedROI].Count == 0)
                return;
            if (m_smVisionInfo.g_blnViewRotatedImage)
                ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            List<int> arrrThreshold = new List<int>();
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[0].ref_intPkgImage1MoldFlashThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[1].ref_intPkgImage1MoldFlashThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[2].ref_intPkgImage1MoldFlashThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[3].ref_intPkgImage1MoldFlashThreshold);
            if (m_smVisionInfo.g_arrLead3D.Length > 0)
                arrrThreshold.Add(m_smVisionInfo.g_arrLead3D[4].ref_intPkgImage1MoldFlashThreshold);

            bool blnWantSetToAllROICheckBox = false;//(m_smVisionInfo.g_arrLeadROIs.Count > 1);
            ThresholdForm objThresholdForm = new ThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0], true, blnWantSetToAllROICheckBox, arrrThreshold); //[0]
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(769, 310);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage1MoldFlashThreshold = m_smVisionInfo.g_intThresholdValue;
                        if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage1LowSurfaceThreshold = m_smVisionInfo.g_intThresholdValue;
                            m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage1HighSurfaceThreshold = 255;
                        }
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage1MoldFlashThreshold = m_smVisionInfo.g_intThresholdValue;
                    if (!m_smVisionInfo.g_blnWantUseDetailThreshold_LeadPackage)
                    {
                        m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage1LowSurfaceThreshold = m_smVisionInfo.g_intThresholdValue;
                        m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage1HighSurfaceThreshold = 255;
                    }
                }
                lbl_MoldFlashThreshold.Text = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage1MoldFlashThreshold.ToString();
            }
            else
            {
                m_smVisionInfo.g_intThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage1MoldFlashThreshold;
            }
            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CrackMinArea_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (txt_CrackMinArea.Text == "")
                return;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {

                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }


            m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_fCrackMinArea = Convert.ToInt32(txt_CrackMinArea.Text);


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void btn_CrackThreshold_Click(object sender, EventArgs e)
        {
            // Clear Result drawing
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                m_smVisionInfo.g_arrLead3D[i].ref_blnViewLeadResultDrawing = false;

            int intSelectedROI = 0;
            for (int j = 0; j < m_smVisionInfo.g_arrLeadROIs.Count; j++)
            {

                if (m_smVisionInfo.g_arrLeadROIs[j].Count > 0)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[j][0].GetROIHandle())
                    {
                        intSelectedROI = j;
                        break;
                    }
                }
            }


            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[intSelectedROI].GetGrabImageIndex(2);
            if (m_smVisionInfo.g_arrLeadROIs[intSelectedROI].Count == 0)
                return;

            m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2LowCrackThreshold;
            m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2HighCrackThreshold;

            if (m_smVisionInfo.g_blnViewRotatedImage)
                ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrRotatedImages[m_smVisionInfo.g_intSelectedImage]);
            else
                ROI.AttachROIToImage(m_smVisionInfo.g_arrLeadROIs, m_smVisionInfo.g_arrImages[m_smVisionInfo.g_intSelectedImage]);

            bool blnWantSetToAllROICheckBox = false;// (m_smVisionInfo.g_arrLeadROIs.Count > 1);
            DoubleThresholdForm objThresholdForm = new DoubleThresholdForm(m_smVisionInfo, m_smVisionInfo.g_arrLeadROIs[intSelectedROI][0], false, blnWantSetToAllROICheckBox);
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objThresholdForm.Location = new Point(resolution.Width - objThresholdForm.Size.Width, resolution.Height - objThresholdForm.Size.Height);
            //objThresholdForm.Location = new Point(670, 200);
            if (objThresholdForm.ShowDialog() == DialogResult.OK)
            {
                if (objThresholdForm.ref_blnSetToAllROI)
                {
                    // 2019 04 13-CCENG: Center ROI threshold setting allow set to other sides ROI. Side ROI threshold setting not allow set to center ROI.
                    for (int i = Math.Min(intSelectedROI, 1); i < m_smVisionInfo.g_arrLead3D.Length; i++)
                    {
                        if (!m_smVisionInfo.g_arrLead3D[i].ref_blnSelected)
                            continue;

                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2LowCrackThreshold = m_smVisionInfo.g_intLowThresholdValue;
                        m_smVisionInfo.g_arrLead3D[i].ref_intPkgImage2HighCrackThreshold = m_smVisionInfo.g_intHighThresholdValue;

                        lbl_LowCrackViewThreshold.Text = m_smVisionInfo.g_intLowThresholdValue.ToString();
                        lbl_HighCrackViewThreshold.Text = m_smVisionInfo.g_intHighThresholdValue.ToString();
                    }
                }
                else
                {
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2LowCrackThreshold = m_smVisionInfo.g_intLowThresholdValue;
                    m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2HighCrackThreshold = m_smVisionInfo.g_intHighThresholdValue;

                    lbl_LowCrackViewThreshold.Text = m_smVisionInfo.g_intLowThresholdValue.ToString();
                    lbl_HighCrackViewThreshold.Text = m_smVisionInfo.g_intHighThresholdValue.ToString();
                }
            }
            else
            {
                m_smVisionInfo.g_intLowThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2LowCrackThreshold;
                m_smVisionInfo.g_intHighThresholdValue = m_smVisionInfo.g_arrLead3D[intSelectedROI].ref_intPkgImage2HighCrackThreshold;

            }


            objThresholdForm.Dispose();

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetToAll_Click(object sender, EventArgs e)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("SetToAllEdges_ROI_LeadPackage", chk_SetToAll.Checked);
        }

        private void txt_PkgStartPointFromEdge_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_PkgStartPointFromEdge.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_PkgStartPointFromEdge.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;

            float fUnitWidth;
            float fUnitHeight;

            // int intEdgeDistance = (int)Math.Round(Convert.ToSingle(fStartPixelFromEdge) * m_smVisionInfo.g_fCalibPixelY, 0, MidpointRounding.AwayFromZero);


            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                //if (fUnitWidth < fUnitHeight)
                //{
                //    if (fStartPixelFromEdge >= (fUnitWidth / 4))
                //    {
                //        SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                //        txt_StartPixelFromEdge.Text = m_fStartPixelFromEdgePrev.ToString("F4");
                //        return;
                //    }
                //}
                //else
                //{
                if ((i == 0) && fStartPixelFromEdge >= m_smVisionInfo.g_arrLead3D[i].Get2DYinPixel(fUnitHeight / 4))//* m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPointFromEdge.Text = m_fStartPixelFromEdgePrev.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                //}
                if (chk_SetToAll.Checked)
                {
                    txt_PkgStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                    txt_PkgStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                    txt_PkgStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;
                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromEdge = fStartPixelFromEdge;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromBottom = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromLeft = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromRight = fStartPixelFromEdge;
                        }
                        break;

                }


                m_smVisionInfo.g_arrLeadROIs[i][1].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);

            }

            m_fStartPixelFromEdgePrev = fStartPixelFromEdge;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromRightPrev = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev = fStartPixelFromEdge;
            }


            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromRight = 0;
            if (!float.TryParse(txt_PkgStartPixelFromRight.Text, out fStartPixelFromRight))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromRight.Text == "" || fStartPixelFromRight < 0)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                // if (fUnitWidth < fUnitHeight)
                // {
                if ((i == 0) && fStartPixelFromRight >= m_smVisionInfo.g_arrLead3D[i].Get2DXinPixel(fUnitWidth / 4))// * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromRight.Text = m_fStartPixelFromEdgePrev.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                //  }

                if (chk_SetToAll.Checked)
                {
                    txt_PkgStartPointFromEdge.Text = fStartPixelFromRight.ToString();
                    txt_PkgStartPixelFromBottom.Text = fStartPixelFromRight.ToString();
                    txt_PkgStartPixelFromLeft.Text = fStartPixelFromRight.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;
                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromRight = fStartPixelFromRight;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromBottom = fStartPixelFromRight;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromLeft = fStartPixelFromRight;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromEdge = fStartPixelFromRight;
                        }
                        break;
                }

                m_smVisionInfo.g_arrLeadROIs[i][1].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);
            }



            m_fStartPixelFromRightPrev = fStartPixelFromRight;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromEdgePrev = fStartPixelFromRight;
                m_fStartPixelFromBottomPrev = fStartPixelFromRight;
                m_fStartPixelFromLeftPrev = fStartPixelFromRight;
            }


            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromBottom = 0;
            if (!float.TryParse(txt_PkgStartPixelFromBottom.Text, out fStartPixelFromBottom))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromBottom.Text == "" || fStartPixelFromBottom < 0)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if ((i == 0 || i == 1 || i == 3) && fStartPixelFromBottom >= m_smVisionInfo.g_arrLead3D[i].Get2DYinPixel(fUnitHeight / 4))// * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom.Text = m_fStartPixelFromEdgePrev.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }
                else if ((i == 2 || i == 4) && fStartPixelFromBottom >= m_smVisionInfo.g_arrLead3D[i].Get2DXinPixel(fUnitWidth / 4))// * m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromBottom.Text = m_fStartPixelFromEdgePrev.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_PkgStartPointFromEdge.Text = fStartPixelFromBottom.ToString();
                    txt_PkgStartPixelFromRight.Text = fStartPixelFromBottom.ToString();
                    txt_PkgStartPixelFromLeft.Text = fStartPixelFromBottom.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromBottom = fStartPixelFromBottom;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromEdge = fStartPixelFromBottom;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromLeft = fStartPixelFromBottom;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromRight = fStartPixelFromBottom;
                        }
                        break;

                }

                m_smVisionInfo.g_arrLeadROIs[i][1].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);
            }



            m_fStartPixelFromBottomPrev = fStartPixelFromBottom;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromRightPrev = fStartPixelFromBottom;
                m_fStartPixelFromEdgePrev = fStartPixelFromBottom;
                m_fStartPixelFromLeftPrev = fStartPixelFromBottom;
            }

            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromLeft = 0;
            if (!float.TryParse(txt_PkgStartPixelFromLeft.Text, out fStartPixelFromLeft))
                return;

            if (!m_blnInitDone || txt_PkgStartPixelFromLeft.Text == "" || fStartPixelFromLeft < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;

            float fUnitWidth;
            float fUnitHeight;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0) && fStartPixelFromLeft >= m_smVisionInfo.g_arrLead3D[i].Get2DXinPixel(fUnitWidth / 4) )//* m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_PkgStartPixelFromLeft.Text = m_fStartPixelFromEdgePrev.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_PkgStartPointFromEdge.Text = fStartPixelFromLeft.ToString();
                    txt_PkgStartPixelFromRight.Text = fStartPixelFromLeft.ToString();
                    txt_PkgStartPixelFromBottom.Text = fStartPixelFromLeft.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromLeft = fStartPixelFromLeft;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromEdge = fStartPixelFromLeft;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromBottom = fStartPixelFromLeft;
                            m_smVisionInfo.g_arrLead3D[i].ref_fPkgStartPixelFromRight = fStartPixelFromLeft;
                        }
                        break;

                }

                m_smVisionInfo.g_arrLeadROIs[i][1].AttachImage(m_smVisionInfo.g_arrLeadROIs[i][0]);
            }



            m_fStartPixelFromLeftPrev = fStartPixelFromLeft;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromRightPrev = fStartPixelFromLeft;
                m_fStartPixelFromEdgePrev = fStartPixelFromLeft;
                m_fStartPixelFromBottomPrev = fStartPixelFromLeft;
            }

            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPointFromEdge_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPointFromEdge.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_ChipStartPointFromEdge.Text == "" || fStartPixelFromEdge < 0)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;

            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0) && fStartPixelFromEdge >= m_smVisionInfo.g_arrLead3D[i].Get2DYinPixel(fUnitHeight / 4))// * m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPointFromEdge.Text = m_fStartPixelFromEdgePrev_Chipped.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }


                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                    txt_ChipStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                    txt_ChipStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromEdge = fStartPixelFromEdge;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromBottom = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromLeft = fStartPixelFromEdge;
                        }
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromLeft = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromBottom = fStartPixelFromEdge;
                        }
                        break;

                }
            }



            m_fStartPixelFromEdgePrev_Chipped = fStartPixelFromEdge;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromRightPrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev_Chipped = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromRight.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_ChipStartPixelFromRight.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0) && fStartPixelFromEdge >= m_smVisionInfo.g_arrLead3D[i].Get2DXinPixel(fUnitWidth / 4) )//* m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromRight.Text = m_fStartPixelFromRightPrev_Chipped.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPointFromEdge.Text = fStartPixelFromEdge.ToString();
                    txt_ChipStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                    txt_ChipStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromRight = fStartPixelFromEdge;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromEdge = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromBottom = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromLeft = fStartPixelFromEdge;
                        }
                        break;

                }
            }

            m_fStartPixelFromRightPrev_Chipped = fStartPixelFromEdge;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromEdgePrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev_Chipped = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromBottom.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_ChipStartPixelFromBottom.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0) && fStartPixelFromEdge >= m_smVisionInfo.g_arrLead3D[i].Get2DYinPixel(fUnitHeight / 4) )//* m_smVisionInfo.g_fCalibPixelY)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromBottom.Text = m_fStartPixelFromBottomPrev_Chipped.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPointFromEdge.Text = fStartPixelFromEdge.ToString();
                    txt_ChipStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                    txt_ChipStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                }
            }
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromBottom = fStartPixelFromEdge;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromEdge = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromLeft = fStartPixelFromEdge;
                        }
                        break;

                }
            }

            m_fStartPixelFromBottomPrev_Chipped = fStartPixelFromEdge;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromEdgePrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromRightPrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev_Chipped = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_ChipStartPixelFromLeft.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_ChipStartPixelFromLeft.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if ((i == 0) && fStartPixelFromEdge >= m_smVisionInfo.g_arrLead3D[i].Get2DXinPixel(fUnitWidth / 4) )//* m_smVisionInfo.g_fCalibPixelX)
                {
                    SRMMessageBox.Show("Edge tolerance can only goto quarter of unit size.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    txt_ChipStartPixelFromLeft.Text = m_fStartPixelFromLeftPrev_Chipped.ToString();
                    m_blnUpdateSelectedROISetting = false;
                    return;
                }

                if (chk_SetToAll.Checked)
                {
                    txt_ChipStartPointFromEdge.Text = fStartPixelFromEdge.ToString();
                    txt_ChipStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                    txt_ChipStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromLeft = fStartPixelFromEdge;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromEdge = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fChipStartPixelFromBottom = fStartPixelFromEdge;
                        }
                        break;

                }
            }


            m_fStartPixelFromLeftPrev_Chipped = fStartPixelFromEdge;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromEdgePrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromRightPrev_Chipped = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev_Chipped = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPointFromEdge_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_MoldStartPointFromEdge.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_MoldStartPointFromEdge.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;
                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll.Checked)
                {

                    if ((i == 0) && (m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalY - fStartPixelFromEdge <= m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPointFromEdge.Text = m_fStartPixelFromEdgePrev_Mold.ToString();
                        txt_MoldStartPixelFromRight.Text = m_fStartPixelFromRightPrev_Mold.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_fStartPixelFromLeftPrev_Mold.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_fStartPixelFromBottomPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }

                }
                else
                {

                    if ((i == 0) && (m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalY - fStartPixelFromEdge <= m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalY))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPointFromEdge.Text = m_fStartPixelFromEdgePrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }

                }
                if (chk_SetToAll.Checked)
                {
                    txt_MoldStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                    txt_MoldStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                    txt_MoldStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromEdge = fStartPixelFromEdge;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromBottom = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromLeft = fStartPixelFromEdge;
                        }
                        break;

                }
            }



            m_fStartPixelFromEdgePrev_Mold = fStartPixelFromEdge;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromRightPrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev_Mold = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromRight_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_MoldStartPixelFromRight.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_MoldStartPixelFromRight.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll.Checked)
                {

                    if ((i == 0) && ((m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROIWidth) + fStartPixelFromEdge >= (m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIWidth)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPointFromEdge.Text = m_fStartPixelFromEdgePrev_Mold.ToString();
                        txt_MoldStartPixelFromRight.Text = m_fStartPixelFromRightPrev_Mold.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_fStartPixelFromLeftPrev_Mold.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_fStartPixelFromBottomPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }

                }
                else
                {

                    if ((i == 0) && (m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROIWidth) + fStartPixelFromEdge >= (m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalX + m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIWidth))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromRight.Text = m_fStartPixelFromRightPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }

                }

                if (chk_SetToAll.Checked)
                {
                    txt_MoldStartPointFromEdge.Text = fStartPixelFromEdge.ToString();
                    txt_MoldStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                    txt_MoldStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromRight = fStartPixelFromEdge;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromEdge = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromBottom = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromLeft = fStartPixelFromEdge;
                        }
                        break;

                }
            }

            m_fStartPixelFromRightPrev_Mold = fStartPixelFromEdge;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromEdgePrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev_Mold = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromBottom_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_MoldStartPixelFromBottom.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_MoldStartPixelFromBottom.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll.Checked)
                {

                    if ((i == 0) && ((m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROIHeight) + fStartPixelFromEdge >= (m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIHeight)))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPointFromEdge.Text = m_fStartPixelFromEdgePrev_Mold.ToString();
                        txt_MoldStartPixelFromRight.Text = m_fStartPixelFromRightPrev_Mold.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_fStartPixelFromLeftPrev_Mold.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_fStartPixelFromBottomPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }

                }
                else
                {

                    if ((i == 0) && (m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROIHeight) + fStartPixelFromEdge >= (m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalY + m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROIHeight))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromBottom.Text = m_fStartPixelFromBottomPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }

                }
                if (chk_SetToAll.Checked)
                {
                    txt_MoldStartPointFromEdge.Text = fStartPixelFromEdge.ToString();
                    txt_MoldStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                    txt_MoldStartPixelFromLeft.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromBottom = fStartPixelFromEdge;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromEdge = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromLeft = fStartPixelFromEdge;
                        }
                        break;

                }
            }



            m_fStartPixelFromBottomPrev_Mold = fStartPixelFromEdge;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromEdgePrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromRightPrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromLeftPrev_Mold = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPixelFromLeft_TextChanged(object sender, EventArgs e)
        {
            float fStartPixelFromEdge = 0;
            if (!float.TryParse(txt_MoldStartPixelFromLeft.Text, out fStartPixelFromEdge))
                return;

            if (!m_blnInitDone || txt_MoldStartPixelFromLeft.Text == "" || fStartPixelFromEdge < 0)
                return;
            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;
            float fUnitWidth;
            float fUnitHeight;

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                else
                    fUnitWidth = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultDownWidth_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                    fUnitHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                else
                    fUnitHeight = (float)Math.Round((m_smVisionInfo.g_arrLead3D[i].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[i].GetResultRightHeight_RectGauge4L(1)) / 2, 4, MidpointRounding.AwayFromZero);


                if (chk_SetToAll.Checked)
                {

                    if ((i == 0) && (m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalX - fStartPixelFromEdge <= m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPointFromEdge.Text = m_fStartPixelFromEdgePrev_Mold.ToString();
                        txt_MoldStartPixelFromRight.Text = m_fStartPixelFromRightPrev_Mold.ToString();
                        txt_MoldStartPixelFromLeft.Text = m_fStartPixelFromLeftPrev_Mold.ToString();
                        txt_MoldStartPixelFromBottom.Text = m_fStartPixelFromBottomPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }

                }
                else
                {

                    if ((i == 0) && (m_smVisionInfo.g_arrLeadROIs[i][1].ref_ROITotalX - fStartPixelFromEdge <= m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalX))
                    {
                        SRMMessageBox.Show("Edge tolerance cannot goes beyond Search ROI.", "Incorrect Setting", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        txt_MoldStartPixelFromLeft.Text = m_fStartPixelFromLeftPrev_Mold.ToString();
                        m_blnUpdateSelectedROISetting = false;
                        return;
                    }

                }
                if (chk_SetToAll.Checked)
                {
                    txt_MoldStartPointFromEdge.Text = fStartPixelFromEdge.ToString();
                    txt_MoldStartPixelFromRight.Text = fStartPixelFromEdge.ToString();
                    txt_MoldStartPixelFromBottom.Text = fStartPixelFromEdge.ToString();
                }
            }

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i != m_smVisionInfo.g_intSelectedROI)
                    continue;

                switch (i)
                {
                    case 0: // Center
                        m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromLeft = fStartPixelFromEdge;
                        if (chk_SetToAll.Checked)
                        {
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromEdge = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromRight = fStartPixelFromEdge;
                            m_smVisionInfo.g_arrLead3D[i].ref_fMoldStartPixelFromBottom = fStartPixelFromEdge;
                        }
                        break;

                }
            }

            m_fStartPixelFromLeftPrev_Mold = fStartPixelFromEdge;
            if (chk_SetToAll.Checked)
            {
                m_fStartPixelFromEdgePrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromRightPrev_Mold = fStartPixelFromEdge;
                m_fStartPixelFromBottomPrev_Mold = fStartPixelFromEdge;
            }
            m_blnUpdateSelectedROISetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void DefineMostSelectedImageNo()
        {
            int[] intCountImage = { 0, 0, 0, 0, 0 };

            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if (i > 0 && !m_smVisionInfo.g_blnCheck4Sides)
                    break;

                int[] arrPackageSizeImageIndex = ImageDrawing.GetArrayImageIndex(m_smVisionInfo.g_arrLead3D[i].GetEdgeImageViewNo(), m_smVisionInfo.g_intVisionIndex);

                for (int j = 0; j < 4; j++)
                {
                    switch (arrPackageSizeImageIndex[j])
                    {
                        case 0:
                            intCountImage[0]++;
                            break;
                        case 1:
                            intCountImage[1]++;
                            break;
                        case 2:
                            intCountImage[2]++;
                            break;
                        case 3:
                            intCountImage[3]++;
                            break;
                        case 4:
                            intCountImage[4]++;
                            break;
                    }
                }
            }

            int intMostOccurValue = Math.Max(Math.Max(Math.Max(intCountImage[0], intCountImage[1]), Math.Max(intCountImage[2], intCountImage[3])), intCountImage[4]);
            m_smVisionInfo.g_intSelectedImage = Array.IndexOf(intCountImage, intMostOccurValue);
        }
        private void txt_PkgStartPoint_Enter(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            AddTrainPackageROI(m_smVisionInfo.g_arrLeadROIs);
            if (m_intSelectedTabPage == 0)
                m_smVisionInfo.g_intSelectedImage = 0;// m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(0);
            else
                DefineMostSelectedImageNo();
            m_smVisionInfo.g_blnViewLeadPkgStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_PkgStartPoint_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            m_smVisionInfo.g_blnViewLeadPkgStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPoint_Enter(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(1);
            m_smVisionInfo.g_blnViewLeadChipStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ChipStartPoint_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            m_smVisionInfo.g_blnViewLeadChipStartPixelFromEdge = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldStartPoint_Enter(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            m_smVisionInfo.g_intSelectedImage = m_smVisionInfo.g_arrLead3D[0].GetGrabImageIndex(1);
            m_smVisionInfo.g_blnViewLeadMoldStartPixelFromEdge = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void txt_MoldStartPoint_Leave(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            m_smVisionInfo.g_blnViewLeadMoldStartPixelFromEdge = false;
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

            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                    m_smVisionInfo.g_arrLeadROIs[i][0].AttachImage(m_smVisionInfo.g_arrImages[0]);
            }

            m_smVisionInfo.AT_VM_ManualTestMode = true;

            m_smVisionInfo.PR_MN_TestDone = false;
            m_smVisionInfo.MN_PR_StartTest = true;
        }

        private void txt_BaseOffset_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_intBaseOffset = Convert.ToInt32(txt_BaseOffset.Text);
            }
            TriggerOfflineTest();
            //if (BuildObjects())
            //{
            //    BuildLead();
            //}

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipOffset_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_arrLead3D[0].ref_intTipOffset = Convert.ToInt32(txt_TipOffset.Text);
            TriggerOfflineTest();
            //if (BuildObjects())
            //{
            //    BuildLead();
            //}

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_TipOffsetSide_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            // Scan side ROI only
            for (int i = 1; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
            {
                m_smVisionInfo.g_arrLead3D[i].ref_intTipOffset = Convert.ToInt32(txt_TipOffsetSide.Text);
            }
            TriggerOfflineTest();
            ////if (BuildObjects())
            //{
            //    BuildLead();
            //}

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_BaseOffset_Enter(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewLeadTipBasePoint = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_BaseOffset_Leave(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewLeadTipBasePoint = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_SelectROI_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;
            
            m_smVisionInfo.g_intSelectedROI = cbo_SelectROI.SelectedIndex;
            m_smVisionInfo.g_intSelectedROIMask = 0x01 << m_smVisionInfo.g_intSelectedROI;
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                if ((m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
                {
                    if (i < m_smVisionInfo.g_arrLeadROIs.Count)
                    {
                        if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                        {
                            m_smVisionInfo.g_arrLeadROIs[i][0].VerifyROIArea(m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterX, m_smVisionInfo.g_arrLeadROIs[i][0].ref_ROITotalCenterY);
                        }
                    }
                }
                else
                {
                    if (i < m_smVisionInfo.g_arrLeadROIs.Count)
                    {
                        if (m_smVisionInfo.g_arrLeadROIs[i].Count > 0)
                        {
                            m_smVisionInfo.g_arrLeadROIs[i][0].ClearDragHandle();
                        }
                    }
                }

            }

            m_smVisionInfo.g_blnUpdateSelectedROI = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
    }
}
