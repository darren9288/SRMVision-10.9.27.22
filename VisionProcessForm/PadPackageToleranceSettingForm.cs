using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using VisionProcessing;
using SharedMemory;


namespace VisionProcessForm
{
    public partial class PadPackageToleranceSettingForm : Form
    {
        #region Enum
        public enum RowSettingName
        {
            PackageSize,
            XDimension,
            YDimension,
            ZDimension,
            TopThickness,
            RightThickness,
            BottomThickness,
            LeftThickness,
            CenterPackage,
            CenterPkg_ScratchesLength,
            CenterPkg_ScratchesArea,
            CenterPkg_ChippedOffArea,
            CenterPkg_ContaminationLength,
            CenterPkg_ContaminationArea,
            CenterPkg_ContaminationTotalArea,
            CenterPkg_MoldFlashArea,
            CenterPkg_VoidLength,
            CenterPkg_VoidArea,
            CenterPkg_CrackLength,
            CenterPkg_CrackArea,
            SidePackage,
            SidePkg_ScratchesLength,
            SidePkg_ScratchesArea,
            SidePkg_ChippedOffArea,
            SidePkg_ContaminationLength,
            SidePkg_ContaminationArea,
            SidePkg_ContaminationTotalArea,
            SidePkg_MoldFlashArea,
            SidePkg_VoidLength,
            SidePkg_VoidArea,
            SidePkg_CrackLength,
            SidePkg_CrackArea,
        };
        #endregion

        #region Member Variables

        private bool m_blnUpdateSelectedROISetting = false;
        private bool m_blnInitDone = false;
        private int m_intDecimal = 4;
        private int m_intDecimal2 = 6;
        private int m_intUserGroup = 5;
        private string m_strSelectedRecipe;
        private List<RowSettingName> m_arrRowSettingName = new List<RowSettingName>();

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        #endregion

        public PadPackageToleranceSettingForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup)
        {
            InitializeComponent();

            m_strSelectedRecipe = strSelectedRecipe;
            m_smCustomizeInfo = smCustomizeInfo;
            m_smVisionInfo = visionInfo;
            m_smProductionInfo = smProductionInfo;
            m_intUserGroup = intUserGroup;
           
            if (m_smVisionInfo.g_intSelectedROIMask == 0)
            {
                m_smVisionInfo.g_intSelectedROIMask = 0x01;
                m_smVisionInfo.g_intSelectedROI = 0;    // Reset to selecting center ROI when display form.
            }

            if (!m_smVisionInfo.g_blnWantCheckPH && !m_smVisionInfo.g_blnCheckPH)
            {
                // Clear ROI drag handler
                for (int i = 0; i < m_smVisionInfo.g_arrPadROIs.Count; i++)
                {
                    if (m_smVisionInfo.g_arrPadROIs[i].Count > 0 && m_smVisionInfo.g_arrPadROIs[i][0].GetROIHandle())
                        m_smVisionInfo.g_arrPadROIs[i][0].ClearDragHandle();
                }
            }
            else if (m_smVisionInfo.g_blnWantCheckPH && m_smVisionInfo.g_blnCheckPH)
            {
                if(m_smVisionInfo.g_arrPHROIs.Count > 0)
                if (m_smVisionInfo.g_arrPHROIs[0].GetROIHandle())
                    m_smVisionInfo.g_arrPHROIs[0].ClearDragHandle();
            }
            DisableField2();
            UpdateGUI();
            chk_SetToAllSideROI.Visible = chk_SetToAllSideROI.Checked = false;
            m_blnInitDone = true;
        }
        private void DisableField()
        {
            string strChild1 = "Test Page";
            string strChild2 = "";
            
            strChild1 = "Tolerance Page";
            strChild2 = "Pad Package Tolerance Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                txt_BlackAreaArea.Enabled = false;
                dgd_Position.ReadOnly = true;
                dgd_PackageSetting.ReadOnly = true;
            }

