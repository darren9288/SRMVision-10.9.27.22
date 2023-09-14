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
using System.IO;

namespace VisionProcessForm
{
    public partial class InspectionOptionForm2 : Form
    {
        #region Member Variables
        private bool m_blnFailMaskOrientation_Previous;
        private bool m_blnFailMaskOrientAngle_Previous;
        private bool m_blnFailMaskOrientPosX_Previous;
        private bool m_blnFailMaskOrientPosY_Previous;
        private bool m_blnWantCheckMark_Previous;
        private int m_intFailMaskMark_Previous;
        private int m_intFailMaskPackage_Previous;
        private bool m_blnFailMaskPackageDefect_Previous;
        private bool[] m_arrFailMaskDefectLength_Previous = new bool[9];
        private bool[] m_arrFailMaskDefectArea_Previous = new bool[10];
        private List<bool> m_arrFailMaskPin1_Previous = new List<bool>();
        private bool m_blnFailMaskInspectLead_Previous;
        private int m_intFailMaskLead_Previous;
        private bool m_blnFailMaskLeadExtraLength_Previous;
        private bool m_blnFailMaskLeadExtraArea_Previous;
        private bool m_blnFailMaskInspectPad_Previous;
        private int m_intFailMaskCenterPad_Previous;
        private bool m_blnFailMaskCenterPadExtraLength_Previous;
        private bool m_blnFailMaskCenterPadExtraArea_Previous;
        private bool m_blnFailMaskCenterPadBrokenLength_Previous;
        private bool m_blnFailMaskCenterPadBrokenArea_Previous;
        private int m_intFailMaskSidePad_Previous;
        private bool m_blnFailMaskSidePadExtraLength_Previous;
        private bool m_blnFailMaskSidePadExtraArea_Previous;
        private bool m_blnFailMaskSidePadBrokenLength_Previous;
        private bool m_blnFailMaskSidePadBrokenArea_Previous;
        private int m_intFailMaskCenterPadPackage_Previous;
        private bool m_blnFailMaskCenterPadPackageDefect_Previous;
        private int m_intFailMaskSidePadPackage_Previous;
        private bool m_blnFailMaskSidePadPackageDefect_Previous;
        private int m_intFailMaskLead3D_Previous;
        private bool m_blnFailMaskLead3DExtraLength_Previous;
        private bool m_blnFailMaskLead3DExtraArea_Previous;
        private int m_intFailMaskLead3DPackage_Previous;
        private bool m_blnFailMaskLead3DPackageDefect_Previous;
        private int m_intFailMaskSeal_Previous;
        private int m_intFailMaskCenterColorPad_Previous;
        private int m_intFailMaskSideColorPad_Top_Previous;
        private int m_intFailMaskSideColorPad_Right_Previous;
        private int m_intFailMaskSideColorPad_Bottom_Previous;
        private int m_intFailMaskSideColorPad_Left_Previous;
        private int m_intFailMaskColorPackage_Previous;

        private bool m_blnInitDone = false;
        private int m_intUserGroup = 5;
        private int m_intVisionType = 0;    // Mask Bit 1:Orient, 2:Mark
        private string m_strSelectedRecipe;
        private bool m_blnWantSet1ToAll = false;
        private int m_intSettingType = 0;
        private long m_intOptionControlMaskPrev = 0;
        private long m_intPkgOptionControlMaskPrev = 0;
        private int m_intPkgOptionControlMask2Prev = 0;
        private int m_intLeadOptionControlMaskPrev = 0;

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        private bool m_blnMustDisableExtraMarkSide = false;

        #endregion

        public InspectionOptionForm2(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intVisionType)
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
            m_intLeadOptionControlMaskPrev = m_smVisionInfo.g_intLeadOptionControlMask;

            //DisableField();
            UpdateGUI();
            UpdateControlSetting();
            UpdateTabPage();
            DisableField2();
    
            pnl_GroupExcessMark.Visible = m_smVisionInfo.g_blnWantCheckMarkTotalExcess;
            pnl_MarkAverageGrayValue.Visible = m_smVisionInfo.g_blnWantCheckMarkAverageGrayValue;
            pnl_BrokenMark.Visible = m_smVisionInfo.g_blnWantCheckMarkBroken;
            
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

            strChild2 = "Inspection Option Advance Control";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_ControlSetting.Enabled = false;
            }
        }
        private int GetUserRightGroup_Child2(string Child1, string Child2)
        {
            //NewUserRight objUserRight = new NewUserRight(false);
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Orient":
                case "BottomOrient":
                case "BottomPosition":
                    return m_smCustomizeInfo.objNewUserRight.GetOrientationChild2Group(Child1, Child2);
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "Package":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetMarkOrientChild2Group(Child1, Child2);
                    break;
                case "IPMLi":
                case "IPMLiPkg":
                case "InPocket":
                case "InPocketPkg":
                case "InPocketPkgPos":
                    return m_smCustomizeInfo.objNewUserRight.GetInPocketChild2Group(Child1, Child2, m_smVisionInfo.g_intVisionNameNo);
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
                    return m_smCustomizeInfo.objNewUserRight.GetPadChild2Group(Child1, Child2);
                    break;
                case "Li3D":
                case "Li3DPkg":
                    return m_smCustomizeInfo.objNewUserRight.GetLead3DChild2Group(Child1, Child2);
                    break;
                case "Seal":
                    return m_smCustomizeInfo.objNewUserRight.GetSealChild2Group(Child1, Child2, m_smVisionInfo.g_intVisionNameNo);
                    break;
                case "Barcode":
                    return m_smCustomizeInfo.objNewUserRight.GetBarcodeChild2Group(Child1, Child2);
                    break;
            }

            return 1;
        }

        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            string strChild1 = "Option";
            string strChild2 = "Save Button";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
            }

            strChild2 = "Inspection Option Adv. Control Button";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                btn_ControlSetting.Enabled = false;
            }

            strChild2 = "Orient TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                tab_VisionControl.TabPages.Remove(tp_Orient);
                tab_VisionControl.TabPages.Remove(tp_OrientPad);
            }

            strChild2 = "Mark TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                tab_VisionControl.TabPages.Remove(tp_Mark);
            }

            strChild2 = "Pad TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                tab_VisionControl.TabPages.Remove(tp_Pad);
            }

            strChild2 = "Color TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                tab_VisionControl.TabPages.Remove(tp_ColorPad);
                tab_VisionControl.TabPages.Remove(tp_ColorPackage);
            }

            strChild2 = "Package TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                tab_VisionControl.TabPages.Remove(tp_PackageSimple);
                tab_VisionControl.TabPages.Remove(tp_PackagePadSimple);
                tab_VisionControl.TabPages.Remove(tp_Lead3DPkg);
            }

            strChild2 = "Seal TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                tab_VisionControl.TabPages.Remove(tp_Seal);
            }

            strChild2 = "Pin 1 TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                tab_VisionControl.TabPages.Remove(tp_Pin1);
            }

            strChild2 = "Lead TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                tab_VisionControl.TabPages.Remove(tp_Lead);
            }

            strChild2 = "Lead3D TabPage";
            if (m_intUserGroup > GetUserRightGroup_Child2(strChild1, strChild2))
            {
                tab_VisionControl.TabPages.Remove(tp_Lead3D);
            }
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

                        if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomPosition"))
                        {
                            tp_Orient.Text = "Posiiton";
                            chk_WantInspectOrientAngleTolerance.Text = "Unit Angle Tolerance";
                            chk_WantInspectOrientXTolerance.Text = "Unit X Tolerance";
                            chk_WantInspectOrientYTolerance.Text = "Unit Y Tolerance";

                            if (m_smCustomizeInfo.g_intLanguageCulture != 1)
                            {
                                tp_Orient.Text = "位置";
                                chk_WantInspectOrientAngleTolerance.Text = "单元角度容许度";
                                chk_WantInspectOrientXTolerance.Text = "单元X容许度";
                                chk_WantInspectOrientYTolerance.Text = "单元Y容许度";
                            }
                        }

                        if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomPosition") && m_smVisionInfo.g_blnOrientWantPackage
                            || (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomOrient") && m_smVisionInfo.g_blnOrientWantPackage)
                        {
                            tab_VisionControl.Controls.Add(tp_PackageSimple);
                            UpdatePackageFailMaskGUI();
                        }

                        UpdateOrientFailMaskGUI();
                    }
                    break;
                case "Mark":
                case "InPocket":
                case "IPMLi":
                    tab_VisionControl.Controls.Add(tp_Mark);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);
                    UpdateMultiTemplateGUI();
                    UpdateMarkFailMaskGUI();
                    if ((m_intVisionType & 0x10) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_Lead);
                        UpdateLeadFailMaskGUI();
                    }
                    m_intSettingType = 0;
                    break;
                case "MarkOrient":
                case "MOLi":
                    tab_VisionControl.Controls.Add(tp_Mark);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    if (m_smVisionInfo.g_blnWantDisplayOrientationInOptionForm)
                        tab_VisionControl.Controls.Add(tp_Orient_ForMO);

                    UpdateMultiTemplateGUI();
                    UpdateMarkFailMaskGUI();
                    if ((m_intVisionType & 0x10) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_Lead);
                        UpdateLeadFailMaskGUI();
                    }
                    m_intSettingType = 0;
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
                        UpdateLeadFailMaskGUI();
                    }
                    UpdateMultiTemplateGUI();
                    UpdateMarkFailMaskGUI();
                    UpdatePackageFailMaskGUI();
                    m_intSettingType = 0;

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPackage);
                        UpdateColorPackageFailMaskGUI();
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
                        UpdateLeadFailMaskGUI();
                    }
                    UpdateMultiTemplateGUI();
                    UpdateMarkFailMaskGUI();
                    UpdateOrientationFailMaskGUI();
                    UpdatePackageFailMaskGUI();
                    m_intSettingType = 0;

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPackage);
                        UpdateColorPackageFailMaskGUI();
                    }

                    break;

                case "Package":
                    tab_VisionControl.Controls.Add(tp_PackageSimple);
                    UpdatePackageFailMaskGUI();
                    m_intSettingType = 1;

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPackage);
                        UpdateColorPackageFailMaskGUI();
                    }

                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                    pnl_InspectPad.Visible = true;

                    tab_VisionControl.Controls.Add(tp_OrientPad);
                    tab_VisionControl.Controls.Add(tp_Pad);
                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_PackagePadSimple);
                        pnl_SidePad.Visible = false;
                        pnl_SidePadLabel.Visible = false;
                        pnl_SidePadPkg.Visible = false;
                        pnl_SidePadPkgLabel.Visible = false;
                        UpdateCenterPacakgePadFailMaskGUI();
                    }

                    pnl_SidePad.Visible = false;
                    pnl_SidePadLabel.Visible = false;

                    string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
              m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Settings.xml";

                    XmlParser objFileHandle = new XmlParser(strPath);
                    objFileHandle.GetFirstSection("Advanced");
                    chk_InspectPad.Checked = objFileHandle.GetValueAsBoolean("WantCheckPad", true);

                    UpdateOrientPadFailMaskGUI();
                    UpdatePadFailMaskGUI();
                    pnl_PageButton.Visible = false;
                    break;
                case "Pad":
                case "PadPos":
                    tab_VisionControl.Controls.Add(tp_Pad);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    pnl_SidePad.Visible = false;
                    pnl_SidePadLabel.Visible = false;

                    UpdatePadFailMaskGUI();

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPad);
                        UpdateCenterColorPadFailMaskGUI();
                        pnl_SidePadColorLabel_Top.Visible = false;
                        pnl_SidePadColor_Top.Visible = false;
                        pnl_SidePadColorLabel_Right.Visible = false;
                        pnl_SidePadColor_Right.Visible = false;
                        pnl_SidePadColorLabel_Bottom.Visible = false;
                        pnl_SidePadColor_Bottom.Visible = false;
                        pnl_SidePadColorLabel_Left.Visible = false;
                        pnl_SidePadColor_Left.Visible = false;
                    }

                    pnl_PageButton.Visible = false;
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

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        tab_VisionControl.Controls.Add(tp_ColorPad);
                        UpdateCenterColorPadFailMaskGUI();
                        pnl_SidePadColorLabel_Top.Visible = false;
                        pnl_SidePadColor_Top.Visible = false;
                        pnl_SidePadColorLabel_Right.Visible = false;
                        pnl_SidePadColor_Right.Visible = false;
                        pnl_SidePadColorLabel_Bottom.Visible = false;
                        pnl_SidePadColor_Bottom.Visible = false;
                        pnl_SidePadColorLabel_Left.Visible = false;
                        pnl_SidePadColor_Left.Visible = false;
                    }

                    UpdatePadFailMaskGUI();
                    UpdateCenterPacakgePadFailMaskGUI();
                    pnl_PageButton.Visible = false;
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
                        UpdateCenterColorPadFailMaskGUI();
                        if (m_smVisionInfo.g_blnCheck4Sides)
                            UpdateSideColorPadFailMaskGUI();
                    }

                    UpdatePadFailMaskGUI();
                    if (m_smVisionInfo.g_blnCheck4Sides)
                        UpdateSidePadFailMaskGUI();
                    pnl_PageButton.Visible = false;
                    break;
                case "Pad5SPkg":
                case "Pad5SPkgPos":
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

                    tab_VisionControl.Controls.Add(tp_PackagePadSimple);
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
                        UpdateCenterColorPadFailMaskGUI();
                        if (m_smVisionInfo.g_blnCheck4Sides)
                            UpdateSideColorPadFailMaskGUI();
                    }

                    UpdatePadFailMaskGUI();
                    if (m_smVisionInfo.g_blnCheck4Sides)
                        UpdateSidePadFailMaskGUI();
                    UpdateCenterPacakgePadFailMaskGUI();
                    if (m_smVisionInfo.g_blnCheck4Sides)
                        UpdateSidePacakgePadFailMaskGUI();
                    pnl_PageButton.Visible = false;
                    break;
                case "Li3D":
                    tab_VisionControl.Controls.Add(tp_Lead3D);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    UpdateLead3DFailMaskGUI();
                    pnl_PageButton.Visible = false;
                    break;
                case "Li3DPkg":
                    tab_VisionControl.Controls.Add(tp_Lead3D);
                    tab_VisionControl.Controls.Add(tp_Lead3DPkg);

                    if (m_smVisionInfo.g_blnWantPin1)
                        tab_VisionControl.Controls.Add(tp_Pin1);

                    UpdateLead3DFailMaskGUI();
                    UpdateLead3DPkgFailMaskGUI();
                    pnl_PageButton.Visible = false;
                    break;
                case "Seal":
                    tab_VisionControl.Controls.Add(tp_Seal);
                    UpdateSealFailMaskGUI();
                    pnl_PageButton.Visible = false;

                    if (m_smVisionInfo.g_objSeal != null && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHole)
                    {
                        pnl_CheckSealSprocketHoleDistance.Visible = false;
                    }

                    if (m_smVisionInfo.g_objSeal != null && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleDiameterAndDefect)
                    {
                        pnl_CheckSealSprocketHoleDiameter.Visible = false;
                        pnl_CheckSealSprocketHoleDefect.Visible = false;
                    }

                    if (m_smVisionInfo.g_objSeal != null && m_smVisionInfo.g_objSeal.ref_blnWantSkipSprocketHoleBrokenAndRoundness)
                    {
                        pnl_CheckSealSprocketHoleBroken.Visible = false;
                        pnl_CheckSealSprocketHoleRoundness.Visible = false;
                    }

                    if (m_smVisionInfo.g_objSeal != null && !m_smVisionInfo.g_objSeal.ref_blnWantCheckSealEdgeStraightness)
                    {
                        pnl_CheckSealEdgeStraightness.Visible = false;
                    }
                    break;
                case "UnitPresent":
                    break;
                default:
                    SRMMessageBox.Show("btn_Learn_Click -> There is no such vision module name " + m_smVisionInfo.g_strVisionName + " in this SRMVision software version.");
                    break;
            }

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

                m_smVisionInfo.g_arrPad[i].LoadPad(strPath + "Template.xml", strSectionName);

            }

            if (m_smVisionInfo.g_arrPin1 != null)
            {
                m_smVisionInfo.g_arrPin1[0].LoadTemplate(strPath);
            }
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

                m_smVisionInfo.g_arrLead3D[i].SaveLead3D(strFolderPath + "Template.xml",
                    false, strSectionName, true);
            }

            if (m_smVisionInfo.g_arrPin1 != null)
            {
                m_smVisionInfo.g_arrPin1[0].SavePin1Setting(strFolderPath);
            }
        }
        private void SaveSealSetting(string strFolderPath)
        {
            m_smVisionInfo.g_objSeal.SaveSeal(strFolderPath + "Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX);


        }
        /// <summary>
        /// Save pad settings to xml
        /// </summary>
        /// <param name="strFolderPath">xml folder path</param>
        /// <param name="blnNewRecipe"></param>
        private void SavePadSetting(string strFolderPath)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                 m_smVisionInfo.g_strVisionFolderName + "\\Pad\\Settings.xml";

            //STDeviceEdit.CopySettingFile(strPath, "");
            XmlParser objFileHandle = new XmlParser(strPath);
            objFileHandle.WriteSectionElement("Advanced");
            objFileHandle.WriteElement1Value("WantCheckPad", chk_InspectPad.Checked);
            m_smVisionInfo.g_blnCheckPad = chk_InspectPad.Checked;
            objFileHandle.WriteEndElement();

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

                m_smVisionInfo.g_arrPad[i].SavePad(strFolderPath + "Template.xml",
                    false, strSectionName, true);
            }

            if (m_smVisionInfo.g_arrPin1 != null)
            {
                m_smVisionInfo.g_arrPin1[0].SavePin1Setting(strFolderPath);
            }
        }
        private void SetCenterColorPadFailMask(object sender)
        {
            //EnablePadFailMaskGUI(chk_InspectPad.Checked);

            int intFailMask = m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask;

            if (sender == chk_ColorPad1Length_Center)
            {
                if (chk_ColorPad1Length_Center.Checked)
                { 
                    intFailMask |= 0x01;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Center.Text == chk_ColorPad2Length_Center.Text)
                        intFailMask |= 0x04;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Center.Text == chk_ColorPad3Length_Center.Text)
                        intFailMask |= 0x10;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Center.Text == chk_ColorPad4Length_Center.Text)
                        intFailMask |= 0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        intFailMask |= 0x100;
                }
                else
                { 
                    intFailMask &= ~0x01;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Center.Text == chk_ColorPad2Length_Center.Text)
                        intFailMask &= ~0x04;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Center.Text == chk_ColorPad3Length_Center.Text)
                        intFailMask &= ~0x10;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Center.Text == chk_ColorPad4Length_Center.Text)
                        intFailMask &= ~0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        intFailMask &= ~0x100;
                }
            }
            else if (sender == chk_ColorPad1Area_Center)
            {
                if (chk_ColorPad1Area_Center.Checked)
                { 
                    intFailMask |= 0x02;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Center.Text == chk_ColorPad2Area_Center.Text)
                        intFailMask |= 0x08;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Center.Text == chk_ColorPad3Area_Center.Text)
                        intFailMask |= 0x20;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Center.Text == chk_ColorPad4Area_Center.Text)
                        intFailMask |= 0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        intFailMask |= 0x200;
                }
                else
                { 
                    intFailMask &= ~0x02;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Center.Text == chk_ColorPad2Area_Center.Text)
                        intFailMask &= ~0x08;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Center.Text == chk_ColorPad3Area_Center.Text)
                        intFailMask &= ~0x20;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Center.Text == chk_ColorPad4Area_Center.Text)
                        intFailMask &= ~0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        intFailMask &= ~0x200;
                }
            }
            else if (sender == chk_ColorPad2Length_Center)
            {
                if (chk_ColorPad2Length_Center.Checked)
                { 
                    intFailMask |= 0x04;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Center.Text == chk_ColorPad3Length_Center.Text)
                        intFailMask |= 0x10;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Center.Text == chk_ColorPad4Length_Center.Text)
                        intFailMask |= 0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        intFailMask |= 0x100;
                }
                else
                { 
                    intFailMask &= ~0x04;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Center.Text == chk_ColorPad3Length_Center.Text)
                        intFailMask &= ~0x10;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Center.Text == chk_ColorPad4Length_Center.Text)
                        intFailMask &= ~0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        intFailMask &= ~0x100;
                }
            }
            else if (sender == chk_ColorPad2Area_Center)
            {
                if (chk_ColorPad2Area_Center.Checked)
                { 
                    intFailMask |= 0x08;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Center.Text == chk_ColorPad3Area_Center.Text)
                        intFailMask |= 0x20;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Center.Text == chk_ColorPad4Area_Center.Text)
                        intFailMask |= 0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        intFailMask |= 0x200;
                }
                else
                { 
                    intFailMask &= ~0x08;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Center.Text == chk_ColorPad3Area_Center.Text)
                        intFailMask &= ~0x20;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Center.Text == chk_ColorPad4Area_Center.Text)
                        intFailMask &= ~0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        intFailMask &= ~0x200;
                }
            }
            else if (sender == chk_ColorPad3Length_Center)
            {
                if (chk_ColorPad3Length_Center.Checked)
                { 
                    intFailMask |= 0x10;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Center.Text == chk_ColorPad4Length_Center.Text)
                        intFailMask |= 0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        intFailMask |= 0x100;
                }
                else
                { 
                    intFailMask &= ~0x10;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Center.Text == chk_ColorPad4Length_Center.Text)
                        intFailMask &= ~0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        intFailMask &= ~0x100;
                }
            }
            else if (sender == chk_ColorPad3Area_Center)
            {
                if (chk_ColorPad3Area_Center.Checked)
                { 
                    intFailMask |= 0x20;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Center.Text == chk_ColorPad4Area_Center.Text)
                        intFailMask |= 0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        intFailMask |= 0x200;
                }
                else
                { 
                    intFailMask &= ~0x20;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Center.Text == chk_ColorPad4Area_Center.Text)
                        intFailMask &= ~0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        intFailMask &= ~0x200;
                }
            }
            else if (sender == chk_ColorPad4Length_Center)
            {
                if (chk_ColorPad4Length_Center.Checked)
                { 
                    intFailMask |= 0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        intFailMask |= 0x100;
                }
                else
                {
                    intFailMask &= ~0x40;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Center.Text == chk_ColorPad5Length_Center.Text)
                        intFailMask &= ~0x100;
                }
            }
            else if (sender == chk_ColorPad4Area_Center)
            {
                if (chk_ColorPad4Area_Center.Checked)
                { 
                    intFailMask |= 0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        intFailMask |= 0x200;
                }
                else
                { 
                    intFailMask &= ~0x80;
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Center.Text == chk_ColorPad5Area_Center.Text)
                        intFailMask &= ~0x200;
                }
            }
            else if (sender == chk_ColorPad5Length_Center)
            {
                if (chk_ColorPad5Length_Center.Checked)
                    intFailMask |= 0x100;
                else
                    intFailMask &= ~0x100;

            }
            else if (sender == chk_ColorPad5Area_Center)
            {
                if (chk_ColorPad5Area_Center.Checked)
                    intFailMask |= 0x200;
                else
                    intFailMask &= ~0x200;
            }

            m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask = intFailMask;

        }
        private void SetSideColorPadFailMask_Top(object sender)
        {
            //EnablePadFailMaskGUI(chk_InspectPad.Checked);
            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[1].ref_intColorPadGroupIndex)
                    continue;

                int intFailMask = m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask;

                if (sender == chk_ColorPad1Length_Side_Top)
                {
                    if (chk_ColorPad1Length_Side_Top.Checked)
                    { 
                        intFailMask |= 0x01;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad2Length_Side_Top.Text)
                            intFailMask |= 0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad3Length_Side_Top.Text)
                            intFailMask |= 0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad4Length_Side_Top.Text)
                            intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x01;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad2Length_Side_Top.Text)
                            intFailMask &= ~0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad3Length_Side_Top.Text)
                            intFailMask &= ~0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad4Length_Side_Top.Text)
                            intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad1Area_Side_Top)
                {
                    if (chk_ColorPad1Area_Side_Top.Checked)
                    { 
                        intFailMask |= 0x02;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad2Area_Side_Top.Text)
                            intFailMask |= 0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad3Area_Side_Top.Text)
                            intFailMask |= 0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad4Area_Side_Top.Text)
                            intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x02;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad2Area_Side_Top.Text)
                            intFailMask &= ~0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad3Area_Side_Top.Text)
                            intFailMask &= ~0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad4Area_Side_Top.Text)
                            intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad2Length_Side_Top)
                {
                    if (chk_ColorPad2Length_Side_Top.Checked)
                    {
                        intFailMask |= 0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Top.Text == chk_ColorPad3Length_Side_Top.Text)
                            intFailMask |= 0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Top.Text == chk_ColorPad4Length_Side_Top.Text)
                            intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Top.Text == chk_ColorPad3Length_Side_Top.Text)
                            intFailMask &= ~0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Top.Text == chk_ColorPad4Length_Side_Top.Text)
                            intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad2Area_Side_Top)
                {
                    if (chk_ColorPad2Area_Side_Top.Checked)
                    { 
                        intFailMask |= 0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Top.Text == chk_ColorPad3Area_Side_Top.Text)
                            intFailMask |= 0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Top.Text == chk_ColorPad4Area_Side_Top.Text)
                            intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Top.Text == chk_ColorPad3Area_Side_Top.Text)
                            intFailMask &= ~0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Top.Text == chk_ColorPad4Area_Side_Top.Text)
                            intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad3Length_Side_Top)
                {
                    if (chk_ColorPad3Length_Side_Top.Checked)
                    { 
                        intFailMask |= 0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Top.Text == chk_ColorPad4Length_Side_Top.Text)
                            intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Top.Text == chk_ColorPad4Length_Side_Top.Text)
                            intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad3Area_Side_Top)
                {
                    if (chk_ColorPad3Area_Side_Top.Checked)
                    { 
                        intFailMask |= 0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Top.Text == chk_ColorPad4Area_Side_Top.Text)
                            intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Top.Text == chk_ColorPad4Area_Side_Top.Text)
                            intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad4Length_Side_Top)
                {
                    if (chk_ColorPad4Length_Side_Top.Checked)
                    { 
                        intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Top.Text == chk_ColorPad5Length_Side_Top.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad4Area_Side_Top)
                {
                    if (chk_ColorPad4Area_Side_Top.Checked)
                    { 
                        intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Top.Text == chk_ColorPad5Area_Side_Top.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad5Length_Side_Top)
                {
                    if (chk_ColorPad5Length_Side_Top.Checked)
                        intFailMask |= 0x100;
                    else
                        intFailMask &= ~0x100;

                }
                else if (sender == chk_ColorPad5Area_Side_Top)
                {
                    if (chk_ColorPad5Area_Side_Top.Checked)
                        intFailMask |= 0x200;
                    else
                        intFailMask &= ~0x200;
                }

                m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask = intFailMask;
            }
        }
        private void SetSideColorPadFailMask_Right(object sender)
        {
            //EnablePadFailMaskGUI(chk_InspectPad.Checked);
            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[2].ref_intColorPadGroupIndex)
                    continue;

                int intFailMask = m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask;

                if (sender == chk_ColorPad1Length_Side_Right)
                {
                    if (chk_ColorPad1Length_Side_Right.Checked)
                    { 
                        intFailMask |= 0x01;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad2Length_Side_Right.Text)
                            intFailMask |= 0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad3Length_Side_Right.Text)
                            intFailMask |= 0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad4Length_Side_Right.Text)
                            intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x01;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad2Length_Side_Right.Text)
                            intFailMask &= ~0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad3Length_Side_Right.Text)
                            intFailMask &= ~0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad4Length_Side_Right.Text)
                            intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad1Area_Side_Right)
                {
                    if (chk_ColorPad1Area_Side_Right.Checked)
                    { 
                        intFailMask |= 0x02;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad2Area_Side_Right.Text)
                            intFailMask |= 0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad3Area_Side_Right.Text)
                            intFailMask |= 0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad4Area_Side_Right.Text)
                            intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x02;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad2Area_Side_Right.Text)
                            intFailMask &= ~0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad3Area_Side_Right.Text)
                            intFailMask &= ~0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad4Area_Side_Right.Text)
                            intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad2Length_Side_Right)
                {
                    if (chk_ColorPad2Length_Side_Right.Checked)
                    { 
                        intFailMask |= 0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Right.Text == chk_ColorPad3Length_Side_Right.Text)
                            intFailMask |= 0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Right.Text == chk_ColorPad4Length_Side_Right.Text)
                            intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Right.Text == chk_ColorPad3Length_Side_Right.Text)
                            intFailMask &= ~0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Right.Text == chk_ColorPad4Length_Side_Right.Text)
                            intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad2Area_Side_Right)
                {
                    if (chk_ColorPad2Area_Side_Right.Checked)
                    { 
                        intFailMask |= 0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Right.Text == chk_ColorPad3Area_Side_Right.Text)
                            intFailMask |= 0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Right.Text == chk_ColorPad4Area_Side_Right.Text)
                            intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Right.Text == chk_ColorPad3Area_Side_Right.Text)
                            intFailMask &= ~0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Right.Text == chk_ColorPad4Area_Side_Right.Text)
                            intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad3Length_Side_Right)
                {
                    if (chk_ColorPad3Length_Side_Right.Checked)
                    { 
                        intFailMask |= 0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Right.Text == chk_ColorPad4Length_Side_Right.Text)
                            intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Right.Text == chk_ColorPad4Length_Side_Right.Text)
                            intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad3Area_Side_Right)
                {
                    if (chk_ColorPad3Area_Side_Right.Checked)
                    { 
                        intFailMask |= 0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Right.Text == chk_ColorPad4Area_Side_Right.Text)
                            intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Right.Text == chk_ColorPad4Area_Side_Right.Text)
                            intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad4Length_Side_Right)
                {
                    if (chk_ColorPad4Length_Side_Right.Checked)
                    { 
                        intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Right.Text == chk_ColorPad5Length_Side_Right.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad4Area_Side_Right)
                {
                    if (chk_ColorPad4Area_Side_Right.Checked)
                    { 
                        intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Right.Text == chk_ColorPad5Area_Side_Right.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad5Length_Side_Right)
                {
                    if (chk_ColorPad5Length_Side_Right.Checked)
                        intFailMask |= 0x100;
                    else
                        intFailMask &= ~0x100;

                }
                else if (sender == chk_ColorPad5Area_Side_Right)
                {
                    if (chk_ColorPad5Area_Side_Right.Checked)
                        intFailMask |= 0x200;
                    else
                        intFailMask &= ~0x200;
                }

                m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask = intFailMask;
            }
        }
        private void SetSideColorPadFailMask_Bottom(object sender)
        {
            //EnablePadFailMaskGUI(chk_InspectPad.Checked);
            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[3].ref_intColorPadGroupIndex)
                    continue;

                int intFailMask = m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask;

                if (sender == chk_ColorPad1Length_Side_Bottom)
                {
                    if (chk_ColorPad1Length_Side_Bottom.Checked)
                    { 
                        intFailMask |= 0x01;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad2Length_Side_Bottom.Text)
                            intFailMask |= 0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad3Length_Side_Bottom.Text)
                            intFailMask |= 0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad4Length_Side_Bottom.Text)
                            intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x01;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad2Length_Side_Bottom.Text)
                            intFailMask &= ~0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad3Length_Side_Bottom.Text)
                            intFailMask &= ~0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad4Length_Side_Bottom.Text)
                            intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad1Area_Side_Bottom)
                {
                    if (chk_ColorPad1Area_Side_Bottom.Checked)
                    { 
                        intFailMask |= 0x02;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad2Area_Side_Bottom.Text)
                            intFailMask |= 0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad3Area_Side_Bottom.Text)
                            intFailMask |= 0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad4Area_Side_Bottom.Text)
                            intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x02;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad2Area_Side_Bottom.Text)
                            intFailMask &= ~0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad3Area_Side_Bottom.Text)
                            intFailMask &= ~0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad4Area_Side_Bottom.Text)
                            intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad2Length_Side_Bottom)
                {
                    if (chk_ColorPad2Length_Side_Bottom.Checked)
                    { 
                        intFailMask |= 0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Bottom.Text == chk_ColorPad3Length_Side_Bottom.Text)
                            intFailMask |= 0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Bottom.Text == chk_ColorPad4Length_Side_Bottom.Text)
                            intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Bottom.Text == chk_ColorPad3Length_Side_Bottom.Text)
                            intFailMask &= ~0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Bottom.Text == chk_ColorPad4Length_Side_Bottom.Text)
                            intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad2Area_Side_Bottom)
                {
                    if (chk_ColorPad2Area_Side_Bottom.Checked)
                    { 
                        intFailMask |= 0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Bottom.Text == chk_ColorPad3Area_Side_Bottom.Text)
                            intFailMask |= 0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Bottom.Text == chk_ColorPad4Area_Side_Bottom.Text)
                            intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Bottom.Text == chk_ColorPad3Area_Side_Bottom.Text)
                            intFailMask &= ~0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Bottom.Text == chk_ColorPad4Area_Side_Bottom.Text)
                            intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad3Length_Side_Bottom)
                {
                    if (chk_ColorPad3Length_Side_Bottom.Checked)
                    { 
                        intFailMask |= 0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Bottom.Text == chk_ColorPad4Length_Side_Bottom.Text)
                            intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Bottom.Text == chk_ColorPad4Length_Side_Bottom.Text)
                            intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad3Area_Side_Bottom)
                {
                    if (chk_ColorPad3Area_Side_Bottom.Checked)
                    { 
                        intFailMask |= 0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Bottom.Text == chk_ColorPad4Area_Side_Bottom.Text)
                            intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Bottom.Text == chk_ColorPad4Area_Side_Bottom.Text)
                            intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad4Length_Side_Bottom)
                {
                    if (chk_ColorPad4Length_Side_Bottom.Checked)
                    { 
                        intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Bottom.Text == chk_ColorPad5Length_Side_Bottom.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad4Area_Side_Bottom)
                {
                    if (chk_ColorPad4Area_Side_Bottom.Checked)
                    { 
                        intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Bottom.Text == chk_ColorPad5Area_Side_Bottom.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad5Length_Side_Bottom)
                {
                    if (chk_ColorPad5Length_Side_Bottom.Checked)
                        intFailMask |= 0x100;
                    else
                        intFailMask &= ~0x100;

                }
                else if (sender == chk_ColorPad5Area_Side_Bottom)
                {
                    if (chk_ColorPad5Area_Side_Bottom.Checked)
                        intFailMask |= 0x200;
                    else
                        intFailMask &= ~0x200;
                }

                m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask = intFailMask;
            }
        }
        private void SetSideColorPadFailMask_Left(object sender)
        {
            //EnablePadFailMaskGUI(chk_InspectPad.Checked);
            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                if (m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[4].ref_intColorPadGroupIndex)
                    continue;

                int intFailMask = m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask;

                if (sender == chk_ColorPad1Length_Side_Left)
                {
                    if (chk_ColorPad1Length_Side_Left.Checked)
                    { 
                        intFailMask |= 0x01;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad2Length_Side_Left.Text)
                            intFailMask |= 0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad3Length_Side_Left.Text)
                            intFailMask |= 0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad4Length_Side_Left.Text)
                            intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x01;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad2Length_Side_Left.Text)
                            intFailMask &= ~0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad3Length_Side_Left.Text)
                            intFailMask &= ~0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad4Length_Side_Left.Text)
                            intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad1Area_Side_Left)
                {
                    if (chk_ColorPad1Area_Side_Left.Checked)
                    { 
                        intFailMask |= 0x02;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad2Area_Side_Left.Text)
                            intFailMask |= 0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad3Area_Side_Left.Text)
                            intFailMask |= 0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad4Area_Side_Left.Text)
                            intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x02;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 1 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad2Area_Side_Left.Text)
                            intFailMask &= ~0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad3Area_Side_Left.Text)
                            intFailMask &= ~0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad4Area_Side_Left.Text)
                            intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad1Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad2Length_Side_Left)
                {
                    if (chk_ColorPad2Length_Side_Left.Checked)
                    { 
                        intFailMask |= 0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Left.Text == chk_ColorPad3Length_Side_Left.Text)
                            intFailMask |= 0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Left.Text == chk_ColorPad4Length_Side_Left.Text)
                            intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x04;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Length_Side_Left.Text == chk_ColorPad3Length_Side_Left.Text)
                            intFailMask &= ~0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Length_Side_Left.Text == chk_ColorPad4Length_Side_Left.Text)
                            intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad2Area_Side_Left)
                {
                    if (chk_ColorPad2Area_Side_Left.Checked)
                    { 
                        intFailMask |= 0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Left.Text == chk_ColorPad3Area_Side_Left.Text)
                            intFailMask |= 0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Left.Text == chk_ColorPad4Area_Side_Left.Text)
                            intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x08;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 2 && chk_ColorPad2Area_Side_Left.Text == chk_ColorPad3Area_Side_Left.Text)
                            intFailMask &= ~0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad2Area_Side_Left.Text == chk_ColorPad4Area_Side_Left.Text)
                            intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad2Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad3Length_Side_Left)
                {
                    if (chk_ColorPad3Length_Side_Left.Checked)
                    { 
                        intFailMask |= 0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Left.Text == chk_ColorPad4Length_Side_Left.Text)
                            intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x10;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Length_Side_Left.Text == chk_ColorPad4Length_Side_Left.Text)
                            intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad3Area_Side_Left)
                {
                    if (chk_ColorPad3Area_Side_Left.Checked)
                    { 
                        intFailMask |= 0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Left.Text == chk_ColorPad4Area_Side_Left.Text)
                            intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x20;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 3 && chk_ColorPad3Area_Side_Left.Text == chk_ColorPad4Area_Side_Left.Text)
                            intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad3Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad4Length_Side_Left)
                {
                    if (chk_ColorPad4Length_Side_Left.Checked)
                    { 
                        intFailMask |= 0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask |= 0x100;
                    }
                    else
                    { 
                        intFailMask &= ~0x40;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Length_Side_Left.Text == chk_ColorPad5Length_Side_Left.Text)
                            intFailMask &= ~0x100;
                    }
                }
                else if (sender == chk_ColorPad4Area_Side_Left)
                {
                    if (chk_ColorPad4Area_Side_Left.Checked)
                    { 
                        intFailMask |= 0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask |= 0x200;
                    }
                    else
                    { 
                        intFailMask &= ~0x80;
                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count > 4 && chk_ColorPad4Area_Side_Left.Text == chk_ColorPad5Area_Side_Left.Text)
                            intFailMask &= ~0x200;
                    }
                }
                else if (sender == chk_ColorPad5Length_Side_Left)
                {
                    if (chk_ColorPad5Length_Side_Left.Checked)
                        intFailMask |= 0x100;
                    else
                        intFailMask &= ~0x100;

                }
                else if (sender == chk_ColorPad5Area_Side_Left)
                {
                    if (chk_ColorPad5Area_Side_Left.Checked)
                        intFailMask |= 0x200;
                    else
                        intFailMask &= ~0x200;
                }

                m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask = intFailMask;
            }
        }
        private void SetPadFailMask(object sender)
        {
            EnablePadFailMaskGUI(chk_InspectPad.Checked);
            //for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
            //{
            //    if (m_smVisionInfo.g_arrPad.Length == 1 || (m_smVisionInfo.g_intSelectedROIMask & (0x01 << i)) > 0)
            //    {
            int intFailMask = m_smVisionInfo.g_arrPad[0].ref_intFailOptionMask;

            if (sender == chk_Area_Pad)
            {
                if (chk_Area_Pad.Checked)
                    intFailMask |= 0x20;
                else
                    intFailMask &= ~0x20;
            }
            else if (sender == chk_WidthHeight_Pad)
            {
                if (chk_WidthHeight_Pad.Checked)
                    intFailMask |= 0xC0;
                else
                    intFailMask &= ~0xC0;
            }
            else if (sender == chk_OffSet_Pad)
            {
                if (chk_OffSet_Pad.Checked)
                    intFailMask |= 0x100;
                else
                    intFailMask &= ~0x100;
            }
            else if (sender == chk_Gap_Pad)
            {
                if (chk_Gap_Pad.Checked)
                    intFailMask |= 0x600;
                else
                    intFailMask &= ~0x600;
            }
            else if (sender == chk_CheckForeignMaterialArea_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadArea = chk_CheckForeignMaterialArea_Pad.Checked;
                if (chk_CheckForeignMaterialArea_Pad.Checked)
                {
                    intFailMask |= 0x01;
                }
                else
                {
                    if (!chk_CheckForeignMaterialLength_Pad.Checked)
                        intFailMask &= ~0x01;
                }
            }
            else if (sender == chk_CheckForeignMaterialTotalArea_Pad)
            {
                if (chk_CheckForeignMaterialTotalArea_Pad.Checked)
                    intFailMask |= 0x1000;
                else
                    intFailMask &= ~0x1000;
            }
            else if (sender == chk_CheckForeignMaterialLength_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadLength = chk_CheckForeignMaterialLength_Pad.Checked;
                if (chk_CheckForeignMaterialLength_Pad.Checked)
                {
                    intFailMask |= 0x01;
                }
                else
                {
                    if (!chk_CheckForeignMaterialArea_Pad.Checked)
                        intFailMask &= ~0x01;
                }
            }
            else if (sender == chk_CheckBrokenArea_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadArea = chk_CheckBrokenArea_Pad.Checked;
                if (chk_CheckBrokenArea_Pad.Checked)
                {
                    intFailMask |= 0x18;    // 0x08 = check broken  , 0x10 = check hole
                }
                else
                {
                    if (!chk_CheckBrokenLength_Pad.Checked)
                        intFailMask &= ~0x18;
                }
            }
            else if (sender == chk_CheckBrokenLength_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadLength = chk_CheckBrokenLength_Pad.Checked;
                if (chk_CheckBrokenLength_Pad.Checked)
                {
                    intFailMask |= 0x18;    // 0x08 = check broken  , 0x10 = check hole
                }
                else
                {
                    if (!chk_CheckBrokenArea_Pad.Checked)
                        intFailMask &= ~0x18;
                }
            }
            else if (sender == chk_CheckExcess_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExcessPadArea = chk_CheckExcess_Pad.Checked;
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExcessPadLength = false;
                if (chk_CheckExcess_Pad.Checked)
                    intFailMask |= 0x800;
                else
                    intFailMask &= ~0x800;
            }
            else if (sender == chk_CheckSmear_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckSmearPadLength = chk_CheckSmear_Pad.Checked;
                if (chk_CheckSmear_Pad.Checked)
                    intFailMask |= 0x2000;
                else
                    intFailMask &= ~0x2000;
            }
            else if (sender == chk_CheckEdgeLimit_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadEdgeLimit = chk_CheckEdgeLimit_Pad.Checked;
                if (chk_CheckEdgeLimit_Pad.Checked)
                    intFailMask |= 0x4000;
                else
                    intFailMask &= ~0x4000;
            }
            else if (sender == chk_CheckStandOff_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadStandOff = chk_CheckStandOff_Pad.Checked;
                if (chk_CheckStandOff_Pad.Checked)
                    intFailMask |= 0x8000;
                else
                    intFailMask &= ~0x8000;
            }
            else if (sender == chk_CheckEdgeDistance_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadEdgeDistance = chk_CheckEdgeDistance_Pad.Checked;
                if (chk_CheckEdgeDistance_Pad.Checked)
                    intFailMask |= 0x10000;
                else
                    intFailMask &= ~0x10000;
            }
            else if (sender == chk_CheckSpanX_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadSpanX = chk_CheckSpanX_Pad.Checked;
                if (chk_CheckSpanX_Pad.Checked)
                    intFailMask |= 0x20000;
                else
                    intFailMask &= ~0x20000;
            }
            else if (sender == chk_CheckSpanY_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadSpanY = chk_CheckSpanY_Pad.Checked;
                if (chk_CheckSpanY_Pad.Checked)
                    intFailMask |= 0x40000;
                else
                    intFailMask &= ~0x40000;
            }
            //else if (sender == chk_CheckEmpty)
            //{
            //    if (chk_CheckEmpty.Checked)
            //        intFailMask |= 0x4000;
            //    else
            //        intFailMask &= ~0x4000;
            //}
            //else if (sender == chk_CheckPosition)
            //{
            //    if (chk_CheckPosition.Checked)
            //        intFailMask |= 0x8000;
            //    else
            //        intFailMask &= ~0x8000;
            //}

            if ((m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg") && m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize && !m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON)
            {
                intFailMask &= ~0x1001; // Disable "Check Foreign material/contamination" under Pad when package is ON. Packge will do the contamination checking instead.
            }

            m_smVisionInfo.g_arrPad[0].ref_intFailOptionMask = intFailMask;
            //    }
            //}
        }
        private void SetPadFailMask_Old(object sender)
        {
            int intFailMask = m_smVisionInfo.g_arrPad[0].ref_intFailOptionMask;

            if (sender == chk_Area_Pad)
            {
                if (chk_Area_Pad.Checked)
                    intFailMask |= 0x20;
                else
                    intFailMask &= ~0x20;
            }
            else if (sender == chk_WidthHeight_Pad)
            {
                if (chk_WidthHeight_Pad.Checked)
                    intFailMask |= 0xC0;
                else
                    intFailMask &= ~0xC0;
            }
            else if (sender == chk_OffSet_Pad)
            {
                if (chk_OffSet_Pad.Checked)
                    intFailMask |= 0x100;
                else
                    intFailMask &= ~0x100;
            }
            else if (sender == chk_Gap_Pad)
            {
                if (chk_Gap_Pad.Checked)
                    intFailMask |= 0x600;
                else
                    intFailMask &= ~0x600;
            }
            else if (sender == chk_CheckForeignMaterialArea_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadArea = chk_CheckForeignMaterialArea_Pad.Checked;
                if (chk_CheckForeignMaterialArea_Pad.Checked)
                {
                    intFailMask |= 0x01;
                }
                else
                {
                    if (!chk_CheckForeignMaterialLength_Pad.Checked)
                        intFailMask &= ~0x01;
                }
            }
            else if (sender == chk_CheckForeignMaterialTotalArea_Pad)
            {
                if (chk_CheckForeignMaterialTotalArea_Pad.Checked)
                    intFailMask |= 0x1000;
                else
                    intFailMask &= ~0x1000;
            }
            else if (sender == chk_CheckForeignMaterialLength_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadLength = chk_CheckForeignMaterialLength_Pad.Checked;
                if (chk_CheckForeignMaterialLength_Pad.Checked)
                {
                    intFailMask |= 0x01;
                }
                else
                {
                    if (!chk_CheckForeignMaterialArea_Pad.Checked)
                        intFailMask &= ~0x01;
                }
            }
            else if (sender == chk_CheckBrokenArea_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadArea = chk_CheckBrokenArea_Pad.Checked;
                if (chk_CheckBrokenArea_Pad.Checked)
                {
                    intFailMask |= 0x18;    // 0x08 = check broken  , 0x10 = check hole
                }
                else
                {
                    if (!chk_CheckBrokenLength_Pad.Checked)
                        intFailMask &= ~0x18;
                }
            }
            else if (sender == chk_CheckBrokenLength_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadLength = chk_CheckBrokenLength_Pad.Checked;
                if (chk_CheckBrokenLength_Pad.Checked)
                {
                    intFailMask |= 0x18;    // 0x08 = check broken  , 0x10 = check hole
                }
                else
                {
                    if (!chk_CheckBrokenArea_Pad.Checked)
                        intFailMask &= ~0x18;
                }
            }
            else if (sender == chk_CheckExcess_Pad)
            {
                m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExcessPadArea = chk_CheckExcess_Pad.Checked;
                //m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExcessPadLength = false;
                if (chk_CheckExcess_Pad.Checked)
                    intFailMask |= 0x800;
                else
                    intFailMask &= ~0x800;
            }

            m_smVisionInfo.g_arrPad[0].ref_intFailOptionMask = intFailMask;
        }
        
        private void SetCenterPadPackageFailMask_Simple(object sender)
        {
            int intFailPkgMask = m_smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask;

            if (chk_CheckPkgSize_Pad2.Checked)
                intFailPkgMask |= 0x01;
            else
                intFailPkgMask &= ~0x01;

            if (chk_InspectPackage_Pad2.Checked)
            {
                chk_BrightFieldArea_Pad.Enabled = true;
                chk_BrightFieldLength_Pad.Enabled = true;
                chk_DarkFieldArea_Pad.Enabled = true;
                chk_DarkFieldLength_Pad.Enabled = true;
                chk_CrackDarkFieldArea_Pad.Enabled = true;
                chk_CrackDarkFieldLength_Pad.Enabled = true;
                chk_ChippedOffDarkFieldArea_Pad.Enabled = true;
                chk_ChippedOffBrightFieldArea_Pad.Enabled = true;
                chk_MoldFlashBrightFieldArea_Pad.Enabled = true;
                chk_MoldFlashBrightFieldLength_Pad.Enabled = true;
                chk_ForeignMaterialBrightFieldArea_Pad.Enabled = true;
                chk_ForeignMaterialBrightFieldLength_Pad.Enabled = true;

                if (chk_BrightFieldArea_Pad.Checked)
                    intFailPkgMask |= 0x10000;
                else
                    intFailPkgMask &= ~0x10000;

                if (chk_BrightFieldLength_Pad.Checked)
                    intFailPkgMask |= 0x20000;
                else
                    intFailPkgMask &= ~0x20000;

                if (chk_DarkFieldArea_Pad.Checked)
                    intFailPkgMask |= 0x40000;
                else
                    intFailPkgMask &= ~0x40000;

                if (chk_DarkFieldLength_Pad.Checked)
                    intFailPkgMask |= 0x80000;
                else
                    intFailPkgMask &= ~0x80000;

                if (chk_CrackDarkFieldArea_Pad.Checked)
                    intFailPkgMask |= 0x800;
                else
                    intFailPkgMask &= ~0x800;

                if (chk_CrackDarkFieldLength_Pad.Checked)
                    intFailPkgMask |= 0x400;
                else
                    intFailPkgMask &= ~0x400;

                if (chk_ChippedOffDarkFieldArea_Pad.Checked || chk_ChippedOffBrightFieldArea_Pad.Checked)
                    intFailPkgMask |= 0x04;
                else
                    intFailPkgMask &= ~0x04;

                if (chk_ChippedOffDarkFieldArea_Pad.Checked)
                    intFailPkgMask |= 0x100000;
                else
                    intFailPkgMask &= ~0x100000;

                if (chk_ChippedOffBrightFieldArea_Pad.Checked)
                    intFailPkgMask |= 0x200000;
                else
                    intFailPkgMask &= ~0x200000;

                if (chk_MoldFlashBrightFieldArea_Pad.Checked)
                    intFailPkgMask |= 0x80;
                else
                    intFailPkgMask &= ~0x80;

                if (chk_MoldFlashBrightFieldLength_Pad.Checked)
                    intFailPkgMask |= 0x1000000;
                else
                    intFailPkgMask &= ~0x1000000;
                
                if (chk_ForeignMaterialBrightFieldArea_Pad.Checked)
                    intFailPkgMask |= 0x400000;
                else
                    intFailPkgMask &= ~0x400000;

                if (chk_ForeignMaterialBrightFieldLength_Pad.Checked)
                    intFailPkgMask |= 0x800000;
                else
                    intFailPkgMask &= ~0x800000;


                m_smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask = intFailPkgMask;

                m_smVisionInfo.g_arrPad[0].SetWantInspectPackage(true);
            }
            else
            {
                chk_BrightFieldArea_Pad.Enabled = false;
                chk_BrightFieldLength_Pad.Enabled = false;
                chk_DarkFieldArea_Pad.Enabled = false;
                chk_DarkFieldLength_Pad.Enabled = false;
                chk_CrackDarkFieldArea_Pad.Enabled = false;
                chk_CrackDarkFieldLength_Pad.Enabled = false;
                chk_ChippedOffDarkFieldArea_Pad.Enabled = false;
                chk_ChippedOffBrightFieldArea_Pad.Enabled = false;
                chk_MoldFlashBrightFieldArea_Pad.Enabled = false;
                chk_MoldFlashBrightFieldLength_Pad.Enabled = false;
                chk_ForeignMaterialBrightFieldArea_Pad.Enabled = false;
                chk_ForeignMaterialBrightFieldLength_Pad.Enabled = false;

                m_smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask = intFailPkgMask;

                m_smVisionInfo.g_arrPad[0].SetWantInspectPackage(false);
            }
        }

        private void SetSidePadFailMask(object sender)
        {
            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
            {
                int intFailMask = m_smVisionInfo.g_arrPad[i].ref_intFailOptionMask;

                if (chk_Area_SidePad.Checked)
                    intFailMask |= 0x20;
                else
                    intFailMask &= ~0x20;

                if (chk_WidthHeight_SidePad.Checked)
                    intFailMask |= 0xC0;
                else
                    intFailMask &= ~0xC0;

                if (chk_OffSet_SidePad.Checked)
                    intFailMask |= 0x100;
                else
                    intFailMask &= ~0x100;

                if (chk_Gap_SidePad.Checked)
                    intFailMask |= 0x600;
                else
                    intFailMask &= ~0x600;

                m_smVisionInfo.g_arrPad[i].ref_blnWantCheckExtraPadArea = chk_CheckForeignMaterialArea_SidePad.Checked;
                if (chk_CheckForeignMaterialArea_SidePad.Checked)
                {
                    intFailMask |= 0x01;
                }
                else
                {
                    if (!chk_CheckForeignMaterialLength_SidePad.Checked)
                        intFailMask &= ~0x01;
                }

                if (chk_CheckForeignMaterialTotalArea_SidePad.Checked)
                    intFailMask |= 0x1000;
                else
                    intFailMask &= ~0x1000;

                m_smVisionInfo.g_arrPad[i].ref_blnWantCheckExtraPadLength = chk_CheckForeignMaterialLength_SidePad.Checked;
                if (chk_CheckForeignMaterialLength_SidePad.Checked)
                {
                    intFailMask |= 0x01;
                }
                else
                {
                    if (!chk_CheckForeignMaterialArea_SidePad.Checked)
                        intFailMask &= ~0x01;
                }

                m_smVisionInfo.g_arrPad[i].ref_blnWantCheckBrokenPadArea = chk_CheckBrokenArea_SidePad.Checked;
                if (chk_CheckBrokenArea_SidePad.Checked)
                {
                    intFailMask |= 0x18;    // 0x08 = check broken  , 0x10 = check hole
                }
                else
                {
                    if (!chk_CheckBrokenLength_SidePad.Checked)
                        intFailMask &= ~0x18;
                }

                m_smVisionInfo.g_arrPad[i].ref_blnWantCheckBrokenPadLength = chk_CheckBrokenLength_SidePad.Checked;
                if (chk_CheckBrokenLength_SidePad.Checked)
                {
                    intFailMask |= 0x18;    // 0x08 = check broken  , 0x10 = check hole
                }
                else
                {
                    if (!chk_CheckBrokenArea_SidePad.Checked)
                        intFailMask &= ~0x18;
                }

                m_smVisionInfo.g_arrPad[i].ref_blnWantCheckExcessPadArea = chk_CheckExcess_SidePad.Checked;
                //m_smVisionInfo.g_arrPad[i].ref_blnWantCheckExcessPadLength = false;
                if (chk_CheckExcess_SidePad.Checked)
                    intFailMask |= 0x800;
                else
                    intFailMask &= ~0x800;


                m_smVisionInfo.g_arrPad[i].ref_blnWantCheckSmearPadLength = chk_CheckSmear_SidePad.Checked;
                if (chk_CheckSmear_SidePad.Checked)
                    intFailMask |= 0x2000;
                else
                    intFailMask &= ~0x2000;
                
                m_smVisionInfo.g_arrPad[i].ref_blnWantCheckPadEdgeLimit = chk_CheckEdgeLimit_SidePad.Checked;
                if (chk_CheckEdgeLimit_SidePad.Checked)
                    intFailMask |= 0x4000;
                else
                    intFailMask &= ~0x4000;

                m_smVisionInfo.g_arrPad[i].ref_blnWantCheckPadStandOff = chk_CheckStandOff_SidePad.Checked;
                if (chk_CheckStandOff_SidePad.Checked)
                    intFailMask |= 0x8000;
                else
                    intFailMask &= ~0x8000;

                m_smVisionInfo.g_arrPad[i].ref_blnWantCheckPadEdgeDistance = chk_CheckEdgeDistance_SidePad.Checked;
                if (chk_CheckEdgeDistance_SidePad.Checked)
                    intFailMask |= 0x10000;
                else
                    intFailMask &= ~0x10000;

                m_smVisionInfo.g_arrPad[i].ref_blnWantCheckPadSpanX = chk_CheckSpanX_SidePad.Checked;
                if (chk_CheckSpanX_SidePad.Checked)
                    intFailMask |= 0x20000;
                else
                    intFailMask &= ~0x20000;

                m_smVisionInfo.g_arrPad[i].ref_blnWantCheckPadSpanY = chk_CheckSpanY_SidePad.Checked;
                if (chk_CheckSpanY_SidePad.Checked)
                    intFailMask |= 0x40000;
                else
                    intFailMask &= ~0x40000;

                if ((m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg") && m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize && !m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON)
                {
                    intFailMask &= ~0x1001; // Disable "Check Foreign material/contamination" under Pad when package is ON. Packge will do the contamination checking instead.
                }

                m_smVisionInfo.g_arrPad[i].ref_intFailOptionMask = intFailMask;
            }
        }
        
        private void SetSidePadPackageFailMask_Simple(object sender)
        {
            int intFailPkgMask = m_smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask;

            if (chk_CheckPkgSize_SidePad2.Checked)
                intFailPkgMask |= 0x01;
            else
                intFailPkgMask &= ~0x01;

            if (chk_InspectPackage_SidePad2.Checked)
            {
                chk_BrightFieldArea_SidePad.Enabled = true;
                chk_BrightFieldLength_SidePad.Enabled = true;
                chk_DarkFieldArea_SidePad.Enabled = true;
                chk_DarkFieldLength_SidePad.Enabled = true;
                chk_CrackDarkFieldArea_SidePad.Enabled = true;
                chk_CrackDarkFieldLength_SidePad.Enabled = true;
                chk_ChippedOffDarkFieldArea_SidePad.Enabled = true;
                chk_ChippedOffBrightFieldArea_SidePad.Enabled = true;
                chk_MoldFlashBrightFieldArea_SidePad.Enabled = true;
                chk_MoldFlashBrightFieldLength_SidePad.Enabled = true;
                chk_ForeignMaterialBrightFieldArea_SidePad.Enabled = true;
                chk_ForeignMaterialBrightFieldLength_SidePad.Enabled = true;

                if (chk_BrightFieldArea_SidePad.Checked)
                    intFailPkgMask |= 0x10000;
                else
                    intFailPkgMask &= ~0x10000;

                if (chk_BrightFieldLength_SidePad.Checked)
                    intFailPkgMask |= 0x20000;
                else
                    intFailPkgMask &= ~0x20000;

                if (chk_DarkFieldArea_SidePad.Checked)
                    intFailPkgMask |= 0x40000;
                else
                    intFailPkgMask &= ~0x40000;

                if (chk_DarkFieldLength_SidePad.Checked)
                    intFailPkgMask |= 0x80000;
                else
                    intFailPkgMask &= ~0x80000;

                if (chk_CrackDarkFieldArea_SidePad.Checked)
                    intFailPkgMask |= 0x800;
                else
                    intFailPkgMask &= ~0x800;

                if (chk_CrackDarkFieldLength_SidePad.Checked)
                    intFailPkgMask |= 0x400;
                else
                    intFailPkgMask &= ~0x400;

                if (chk_ChippedOffDarkFieldArea_SidePad.Checked || chk_ChippedOffBrightFieldArea_SidePad.Checked)
                    intFailPkgMask |= 0x04;
                else
                    intFailPkgMask &= ~0x04;

                if (chk_ChippedOffDarkFieldArea_SidePad.Checked)
                    intFailPkgMask |= 0x100000;
                else
                    intFailPkgMask &= ~0x100000;

                if (chk_ChippedOffBrightFieldArea_SidePad.Checked)
                    intFailPkgMask |= 0x200000;
                else
                    intFailPkgMask &= ~0x200000;

                if (chk_MoldFlashBrightFieldArea_SidePad.Checked)
                    intFailPkgMask |= 0x80;
                else
                    intFailPkgMask &= ~0x80;

                if (chk_MoldFlashBrightFieldLength_SidePad.Checked)
                    intFailPkgMask |= 0x1000000;
                else
                    intFailPkgMask &= ~0x1000000;

                if (chk_ForeignMaterialBrightFieldArea_SidePad.Checked)
                    intFailPkgMask |= 0x400000;
                else
                    intFailPkgMask &= ~0x400000;

                if (chk_ForeignMaterialBrightFieldLength_SidePad.Checked)
                    intFailPkgMask |= 0x800000;
                else
                    intFailPkgMask &= ~0x800000;

                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    m_smVisionInfo.g_arrPad[i].ref_intFailPkgOptionMask = intFailPkgMask;
                    m_smVisionInfo.g_arrPad[i].SetWantInspectPackage(true);
                }
            }
            else
            {
                chk_BrightFieldArea_SidePad.Enabled = false;
                chk_BrightFieldLength_SidePad.Enabled = false;
                chk_DarkFieldArea_SidePad.Enabled = false;
                chk_DarkFieldLength_SidePad.Enabled = false;
                chk_CrackDarkFieldArea_SidePad.Enabled = false;
                chk_CrackDarkFieldLength_SidePad.Enabled = false;
                chk_ChippedOffDarkFieldArea_SidePad.Enabled = false;
                chk_ChippedOffBrightFieldArea_SidePad.Enabled = false;
                chk_MoldFlashBrightFieldArea_SidePad.Enabled = false;
                chk_MoldFlashBrightFieldLength_SidePad.Enabled = false;
                chk_ForeignMaterialBrightFieldArea_SidePad.Enabled = false;
                chk_ForeignMaterialBrightFieldLength_SidePad.Enabled = false;

                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                {
                    m_smVisionInfo.g_arrPad[i].ref_intFailPkgOptionMask = intFailPkgMask;
                    m_smVisionInfo.g_arrPad[i].SetWantInspectPackage(false);
                }
            }
        }

        private void SetLeadFailMask(object sender)
        {
            if (chk_InspectLead.Checked)
            {
                chk_WidthHeight_Lead.Enabled = true;
                chk_OffSet_Lead.Enabled = true;
                chk_Skew_Lead.Enabled = true;
                chk_PitchGap_Lead.Enabled = true;
                chk_Variance_Lead.Enabled = true;
                chk_AverageGrayValue_Lead.Enabled = true;
                chk_Span_Lead.Enabled = true;
                chk_CheckForeignMaterialArea_Lead.Enabled = true;
                chk_CheckForeignMaterialLength_Lead.Enabled = true;
                chk_CheckForeignMaterialTotalArea_Lead.Enabled = true;
                chk_BaseLeadArea.Enabled = true;
                chk_BaseLeadOffset.Enabled = true;

                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    int intFailMask = m_smVisionInfo.g_arrLead[i].ref_intFailOptionMask;

                    if (sender == chk_WidthHeight_Lead)
                    {
                        if (chk_WidthHeight_Lead.Checked)
                            intFailMask |= 0xC0;
                        else
                            intFailMask &= ~0xC0;
                    }
                    else if (sender == chk_OffSet_Lead)
                    {
                        if (chk_OffSet_Lead.Checked)
                            intFailMask |= 0x100;
                        else
                            intFailMask &= ~0x100;
                    }
                    else if (sender == chk_Skew_Lead)
                    {
                        if (chk_Skew_Lead.Checked)
                            intFailMask |= 0x8000;
                        else
                            intFailMask &= ~0x8000;
                    }
                    else if (sender == chk_PitchGap_Lead)
                    {
                        if (chk_PitchGap_Lead.Checked)
                            intFailMask |= 0x600;
                        else
                            intFailMask &= ~0x600;
                    }
                    else if (sender == chk_Variance_Lead)
                    {
                        if (chk_Variance_Lead.Checked)
                            intFailMask |= 0x800;
                        else
                            intFailMask &= ~0x800;
                    }
                    else if (sender == chk_AverageGrayValue_Lead)
                    {
                        if (chk_AverageGrayValue_Lead.Checked)
                            intFailMask |= 0x4000;
                        else
                            intFailMask &= ~0x4000;
                    }
                    else if (sender == chk_Span_Lead)
                    {
                        if (chk_Span_Lead.Checked)
                            intFailMask |= 0x1000;
                        else
                            intFailMask &= ~0x1000;
                    }
                    else if (sender == chk_CheckForeignMaterialArea_Lead || sender == chk_CheckForeignMaterialLength_Lead)
                    {
                        m_smVisionInfo.g_arrLead[i].ref_blnWantCheckExtraLeadArea = chk_CheckForeignMaterialArea_Lead.Checked;
                        m_smVisionInfo.g_arrLead[i].ref_blnWantCheckExtraLeadLength = chk_CheckForeignMaterialLength_Lead.Checked;
                        if (chk_CheckForeignMaterialArea_Lead.Checked || chk_CheckForeignMaterialLength_Lead.Checked)
                            intFailMask |= 0x01;
                        else
                            intFailMask &= ~0x01;
                    }
                    else if (sender == chk_CheckForeignMaterialTotalArea_Lead)
                    {
                        if (chk_CheckForeignMaterialTotalArea_Lead.Checked)
                            intFailMask |= 0x2000;
                        else
                            intFailMask &= ~0x2000;
                    }
                    else if (sender == chk_BaseLeadOffset)
                    {
                        if (chk_BaseLeadOffset.Checked)
                            intFailMask |= 0x10000;
                        else
                            intFailMask &= ~0x10000;
                    }
                    else if (sender == chk_BaseLeadArea)
                    {
                        if (chk_BaseLeadArea.Checked)
                            intFailMask |= 0x20000;
                        else
                            intFailMask &= ~0x20000;
                    }
                    m_smVisionInfo.g_arrLead[i].ref_intFailOptionMask = intFailMask;
                    m_smVisionInfo.g_arrLead[i].SetWantInspectLead(true);
                }
            }
            else
            {
                chk_WidthHeight_Lead.Enabled = false;
                chk_OffSet_Lead.Enabled = false;
                chk_Skew_Lead.Enabled = false;
                chk_PitchGap_Lead.Enabled = false;
                chk_Variance_Lead.Enabled = false;
                chk_AverageGrayValue_Lead.Enabled = false;
                chk_Span_Lead.Enabled = false;
                chk_CheckForeignMaterialArea_Lead.Enabled = false;
                chk_CheckForeignMaterialLength_Lead.Enabled = false;
                chk_CheckForeignMaterialTotalArea_Lead.Enabled = false;
                chk_BaseLeadArea.Enabled = false;
                chk_BaseLeadOffset.Enabled = false;

                for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
                {
                    m_smVisionInfo.g_arrLead[i].SetWantInspectLead(false);
                }
            }
        }

        private void SetLead3DFailMask(object sender)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                int intFailMask = m_smVisionInfo.g_arrLead3D[i].ref_intFailOptionMask;

                if (sender == chk_Offset_Lead3D)
                {
                    if (chk_Offset_Lead3D.Checked)
                        intFailMask |= 0x20000;
                    else
                        intFailMask &= ~0x20000;
                }

                if (sender == chk_Skew_Lead3D)
                {
                    if (chk_Skew_Lead3D.Checked)
                        intFailMask |= 0x100;
                    else
                        intFailMask &= ~0x100;
                }

                if (sender == chk_Width_Lead3D)
                {
                    if (chk_Width_Lead3D.Checked)
                        intFailMask |= 0x40;
                    else
                        intFailMask &= ~0x40;
                }

                if (sender == chk_Length_Lead3D)
                {
                    if (chk_Length_Lead3D.Checked)
                        intFailMask |= 0x80;
                    else
                        intFailMask &= ~0x80;
                }

                if (sender == chk_LengthVariance_Lead3D)
                {
                    if (chk_LengthVariance_Lead3D.Checked)
                        intFailMask |= 0x800;
                    else
                        intFailMask &= ~0x800;
                }

                if (sender == chk_PitchGap_Lead3D)
                {
                    if (chk_PitchGap_Lead3D.Checked)
                        intFailMask |= 0x600; //0x200
                    else
                        intFailMask &= ~0x600;//0x200
                }

                if (sender == chk_PitchVariance_Lead3D)
                {
                    if (chk_PitchVariance_Lead3D.Checked)
                        intFailMask |= 0x2000;
                    else
                        intFailMask &= ~0x2000;
                }

                if (sender == chk_StandOff_Lead3D)
                {
                    if (chk_StandOff_Lead3D.Checked)
                        intFailMask |= 0x01;
                    else
                        intFailMask &= ~0x01;
                }

                if (sender == chk_StandoffVariance_Lead3D)
                {
                    if (chk_StandoffVariance_Lead3D.Checked)
                        intFailMask |= 0x4000;
                    else
                        intFailMask &= ~0x4000;
                }

                if (sender == chk_Coplan_Lead3D)
                {
                    if (chk_Coplan_Lead3D.Checked)
                        intFailMask |= 0x02;
                    else
                        intFailMask &= ~0x02;
                }

                if (sender == chk_Span_Lead3D)
                {
                    if (chk_Span_Lead3D.Checked)
                        intFailMask |= 0x1000;
                    else
                        intFailMask &= ~0x1000;
                }

                if (sender == chk_LeadSweeps_Lead3D)
                {
                    if (chk_LeadSweeps_Lead3D.Checked)
                        intFailMask |= 0x04;
                    else
                        intFailMask &= ~0x04;
                }

                if (sender == chk_SolderPadLength_Lead3D)
                {
                    if (chk_SolderPadLength_Lead3D.Checked)
                        intFailMask |= 0x08;
                    else
                        intFailMask &= ~0x08;
                }

                if (sender == chk_UnCutTiebar_Lead3D)
                {
                    if (chk_UnCutTiebar_Lead3D.Checked)
                        intFailMask |= 0x10;
                    else
                        intFailMask &= ~0x10;
                }
                else if (sender == chk_CheckForeignMaterialArea_Lead3D || sender == chk_CheckForeignMaterialLength_Lead3D)
                {
                    m_smVisionInfo.g_arrLead3D[i].ref_blnWantCheckExtraLeadArea = chk_CheckForeignMaterialArea_Lead3D.Checked;
                    m_smVisionInfo.g_arrLead3D[i].ref_blnWantCheckExtraLeadLength = chk_CheckForeignMaterialLength_Lead3D.Checked;
                    if (chk_CheckForeignMaterialArea_Lead3D.Checked || chk_CheckForeignMaterialLength_Lead3D.Checked)
                        intFailMask |= 0x8000;
                    else
                        intFailMask &= ~0x8000;
                }
                else if (sender == chk_CheckForeignMaterialTotalArea_Lead3D)
                {
                    if (chk_CheckForeignMaterialTotalArea_Lead3D.Checked)
                        intFailMask |= 0x10000;
                    else
                        intFailMask &= ~0x10000;
                }
                else if(sender == chk_AverageGrayValue_Lead3D)
                {
                    if (chk_AverageGrayValue_Lead3D.Checked)
                        intFailMask |= 0x40000;
                    else
                        intFailMask &= ~0x40000;
                }
                else if (sender == chk_LeadMinAndMaxWidth_Lead3D)
                {
                    if (chk_LeadMinAndMaxWidth_Lead3D.Checked)
                        intFailMask |= 0x100000;
                    else
                        intFailMask &= ~0x100000;
                }
                else if (sender == chk_LeadBurrWidth)
                {
                    if (chk_LeadBurrWidth.Checked)
                        intFailMask |= 0x200000;
                    else
                        intFailMask &= ~0x200000;
                }

                m_smVisionInfo.g_arrLead3D[i].ref_intFailOptionMask = intFailMask;
            }
        }

        private void SetLead3DPkgFailMask_Simple(object sender)
        {
            int intFailPkgMask = m_smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask;

            if (chk_CheckPkgSize_Lead3D.Checked)
                intFailPkgMask |= 0x01;
            else
                intFailPkgMask &= ~0x01;

            if (chk_InspectPackage_Lead3D.Checked)
            {
                chk_BrightFieldArea_Lead3D.Enabled = true;
                chk_BrightFieldLength_Lead3D.Enabled = true;
                chk_DarkFieldArea_Lead3D.Enabled = true;
                chk_DarkFieldLength_Lead3D.Enabled = true;
                chk_CrackDarkFieldArea_Lead3D.Enabled = true;
                chk_CrackDarkFieldLength_Lead3D.Enabled = true;
                chk_ChippedOffDarkFieldArea_Lead3D.Enabled = true;
                chk_ChippedOffBrightFieldArea_Lead3D.Enabled = true;
                chk_MoldFlashBrightFieldArea_Lead3D.Enabled = true;

                if (chk_BrightFieldArea_Lead3D.Checked)
                    intFailPkgMask |= 0x10000;
                else
                    intFailPkgMask &= ~0x10000;

                if (chk_BrightFieldLength_Lead3D.Checked)
                    intFailPkgMask |= 0x20000;
                else
                    intFailPkgMask &= ~0x20000;

                if (chk_DarkFieldArea_Lead3D.Checked)
                    intFailPkgMask |= 0x40000;
                else
                    intFailPkgMask &= ~0x40000;

                if (chk_DarkFieldLength_Lead3D.Checked)
                    intFailPkgMask |= 0x80000;
                else
                    intFailPkgMask &= ~0x80000;

                if (chk_CrackDarkFieldArea_Lead3D.Checked)
                    intFailPkgMask |= 0x800;
                else
                    intFailPkgMask &= ~0x800;

                if (chk_CrackDarkFieldLength_Lead3D.Checked)
                    intFailPkgMask |= 0x400;
                else
                    intFailPkgMask &= ~0x400;

                if (chk_ChippedOffDarkFieldArea_Lead3D.Checked || chk_ChippedOffBrightFieldArea_Lead3D.Checked)
                    intFailPkgMask |= 0x04;
                else
                    intFailPkgMask &= ~0x04;

                if (chk_ChippedOffDarkFieldArea_Lead3D.Checked)
                    intFailPkgMask |= 0x100000;
                else
                    intFailPkgMask &= ~0x100000;

                if (chk_ChippedOffBrightFieldArea_Lead3D.Checked)
                    intFailPkgMask |= 0x200000;
                else
                    intFailPkgMask &= ~0x200000;

                if (chk_MoldFlashBrightFieldArea_Lead3D.Checked)
                    intFailPkgMask |= 0x80;
                else
                    intFailPkgMask &= ~0x80;

                m_smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask = intFailPkgMask;

                m_smVisionInfo.g_arrLead3D[0].SetWantInspectPackage(true);
            }
            else
            {
                chk_BrightFieldArea_Lead3D.Enabled = false;
                chk_BrightFieldLength_Lead3D.Enabled = false;
                chk_DarkFieldArea_Lead3D.Enabled = false;
                chk_DarkFieldLength_Lead3D.Enabled = false;
                chk_CrackDarkFieldArea_Lead3D.Enabled = false;
                chk_CrackDarkFieldLength_Lead3D.Enabled = false;
                chk_ChippedOffDarkFieldArea_Lead3D.Enabled = false;
                chk_ChippedOffBrightFieldArea_Lead3D.Enabled = false;
                chk_MoldFlashBrightFieldArea_Lead3D.Enabled = false;

                m_smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask = intFailPkgMask;

                m_smVisionInfo.g_arrLead3D[0].SetWantInspectPackage(false);
            }
        }
      
        private void SetColorPackageFailMask(object sender)
        {
            int intFailMask = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask;

            if (sender == chk_ColorPackage1Length)
            {
                if (chk_ColorPackage1Length.Checked)
                {
                    intFailMask |= 0x01;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 1 && chk_ColorPackage1Length.Text == chk_ColorPackage2Length.Text)
                        intFailMask |= 0x04;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage1Length.Text == chk_ColorPackage3Length.Text)
                        intFailMask |= 0x10;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage1Length.Text == chk_ColorPackage4Length.Text)
                        intFailMask |= 0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage1Length.Text == chk_ColorPackage5Length.Text)
                        intFailMask |= 0x100;
                }
                else
                { 
                    intFailMask &= ~0x01;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 1 && chk_ColorPackage1Length.Text == chk_ColorPackage2Length.Text)
                        intFailMask &= ~0x04;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage1Length.Text == chk_ColorPackage3Length.Text)
                        intFailMask &= ~0x10;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage1Length.Text == chk_ColorPackage4Length.Text)
                        intFailMask &= ~0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage1Length.Text == chk_ColorPackage5Length.Text)
                        intFailMask &= ~0x100;
                }
            }
            else if (sender == chk_ColorPackage1Area)
            {
                if (chk_ColorPackage1Area.Checked)
                { 
                    intFailMask |= 0x02;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 1 && chk_ColorPackage1Area.Text == chk_ColorPackage2Area.Text)
                        intFailMask |= 0x08;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage1Area.Text == chk_ColorPackage3Area.Text)
                        intFailMask |= 0x20;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage1Area.Text == chk_ColorPackage4Area.Text)
                        intFailMask |= 0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage1Area.Text == chk_ColorPackage5Area.Text)
                        intFailMask |= 0x200;
                }
                else
                { 
                    intFailMask &= ~0x02;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 1 && chk_ColorPackage1Area.Text == chk_ColorPackage2Area.Text)
                        intFailMask &= ~0x08;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage1Area.Text == chk_ColorPackage3Area.Text)
                        intFailMask &= ~0x20;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage1Area.Text == chk_ColorPackage4Area.Text)
                        intFailMask &= ~0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage1Area.Text == chk_ColorPackage5Area.Text)
                        intFailMask &= ~0x200;
                }
            }
            else if (sender == chk_ColorPackage2Length)
            {
                if (chk_ColorPackage2Length.Checked)
                { 
                    intFailMask |= 0x04;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage2Length.Text == chk_ColorPackage3Length.Text)
                        intFailMask |= 0x10;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage2Length.Text == chk_ColorPackage4Length.Text)
                        intFailMask |= 0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage2Length.Text == chk_ColorPackage5Length.Text)
                        intFailMask |= 0x100;
                }
                else
                { 
                    intFailMask &= ~0x04;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage2Length.Text == chk_ColorPackage3Length.Text)
                        intFailMask &= ~0x10;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage2Length.Text == chk_ColorPackage4Length.Text)
                        intFailMask &= ~0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage2Length.Text == chk_ColorPackage5Length.Text)
                        intFailMask &= ~0x100;
                }
            }
            else if (sender == chk_ColorPackage2Area)
            {
                if (chk_ColorPackage2Area.Checked)
                { 
                    intFailMask |= 0x08;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage2Area.Text == chk_ColorPackage3Area.Text)
                        intFailMask |= 0x20;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage2Area.Text == chk_ColorPackage4Area.Text)
                        intFailMask |= 0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage2Area.Text == chk_ColorPackage5Area.Text)
                        intFailMask |= 0x200;
                }
                else
                { 
                    intFailMask &= ~0x08;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 2 && chk_ColorPackage2Area.Text == chk_ColorPackage3Area.Text)
                        intFailMask &= ~0x20;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage2Area.Text == chk_ColorPackage4Area.Text)
                        intFailMask &= ~0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage2Area.Text == chk_ColorPackage5Area.Text)
                        intFailMask &= ~0x200;
                }
            }
            else if (sender == chk_ColorPackage3Length)
            {
                if (chk_ColorPackage3Length.Checked)
                { 
                    intFailMask |= 0x10;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage3Length.Text == chk_ColorPackage4Length.Text)
                        intFailMask |= 0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage3Length.Text == chk_ColorPackage5Length.Text)
                        intFailMask |= 0x100;
                }
                else
                { 
                    intFailMask &= ~0x10;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage3Length.Text == chk_ColorPackage4Length.Text)
                        intFailMask &= ~0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage3Length.Text == chk_ColorPackage5Length.Text)
                        intFailMask &= ~0x100;
                }
            }
            else if (sender == chk_ColorPackage3Area)
            {
                if (chk_ColorPackage3Area.Checked)
                { 
                    intFailMask |= 0x20;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage3Area.Text == chk_ColorPackage4Area.Text)
                        intFailMask |= 0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage3Area.Text == chk_ColorPackage5Area.Text)
                        intFailMask |= 0x200;
                }
                else
                { 
                    intFailMask &= ~0x20;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 3 && chk_ColorPackage3Area.Text == chk_ColorPackage4Area.Text)
                        intFailMask &= ~0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage3Area.Text == chk_ColorPackage5Area.Text)
                        intFailMask &= ~0x200;
                }
            }
            else if (sender == chk_ColorPackage4Length)
            {
                if (chk_ColorPackage4Length.Checked)
                { 
                    intFailMask |= 0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage4Length.Text == chk_ColorPackage5Length.Text)
                        intFailMask |= 0x100;
                }
                else
                { 
                    intFailMask &= ~0x40;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage4Length.Text == chk_ColorPackage5Length.Text)
                        intFailMask &= ~0x100;
                }
            }
            else if (sender == chk_ColorPackage4Area)
            {
                if (chk_ColorPackage4Area.Checked)
                { 
                    intFailMask |= 0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage4Area.Text == chk_ColorPackage5Area.Text)
                        intFailMask |= 0x200;
                }
                else
                { 
                    intFailMask &= ~0x80;
                    if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_arrDefectColor.Count > 4 && chk_ColorPackage4Area.Text == chk_ColorPackage5Area.Text)
                        intFailMask &= ~0x200;
                }
            }
            else if (sender == chk_ColorPackage5Length)
            {
                if (chk_ColorPackage5Length.Checked)
                    intFailMask |= 0x100;
                else
                    intFailMask &= ~0x100;

            }
            else if (sender == chk_ColorPackage5Area)
            {
                if (chk_ColorPackage5Area.Checked)
                    intFailMask |= 0x200;
                else
                    intFailMask &= ~0x200;
            }

            m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask = intFailMask;

        }
        private void SetPkgFailMask_Simple(object sender)
        {
            if (!m_blnInitDone)
                return;

            m_smVisionInfo.g_intPackageFailMask = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask;

            if (chk_CheckPkgSize2.Checked)
                m_smVisionInfo.g_intPackageFailMask |= 0x1000;
            else
                m_smVisionInfo.g_intPackageFailMask &= ~0x1000;

            if (chk_CheckPkgAngle.Checked)
                m_smVisionInfo.g_intPackageFailMask |= 0x2000;
            else
                m_smVisionInfo.g_intPackageFailMask &= ~0x2000;

            if (chk_InspectPackage2.Checked == false)
            {
                chk_BrightFieldArea.Enabled = false;
                chk_BrightFieldLength.Enabled = false;
                chk_DarkFieldArea.Enabled = false;
                chk_DarkFieldLength.Enabled = false;
                chk_DarkField2Area.Enabled = false;
                chk_DarkField2Length.Enabled = false;
                chk_DarkField3Area.Enabled = false;
                chk_DarkField3Length.Enabled = false;
                chk_DarkField4Area.Enabled = false;
                chk_DarkField4Length.Enabled = false;
                chk_CrackDarkFieldArea.Enabled = false;
                chk_CrackDarkFieldLength.Enabled = false;
                chk_VoidDarkFieldArea.Enabled = false;
                chk_VoidDarkFieldLength.Enabled = false;
                chk_MoldFlashBrightFieldArea.Enabled = false;
                chk_CheckChippedOffBright_Area.Enabled = false;
                chk_CheckChippedOffBright_Length.Enabled = false;
                chk_CheckChippedOffDark_Area.Enabled = false;
                chk_CheckChippedOffDark_Length.Enabled = false;

                for (int u = 0; u < m_smVisionInfo.g_arrPackage.Count; u++)
                    m_smVisionInfo.g_arrPackage[u].ref_intFailMask = m_smVisionInfo.g_intPackageFailMask;

                for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                {
                    m_smVisionInfo.g_arrPackage[i].SetWantInspectPackage(false);


                }
            }
            else
            {
                chk_BrightFieldArea.Enabled = true;
                chk_BrightFieldLength.Enabled = true;
                chk_DarkFieldArea.Enabled = true;
                chk_DarkFieldLength.Enabled = true;
                chk_DarkField2Area.Enabled = true;
                chk_DarkField2Length.Enabled = true;
                chk_DarkField3Area.Enabled = true;
                chk_DarkField3Length.Enabled = true;
                chk_DarkField4Area.Enabled = true;
                chk_DarkField4Length.Enabled = true;
                chk_CrackDarkFieldArea.Enabled = true;
                chk_CrackDarkFieldLength.Enabled = true;
                chk_VoidDarkFieldArea.Enabled = true;
                chk_VoidDarkFieldLength.Enabled = true;
                chk_MoldFlashBrightFieldArea.Enabled = true;
                chk_CheckChippedOffBright_Area.Enabled = true;
                chk_CheckChippedOffBright_Length.Enabled = true;
                chk_CheckChippedOffDark_Area.Enabled = true;
                chk_CheckChippedOffDark_Length.Enabled = true;

                if (chk_BrightFieldArea.Checked || chk_BrightFieldLength.Checked)
                    m_smVisionInfo.g_intPackageFailMask |= 0x100;
                else
                    m_smVisionInfo.g_intPackageFailMask &= ~0x100;

                if (chk_DarkFieldArea.Checked || chk_DarkFieldLength.Checked)
                    m_smVisionInfo.g_intPackageFailMask |= 0x200;
                else
                    m_smVisionInfo.g_intPackageFailMask &= ~0x200;

                if (chk_DarkField2Area.Checked || chk_DarkField2Length.Checked)
                    m_smVisionInfo.g_intPackageFailMask |= 0x400;
                else
                    m_smVisionInfo.g_intPackageFailMask &= ~0x400;

                if (chk_DarkField3Area.Checked || chk_DarkField3Length.Checked)
                    m_smVisionInfo.g_intPackageFailMask |= 0x800;
                else
                    m_smVisionInfo.g_intPackageFailMask &= ~0x800;

                if (chk_DarkField4Area.Checked || chk_DarkField4Length.Checked)
                    m_smVisionInfo.g_intPackageFailMask |= 0x4000;
                else
                    m_smVisionInfo.g_intPackageFailMask &= ~0x4000;

                if (chk_CrackDarkFieldArea.Checked || chk_CrackDarkFieldLength.Checked)
                    m_smVisionInfo.g_intPackageFailMask |= 0x01;
                else
                    m_smVisionInfo.g_intPackageFailMask &= ~0x01;

                if (chk_VoidDarkFieldArea.Checked || chk_VoidDarkFieldLength.Checked)
                    m_smVisionInfo.g_intPackageFailMask |= 0x20;
                else
                    m_smVisionInfo.g_intPackageFailMask &= ~0x20;

                if (chk_MoldFlashBrightFieldArea.Checked)
                    m_smVisionInfo.g_intPackageFailMask |= 0x08;
                else
                    m_smVisionInfo.g_intPackageFailMask &= ~0x08;

                if (chk_CheckChippedOffDark_Area.Checked || chk_CheckChippedOffDark_Length.Checked || chk_CheckChippedOffBright_Area.Checked || chk_CheckChippedOffBright_Length.Checked)// || chk_CheckChippedOff_Length.Checked)
                    m_smVisionInfo.g_intPackageFailMask |= 0x02;
                else
                    m_smVisionInfo.g_intPackageFailMask &= ~0x02;

                for (int u = 0; u < m_smVisionInfo.g_arrPackage.Count; u++)
                    m_smVisionInfo.g_arrPackage[u].ref_intFailMask = m_smVisionInfo.g_intPackageFailMask;


                string strSenderName = ((CheckBox)sender).Name;
                for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                {
                    m_smVisionInfo.g_arrPackage[i].SetWantInspectPackage(true);

                    if (strSenderName == chk_BrightFieldArea.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Bright, chk_BrightFieldArea.Checked);

                        if(chk_BrightFieldLength.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Bright, chk_BrightFieldLength.Checked);
                    }
                    else if (strSenderName == chk_BrightFieldLength.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Bright, chk_BrightFieldLength.Checked);

                        if(chk_BrightFieldArea.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Bright, chk_BrightFieldArea.Checked);
                    }
                    else if (strSenderName == chk_DarkFieldArea.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Dark, chk_DarkFieldArea.Checked);

                        if(chk_DarkFieldLength.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Dark, chk_DarkFieldLength.Checked);
                    }
                    else if (strSenderName == chk_DarkFieldLength.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Dark, chk_DarkFieldLength.Checked);

                        if(chk_DarkFieldArea.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Dark, chk_DarkFieldArea.Checked);
                    }
                    else if (strSenderName == chk_DarkField2Area.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Dark2, chk_DarkField2Area.Checked);

                        if (chk_DarkField2Length.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Dark2, chk_DarkField2Length.Checked);
                    }
                    else if (strSenderName == chk_DarkField2Length.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Dark2, chk_DarkField2Length.Checked);

                        if (chk_DarkField2Area.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Dark2, chk_DarkField2Area.Checked);
                    }
                    else if (strSenderName == chk_DarkField3Area.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Dark3, chk_DarkField3Area.Checked);

                        if (chk_DarkField3Length.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Dark3, chk_DarkField3Length.Checked);
                    }
                    else if (strSenderName == chk_DarkField3Length.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Dark3, chk_DarkField3Length.Checked);

                        if (chk_DarkField3Area.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Dark3, chk_DarkField3Area.Checked);
                    }
                    else if (strSenderName == chk_DarkField4Area.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Dark4, chk_DarkField4Area.Checked);

                        if (chk_DarkField4Length.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Dark4, chk_DarkField4Length.Checked);
                    }
                    else if (strSenderName == chk_DarkField4Length.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Dark4, chk_DarkField4Length.Checked);

                        if (chk_DarkField4Area.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Dark4, chk_DarkField4Area.Checked);
                    }
                    else if (strSenderName == chk_CrackDarkFieldArea.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Crack, chk_CrackDarkFieldArea.Checked);

                        if(chk_CrackDarkFieldLength.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Crack, chk_CrackDarkFieldLength.Checked);
                    }
                    else if (strSenderName == chk_CrackDarkFieldLength.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Crack, chk_CrackDarkFieldLength.Checked);

                        if(chk_CrackDarkFieldArea.Checked== false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Crack, chk_CrackDarkFieldArea.Checked);
                    }
                    else if (strSenderName == chk_VoidDarkFieldArea.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Void, chk_VoidDarkFieldArea.Checked);
                    }
                    else if (strSenderName == chk_VoidDarkFieldLength.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.Void, chk_VoidDarkFieldLength.Checked);
                    }
                    else if (strSenderName == chk_MoldFlashBrightFieldArea.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.MoldFlash, chk_MoldFlashBrightFieldArea.Checked);
                    }
                    else if (strSenderName == chk_CheckChippedOffBright_Area.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.ChipBright, chk_CheckChippedOffBright_Area.Checked);

                        if(chk_CheckChippedOffDark_Area.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.ChipDark, chk_CheckChippedOffDark_Area.Checked);
                    }
                    else if (strSenderName == chk_CheckChippedOffBright_Length.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.ChipBright, chk_CheckChippedOffBright_Length.Checked);
                        
                        if (chk_CheckChippedOffDark_Length.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.ChipDark, chk_CheckChippedOffDark_Length.Checked);
                    }
                    else if (strSenderName == chk_CheckChippedOffDark_Area.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.ChipDark, chk_CheckChippedOffDark_Area.Checked);

                        if(chk_CheckChippedOffBright_Area.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.ChipBright, chk_CheckChippedOffBright_Area.Checked);
                    }
                    else if (strSenderName == chk_CheckChippedOffDark_Length.Name)
                    {
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.ChipDark, chk_CheckChippedOffDark_Length.Checked);
                        
                        if (chk_CheckChippedOffBright_Length.Checked == false)
                            m_smVisionInfo.g_arrPackage[i].SetWantDefectParam((int)Package.eWantDefect.ChipBright, chk_CheckChippedOffBright_Length.Checked);
                    }

                    if (chk_CheckChippedOffDark_Area.Checked || chk_CheckChippedOffBright_Area.Checked)
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Chip, true);
                    else
                        m_smVisionInfo.g_arrPackage[i].SetWantDefectAreaParam((int)Package.eWantDefect.Chip, false);
                }
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;

        }

        private void UpdateLead3DFailMaskGUI()
        {
            m_intFailMaskLead3D_Previous = m_smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask;
            m_blnFailMaskLead3DExtraArea_Previous = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_blnWantCheckExtraLeadLength;
            m_blnFailMaskLead3DExtraArea_Previous = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_blnWantCheckExtraLeadArea;

            if (m_smVisionInfo.g_blnWantPin1)
            {
                m_arrFailMaskPin1_Previous.Add(m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate));
            }

            int intFailMask = m_smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask;

            // Offset 
            chk_Offset_Lead3D.Checked = (intFailMask & 0x20000) > 0;

            // Skew 
            chk_Skew_Lead3D.Checked = (intFailMask & 0x100) > 0;
            //chk_Skew_Lead3D.Visible = !m_smVisionInfo.g_arrLead3D[0].ref_blnWantUsePkgToBaseTolerance; //2020-07-28 ZJYEOH : Hide Skew if use package to base method

            // Width 
            chk_Width_Lead3D.Checked = (intFailMask & 0x40) > 0;

            // Length
            chk_Length_Lead3D.Checked = (intFailMask & 0x80) > 0;

            // Length Variance 
            chk_LengthVariance_Lead3D.Checked = (intFailMask & 0x800) > 0;

            // Pitch
            chk_PitchGap_Lead3D.Checked = (intFailMask & 0x600) > 0;//0x200

            // Pitch Variance
            chk_PitchVariance_Lead3D.Checked = (intFailMask & 0x2000) > 0;

            // Stand Off 
            chk_StandOff_Lead3D.Checked = (intFailMask & 0x01) > 0;

            // Stand Off Variance 
            chk_StandoffVariance_Lead3D.Checked = (intFailMask & 0x4000) > 0;

            // Coplan 
            chk_Coplan_Lead3D.Checked = (intFailMask & 0x02) > 0;

            // Span
            chk_Span_Lead3D.Checked = (intFailMask & 0x1000) > 0;

            // Lead Sweeps
            chk_LeadSweeps_Lead3D.Checked = (intFailMask & 0x04) > 0;

            // Solder Pad Length  
            chk_SolderPadLength_Lead3D.Checked = (intFailMask & 0x08) > 0;

            // Un-Cut Tiebar
            chk_UnCutTiebar_Lead3D.Checked = (intFailMask & 0x10) > 0;

            // Foreign Material / Contamination
            if ((intFailMask & 0x8000) > 0)
            {
                chk_CheckForeignMaterialArea_Lead3D.Checked = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_blnWantCheckExtraLeadArea;
                chk_CheckForeignMaterialLength_Lead3D.Checked = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_blnWantCheckExtraLeadLength;
            }
            else
            {
                chk_CheckForeignMaterialArea_Lead3D.Checked = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_blnWantCheckExtraLeadArea = false;
                chk_CheckForeignMaterialLength_Lead3D.Checked = m_smVisionInfo.g_arrLead3D[m_smVisionInfo.g_intSelectedROI].ref_blnWantCheckExtraLeadLength = false;
            }

            chk_CheckForeignMaterialTotalArea_Lead3D.Checked = (intFailMask & 0x10000) > 0;

            // Average Gray Value
            chk_AverageGrayValue_Lead3D.Checked = (intFailMask & 0x40000) > 0;

            if (!m_smVisionInfo.g_arrLead3D[0].ref_blnWantUseAverageGrayValueMethod)
            {
                pnl_AverageGrayValue_Lead3D.Visible = false;
            }

            // Laed Min and Max Width
            chk_LeadMinAndMaxWidth_Lead3D.Checked = (intFailMask & 0x100000) > 0;

            // Lead Burr Width
            chk_LeadBurrWidth.Checked = (intFailMask & 0x200000) > 0;


            //------------------------------------Pin1--------------------------------
            if (m_smVisionInfo.g_blnWantPin1)
            {
                chk_WantInspectPin1.Checked = m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate);
            }

        }

        private void UpdateLead3DPkgFailMaskGUI()
        {
            m_intFailMaskLead3DPackage_Previous = m_smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask;
            m_blnFailMaskLead3DPackageDefect_Previous = m_smVisionInfo.g_arrLead3D[0].GetWantInspectPackage();

            int intPkgFailMask = m_smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask;
            // Use simple defect criteria

            chk_InspectPackage_Lead3D.Checked = m_smVisionInfo.g_arrLead3D[0].GetWantInspectPackage();
            chk_CheckPkgSize_Lead3D.Checked = (intPkgFailMask & 0x01) > 0;

            chk_BrightFieldArea_Lead3D.Checked = (intPkgFailMask & 0x10000) > 0;
            chk_BrightFieldLength_Lead3D.Checked = (intPkgFailMask & 0x20000) > 0;
            chk_DarkFieldArea_Lead3D.Checked = (intPkgFailMask & 0x40000) > 0;
            chk_DarkFieldLength_Lead3D.Checked = (intPkgFailMask & 0x80000) > 0;
            chk_CrackDarkFieldArea_Lead3D.Checked = (intPkgFailMask & 0x800) > 0;
            chk_CrackDarkFieldLength_Lead3D.Checked = (intPkgFailMask & 0x400) > 0;
            chk_ChippedOffDarkFieldArea_Lead3D.Checked = (intPkgFailMask & 0x100000) > 0;
            chk_ChippedOffBrightFieldArea_Lead3D.Checked = (intPkgFailMask & 0x200000) > 0;
            chk_MoldFlashBrightFieldArea_Lead3D.Checked = (intPkgFailMask & 0x80) > 0;

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

            if (chk_InspectPackage_Lead3D.Checked)
            {
                chk_BrightFieldArea_Lead3D.Enabled = true;
                chk_BrightFieldLength_Lead3D.Enabled = true;
                chk_DarkFieldArea_Lead3D.Enabled = true;
                chk_DarkFieldLength_Lead3D.Enabled = true;
                chk_CrackDarkFieldArea_Lead3D.Enabled = true;
                chk_CrackDarkFieldLength_Lead3D.Enabled = true;
                chk_ChippedOffDarkFieldArea_Lead3D.Enabled = true;
                chk_ChippedOffBrightFieldArea_Lead3D.Enabled = true;
                chk_MoldFlashBrightFieldArea_Lead3D.Enabled = true;
            }
            else
            {
                chk_BrightFieldArea_Lead3D.Enabled = false;
                chk_BrightFieldLength_Lead3D.Enabled = false;
                chk_DarkFieldArea_Lead3D.Enabled = false;
                chk_DarkFieldLength_Lead3D.Enabled = false;
                chk_CrackDarkFieldArea_Lead3D.Enabled = false;
                chk_CrackDarkFieldLength_Lead3D.Enabled = false;
                chk_ChippedOffDarkFieldArea_Lead3D.Enabled = false;
                chk_ChippedOffBrightFieldArea_Lead3D.Enabled = false;
                chk_MoldFlashBrightFieldArea_Lead3D.Enabled = false;
            }

        }

        private void UpdatePadFailMaskGUI()
        {
            EnablePadFailMaskGUI(chk_InspectPad.Checked);

            m_blnFailMaskInspectPad_Previous = chk_InspectPad.Checked;
            m_intFailMaskCenterPad_Previous = m_smVisionInfo.g_arrPad[0].ref_intFailOptionMask;
            m_blnFailMaskCenterPadExtraLength_Previous = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadLength;
            m_blnFailMaskCenterPadExtraArea_Previous = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadArea;
            m_blnFailMaskCenterPadBrokenLength_Previous = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadLength;
            m_blnFailMaskCenterPadBrokenArea_Previous = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadArea;
            
            if (m_smVisionInfo.g_blnWantPin1)
            {
                m_arrFailMaskPin1_Previous.Add(m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate));
            }

            int intFailMask = m_smVisionInfo.g_arrPad[0].ref_intFailOptionMask;

            // Area
            chk_Area_Pad.Checked = (intFailMask & 0x20) > 0;

            // Width and Height
            if ((intFailMask & 0xC0) > 0)
            {
                intFailMask |= 0xC0;    // Make sure both Width and Height are ON
                chk_WidthHeight_Pad.Checked = true;
            }
            else
            {
                intFailMask &= ~0xC0;   // Make sure both Width and Height are OFF
                chk_WidthHeight_Pad.Checked = false;
            }

            // Off Set
            chk_OffSet_Pad.Checked = (intFailMask & 0x100) > 0;

            // Foreign Material / Contamination
            if ((intFailMask & 0x01) > 0)
            {
                chk_CheckForeignMaterialArea_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadArea;
                chk_CheckForeignMaterialLength_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadLength;
            }
            else
            {
                chk_CheckForeignMaterialArea_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadArea = false;
                chk_CheckForeignMaterialLength_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExtraPadLength = false;
            }

            // Broken Pad 
            if ((intFailMask & 0x08) > 0 || (intFailMask & 0x10) > 0)
            {
                if ((intFailMask & 0x08) <= 0)  // Splite or Scratch
                    intFailMask |= 0x08;

                if ((intFailMask & 0x10) <= 0)  // Hole
                    intFailMask |= 0x10;

                //chk_BrokenPad.Checked = true;
                chk_CheckBrokenArea_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadArea;
                chk_CheckBrokenLength_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadLength;
            }
            else
            {
                //chk_BrokenPad.Checked = false;
                chk_CheckBrokenArea_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadArea = false;
                chk_CheckBrokenLength_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckBrokenPadLength = false;
            }

            // Pitch Gap
            if ((intFailMask & 0x200) > 0 || (intFailMask & 0x400) > 0)
            {
                if ((intFailMask & 0x200) <= 0)
                    intFailMask |= 0x200;

                if ((intFailMask & 0x400) <= 0)
                    intFailMask |= 0x400;

                chk_Gap_Pad.Checked = true;
            }
            else
            {
                chk_Gap_Pad.Checked = false;
            }

            if ((intFailMask & 0x800) > 0)
            {
                chk_CheckExcess_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExcessPadArea = true;
                //m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExcessPadLength = false;
            }
            else
            {
                chk_CheckExcess_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExcessPadArea = false;
                //m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExcessPadLength = false;
            }

            if ((intFailMask & 0x2000) > 0)
            {
                chk_CheckSmear_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExcessPadLength = true;
            }
            else
            {
                chk_CheckSmear_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckExcessPadLength = false;
            }

            if ((intFailMask & 0x4000) > 0)
            {
                chk_CheckEdgeLimit_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadEdgeLimit = true;
            }
            else
            {
                chk_CheckEdgeLimit_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadEdgeLimit = false;
            }

            if ((intFailMask & 0x8000) > 0)
            {
                chk_CheckStandOff_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadStandOff = true;
            }
            else
            {
                chk_CheckStandOff_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadStandOff = false;
            }

            if ((intFailMask & 0x10000) > 0)
            {
                chk_CheckEdgeDistance_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadEdgeDistance = true;
            }
            else
            {
                chk_CheckEdgeDistance_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadEdgeDistance = false;
            }

            if ((intFailMask & 0x20000) > 0)
            {
                chk_CheckSpanX_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadSpanX = true;
            }
            else
            {
                chk_CheckSpanX_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadSpanX = false;
            }

            if ((intFailMask & 0x40000) > 0)
            {
                chk_CheckSpanY_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadSpanY = true;
            }
            else
            {
                chk_CheckSpanY_Pad.Checked = m_smVisionInfo.g_arrPad[0].ref_blnWantCheckPadSpanY = false;
            }

            chk_CheckForeignMaterialTotalArea_Pad.Checked = (intFailMask & 0x1000) > 0;
            //chk_CheckEmpty.Checked = (intFailMask & 0x4000) > 0;

            //chk_CheckPosition.Checked = (intFailMask & 0x8000) > 0;

            //------------------------------------Pin1--------------------------------
            if (m_smVisionInfo.g_blnWantPin1)
            {
                chk_WantInspectPin1.Checked = m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate);
            }

            // 2019 04 17-CCENG: Hide Foreign Material CheckBox (under Bottom Pad tab page) if Package is ON. Because Foreign Material/Contamination will be checked under package inspectoin.
            //2020-09-25 ZJYEOH : Open back Foreign Material when user dont use gauge for pad
            if ((m_smVisionInfo.g_strVisionName == "Pad5SPkg" || m_smVisionInfo.g_strVisionName == "PadPkg") && m_smVisionInfo.g_arrPad[0].ref_blnWantGaugeMeasurePkgSize && !m_smVisionInfo.g_arrPad[0].ref_blnWantViewCheckForeignMaterialOptionWhenPackageON)
            {
                pnl_CheckForeignMaterialArea_Pad.Visible = false;
                pnl_CheckForeignMaterialTotalArea_Pad.Visible = false;
                pnl_CheckForeignMaterialLength_Pad.Visible = false;
                pnl_Pad.Size = new Size(pnl_Pad.Size.Width, pnl_Pad.Size.Height - pnl_CheckForeignMaterialArea_Pad.Size.Height - pnl_CheckForeignMaterialTotalArea_Pad.Size.Height - pnl_CheckForeignMaterialLength_Pad.Size.Height);
                //chk_Pin1_Pad.Location = chk_CheckForeignMaterialArea_Pad.Location;
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

        private void UpdateSidePadFailMaskGUI()
        {
            m_intFailMaskSidePad_Previous = m_smVisionInfo.g_arrPad[1].ref_intFailOptionMask;
            m_blnFailMaskSidePadExtraLength_Previous = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadLength;
            m_blnFailMaskSidePadExtraArea_Previous = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadArea;
            m_blnFailMaskSidePadBrokenLength_Previous = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadLength;
            m_blnFailMaskSidePadBrokenArea_Previous = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadArea;

            int intFailMask = m_smVisionInfo.g_arrPad[1].ref_intFailOptionMask;

            // Area
            chk_Area_SidePad.Checked = (intFailMask & 0x20) > 0;

            // Width and Height
            if ((intFailMask & 0xC0) > 0)
            {
                intFailMask |= 0xC0;    // Make sure both Width and Height are ON
                chk_WidthHeight_SidePad.Checked = true;
            }
            else
            {
                intFailMask &= ~0xC0;   // Make sure both Width and Height are OFF
                chk_WidthHeight_SidePad.Checked = false;
            }

            // Off Set
            chk_OffSet_SidePad.Checked = (intFailMask & 0x100) > 0;

            // Foreign Material / Contamination
            if ((intFailMask & 0x01) > 0)
            {
                chk_CheckForeignMaterialArea_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadArea;
                chk_CheckForeignMaterialLength_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadLength;
            }
            else
            {
                chk_CheckForeignMaterialArea_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadArea = false;
                chk_CheckForeignMaterialLength_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckExtraPadLength = false;
            }

            // Broken Pad 
            if ((intFailMask & 0x08) > 0 || (intFailMask & 0x10) > 0)
            {
                if ((intFailMask & 0x08) <= 0)  // Splite or Scratch
                    intFailMask |= 0x08;

                if ((intFailMask & 0x10) <= 0)  // Hole
                    intFailMask |= 0x10;

                //chk_BrokenPad.Checked = true;
                chk_CheckBrokenArea_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadArea;
                chk_CheckBrokenLength_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadLength;
            }
            else
            {
                //chk_BrokenPad.Checked = false;
                chk_CheckBrokenArea_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadArea = false;
                chk_CheckBrokenLength_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckBrokenPadLength = false;
            }

            // Pitch Gap
            if ((intFailMask & 0x200) > 0 || (intFailMask & 0x400) > 0)
            {
                if ((intFailMask & 0x200) <= 0)
                    intFailMask |= 0x200;

                if ((intFailMask & 0x400) <= 0)
                    intFailMask |= 0x400;

                chk_Gap_SidePad.Checked = true;
            }
            else
            {
                chk_Gap_SidePad.Checked = false;
            }

            if ((intFailMask & 0x800) > 0)
            {
                chk_CheckExcess_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckExcessPadArea = true;
                //m_smVisionInfo.g_arrPad[1].ref_blnWantCheckExcessPadLength = false;
            }
            else
            {
                chk_CheckExcess_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckExcessPadArea = false;
                //m_smVisionInfo.g_arrPad[1].ref_blnWantCheckExcessPadLength = false;
            }

            if ((intFailMask & 0x2000) > 0)
            {
                chk_CheckSmear_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckExcessPadLength = true;
            }
            else
            {
                chk_CheckSmear_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckExcessPadLength = false;
            }

            if ((intFailMask & 0x4000) > 0)
            {
                chk_CheckEdgeLimit_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckPadEdgeLimit = true;
            }
            else
            {
                chk_CheckEdgeLimit_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckPadEdgeLimit = false;
            }

            if ((intFailMask & 0x8000) > 0)
            {
                chk_CheckStandOff_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckPadStandOff = true;
            }
            else
            {
                chk_CheckStandOff_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckPadStandOff = false;
            }

            if ((intFailMask & 0x10000) > 0)
            {
                chk_CheckEdgeDistance_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckPadEdgeDistance = true;
            }
            else
            {
                chk_CheckEdgeDistance_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckPadEdgeDistance = false;
            }

            if ((intFailMask & 0x20000) > 0)
            {
                chk_CheckSpanX_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckPadSpanX = true;
            }
            else
            {
                chk_CheckSpanX_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckPadSpanX = false;
            }

            if ((intFailMask & 0x40000) > 0)
            {
                chk_CheckSpanY_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckPadSpanY = true;
            }
            else
            {
                chk_CheckSpanY_SidePad.Checked = m_smVisionInfo.g_arrPad[1].ref_blnWantCheckPadSpanY = false;
            }

            chk_CheckForeignMaterialTotalArea_SidePad.Checked = (intFailMask & 0x1000) > 0;

            //2020-09-25 ZJYEOH : Open back Foreign Material when user dont use gauge for pad
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

        private void UpdateCenterPacakgePadFailMaskGUI()
        {
            m_intFailMaskCenterPadPackage_Previous = m_smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask;
            m_blnFailMaskCenterPadPackageDefect_Previous = m_smVisionInfo.g_arrPad[0].GetWantInspectPackage();

            int intPkgFailMask = m_smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask;

            chk_InspectPackage_Pad2.Checked = m_smVisionInfo.g_arrPad[0].GetWantInspectPackage();
            chk_CheckPkgSize_Pad2.Checked = (intPkgFailMask & 0x01) > 0;

            chk_BrightFieldArea_Pad.Checked = (intPkgFailMask & 0x10000) > 0;
            chk_BrightFieldLength_Pad.Checked = (intPkgFailMask & 0x20000) > 0;
            chk_DarkFieldArea_Pad.Checked = (intPkgFailMask & 0x40000) > 0;
            chk_DarkFieldLength_Pad.Checked = (intPkgFailMask & 0x80000) > 0;
            chk_CrackDarkFieldArea_Pad.Checked = (intPkgFailMask & 0x800) > 0;
            chk_CrackDarkFieldLength_Pad.Checked = (intPkgFailMask & 0x400) > 0;
            chk_ChippedOffDarkFieldArea_Pad.Checked = (intPkgFailMask & 0x100000) > 0;
            chk_ChippedOffBrightFieldArea_Pad.Checked = (intPkgFailMask & 0x200000) > 0;
            chk_MoldFlashBrightFieldArea_Pad.Checked = (intPkgFailMask & 0x80) > 0;
            chk_MoldFlashBrightFieldLength_Pad.Checked = (intPkgFailMask & 0x1000000) > 0;
            chk_ForeignMaterialBrightFieldArea_Pad.Checked = (intPkgFailMask & 0x400000) > 0;
            chk_ForeignMaterialBrightFieldLength_Pad.Checked = (intPkgFailMask & 0x800000) > 0;

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


            if (chk_InspectPackage_Pad2.Checked)
            {
                chk_BrightFieldArea_Pad.Enabled = true;
                chk_BrightFieldLength_Pad.Enabled = true;
                chk_DarkFieldArea_Pad.Enabled = true;
                chk_DarkFieldLength_Pad.Enabled = true;
                chk_CrackDarkFieldArea_Pad.Enabled = true;
                chk_CrackDarkFieldLength_Pad.Enabled = true;
                chk_ChippedOffDarkFieldArea_Pad.Enabled = true;
                chk_ChippedOffBrightFieldArea_Pad.Enabled = true;
                chk_MoldFlashBrightFieldArea_Pad.Enabled = true;
                chk_MoldFlashBrightFieldLength_Pad.Enabled = true;
                chk_ForeignMaterialBrightFieldArea_Pad.Enabled = true;
                chk_ForeignMaterialBrightFieldLength_Pad.Enabled = true;
            }
            else
            {
                chk_BrightFieldArea_Pad.Enabled = false;
                chk_BrightFieldLength_Pad.Enabled = false;
                chk_DarkFieldArea_Pad.Enabled = false;
                chk_DarkFieldLength_Pad.Enabled = false;
                chk_CrackDarkFieldArea_Pad.Enabled = false;
                chk_CrackDarkFieldLength_Pad.Enabled = false;
                chk_ChippedOffDarkFieldArea_Pad.Enabled = false;
                chk_ChippedOffBrightFieldArea_Pad.Enabled = false;
                chk_MoldFlashBrightFieldArea_Pad.Enabled = false;
                chk_MoldFlashBrightFieldLength_Pad.Enabled = false;
                chk_ForeignMaterialBrightFieldArea_Pad.Enabled = false;
                chk_ForeignMaterialBrightFieldLength_Pad.Enabled = false;
            }
        }

        private void UpdateSidePacakgePadFailMaskGUI()
        {
            m_intFailMaskSidePadPackage_Previous = m_smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask;
            m_blnFailMaskSidePadPackageDefect_Previous = m_smVisionInfo.g_arrPad[1].GetWantInspectPackage();

            int intPkgFailMask = m_smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask;

            chk_InspectPackage_SidePad2.Checked = m_smVisionInfo.g_arrPad[1].GetWantInspectPackage();
            chk_CheckPkgSize_SidePad2.Checked = (intPkgFailMask & 0x01) > 0;

            chk_BrightFieldArea_SidePad.Checked = (intPkgFailMask & 0x10000) > 0;
            chk_BrightFieldLength_SidePad.Checked = (intPkgFailMask & 0x20000) > 0;
            chk_DarkFieldArea_SidePad.Checked = (intPkgFailMask & 0x40000) > 0;
            chk_DarkFieldLength_SidePad.Checked = (intPkgFailMask & 0x80000) > 0;
            chk_CrackDarkFieldArea_SidePad.Checked = (intPkgFailMask & 0x800) > 0;
            chk_CrackDarkFieldLength_SidePad.Checked = (intPkgFailMask & 0x400) > 0;
            chk_ChippedOffDarkFieldArea_SidePad.Checked = (intPkgFailMask & 0x100000) > 0;
            chk_ChippedOffBrightFieldArea_SidePad.Checked = (intPkgFailMask & 0x200000) > 0;
            chk_MoldFlashBrightFieldArea_SidePad.Checked = (intPkgFailMask & 0x80) > 0;
            chk_MoldFlashBrightFieldLength_SidePad.Checked = (intPkgFailMask & 0x1000000) > 0;
            chk_ForeignMaterialBrightFieldArea_SidePad.Checked = (intPkgFailMask & 0x400000) > 0;
            chk_ForeignMaterialBrightFieldLength_SidePad.Checked = (intPkgFailMask & 0x800000) > 0;

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


            if (chk_InspectPackage_SidePad2.Checked)
            {
                chk_BrightFieldArea_SidePad.Enabled = true;
                chk_BrightFieldLength_SidePad.Enabled = true;
                chk_DarkFieldArea_SidePad.Enabled = true;
                chk_DarkFieldLength_SidePad.Enabled = true;
                chk_CrackDarkFieldArea_SidePad.Enabled = true;
                chk_CrackDarkFieldLength_SidePad.Enabled = true;
                chk_ChippedOffDarkFieldArea_SidePad.Enabled = true;
                chk_ChippedOffBrightFieldArea_SidePad.Enabled = true;
                chk_MoldFlashBrightFieldArea_SidePad.Enabled = true;
                chk_MoldFlashBrightFieldLength_SidePad.Enabled = true;
                chk_ForeignMaterialBrightFieldArea_SidePad.Enabled = true;
                chk_ForeignMaterialBrightFieldLength_SidePad.Enabled = true;
            }
            else
            {
                chk_BrightFieldArea_SidePad.Enabled = false;
                chk_BrightFieldLength_SidePad.Enabled = false;
                chk_DarkFieldArea_SidePad.Enabled = false;
                chk_DarkFieldLength_SidePad.Enabled = false;
                chk_CrackDarkFieldArea_SidePad.Enabled = false;
                chk_CrackDarkFieldLength_SidePad.Enabled = false;
                chk_ChippedOffDarkFieldArea_SidePad.Enabled = false;
                chk_ChippedOffBrightFieldArea_SidePad.Enabled = false;
                chk_MoldFlashBrightFieldArea_SidePad.Enabled = false;
                chk_MoldFlashBrightFieldLength_SidePad.Enabled = false;
                chk_ForeignMaterialBrightFieldArea_SidePad.Enabled = false;
                chk_ForeignMaterialBrightFieldLength_SidePad.Enabled = false;
            }
        }
        private void UpdateCenterColorPadFailMaskGUI()
        {
            m_intFailMaskCenterColorPad_Previous = m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask;

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
                        chk_ColorPad1Length_Center.Checked = ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x01) > 0);
                        //chk_ColorPad1Area_Center.Visible = true;
                        chk_ColorPad1Area_Center.Checked = ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x02) > 0);
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
                        chk_ColorPad2Length_Center.Checked = ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x04) > 0);
                        //chk_ColorPad2Area_Center.Visible = true;
                        chk_ColorPad2Area_Center.Checked = ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x08) > 0);
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
                        chk_ColorPad3Length_Center.Checked = ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x10) > 0);
                        //chk_ColorPad3Area_Center.Visible = true;
                        chk_ColorPad3Area_Center.Checked = ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x20) > 0);
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
                        chk_ColorPad4Length_Center.Checked = ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x40) > 0);
                        //chk_ColorPad4Area_Center.Visible = true;
                        chk_ColorPad4Area_Center.Checked = ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x80) > 0);
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
                        chk_ColorPad5Length_Center.Checked = ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x100) > 0);
                        //chk_ColorPad5Area_Center.Visible = true;
                        chk_ColorPad5Area_Center.Checked = ((m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask & 0x200) > 0);
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
        private void UpdateSideColorPadFailMaskGUI()
        {
            m_intFailMaskSideColorPad_Top_Previous = m_smVisionInfo.g_arrPad[1].ref_intFailColorOptionMask;
            m_intFailMaskSideColorPad_Right_Previous = m_smVisionInfo.g_arrPad[2].ref_intFailColorOptionMask;
            m_intFailMaskSideColorPad_Bottom_Previous = m_smVisionInfo.g_arrPad[3].ref_intFailColorOptionMask;
            m_intFailMaskSideColorPad_Left_Previous = m_smVisionInfo.g_arrPad[4].ref_intFailColorOptionMask;

            //int intDefectMaxCount = 0;
            //for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
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

                        for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count; j++) //intDefectMaxCount
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
                                    chk_ColorPad1Length_Side_Top.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x01) > 0);
                                    //chk_ColorPad1Area_Side_Top.Visible = true;
                                    chk_ColorPad1Area_Side_Top.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x02) > 0);
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
                                    chk_ColorPad2Length_Side_Top.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x04) > 0);
                                    //chk_ColorPad2Area_Side_Top.Visible = true;
                                    chk_ColorPad2Area_Side_Top.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x08) > 0);
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
                                    chk_ColorPad3Length_Side_Top.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x10) > 0);
                                    //chk_ColorPad3Area_Side_Top.Visible = true;
                                    chk_ColorPad3Area_Side_Top.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x20) > 0);
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
                                    chk_ColorPad4Length_Side_Top.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x40) > 0);
                                    //chk_ColorPad4Area_Side_Top.Visible = true;
                                    chk_ColorPad4Area_Side_Top.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x80) > 0);
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
                                    chk_ColorPad5Length_Side_Top.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x100) > 0);
                                    //chk_ColorPad5Area_Side_Top.Visible = true;
                                    chk_ColorPad5Area_Side_Top.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x200) > 0);
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

                        for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count; j++) //intDefectMaxCount
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
                                    chk_ColorPad1Length_Side_Right.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x01) > 0);
                                    //chk_ColorPad1Area_Side_Right.Visible = true;
                                    chk_ColorPad1Area_Side_Right.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x02) > 0);
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
                                    chk_ColorPad2Length_Side_Right.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x04) > 0);
                                    //chk_ColorPad2Area_Side_Right.Visible = true;
                                    chk_ColorPad2Area_Side_Right.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x08) > 0);
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
                                    chk_ColorPad3Length_Side_Right.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x10) > 0);
                                    //chk_ColorPad3Area_Side_Right.Visible = true;
                                    chk_ColorPad3Area_Side_Right.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x20) > 0);
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
                                    chk_ColorPad4Length_Side_Right.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x40) > 0);
                                    //chk_ColorPad4Area_Side_Right.Visible = true;
                                    chk_ColorPad4Area_Side_Right.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x80) > 0);
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
                                    chk_ColorPad5Length_Side_Right.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x100) > 0);
                                    //chk_ColorPad5Area_Side_Right.Visible = true;
                                    chk_ColorPad5Area_Side_Right.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x200) > 0);
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

                        for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count; j++) //intDefectMaxCount
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
                                    chk_ColorPad1Length_Side_Bottom.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x01) > 0);
                                    //chk_ColorPad1Area_Side_Bottom.Visible = true;
                                    chk_ColorPad1Area_Side_Bottom.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x02) > 0);
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
                                    chk_ColorPad2Length_Side_Bottom.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x04) > 0);
                                    //chk_ColorPad2Area_Side_Bottom.Visible = true;
                                    chk_ColorPad2Area_Side_Bottom.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x08) > 0);
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
                                    chk_ColorPad3Length_Side_Bottom.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x10) > 0);
                                    //chk_ColorPad3Area_Side_Bottom.Visible = true;
                                    chk_ColorPad3Area_Side_Bottom.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x20) > 0);
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
                                    chk_ColorPad4Length_Side_Bottom.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x40) > 0);
                                    //chk_ColorPad4Area_Side_Bottom.Visible = true;
                                    chk_ColorPad4Area_Side_Bottom.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x80) > 0);
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
                                    chk_ColorPad5Length_Side_Bottom.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x100) > 0);
                                    //chk_ColorPad5Area_Side_Bottom.Visible = true;
                                    chk_ColorPad5Area_Side_Bottom.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x200) > 0);
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

                        for (int j = 0; j < m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count; j++) //intDefectMaxCount
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
                                    chk_ColorPad1Length_Side_Left.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x01) > 0);
                                    //chk_ColorPad1Area_Side_Left.Visible = true;
                                    chk_ColorPad1Area_Side_Left.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x02) > 0);
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
                                    chk_ColorPad2Length_Side_Left.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x04) > 0);
                                    //chk_ColorPad2Area_Side_Left.Visible = true;
                                    chk_ColorPad2Area_Side_Left.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x08) > 0);
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
                                    chk_ColorPad3Length_Side_Left.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x10) > 0);
                                    //chk_ColorPad3Area_Side_Left.Visible = true;
                                    chk_ColorPad3Area_Side_Left.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x20) > 0);
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
                                    chk_ColorPad4Length_Side_Left.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x40) > 0);
                                    //chk_ColorPad4Area_Side_Left.Visible = true;
                                    chk_ColorPad4Area_Side_Left.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x80) > 0);
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
                                    chk_ColorPad5Length_Side_Left.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x100) > 0);
                                    //chk_ColorPad5Area_Side_Left.Visible = true;
                                    chk_ColorPad5Area_Side_Left.Checked = ((m_smVisionInfo.g_arrPad[i].ref_intFailColorOptionMask & 0x200) > 0);
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
        private void UpdateOrientFailMaskGUI()
        {
            if (m_smVisionInfo.g_arrOrients.Count > 0)
            {
                if (m_smVisionInfo.g_arrOrients[0].Count > 0)
                {
                    m_blnFailMaskOrientAngle_Previous = chk_WantInspectOrientAngleTolerance.Checked = m_smVisionInfo.g_arrOrients[0][0].ref_blnWantCheckOrientAngleTolerance;
                    m_blnFailMaskOrientPosX_Previous = chk_WantInspectOrientXTolerance.Checked = m_smVisionInfo.g_arrOrients[0][0].ref_blnWantCheckOrientXTolerance;
                    m_blnFailMaskOrientPosY_Previous = chk_WantInspectOrientYTolerance.Checked = m_smVisionInfo.g_arrOrients[0][0].ref_blnWantCheckOrientYTolerance;

                }
            }
        }

        private void UpdateOrientationFailMaskGUI()
        {
            if (m_smVisionInfo.g_arrOrients.Count > 0)
            {
                if (m_smVisionInfo.g_arrOrients[0].Count > 0)
                {
                    m_blnFailMaskOrientation_Previous = chk_InspectOrient_ForMO.Checked = m_smVisionInfo.g_arrOrients[0][0].ref_blnWantCheckOrientation;
                }
            }
        }

        private void UpdateOrientPadFailMaskGUI()
        {
            if (m_smVisionInfo.g_objPadOrient != null)
            {
                    m_blnFailMaskOrientAngle_Previous = chk_WantInspectOrientAngleTolerance_Pad.Checked = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientAngleTolerance;
                    m_blnFailMaskOrientPosX_Previous = chk_WantInspectOrientXTolerance_Pad.Checked = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientXTolerance;
                    m_blnFailMaskOrientPosY_Previous = chk_WantInspectOrientYTolerance_Pad.Checked = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientYTolerance;
            }
        }
        private void UpdateMarkFailMaskGUI()
        {
            //------------------------------------Mark--------------------------------
            m_blnWantCheckMark_Previous = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_blnCheckMark;
            m_intFailMaskMark_Previous = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);

            chk_CheckMark.Checked = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].ref_blnCheckMark;
            EnableMarkFailOptions(chk_CheckMark.Checked);

            int intFailMask = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);

            //if (((m_intVisionType & 0x08) > 0) && m_smVisionInfo.g_blnCheckPackage)
            //{
            //    chk_ExtraMark.Enabled = false;
            //    chk_ExtraMarkUncheckArea.Enabled = false;
            //    chk_ExtraMarkCharArea.Enabled = false;
            //    chk_GroupExtraMark.Enabled = false;
            //}

            //if ((intFailMask & 0x01) > 0 && (m_intVisionType & 0x08) == 0)
            if ((intFailMask & 0x01) > 0)
            {
                chk_ExcessMarkCharArea.Checked = true;
            }
            else
            {
                chk_ExcessMarkCharArea.Checked = false;
            }

            if (m_smVisionInfo.g_blnWantCheckMarkTotalExcess)
            {
                if ((intFailMask & 0x100) > 0)
                {
                    chk_GroupExcessMark.Checked = true;
                }
                else
                {
                    chk_GroupExcessMark.Checked = false;
                }
            }

            if (m_smVisionInfo.g_blnWantCheckMarkAverageGrayValue)
            {
                if ((intFailMask & 0x200) > 0)
                {
                    chk_MarkAverageGrayValue.Checked = true;
                }
                else
                {
                    chk_MarkAverageGrayValue.Checked = false;
                }
            }

            //if ((intFailMask & 0x02) > 0 && (m_intVisionType & 0x08) == 0)
            if ((intFailMask & 0x02) > 0)
            {
                chk_ExtraMark.Checked = true;
            }
            else
            {
                chk_ExtraMark.Checked = false;
            }

            //if ((intFailMask & 0x04) > 0 && (m_intVisionType & 0x08) == 0)
            if ((intFailMask & 0x04) > 0)
            {
                chk_ExtraMarkUncheckArea.Checked = true;
            }
            else
            {
                chk_ExtraMarkUncheckArea.Checked = false;
            }

            //if ((intFailMask & 0x08) > 0 && (m_intVisionType & 0x08) == 0)
            if ((intFailMask & 0x08) > 0)
            {
                chk_GroupExtraMark.Checked = true;
            }
            else
            {
                chk_GroupExtraMark.Checked = false;
            }

            if ((intFailMask & 0x10) > 0)
            {
                chk_MissingMark.Checked = true;
            }
            else
            {
                chk_MissingMark.Checked = false;
            }

            if (m_smVisionInfo.g_blnWantCheckMarkBroken)
            {
                pnl_BrokenMark.Visible = true;
                if ((intFailMask & 0x20) > 0)
                {
                    chk_BrokenMark.Checked = true;
                }
                else
                {
                    chk_BrokenMark.Checked = false;
                }
            }
            else
            {
                chk_BrokenMark.Checked = false;
                pnl_BrokenMark.Visible = false;
            }

            if ((intFailMask & 0x40) > 0)
            {
                chk_TextShifted.Checked = true;
            }
            else
            {
                chk_TextShifted.Checked = false;
            }

            if ((intFailMask & 0x80) > 0)
            {
                chk_JointMark.Checked = true;
            }
            else
            {
                chk_JointMark.Checked = false;
            }

            if (m_smVisionInfo.g_blnWantCheckMarkAngle)
            {
                pnl_MarkAngle.Visible = true;

                if ((intFailMask & 0x2000) > 0)
                {
                    chk_MarkAngle.Checked = true;
                }
                else
                {
                    chk_MarkAngle.Checked = false;
                }
            }
            else
            {
                chk_MarkAngle.Checked = false;
                pnl_MarkAngle.Visible = false;
            }

            if (!m_smVisionInfo.g_blnWantGauge)
            {
                if (m_smVisionInfo.g_strVisionName.Contains("Pkg"))
                {
                    chk_ExtraMarkUncheckArea.Enabled = false;
                    m_blnMustDisableExtraMarkSide = true;

                    if (chk_ExtraMarkUncheckArea.Checked)
                    {
                        chk_ExtraMarkUncheckArea.Checked = false;

                        intFailMask &= ~0x04;
                        m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetFailOptionMask(intFailMask, true);

                        string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                m_smVisionInfo.g_strVisionFolderName + "\\";

                        SaveMarkSettings(strPath + "Mark\\Template\\");
                    }

                }
                else
                {
                    chk_ExtraMarkUncheckArea.Enabled = false;
                    m_blnMustDisableExtraMarkSide = true;
                    chk_TextShifted.Enabled = false;

                    if (chk_ExtraMarkUncheckArea.Checked || chk_TextShifted.Checked)//(chk_ExtraMark.Checked || chk_ExtraMarkUncheckArea.Checked || chk_GroupExtraMark.Checked || chk_TextShifted.Checked)
                    {
                        //chk_ExtraMark.Checked = false;
                        chk_ExtraMarkUncheckArea.Checked = false;
                        //chk_GroupExtraMark.Checked = false;
                        chk_TextShifted.Checked = false;

                        intFailMask &= ~0x44; // 4E
                        m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].SetFailOptionMask(intFailMask, true);

                        string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                                m_smVisionInfo.g_strVisionFolderName + "\\";

                        SaveMarkSettings(strPath + "Mark\\Template\\");
                    }
                }
            }

            //------------------------------------Pin1--------------------------------
            if (m_smVisionInfo.g_blnWantPin1)
            {
                chk_WantInspectPin1.Checked = m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate);

                for (int i = 0; i < cbo_TemplateNo.Items.Count; i++)
                    m_arrFailMaskPin1_Previous.Add(m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(i));
            }
        }

        private void UpdateLeadFailMaskGUI()
        {
            //------------------------------------Lead--------------------------------
            if (m_smVisionInfo.g_intSelectedROI >= m_smVisionInfo.g_arrLead.Length)
                m_smVisionInfo.g_intSelectedROI = 0;
            m_blnFailMaskInspectLead_Previous = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetWantInspectLead();
            m_intFailMaskLead_Previous = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intFailOptionMask;
            m_blnFailMaskLeadExtraLength_Previous = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_blnWantCheckExtraLeadLength;
            m_blnFailMaskLeadExtraArea_Previous = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_blnWantCheckExtraLeadArea;

            int intLeadFailMask = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intFailOptionMask;

            // Width and Height
            if ((intLeadFailMask & 0xC0) > 0)
            {
                intLeadFailMask |= 0xC0;    // Make sure both Width and Height are ON
                chk_WidthHeight_Lead.Checked = true;
            }
            else
            {
                intLeadFailMask &= ~0xC0;   // Make sure both Width and Height are OFF
                chk_WidthHeight_Lead.Checked = false;
            }

            // Off Set
            chk_OffSet_Lead.Checked = (intLeadFailMask & 0x100) > 0;

            // Skew
            chk_Skew_Lead.Checked = (intLeadFailMask & 0x8000) > 0;
            //chk_Skew_Lead.Visible = !m_smVisionInfo.g_arrLead[0].ref_blnWantUsePkgToBaseTolerance; //2020-08-12 ZJYEOH : Hide Skew if use package to base method

            // Pitch Gap
            if ((intLeadFailMask & 0x200) > 0 || (intLeadFailMask & 0x400) > 0)
            {
                if ((intLeadFailMask & 0x200) <= 0)
                    intLeadFailMask |= 0x200;

                if ((intLeadFailMask & 0x400) <= 0)
                    intLeadFailMask |= 0x400;

                chk_PitchGap_Lead.Checked = true;
            }
            else
            {
                chk_PitchGap_Lead.Checked = false;
            }

            chk_Variance_Lead.Checked = (intLeadFailMask & 0x800) > 0;

            chk_AverageGrayValue_Lead.Checked = (intLeadFailMask & 0x4000) > 0;

            if (!m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_blnWantUseAverageGrayValueMethod)
            {
                pnl_AverageGrayValue_Lead.Visible = false;
            }

            chk_Span_Lead.Checked = (intLeadFailMask & 0x1000) > 0;

            //chk_CheckEmpty.Checked = (intLeadFailMask & 0x4000) > 0;

            //chk_CheckPosition.Checked = (intLeadFailMask & 0x8000) > 0;

            // Foreign Material / Contamination
            if ((intLeadFailMask & 0x01) > 0)
            {
                chk_CheckForeignMaterialArea_Lead.Checked = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_blnWantCheckExtraLeadArea;
                chk_CheckForeignMaterialLength_Lead.Checked = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_blnWantCheckExtraLeadLength;
            }
            else
            {
                chk_CheckForeignMaterialArea_Lead.Checked = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_blnWantCheckExtraLeadArea = false;
                chk_CheckForeignMaterialLength_Lead.Checked = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_blnWantCheckExtraLeadLength = false;
            }

            chk_CheckForeignMaterialTotalArea_Lead.Checked = (intLeadFailMask & 0x2000) > 0;

            chk_BaseLeadOffset.Checked = (intLeadFailMask & 0x10000) > 0;

            chk_BaseLeadArea.Checked = (intLeadFailMask & 0x20000) > 0;

            if (!m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_blnWantInspectBaseLead)
            {
                pnl_BaseLeadOffset.Visible = pnl_BaseLeadArea.Visible = false;
            }

            chk_InspectLead.Checked = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].GetWantInspectLead();

            if (chk_InspectLead.Checked)
            {
                chk_WidthHeight_Lead.Enabled = true;
                chk_OffSet_Lead.Enabled = true;
                chk_Skew_Lead.Enabled = true;
                chk_PitchGap_Lead.Enabled = true;
                chk_Variance_Lead.Enabled = true;
                chk_AverageGrayValue_Lead.Enabled = true;
                chk_Span_Lead.Enabled = true;
                chk_CheckForeignMaterialArea_Lead.Enabled = true;
                chk_CheckForeignMaterialLength_Lead.Enabled = true;
                chk_CheckForeignMaterialTotalArea_Lead.Enabled = true;
                chk_BaseLeadOffset.Enabled = true;
                chk_BaseLeadArea.Enabled = true;
            }
            else
            {
                chk_WidthHeight_Lead.Enabled = false;
                chk_OffSet_Lead.Enabled = false;
                chk_Skew_Lead.Enabled = false;
                chk_PitchGap_Lead.Enabled = false;
                chk_Variance_Lead.Enabled = false;
                chk_AverageGrayValue_Lead.Enabled = false;
                chk_Span_Lead.Enabled = false;
                chk_CheckForeignMaterialArea_Lead.Enabled = false;
                chk_CheckForeignMaterialLength_Lead.Enabled = false;
                chk_CheckForeignMaterialTotalArea_Lead.Enabled = false;
                chk_BaseLeadOffset.Enabled = false;
                chk_BaseLeadArea.Enabled = false;
            }
        }
        private void UpdateColorPackageFailMaskGUI()
        {
            m_intFailMaskColorPackage_Previous = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask;

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
                        chk_ColorPackage1Length.Checked = ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x01) > 0);
                        //chk_ColorPackage1Area.Visible = true;
                        chk_ColorPackage1Area.Checked = ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x02) > 0);
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
                        chk_ColorPackage2Length.Checked = ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x04) > 0);
                        //chk_ColorPackage2Area.Visible = true;
                        chk_ColorPackage2Area.Checked = ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x08) > 0);
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
                        chk_ColorPackage3Length.Checked = ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x10) > 0);
                        //chk_ColorPackage3Area.Visible = true;
                        chk_ColorPackage3Area.Checked = ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x20) > 0);
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
                        chk_ColorPackage4Length.Checked = ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x40) > 0);
                        //chk_ColorPackage4Area.Visible = true;
                        chk_ColorPackage4Area.Checked = ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x80) > 0);
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
                        chk_ColorPackage5Length.Checked = ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x100) > 0);
                        //chk_ColorPackage5Area.Visible = true;
                        chk_ColorPackage5Area.Checked = ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask & 0x200) > 0);
                    }
                }
            }

        }
        private void UpdatePackageFailMaskGUI()
        {
            m_intFailMaskPackage_Previous = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask;
            m_blnFailMaskPackageDefect_Previous = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantInspectPackage();

            m_arrFailMaskDefectLength_Previous[0] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Bright);//chk_BrightFieldLength.Checked
            m_arrFailMaskDefectLength_Previous[1] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark); //chk_DarkFieldLength.Checked
            m_arrFailMaskDefectLength_Previous[2] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Crack); //chk_CrackDarkFieldLength.Checked
            m_arrFailMaskDefectLength_Previous[3] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Void); //chk_VoidDarkFieldLength.Checked
            m_arrFailMaskDefectLength_Previous[4] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark2); //chk_DarkField2Length.Checked
            m_arrFailMaskDefectLength_Previous[5] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark3); //chk_DarkField3Length.Checked
            m_arrFailMaskDefectLength_Previous[6] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark4);//chk_DarkField4Length.Checked
            m_arrFailMaskDefectLength_Previous[7] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipBright);//chk_CheckChippedOffBright_Length.Checked
            m_arrFailMaskDefectLength_Previous[8] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipDark);//chk_CheckChippedOffDark_Length.Checked

            m_arrFailMaskDefectArea_Previous[0] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Bright); //chk_BrightFieldArea.Checked
            m_arrFailMaskDefectArea_Previous[1] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark); //chk_DarkFieldArea.Checked
            m_arrFailMaskDefectArea_Previous[2] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Crack); //chk_CrackDarkFieldArea.Checked
            m_arrFailMaskDefectArea_Previous[3] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Void);//chk_VoidDarkFieldArea.Checked
            m_arrFailMaskDefectArea_Previous[4] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.MoldFlash);//chk_MoldFlashBrightFieldArea.Checked
            m_arrFailMaskDefectArea_Previous[5] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipBright);//chk_CheckChippedOffBright_Area.Checked
            m_arrFailMaskDefectArea_Previous[6] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipDark);//chk_CheckChippedOffDark_Area.Checked
            m_arrFailMaskDefectArea_Previous[7] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark2);//chk_DarkField2Area.Checked
            m_arrFailMaskDefectArea_Previous[8] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark3); //chk_DarkField3Area.Checked
            m_arrFailMaskDefectArea_Previous[9] = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark4); //chk_DarkField4Area.Checked

            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomPosition") || (m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0 && m_smVisionInfo.g_strVisionName.Contains("BottomOrient"))
            {
                chk_CheckPkgSize2.Checked = (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x1000) > 0;
                pnl_PackageAngle.Visible = false;
                pnl_InspectPackage2.Visible = false;
                pnl_BrightFieldArea.Visible = false;
                pnl_BrightFieldLength.Visible = false;
                pnl_DarkFieldArea.Visible = false;
                pnl_DarkFieldLength.Visible = false;
                pnl_DarkField2Area.Visible = false;
                pnl_DarkField2Length.Visible = false;
                pnl_DarkField3Area.Visible = false;
                pnl_DarkField3Length.Visible = false;
                pnl_DarkField4Area.Visible = false;
                pnl_DarkField4Length.Visible = false;
                pnl_CrackDarkFieldArea.Visible = false;
                pnl_CrackDarkFieldLength.Visible = false;
                pnl_MoldFlashBrightFieldArea.Visible = false;
                pnl_CheckChippedOffBright_Area.Visible = false;
                pnl_CheckChippedOffBright_Length.Visible = false;
                pnl_CheckChippedOffDark_Area.Visible = false;
                pnl_CheckChippedOffDark_Length.Visible = false;
                return;
            }

            chk_InspectPackage2.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantInspectPackage();
            if (chk_InspectPackage2.Checked == false)
            {
                chk_BrightFieldArea.Enabled = false;
                chk_BrightFieldLength.Enabled = false;
                chk_DarkFieldArea.Enabled = false;
                chk_DarkFieldLength.Enabled = false;
                chk_DarkField2Area.Enabled = false;
                chk_DarkField2Length.Enabled = false;
                chk_DarkField3Area.Enabled = false;
                chk_DarkField3Length.Enabled = false;
                chk_DarkField4Area.Enabled = false;
                chk_DarkField4Length.Enabled = false;
                chk_CrackDarkFieldArea.Enabled = false;
                chk_CrackDarkFieldLength.Enabled = false;
                chk_VoidDarkFieldArea.Enabled = false;
                chk_VoidDarkFieldLength.Enabled = false;
                chk_MoldFlashBrightFieldArea.Enabled = false;
                chk_CheckChippedOffBright_Area.Enabled = false;
                chk_CheckChippedOffBright_Length.Enabled = false;
                chk_CheckChippedOffDark_Area.Enabled = false;
                chk_CheckChippedOffDark_Length.Enabled = false;
            }
            else
            {
                chk_BrightFieldArea.Enabled = true;
                chk_BrightFieldLength.Enabled = true;
                chk_DarkFieldArea.Enabled = true;
                chk_DarkFieldLength.Enabled = true;
                chk_DarkField2Area.Enabled = true;
                chk_DarkField2Length.Enabled = true;
                chk_DarkField3Area.Enabled = true;
                chk_DarkField3Length.Enabled = true;
                chk_DarkField4Area.Enabled = true;
                chk_DarkField4Length.Enabled = true;
                chk_CrackDarkFieldArea.Enabled = true;
                chk_CrackDarkFieldLength.Enabled = true;
                chk_VoidDarkFieldArea.Enabled = true;
                chk_VoidDarkFieldLength.Enabled = true;
                chk_MoldFlashBrightFieldArea.Enabled = true;
                chk_CheckChippedOffBright_Area.Enabled = true;
                chk_CheckChippedOffBright_Length.Enabled = true;
                chk_CheckChippedOffDark_Area.Enabled = true;
                chk_CheckChippedOffDark_Length.Enabled = true;
            }

            chk_CheckPkgSize2.Checked = (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x1000) > 0;

            if (m_smVisionInfo.g_blnWantCheckPackageAngle)
            {
                pnl_PackageAngle.Visible = true;
                chk_CheckPkgAngle.Checked = (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x2000) > 0;
            }
            else
            {
                pnl_PackageAngle.Visible = false;
                chk_CheckPkgAngle.Checked = false;
            }

            if ((m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x100) == 0)

            chk_BrightFieldArea.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Bright)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x100) > 0;
            chk_BrightFieldLength.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Bright) && (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x100) > 0;
            chk_DarkFieldArea.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x200) > 0;
            chk_DarkFieldLength.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark) && (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x200) > 0;
            chk_DarkField2Area.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark2)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x400) > 0;
            chk_DarkField2Length.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark2)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x400) > 0;
            chk_DarkField3Area.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark3)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x800) > 0;
            chk_DarkField3Length.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark3)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x800) > 0;
            chk_DarkField4Area.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Dark4)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x4000) > 0;
            chk_DarkField4Length.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Dark4)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x4000) > 0;
            chk_CrackDarkFieldArea.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Crack)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x01) > 0;
            chk_CrackDarkFieldLength.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Crack)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x01) > 0;
            chk_VoidDarkFieldArea.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.Void);
            chk_VoidDarkFieldLength.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.Void);
            chk_MoldFlashBrightFieldArea.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.MoldFlash)&&(m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x08) > 0;
            chk_CheckChippedOffBright_Area.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipBright)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x02) > 0;
            chk_CheckChippedOffBright_Length.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipBright)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x02) > 0;
            chk_CheckChippedOffDark_Area.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectAreaParam((int)Package.eWantDefect.ChipDark)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x02) > 0;
            chk_CheckChippedOffDark_Length.Checked = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].GetWantDefectParam((int)Package.eWantDefect.ChipDark)&& (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask & 0x02) > 0;

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

        private void UpdateMultiTemplateGUI()
        {
            if ((m_smVisionInfo.g_arrMarks[0].GetNumTemplates() <= 1) || !m_smVisionInfo.g_blnWantMultiTemplates)
            {
                panel_Template.Visible = false;
                pnl_PageButton.Size = new Size(pnl_PageButton.Width, pnl_PageButton.Height - panel_Template.Height);
                pnl_PageButton.Location = new Point(pnl_PageButton.Location.X, pnl_PageButton.Location.Y + panel_Template.Height);
            }
            else
            {
                cbo_TemplateNo.Items.Clear();
                for (int i = 0; i < m_smVisionInfo.g_arrMarks[0].GetNumTemplates(); i++)
                {
                    cbo_TemplateNo.Items.Add((i + 1));
                }
            }

            // 2020 10 06 - Make sure g_intSelectedTemplate is 0. Will load wrong check box GUI if g_intSelectedTemplate not 0.
            cbo_TemplateNo.SelectedIndex = m_smVisionInfo.g_arrMarks[0].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = 0;

            chk_SetAllTemplates.Checked = m_blnWantSet1ToAll = m_smVisionInfo.g_blnWantSet1ToAll;
            chk_SetAllTemplates.Visible = !m_smVisionInfo.g_blnWantSet1ToAll;//2020-05-11 ZJYEOH : Hide Set to All checkbox when user tick set 1 to all in advance setting
        }

        private void UpdateTabPage()
        {
            switch (m_intSettingType)
            {
                case 0: // Mark Page                        
                    lbl_PageLabel.Text = "Mark Setting";
                    tp_Mark.Controls.Add(pnl_PageButton);
                    break;
                case 2:
                    lbl_PageLabel.Text = "Pin 1 Setting";
                    tp_Pin1.Controls.Add(pnl_PageButton);
                    break;
                case 3: // Package Simple Page
                    lbl_PageLabel.Text = "Package Setting";
                    //tp_PackageSimple.Controls.Add(pnl_PageButton);    // 2020 04 22 - CCENG: No multi template
                    break;
            }
        }

        //private void UpdateSelectedTemplateChange()
        //{
        //    UpdateFailMaskGUI("Mark");
        //}

        private void LoadMarkSettings(string strFolderPath)
        {
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].LoadTemplate(strFolderPath, m_smVisionInfo.g_arrMarkROIs, m_smVisionInfo.g_arrMarkDontCareROIs.Count, m_smVisionInfo.g_objWhiteImage);
            }
        }
        private void LoadOrientSettings(string strFolderPath)
        {
            if (m_smVisionInfo.g_arrOrients.Count > 0)
            {
                for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                {
                    for (int j = 0; j < m_smVisionInfo.g_arrOrients[i].Count; j++)
                        m_smVisionInfo.g_arrOrients[i][j].LoadOrient(strFolderPath + "Settings.xml", "General");
                }
            }

        }
        private void SaveOrientSettings(string strFolderPath)
        {
            if (m_smVisionInfo.g_arrOrients.Count > 0)
            {
                for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                {
                    for (int j = 0; j < m_smVisionInfo.g_arrOrients[i].Count; j++)
                        m_smVisionInfo.g_arrOrients[i][j].SaveOrient(strFolderPath + "Settings.xml", false, "General", true);
                }
            }

        }
        private void LoadOrientPadSettings(string strFolderPath)
        {
            if (m_smVisionInfo.g_objPadOrient != null)
            {
                m_smVisionInfo.g_objPadOrient.LoadOrient(strFolderPath + "Settings.xml", "General");
                m_smVisionInfo.g_objPadOrient.SetCalibrationData(
                                   m_smVisionInfo.g_fCalibPixelX,
                                   m_smVisionInfo.g_fCalibPixelY, m_smCustomizeInfo.g_intUnitDisplay);
            }

        }
        private void SaveOrientPadSettings(string strFolderPath)
        {
            if (m_smVisionInfo.g_objPadOrient != null)
            {
                m_smVisionInfo.g_objPadOrient.SaveOrient(strFolderPath + "Settings.xml", false, "General", true);
            }

        }
        private void SaveMarkSettings(string strFolderPath)
        {
            m_smVisionInfo.g_arrMarks[0].SaveTemplate(strFolderPath, false);
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

        private void LoadLeadSetting(string strFolderPath)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                m_smVisionInfo.g_arrLead[i].SetCalibrationData(
                              m_smVisionInfo.g_fCalibPixelX,
                              m_smVisionInfo.g_fCalibPixelY,
                              m_smVisionInfo.g_fCalibOffSetX,
                              m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);

                // Load Lead Template Setting
                string strSectionName = "";
                if (i == 0)
                    strSectionName = "SearchROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                m_smVisionInfo.g_arrLead[i].LoadLead(strFolderPath + "Template\\Template.xml", strSectionName, m_smVisionInfo.g_arrImages.Count);

                m_smVisionInfo.g_arrLead[i].LoadLeadTemplateImage(strFolderPath + "Template\\", i);

            }
        }

        private void LoadLead3DSetting(string strFolderPath)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
            {
                //m_smVisionInfo.g_arrLead3D[i].SetCalibrationData(
                //              m_smVisionInfo.g_fCalibPixelX,
                //              m_smVisionInfo.g_fCalibPixelY,
                //              m_smVisionInfo.g_fCalibOffSetX,
                //              m_smVisionInfo.g_fCalibOffSetY, m_smCustomizeInfo.g_intUnitDisplay);
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

                m_smVisionInfo.g_arrLead3D[i].LoadLead3D(strFolderPath + "Template\\Template.xml", strSectionName);

                m_smVisionInfo.g_arrLead3D[i].LoadLeadTemplateImage(strFolderPath + "Template\\", i);
                if (i == 0)
                    m_smVisionInfo.g_arrLead3D[i].LoadUnitPattern(strFolderPath + "Template\\PatternMatcher0.mch");
            }

            if (m_smVisionInfo.g_arrPin1 != null)
            {
                m_smVisionInfo.g_arrPin1[0].LoadTemplate(strFolderPath+ "Template\\");
            }
        }

        private bool SaveLeadSettings(string strFolderPath)
        {
            for (int i = 0; i < m_smVisionInfo.g_arrLead.Length; i++)
            {
                string strSectionName = "";
                if (i == 0)
                    strSectionName = "SearchROI";
                else if (i == 1)
                    strSectionName = "TopROI";
                else if (i == 2)
                    strSectionName = "RightROI";
                else if (i == 3)
                    strSectionName = "BottomROI";
                else if (i == 4)
                    strSectionName = "LeftROI";

                m_smVisionInfo.g_arrLead[i].SaveLead(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
            }

            return true;
        }

        private void LoadPackageSetting(string strFolderPath)
        {
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (u == 0)
                    m_smVisionInfo.g_arrPackage[u].LoadPackage(strFolderPath + "\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                else
                {
                    if (File.Exists(strFolderPath + "\\Settings2.xml"))
                        m_smVisionInfo.g_arrPackage[u].LoadPackage(strFolderPath + "\\Settings2.xml", "Settings", m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                    else
                        m_smVisionInfo.g_arrPackage[u].LoadPackage(strFolderPath + "\\Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                }

                m_smVisionInfo.g_arrPackage[u].SetBlobSettings(m_smVisionInfo.g_arrPackage[u].ref_intPkgViewThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intPkgViewMinArea,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewHighThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewLowThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewMinArea);
            }
        }

        private void SavePackageSettings(string strFolderPath)
        {
            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                if (u == 0)
                    m_smVisionInfo.g_arrPackage[u].SavePackage(strFolderPath + "\\Settings.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);
                else
                    m_smVisionInfo.g_arrPackage[u].SavePackage(strFolderPath + "\\Settings2.xml", false, "Settings", true, m_smVisionInfo.g_fCalibPixelX, m_smVisionInfo.g_fCalibPixelY);

                m_smVisionInfo.g_arrPackage[u].SetBlobSettings(m_smVisionInfo.g_arrPackage[u].ref_intPkgViewThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intPkgViewMinArea,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewHighThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewLowThreshold,
                                                               m_smVisionInfo.g_arrPackage[u].ref_intMarkViewMinArea);
            }
        }


        private void UpdateControlSetting()
        {
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrient":
                case "BottomPosition":
                    if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                    {
                        UpdateOrientControlSetting();
                    }
                    break;
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "InPocket":
                case "IPMLi":
                    UpdateMarkControlSetting();
                    if ((m_intVisionType & 0x10) > 0)
                    {
                        UpdateLeadControlSetting();
                    }
                    break;
                case "MarkPkg":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLiPkg":
                    if ((m_intVisionType & 0x10) > 0)
                    {
                        UpdateLeadControlSetting();
                    }
                    UpdateMarkControlSetting();
                    UpdatePackageControlMaskGUI();
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        UpdateColorPackageControlSetting();
                    }
                    break;
                case "MOPkg":
                case "MOLiPkg":
                    if ((m_intVisionType & 0x10) > 0)
                    {
                        UpdateLeadControlSetting();
                    }
                    UpdateOrientationControlSetting();
                    UpdateMarkControlSetting();
                    UpdatePackageControlMaskGUI();
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        UpdateColorPackageControlSetting();
                    }
                    break;
                case "Package":
                    UpdatePackageControlMaskGUI();
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        UpdateColorPackageControlSetting();
                    }
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                    UpdateOrientControlSetting();
                    UpdatePadControlSetting();
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        UpdateCenterColorPadControlSetting();
                    }
                    break;
                case "Pad":
                case "PadPos":
                    UpdatePadControlSetting();
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        UpdateCenterColorPadControlSetting();
                    }
                    break;
                case "PadPkg":
                case "PadPkgPos":
                    UpdatePadControlSetting();
                    UpdateCenterPackagePadControlSetting();
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        UpdateCenterColorPadControlSetting();
                    }
                    break;
                case "Pad5S":
                case "Pad5SPos":
                    UpdatePadControlSetting();
                    if (m_smVisionInfo.g_blnCheck4Sides)
                        UpdateSidePadControlSetting();
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        UpdateCenterColorPadControlSetting();

                        if (m_smVisionInfo.g_blnCheck4Sides)
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (i == 1)
                                    UpdateSideColorPadControlSetting_Top();

                                if (i == 2)
                                {
                                    if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0))
                                        continue;
                                    else
                                        UpdateSideColorPadControlSetting_Right();
                                }

                                if (i == 3)
                                {
                                    if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x04) > 0))
                                        continue;
                                    else
                                        UpdateSideColorPadControlSetting_Bottom();
                                }

                                if (i == 4)
                                {
                                    if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x04) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x08) > 0))
                                        continue;
                                    else
                                        UpdateSideColorPadControlSetting_Left();
                                }
                            }
                        }
                    }
                    break;
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    UpdatePadControlSetting();
                    if (m_smVisionInfo.g_blnCheck4Sides)
                        UpdateSidePadControlSetting();
                    UpdateCenterPackagePadControlSetting();
                    if (m_smVisionInfo.g_blnCheck4Sides)
                        UpdateSidePacakgePadControlSetting();
                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        UpdateCenterColorPadControlSetting();

                        if (m_smVisionInfo.g_blnCheck4Sides)
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            {
                                if (i == 1)
                                    UpdateSideColorPadControlSetting_Top();

                                if (i == 2)
                                {
                                    if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0))
                                        continue;
                                    else
                                        UpdateSideColorPadControlSetting_Right();
                                }

                                if (i == 3)
                                {
                                    if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x04) > 0))
                                        continue;
                                    else
                                        UpdateSideColorPadControlSetting_Bottom();
                                }

                                if (i == 4)
                                {
                                    if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x04) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x08) > 0))
                                        continue;
                                    else
                                        UpdateSideColorPadControlSetting_Left();
                                }
                            }
                        }
                    }
                    break;
                case "Li3D":
                    UpdateLead3DControlSetting();
                    break;
                case "Li3DPkg":
                    UpdateLead3DControlSetting();
                    UpdateLead3DPkgControlSetting();
                    break;
                case "Seal":
                    UpdateSealControlSetting();
                    break;
                case "UnitPresent":
                    break;
                default:
                    SRMMessageBox.Show("btn_Learn_Click -> There is no such vision module name " + m_smVisionInfo.g_strVisionName + " in this SRMVision software version.");
                    break;
            }
        }
        private void UpdateOrientControlSetting()
        {
            //------------------------------------Orient--------------------------------
            if ((m_smVisionInfo.g_intOptionControlMask & 0x01) > 0)
            {
                if (!chk_WantInspectOrientAngleTolerance.Text.Contains("*"))
                    chk_WantInspectOrientAngleTolerance.Text += "*";
            }
            else
            {
                if (chk_WantInspectOrientAngleTolerance.Text.Contains("*"))
                    chk_WantInspectOrientAngleTolerance.Text = chk_WantInspectOrientAngleTolerance.Text.Substring(0, chk_WantInspectOrientAngleTolerance.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x02) > 0)
            {
                if (!chk_WantInspectOrientXTolerance.Text.Contains("*"))
                    chk_WantInspectOrientXTolerance.Text += "*";
            }
            else
            {
                if (chk_WantInspectOrientXTolerance.Text.Contains("*"))
                    chk_WantInspectOrientXTolerance.Text = chk_WantInspectOrientXTolerance.Text.Substring(0, chk_WantInspectOrientXTolerance.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x04) > 0)
            {
                if (!chk_WantInspectOrientYTolerance.Text.Contains("*"))
                    chk_WantInspectOrientYTolerance.Text += "*";
            }
            else
            {
                if (chk_WantInspectOrientYTolerance.Text.Contains("*"))
                    chk_WantInspectOrientYTolerance.Text = chk_WantInspectOrientYTolerance.Text.Substring(0, chk_WantInspectOrientYTolerance.Text.Length - 1);
            }
        }

        private void UpdateOrientationControlSetting()
        {
            if ((m_smVisionInfo.g_intOptionControlMask & 0x40000000) > 0)
            {
                if (!chk_InspectOrient_ForMO.Text.Contains("*"))
                    chk_InspectOrient_ForMO.Text += "*";
            }
            else
            {
                if (chk_InspectOrient_ForMO.Text.Contains("*"))
                    chk_InspectOrient_ForMO.Text = chk_InspectOrient_ForMO.Text.Substring(0, chk_InspectOrient_ForMO.Text.Length - 1);
            }
        }

        private void UpdateOrientPadControlSetting()
        {
            //------------------------------------OrientPad--------------------------------
            if ((m_smVisionInfo.g_intOptionControlMask & 0x8000000) > 0)
            {
                if (!chk_WantInspectOrientAngleTolerance_Pad.Text.Contains("*"))
                    chk_WantInspectOrientAngleTolerance_Pad.Text += "*";
            }
            else
            {
                if (chk_WantInspectOrientAngleTolerance_Pad.Text.Contains("*"))
                    chk_WantInspectOrientAngleTolerance_Pad.Text = chk_WantInspectOrientAngleTolerance_Pad.Text.Substring(0, chk_WantInspectOrientAngleTolerance_Pad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x10000000) > 0)
            {
                if (!chk_WantInspectOrientXTolerance_Pad.Text.Contains("*"))
                    chk_WantInspectOrientXTolerance_Pad.Text += "*";
            }
            else
            {
                if (chk_WantInspectOrientXTolerance_Pad.Text.Contains("*"))
                    chk_WantInspectOrientXTolerance_Pad.Text = chk_WantInspectOrientXTolerance_Pad.Text.Substring(0, chk_WantInspectOrientXTolerance_Pad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x20000000) > 0)
            {
                if (!chk_WantInspectOrientYTolerance_Pad.Text.Contains("*"))
                    chk_WantInspectOrientYTolerance_Pad.Text += "*";
            }
            else
            {
                if (chk_WantInspectOrientYTolerance_Pad.Text.Contains("*"))
                    chk_WantInspectOrientYTolerance_Pad.Text = chk_WantInspectOrientYTolerance_Pad.Text.Substring(0, chk_WantInspectOrientYTolerance_Pad.Text.Length - 1);
            }
        }
        private void UpdateMarkControlSetting()
        {
            //------------------------------------Mark--------------------------------
            if ((m_smVisionInfo.g_intOptionControlMask & 0x01) > 0)
            {
                if (!chk_CheckMark.Text.Contains("*"))
                    chk_CheckMark.Text += "*";
            }
            else
            {
                if (chk_CheckMark.Text.Contains("*"))
                    chk_CheckMark.Text = chk_CheckMark.Text.Substring(0, chk_CheckMark.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x02) > 0)
            {
                if (!chk_ExcessMarkCharArea.Text.Contains("*"))
                    chk_ExcessMarkCharArea.Text += "*";
            }
            else
            {
                if (chk_ExcessMarkCharArea.Text.Contains("*"))
                    chk_ExcessMarkCharArea.Text = chk_ExcessMarkCharArea.Text.Substring(0, chk_ExcessMarkCharArea.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x200) > 0)
            {
                if (!chk_GroupExcessMark.Text.Contains("*"))
                    chk_GroupExcessMark.Text += "*";
            }
            else
            {
                if (chk_GroupExcessMark.Text.Contains("*"))
                    chk_GroupExcessMark.Text = chk_GroupExcessMark.Text.Substring(0, chk_GroupExcessMark.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x400) > 0)
            {
                if (!chk_MarkAverageGrayValue.Text.Contains("*"))
                    chk_MarkAverageGrayValue.Text += "*";
            }
            else
            {
                if (chk_MarkAverageGrayValue.Text.Contains("*"))
                    chk_MarkAverageGrayValue.Text = chk_MarkAverageGrayValue.Text.Substring(0, chk_MarkAverageGrayValue.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x04) > 0)
            {
                if (!chk_ExtraMark.Text.Contains("*"))
                    chk_ExtraMark.Text += "*";
            }
            else
            {
                if (chk_ExtraMark.Text.Contains("*"))
                    chk_ExtraMark.Text = chk_ExtraMark.Text.Substring(0, chk_ExtraMark.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x08) > 0)
            {
                if (!chk_ExtraMarkUncheckArea.Text.Contains("*"))
                    chk_ExtraMarkUncheckArea.Text += "*";
            }
            else
            {
                if (chk_ExtraMarkUncheckArea.Text.Contains("*"))
                    chk_ExtraMarkUncheckArea.Text = chk_ExtraMarkUncheckArea.Text.Substring(0, chk_ExtraMarkUncheckArea.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x10) > 0)
            {
                if (!chk_GroupExtraMark.Text.Contains("*"))
                    chk_GroupExtraMark.Text += "*";
            }
            else
            {
                if (chk_GroupExtraMark.Text.Contains("*"))
                    chk_GroupExtraMark.Text = chk_GroupExtraMark.Text.Substring(0, chk_GroupExtraMark.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x20) > 0)
            {
                if (!chk_MissingMark.Text.Contains("*"))
                    chk_MissingMark.Text += "*";
            }
            else
            {
                if (chk_MissingMark.Text.Contains("*"))
                    chk_MissingMark.Text = chk_MissingMark.Text.Substring(0, chk_MissingMark.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x40) > 0)
            {
                if (!chk_BrokenMark.Text.Contains("*"))
                    chk_BrokenMark.Text += "*";
            }
            else
            {
                if (chk_BrokenMark.Text.Contains("*"))
                    chk_BrokenMark.Text = chk_BrokenMark.Text.Substring(0, chk_BrokenMark.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x80) > 0)
            {
                if (!chk_TextShifted.Text.Contains("*"))
                    chk_TextShifted.Text += "*";
            }
            else
            {
                if (chk_TextShifted.Text.Contains("*"))
                    chk_TextShifted.Text = chk_TextShifted.Text.Substring(0, chk_TextShifted.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x100) > 0)
            {
                if (!chk_JointMark.Text.Contains("*"))
                    chk_JointMark.Text += "*";
            }
            else
            {
                if (chk_JointMark.Text.Contains("*"))
                    chk_JointMark.Text = chk_JointMark.Text.Substring(0, chk_JointMark.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x2000) > 0)
            {
                if (!chk_MarkAngle.Text.Contains("*"))
                    chk_MarkAngle.Text += "*";
            }
            else
            {
                if (chk_MarkAngle.Text.Contains("*"))
                    chk_MarkAngle.Text = chk_MarkAngle.Text.Substring(0, chk_MarkAngle.Text.Length - 1);
            }

            //------------------------------------Pin1--------------------------------

            if (m_smVisionInfo.g_blnWantPin1)
            {
                if ((m_smVisionInfo.g_intOptionControlMask & 0x800) > 0)
                {
                    if (!chk_WantInspectPin1.Text.Contains("*"))
                        chk_WantInspectPin1.Text += "*";
                }
                else
                {
                    if (chk_WantInspectPin1.Text.Contains("*"))
                        chk_WantInspectPin1.Text = chk_WantInspectPin1.Text.Substring(0, chk_WantInspectPin1.Text.Length - 1);
                }
            }
        }

        private void UpdateLeadControlSetting()
        {
            //------------------------------------Lead--------------------------------

            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0x40000) > 0)
            {
                if (!chk_InspectLead.Text.Contains("*"))
                    chk_InspectLead.Text += "*";
            }
            else
            {
                if (chk_InspectLead.Text.Contains("*"))
                    chk_InspectLead.Text = chk_InspectLead.Text.Substring(0, chk_InspectLead.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0xC0) > 0)
            {
                if (!chk_WidthHeight_Lead.Text.Contains("*"))
                    chk_WidthHeight_Lead.Text += "*";
            }
            else
            {
                if (chk_WidthHeight_Lead.Text.Contains("*"))
                    chk_WidthHeight_Lead.Text = chk_WidthHeight_Lead.Text.Substring(0, chk_WidthHeight_Lead.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0x100) > 0)
            {
                if (!chk_OffSet_Lead.Text.Contains("*"))
                    chk_OffSet_Lead.Text += "*";
            }
            else
            {
                if (chk_OffSet_Lead.Text.Contains("*"))
                    chk_OffSet_Lead.Text = chk_OffSet_Lead.Text.Substring(0, chk_OffSet_Lead.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0x20000) > 0)
            {
                if (!chk_Skew_Lead.Text.Contains("*"))
                    chk_Skew_Lead.Text += "*";
            }
            else
            {
                if (chk_Skew_Lead.Text.Contains("*"))
                    chk_Skew_Lead.Text = chk_Skew_Lead.Text.Substring(0, chk_Skew_Lead.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0x600) > 0)
            {
                if (!chk_PitchGap_Lead.Text.Contains("*"))
                    chk_PitchGap_Lead.Text += "*";
            }
            else
            {
                if (chk_PitchGap_Lead.Text.Contains("*"))
                    chk_PitchGap_Lead.Text = chk_PitchGap_Lead.Text.Substring(0, chk_PitchGap_Lead.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0x800) > 0)
            {
                if (!chk_Variance_Lead.Text.Contains("*"))
                    chk_Variance_Lead.Text += "*";
            }
            else
            {
                if (chk_Variance_Lead.Text.Contains("*"))
                    chk_Variance_Lead.Text = chk_Variance_Lead.Text.Substring(0, chk_Variance_Lead.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0x10000) > 0)
            {
                if (!chk_AverageGrayValue_Lead.Text.Contains("*"))
                    chk_AverageGrayValue_Lead.Text += "*";
            }
            else
            {
                if (chk_AverageGrayValue_Lead.Text.Contains("*"))
                    chk_AverageGrayValue_Lead.Text = chk_AverageGrayValue_Lead.Text.Substring(0, chk_AverageGrayValue_Lead.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0x1000) > 0)
            {
                if (!chk_Span_Lead.Text.Contains("*"))
                    chk_Span_Lead.Text += "*";
            }
            else
            {
                if (chk_Span_Lead.Text.Contains("*"))
                    chk_Span_Lead.Text = chk_Span_Lead.Text.Substring(0, chk_Span_Lead.Text.Length - 1);
            }

            // Foreign Material / Contamination
            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0x4000) > 0)
            {
                if (!chk_CheckForeignMaterialArea_Lead.Text.Contains("*"))
                    chk_CheckForeignMaterialArea_Lead.Text += "*";
            }
            else
            {
                if (chk_CheckForeignMaterialArea_Lead.Text.Contains("*"))
                    chk_CheckForeignMaterialArea_Lead.Text = chk_CheckForeignMaterialArea_Lead.Text.Substring(0, chk_CheckForeignMaterialArea_Lead.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0x8000) > 0)
            {
                if (!chk_CheckForeignMaterialLength_Lead.Text.Contains("*"))
                    chk_CheckForeignMaterialLength_Lead.Text += "*";
            }
            else
            {
                if (chk_CheckForeignMaterialLength_Lead.Text.Contains("*"))
                    chk_CheckForeignMaterialLength_Lead.Text = chk_CheckForeignMaterialLength_Lead.Text.Substring(0, chk_CheckForeignMaterialLength_Lead.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0x2000) > 0)
            {
                if (!chk_CheckForeignMaterialTotalArea_Lead.Text.Contains("*"))
                    chk_CheckForeignMaterialTotalArea_Lead.Text += "*";
            }
            else
            {
                if (chk_CheckForeignMaterialTotalArea_Lead.Text.Contains("*"))
                    chk_CheckForeignMaterialTotalArea_Lead.Text = chk_CheckForeignMaterialTotalArea_Lead.Text.Substring(0, chk_CheckForeignMaterialTotalArea_Lead.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0x80000) > 0)
            {
                if (!chk_BaseLeadOffset.Text.Contains("*"))
                    chk_BaseLeadOffset.Text += "*";
            }
            else
            {
                if (chk_BaseLeadOffset.Text.Contains("*"))
                    chk_BaseLeadOffset.Text = chk_BaseLeadOffset.Text.Substring(0, chk_BaseLeadOffset.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intLeadOptionControlMask & 0x100000) > 0)
            {
                if (!chk_BaseLeadArea.Text.Contains("*"))
                    chk_BaseLeadArea.Text += "*";
            }
            else
            {
                if (chk_BaseLeadArea.Text.Contains("*"))
                    chk_BaseLeadArea.Text = chk_BaseLeadArea.Text.Substring(0, chk_BaseLeadArea.Text.Length - 1);
            }
        }
        private void UpdateColorPackageControlSetting()
        {
            // Color Defect 1 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x01) > 0)
            {
                if (!chk_ColorPackage1Length.Text.Contains("*"))
                    chk_ColorPackage1Length.Text += "*";
            }
            else
            {
                if (chk_ColorPackage1Length.Text.Contains("*"))
                    chk_ColorPackage1Length.Text = chk_ColorPackage1Length.Text.Substring(0, chk_ColorPackage1Length.Text.Length - 1);
            }

            // Color Defect 1 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x02) > 0)
            {
                if (!chk_ColorPackage1Area.Text.Contains("*"))
                    chk_ColorPackage1Area.Text += "*";
            }
            else
            {
                if (chk_ColorPackage1Area.Text.Contains("*"))
                    chk_ColorPackage1Area.Text = chk_ColorPackage1Area.Text.Substring(0, chk_ColorPackage1Area.Text.Length - 1);
            }

            // Color Defect 2 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x04) > 0)
            {
                if (!chk_ColorPackage2Length.Text.Contains("*"))
                    chk_ColorPackage2Length.Text += "*";
            }
            else
            {
                if (chk_ColorPackage2Length.Text.Contains("*"))
                    chk_ColorPackage2Length.Text = chk_ColorPackage2Length.Text.Substring(0, chk_ColorPackage2Length.Text.Length - 1);
            }

            // Color Defect 2 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x08) > 0)
            {
                if (!chk_ColorPackage2Area.Text.Contains("*"))
                    chk_ColorPackage2Area.Text += "*";
            }
            else
            {
                if (chk_ColorPackage2Area.Text.Contains("*"))
                    chk_ColorPackage2Area.Text = chk_ColorPackage2Area.Text.Substring(0, chk_ColorPackage2Area.Text.Length - 1);
            }

            // Color Defect 3 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x10) > 0)
            {
                if (!chk_ColorPackage3Length.Text.Contains("*"))
                    chk_ColorPackage3Length.Text += "*";
            }
            else
            {
                if (chk_ColorPackage3Length.Text.Contains("*"))
                    chk_ColorPackage3Length.Text = chk_ColorPackage3Length.Text.Substring(0, chk_ColorPackage3Length.Text.Length - 1);
            }

            // Color Defect 3 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x20) > 0)
            {
                if (!chk_ColorPackage3Area.Text.Contains("*"))
                    chk_ColorPackage3Area.Text += "*";
            }
            else
            {
                if (chk_ColorPackage3Area.Text.Contains("*"))
                    chk_ColorPackage3Area.Text = chk_ColorPackage3Area.Text.Substring(0, chk_ColorPackage3Area.Text.Length - 1);
            }

            // Color Defect 4 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x40) > 0)
            {
                if (!chk_ColorPackage4Length.Text.Contains("*"))
                    chk_ColorPackage4Length.Text += "*";
            }
            else
            {
                if (chk_ColorPackage4Length.Text.Contains("*"))
                    chk_ColorPackage4Length.Text = chk_ColorPackage4Length.Text.Substring(0, chk_ColorPackage4Length.Text.Length - 1);
            }

            // Color Defect 4 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x80) > 0)
            {
                if (!chk_ColorPackage4Area.Text.Contains("*"))
                    chk_ColorPackage4Area.Text += "*";
            }
            else
            {
                if (chk_ColorPackage4Area.Text.Contains("*"))
                    chk_ColorPackage4Area.Text = chk_ColorPackage4Area.Text.Substring(0, chk_ColorPackage4Area.Text.Length - 1);
            }

            // Color Defect 5 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x100) > 0)
            {
                if (!chk_ColorPackage5Length.Text.Contains("*"))
                    chk_ColorPackage5Length.Text += "*";
            }
            else
            {
                if (chk_ColorPackage5Length.Text.Contains("*"))
                    chk_ColorPackage5Length.Text = chk_ColorPackage5Length.Text.Substring(0, chk_ColorPackage5Length.Text.Length - 1);
            }

            // Color Defect 5 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x200) > 0)
            {
                if (!chk_ColorPackage5Area.Text.Contains("*"))
                    chk_ColorPackage5Area.Text += "*";
            }
            else
            {
                if (chk_ColorPackage5Area.Text.Contains("*"))
                    chk_ColorPackage5Area.Text = chk_ColorPackage5Area.Text.Substring(0, chk_ColorPackage5Area.Text.Length - 1);
            }

        }
        private void UpdatePackageControlMaskGUI()
        {
            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x1000) > 0)
            {
                if (!chk_InspectPackage2.Text.Contains("*"))
                    chk_InspectPackage2.Text += "*";
            }
            else
            {
                if (chk_InspectPackage2.Text.Contains("*"))
                    chk_InspectPackage2.Text = chk_InspectPackage2.Text.Substring(0, chk_InspectPackage2.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x2000) > 0)
            {
                if (!chk_CheckPkgSize2.Text.Contains("*"))
                    chk_CheckPkgSize2.Text += "*";
            }
            else
            {
                if (chk_CheckPkgSize2.Text.Contains("*"))
                    chk_CheckPkgSize2.Text = chk_CheckPkgSize2.Text.Substring(0, chk_CheckPkgSize2.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x2000000) > 0)
            {
                if (!chk_CheckPkgAngle.Text.Contains("*"))
                    chk_CheckPkgAngle.Text += "*";
            }
            else
            {
                if (chk_CheckPkgAngle.Text.Contains("*"))
                    chk_CheckPkgAngle.Text = chk_CheckPkgAngle.Text.Substring(0, chk_CheckPkgAngle.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x4000) > 0)
            {
                if (!chk_BrightFieldArea.Text.Contains("*"))
                    chk_BrightFieldArea.Text += "*";
            }
            else
            {
                if (chk_BrightFieldArea.Text.Contains("*"))
                    chk_BrightFieldArea.Text = chk_BrightFieldArea.Text.Substring(0, chk_BrightFieldArea.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x8000) > 0)
            {
                if (!chk_BrightFieldLength.Text.Contains("*"))
                    chk_BrightFieldLength.Text += "*";
            }
            else
            {
                if (chk_BrightFieldLength.Text.Contains("*"))
                    chk_BrightFieldLength.Text = chk_BrightFieldLength.Text.Substring(0, chk_BrightFieldLength.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x10000) > 0)
            {
                if (!chk_DarkFieldArea.Text.Contains("*"))
                    chk_DarkFieldArea.Text += "*";
            }
            else
            {
                if (chk_DarkFieldArea.Text.Contains("*"))
                    chk_DarkFieldArea.Text = chk_DarkFieldArea.Text.Substring(0, chk_DarkFieldArea.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x20000) > 0)
            {
                if (!chk_DarkFieldLength.Text.Contains("*"))
                    chk_DarkFieldLength.Text += "*";
            }
            else
            {
                if (chk_DarkFieldLength.Text.Contains("*"))
                    chk_DarkFieldLength.Text = chk_DarkFieldLength.Text.Substring(0, chk_DarkFieldLength.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x10000000) > 0)
            {
                if (!chk_DarkField3Area.Text.Contains("*"))
                    chk_DarkField3Area.Text += "*";
            }
            else
            {
                if (chk_DarkField3Area.Text.Contains("*"))
                    chk_DarkField3Area.Text = chk_DarkField3Area.Text.Substring(0, chk_DarkField3Area.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x20000000) > 0)
            {
                if (!chk_DarkField3Length.Text.Contains("*"))
                    chk_DarkField3Length.Text += "*";
            }
            else
            {
                if (chk_DarkField3Length.Text.Contains("*"))
                    chk_DarkField3Length.Text = chk_DarkField3Length.Text.Substring(0, chk_DarkField3Length.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x100000) > 0) //use defect void masking
            {
                if (!chk_DarkField4Area.Text.Contains("*"))
                    chk_DarkField4Area.Text += "*";
            }
            else
            {
                if (chk_DarkField4Area.Text.Contains("*"))
                    chk_DarkField4Area.Text = chk_DarkField4Area.Text.Substring(0, chk_DarkField4Area.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x200000) > 0) //use defect void masking
            {
                if (!chk_DarkField4Length.Text.Contains("*"))
                    chk_DarkField4Length.Text += "*";
            }
            else
            {
                if (chk_DarkField4Length.Text.Contains("*"))
                    chk_DarkField4Length.Text = chk_DarkField4Length.Text.Substring(0, chk_DarkField4Length.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x4000000) > 0)
            {
                if (!chk_DarkField2Area.Text.Contains("*"))
                    chk_DarkField2Area.Text += "*";
            }
            else
            {
                if (chk_DarkField2Area.Text.Contains("*"))
                    chk_DarkField2Area.Text = chk_DarkField2Area.Text.Substring(0, chk_DarkField2Area.Text.Length - 1);
            }


            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x8000000) > 0)
            {
                if (!chk_DarkField2Length.Text.Contains("*"))
                    chk_DarkField2Length.Text += "*";
            }
            else
            {
                if (chk_DarkField2Length.Text.Contains("*"))
                    chk_DarkField2Length.Text = chk_DarkField2Length.Text.Substring(0, chk_DarkField2Length.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x80000) > 0)
            {
                if (!chk_CrackDarkFieldLength.Text.Contains("*"))
                    chk_CrackDarkFieldLength.Text += "*";
            }
            else
            {
                if (chk_CrackDarkFieldLength.Text.Contains("*"))
                    chk_CrackDarkFieldLength.Text = chk_CrackDarkFieldLength.Text.Substring(0, chk_CrackDarkFieldLength.Text.Length - 1);
            }

            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateCrackDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x40000) > 0)
                {
                    if (!chk_CrackDarkFieldArea.Text.Contains("*"))
                        chk_CrackDarkFieldArea.Text += "*";
                }
                else
                {
                    if (chk_CrackDarkFieldArea.Text.Contains("*"))
                        chk_CrackDarkFieldArea.Text = chk_CrackDarkFieldArea.Text.Substring(0, chk_CrackDarkFieldArea.Text.Length - 1);
                }
            }

            //if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateVoidDefectSetting)
            //{
            //    if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x100000) > 0)
            //    {
            //        if (!chk_VoidDarkFieldArea.Text.Contains("*"))
            //            chk_VoidDarkFieldArea.Text += "*";
            //    }
            //    else
            //    {
            //        if (chk_VoidDarkFieldArea.Text.Contains("*"))
            //            chk_VoidDarkFieldArea.Text = chk_VoidDarkFieldArea.Text.Substring(0, chk_VoidDarkFieldArea.Text.Length - 1);
            //    }

            //    if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x200000) > 0)
            //    {
            //        if (!chk_VoidDarkFieldLength.Text.Contains("*"))
            //            chk_VoidDarkFieldLength.Text += "*";
            //    }
            //    else
            //    {
            //        if (chk_VoidDarkFieldLength.Text.Contains("*"))
            //            chk_VoidDarkFieldLength.Text = chk_VoidDarkFieldLength.Text.Substring(0, chk_VoidDarkFieldLength.Text.Length - 1);
            //    }
            //}

            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateMoldFlashDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x400000) > 0)
                {
                    if (!chk_MoldFlashBrightFieldArea.Text.Contains("*"))
                        chk_MoldFlashBrightFieldArea.Text += "*";
                }
                else
                {
                    if (chk_MoldFlashBrightFieldArea.Text.Contains("*"))
                        chk_MoldFlashBrightFieldArea.Text = chk_MoldFlashBrightFieldArea.Text.Substring(0, chk_MoldFlashBrightFieldArea.Text.Length - 1);
                }
            }

            if (m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_blnSeperateChippedOffDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x800000) > 0)
                {
                    if (!chk_CheckChippedOffBright_Area.Text.Contains("*"))
                        chk_CheckChippedOffBright_Area.Text += "*";
                }
                else
                {
                    if (chk_CheckChippedOffBright_Area.Text.Contains("*"))
                        chk_CheckChippedOffBright_Area.Text = chk_CheckChippedOffBright_Area.Text.Substring(0, chk_CheckChippedOffBright_Area.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x40000000) > 0)
                {
                    if (!chk_CheckChippedOffBright_Length.Text.Contains("*"))
                        chk_CheckChippedOffBright_Length.Text += "*";
                }
                else
                {
                    if (chk_CheckChippedOffBright_Length.Text.Contains("*"))
                        chk_CheckChippedOffBright_Length.Text = chk_CheckChippedOffBright_Length.Text.Substring(0, chk_CheckChippedOffBright_Length.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x1000000) > 0)
                {
                    if (!chk_CheckChippedOffDark_Area.Text.Contains("*"))
                        chk_CheckChippedOffDark_Area.Text += "*";
                }
                else
                {
                    if (chk_CheckChippedOffDark_Area.Text.Contains("*"))
                        chk_CheckChippedOffDark_Area.Text = chk_CheckChippedOffDark_Area.Text.Substring(0, chk_CheckChippedOffDark_Area.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x80000000) > 0)
                {
                    if (!chk_CheckChippedOffDark_Length.Text.Contains("*"))
                        chk_CheckChippedOffDark_Length.Text += "*";
                }
                else
                {
                    if (chk_CheckChippedOffDark_Length.Text.Contains("*"))
                        chk_CheckChippedOffDark_Length.Text = chk_CheckChippedOffDark_Length.Text.Substring(0, chk_CheckChippedOffDark_Length.Text.Length - 1);
                }

            }
        }
        private void UpdateCenterColorPadControlSetting()
        {
            //Center Color Defect 1 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x01) > 0)
            {
                if (!chk_ColorPad1Length_Center.Text.Contains("*"))
                    chk_ColorPad1Length_Center.Text += "*";
            }
            else
            {
                if (chk_ColorPad1Length_Center.Text.Contains("*"))
                    chk_ColorPad1Length_Center.Text = chk_ColorPad1Length_Center.Text.Substring(0, chk_ColorPad1Length_Center.Text.Length - 1);
            }

            //Center Color Defect 1 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x02) > 0)
            {
                if (!chk_ColorPad1Area_Center.Text.Contains("*"))
                    chk_ColorPad1Area_Center.Text += "*";
            }
            else
            {
                if (chk_ColorPad1Area_Center.Text.Contains("*"))
                    chk_ColorPad1Area_Center.Text = chk_ColorPad1Area_Center.Text.Substring(0, chk_ColorPad1Area_Center.Text.Length - 1);
            }

            //Center Color Defect 2 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x04) > 0)
            {
                if (!chk_ColorPad2Length_Center.Text.Contains("*"))
                    chk_ColorPad2Length_Center.Text += "*";
            }
            else
            {
                if (chk_ColorPad2Length_Center.Text.Contains("*"))
                    chk_ColorPad2Length_Center.Text = chk_ColorPad2Length_Center.Text.Substring(0, chk_ColorPad2Length_Center.Text.Length - 1);
            }

            //Center Color Defect 2 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x08) > 0)
            {
                if (!chk_ColorPad2Area_Center.Text.Contains("*"))
                    chk_ColorPad2Area_Center.Text += "*";
            }
            else
            {
                if (chk_ColorPad2Area_Center.Text.Contains("*"))
                    chk_ColorPad2Area_Center.Text = chk_ColorPad2Area_Center.Text.Substring(0, chk_ColorPad2Area_Center.Text.Length - 1);
            }

            //Center Color Defect 3 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x10) > 0)
            {
                if (!chk_ColorPad3Length_Center.Text.Contains("*"))
                    chk_ColorPad3Length_Center.Text += "*";
            }
            else
            {
                if (chk_ColorPad3Length_Center.Text.Contains("*"))
                    chk_ColorPad3Length_Center.Text = chk_ColorPad3Length_Center.Text.Substring(0, chk_ColorPad3Length_Center.Text.Length - 1);
            }

            //Center Color Defect 3 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x20) > 0)
            {
                if (!chk_ColorPad3Area_Center.Text.Contains("*"))
                    chk_ColorPad3Area_Center.Text += "*";
            }
            else
            {
                if (chk_ColorPad3Area_Center.Text.Contains("*"))
                    chk_ColorPad3Area_Center.Text = chk_ColorPad3Area_Center.Text.Substring(0, chk_ColorPad3Area_Center.Text.Length - 1);
            }

            //Center Color Defect 4 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x40) > 0)
            {
                if (!chk_ColorPad4Length_Center.Text.Contains("*"))
                    chk_ColorPad4Length_Center.Text += "*";
            }
            else
            {
                if (chk_ColorPad4Length_Center.Text.Contains("*"))
                    chk_ColorPad4Length_Center.Text = chk_ColorPad4Length_Center.Text.Substring(0, chk_ColorPad4Length_Center.Text.Length - 1);
            }

            //Center Color Defect 4 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x80) > 0)
            {
                if (!chk_ColorPad4Area_Center.Text.Contains("*"))
                    chk_ColorPad4Area_Center.Text += "*";
            }
            else
            {
                if (chk_ColorPad4Area_Center.Text.Contains("*"))
                    chk_ColorPad4Area_Center.Text = chk_ColorPad4Area_Center.Text.Substring(0, chk_ColorPad4Area_Center.Text.Length - 1);
            }

            //Center Color Defect 5 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x100) > 0)
            {
                if (!chk_ColorPad5Length_Center.Text.Contains("*"))
                    chk_ColorPad5Length_Center.Text += "*";
            }
            else
            {
                if (chk_ColorPad5Length_Center.Text.Contains("*"))
                    chk_ColorPad5Length_Center.Text = chk_ColorPad5Length_Center.Text.Substring(0, chk_ColorPad5Length_Center.Text.Length - 1);
            }

            //Center Color Defect 5 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x200) > 0)
            {
                if (!chk_ColorPad5Area_Center.Text.Contains("*"))
                    chk_ColorPad5Area_Center.Text += "*";
            }
            else
            {
                if (chk_ColorPad5Area_Center.Text.Contains("*"))
                    chk_ColorPad5Area_Center.Text = chk_ColorPad5Area_Center.Text.Substring(0, chk_ColorPad5Area_Center.Text.Length - 1);
            }

        }
        private void UpdateSideColorPadControlSetting_Top()
        {
            //Side Color Defect 1 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x1000) > 0)
            {
                if (!chk_ColorPad1Length_Side_Top.Text.Contains("*"))
                    chk_ColorPad1Length_Side_Top.Text += "*";
            }
            else
            {
                if (chk_ColorPad1Length_Side_Top.Text.Contains("*"))
                    chk_ColorPad1Length_Side_Top.Text = chk_ColorPad1Length_Side_Top.Text.Substring(0, chk_ColorPad1Length_Side_Top.Text.Length - 1);
            }

            //Side Color Defect 1 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x2000) > 0)
            {
                if (!chk_ColorPad1Area_Side_Top.Text.Contains("*"))
                    chk_ColorPad1Area_Side_Top.Text += "*";
            }
            else
            {
                if (chk_ColorPad1Area_Side_Top.Text.Contains("*"))
                    chk_ColorPad1Area_Side_Top.Text = chk_ColorPad1Area_Side_Top.Text.Substring(0, chk_ColorPad1Area_Side_Top.Text.Length - 1);
            }

            //Side Color Defect 2 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x4000) > 0)
            {
                if (!chk_ColorPad2Length_Side_Top.Text.Contains("*"))
                    chk_ColorPad2Length_Side_Top.Text += "*";
            }
            else
            {
                if (chk_ColorPad2Length_Side_Top.Text.Contains("*"))
                    chk_ColorPad2Length_Side_Top.Text = chk_ColorPad2Length_Side_Top.Text.Substring(0, chk_ColorPad2Length_Side_Top.Text.Length - 1);
            }

            //Side Color Defect 2 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x8000) > 0)
            {
                if (!chk_ColorPad2Area_Side_Top.Text.Contains("*"))
                    chk_ColorPad2Area_Side_Top.Text += "*";
            }
            else
            {
                if (chk_ColorPad2Area_Side_Top.Text.Contains("*"))
                    chk_ColorPad2Area_Side_Top.Text = chk_ColorPad2Area_Side_Top.Text.Substring(0, chk_ColorPad2Area_Side_Top.Text.Length - 1);
            }

            //Side Color Defect 3 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x10000) > 0)
            {
                if (!chk_ColorPad3Length_Side_Top.Text.Contains("*"))
                    chk_ColorPad3Length_Side_Top.Text += "*";
            }
            else
            {
                if (chk_ColorPad3Length_Side_Top.Text.Contains("*"))
                    chk_ColorPad3Length_Side_Top.Text = chk_ColorPad3Length_Side_Top.Text.Substring(0, chk_ColorPad3Length_Side_Top.Text.Length - 1);
            }

            //Side Color Defect 3 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x20000) > 0)
            {
                if (!chk_ColorPad3Area_Side_Top.Text.Contains("*"))
                    chk_ColorPad3Area_Side_Top.Text += "*";
            }
            else
            {
                if (chk_ColorPad3Area_Side_Top.Text.Contains("*"))
                    chk_ColorPad3Area_Side_Top.Text = chk_ColorPad3Area_Side_Top.Text.Substring(0, chk_ColorPad3Area_Side_Top.Text.Length - 1);
            }

            //Side Color Defect 4 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x40000) > 0)
            {
                if (!chk_ColorPad4Length_Side_Top.Text.Contains("*"))
                    chk_ColorPad4Length_Side_Top.Text += "*";
            }
            else
            {
                if (chk_ColorPad4Length_Side_Top.Text.Contains("*"))
                    chk_ColorPad4Length_Side_Top.Text = chk_ColorPad4Length_Side_Top.Text.Substring(0, chk_ColorPad4Length_Side_Top.Text.Length - 1);
            }

            //Side Color Defect 4 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x80000) > 0)
            {
                if (!chk_ColorPad4Area_Side_Top.Text.Contains("*"))
                    chk_ColorPad4Area_Side_Top.Text += "*";
            }
            else
            {
                if (chk_ColorPad4Area_Side_Top.Text.Contains("*"))
                    chk_ColorPad4Area_Side_Top.Text = chk_ColorPad4Area_Side_Top.Text.Substring(0, chk_ColorPad4Area_Side_Top.Text.Length - 1);
            }

            //Side Color Defect 5 Length
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x100000) > 0)
            {
                if (!chk_ColorPad5Length_Side_Top.Text.Contains("*"))
                    chk_ColorPad5Length_Side_Top.Text += "*";
            }
            else
            {
                if (chk_ColorPad5Length_Side_Top.Text.Contains("*"))
                    chk_ColorPad5Length_Side_Top.Text = chk_ColorPad5Length_Side_Top.Text.Substring(0, chk_ColorPad5Length_Side_Top.Text.Length - 1);
            }

            //Side Color Defect 5 Area
            if ((m_smVisionInfo.g_intOptionControlMask2 & 0x200000) > 0)
            {
                if (!chk_ColorPad5Area_Side_Top.Text.Contains("*"))
                    chk_ColorPad5Area_Side_Top.Text += "*";
            }
            else
            {
                if (chk_ColorPad5Area_Side_Top.Text.Contains("*"))
                    chk_ColorPad5Area_Side_Top.Text = chk_ColorPad5Area_Side_Top.Text.Substring(0, chk_ColorPad5Area_Side_Top.Text.Length - 1);
            }

        }
        private void UpdateSideColorPadControlSetting_Right()
        {
            //Side Color Defect 1 Length
            if ((m_smVisionInfo.g_intOptionControlMask3 & 0x1000) > 0)
            {
                if (!chk_ColorPad1Length_Side_Right.Text.Contains("*"))
                    chk_ColorPad1Length_Side_Right.Text += "*";
            }
            else
            {
                if (chk_ColorPad1Length_Side_Right.Text.Contains("*"))
                    chk_ColorPad1Length_Side_Right.Text = chk_ColorPad1Length_Side_Right.Text.Substring(0, chk_ColorPad1Length_Side_Right.Text.Length - 1);
            }

            //Side Color Defect 1 Area
            if ((m_smVisionInfo.g_intOptionControlMask3 & 0x2000) > 0)
            {
                if (!chk_ColorPad1Area_Side_Right.Text.Contains("*"))
                    chk_ColorPad1Area_Side_Right.Text += "*";
            }
            else
            {
                if (chk_ColorPad1Area_Side_Right.Text.Contains("*"))
                    chk_ColorPad1Area_Side_Right.Text = chk_ColorPad1Area_Side_Right.Text.Substring(0, chk_ColorPad1Area_Side_Right.Text.Length - 1);
            }

            //Side Color Defect 2 Length
            if ((m_smVisionInfo.g_intOptionControlMask3 & 0x4000) > 0)
            {
                if (!chk_ColorPad2Length_Side_Right.Text.Contains("*"))
                    chk_ColorPad2Length_Side_Right.Text += "*";
            }
            else
            {
                if (chk_ColorPad2Length_Side_Right.Text.Contains("*"))
                    chk_ColorPad2Length_Side_Right.Text = chk_ColorPad2Length_Side_Right.Text.Substring(0, chk_ColorPad2Length_Side_Right.Text.Length - 1);
            }

            //Side Color Defect 2 Area
            if ((m_smVisionInfo.g_intOptionControlMask3 & 0x8000) > 0)
            {
                if (!chk_ColorPad2Area_Side_Right.Text.Contains("*"))
                    chk_ColorPad2Area_Side_Right.Text += "*";
            }
            else
            {
                if (chk_ColorPad2Area_Side_Right.Text.Contains("*"))
                    chk_ColorPad2Area_Side_Right.Text = chk_ColorPad2Area_Side_Right.Text.Substring(0, chk_ColorPad2Area_Side_Right.Text.Length - 1);
            }

            //Side Color Defect 3 Length
            if ((m_smVisionInfo.g_intOptionControlMask3 & 0x10000) > 0)
            {
                if (!chk_ColorPad3Length_Side_Right.Text.Contains("*"))
                    chk_ColorPad3Length_Side_Right.Text += "*";
            }
            else
            {
                if (chk_ColorPad3Length_Side_Right.Text.Contains("*"))
                    chk_ColorPad3Length_Side_Right.Text = chk_ColorPad3Length_Side_Right.Text.Substring(0, chk_ColorPad3Length_Side_Right.Text.Length - 1);
            }

            //Side Color Defect 3 Area
            if ((m_smVisionInfo.g_intOptionControlMask3 & 0x20000) > 0)
            {
                if (!chk_ColorPad3Area_Side_Right.Text.Contains("*"))
                    chk_ColorPad3Area_Side_Right.Text += "*";
            }
            else
            {
                if (chk_ColorPad3Area_Side_Right.Text.Contains("*"))
                    chk_ColorPad3Area_Side_Right.Text = chk_ColorPad3Area_Side_Right.Text.Substring(0, chk_ColorPad3Area_Side_Right.Text.Length - 1);
            }

            //Side Color Defect 4 Length
            if ((m_smVisionInfo.g_intOptionControlMask3 & 0x40000) > 0)
            {
                if (!chk_ColorPad4Length_Side_Right.Text.Contains("*"))
                    chk_ColorPad4Length_Side_Right.Text += "*";
            }
            else
            {
                if (chk_ColorPad4Length_Side_Right.Text.Contains("*"))
                    chk_ColorPad4Length_Side_Right.Text = chk_ColorPad4Length_Side_Right.Text.Substring(0, chk_ColorPad4Length_Side_Right.Text.Length - 1);
            }

            //Side Color Defect 4 Area
            if ((m_smVisionInfo.g_intOptionControlMask3 & 0x80000) > 0)
            {
                if (!chk_ColorPad4Area_Side_Right.Text.Contains("*"))
                    chk_ColorPad4Area_Side_Right.Text += "*";
            }
            else
            {
                if (chk_ColorPad4Area_Side_Right.Text.Contains("*"))
                    chk_ColorPad4Area_Side_Right.Text = chk_ColorPad4Area_Side_Right.Text.Substring(0, chk_ColorPad4Area_Side_Right.Text.Length - 1);
            }

            //Side Color Defect 5 Length
            if ((m_smVisionInfo.g_intOptionControlMask3 & 0x100000) > 0)
            {
                if (!chk_ColorPad5Length_Side_Right.Text.Contains("*"))
                    chk_ColorPad5Length_Side_Right.Text += "*";
            }
            else
            {
                if (chk_ColorPad5Length_Side_Right.Text.Contains("*"))
                    chk_ColorPad5Length_Side_Right.Text = chk_ColorPad5Length_Side_Right.Text.Substring(0, chk_ColorPad5Length_Side_Right.Text.Length - 1);
            }

            //Side Color Defect 5 Area
            if ((m_smVisionInfo.g_intOptionControlMask3 & 0x200000) > 0)
            {
                if (!chk_ColorPad5Area_Side_Right.Text.Contains("*"))
                    chk_ColorPad5Area_Side_Right.Text += "*";
            }
            else
            {
                if (chk_ColorPad5Area_Side_Right.Text.Contains("*"))
                    chk_ColorPad5Area_Side_Right.Text = chk_ColorPad5Area_Side_Right.Text.Substring(0, chk_ColorPad5Area_Side_Right.Text.Length - 1);
            }

        }
        private void UpdateSideColorPadControlSetting_Bottom()
        {
            //Side Color Defect 1 Length
            if ((m_smVisionInfo.g_intOptionControlMask4 & 0x1000) > 0)
            {
                if (!chk_ColorPad1Length_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad1Length_Side_Bottom.Text += "*";
            }
            else
            {
                if (chk_ColorPad1Length_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad1Length_Side_Bottom.Text = chk_ColorPad1Length_Side_Bottom.Text.Substring(0, chk_ColorPad1Length_Side_Bottom.Text.Length - 1);
            }

            //Side Color Defect 1 Area
            if ((m_smVisionInfo.g_intOptionControlMask4 & 0x2000) > 0)
            {
                if (!chk_ColorPad1Area_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad1Area_Side_Bottom.Text += "*";
            }
            else
            {
                if (chk_ColorPad1Area_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad1Area_Side_Bottom.Text = chk_ColorPad1Area_Side_Bottom.Text.Substring(0, chk_ColorPad1Area_Side_Bottom.Text.Length - 1);
            }

            //Side Color Defect 2 Length
            if ((m_smVisionInfo.g_intOptionControlMask4 & 0x4000) > 0)
            {
                if (!chk_ColorPad2Length_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad2Length_Side_Bottom.Text += "*";
            }
            else
            {
                if (chk_ColorPad2Length_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad2Length_Side_Bottom.Text = chk_ColorPad2Length_Side_Bottom.Text.Substring(0, chk_ColorPad2Length_Side_Bottom.Text.Length - 1);
            }

            //Side Color Defect 2 Area
            if ((m_smVisionInfo.g_intOptionControlMask4 & 0x8000) > 0)
            {
                if (!chk_ColorPad2Area_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad2Area_Side_Bottom.Text += "*";
            }
            else
            {
                if (chk_ColorPad2Area_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad2Area_Side_Bottom.Text = chk_ColorPad2Area_Side_Bottom.Text.Substring(0, chk_ColorPad2Area_Side_Bottom.Text.Length - 1);
            }

            //Side Color Defect 3 Length
            if ((m_smVisionInfo.g_intOptionControlMask4 & 0x10000) > 0)
            {
                if (!chk_ColorPad3Length_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad3Length_Side_Bottom.Text += "*";
            }
            else
            {
                if (chk_ColorPad3Length_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad3Length_Side_Bottom.Text = chk_ColorPad3Length_Side_Bottom.Text.Substring(0, chk_ColorPad3Length_Side_Bottom.Text.Length - 1);
            }

            //Side Color Defect 3 Area
            if ((m_smVisionInfo.g_intOptionControlMask4 & 0x20000) > 0)
            {
                if (!chk_ColorPad3Area_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad3Area_Side_Bottom.Text += "*";
            }
            else
            {
                if (chk_ColorPad3Area_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad3Area_Side_Bottom.Text = chk_ColorPad3Area_Side_Bottom.Text.Substring(0, chk_ColorPad3Area_Side_Bottom.Text.Length - 1);
            }

            //Side Color Defect 4 Length
            if ((m_smVisionInfo.g_intOptionControlMask4 & 0x40000) > 0)
            {
                if (!chk_ColorPad4Length_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad4Length_Side_Bottom.Text += "*";
            }
            else
            {
                if (chk_ColorPad4Length_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad4Length_Side_Bottom.Text = chk_ColorPad4Length_Side_Bottom.Text.Substring(0, chk_ColorPad4Length_Side_Bottom.Text.Length - 1);
            }

            //Side Color Defect 4 Area
            if ((m_smVisionInfo.g_intOptionControlMask4 & 0x80000) > 0)
            {
                if (!chk_ColorPad4Area_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad4Area_Side_Bottom.Text += "*";
            }
            else
            {
                if (chk_ColorPad4Area_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad4Area_Side_Bottom.Text = chk_ColorPad4Area_Side_Bottom.Text.Substring(0, chk_ColorPad4Area_Side_Bottom.Text.Length - 1);
            }

            //Side Color Defect 5 Length
            if ((m_smVisionInfo.g_intOptionControlMask4 & 0x100000) > 0)
            {
                if (!chk_ColorPad5Length_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad5Length_Side_Bottom.Text += "*";
            }
            else
            {
                if (chk_ColorPad5Length_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad5Length_Side_Bottom.Text = chk_ColorPad5Length_Side_Bottom.Text.Substring(0, chk_ColorPad5Length_Side_Bottom.Text.Length - 1);
            }

            //Side Color Defect 5 Area
            if ((m_smVisionInfo.g_intOptionControlMask4 & 0x200000) > 0)
            {
                if (!chk_ColorPad5Area_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad5Area_Side_Bottom.Text += "*";
            }
            else
            {
                if (chk_ColorPad5Area_Side_Bottom.Text.Contains("*"))
                    chk_ColorPad5Area_Side_Bottom.Text = chk_ColorPad5Area_Side_Bottom.Text.Substring(0, chk_ColorPad5Area_Side_Bottom.Text.Length - 1);
            }

        }
        private void UpdateSideColorPadControlSetting_Left()
        {
            //Side Color Defect 1 Length
            if ((m_smVisionInfo.g_intOptionControlMask5 & 0x1000) > 0)
            {
                if (!chk_ColorPad1Length_Side_Left.Text.Contains("*"))
                    chk_ColorPad1Length_Side_Left.Text += "*";
            }
            else
            {
                if (chk_ColorPad1Length_Side_Left.Text.Contains("*"))
                    chk_ColorPad1Length_Side_Left.Text = chk_ColorPad1Length_Side_Left.Text.Substring(0, chk_ColorPad1Length_Side_Left.Text.Length - 1);
            }

            //Side Color Defect 1 Area
            if ((m_smVisionInfo.g_intOptionControlMask5 & 0x2000) > 0)
            {
                if (!chk_ColorPad1Area_Side_Left.Text.Contains("*"))
                    chk_ColorPad1Area_Side_Left.Text += "*";
            }
            else
            {
                if (chk_ColorPad1Area_Side_Left.Text.Contains("*"))
                    chk_ColorPad1Area_Side_Left.Text = chk_ColorPad1Area_Side_Left.Text.Substring(0, chk_ColorPad1Area_Side_Left.Text.Length - 1);
            }

            //Side Color Defect 2 Length
            if ((m_smVisionInfo.g_intOptionControlMask5 & 0x4000) > 0)
            {
                if (!chk_ColorPad2Length_Side_Left.Text.Contains("*"))
                    chk_ColorPad2Length_Side_Left.Text += "*";
            }
            else
            {
                if (chk_ColorPad2Length_Side_Left.Text.Contains("*"))
                    chk_ColorPad2Length_Side_Left.Text = chk_ColorPad2Length_Side_Left.Text.Substring(0, chk_ColorPad2Length_Side_Left.Text.Length - 1);
            }

            //Side Color Defect 2 Area
            if ((m_smVisionInfo.g_intOptionControlMask5 & 0x8000) > 0)
            {
                if (!chk_ColorPad2Area_Side_Left.Text.Contains("*"))
                    chk_ColorPad2Area_Side_Left.Text += "*";
            }
            else
            {
                if (chk_ColorPad2Area_Side_Left.Text.Contains("*"))
                    chk_ColorPad2Area_Side_Left.Text = chk_ColorPad2Area_Side_Left.Text.Substring(0, chk_ColorPad2Area_Side_Left.Text.Length - 1);
            }

            //Side Color Defect 3 Length
            if ((m_smVisionInfo.g_intOptionControlMask5 & 0x10000) > 0)
            {
                if (!chk_ColorPad3Length_Side_Left.Text.Contains("*"))
                    chk_ColorPad3Length_Side_Left.Text += "*";
            }
            else
            {
                if (chk_ColorPad3Length_Side_Left.Text.Contains("*"))
                    chk_ColorPad3Length_Side_Left.Text = chk_ColorPad3Length_Side_Left.Text.Substring(0, chk_ColorPad3Length_Side_Left.Text.Length - 1);
            }

            //Side Color Defect 3 Area
            if ((m_smVisionInfo.g_intOptionControlMask5 & 0x20000) > 0)
            {
                if (!chk_ColorPad3Area_Side_Left.Text.Contains("*"))
                    chk_ColorPad3Area_Side_Left.Text += "*";
            }
            else
            {
                if (chk_ColorPad3Area_Side_Left.Text.Contains("*"))
                    chk_ColorPad3Area_Side_Left.Text = chk_ColorPad3Area_Side_Left.Text.Substring(0, chk_ColorPad3Area_Side_Left.Text.Length - 1);
            }

            //Side Color Defect 4 Length
            if ((m_smVisionInfo.g_intOptionControlMask5 & 0x40000) > 0)
            {
                if (!chk_ColorPad4Length_Side_Left.Text.Contains("*"))
                    chk_ColorPad4Length_Side_Left.Text += "*";
            }
            else
            {
                if (chk_ColorPad4Length_Side_Left.Text.Contains("*"))
                    chk_ColorPad4Length_Side_Left.Text = chk_ColorPad4Length_Side_Left.Text.Substring(0, chk_ColorPad4Length_Side_Left.Text.Length - 1);
            }

            //Side Color Defect 4 Area
            if ((m_smVisionInfo.g_intOptionControlMask5 & 0x80000) > 0)
            {
                if (!chk_ColorPad4Area_Side_Left.Text.Contains("*"))
                    chk_ColorPad4Area_Side_Left.Text += "*";
            }
            else
            {
                if (chk_ColorPad4Area_Side_Left.Text.Contains("*"))
                    chk_ColorPad4Area_Side_Left.Text = chk_ColorPad4Area_Side_Left.Text.Substring(0, chk_ColorPad4Area_Side_Left.Text.Length - 1);
            }

            //Side Color Defect 5 Length
            if ((m_smVisionInfo.g_intOptionControlMask5 & 0x100000) > 0)
            {
                if (!chk_ColorPad5Length_Side_Left.Text.Contains("*"))
                    chk_ColorPad5Length_Side_Left.Text += "*";
            }
            else
            {
                if (chk_ColorPad5Length_Side_Left.Text.Contains("*"))
                    chk_ColorPad5Length_Side_Left.Text = chk_ColorPad5Length_Side_Left.Text.Substring(0, chk_ColorPad5Length_Side_Left.Text.Length - 1);
            }

            //Side Color Defect 5 Area
            if ((m_smVisionInfo.g_intOptionControlMask5 & 0x200000) > 0)
            {
                if (!chk_ColorPad5Area_Side_Left.Text.Contains("*"))
                    chk_ColorPad5Area_Side_Left.Text += "*";
            }
            else
            {
                if (chk_ColorPad5Area_Side_Left.Text.Contains("*"))
                    chk_ColorPad5Area_Side_Left.Text = chk_ColorPad5Area_Side_Left.Text.Substring(0, chk_ColorPad5Area_Side_Left.Text.Length - 1);
            }

        }
        private void UpdatePadControlSetting()
        {
            //Inspect Pad
            if ((m_smVisionInfo.g_intOptionControlMask & 0x4000000) > 0)
            {
                if (!chk_InspectPad.Text.Contains("*"))
                    chk_InspectPad.Text += "*";
            }
            else
            {
                if (chk_InspectPad.Text.Contains("*"))
                    chk_InspectPad.Text = chk_InspectPad.Text.Substring(0, chk_InspectPad.Text.Length - 1);
            }

            // Area
            if ((m_smVisionInfo.g_intOptionControlMask & 0x01) > 0)
            {
                if (!chk_Area_Pad.Text.Contains("*"))
                    chk_Area_Pad.Text += "*";
            }
            else
            {
                if (chk_Area_Pad.Text.Contains("*"))
                    chk_Area_Pad.Text = chk_Area_Pad.Text.Substring(0, chk_Area_Pad.Text.Length - 1);
            }

            // Width and Height
            if ((m_smVisionInfo.g_intOptionControlMask & 0x02) > 0)
            {
                if (!chk_WidthHeight_Pad.Text.Contains("*"))
                    chk_WidthHeight_Pad.Text += "*";
            }
            else
            {
                if (chk_WidthHeight_Pad.Text.Contains("*"))
                    chk_WidthHeight_Pad.Text = chk_WidthHeight_Pad.Text.Substring(0, chk_WidthHeight_Pad.Text.Length - 1);
            }

            // Off Set
            if ((m_smVisionInfo.g_intOptionControlMask & 0x04) > 0)
            {
                if (!chk_OffSet_Pad.Text.Contains("*"))
                    chk_OffSet_Pad.Text += "*";
            }
            else
            {
                if (chk_OffSet_Pad.Text.Contains("*"))
                    chk_OffSet_Pad.Text = chk_OffSet_Pad.Text.Substring(0, chk_OffSet_Pad.Text.Length - 1);
            }

            // Broken Pad 
            if ((m_smVisionInfo.g_intOptionControlMask & 0x20) > 0)
            {
                if (!chk_CheckBrokenArea_Pad.Text.Contains("*"))
                    chk_CheckBrokenArea_Pad.Text += "*";
            }
            else
            {
                if (chk_CheckBrokenArea_Pad.Text.Contains("*"))
                    chk_CheckBrokenArea_Pad.Text = chk_CheckBrokenArea_Pad.Text.Substring(0, chk_CheckBrokenArea_Pad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x40) > 0)
            {
                if (!chk_CheckBrokenLength_Pad.Text.Contains("*"))
                    chk_CheckBrokenLength_Pad.Text += "*";
            }
            else
            {
                if (chk_CheckBrokenLength_Pad.Text.Contains("*"))
                    chk_CheckBrokenLength_Pad.Text = chk_CheckBrokenLength_Pad.Text.Substring(0, chk_CheckBrokenLength_Pad.Text.Length - 1);
            }

            // Pitch Gap
            if ((m_smVisionInfo.g_intOptionControlMask & 0x80) > 0)
            {
                if (!chk_Gap_Pad.Text.Contains("*"))
                    chk_Gap_Pad.Text += "*";
            }
            else
            {
                if (chk_Gap_Pad.Text.Contains("*"))
                    chk_Gap_Pad.Text = chk_Gap_Pad.Text.Substring(0, chk_Gap_Pad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x100) > 0)
            {
                if (!chk_CheckExcess_Pad.Text.Contains("*"))
                    chk_CheckExcess_Pad.Text += "*";
            }
            else
            {
                if (chk_CheckExcess_Pad.Text.Contains("*"))
                    chk_CheckExcess_Pad.Text = chk_CheckExcess_Pad.Text.Substring(0, chk_CheckExcess_Pad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x200) > 0)
            {
                if (!chk_CheckSmear_Pad.Text.Contains("*"))
                    chk_CheckSmear_Pad.Text += "*";
            }
            else
            {
                if (chk_CheckSmear_Pad.Text.Contains("*"))
                    chk_CheckSmear_Pad.Text = chk_CheckSmear_Pad.Text.Substring(0, chk_CheckSmear_Pad.Text.Length - 1);
            }
            
            if ((m_smVisionInfo.g_intOptionControlMask & 0x800000) > 0)
            {
                if (!chk_CheckEdgeLimit_Pad.Text.Contains("*"))
                    chk_CheckEdgeLimit_Pad.Text += "*";
            }
            else
            {
                if (chk_CheckEdgeLimit_Pad.Text.Contains("*"))
                    chk_CheckEdgeLimit_Pad.Text = chk_CheckEdgeLimit_Pad.Text.Substring(0, chk_CheckEdgeLimit_Pad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x2000000) > 0)
            {
                if (!chk_CheckStandOff_Pad.Text.Contains("*"))
                    chk_CheckStandOff_Pad.Text += "*";
            }
            else
            {
                if (chk_CheckStandOff_Pad.Text.Contains("*"))
                    chk_CheckStandOff_Pad.Text = chk_CheckStandOff_Pad.Text.Substring(0, chk_CheckStandOff_Pad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x10000000) > 0)
            {
                if (!chk_CheckEdgeDistance_Pad.Text.Contains("*"))
                    chk_CheckEdgeDistance_Pad.Text += "*";
            }
            else
            {
                if (chk_CheckEdgeDistance_Pad.Text.Contains("*"))
                    chk_CheckEdgeDistance_Pad.Text = chk_CheckEdgeDistance_Pad.Text.Substring(0, chk_CheckEdgeDistance_Pad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x40000000) > 0)
            {
                if (!chk_CheckSpanX_Pad.Text.Contains("*"))
                    chk_CheckSpanX_Pad.Text += "*";
            }
            else
            {
                if (chk_CheckSpanX_Pad.Text.Contains("*"))
                    chk_CheckSpanX_Pad.Text = chk_CheckSpanX_Pad.Text.Substring(0, chk_CheckSpanX_Pad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x80000000) > 0)
            {
                if (!chk_CheckSpanY_Pad.Text.Contains("*"))
                    chk_CheckSpanY_Pad.Text += "*";
            }
            else
            {
                if (chk_CheckSpanY_Pad.Text.Contains("*"))
                    chk_CheckSpanY_Pad.Text = chk_CheckSpanY_Pad.Text.Substring(0, chk_CheckSpanY_Pad.Text.Length - 1);
            }

            //------------------------------------Pin1--------------------------------
            if (m_smVisionInfo.g_blnWantPin1)
            {
                if ((m_smVisionInfo.g_intOptionControlMask & 0x800) > 0)
                {
                    if (!chk_WantInspectPin1.Text.Contains("*"))
                        chk_WantInspectPin1.Text += "*";
                }
                else
                {
                    if (chk_WantInspectPin1.Text.Contains("*"))
                        chk_WantInspectPin1.Text = chk_WantInspectPin1.Text.Substring(0, chk_WantInspectPin1.Text.Length - 1);
                }
            }

            if (m_smVisionInfo.g_strVisionName != "Pad5SPkg" && m_smVisionInfo.g_strVisionName != "PadPkg")
            {
                // Foreign Material / Contamination
                if ((m_smVisionInfo.g_intOptionControlMask & 0x08) > 0)
                {
                    if (!chk_CheckForeignMaterialArea_Pad.Text.Contains("*"))
                        chk_CheckForeignMaterialArea_Pad.Text += "*";
                }
                else
                {
                    if (chk_CheckForeignMaterialArea_Pad.Text.Contains("*"))
                        chk_CheckForeignMaterialArea_Pad.Text = chk_CheckForeignMaterialArea_Pad.Text.Substring(0, chk_CheckForeignMaterialArea_Pad.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intOptionControlMask & 0x400) > 0)
                {
                    if (!chk_CheckForeignMaterialTotalArea_Pad.Text.Contains("*"))
                        chk_CheckForeignMaterialTotalArea_Pad.Text += "*";
                }
                else
                {
                    if (chk_CheckForeignMaterialTotalArea_Pad.Text.Contains("*"))
                        chk_CheckForeignMaterialTotalArea_Pad.Text = chk_CheckForeignMaterialTotalArea_Pad.Text.Substring(0, chk_CheckForeignMaterialTotalArea_Pad.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intOptionControlMask & 0x10) > 0)
                {
                    if (!chk_CheckForeignMaterialLength_Pad.Text.Contains("*"))
                        chk_CheckForeignMaterialLength_Pad.Text += "*";
                }
                else
                {
                    if (chk_CheckForeignMaterialLength_Pad.Text.Contains("*"))
                        chk_CheckForeignMaterialLength_Pad.Text = chk_CheckForeignMaterialLength_Pad.Text.Substring(0, chk_CheckForeignMaterialLength_Pad.Text.Length - 1);
                }
            }
        }

        private void UpdateSidePadControlSetting()
        {
            // Area
            if ((m_smVisionInfo.g_intOptionControlMask & 0x1000) > 0)
            {
                if (!chk_Area_SidePad.Text.Contains("*"))
                    chk_Area_SidePad.Text += "*";
            }
            else
            {
                if (chk_Area_SidePad.Text.Contains("*"))
                    chk_Area_SidePad.Text = chk_Area_SidePad.Text.Substring(0, chk_Area_SidePad.Text.Length - 1);
            }

            // Width and Height
            if ((m_smVisionInfo.g_intOptionControlMask & 0x2000) > 0)
            {
                if (!chk_WidthHeight_SidePad.Text.Contains("*"))
                    chk_WidthHeight_SidePad.Text += "*";
            }
            else
            {
                if (chk_WidthHeight_SidePad.Text.Contains("*"))
                    chk_WidthHeight_SidePad.Text = chk_WidthHeight_SidePad.Text.Substring(0, chk_WidthHeight_SidePad.Text.Length - 1);
            }

            // Off Set
            if ((m_smVisionInfo.g_intOptionControlMask & 0x4000) > 0)
            {
                if (!chk_OffSet_SidePad.Text.Contains("*"))
                    chk_OffSet_SidePad.Text += "*";
            }
            else
            {
                if (chk_OffSet_SidePad.Text.Contains("*"))
                    chk_OffSet_SidePad.Text = chk_OffSet_SidePad.Text.Substring(0, chk_OffSet_SidePad.Text.Length - 1);
            }

            // Broken Pad 
            if ((m_smVisionInfo.g_intOptionControlMask & 0x20000) > 0)
            {
                if (!chk_CheckBrokenArea_SidePad.Text.Contains("*"))
                    chk_CheckBrokenArea_SidePad.Text += "*";
            }
            else
            {
                if (chk_CheckBrokenArea_SidePad.Text.Contains("*"))
                    chk_CheckBrokenArea_SidePad.Text = chk_CheckBrokenArea_SidePad.Text.Substring(0, chk_CheckBrokenArea_SidePad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x40000) > 0)
            {
                if (!chk_CheckBrokenLength_SidePad.Text.Contains("*"))
                    chk_CheckBrokenLength_SidePad.Text += "*";
            }
            else
            {
                if (chk_CheckBrokenLength_SidePad.Text.Contains("*"))
                    chk_CheckBrokenLength_SidePad.Text = chk_CheckBrokenLength_SidePad.Text.Substring(0, chk_CheckBrokenLength_SidePad.Text.Length - 1);
            }

            // Pitch Gap
            if ((m_smVisionInfo.g_intOptionControlMask & 0x80000) > 0)
            {
                if (!chk_Gap_SidePad.Text.Contains("*"))
                    chk_Gap_SidePad.Text += "*";
            }
            else
            {
                if (chk_Gap_SidePad.Text.Contains("*"))
                    chk_Gap_SidePad.Text = chk_Gap_SidePad.Text.Substring(0, chk_Gap_SidePad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x100000) > 0)
            {
                if (!chk_CheckExcess_SidePad.Text.Contains("*"))
                    chk_CheckExcess_SidePad.Text += "*";
            }
            else
            {
                if (chk_CheckExcess_SidePad.Text.Contains("*"))
                    chk_CheckExcess_SidePad.Text = chk_CheckExcess_SidePad.Text.Substring(0, chk_CheckExcess_SidePad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x200000) > 0)
            {
                if (!chk_CheckSmear_SidePad.Text.Contains("*"))
                    chk_CheckSmear_SidePad.Text += "*";
            }
            else
            {
                if (chk_CheckSmear_SidePad.Text.Contains("*"))
                    chk_CheckSmear_SidePad.Text = chk_CheckSmear_SidePad.Text.Substring(0, chk_CheckSmear_SidePad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x1000000) > 0)
            {
                if (!chk_CheckEdgeLimit_SidePad.Text.Contains("*"))
                    chk_CheckEdgeLimit_SidePad.Text += "*";
            }
            else
            {
                if (chk_CheckEdgeLimit_SidePad.Text.Contains("*"))
                    chk_CheckEdgeLimit_SidePad.Text = chk_CheckEdgeLimit_SidePad.Text.Substring(0, chk_CheckEdgeLimit_SidePad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x8000000) > 0)
            {
                if (!chk_CheckStandOff_SidePad.Text.Contains("*"))
                    chk_CheckStandOff_SidePad.Text += "*";
            }
            else
            {
                if (chk_CheckStandOff_SidePad.Text.Contains("*"))
                    chk_CheckStandOff_SidePad.Text = chk_CheckStandOff_SidePad.Text.Substring(0, chk_CheckStandOff_SidePad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x20000000) > 0)
            {
                if (!chk_CheckEdgeDistance_SidePad.Text.Contains("*"))
                    chk_CheckEdgeDistance_SidePad.Text += "*";
            }
            else
            {
                if (chk_CheckEdgeDistance_SidePad.Text.Contains("*"))
                    chk_CheckEdgeDistance_SidePad.Text = chk_CheckEdgeDistance_SidePad.Text.Substring(0, chk_CheckEdgeDistance_SidePad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x100000000) > 0)
            {
                if (!chk_CheckSpanX_SidePad.Text.Contains("*"))
                    chk_CheckSpanX_SidePad.Text += "*";
            }
            else
            {
                if (chk_CheckSpanX_SidePad.Text.Contains("*"))
                    chk_CheckSpanX_SidePad.Text = chk_CheckSpanX_SidePad.Text.Substring(0, chk_CheckSpanX_SidePad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x200000000) > 0)
            {
                if (!chk_CheckSpanY_SidePad.Text.Contains("*"))
                    chk_CheckSpanY_SidePad.Text += "*";
            }
            else
            {
                if (chk_CheckSpanY_SidePad.Text.Contains("*"))
                    chk_CheckSpanY_SidePad.Text = chk_CheckSpanY_SidePad.Text.Substring(0, chk_CheckSpanY_SidePad.Text.Length - 1);
            }

            if (m_smVisionInfo.g_strVisionName != "Pad5SPkg" && m_smVisionInfo.g_strVisionName != "PadPkg")
            {
                // Foreign Material / Contamination
                if ((m_smVisionInfo.g_intOptionControlMask & 0x8000) > 0)
                {
                    if (!chk_CheckForeignMaterialArea_SidePad.Text.Contains("*"))
                        chk_CheckForeignMaterialArea_SidePad.Text += "*";
                }
                else
                {
                    if (chk_CheckForeignMaterialArea_SidePad.Text.Contains("*"))
                        chk_CheckForeignMaterialArea_SidePad.Text = chk_CheckForeignMaterialArea_SidePad.Text.Substring(0, chk_CheckForeignMaterialArea_SidePad.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intOptionControlMask & 0x400000) > 0)
                {
                    if (!chk_CheckForeignMaterialTotalArea_SidePad.Text.Contains("*"))
                        chk_CheckForeignMaterialTotalArea_SidePad.Text += "*";
                }
                else
                {
                    if (chk_CheckForeignMaterialTotalArea_SidePad.Text.Contains("*"))
                        chk_CheckForeignMaterialTotalArea_SidePad.Text = chk_CheckForeignMaterialTotalArea_SidePad.Text.Substring(0, chk_CheckForeignMaterialTotalArea_SidePad.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intOptionControlMask & 0x10000) > 0)
                {
                    if (!chk_CheckForeignMaterialLength_SidePad.Text.Contains("*"))
                        chk_CheckForeignMaterialLength_SidePad.Text += "*";
                }
                else
                {
                    if (chk_CheckForeignMaterialLength_SidePad.Text.Contains("*"))
                        chk_CheckForeignMaterialLength_SidePad.Text = chk_CheckForeignMaterialLength_SidePad.Text.Substring(0, chk_CheckForeignMaterialLength_SidePad.Text.Length - 1);
                }
            }
        }

        private void UpdateCenterPackagePadControlSetting()
        {
            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x2000) > 0)
            {
                if (!chk_InspectPackage_Pad2.Text.Contains("*"))
                    chk_InspectPackage_Pad2.Text += "*";
            }
            else
            {
                if (chk_InspectPackage_Pad2.Text.Contains("*"))
                    chk_InspectPackage_Pad2.Text = chk_InspectPackage_Pad2.Text.Substring(0, chk_InspectPackage_Pad2.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x4000) > 0)
            {
                if (!chk_CheckPkgSize_Pad2.Text.Contains("*"))
                    chk_CheckPkgSize_Pad2.Text += "*";
            }
            else
            {
                if (chk_CheckPkgSize_Pad2.Text.Contains("*"))
                    chk_CheckPkgSize_Pad2.Text = chk_CheckPkgSize_Pad2.Text.Substring(0, chk_CheckPkgSize_Pad2.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x8000) > 0)
            {
                if (!chk_BrightFieldArea_Pad.Text.Contains("*"))
                    chk_BrightFieldArea_Pad.Text += "*";
            }
            else
            {
                if (chk_BrightFieldArea_Pad.Text.Contains("*"))
                    chk_BrightFieldArea_Pad.Text = chk_BrightFieldArea_Pad.Text.Substring(0, chk_BrightFieldArea_Pad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x10000) > 0)
            {
                if (!chk_BrightFieldLength_Pad.Text.Contains("*"))
                    chk_BrightFieldLength_Pad.Text += "*";
            }
            else
            {
                if (chk_BrightFieldLength_Pad.Text.Contains("*"))
                    chk_BrightFieldLength_Pad.Text = chk_BrightFieldLength_Pad.Text.Substring(0, chk_BrightFieldLength_Pad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x20000) > 0)
            {
                if (!chk_DarkFieldArea_Pad.Text.Contains("*"))
                    chk_DarkFieldArea_Pad.Text += "*";
            }
            else
            {
                if (chk_DarkFieldArea_Pad.Text.Contains("*"))
                    chk_DarkFieldArea_Pad.Text = chk_DarkFieldArea_Pad.Text.Substring(0, chk_DarkFieldArea_Pad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x40000) > 0)
            {
                if (!chk_DarkFieldLength_Pad.Text.Contains("*"))
                    chk_DarkFieldLength_Pad.Text += "*";
            }
            else
            {
                if (chk_DarkFieldLength_Pad.Text.Contains("*"))
                    chk_DarkFieldLength_Pad.Text = chk_DarkFieldLength_Pad.Text.Substring(0, chk_DarkFieldLength_Pad.Text.Length - 1);
            }

            if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x80000) > 0)
                {
                    if (!chk_CrackDarkFieldArea_Pad.Text.Contains("*"))
                        chk_CrackDarkFieldArea_Pad.Text += "*";
                }
                else
                {
                    if (chk_CrackDarkFieldArea_Pad.Text.Contains("*"))
                        chk_CrackDarkFieldArea_Pad.Text = chk_CrackDarkFieldArea_Pad.Text.Substring(0, chk_CrackDarkFieldArea_Pad.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x100000) > 0)
                {
                    if (!chk_CrackDarkFieldLength_Pad.Text.Contains("*"))
                        chk_CrackDarkFieldLength_Pad.Text += "*";
                }
                else
                {
                    if (chk_CrackDarkFieldLength_Pad.Text.Contains("*"))
                        chk_CrackDarkFieldLength_Pad.Text = chk_CrackDarkFieldLength_Pad.Text.Substring(0, chk_CrackDarkFieldLength_Pad.Text.Length - 1);
                }
            }

            if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateChippedOffDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x200000) > 0)
                {
                    if (!chk_ChippedOffDarkFieldArea_Pad.Text.Contains("*"))
                        chk_ChippedOffDarkFieldArea_Pad.Text += "*";
                }
                else
                {
                    if (chk_ChippedOffDarkFieldArea_Pad.Text.Contains("*"))
                        chk_ChippedOffDarkFieldArea_Pad.Text = chk_ChippedOffDarkFieldArea_Pad.Text.Substring(0, chk_ChippedOffDarkFieldArea_Pad.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x400000) > 0)
                {
                    if (!chk_ChippedOffBrightFieldArea_Pad.Text.Contains("*"))
                        chk_ChippedOffBrightFieldArea_Pad.Text += "*";
                }
                else
                {
                    if (chk_ChippedOffBrightFieldArea_Pad.Text.Contains("*"))
                        chk_ChippedOffBrightFieldArea_Pad.Text = chk_ChippedOffBrightFieldArea_Pad.Text.Substring(0, chk_ChippedOffBrightFieldArea_Pad.Text.Length - 1);
                }
            }

            if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x800000) > 0)
                {
                    if (!chk_MoldFlashBrightFieldArea_Pad.Text.Contains("*"))
                        chk_MoldFlashBrightFieldArea_Pad.Text += "*";
                }
                else
                {
                    if (chk_MoldFlashBrightFieldArea_Pad.Text.Contains("*"))
                        chk_MoldFlashBrightFieldArea_Pad.Text = chk_MoldFlashBrightFieldArea_Pad.Text.Substring(0, chk_MoldFlashBrightFieldArea_Pad.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x4000000) > 0)
                {
                    if (!chk_MoldFlashBrightFieldLength_Pad.Text.Contains("*"))
                        chk_MoldFlashBrightFieldLength_Pad.Text += "*";
                }
                else
                {
                    if (chk_MoldFlashBrightFieldLength_Pad.Text.Contains("*"))
                        chk_MoldFlashBrightFieldLength_Pad.Text = chk_MoldFlashBrightFieldLength_Pad.Text.Substring(0, chk_MoldFlashBrightFieldLength_Pad.Text.Length - 1);
                }
            }
            if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateForeignMaterialDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x1000000) > 0)
                {
                    if (!chk_ForeignMaterialBrightFieldArea_Pad.Text.Contains("*"))
                        chk_ForeignMaterialBrightFieldArea_Pad.Text += "*";
                }
                else
                {
                    if (chk_ForeignMaterialBrightFieldArea_Pad.Text.Contains("*"))
                        chk_ForeignMaterialBrightFieldArea_Pad.Text = chk_ForeignMaterialBrightFieldArea_Pad.Text.Substring(0, chk_ForeignMaterialBrightFieldArea_Pad.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x2000000) > 0)
                {
                    if (!chk_ForeignMaterialBrightFieldLength_Pad.Text.Contains("*"))
                        chk_ForeignMaterialBrightFieldLength_Pad.Text += "*";
                }
                else
                {
                    if (chk_ForeignMaterialBrightFieldLength_Pad.Text.Contains("*"))
                        chk_ForeignMaterialBrightFieldLength_Pad.Text = chk_ForeignMaterialBrightFieldLength_Pad.Text.Substring(0, chk_ForeignMaterialBrightFieldLength_Pad.Text.Length - 1);
                }
            }
        }

        private void UpdateSidePacakgePadControlSetting()
        {
            if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x2000) > 0)
            {
                if (!chk_InspectPackage_SidePad2.Text.Contains("*"))
                    chk_InspectPackage_SidePad2.Text += "*";
            }
            else
            {
                if (chk_InspectPackage_SidePad2.Text.Contains("*"))
                    chk_InspectPackage_SidePad2.Text = chk_InspectPackage_SidePad2.Text.Substring(0, chk_InspectPackage_SidePad2.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x4000) > 0)
            {
                if (!chk_CheckPkgSize_SidePad2.Text.Contains("*"))
                    chk_CheckPkgSize_SidePad2.Text += "*";
            }
            else
            {
                if (chk_CheckPkgSize_SidePad2.Text.Contains("*"))
                    chk_CheckPkgSize_SidePad2.Text = chk_CheckPkgSize_SidePad2.Text.Substring(0, chk_CheckPkgSize_SidePad2.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x8000) > 0)
            {
                if (!chk_BrightFieldArea_SidePad.Text.Contains("*"))
                    chk_BrightFieldArea_SidePad.Text += "*";
            }
            else
            {
                if (chk_BrightFieldArea_SidePad.Text.Contains("*"))
                    chk_BrightFieldArea_SidePad.Text = chk_BrightFieldArea_SidePad.Text.Substring(0, chk_BrightFieldArea_SidePad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x10000) > 0)
            {
                if (!chk_BrightFieldLength_SidePad.Text.Contains("*"))
                    chk_BrightFieldLength_SidePad.Text += "*";
            }
            else
            {
                if (chk_BrightFieldLength_SidePad.Text.Contains("*"))
                    chk_BrightFieldLength_SidePad.Text = chk_BrightFieldLength_SidePad.Text.Substring(0, chk_BrightFieldLength_SidePad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x20000) > 0)
            {
                if (!chk_DarkFieldArea_SidePad.Text.Contains("*"))
                    chk_DarkFieldArea_SidePad.Text += "*";
            }
            else
            {
                if (chk_DarkFieldArea_SidePad.Text.Contains("*"))
                    chk_DarkFieldArea_SidePad.Text = chk_DarkFieldArea_SidePad.Text.Substring(0, chk_DarkFieldArea_SidePad.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x40000) > 0)
            {
                if (!chk_DarkFieldLength_SidePad.Text.Contains("*"))
                    chk_DarkFieldLength_SidePad.Text += "*";
            }
            else
            {
                if (chk_DarkFieldLength_SidePad.Text.Contains("*"))
                    chk_DarkFieldLength_SidePad.Text = chk_DarkFieldLength_SidePad.Text.Substring(0, chk_DarkFieldLength_SidePad.Text.Length - 1);
            }

            if (m_smVisionInfo.g_arrPad[1].ref_blnSeperateCrackDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x80000) > 0)
                {
                    if (!chk_CrackDarkFieldArea_SidePad.Text.Contains("*"))
                        chk_CrackDarkFieldArea_SidePad.Text += "*";
                }
                else
                {
                    if (chk_CrackDarkFieldArea_SidePad.Text.Contains("*"))
                        chk_CrackDarkFieldArea_SidePad.Text = chk_CrackDarkFieldArea_SidePad.Text.Substring(0, chk_CrackDarkFieldArea_SidePad.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x100000) > 0)
                {
                    if (!chk_CrackDarkFieldLength_SidePad.Text.Contains("*"))
                        chk_CrackDarkFieldLength_SidePad.Text += "*";
                }
                else
                {
                    if (chk_CrackDarkFieldLength_SidePad.Text.Contains("*"))
                        chk_CrackDarkFieldLength_SidePad.Text = chk_CrackDarkFieldLength_SidePad.Text.Substring(0, chk_CrackDarkFieldLength_SidePad.Text.Length - 1);
                }
            }

            if (m_smVisionInfo.g_arrPad[1].ref_blnSeperateChippedOffDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x200000) > 0)
                {
                    if (!chk_ChippedOffDarkFieldArea_SidePad.Text.Contains("*"))
                        chk_ChippedOffDarkFieldArea_SidePad.Text += "*";
                }
                else
                {
                    if (chk_ChippedOffDarkFieldArea_SidePad.Text.Contains("*"))
                        chk_ChippedOffDarkFieldArea_SidePad.Text = chk_ChippedOffDarkFieldArea_SidePad.Text.Substring(0, chk_ChippedOffDarkFieldArea_SidePad.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x400000) > 0)
                {
                    if (!chk_ChippedOffBrightFieldArea_SidePad.Text.Contains("*"))
                        chk_ChippedOffBrightFieldArea_SidePad.Text += "*";
                }
                else
                {
                    if (chk_ChippedOffBrightFieldArea_SidePad.Text.Contains("*"))
                        chk_ChippedOffBrightFieldArea_SidePad.Text = chk_ChippedOffBrightFieldArea_SidePad.Text.Substring(0, chk_ChippedOffBrightFieldArea_SidePad.Text.Length - 1);
                }
            }

            if (m_smVisionInfo.g_arrPad[1].ref_blnSeperateMoldFlashDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x800000) > 0)
                {
                    if (!chk_MoldFlashBrightFieldArea_SidePad.Text.Contains("*"))
                        chk_MoldFlashBrightFieldArea_SidePad.Text += "*";
                }
                else
                {
                    if (chk_MoldFlashBrightFieldArea_SidePad.Text.Contains("*"))
                        chk_MoldFlashBrightFieldArea_SidePad.Text = chk_MoldFlashBrightFieldArea_SidePad.Text.Substring(0, chk_MoldFlashBrightFieldArea_SidePad.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x4000000) > 0)
                {
                    if (!chk_MoldFlashBrightFieldLength_SidePad.Text.Contains("*"))
                        chk_MoldFlashBrightFieldLength_SidePad.Text += "*";
                }
                else
                {
                    if (chk_MoldFlashBrightFieldLength_SidePad.Text.Contains("*"))
                        chk_MoldFlashBrightFieldLength_SidePad.Text = chk_MoldFlashBrightFieldLength_SidePad.Text.Substring(0, chk_MoldFlashBrightFieldLength_SidePad.Text.Length - 1);
                }
            }
            if (m_smVisionInfo.g_arrPad[1].ref_blnSeperateForeignMaterialDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x1000000) > 0)
                {
                    if (!chk_ForeignMaterialBrightFieldArea_SidePad.Text.Contains("*"))
                        chk_ForeignMaterialBrightFieldArea_SidePad.Text += "*";
                }
                else
                {
                    if (chk_ForeignMaterialBrightFieldArea_SidePad.Text.Contains("*"))
                        chk_ForeignMaterialBrightFieldArea_SidePad.Text = chk_ForeignMaterialBrightFieldArea_SidePad.Text.Substring(0, chk_ForeignMaterialBrightFieldArea_SidePad.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask2 & 0x2000000) > 0)
                {
                    if (!chk_ForeignMaterialBrightFieldLength_SidePad.Text.Contains("*"))
                        chk_ForeignMaterialBrightFieldLength_SidePad.Text += "*";
                }
                else
                {
                    if (chk_ForeignMaterialBrightFieldLength_SidePad.Text.Contains("*"))
                        chk_ForeignMaterialBrightFieldLength_SidePad.Text = chk_ForeignMaterialBrightFieldLength_SidePad.Text.Substring(0, chk_ForeignMaterialBrightFieldLength_SidePad.Text.Length - 1);
                }
            }
        }

        private void UpdateLead3DControlSetting()
        {
            // Offset 
            if ((m_smVisionInfo.g_intOptionControlMask & 0x10000) > 0)
            {
                if (!chk_Offset_Lead3D.Text.Contains("*"))
                    chk_Offset_Lead3D.Text += "*";
            }
            else
            {
                if (chk_Offset_Lead3D.Text.Contains("*"))
                    chk_Offset_Lead3D.Text = chk_Offset_Lead3D.Text.Substring(0, chk_Offset_Lead3D.Text.Length - 1);
            }

            // Skew 
            if ((m_smVisionInfo.g_intOptionControlMask & 0x8000) > 0)
            {
                if (!chk_Skew_Lead3D.Text.Contains("*"))
                    chk_Skew_Lead3D.Text += "*";
            }
            else
            {
                if (chk_Skew_Lead3D.Text.Contains("*"))
                    chk_Skew_Lead3D.Text = chk_Skew_Lead3D.Text.Substring(0, chk_Skew_Lead3D.Text.Length - 1);
            }

            // Width 
            if ((m_smVisionInfo.g_intOptionControlMask & 0x01) > 0)
            {
                if (!chk_Width_Lead3D.Text.Contains("*"))
                    chk_Width_Lead3D.Text += "*";
            }
            else
            {
                if (chk_Width_Lead3D.Text.Contains("*"))
                    chk_Width_Lead3D.Text = chk_Width_Lead3D.Text.Substring(0, chk_Width_Lead3D.Text.Length - 1);
            }

            // Length
            if ((m_smVisionInfo.g_intOptionControlMask & 0x02) > 0)
            {
                if (!chk_Length_Lead3D.Text.Contains("*"))
                    chk_Length_Lead3D.Text += "*";
            }
            else
            {
                if (chk_Length_Lead3D.Text.Contains("*"))
                    chk_Length_Lead3D.Text = chk_Length_Lead3D.Text.Substring(0, chk_Length_Lead3D.Text.Length - 1);
            }

            // Length Variance 
            if ((m_smVisionInfo.g_intOptionControlMask & 0x04) > 0)
            {
                if (!chk_LengthVariance_Lead3D.Text.Contains("*"))
                    chk_LengthVariance_Lead3D.Text += "*";
            }
            else
            {
                if (chk_LengthVariance_Lead3D.Text.Contains("*"))
                    chk_LengthVariance_Lead3D.Text = chk_LengthVariance_Lead3D.Text.Substring(0, chk_LengthVariance_Lead3D.Text.Length - 1);
            }

            // Pitch
            if ((m_smVisionInfo.g_intOptionControlMask & 0x08) > 0)
            {
                if (!chk_PitchGap_Lead3D.Text.Contains("*"))
                    chk_PitchGap_Lead3D.Text += "*";
            }
            else
            {
                if (chk_PitchGap_Lead3D.Text.Contains("*"))
                    chk_PitchGap_Lead3D.Text = chk_PitchGap_Lead3D.Text.Substring(0, chk_PitchGap_Lead3D.Text.Length - 1);
            }

            // Pitch Variance
            if ((m_smVisionInfo.g_intOptionControlMask & 0x10) > 0)
            {
                if (!chk_PitchVariance_Lead3D.Text.Contains("*"))
                    chk_PitchVariance_Lead3D.Text += "*";
            }
            else
            {
                if (chk_PitchVariance_Lead3D.Text.Contains("*"))
                    chk_PitchVariance_Lead3D.Text = chk_PitchVariance_Lead3D.Text.Substring(0, chk_PitchVariance_Lead3D.Text.Length - 1);
            }

            // Stand Off 
            if ((m_smVisionInfo.g_intOptionControlMask & 0x20) > 0)
            {
                if (!chk_StandOff_Lead3D.Text.Contains("*"))
                    chk_StandOff_Lead3D.Text += "*";
            }
            else
            {
                if (chk_StandOff_Lead3D.Text.Contains("*"))
                    chk_StandOff_Lead3D.Text = chk_StandOff_Lead3D.Text.Substring(0, chk_StandOff_Lead3D.Text.Length - 1);
            }

            // Stand Off Variance 
            if ((m_smVisionInfo.g_intOptionControlMask & 0x40) > 0)
            {
                if (!chk_StandoffVariance_Lead3D.Text.Contains("*"))
                    chk_StandoffVariance_Lead3D.Text += "*";
            }
            else
            {
                if (chk_StandoffVariance_Lead3D.Text.Contains("*"))
                    chk_StandoffVariance_Lead3D.Text = chk_StandoffVariance_Lead3D.Text.Substring(0, chk_StandoffVariance_Lead3D.Text.Length - 1);
            }

            // Coplan 
            if ((m_smVisionInfo.g_intOptionControlMask & 0x80) > 0)
            {
                if (!chk_Coplan_Lead3D.Text.Contains("*"))
                    chk_Coplan_Lead3D.Text += "*";
            }
            else
            {
                if (chk_Coplan_Lead3D.Text.Contains("*"))
                    chk_Coplan_Lead3D.Text = chk_Coplan_Lead3D.Text.Substring(0, chk_Coplan_Lead3D.Text.Length - 1);
            }

            // Span
            if ((m_smVisionInfo.g_intOptionControlMask & 0x100) > 0)
            {
                if (!chk_Span_Lead3D.Text.Contains("*"))
                    chk_Span_Lead3D.Text += "*";
            }
            else
            {
                if (chk_Span_Lead3D.Text.Contains("*"))
                    chk_Span_Lead3D.Text = chk_Span_Lead3D.Text.Substring(0, chk_Span_Lead3D.Text.Length - 1);
            }

            // Lead Sweeps
            if ((m_smVisionInfo.g_intOptionControlMask & 0x200) > 0)
            {
                if (!chk_LeadSweeps_Lead3D.Text.Contains("*"))
                    chk_LeadSweeps_Lead3D.Text += "*";
            }
            else
            {
                if (chk_LeadSweeps_Lead3D.Text.Contains("*"))
                    chk_LeadSweeps_Lead3D.Text = chk_LeadSweeps_Lead3D.Text.Substring(0, chk_LeadSweeps_Lead3D.Text.Length - 1);
            }

            // Solder Pad Length  
            if ((m_smVisionInfo.g_intOptionControlMask & 0x400) > 0)
            {
                if (!chk_SolderPadLength_Lead3D.Text.Contains("*"))
                    chk_SolderPadLength_Lead3D.Text += "*";
            }
            else
            {
                if (chk_SolderPadLength_Lead3D.Text.Contains("*"))
                    chk_SolderPadLength_Lead3D.Text = chk_SolderPadLength_Lead3D.Text.Substring(0, chk_SolderPadLength_Lead3D.Text.Length - 1);
            }

            // Un-Cut Tiebar
            if ((m_smVisionInfo.g_intOptionControlMask & 0x800) > 0)
            {
                if (!chk_UnCutTiebar_Lead3D.Text.Contains("*"))
                    chk_UnCutTiebar_Lead3D.Text += "*";
            }
            else
            {
                if (chk_UnCutTiebar_Lead3D.Text.Contains("*"))
                    chk_UnCutTiebar_Lead3D.Text = chk_UnCutTiebar_Lead3D.Text.Substring(0, chk_UnCutTiebar_Lead3D.Text.Length - 1);
            }

            // Foreign Material / Contamination
            if ((m_smVisionInfo.g_intOptionControlMask & 0x1000) > 0)
            {
                if (!chk_CheckForeignMaterialArea_Lead3D.Text.Contains("*"))
                    chk_CheckForeignMaterialArea_Lead3D.Text += "*";
            }
            else
            {
                if (chk_CheckForeignMaterialArea_Lead3D.Text.Contains("*"))
                    chk_CheckForeignMaterialArea_Lead3D.Text = chk_CheckForeignMaterialArea_Lead3D.Text.Substring(0, chk_CheckForeignMaterialArea_Lead3D.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x2000) > 0)
            {
                if (!chk_CheckForeignMaterialLength_Lead3D.Text.Contains("*"))
                    chk_CheckForeignMaterialLength_Lead3D.Text += "*";
            }
            else
            {
                if (chk_CheckForeignMaterialLength_Lead3D.Text.Contains("*"))
                    chk_CheckForeignMaterialLength_Lead3D.Text = chk_CheckForeignMaterialLength_Lead3D.Text.Substring(0, chk_CheckForeignMaterialLength_Lead3D.Text.Length - 1);
            }
            
            if ((m_smVisionInfo.g_intOptionControlMask & 0x4000) > 0)
            {
                if (!chk_CheckForeignMaterialTotalArea_Lead3D.Text.Contains("*"))
                    chk_CheckForeignMaterialTotalArea_Lead3D.Text += "*";
            }
            else
            {
                if (chk_CheckForeignMaterialTotalArea_Lead3D.Text.Contains("*"))
                    chk_CheckForeignMaterialTotalArea_Lead3D.Text = chk_CheckForeignMaterialTotalArea_Lead3D.Text.Substring(0, chk_CheckForeignMaterialTotalArea_Lead3D.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x20000) > 0)
            {
                if (!chk_AverageGrayValue_Lead3D.Text.Contains("*"))
                    chk_AverageGrayValue_Lead3D.Text += "*";
            }
            else
            {
                if (chk_AverageGrayValue_Lead3D.Text.Contains("*"))
                    chk_AverageGrayValue_Lead3D.Text = chk_AverageGrayValue_Lead3D.Text.Substring(0, chk_AverageGrayValue_Lead3D.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x40000) > 0)
            {
                if (!chk_LeadMinAndMaxWidth_Lead3D.Text.Contains("*"))
                    chk_LeadMinAndMaxWidth_Lead3D.Text += "*";
            }
            else
            {
                if (chk_LeadMinAndMaxWidth_Lead3D.Text.Contains("*"))
                    chk_LeadMinAndMaxWidth_Lead3D.Text = chk_LeadMinAndMaxWidth_Lead3D.Text.Substring(0, chk_LeadMinAndMaxWidth_Lead3D.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x80000) > 0)
            {
                if (!chk_LeadBurrWidth.Text.Contains("*"))
                    chk_LeadBurrWidth.Text += "*";
            }
            else
            {
                if (chk_LeadBurrWidth.Text.Contains("*"))
                    chk_LeadBurrWidth.Text = chk_LeadBurrWidth.Text.Substring(0, chk_LeadBurrWidth.Text.Length - 1);
            }

            //------------------------------------Pin1--------------------------------
            if (m_smVisionInfo.g_blnWantPin1)
            {
                if ((m_smVisionInfo.g_intOptionControlMask & 0x800) > 0)
                {
                    if (!chk_WantInspectPin1.Text.Contains("*"))
                        chk_WantInspectPin1.Text += "*";
                }
                else
                {
                    if (chk_WantInspectPin1.Text.Contains("*"))
                        chk_WantInspectPin1.Text = chk_WantInspectPin1.Text.Substring(0, chk_WantInspectPin1.Text.Length - 1);
                }
            }

        }

        private void UpdateLead3DPkgControlSetting()
        {
            // Use simple defect criteria

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x2000) > 0)
            {
                if (!chk_InspectPackage_Lead3D.Text.Contains("*"))
                    chk_InspectPackage_Lead3D.Text += "*";
            }
            else
            {
                if (chk_InspectPackage_Lead3D.Text.Contains("*"))
                    chk_InspectPackage_Lead3D.Text = chk_InspectPackage_Lead3D.Text.Substring(0, chk_InspectPackage_Lead3D.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x4000) > 0)
            {
                if (!chk_CheckPkgSize_Lead3D.Text.Contains("*"))
                    chk_CheckPkgSize_Lead3D.Text += "*";
            }
            else
            {
                if (chk_CheckPkgSize_Lead3D.Text.Contains("*"))
                    chk_CheckPkgSize_Lead3D.Text = chk_CheckPkgSize_Lead3D.Text.Substring(0, chk_CheckPkgSize_Lead3D.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x8000) > 0)
            {
                if (!chk_BrightFieldArea_Lead3D.Text.Contains("*"))
                    chk_BrightFieldArea_Lead3D.Text += "*";
            }
            else
            {
                if (chk_BrightFieldArea_Lead3D.Text.Contains("*"))
                    chk_BrightFieldArea_Lead3D.Text = chk_BrightFieldArea_Lead3D.Text.Substring(0, chk_BrightFieldArea_Lead3D.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x10000) > 0)
            {
                if (!chk_BrightFieldLength_Lead3D.Text.Contains("*"))
                    chk_BrightFieldLength_Lead3D.Text += "*";
            }
            else
            {
                if (chk_BrightFieldLength_Lead3D.Text.Contains("*"))
                    chk_BrightFieldLength_Lead3D.Text = chk_BrightFieldLength_Lead3D.Text.Substring(0, chk_BrightFieldLength_Lead3D.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x20000) > 0)
            {
                if (!chk_DarkFieldArea_Lead3D.Text.Contains("*"))
                    chk_DarkFieldArea_Lead3D.Text += "*";
            }
            else
            {
                if (chk_DarkFieldArea_Lead3D.Text.Contains("*"))
                    chk_DarkFieldArea_Lead3D.Text = chk_DarkFieldArea_Lead3D.Text.Substring(0, chk_DarkFieldArea_Lead3D.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x40000) > 0)
            {
                if (!chk_DarkFieldLength_Lead3D.Text.Contains("*"))
                    chk_DarkFieldLength_Lead3D.Text += "*";
            }
            else
            {
                if (chk_DarkFieldLength_Lead3D.Text.Contains("*"))
                    chk_DarkFieldLength_Lead3D.Text = chk_DarkFieldLength_Lead3D.Text.Substring(0, chk_DarkFieldLength_Lead3D.Text.Length - 1);
            }

            if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x80000) > 0)
                {
                    if (!chk_CrackDarkFieldArea_Lead3D.Text.Contains("*"))
                        chk_CrackDarkFieldArea_Lead3D.Text += "*";
                }
                else
                {
                    if (chk_CrackDarkFieldArea_Lead3D.Text.Contains("*"))
                        chk_CrackDarkFieldArea_Lead3D.Text = chk_CrackDarkFieldArea_Lead3D.Text.Substring(0, chk_CrackDarkFieldArea_Lead3D.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x100000) > 0)
                {
                    if (!chk_CrackDarkFieldLength_Lead3D.Text.Contains("*"))
                        chk_CrackDarkFieldLength_Lead3D.Text += "*";
                }
                else
                {
                    if (chk_CrackDarkFieldLength_Lead3D.Text.Contains("*"))
                        chk_CrackDarkFieldLength_Lead3D.Text = chk_CrackDarkFieldLength_Lead3D.Text.Substring(0, chk_CrackDarkFieldLength_Lead3D.Text.Length - 1);
                }
            }

            if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x200000) > 0)
                {
                    if (!chk_ChippedOffDarkFieldArea_Lead3D.Text.Contains("*"))
                        chk_ChippedOffDarkFieldArea_Lead3D.Text += "*";
                }
                else
                {
                    if (chk_ChippedOffDarkFieldArea_Lead3D.Text.Contains("*"))
                        chk_ChippedOffDarkFieldArea_Lead3D.Text = chk_ChippedOffDarkFieldArea_Lead3D.Text.Substring(0, chk_ChippedOffDarkFieldArea_Lead3D.Text.Length - 1);
                }

                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x400000) > 0)
                {
                    if (!chk_ChippedOffBrightFieldArea_Lead3D.Text.Contains("*"))
                        chk_ChippedOffBrightFieldArea_Lead3D.Text += "*";
                }
                else
                {
                    if (chk_ChippedOffBrightFieldArea_Lead3D.Text.Contains("*"))
                        chk_ChippedOffBrightFieldArea_Lead3D.Text = chk_ChippedOffBrightFieldArea_Lead3D.Text.Substring(0, chk_ChippedOffBrightFieldArea_Lead3D.Text.Length - 1);
                }
            }

            if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
            {
                if ((m_smVisionInfo.g_intPkgOptionControlMask & 0x800000) > 0)
                {
                    if (!chk_MoldFlashBrightFieldArea_Lead3D.Text.Contains("*"))
                        chk_MoldFlashBrightFieldArea_Lead3D.Text += "*";
                }
                else
                {
                    if (chk_MoldFlashBrightFieldArea_Lead3D.Text.Contains("*"))
                        chk_MoldFlashBrightFieldArea_Lead3D.Text = chk_MoldFlashBrightFieldArea_Lead3D.Text.Substring(0, chk_MoldFlashBrightFieldArea_Lead3D.Text.Length - 1);
                }
            }

        }

        private bool CheckControlSetting()
        {
            switch (m_smVisionInfo.g_strVisionName)
            {
                case "Mark":
                case "MarkOrient":
                case "MOLi":
                case "InPocket":
                case "IPMLi":
                    if (!CheckMarkControlSetting())
                        return false;

                    if ((m_intVisionType & 0x10) > 0)
                    {
                        if (!CheckLeadControlSetting())
                            return false;
                    }
                    break;
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLiPkg":
                    if ((m_intVisionType & 0x10) > 0)
                    {
                        if (!CheckLeadControlSetting())
                            return false;
                    }
                    if (!CheckMarkControlSetting())
                        return false;

                    if (!CheckPackageControlSetting())
                        return false;

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        if (!CheckColorPackageControlSetting())
                            return false;
                    }
                    break;
                case "Package":
                    if (CheckPackageControlSetting())
                        return false;

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        if (!CheckColorPackageControlSetting())
                            return false;
                    }
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                case "Pad":
                case "PadPos":
                    if (!CheckPadControlSetting())
                        return false;

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        if (!CheckCenterColorPadControlSetting())
                            return false;
                    }
                    break;
                case "PadPkg":
                case "PadPkgPos":
                    if (!CheckPadControlSetting())
                        return false;

                    if (!CheckCenterPackagePadControlSetting())
                        return false;

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        if (!CheckCenterColorPadControlSetting())
                            return false;
                    }
                    break;
                case "Pad5S":
                case "Pad5SPos":
                    if (!CheckPadControlSetting())
                        return false;

                    if (!CheckSidePadControlSetting())
                        return false;

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        if (!CheckCenterColorPadControlSetting())
                            return false;

                        if (!CheckSideColorPadControlSetting_Top())
                            return false;
                        if (!CheckSideColorPadControlSetting_Right())
                            return false;
                        if (!CheckSideColorPadControlSetting_Bottom())
                            return false;
                        if (!CheckSideColorPadControlSetting_Left())
                            return false;
                    }
                    break;
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    if (!CheckPadControlSetting())
                        return false;

                    if (!CheckSidePadControlSetting())
                        return false;

                    if (!CheckCenterPackagePadControlSetting())
                        return false;

                    if (!CheckSidePackagePadControlSetting())
                        return false;

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        if (!CheckCenterColorPadControlSetting())
                            return false;

                        if (!CheckSideColorPadControlSetting_Top())
                            return false;
                        if (!CheckSideColorPadControlSetting_Right())
                            return false;
                        if (!CheckSideColorPadControlSetting_Bottom())
                            return false;
                        if (!CheckSideColorPadControlSetting_Left())
                            return false;
                    }
                    break;
                case "Li3D":
                    if (!CheckLead3DControlSetting())
                        return false;
                    break;
                case "Li3DPkg":
                    if (!CheckLead3DControlSetting())
                        return false;

                    if (!CheckLead3DPkgControlSetting())
                        return false;
                    break;
                case "Seal":
                    break;
                case "UnitPresent":
                    break;
                default:
                    SRMMessageBox.Show("btn_Learn_Click -> There is no such vision module name " + m_smVisionInfo.g_strVisionName + " in this SRMVision software version.");
                    break;
            }

            return true;
        }

        private bool CheckMarkControlSetting()
        {
            bool blnResult = true;
            //------------------------------------Mark--------------------------------

            if (chk_CheckMark.Text.Contains("*") && !chk_CheckMark.Checked)
                blnResult = false;

            if (chk_ExcessMarkCharArea.Text.Contains("*") && !chk_ExcessMarkCharArea.Checked || chk_ExcessMarkCharArea.Text.Contains("*") && !chk_ExcessMarkCharArea.Enabled)
                blnResult = false;

            if (chk_GroupExcessMark.Text.Contains("*") && !chk_GroupExcessMark.Checked || chk_GroupExcessMark.Text.Contains("*") && !chk_GroupExcessMark.Enabled && m_smVisionInfo.g_blnWantCheckMarkTotalExcess)
                blnResult = false;

            if (chk_MarkAverageGrayValue.Text.Contains("*") && !chk_MarkAverageGrayValue.Checked || chk_MarkAverageGrayValue.Text.Contains("*") && !chk_MarkAverageGrayValue.Enabled && m_smVisionInfo.g_blnWantCheckMarkAverageGrayValue)
                blnResult = false;

            if (chk_ExtraMark.Text.Contains("*") && !chk_ExtraMark.Checked || chk_ExtraMark.Text.Contains("*") && !chk_ExtraMark.Enabled)
                blnResult = false;

            if (chk_ExtraMarkUncheckArea.Text.Contains("*") && !chk_ExtraMarkUncheckArea.Checked || chk_ExtraMarkUncheckArea.Text.Contains("*") && !chk_ExtraMarkUncheckArea.Enabled)
                blnResult = false;

            if (chk_GroupExtraMark.Text.Contains("*") && !chk_GroupExtraMark.Checked || chk_GroupExtraMark.Text.Contains("*") && !chk_GroupExtraMark.Enabled)
                blnResult = false;

            if (chk_MissingMark.Text.Contains("*") && !chk_MissingMark.Checked || chk_MissingMark.Text.Contains("*") && !chk_MissingMark.Enabled)
                blnResult = false;

            if (chk_BrokenMark.Text.Contains("*") && !chk_BrokenMark.Checked || chk_BrokenMark.Text.Contains("*") && !chk_BrokenMark.Enabled && m_smVisionInfo.g_blnWantCheckMarkBroken)
                blnResult = false;

            if (chk_TextShifted.Text.Contains("*") && !chk_TextShifted.Checked || chk_TextShifted.Text.Contains("*") && !chk_TextShifted.Enabled)
                blnResult = false;

            if (chk_JointMark.Text.Contains("*") && !chk_JointMark.Checked || chk_JointMark.Text.Contains("*") && !chk_JointMark.Enabled)
                blnResult = false;

            if (chk_MarkAngle.Text.Contains("*") && !chk_MarkAngle.Checked || chk_MarkAngle.Text.Contains("*") && !chk_MarkAngle.Enabled && m_smVisionInfo.g_blnWantCheckMarkAngle)
                blnResult = false;

            //------------------------------------Pin1--------------------------------

            if (m_smVisionInfo.g_blnWantPin1)
            {
                if (chk_WantInspectPin1.Text.Contains("*") && !chk_WantInspectPin1.Checked)
                    blnResult = false;
            }

            return blnResult;
        }

        private bool CheckLeadControlSetting()
        {
            bool blnResult = true;
            //------------------------------------Lead--------------------------------

            if (chk_InspectLead.Text.Contains("*") && !chk_InspectLead.Checked)
                blnResult = false;

            if (chk_WidthHeight_Lead.Text.Contains("*") && !chk_WidthHeight_Lead.Checked)
                blnResult = false;

            if (chk_OffSet_Lead.Text.Contains("*") && !chk_OffSet_Lead.Checked)
                blnResult = false;

            if (chk_Skew_Lead.Text.Contains("*") && !chk_Skew_Lead.Checked)
                blnResult = false;

            if (chk_PitchGap_Lead.Text.Contains("*") && !chk_PitchGap_Lead.Checked)
                blnResult = false;

            if (chk_Variance_Lead.Text.Contains("*") && !chk_Variance_Lead.Checked)
                blnResult = false;

            if (chk_AverageGrayValue_Lead.Text.Contains("*") && !chk_AverageGrayValue_Lead.Checked && m_smVisionInfo.g_arrLead[0].ref_blnWantUseAverageGrayValueMethod)
                blnResult = false;

            if (chk_Span_Lead.Text.Contains("*") && !chk_Span_Lead.Checked)
                blnResult = false;

            if (chk_BaseLeadOffset.Text.Contains("*") && !chk_BaseLeadOffset.Checked && m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                blnResult = false;

            if (chk_BaseLeadArea.Text.Contains("*") && !chk_BaseLeadArea.Checked && m_smVisionInfo.g_arrLead[0].ref_blnWantInspectBaseLead)
                blnResult = false;

            return blnResult;
        }
        private bool CheckColorPackageControlSetting()
        {
            bool blnResult = true;

            // Color Defect 1 Length
            if (chk_ColorPackage1Length.Text.Contains("*") && !chk_ColorPackage1Length.Checked)
                blnResult = false;

            // Color Defect 1 Area
            if (chk_ColorPackage1Area.Text.Contains("*") && !chk_ColorPackage1Area.Checked)
                blnResult = false;

            // Color Defect 2 Length
            if (chk_ColorPackage2Length.Text.Contains("*") && !chk_ColorPackage2Length.Checked)
                blnResult = false;

            // Color Defect 2 Area
            if (chk_ColorPackage2Area.Text.Contains("*") && !chk_ColorPackage2Area.Checked)
                blnResult = false;

            // Color Defect 3 Length
            if (chk_ColorPackage3Length.Text.Contains("*") && !chk_ColorPackage3Length.Checked)
                blnResult = false;

            // Color Defect 3 Area
            if (chk_ColorPackage3Area.Text.Contains("*") && !chk_ColorPackage3Area.Checked)
                blnResult = false;

            // Color Defect 4 Length
            if (chk_ColorPackage4Length.Text.Contains("*") && !chk_ColorPackage4Length.Checked)
                blnResult = false;

            // Color Defect 4 Area
            if (chk_ColorPackage4Area.Text.Contains("*") && !chk_ColorPackage4Area.Checked)
                blnResult = false;

            // Color Defect 5 Length
            if (chk_ColorPackage5Length.Text.Contains("*") && !chk_ColorPackage5Length.Checked)
                blnResult = false;

            // Color Defect 5 Area
            if (chk_ColorPackage5Area.Text.Contains("*") && !chk_ColorPackage5Area.Checked)
                blnResult = false;

            return blnResult;
        }
        private bool CheckPackageControlSetting()
        {
            bool blnResult = true;


            if (chk_InspectPackage2.Text.Contains("*") && !chk_InspectPackage2.Checked)
                blnResult = false;

            if (chk_CheckPkgSize2.Text.Contains("*") && !chk_CheckPkgSize2.Checked)
                blnResult = false;

            if (chk_BrightFieldArea.Text.Contains("*") && !chk_BrightFieldArea.Checked || chk_BrightFieldArea.Text.Contains("*") && !chk_BrightFieldArea.Enabled)
                blnResult = false;

            if (chk_BrightFieldLength.Text.Contains("*") && !chk_BrightFieldLength.Checked || chk_BrightFieldLength.Text.Contains("*") && !chk_BrightFieldLength.Enabled)
                blnResult = false;

            if (chk_DarkFieldArea.Text.Contains("*") && !chk_DarkFieldArea.Checked || chk_DarkFieldArea.Text.Contains("*") && !chk_DarkFieldArea.Enabled)
                blnResult = false;

            if (chk_DarkField2Area.Text.Contains("*") && !chk_DarkField2Area.Checked || chk_DarkField2Area.Text.Contains("*") && !chk_DarkField2Area.Enabled)
                blnResult = false;

            if (chk_DarkField3Area.Text.Contains("*") && !chk_DarkField3Area.Checked || chk_DarkField3Area.Text.Contains("*") && !chk_DarkField3Area.Enabled)
                blnResult = false;

            if (chk_DarkField4Area.Text.Contains("*") && !chk_DarkField4Area.Checked || chk_DarkField4Area.Text.Contains("*") && !chk_DarkField4Area.Enabled)
                blnResult = false;

            if (chk_DarkFieldLength.Text.Contains("*") && !chk_DarkFieldLength.Checked || chk_DarkFieldLength.Text.Contains("*") && !chk_DarkFieldLength.Enabled)
                blnResult = false;

            if (chk_CrackDarkFieldLength.Text.Contains("*") && !chk_CrackDarkFieldLength.Checked || chk_CrackDarkFieldLength.Text.Contains("*") && !chk_CrackDarkFieldLength.Enabled)
                blnResult = false;

            if (chk_VoidDarkFieldLength.Text.Contains("*") && !chk_VoidDarkFieldLength.Checked || chk_VoidDarkFieldLength.Text.Contains("*") && !chk_VoidDarkFieldLength.Enabled)
                blnResult = false;

            if (chk_CrackDarkFieldArea.Text.Contains("*") && !chk_CrackDarkFieldArea.Checked || chk_CrackDarkFieldArea.Text.Contains("*") && !chk_CrackDarkFieldArea.Enabled)
                blnResult = false;

            if (chk_VoidDarkFieldArea.Text.Contains("*") && !chk_VoidDarkFieldArea.Checked || chk_VoidDarkFieldArea.Text.Contains("*") && !chk_VoidDarkFieldArea.Enabled)
                blnResult = false;

            if (chk_MoldFlashBrightFieldArea.Text.Contains("*") && !chk_MoldFlashBrightFieldArea.Checked || chk_MoldFlashBrightFieldArea.Text.Contains("*") && !chk_MoldFlashBrightFieldArea.Enabled)
                blnResult = false;

            if (chk_CheckChippedOffBright_Area.Text.Contains("*") && !chk_CheckChippedOffBright_Area.Checked || chk_CheckChippedOffBright_Area.Text.Contains("*") && !chk_CheckChippedOffBright_Area.Enabled)
                blnResult = false;

            if (chk_CheckChippedOffBright_Length.Text.Contains("*") && !chk_CheckChippedOffBright_Length.Checked || chk_CheckChippedOffBright_Length.Text.Contains("*") && !chk_CheckChippedOffBright_Length.Enabled)
                blnResult = false;

            if (chk_CheckChippedOffDark_Area.Text.Contains("*") && !chk_CheckChippedOffDark_Area.Checked || chk_CheckChippedOffDark_Area.Text.Contains("*") && !chk_CheckChippedOffDark_Area.Enabled)
                blnResult = false;

            if (chk_CheckChippedOffDark_Length.Text.Contains("*") && !chk_CheckChippedOffDark_Length.Checked || chk_CheckChippedOffDark_Length.Text.Contains("*") && !chk_CheckChippedOffDark_Length.Enabled)
                blnResult = false;

            return blnResult;
        }
        private bool CheckCenterColorPadControlSetting()
        {
            bool blnResult = true;

            // Center Color Defect 1 Length
            if (chk_ColorPad1Length_Center.Text.Contains("*") && !chk_ColorPad1Length_Center.Checked)
                blnResult = false;

            // Center Color Defect 1 Area
            if (chk_ColorPad1Area_Center.Text.Contains("*") && !chk_ColorPad1Area_Center.Checked)
                blnResult = false;

            // Center Color Defect 2 Length
            if (chk_ColorPad2Length_Center.Text.Contains("*") && !chk_ColorPad2Length_Center.Checked)
                blnResult = false;

            // Center Color Defect 2 Area
            if (chk_ColorPad2Area_Center.Text.Contains("*") && !chk_ColorPad2Area_Center.Checked)
                blnResult = false;

            // Center Color Defect 3 Length
            if (chk_ColorPad3Length_Center.Text.Contains("*") && !chk_ColorPad3Length_Center.Checked)
                blnResult = false;

            // Center Color Defect 3 Area
            if (chk_ColorPad3Area_Center.Text.Contains("*") && !chk_ColorPad3Area_Center.Checked)
                blnResult = false;

            // Center Color Defect 4 Length
            if (chk_ColorPad4Length_Center.Text.Contains("*") && !chk_ColorPad4Length_Center.Checked)
                blnResult = false;

            // Center Color Defect 4 Area
            if (chk_ColorPad4Area_Center.Text.Contains("*") && !chk_ColorPad4Area_Center.Checked)
                blnResult = false;

            // Center Color Defect 5 Length
            if (chk_ColorPad5Length_Center.Text.Contains("*") && !chk_ColorPad5Length_Center.Checked)
                blnResult = false;

            // Center Color Defect 5 Area
            if (chk_ColorPad5Area_Center.Text.Contains("*") && !chk_ColorPad5Area_Center.Checked)
                blnResult = false;

            return blnResult;
        }
        private bool CheckSideColorPadControlSetting_Top()
        {
            bool blnResult = true;

            // Side Color Defect 1 Length
            if (chk_ColorPad1Length_Side_Top.Text.Contains("*") && !chk_ColorPad1Length_Side_Top.Checked)
                blnResult = false;

            // Side Color Defect 1 Area
            if (chk_ColorPad1Area_Side_Top.Text.Contains("*") && !chk_ColorPad1Area_Side_Top.Checked)
                blnResult = false;

            // Side Color Defect 2 Length
            if (chk_ColorPad2Length_Side_Top.Text.Contains("*") && !chk_ColorPad2Length_Side_Top.Checked)
                blnResult = false;

            // Side Color Defect 2 Area
            if (chk_ColorPad2Area_Side_Top.Text.Contains("*") && !chk_ColorPad2Area_Side_Top.Checked)
                blnResult = false;

            // Side Color Defect 3 Length
            if (chk_ColorPad3Length_Side_Top.Text.Contains("*") && !chk_ColorPad3Length_Side_Top.Checked)
                blnResult = false;

            // Side Color Defect 3 Area
            if (chk_ColorPad3Area_Side_Top.Text.Contains("*") && !chk_ColorPad3Area_Side_Top.Checked)
                blnResult = false;

            // Side Color Defect 4 Length
            if (chk_ColorPad4Length_Side_Top.Text.Contains("*") && !chk_ColorPad4Length_Side_Top.Checked)
                blnResult = false;

            // Side Color Defect 4 Area
            if (chk_ColorPad4Area_Side_Top.Text.Contains("*") && !chk_ColorPad4Area_Side_Top.Checked)
                blnResult = false;

            // Side Color Defect 5 Length
            if (chk_ColorPad5Length_Side_Top.Text.Contains("*") && !chk_ColorPad5Length_Side_Top.Checked)
                blnResult = false;

            // Side Color Defect 5 Area
            if (chk_ColorPad5Area_Side_Top.Text.Contains("*") && !chk_ColorPad5Area_Side_Top.Checked)
                blnResult = false;

            return blnResult;
        }
        private bool CheckSideColorPadControlSetting_Right()
        {
            bool blnResult = true;

            // Side Color Defect 1 Length
            if (chk_ColorPad1Length_Side_Right.Text.Contains("*") && !chk_ColorPad1Length_Side_Right.Checked)
                blnResult = false;

            // Side Color Defect 1 Area
            if (chk_ColorPad1Area_Side_Right.Text.Contains("*") && !chk_ColorPad1Area_Side_Right.Checked)
                blnResult = false;

            // Side Color Defect 2 Length
            if (chk_ColorPad2Length_Side_Right.Text.Contains("*") && !chk_ColorPad2Length_Side_Right.Checked)
                blnResult = false;

            // Side Color Defect 2 Area
            if (chk_ColorPad2Area_Side_Right.Text.Contains("*") && !chk_ColorPad2Area_Side_Right.Checked)
                blnResult = false;

            // Side Color Defect 3 Length
            if (chk_ColorPad3Length_Side_Right.Text.Contains("*") && !chk_ColorPad3Length_Side_Right.Checked)
                blnResult = false;

            // Side Color Defect 3 Area
            if (chk_ColorPad3Area_Side_Right.Text.Contains("*") && !chk_ColorPad3Area_Side_Right.Checked)
                blnResult = false;

            // Side Color Defect 4 Length
            if (chk_ColorPad4Length_Side_Right.Text.Contains("*") && !chk_ColorPad4Length_Side_Right.Checked)
                blnResult = false;

            // Side Color Defect 4 Area
            if (chk_ColorPad4Area_Side_Right.Text.Contains("*") && !chk_ColorPad4Area_Side_Right.Checked)
                blnResult = false;

            // Side Color Defect 5 Length
            if (chk_ColorPad5Length_Side_Right.Text.Contains("*") && !chk_ColorPad5Length_Side_Right.Checked)
                blnResult = false;

            // Side Color Defect 5 Area
            if (chk_ColorPad5Area_Side_Right.Text.Contains("*") && !chk_ColorPad5Area_Side_Right.Checked)
                blnResult = false;

            return blnResult;
        }
        private bool CheckSideColorPadControlSetting_Bottom()
        {
            bool blnResult = true;

            // Side Color Defect 1 Length
            if (chk_ColorPad1Length_Side_Bottom.Text.Contains("*") && !chk_ColorPad1Length_Side_Bottom.Checked)
                blnResult = false;

            // Side Color Defect 1 Area
            if (chk_ColorPad1Area_Side_Bottom.Text.Contains("*") && !chk_ColorPad1Area_Side_Bottom.Checked)
                blnResult = false;

            // Side Color Defect 2 Length
            if (chk_ColorPad2Length_Side_Bottom.Text.Contains("*") && !chk_ColorPad2Length_Side_Bottom.Checked)
                blnResult = false;

            // Side Color Defect 2 Area
            if (chk_ColorPad2Area_Side_Bottom.Text.Contains("*") && !chk_ColorPad2Area_Side_Bottom.Checked)
                blnResult = false;

            // Side Color Defect 3 Length
            if (chk_ColorPad3Length_Side_Bottom.Text.Contains("*") && !chk_ColorPad3Length_Side_Bottom.Checked)
                blnResult = false;

            // Side Color Defect 3 Area
            if (chk_ColorPad3Area_Side_Bottom.Text.Contains("*") && !chk_ColorPad3Area_Side_Bottom.Checked)
                blnResult = false;

            // Side Color Defect 4 Length
            if (chk_ColorPad4Length_Side_Bottom.Text.Contains("*") && !chk_ColorPad4Length_Side_Bottom.Checked)
                blnResult = false;

            // Side Color Defect 4 Area
            if (chk_ColorPad4Area_Side_Bottom.Text.Contains("*") && !chk_ColorPad4Area_Side_Bottom.Checked)
                blnResult = false;

            // Side Color Defect 5 Length
            if (chk_ColorPad5Length_Side_Bottom.Text.Contains("*") && !chk_ColorPad5Length_Side_Bottom.Checked)
                blnResult = false;

            // Side Color Defect 5 Area
            if (chk_ColorPad5Area_Side_Bottom.Text.Contains("*") && !chk_ColorPad5Area_Side_Bottom.Checked)
                blnResult = false;

            return blnResult;
        }
        private bool CheckSideColorPadControlSetting_Left()
        {
            bool blnResult = true;

            // Side Color Defect 1 Length
            if (chk_ColorPad1Length_Side_Left.Text.Contains("*") && !chk_ColorPad1Length_Side_Left.Checked)
                blnResult = false;

            // Side Color Defect 1 Area
            if (chk_ColorPad1Area_Side_Left.Text.Contains("*") && !chk_ColorPad1Area_Side_Left.Checked)
                blnResult = false;

            // Side Color Defect 2 Length
            if (chk_ColorPad2Length_Side_Left.Text.Contains("*") && !chk_ColorPad2Length_Side_Left.Checked)
                blnResult = false;

            // Side Color Defect 2 Area
            if (chk_ColorPad2Area_Side_Left.Text.Contains("*") && !chk_ColorPad2Area_Side_Left.Checked)
                blnResult = false;

            // Side Color Defect 3 Length
            if (chk_ColorPad3Length_Side_Left.Text.Contains("*") && !chk_ColorPad3Length_Side_Left.Checked)
                blnResult = false;

            // Side Color Defect 3 Area
            if (chk_ColorPad3Area_Side_Left.Text.Contains("*") && !chk_ColorPad3Area_Side_Left.Checked)
                blnResult = false;

            // Side Color Defect 4 Length
            if (chk_ColorPad4Length_Side_Left.Text.Contains("*") && !chk_ColorPad4Length_Side_Left.Checked)
                blnResult = false;

            // Side Color Defect 4 Area
            if (chk_ColorPad4Area_Side_Left.Text.Contains("*") && !chk_ColorPad4Area_Side_Left.Checked)
                blnResult = false;

            // Side Color Defect 5 Length
            if (chk_ColorPad5Length_Side_Left.Text.Contains("*") && !chk_ColorPad5Length_Side_Left.Checked)
                blnResult = false;

            // Side Color Defect 5 Area
            if (chk_ColorPad5Area_Side_Left.Text.Contains("*") && !chk_ColorPad5Area_Side_Left.Checked)
                blnResult = false;

            return blnResult;
        }
        private bool CheckPadControlSetting()
        {
            bool blnResult = true;
            
            // Inspect Pad
            if (chk_InspectPad.Text.Contains("*") && !chk_InspectPad.Checked)
                blnResult = false;

            // Area
            if (chk_Area_Pad.Text.Contains("*") && !chk_Area_Pad.Checked)
                blnResult = false;

            // Width and Height
            if (chk_WidthHeight_Pad.Text.Contains("*") && !chk_WidthHeight_Pad.Checked)
                blnResult = false;

            // Off Set
            if (chk_OffSet_Pad.Text.Contains("*") && !chk_OffSet_Pad.Checked)
                blnResult = false;

            // Broken Pad 
            if (chk_CheckBrokenArea_Pad.Text.Contains("*") && !chk_CheckBrokenArea_Pad.Checked)
                blnResult = false;

            if (chk_CheckBrokenLength_Pad.Text.Contains("*") && !chk_CheckBrokenLength_Pad.Checked)
                blnResult = false;

            // Pitch Gap
            if (chk_Gap_Pad.Text.Contains("*") && !chk_Gap_Pad.Checked)
                blnResult = false;

            if (chk_CheckExcess_Pad.Text.Contains("*") && !chk_CheckExcess_Pad.Checked)
                blnResult = false;

            if (chk_CheckSmear_Pad.Text.Contains("*") && !chk_CheckSmear_Pad.Checked)
                blnResult = false;

            if (chk_CheckEdgeLimit_Pad.Text.Contains("*") && !chk_CheckEdgeLimit_Pad.Checked)
                blnResult = false;

            if (chk_CheckStandOff_Pad.Text.Contains("*") && !chk_CheckStandOff_Pad.Checked)
                blnResult = false;

            //------------------------------------Pin1--------------------------------
            if (m_smVisionInfo.g_blnWantPin1)
            {
                if (chk_WantInspectPin1.Text.Contains("*") && !chk_WantInspectPin1.Checked)
                    blnResult = false;
            }

            if (m_smVisionInfo.g_strVisionName != "Pad5SPkg" && m_smVisionInfo.g_strVisionName != "PadPkg")
            {
                // Foreign Material / Contamination
                if (chk_CheckForeignMaterialArea_Pad.Text.Contains("*") && !chk_CheckForeignMaterialArea_Pad.Checked)
                    blnResult = false;

                if (chk_CheckForeignMaterialTotalArea_Pad.Text.Contains("*") && !chk_CheckForeignMaterialTotalArea_Pad.Checked)
                    blnResult = false;

                if (chk_CheckForeignMaterialLength_Pad.Text.Contains("*") && !chk_CheckForeignMaterialLength_Pad.Checked)
                    blnResult = false;
            }

            return blnResult;
        }

        private bool CheckSidePadControlSetting()
        {
            bool blnResult = true;
            // Area
            if (chk_Area_SidePad.Text.Contains("*") && !chk_Area_SidePad.Checked)
                blnResult = false;

            // Width and Height
            if (chk_WidthHeight_SidePad.Text.Contains("*") && !chk_WidthHeight_SidePad.Checked)
                blnResult = false;

            // Off Set
            if (chk_OffSet_SidePad.Text.Contains("*") && !chk_OffSet_SidePad.Checked)
                blnResult = false;

            // Broken Pad 
            if (chk_CheckBrokenArea_SidePad.Text.Contains("*") && !chk_CheckBrokenArea_SidePad.Checked)
                blnResult = false;

            if (chk_CheckBrokenLength_SidePad.Text.Contains("*") && !chk_CheckBrokenLength_SidePad.Checked)
                blnResult = false;

            // Pitch Gap
            if (chk_Gap_SidePad.Text.Contains("*") && !chk_Gap_SidePad.Checked)
                blnResult = false;

            if (chk_CheckExcess_SidePad.Text.Contains("*") && !chk_CheckExcess_SidePad.Checked)
                blnResult = false;

            if (chk_CheckSmear_SidePad.Text.Contains("*") && !chk_CheckSmear_SidePad.Checked)
                blnResult = false;

            if (chk_CheckEdgeLimit_SidePad.Text.Contains("*") && !chk_CheckEdgeLimit_SidePad.Checked)
                blnResult = false;

            if (chk_CheckStandOff_SidePad.Text.Contains("*") && !chk_CheckStandOff_SidePad.Checked)
                blnResult = false;

            if (m_smVisionInfo.g_strVisionName != "Pad5SPkg" && m_smVisionInfo.g_strVisionName != "PadPkg")
            {
                // Foreign Material / Contamination
                if (chk_CheckForeignMaterialArea_SidePad.Text.Contains("*") && !chk_CheckForeignMaterialArea_SidePad.Checked)
                    blnResult = false;

                if (chk_CheckForeignMaterialTotalArea_SidePad.Text.Contains("*") && !chk_CheckForeignMaterialTotalArea_SidePad.Checked)
                    blnResult = false;

                if (chk_CheckForeignMaterialLength_SidePad.Text.Contains("*") && !chk_CheckForeignMaterialLength_SidePad.Checked)
                    blnResult = false;
            }

            return blnResult;
        }

        private bool CheckCenterPackagePadControlSetting()
        {
            bool blnResult = true;

            if (chk_InspectPackage_Pad2.Text.Contains("*") && !chk_InspectPackage_Pad2.Checked)
                blnResult = false;

            if (chk_CheckPkgSize_Pad2.Text.Contains("*") && !chk_CheckPkgSize_Pad2.Checked)
                blnResult = false;

            if (chk_BrightFieldArea_Pad.Text.Contains("*") && !chk_BrightFieldArea_Pad.Checked || chk_BrightFieldArea_Pad.Text.Contains("*") && !chk_BrightFieldArea_Pad.Enabled)
                blnResult = false;

            if (chk_BrightFieldLength_Pad.Text.Contains("*") && !chk_BrightFieldLength_Pad.Checked || chk_BrightFieldLength_Pad.Text.Contains("*") && !chk_BrightFieldLength_Pad.Enabled)
                blnResult = false;

            if (chk_DarkFieldArea_Pad.Text.Contains("*") && !chk_DarkFieldArea_Pad.Checked || chk_DarkFieldArea_Pad.Text.Contains("*") && !chk_DarkFieldArea_Pad.Enabled)
                blnResult = false;

            if (chk_DarkFieldLength_Pad.Text.Contains("*") && !chk_DarkFieldLength_Pad.Checked || chk_DarkFieldLength_Pad.Text.Contains("*") && !chk_DarkFieldLength_Pad.Enabled)
                blnResult = false;

            if (chk_CrackDarkFieldArea_Pad.Text.Contains("*") && !chk_CrackDarkFieldArea_Pad.Checked || chk_CrackDarkFieldArea_Pad.Text.Contains("*") && !chk_CrackDarkFieldArea_Pad.Enabled)
                blnResult = false;

            if (chk_CrackDarkFieldLength_Pad.Text.Contains("*") && !chk_CrackDarkFieldLength_Pad.Checked || chk_CrackDarkFieldLength_Pad.Text.Contains("*") && !chk_CrackDarkFieldLength_Pad.Enabled)
                blnResult = false;

            if (chk_ChippedOffDarkFieldArea_Pad.Text.Contains("*") && !chk_ChippedOffDarkFieldArea_Pad.Checked || chk_ChippedOffDarkFieldArea_Pad.Text.Contains("*") && !chk_ChippedOffDarkFieldArea_Pad.Enabled)
                blnResult = false;

            if (chk_ChippedOffBrightFieldArea_Pad.Text.Contains("*") && !chk_ChippedOffBrightFieldArea_Pad.Checked || chk_ChippedOffBrightFieldArea_Pad.Text.Contains("*") && !chk_ChippedOffBrightFieldArea_Pad.Enabled)
                blnResult = false;

            if (chk_MoldFlashBrightFieldArea_Pad.Text.Contains("*") && !chk_MoldFlashBrightFieldArea_Pad.Checked || chk_MoldFlashBrightFieldArea_Pad.Text.Contains("*") && !chk_MoldFlashBrightFieldArea_Pad.Enabled)
                blnResult = false;

            if (chk_MoldFlashBrightFieldLength_Pad.Text.Contains("*") && !chk_MoldFlashBrightFieldLength_Pad.Checked || chk_MoldFlashBrightFieldLength_Pad.Text.Contains("*") && !chk_MoldFlashBrightFieldLength_Pad.Enabled)
                blnResult = false;

            if (chk_ForeignMaterialBrightFieldArea_Pad.Text.Contains("*") && !chk_ForeignMaterialBrightFieldArea_Pad.Checked || chk_ForeignMaterialBrightFieldArea_Pad.Text.Contains("*") && !chk_ForeignMaterialBrightFieldArea_Pad.Enabled)
                blnResult = false;

            if (chk_ForeignMaterialBrightFieldLength_Pad.Text.Contains("*") && !chk_ForeignMaterialBrightFieldLength_Pad.Checked || chk_ForeignMaterialBrightFieldLength_Pad.Text.Contains("*") && !chk_ForeignMaterialBrightFieldLength_Pad.Enabled)
                blnResult = false;

            return blnResult;
        }

        private bool CheckSidePackagePadControlSetting()
        {
            bool blnResult = true;


            if (chk_InspectPackage_SidePad2.Text.Contains("*") && !chk_InspectPackage_SidePad2.Checked)
                blnResult = false;

            if (chk_CheckPkgSize_SidePad2.Text.Contains("*") && !chk_CheckPkgSize_SidePad2.Checked)
                blnResult = false;

            if (chk_BrightFieldArea_SidePad.Text.Contains("*") && !chk_BrightFieldArea_SidePad.Checked || chk_BrightFieldArea_SidePad.Text.Contains("*") && !chk_BrightFieldArea_SidePad.Enabled)
                blnResult = false;

            if (chk_BrightFieldLength_SidePad.Text.Contains("*") && !chk_BrightFieldLength_SidePad.Checked || chk_BrightFieldLength_SidePad.Text.Contains("*") && !chk_BrightFieldLength_SidePad.Enabled)
                blnResult = false;

            if (chk_DarkFieldArea_SidePad.Text.Contains("*") && !chk_DarkFieldArea_SidePad.Checked || chk_DarkFieldArea_SidePad.Text.Contains("*") && !chk_DarkFieldArea_SidePad.Enabled)
                blnResult = false;

            if (chk_DarkFieldLength_SidePad.Text.Contains("*") && !chk_DarkFieldLength_SidePad.Checked || chk_DarkFieldLength_SidePad.Text.Contains("*") && !chk_DarkFieldLength_SidePad.Enabled)
                blnResult = false;

            if (chk_CrackDarkFieldArea_SidePad.Text.Contains("*") && !chk_CrackDarkFieldArea_SidePad.Checked || chk_CrackDarkFieldArea_SidePad.Text.Contains("*") && !chk_CrackDarkFieldArea_SidePad.Enabled)
                blnResult = false;

            if (chk_CrackDarkFieldLength_SidePad.Text.Contains("*") && !chk_CrackDarkFieldLength_SidePad.Checked || chk_CrackDarkFieldLength_SidePad.Text.Contains("*") && !chk_CrackDarkFieldLength_SidePad.Enabled)
                blnResult = false;

            if (chk_ChippedOffDarkFieldArea_SidePad.Text.Contains("*") && !chk_ChippedOffDarkFieldArea_SidePad.Checked || chk_ChippedOffDarkFieldArea_SidePad.Text.Contains("*") && !chk_ChippedOffDarkFieldArea_SidePad.Enabled)
                blnResult = false;

            if (chk_ChippedOffBrightFieldArea_SidePad.Text.Contains("*") && !chk_ChippedOffBrightFieldArea_SidePad.Checked || chk_ChippedOffBrightFieldArea_SidePad.Text.Contains("*") && !chk_ChippedOffBrightFieldArea_SidePad.Enabled)
                blnResult = false;

            if (chk_MoldFlashBrightFieldArea_SidePad.Text.Contains("*") && !chk_MoldFlashBrightFieldArea_SidePad.Checked || chk_MoldFlashBrightFieldArea_SidePad.Text.Contains("*") && !chk_MoldFlashBrightFieldArea_SidePad.Enabled)
                blnResult = false;

            if (chk_MoldFlashBrightFieldLength_SidePad.Text.Contains("*") && !chk_MoldFlashBrightFieldLength_SidePad.Checked || chk_MoldFlashBrightFieldLength_SidePad.Text.Contains("*") && !chk_MoldFlashBrightFieldLength_SidePad.Enabled)
                blnResult = false;

            if (chk_ForeignMaterialBrightFieldArea_SidePad.Text.Contains("*") && !chk_ForeignMaterialBrightFieldArea_SidePad.Checked || chk_ForeignMaterialBrightFieldArea_SidePad.Text.Contains("*") && !chk_ForeignMaterialBrightFieldArea_SidePad.Enabled)
                blnResult = false;

            if (chk_ForeignMaterialBrightFieldLength_SidePad.Text.Contains("*") && !chk_ForeignMaterialBrightFieldLength_SidePad.Checked || chk_ForeignMaterialBrightFieldLength_SidePad.Text.Contains("*") && !chk_ForeignMaterialBrightFieldLength_SidePad.Enabled)
                blnResult = false;

            return blnResult;
        }

        private bool CheckLead3DControlSetting()
        {
            bool blnResult = true;

            // Offset 
            if (chk_Offset_Lead3D.Text.Contains("*") && !chk_Offset_Lead3D.Checked)
                blnResult = false;

            // Skew 
            if (chk_Skew_Lead3D.Text.Contains("*") && !chk_Skew_Lead3D.Checked)
                blnResult = false;

            // Width 
            if (chk_Width_Lead3D.Text.Contains("*") && !chk_Width_Lead3D.Checked)
                blnResult = false;

            // Length
            if (chk_Length_Lead3D.Text.Contains("*") && !chk_Length_Lead3D.Checked)
                blnResult = false;

            // Length Variance 
            if (chk_LengthVariance_Lead3D.Text.Contains("*") && !chk_LengthVariance_Lead3D.Checked)
                blnResult = false;

            // Pitch
            if (chk_PitchGap_Lead3D.Text.Contains("*") && !chk_PitchGap_Lead3D.Checked)
                blnResult = false;

            // Pitch Variance
            if (chk_PitchVariance_Lead3D.Text.Contains("*") && !chk_PitchVariance_Lead3D.Checked)
                blnResult = false;

            // Stand Off 
            if (chk_StandOff_Lead3D.Text.Contains("*") && !chk_StandOff_Lead3D.Checked)
                blnResult = false;

            // Stand Off Variance 
            if (chk_StandoffVariance_Lead3D.Text.Contains("*") && !chk_StandoffVariance_Lead3D.Checked)
                blnResult = false;

            // Coplan 
            if (chk_Coplan_Lead3D.Text.Contains("*") && !chk_Coplan_Lead3D.Checked)
                blnResult = false;

            // Span
            if (chk_Span_Lead3D.Text.Contains("*") && !chk_Span_Lead3D.Checked)
                blnResult = false;

            // Lead Sweeps
            if (chk_LeadSweeps_Lead3D.Text.Contains("*") && !chk_LeadSweeps_Lead3D.Checked)
                blnResult = false;

            // Solder Pad Length  
            if (chk_SolderPadLength_Lead3D.Text.Contains("*") && !chk_SolderPadLength_Lead3D.Checked)
                blnResult = false;

            // Un-Cut Tiebar
            if (chk_UnCutTiebar_Lead3D.Text.Contains("*") && !chk_UnCutTiebar_Lead3D.Checked)
                blnResult = false;

            // Foreign Material / Contamination
            if (!chk_CheckForeignMaterialArea_Lead3D.Text.Contains("*") && !chk_CheckForeignMaterialArea_Lead3D.Checked)
                blnResult = false;

            if (!chk_CheckForeignMaterialLength_Lead3D.Text.Contains("*") && !chk_CheckForeignMaterialLength_Lead3D.Checked)
                blnResult = false;

            if (!chk_CheckForeignMaterialTotalArea_Lead3D.Text.Contains("*") && !chk_CheckForeignMaterialTotalArea_Lead3D.Checked)
                blnResult = false;
            
            // Average Gray Value
            if (chk_AverageGrayValue_Lead3D.Text.Contains("*") && !chk_AverageGrayValue_Lead3D.Checked && m_smVisionInfo.g_arrLead3D[0].ref_blnWantUseAverageGrayValueMethod)
                blnResult = false;

            // Lead Min and Max Width
            if (chk_LeadMinAndMaxWidth_Lead3D.Text.Contains("*") && !chk_LeadMinAndMaxWidth_Lead3D.Checked)
                blnResult = false;

            // Lead Burr Width
            if (chk_LeadBurrWidth.Text.Contains("*") && !chk_LeadBurrWidth.Checked)
                blnResult = false;

            //------------------------------------Pin1--------------------------------
            if (m_smVisionInfo.g_blnWantPin1)
            {
                if (chk_WantInspectPin1.Text.Contains("*") && !chk_WantInspectPin1.Checked)
                    blnResult = false;
            }

            return blnResult;
        }

        private bool CheckLead3DPkgControlSetting()
        {
            bool blnResult = true;

            // Use simple defect criteria

            if (chk_InspectPackage_Lead3D.Text.Contains("*") && !chk_InspectPackage_Lead3D.Checked)
                blnResult = false;

            if (chk_CheckPkgSize_Lead3D.Text.Contains("*") && !chk_CheckPkgSize_Lead3D.Checked)
                blnResult = false;

            if (chk_BrightFieldArea_Lead3D.Text.Contains("*") && !chk_BrightFieldArea_Lead3D.Checked || chk_BrightFieldArea_Lead3D.Text.Contains("*") && !chk_BrightFieldArea_Lead3D.Enabled)
                blnResult = false;

            if (chk_BrightFieldLength_Lead3D.Text.Contains("*") && !chk_BrightFieldLength_Lead3D.Checked || chk_BrightFieldLength_Lead3D.Text.Contains("*") && !chk_BrightFieldLength_Lead3D.Enabled)
                blnResult = false;

            if (chk_DarkFieldArea_Lead3D.Text.Contains("*") && !chk_DarkFieldArea_Lead3D.Checked || chk_DarkFieldArea_Lead3D.Text.Contains("*") && !chk_DarkFieldArea_Lead3D.Enabled)
                blnResult = false;

            if (chk_DarkFieldLength_Lead3D.Text.Contains("*") && !chk_DarkFieldLength_Lead3D.Checked || chk_DarkFieldLength_Lead3D.Text.Contains("*") && !chk_DarkFieldLength_Lead3D.Enabled)
                blnResult = false;

            if (chk_CrackDarkFieldArea_Lead3D.Text.Contains("*") && !chk_CrackDarkFieldArea_Lead3D.Checked || chk_CrackDarkFieldArea_Lead3D.Text.Contains("*") && !chk_CrackDarkFieldArea_Lead3D.Enabled)
                blnResult = false;

            if (chk_CrackDarkFieldLength_Lead3D.Text.Contains("*") && !chk_CrackDarkFieldLength_Lead3D.Checked || chk_CrackDarkFieldLength_Lead3D.Text.Contains("*") && !chk_CrackDarkFieldLength_Lead3D.Enabled)
                blnResult = false;

            if (chk_ChippedOffDarkFieldArea_Lead3D.Text.Contains("*") && !chk_ChippedOffDarkFieldArea_Lead3D.Checked || chk_ChippedOffDarkFieldArea_Lead3D.Text.Contains("*") && !chk_ChippedOffDarkFieldArea_Lead3D.Enabled)
                blnResult = false;

            if (chk_ChippedOffBrightFieldArea_Lead3D.Text.Contains("*") && !chk_ChippedOffBrightFieldArea_Lead3D.Checked || chk_ChippedOffBrightFieldArea_Lead3D.Text.Contains("*") && !chk_ChippedOffBrightFieldArea_Lead3D.Enabled)
                blnResult = false;

            if (chk_MoldFlashBrightFieldArea_Lead3D.Text.Contains("*") && !chk_MoldFlashBrightFieldArea_Lead3D.Checked || chk_MoldFlashBrightFieldArea_Lead3D.Text.Contains("*") && !chk_MoldFlashBrightFieldArea_Lead3D.Enabled)
                blnResult = false;


            return blnResult;
        }

        private void chk_FailMask_Click(object sender, EventArgs e)
        {
            SetPadFailMask(sender);
        }

        private void chk_SidepadFailMask_Click(object sender, EventArgs e)
        {
            SetSidePadFailMask(sender);
        }

        private void tp_Package_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            //if (!CheckControlSetting())
            //{
            //    SRMMessageBox.Show("Please make sure all important setting (*) are selected!", "Inspection Option", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            if (m_smVisionInfo.g_objSeal != null)
                STTrackLog.WriteLine("btn_Save_Click > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());

            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\";

            STTrackLog.WriteLine(m_smVisionInfo.g_strVisionFolderName + " Save Path = " + strPath);

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrient":
                case "BottomPosition":
                    if ((m_smCustomizeInfo.g_intWantBottom & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        SaveOrientSettings(strPath + "Orient\\");
                        SaveControlSettings(strPath + "Orient\\");
                        CheckChanges_Orient();
                    }

                    if ((m_intVisionType & 0x08) > 0)
                    {
                        //STDeviceEdit.CopySettingFile(strPath + "Package\\", "\\Settings.xml");
                        SavePackageSettings(strPath + "Package\\");
                        SaveControlSettings(strPath + "Package\\Template\\");
                        CheckChanges_Package();
                        //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Package Option", strPath + "Package\\", "\\Settings.xml", m_smProductionInfo.g_strLotID);
                    }
                    break;
                case "Mark":
                case "MarkOrient":
                case "InPocket":
                case "IPMLi":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                case "MOLi":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLiPkg":
                    //STDeviceEdit.CopySettingFile(strPath + "Mark\\Template\\", "Template.xml");
                    SaveOrientSettings(strPath + "Orient\\");
                    SaveControlSettings(strPath + "Orient\\");

                    SaveMarkSettings(strPath + "Mark\\Template\\");
                    SaveControlSettings(strPath + "Mark\\Template\\");
                    CheckChanges_Mark();
                    //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Mark Option", strPath + "Mark\\Template\\", "Template.xml", m_smProductionInfo.g_strLotID);
                    if (m_smVisionInfo.g_arrPin1 != null)
                    {
                        //STDeviceEdit.CopySettingFile(strPath + "Orient\\Template\\", "Pin1Template.xml");
                        m_smVisionInfo.g_arrPin1[0].SavePin1Setting(strPath + "Orient\\Template\\");
                        CheckChanges_Pin1();
                        //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pin1 Option", strPath + "Orient\\Template\\", "Pin1Template.xml", m_smProductionInfo.g_strLotID);
                    }

                    if ((m_intVisionType & 0x08) > 0)
                    {
                        //STDeviceEdit.CopySettingFile(strPath + "Package\\", "\\Settings.xml");
                        SavePackageSettings(strPath + "Package\\");
                        SaveControlSettings(strPath + "Package\\Template\\");
                        CheckChanges_Package();
                        //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Package Option", strPath + "Package\\", "\\Settings.xml", m_smProductionInfo.g_strLotID);
                    }
                    if ((m_intVisionType & 0x10) > 0)
                    {
                        //STDeviceEdit.CopySettingFile(strPath + "Lead\\", "Template\\Template.xml");
                        SaveLeadSettings(strPath + "Lead\\");
                        SaveControlSettings(strPath + "Lead\\Template\\");
                        CheckChanges_Lead();
                        //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Lead Option", strPath + "Lead\\", "Template\\Template.xml", m_smProductionInfo.g_strLotID);
                    }

                    if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                    {
                        CheckChanges_ColorPackage();
                    }
                    break;
                case "Package":
                    if ((m_intVisionType & 0x08) > 0)
                    {
                        //STDeviceEdit.CopySettingFile(strPath + "Package\\", "\\Settings.xml");
                        SavePackageSettings(strPath + "Package\\");
                        SaveControlSettings(strPath + "Package\\Template\\");
                        CheckChanges_Package();

                        if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        {
                            CheckChanges_ColorPackage();
                        }
                        //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Package Option", strPath + "Package\\", "\\Settings.xml", m_smProductionInfo.g_strLotID);
                    }
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                    {
                        SaveOrientPadSettings(strPath + "Orient\\");
                        CheckChanges_OrientPad();

                        //STDeviceEdit.CopySettingFile(strPath + "Pad\\Template\\", "Template.xml");
                        SavePadSetting(strPath + "Pad\\Template\\");
                        SaveControlSettings(strPath + "Pad\\Template\\");
                        CheckChanges_CenterPad();
                        if (m_smVisionInfo.g_arrPad.Length > 1)
                            CheckChanges_SidePad();
                        //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Option", strPath + "Pad\\Template\\", "Template.xml", m_smProductionInfo.g_strLotID);

                        if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        {
                            CheckChanges_CenterPadPackage();
                            if (m_smVisionInfo.g_arrPad.Length > 1)
                                CheckChanges_SidePadPackage();
                        }

                        if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        {
                            CheckChanges_CenterColorPad();
                        }
                    }

                    break;
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    {
                        //STDeviceEdit.CopySettingFile(strPath + "Pad\\Template\\", "Template.xml");
                        SavePadSetting(strPath + "Pad\\Template\\");
                        SaveControlSettings(strPath + "Pad\\Template\\");
                        CheckChanges_CenterPad();
                        if(m_smVisionInfo.g_arrPad.Length > 1)
                            CheckChanges_SidePad();
                        //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Pad Option", strPath + "Pad\\Template\\", "Template.xml", m_smProductionInfo.g_strLotID);

                        if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        {
                            CheckChanges_CenterPadPackage();
                            if (m_smVisionInfo.g_arrPad.Length > 1)
                                CheckChanges_SidePadPackage();
                        }

                        if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        {
                            CheckChanges_CenterColorPad();

                            if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
                            {
                                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                {
                                    if (i == 1)
                                        CheckChanges_SideColorPad_Top();

                                    if (i == 2)
                                    {
                                        if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0))
                                            continue;
                                        else
                                            CheckChanges_SideColorPad_Right();
                                    }

                                    if (i == 3)
                                    {
                                        if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x04) > 0))
                                            continue;
                                        else
                                            CheckChanges_SideColorPad_Bottom();
                                    }

                                    if (i == 4)
                                    {
                                        if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x04) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x08) > 0))
                                            continue;
                                        else
                                            CheckChanges_SideColorPad_Left();
                                    }
                                }
                            }
                        }
                    }

                    break;
                case "Li3D":
                case "Li3DPkg":
                    {
                        //STDeviceEdit.CopySettingFile(strPath + "Lead3D\\Template\\", "Template.xml");
                        SaveLead3DSetting(strPath + "Lead3D\\Template\\");
                        SaveControlSettings(strPath + "Lead3D\\Template\\");
                        CheckChanges_Lead3D();
                        //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Lead3D Option", strPath + "Lead3D\\Template\\", "Template.xml", m_smProductionInfo.g_strLotID);

                        if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                        {
                            CheckChanges_Lead3DPackage();
                        }
                    }
                    break;
                case "Seal":
                    {
                        if (m_smVisionInfo.g_objSeal != null)
                            STTrackLog.WriteLine("btn_Save_Click a1 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());

                        //STDeviceEdit.CopySettingFile(strPath + "Seal\\", "Settings.xml");
                        SaveSealSetting(strPath + "Seal\\");
                        SaveControlSettings(strPath + "Seal\\");
                        CheckChanges_Seal();
                        //STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + "-Seal Option", strPath + "Seal\\", "Settings.xml", m_smProductionInfo.g_strLotID);

                        if (m_smVisionInfo.g_objSeal != null)
                            STTrackLog.WriteLine("btn_Save_Click a2 > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());
                    }
                    break;
                case "UnitPresent":
                    break;
            }

            if (m_smVisionInfo.g_objSeal != null)
                STTrackLog.WriteLine("btn_Save_Click b > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            m_smVisionInfo.g_blnUpdateSECSGEMFile = true;

            if (m_smVisionInfo.g_objSeal != null)
                STTrackLog.WriteLine("btn_Save_Click c > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());

            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_objSeal != null)
                STTrackLog.WriteLine("btn_Cancel_Click > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());

            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            switch (m_smVisionInfo.g_strVisionName)
            {
                case "BottomOrient":
                case "BottomPosition":
                    if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
                        LoadOrientSettings(strFolderPath + "Orient\\");

                    if ((m_intVisionType & 0x08) > 0)
                        LoadPackageSetting(strFolderPath + "Package\\");
                    break;
                case "Mark":
                case "MarkOrient":
                case "InPocket":
                case "IPMLi":
                case "MarkPkg":
                case "MOPkg":
                case "MOLiPkg":
                case "MOLi":
                case "InPocketPkg":
                case "InPocketPkgPos":
                case "IPMLiPkg":
                    LoadOrientSettings(strFolderPath + "Orient\\");
                    LoadMarkSettings(strFolderPath + "Mark\\Template\\");

                    if (m_smVisionInfo.g_arrPin1 != null)
                    {
                        m_smVisionInfo.g_arrPin1[0].LoadTemplate(strFolderPath + "Orient\\Template\\");
                    }

                    if ((m_intVisionType & 0x08) > 0)
                        LoadPackageSetting(strFolderPath + "Package\\");

                    if ((m_intVisionType & 0x10) > 0)
                        LoadLeadSetting(strFolderPath + "Lead\\");
                    break;
                case "Package":
                    if ((m_intVisionType & 0x08) > 0)
                        LoadPackageSetting(strFolderPath + "Package\\");
                    break;
                case "BottomOrientPad":
                case "BottomOPadPkg":
                    LoadOrientPadSettings(strFolderPath + "Orient\\");
                    LoadPadSetting(strFolderPath + "Pad\\Template\\");
                    break;
                case "Pad":
                case "PadPos":
                case "PadPkg":
                case "PadPkgPos":
                case "Pad5S":
                case "Pad5SPos":
                case "Pad5SPkg":
                case "Pad5SPkgPos":
                    LoadPadSetting(strFolderPath + "Pad\\Template\\");
                    break;
                case "Li3D":
                case "Li3DPkg":
                    LoadLead3DSetting(strFolderPath + "Lead3D\\");
                    break;
                case "Seal":
                    LoadSealSetting(strFolderPath + "Seal\\");
                    break;
                case "UnitPresent":
                    break;
            }

            m_smVisionInfo.g_intOptionControlMask = m_intOptionControlMaskPrev;
            m_smVisionInfo.g_intPkgOptionControlMask = m_intPkgOptionControlMaskPrev;
            m_smVisionInfo.g_intPkgOptionControlMask2 = m_intPkgOptionControlMask2Prev;
            m_smVisionInfo.g_intLeadOptionControlMask = m_intLeadOptionControlMaskPrev;

            if (m_smVisionInfo.g_objSeal != null)
                STTrackLog.WriteLine("btn_Cancel_Click > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());

            this.Close();
            this.Dispose();
        }

        private void InspectionOptionForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
        }

        private void InspectionOptionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Inspection Option Form Closed", "Exit Inspection Option Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        private void tp_Mark_Click(object sender, EventArgs e)
        {

        }

        private void tp_Orient_Click(object sender, EventArgs e)
        {

        }

        private void chk_WantInspectPin1_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrPin1 != null)
            {
                // Load Pin 1
                for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
                {
                    m_smVisionInfo.g_arrPin1[u].setWantCheckPin1(m_smVisionInfo.g_intSelectedTemplate, chk_WantInspectPin1.Checked, m_blnWantSet1ToAll);
                }
            }
        }

        private void EnableMarkFailOptions(bool bln_chkMainCheckMain)
        {
            // Disable all Fail Option if main inspection option is Off and vice versa
            chk_ExtraMarkUncheckArea.Enabled = m_blnMustDisableExtraMarkSide ? false : bln_chkMainCheckMain;
            chk_ExtraMark.Enabled = bln_chkMainCheckMain;
            chk_ExcessMarkCharArea.Enabled = bln_chkMainCheckMain;
            chk_GroupExcessMark.Enabled = bln_chkMainCheckMain;
            chk_MarkAverageGrayValue.Enabled = bln_chkMainCheckMain;
            chk_GroupExtraMark.Enabled = bln_chkMainCheckMain;
            chk_MissingMark.Enabled = bln_chkMainCheckMain;
            chk_BrokenMark.Enabled = bln_chkMainCheckMain;
            chk_TextShifted.Enabled = bln_chkMainCheckMain;
            chk_JointMark.Enabled = bln_chkMainCheckMain;
            chk_MarkAngle.Enabled = bln_chkMainCheckMain;
        }

        private void chk_MarkFailMask_Click(object sender, EventArgs e)
        {
            EnableMarkFailOptions(chk_CheckMark.Checked);

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_blnCheckMark = chk_CheckMark.Checked;

                if (sender == chk_ExtraMarkUncheckArea)
                {
                    if (chk_ExtraMarkUncheckArea.Checked)
                        m_smVisionInfo.g_arrMarks[u].AddLogicFailOptionMask(0x04, m_blnWantSet1ToAll);
                    else
                        m_smVisionInfo.g_arrMarks[u].RemoveLogicFailOptionMask(0x04, m_blnWantSet1ToAll);
                }
                else if (sender == chk_ExtraMark)
                {
                    if (chk_ExtraMark.Checked)
                        m_smVisionInfo.g_arrMarks[u].AddLogicFailOptionMask(0x02, m_blnWantSet1ToAll);
                    else
                        m_smVisionInfo.g_arrMarks[u].RemoveLogicFailOptionMask(0x02, m_blnWantSet1ToAll);
                }
                else if (sender == chk_ExcessMarkCharArea)
                {
                    if (chk_ExcessMarkCharArea.Checked)
                        m_smVisionInfo.g_arrMarks[u].AddLogicFailOptionMask(0x01, m_blnWantSet1ToAll);
                    else
                        m_smVisionInfo.g_arrMarks[u].RemoveLogicFailOptionMask(0x01, m_blnWantSet1ToAll);
                }
                else if (sender == chk_GroupExcessMark)
                {
                    if (chk_GroupExcessMark.Checked)
                        m_smVisionInfo.g_arrMarks[u].AddLogicFailOptionMask(0x100, m_blnWantSet1ToAll);
                    else
                        m_smVisionInfo.g_arrMarks[u].RemoveLogicFailOptionMask(0x100, m_blnWantSet1ToAll);
                }
                else if (sender == chk_MarkAverageGrayValue)
                {
                    if (chk_MarkAverageGrayValue.Checked)
                        m_smVisionInfo.g_arrMarks[u].AddLogicFailOptionMask(0x200, m_blnWantSet1ToAll);
                    else
                        m_smVisionInfo.g_arrMarks[u].RemoveLogicFailOptionMask(0x200, m_blnWantSet1ToAll);
                }
                else if (sender == chk_GroupExtraMark)
                {
                    if (chk_GroupExtraMark.Checked)
                        m_smVisionInfo.g_arrMarks[u].AddLogicFailOptionMask(0x08, m_blnWantSet1ToAll);
                    else
                        m_smVisionInfo.g_arrMarks[u].RemoveLogicFailOptionMask(0x08, m_blnWantSet1ToAll);
                }
                else if (sender == chk_MissingMark)
                {
                    if (chk_MissingMark.Checked)
                        m_smVisionInfo.g_arrMarks[u].AddLogicFailOptionMask(0x10, m_blnWantSet1ToAll);
                    else
                        m_smVisionInfo.g_arrMarks[u].RemoveLogicFailOptionMask(0x10, m_blnWantSet1ToAll);
                }
                else if (sender == chk_BrokenMark)
                {
                    if (chk_BrokenMark.Checked)
                        m_smVisionInfo.g_arrMarks[u].AddLogicFailOptionMask(0x20, m_blnWantSet1ToAll);
                    else
                        m_smVisionInfo.g_arrMarks[u].RemoveLogicFailOptionMask(0x20, m_blnWantSet1ToAll);
                }
                else if (sender == chk_TextShifted)
                {
                    if (chk_TextShifted.Checked)
                        m_smVisionInfo.g_arrMarks[u].AddLogicFailOptionMask(0x40, m_blnWantSet1ToAll);
                    else
                        m_smVisionInfo.g_arrMarks[u].RemoveLogicFailOptionMask(0x40, m_blnWantSet1ToAll);
                }
                else if (sender == chk_JointMark)
                {
                    if (chk_JointMark.Checked)
                        m_smVisionInfo.g_arrMarks[u].AddLogicFailOptionMask(0x80, m_blnWantSet1ToAll);
                    else
                        m_smVisionInfo.g_arrMarks[u].RemoveLogicFailOptionMask(0x80, m_blnWantSet1ToAll);
                }
                else if (sender == chk_MarkAngle)
                {
                    if (chk_MarkAngle.Checked)
                        m_smVisionInfo.g_arrMarks[u].AddLogicFailOptionMask(0x2000, m_blnWantSet1ToAll);
                    else
                        m_smVisionInfo.g_arrMarks[u].RemoveLogicFailOptionMask(0x2000, m_blnWantSet1ToAll);
                }
            }
        }

        private void cbo_TemplateNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            for (int u = 0; u < m_smVisionInfo.g_intUnitsOnImage; u++)
            {
                m_smVisionInfo.g_arrMarks[u].ref_intTemplateIndex = m_smVisionInfo.g_intSelectedTemplate = cbo_TemplateNo.SelectedIndex;
            }

            //UpdateSelectedTemplateChange();
            UpdateMarkFailMaskGUI();

            if ((m_smCustomizeInfo.g_intWantOrient0Deg & (1 << m_smVisionInfo.g_intVisionPos)) == 0)
            {
                UpdateOrientFailMaskGUI();
            }

            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void chk_SetAllTemplates_Click(object sender, EventArgs e)
        {
            m_blnWantSet1ToAll = chk_SetAllTemplates.Checked;
        }

        private void tab_VisionControl_Selected(object sender, TabControlEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (e.TabPage == tp_Mark)
            {
                m_intSettingType = 0;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_PackageSimple)
            {
                m_intSettingType = 3;
                UpdateTabPage();
            }
            else if (e.TabPage == tp_Pin1)
            {
                m_intSettingType = 2;
                UpdateTabPage();
            }
        }

        private void chk_CenterPkgPadFailMask_Click(object sender, EventArgs e)
        {
            SetCenterPadPackageFailMask_Simple(sender);
        }

        private void chk_SidePkgPadFailMask_Click(object sender, EventArgs e)
        {
            SetSidePadPackageFailMask_Simple(sender);

        }

        private void chk_LeadFailMask_Click(object sender, EventArgs e)
        {
            SetLeadFailMask(sender);
        }

        private void chk_PkgFailMask_Click(object sender, EventArgs e)
        {
            SetPkgFailMask_Simple(sender);
        }

        private void chk_Lead3DFailMask_Click(object sender, EventArgs e)
        {
            SetLead3DFailMask(sender);
        }

        private void chk_Lead3DPkgFailMask_Click(object sender, EventArgs e)
        {
            SetLead3DPkgFailMask_Simple(sender);
        }

        private void chk_SealFailMask_Click(object sender, EventArgs e)
        {
            SetSealFailMask(sender);
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
           
            //if ((m_smVisionInfo.g_blnCheckPackage && ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)))
            //{
            //    if (chk_CheckForeignMaterialArea_Pad.Enabled)
            //        chk_CheckForeignMaterialArea_Pad.Enabled = false;

            //    if (chk_CheckForeignMaterialTotalArea_Pad.Enabled)
            //        chk_CheckForeignMaterialTotalArea_Pad.Enabled = false;

            //    if (chk_CheckForeignMaterialLength_Pad.Enabled)
            //        chk_CheckForeignMaterialLength_Pad.Enabled = false;

            //    if (chk_CheckForeignMaterialArea_SidePad.Enabled)
            //        chk_CheckForeignMaterialArea_SidePad.Enabled = false;

            //    if (chk_CheckForeignMaterialTotalArea_SidePad.Enabled)
            //        chk_CheckForeignMaterialTotalArea_SidePad.Enabled = false;

            //    if (chk_CheckForeignMaterialLength_SidePad.Enabled)
            //        chk_CheckForeignMaterialLength_SidePad.Enabled = false;
            //}
            //else
            //{
            //    if (!chk_CheckForeignMaterialArea_Pad.Enabled)
            //        chk_CheckForeignMaterialArea_Pad.Enabled = true;

            //    if (!chk_CheckForeignMaterialTotalArea_Pad.Enabled)
            //        chk_CheckForeignMaterialTotalArea_Pad.Enabled = true;

            //    if (!chk_CheckForeignMaterialLength_Pad.Enabled)
            //        chk_CheckForeignMaterialLength_Pad.Enabled = true;

            //    if (!chk_CheckForeignMaterialArea_SidePad.Enabled)
            //        chk_CheckForeignMaterialArea_SidePad.Enabled = true;

            //    if (!chk_CheckForeignMaterialTotalArea_SidePad.Enabled)
            //        chk_CheckForeignMaterialTotalArea_SidePad.Enabled = true;

            //    if (!chk_CheckForeignMaterialLength_SidePad.Enabled)
            //        chk_CheckForeignMaterialLength_SidePad.Enabled = true;
            //}
        }
        
        private void btn_ControlSetting_Click(object sender, EventArgs e)
        {
            InspectionOptionControlSettingForm2 objInspectionOptionControlSettingForm = new InspectionOptionControlSettingForm2(m_smCustomizeInfo, m_smVisionInfo, m_smProductionInfo, m_strSelectedRecipe, m_intUserGroup, m_intVisionType);
            objInspectionOptionControlSettingForm.StartPosition = FormStartPosition.Manual;
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            objInspectionOptionControlSettingForm.Location = new Point(resolution.Width - objInspectionOptionControlSettingForm.Size.Width, resolution.Height - objInspectionOptionControlSettingForm.Size.Height);

            if (objInspectionOptionControlSettingForm.ShowDialog() == DialogResult.OK)
            {
                //update masking
                UpdateControlSetting();
            }
        }

        private void chk_WantInspectOrientAngleTolerance_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrOrients.Count > 0)
            {
                for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                {
                    for (int j = 0; j < m_smVisionInfo.g_arrOrients[i].Count; j++)
                        m_smVisionInfo.g_arrOrients[i][j].ref_blnWantCheckOrientAngleTolerance = chk_WantInspectOrientAngleTolerance.Checked;
                }
            }
        }

        private void chk_WantInspectOrientXTolerance_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrOrients.Count > 0)
            {
                for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                {
                    for (int j = 0; j < m_smVisionInfo.g_arrOrients[i].Count; j++)
                        m_smVisionInfo.g_arrOrients[i][j].ref_blnWantCheckOrientXTolerance = chk_WantInspectOrientXTolerance.Checked;

                }
            }
        }

        private void chk_WantInspectOrientYTolerance_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_arrOrients.Count > 0)
            {
                for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
                {
                    for (int j = 0; j < m_smVisionInfo.g_arrOrients[i].Count; j++)
                        m_smVisionInfo.g_arrOrients[i][j].ref_blnWantCheckOrientYTolerance = chk_WantInspectOrientYTolerance.Checked;

                }
            }
        }

        private void SetSealFailMask(object sender)
        {
            int intFailMask = m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal;

            if (m_smVisionInfo.g_objSeal != null)
                STTrackLog.WriteLine("SetSealFailMask > Before ref_intFailOptionMaskSeal=" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());

            if (sender == chk_CheckSealWidth)
            {
                if (chk_CheckSealWidth.Checked)
                    intFailMask |= 0x01;
                else
                    intFailMask &= ~0x01;
            }
            else if (sender == chk_CheckSealBubble)
            {
                if (chk_CheckSealBubble.Checked)
                    intFailMask |= 0x02;
                else
                    intFailMask &= ~0x02;
            }
            else if (sender == chk_CheckSealShifted)
            {
                if (chk_CheckSealShifted.Checked)
                    intFailMask |= 0x04;
                else
                    intFailMask &= ~0x04;
            }
            else if (sender == chk_CheckSealDistance)
            {
                if (chk_CheckSealDistance.Checked)
                    intFailMask |= 0x08;
                else
                    intFailMask &= ~0x08;
            }
            else if (sender == chk_CheckSealOverHeat)
            {
                if (chk_CheckSealOverHeat.Checked)
                    intFailMask |= 0x10;
                else
                    intFailMask &= ~0x10;
            }
            else if (sender == chk_CheckSealBroken)
            {
                if (chk_CheckSealBroken.Checked)
                    intFailMask |= 0x20;
                else
                    intFailMask &= ~0x20;
            }
            else if (sender == chk_CheckSealUnitPresent)
            {
                if (chk_CheckSealUnitPresent.Checked)
                    intFailMask |= 0x40;
                else
                    intFailMask &= ~0x40;
            }
            else if (sender == chk_CheckSealUnitOrient)
            {
                if (chk_CheckSealUnitOrient.Checked)
                    intFailMask |= 0x80;
                else
                    intFailMask &= ~0x80;
            }
            else if (sender == chk_CheckSealSprocketHoleDistance)
            {
                if (chk_CheckSealSprocketHoleDistance.Checked)
                    intFailMask |= 0x100;
                else
                    intFailMask &= ~0x100;
            }
            else if (sender == chk_CheckSealSprocketHoleDiameter)
            {
                if (chk_CheckSealSprocketHoleDiameter.Checked)
                    intFailMask |= 0x200;
                else
                    intFailMask &= ~0x200;
            }
            else if (sender == chk_CheckSealSprocketHoleDefect)
            {
                if (chk_CheckSealSprocketHoleDefect.Checked)
                    intFailMask |= 0x400;
                else
                    intFailMask &= ~0x400;
            }
            else if (sender == chk_CheckSealSprocketHoleBroken)
            {
                if (chk_CheckSealSprocketHoleBroken.Checked)
                    intFailMask |= 0x800;
                else
                    intFailMask &= ~0x800;
            }
            else if (sender == chk_CheckSealSprocketHoleRoundness)
            {
                if (chk_CheckSealSprocketHoleRoundness.Checked)
                    intFailMask |= 0x1000;
                else
                    intFailMask &= ~0x1000;
            }
            else if (sender == chk_CheckSealEdgeStraightness)
            {
                if (chk_CheckSealEdgeStraightness.Checked)
                    intFailMask |= 0x2000;
                else
                    intFailMask &= ~0x2000;
            }

            if (m_smVisionInfo.g_objSeal != null)
                STTrackLog.WriteLine("SetSealFailMask > After ref_intFailOptionMaskSeal=" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());
            m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal = intFailMask;

        }

        private void LoadSealSetting(string strFolderPath)
        {
            if (m_smVisionInfo.g_objSeal != null)
                STTrackLog.WriteLine("LoadSealSetting > Before ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());

            m_smVisionInfo.g_objSeal.LoadSeal(strFolderPath + "Settings.xml", "Settings", m_smVisionInfo.g_fCalibPixelX);

            if (m_smVisionInfo.g_objSeal != null)
                STTrackLog.WriteLine("LoadSealSetting > After ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());
        }
        private void UpdateSealFailMaskGUI()
        {
            m_intFailMaskSeal_Previous = m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal;

            int intFailMask = m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal;

            if (m_smVisionInfo.g_objSeal != null)
                STTrackLog.WriteLine("UpdateSealFailMaskGUI > ref_intFailOptionMaskSeal =" + m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal.ToString());

            chk_CheckSealWidth.Checked = (intFailMask & 0x01) > 0;
            chk_CheckSealBubble.Checked = (intFailMask & 0x02) > 0;
            chk_CheckSealShifted.Checked = (intFailMask & 0x04) > 0;
            chk_CheckSealDistance.Checked = (intFailMask & 0x08) > 0;
            chk_CheckSealOverHeat.Checked = (intFailMask & 0x10) > 0;
            chk_CheckSealBroken.Checked = (intFailMask & 0x20) > 0;
            chk_CheckSealUnitPresent.Checked = (intFailMask & 0x40) > 0;
            chk_CheckSealUnitOrient.Checked = (intFailMask & 0x80) > 0;
            chk_CheckSealSprocketHoleDistance.Checked = (intFailMask & 0x100) > 0;
            chk_CheckSealSprocketHoleDiameter.Checked = (intFailMask & 0x200) > 0;
            chk_CheckSealSprocketHoleDefect.Checked = (intFailMask & 0x400) > 0;
            chk_CheckSealSprocketHoleBroken.Checked = (intFailMask & 0x800) > 0;
            chk_CheckSealSprocketHoleRoundness.Checked = (intFailMask & 0x1000) > 0;
            chk_CheckSealEdgeStraightness.Checked = (intFailMask & 0x2000) > 0;
        }
        private void UpdateSealControlSetting()
        {
            //------------------------------------Seal--------------------------------
            if ((m_smVisionInfo.g_intOptionControlMask & 0x01) > 0)
            {
                if (!chk_CheckSealWidth.Text.Contains("*"))
                    chk_CheckSealWidth.Text += "*";
            }
            else
            {
                if (chk_CheckSealWidth.Text.Contains("*"))
                    chk_CheckSealWidth.Text = chk_CheckSealWidth.Text.Substring(0, chk_CheckSealWidth.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x02) > 0)
            {
                if (!chk_CheckSealBubble.Text.Contains("*"))
                    chk_CheckSealBubble.Text += "*";
            }
            else
            {
                if (chk_CheckSealBubble.Text.Contains("*"))
                    chk_CheckSealBubble.Text = chk_CheckSealBubble.Text.Substring(0, chk_CheckSealBubble.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x04) > 0)
            {
                if (!chk_CheckSealShifted.Text.Contains("*"))
                    chk_CheckSealShifted.Text += "*";
            }
            else
            {
                if (chk_CheckSealShifted.Text.Contains("*"))
                    chk_CheckSealShifted.Text = chk_CheckSealShifted.Text.Substring(0, chk_CheckSealShifted.Text.Length - 1);
            }
            
            if ((m_smVisionInfo.g_intOptionControlMask & 0x08) > 0)
            {
                if (!chk_CheckSealDistance.Text.Contains("*"))
                    chk_CheckSealDistance.Text += "*";
            }
            else
            {
                if (chk_CheckSealDistance.Text.Contains("*"))
                    chk_CheckSealDistance.Text = chk_CheckSealDistance.Text.Substring(0, chk_CheckSealDistance.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x10) > 0)
            {
                if (!chk_CheckSealOverHeat.Text.Contains("*"))
                    chk_CheckSealOverHeat.Text += "*";
            }
            else
            {
                if (chk_CheckSealOverHeat.Text.Contains("*"))
                    chk_CheckSealOverHeat.Text = chk_CheckSealOverHeat.Text.Substring(0, chk_CheckSealOverHeat.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x20) > 0)
            {
                if (!chk_CheckSealBroken.Text.Contains("*"))
                    chk_CheckSealBroken.Text += "*";
            }
            else
            {
                if (chk_CheckSealBroken.Text.Contains("*"))
                    chk_CheckSealBroken.Text = chk_CheckSealBroken.Text.Substring(0, chk_CheckSealBroken.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x40) > 0)
            {
                if (!chk_CheckSealUnitPresent.Text.Contains("*"))
                    chk_CheckSealUnitPresent.Text += "*";
            }
            else
            {
                if (chk_CheckSealUnitPresent.Text.Contains("*"))
                    chk_CheckSealUnitPresent.Text = chk_CheckSealUnitPresent.Text.Substring(0, chk_CheckSealUnitPresent.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x80) > 0)
            {
                if (!chk_CheckSealUnitOrient.Text.Contains("*"))
                    chk_CheckSealUnitOrient.Text += "*";
            }
            else
            {
                if (chk_CheckSealUnitOrient.Text.Contains("*"))
                    chk_CheckSealUnitOrient.Text = chk_CheckSealUnitOrient.Text.Substring(0, chk_CheckSealUnitOrient.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x100) > 0)
            {
                if (!chk_CheckSealSprocketHoleDistance.Text.Contains("*"))
                    chk_CheckSealSprocketHoleDistance.Text += "*";
            }
            else
            {
                if (chk_CheckSealSprocketHoleDistance.Text.Contains("*"))
                    chk_CheckSealSprocketHoleDistance.Text = chk_CheckSealSprocketHoleDistance.Text.Substring(0, chk_CheckSealSprocketHoleDistance.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x200) > 0)
            {
                if (!chk_CheckSealSprocketHoleDiameter.Text.Contains("*"))
                    chk_CheckSealSprocketHoleDiameter.Text += "*";
            }
            else
            {
                if (chk_CheckSealSprocketHoleDiameter.Text.Contains("*"))
                    chk_CheckSealSprocketHoleDiameter.Text = chk_CheckSealSprocketHoleDiameter.Text.Substring(0, chk_CheckSealSprocketHoleDiameter.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x400) > 0)
            {
                if (!chk_CheckSealSprocketHoleDefect.Text.Contains("*"))
                    chk_CheckSealSprocketHoleDefect.Text += "*";
            }
            else
            {
                if (chk_CheckSealSprocketHoleDefect.Text.Contains("*"))
                    chk_CheckSealSprocketHoleDefect.Text = chk_CheckSealSprocketHoleDefect.Text.Substring(0, chk_CheckSealSprocketHoleDefect.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x800) > 0)
            {
                if (!chk_CheckSealSprocketHoleBroken.Text.Contains("*"))
                    chk_CheckSealSprocketHoleBroken.Text += "*";
            }
            else
            {
                if (chk_CheckSealSprocketHoleBroken.Text.Contains("*"))
                    chk_CheckSealSprocketHoleBroken.Text = chk_CheckSealSprocketHoleBroken.Text.Substring(0, chk_CheckSealSprocketHoleBroken.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x1000) > 0)
            {
                if (!chk_CheckSealSprocketHoleRoundness.Text.Contains("*"))
                    chk_CheckSealSprocketHoleRoundness.Text += "*";
            }
            else
            {
                if (chk_CheckSealSprocketHoleRoundness.Text.Contains("*"))
                    chk_CheckSealSprocketHoleRoundness.Text = chk_CheckSealSprocketHoleRoundness.Text.Substring(0, chk_CheckSealSprocketHoleRoundness.Text.Length - 1);
            }

            if ((m_smVisionInfo.g_intOptionControlMask & 0x2000) > 0)
            {
                if (!chk_CheckSealEdgeStraightness.Text.Contains("*"))
                    chk_CheckSealEdgeStraightness.Text += "*";
            }
            else
            {
                if (chk_CheckSealEdgeStraightness.Text.Contains("*"))
                    chk_CheckSealEdgeStraightness.Text = chk_CheckSealEdgeStraightness.Text.Substring(0, chk_CheckSealEdgeStraightness.Text.Length - 1);
            }
        }

        private void CheckChanges_Orient()
        {
            
            if (m_blnFailMaskOrientAngle_Previous != chk_WantInspectOrientAngleTolerance.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Orient", "Inspection Option>" + chk_WantInspectOrientAngleTolerance.Text, (!chk_WantInspectOrientAngleTolerance.Checked).ToString(), chk_WantInspectOrientAngleTolerance.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskOrientPosX_Previous != chk_WantInspectOrientXTolerance.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Orient", "Inspection Option>" + chk_WantInspectOrientXTolerance.Text, (!chk_WantInspectOrientXTolerance.Checked).ToString(), chk_WantInspectOrientXTolerance.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskOrientPosY_Previous != chk_WantInspectOrientYTolerance.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Orient", "Inspection Option>" + chk_WantInspectOrientYTolerance.Text, (!chk_WantInspectOrientYTolerance.Checked).ToString(), chk_WantInspectOrientYTolerance.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
        }

        private void CheckChanges_Orientation()
        {
            
            if (m_blnFailMaskOrientation_Previous != chk_InspectOrient_ForMO.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Orient", "Inspection Option>" + chk_InspectOrient_ForMO.Text, (!chk_InspectOrient_ForMO.Checked).ToString(), chk_InspectOrient_ForMO.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
        }


        private void CheckChanges_OrientPad()
        {
            
            if (m_blnFailMaskOrientAngle_Previous != chk_WantInspectOrientAngleTolerance_Pad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">OrientPad", "Inspection Option>" + chk_WantInspectOrientAngleTolerance_Pad.Text, (!chk_WantInspectOrientAngleTolerance_Pad.Checked).ToString(), chk_WantInspectOrientAngleTolerance_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskOrientPosX_Previous != chk_WantInspectOrientXTolerance_Pad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">OrientPad", "Inspection Option>" + chk_WantInspectOrientXTolerance_Pad.Text, (!chk_WantInspectOrientXTolerance_Pad.Checked).ToString(), chk_WantInspectOrientXTolerance_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskOrientPosY_Previous != chk_WantInspectOrientYTolerance_Pad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">OrientPad", "Inspection Option>" + chk_WantInspectOrientYTolerance_Pad.Text, (!chk_WantInspectOrientYTolerance_Pad.Checked).ToString(), chk_WantInspectOrientYTolerance_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
        }
        private void CheckChanges_Mark()
        {
            //------------------------------------Mark--------------------------------
            
            if (m_blnWantCheckMark_Previous != chk_CheckMark.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Inspection Option>" + chk_CheckMark.Text, (!chk_CheckMark.Checked).ToString(), chk_CheckMark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            int intFailMask = m_smVisionInfo.g_arrMarks[m_smVisionInfo.g_intSelectedUnit].GetFailOptionMask(m_smVisionInfo.g_intSelectedGroup, m_smVisionInfo.g_intSelectedTemplate);
            int intXOR = intFailMask ^ m_intFailMaskMark_Previous;

            //if ((intFailMask & 0x01) > 0 && (m_intVisionType & 0x08) == 0)
            if ((intXOR & 0x01) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Inspection Option>" + chk_ExcessMarkCharArea.Text, (!chk_ExcessMarkCharArea.Checked).ToString(), chk_ExcessMarkCharArea.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            //if ((intFailMask & 0x100) > 0 && (m_intVisionType & 0x08) == 0)
            if ((intXOR & 0x100) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Inspection Option>" + chk_GroupExcessMark.Text, (!chk_GroupExcessMark.Checked).ToString(), chk_GroupExcessMark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            //if ((intFailMask & 0x200) > 0 && (m_intVisionType & 0x08) == 0)
            if ((intXOR & 0x200) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Inspection Option>" + chk_MarkAverageGrayValue.Text, (!chk_MarkAverageGrayValue.Checked).ToString(), chk_MarkAverageGrayValue.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            //if ((intFailMask & 0x02) > 0 && (m_intVisionType & 0x08) == 0)
            if ((intXOR & 0x02) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Inspection Option>" + chk_ExtraMark.Text, (!chk_ExtraMark.Checked).ToString(), chk_ExtraMark.Checked.ToString(), m_smProductionInfo.g_strLotID);   
            }

            //if ((intFailMask & 0x04) > 0 && (m_intVisionType & 0x08) == 0)
            if ((intXOR & 0x04) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Inspection Option>" + chk_ExtraMarkUncheckArea.Text, (!chk_ExtraMarkUncheckArea.Checked).ToString(), chk_ExtraMarkUncheckArea.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            //if ((intFailMask & 0x08) > 0 && (m_intVisionType & 0x08) == 0)
            if ((intXOR & 0x08) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Inspection Option>" + chk_GroupExtraMark.Text, (!chk_GroupExtraMark.Checked).ToString(), chk_GroupExtraMark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x10) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Inspection Option>" + chk_MissingMark.Text, (!chk_MissingMark.Checked).ToString(), chk_MissingMark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Inspection Option>" + chk_BrokenMark.Text, (!chk_BrokenMark.Checked).ToString(), chk_BrokenMark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Inspection Option>" + chk_TextShifted.Text, (!chk_TextShifted.Checked).ToString(), chk_TextShifted.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Inspection Option>" + chk_JointMark.Text, (!chk_JointMark.Checked).ToString(), chk_JointMark.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_smVisionInfo.g_blnWantCheckMarkAngle)
            {
                pnl_MarkAngle.Visible = true;

                if ((intXOR & 0x2000) > 0)
                {
                    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Mark", "Inspection Option>" + chk_MarkAngle.Text, (!chk_MarkAngle.Checked).ToString(), chk_MarkAngle.Checked.ToString(), m_smProductionInfo.g_strLotID);
                }
            }
            
        }
        private void CheckChanges_ColorPackage()
        {

            int intFailMask = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailColorOptionMask;

            int intXOR = intFailMask ^ m_intFailMaskColorPackage_Previous;

            if ((intXOR & 0x01) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Color Package", "Inspection Option>" + chk_ColorPackage1Length.Text, (!chk_ColorPackage1Length.Checked).ToString(), chk_ColorPackage1Length.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x02) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Color Package", "Inspection Option>" + chk_ColorPackage1Area.Text, (!chk_ColorPackage1Area.Checked).ToString(), chk_ColorPackage1Area.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x04) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Color Package", "Inspection Option>" + chk_ColorPackage2Length.Text, (!chk_ColorPackage2Length.Checked).ToString(), chk_ColorPackage2Length.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x08) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Color Package", "Inspection Option>" + chk_ColorPackage2Area.Text, (!chk_ColorPackage2Area.Checked).ToString(), chk_ColorPackage2Area.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }


            if ((intXOR & 0x10) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Color Package", "Inspection Option>" + chk_ColorPackage3Length.Text, (!chk_ColorPackage3Length.Checked).ToString(), chk_ColorPackage3Length.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Color Package", "Inspection Option>" + chk_ColorPackage3Area.Text, (!chk_ColorPackage3Area.Checked).ToString(), chk_ColorPackage3Area.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Color Package", "Inspection Option>" + chk_ColorPackage4Length.Text, (!chk_ColorPackage4Length.Checked).ToString(), chk_ColorPackage4Length.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Color Package", "Inspection Option>" + chk_ColorPackage4Area.Text, (!chk_ColorPackage4Area.Checked).ToString(), chk_ColorPackage4Area.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Color Package", "Inspection Option>" + chk_ColorPackage5Length.Text, (!chk_ColorPackage5Length.Checked).ToString(), chk_ColorPackage5Length.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x200) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Color Package", "Inspection Option>" + chk_ColorPackage5Area.Text, (!chk_ColorPackage5Area.Checked).ToString(), chk_ColorPackage5Area.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

        }
        private void CheckChanges_Package()
        {
            int intFailMask = m_smVisionInfo.g_arrPackage[m_smVisionInfo.g_intSelectedUnit].ref_intFailMask;
            int intXOR = intFailMask ^ m_intFailMaskPackage_Previous;
            
            if ((intXOR & 0x1000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_CheckPkgSize2.Text, (!chk_CheckPkgSize2.Checked).ToString(), chk_CheckPkgSize2.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            if (m_blnFailMaskPackageDefect_Previous != chk_InspectPackage2.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_InspectPackage2.Text, (!chk_InspectPackage2.Checked).ToString(), chk_InspectPackage2.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x2000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_CheckPkgAngle.Text, (!chk_CheckPkgAngle.Checked).ToString(), chk_CheckPkgAngle.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectLength_Previous[0] != chk_BrightFieldLength.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_BrightFieldLength.Text, (!chk_BrightFieldLength.Checked).ToString(), chk_BrightFieldLength.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectLength_Previous[1] != chk_DarkFieldLength.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_DarkFieldLength.Text, (!chk_DarkFieldLength.Checked).ToString(), chk_DarkFieldLength.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectLength_Previous[4] != chk_DarkField2Length.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_DarkField2Length.Text, (!chk_DarkField2Length.Checked).ToString(), chk_DarkField2Length.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectLength_Previous[5] != chk_DarkField3Length.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_DarkField3Length.Text, (!chk_DarkField3Length.Checked).ToString(), chk_DarkField3Length.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectLength_Previous[6] != chk_DarkField4Length.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_DarkField4Length.Text, (!chk_DarkField4Length.Checked).ToString(), chk_DarkField4Length.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectLength_Previous[2] != chk_CrackDarkFieldLength.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_CrackDarkFieldLength.Text, (!chk_CrackDarkFieldLength.Checked).ToString(), chk_CrackDarkFieldLength.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectLength_Previous[3] != chk_VoidDarkFieldLength.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_VoidDarkFieldLength.Text, (!chk_VoidDarkFieldLength.Checked).ToString(), chk_VoidDarkFieldLength.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectArea_Previous[7] != chk_CheckChippedOffBright_Length.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_CheckChippedOffBright_Length.Text, (!chk_CheckChippedOffBright_Length.Checked).ToString(), chk_CheckChippedOffBright_Length.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectArea_Previous[8] != chk_CheckChippedOffDark_Length.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_CheckChippedOffDark_Length.Text, (!chk_CheckChippedOffDark_Length.Checked).ToString(), chk_CheckChippedOffDark_Length.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectArea_Previous[0] != chk_BrightFieldArea.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_BrightFieldArea.Text, (!chk_BrightFieldArea.Checked).ToString(), chk_BrightFieldArea.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectArea_Previous[1] != chk_DarkFieldArea.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_DarkFieldArea.Text, (!chk_DarkFieldArea.Checked).ToString(), chk_DarkFieldArea.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectArea_Previous[7] != chk_DarkField2Area.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_DarkField2Area.Text, (!chk_DarkField2Area.Checked).ToString(), chk_DarkField2Area.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectArea_Previous[8] != chk_DarkField3Area.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_DarkField3Area.Text, (!chk_DarkField3Area.Checked).ToString(), chk_DarkField3Area.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectArea_Previous[9] != chk_DarkField4Area.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_DarkField4Area.Text, (!chk_DarkField4Area.Checked).ToString(), chk_DarkField4Area.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectArea_Previous[2] != chk_CrackDarkFieldArea.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_CrackDarkFieldArea.Text, (!chk_CrackDarkFieldArea.Checked).ToString(), chk_CrackDarkFieldArea.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectArea_Previous[3] != chk_VoidDarkFieldArea.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_VoidDarkFieldArea.Text, (!chk_VoidDarkFieldArea.Checked).ToString(), chk_VoidDarkFieldArea.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectArea_Previous[4] != chk_MoldFlashBrightFieldArea.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_MoldFlashBrightFieldArea.Text, (!chk_MoldFlashBrightFieldArea.Checked).ToString(), chk_MoldFlashBrightFieldArea.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectArea_Previous[5] != chk_CheckChippedOffBright_Area.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_CheckChippedOffBright_Area.Text, (!chk_CheckChippedOffBright_Area.Checked).ToString(), chk_CheckChippedOffBright_Area.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_arrFailMaskDefectArea_Previous[6] != chk_CheckChippedOffDark_Area.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Package", "Inspection Option>" + chk_CheckChippedOffDark_Area.Text, (!chk_CheckChippedOffDark_Area.Checked).ToString(), chk_CheckChippedOffDark_Area.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
        }
        private void CheckChanges_Lead()
        {
            //------------------------------------Lead--------------------------------
            if (m_smVisionInfo.g_intSelectedROI >= m_smVisionInfo.g_arrLead.Length)
                m_smVisionInfo.g_intSelectedROI = 0;
            int intFailMask = m_smVisionInfo.g_arrLead[m_smVisionInfo.g_intSelectedROI].ref_intFailOptionMask;
            
            if (m_blnFailMaskInspectLead_Previous != chk_InspectLead.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_InspectLead.Text, (!chk_InspectLead.Checked).ToString(), chk_InspectLead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            int intXOR = intFailMask ^ m_intFailMaskLead_Previous;

            if ((intXOR & 0xC0) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_WidthHeight_Lead.Text, (!chk_WidthHeight_Lead.Checked).ToString(), chk_WidthHeight_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_OffSet_Lead.Text, (!chk_OffSet_Lead.Checked).ToString(), chk_OffSet_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x8000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_Skew_Lead.Text, (!chk_Skew_Lead.Checked).ToString(), chk_Skew_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x600) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_PitchGap_Lead.Text, (!chk_PitchGap_Lead.Checked).ToString(), chk_PitchGap_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x800) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_Variance_Lead.Text, (!chk_Variance_Lead.Checked).ToString(), chk_Variance_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x4000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_AverageGrayValue_Lead.Text, (!chk_AverageGrayValue_Lead.Checked).ToString(), chk_AverageGrayValue_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x1000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_Span_Lead.Text, (!chk_Span_Lead.Checked).ToString(), chk_Span_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskLeadExtraLength_Previous != chk_CheckForeignMaterialLength_Lead.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_CheckForeignMaterialLength_Lead.Text, (!chk_CheckForeignMaterialLength_Lead.Checked).ToString(), chk_CheckForeignMaterialLength_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskLeadExtraArea_Previous != chk_CheckForeignMaterialArea_Lead.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_CheckForeignMaterialArea_Lead.Text, (!chk_CheckForeignMaterialArea_Lead.Checked).ToString(), chk_CheckForeignMaterialArea_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x2000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_CheckForeignMaterialTotalArea_Lead.Text, (!chk_CheckForeignMaterialTotalArea_Lead.Checked).ToString(), chk_CheckForeignMaterialTotalArea_Lead.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x10000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_BaseLeadOffset.Text, (!chk_BaseLeadOffset.Checked).ToString(), chk_BaseLeadOffset.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead", "Inspection Option>" + chk_BaseLeadArea.Text, (!chk_BaseLeadArea.Checked).ToString(), chk_BaseLeadArea.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
        }

        private void CheckChanges_Pin1()
        {
            //------------------------------------Pin1--------------------------------
            if (m_smVisionInfo.g_blnWantPin1)
            {
                
                for (int i = 0; i < m_arrFailMaskPin1_Previous.Count; i++)
                {
                    if (m_arrFailMaskPin1_Previous[i] != m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(i))
                    {
                        STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pin1", "Inspection Option>" + chk_WantInspectPin1.Text + " Template " + (i + 1).ToString(), (!m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(i)).ToString(), m_smVisionInfo.g_arrPin1[0].getWantCheckPin1(i).ToString(), m_smProductionInfo.g_strLotID);
                    }
                }
                
            }
        }
        private void CheckChanges_CenterColorPad()
        {
            
            int intFailMask = m_smVisionInfo.g_arrPad[0].ref_intFailColorOptionMask;

            int intXOR = intFailMask ^ m_intFailMaskCenterColorPad_Previous;

            if ((intXOR & 0x01) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Color Pad", "Inspection Option>" + chk_ColorPad1Length_Center.Text, (!chk_ColorPad1Length_Center.Checked).ToString(), chk_ColorPad1Length_Center.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x02) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Color Pad", "Inspection Option>" + chk_ColorPad1Area_Center.Text, (!chk_ColorPad1Area_Center.Checked).ToString(), chk_ColorPad1Area_Center.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x04) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Color Pad", "Inspection Option>" + chk_ColorPad2Length_Center.Text, (!chk_ColorPad2Length_Center.Checked).ToString(), chk_ColorPad2Length_Center.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x08) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Color Pad", "Inspection Option>" + chk_ColorPad2Area_Center.Text, (!chk_ColorPad2Area_Center.Checked).ToString(), chk_ColorPad2Area_Center.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }


            if ((intXOR & 0x10) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Color Pad", "Inspection Option>" + chk_ColorPad3Length_Center.Text, (!chk_ColorPad3Length_Center.Checked).ToString(), chk_ColorPad3Length_Center.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Color Pad", "Inspection Option>" + chk_ColorPad3Area_Center.Text, (!chk_ColorPad3Area_Center.Checked).ToString(), chk_ColorPad3Area_Center.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Color Pad", "Inspection Option>" + chk_ColorPad4Length_Center.Text, (!chk_ColorPad4Length_Center.Checked).ToString(), chk_ColorPad4Length_Center.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Color Pad", "Inspection Option>" + chk_ColorPad4Area_Center.Text, (!chk_ColorPad4Area_Center.Checked).ToString(), chk_ColorPad4Area_Center.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Color Pad", "Inspection Option>" + chk_ColorPad5Length_Center.Text, (!chk_ColorPad5Length_Center.Checked).ToString(), chk_ColorPad5Length_Center.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x200) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Color Pad", "Inspection Option>" + chk_ColorPad5Area_Center.Text, (!chk_ColorPad5Area_Center.Checked).ToString(), chk_ColorPad5Area_Center.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
        }
        private void CheckChanges_SideColorPad_Top()
        {
            
            int intFailMask = m_smVisionInfo.g_arrPad[1].ref_intFailColorOptionMask;

            int intXOR = intFailMask ^ m_intFailMaskSideColorPad_Top_Previous;

            if ((intXOR & 0x01) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad1Length_Side_Top.Text, (!chk_ColorPad1Length_Side_Top.Checked).ToString(), chk_ColorPad1Length_Side_Top.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x02) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad1Area_Side_Top.Text, (!chk_ColorPad1Area_Side_Top.Checked).ToString(), chk_ColorPad1Area_Side_Top.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x04) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad2Length_Side_Top.Text, (!chk_ColorPad2Length_Side_Top.Checked).ToString(), chk_ColorPad2Length_Side_Top.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x08) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad2Area_Side_Top.Text, (!chk_ColorPad2Area_Side_Top.Checked).ToString(), chk_ColorPad2Area_Side_Top.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }


            if ((intXOR & 0x10) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad3Length_Side_Top.Text, (!chk_ColorPad3Length_Side_Top.Checked).ToString(), chk_ColorPad3Length_Side_Top.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad3Area_Side_Top.Text, (!chk_ColorPad3Area_Side_Top.Checked).ToString(), chk_ColorPad3Area_Side_Top.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad4Length_Side_Top.Text, (!chk_ColorPad4Length_Side_Top.Checked).ToString(), chk_ColorPad4Length_Side_Top.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad4Area_Side_Top.Text, (!chk_ColorPad4Area_Side_Top.Checked).ToString(), chk_ColorPad4Area_Side_Top.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad5Length_Side_Top.Text, (!chk_ColorPad5Length_Side_Top.Checked).ToString(), chk_ColorPad5Length_Side_Top.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x200) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad5Area_Side_Top.Text, (!chk_ColorPad5Area_Side_Top.Checked).ToString(), chk_ColorPad5Area_Side_Top.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

        }
        private void CheckChanges_SideColorPad_Right()
        {

            int intFailMask = m_smVisionInfo.g_arrPad[2].ref_intFailColorOptionMask;

            int intXOR = intFailMask ^ m_intFailMaskSideColorPad_Right_Previous;

            if ((intXOR & 0x01) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad1Length_Side_Right.Text, (!chk_ColorPad1Length_Side_Right.Checked).ToString(), chk_ColorPad1Length_Side_Right.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x02) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad1Area_Side_Right.Text, (!chk_ColorPad1Area_Side_Right.Checked).ToString(), chk_ColorPad1Area_Side_Right.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x04) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad2Length_Side_Right.Text, (!chk_ColorPad2Length_Side_Right.Checked).ToString(), chk_ColorPad2Length_Side_Right.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x08) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad2Area_Side_Right.Text, (!chk_ColorPad2Area_Side_Right.Checked).ToString(), chk_ColorPad2Area_Side_Right.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }


            if ((intXOR & 0x10) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad3Length_Side_Right.Text, (!chk_ColorPad3Length_Side_Right.Checked).ToString(), chk_ColorPad3Length_Side_Right.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad3Area_Side_Right.Text, (!chk_ColorPad3Area_Side_Right.Checked).ToString(), chk_ColorPad3Area_Side_Right.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad4Length_Side_Right.Text, (!chk_ColorPad4Length_Side_Right.Checked).ToString(), chk_ColorPad4Length_Side_Right.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad4Area_Side_Right.Text, (!chk_ColorPad4Area_Side_Right.Checked).ToString(), chk_ColorPad4Area_Side_Right.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad5Length_Side_Right.Text, (!chk_ColorPad5Length_Side_Right.Checked).ToString(), chk_ColorPad5Length_Side_Right.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x200) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad5Area_Side_Right.Text, (!chk_ColorPad5Area_Side_Right.Checked).ToString(), chk_ColorPad5Area_Side_Right.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

        }
        private void CheckChanges_SideColorPad_Bottom()
        {

            int intFailMask = m_smVisionInfo.g_arrPad[3].ref_intFailColorOptionMask;

            int intXOR = intFailMask ^ m_intFailMaskSideColorPad_Bottom_Previous;

            if ((intXOR & 0x01) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad1Length_Side_Bottom.Text, (!chk_ColorPad1Length_Side_Bottom.Checked).ToString(), chk_ColorPad1Length_Side_Bottom.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x02) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad1Area_Side_Bottom.Text, (!chk_ColorPad1Area_Side_Bottom.Checked).ToString(), chk_ColorPad1Area_Side_Bottom.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x04) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad2Length_Side_Bottom.Text, (!chk_ColorPad2Length_Side_Bottom.Checked).ToString(), chk_ColorPad2Length_Side_Bottom.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x08) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad2Area_Side_Bottom.Text, (!chk_ColorPad2Area_Side_Bottom.Checked).ToString(), chk_ColorPad2Area_Side_Bottom.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }


            if ((intXOR & 0x10) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad3Length_Side_Bottom.Text, (!chk_ColorPad3Length_Side_Bottom.Checked).ToString(), chk_ColorPad3Length_Side_Bottom.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad3Area_Side_Bottom.Text, (!chk_ColorPad3Area_Side_Bottom.Checked).ToString(), chk_ColorPad3Area_Side_Bottom.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad4Length_Side_Bottom.Text, (!chk_ColorPad4Length_Side_Bottom.Checked).ToString(), chk_ColorPad4Length_Side_Bottom.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad4Area_Side_Bottom.Text, (!chk_ColorPad4Area_Side_Bottom.Checked).ToString(), chk_ColorPad4Area_Side_Bottom.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad5Length_Side_Bottom.Text, (!chk_ColorPad5Length_Side_Bottom.Checked).ToString(), chk_ColorPad5Length_Side_Bottom.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x200) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad5Area_Side_Bottom.Text, (!chk_ColorPad5Area_Side_Bottom.Checked).ToString(), chk_ColorPad5Area_Side_Bottom.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

        }
        private void CheckChanges_SideColorPad_Left()
        {

            int intFailMask = m_smVisionInfo.g_arrPad[4].ref_intFailColorOptionMask;

            int intXOR = intFailMask ^ m_intFailMaskSideColorPad_Left_Previous;

            if ((intXOR & 0x01) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad1Length_Side_Left.Text, (!chk_ColorPad1Length_Side_Left.Checked).ToString(), chk_ColorPad1Length_Side_Left.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x02) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad1Area_Side_Left.Text, (!chk_ColorPad1Area_Side_Left.Checked).ToString(), chk_ColorPad1Area_Side_Left.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x04) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad2Length_Side_Left.Text, (!chk_ColorPad2Length_Side_Left.Checked).ToString(), chk_ColorPad2Length_Side_Left.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x08) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad2Area_Side_Left.Text, (!chk_ColorPad2Area_Side_Left.Checked).ToString(), chk_ColorPad2Area_Side_Left.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }


            if ((intXOR & 0x10) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad3Length_Side_Left.Text, (!chk_ColorPad3Length_Side_Left.Checked).ToString(), chk_ColorPad3Length_Side_Left.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad3Area_Side_Left.Text, (!chk_ColorPad3Area_Side_Left.Checked).ToString(), chk_ColorPad3Area_Side_Left.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad4Length_Side_Left.Text, (!chk_ColorPad4Length_Side_Left.Checked).ToString(), chk_ColorPad4Length_Side_Left.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad4Area_Side_Left.Text, (!chk_ColorPad4Area_Side_Left.Checked).ToString(), chk_ColorPad4Area_Side_Left.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad5Length_Side_Left.Text, (!chk_ColorPad5Length_Side_Left.Checked).ToString(), chk_ColorPad5Length_Side_Left.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x200) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Color Pad", "Inspection Option>" + chk_ColorPad5Area_Side_Left.Text, (!chk_ColorPad5Area_Side_Left.Checked).ToString(), chk_ColorPad5Area_Side_Left.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

        }
        private void CheckChanges_CenterPad()
        {
            
            if (m_blnFailMaskInspectPad_Previous != chk_InspectPad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_InspectPad.Text, (!chk_InspectPad.Checked).ToString(), chk_InspectPad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            int intFailMask = m_smVisionInfo.g_arrPad[0].ref_intFailOptionMask;

            int intXOR = intFailMask ^ m_intFailMaskCenterPad_Previous;

            if ((intXOR & 0x20) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_Area_Pad.Text, (!chk_Area_Pad.Checked).ToString(), chk_Area_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0xC0) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_WidthHeight_Pad.Text, (!chk_WidthHeight_Pad.Checked).ToString(), chk_WidthHeight_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_OffSet_Pad.Text, (!chk_OffSet_Pad.Checked).ToString(), chk_OffSet_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskCenterPadExtraLength_Previous != chk_CheckForeignMaterialLength_Pad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_CheckForeignMaterialLength_Pad.Text, (!chk_CheckForeignMaterialLength_Pad.Checked).ToString(), chk_CheckForeignMaterialLength_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskCenterPadExtraArea_Previous != chk_CheckForeignMaterialArea_Pad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_CheckForeignMaterialArea_Pad.Text, (!chk_CheckForeignMaterialArea_Pad.Checked).ToString(), chk_CheckForeignMaterialArea_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskCenterPadBrokenLength_Previous != chk_CheckBrokenLength_Pad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_CheckBrokenLength_Pad.Text, (!chk_CheckBrokenLength_Pad.Checked).ToString(), chk_CheckBrokenLength_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskCenterPadBrokenArea_Previous != chk_CheckBrokenArea_Pad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_CheckBrokenArea_Pad.Text, (!chk_CheckBrokenArea_Pad.Checked).ToString(), chk_CheckBrokenArea_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x600) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_Gap_Pad.Text, (!chk_Gap_Pad.Checked).ToString(), chk_Gap_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x800) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_CheckExcess_Pad.Text, (!chk_CheckExcess_Pad.Checked).ToString(), chk_CheckExcess_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x2000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_CheckSmear_Pad.Text, (!chk_CheckSmear_Pad.Checked).ToString(), chk_CheckSmear_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x4000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_CheckEdgeLimit_Pad.Text, (!chk_CheckEdgeLimit_Pad.Checked).ToString(), chk_CheckEdgeLimit_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            if ((intXOR & 0x8000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_CheckStandOff_Pad.Text, (!chk_CheckStandOff_Pad.Checked).ToString(), chk_CheckStandOff_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x10000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_CheckEdgeDistance_Pad.Text, (!chk_CheckEdgeDistance_Pad.Checked).ToString(), chk_CheckEdgeDistance_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_CheckSpanX_Pad.Text, (!chk_CheckSpanX_Pad.Checked).ToString(), chk_CheckSpanX_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_CheckSpanY_Pad.Text, (!chk_CheckSpanY_Pad.Checked).ToString(), chk_CheckSpanY_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x1000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_CheckForeignMaterialTotalArea_Pad.Text, (!chk_CheckForeignMaterialTotalArea_Pad.Checked).ToString(), chk_CheckForeignMaterialTotalArea_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            //------------------------------------Pin1--------------------------------
            if (m_smVisionInfo.g_blnWantPin1)
            {
                if(m_arrFailMaskPin1_Previous[m_smVisionInfo.g_intSelectedTemplate] != chk_WantInspectPin1.Checked)
                {
                    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_WantInspectPin1.Text, (!chk_WantInspectPin1.Checked).ToString(), chk_WantInspectPin1.Checked.ToString(), m_smProductionInfo.g_strLotID);
                }
            }
            
        }

        private void CheckChanges_SidePad()
        {
            int intFailMask = m_smVisionInfo.g_arrPad[1].ref_intFailOptionMask;

            int intXOR = intFailMask ^ m_intFailMaskSidePad_Previous;
            
            if ((intXOR & 0x20) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_Area_SidePad.Text, (!chk_Area_SidePad.Checked).ToString(), chk_Area_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0xC0) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_WidthHeight_SidePad.Text, (!chk_WidthHeight_SidePad.Checked).ToString(), chk_WidthHeight_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_OffSet_SidePad.Text, (!chk_OffSet_SidePad.Checked).ToString(), chk_OffSet_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskSidePadExtraLength_Previous != chk_CheckForeignMaterialLength_SidePad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_CheckForeignMaterialLength_SidePad.Text, (!chk_CheckForeignMaterialLength_SidePad.Checked).ToString(), chk_CheckForeignMaterialLength_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskSidePadExtraArea_Previous != chk_CheckForeignMaterialArea_SidePad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_CheckForeignMaterialArea_SidePad.Text, (!chk_CheckForeignMaterialArea_SidePad.Checked).ToString(), chk_CheckForeignMaterialArea_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskSidePadBrokenLength_Previous != chk_CheckBrokenLength_SidePad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_CheckBrokenLength_SidePad.Text, (!chk_CheckBrokenLength_SidePad.Checked).ToString(), chk_CheckBrokenLength_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskSidePadBrokenArea_Previous != chk_CheckBrokenArea_SidePad.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_CheckBrokenArea_SidePad.Text, (!chk_CheckBrokenArea_SidePad.Checked).ToString(), chk_CheckBrokenArea_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x600) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_Gap_SidePad.Text, (!chk_Gap_SidePad.Checked).ToString(), chk_Gap_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x800) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_CheckExcess_SidePad.Text, (!chk_CheckExcess_SidePad.Checked).ToString(), chk_CheckExcess_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x2000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_CheckSmear_SidePad.Text, (!chk_CheckSmear_SidePad.Checked).ToString(), chk_CheckSmear_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x4000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_CheckEdgeLimit_SidePad.Text, (!chk_CheckEdgeLimit_SidePad.Checked).ToString(), chk_CheckEdgeLimit_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x8000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_CheckStandOff_SidePad.Text, (!chk_CheckStandOff_SidePad.Checked).ToString(), chk_CheckStandOff_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x10000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_CheckEdgeDistance_SidePad.Text, (!chk_CheckEdgeDistance_SidePad.Checked).ToString(), chk_CheckEdgeDistance_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_CheckSpanX_SidePad.Text, (!chk_CheckSpanX_SidePad.Checked).ToString(), chk_CheckSpanX_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_CheckSpanY_SidePad.Text, (!chk_CheckSpanY_SidePad.Checked).ToString(), chk_CheckSpanY_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x1000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad", "Inspection Option>" + chk_CheckForeignMaterialTotalArea_SidePad.Text, (!chk_CheckForeignMaterialTotalArea_SidePad.Checked).ToString(), chk_CheckForeignMaterialTotalArea_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
        }

        private void CheckChanges_CenterPadPackage()
        {
            int intFailMask = m_smVisionInfo.g_arrPad[0].ref_intFailPkgOptionMask;

            int intXOR = intFailMask ^ m_intFailMaskCenterPadPackage_Previous;
            
            if ((intXOR & 0x01) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_CheckPkgSize_Pad2.Text, (!chk_CheckPkgSize_Pad2.Checked).ToString(), chk_CheckPkgSize_Pad2.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskCenterPadPackageDefect_Previous != chk_InspectPackage_Pad2.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_InspectPackage_Pad2.Text, (!chk_InspectPackage_Pad2.Checked).ToString(), chk_InspectPackage_Pad2.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x10000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_BrightFieldArea_Pad.Text, (!chk_BrightFieldArea_Pad.Checked).ToString(), chk_BrightFieldArea_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_BrightFieldLength_Pad.Text, (!chk_BrightFieldLength_Pad.Checked).ToString(), chk_BrightFieldLength_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_DarkFieldArea_Pad.Text, (!chk_DarkFieldArea_Pad.Checked).ToString(), chk_DarkFieldArea_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_DarkFieldLength_Pad.Text, (!chk_DarkFieldLength_Pad.Checked).ToString(), chk_DarkFieldLength_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x800) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_CrackDarkFieldArea_Pad.Text, (!chk_CrackDarkFieldArea_Pad.Checked).ToString(), chk_CrackDarkFieldArea_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x400) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_CrackDarkFieldLength_Pad.Text, (!chk_CrackDarkFieldLength_Pad.Checked).ToString(), chk_CrackDarkFieldLength_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_ChippedOffDarkFieldArea_Pad.Text, (!chk_ChippedOffDarkFieldArea_Pad.Checked).ToString(), chk_ChippedOffDarkFieldArea_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x200000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_ChippedOffBrightFieldArea_Pad.Text, (!chk_ChippedOffBrightFieldArea_Pad.Checked).ToString(), chk_ChippedOffBrightFieldArea_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_MoldFlashBrightFieldArea_Pad.Text, (!chk_MoldFlashBrightFieldArea_Pad.Checked).ToString(), chk_MoldFlashBrightFieldArea_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x1000000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_MoldFlashBrightFieldLength_Pad.Text, (!chk_MoldFlashBrightFieldLength_Pad.Checked).ToString(), chk_MoldFlashBrightFieldLength_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x400000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_ForeignMaterialBrightFieldArea_Pad.Text, (!chk_ForeignMaterialBrightFieldArea_Pad.Checked).ToString(), chk_ForeignMaterialBrightFieldArea_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x800000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad Package", "Inspection Option>" + chk_ForeignMaterialBrightFieldLength_Pad.Text, (!chk_ForeignMaterialBrightFieldLength_Pad.Checked).ToString(), chk_ForeignMaterialBrightFieldLength_Pad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            
        }

        private void CheckChanges_SidePadPackage()
        {
            int intFailMask = m_smVisionInfo.g_arrPad[1].ref_intFailPkgOptionMask;

            int intXOR = intFailMask ^ m_intFailMaskSidePadPackage_Previous;
            
            if ((intXOR & 0x01) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_CheckPkgSize_SidePad2.Text, (!chk_CheckPkgSize_SidePad2.Checked).ToString(), chk_CheckPkgSize_SidePad2.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskCenterPadPackageDefect_Previous != chk_InspectPackage_SidePad2.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_InspectPackage_SidePad2.Text, (!chk_InspectPackage_SidePad2.Checked).ToString(), chk_InspectPackage_SidePad2.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x10000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_BrightFieldArea_SidePad.Text, (!chk_BrightFieldArea_SidePad.Checked).ToString(), chk_BrightFieldArea_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_BrightFieldLength_SidePad.Text, (!chk_BrightFieldLength_SidePad.Checked).ToString(), chk_BrightFieldLength_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_DarkFieldArea_SidePad.Text, (!chk_DarkFieldArea_SidePad.Checked).ToString(), chk_DarkFieldArea_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_DarkFieldLength_SidePad.Text, (!chk_DarkFieldLength_SidePad.Checked).ToString(), chk_DarkFieldLength_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x800) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_CrackDarkFieldArea_SidePad.Text, (!chk_CrackDarkFieldArea_SidePad.Checked).ToString(), chk_CrackDarkFieldArea_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x400) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_CrackDarkFieldLength_SidePad.Text, (!chk_CrackDarkFieldLength_SidePad.Checked).ToString(), chk_CrackDarkFieldLength_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_ChippedOffDarkFieldArea_SidePad.Text, (!chk_ChippedOffDarkFieldArea_SidePad.Checked).ToString(), chk_ChippedOffDarkFieldArea_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x200000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_ChippedOffBrightFieldArea_SidePad.Text, (!chk_ChippedOffBrightFieldArea_SidePad.Checked).ToString(), chk_ChippedOffBrightFieldArea_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_MoldFlashBrightFieldArea_SidePad.Text, (!chk_MoldFlashBrightFieldArea_SidePad.Checked).ToString(), chk_MoldFlashBrightFieldArea_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x1000000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_MoldFlashBrightFieldLength_SidePad.Text, (!chk_MoldFlashBrightFieldLength_SidePad.Checked).ToString(), chk_MoldFlashBrightFieldLength_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x400000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_ForeignMaterialBrightFieldArea_SidePad.Text, (!chk_ForeignMaterialBrightFieldArea_SidePad.Checked).ToString(), chk_ForeignMaterialBrightFieldArea_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x800000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Side Pad Package", "Inspection Option>" + chk_ForeignMaterialBrightFieldLength_SidePad.Text, (!chk_ForeignMaterialBrightFieldLength_SidePad.Checked).ToString(), chk_ForeignMaterialBrightFieldLength_SidePad.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            
        }

        private void CheckChanges_Lead3D()
        {
            int intFailMask = m_smVisionInfo.g_arrLead3D[0].ref_intFailOptionMask;
            
            int intXOR = intFailMask ^ m_intFailMaskLead3D_Previous;

            if ((intXOR & 0x20000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_Offset_Lead3D.Text, (!chk_Offset_Lead3D.Checked).ToString(), chk_Offset_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_Skew_Lead3D.Text, (!chk_Skew_Lead3D.Checked).ToString(), chk_Skew_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_Width_Lead3D.Text, (!chk_Width_Lead3D.Checked).ToString(), chk_Width_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_Length_Lead3D.Text, (!chk_Length_Lead3D.Checked).ToString(), chk_Length_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x800) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_LengthVariance_Lead3D.Text, (!chk_LengthVariance_Lead3D.Checked).ToString(), chk_LengthVariance_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x600) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_PitchGap_Lead3D.Text, (!chk_PitchGap_Lead3D.Checked).ToString(), chk_PitchGap_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x2000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_PitchVariance_Lead3D.Text, (!chk_PitchVariance_Lead3D.Checked).ToString(), chk_PitchVariance_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x01) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_StandOff_Lead3D.Text, (!chk_StandOff_Lead3D.Checked).ToString(), chk_StandOff_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x4000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_StandoffVariance_Lead3D.Text, (!chk_StandoffVariance_Lead3D.Checked).ToString(), chk_StandoffVariance_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x02) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_Coplan_Lead3D.Text, (!chk_Coplan_Lead3D.Checked).ToString(), chk_Coplan_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x1000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_Span_Lead3D.Text, (!chk_Span_Lead3D.Checked).ToString(), chk_Span_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x04) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_LeadSweeps_Lead3D.Text, (!chk_LeadSweeps_Lead3D.Checked).ToString(), chk_LeadSweeps_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x08) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_SolderPadLength_Lead3D.Text, (!chk_SolderPadLength_Lead3D.Checked).ToString(), chk_SolderPadLength_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x10) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_UnCutTiebar_Lead3D.Text, (!chk_UnCutTiebar_Lead3D.Checked).ToString(), chk_UnCutTiebar_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskLead3DExtraLength_Previous != chk_CheckForeignMaterialLength_Lead3D.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_CheckForeignMaterialLength_Lead3D.Text, (!chk_CheckForeignMaterialLength_Lead3D.Checked).ToString(), chk_CheckForeignMaterialLength_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskLead3DExtraArea_Previous != chk_CheckForeignMaterialArea_Lead3D.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_CheckForeignMaterialArea_Lead3D.Text, (!chk_CheckForeignMaterialArea_Lead3D.Checked).ToString(), chk_CheckForeignMaterialArea_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x10000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_CheckForeignMaterialTotalArea_Lead3D.Text, (!chk_CheckForeignMaterialTotalArea_Lead3D.Checked).ToString(), chk_CheckForeignMaterialTotalArea_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_AverageGrayValue_Lead3D.Text, (!chk_AverageGrayValue_Lead3D.Checked).ToString(), chk_AverageGrayValue_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_LeadMinAndMaxWidth_Lead3D.Text, (!chk_LeadMinAndMaxWidth_Lead3D.Checked).ToString(), chk_LeadMinAndMaxWidth_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x200000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D", "Inspection Option>" + chk_LeadBurrWidth.Text, (!chk_LeadBurrWidth.Checked).ToString(), chk_LeadBurrWidth.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            //------------------------------------Pin1--------------------------------
            if (m_smVisionInfo.g_blnWantPin1)
            {
                if (m_arrFailMaskPin1_Previous[m_smVisionInfo.g_intSelectedTemplate] != chk_WantInspectPin1.Checked)
                {
                    STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Center Pad", "Inspection Option>" + chk_WantInspectPin1.Text, (!chk_WantInspectPin1.Checked).ToString(), chk_WantInspectPin1.Checked.ToString(), m_smProductionInfo.g_strLotID);
                }
            }
            
        }
        private void CheckChanges_Lead3DPackage()
        {
            int intFailMask = m_smVisionInfo.g_arrLead3D[0].ref_intFailPkgOptionMask;

            int intXOR = intFailMask ^ m_intFailMaskLead3DPackage_Previous;
            
            if ((intXOR & 0x01) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Inspection Option>" + chk_CheckPkgSize_Lead3D.Text, (!chk_CheckPkgSize_Lead3D.Checked).ToString(), chk_CheckPkgSize_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if (m_blnFailMaskLead3DPackageDefect_Previous != chk_InspectPackage_Lead3D.Checked)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Inspection Option>" + chk_InspectPackage_Lead3D.Text, (!chk_InspectPackage_Lead3D.Checked).ToString(), chk_InspectPackage_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x10000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Inspection Option>" + chk_BrightFieldArea_Lead3D.Text, (!chk_BrightFieldArea_Lead3D.Checked).ToString(), chk_BrightFieldArea_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Inspection Option>" + chk_BrightFieldLength_Lead3D.Text, (!chk_BrightFieldLength_Lead3D.Checked).ToString(), chk_BrightFieldLength_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Inspection Option>" + chk_DarkFieldArea_Lead3D.Text, (!chk_DarkFieldArea_Lead3D.Checked).ToString(), chk_DarkFieldArea_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Inspection Option>" + chk_DarkFieldLength_Lead3D.Text, (!chk_DarkFieldLength_Lead3D.Checked).ToString(), chk_DarkFieldLength_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x800) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Inspection Option>" + chk_CrackDarkFieldArea_Lead3D.Text, (!chk_CrackDarkFieldArea_Lead3D.Checked).ToString(), chk_CrackDarkFieldArea_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x400) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Inspection Option>" + chk_CrackDarkFieldLength_Lead3D.Text, (!chk_CrackDarkFieldLength_Lead3D.Checked).ToString(), chk_CrackDarkFieldLength_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Inspection Option>" + chk_ChippedOffDarkFieldArea_Lead3D.Text, (!chk_ChippedOffDarkFieldArea_Lead3D.Checked).ToString(), chk_ChippedOffDarkFieldArea_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x200000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Inspection Option>" + chk_ChippedOffBrightFieldArea_Lead3D.Text, (!chk_ChippedOffBrightFieldArea_Lead3D.Checked).ToString(), chk_ChippedOffBrightFieldArea_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package", "Inspection Option>" + chk_MoldFlashBrightFieldArea_Lead3D.Text, (!chk_MoldFlashBrightFieldArea_Lead3D.Checked).ToString(), chk_MoldFlashBrightFieldArea_Lead3D.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
        }

        private void CheckChanges_Seal()
        {
            int intFailMask = m_smVisionInfo.g_objSeal.ref_intFailOptionMaskSeal;

            int intXOR = intFailMask ^ m_intFailMaskSeal_Previous;
            
            if ((intXOR & 0x01) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealWidth.Text, (!chk_CheckSealWidth.Checked).ToString(), chk_CheckSealWidth.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x02) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealBubble.Text, (!chk_CheckSealBubble.Checked).ToString(), chk_CheckSealBubble.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            if ((intXOR & 0x04) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealShifted.Text, (!chk_CheckSealShifted.Checked).ToString(), chk_CheckSealShifted.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x08) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealDistance.Text, (!chk_CheckSealDistance.Checked).ToString(), chk_CheckSealDistance.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x10) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealOverHeat.Text, (!chk_CheckSealOverHeat.Checked).ToString(), chk_CheckSealOverHeat.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x20) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealBroken.Text, (!chk_CheckSealBroken.Checked).ToString(), chk_CheckSealBroken.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x40) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealUnitPresent.Text, (!chk_CheckSealUnitPresent.Checked).ToString(), chk_CheckSealUnitPresent.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x80) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealUnitOrient.Text, (!chk_CheckSealUnitOrient.Checked).ToString(), chk_CheckSealUnitOrient.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x100) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealSprocketHoleDistance.Text, (!chk_CheckSealSprocketHoleDistance.Checked).ToString(), chk_CheckSealSprocketHoleDistance.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x200) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealSprocketHoleDiameter.Text, (!chk_CheckSealSprocketHoleDiameter.Checked).ToString(), chk_CheckSealSprocketHoleDiameter.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
            
            if ((intXOR & 0x400) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealSprocketHoleDefect.Text, (!chk_CheckSealSprocketHoleDefect.Checked).ToString(), chk_CheckSealSprocketHoleDefect.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x800) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealSprocketHoleBroken.Text, (!chk_CheckSealSprocketHoleBroken.Checked).ToString(), chk_CheckSealSprocketHoleBroken.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x1000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealSprocketHoleRoundness.Text, (!chk_CheckSealSprocketHoleRoundness.Checked).ToString(), chk_CheckSealSprocketHoleRoundness.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }

            if ((intXOR & 0x2000) > 0)
            {
                STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Seal", "Inspection Option>" + chk_CheckSealEdgeStraightness.Text, (!chk_CheckSealEdgeStraightness.Checked).ToString(), chk_CheckSealEdgeStraightness.Checked.ToString(), m_smProductionInfo.g_strLotID);
            }
        }
        private void EnablePadFailMaskGUI(bool blnEnable)
        {
            chk_Area_Pad.Enabled = blnEnable;
            
            chk_WidthHeight_Pad.Enabled = blnEnable;
            
            chk_OffSet_Pad.Enabled = blnEnable;
            
            chk_CheckForeignMaterialArea_Pad.Enabled = blnEnable;
            chk_CheckForeignMaterialLength_Pad.Enabled = blnEnable;
            
            chk_CheckBrokenArea_Pad.Enabled = blnEnable;
            chk_CheckBrokenLength_Pad.Enabled = blnEnable;
            
            chk_Gap_Pad.Enabled = blnEnable;
            
            chk_CheckExcess_Pad.Enabled = blnEnable;
          
            chk_CheckSmear_Pad.Enabled = blnEnable;
            
            chk_CheckEdgeLimit_Pad.Enabled = blnEnable;
            
            chk_CheckStandOff_Pad.Enabled = blnEnable;

            chk_CheckEdgeDistance_Pad.Enabled = blnEnable;

            chk_CheckSpanX_Pad.Enabled = blnEnable;

            chk_CheckSpanY_Pad.Enabled = blnEnable;

            chk_CheckForeignMaterialTotalArea_Pad.Enabled = blnEnable;
        }

        private void chk_WantInspectOrientAngleTolerance_Pad_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_objPadOrient != null)
            {
                m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientAngleTolerance = chk_WantInspectOrientAngleTolerance_Pad.Checked;
            }
        }

        private void chk_WantInspectOrientXTolerance_Pad_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_objPadOrient != null)
            {
                m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientXTolerance = chk_WantInspectOrientXTolerance_Pad.Checked;
            }
        }

        private void chk_WantInspectOrientYTolerance_Pad_Click(object sender, EventArgs e)
        {
            if (m_smVisionInfo.g_objPadOrient != null)
            {
                m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientYTolerance = chk_WantInspectOrientYTolerance_Pad.Checked;
            }
        }

        private void chk_InspectOrient_ForMO_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < m_smVisionInfo.g_intUnitsOnImage; i++)
            {
                if (i < m_smVisionInfo.g_arrOrients.Count)
                {
                    for (int j = 0; j < m_smVisionInfo.g_arrOrients[i].Count; j++)
                        m_smVisionInfo.g_arrOrients[i][j].ref_blnWantCheckOrientation = chk_InspectOrient_ForMO.Checked;
                }
            }
        }

        private void chk_CenterColorPadFailMask_Click(object sender, EventArgs e)
        {
            SetCenterColorPadFailMask(sender);
        }

        private void chk_SideColorPadFailMask_Top_Click(object sender, EventArgs e)
        {
            SetSideColorPadFailMask_Top(sender);
        }
        private void chk_SideColorPadFailMask_Right_Click(object sender, EventArgs e)
        {
            SetSideColorPadFailMask_Right(sender);
        }
        private void chk_SideColorPadFailMask_Bottom_Click(object sender, EventArgs e)
        {
            SetSideColorPadFailMask_Bottom(sender);
        }
        private void chk_SideColorPadFailMask_Left_Click(object sender, EventArgs e)
        {
            SetSideColorPadFailMask_Left(sender);
        }
        private void chk_ColorPackageFailMask_Click(object sender, EventArgs e)
        {
            SetColorPackageFailMask(sender);
        }
        
    }
}