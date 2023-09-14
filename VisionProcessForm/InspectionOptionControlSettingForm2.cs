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


namespace VisionProcessForm
{
    public partial class InspectionOptionControlSettingForm2 : Form
    {
        #region Member Variables

        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private int m_intVisionType = 0;    // Mask Bit 1:Orient, 2:Mark
        private string m_strSelectedRecipe;
        private bool m_blnWantSet1ToAll = false;
        private long m_intOptionControlMaskPrev = 0;
        private long m_intPkgOptionControlMaskPrev = 0;
        private int m_intPkgOptionControlMask2Prev = 0;
        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        #endregion

        public InspectionOptionControlSettingForm2(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intVisionType)
        {
            InitializeComponent();
            tab_VisionControl.Multiline = true; // 2021-07-28 ZJYEOH : Set here so that the form GUI wont show messy when editing
            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
            m_intVisionType = intVisionType;

            m_intOptionControlMaskPrev = m_smVisionInfo.g_intOptionControlMask;
            m_intPkgOptionControlMaskPrev = m_smVisionInfo.g_intPkgOptionControlMask;
            m_intPkgOptionControlMask2Prev = m_smVisionInfo.g_intPkgOptionControlMask2;

            DisableField2();
            UpdateGUI();
            m_blnInitDone = true;
        }
        private void DisableField()
        {
            string strChild1 = "Option Page";
            string strChild2 = "Save Option Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
            }
        }

        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild2 = "Inspection Option Adv. Control Button";
            string strChild3 = "Save Button";
            if (m_intUserGroup > GetUserRightGroup_Child3(strChild2, strChild3))
            {
                btn_Save.Enabled = false;
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
                case "BottomPosition":
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
            int intTabPageCount = tab_VisionControl.Controls.Count;
            for (int i = 0; i < intTabPageCount; i++)
            {
                tab_VisionControl.Controls.Remove(tab_VisionControl.Controls[0]);
            }

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrient":
                case "BottomPosition":
                    if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    {
                        tab_VisionControl.Controls.Add(tp_Orient);
                        UpdateOrientControlMaskGUI();
                    }

                    if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomPosition") && m_smVisionInfo.g_blnOrientWantPackage
                        || (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomOrient") && m_smVisionInfo.g_blnOrientWantPackage)
                    {
                        tab_VisionControl.Controls.Add(tp_PackageSimple);
                        UpdatePackageControlMaskGUI();
                    }

                    break;
                case "Mark":
                case "InPocket":
                case "IPMLi":
                    tab_VisionControl.Controls.Add(tp_Mark);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);
                    UpdateMarkControlMaskGUI();
                    if ((m_intVisionType & 0x10) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_Lead);
                        UpdateLeadControlMaskGUI();
                    }
                    break;
                case "MarkOrient":
                case "MOLi":
                    tab_VisionControl.Controls.Add(tp_Mark);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    if (m_smVisionInfo.g_blnWantDisplayOrientationInOptionForm)
                        tab_VisionControl.Controls.Add(tp_Orient_ForMO);