            strChild1 = "Tolerance Page";
            strChild2 = "Save Tolerance Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
            }

            strChild1 = "Tolerance Page";
            strChild2 = "Pad Package Size Offset Setting";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                group_PackageOffset.Visible = false;
            }
            else
            {
                group_PackageOffset.Visible = true;
            }
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            //NewUserRight objUserRight = new NewUserRight(false);
            string strChild1 = "Tolerance";
            string strChild2 = "";

            if (m_smVisionInfo.g_strVisionName == "BottomOrientPad" || m_smVisionInfo.g_strVisionName == "BottomOPadPkg")
            {
                strChild2 = "Package TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild2Group(strChild1, strChild2))
                {
                    group_GeneralDefectSetting.Enabled = false;
                    group_ForeignMaterial.Enabled = false;
                    group_ChippedOff.Enabled = false;
                    group_MoldFlash.Enabled = false;
                    group_Crack.Enabled = false;
                    group_GeneralDefectSetting_Side.Enabled = false;
                    group_ForeignMaterial_Side.Enabled = false;
                    group_ChippedOff_Side.Enabled = false;
                    group_MoldFlash_Side.Enabled = false;
                    group_Crack_Side.Enabled = false;
                    dgd_PackageSetting.Enabled = false;
                }

                strChild2 = "Color TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild2Group(strChild1, strChild2))
                {
                    gbox_ColorDefect1_Center.Enabled = false;
                    gbox_ColorDefect2_Center.Enabled = false;
                    gbox_ColorDefect3_Center.Enabled = false;
                    gbox_ColorDefect4_Center.Enabled = false;
                    gbox_ColorDefect5_Center.Enabled = false;
                    gbox_ColorDefect1_Side.Enabled = false;
                    gbox_ColorDefect2_Side.Enabled = false;
                    gbox_ColorDefect3_Side.Enabled = false;
                    gbox_ColorDefect4_Side.Enabled = false;
                    gbox_ColorDefect5_Side.Enabled = false;
                    dgd_CenterColor.Enabled = false;
                    dgd_SideColor.Enabled = false;
                }

                strChild2 = "Position TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild2Group(strChild1, strChild2))
                {
                    dgd_Position.Enabled = false;

                }

                strChild2 = "Pin 1 TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild2Group(strChild1, strChild2))
                {
                    group_Pin1Setting.Enabled = false;

                }

                strChild2 = "PH TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild2Group(strChild1, strChild2))
                {
                    group_PHSetting.Enabled = false;

                }

                strChild2 = "Orient TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild2Group(strChild1, strChild2))
                {
                    group_OrientScoreSetting.Enabled = false;

                }

                strChild2 = "Save Button";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild2Group(strChild1, strChild2))
                {
                    btn_Save.Enabled = false;
                }

                strChild2 = "Package Size Offset Setting";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetOrientationChild2Group(strChild1, strChild2))
                {
                   btn_PackageOffset.Enabled = false;
                }
                else
                {
                    btn_PackageOffset.Enabled = true;
                }
            }
            else
            {
                strChild2 = "Package TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild2Group(strChild1, strChild2))
                {
                    group_GeneralDefectSetting.Enabled = false;
                    group_ChippedOff.Enabled = false;
                    group_MoldFlash.Enabled = false;
                    group_Crack.Enabled = false;
                    group_GeneralDefectSetting_Side.Enabled = false;
                    group_ChippedOff_Side.Enabled = false;
                    group_MoldFlash_Side.Enabled = false;
                    group_Crack_Side.Enabled = false;
                    dgd_PackageSetting.Enabled = false;
                    chk_SetToAllSideROI.Enabled = false;
                }

                strChild2 = "Position TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild2Group(strChild1, strChild2))
                {
                    dgd_Position.Enabled = false;

                }

                strChild2 = "Pin 1 TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild2Group(strChild1, strChild2))
                {
                    group_Pin1Setting.Enabled = false;

                }

                strChild2 = "PH TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild2Group(strChild1, strChild2))
                {
                    group_PHSetting.Enabled = false;

                }

                strChild2 = "Orient TabPage";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild2Group(strChild1, strChild2))
                {
                    group_OrientScoreSetting.Enabled = false;

                }

                strChild2 = "Save Button";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild2Group(strChild1, strChild2))
                {
                    btn_Save.Enabled = false;
                }

                strChild2 = "Package Size Offset Setting";
                if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetPadChild2Group(strChild1, strChild2))
                {
                    btn_PackageOffset.Enabled = false;
                }
                else
                {
                    btn_PackageOffset.Enabled = true;
                }
            }
        }
        private void AddRowsToTable()
        {
            m_arrRowSettingName.Clear();

            m_arrRowSettingName.Add(RowSettingName.PackageSize);
            m_arrRowSettingName.Add(RowSettingName.XDimension);
            m_arrRowSettingName.Add(RowSettingName.YDimension);
            if (m_smVisionInfo.g_strVisionName.Contains("5S") && m_smVisionInfo.g_blnCheck4Sides)
                m_arrRowSettingName.Add(RowSettingName.ZDimension);

            if (m_smVisionInfo.g_strVisionName.Contains("5S"))
            {
                if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides && m_smVisionInfo.g_arrPad[1].ref_blnWantIndividualSideThickness)
                {
                    m_arrRowSettingName.Add(RowSettingName.TopThickness);
                    m_arrRowSettingName.Add(RowSettingName.RightThickness);
                    m_arrRowSettingName.Add(RowSettingName.BottomThickness);
                    m_arrRowSettingName.Add(RowSettingName.LeftThickness);
                }
            }

            if (m_smVisionInfo.g_arrPad[0].ref_blnUseDetailDefectCriteria)
            {
                m_arrRowSettingName.Add(RowSettingName.CenterPackage);
                m_arrRowSettingName.Add(RowSettingName.CenterPkg_ChippedOffArea);
                m_arrRowSettingName.Add(RowSettingName.CenterPkg_ScratchesLength);
                m_arrRowSettingName.Add(RowSettingName.CenterPkg_ScratchesArea);
                m_arrRowSettingName.Add(RowSettingName.CenterPkg_ContaminationArea);
                m_arrRowSettingName.Add(RowSettingName.CenterPkg_ContaminationTotalArea);
                m_arrRowSettingName.Add(RowSettingName.CenterPkg_ContaminationLength);
                m_arrRowSettingName.Add(RowSettingName.CenterPkg_MoldFlashArea);
                m_arrRowSettingName.Add(RowSettingName.CenterPkg_VoidLength);
                m_arrRowSettingName.Add(RowSettingName.CenterPkg_VoidArea);
                m_arrRowSettingName.Add(RowSettingName.CenterPkg_CrackLength);
                m_arrRowSettingName.Add(RowSettingName.CenterPkg_CrackArea);

                if (m_smVisionInfo.g_blnCheck4Sides)
                {
                    m_arrRowSettingName.Add(RowSettingName.SidePackage);
                    m_arrRowSettingName.Add(RowSettingName.SidePkg_ChippedOffArea);
                    m_arrRowSettingName.Add(RowSettingName.SidePkg_ScratchesLength);
                    m_arrRowSettingName.Add(RowSettingName.SidePkg_ScratchesArea);
                    m_arrRowSettingName.Add(RowSettingName.SidePkg_ContaminationArea);
                    m_arrRowSettingName.Add(RowSettingName.SidePkg_ContaminationTotalArea);
                    m_arrRowSettingName.Add(RowSettingName.SidePkg_ContaminationLength);
                    m_arrRowSettingName.Add(RowSettingName.SidePkg_MoldFlashArea);
                    m_arrRowSettingName.Add(RowSettingName.SidePkg_VoidLength);
                    m_arrRowSettingName.Add(RowSettingName.SidePkg_VoidArea);
                    m_arrRowSettingName.Add(RowSettingName.SidePkg_CrackLength);
                    m_arrRowSettingName.Add(RowSettingName.SidePkg_CrackArea);
                }
            }

            dgd_PackageSetting.Rows.Clear();

            for (int i = 0; i < m_arrRowSettingName.Count; i++)
            {   
                switch (m_arrRowSettingName[i])
                {
                    case RowSettingName.PackageSize:
                        {
                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Package 长宽";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Package Size";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "";
                            dgd_PackageSetting.Rows[i].Cells[0].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[0].Style.SelectionBackColor = Color.LightGray;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.LightGray;
                            dgd_PackageSetting.Rows[i].Cells[2].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.LightGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.LightGray;
                            dgd_PackageSetting.Rows[i].Cells[4].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.LightGray;
                        }
                        break;
                    case RowSettingName.XDimension:
                        {
                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "X 尺寸";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "X Dimension";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                        }
                        break;
                    case RowSettingName.YDimension:
                        {
                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Y 尺寸";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Y Dimension";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                        }
                        break;
                    case RowSettingName.ZDimension:
                        {
                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Z 尺寸";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Z Dimension";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                        }
                        break;
                    case RowSettingName.TopThickness:
                        {
                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "上边尺寸";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Top Dimension";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[2].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                        }
                        break;
                    case RowSettingName.RightThickness:
                        {
                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "右边尺寸";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Right Dimension";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[2].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                        }
                        break;
                    case RowSettingName.BottomThickness:
                        {
                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "下边尺寸";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Bottom Dimension";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[2].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                        }
                        break;
                    case RowSettingName.LeftThickness:
                        {
                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "左边尺寸";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Left Dimension";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[2].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "---";
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                        }
                        break;
                    case RowSettingName.CenterPackage:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "中部 Package";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Center Package";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "";
                            dgd_PackageSetting.Rows[i].Cells[0].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[0].Style.SelectionBackColor = Color.LightGray;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.LightGray;
                            dgd_PackageSetting.Rows[i].Cells[2].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.LightGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.LightGray;
                            dgd_PackageSetting.Rows[i].Cells[4].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.LightGray;
                        }
                        break;
                    case RowSettingName.CenterPkg_ScratchesLength:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "刮伤长度";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Scratches Length";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.CenterPkg_ScratchesArea:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "刮伤面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Scratches Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.CenterPkg_ChippedOffArea:
                        {
                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Chipped Off 面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Chipped Off Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.CenterPkg_ContaminationLength:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "污染长度";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Cont. Length";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.CenterPkg_ContaminationArea:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "污染面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Cont. Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.CenterPkg_ContaminationTotalArea:
                        {
                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "污染总面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Cont. Total Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.CenterPkg_MoldFlashArea:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Mold Flash 面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Mold Flash Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.CenterPkg_VoidLength:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Void 长度";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Void Length";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.CenterPkg_VoidArea:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Void 面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Void Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.CenterPkg_CrackLength:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "裂缝长度";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Crack Length";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.CenterPkg_CrackArea:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "裂缝面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Crack Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.SidePackage:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "侧边 Package";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Side Package";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "";
                            dgd_PackageSetting.Rows[i].Cells[0].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[0].Style.SelectionBackColor = Color.LightGray;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.LightGray;
                            dgd_PackageSetting.Rows[i].Cells[2].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.LightGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.LightGray;
                            dgd_PackageSetting.Rows[i].Cells[4].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.LightGray;
                        }
                        break;
                    case RowSettingName.SidePkg_ScratchesLength:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "刮伤长度";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Scratches Length";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.SidePkg_ScratchesArea:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "刮伤面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Scratches Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.SidePkg_ChippedOffArea:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Chipped Off 面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Chipped Off Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.SidePkg_ContaminationLength:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "污染长度";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Cont. Length";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.SidePkg_ContaminationArea:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "污染面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Cont. Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.SidePkg_ContaminationTotalArea:
                        {
                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "污染总面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Cont. Total Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.SidePkg_MoldFlashArea:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Mold Flash 面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Mold Flash Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.SidePkg_VoidLength:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Void长度";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Void Length";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.SidePkg_VoidArea:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Void面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Void Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.SidePkg_CrackLength:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "裂缝长度";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Crack Length";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                    case RowSettingName.SidePkg_CrackArea:
                        {

                            dgd_PackageSetting.Rows.Add();
                            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "裂缝面积";
                            else
                                dgd_PackageSetting.Rows[i].Cells[0].Value = "Crack Area";
                            dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                            dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                            dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                            dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                        }
                        break;
                }
            }
        }

        private void UpdateGUI()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_DisplayResult.Checked = Convert.ToBoolean(subkey1.GetValue("CurrentDisplayResult_MoTolerance", false));

            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if (!tab_VisionControl.TabPages.Contains(tp_Orient))
                    tab_VisionControl.Controls.Remove(tp_Orient);

                float fScore;
                fScore = m_smVisionInfo.g_objPadOrient.GetMinScore() * 100;
                if (fScore < 0)
                    lbl_Score.Text = "----";
                else
                    lbl_Score.Text = fScore.ToString("f2");

                if (Convert.ToInt32(m_smVisionInfo.g_objPadOrient.ref_fMinScore * 100) != 0)
                    trackBar_OrientTolerance.Value = Convert.ToInt32(m_smVisionInfo.g_objPadOrient.ref_fMinScore * 100);

                txt_OrientTolerance.Text = trackBar_OrientTolerance.Value.ToString();
            }
            else
            {
                if (tab_VisionControl.TabPages.Contains(tp_Orient))
                    tab_VisionControl.Controls.Remove(tp_Orient);
            }

            txt_BlackAreaArea.Text = m_smVisionInfo.g_objPositioning.ref_intPHBlackArea.ToString();
            lbl_PHBlobBlackArea.Text = m_smVisionInfo.g_objPositioning.ref_intPHBlobBlackArea.ToString();
            if (m_smVisionInfo.g_blnWantCheckPH)
            {
                if (!tab_VisionControl.TabPages.Contains(tp_PH))
                    tab_VisionControl.Controls.Add(tp_PH);
            }
            else
            {
                if (tab_VisionControl.TabPages.Contains(tp_PH))
                    tab_VisionControl.Controls.Remove(tp_PH);
            }

            if ((m_smCustomizeInfo.g_intWantPackage & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if (m_smVisionInfo.g_arrPad[0].ref_blnUseDetailDefectCriteria)
                {
                    if (tab_VisionControl.TabPages.Contains(tp_PackageSimple))
                    {
                        tab_VisionControl.Controls.Remove(tp_PackageSimple);
                        tab_VisionControl.Controls.Remove(tp_PackageSimple2);
                    }

                    if (tab_VisionControl.TabPages.Contains(tp_PackageSimple_Side))
                    {
                        tab_VisionControl.Controls.Remove(tp_PackageSimple_Side);
                        tab_VisionControl.Controls.Remove(tp_PackageSimple_Side2);
                    }
                }
                else
                {
                    //if (tab_VisionControl.TabPages.Contains(tp_Package))
                    //    tab_VisionControl.Controls.Remove(tp_Package);

                    if (m_smVisionInfo.g_arrPad.Length < 2 || !m_smVisionInfo.g_blnCheck4Sides)
                    {
                        if (tab_VisionControl.TabPages.Contains(tp_PackageSimple_Side))
                        {
                            tab_VisionControl.Controls.Remove(tp_PackageSimple_Side);
                            tab_VisionControl.Controls.Remove(tp_PackageSimple_Side2);
                        }
                    }
                    else
                    {
                        if (m_smVisionInfo.g_arrPad[1].ref_blnSeperateChippedOffDefectSetting)
                            group_ChippedOff_Side.Visible = true;
                        else
                            group_ChippedOff_Side.Visible = false;

                        if (m_smVisionInfo.g_arrPad[1].ref_blnSeperateMoldFlashDefectSetting)
                            group_MoldFlash_Side.Visible = true;
                        else
                            group_MoldFlash_Side.Visible = false;

                        if (m_smVisionInfo.g_arrPad[1].ref_blnSeperateCrackDefectSetting)
                            group_Crack_Side.Visible = true;
                        else
                            group_Crack_Side.Visible = false;

                        if (m_smVisionInfo.g_arrPad[1].ref_blnSeperateForeignMaterialDefectSetting)
                            group_ForeignMaterial_Side.Visible = true;
                        else
                        {
                            group_ForeignMaterial_Side.Visible = false;
                            tab_VisionControl.Controls.Remove(tp_PackageSimple_Side2);
                        }

                    }

                    if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateChippedOffDefectSetting)
                        group_ChippedOff.Visible = true;
                    else
                        group_ChippedOff.Visible = false;

                    if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateMoldFlashDefectSetting)
                        group_MoldFlash.Visible = true;
                    else
                        group_MoldFlash.Visible = false;

                    if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateCrackDefectSetting)
                        group_Crack.Visible = true;
                    else
                        group_Crack.Visible = false;

                    if (m_smVisionInfo.g_arrPad[0].ref_blnSeperateForeignMaterialDefectSetting)
                        group_ForeignMaterial.Visible = true;
                    else
                    {
                        group_ForeignMaterial.Visible = false;
                        tab_VisionControl.Controls.Remove(tp_PackageSimple2);
                    }
                }
            }
            else
            {
                if (tab_VisionControl.TabPages.Contains(tp_Package))
                    tab_VisionControl.Controls.Remove(tp_Package);

                if (tab_VisionControl.TabPages.Contains(tp_PackageSimple))
                {
                    tab_VisionControl.Controls.Remove(tp_PackageSimple);
                    tab_VisionControl.Controls.Remove(tp_PackageSimple2);
                }

                if (tab_VisionControl.TabPages.Contains(tp_PackageSimple_Side))
                {
                    tab_VisionControl.Controls.Remove(tp_PackageSimple_Side);
                    tab_VisionControl.Controls.Remove(tp_PackageSimple_Side2);
                }
            }

            if (m_smVisionInfo.g_blnWantPin1)
            {
                if (!tab_VisionControl.TabPages.Contains(tp_Pin1))
                    tab_VisionControl.Controls.Add(tp_Pin1);
            }
            else
            {
                if (tab_VisionControl.TabPages.Contains(tp_Pin1))
                    tab_VisionControl.Controls.Remove(tp_Pin1);
            }
            if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                gbox_ColorDefect1_Center.Visible = false;
                gbox_ColorDefect2_Center.Visible = false;
                gbox_ColorDefect3_Center.Visible = false;
                gbox_ColorDefect4_Center.Visible = false;
                gbox_ColorDefect5_Center.Visible = false;

                List<int> arrColorDefectSkipNo_Center = new List<int>();
                int intTotalColorCount_Center = m_smVisionInfo.g_arrPad[0].GetColorDefectCount(ref arrColorDefectSkipNo_Center);

                if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count == 0)
                {
                    if (tab_VisionControl.TabPages.Contains(tp_CenterColor))
                        tab_VisionControl.TabPages.Remove(tp_CenterColor);
                    if (tab_VisionControl.TabPages.Contains(tp_CenterColor2))
                        tab_VisionControl.TabPages.Remove(tp_CenterColor2);
                    if (tab_VisionControl.TabPages.Contains(tp_CenterColor_Table))
                        tab_VisionControl.TabPages.Remove(tp_CenterColor_Table);
                }
                if (tab_VisionControl.TabPages.Contains(tp_CenterColor))
                    tab_VisionControl.TabPages.Remove(tp_CenterColor);
                if (tab_VisionControl.TabPages.Contains(tp_CenterColor2))
                    tab_VisionControl.TabPages.Remove(tp_CenterColor2);

                //if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count < 2)
                //{
                //    gbox_ColorDefect2_Center.Visible = false;
                //}
                //if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count < 3)
                //{
                //    gbox_ColorDefect3_Center.Visible = false;
                //}
                //if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count < 4)
                //{
                //    if (tab_VisionControl.TabPages.Contains(tp_CenterColor2))
                //        tab_VisionControl.TabPages.Remove(tp_CenterColor2);
                //}
                //if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count < 5)
                //{
                //    gbox_ColorDefect5_Center.Visible = false;
                //}

                for (int intColorThresIndex = 0; intColorThresIndex < m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count; intColorThresIndex++)
                {
                    if (arrColorDefectSkipNo_Center.Contains(intColorThresIndex))
                    {
                        dgd_CenterColor.Rows.Add();
                        dgd_CenterColor.Rows[dgd_CenterColor.RowCount - 1].Visible = false;
                        continue;
                    }

                    dgd_CenterColor.Rows.Add();
                    dgd_CenterColor.Rows[dgd_CenterColor.RowCount - 1].Cells[0].Value = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[intColorThresIndex];
                    dgd_CenterColor.Rows[dgd_CenterColor.RowCount - 1].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                    dgd_CenterColor.Rows[dgd_CenterColor.RowCount - 1].Cells[3].Value = (dgd_CenterColor.Rows[dgd_CenterColor.RowCount - 1].Cells[3] as DataGridViewComboBoxCell).Items[m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionFailCondition(intColorThresIndex)];
                    dgd_CenterColor.Rows[dgd_CenterColor.RowCount - 1].Cells[4].Value = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                    dgd_CenterColor.Rows[dgd_CenterColor.RowCount - 1].Cells[6].Value = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    if (m_smVisionInfo.g_arrPad[0].ref_arrDefectType[intColorThresIndex] == 0)
                    {
                        dgd_CenterColor.Rows[dgd_CenterColor.RowCount - 1].Cells[6].ReadOnly = true;
                        dgd_CenterColor.Rows[dgd_CenterColor.RowCount - 1].Cells[6].Style.BackColor = SystemColors.Control;
                        dgd_CenterColor.Rows[dgd_CenterColor.RowCount - 1].Cells[6].Style.SelectionBackColor = SystemColors.Control;
                    }
                    dgd_CenterColor.Rows[dgd_CenterColor.RowCount - 1].Cells[8].Value = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    dgd_CenterColor.Rows[dgd_CenterColor.RowCount - 1].Cells[10].Value = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);

                    //if (intColorThresIndex == 0)
                    //{
                    //    gbox_ColorDefect1_Center.Visible = true;
                    //    gbox_ColorDefect1_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[intColorThresIndex] + " (Center ROI)";
                    //    txt_ColorDefect1Length_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                    //    txt_ColorDefect1Width_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                    //    txt_ColorDefect1MinArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //    txt_ColorDefect1MinArea_Center.Enabled = (m_smVisionInfo.g_arrPad[0].ref_arrDefectType[intColorThresIndex] == 1);
                    //    txt_ColorDefect1MaxArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //    txt_ColorDefect1TotalArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //}
                    //else if (intColorThresIndex == 1)
                    //{
                    //    gbox_ColorDefect2_Center.Visible = true;
                    //    gbox_ColorDefect2_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[intColorThresIndex] + " (Center ROI)";
                    //    txt_ColorDefect2Length_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                    //    txt_ColorDefect2Width_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                    //    txt_ColorDefect2MinArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //    txt_ColorDefect2MinArea_Center.Enabled = (m_smVisionInfo.g_arrPad[0].ref_arrDefectType[intColorThresIndex] == 1);
                    //    txt_ColorDefect2MaxArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //    txt_ColorDefect2TotalArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //}
                    //else if (intColorThresIndex == 2)
                    //{
                    //    gbox_ColorDefect3_Center.Visible = true;
                    //    gbox_ColorDefect3_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[intColorThresIndex] + " (Center ROI)";
                    //    txt_ColorDefect3Length_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                    //    txt_ColorDefect3Width_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                    //    txt_ColorDefect3MinArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //    txt_ColorDefect3MinArea_Center.Enabled = (m_smVisionInfo.g_arrPad[0].ref_arrDefectType[intColorThresIndex] == 1);
                    //    txt_ColorDefect3MaxArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //    txt_ColorDefect3TotalArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //}
                    //else if (intColorThresIndex == 3)
                    //{
                    //    gbox_ColorDefect4_Center.Visible = true;
                    //    gbox_ColorDefect4_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[intColorThresIndex] + " (Center ROI)";
                    //    txt_ColorDefect4Length_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                    //    txt_ColorDefect4Width_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                    //    txt_ColorDefect4MinArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //    txt_ColorDefect4MinArea_Center.Enabled = (m_smVisionInfo.g_arrPad[0].ref_arrDefectType[intColorThresIndex] == 1);
                    //    txt_ColorDefect4MaxArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //    txt_ColorDefect4TotalArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //}
                    //else if (intColorThresIndex == 4)
                    //{
                    //    gbox_ColorDefect5_Center.Visible = true;
                    //    gbox_ColorDefect5_Center.Text = m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[intColorThresIndex] + " (Center ROI)";
                    //    txt_ColorDefect5Length_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                    //    txt_ColorDefect5Width_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                    //    txt_ColorDefect5MinArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //    txt_ColorDefect5MinArea_Center.Enabled = (m_smVisionInfo.g_arrPad[0].ref_arrDefectType[intColorThresIndex] == 1);
                    //    txt_ColorDefect5MaxArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //    txt_ColorDefect5TotalArea_Center.Text = m_smVisionInfo.g_arrPad[0].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                    //}
                }

                //if (arrColorDefectSkipNo_Center.Contains(0) && arrColorDefectSkipNo_Center.Contains(1) && arrColorDefectSkipNo_Center.Contains(2))
                //{
                //    if (tab_VisionControl.TabPages.Contains(tp_CenterColor))
                //        tab_VisionControl.TabPages.Remove(tp_CenterColor);
                //}
                //if (m_smVisionInfo.g_arrPad[0].ref_arrDefectColor.Count < 4 || (arrColorDefectSkipNo_Center.Contains(3) && arrColorDefectSkipNo_Center.Contains(4)))
                //{
                //    if (tab_VisionControl.TabPages.Contains(tp_CenterColor2))
                //        tab_VisionControl.TabPages.Remove(tp_CenterColor2);
                //}

                if (m_smVisionInfo.g_arrPad.Length < 2 || (m_smVisionInfo.g_arrPad.Length > 1 && !m_smVisionInfo.g_blnCheck4Sides))
                {
                    if (tab_VisionControl.TabPages.Contains(tp_SideColor))
                        tab_VisionControl.TabPages.Remove(tp_SideColor);
                    if (tab_VisionControl.TabPages.Contains(tp_SideColor2))
                        tab_VisionControl.TabPages.Remove(tp_SideColor2);
                    if (tab_VisionControl.TabPages.Contains(tp_SideColor_Table))
                        tab_VisionControl.TabPages.Remove(tp_SideColor_Table);
                }
                else
                {
                    //int intSelectedROI = 1;
                    //if (m_smVisionInfo.g_intSelectedROI > 1)
                    //    intSelectedROI = m_smVisionInfo.g_intSelectedROI;

                    //if (!tab_VisionControl.TabPages.Contains(tp_SideColor))
                    //    tab_VisionControl.TabPages.Add(tp_SideColor);
                    //if (!tab_VisionControl.TabPages.Contains(tp_SideColor2))
                    //    tab_VisionControl.TabPages.Add(tp_SideColor2);
                    if (!tab_VisionControl.TabPages.Contains(tp_SideColor_Table))
                        tab_VisionControl.TabPages.Add(tp_SideColor_Table);

                    if (tab_VisionControl.TabPages.Contains(tp_SideColor))
                        tab_VisionControl.TabPages.Remove(tp_SideColor);
                    if (tab_VisionControl.TabPages.Contains(tp_SideColor2))
                        tab_VisionControl.TabPages.Remove(tp_SideColor2);

                    //if (!gbox_ColorDefect1_Side.Visible)
                    //    gbox_ColorDefect1_Side.Visible = true;
                    //if (!gbox_ColorDefect2_Side.Visible)
                    //    gbox_ColorDefect2_Side.Visible = true;
                    //if (!gbox_ColorDefect3_Side.Visible)
                    //    gbox_ColorDefect3_Side.Visible = true;
                    //if (!gbox_ColorDefect4_Side.Visible)
                    //    gbox_ColorDefect4_Side.Visible = true;
                    //if (!gbox_ColorDefect5_Side.Visible)
                    //    gbox_ColorDefect5_Side.Visible = true;

                    gbox_ColorDefect1_Side.Visible = false;
                    gbox_ColorDefect2_Side.Visible = false;
                    gbox_ColorDefect3_Side.Visible = false;
                    gbox_ColorDefect4_Side.Visible = false;
                    gbox_ColorDefect5_Side.Visible = false;

                    List<int> arrColorDefectSkipNo = new List<int>();
                    for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        bool blnRowVisible = true;

                        dgd_SideColor.Rows.Add();
                        if (i == 1)
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[0].Value = GetColorTabName(i);
                        else if (i == 2)
                        {
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[0].Value = GetColorTabName(i);

                            if ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0)
                            {
                                blnRowVisible = false;
                            }
                        }
                        else if (i == 3)
                        {
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[0].Value = GetColorTabName(i);

                            if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x04) > 0))
                            {
                                blnRowVisible = false;
                            }
                        }
                        else if (i == 4)
                        {
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[0].Value = GetColorTabName(i);

                            if (((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x02) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x04) > 0) || ((m_smVisionInfo.g_arrPad[i].ref_intColorPadGroupIndex & 0x08) > 0))
                            {
                                blnRowVisible = false;
                            }
                        }

                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Visible = blnRowVisible;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].ReadOnly = true;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[0].Style.BackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[0].Style.SelectionBackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[1].Style.BackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[1].Style.SelectionBackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[2].Style.BackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[2].Style.SelectionBackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[3].Style.BackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[3].Style.SelectionBackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[4].Style.BackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[4].Style.SelectionBackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[5].Style.BackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[5].Style.SelectionBackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[6].Style.BackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[6].Style.SelectionBackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[7].Style.BackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[7].Style.SelectionBackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[8].Style.BackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[8].Style.SelectionBackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[9].Style.BackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[9].Style.SelectionBackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[10].Style.BackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[10].Style.SelectionBackColor = SystemColors.Control;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[11].Value = i;
                        dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[12].Value = -1;

                        int intTotalColorCount = m_smVisionInfo.g_arrPad[i].GetColorDefectCount(ref arrColorDefectSkipNo);

                        if (m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count == 0)
                        {
                            if (tab_VisionControl.TabPages.Contains(tp_SideColor))
                                tab_VisionControl.TabPages.Remove(tp_SideColor);
                            if (tab_VisionControl.TabPages.Contains(tp_SideColor2))
                                tab_VisionControl.TabPages.Remove(tp_SideColor2);
                        }
                        //if (m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColor.Count < 2)
                        //{
                        //    gbox_ColorDefect2_Side.Visible = false;
                        //}
                        //if (m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColor.Count < 3)
                        //{
                        //    gbox_ColorDefect3_Side.Visible = false;
                        //}
                        //if (m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColor.Count < 4)
                        //{
                        //    if (tab_VisionControl.TabPages.Contains(tp_SideColor2))
                        //        tab_VisionControl.TabPages.Remove(tp_SideColor2);
                        //}
                        //if (m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColor.Count < 5)
                        //{
                        //    gbox_ColorDefect5_Side.Visible = false;
                        //}

                        for (int intColorThresIndex = 0; intColorThresIndex < m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count; intColorThresIndex++)
                        {
                            if (arrColorDefectSkipNo.Contains(intColorThresIndex))
                            {
                                dgd_SideColor.Rows.Add();
                                dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Visible = false;
                                dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[11].Value = i;
                                dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[12].Value = intColorThresIndex;
                                continue;
                            }

                            dgd_SideColor.Rows.Add();
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Visible = blnRowVisible;
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[0].Value = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[intColorThresIndex];
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[2].Value = m_smVisionInfo.g_arrPad[i].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[3].Value = (dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[3] as DataGridViewComboBoxCell).Items[m_smVisionInfo.g_arrPad[i].GetColorDefectInspectionFailCondition(intColorThresIndex)];
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[4].Value = m_smVisionInfo.g_arrPad[i].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[6].Value = m_smVisionInfo.g_arrPad[i].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            if (m_smVisionInfo.g_arrPad[i].ref_arrDefectType[intColorThresIndex] == 0)
                            {
                                dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[6].ReadOnly = true;
                                dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[6].Style.BackColor = SystemColors.Control;
                                dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[6].Style.SelectionBackColor = SystemColors.Control;
                            }
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[8].Value = m_smVisionInfo.g_arrPad[i].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[10].Value = m_smVisionInfo.g_arrPad[i].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[11].Value = i;
                            dgd_SideColor.Rows[dgd_SideColor.RowCount - 1].Cells[12].Value = intColorThresIndex;

                            //if (intColorThresIndex == 0)
                            //{
                            //    gbox_ColorDefect1_Side.Visible = true;
                            //    gbox_ColorDefect1_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[intColorThresIndex] + " (Side ROI)";
                            //    txt_ColorDefect1Length_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            //    txt_ColorDefect1Width_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            //    txt_ColorDefect1MinArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //    txt_ColorDefect1MinArea_Side.Enabled = (m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectType[intColorThresIndex] == 1);
                            //    txt_ColorDefect1MaxArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //    txt_ColorDefect1TotalArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //}
                            //else if (intColorThresIndex == 1)
                            //{
                            //    gbox_ColorDefect2_Side.Visible = true;
                            //    gbox_ColorDefect2_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[intColorThresIndex] + " (Side ROI)";
                            //    txt_ColorDefect2Length_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            //    txt_ColorDefect2Width_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            //    txt_ColorDefect2MinArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //    txt_ColorDefect2MinArea_Side.Enabled = (m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectType[intColorThresIndex] == 1);
                            //    txt_ColorDefect2MaxArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //    txt_ColorDefect2TotalArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //}
                            //else if (intColorThresIndex == 2)
                            //{
                            //    gbox_ColorDefect3_Side.Visible = true;
                            //    gbox_ColorDefect3_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[intColorThresIndex] + " (Side ROI)";
                            //    txt_ColorDefect3Length_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            //    txt_ColorDefect3Width_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            //    txt_ColorDefect3MinArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //    txt_ColorDefect3MinArea_Side.Enabled = (m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectType[intColorThresIndex] == 1);
                            //    txt_ColorDefect3MaxArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //    txt_ColorDefect3TotalArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //}
                            //else if (intColorThresIndex == 3)
                            //{
                            //    gbox_ColorDefect4_Side.Visible = true;
                            //    gbox_ColorDefect4_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[intColorThresIndex] + " (Side ROI)";
                            //    txt_ColorDefect4Length_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            //    txt_ColorDefect4Width_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            //    txt_ColorDefect4MinArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //    txt_ColorDefect4MinArea_Side.Enabled = (m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectType[intColorThresIndex] == 1);
                            //    txt_ColorDefect4MaxArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //    txt_ColorDefect4TotalArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //}
                            //else if (intColorThresIndex == 4)
                            //{
                            //    gbox_ColorDefect5_Side.Visible = true;
                            //    gbox_ColorDefect5_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[intColorThresIndex] + " (Side ROI)";
                            //    txt_ColorDefect5Length_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            //    txt_ColorDefect5Width_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            //    txt_ColorDefect5MinArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //    txt_ColorDefect5MinArea_Side.Enabled = (m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectType[intColorThresIndex] == 1);
                            //    txt_ColorDefect5MaxArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //    txt_ColorDefect5TotalArea_Side.Text = m_smVisionInfo.g_arrPad[intSelectedROI].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            //}
                        }

                        //if (arrColorDefectSkipNo.Contains(0) && arrColorDefectSkipNo.Contains(1) && arrColorDefectSkipNo.Contains(2))
                        //{
                        //    if (tab_VisionControl.TabPages.Contains(tp_SideColor))
                        //        tab_VisionControl.TabPages.Remove(tp_SideColor);
                        //}
                        //if (m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColor.Count < 4 || (arrColorDefectSkipNo.Contains(3) && arrColorDefectSkipNo.Contains(4)))
                        //{
                        //    if (tab_VisionControl.TabPages.Contains(tp_SideColor2))
                        //        tab_VisionControl.TabPages.Remove(tp_SideColor2);
                        //}
                    }
                }

            }
            else
            {
                if (tab_VisionControl.TabPages.Contains(tp_CenterColor))
                    tab_VisionControl.TabPages.Remove(tp_CenterColor);
                if (tab_VisionControl.TabPages.Contains(tp_CenterColor2))
                    tab_VisionControl.TabPages.Remove(tp_CenterColor2);
                if (tab_VisionControl.TabPages.Contains(tp_SideColor))
                    tab_VisionControl.TabPages.Remove(tp_SideColor);
                if (tab_VisionControl.TabPages.Contains(tp_SideColor2))
                    tab_VisionControl.TabPages.Remove(tp_SideColor2);
                if (tab_VisionControl.TabPages.Contains(tp_CenterColor_Table))
                    tab_VisionControl.TabPages.Remove(tp_CenterColor_Table);
                if (tab_VisionControl.TabPages.Contains(tp_SideColor_Table))
                    tab_VisionControl.TabPages.Remove(tp_SideColor_Table);
            }

            UpdateUnitDisplay();
            AddRowsToTable();
            // Add row to table
            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                UpdatePositionOrient();
            else
            {
                if (m_smVisionInfo.g_blnCheckPad)
                    UpdatePosition(); //m_intPadIndex
            }

            // Update Pin 
            if (m_smVisionInfo.g_arrPin1 != null)
            {
                float fSetValue = Math.Max(1, (int)(m_smVisionInfo.g_arrPin1[0].GetMinScoreSetting(0) * 100));
                float fResultValue = Math.Max(0, (m_smVisionInfo.g_arrPin1[0].GetResultScore(0) * 100));

                trackBar_Pin1Tolerance.Value = Math.Max(1, (int)(m_smVisionInfo.g_arrPin1[0].GetMinScoreSetting(0) * 100));
                txt_Pin1Tolerance.Text = trackBar_Pin1Tolerance.Value.ToString();
                lbl_Pin1Score.Text = Math.Max(0, (m_smVisionInfo.g_arrPin1[0].GetResultScore(0) * 100)).ToString("F2");

                if (fResultValue < fSetValue)
                {
                    lbl_Pin1Score.BackColor = Color.Red;
                    lbl_Pin1Score.ForeColor = Color.Yellow;
                }
            }

            UpdateSettingGUI();         // Update table's setting column with setting value
            UpdateInfo();
            ViewOrHideResultColumn(chk_DisplayResult.Checked);
            m_smVisionInfo.PR_TL_UpdateInfo2 = false;
        }
        private string GetColorTabName(int intPadIndex)
        {
            string strName = "";
            if (m_smVisionInfo.g_arrPad[intPadIndex].ref_intColorPadGroupIndex == 0x1E)
            {
                strName = "All Side";
            }
            else
            {
                if ((m_smVisionInfo.g_arrPad[intPadIndex].ref_intColorPadGroupIndex & 0x02) > 0)
                {

                    if (strName == "")
                    {
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            strName += "Top";
                        else
                            strName += "上";
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
                            strName += "Right";
                        else
                            strName += "右";
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
                            strName += "Bottom";
                        else
                            strName += "下";
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
                            strName += "Left";
                        else
                            strName += "左";
                    }
                    else
                    {
                        if (m_smCustomizeInfo.g_intLanguageCulture == 1)
                            strName += " Left";
                        else
                            strName += " 左";
                    }
                }
            }
            return strName;
        }
        private void UpdateSideColorGUI()
        {
            if ((m_smCustomizeInfo.g_intUseColorCamera & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            {
                if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
                {
                    List<int> arrColorDefectSkipNo = new List<int>();
                    int intRowIndex = 0;
                    for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    {
                        intRowIndex++;
                        int intTotalColorCount = m_smVisionInfo.g_arrPad[i].GetColorDefectCount(ref arrColorDefectSkipNo);
                        
                        for (int intColorThresIndex = 0; intColorThresIndex < m_smVisionInfo.g_arrPad[i].ref_arrDefectColor.Count; intColorThresIndex++)
                        {
                            if (arrColorDefectSkipNo.Contains(intColorThresIndex))
                            {
                                intRowIndex++;
                                continue;
                            }
                            
                            dgd_SideColor.Rows[intRowIndex].Cells[0].Value = m_smVisionInfo.g_arrPad[i].ref_arrDefectColorThresName[intColorThresIndex];
                            dgd_SideColor.Rows[intRowIndex].Cells[2].Value = m_smVisionInfo.g_arrPad[i].GetColorDefectInspectionWidthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            dgd_SideColor.Rows[intRowIndex].Cells[3].Value = (dgd_SideColor.Rows[intRowIndex].Cells[3] as DataGridViewComboBoxCell).Items[m_smVisionInfo.g_arrPad[i].GetColorDefectInspectionFailCondition(intColorThresIndex)];
                            dgd_SideColor.Rows[intRowIndex].Cells[4].Value = m_smVisionInfo.g_arrPad[i].GetColorDefectInspectionLengthLimit(1, intColorThresIndex).ToString("F" + m_intDecimal);
                            dgd_SideColor.Rows[intRowIndex].Cells[6].Value = m_smVisionInfo.g_arrPad[i].GetColorDefectInspectionMinAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            dgd_SideColor.Rows[intRowIndex].Cells[8].Value = m_smVisionInfo.g_arrPad[i].GetColorDefectInspectionMaxAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            dgd_SideColor.Rows[intRowIndex].Cells[10].Value = m_smVisionInfo.g_arrPad[i].GetColorDefectInspectionTotalAreaLimit(1, intColorThresIndex).ToString("F" + m_intDecimal2);
                            intRowIndex++;
                        }

                    }
                }

            }
        }
        private void UpdatePosition()
        {
            dgd_Position.Rows.Clear();

            float Angle = 0, XTolerance = 0, YTolerance = 0;
            m_smVisionInfo.g_arrPad[0].GetPositionResult(ref Angle, ref XTolerance, ref YTolerance);

            dgd_Position.Rows.Add();
            dgd_Position.Rows[0].Cells[0].Value = "Center Pad";
            dgd_Position.Rows[0].Cells[1].Value = "";
            dgd_Position.Rows[0].Cells[2].Value = "";
            dgd_Position.Rows[0].Cells[0].Style.BackColor = dgd_Position.Rows[0].Cells[0].Style.SelectionBackColor = Color.LightGray;
            dgd_Position.Rows[0].Cells[1].Style.BackColor = dgd_Position.Rows[0].Cells[1].Style.SelectionBackColor = Color.LightGray;
            dgd_Position.Rows[0].Cells[2].Style.BackColor = dgd_Position.Rows[0].Cells[2].Style.SelectionBackColor = Color.LightGray;


            dgd_Position.Rows.Add();
            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                dgd_Position.Rows[1].Cells[0].Value = "角度";
            else
                dgd_Position.Rows[1].Cells[0].Value = "Angle";
            dgd_Position.Rows[1].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fAngleTolerance.ToString("f4");
            dgd_Position.Rows[1].Cells[2].Value = Angle.ToString("f4");
            //dgd_Position.Rows[1].Cells[0].Style.BackColor = dgd_Position.Rows[1].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //dgd_Position.Rows[1].Cells[1].Style.BackColor = dgd_Position.Rows[1].Cells[1].Style.SelectionBackColor = Color.LightGray;
            if (Math.Abs(Angle) >= m_smVisionInfo.g_arrPad[0].ref_fAngleTolerance)
            {
                dgd_Position.Rows[1].Cells[2].Style.ForeColor = dgd_Position.Rows[1].Cells[2].Style.SelectionForeColor = Color.Yellow;
                dgd_Position.Rows[1].Cells[2].Style.BackColor = dgd_Position.Rows[1].Cells[2].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Position.Rows[1].Cells[2].Style.ForeColor = dgd_Position.Rows[1].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[1].Cells[2].Style.BackColor = dgd_Position.Rows[1].Cells[2].Style.SelectionBackColor = Color.Lime;
            }

            dgd_Position.Rows.Add();
            dgd_Position.Rows[2].Cells[0].Value = "X Tol.(mm)";
            dgd_Position.Rows[2].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fXTolerance.ToString("f4");
            dgd_Position.Rows[2].Cells[2].Value = XTolerance.ToString("f4");
            //dgd_Position.Rows[2].Cells[0].Style.BackColor = dgd_Position.Rows[2].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //dgd_Position.Rows[2].Cells[1].Style.BackColor = dgd_Position.Rows[2].Cells[1].Style.SelectionBackColor = Color.LightGray;

            if (Math.Abs(XTolerance) >= m_smVisionInfo.g_arrPad[0].ref_fXTolerance)
            {
                dgd_Position.Rows[2].Cells[2].Style.ForeColor = dgd_Position.Rows[2].Cells[2].Style.SelectionForeColor = Color.Yellow;
                dgd_Position.Rows[2].Cells[2].Style.BackColor = dgd_Position.Rows[2].Cells[2].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Position.Rows[2].Cells[2].Style.ForeColor = dgd_Position.Rows[2].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[2].Cells[2].Style.BackColor = dgd_Position.Rows[2].Cells[2].Style.SelectionBackColor = Color.Lime;
            }



            dgd_Position.Rows.Add();
            dgd_Position.Rows[3].Cells[0].Value = "Y Tol.(mm)";
            dgd_Position.Rows[3].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fYTolerance.ToString("f4");
            dgd_Position.Rows[3].Cells[2].Value = YTolerance.ToString("f4");
            //dgd_Position.Rows[3].Cells[0].Style.BackColor = dgd_Position.Rows[3].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //dgd_Position.Rows[3].Cells[1].Style.BackColor = dgd_Position.Rows[3].Cells[1].Style.SelectionBackColor = Color.LightGray;
            if (Math.Abs(YTolerance) >= m_smVisionInfo.g_arrPad[0].ref_fYTolerance)
            {
                dgd_Position.Rows[3].Cells[2].Style.ForeColor = dgd_Position.Rows[3].Cells[2].Style.SelectionForeColor = Color.Yellow;
                dgd_Position.Rows[3].Cells[2].Style.BackColor = dgd_Position.Rows[3].Cells[2].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Position.Rows[3].Cells[2].Style.ForeColor = dgd_Position.Rows[3].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[3].Cells[2].Style.BackColor = dgd_Position.Rows[3].Cells[2].Style.SelectionBackColor = Color.Lime;
            }

            //if (m_smVisionInfo.g_blnCheck4Sides)
            //{
            //    m_smVisionInfo.g_arrPad[1].GetPositionResult(ref Angle, ref XTolerance, ref YTolerance);

            //    dgd_Position.Rows.Add();
            //    dgd_Position.Rows[4].Cells[0].Value = "Side Pad";
            //    dgd_Position.Rows[4].Cells[1].Value = "";
            //    dgd_Position.Rows[4].Cells[2].Value = "";
            //    dgd_Position.Rows[4].Cells[0].Style.BackColor = dgd_Position.Rows[4].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //    dgd_Position.Rows[4].Cells[1].Style.BackColor = dgd_Position.Rows[4].Cells[1].Style.SelectionBackColor = Color.LightGray;
            //    dgd_Position.Rows[4].Cells[2].Style.BackColor = dgd_Position.Rows[4].Cells[2].Style.SelectionBackColor = Color.LightGray;



            //    dgd_Position.Rows.Add();
            //    dgd_Position.Rows[5].Cells[0].Value = "Angle";
            //    dgd_Position.Rows[5].Cells[1].Value = m_smVisionInfo.g_arrPad[1].ref_fAngleTolerance.ToString("f4");
            //    dgd_Position.Rows[5].Cells[2].Value = Angle.ToString("f4"); ;
            //    //dgd_Position.Rows[5].Cells[0].Style.BackColor = dgd_Position.Rows[5].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //    //dgd_Position.Rows[5].Cells[1].Style.BackColor = dgd_Position.Rows[5].Cells[1].Style.SelectionBackColor = Color.LightGray;
            //    if (Angle >= m_smVisionInfo.g_arrPad[1].ref_fAngleTolerance)
            //    {
            //        dgd_Position.Rows[5].Cells[2].Style.ForeColor = dgd_Position.Rows[5].Cells[2].Style.SelectionForeColor = Color.Yellow;
            //        dgd_Position.Rows[5].Cells[2].Style.BackColor = dgd_Position.Rows[5].Cells[2].Style.SelectionBackColor = Color.Red;
            //    }
            //    else
            //    {
            //        dgd_Position.Rows[5].Cells[2].Style.ForeColor = dgd_Position.Rows[5].Cells[2].Style.SelectionForeColor = Color.Black;
            //        dgd_Position.Rows[5].Cells[2].Style.BackColor = dgd_Position.Rows[5].Cells[2].Style.SelectionBackColor = Color.Lime;
            //    }



            //    dgd_Position.Rows.Add();
            //    dgd_Position.Rows[6].Cells[0].Value = "X Tol.";
            //    dgd_Position.Rows[6].Cells[1].Value = m_smVisionInfo.g_arrPad[1].ref_fXTolerance.ToString("f4");
            //    dgd_Position.Rows[6].Cells[2].Value = XTolerance.ToString("f4");
            //    //dgd_Position.Rows[6].Cells[0].Style.BackColor = dgd_Position.Rows[6].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //    //dgd_Position.Rows[6].Cells[1].Style.BackColor = dgd_Position.Rows[6].Cells[1].Style.SelectionBackColor = Color.LightGray;

            //    if (XTolerance >= m_smVisionInfo.g_arrPad[1].ref_fXTolerance)
            //    {
            //        dgd_Position.Rows[6].Cells[2].Style.ForeColor = dgd_Position.Rows[6].Cells[2].Style.SelectionForeColor = Color.Yellow;
            //        dgd_Position.Rows[6].Cells[2].Style.BackColor = dgd_Position.Rows[6].Cells[2].Style.SelectionBackColor = Color.Red;
            //    }
            //    else
            //    {
            //        dgd_Position.Rows[6].Cells[2].Style.ForeColor = dgd_Position.Rows[6].Cells[2].Style.SelectionForeColor = Color.Black;
            //        dgd_Position.Rows[6].Cells[2].Style.BackColor = dgd_Position.Rows[6].Cells[2].Style.SelectionBackColor = Color.Lime;
            //    }


            //    dgd_Position.Rows.Add();
            //    dgd_Position.Rows[7].Cells[0].Value = "Y Tol.";
            //    dgd_Position.Rows[7].Cells[1].Value = m_smVisionInfo.g_arrPad[1].ref_fYTolerance.ToString("f4");
            //    dgd_Position.Rows[7].Cells[2].Value = YTolerance.ToString("f4"); ;
            //    //dgd_Position.Rows[7].Cells[0].Style.BackColor = dgd_Position.Rows[7].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //    //dgd_Position.Rows[7].Cells[1].Style.BackColor = dgd_Position.Rows[7].Cells[1].Style.SelectionBackColor = Color.LightGray;

            //    if (YTolerance >= m_smVisionInfo.g_arrPad[1].ref_fYTolerance)
            //    {
            //        dgd_Position.Rows[7].Cells[2].Style.ForeColor = dgd_Position.Rows[7].Cells[2].Style.SelectionForeColor = Color.Yellow;
            //        dgd_Position.Rows[7].Cells[2].Style.BackColor = dgd_Position.Rows[7].Cells[2].Style.SelectionBackColor = Color.Red;
            //    }
            //    else
            //    {
            //        dgd_Position.Rows[7].Cells[2].Style.ForeColor = dgd_Position.Rows[7].Cells[2].Style.SelectionForeColor = Color.Black;
            //        dgd_Position.Rows[7].Cells[2].Style.BackColor = dgd_Position.Rows[7].Cells[2].Style.SelectionBackColor = Color.Lime;
            //    }

            //    if (!m_smVisionInfo.g_arrPad[1].ref_blnPadFound)
            //    {
            //        dgd_Position.Rows[5].Cells[2].Value = "---";
            //        dgd_Position.Rows[5].Cells[2].Style.ForeColor = dgd_Position.Rows[5].Cells[2].Style.SelectionForeColor = Color.Black;
            //        dgd_Position.Rows[5].Cells[2].Style.BackColor = dgd_Position.Rows[5].Cells[2].Style.SelectionBackColor = Color.Lime;
            //        dgd_Position.Rows[6].Cells[2].Value = "---";
            //        dgd_Position.Rows[6].Cells[2].Style.ForeColor = dgd_Position.Rows[6].Cells[2].Style.SelectionForeColor = Color.Black;
            //        dgd_Position.Rows[6].Cells[2].Style.BackColor = dgd_Position.Rows[6].Cells[2].Style.SelectionBackColor = Color.Lime;
            //        dgd_Position.Rows[7].Cells[2].Value = "---";
            //        dgd_Position.Rows[7].Cells[2].Style.ForeColor = dgd_Position.Rows[7].Cells[2].Style.SelectionForeColor = Color.Black;
            //        dgd_Position.Rows[7].Cells[2].Style.BackColor = dgd_Position.Rows[7].Cells[2].Style.SelectionBackColor = Color.Lime;
            //    }
            //}

            if (!m_smVisionInfo.g_arrPad[0].ref_blnPadFound)
            {
                dgd_Position.Rows[1].Cells[2].Value = "---";
                dgd_Position.Rows[1].Cells[2].Style.ForeColor = dgd_Position.Rows[1].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[1].Cells[2].Style.BackColor = dgd_Position.Rows[1].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_Position.Rows[2].Cells[2].Value = "---";
                dgd_Position.Rows[2].Cells[2].Style.ForeColor = dgd_Position.Rows[2].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[2].Cells[2].Style.BackColor = dgd_Position.Rows[2].Cells[2].Style.SelectionBackColor = Color.Lime;
                dgd_Position.Rows[3].Cells[2].Value = "---";
                dgd_Position.Rows[3].Cells[2].Style.ForeColor = dgd_Position.Rows[3].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[3].Cells[2].Style.BackColor = dgd_Position.Rows[3].Cells[2].Style.SelectionBackColor = Color.Lime;
            }

            //if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
            //{
            //    dgd_Position.Rows[0].Visible = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientAngleTolerance;
            //    dgd_Position.Rows[1].Visible = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientXTolerance;
            //    dgd_Position.Rows[2].Visible = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientYTolerance;
            //}
        }
        private void UpdatePositionOrient()
        {
            dgd_Position.Rows.Clear();
            float fAngleResult = Math.Abs(m_smVisionInfo.g_objPadOrient.ref_fDegAngleResult); //GetResultAngle()
            float CenterX = 0;
            float CenterY = 0;
            float fXAfterRotated = 0;
            float fYAfterRotated = 0;
            float fCenterXDiff = 0;
            float fCenterYDiff = 0;
            int intOrientAngle = 0;
            if (m_smVisionInfo.g_intOrientResult[1] < 4)
            {
                switch (m_smVisionInfo.g_intOrientResult[1])
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

            }

            //CenterX = m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX + (float)(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIWidth) / 2;
            //CenterY = m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY + (float)(m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIHeight) / 2;

            ////2020-09-24 ZJYEOH : Should use current angle to rotate template center point because when get center point different, the object center point is based on current angle
            //fXAfterRotated = (float)((CenterX) + ((m_smVisionInfo.g_objPadOrient.ref_fTemplateX - CenterX) * Math.Cos(intOrientAngle * Math.PI / 180)) - //m_smVisionInfo.g_objPadOrient.ref_intRotatedAngle
            //                   ((m_smVisionInfo.g_objPadOrient.ref_fTemplateY - CenterY) * Math.Sin(intOrientAngle * Math.PI / 180)));
            //fYAfterRotated = (float)((CenterY) + ((m_smVisionInfo.g_objPadOrient.ref_fTemplateX - CenterX) * Math.Sin(intOrientAngle * Math.PI / 180)) +
            //                    ((m_smVisionInfo.g_objPadOrient.ref_fTemplateY - CenterY) * Math.Cos(intOrientAngle * Math.PI / 180)));

            fXAfterRotated = m_smVisionInfo.g_objPadOrient.ref_fTemplateX;
            fYAfterRotated = m_smVisionInfo.g_objPadOrient.ref_fTemplateY;
            fCenterXDiff = m_smVisionInfo.g_objPadOrient.GetCenterXDiff(fXAfterRotated, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionX);
            fCenterYDiff = m_smVisionInfo.g_objPadOrient.GetCenterYDiff(fYAfterRotated, m_smVisionInfo.g_arrPadOrientROIs[0].ref_ROIPositionY);

            dgd_Position.Rows.Add();
            dgd_Position.Rows[0].Cells[0].Value = "Center Pad";
            dgd_Position.Rows[0].Cells[1].Value = "";
            dgd_Position.Rows[0].Cells[2].Value = "";
            dgd_Position.Rows[0].Cells[0].Style.BackColor = dgd_Position.Rows[0].Cells[0].Style.SelectionBackColor = Color.LightGray;
            dgd_Position.Rows[0].Cells[1].Style.BackColor = dgd_Position.Rows[0].Cells[1].Style.SelectionBackColor = Color.LightGray;
            dgd_Position.Rows[0].Cells[2].Style.BackColor = dgd_Position.Rows[0].Cells[2].Style.SelectionBackColor = Color.LightGray;


            dgd_Position.Rows.Add();
            if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                dgd_Position.Rows[1].Cells[0].Value = "角度";
            else
                dgd_Position.Rows[1].Cells[0].Value = "Angle";
            dgd_Position.Rows[1].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fAngleTolerance.ToString("f4");
            dgd_Position.Rows[1].Cells[2].Value = fAngleResult.ToString("f4");
            //dgd_Position.Rows[1].Cells[0].Style.BackColor = dgd_Position.Rows[1].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //dgd_Position.Rows[1].Cells[1].Style.BackColor = dgd_Position.Rows[1].Cells[1].Style.SelectionBackColor = Color.LightGray;
            if (Math.Abs(fAngleResult) >= m_smVisionInfo.g_arrPad[0].ref_fAngleTolerance)
            {
                dgd_Position.Rows[1].Cells[2].Style.ForeColor = dgd_Position.Rows[1].Cells[2].Style.SelectionForeColor = Color.Yellow;
                dgd_Position.Rows[1].Cells[2].Style.BackColor = dgd_Position.Rows[1].Cells[2].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Position.Rows[1].Cells[2].Style.ForeColor = dgd_Position.Rows[1].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[1].Cells[2].Style.BackColor = dgd_Position.Rows[1].Cells[2].Style.SelectionBackColor = Color.Lime;
            }

            dgd_Position.Rows.Add();
            dgd_Position.Rows[2].Cells[0].Value = "X Tol.(mm)";
            dgd_Position.Rows[2].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fXTolerance.ToString("f4");
            dgd_Position.Rows[2].Cells[2].Value = fCenterXDiff.ToString("f4");
            //dgd_Position.Rows[2].Cells[0].Style.BackColor = dgd_Position.Rows[2].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //dgd_Position.Rows[2].Cells[1].Style.BackColor = dgd_Position.Rows[2].Cells[1].Style.SelectionBackColor = Color.LightGray;

            if (Math.Abs(fCenterXDiff) >= m_smVisionInfo.g_arrPad[0].ref_fXTolerance)
            {
                dgd_Position.Rows[2].Cells[2].Style.ForeColor = dgd_Position.Rows[2].Cells[2].Style.SelectionForeColor = Color.Yellow;
                dgd_Position.Rows[2].Cells[2].Style.BackColor = dgd_Position.Rows[2].Cells[2].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Position.Rows[2].Cells[2].Style.ForeColor = dgd_Position.Rows[2].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[2].Cells[2].Style.BackColor = dgd_Position.Rows[2].Cells[2].Style.SelectionBackColor = Color.Lime;
            }



            dgd_Position.Rows.Add();
            dgd_Position.Rows[3].Cells[0].Value = "Y Tol.(mm)";
            dgd_Position.Rows[3].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fYTolerance.ToString("f4");
            dgd_Position.Rows[3].Cells[2].Value = fCenterYDiff.ToString("f4");
            //dgd_Position.Rows[3].Cells[0].Style.BackColor = dgd_Position.Rows[3].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //dgd_Position.Rows[3].Cells[1].Style.BackColor = dgd_Position.Rows[3].Cells[1].Style.SelectionBackColor = Color.LightGray;
            if (Math.Abs(fCenterYDiff) >= m_smVisionInfo.g_arrPad[0].ref_fYTolerance)
            {
                dgd_Position.Rows[3].Cells[2].Style.ForeColor = dgd_Position.Rows[3].Cells[2].Style.SelectionForeColor = Color.Yellow;
                dgd_Position.Rows[3].Cells[2].Style.BackColor = dgd_Position.Rows[3].Cells[2].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Position.Rows[3].Cells[2].Style.ForeColor = dgd_Position.Rows[3].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[3].Cells[2].Style.BackColor = dgd_Position.Rows[3].Cells[2].Style.SelectionBackColor = Color.Lime;
            }

            //if (Convert.ToDouble(m_smVisionInfo.g_objPadOrient.ref_fMinScore * 100) > Convert.ToDouble(txt_OrientTolerance.Text))
            //{
            //    dgd_Position.Rows[1].Cells[2].Value = "---";
            //    dgd_Position.Rows[1].Cells[2].Style.ForeColor = dgd_Position.Rows[1].Cells[2].Style.SelectionForeColor = Color.Black;
            //    dgd_Position.Rows[1].Cells[2].Style.BackColor = dgd_Position.Rows[1].Cells[2].Style.SelectionBackColor = Color.Lime;
            //    dgd_Position.Rows[2].Cells[2].Value = "---";
            //    dgd_Position.Rows[2].Cells[2].Style.ForeColor = dgd_Position.Rows[2].Cells[2].Style.SelectionForeColor = Color.Black;
            //    dgd_Position.Rows[2].Cells[2].Style.BackColor = dgd_Position.Rows[2].Cells[2].Style.SelectionBackColor = Color.Lime;
            //    dgd_Position.Rows[3].Cells[2].Value = "---";
            //    dgd_Position.Rows[3].Cells[2].Style.ForeColor = dgd_Position.Rows[3].Cells[2].Style.SelectionForeColor = Color.Black;
            //    dgd_Position.Rows[3].Cells[2].Style.BackColor = dgd_Position.Rows[3].Cells[2].Style.SelectionBackColor = Color.Lime;
            //}

            //dgd_Position.Rows[0].Visible = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientAngleTolerance;
            //dgd_Position.Rows[1].Visible = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientXTolerance;
            //dgd_Position.Rows[2].Visible = m_smVisionInfo.g_objPadOrient.ref_blnWantCheckOrientYTolerance;

        }
        private void UpdateSettingGUI()
        {
            for (int i = 0; i < m_arrRowSettingName.Count; i++)
            {
                switch (m_arrRowSettingName[i])
                {
                    case RowSettingName.XDimension:
                        dgd_PackageSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrPad[0].GetUnitWidthMin(1).ToString("F" + m_intDecimal);
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetUnitWidthMax(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.YDimension:
                        dgd_PackageSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrPad[0].GetUnitHeightMin(1).ToString("F" + m_intDecimal);
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetUnitHeightMax(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.ZDimension:
                        dgd_PackageSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrPad[0].GetUnitThicknessMin(1).ToString("F" + m_intDecimal);
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetUnitThicknessMax(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.CenterPkg_ScratchesLength:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetScratchLengthLimit(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.CenterPkg_ScratchesArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetScratchAreaLimit(1).ToString("F" + m_intDecimal2);
                        break;
                    case RowSettingName.CenterPkg_ChippedOffArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetChipAreaLimit(1).ToString("F" + m_intDecimal2);
                        break;
                    case RowSettingName.CenterPkg_ContaminationLength:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetExtraPadLengthLimit(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.CenterPkg_ContaminationArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetExtraPadMinArea(1).ToString("F" + m_intDecimal2);
                        break;
                    case RowSettingName.CenterPkg_ContaminationTotalArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetTotalExtraPadMinArea(1).ToString("F" + m_intDecimal2);
                        break;
                    case RowSettingName.CenterPkg_MoldFlashArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetMoldFlashAreaLimit(1).ToString("F" + m_intDecimal2);
                        break;
                    case RowSettingName.CenterPkg_VoidLength:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetVoidLengthLimit(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.CenterPkg_VoidArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetVoidAreaLimit(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.CenterPkg_CrackLength:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetCrackLengthLimit(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.CenterPkg_CrackArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[0].GetCrackAreaLimit(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.SidePkg_ScratchesLength:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[1].GetScratchLengthLimit(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.SidePkg_ScratchesArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[1].GetScratchAreaLimit(1).ToString("F" + m_intDecimal2);
                        break;
                    case RowSettingName.SidePkg_ChippedOffArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[1].GetChipAreaLimit(1).ToString("F" + m_intDecimal2);
                        break;
                    case RowSettingName.SidePkg_ContaminationLength:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[1].GetExtraPadLengthLimit(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.SidePkg_ContaminationArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[1].GetExtraPadMinArea(1).ToString("F" + m_intDecimal2);
                        break;
                    case RowSettingName.SidePkg_ContaminationTotalArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[1].GetTotalExtraPadMinArea(1).ToString("F" + m_intDecimal2);
                        break;
                    case RowSettingName.SidePkg_MoldFlashArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[1].GetMoldFlashAreaLimit(1).ToString("F" + m_intDecimal2);
                        break;
                    case RowSettingName.SidePkg_VoidLength:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[1].GetVoidLengthLimit(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.SidePkg_VoidArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[1].GetVoidAreaLimit(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.SidePkg_CrackLength:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[1].GetCrackLengthLimit(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.SidePkg_CrackArea:
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrPad[1].GetCrackAreaLimit(1).ToString("F" + m_intDecimal);
                        break;
                }
            }

            //General
            txt_GeneralVerticalBright.Text = m_smVisionInfo.g_arrPad[0].GetBrightLengthLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralVerticalDark.Text = m_smVisionInfo.g_arrPad[0].GetDarkLengthLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralHorizontalBright.Text = m_smVisionInfo.g_arrPad[0].GetBrightWidthLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralHorizontalDark.Text = m_smVisionInfo.g_arrPad[0].GetDarkWidthLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralAreaBright.Text = m_smVisionInfo.g_arrPad[0].GetBrightAreaLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralAreaDark.Text = m_smVisionInfo.g_arrPad[0].GetDarkAreaLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralTotalAreaBright.Text = m_smVisionInfo.g_arrPad[0].GetBrightTotalAreaLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralTotalAreaDark.Text = m_smVisionInfo.g_arrPad[0].GetDarkTotalAreaLimit(1).ToString("F" + m_intDecimal);
            //txt_GeneralChipAreaBright.Text = m_smVisionInfo.g_arrPad[0].GetBrightLengthLimit(1).ToString("F" + m_intDecimal);
            //txt_GeneralChipAreaDark.Text = m_smVisionInfo.g_arrPad[0].GetBrightLengthLimit(1).ToString("F" + m_intDecimal);

            cbo_MoldFlashBrightDefectFailCondition_Center.SelectedIndex = m_smVisionInfo.g_arrPad[0].ref_intMoldFlashDefectDimensionFailCondition;
            cbo_GeneralBrightDefectFailCondition_Center.SelectedIndex = m_smVisionInfo.g_arrPad[0].ref_intBrightDefectDimensionFailCondition;
            cbo_ForeignMaterialBrightDefectFailCondition_Center.SelectedIndex = m_smVisionInfo.g_arrPad[0].ref_intForeignMaterialBrightDefectDimensionFailCondition;
            cbo_GeneralDarkDefectFailCondition_Center.SelectedIndex = m_smVisionInfo.g_arrPad[0].ref_intDarkDefectDimensionFailCondition;
            cbo_CrackDarkDefectFailCondition_Center.SelectedIndex = m_smVisionInfo.g_arrPad[0].ref_intCrackDarkDefectDimensionFailCondition;

            txt_CrackAreaDark.Text = m_smVisionInfo.g_arrPad[0].GetCrackAreaLimit(1).ToString("F" + m_intDecimal);
            txt_MoldFlashVerticalBright.Text = m_smVisionInfo.g_arrPad[0].GetMoldFlashLengthLimit(1).ToString("F" + m_intDecimal);
            txt_MoldFlashHorizontalBright.Text = m_smVisionInfo.g_arrPad[0].GetMoldFlashWidthLimit(1).ToString("F" + m_intDecimal);
            txt_MoldFlashAreaBright.Text = m_smVisionInfo.g_arrPad[0].GetMoldFlashAreaLimit(1).ToString("F" + m_intDecimal);
            txt_MoldFlashTotalAreaBright.Text = m_smVisionInfo.g_arrPad[0].GetMoldFlashTotalAreaLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralChipAreaBright.Text = m_smVisionInfo.g_arrPad[0].GetBrightChippedOffAreaLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralChipAreaDark.Text = m_smVisionInfo.g_arrPad[0].GetDarkChippedOffAreaLimit(1).ToString("F" + m_intDecimal);
            txt_CrackVerticalDark.Text = m_smVisionInfo.g_arrPad[0].GetDarkVerticalCrackLimit(1).ToString("F" + m_intDecimal);
            txt_CrackHorizontalDark.Text = m_smVisionInfo.g_arrPad[0].GetDarkHorizontalCrackLimit(1).ToString("F" + m_intDecimal);

            txt_ForeignMaterialAreaBright.Text = m_smVisionInfo.g_arrPad[0].GetForeignMaterialAreaLimit(1).ToString("F" + m_intDecimal);
            txt_ForeignMaterialVerticalBright.Text = m_smVisionInfo.g_arrPad[0].GetBrightVerticalForeignMaterialLimit(1).ToString("F" + m_intDecimal);
            txt_ForeignMaterialHorizontalBright.Text = m_smVisionInfo.g_arrPad[0].GetBrightHorizontalForeignMaterialLimit(1).ToString("F" + m_intDecimal);

            if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
            {
                txt_GeneralVerticalBright_Side.Text = m_smVisionInfo.g_arrPad[1].GetBrightLengthLimit(1).ToString("F" + m_intDecimal);
                txt_GeneralVerticalDark_Side.Text = m_smVisionInfo.g_arrPad[1].GetDarkLengthLimit(1).ToString("F" + m_intDecimal);
                txt_GeneralHorizontalBright_Side.Text = m_smVisionInfo.g_arrPad[1].GetBrightWidthLimit(1).ToString("F" + m_intDecimal);
                txt_GeneralHorizontalDark_Side.Text = m_smVisionInfo.g_arrPad[1].GetDarkWidthLimit(1).ToString("F" + m_intDecimal);
                txt_GeneralAreaBright_Side.Text = m_smVisionInfo.g_arrPad[1].GetBrightAreaLimit(1).ToString("F" + m_intDecimal);
                txt_GeneralAreaDark_Side.Text = m_smVisionInfo.g_arrPad[1].GetDarkAreaLimit(1).ToString("F" + m_intDecimal);
                txt_GeneralTotalAreaBright_Side.Text = m_smVisionInfo.g_arrPad[1].GetBrightTotalAreaLimit(1).ToString("F" + m_intDecimal);
                txt_GeneralTotalAreaDark_Side.Text = m_smVisionInfo.g_arrPad[1].GetDarkTotalAreaLimit(1).ToString("F" + m_intDecimal);

                cbo_MoldFlashBrightDefectFailCondition_Side.SelectedIndex = m_smVisionInfo.g_arrPad[1].ref_intMoldFlashDefectDimensionFailCondition;
                cbo_GeneralBrightDefectFailCondition_Side.SelectedIndex = m_smVisionInfo.g_arrPad[1].ref_intBrightDefectDimensionFailCondition;
                cbo_ForeignMaterialBrightDefectFailCondition_Side.SelectedIndex = m_smVisionInfo.g_arrPad[1].ref_intForeignMaterialBrightDefectDimensionFailCondition;
                cbo_GeneralDarkDefectFailCondition_Side.SelectedIndex = m_smVisionInfo.g_arrPad[1].ref_intDarkDefectDimensionFailCondition;
                cbo_CrackDarkDefectFailCondition_Side.SelectedIndex = m_smVisionInfo.g_arrPad[1].ref_intCrackDarkDefectDimensionFailCondition;

                txt_CrackAreaDark_Side.Text = m_smVisionInfo.g_arrPad[1].GetCrackAreaLimit(1).ToString("F" + m_intDecimal);
                txt_MoldFlashVerticalBright_Side.Text = m_smVisionInfo.g_arrPad[1].GetMoldFlashLengthLimit(1).ToString("F" + m_intDecimal);
                txt_MoldFlashHorizontalBright_Side.Text = m_smVisionInfo.g_arrPad[1].GetMoldFlashWidthLimit(1).ToString("F" + m_intDecimal);
                txt_MoldFlashAreaBright_Side.Text = m_smVisionInfo.g_arrPad[1].GetMoldFlashAreaLimit(1).ToString("F" + m_intDecimal);
                txt_MoldFlashTotalAreaBright_Side.Text = m_smVisionInfo.g_arrPad[1].GetMoldFlashTotalAreaLimit(1).ToString("F" + m_intDecimal);
                txt_GeneralChipAreaBright_Side.Text = m_smVisionInfo.g_arrPad[1].GetBrightChippedOffAreaLimit(1).ToString("F" + m_intDecimal);
                txt_GeneralChipAreaDark_Side.Text = m_smVisionInfo.g_arrPad[1].GetDarkChippedOffAreaLimit(1).ToString("F" + m_intDecimal);
                txt_CrackVerticalDark_Side.Text = m_smVisionInfo.g_arrPad[1].GetDarkVerticalCrackLimit(1).ToString("F" + m_intDecimal);
                txt_CrackHorizontalDark_Side.Text = m_smVisionInfo.g_arrPad[1].GetDarkHorizontalCrackLimit(1).ToString("F" + m_intDecimal);

                txt_ForeignMaterialAreaBright_Side.Text = m_smVisionInfo.g_arrPad[1].GetForeignMaterialAreaLimit(1).ToString("F" + m_intDecimal);
                txt_ForeignMaterialVerticalBright_Side.Text = m_smVisionInfo.g_arrPad[1].GetBrightVerticalForeignMaterialLimit(1).ToString("F" + m_intDecimal);
                txt_ForeignMaterialHorizontalBright_Side.Text = m_smVisionInfo.g_arrPad[1].GetBrightHorizontalForeignMaterialLimit(1).ToString("F" + m_intDecimal);
            }
        }

        private void UpdateInfo()
        {
            for (int i = 0; i < m_arrRowSettingName.Count; i++)
            {           
                float fWidth = 0;
                float fHeight = 0;
                float fThickness = 0;
                switch (m_arrRowSettingName[i])
                {
                    case RowSettingName.XDimension:
                        fWidth = (m_smVisionInfo.g_arrPad[0].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[0].GetResultDownWidth_RectGauge4L(1)) / 2;

                        // 2019-10-25 ZJYEOH : Add Offset to package width
                        fWidth += m_smVisionInfo.g_arrPad[0].ref_fPackageWidthOffsetMM;

                        dgd_PackageSetting.Rows[i].Cells[3].Value = fWidth.ToString("F" + m_intDecimal);

                        if (fWidth > m_smVisionInfo.g_arrPad[0].GetUnitWidthMax(1) || fWidth < m_smVisionInfo.g_arrPad[0].GetUnitWidthMin(1))
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        }
                        break;
                    case RowSettingName.YDimension:
                        fHeight = (m_smVisionInfo.g_arrPad[0].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[0].GetResultRightHeight_RectGauge4L(1)) / 2;

                        // 2019-10-25 ZJYEOH : Add Offset to package height
                        fHeight += m_smVisionInfo.g_arrPad[0].ref_fPackageHeightOffsetMM;

                        dgd_PackageSetting.Rows[i].Cells[3].Value = fHeight.ToString("F" + m_intDecimal);

                        if (fHeight > m_smVisionInfo.g_arrPad[0].GetUnitHeightMax(1) || fHeight < m_smVisionInfo.g_arrPad[0].GetUnitHeightMin(1))
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        }
                        break;
                    case RowSettingName.ZDimension:

                        if (m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_arrPad[1].GetOverallWantGaugeMeasurePkgSize(true))
                        {
                            float fTotalThinkness = 0;
                            int intCount = 0;
                            for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                            {
                                if (j == 1 || j == 3)
                                    fTotalThinkness += m_smVisionInfo.g_arrPad[j].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[j].GetResultRightHeight_RectGauge4L(1);
                                else
                                    fTotalThinkness += m_smVisionInfo.g_arrPad[j].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[j].GetResultDownWidth_RectGauge4L(1);

                                intCount += 2;
                            }
                            fThickness = fTotalThinkness / intCount;

                            // 2019-10-25 ZJYEOH : Add Offset to package thickness
                            fThickness += m_smVisionInfo.g_arrPad[1].ref_fPackageThicknessOffsetMM;

                            dgd_PackageSetting.Rows[i].Cells[3].Value = fThickness.ToString("F" + m_intDecimal);

                            if (fThickness > m_smVisionInfo.g_arrPad[0].GetUnitThicknessMax(1) || fThickness < m_smVisionInfo.g_arrPad[0].GetUnitThicknessMin(1))
                            {
                                dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                                dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                            }
                            else
                            {
                                dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                                dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                            }
                        }
                        else
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Value = "----";
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.White;
                        }
                        break;
                    case RowSettingName.TopThickness:
                        float fTopThinkness = 0;

                        fTopThinkness = (m_smVisionInfo.g_arrPad[1].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[1].GetResultRightHeight_RectGauge4L(1)) / 2;

                        // 2019-10-25 ZJYEOH : Add Offset to package thickness
                        fTopThinkness += m_smVisionInfo.g_arrPad[1].ref_fPackageThicknessOffsetMM;

                        dgd_PackageSetting.Rows[i].Cells[3].Value = fTopThinkness.ToString("F" + m_intDecimal);

                        if (fTopThinkness > m_smVisionInfo.g_arrPad[0].GetUnitThicknessMax(1) || fTopThinkness < m_smVisionInfo.g_arrPad[0].GetUnitThicknessMin(1))
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        }
                        break;
                    case RowSettingName.RightThickness:
                        float fRightThinkness = 0;

                        fRightThinkness = (m_smVisionInfo.g_arrPad[2].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[2].GetResultDownWidth_RectGauge4L(1)) / 2;

                        // 2019-10-25 ZJYEOH : Add Offset to package thickness
                        fRightThinkness += m_smVisionInfo.g_arrPad[1].ref_fPackageThicknessOffsetMM;

                        dgd_PackageSetting.Rows[i].Cells[3].Value = fRightThinkness.ToString("F" + m_intDecimal);

                        if (fRightThinkness > m_smVisionInfo.g_arrPad[0].GetUnitThicknessMax(1) || fRightThinkness < m_smVisionInfo.g_arrPad[0].GetUnitThicknessMin(1))
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        }
                        break;
                    case RowSettingName.BottomThickness:
                        float fBottomThinkness = 0;

                        fBottomThinkness = (m_smVisionInfo.g_arrPad[3].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrPad[3].GetResultRightHeight_RectGauge4L(1)) / 2;

                        // 2019-10-25 ZJYEOH : Add Offset to package thickness
                        fBottomThinkness += m_smVisionInfo.g_arrPad[1].ref_fPackageThicknessOffsetMM;

                        dgd_PackageSetting.Rows[i].Cells[3].Value = fBottomThinkness.ToString("F" + m_intDecimal);

                        if (fBottomThinkness > m_smVisionInfo.g_arrPad[0].GetUnitThicknessMax(1) || fBottomThinkness < m_smVisionInfo.g_arrPad[0].GetUnitThicknessMin(1))
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        }
                        break;
                    case RowSettingName.LeftThickness:
                        float fLeftThinkness = 0;

                        fLeftThinkness = (m_smVisionInfo.g_arrPad[4].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrPad[4].GetResultDownWidth_RectGauge4L(1)) / 2;

                        // 2019-10-25 ZJYEOH : Add Offset to package thickness
                        fLeftThinkness += m_smVisionInfo.g_arrPad[1].ref_fPackageThicknessOffsetMM;

                        dgd_PackageSetting.Rows[i].Cells[3].Value = fLeftThinkness.ToString("F" + m_intDecimal);

                        if (fLeftThinkness > m_smVisionInfo.g_arrPad[0].GetUnitThicknessMax(1) || fLeftThinkness < m_smVisionInfo.g_arrPad[0].GetUnitThicknessMin(1))
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        }
                        else
                        {
                            dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                            dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        }
                        break;
                }
            }


        }

        private void UpdateUnitDisplay()
        {
            string strUnitDisplay;
            switch (m_smCustomizeInfo.g_intUnitDisplay)
            {
                case 1:
                default:
                    strUnitDisplay = "mm";
                    m_intDecimal = 4;
                    m_intDecimal2 = 6;
                    break;
                case 2:
                    strUnitDisplay = "mil";
                    m_intDecimal = 3;
                    m_intDecimal2 = 6;
                    break;
                case 3:
                    strUnitDisplay = "um";
                    m_intDecimal = 1;
                    m_intDecimal2 = 2;
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
                STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">PadPackage " + strSectionName + " Tolerance Setting", m_smProductionInfo.g_strLotID);
                
                //objFile.WriteElement1Value("OrientSetting", true);
                //objFile.WriteElement2Value("MatchMinScore", m_smVisionInfo.g_arrPadOrient[i].ref_fMinScore); 
            }
        }

        private void ViewOrHideResultColumn(bool blnWantView)
        {
            if (m_smVisionInfo.g_blnCheck4Sides)
                dgd_PackageSetting.Columns[3].Visible = blnWantView;
            dgd_Position.Columns[2].Visible = blnWantView;
            lbl_PHBlobBlackArea.Visible = blnWantView;
            srmLabel17.Visible = blnWantView;
            lbl_Pin1Score.Visible = lbl_Pin1ScorePercent.Visible = lbl_PinScoreTitle.Visible = blnWantView; 
        }
        private void SaveOrientSettings(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "Template\\Template.xml");

            objFile.WriteSectionElement("Template" + 0);
            objFile.WriteElement1Value("MinScore", m_smVisionInfo.g_objPadOrient.ref_fMinScore);

            objFile.WriteEndElement();
         
        }
        private void LoadOrientSettings(string strFolderPath)
        {
            XmlParser objFile = new XmlParser(strFolderPath + "Template\\Template.xml");

            objFile.GetFirstSection("Template" + 0);

            m_smVisionInfo.g_objPadOrient.ref_fMinScore = objFile.GetValueAsFloat("MinScore", 0.7f);
            m_smVisionInfo.g_objPadOrient.SetCalibrationData(
                                  m_smVisionInfo.g_fCalibPixelX,
                                  m_smVisionInfo.g_fCalibPixelY, m_smCustomizeInfo.g_intUnitDisplay);

        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\";

            SavePadSetting(strPath + "Pad\\");

            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                SaveOrientSettings(strPath + "Orient\\");

            m_smVisionInfo.g_objPositioning.SavePosition(strPath + "Positioning\\Settings.xml", false, "General", true);

            if (m_smVisionInfo.g_arrPin1 != null)
            {
                m_smVisionInfo.g_arrPin1[0].SaveTemplate(strPath + "Pad\\Template\\");
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

            if ((m_smCustomizeInfo.g_intWantOrient & (1 << m_smVisionInfo.g_intVisionPos)) > 0)
                LoadOrientSettings(strFolderPath + "Orient\\");

            m_smVisionInfo.g_objPositioning.LoadPosition(strFolderPath + "Positioning\\Settings.xml", "General");
            if (m_smVisionInfo.g_arrPin1 != null)
            {
                m_smVisionInfo.g_arrPin1[0].LoadTemplate(strFolderPath + "Pad\\Template\\");
            }

            this.Close();
            this.Dispose();
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

                //UpdateGUI();
            }

            if (m_smVisionInfo.PR_TL_UpdateInfo2)
            {
                UpdateInfo();
                m_smVisionInfo.PR_TL_UpdateInfo2 = false;
            }
        }

        private void PadPackageToleranceSettingForm_Load(object sender, EventArgs e)
        {
            m_smProductionInfo.g_DTStartAutoMode = m_smProductionInfo.g_DTStartAutoMode_IndividualForm = DateTime.Now;
            m_smVisionInfo.g_blnViewPadSettingDrawing = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void PadPackageToleranceSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_smVisionInfo.g_blnViewROI = false;
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Pad Package Tolerance Setting Form Closed", "Exit Pad Package Tolerance Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_blnViewPadSettingDrawing = false;
            m_smVisionInfo.g_blnViewPadPackageDefectSetting = false;
            m_smVisionInfo.PR_MN_UpdateSettingInfo = true;
            m_smVisionInfo.VM_AT_SettingInDialog = false;
        }

        private void chk_DisplayResult_Click(object sender, EventArgs e)
        {
            ViewOrHideResultColumn(chk_DisplayResult.Checked);

            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey = key.CreateSubKey("SVG\\AutoMode");
            subkey.SetValue("CurrentDisplayResult_MoTolerance", chk_DisplayResult.Checked);
        }

        private void dgd_PackageSetting_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            /*
             * Please take note dgd_PackageSetting_CellValueChanged event will be triggered during loading and when value change in cell.
             * dgd_MarkSetting_CellEndEdit will not be triggred although value change. It will only been triggered after user finish change cell value manually (even same value), 
             */

            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            int c = e.ColumnIndex;
            int r = e.RowIndex;

            switch (m_arrRowSettingName[r])
            {
                case RowSettingName.XDimension:
                    {
                        if (c == 1)
                        {
                            float fValue;
                            if (dgd_PackageSetting.Rows[r].Cells[1].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[1].Value.ToString(), out fValue))
                            {
                                float fMaxValue = 0;
                                if (float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fMaxValue))
                                {
                                    if (fValue > fMaxValue)
                                    {
                                        SRMMessageBox.Show("Setting Fail. Min value cannot bigger than Max value.");
                                    }
                                    else
                                    {
                                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                            m_smVisionInfo.g_arrPad[i].SetUnitWidthMin(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                                    }
                                }
                            }
                        }
                        else if (c == 2)
                        {
                            float fValue;
                            if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                            {
                                float fMinValue = 0;
                                if (float.TryParse(dgd_PackageSetting.Rows[r].Cells[1].Value.ToString(), out fMinValue))
                                {
                                    if (fValue < fMinValue)
                                    {
                                        SRMMessageBox.Show("Setting Fail. Max value cannot smaller than Min value.");
                                    }
                                    else
                                    {
                                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                            m_smVisionInfo.g_arrPad[i].SetUnitWidthMax(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case RowSettingName.YDimension:
                    {
                        if (c == 1)
                        {
                            float fValue;
                            if (dgd_PackageSetting.Rows[r].Cells[1].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[1].Value.ToString(), out fValue))
                            {
                                float fMaxValue = 0;
                                if (float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fMaxValue))
                                {
                                    if (fValue > fMaxValue)
                                    {
                                        SRMMessageBox.Show("Setting Fail. Min value cannot bigger than Max value.");
                                    }
                                    else
                                    {
                                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                            m_smVisionInfo.g_arrPad[i].SetUnitHeightMin(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                                    }
                                }
                            }
                        }
                        else if (c == 2)
                        {
                            float fValue;
                            if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                            {
                                float fMinValue = 0;
                                if (float.TryParse(dgd_PackageSetting.Rows[r].Cells[1].Value.ToString(), out fMinValue))
                                {
                                    if (fValue < fMinValue)
                                    {
                                        SRMMessageBox.Show("Setting Fail. Max value cannot smaller than Min value.");
                                    }
                                    else
                                    {
                                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                            m_smVisionInfo.g_arrPad[i].SetUnitHeightMax(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case RowSettingName.ZDimension:
                    {
                        if (c == 1)
                        {
                            float fValue;
                            if (dgd_PackageSetting.Rows[r].Cells[1].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[1].Value.ToString(), out fValue))
                            {
                                float fMaxValue = 0;
                                if (float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fMaxValue))
                                {
                                    if (fValue > fMaxValue)
                                    {
                                        SRMMessageBox.Show("Setting Fail. Min value cannot bigger than Max value.");
                                    }
                                    else
                                    {
                                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                            m_smVisionInfo.g_arrPad[i].SetUnitThicknessMin(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                                    }
                                }
                            }
                        }
                        else if (c == 2)
                        {
                            float fValue;
                            if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                            {
                                float fMinValue = 0;
                                if (float.TryParse(dgd_PackageSetting.Rows[r].Cells[1].Value.ToString(), out fMinValue))
                                {
                                    if (fValue < fMinValue)
                                    {
                                        SRMMessageBox.Show("Setting Fail. Max value cannot smaller than Min value.");
                                    }
                                    else
                                    {
                                        for (int i = 0; i < m_smVisionInfo.g_arrPad.Length; i++)
                                            m_smVisionInfo.g_arrPad[i].SetUnitThicknessMax(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case RowSettingName.CenterPkg_ScratchesLength:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            m_smVisionInfo.g_arrPad[0].SetScratchLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.CenterPkg_ScratchesArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            m_smVisionInfo.g_arrPad[0].SetScratchAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.CenterPkg_ChippedOffArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            m_smVisionInfo.g_arrPad[0].SetChipAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.CenterPkg_ContaminationLength:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            m_smVisionInfo.g_arrPad[0].SetExtraPadLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.CenterPkg_ContaminationArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            m_smVisionInfo.g_arrPad[0].SetExtraPadMinArea(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.CenterPkg_ContaminationTotalArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            m_smVisionInfo.g_arrPad[0].SetTotalExtraPadMinArea(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.CenterPkg_MoldFlashArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            m_smVisionInfo.g_arrPad[0].SetMoldFlashAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.CenterPkg_VoidLength:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            m_smVisionInfo.g_arrPad[0].SetVoidLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.CenterPkg_VoidArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            m_smVisionInfo.g_arrPad[0].SetVoidAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.CenterPkg_CrackLength:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            m_smVisionInfo.g_arrPad[0].SetCrackLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.CenterPkg_CrackArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            m_smVisionInfo.g_arrPad[0].SetCrackAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.SidePkg_ScratchesLength:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetScratchLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.SidePkg_ScratchesArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetScratchAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.SidePkg_ChippedOffArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetChipAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.SidePkg_ContaminationLength:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetExtraPadLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.SidePkg_ContaminationArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetExtraPadMinArea(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.SidePkg_ContaminationTotalArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetTotalExtraPadMinArea(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.SidePkg_MoldFlashArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetMoldFlashAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.SidePkg_VoidLength:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetVoidLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.SidePkg_VoidArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetVoidAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.SidePkg_CrackLength:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetCrackLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
                case RowSettingName.SidePkg_CrackArea:
                    {
                        float fValue;
                        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetCrackAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                        }
                    }
                    break;
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            UpdateSettingGUI();
        }

        private void dgd_PackageSetting_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            m_smVisionInfo.g_intSelectedPackageDefect = -1;

            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            if (e.ColumnIndex < 0 || e.RowIndex < 0)
                return;

            int c = e.ColumnIndex;
            int r = e.RowIndex;

            switch (m_arrRowSettingName[r])
            {
                case RowSettingName.XDimension:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 0;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                        if (c == 1)
                            m_smVisionInfo.g_blnViewPackageMinDefect = true;
                        else if (c == 2)
                            m_smVisionInfo.g_blnViewPackageMinDefect = false;
                    }
                    break;
                case RowSettingName.YDimension:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 1;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                        if (c == 1)
                            m_smVisionInfo.g_blnViewPackageMinDefect = true;
                        else if (c == 2)
                            m_smVisionInfo.g_blnViewPackageMinDefect = false;
                    }
                    break;
                case RowSettingName.ZDimension:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 2;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                        if (c == 1)
                            m_smVisionInfo.g_blnViewPackageMinDefect = true;
                        else if (c == 2)
                            m_smVisionInfo.g_blnViewPackageMinDefect = false;
                    }
                    break;
                case RowSettingName.CenterPkg_ScratchesLength:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 3;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.CenterPkg_ScratchesArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 4;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = true;
                    }
                    break;
                case RowSettingName.CenterPkg_ChippedOffArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 5;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.CenterPkg_ContaminationLength:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 15;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.CenterPkg_ContaminationArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 16;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = true;
                    }
                    break;
                case RowSettingName.CenterPkg_ContaminationTotalArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 17;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.CenterPkg_MoldFlashArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 6;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.CenterPkg_VoidLength:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 11;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.CenterPkg_VoidArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 12;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.CenterPkg_CrackLength:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 13;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.CenterPkg_CrackArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 14;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.SidePkg_ScratchesLength:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 7;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.SidePkg_ScratchesArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 8;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = true;
                    }
                    break;
                case RowSettingName.SidePkg_ChippedOffArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 9;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.SidePkg_ContaminationLength:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 18;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.SidePkg_ContaminationArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 19;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = true;
                    }
                    break;
                case RowSettingName.SidePkg_ContaminationTotalArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 20;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.SidePkg_MoldFlashArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 10;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.SidePkg_VoidLength:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 21;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.SidePkg_VoidArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 22;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.SidePkg_CrackLength:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 23;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
                case RowSettingName.SidePkg_CrackArea:
                    {
                        m_smVisionInfo.g_intSelectedPackageDefect = 24;
                        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                    }
                    break;
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_PackageSetting_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            m_smVisionInfo.g_blnViewPadPackageDefectSetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_Position_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;
            switch (e.RowIndex)
            {
                case 2:
                case 3:
                    m_smVisionInfo.g_blnViewCenterPadPositionSetting = true;
                    //m_smVisionInfo.g_blnViewSidePadPositionSetting = false;
                    break;
                //case 6:
                //case 7:
                //    m_smVisionInfo.g_blnViewSidePadPositionSetting = true;
                //    m_smVisionInfo.g_blnViewCenterPadPositionSetting = false;
                //    break;
            }
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void dgd_Position_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            m_smVisionInfo.g_blnViewCenterPadPositionSetting = false;
            //m_smVisionInfo.g_blnViewSidePadPositionSetting = false;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_Position_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            float fMaxValue;
            switch (e.RowIndex)
            {
                case 1:
                    if (float.TryParse(dgd_Position.Rows[1].Cells[1].Value.ToString(), out fMaxValue))
                    {
                        if (fMaxValue > 0)
                            m_smVisionInfo.g_arrPad[0].ref_fAngleTolerance = fMaxValue;
                        else
                        {
                            SRMMessageBox.Show("Please enter positive value!");
                            dgd_Position.Rows[1].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fAngleTolerance.ToString("f4");
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Please enter valid numerical value!");
                        dgd_Position.Rows[1].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fAngleTolerance.ToString("f4");
                    }
                    break;
                case 2:
                    if (float.TryParse(dgd_Position.Rows[2].Cells[1].Value.ToString(), out fMaxValue))
                    {
                        if (fMaxValue > 0)
                        {
                            m_smVisionInfo.g_arrPad[0].ref_fXTolerance = fMaxValue;
                            m_smVisionInfo.g_blnViewCenterPadPositionSetting = true;
                        }
                        else
                        {
                            SRMMessageBox.Show("Please enter positive value!");
                            dgd_Position.Rows[2].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fXTolerance.ToString("f4");
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Please enter numerical value!");
                        dgd_Position.Rows[2].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fXTolerance.ToString("f4");
                    }
                    break;
                case 3:
                    if (float.TryParse(dgd_Position.Rows[3].Cells[1].Value.ToString(), out fMaxValue))
                    {
                        if (fMaxValue > 0)
                        {
                            m_smVisionInfo.g_arrPad[0].ref_fYTolerance = fMaxValue;
                            m_smVisionInfo.g_blnViewCenterPadPositionSetting = true;
                        }
                        else
                        {
                            SRMMessageBox.Show("Please enter positive value!");
                            dgd_Position.Rows[3].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fYTolerance.ToString("f4");
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Please enter numerical value!");
                        dgd_Position.Rows[3].Cells[1].Value = m_smVisionInfo.g_arrPad[0].ref_fYTolerance.ToString("f4");
                    }
                    break;
                //case 5:
                //    if (float.TryParse(dgd_Position.Rows[5].Cells[1].Value.ToString(), out fMaxValue))
                //    {
                //        if (fMaxValue > 0)
                //        {
                //            m_smVisionInfo.g_arrPad[1].ref_fAngleTolerance = fMaxValue;
                //            m_smVisionInfo.g_arrPad[2].ref_fAngleTolerance = fMaxValue;
                //            m_smVisionInfo.g_arrPad[3].ref_fAngleTolerance = fMaxValue;
                //            m_smVisionInfo.g_arrPad[4].ref_fAngleTolerance = fMaxValue;
                //        }
                //        else
                //        {
                //            SRMMessageBox.Show("Please enter positive value!");
                //            dgd_Position.Rows[5].Cells[1].Value = m_smVisionInfo.g_arrPad[1].ref_fAngleTolerance.ToString("f4");
                //        }
                //    }
                //    else
                //    {
                //        SRMMessageBox.Show("Please enter numerical value!");
                //        dgd_Position.Rows[5].Cells[1].Value = m_smVisionInfo.g_arrPad[1].ref_fAngleTolerance.ToString("f4");
                //    }
                //    break;
                //case 6:
                //    if (float.TryParse(dgd_Position.Rows[6].Cells[1].Value.ToString(), out fMaxValue))
                //    {
                //        if (fMaxValue > 0)
                //        {
                //            m_smVisionInfo.g_arrPad[1].ref_fXTolerance = fMaxValue;
                //            m_smVisionInfo.g_arrPad[2].ref_fXTolerance = fMaxValue;
                //            m_smVisionInfo.g_arrPad[3].ref_fXTolerance = fMaxValue;
                //            m_smVisionInfo.g_arrPad[4].ref_fXTolerance = fMaxValue;
                //            m_smVisionInfo.g_blnViewSidePadPositionSetting = true;
                //        }
                //        else
                //        {
                //            SRMMessageBox.Show("Please enter positive value!");
                //            dgd_Position.Rows[6].Cells[1].Value = m_smVisionInfo.g_arrPad[1].ref_fXTolerance.ToString("f4");
                //        }
                //    }
                //    else
                //    {
                //        SRMMessageBox.Show("Please enter numerical value!");
                //        dgd_Position.Rows[6].Cells[1].Value = m_smVisionInfo.g_arrPad[1].ref_fXTolerance.ToString("f4");
                //    }
                //    break;
                //case 7:
                //    if (float.TryParse(dgd_Position.Rows[7].Cells[1].Value.ToString(), out fMaxValue))
                //    {
                //        if (fMaxValue > 0)
                //        {
                //            m_smVisionInfo.g_arrPad[1].ref_fYTolerance = fMaxValue;
                //            m_smVisionInfo.g_arrPad[2].ref_fYTolerance = fMaxValue;
                //            m_smVisionInfo.g_arrPad[3].ref_fYTolerance = fMaxValue;
                //            m_smVisionInfo.g_arrPad[4].ref_fYTolerance = fMaxValue;
                //            m_smVisionInfo.g_blnViewSidePadPositionSetting = true;
                //        }
                //        else
                //        {
                //            SRMMessageBox.Show("Please enter positive value!");
                //            dgd_Position.Rows[7].Cells[1].Value = m_smVisionInfo.g_arrPad[1].ref_fYTolerance.ToString("f4");
                //        }
                //    }
                //    else
                //    {
                //        SRMMessageBox.Show("Please enter numerical value!");
                //        dgd_Position.Rows[7].Cells[1].Value = m_smVisionInfo.g_arrPad[1].ref_fYTolerance.ToString("f4");
                //    }
                //    break;
            }

           
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_BlackAreaArea_TextChanged(object sender, EventArgs e)
        {
                if (txt_BlackAreaArea.Text == "")
                {

                    SRMMessageBox.Show("Minimum Black Area cannot be empty!");
                txt_BlackAreaArea.Text = m_smVisionInfo.g_objPositioning.ref_intPHBlackArea.ToString();
                    return;
                }
            m_smVisionInfo.g_objPositioning.ref_intPHBlackArea = Convert.ToInt32(txt_BlackAreaArea.Text);
        }

        private void txt_GeneralVerticalBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralVerticalBright.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetBrightLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            } 

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralVerticalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralVerticalDark.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetDarkLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralHorizontalBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralHorizontalBright.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetBrightWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralHorizontalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralHorizontalDark.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetDarkWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralAreaBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralAreaBright.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetBrightAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralAreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralAreaDark.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetDarkAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralTotalAreaBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralTotalAreaBright.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetBrightTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralTotalAreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralTotalAreaDark.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetDarkTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralChipAreaBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralChipAreaBright.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetBrightChippedOffAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralChipAreaDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralChipAreaDark.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetDarkChippedOffAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CrackVerticalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CrackVerticalDark.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetDarkVerticalCrackLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CrackHorizontalDark_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CrackHorizontalDark.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetDarkHorizontalCrackLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralVerticalBright_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralVerticalBright_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetBrightLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralVerticalDark_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralVerticalDark_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetDarkLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralHorizontalBright_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralHorizontalBright_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetBrightWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralHorizontalDark_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralHorizontalDark_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetDarkWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralAreaBright_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralAreaBright_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetBrightAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralAreaDark_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralAreaDark_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetDarkAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralTotalAreaBright_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralTotalAreaBright_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetBrightTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralTotalAreaDark_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralTotalAreaDark_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetDarkTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_Pin1Tolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            trackBar_Pin1Tolerance.Value = Convert.ToInt32(txt_Pin1Tolerance.Text);

            m_smVisionInfo.g_arrPin1[0].SetMinScoreSetting(0, trackBar_Pin1Tolerance.Value / 100.0f);
        }

        private void trackBar_Pin1Tolerance_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_blnUpdateSelectedROISetting = true;

            txt_Pin1Tolerance.Text = trackBar_Pin1Tolerance.Value.ToString();

            m_smVisionInfo.g_arrPin1[0].SetMinScoreSetting(0, trackBar_Pin1Tolerance.Value / 100.0f);

            m_blnUpdateSelectedROISetting = false;
        }

        private void txt_CrackAreaDark_TextChanged(object sender, EventArgs e)
        {
            float fValue;
            if (txt_CrackAreaDark.Text != null && float.TryParse(txt_CrackAreaDark.Text.ToString(), out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetCrackAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }
        }

        private void txt_CrackAreaDark_Side_TextChanged(object sender, EventArgs e)
        {
            float fValue;
            if (txt_CrackAreaDark_Side.Text != null && float.TryParse(txt_CrackAreaDark_Side.Text.ToString(), out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetCrackAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }
        }

        private void txt_MoldFlashAreaBright_Side_TextChanged(object sender, EventArgs e)
        {
            float fValue;
            if (txt_MoldFlashAreaBright_Side.Text != null && float.TryParse(txt_MoldFlashAreaBright_Side.Text.ToString(), out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetMoldFlashAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }
        }

        private void txt_MoldFlashAreaBright_TextChanged(object sender, EventArgs e)
        {
            float fValue;
            if (txt_MoldFlashAreaBright.Text != null && float.TryParse(txt_MoldFlashAreaBright.Text.ToString(), out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetMoldFlashAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }
        }

      

        private void txt_GeneralChipAreaBright_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralChipAreaBright_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetBrightChippedOffAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_GeneralChipAreaDark_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_GeneralChipAreaDark_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetDarkChippedOffAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_CrackVerticalDark_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CrackVerticalDark_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetDarkVerticalCrackLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_CrackHorizontalDark_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_CrackHorizontalDark_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetDarkHorizontalCrackLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_OrientTolerance_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            trackBar_OrientTolerance.Value = Convert.ToInt32(txt_OrientTolerance.Text);

            m_smVisionInfo.g_objPadOrient.ref_fMinScore = (trackBar_OrientTolerance.Value / 100.0f);

        }

        private void trackBar_OrientTolerance_Scroll(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            txt_OrientTolerance.Text = trackBar_OrientTolerance.Value.ToString();

        }

        private void txt_MoldFlashTotalAreaBright_TextChanged(object sender, EventArgs e)
        {
            float fValue;
            if (txt_MoldFlashTotalAreaBright.Text != null && float.TryParse(txt_MoldFlashTotalAreaBright.Text.ToString(), out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetMoldFlashTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }
        }

        private void txt_MoldFlashTotalAreaBright_Side_TextChanged(object sender, EventArgs e)
        {
            float fValue;
            if (txt_MoldFlashTotalAreaBright_Side.Text != null && float.TryParse(txt_MoldFlashTotalAreaBright_Side.Text.ToString(), out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetMoldFlashTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }
        }

        private void txt_ColorDefectLength_Center_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;

            if (((TextBox)sender).Name.Contains("1"))
            {
                if (float.TryParse(txt_ColorDefect1Length_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[0]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("2"))
            {
                if (float.TryParse(txt_ColorDefect2Length_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[1]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("3"))
            {
                if (float.TryParse(txt_ColorDefect3Length_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[2]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("4"))
            {
                if (float.TryParse(txt_ColorDefect4Length_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[3]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("5"))
            {
                if (float.TryParse(txt_ColorDefect5Length_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[4]);
                    }
                }
            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectWidth_Center_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;

            if (((TextBox)sender).Name.Contains("1"))
            {
                if (float.TryParse(txt_ColorDefect1Width_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[0]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("2"))
            {
                if (float.TryParse(txt_ColorDefect2Width_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[1]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("3"))
            {
                if (float.TryParse(txt_ColorDefect3Width_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[2]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("4"))
            {
                if (float.TryParse(txt_ColorDefect4Width_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[3]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("5"))
            {
                if (float.TryParse(txt_ColorDefect5Width_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[4]);
                    }
                }
            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectMinArea_Center_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;

            if (((TextBox)sender).Name.Contains("1"))
            {
                if (float.TryParse(txt_ColorDefect1MinArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[0]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("2"))
            {
                if (float.TryParse(txt_ColorDefect2MinArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[1]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("3"))
            {
                if (float.TryParse(txt_ColorDefect3MinArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[2]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("4"))
            {
                if (float.TryParse(txt_ColorDefect4MinArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[3]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("5"))
            {
                if (float.TryParse(txt_ColorDefect5MinArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[4]);
                    }
                }
            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectMaxArea_Center_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;

            if (((TextBox)sender).Name.Contains("1"))
            {
                if (float.TryParse(txt_ColorDefect1MaxArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[0]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("2"))
            {
                if (float.TryParse(txt_ColorDefect2MaxArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[1]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("3"))
            {
                if (float.TryParse(txt_ColorDefect3MaxArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[2]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("4"))
            {
                if (float.TryParse(txt_ColorDefect4MaxArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[3]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("5"))
            {
                if (float.TryParse(txt_ColorDefect5MaxArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[4]);
                    }
                }
            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectTotalArea_Center_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;

            if (((TextBox)sender).Name.Contains("1"))
            {
                if (float.TryParse(txt_ColorDefect1TotalArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[0]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("2"))
            {
                if (float.TryParse(txt_ColorDefect2TotalArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[1]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("3"))
            {
                if (float.TryParse(txt_ColorDefect3TotalArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[2]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("4"))
            {
                if (float.TryParse(txt_ColorDefect4TotalArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[3]);
                    }
                }
            }
            else if (((TextBox)sender).Name.Contains("5"))
            {
                if (float.TryParse(txt_ColorDefect5TotalArea_Center.Text, out fValue))
                {
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[4]);
                    }
                }
            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectLength_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;

            if (chk_SetToAllSideROI.Checked)
            {
                int intSelectedROI = 1;
                if (m_smVisionInfo.g_intSelectedROI > 1)
                    intSelectedROI = m_smVisionInfo.g_intSelectedROI;

                if (((TextBox)sender).Name.Contains("1"))
                {
                    if (float.TryParse(txt_ColorDefect1Length_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[0]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("2"))
                {
                    if (float.TryParse(txt_ColorDefect2Length_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[1]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("3"))
                {
                    if (float.TryParse(txt_ColorDefect3Length_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[2]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("4"))
                {
                    if (float.TryParse(txt_ColorDefect4Length_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[3]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("5"))
                {
                    if (float.TryParse(txt_ColorDefect5Length_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[4]);
                    }
                }

            }
            else
            {
                int intSelectedROI = 1;
                if (m_smVisionInfo.g_intSelectedROI > 1)
                    intSelectedROI = m_smVisionInfo.g_intSelectedROI;

                if (((TextBox)sender).Name.Contains("1"))
                {
                    if (float.TryParse(txt_ColorDefect1Length_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                    }
                }
                else if (((TextBox)sender).Name.Contains("2"))
                {
                    if (float.TryParse(txt_ColorDefect2Length_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                    }
                }
                else if (((TextBox)sender).Name.Contains("3"))
                {
                    if (float.TryParse(txt_ColorDefect3Length_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                    }
                }
                else if (((TextBox)sender).Name.Contains("4"))
                {
                    if (float.TryParse(txt_ColorDefect4Length_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                    }
                }
                else if (((TextBox)sender).Name.Contains("5"))
                {
                    if (float.TryParse(txt_ColorDefect5Length_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                    }
                }

            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectWidth_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;

            if (chk_SetToAllSideROI.Checked)
            {
                int intSelectedROI = 1;
                if (m_smVisionInfo.g_intSelectedROI > 1)
                    intSelectedROI = m_smVisionInfo.g_intSelectedROI;

                if (((TextBox)sender).Name.Contains("1"))
                {
                    if (float.TryParse(txt_ColorDefect1Width_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[0]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("2"))
                {
                    if (float.TryParse(txt_ColorDefect2Width_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[1]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("3"))
                {
                    if (float.TryParse(txt_ColorDefect3Width_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[2]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("4"))
                {
                    if (float.TryParse(txt_ColorDefect4Width_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[3]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("5"))
                {
                    if (float.TryParse(txt_ColorDefect5Width_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[4]);
                    }
                }

            }
            else
            {
                int intSelectedROI = 1;
                if (m_smVisionInfo.g_intSelectedROI > 1)
                    intSelectedROI = m_smVisionInfo.g_intSelectedROI;

                if (((TextBox)sender).Name.Contains("1"))
                {
                    if (float.TryParse(txt_ColorDefect1Width_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                    }
                }
                else if (((TextBox)sender).Name.Contains("2"))
                {
                    if (float.TryParse(txt_ColorDefect2Width_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                    }
                }
                else if (((TextBox)sender).Name.Contains("3"))
                {
                    if (float.TryParse(txt_ColorDefect3Width_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                    }
                }
                else if (((TextBox)sender).Name.Contains("4"))
                {
                    if (float.TryParse(txt_ColorDefect4Width_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                    }
                }
                else if (((TextBox)sender).Name.Contains("5"))
                {
                    if (float.TryParse(txt_ColorDefect5Width_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                    }
                }

            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectMinArea_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;

            if (chk_SetToAllSideROI.Checked)
            {
                int intSelectedROI = 1;
                if (m_smVisionInfo.g_intSelectedROI > 1)
                    intSelectedROI = m_smVisionInfo.g_intSelectedROI;

                if (((TextBox)sender).Name.Contains("1"))
                {
                    if (float.TryParse(txt_ColorDefect1MinArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[0]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("2"))
                {
                    if (float.TryParse(txt_ColorDefect2MinArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[1]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("3"))
                {
                    if (float.TryParse(txt_ColorDefect3MinArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[2]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("4"))
                {
                    if (float.TryParse(txt_ColorDefect4MinArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[3]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("5"))
                {
                    if (float.TryParse(txt_ColorDefect5MinArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[4]);
                    }
                }

            }
            else
            {
                int intSelectedROI = 1;
                if (m_smVisionInfo.g_intSelectedROI > 1)
                    intSelectedROI = m_smVisionInfo.g_intSelectedROI;

                if (((TextBox)sender).Name.Contains("1"))
                {
                    if (float.TryParse(txt_ColorDefect1MinArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                    }
                }
                else if (((TextBox)sender).Name.Contains("2"))
                {
                    if (float.TryParse(txt_ColorDefect2MinArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                    }
                }
                else if (((TextBox)sender).Name.Contains("3"))
                {
                    if (float.TryParse(txt_ColorDefect3MinArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                    }
                }
                else if (((TextBox)sender).Name.Contains("4"))
                {
                    if (float.TryParse(txt_ColorDefect4MinArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                    }
                }
                else if (((TextBox)sender).Name.Contains("5"))
                {
                    if (float.TryParse(txt_ColorDefect5MinArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                    }
                }

            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectMaxArea_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;

            if (chk_SetToAllSideROI.Checked)
            {
                int intSelectedROI = 1;
                if (m_smVisionInfo.g_intSelectedROI > 1)
                    intSelectedROI = m_smVisionInfo.g_intSelectedROI;

                if (((TextBox)sender).Name.Contains("1"))
                {
                    if (float.TryParse(txt_ColorDefect1MaxArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[0]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("2"))
                {
                    if (float.TryParse(txt_ColorDefect2MaxArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[1]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("3"))
                {
                    if (float.TryParse(txt_ColorDefect3MaxArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[2]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("4"))
                {
                    if (float.TryParse(txt_ColorDefect4MaxArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[3]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("5"))
                {
                    if (float.TryParse(txt_ColorDefect5MaxArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[4]);
                    }
                }

            }
            else
            {
                int intSelectedROI = 1;
                if (m_smVisionInfo.g_intSelectedROI > 1)
                    intSelectedROI = m_smVisionInfo.g_intSelectedROI;

                if (((TextBox)sender).Name.Contains("1"))
                {
                    if (float.TryParse(txt_ColorDefect1MaxArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                    }
                }
                else if (((TextBox)sender).Name.Contains("2"))
                {
                    if (float.TryParse(txt_ColorDefect2MaxArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                    }
                }
                else if (((TextBox)sender).Name.Contains("3"))
                {
                    if (float.TryParse(txt_ColorDefect3MaxArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                    }
                }
                else if (((TextBox)sender).Name.Contains("4"))
                {
                    if (float.TryParse(txt_ColorDefect4MaxArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                    }
                }
                else if (((TextBox)sender).Name.Contains("5"))
                {
                    if (float.TryParse(txt_ColorDefect5MaxArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                    }
                }

            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ColorDefectTotalArea_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;

            if (chk_SetToAllSideROI.Checked)
            {
                int intSelectedROI = 1;
                if (m_smVisionInfo.g_intSelectedROI > 1)
                    intSelectedROI = m_smVisionInfo.g_intSelectedROI;

                if (((TextBox)sender).Name.Contains("1"))
                {
                    if (float.TryParse(txt_ColorDefect1TotalArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[0]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("2"))
                {
                    if (float.TryParse(txt_ColorDefect2TotalArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[1]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("3"))
                {
                    if (float.TryParse(txt_ColorDefect3TotalArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[2]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("4"))
                {
                    if (float.TryParse(txt_ColorDefect4TotalArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[3]);
                    }
                }
                else if (((TextBox)sender).Name.Contains("5"))
                {
                    if (float.TryParse(txt_ColorDefect5TotalArea_Side.Text, out fValue))
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[intSelectedROI].ref_arrDefectColorThresName[4]);
                    }
                }

            }
            else
            {
                int intSelectedROI = 1;
                if (m_smVisionInfo.g_intSelectedROI > 1)
                    intSelectedROI = m_smVisionInfo.g_intSelectedROI;

                if (((TextBox)sender).Name.Contains("1"))
                {
                    if (float.TryParse(txt_ColorDefect1TotalArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 0);
                    }
                }
                else if (((TextBox)sender).Name.Contains("2"))
                {
                    if (float.TryParse(txt_ColorDefect2TotalArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 1);
                    }
                }
                else if (((TextBox)sender).Name.Contains("3"))
                {
                    if (float.TryParse(txt_ColorDefect3TotalArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 2);
                    }
                }
                else if (((TextBox)sender).Name.Contains("4"))
                {
                    if (float.TryParse(txt_ColorDefect4TotalArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 3);
                    }
                }
                else if (((TextBox)sender).Name.Contains("5"))
                {
                    if (float.TryParse(txt_ColorDefect5TotalArea_Side.Text, out fValue))
                    {
                        m_smVisionInfo.g_arrPad[intSelectedROI].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, 4);
                    }
                }

            }

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void tab_VisionControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((SRMControl.SRMTabControl)sender).TabPages[((SRMControl.SRMTabControl)sender).SelectedIndex].Name.Contains("Color") && m_smVisionInfo.g_arrPad.Length > 1 && m_smVisionInfo.g_blnCheck4Sides)
            {
                //UpdateGUI();
                if (m_smVisionInfo.g_arrPad.Length > 1)
                {
                    chk_SetToAllSideROI.Visible = true;
                    m_smVisionInfo.g_blnViewROI = true;
                }
                else
                {
                    chk_SetToAllSideROI.Visible = false;
                    m_smVisionInfo.g_blnViewROI = false;
                }

                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
            else
            {
                m_smVisionInfo.g_blnViewROI = false;
                chk_SetToAllSideROI.Visible = false;
                m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
            }
        }

        private void txt_ForeignMaterialVerticalBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_ForeignMaterialVerticalBright.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetBrightVerticalForeignMaterialLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ForeignMaterialHorizontalBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_ForeignMaterialHorizontalBright.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetBrightHorizontalForeignMaterialLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ForeignMaterialAreaBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue;
            if (txt_ForeignMaterialAreaBright.Text != null && float.TryParse(txt_ForeignMaterialAreaBright.Text.ToString(), out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetForeignMaterialAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }
        }

        private void txt_ForeignMaterialVerticalBright_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_ForeignMaterialVerticalBright_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetBrightVerticalForeignMaterialLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ForeignMaterialHorizontalBright_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_ForeignMaterialHorizontalBright_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetBrightHorizontalForeignMaterialLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_ForeignMaterialAreaBright_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue;
            if (txt_ForeignMaterialAreaBright_Side.Text != null && float.TryParse(txt_ForeignMaterialAreaBright_Side.Text.ToString(), out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetForeignMaterialAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }
        }

        private void cbo_GeneralBrightDefectFailCondition_Center_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_smVisionInfo.g_arrPad[0].ref_intBrightDefectDimensionFailCondition = cbo_GeneralBrightDefectFailCondition_Center.SelectedIndex;
        }

        private void cbo_GeneralDarkDefectFailCondition_Center_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_smVisionInfo.g_arrPad[0].ref_intDarkDefectDimensionFailCondition = cbo_GeneralDarkDefectFailCondition_Center.SelectedIndex;
        }

        private void cbo_CrackDarkDefectFailCondition_Center_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_smVisionInfo.g_arrPad[0].ref_intCrackDarkDefectDimensionFailCondition = cbo_CrackDarkDefectFailCondition_Center.SelectedIndex;
        }

        private void cbo_ForeignMaterialBrightDefectFailCondition_Center_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_smVisionInfo.g_arrPad[0].ref_intForeignMaterialBrightDefectDimensionFailCondition = cbo_ForeignMaterialBrightDefectFailCondition_Center.SelectedIndex;
        }

        private void cbo_GeneralBrightDefectFailCondition_Side_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_intBrightDefectDimensionFailCondition = cbo_GeneralBrightDefectFailCondition_Side.SelectedIndex;
        }

        private void cbo_GeneralDarkDefectFailCondition_Side_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_intDarkDefectDimensionFailCondition = cbo_GeneralDarkDefectFailCondition_Side.SelectedIndex;
        }

        private void cbo_CrackDarkDefectFailCondition_Side_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_intCrackDarkDefectDimensionFailCondition = cbo_CrackDarkDefectFailCondition_Side.SelectedIndex;
        }

        private void cbo_ForeignMaterialBrightDefectFailCondition_Side_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_intForeignMaterialBrightDefectDimensionFailCondition = cbo_ForeignMaterialBrightDefectFailCondition_Side.SelectedIndex;
        }

        private void btn_PackageOffset_Click(object sender, EventArgs e)
        {
            this.TopMost = false;
            PackageOffsetSetting objform = new PackageOffsetSetting(m_smVisionInfo, m_smCustomizeInfo, m_smProductionInfo, m_smVisionInfo.g_intSelectedUnit, true);
            if (objform.ShowDialog() == DialogResult.Yes)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = true;
            }
        }

        private void dgd_CenterColor_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex == 1 || e.ColumnIndex == 5 || e.ColumnIndex == 7 || e.ColumnIndex == 9)
                return;

            float fValue = 0;

            switch (e.ColumnIndex)
            {
                case 2:
                    if (float.TryParse(dgd_CenterColor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, e.RowIndex);
                        m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[e.RowIndex]);
                        if (chk_SetToAllSideROI.Checked)
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[e.RowIndex]);
                        }
                    }
                    break;
                case 3:
                    int intValue = (dgd_CenterColor.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell).Items.IndexOf(dgd_CenterColor.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue);
                    m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionFailCondition(intValue, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[e.RowIndex]);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                            m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionFailCondition(intValue, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[e.RowIndex]);
                    }
                    break;
                case 4:
                    if (float.TryParse(dgd_CenterColor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, e.RowIndex);
                        m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[e.RowIndex]);
                        if (chk_SetToAllSideROI.Checked)
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[e.RowIndex]);
                        }
                    }
                    break;
                case 6:
                    if (float.TryParse(dgd_CenterColor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, e.RowIndex);
                        m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[e.RowIndex]);
                        if (chk_SetToAllSideROI.Checked)
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[e.RowIndex]);
                        }
                    }
                    break;
                case 8:
                    if (float.TryParse(dgd_CenterColor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, e.RowIndex);
                        m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[e.RowIndex]);
                        if (chk_SetToAllSideROI.Checked)
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[e.RowIndex]);
                        }
                    }
                    break;
                case 10:
                    if (float.TryParse(dgd_CenterColor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, e.RowIndex);
                        m_smVisionInfo.g_arrPad[0].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[e.RowIndex]);
                        if (chk_SetToAllSideROI.Checked)
                        {
                            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                                m_smVisionInfo.g_arrPad[i].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[0].ref_arrDefectColorThresName[e.RowIndex]);
                        }
                    }
                    break;
            }

            m_blnUpdateSelectedROISetting = true;
            UpdateSideColorGUI();
            m_blnUpdateSelectedROISetting = false;

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_SideColor_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex == 1 || e.ColumnIndex == 5 || e.ColumnIndex == 7 || e.ColumnIndex == 9 || e.ColumnIndex == 11 || e.ColumnIndex == 12)
                return;

            if (Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value) < 0)
            {
                return;
            }

            float fValue = 0;

            switch (e.ColumnIndex)
            {
                case 2:
                    if (float.TryParse(dgd_SideColor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value));
                        m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                        if (chk_SetToAllSideROI.Checked)
                        {
                            for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                            {
                                if (j == Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value))
                                    continue;

                                m_smVisionInfo.g_arrPad[j].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                            }
                        }
                        else
                        {
                            for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                            {
                                if (j == Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value) || (m_smVisionInfo.g_arrPad[j].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_intColorPadGroupIndex))
                                    continue;

                                m_smVisionInfo.g_arrPad[j].SetColorDefectInspectionWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                            }
                        }
                    }
                    break;
                case 3:
                    int intValue = (dgd_SideColor.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewComboBoxCell).Items.IndexOf(dgd_SideColor.Rows[e.RowIndex].Cells[e.ColumnIndex].EditedFormattedValue);
                    m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].SetColorDefectInspectionFailCondition(intValue, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                    if (chk_SetToAllSideROI.Checked)
                    {
                        for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                        {
                            if (j == Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value))
                                continue;

                            m_smVisionInfo.g_arrPad[j].SetColorDefectInspectionFailCondition(intValue, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                        }
                    }
                    else
                    {
                        for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                        {
                            if (j == Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value) || (m_smVisionInfo.g_arrPad[j].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_intColorPadGroupIndex))
                                continue;

                            m_smVisionInfo.g_arrPad[j].SetColorDefectInspectionFailCondition(intValue, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                        }
                    }
                    break;
                case 4:
                    if (float.TryParse(dgd_SideColor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value));
                        m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                        if (chk_SetToAllSideROI.Checked)
                        {
                            for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                            {
                                if (j == Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value))
                                    continue;

                                m_smVisionInfo.g_arrPad[j].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                            }
                        }
                        else
                        {
                            for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                            {
                                if (j == Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value) || (m_smVisionInfo.g_arrPad[j].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_intColorPadGroupIndex))
                                    continue;

                                m_smVisionInfo.g_arrPad[j].SetColorDefectInspectionLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                            }
                        }
                    }
                    break;
                case 6:
                    if (float.TryParse(dgd_SideColor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value));
                        m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                        if (chk_SetToAllSideROI.Checked)
                        {
                            for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                            {
                                if (j == Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value))
                                    continue;

                                m_smVisionInfo.g_arrPad[j].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                            }
                        }
                        else
                        {
                            for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                            {
                                if (j == Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value) || (m_smVisionInfo.g_arrPad[j].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_intColorPadGroupIndex))
                                    continue;

                                m_smVisionInfo.g_arrPad[j].SetColorDefectInspectionMinAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                            }
                        }
                    }
                    break;
                case 8:
                    if (float.TryParse(dgd_SideColor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value));
                        m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                        if (chk_SetToAllSideROI.Checked)
                        {
                            for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                            {
                                if (j == Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value))
                                    continue;

                                m_smVisionInfo.g_arrPad[j].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                            }
                        }
                        else
                        {
                            for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                            {
                                if (j == Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value) || (m_smVisionInfo.g_arrPad[j].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_intColorPadGroupIndex))
                                    continue;

                                m_smVisionInfo.g_arrPad[j].SetColorDefectInspectionMaxAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                            }
                        }
                    }
                    break;
                case 10:
                    if (float.TryParse(dgd_SideColor.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out fValue))
                    {
                        //m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value));
                        m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                        if (chk_SetToAllSideROI.Checked)
                        {
                            for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                            {
                                if (j == Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value))
                                    continue;

                                m_smVisionInfo.g_arrPad[j].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                            }
                        }
                        else
                        {
                            for (int j = 1; j < m_smVisionInfo.g_arrPad.Length; j++)
                            {
                                if (j == Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value) || (m_smVisionInfo.g_arrPad[j].ref_intColorPadGroupIndex != m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_intColorPadGroupIndex))
                                    continue;

                                m_smVisionInfo.g_arrPad[j].SetColorDefectInspectionTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay, m_smVisionInfo.g_arrPad[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[11].Value)].ref_arrDefectColorThresName[Convert.ToInt32(dgd_SideColor.Rows[e.RowIndex].Cells[12].Value)]);
                            }
                        }
                    }
                    break;
            }

            m_blnUpdateSelectedROISetting = true;
            UpdateSideColorGUI();
            m_blnUpdateSelectedROISetting = false;

            m_smVisionInfo.g_blnViewColorDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldFlashVerticalBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_MoldFlashVerticalBright.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetMoldFlashLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            //m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldFlashHorizontalBright_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_MoldFlashHorizontalBright.Text, out fValue))
            {
                m_smVisionInfo.g_arrPad[0].SetMoldFlashWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            //m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        private void txt_MoldFlashVerticalBright_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_MoldFlashVerticalBright_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetMoldFlashLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            //m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void txt_MoldFlashHorizontalBright_Side_TextChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            float fValue = 0;
            if (float.TryParse(txt_MoldFlashHorizontalBright_Side.Text, out fValue))
            {
                for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                    m_smVisionInfo.g_arrPad[i].SetMoldFlashWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            //m_smVisionInfo.g_blnViewPadPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void cbo_MoldFlashBrightDefectFailCondition_Center_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            m_smVisionInfo.g_arrPad[0].ref_intMoldFlashDefectDimensionFailCondition = cbo_MoldFlashBrightDefectFailCondition_Center.SelectedIndex;
        }

        private void cbo_MoldFlashBrightDefectFailCondition_Side_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnInitDone)
                return;

            if (m_blnUpdateSelectedROISetting)
                return;

            for (int i = 1; i < m_smVisionInfo.g_arrPad.Length; i++)
                m_smVisionInfo.g_arrPad[i].ref_intMoldFlashDefectDimensionFailCondition = cbo_MoldFlashBrightDefectFailCondition_Side.SelectedIndex;
        }
    }
}