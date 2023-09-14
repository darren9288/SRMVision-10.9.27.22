using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Common;
using VisionProcessing;
using SharedMemory;

namespace VisionProcessForm
{
    public partial class Lead3DPkgToleranceForm : Form
    {
        #region Enum
        public enum RowSettingName
        {
            PackageSize,
            XDimension,
            YDimension,
            //ZDimension,
            //Lead3DPackage,
            //Lead3DPkg_ScratchesLength,
            //Lead3DPkg_ScratchesArea,
            //Lead3DPkg_ChippedOffArea,
            //Lead3DPkg_ContaminationLength,
            //Lead3DPkg_ContaminationArea,
            //Lead3DPkg_ContaminationTotalArea,
            //Lead3DPkg_MoldFlashArea,
            //Lead3DPkg_VoidLength,
            //Lead3DPkg_VoidArea,
            //Lead3DPkg_CrackLength,
            //Lead3DPkg_CrackArea
        };
        #endregion

        #region Member Variables
        private int m_intVisionType = 0;
        private bool m_blnUpdateSelectedROISetting = false;
        private bool m_blnInitDone = false;
        private int m_intDecimal = 3;
        private int m_intDecimal2 = 6;
        private int m_intUserGroup = 5;
        private string m_strSelectedRecipe;
        private List<RowSettingName> m_arrRowSettingName = new List<RowSettingName>();

        private CustomOption m_smCustomizeInfo;
        private VisionInfo m_smVisionInfo;
        private ProductionInfo m_smProductionInfo;
        private UserRight m_objUserRight = new UserRight();
        
        #endregion
        public Lead3DPkgToleranceForm(CustomOption smCustomizeInfo, VisionInfo visionInfo, ProductionInfo smProductionInfo, string strSelectedRecipe, int intUserGroup, int intVisionType)
        {
            InitializeComponent();
            m_intVisionType = intVisionType;
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

            if (!m_smVisionInfo.g_blnWantCheckPH && !m_smVisionInfo.g_blnCheckPH)
            {
                // Clear ROI drag handler
                for (int i = 0; i < m_smVisionInfo.g_arrLeadROIs.Count; i++)
                {
                    if (m_smVisionInfo.g_arrLeadROIs[i][0].GetROIHandle())
                        m_smVisionInfo.g_arrLeadROIs[i][0].ClearDragHandle();
                }
            }
            else if (m_smVisionInfo.g_blnWantCheckPH && m_smVisionInfo.g_blnCheckPH)
            {
                if (m_smVisionInfo.g_arrPHROIs[0].GetROIHandle())
                    m_smVisionInfo.g_arrPHROIs[0].ClearDragHandle();
            }

            m_smVisionInfo.g_intSelectedROI = 0;
            DisableField2();
            UpdateGUI();

            m_blnInitDone = true;
        }

        private void DisableField()
        {
            string strChild1 = "Test Page";
            string strChild2 = "";

            strChild1 = "Tolerance Page";
            //strChild2 = "Pad Package Tolerance Setting";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    txt_BlackAreaArea.Enabled = false;
            //    dgd_Position.ReadOnly = true;
            //    dgd_PackageSetting.ReadOnly = true;
            //}

            strChild1 = "Tolerance Page";
            strChild2 = "Save Tolerance Button";
            if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
            }

            strChild1 = "Tolerance Page";
            //strChild2 = "Pad Package Size Offset Setting";
            //if (m_intUserGroup > m_objUserRight.GetGroupLevel3(strChild1, strChild2))
            //{
            //    group_PackageOffset.Visible = false;
            //}
            //else
            //{
            //    group_PackageOffset.Visible = true;
            //}
        }
        private void DisableField2()
        {
            m_intUserGroup = m_smProductionInfo.g_intUserGroup; // 2020-05-21 ZJYEOH : Need real time user group value as now got auto logout feature

            //NewUserRight objUserRight = new NewUserRight(false);
            string strChild1 = "Tolerance";
            string strChild2 = "";

            strChild2 = "Package TabPage";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild2Group(strChild1, strChild2))
            {
                group_GeneralDefectSetting.Enabled = false;
                group_ChippedOff.Enabled = false;
                group_MoldFlash.Enabled = false;
                group_Crack.Enabled = false;
                dgd_PackageSetting.Enabled = false;
            }