                    UpdateOrientationControlMaskGUI();
                    UpdateMarkControlMaskGUI();
                    if ((m_intVisionType & 0x10) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_Lead);
                        UpdateLeadControlMaskGUI();
                    }
                    break;

                case "MarkPkg":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLiPkg":
                    tab_VisionControl.Controls.Add(tp_Mark);

                    tab_VisionControl.Controls.Add(tp_PackageSimple);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    if ((m_intVisionType & 0x10) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_Lead);
                        UpdateLeadControlMaskGUI();
                    }
                    UpdateMarkControlMaskGUI();
                    UpdatePackageControlMaskGUI();

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPackage);
                        UpdateColorPackageControlMaskGUI();
                    }
                    break;
                case "MOPkg":
                case "MOLiPkg":
                    tab_VisionControl.Controls.Add(tp_Mark);

                    tab_VisionControl.Controls.Add(tp_PackageSimple);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    if (m_smVisionInfo.g_blnWantDisplayOrientationInOptionForm)
                        tab_VisionControl.Controls.Add(tp_Orient_ForMO);

                    if ((m_intVisionType & 0x10) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_Lead);
                        UpdateLeadControlMaskGUI();
                    }

                    UpdateOrientationControlMaskGUI();
                    UpdateMarkControlMaskGUI();
                    UpdatePackageControlMaskGUI();

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPackage);
                        UpdateColorPackageControlMaskGUI();
                    }
                    break;

                case "Package":
                    tab_VisionControl.Controls.Add(tp_PackageSimple);
                    UpdatePackageControlMaskGUI();

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPackage);
                        UpdateColorPackageControlMaskGUI();
                    }
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                    tab_VisionControl.Controls.Add(tp_OrientPad);
                    tab_VisionControl.Controls.Add(tp_Pad);
                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);
                    
                    UpdateOrientPadControlMaskGUI();
                    pnl_InspectPad.Visible = true;

                    pnl_SidePad.Visible = false;
                    pnl_SidePadLabel.Visible = false;

                    UpdatePadControlMaskGUI();

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPad);
                        UpdateCenterColorPadControlMaskGUI();
                    }
                    break;
                case "Pad":
                case "PadPos":
                    tab_VisionControl.Controls.Add(tp_Pad);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    pnl_SidePad.Visible = false;
                    pnl_SidePadLabel.Visible = false;

                    UpdatePadControlMaskGUI();

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPad);
                        UpdateCenterColorPadControlMaskGUI();
                        pnl_SidePadColorLabel_Top.Visible = false;
                        pnl_SidePadColor_Top.Visible = false;
                        pnl_SidePadColorLabel_Right.Visible = false;
                        pnl_SidePadColor_Right.Visible = false;
                        pnl_SidePadColorLabel_Bottom.Visible = false;
                        pnl_SidePadColor_Bottom.Visible = false;
                        pnl_SidePadColorLabel_Left.Visible = false;
                        pnl_SidePadColor_Left.Visible = false;
                    }
                    break;
                case "PadPkg":
                case "PadPkgPos":
                    tab_VisionControl.Controls.Add(tp_Pad);
                    tab_VisionControl.Controls.Add(tp_PackagePadSimple);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);
                    
                    pnl_SidePad.Visible = false;
                    pnl_SidePadLabel.Visible = false;
                    pnl_SidePadPkg.Visible = false;
                    pnl_SidePadPkgLabel.Visible = false;

                    UpdatePadControlMaskGUI();
                    UpdateCenterPackagePadControlMaskGUI();

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPad);
                        UpdateCenterColorPadControlMaskGUI();
                        pnl_SidePadColorLabel_Top.Visible = false;
                        pnl_SidePadColor_Top.Visible = false;
                        pnl_SidePadColorLabel_Right.Visible = false;
                        pnl_SidePadColor_Right.Visible = false;
                        pnl_SidePadColorLabel_Bottom.Visible = false;
                        pnl_SidePadColor_Bottom.Visible = false;
                        pnl_SidePadColorLabel_Left.Visible = false;
                        pnl_SidePadColor_Left.Visible = false;
                    }
                    break;
                case "Pad5S":
                case "Pad5SPos":
                    tab_VisionControl.Controls.Add(tp_Pad);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    if (m_smVisionInfo.g_blnCheck4Sides)
                    {
                        pnl_SidePad.Visible = true;
                        pnl_SidePadLabel.Visible = true;
                    }
                    else
                    {
                        pnl_SidePad.Visible = false;
                        pnl_SidePadLabel.Visible = false;
                    }


                    UpdatePadControlMaskGUI();
                    if (m_smVisionInfo.g_blnCheck4Sides)
                        UpdateSidePadControlMaskGUI();

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPad);
                        if (m_smVisionInfo.g_blnCheck4Sides)
                        {
                            pnl_SidePadColorLabel_Top.Visible = true;
                            pnl_SidePadColor_Top.Visible = true;
                            pnl_SidePadColorLabel_Right.Visible = true;
                            pnl_SidePadColor_Right.Visible = true;
                            pnl_SidePadColorLabel_Bottom.Visible = true;
                            pnl_SidePadColor_Bottom.Visible = true;
                            pnl_SidePadColorLabel_Left.Visible = true;
                            pnl_SidePadColor_Left.Visible = true;
                        }
                        else
                        {
                            pnl_SidePadColorLabel_Top.Visible = false;
                            pnl_SidePadColor_Top.Visible = false;
                            pnl_SidePadColorLabel_Right.Visible = false;
                            pnl_SidePadColor_Right.Visible = false;
                            pnl_SidePadColorLabel_Bottom.Visible = false;
                            pnl_SidePadColor_Bottom.Visible = false;
                            pnl_SidePadColorLabel_Left.Visible = false;
                            pnl_SidePadColor_Left.Visible = false;
                        }
                        UpdateCenterColorPadControlMaskGUI();
                        if (m_smVisionInfo.g_blnCheck4Sides)
                            UpdateSideColorPadControlMaskGUI();
                    }
                    break;
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    tab_VisionControl.Controls.Add(tp_Pad);
                    tab_VisionControl.Controls.Add(tp_PackagePadSimple);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    if (m_smVisionInfo.g_blnCheck4Sides)
                    {
                        pnl_SidePad.Visible = true;
                        pnl_SidePadLabel.Visible = true;
                    }
                    else
                    {
                        pnl_SidePad.Visible = false;
                        pnl_SidePadLabel.Visible = false;
                    }

                    if (m_smVisionInfo.g_blnCheck4Sides)
                    {
                        pnl_SidePadPkg.Visible = true;
                        pnl_SidePadPkgLabel.Visible = true;
                    }
                    else
                    {
                        pnl_SidePadPkg.Visible = false;
                        pnl_SidePadPkgLabel.Visible = false;
                    }

                    UpdatePadControlMaskGUI();
                    if (m_smVisionInfo.g_blnCheck4Sides)
                        UpdateSidePadControlMaskGUI();
                    UpdateCenterPackagePadControlMaskGUI();
                    if (m_smVisionInfo.g_blnCheck4Sides)
                        UpdateSidePacakgePadControlMaskGUI();

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPad);
                        if (m_smVisionInfo.g_blnCheck4Sides)
                        {
                            pnl_SidePadColorLabel_Top.Visible = true;
                            pnl_SidePadColor_Top.Visible = true;
                            pnl_SidePadColorLabel_Right.Visible = true;
                            pnl_SidePadColor_Right.Visible = true;
                            pnl_SidePadColorLabel_Bottom.Visible = true;
                            pnl_SidePadColor_Bottom.Visible = true;
                            pnl_SidePadColorLabel_Left.Visible = true;
                            pnl_SidePadColor_Left.Visible = true;
                        }
                        else
                        {
                            pnl_SidePadColorLabel_Top.Visible = false;
                            pnl_SidePadColor_Top.Visible = false;
                            pnl_SidePadColorLabel_Right.Visible = false;
                            pnl_SidePadColor_Right.Visible = false;
                            pnl_SidePadColorLabel_Bottom.Visible = false;
                            pnl_SidePadColor_Bottom.Visible = false;
                            pnl_SidePadColorLabel_Left.Visible = false;
                            pnl_SidePadColor_Left.Visible = false;
                        }
                        UpdateCenterColorPadControlMaskGUI();
                        if (m_smVisionInfo.g_blnCheck4Sides)
                            UpdateSideColorPadControlMaskGUI();
                    }
                    break;
                case "Li3D":
                    tab_VisionControl.Controls.Add(tp_Lead3D);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    UpdateLead3DControlMaskGUI();
                    break;
                case "Li3DPkg":
                    tab_VisionControl.Controls.Add(tp_Lead3D);
                    tab_VisionControl.Controls.Add(tp_Lead3DPkg);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    UpdateLead3DControlMaskGUI();
                    UpdateLead3DPkgControlMaskGUI();
                    break;
                case "Seal":
                    tab_VisionControl.Controls.Add(tp_Seal);
                    UpdateSealControlMaskGUI();
                    break;
                case "UnitPresent":
                    break;
                default:
                    SRMMessageBox.Show("btn_Learn_Click -> There is no such vision module name " + m_smVisionInfo.g_strVisionName + " in this SRMVision software version.");
                    break;
            }

        }
        private void SetCenterColorPadControlMask(object sender)
        {
            if (sender == chk_ColorPad1Length_Center)
            {
                if (chk_ColorPad1Length_Center.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x01;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Center.Text == chk_ColorPad2Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x04;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Center.Text == chk_ColorPad3Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x10;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Center.Text == chk_ColorPad4Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                }
                else
                {
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x01;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Center.Text == chk_ColorPad2Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x04;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Center.Text == chk_ColorPad3Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x10;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Center.Text == chk_ColorPad4Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
                }
            }
            else if (sender == chk_ColorPad1Area_Center)
            {
                if (chk_ColorPad1Area_Center.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x02;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Center.Text == chk_ColorPad2Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x08;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Center.Text == chk_ColorPad3Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x20;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Center.Text == chk_ColorPad4Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                }
                else
                {
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x02;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Center.Text == chk_ColorPad2Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x08;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Center.Text == chk_ColorPad3Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x20;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Center.Text == chk_ColorPad4Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
                }
            }
            else if (sender == chk_ColorPad2Length_Center)
            {
                if (chk_ColorPad2Length_Center.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x04;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Center.Text == chk_ColorPad3Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x10;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Center.Text == chk_ColorPad4Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                }
                else
                {
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x04;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Center.Text == chk_ColorPad3Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x10;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Center.Text == chk_ColorPad4Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
                }
            }
            else if (sender == chk_ColorPad2Area_Center)
            {
                if (chk_ColorPad2Area_Center.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x08;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Center.Text == chk_ColorPad3Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x20;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Center.Text == chk_ColorPad4Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                }
                else
                { 
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x08;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Center.Text == chk_ColorPad3Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x20;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Center.Text == chk_ColorPad4Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
                }
            }
            else if (sender == chk_ColorPad3Length_Center)
            {
                if (chk_ColorPad3Length_Center.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x10;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Center.Text == chk_ColorPad4Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                }
                else
                { 
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x10;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Center.Text == chk_ColorPad4Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
                }
            }
            else if (sender == chk_ColorPad3Area_Center)
            {
                if (chk_ColorPad3Area_Center.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x20;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Center.Text == chk_ColorPad4Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                }
                else
                { 
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x20;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Center.Text == chk_ColorPad4Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
                }
            }
            else if (sender == chk_ColorPad4Length_Center)
            {
                if (chk_ColorPad4Length_Center.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                }
                else
                { 
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
                }
            }
            else if (sender == chk_ColorPad4Area_Center)
            {
                if (chk_ColorPad4Area_Center.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                }
                else
                { 
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
                }
            }
            else if (sender == chk_ColorPad5Length_Center)
            {
                if (chk_ColorPad5Length_Center.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
            }
            else if (sender == chk_ColorPad5Area_Center)
            {
                if (chk_ColorPad5Area_Center.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
            }

        }
        private void SetSideColorPadControlMask_Top(object sender)
        {
            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex)
                    continue;

                int intFailMask = 0;

                if (i == 1)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask2;
                else if (i == 2)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask3;
                else if (i == 3)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask4;
                else if (i == 4)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask5;

                if (sender == chk_ColorPad1Length_Side_Top)
                {
                    if (chk_ColorPad1Length_Side_Top.Checked)
                    {
                        intFailMask |= 0x1000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad2Length_Side_Top.Text)
                            intFailMask |= 0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad3Length_Side_Top.Text)
                            intFailMask |= 0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad4Length_Side_Top.Text)
                            intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    {
                        intFailMask &= ~0x1000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad2Length_Side_Top.Text)
                            intFailMask &= ~0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad3Length_Side_Top.Text)
                            intFailMask &= ~0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad4Length_Side_Top.Text)
                            intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad1Area_Side_Top)
                {
                    if (chk_ColorPad1Area_Side_Top.Checked)
                    {
                        intFailMask |= 0x2000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad2Area_Side_Top.Text)
                            intFailMask |= 0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad3Area_Side_Top.Text)
                            intFailMask |= 0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad4Area_Side_Top.Text)
                            intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    {
                        intFailMask &= ~0x2000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad2Area_Side_Top.Text)
                            intFailMask &= ~0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad3Area_Side_Top.Text)
                            intFailMask &= ~0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad4Area_Side_Top.Text)
                            intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad2Length_Side_Top)
                {
                    if (chk_ColorPad2Length_Side_Top.Checked)
                    {
                        intFailMask |= 0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Top.Text == chk_ColorPad3Length_Side_Top.Text)
                            intFailMask |= 0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Top.Text == chk_ColorPad4Length_Side_Top.Text)
                            intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    {
                        intFailMask &= ~0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Top.Text == chk_ColorPad3Length_Side_Top.Text)
                            intFailMask &= ~0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Top.Text == chk_ColorPad4Length_Side_Top.Text)
                            intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad2Area_Side_Top)
                {
                    if (chk_ColorPad2Area_Side_Top.Checked)
                    {
                        intFailMask |= 0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Top.Text == chk_ColorPad3Area_Side_Top.Text)
                            intFailMask |= 0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Top.Text == chk_ColorPad4Area_Side_Top.Text)
                            intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    {
                        intFailMask &= ~0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Top.Text == chk_ColorPad3Area_Side_Top.Text)
                            intFailMask &= ~0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Top.Text == chk_ColorPad4Area_Side_Top.Text)
                            intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad3Length_Side_Top)
                {
                    if (chk_ColorPad3Length_Side_Top.Checked)
                    {
                        intFailMask |= 0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Top.Text == chk_ColorPad4Length_Side_Top.Text)
                            intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    {
                        intFailMask &= ~0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Top.Text == chk_ColorPad4Length_Side_Top.Text)
                            intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad3Area_Side_Top)
                {
                    if (chk_ColorPad3Area_Side_Top.Checked)
                    {
                        intFailMask |= 0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Top.Text == chk_ColorPad4Area_Side_Top.Text)
                            intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    {
                        intFailMask &= ~0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Top.Text == chk_ColorPad4Area_Side_Top.Text)
                            intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad4Length_Side_Top)
                {
                    if (chk_ColorPad4Length_Side_Top.Checked)
                    {
                        intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    {
                        intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad4Area_Side_Top)
                {
                    if (chk_ColorPad4Area_Side_Top.Checked)
                    {
                        intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    {
                        intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad5Length_Side_Top)
                {
                    if (chk_ColorPad5Length_Side_Top.Checked)
                    {
                        intFailMask |= 0x100000;
                    }
                    else
                        intFailMask &= ~0x100000;
                }
                else if (sender == chk_ColorPad5Area_Side_Top)
                {
                    if (chk_ColorPad5Area_Side_Top.Checked)
                    {
                        intFailMask |= 0x200000;
                    }
                    else
                        intFailMask &= ~0x200000;
                }

                if (i == 1)
                    m_smVisionInfo.g_intOptionControlMask2 = intFailMask;
                else if (i == 2)
                    m_smVisionInfo.g_intOptionControlMask3 = intFailMask;
                else if (i == 3)
                    m_smVisionInfo.g_intOptionControlMask4 = intFailMask;
                else if (i == 4)
                    m_smVisionInfo.g_intOptionControlMask5 = intFailMask;
            }
        }
        private void SetSideColorPadControlMask_Right(object sender)
        {
            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex)
                    continue;

                int intFailMask = 0;

                if (i == 1)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask2;
                else if (i == 2)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask3;
                else if (i == 3)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask4;
                else if (i == 4)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask5;

                if (sender == chk_ColorPad1Length_Side_Right)
                {
                    if (chk_ColorPad1Length_Side_Right.Checked)
                    {
                        intFailMask |= 0x1000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad2Length_Side_Right.Text)
                            intFailMask |= 0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad3Length_Side_Right.Text)
                            intFailMask |= 0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad4Length_Side_Right.Text)
                            intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    { 
                        intFailMask &= ~0x1000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad2Length_Side_Right.Text)
                            intFailMask &= ~0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad3Length_Side_Right.Text)
                            intFailMask &= ~0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad4Length_Side_Right.Text)
                            intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad1Area_Side_Right)
                {
                    if (chk_ColorPad1Area_Side_Right.Checked)
                    {
                        intFailMask |= 0x2000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad2Area_Side_Right.Text)
                            intFailMask |= 0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad3Area_Side_Right.Text)
                            intFailMask |= 0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad4Area_Side_Right.Text)
                            intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    { 
                        intFailMask &= ~0x2000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad2Area_Side_Right.Text)
                            intFailMask &= ~0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad3Area_Side_Right.Text)
                            intFailMask &= ~0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad4Area_Side_Right.Text)
                            intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad2Length_Side_Right)
                {
                    if (chk_ColorPad2Length_Side_Right.Checked)
                    {
                        intFailMask |= 0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Right.Text == chk_ColorPad3Length_Side_Right.Text)
                            intFailMask |= 0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Right.Text == chk_ColorPad4Length_Side_Right.Text)
                            intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    { 
                        intFailMask &= ~0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Right.Text == chk_ColorPad3Length_Side_Right.Text)
                            intFailMask &= ~0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Right.Text == chk_ColorPad4Length_Side_Right.Text)
                            intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad2Area_Side_Right)
                {
                    if (chk_ColorPad2Area_Side_Right.Checked)
                    {
                        intFailMask |= 0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Right.Text == chk_ColorPad3Area_Side_Right.Text)
                            intFailMask |= 0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Right.Text == chk_ColorPad4Area_Side_Right.Text)
                            intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    { 
                        intFailMask &= ~0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Right.Text == chk_ColorPad3Area_Side_Right.Text)
                            intFailMask &= ~0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Right.Text == chk_ColorPad4Area_Side_Right.Text)
                            intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad3Length_Side_Right)
                {
                    if (chk_ColorPad3Length_Side_Right.Checked)
                    {
                        intFailMask |= 0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Right.Text == chk_ColorPad4Length_Side_Right.Text)
                            intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    { 
                        intFailMask &= ~0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Right.Text == chk_ColorPad4Length_Side_Right.Text)
                            intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad3Area_Side_Right)
                {
                    if (chk_ColorPad3Area_Side_Right.Checked)
                    {
                        intFailMask |= 0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Right.Text == chk_ColorPad4Area_Side_Right.Text)
                            intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    { 
                        intFailMask &= ~0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Right.Text == chk_ColorPad4Area_Side_Right.Text)
                            intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad4Length_Side_Right)
                {
                    if (chk_ColorPad4Length_Side_Right.Checked)
                    {
                        intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    { 
                        intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad4Area_Side_Right)
                {
                    if (chk_ColorPad4Area_Side_Right.Checked)
                    {
                        intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    { 
                        intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad5Length_Side_Right)
                {
                    if (chk_ColorPad5Length_Side_Right.Checked)
                    {
                        intFailMask |= 0x100000;
                    }
                    else
                        intFailMask &= ~0x100000;
                }
                else if (sender == chk_ColorPad5Area_Side_Right)
                {
                    if (chk_ColorPad5Area_Side_Right.Checked)
                    {
                        intFailMask |= 0x200000;
                    }
                    else
                        intFailMask &= ~0x200000;
                }

                if (i == 1)
                    m_smVisionInfo.g_intOptionControlMask2 = intFailMask;
                else if (i == 2)
                    m_smVisionInfo.g_intOptionControlMask3 = intFailMask;
                else if (i == 3)
                    m_smVisionInfo.g_intOptionControlMask4 = intFailMask;
                else if (i == 4)
                    m_smVisionInfo.g_intOptionControlMask5 = intFailMask;
            }
        }
        private void SetSideColorPadControlMask_Bottom(object sender)
        {
            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex)
                    continue;

                int intFailMask = 0;

                if (i == 1)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask2;
                else if (i == 2)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask3;
                else if (i == 3)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask4;
                else if (i == 4)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask5;

                if (sender == chk_ColorPad1Length_Side_Bottom)
                {
                    if (chk_ColorPad1Length_Side_Bottom.Checked)
                    {
                        intFailMask |= 0x1000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad2Length_Side_Bottom.Text)
                            intFailMask |= 0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad3Length_Side_Bottom.Text)
                            intFailMask |= 0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad4Length_Side_Bottom.Text)
                            intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    { 
                        intFailMask &= ~0x1000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad2Length_Side_Bottom.Text)
                            intFailMask &= ~0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad3Length_Side_Bottom.Text)
                            intFailMask &= ~0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad4Length_Side_Bottom.Text)
                            intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad1Area_Side_Bottom)
                {
                    if (chk_ColorPad1Area_Side_Bottom.Checked)
                    {
                        intFailMask |= 0x2000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad2Area_Side_Bottom.Text)
                            intFailMask |= 0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad3Area_Side_Bottom.Text)
                            intFailMask |= 0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad4Area_Side_Bottom.Text)
                            intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    { 
                        intFailMask &= ~0x2000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad2Area_Side_Bottom.Text)
                            intFailMask &= ~0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad3Area_Side_Bottom.Text)
                            intFailMask &= ~0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad4Area_Side_Bottom.Text)
                            intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad2Length_Side_Bottom)
                {
                    if (chk_ColorPad2Length_Side_Bottom.Checked)
                    {
                        intFailMask |= 0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Bottom.Text == chk_ColorPad3Length_Side_Bottom.Text)
                            intFailMask |= 0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Bottom.Text == chk_ColorPad4Length_Side_Bottom.Text)
                            intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    { 
                        intFailMask &= ~0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Bottom.Text == chk_ColorPad3Length_Side_Bottom.Text)
                            intFailMask &= ~0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Bottom.Text == chk_ColorPad4Length_Side_Bottom.Text)
                            intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad2Area_Side_Bottom)
                {
                    if (chk_ColorPad2Area_Side_Bottom.Checked)
                    {
                        intFailMask |= 0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Bottom.Text == chk_ColorPad3Area_Side_Bottom.Text)
                            intFailMask |= 0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Bottom.Text == chk_ColorPad4Area_Side_Bottom.Text)
                            intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    { 
                        intFailMask &= ~0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Bottom.Text == chk_ColorPad3Area_Side_Bottom.Text)
                            intFailMask &= ~0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Bottom.Text == chk_ColorPad4Area_Side_Bottom.Text)
                            intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad3Length_Side_Bottom)
                {
                    if (chk_ColorPad3Length_Side_Bottom.Checked)
                    {
                        intFailMask |= 0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Bottom.Text == chk_ColorPad4Length_Side_Bottom.Text)
                            intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    { 
                        intFailMask &= ~0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Bottom.Text == chk_ColorPad4Length_Side_Bottom.Text)
                            intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad3Area_Side_Bottom)
                {
                    if (chk_ColorPad3Area_Side_Bottom.Checked)
                    {
                        intFailMask |= 0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Bottom.Text == chk_ColorPad4Area_Side_Bottom.Text)
                            intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    { 
                        intFailMask &= ~0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Bottom.Text == chk_ColorPad4Area_Side_Bottom.Text)
                            intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad4Length_Side_Bottom)
                {
                    if (chk_ColorPad4Length_Side_Bottom.Checked)
                    {
                        intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    { 
                        intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad4Area_Side_Bottom)
                {
                    if (chk_ColorPad4Area_Side_Bottom.Checked)
                    {
                        intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    { 
                        intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad5Length_Side_Bottom)
                {
                    if (chk_ColorPad5Length_Side_Bottom.Checked)
                    {
                        intFailMask |= 0x100000;
                    }
                    else
                        intFailMask &= ~0x100000;
                }
                else if (sender == chk_ColorPad5Area_Side_Bottom)
                {
                    if (chk_ColorPad5Area_Side_Bottom.Checked)
                    {
                        intFailMask |= 0x200000;
                    }
                    else
                        intFailMask &= ~0x200000;
                }

                if (i == 1)
                    m_smVisionInfo.g_intOptionControlMask2 = intFailMask;
                else if (i == 2)
                    m_smVisionInfo.g_intOptionControlMask3 = intFailMask;
                else if (i == 3)
                    m_smVisionInfo.g_intOptionControlMask4 = intFailMask;
                else if (i == 4)
                    m_smVisionInfo.g_intOptionControlMask5 = intFailMask;
            }
        }
        private void SetSideColorPadControlMask_Left(object sender)
        {
            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex)
                    continue;

                int intFailMask = 0;

                if (i == 1)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask2;
                else if (i == 2)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask3;
                else if (i == 3)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask4;
                else if (i == 4)
                    intFailMask = m_smVisionInfo.g_intOptionControlMask5;

                if (sender == chk_ColorPad1Length_Side_Left)
                {
                    if (chk_ColorPad1Length_Side_Left.Checked)
                    {
                        intFailMask |= 0x1000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad2Length_Side_Left.Text)
                            intFailMask |= 0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad3Length_Side_Left.Text)
                            intFailMask |= 0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad4Length_Side_Left.Text)
                            intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    { 
                        intFailMask &= ~0x1000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad2Length_Side_Left.Text)
                            intFailMask &= ~0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad3Length_Side_Left.Text)
                            intFailMask &= ~0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad4Length_Side_Left.Text)
                            intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad1Area_Side_Left)
                {
                    if (chk_ColorPad1Area_Side_Left.Checked)
                    {
                        intFailMask |= 0x2000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad2Area_Side_Left.Text)
                            intFailMask |= 0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad3Area_Side_Left.Text)
                            intFailMask |= 0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad4Area_Side_Left.Text)
                            intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    { 
                        intFailMask &= ~0x2000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad2Area_Side_Left.Text)
                            intFailMask &= ~0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad3Area_Side_Left.Text)
                            intFailMask &= ~0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad4Area_Side_Left.Text)
                            intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad2Length_Side_Left)
                {
                    if (chk_ColorPad2Length_Side_Left.Checked)
                    {
                        intFailMask |= 0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Left.Text == chk_ColorPad3Length_Side_Left.Text)
                            intFailMask |= 0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Left.Text == chk_ColorPad4Length_Side_Left.Text)
                            intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    { 
                        intFailMask &= ~0x4000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Left.Text == chk_ColorPad3Length_Side_Left.Text)
                            intFailMask &= ~0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Left.Text == chk_ColorPad4Length_Side_Left.Text)
                            intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad2Area_Side_Left)
                {
                    if (chk_ColorPad2Area_Side_Left.Checked)
                    {
                        intFailMask |= 0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Left.Text == chk_ColorPad3Area_Side_Left.Text)
                            intFailMask |= 0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Left.Text == chk_ColorPad4Area_Side_Left.Text)
                            intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    { 
                        intFailMask &= ~0x8000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Left.Text == chk_ColorPad3Area_Side_Left.Text)
                            intFailMask &= ~0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Left.Text == chk_ColorPad4Area_Side_Left.Text)
                            intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad3Length_Side_Left)
                {
                    if (chk_ColorPad3Length_Side_Left.Checked)
                    {
                        intFailMask |= 0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Left.Text == chk_ColorPad4Length_Side_Left.Text)
                            intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    { 
                        intFailMask &= ~0x10000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Left.Text == chk_ColorPad4Length_Side_Left.Text)
                            intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad3Area_Side_Left)
                {
                    if (chk_ColorPad3Area_Side_Left.Checked)
                    {
                        intFailMask |= 0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Left.Text == chk_ColorPad4Area_Side_Left.Text)
                            intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    { 
                        intFailMask &= ~0x20000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Left.Text == chk_ColorPad4Area_Side_Left.Text)
                            intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad4Length_Side_Left)
                {
                    if (chk_ColorPad4Length_Side_Left.Checked)
                    {
                        intFailMask |= 0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask |= 0x100000;
                    }
                    else
                    { 
                        intFailMask &= ~0x40000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask &= ~0x100000;
                    }
                }
                else if (sender == chk_ColorPad4Area_Side_Left)
                {
                    if (chk_ColorPad4Area_Side_Left.Checked)
                    {
                        intFailMask |= 0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask |= 0x200000;
                    }
                    else
                    { 
                        intFailMask &= ~0x80000;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask &= ~0x200000;
                    }
                }
                else if (sender == chk_ColorPad5Length_Side_Left)
                {
                    if (chk_ColorPad5Length_Side_Left.Checked)
                    {
                        intFailMask |= 0x100000;
                    }
                    else
                        intFailMask &= ~0x100000;
                }
                else if (sender == chk_ColorPad5Area_Side_Left)
                {
                    if (chk_ColorPad5Area_Side_Left.Checked)
                    {
                        intFailMask |= 0x200000;
                    }
                    else
                        intFailMask &= ~0x200000;
                }

                if (i == 1)
                    m_smVisionInfo.g_intOptionControlMask2 = intFailMask;
                else if (i == 2)
                    m_smVisionInfo.g_intOptionControlMask3 = intFailMask;
                else if (i == 3)
                    m_smVisionInfo.g_intOptionControlMask4 = intFailMask;
                else if (i == 4)
                    m_smVisionInfo.g_intOptionControlMask5 = intFailMask;
            }
        }
        private void SetPadControlMask(object sender)
        {
            if (sender == chk_InspectPad)
            {
                if (chk_InspectPad.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x4000000;
                else
                {
                    chk_Area_Pad.Checked = false;
                    chk_WidthHeight_Pad.Checked = false;
                    chk_OffSet_Pad.Checked = false;
                    chk_CheckForeignMaterialArea_Pad.Checked = false;
                    chk_CheckForeignMaterialLength_Pad.Checked = false;
                    chk_CheckBrokenArea_Pad.Checked = false;
                    chk_CheckBrokenLength_Pad.Checked = false;
                    chk_Gap_Pad.Checked = false;
                    chk_CheckExcess_Pad.Checked = false;
                    chk_CheckSmear_Pad.Checked = false;
                    chk_CheckForeignMaterialTotalArea_Pad.Checked = false;
                    chk_CheckEdgeLimit_Pad.Checked = false;
                    chk_CheckStandOff_Pad.Checked = false;
                    chk_CheckEdgeDistance_Pad.Checked = false;
                    chk_CheckSpanX_Pad.Checked = false;
                    chk_CheckSpanY_Pad.Checked = false;
                    m_smVisionInfo.g_intOptionControlMask &= ~0xD6FFFFFF;
                }
            }
            else if (sender == chk_Area_Pad)
            {
                if (chk_Area_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x01;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x01;
            }
            else if (sender == chk_WidthHeight_Pad)
            {
                if (chk_WidthHeight_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x02;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x02;
            }
            else if (sender == chk_OffSet_Pad)
            {
                if (chk_OffSet_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x04;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x04;
            }
            else if (sender == chk_CheckForeignMaterialArea_Pad)
            {
                if (chk_CheckForeignMaterialArea_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x08;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x08;
            }
            else if (sender == chk_CheckForeignMaterialLength_Pad)
            {
                if (chk_CheckForeignMaterialLength_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x10;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x10;
            }
            else if (sender == chk_CheckBrokenArea_Pad)
            {
                if (chk_CheckBrokenArea_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x20;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x20;
            }
            else if (sender == chk_CheckBrokenLength_Pad)
            {
                if (chk_CheckBrokenLength_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x40;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x40;
            }
            else if (sender == chk_Gap_Pad)
            {
                if (chk_Gap_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x80;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x80;
            }
            else if (sender == chk_CheckExcess_Pad)
            {
                if (chk_CheckExcess_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x100;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x100;
            }
            else if (sender == chk_CheckSmear_Pad)
            {
                if (chk_CheckSmear_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x200;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x200;
            }
            else if (sender == chk_CheckForeignMaterialTotalArea_Pad)
            {
                if (chk_CheckForeignMaterialTotalArea_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x400;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x400;
            }
            else if (sender == chk_CheckEdgeLimit_Pad)
            {
                if (chk_CheckEdgeLimit_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x800000;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x800000;
            }
            else if (sender == chk_CheckStandOff_Pad)
            {
                if (chk_CheckStandOff_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x2000000;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x2000000;
            }
            else if (sender == chk_CheckEdgeDistance_Pad)
            {
                if (chk_CheckEdgeDistance_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x10000000;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x10000000;
            }
            else if (sender == chk_CheckSpanX_Pad)
            {
                if (chk_CheckSpanX_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x40000000;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x40000000;
            }
            else if (sender == chk_CheckSpanY_Pad)
            {
                if (chk_CheckSpanY_Pad.Checked)
                {
                    if (!chk_InspectPad.Checked)
                    {
                        chk_InspectPad.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                    }

                    m_smVisionInfo.g_intOptionControlMask |= 0x80000000;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x80000000;
            }
        }
        
        private void SetCenterPadPackageControlMask_Simple(object sender)
        {
            if (sender == chk_CheckPkgSize_Pad2)
            {
                if (chk_CheckPkgSize_Pad2.Checked)
                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000;
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x4000;
            }
            else if (sender == chk_InspectPackage_Pad2)
            {
                if (chk_InspectPackage_Pad2.Checked)
                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                else
                {
                    chk_BrightFieldArea_Pad.Checked = false;
                    chk_BrightFieldLength_Pad.Checked = false;
                    chk_DarkFieldArea_Pad.Checked = false;
                    chk_DarkFieldLength_Pad.Checked = false;
                    chk_CrackDarkFieldArea_Pad.Checked = false;
                    chk_CrackDarkFieldLength_Pad.Checked = false;
                    chk_ChippedOffDarkFieldArea_Pad.Checked = false;
                    chk_ChippedOffBrightFieldArea_Pad.Checked = false;
                    chk_MoldFlashBrightFieldArea_Pad.Checked = false;
                    chk_MoldFlashBrightFieldLength_Pad.Checked = false;
                    chk_ForeignMaterialBrightFieldArea_Pad.Checked = false;
                    chk_ForeignMaterialBrightFieldLength_Pad.Checked = false;
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x7FFA000;
                }
            }
            else if (sender == chk_BrightFieldArea_Pad)
            {
                if (chk_BrightFieldArea_Pad.Checked)
                {
                    if (!chk_InspectPackage_Pad2.Checked)
                    {
                        chk_InspectPackage_Pad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x8000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x8000;
            }
            else if (sender == chk_BrightFieldLength_Pad)
            {
                if (chk_BrightFieldLength_Pad.Checked)
                {
                    if (!chk_InspectPackage_Pad2.Checked)
                    {
                        chk_InspectPackage_Pad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x10000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x10000;
            }
            else if (sender == chk_DarkFieldArea_Pad)
            {
                if (chk_DarkFieldArea_Pad.Checked)
                {
                    if (!chk_InspectPackage_Pad2.Checked)
                    {
                        chk_InspectPackage_Pad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x20000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x20000;
            }
            else if (sender == chk_DarkFieldLength_Pad)
            {
                if (chk_DarkFieldLength_Pad.Checked)
                {
                    if (!chk_InspectPackage_Pad2.Checked)
                    {
                        chk_InspectPackage_Pad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x40000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x40000;
            }
            else if (sender == chk_CrackDarkFieldArea_Pad)
            {
                if (chk_CrackDarkFieldArea_Pad.Checked)
                {
                    if (!chk_InspectPackage_Pad2.Checked)
                    {
                        chk_InspectPackage_Pad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x80000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x80000;
            }
            else if (sender == chk_CrackDarkFieldLength_Pad)
            {
                if (chk_CrackDarkFieldLength_Pad.Checked)
                {
                    if (!chk_InspectPackage_Pad2.Checked)
                    {
                        chk_InspectPackage_Pad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x100000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x100000;
            }
            else if (sender == chk_ChippedOffDarkFieldArea_Pad)
            {
                if (chk_ChippedOffDarkFieldArea_Pad.Checked)
                {
                    if (!chk_InspectPackage_Pad2.Checked)
                    {
                        chk_InspectPackage_Pad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x200000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x200000;
            }
            else if (sender == chk_ChippedOffBrightFieldArea_Pad)
            {
                if (chk_ChippedOffBrightFieldArea_Pad.Checked)
                {
                    if (!chk_InspectPackage_Pad2.Checked)
                    {
                        chk_InspectPackage_Pad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x400000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x400000;
            }
            else if (sender == chk_MoldFlashBrightFieldArea_Pad)
            {
                if (chk_MoldFlashBrightFieldArea_Pad.Checked)
                {
                    if (!chk_InspectPackage_Pad2.Checked)
                    {
                        chk_InspectPackage_Pad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x800000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x800000;
            }
            else if (sender == chk_MoldFlashBrightFieldLength_Pad)
            {
                if (chk_MoldFlashBrightFieldLength_Pad.Checked)
                {
                    if (!chk_InspectPackage_Pad2.Checked)
                    {
                        chk_InspectPackage_Pad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x4000000;
            }
            else if (sender == chk_ForeignMaterialBrightFieldArea_Pad)
            {
                if (chk_ForeignMaterialBrightFieldArea_Pad.Checked)
                {
                    if (!chk_InspectPackage_Pad2.Checked)
                    {
                        chk_InspectPackage_Pad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x1000000;
            }
            else if (sender == chk_ForeignMaterialBrightFieldLength_Pad)
            {
                if (chk_ForeignMaterialBrightFieldLength_Pad.Checked)
                {
                    if (!chk_InspectPackage_Pad2.Checked)
                    {
                        chk_InspectPackage_Pad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x2000000;
            }
        }

        private void SetSidePadControlMask(object sender)
        {
            if (chk_Area_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x1000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x1000;

            if (chk_WidthHeight_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x2000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x2000;

            if (chk_OffSet_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x4000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x4000;

            if (chk_CheckForeignMaterialArea_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x8000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x8000;

            if (chk_CheckForeignMaterialLength_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x10000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x10000;

            if (chk_CheckBrokenArea_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x20000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x20000;

            if (chk_CheckBrokenLength_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x40000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x40000;

            if (chk_Gap_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x80000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x80000;

            if (chk_CheckExcess_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x100000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x100000;

            if (chk_CheckSmear_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x200000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x200000;

            if (chk_CheckForeignMaterialTotalArea_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x400000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x400000;

            if (chk_CheckEdgeLimit_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x1000000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x1000000;

            if (chk_CheckStandOff_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x8000000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x8000000;

            if (chk_CheckEdgeDistance_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x20000000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x20000000;

            if (chk_CheckSpanX_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x100000000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x100000000;

            if (chk_CheckSpanX_SidePad.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x200000000;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x200000000;

        }

        private void SetSidePadPackageControlMask_Simple(object sender)
        {
            if (sender == chk_CheckPkgSize_SidePad2)
            {
                if (chk_CheckPkgSize_SidePad2.Checked)
                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x4000;
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x4000;
            }
            else if (sender == chk_InspectPackage_SidePad2)
            {
                if (chk_InspectPackage_SidePad2.Checked)
                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                else
                {
                    chk_BrightFieldArea_SidePad.Checked = false;
                    chk_BrightFieldLength_SidePad.Checked = false;
                    chk_DarkFieldArea_SidePad.Checked = false;
                    chk_DarkFieldLength_SidePad.Checked = false;
                    chk_CrackDarkFieldArea_SidePad.Checked = false;
                    chk_CrackDarkFieldLength_SidePad.Checked = false;
                    chk_ChippedOffDarkFieldArea_SidePad.Checked = false;
                    chk_ChippedOffBrightFieldArea_SidePad.Checked = false;
                    chk_MoldFlashBrightFieldArea_SidePad.Checked = false;
                    chk_MoldFlashBrightFieldLength_SidePad.Checked = false;
                    chk_ForeignMaterialBrightFieldArea_SidePad.Checked = false;
                    chk_ForeignMaterialBrightFieldLength_SidePad.Checked = false;
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x7FFA000;
                }
            }
            else if (sender == chk_BrightFieldArea_SidePad)
            {
                if (chk_BrightFieldArea_SidePad.Checked)
                {
                    if (!chk_InspectPackage_SidePad2.Checked)
                    {
                        chk_InspectPackage_SidePad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x8000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x8000;
            }
            else if (sender == chk_BrightFieldLength_SidePad)
            {
                if (chk_BrightFieldLength_SidePad.Checked)
                {
                    if (!chk_InspectPackage_SidePad2.Checked)
                    {
                        chk_InspectPackage_SidePad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x10000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x10000;
            }
            else if (sender == chk_DarkFieldArea_SidePad)
            {
                if (chk_DarkFieldArea_SidePad.Checked)
                {
                    if (!chk_InspectPackage_SidePad2.Checked)
                    {
                        chk_InspectPackage_SidePad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x20000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x20000;
            }
            else if (sender == chk_DarkFieldLength_SidePad)
            {
                if (chk_DarkFieldLength_SidePad.Checked)
                {
                    if (!chk_InspectPackage_SidePad2.Checked)
                    {
                        chk_InspectPackage_SidePad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x40000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x40000;
            }
            else if (sender == chk_CrackDarkFieldArea_SidePad)
            {
                if (chk_CrackDarkFieldArea_SidePad.Checked)
                {
                    if (!chk_InspectPackage_SidePad2.Checked)
                    {
                        chk_InspectPackage_SidePad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x80000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x80000;
            }
            else if (sender == chk_CrackDarkFieldLength_SidePad)
            {
                if (chk_CrackDarkFieldLength_SidePad.Checked)
                {
                    if (!chk_InspectPackage_SidePad2.Checked)
                    {
                        chk_InspectPackage_SidePad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x100000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x100000;
            }
            else if (sender == chk_ChippedOffDarkFieldArea_SidePad)
            {
                if (chk_ChippedOffDarkFieldArea_SidePad.Checked)
                {
                    if (!chk_InspectPackage_SidePad2.Checked)
                    {
                        chk_InspectPackage_SidePad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x200000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x200000;
            }
            else if (sender == chk_ChippedOffBrightFieldArea_SidePad)
            {
                if (chk_ChippedOffBrightFieldArea_SidePad.Checked)
                {
                    if (!chk_InspectPackage_SidePad2.Checked)
                    {
                        chk_InspectPackage_SidePad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x400000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x400000;
            }
            else if (sender == chk_MoldFlashBrightFieldArea_SidePad)
            {
                if (chk_MoldFlashBrightFieldArea_SidePad.Checked)
                {
                    if (!chk_InspectPackage_SidePad2.Checked)
                    {
                        chk_InspectPackage_SidePad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x800000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x800000;
            }
            else if (sender == chk_MoldFlashBrightFieldLength_SidePad)
            {
                if (chk_MoldFlashBrightFieldLength_SidePad.Checked)
                {
                    if (!chk_InspectPackage_SidePad2.Checked)
                    {
                        chk_InspectPackage_SidePad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x4000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x4000000;
            }
            else if (sender == chk_ForeignMaterialBrightFieldArea_SidePad)
            {
                if (chk_ForeignMaterialBrightFieldArea_SidePad.Checked)
                {
                    if (!chk_InspectPackage_SidePad2.Checked)
                    {
                        chk_InspectPackage_SidePad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x1000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x1000000;
            }
            else if (sender == chk_ForeignMaterialBrightFieldLength_SidePad)
            {
                if (chk_ForeignMaterialBrightFieldLength_SidePad.Checked)
                {
                    if (!chk_InspectPackage_SidePad2.Checked)
                    {
                        chk_InspectPackage_SidePad2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask2 |= 0x2000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask2 &= ~0x2000000;
            }
        }

        private void SetLeadControlMask(object sender)
        {
            if (sender == chk_InspectLead)
            {
                if (chk_InspectLead.Checked)
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                else
                {
                    chk_WidthHeight_Lead.Checked = false;
                    chk_OffSet_Lead.Checked = false;
                    chk_Skew_Lead.Checked = false;
                    chk_Pitch_Lead.Checked = false;
                    chk_Variance_Lead.Checked = false;
                    chk_AverageGrayValue_Lead.Checked = false;
                    chk_Span_Lead.Checked = false;
                    chk_CheckForeignMaterialArea_Lead.Checked = false;
                    chk_CheckForeignMaterialTotalArea_Lead.Checked = false;
                    chk_CheckForeignMaterialLength_Lead.Checked = false;
                    chk_BaseLeadOffset.Checked = false;
                    chk_BaseLeadArea.Checked = false;
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0x1FFFC0;
                }
            }
            else if (sender == chk_WidthHeight_Lead)
            {
                if (chk_WidthHeight_Lead.Checked)
                {
                    if (!chk_InspectLead.Checked)
                    {
                        chk_InspectLead.Checked = true;
                        m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                    }
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0xC0;
                }
                else
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0xC0;
            }
            else if (sender == chk_OffSet_Lead)
            {
                if (chk_OffSet_Lead.Checked)
                {
                    if (!chk_InspectLead.Checked)
                    {
                        chk_InspectLead.Checked = true;
                        m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                    }
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0x100;
                }
                else
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0x100;
            }
            else if (sender == chk_Skew_Lead)
            {
                if (chk_Skew_Lead.Checked)
                {
                    if (!chk_InspectLead.Checked)
                    {
                        chk_InspectLead.Checked = true;
                        m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                    }
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0x20000;
                }
                else
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0x20000;
            }
            else if (sender == chk_Pitch_Lead)
            {
                if (chk_Pitch_Lead.Checked)
                {
                    if (!chk_InspectLead.Checked)
                    {
                        chk_InspectLead.Checked = true;
                        m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                    }
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0x600;
                }
                else
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0x600;
            }
            else if (sender == chk_Variance_Lead)
            {
                if (chk_Variance_Lead.Checked)
                {
                    if (!chk_InspectLead.Checked)
                    {
                        chk_InspectLead.Checked = true;
                        m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                    }
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0x800;
                }
                else
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0x800;
            }
            else if (sender == chk_AverageGrayValue_Lead)
            {
                if (chk_AverageGrayValue_Lead.Checked)
                {
                    if (!chk_InspectLead.Checked)
                    {
                        chk_InspectLead.Checked = true;
                        m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                    }
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0x10000;
                }
                else
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0x10000;
            }
            else if (sender == chk_Span_Lead)
            {
                if (chk_Span_Lead.Checked)
                {
                    if (!chk_InspectLead.Checked)
                    {
                        chk_InspectLead.Checked = true;
                        m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                    }
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0x1000;
                }
                else
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0x1000;
            }
            else if (sender == chk_CheckForeignMaterialArea_Lead)
            {
                if (chk_CheckForeignMaterialArea_Lead.Checked)
                {
                    if (!chk_InspectLead.Checked)
                    {
                        chk_InspectLead.Checked = true;
                        m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                    }
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0x4000;
                }
                else
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0x4000;
            }
            else if (sender == chk_CheckForeignMaterialLength_Lead)
            {
                if (chk_CheckForeignMaterialLength_Lead.Checked)
                {
                    if (!chk_InspectLead.Checked)
                    {
                        chk_InspectLead.Checked = true;
                        m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                    }
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0x8000;
                }
                else
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0x8000;
            }
            else if (sender == chk_CheckForeignMaterialTotalArea_Lead)
            {
                if (chk_CheckForeignMaterialTotalArea_Lead.Checked)
                {
                    if (!chk_InspectLead.Checked)
                    {
                        chk_InspectLead.Checked = true;
                        m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                    }
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0x2000;
                }
                else
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0x2000;
            }
            else if (sender == chk_BaseLeadOffset)
            {
                if (chk_BaseLeadOffset.Checked)
                {
                    if (!chk_InspectLead.Checked)
                    {
                        chk_InspectLead.Checked = true;
                        m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                    }
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0x80000;
                }
                else
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0x80000;
            }
            else if (sender == chk_BaseLeadArea)
            {
                if (chk_BaseLeadArea.Checked)
                {
                    if (!chk_InspectLead.Checked)
                    {
                        chk_InspectLead.Checked = true;
                        m_smVisionInfo.g_intLeadOptionControlMask |= 0x40000;
                    }
                    m_smVisionInfo.g_intLeadOptionControlMask |= 0x100000;
                }
                else
                    m_smVisionInfo.g_intLeadOptionControlMask &= ~0x100000;
            }
        }

        private void SetLead3DControlMask(object sender)
        {
            if (sender == chk_Offset_Lead3D)
            {
                if (chk_Offset_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x10000;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x10000;
            }

            if (sender == chk_Skew_Lead3D)
            {
                if (chk_Skew_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x8000;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x8000;
            }

            if (sender == chk_Width_Lead3D)
            {
                if (chk_Width_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x01;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x01;
            }

            if (sender == chk_Length_Lead3D)
            {
                if (chk_Length_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x02;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x02;
            }

            if (sender == chk_LengthVariance_Lead3D)
            {
                if (chk_LengthVariance_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x04;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x04;
            }

            if (sender == chk_PitchGap_Lead3D)
            {
                if (chk_PitchGap_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x08;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x08;
            }

            if (sender == chk_PitchVariance_Lead3D)
            {
                if (chk_PitchVariance_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x10;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x10;
            }

            if (sender == chk_StandOff_Lead3D)
            {
                if (chk_StandOff_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x20;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x20;
            }

            if (sender == chk_StandoffVariance_Lead3D)
            {
                if (chk_StandoffVariance_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x40;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x40;
            }

            if (sender == chk_Coplan_Lead3D)
            {
                if (chk_Coplan_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x80;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x80;
            }

            if (sender == chk_Span_Lead3D)
            {
                if (chk_Span_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x100;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x100;
            }

            if (sender == chk_LeadSweeps_Lead3D)
            {
                if (chk_LeadSweeps_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x200;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x200;
            }

            if (sender == chk_SolderPadLength_Lead3D)
            {
                if (chk_SolderPadLength_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x400;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x400;
            }

            if (sender == chk_UnCutTiebar_Lead3D)
            {
                if (chk_UnCutTiebar_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x800;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x800;
            }

            if (sender == chk_CheckForeignMaterialArea_Lead3D)
            {
                if (chk_CheckForeignMaterialArea_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x1000;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x1000;
            }

            if (sender == chk_CheckForeignMaterialLength_Lead3D)
            {
                if (chk_CheckForeignMaterialLength_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x2000;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x2000;
            }

            if (sender == chk_CheckForeignMaterialTotalArea_Lead3D)
            {
                if (chk_CheckForeignMaterialTotalArea_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x4000;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x4000;
            }

            if (sender == chk_AverageGrayValue_Lead3D)
            {
                if (chk_AverageGrayValue_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x20000;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x20000;
            }

            if (sender == chk_LeadMinAndMaxWidth_Lead3D)
            {
                if (chk_LeadMinAndMaxWidth_Lead3D.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x40000;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x40000;
            }

            if (sender == chk_LeadBurrWidth)
            {
                if (chk_LeadBurrWidth.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x80000;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x80000;
            }

        }

        private void SetLead3DPkgControlMask_Simple(object sender)
        {
            if (sender == chk_CheckPkgSize_Lead3D)
            {
                if (chk_CheckPkgSize_Lead3D.Checked)
                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000;
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x4000;
            }
            else if (sender == chk_InspectPackage_Lead3D)
            {
                if (chk_InspectPackage_Lead3D.Checked)
                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                else
                {
                    chk_BrightFieldArea_Lead3D.Checked = false;
                    chk_BrightFieldLength_Lead3D.Checked = false;
                    chk_DarkFieldArea_Lead3D.Checked = false;
                    chk_DarkFieldLength_Lead3D.Checked = false;
                    chk_CrackDarkFieldArea_Lead3D.Checked = false;
                    chk_CrackDarkFieldLength_Lead3D.Checked = false;
                    chk_ChippedOffDarkFieldArea_Lead3D.Checked = false;
                    chk_ChippedOffBrightFieldArea_Lead3D.Checked = false;
                    chk_MoldFlashBrightFieldArea_Lead3D.Checked = false;
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0xFFA000;
                }
            }
            else if (sender == chk_BrightFieldArea_Lead3D)
            {
                if (chk_BrightFieldArea_Lead3D.Checked)
                {
                    if (!chk_InspectPackage_Lead3D.Checked)
                    {
                        chk_InspectPackage_Lead3D.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x8000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x8000;
            }
            else if (sender == chk_BrightFieldLength_Lead3D)
            {
                if (chk_BrightFieldLength_Lead3D.Checked)
                {
                    if (!chk_InspectPackage_Lead3D.Checked)
                    {
                        chk_InspectPackage_Lead3D.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x10000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x10000;
            }
            else if (sender == chk_DarkFieldArea_Lead3D)
            {
                if (chk_DarkFieldArea_Lead3D.Checked)
                {
                    if (!chk_InspectPackage_Lead3D.Checked)
                    {
                        chk_InspectPackage_Lead3D.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x20000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x20000;
            }
            else if (sender == chk_DarkFieldLength_Lead3D)
            {
                if (chk_DarkFieldLength_Lead3D.Checked)
                {
                    if (!chk_InspectPackage_Lead3D.Checked)
                    {
                        chk_InspectPackage_Lead3D.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x40000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x40000;
            }
            else if (sender == chk_CrackDarkFieldArea_Lead3D)
            {
                if (chk_CrackDarkFieldArea_Lead3D.Checked)
                {
                    if (!chk_InspectPackage_Lead3D.Checked)
                    {
                        chk_InspectPackage_Lead3D.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x80000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x80000;
            }
            else if (sender == chk_CrackDarkFieldLength_Lead3D)
            {
                if (chk_CrackDarkFieldLength_Lead3D.Checked)
                {
                    if (!chk_InspectPackage_Lead3D.Checked)
                    {
                        chk_InspectPackage_Lead3D.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x100000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x100000;
            }
            else if (sender == chk_ChippedOffDarkFieldArea_Lead3D)
            {
                if (chk_ChippedOffDarkFieldArea_Lead3D.Checked)
                {
                    if (!chk_InspectPackage_Lead3D.Checked)
                    {
                        chk_InspectPackage_Lead3D.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x200000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x200000;
            }
            else if (sender == chk_ChippedOffBrightFieldArea_Lead3D)
            {
                if (chk_ChippedOffBrightFieldArea_Lead3D.Checked)
                {
                    if (!chk_InspectPackage_Lead3D.Checked)
                    {
                        chk_InspectPackage_Lead3D.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x400000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x400000;
            }
            else if (sender == chk_MoldFlashBrightFieldArea_Lead3D)
            {
                if (chk_MoldFlashBrightFieldArea_Lead3D.Checked)
                {
                    if (!chk_InspectPackage_Lead3D.Checked)
                    {
                        chk_InspectPackage_Lead3D.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x2000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x800000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x800000;
            }
        }
        
        private void SetColorPackageControlMask(object sender)
        {
            if (sender == chk_ColorPackage1Length)
            {
                if (chk_ColorPackage1Length.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x01;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 1 && chk_ColorPackage1Length.Text == chk_ColorPackage2Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x04;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage1Length.Text == chk_ColorPackage3Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x10;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage1Length.Text == chk_ColorPackage4Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage1Length.Text == chk_ColorPackage5Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                }
                else
                {
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x01;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 1 && chk_ColorPackage1Length.Text == chk_ColorPackage2Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x04;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage1Length.Text == chk_ColorPackage3Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x10;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage1Length.Text == chk_ColorPackage4Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage1Length.Text == chk_ColorPackage5Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
                }
            }
            else if (sender == chk_ColorPackage1Area)
            {
                if (chk_ColorPackage1Area.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x02;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 1 && chk_ColorPackage1Area.Text == chk_ColorPackage2Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x08;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage1Area.Text == chk_ColorPackage3Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x20;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage1Area.Text == chk_ColorPackage4Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage1Area.Text == chk_ColorPackage5Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                }
                else
                {
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x02;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 1 && chk_ColorPackage1Area.Text == chk_ColorPackage2Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x08;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage1Area.Text == chk_ColorPackage3Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x20;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage1Area.Text == chk_ColorPackage4Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage1Area.Text == chk_ColorPackage5Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
                }
            }
            else if (sender == chk_ColorPackage2Length)
            {
                if (chk_ColorPackage2Length.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x04;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage2Length.Text == chk_ColorPackage3Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x10;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage2Length.Text == chk_ColorPackage4Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage2Length.Text == chk_ColorPackage5Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                }
                else
                {
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x04;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage2Length.Text == chk_ColorPackage3Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x10;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage2Length.Text == chk_ColorPackage4Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage2Length.Text == chk_ColorPackage5Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
                }
            }
            else if (sender == chk_ColorPackage2Area)
            {
                if (chk_ColorPackage2Area.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x08;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage2Area.Text == chk_ColorPackage3Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x20;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage2Area.Text == chk_ColorPackage4Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage2Area.Text == chk_ColorPackage5Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                }
                else
                {
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x08;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage2Area.Text == chk_ColorPackage3Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x20;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage2Area.Text == chk_ColorPackage4Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage2Area.Text == chk_ColorPackage5Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
                }
            }
            else if (sender == chk_ColorPackage3Length)
            {
                if (chk_ColorPackage3Length.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x10;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage3Length.Text == chk_ColorPackage4Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage3Length.Text == chk_ColorPackage5Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                }
                else
                {
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x10;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage3Length.Text == chk_ColorPackage4Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage3Length.Text == chk_ColorPackage5Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
                }
            }
            else if (sender == chk_ColorPackage3Area)
            {
                if (chk_ColorPackage3Area.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x20;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage3Area.Text == chk_ColorPackage4Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage3Area.Text == chk_ColorPackage5Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                }
                else
                {
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x20;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage3Area.Text == chk_ColorPackage4Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage3Area.Text == chk_ColorPackage5Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
                }
            }
            else if (sender == chk_ColorPackage4Length)
            {
                if (chk_ColorPackage4Length.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage4Length.Text == chk_ColorPackage5Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                }
                else
                {
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage4Length.Text == chk_ColorPackage5Length.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
                }
            }
            else if (sender == chk_ColorPackage4Area)
            {
                if (chk_ColorPackage4Area.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage4Area.Text == chk_ColorPackage5Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                }
                else
                {
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage4Area.Text == chk_ColorPackage5Area.Text)
                        m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
                }
            }
            else if (sender == chk_ColorPackage5Length)
            {
                if (chk_ColorPackage5Length.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x100;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x100;
            }
            else if (sender == chk_ColorPackage5Area)
            {
                if (chk_ColorPackage5Area.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask2 |= 0x200;
                }
                else
                    m_smVisionInfo.g_intOptionControlMask2 &= ~0x200;
            }

        }
        private void SetPkgControlMask_Simple(object sender)
        {
            if (!m_blnInitDone)
                return;

            if (sender == chk_CheckPkgSize2)
            {
                if (chk_CheckPkgSize2.Checked)
                    m_smVisionInfo.g_intPkgOptionControlMask |= 0X2000;
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0X2000;
            }
            else if (sender == chk_CheckPkgAngle)
            {
                if (chk_CheckPkgAngle.Checked)
                    m_smVisionInfo.g_intPkgOptionControlMask |= 0X2000000;
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0X2000000;
            }
            else if (sender == chk_InspectPackage2)
            {
                if (chk_InspectPackage2.Checked)
                    m_smVisionInfo.g_intPkgOptionControlMask |= 0X1000;
                else
                {
                    chk_BrightFieldArea.Checked = false;
                    chk_BrightFieldLength.Checked = false;
                    chk_DarkFieldArea.Checked = false;
                    chk_DarkFieldLength.Checked = false;
                    chk_DarkField2Area.Checked = false;
                    chk_DarkField2Length.Checked = false;
                    chk_DarkField3Area.Checked = false;
                    chk_DarkField3Length.Checked = false;
                    chk_DarkField4Area.Checked = false;
                    chk_DarkField4Length.Checked = false;
                    chk_CrackDarkFieldArea.Checked = false;
                    chk_CrackDarkFieldLength.Checked = false;
                    chk_VoidDarkFieldArea.Checked = false;
                    chk_VoidDarkFieldLength.Checked = false;
                    chk_MoldFlashBrightFieldArea.Checked = false;
                    chk_CheckChippedOffBright_Area.Checked = false;
                    chk_CheckChippedOffBright_Length.Checked = false;
                    chk_CheckChippedOffDark_Area.Checked = false;
                    chk_CheckChippedOffDark_Length.Checked = false;
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0XFFFFD000; //~0X1FFD0000 2020-05-29 ZJYEOH : Extra one zero 
                }
            }
            else if (sender == chk_BrightFieldArea)
            {
                if (chk_BrightFieldArea.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x4000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x4000;
            }
            else if (sender == chk_BrightFieldLength)
            {
                if (chk_BrightFieldLength.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x8000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x8000;
            }
            else if (sender == chk_DarkFieldArea)
            {
                if (chk_DarkFieldArea.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x10000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x10000;
            }
            else if (sender == chk_DarkFieldLength)
            {
                if (chk_DarkFieldLength.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x20000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x20000;
            }
            else if (sender == chk_DarkField2Area)
            {
                if (chk_DarkField2Area.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0X4000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0X4000000;
            }
            else if (sender == chk_DarkField2Length)
            {
                if (chk_DarkField2Length.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0X8000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0X8000000;
            }
            else if (sender == chk_DarkField3Area)
            {
                if (chk_DarkField3Area.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0X10000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0X10000000;
            }
            else if (sender == chk_DarkField3Length)
            {
                if (chk_DarkField3Length.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0X20000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0X20000000;
            }
            else if (sender == chk_DarkField4Area)
            {
                if (chk_DarkField4Area.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x100000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x100000;
            }
            else if (sender == chk_DarkField4Length)
            {
                if (chk_DarkField4Length.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x200000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x200000;
            }
            else if (sender == chk_CrackDarkFieldArea)
            {
                if (chk_CrackDarkFieldArea.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x40000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x40000;
            }
            else if (sender == chk_CrackDarkFieldLength)
            {
                if (chk_CrackDarkFieldLength.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x80000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x80000;
            }
            else if (sender == chk_VoidDarkFieldArea)
            {
                if (chk_VoidDarkFieldArea.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x100000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x100000;
            }
            else if (sender == chk_VoidDarkFieldLength)
            {
                if (chk_VoidDarkFieldLength.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x200000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x200000;
            }
            else if (sender == chk_MoldFlashBrightFieldArea)
            {
                if (chk_MoldFlashBrightFieldArea.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x400000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x400000;
            }
            else if (sender == chk_CheckChippedOffBright_Area)
            {
                if (chk_CheckChippedOffBright_Area.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x800000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x800000;
            }
            else if (sender == chk_CheckChippedOffBright_Length)
            {
                if (chk_CheckChippedOffBright_Length.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0X40000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0X40000000;
            }
            else if (sender == chk_CheckChippedOffDark_Area)
            {
                if (chk_CheckChippedOffDark_Area.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0x1000000;
            }
            else if (sender == chk_CheckChippedOffDark_Length)
            {
                if (chk_CheckChippedOffDark_Length.Checked)
                {
                    if (!chk_InspectPackage2.Checked)
                    {
                        chk_InspectPackage2.Checked = true;
                        m_smVisionInfo.g_intPkgOptionControlMask |= 0x1000;
                    }

                    m_smVisionInfo.g_intPkgOptionControlMask |= 0X80000000;
                }
                else
                    m_smVisionInfo.g_intPkgOptionControlMask &= ~0X80000000;
            }
        }

        private void UpdateLead3DControlMaskGUI()
        {
            // Offset 
            chk_Offset_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x10000) > 0;

            // Skew 
            chk_Skew_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x8000) > 0;
            //chk_Skew_Lead3D.Visible = !m_smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance; //2020-07-28 ZJYEOH : Hide Skew if use package to base method

            // Width 
            chk_Width_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x01) > 0;

            // Length
            chk_Length_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x02) > 0;

            // Length Variance 
            chk_LengthVariance_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x04) > 0;

            // Pitch
            chk_PitchGap_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x08) > 0;

            // Pitch Variance
            chk_PitchVariance_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x10) > 0;

            // Stand Off 
            chk_StandOff_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x20) > 0;

            // Stand Off Variance 
            chk_StandoffVariance_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x40) > 0;

            // Coplan 
            chk_Coplan_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x80) > 0;

            // Span
            chk_Span_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x100) > 0;

            // Lead Sweeps
            chk_LeadSweeps_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x200) > 0;

            // Solder Pad Length  
            chk_SolderPadLength_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x400) > 0;

            // Un-Cut Tiebar
            chk_UnCutTiebar_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x800) > 0;

            // Contamination
            chk_CheckForeignMaterialArea_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x1000) > 0;
            chk_CheckForeignMaterialLength_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x2000) > 0;
            chk_CheckForeignMaterialTotalArea_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x4000) > 0;

            // Average Gray Value
            chk_AverageGrayValue_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x20000) > 0;

            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnWantUseAverageGrayValueMethod)
            {
                pnl_AverageGrayValue_Lead3D.Visible = false;
            }

            // Min/Max Width
            chk_LeadMinAndMaxWidth_Lead3D.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x40000) > 0;

            // Burr Width
            chk_LeadBurrWidth.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x80000) > 0;

            //------------------------------------Pin1--------------------------------
            if (m_smVisionInfo.g_blnWantPin1)
            {
                chk_WantInspectPin1.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x800) > 0;
            }

        }

        private void UpdateLead3DPkgControlMaskGUI()
        {
            // Use simple defect criteria

            chk_InspectPackage_Lead3D.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x2000) > 0;
            chk_CheckPkgSize_Lead3D.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x4000) > 0;
            chk_BrightFieldArea_Lead3D.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x8000) > 0;
            chk_BrightFieldLength_Lead3D.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x10000) > 0;
            chk_DarkFieldArea_Lead3D.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x20000) > 0;
            chk_DarkFieldLength_Lead3D.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x40000) > 0;
            chk_CrackDarkFieldArea_Lead3D.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x80000) > 0;
            chk_CrackDarkFieldLength_Lead3D.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x100000) > 0;
            chk_ChippedOffDarkFieldArea_Lead3D.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x200000) > 0;
            chk_ChippedOffBrightFieldArea_Lead3D.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x400000) > 0;
            chk_MoldFlashBrightFieldArea_Lead3D.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x800000) > 0;

            int intAddY = 25;
            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
            {
                pnl_CrackDarkFieldArea_Lead3D.Visible = false;
                pnl_CrackDarkFieldLength_Lead3D.Visible = false;
            }
            //else
            //{
            //    chk_CrackDarkFieldArea_Lead3D.Location = new Point(chk_DarkFieldLength_Lead3D.Location.X, chk_DarkFieldLength_Lead3D.Location.Y + intAddY);
            //    intAddY += 25;
            //    chk_CrackDarkFieldLength_Lead3D.Location = new Point(chk_DarkFieldLength_Lead3D.Location.X, chk_DarkFieldLength_Lead3D.Location.Y + intAddY);
            //    intAddY += 25;
            //}

            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting)
            {
                pnl_ChippedOffDarkFieldArea_Lead3D.Visible = false;
                pnl_ChippedOffBrightFieldArea_Lead3D.Visible = false;
            }
            //else
            //{
            //    chk_ChippedOffDarkFieldArea_Lead3D.Location = new Point(chk_DarkFieldLength_Lead3D.Location.X, chk_DarkFieldLength_Lead3D.Location.Y + intAddY);
            //    intAddY += 25;
            //    chk_ChippedOffBrightFieldArea_Lead3D.Location = new Point(chk_DarkFieldLength_Lead3D.Location.X, chk_DarkFieldLength_Lead3D.Location.Y + intAddY);
            //    intAddY += 25;
            //}

            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
            {
                pnl_MoldFlashBrightFieldArea_Lead3D.Visible = false;
            }
            //else
            //{
            //    chk_MoldFlashBrightFieldArea_Lead3D.Location = new Point(chk_DarkFieldLength_Lead3D.Location.X, chk_DarkFieldLength_Lead3D.Location.Y + intAddY);
            //    intAddY += 25;
            //}

        }
        private void UpdateCenterColorPadControlMaskGUI()
        {
            if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count == 0)
            {
                pnl_CenterPadColorLabel.Visible = false;
                pnl_CenterPadColor.Visible = false;
            }

            pnl_CenterPadColor.Size = new Size(pnl_CenterPadColor.Size.Width, 0);

            List<int> arrColorDefectSkipNo = new List<int>();
            int intTotalColorCount = m_smVisionInfo.g_arrPad[0].GetColorDefectCount(ref arrColorDefectSkipNo);

            for (int i = 0; i < m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count; i++)
            {
                //if (arrColorDefectSkipNo.Contains(i))
                //    continue;

                if (i == 0)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    {
                        chk_ColorPad1Length_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " Length";
                        chk_ColorPad1Area_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " Area";
                    }
                    else
                    {
                        chk_ColorPad1Length_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " 长度";
                        chk_ColorPad1Area_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " 面积";
                    }

                    if (!arrColorDefectSkipNo.Contains(i))
                    {
                        pnl_CenterColorDefect1.Visible = true;
                        pnl_CenterPadColor.Size = new Size(pnl_CenterPadColor.Size.Width, pnl_CenterPadColor.Size.Height + pnl_CenterColorDefect1.Size.Height);
                        //chk_ColorPad1Length_Center.Visible = true;
                        chk_ColorPad1Length_Center.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x01) > 0);
                        //chk_ColorPad1Area_Center.Visible = true;
                        chk_ColorPad1Area_Center.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x02) > 0);
                    }
                }
                if (i == 1)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    {
                        chk_ColorPad2Length_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " Length";
                        chk_ColorPad2Area_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " Area";
                    }
                    else
                    {
                        chk_ColorPad2Length_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " 长度";
                        chk_ColorPad2Area_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " 面积";
                    }
                    if (!arrColorDefectSkipNo.Contains(i))
                    {
                        pnl_CenterColorDefect2.Visible = true;
                        pnl_CenterPadColor.Size = new Size(pnl_CenterPadColor.Size.Width, pnl_CenterPadColor.Size.Height + pnl_CenterColorDefect2.Size.Height);
                        //chk_ColorPad2Length_Center.Visible = true;
                        chk_ColorPad2Length_Center.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x04) > 0);
                        //chk_ColorPad2Area_Center.Visible = true;
                        chk_ColorPad2Area_Center.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x08) > 0);
                    }
                }
                if (i == 2)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    {
                        chk_ColorPad3Length_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " Length";
                        chk_ColorPad3Area_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " Area";
                    }
                    else
                    {
                        chk_ColorPad3Length_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " 长度";
                        chk_ColorPad3Area_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " 面积";
                    }
                    if (!arrColorDefectSkipNo.Contains(i))
                    {
                        pnl_CenterColorDefect3.Visible = true;
                        pnl_CenterPadColor.Size = new Size(pnl_CenterPadColor.Size.Width, pnl_CenterPadColor.Size.Height + pnl_CenterColorDefect3.Size.Height);
                        //chk_ColorPad3Length_Center.Visible = true;
                        chk_ColorPad3Length_Center.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x10) > 0);
                        //chk_ColorPad3Area_Center.Visible = true;
                        chk_ColorPad3Area_Center.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x20) > 0);
                    }
                }
                if (i == 3)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    {
                        chk_ColorPad4Length_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " Length";
                        chk_ColorPad4Area_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " Area";
                    }
                    else
                    {
                        chk_ColorPad4Length_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " 长度";
                        chk_ColorPad4Area_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " 面积";
                    }
                    if (!arrColorDefectSkipNo.Contains(i))
                    {
                        pnl_CenterColorDefect4.Visible = true;
                        pnl_CenterPadColor.Size = new Size(pnl_CenterPadColor.Size.Width, pnl_CenterPadColor.Size.Height + pnl_CenterColorDefect4.Size.Height);
                        //chk_ColorPad4Length_Center.Visible = true;
                        chk_ColorPad4Length_Center.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x40) > 0);
                        //chk_ColorPad4Area_Center.Visible = true;
                        chk_ColorPad4Area_Center.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x80) > 0);
                    }
                }
                if (i == 4)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    {
                        chk_ColorPad5Length_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " Length";
                        chk_ColorPad5Area_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " Area";
                    }
                    else
                    {
                        chk_ColorPad5Length_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " 长度";
                        chk_ColorPad5Area_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[i] + " 面积";
                    }
                    if (!arrColorDefectSkipNo.Contains(i))
                    {
                        pnl_CenterColorDefect5.Visible = true;
                        pnl_CenterPadColor.Size = new Size(pnl_CenterPadColor.Size.Width, pnl_CenterPadColor.Size.Height + pnl_CenterColorDefect5.Size.Height);
                        //chk_ColorPad5Length_Center.Visible = true;
                        chk_ColorPad5Length_Center.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x100) > 0);
                        //chk_ColorPad5Area_Center.Visible = true;
                        chk_ColorPad5Area_Center.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x200) > 0);
                    }
                }
            }
        }
        private string GetColorTabName(int intPadIndex)
        {
            string strName = "";
            if (m_smVisionInfo.g_arrPad[intPadIndex].ref_intColorPadGroupIndex == 0x1E)
            {
                strName = "";
            }
            else
            {
                if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intColorPadGroupIndex & 0x02) > 0)
                {
                    if (strName == "")
                    {
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            strName += "(Top";
                        else
                            strName += "(上";
                    }
                    else
                    {
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            strName += " Top";
                        else
                            strName += " 上";
                    }
                }
                if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intColorPadGroupIndex & 0x04) > 0)
                {
                    if (strName == "")
                    {
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            strName += "(Right";
                        else
                            strName += "(右";
                    }
                    else
                    {
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            strName += " Right";
                        else
                            strName += " 右";
                    }
                }
                if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intColorPadGroupIndex & 0x08) > 0)
                {
                    if (strName == "")
                    {
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            strName += "(Bottom";
                        else
                            strName += "(下";
                    }
                    else
                    {
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            strName += " Bottom";
                        else
                            strName += " 下";
                    }
                }
                if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intColorPadGroupIndex & 0x10) > 0)
                {
                    if (strName == "")
                    {
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            strName += "(Left";
                        else
                            strName += "(左";
                    }
                    else
                    {
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            strName += " Left";
                        else
                            strName += " 左";
                    }
                }
                if (strName != "")
                    strName += ")";
            }
            return strName;
        }
        private void UpdateSideColorPadControlMaskGUI()
        {
            //int intDefectMaxCount = 0;
            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    if (intDefectMaxCount < m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count)
            //        intDefectMaxCount = m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count;
            //}

            //if (intDefectMaxCount == 0)
            //{
            //    if (tab_VisionControl.TabPages.Contains(tp_ColorPad_Side_Top))
            //        tab_VisionControl.TabPages.Remove(tp_ColorPad_Side_Top);
            //}
            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                List<int> arrColorDefectSkipNo = new List<int>();
                int intTotalColorCount = m_smVisionInfo.g_arrPad[i].GetColorDefectCount(ref arrColorDefectSkipNo);
                switch (i)
                {
                    case 1:
                        pnl_SidePadColor_Top.Size = new Size(pnl_SidePadColor_Top.Size.Width, 0);
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            lbl_SidePadColor_Top.Text = "Side Color " + GetColorTabName(i) + ":";
                        else
                            lbl_SidePadColor_Top.Text = "侧边颜色 " + GetColorTabName(i) + ":";

                        for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count; j++)//intDefectMaxCount
                        {
                            //if (arrColorDefectSkipNo.Contains(j))
                            //    continue;

                            if (j == 0)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad1Length_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad1Area_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad1Length_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad1Area_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect1_Top.Visible = true;
                                    pnl_SidePadColor_Top.Size = new Size(pnl_SidePadColor_Top.Size.Width, pnl_SidePadColor_Top.Size.Height + pnl_SideColorDefect1_Top.Size.Height);
                                    //chk_ColorPad1Length_Side_Top.Visible = true;
                                    chk_ColorPad1Length_Side_Top.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x1000) > 0);
                                    //chk_ColorPad1Area_Side_Top.Visible = true;
                                    chk_ColorPad1Area_Side_Top.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x2000) > 0);
                                }
                            }
                            if (j == 1)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad2Length_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad2Area_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad2Length_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad2Area_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect2_Top.Visible = true;
                                    pnl_SidePadColor_Top.Size = new Size(pnl_SidePadColor_Top.Size.Width, pnl_SidePadColor_Top.Size.Height + pnl_SideColorDefect2_Top.Size.Height);
                                    //chk_ColorPad2Length_Side_Top.Visible = true;
                                    chk_ColorPad2Length_Side_Top.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x4000) > 0);
                                    //chk_ColorPad2Area_Side_Top.Visible = true;
                                    chk_ColorPad2Area_Side_Top.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x8000) > 0);
                                }
                            }
                            if (j == 2)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad3Length_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad3Area_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad3Length_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad3Area_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect3_Top.Visible = true;
                                    pnl_SidePadColor_Top.Size = new Size(pnl_SidePadColor_Top.Size.Width, pnl_SidePadColor_Top.Size.Height + pnl_SideColorDefect3_Top.Size.Height);
                                    //chk_ColorPad3Length_Side_Top.Visible = true;
                                    chk_ColorPad3Length_Side_Top.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x10000) > 0);
                                    //chk_ColorPad3Area_Side_Top.Visible = true;
                                    chk_ColorPad3Area_Side_Top.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x20000) > 0);
                                }
                            }
                            if (j == 3)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad4Length_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad4Area_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad4Length_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad4Area_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect4_Top.Visible = true;
                                    pnl_SidePadColor_Top.Size = new Size(pnl_SidePadColor_Top.Size.Width, pnl_SidePadColor_Top.Size.Height + pnl_SideColorDefect4_Top.Size.Height);
                                    //chk_ColorPad4Length_Side_Top.Visible = true;
                                    chk_ColorPad4Length_Side_Top.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x40000) > 0);
                                    //chk_ColorPad4Area_Side_Top.Visible = true;
                                    chk_ColorPad4Area_Side_Top.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x80000) > 0);
                                }
                            }
                            if (j == 4)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad5Length_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad5Area_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad5Length_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad5Area_Side_Top.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect5_Top.Visible = true;
                                    pnl_SidePadColor_Top.Size = new Size(pnl_SidePadColor_Top.Size.Width, pnl_SidePadColor_Top.Size.Height + pnl_SideColorDefect5_Top.Size.Height);
                                    //chk_ColorPad5Length_Side_Top.Visible = true;
                                    chk_ColorPad5Length_Side_Top.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x100000) > 0);
                                    //chk_ColorPad5Area_Side_Top.Visible = true;
                                    chk_ColorPad5Area_Side_Top.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x200000) > 0);
                                }
                            }
                        }

                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count == 0)
                        {
                            pnl_SidePadColorLabel_Top.Visible = false;
                            pnl_SidePadColor_Top.Visible = false;
                        }
                        break;
                    case 2:
                        if ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0)
                        {
                            pnl_SidePadColorLabel_Right.Visible = false;
                            pnl_SidePadColor_Right.Visible = false;

                            continue;
                        }

                        pnl_SidePadColor_Right.Size = new Size(pnl_SidePadColor_Right.Size.Width, 0);
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            lbl_SidePadColor_Right.Text = "Side Color " + GetColorTabName(i) + ":";
                        else
                            lbl_SidePadColor_Right.Text = "侧边颜色 " + GetColorTabName(i) + ":";

                        for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count; j++)//intDefectMaxCount
                        {
                            //if (arrColorDefectSkipNo.Contains(j))
                            //    continue;

                            if (j == 0)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad1Length_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad1Area_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad1Length_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad1Area_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect1_Right.Visible = true;
                                    pnl_SidePadColor_Right.Size = new Size(pnl_SidePadColor_Right.Size.Width, pnl_SidePadColor_Right.Size.Height + pnl_SideColorDefect1_Right.Size.Height);
                                    //chk_ColorPad1Length_Side_Right.Visible = true;
                                    chk_ColorPad1Length_Side_Right.Checked = ((m_smVisionInfo.g_intOptionControlMask3 & 0x1000) > 0);
                                    //chk_ColorPad1Area_Side_Right.Visible = true;
                                    chk_ColorPad1Area_Side_Right.Checked = ((m_smVisionInfo.g_intOptionControlMask3 & 0x2000) > 0);
                                }
                            }
                            if (j == 1)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad2Length_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad2Area_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad2Length_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad2Area_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect2_Right.Visible = true;
                                    pnl_SidePadColor_Right.Size = new Size(pnl_SidePadColor_Right.Size.Width, pnl_SidePadColor_Right.Size.Height + pnl_SideColorDefect2_Right.Size.Height);
                                    //chk_ColorPad2Length_Side_Right.Visible = true;
                                    chk_ColorPad2Length_Side_Right.Checked = ((m_smVisionInfo.g_intOptionControlMask3 & 0x4000) > 0);
                                    //chk_ColorPad2Area_Side_Right.Visible = true;
                                    chk_ColorPad2Area_Side_Right.Checked = ((m_smVisionInfo.g_intOptionControlMask3 & 0x8000) > 0);
                                }
                            }
                            if (j == 2)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad3Length_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad3Area_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad3Length_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad3Area_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect3_Right.Visible = true;
                                    pnl_SidePadColor_Right.Size = new Size(pnl_SidePadColor_Right.Size.Width, pnl_SidePadColor_Right.Size.Height + pnl_SideColorDefect3_Right.Size.Height);
                                    //chk_ColorPad3Length_Side_Right.Visible = true;
                                    chk_ColorPad3Length_Side_Right.Checked = ((m_smVisionInfo.g_intOptionControlMask3 & 0x10000) > 0);
                                    //chk_ColorPad3Area_Side_Right.Visible = true;
                                    chk_ColorPad3Area_Side_Right.Checked = ((m_smVisionInfo.g_intOptionControlMask3 & 0x20000) > 0);
                                }
                            }
                            if (j == 3)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad4Length_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad4Area_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad4Length_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad4Area_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect4_Right.Visible = true;
                                    pnl_SidePadColor_Right.Size = new Size(pnl_SidePadColor_Right.Size.Width, pnl_SidePadColor_Right.Size.Height + pnl_SideColorDefect4_Right.Size.Height);
                                    //chk_ColorPad4Length_Side_Right.Visible = true;
                                    chk_ColorPad4Length_Side_Right.Checked = ((m_smVisionInfo.g_intOptionControlMask3 & 0x40000) > 0);
                                    //chk_ColorPad4Area_Side_Right.Visible = true;
                                    chk_ColorPad4Area_Side_Right.Checked = ((m_smVisionInfo.g_intOptionControlMask3 & 0x80000) > 0);
                                }
                            }
                            if (j == 4)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad5Length_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad5Area_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad5Length_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad5Area_Side_Right.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect5_Right.Visible = true;
                                    pnl_SidePadColor_Right.Size = new Size(pnl_SidePadColor_Right.Size.Width, pnl_SidePadColor_Right.Size.Height + pnl_SideColorDefect5_Right.Size.Height);
                                    //chk_ColorPad5Length_Side_Right.Visible = true;
                                    chk_ColorPad5Length_Side_Right.Checked = ((m_smVisionInfo.g_intOptionControlMask3 & 0x100000) > 0);
                                    //chk_ColorPad5Area_Side_Right.Visible = true;
                                    chk_ColorPad5Area_Side_Right.Checked = ((m_smVisionInfo.g_intOptionControlMask3 & 0x200000) > 0);
                                }
                            }
                        }

                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count == 0)
                        {
                            pnl_SidePadColorLabel_Right.Visible = false;
                            pnl_SidePadColor_Right.Visible = false;
                        }
                        break;
                    case 3:
                        if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x04) > 0))
                        {
                            pnl_SidePadColorLabel_Bottom.Visible = false;
                            pnl_SidePadColor_Bottom.Visible = false;

                            continue;
                        }

                        pnl_SidePadColor_Bottom.Size = new Size(pnl_SidePadColor_Bottom.Size.Width, 0);
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            lbl_SidePadColor_Bottom.Text = "Side Color " + GetColorTabName(i) + ":";
                        else
                            lbl_SidePadColor_Bottom.Text = "侧边颜色 " + GetColorTabName(i) + ":";

                        for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count; j++)//intDefectMaxCount
                        {
                            //if (arrColorDefectSkipNo.Contains(j))
                            //    continue;

                            if (j == 0)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad1Length_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad1Area_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad1Length_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad1Area_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect1_Bottom.Visible = true;
                                    pnl_SidePadColor_Bottom.Size = new Size(pnl_SidePadColor_Bottom.Size.Width, pnl_SidePadColor_Bottom.Size.Height + pnl_SideColorDefect1_Bottom.Size.Height);
                                    //chk_ColorPad1Length_Side_Bottom.Visible = true;
                                    chk_ColorPad1Length_Side_Bottom.Checked = ((m_smVisionInfo.g_intOptionControlMask4 & 0x1000) > 0);
                                    //chk_ColorPad1Area_Side_Bottom.Visible = true;
                                    chk_ColorPad1Area_Side_Bottom.Checked = ((m_smVisionInfo.g_intOptionControlMask4 & 0x2000) > 0);
                                }
                            }
                            if (j == 1)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad2Length_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad2Area_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad2Length_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad2Area_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect2_Bottom.Visible = true;
                                    pnl_SidePadColor_Bottom.Size = new Size(pnl_SidePadColor_Bottom.Size.Width, pnl_SidePadColor_Bottom.Size.Height + pnl_SideColorDefect2_Bottom.Size.Height);
                                    //chk_ColorPad2Length_Side_Bottom.Visible = true;
                                    chk_ColorPad2Length_Side_Bottom.Checked = ((m_smVisionInfo.g_intOptionControlMask4 & 0x4000) > 0);
                                    //chk_ColorPad2Area_Side_Bottom.Visible = true;
                                    chk_ColorPad2Area_Side_Bottom.Checked = ((m_smVisionInfo.g_intOptionControlMask4 & 0x8000) > 0);
                                }
                            }
                            if (j == 2)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad3Length_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad3Area_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad3Length_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad3Area_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect3_Bottom.Visible = true;
                                    pnl_SidePadColor_Bottom.Size = new Size(pnl_SidePadColor_Bottom.Size.Width, pnl_SidePadColor_Bottom.Size.Height + pnl_SideColorDefect3_Bottom.Size.Height);
                                    //chk_ColorPad3Length_Side_Bottom.Visible = true;
                                    chk_ColorPad3Length_Side_Bottom.Checked = ((m_smVisionInfo.g_intOptionControlMask4 & 0x10000) > 0);
                                    //chk_ColorPad3Area_Side_Bottom.Visible = true;
                                    chk_ColorPad3Area_Side_Bottom.Checked = ((m_smVisionInfo.g_intOptionControlMask4 & 0x20000) > 0);
                                }
                            }
                            if (j == 3)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad4Length_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad4Area_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad4Length_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad4Area_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect4_Bottom.Visible = true;
                                    pnl_SidePadColor_Bottom.Size = new Size(pnl_SidePadColor_Bottom.Size.Width, pnl_SidePadColor_Bottom.Size.Height + pnl_SideColorDefect4_Bottom.Size.Height);
                                    //chk_ColorPad4Length_Side_Bottom.Visible = true;
                                    chk_ColorPad4Length_Side_Bottom.Checked = ((m_smVisionInfo.g_intOptionControlMask4 & 0x40000) > 0);
                                    //chk_ColorPad4Area_Side_Bottom.Visible = true;
                                    chk_ColorPad4Area_Side_Bottom.Checked = ((m_smVisionInfo.g_intOptionControlMask4 & 0x80000) > 0);
                                }
                            }
                            if (j == 4)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad5Length_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad5Area_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad5Length_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad5Area_Side_Bottom.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect5_Bottom.Visible = true;
                                    pnl_SidePadColor_Bottom.Size = new Size(pnl_SidePadColor_Bottom.Size.Width, pnl_SidePadColor_Bottom.Size.Height + pnl_SideColorDefect5_Bottom.Size.Height);
                                    //chk_ColorPad5Length_Side_Bottom.Visible = true;
                                    chk_ColorPad5Length_Side_Bottom.Checked = ((m_smVisionInfo.g_intOptionControlMask4 & 0x100000) > 0);
                                    //chk_ColorPad5Area_Side_Bottom.Visible = true;
                                    chk_ColorPad5Area_Side_Bottom.Checked = ((m_smVisionInfo.g_intOptionControlMask4 & 0x200000) > 0);
                                }
                            }
                        }

                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count == 0)
                        {
                            pnl_SidePadColorLabel_Bottom.Visible = false;
                            pnl_SidePadColor_Bottom.Visible = false;
                        }
                        break;
                    case 4:
                        if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x04) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x08) > 0))
                        {
                            pnl_SidePadColorLabel_Left.Visible = false;
                            pnl_SidePadColor_Left.Visible = false;

                            continue;
                        }

                        pnl_SidePadColor_Left.Size = new Size(pnl_SidePadColor_Left.Size.Width, 0);
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            lbl_SidePadColor_Left.Text = "Side Color " + GetColorTabName(i) + ":";
                        else
                            lbl_SidePadColor_Left.Text = "侧边颜色 " + GetColorTabName(i) + ":";

                        for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count; j++)//intDefectMaxCount
                        {
                            //if (arrColorDefectSkipNo.Contains(j))
                            //    continue;

                            if (j == 0)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad1Length_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad1Area_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad1Length_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad1Area_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect1_Left.Visible = true;
                                    pnl_SidePadColor_Left.Size = new Size(pnl_SidePadColor_Left.Size.Width, pnl_SidePadColor_Left.Size.Height + pnl_SideColorDefect1_Left.Size.Height);
                                    //chk_ColorPad1Length_Side_Left.Visible = true;
                                    chk_ColorPad1Length_Side_Left.Checked = ((m_smVisionInfo.g_intOptionControlMask5 & 0x1000) > 0);
                                    //chk_ColorPad1Area_Side_Left.Visible = true;
                                    chk_ColorPad1Area_Side_Left.Checked = ((m_smVisionInfo.g_intOptionControlMask5 & 0x2000) > 0);
                                }
                            }
                            if (j == 1)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad2Length_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad2Area_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad2Length_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad2Area_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect2_Left.Visible = true;
                                    pnl_SidePadColor_Left.Size = new Size(pnl_SidePadColor_Left.Size.Width, pnl_SidePadColor_Left.Size.Height + pnl_SideColorDefect2_Left.Size.Height);
                                    //chk_ColorPad2Length_Side_Left.Visible = true;
                                    chk_ColorPad2Length_Side_Left.Checked = ((m_smVisionInfo.g_intOptionControlMask5 & 0x4000) > 0);
                                    //chk_ColorPad2Area_Side_Left.Visible = true;
                                    chk_ColorPad2Area_Side_Left.Checked = ((m_smVisionInfo.g_intOptionControlMask5 & 0x8000) > 0);
                                }
                            }
                            if (j == 2)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad3Length_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad3Area_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad3Length_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad3Area_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect3_Left.Visible = true;
                                    pnl_SidePadColor_Left.Size = new Size(pnl_SidePadColor_Left.Size.Width, pnl_SidePadColor_Left.Size.Height + pnl_SideColorDefect3_Left.Size.Height);
                                    //chk_ColorPad3Length_Side_Left.Visible = true;
                                    chk_ColorPad3Length_Side_Left.Checked = ((m_smVisionInfo.g_intOptionControlMask5 & 0x10000) > 0);
                                    //chk_ColorPad3Area_Side_Left.Visible = true;
                                    chk_ColorPad3Area_Side_Left.Checked = ((m_smVisionInfo.g_intOptionControlMask5 & 0x20000) > 0);
                                }
                            }
                            if (j == 3)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad4Length_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad4Area_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad4Length_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad4Area_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect4_Left.Visible = true;
                                    pnl_SidePadColor_Left.Size = new Size(pnl_SidePadColor_Left.Size.Width, pnl_SidePadColor_Left.Size.Height + pnl_SideColorDefect4_Left.Size.Height);
                                    //chk_ColorPad4Length_Side_Left.Visible = true;
                                    chk_ColorPad4Length_Side_Left.Checked = ((m_smVisionInfo.g_intOptionControlMask5 & 0x40000) > 0);
                                    //chk_ColorPad4Area_Side_Left.Visible = true;
                                    chk_ColorPad4Area_Side_Left.Checked = ((m_smVisionInfo.g_intOptionControlMask5 & 0x80000) > 0);
                                }
                            }
                            if (j == 4)
                            {
                                if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                                {
                                    chk_ColorPad5Length_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Length";
                                    chk_ColorPad5Area_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " Area";
                                }
                                else
                                {
                                    chk_ColorPad5Length_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 长度";
                                    chk_ColorPad5Area_Side_Left.Text = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[j] + " 面积";
                                }
                                if (!arrColorDefectSkipNo.Contains(j))
                                {
                                    pnl_SideColorDefect5_Left.Visible = true;
                                    pnl_SidePadColor_Left.Size = new Size(pnl_SidePadColor_Left.Size.Width, pnl_SidePadColor_Left.Size.Height + pnl_SideColorDefect5_Left.Size.Height);
                                    //chk_ColorPad5Length_Side_Left.Visible = true;
                                    chk_ColorPad5Length_Side_Left.Checked = ((m_smVisionInfo.g_intOptionControlMask5 & 0x100000) > 0);
                                    //chk_ColorPad5Area_Side_Left.Visible = true;
                                    chk_ColorPad5Area_Side_Left.Checked = ((m_smVisionInfo.g_intOptionControlMask5 & 0x200000) > 0);
                                }
                            }
                        }

                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count == 0)
                        {
                            pnl_SidePadColorLabel_Left.Visible = false;
                            pnl_SidePadColor_Left.Visible = false;
                        }
                        break;
                }
            }
        }
        private void UpdatePadControlMaskGUI()
        {
            // Inspect Pad
            chk_InspectPad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x4000000) > 0;

            // Area
            chk_Area_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x01) > 0;

            // Width and Height
            chk_WidthHeight_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x02) > 0;

            // Off Set
            chk_OffSet_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x04) > 0;

            // Foreign Material / Contamination
            chk_CheckForeignMaterialArea_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x08) > 0;
            chk_CheckForeignMaterialLength_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x10) > 0;

            // Broken Pad 
            chk_CheckBrokenArea_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x20) > 0;
            chk_CheckBrokenLength_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x40) > 0;

            // Pitch Gap
            chk_Gap_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x80) > 0;

            chk_CheckExcess_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x100) > 0;

            chk_CheckSmear_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x200) > 0;

            chk_CheckEdgeLimit_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x800000) > 0;

            chk_CheckStandOff_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x2000000) > 0;

            chk_CheckEdgeDistance_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x10000000) > 0;

            chk_CheckSpanX_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x40000000) > 0;

            chk_CheckSpanY_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x80000000) > 0;

            chk_CheckForeignMaterialTotalArea_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x400) > 0;

            //------------------------------------Pin1--------------------------------
            if (m_smVisionInfo.g_blnWantPin1)
            {
                chk_WantInspectPin1.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x800) > 0;
            }

            if ((m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg") && m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize && !m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON)
            {
                pnl_CheckForeignMaterialArea_Pad.Visible = false;
                pnl_CheckForeignMaterialTotalArea_Pad.Visible = false;
                pnl_CheckForeignMaterialLength_Pad.Visible = false;
                pnl_Pad.Size = new Size(pnl_Pad.Size.Width, pnl_Pad.Size.Height - pnl_CheckForeignMaterialArea_Pad.Size.Height - pnl_CheckForeignMaterialTotalArea_Pad.Size.Height - pnl_CheckForeignMaterialLength_Pad.Size.Height);
                //chk_CheckSmear_Pad.Location = chk_CheckForeignMaterialArea_Pad.Location;
                //if (m_smVisionInfo.g_arrPad[0].ref_blnWantEdgeLimit_Pad)
                //    chk_CheckEdgeLimit_Pad.Location = chk_CheckForeignMaterialTotalArea_Pad.Location;
                //else if (m_smVisionInfo.g_arrPad[0].ref_blnWantStandOff_Pad)
                //    chk_CheckStandOff_Pad.Location = chk_CheckForeignMaterialTotalArea_Pad.Location;
            }

            if (!m_smVisionInfo.g_arrPad[0].ref_blnWantEdgeLimit_Pad)
            {
                pnl_CheckEdgeLimit_Pad.Visible = false;
                pnl_Pad.Size = new Size(pnl_Pad.Size.Width, pnl_Pad.Size.Height - pnl_CheckEdgeLimit_Pad.Size.Height);
            }
            if (!m_smVisionInfo.g_arrPad[0].ref_blnWantStandOff_Pad)
            {
                pnl_CheckStandOff_Pad.Visible = false;
                pnl_Pad.Size = new Size(pnl_Pad.Size.Width, pnl_Pad.Size.Height - pnl_CheckStandOff_Pad.Size.Height);
            }
            if (!m_smVisionInfo.g_arrPad[0].ref_blnWantEdgeDistance_Pad)
            {
                pnl_CheckEdgeDistance_Pad.Visible = false;
                pnl_Pad.Size = new Size(pnl_Pad.Size.Width, pnl_Pad.Size.Height - pnl_CheckEdgeDistance_Pad.Size.Height);
            }
            if (!m_smVisionInfo.g_arrPad[0].ref_blnWantSpan_Pad)
            {
                pnl_CheckSpanX_Pad.Visible = false;
                pnl_Pad.Size = new Size(pnl_Pad.Size.Width, pnl_Pad.Size.Height - pnl_CheckSpanX_Pad.Size.Height);
                pnl_CheckSpanY_Pad.Visible = false;
                pnl_Pad.Size = new Size(pnl_Pad.Size.Width, pnl_Pad.Size.Height - pnl_CheckSpanY_Pad.Size.Height);
            }
        }

        private void UpdateSidePadControlMaskGUI()
        {
            // Area
            chk_Area_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x1000) > 0;

            // Width and Height
            chk_WidthHeight_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x2000) > 0;

            // Off Set
            chk_OffSet_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x4000) > 0;

            // Foreign Material / Contamination
            chk_CheckForeignMaterialArea_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x8000) > 0;
            chk_CheckForeignMaterialLength_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x10000) > 0;

            // Broken Pad 
            chk_CheckBrokenArea_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x20000) > 0;
            chk_CheckBrokenLength_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x40000) > 0;

            // Pitch Gap
            chk_Gap_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x80000) > 0;

            chk_CheckExcess_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x100000) > 0;

            chk_CheckSmear_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x200000) > 0;

            chk_CheckEdgeLimit_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x1000000) > 0;

            chk_CheckStandOff_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x8000000) > 0;

            chk_CheckEdgeDistance_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x20000000) > 0;

            chk_CheckSpanX_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x100000000) > 0;

            chk_CheckSpanY_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x200000000) > 0;

            chk_CheckForeignMaterialTotalArea_SidePad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x400000) > 0;

            if ((m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg") && m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize && !m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON)
            {
                pnl_CheckForeignMaterialArea_SidePad.Visible = false;
                pnl_CheckForeignMaterialLength_SidePad.Visible = false;
                pnl_CheckForeignMaterialTotalArea_SidePad.Visible = false;
                pnl_SidePad.Size = new Size(pnl_SidePad.Size.Width, pnl_SidePad.Size.Height - pnl_CheckForeignMaterialArea_SidePad.Size.Height - pnl_CheckForeignMaterialLength_SidePad.Size.Height - pnl_CheckForeignMaterialTotalArea_SidePad.Size.Height);
                //chk_CheckSmear_SidePad.Location = chk_CheckForeignMaterialArea_SidePad.Location;
                //if (m_smVisionInfo.g_arrPad[1].ref_blnWantEdgeLimit_Pad)
                //    chk_CheckEdgeLimit_SidePad.Location = chk_CheckForeignMaterialTotalArea_SidePad.Location;
                //else if (m_smVisionInfo.g_arrPad[1].ref_blnWantStandOff_Pad)
                //    chk_CheckStandOff_SidePad.Location = chk_CheckForeignMaterialTotalArea_SidePad.Location;
            }

            if (!m_smVisionInfo.g_arrPad[1].ref_blnWantEdgeLimit_Pad)
            {
                pnl_CheckEdgeLimit_SidePad.Visible = false;
                pnl_SidePad.Size = new Size(pnl_SidePad.Size.Width, pnl_SidePad.Size.Height - pnl_CheckEdgeLimit_SidePad.Size.Height);
            }
            if (!m_smVisionInfo.g_arrPad[1].ref_blnWantStandOff_Pad)
            {
                pnl_CheckStandOff_SidePad.Visible = false;
                pnl_SidePad.Size = new Size(pnl_SidePad.Size.Width, pnl_SidePad.Size.Height - pnl_CheckStandOff_SidePad.Size.Height);
            }
            if (!m_smVisionInfo.g_arrPad[1].ref_blnWantEdgeDistance_Pad)
            {
                pnl_CheckEdgeDistance_SidePad.Visible = false;
                pnl_SidePad.Size = new Size(pnl_SidePad.Size.Width, pnl_SidePad.Size.Height - pnl_CheckEdgeDistance_SidePad.Size.Height);
            }
            if (!m_smVisionInfo.g_arrPad[1].ref_blnWantSpan_Pad)
            {
                pnl_CheckSpanX_SidePad.Visible = false;
                pnl_SidePad.Size = new Size(pnl_SidePad.Size.Width, pnl_SidePad.Size.Height - pnl_CheckSpanX_SidePad.Size.Height);
                pnl_CheckSpanY_SidePad.Visible = false;
                pnl_SidePad.Size = new Size(pnl_SidePad.Size.Width, pnl_SidePad.Size.Height - pnl_CheckSpanY_SidePad.Size.Height);
            }
        }

        private void UpdateCenterPackagePadControlMaskGUI()
        {
            chk_InspectPackage_Pad2.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x2000) > 0;
            chk_CheckPkgSize_Pad2.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x4000) > 0;
            chk_BrightFieldArea_Pad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x8000) > 0;
            chk_BrightFieldLength_Pad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x10000) > 0;
            chk_DarkFieldArea_Pad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x20000) > 0;
            chk_DarkFieldLength_Pad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x40000) > 0;
            chk_CrackDarkFieldArea_Pad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x80000) > 0;
            chk_CrackDarkFieldLength_Pad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x100000) > 0;
            chk_ChippedOffDarkFieldArea_Pad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x200000) > 0;
            chk_ChippedOffBrightFieldArea_Pad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x400000) > 0;
            chk_MoldFlashBrightFieldArea_Pad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x800000) > 0;
            chk_MoldFlashBrightFieldLength_Pad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x4000000) > 0;
            chk_ForeignMaterialBrightFieldArea_Pad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x1000000) > 0;
            chk_ForeignMaterialBrightFieldLength_Pad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x2000000) > 0;

            int intAddY = 25;
            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
            {
                pnl_CrackDarkFieldArea_Pad.Visible = false;
                pnl_CrackDarkFieldLength_Pad.Visible = false;
                pnl_PadPkg.Size = new Size(pnl_PadPkg.Size.Width, pnl_PadPkg.Size.Height - pnl_CrackDarkFieldArea_Pad.Size.Height - pnl_CrackDarkFieldLength_Pad.Size.Height);
            }
            //else
            //{
            //    chk_CrackDarkFieldArea_Pad.Location = new Point(chk_DarkFieldLength_Pad.Location.X, chk_DarkFieldLength_Pad.Location.Y + intAddY);
            //    intAddY += 25;
            //    chk_CrackDarkFieldLength_Pad.Location = new Point(chk_DarkFieldLength_Pad.Location.X, chk_DarkFieldLength_Pad.Location.Y + intAddY);
            //    intAddY += 25;
            //}

            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateChippedOffDefectSetting)
            {
                pnl_ChippedOffDarkFieldArea_Pad.Visible = false;
                pnl_ChippedOffBrightFieldArea_Pad.Visible = false;
                pnl_PadPkg.Size = new Size(pnl_PadPkg.Size.Width, pnl_PadPkg.Size.Height - pnl_ChippedOffDarkFieldArea_Pad.Size.Height - pnl_ChippedOffBrightFieldArea_Pad.Size.Height);
            }
            //else
            //{
            //    chk_ChippedOffDarkFieldArea_Pad.Location = new Point(chk_DarkFieldLength_Pad.Location.X, chk_DarkFieldLength_Pad.Location.Y + intAddY);
            //    intAddY += 25;
            //    chk_ChippedOffBrightFieldArea_Pad.Location = new Point(chk_DarkFieldLength_Pad.Location.X, chk_DarkFieldLength_Pad.Location.Y + intAddY);
            //    intAddY += 25;
            //}

            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
            {
                pnl_MoldFlashBrightFieldArea_Pad.Visible = false;
                pnl_PadPkg.Size = new Size(pnl_PadPkg.Size.Width, pnl_PadPkg.Size.Height - pnl_MoldFlashBrightFieldArea_Pad.Size.Height);
                pnl_MoldFlashBrightFieldLength_Pad.Visible = false;
                pnl_PadPkg.Size = new Size(pnl_PadPkg.Size.Width, pnl_PadPkg.Size.Height - pnl_MoldFlashBrightFieldLength_Pad.Size.Height);
            }
            //else
            //{
            //    chk_MoldFlashBrightFieldArea_Pad.Location = new Point(chk_DarkFieldLength_Pad.Location.X, chk_DarkFieldLength_Pad.Location.Y + intAddY);
            //    intAddY += 25;
            //}

            if (!m_smVisionInfo.g_arrPad[0].ref_blnSeperateForeignMaterialDefectSetting)
            {
                pnl_ForeignMaterialBrightFieldArea_Pad.Visible = false;
                pnl_ForeignMaterialBrightFieldLength_Pad.Visible = false;
                pnl_PadPkg.Size = new Size(pnl_PadPkg.Size.Width, pnl_PadPkg.Size.Height - pnl_ForeignMaterialBrightFieldArea_Pad.Size.Height - pnl_ForeignMaterialBrightFieldLength_Pad.Size.Height);
            }
            //else
            //{
            //    chk_ForeignMaterialBrightFieldArea_Pad.Location = new Point(chk_DarkFieldLength_Pad.Location.X, chk_DarkFieldLength_Pad.Location.Y + intAddY);
            //    intAddY += 25;
            //    chk_ForeignMaterialBrightFieldLength_Pad.Location = new Point(chk_DarkFieldLength_Pad.Location.X, chk_DarkFieldLength_Pad.Location.Y + intAddY);
            //    intAddY += 25;
            //}

        }

        private void UpdateSidePacakgePadControlMaskGUI()
        {
            chk_InspectPackage_SidePad2.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x2000) > 0;
            chk_CheckPkgSize_SidePad2.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x4000) > 0;
            chk_BrightFieldArea_SidePad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x8000) > 0;
            chk_BrightFieldLength_SidePad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x10000) > 0;
            chk_DarkFieldArea_SidePad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x20000) > 0;
            chk_DarkFieldLength_SidePad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x40000) > 0;
            chk_CrackDarkFieldArea_SidePad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x80000) > 0;
            chk_CrackDarkFieldLength_SidePad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x100000) > 0;
            chk_ChippedOffDarkFieldArea_SidePad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x200000) > 0;
            chk_ChippedOffBrightFieldArea_SidePad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x400000) > 0;
            chk_MoldFlashBrightFieldArea_SidePad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x800000) > 0;
            chk_MoldFlashBrightFieldLength_SidePad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x4000000) > 0;
            chk_ForeignMaterialBrightFieldArea_SidePad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x1000000) > 0;
            chk_ForeignMaterialBrightFieldLength_SidePad.Checked = (m_smVisionInfo.g_intPkgOptionControlMask2 & 0x2000000) > 0;

            int intAddY = 25;
            if (!m_smVisionInfo.g_arrPad[1].ref_blnSeperateCrackDefectSetting)
            {
                pnl_CrackDarkFieldArea_SidePad.Visible = false;
                pnl_CrackDarkFieldLength_SidePad.Visible = false;
                pnl_SidePadPkg.Size = new Size(pnl_SidePadPkg.Size.Width, pnl_SidePadPkg.Size.Height - pnl_CrackDarkFieldArea_SidePad.Size.Height - pnl_CrackDarkFieldLength_SidePad.Size.Height);
            }
            //else
            //{
            //    chk_CrackDarkFieldArea_SidePad.Location = new Point(chk_DarkFieldLength_SidePad.Location.X, chk_DarkFieldLength_SidePad.Location.Y + intAddY);
            //    intAddY += 25;
            //    chk_CrackDarkFieldLength_SidePad.Location = new Point(chk_DarkFieldLength_SidePad.Location.X, chk_DarkFieldLength_SidePad.Location.Y + intAddY);
            //    intAddY += 25;
            //}

            if (!m_smVisionInfo.g_arrPad[1].ref_blnSeperateChippedOffDefectSetting)
            {
                pnl_ChippedOffDarkFieldArea_SidePad.Visible = false;
                pnl_ChippedOffBrightFieldArea_SidePad.Visible = false;
                pnl_SidePadPkg.Size = new Size(pnl_SidePadPkg.Size.Width, pnl_SidePadPkg.Size.Height - pnl_ChippedOffDarkFieldArea_SidePad.Size.Height - pnl_ChippedOffBrightFieldArea_SidePad.Size.Height);
            }
            //else
            //{
            //    chk_ChippedOffDarkFieldArea_SidePad.Location = new Point(chk_DarkFieldLength_SidePad.Location.X, chk_DarkFieldLength_SidePad.Location.Y + intAddY);
            //    intAddY += 25;
            //    chk_ChippedOffBrightFieldArea_SidePad.Location = new Point(chk_DarkFieldLength_SidePad.Location.X, chk_DarkFieldLength_SidePad.Location.Y + intAddY);
            //    intAddY += 25;
            //}

            if (!m_smVisionInfo.g_arrPad[1].ref_blnSeperateMoldFlashDefectSetting)
            {
                pnl_MoldFlashBrightFieldArea_SidePad.Visible = false;
                pnl_SidePadPkg.Size = new Size(pnl_SidePadPkg.Size.Width, pnl_SidePadPkg.Size.Height - pnl_MoldFlashBrightFieldArea_SidePad.Size.Height);
                pnl_MoldFlashBrightFieldLength_SidePad.Visible = false;
                pnl_SidePadPkg.Size = new Size(pnl_SidePadPkg.Size.Width, pnl_SidePadPkg.Size.Height - pnl_MoldFlashBrightFieldLength_SidePad.Size.Height);
            }
            //else
            //{
            //    chk_MoldFlashBrightFieldArea_SidePad.Location = new Point(chk_DarkFieldLength_SidePad.Location.X, chk_DarkFieldLength_SidePad.Location.Y + intAddY);
            //    intAddY += 25;
            //}

            if (!m_smVisionInfo.g_arrPad[1].ref_blnSeperateForeignMaterialDefectSetting)
            {
                pnl_ForeignMaterialBrightFieldArea_SidePad.Visible = false;
                pnl_ForeignMaterialBrightFieldLength_SidePad.Visible = false;
                pnl_SidePadPkg.Size = new Size(pnl_SidePadPkg.Size.Width, pnl_SidePadPkg.Size.Height - pnl_ForeignMaterialBrightFieldArea_SidePad.Size.Height - pnl_ForeignMaterialBrightFieldLength_SidePad.Size.Height);
            }
            //else
            //{
            //    chk_ForeignMaterialBrightFieldArea_SidePad.Location = new Point(chk_DarkFieldLength_SidePad.Location.X, chk_DarkFieldLength_SidePad.Location.Y + intAddY);
            //    intAddY += 25;
            //    chk_ForeignMaterialBrightFieldLength_SidePad.Location = new Point(chk_DarkFieldLength_SidePad.Location.X, chk_DarkFieldLength_SidePad.Location.Y + intAddY);
            //    intAddY += 25;
            //}

        }
        private void UpdateOrientControlMaskGUI()
        {
            //------------------------------------Orient--------------------------------

            chk_WantInspectOrientAngleTolerance.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x01) > 0;
            chk_WantInspectOrientXTolerance.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x02) > 0;
            chk_WantInspectOrientYTolerance.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x04) > 0;

        }

        private void UpdateOrientationControlMaskGUI()
        {
            //------------------------------------Orient--------------------------------
            chk_InspectOrient_ForMO.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x40000000) > 0;
        }

        private void UpdateOrientPadControlMaskGUI()
        {
            //------------------------------------OrientPad--------------------------------

            chk_WantInspectOrientAngleTolerance_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x8000000) > 0;
            chk_WantInspectOrientXTolerance_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x10000000) > 0;
            chk_WantInspectOrientYTolerance_Pad.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x20000000) > 0;

        }
        private void UpdateMarkControlMaskGUI()
        {
            //------------------------------------Mark--------------------------------

            chk_CheckMark.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x01) > 0;
            chk_ExcessMarkCharArea.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x02) > 0;
            chk_GroupExcessMark.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x200) > 0;
            chk_MarkAverageGrayValue.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x400) > 0;
            chk_ExtraMark.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x04) > 0;
            chk_ExtraMarkUncheckArea.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x08) > 0;
            chk_GroupExtraMark.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x10) > 0;
            chk_MissingMark.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x20) > 0;
            chk_BrokenMark.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x40) > 0;
            chk_TextShifted.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x80) > 0;
            chk_JointMark.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x100) > 0;
            chk_MarkAngle.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x2000) > 0;

            pnl_GroupExcessMark.Visible = m_smVisionInfo.g_blnWantCheckMarkTotalExcess;
            pnl_MarkAverageGrayValue.Visible = m_smVisionInfo.g_blnWantCheckMarkAverageGrayValue;
            pnl_BrokenMark.Visible = m_smVisionInfo.g_blnWantCheckMarkBroken;

            if (!m_smVisionInfo.g_blnWantGauge)
            {
                if (m_smVisionInfo.g_strVisionName.Contains("Pkg"))
                {
                    chk_ExtraMarkUncheckArea.Enabled = false;

                    if (chk_ExtraMarkUncheckArea.Checked)
                    {
                        chk_ExtraMarkUncheckArea.Checked = false;

                        m_smVisionInfo.g_intOptionControlMask &= ~0x08;
                    }

                }
                else
                {
                    chk_ExtraMarkUncheckArea.Enabled = false;

                    chk_TextShifted.Enabled = false;

                    if (chk_ExtraMarkUncheckArea.Checked || chk_TextShifted.Checked)
                    {
                        chk_ExtraMarkUncheckArea.Checked = false;
                        chk_TextShifted.Checked = false;

                        m_smVisionInfo.g_intOptionControlMask &= ~0x88;
                    }
                }
            }

            //------------------------------------Pin1--------------------------------

            if (m_smVisionInfo.g_blnWantPin1)
            {
                chk_WantInspectPin1.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x800) > 0;
            }

        }

        private void UpdateLeadControlMaskGUI()
        {
            //------------------------------------Lead--------------------------------
            chk_InspectLead.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0x40000) > 0;
            chk_WidthHeight_Lead.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0xC0) > 0;
            chk_OffSet_Lead.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0x100) > 0;
            //chk_Skew_Lead.Visible = !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance; //2020-08-12 ZJYEOH : Hide Skew if use package to base method
            chk_Skew_Lead.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0x20000) > 0;
            chk_Pitch_Lead.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0x600) > 0;
            chk_Variance_Lead.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0x800) > 0;
            chk_AverageGrayValue_Lead.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0x10000) > 0;

            if (!m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_blnWantUseAverageGrayValueMethod)
            {
                pnl_AverageGrayValue_Lead.Visible = false;
            }

            chk_Span_Lead.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0x1000) > 0;
            chk_CheckForeignMaterialTotalArea_Lead.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0x2000) > 0;
            chk_CheckForeignMaterialArea_Lead.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0x4000) > 0;
            chk_CheckForeignMaterialLength_Lead.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0x8000) > 0;
            chk_BaseLeadOffset.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0x80000) > 0;
            chk_BaseLeadArea.Checked = (m_smVisionInfo.g_intLeadOptionControlMask & 0x100000) > 0;

            if (!m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_blnWantInspectBaseLead)
            {
                pnl_BaseLeadOffset.Visible = pnl_BaseLeadArea.Visible = false;
            }

        }
        private void UpdateColorPackageControlMaskGUI()
        {
            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count == 0)
            {
                if (tab_VisionControl.TabPages.Contains(tp_ColorPackage))
                    tab_VisionControl.TabPages.Remove(tp_ColorPackage);
            }

            List<int> arrColorDefectSkipNo = new List<int>();
            int intTotalColorCount = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetColorDefectCount(ref arrColorDefectSkipNo);

            for (int i = 0; i < m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count; i++)
            {
                //if (arrColorDefectSkipNo.Contains(i))
                //    continue;

                if (i == 0)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    {
                        chk_ColorPackage1Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " Length";
                        chk_ColorPackage1Area.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " Area";
                    }
                    else
                    {
                        chk_ColorPackage1Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " 长度";
                        chk_ColorPackage1Area.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " 面积";
                    }
                    if (!arrColorDefectSkipNo.Contains(i))
                    {
                        pnl_PackageColorDefect1.Visible = true;
                        //chk_ColorPackage1Length.Visible = true;
                        chk_ColorPackage1Length.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x01) > 0);
                        //chk_ColorPackage1Area.Visible = true;
                        chk_ColorPackage1Area.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x02) > 0);
                    }
                }
                if (i == 1)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    {
                        chk_ColorPackage2Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " Length";
                        chk_ColorPackage2Area.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " Area";
                    }
                    else
                    {
                        chk_ColorPackage2Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " 长度";
                        chk_ColorPackage2Area.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " 面积";
                    }
                    if (!arrColorDefectSkipNo.Contains(i))
                    {
                        pnl_PackageColorDefect2.Visible = true;
                        //chk_ColorPackage2Length.Visible = true;
                        chk_ColorPackage2Length.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x04) > 0);
                        //chk_ColorPackage2Area.Visible = true;
                        chk_ColorPackage2Area.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x08) > 0);
                    }
                }
                if (i == 2)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    {
                        chk_ColorPackage3Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " Length";
                        chk_ColorPackage3Area.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " Area";
                    }
                    else
                    {
                        chk_ColorPackage3Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " 长度";
                        chk_ColorPackage3Area.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " 面积";
                    }
                    if (!arrColorDefectSkipNo.Contains(i))
                    {
                        pnl_PackageColorDefect3.Visible = true;
                        //chk_ColorPackage3Length.Visible = true;
                        chk_ColorPackage3Length.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x10) > 0);
                        //chk_ColorPackage3Area.Visible = true;
                        chk_ColorPackage3Area.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x20) > 0);
                    }
                }
                if (i == 3)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    {
                        chk_ColorPackage4Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " Length";
                        chk_ColorPackage4Area.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " Area";
                    }
                    else
                    {
                        chk_ColorPackage4Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " 长度";
                        chk_ColorPackage4Area.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " 面积";
                    }
                    if (!arrColorDefectSkipNo.Contains(i))
                    {
                        pnl_PackageColorDefect4.Visible = true;
                        //chk_ColorPackage4Length.Visible = true;
                        chk_ColorPackage4Length.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x40) > 0);
                        //chk_ColorPackage4Area.Visible = true;
                        chk_ColorPackage4Area.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x80) > 0);
                    }
                }
                if (i == 4)
                {
                    if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                    {
                        chk_ColorPackage5Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " Length";
                        chk_ColorPackage5Area.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " Area";
                    }
                    else
                    {
                        chk_ColorPackage5Length.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " 长度";
                        chk_ColorPackage5Area.Text = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColorThresName[i] + " 面积";
                    }
                    if (!arrColorDefectSkipNo.Contains(i))
                    {
                        pnl_PackageColorDefect5.Visible = true;
                        //chk_ColorPackage5Length.Visible = true;
                        chk_ColorPackage5Length.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x100) > 0);
                        //chk_ColorPackage5Area.Visible = true;
                        chk_ColorPackage5Area.Checked = ((m_smVisionInfo.g_intOptionControlMask2 & 0x200) > 0);
                    }
                }
            }
        }
        private void UpdatePackageControlMaskGUI()
        {
            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomPosition") || (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomOrient"))
            {
                chk_CheckPkgSize2.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x2000) > 0;
                chk_InspectPackage2.Visible = false;
                chk_BrightFieldArea.Visible = false;
                chk_BrightFieldLength.Visible = false;
                chk_DarkFieldArea.Visible = false;
                chk_DarkFieldLength.Visible = false;
                chk_DarkField2Area.Visible = false;
                chk_DarkField2Length.Visible = false;
                chk_CrackDarkFieldArea.Visible = false;
                chk_CrackDarkFieldLength.Visible = false;
                chk_VoidDarkFieldArea.Visible = false;
                chk_VoidDarkFieldLength.Visible = false;
                chk_MoldFlashBrightFieldArea.Visible = false;
                chk_CheckChippedOffBright_Area.Visible = false;
                chk_CheckChippedOffBright_Length.Visible = false;
                chk_CheckChippedOffDark_Area.Visible = false;
                chk_CheckChippedOffDark_Length.Visible = false;
                chk_CheckPkgAngle.Visible = false;
                return;
            }

            chk_InspectPackage2.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x1000) > 0;
            chk_CheckPkgSize2.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x2000) > 0;
            chk_BrightFieldArea.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x4000) > 0;
            chk_BrightFieldLength.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x8000) > 0;
            chk_DarkFieldArea.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x10000) > 0;
            chk_DarkFieldLength.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x20000) > 0;
            chk_DarkField2Area.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x4000000) > 0;
            chk_DarkField2Length.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x8000000) > 0;
            chk_DarkField3Area.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x10000000) > 0;
            chk_DarkField3Length.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x20000000) > 0;
            chk_DarkField4Area.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x100000) > 0;
            chk_DarkField4Length.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x200000) > 0;
            chk_CrackDarkFieldArea.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x40000) > 0;
            chk_CrackDarkFieldLength.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x80000) > 0;
            chk_VoidDarkFieldArea.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x100000) > 0;
            chk_VoidDarkFieldLength.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x200000) > 0;
            chk_MoldFlashBrightFieldArea.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x400000) > 0;
            chk_CheckChippedOffBright_Area.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x800000) > 0;
            chk_CheckChippedOffBright_Length.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x40000000) > 0;
            chk_CheckChippedOffDark_Area.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x1000000) > 0;
            chk_CheckChippedOffDark_Length.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x80000000) > 0;
            chk_CheckPkgAngle.Checked = (m_smVisionInfo.g_intPkgOptionControlMask & 0x2000000) > 0;

            int intAddY = 25;
            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField2DefectSetting)
            {
                pnl_DarkField2Area.Visible = false;
                pnl_DarkField2Length.Visible = false;
            }

            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField3DefectSetting)
            {
                pnl_DarkField3Area.Visible = false;
                pnl_DarkField3Length.Visible = false;
            }

            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateDarkField4DefectSetting)
            {
                pnl_DarkField4Area.Visible = false;
                pnl_DarkField4Length.Visible = false;
            }

            //else
            //{
            //    chk_DarkField2Area.Location = new Point(chk_DarkFieldLength.Location.X, chk_DarkFieldLength.Location.Y + intAddY);
            //    intAddY += 25;
            //    chk_DarkField2Length.Location = new Point(chk_DarkFieldLength.Location.X, chk_DarkFieldLength.Location.Y + intAddY);
            //    intAddY += 25;
            //}

            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
            {
                pnl_CrackDarkFieldArea.Visible = false;
                pnl_CrackDarkFieldLength.Visible = false;
            }
            //else
            //{
            //    chk_CrackDarkFieldArea.Location = new Point(chk_DarkFieldLength.Location.X, chk_DarkFieldLength.Location.Y + intAddY);
            //    intAddY += 25;
            //    chk_CrackDarkFieldLength.Location = new Point(chk_DarkFieldLength.Location.X, chk_DarkFieldLength.Location.Y + intAddY);
            //    intAddY += 25;
            //}

            //if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateVoidDefectSetting)
            //{
            //    chk_VoidDarkFieldArea.Location = new Point(chk_DarkFieldLength.Location.X, chk_DarkFieldLength.Location.Y + intAddY);
            //    intAddY += 25;
            //    chk_VoidDarkFieldLength.Location = new Point(chk_DarkFieldLength.Location.X, chk_DarkFieldLength.Location.Y + intAddY);
            //    intAddY += 25;
            //}
            //else
            //{
            //    chk_VoidDarkFieldArea.Visible = false;
            //    chk_VoidDarkFieldLength.Visible = false;
            //}

            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
            {
                pnl_MoldFlashBrightFieldArea.Visible = false;
            }
            //else
            //{
            //    chk_MoldFlashBrightFieldArea.Location = new Point(chk_DarkFieldLength.Location.X, chk_DarkFieldLength.Location.Y + intAddY);
            //    intAddY += 25;
            //}

            if (!m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
            {
                pnl_CheckChippedOffBright_Area.Visible = false;
                pnl_CheckChippedOffBright_Length.Visible = false;
                pnl_CheckChippedOffDark_Area.Visible = false;
                pnl_CheckChippedOffDark_Length.Visible = false;
            }
            //else
            //{
            //    chk_CheckChippedOffBright_Area.Location = new Point(chk_DarkFieldLength.Location.X, chk_DarkFieldLength.Location.Y + intAddY);
            //    intAddY += 25;
            //    chk_CheckChippedOffDark_Area.Location = new Point(chk_DarkFieldLength.Location.X, chk_DarkFieldLength.Location.Y + intAddY);
            //    intAddY += 25;
            //}

        }


        private void SaveControlSettings(string strFolderPath)
        {
            XmlParser objFile;
            objFile = new XmlParser(strFolderPath + "ControlSetting.xml");
            objFile.WriteSectionElement("ControlSetting", false);
            objFile.WriteElement1Value("ControlMask", m_smVisionInfo.g_intOptionControlMask, "Control Mask", true);
            objFile.WriteElement1Value("ControlMask2", m_smVisionInfo.g_intOptionControlMask2, "Control Mask 2", true);
            objFile.WriteElement1Value("ControlMask3", m_smVisionInfo.g_intOptionControlMask3, "Control Mask 3", true);
            objFile.WriteElement1Value("ControlMask4", m_smVisionInfo.g_intOptionControlMask4, "Control Mask 4", true);
            objFile.WriteElement1Value("ControlMask5", m_smVisionInfo.g_intOptionControlMask5, "Control Mask 5", true);
            objFile.WriteElement1Value("PkgControlMask", m_smVisionInfo.g_intPkgOptionControlMask, "Package Control Mask", true);
            objFile.WriteElement1Value("PkgControlMask2", m_smVisionInfo.g_intPkgOptionControlMask2, "Package Control Mask 2", true);
            objFile.WriteElement1Value("LeadControlMask", m_smVisionInfo.g_intLeadOptionControlMask, "Lead Control Mask", true);
            objFile.WriteEndElement();
        }


        private void chk_FailMask_Click(object sender, EventArgs e)
        {
            SetPadControlMask(sender);
        }

        private void chk_SidepadFailMask_Click(object sender, EventArgs e)
        {
            SetSidePadControlMask(sender);
        }

        private void tp_Package_Click(object sender, EventArgs e)
        {

        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            //string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
            //                m_smVisionInfo.g_strVisionFolderName + "\\";
            //

            //switch (m_smVisionInfo.g_strVisionName)
            //{
            //    case "Mark":
            //    case "MarkOrient":
            //    case "InPocket":
            //    case "MarkPkg":
            //    case "MOPkg":
            //    case "MOLiPkg":
            //    case "MOLi":
            //    case "InPocketPkg":
            //    case "InPocketPkgPos":
            //        STDeviceEdit.CopySettingFile(strPath + "Mark\\Template\\", "ControlSetting.xml");
            //        SaveControlSettings(strPath + "Mark\\Template\\");
            //        STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Mark Option", strPath + "Mark\\Template\\", "ControlSetting.xml");

            //        if ((m_intVisionType & 0x08) > 0)
            //        {
            //            STDeviceEdit.CopySettingFile(strPath + "Package\\", "\\Settings.xml");
            //            SaveControlSettings(strPath + "Package\\Template\\");
            //            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Package Option", strPath + "Package\\", "\\Settings.xml");
            //        }
            //        if ((m_intVisionType & 0x10) > 0)
            //        {
            //            STDeviceEdit.CopySettingFile(strPath + "Lead\\", "Template\\ControlSetting.xml");
            //            SaveControlSettings(strPath + "Lead\\Template\\");
            //            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Lead Option", strPath + "Lead\\", "Template\\ControlSetting.xml");
            //        }
            //        break;
            //    case "Package":
            //        if ((m_intVisionType & 0x08) > 0)
            //        {
            //            STDeviceEdit.CopySettingFile(strPath + "Package\\", "\\Settings.xml");
            //            SaveControlSettings(strPath + "Package\\Template\\");
            //            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Package Option", strPath + "Package\\", "\\Settings.xml");
            //        }
            //        break;
            //    case "Pad":
            //    case "PadPos":
            //    case "PadPkg":
            //    case "PadPkgPos":
            //    case "Pad5S":
            //    case "Pad5SPos":
            //    case "Pad5SPkg":
            //    case "Pad5SPkgPos":
            //        {
            //            STDeviceEdit.CopySettingFile(strPath + "Pad\\Template\\", "ControlSetting.xml");
            //            SaveControlSettings(strPath + "Pad\\Template\\");
            //            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Option", strPath + "Pad\\Template\\", "ControlSetting.xml");
            //        }

            //        break;   
            //case "Li3D":
            //        {
            //            STDeviceEdit.CopySettingFile(strPath + "Lead3D\\Template\\", "ControlSetting.xml");
            //            SaveControlSettings(strPath + "Lead3D\\Template\\");
            //            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Lead3D Option", strPath + "Lead3D\\Template\\", "ControlSetting.xml");
            //        }
            //        break;
            //case "Li3DPkg":
            //        {
            //            STDeviceEdit.CopySettingFile(strPath + "Lead3D\\Template\\", "ControlSetting.xml");
            //            SaveControlSettings(strPath + "Lead3D\\Template\\");
            //            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Lead3D Option", strPath + "Lead3D\\Template\\", "ControlSetting.xml");
            //        }
            //        break;
            //    case "Seal":
            //        break;
            //    case "UnitPresent":
            //        break;
            //}

            //if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
            //    m_smProductionInfo.g_blnSaveRecipeToServer = true;

            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            m_smVisionInfo.g_intOptionControlMask = m_intOptionControlMaskPrev;
            m_smVisionInfo.g_intPkgOptionControlMask = m_intPkgOptionControlMaskPrev;
            m_smVisionInfo.g_intPkgOptionControlMask2 = m_intPkgOptionControlMask2Prev;

            this.Close();
            this.Dispose();
        }

        private void InspectionOptionForm_Load(object sender, EventArgs e)
        {
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void InspectionOptionForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void tp_Mark_Click(object sender, EventArgs e)
        {

        }

        private void tp_Orient_Click(object sender, EventArgs e)
        {

        }

        private void chk_WantInspectPin1_Click(object sender, EventArgs e)
        {
            if (chk_WantInspectPin1.Checked)
                m_smVisionInfo.g_intOptionControlMask |= 0x800;
            else
                m_smVisionInfo.g_intOptionControlMask &= ~0x800;
        }
        private void chk_OrientFailMask_Click(object sender, EventArgs e)
        {
            if (sender == chk_WantInspectOrientAngleTolerance)
            {
                if (chk_WantInspectOrientAngleTolerance.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x01;

                    if (!chk_WantInspectOrientAngleTolerance.Checked)
                    {
                        chk_WantInspectOrientAngleTolerance.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x01;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x01;
            }
            else if (sender == chk_WantInspectOrientXTolerance)
            {
                if (chk_WantInspectOrientXTolerance.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x02;

                    if (!chk_WantInspectOrientXTolerance.Checked)
                    {
                        chk_WantInspectOrientXTolerance.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x02;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x02;
            }
            else if (sender == chk_WantInspectOrientYTolerance)
            {
                if (chk_WantInspectOrientYTolerance.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x04;

                    if (!chk_WantInspectOrientYTolerance.Checked)
                    {
                        chk_WantInspectOrientYTolerance.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x04;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x04;
            }
        }
        private void chk_OrientPadFailMask_Click(object sender, EventArgs e)
        {
            if (sender == chk_WantInspectOrientAngleTolerance_Pad)
            {
                if (chk_WantInspectOrientAngleTolerance_Pad.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x8000000;

                    if (!chk_WantInspectOrientAngleTolerance_Pad.Checked)
                    {
                        chk_WantInspectOrientAngleTolerance_Pad.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x8000000;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x8000000;
            }
            else if (sender == chk_WantInspectOrientXTolerance_Pad)
            {
                if (chk_WantInspectOrientXTolerance_Pad.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x10000000;

                    if (!chk_WantInspectOrientXTolerance_Pad.Checked)
                    {
                        chk_WantInspectOrientXTolerance_Pad.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x10000000;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x10000000;
            }
            else if (sender == chk_WantInspectOrientYTolerance_Pad)
            {
                if (chk_WantInspectOrientYTolerance_Pad.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x20000000;

                    if (!chk_WantInspectOrientYTolerance_Pad.Checked)
                    {
                        chk_WantInspectOrientYTolerance_Pad.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x20000000;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x20000000;
            }
        }
        private void chk_MarkFailMask_Click(object sender, EventArgs e)
        {
            if (sender == chk_CheckMark)
            {
                if (chk_CheckMark.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x01;
                else
                {
                    chk_ExcessMarkCharArea.Checked = false;
                    chk_GroupExcessMark.Checked = false;
                    chk_MarkAverageGrayValue.Checked = false;
                    chk_ExtraMark.Checked = false;
                    chk_ExtraMarkUncheckArea.Checked = false;
                    chk_GroupExtraMark.Checked = false;
                    chk_MissingMark.Checked = false;
                    chk_BrokenMark.Checked = false;
                    chk_TextShifted.Checked = false;
                    chk_JointMark.Checked = false;
                    chk_MarkAngle.Checked = false;
                    m_smVisionInfo.g_intOptionControlMask &= ~0x23FF; //~0x1FF 2020-05-29 ZJYEOH : Add 2 in front for disable mark angle
                }
            }
            else if (sender == chk_ExcessMarkCharArea)
            {
                if (chk_ExcessMarkCharArea.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x02;

                    if (!chk_CheckMark.Checked)
                    {
                        chk_CheckMark.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x01;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x02;
            }
            else if (sender == chk_GroupExcessMark)
            {
                if (chk_GroupExcessMark.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x200;

                    if (!chk_CheckMark.Checked)
                    {
                        chk_CheckMark.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x01;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x200;
            }
            else if (sender == chk_MarkAverageGrayValue)
            {
                if (chk_MarkAverageGrayValue.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x400;

                    if (!chk_CheckMark.Checked)
                    {
                        chk_CheckMark.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x01;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x400;
            }
            else if (sender == chk_ExtraMark)
            {
                if (chk_ExtraMark.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x04;

                    if (!chk_CheckMark.Checked)
                    {
                        chk_CheckMark.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x01;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x04;
            }
            else if (sender == chk_ExtraMarkUncheckArea)
            {
                if (chk_ExtraMarkUncheckArea.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x08;

                    if (!chk_CheckMark.Checked)
                    {
                        chk_CheckMark.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x01;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x08;
            }
            else if (sender == chk_GroupExtraMark)
            {
                if (chk_GroupExtraMark.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x10;

                    if (!chk_CheckMark.Checked)
                    {
                        chk_CheckMark.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x01;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x10;
            }
            else if (sender == chk_MissingMark)
            {
                if (chk_MissingMark.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x20;

                    if (!chk_CheckMark.Checked)
                    {
                        chk_CheckMark.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x01;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x20;
            }
            else if (sender == chk_BrokenMark)
            {
                if (chk_BrokenMark.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x40;

                    if (!chk_CheckMark.Checked)
                    {
                        chk_CheckMark.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x01;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x40;
            }
            else if (sender == chk_TextShifted)
            {
                if (chk_TextShifted.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x80;

                    if (!chk_CheckMark.Checked)
                    {
                        chk_CheckMark.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x01;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x80;
            }
            else if (sender == chk_JointMark)
            {
                if (chk_JointMark.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x100;

                    if (!chk_CheckMark.Checked)
                    {
                        chk_CheckMark.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x01;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x100;
            }
            else if (sender == chk_MarkAngle)
            {
                if (chk_MarkAngle.Checked)
                {
                    m_smVisionInfo.g_intOptionControlMask |= 0x2000;

                    if (!chk_CheckMark.Checked)
                    {
                        chk_CheckMark.Checked = true;
                        m_smVisionInfo.g_intOptionControlMask |= 0x01;
                    }
                }
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x2000;
            }
        }


        private void chk_CenterPkgPadFailMask_Click(object sender, EventArgs e)
        {
            SetCenterPadPackageControlMask_Simple(sender);
        }

        private void chk_SidePkgPadFailMask_Click(object sender, EventArgs e)
        {
            SetSidePadPackageControlMask_Simple(sender);

        }

        private void chk_LeadFailMask_Click(object sender, EventArgs e)
        {
            SetLeadControlMask(sender);
        }

        private void chk_PkgFailMask_Click(object sender, EventArgs e)
        {
            SetPkgControlMask_Simple(sender);
        }

        private void chk_Lead3DFailMask_Click(object sender, EventArgs e)
        {
            SetLead3DControlMask(sender);
        }

        private void chk_Lead3DPkgFailMask_Click(object sender, EventArgs e)
        {
            SetLead3DPkgControlMask_Simple(sender);
        }
        private void chk_SealFailMask_Click(object sender, EventArgs e)
        {
            SetSealControlMask(sender);
        }
        private void timer_InspectionOption_Tick(object sender, EventArgs e)
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

        private void UpdateSealControlMaskGUI()
        {
            chk_CheckSealWidth.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x01) > 0;

            chk_CheckSealBubble.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x02) > 0;

            chk_CheckSealShifted.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x04) > 0;

            chk_CheckSealDistance.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x08) > 0;

            chk_CheckSealOverHeat.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x10) > 0;

            chk_CheckSealBroken.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x20) > 0;

            chk_CheckSealUnitPresent.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x40) > 0;

            chk_CheckSealUnitOrient.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x80) > 0;

            if (m_smVisionInfo.g_objSeal != null && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole)
            {
                chk_CheckSealSprocketHoleDistance.Checked = false;
                pnl_CheckSealSprocketHoleDistance.Visible = false;
            }
            else
            {
                chk_CheckSealSprocketHoleDistance.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x100) > 0;
                pnl_CheckSealSprocketHoleDistance.Visible = true;
            }

            if (m_smVisionInfo.g_objSeal != null && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
            {
                chk_CheckSealSprocketHoleDiameter.Checked = false;
                pnl_CheckSealSprocketHoleDiameter.Visible = false;
                chk_CheckSealSprocketHoleDefect.Checked = false;
                pnl_CheckSealSprocketHoleDefect.Visible = false;
            }
            else
            {
                chk_CheckSealSprocketHoleDiameter.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x200) > 0;
                pnl_CheckSealSprocketHoleDiameter.Visible = true;
                chk_CheckSealSprocketHoleDefect.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x400) > 0;
                pnl_CheckSealSprocketHoleDefect.Visible = true;
            }

            if (m_smVisionInfo.g_objSeal != null && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
            {
                chk_CheckSealSprocketHoleBroken.Checked = false;
                pnl_CheckSealSprocketHoleBroken.Visible = false;
                chk_CheckSealSprocketHoleRoundness.Checked = false;
                pnl_CheckSealSprocketHoleRoundness.Visible = false;
            }
            else
            {
                chk_CheckSealSprocketHoleBroken.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x800) > 0;
                pnl_CheckSealSprocketHoleBroken.Visible = true;
                chk_CheckSealSprocketHoleRoundness.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x1000) > 0;
                pnl_CheckSealSprocketHoleRoundness.Visible = true;
            }

            if (m_smVisionInfo.g_objSeal != null && !m_smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness)
            {
                chk_CheckSealEdgeStraightness.Checked = false;
                pnl_CheckSealEdgeStraightness.Visible = false;
            }
            else
            {
                chk_CheckSealEdgeStraightness.Checked = (m_smVisionInfo.g_intOptionControlMask & 0x2000) > 0;
                pnl_CheckSealEdgeStraightness.Visible = true;
            }
        }

        private void SetSealControlMask(object sender)
        {
            if (sender == chk_CheckSealWidth)
            {
                if (chk_CheckSealWidth.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x01;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x01;
            }
            else if (sender == chk_CheckSealBubble)
            {
                if (chk_CheckSealBubble.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x02;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x02;
            }
            else if (sender == chk_CheckSealShifted)
            {
                if (chk_CheckSealShifted.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x04;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x04;
            }
            else if (sender == chk_CheckSealDistance)
            {
                if (chk_CheckSealDistance.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x08;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x08;
            }
            else if (sender == chk_CheckSealOverHeat)
            {
                if (chk_CheckSealOverHeat.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x10;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x10;
            }
            else if (sender == chk_CheckSealBroken)
            {
                if (chk_CheckSealBroken.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x20;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x20;
            }
            else if (sender == chk_CheckSealUnitPresent)
            {
                if (chk_CheckSealUnitPresent.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x40;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x40;
            }
            else if (sender == chk_CheckSealUnitOrient)
            {
                if (chk_CheckSealUnitOrient.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x80;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x80;
            }
            else if (sender == chk_CheckSealSprocketHoleDistance)
            {
                if (chk_CheckSealSprocketHoleDistance.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x100;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x100;
            }
            else if (sender == chk_CheckSealSprocketHoleDiameter)
            {
                if (chk_CheckSealSprocketHoleDiameter.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x200;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x200;
            }
            else if (sender == chk_CheckSealSprocketHoleDefect)
            {
                if (chk_CheckSealSprocketHoleDefect.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x400;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x400;
            }
            else if (sender == chk_CheckSealSprocketHoleBroken)
            {
                if (chk_CheckSealSprocketHoleBroken.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x800;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x800;
            }
            else if (sender == chk_CheckSealSprocketHoleRoundness)
            {
                if (chk_CheckSealSprocketHoleRoundness.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x1000;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x1000;
            }
            else if (sender == chk_CheckSealEdgeStraightness)
            {
                if (chk_CheckSealEdgeStraightness.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x2000;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x2000;
            }
        }

        private void chk_OrientationFailMask_Click(object sender, EventArgs e)
        {
            if (sender == chk_InspectOrient_ForMO)
            {
                if (chk_InspectOrient_ForMO.Checked)
                    m_smVisionInfo.g_intOptionControlMask |= 0x40000000;
                else
                    m_smVisionInfo.g_intOptionControlMask &= ~0x40000000;
            }
        }

        private void chk_CenterColorPadFailMask_Click(object sender, EventArgs e)
        {
            SetCenterColorPadControlMask(sender);
        }

        private void chk_SideColorPadFailMask_Top_Click(object sender, EventArgs e)
        {
            SetSideColorPadControlMask_Top(sender);
        }
        private void chk_SideColorPadFailMask_Right_Click(object sender, EventArgs e)
        {
            SetSideColorPadControlMask_Right(sender);
        }
        private void chk_SideColorPadFailMask_Bottom_Click(object sender, EventArgs e)
        {
            SetSideColorPadControlMask_Bottom(sender);
        }
        private void chk_SideColorPadFailMask_Left_Click(object sender, EventArgs e)
        {
            SetSideColorPadControlMask_Left(sender);
        }
        private void chk_ColorPackageFailMask_Click(object sender, EventArgs e)
        {
            SetColorPackageControlMask(sender);
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel9_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}