            strChild2 = "Position TabPage";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild2Group(strChild1, strChild2))
            {
                dgd_Position.Enabled = false;

            }

            strChild2 = "Pin 1 TabPage";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild2Group(strChild1, strChild2))
            {
                group_Pin1Setting.Enabled = false;
            }

            strChild2 = "PH TabPage";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild2Group(strChild1, strChild2))
            {
                group_PHSetting.Enabled = false;

            }

            strChild2 = "Save Button";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild2Group(strChild1, strChild2))
            {
                btn_Save.Enabled = false;
            }

            strChild2 = "Package Size Offset Setting";
            if (m_intUserGroup > m_smCustomizeInfo.objNewUserRight.GetLead3DChild2Group(strChild1, strChild2))
            {
                btn_PackageOffset.Enabled = false;
            }
            else
            {
                btn_PackageOffset.Enabled = true;
            }
        }
        private void AddRowsToTable()
        {
            m_arrRowSettingName.Clear();

            m_arrRowSettingName.Add(RowSettingName.PackageSize);
            m_arrRowSettingName.Add(RowSettingName.XDimension);
            m_arrRowSettingName.Add(RowSettingName.YDimension);
            //if (m_smVisionInfo.g_strVisionName.Contains("5S"))
            //    m_arrRowSettingName.Add(RowSettingName.ZDimension);

            //if (m_smVisionInfo.g_arrLead3D[0].ref_blnUseDetailDefectCriteria)
            //{
            //    m_arrRowSettingName.Add(RowSettingName.Lead3DPackage);
            //    m_arrRowSettingName.Add(RowSettingName.Lead3DPkg_ChippedOffArea);
            //    m_arrRowSettingName.Add(RowSettingName.Lead3DPkg_ScratchesLength);
            //    m_arrRowSettingName.Add(RowSettingName.Lead3DPkg_ScratchesArea);
            //    m_arrRowSettingName.Add(RowSettingName.Lead3DPkg_ContaminationArea);
            //    m_arrRowSettingName.Add(RowSettingName.Lead3DPkg_ContaminationTotalArea);
            //    m_arrRowSettingName.Add(RowSettingName.Lead3DPkg_ContaminationLength);
            //    m_arrRowSettingName.Add(RowSettingName.Lead3DPkg_MoldFlashArea);
            //    m_arrRowSettingName.Add(RowSettingName.Lead3DPkg_VoidLength);
            //    m_arrRowSettingName.Add(RowSettingName.Lead3DPkg_VoidArea);
            //    m_arrRowSettingName.Add(RowSettingName.Lead3DPkg_CrackLength);
            //    m_arrRowSettingName.Add(RowSettingName.Lead3DPkg_CrackArea);
                
            //}

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
                    //case RowSettingName.ZDimension:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Z 尺寸";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Z Dimension";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "---";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                    //    }
                    //    break;
                    //case RowSettingName.Lead3DPackage:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "中部 Package";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Center Package";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "";
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "";
                    //        dgd_PackageSetting.Rows[i].Cells[0].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[0].Style.SelectionBackColor = Color.LightGray;
                    //        dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.LightGray;
                    //        dgd_PackageSetting.Rows[i].Cells[2].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[2].Style.SelectionBackColor = Color.LightGray;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.LightGray;
                    //        dgd_PackageSetting.Rows[i].Cells[4].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[4].Style.SelectionBackColor = Color.LightGray;
                    //    }
                    //    break;
                    //case RowSettingName.Lead3DPkg_ScratchesLength:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "刮伤长度";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Scratches Length";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                    //        dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                    //        dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                    //    }
                    //    break;
                    //case RowSettingName.Lead3DPkg_ScratchesArea:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "刮伤面积";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Scratches Area";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                    //        dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                    //        dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                    //    }
                    //    break;
                    //case RowSettingName.Lead3DPkg_ChippedOffArea:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Chipped Off 面积";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Chipped Off Area";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                    //        dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                    //        dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                    //    }
                    //    break;
                    //case RowSettingName.Lead3DPkg_ContaminationLength:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "污染长度";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Cont. Length";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                    //        dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                    //        dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                    //    }
                    //    break;
                    //case RowSettingName.Lead3DPkg_ContaminationArea:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "污染面积";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Cont. Area";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                    //        dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                    //        dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                    //    }
                    //    break;
                    //case RowSettingName.Lead3DPkg_ContaminationTotalArea:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "污染总面积";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Cont. Total Area";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                    //        dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                    //        dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                    //    }
                    //    break;
                    //case RowSettingName.Lead3DPkg_MoldFlashArea:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Mold Flash 面积";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Mold Flash Area";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                    //        dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                    //        dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                    //    }
                    //    break;
                    //case RowSettingName.Lead3DPkg_VoidLength:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Void 长度";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Void Length";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                    //        dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                    //        dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                    //    }
                    //    break;
                    //case RowSettingName.Lead3DPkg_VoidArea:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Void 面积";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Void Area";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                    //        dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                    //        dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                    //    }
                    //    break;
                    //case RowSettingName.Lead3DPkg_CrackLength:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "裂缝长度";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Crack Length";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "mm";
                    //        dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                    //        dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                    //    }
                    //    break;
                    //case RowSettingName.Lead3DPkg_CrackArea:
                    //    {
                    //        dgd_PackageSetting.Rows.Add();
                    //        if (m_smCustomizeInfo.g_intLanguageCulture == 2)
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "裂缝面积";
                    //        else
                    //            dgd_PackageSetting.Rows[i].Cells[0].Value = "Crack Area";
                    //        dgd_PackageSetting.Rows[i].Cells[1].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[2].Value = "0";
                    //        dgd_PackageSetting.Rows[i].Cells[3].Value = "NA";
                    //        dgd_PackageSetting.Rows[i].Cells[4].Value = "mm^2";
                    //        dgd_PackageSetting.Rows[i].Cells[1].ReadOnly = true;
                    //        dgd_PackageSetting.Rows[i].Cells[1].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[1].Style.SelectionBackColor = Color.DarkGray;
                    //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.DarkGray;
                    //    }
                    //    break;
                }
            }
        }

        private void UpdateGUI()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey("Software", true);
            RegistryKey subkey1 = key.CreateSubKey("SVG\\AutoMode");
            chk_DisplayResult.Checked = Convert.ToBoolean(subkey1.GetValue("CurrentDisplayResult_MoTolerance", false));

            if (m_intVisionType > 0)
            {
                //if (tab_VisionControl.TabPages.Contains(tp_Package))
                //    tab_VisionControl.Controls.Remove(tp_Package);

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateChippedOffDefectSetting)
                    group_ChippedOff.Visible = true;
                else
                    group_ChippedOff.Visible = false;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateMoldFlashDefectSetting)
                    group_MoldFlash.Visible = true;
                else
                    group_MoldFlash.Visible = false;

                if (m_smVisionInfo.g_arrLead3D[0].ref_blnSeperateCrackDefectSetting)
                    group_Crack.Visible = true;
                else
                    group_Crack.Visible = false;

                UpdateUnitDisplay();
                AddRowsToTable();           // Add row to table


                UpdateSettingGUI();         // Update table's setting column with setting value
                UpdateInfo();
            }
            else
            {
                if (tab_VisionControl.TabPages.Contains(tp_Package))
                    tab_VisionControl.TabPages.Remove(tp_Package);

                if (tab_VisionControl.TabPages.Contains(tp_PackageSimple))
                    tab_VisionControl.TabPages.Remove(tp_PackageSimple);
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

            UpdatePosition();

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

            ViewOrHideResultColumn(chk_DisplayResult.Checked);
            m_smVisionInfo.PR_TL_UpdateInfo2 = false;
        }

        private void UpdatePosition()
        {
            dgd_Position.Rows.Clear();

            float Angle = 0, XTolerance = 0, YTolerance = 0;
            m_smVisionInfo.g_arrLead3D[0].GetPositionResult_PatternMatch(ref Angle, ref XTolerance, ref YTolerance);

            dgd_Position.Rows.Add();
            dgd_Position.Rows[0].Cells[0].Value = "Lead3D Pkg";
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
            dgd_Position.Rows[1].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fAngleTolerance.ToString("f4");
            dgd_Position.Rows[1].Cells[2].Value = Angle.ToString("f4");
            //dgd_Position.Rows[1].Cells[0].Style.BackColor = dgd_Position.Rows[1].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //dgd_Position.Rows[1].Cells[1].Style.BackColor = dgd_Position.Rows[1].Cells[1].Style.SelectionBackColor = Color.LightGray;
            if (Math.Abs(Angle) >= m_smVisionInfo.g_arrLead3D[0].ref_fAngleTolerance)
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
            dgd_Position.Rows[2].Cells[0].Value = "X Tol.";
            dgd_Position.Rows[2].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fXTolerance.ToString("f4");
            dgd_Position.Rows[2].Cells[2].Value = XTolerance.ToString("f4");
            //dgd_Position.Rows[2].Cells[0].Style.BackColor = dgd_Position.Rows[2].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //dgd_Position.Rows[2].Cells[1].Style.BackColor = dgd_Position.Rows[2].Cells[1].Style.SelectionBackColor = Color.LightGray;

            if (Math.Abs(XTolerance) >= m_smVisionInfo.g_arrLead3D[0].ref_fXTolerance)
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
            dgd_Position.Rows[3].Cells[0].Value = "Y Tol.";
            dgd_Position.Rows[3].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fYTolerance.ToString("f4");
            dgd_Position.Rows[3].Cells[2].Value = YTolerance.ToString("f4");
            //dgd_Position.Rows[3].Cells[0].Style.BackColor = dgd_Position.Rows[3].Cells[0].Style.SelectionBackColor = Color.LightGray;
            //dgd_Position.Rows[3].Cells[1].Style.BackColor = dgd_Position.Rows[3].Cells[1].Style.SelectionBackColor = Color.LightGray;
            if (Math.Abs(YTolerance) >= m_smVisionInfo.g_arrLead3D[0].ref_fYTolerance)
            {
                dgd_Position.Rows[3].Cells[2].Style.ForeColor = dgd_Position.Rows[3].Cells[2].Style.SelectionForeColor = Color.Yellow;
                dgd_Position.Rows[3].Cells[2].Style.BackColor = dgd_Position.Rows[3].Cells[2].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Position.Rows[3].Cells[2].Style.ForeColor = dgd_Position.Rows[3].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[3].Cells[2].Style.BackColor = dgd_Position.Rows[3].Cells[2].Style.SelectionBackColor = Color.Lime;
            }

            dgd_Position.Rows.Add();
            dgd_Position.Rows[4].Cells[0].Value = "Base Line Angle(Top)";
            dgd_Position.Rows[4].Cells[1].Value = m_smVisionInfo.g_arrLead3D[1].ref_fBaseLineMaxAngle.ToString("f4");
            dgd_Position.Rows[4].Cells[2].Value = m_smVisionInfo.g_arrLead3D[1].ref_fBaseLineAngle.ToString("f4");
            if (Math.Abs(m_smVisionInfo.g_arrLead3D[1].ref_fBaseLineAngle) >= m_smVisionInfo.g_arrLead3D[1].ref_fBaseLineMaxAngle)
            {
                dgd_Position.Rows[4].Cells[2].Style.ForeColor = dgd_Position.Rows[4].Cells[2].Style.SelectionForeColor = Color.Yellow;
                dgd_Position.Rows[4].Cells[2].Style.BackColor = dgd_Position.Rows[4].Cells[2].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Position.Rows[4].Cells[2].Style.ForeColor = dgd_Position.Rows[4].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[4].Cells[2].Style.BackColor = dgd_Position.Rows[4].Cells[2].Style.SelectionBackColor = Color.Lime;
            }

            dgd_Position.Rows.Add();
            dgd_Position.Rows[5].Cells[0].Value = "Base Line Angle(Right)";
            dgd_Position.Rows[5].Cells[1].Value = m_smVisionInfo.g_arrLead3D[2].ref_fBaseLineMaxAngle.ToString("f4");
            dgd_Position.Rows[5].Cells[2].Value = m_smVisionInfo.g_arrLead3D[2].ref_fBaseLineAngle.ToString("f4");
            if (Math.Abs(m_smVisionInfo.g_arrLead3D[2].ref_fBaseLineAngle) >= m_smVisionInfo.g_arrLead3D[2].ref_fBaseLineMaxAngle)
            {
                dgd_Position.Rows[5].Cells[2].Style.ForeColor = dgd_Position.Rows[5].Cells[2].Style.SelectionForeColor = Color.Yellow;
                dgd_Position.Rows[5].Cells[2].Style.BackColor = dgd_Position.Rows[5].Cells[2].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Position.Rows[5].Cells[2].Style.ForeColor = dgd_Position.Rows[5].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[5].Cells[2].Style.BackColor = dgd_Position.Rows[5].Cells[2].Style.SelectionBackColor = Color.Lime;
            }

            dgd_Position.Rows.Add();
            dgd_Position.Rows[6].Cells[0].Value = "Base Line Angle(Bottom)";
            dgd_Position.Rows[6].Cells[1].Value = m_smVisionInfo.g_arrLead3D[3].ref_fBaseLineMaxAngle.ToString("f4");
            dgd_Position.Rows[6].Cells[2].Value = m_smVisionInfo.g_arrLead3D[3].ref_fBaseLineAngle.ToString("f4");
            if (Math.Abs(m_smVisionInfo.g_arrLead3D[3].ref_fBaseLineAngle) >= m_smVisionInfo.g_arrLead3D[3].ref_fBaseLineMaxAngle)
            {
                dgd_Position.Rows[6].Cells[2].Style.ForeColor = dgd_Position.Rows[6].Cells[2].Style.SelectionForeColor = Color.Yellow;
                dgd_Position.Rows[6].Cells[2].Style.BackColor = dgd_Position.Rows[6].Cells[2].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Position.Rows[6].Cells[2].Style.ForeColor = dgd_Position.Rows[6].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[6].Cells[2].Style.BackColor = dgd_Position.Rows[6].Cells[2].Style.SelectionBackColor = Color.Lime;
            }

            dgd_Position.Rows.Add();
            dgd_Position.Rows[7].Cells[0].Value = "Base Line Angle(Left)";
            dgd_Position.Rows[7].Cells[1].Value = m_smVisionInfo.g_arrLead3D[4].ref_fBaseLineMaxAngle.ToString("f4");
            dgd_Position.Rows[7].Cells[2].Value = m_smVisionInfo.g_arrLead3D[4].ref_fBaseLineAngle.ToString("f4");
            if (Math.Abs(m_smVisionInfo.g_arrLead3D[4].ref_fBaseLineAngle) >= m_smVisionInfo.g_arrLead3D[4].ref_fBaseLineMaxAngle)
            {
                dgd_Position.Rows[7].Cells[2].Style.ForeColor = dgd_Position.Rows[7].Cells[2].Style.SelectionForeColor = Color.Yellow;
                dgd_Position.Rows[7].Cells[2].Style.BackColor = dgd_Position.Rows[7].Cells[2].Style.SelectionBackColor = Color.Red;
            }
            else
            {
                dgd_Position.Rows[7].Cells[2].Style.ForeColor = dgd_Position.Rows[7].Cells[2].Style.SelectionForeColor = Color.Black;
                dgd_Position.Rows[7].Cells[2].Style.BackColor = dgd_Position.Rows[7].Cells[2].Style.SelectionBackColor = Color.Lime;
            }


            //if (!m_smVisionInfo.g_arrLead3D[0].ref_blnLeadFound)
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

        }
        private void UpdateSettingGUI()
        {
            for (int i = 0; i < m_arrRowSettingName.Count; i++)
            {
                switch (m_arrRowSettingName[i])
                {
                    case RowSettingName.XDimension:
                        dgd_PackageSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].GetUnitWidthMin(1).ToString("F" + m_intDecimal);
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetUnitWidthMax(1).ToString("F" + m_intDecimal);
                        break;
                    case RowSettingName.YDimension:
                        dgd_PackageSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].GetUnitHeightMin(1).ToString("F" + m_intDecimal);
                        dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetUnitHeightMax(1).ToString("F" + m_intDecimal);
                        break;
                    //case RowSettingName.ZDimension:
                    //    dgd_PackageSetting.Rows[i].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].GetUnitThicknessMin(1).ToString("F" + m_intDecimal);
                    //    dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetUnitThicknessMax(1).ToString("F" + m_intDecimal);
                    //    break;
                    //case RowSettingName.Lead3DPkg_ScratchesLength:
                    //    dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetScratchLengthLimit(1).ToString("F" + m_intDecimal);
                    //    break;
                    //case RowSettingName.Lead3DPkg_ScratchesArea:
                    //    dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetScratchAreaLimit(1).ToString("F" + m_intDecimal2);
                    //    break;
                    //case RowSettingName.Lead3DPkg_ChippedOffArea:
                    //    dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetChipAreaLimit(1).ToString("F" + m_intDecimal2);
                    //    break;
                    //case RowSettingName.Lead3DPkg_ContaminationLength:
                    //    dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetExtraPadLengthLimit(1).ToString("F" + m_intDecimal);
                    //    break;
                    //case RowSettingName.Lead3DPkg_ContaminationArea:
                    //    dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetExtraPadMinArea(1).ToString("F" + m_intDecimal2);
                    //    break;
                    //case RowSettingName.Lead3DPkg_ContaminationTotalArea:
                    //    dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetTotalExtraPadMinArea(1).ToString("F" + m_intDecimal2);
                    //    break;
                    //case RowSettingName.Lead3DPkg_MoldFlashArea:
                    //    dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetMoldFlashAreaLimit(1).ToString("F" + m_intDecimal2);
                    //    break;
                    //case RowSettingName.Lead3DPkg_VoidLength:
                    //    dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetVoidLengthLimit(1).ToString("F" + m_intDecimal);
                    //    break;
                    //case RowSettingName.Lead3DPkg_VoidArea:
                    //    dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetVoidAreaLimit(1).ToString("F" + m_intDecimal);
                    //    break;
                    //case RowSettingName.Lead3DPkg_CrackLength:
                    //    dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetCrackLengthLimit(1).ToString("F" + m_intDecimal);
                    //    break;
                    //case RowSettingName.Lead3DPkg_CrackArea:
                    //    dgd_PackageSetting.Rows[i].Cells[2].Value = m_smVisionInfo.g_arrLead3D[0].GetCrackAreaLimit(1).ToString("F" + m_intDecimal);
                    //    break;
                }
            }

            //General
            txt_GeneralVerticalBright.Text = m_smVisionInfo.g_arrLead3D[0].GetBrightLengthLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralVerticalDark.Text = m_smVisionInfo.g_arrLead3D[0].GetDarkLengthLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralHorizontalBright.Text = m_smVisionInfo.g_arrLead3D[0].GetBrightWidthLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralHorizontalDark.Text = m_smVisionInfo.g_arrLead3D[0].GetDarkWidthLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralAreaBright.Text = m_smVisionInfo.g_arrLead3D[0].GetBrightAreaLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralAreaDark.Text = m_smVisionInfo.g_arrLead3D[0].GetDarkAreaLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralTotalAreaBright.Text = m_smVisionInfo.g_arrLead3D[0].GetBrightTotalAreaLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralTotalAreaDark.Text = m_smVisionInfo.g_arrLead3D[0].GetDarkTotalAreaLimit(1).ToString("F" + m_intDecimal);
            //txt_GeneralChipAreaBright.Text = m_smVisionInfo.g_arrLead3D[0].GetBrightLengthLimit(1).ToString("F" + m_intDecimal);
            //txt_GeneralChipAreaDark.Text = m_smVisionInfo.g_arrLead3D[0].GetBrightLengthLimit(1).ToString("F" + m_intDecimal);


            txt_CrackAreaDark.Text = m_smVisionInfo.g_arrLead3D[0].GetCrackAreaLimit(1).ToString("F" + m_intDecimal);
            txt_MoldFlashAreaBright.Text = m_smVisionInfo.g_arrLead3D[0].GetMoldFlashAreaLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralChipAreaBright.Text = m_smVisionInfo.g_arrLead3D[0].GetBrightChippedOffAreaLimit(1).ToString("F" + m_intDecimal);
            txt_GeneralChipAreaDark.Text = m_smVisionInfo.g_arrLead3D[0].GetDarkChippedOffAreaLimit(1).ToString("F" + m_intDecimal);
            txt_CrackVerticalDark.Text = m_smVisionInfo.g_arrLead3D[0].GetDarkVerticalCrackLimit(1).ToString("F" + m_intDecimal);
            txt_CrackHorizontalDark.Text = m_smVisionInfo.g_arrLead3D[0].GetDarkHorizontalCrackLimit(1).ToString("F" + m_intDecimal);
            
            //// Package Size Offset
            //txt_WidthOffset.Text = m_smVisionInfo.g_arrLead3D[0].ref_fPackageWidthOffsetMM.ToString();
            //txt_HeightOffset.Text = m_smVisionInfo.g_arrLead3D[0].ref_fPackageHeightOffsetMM.ToString();
            //txt_ThicknessOffset.Text = m_smVisionInfo.g_arrLead3D[1].ref_fPackageThicknessOffsetMM.ToString();
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
                        if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                            fWidth = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitWidthMM;
                        else
                            fWidth = (m_smVisionInfo.g_arrLead3D[0].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[0].GetResultDownWidth_RectGauge4L(1)) / 2;

                        //// 2019-10-25 ZJYEOH : Add Offset to package width
                        //fWidth += m_smVisionInfo.g_arrLead3D[0].ref_fPackageWidthOffsetMM;

                        dgd_PackageSetting.Rows[i].Cells[3].Value = fWidth.ToString("F" + m_intDecimal);

                        if (fWidth > m_smVisionInfo.g_arrLead3D[0].GetUnitWidthMax(1) || fWidth < m_smVisionInfo.g_arrLead3D[0].GetUnitWidthMin(1))
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
                        if (m_smVisionInfo.g_arrLead3D[0].ref_blnMeasureCenterPkgSizeUsingCorner)
                            fHeight = m_smVisionInfo.g_arrLead3D[0].ref_fCenterUnitHeightMM;
                        else
                            fHeight = (m_smVisionInfo.g_arrLead3D[0].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[0].GetResultRightHeight_RectGauge4L(1)) / 2;

                        //// 2019-10-25 ZJYEOH : Add Offset to package height
                        //fHeight += m_smVisionInfo.g_arrLead3D[0].ref_fPackageHeightOffsetMM;

                        dgd_PackageSetting.Rows[i].Cells[3].Value = fHeight.ToString("F" + m_intDecimal);

                        if (fHeight > m_smVisionInfo.g_arrLead3D[0].GetUnitHeightMax(1) || fHeight < m_smVisionInfo.g_arrLead3D[0].GetUnitHeightMin(1))
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
                        //case RowSettingName.ZDimension:
                        //    float fTotalThinkness = 0;
                        //    int intCount = 0;
                        //    for (int j = 1; j < m_smVisionInfo.g_arrLead3D.Length; j++)
                        //    {
                        //        if (j == 1 || j == 3)
                        //            fTotalThinkness += m_smVisionInfo.g_arrLead3D[j].GetResultLeftHeight_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[j].GetResultRightHeight_RectGauge4L(1);
                        //        else
                        //            fTotalThinkness += m_smVisionInfo.g_arrLead3D[j].GetResultUpWidth_RectGauge4L(1) + m_smVisionInfo.g_arrLead3D[j].GetResultDownWidth_RectGauge4L(1);

                        //        intCount += 2;
                        //    }
                        //    fThickness = fTotalThinkness / intCount;

                        //    // 2019-10-25 ZJYEOH : Add Offset to package thickness
                        //    fThickness += m_smVisionInfo.g_arrLead3D[1].ref_fPackageThicknessOffsetMM;

                        //    dgd_PackageSetting.Rows[i].Cells[3].Value = fThickness.ToString("F" + m_intDecimal);

                        //    if (fThickness > m_smVisionInfo.g_arrLead3D[0].GetUnitThicknessMax(1) || fThickness < m_smVisionInfo.g_arrLead3D[0].GetUnitThicknessMin(1))
                        //    {
                        //        dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Yellow;
                        //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Red;
                        //    }
                        //    else
                        //    {
                        //        dgd_PackageSetting.Rows[i].Cells[3].Style.ForeColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionForeColor = Color.Black;
                        //        dgd_PackageSetting.Rows[i].Cells[3].Style.BackColor = dgd_PackageSetting.Rows[i].Cells[3].Style.SelectionBackColor = Color.Lime;
                        //    }
                        //    break;
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
        /// Load Lead3D settings from selected path
        /// </summary>
        /// <param name="strPath">selected path</param>
        private void LoadLead3DSetting(string strPath)
        {
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

                XmlParser objFile;
                // Load Lead3D Advance Setting
                objFile = new XmlParser(strPath + "Settings.xml");
                objFile.GetFirstSection("Advanced");
                m_smVisionInfo.g_arrLead3D[i].ref_blnWhiteOnBlack = objFile.GetValueAsBoolean("WhiteOnBlack", true, 1);

                // Load Lead3D Template Setting
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

            }
        }

        /// <summary>
        /// Save Lead3D settings to xml
        /// </summary>
        /// <param name="strFolderPath">xml folder path</param>
        /// <param name="blnNewRecipe"></param>
        private void SaveLead3DSetting(string strFolderPath)
        {
            
            STDeviceEdit.CopySettingFile(strFolderPath, "Template\\Template.xml");
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

                
                m_smVisionInfo.g_arrLead3D[i].SaveLead3D(strFolderPath + "Template\\Template.xml",
                    false, strSectionName, true);
                
            }
            STDeviceEdit.XMLChangesTracing(m_smVisionInfo.g_strVisionFolderName + ">Lead3DPackage Tolerance Setting", m_smProductionInfo.g_strLotID);
            
        }

        private void ViewOrHideResultColumn(bool blnWantView)
        {
            if (m_intVisionType > 0)
                dgd_PackageSetting.Columns[3].Visible = blnWantView;

            dgd_Position.Columns[2].Visible = blnWantView;

            lbl_PHBlobBlackArea.Visible = blnWantView;
            srmLabel17.Visible = blnWantView;
            lbl_Pin1Score.Visible = lbl_Pin1ScorePercent.Visible = lbl_PinScoreTitle.Visible = blnWantView;

        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            string strPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" +
                            m_smVisionInfo.g_strVisionFolderName + "\\";

            SaveLead3DSetting(strPath + "Lead3D\\");
            m_smVisionInfo.g_objPositioning.SavePosition(strPath + "Positioning\\Settings.xml", false, "General", true);

            if (m_smVisionInfo.g_arrPin1 != null)
            {
                m_smVisionInfo.g_arrPin1[0].SaveTemplate(strPath + "Lead3D\\Template\\");
            }

            if (m_smCustomizeInfo.g_blnConfigShowNetwork && m_smCustomizeInfo.g_blnWantNetwork)
                m_smProductionInfo.g_blnSaveRecipeToServer = true;

            this.Close();
            this.Dispose();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            string strFolderPath = m_smProductionInfo.g_strRecipePath + m_strSelectedRecipe + "\\" + m_smVisionInfo.g_strVisionFolderName + "\\";

            LoadLead3DSetting(strFolderPath + "Lead3D\\");
            m_smVisionInfo.g_objPositioning.LoadPosition(strFolderPath + "Positioning\\Settings.xml", "General");
            if (m_smVisionInfo.g_arrPin1 != null)
            {
                m_smVisionInfo.g_arrPin1[0].LoadTemplate(strFolderPath + "Lead3D\\Template\\");
            }

            this.Close();
            this.Dispose();
        }

        private void timer_Lead3D_Tick(object sender, EventArgs e)
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

            if (m_smVisionInfo.PR_TL_UpdateInfo2)
            {
                if (m_intVisionType > 0)
                    UpdateInfo();
                m_smVisionInfo.PR_TL_UpdateInfo2 = false;
            }
        }

        private void Lead3DPackageToleranceSettingForm_Load(object sender, EventArgs e)
        {
            m_smVisionInfo.g_blnViewLeadInspection = false;
            m_smVisionInfo.g_blnViewLead3DSettingDrawing = true;
            m_smVisionInfo.VM_AT_SettingInDialog = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void Lead3DPackageToleranceSettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            STDeviceEdit.SaveDeviceEditLog(m_smVisionInfo.g_strVisionFolderName + ">Lead3D Package Tolerance Setting Closed", "Exit Lead3D Package Tolerance Form", "", "", m_smProductionInfo.g_strLotID);
            
            m_smVisionInfo.g_blnViewLeadInspection = false;
            m_smVisionInfo.g_blnViewLead3DSettingDrawing = false;
            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = false;
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
                                        for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                            m_smVisionInfo.g_arrLead3D[i].SetUnitWidthMin(fValue, m_smCustomizeInfo.g_intUnitDisplay);
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
                                        for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                            m_smVisionInfo.g_arrLead3D[i].SetUnitWidthMax(fValue, m_smCustomizeInfo.g_intUnitDisplay);
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
                                        for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                            m_smVisionInfo.g_arrLead3D[i].SetUnitHeightMin(fValue, m_smCustomizeInfo.g_intUnitDisplay);
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
                                        for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                                            m_smVisionInfo.g_arrLead3D[i].SetUnitHeightMax(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                                    }
                                }
                            }
                        }
                    }
                    break;
                //case RowSettingName.ZDimension:
                //    {
                //        if (c == 1)
                //        {
                //            float fValue;
                //            if (dgd_PackageSetting.Rows[r].Cells[1].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[1].Value.ToString(), out fValue))
                //            {
                //                float fMaxValue = 0;
                //                if (float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fMaxValue))
                //                {
                //                    if (fValue > fMaxValue)
                //                    {
                //                        SRMMessageBox.Show("Setting Fail. Min value cannot bigger than Max value.");
                //                    }
                //                    else
                //                    {
                //                        for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                //                            m_smVisionInfo.g_arrLead3D[i].SetUnitThicknessMin(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //                    }
                //                }
                //            }
                //        }
                //        else if (c == 2)
                //        {
                //            float fValue;
                //            if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                //            {
                //                float fMinValue = 0;
                //                if (float.TryParse(dgd_PackageSetting.Rows[r].Cells[1].Value.ToString(), out fMinValue))
                //                {
                //                    if (fValue < fMinValue)
                //                    {
                //                        SRMMessageBox.Show("Setting Fail. Max value cannot smaller than Min value.");
                //                    }
                //                    else
                //                    {
                //                        for (int i = 0; i < m_smVisionInfo.g_arrLead3D.Length; i++)
                //                            m_smVisionInfo.g_arrLead3D[i].SetUnitThicknessMax(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_ScratchesLength:
                //    {
                //        float fValue;
                //        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                //        {
                //            m_smVisionInfo.g_arrLead3D[0].SetScratchLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //        }
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_ScratchesArea:
                //    {
                //        float fValue;
                //        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                //        {
                //            m_smVisionInfo.g_arrLead3D[0].SetScratchAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //        }
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_ChippedOffArea:
                //    {
                //        float fValue;
                //        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                //        {
                //            m_smVisionInfo.g_arrLead3D[0].SetChipAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //        }
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_ContaminationLength:
                //    {
                //        float fValue;
                //        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                //        {
                //            m_smVisionInfo.g_arrLead3D[0].SetExtraPadLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //        }
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_ContaminationArea:
                //    {
                //        float fValue;
                //        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                //        {
                //            m_smVisionInfo.g_arrLead3D[0].SetExtraPadMinArea(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //        }
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_ContaminationTotalArea:
                //    {
                //        float fValue;
                //        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                //        {
                //            m_smVisionInfo.g_arrLead3D[0].SetTotalExtraPadMinArea(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //        }
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_MoldFlashArea:
                //    {
                //        float fValue;
                //        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                //        {
                //            m_smVisionInfo.g_arrLead3D[0].SetMoldFlashAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //        }
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_VoidLength:
                //    {
                //        float fValue;
                //        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                //        {
                //            m_smVisionInfo.g_arrLead3D[0].SetVoidLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //        }
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_VoidArea:
                //    {
                //        float fValue;
                //        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                //        {
                //            m_smVisionInfo.g_arrLead3D[0].SetVoidAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //        }
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_CrackLength:
                //    {
                //        float fValue;
                //        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                //        {
                //            m_smVisionInfo.g_arrLead3D[0].SetCrackLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //        }
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_CrackArea:
                //    {
                //        float fValue;
                //        if (dgd_PackageSetting.Rows[r].Cells[2].Value != null && float.TryParse(dgd_PackageSetting.Rows[r].Cells[2].Value.ToString(), out fValue))
                //        {
                //            m_smVisionInfo.g_arrLead3D[0].SetCrackAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
                //        }
                //    }
                //    break;
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
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
                //case RowSettingName.ZDimension:
                //    {
                //        m_smVisionInfo.g_intSelectedPackageDefect = 2;
                //        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                //        if (c == 1)
                //            m_smVisionInfo.g_blnViewPackageMinDefect = true;
                //        else if (c == 2)
                //            m_smVisionInfo.g_blnViewPackageMinDefect = false;
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_ScratchesLength:
                //    {
                //        m_smVisionInfo.g_intSelectedPackageDefect = 3;
                //        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_ScratchesArea:
                //    {
                //        m_smVisionInfo.g_intSelectedPackageDefect = 4;
                //        m_smVisionInfo.g_blnViewPackageAreaDefect = true;
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_ChippedOffArea:
                //    {
                //        m_smVisionInfo.g_intSelectedPackageDefect = 5;
                //        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_ContaminationLength:
                //    {
                //        m_smVisionInfo.g_intSelectedPackageDefect = 15;
                //        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_ContaminationArea:
                //    {
                //        m_smVisionInfo.g_intSelectedPackageDefect = 16;
                //        m_smVisionInfo.g_blnViewPackageAreaDefect = true;
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_ContaminationTotalArea:
                //    {
                //        m_smVisionInfo.g_intSelectedPackageDefect = 17;
                //        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_MoldFlashArea:
                //    {
                //        m_smVisionInfo.g_intSelectedPackageDefect = 6;
                //        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_VoidLength:
                //    {
                //        m_smVisionInfo.g_intSelectedPackageDefect = 11;
                //        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_VoidArea:
                //    {
                //        m_smVisionInfo.g_intSelectedPackageDefect = 12;
                //        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_CrackLength:
                //    {
                //        m_smVisionInfo.g_intSelectedPackageDefect = 13;
                //        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                //    }
                //    break;
                //case RowSettingName.Lead3DPkg_CrackArea:
                //    {
                //        m_smVisionInfo.g_intSelectedPackageDefect = 14;
                //        m_smVisionInfo.g_blnViewPackageAreaDefect = false;
                //    }
                //    break;
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }

        private void dgd_PackageSetting_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = false;
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
                    m_smVisionInfo.g_blnViewLead3DPositionSetting = true;
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
            m_smVisionInfo.g_blnViewLead3DPositionSetting = false;
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
                        {
                            if(fMaxValue > 10)
                            {
                                SRMMessageBox.Show("Please set value less than 10!");
                                dgd_Position.Rows[1].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fAngleTolerance.ToString("f4");
                            }
                            m_smVisionInfo.g_arrLead3D[0].ref_fAngleTolerance = fMaxValue;
                        }
                        else
                        {
                            SRMMessageBox.Show("Please enter positive value!");
                            dgd_Position.Rows[1].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fAngleTolerance.ToString("f4");
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Please enter valid numerical value!");
                        dgd_Position.Rows[1].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fAngleTolerance.ToString("f4");
                    }
                    break;
                case 2:
                    if (float.TryParse(dgd_Position.Rows[2].Cells[1].Value.ToString(), out fMaxValue))
                    {
                        if (fMaxValue > 0)
                        {
                            m_smVisionInfo.g_arrLead3D[0].ref_fXTolerance = fMaxValue;
                            m_smVisionInfo.g_blnViewLead3DPositionSetting = true;
                        }
                        else
                        {
                            SRMMessageBox.Show("Please enter positive value!");
                            dgd_Position.Rows[2].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fXTolerance.ToString("f4");
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Please enter numerical value!");
                        dgd_Position.Rows[2].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fXTolerance.ToString("f4");
                    }
                    break;
                case 3:
                    if (float.TryParse(dgd_Position.Rows[3].Cells[1].Value.ToString(), out fMaxValue))
                    {
                        if (fMaxValue > 0)
                        {
                            m_smVisionInfo.g_arrLead3D[0].ref_fYTolerance = fMaxValue;
                            m_smVisionInfo.g_blnViewLead3DPositionSetting = true;
                        }
                        else
                        {
                            SRMMessageBox.Show("Please enter positive value!");
                            dgd_Position.Rows[3].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fYTolerance.ToString("f4");
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Please enter numerical value!");
                        dgd_Position.Rows[3].Cells[1].Value = m_smVisionInfo.g_arrLead3D[0].ref_fYTolerance.ToString("f4");
                    }
                    break;
                case 4:
                    if (float.TryParse(dgd_Position.Rows[4].Cells[1].Value.ToString(), out fMaxValue))
                    {
                        if (fMaxValue > 0)
                        {
                            if (fMaxValue > 10)
                            {
                                SRMMessageBox.Show("Please set value less than 10!");
                                dgd_Position.Rows[4].Cells[1].Value = m_smVisionInfo.g_arrLead3D[1].ref_fBaseLineMaxAngle.ToString("f4");
                            }
                            m_smVisionInfo.g_arrLead3D[1].ref_fBaseLineMaxAngle = fMaxValue;
                        }
                        else
                        {
                            SRMMessageBox.Show("Please enter positive value!");
                            dgd_Position.Rows[4].Cells[1].Value = m_smVisionInfo.g_arrLead3D[1].ref_fBaseLineMaxAngle.ToString("f4");
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Please enter valid numerical value!");
                        dgd_Position.Rows[4].Cells[1].Value = m_smVisionInfo.g_arrLead3D[1].ref_fBaseLineMaxAngle.ToString("f4");
                    }
                    break;
                case 5:
                    if (float.TryParse(dgd_Position.Rows[5].Cells[1].Value.ToString(), out fMaxValue))
                    {
                        if (fMaxValue > 0)
                        {
                            if (fMaxValue > 10)
                            {
                                SRMMessageBox.Show("Please set value less than 10!");
                                dgd_Position.Rows[5].Cells[1].Value = m_smVisionInfo.g_arrLead3D[2].ref_fBaseLineMaxAngle.ToString("f4");
                            }
                            m_smVisionInfo.g_arrLead3D[2].ref_fBaseLineMaxAngle = fMaxValue;
                        }
                        else
                        {
                            SRMMessageBox.Show("Please enter positive value!");
                            dgd_Position.Rows[5].Cells[1].Value = m_smVisionInfo.g_arrLead3D[2].ref_fBaseLineMaxAngle.ToString("f4");
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Please enter valid numerical value!");
                        dgd_Position.Rows[5].Cells[1].Value = m_smVisionInfo.g_arrLead3D[2].ref_fBaseLineMaxAngle.ToString("f4");
                    }
                    break;
                case 6:
                    if (float.TryParse(dgd_Position.Rows[6].Cells[1].Value.ToString(), out fMaxValue))
                    {
                        if (fMaxValue > 0)
                        {
                            if (fMaxValue > 10)
                            {
                                SRMMessageBox.Show("Please set value less than 10!");
                                dgd_Position.Rows[6].Cells[1].Value = m_smVisionInfo.g_arrLead3D[3].ref_fBaseLineMaxAngle.ToString("f4");
                            }
                            m_smVisionInfo.g_arrLead3D[3].ref_fBaseLineMaxAngle = fMaxValue;
                        }
                        else
                        {
                            SRMMessageBox.Show("Please enter positive value!");
                            dgd_Position.Rows[6].Cells[1].Value = m_smVisionInfo.g_arrLead3D[3].ref_fBaseLineMaxAngle.ToString("f4");
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Please enter valid numerical value!");
                        dgd_Position.Rows[6].Cells[1].Value = m_smVisionInfo.g_arrLead3D[3].ref_fBaseLineMaxAngle.ToString("f4");
                    }
                    break;
                case 7:
                    if (float.TryParse(dgd_Position.Rows[7].Cells[1].Value.ToString(), out fMaxValue))
                    {
                        if (fMaxValue > 0)
                        {
                            if (fMaxValue > 10)
                            {
                                SRMMessageBox.Show("Please set value less than 10!");
                                dgd_Position.Rows[7].Cells[1].Value = m_smVisionInfo.g_arrLead3D[4].ref_fBaseLineMaxAngle.ToString("f4");
                            }
                            m_smVisionInfo.g_arrLead3D[4].ref_fBaseLineMaxAngle = fMaxValue;
                        }
                        else
                        {
                            SRMMessageBox.Show("Please enter positive value!");
                            dgd_Position.Rows[7].Cells[1].Value = m_smVisionInfo.g_arrLead3D[4].ref_fBaseLineMaxAngle.ToString("f4");
                        }
                    }
                    else
                    {
                        SRMMessageBox.Show("Please enter valid numerical value!");
                        dgd_Position.Rows[7].Cells[1].Value = m_smVisionInfo.g_arrLead3D[4].ref_fBaseLineMaxAngle.ToString("f4");
                    }
                    break;
            }


            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
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
                m_smVisionInfo.g_arrLead3D[0].SetBrightLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
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
                m_smVisionInfo.g_arrLead3D[0].SetDarkLengthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
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
                m_smVisionInfo.g_arrLead3D[0].SetBrightWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
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
                m_smVisionInfo.g_arrLead3D[0].SetDarkWidthLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
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
                m_smVisionInfo.g_arrLead3D[0].SetBrightAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
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
                m_smVisionInfo.g_arrLead3D[0].SetDarkAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
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
                m_smVisionInfo.g_arrLead3D[0].SetBrightTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
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
                m_smVisionInfo.g_arrLead3D[0].SetDarkTotalAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
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
                m_smVisionInfo.g_arrLead3D[0].SetBrightChippedOffAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
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
                m_smVisionInfo.g_arrLead3D[0].SetDarkChippedOffAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
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
                m_smVisionInfo.g_arrLead3D[0].SetDarkVerticalCrackLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
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
                m_smVisionInfo.g_arrLead3D[0].SetDarkHorizontalCrackLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }

            m_smVisionInfo.g_blnViewLead3DPackageDefectSetting = true;
            m_smVisionInfo.ALL_VM_UpdatePictureBox = true;
        }
        
        private void txt_CrackAreaDark_TextChanged(object sender, EventArgs e)
        {
            float fValue;
            if (txt_CrackAreaDark.Text != null && float.TryParse(txt_CrackAreaDark.Text.ToString(), out fValue))
            {
                m_smVisionInfo.g_arrLead3D[0].SetCrackAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }
        }
        
        private void txt_MoldFlashAreaBright_TextChanged(object sender, EventArgs e)
        {
            float fValue;
            if (txt_MoldFlashAreaBright.Text != null && float.TryParse(txt_MoldFlashAreaBright.Text.ToString(), out fValue))
            {
                m_smVisionInfo.g_arrLead3D[0].SetMoldFlashAreaLimit(fValue, m_smCustomizeInfo.g_intUnitDisplay);
            }
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

        //private void txt_WidthOffset_TextChanged(object sender, EventArgs e)
        //{
        //    if (!m_blnInitDone)
        //        return;

        //    if (m_blnUpdateSelectedROISetting)
        //        return;

        //    float fValue = 0;
        //    if (float.TryParse(txt_WidthOffset.Text, out fValue))
        //    {
        //        m_smVisionInfo.g_arrLead3D[0].ref_fPackageWidthOffsetMM = fValue;
        //    }
        //}

        //private void txt_HeightOffset_TextChanged(object sender, EventArgs e)
        //{
        //    if (!m_blnInitDone)
        //        return;

        //    if (m_blnUpdateSelectedROISetting)
        //        return;

        //    float fValue = 0;
        //    if (float.TryParse(txt_HeightOffset.Text, out fValue))
        //    {
        //        m_smVisionInfo.g_arrLead3D[0].ref_fPackageHeightOffsetMM = fValue;
        //    }
        //}

        //private void txt_ThicknessOffset_TextChanged(object sender, EventArgs e)
        //{
        //    if (!m_blnInitDone)
        //        return;

        //    if (m_blnUpdateSelectedROISetting)
        //        return;

        //    float fValue = 0;
        //    if (float.TryParse(txt_ThicknessOffset.Text, out fValue))
        //    {
        //        for (int i = 1; i < m_smVisionInfo.g_arrLead3D.Length; i++)
        //            m_smVisionInfo.g_arrLead3D[i].ref_fPackageThicknessOffsetMM = fValue;
        //    }
        //}
    }
}
